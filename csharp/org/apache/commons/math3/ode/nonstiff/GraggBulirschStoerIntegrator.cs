using System;

/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace org.apache.commons.math3.ode.nonstiff
{

	using UnivariateSolver = org.apache.commons.math3.analysis.solvers.UnivariateSolver;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MaxCountExceededException = org.apache.commons.math3.exception.MaxCountExceededException;
	using NoBracketingException = org.apache.commons.math3.exception.NoBracketingException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using EventHandler = org.apache.commons.math3.ode.events.EventHandler;
	using AbstractStepInterpolator = org.apache.commons.math3.ode.sampling.AbstractStepInterpolator;
	using StepHandler = org.apache.commons.math3.ode.sampling.StepHandler;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// This class implements a Gragg-Bulirsch-Stoer integrator for
	/// Ordinary Differential Equations.
	/// 
	/// <p>The Gragg-Bulirsch-Stoer algorithm is one of the most efficient
	/// ones currently available for smooth problems. It uses Richardson
	/// extrapolation to estimate what would be the solution if the step
	/// size could be decreased down to zero.</p>
	/// 
	/// <p>
	/// This method changes both the step size and the order during
	/// integration, in order to minimize computation cost. It is
	/// particularly well suited when a very high precision is needed. The
	/// limit where this method becomes more efficient than high-order
	/// embedded Runge-Kutta methods like {@link DormandPrince853Integrator
	/// Dormand-Prince 8(5,3)} depends on the problem. Results given in the
	/// Hairer, Norsett and Wanner book show for example that this limit
	/// occurs for accuracy around 1e-6 when integrating Saltzam-Lorenz
	/// equations (the authors note this problem is <i>extremely sensitive
	/// to the errors in the first integration steps</i>), and around 1e-11
	/// for a two dimensional celestial mechanics problems with seven
	/// bodies (pleiades problem, involving quasi-collisions for which
	/// <i>automatic step size control is essential</i>).
	/// </p>
	/// 
	/// <p>
	/// This implementation is basically a reimplementation in Java of the
	/// <a
	/// href="http://www.unige.ch/math/folks/hairer/prog/nonstiff/odex.f">odex</a>
	/// fortran code by E. Hairer and G. Wanner. The redistribution policy
	/// for this code is available <a
	/// href="http://www.unige.ch/~hairer/prog/licence.txt">here</a>, for
	/// convenience, it is reproduced below.</p>
	/// </p>
	/// 
	/// <table border="0" width="80%" cellpadding="10" align="center" bgcolor="#E0E0E0">
	/// <tr><td>Copyright (c) 2004, Ernst Hairer</td></tr>
	/// 
	/// <tr><td>Redistribution and use in source and binary forms, with or
	/// without modification, are permitted provided that the following
	/// conditions are met:
	/// <ul>
	///  <li>Redistributions of source code must retain the above copyright
	///      notice, this list of conditions and the following disclaimer.</li>
	///  <li>Redistributions in binary form must reproduce the above copyright
	///      notice, this list of conditions and the following disclaimer in the
	///      documentation and/or other materials provided with the distribution.</li>
	/// </ul></td></tr>
	/// 
	/// <tr><td><strong>THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
	/// CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,
	/// BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
	/// FOR A  PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE REGENTS OR
	/// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
	/// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
	/// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
	/// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
	/// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
	/// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
	/// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.</strong></td></tr>
	/// </table>
	/// 
	/// @version $Id: GraggBulirschStoerIntegrator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2
	/// </summary>

	public class GraggBulirschStoerIntegrator : AdaptiveStepsizeIntegrator
	{

		/// <summary>
		/// Integrator method name. </summary>
		private const string METHOD_NAME = "Gragg-Bulirsch-Stoer";

		/// <summary>
		/// maximal order. </summary>
		private int maxOrder;

		/// <summary>
		/// step size sequence. </summary>
		private int[] sequence;

		/// <summary>
		/// overall cost of applying step reduction up to iteration k+1, in number of calls. </summary>
		private int[] costPerStep;

		/// <summary>
		/// cost per unit step. </summary>
		private double[] costPerTimeUnit;

		/// <summary>
		/// optimal steps for each order. </summary>
		private double[] optimalStep;

		/// <summary>
		/// extrapolation coefficients. </summary>
		private double[][] coeff;

		/// <summary>
		/// stability check enabling parameter. </summary>
		private bool performTest;

		/// <summary>
		/// maximal number of checks for each iteration. </summary>
		private int maxChecks;

		/// <summary>
		/// maximal number of iterations for which checks are performed. </summary>
		private int maxIter;

		/// <summary>
		/// stepsize reduction factor in case of stability check failure. </summary>
		private double stabilityReduction;

		/// <summary>
		/// first stepsize control factor. </summary>
		private double stepControl1;

		/// <summary>
		/// second stepsize control factor. </summary>
		private double stepControl2;

		/// <summary>
		/// third stepsize control factor. </summary>
		private double stepControl3;

		/// <summary>
		/// fourth stepsize control factor. </summary>
		private double stepControl4;

		/// <summary>
		/// first order control factor. </summary>
		private double orderControl1;

		/// <summary>
		/// second order control factor. </summary>
		private double orderControl2;

		/// <summary>
		/// use interpolation error in stepsize control. </summary>
		private bool useInterpolationError;

		/// <summary>
		/// interpolation order control parameter. </summary>
		private int mudif;

	  /// <summary>
	  /// Simple constructor.
	  /// Build a Gragg-Bulirsch-Stoer integrator with the given step
	  /// bounds. All tuning parameters are set to their default
	  /// values. The default step handler does nothing. </summary>
	  /// <param name="minStep"> minimal step (sign is irrelevant, regardless of
	  /// integration direction, forward or backward), the last step can
	  /// be smaller than this </param>
	  /// <param name="maxStep"> maximal step (sign is irrelevant, regardless of
	  /// integration direction, forward or backward), the last step can
	  /// be smaller than this </param>
	  /// <param name="scalAbsoluteTolerance"> allowed absolute error </param>
	  /// <param name="scalRelativeTolerance"> allowed relative error </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public GraggBulirschStoerIntegrator(final double minStep, final double maxStep, final double scalAbsoluteTolerance, final double scalRelativeTolerance)
	  public GraggBulirschStoerIntegrator(double minStep, double maxStep, double scalAbsoluteTolerance, double scalRelativeTolerance) : base(METHOD_NAME, minStep, maxStep, scalAbsoluteTolerance, scalRelativeTolerance)
	  {
		setStabilityCheck(true, -1, -1, -1);
		setControlFactors(-1, -1, -1, -1);
		setOrderControl(-1, -1, -1);
		setInterpolationControl(true, -1);
	  }

	  /// <summary>
	  /// Simple constructor.
	  /// Build a Gragg-Bulirsch-Stoer integrator with the given step
	  /// bounds. All tuning parameters are set to their default
	  /// values. The default step handler does nothing. </summary>
	  /// <param name="minStep"> minimal step (must be positive even for backward
	  /// integration), the last step can be smaller than this </param>
	  /// <param name="maxStep"> maximal step (must be positive even for backward
	  /// integration) </param>
	  /// <param name="vecAbsoluteTolerance"> allowed absolute error </param>
	  /// <param name="vecRelativeTolerance"> allowed relative error </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public GraggBulirschStoerIntegrator(final double minStep, final double maxStep, final double[] vecAbsoluteTolerance, final double[] vecRelativeTolerance)
	  public GraggBulirschStoerIntegrator(double minStep, double maxStep, double[] vecAbsoluteTolerance, double[] vecRelativeTolerance) : base(METHOD_NAME, minStep, maxStep, vecAbsoluteTolerance, vecRelativeTolerance)
	  {
		setStabilityCheck(true, -1, -1, -1);
		setControlFactors(-1, -1, -1, -1);
		setOrderControl(-1, -1, -1);
		setInterpolationControl(true, -1);
	  }

	  /// <summary>
	  /// Set the stability check controls.
	  /// <p>The stability check is performed on the first few iterations of
	  /// the extrapolation scheme. If this test fails, the step is rejected
	  /// and the stepsize is reduced.</p>
	  /// <p>By default, the test is performed, at most during two
	  /// iterations at each step, and at most once for each of these
	  /// iterations. The default stepsize reduction factor is 0.5.</p> </summary>
	  /// <param name="performStabilityCheck"> if true, stability check will be performed,
	  ///   if false, the check will be skipped </param>
	  /// <param name="maxNumIter"> maximal number of iterations for which checks are
	  /// performed (the number of iterations is reset to default if negative
	  /// or null) </param>
	  /// <param name="maxNumChecks"> maximal number of checks for each iteration
	  /// (the number of checks is reset to default if negative or null) </param>
	  /// <param name="stepsizeReductionFactor"> stepsize reduction factor in case of
	  /// failure (the factor is reset to default if lower than 0.0001 or
	  /// greater than 0.9999) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setStabilityCheck(final boolean performStabilityCheck, final int maxNumIter, final int maxNumChecks, final double stepsizeReductionFactor)
	  public virtual void setStabilityCheck(bool performStabilityCheck, int maxNumIter, int maxNumChecks, double stepsizeReductionFactor)
	  {

		this.performTest = performStabilityCheck;
		this.maxIter = (maxNumIter <= 0) ? 2 : maxNumIter;
		this.maxChecks = (maxNumChecks <= 0) ? 1 : maxNumChecks;

		if ((stepsizeReductionFactor < 0.0001) || (stepsizeReductionFactor > 0.9999))
		{
		  this.stabilityReduction = 0.5;
		}
		else
		{
		  this.stabilityReduction = stepsizeReductionFactor;
		}

	  }

	  /// <summary>
	  /// Set the step size control factors.
	  /// 
	  /// <p>The new step size hNew is computed from the old one h by:
	  /// <pre>
	  /// hNew = h * stepControl2 / (err/stepControl1)^(1/(2k+1))
	  /// </pre>
	  /// where err is the scaled error and k the iteration number of the
	  /// extrapolation scheme (counting from 0). The default values are
	  /// 0.65 for stepControl1 and 0.94 for stepControl2.</p>
	  /// <p>The step size is subject to the restriction:
	  /// <pre>
	  /// stepControl3^(1/(2k+1))/stepControl4 <= hNew/h <= 1/stepControl3^(1/(2k+1))
	  /// </pre>
	  /// The default values are 0.02 for stepControl3 and 4.0 for
	  /// stepControl4.</p> </summary>
	  /// <param name="control1"> first stepsize control factor (the factor is
	  /// reset to default if lower than 0.0001 or greater than 0.9999) </param>
	  /// <param name="control2"> second stepsize control factor (the factor
	  /// is reset to default if lower than 0.0001 or greater than 0.9999) </param>
	  /// <param name="control3"> third stepsize control factor (the factor is
	  /// reset to default if lower than 0.0001 or greater than 0.9999) </param>
	  /// <param name="control4"> fourth stepsize control factor (the factor
	  /// is reset to default if lower than 1.0001 or greater than 999.9) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setControlFactors(final double control1, final double control2, final double control3, final double control4)
	  public virtual void setControlFactors(double control1, double control2, double control3, double control4)
	  {

		if ((control1 < 0.0001) || (control1 > 0.9999))
		{
		  this.stepControl1 = 0.65;
		}
		else
		{
		  this.stepControl1 = control1;
		}

		if ((control2 < 0.0001) || (control2 > 0.9999))
		{
		  this.stepControl2 = 0.94;
		}
		else
		{
		  this.stepControl2 = control2;
		}

		if ((control3 < 0.0001) || (control3 > 0.9999))
		{
		  this.stepControl3 = 0.02;
		}
		else
		{
		  this.stepControl3 = control3;
		}

		if ((control4 < 1.0001) || (control4 > 999.9))
		{
		  this.stepControl4 = 4.0;
		}
		else
		{
		  this.stepControl4 = control4;
		}

	  }

	  /// <summary>
	  /// Set the order control parameters.
	  /// <p>The Gragg-Bulirsch-Stoer method changes both the step size and
	  /// the order during integration, in order to minimize computation
	  /// cost. Each extrapolation step increases the order by 2, so the
	  /// maximal order that will be used is always even, it is twice the
	  /// maximal number of columns in the extrapolation table.</p>
	  /// <pre>
	  /// order is decreased if w(k-1) <= w(k)   * orderControl1
	  /// order is increased if w(k)   <= w(k-1) * orderControl2
	  /// </pre>
	  /// <p>where w is the table of work per unit step for each order
	  /// (number of function calls divided by the step length), and k is
	  /// the current order.</p>
	  /// <p>The default maximal order after construction is 18 (i.e. the
	  /// maximal number of columns is 9). The default values are 0.8 for
	  /// orderControl1 and 0.9 for orderControl2.</p> </summary>
	  /// <param name="maximalOrder"> maximal order in the extrapolation table (the
	  /// maximal order is reset to default if order <= 6 or odd) </param>
	  /// <param name="control1"> first order control factor (the factor is
	  /// reset to default if lower than 0.0001 or greater than 0.9999) </param>
	  /// <param name="control2"> second order control factor (the factor
	  /// is reset to default if lower than 0.0001 or greater than 0.9999) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setOrderControl(final int maximalOrder, final double control1, final double control2)
	  public virtual void setOrderControl(int maximalOrder, double control1, double control2)
	  {

		if ((maximalOrder <= 6) || (maximalOrder % 2 != 0))
		{
		  this.maxOrder = 18;
		}

		if ((control1 < 0.0001) || (control1 > 0.9999))
		{
		  this.orderControl1 = 0.8;
		}
		else
		{
		  this.orderControl1 = control1;
		}

		if ((control2 < 0.0001) || (control2 > 0.9999))
		{
		  this.orderControl2 = 0.9;
		}
		else
		{
		  this.orderControl2 = control2;
		}

		// reinitialize the arrays
		initializeArrays();

	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void addStepHandler(final org.apache.commons.math3.ode.sampling.StepHandler handler)
	  public override void addStepHandler(StepHandler handler)
	  {

		base.addStepHandler(handler);

		// reinitialize the arrays
		initializeArrays();

	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void addEventHandler(final org.apache.commons.math3.ode.events.EventHandler function, final double maxCheckInterval, final double convergence, final int maxIterationCount, final org.apache.commons.math3.analysis.solvers.UnivariateSolver solver)
	  public override void addEventHandler(EventHandler function, double maxCheckInterval, double convergence, int maxIterationCount, UnivariateSolver solver)
	  {
		base.addEventHandler(function, maxCheckInterval, convergence, maxIterationCount, solver);

		// reinitialize the arrays
		initializeArrays();

	  }

	  /// <summary>
	  /// Initialize the integrator internal arrays. </summary>
	  private void initializeArrays()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = maxOrder / 2;
		int size = maxOrder / 2;

		if ((sequence == null) || (sequence.Length != size))
		{
		  // all arrays should be reallocated with the right size
		  sequence = new int[size];
		  costPerStep = new int[size];
		  coeff = new double[size][];
		  costPerTimeUnit = new double[size];
		  optimalStep = new double[size];
		}

		// step size sequence: 2, 6, 10, 14, ...
		for (int k = 0; k < size; ++k)
		{
			sequence[k] = 4 * k + 2;
		}

		// initialize the order selection cost array
		// (number of function calls for each column of the extrapolation table)
		costPerStep[0] = sequence[0] + 1;
		for (int k = 1; k < size; ++k)
		{
		  costPerStep[k] = costPerStep[k - 1] + sequence[k];
		}

		// initialize the extrapolation tables
		for (int k = 0; k < size; ++k)
		{
		  coeff[k] = (k > 0) ? new double[k] : null;
		  for (int l = 0; l < k; ++l)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ratio = ((double) sequence[k]) / sequence[k-l-1];
			double ratio = ((double) sequence[k]) / sequence[k - l - 1];
			coeff[k][l] = 1.0 / (ratio * ratio - 1.0);
		  }
		}

	  }

	  /// <summary>
	  /// Set the interpolation order control parameter.
	  /// The interpolation order for dense output is 2k - mudif + 1. The
	  /// default value for mudif is 4 and the interpolation error is used
	  /// in stepsize control by default.
	  /// </summary>
	  /// <param name="useInterpolationErrorForControl"> if true, interpolation error is used
	  /// for stepsize control </param>
	  /// <param name="mudifControlParameter"> interpolation order control parameter (the parameter
	  /// is reset to default if <= 0 or >= 7) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setInterpolationControl(final boolean useInterpolationErrorForControl, final int mudifControlParameter)
	  public virtual void setInterpolationControl(bool useInterpolationErrorForControl, int mudifControlParameter)
	  {

		this.useInterpolationError = useInterpolationErrorForControl;

		if ((mudifControlParameter <= 0) || (mudifControlParameter >= 7))
		{
		  this.mudif = 4;
		}
		else
		{
		  this.mudif = mudifControlParameter;
		}

	  }

	  /// <summary>
	  /// Update scaling array. </summary>
	  /// <param name="y1"> first state vector to use for scaling </param>
	  /// <param name="y2"> second state vector to use for scaling </param>
	  /// <param name="scale"> scaling array to update (can be shorter than state) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void rescale(final double[] y1, final double[] y2, final double[] scale)
	  private void rescale(double[] y1, double[] y2, double[] scale)
	  {
		if (vecAbsoluteTolerance == null)
		{
		  for (int i = 0; i < scale.Length; ++i)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yi = org.apache.commons.math3.util.FastMath.max(org.apache.commons.math3.util.FastMath.abs(y1[i]), org.apache.commons.math3.util.FastMath.abs(y2[i]));
			double yi = FastMath.max(FastMath.abs(y1[i]), FastMath.abs(y2[i]));
			scale[i] = scalAbsoluteTolerance + scalRelativeTolerance * yi;
		  }
		}
		else
		{
		  for (int i = 0; i < scale.Length; ++i)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yi = org.apache.commons.math3.util.FastMath.max(org.apache.commons.math3.util.FastMath.abs(y1[i]), org.apache.commons.math3.util.FastMath.abs(y2[i]));
			double yi = FastMath.max(FastMath.abs(y1[i]), FastMath.abs(y2[i]));
			scale[i] = vecAbsoluteTolerance[i] + vecRelativeTolerance[i] * yi;
		  }
		}
	  }

	  /// <summary>
	  /// Perform integration over one step using substeps of a modified
	  /// midpoint method. </summary>
	  /// <param name="t0"> initial time </param>
	  /// <param name="y0"> initial value of the state vector at t0 </param>
	  /// <param name="step"> global step </param>
	  /// <param name="k"> iteration number (from 0 to sequence.length - 1) </param>
	  /// <param name="scale"> scaling array (can be shorter than state) </param>
	  /// <param name="f"> placeholder where to put the state vector derivatives at each substep
	  ///          (element 0 already contains initial derivative) </param>
	  /// <param name="yMiddle"> placeholder where to put the state vector at the middle of the step </param>
	  /// <param name="yEnd"> placeholder where to put the state vector at the end </param>
	  /// <param name="yTmp"> placeholder for one state vector </param>
	  /// <returns> true if computation was done properly,
	  ///         false if stability check failed before end of computation </returns>
	  /// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
	  /// <exception cref="DimensionMismatchException"> if arrays dimensions do not match equations settings </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean tryStep(final double t0, final double[] y0, final double step, final int k, final double[] scale, final double[][] f, final double[] yMiddle, final double[] yEnd, final double[] yTmp) throws org.apache.commons.math3.exception.MaxCountExceededException, org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  private bool tryStep(double t0, double[] y0, double step, int k, double[] scale, double[][] f, double[] yMiddle, double[] yEnd, double[] yTmp)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = sequence[k];
		int n = sequence[k];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double subStep = step / n;
		double subStep = step / n;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double subStep2 = 2 * subStep;
		double subStep2 = 2 * subStep;

		// first substep
		double t = t0 + subStep;
		for (int i = 0; i < y0.Length; ++i)
		{
		  yTmp[i] = y0[i];
		  yEnd[i] = y0[i] + subStep * f[0][i];
		}
		computeDerivatives(t, yEnd, f[1]);

		// other substeps
		for (int j = 1; j < n; ++j)
		{

		  if (2 * j == n)
		  {
			// save the point at the middle of the step
			Array.Copy(yEnd, 0, yMiddle, 0, y0.Length);
		  }

		  t += subStep;
		  for (int i = 0; i < y0.Length; ++i)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double middle = yEnd[i];
			double middle = yEnd[i];
			yEnd[i] = yTmp[i] + subStep2 * f[j][i];
			yTmp[i] = middle;
		  }

		  computeDerivatives(t, yEnd, f[j + 1]);

		  // stability check
		  if (performTest && (j <= maxChecks) && (k < maxIter))
		  {
			double initialNorm = 0.0;
			for (int l = 0; l < scale.Length; ++l)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ratio = f[0][l] / scale[l];
			  double ratio = f[0][l] / scale[l];
			  initialNorm += ratio * ratio;
			}
			double deltaNorm = 0.0;
			for (int l = 0; l < scale.Length; ++l)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ratio = (f[j+1][l] - f[0][l]) / scale[l];
			  double ratio = (f[j + 1][l] - f[0][l]) / scale[l];
			  deltaNorm += ratio * ratio;
			}
			if (deltaNorm > 4 * FastMath.max(1.0e-15, initialNorm))
			{
			  return false;
			}
		  }

		}

		// correction of the last substep (at t0 + step)
		for (int i = 0; i < y0.Length; ++i)
		{
		  yEnd[i] = 0.5 * (yTmp[i] + yEnd[i] + subStep * f[n][i]);
		}

		return true;

	  }

	  /// <summary>
	  /// Extrapolate a vector. </summary>
	  /// <param name="offset"> offset to use in the coefficients table </param>
	  /// <param name="k"> index of the last updated point </param>
	  /// <param name="diag"> working diagonal of the Aitken-Neville's
	  /// triangle, without the last element </param>
	  /// <param name="last"> last element </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void extrapolate(final int offset, final int k, final double[][] diag, final double[] last)
	  private void extrapolate(int offset, int k, double[][] diag, double[] last)
	  {

		// update the diagonal
		for (int j = 1; j < k; ++j)
		{
		  for (int i = 0; i < last.Length; ++i)
		  {
			// Aitken-Neville's recursive formula
			diag[k - j - 1][i] = diag[k - j][i] + coeff[k + offset][j - 1] * (diag[k - j][i] - diag[k - j - 1][i]);
		  }
		}

		// update the last element
		for (int i = 0; i < last.Length; ++i)
		{
		  // Aitken-Neville's recursive formula
		  last[i] = diag[0][i] + coeff[k + offset][k - 1] * (diag[0][i] - last[i]);
		}
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void integrate(final org.apache.commons.math3.ode.ExpandableStatefulODE equations, final double t) throws org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MaxCountExceededException, org.apache.commons.math3.exception.NoBracketingException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  public override void integrate(ExpandableStatefulODE equations, double t)
	  {

		sanityChecks(equations, t);
		Equations = equations;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean forward = t > equations.getTime();
		bool forward = t > equations.Time;

		// create some internal working arrays
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y0 = equations.getCompleteState();
		double[] y0 = equations.CompleteState;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y = y0.clone();
		double[] y = y0.clone();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yDot0 = new double[y.length];
		double[] yDot0 = new double[y.Length];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y1 = new double[y.length];
		double[] y1 = new double[y.Length];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yTmp = new double[y.length];
		double[] yTmp = new double[y.Length];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yTmpDot = new double[y.length];
		double[] yTmpDot = new double[y.Length];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] diagonal = new double[sequence.length-1][];
		double[][] diagonal = new double[sequence.Length - 1][];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] y1Diag = new double[sequence.length-1][];
		double[][] y1Diag = new double[sequence.Length - 1][];
		for (int k = 0; k < sequence.Length - 1; ++k)
		{
		  diagonal[k] = new double[y.Length];
		  y1Diag[k] = new double[y.Length];
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][][] fk = new double[sequence.length][][];
		double[][][] fk = new double[sequence.Length][][];
		for (int k = 0; k < sequence.Length; ++k)
		{

		  fk[k] = new double[sequence[k] + 1][];

		  // all substeps start at the same point, so share the first array
		  fk[k][0] = yDot0;

		  for (int l = 0; l < sequence[k]; ++l)
		  {
			fk[k][l + 1] = new double[y0.Length];
		  }

		}

		if (y != y0)
		{
		  Array.Copy(y0, 0, y, 0, y0.Length);
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yDot1 = new double[y0.length];
		double[] yDot1 = new double[y0.Length];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] yMidDots = new double[1 + 2 * sequence.length][y0.length];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] yMidDots = new double[1 + 2 * sequence.Length][y0.Length];
		double[][] yMidDots = RectangularArrays.ReturnRectangularDoubleArray(1 + 2 * sequence.Length, y0.Length);

		// initial scaling
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] scale = new double[mainSetDimension];
		double[] scale = new double[mainSetDimension];
		rescale(y, y, scale);

		// initial order selection
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tol = (vecRelativeTolerance == null) ? scalRelativeTolerance : vecRelativeTolerance[0];
		double tol = (vecRelativeTolerance == null) ? scalRelativeTolerance : vecRelativeTolerance[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double log10R = org.apache.commons.math3.util.FastMath.log10(org.apache.commons.math3.util.FastMath.max(1.0e-10, tol));
		double log10R = FastMath.log10(FastMath.max(1.0e-10, tol));
		int targetIter = FastMath.max(1, FastMath.min(sequence.Length - 2, (int) FastMath.floor(0.5 - 0.6 * log10R)));

		// set up an interpolator sharing the integrator arrays
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.ode.sampling.AbstractStepInterpolator interpolator = new GraggBulirschStoerStepInterpolator(y, yDot0, y1, yDot1, yMidDots, forward, equations.getPrimaryMapper(), equations.getSecondaryMappers());
		AbstractStepInterpolator interpolator = new GraggBulirschStoerStepInterpolator(y, yDot0, y1, yDot1, yMidDots, forward, equations.PrimaryMapper, equations.SecondaryMappers);
		interpolator.storeTime(equations.Time);

		stepStart = equations.Time;
		double hNew = 0;
		double maxError = double.MaxValue;
		bool previousRejected = false;
		bool firstTime = true;
		bool newStep = true;
		bool firstStepAlreadyComputed = false;
		initIntegration(equations.Time, y0, t);
		costPerTimeUnit[0] = 0;
		isLastStep = false;
		do
		{

		  double error;
		  bool reject = false;

		  if (newStep)
		  {

			interpolator.shift();

			// first evaluation, at the beginning of the step
			if (!firstStepAlreadyComputed)
			{
			  computeDerivatives(stepStart, y, yDot0);
			}

			if (firstTime)
			{
			  hNew = initializeStep(forward, 2 * targetIter + 1, scale, stepStart, y, yDot0, yTmp, yTmpDot);
			}

			newStep = false;

		  }

		  stepSize = hNew;

		  // step adjustment near bounds
		  if ((forward && (stepStart + stepSize > t)) || ((!forward) && (stepStart + stepSize < t)))
		  {
			stepSize = t - stepStart;
		  }
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double nextT = stepStart + stepSize;
		  double nextT = stepStart + stepSize;
		  isLastStep = forward ? (nextT >= t) : (nextT <= t);

		  // iterate over several substep sizes
		  int k = -1;
		  for (bool loop = true; loop;)
		  {

			++k;

			// modified midpoint integration with the current substep
			if (!tryStep(stepStart, y, stepSize, k, scale, fk[k], (k == 0) ? yMidDots[0] : diagonal[k - 1], (k == 0) ? y1 : y1Diag[k - 1], yTmp))
			{

			  // the stability check failed, we reduce the global step
			  hNew = FastMath.abs(filterStep(stepSize * stabilityReduction, forward, false));
			  reject = true;
			  loop = false;

			}
			else
			{

			  // the substep was computed successfully
			  if (k > 0)
			  {

				// extrapolate the state at the end of the step
				// using last iteration data
				extrapolate(0, k, y1Diag, y1);
				rescale(y, y1, scale);

				// estimate the error at the end of the step.
				error = 0;
				for (int j = 0; j < mainSetDimension; ++j)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double e = org.apache.commons.math3.util.FastMath.abs(y1[j] - y1Diag[0][j]) / scale[j];
				  double e = FastMath.abs(y1[j] - y1Diag[0][j]) / scale[j];
				  error += e * e;
				}
				error = FastMath.sqrt(error / mainSetDimension);

				if ((error > 1.0e15) || ((k > 1) && (error > maxError)))
				{
				  // error is too big, we reduce the global step
				  hNew = FastMath.abs(filterStep(stepSize * stabilityReduction, forward, false));
				  reject = true;
				  loop = false;
				}
				else
				{

				  maxError = FastMath.max(4 * error, 1.0);

				  // compute optimal stepsize for this order
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double exp = 1.0 / (2 * k + 1);
				  double exp = 1.0 / (2 * k + 1);
				  double fac = stepControl2 / FastMath.pow(error / stepControl1, exp);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double pow = org.apache.commons.math3.util.FastMath.pow(stepControl3, exp);
				  double pow = FastMath.pow(stepControl3, exp);
				  fac = FastMath.max(pow / stepControl4, FastMath.min(1 / pow, fac));
				  optimalStep[k] = FastMath.abs(filterStep(stepSize * fac, forward, true));
				  costPerTimeUnit[k] = costPerStep[k] / optimalStep[k];

				  // check convergence
				  switch (k - targetIter)
				  {

				  case -1 :
					if ((targetIter > 1) && !previousRejected)
					{

					  // check if we can stop iterations now
					  if (error <= 1.0)
					  {
						// convergence have been reached just before targetIter
						loop = false;
					  }
					  else
					  {
						// estimate if there is a chance convergence will
						// be reached on next iteration, using the
						// asymptotic evolution of error
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ratio = ((double) sequence [targetIter] * sequence[targetIter + 1]) / (sequence[0] * sequence[0]);
						double ratio = ((double) sequence [targetIter] * sequence[targetIter + 1]) / (sequence[0] * sequence[0]);
						if (error > ratio * ratio)
						{
						  // we don't expect to converge on next iteration
						  // we reject the step immediately and reduce order
						  reject = true;
						  loop = false;
						  targetIter = k;
						  if ((targetIter > 1) && (costPerTimeUnit[targetIter - 1] < orderControl1 * costPerTimeUnit[targetIter]))
						  {
							--targetIter;
						  }
						  hNew = optimalStep[targetIter];
						}
					  }
					}
					break;

				  case 0:
					if (error <= 1.0)
					{
					  // convergence has been reached exactly at targetIter
					  loop = false;
					}
					else
					{
					  // estimate if there is a chance convergence will
					  // be reached on next iteration, using the
					  // asymptotic evolution of error
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ratio = ((double) sequence[k+1]) / sequence[0];
					  double ratio = ((double) sequence[k + 1]) / sequence[0];
					  if (error > ratio * ratio)
					  {
						// we don't expect to converge on next iteration
						// we reject the step immediately
						reject = true;
						loop = false;
						if ((targetIter > 1) && (costPerTimeUnit[targetIter - 1] < orderControl1 * costPerTimeUnit[targetIter]))
						{
						  --targetIter;
						}
						hNew = optimalStep[targetIter];
					  }
					}
					break;

				  case 1 :
					if (error > 1.0)
					{
					  reject = true;
					  if ((targetIter > 1) && (costPerTimeUnit[targetIter - 1] < orderControl1 * costPerTimeUnit[targetIter]))
					  {
						--targetIter;
					  }
					  hNew = optimalStep[targetIter];
					}
					loop = false;
					break;

				  default :
					if ((firstTime || isLastStep) && (error <= 1.0))
					{
					  loop = false;
					}
					break;

				  }

				}
			  }
			}
		  }

		  if (!reject)
		  {
			  // derivatives at end of step
			  computeDerivatives(stepStart + stepSize, y1, yDot1);
		  }

		  // dense output handling
		  double hInt = MaxStep;
		  if (!reject)
		  {

			// extrapolate state at middle point of the step
			for (int j = 1; j <= k; ++j)
			{
			  extrapolate(0, j, diagonal, yMidDots[0]);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int mu = 2 * k - mudif + 3;
			int mu = 2 * k - mudif + 3;

			for (int l = 0; l < mu; ++l)
			{

			  // derivative at middle point of the step
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int l2 = l / 2;
			  int l2 = l / 2;
			  double factor = FastMath.pow(0.5 * sequence[l2], l);
			  int middleIndex = fk[l2].Length / 2;
			  for (int i = 0; i < y0.Length; ++i)
			  {
				yMidDots[l + 1][i] = factor * fk[l2][middleIndex + l][i];
			  }
			  for (int j = 1; j <= k - l2; ++j)
			  {
				factor = FastMath.pow(0.5 * sequence[j + l2], l);
				middleIndex = fk[l2 + j].Length / 2;
				for (int i = 0; i < y0.Length; ++i)
				{
				  diagonal[j - 1][i] = factor * fk[l2 + j][middleIndex + l][i];
				}
				extrapolate(l2, j, diagonal, yMidDots[l + 1]);
			  }
			  for (int i = 0; i < y0.Length; ++i)
			  {
				yMidDots[l + 1][i] *= stepSize;
			  }

			  // compute centered differences to evaluate next derivatives
			  for (int j = (l + 1) / 2; j <= k; ++j)
			  {
				for (int m = fk[j].Length - 1; m >= 2 * (l + 1); --m)
				{
				  for (int i = 0; i < y0.Length; ++i)
				  {
					fk[j][m][i] -= fk[j][m - 2][i];
				  }
				}
			  }

			}

			if (mu >= 0)
			{

			  // estimate the dense output coefficients
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final GraggBulirschStoerStepInterpolator gbsInterpolator = (GraggBulirschStoerStepInterpolator) interpolator;
			  GraggBulirschStoerStepInterpolator gbsInterpolator = (GraggBulirschStoerStepInterpolator) interpolator;
			  gbsInterpolator.computeCoefficients(mu, stepSize);

			  if (useInterpolationError)
			  {
				// use the interpolation error to limit stepsize
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double interpError = gbsInterpolator.estimateError(scale);
				double interpError = gbsInterpolator.estimateError(scale);
				hInt = FastMath.abs(stepSize / FastMath.max(FastMath.pow(interpError, 1.0 / (mu + 4)), 0.01));
				if (interpError > 10.0)
				{
				  hNew = hInt;
				  reject = true;
				}
			  }

			}

		  }

		  if (!reject)
		  {

			// Discrete events handling
			interpolator.storeTime(stepStart + stepSize);
			stepStart = acceptStep(interpolator, y1, yDot1, t);

			// prepare next step
			interpolator.storeTime(stepStart);
			Array.Copy(y1, 0, y, 0, y0.Length);
			Array.Copy(yDot1, 0, yDot0, 0, y0.Length);
			firstStepAlreadyComputed = true;

			int optimalIter;
			if (k == 1)
			{
			  optimalIter = 2;
			  if (previousRejected)
			  {
				optimalIter = 1;
			  }
			}
			else if (k <= targetIter)
			{
			  optimalIter = k;
			  if (costPerTimeUnit[k - 1] < orderControl1 * costPerTimeUnit[k])
			  {
				optimalIter = k - 1;
			  }
			  else if (costPerTimeUnit[k] < orderControl2 * costPerTimeUnit[k - 1])
			  {
				optimalIter = FastMath.min(k + 1, sequence.Length - 2);
			  }
			}
			else
			{
			  optimalIter = k - 1;
			  if ((k > 2) && (costPerTimeUnit[k - 2] < orderControl1 * costPerTimeUnit[k - 1]))
			  {
				optimalIter = k - 2;
			  }
			  if (costPerTimeUnit[k] < orderControl2 * costPerTimeUnit[optimalIter])
			  {
				optimalIter = FastMath.min(k, sequence.Length - 2);
			  }
			}

			if (previousRejected)
			{
			  // after a rejected step neither order nor stepsize
			  // should increase
			  targetIter = FastMath.min(optimalIter, k);
			  hNew = FastMath.min(FastMath.abs(stepSize), optimalStep[targetIter]);
			}
			else
			{
			  // stepsize control
			  if (optimalIter <= k)
			  {
				hNew = optimalStep[optimalIter];
			  }
			  else
			  {
				if ((k < targetIter) && (costPerTimeUnit[k] < orderControl2 * costPerTimeUnit[k - 1]))
				{
				  hNew = filterStep(optimalStep[k] * costPerStep[optimalIter + 1] / costPerStep[k], forward, false);
				}
				else
				{
				  hNew = filterStep(optimalStep[k] * costPerStep[optimalIter] / costPerStep[k], forward, false);
				}
			  }

			  targetIter = optimalIter;

			}

			newStep = true;

		  }

		  hNew = FastMath.min(hNew, hInt);
		  if (!forward)
		  {
			hNew = -hNew;
		  }

		  firstTime = false;

		  if (reject)
		  {
			isLastStep = false;
			previousRejected = true;
		  }
		  else
		  {
			previousRejected = false;
		  }

		} while (!isLastStep);

		// dispatch results
		equations.Time = stepStart;
		equations.CompleteState = y;

		resetInternalState();

	  }

	}

}
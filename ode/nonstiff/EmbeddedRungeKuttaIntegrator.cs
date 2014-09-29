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

namespace mathlib.ode.nonstiff
{

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using NoBracketingException = mathlib.exception.NoBracketingException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// This class implements the common part of all embedded Runge-Kutta
	/// integrators for Ordinary Differential Equations.
	/// 
	/// <p>These methods are embedded explicit Runge-Kutta methods with two
	/// sets of coefficients allowing to estimate the error, their Butcher
	/// arrays are as follows :
	/// <pre>
	///    0  |
	///   c2  | a21
	///   c3  | a31  a32
	///   ... |        ...
	///   cs  | as1  as2  ...  ass-1
	///       |--------------------------
	///       |  b1   b2  ...   bs-1  bs
	///       |  b'1  b'2 ...   b's-1 b's
	/// </pre>
	/// </p>
	/// 
	/// <p>In fact, we rather use the array defined by ej = bj - b'j to
	/// compute directly the error rather than computing two estimates and
	/// then comparing them.</p>
	/// 
	/// <p>Some methods are qualified as <i>fsal</i> (first same as last)
	/// methods. This means the last evaluation of the derivatives in one
	/// step is the same as the first in the next step. Then, this
	/// evaluation can be reused from one step to the next one and the cost
	/// of such a method is really s-1 evaluations despite the method still
	/// has s stages. This behaviour is true only for successful steps, if
	/// the step is rejected after the error estimation phase, no
	/// evaluation is saved. For an <i>fsal</i> method, we have cs = 1 and
	/// asi = bi for all i.</p>
	/// 
	/// @version $Id: EmbeddedRungeKuttaIntegrator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2
	/// </summary>

	public abstract class EmbeddedRungeKuttaIntegrator : AdaptiveStepsizeIntegrator
	{

		/// <summary>
		/// Indicator for <i>fsal</i> methods. </summary>
		private readonly bool fsal;

		/// <summary>
		/// Time steps from Butcher array (without the first zero). </summary>
		private readonly double[] c;

		/// <summary>
		/// Internal weights from Butcher array (without the first empty row). </summary>
		private readonly double[][] a;

		/// <summary>
		/// External weights for the high order method from Butcher array. </summary>
		private readonly double[] b;

		/// <summary>
		/// Prototype of the step interpolator. </summary>
		private readonly RungeKuttaStepInterpolator prototype;

		/// <summary>
		/// Stepsize control exponent. </summary>
		private readonly double exp;

		/// <summary>
		/// Safety factor for stepsize control. </summary>
		private double safety;

		/// <summary>
		/// Minimal reduction factor for stepsize control. </summary>
		private double minReduction;

		/// <summary>
		/// Maximal growth factor for stepsize control. </summary>
		private double maxGrowth;

	  /// <summary>
	  /// Build a Runge-Kutta integrator with the given Butcher array. </summary>
	  /// <param name="name"> name of the method </param>
	  /// <param name="fsal"> indicate that the method is an <i>fsal</i> </param>
	  /// <param name="c"> time steps from Butcher array (without the first zero) </param>
	  /// <param name="a"> internal weights from Butcher array (without the first empty row) </param>
	  /// <param name="b"> propagation weights for the high order method from Butcher array </param>
	  /// <param name="prototype"> prototype of the step interpolator to use </param>
	  /// <param name="minStep"> minimal step (sign is irrelevant, regardless of
	  /// integration direction, forward or backward), the last step can
	  /// be smaller than this </param>
	  /// <param name="maxStep"> maximal step (sign is irrelevant, regardless of
	  /// integration direction, forward or backward), the last step can
	  /// be smaller than this </param>
	  /// <param name="scalAbsoluteTolerance"> allowed absolute error </param>
	  /// <param name="scalRelativeTolerance"> allowed relative error </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected EmbeddedRungeKuttaIntegrator(final String name, final boolean fsal, final double[] c, final double[][] a, final double[] b, final RungeKuttaStepInterpolator prototype, final double minStep, final double maxStep, final double scalAbsoluteTolerance, final double scalRelativeTolerance)
	  protected internal EmbeddedRungeKuttaIntegrator(string name, bool fsal, double[] c, double[][] a, double[] b, RungeKuttaStepInterpolator prototype, double minStep, double maxStep, double scalAbsoluteTolerance, double scalRelativeTolerance) : base(name, minStep, maxStep, scalAbsoluteTolerance, scalRelativeTolerance)
	  {


		this.fsal = fsal;
		this.c = c;
		this.a = a;
		this.b = b;
		this.prototype = prototype;

		exp = -1.0 / Order;

		// set the default values of the algorithm control parameters
		Safety = 0.9;
		MinReduction = 0.2;
		MaxGrowth = 10.0;

	  }

	  /// <summary>
	  /// Build a Runge-Kutta integrator with the given Butcher array. </summary>
	  /// <param name="name"> name of the method </param>
	  /// <param name="fsal"> indicate that the method is an <i>fsal</i> </param>
	  /// <param name="c"> time steps from Butcher array (without the first zero) </param>
	  /// <param name="a"> internal weights from Butcher array (without the first empty row) </param>
	  /// <param name="b"> propagation weights for the high order method from Butcher array </param>
	  /// <param name="prototype"> prototype of the step interpolator to use </param>
	  /// <param name="minStep"> minimal step (must be positive even for backward
	  /// integration), the last step can be smaller than this </param>
	  /// <param name="maxStep"> maximal step (must be positive even for backward
	  /// integration) </param>
	  /// <param name="vecAbsoluteTolerance"> allowed absolute error </param>
	  /// <param name="vecRelativeTolerance"> allowed relative error </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected EmbeddedRungeKuttaIntegrator(final String name, final boolean fsal, final double[] c, final double[][] a, final double[] b, final RungeKuttaStepInterpolator prototype, final double minStep, final double maxStep, final double[] vecAbsoluteTolerance, final double[] vecRelativeTolerance)
	  protected internal EmbeddedRungeKuttaIntegrator(string name, bool fsal, double[] c, double[][] a, double[] b, RungeKuttaStepInterpolator prototype, double minStep, double maxStep, double[] vecAbsoluteTolerance, double[] vecRelativeTolerance) : base(name, minStep, maxStep, vecAbsoluteTolerance, vecRelativeTolerance)
	  {


		this.fsal = fsal;
		this.c = c;
		this.a = a;
		this.b = b;
		this.prototype = prototype;

		exp = -1.0 / Order;

		// set the default values of the algorithm control parameters
		Safety = 0.9;
		MinReduction = 0.2;
		MaxGrowth = 10.0;

	  }

	  /// <summary>
	  /// Get the order of the method. </summary>
	  /// <returns> order of the method </returns>
	  public abstract int Order {get;}

	  /// <summary>
	  /// Get the safety factor for stepsize control. </summary>
	  /// <returns> safety factor </returns>
	  public virtual double Safety
	  {
		  get
		  {
			return safety;
		  }
		  set
		  {
			this.safety = value;
		  }
	  }

	  /// <summary>
	  /// Set the safety factor for stepsize control. </summary>
	  /// <param name="safety"> safety factor </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setSafety(final double safety)

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void integrate(final mathlib.ode.ExpandableStatefulODE equations, final double t) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.DimensionMismatchException, mathlib.exception.MaxCountExceededException, mathlib.exception.NoBracketingException
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
//ORIGINAL LINE: final int stages = c.length + 1;
		int stages = c.Length + 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] yDotK = new double[stages][y.length];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] yDotK = new double[stages][y.Length];
		double[][] yDotK = RectangularArrays.ReturnRectangularDoubleArray(stages, y.Length);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yTmp = y0.clone();
		double[] yTmp = y0.clone();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yDotTmp = new double[y.length];
		double[] yDotTmp = new double[y.Length];

		// set up an interpolator sharing the integrator arrays
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RungeKuttaStepInterpolator interpolator = (RungeKuttaStepInterpolator) prototype.copy();
		RungeKuttaStepInterpolator interpolator = (RungeKuttaStepInterpolator) prototype.copy();
		interpolator.reinitialize(this, yTmp, yDotK, forward, equations.PrimaryMapper, equations.SecondaryMappers);
		interpolator.storeTime(equations.Time);

		// set up integration control objects
		stepStart = equations.Time;
		double hNew = 0;
		bool firstTime = true;
		initIntegration(equations.Time, y0, t);

		// main integration loop
		isLastStep = false;
		do
		{

		  interpolator.shift();

		  // iterate over step size, ensuring local normalized error is smaller than 1
		  double error = 10;
		  while (error >= 1.0)
		  {

			if (firstTime || !fsal)
			{
			  // first stage
			  computeDerivatives(stepStart, y, yDotK[0]);
			}

			if (firstTime)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] scale = new double[mainSetDimension];
			  double[] scale = new double[mainSetDimension];
			  if (vecAbsoluteTolerance == null)
			  {
				  for (int i = 0; i < scale.Length; ++i)
				  {
					scale[i] = scalAbsoluteTolerance + scalRelativeTolerance * FastMath.abs(y[i]);
				  }
			  }
			  else
			  {
				  for (int i = 0; i < scale.Length; ++i)
				  {
					scale[i] = vecAbsoluteTolerance[i] + vecRelativeTolerance[i] * FastMath.abs(y[i]);
				  }
			  }
			  hNew = initializeStep(forward, Order, scale, stepStart, y, yDotK[0], yTmp, yDotK[1]);
			  firstTime = false;
			}

			stepSize = hNew;
			if (forward)
			{
				if (stepStart + stepSize >= t)
				{
					stepSize = t - stepStart;
				}
			}
			else
			{
				if (stepStart + stepSize <= t)
				{
					stepSize = t - stepStart;
				}
			}

			// next stages
			for (int k = 1; k < stages; ++k)
			{

			  for (int j = 0; j < y0.Length; ++j)
			  {
				double sum = a[k - 1][0] * yDotK[0][j];
				for (int l = 1; l < k; ++l)
				{
				  sum += a[k - 1][l] * yDotK[l][j];
				}
				yTmp[j] = y[j] + stepSize * sum;
			  }

			  computeDerivatives(stepStart + c[k - 1] * stepSize, yTmp, yDotK[k]);

			}

			// estimate the state at the end of the step
			for (int j = 0; j < y0.Length; ++j)
			{
			  double sum = b[0] * yDotK[0][j];
			  for (int l = 1; l < stages; ++l)
			  {
				sum += b[l] * yDotK[l][j];
			  }
			  yTmp[j] = y[j] + stepSize * sum;
			}

			// estimate the error at the end of the step
			error = estimateError(yDotK, y, yTmp, stepSize);
			if (error >= 1.0)
			{
			  // reject the step and attempt to reduce error by stepsize control
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double factor = mathlib.util.FastMath.min(maxGrowth, mathlib.util.FastMath.max(minReduction, safety * mathlib.util.FastMath.pow(error, exp)));
			  double factor = FastMath.min(maxGrowth, FastMath.max(minReduction, safety * FastMath.pow(error, exp)));
			  hNew = filterStep(stepSize * factor, forward, false);
			}

		  }

		  // local error is small enough: accept the step, trigger events and step handlers
		  interpolator.storeTime(stepStart + stepSize);
		  Array.Copy(yTmp, 0, y, 0, y0.Length);
		  Array.Copy(yDotK[stages - 1], 0, yDotTmp, 0, y0.Length);
		  stepStart = acceptStep(interpolator, y, yDotTmp, t);
		  Array.Copy(y, 0, yTmp, 0, y.Length);

		  if (!isLastStep)
		  {

			  // prepare next step
			  interpolator.storeTime(stepStart);

			  if (fsal)
			  {
				  // save the last evaluation for the next step
				  Array.Copy(yDotTmp, 0, yDotK[0], 0, y0.Length);
			  }

			  // stepsize control for next step
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double factor = mathlib.util.FastMath.min(maxGrowth, mathlib.util.FastMath.max(minReduction, safety * mathlib.util.FastMath.pow(error, exp)));
			  double factor = FastMath.min(maxGrowth, FastMath.max(minReduction, safety * FastMath.pow(error, exp)));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double scaledH = stepSize * factor;
			  double scaledH = stepSize * factor;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double nextT = stepStart + scaledH;
			  double nextT = stepStart + scaledH;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean nextIsLast = forward ? (nextT >= t) : (nextT <= t);
			  bool nextIsLast = forward ? (nextT >= t) : (nextT <= t);
			  hNew = filterStep(scaledH, forward, nextIsLast);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double filteredNextT = stepStart + hNew;
			  double filteredNextT = stepStart + hNew;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean filteredNextIsLast = forward ? (filteredNextT >= t) : (filteredNextT <= t);
			  bool filteredNextIsLast = forward ? (filteredNextT >= t) : (filteredNextT <= t);
			  if (filteredNextIsLast)
			  {
				  hNew = t - stepStart;
			  }

		  }

		} while (!isLastStep);

		// dispatch results
		equations.Time = stepStart;
		equations.CompleteState = y;

		resetInternalState();

	  }

	  /// <summary>
	  /// Get the minimal reduction factor for stepsize control. </summary>
	  /// <returns> minimal reduction factor </returns>
	  public virtual double MinReduction
	  {
		  get
		  {
			return minReduction;
		  }
		  set
		  {
			this.minReduction = value;
		  }
	  }

	  /// <summary>
	  /// Set the minimal reduction factor for stepsize control. </summary>
	  /// <param name="minReduction"> minimal reduction factor </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setMinReduction(final double minReduction)

	  /// <summary>
	  /// Get the maximal growth factor for stepsize control. </summary>
	  /// <returns> maximal growth factor </returns>
	  public virtual double MaxGrowth
	  {
		  get
		  {
			return maxGrowth;
		  }
		  set
		  {
			this.maxGrowth = value;
		  }
	  }

	  /// <summary>
	  /// Set the maximal growth factor for stepsize control. </summary>
	  /// <param name="maxGrowth"> maximal growth factor </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setMaxGrowth(final double maxGrowth)

	  /// <summary>
	  /// Compute the error ratio. </summary>
	  /// <param name="yDotK"> derivatives computed during the first stages </param>
	  /// <param name="y0"> estimate of the step at the start of the step </param>
	  /// <param name="y1"> estimate of the step at the end of the step </param>
	  /// <param name="h">  current step </param>
	  /// <returns> error ratio, greater than 1 if step should be rejected </returns>
	  protected internal abstract double estimateError(double[][] yDotK, double[] y0, double[] y1, double h);

	}

}
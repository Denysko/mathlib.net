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
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// This abstract class holds the common part of all adaptive
	/// stepsize integrators for Ordinary Differential Equations.
	/// 
	/// <p>These algorithms perform integration with stepsize control, which
	/// means the user does not specify the integration step but rather a
	/// tolerance on error. The error threshold is computed as
	/// <pre>
	/// threshold_i = absTol_i + relTol_i * max (abs (ym), abs (ym+1))
	/// </pre>
	/// where absTol_i is the absolute tolerance for component i of the
	/// state vector and relTol_i is the relative tolerance for the same
	/// component. The user can also use only two scalar values absTol and
	/// relTol which will be used for all components.
	/// </p>
	/// <p>
	/// If the Ordinary Differential Equations is an {@link ExpandableStatefulODE
	/// extended ODE} rather than a {@link
	/// mathlib.ode.FirstOrderDifferentialEquations basic ODE}, then
	/// <em>only</em> the <seealso cref="ExpandableStatefulODE#getPrimaryState() primary part"/>
	/// of the state vector is used for stepsize control, not the complete state vector.
	/// </p>
	/// 
	/// <p>If the estimated error for ym+1 is such that
	/// <pre>
	/// sqrt((sum (errEst_i / threshold_i)^2 ) / n) < 1
	/// </pre>
	/// 
	/// (where n is the main set dimension) then the step is accepted,
	/// otherwise the step is rejected and a new attempt is made with a new
	/// stepsize.</p>
	/// 
	/// @version $Id: AdaptiveStepsizeIntegrator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2
	/// 
	/// </summary>

	public abstract class AdaptiveStepsizeIntegrator : AbstractIntegrator
	{

		/// <summary>
		/// Allowed absolute scalar error. </summary>
		protected internal double scalAbsoluteTolerance;

		/// <summary>
		/// Allowed relative scalar error. </summary>
		protected internal double scalRelativeTolerance;

		/// <summary>
		/// Allowed absolute vectorial error. </summary>
		protected internal double[] vecAbsoluteTolerance;

		/// <summary>
		/// Allowed relative vectorial error. </summary>
		protected internal double[] vecRelativeTolerance;

		/// <summary>
		/// Main set dimension. </summary>
		protected internal int mainSetDimension;

		/// <summary>
		/// User supplied initial step. </summary>
		private double initialStep;

		/// <summary>
		/// Minimal step. </summary>
		private double minStep;

		/// <summary>
		/// Maximal step. </summary>
		private double maxStep;

	  /// <summary>
	  /// Build an integrator with the given stepsize bounds.
	  /// The default step handler does nothing. </summary>
	  /// <param name="name"> name of the method </param>
	  /// <param name="minStep"> minimal step (sign is irrelevant, regardless of
	  /// integration direction, forward or backward), the last step can
	  /// be smaller than this </param>
	  /// <param name="maxStep"> maximal step (sign is irrelevant, regardless of
	  /// integration direction, forward or backward), the last step can
	  /// be smaller than this </param>
	  /// <param name="scalAbsoluteTolerance"> allowed absolute error </param>
	  /// <param name="scalRelativeTolerance"> allowed relative error </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public AdaptiveStepsizeIntegrator(final String name, final double minStep, final double maxStep, final double scalAbsoluteTolerance, final double scalRelativeTolerance)
	  public AdaptiveStepsizeIntegrator(string name, double minStep, double maxStep, double scalAbsoluteTolerance, double scalRelativeTolerance) : base(name)
	  {

		setStepSizeControl(minStep, maxStep, scalAbsoluteTolerance, scalRelativeTolerance);
		resetInternalState();

	  }

	  /// <summary>
	  /// Build an integrator with the given stepsize bounds.
	  /// The default step handler does nothing. </summary>
	  /// <param name="name"> name of the method </param>
	  /// <param name="minStep"> minimal step (sign is irrelevant, regardless of
	  /// integration direction, forward or backward), the last step can
	  /// be smaller than this </param>
	  /// <param name="maxStep"> maximal step (sign is irrelevant, regardless of
	  /// integration direction, forward or backward), the last step can
	  /// be smaller than this </param>
	  /// <param name="vecAbsoluteTolerance"> allowed absolute error </param>
	  /// <param name="vecRelativeTolerance"> allowed relative error </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public AdaptiveStepsizeIntegrator(final String name, final double minStep, final double maxStep, final double[] vecAbsoluteTolerance, final double[] vecRelativeTolerance)
	  public AdaptiveStepsizeIntegrator(string name, double minStep, double maxStep, double[] vecAbsoluteTolerance, double[] vecRelativeTolerance) : base(name)
	  {

		setStepSizeControl(minStep, maxStep, vecAbsoluteTolerance, vecRelativeTolerance);
		resetInternalState();

	  }

	  /// <summary>
	  /// Set the adaptive step size control parameters.
	  /// <p>
	  /// A side effect of this method is to also reset the initial
	  /// step so it will be automatically computed by the integrator
	  /// if <seealso cref="#setInitialStepSize(double) setInitialStepSize"/>
	  /// is not called by the user.
	  /// </p> </summary>
	  /// <param name="minimalStep"> minimal step (must be positive even for backward
	  /// integration), the last step can be smaller than this </param>
	  /// <param name="maximalStep"> maximal step (must be positive even for backward
	  /// integration) </param>
	  /// <param name="absoluteTolerance"> allowed absolute error </param>
	  /// <param name="relativeTolerance"> allowed relative error </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setStepSizeControl(final double minimalStep, final double maximalStep, final double absoluteTolerance, final double relativeTolerance)
	  public virtual void setStepSizeControl(double minimalStep, double maximalStep, double absoluteTolerance, double relativeTolerance)
	  {

		  minStep = FastMath.abs(minimalStep);
		  maxStep = FastMath.abs(maximalStep);
		  initialStep = -1;

		  scalAbsoluteTolerance = absoluteTolerance;
		  scalRelativeTolerance = relativeTolerance;
		  vecAbsoluteTolerance = null;
		  vecRelativeTolerance = null;

	  }

	  /// <summary>
	  /// Set the adaptive step size control parameters.
	  /// <p>
	  /// A side effect of this method is to also reset the initial
	  /// step so it will be automatically computed by the integrator
	  /// if <seealso cref="#setInitialStepSize(double) setInitialStepSize"/>
	  /// is not called by the user.
	  /// </p> </summary>
	  /// <param name="minimalStep"> minimal step (must be positive even for backward
	  /// integration), the last step can be smaller than this </param>
	  /// <param name="maximalStep"> maximal step (must be positive even for backward
	  /// integration) </param>
	  /// <param name="absoluteTolerance"> allowed absolute error </param>
	  /// <param name="relativeTolerance"> allowed relative error </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setStepSizeControl(final double minimalStep, final double maximalStep, final double[] absoluteTolerance, final double[] relativeTolerance)
	  public virtual void setStepSizeControl(double minimalStep, double maximalStep, double[] absoluteTolerance, double[] relativeTolerance)
	  {

		  minStep = FastMath.abs(minimalStep);
		  maxStep = FastMath.abs(maximalStep);
		  initialStep = -1;

		  scalAbsoluteTolerance = 0;
		  scalRelativeTolerance = 0;
		  vecAbsoluteTolerance = absoluteTolerance.clone();
		  vecRelativeTolerance = relativeTolerance.clone();

	  }

	  /// <summary>
	  /// Set the initial step size.
	  /// <p>This method allows the user to specify an initial positive
	  /// step size instead of letting the integrator guess it by
	  /// itself. If this method is not called before integration is
	  /// started, the initial step size will be estimated by the
	  /// integrator.</p> </summary>
	  /// <param name="initialStepSize"> initial step size to use (must be positive even
	  /// for backward integration ; providing a negative value or a value
	  /// outside of the min/max step interval will lead the integrator to
	  /// ignore the value and compute the initial step size by itself) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setInitialStepSize(final double initialStepSize)
	  public virtual double InitialStepSize
	  {
		  set
		  {
			if ((value < minStep) || (value > maxStep))
			{
			  initialStep = -1.0;
			}
			else
			{
			  initialStep = value;
			}
		  }
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void sanityChecks(final mathlib.ode.ExpandableStatefulODE equations, final double t) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  protected internal override void sanityChecks(ExpandableStatefulODE equations, double t)
	  {

		  base.sanityChecks(equations, t);

		  mainSetDimension = equations.PrimaryMapper.Dimension;

		  if ((vecAbsoluteTolerance != null) && (vecAbsoluteTolerance.Length != mainSetDimension))
		  {
			  throw new DimensionMismatchException(mainSetDimension, vecAbsoluteTolerance.Length);
		  }

		  if ((vecRelativeTolerance != null) && (vecRelativeTolerance.Length != mainSetDimension))
		  {
			  throw new DimensionMismatchException(mainSetDimension, vecRelativeTolerance.Length);
		  }

	  }

	  /// <summary>
	  /// Initialize the integration step. </summary>
	  /// <param name="forward"> forward integration indicator </param>
	  /// <param name="order"> order of the method </param>
	  /// <param name="scale"> scaling vector for the state vector (can be shorter than state vector) </param>
	  /// <param name="t0"> start time </param>
	  /// <param name="y0"> state vector at t0 </param>
	  /// <param name="yDot0"> first time derivative of y0 </param>
	  /// <param name="y1"> work array for a state vector </param>
	  /// <param name="yDot1"> work array for the first time derivative of y1 </param>
	  /// <returns> first integration step </returns>
	  /// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
	  /// <exception cref="DimensionMismatchException"> if arrays dimensions do not match equations settings </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double initializeStep(final boolean forward, final int order, final double[] scale, final double t0, final double[] y0, final double[] yDot0, final double[] y1, final double[] yDot1) throws mathlib.exception.MaxCountExceededException, mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  public virtual double initializeStep(bool forward, int order, double[] scale, double t0, double[] y0, double[] yDot0, double[] y1, double[] yDot1)
	  {

		if (initialStep > 0)
		{
		  // use the user provided value
		  return forward ? initialStep : -initialStep;
		}

		// very rough first guess : h = 0.01 * ||y/scale|| / ||y'/scale||
		// this guess will be used to perform an Euler step
		double ratio;
		double yOnScale2 = 0;
		double yDotOnScale2 = 0;
		for (int j = 0; j < scale.Length; ++j)
		{
		  ratio = y0[j] / scale[j];
		  yOnScale2 += ratio * ratio;
		  ratio = yDot0[j] / scale[j];
		  yDotOnScale2 += ratio * ratio;
		}

		double h = ((yOnScale2 < 1.0e-10) || (yDotOnScale2 < 1.0e-10)) ? 1.0e-6 : (0.01 * FastMath.sqrt(yOnScale2 / yDotOnScale2));
		if (!forward)
		{
		  h = -h;
		}

		// perform an Euler step using the preceding rough guess
		for (int j = 0; j < y0.Length; ++j)
		{
		  y1[j] = y0[j] + h * yDot0[j];
		}
		computeDerivatives(t0 + h, y1, yDot1);

		// estimate the second derivative of the solution
		double yDDotOnScale = 0;
		for (int j = 0; j < scale.Length; ++j)
		{
		  ratio = (yDot1[j] - yDot0[j]) / scale[j];
		  yDDotOnScale += ratio * ratio;
		}
		yDDotOnScale = FastMath.sqrt(yDDotOnScale) / h;

		// step size is computed such that
		// h^order * max (||y'/tol||, ||y''/tol||) = 0.01
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double maxInv2 = mathlib.util.FastMath.max(mathlib.util.FastMath.sqrt(yDotOnScale2), yDDotOnScale);
		double maxInv2 = FastMath.max(FastMath.sqrt(yDotOnScale2), yDDotOnScale);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double h1 = (maxInv2 < 1.0e-15) ? mathlib.util.FastMath.max(1.0e-6, 0.001 * mathlib.util.FastMath.abs(h)) : mathlib.util.FastMath.pow(0.01 / maxInv2, 1.0 / order);
		double h1 = (maxInv2 < 1.0e-15) ? FastMath.max(1.0e-6, 0.001 * FastMath.abs(h)) : FastMath.pow(0.01 / maxInv2, 1.0 / order);
		h = FastMath.min(100.0 * FastMath.abs(h), h1);
		h = FastMath.max(h, 1.0e-12 * FastMath.abs(t0)); // avoids cancellation when computing t1 - t0
		if (h < MinStep)
		{
		  h = MinStep;
		}
		if (h > MaxStep)
		{
		  h = MaxStep;
		}
		if (!forward)
		{
		  h = -h;
		}

		return h;

	  }

	  /// <summary>
	  /// Filter the integration step. </summary>
	  /// <param name="h"> signed step </param>
	  /// <param name="forward"> forward integration indicator </param>
	  /// <param name="acceptSmall"> if true, steps smaller than the minimal value
	  /// are silently increased up to this value, if false such small
	  /// steps generate an exception </param>
	  /// <returns> a bounded integration step (h if no bound is reach, or a bounded value) </returns>
	  /// <exception cref="NumberIsTooSmallException"> if the step is too small and acceptSmall is false </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected double filterStep(final double h, final boolean forward, final boolean acceptSmall) throws mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  protected internal virtual double filterStep(double h, bool forward, bool acceptSmall)
	  {

		  double filteredH = h;
		  if (FastMath.abs(h) < minStep)
		  {
			  if (acceptSmall)
			  {
				  filteredH = forward ? minStep : -minStep;
			  }
			  else
			  {
				  throw new NumberIsTooSmallException(LocalizedFormats.MINIMAL_STEPSIZE_REACHED_DURING_INTEGRATION, FastMath.abs(h), minStep, true);
			  }
		  }

		  if (filteredH > maxStep)
		  {
			  filteredH = maxStep;
		  }
		  else if (filteredH < -maxStep)
		  {
			  filteredH = -maxStep;
		  }

		  return filteredH;

	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public abstract void integrate(mathlib.ode.ExpandableStatefulODE equations, double t) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.DimensionMismatchException, mathlib.exception.MaxCountExceededException, mathlib.exception.NoBracketingException;
	  public override abstract void integrate(ExpandableStatefulODE equations, double t);

	  /// <summary>
	  /// {@inheritDoc} </summary>
	  public override double CurrentStepStart
	  {
		  get
		  {
			return stepStart;
		  }
	  }

	  /// <summary>
	  /// Reset internal state to dummy values. </summary>
	  protected internal virtual void resetInternalState()
	  {
		stepStart = double.NaN;
		stepSize = FastMath.sqrt(minStep * maxStep);
	  }

	  /// <summary>
	  /// Get the minimal step. </summary>
	  /// <returns> minimal step </returns>
	  public virtual double MinStep
	  {
		  get
		  {
			return minStep;
		  }
	  }

	  /// <summary>
	  /// Get the maximal step. </summary>
	  /// <returns> maximal step </returns>
	  public virtual double MaxStep
	  {
		  get
		  {
			return maxStep;
		  }
	  }

	}

}
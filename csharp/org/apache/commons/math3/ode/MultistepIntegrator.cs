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

namespace org.apache.commons.math3.ode
{

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MaxCountExceededException = org.apache.commons.math3.exception.MaxCountExceededException;
	using NoBracketingException = org.apache.commons.math3.exception.NoBracketingException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using Array2DRowRealMatrix = org.apache.commons.math3.linear.Array2DRowRealMatrix;
	using AdaptiveStepsizeIntegrator = org.apache.commons.math3.ode.nonstiff.AdaptiveStepsizeIntegrator;
	using DormandPrince853Integrator = org.apache.commons.math3.ode.nonstiff.DormandPrince853Integrator;
	using StepHandler = org.apache.commons.math3.ode.sampling.StepHandler;
	using StepInterpolator = org.apache.commons.math3.ode.sampling.StepInterpolator;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// This class is the base class for multistep integrators for Ordinary
	/// Differential Equations.
	/// <p>We define scaled derivatives s<sub>i</sub>(n) at step n as:
	/// <pre>
	/// s<sub>1</sub>(n) = h y'<sub>n</sub> for first derivative
	/// s<sub>2</sub>(n) = h<sup>2</sup>/2 y''<sub>n</sub> for second derivative
	/// s<sub>3</sub>(n) = h<sup>3</sup>/6 y'''<sub>n</sub> for third derivative
	/// ...
	/// s<sub>k</sub>(n) = h<sup>k</sup>/k! y<sup>(k)</sup><sub>n</sub> for k<sup>th</sup> derivative
	/// </pre></p>
	/// <p>Rather than storing several previous steps separately, this implementation uses
	/// the Nordsieck vector with higher degrees scaled derivatives all taken at the same
	/// step (y<sub>n</sub>, s<sub>1</sub>(n) and r<sub>n</sub>) where r<sub>n</sub> is defined as:
	/// <pre>
	/// r<sub>n</sub> = [ s<sub>2</sub>(n), s<sub>3</sub>(n) ... s<sub>k</sub>(n) ]<sup>T</sup>
	/// </pre>
	/// (we omit the k index in the notation for clarity)</p>
	/// <p>
	/// Multistep integrators with Nordsieck representation are highly sensitive to
	/// large step changes because when the step is multiplied by factor a, the
	/// k<sup>th</sup> component of the Nordsieck vector is multiplied by a<sup>k</sup>
	/// and the last components are the least accurate ones. The default max growth
	/// factor is therefore set to a quite low value: 2<sup>1/order</sup>.
	/// </p>
	/// </summary>
	/// <seealso cref= org.apache.commons.math3.ode.nonstiff.AdamsBashforthIntegrator </seealso>
	/// <seealso cref= org.apache.commons.math3.ode.nonstiff.AdamsMoultonIntegrator
	/// @version $Id: MultistepIntegrator.java 1463684 2013-04-02 19:04:13Z luc $
	/// @since 2.0 </seealso>
	public abstract class MultistepIntegrator : AdaptiveStepsizeIntegrator
	{

		/// <summary>
		/// First scaled derivative (h y'). </summary>
		protected internal double[] scaled;

		/// <summary>
		/// Nordsieck matrix of the higher scaled derivatives.
		/// <p>(h<sup>2</sup>/2 y'', h<sup>3</sup>/6 y''' ..., h<sup>k</sup>/k! y<sup>(k)</sup>)</p>
		/// </summary>
		protected internal Array2DRowRealMatrix nordsieck;

		/// <summary>
		/// Starter integrator. </summary>
		private FirstOrderIntegrator starter;

		/// <summary>
		/// Number of steps of the multistep method (excluding the one being computed). </summary>
		private readonly int nSteps;

		/// <summary>
		/// Stepsize control exponent. </summary>
		private double exp;

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
		/// Build a multistep integrator with the given stepsize bounds.
		/// <p>The default starter integrator is set to the {@link
		/// DormandPrince853Integrator Dormand-Prince 8(5,3)} integrator with
		/// some defaults settings.</p>
		/// <p>
		/// The default max growth factor is set to a quite low value: 2<sup>1/order</sup>.
		/// </p> </summary>
		/// <param name="name"> name of the method </param>
		/// <param name="nSteps"> number of steps of the multistep method
		/// (excluding the one being computed) </param>
		/// <param name="order"> order of the method </param>
		/// <param name="minStep"> minimal step (must be positive even for backward
		/// integration), the last step can be smaller than this </param>
		/// <param name="maxStep"> maximal step (must be positive even for backward
		/// integration) </param>
		/// <param name="scalAbsoluteTolerance"> allowed absolute error </param>
		/// <param name="scalRelativeTolerance"> allowed relative error </param>
		/// <exception cref="NumberIsTooSmallException"> if number of steps is smaller than 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected MultistepIntegrator(final String name, final int nSteps, final int order, final double minStep, final double maxStep, final double scalAbsoluteTolerance, final double scalRelativeTolerance) throws org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal MultistepIntegrator(string name, int nSteps, int order, double minStep, double maxStep, double scalAbsoluteTolerance, double scalRelativeTolerance) : base(name, minStep, maxStep, scalAbsoluteTolerance, scalRelativeTolerance)
		{


			if (nSteps < 2)
			{
				throw new NumberIsTooSmallException(LocalizedFormats.INTEGRATION_METHOD_NEEDS_AT_LEAST_TWO_PREVIOUS_POINTS, nSteps, 2, true);
			}

			starter = new DormandPrince853Integrator(minStep, maxStep, scalAbsoluteTolerance, scalRelativeTolerance);
			this.nSteps = nSteps;

			exp = -1.0 / order;

			// set the default values of the algorithm control parameters
			Safety = 0.9;
			MinReduction = 0.2;
			MaxGrowth = FastMath.pow(2.0, -exp);

		}

		/// <summary>
		/// Build a multistep integrator with the given stepsize bounds.
		/// <p>The default starter integrator is set to the {@link
		/// DormandPrince853Integrator Dormand-Prince 8(5,3)} integrator with
		/// some defaults settings.</p>
		/// <p>
		/// The default max growth factor is set to a quite low value: 2<sup>1/order</sup>.
		/// </p> </summary>
		/// <param name="name"> name of the method </param>
		/// <param name="nSteps"> number of steps of the multistep method
		/// (excluding the one being computed) </param>
		/// <param name="order"> order of the method </param>
		/// <param name="minStep"> minimal step (must be positive even for backward
		/// integration), the last step can be smaller than this </param>
		/// <param name="maxStep"> maximal step (must be positive even for backward
		/// integration) </param>
		/// <param name="vecAbsoluteTolerance"> allowed absolute error </param>
		/// <param name="vecRelativeTolerance"> allowed relative error </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected MultistepIntegrator(final String name, final int nSteps, final int order, final double minStep, final double maxStep, final double[] vecAbsoluteTolerance, final double[] vecRelativeTolerance)
		protected internal MultistepIntegrator(string name, int nSteps, int order, double minStep, double maxStep, double[] vecAbsoluteTolerance, double[] vecRelativeTolerance) : base(name, minStep, maxStep, vecAbsoluteTolerance, vecRelativeTolerance)
		{
			starter = new DormandPrince853Integrator(minStep, maxStep, vecAbsoluteTolerance, vecRelativeTolerance);
			this.nSteps = nSteps;

			exp = -1.0 / order;

			// set the default values of the algorithm control parameters
			Safety = 0.9;
			MinReduction = 0.2;
			MaxGrowth = FastMath.pow(2.0, -exp);

		}

		/// <summary>
		/// Get the starter integrator. </summary>
		/// <returns> starter integrator </returns>
		public virtual ODEIntegrator StarterIntegrator
		{
			get
			{
				return starter;
			}
			set
			{
				this.starter = value;
			}
		}


		/// <summary>
		/// Start the integration.
		/// <p>This method computes one step using the underlying starter integrator,
		/// and initializes the Nordsieck vector at step start. The starter integrator
		/// purpose is only to establish initial conditions, it does not really change
		/// time by itself. The top level multistep integrator remains in charge of
		/// handling time propagation and events handling as it will starts its own
		/// computation right from the beginning. In a sense, the starter integrator
		/// can be seen as a dummy one and so it will never trigger any user event nor
		/// call any user step handler.</p> </summary>
		/// <param name="t0"> initial time </param>
		/// <param name="y0"> initial value of the state vector at t0 </param>
		/// <param name="t"> target time for the integration
		/// (can be set to a value smaller than <code>t0</code> for backward integration) </param>
		/// <exception cref="DimensionMismatchException"> if arrays dimension do not match equations settings </exception>
		/// <exception cref="NumberIsTooSmallException"> if integration step is too small </exception>
		/// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
		/// <exception cref="NoBracketingException"> if the location of an event cannot be bracketed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void start(final double t0, final double[] y0, final double t) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.MaxCountExceededException, org.apache.commons.math3.exception.NoBracketingException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual void start(double t0, double[] y0, double t)
		{

			// make sure NO user event nor user step handler is triggered,
			// this is the task of the top level integrator, not the task
			// of the starter integrator
			starter.clearEventHandlers();
			starter.clearStepHandlers();

			// set up one specific step handler to extract initial Nordsieck vector
			starter.addStepHandler(new NordsieckInitializer(this, nSteps, y0.Length));

			// start integration, expecting a InitializationCompletedMarkerException
			try
			{

				if (starter is AbstractIntegrator)
				{
					((AbstractIntegrator) starter).integrate(Expandable, t);
				}
				else
				{
					starter.integrate(new FirstOrderDifferentialEquationsAnonymousInnerClassHelper(this, t), t0, y0, t, new double[y0.Length]);
				}

			} // NOPMD
			catch (InitializationCompletedMarkerException icme)
			{
				// this is the expected nominal interruption of the start integrator

				// count the evaluations used by the starter
				EvaluationsCounter.incrementCount(starter.Evaluations);

			}

			// remove the specific step handler
			starter.clearStepHandlers();

		}

		private class FirstOrderDifferentialEquationsAnonymousInnerClassHelper : FirstOrderDifferentialEquations
		{
			private readonly MultistepIntegrator outerInstance;

			private double t;

			public FirstOrderDifferentialEquationsAnonymousInnerClassHelper(MultistepIntegrator outerInstance, double t)
			{
				this.outerInstance = outerInstance;
				this.t = t;
			}


							/// <summary>
							/// {@inheritDoc} </summary>
			public virtual int Dimension
			{
				get
				{
					return outerInstance.Expandable.TotalDimension;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual void computeDerivatives(double t, double[] y, double[] yDot)
			{
				outerInstance.Expandable.computeDerivatives(t, y, yDot);
			}

		}

		/// <summary>
		/// Initialize the high order scaled derivatives at step start. </summary>
		/// <param name="h"> step size to use for scaling </param>
		/// <param name="t"> first steps times </param>
		/// <param name="y"> first steps states </param>
		/// <param name="yDot"> first steps derivatives </param>
		/// <returns> Nordieck vector at first step (h<sup>2</sup>/2 y''<sub>n</sub>,
		/// h<sup>3</sup>/6 y'''<sub>n</sub> ... h<sup>k</sup>/k! y<sup>(k)</sup><sub>n</sub>) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected abstract org.apache.commons.math3.linear.Array2DRowRealMatrix initializeHighOrderDerivatives(final double h, final double[] t, final double[][] y, final double[][] yDot);
		protected internal abstract Array2DRowRealMatrix initializeHighOrderDerivatives(double h, double[] t, double[][] y, double[][] yDot);

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
		/// Compute step grow/shrink factor according to normalized error. </summary>
		/// <param name="error"> normalized error of the current step </param>
		/// <returns> grow/shrink factor for next step </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected double computeStepGrowShrinkFactor(final double error)
		protected internal virtual double computeStepGrowShrinkFactor(double error)
		{
			return FastMath.min(maxGrowth, FastMath.max(minReduction, safety * FastMath.pow(error, exp)));
		}

		/// <summary>
		/// Transformer used to convert the first step to Nordsieck representation. </summary>
		public interface NordsieckTransformer
		{
			/// <summary>
			/// Initialize the high order scaled derivatives at step start. </summary>
			/// <param name="h"> step size to use for scaling </param>
			/// <param name="t"> first steps times </param>
			/// <param name="y"> first steps states </param>
			/// <param name="yDot"> first steps derivatives </param>
			/// <returns> Nordieck vector at first step (h<sup>2</sup>/2 y''<sub>n</sub>,
			/// h<sup>3</sup>/6 y'''<sub>n</sub> ... h<sup>k</sup>/k! y<sup>(k)</sup><sub>n</sub>) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: org.apache.commons.math3.linear.Array2DRowRealMatrix initializeHighOrderDerivatives(final double h, final double[] t, final double[][] y, final double[][] yDot);
			Array2DRowRealMatrix initializeHighOrderDerivatives(double h, double[] t, double[][] y, double[][] yDot);
		}

		/// <summary>
		/// Specialized step handler storing the first step. </summary>
		private class NordsieckInitializer : StepHandler
		{
			private readonly MultistepIntegrator outerInstance;


			/// <summary>
			/// Steps counter. </summary>
			internal int count;

			/// <summary>
			/// First steps times. </summary>
			internal readonly double[] t;

			/// <summary>
			/// First steps states. </summary>
			internal readonly double[][] y;

			/// <summary>
			/// First steps derivatives. </summary>
			internal readonly double[][] yDot;

			/// <summary>
			/// Simple constructor. </summary>
			/// <param name="nSteps"> number of steps of the multistep method (excluding the one being computed) </param>
			/// <param name="n"> problem dimension </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NordsieckInitializer(final int nSteps, final int n)
			public NordsieckInitializer(MultistepIntegrator outerInstance, int nSteps, int n)
			{
				this.outerInstance = outerInstance;
				this.count = 0;
				this.t = new double[nSteps];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: this.y = new double[nSteps][n];
				this.y = RectangularArrays.ReturnRectangularDoubleArray(nSteps, n);
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: this.yDot = new double[nSteps][n];
				this.yDot = RectangularArrays.ReturnRectangularDoubleArray(nSteps, n);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void handleStep(org.apache.commons.math3.ode.sampling.StepInterpolator interpolator, boolean isLast) throws org.apache.commons.math3.exception.MaxCountExceededException
			public virtual void handleStep(StepInterpolator interpolator, bool isLast)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prev = interpolator.getPreviousTime();
				double prev = interpolator.PreviousTime;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double curr = interpolator.getCurrentTime();
				double curr = interpolator.CurrentTime;

				if (count == 0)
				{
					// first step, we need to store also the beginning of the step
					interpolator.InterpolatedTime = prev;
					t[0] = prev;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ExpandableStatefulODE expandable = getExpandable();
					ExpandableStatefulODE expandable = outerInstance.Expandable;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final EquationsMapper primary = expandable.getPrimaryMapper();
					EquationsMapper primary = expandable.PrimaryMapper;
					primary.insertEquationData(interpolator.InterpolatedState, y[count]);
					primary.insertEquationData(interpolator.InterpolatedDerivatives, yDot[count]);
					int index = 0;
					foreach (EquationsMapper secondary in expandable.SecondaryMappers)
					{
						secondary.insertEquationData(interpolator.getInterpolatedSecondaryState(index), y[count]);
						secondary.insertEquationData(interpolator.getInterpolatedSecondaryDerivatives(index), yDot[count]);
						++index;
					}
				}

				// store the end of the step
				++count;
				interpolator.InterpolatedTime = curr;
				t[count] = curr;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ExpandableStatefulODE expandable = getExpandable();
				ExpandableStatefulODE expandable = outerInstance.Expandable;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final EquationsMapper primary = expandable.getPrimaryMapper();
				EquationsMapper primary = expandable.PrimaryMapper;
				primary.insertEquationData(interpolator.InterpolatedState, y[count]);
				primary.insertEquationData(interpolator.InterpolatedDerivatives, yDot[count]);
				int index = 0;
				foreach (EquationsMapper secondary in expandable.SecondaryMappers)
				{
					secondary.insertEquationData(interpolator.getInterpolatedSecondaryState(index), y[count]);
					secondary.insertEquationData(interpolator.getInterpolatedSecondaryDerivatives(index), yDot[count]);
					++index;
				}

				if (count == t.Length - 1)
				{

					// this was the last step we needed, we can compute the derivatives
					outerInstance.stepStart = t[0];
					outerInstance.stepSize = (t[t.Length - 1] - t[0]) / (t.Length - 1);

					// first scaled derivative
					outerInstance.scaled = yDot[0].clone();
					for (int j = 0; j < outerInstance.scaled.Length; ++j)
					{
						outerInstance.scaled[j] *= outerInstance.stepSize;
					}

					// higher order derivatives
					outerInstance.nordsieck = outerInstance.initializeHighOrderDerivatives(outerInstance.stepSize, t, y, yDot);

					// stop the integrator now that all needed steps have been handled
					throw new InitializationCompletedMarkerException();

				}

			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual void init(double t0, double[] y0, double time)
			{
				// nothing to do
			}

		}

		/// <summary>
		/// Marker exception used ONLY to stop the starter integrator after first step. </summary>
		private class InitializationCompletedMarkerException : Exception
		{

			/// <summary>
			/// Serializable version identifier. </summary>
			internal const long serialVersionUID = -1914085471038046418L;

			/// <summary>
			/// Simple constructor. </summary>
			public InitializationCompletedMarkerException() : base((Exception) null)
			{
			}

		}

	}

}
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

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MaxCountExceededException = org.apache.commons.math3.exception.MaxCountExceededException;
	using NoBracketingException = org.apache.commons.math3.exception.NoBracketingException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using Array2DRowRealMatrix = org.apache.commons.math3.linear.Array2DRowRealMatrix;
	using RealMatrixPreservingVisitor = org.apache.commons.math3.linear.RealMatrixPreservingVisitor;
	using NordsieckStepInterpolator = org.apache.commons.math3.ode.sampling.NordsieckStepInterpolator;
	using FastMath = org.apache.commons.math3.util.FastMath;


	/// <summary>
	/// This class implements implicit Adams-Moulton integrators for Ordinary
	/// Differential Equations.
	/// 
	/// <p>Adams-Moulton methods (in fact due to Adams alone) are implicit
	/// multistep ODE solvers. This implementation is a variation of the classical
	/// one: it uses adaptive stepsize to implement error control, whereas
	/// classical implementations are fixed step size. The value of state vector
	/// at step n+1 is a simple combination of the value at step n and of the
	/// derivatives at steps n+1, n, n-1 ... Since y'<sub>n+1</sub> is needed to
	/// compute y<sub>n+1</sub>, another method must be used to compute a first
	/// estimate of y<sub>n+1</sub>, then compute y'<sub>n+1</sub>, then compute
	/// a final estimate of y<sub>n+1</sub> using the following formulas. Depending
	/// on the number k of previous steps one wants to use for computing the next
	/// value, different formulas are available for the final estimate:</p>
	/// <ul>
	///   <li>k = 1: y<sub>n+1</sub> = y<sub>n</sub> + h y'<sub>n+1</sub></li>
	///   <li>k = 2: y<sub>n+1</sub> = y<sub>n</sub> + h (y'<sub>n+1</sub>+y'<sub>n</sub>)/2</li>
	///   <li>k = 3: y<sub>n+1</sub> = y<sub>n</sub> + h (5y'<sub>n+1</sub>+8y'<sub>n</sub>-y'<sub>n-1</sub>)/12</li>
	///   <li>k = 4: y<sub>n+1</sub> = y<sub>n</sub> + h (9y'<sub>n+1</sub>+19y'<sub>n</sub>-5y'<sub>n-1</sub>+y'<sub>n-2</sub>)/24</li>
	///   <li>...</li>
	/// </ul>
	/// 
	/// <p>A k-steps Adams-Moulton method is of order k+1.</p>
	/// 
	/// <h3>Implementation details</h3>
	/// 
	/// <p>We define scaled derivatives s<sub>i</sub>(n) at step n as:
	/// <pre>
	/// s<sub>1</sub>(n) = h y'<sub>n</sub> for first derivative
	/// s<sub>2</sub>(n) = h<sup>2</sup>/2 y''<sub>n</sub> for second derivative
	/// s<sub>3</sub>(n) = h<sup>3</sup>/6 y'''<sub>n</sub> for third derivative
	/// ...
	/// s<sub>k</sub>(n) = h<sup>k</sup>/k! y<sup>(k)</sup><sub>n</sub> for k<sup>th</sup> derivative
	/// </pre></p>
	/// 
	/// <p>The definitions above use the classical representation with several previous first
	/// derivatives. Lets define
	/// <pre>
	///   q<sub>n</sub> = [ s<sub>1</sub>(n-1) s<sub>1</sub>(n-2) ... s<sub>1</sub>(n-(k-1)) ]<sup>T</sup>
	/// </pre>
	/// (we omit the k index in the notation for clarity). With these definitions,
	/// Adams-Moulton methods can be written:
	/// <ul>
	///   <li>k = 1: y<sub>n+1</sub> = y<sub>n</sub> + s<sub>1</sub>(n+1)</li>
	///   <li>k = 2: y<sub>n+1</sub> = y<sub>n</sub> + 1/2 s<sub>1</sub>(n+1) + [ 1/2 ] q<sub>n+1</sub></li>
	///   <li>k = 3: y<sub>n+1</sub> = y<sub>n</sub> + 5/12 s<sub>1</sub>(n+1) + [ 8/12 -1/12 ] q<sub>n+1</sub></li>
	///   <li>k = 4: y<sub>n+1</sub> = y<sub>n</sub> + 9/24 s<sub>1</sub>(n+1) + [ 19/24 -5/24 1/24 ] q<sub>n+1</sub></li>
	///   <li>...</li>
	/// </ul></p>
	/// 
	/// <p>Instead of using the classical representation with first derivatives only (y<sub>n</sub>,
	/// s<sub>1</sub>(n+1) and q<sub>n+1</sub>), our implementation uses the Nordsieck vector with
	/// higher degrees scaled derivatives all taken at the same step (y<sub>n</sub>, s<sub>1</sub>(n)
	/// and r<sub>n</sub>) where r<sub>n</sub> is defined as:
	/// <pre>
	/// r<sub>n</sub> = [ s<sub>2</sub>(n), s<sub>3</sub>(n) ... s<sub>k</sub>(n) ]<sup>T</sup>
	/// </pre>
	/// (here again we omit the k index in the notation for clarity)
	/// </p>
	/// 
	/// <p>Taylor series formulas show that for any index offset i, s<sub>1</sub>(n-i) can be
	/// computed from s<sub>1</sub>(n), s<sub>2</sub>(n) ... s<sub>k</sub>(n), the formula being exact
	/// for degree k polynomials.
	/// <pre>
	/// s<sub>1</sub>(n-i) = s<sub>1</sub>(n) + &sum;<sub>j</sub> j (-i)<sup>j-1</sup> s<sub>j</sub>(n)
	/// </pre>
	/// The previous formula can be used with several values for i to compute the transform between
	/// classical representation and Nordsieck vector. The transform between r<sub>n</sub>
	/// and q<sub>n</sub> resulting from the Taylor series formulas above is:
	/// <pre>
	/// q<sub>n</sub> = s<sub>1</sub>(n) u + P r<sub>n</sub>
	/// </pre>
	/// where u is the [ 1 1 ... 1 ]<sup>T</sup> vector and P is the (k-1)&times;(k-1) matrix built
	/// with the j (-i)<sup>j-1</sup> terms:
	/// <pre>
	///        [  -2   3   -4    5  ... ]
	///        [  -4  12  -32   80  ... ]
	///   P =  [  -6  27 -108  405  ... ]
	///        [  -8  48 -256 1280  ... ]
	///        [          ...           ]
	/// </pre></p>
	/// 
	/// <p>Using the Nordsieck vector has several advantages:
	/// <ul>
	///   <li>it greatly simplifies step interpolation as the interpolator mainly applies
	///   Taylor series formulas,</li>
	///   <li>it simplifies step changes that occur when discrete events that truncate
	///   the step are triggered,</li>
	///   <li>it allows to extend the methods in order to support adaptive stepsize.</li>
	/// </ul></p>
	/// 
	/// <p>The predicted Nordsieck vector at step n+1 is computed from the Nordsieck vector at step
	/// n as follows:
	/// <ul>
	///   <li>Y<sub>n+1</sub> = y<sub>n</sub> + s<sub>1</sub>(n) + u<sup>T</sup> r<sub>n</sub></li>
	///   <li>S<sub>1</sub>(n+1) = h f(t<sub>n+1</sub>, Y<sub>n+1</sub>)</li>
	///   <li>R<sub>n+1</sub> = (s<sub>1</sub>(n) - S<sub>1</sub>(n+1)) P<sup>-1</sup> u + P<sup>-1</sup> A P r<sub>n</sub></li>
	/// </ul>
	/// where A is a rows shifting matrix (the lower left part is an identity matrix):
	/// <pre>
	///        [ 0 0   ...  0 0 | 0 ]
	///        [ ---------------+---]
	///        [ 1 0   ...  0 0 | 0 ]
	///    A = [ 0 1   ...  0 0 | 0 ]
	///        [       ...      | 0 ]
	///        [ 0 0   ...  1 0 | 0 ]
	///        [ 0 0   ...  0 1 | 0 ]
	/// </pre>
	/// From this predicted vector, the corrected vector is computed as follows:
	/// <ul>
	///   <li>y<sub>n+1</sub> = y<sub>n</sub> + S<sub>1</sub>(n+1) + [ -1 +1 -1 +1 ... &plusmn;1 ] r<sub>n+1</sub></li>
	///   <li>s<sub>1</sub>(n+1) = h f(t<sub>n+1</sub>, y<sub>n+1</sub>)</li>
	///   <li>r<sub>n+1</sub> = R<sub>n+1</sub> + (s<sub>1</sub>(n+1) - S<sub>1</sub>(n+1)) P<sup>-1</sup> u</li>
	/// </ul>
	/// where the upper case Y<sub>n+1</sub>, S<sub>1</sub>(n+1) and R<sub>n+1</sub> represent the
	/// predicted states whereas the lower case y<sub>n+1</sub>, s<sub>n+1</sub> and r<sub>n+1</sub>
	/// represent the corrected states.</p>
	/// 
	/// <p>The P<sup>-1</sup>u vector and the P<sup>-1</sup> A P matrix do not depend on the state,
	/// they only depend on k and therefore are precomputed once for all.</p>
	/// 
	/// @version $Id: AdamsMoultonIntegrator.java 1463684 2013-04-02 19:04:13Z luc $
	/// @since 2.0
	/// </summary>
	public class AdamsMoultonIntegrator : AdamsIntegrator
	{

		/// <summary>
		/// Integrator method name. </summary>
		private const string METHOD_NAME = "Adams-Moulton";

		/// <summary>
		/// Build an Adams-Moulton integrator with the given order and error control parameters. </summary>
		/// <param name="nSteps"> number of steps of the method excluding the one being computed </param>
		/// <param name="minStep"> minimal step (sign is irrelevant, regardless of
		/// integration direction, forward or backward), the last step can
		/// be smaller than this </param>
		/// <param name="maxStep"> maximal step (sign is irrelevant, regardless of
		/// integration direction, forward or backward), the last step can
		/// be smaller than this </param>
		/// <param name="scalAbsoluteTolerance"> allowed absolute error </param>
		/// <param name="scalRelativeTolerance"> allowed relative error </param>
		/// <exception cref="NumberIsTooSmallException"> if order is 1 or less </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AdamsMoultonIntegrator(final int nSteps, final double minStep, final double maxStep, final double scalAbsoluteTolerance, final double scalRelativeTolerance) throws org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public AdamsMoultonIntegrator(int nSteps, double minStep, double maxStep, double scalAbsoluteTolerance, double scalRelativeTolerance) : base(METHOD_NAME, nSteps, nSteps + 1, minStep, maxStep, scalAbsoluteTolerance, scalRelativeTolerance)
		{
		}

		/// <summary>
		/// Build an Adams-Moulton integrator with the given order and error control parameters. </summary>
		/// <param name="nSteps"> number of steps of the method excluding the one being computed </param>
		/// <param name="minStep"> minimal step (sign is irrelevant, regardless of
		/// integration direction, forward or backward), the last step can
		/// be smaller than this </param>
		/// <param name="maxStep"> maximal step (sign is irrelevant, regardless of
		/// integration direction, forward or backward), the last step can
		/// be smaller than this </param>
		/// <param name="vecAbsoluteTolerance"> allowed absolute error </param>
		/// <param name="vecRelativeTolerance"> allowed relative error </param>
		/// <exception cref="IllegalArgumentException"> if order is 1 or less </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AdamsMoultonIntegrator(final int nSteps, final double minStep, final double maxStep, final double[] vecAbsoluteTolerance, final double[] vecRelativeTolerance) throws IllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public AdamsMoultonIntegrator(int nSteps, double minStep, double maxStep, double[] vecAbsoluteTolerance, double[] vecRelativeTolerance) : base(METHOD_NAME, nSteps, nSteps + 1, minStep, maxStep, vecAbsoluteTolerance, vecRelativeTolerance)
		{
		}


		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void integrate(final org.apache.commons.math3.ode.ExpandableStatefulODE equations,final double t) throws org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MaxCountExceededException, org.apache.commons.math3.exception.NoBracketingException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void integrate(ExpandableStatefulODE equations, double t)
		{

			sanityChecks(equations, t);
			Equations = equations;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean forward = t > equations.getTime();
			bool forward = t > equations.Time;

			// initialize working arrays
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y0 = equations.getCompleteState();
			double[] y0 = equations.CompleteState;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y = y0.clone();
			double[] y = y0.clone();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yDot = new double[y.length];
			double[] yDot = new double[y.Length];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yTmp = new double[y.length];
			double[] yTmp = new double[y.Length];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] predictedScaled = new double[y.length];
			double[] predictedScaled = new double[y.Length];
			Array2DRowRealMatrix nordsieckTmp = null;

			// set up two interpolators sharing the integrator arrays
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.ode.sampling.NordsieckStepInterpolator interpolator = new org.apache.commons.math3.ode.sampling.NordsieckStepInterpolator();
			NordsieckStepInterpolator interpolator = new NordsieckStepInterpolator();
			interpolator.reinitialize(y, forward, equations.PrimaryMapper, equations.SecondaryMappers);

			// set up integration control objects
			initIntegration(equations.Time, y0, t);

			// compute the initial Nordsieck vector using the configured starter integrator
			start(equations.Time, y, t);
			interpolator.reinitialize(stepStart, stepSize, scaled, nordsieck);
			interpolator.storeTime(stepStart);

			double hNew = stepSize;
			interpolator.rescale(hNew);

			isLastStep = false;
			do
			{

				double error = 10;
				while (error >= 1.0)
				{

					stepSize = hNew;

					// predict a first estimate of the state at step end (P in the PECE sequence)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double stepEnd = stepStart + stepSize;
					double stepEnd = stepStart + stepSize;
					interpolator.InterpolatedTime = stepEnd;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.ode.ExpandableStatefulODE expandable = getExpandable();
					ExpandableStatefulODE expandable = Expandable;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.ode.EquationsMapper primary = expandable.getPrimaryMapper();
					EquationsMapper primary = expandable.PrimaryMapper;
					primary.insertEquationData(interpolator.InterpolatedState, yTmp);
					int index = 0;
					foreach (EquationsMapper secondary in expandable.SecondaryMappers)
					{
						secondary.insertEquationData(interpolator.getInterpolatedSecondaryState(index), yTmp);
						++index;
					}

					// evaluate a first estimate of the derivative (first E in the PECE sequence)
					computeDerivatives(stepEnd, yTmp, yDot);

					// update Nordsieck vector
					for (int j = 0; j < y0.Length; ++j)
					{
						predictedScaled[j] = stepSize * yDot[j];
					}
					nordsieckTmp = updateHighOrderDerivativesPhase1(nordsieck);
					updateHighOrderDerivativesPhase2(scaled, predictedScaled, nordsieckTmp);

					// apply correction (C in the PECE sequence)
					error = nordsieckTmp.walkInOptimizedOrder(new Corrector(this, y, predictedScaled, yTmp));

					if (error >= 1.0)
					{
						// reject the step and attempt to reduce error by stepsize control
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double factor = computeStepGrowShrinkFactor(error);
						double factor = computeStepGrowShrinkFactor(error);
						hNew = filterStep(stepSize * factor, forward, false);
						interpolator.rescale(hNew);
					}
				}

				// evaluate a final estimate of the derivative (second E in the PECE sequence)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double stepEnd = stepStart + stepSize;
				double stepEnd = stepStart + stepSize;
				computeDerivatives(stepEnd, yTmp, yDot);

				// update Nordsieck vector
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] correctedScaled = new double[y0.length];
				double[] correctedScaled = new double[y0.Length];
				for (int j = 0; j < y0.Length; ++j)
				{
					correctedScaled[j] = stepSize * yDot[j];
				}
				updateHighOrderDerivativesPhase2(predictedScaled, correctedScaled, nordsieckTmp);

				// discrete events handling
				Array.Copy(yTmp, 0, y, 0, y.Length);
				interpolator.reinitialize(stepEnd, stepSize, correctedScaled, nordsieckTmp);
				interpolator.storeTime(stepStart);
				interpolator.shift();
				interpolator.storeTime(stepEnd);
				stepStart = acceptStep(interpolator, y, yDot, t);
				scaled = correctedScaled;
				nordsieck = nordsieckTmp;

				if (!isLastStep)
				{

					// prepare next step
					interpolator.storeTime(stepStart);

					if (resetOccurred)
					{
						// some events handler has triggered changes that
						// invalidate the derivatives, we need to restart from scratch
						start(stepStart, y, t);
						interpolator.reinitialize(stepStart, stepSize, scaled, nordsieck);

					}

					// stepsize control for next step
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double factor = computeStepGrowShrinkFactor(error);
					double factor = computeStepGrowShrinkFactor(error);
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

					interpolator.rescale(hNew);
				}

			} while (!isLastStep);

			// dispatch results
			equations.Time = stepStart;
			equations.CompleteState = y;

			resetInternalState();

		}

		/// <summary>
		/// Corrector for current state in Adams-Moulton method.
		/// <p>
		/// This visitor implements the Taylor series formula:
		/// <pre>
		/// Y<sub>n+1</sub> = y<sub>n</sub> + s<sub>1</sub>(n+1) + [ -1 +1 -1 +1 ... &plusmn;1 ] r<sub>n+1</sub>
		/// </pre>
		/// </p>
		/// </summary>
		private class Corrector : RealMatrixPreservingVisitor
		{
			private readonly AdamsMoultonIntegrator outerInstance;


			/// <summary>
			/// Previous state. </summary>
			internal readonly double[] previous;

			/// <summary>
			/// Current scaled first derivative. </summary>
			internal readonly double[] scaled;

			/// <summary>
			/// Current state before correction. </summary>
			internal readonly double[] before;

			/// <summary>
			/// Current state after correction. </summary>
			internal readonly double[] after;

			/// <summary>
			/// Simple constructor. </summary>
			/// <param name="previous"> previous state </param>
			/// <param name="scaled"> current scaled first derivative </param>
			/// <param name="state"> state to correct (will be overwritten after visit) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Corrector(final double[] previous, final double[] scaled, final double[] state)
			public Corrector(AdamsMoultonIntegrator outerInstance, double[] previous, double[] scaled, double[] state)
			{
				this.outerInstance = outerInstance;
				this.previous = previous;
				this.scaled = scaled;
				this.after = state;
				this.before = state.clone();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual void start(int rows, int columns, int startRow, int endRow, int startColumn, int endColumn)
			{
				Arrays.fill(after, 0.0);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual void visit(int row, int column, double value)
			{
				if ((row & 0x1) == 0)
				{
					after[column] -= value;
				}
				else
				{
					after[column] += value;
				}
			}

			/// <summary>
			/// End visiting the Nordsieck vector.
			/// <p>The correction is used to control stepsize. So its amplitude is
			/// considered to be an error, which must be normalized according to
			/// error control settings. If the normalized value is greater than 1,
			/// the correction was too large and the step must be rejected.</p> </summary>
			/// <returns> the normalized correction, if greater than 1, the step
			/// must be rejected </returns>
			public virtual double end()
			{

				double error = 0;
				for (int i = 0; i < after.Length; ++i)
				{
					after[i] += previous[i] + scaled[i];
					if (i < outerInstance.mainSetDimension)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yScale = org.apache.commons.math3.util.FastMath.max(org.apache.commons.math3.util.FastMath.abs(previous[i]), org.apache.commons.math3.util.FastMath.abs(after[i]));
						double yScale = FastMath.max(FastMath.abs(previous[i]), FastMath.abs(after[i]));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tol = (vecAbsoluteTolerance == null) ? (scalAbsoluteTolerance + scalRelativeTolerance * yScale) : (vecAbsoluteTolerance[i] + vecRelativeTolerance[i] * yScale);
						double tol = (outerInstance.vecAbsoluteTolerance == null) ? (outerInstance.scalAbsoluteTolerance + outerInstance.scalRelativeTolerance * yScale) : (outerInstance.vecAbsoluteTolerance[i] + outerInstance.vecRelativeTolerance[i] * yScale);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ratio = (after[i] - before[i]) / tol;
						double ratio = (after[i] - before[i]) / tol;
						error += ratio * ratio;
					}
				}

				return FastMath.sqrt(error / outerInstance.mainSetDimension);

			}
		}

	}

}
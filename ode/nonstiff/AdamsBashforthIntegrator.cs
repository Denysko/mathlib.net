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
	using Array2DRowRealMatrix = mathlib.linear.Array2DRowRealMatrix;
	using NordsieckStepInterpolator = mathlib.ode.sampling.NordsieckStepInterpolator;
	using FastMath = mathlib.util.FastMath;


	/// <summary>
	/// This class implements explicit Adams-Bashforth integrators for Ordinary
	/// Differential Equations.
	/// 
	/// <p>Adams-Bashforth methods (in fact due to Adams alone) are explicit
	/// multistep ODE solvers. This implementation is a variation of the classical
	/// one: it uses adaptive stepsize to implement error control, whereas
	/// classical implementations are fixed step size. The value of state vector
	/// at step n+1 is a simple combination of the value at step n and of the
	/// derivatives at steps n, n-1, n-2 ... Depending on the number k of previous
	/// steps one wants to use for computing the next value, different formulas
	/// are available:</p>
	/// <ul>
	///   <li>k = 1: y<sub>n+1</sub> = y<sub>n</sub> + h y'<sub>n</sub></li>
	///   <li>k = 2: y<sub>n+1</sub> = y<sub>n</sub> + h (3y'<sub>n</sub>-y'<sub>n-1</sub>)/2</li>
	///   <li>k = 3: y<sub>n+1</sub> = y<sub>n</sub> + h (23y'<sub>n</sub>-16y'<sub>n-1</sub>+5y'<sub>n-2</sub>)/12</li>
	///   <li>k = 4: y<sub>n+1</sub> = y<sub>n</sub> + h (55y'<sub>n</sub>-59y'<sub>n-1</sub>+37y'<sub>n-2</sub>-9y'<sub>n-3</sub>)/24</li>
	///   <li>...</li>
	/// </ul>
	/// 
	/// <p>A k-steps Adams-Bashforth method is of order k.</p>
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
	/// Adams-Bashforth methods can be written:
	/// <ul>
	///   <li>k = 1: y<sub>n+1</sub> = y<sub>n</sub> + s<sub>1</sub>(n)</li>
	///   <li>k = 2: y<sub>n+1</sub> = y<sub>n</sub> + 3/2 s<sub>1</sub>(n) + [ -1/2 ] q<sub>n</sub></li>
	///   <li>k = 3: y<sub>n+1</sub> = y<sub>n</sub> + 23/12 s<sub>1</sub>(n) + [ -16/12 5/12 ] q<sub>n</sub></li>
	///   <li>k = 4: y<sub>n+1</sub> = y<sub>n</sub> + 55/24 s<sub>1</sub>(n) + [ -59/24 37/24 -9/24 ] q<sub>n</sub></li>
	///   <li>...</li>
	/// </ul></p>
	/// 
	/// <p>Instead of using the classical representation with first derivatives only (y<sub>n</sub>,
	/// s<sub>1</sub>(n) and q<sub>n</sub>), our implementation uses the Nordsieck vector with
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
	/// <p>The Nordsieck vector at step n+1 is computed from the Nordsieck vector at step n as follows:
	/// <ul>
	///   <li>y<sub>n+1</sub> = y<sub>n</sub> + s<sub>1</sub>(n) + u<sup>T</sup> r<sub>n</sub></li>
	///   <li>s<sub>1</sub>(n+1) = h f(t<sub>n+1</sub>, y<sub>n+1</sub>)</li>
	///   <li>r<sub>n+1</sub> = (s<sub>1</sub>(n) - s<sub>1</sub>(n+1)) P<sup>-1</sup> u + P<sup>-1</sup> A P r<sub>n</sub></li>
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
	/// </pre></p>
	/// 
	/// <p>The P<sup>-1</sup>u vector and the P<sup>-1</sup> A P matrix do not depend on the state,
	/// they only depend on k and therefore are precomputed once for all.</p>
	/// 
	/// @version $Id: AdamsBashforthIntegrator.java 1463684 2013-04-02 19:04:13Z luc $
	/// @since 2.0
	/// </summary>
	public class AdamsBashforthIntegrator : AdamsIntegrator
	{

		/// <summary>
		/// Integrator method name. </summary>
		private const string METHOD_NAME = "Adams-Bashforth";

		/// <summary>
		/// Build an Adams-Bashforth integrator with the given order and step control parameters. </summary>
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
//ORIGINAL LINE: public AdamsBashforthIntegrator(final int nSteps, final double minStep, final double maxStep, final double scalAbsoluteTolerance, final double scalRelativeTolerance) throws mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public AdamsBashforthIntegrator(int nSteps, double minStep, double maxStep, double scalAbsoluteTolerance, double scalRelativeTolerance) : base(METHOD_NAME, nSteps, nSteps, minStep, maxStep, scalAbsoluteTolerance, scalRelativeTolerance)
		{
		}

		/// <summary>
		/// Build an Adams-Bashforth integrator with the given order and step control parameters. </summary>
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
//ORIGINAL LINE: public AdamsBashforthIntegrator(final int nSteps, final double minStep, final double maxStep, final double[] vecAbsoluteTolerance, final double[] vecRelativeTolerance) throws IllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public AdamsBashforthIntegrator(int nSteps, double minStep, double maxStep, double[] vecAbsoluteTolerance, double[] vecRelativeTolerance) : base(METHOD_NAME, nSteps, nSteps, minStep, maxStep, vecAbsoluteTolerance, vecRelativeTolerance)
		{
		}

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

			// set up an interpolator sharing the integrator arrays
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.ode.sampling.NordsieckStepInterpolator interpolator = new mathlib.ode.sampling.NordsieckStepInterpolator();
			NordsieckStepInterpolator interpolator = new NordsieckStepInterpolator();
			interpolator.reinitialize(y, forward, equations.PrimaryMapper, equations.SecondaryMappers);

			// set up integration control objects
			initIntegration(equations.Time, y0, t);

			// compute the initial Nordsieck vector using the configured starter integrator
			start(equations.Time, y, t);
			interpolator.reinitialize(stepStart, stepSize, scaled, nordsieck);
			interpolator.storeTime(stepStart);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lastRow = nordsieck.getRowDimension() - 1;
			int lastRow = nordsieck.RowDimension - 1;

			// reuse the step that was chosen by the starter integrator
			double hNew = stepSize;
			interpolator.rescale(hNew);

			// main integration loop
			isLastStep = false;
			do
			{

				double error = 10;
				while (error >= 1.0)
				{

					stepSize = hNew;

					// evaluate error using the last term of the Taylor expansion
					error = 0;
					for (int i = 0; i < mainSetDimension; ++i)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yScale = mathlib.util.FastMath.abs(y[i]);
						double yScale = FastMath.abs(y[i]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tol = (vecAbsoluteTolerance == null) ? (scalAbsoluteTolerance + scalRelativeTolerance * yScale) : (vecAbsoluteTolerance[i] + vecRelativeTolerance[i] * yScale);
						double tol = (vecAbsoluteTolerance == null) ? (scalAbsoluteTolerance + scalRelativeTolerance * yScale) : (vecAbsoluteTolerance[i] + vecRelativeTolerance[i] * yScale);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ratio = nordsieck.getEntry(lastRow, i) / tol;
						double ratio = nordsieck.getEntry(lastRow, i) / tol;
						error += ratio * ratio;
					}
					error = FastMath.sqrt(error / mainSetDimension);

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

				// predict a first estimate of the state at step end
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double stepEnd = stepStart + stepSize;
				double stepEnd = stepStart + stepSize;
				interpolator.shift();
				interpolator.InterpolatedTime = stepEnd;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.ode.ExpandableStatefulODE expandable = getExpandable();
				ExpandableStatefulODE expandable = Expandable;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.ode.EquationsMapper primary = expandable.getPrimaryMapper();
				EquationsMapper primary = expandable.PrimaryMapper;
				primary.insertEquationData(interpolator.InterpolatedState, y);
				int index = 0;
				foreach (EquationsMapper secondary in expandable.SecondaryMappers)
				{
					secondary.insertEquationData(interpolator.getInterpolatedSecondaryState(index), y);
					++index;
				}

				// evaluate the derivative
				computeDerivatives(stepEnd, y, yDot);

				// update Nordsieck vector
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] predictedScaled = new double[y0.length];
				double[] predictedScaled = new double[y0.Length];
				for (int j = 0; j < y0.Length; ++j)
				{
					predictedScaled[j] = stepSize * yDot[j];
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.Array2DRowRealMatrix nordsieckTmp = updateHighOrderDerivativesPhase1(nordsieck);
				Array2DRowRealMatrix nordsieckTmp = updateHighOrderDerivativesPhase1(nordsieck);
				updateHighOrderDerivativesPhase2(scaled, predictedScaled, nordsieckTmp);
				interpolator.reinitialize(stepEnd, stepSize, predictedScaled, nordsieckTmp);

				// discrete events handling
				interpolator.storeTime(stepEnd);
				stepStart = acceptStep(interpolator, y, yDot, t);
				scaled = predictedScaled;
				nordsieck = nordsieckTmp;
				interpolator.reinitialize(stepEnd, stepSize, scaled, nordsieck);

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

	}

}
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
namespace org.apache.commons.math3.dfp
{


	using AllowedSolution = org.apache.commons.math3.analysis.solvers.AllowedSolution;
	using MathInternalError = org.apache.commons.math3.exception.MathInternalError;
	using NoBracketingException = org.apache.commons.math3.exception.NoBracketingException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using Incrementor = org.apache.commons.math3.util.Incrementor;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// This class implements a modification of the <a
	/// href="http://mathworld.wolfram.com/BrentsMethod.html"> Brent algorithm</a>.
	/// <p>
	/// The changes with respect to the original Brent algorithm are:
	/// <ul>
	///   <li>the returned value is chosen in the current interval according
	///   to user specified <seealso cref="AllowedSolution"/>,</li>
	///   <li>the maximal order for the invert polynomial root search is
	///   user-specified instead of being invert quadratic only</li>
	/// </ul>
	/// </p>
	/// The given interval must bracket the root.
	/// 
	/// @version $Id: BracketingNthOrderBrentSolverDFP.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class BracketingNthOrderBrentSolverDFP
	{

	   /// <summary>
	   /// Maximal aging triggering an attempt to balance the bracketing interval. </summary>
		private const int MAXIMAL_AGING = 2;

		/// <summary>
		/// Maximal order. </summary>
		private readonly int maximalOrder;

		/// <summary>
		/// Function value accuracy. </summary>
		private readonly Dfp functionValueAccuracy;

		/// <summary>
		/// Absolute accuracy. </summary>
		private readonly Dfp absoluteAccuracy;

		/// <summary>
		/// Relative accuracy. </summary>
		private readonly Dfp relativeAccuracy;

		/// <summary>
		/// Evaluations counter. </summary>
		private readonly Incrementor evaluations = new Incrementor();

		/// <summary>
		/// Construct a solver.
		/// </summary>
		/// <param name="relativeAccuracy"> Relative accuracy. </param>
		/// <param name="absoluteAccuracy"> Absolute accuracy. </param>
		/// <param name="functionValueAccuracy"> Function value accuracy. </param>
		/// <param name="maximalOrder"> maximal order. </param>
		/// <exception cref="NumberIsTooSmallException"> if maximal order is lower than 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BracketingNthOrderBrentSolverDFP(final Dfp relativeAccuracy, final Dfp absoluteAccuracy, final Dfp functionValueAccuracy, final int maximalOrder) throws org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public BracketingNthOrderBrentSolverDFP(Dfp relativeAccuracy, Dfp absoluteAccuracy, Dfp functionValueAccuracy, int maximalOrder)
		{
			if (maximalOrder < 2)
			{
				throw new NumberIsTooSmallException(maximalOrder, 2, true);
			}
			this.maximalOrder = maximalOrder;
			this.absoluteAccuracy = absoluteAccuracy;
			this.relativeAccuracy = relativeAccuracy;
			this.functionValueAccuracy = functionValueAccuracy;
		}

		/// <summary>
		/// Get the maximal order. </summary>
		/// <returns> maximal order </returns>
		public virtual int MaximalOrder
		{
			get
			{
				return maximalOrder;
			}
		}

		/// <summary>
		/// Get the maximal number of function evaluations.
		/// </summary>
		/// <returns> the maximal number of function evaluations. </returns>
		public virtual int MaxEvaluations
		{
			get
			{
				return evaluations.MaximalCount;
			}
		}

		/// <summary>
		/// Get the number of evaluations of the objective function.
		/// The number of evaluations corresponds to the last call to the
		/// {@code optimize} method. It is 0 if the method has not been
		/// called yet.
		/// </summary>
		/// <returns> the number of evaluations of the objective function. </returns>
		public virtual int Evaluations
		{
			get
			{
				return evaluations.Count;
			}
		}

		/// <summary>
		/// Get the absolute accuracy. </summary>
		/// <returns> absolute accuracy </returns>
		public virtual Dfp AbsoluteAccuracy
		{
			get
			{
				return absoluteAccuracy;
			}
		}

		/// <summary>
		/// Get the relative accuracy. </summary>
		/// <returns> relative accuracy </returns>
		public virtual Dfp RelativeAccuracy
		{
			get
			{
				return relativeAccuracy;
			}
		}

		/// <summary>
		/// Get the function accuracy. </summary>
		/// <returns> function accuracy </returns>
		public virtual Dfp FunctionValueAccuracy
		{
			get
			{
				return functionValueAccuracy;
			}
		}

		/// <summary>
		/// Solve for a zero in the given interval.
		/// A solver may require that the interval brackets a single zero root.
		/// Solvers that do require bracketing should be able to handle the case
		/// where one of the endpoints is itself a root.
		/// </summary>
		/// <param name="maxEval"> Maximum number of evaluations. </param>
		/// <param name="f"> Function to solve. </param>
		/// <param name="min"> Lower bound for the interval. </param>
		/// <param name="max"> Upper bound for the interval. </param>
		/// <param name="allowedSolution"> The kind of solutions that the root-finding algorithm may
		/// accept as solutions. </param>
		/// <returns> a value where the function is zero. </returns>
		/// <exception cref="NullArgumentException"> if f is null. </exception>
		/// <exception cref="NoBracketingException"> if root cannot be bracketed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Dfp solve(final int maxEval, final UnivariateDfpFunction f, final Dfp min, final Dfp max, final org.apache.commons.math3.analysis.solvers.AllowedSolution allowedSolution) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoBracketingException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual Dfp solve(int maxEval, UnivariateDfpFunction f, Dfp min, Dfp max, AllowedSolution allowedSolution)
		{
			return solve(maxEval, f, min, max, min.add(max).divide(2), allowedSolution);
		}

		/// <summary>
		/// Solve for a zero in the given interval, start at {@code startValue}.
		/// A solver may require that the interval brackets a single zero root.
		/// Solvers that do require bracketing should be able to handle the case
		/// where one of the endpoints is itself a root.
		/// </summary>
		/// <param name="maxEval"> Maximum number of evaluations. </param>
		/// <param name="f"> Function to solve. </param>
		/// <param name="min"> Lower bound for the interval. </param>
		/// <param name="max"> Upper bound for the interval. </param>
		/// <param name="startValue"> Start value to use. </param>
		/// <param name="allowedSolution"> The kind of solutions that the root-finding algorithm may
		/// accept as solutions. </param>
		/// <returns> a value where the function is zero. </returns>
		/// <exception cref="NullArgumentException"> if f is null. </exception>
		/// <exception cref="NoBracketingException"> if root cannot be bracketed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Dfp solve(final int maxEval, final UnivariateDfpFunction f, final Dfp min, final Dfp max, final Dfp startValue, final org.apache.commons.math3.analysis.solvers.AllowedSolution allowedSolution) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoBracketingException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual Dfp solve(int maxEval, UnivariateDfpFunction f, Dfp min, Dfp max, Dfp startValue, AllowedSolution allowedSolution)
		{

			// Checks.
			MathUtils.checkNotNull(f);

			// Reset.
			evaluations.MaximalCount = maxEval;
			evaluations.resetCount();
			Dfp zero = startValue.Zero;
			Dfp nan = zero.newInstance((sbyte) 1, Dfp.QNAN);

			// prepare arrays with the first points
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp[] x = new Dfp[maximalOrder + 1];
			Dfp[] x = new Dfp[maximalOrder + 1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp[] y = new Dfp[maximalOrder + 1];
			Dfp[] y = new Dfp[maximalOrder + 1];
			x[0] = min;
			x[1] = startValue;
			x[2] = max;

			// evaluate initial guess
			evaluations.incrementCount();
			y[1] = f.value(x[1]);
			if (y[1].Zero)
			{
				// return the initial guess if it is a perfect root.
				return x[1];
			}

			// evaluate first  endpoint
			evaluations.incrementCount();
			y[0] = f.value(x[0]);
			if (y[0].Zero)
			{
				// return the first endpoint if it is a perfect root.
				return x[0];
			}

			int nbPoints;
			int signChangeIndex;
			if (y[0].multiply(y[1]).negativeOrNull())
			{

				// reduce interval if it brackets the root
				nbPoints = 2;
				signChangeIndex = 1;

			}
			else
			{

				// evaluate second endpoint
				evaluations.incrementCount();
				y[2] = f.value(x[2]);
				if (y[2].Zero)
				{
					// return the second endpoint if it is a perfect root.
					return x[2];
				}

				if (y[1].multiply(y[2]).negativeOrNull())
				{
					// use all computed point as a start sampling array for solving
					nbPoints = 3;
					signChangeIndex = 2;
				}
				else
				{
					throw new NoBracketingException(x[0].toDouble(), x[2].toDouble(), y[0].toDouble(), y[2].toDouble());
				}

			}

			// prepare a work array for inverse polynomial interpolation
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp[] tmpX = new Dfp[x.length];
			Dfp[] tmpX = new Dfp[x.Length];

			// current tightest bracketing of the root
			Dfp xA = x[signChangeIndex - 1];
			Dfp yA = y[signChangeIndex - 1];
			Dfp absXA = xA.abs();
			Dfp absYA = yA.abs();
			int agingA = 0;
			Dfp xB = x[signChangeIndex];
			Dfp yB = y[signChangeIndex];
			Dfp absXB = xB.abs();
			Dfp absYB = yB.abs();
			int agingB = 0;

			// search loop
			while (true)
			{

				// check convergence of bracketing interval
				Dfp maxX = absXA.lessThan(absXB) ? absXB : absXA;
				Dfp maxY = absYA.lessThan(absYB) ? absYB : absYA;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp xTol = absoluteAccuracy.add(relativeAccuracy.multiply(maxX));
				Dfp xTol = absoluteAccuracy.add(relativeAccuracy.multiply(maxX));
				if (xB.subtract(xA).subtract(xTol).negativeOrNull() || maxY.lessThan(functionValueAccuracy))
				{
					switch (allowedSolution)
					{
					case AllowedSolution.ANY_SIDE:
						return absYA.lessThan(absYB) ? xA : xB;
					case AllowedSolution.LEFT_SIDE:
						return xA;
					case AllowedSolution.RIGHT_SIDE:
						return xB;
					case AllowedSolution.BELOW_SIDE:
						return yA.lessThan(zero) ? xA : xB;
					case AllowedSolution.ABOVE_SIDE:
						return yA.lessThan(zero) ? xB : xA;
					default :
						// this should never happen
						throw new MathInternalError(null);
					}
				}

				// target for the next evaluation point
				Dfp targetY;
				if (agingA >= MAXIMAL_AGING)
				{
					// we keep updating the high bracket, try to compensate this
					targetY = yB.divide(16).negate();
				}
				else if (agingB >= MAXIMAL_AGING)
				{
					// we keep updating the low bracket, try to compensate this
					targetY = yA.divide(16).negate();
				}
				else
				{
					// bracketing is balanced, try to find the root itself
					targetY = zero;
				}

				// make a few attempts to guess a root,
				Dfp nextX;
				int start = 0;
				int end = nbPoints;
				do
				{

					// guess a value for current target, using inverse polynomial interpolation
					Array.Copy(x, start, tmpX, start, end - start);
					nextX = guessX(targetY, tmpX, y, start, end);

					if (!(nextX.greaterThan(xA) && nextX.lessThan(xB)))
					{
						// the guessed root is not strictly inside of the tightest bracketing interval

						// the guessed root is either not strictly inside the interval or it
						// is a NaN (which occurs when some sampling points share the same y)
						// we try again with a lower interpolation order
						if (signChangeIndex - start >= end - signChangeIndex)
						{
							// we have more points before the sign change, drop the lowest point
							++start;
						}
						else
						{
							// we have more points after sign change, drop the highest point
							--end;
						}

						// we need to do one more attempt
						nextX = nan;

					}

				} while (nextX.NaN && (end - start > 1));

				if (nextX.NaN)
				{
					// fall back to bisection
					nextX = xA.add(xB.subtract(xA).divide(2));
					start = signChangeIndex - 1;
					end = signChangeIndex;
				}

				// evaluate the function at the guessed root
				evaluations.incrementCount();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp nextY = f.value(nextX);
				Dfp nextY = f.value(nextX);
				if (nextY.Zero)
				{
					// we have found an exact root, since it is not an approximation
					// we don't need to bother about the allowed solutions setting
					return nextX;
				}

				if ((nbPoints > 2) && (end - start != nbPoints))
				{

					// we have been forced to ignore some points to keep bracketing,
					// they are probably too far from the root, drop them from now on
					nbPoints = end - start;
					Array.Copy(x, start, x, 0, nbPoints);
					Array.Copy(y, start, y, 0, nbPoints);
					signChangeIndex -= start;

				}
				else if (nbPoints == x.Length)
				{

					// we have to drop one point in order to insert the new one
					nbPoints--;

					// keep the tightest bracketing interval as centered as possible
					if (signChangeIndex >= (x.Length + 1) / 2)
					{
						// we drop the lowest point, we have to shift the arrays and the index
						Array.Copy(x, 1, x, 0, nbPoints);
						Array.Copy(y, 1, y, 0, nbPoints);
						--signChangeIndex;
					}

				}

				// insert the last computed point
				//(by construction, we know it lies inside the tightest bracketing interval)
				Array.Copy(x, signChangeIndex, x, signChangeIndex + 1, nbPoints - signChangeIndex);
				x[signChangeIndex] = nextX;
				Array.Copy(y, signChangeIndex, y, signChangeIndex + 1, nbPoints - signChangeIndex);
				y[signChangeIndex] = nextY;
				++nbPoints;

				// update the bracketing interval
				if (nextY.multiply(yA).negativeOrNull())
				{
					// the sign change occurs before the inserted point
					xB = nextX;
					yB = nextY;
					absYB = yB.abs();
					++agingA;
					agingB = 0;
				}
				else
				{
					// the sign change occurs after the inserted point
					xA = nextX;
					yA = nextY;
					absYA = yA.abs();
					agingA = 0;
					++agingB;

					// update the sign change index
					signChangeIndex++;

				}

			}

		}

		/// <summary>
		/// Guess an x value by n<sup>th</sup> order inverse polynomial interpolation.
		/// <p>
		/// The x value is guessed by evaluating polynomial Q(y) at y = targetY, where Q
		/// is built such that for all considered points (x<sub>i</sub>, y<sub>i</sub>),
		/// Q(y<sub>i</sub>) = x<sub>i</sub>.
		/// </p> </summary>
		/// <param name="targetY"> target value for y </param>
		/// <param name="x"> reference points abscissas for interpolation,
		/// note that this array <em>is</em> modified during computation </param>
		/// <param name="y"> reference points ordinates for interpolation </param>
		/// <param name="start"> start index of the points to consider (inclusive) </param>
		/// <param name="end"> end index of the points to consider (exclusive) </param>
		/// <returns> guessed root (will be a NaN if two points share the same y) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Dfp guessX(final Dfp targetY, final Dfp[] x, final Dfp[] y, final int start, final int end)
		private Dfp guessX(Dfp targetY, Dfp[] x, Dfp[] y, int start, int end)
		{

			// compute Q Newton coefficients by divided differences
			for (int i = start; i < end - 1; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int delta = i + 1 - start;
				int delta = i + 1 - start;
				for (int j = end - 1; j > i; --j)
				{
					x[j] = x[j].subtract(x[j - 1]).divide(y[j].subtract(y[j - delta]));
				}
			}

			// evaluate Q(targetY)
			Dfp x0 = targetY.Zero;
			for (int j = end - 1; j >= start; --j)
			{
				x0 = x[j].add(x0.multiply(targetY.subtract(y[j])));
			}

			return x0;

		}

	}

}
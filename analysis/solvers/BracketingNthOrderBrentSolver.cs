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
namespace mathlib.analysis.solvers
{


	using MathInternalError = mathlib.exception.MathInternalError;
	using NoBracketingException = mathlib.exception.NoBracketingException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;
	using FastMath = mathlib.util.FastMath;
	using Precision = mathlib.util.Precision;

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
	/// @version $Id: BracketingNthOrderBrentSolver.java 1379560 2012-08-31 19:40:30Z erans $
	/// </summary>
	public class BracketingNthOrderBrentSolver : AbstractUnivariateSolver, BracketedUnivariateSolver<UnivariateFunction>
	{

		/// <summary>
		/// Default absolute accuracy. </summary>
		private const double DEFAULT_ABSOLUTE_ACCURACY = 1e-6;

		/// <summary>
		/// Default maximal order. </summary>
		private const int DEFAULT_MAXIMAL_ORDER = 5;

		/// <summary>
		/// Maximal aging triggering an attempt to balance the bracketing interval. </summary>
		private const int MAXIMAL_AGING = 2;

		/// <summary>
		/// Reduction factor for attempts to balance the bracketing interval. </summary>
		private const double REDUCTION_FACTOR = 1.0 / 16.0;

		/// <summary>
		/// Maximal order. </summary>
		private readonly int maximalOrder;

		/// <summary>
		/// The kinds of solutions that the algorithm may accept. </summary>
		private AllowedSolution allowed;

		/// <summary>
		/// Construct a solver with default accuracy and maximal order (1e-6 and 5 respectively)
		/// </summary>
		public BracketingNthOrderBrentSolver() : this(DEFAULT_ABSOLUTE_ACCURACY, DEFAULT_MAXIMAL_ORDER)
		{
		}

		/// <summary>
		/// Construct a solver.
		/// </summary>
		/// <param name="absoluteAccuracy"> Absolute accuracy. </param>
		/// <param name="maximalOrder"> maximal order. </param>
		/// <exception cref="NumberIsTooSmallException"> if maximal order is lower than 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BracketingNthOrderBrentSolver(final double absoluteAccuracy, final int maximalOrder) throws mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public BracketingNthOrderBrentSolver(double absoluteAccuracy, int maximalOrder) : base(absoluteAccuracy)
		{
			if (maximalOrder < 2)
			{
				throw new NumberIsTooSmallException(maximalOrder, 2, true);
			}
			this.maximalOrder = maximalOrder;
			this.allowed = AllowedSolution.ANY_SIDE;
		}

		/// <summary>
		/// Construct a solver.
		/// </summary>
		/// <param name="relativeAccuracy"> Relative accuracy. </param>
		/// <param name="absoluteAccuracy"> Absolute accuracy. </param>
		/// <param name="maximalOrder"> maximal order. </param>
		/// <exception cref="NumberIsTooSmallException"> if maximal order is lower than 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BracketingNthOrderBrentSolver(final double relativeAccuracy, final double absoluteAccuracy, final int maximalOrder) throws mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public BracketingNthOrderBrentSolver(double relativeAccuracy, double absoluteAccuracy, int maximalOrder) : base(relativeAccuracy, absoluteAccuracy)
		{
			if (maximalOrder < 2)
			{
				throw new NumberIsTooSmallException(maximalOrder, 2, true);
			}
			this.maximalOrder = maximalOrder;
			this.allowed = AllowedSolution.ANY_SIDE;
		}

		/// <summary>
		/// Construct a solver.
		/// </summary>
		/// <param name="relativeAccuracy"> Relative accuracy. </param>
		/// <param name="absoluteAccuracy"> Absolute accuracy. </param>
		/// <param name="functionValueAccuracy"> Function value accuracy. </param>
		/// <param name="maximalOrder"> maximal order. </param>
		/// <exception cref="NumberIsTooSmallException"> if maximal order is lower than 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BracketingNthOrderBrentSolver(final double relativeAccuracy, final double absoluteAccuracy, final double functionValueAccuracy, final int maximalOrder) throws mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public BracketingNthOrderBrentSolver(double relativeAccuracy, double absoluteAccuracy, double functionValueAccuracy, int maximalOrder) : base(relativeAccuracy, absoluteAccuracy, functionValueAccuracy)
		{
			if (maximalOrder < 2)
			{
				throw new NumberIsTooSmallException(maximalOrder, 2, true);
			}
			this.maximalOrder = maximalOrder;
			this.allowed = AllowedSolution.ANY_SIDE;
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
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected double doSolve() throws mathlib.exception.TooManyEvaluationsException, mathlib.exception.NumberIsTooLargeException, mathlib.exception.NoBracketingException
		protected internal override double doSolve()
		{
			// prepare arrays with the first points
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] x = new double[maximalOrder + 1];
			double[] x = new double[maximalOrder + 1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y = new double[maximalOrder + 1];
			double[] y = new double[maximalOrder + 1];
			x[0] = Min;
			x[1] = StartValue;
			x[2] = Max;
			verifySequence(x[0], x[1], x[2]);

			// evaluate initial guess
			y[1] = computeObjectiveValue(x[1]);
			if (Precision.Equals(y[1], 0.0, 1))
			{
				// return the initial guess if it is a perfect root.
				return x[1];
			}

			// evaluate first  endpoint
			y[0] = computeObjectiveValue(x[0]);
			if (Precision.Equals(y[0], 0.0, 1))
			{
				// return the first endpoint if it is a perfect root.
				return x[0];
			}

			int nbPoints;
			int signChangeIndex;
			if (y[0] * y[1] < 0)
			{

				// reduce interval if it brackets the root
				nbPoints = 2;
				signChangeIndex = 1;

			}
			else
			{

				// evaluate second endpoint
				y[2] = computeObjectiveValue(x[2]);
				if (Precision.Equals(y[2], 0.0, 1))
				{
					// return the second endpoint if it is a perfect root.
					return x[2];
				}

				if (y[1] * y[2] < 0)
				{
					// use all computed point as a start sampling array for solving
					nbPoints = 3;
					signChangeIndex = 2;
				}
				else
				{
					throw new NoBracketingException(x[0], x[2], y[0], y[2]);
				}

			}

			// prepare a work array for inverse polynomial interpolation
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tmpX = new double[x.length];
			double[] tmpX = new double[x.Length];

			// current tightest bracketing of the root
			double xA = x[signChangeIndex - 1];
			double yA = y[signChangeIndex - 1];
			double absYA = FastMath.abs(yA);
			int agingA = 0;
			double xB = x[signChangeIndex];
			double yB = y[signChangeIndex];
			double absYB = FastMath.abs(yB);
			int agingB = 0;

			// search loop
			while (true)
			{

				// check convergence of bracketing interval
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xTol = getAbsoluteAccuracy() + getRelativeAccuracy() * mathlib.util.FastMath.max(mathlib.util.FastMath.abs(xA), mathlib.util.FastMath.abs(xB));
				double xTol = AbsoluteAccuracy + RelativeAccuracy * FastMath.max(FastMath.abs(xA), FastMath.abs(xB));
				if (((xB - xA) <= xTol) || (FastMath.max(absYA, absYB) < FunctionValueAccuracy))
				{
					switch (allowed)
					{
					case mathlib.analysis.solvers.AllowedSolution.ANY_SIDE:
						return absYA < absYB ? xA : xB;
					case mathlib.analysis.solvers.AllowedSolution.LEFT_SIDE:
						return xA;
					case mathlib.analysis.solvers.AllowedSolution.RIGHT_SIDE:
						return xB;
					case mathlib.analysis.solvers.AllowedSolution.BELOW_SIDE:
						return (yA <= 0) ? xA : xB;
					case mathlib.analysis.solvers.AllowedSolution.ABOVE_SIDE:
						return (yA < 0) ? xB : xA;
					default :
						// this should never happen
						throw new MathInternalError();
					}
				}

				// target for the next evaluation point
				double targetY;
				if (agingA >= MAXIMAL_AGING)
				{
					// we keep updating the high bracket, try to compensate this
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int p = agingA - MAXIMAL_AGING;
					int p = agingA - MAXIMAL_AGING;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double weightA = (1 << p) - 1;
					double weightA = (1 << p) - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double weightB = p + 1;
					double weightB = p + 1;
					targetY = (weightA * yA - weightB * REDUCTION_FACTOR * yB) / (weightA + weightB);
				}
				else if (agingB >= MAXIMAL_AGING)
				{
					// we keep updating the low bracket, try to compensate this
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int p = agingB - MAXIMAL_AGING;
					int p = agingB - MAXIMAL_AGING;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double weightA = p + 1;
					double weightA = p + 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double weightB = (1 << p) - 1;
					double weightB = (1 << p) - 1;
					targetY = (weightB * yB - weightA * REDUCTION_FACTOR * yA) / (weightA + weightB);
				}
				else
				{
					// bracketing is balanced, try to find the root itself
					targetY = 0;
				}

				// make a few attempts to guess a root,
				double nextX;
				int start = 0;
				int end = nbPoints;
				do
				{

					// guess a value for current target, using inverse polynomial interpolation
					Array.Copy(x, start, tmpX, start, end - start);
					nextX = guessX(targetY, tmpX, y, start, end);

					if (!((nextX > xA) && (nextX < xB)))
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
						nextX = double.NaN;

					}

				} while (double.IsNaN(nextX) && (end - start > 1));

				if (double.IsNaN(nextX))
				{
					// fall back to bisection
					nextX = xA + 0.5 * (xB - xA);
					start = signChangeIndex - 1;
					end = signChangeIndex;
				}

				// evaluate the function at the guessed root
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double nextY = computeObjectiveValue(nextX);
				double nextY = computeObjectiveValue(nextX);
				if (Precision.Equals(nextY, 0.0, 1))
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
				if (nextY * yA <= 0)
				{
					// the sign change occurs before the inserted point
					xB = nextX;
					yB = nextY;
					absYB = FastMath.abs(yB);
					++agingA;
					agingB = 0;
				}
				else
				{
					// the sign change occurs after the inserted point
					xA = nextX;
					yA = nextY;
					absYA = FastMath.abs(yA);
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
//ORIGINAL LINE: private double guessX(final double targetY, final double[] x, final double[] y, final int start, final int end)
		private double guessX(double targetY, double[] x, double[] y, int start, int end)
		{

			// compute Q Newton coefficients by divided differences
			for (int i = start; i < end - 1; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int delta = i + 1 - start;
				int delta = i + 1 - start;
				for (int j = end - 1; j > i; --j)
				{
					x[j] = (x[j] - x[j - 1]) / (y[j] - y[j - delta]);
				}
			}

			// evaluate Q(targetY)
			double x0 = 0;
			for (int j = end - 1; j >= start; --j)
			{
				x0 = x[j] + x0 * (targetY - y[j]);
			}

			return x0;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double solve(int maxEval, mathlib.analysis.UnivariateFunction f, double min, double max, AllowedSolution allowedSolution) throws mathlib.exception.TooManyEvaluationsException, mathlib.exception.NumberIsTooLargeException, mathlib.exception.NoBracketingException
		public virtual double solve(int maxEval, UnivariateFunction f, double min, double max, AllowedSolution allowedSolution)
		{
			this.allowed = allowedSolution;
			return base.solve(maxEval, f, min, max);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double solve(int maxEval, mathlib.analysis.UnivariateFunction f, double min, double max, double startValue, AllowedSolution allowedSolution) throws mathlib.exception.TooManyEvaluationsException, mathlib.exception.NumberIsTooLargeException, mathlib.exception.NoBracketingException
		public virtual double solve(int maxEval, UnivariateFunction f, double min, double max, double startValue, AllowedSolution allowedSolution)
		{
			this.allowed = allowedSolution;
			return base.solve(maxEval, f, min, max, startValue);
		}

	}

}
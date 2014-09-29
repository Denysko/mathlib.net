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


	using NoBracketingException = mathlib.exception.NoBracketingException;
	using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using FastMath = mathlib.util.FastMath;
	using Precision = mathlib.util.Precision;

	/// <summary>
	/// This class implements the <a href="http://mathworld.wolfram.com/BrentsMethod.html">
	/// Brent algorithm</a> for finding zeros of real univariate functions.
	/// The function should be continuous but not necessarily smooth.
	/// The {@code solve} method returns a zero {@code x} of the function {@code f}
	/// in the given interval {@code [a, b]} to within a tolerance
	/// {@code 6 eps abs(x) + t} where {@code eps} is the relative accuracy and
	/// {@code t} is the absolute accuracy.
	/// The given interval must bracket the root.
	/// 
	/// @version $Id: BrentSolver.java 1379560 2012-08-31 19:40:30Z erans $
	/// </summary>
	public class BrentSolver : AbstractUnivariateSolver
	{

		/// <summary>
		/// Default absolute accuracy. </summary>
		private const double DEFAULT_ABSOLUTE_ACCURACY = 1e-6;

		/// <summary>
		/// Construct a solver with default accuracy (1e-6).
		/// </summary>
		public BrentSolver() : this(DEFAULT_ABSOLUTE_ACCURACY)
		{
		}
		/// <summary>
		/// Construct a solver.
		/// </summary>
		/// <param name="absoluteAccuracy"> Absolute accuracy. </param>
		public BrentSolver(double absoluteAccuracy) : base(absoluteAccuracy)
		{
		}
		/// <summary>
		/// Construct a solver.
		/// </summary>
		/// <param name="relativeAccuracy"> Relative accuracy. </param>
		/// <param name="absoluteAccuracy"> Absolute accuracy. </param>
		public BrentSolver(double relativeAccuracy, double absoluteAccuracy) : base(relativeAccuracy, absoluteAccuracy)
		{
		}
		/// <summary>
		/// Construct a solver.
		/// </summary>
		/// <param name="relativeAccuracy"> Relative accuracy. </param>
		/// <param name="absoluteAccuracy"> Absolute accuracy. </param>
		/// <param name="functionValueAccuracy"> Function value accuracy. </param>
		public BrentSolver(double relativeAccuracy, double absoluteAccuracy, double functionValueAccuracy) : base(relativeAccuracy, absoluteAccuracy, functionValueAccuracy)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected double doSolve() throws mathlib.exception.NoBracketingException, mathlib.exception.TooManyEvaluationsException, mathlib.exception.NumberIsTooLargeException
		protected internal override double doSolve()
		{
			double min = Min;
			double max = Max;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double initial = getStartValue();
			double initial = StartValue;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double functionValueAccuracy = getFunctionValueAccuracy();
			double functionValueAccuracy = FunctionValueAccuracy;

			verifySequence(min, initial, max);

			// Return the initial guess if it is good enough.
			double yInitial = computeObjectiveValue(initial);
			if (FastMath.abs(yInitial) <= functionValueAccuracy)
			{
				return initial;
			}

			// Return the first endpoint if it is good enough.
			double yMin = computeObjectiveValue(min);
			if (FastMath.abs(yMin) <= functionValueAccuracy)
			{
				return min;
			}

			// Reduce interval if min and initial bracket the root.
			if (yInitial * yMin < 0)
			{
				return brent(min, initial, yMin, yInitial);
			}

			// Return the second endpoint if it is good enough.
			double yMax = computeObjectiveValue(max);
			if (FastMath.abs(yMax) <= functionValueAccuracy)
			{
				return max;
			}

			// Reduce interval if initial and max bracket the root.
			if (yInitial * yMax < 0)
			{
				return brent(initial, max, yInitial, yMax);
			}

			throw new NoBracketingException(min, max, yMin, yMax);
		}

		/// <summary>
		/// Search for a zero inside the provided interval.
		/// This implementation is based on the algorithm described at page 58 of
		/// the book
		/// <quote>
		///  <b>Algorithms for Minimization Without Derivatives</b>
		///  <it>Richard P. Brent</it>
		///  Dover 0-486-41998-3
		/// </quote>
		/// </summary>
		/// <param name="lo"> Lower bound of the search interval. </param>
		/// <param name="hi"> Higher bound of the search interval. </param>
		/// <param name="fLo"> Function value at the lower bound of the search interval. </param>
		/// <param name="fHi"> Function value at the higher bound of the search interval. </param>
		/// <returns> the value where the function is zero. </returns>
		private double brent(double lo, double hi, double fLo, double fHi)
		{
			double a = lo;
			double fa = fLo;
			double b = hi;
			double fb = fHi;
			double c = a;
			double fc = fa;
			double d = b - a;
			double e = d;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = getAbsoluteAccuracy();
			double t = AbsoluteAccuracy;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double eps = getRelativeAccuracy();
			double eps = RelativeAccuracy;

			while (true)
			{
				if (FastMath.abs(fc) < FastMath.abs(fb))
				{
					a = b;
					b = c;
					c = a;
					fa = fb;
					fb = fc;
					fc = fa;
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tol = 2 * eps * mathlib.util.FastMath.abs(b) + t;
				double tol = 2 * eps * FastMath.abs(b) + t;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double m = 0.5 * (c - b);
				double m = 0.5 * (c - b);

				if (FastMath.abs(m) <= tol || Precision.Equals(fb, 0))
				{
					return b;
				}
				if (FastMath.abs(e) < tol || FastMath.abs(fa) <= FastMath.abs(fb))
				{
					// Force bisection.
					d = m;
					e = d;
				}
				else
				{
					double s = fb / fa;
					double p;
					double q;
					// The equality test (a == c) is intentional,
					// it is part of the original Brent's method and
					// it should NOT be replaced by proximity test.
					if (a == c)
					{
						// Linear interpolation.
						p = 2 * m * s;
						q = 1 - s;
					}
					else
					{
						// Inverse quadratic interpolation.
						q = fa / fc;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double r = fb / fc;
						double r = fb / fc;
						p = s * (2 * m * q * (q - r) - (b - a) * (r - 1));
						q = (q - 1) * (r - 1) * (s - 1);
					}
					if (p > 0)
					{
						q = -q;
					}
					else
					{
						p = -p;
					}
					s = e;
					e = d;
					if (p >= 1.5 * m * q - FastMath.abs(tol * q) || p >= FastMath.abs(0.5 * s * q))
					{
						// Inverse quadratic interpolation gives a value
						// in the wrong direction, or progress is slow.
						// Fall back to bisection.
						d = m;
						e = d;
					}
					else
					{
						d = p / q;
					}
				}
				a = b;
				fa = fb;

				if (FastMath.abs(d) > tol)
				{
					b += d;
				}
				else if (m > 0)
				{
					b += tol;
				}
				else
				{
					b -= tol;
				}
				fb = computeObjectiveValue(b);
				if ((fb > 0 && fc > 0) || (fb <= 0 && fc <= 0))
				{
					c = a;
					fc = fa;
					d = b - a;
					e = d;
				}
			}
		}
	}

}
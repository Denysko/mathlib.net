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
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// This class implements the <a href="http://mathworld.wolfram.com/MullersMethod.html">
	/// Muller's Method</a> for root finding of real univariate functions. For
	/// reference, see <b>Elementary Numerical Analysis</b>, ISBN 0070124477,
	/// chapter 3.
	/// <p>
	/// Muller's method applies to both real and complex functions, but here we
	/// restrict ourselves to real functions.
	/// This class differs from <seealso cref="MullerSolver"/> in the way it avoids complex
	/// operations.</p>
	/// Except for the initial [min, max], it does not require bracketing
	/// condition, e.g. f(x0), f(x1), f(x2) can have the same sign. If complex
	/// number arises in the computation, we simply use its modulus as real
	/// approximation.</p>
	/// <p>
	/// Because the interval may not be bracketing, bisection alternative is
	/// not applicable here. However in practice our treatment usually works
	/// well, especially near real zeroes where the imaginary part of complex
	/// approximation is often negligible.</p>
	/// <p>
	/// The formulas here do not use divided differences directly.</p>
	/// 
	/// @version $Id: MullerSolver2.java 1379560 2012-08-31 19:40:30Z erans $
	/// @since 1.2 </summary>
	/// <seealso cref= MullerSolver </seealso>
	public class MullerSolver2 : AbstractUnivariateSolver
	{

		/// <summary>
		/// Default absolute accuracy. </summary>
		private const double DEFAULT_ABSOLUTE_ACCURACY = 1e-6;

		/// <summary>
		/// Construct a solver with default accuracy (1e-6).
		/// </summary>
		public MullerSolver2() : this(DEFAULT_ABSOLUTE_ACCURACY)
		{
		}
		/// <summary>
		/// Construct a solver.
		/// </summary>
		/// <param name="absoluteAccuracy"> Absolute accuracy. </param>
		public MullerSolver2(double absoluteAccuracy) : base(absoluteAccuracy)
		{
		}
		/// <summary>
		/// Construct a solver.
		/// </summary>
		/// <param name="relativeAccuracy"> Relative accuracy. </param>
		/// <param name="absoluteAccuracy"> Absolute accuracy. </param>
		public MullerSolver2(double relativeAccuracy, double absoluteAccuracy) : base(relativeAccuracy, absoluteAccuracy)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected double doSolve() throws mathlib.exception.TooManyEvaluationsException, mathlib.exception.NumberIsTooLargeException, mathlib.exception.NoBracketingException
		protected internal override double doSolve()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double min = getMin();
			double min = Min;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double max = getMax();
			double max = Max;

			verifyInterval(min, max);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double relativeAccuracy = getRelativeAccuracy();
			double relativeAccuracy = RelativeAccuracy;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double absoluteAccuracy = getAbsoluteAccuracy();
			double absoluteAccuracy = AbsoluteAccuracy;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double functionValueAccuracy = getFunctionValueAccuracy();
			double functionValueAccuracy = FunctionValueAccuracy;

			// x2 is the last root approximation
			// x is the new approximation and new x2 for next round
			// x0 < x1 < x2 does not hold here

			double x0 = min;
			double y0 = computeObjectiveValue(x0);
			if (FastMath.abs(y0) < functionValueAccuracy)
			{
				return x0;
			}
			double x1 = max;
			double y1 = computeObjectiveValue(x1);
			if (FastMath.abs(y1) < functionValueAccuracy)
			{
				return x1;
			}

			if (y0 * y1 > 0)
			{
				throw new NoBracketingException(x0, x1, y0, y1);
			}

			double x2 = 0.5 * (x0 + x1);
			double y2 = computeObjectiveValue(x2);

			double oldx = double.PositiveInfinity;
			while (true)
			{
				// quadratic interpolation through x0, x1, x2
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double q = (x2 - x1) / (x1 - x0);
				double q = (x2 - x1) / (x1 - x0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a = q * (y2 - (1 + q) * y1 + q * y0);
				double a = q * (y2 - (1 + q) * y1 + q * y0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b = (2 * q + 1) * y2 - (1 + q) * (1 + q) * y1 + q * q * y0;
				double b = (2 * q + 1) * y2 - (1 + q) * (1 + q) * y1 + q * q * y0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c = (1 + q) * y2;
				double c = (1 + q) * y2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double delta = b * b - 4 * a * c;
				double delta = b * b - 4 * a * c;
				double x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double denominator;
				double denominator;
				if (delta >= 0.0)
				{
					// choose a denominator larger in magnitude
					double dplus = b + FastMath.sqrt(delta);
					double dminus = b - FastMath.sqrt(delta);
					denominator = FastMath.abs(dplus) > FastMath.abs(dminus) ? dplus : dminus;
				}
				else
				{
					// take the modulus of (B +/- FastMath.sqrt(delta))
					denominator = FastMath.sqrt(b * b - delta);
				}
				if (denominator != 0)
				{
					x = x2 - 2.0 * c * (x2 - x1) / denominator;
					// perturb x if it exactly coincides with x1 or x2
					// the equality tests here are intentional
					while (x == x1 || x == x2)
					{
						x += absoluteAccuracy;
					}
				}
				else
				{
					// extremely rare case, get a random number to skip it
					x = min + FastMath.random() * (max - min);
					oldx = double.PositiveInfinity;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = computeObjectiveValue(x);
				double y = computeObjectiveValue(x);

				// check for convergence
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tolerance = mathlib.util.FastMath.max(relativeAccuracy * mathlib.util.FastMath.abs(x), absoluteAccuracy);
				double tolerance = FastMath.max(relativeAccuracy * FastMath.abs(x), absoluteAccuracy);
				if (FastMath.abs(x - oldx) <= tolerance || FastMath.abs(y) <= functionValueAccuracy)
				{
					return x;
				}

				// prepare the next iteration
				x0 = x1;
				y0 = y1;
				x1 = x2;
				y1 = y2;
				x2 = x;
				y2 = y;
				oldx = x;
			}
		}
	}

}
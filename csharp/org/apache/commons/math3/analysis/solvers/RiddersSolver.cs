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
namespace org.apache.commons.math3.analysis.solvers
{

	using FastMath = org.apache.commons.math3.util.FastMath;
	using NoBracketingException = org.apache.commons.math3.exception.NoBracketingException;
	using TooManyEvaluationsException = org.apache.commons.math3.exception.TooManyEvaluationsException;

	/// <summary>
	/// Implements the <a href="http://mathworld.wolfram.com/RiddersMethod.html">
	/// Ridders' Method</a> for root finding of real univariate functions. For
	/// reference, see C. Ridders, <i>A new algorithm for computing a single root
	/// of a real continuous function </i>, IEEE Transactions on Circuits and
	/// Systems, 26 (1979), 979 - 980.
	/// <p>
	/// The function should be continuous but not necessarily smooth.</p>
	/// 
	/// @version $Id: RiddersSolver.java 1379560 2012-08-31 19:40:30Z erans $
	/// @since 1.2
	/// </summary>
	public class RiddersSolver : AbstractUnivariateSolver
	{
		/// <summary>
		/// Default absolute accuracy. </summary>
		private const double DEFAULT_ABSOLUTE_ACCURACY = 1e-6;

		/// <summary>
		/// Construct a solver with default accuracy (1e-6).
		/// </summary>
		public RiddersSolver() : this(DEFAULT_ABSOLUTE_ACCURACY)
		{
		}
		/// <summary>
		/// Construct a solver.
		/// </summary>
		/// <param name="absoluteAccuracy"> Absolute accuracy. </param>
		public RiddersSolver(double absoluteAccuracy) : base(absoluteAccuracy)
		{
		}
		/// <summary>
		/// Construct a solver.
		/// </summary>
		/// <param name="relativeAccuracy"> Relative accuracy. </param>
		/// <param name="absoluteAccuracy"> Absolute accuracy. </param>
		public RiddersSolver(double relativeAccuracy, double absoluteAccuracy) : base(relativeAccuracy, absoluteAccuracy)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected double doSolve() throws org.apache.commons.math3.exception.TooManyEvaluationsException, org.apache.commons.math3.exception.NoBracketingException
		protected internal override double doSolve()
		{
			double min = Min;
			double max = Max;
			// [x1, x2] is the bracketing interval in each iteration
			// x3 is the midpoint of [x1, x2]
			// x is the new root approximation and an endpoint of the new interval
			double x1 = min;
			double y1 = computeObjectiveValue(x1);
			double x2 = max;
			double y2 = computeObjectiveValue(x2);

			// check for zeros before verifying bracketing
			if (y1 == 0)
			{
				return min;
			}
			if (y2 == 0)
			{
				return max;
			}
			verifyBracketing(min, max);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double absoluteAccuracy = getAbsoluteAccuracy();
			double absoluteAccuracy = AbsoluteAccuracy;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double functionValueAccuracy = getFunctionValueAccuracy();
			double functionValueAccuracy = FunctionValueAccuracy;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double relativeAccuracy = getRelativeAccuracy();
			double relativeAccuracy = RelativeAccuracy;

			double oldx = double.PositiveInfinity;
			while (true)
			{
				// calculate the new root approximation
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x3 = 0.5 * (x1 + x2);
				double x3 = 0.5 * (x1 + x2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y3 = computeObjectiveValue(x3);
				double y3 = computeObjectiveValue(x3);
				if (FastMath.abs(y3) <= functionValueAccuracy)
				{
					return x3;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double delta = 1 - (y1 * y2) / (y3 * y3);
				double delta = 1 - (y1 * y2) / (y3 * y3); // delta > 1 due to bracketing
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double correction = (org.apache.commons.math3.util.FastMath.signum(y2) * org.apache.commons.math3.util.FastMath.signum(y3)) * (x3 - x1) / org.apache.commons.math3.util.FastMath.sqrt(delta);
				double correction = (FastMath.signum(y2) * FastMath.signum(y3)) * (x3 - x1) / FastMath.sqrt(delta);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = x3 - correction;
				double x = x3 - correction; // correction != 0
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = computeObjectiveValue(x);
				double y = computeObjectiveValue(x);

				// check for convergence
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tolerance = org.apache.commons.math3.util.FastMath.max(relativeAccuracy * org.apache.commons.math3.util.FastMath.abs(x), absoluteAccuracy);
				double tolerance = FastMath.max(relativeAccuracy * FastMath.abs(x), absoluteAccuracy);
				if (FastMath.abs(x - oldx) <= tolerance)
				{
					return x;
				}
				if (FastMath.abs(y) <= functionValueAccuracy)
				{
					return x;
				}

				// prepare the new interval for next iteration
				// Ridders' method guarantees x1 < x < x2
				if (correction > 0.0) // x1 < x < x3
				{
					if (FastMath.signum(y1) + FastMath.signum(y) == 0.0)
					{
						x2 = x;
						y2 = y;
					}
					else
					{
						x1 = x;
						x2 = x3;
						y1 = y;
						y2 = y3;
					}
				} // x3 < x < x2
				else
				{
					if (FastMath.signum(y2) + FastMath.signum(y) == 0.0)
					{
						x1 = x;
						y1 = y;
					}
					else
					{
						x1 = x3;
						x2 = x;
						y1 = y3;
						y2 = y;
					}
				}
				oldx = x;
			}
		}
	}

}
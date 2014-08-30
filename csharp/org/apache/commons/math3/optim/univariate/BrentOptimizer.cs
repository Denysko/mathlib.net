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
namespace org.apache.commons.math3.optim.univariate
{

	using Precision = org.apache.commons.math3.util.Precision;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using org.apache.commons.math3.optim;
	using GoalType = org.apache.commons.math3.optim.nonlinear.scalar.GoalType;

	/// <summary>
	/// For a function defined on some interval {@code (lo, hi)}, this class
	/// finds an approximation {@code x} to the point at which the function
	/// attains its minimum.
	/// It implements Richard Brent's algorithm (from his book "Algorithms for
	/// Minimization without Derivatives", p. 79) for finding minima of real
	/// univariate functions.
	/// <br/>
	/// This code is an adaptation, partly based on the Python code from SciPy
	/// (module "optimize.py" v0.5); the original algorithm is also modified
	/// <ul>
	///  <li>to use an initial guess provided by the user,</li>
	///  <li>to ensure that the best point encountered is the one returned.</li>
	/// </ul>
	/// 
	/// @version $Id: BrentOptimizer.java 1462503 2013-03-29 15:48:27Z luc $
	/// @since 2.0
	/// </summary>
	public class BrentOptimizer : UnivariateOptimizer
	{
		/// <summary>
		/// Golden section.
		/// </summary>
		private static readonly double GOLDEN_SECTION = 0.5 * (3 - FastMath.sqrt(5));
		/// <summary>
		/// Minimum relative tolerance.
		/// </summary>
		private static readonly double MIN_RELATIVE_TOLERANCE = 2 * FastMath.ulp(1d);
		/// <summary>
		/// Relative threshold.
		/// </summary>
		private readonly double relativeThreshold;
		/// <summary>
		/// Absolute threshold.
		/// </summary>
		private readonly double absoluteThreshold;

		/// <summary>
		/// The arguments are used implement the original stopping criterion
		/// of Brent's algorithm.
		/// {@code abs} and {@code rel} define a tolerance
		/// {@code tol = rel |x| + abs}. {@code rel} should be no smaller than
		/// <em>2 macheps</em> and preferably not much less than <em>sqrt(macheps)</em>,
		/// where <em>macheps</em> is the relative machine precision. {@code abs} must
		/// be positive.
		/// </summary>
		/// <param name="rel"> Relative threshold. </param>
		/// <param name="abs"> Absolute threshold. </param>
		/// <param name="checker"> Additional, user-defined, convergence checking
		/// procedure. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code abs <= 0}. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code rel < 2 * Math.ulp(1d)}. </exception>
		public BrentOptimizer(double rel, double abs, ConvergenceChecker<UnivariatePointValuePair> checker) : base(checker)
		{

			if (rel < MIN_RELATIVE_TOLERANCE)
			{
				throw new NumberIsTooSmallException(rel, MIN_RELATIVE_TOLERANCE, true);
			}
			if (abs <= 0)
			{
				throw new NotStrictlyPositiveException(abs);
			}

			relativeThreshold = rel;
			absoluteThreshold = abs;
		}

		/// <summary>
		/// The arguments are used for implementing the original stopping criterion
		/// of Brent's algorithm.
		/// {@code abs} and {@code rel} define a tolerance
		/// {@code tol = rel |x| + abs}. {@code rel} should be no smaller than
		/// <em>2 macheps</em> and preferably not much less than <em>sqrt(macheps)</em>,
		/// where <em>macheps</em> is the relative machine precision. {@code abs} must
		/// be positive.
		/// </summary>
		/// <param name="rel"> Relative threshold. </param>
		/// <param name="abs"> Absolute threshold. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code abs <= 0}. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code rel < 2 * Math.ulp(1d)}. </exception>
		public BrentOptimizer(double rel, double abs) : this(rel, abs, null)
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override UnivariatePointValuePair doOptimize()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isMinim = getGoalType() == org.apache.commons.math3.optim.nonlinear.scalar.GoalType.MINIMIZE;
			bool isMinim = GoalType == GoalType.MINIMIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lo = getMin();
			double lo = Min;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double mid = getStartValue();
			double mid = StartValue;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double hi = getMax();
			double hi = Max;

			// Optional additional convergence criteria.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.ConvergenceChecker<UnivariatePointValuePair> checker = getConvergenceChecker();
			ConvergenceChecker<UnivariatePointValuePair> checker = ConvergenceChecker;

			double a;
			double b;
			if (lo < hi)
			{
				a = lo;
				b = hi;
			}
			else
			{
				a = hi;
				b = lo;
			}

			double x = mid;
			double v = x;
			double w = x;
			double d = 0;
			double e = 0;
			double fx = computeObjectiveValue(x);
			if (!isMinim)
			{
				fx = -fx;
			}
			double fv = fx;
			double fw = fx;

			UnivariatePointValuePair previous = null;
			UnivariatePointValuePair current = new UnivariatePointValuePair(x, isMinim ? fx : -fx);
			// Best point encountered so far (which is the initial guess).
			UnivariatePointValuePair best = current;

			int iter = 0;
			while (true)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double m = 0.5 * (a + b);
				double m = 0.5 * (a + b);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tol1 = relativeThreshold * org.apache.commons.math3.util.FastMath.abs(x) + absoluteThreshold;
				double tol1 = relativeThreshold * FastMath.abs(x) + absoluteThreshold;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tol2 = 2 * tol1;
				double tol2 = 2 * tol1;

				// Default stopping criterion.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean stop = org.apache.commons.math3.util.FastMath.abs(x - m) <= tol2 - 0.5 * (b - a);
				bool stop = FastMath.abs(x - m) <= tol2 - 0.5 * (b - a);
				if (!stop)
				{
					double p = 0;
					double q = 0;
					double r = 0;
					double u = 0;

					if (FastMath.abs(e) > tol1) // Fit parabola.
					{
						r = (x - w) * (fx - fv);
						q = (x - v) * (fx - fw);
						p = (x - v) * q - (x - w) * r;
						q = 2 * (q - r);

						if (q > 0)
						{
							p = -p;
						}
						else
						{
							q = -q;
						}

						r = e;
						e = d;

						if (p > q * (a - x) && p < q * (b - x) && FastMath.abs(p) < FastMath.abs(0.5 * q * r))
						{
							// Parabolic interpolation step.
							d = p / q;
							u = x + d;

							// f must not be evaluated too close to a or b.
							if (u - a < tol2 || b - u < tol2)
							{
								if (x <= m)
								{
									d = tol1;
								}
								else
								{
									d = -tol1;
								}
							}
						}
						else
						{
							// Golden section step.
							if (x < m)
							{
								e = b - x;
							}
							else
							{
								e = a - x;
							}
							d = GOLDEN_SECTION * e;
						}
					}
					else
					{
						// Golden section step.
						if (x < m)
						{
							e = b - x;
						}
						else
						{
							e = a - x;
						}
						d = GOLDEN_SECTION * e;
					}

					// Update by at least "tol1".
					if (FastMath.abs(d) < tol1)
					{
						if (d >= 0)
						{
							u = x + tol1;
						}
						else
						{
							u = x - tol1;
						}
					}
					else
					{
						u = x + d;
					}

					double fu = computeObjectiveValue(u);
					if (!isMinim)
					{
						fu = -fu;
					}

					// User-defined convergence checker.
					previous = current;
					current = new UnivariatePointValuePair(u, isMinim ? fu : -fu);
					best = best(best, best(previous, current, isMinim), isMinim);

					if (checker != null && checker.converged(iter, previous, current))
					{
						return best;
					}

					// Update a, b, v, w and x.
					if (fu <= fx)
					{
						if (u < x)
						{
							b = x;
						}
						else
						{
							a = x;
						}
						v = w;
						fv = fw;
						w = x;
						fw = fx;
						x = u;
						fx = fu;
					}
					else
					{
						if (u < x)
						{
							a = u;
						}
						else
						{
							b = u;
						}
						if (fu <= fw || Precision.Equals(w, x))
						{
							v = w;
							fv = fw;
							w = u;
							fw = fu;
						}
						else if (fu <= fv || Precision.Equals(v, x) || Precision.Equals(v, w))
						{
							v = u;
							fv = fu;
						}
					}
				} // Default termination (Brent's criterion).
				else
				{
					return best(best, best(previous, current, isMinim), isMinim);
				}
				++iter;
			}
		}

		/// <summary>
		/// Selects the best of two points.
		/// </summary>
		/// <param name="a"> Point and value. </param>
		/// <param name="b"> Point and value. </param>
		/// <param name="isMinim"> {@code true} if the selected point must be the one with
		/// the lowest value. </param>
		/// <returns> the best point, or {@code null} if {@code a} and {@code b} are
		/// both {@code null}. When {@code a} and {@code b} have the same function
		/// value, {@code a} is returned. </returns>
		private UnivariatePointValuePair best(UnivariatePointValuePair a, UnivariatePointValuePair b, bool isMinim)
		{
			if (a == null)
			{
				return b;
			}
			if (b == null)
			{
				return a;
			}

			if (isMinim)
			{
				return a.Value <= b.Value ? a : b;
			}
			else
			{
				return a.Value >= b.Value ? a : b;
			}
		}
	}

}
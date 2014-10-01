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

	using FastMath = mathlib.util.FastMath;
	using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;

	/// <summary>
	/// Implements <a href="http://mathworld.wolfram.com/NewtonsMethod.html">
	/// Newton's Method</a> for finding zeros of real univariate functions.
	/// <p>
	/// The function should be continuous but not necessarily smooth.</p>
	/// </summary>
	/// @deprecated as of 3.1, replaced by <seealso cref="NewtonRaphsonSolver"/>
	/// @version $Id: NewtonSolver.java 1395937 2012-10-09 10:04:36Z luc $ 
	[Obsolete]//("as of 3.1, replaced by <seealso cref="NewtonRaphsonSolver"/>")]
	public class NewtonSolver : AbstractDifferentiableUnivariateSolver
	{
		/// <summary>
		/// Default absolute accuracy. </summary>
		private const double DEFAULT_ABSOLUTE_ACCURACY = 1e-6;

		/// <summary>
		/// Construct a solver.
		/// </summary>
		public NewtonSolver() : this(DEFAULT_ABSOLUTE_ACCURACY)
		{
		}
		/// <summary>
		/// Construct a solver.
		/// </summary>
		/// <param name="absoluteAccuracy"> Absolute accuracy. </param>
		public NewtonSolver(double absoluteAccuracy) : base(absoluteAccuracy)
		{
		}

		/// <summary>
		/// Find a zero near the midpoint of {@code min} and {@code max}.
		/// </summary>
		/// <param name="f"> Function to solve. </param>
		/// <param name="min"> Lower bound for the interval. </param>
		/// <param name="max"> Upper bound for the interval. </param>
		/// <param name="maxEval"> Maximum number of evaluations. </param>
		/// <returns> the value where the function is zero. </returns>
		/// <exception cref="mathlib.exception.TooManyEvaluationsException">
		/// if the maximum evaluation count is exceeded. </exception>
		/// <exception cref="mathlib.exception.NumberIsTooLargeException">
		/// if {@code min >= max}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double solve(int maxEval, final mathlib.analysis.DifferentiableUnivariateFunction f, final double min, final double max) throws mathlib.exception.TooManyEvaluationsException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double solve(int maxEval, DifferentiableUnivariateFunction f, double min, double max)
		{
			return base.solve(maxEval, f, UnivariateSolverUtils.midpoint(min, max));
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected double doSolve() throws mathlib.exception.TooManyEvaluationsException
		protected internal override double doSolve()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double startValue = getStartValue();
			double startValue = StartValue;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double absoluteAccuracy = getAbsoluteAccuracy();
			double absoluteAccuracy = AbsoluteAccuracy;

			double x0 = startValue;
			double x1;
			while (true)
			{
				x1 = x0 - (computeObjectiveValue(x0) / computeDerivativeObjectiveValue(x0));
				if (FastMath.abs(x1 - x0) <= absoluteAccuracy)
				{
					return x1;
				}

				x0 = x1;
			}
		}
	}

}
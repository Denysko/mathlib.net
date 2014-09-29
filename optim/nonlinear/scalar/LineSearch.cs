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
namespace mathlib.optim.nonlinear.scalar
{

	using UnivariateOptimizer = mathlib.optim.univariate.UnivariateOptimizer;
	using BrentOptimizer = mathlib.optim.univariate.BrentOptimizer;
	using BracketFinder = mathlib.optim.univariate.BracketFinder;
	using UnivariatePointValuePair = mathlib.optim.univariate.UnivariatePointValuePair;
	using SimpleUnivariateValueChecker = mathlib.optim.univariate.SimpleUnivariateValueChecker;
	using SearchInterval = mathlib.optim.univariate.SearchInterval;
	using UnivariateObjectiveFunction = mathlib.optim.univariate.UnivariateObjectiveFunction;
	using UnivariateFunction = mathlib.analysis.UnivariateFunction;

	/// <summary>
	/// Class for finding the minimum of the objective function along a given
	/// direction.
	/// 
	/// @since 3.3
	/// @version $Id: LineSearch.java 1573341 2014-03-02 19:38:47Z erans $
	/// </summary>
	public class LineSearch
	{
		/// <summary>
		/// Value that will pass the precondition check for <seealso cref="BrentOptimizer"/>
		/// but will not pass the convergence check, so that the custom checker
		/// will always decide when to stop the line search.
		/// </summary>
		private const double REL_TOL_UNUSED = 1e-15;
		/// <summary>
		/// Value that will pass the precondition check for <seealso cref="BrentOptimizer"/>
		/// but will not pass the convergence check, so that the custom checker
		/// will always decide when to stop the line search.
		/// </summary>
		private static readonly double ABS_TOL_UNUSED = double.MinValue;
		/// <summary>
		/// Optimizer used for line search.
		/// </summary>
		private readonly UnivariateOptimizer lineOptimizer;
		/// <summary>
		/// Automatic bracketing.
		/// </summary>
		private readonly BracketFinder bracket = new BracketFinder();
		/// <summary>
		/// Extent of the initial interval used to find an interval that
		/// brackets the optimum.
		/// </summary>
		private readonly double initialBracketingRange;
		/// <summary>
		/// Optimizer on behalf of which the line search must be performed.
		/// </summary>
		private readonly MultivariateOptimizer mainOptimizer;

		/// <summary>
		/// The {@code BrentOptimizer} default stopping criterion uses the
		/// tolerances to check the domain (point) values, not the function
		/// values.
		/// The {@code relativeTolerance} and {@code absoluteTolerance}
		/// arguments are thus passed to a {@link SimpleUnivariateValueChecker
		/// custom checker} that will use the function values.
		/// </summary>
		/// <param name="optimizer"> Optimizer on behalf of which the line search
		/// be performed.
		/// Its {@link MultivariateOptimizer#computeObjectiveValue(double[])
		/// computeObjectiveValue} method will be called by the
		/// <seealso cref="#search(double[],double[]) search"/> method. </param>
		/// <param name="relativeTolerance"> Search will stop when the function relative
		/// difference between successive iterations is below this value. </param>
		/// <param name="absoluteTolerance"> Search will stop when the function absolute
		/// difference between successive iterations is below this value. </param>
		/// <param name="initialBracketingRange"> Extent of the initial interval used to
		/// find an interval that brackets the optimum.
		/// If the optimized function varies a lot in the vicinity of the optimum,
		/// it may be necessary to provide a value lower than the distance between
		/// successive local minima. </param>
		public LineSearch(MultivariateOptimizer optimizer, double relativeTolerance, double absoluteTolerance, double initialBracketingRange)
		{
			mainOptimizer = optimizer;
			lineOptimizer = new BrentOptimizer(REL_TOL_UNUSED, ABS_TOL_UNUSED, new SimpleUnivariateValueChecker(relativeTolerance, absoluteTolerance));
			this.initialBracketingRange = initialBracketingRange;
		}

		/// <summary>
		/// Finds the number {@code alpha} that optimizes
		/// {@code f(startPoint + alpha * direction)}.
		/// </summary>
		/// <param name="startPoint"> Starting point. </param>
		/// <param name="direction"> Search direction. </param>
		/// <returns> the optimum. </returns>
		/// <exception cref="mathlib.exception.TooManyEvaluationsException">
		/// if the number of evaluations is exceeded. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.optim.univariate.UnivariatePointValuePair search(final double[] startPoint, final double[] direction)
		public virtual UnivariatePointValuePair search(double[] startPoint, double[] direction)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = startPoint.length;
			int n = startPoint.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.analysis.UnivariateFunction f = new mathlib.analysis.UnivariateFunction()
			UnivariateFunction f = new UnivariateFunctionAnonymousInnerClassHelper(this, startPoint, direction, n);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final GoalType goal = mainOptimizer.getGoalType();
			GoalType goal = mainOptimizer.GoalType;
			bracket.search(f, goal, 0, initialBracketingRange);
			// Passing "MAX_VALUE" as a dummy value because it is the enclosing
			// class that counts the number of evaluations (and will eventually
			// generate the exception).
			return lineOptimizer.optimize(new MaxEval(int.MaxValue), new UnivariateObjectiveFunction(f), goal, new SearchInterval(bracket.Lo, bracket.Hi, bracket.Mid));
		}

		private class UnivariateFunctionAnonymousInnerClassHelper : UnivariateFunction
		{
			private readonly LineSearch outerInstance;

			private double[] startPoint;
			private double[] direction;
			private int n;

			public UnivariateFunctionAnonymousInnerClassHelper(LineSearch outerInstance, double[] startPoint, double[] direction, int n)
			{
				this.outerInstance = outerInstance;
				this.startPoint = startPoint;
				this.direction = direction;
				this.n = n;
			}

			public virtual double value(double alpha)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] x = new double[n];
				double[] x = new double[n];
				for (int i = 0; i < n; i++)
				{
					x[i] = startPoint[i] + alpha * direction[i];
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double obj = mainOptimizer.computeObjectiveValue(x);
				double obj = outerInstance.mainOptimizer.computeObjectiveValue(x);
				return obj;
			}
		}
	}

}
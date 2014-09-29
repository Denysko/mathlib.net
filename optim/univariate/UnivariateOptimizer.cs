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
namespace mathlib.optim.univariate
{

	using UnivariateFunction = mathlib.analysis.UnivariateFunction;
	using mathlib.optim;
	using GoalType = mathlib.optim.nonlinear.scalar.GoalType;
	using mathlib.optim;
	using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;

	/// <summary>
	/// Base class for a univariate scalar function optimizer.
	/// 
	/// @version $Id: UnivariateOptimizer.java 1503290 2013-07-15 15:16:29Z sebb $
	/// @since 3.1
	/// </summary>
	public abstract class UnivariateOptimizer : BaseOptimizer<UnivariatePointValuePair>
	{
		/// <summary>
		/// Objective function. </summary>
		private UnivariateFunction function;
		/// <summary>
		/// Type of optimization. </summary>
		private GoalType goal;
		/// <summary>
		/// Initial guess. </summary>
		private double start;
		/// <summary>
		/// Lower bound. </summary>
		private double min;
		/// <summary>
		/// Upper bound. </summary>
		private double max;

		/// <param name="checker"> Convergence checker. </param>
		protected internal UnivariateOptimizer(ConvergenceChecker<UnivariatePointValuePair> checker) : base(checker)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <param name="optData"> Optimization data. In addition to those documented in
		/// {@link BaseOptimizer#parseOptimizationData(OptimizationData[])
		/// BaseOptimizer}, this method will register the following data:
		/// <ul>
		///  <li><seealso cref="GoalType"/></li>
		///  <li><seealso cref="SearchInterval"/></li>
		///  <li><seealso cref="UnivariateObjectiveFunction"/></li>
		/// </ul> </param>
		/// <returns> {@inheritDoc} </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of
		/// evaluations is exceeded. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public UnivariatePointValuePair optimize(mathlib.optim.OptimizationData... optData) throws mathlib.exception.TooManyEvaluationsException
		public override UnivariatePointValuePair optimize(params OptimizationData[] optData)
		{
			// Perform computation.
			return base.optimize(optData);
		}

		/// <returns> the optimization type. </returns>
		public virtual GoalType GoalType
		{
			get
			{
				return goal;
			}
		}

		/// <summary>
		/// Scans the list of (required and optional) optimization data that
		/// characterize the problem.
		/// </summary>
		/// <param name="optData"> Optimization data.
		/// The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="GoalType"/></li>
		///  <li><seealso cref="SearchInterval"/></li>
		///  <li><seealso cref="UnivariateObjectiveFunction"/></li>
		/// </ul> </param>
		protected internal override void parseOptimizationData(params OptimizationData[] optData)
		{
			// Allow base class to register its own data.
			base.parseOptimizationData(optData);

			// The existing values (as set by the previous call) are reused if
			// not provided in the argument list.
			foreach (OptimizationData data in optData)
			{
				if (data is SearchInterval)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SearchInterval interval = (SearchInterval) data;
					SearchInterval interval = (SearchInterval) data;
					min = interval.Min;
					max = interval.Max;
					start = interval.StartValue;
					continue;
				}
				if (data is UnivariateObjectiveFunction)
				{
					function = ((UnivariateObjectiveFunction) data).ObjectiveFunction;
					continue;
				}
				if (data is GoalType)
				{
					goal = (GoalType) data;
					continue;
				}
			}
		}

		/// <returns> the initial guess. </returns>
		public virtual double StartValue
		{
			get
			{
				return start;
			}
		}
		/// <returns> the lower bounds. </returns>
		public virtual double Min
		{
			get
			{
				return min;
			}
		}
		/// <returns> the upper bounds. </returns>
		public virtual double Max
		{
			get
			{
				return max;
			}
		}

		/// <summary>
		/// Computes the objective function value.
		/// This method <em>must</em> be called by subclasses to enforce the
		/// evaluation counter limit.
		/// </summary>
		/// <param name="x"> Point at which the objective function must be evaluated. </param>
		/// <returns> the objective function value at the specified point. </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of
		/// evaluations is exceeded. </exception>
		protected internal virtual double computeObjectiveValue(double x)
		{
			base.incrementEvaluationCount();
			return function.value(x);
		}
	}

}
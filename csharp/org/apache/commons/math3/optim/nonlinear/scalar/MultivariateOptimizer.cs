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
namespace org.apache.commons.math3.optim.nonlinear.scalar
{

	using MultivariateFunction = org.apache.commons.math3.analysis.MultivariateFunction;
	using org.apache.commons.math3.optim;
	using org.apache.commons.math3.optim;
	using TooManyEvaluationsException = org.apache.commons.math3.exception.TooManyEvaluationsException;

	/// <summary>
	/// Base class for a multivariate scalar function optimizer.
	/// 
	/// @version $Id: MultivariateOptimizer.java 1572988 2014-02-28 16:23:26Z erans $
	/// @since 3.1
	/// </summary>
	public abstract class MultivariateOptimizer : BaseMultivariateOptimizer<PointValuePair>
	{
		/// <summary>
		/// Objective function. </summary>
		private MultivariateFunction function;
		/// <summary>
		/// Type of optimization. </summary>
		private GoalType goal;

		/// <param name="checker"> Convergence checker. </param>
		protected internal MultivariateOptimizer(ConvergenceChecker<PointValuePair> checker) : base(checker)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <param name="optData"> Optimization data. In addition to those documented in
		/// {@link BaseMultivariateOptimizer#parseOptimizationData(OptimizationData[])
		/// BaseMultivariateOptimizer}, this method will register the following data:
		/// <ul>
		///  <li><seealso cref="ObjectiveFunction"/></li>
		///  <li><seealso cref="GoalType"/></li>
		/// </ul> </param>
		/// <returns> {@inheritDoc} </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of
		/// evaluations is exceeded. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.apache.commons.math3.optim.PointValuePair optimize(org.apache.commons.math3.optim.OptimizationData... optData) throws org.apache.commons.math3.exception.TooManyEvaluationsException
		public override PointValuePair optimize(params OptimizationData[] optData)
		{
			// Set up base class and perform computation.
			return base.optimize(optData);
		}

		/// <summary>
		/// Scans the list of (required and optional) optimization data that
		/// characterize the problem.
		/// </summary>
		/// <param name="optData"> Optimization data.
		/// The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="ObjectiveFunction"/></li>
		///  <li><seealso cref="GoalType"/></li>
		/// </ul> </param>
		protected internal override void parseOptimizationData(params OptimizationData[] optData)
		{
			// Allow base class to register its own data.
			base.parseOptimizationData(optData);

			// The existing values (as set by the previous call) are reused if
			// not provided in the argument list.
			foreach (OptimizationData data in optData)
			{
				if (data is GoalType)
				{
					goal = (GoalType) data;
					continue;
				}
				if (data is ObjectiveFunction)
				{
					function = ((ObjectiveFunction) data).ObjectiveFunction;
					continue;
				}
			}
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
		/// Computes the objective function value.
		/// This method <em>must</em> be called by subclasses to enforce the
		/// evaluation counter limit.
		/// </summary>
		/// <param name="params"> Point at which the objective function must be evaluated. </param>
		/// <returns> the objective function value at the specified point. </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of
		/// evaluations is exceeded. </exception>
		public virtual double computeObjectiveValue(double[] @params)
		{
			base.incrementEvaluationCount();
			return function.value(@params);
		}
	}

}
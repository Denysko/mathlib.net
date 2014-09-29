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

	using MultivariateVectorFunction = mathlib.analysis.MultivariateVectorFunction;
	using mathlib.optim;
	using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;

	/// <summary>
	/// Base class for implementing optimizers for multivariate scalar
	/// differentiable functions.
	/// It contains boiler-plate code for dealing with gradient evaluation.
	/// 
	/// @version $Id: GradientMultivariateOptimizer.java 1443444 2013-02-07 12:41:36Z erans $
	/// @since 3.1
	/// </summary>
	public abstract class GradientMultivariateOptimizer : MultivariateOptimizer
	{
		/// <summary>
		/// Gradient of the objective function.
		/// </summary>
		private MultivariateVectorFunction gradient;

		/// <param name="checker"> Convergence checker. </param>
		protected internal GradientMultivariateOptimizer(ConvergenceChecker<PointValuePair> checker) : base(checker)
		{
		}

		/// <summary>
		/// Compute the gradient vector.
		/// </summary>
		/// <param name="params"> Point at which the gradient must be evaluated. </param>
		/// <returns> the gradient at the specified point. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected double[] computeObjectiveGradient(final double[] params)
		protected internal virtual double[] computeObjectiveGradient(double[] @params)
		{
			return gradient.value(@params);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <param name="optData"> Optimization data. In addition to those documented in
		/// {@link MultivariateOptimizer#parseOptimizationData(OptimizationData[])
		/// MultivariateOptimizer}, this method will register the following data:
		/// <ul>
		///  <li><seealso cref="ObjectiveFunctionGradient"/></li>
		/// </ul> </param>
		/// <returns> {@inheritDoc} </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of
		/// evaluations (of the objective function) is exceeded. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public mathlib.optim.PointValuePair optimize(mathlib.optim.OptimizationData... optData) throws mathlib.exception.TooManyEvaluationsException
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
		///  <li><seealso cref="ObjectiveFunctionGradient"/></li>
		/// </ul> </param>
		protected internal override void parseOptimizationData(params OptimizationData[] optData)
		{
			// Allow base class to register its own data.
			base.parseOptimizationData(optData);

			// The existing values (as set by the previous call) are reused if
			// not provided in the argument list.
			foreach (OptimizationData data in optData)
			{
				if (data is ObjectiveFunctionGradient)
				{
					gradient = ((ObjectiveFunctionGradient) data).ObjectiveFunctionGradient;
					// If more data must be parsed, this statement _must_ be
					// changed to "continue".
					break;
				}
			}
		}
	}

}
using System.Collections.Generic;

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
namespace mathlib.optim.linear
{

	using TooManyIterationsException = mathlib.exception.TooManyIterationsException;
	using MultivariateOptimizer = mathlib.optim.nonlinear.scalar.MultivariateOptimizer;

	/// <summary>
	/// Base class for implementing linear optimizers.
	/// 
	/// @version $Id: LinearOptimizer.java 1443444 2013-02-07 12:41:36Z erans $
	/// @since 3.1
	/// </summary>
	public abstract class LinearOptimizer : MultivariateOptimizer
	{
		/// <summary>
		/// Linear objective function.
		/// </summary>
		private LinearObjectiveFunction function;
		/// <summary>
		/// Linear constraints.
		/// </summary>
		private ICollection<LinearConstraint> linearConstraints;
		/// <summary>
		/// Whether to restrict the variables to non-negative values.
		/// </summary>
		private bool nonNegative;

		/// <summary>
		/// Simple constructor with default settings.
		/// 
		/// </summary>
		protected internal LinearOptimizer() : base(null); / / No convergence checker.
		{
		}

		/// <returns> {@code true} if the variables are restricted to non-negative values. </returns>
		protected internal virtual bool RestrictedToNonNegative
		{
			get
			{
				return nonNegative;
			}
		}

		/// <returns> the optimization type. </returns>
		protected internal virtual LinearObjectiveFunction Function
		{
			get
			{
				return function;
			}
		}

		/// <returns> the optimization type. </returns>
		protected internal virtual ICollection<LinearConstraint> Constraints
		{
			get
			{
				return Collections.unmodifiableCollection(linearConstraints);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <param name="optData"> Optimization data. In addition to those documented in
		/// {@link MultivariateOptimizer#parseOptimizationData(OptimizationData[])
		/// MultivariateOptimizer}, this method will register the following data:
		/// <ul>
		///  <li><seealso cref="LinearObjectiveFunction"/></li>
		///  <li><seealso cref="LinearConstraintSet"/></li>
		///  <li><seealso cref="NonNegativeConstraint"/></li>
		/// </ul> </param>
		/// <returns> {@inheritDoc} </returns>
		/// <exception cref="TooManyIterationsException"> if the maximal number of
		/// iterations is exceeded. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public mathlib.optim.PointValuePair optimize(mathlib.optim.OptimizationData... optData) throws mathlib.exception.TooManyIterationsException
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
		///  <li><seealso cref="LinearObjectiveFunction"/></li>
		///  <li><seealso cref="LinearConstraintSet"/></li>
		///  <li><seealso cref="NonNegativeConstraint"/></li>
		/// </ul> </param>
		protected internal override void parseOptimizationData(params OptimizationData[] optData)
		{
			// Allow base class to register its own data.
			base.parseOptimizationData(optData);

			// The existing values (as set by the previous call) are reused if
			// not provided in the argument list.
			foreach (OptimizationData data in optData)
			{
				if (data is LinearObjectiveFunction)
				{
					function = (LinearObjectiveFunction) data;
					continue;
				}
				if (data is LinearConstraintSet)
				{
					linearConstraints = ((LinearConstraintSet) data).Constraints;
					continue;
				}
				if (data is NonNegativeConstraint)
				{
					nonNegative = ((NonNegativeConstraint) data).RestrictedToNonNegative;
					continue;
				}
			}
		}
	}

}
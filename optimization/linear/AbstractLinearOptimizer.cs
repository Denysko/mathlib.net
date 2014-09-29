using System;
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

namespace mathlib.optimization.linear
{


	using MathIllegalStateException = mathlib.exception.MathIllegalStateException;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;

	/// <summary>
	/// Base class for implementing linear optimizers.
	/// <p>
	/// This base class handles the boilerplate methods associated to thresholds
	/// settings and iterations counters.
	/// 
	/// @version $Id: AbstractLinearOptimizer.java 1422230 2012-12-15 12:11:13Z erans $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 2.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public abstract class AbstractLinearOptimizer : LinearOptimizer
	{

		/// <summary>
		/// Default maximal number of iterations allowed. </summary>
		public const int DEFAULT_MAX_ITERATIONS = 100;

		/// <summary>
		/// Linear objective function.
		/// @since 2.1
		/// </summary>
		private LinearObjectiveFunction function;

		/// <summary>
		/// Linear constraints.
		/// @since 2.1
		/// </summary>
		private ICollection<LinearConstraint> linearConstraints;

		/// <summary>
		/// Type of optimization goal: either <seealso cref="GoalType#MAXIMIZE"/> or <seealso cref="GoalType#MINIMIZE"/>.
		/// @since 2.1
		/// </summary>
		private GoalType goal;

		/// <summary>
		/// Whether to restrict the variables to non-negative values.
		/// @since 2.1
		/// </summary>
		private bool nonNegative;

		/// <summary>
		/// Maximal number of iterations allowed. </summary>
		private int maxIterations;

		/// <summary>
		/// Number of iterations already performed. </summary>
		private int iterations;

		/// <summary>
		/// Simple constructor with default settings.
		/// <p>The maximal number of evaluation is set to its default value.</p>
		/// </summary>
		protected internal AbstractLinearOptimizer()
		{
			MaxIterations = DEFAULT_MAX_ITERATIONS;
		}

		/// <returns> {@code true} if the variables are restricted to non-negative values. </returns>
		protected internal virtual bool restrictToNonNegative()
		{
			return nonNegative;
		}

		/// <returns> the optimization type. </returns>
		protected internal virtual GoalType GoalType
		{
			get
			{
				return goal;
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
		/// {@inheritDoc} </summary>
		public virtual int MaxIterations
		{
			set
			{
				this.maxIterations = value;
			}
			get
			{
				return maxIterations;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int Iterations
		{
			get
			{
				return iterations;
			}
		}

		/// <summary>
		/// Increment the iterations counter by 1. </summary>
		/// <exception cref="MaxCountExceededException"> if the maximal number of iterations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void incrementIterationsCounter() throws mathlib.exception.MaxCountExceededException
		protected internal virtual void incrementIterationsCounter()
		{
			if (++iterations > maxIterations)
			{
				throw new MaxCountExceededException(maxIterations);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public mathlib.optimization.PointValuePair optimize(final LinearObjectiveFunction f, final java.util.Collection<LinearConstraint> constraints, final mathlib.optimization.GoalType goalType, final boolean restrictToNonNegative) throws mathlib.exception.MathIllegalStateException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual PointValuePair optimize(LinearObjectiveFunction f, ICollection<LinearConstraint> constraints, GoalType goalType, bool restrictToNonNegative)
		{

			// store linear problem characteristics
			this.function = f;
			this.linearConstraints = constraints;
			this.goal = goalType;
			this.nonNegative = restrictToNonNegative;

			iterations = 0;

			// solve the problem
			return doOptimize();

		}

		/// <summary>
		/// Perform the bulk of optimization algorithm. </summary>
		/// <returns> the point/value pair giving the optimal value for objective function </returns>
		/// <exception cref="MathIllegalStateException"> if no solution fulfilling the constraints
		/// can be found in the allowed number of iterations </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract mathlib.optimization.PointValuePair doOptimize() throws mathlib.exception.MathIllegalStateException;
		protected internal abstract PointValuePair doOptimize();

	}

}
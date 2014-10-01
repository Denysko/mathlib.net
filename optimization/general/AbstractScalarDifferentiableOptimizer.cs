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

namespace mathlib.optimization.general
{

	using DifferentiableMultivariateFunction = mathlib.analysis.DifferentiableMultivariateFunction;
	using MultivariateVectorFunction = mathlib.analysis.MultivariateVectorFunction;
	using FunctionUtils = mathlib.analysis.FunctionUtils;
	using MultivariateDifferentiableFunction = mathlib.analysis.differentiation.MultivariateDifferentiableFunction;
	using mathlib.optimization;
	using mathlib.optimization.direct;

	/// <summary>
	/// Base class for implementing optimizers for multivariate scalar
	/// differentiable functions.
	/// It contains boiler-plate code for dealing with gradient evaluation.
	/// 
	/// @version $Id: AbstractScalarDifferentiableOptimizer.java 1422230 2012-12-15 12:11:13Z erans $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 2.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public abstract class AbstractScalarDifferentiableOptimizer : BaseAbstractMultivariateOptimizer<DifferentiableMultivariateFunction>, DifferentiableMultivariateOptimizer
	{
		public abstract PointValuePair optimize(int maxEval, FUNC f, GoalType goalType, double[] startPoint);
		/// <summary>
		/// Objective function gradient.
		/// </summary>
		private MultivariateVectorFunction gradient;

		/// <summary>
		/// Simple constructor with default settings.
		/// The convergence check is set to a
		/// {@link mathlib.optimization.SimpleValueChecker
		/// SimpleValueChecker}. </summary>
		/// @deprecated See <seealso cref="mathlib.optimization.SimpleValueChecker#SimpleValueChecker()"/> 
		[Obsolete]//("See <seealso cref="mathlib.optimization.SimpleValueChecker#SimpleValueChecker()"/>")]
		protected internal AbstractScalarDifferentiableOptimizer()
		{
		}

		/// <param name="checker"> Convergence checker. </param>
		protected internal AbstractScalarDifferentiableOptimizer(ConvergenceChecker<PointValuePair> checker) : base(checker)
		{
		}

		/// <summary>
		/// Compute the gradient vector.
		/// </summary>
		/// <param name="evaluationPoint"> Point at which the gradient must be evaluated. </param>
		/// <returns> the gradient at the specified point. </returns>
		/// <exception cref="mathlib.exception.TooManyEvaluationsException">
		/// if the allowed number of evaluations is exceeded. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected double[] computeObjectiveGradient(final double[] evaluationPoint)
		protected internal virtual double[] computeObjectiveGradient(double[] evaluationPoint)
		{
			return gradient.value(evaluationPoint);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected mathlib.optimization.PointValuePair optimizeInternal(int maxEval, final mathlib.analysis.DifferentiableMultivariateFunction f, final mathlib.optimization.GoalType goalType, final double[] startPoint)
		protected internal override PointValuePair optimizeInternal(int maxEval, DifferentiableMultivariateFunction f, GoalType goalType, double[] startPoint)
		{
			// Store optimization problem characteristics.
			gradient = f.gradient();

			return base.optimizeInternal(maxEval, f, goalType, startPoint);
		}

		/// <summary>
		/// Optimize an objective function.
		/// </summary>
		/// <param name="f"> Objective function. </param>
		/// <param name="goalType"> Type of optimization goal: either
		/// <seealso cref="GoalType#MAXIMIZE"/> or <seealso cref="GoalType#MINIMIZE"/>. </param>
		/// <param name="startPoint"> Start point for optimization. </param>
		/// <param name="maxEval"> Maximum number of function evaluations. </param>
		/// <returns> the point/value pair giving the optimal value for objective
		/// function. </returns>
		/// <exception cref="mathlib.exception.DimensionMismatchException">
		/// if the start point dimension is wrong. </exception>
		/// <exception cref="mathlib.exception.TooManyEvaluationsException">
		/// if the maximal number of evaluations is exceeded. </exception>
		/// <exception cref="mathlib.exception.NullArgumentException"> if
		/// any argument is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.optimization.PointValuePair optimize(final int maxEval, final mathlib.analysis.differentiation.MultivariateDifferentiableFunction f, final mathlib.optimization.GoalType goalType, final double[] startPoint)
		public virtual PointValuePair optimize(int maxEval, MultivariateDifferentiableFunction f, GoalType goalType, double[] startPoint)
		{
			return optimizeInternal(maxEval, FunctionUtils.toDifferentiableMultivariateFunction(f), goalType, startPoint);
		}
	}

}
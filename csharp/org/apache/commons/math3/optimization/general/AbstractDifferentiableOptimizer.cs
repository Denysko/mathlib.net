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

namespace org.apache.commons.math3.optimization.general
{

	using MultivariateVectorFunction = org.apache.commons.math3.analysis.MultivariateVectorFunction;
	using GradientFunction = org.apache.commons.math3.analysis.differentiation.GradientFunction;
	using MultivariateDifferentiableFunction = org.apache.commons.math3.analysis.differentiation.MultivariateDifferentiableFunction;
	using org.apache.commons.math3.optimization;
	using org.apache.commons.math3.optimization.direct;

	/// <summary>
	/// Base class for implementing optimizers for multivariate scalar
	/// differentiable functions.
	/// It contains boiler-plate code for dealing with gradient evaluation.
	/// 
	/// @version $Id: AbstractDifferentiableOptimizer.java 1422230 2012-12-15 12:11:13Z erans $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.1 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public abstract class AbstractDifferentiableOptimizer : BaseAbstractMultivariateOptimizer<MultivariateDifferentiableFunction>
	{
		/// <summary>
		/// Objective function gradient.
		/// </summary>
		private MultivariateVectorFunction gradient;

		/// <param name="checker"> Convergence checker. </param>
		protected internal AbstractDifferentiableOptimizer(ConvergenceChecker<PointValuePair> checker) : base(checker)
		{
		}

		/// <summary>
		/// Compute the gradient vector.
		/// </summary>
		/// <param name="evaluationPoint"> Point at which the gradient must be evaluated. </param>
		/// <returns> the gradient at the specified point. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected double[] computeObjectiveGradient(final double[] evaluationPoint)
		protected internal virtual double[] computeObjectiveGradient(double[] evaluationPoint)
		{
			return gradient.value(evaluationPoint);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// @deprecated In 3.1. Please use
		/// <seealso cref="#optimizeInternal(int,MultivariateDifferentiableFunction,GoalType,OptimizationData[])"/>
		/// instead. 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override@Deprecated protected org.apache.commons.math3.optimization.PointValuePair optimizeInternal(final int maxEval, final org.apache.commons.math3.analysis.differentiation.MultivariateDifferentiableFunction f, final org.apache.commons.math3.optimization.GoalType goalType, final double[] startPoint)
		Deprecated protected internal override PointValuePair optimizeInternal(int maxEval, MultivariateDifferentiableFunction f, GoalType goalType, double[] startPoint)
		{
			return optimizeInternal(maxEval, f, goalType, new InitialGuess(startPoint));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected org.apache.commons.math3.optimization.PointValuePair optimizeInternal(final int maxEval, final org.apache.commons.math3.analysis.differentiation.MultivariateDifferentiableFunction f, final org.apache.commons.math3.optimization.GoalType goalType, final org.apache.commons.math3.optimization.OptimizationData... optData)
		protected internal override PointValuePair optimizeInternal(int maxEval, MultivariateDifferentiableFunction f, GoalType goalType, params OptimizationData[] optData)
		{
			// Store optimization problem characteristics.
			gradient = new GradientFunction(f);

			// Perform optimization.
			return base.optimizeInternal(maxEval, f, goalType, optData);
		}
	}

}
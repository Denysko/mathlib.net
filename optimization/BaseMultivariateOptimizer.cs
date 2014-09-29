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

namespace mathlib.optimization
{

	using MultivariateFunction = mathlib.analysis.MultivariateFunction;

	/// <summary>
	/// This interface is mainly intended to enforce the internal coherence of
	/// Commons-FastMath. Users of the API are advised to base their code on
	/// the following interfaces:
	/// <ul>
	///  <li><seealso cref="mathlib.optimization.MultivariateOptimizer"/></li>
	///  <li><seealso cref="mathlib.optimization.MultivariateDifferentiableOptimizer"/></li>
	/// </ul>
	/// </summary>
	/// @param <FUNC> Type of the objective function to be optimized.
	/// 
	/// @version $Id: BaseMultivariateOptimizer.java 1422230 2012-12-15 12:11:13Z erans $ </param>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public interface BaseMultivariateOptimizer<FUNC> : BaseOptimizer<PointValuePair> where FUNC : mathlib.analysis.MultivariateFunction
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
		/// @deprecated As of 3.1. In 4.0, it will be replaced by the declaration
		/// corresponding to this <seealso cref="mathlib.optimization.direct.BaseAbstractMultivariateOptimizer#optimize(int,MultivariateFunction,GoalType,OptimizationData[]) method"/>. 
	{
		[Obsolete("As of 3.1. In 4.0, it will be replaced by the declaration")]
		PointValuePair optimize(int maxEval, FUNC f, GoalType goalType, double[] startPoint);
	}

}
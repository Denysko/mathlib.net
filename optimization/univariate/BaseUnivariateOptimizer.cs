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

namespace mathlib.optimization.univariate
{

	using UnivariateFunction = mathlib.analysis.UnivariateFunction;
	using mathlib.optimization;

	/// <summary>
	/// This interface is mainly intended to enforce the internal coherence of
	/// Commons-Math. Users of the API are advised to base their code on
	/// the following interfaces:
	/// <ul>
	///  <li><seealso cref="mathlib.optimization.univariate.UnivariateOptimizer"/></li>
	/// </ul>
	/// </summary>
	/// @param <FUNC> Type of the objective function to be optimized.
	/// 
	/// @version $Id: BaseUnivariateOptimizer.java 1422230 2012-12-15 12:11:13Z erans $ </param>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public interface BaseUnivariateOptimizer<FUNC> : BaseOptimizer<UnivariatePointValuePair> where FUNC : mathlib.analysis.UnivariateFunction
	{
		/// <summary>
		/// Find an optimum in the given interval.
		/// 
		/// An optimizer may require that the interval brackets a single optimum.
		/// </summary>
		/// <param name="f"> Function to optimize. </param>
		/// <param name="goalType"> Type of optimization goal: either
		/// <seealso cref="GoalType#MAXIMIZE"/> or <seealso cref="GoalType#MINIMIZE"/>. </param>
		/// <param name="min"> Lower bound for the interval. </param>
		/// <param name="max"> Upper bound for the interval. </param>
		/// <param name="maxEval"> Maximum number of function evaluations. </param>
		/// <returns> a (point, value) pair where the function is optimum. </returns>
		/// <exception cref="mathlib.exception.TooManyEvaluationsException">
		/// if the maximum evaluation count is exceeded. </exception>
		/// <exception cref="mathlib.exception.ConvergenceException">
		/// if the optimizer detects a convergence problem. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code min > max} or the endpoints
		/// do not satisfy the requirements specified by the optimizer. </exception>
		UnivariatePointValuePair optimize(int maxEval, FUNC f, GoalType goalType, double min, double max);

		/// <summary>
		/// Find an optimum in the given interval, start at startValue.
		/// An optimizer may require that the interval brackets a single optimum.
		/// </summary>
		/// <param name="f"> Function to optimize. </param>
		/// <param name="goalType"> Type of optimization goal: either
		/// <seealso cref="GoalType#MAXIMIZE"/> or <seealso cref="GoalType#MINIMIZE"/>. </param>
		/// <param name="min"> Lower bound for the interval. </param>
		/// <param name="max"> Upper bound for the interval. </param>
		/// <param name="startValue"> Start value to use. </param>
		/// <param name="maxEval"> Maximum number of function evaluations. </param>
		/// <returns> a (point, value) pair where the function is optimum. </returns>
		/// <exception cref="mathlib.exception.TooManyEvaluationsException">
		/// if the maximum evaluation count is exceeded. </exception>
		/// <exception cref="mathlib.exception.ConvergenceException"> if the
		/// optimizer detects a convergence problem. </exception>
		/// <exception cref="IllegalArgumentException"> if {@code min > max} or the endpoints
		/// do not satisfy the requirements specified by the optimizer. </exception>
		/// <exception cref="mathlib.exception.NullArgumentException"> if any
		/// argument is {@code null}. </exception>
		UnivariatePointValuePair optimize(int maxEval, FUNC f, GoalType goalType, double min, double max, double startValue);
	}

}
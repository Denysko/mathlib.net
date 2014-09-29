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

	using MultivariateDifferentiableFunction = mathlib.analysis.differentiation.MultivariateDifferentiableFunction;

	/// <summary>
	/// This interface represents an optimization algorithm for
	/// {@link MultivariateDifferentiableFunction scalar differentiable objective
	/// functions}.
	/// Optimization algorithms find the input point set that either {@link GoalType
	/// maximize or minimize} an objective function.
	/// </summary>
	/// <seealso cref= MultivariateOptimizer </seealso>
	/// <seealso cref= MultivariateDifferentiableVectorOptimizer
	/// 
	/// @version $Id: MultivariateDifferentiableOptimizer.java 1422230 2012-12-15 12:11:13Z erans $ </seealso>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.1 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public interface MultivariateDifferentiableOptimizer : BaseMultivariateOptimizer<MultivariateDifferentiableFunction>
	{
	}

}
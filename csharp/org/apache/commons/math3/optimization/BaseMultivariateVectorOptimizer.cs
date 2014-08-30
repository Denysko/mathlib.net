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

namespace org.apache.commons.math3.optimization
{

	using MultivariateVectorFunction = org.apache.commons.math3.analysis.MultivariateVectorFunction;

	/// <summary>
	/// This interface is mainly intended to enforce the internal coherence of
	/// Commons-Math. Users of the API are advised to base their code on
	/// the following interfaces:
	/// <ul>
	///  <li><seealso cref="org.apache.commons.math3.optimization.DifferentiableMultivariateVectorOptimizer"/></li>
	/// </ul>
	/// </summary>
	/// @param <FUNC> Type of the objective function to be optimized.
	/// 
	/// @version $Id: BaseMultivariateVectorOptimizer.java 1422230 2012-12-15 12:11:13Z erans $ </param>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public interface BaseMultivariateVectorOptimizer<FUNC> : BaseOptimizer<PointVectorValuePair> where FUNC : org.apache.commons.math3.analysis.MultivariateVectorFunction
		/// <summary>
		/// Optimize an objective function.
		/// Optimization is considered to be a weighted least-squares minimization.
		/// The cost function to be minimized is
		/// <code>&sum;weight<sub>i</sub>(objective<sub>i</sub> - target<sub>i</sub>)<sup>2</sup></code>
		/// </summary>
		/// <param name="f"> Objective function. </param>
		/// <param name="target"> Target value for the objective functions at optimum. </param>
		/// <param name="weight"> Weights for the least squares cost computation. </param>
		/// <param name="startPoint"> Start point for optimization. </param>
		/// <returns> the point/value pair giving the optimal value for objective
		/// function. </returns>
		/// <param name="maxEval"> Maximum number of function evaluations. </param>
		/// <exception cref="org.apache.commons.math3.exception.DimensionMismatchException">
		/// if the start point dimension is wrong. </exception>
		/// <exception cref="org.apache.commons.math3.exception.TooManyEvaluationsException">
		/// if the maximal number of evaluations is exceeded. </exception>
		/// <exception cref="org.apache.commons.math3.exception.NullArgumentException"> if
		/// any argument is {@code null}. </exception>
		/// @deprecated As of 3.1. In 4.0, this will be replaced by the declaration
		/// corresponding to this <seealso cref="org.apache.commons.math3.optimization.direct.BaseAbstractMultivariateVectorOptimizer#optimize(int,MultivariateVectorFunction,OptimizationData[]) method"/>. 
	{
		[Obsolete("As of 3.1. In 4.0, this will be replaced by the declaration")]
		PointVectorValuePair optimize(int maxEval, FUNC f, double[] target, double[] weight, double[] startPoint);
	}

}
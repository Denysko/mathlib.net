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

	/// <summary>
	/// This interface is mainly intended to enforce the internal coherence of
	/// Commons-Math. Users of the API are advised to base their code on
	/// the following interfaces:
	/// <ul>
	///  <li><seealso cref="mathlib.optimization.MultivariateOptimizer"/></li>
	///  <li><seealso cref="mathlib.optimization.MultivariateDifferentiableOptimizer"/></li>
	///  <li><seealso cref="mathlib.optimization.MultivariateDifferentiableVectorOptimizer"/></li>
	///  <li><seealso cref="mathlib.optimization.univariate.UnivariateOptimizer"/></li>
	/// </ul>
	/// </summary>
	/// @param <PAIR> Type of the point/objective pair.
	/// 
	/// @version $Id: BaseOptimizer.java 1422230 2012-12-15 12:11:13Z erans $ </param>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public interface BaseOptimizer<PAIR>
	{
		/// <summary>
		/// Get the maximal number of function evaluations.
		/// </summary>
		/// <returns> the maximal number of function evaluations. </returns>
		int MaxEvaluations {get;}

		/// <summary>
		/// Get the number of evaluations of the objective function.
		/// The number of evaluations corresponds to the last call to the
		/// {@code optimize} method. It is 0 if the method has not been
		/// called yet.
		/// </summary>
		/// <returns> the number of evaluations of the objective function. </returns>
		int Evaluations {get;}

		/// <summary>
		/// Get the convergence checker.
		/// </summary>
		/// <returns> the object used to check for convergence. </returns>
		ConvergenceChecker<PAIR> ConvergenceChecker {get;}
	}

}
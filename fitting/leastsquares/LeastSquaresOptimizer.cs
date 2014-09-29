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
namespace mathlib.fitting.leastsquares
{

	/// <summary>
	/// An algorithm that can be applied to a non-linear least squares problem.
	/// 
	/// @version $Id: LeastSquaresOptimizer.java 1569362 2014-02-18 14:33:49Z luc $
	/// @since 3.3
	/// </summary>
	public interface LeastSquaresOptimizer
	{

		/// <summary>
		/// Solve the non-linear least squares problem.
		/// 
		/// </summary>
		/// <param name="leastSquaresProblem"> the problem definition, including model function and
		///                            convergence criteria. </param>
		/// <returns> The optimum. </returns>
		LeastSquaresOptimizer_Optimum optimize(LeastSquaresProblem leastSquaresProblem);

		/// <summary>
		/// The optimum found by the optimizer. This object contains the point, its value, and
		/// some metadata.
		/// </summary>
		//TODO Solution?

	}

	public interface LeastSquaresOptimizer_Optimum : LeastSquaresProblem_Evaluation
	{

		/// <summary>
		/// Get the number of times the model was evaluated in order to produce this
		/// optimum.
		/// </summary>
		/// <returns> the number of model (objective) function evaluations </returns>
		int Evaluations {get;}

		/// <summary>
		/// Get the number of times the algorithm iterated in order to produce this
		/// optimum. In general least squares it is common to have one {@link
		/// #getEvaluations() evaluation} per iterations.
		/// </summary>
		/// <returns> the number of iterations </returns>
		int Iterations {get;}

	}

}
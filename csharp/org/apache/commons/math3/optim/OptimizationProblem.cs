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
namespace org.apache.commons.math3.optim
{

	using Incrementor = org.apache.commons.math3.util.Incrementor;

	/// <summary>
	/// Common settings for all optimization problems. Includes divergence and convergence
	/// criteria.
	/// </summary>
	/// @param <PAIR> The type of value the {@link #getConvergenceChecker() convergence
	///               checker} will operate on. It should include the value of the model
	///               function and point where it was evaluated.
	/// @version $Id: OptimizationProblem.java 1571015 2014-02-23 14:00:48Z luc $
	/// @since 3.3 </param>
	public interface OptimizationProblem<PAIR>
	{
		/// <summary>
		/// Get a independent Incrementor that counts up to the maximum number of evaluations
		/// and then throws an exception.
		/// </summary>
		/// <returns> a counter for the evaluations. </returns>
		Incrementor EvaluationCounter {get;}

		/// <summary>
		/// Get a independent Incrementor that counts up to the maximum number of iterations
		/// and then throws an exception.
		/// </summary>
		/// <returns> a counter for the evaluations. </returns>
		Incrementor IterationCounter {get;}

		/// <summary>
		/// Gets the convergence checker.
		/// </summary>
		/// <returns> the object used to check for convergence. </returns>
		ConvergenceChecker<PAIR> ConvergenceChecker {get;}
	}

}
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
namespace mathlib.optim.nonlinear.scalar
{

	using MultivariateFunction = mathlib.analysis.MultivariateFunction;

	/// <summary>
	/// Scalar function to be optimized.
	/// 
	/// @version $Id: ObjectiveFunction.java 1435539 2013-01-19 13:27:24Z tn $
	/// @since 3.1
	/// </summary>
	public class ObjectiveFunction : OptimizationData
	{
		/// <summary>
		/// Function to be optimized. </summary>
		private readonly MultivariateFunction function;

		/// <param name="f"> Function to be optimized. </param>
		public ObjectiveFunction(MultivariateFunction f)
		{
			function = f;
		}

		/// <summary>
		/// Gets the function to be optimized.
		/// </summary>
		/// <returns> the objective function. </returns>
		public virtual MultivariateFunction ObjectiveFunction
		{
			get
			{
				return function;
			}
		}
	}

}
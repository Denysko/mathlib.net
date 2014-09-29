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
namespace mathlib.optim.nonlinear.vector
{

	using MultivariateMatrixFunction = mathlib.analysis.MultivariateMatrixFunction;

	/// <summary>
	/// Jacobian of the model (vector) function to be optimized.
	/// 
	/// @version $Id: ModelFunctionJacobian.java 1515242 2013-08-18 23:27:29Z erans $
	/// @since 3.1 </summary>
	/// @deprecated All classes and interfaces in this package are deprecated.
	/// The optimizers that were provided here were moved to the
	/// <seealso cref="mathlib.fitting.leastsquares"/> package
	/// (cf. MATH-1008). 
	[Obsolete("All classes and interfaces in this package are deprecated.")]
	public class ModelFunctionJacobian : OptimizationData
	{
		/// <summary>
		/// Function to be optimized. </summary>
		private readonly MultivariateMatrixFunction jacobian;

		/// <param name="j"> Jacobian of the model function to be optimized. </param>
		public ModelFunctionJacobian(MultivariateMatrixFunction j)
		{
			jacobian = j;
		}

		/// <summary>
		/// Gets the Jacobian of the model function to be optimized.
		/// </summary>
		/// <returns> the model function Jacobian. </returns>
		public virtual MultivariateMatrixFunction ModelFunctionJacobian
		{
			get
			{
				return jacobian;
			}
		}
	}

}
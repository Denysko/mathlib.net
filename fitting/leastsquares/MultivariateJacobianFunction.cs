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

	using RealMatrix = mathlib.linear.RealMatrix;
	using RealVector = mathlib.linear.RealVector;
	using mathlib.util;

	/// <summary>
	/// A interface for functions that compute a vector of values and can compute their
	/// derivatives (Jacobian).
	/// 
	/// @version $Id: MultivariateJacobianFunction.java 1569362 2014-02-18 14:33:49Z luc $
	/// @since 3.3
	/// </summary>
	public interface MultivariateJacobianFunction
	{

		/// <summary>
		/// Compute the function value and its Jacobian.
		/// </summary>
		/// <param name="point"> the abscissae </param>
		/// <returns> the values and their Jacobian of this vector valued function. </returns>
		Pair<RealVector, RealMatrix> value(RealVector point);

	}

}
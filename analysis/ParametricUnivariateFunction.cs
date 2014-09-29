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

namespace mathlib.analysis
{

	/// <summary>
	/// An interface representing a real function that depends on one independent
	/// variable plus some extra parameters.
	/// 
	/// @since 3.0
	/// @version $Id: ParametricUnivariateFunction.java 1364387 2012-07-22 18:14:11Z tn $
	/// </summary>
	public interface ParametricUnivariateFunction
	{
		/// <summary>
		/// Compute the value of the function.
		/// </summary>
		/// <param name="x"> Point for which the function value should be computed. </param>
		/// <param name="parameters"> Function parameters. </param>
		/// <returns> the value. </returns>
		double value(double x, params double[] parameters);

		/// <summary>
		/// Compute the gradient of the function with respect to its parameters.
		/// </summary>
		/// <param name="x"> Point for which the function value should be computed. </param>
		/// <param name="parameters"> Function parameters. </param>
		/// <returns> the value. </returns>
		double[] gradient(double x, params double[] parameters);
	}

}
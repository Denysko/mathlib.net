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

namespace mathlib.analysis
{

	/// <summary>
	/// Extension of <seealso cref="MultivariateFunction"/> representing a differentiable
	/// multivariate real function.
	/// @version $Id: DifferentiableMultivariateFunction.java 1415149 2012-11-29 13:12:55Z erans $
	/// @since 2.0 </summary>
	/// @deprecated as of 3.1 replaced by <seealso cref="mathlib.analysis.differentiation.MultivariateDifferentiableFunction"/> 
	[Obsolete("as of 3.1 replaced by <seealso cref="mathlib.analysis.differentiation.MultivariateDifferentiableFunction"/>")]
	public interface DifferentiableMultivariateFunction : MultivariateFunction
	{

		/// <summary>
		/// Returns the partial derivative of the function with respect to a point coordinate.
		/// <p>
		/// The partial derivative is defined with respect to point coordinate
		/// x<sub>k</sub>. If the partial derivatives with respect to all coordinates are
		/// needed, it may be more efficient to use the <seealso cref="#gradient()"/> method which will
		/// compute them all at once.
		/// </p> </summary>
		/// <param name="k"> index of the coordinate with respect to which the partial
		/// derivative is computed </param>
		/// <returns> the partial derivative function with respect to k<sup>th</sup> point coordinate </returns>
		MultivariateFunction partialDerivative(int k);

		/// <summary>
		/// Returns the gradient function.
		/// <p>If only one partial derivative with respect to a specific coordinate is
		/// needed, it may be more efficient to use the <seealso cref="#partialDerivative(int)"/> method
		/// which will compute only the specified component.</p> </summary>
		/// <returns> the gradient function </returns>
		MultivariateVectorFunction gradient();

	}

}
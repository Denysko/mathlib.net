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

namespace org.apache.commons.math3.analysis.differentiation
{

	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;

	/// <summary>
	/// Extension of <seealso cref="MultivariateFunction"/> representing a
	/// multivariate differentiable real function.
	/// @version $Id: MultivariateDifferentiableFunction.java 1462496 2013-03-29 14:56:08Z psteitz $
	/// @since 3.1
	/// </summary>
	public interface MultivariateDifferentiableFunction : MultivariateFunction
	{

		/// <summary>
		/// Compute the value for the function at the given point.
		/// </summary>
		/// <param name="point"> Point at which the function must be evaluated. </param>
		/// <returns> the function value for the given point. </returns>
		/// <exception cref="MathIllegalArgumentException"> if {@code point} does not
		/// satisfy the function's constraints (wrong dimension, argument out of bound,
		/// or unsupported derivative order for example) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: DerivativeStructure value(DerivativeStructure[] point) throws org.apache.commons.math3.exception.MathIllegalArgumentException;
		DerivativeStructure value(DerivativeStructure[] point);

	}

}
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
namespace mathlib.analysis.differentiation
{

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;

	/// <summary>
	/// Extension of <seealso cref="UnivariateVectorFunction"/> representing a univariate differentiable vectorial function.
	/// 
	/// @version $Id: UnivariateDifferentiableVectorFunction.java 1462699 2013-03-30 04:09:17Z psteitz $
	/// @since 3.1
	/// </summary>
	public interface UnivariateDifferentiableVectorFunction : UnivariateVectorFunction
	{

		/// <summary>
		/// Compute the value for the function. </summary>
		/// <param name="x"> the point for which the function value should be computed </param>
		/// <returns> the value </returns>
		/// <exception cref="MathIllegalArgumentException"> if {@code x} does not
		/// satisfy the function's constraints (argument out of bound, or unsupported
		/// derivative order for example) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: DerivativeStructure[] value(DerivativeStructure x) throws mathlib.exception.MathIllegalArgumentException;
		DerivativeStructure[] value(DerivativeStructure x);

	}

}
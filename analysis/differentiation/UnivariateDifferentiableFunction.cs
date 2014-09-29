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

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;

	/// <summary>
	/// Interface for univariate functions derivatives.
	/// <p>This interface represents a simple function which computes
	/// both the value and the first derivative of a mathematical function.
	/// The derivative is computed with respect to the input variable.</p> </summary>
	/// <seealso cref= UnivariateDifferentiableFunction </seealso>
	/// <seealso cref= UnivariateFunctionDifferentiator
	/// @since 3.1
	/// @version $Id: UnivariateDifferentiableFunction.java 1462496 2013-03-29 14:56:08Z psteitz $ </seealso>
	public interface UnivariateDifferentiableFunction : UnivariateFunction
	{

		/// <summary>
		/// Simple mathematical function.
		/// <p><seealso cref="UnivariateDifferentiableFunction"/> classes compute both the
		/// value and the first derivative of the function.</p> </summary>
		/// <param name="t"> function input value </param>
		/// <returns> function result </returns>
		/// <exception cref="DimensionMismatchException"> if t is inconsistent with the
		/// function's free parameters or order </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: DerivativeStructure value(DerivativeStructure t) throws mathlib.exception.DimensionMismatchException;
		DerivativeStructure value(DerivativeStructure t);

	}

}
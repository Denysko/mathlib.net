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
namespace mathlib.transform
{

	using UnivariateFunction = mathlib.analysis.UnivariateFunction;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NonMonotonicSequenceException = mathlib.exception.NonMonotonicSequenceException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;

	/// <summary>
	/// Interface for one-dimensional data sets transformations producing real results.
	/// <p>
	/// Such transforms include <seealso cref="FastSineTransformer sine transform"/>,
	/// <seealso cref="FastCosineTransformer cosine transform"/> or {@link
	/// FastHadamardTransformer Hadamard transform}. {@link FastFourierTransformer
	/// Fourier transform} is of a different kind and does not implement this
	/// interface since it produces <seealso cref="mathlib.complex.Complex"/>
	/// results instead of real ones.
	/// 
	/// @version $Id: RealTransformer.java 1385310 2012-09-16 16:32:10Z tn $
	/// @since 2.0
	/// </summary>
	public interface RealTransformer
	{

		/// <summary>
		/// Returns the (forward, inverse) transform of the specified real data set.
		/// </summary>
		/// <param name="f"> the real data array to be transformed (signal) </param>
		/// <param name="type"> the type of transform (forward, inverse) to be performed </param>
		/// <returns> the real transformed array (spectrum) </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array cannot be transformed
		///   with the given type (this may be for example due to array size, which is
		///   constrained in some transforms) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double[] transform(double[] f, TransformType type) throws mathlib.exception.MathIllegalArgumentException;
		double[] transform(double[] f, TransformType type);

		/// <summary>
		/// Returns the (forward, inverse) transform of the specified real function,
		/// sampled on the specified interval.
		/// </summary>
		/// <param name="f"> the function to be sampled and transformed </param>
		/// <param name="min"> the (inclusive) lower bound for the interval </param>
		/// <param name="max"> the (exclusive) upper bound for the interval </param>
		/// <param name="n"> the number of sample points </param>
		/// <param name="type"> the type of transform (forward, inverse) to be performed </param>
		/// <returns> the real transformed array </returns>
		/// <exception cref="NonMonotonicSequenceException"> if the lower bound is greater than, or equal to the upper bound </exception>
		/// <exception cref="NotStrictlyPositiveException"> if the number of sample points is negative </exception>
		/// <exception cref="MathIllegalArgumentException"> if the sample cannot be transformed
		///   with the given type (this may be for example due to sample size, which is
		///   constrained in some transforms) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double[] transform(mathlib.analysis.UnivariateFunction f, double min, double max, int n, TransformType type) throws mathlib.exception.NonMonotonicSequenceException, mathlib.exception.NotStrictlyPositiveException, mathlib.exception.MathIllegalArgumentException;
		double[] transform(UnivariateFunction f, double min, double max, int n, TransformType type);

	}

}
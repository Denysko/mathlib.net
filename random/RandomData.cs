using System;
using System.Collections.Generic;

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

namespace mathlib.random
{

	using NotANumberException = mathlib.exception.NotANumberException;
	using NotFiniteNumberException = mathlib.exception.NotFiniteNumberException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;

	/// <summary>
	/// Random data generation utilities. </summary>
	/// @deprecated to be removed in 4.0.  Use <seealso cref="RandomDataGenerator"/> directly
	/// @version $Id: RandomData.java 1499808 2013-07-04 17:00:42Z sebb $ 
	[Obsolete]//("to be removed in 4.0.  Use <seealso cref="RandomDataGenerator"/> directly")]
	public interface RandomData
	{
		/// <summary>
		/// Generates a random string of hex characters of length {@code len}.
		/// <p>
		/// The generated string will be random, but not cryptographically
		/// secure. To generate cryptographically secure strings, use
		/// <seealso cref="#nextSecureHexString(int)"/>.
		/// </p>
		/// </summary>
		/// <param name="len"> the length of the string to be generated </param>
		/// <returns> a random string of hex characters of length {@code len} </returns>
		/// <exception cref="NotStrictlyPositiveException">
		/// if {@code len <= 0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String nextHexString(int len) throws mathlib.exception.NotStrictlyPositiveException;
		string nextHexString(int len);

		/// <summary>
		/// Generates a uniformly distributed random integer between {@code lower}
		/// and {@code upper} (endpoints included).
		/// <p>
		/// The generated integer will be random, but not cryptographically secure.
		/// To generate cryptographically secure integer sequences, use
		/// <seealso cref="#nextSecureInt(int, int)"/>.
		/// </p>
		/// </summary>
		/// <param name="lower"> lower bound for generated integer </param>
		/// <param name="upper"> upper bound for generated integer </param>
		/// <returns> a random integer greater than or equal to {@code lower}
		/// and less than or equal to {@code upper} </returns>
		/// <exception cref="NumberIsTooLargeException"> if {@code lower >= upper} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int nextInt(int lower, int upper) throws mathlib.exception.NumberIsTooLargeException;
		int nextInt(int lower, int upper);

		/// <summary>
		/// Generates a uniformly distributed random long integer between
		/// {@code lower} and {@code upper} (endpoints included).
		/// <p>
		/// The generated long integer values will be random, but not
		/// cryptographically secure. To generate cryptographically secure sequences
		/// of longs, use <seealso cref="#nextSecureLong(long, long)"/>.
		/// </p>
		/// </summary>
		/// <param name="lower"> lower bound for generated long integer </param>
		/// <param name="upper"> upper bound for generated long integer </param>
		/// <returns> a random long integer greater than or equal to {@code lower} and
		/// less than or equal to {@code upper} </returns>
		/// <exception cref="NumberIsTooLargeException"> if {@code lower >= upper} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: long nextLong(long lower, long upper) throws mathlib.exception.NumberIsTooLargeException;
		long nextLong(long lower, long upper);

		/// <summary>
		/// Generates a random string of hex characters from a secure random
		/// sequence.
		/// <p>
		/// If cryptographic security is not required, use
		/// <seealso cref="#nextHexString(int)"/>.
		/// </p>
		/// </summary>
		/// <param name="len"> the length of the string to be generated </param>
		/// <returns> a random string of hex characters of length {@code len} </returns>
		/// <exception cref="NotStrictlyPositiveException"> if {@code len <= 0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String nextSecureHexString(int len) throws mathlib.exception.NotStrictlyPositiveException;
		string nextSecureHexString(int len);

		/// <summary>
		/// Generates a uniformly distributed random integer between {@code lower}
		/// and {@code upper} (endpoints included) from a secure random sequence.
		/// <p>
		/// Sequences of integers generated using this method will be
		/// cryptographically secure. If cryptographic security is not required,
		/// <seealso cref="#nextInt(int, int)"/> should be used instead of this method.</p>
		/// <p>
		/// <strong>Definition</strong>:
		/// <a href="http://en.wikipedia.org/wiki/Cryptographically_secure_pseudo-random_number_generator">
		/// Secure Random Sequence</a></p>
		/// </summary>
		/// <param name="lower"> lower bound for generated integer </param>
		/// <param name="upper"> upper bound for generated integer </param>
		/// <returns> a random integer greater than or equal to {@code lower} and less
		/// than or equal to {@code upper}. </returns>
		/// <exception cref="NumberIsTooLargeException"> if {@code lower >= upper}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int nextSecureInt(int lower, int upper) throws mathlib.exception.NumberIsTooLargeException;
		int nextSecureInt(int lower, int upper);

		/// <summary>
		/// Generates a uniformly distributed random long integer between
		/// {@code lower} and {@code upper} (endpoints included) from a secure random
		/// sequence.
		/// <p>
		/// Sequences of long values generated using this method will be
		/// cryptographically secure. If cryptographic security is not required,
		/// <seealso cref="#nextLong(long, long)"/> should be used instead of this method.</p>
		/// <p>
		/// <strong>Definition</strong>:
		/// <a href="http://en.wikipedia.org/wiki/Cryptographically_secure_pseudo-random_number_generator">
		/// Secure Random Sequence</a></p>
		/// </summary>
		/// <param name="lower"> lower bound for generated integer </param>
		/// <param name="upper"> upper bound for generated integer </param>
		/// <returns> a random long integer greater than or equal to {@code lower} and
		/// less than or equal to {@code upper}. </returns>
		/// <exception cref="NumberIsTooLargeException"> if {@code lower >= upper}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: long nextSecureLong(long lower, long upper) throws mathlib.exception.NumberIsTooLargeException;
		long nextSecureLong(long lower, long upper);

		/// <summary>
		/// Generates a random value from the Poisson distribution with the given
		/// mean.
		/// <p>
		/// <strong>Definition</strong>:
		/// <a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda366j.htm">
		/// Poisson Distribution</a></p>
		/// </summary>
		/// <param name="mean"> the mean of the Poisson distribution </param>
		/// <returns> a random value following the specified Poisson distribution </returns>
		/// <exception cref="NotStrictlyPositiveException"> if {@code mean <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: long nextPoisson(double mean) throws mathlib.exception.NotStrictlyPositiveException;
		long nextPoisson(double mean);

		/// <summary>
		/// Generates a random value from the Normal (or Gaussian) distribution with
		/// specified mean and standard deviation.
		/// <p>
		/// <strong>Definition</strong>:
		/// <a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda3661.htm">
		/// Normal Distribution</a></p>
		/// </summary>
		/// <param name="mu"> the mean of the distribution </param>
		/// <param name="sigma"> the standard deviation of the distribution </param>
		/// <returns> a random value following the specified Gaussian distribution </returns>
		/// <exception cref="NotStrictlyPositiveException"> if {@code sigma <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double nextGaussian(double mu, double sigma) throws mathlib.exception.NotStrictlyPositiveException;
		double nextGaussian(double mu, double sigma);

		/// <summary>
		/// Generates a random value from the exponential distribution
		/// with specified mean.
		/// <p>
		/// <strong>Definition</strong>:
		/// <a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda3667.htm">
		/// Exponential Distribution</a></p>
		/// </summary>
		/// <param name="mean"> the mean of the distribution </param>
		/// <returns> a random value following the specified exponential distribution </returns>
		/// <exception cref="NotStrictlyPositiveException"> if {@code mean <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double nextExponential(double mean) throws mathlib.exception.NotStrictlyPositiveException;
		double nextExponential(double mean);

		/// <summary>
		/// Generates a uniformly distributed random value from the open interval
		/// {@code (lower, upper)} (i.e., endpoints excluded).
		/// <p>
		/// <strong>Definition</strong>:
		/// <a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda3662.htm">
		/// Uniform Distribution</a> {@code lower} and {@code upper - lower} are the
		/// <a href = "http://www.itl.nist.gov/div898/handbook/eda/section3/eda364.htm">
		/// location and scale parameters</a>, respectively.</p>
		/// </summary>
		/// <param name="lower"> the exclusive lower bound of the support </param>
		/// <param name="upper"> the exclusive upper bound of the support </param>
		/// <returns> a uniformly distributed random value between lower and upper
		/// (exclusive) </returns>
		/// <exception cref="NumberIsTooLargeException"> if {@code lower >= upper} </exception>
		/// <exception cref="NotFiniteNumberException"> if one of the bounds is infinite </exception>
		/// <exception cref="NotANumberException"> if one of the bounds is NaN </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double nextUniform(double lower, double upper) throws mathlib.exception.NumberIsTooLargeException, mathlib.exception.NotFiniteNumberException, mathlib.exception.NotANumberException;
		double nextUniform(double lower, double upper);

		/// <summary>
		/// Generates a uniformly distributed random value from the interval
		/// {@code (lower, upper)} or the interval {@code [lower, upper)}. The lower
		/// bound is thus optionally included, while the upper bound is always
		/// excluded.
		/// <p>
		/// <strong>Definition</strong>:
		/// <a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda3662.htm">
		/// Uniform Distribution</a> {@code lower} and {@code upper - lower} are the
		/// <a href = "http://www.itl.nist.gov/div898/handbook/eda/section3/eda364.htm">
		/// location and scale parameters</a>, respectively.</p>
		/// </summary>
		/// <param name="lower"> the lower bound of the support </param>
		/// <param name="upper"> the exclusive upper bound of the support </param>
		/// <param name="lowerInclusive"> {@code true} if the lower bound is inclusive </param>
		/// <returns> uniformly distributed random value in the {@code (lower, upper)}
		/// interval, if {@code lowerInclusive} is {@code false}, or in the
		/// {@code [lower, upper)} interval, if {@code lowerInclusive} is
		/// {@code true} </returns>
		/// <exception cref="NumberIsTooLargeException"> if {@code lower >= upper} </exception>
		/// <exception cref="NotFiniteNumberException"> if one of the bounds is infinite </exception>
		/// <exception cref="NotANumberException"> if one of the bounds is NaN </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double nextUniform(double lower, double upper, boolean lowerInclusive) throws mathlib.exception.NumberIsTooLargeException, mathlib.exception.NotFiniteNumberException, mathlib.exception.NotANumberException;
		double nextUniform(double lower, double upper, bool lowerInclusive);

		/// <summary>
		/// Generates an integer array of length {@code k} whose entries are selected
		/// randomly, without repetition, from the integers {@code 0, ..., n - 1}
		/// (inclusive).
		/// <p>
		/// Generated arrays represent permutations of {@code n} taken {@code k} at a
		/// time.</p>
		/// </summary>
		/// <param name="n"> the domain of the permutation </param>
		/// <param name="k"> the size of the permutation </param>
		/// <returns> a random {@code k}-permutation of {@code n}, as an array of
		/// integers </returns>
		/// <exception cref="NumberIsTooLargeException"> if {@code k > n}. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if {@code k <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int[] nextPermutation(int n, int k) throws mathlib.exception.NumberIsTooLargeException, mathlib.exception.NotStrictlyPositiveException;
		int[] nextPermutation(int n, int k);

		/// <summary>
		/// Returns an array of {@code k} objects selected randomly from the
		/// Collection {@code c}.
		/// <p>
		/// Sampling from {@code c} is without replacement; but if {@code c} contains
		/// identical objects, the sample may include repeats.  If all elements of
		/// {@code c} are distinct, the resulting object array represents a
		/// <a href="http://rkb.home.cern.ch/rkb/AN16pp/node250.html#SECTION0002500000000000000000">
		/// Simple Random Sample</a> of size {@code k} from the elements of
		/// {@code c}.</p>
		/// </summary>
		/// <param name="c"> the collection to be sampled </param>
		/// <param name="k"> the size of the sample </param>
		/// <returns> a random sample of {@code k} elements from {@code c} </returns>
		/// <exception cref="NumberIsTooLargeException"> if {@code k > c.size()}. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if {@code k <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object[] nextSample(java.util.Collection<?> c, int k) throws mathlib.exception.NumberIsTooLargeException, mathlib.exception.NotStrictlyPositiveException;
		object[] nextSample<T1>(ICollection<T1> c, int k);

	}

}
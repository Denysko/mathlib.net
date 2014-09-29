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


	/// <summary>
	/// Interface extracted from <code>java.util.Random</code>.  This interface is
	/// implemented by <seealso cref="AbstractRandomGenerator"/>.
	/// 
	/// @since 1.1
	/// @version $Id: RandomGenerator.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public interface RandomGenerator
	{

		/// <summary>
		/// Sets the seed of the underlying random number generator using an
		/// <code>int</code> seed.
		/// <p>Sequences of values generated starting with the same seeds
		/// should be identical.
		/// </p> </summary>
		/// <param name="seed"> the seed value </param>
		int Seed {set;}

		/// <summary>
		/// Sets the seed of the underlying random number generator using an
		/// <code>int</code> array seed.
		/// <p>Sequences of values generated starting with the same seeds
		/// should be identical.
		/// </p> </summary>
		/// <param name="seed"> the seed value </param>
		int[] Seed {set;}

		/// <summary>
		/// Sets the seed of the underlying random number generator using a
		/// <code>long</code> seed.
		/// <p>Sequences of values generated starting with the same seeds
		/// should be identical.
		/// </p> </summary>
		/// <param name="seed"> the seed value </param>
		long Seed {set;}

		/// <summary>
		/// Generates random bytes and places them into a user-supplied
		/// byte array.  The number of random bytes produced is equal to
		/// the length of the byte array.
		/// </summary>
		/// <param name="bytes"> the non-null byte array in which to put the
		/// random bytes </param>
		void nextBytes(sbyte[] bytes);

		/// <summary>
		/// Returns the next pseudorandom, uniformly distributed <code>int</code>
		/// value from this random number generator's sequence.
		/// All 2<font size="-1"><sup>32</sup></font> possible <tt>int</tt> values
		/// should be produced with  (approximately) equal probability.
		/// </summary>
		/// <returns> the next pseudorandom, uniformly distributed <code>int</code>
		///  value from this random number generator's sequence </returns>
		int nextInt();

		/// <summary>
		/// Returns a pseudorandom, uniformly distributed <tt>int</tt> value
		/// between 0 (inclusive) and the specified value (exclusive), drawn from
		/// this random number generator's sequence.
		/// </summary>
		/// <param name="n"> the bound on the random number to be returned.  Must be
		/// positive. </param>
		/// <returns>  a pseudorandom, uniformly distributed <tt>int</tt>
		/// value between 0 (inclusive) and n (exclusive). </returns>
		/// <exception cref="IllegalArgumentException">  if n is not positive. </exception>
		int nextInt(int n);

		/// <summary>
		/// Returns the next pseudorandom, uniformly distributed <code>long</code>
		/// value from this random number generator's sequence.  All
		/// 2<font size="-1"><sup>64</sup></font> possible <tt>long</tt> values
		/// should be produced with (approximately) equal probability.
		/// </summary>
		/// <returns>  the next pseudorandom, uniformly distributed <code>long</code>
		/// value from this random number generator's sequence </returns>
		long nextLong();

		/// <summary>
		/// Returns the next pseudorandom, uniformly distributed
		/// <code>boolean</code> value from this random number generator's
		/// sequence.
		/// </summary>
		/// <returns>  the next pseudorandom, uniformly distributed
		/// <code>boolean</code> value from this random number generator's
		/// sequence </returns>
		bool nextBoolean();

		/// <summary>
		/// Returns the next pseudorandom, uniformly distributed <code>float</code>
		/// value between <code>0.0</code> and <code>1.0</code> from this random
		/// number generator's sequence.
		/// </summary>
		/// <returns>  the next pseudorandom, uniformly distributed <code>float</code>
		/// value between <code>0.0</code> and <code>1.0</code> from this
		/// random number generator's sequence </returns>
		float nextFloat();

		/// <summary>
		/// Returns the next pseudorandom, uniformly distributed
		/// <code>double</code> value between <code>0.0</code> and
		/// <code>1.0</code> from this random number generator's sequence.
		/// </summary>
		/// <returns>  the next pseudorandom, uniformly distributed
		///  <code>double</code> value between <code>0.0</code> and
		///  <code>1.0</code> from this random number generator's sequence </returns>
		double nextDouble();

		/// <summary>
		/// Returns the next pseudorandom, Gaussian ("normally") distributed
		/// <code>double</code> value with mean <code>0.0</code> and standard
		/// deviation <code>1.0</code> from this random number generator's sequence.
		/// </summary>
		/// <returns>  the next pseudorandom, Gaussian ("normally") distributed
		/// <code>double</code> value with mean <code>0.0</code> and
		/// standard deviation <code>1.0</code> from this random number
		///  generator's sequence </returns>
		double nextGaussian();
	}

}
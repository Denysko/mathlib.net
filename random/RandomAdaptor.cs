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
namespace mathlib.random
{

	/// <summary>
	/// Extension of <code>java.util.Random</code> wrapping a
	/// <seealso cref="RandomGenerator"/>.
	/// 
	/// @since 1.1
	/// @version $Id: RandomAdaptor.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class RandomAdaptor : Random, RandomGenerator
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 2306581345647615033L;

		/// <summary>
		/// Wrapped randomGenerator instance </summary>
		private readonly RandomGenerator randomGenerator;

		/// <summary>
		/// Prevent instantiation without a generator argument
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") private RandomAdaptor()
		private RandomAdaptor()
		{
			randomGenerator = null;
		}

		/// <summary>
		/// Construct a RandomAdaptor wrapping the supplied RandomGenerator.
		/// </summary>
		/// <param name="randomGenerator">  the wrapped generator </param>
		public RandomAdaptor(RandomGenerator randomGenerator)
		{
			this.randomGenerator = randomGenerator;
		}

		/// <summary>
		/// Factory method to create a <code>Random</code> using the supplied
		/// <code>RandomGenerator</code>.
		/// </summary>
		/// <param name="randomGenerator">  wrapped RandomGenerator instance </param>
		/// <returns> a Random instance wrapping the RandomGenerator </returns>
		public static Random createAdaptor(RandomGenerator randomGenerator)
		{
			return new RandomAdaptor(randomGenerator);
		}

		/// <summary>
		/// Returns the next pseudorandom, uniformly distributed
		/// <code>boolean</code> value from this random number generator's
		/// sequence.
		/// </summary>
		/// <returns>  the next pseudorandom, uniformly distributed
		/// <code>boolean</code> value from this random number generator's
		/// sequence </returns>
		public override bool nextBoolean()
		{
			return randomGenerator.nextBoolean();
		}

		 /// <summary>
		 /// Generates random bytes and places them into a user-supplied
		 /// byte array.  The number of random bytes produced is equal to
		 /// the length of the byte array.
		 /// </summary>
		 /// <param name="bytes"> the non-null byte array in which to put the
		 /// random bytes </param>
		public override void nextBytes(sbyte[] bytes)
		{
			randomGenerator.nextBytes(bytes);
		}

		 /// <summary>
		 /// Returns the next pseudorandom, uniformly distributed
		 /// <code>double</code> value between <code>0.0</code> and
		 /// <code>1.0</code> from this random number generator's sequence.
		 /// </summary>
		 /// <returns>  the next pseudorandom, uniformly distributed
		 ///  <code>double</code> value between <code>0.0</code> and
		 ///  <code>1.0</code> from this random number generator's sequence </returns>
		public override double nextDouble()
		{
			return randomGenerator.NextDouble();
		}

		/// <summary>
		/// Returns the next pseudorandom, uniformly distributed <code>float</code>
		/// value between <code>0.0</code> and <code>1.0</code> from this random
		/// number generator's sequence.
		/// </summary>
		/// <returns>  the next pseudorandom, uniformly distributed <code>float</code>
		/// value between <code>0.0</code> and <code>1.0</code> from this
		/// random number generator's sequence </returns>
		public override float nextFloat()
		{
			return randomGenerator.nextFloat();
		}

		/// <summary>
		/// Returns the next pseudorandom, Gaussian ("normally") distributed
		/// <code>double</code> value with mean <code>0.0</code> and standard
		/// deviation <code>1.0</code> from this random number generator's sequence.
		/// </summary>
		/// <returns>  the next pseudorandom, Gaussian ("normally") distributed
		/// <code>double</code> value with mean <code>0.0</code> and
		/// standard deviation <code>1.0</code> from this random number
		///  generator's sequence </returns>
		public override double nextGaussian()
		{
			return randomGenerator.nextGaussian();
		}

		 /// <summary>
		 /// Returns the next pseudorandom, uniformly distributed <code>int</code>
		 /// value from this random number generator's sequence.
		 /// All 2<font size="-1"><sup>32</sup></font> possible <tt>int</tt> values
		 /// should be produced with  (approximately) equal probability.
		 /// </summary>
		 /// <returns> the next pseudorandom, uniformly distributed <code>int</code>
		 ///  value from this random number generator's sequence </returns>
		public override int nextInt()
		{
			return randomGenerator.Next();
		}

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
		public override int nextInt(int n)
		{
			return randomGenerator.Next(n);
		}

		/// <summary>
		/// Returns the next pseudorandom, uniformly distributed <code>long</code>
		/// value from this random number generator's sequence.  All
		/// 2<font size="-1"><sup>64</sup></font> possible <tt>long</tt> values
		/// should be produced with (approximately) equal probability.
		/// </summary>
		/// <returns>  the next pseudorandom, uniformly distributed <code>long</code>
		/// value from this random number generator's sequence </returns>
		public override long nextLong()
		{
			return randomGenerator.nextLong();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int Seed
		{
			set
			{
				if (randomGenerator != null) // required to avoid NPE in constructor
				{
					randomGenerator.Seed = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int[] Seed
		{
			set
			{
				if (randomGenerator != null) // required to avoid NPE in constructor
				{
					randomGenerator.Seed = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override long Seed
		{
			set
			{
				if (randomGenerator != null) // required to avoid NPE in constructor
				{
					randomGenerator.Seed = value;
				}
			}
		}

	}

}
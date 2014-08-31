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
namespace org.apache.commons.math3.random
{

	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;

	/// <summary>
	/// Utilities for creating <seealso cref="RandomGenerator"/> instances.
	/// 
	/// @since 3.3
	/// @version $Id: RandomGeneratorFactory.java 1509230 2013-08-01 13:34:46Z erans $
	/// </summary>
	public class RandomGeneratorFactory
	{
		/// <summary>
		/// Class contains only static methods.
		/// </summary>
		private RandomGeneratorFactory()
		{
		}

		/// <summary>
		/// Creates a <seealso cref="RandomDataGenerator"/> instance that wraps a
		/// <seealso cref="Random"/> instance.
		/// </summary>
		/// <param name="rng"> JDK <seealso cref="Random"/> instance that will generate the
		/// the random data. </param>
		/// <returns> the given RNG, wrapped in a <seealso cref="RandomGenerator"/>. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static RandomGenerator createRandomGenerator(final java.util.Random rng)
		public static RandomGenerator createRandomGenerator(Random rng)
		{
			return new RandomGeneratorAnonymousInnerClassHelper(rng);
		}

		private class RandomGeneratorAnonymousInnerClassHelper : RandomGenerator
		{
			private Random rng;

			public RandomGeneratorAnonymousInnerClassHelper(Random rng)
			{
				this.rng = rng;
			}

					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual int Seed
			{
				set
				{
					rng.Seed = (long) value;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual int[] Seed
			{
				set
				{
					rng.Seed = convertToLong(value);
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual long Seed
			{
				set
				{
					rng.Seed = value;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual void nextBytes(sbyte[] bytes)
			{
				rng.nextBytes(bytes);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual int nextInt()
			{
				return rng.Next();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual int nextInt(int n)
			{
				if (n <= 0)
				{
					throw new NotStrictlyPositiveException(n);
				}
				return rng.Next(n);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual long nextLong()
			{
				return rng.nextLong();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual bool nextBoolean()
			{
				return rng.nextBoolean();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual float nextFloat()
			{
				return rng.nextFloat();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual double nextDouble()
			{
				return rng.NextDouble();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual double nextGaussian()
			{
				return rng.nextGaussian();
			}
		}

		/// <summary>
		/// Converts seed from one representation to another.
		/// </summary>
		/// <param name="seed"> Original seed. </param>
		/// <returns> the converted seed. </returns>
		public static long convertToLong(int[] seed)
		{
			// The following number is the largest prime that fits
			// in 32 bits (i.e. 2^32 - 5).
			const long prime = 4294967291l;

			long combined = 0l;
			foreach (int s in seed)
			{
				combined = combined * prime + s;
			}

			return combined;
		}
	}

}
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
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Base class for random number generators that generates bits streams.
	/// 
	/// @version $Id: BitsStreamGenerator.java 1538368 2013-11-03 13:57:37Z erans $
	/// @since 2.0
	/// </summary>
	[Serializable]
	public abstract class BitsStreamGenerator : RandomGenerator
	{
		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 20130104L;
		/// <summary>
		/// Next gaussian. </summary>
		private double nextGaussian_Renamed;

		/// <summary>
		/// Creates a new random number generator.
		/// </summary>
		public BitsStreamGenerator()
		{
			nextGaussian_Renamed = double.NaN;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public abstract int Seed {set;}

		/// <summary>
		/// {@inheritDoc} </summary>
		public abstract int[] Seed {set;}

		/// <summary>
		/// {@inheritDoc} </summary>
		public abstract long Seed {set;}

		/// <summary>
		/// Generate next pseudorandom number.
		/// <p>This method is the core generation algorithm. It is used by all the
		/// public generation methods for the various primitive types {@link
		/// #nextBoolean()}, <seealso cref="#nextBytes(byte[])"/>, <seealso cref="#nextDouble()"/>,
		/// <seealso cref="#nextFloat()"/>, <seealso cref="#nextGaussian()"/>, <seealso cref="#nextInt()"/>,
		/// <seealso cref="#next(int)"/> and <seealso cref="#nextLong()"/>.</p> </summary>
		/// <param name="bits"> number of random bits to produce </param>
		/// <returns> random bits generated </returns>
		protected internal abstract int next(int bits);

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual bool nextBoolean()
		{
			return next(1) != 0;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual void nextBytes(sbyte[] bytes)
		{
			int i = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iEnd = bytes.length - 3;
			int iEnd = bytes.Length - 3;
			while (i < iEnd)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int random = next(32);
				int random = next(32);
				bytes[i] = unchecked((sbyte)(random & 0xff));
				bytes[i + 1] = unchecked((sbyte)((random >> 8) & 0xff));
				bytes[i + 2] = unchecked((sbyte)((random >> 16) & 0xff));
				bytes[i + 3] = unchecked((sbyte)((random >> 24) & 0xff));
				i += 4;
			}
			int random = next(32);
			while (i < bytes.Length)
			{
				bytes[i++] = unchecked((sbyte)(random & 0xff));
				random >>= 8;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double nextDouble()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long high = ((long) next(26)) << 26;
			long high = ((long) next(26)) << 26;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int low = next(26);
			int low = next(26);
			return (high | low) * 0x1.0p - 52d;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual float nextFloat()
		{
			return next(23) * 0x1.0p - 23f;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double nextGaussian()
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double random;
			double random;
			if (double.IsNaN(nextGaussian_Renamed))
			{
				// generate a new pair of gaussian numbers
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = nextDouble();
				double x = nextDouble();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = nextDouble();
				double y = nextDouble();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double alpha = 2 * org.apache.commons.math3.util.FastMath.PI * x;
				double alpha = 2 * FastMath.PI * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double r = org.apache.commons.math3.util.FastMath.sqrt(-2 * org.apache.commons.math3.util.FastMath.log(y));
				double r = FastMath.sqrt(-2 * FastMath.log(y));
				random = r * FastMath.cos(alpha);
				nextGaussian_Renamed = r * FastMath.sin(alpha);
			}
			else
			{
				// use the second element of the pair already generated
				random = nextGaussian_Renamed;
				nextGaussian_Renamed = double.NaN;
			}

			return random;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int nextInt()
		{
			return next(32);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>This default implementation is copied from Apache Harmony
		/// java.util.Random (r929253).</p>
		/// 
		/// <p>Implementation notes: <ul>
		/// <li>If n is a power of 2, this method returns
		/// {@code (int) ((n * (long) next(31)) >> 31)}.</li>
		/// 
		/// <li>If n is not a power of 2, what is returned is {@code next(31) % n}
		/// with {@code next(31)} values rejected (i.e. regenerated) until a
		/// value that is larger than the remainder of {@code Integer.MAX_VALUE / n}
		/// is generated. Rejection of this initial segment is necessary to ensure
		/// a uniform distribution.</li></ul></p>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextInt(int n) throws IllegalArgumentException
		public virtual int nextInt(int n)
		{
			if (n > 0)
			{
				if ((n & -n) == n)
				{
					return (int)((n * (long) next(31)) >> 31);
				}
				int bits;
				int val;
				do
				{
					bits = next(31);
					val = bits % n;
				} while (bits - val + (n - 1) < 0);
				return val;
			}
			throw new NotStrictlyPositiveException(n);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual long nextLong()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long high = ((long) next(32)) << 32;
			long high = ((long) next(32)) << 32;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long low = ((long) next(32)) & 0xffffffffL;
			long low = ((long) next(32)) & 0xffffffffL;
			return high | low;
		}

		/// <summary>
		/// Returns a pseudorandom, uniformly distributed <tt>long</tt> value
		/// between 0 (inclusive) and the specified value (exclusive), drawn from
		/// this random number generator's sequence.
		/// </summary>
		/// <param name="n"> the bound on the random number to be returned.  Must be
		/// positive. </param>
		/// <returns>  a pseudorandom, uniformly distributed <tt>long</tt>
		/// value between 0 (inclusive) and n (exclusive). </returns>
		/// <exception cref="IllegalArgumentException">  if n is not positive. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long nextLong(long n) throws IllegalArgumentException
		public virtual long nextLong(long n)
		{
			if (n > 0)
			{
				long bits;
				long val;
				do
				{
					bits = ((long) next(31)) << 32;
					bits |= ((long) next(32)) & 0xffffffffL;
					val = bits % n;
				} while (bits - val + (n - 1) < 0);
				return val;
			}
			throw new NotStrictlyPositiveException(n);
		}

		/// <summary>
		/// Clears the cache used by the default implementation of
		/// <seealso cref="#nextGaussian"/>.
		/// </summary>
		public virtual void clear()
		{
			nextGaussian_Renamed = double.NaN;
		}

	}

}
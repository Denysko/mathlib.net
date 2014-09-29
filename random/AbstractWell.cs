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

	using FastMath = mathlib.util.FastMath;


	/// <summary>
	/// This abstract class implements the WELL class of pseudo-random number generator
	/// from Fran&ccedil;ois Panneton, Pierre L'Ecuyer and Makoto Matsumoto.
	/// 
	/// <p>This generator is described in a paper by Fran&ccedil;ois Panneton,
	/// Pierre L'Ecuyer and Makoto Matsumoto <a
	/// href="http://www.iro.umontreal.ca/~lecuyer/myftp/papers/wellrng.pdf">Improved
	/// Long-Period Generators Based on Linear Recurrences Modulo 2</a> ACM
	/// Transactions on Mathematical Software, 32, 1 (2006). The errata for the paper
	/// are in <a href="http://www.iro.umontreal.ca/~lecuyer/myftp/papers/wellrng-errata.txt">wellrng-errata.txt</a>.</p>
	/// </summary>
	/// <seealso cref= <a href="http://www.iro.umontreal.ca/~panneton/WELLRNG.html">WELL Random number generator</a>
	/// @version $Id: AbstractWell.java 1547633 2013-12-03 23:03:06Z tn $
	/// @since 2.2
	///  </seealso>
	[Serializable]
	public abstract class AbstractWell : BitsStreamGenerator
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -817701723016583596L;

		/// <summary>
		/// Current index in the bytes pool. </summary>
		protected internal int index;

		/// <summary>
		/// Bytes pool. </summary>
		protected internal readonly int[] v;

		/// <summary>
		/// Index indirection table giving for each index its predecessor taking table size into account. </summary>
		protected internal readonly int[] iRm1;

		/// <summary>
		/// Index indirection table giving for each index its second predecessor taking table size into account. </summary>
		protected internal readonly int[] iRm2;

		/// <summary>
		/// Index indirection table giving for each index the value index + m1 taking table size into account. </summary>
		protected internal readonly int[] i1;

		/// <summary>
		/// Index indirection table giving for each index the value index + m2 taking table size into account. </summary>
		protected internal readonly int[] i2;

		/// <summary>
		/// Index indirection table giving for each index the value index + m3 taking table size into account. </summary>
		protected internal readonly int[] i3;

		/// <summary>
		/// Creates a new random number generator.
		/// <p>The instance is initialized using the current time plus the
		/// system identity hash code of this instance as the seed.</p> </summary>
		/// <param name="k"> number of bits in the pool (not necessarily a multiple of 32) </param>
		/// <param name="m1"> first parameter of the algorithm </param>
		/// <param name="m2"> second parameter of the algorithm </param>
		/// <param name="m3"> third parameter of the algorithm </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractWell(final int k, final int m1, final int m2, final int m3)
		protected internal AbstractWell(int k, int m1, int m2, int m3) : this(k, m1, m2, m3, null)
		{
		}

		/// <summary>
		/// Creates a new random number generator using a single int seed. </summary>
		/// <param name="k"> number of bits in the pool (not necessarily a multiple of 32) </param>
		/// <param name="m1"> first parameter of the algorithm </param>
		/// <param name="m2"> second parameter of the algorithm </param>
		/// <param name="m3"> third parameter of the algorithm </param>
		/// <param name="seed"> the initial seed (32 bits integer) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractWell(final int k, final int m1, final int m2, final int m3, final int seed)
		protected internal AbstractWell(int k, int m1, int m2, int m3, int seed) : this(k, m1, m2, m3, new int[] {seed})
		{
		}

		/// <summary>
		/// Creates a new random number generator using an int array seed. </summary>
		/// <param name="k"> number of bits in the pool (not necessarily a multiple of 32) </param>
		/// <param name="m1"> first parameter of the algorithm </param>
		/// <param name="m2"> second parameter of the algorithm </param>
		/// <param name="m3"> third parameter of the algorithm </param>
		/// <param name="seed"> the initial seed (32 bits integers array), if null
		/// the seed of the generator will be related to the current time </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractWell(final int k, final int m1, final int m2, final int m3, final int[] seed)
		protected internal AbstractWell(int k, int m1, int m2, int m3, int[] seed)
		{

			// the bits pool contains k bits, k = r w - p where r is the number
			// of w bits blocks, w is the block size (always 32 in the original paper)
			// and p is the number of unused bits in the last block
			const int w = 32;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int r = (k + w - 1) / w;
			int r = (k + w - 1) / w;
			this.v = new int[r];
			this.index = 0;

			// precompute indirection index tables. These tables are used for optimizing access
			// they allow saving computations like "(j + r - 2) % r" with costly modulo operations
			iRm1 = new int[r];
			iRm2 = new int[r];
			i1 = new int[r];
			i2 = new int[r];
			i3 = new int[r];
			for (int j = 0; j < r; ++j)
			{
				iRm1[j] = (j + r - 1) % r;
				iRm2[j] = (j + r - 2) % r;
				i1[j] = (j + m1) % r;
				i2[j] = (j + m2) % r;
				i3[j] = (j + m3) % r;
			}

			// initialize the pool content
			Seed = seed;

		}

		/// <summary>
		/// Creates a new random number generator using a single long seed. </summary>
		/// <param name="k"> number of bits in the pool (not necessarily a multiple of 32) </param>
		/// <param name="m1"> first parameter of the algorithm </param>
		/// <param name="m2"> second parameter of the algorithm </param>
		/// <param name="m3"> third parameter of the algorithm </param>
		/// <param name="seed"> the initial seed (64 bits integer) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractWell(final int k, final int m1, final int m2, final int m3, final long seed)
		protected internal AbstractWell(int k, int m1, int m2, int m3, long seed) : this(k, m1, m2, m3, new int[] {(int)((long)((ulong)seed >> 32)), (int)(seed & 0xffffffffl)})
		{
		}

		/// <summary>
		/// Reinitialize the generator as if just built with the given int seed.
		/// <p>The state of the generator is exactly the same as a new
		/// generator built with the same seed.</p> </summary>
		/// <param name="seed"> the initial seed (32 bits integer) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void setSeed(final int seed)
		public override int Seed
		{
			set
			{
				Seed = new int[] {value};
			}
		}

		/// <summary>
		/// Reinitialize the generator as if just built with the given int array seed.
		/// <p>The state of the generator is exactly the same as a new
		/// generator built with the same seed.</p> </summary>
		/// <param name="seed"> the initial seed (32 bits integers array). If null
		/// the seed of the generator will be the system time plus the system identity
		/// hash code of the instance. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void setSeed(final int[] seed)
		public override int[] Seed
		{
			set
			{
				if (value == null)
				{
					Seed = System.currentTimeMillis() + System.identityHashCode(this);
					return;
				}
    
				Array.Copy(value, 0, v, 0, FastMath.min(value.Length, v.Length));
    
				if (value.Length < v.Length)
				{
					for (int i = value.Length; i < v.Length; ++i)
					{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final long l = v[i - value.length];
						long l = v[i - value.Length];
						v[i] = (int)((1812433253l * (l ^ (l >> 30)) + i) & 0xffffffffL);
					}
				}
    
				index = 0;
				clear(); // Clear normal deviate cache
			}
		}

		/// <summary>
		/// Reinitialize the generator as if just built with the given long seed.
		/// <p>The state of the generator is exactly the same as a new
		/// generator built with the same seed.</p> </summary>
		/// <param name="seed"> the initial seed (64 bits integer) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void setSeed(final long seed)
		public override long Seed
		{
			set
			{
				Seed = new int[] {(int)((long)((ulong)value >> 32)), (int)(value & 0xffffffffl)};
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected abstract int next(final int bits);
		protected internal override abstract int next(int bits);

	}

}
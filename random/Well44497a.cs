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
	/// This class implements the WELL44497a pseudo-random number generator
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
	/// @version $Id: Well44497a.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.2
	///  </seealso>
	public class Well44497a : AbstractWell
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -3859207588353972099L;

		/// <summary>
		/// Number of bits in the pool. </summary>
		private const int K = 44497;

		/// <summary>
		/// First parameter of the algorithm. </summary>
		private const int M1 = 23;

		/// <summary>
		/// Second parameter of the algorithm. </summary>
		private const int M2 = 481;

		/// <summary>
		/// Third parameter of the algorithm. </summary>
		private const int M3 = 229;

		/// <summary>
		/// Creates a new random number generator.
		/// <p>The instance is initialized using the current time as the
		/// seed.</p>
		/// </summary>
		public Well44497a() : base(K, M1, M2, M3)
		{
		}

		/// <summary>
		/// Creates a new random number generator using a single int seed. </summary>
		/// <param name="seed"> the initial seed (32 bits integer) </param>
		public Well44497a(int seed) : base(K, M1, M2, M3, seed)
		{
		}

		/// <summary>
		/// Creates a new random number generator using an int array seed. </summary>
		/// <param name="seed"> the initial seed (32 bits integers array), if null
		/// the seed of the generator will be related to the current time </param>
		public Well44497a(int[] seed) : base(K, M1, M2, M3, seed)
		{
		}

		/// <summary>
		/// Creates a new random number generator using a single long seed. </summary>
		/// <param name="seed"> the initial seed (64 bits integer) </param>
		public Well44497a(long seed) : base(K, M1, M2, M3, seed)
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected int next(final int bits)
		protected internal override int next(int bits)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int indexRm1 = iRm1[index];
			int indexRm1 = iRm1[index];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int indexRm2 = iRm2[index];
			int indexRm2 = iRm2[index];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int v0 = v[index];
			int v0 = v[index];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int vM1 = v[i1[index]];
			int vM1 = v[i1[index]];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int vM2 = v[i2[index]];
			int vM2 = v[i2[index]];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int vM3 = v[i3[index]];
			int vM3 = v[i3[index]];

			// the values below include the errata of the original article
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int z0 = (0xFFFF8000 & v[indexRm1]) ^ (0x00007FFF & v[indexRm2]);
			int z0 = (0xFFFF8000 & v[indexRm1]) ^ (0x00007FFF & v[indexRm2]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int z1 = (v0 ^ (v0 << 24)) ^ (vM1 ^ (vM1 >>> 30));
			int z1 = (v0 ^ (v0 << 24)) ^ (vM1 ^ ((int)((uint)vM1 >> 30)));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int z2 = (vM2 ^ (vM2 << 10)) ^ (vM3 << 26);
			int z2 = (vM2 ^ (vM2 << 10)) ^ (vM3 << 26);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int z3 = z1 ^ z2;
			int z3 = z1 ^ z2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int z2Prime = ((z2 << 9) ^ (z2 >>> 23)) & 0xfbffffff;
			int z2Prime = ((z2 << 9) ^ ((int)((uint)z2 >> 23))) & unchecked((int)0xfbffffff);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int z2Second = ((z2 & 0x00020000) != 0) ? (z2Prime ^ 0xb729fcec) : z2Prime;
			int z2Second = ((z2 & 0x00020000) != 0) ? (z2Prime ^ 0xb729fcec) : z2Prime;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int z4 = z0 ^ (z1 ^ (z1 >>> 20)) ^ z2Second ^ z3;
			int z4 = z0 ^ (z1 ^ ((int)((uint)z1 >> 20))) ^ z2Second ^ z3;

			v[index] = z3;
			v[indexRm1] = z4;
			v[indexRm2] &= unchecked((int)0xFFFF8000);
			index = indexRm1;

			return (int)((uint)z4 >> (32 - bits));

		}
	}

}
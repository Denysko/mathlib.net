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
	/// <a href="http://burtleburtle.net/bob/rand/isaacafa.html">
	///  ISAAC: a fast cryptographic pseudo-random number generator</a>
	/// <br/>
	/// ISAAC (Indirection, Shift, Accumulate, Add, and Count) generates 32-bit
	/// random numbers.
	/// ISAAC has been designed to be cryptographically secure and is inspired
	/// by RC4.
	/// Cycles are guaranteed to be at least 2<sup>40</sup> values long, and they
	/// are 2<sup>8295</sup> values long on average.
	/// The results are uniformly distributed, unbiased, and unpredictable unless
	/// you know the seed.
	/// <br/>
	/// This code is based (with minor changes and improvements) on the original
	/// implementation of the algorithm by Bob Jenkins.
	/// <br/>
	/// 
	/// @version $Id: ISAACRandom.java 1547633 2013-12-03 23:03:06Z tn $
	/// @since 3.0
	/// </summary>
	[Serializable]
	public class ISAACRandom : BitsStreamGenerator
	{
		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 7288197941165002400L;
		/// <summary>
		/// Log of size of rsl[] and mem[] </summary>
		private const int SIZE_L = 8;
		/// <summary>
		/// Size of rsl[] and mem[] </summary>
		private static readonly int SIZE = 1 << SIZE_L;
		/// <summary>
		/// Half-size of rsl[] and mem[] </summary>
		private static readonly int H_SIZE = SIZE >> 1;
		/// <summary>
		/// For pseudo-random lookup </summary>
		private static readonly int MASK = SIZE - 1 << 2;
		/// <summary>
		/// The golden ratio </summary>
		private const int GLD_RATIO = unchecked((int)0x9e3779b9);
		/// <summary>
		/// The results given to the user </summary>
		private readonly int[] rsl = new int[SIZE];
		/// <summary>
		/// The internal state </summary>
		private readonly int[] mem = new int[SIZE];
		/// <summary>
		/// Count through the results in rsl[] </summary>
		private int count;
		/// <summary>
		/// Accumulator </summary>
		private int isaacA;
		/// <summary>
		/// The last result </summary>
		private int isaacB;
		/// <summary>
		/// Counter, guarantees cycle is at least 2^40 </summary>
		private int isaacC;
		/// <summary>
		/// Service variable. </summary>
		private readonly int[] arr = new int[8];
		/// <summary>
		/// Service variable. </summary>
		private int isaacX;
		/// <summary>
		/// Service variable. </summary>
		private int isaacI;
		/// <summary>
		/// Service variable. </summary>
		private int isaacJ;


		/// <summary>
		/// Creates a new ISAAC random number generator.
		/// <br/>
		/// The instance is initialized using a combination of the
		/// current time and system hash code of the instance as the seed.
		/// </summary>
		public ISAACRandom()
		{
			Seed = System.currentTimeMillis() + System.identityHashCode(this);
		}

		/// <summary>
		/// Creates a new ISAAC random number generator using a single long seed.
		/// </summary>
		/// <param name="seed"> Initial seed. </param>
		public ISAACRandom(long seed)
		{
			Seed = seed;
		}

		/// <summary>
		/// Creates a new ISAAC random number generator using an int array seed.
		/// </summary>
		/// <param name="seed"> Initial seed. If {@code null}, the seed will be related
		/// to the current time. </param>
		public ISAACRandom(int[] seed)
		{
			Seed = seed;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int Seed
		{
			set
			{
				Seed = new int[]{value};
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override long Seed
		{
			set
			{
				Seed = new int[]{(int)((long)((ulong)value >> 32)), (int)(value & 0xffffffffL)};
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int[] Seed
		{
			set
			{
				if (value == null)
				{
					Seed = System.currentTimeMillis() + System.identityHashCode(this);
					return;
				}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int seedLen = value.length;
				int seedLen = value.Length;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int rslLen = rsl.length;
				int rslLen = rsl.Length;
				Array.Copy(value, 0, rsl, 0, FastMath.min(seedLen, rslLen));
				if (seedLen < rslLen)
				{
					for (int j = seedLen; j < rslLen; j++)
					{
						long k = rsl[j - seedLen];
						rsl[j] = (int)(0x6c078965L * (k ^ k >> 30) + j & 0xffffffffL);
					}
				}
				initState();
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override int next(int bits)
		{
			if (count < 0)
			{
				isaac();
				count = SIZE - 1;
			}
			return (int)((uint)rsl[count--] >> 32 - bits);
		}

		/// <summary>
		/// Generate 256 results </summary>
		private void isaac()
		{
			isaacI = 0;
			isaacJ = H_SIZE;
			isaacB += ++isaacC;
			while (isaacI < H_SIZE)
			{
				isaac2();
			}
			isaacJ = 0;
			while (isaacJ < H_SIZE)
			{
				isaac2();
			}
		}

		/// <summary>
		/// Intermediate internal loop. </summary>
		private void isaac2()
		{
			isaacX = mem[isaacI];
			isaacA ^= isaacA << 13;
			isaacA += mem[isaacJ++];
			isaac3();
			isaacX = mem[isaacI];
			isaacA ^= (int)((uint)isaacA >> 6);
			isaacA += mem[isaacJ++];
			isaac3();
			isaacX = mem[isaacI];
			isaacA ^= isaacA << 2;
			isaacA += mem[isaacJ++];
			isaac3();
			isaacX = mem[isaacI];
			isaacA ^= (int)((uint)isaacA >> 16);
			isaacA += mem[isaacJ++];
			isaac3();
		}

		/// <summary>
		/// Lowest level internal loop. </summary>
		private void isaac3()
		{
			mem[isaacI] = mem[(isaacX & MASK) >> 2] + isaacA + isaacB;
			isaacB = mem[(mem[isaacI] >> SIZE_L & MASK) >> 2] + isaacX;
			rsl[isaacI++] = isaacB;
		}

		/// <summary>
		/// Initialize, or reinitialize, this instance of rand. </summary>
		private void initState()
		{
			isaacA = 0;
			isaacB = 0;
			isaacC = 0;
			for (int j = 0; j < arr.Length; j++)
			{
				arr[j] = GLD_RATIO;
			}
			for (int j = 0; j < 4; j++)
			{
				shuffle();
			}
			// fill in mem[] with messy stuff
			for (int j = 0; j < SIZE; j += 8)
			{
				arr[0] += rsl[j];
				arr[1] += rsl[j + 1];
				arr[2] += rsl[j + 2];
				arr[3] += rsl[j + 3];
				arr[4] += rsl[j + 4];
				arr[5] += rsl[j + 5];
				arr[6] += rsl[j + 6];
				arr[7] += rsl[j + 7];
				shuffle();
				State = j;
			}
			// second pass makes all of seed affect all of mem
			for (int j = 0; j < SIZE; j += 8)
			{
				arr[0] += mem[j];
				arr[1] += mem[j + 1];
				arr[2] += mem[j + 2];
				arr[3] += mem[j + 3];
				arr[4] += mem[j + 4];
				arr[5] += mem[j + 5];
				arr[6] += mem[j + 6];
				arr[7] += mem[j + 7];
				shuffle();
				State = j;
			}
			isaac();
			count = SIZE - 1;
			clear();
		}

		/// <summary>
		/// Shuffle array. </summary>
		private void shuffle()
		{
			arr[0] ^= arr[1] << 11;
			arr[3] += arr[0];
			arr[1] += arr[2];
			arr[1] ^= (int)((uint)arr[2] >> 2);
			arr[4] += arr[1];
			arr[2] += arr[3];
			arr[2] ^= arr[3] << 8;
			arr[5] += arr[2];
			arr[3] += arr[4];
			arr[3] ^= (int)((uint)arr[4] >> 16);
			arr[6] += arr[3];
			arr[4] += arr[5];
			arr[4] ^= arr[5] << 10;
			arr[7] += arr[4];
			arr[5] += arr[6];
			arr[5] ^= (int)((uint)arr[6] >> 4);
			arr[0] += arr[5];
			arr[6] += arr[7];
			arr[6] ^= arr[7] << 8;
			arr[1] += arr[6];
			arr[7] += arr[0];
			arr[7] ^= (int)((uint)arr[0] >> 9);
			arr[2] += arr[7];
			arr[0] += arr[1];
		}

		/// <summary>
		/// Set the state by copying the internal arrays.
		/// </summary>
		/// <param name="start"> First index into <seealso cref="#mem"/> array. </param>
		private int State
		{
			set
			{
				mem[value] = arr[0];
				mem[value + 1] = arr[1];
				mem[value + 2] = arr[2];
				mem[value + 3] = arr[3];
				mem[value + 4] = arr[4];
				mem[value + 5] = arr[5];
				mem[value + 6] = arr[6];
				mem[value + 7] = arr[7];
			}
		}
	}

}
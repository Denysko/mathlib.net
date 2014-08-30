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

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// Implementation of a Halton sequence.
	/// <p>
	/// A Halton sequence is a low-discrepancy sequence generating points in the interval [0, 1] according to
	/// <pre>
	///   H(n) = d_0 / b + d_1 / b^2 .... d_j / b^j+1
	/// 
	///   with
	/// 
	///   n = d_j * b^j-1 + ... d_1 * b + d_0 * b^0
	/// </pre>
	/// For higher dimensions, subsequent prime numbers are used as base, e.g. { 2, 3, 5 } for a Halton sequence in R^3.
	/// <p>
	/// Halton sequences are known to suffer from linear correlation for larger prime numbers, thus the individual digits
	/// are usually scrambled. This implementation already comes with support for up to 40 dimensions with optimal weight
	/// numbers from <a href="http://etd.lib.fsu.edu/theses/available/etd-07062004-140409/unrestricted/dissertation1.pdf">
	/// H. Chi: Scrambled quasirandom sequences and their applications</a>.
	/// <p>
	/// The generator supports two modes:
	/// <ul>
	///   <li>sequential generation of points: <seealso cref="#nextVector()"/></li>
	///   <li>random access to the i-th point in the sequence: <seealso cref="#skipTo(int)"/></li>
	/// </ul>
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Halton_sequence">Halton sequence (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="https://lirias.kuleuven.be/bitstream/123456789/131168/1/mcm2005_bartv.pdf">
	/// On the Halton sequence and its scramblings</a>
	/// @version $Id: HaltonSequenceGenerator.java 1512043 2013-08-08 21:27:57Z tn $
	/// @since 3.3 </seealso>
	public class HaltonSequenceGenerator : RandomVectorGenerator
	{

		/// <summary>
		/// The first 40 primes. </summary>
		private static readonly int[] PRIMES = new int[] {2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173};

		/// <summary>
		/// The optimal weights used for scrambling of the first 40 dimension. </summary>
		private static readonly int[] WEIGHTS = new int[] {1, 2, 3, 3, 8, 11, 12, 14, 7, 18, 12, 13, 17, 18, 29, 14, 18, 43, 41, 44, 40, 30, 47, 65, 71, 28, 40, 60, 79, 89, 56, 50, 52, 61, 108, 56, 66, 63, 60, 66};

		/// <summary>
		/// Space dimension. </summary>
		private readonly int dimension;

		/// <summary>
		/// The current index in the sequence. </summary>
		private int count = 0;

		/// <summary>
		/// The base numbers for each component. </summary>
		private readonly int[] @base;

		/// <summary>
		/// The scrambling weights for each component. </summary>
		private readonly int[] weight;

		/// <summary>
		/// Construct a new Halton sequence generator for the given space dimension.
		/// </summary>
		/// <param name="dimension"> the space dimension </param>
		/// <exception cref="OutOfRangeException"> if the space dimension is outside the allowed range of [1, 40] </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public HaltonSequenceGenerator(final int dimension) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public HaltonSequenceGenerator(int dimension) : this(dimension, PRIMES, WEIGHTS)
		{
		}

		/// <summary>
		/// Construct a new Halton sequence generator with the given base numbers and weights for each dimension.
		/// The length of the bases array defines the space dimension and is required to be &gt; 0.
		/// </summary>
		/// <param name="dimension"> the space dimension </param>
		/// <param name="bases"> the base number for each dimension, entries should be (pairwise) prime, may not be null </param>
		/// <param name="weights"> the weights used during scrambling, may be null in which case no scrambling will be performed </param>
		/// <exception cref="NullArgumentException"> if base is null </exception>
		/// <exception cref="OutOfRangeException"> if the space dimension is outside the range [1, len], where
		///   len refers to the length of the bases array </exception>
		/// <exception cref="DimensionMismatchException"> if weights is non-null and the length of the input arrays differ </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public HaltonSequenceGenerator(final int dimension, final int[] bases, final int[] weights) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public HaltonSequenceGenerator(int dimension, int[] bases, int[] weights)
		{

			MathUtils.checkNotNull(bases);

			if (dimension < 1 || dimension > bases.Length)
			{
				throw new OutOfRangeException(dimension, 1, PRIMES.Length);
			}

			if (weights != null && weights.Length != bases.Length)
			{
				throw new DimensionMismatchException(weights.Length, bases.Length);
			}

			this.dimension = dimension;
			this.@base = bases.clone();
			this.weight = weights == null ? null : weights.clone();
			count = 0;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double[] nextVector()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] v = new double[dimension];
			double[] v = new double[dimension];
			for (int i = 0; i < dimension; i++)
			{
				int index = count;
				double f = 1.0 / @base[i];

				int j = 0;
				while (index > 0)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int digit = scramble(i, j, base[i], index % base[i]);
					int digit = scramble(i, j, @base[i], index % @base[i]);
					v[i] += f * digit;
					index /= @base[i]; // floor( index / base )
					f /= @base[i];
				}
			}
			count++;
			return v;
		}

		/// <summary>
		/// Performs scrambling of digit {@code d_j} according to the formula:
		/// <pre>
		///   ( weight_i * d_j ) mod base
		/// </pre>
		/// Implementations can override this method to do a different scrambling.
		/// </summary>
		/// <param name="i"> the dimension index </param>
		/// <param name="j"> the digit index </param>
		/// <param name="b"> the base for this dimension </param>
		/// <param name="digit"> the j-th digit </param>
		/// <returns> the scrambled digit </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected int scramble(final int i, final int j, final int b, final int digit)
		protected internal virtual int scramble(int i, int j, int b, int digit)
		{
			return weight != null ? (weight[i] * digit) % b : digit;
		}

		/// <summary>
		/// Skip to the i-th point in the Halton sequence.
		/// <p>
		/// This operation can be performed in O(1).
		/// </summary>
		/// <param name="index"> the index in the sequence to skip to </param>
		/// <returns> the i-th point in the Halton sequence </returns>
		/// <exception cref="NotPositiveException"> if index &lt; 0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] skipTo(final int index) throws org.apache.commons.math3.exception.NotPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double[] skipTo(int index)
		{
			count = index;
			return nextVector();
		}

		/// <summary>
		/// Returns the index i of the next point in the Halton sequence that will be returned
		/// by calling <seealso cref="#nextVector()"/>.
		/// </summary>
		/// <returns> the index of the next point </returns>
		public virtual int NextIndex
		{
			get
			{
				return count;
			}
		}

	}

}
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
namespace mathlib.util
{

    using MathInternalError = mathlib.exception.MathInternalError;
    using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
    using OutOfRangeException = mathlib.exception.OutOfRangeException;

	/// <summary>
	/// Utility to create <a href="http://en.wikipedia.org/wiki/Combination">
	/// combinations</a> {@code (n, k)} of {@code k} elements in a set of
	/// {@code n} elements.
	/// 
	/// @version $Id: Combinations.java 1591835 2014-05-02 09:04:01Z tn $
	/// @since 3.3
	/// </summary>
	public class Combinations : IEnumerable<int[]>
	{
		/// <summary>
		/// Size of the set from which combinations are drawn. </summary>
		private readonly int n;
		/// <summary>
		/// Number of elements in each combination. </summary>
		private readonly int k;
		/// <summary>
		/// Iteration order. </summary>
		private readonly IterationOrder iterationOrder;

		/// <summary>
		/// Describes the type of iteration performed by the
		/// <seealso cref="#iterator() iterator"/>.
		/// </summary>
		private enum IterationOrder
		{
			/// <summary>
			/// Lexicographic order. </summary>
			LEXICOGRAPHIC
		}

	   /// <summary>
	   /// Creates an instance whose range is the k-element subsets of
	   /// {0, ..., n - 1} represented as {@code int[]} arrays.
	   /// <p>
	   /// The iteration order is lexicographic: the arrays returned by the
	   /// <seealso cref="#iterator() iterator"/> are sorted in descending order and
	   /// they are visited in lexicographic order with significance from
	   /// right to left.
	   /// For example, {@code new Combinations(4, 2).iterator()} returns
	   /// an iterator that will generate the following sequence of arrays
	   /// on successive calls to
	   /// {@code next()}:<br/>
	   /// {@code [0, 1], [0, 2], [1, 2], [0, 3], [1, 3], [2, 3]}
	   /// </p>
	   /// If {@code k == 0} an iterator containing an empty array is returned;
	   /// if {@code k == n} an iterator containing [0, ..., n - 1] is returned.
	   /// </summary>
	   /// <param name="n"> Size of the set from which subsets are selected. </param>
	   /// <param name="k"> Size of the subsets to be enumerated. </param>
	   /// <exception cref="org.apache.commons.math3.exception.NotPositiveException"> if {@code n < 0}. </exception>
	   /// <exception cref="org.apache.commons.math3.exception.NumberIsTooLargeException"> if {@code k > n}. </exception>
		public Combinations(int n, int k) : this(n, k, IterationOrder.LEXICOGRAPHIC)
		{
		}

		/// <summary>
		/// Creates an instance whose range is the k-element subsets of
		/// {0, ..., n - 1} represented as {@code int[]} arrays.
		/// <p>
		/// If the {@code iterationOrder} argument is set to
		/// <seealso cref="IterationOrder#LEXICOGRAPHIC"/>, the arrays returned by the
		/// <seealso cref="#iterator() iterator"/> are sorted in descending order and
		/// they are visited in lexicographic order with significance from
		/// right to left.
		/// For example, {@code new Combinations(4, 2).iterator()} returns
		/// an iterator that will generate the following sequence of arrays
		/// on successive calls to
		/// {@code next()}:<br/>
		/// {@code [0, 1], [0, 2], [1, 2], [0, 3], [1, 3], [2, 3]}
		/// </p>
		/// If {@code k == 0} an iterator containing an empty array is returned;
		/// if {@code k == n} an iterator containing [0, ..., n - 1] is returned.
		/// </summary>
		/// <param name="n"> Size of the set from which subsets are selected. </param>
		/// <param name="k"> Size of the subsets to be enumerated. </param>
		/// <param name="iterationOrder"> Specifies the <seealso cref="#iterator() iteration order"/>. </param>
		/// <exception cref="org.apache.commons.math3.exception.NotPositiveException"> if {@code n < 0}. </exception>
		/// <exception cref="org.apache.commons.math3.exception.NumberIsTooLargeException"> if {@code k > n}. </exception>
		private Combinations(int n, int k, IterationOrder iterationOrder)
		{
			CombinatoricsUtils.checkBinomial(n, k);
			this.n = n;
			this.k = k;
			this.iterationOrder = iterationOrder;
		}

		/// <summary>
		/// Gets the size of the set from which combinations are drawn.
		/// </summary>
		/// <returns> the size of the universe. </returns>
		public virtual int N
		{
			get
			{
				return n;
			}
		}

		/// <summary>
		/// Gets the number of elements in each combination.
		/// </summary>
		/// <returns> the size of the subsets to be enumerated. </returns>
		public virtual int K
		{
			get
			{
				return k;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual IEnumerator<int[]> GetEnumerator()
		{
			if (k == 0 || k == n)
			{
				return new SingletonIterator(MathArrays.natural(k));
			}

			switch (iterationOrder)
			{
			case org.apache.commons.math3.util.Combinations.IterationOrder.LEXICOGRAPHIC:
				return new LexicographicIterator(n, k);
			default:
				throw new MathInternalError(); // Should never happen.
			}
		}

		/// <summary>
		/// Defines a lexicographic ordering of combinations.
		/// The returned comparator allows to compare any two combinations
		/// that can be produced by this instance's <seealso cref="#iterator() iterator"/>.
		/// Its {@code compare(int[],int[])} method will throw exceptions if
		/// passed combinations that are inconsistent with this instance:
		/// <ul>
		///  <li>{@code DimensionMismatchException} if the array lengths are not
		///      equal to {@code k},</li>
		///  <li>{@code OutOfRangeException} if an element of the array is not
		///      within the interval [0, {@code n}).</li>
		/// </ul> </summary>
		/// <returns> a lexicographic comparator. </returns>
		public virtual IComparer<int[]> comparator()
		{
			return new LexicographicComparator(n, k);
		}

		/// <summary>
		/// Lexicographic combinations iterator.
		/// <p>
		/// Implementation follows Algorithm T in <i>The Art of Computer Programming</i>
		/// Internet Draft (PRE-FASCICLE 3A), "A Draft of Section 7.2.1.3 Generating All
		/// Combinations</a>, D. Knuth, 2004.</p>
		/// <p>
		/// The degenerate cases {@code k == 0} and {@code k == n} are NOT handled by this
		/// implementation.  If constructor arguments satisfy {@code k == 0}
		/// or {@code k >= n}, no exception is generated, but the iterator is empty.
		/// </p>
		/// 
		/// </summary>
		private class LexicographicIterator : IEnumerator<int[]>
		{
			/// <summary>
			/// Size of subsets returned by the iterator </summary>
			internal readonly int k;

			/// <summary>
			/// c[1], ..., c[k] stores the next combination; c[k + 1], c[k + 2] are
			/// sentinels.
			/// <p>
			/// Note that c[0] is "wasted" but this makes it a little easier to
			/// follow the code.
			/// </p>
			/// </summary>
			internal readonly int[] c;

			/// <summary>
			/// Return value for <seealso cref="#hasNext()"/> </summary>
			internal bool more = true;

			/// <summary>
			/// Marker: smallest index such that c[j + 1] > j </summary>
			internal int j;

			/// <summary>
			/// Construct a CombinationIterator to enumerate k-sets from n.
			/// <p>
			/// NOTE: If {@code k === 0} or {@code k >= n}, the Iterator will be empty
			/// (that is, <seealso cref="#hasNext()"/> will return {@code false} immediately.
			/// </p>
			/// </summary>
			/// <param name="n"> size of the set from which subsets are enumerated </param>
			/// <param name="k"> size of the subsets to enumerate </param>
			public LexicographicIterator(int n, int k)
			{
				this.k = k;
				c = new int[k + 3];
				if (k == 0 || k >= n)
				{
					more = false;
					return;
				}
				// Initialize c to start with lexicographically first k-set
				for (int i = 1; i <= k; i++)
				{
					c[i] = i - 1;
				}
				// Initialize sentinels
				c[k + 1] = n;
				c[k + 2] = 0;
				j = k; // Set up invariant: j is smallest index such that c[j + 1] > j
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			public virtual bool hasNext()
			{
				return more;
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			public virtual int[] next()
			{
				if (!more)
				{
					throw new NoSuchElementException();
				}
				// Copy return value (prepared by last activation)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] ret = new int[k];
				int[] ret = new int[k];
				Array.Copy(c, 1, ret, 0, k);

				// Prepare next iteration
				// T2 and T6 loop
				int x = 0;
				if (j > 0)
				{
					x = j;
					c[j] = x;
					j--;
					return ret;
				}
				// T3
				if (c[1] + 1 < c[2])
				{
					c[1]++;
					return ret;
				}
				else
				{
					j = 2;
				}
				// T4
				bool stepDone = false;
				while (!stepDone)
				{
					c[j - 1] = j - 2;
					x = c[j] + 1;
					if (x == c[j + 1])
					{
						j++;
					}
					else
					{
						stepDone = true;
					}
				}
				// T5
				if (j > k)
				{
					more = false;
					return ret;
				}
				// T6
				c[j] = x;
				j--;
				return ret;
			}

			/// <summary>
			/// Not supported.
			/// </summary>
			public virtual void remove()
			{
				throw new System.NotSupportedException();
			}
		}

		/// <summary>
		/// Iterator with just one element to handle degenerate cases (full array,
		/// empty array) for combination iterator.
		/// </summary>
		private class SingletonIterator : IEnumerator<int[]>
		{
			/// <summary>
			/// Singleton array </summary>
			internal readonly int[] singleton;
			/// <summary>
			/// True on initialization, false after first call to next </summary>
			internal bool more = true;
			/// <summary>
			/// Create a singleton iterator providing the given array. </summary>
			/// <param name="singleton"> array returned by the iterator </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SingletonIterator(final int[] singleton)
			public SingletonIterator(int[] singleton)
			{
				this.singleton = singleton;
			}
			/// <returns> True until next is called the first time, then false </returns>
			public virtual bool hasNext()
			{
				return more;
			}
			/// <returns> the singleton in first activation; throws NSEE thereafter </returns>
			public virtual int[] next()
			{
				if (more)
				{
					more = false;
					return singleton;
				}
				else
				{
					throw new NoSuchElementException();
				}
			}
			/// <summary>
			/// Not supported </summary>
			public virtual void remove()
			{
				throw new System.NotSupportedException();
			}
		}

		/// <summary>
		/// Defines the lexicographic ordering of combinations, using
		/// the <seealso cref="#lexNorm(int[])"/> method.
		/// </summary>
		[Serializable]
		private class LexicographicComparator : IComparer<int[]>
		{
			/// <summary>
			/// Serializable version identifier. </summary>
			internal const long serialVersionUID = 20130906L;
			/// <summary>
			/// Size of the set from which combinations are drawn. </summary>
			internal readonly int n;
			/// <summary>
			/// Number of elements in each combination. </summary>
			internal readonly int k;

			/// <param name="n"> Size of the set from which subsets are selected. </param>
			/// <param name="k"> Size of the subsets to be enumerated. </param>
			public LexicographicComparator(int n, int k)
			{
				this.n = n;
				this.k = k;
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			/// <exception cref="DimensionMismatchException"> if the array lengths are not
			/// equal to {@code k}. </exception>
			/// <exception cref="OutOfRangeException"> if an element of the array is not
			/// within the interval [0, {@code n}). </exception>
			public virtual int Compare(int[] c1, int[] c2)
			{
				if (c1.Length != k)
				{
					throw new DimensionMismatchException(c1.Length, k);
				}
				if (c2.Length != k)
				{
					throw new DimensionMismatchException(c2.Length, k);
				}

				// Method "lexNorm" works with ordered arrays.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] c1s = MathArrays.copyOf(c1);
				int[] c1s = MathArrays.copyOf(c1);
				Arrays.sort(c1s);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] c2s = MathArrays.copyOf(c2);
				int[] c2s = MathArrays.copyOf(c2);
				Arrays.sort(c2s);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long v1 = lexNorm(c1s);
				long v1 = lexNorm(c1s);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long v2 = lexNorm(c2s);
				long v2 = lexNorm(c2s);

				if (v1 < v2)
				{
					return -1;
				}
				else if (v1 > v2)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}

			/// <summary>
			/// Computes the value (in base 10) represented by the digit
			/// (interpreted in base {@code n}) in the input array in reverse
			/// order.
			/// For example if {@code c} is {@code {3, 2, 1}}, and {@code n}
			/// is 3, the method will return 18.
			/// </summary>
			/// <param name="c"> Input array. </param>
			/// <returns> the lexicographic norm. </returns>
			/// <exception cref="OutOfRangeException"> if an element of the array is not
			/// within the interval [0, {@code n}). </exception>
			internal virtual long lexNorm(int[] c)
			{
				long ret = 0;
				for (int i = 0; i < c.Length; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int digit = c[i];
					int digit = c[i];
					if (digit < 0 || digit >= n)
					{
						throw new OutOfRangeException(digit, 0, n - 1);
					}

					ret += c[i] * ArithmeticUtils.pow(n, i);
				}
				return ret;
			}
		}
	}

}
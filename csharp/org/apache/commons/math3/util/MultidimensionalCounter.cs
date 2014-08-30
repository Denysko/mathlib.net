using System.Text;

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

namespace org.apache.commons.math3.util
{

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;

	/// <summary>
	/// Converter between unidimensional storage structure and multidimensional
	/// conceptual structure.
	/// This utility will convert from indices in a multidimensional structure
	/// to the corresponding index in a one-dimensional array. For example,
	/// assuming that the ranges (in 3 dimensions) of indices are 2, 4 and 3,
	/// the following correspondences, between 3-tuples indices and unidimensional
	/// indices, will hold:
	/// <ul>
	///  <li>(0, 0, 0) corresponds to 0</li>
	///  <li>(0, 0, 1) corresponds to 1</li>
	///  <li>(0, 0, 2) corresponds to 2</li>
	///  <li>(0, 1, 0) corresponds to 3</li>
	///  <li>...</li>
	///  <li>(1, 0, 0) corresponds to 12</li>
	///  <li>...</li>
	///  <li>(1, 3, 2) corresponds to 23</li>
	/// </ul>
	/// 
	/// @since 2.2
	/// @version $Id: MultidimensionalCounter.java 1558833 2014-01-16 15:26:29Z erans $
	/// </summary>
	public class MultidimensionalCounter : IEnumerable<int?>
	{
		/// <summary>
		/// Number of dimensions.
		/// </summary>
		private readonly int dimension;
		/// <summary>
		/// Offset for each dimension.
		/// </summary>
		private readonly int[] uniCounterOffset;
		/// <summary>
		/// Counter sizes.
		/// </summary>
		private readonly int[] size;
		/// <summary>
		/// Total number of (one-dimensional) slots.
		/// </summary>
		private readonly int totalSize;
		/// <summary>
		/// Index of last dimension.
		/// </summary>
		private readonly int last;

		/// <summary>
		/// Perform iteration over the multidimensional counter.
		/// </summary>
		public class Iterator : IEnumerator<int?>
		{
			internal bool InstanceFieldsInitialized = false;

			internal virtual void InitializeInstanceFields()
			{
				counter = new int[outerInstance.dimension];
				maxCount = outerInstance.totalSize - 1;
			}

			private readonly MultidimensionalCounter outerInstance;

			/// <summary>
			/// Multidimensional counter.
			/// </summary>
			internal int[] counter;
			/// <summary>
			/// Unidimensional counter.
			/// </summary>
			internal int count = -1;
			/// <summary>
			/// Maximum value for <seealso cref="#count"/>.
			/// </summary>
			internal int maxCount;

			/// <summary>
			/// Create an iterator </summary>
			/// <seealso cref= #iterator() </seealso>
			internal Iterator(MultidimensionalCounter outerInstance)
			{
				this.outerInstance = outerInstance;

				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = true;
				}
				counter[outerInstance.last] = -1;
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			public virtual bool hasNext()
			{
				return count < maxCount;
			}

			/// <returns> the unidimensional count after the counter has been
			/// incremented by {@code 1}. </returns>
			/// <exception cref="NoSuchElementException"> if <seealso cref="#hasNext()"/> would have
			/// returned {@code false}. </exception>
			public virtual int? next()
			{
				if (!hasNext())
				{
					throw new NoSuchElementException();
				}

				for (int i = outerInstance.last; i >= 0; i--)
				{
					if (counter[i] == outerInstance.size[i] - 1)
					{
						counter[i] = 0;
					}
					else
					{
						++counter[i];
						break;
					}
				}

				return ++count;
			}

			/// <summary>
			/// Get the current unidimensional counter slot.
			/// </summary>
			/// <returns> the index within the unidimensionl counter. </returns>
			public virtual int Count
			{
				get
				{
					return count;
				}
			}
			/// <summary>
			/// Get the current multidimensional counter slots.
			/// </summary>
			/// <returns> the indices within the multidimensional counter. </returns>
			public virtual int[] Counts
			{
				get
				{
					return MathArrays.copyOf(counter);
				}
			}

			/// <summary>
			/// Get the current count in the selected dimension.
			/// </summary>
			/// <param name="dim"> Dimension index. </param>
			/// <returns> the count at the corresponding index for the current state
			/// of the iterator. </returns>
			/// <exception cref="IndexOutOfBoundsException"> if {@code index} is not in the
			/// correct interval (as defined by the length of the argument in the
			/// {@link MultidimensionalCounter#MultidimensionalCounter(int[])
			/// constructor of the enclosing class}). </exception>
			public virtual int getCount(int dim)
			{
				return counter[dim];
			}

			/// <exception cref="UnsupportedOperationException"> </exception>
			public virtual void remove()
			{
				throw new System.NotSupportedException();
			}
		}

		/// <summary>
		/// Create a counter.
		/// </summary>
		/// <param name="size"> Counter sizes (number of slots in each dimension). </param>
		/// <exception cref="NotStrictlyPositiveException"> if one of the sizes is
		/// negative or zero. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MultidimensionalCounter(int... size) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public MultidimensionalCounter(params int[] size)
		{
			dimension = size.Length;
			this.size = MathArrays.copyOf(size);

			uniCounterOffset = new int[dimension];

			last = dimension - 1;
			int tS = size[last];
			for (int i = 0; i < last; i++)
			{
				int count = 1;
				for (int j = i + 1; j < dimension; j++)
				{
					count *= size[j];
				}
				uniCounterOffset[i] = count;
				tS *= size[i];
			}
			uniCounterOffset[last] = 0;

			if (tS <= 0)
			{
				throw new NotStrictlyPositiveException(tS);
			}

			totalSize = tS;
		}

		/// <summary>
		/// Create an iterator over this counter.
		/// </summary>
		/// <returns> the iterator. </returns>
		public virtual Iterator iterator()
		{
			return new Iterator(this);
		}

		/// <summary>
		/// Get the number of dimensions of the multidimensional counter.
		/// </summary>
		/// <returns> the number of dimensions. </returns>
		public virtual int Dimension
		{
			get
			{
				return dimension;
			}
		}

		/// <summary>
		/// Convert to multidimensional counter.
		/// </summary>
		/// <param name="index"> Index in unidimensional counter. </param>
		/// <returns> the multidimensional counts. </returns>
		/// <exception cref="OutOfRangeException"> if {@code index} is not between
		/// {@code 0} and the value returned by <seealso cref="#getSize()"/> (excluded). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int[] getCounts(int index) throws org.apache.commons.math3.exception.OutOfRangeException
		public virtual int[] getCounts(int index)
		{
			if (index < 0 || index >= totalSize)
			{
				throw new OutOfRangeException(index, 0, totalSize);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] indices = new int[dimension];
			int[] indices = new int[dimension];

			int count = 0;
			for (int i = 0; i < last; i++)
			{
				int idx = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = uniCounterOffset[i];
				int offset = uniCounterOffset[i];
				while (count <= index)
				{
					count += offset;
					++idx;
				}
				--idx;
				count -= offset;
				indices[i] = idx;
			}

			indices[last] = index - count;

			return indices;
		}

		/// <summary>
		/// Convert to unidimensional counter.
		/// </summary>
		/// <param name="c"> Indices in multidimensional counter. </param>
		/// <returns> the index within the unidimensionl counter. </returns>
		/// <exception cref="DimensionMismatchException"> if the size of {@code c}
		/// does not match the size of the array given in the constructor. </exception>
		/// <exception cref="OutOfRangeException"> if a value of {@code c} is not in
		/// the range of the corresponding dimension, as defined in the
		/// <seealso cref="MultidimensionalCounter#MultidimensionalCounter(int...) constructor"/>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getCount(int... c) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.DimensionMismatchException
		public virtual int getCount(params int[] c)
		{
			if (c.Length != dimension)
			{
				throw new DimensionMismatchException(c.Length, dimension);
			}
			int count = 0;
			for (int i = 0; i < dimension; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = c[i];
				int index = c[i];
				if (index < 0 || index >= size[i])
				{
					throw new OutOfRangeException(index, 0, size[i] - 1);
				}
				count += uniCounterOffset[i] * c[i];
			}
			return count + c[last];
		}

		/// <summary>
		/// Get the total number of elements.
		/// </summary>
		/// <returns> the total size of the unidimensional counter. </returns>
		public virtual int Size
		{
			get
			{
				return totalSize;
			}
		}
		/// <summary>
		/// Get the number of multidimensional counter slots in each dimension.
		/// </summary>
		/// <returns> the sizes of the multidimensional counter in each dimension. </returns>
		public virtual int[] Sizes
		{
			get
			{
				return MathArrays.copyOf(size);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < dimension; i++)
			{
				sb.Append("[").Append(getCount(i)).Append("]");
			}
			return sb.ToString();
		}
	}

}
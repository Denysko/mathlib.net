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
namespace mathlib.linear
{

	using mathlib;
	using mathlib;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MathArithmeticException = mathlib.exception.MathArithmeticException;
	using NotPositiveException = mathlib.exception.NotPositiveException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using MathArrays = mathlib.util.MathArrays;
	using MathUtils = mathlib.util.MathUtils;
	using mathlib.util;

	/// <summary>
	/// This class implements the <seealso cref="FieldVector"/> interface with a <seealso cref="OpenIntToFieldHashMap"/> backing store.
	/// <p>
	///  Caveat: This implementation assumes that, for any {@code x},
	///  the equality {@code x * 0d == 0d} holds. But it is is not true for
	///  {@code NaN}. Moreover, zero entries will lose their sign.
	///  Some operations (that involve {@code NaN} and/or infinities) may
	///  thus give incorrect results.
	/// </p> </summary>
	/// @param <T> the type of the field elements
	/// @version $Id: SparseFieldVector.java 1570536 2014-02-21 11:26:09Z luc $
	/// @since 2.0 </param>
	[Serializable]
	public class SparseFieldVector<T> : FieldVector<T> where T : mathlib.FieldElement<T>
	{
		/// <summary>
		///  Serialization identifier. </summary>
		private const long serialVersionUID = 7841233292190413362L;
		/// <summary>
		/// Field to which the elements belong. </summary>
		private readonly Field<T> field;
		/// <summary>
		/// Entries of the vector. </summary>
		private readonly OpenIntToFieldHashMap<T> entries;
		/// <summary>
		/// Dimension of the vector. </summary>
		private readonly int virtualSize;

		/// <summary>
		/// Build a 0-length vector.
		/// Zero-length vectors may be used to initialize construction of vectors
		/// by data gathering. We start with zero-length and use either the {@link
		/// #SparseFieldVector(SparseFieldVector, int)} constructor
		/// or one of the {@code append} method (<seealso cref="#append(FieldVector)"/> or
		/// <seealso cref="#append(SparseFieldVector)"/>) to gather data into this vector.
		/// </summary>
		/// <param name="field"> Field to which the elements belong. </param>
		public SparseFieldVector(Field<T> field) : this(field, 0)
		{
		}


		/// <summary>
		/// Construct a vector of zeroes.
		/// </summary>
		/// <param name="field"> Field to which the elements belong. </param>
		/// <param name="dimension"> Size of the vector. </param>
		public SparseFieldVector(Field<T> field, int dimension)
		{
			this.field = field;
			virtualSize = dimension;
			entries = new OpenIntToFieldHashMap<T>(field);
		}

		/// <summary>
		/// Build a resized vector, for use with append.
		/// </summary>
		/// <param name="v"> Original vector </param>
		/// <param name="resize"> Amount to add. </param>
		protected internal SparseFieldVector(SparseFieldVector<T> v, int resize)
		{
			field = v.field;
			virtualSize = v.Dimension + resize;
			entries = new OpenIntToFieldHashMap<T>(v.entries);
		}


		/// <summary>
		/// Build a vector with known the sparseness (for advanced use only).
		/// </summary>
		/// <param name="field"> Field to which the elements belong. </param>
		/// <param name="dimension"> Size of the vector. </param>
		/// <param name="expectedSize"> Expected number of non-zero entries. </param>
		public SparseFieldVector(Field<T> field, int dimension, int expectedSize)
		{
			this.field = field;
			virtualSize = dimension;
			entries = new OpenIntToFieldHashMap<T>(field,expectedSize);
		}

		/// <summary>
		/// Create from a Field array.
		/// Only non-zero entries will be stored.
		/// </summary>
		/// <param name="field"> Field to which the elements belong. </param>
		/// <param name="values"> Set of values to create from. </param>
		/// <exception cref="NullArgumentException"> if values is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SparseFieldVector(mathlib.Field<T> field, T[] values) throws mathlib.exception.NullArgumentException
		public SparseFieldVector(Field<T> field, T[] values)
		{
			MathUtils.checkNotNull(values);
			this.field = field;
			virtualSize = values.Length;
			entries = new OpenIntToFieldHashMap<T>(field);
			for (int key = 0; key < values.Length; key++)
			{
				T value = values[key];
				entries.put(key, value);
			}
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="v"> Instance to copy. </param>
		public SparseFieldVector(SparseFieldVector<T> v)
		{
			field = v.field;
			virtualSize = v.Dimension;
			entries = new OpenIntToFieldHashMap<T>(v.Entries);
		}

		/// <summary>
		/// Get the entries of this instance.
		/// </summary>
		/// <returns> the entries of this instance </returns>
		private OpenIntToFieldHashMap<T> Entries
		{
			get
			{
				return entries;
			}
		}

		/// <summary>
		/// Optimized method to add sparse vectors.
		/// </summary>
		/// <param name="v"> Vector to add. </param>
		/// <returns> {@code this + v}. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> add(SparseFieldVector<T> v) throws mathlib.exception.DimensionMismatchException
		public virtual FieldVector<T> add(SparseFieldVector<T> v)
		{
			checkVectorDimensions(v.Dimension);
			SparseFieldVector<T> res = (SparseFieldVector<T>)copy();
			OpenIntToFieldHashMap<T>.Iterator iter = v.Entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				int key = iter.key();
				T value = iter.value();
				if (entries.containsKey(key))
				{
					res.setEntry(key, entries.get(key).add(value));
				}
				else
				{
					res.setEntry(key, value);
				}
			}
			return res;

		}

		/// <summary>
		/// Construct a vector by appending a vector to this vector.
		/// </summary>
		/// <param name="v"> Vector to append to this one. </param>
		/// <returns> a new vector. </returns>
		public virtual FieldVector<T> append(SparseFieldVector<T> v)
		{
			SparseFieldVector<T> res = new SparseFieldVector<T>(this, v.Dimension);
			OpenIntToFieldHashMap<T>.Iterator iter = v.entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				res.setEntry(iter.key() + virtualSize, iter.value());
			}
			return res;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual FieldVector<T> append(FieldVector<T> v)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (v instanceof SparseFieldVector<?>)
			if (v is SparseFieldVector<?>)
			{
				return append((SparseFieldVector<T>) v);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = v.getDimension();
				int n = v.Dimension;
				FieldVector<T> res = new SparseFieldVector<T>(this, n);
				for (int i = 0; i < n; i++)
				{
					res.setEntry(i + virtualSize, v.getEntry(i));
				}
				return res;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="NullArgumentException"> if d is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> append(T d) throws mathlib.exception.NullArgumentException
		public virtual FieldVector<T> append(T d)
		{
			MathUtils.checkNotNull(d);
			FieldVector<T> res = new SparseFieldVector<T>(this, 1);
			res.setEntry(virtualSize, d);
			return res;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual FieldVector<T> copy()
		{
			return new SparseFieldVector<T>(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T dotProduct(FieldVector<T> v) throws mathlib.exception.DimensionMismatchException
		public virtual T dotProduct(FieldVector<T> v)
		{
			checkVectorDimensions(v.Dimension);
			T res = field.Zero;
			OpenIntToFieldHashMap<T>.Iterator iter = entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				res = res.add(v.getEntry(iter.key()).multiply(iter.value()));
			}
			return res;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> ebeDivide(FieldVector<T> v) throws mathlib.exception.DimensionMismatchException, mathlib.exception.MathArithmeticException
		public virtual FieldVector<T> ebeDivide(FieldVector<T> v)
		{
			checkVectorDimensions(v.Dimension);
			SparseFieldVector<T> res = new SparseFieldVector<T>(this);
			OpenIntToFieldHashMap<T>.Iterator iter = res.entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				res.setEntry(iter.key(), iter.value().divide(v.getEntry(iter.key())));
			}
			return res;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> ebeMultiply(FieldVector<T> v) throws mathlib.exception.DimensionMismatchException
		public virtual FieldVector<T> ebeMultiply(FieldVector<T> v)
		{
			checkVectorDimensions(v.Dimension);
			SparseFieldVector<T> res = new SparseFieldVector<T>(this);
			OpenIntToFieldHashMap<T>.Iterator iter = res.entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				res.setEntry(iter.key(), iter.value().multiply(v.getEntry(iter.key())));
			}
			return res;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// @deprecated as of 3.1, to be removed in 4.0. Please use the <seealso cref="#toArray()"/> method instead. 
		[Obsolete("as of 3.1, to be removed in 4.0. Please use the <seealso cref="#toArray()"/> method instead.")]
		public virtual T[] Data
		{
			get
			{
				return toArray();
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int Dimension
		{
			get
			{
				return virtualSize;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T getEntry(int index) throws mathlib.exception.OutOfRangeException
		public virtual T getEntry(int index)
		{
			checkIndex(index);
			return entries.get(index);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Field<T> Field
		{
			get
			{
				return field;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> getSubVector(int index, int n) throws mathlib.exception.OutOfRangeException, mathlib.exception.NotPositiveException
		public virtual FieldVector<T> getSubVector(int index, int n)
		{
			if (n < 0)
			{
				throw new NotPositiveException(LocalizedFormats.NUMBER_OF_ELEMENTS_SHOULD_BE_POSITIVE, n);
			}
			checkIndex(index);
			checkIndex(index + n - 1);
			SparseFieldVector<T> res = new SparseFieldVector<T>(field,n);
			int end = index + n;
			OpenIntToFieldHashMap<T>.Iterator iter = entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				int key = iter.key();
				if (key >= index && key < end)
				{
					res.setEntry(key - index, iter.value());
				}
			}
			return res;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapAdd(T d) throws mathlib.exception.NullArgumentException
		public virtual FieldVector<T> mapAdd(T d)
		{
			return copy().mapAddToSelf(d);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapAddToSelf(T d) throws mathlib.exception.NullArgumentException
		public virtual FieldVector<T> mapAddToSelf(T d)
		{
			for (int i = 0; i < virtualSize; i++)
			{
				setEntry(i, getEntry(i).add(d));
			}
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapDivide(T d) throws mathlib.exception.NullArgumentException, mathlib.exception.MathArithmeticException
		public virtual FieldVector<T> mapDivide(T d)
		{
			return copy().mapDivideToSelf(d);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapDivideToSelf(T d) throws mathlib.exception.NullArgumentException, mathlib.exception.MathArithmeticException
		public virtual FieldVector<T> mapDivideToSelf(T d)
		{
			OpenIntToFieldHashMap<T>.Iterator iter = entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				entries.put(iter.key(), iter.value().divide(d));
			}
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapInv() throws mathlib.exception.MathArithmeticException
		public virtual FieldVector<T> mapInv()
		{
			return copy().mapInvToSelf();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapInvToSelf() throws mathlib.exception.MathArithmeticException
		public virtual FieldVector<T> mapInvToSelf()
		{
			for (int i = 0; i < virtualSize; i++)
			{
				setEntry(i, field.One.divide(getEntry(i)));
			}
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapMultiply(T d) throws mathlib.exception.NullArgumentException
		public virtual FieldVector<T> mapMultiply(T d)
		{
			return copy().mapMultiplyToSelf(d);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapMultiplyToSelf(T d) throws mathlib.exception.NullArgumentException
		public virtual FieldVector<T> mapMultiplyToSelf(T d)
		{
			OpenIntToFieldHashMap<T>.Iterator iter = entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				entries.put(iter.key(), iter.value().multiply(d));
			}
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapSubtract(T d) throws mathlib.exception.NullArgumentException
		public virtual FieldVector<T> mapSubtract(T d)
		{
			return copy().mapSubtractToSelf(d);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapSubtractToSelf(T d) throws mathlib.exception.NullArgumentException
		public virtual FieldVector<T> mapSubtractToSelf(T d)
		{
			return mapAddToSelf(field.Zero.subtract(d));
		}

		/// <summary>
		/// Optimized method to compute outer product when both vectors are sparse. </summary>
		/// <param name="v"> vector with which outer product should be computed </param>
		/// <returns> the matrix outer product between instance and v </returns>
		public virtual FieldMatrix<T> outerProduct(SparseFieldVector<T> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = v.getDimension();
			int n = v.Dimension;
			SparseFieldMatrix<T> res = new SparseFieldMatrix<T>(field, virtualSize, n);
			OpenIntToFieldHashMap<T>.Iterator iter = entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				OpenIntToFieldHashMap<T>.Iterator iter2 = v.entries.GetEnumerator();
				while (iter2.hasNext())
				{
					iter2.advance();
					res.setEntry(iter.key(), iter2.key(), iter.value().multiply(iter2.value()));
				}
			}
			return res;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual FieldMatrix<T> outerProduct(FieldVector<T> v)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (v instanceof SparseFieldVector<?>)
			if (v is SparseFieldVector<?>)
			{
				return outerProduct((SparseFieldVector<T>)v);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = v.getDimension();
				int n = v.Dimension;
				FieldMatrix<T> res = new SparseFieldMatrix<T>(field, virtualSize, n);
				OpenIntToFieldHashMap<T>.Iterator iter = entries.GetEnumerator();
				while (iter.hasNext())
				{
					iter.advance();
					int row = iter.key();
					FieldElement<T>value = iter.value();
					for (int col = 0; col < n; col++)
					{
						res.setEntry(row, col, value.multiply(v.getEntry(col)));
					}
				}
				return res;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> projection(FieldVector<T> v) throws mathlib.exception.DimensionMismatchException, mathlib.exception.MathArithmeticException
		public virtual FieldVector<T> projection(FieldVector<T> v)
		{
			checkVectorDimensions(v.Dimension);
			return v.mapMultiply(dotProduct(v).divide(v.dotProduct(v)));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="NullArgumentException"> if value is null </exception>
		public virtual void set(T value)
		{
			MathUtils.checkNotNull(value);
			for (int i = 0; i < virtualSize; i++)
			{
				setEntry(i, value);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="NullArgumentException"> if value is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setEntry(int index, T value) throws mathlib.exception.NullArgumentException, mathlib.exception.OutOfRangeException
		public virtual void setEntry(int index, T value)
		{
			MathUtils.checkNotNull(value);
			checkIndex(index);
			entries.put(index, value);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSubVector(int index, FieldVector<T> v) throws mathlib.exception.OutOfRangeException
		public virtual void setSubVector(int index, FieldVector<T> v)
		{
			checkIndex(index);
			checkIndex(index + v.Dimension - 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = v.getDimension();
			int n = v.Dimension;
			for (int i = 0; i < n; i++)
			{
				setEntry(i + index, v.getEntry(i));
			}
		}

		/// <summary>
		/// Optimized method to compute {@code this} minus {@code v}. </summary>
		/// <param name="v"> vector to be subtracted </param>
		/// <returns> {@code this - v} </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SparseFieldVector<T> subtract(SparseFieldVector<T> v) throws mathlib.exception.DimensionMismatchException
		public virtual SparseFieldVector<T> subtract(SparseFieldVector<T> v)
		{
			checkVectorDimensions(v.Dimension);
			SparseFieldVector<T> res = (SparseFieldVector<T>)copy();
			OpenIntToFieldHashMap<T>.Iterator iter = v.Entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				int key = iter.key();
				if (entries.containsKey(key))
				{
					res.setEntry(key, entries.get(key).subtract(iter.value()));
				}
				else
				{
					res.setEntry(key, field.Zero.subtract(iter.value()));
				}
			}
			return res;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> subtract(FieldVector<T> v) throws mathlib.exception.DimensionMismatchException
		public virtual FieldVector<T> subtract(FieldVector<T> v)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (v instanceof SparseFieldVector<?>)
			if (v is SparseFieldVector<?>)
			{
				return subtract((SparseFieldVector<T>)v);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = v.getDimension();
				int n = v.Dimension;
				checkVectorDimensions(n);
				SparseFieldVector<T> res = new SparseFieldVector<T>(this);
				for (int i = 0; i < n; i++)
				{
					if (entries.containsKey(i))
					{
						res.setEntry(i, entries.get(i).subtract(v.getEntry(i)));
					}
					else
					{
						res.setEntry(i, field.Zero.subtract(v.getEntry(i)));
					}
				}
				return res;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual T[] toArray()
		{
			T[] res = MathArrays.buildArray(field, virtualSize);
			OpenIntToFieldHashMap<T>.Iterator iter = entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				res[iter.key()] = iter.value();
			}
			return res;
		}

		/// <summary>
		/// Check whether an index is valid.
		/// </summary>
		/// <param name="index"> Index to check. </param>
		/// <exception cref="OutOfRangeException"> if the index is not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkIndex(final int index) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void checkIndex(int index)
		{
			if (index < 0 || index >= Dimension)
			{
				throw new OutOfRangeException(index, 0, Dimension - 1);
			}
		}

		/// <summary>
		/// Checks that the indices of a subvector are valid.
		/// </summary>
		/// <param name="start"> the index of the first entry of the subvector </param>
		/// <param name="end"> the index of the last entry of the subvector (inclusive) </param>
		/// <exception cref="OutOfRangeException"> if {@code start} of {@code end} are not valid </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code end < start}
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkIndices(final int start, final int end) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void checkIndices(int start, int end)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = getDimension();
			int dim = Dimension;
			if ((start < 0) || (start >= dim))
			{
				throw new OutOfRangeException(LocalizedFormats.INDEX, start, 0, dim - 1);
			}
			if ((end < 0) || (end >= dim))
			{
				throw new OutOfRangeException(LocalizedFormats.INDEX, end, 0, dim - 1);
			}
			if (end < start)
			{
				throw new NumberIsTooSmallException(LocalizedFormats.INITIAL_ROW_AFTER_FINAL_ROW, end, start, false);
			}
		}

		/// <summary>
		/// Check if instance dimension is equal to some expected value.
		/// </summary>
		/// <param name="n"> Expected dimension. </param>
		/// <exception cref="DimensionMismatchException"> if the dimensions do not match. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkVectorDimensions(int n) throws mathlib.exception.DimensionMismatchException
		protected internal virtual void checkVectorDimensions(int n)
		{
			if (Dimension != n)
			{
				throw new DimensionMismatchException(Dimension, n);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> add(FieldVector<T> v) throws mathlib.exception.DimensionMismatchException
		public virtual FieldVector<T> add(FieldVector<T> v)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (v instanceof SparseFieldVector<?>)
			if (v is SparseFieldVector<?>)
			{
				return add((SparseFieldVector<T>) v);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = v.getDimension();
				int n = v.Dimension;
				checkVectorDimensions(n);
				SparseFieldVector<T> res = new SparseFieldVector<T>(field, Dimension);
				for (int i = 0; i < n; i++)
				{
					res.setEntry(i, v.getEntry(i).add(getEntry(i)));
				}
				return res;
			}
		}

		/// <summary>
		/// Visits (but does not alter) all entries of this vector in default order
		/// (increasing index).
		/// </summary>
		/// <param name="visitor"> the visitor to be used to process the entries of this
		/// vector </param>
		/// <returns> the value returned by <seealso cref="FieldVectorPreservingVisitor#end()"/>
		/// at the end of the walk
		/// @since 3.3 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T walkInDefaultOrder(final FieldVectorPreservingVisitor<T> visitor)
		public virtual T walkInDefaultOrder(FieldVectorPreservingVisitor<T> visitor)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = getDimension();
			int dim = Dimension;
			visitor.start(dim, 0, dim - 1);
			for (int i = 0; i < dim; i++)
			{
				visitor.visit(i, getEntry(i));
			}
			return visitor.end();
		}

		/// <summary>
		/// Visits (but does not alter) some entries of this vector in default order
		/// (increasing index).
		/// </summary>
		/// <param name="visitor"> visitor to be used to process the entries of this vector </param>
		/// <param name="start"> the index of the first entry to be visited </param>
		/// <param name="end"> the index of the last entry to be visited (inclusive) </param>
		/// <returns> the value returned by <seealso cref="FieldVectorPreservingVisitor#end()"/>
		/// at the end of the walk </returns>
		/// <exception cref="NumberIsTooSmallException"> if {@code end < start}. </exception>
		/// <exception cref="OutOfRangeException"> if the indices are not valid.
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T walkInDefaultOrder(final FieldVectorPreservingVisitor<T> visitor, final int start, final int end) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T walkInDefaultOrder(FieldVectorPreservingVisitor<T> visitor, int start, int end)
		{
			checkIndices(start, end);
			visitor.start(Dimension, start, end);
			for (int i = start; i <= end; i++)
			{
				visitor.visit(i, getEntry(i));
			}
			return visitor.end();
		}

		/// <summary>
		/// Visits (but does not alter) all entries of this vector in optimized
		/// order. The order in which the entries are visited is selected so as to
		/// lead to the most efficient implementation; it might depend on the
		/// concrete implementation of this abstract class.
		/// </summary>
		/// <param name="visitor"> the visitor to be used to process the entries of this
		/// vector </param>
		/// <returns> the value returned by <seealso cref="FieldVectorPreservingVisitor#end()"/>
		/// at the end of the walk
		/// @since 3.3 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T walkInOptimizedOrder(final FieldVectorPreservingVisitor<T> visitor)
		public virtual T walkInOptimizedOrder(FieldVectorPreservingVisitor<T> visitor)
		{
			return walkInDefaultOrder(visitor);
		}

		/// <summary>
		/// Visits (but does not alter) some entries of this vector in optimized
		/// order. The order in which the entries are visited is selected so as to
		/// lead to the most efficient implementation; it might depend on the
		/// concrete implementation of this abstract class.
		/// </summary>
		/// <param name="visitor"> visitor to be used to process the entries of this vector </param>
		/// <param name="start"> the index of the first entry to be visited </param>
		/// <param name="end"> the index of the last entry to be visited (inclusive) </param>
		/// <returns> the value returned by <seealso cref="FieldVectorPreservingVisitor#end()"/>
		/// at the end of the walk </returns>
		/// <exception cref="NumberIsTooSmallException"> if {@code end < start}. </exception>
		/// <exception cref="OutOfRangeException"> if the indices are not valid.
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T walkInOptimizedOrder(final FieldVectorPreservingVisitor<T> visitor, final int start, final int end) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T walkInOptimizedOrder(FieldVectorPreservingVisitor<T> visitor, int start, int end)
		{
			return walkInDefaultOrder(visitor, start, end);
		}

		/// <summary>
		/// Visits (and possibly alters) all entries of this vector in default order
		/// (increasing index).
		/// </summary>
		/// <param name="visitor"> the visitor to be used to process and modify the entries
		/// of this vector </param>
		/// <returns> the value returned by <seealso cref="FieldVectorChangingVisitor#end()"/>
		/// at the end of the walk
		/// @since 3.3 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T walkInDefaultOrder(final FieldVectorChangingVisitor<T> visitor)
		public virtual T walkInDefaultOrder(FieldVectorChangingVisitor<T> visitor)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = getDimension();
			int dim = Dimension;
			visitor.start(dim, 0, dim - 1);
			for (int i = 0; i < dim; i++)
			{
				setEntry(i, visitor.visit(i, getEntry(i)));
			}
			return visitor.end();
		}

		/// <summary>
		/// Visits (and possibly alters) some entries of this vector in default order
		/// (increasing index).
		/// </summary>
		/// <param name="visitor"> visitor to be used to process the entries of this vector </param>
		/// <param name="start"> the index of the first entry to be visited </param>
		/// <param name="end"> the index of the last entry to be visited (inclusive) </param>
		/// <returns> the value returned by <seealso cref="FieldVectorChangingVisitor#end()"/>
		/// at the end of the walk </returns>
		/// <exception cref="NumberIsTooSmallException"> if {@code end < start}. </exception>
		/// <exception cref="OutOfRangeException"> if the indices are not valid.
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T walkInDefaultOrder(final FieldVectorChangingVisitor<T> visitor, final int start, final int end) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T walkInDefaultOrder(FieldVectorChangingVisitor<T> visitor, int start, int end)
		{
			checkIndices(start, end);
			visitor.start(Dimension, start, end);
			for (int i = start; i <= end; i++)
			{
				setEntry(i, visitor.visit(i, getEntry(i)));
			}
			return visitor.end();
		}

		/// <summary>
		/// Visits (and possibly alters) all entries of this vector in optimized
		/// order. The order in which the entries are visited is selected so as to
		/// lead to the most efficient implementation; it might depend on the
		/// concrete implementation of this abstract class.
		/// </summary>
		/// <param name="visitor"> the visitor to be used to process the entries of this
		/// vector </param>
		/// <returns> the value returned by <seealso cref="FieldVectorChangingVisitor#end()"/>
		/// at the end of the walk
		/// @since 3.3 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T walkInOptimizedOrder(final FieldVectorChangingVisitor<T> visitor)
		public virtual T walkInOptimizedOrder(FieldVectorChangingVisitor<T> visitor)
		{
			return walkInDefaultOrder(visitor);
		}

		/// <summary>
		/// Visits (and possibly change) some entries of this vector in optimized
		/// order. The order in which the entries are visited is selected so as to
		/// lead to the most efficient implementation; it might depend on the
		/// concrete implementation of this abstract class.
		/// </summary>
		/// <param name="visitor"> visitor to be used to process the entries of this vector </param>
		/// <param name="start"> the index of the first entry to be visited </param>
		/// <param name="end"> the index of the last entry to be visited (inclusive) </param>
		/// <returns> the value returned by <seealso cref="FieldVectorChangingVisitor#end()"/>
		/// at the end of the walk </returns>
		/// <exception cref="NumberIsTooSmallException"> if {@code end < start}. </exception>
		/// <exception cref="OutOfRangeException"> if the indices are not valid.
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T walkInOptimizedOrder(final FieldVectorChangingVisitor<T> visitor, final int start, final int end) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T walkInOptimizedOrder(FieldVectorChangingVisitor<T> visitor, int start, int end)
		{
			return walkInDefaultOrder(visitor, start, end);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + ((field == null) ? 0 : field.GetHashCode());
			result = prime * result + virtualSize;
			OpenIntToFieldHashMap<T>.Iterator iter = entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				int temp = iter.value().GetHashCode();
				result = prime * result + temp;
			}
			return result;
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool Equals(object obj)
		{

			if (this == obj)
			{
				return true;
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (!(obj instanceof SparseFieldVector<?>))
			if (!(obj is SparseFieldVector<?>))
			{
				return false;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") SparseFieldVector<T> other = (SparseFieldVector<T>) obj;
			SparseFieldVector<T> other = (SparseFieldVector<T>) obj; // OK, because "else if" check below ensures that
										   // other must be the same type as this
			if (field == null)
			{
				if (other.field != null)
				{
					return false;
				}
			}
			else if (!field.Equals(other.field))
			{
				return false;
			}
			if (virtualSize != other.virtualSize)
			{
				return false;
			}

			OpenIntToFieldHashMap<T>.Iterator iter = entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				T test = other.getEntry(iter.key());
				if (!test.Equals(iter.value()))
				{
					return false;
				}
			}
			iter = other.Entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				T test = iter.value();
				if (!test.Equals(getEntry(iter.key())))
				{
					return false;
				}
			}
			return true;
		}
	}

}
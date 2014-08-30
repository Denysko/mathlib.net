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
namespace org.apache.commons.math3.linear
{


	using org.apache.commons.math3;
	using org.apache.commons.math3;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MathArithmeticException = org.apache.commons.math3.exception.MathArithmeticException;
	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using ZeroException = org.apache.commons.math3.exception.ZeroException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using MathArrays = org.apache.commons.math3.util.MathArrays;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// This class implements the <seealso cref="FieldVector"/> interface with a <seealso cref="FieldElement"/> array. </summary>
	/// @param <T> the type of the field elements
	/// @version $Id: ArrayFieldVector.java 1570536 2014-02-21 11:26:09Z luc $
	/// @since 2.0 </param>
	[Serializable]
	public class ArrayFieldVector<T> : FieldVector<T> where T : org.apache.commons.math3.FieldElement<T>
	{
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 7648186910365927050L;

		/// <summary>
		/// Entries of the vector. </summary>
		private T[] data;

		/// <summary>
		/// Field to which the elements belong. </summary>
		private readonly Field<T> field;

		/// <summary>
		/// Build a 0-length vector.
		/// Zero-length vectors may be used to initialize construction of vectors
		/// by data gathering. We start with zero-length and use either the {@link
		/// #ArrayFieldVector(ArrayFieldVector, ArrayFieldVector)} constructor
		/// or one of the {@code append} methods (<seealso cref="#add(FieldVector)"/> or
		/// <seealso cref="#append(ArrayFieldVector)"/>) to gather data into this vector.
		/// </summary>
		/// <param name="field"> field to which the elements belong </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ArrayFieldVector(final org.apache.commons.math3.Field<T> field)
		public ArrayFieldVector(Field<T> field) : this(field, 0)
		{
		}

		/// <summary>
		/// Construct a vector of zeroes.
		/// </summary>
		/// <param name="field"> Field to which the elements belong. </param>
		/// <param name="size"> Size of the vector. </param>
		public ArrayFieldVector(Field<T> field, int size)
		{
			this.field = field;
			this.data = MathArrays.buildArray(field, size);
		}

		/// <summary>
		/// Construct a vector with preset values.
		/// </summary>
		/// <param name="size"> Size of the vector. </param>
		/// <param name="preset"> All entries will be set with this value. </param>
		public ArrayFieldVector(int size, T preset) : this(preset.Field, size)
		{
			Arrays.fill(data, preset);
		}

		/// <summary>
		/// Construct a vector from an array, copying the input array.
		/// This constructor needs a non-empty {@code d} array to retrieve
		/// the field from its first element. This implies it cannot build
		/// 0 length vectors. To build vectors from any size, one should
		/// use the <seealso cref="#ArrayFieldVector(Field, FieldElement[])"/> constructor.
		/// </summary>
		/// <param name="d"> Array. </param>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <exception cref="ZeroException"> if {@code d} is empty. </exception>
		/// <seealso cref= #ArrayFieldVector(Field, FieldElement[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector(T[] d) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.ZeroException
		public ArrayFieldVector(T[] d)
		{
			MathUtils.checkNotNull(d);
			try
			{
				field = d[0].Field;
				data = d.clone();
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new ZeroException(LocalizedFormats.VECTOR_MUST_HAVE_AT_LEAST_ONE_ELEMENT);
			}
		}

		/// <summary>
		/// Construct a vector from an array, copying the input array.
		/// </summary>
		/// <param name="field"> Field to which the elements belong. </param>
		/// <param name="d"> Array. </param>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <seealso cref= #ArrayFieldVector(FieldElement[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector(org.apache.commons.math3.Field<T> field, T[] d) throws org.apache.commons.math3.exception.NullArgumentException
		public ArrayFieldVector(Field<T> field, T[] d)
		{
			MathUtils.checkNotNull(d);
			this.field = field;
			data = d.clone();
		}

		/// <summary>
		/// Create a new ArrayFieldVector using the input array as the underlying
		/// data array.
		/// If an array is built specially in order to be embedded in a
		/// ArrayFieldVector and not used directly, the {@code copyArray} may be
		/// set to {@code false}. This will prevent the copying and improve
		/// performance as no new array will be built and no data will be copied.
		/// This constructor needs a non-empty {@code d} array to retrieve
		/// the field from its first element. This implies it cannot build
		/// 0 length vectors. To build vectors from any size, one should
		/// use the <seealso cref="#ArrayFieldVector(Field, FieldElement[], boolean)"/>
		/// constructor.
		/// </summary>
		/// <param name="d"> Data for the new vector. </param>
		/// <param name="copyArray"> If {@code true}, the input array will be copied,
		/// otherwise it will be referenced. </param>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <exception cref="ZeroException"> if {@code d} is empty. </exception>
		/// <seealso cref= #ArrayFieldVector(FieldElement[]) </seealso>
		/// <seealso cref= #ArrayFieldVector(Field, FieldElement[], boolean) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector(T[] d, boolean copyArray) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.ZeroException
		public ArrayFieldVector(T[] d, bool copyArray)
		{
			MathUtils.checkNotNull(d);
			if (d.Length == 0)
			{
				throw new ZeroException(LocalizedFormats.VECTOR_MUST_HAVE_AT_LEAST_ONE_ELEMENT);
			}
			field = d[0].Field;
			data = copyArray ? d.clone() : d;
		}

		/// <summary>
		/// Create a new ArrayFieldVector using the input array as the underlying
		/// data array.
		/// If an array is built specially in order to be embedded in a
		/// ArrayFieldVector and not used directly, the {@code copyArray} may be
		/// set to {@code false}. This will prevent the copying and improve
		/// performance as no new array will be built and no data will be copied.
		/// </summary>
		/// <param name="field"> Field to which the elements belong. </param>
		/// <param name="d"> Data for the new vector. </param>
		/// <param name="copyArray"> If {@code true}, the input array will be copied,
		/// otherwise it will be referenced. </param>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <seealso cref= #ArrayFieldVector(FieldElement[], boolean) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector(org.apache.commons.math3.Field<T> field, T[] d, boolean copyArray) throws org.apache.commons.math3.exception.NullArgumentException
		public ArrayFieldVector(Field<T> field, T[] d, bool copyArray)
		{
			MathUtils.checkNotNull(d);
			this.field = field;
			data = copyArray ? d.clone() : d;
		}

		/// <summary>
		/// Construct a vector from part of a array.
		/// </summary>
		/// <param name="d"> Array. </param>
		/// <param name="pos"> Position of the first entry. </param>
		/// <param name="size"> Number of entries to copy. </param>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <exception cref="NumberIsTooLargeException"> if the size of {@code d} is less
		/// than {@code pos + size}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector(T[] d, int pos, int size) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NumberIsTooLargeException
		public ArrayFieldVector(T[] d, int pos, int size)
		{
			MathUtils.checkNotNull(d);
			if (d.Length < pos + size)
			{
				throw new NumberIsTooLargeException(pos + size, d.Length, true);
			}
			field = d[0].Field;
			data = MathArrays.buildArray(field, size);
			Array.Copy(d, pos, data, 0, size);
		}

		/// <summary>
		/// Construct a vector from part of a array.
		/// </summary>
		/// <param name="field"> Field to which the elements belong. </param>
		/// <param name="d"> Array. </param>
		/// <param name="pos"> Position of the first entry. </param>
		/// <param name="size"> Number of entries to copy. </param>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <exception cref="NumberIsTooLargeException"> if the size of {@code d} is less
		/// than {@code pos + size}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector(org.apache.commons.math3.Field<T> field, T[] d, int pos, int size) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NumberIsTooLargeException
		public ArrayFieldVector(Field<T> field, T[] d, int pos, int size)
		{
			MathUtils.checkNotNull(d);
			if (d.Length < pos + size)
			{
				throw new NumberIsTooLargeException(pos + size, d.Length, true);
			}
			this.field = field;
			data = MathArrays.buildArray(field, size);
			Array.Copy(d, pos, data, 0, size);
		}

		/// <summary>
		/// Construct a vector from another vector, using a deep copy.
		/// </summary>
		/// <param name="v"> Vector to copy. </param>
		/// <exception cref="NullArgumentException"> if {@code v} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector(FieldVector<T> v) throws org.apache.commons.math3.exception.NullArgumentException
		public ArrayFieldVector(FieldVector<T> v)
		{
			MathUtils.checkNotNull(v);
			field = v.Field;
			data = MathArrays.buildArray(field, v.Dimension);
			for (int i = 0; i < data.Length; ++i)
			{
				data[i] = v.getEntry(i);
			}
		}

		/// <summary>
		/// Construct a vector from another vector, using a deep copy.
		/// </summary>
		/// <param name="v"> Vector to copy. </param>
		/// <exception cref="NullArgumentException"> if {@code v} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector(ArrayFieldVector<T> v) throws org.apache.commons.math3.exception.NullArgumentException
		public ArrayFieldVector(ArrayFieldVector<T> v)
		{
			MathUtils.checkNotNull(v);
			field = v.Field;
			data = v.data.clone();
		}

		/// <summary>
		/// Construct a vector from another vector.
		/// </summary>
		/// <param name="v"> Vector to copy. </param>
		/// <param name="deep"> If {@code true} perform a deep copy, otherwise perform
		/// a shallow copy </param>
		/// <exception cref="NullArgumentException"> if {@code v} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector(ArrayFieldVector<T> v, boolean deep) throws org.apache.commons.math3.exception.NullArgumentException
		public ArrayFieldVector(ArrayFieldVector<T> v, bool deep)
		{
			MathUtils.checkNotNull(v);
			field = v.Field;
			data = deep ? v.data.clone() : v.data;
		}

		/// <summary>
		/// Construct a vector by appending one vector to another vector.
		/// </summary>
		/// <param name="v1"> First vector (will be put in front of the new vector). </param>
		/// <param name="v2"> Second vector (will be put at back of the new vector). </param>
		/// <exception cref="NullArgumentException"> if {@code v1} or {@code v2} is
		/// {@code null}. </exception>
		/// @deprecated as of 3.2, replaced by <seealso cref="#ArrayFieldVector(FieldVector, FieldVector)"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.2, replaced by <seealso cref="#ArrayFieldVector(FieldVector, FieldVector)"/>") public ArrayFieldVector(ArrayFieldVector<T> v1, ArrayFieldVector<T> v2) throws org.apache.commons.math3.exception.NullArgumentException
		[Obsolete("as of 3.2, replaced by <seealso cref="#ArrayFieldVector(FieldVector, FieldVector)"/>")]
		public ArrayFieldVector(ArrayFieldVector<T> v1, ArrayFieldVector<T> v2) : this((FieldVector<T>) v1, (FieldVector<T>) v2)
		{
		}

		/// <summary>
		/// Construct a vector by appending one vector to another vector.
		/// </summary>
		/// <param name="v1"> First vector (will be put in front of the new vector). </param>
		/// <param name="v2"> Second vector (will be put at back of the new vector). </param>
		/// <exception cref="NullArgumentException"> if {@code v1} or {@code v2} is
		/// {@code null}.
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector(FieldVector<T> v1, FieldVector<T> v2) throws org.apache.commons.math3.exception.NullArgumentException
		public ArrayFieldVector(FieldVector<T> v1, FieldVector<T> v2)
		{
			MathUtils.checkNotNull(v1);
			MathUtils.checkNotNull(v2);
			field = v1.Field;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] v1Data = (v1 instanceof ArrayFieldVector) ? ((ArrayFieldVector<T>) v1).data : v1.toArray();
			T[] v1Data = (v1 is ArrayFieldVector) ? ((ArrayFieldVector<T>) v1).data : v1.toArray();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] v2Data = (v2 instanceof ArrayFieldVector) ? ((ArrayFieldVector<T>) v2).data : v2.toArray();
			T[] v2Data = (v2 is ArrayFieldVector) ? ((ArrayFieldVector<T>) v2).data : v2.toArray();
			data = MathArrays.buildArray(field, v1Data.Length + v2Data.Length);
			Array.Copy(v1Data, 0, data, 0, v1Data.Length);
			Array.Copy(v2Data, 0, data, v1Data.Length, v2Data.Length);
		}

		/// <summary>
		/// Construct a vector by appending one vector to another vector.
		/// </summary>
		/// <param name="v1"> First vector (will be put in front of the new vector). </param>
		/// <param name="v2"> Second vector (will be put at back of the new vector). </param>
		/// <exception cref="NullArgumentException"> if {@code v1} or {@code v2} is
		/// {@code null}. </exception>
		/// @deprecated as of 3.2, replaced by <seealso cref="#ArrayFieldVector(FieldVector, FieldElement[])"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.2, replaced by <seealso cref="#ArrayFieldVector(FieldVector, org.apache.commons.math3.FieldElement[])"/>") public ArrayFieldVector(ArrayFieldVector<T> v1, T[] v2) throws org.apache.commons.math3.exception.NullArgumentException
		[Obsolete("as of 3.2, replaced by <seealso cref="#ArrayFieldVector(FieldVector, org.apache.commons.math3.FieldElement[])"/>")]
		public ArrayFieldVector(ArrayFieldVector<T> v1, T[] v2) : this((FieldVector<T>) v1, v2)
		{
		}

		/// <summary>
		/// Construct a vector by appending one vector to another vector.
		/// </summary>
		/// <param name="v1"> First vector (will be put in front of the new vector). </param>
		/// <param name="v2"> Second vector (will be put at back of the new vector). </param>
		/// <exception cref="NullArgumentException"> if {@code v1} or {@code v2} is
		/// {@code null}.
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector(FieldVector<T> v1, T[] v2) throws org.apache.commons.math3.exception.NullArgumentException
		public ArrayFieldVector(FieldVector<T> v1, T[] v2)
		{
			MathUtils.checkNotNull(v1);
			MathUtils.checkNotNull(v2);
			field = v1.Field;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] v1Data = (v1 instanceof ArrayFieldVector) ? ((ArrayFieldVector<T>) v1).data : v1.toArray();
			T[] v1Data = (v1 is ArrayFieldVector) ? ((ArrayFieldVector<T>) v1).data : v1.toArray();
			data = MathArrays.buildArray(field, v1Data.Length + v2.Length);
			Array.Copy(v1Data, 0, data, 0, v1Data.Length);
			Array.Copy(v2, 0, data, v1Data.Length, v2.Length);
		}

		/// <summary>
		/// Construct a vector by appending one vector to another vector.
		/// </summary>
		/// <param name="v1"> First vector (will be put in front of the new vector). </param>
		/// <param name="v2"> Second vector (will be put at back of the new vector). </param>
		/// <exception cref="NullArgumentException"> if {@code v1} or {@code v2} is
		/// {@code null}. </exception>
		/// @deprecated as of 3.2, replaced by <seealso cref="#ArrayFieldVector(FieldElement[], FieldVector)"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.2, replaced by <seealso cref="#ArrayFieldVector(org.apache.commons.math3.FieldElement[] , FieldVector)"/>") public ArrayFieldVector(T[] v1, ArrayFieldVector<T> v2) throws org.apache.commons.math3.exception.NullArgumentException
		[Obsolete("as of 3.2, replaced by <seealso cref="#ArrayFieldVector(org.apache.commons.math3.FieldElement[] , FieldVector)"/>")]
		public ArrayFieldVector(T[] v1, ArrayFieldVector<T> v2) : this(v1, (FieldVector<T>) v2)
		{
		}

		/// <summary>
		/// Construct a vector by appending one vector to another vector.
		/// </summary>
		/// <param name="v1"> First vector (will be put in front of the new vector). </param>
		/// <param name="v2"> Second vector (will be put at back of the new vector). </param>
		/// <exception cref="NullArgumentException"> if {@code v1} or {@code v2} is
		/// {@code null}.
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector(T[] v1, FieldVector<T> v2) throws org.apache.commons.math3.exception.NullArgumentException
		public ArrayFieldVector(T[] v1, FieldVector<T> v2)
		{
			MathUtils.checkNotNull(v1);
			MathUtils.checkNotNull(v2);
			field = v2.Field;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] v2Data = (v2 instanceof ArrayFieldVector) ? ((ArrayFieldVector<T>) v2).data : v2.toArray();
			T[] v2Data = (v2 is ArrayFieldVector) ? ((ArrayFieldVector<T>) v2).data : v2.toArray();
			data = MathArrays.buildArray(field, v1.Length + v2Data.Length);
			Array.Copy(v1, 0, data, 0, v1.Length);
			Array.Copy(v2Data, 0, data, v1.Length, v2Data.Length);
		}

		/// <summary>
		/// Construct a vector by appending one vector to another vector.
		/// This constructor needs at least one non-empty array to retrieve
		/// the field from its first element. This implies it cannot build
		/// 0 length vectors. To build vectors from any size, one should
		/// use the <seealso cref="#ArrayFieldVector(Field, FieldElement[], FieldElement[])"/>
		/// constructor.
		/// </summary>
		/// <param name="v1"> First vector (will be put in front of the new vector). </param>
		/// <param name="v2"> Second vector (will be put at back of the new vector). </param>
		/// <exception cref="NullArgumentException"> if {@code v1} or {@code v2} is
		/// {@code null}. </exception>
		/// <exception cref="ZeroException"> if both arrays are empty. </exception>
		/// <seealso cref= #ArrayFieldVector(Field, FieldElement[], FieldElement[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector(T[] v1, T[] v2) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.ZeroException
		public ArrayFieldVector(T[] v1, T[] v2)
		{
			MathUtils.checkNotNull(v1);
			MathUtils.checkNotNull(v2);
			if (v1.Length + v2.Length == 0)
			{
				throw new ZeroException(LocalizedFormats.VECTOR_MUST_HAVE_AT_LEAST_ONE_ELEMENT);
			}
			data = MathArrays.buildArray(v1[0].Field, v1.Length + v2.Length);
			Array.Copy(v1, 0, data, 0, v1.Length);
			Array.Copy(v2, 0, data, v1.Length, v2.Length);
			field = data[0].Field;
		}

		/// <summary>
		/// Construct a vector by appending one vector to another vector.
		/// </summary>
		/// <param name="field"> Field to which the elements belong. </param>
		/// <param name="v1"> First vector (will be put in front of the new vector). </param>
		/// <param name="v2"> Second vector (will be put at back of the new vector). </param>
		/// <exception cref="NullArgumentException"> if {@code v1} or {@code v2} is
		/// {@code null}. </exception>
		/// <exception cref="ZeroException"> if both arrays are empty. </exception>
		/// <seealso cref= #ArrayFieldVector(FieldElement[], FieldElement[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector(org.apache.commons.math3.Field<T> field, T[] v1, T[] v2) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.ZeroException
		public ArrayFieldVector(Field<T> field, T[] v1, T[] v2)
		{
			MathUtils.checkNotNull(v1);
			MathUtils.checkNotNull(v2);
			if (v1.Length + v2.Length == 0)
			{
				throw new ZeroException(LocalizedFormats.VECTOR_MUST_HAVE_AT_LEAST_ONE_ELEMENT);
			}
			data = MathArrays.buildArray(field, v1.Length + v2.Length);
			Array.Copy(v1, 0, data, 0, v1.Length);
			Array.Copy(v2, 0, data, v1.Length, v2.Length);
			this.field = field;
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
		public virtual FieldVector<T> copy()
		{
			return new ArrayFieldVector<T>(this, true);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> add(FieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual FieldVector<T> add(FieldVector<T> v)
		{
			try
			{
				return add((ArrayFieldVector<T>) v);
			}
			catch (System.InvalidCastException cce)
			{
				checkVectorDimensions(v);
				T[] @out = MathArrays.buildArray(field, data.Length);
				for (int i = 0; i < data.Length; i++)
				{
					@out[i] = data[i].add(v.getEntry(i));
				}
				return new ArrayFieldVector<T>(field, @out, false);
			}
		}

		/// <summary>
		/// Compute the sum of {@code this} and {@code v}. </summary>
		/// <param name="v"> vector to be added </param>
		/// <returns> {@code this + v} </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector<T> add(ArrayFieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual ArrayFieldVector<T> add(ArrayFieldVector<T> v)
		{
			checkVectorDimensions(v.data.Length);
			T[] @out = MathArrays.buildArray(field, data.Length);
			for (int i = 0; i < data.Length; i++)
			{
				@out[i] = data[i].add(v.data[i]);
			}
			return new ArrayFieldVector<T>(field, @out, false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> subtract(FieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual FieldVector<T> subtract(FieldVector<T> v)
		{
			try
			{
				return subtract((ArrayFieldVector<T>) v);
			}
			catch (System.InvalidCastException cce)
			{
				checkVectorDimensions(v);
				T[] @out = MathArrays.buildArray(field, data.Length);
				for (int i = 0; i < data.Length; i++)
				{
					@out[i] = data[i].subtract(v.getEntry(i));
				}
				return new ArrayFieldVector<T>(field, @out, false);
			}
		}

		/// <summary>
		/// Compute {@code this} minus {@code v}. </summary>
		/// <param name="v"> vector to be subtracted </param>
		/// <returns> {@code this - v} </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector<T> subtract(ArrayFieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual ArrayFieldVector<T> subtract(ArrayFieldVector<T> v)
		{
			checkVectorDimensions(v.data.Length);
			T[] @out = MathArrays.buildArray(field, data.Length);
			for (int i = 0; i < data.Length; i++)
			{
				@out[i] = data[i].subtract(v.data[i]);
			}
			return new ArrayFieldVector<T>(field, @out, false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapAdd(T d) throws org.apache.commons.math3.exception.NullArgumentException
		public virtual FieldVector<T> mapAdd(T d)
		{
			T[] @out = MathArrays.buildArray(field, data.Length);
			for (int i = 0; i < data.Length; i++)
			{
				@out[i] = data[i].add(d);
			}
			return new ArrayFieldVector<T>(field, @out, false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapAddToSelf(T d) throws org.apache.commons.math3.exception.NullArgumentException
		public virtual FieldVector<T> mapAddToSelf(T d)
		{
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = data[i].add(d);
			}
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapSubtract(T d) throws org.apache.commons.math3.exception.NullArgumentException
		public virtual FieldVector<T> mapSubtract(T d)
		{
			T[] @out = MathArrays.buildArray(field, data.Length);
			for (int i = 0; i < data.Length; i++)
			{
				@out[i] = data[i].subtract(d);
			}
			return new ArrayFieldVector<T>(field, @out, false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapSubtractToSelf(T d) throws org.apache.commons.math3.exception.NullArgumentException
		public virtual FieldVector<T> mapSubtractToSelf(T d)
		{
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = data[i].subtract(d);
			}
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapMultiply(T d) throws org.apache.commons.math3.exception.NullArgumentException
		public virtual FieldVector<T> mapMultiply(T d)
		{
			T[] @out = MathArrays.buildArray(field, data.Length);
			for (int i = 0; i < data.Length; i++)
			{
				@out[i] = data[i].multiply(d);
			}
			return new ArrayFieldVector<T>(field, @out, false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapMultiplyToSelf(T d) throws org.apache.commons.math3.exception.NullArgumentException
		public virtual FieldVector<T> mapMultiplyToSelf(T d)
		{
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = data[i].multiply(d);
			}
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapDivide(T d) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.MathArithmeticException
		public virtual FieldVector<T> mapDivide(T d)
		{
			MathUtils.checkNotNull(d);
			T[] @out = MathArrays.buildArray(field, data.Length);
			for (int i = 0; i < data.Length; i++)
			{
				@out[i] = data[i].divide(d);
			}
			return new ArrayFieldVector<T>(field, @out, false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapDivideToSelf(T d) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.MathArithmeticException
		public virtual FieldVector<T> mapDivideToSelf(T d)
		{
			MathUtils.checkNotNull(d);
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = data[i].divide(d);
			}
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapInv() throws org.apache.commons.math3.exception.MathArithmeticException
		public virtual FieldVector<T> mapInv()
		{
			T[] @out = MathArrays.buildArray(field, data.Length);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T one = field.getOne();
			T one = field.One;
			for (int i = 0; i < data.Length; i++)
			{
				try
				{
					@out[i] = one.divide(data[i]);
				}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not allowed in C#:
//ORIGINAL LINE: catch (final org.apache.commons.math3.exception.MathArithmeticException e)
				catch (MathArithmeticException e)
				{
					throw new MathArithmeticException(LocalizedFormats.INDEX, i);
				}
			}
			return new ArrayFieldVector<T>(field, @out, false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> mapInvToSelf() throws org.apache.commons.math3.exception.MathArithmeticException
		public virtual FieldVector<T> mapInvToSelf()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T one = field.getOne();
			T one = field.One;
			for (int i = 0; i < data.Length; i++)
			{
				try
				{
					data[i] = one.divide(data[i]);
				}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not allowed in C#:
//ORIGINAL LINE: catch (final org.apache.commons.math3.exception.MathArithmeticException e)
				catch (MathArithmeticException e)
				{
					throw new MathArithmeticException(LocalizedFormats.INDEX, i);
				}
			}
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> ebeMultiply(FieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual FieldVector<T> ebeMultiply(FieldVector<T> v)
		{
			try
			{
				return ebeMultiply((ArrayFieldVector<T>) v);
			}
			catch (System.InvalidCastException cce)
			{
				checkVectorDimensions(v);
				T[] @out = MathArrays.buildArray(field, data.Length);
				for (int i = 0; i < data.Length; i++)
				{
					@out[i] = data[i].multiply(v.getEntry(i));
				}
				return new ArrayFieldVector<T>(field, @out, false);
			}
		}

		/// <summary>
		/// Element-by-element multiplication. </summary>
		/// <param name="v"> vector by which instance elements must be multiplied </param>
		/// <returns> a vector containing {@code this[i] * v[i]} for all {@code i} </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector<T> ebeMultiply(ArrayFieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual ArrayFieldVector<T> ebeMultiply(ArrayFieldVector<T> v)
		{
			checkVectorDimensions(v.data.Length);
			T[] @out = MathArrays.buildArray(field, data.Length);
			for (int i = 0; i < data.Length; i++)
			{
				@out[i] = data[i].multiply(v.data[i]);
			}
			return new ArrayFieldVector<T>(field, @out, false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> ebeDivide(FieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathArithmeticException
		public virtual FieldVector<T> ebeDivide(FieldVector<T> v)
		{
			try
			{
				return ebeDivide((ArrayFieldVector<T>) v);
			}
			catch (System.InvalidCastException cce)
			{
				checkVectorDimensions(v);
				T[] @out = MathArrays.buildArray(field, data.Length);
				for (int i = 0; i < data.Length; i++)
				{
					try
					{
						@out[i] = data[i].divide(v.getEntry(i));
					}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not allowed in C#:
//ORIGINAL LINE: catch (final org.apache.commons.math3.exception.MathArithmeticException e)
					catch (MathArithmeticException e)
					{
						throw new MathArithmeticException(LocalizedFormats.INDEX, i);
					}
				}
				return new ArrayFieldVector<T>(field, @out, false);
			}
		}

		/// <summary>
		/// Element-by-element division. </summary>
		/// <param name="v"> vector by which instance elements must be divided </param>
		/// <returns> a vector containing {@code this[i] / v[i]} for all {@code i} </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this} </exception>
		/// <exception cref="MathArithmeticException"> if one entry of {@code v} is zero. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector<T> ebeDivide(ArrayFieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathArithmeticException
		public virtual ArrayFieldVector<T> ebeDivide(ArrayFieldVector<T> v)
		{
			checkVectorDimensions(v.data.Length);
			T[] @out = MathArrays.buildArray(field, data.Length);
			for (int i = 0; i < data.Length; i++)
			{
				try
				{
					@out[i] = data[i].divide(v.data[i]);
				}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not allowed in C#:
//ORIGINAL LINE: catch (final org.apache.commons.math3.exception.MathArithmeticException e)
				catch (MathArithmeticException e)
				{
					throw new MathArithmeticException(LocalizedFormats.INDEX, i);
				}
			}
			return new ArrayFieldVector<T>(field, @out, false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual T[] Data
		{
			get
			{
				return data.clone();
			}
		}

		/// <summary>
		/// Returns a reference to the underlying data array.
		/// <p>Does not make a fresh copy of the underlying data.</p> </summary>
		/// <returns> array of entries </returns>
		public virtual T[] DataRef
		{
			get
			{
				return data;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T dotProduct(FieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual T dotProduct(FieldVector<T> v)
		{
			try
			{
				return dotProduct((ArrayFieldVector<T>) v);
			}
			catch (System.InvalidCastException cce)
			{
				checkVectorDimensions(v);
				T dot = field.Zero;
				for (int i = 0; i < data.Length; i++)
				{
					dot = dot.add(data[i].multiply(v.getEntry(i)));
				}
				return dot;
			}
		}

		/// <summary>
		/// Compute the dot product. </summary>
		/// <param name="v"> vector with which dot product should be computed </param>
		/// <returns> the scalar dot product of {@code this} and {@code v} </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T dotProduct(ArrayFieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual T dotProduct(ArrayFieldVector<T> v)
		{
			checkVectorDimensions(v.data.Length);
			T dot = field.Zero;
			for (int i = 0; i < data.Length; i++)
			{
				dot = dot.add(data[i].multiply(v.data[i]));
			}
			return dot;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> projection(FieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathArithmeticException
		public virtual FieldVector<T> projection(FieldVector<T> v)
		{
			return v.mapMultiply(dotProduct(v).divide(v.dotProduct(v)));
		}

		/// <summary>
		/// Find the orthogonal projection of this vector onto another vector. </summary>
		/// <param name="v"> vector onto which {@code this} must be projected </param>
		/// <returns> projection of {@code this} onto {@code v} </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this} </exception>
		/// <exception cref="MathArithmeticException"> if {@code v} is the null vector. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayFieldVector<T> projection(ArrayFieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathArithmeticException
		public virtual ArrayFieldVector<T> projection(ArrayFieldVector<T> v)
		{
			return (ArrayFieldVector<T>) v.mapMultiply(dotProduct(v).divide(v.dotProduct(v)));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual FieldMatrix<T> outerProduct(FieldVector<T> v)
		{
			try
			{
				return outerProduct((ArrayFieldVector<T>) v);
			}
			catch (System.InvalidCastException cce)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = data.length;
				int m = data.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = v.getDimension();
				int n = v.Dimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldMatrix<T> out = new Array2DRowFieldMatrix<T>(field, m, n);
				FieldMatrix<T> @out = new Array2DRowFieldMatrix<T>(field, m, n);
				for (int i = 0; i < m; i++)
				{
					for (int j = 0; j < n; j++)
					{
						@out.setEntry(i, j, data[i].multiply(v.getEntry(j)));
					}
				}
				return @out;
			}
		}

		/// <summary>
		/// Compute the outer product. </summary>
		/// <param name="v"> vector with which outer product should be computed </param>
		/// <returns> the matrix outer product between instance and v </returns>
		public virtual FieldMatrix<T> outerProduct(ArrayFieldVector<T> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = data.length;
			int m = data.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = v.data.length;
			int n = v.data.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldMatrix<T> out = new Array2DRowFieldMatrix<T>(field, m, n);
			FieldMatrix<T> @out = new Array2DRowFieldMatrix<T>(field, m, n);
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					@out.setEntry(i, j, data[i].multiply(v.data[j]));
				}
			}
			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual T getEntry(int index)
		{
			return data[index];
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int Dimension
		{
			get
			{
				return data.Length;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual FieldVector<T> append(FieldVector<T> v)
		{
			try
			{
				return append((ArrayFieldVector<T>) v);
			}
			catch (System.InvalidCastException cce)
			{
				return new ArrayFieldVector<T>(this,new ArrayFieldVector<T>(v));
			}
		}

		/// <summary>
		/// Construct a vector by appending a vector to this vector. </summary>
		/// <param name="v"> vector to append to this one. </param>
		/// <returns> a new vector </returns>
		public virtual ArrayFieldVector<T> append(ArrayFieldVector<T> v)
		{
			return new ArrayFieldVector<T>(this, v);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual FieldVector<T> append(T @in)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] out = org.apache.commons.math3.util.MathArrays.buildArray(field, data.length + 1);
			T[] @out = MathArrays.buildArray(field, data.Length + 1);
			Array.Copy(data, 0, @out, 0, data.Length);
			@out[data.Length] = @in;
			return new ArrayFieldVector<T>(field, @out, false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> getSubVector(int index, int n) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NotPositiveException
		public virtual FieldVector<T> getSubVector(int index, int n)
		{
			if (n < 0)
			{
				throw new NotPositiveException(LocalizedFormats.NUMBER_OF_ELEMENTS_SHOULD_BE_POSITIVE, n);
			}
			ArrayFieldVector<T> @out = new ArrayFieldVector<T>(field, n);
			try
			{
				Array.Copy(data, index, @out.data, 0, n);
			}
			catch (System.IndexOutOfRangeException e)
			{
				checkIndex(index);
				checkIndex(index + n - 1);
			}
			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual void setEntry(int index, T value)
		{
			try
			{
				data[index] = value;
			}
			catch (System.IndexOutOfRangeException e)
			{
				checkIndex(index);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSubVector(int index, FieldVector<T> v) throws org.apache.commons.math3.exception.OutOfRangeException
		public virtual void setSubVector(int index, FieldVector<T> v)
		{
			try
			{
				try
				{
					set(index, (ArrayFieldVector<T>) v);
				}
				catch (System.InvalidCastException cce)
				{
					for (int i = index; i < index + v.Dimension; ++i)
					{
						data[i] = v.getEntry(i - index);
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				checkIndex(index);
				checkIndex(index + v.Dimension - 1);
			}
		}

		/// <summary>
		/// Set a set of consecutive elements.
		/// </summary>
		/// <param name="index"> index of first element to be set. </param>
		/// <param name="v"> vector containing the values to set. </param>
		/// <exception cref="OutOfRangeException"> if the index is invalid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void set(int index, ArrayFieldVector<T> v) throws org.apache.commons.math3.exception.OutOfRangeException
		public virtual void set(int index, ArrayFieldVector<T> v)
		{
			try
			{
				Array.Copy(v.data, 0, data, index, v.data.Length);
			}
			catch (System.IndexOutOfRangeException e)
			{
				checkIndex(index);
				checkIndex(index + v.data.Length - 1);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual void set(T value)
		{
			Arrays.fill(data, value);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual T[] toArray()
		{
			return data.clone();
		}

		/// <summary>
		/// Check if instance and specified vectors have the same dimension. </summary>
		/// <param name="v"> vector to compare instance with </param>
		/// <exception cref="DimensionMismatchException"> if the vectors do not
		/// have the same dimensions </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkVectorDimensions(FieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException
		protected internal virtual void checkVectorDimensions(FieldVector<T> v)
		{
			checkVectorDimensions(v.Dimension);
		}

		/// <summary>
		/// Check if instance dimension is equal to some expected value.
		/// </summary>
		/// <param name="n"> Expected dimension. </param>
		/// <exception cref="DimensionMismatchException"> if the dimension is not equal to the
		/// size of {@code this} vector. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkVectorDimensions(int n) throws org.apache.commons.math3.exception.DimensionMismatchException
		protected internal virtual void checkVectorDimensions(int n)
		{
			if (data.Length != n)
			{
				throw new DimensionMismatchException(data.Length, n);
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
//ORIGINAL LINE: public T walkInDefaultOrder(final FieldVectorPreservingVisitor<T> visitor, final int start, final int end) throws org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.OutOfRangeException
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
//ORIGINAL LINE: public T walkInOptimizedOrder(final FieldVectorPreservingVisitor<T> visitor, final int start, final int end) throws org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.OutOfRangeException
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
//ORIGINAL LINE: public T walkInDefaultOrder(final FieldVectorChangingVisitor<T> visitor, final int start, final int end) throws org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.OutOfRangeException
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
//ORIGINAL LINE: public T walkInOptimizedOrder(final FieldVectorChangingVisitor<T> visitor, final int start, final int end) throws org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T walkInOptimizedOrder(FieldVectorChangingVisitor<T> visitor, int start, int end)
		{
			return walkInDefaultOrder(visitor, start, end);
		}

		/// <summary>
		/// Test for the equality of two vectors.
		/// </summary>
		/// <param name="other"> Object to test for equality. </param>
		/// <returns> {@code true} if two vector objects are equal, {@code false}
		/// otherwise. </returns>
		public override bool Equals(object other)
		{
			if (this == other)
			{
				return true;
			}
			if (other == null)
			{
				return false;
			}

			try
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") FieldVector<T> rhs = (FieldVector<T>) other;
				FieldVector<T> rhs = (FieldVector<T>) other; // May fail, but we ignore ClassCastException
				if (data.Length != rhs.Dimension)
				{
					return false;
				}

				for (int i = 0; i < data.Length; ++i)
				{
					if (!data[i].Equals(rhs.getEntry(i)))
					{
						return false;
					}
				}
				return true;
			}
			catch (System.InvalidCastException ex)
			{
				// ignore exception
				return false;
			}
		}

		/// <summary>
		/// Get a hashCode for the real vector.
		/// <p>All NaN values have the same hash code.</p> </summary>
		/// <returns> a hash code value for this object </returns>
		public override int GetHashCode()
		{
			int h = 3542;
			foreach (T a in data)
			{
				h ^= a.GetHashCode();
			}
			return h;
		}

		/// <summary>
		/// Check if an index is valid.
		/// </summary>
		/// <param name="index"> Index to check. </param>
		/// <exception cref="OutOfRangeException"> if the index is not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkIndex(final int index) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void checkIndex(int index)
		{
			if (index < 0 || index >= Dimension)
			{
				throw new OutOfRangeException(LocalizedFormats.INDEX, index, 0, Dimension - 1);
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
//ORIGINAL LINE: private void checkIndices(final int start, final int end) throws org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.OutOfRangeException
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

	}

}
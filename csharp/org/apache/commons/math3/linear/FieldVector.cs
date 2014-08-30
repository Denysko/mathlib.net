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
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;

	/// <summary>
	/// Interface defining a field-valued vector with basic algebraic operations.
	/// <p>
	/// vector element indexing is 0-based -- e.g., <code>getEntry(0)</code>
	/// returns the first element of the vector.
	/// </p>
	/// <p>
	/// The various <code>mapXxx</code> and <code>mapXxxToSelf</code> methods operate
	/// on vectors element-wise, i.e. they perform the same operation (adding a scalar,
	/// applying a function ...) on each element in turn. The <code>mapXxx</code>
	/// versions create a new vector to hold the result and do not change the instance.
	/// The <code>mapXxxToSelf</code> versions use the instance itself to store the
	/// results, so the instance is changed by these methods. In both cases, the result
	/// vector is returned by the methods, this allows to use the <i>fluent API</i>
	/// style, like this:
	/// </p>
	/// <pre>
	///   RealVector result = v.mapAddToSelf(3.0).mapTanToSelf().mapSquareToSelf();
	/// </pre>
	/// <p>
	/// Note that as almost all operations on <seealso cref="FieldElement"/> throw {@link
	/// NullArgumentException} when operating on a null element, it is the responsibility
	/// of <code>FieldVector</code> implementations to make sure no null elements
	/// are inserted into the vector. This must be done in all constructors and
	/// all setters.
	/// <p>
	/// </summary>
	/// @param <T> the type of the field elements
	/// @version $Id: FieldVector.java 1455233 2013-03-11 17:00:41Z luc $
	/// @since 2.0 </param>
	public interface FieldVector<T> where T : org.apache.commons.math3.FieldElement<T>
	{

		/// <summary>
		/// Get the type of field elements of the vector. </summary>
		/// <returns> type of field elements of the vector </returns>
		Field<T> Field {get;}

		/// <summary>
		/// Returns a (deep) copy of this. </summary>
		/// <returns> vector copy </returns>
		FieldVector<T> copy();

		/// <summary>
		/// Compute the sum of {@code this} and {@code v}. </summary>
		/// <param name="v"> vector to be added </param>
		/// <returns> {@code this + v} </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as {@code this} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> add(FieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException;
		FieldVector<T> add(FieldVector<T> v);

		/// <summary>
		/// Compute {@code this} minus {@code v}. </summary>
		/// <param name="v"> vector to be subtracted </param>
		/// <returns> {@code this - v} </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as {@code this} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> subtract(FieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException;
		FieldVector<T> subtract(FieldVector<T> v);

		/// <summary>
		/// Map an addition operation to each entry. </summary>
		/// <param name="d"> value to be added to each entry </param>
		/// <returns> {@code this + d} </returns>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> mapAdd(T d) throws org.apache.commons.math3.exception.NullArgumentException;
		FieldVector<T> mapAdd(T d);

		/// <summary>
		/// Map an addition operation to each entry.
		/// <p>The instance <strong>is</strong> changed by this method.</p> </summary>
		/// <param name="d"> value to be added to each entry </param>
		/// <returns> for convenience, return {@code this} </returns>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> mapAddToSelf(T d) throws org.apache.commons.math3.exception.NullArgumentException;
		FieldVector<T> mapAddToSelf(T d);

		/// <summary>
		/// Map a subtraction operation to each entry. </summary>
		/// <param name="d"> value to be subtracted to each entry </param>
		/// <returns> {@code this - d} </returns>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> mapSubtract(T d) throws org.apache.commons.math3.exception.NullArgumentException;
		FieldVector<T> mapSubtract(T d);

		/// <summary>
		/// Map a subtraction operation to each entry.
		/// <p>The instance <strong>is</strong> changed by this method.</p> </summary>
		/// <param name="d"> value to be subtracted to each entry </param>
		/// <returns> for convenience, return {@code this} </returns>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> mapSubtractToSelf(T d) throws org.apache.commons.math3.exception.NullArgumentException;
		FieldVector<T> mapSubtractToSelf(T d);

		/// <summary>
		/// Map a multiplication operation to each entry. </summary>
		/// <param name="d"> value to multiply all entries by </param>
		/// <returns> {@code this * d} </returns>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> mapMultiply(T d) throws org.apache.commons.math3.exception.NullArgumentException;
		FieldVector<T> mapMultiply(T d);

		/// <summary>
		/// Map a multiplication operation to each entry.
		/// <p>The instance <strong>is</strong> changed by this method.</p> </summary>
		/// <param name="d"> value to multiply all entries by </param>
		/// <returns> for convenience, return {@code this} </returns>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> mapMultiplyToSelf(T d) throws org.apache.commons.math3.exception.NullArgumentException;
		FieldVector<T> mapMultiplyToSelf(T d);

		/// <summary>
		/// Map a division operation to each entry. </summary>
		/// <param name="d"> value to divide all entries by </param>
		/// <returns> {@code this / d} </returns>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <exception cref="MathArithmeticException"> if {@code d} is zero. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> mapDivide(T d) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.MathArithmeticException;
		FieldVector<T> mapDivide(T d);

		/// <summary>
		/// Map a division operation to each entry.
		/// <p>The instance <strong>is</strong> changed by this method.</p> </summary>
		/// <param name="d"> value to divide all entries by </param>
		/// <returns> for convenience, return {@code this} </returns>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <exception cref="MathArithmeticException"> if {@code d} is zero. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> mapDivideToSelf(T d) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.MathArithmeticException;
		FieldVector<T> mapDivideToSelf(T d);

		/// <summary>
		/// Map the 1/x function to each entry. </summary>
		/// <returns> a vector containing the result of applying the function to each entry. </returns>
		/// <exception cref="MathArithmeticException"> if one of the entries is zero. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> mapInv() throws org.apache.commons.math3.exception.MathArithmeticException;
		FieldVector<T> mapInv();

		/// <summary>
		/// Map the 1/x function to each entry.
		/// <p>The instance <strong>is</strong> changed by this method.</p> </summary>
		/// <returns> for convenience, return {@code this} </returns>
		/// <exception cref="MathArithmeticException"> if one of the entries is zero. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> mapInvToSelf() throws org.apache.commons.math3.exception.MathArithmeticException;
		FieldVector<T> mapInvToSelf();

		/// <summary>
		/// Element-by-element multiplication. </summary>
		/// <param name="v"> vector by which instance elements must be multiplied </param>
		/// <returns> a vector containing {@code this[i] * v[i]} for all {@code i} </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as {@code this} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> ebeMultiply(FieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException;
		FieldVector<T> ebeMultiply(FieldVector<T> v);

		/// <summary>
		/// Element-by-element division. </summary>
		/// <param name="v"> vector by which instance elements must be divided </param>
		/// <returns> a vector containing {@code this[i] / v[i]} for all {@code i} </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as {@code this} </exception>
		/// <exception cref="MathArithmeticException"> if one entry of {@code v} is zero. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> ebeDivide(FieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathArithmeticException;
		FieldVector<T> ebeDivide(FieldVector<T> v);

		/// <summary>
		/// Returns vector entries as a T array. </summary>
		/// <returns> T array of entries </returns>
		/// @deprecated as of 3.1, to be removed in 4.0. Please use the <seealso cref="#toArray()"/> method instead. 
		[Obsolete("as of 3.1, to be removed in 4.0. Please use the <seealso cref="#toArray()"/> method instead.")]
		T[] Data {get;}

		/// <summary>
		/// Compute the dot product. </summary>
		/// <param name="v"> vector with which dot product should be computed </param>
		/// <returns> the scalar dot product of {@code this} and {@code v} </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as {@code this} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T dotProduct(FieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException;
		T dotProduct(FieldVector<T> v);

		/// <summary>
		/// Find the orthogonal projection of this vector onto another vector. </summary>
		/// <param name="v"> vector onto which {@code this} must be projected </param>
		/// <returns> projection of {@code this} onto {@code v} </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as {@code this} </exception>
		/// <exception cref="MathArithmeticException"> if {@code v} is the null vector. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> projection(FieldVector<T> v) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathArithmeticException;
		FieldVector<T> projection(FieldVector<T> v);

		/// <summary>
		/// Compute the outer product. </summary>
		/// <param name="v"> vector with which outer product should be computed </param>
		/// <returns> the matrix outer product between instance and v </returns>
		FieldMatrix<T> outerProduct(FieldVector<T> v);

		/// <summary>
		/// Returns the entry in the specified index.
		/// </summary>
		/// <param name="index"> Index location of entry to be fetched. </param>
		/// <returns> the vector entry at {@code index}. </returns>
		/// <exception cref="OutOfRangeException"> if the index is not valid. </exception>
		/// <seealso cref= #setEntry(int, FieldElement) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T getEntry(int index) throws org.apache.commons.math3.exception.OutOfRangeException;
		T getEntry(int index);

		/// <summary>
		/// Set a single element. </summary>
		/// <param name="index"> element index. </param>
		/// <param name="value"> new value for the element. </param>
		/// <exception cref="OutOfRangeException"> if the index is not valid. </exception>
		/// <seealso cref= #getEntry(int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setEntry(int index, T value) throws org.apache.commons.math3.exception.OutOfRangeException;
		void setEntry(int index, T value);

		/// <summary>
		/// Returns the size of the vector. </summary>
		/// <returns> size </returns>
		int Dimension {get;}

		/// <summary>
		/// Construct a vector by appending a vector to this vector. </summary>
		/// <param name="v"> vector to append to this one. </param>
		/// <returns> a new vector </returns>
		FieldVector<T> append(FieldVector<T> v);

		/// <summary>
		/// Construct a vector by appending a T to this vector. </summary>
		/// <param name="d"> T to append. </param>
		/// <returns> a new vector </returns>
		FieldVector<T> append(T d);

		/// <summary>
		/// Get a subvector from consecutive elements. </summary>
		/// <param name="index"> index of first element. </param>
		/// <param name="n"> number of elements to be retrieved. </param>
		/// <returns> a vector containing n elements. </returns>
		/// <exception cref="OutOfRangeException"> if the index is not valid. </exception>
		/// <exception cref="NotPositiveException"> if the number of elements if not positive. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> getSubVector(int index, int n) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NotPositiveException;
		FieldVector<T> getSubVector(int index, int n);

		/// <summary>
		/// Set a set of consecutive elements. </summary>
		/// <param name="index"> index of first element to be set. </param>
		/// <param name="v"> vector containing the values to set. </param>
		/// <exception cref="OutOfRangeException"> if the index is not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setSubVector(int index, FieldVector<T> v) throws org.apache.commons.math3.exception.OutOfRangeException;
		void setSubVector(int index, FieldVector<T> v);

		/// <summary>
		/// Set all elements to a single value. </summary>
		/// <param name="value"> single value to set for all elements </param>
		void set(T value);

		/// <summary>
		/// Convert the vector to a T array.
		/// <p>The array is independent from vector data, it's elements
		/// are copied.</p> </summary>
		/// <returns> array containing a copy of vector elements </returns>
		T[] toArray();

	}

}
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
namespace org.apache.commons.math3.stat.descriptive
{

	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;
	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using MathArrays = org.apache.commons.math3.util.MathArrays;

	/// <summary>
	/// Abstract base class for all implementations of the
	/// <seealso cref="UnivariateStatistic"/> interface.
	/// <p>
	/// Provides a default implementation of <code>evaluate(double[]),</code>
	/// delegating to <code>evaluate(double[], int, int)</code> in the natural way.
	/// </p>
	/// <p>
	/// Also includes a <code>test</code> method that performs generic parameter
	/// validation for the <code>evaluate</code> methods.</p>
	/// 
	/// @version $Id: AbstractUnivariateStatistic.java 1588601 2014-04-19 01:27:33Z psteitz $
	/// </summary>
	public abstract class AbstractUnivariateStatistic : UnivariateStatistic
	{

		/// <summary>
		/// Stored data. </summary>
		private double[] storedData;

		/// <summary>
		/// Set the data array.
		/// <p>
		/// The stored value is a copy of the parameter array, not the array itself.
		/// </p> </summary>
		/// <param name="values"> data array to store (may be null to remove stored data) </param>
		/// <seealso cref= #evaluate() </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setData(final double[] values)
		public virtual double[] Data
		{
			set
			{
				storedData = (value == null) ? null : value.clone();
			}
			get
			{
				return (storedData == null) ? null : storedData.clone();
			}
		}


		/// <summary>
		/// Get a reference to the stored data array. </summary>
		/// <returns> reference to the stored data array (may be null) </returns>
		protected internal virtual double[] DataRef
		{
			get
			{
				return storedData;
			}
		}

		/// <summary>
		/// Set the data array.  The input array is copied, not referenced.
		/// </summary>
		/// <param name="values"> data array to store </param>
		/// <param name="begin"> the index of the first element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <exception cref="MathIllegalArgumentException"> if values is null or the indices
		/// are not valid </exception>
		/// <seealso cref= #evaluate() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setData(final double[] values, final int begin, final int length) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setData(double[] values, int begin, int length)
		{
			if (values == null)
			{
				throw new NullArgumentException(LocalizedFormats.INPUT_ARRAY);
			}

			if (begin < 0)
			{
				throw new NotPositiveException(LocalizedFormats.START_POSITION, begin);
			}

			if (length < 0)
			{
				throw new NotPositiveException(LocalizedFormats.LENGTH, length);
			}

			if (begin + length > values.Length)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.SUBARRAY_ENDS_AFTER_ARRAY_END, begin + length, values.Length, true);
			}
			storedData = new double[length];
			Array.Copy(values, begin, storedData, 0, length);
		}

		/// <summary>
		/// Returns the result of evaluating the statistic over the stored data.
		/// <p>
		/// The stored array is the one which was set by previous calls to <seealso cref="#setData(double[])"/>.
		/// </p> </summary>
		/// <returns> the value of the statistic applied to the stored data </returns>
		/// <exception cref="MathIllegalArgumentException"> if the stored data array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate() throws org.apache.commons.math3.exception.MathIllegalArgumentException
		public virtual double evaluate()
		{
			return evaluate(storedData);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values)
		{
			test(values, 0, 0);
			return evaluate(values, 0, values.Length);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract double evaluate(final double[] values, final int begin, final int length) throws org.apache.commons.math3.exception.MathIllegalArgumentException;
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public abstract double evaluate(double[] values, int begin, int length);

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public abstract UnivariateStatistic copy();

		/// <summary>
		/// This method is used by <code>evaluate(double[], int, int)</code> methods
		/// to verify that the input parameters designate a subarray of positive length.
		/// <p>
		/// <ul>
		/// <li>returns <code>true</code> iff the parameters designate a subarray of
		/// positive length</li>
		/// <li>throws <code>MathIllegalArgumentException</code> if the array is null or
		/// or the indices are invalid</li>
		/// <li>returns <code>false</li> if the array is non-null, but
		/// <code>length</code> is 0.
		/// </ul></p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> true if the parameters are valid and designate a subarray of positive length </returns>
		/// <exception cref="MathIllegalArgumentException"> if the indices are invalid or the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean test(final double[] values, final int begin, final int length) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual bool test(double[] values, int begin, int length)
		{
			return MathArrays.verifyValues(values, begin, length, false);
		}

		/// <summary>
		/// This method is used by <code>evaluate(double[], int, int)</code> methods
		/// to verify that the input parameters designate a subarray of positive length.
		/// <p>
		/// <ul>
		/// <li>returns <code>true</code> iff the parameters designate a subarray of
		/// non-negative length</li>
		/// <li>throws <code>IllegalArgumentException</code> if the array is null or
		/// or the indices are invalid</li>
		/// <li>returns <code>false</li> if the array is non-null, but
		/// <code>length</code> is 0 unless <code>allowEmpty</code> is <code>true</code>
		/// </ul></p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <param name="allowEmpty"> if <code>true</code> then zero length arrays are allowed </param>
		/// <returns> true if the parameters are valid </returns>
		/// <exception cref="MathIllegalArgumentException"> if the indices are invalid or the array is null
		/// @since 3.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean test(final double[] values, final int begin, final int length, final boolean allowEmpty) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual bool test(double[] values, int begin, int length, bool allowEmpty)
		{
			return MathArrays.verifyValues(values, begin, length, allowEmpty);
		}

		/// <summary>
		/// This method is used by <code>evaluate(double[], double[], int, int)</code> methods
		/// to verify that the begin and length parameters designate a subarray of positive length
		/// and the weights are all non-negative, non-NaN, finite, and not all zero.
		/// <p>
		/// <ul>
		/// <li>returns <code>true</code> iff the parameters designate a subarray of
		/// positive length and the weights array contains legitimate values.</li>
		/// <li>throws <code>IllegalArgumentException</code> if any of the following are true:
		/// <ul><li>the values array is null</li>
		///     <li>the weights array is null</li>
		///     <li>the weights array does not have the same length as the values array</li>
		///     <li>the weights array contains one or more infinite values</li>
		///     <li>the weights array contains one or more NaN values</li>
		///     <li>the weights array contains negative values</li>
		///     <li>the start and length arguments do not determine a valid array</li></ul>
		/// </li>
		/// <li>returns <code>false</li> if the array is non-null, but
		/// <code>length</code> is 0.
		/// </ul></p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="weights"> the weights array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> true if the parameters are valid and designate a subarray of positive length </returns>
		/// <exception cref="MathIllegalArgumentException"> if the indices are invalid or the array is null
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean test(final double[] values, final double[] weights, final int begin, final int length) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual bool test(double[] values, double[] weights, int begin, int length)
		{
			return MathArrays.verifyValues(values, weights, begin, length, false);
		}

		/// <summary>
		/// This method is used by <code>evaluate(double[], double[], int, int)</code> methods
		/// to verify that the begin and length parameters designate a subarray of positive length
		/// and the weights are all non-negative, non-NaN, finite, and not all zero.
		/// <p>
		/// <ul>
		/// <li>returns <code>true</code> iff the parameters designate a subarray of
		/// non-negative length and the weights array contains legitimate values.</li>
		/// <li>throws <code>MathIllegalArgumentException</code> if any of the following are true:
		/// <ul><li>the values array is null</li>
		///     <li>the weights array is null</li>
		///     <li>the weights array does not have the same length as the values array</li>
		///     <li>the weights array contains one or more infinite values</li>
		///     <li>the weights array contains one or more NaN values</li>
		///     <li>the weights array contains negative values</li>
		///     <li>the start and length arguments do not determine a valid array</li></ul>
		/// </li>
		/// <li>returns <code>false</li> if the array is non-null, but
		/// <code>length</code> is 0 unless <code>allowEmpty</code> is <code>true</code>.
		/// </ul></p>
		/// </summary>
		/// <param name="values"> the input array. </param>
		/// <param name="weights"> the weights array. </param>
		/// <param name="begin"> index of the first array element to include. </param>
		/// <param name="length"> the number of elements to include. </param>
		/// <param name="allowEmpty"> if {@code true} than allow zero length arrays to pass. </param>
		/// <returns> {@code true} if the parameters are valid. </returns>
		/// <exception cref="NullArgumentException"> if either of the arrays are null </exception>
		/// <exception cref="MathIllegalArgumentException"> if the array indices are not valid,
		/// the weights array contains NaN, infinite or negative elements, or there
		/// are no positive weights.
		/// @since 3.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean test(final double[] values, final double[] weights, final int begin, final int length, final boolean allowEmpty) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual bool test(double[] values, double[] weights, int begin, int length, bool allowEmpty)
		{

			return MathArrays.verifyValues(values, weights, begin, length, allowEmpty);
		}
	}


}
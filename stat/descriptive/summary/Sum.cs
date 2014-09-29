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
namespace mathlib.stat.descriptive.summary
{

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using MathUtils = mathlib.util.MathUtils;


	/// <summary>
	/// Returns the sum of the available values.
	/// <p>
	/// If there are no values in the dataset, then 0 is returned.
	/// If any of the values are
	/// <code>NaN</code>, then <code>NaN</code> is returned.</p>
	/// <p>
	/// <strong>Note that this implementation is not synchronized.</strong> If
	/// multiple threads access an instance of this class concurrently, and at least
	/// one of the threads invokes the <code>increment()</code> or
	/// <code>clear()</code> method, it must be synchronized externally.</p>
	/// 
	/// @version $Id: Sum.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class Sum : AbstractStorelessUnivariateStatistic
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -8231831954703408316L;

		private long n;

		/// <summary>
		/// The currently running sum.
		/// </summary>
		private double value;

		/// <summary>
		/// Create a Sum instance
		/// </summary>
		public Sum()
		{
			n = 0;
			value = 0;
		}

		/// <summary>
		/// Copy constructor, creates a new {@code Sum} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code Sum} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Sum(Sum original) throws mathlib.exception.NullArgumentException
		public Sum(Sum original)
		{
			copy(original, this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void increment(final double d)
		public override void increment(double d)
		{
			value += d;
			n++;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double Result
		{
			get
			{
				return value;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override long N
		{
			get
			{
				return n;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override void clear()
		{
			value = 0;
			n = 0;
		}

		/// <summary>
		/// The sum of the entries in the specified portion of
		/// the input array, or 0 if the designated subarray
		/// is empty.
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the sum of the values or 0 if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		///  parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double[] values, int begin, int length)
		{
			double sum = double.NaN;
			if (test(values, begin, length, true))
			{
				sum = 0.0;
				for (int i = begin; i < begin + length; i++)
				{
					sum += values[i];
				}
			}
			return sum;
		}

		/// <summary>
		/// The weighted sum of the entries in the specified portion of
		/// the input array, or 0 if the designated subarray
		/// is empty.
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if any of the following are true:
		/// <ul><li>the values array is null</li>
		///     <li>the weights array is null</li>
		///     <li>the weights array does not have the same length as the values array</li>
		///     <li>the weights array contains one or more infinite values</li>
		///     <li>the weights array contains one or more NaN values</li>
		///     <li>the weights array contains negative values</li>
		///     <li>the start and length arguments do not determine a valid array</li>
		/// </ul></p>
		/// <p>
		/// Uses the formula, <pre>
		///    weighted sum = &Sigma;(values[i] * weights[i])
		/// </pre></p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="weights"> the weights array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the sum of the values or 0 if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the parameters are not valid
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double[] weights, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values, double[] weights, int begin, int length)
		{
			double sum = double.NaN;
			if (test(values, weights, begin, length, true))
			{
				sum = 0.0;
				for (int i = begin; i < begin + length; i++)
				{
					sum += values[i] * weights[i];
				}
			}
			return sum;
		}

		/// <summary>
		/// The weighted sum of the entries in the the input array.
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if any of the following are true:
		/// <ul><li>the values array is null</li>
		///     <li>the weights array is null</li>
		///     <li>the weights array does not have the same length as the values array</li>
		///     <li>the weights array contains one or more infinite values</li>
		///     <li>the weights array contains one or more NaN values</li>
		///     <li>the weights array contains negative values</li>
		/// </ul></p>
		/// <p>
		/// Uses the formula, <pre>
		///    weighted sum = &Sigma;(values[i] * weights[i])
		/// </pre></p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="weights"> the weights array </param>
		/// <returns> the sum of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the parameters are not valid
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double[] weights) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values, double[] weights)
		{
			return evaluate(values, weights, 0, values.Length);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override Sum copy()
		{
			Sum result = new Sum();
			// No try-catch or advertised exception because args are valid
			copy(this, result);
			return result;
		}

		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> Sum to copy </param>
		/// <param name="dest"> Sum to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(Sum source, Sum dest) throws mathlib.exception.NullArgumentException
		public static void copy(Sum source, Sum dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			dest.Data = source.DataRef;
			dest.n = source.n;
			dest.value = source.value;
		}

	}

}
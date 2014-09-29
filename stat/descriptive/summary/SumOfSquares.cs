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
	/// Returns the sum of the squares of the available values.
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
	/// @version $Id: SumOfSquares.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class SumOfSquares : AbstractStorelessUnivariateStatistic
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 1460986908574398008L;

		private long n;

		/// <summary>
		/// The currently running sumSq
		/// </summary>
		private double value;

		/// <summary>
		/// Create a SumOfSquares instance
		/// </summary>
		public SumOfSquares()
		{
			n = 0;
			value = 0;
		}

		/// <summary>
		/// Copy constructor, creates a new {@code SumOfSquares} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code SumOfSquares} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SumOfSquares(SumOfSquares original) throws mathlib.exception.NullArgumentException
		public SumOfSquares(SumOfSquares original)
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
			value += d * d;
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
		/// Returns the sum of the squares of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the sum of the squares of the values or 0 if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		///  parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values,final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double[] values, int begin, int length)
		{
			double sumSq = double.NaN;
			if (test(values, begin, length, true))
			{
				sumSq = 0.0;
				for (int i = begin; i < begin + length; i++)
				{
					sumSq += values[i] * values[i];
				}
			}
			return sumSq;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override SumOfSquares copy()
		{
			SumOfSquares result = new SumOfSquares();
			// no try-catch or advertised exception here because args are valid
			copy(this, result);
			return result;
		}

		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> SumOfSquares to copy </param>
		/// <param name="dest"> SumOfSquares to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(SumOfSquares source, SumOfSquares dest) throws mathlib.exception.NullArgumentException
		public static void copy(SumOfSquares source, SumOfSquares dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			dest.Data = source.DataRef;
			dest.n = source.n;
			dest.value = source.value;
		}

	}

}
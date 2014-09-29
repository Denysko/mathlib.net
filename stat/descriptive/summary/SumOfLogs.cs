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
	using FastMath = mathlib.util.FastMath;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// Returns the sum of the natural logs for this collection of values.
	/// <p>
	/// Uses <seealso cref="mathlib.util.FastMath#log(double)"/> to compute the logs.
	/// Therefore,
	/// <ul>
	/// <li>If any of values are &lt; 0, the result is <code>NaN.</code></li>
	/// <li>If all values are non-negative and less than
	/// <code>Double.POSITIVE_INFINITY</code>,  but at least one value is 0, the
	/// result is <code>Double.NEGATIVE_INFINITY.</code></li>
	/// <li>If both <code>Double.POSITIVE_INFINITY</code> and
	/// <code>Double.NEGATIVE_INFINITY</code> are among the values, the result is
	/// <code>NaN.</code></li>
	/// </ul></p>
	/// <p>
	/// <strong>Note that this implementation is not synchronized.</strong> If
	/// multiple threads access an instance of this class concurrently, and at least
	/// one of the threads invokes the <code>increment()</code> or
	/// <code>clear()</code> method, it must be synchronized externally.</p>
	/// 
	/// @version $Id: SumOfLogs.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class SumOfLogs : AbstractStorelessUnivariateStatistic
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -370076995648386763L;

		/// <summary>
		///Number of values that have been added </summary>
		private int n;

		/// <summary>
		/// The currently running value
		/// </summary>
		private double value;

		/// <summary>
		/// Create a SumOfLogs instance
		/// </summary>
		public SumOfLogs()
		{
		   value = 0d;
		   n = 0;
		}

		/// <summary>
		/// Copy constructor, creates a new {@code SumOfLogs} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code SumOfLogs} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SumOfLogs(SumOfLogs original) throws mathlib.exception.NullArgumentException
		public SumOfLogs(SumOfLogs original)
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
			value += FastMath.log(d);
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
			value = 0d;
			n = 0;
		}

		/// <summary>
		/// Returns the sum of the natural logs of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		/// <p>
		/// See <seealso cref="SumOfLogs"/>.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the sum of the natural logs of the values or 0 if
		/// length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		///  parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double[] values, int begin, int length)
		{
			double sumLog = double.NaN;
			if (test(values, begin, length, true))
			{
				sumLog = 0.0;
				for (int i = begin; i < begin + length; i++)
				{
					sumLog += FastMath.log(values[i]);
				}
			}
			return sumLog;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override SumOfLogs copy()
		{
			SumOfLogs result = new SumOfLogs();
			// No try-catch or advertised exception here because args are valid
			copy(this, result);
			return result;
		}

		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> SumOfLogs to copy </param>
		/// <param name="dest"> SumOfLogs to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(SumOfLogs source, SumOfLogs dest) throws mathlib.exception.NullArgumentException
		public static void copy(SumOfLogs source, SumOfLogs dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			dest.Data = source.DataRef;
			dest.n = source.n;
			dest.value = source.value;
		}
	}

}
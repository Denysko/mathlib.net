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
namespace mathlib.stat.descriptive.rank
{

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// Returns the maximum of the available values.
	/// <p>
	/// <ul>
	/// <li>The result is <code>NaN</code> iff all values are <code>NaN</code>
	/// (i.e. <code>NaN</code> values have no impact on the value of the statistic).</li>
	/// <li>If any of the values equals <code>Double.POSITIVE_INFINITY</code>,
	/// the result is <code>Double.POSITIVE_INFINITY.</code></li>
	/// </ul></p>
	/// <p>
	/// <strong>Note that this implementation is not synchronized.</strong> If
	/// multiple threads access an instance of this class concurrently, and at least
	/// one of the threads invokes the <code>increment()</code> or
	/// <code>clear()</code> method, it must be synchronized externally.</p>
	/// 
	/// @version $Id: Max.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class Max : AbstractStorelessUnivariateStatistic
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -5593383832225844641L;

		/// <summary>
		/// Number of values that have been added </summary>
		private long n;

		/// <summary>
		/// Current value of the statistic </summary>
		private double value;

		/// <summary>
		/// Create a Max instance
		/// </summary>
		public Max()
		{
			n = 0;
			value = double.NaN;
		}

		/// <summary>
		/// Copy constructor, creates a new {@code Max} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code Max} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Max(Max original) throws mathlib.exception.NullArgumentException
		public Max(Max original)
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
			if (d > value || double.IsNaN(value))
			{
				value = d;
			}
			n++;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override void clear()
		{
			value = double.NaN;
			n = 0;
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
		/// Returns the maximum of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null or
		/// the array index parameters are not valid.</p>
		/// <p>
		/// <ul>
		/// <li>The result is <code>NaN</code> iff all values are <code>NaN</code>
		/// (i.e. <code>NaN</code> values have no impact on the value of the statistic).</li>
		/// <li>If any of the values equals <code>Double.POSITIVE_INFINITY</code>,
		/// the result is <code>Double.POSITIVE_INFINITY.</code></li>
		/// </ul></p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the maximum of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		///  parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double[] values, int begin, int length)
		{
			double max = double.NaN;
			if (test(values, begin, length))
			{
				max = values[begin];
				for (int i = begin; i < begin + length; i++)
				{
					if (!double.IsNaN(values[i]))
					{
						max = (max > values[i]) ? max : values[i];
					}
				}
			}
			return max;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override Max copy()
		{
			Max result = new Max();
			// No try-catch or advertised exception because args are non-null
			copy(this, result);
			return result;
		}

		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> Max to copy </param>
		/// <param name="dest"> Max to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(Max source, Max dest) throws mathlib.exception.NullArgumentException
		public static void copy(Max source, Max dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			dest.Data = source.DataRef;
			dest.n = source.n;
			dest.value = source.value;
		}
	}

}
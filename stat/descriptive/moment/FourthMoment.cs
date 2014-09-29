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
namespace mathlib.stat.descriptive.moment
{

	using NullArgumentException = mathlib.exception.NullArgumentException;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// Computes a statistic related to the Fourth Central Moment.  Specifically,
	/// what is computed is the sum of
	/// <p>
	/// (x_i - xbar) ^ 4, </p>
	/// <p>
	/// where the x_i are the
	/// sample observations and xbar is the sample mean. </p>
	/// <p>
	/// The following recursive updating formula is used: </p>
	/// <p>
	/// Let <ul>
	/// <li> dev = (current obs - previous mean) </li>
	/// <li> m2 = previous value of <seealso cref="SecondMoment"/> </li>
	/// <li> m2 = previous value of <seealso cref="ThirdMoment"/> </li>
	/// <li> n = number of observations (including current obs) </li>
	/// </ul>
	/// Then </p>
	/// <p>
	/// new value = old value - 4 * (dev/n) * m3 + 6 * (dev/n)^2 * m2 + <br>
	/// [n^2 - 3 * (n-1)] * dev^4 * (n-1) / n^3 </p>
	/// <p>
	/// Returns <code>Double.NaN</code> if no data values have been added and
	/// returns <code>0</code> if there is just one value in the data set. </p>
	/// <p>
	/// <strong>Note that this implementation is not synchronized.</strong> If
	/// multiple threads access an instance of this class concurrently, and at least
	/// one of the threads invokes the <code>increment()</code> or
	/// <code>clear()</code> method, it must be synchronized externally. </p>
	/// 
	/// @version $Id: FourthMoment.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	internal class FourthMoment : ThirdMoment
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 4763990447117157611L;

		/// <summary>
		/// fourth moment of values that have been added </summary>
		private double m4;

		/// <summary>
		/// Create a FourthMoment instance
		/// </summary>
		public FourthMoment() : base()
		{
			m4 = double.NaN;
		}

		/// <summary>
		/// Copy constructor, creates a new {@code FourthMoment} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code FourthMoment} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FourthMoment(FourthMoment original) throws mathlib.exception.NullArgumentException
		 public FourthMoment(FourthMoment original) : base()
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
			if (n < 1)
			{
				m4 = 0.0;
				m3 = 0.0;
				m2 = 0.0;
				m1 = 0.0;
			}

			double prevM3 = m3;
			double prevM2 = m2;

			base.increment(d);

			double n0 = n;

			m4 = m4 - 4.0 * nDev * prevM3 + 6.0 * nDevSq * prevM2 + ((n0 * n0) - 3 * (n0 - 1)) * (nDevSq * nDevSq * (n0 - 1) * n0);
		 }

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double Result
		{
			get
			{
				return m4;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override void clear()
		{
			base.clear();
			m4 = double.NaN;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override FourthMoment copy()
		{
			FourthMoment result = new FourthMoment();
			// No try-catch or advertised exception because args are guaranteed non-null
			copy(this, result);
			return result;
		}

		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> FourthMoment to copy </param>
		/// <param name="dest"> FourthMoment to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(FourthMoment source, FourthMoment dest) throws mathlib.exception.NullArgumentException
		public static void copy(FourthMoment source, FourthMoment dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			ThirdMoment.copy(source, dest);
			dest.m4 = source.m4;
		}
	}

}
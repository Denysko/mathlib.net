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
namespace org.apache.commons.math3.stat.descriptive.moment
{

	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using MathUtils = org.apache.commons.math3.util.MathUtils;


	/// <summary>
	/// Computes a statistic related to the Third Central Moment.  Specifically,
	/// what is computed is the sum of cubed deviations from the sample mean.
	/// <p>
	/// The following recursive updating formula is used:</p>
	/// <p>
	/// Let <ul>
	/// <li> dev = (current obs - previous mean) </li>
	/// <li> m2 = previous value of <seealso cref="SecondMoment"/> </li>
	/// <li> n = number of observations (including current obs) </li>
	/// </ul>
	/// Then</p>
	/// <p>
	/// new value = old value - 3 * (dev/n) * m2 + (n-1) * (n -2) * (dev^3/n^2)</p>
	/// <p>
	/// Returns <code>Double.NaN</code> if no data values have been added and
	/// returns <code>0</code> if there is just one value in the data set.</p>
	/// <p>
	/// <strong>Note that this implementation is not synchronized.</strong> If
	/// multiple threads access an instance of this class concurrently, and at least
	/// one of the threads invokes the <code>increment()</code> or
	/// <code>clear()</code> method, it must be synchronized externally.</p>
	/// 
	/// @version $Id: ThirdMoment.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	internal class ThirdMoment : SecondMoment
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -7818711964045118679L;

		/// <summary>
		/// third moment of values that have been added </summary>
		protected internal double m3;

		 /// <summary>
		 /// Square of deviation of most recently added value from previous first
		 /// moment, normalized by previous sample size.  Retained to prevent
		 /// repeated computation in higher order moments.  nDevSq = nDev * nDev.
		 /// </summary>
		protected internal double nDevSq;

		/// <summary>
		/// Create a FourthMoment instance
		/// </summary>
		public ThirdMoment() : base()
		{
			m3 = double.NaN;
			nDevSq = double.NaN;
		}

		/// <summary>
		/// Copy constructor, creates a new {@code ThirdMoment} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code ThirdMoment} instance to copy </param>
		/// <exception cref="NullArgumentException"> if orginal is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ThirdMoment(ThirdMoment original) throws org.apache.commons.math3.exception.NullArgumentException
		public ThirdMoment(ThirdMoment original)
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
				m3 = m2 = m1 = 0.0;
			}

			double prevM2 = m2;
			base.increment(d);
			nDevSq = nDev * nDev;
			double n0 = n;
			m3 = m3 - 3.0 * nDev * prevM2 + (n0 - 1) * (n0 - 2) * nDevSq * dev;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double Result
		{
			get
			{
				return m3;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override void clear()
		{
			base.clear();
			m3 = double.NaN;
			nDevSq = double.NaN;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override ThirdMoment copy()
		{
			ThirdMoment result = new ThirdMoment();
			// No try-catch or advertised exception because args are guaranteed non-null
			copy(this, result);
			return result;
		}

		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> ThirdMoment to copy </param>
		/// <param name="dest"> ThirdMoment to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(ThirdMoment source, ThirdMoment dest) throws org.apache.commons.math3.exception.NullArgumentException
		public static void copy(ThirdMoment source, ThirdMoment dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			SecondMoment.copy(source, dest);
			dest.m3 = source.m3;
			dest.nDevSq = source.nDevSq;
		}

	}

}
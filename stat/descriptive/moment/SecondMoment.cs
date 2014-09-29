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
	/// Computes a statistic related to the Second Central Moment.  Specifically,
	/// what is computed is the sum of squared deviations from the sample mean.
	/// <p>
	/// The following recursive updating formula is used:</p>
	/// <p>
	/// Let <ul>
	/// <li> dev = (current obs - previous mean) </li>
	/// <li> n = number of observations (including current obs) </li>
	/// </ul>
	/// Then</p>
	/// <p>
	/// new value = old value + dev^2 * (n -1) / n.</p>
	/// <p>
	/// Returns <code>Double.NaN</code> if no data values have been added and
	/// returns <code>0</code> if there is just one value in the data set.</p>
	/// <p>
	/// <strong>Note that this implementation is not synchronized.</strong> If
	/// multiple threads access an instance of this class concurrently, and at least
	/// one of the threads invokes the <code>increment()</code> or
	/// <code>clear()</code> method, it must be synchronized externally.</p>
	/// 
	/// @version $Id: SecondMoment.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class SecondMoment : FirstMoment
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 3942403127395076445L;

		/// <summary>
		/// second moment of values that have been added </summary>
		protected internal double m2;

		/// <summary>
		/// Create a SecondMoment instance
		/// </summary>
		public SecondMoment() : base()
		{
			m2 = double.NaN;
		}

		/// <summary>
		/// Copy constructor, creates a new {@code SecondMoment} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code SecondMoment} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SecondMoment(SecondMoment original) throws mathlib.exception.NullArgumentException
		public SecondMoment(SecondMoment original) : base(original)
		{
			this.m2 = original.m2;
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
				m1 = m2 = 0.0;
			}
			base.increment(d);
			m2 += ((double) n - 1) * dev * nDev;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override void clear()
		{
			base.clear();
			m2 = double.NaN;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double Result
		{
			get
			{
				return m2;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override SecondMoment copy()
		{
			SecondMoment result = new SecondMoment();
			// no try-catch or advertised NAE because args are guaranteed non-null
			copy(this, result);
			return result;
		}

		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> SecondMoment to copy </param>
		/// <param name="dest"> SecondMoment to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(SecondMoment source, SecondMoment dest) throws mathlib.exception.NullArgumentException
		public static void copy(SecondMoment source, SecondMoment dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			FirstMoment.copy(source, dest);
			dest.m2 = source.m2;
		}

	}

}
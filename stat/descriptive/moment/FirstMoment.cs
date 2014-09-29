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
	/// Computes the first moment (arithmetic mean).  Uses the definitional formula:
	/// <p>
	/// mean = sum(x_i) / n </p>
	/// <p>
	/// where <code>n</code> is the number of observations. </p>
	/// <p>
	/// To limit numeric errors, the value of the statistic is computed using the
	/// following recursive updating algorithm: </p>
	/// <p>
	/// <ol>
	/// <li>Initialize <code>m = </code> the first value</li>
	/// <li>For each additional value, update using <br>
	///   <code>m = m + (new value - m) / (number of observations)</code></li>
	/// </ol></p>
	/// <p>
	///  Returns <code>Double.NaN</code> if the dataset is empty.</p>
	/// <p>
	/// <strong>Note that this implementation is not synchronized.</strong> If
	/// multiple threads access an instance of this class concurrently, and at least
	/// one of the threads invokes the <code>increment()</code> or
	/// <code>clear()</code> method, it must be synchronized externally.</p>
	/// 
	/// @version $Id: FirstMoment.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	internal class FirstMoment : AbstractStorelessUnivariateStatistic
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 6112755307178490473L;


		/// <summary>
		/// Count of values that have been added </summary>
		protected internal long n;

		/// <summary>
		/// First moment of values that have been added </summary>
		protected internal double m1;

		/// <summary>
		/// Deviation of most recently added value from previous first moment.
		/// Retained to prevent repeated computation in higher order moments.
		/// </summary>
		protected internal double dev;

		/// <summary>
		/// Deviation of most recently added value from previous first moment,
		/// normalized by previous sample size.  Retained to prevent repeated
		/// computation in higher order moments
		/// </summary>
		protected internal double nDev;

		/// <summary>
		/// Create a FirstMoment instance
		/// </summary>
		public FirstMoment()
		{
			n = 0;
			m1 = double.NaN;
			dev = double.NaN;
			nDev = double.NaN;
		}

		/// <summary>
		/// Copy constructor, creates a new {@code FirstMoment} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code FirstMoment} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FirstMoment(FirstMoment original) throws mathlib.exception.NullArgumentException
		 public FirstMoment(FirstMoment original) : base()
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
			if (n == 0)
			{
				m1 = 0.0;
			}
			n++;
			double n0 = n;
			dev = d - m1;
			nDev = dev / n0;
			m1 += nDev;
		 }

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override void clear()
		{
			m1 = double.NaN;
			n = 0;
			dev = double.NaN;
			nDev = double.NaN;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double Result
		{
			get
			{
				return m1;
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
		public override FirstMoment copy()
		{
			FirstMoment result = new FirstMoment();
			// No try-catch or advertised exception because args are guaranteed non-null
			copy(this, result);
			return result;
		}

		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> FirstMoment to copy </param>
		/// <param name="dest"> FirstMoment to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(FirstMoment source, FirstMoment dest) throws mathlib.exception.NullArgumentException
		public static void copy(FirstMoment source, FirstMoment dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			dest.Data = source.DataRef;
			dest.n = source.n;
			dest.m1 = source.m1;
			dest.dev = source.dev;
			dest.nDev = source.nDev;
		}
	}

}
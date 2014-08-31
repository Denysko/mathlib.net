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

	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using MathIllegalStateException = org.apache.commons.math3.exception.MathIllegalStateException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using SumOfLogs = org.apache.commons.math3.stat.descriptive.summary.SumOfLogs;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// Returns the <a href="http://www.xycoon.com/geometric_mean.htm">
	/// geometric mean </a> of the available values.
	/// <p>
	/// Uses a <seealso cref="SumOfLogs"/> instance to compute sum of logs and returns
	/// <code> exp( 1/n  (sum of logs) ).</code>  Therefore, </p>
	/// <ul>
	/// <li>If any of values are < 0, the result is <code>NaN.</code></li>
	/// <li>If all values are non-negative and less than
	/// <code>Double.POSITIVE_INFINITY</code>,  but at least one value is 0, the
	/// result is <code>0.</code></li>
	/// <li>If both <code>Double.POSITIVE_INFINITY</code> and
	/// <code>Double.NEGATIVE_INFINITY</code> are among the values, the result is
	/// <code>NaN.</code></li>
	/// </ul> </p>
	/// <p>
	/// <strong>Note that this implementation is not synchronized.</strong> If
	/// multiple threads access an instance of this class concurrently, and at least
	/// one of the threads invokes the <code>increment()</code> or
	/// <code>clear()</code> method, it must be synchronized externally.</p>
	/// 
	/// 
	/// @version $Id: GeometricMean.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class GeometricMean : AbstractStorelessUnivariateStatistic
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -8178734905303459453L;

		/// <summary>
		/// Wrapped SumOfLogs instance </summary>
		private StorelessUnivariateStatistic sumOfLogs;

		/// <summary>
		/// Create a GeometricMean instance
		/// </summary>
		public GeometricMean()
		{
			sumOfLogs = new SumOfLogs();
		}

		/// <summary>
		/// Copy constructor, creates a new {@code GeometricMean} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code GeometricMean} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GeometricMean(GeometricMean original) throws org.apache.commons.math3.exception.NullArgumentException
		public GeometricMean(GeometricMean original) : base()
		{
			copy(original, this);
		}

		/// <summary>
		/// Create a GeometricMean instance using the given SumOfLogs instance </summary>
		/// <param name="sumOfLogs"> sum of logs instance to use for computation </param>
		public GeometricMean(SumOfLogs sumOfLogs)
		{
			this.sumOfLogs = sumOfLogs;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override GeometricMean copy()
		{
			GeometricMean result = new GeometricMean();
			// no try-catch or advertised exception because args guaranteed non-null
			copy(this, result);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void increment(final double d)
		public override void increment(double d)
		{
			sumOfLogs.increment(d);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double Result
		{
			get
			{
				if (sumOfLogs.N > 0)
				{
					return FastMath.exp(sumOfLogs.Result / sumOfLogs.N);
				}
				else
				{
					return double.NaN;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override void clear()
		{
			sumOfLogs.clear();
		}

		/// <summary>
		/// Returns the geometric mean of the entries in the specified portion
		/// of the input array.
		/// <p>
		/// See <seealso cref="GeometricMean"/> for details on the computing algorithm.</p>
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values"> input array containing the values </param>
		/// <param name="begin"> first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the geometric mean or Double.NaN if length = 0 or
		/// any of the values are &lt;= 0. </returns>
		/// <exception cref="MathIllegalArgumentException"> if the input array is null or the array
		/// index parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values, final int begin, final int length) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double[] values, int begin, int length)
		{
			return FastMath.exp(sumOfLogs.evaluate(values, begin, length) / length);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override long N
		{
			get
			{
				return sumOfLogs.N;
			}
		}

		/// <summary>
		/// <p>Sets the implementation for the sum of logs.</p>
		/// <p>This method must be activated before any data has been added - i.e.,
		/// before <seealso cref="#increment(double) increment"/> has been used to add data;
		/// otherwise an IllegalStateException will be thrown.</p>
		/// </summary>
		/// <param name="sumLogImpl"> the StorelessUnivariateStatistic instance to use
		/// for computing the log sum </param>
		/// <exception cref="MathIllegalStateException"> if data has already been added
		///  (i.e if n > 0) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSumLogImpl(org.apache.commons.math3.stat.descriptive.StorelessUnivariateStatistic sumLogImpl) throws org.apache.commons.math3.exception.MathIllegalStateException
		public virtual StorelessUnivariateStatistic SumLogImpl
		{
			set
			{
				checkEmpty();
				this.sumOfLogs = value;
			}
			get
			{
				return sumOfLogs;
			}
		}


		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> GeometricMean to copy </param>
		/// <param name="dest"> GeometricMean to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(GeometricMean source, GeometricMean dest) throws org.apache.commons.math3.exception.NullArgumentException
		public static void copy(GeometricMean source, GeometricMean dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			dest.Data = source.DataRef;
			dest.sumOfLogs = source.sumOfLogs.copy();
		}


		/// <summary>
		/// Throws MathIllegalStateException if n > 0. </summary>
		/// <exception cref="MathIllegalStateException"> if data has been added to this statistic </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkEmpty() throws org.apache.commons.math3.exception.MathIllegalStateException
		private void checkEmpty()
		{
			if (N > 0)
			{
				throw new MathIllegalStateException(LocalizedFormats.VALUES_ADDED_BEFORE_CONFIGURING_STATISTIC, N);
			}
		}

	}

}
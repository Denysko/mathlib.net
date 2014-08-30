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
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathUtils = org.apache.commons.math3.util.MathUtils;


	/// <summary>
	/// Computes the Kurtosis of the available values.
	/// <p>
	/// We use the following (unbiased) formula to define kurtosis:</p>
	///  <p>
	///  kurtosis = { [n(n+1) / (n -1)(n - 2)(n-3)] sum[(x_i - mean)^4] / std^4 } - [3(n-1)^2 / (n-2)(n-3)]
	///  </p><p>
	///  where n is the number of values, mean is the <seealso cref="Mean"/> and std is the
	/// <seealso cref="StandardDeviation"/></p>
	/// <p>
	///  Note that this statistic is undefined for n < 4.  <code>Double.Nan</code>
	///  is returned when there is not sufficient data to compute the statistic.</p>
	/// <p>
	/// <strong>Note that this implementation is not synchronized.</strong> If
	/// multiple threads access an instance of this class concurrently, and at least
	/// one of the threads invokes the <code>increment()</code> or
	/// <code>clear()</code> method, it must be synchronized externally.</p>
	/// 
	/// @version $Id: Kurtosis.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class Kurtosis : AbstractStorelessUnivariateStatistic
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 2784465764798260919L;

		/// <summary>
		///Fourth Moment on which this statistic is based </summary>
		protected internal FourthMoment moment;

		/// <summary>
		/// Determines whether or not this statistic can be incremented or cleared.
		/// <p>
		/// Statistics based on (constructed from) external moments cannot
		/// be incremented or cleared.</p>
		/// </summary>
		protected internal bool incMoment;

		/// <summary>
		/// Construct a Kurtosis
		/// </summary>
		public Kurtosis()
		{
			incMoment = true;
			moment = new FourthMoment();
		}

		/// <summary>
		/// Construct a Kurtosis from an external moment
		/// </summary>
		/// <param name="m4"> external Moment </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Kurtosis(final FourthMoment m4)
		public Kurtosis(FourthMoment m4)
		{
			incMoment = false;
			this.moment = m4;
		}

		/// <summary>
		/// Copy constructor, creates a new {@code Kurtosis} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code Kurtosis} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Kurtosis(Kurtosis original) throws org.apache.commons.math3.exception.NullArgumentException
		public Kurtosis(Kurtosis original)
		{
			copy(original, this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>Note that when <seealso cref="#Kurtosis(FourthMoment)"/> is used to
		/// create a Variance, this method does nothing. In that case, the
		/// FourthMoment should be incremented directly.</p>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void increment(final double d)
		public override void increment(double d)
		{
			if (incMoment)
			{
				moment.increment(d);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double Result
		{
			get
			{
				double kurtosis = double.NaN;
				if (moment.N > 3)
				{
					double variance = moment.m2 / (moment.n - 1);
						if (moment.n <= 3 || variance < 10E-20)
						{
							kurtosis = 0.0;
						}
						else
						{
							double n = moment.n;
							kurtosis = (n * (n + 1) * moment.Result - 3 * moment.m2 * moment.m2 * (n - 1)) / ((n - 1) * (n - 2) * (n - 3) * variance * variance);
						}
				}
				return kurtosis;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override void clear()
		{
			if (incMoment)
			{
				moment.clear();
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override long N
		{
			get
			{
				return moment.N;
			}
		}

		/* UnvariateStatistic Approach  */

		/// <summary>
		/// Returns the kurtosis of the entries in the specified portion of the
		/// input array.
		/// <p>
		/// See <seealso cref="Kurtosis"/> for details on the computing algorithm.</p>
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the kurtosis of the values or Double.NaN if length is less than 4 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the input array is null or the array
		/// index parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values,final int begin, final int length) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double[] values, int begin, int length)
		{
			// Initialize the kurtosis
			double kurt = double.NaN;

			if (test(values, begin, length) && length > 3)
			{

				// Compute the mean and standard deviation
				Variance variance = new Variance();
				variance.incrementAll(values, begin, length);
				double mean = variance.moment.m1;
				double stdDev = FastMath.sqrt(variance.Result);

				// Sum the ^4 of the distance from the mean divided by the
				// standard deviation
				double accum3 = 0.0;
				for (int i = begin; i < begin + length; i++)
				{
					accum3 += FastMath.pow(values[i] - mean, 4.0);
				}
				accum3 /= FastMath.pow(stdDev, 4.0d);

				// Get N
				double n0 = length;

				double coefficientOne = (n0 * (n0 + 1)) / ((n0 - 1) * (n0 - 2) * (n0 - 3));
				double termTwo = (3 * FastMath.pow(n0 - 1, 2.0)) / ((n0 - 2) * (n0 - 3));

				// Calculate kurtosis
				kurt = (coefficientOne * accum3) - termTwo;
			}
			return kurt;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override Kurtosis copy()
		{
			Kurtosis result = new Kurtosis();
			// No try-catch because args are guaranteed non-null
			copy(this, result);
			return result;
		}

		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> Kurtosis to copy </param>
		/// <param name="dest"> Kurtosis to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(Kurtosis source, Kurtosis dest) throws org.apache.commons.math3.exception.NullArgumentException
		public static void copy(Kurtosis source, Kurtosis dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			dest.Data = source.DataRef;
			dest.moment = source.moment.copy();
			dest.incMoment = source.incMoment;
		}

	}

}
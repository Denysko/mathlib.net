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
	/// Computes the skewness of the available values.
	/// <p>
	/// We use the following (unbiased) formula to define skewness:</p>
	/// <p>
	/// skewness = [n / (n -1) (n - 2)] sum[(x_i - mean)^3] / std^3 </p>
	/// <p>
	/// where n is the number of values, mean is the <seealso cref="Mean"/> and std is the
	/// <seealso cref="StandardDeviation"/> </p>
	/// <p>
	/// <strong>Note that this implementation is not synchronized.</strong> If
	/// multiple threads access an instance of this class concurrently, and at least
	/// one of the threads invokes the <code>increment()</code> or
	/// <code>clear()</code> method, it must be synchronized externally. </p>
	/// 
	/// @version $Id: Skewness.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class Skewness : AbstractStorelessUnivariateStatistic
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 7101857578996691352L;

		/// <summary>
		/// Third moment on which this statistic is based </summary>
		protected internal ThirdMoment moment = null;

		 /// <summary>
		 /// Determines whether or not this statistic can be incremented or cleared.
		 /// <p>
		 /// Statistics based on (constructed from) external moments cannot
		 /// be incremented or cleared.</p>
		 /// </summary>
		protected internal bool incMoment;

		/// <summary>
		/// Constructs a Skewness
		/// </summary>
		public Skewness()
		{
			incMoment = true;
			moment = new ThirdMoment();
		}

		/// <summary>
		/// Constructs a Skewness with an external moment </summary>
		/// <param name="m3"> external moment </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Skewness(final ThirdMoment m3)
		public Skewness(ThirdMoment m3)
		{
			incMoment = false;
			this.moment = m3;
		}

		/// <summary>
		/// Copy constructor, creates a new {@code Skewness} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code Skewness} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Skewness(Skewness original) throws org.apache.commons.math3.exception.NullArgumentException
		public Skewness(Skewness original)
		{
			copy(original, this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>Note that when <seealso cref="#Skewness(ThirdMoment)"/> is used to
		/// create a Skewness, this method does nothing. In that case, the
		/// ThirdMoment should be incremented directly.</p>
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
		/// Returns the value of the statistic based on the values that have been added.
		/// <p>
		/// See <seealso cref="Skewness"/> for the definition used in the computation.</p>
		/// </summary>
		/// <returns> the skewness of the available values. </returns>
		public override double Result
		{
			get
			{
    
				if (moment.n < 3)
				{
					return double.NaN;
				}
				double variance = moment.m2 / (moment.n - 1);
				if (variance < 10E-20)
				{
					return 0.0d;
				}
				else
				{
					double n0 = moment.N;
					return (n0 * moment.m3) / ((n0 - 1) * (n0 - 2) * FastMath.sqrt(variance) * variance);
				}
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
		/// Returns the Skewness of the entries in the specifed portion of the
		/// input array.
		/// <p>
		/// See <seealso cref="Skewness"/> for the definition used in the computation.</p>
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> the index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the skewness of the values or Double.NaN if length is less than
		/// 3 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		///  parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values,final int begin, final int length) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double[] values, int begin, int length)
		{

			// Initialize the skewness
			double skew = double.NaN;

			if (test(values, begin, length) && length > 2)
			{
				Mean mean = new Mean();
				// Get the mean and the standard deviation
				double m = mean.evaluate(values, begin, length);

				// Calc the std, this is implemented here instead
				// of using the standardDeviation method eliminate
				// a duplicate pass to get the mean
				double accum = 0.0;
				double accum2 = 0.0;
				for (int i = begin; i < begin + length; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d = values[i] - m;
					double d = values[i] - m;
					accum += d * d;
					accum2 += d;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double variance = (accum - (accum2 * accum2 / length)) / (length - 1);
				double variance = (accum - (accum2 * accum2 / length)) / (length - 1);

				double accum3 = 0.0;
				for (int i = begin; i < begin + length; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d = values[i] - m;
					double d = values[i] - m;
					accum3 += d * d * d;
				}
				accum3 /= variance * FastMath.sqrt(variance);

				// Get N
				double n0 = length;

				// Calculate skewness
				skew = (n0 / ((n0 - 1) * (n0 - 2))) * accum3;
			}
			return skew;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override Skewness copy()
		{
			Skewness result = new Skewness();
			// No try-catch or advertised exception because args are guaranteed non-null
			copy(this, result);
			return result;
		}

		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> Skewness to copy </param>
		/// <param name="dest"> Skewness to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(Skewness source, Skewness dest) throws org.apache.commons.math3.exception.NullArgumentException
		public static void copy(Skewness source, Skewness dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			dest.Data = source.DataRef;
			dest.moment = new ThirdMoment(source.moment.copy());
			dest.incMoment = source.incMoment;
		}
	}

}
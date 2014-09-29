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

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using Sum = mathlib.stat.descriptive.summary.Sum;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// <p>Computes the arithmetic mean of a set of values. Uses the definitional
	/// formula:</p>
	/// <p>
	/// mean = sum(x_i) / n
	/// </p>
	/// <p>where <code>n</code> is the number of observations.
	/// </p>
	/// <p>When <seealso cref="#increment(double)"/> is used to add data incrementally from a
	/// stream of (unstored) values, the value of the statistic that
	/// <seealso cref="#getResult()"/> returns is computed using the following recursive
	/// updating algorithm: </p>
	/// <ol>
	/// <li>Initialize <code>m = </code> the first value</li>
	/// <li>For each additional value, update using <br>
	///   <code>m = m + (new value - m) / (number of observations)</code></li>
	/// </ol>
	/// <p> If <seealso cref="#evaluate(double[])"/> is used to compute the mean of an array
	/// of stored values, a two-pass, corrected algorithm is used, starting with
	/// the definitional formula computed using the array of stored values and then
	/// correcting this by adding the mean deviation of the data values from the
	/// arithmetic mean. See, e.g. "Comparison of Several Algorithms for Computing
	/// Sample Means and Variances," Robert F. Ling, Journal of the American
	/// Statistical Association, Vol. 69, No. 348 (Dec., 1974), pp. 859-866. </p>
	/// <p>
	///  Returns <code>Double.NaN</code> if the dataset is empty.
	/// </p>
	/// <strong>Note that this implementation is not synchronized.</strong> If
	/// multiple threads access an instance of this class concurrently, and at least
	/// one of the threads invokes the <code>increment()</code> or
	/// <code>clear()</code> method, it must be synchronized externally.
	/// 
	/// @version $Id: Mean.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class Mean : AbstractStorelessUnivariateStatistic, WeightedEvaluation
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -1296043746617791564L;

		/// <summary>
		/// First moment on which this statistic is based. </summary>
		protected internal FirstMoment moment;

		/// <summary>
		/// Determines whether or not this statistic can be incremented or cleared.
		/// <p>
		/// Statistics based on (constructed from) external moments cannot
		/// be incremented or cleared.</p>
		/// </summary>
		protected internal bool incMoment;

		/// <summary>
		/// Constructs a Mean. </summary>
		public Mean()
		{
			incMoment = true;
			moment = new FirstMoment();
		}

		/// <summary>
		/// Constructs a Mean with an External Moment.
		/// </summary>
		/// <param name="m1"> the moment </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Mean(final FirstMoment m1)
		public Mean(FirstMoment m1)
		{
			this.moment = m1;
			incMoment = false;
		}

		/// <summary>
		/// Copy constructor, creates a new {@code Mean} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code Mean} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Mean(Mean original) throws mathlib.exception.NullArgumentException
		public Mean(Mean original)
		{
			copy(original, this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>Note that when <seealso cref="#Mean(FirstMoment)"/> is used to
		/// create a Mean, this method does nothing. In that case, the
		/// FirstMoment should be incremented directly.</p>
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
		public override double Result
		{
			get
			{
				return moment.m1;
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
		/// Returns the arithmetic mean of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the array is null.</p>
		/// <p>
		/// See <seealso cref="Mean"/> for details on the computing algorithm.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the mean of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		///  parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values,final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double[] values, int begin, int length)
		{
			if (test(values, begin, length))
			{
				Sum sum = new Sum();
				double sampleSize = length;

				// Compute initial estimate using definitional formula
				double xbar = sum.evaluate(values, begin, length) / sampleSize;

				// Compute correction factor in second pass
				double correction = 0;
				for (int i = begin; i < begin + length; i++)
				{
					correction += values[i] - xbar;
				}
				return xbar + (correction / sampleSize);
			}
			return double.NaN;
		}

		/// <summary>
		/// Returns the weighted arithmetic mean of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if either array is null.</p>
		/// <p>
		/// See <seealso cref="Mean"/> for details on the computing algorithm. The two-pass algorithm
		/// described above is used here, with weights applied in computing both the original
		/// estimate and the correction factor.</p>
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if any of the following are true:
		/// <ul><li>the values array is null</li>
		///     <li>the weights array is null</li>
		///     <li>the weights array does not have the same length as the values array</li>
		///     <li>the weights array contains one or more infinite values</li>
		///     <li>the weights array contains one or more NaN values</li>
		///     <li>the weights array contains negative values</li>
		///     <li>the start and length arguments do not determine a valid array</li>
		/// </ul></p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="weights"> the weights array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the mean of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the parameters are not valid
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double[] weights, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values, double[] weights, int begin, int length)
		{
			if (test(values, weights, begin, length))
			{
				Sum sum = new Sum();

				// Compute initial estimate using definitional formula
				double sumw = sum.evaluate(weights,begin,length);
				double xbarw = sum.evaluate(values, weights, begin, length) / sumw;

				// Compute correction factor in second pass
				double correction = 0;
				for (int i = begin; i < begin + length; i++)
				{
					correction += weights[i] * (values[i] - xbarw);
				}
				return xbarw + (correction / sumw);
			}
			return double.NaN;
		}

		/// <summary>
		/// Returns the weighted arithmetic mean of the entries in the input array.
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if either array is null.</p>
		/// <p>
		/// See <seealso cref="Mean"/> for details on the computing algorithm. The two-pass algorithm
		/// described above is used here, with weights applied in computing both the original
		/// estimate and the correction factor.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if any of the following are true:
		/// <ul><li>the values array is null</li>
		///     <li>the weights array is null</li>
		///     <li>the weights array does not have the same length as the values array</li>
		///     <li>the weights array contains one or more infinite values</li>
		///     <li>the weights array contains one or more NaN values</li>
		///     <li>the weights array contains negative values</li>
		/// </ul></p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="weights"> the weights array </param>
		/// <returns> the mean of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the parameters are not valid
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double[] weights) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values, double[] weights)
		{
			return evaluate(values, weights, 0, values.Length);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override Mean copy()
		{
			Mean result = new Mean();
			// No try-catch or advertised exception because args are guaranteed non-null
			copy(this, result);
			return result;
		}


		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> Mean to copy </param>
		/// <param name="dest"> Mean to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(Mean source, Mean dest) throws mathlib.exception.NullArgumentException
		public static void copy(Mean source, Mean dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			dest.Data = source.DataRef;
			dest.incMoment = source.incMoment;
			dest.moment = source.moment.copy();
		}
	}

}
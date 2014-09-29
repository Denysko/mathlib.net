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
	using FastMath = mathlib.util.FastMath;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// Computes the sample standard deviation.  The standard deviation
	/// is the positive square root of the variance.  This implementation wraps a
	/// <seealso cref="Variance"/> instance.  The <code>isBiasCorrected</code> property of the
	/// wrapped Variance instance is exposed, so that this class can be used to
	/// compute both the "sample standard deviation" (the square root of the
	/// bias-corrected "sample variance") or the "population standard deviation"
	/// (the square root of the non-bias-corrected "population variance"). See
	/// <seealso cref="Variance"/> for more information.
	/// <p>
	/// <strong>Note that this implementation is not synchronized.</strong> If
	/// multiple threads access an instance of this class concurrently, and at least
	/// one of the threads invokes the <code>increment()</code> or
	/// <code>clear()</code> method, it must be synchronized externally.</p>
	/// 
	/// @version $Id: StandardDeviation.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class StandardDeviation : AbstractStorelessUnivariateStatistic
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 5728716329662425188L;

		/// <summary>
		/// Wrapped Variance instance </summary>
		private Variance variance = null;

		/// <summary>
		/// Constructs a StandardDeviation.  Sets the underlying <seealso cref="Variance"/>
		/// instance's <code>isBiasCorrected</code> property to true.
		/// </summary>
		public StandardDeviation()
		{
			variance = new Variance();
		}

		/// <summary>
		/// Constructs a StandardDeviation from an external second moment.
		/// </summary>
		/// <param name="m2"> the external moment </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public StandardDeviation(final SecondMoment m2)
		public StandardDeviation(SecondMoment m2)
		{
			variance = new Variance(m2);
		}

		/// <summary>
		/// Copy constructor, creates a new {@code StandardDeviation} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code StandardDeviation} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public StandardDeviation(StandardDeviation original) throws mathlib.exception.NullArgumentException
		public StandardDeviation(StandardDeviation original)
		{
			copy(original, this);
		}

		/// <summary>
		/// Contructs a StandardDeviation with the specified value for the
		/// <code>isBiasCorrected</code> property.  If this property is set to
		/// <code>true</code>, the <seealso cref="Variance"/> used in computing results will
		/// use the bias-corrected, or "sample" formula.  See <seealso cref="Variance"/> for
		/// details.
		/// </summary>
		/// <param name="isBiasCorrected">  whether or not the variance computation will use
		/// the bias-corrected formula </param>
		public StandardDeviation(bool isBiasCorrected)
		{
			variance = new Variance(isBiasCorrected);
		}

		/// <summary>
		/// Contructs a StandardDeviation with the specified value for the
		/// <code>isBiasCorrected</code> property and the supplied external moment.
		/// If <code>isBiasCorrected</code> is set to <code>true</code>, the
		/// <seealso cref="Variance"/> used in computing results will use the bias-corrected,
		/// or "sample" formula.  See <seealso cref="Variance"/> for details.
		/// </summary>
		/// <param name="isBiasCorrected">  whether or not the variance computation will use
		/// the bias-corrected formula </param>
		/// <param name="m2"> the external moment </param>
		public StandardDeviation(bool isBiasCorrected, SecondMoment m2)
		{
			variance = new Variance(isBiasCorrected, m2);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void increment(final double d)
		public override void increment(double d)
		{
			variance.increment(d);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override long N
		{
			get
			{
				return variance.N;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double Result
		{
			get
			{
				return FastMath.sqrt(variance.Result);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override void clear()
		{
			variance.clear();
		}

		/// <summary>
		/// Returns the Standard Deviation of the entries in the input array, or
		/// <code>Double.NaN</code> if the array is empty.
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		/// <p>
		/// Does not change the internal state of the statistic.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <returns> the standard deviation of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double[] values)
		{
			return FastMath.sqrt(variance.evaluate(values));
		}

		/// <summary>
		/// Returns the Standard Deviation of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample. </p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		/// <p>
		/// Does not change the internal state of the statistic.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the standard deviation of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		///  parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double[] values, int begin, int length)
		{
		   return FastMath.sqrt(variance.evaluate(values, begin, length));
		}

		/// <summary>
		/// Returns the Standard Deviation of the entries in the specified portion of
		/// the input array, using the precomputed mean value.  Returns
		/// <code>Double.NaN</code> if the designated subarray is empty.
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// The formula used assumes that the supplied mean value is the arithmetic
		/// mean of the sample data, not a known population parameter.  This method
		/// is supplied only to save computation when the mean has already been
		/// computed.</p>
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the array is null.</p>
		/// <p>
		/// Does not change the internal state of the statistic.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="mean"> the precomputed mean value </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the standard deviation of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		///  parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double mean, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values, double mean, int begin, int length)
		{
			return FastMath.sqrt(variance.evaluate(values, mean, begin, length));
		}

		/// <summary>
		/// Returns the Standard Deviation of the entries in the input array, using
		/// the precomputed mean value.  Returns
		/// <code>Double.NaN</code> if the designated subarray is empty.
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// The formula used assumes that the supplied mean value is the arithmetic
		/// mean of the sample data, not a known population parameter.  This method
		/// is supplied only to save computation when the mean has already been
		/// computed.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		/// <p>
		/// Does not change the internal state of the statistic.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="mean"> the precomputed mean value </param>
		/// <returns> the standard deviation of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double mean) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values, double mean)
		{
			return FastMath.sqrt(variance.evaluate(values, mean));
		}

		/// <returns> Returns the isBiasCorrected. </returns>
		public virtual bool BiasCorrected
		{
			get
			{
				return variance.BiasCorrected;
			}
			set
			{
				variance.BiasCorrected = value;
			}
		}


		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override StandardDeviation copy()
		{
			StandardDeviation result = new StandardDeviation();
			// No try-catch or advertised exception because args are guaranteed non-null
			copy(this, result);
			return result;
		}


		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> StandardDeviation to copy </param>
		/// <param name="dest"> StandardDeviation to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(StandardDeviation source, StandardDeviation dest) throws mathlib.exception.NullArgumentException
		public static void copy(StandardDeviation source, StandardDeviation dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			dest.Data = source.DataRef;
			dest.variance = source.variance.copy();
		}

	}

}
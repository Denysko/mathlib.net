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
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// Computes the variance of the available values.  By default, the unbiased
	/// "sample variance" definitional formula is used:
	/// <p>
	/// variance = sum((x_i - mean)^2) / (n - 1) </p>
	/// <p>
	/// where mean is the <seealso cref="Mean"/> and <code>n</code> is the number
	/// of sample observations.</p>
	/// <p>
	/// The definitional formula does not have good numerical properties, so
	/// this implementation does not compute the statistic using the definitional
	/// formula. <ul>
	/// <li> The <code>getResult</code> method computes the variance using
	/// updating formulas based on West's algorithm, as described in
	/// <a href="http://doi.acm.org/10.1145/359146.359152"> Chan, T. F. and
	/// J. G. Lewis 1979, <i>Communications of the ACM</i>,
	/// vol. 22 no. 9, pp. 526-531.</a></li>
	/// <li> The <code>evaluate</code> methods leverage the fact that they have the
	/// full array of values in memory to execute a two-pass algorithm.
	/// Specifically, these methods use the "corrected two-pass algorithm" from
	/// Chan, Golub, Levesque, <i>Algorithms for Computing the Sample Variance</i>,
	/// American Statistician, vol. 37, no. 3 (1983) pp. 242-247.</li></ul>
	/// Note that adding values using <code>increment</code> or
	/// <code>incrementAll</code> and then executing <code>getResult</code> will
	/// sometimes give a different, less accurate, result than executing
	/// <code>evaluate</code> with the full array of values. The former approach
	/// should only be used when the full array of values is not available.</p>
	/// <p>
	/// The "population variance"  ( sum((x_i - mean)^2) / n ) can also
	/// be computed using this statistic.  The <code>isBiasCorrected</code>
	/// property determines whether the "population" or "sample" value is
	/// returned by the <code>evaluate</code> and <code>getResult</code> methods.
	/// To compute population variances, set this property to <code>false.</code>
	/// </p>
	/// <p>
	/// <strong>Note that this implementation is not synchronized.</strong> If
	/// multiple threads access an instance of this class concurrently, and at least
	/// one of the threads invokes the <code>increment()</code> or
	/// <code>clear()</code> method, it must be synchronized externally.</p>
	/// 
	/// @version $Id: Variance.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class Variance : AbstractStorelessUnivariateStatistic, WeightedEvaluation
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -9111962718267217978L;

		/// <summary>
		/// SecondMoment is used in incremental calculation of Variance </summary>
		protected internal SecondMoment moment = null;

		/// <summary>
		/// Whether or not <seealso cref="#increment(double)"/> should increment
		/// the internal second moment. When a Variance is constructed with an
		/// external SecondMoment as a constructor parameter, this property is
		/// set to false and increments must be applied to the second moment
		/// directly.
		/// </summary>
		protected internal bool incMoment = true;

		/// <summary>
		/// Whether or not bias correction is applied when computing the
		/// value of the statistic. True means that bias is corrected.  See
		/// <seealso cref="Variance"/> for details on the formula.
		/// </summary>
		private bool isBiasCorrected = true;

		/// <summary>
		/// Constructs a Variance with default (true) <code>isBiasCorrected</code>
		/// property.
		/// </summary>
		public Variance()
		{
			moment = new SecondMoment();
		}

		/// <summary>
		/// Constructs a Variance based on an external second moment.
		/// When this constructor is used, the statistic may only be
		/// incremented via the moment, i.e., <seealso cref="#increment(double)"/>
		/// does nothing; whereas {@code m2.increment(value)} increments
		/// both {@code m2} and the Variance instance constructed from it.
		/// </summary>
		/// <param name="m2"> the SecondMoment (Third or Fourth moments work
		/// here as well.) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Variance(final SecondMoment m2)
		public Variance(SecondMoment m2)
		{
			incMoment = false;
			this.moment = m2;
		}

		/// <summary>
		/// Constructs a Variance with the specified <code>isBiasCorrected</code>
		/// property
		/// </summary>
		/// <param name="isBiasCorrected">  setting for bias correction - true means
		/// bias will be corrected and is equivalent to using the argumentless
		/// constructor </param>
		public Variance(bool isBiasCorrected)
		{
			moment = new SecondMoment();
			this.isBiasCorrected = isBiasCorrected;
		}

		/// <summary>
		/// Constructs a Variance with the specified <code>isBiasCorrected</code>
		/// property and the supplied external second moment.
		/// </summary>
		/// <param name="isBiasCorrected">  setting for bias correction - true means
		/// bias will be corrected </param>
		/// <param name="m2"> the SecondMoment (Third or Fourth moments work
		/// here as well.) </param>
		public Variance(bool isBiasCorrected, SecondMoment m2)
		{
			incMoment = false;
			this.moment = m2;
			this.isBiasCorrected = isBiasCorrected;
		}

		/// <summary>
		/// Copy constructor, creates a new {@code Variance} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code Variance} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Variance(Variance original) throws mathlib.exception.NullArgumentException
		public Variance(Variance original)
		{
			copy(original, this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>If all values are available, it is more accurate to use
		/// <seealso cref="#evaluate(double[])"/> rather than adding values one at a time
		/// using this method and then executing <seealso cref="#getResult"/>, since
		/// <code>evaluate</code> leverages the fact that is has the full
		/// list of values together to execute a two-pass algorithm.
		/// See <seealso cref="Variance"/>.</p>
		/// 
		/// <p>Note also that when <seealso cref="#Variance(SecondMoment)"/> is used to
		/// create a Variance, this method does nothing. In that case, the
		/// SecondMoment should be incremented directly.</p>
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
					if (moment.n == 0)
					{
						return double.NaN;
					}
					else if (moment.n == 1)
					{
						return 0d;
					}
					else
					{
						if (isBiasCorrected)
						{
							return moment.m2 / (moment.n - 1d);
						}
						else
						{
							return moment.m2 / (moment.n);
						}
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
		/// Returns the variance of the entries in the input array, or
		/// <code>Double.NaN</code> if the array is empty.
		/// <p>
		/// See <seealso cref="Variance"/> for details on the computing algorithm.</p>
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		/// <p>
		/// Does not change the internal state of the statistic.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <returns> the variance of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double[] values)
		{
			if (values == null)
			{
				throw new NullArgumentException(LocalizedFormats.INPUT_ARRAY);
			}
			return evaluate(values, 0, values.Length);
		}

		/// <summary>
		/// Returns the variance of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// <p>
		/// See <seealso cref="Variance"/> for details on the computing algorithm.</p>
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// Does not change the internal state of the statistic.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the variance of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		///  parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double[] values, int begin, int length)
		{

			double @var = double.NaN;

			if (test(values, begin, length))
			{
				clear();
				if (length == 1)
				{
					@var = 0.0;
				}
				else if (length > 1)
				{
					Mean mean = new Mean();
					double m = mean.evaluate(values, begin, length);
					@var = evaluate(values, m, begin, length);
				}
			}
			return @var;
		}

		/// <summary>
		/// <p>Returns the weighted variance of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.</p>
		/// <p>
		/// Uses the formula <pre>
		///   &Sigma;(weights[i]*(values[i] - weightedMean)<sup>2</sup>)/(&Sigma;(weights[i]) - 1)
		/// </pre>
		/// where weightedMean is the weighted mean</p>
		/// <p>
		/// This formula will not return the same result as the unweighted variance when all
		/// weights are equal, unless all weights are equal to 1. The formula assumes that
		/// weights are to be treated as "expansion values," as will be the case if for example
		/// the weights represent frequency counts. To normalize weights so that the denominator
		/// in the variance computation equals the length of the input vector minus one, use <pre>
		///   <code>evaluate(values, MathArrays.normalizeArray(weights, values.length)); </code>
		/// </pre>
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
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
		/// <p>
		/// Does not change the internal state of the statistic.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if either array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="weights"> the weights array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the weighted variance of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the parameters are not valid
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double[] weights, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values, double[] weights, int begin, int length)
		{

			double @var = double.NaN;

			if (test(values, weights,begin, length))
			{
				clear();
				if (length == 1)
				{
					@var = 0.0;
				}
				else if (length > 1)
				{
					Mean mean = new Mean();
					double m = mean.evaluate(values, weights, begin, length);
					@var = evaluate(values, weights, m, begin, length);
				}
			}
			return @var;
		}

		/// <summary>
		/// <p>
		/// Returns the weighted variance of the entries in the the input array.</p>
		/// <p>
		/// Uses the formula <pre>
		///   &Sigma;(weights[i]*(values[i] - weightedMean)<sup>2</sup>)/(&Sigma;(weights[i]) - 1)
		/// </pre>
		/// where weightedMean is the weighted mean</p>
		/// <p>
		/// This formula will not return the same result as the unweighted variance when all
		/// weights are equal, unless all weights are equal to 1. The formula assumes that
		/// weights are to be treated as "expansion values," as will be the case if for example
		/// the weights represent frequency counts. To normalize weights so that the denominator
		/// in the variance computation equals the length of the input vector minus one, use <pre>
		///   <code>evaluate(values, MathArrays.normalizeArray(weights, values.length)); </code>
		/// </pre>
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if any of the following are true:
		/// <ul><li>the values array is null</li>
		///     <li>the weights array is null</li>
		///     <li>the weights array does not have the same length as the values array</li>
		///     <li>the weights array contains one or more infinite values</li>
		///     <li>the weights array contains one or more NaN values</li>
		///     <li>the weights array contains negative values</li>
		/// </ul></p>
		/// <p>
		/// Does not change the internal state of the statistic.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if either array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="weights"> the weights array </param>
		/// <returns> the weighted variance of the values </returns>
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
		/// Returns the variance of the entries in the specified portion of
		/// the input array, using the precomputed mean value.  Returns
		/// <code>Double.NaN</code> if the designated subarray is empty.
		/// <p>
		/// See <seealso cref="Variance"/> for details on the computing algorithm.</p>
		/// <p>
		/// The formula used assumes that the supplied mean value is the arithmetic
		/// mean of the sample data, not a known population parameter.  This method
		/// is supplied only to save computation when the mean has already been
		/// computed.</p>
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		/// <p>
		/// Does not change the internal state of the statistic.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="mean"> the precomputed mean value </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the variance of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		///  parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double mean, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values, double mean, int begin, int length)
		{

			double @var = double.NaN;

			if (test(values, begin, length))
			{
				if (length == 1)
				{
					@var = 0.0;
				}
				else if (length > 1)
				{
					double accum = 0.0;
					double dev = 0.0;
					double accum2 = 0.0;
					for (int i = begin; i < begin + length; i++)
					{
						dev = values[i] - mean;
						accum += dev * dev;
						accum2 += dev;
					}
					double len = length;
					if (isBiasCorrected)
					{
						@var = (accum - (accum2 * accum2 / len)) / (len - 1.0);
					}
					else
					{
						@var = (accum - (accum2 * accum2 / len)) / len;
					}
				}
			}
			return @var;
		}

		/// <summary>
		/// Returns the variance of the entries in the input array, using the
		/// precomputed mean value.  Returns <code>Double.NaN</code> if the array
		/// is empty.
		/// <p>
		/// See <seealso cref="Variance"/> for details on the computing algorithm.</p>
		/// <p>
		/// If <code>isBiasCorrected</code> is <code>true</code> the formula used
		/// assumes that the supplied mean value is the arithmetic mean of the
		/// sample data, not a known population parameter.  If the mean is a known
		/// population parameter, or if the "population" version of the variance is
		/// desired, set <code>isBiasCorrected</code> to <code>false</code> before
		/// invoking this method.</p>
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		/// <p>
		/// Does not change the internal state of the statistic.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="mean"> the precomputed mean value </param>
		/// <returns> the variance of the values or Double.NaN if the array is empty </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double mean) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values, double mean)
		{
			return evaluate(values, mean, 0, values.Length);
		}

		/// <summary>
		/// Returns the weighted variance of the entries in the specified portion of
		/// the input array, using the precomputed weighted mean value.  Returns
		/// <code>Double.NaN</code> if the designated subarray is empty.
		/// <p>
		/// Uses the formula <pre>
		///   &Sigma;(weights[i]*(values[i] - mean)<sup>2</sup>)/(&Sigma;(weights[i]) - 1)
		/// </pre></p>
		/// <p>
		/// The formula used assumes that the supplied mean value is the weighted arithmetic
		/// mean of the sample data, not a known population parameter. This method
		/// is supplied only to save computation when the mean has already been
		/// computed.</p>
		/// <p>
		/// This formula will not return the same result as the unweighted variance when all
		/// weights are equal, unless all weights are equal to 1. The formula assumes that
		/// weights are to be treated as "expansion values," as will be the case if for example
		/// the weights represent frequency counts. To normalize weights so that the denominator
		/// in the variance computation equals the length of the input vector minus one, use <pre>
		///   <code>evaluate(values, MathArrays.normalizeArray(weights, values.length), mean); </code>
		/// </pre>
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if any of the following are true:
		/// <ul><li>the values array is null</li>
		///     <li>the weights array is null</li>
		///     <li>the weights array does not have the same length as the values array</li>
		///     <li>the weights array contains one or more infinite values</li>
		///     <li>the weights array contains one or more NaN values</li>
		///     <li>the weights array contains negative values</li>
		///     <li>the start and length arguments do not determine a valid array</li>
		/// </ul></p>
		/// <p>
		/// Does not change the internal state of the statistic.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="weights"> the weights array </param>
		/// <param name="mean"> the precomputed weighted mean value </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the variance of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the parameters are not valid
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double[] weights, final double mean, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values, double[] weights, double mean, int begin, int length)
		{

			double @var = double.NaN;

			if (test(values, weights, begin, length))
			{
				if (length == 1)
				{
					@var = 0.0;
				}
				else if (length > 1)
				{
					double accum = 0.0;
					double dev = 0.0;
					double accum2 = 0.0;
					for (int i = begin; i < begin + length; i++)
					{
						dev = values[i] - mean;
						accum += weights[i] * (dev * dev);
						accum2 += weights[i] * dev;
					}

					double sumWts = 0;
					for (int i = begin; i < begin + length; i++)
					{
						sumWts += weights[i];
					}

					if (isBiasCorrected)
					{
						@var = (accum - (accum2 * accum2 / sumWts)) / (sumWts - 1.0);
					}
					else
					{
						@var = (accum - (accum2 * accum2 / sumWts)) / sumWts;
					}
				}
			}
			return @var;
		}

		/// <summary>
		/// <p>Returns the weighted variance of the values in the input array, using
		/// the precomputed weighted mean value.</p>
		/// <p>
		/// Uses the formula <pre>
		///   &Sigma;(weights[i]*(values[i] - mean)<sup>2</sup>)/(&Sigma;(weights[i]) - 1)
		/// </pre></p>
		/// <p>
		/// The formula used assumes that the supplied mean value is the weighted arithmetic
		/// mean of the sample data, not a known population parameter. This method
		/// is supplied only to save computation when the mean has already been
		/// computed.</p>
		/// <p>
		/// This formula will not return the same result as the unweighted variance when all
		/// weights are equal, unless all weights are equal to 1. The formula assumes that
		/// weights are to be treated as "expansion values," as will be the case if for example
		/// the weights represent frequency counts. To normalize weights so that the denominator
		/// in the variance computation equals the length of the input vector minus one, use <pre>
		///   <code>evaluate(values, MathArrays.normalizeArray(weights, values.length), mean); </code>
		/// </pre>
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if any of the following are true:
		/// <ul><li>the values array is null</li>
		///     <li>the weights array is null</li>
		///     <li>the weights array does not have the same length as the values array</li>
		///     <li>the weights array contains one or more infinite values</li>
		///     <li>the weights array contains one or more NaN values</li>
		///     <li>the weights array contains negative values</li>
		/// </ul></p>
		/// <p>
		/// Does not change the internal state of the statistic.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="weights"> the weights array </param>
		/// <param name="mean"> the precomputed weighted mean value </param>
		/// <returns> the variance of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the parameters are not valid
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double[] weights, final double mean) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values, double[] weights, double mean)
		{
			return evaluate(values, weights, mean, 0, values.Length);
		}

		/// <returns> Returns the isBiasCorrected. </returns>
		public virtual bool BiasCorrected
		{
			get
			{
				return isBiasCorrected;
			}
			set
			{
				this.isBiasCorrected = value;
			}
		}


		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override Variance copy()
		{
			Variance result = new Variance();
			// No try-catch or advertised exception because parameters are guaranteed non-null
			copy(this, result);
			return result;
		}

		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> Variance to copy </param>
		/// <param name="dest"> Variance to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(Variance source, Variance dest) throws mathlib.exception.NullArgumentException
		public static void copy(Variance source, Variance dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			dest.Data = source.DataRef;
			dest.moment = source.moment.copy();
			dest.isBiasCorrected = source.isBiasCorrected;
			dest.incMoment = source.incMoment;
		}
	}

}
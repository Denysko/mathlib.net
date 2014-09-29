using System;
using System.Collections.Generic;

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
namespace mathlib.stat
{

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NotPositiveException = mathlib.exception.NotPositiveException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using NoDataException = mathlib.exception.NoDataException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using DescriptiveStatistics = mathlib.stat.descriptive.DescriptiveStatistics;
	using UnivariateStatistic = mathlib.stat.descriptive.UnivariateStatistic;
	using GeometricMean = mathlib.stat.descriptive.moment.GeometricMean;
	using Mean = mathlib.stat.descriptive.moment.Mean;
	using Variance = mathlib.stat.descriptive.moment.Variance;
	using Max = mathlib.stat.descriptive.rank.Max;
	using Min = mathlib.stat.descriptive.rank.Min;
	using Percentile = mathlib.stat.descriptive.rank.Percentile;
	using Product = mathlib.stat.descriptive.summary.Product;
	using Sum = mathlib.stat.descriptive.summary.Sum;
	using SumOfLogs = mathlib.stat.descriptive.summary.SumOfLogs;
	using SumOfSquares = mathlib.stat.descriptive.summary.SumOfSquares;

	/// <summary>
	/// StatUtils provides static methods for computing statistics based on data
	/// stored in double[] arrays.
	/// 
	/// @version $Id: StatUtils.java 1505931 2013-07-23 08:37:39Z luc $
	/// </summary>
	public sealed class StatUtils
	{

		/// <summary>
		/// sum </summary>
		private static readonly UnivariateStatistic SUM = new Sum();

		/// <summary>
		/// sumSq </summary>
		private static readonly UnivariateStatistic SUM_OF_SQUARES = new SumOfSquares();

		/// <summary>
		/// prod </summary>
		private static readonly UnivariateStatistic PRODUCT = new Product();

		/// <summary>
		/// sumLog </summary>
		private static readonly UnivariateStatistic SUM_OF_LOGS = new SumOfLogs();

		/// <summary>
		/// min </summary>
		private static readonly UnivariateStatistic MIN = new Min();

		/// <summary>
		/// max </summary>
		private static readonly UnivariateStatistic MAX = new Max();

		/// <summary>
		/// mean </summary>
		private static readonly UnivariateStatistic MEAN = new Mean();

		/// <summary>
		/// variance </summary>
		private static readonly Variance VARIANCE = new Variance();

		/// <summary>
		/// percentile </summary>
		private static readonly Percentile PERCENTILE = new Percentile();

		/// <summary>
		/// geometric mean </summary>
		private static readonly GeometricMean GEOMETRIC_MEAN = new GeometricMean();

		/// <summary>
		/// Private Constructor
		/// </summary>
		private StatUtils()
		{
		}

		/// <summary>
		/// Returns the sum of the values in the input array, or
		/// <code>Double.NaN</code> if the array is empty.
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the input array
		/// is null.</p>
		/// </summary>
		/// <param name="values">  array of values to sum </param>
		/// <returns> the sum of the values or <code>Double.NaN</code> if the array
		/// is empty </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double sum(final double[] values) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double sum(double[] values)
		{
			return SUM.evaluate(values);
		}

		/// <summary>
		/// Returns the sum of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the sum of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		///  parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double sum(final double[] values, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double sum(double[] values, int begin, int length)
		{
			return SUM.evaluate(values, begin, length);
		}

		/// <summary>
		/// Returns the sum of the squares of the entries in the input array, or
		/// <code>Double.NaN</code> if the array is empty.
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values">  input array </param>
		/// <returns> the sum of the squared values or <code>Double.NaN</code> if the
		/// array is empty </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double sumSq(final double[] values) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double sumSq(double[] values)
		{
			return SUM_OF_SQUARES.evaluate(values);
		}

		/// <summary>
		/// Returns the sum of the squares of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the sum of the squares of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		/// parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double sumSq(final double[] values, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double sumSq(double[] values, int begin, int length)
		{
			return SUM_OF_SQUARES.evaluate(values, begin, length);
		}

		/// <summary>
		/// Returns the product of the entries in the input array, or
		/// <code>Double.NaN</code> if the array is empty.
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <returns> the product of the values or Double.NaN if the array is empty </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double product(final double[] values) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double product(double[] values)
		{
			return PRODUCT.evaluate(values);
		}

		/// <summary>
		/// Returns the product of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the product of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		/// parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double product(final double[] values, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double product(double[] values, int begin, int length)
		{
			return PRODUCT.evaluate(values, begin, length);
		}

		/// <summary>
		/// Returns the sum of the natural logs of the entries in the input array, or
		/// <code>Double.NaN</code> if the array is empty.
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the array is null.</p>
		/// <p>
		/// See <seealso cref="mathlib.stat.descriptive.summary.SumOfLogs"/>.
		/// </p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <returns> the sum of the natural logs of the values or Double.NaN if
		/// the array is empty </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double sumLog(final double[] values) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double sumLog(double[] values)
		{
			return SUM_OF_LOGS.evaluate(values);
		}

		/// <summary>
		/// Returns the sum of the natural logs of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the array is null.</p>
		/// <p>
		/// See <seealso cref="mathlib.stat.descriptive.summary.SumOfLogs"/>.
		/// </p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the sum of the natural logs of the values or Double.NaN if
		/// length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		/// parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double sumLog(final double[] values, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double sumLog(double[] values, int begin, int length)
		{
			return SUM_OF_LOGS.evaluate(values, begin, length);
		}

		/// <summary>
		/// Returns the arithmetic mean of the entries in the input array, or
		/// <code>Double.NaN</code> if the array is empty.
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the array is null.</p>
		/// <p>
		/// See <seealso cref="mathlib.stat.descriptive.moment.Mean"/> for
		/// details on the computing algorithm.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <returns> the mean of the values or Double.NaN if the array is empty </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double mean(final double[] values) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double mean(double[] values)
		{
			return MEAN.evaluate(values);
		}

		/// <summary>
		/// Returns the arithmetic mean of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the array is null.</p>
		/// <p>
		/// See <seealso cref="mathlib.stat.descriptive.moment.Mean"/> for
		/// details on the computing algorithm.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the mean of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		/// parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double mean(final double[] values, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double mean(double[] values, int begin, int length)
		{
			return MEAN.evaluate(values, begin, length);
		}

		/// <summary>
		/// Returns the geometric mean of the entries in the input array, or
		/// <code>Double.NaN</code> if the array is empty.
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the array is null.</p>
		/// <p>
		/// See <seealso cref="mathlib.stat.descriptive.moment.GeometricMean"/>
		/// for details on the computing algorithm.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <returns> the geometric mean of the values or Double.NaN if the array is empty </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double geometricMean(final double[] values) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double geometricMean(double[] values)
		{
			return GEOMETRIC_MEAN.evaluate(values);
		}

		/// <summary>
		/// Returns the geometric mean of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// <p>
		/// Throws <code>IllegalArgumentException</code> if the array is null.</p>
		/// <p>
		/// See <seealso cref="mathlib.stat.descriptive.moment.GeometricMean"/>
		/// for details on the computing algorithm.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the geometric mean of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		/// parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double geometricMean(final double[] values, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double geometricMean(double[] values, int begin, int length)
		{
			return GEOMETRIC_MEAN.evaluate(values, begin, length);
		}


		/// <summary>
		/// Returns the variance of the entries in the input array, or
		/// <code>Double.NaN</code> if the array is empty.
		/// 
		/// <p>This method returns the bias-corrected sample variance (using {@code n - 1} in
		/// the denominator).  Use <seealso cref="#populationVariance(double[])"/> for the non-bias-corrected
		/// population variance.</p>
		/// <p>
		/// See <seealso cref="mathlib.stat.descriptive.moment.Variance"/> for
		/// details on the computing algorithm.</p>
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <returns> the variance of the values or Double.NaN if the array is empty </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double variance(final double[] values) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double variance(double[] values)
		{
			return VARIANCE.evaluate(values);
		}

		/// <summary>
		/// Returns the variance of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// 
		/// <p>This method returns the bias-corrected sample variance (using {@code n - 1} in
		/// the denominator).  Use <seealso cref="#populationVariance(double[], int, int)"/> for the non-bias-corrected
		/// population variance.</p>
		/// <p>
		/// See <seealso cref="mathlib.stat.descriptive.moment.Variance"/> for
		/// details on the computing algorithm.</p>
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null or the
		/// array index parameters are not valid.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the variance of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		///  parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double variance(final double[] values, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double variance(double[] values, int begin, int length)
		{
			return VARIANCE.evaluate(values, begin, length);
		}

		/// <summary>
		/// Returns the variance of the entries in the specified portion of
		/// the input array, using the precomputed mean value.  Returns
		/// <code>Double.NaN</code> if the designated subarray is empty.
		/// 
		/// <p>This method returns the bias-corrected sample variance (using {@code n - 1} in
		/// the denominator).  Use <seealso cref="#populationVariance(double[], double, int, int)"/> for the non-bias-corrected
		/// population variance.</p>
		/// <p>
		/// See <seealso cref="mathlib.stat.descriptive.moment.Variance"/> for
		/// details on the computing algorithm.</p>
		/// <p>
		/// The formula used assumes that the supplied mean value is the arithmetic
		/// mean of the sample data, not a known population parameter.  This method
		/// is supplied only to save computation when the mean has already been
		/// computed.</p>
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null or the
		/// array index parameters are not valid.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="mean"> the precomputed mean value </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the variance of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		///  parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double variance(final double[] values, final double mean, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double variance(double[] values, double mean, int begin, int length)
		{
			return VARIANCE.evaluate(values, mean, begin, length);
		}

		/// <summary>
		/// Returns the variance of the entries in the input array, using the
		/// precomputed mean value.  Returns <code>Double.NaN</code> if the array
		/// is empty.
		/// 
		/// <p>This method returns the bias-corrected sample variance (using {@code n - 1} in
		/// the denominator).  Use <seealso cref="#populationVariance(double[], double)"/> for the non-bias-corrected
		/// population variance.</p>
		/// <p>
		/// See <seealso cref="mathlib.stat.descriptive.moment.Variance"/> for
		/// details on the computing algorithm.</p>
		/// <p>
		/// The formula used assumes that the supplied mean value is the arithmetic
		/// mean of the sample data, not a known population parameter.  This method
		/// is supplied only to save computation when the mean has already been
		/// computed.</p>
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="mean"> the precomputed mean value </param>
		/// <returns> the variance of the values or Double.NaN if the array is empty </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double variance(final double[] values, final double mean) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double variance(double[] values, double mean)
		{
			return VARIANCE.evaluate(values, mean);
		}

		/// <summary>
		/// Returns the <a href="http://en.wikibooks.org/wiki/Statistics/Summary/Variance">
		/// population variance</a> of the entries in the input array, or
		/// <code>Double.NaN</code> if the array is empty.
		/// <p>
		/// See <seealso cref="mathlib.stat.descriptive.moment.Variance"/> for
		/// details on the formula and computing algorithm.</p>
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <returns> the population variance of the values or Double.NaN if the array is empty </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double populationVariance(final double[] values) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double populationVariance(double[] values)
		{
			return (new Variance(false)).evaluate(values);
		}

		/// <summary>
		/// Returns the <a href="http://en.wikibooks.org/wiki/Statistics/Summary/Variance">
		/// population variance</a> of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// <p>
		/// See <seealso cref="mathlib.stat.descriptive.moment.Variance"/> for
		/// details on the computing algorithm.</p>
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null or the
		/// array index parameters are not valid.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the population variance of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		///  parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double populationVariance(final double[] values, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double populationVariance(double[] values, int begin, int length)
		{
			return (new Variance(false)).evaluate(values, begin, length);
		}

		/// <summary>
		/// Returns the <a href="http://en.wikibooks.org/wiki/Statistics/Summary/Variance">
		/// population variance</a> of the entries in the specified portion of
		/// the input array, using the precomputed mean value.  Returns
		/// <code>Double.NaN</code> if the designated subarray is empty.
		/// <p>
		/// See <seealso cref="mathlib.stat.descriptive.moment.Variance"/> for
		/// details on the computing algorithm.</p>
		/// <p>
		/// The formula used assumes that the supplied mean value is the arithmetic
		/// mean of the sample data, not a known population parameter.  This method
		/// is supplied only to save computation when the mean has already been
		/// computed.</p>
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null or the
		/// array index parameters are not valid.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="mean"> the precomputed mean value </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the population variance of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		///  parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double populationVariance(final double[] values, final double mean, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double populationVariance(double[] values, double mean, int begin, int length)
		{
			return (new Variance(false)).evaluate(values, mean, begin, length);
		}

		/// <summary>
		/// Returns the <a href="http://en.wikibooks.org/wiki/Statistics/Summary/Variance">
		/// population variance</a> of the entries in the input array, using the
		/// precomputed mean value.  Returns <code>Double.NaN</code> if the array
		/// is empty.
		/// <p>
		/// See <seealso cref="mathlib.stat.descriptive.moment.Variance"/> for
		/// details on the computing algorithm.</p>
		/// <p>
		/// The formula used assumes that the supplied mean value is the arithmetic
		/// mean of the sample data, not a known population parameter.  This method
		/// is supplied only to save computation when the mean has already been
		/// computed.</p>
		/// <p>
		/// Returns 0 for a single-value (i.e. length = 1) sample.</p>
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="mean"> the precomputed mean value </param>
		/// <returns> the population variance of the values or Double.NaN if the array is empty </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double populationVariance(final double[] values, final double mean) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double populationVariance(double[] values, double mean)
		{
			return (new Variance(false)).evaluate(values, mean);
		}

		/// <summary>
		/// Returns the maximum of the entries in the input array, or
		/// <code>Double.NaN</code> if the array is empty.
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		/// <p>
		/// <ul>
		/// <li>The result is <code>NaN</code> iff all values are <code>NaN</code>
		/// (i.e. <code>NaN</code> values have no impact on the value of the statistic).</li>
		/// <li>If any of the values equals <code>Double.POSITIVE_INFINITY</code>,
		/// the result is <code>Double.POSITIVE_INFINITY.</code></li>
		/// </ul></p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <returns> the maximum of the values or Double.NaN if the array is empty </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double max(final double[] values) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double max(double[] values)
		{
			return MAX.evaluate(values);
		}

		/// <summary>
		/// Returns the maximum of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null or
		/// the array index parameters are not valid.</p>
		/// <p>
		/// <ul>
		/// <li>The result is <code>NaN</code> iff all values are <code>NaN</code>
		/// (i.e. <code>NaN</code> values have no impact on the value of the statistic).</li>
		/// <li>If any of the values equals <code>Double.POSITIVE_INFINITY</code>,
		/// the result is <code>Double.POSITIVE_INFINITY.</code></li>
		/// </ul></p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the maximum of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		/// parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double max(final double[] values, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double max(double[] values, int begin, int length)
		{
			return MAX.evaluate(values, begin, length);
		}

		 /// <summary>
		 /// Returns the minimum of the entries in the input array, or
		 /// <code>Double.NaN</code> if the array is empty.
		 /// <p>
		 /// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		 /// <p>
		 /// <ul>
		 /// <li>The result is <code>NaN</code> iff all values are <code>NaN</code>
		 /// (i.e. <code>NaN</code> values have no impact on the value of the statistic).</li>
		 /// <li>If any of the values equals <code>Double.NEGATIVE_INFINITY</code>,
		 /// the result is <code>Double.NEGATIVE_INFINITY.</code></li>
		 /// </ul> </p>
		 /// </summary>
		 /// <param name="values"> the input array </param>
		 /// <returns> the minimum of the values or Double.NaN if the array is empty </returns>
		 /// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double min(final double[] values) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double min(double[] values)
		{
			return MIN.evaluate(values);
		}

		 /// <summary>
		 /// Returns the minimum of the entries in the specified portion of
		 /// the input array, or <code>Double.NaN</code> if the designated subarray
		 /// is empty.
		 /// <p>
		 /// Throws <code>MathIllegalArgumentException</code> if the array is null or
		 /// the array index parameters are not valid.</p>
		 /// <p>
		 /// <ul>
		 /// <li>The result is <code>NaN</code> iff all values are <code>NaN</code>
		 /// (i.e. <code>NaN</code> values have no impact on the value of the statistic).</li>
		 /// <li>If any of the values equals <code>Double.NEGATIVE_INFINITY</code>,
		 /// the result is <code>Double.NEGATIVE_INFINITY.</code></li>
		 /// </ul></p>
		 /// </summary>
		 /// <param name="values"> the input array </param>
		 /// <param name="begin"> index of the first array element to include </param>
		 /// <param name="length"> the number of elements to include </param>
		 /// <returns> the minimum of the values or Double.NaN if length = 0 </returns>
		 /// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		 /// parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double min(final double[] values, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double min(double[] values, int begin, int length)
		{
			return MIN.evaluate(values, begin, length);
		}

		/// <summary>
		/// Returns an estimate of the <code>p</code>th percentile of the values
		/// in the <code>values</code> array.
		/// <p>
		/// <ul>
		/// <li>Returns <code>Double.NaN</code> if <code>values</code> has length
		/// <code>0</code></li></p>
		/// <li>Returns (for any value of <code>p</code>) <code>values[0]</code>
		///  if <code>values</code> has length <code>1</code></li>
		/// <li>Throws <code>IllegalArgumentException</code> if <code>values</code>
		/// is null  or p is not a valid quantile value (p must be greater than 0
		/// and less than or equal to 100)</li>
		/// </ul></p>
		/// <p>
		/// See <seealso cref="mathlib.stat.descriptive.rank.Percentile"/> for
		/// a description of the percentile estimation algorithm used.</p>
		/// </summary>
		/// <param name="values"> input array of values </param>
		/// <param name="p"> the percentile value to compute </param>
		/// <returns> the percentile value or Double.NaN if the array is empty </returns>
		/// <exception cref="MathIllegalArgumentException"> if <code>values</code> is null
		/// or p is invalid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double percentile(final double[] values, final double p) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double percentile(double[] values, double p)
		{
				return PERCENTILE.evaluate(values,p);
		}

		 /// <summary>
		 /// Returns an estimate of the <code>p</code>th percentile of the values
		 /// in the <code>values</code> array, starting with the element in (0-based)
		 /// position <code>begin</code> in the array and including <code>length</code>
		 /// values.
		 /// <p>
		 /// <ul>
		 /// <li>Returns <code>Double.NaN</code> if <code>length = 0</code></li>
		 /// <li>Returns (for any value of <code>p</code>) <code>values[begin]</code>
		 ///  if <code>length = 1 </code></li>
		 /// <li>Throws <code>MathIllegalArgumentException</code> if <code>values</code>
		 ///  is null , <code>begin</code> or <code>length</code> is invalid, or
		 /// <code>p</code> is not a valid quantile value (p must be greater than 0
		 /// and less than or equal to 100)</li>
		 /// </ul></p>
		 /// <p>
		 /// See <seealso cref="mathlib.stat.descriptive.rank.Percentile"/> for
		 /// a description of the percentile estimation algorithm used.</p>
		 /// </summary>
		 /// <param name="values"> array of input values </param>
		 /// <param name="p">  the percentile to compute </param>
		 /// <param name="begin">  the first (0-based) element to include in the computation </param>
		 /// <param name="length">  the number of array elements to include </param>
		 /// <returns>  the percentile value </returns>
		 /// <exception cref="MathIllegalArgumentException"> if the parameters are not valid or the
		 /// input array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double percentile(final double[] values, final int begin, final int length, final double p) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double percentile(double[] values, int begin, int length, double p)
		{
			return PERCENTILE.evaluate(values, begin, length, p);
		}

		/// <summary>
		/// Returns the sum of the (signed) differences between corresponding elements of the
		/// input arrays -- i.e., sum(sample1[i] - sample2[i]).
		/// </summary>
		/// <param name="sample1">  the first array </param>
		/// <param name="sample2">  the second array </param>
		/// <returns> sum of paired differences </returns>
		/// <exception cref="DimensionMismatchException"> if the arrays do not have the same
		/// (positive) length. </exception>
		/// <exception cref="NoDataException"> if the sample arrays are empty. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double sumDifference(final double[] sample1, final double[] sample2) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NoDataException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double sumDifference(double[] sample1, double[] sample2)
		{
			int n = sample1.Length;
			if (n != sample2.Length)
			{
				throw new DimensionMismatchException(n, sample2.Length);
			}
			if (n <= 0)
			{
				throw new NoDataException(LocalizedFormats.INSUFFICIENT_DIMENSION);
			}
			double result = 0;
			for (int i = 0; i < n; i++)
			{
				result += sample1[i] - sample2[i];
			}
			return result;
		}

		/// <summary>
		/// Returns the mean of the (signed) differences between corresponding elements of the
		/// input arrays -- i.e., sum(sample1[i] - sample2[i]) / sample1.length.
		/// </summary>
		/// <param name="sample1">  the first array </param>
		/// <param name="sample2">  the second array </param>
		/// <returns> mean of paired differences </returns>
		/// <exception cref="DimensionMismatchException"> if the arrays do not have the same
		/// (positive) length. </exception>
		/// <exception cref="NoDataException"> if the sample arrays are empty. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double meanDifference(final double[] sample1, final double[] sample2) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NoDataException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double meanDifference(double[] sample1, double[] sample2)
		{
			return sumDifference(sample1, sample2) / sample1.Length;
		}

		/// <summary>
		/// Returns the variance of the (signed) differences between corresponding elements of the
		/// input arrays -- i.e., var(sample1[i] - sample2[i]).
		/// </summary>
		/// <param name="sample1">  the first array </param>
		/// <param name="sample2">  the second array </param>
		/// <param name="meanDifference">   the mean difference between corresponding entries </param>
		/// <seealso cref= #meanDifference(double[],double[]) </seealso>
		/// <returns> variance of paired differences </returns>
		/// <exception cref="DimensionMismatchException"> if the arrays do not have the same
		/// length. </exception>
		/// <exception cref="NumberIsTooSmallException"> if the arrays length is less than 2. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double varianceDifference(final double[] sample1, final double[] sample2, double meanDifference) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double varianceDifference(double[] sample1, double[] sample2, double meanDifference)
		{
			double sum1 = 0d;
			double sum2 = 0d;
			double diff = 0d;
			int n = sample1.Length;
			if (n != sample2.Length)
			{
				throw new DimensionMismatchException(n, sample2.Length);
			}
			if (n < 2)
			{
				throw new NumberIsTooSmallException(n, 2, true);
			}
			for (int i = 0; i < n; i++)
			{
				diff = sample1[i] - sample2[i];
				sum1 += (diff - meanDifference) * (diff - meanDifference);
				sum2 += diff - meanDifference;
			}
			return (sum1 - (sum2 * sum2 / n)) / (n - 1);
		}

		/// <summary>
		/// Normalize (standardize) the sample, so it is has a mean of 0 and a standard deviation of 1.
		/// </summary>
		/// <param name="sample"> Sample to normalize. </param>
		/// <returns> normalized (standardized) sample.
		/// @since 2.2 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static double[] normalize(final double[] sample)
		public static double[] normalize(double[] sample)
		{
			DescriptiveStatistics stats = new DescriptiveStatistics();

			// Add the data from the series to stats
			for (int i = 0; i < sample.Length; i++)
			{
				stats.addValue(sample[i]);
			}

			// Compute mean and standard deviation
			double mean = stats.Mean;
			double standardDeviation = stats.StandardDeviation;

			// initialize the standardizedSample, which has the same length as the sample
			double[] standardizedSample = new double[sample.Length];

			for (int i = 0; i < sample.Length; i++)
			{
				// z = (x- mean)/standardDeviation
				standardizedSample[i] = (sample[i] - mean) / standardDeviation;
			}
			return standardizedSample;
		}

		/// <summary>
		/// Returns the sample mode(s).  The mode is the most frequently occurring
		/// value in the sample. If there is a unique value with maximum frequency,
		/// this value is returned as the only element of the output array. Otherwise,
		/// the returned array contains the maximum frequency elements in increasing
		/// order.  For example, if {@code sample} is {0, 12, 5, 6, 0, 13, 5, 17},
		/// the returned array will have length two, with 0 in the first element and
		/// 5 in the second.
		/// 
		/// <p>NaN values are ignored when computing the mode - i.e., NaNs will never
		/// appear in the output array.  If the sample includes only NaNs or has
		/// length 0, an empty array is returned.</p>
		/// </summary>
		/// <param name="sample"> input data </param>
		/// <returns> array of array of the most frequently occurring element(s) sorted in ascending order. </returns>
		/// <exception cref="MathIllegalArgumentException"> if the indices are invalid or the array is null
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double[] mode(double[] sample) throws mathlib.exception.MathIllegalArgumentException
		public static double[] mode(double[] sample)
		{
			if (sample == null)
			{
				throw new NullArgumentException(LocalizedFormats.INPUT_ARRAY);
			}
			return getMode(sample, 0, sample.Length);
		}

		/// <summary>
		/// Returns the sample mode(s).  The mode is the most frequently occurring
		/// value in the sample. If there is a unique value with maximum frequency,
		/// this value is returned as the only element of the output array. Otherwise,
		/// the returned array contains the maximum frequency elements in increasing
		/// order.  For example, if {@code sample} is {0, 12, 5, 6, 0, 13, 5, 17},
		/// the returned array will have length two, with 0 in the first element and
		/// 5 in the second.
		/// 
		/// <p>NaN values are ignored when computing the mode - i.e., NaNs will never
		/// appear in the output array.  If the sample includes only NaNs or has
		/// length 0, an empty array is returned.</p>
		/// </summary>
		/// <param name="sample"> input data </param>
		/// <param name="begin"> index (0-based) of the first array element to include </param>
		/// <param name="length"> the number of elements to include
		/// </param>
		/// <returns> array of array of the most frequently occurring element(s) sorted in ascending order. </returns>
		/// <exception cref="MathIllegalArgumentException"> if the indices are invalid or the array is null
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static double[] mode(double[] sample, final int begin, final int length)
		public static double[] mode(double[] sample, int begin, int length)
		{
			if (sample == null)
			{
				throw new NullArgumentException(LocalizedFormats.INPUT_ARRAY);
			}

			if (begin < 0)
			{
				throw new NotPositiveException(LocalizedFormats.START_POSITION, Convert.ToInt32(begin));
			}

			if (length < 0)
			{
				throw new NotPositiveException(LocalizedFormats.LENGTH, Convert.ToInt32(length));
			}

			return getMode(sample, begin, length);
		}

		/// <summary>
		/// Private helper method.
		/// Assumes parameters have been validated. </summary>
		/// <param name="values"> input data </param>
		/// <param name="begin"> index (0-based) of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> array of array of the most frequently occurring element(s) sorted in ascending order. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static double[] getMode(double[] values, final int begin, final int length)
		private static double[] getMode(double[] values, int begin, int length)
		{
			// Add the values to the frequency table
			Frequency freq = new Frequency();
			for (int i = begin; i < begin + length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double value = values[i];
				double value = values[i];
				if (!double.IsNaN(value))
				{
					freq.addValue(Convert.ToDouble(value));
				}
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<Comparable<?>> list = freq.getMode();
			IList<IComparable<?>> list = freq.Mode;
			// Convert the list to an array of primitive double
			double[] modes = new double[list.Count];
			int i = 0;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for(Comparable<?> c : list)
			foreach (IComparable<?> c in list)
			{
				modes[i++] = (double)((double?) c);
			}
			return modes;
		}

	}

}
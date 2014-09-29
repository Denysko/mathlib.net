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
namespace mathlib.stat.inference
{

	using TDistribution = mathlib.distribution.TDistribution;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using NoDataException = mathlib.exception.NoDataException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using StatisticalSummary = mathlib.stat.descriptive.StatisticalSummary;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// An implementation for Student's t-tests.
	/// <p>
	/// Tests can be:<ul>
	/// <li>One-sample or two-sample</li>
	/// <li>One-sided or two-sided</li>
	/// <li>Paired or unpaired (for two-sample tests)</li>
	/// <li>Homoscedastic (equal variance assumption) or heteroscedastic
	/// (for two sample tests)</li>
	/// <li>Fixed significance level (boolean-valued) or returning p-values.
	/// </li></ul></p>
	/// <p>
	/// Test statistics are available for all tests.  Methods including "Test" in
	/// in their names perform tests, all other methods return t-statistics.  Among
	/// the "Test" methods, <code>double-</code>valued methods return p-values;
	/// <code>boolean-</code>valued methods perform fixed significance level tests.
	/// Significance levels are always specified as numbers between 0 and 0.5
	/// (e.g. tests at the 95% level  use <code>alpha=0.05</code>).</p>
	/// <p>
	/// Input to tests can be either <code>double[]</code> arrays or
	/// <seealso cref="StatisticalSummary"/> instances.</p><p>
	/// Uses commons-math <seealso cref="mathlib.distribution.TDistribution"/>
	/// implementation to estimate exact p-values.</p>
	/// 
	/// @version $Id: TTest.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class TTest
	{
		/// <summary>
		/// Computes a paired, 2-sample t-statistic based on the data in the input
		/// arrays.  The t-statistic returned is equivalent to what would be returned by
		/// computing the one-sample t-statistic <seealso cref="#t(double, double[])"/>, with
		/// <code>mu = 0</code> and the sample array consisting of the (signed)
		/// differences between corresponding entries in <code>sample1</code> and
		/// <code>sample2.</code>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The input arrays must have the same length and their common length
		/// must be at least 2.
		/// </li></ul></p>
		/// </summary>
		/// <param name="sample1"> array of sample data values </param>
		/// <param name="sample2"> array of sample data values </param>
		/// <returns> t statistic </returns>
		/// <exception cref="NullArgumentException"> if the arrays are <code>null</code> </exception>
		/// <exception cref="NoDataException"> if the arrays are empty </exception>
		/// <exception cref="DimensionMismatchException"> if the length of the arrays is not equal </exception>
		/// <exception cref="NumberIsTooSmallException"> if the length of the arrays is &lt; 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double pairedT(final double[] sample1, final double[] sample2) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException, mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double pairedT(double[] sample1, double[] sample2)
		{

			checkSampleData(sample1);
			checkSampleData(sample2);
			double meanDifference = StatUtils.meanDifference(sample1, sample2);
			return t(meanDifference, 0, StatUtils.varianceDifference(sample1, sample2, meanDifference), sample1.Length);

		}

		/// <summary>
		/// Returns the <i>observed significance level</i>, or
		/// <i> p-value</i>, associated with a paired, two-sample, two-tailed t-test
		/// based on the data in the input arrays.
		/// <p>
		/// The number returned is the smallest significance level
		/// at which one can reject the null hypothesis that the mean of the paired
		/// differences is 0 in favor of the two-sided alternative that the mean paired
		/// difference is not equal to 0. For a one-sided test, divide the returned
		/// value by 2.</p>
		/// <p>
		/// This test is equivalent to a one-sample t-test computed using
		/// <seealso cref="#tTest(double, double[])"/> with <code>mu = 0</code> and the sample
		/// array consisting of the signed differences between corresponding elements of
		/// <code>sample1</code> and <code>sample2.</code></p>
		/// <p>
		/// <strong>Usage Note:</strong><br>
		/// The validity of the p-value depends on the assumptions of the parametric
		/// t-test procedure, as discussed
		/// <a href="http://www.basic.nwu.edu/statguidefiles/ttest_unpaired_ass_viol.html">
		/// here</a></p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The input array lengths must be the same and their common length must
		/// be at least 2.
		/// </li></ul></p>
		/// </summary>
		/// <param name="sample1"> array of sample data values </param>
		/// <param name="sample2"> array of sample data values </param>
		/// <returns> p-value for t-test </returns>
		/// <exception cref="NullArgumentException"> if the arrays are <code>null</code> </exception>
		/// <exception cref="NoDataException"> if the arrays are empty </exception>
		/// <exception cref="DimensionMismatchException"> if the length of the arrays is not equal </exception>
		/// <exception cref="NumberIsTooSmallException"> if the length of the arrays is &lt; 2 </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double pairedTTest(final double[] sample1, final double[] sample2) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException, mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double pairedTTest(double[] sample1, double[] sample2)
		{

			double meanDifference = StatUtils.meanDifference(sample1, sample2);
			return tTest(meanDifference, 0, StatUtils.varianceDifference(sample1, sample2, meanDifference), sample1.Length);

		}

		/// <summary>
		/// Performs a paired t-test evaluating the null hypothesis that the
		/// mean of the paired differences between <code>sample1</code> and
		/// <code>sample2</code> is 0 in favor of the two-sided alternative that the
		/// mean paired difference is not equal to 0, with significance level
		/// <code>alpha</code>.
		/// <p>
		/// Returns <code>true</code> iff the null hypothesis can be rejected with
		/// confidence <code>1 - alpha</code>.  To perform a 1-sided test, use
		/// <code>alpha * 2</code></p>
		/// <p>
		/// <strong>Usage Note:</strong><br>
		/// The validity of the test depends on the assumptions of the parametric
		/// t-test procedure, as discussed
		/// <a href="http://www.basic.nwu.edu/statguidefiles/ttest_unpaired_ass_viol.html">
		/// here</a></p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The input array lengths must be the same and their common length
		/// must be at least 2.
		/// </li>
		/// <li> <code> 0 &lt; alpha &lt; 0.5 </code>
		/// </li></ul></p>
		/// </summary>
		/// <param name="sample1"> array of sample data values </param>
		/// <param name="sample2"> array of sample data values </param>
		/// <param name="alpha"> significance level of the test </param>
		/// <returns> true if the null hypothesis can be rejected with
		/// confidence 1 - alpha </returns>
		/// <exception cref="NullArgumentException"> if the arrays are <code>null</code> </exception>
		/// <exception cref="NoDataException"> if the arrays are empty </exception>
		/// <exception cref="DimensionMismatchException"> if the length of the arrays is not equal </exception>
		/// <exception cref="NumberIsTooSmallException"> if the length of the arrays is &lt; 2 </exception>
		/// <exception cref="OutOfRangeException"> if <code>alpha</code> is not in the range (0, 0.5] </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean pairedTTest(final double[] sample1, final double[] sample2, final double alpha) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException, mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual bool pairedTTest(double[] sample1, double[] sample2, double alpha)
		{

			checkSignificanceLevel(alpha);
			return pairedTTest(sample1, sample2) < alpha;

		}

		/// <summary>
		/// Computes a <a href="http://www.itl.nist.gov/div898/handbook/prc/section2/prc22.htm#formula">
		/// t statistic </a> given observed values and a comparison constant.
		/// <p>
		/// This statistic can be used to perform a one sample t-test for the mean.
		/// </p><p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The observed array length must be at least 2.
		/// </li></ul></p>
		/// </summary>
		/// <param name="mu"> comparison constant </param>
		/// <param name="observed"> array of values </param>
		/// <returns> t statistic </returns>
		/// <exception cref="NullArgumentException"> if <code>observed</code> is <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the length of <code>observed</code> is &lt; 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double t(final double mu, final double[] observed) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double t(double mu, double[] observed)
		{

			checkSampleData(observed);
			// No try-catch or advertised exception because args have just been checked
			return t(StatUtils.mean(observed), mu, StatUtils.variance(observed), observed.Length);

		}

		/// <summary>
		/// Computes a <a href="http://www.itl.nist.gov/div898/handbook/prc/section2/prc22.htm#formula">
		/// t statistic </a> to use in comparing the mean of the dataset described by
		/// <code>sampleStats</code> to <code>mu</code>.
		/// <p>
		/// This statistic can be used to perform a one sample t-test for the mean.
		/// </p><p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li><code>observed.getN() &ge; 2</code>.
		/// </li></ul></p>
		/// </summary>
		/// <param name="mu"> comparison constant </param>
		/// <param name="sampleStats"> DescriptiveStatistics holding sample summary statitstics </param>
		/// <returns> t statistic </returns>
		/// <exception cref="NullArgumentException"> if <code>sampleStats</code> is <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the number of samples is &lt; 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double t(final double mu, final mathlib.stat.descriptive.StatisticalSummary sampleStats) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double t(double mu, StatisticalSummary sampleStats)
		{

			checkSampleData(sampleStats);
			return t(sampleStats.Mean, mu, sampleStats.Variance, sampleStats.N);

		}

		/// <summary>
		/// Computes a 2-sample t statistic,  under the hypothesis of equal
		/// subpopulation variances.  To compute a t-statistic without the
		/// equal variances hypothesis, use <seealso cref="#t(double[], double[])"/>.
		/// <p>
		/// This statistic can be used to perform a (homoscedastic) two-sample
		/// t-test to compare sample means.</p>
		/// <p>
		/// The t-statistic is</p>
		/// <p>
		/// &nbsp;&nbsp;<code>  t = (m1 - m2) / (sqrt(1/n1 +1/n2) sqrt(var))</code>
		/// </p><p>
		/// where <strong><code>n1</code></strong> is the size of first sample;
		/// <strong><code> n2</code></strong> is the size of second sample;
		/// <strong><code> m1</code></strong> is the mean of first sample;
		/// <strong><code> m2</code></strong> is the mean of second sample</li>
		/// </ul>
		/// and <strong><code>var</code></strong> is the pooled variance estimate:
		/// </p><p>
		/// <code>var = sqrt(((n1 - 1)var1 + (n2 - 1)var2) / ((n1-1) + (n2-1)))</code>
		/// </p><p>
		/// with <strong><code>var1</code></strong> the variance of the first sample and
		/// <strong><code>var2</code></strong> the variance of the second sample.
		/// </p><p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The observed array lengths must both be at least 2.
		/// </li></ul></p>
		/// </summary>
		/// <param name="sample1"> array of sample data values </param>
		/// <param name="sample2"> array of sample data values </param>
		/// <returns> t statistic </returns>
		/// <exception cref="NullArgumentException"> if the arrays are <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the length of the arrays is &lt; 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double homoscedasticT(final double[] sample1, final double[] sample2) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double homoscedasticT(double[] sample1, double[] sample2)
		{

			checkSampleData(sample1);
			checkSampleData(sample2);
			// No try-catch or advertised exception because args have just been checked
			return homoscedasticT(StatUtils.mean(sample1), StatUtils.mean(sample2), StatUtils.variance(sample1), StatUtils.variance(sample2), sample1.Length, sample2.Length);

		}

		/// <summary>
		/// Computes a 2-sample t statistic, without the hypothesis of equal
		/// subpopulation variances.  To compute a t-statistic assuming equal
		/// variances, use <seealso cref="#homoscedasticT(double[], double[])"/>.
		/// <p>
		/// This statistic can be used to perform a two-sample t-test to compare
		/// sample means.</p>
		/// <p>
		/// The t-statistic is</p>
		/// <p>
		/// &nbsp;&nbsp; <code>  t = (m1 - m2) / sqrt(var1/n1 + var2/n2)</code>
		/// </p><p>
		///  where <strong><code>n1</code></strong> is the size of the first sample
		/// <strong><code> n2</code></strong> is the size of the second sample;
		/// <strong><code> m1</code></strong> is the mean of the first sample;
		/// <strong><code> m2</code></strong> is the mean of the second sample;
		/// <strong><code> var1</code></strong> is the variance of the first sample;
		/// <strong><code> var2</code></strong> is the variance of the second sample;
		/// </p><p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The observed array lengths must both be at least 2.
		/// </li></ul></p>
		/// </summary>
		/// <param name="sample1"> array of sample data values </param>
		/// <param name="sample2"> array of sample data values </param>
		/// <returns> t statistic </returns>
		/// <exception cref="NullArgumentException"> if the arrays are <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the length of the arrays is &lt; 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double t(final double[] sample1, final double[] sample2) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double t(double[] sample1, double[] sample2)
		{

			checkSampleData(sample1);
			checkSampleData(sample2);
			// No try-catch or advertised exception because args have just been checked
			return t(StatUtils.mean(sample1), StatUtils.mean(sample2), StatUtils.variance(sample1), StatUtils.variance(sample2), sample1.Length, sample2.Length);

		}

		/// <summary>
		/// Computes a 2-sample t statistic </a>, comparing the means of the datasets
		/// described by two <seealso cref="StatisticalSummary"/> instances, without the
		/// assumption of equal subpopulation variances.  Use
		/// <seealso cref="#homoscedasticT(StatisticalSummary, StatisticalSummary)"/> to
		/// compute a t-statistic under the equal variances assumption.
		/// <p>
		/// This statistic can be used to perform a two-sample t-test to compare
		/// sample means.</p>
		/// <p>
		/// The returned  t-statistic is</p>
		/// <p>
		/// &nbsp;&nbsp; <code>  t = (m1 - m2) / sqrt(var1/n1 + var2/n2)</code>
		/// </p><p>
		/// where <strong><code>n1</code></strong> is the size of the first sample;
		/// <strong><code> n2</code></strong> is the size of the second sample;
		/// <strong><code> m1</code></strong> is the mean of the first sample;
		/// <strong><code> m2</code></strong> is the mean of the second sample
		/// <strong><code> var1</code></strong> is the variance of the first sample;
		/// <strong><code> var2</code></strong> is the variance of the second sample
		/// </p><p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The datasets described by the two Univariates must each contain
		/// at least 2 observations.
		/// </li></ul></p>
		/// </summary>
		/// <param name="sampleStats1"> StatisticalSummary describing data from the first sample </param>
		/// <param name="sampleStats2"> StatisticalSummary describing data from the second sample </param>
		/// <returns> t statistic </returns>
		/// <exception cref="NullArgumentException"> if the sample statistics are <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the number of samples is &lt; 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double t(final mathlib.stat.descriptive.StatisticalSummary sampleStats1, final mathlib.stat.descriptive.StatisticalSummary sampleStats2) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double t(StatisticalSummary sampleStats1, StatisticalSummary sampleStats2)
		{

			checkSampleData(sampleStats1);
			checkSampleData(sampleStats2);
			return t(sampleStats1.Mean, sampleStats2.Mean, sampleStats1.Variance, sampleStats2.Variance, sampleStats1.N, sampleStats2.N);

		}

		/// <summary>
		/// Computes a 2-sample t statistic, comparing the means of the datasets
		/// described by two <seealso cref="StatisticalSummary"/> instances, under the
		/// assumption of equal subpopulation variances.  To compute a t-statistic
		/// without the equal variances assumption, use
		/// <seealso cref="#t(StatisticalSummary, StatisticalSummary)"/>.
		/// <p>
		/// This statistic can be used to perform a (homoscedastic) two-sample
		/// t-test to compare sample means.</p>
		/// <p>
		/// The t-statistic returned is</p>
		/// <p>
		/// &nbsp;&nbsp;<code>  t = (m1 - m2) / (sqrt(1/n1 +1/n2) sqrt(var))</code>
		/// </p><p>
		/// where <strong><code>n1</code></strong> is the size of first sample;
		/// <strong><code> n2</code></strong> is the size of second sample;
		/// <strong><code> m1</code></strong> is the mean of first sample;
		/// <strong><code> m2</code></strong> is the mean of second sample
		/// and <strong><code>var</code></strong> is the pooled variance estimate:
		/// </p><p>
		/// <code>var = sqrt(((n1 - 1)var1 + (n2 - 1)var2) / ((n1-1) + (n2-1)))</code>
		/// </p><p>
		/// with <strong><code>var1</code></strong> the variance of the first sample and
		/// <strong><code>var2</code></strong> the variance of the second sample.
		/// </p><p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The datasets described by the two Univariates must each contain
		/// at least 2 observations.
		/// </li></ul></p>
		/// </summary>
		/// <param name="sampleStats1"> StatisticalSummary describing data from the first sample </param>
		/// <param name="sampleStats2"> StatisticalSummary describing data from the second sample </param>
		/// <returns> t statistic </returns>
		/// <exception cref="NullArgumentException"> if the sample statistics are <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the number of samples is &lt; 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double homoscedasticT(final mathlib.stat.descriptive.StatisticalSummary sampleStats1, final mathlib.stat.descriptive.StatisticalSummary sampleStats2) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double homoscedasticT(StatisticalSummary sampleStats1, StatisticalSummary sampleStats2)
		{

			checkSampleData(sampleStats1);
			checkSampleData(sampleStats2);
			return homoscedasticT(sampleStats1.Mean, sampleStats2.Mean, sampleStats1.Variance, sampleStats2.Variance, sampleStats1.N, sampleStats2.N);

		}

		/// <summary>
		/// Returns the <i>observed significance level</i>, or
		/// <i>p-value</i>, associated with a one-sample, two-tailed t-test
		/// comparing the mean of the input array with the constant <code>mu</code>.
		/// <p>
		/// The number returned is the smallest significance level
		/// at which one can reject the null hypothesis that the mean equals
		/// <code>mu</code> in favor of the two-sided alternative that the mean
		/// is different from <code>mu</code>. For a one-sided test, divide the
		/// returned value by 2.</p>
		/// <p>
		/// <strong>Usage Note:</strong><br>
		/// The validity of the test depends on the assumptions of the parametric
		/// t-test procedure, as discussed
		/// <a href="http://www.basic.nwu.edu/statguidefiles/ttest_unpaired_ass_viol.html">here</a>
		/// </p><p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The observed array length must be at least 2.
		/// </li></ul></p>
		/// </summary>
		/// <param name="mu"> constant value to compare sample mean against </param>
		/// <param name="sample"> array of sample data values </param>
		/// <returns> p-value </returns>
		/// <exception cref="NullArgumentException"> if the sample array is <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the length of the array is &lt; 2 </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double tTest(final double mu, final double[] sample) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double tTest(double mu, double[] sample)
		{

			checkSampleData(sample);
			// No try-catch or advertised exception because args have just been checked
			return tTest(StatUtils.mean(sample), mu, StatUtils.variance(sample), sample.Length);

		}

		/// <summary>
		/// Performs a <a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda353.htm">
		/// two-sided t-test</a> evaluating the null hypothesis that the mean of the population from
		/// which <code>sample</code> is drawn equals <code>mu</code>.
		/// <p>
		/// Returns <code>true</code> iff the null hypothesis can be
		/// rejected with confidence <code>1 - alpha</code>.  To
		/// perform a 1-sided test, use <code>alpha * 2</code></p>
		/// <p>
		/// <strong>Examples:</strong><br><ol>
		/// <li>To test the (2-sided) hypothesis <code>sample mean = mu </code> at
		/// the 95% level, use <br><code>tTest(mu, sample, 0.05) </code>
		/// </li>
		/// <li>To test the (one-sided) hypothesis <code> sample mean < mu </code>
		/// at the 99% level, first verify that the measured sample mean is less
		/// than <code>mu</code> and then use
		/// <br><code>tTest(mu, sample, 0.02) </code>
		/// </li></ol></p>
		/// <p>
		/// <strong>Usage Note:</strong><br>
		/// The validity of the test depends on the assumptions of the one-sample
		/// parametric t-test procedure, as discussed
		/// <a href="http://www.basic.nwu.edu/statguidefiles/sg_glos.html#one-sample">here</a>
		/// </p><p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The observed array length must be at least 2.
		/// </li></ul></p>
		/// </summary>
		/// <param name="mu"> constant value to compare sample mean against </param>
		/// <param name="sample"> array of sample data values </param>
		/// <param name="alpha"> significance level of the test </param>
		/// <returns> p-value </returns>
		/// <exception cref="NullArgumentException"> if the sample array is <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the length of the array is &lt; 2 </exception>
		/// <exception cref="OutOfRangeException"> if <code>alpha</code> is not in the range (0, 0.5] </exception>
		/// <exception cref="MaxCountExceededException"> if an error computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean tTest(final double mu, final double[] sample, final double alpha) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual bool tTest(double mu, double[] sample, double alpha)
		{

			checkSignificanceLevel(alpha);
			return tTest(mu, sample) < alpha;

		}

		/// <summary>
		/// Returns the <i>observed significance level</i>, or
		/// <i>p-value</i>, associated with a one-sample, two-tailed t-test
		/// comparing the mean of the dataset described by <code>sampleStats</code>
		/// with the constant <code>mu</code>.
		/// <p>
		/// The number returned is the smallest significance level
		/// at which one can reject the null hypothesis that the mean equals
		/// <code>mu</code> in favor of the two-sided alternative that the mean
		/// is different from <code>mu</code>. For a one-sided test, divide the
		/// returned value by 2.</p>
		/// <p>
		/// <strong>Usage Note:</strong><br>
		/// The validity of the test depends on the assumptions of the parametric
		/// t-test procedure, as discussed
		/// <a href="http://www.basic.nwu.edu/statguidefiles/ttest_unpaired_ass_viol.html">
		/// here</a></p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The sample must contain at least 2 observations.
		/// </li></ul></p>
		/// </summary>
		/// <param name="mu"> constant value to compare sample mean against </param>
		/// <param name="sampleStats"> StatisticalSummary describing sample data </param>
		/// <returns> p-value </returns>
		/// <exception cref="NullArgumentException"> if <code>sampleStats</code> is <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the number of samples is &lt; 2 </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double tTest(final double mu, final mathlib.stat.descriptive.StatisticalSummary sampleStats) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double tTest(double mu, StatisticalSummary sampleStats)
		{

			checkSampleData(sampleStats);
			return tTest(sampleStats.Mean, mu, sampleStats.Variance, sampleStats.N);

		}

		/// <summary>
		/// Performs a <a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda353.htm">
		/// two-sided t-test</a> evaluating the null hypothesis that the mean of the
		/// population from which the dataset described by <code>stats</code> is
		/// drawn equals <code>mu</code>.
		/// <p>
		/// Returns <code>true</code> iff the null hypothesis can be rejected with
		/// confidence <code>1 - alpha</code>.  To  perform a 1-sided test, use
		/// <code>alpha * 2.</code></p>
		/// <p>
		/// <strong>Examples:</strong><br><ol>
		/// <li>To test the (2-sided) hypothesis <code>sample mean = mu </code> at
		/// the 95% level, use <br><code>tTest(mu, sampleStats, 0.05) </code>
		/// </li>
		/// <li>To test the (one-sided) hypothesis <code> sample mean < mu </code>
		/// at the 99% level, first verify that the measured sample mean is less
		/// than <code>mu</code> and then use
		/// <br><code>tTest(mu, sampleStats, 0.02) </code>
		/// </li></ol></p>
		/// <p>
		/// <strong>Usage Note:</strong><br>
		/// The validity of the test depends on the assumptions of the one-sample
		/// parametric t-test procedure, as discussed
		/// <a href="http://www.basic.nwu.edu/statguidefiles/sg_glos.html#one-sample">here</a>
		/// </p><p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The sample must include at least 2 observations.
		/// </li></ul></p>
		/// </summary>
		/// <param name="mu"> constant value to compare sample mean against </param>
		/// <param name="sampleStats"> StatisticalSummary describing sample data values </param>
		/// <param name="alpha"> significance level of the test </param>
		/// <returns> p-value </returns>
		/// <exception cref="NullArgumentException"> if <code>sampleStats</code> is <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the number of samples is &lt; 2 </exception>
		/// <exception cref="OutOfRangeException"> if <code>alpha</code> is not in the range (0, 0.5] </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean tTest(final double mu, final mathlib.stat.descriptive.StatisticalSummary sampleStats, final double alpha) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual bool tTest(double mu, StatisticalSummary sampleStats, double alpha)
		{

			checkSignificanceLevel(alpha);
			return tTest(mu, sampleStats) < alpha;

		}

		/// <summary>
		/// Returns the <i>observed significance level</i>, or
		/// <i>p-value</i>, associated with a two-sample, two-tailed t-test
		/// comparing the means of the input arrays.
		/// <p>
		/// The number returned is the smallest significance level
		/// at which one can reject the null hypothesis that the two means are
		/// equal in favor of the two-sided alternative that they are different.
		/// For a one-sided test, divide the returned value by 2.</p>
		/// <p>
		/// The test does not assume that the underlying popuation variances are
		/// equal  and it uses approximated degrees of freedom computed from the
		/// sample data to compute the p-value.  The t-statistic used is as defined in
		/// <seealso cref="#t(double[], double[])"/> and the Welch-Satterthwaite approximation
		/// to the degrees of freedom is used,
		/// as described
		/// <a href="http://www.itl.nist.gov/div898/handbook/prc/section3/prc31.htm">
		/// here.</a>  To perform the test under the assumption of equal subpopulation
		/// variances, use <seealso cref="#homoscedasticTTest(double[], double[])"/>.</p>
		/// <p>
		/// <strong>Usage Note:</strong><br>
		/// The validity of the p-value depends on the assumptions of the parametric
		/// t-test procedure, as discussed
		/// <a href="http://www.basic.nwu.edu/statguidefiles/ttest_unpaired_ass_viol.html">
		/// here</a></p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The observed array lengths must both be at least 2.
		/// </li></ul></p>
		/// </summary>
		/// <param name="sample1"> array of sample data values </param>
		/// <param name="sample2"> array of sample data values </param>
		/// <returns> p-value for t-test </returns>
		/// <exception cref="NullArgumentException"> if the arrays are <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the length of the arrays is &lt; 2 </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double tTest(final double[] sample1, final double[] sample2) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double tTest(double[] sample1, double[] sample2)
		{

			checkSampleData(sample1);
			checkSampleData(sample2);
			// No try-catch or advertised exception because args have just been checked
			return tTest(StatUtils.mean(sample1), StatUtils.mean(sample2), StatUtils.variance(sample1), StatUtils.variance(sample2), sample1.Length, sample2.Length);

		}

		/// <summary>
		/// Returns the <i>observed significance level</i>, or
		/// <i>p-value</i>, associated with a two-sample, two-tailed t-test
		/// comparing the means of the input arrays, under the assumption that
		/// the two samples are drawn from subpopulations with equal variances.
		/// To perform the test without the equal variances assumption, use
		/// <seealso cref="#tTest(double[], double[])"/>.</p>
		/// <p>
		/// The number returned is the smallest significance level
		/// at which one can reject the null hypothesis that the two means are
		/// equal in favor of the two-sided alternative that they are different.
		/// For a one-sided test, divide the returned value by 2.</p>
		/// <p>
		/// A pooled variance estimate is used to compute the t-statistic.  See
		/// <seealso cref="#homoscedasticT(double[], double[])"/>. The sum of the sample sizes
		/// minus 2 is used as the degrees of freedom.</p>
		/// <p>
		/// <strong>Usage Note:</strong><br>
		/// The validity of the p-value depends on the assumptions of the parametric
		/// t-test procedure, as discussed
		/// <a href="http://www.basic.nwu.edu/statguidefiles/ttest_unpaired_ass_viol.html">
		/// here</a></p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The observed array lengths must both be at least 2.
		/// </li></ul></p>
		/// </summary>
		/// <param name="sample1"> array of sample data values </param>
		/// <param name="sample2"> array of sample data values </param>
		/// <returns> p-value for t-test </returns>
		/// <exception cref="NullArgumentException"> if the arrays are <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the length of the arrays is &lt; 2 </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double homoscedasticTTest(final double[] sample1, final double[] sample2) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double homoscedasticTTest(double[] sample1, double[] sample2)
		{

			checkSampleData(sample1);
			checkSampleData(sample2);
			// No try-catch or advertised exception because args have just been checked
			return homoscedasticTTest(StatUtils.mean(sample1), StatUtils.mean(sample2), StatUtils.variance(sample1), StatUtils.variance(sample2), sample1.Length, sample2.Length);

		}

		/// <summary>
		/// Performs a
		/// <a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda353.htm">
		/// two-sided t-test</a> evaluating the null hypothesis that <code>sample1</code>
		/// and <code>sample2</code> are drawn from populations with the same mean,
		/// with significance level <code>alpha</code>.  This test does not assume
		/// that the subpopulation variances are equal.  To perform the test assuming
		/// equal variances, use
		/// <seealso cref="#homoscedasticTTest(double[], double[], double)"/>.
		/// <p>
		/// Returns <code>true</code> iff the null hypothesis that the means are
		/// equal can be rejected with confidence <code>1 - alpha</code>.  To
		/// perform a 1-sided test, use <code>alpha * 2</code></p>
		/// <p>
		/// See <seealso cref="#t(double[], double[])"/> for the formula used to compute the
		/// t-statistic.  Degrees of freedom are approximated using the
		/// <a href="http://www.itl.nist.gov/div898/handbook/prc/section3/prc31.htm">
		/// Welch-Satterthwaite approximation.</a></p>
		/// <p>
		/// <strong>Examples:</strong><br><ol>
		/// <li>To test the (2-sided) hypothesis <code>mean 1 = mean 2 </code> at
		/// the 95% level,  use
		/// <br><code>tTest(sample1, sample2, 0.05). </code>
		/// </li>
		/// <li>To test the (one-sided) hypothesis <code> mean 1 < mean 2 </code>,
		/// at the 99% level, first verify that the measured  mean of <code>sample 1</code>
		/// is less than the mean of <code>sample 2</code> and then use
		/// <br><code>tTest(sample1, sample2, 0.02) </code>
		/// </li></ol></p>
		/// <p>
		/// <strong>Usage Note:</strong><br>
		/// The validity of the test depends on the assumptions of the parametric
		/// t-test procedure, as discussed
		/// <a href="http://www.basic.nwu.edu/statguidefiles/ttest_unpaired_ass_viol.html">
		/// here</a></p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The observed array lengths must both be at least 2.
		/// </li>
		/// <li> <code> 0 < alpha < 0.5 </code>
		/// </li></ul></p>
		/// </summary>
		/// <param name="sample1"> array of sample data values </param>
		/// <param name="sample2"> array of sample data values </param>
		/// <param name="alpha"> significance level of the test </param>
		/// <returns> true if the null hypothesis can be rejected with
		/// confidence 1 - alpha </returns>
		/// <exception cref="NullArgumentException"> if the arrays are <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the length of the arrays is &lt; 2 </exception>
		/// <exception cref="OutOfRangeException"> if <code>alpha</code> is not in the range (0, 0.5] </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean tTest(final double[] sample1, final double[] sample2, final double alpha) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual bool tTest(double[] sample1, double[] sample2, double alpha)
		{

			checkSignificanceLevel(alpha);
			return tTest(sample1, sample2) < alpha;

		}

		/// <summary>
		/// Performs a
		/// <a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda353.htm">
		/// two-sided t-test</a> evaluating the null hypothesis that <code>sample1</code>
		/// and <code>sample2</code> are drawn from populations with the same mean,
		/// with significance level <code>alpha</code>,  assuming that the
		/// subpopulation variances are equal.  Use
		/// <seealso cref="#tTest(double[], double[], double)"/> to perform the test without
		/// the assumption of equal variances.
		/// <p>
		/// Returns <code>true</code> iff the null hypothesis that the means are
		/// equal can be rejected with confidence <code>1 - alpha</code>.  To
		/// perform a 1-sided test, use <code>alpha * 2.</code>  To perform the test
		/// without the assumption of equal subpopulation variances, use
		/// <seealso cref="#tTest(double[], double[], double)"/>.</p>
		/// <p>
		/// A pooled variance estimate is used to compute the t-statistic. See
		/// <seealso cref="#t(double[], double[])"/> for the formula. The sum of the sample
		/// sizes minus 2 is used as the degrees of freedom.</p>
		/// <p>
		/// <strong>Examples:</strong><br><ol>
		/// <li>To test the (2-sided) hypothesis <code>mean 1 = mean 2 </code> at
		/// the 95% level, use <br><code>tTest(sample1, sample2, 0.05). </code>
		/// </li>
		/// <li>To test the (one-sided) hypothesis <code> mean 1 < mean 2, </code>
		/// at the 99% level, first verify that the measured mean of
		/// <code>sample 1</code> is less than the mean of <code>sample 2</code>
		/// and then use
		/// <br><code>tTest(sample1, sample2, 0.02) </code>
		/// </li></ol></p>
		/// <p>
		/// <strong>Usage Note:</strong><br>
		/// The validity of the test depends on the assumptions of the parametric
		/// t-test procedure, as discussed
		/// <a href="http://www.basic.nwu.edu/statguidefiles/ttest_unpaired_ass_viol.html">
		/// here</a></p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The observed array lengths must both be at least 2.
		/// </li>
		/// <li> <code> 0 < alpha < 0.5 </code>
		/// </li></ul></p>
		/// </summary>
		/// <param name="sample1"> array of sample data values </param>
		/// <param name="sample2"> array of sample data values </param>
		/// <param name="alpha"> significance level of the test </param>
		/// <returns> true if the null hypothesis can be rejected with
		/// confidence 1 - alpha </returns>
		/// <exception cref="NullArgumentException"> if the arrays are <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the length of the arrays is &lt; 2 </exception>
		/// <exception cref="OutOfRangeException"> if <code>alpha</code> is not in the range (0, 0.5] </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean homoscedasticTTest(final double[] sample1, final double[] sample2, final double alpha) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual bool homoscedasticTTest(double[] sample1, double[] sample2, double alpha)
		{

			checkSignificanceLevel(alpha);
			return homoscedasticTTest(sample1, sample2) < alpha;

		}

		/// <summary>
		/// Returns the <i>observed significance level</i>, or
		/// <i>p-value</i>, associated with a two-sample, two-tailed t-test
		/// comparing the means of the datasets described by two StatisticalSummary
		/// instances.
		/// <p>
		/// The number returned is the smallest significance level
		/// at which one can reject the null hypothesis that the two means are
		/// equal in favor of the two-sided alternative that they are different.
		/// For a one-sided test, divide the returned value by 2.</p>
		/// <p>
		/// The test does not assume that the underlying population variances are
		/// equal  and it uses approximated degrees of freedom computed from the
		/// sample data to compute the p-value.   To perform the test assuming
		/// equal variances, use
		/// <seealso cref="#homoscedasticTTest(StatisticalSummary, StatisticalSummary)"/>.</p>
		/// <p>
		/// <strong>Usage Note:</strong><br>
		/// The validity of the p-value depends on the assumptions of the parametric
		/// t-test procedure, as discussed
		/// <a href="http://www.basic.nwu.edu/statguidefiles/ttest_unpaired_ass_viol.html">
		/// here</a></p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The datasets described by the two Univariates must each contain
		/// at least 2 observations.
		/// </li></ul></p>
		/// </summary>
		/// <param name="sampleStats1">  StatisticalSummary describing data from the first sample </param>
		/// <param name="sampleStats2">  StatisticalSummary describing data from the second sample </param>
		/// <returns> p-value for t-test </returns>
		/// <exception cref="NullArgumentException"> if the sample statistics are <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the number of samples is &lt; 2 </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double tTest(final mathlib.stat.descriptive.StatisticalSummary sampleStats1, final mathlib.stat.descriptive.StatisticalSummary sampleStats2) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double tTest(StatisticalSummary sampleStats1, StatisticalSummary sampleStats2)
		{

			checkSampleData(sampleStats1);
			checkSampleData(sampleStats2);
			return tTest(sampleStats1.Mean, sampleStats2.Mean, sampleStats1.Variance, sampleStats2.Variance, sampleStats1.N, sampleStats2.N);

		}

		/// <summary>
		/// Returns the <i>observed significance level</i>, or
		/// <i>p-value</i>, associated with a two-sample, two-tailed t-test
		/// comparing the means of the datasets described by two StatisticalSummary
		/// instances, under the hypothesis of equal subpopulation variances. To
		/// perform a test without the equal variances assumption, use
		/// <seealso cref="#tTest(StatisticalSummary, StatisticalSummary)"/>.
		/// <p>
		/// The number returned is the smallest significance level
		/// at which one can reject the null hypothesis that the two means are
		/// equal in favor of the two-sided alternative that they are different.
		/// For a one-sided test, divide the returned value by 2.</p>
		/// <p>
		/// See <seealso cref="#homoscedasticT(double[], double[])"/> for the formula used to
		/// compute the t-statistic. The sum of the  sample sizes minus 2 is used as
		/// the degrees of freedom.</p>
		/// <p>
		/// <strong>Usage Note:</strong><br>
		/// The validity of the p-value depends on the assumptions of the parametric
		/// t-test procedure, as discussed
		/// <a href="http://www.basic.nwu.edu/statguidefiles/ttest_unpaired_ass_viol.html">here</a>
		/// </p><p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The datasets described by the two Univariates must each contain
		/// at least 2 observations.
		/// </li></ul></p>
		/// </summary>
		/// <param name="sampleStats1">  StatisticalSummary describing data from the first sample </param>
		/// <param name="sampleStats2">  StatisticalSummary describing data from the second sample </param>
		/// <returns> p-value for t-test </returns>
		/// <exception cref="NullArgumentException"> if the sample statistics are <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the number of samples is &lt; 2 </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double homoscedasticTTest(final mathlib.stat.descriptive.StatisticalSummary sampleStats1, final mathlib.stat.descriptive.StatisticalSummary sampleStats2) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double homoscedasticTTest(StatisticalSummary sampleStats1, StatisticalSummary sampleStats2)
		{

			checkSampleData(sampleStats1);
			checkSampleData(sampleStats2);
			return homoscedasticTTest(sampleStats1.Mean, sampleStats2.Mean, sampleStats1.Variance, sampleStats2.Variance, sampleStats1.N, sampleStats2.N);

		}

		/// <summary>
		/// Performs a
		/// <a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda353.htm">
		/// two-sided t-test</a> evaluating the null hypothesis that
		/// <code>sampleStats1</code> and <code>sampleStats2</code> describe
		/// datasets drawn from populations with the same mean, with significance
		/// level <code>alpha</code>.   This test does not assume that the
		/// subpopulation variances are equal.  To perform the test under the equal
		/// variances assumption, use
		/// <seealso cref="#homoscedasticTTest(StatisticalSummary, StatisticalSummary)"/>.
		/// <p>
		/// Returns <code>true</code> iff the null hypothesis that the means are
		/// equal can be rejected with confidence <code>1 - alpha</code>.  To
		/// perform a 1-sided test, use <code>alpha * 2</code></p>
		/// <p>
		/// See <seealso cref="#t(double[], double[])"/> for the formula used to compute the
		/// t-statistic.  Degrees of freedom are approximated using the
		/// <a href="http://www.itl.nist.gov/div898/handbook/prc/section3/prc31.htm">
		/// Welch-Satterthwaite approximation.</a></p>
		/// <p>
		/// <strong>Examples:</strong><br><ol>
		/// <li>To test the (2-sided) hypothesis <code>mean 1 = mean 2 </code> at
		/// the 95%, use
		/// <br><code>tTest(sampleStats1, sampleStats2, 0.05) </code>
		/// </li>
		/// <li>To test the (one-sided) hypothesis <code> mean 1 < mean 2 </code>
		/// at the 99% level,  first verify that the measured mean of
		/// <code>sample 1</code> is less than  the mean of <code>sample 2</code>
		/// and then use
		/// <br><code>tTest(sampleStats1, sampleStats2, 0.02) </code>
		/// </li></ol></p>
		/// <p>
		/// <strong>Usage Note:</strong><br>
		/// The validity of the test depends on the assumptions of the parametric
		/// t-test procedure, as discussed
		/// <a href="http://www.basic.nwu.edu/statguidefiles/ttest_unpaired_ass_viol.html">
		/// here</a></p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>The datasets described by the two Univariates must each contain
		/// at least 2 observations.
		/// </li>
		/// <li> <code> 0 < alpha < 0.5 </code>
		/// </li></ul></p>
		/// </summary>
		/// <param name="sampleStats1"> StatisticalSummary describing sample data values </param>
		/// <param name="sampleStats2"> StatisticalSummary describing sample data values </param>
		/// <param name="alpha"> significance level of the test </param>
		/// <returns> true if the null hypothesis can be rejected with
		/// confidence 1 - alpha </returns>
		/// <exception cref="NullArgumentException"> if the sample statistics are <code>null</code> </exception>
		/// <exception cref="NumberIsTooSmallException"> if the number of samples is &lt; 2 </exception>
		/// <exception cref="OutOfRangeException"> if <code>alpha</code> is not in the range (0, 0.5] </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean tTest(final mathlib.stat.descriptive.StatisticalSummary sampleStats1, final mathlib.stat.descriptive.StatisticalSummary sampleStats2, final double alpha) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual bool tTest(StatisticalSummary sampleStats1, StatisticalSummary sampleStats2, double alpha)
		{

			checkSignificanceLevel(alpha);
			return tTest(sampleStats1, sampleStats2) < alpha;

		}

		//----------------------------------------------- Protected methods

		/// <summary>
		/// Computes approximate degrees of freedom for 2-sample t-test.
		/// </summary>
		/// <param name="v1"> first sample variance </param>
		/// <param name="v2"> second sample variance </param>
		/// <param name="n1"> first sample n </param>
		/// <param name="n2"> second sample n </param>
		/// <returns> approximate degrees of freedom </returns>
		protected internal virtual double df(double v1, double v2, double n1, double n2)
		{
			return (((v1 / n1) + (v2 / n2)) * ((v1 / n1) + (v2 / n2))) / ((v1 * v1) / (n1 * n1 * (n1 - 1d)) + (v2 * v2) / (n2 * n2 * (n2 - 1d)));
		}

		/// <summary>
		/// Computes t test statistic for 1-sample t-test.
		/// </summary>
		/// <param name="m"> sample mean </param>
		/// <param name="mu"> constant to test against </param>
		/// <param name="v"> sample variance </param>
		/// <param name="n"> sample n </param>
		/// <returns> t test statistic </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected double t(final double m, final double mu, final double v, final double n)
		protected internal virtual double t(double m, double mu, double v, double n)
		{
			return (m - mu) / FastMath.sqrt(v / n);
		}

		/// <summary>
		/// Computes t test statistic for 2-sample t-test.
		/// <p>
		/// Does not assume that subpopulation variances are equal.</p>
		/// </summary>
		/// <param name="m1"> first sample mean </param>
		/// <param name="m2"> second sample mean </param>
		/// <param name="v1"> first sample variance </param>
		/// <param name="v2"> second sample variance </param>
		/// <param name="n1"> first sample n </param>
		/// <param name="n2"> second sample n </param>
		/// <returns> t test statistic </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected double t(final double m1, final double m2, final double v1, final double v2, final double n1, final double n2)
		protected internal virtual double t(double m1, double m2, double v1, double v2, double n1, double n2)
		{
			return (m1 - m2) / FastMath.sqrt((v1 / n1) + (v2 / n2));
		}

		/// <summary>
		/// Computes t test statistic for 2-sample t-test under the hypothesis
		/// of equal subpopulation variances.
		/// </summary>
		/// <param name="m1"> first sample mean </param>
		/// <param name="m2"> second sample mean </param>
		/// <param name="v1"> first sample variance </param>
		/// <param name="v2"> second sample variance </param>
		/// <param name="n1"> first sample n </param>
		/// <param name="n2"> second sample n </param>
		/// <returns> t test statistic </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected double homoscedasticT(final double m1, final double m2, final double v1, final double v2, final double n1, final double n2)
		protected internal virtual double homoscedasticT(double m1, double m2, double v1, double v2, double n1, double n2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double pooledVariance = ((n1 - 1) * v1 + (n2 -1) * v2) / (n1 + n2 - 2);
			double pooledVariance = ((n1 - 1) * v1 + (n2 - 1) * v2) / (n1 + n2 - 2);
			return (m1 - m2) / FastMath.sqrt(pooledVariance * (1d / n1 + 1d / n2));
		}

		/// <summary>
		/// Computes p-value for 2-sided, 1-sample t-test.
		/// </summary>
		/// <param name="m"> sample mean </param>
		/// <param name="mu"> constant to test against </param>
		/// <param name="v"> sample variance </param>
		/// <param name="n"> sample n </param>
		/// <returns> p-value </returns>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
		/// <exception cref="MathIllegalArgumentException"> if n is not greater than 1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected double tTest(final double m, final double mu, final double v, final double n) throws mathlib.exception.MaxCountExceededException, mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual double tTest(double m, double mu, double v, double n)
		{

			double t = FastMath.abs(t(m, mu, v, n));
			TDistribution distribution = new TDistribution(n - 1);
			return 2.0 * distribution.cumulativeProbability(-t);

		}

		/// <summary>
		/// Computes p-value for 2-sided, 2-sample t-test.
		/// <p>
		/// Does not assume subpopulation variances are equal. Degrees of freedom
		/// are estimated from the data.</p>
		/// </summary>
		/// <param name="m1"> first sample mean </param>
		/// <param name="m2"> second sample mean </param>
		/// <param name="v1"> first sample variance </param>
		/// <param name="v2"> second sample variance </param>
		/// <param name="n1"> first sample n </param>
		/// <param name="n2"> second sample n </param>
		/// <returns> p-value </returns>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
		/// <exception cref="NotStrictlyPositiveException"> if the estimated degrees of freedom is not
		/// strictly positive </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected double tTest(final double m1, final double m2, final double v1, final double v2, final double n1, final double n2) throws mathlib.exception.MaxCountExceededException, mathlib.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual double tTest(double m1, double m2, double v1, double v2, double n1, double n2)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = mathlib.util.FastMath.abs(t(m1, m2, v1, v2, n1, n2));
			double t = FastMath.abs(t(m1, m2, v1, v2, n1, n2));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double degreesOfFreedom = df(v1, v2, n1, n2);
			double degreesOfFreedom = df(v1, v2, n1, n2);
			TDistribution distribution = new TDistribution(degreesOfFreedom);
			return 2.0 * distribution.cumulativeProbability(-t);

		}

		/// <summary>
		/// Computes p-value for 2-sided, 2-sample t-test, under the assumption
		/// of equal subpopulation variances.
		/// <p>
		/// The sum of the sample sizes minus 2 is used as degrees of freedom.</p>
		/// </summary>
		/// <param name="m1"> first sample mean </param>
		/// <param name="m2"> second sample mean </param>
		/// <param name="v1"> first sample variance </param>
		/// <param name="v2"> second sample variance </param>
		/// <param name="n1"> first sample n </param>
		/// <param name="n2"> second sample n </param>
		/// <returns> p-value </returns>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
		/// <exception cref="NotStrictlyPositiveException"> if the estimated degrees of freedom is not
		/// strictly positive </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected double homoscedasticTTest(double m1, double m2, double v1, double v2, double n1, double n2) throws mathlib.exception.MaxCountExceededException, mathlib.exception.NotStrictlyPositiveException
		protected internal virtual double homoscedasticTTest(double m1, double m2, double v1, double v2, double n1, double n2)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = mathlib.util.FastMath.abs(homoscedasticT(m1, m2, v1, v2, n1, n2));
			double t = FastMath.abs(homoscedasticT(m1, m2, v1, v2, n1, n2));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double degreesOfFreedom = n1 + n2 - 2;
			double degreesOfFreedom = n1 + n2 - 2;
			TDistribution distribution = new TDistribution(degreesOfFreedom);
			return 2.0 * distribution.cumulativeProbability(-t);

		}

		/// <summary>
		/// Check significance level.
		/// </summary>
		/// <param name="alpha"> significance level </param>
		/// <exception cref="OutOfRangeException"> if the significance level is out of bounds. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkSignificanceLevel(final double alpha) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void checkSignificanceLevel(double alpha)
		{

			if (alpha <= 0 || alpha > 0.5)
			{
				throw new OutOfRangeException(LocalizedFormats.SIGNIFICANCE_LEVEL, alpha, 0.0, 0.5);
			}

		}

		/// <summary>
		/// Check sample data.
		/// </summary>
		/// <param name="data"> Sample data. </param>
		/// <exception cref="NullArgumentException"> if {@code data} is {@code null}. </exception>
		/// <exception cref="NumberIsTooSmallException"> if there is not enough sample data. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkSampleData(final double[] data) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void checkSampleData(double[] data)
		{

			if (data == null)
			{
				throw new NullArgumentException();
			}
			if (data.Length < 2)
			{
				throw new NumberIsTooSmallException(LocalizedFormats.INSUFFICIENT_DATA_FOR_T_STATISTIC, data.Length, 2, true);
			}

		}

		/// <summary>
		/// Check sample data.
		/// </summary>
		/// <param name="stat"> Statistical summary. </param>
		/// <exception cref="NullArgumentException"> if {@code data} is {@code null}. </exception>
		/// <exception cref="NumberIsTooSmallException"> if there is not enough sample data. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkSampleData(final mathlib.stat.descriptive.StatisticalSummary stat) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void checkSampleData(StatisticalSummary stat)
		{

			if (stat == null)
			{
				throw new NullArgumentException();
			}
			if (stat.N < 2)
			{
				throw new NumberIsTooSmallException(LocalizedFormats.INSUFFICIENT_DATA_FOR_T_STATISTIC, stat.N, 2, true);
			}

		}

	}

}
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
namespace org.apache.commons.math3.stat.inference
{

	using ChiSquaredDistribution = org.apache.commons.math3.distribution.ChiSquaredDistribution;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MaxCountExceededException = org.apache.commons.math3.exception.MaxCountExceededException;
	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using ZeroException = org.apache.commons.math3.exception.ZeroException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathArrays = org.apache.commons.math3.util.MathArrays;

	/// <summary>
	/// Implements Chi-Square test statistics.
	/// 
	/// <p>This implementation handles both known and unknown distributions.</p>
	/// 
	/// <p>Two samples tests can be used when the distribution is unknown <i>a priori</i>
	/// but provided by one sample, or when the hypothesis under test is that the two
	/// samples come from the same underlying distribution.</p>
	/// 
	/// @version $Id: ChiSquareTest.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class ChiSquareTest
	{

		/// <summary>
		/// Construct a ChiSquareTest
		/// </summary>
		public ChiSquareTest() : base()
		{
		}

		/// <summary>
		/// Computes the <a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda35f.htm">
		/// Chi-Square statistic</a> comparing <code>observed</code> and <code>expected</code>
		/// frequency counts.
		/// <p>
		/// This statistic can be used to perform a Chi-Square test evaluating the null
		/// hypothesis that the observed counts follow the expected distribution.</p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>Expected counts must all be positive.
		/// </li>
		/// <li>Observed counts must all be &ge; 0.
		/// </li>
		/// <li>The observed and expected arrays must have the same length and
		/// their common length must be at least 2.
		/// </li></ul></p><p>
		/// If any of the preconditions are not met, an
		/// <code>IllegalArgumentException</code> is thrown.</p>
		/// <p><strong>Note: </strong>This implementation rescales the
		/// <code>expected</code> array if necessary to ensure that the sum of the
		/// expected and observed counts are equal.</p>
		/// </summary>
		/// <param name="observed"> array of observed frequency counts </param>
		/// <param name="expected"> array of expected frequency counts </param>
		/// <returns> chiSquare test statistic </returns>
		/// <exception cref="NotPositiveException"> if <code>observed</code> has negative entries </exception>
		/// <exception cref="NotStrictlyPositiveException"> if <code>expected</code> has entries that are
		/// not strictly positive </exception>
		/// <exception cref="DimensionMismatchException"> if the arrays length is less than 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double chiSquare(final double[] expected, final long[] observed) throws org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.NotStrictlyPositiveException, org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double chiSquare(double[] expected, long[] observed)
		{

			if (expected.Length < 2)
			{
				throw new DimensionMismatchException(expected.Length, 2);
			}
			if (expected.Length != observed.Length)
			{
				throw new DimensionMismatchException(expected.Length, observed.Length);
			}
			MathArrays.checkPositive(expected);
			MathArrays.checkNonNegative(observed);

			double sumExpected = 0d;
			double sumObserved = 0d;
			for (int i = 0; i < observed.Length; i++)
			{
				sumExpected += expected[i];
				sumObserved += observed[i];
			}
			double ratio = 1.0d;
			bool rescale = false;
			if (FastMath.abs(sumExpected - sumObserved) > 10E-6)
			{
				ratio = sumObserved / sumExpected;
				rescale = true;
			}
			double sumSq = 0.0d;
			for (int i = 0; i < observed.Length; i++)
			{
				if (rescale)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dev = observed[i] - ratio * expected[i];
					double dev = observed[i] - ratio * expected[i];
					sumSq += dev * dev / (ratio * expected[i]);
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dev = observed[i] - expected[i];
					double dev = observed[i] - expected[i];
					sumSq += dev * dev / expected[i];
				}
			}
			return sumSq;

		}

		/// <summary>
		/// Returns the <i>observed significance level</i>, or <a href=
		/// "http://www.cas.lancs.ac.uk/glossary_v1.1/hyptest.html#pvalue">
		/// p-value</a>, associated with a
		/// <a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda35f.htm">
		/// Chi-square goodness of fit test</a> comparing the <code>observed</code>
		/// frequency counts to those in the <code>expected</code> array.
		/// <p>
		/// The number returned is the smallest significance level at which one can reject
		/// the null hypothesis that the observed counts conform to the frequency distribution
		/// described by the expected counts.</p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>Expected counts must all be positive.
		/// </li>
		/// <li>Observed counts must all be &ge; 0.
		/// </li>
		/// <li>The observed and expected arrays must have the same length and
		/// their common length must be at least 2.
		/// </li></ul></p><p>
		/// If any of the preconditions are not met, an
		/// <code>IllegalArgumentException</code> is thrown.</p>
		/// <p><strong>Note: </strong>This implementation rescales the
		/// <code>expected</code> array if necessary to ensure that the sum of the
		/// expected and observed counts are equal.</p>
		/// </summary>
		/// <param name="observed"> array of observed frequency counts </param>
		/// <param name="expected"> array of expected frequency counts </param>
		/// <returns> p-value </returns>
		/// <exception cref="NotPositiveException"> if <code>observed</code> has negative entries </exception>
		/// <exception cref="NotStrictlyPositiveException"> if <code>expected</code> has entries that are
		/// not strictly positive </exception>
		/// <exception cref="DimensionMismatchException"> if the arrays length is less than 2 </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double chiSquareTest(final double[] expected, final long[] observed) throws org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.NotStrictlyPositiveException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double chiSquareTest(double[] expected, long[] observed)
		{

			ChiSquaredDistribution distribution = new ChiSquaredDistribution(expected.Length - 1.0);
			return 1.0 - distribution.cumulativeProbability(chiSquare(expected, observed));
		}

		/// <summary>
		/// Performs a <a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda35f.htm">
		/// Chi-square goodness of fit test</a> evaluating the null hypothesis that the
		/// observed counts conform to the frequency distribution described by the expected
		/// counts, with significance level <code>alpha</code>.  Returns true iff the null
		/// hypothesis can be rejected with 100 * (1 - alpha) percent confidence.
		/// <p>
		/// <strong>Example:</strong><br>
		/// To test the hypothesis that <code>observed</code> follows
		/// <code>expected</code> at the 99% level, use </p><p>
		/// <code>chiSquareTest(expected, observed, 0.01) </code></p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>Expected counts must all be positive.
		/// </li>
		/// <li>Observed counts must all be &ge; 0.
		/// </li>
		/// <li>The observed and expected arrays must have the same length and
		/// their common length must be at least 2.
		/// <li> <code> 0 &lt; alpha &lt; 0.5 </code>
		/// </li></ul></p><p>
		/// If any of the preconditions are not met, an
		/// <code>IllegalArgumentException</code> is thrown.</p>
		/// <p><strong>Note: </strong>This implementation rescales the
		/// <code>expected</code> array if necessary to ensure that the sum of the
		/// expected and observed counts are equal.</p>
		/// </summary>
		/// <param name="observed"> array of observed frequency counts </param>
		/// <param name="expected"> array of expected frequency counts </param>
		/// <param name="alpha"> significance level of the test </param>
		/// <returns> true iff null hypothesis can be rejected with confidence
		/// 1 - alpha </returns>
		/// <exception cref="NotPositiveException"> if <code>observed</code> has negative entries </exception>
		/// <exception cref="NotStrictlyPositiveException"> if <code>expected</code> has entries that are
		/// not strictly positive </exception>
		/// <exception cref="DimensionMismatchException"> if the arrays length is less than 2 </exception>
		/// <exception cref="OutOfRangeException"> if <code>alpha</code> is not in the range (0, 0.5] </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean chiSquareTest(final double[] expected, final long[] observed, final double alpha) throws org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.NotStrictlyPositiveException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual bool chiSquareTest(double[] expected, long[] observed, double alpha)
		{

			if ((alpha <= 0) || (alpha > 0.5))
			{
				throw new OutOfRangeException(LocalizedFormats.OUT_OF_BOUND_SIGNIFICANCE_LEVEL, alpha, 0, 0.5);
			}
			return chiSquareTest(expected, observed) < alpha;

		}

		/// <summary>
		///  Computes the Chi-Square statistic associated with a
		/// <a href="http://www.itl.nist.gov/div898/handbook/prc/section4/prc45.htm">
		///  chi-square test of independence</a> based on the input <code>counts</code>
		///  array, viewed as a two-way table.
		/// <p>
		/// The rows of the 2-way table are
		/// <code>count[0], ... , count[count.length - 1] </code></p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>All counts must be &ge; 0.
		/// </li>
		/// <li>The count array must be rectangular (i.e. all count[i] subarrays
		///  must have the same length).
		/// </li>
		/// <li>The 2-way table represented by <code>counts</code> must have at
		///  least 2 columns and at least 2 rows.
		/// </li>
		/// </li></ul></p><p>
		/// If any of the preconditions are not met, an
		/// <code>IllegalArgumentException</code> is thrown.</p>
		/// </summary>
		/// <param name="counts"> array representation of 2-way table </param>
		/// <returns> chiSquare test statistic </returns>
		/// <exception cref="NullArgumentException"> if the array is null </exception>
		/// <exception cref="DimensionMismatchException"> if the array is not rectangular </exception>
		/// <exception cref="NotPositiveException"> if {@code counts} has negative entries </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double chiSquare(final long[][] counts) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double chiSquare(long[][] counts)
		{

			checkArray(counts);
			int nRows = counts.Length;
			int nCols = counts[0].Length;

			// compute row, column and total sums
			double[] rowSum = new double[nRows];
			double[] colSum = new double[nCols];
			double total = 0.0d;
			for (int row = 0; row < nRows; row++)
			{
				for (int col = 0; col < nCols; col++)
				{
					rowSum[row] += counts[row][col];
					colSum[col] += counts[row][col];
					total += counts[row][col];
				}
			}

			// compute expected counts and chi-square
			double sumSq = 0.0d;
			double expected = 0.0d;
			for (int row = 0; row < nRows; row++)
			{
				for (int col = 0; col < nCols; col++)
				{
					expected = (rowSum[row] * colSum[col]) / total;
					sumSq += ((counts[row][col] - expected) * (counts[row][col] - expected)) / expected;
				}
			}
			return sumSq;

		}

		/// <summary>
		/// Returns the <i>observed significance level</i>, or <a href=
		/// "http://www.cas.lancs.ac.uk/glossary_v1.1/hyptest.html#pvalue">
		/// p-value</a>, associated with a
		/// <a href="http://www.itl.nist.gov/div898/handbook/prc/section4/prc45.htm">
		/// chi-square test of independence</a> based on the input <code>counts</code>
		/// array, viewed as a two-way table.
		/// <p>
		/// The rows of the 2-way table are
		/// <code>count[0], ... , count[count.length - 1] </code></p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>All counts must be &ge; 0.
		/// </li>
		/// <li>The count array must be rectangular (i.e. all count[i] subarrays must have
		///     the same length).
		/// </li>
		/// <li>The 2-way table represented by <code>counts</code> must have at least 2
		///     columns and at least 2 rows.
		/// </li>
		/// </li></ul></p><p>
		/// If any of the preconditions are not met, an
		/// <code>IllegalArgumentException</code> is thrown.</p>
		/// </summary>
		/// <param name="counts"> array representation of 2-way table </param>
		/// <returns> p-value </returns>
		/// <exception cref="NullArgumentException"> if the array is null </exception>
		/// <exception cref="DimensionMismatchException"> if the array is not rectangular </exception>
		/// <exception cref="NotPositiveException"> if {@code counts} has negative entries </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double chiSquareTest(final long[][] counts) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double chiSquareTest(long[][] counts)
		{

			checkArray(counts);
			double df = ((double) counts.Length - 1) * ((double) counts[0].Length - 1);
			ChiSquaredDistribution distribution;
			distribution = new ChiSquaredDistribution(df);
			return 1 - distribution.cumulativeProbability(chiSquare(counts));

		}

		/// <summary>
		/// Performs a <a href="http://www.itl.nist.gov/div898/handbook/prc/section4/prc45.htm">
		/// chi-square test of independence</a> evaluating the null hypothesis that the
		/// classifications represented by the counts in the columns of the input 2-way table
		/// are independent of the rows, with significance level <code>alpha</code>.
		/// Returns true iff the null hypothesis can be rejected with 100 * (1 - alpha) percent
		/// confidence.
		/// <p>
		/// The rows of the 2-way table are
		/// <code>count[0], ... , count[count.length - 1] </code></p>
		/// <p>
		/// <strong>Example:</strong><br>
		/// To test the null hypothesis that the counts in
		/// <code>count[0], ... , count[count.length - 1] </code>
		///  all correspond to the same underlying probability distribution at the 99% level, use</p>
		/// <p><code>chiSquareTest(counts, 0.01)</code></p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>All counts must be &ge; 0.
		/// </li>
		/// <li>The count array must be rectangular (i.e. all count[i] subarrays must have the
		///     same length).</li>
		/// <li>The 2-way table represented by <code>counts</code> must have at least 2 columns and
		///     at least 2 rows.</li>
		/// </li></ul></p><p>
		/// If any of the preconditions are not met, an
		/// <code>IllegalArgumentException</code> is thrown.</p>
		/// </summary>
		/// <param name="counts"> array representation of 2-way table </param>
		/// <param name="alpha"> significance level of the test </param>
		/// <returns> true iff null hypothesis can be rejected with confidence
		/// 1 - alpha </returns>
		/// <exception cref="NullArgumentException"> if the array is null </exception>
		/// <exception cref="DimensionMismatchException"> if the array is not rectangular </exception>
		/// <exception cref="NotPositiveException"> if {@code counts} has any negative entries </exception>
		/// <exception cref="OutOfRangeException"> if <code>alpha</code> is not in the range (0, 0.5] </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean chiSquareTest(final long[][] counts, final double alpha) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual bool chiSquareTest(long[][] counts, double alpha)
		{

			if ((alpha <= 0) || (alpha > 0.5))
			{
				throw new OutOfRangeException(LocalizedFormats.OUT_OF_BOUND_SIGNIFICANCE_LEVEL, alpha, 0, 0.5);
			}
			return chiSquareTest(counts) < alpha;

		}

		/// <summary>
		/// <p>Computes a
		/// <a href="http://www.itl.nist.gov/div898/software/dataplot/refman1/auxillar/chi2samp.htm">
		/// Chi-Square two sample test statistic</a> comparing bin frequency counts
		/// in <code>observed1</code> and <code>observed2</code>.  The
		/// sums of frequency counts in the two samples are not required to be the
		/// same.  The formula used to compute the test statistic is</p>
		/// <code>
		/// &sum;[(K * observed1[i] - observed2[i]/K)<sup>2</sup> / (observed1[i] + observed2[i])]
		/// </code> where
		/// <br/><code>K = &sqrt;[&sum(observed2 / &sum;(observed1)]</code>
		/// </p>
		/// <p>This statistic can be used to perform a Chi-Square test evaluating the
		/// null hypothesis that both observed counts follow the same distribution.</p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>Observed counts must be non-negative.
		/// </li>
		/// <li>Observed counts for a specific bin must not both be zero.
		/// </li>
		/// <li>Observed counts for a specific sample must not all be 0.
		/// </li>
		/// <li>The arrays <code>observed1</code> and <code>observed2</code> must have
		/// the same length and their common length must be at least 2.
		/// </li></ul></p><p>
		/// If any of the preconditions are not met, an
		/// <code>IllegalArgumentException</code> is thrown.</p>
		/// </summary>
		/// <param name="observed1"> array of observed frequency counts of the first data set </param>
		/// <param name="observed2"> array of observed frequency counts of the second data set </param>
		/// <returns> chiSquare test statistic </returns>
		/// <exception cref="DimensionMismatchException"> the the length of the arrays does not match </exception>
		/// <exception cref="NotPositiveException"> if any entries in <code>observed1</code> or
		/// <code>observed2</code> are negative </exception>
		/// <exception cref="ZeroException"> if either all counts of <code>observed1</code> or
		/// <code>observed2</code> are zero, or if the count at some index is zero
		/// for both arrays
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double chiSquareDataSetsComparison(long[] observed1, long[] observed2) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.ZeroException
		public virtual double chiSquareDataSetsComparison(long[] observed1, long[] observed2)
		{

			// Make sure lengths are same
			if (observed1.Length < 2)
			{
				throw new DimensionMismatchException(observed1.Length, 2);
			}
			if (observed1.Length != observed2.Length)
			{
				throw new DimensionMismatchException(observed1.Length, observed2.Length);
			}

			// Ensure non-negative counts
			MathArrays.checkNonNegative(observed1);
			MathArrays.checkNonNegative(observed2);

			// Compute and compare count sums
			long countSum1 = 0;
			long countSum2 = 0;
			bool unequalCounts = false;
			double weight = 0.0;
			for (int i = 0; i < observed1.Length; i++)
			{
				countSum1 += observed1[i];
				countSum2 += observed2[i];
			}
			// Ensure neither sample is uniformly 0
			if (countSum1 == 0 || countSum2 == 0)
			{
				throw new ZeroException();
			}
			// Compare and compute weight only if different
			unequalCounts = countSum1 != countSum2;
			if (unequalCounts)
			{
				weight = FastMath.sqrt((double) countSum1 / (double) countSum2);
			}
			// Compute ChiSquare statistic
			double sumSq = 0.0d;
			double dev = 0.0d;
			double obs1 = 0.0d;
			double obs2 = 0.0d;
			for (int i = 0; i < observed1.Length; i++)
			{
				if (observed1[i] == 0 && observed2[i] == 0)
				{
					throw new ZeroException(LocalizedFormats.OBSERVED_COUNTS_BOTTH_ZERO_FOR_ENTRY, i);
				}
				else
				{
					obs1 = observed1[i];
					obs2 = observed2[i];
					if (unequalCounts) // apply weights
					{
						dev = obs1 / weight - obs2 * weight;
					}
					else
					{
						dev = obs1 - obs2;
					}
					sumSq += (dev * dev) / (obs1 + obs2);
				}
			}
			return sumSq;
		}

		/// <summary>
		/// <p>Returns the <i>observed significance level</i>, or <a href=
		/// "http://www.cas.lancs.ac.uk/glossary_v1.1/hyptest.html#pvalue">
		/// p-value</a>, associated with a Chi-Square two sample test comparing
		/// bin frequency counts in <code>observed1</code> and
		/// <code>observed2</code>.
		/// </p>
		/// <p>The number returned is the smallest significance level at which one
		/// can reject the null hypothesis that the observed counts conform to the
		/// same distribution.
		/// </p>
		/// <p>See <seealso cref="#chiSquareDataSetsComparison(long[], long[])"/> for details
		/// on the formula used to compute the test statistic. The degrees of
		/// of freedom used to perform the test is one less than the common length
		/// of the input observed count arrays.
		/// </p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>Observed counts must be non-negative.
		/// </li>
		/// <li>Observed counts for a specific bin must not both be zero.
		/// </li>
		/// <li>Observed counts for a specific sample must not all be 0.
		/// </li>
		/// <li>The arrays <code>observed1</code> and <code>observed2</code> must
		/// have the same length and
		/// their common length must be at least 2.
		/// </li></ul><p>
		/// If any of the preconditions are not met, an
		/// <code>IllegalArgumentException</code> is thrown.</p>
		/// </summary>
		/// <param name="observed1"> array of observed frequency counts of the first data set </param>
		/// <param name="observed2"> array of observed frequency counts of the second data set </param>
		/// <returns> p-value </returns>
		/// <exception cref="DimensionMismatchException"> the the length of the arrays does not match </exception>
		/// <exception cref="NotPositiveException"> if any entries in <code>observed1</code> or
		/// <code>observed2</code> are negative </exception>
		/// <exception cref="ZeroException"> if either all counts of <code>observed1</code> or
		/// <code>observed2</code> are zero, or if the count at the same index is zero
		/// for both arrays </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs computing the p-value
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double chiSquareTestDataSetsComparison(long[] observed1, long[] observed2) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.ZeroException, org.apache.commons.math3.exception.MaxCountExceededException
		public virtual double chiSquareTestDataSetsComparison(long[] observed1, long[] observed2)
		{

			ChiSquaredDistribution distribution;
			distribution = new ChiSquaredDistribution((double) observed1.Length - 1);
			return 1 - distribution.cumulativeProbability(chiSquareDataSetsComparison(observed1, observed2));

		}

		/// <summary>
		/// <p>Performs a Chi-Square two sample test comparing two binned data
		/// sets. The test evaluates the null hypothesis that the two lists of
		/// observed counts conform to the same frequency distribution, with
		/// significance level <code>alpha</code>.  Returns true iff the null
		/// hypothesis can be rejected with 100 * (1 - alpha) percent confidence.
		/// </p>
		/// <p>See <seealso cref="#chiSquareDataSetsComparison(long[], long[])"/> for
		/// details on the formula used to compute the Chisquare statistic used
		/// in the test. The degrees of of freedom used to perform the test is
		/// one less than the common length of the input observed count arrays.
		/// </p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>Observed counts must be non-negative.
		/// </li>
		/// <li>Observed counts for a specific bin must not both be zero.
		/// </li>
		/// <li>Observed counts for a specific sample must not all be 0.
		/// </li>
		/// <li>The arrays <code>observed1</code> and <code>observed2</code> must
		/// have the same length and their common length must be at least 2.
		/// </li>
		/// <li> <code> 0 < alpha < 0.5 </code>
		/// </li></ul><p>
		/// If any of the preconditions are not met, an
		/// <code>IllegalArgumentException</code> is thrown.</p>
		/// </summary>
		/// <param name="observed1"> array of observed frequency counts of the first data set </param>
		/// <param name="observed2"> array of observed frequency counts of the second data set </param>
		/// <param name="alpha"> significance level of the test </param>
		/// <returns> true iff null hypothesis can be rejected with confidence
		/// 1 - alpha </returns>
		/// <exception cref="DimensionMismatchException"> the the length of the arrays does not match </exception>
		/// <exception cref="NotPositiveException"> if any entries in <code>observed1</code> or
		/// <code>observed2</code> are negative </exception>
		/// <exception cref="ZeroException"> if either all counts of <code>observed1</code> or
		/// <code>observed2</code> are zero, or if the count at the same index is zero
		/// for both arrays </exception>
		/// <exception cref="OutOfRangeException"> if <code>alpha</code> is not in the range (0, 0.5] </exception>
		/// <exception cref="MaxCountExceededException"> if an error occurs performing the test
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean chiSquareTestDataSetsComparison(final long[] observed1, final long[] observed2, final double alpha) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.ZeroException, org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual bool chiSquareTestDataSetsComparison(long[] observed1, long[] observed2, double alpha)
		{

			if (alpha <= 0 || alpha > 0.5)
			{
				throw new OutOfRangeException(LocalizedFormats.OUT_OF_BOUND_SIGNIFICANCE_LEVEL, alpha, 0, 0.5);
			}
			return chiSquareTestDataSetsComparison(observed1, observed2) < alpha;

		}

		/// <summary>
		/// Checks to make sure that the input long[][] array is rectangular,
		/// has at least 2 rows and 2 columns, and has all non-negative entries.
		/// </summary>
		/// <param name="in"> input 2-way table to check </param>
		/// <exception cref="NullArgumentException"> if the array is null </exception>
		/// <exception cref="DimensionMismatchException"> if the array is not valid </exception>
		/// <exception cref="NotPositiveException"> if the array contains any negative entries </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkArray(final long[][] in) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NotPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void checkArray(long[][] @in)
		{

			if (@in.Length < 2)
			{
				throw new DimensionMismatchException(@in.Length, 2);
			}

			if (@in[0].Length < 2)
			{
				throw new DimensionMismatchException(@in[0].Length, 2);
			}

			MathArrays.checkRectangular(@in);
			MathArrays.checkNonNegative(@in);

		}

	}

}
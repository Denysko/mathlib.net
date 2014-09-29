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

namespace mathlib.stat.inference
{


	using RealDistribution = mathlib.distribution.RealDistribution;
	using InsufficientDataException = mathlib.exception.InsufficientDataException;
	using MathArithmeticException = mathlib.exception.MathArithmeticException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using TooManyIterationsException = mathlib.exception.TooManyIterationsException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using BigFraction = mathlib.fraction.BigFraction;
	using BigFractionField = mathlib.fraction.BigFractionField;
	using FractionConversionException = mathlib.fraction.FractionConversionException;
	using mathlib.linear;
	using Array2DRowRealMatrix = mathlib.linear.Array2DRowRealMatrix;
	using mathlib.linear;
	using RealMatrix = mathlib.linear.RealMatrix;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;
	using CombinatoricsUtils = mathlib.util.CombinatoricsUtils;
	using FastMath = mathlib.util.FastMath;
	using MathArrays = mathlib.util.MathArrays;

	/// <summary>
	/// Implementation of the <a href="http://en.wikipedia.org/wiki/Kolmogorov-Smirnov_test">
	/// Kolmogorov-Smirnov (K-S) test</a> for equality of continuous distributions.
	/// <p>
	/// The K-S test uses a statistic based on the maximum deviation of the empirical distribution of
	/// sample data points from the distribution expected under the null hypothesis. For one-sample tests
	/// evaluating the null hypothesis that a set of sample data points follow a given distribution, the
	/// test statistic is \(D_n=\sup_x |F_n(x)-F(x)|\), where \(F\) is the expected distribution and
	/// \(F_n\) is the empirical distribution of the \(n\) sample data points. The distribution of
	/// \(D_n\) is estimated using a method based on [1] with certain quick decisions for extreme values
	/// given in [2].
	/// </p>
	/// <p>
	/// Two-sample tests are also supported, evaluating the null hypothesis that the two samples
	/// {@code x} and {@code y} come from the same underlying distribution. In this case, the test
	/// statistic is \(D_{n,m}=\sup_t | F_n(t)-F_m(t)|\) where \(n\) is the length of {@code x}, \(m\) is
	/// the length of {@code y}, \(F_n\) is the empirical distribution that puts mass \(1/n\) at each of
	/// the values in {@code x} and \(F_m\) is the empirical distribution of the {@code y} values. The
	/// default 2-sample test method, <seealso cref="#kolmogorovSmirnovTest(double[], double[])"/> works as
	/// follows:
	/// <ul>
	/// <li>For very small samples (where the product of the sample sizes is less than
	/// {@value #SMALL_SAMPLE_PRODUCT}), the exact distribution is used to compute the p-value for the
	/// 2-sample test.</li>
	/// <li>For mid-size samples (product of sample sizes greater than or equal to
	/// {@value #SMALL_SAMPLE_PRODUCT} but less than {@value #LARGE_SAMPLE_PRODUCT}), Monte Carlo
	/// simulation is used to compute the p-value. The simulation randomly generates partitions of \(m +
	/// n\) into an \(m\)-set and an \(n\)-set and reports the proportion that give \(D\) values
	/// exceeding the observed value.</li>
	/// <li>When the product of the sample sizes exceeds {@value #LARGE_SAMPLE_PRODUCT}, the asymptotic
	/// distribution of \(D_{n,m}\) is used. See <seealso cref="#approximateP(double, int, int)"/> for details on
	/// the approximation.</li>
	/// </ul>
	/// </p>
	/// <p>
	/// In the two-sample case, \(D_{n,m}\) has a discrete distribution. This makes the p-value
	/// associated with the null hypothesis \(H_0 : D_{n,m} \ge d \) differ from \(H_0 : D_{n,m} > d \)
	/// by the mass of the observed value \(d\). To distinguish these, the two-sample tests use a boolean
	/// {@code strict} parameter. This parameter is ignored for large samples.
	/// </p>
	/// <p>
	/// The methods used by the 2-sample default implementation are also exposed directly:
	/// <ul>
	/// <li><seealso cref="#exactP(double, int, int, boolean)"/> computes exact 2-sample p-values</li>
	/// <li><seealso cref="#monteCarloP(double, int, int, boolean, int)"/> computes 2-sample p-values by Monte
	/// Carlo simulation</li>
	/// <li><seealso cref="#approximateP(double, int, int)"/> uses the asymptotic distribution The {@code boolean}
	/// arguments in the first two methods allow the probability used to estimate the p-value to be
	/// expressed using strict or non-strict inequality. See
	/// <seealso cref="#kolmogorovSmirnovTest(double[], double[], boolean)"/>.</li>
	/// </ul>
	/// </p>
	/// <p>
	/// References:
	/// <ul>
	/// <li>[1] <a href="http://www.jstatsoft.org/v08/i18/"> Evaluating Kolmogorov's Distribution</a> by
	/// George Marsaglia, Wai Wan Tsang, and Jingbo Wang</li>
	/// <li>[2] <a href="http://www.jstatsoft.org/v39/i11/"> Computing the Two-Sided Kolmogorov-Smirnov
	/// Distribution</a> by Richard Simard and Pierre L'Ecuyer</li>
	/// </ul>
	/// <br/>
	/// Note that [1] contains an error in computing h, refer to <a
	/// href="https://issues.apache.org/jira/browse/MATH-437">MATH-437</a> for details.
	/// </p>
	/// 
	/// @since 3.3
	/// @version $Id: KolmogorovSmirnovTest.java 1591211 2014-04-30 08:20:51Z luc $
	/// </summary>
	public class KolmogorovSmirnovTest
	{

		/// <summary>
		/// Bound on the number of partial sums in <seealso cref="#ksSum(double, double, int)"/>
		/// </summary>
		protected internal const int MAXIMUM_PARTIAL_SUM_COUNT = 100000;

		/// <summary>
		/// Convergence criterion for <seealso cref="#ksSum(double, double, int)"/> </summary>
		protected internal const double KS_SUM_CAUCHY_CRITERION = 1E-20;

		/// <summary>
		/// When product of sample sizes is less than this value, 2-sample K-S test is exact </summary>
		protected internal const int SMALL_SAMPLE_PRODUCT = 200;

		/// <summary>
		/// When product of sample sizes exceeds this value, 2-sample K-S test uses asymptotic
		/// distribution for strict inequality p-value.
		/// </summary>
		protected internal const int LARGE_SAMPLE_PRODUCT = 10000;

		/// <summary>
		/// Default number of iterations used by <seealso cref="#monteCarloP(double, int, int, boolean, int)"/> </summary>
		protected internal const int MONTE_CARLO_ITERATIONS = 1000000;

		/// <summary>
		/// Random data generator used by <seealso cref="#monteCarloP(double, int, int, boolean, int)"/> </summary>
		private readonly RandomGenerator rng;

		/// <summary>
		/// Construct a KolmogorovSmirnovTest instance with a default random data generator.
		/// </summary>
		public KolmogorovSmirnovTest()
		{
			rng = new Well19937c();
		}

		/// <summary>
		/// Construct a KolmogorovSmirnovTest with the provided random data generator.
		/// </summary>
		/// <param name="rng"> random data generator used by <seealso cref="#monteCarloP(double, int, int, boolean, int)"/> </param>
		public KolmogorovSmirnovTest(RandomGenerator rng)
		{
			this.rng = rng;
		}

		/// <summary>
		/// Computes the <i>p-value</i>, or <i>observed significance level</i>, of a one-sample <a
		/// href="http://en.wikipedia.org/wiki/Kolmogorov-Smirnov_test"> Kolmogorov-Smirnov test</a>
		/// evaluating the null hypothesis that {@code data} conforms to {@code distribution}. If
		/// {@code exact} is true, the distribution used to compute the p-value is computed using
		/// extended precision. See <seealso cref="#cdfExact(double, int)"/>.
		/// </summary>
		/// <param name="distribution"> reference distribution </param>
		/// <param name="data"> sample being being evaluated </param>
		/// <param name="exact"> whether or not to force exact computation of the p-value </param>
		/// <returns> the p-value associated with the null hypothesis that {@code data} is a sample from
		///         {@code distribution} </returns>
		/// <exception cref="InsufficientDataException"> if {@code data} does not have length at least 2 </exception>
		/// <exception cref="NullArgumentException"> if {@code data} is null </exception>
		public virtual double kolmogorovSmirnovTest(RealDistribution distribution, double[] data, bool exact)
		{
			return 1d - cdf(kolmogorovSmirnovStatistic(distribution, data), data.Length, exact);
		}

		/// <summary>
		/// Computes the one-sample Kolmogorov-Smirnov test statistic, \(D_n=\sup_x |F_n(x)-F(x)|\) where
		/// \(F\) is the distribution (cdf) function associated with {@code distribution}, \(n\) is the
		/// length of {@code data} and \(F_n\) is the empirical distribution that puts mass \(1/n\) at
		/// each of the values in {@code data}.
		/// </summary>
		/// <param name="distribution"> reference distribution </param>
		/// <param name="data"> sample being evaluated </param>
		/// <returns> Kolmogorov-Smirnov statistic \(D_n\) </returns>
		/// <exception cref="InsufficientDataException"> if {@code data} does not have length at least 2 </exception>
		/// <exception cref="NullArgumentException"> if {@code data} is null </exception>
		public virtual double kolmogorovSmirnovStatistic(RealDistribution distribution, double[] data)
		{
			checkArray(data);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = data.length;
			int n = data.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double nd = n;
			double nd = n;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dataCopy = new double[n];
			double[] dataCopy = new double[n];
			Array.Copy(data, 0, dataCopy, 0, n);
			Arrays.sort(dataCopy);
			double d = 0d;
			for (int i = 1; i <= n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yi = distribution.cumulativeProbability(dataCopy[i - 1]);
				double yi = distribution.cumulativeProbability(dataCopy[i - 1]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double currD = mathlib.util.FastMath.max(yi - (i - 1) / nd, i / nd - yi);
				double currD = FastMath.max(yi - (i - 1) / nd, i / nd - yi);
				if (currD > d)
				{
					d = currD;
				}
			}
			return d;
		}

		/// <summary>
		/// Computes the <i>p-value</i>, or <i>observed significance level</i>, of a two-sample <a
		/// href="http://en.wikipedia.org/wiki/Kolmogorov-Smirnov_test"> Kolmogorov-Smirnov test</a>
		/// evaluating the null hypothesis that {@code x} and {@code y} are samples drawn from the same
		/// probability distribution. Specifically, what is returned is an estimate of the probability
		/// that the <seealso cref="#kolmogorovSmirnovStatistic(double[], double[])"/> associated with a randomly
		/// selected partition of the combined sample into subsamples of sizes {@code x.length} and
		/// {@code y.length} will strictly exceed (if {@code strict} is {@code true}) or be at least as
		/// large as {@code strict = false}) as {@code kolmogorovSmirnovStatistic(x, y)}.
		/// <ul>
		/// <li>For very small samples (where the product of the sample sizes is less than
		/// {@value #SMALL_SAMPLE_PRODUCT}), the exact distribution is used to compute the p-value. This
		/// is accomplished by enumerating all partitions of the combined sample into two subsamples of
		/// the respective sample sizes, computing \(D_{n,m}\) for each partition and returning the
		/// proportion of partitions that give \(D\) values exceeding the observed value.</li>
		/// <li>For mid-size samples (product of sample sizes greater than or equal to
		/// {@value #SMALL_SAMPLE_PRODUCT} but less than {@value #LARGE_SAMPLE_PRODUCT}), Monte Carlo
		/// simulation is used to compute the p-value. The simulation randomly generates partitions and
		/// reports the proportion that give \(D\) values exceeding the observed value.</li>
		/// <li>When the product of the sample sizes exceeds {@value #LARGE_SAMPLE_PRODUCT}, the
		/// asymptotic distribution of \(D_{n,m}\) is used. See <seealso cref="#approximateP(double, int, int)"/>
		/// for details on the approximation.</li>
		/// </ul>
		/// </summary>
		/// <param name="x"> first sample dataset </param>
		/// <param name="y"> second sample dataset </param>
		/// <param name="strict"> whether or not the probability to compute is expressed as a strict inequality
		///        (ignored for large samples) </param>
		/// <returns> p-value associated with the null hypothesis that {@code x} and {@code y} represent
		///         samples from the same distribution </returns>
		/// <exception cref="InsufficientDataException"> if either {@code x} or {@code y} does not have length at
		///         least 2 </exception>
		/// <exception cref="NullArgumentException"> if either {@code x} or {@code y} is null </exception>
		public virtual double kolmogorovSmirnovTest(double[] x, double[] y, bool strict)
		{
			if (x.Length * y.Length < SMALL_SAMPLE_PRODUCT)
			{
				return exactP(kolmogorovSmirnovStatistic(x, y), x.Length, y.Length, strict);
			}
			if (x.Length * y.Length < LARGE_SAMPLE_PRODUCT)
			{
				return monteCarloP(kolmogorovSmirnovStatistic(x, y), x.Length, y.Length, strict, MONTE_CARLO_ITERATIONS);
			}
			return approximateP(kolmogorovSmirnovStatistic(x, y), x.Length, y.Length);
		}

		/// <summary>
		/// Computes the <i>p-value</i>, or <i>observed significance level</i>, of a two-sample <a
		/// href="http://en.wikipedia.org/wiki/Kolmogorov-Smirnov_test"> Kolmogorov-Smirnov test</a>
		/// evaluating the null hypothesis that {@code x} and {@code y} are samples drawn from the same
		/// probability distribution. Assumes the strict form of the inequality used to compute the
		/// p-value. See <seealso cref="#kolmogorovSmirnovTest(RealDistribution, double[], boolean)"/>.
		/// </summary>
		/// <param name="x"> first sample dataset </param>
		/// <param name="y"> second sample dataset </param>
		/// <returns> p-value associated with the null hypothesis that {@code x} and {@code y} represent
		///         samples from the same distribution </returns>
		/// <exception cref="InsufficientDataException"> if either {@code x} or {@code y} does not have length at
		///         least 2 </exception>
		/// <exception cref="NullArgumentException"> if either {@code x} or {@code y} is null </exception>
		public virtual double kolmogorovSmirnovTest(double[] x, double[] y)
		{
			return kolmogorovSmirnovTest(x, y, true);
		}

		/// <summary>
		/// Computes the two-sample Kolmogorov-Smirnov test statistic, \(D_{n,m}=\sup_x |F_n(x)-F_m(x)|\)
		/// where \(n\) is the length of {@code x}, \(m\) is the length of {@code y}, \(F_n\) is the
		/// empirical distribution that puts mass \(1/n\) at each of the values in {@code x} and \(F_m\)
		/// is the empirical distribution of the {@code y} values.
		/// </summary>
		/// <param name="x"> first sample </param>
		/// <param name="y"> second sample </param>
		/// <returns> test statistic \(D_{n,m}\) used to evaluate the null hypothesis that {@code x} and
		///         {@code y} represent samples from the same underlying distribution </returns>
		/// <exception cref="InsufficientDataException"> if either {@code x} or {@code y} does not have length at
		///         least 2 </exception>
		/// <exception cref="NullArgumentException"> if either {@code x} or {@code y} is null </exception>
		public virtual double kolmogorovSmirnovStatistic(double[] x, double[] y)
		{
			checkArray(x);
			checkArray(y);
			// Copy and sort the sample arrays
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] sx = mathlib.util.MathArrays.copyOf(x);
			double[] sx = MathArrays.copyOf(x);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] sy = mathlib.util.MathArrays.copyOf(y);
			double[] sy = MathArrays.copyOf(y);
			Arrays.sort(sx);
			Arrays.sort(sy);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = sx.length;
			int n = sx.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = sy.length;
			int m = sy.Length;

			// Find the max difference between cdf_x and cdf_y
			double supD = 0d;
			// First walk x points
			for (int i = 0; i < n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cdf_x = (i + 1d) / n;
				double cdf_x = (i + 1d) / n;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int yIndex = java.util.Arrays.binarySearch(sy, sx[i]);
				int yIndex = Arrays.binarySearch(sy, sx[i]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cdf_y = yIndex >= 0 ? (yIndex + 1d) / m : (-yIndex - 1d) / m;
				double cdf_y = yIndex >= 0 ? (yIndex + 1d) / m : (-yIndex - 1d) / m;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double curD = mathlib.util.FastMath.abs(cdf_x - cdf_y);
				double curD = FastMath.abs(cdf_x - cdf_y);
				if (curD > supD)
				{
					supD = curD;
				}
			}
			// Now look at y
			for (int i = 0; i < m; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cdf_y = (i + 1d) / m;
				double cdf_y = (i + 1d) / m;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int xIndex = java.util.Arrays.binarySearch(sx, sy[i]);
				int xIndex = Arrays.binarySearch(sx, sy[i]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cdf_x = xIndex >= 0 ? (xIndex + 1d) / n : (-xIndex - 1d) / n;
				double cdf_x = xIndex >= 0 ? (xIndex + 1d) / n : (-xIndex - 1d) / n;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double curD = mathlib.util.FastMath.abs(cdf_x - cdf_y);
				double curD = FastMath.abs(cdf_x - cdf_y);
				if (curD > supD)
				{
					supD = curD;
				}
			}
			return supD;
		}

		/// <summary>
		/// Computes the <i>p-value</i>, or <i>observed significance level</i>, of a one-sample <a
		/// href="http://en.wikipedia.org/wiki/Kolmogorov-Smirnov_test"> Kolmogorov-Smirnov test</a>
		/// evaluating the null hypothesis that {@code data} conforms to {@code distribution}.
		/// </summary>
		/// <param name="distribution"> reference distribution </param>
		/// <param name="data"> sample being being evaluated </param>
		/// <returns> the p-value associated with the null hypothesis that {@code data} is a sample from
		///         {@code distribution} </returns>
		/// <exception cref="InsufficientDataException"> if {@code data} does not have length at least 2 </exception>
		/// <exception cref="NullArgumentException"> if {@code data} is null </exception>
		public virtual double kolmogorovSmirnovTest(RealDistribution distribution, double[] data)
		{
			return kolmogorovSmirnovTest(distribution, data, false);
		}

		/// <summary>
		/// Performs a <a href="http://en.wikipedia.org/wiki/Kolmogorov-Smirnov_test"> Kolmogorov-Smirnov
		/// test</a> evaluating the null hypothesis that {@code data} conforms to {@code distribution}.
		/// </summary>
		/// <param name="distribution"> reference distribution </param>
		/// <param name="data"> sample being being evaluated </param>
		/// <param name="alpha"> significance level of the test </param>
		/// <returns> true iff the null hypothesis that {@code data} is a sample from {@code distribution}
		///         can be rejected with confidence 1 - {@code alpha} </returns>
		/// <exception cref="InsufficientDataException"> if {@code data} does not have length at least 2 </exception>
		/// <exception cref="NullArgumentException"> if {@code data} is null </exception>
		public virtual bool kolmogorovSmirnovTest(RealDistribution distribution, double[] data, double alpha)
		{
			if ((alpha <= 0) || (alpha > 0.5))
			{
				throw new OutOfRangeException(LocalizedFormats.OUT_OF_BOUND_SIGNIFICANCE_LEVEL, alpha, 0, 0.5);
			}
			return kolmogorovSmirnovTest(distribution, data) < alpha;
		}

		/// <summary>
		/// Calculates \(P(D_n < d)\) using the method described in [1] with quick decisions for extreme
		/// values given in [2] (see above). The result is not exact as with
		/// <seealso cref="#cdfExact(double, int)"/> because calculations are based on
		/// {@code double} rather than <seealso cref="mathlib.fraction.BigFraction"/>.
		/// </summary>
		/// <param name="d"> statistic </param>
		/// <param name="n"> sample size </param>
		/// <returns> \(P(D_n < d)\) </returns>
		/// <exception cref="MathArithmeticException"> if algorithm fails to convert {@code h} to a
		///         <seealso cref="mathlib.fraction.BigFraction"/> in expressing {@code d} as \((k
		///         - h) / m\) for integer {@code k, m} and \(0 \le h < 1\) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double cdf(double d, int n) throws mathlib.exception.MathArithmeticException
		public virtual double cdf(double d, int n)
		{
			return cdf(d, n, false);
		}

		/// <summary>
		/// Calculates {@code P(D_n < d)}. The result is exact in the sense that BigFraction/BigReal is
		/// used everywhere at the expense of very slow execution time. Almost never choose this in real
		/// applications unless you are very sure; this is almost solely for verification purposes.
		/// Normally, you would choose <seealso cref="#cdf(double, int)"/>. See the class
		/// javadoc for definitions and algorithm description.
		/// </summary>
		/// <param name="d"> statistic </param>
		/// <param name="n"> sample size </param>
		/// <returns> \(P(D_n < d)\) </returns>
		/// <exception cref="MathArithmeticException"> if the algorithm fails to convert {@code h} to a
		///         <seealso cref="mathlib.fraction.BigFraction"/> in expressing {@code d} as \((k
		///         - h) / m\) for integer {@code k, m} and \(0 \le h < 1\) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double cdfExact(double d, int n) throws mathlib.exception.MathArithmeticException
		public virtual double cdfExact(double d, int n)
		{
			return cdf(d, n, true);
		}

		/// <summary>
		/// Calculates {@code P(D_n < d)} using method described in [1] with quick decisions for extreme
		/// values given in [2] (see above).
		/// </summary>
		/// <param name="d"> statistic </param>
		/// <param name="n"> sample size </param>
		/// <param name="exact"> whether the probability should be calculated exact using
		///        <seealso cref="mathlib.fraction.BigFraction"/> everywhere at the expense of
		///        very slow execution time, or if {@code double} should be used convenient places to
		///        gain speed. Almost never choose {@code true} in real applications unless you are very
		///        sure; {@code true} is almost solely for verification purposes. </param>
		/// <returns> \(P(D_n < d)\) </returns>
		/// <exception cref="MathArithmeticException"> if algorithm fails to convert {@code h} to a
		///         <seealso cref="mathlib.fraction.BigFraction"/> in expressing {@code d} as \((k
		///         - h) / m\) for integer {@code k, m} and \(0 \le h < 1\). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double cdf(double d, int n, boolean exact) throws mathlib.exception.MathArithmeticException
		public virtual double cdf(double d, int n, bool exact)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ninv = 1 / ((double) n);
			double ninv = 1 / ((double) n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ninvhalf = 0.5 * ninv;
			double ninvhalf = 0.5 * ninv;

			if (d <= ninvhalf)
			{
				return 0;
			}
			else if (ninvhalf < d && d <= ninv)
			{
				double res = 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double f = 2 * d - ninv;
				double f = 2 * d - ninv;
				// n! f^n = n*f * (n-1)*f * ... * 1*x
				for (int i = 1; i <= n; ++i)
				{
					res *= i * f;
				}
				return res;
			}
			else if (1 - ninv <= d && d < 1)
			{
				return 1 - 2 * Math.Pow(1 - d, n);
			}
			else if (1 <= d)
			{
				return 1;
			}
			return exact ? exactK(d, n) : roundedK(d, n);
		}

		/// <summary>
		/// Calculates the exact value of {@code P(D_n < d)} using the method described in [1] (reference
		/// in class javadoc above) and <seealso cref="mathlib.fraction.BigFraction"/> (see
		/// above).
		/// </summary>
		/// <param name="d"> statistic </param>
		/// <param name="n"> sample size </param>
		/// <returns> the two-sided probability of \(P(D_n < d)\) </returns>
		/// <exception cref="MathArithmeticException"> if algorithm fails to convert {@code h} to a
		///         <seealso cref="mathlib.fraction.BigFraction"/> in expressing {@code d} as \((k
		///         - h) / m\) for integer {@code k, m} and \(0 \le h < 1\). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private double exactK(double d, int n) throws mathlib.exception.MathArithmeticException
		private double exactK(double d, int n)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int k = (int) Math.ceil(n * d);
			int k = (int) Math.Ceiling(n * d);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.FieldMatrix<mathlib.fraction.BigFraction> H = this.createH(d, n);
			FieldMatrix<BigFraction> H = this.createH(d, n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.FieldMatrix<mathlib.fraction.BigFraction> Hpower = H.power(n);
			FieldMatrix<BigFraction> Hpower = H.power(n);

			BigFraction pFrac = Hpower.getEntry(k - 1, k - 1);

			for (int i = 1; i <= n; ++i)
			{
				pFrac = pFrac.multiply(i).divide(n);
			}

			/*
			 * BigFraction.doubleValue converts numerator to double and the denominator to double and
			 * divides afterwards. That gives NaN quite easy. This does not (scale is the number of
			 * digits):
			 */
			return (double)pFrac.bigDecimalValue(20, decimal.ROUND_HALF_UP);
		}

		/// <summary>
		/// Calculates {@code P(D_n < d)} using method described in [1] and doubles (see above).
		/// </summary>
		/// <param name="d"> statistic </param>
		/// <param name="n"> sample size </param>
		/// <returns> the two-sided probability of \(P(D_n < d)\) </returns>
		/// <exception cref="MathArithmeticException"> if algorithm fails to convert {@code h} to a
		///         <seealso cref="mathlib.fraction.BigFraction"/> in expressing {@code d} as \((k
		///         - h) / m\ for integer {@code k, m} and \(0 <= h < 1\). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private double roundedK(double d, int n) throws mathlib.exception.MathArithmeticException
		private double roundedK(double d, int n)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int k = (int) Math.ceil(n * d);
			int k = (int) Math.Ceiling(n * d);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.FieldMatrix<mathlib.fraction.BigFraction> HBigFraction = this.createH(d, n);
			FieldMatrix<BigFraction> HBigFraction = this.createH(d, n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = HBigFraction.getRowDimension();
			int m = HBigFraction.RowDimension;

			/*
			 * Here the rounding part comes into play: use RealMatrix instead of
			 * FieldMatrix<BigFraction>
			 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix H = new mathlib.linear.Array2DRowRealMatrix(m, m);
			RealMatrix H = new Array2DRowRealMatrix(m, m);
			for (int i = 0; i < m; ++i)
			{
				for (int j = 0; j < m; ++j)
				{
					H.setEntry(i, j, (double)HBigFraction.getEntry(i, j));
				}
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix Hpower = H.power(n);
			RealMatrix Hpower = H.power(n);
			double pFrac = Hpower.getEntry(k - 1, k - 1);
			for (int i = 1; i <= n; ++i)
			{
				pFrac *= (double) i / (double) n;
			}
			return pFrac;
		}

		/// <summary>
		///*
		/// Creates {@code H} of size {@code m x m} as described in [1] (see above).
		/// </summary>
		/// <param name="d"> statistic </param>
		/// <param name="n"> sample size </param>
		/// <returns> H matrix </returns>
		/// <exception cref="NumberIsTooLargeException"> if fractional part is greater than 1 </exception>
		/// <exception cref="FractionConversionException"> if algorithm fails to convert {@code h} to a
		///         <seealso cref="mathlib.fraction.BigFraction"/> in expressing {@code d} as \((k
		///         - h) / m\) for integer {@code k, m} and \(0 <= h < 1\). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private mathlib.linear.FieldMatrix<mathlib.fraction.BigFraction> createH(double d, int n) throws mathlib.exception.NumberIsTooLargeException, mathlib.fraction.FractionConversionException
		private FieldMatrix<BigFraction> createH(double d, int n)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int k = (int) Math.ceil(n * d);
			int k = (int) Math.Ceiling(n * d);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = 2 * k - 1;
			int m = 2 * k - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double hDouble = k - n * d;
			double hDouble = k - n * d;
			if (hDouble >= 1)
			{
				throw new NumberIsTooLargeException(hDouble, 1.0, false);
			}
			BigFraction h = null;
			try
			{
				h = new BigFraction(hDouble, 1.0e-20, 10000);
			}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not allowed in C#:
//ORIGINAL LINE: catch (final mathlib.fraction.FractionConversionException e1)
			catch (FractionConversionException e1)
			{
				try
				{
					h = new BigFraction(hDouble, 1.0e-10, 10000);
				}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not allowed in C#:
//ORIGINAL LINE: catch (final mathlib.fraction.FractionConversionException e2)
				catch (FractionConversionException e2)
				{
					h = new BigFraction(hDouble, 1.0e-5, 10000);
				}
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction[][] Hdata = new mathlib.fraction.BigFraction[m][m];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: BigFraction[][] Hdata = new BigFraction[m][m];
			BigFraction[][] Hdata = RectangularArrays.ReturnRectangularBigFractionArray(m, m);

			/*
			 * Start by filling everything with either 0 or 1.
			 */
			for (int i = 0; i < m; ++i)
			{
				for (int j = 0; j < m; ++j)
				{
					if (i - j + 1 < 0)
					{
						Hdata[i][j] = BigFraction.ZERO;
					}
					else
					{
						Hdata[i][j] = BigFraction.ONE;
					}
				}
			}

			/*
			 * Setting up power-array to avoid calculating the same value twice: hPowers[0] = h^1 ...
			 * hPowers[m-1] = h^m
			 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction[] hPowers = new mathlib.fraction.BigFraction[m];
			BigFraction[] hPowers = new BigFraction[m];
			hPowers[0] = h;
			for (int i = 1; i < m; ++i)
			{
				hPowers[i] = h.multiply(hPowers[i - 1]);
			}

			/*
			 * First column and last row has special values (each other reversed).
			 */
			for (int i = 0; i < m; ++i)
			{
				Hdata[i][0] = Hdata[i][0].subtract(hPowers[i]);
				Hdata[m - 1][i] = Hdata[m - 1][i].subtract(hPowers[m - i - 1]);
			}

			/*
			 * [1] states: "For 1/2 < h < 1 the bottom left element of the matrix should be (1 - 2*h^m +
			 * (2h - 1)^m )/m!" Since 0 <= h < 1, then if h > 1/2 is sufficient to check:
			 */
			if (h.compareTo(BigFraction.ONE_HALF) == 1)
			{
				Hdata[m - 1][0] = Hdata[m - 1][0].add(h.multiply(2).subtract(1).pow(m));
			}

			/*
			 * Aside from the first column and last row, the (i, j)-th element is 1/(i - j + 1)! if i -
			 * j + 1 >= 0, else 0. 1's and 0's are already put, so only division with (i - j + 1)! is
			 * needed in the elements that have 1's. There is no need to calculate (i - j + 1)! and then
			 * divide - small steps avoid overflows. Note that i - j + 1 > 0 <=> i + 1 > j instead of
			 * j'ing all the way to m. Also note that it is started at g = 2 because dividing by 1 isn't
			 * really necessary.
			 */
			for (int i = 0; i < m; ++i)
			{
				for (int j = 0; j < i + 1; ++j)
				{
					if (i - j + 1 > 0)
					{
						for (int g = 2; g <= i - j + 1; ++g)
						{
							Hdata[i][j] = Hdata[i][j].divide(g);
						}
					}
				}
			}
			return new Array2DRowFieldMatrix<BigFraction>(BigFractionField.Instance, Hdata);
		}

		/// <summary>
		/// Verifies that {@code array} has length at least 2.
		/// </summary>
		/// <param name="array"> array to test </param>
		/// <exception cref="NullArgumentException"> if array is null </exception>
		/// <exception cref="InsufficientDataException"> if array is too short </exception>
		private void checkArray(double[] array)
		{
			if (array == null)
			{
				throw new NullArgumentException(LocalizedFormats.NULL_NOT_ALLOWED);
			}
			if (array.Length < 2)
			{
				throw new InsufficientDataException(LocalizedFormats.INSUFFICIENT_OBSERVED_POINTS_IN_SAMPLE, array.Length, 2);
			}
		}

		/// <summary>
		/// Computes \( 1 + 2 \sum_{i=1}^\infty (-1)^i e^{-2 i^2 t^2} \) stopping when successive partial
		/// sums are within {@code tolerance} of one another, or when {@code maxIterations} partial sums
		/// have been computed. If the sum does not converge before {@code maxIterations} iterations a
		/// <seealso cref="TooManyIterationsException"/> is thrown.
		/// </summary>
		/// <param name="t"> argument </param>
		/// <param name="tolerance"> Cauchy criterion for partial sums </param>
		/// <param name="maxIterations"> maximum number of partial sums to compute </param>
		/// <returns> Kolmogorov sum evaluated at t </returns>
		/// <exception cref="TooManyIterationsException"> if the series does not converge </exception>
		public virtual double ksSum(double t, double tolerance, int maxIterations)
		{
			// TODO: for small t (say less than 1), the alternative expansion in part 3 of [1]
			// from class javadoc should be used.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = -2 * t * t;
			double x = -2 * t * t;
			int sign = -1;
			long i = 1;
			double partialSum = 0.5d;
			double delta = 1;
			while (delta > tolerance && i < maxIterations)
			{
				delta = FastMath.exp(x * i * i);
				partialSum += sign * delta;
				sign *= -1;
				i++;
			}
			if (i == maxIterations)
			{
				throw new TooManyIterationsException(maxIterations);
			}
			return partialSum * 2;
		}

		/// <summary>
		/// Computes \(P(D_{n,m} > d)\) if {@code strict} is {@code true}; otherwise \(P(D_{n,m} \ge
		/// d)\), where \(D_{n,m}\) is the 2-sample Kolmogorov-Smirnov statistic. See
		/// <seealso cref="#kolmogorovSmirnovStatistic(double[], double[])"/> for the definition of \(D_{n,m}\).
		/// <p>
		/// The returned probability is exact, obtained by enumerating all partitions of {@code m + n}
		/// into {@code m} and {@code n} sets, computing \(D_{n,m}\) for each partition and counting the
		/// number of partitions that yield \(D_{n,m}\) values exceeding (resp. greater than or equal to)
		/// {@code d}.
		/// </p>
		/// <p>
		/// <strong>USAGE NOTE</strong>: Since this method enumerates all combinations in \({m+n} \choose
		/// {n}\), it is very slow if called for large {@code m, n}. For this reason,
		/// <seealso cref="#kolmogorovSmirnovTest(double[], double[])"/> uses this only for {@code m * n < }
		/// {@value #SMALL_SAMPLE_PRODUCT}.
		/// </p>
		/// </summary>
		/// <param name="d"> D-statistic value </param>
		/// <param name="n"> first sample size </param>
		/// <param name="m"> second sample size </param>
		/// <param name="strict"> whether or not the probability to compute is expressed as a strict inequality </param>
		/// <returns> probability that a randomly selected m-n partition of m + n generates \(D_{n,m}\)
		///         greater than (resp. greater than or equal to) {@code d} </returns>
		public virtual double exactP(double d, int n, int m, bool strict)
		{
			IEnumerator<int[]> combinationsIterator = CombinatoricsUtils.combinationsIterator(n + m, n);
			long tail = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] nSet = new double[n];
			double[] nSet = new double[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] mSet = new double[m];
			double[] mSet = new double[m];
			while (combinationsIterator.MoveNext())
			{
				// Generate an n-set
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] nSetI = combinationsIterator.Current;
				int[] nSetI = combinationsIterator.Current;
				// Copy the n-set to nSet and its complement to mSet
				int j = 0;
				int k = 0;
				for (int i = 0; i < n + m; i++)
				{
					if (j < n && nSetI[j] == i)
					{
						nSet[j++] = i;
					}
					else
					{
						mSet[k++] = i;
					}
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double curD = kolmogorovSmirnovStatistic(nSet, mSet);
				double curD = kolmogorovSmirnovStatistic(nSet, mSet);
				if (curD > d)
				{
					tail++;
				}
				else if (curD == d && !strict)
				{
					tail++;
				}
			}
			return (double) tail / (double) CombinatoricsUtils.binomialCoefficient(n + m, n);
		}

		/// <summary>
		/// Uses the Kolmogorov-Smirnov distribution to approximate \(P(D_{n,m} > d)\) where \(D_{n,m}\)
		/// is the 2-sample Kolmogorov-Smirnov statistic. See
		/// <seealso cref="#kolmogorovSmirnovStatistic(double[], double[])"/> for the definition of \(D_{n,m}\).
		/// <p>
		/// Specifically, what is returned is \(1 - k(d \sqrt{mn / (m + n)})\) where \(k(t) = 1 + 2
		/// \sum_{i=1}^\infty (-1)^i e^{-2 i^2 t^2}\). See <seealso cref="#ksSum(double, double, int)"/> for
		/// details on how convergence of the sum is determined. This implementation passes {@code ksSum}
		/// {@value #KS_SUM_CAUCHY_CRITERION} as {@code tolerance} and
		/// {@value #MAXIMUM_PARTIAL_SUM_COUNT} as {@code maxIterations}.
		/// </p>
		/// </summary>
		/// <param name="d"> D-statistic value </param>
		/// <param name="n"> first sample size </param>
		/// <param name="m"> second sample size </param>
		/// <returns> approximate probability that a randomly selected m-n partition of m + n generates
		///         \(D_{n,m}\) greater than {@code d} </returns>
		public virtual double approximateP(double d, int n, int m)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dm = m;
			double dm = m;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dn = n;
			double dn = n;
			return 1 - ksSum(d * FastMath.sqrt((dm * dn) / (dm + dn)), KS_SUM_CAUCHY_CRITERION, MAXIMUM_PARTIAL_SUM_COUNT);
		}

		/// <summary>
		/// Uses Monte Carlo simulation to approximate \(P(D_{n,m} > d)\) where \(D_{n,m}\) is the
		/// 2-sample Kolmogorov-Smirnov statistic. See
		/// <seealso cref="#kolmogorovSmirnovStatistic(double[], double[])"/> for the definition of \(D_{n,m}\).
		/// <p>
		/// The simulation generates {@code iterations} random partitions of {@code m + n} into an
		/// {@code n} set and an {@code m} set, computing \(D_{n,m}\) for each partition and returning
		/// the proportion of values that are greater than {@code d}, or greater than or equal to
		/// {@code d} if {@code strict} is {@code false}.
		/// </p>
		/// </summary>
		/// <param name="d"> D-statistic value </param>
		/// <param name="n"> first sample size </param>
		/// <param name="m"> second sample size </param>
		/// <param name="iterations"> number of random partitions to generate </param>
		/// <param name="strict"> whether or not the probability to compute is expressed as a strict inequality </param>
		/// <returns> proportion of randomly generated m-n partitions of m + n that result in \(D_{n,m}\)
		///         greater than (resp. greater than or equal to) {@code d} </returns>
		public virtual double monteCarloP(double d, int n, int m, bool strict, int iterations)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] nPlusMSet = mathlib.util.MathArrays.natural(m + n);
			int[] nPlusMSet = MathArrays.natural(m + n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] nSet = new double[n];
			double[] nSet = new double[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] mSet = new double[m];
			double[] mSet = new double[m];
			int tail = 0;
			for (int i = 0; i < iterations; i++)
			{
				copyPartition(nSet, mSet, nPlusMSet, n, m);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double curD = kolmogorovSmirnovStatistic(nSet, mSet);
				double curD = kolmogorovSmirnovStatistic(nSet, mSet);
				if (curD > d)
				{
					tail++;
				}
				else if (curD == d && !strict)
				{
					tail++;
				}
				MathArrays.shuffle(nPlusMSet, rng);
				Arrays.sort(nPlusMSet, 0, n);
			}
			return (double) tail / iterations;
		}

		/// <summary>
		/// Copies the first {@code n} elements of {@code nSetI} into {@code nSet} and its complement
		/// relative to {@code m + n} into {@code mSet}. For example, if {@code m = 3}, {@code n = 3} and
		/// {@code nSetI = [1,4,5,2,3,0]} then after this method returns, we will have
		/// {@code nSet = [1,4,5], mSet = [0,2,3]}.
		/// <p>
		/// <strong>Precondition:</strong> The first {@code n} elements of {@code nSetI} must be sorted
		/// in ascending order.
		/// </p>
		/// </summary>
		/// <param name="nSet"> array to fill with the first {@code n} elements of {@code nSetI} </param>
		/// <param name="mSet"> array to fill with the {@code m} complementary elements of {@code nSet} relative
		///        to {@code m + n} </param>
		/// <param name="nSetI"> array whose first {@code n} elements specify the members of {@code nSet} </param>
		/// <param name="n"> number of elements in the first output array </param>
		/// <param name="m"> number of elements in the second output array </param>
		private void copyPartition(double[] nSet, double[] mSet, int[] nSetI, int n, int m)
		{
			int j = 0;
			int k = 0;
			for (int i = 0; i < n + m; i++)
			{
				if (j < n && nSetI[j] == i)
				{
					nSet[j++] = i;
				}
				else
				{
					mSet[k++] = i;
				}
			}
		}
	}

}
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
namespace mathlib.stat.inference
{

	using NormalDistribution = mathlib.distribution.NormalDistribution;
	using ConvergenceException = mathlib.exception.ConvergenceException;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using NoDataException = mathlib.exception.NoDataException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using NaNStrategy = mathlib.stat.ranking.NaNStrategy;
	using NaturalRanking = mathlib.stat.ranking.NaturalRanking;
	using TiesStrategy = mathlib.stat.ranking.TiesStrategy;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// An implementation of the Mann-Whitney U test (also called Wilcoxon rank-sum test).
	/// 
	/// @version $Id: MannWhitneyUTest.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class MannWhitneyUTest
	{

		/// <summary>
		/// Ranking algorithm. </summary>
		private NaturalRanking naturalRanking;

		/// <summary>
		/// Create a test instance using where NaN's are left in place and ties get
		/// the average of applicable ranks. Use this unless you are very sure of
		/// what you are doing.
		/// </summary>
		public MannWhitneyUTest()
		{
			naturalRanking = new NaturalRanking(NaNStrategy.FIXED, TiesStrategy.AVERAGE);
		}

		/// <summary>
		/// Create a test instance using the given strategies for NaN's and ties.
		/// Only use this if you are sure of what you are doing.
		/// </summary>
		/// <param name="nanStrategy">
		///            specifies the strategy that should be used for Double.NaN's </param>
		/// <param name="tiesStrategy">
		///            specifies the strategy that should be used for ties </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MannWhitneyUTest(final mathlib.stat.ranking.NaNStrategy nanStrategy, final mathlib.stat.ranking.TiesStrategy tiesStrategy)
		public MannWhitneyUTest(NaNStrategy nanStrategy, TiesStrategy tiesStrategy)
		{
			naturalRanking = new NaturalRanking(nanStrategy, tiesStrategy);
		}

		/// <summary>
		/// Ensures that the provided arrays fulfills the assumptions.
		/// </summary>
		/// <param name="x"> first sample </param>
		/// <param name="y"> second sample </param>
		/// <exception cref="NullArgumentException"> if {@code x} or {@code y} are {@code null}. </exception>
		/// <exception cref="NoDataException"> if {@code x} or {@code y} are zero-length. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void ensureDataConformance(final double[] x, final double[] y) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void ensureDataConformance(double[] x, double[] y)
		{

			if (x == null || y == null)
			{
				throw new NullArgumentException();
			}
			if (x.Length == 0 || y.Length == 0)
			{
				throw new NoDataException();
			}
		}

		/// <summary>
		/// Concatenate the samples into one array. </summary>
		/// <param name="x"> first sample </param>
		/// <param name="y"> second sample </param>
		/// <returns> concatenated array </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double[] concatenateSamples(final double[] x, final double[] y)
		private double[] concatenateSamples(double[] x, double[] y)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] z = new double[x.length + y.length];
			double[] z = new double[x.Length + y.Length];

			Array.Copy(x, 0, z, 0, x.Length);
			Array.Copy(y, 0, z, x.Length, y.Length);

			return z;
		}

		/// <summary>
		/// Computes the <a
		/// href="http://en.wikipedia.org/wiki/Mann%E2%80%93Whitney_U"> Mann-Whitney
		/// U statistic</a> comparing mean for two independent samples possibly of
		/// different length.
		/// <p>
		/// This statistic can be used to perform a Mann-Whitney U test evaluating
		/// the null hypothesis that the two independent samples has equal mean.
		/// </p>
		/// <p>
		/// Let X<sub>i</sub> denote the i'th individual of the first sample and
		/// Y<sub>j</sub> the j'th individual in the second sample. Note that the
		/// samples would often have different length.
		/// </p>
		/// <p>
		/// <strong>Preconditions</strong>:
		/// <ul>
		/// <li>All observations in the two samples are independent.</li>
		/// <li>The observations are at least ordinal (continuous are also ordinal).</li>
		/// </ul>
		/// </p>
		/// </summary>
		/// <param name="x"> the first sample </param>
		/// <param name="y"> the second sample </param>
		/// <returns> Mann-Whitney U statistic (maximum of U<sup>x</sup> and U<sup>y</sup>) </returns>
		/// <exception cref="NullArgumentException"> if {@code x} or {@code y} are {@code null}. </exception>
		/// <exception cref="NoDataException"> if {@code x} or {@code y} are zero-length. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double mannWhitneyU(final double[] x, final double[] y) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double mannWhitneyU(double[] x, double[] y)
		{

			ensureDataConformance(x, y);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] z = concatenateSamples(x, y);
			double[] z = concatenateSamples(x, y);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] ranks = naturalRanking.rank(z);
			double[] ranks = naturalRanking.rank(z);

			double sumRankX = 0;

			/*
			 * The ranks for x is in the first x.length entries in ranks because x
			 * is in the first x.length entries in z
			 */
			for (int i = 0; i < x.Length; ++i)
			{
				sumRankX += ranks[i];
			}

			/*
			 * U1 = R1 - (n1 * (n1 + 1)) / 2 where R1 is sum of ranks for sample 1,
			 * e.g. x, n1 is the number of observations in sample 1.
			 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double U1 = sumRankX - (x.length * (x.length + 1)) / 2;
			double U1 = sumRankX - (x.Length * (x.Length + 1)) / 2;

			/*
			 * It can be shown that U1 + U2 = n1 * n2
			 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double U2 = x.length * y.length - U1;
			double U2 = x.Length * y.Length - U1;

			return FastMath.max(U1, U2);
		}

		/// <param name="Umin"> smallest Mann-Whitney U value </param>
		/// <param name="n1"> number of subjects in first sample </param>
		/// <param name="n2"> number of subjects in second sample </param>
		/// <returns> two-sided asymptotic p-value </returns>
		/// <exception cref="ConvergenceException"> if the p-value can not be computed
		/// due to a convergence error </exception>
		/// <exception cref="MaxCountExceededException"> if the maximum number of
		/// iterations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private double calculateAsymptoticPValue(final double Umin, final int n1, final int n2) throws mathlib.exception.ConvergenceException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private double calculateAsymptoticPValue(double Umin, int n1, int n2)
		{

			/* long multiplication to avoid overflow (double not used due to efficiency
			 * and to avoid precision loss)
			 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long n1n2prod = (long) n1 * n2;
			long n1n2prod = (long) n1 * n2;

			// http://en.wikipedia.org/wiki/Mann%E2%80%93Whitney_U#Normal_approximation
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double EU = n1n2prod / 2.0;
			double EU = n1n2prod / 2.0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double VarU = n1n2prod * (n1 + n2 + 1) / 12.0;
			double VarU = n1n2prod * (n1 + n2 + 1) / 12.0;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z = (Umin - EU) / mathlib.util.FastMath.sqrt(VarU);
			double z = (Umin - EU) / FastMath.sqrt(VarU);

			// No try-catch or advertised exception because args are valid
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.distribution.NormalDistribution standardNormal = new mathlib.distribution.NormalDistribution(0, 1);
			NormalDistribution standardNormal = new NormalDistribution(0, 1);

			return 2 * standardNormal.cumulativeProbability(z);
		}

		/// <summary>
		/// Returns the asymptotic <i>observed significance level</i>, or <a href=
		/// "http://www.cas.lancs.ac.uk/glossary_v1.1/hyptest.html#pvalue">
		/// p-value</a>, associated with a <a
		/// href="http://en.wikipedia.org/wiki/Mann%E2%80%93Whitney_U"> Mann-Whitney
		/// U statistic</a> comparing mean for two independent samples.
		/// <p>
		/// Let X<sub>i</sub> denote the i'th individual of the first sample and
		/// Y<sub>j</sub> the j'th individual in the second sample. Note that the
		/// samples would often have different length.
		/// </p>
		/// <p>
		/// <strong>Preconditions</strong>:
		/// <ul>
		/// <li>All observations in the two samples are independent.</li>
		/// <li>The observations are at least ordinal (continuous are also ordinal).</li>
		/// </ul>
		/// </p><p>
		/// Ties give rise to biased variance at the moment. See e.g. <a
		/// href="http://mlsc.lboro.ac.uk/resources/statistics/Mannwhitney.pdf"
		/// >http://mlsc.lboro.ac.uk/resources/statistics/Mannwhitney.pdf</a>.</p>
		/// </summary>
		/// <param name="x"> the first sample </param>
		/// <param name="y"> the second sample </param>
		/// <returns> asymptotic p-value </returns>
		/// <exception cref="NullArgumentException"> if {@code x} or {@code y} are {@code null}. </exception>
		/// <exception cref="NoDataException"> if {@code x} or {@code y} are zero-length. </exception>
		/// <exception cref="ConvergenceException"> if the p-value can not be computed due to a
		/// convergence error </exception>
		/// <exception cref="MaxCountExceededException"> if the maximum number of iterations
		/// is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double mannWhitneyUTest(final double[] x, final double[] y) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException, mathlib.exception.ConvergenceException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double mannWhitneyUTest(double[] x, double[] y)
		{

			ensureDataConformance(x, y);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double Umax = mannWhitneyU(x, y);
			double Umax = mannWhitneyU(x, y);

			/*
			 * It can be shown that U1 + U2 = n1 * n2
			 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double Umin = x.length * y.length - Umax;
			double Umin = x.Length * y.Length - Umax;

			return calculateAsymptoticPValue(Umin, x.Length, y.Length);
		}

	}

}
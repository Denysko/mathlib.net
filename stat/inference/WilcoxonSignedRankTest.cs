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
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using NoDataException = mathlib.exception.NoDataException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using NaNStrategy = mathlib.stat.ranking.NaNStrategy;
	using NaturalRanking = mathlib.stat.ranking.NaturalRanking;
	using TiesStrategy = mathlib.stat.ranking.TiesStrategy;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// An implementation of the Wilcoxon signed-rank test.
	/// 
	/// @version $Id: WilcoxonSignedRankTest.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class WilcoxonSignedRankTest
	{

		/// <summary>
		/// Ranking algorithm. </summary>
		private NaturalRanking naturalRanking;

		/// <summary>
		/// Create a test instance where NaN's are left in place and ties get
		/// the average of applicable ranks. Use this unless you are very sure
		/// of what you are doing.
		/// </summary>
		public WilcoxonSignedRankTest()
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
//ORIGINAL LINE: public WilcoxonSignedRankTest(final mathlib.stat.ranking.NaNStrategy nanStrategy, final mathlib.stat.ranking.TiesStrategy tiesStrategy)
		public WilcoxonSignedRankTest(NaNStrategy nanStrategy, TiesStrategy tiesStrategy)
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
		/// <exception cref="DimensionMismatchException"> if {@code x} and {@code y} do not
		/// have the same length. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void ensureDataConformance(final double[] x, final double[] y) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException, mathlib.exception.DimensionMismatchException
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
			if (y.Length != x.Length)
			{
				throw new DimensionMismatchException(y.Length, x.Length);
			}
		}

		/// <summary>
		/// Calculates y[i] - x[i] for all i
		/// </summary>
		/// <param name="x"> first sample </param>
		/// <param name="y"> second sample </param>
		/// <returns> z = y - x </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double[] calculateDifferences(final double[] x, final double[] y)
		private double[] calculateDifferences(double[] x, double[] y)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] z = new double[x.length];
			double[] z = new double[x.Length];

			for (int i = 0; i < x.Length; ++i)
			{
				z[i] = y[i] - x[i];
			}

			return z;
		}

		/// <summary>
		/// Calculates |z[i]| for all i
		/// </summary>
		/// <param name="z"> sample </param>
		/// <returns> |z| </returns>
		/// <exception cref="NullArgumentException"> if {@code z} is {@code null} </exception>
		/// <exception cref="NoDataException"> if {@code z} is zero-length. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private double[] calculateAbsoluteDifferences(final double[] z) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private double[] calculateAbsoluteDifferences(double[] z)
		{

			if (z == null)
			{
				throw new NullArgumentException();
			}

			if (z.Length == 0)
			{
				throw new NoDataException();
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] zAbs = new double[z.length];
			double[] zAbs = new double[z.Length];

			for (int i = 0; i < z.Length; ++i)
			{
				zAbs[i] = FastMath.abs(z[i]);
			}

			return zAbs;
		}

		/// <summary>
		/// Computes the <a
		/// href="http://en.wikipedia.org/wiki/Wilcoxon_signed-rank_test">
		/// Wilcoxon signed ranked statistic</a> comparing mean for two related
		/// samples or repeated measurements on a single sample.
		/// <p>
		/// This statistic can be used to perform a Wilcoxon signed ranked test
		/// evaluating the null hypothesis that the two related samples or repeated
		/// measurements on a single sample has equal mean.
		/// </p>
		/// <p>
		/// Let X<sub>i</sub> denote the i'th individual of the first sample and
		/// Y<sub>i</sub> the related i'th individual in the second sample. Let
		/// Z<sub>i</sub> = Y<sub>i</sub> - X<sub>i</sub>.
		/// </p>
		/// <p>
		/// <strong>Preconditions</strong>:
		/// <ul>
		/// <li>The differences Z<sub>i</sub> must be independent.</li>
		/// <li>Each Z<sub>i</sub> comes from a continuous population (they must be
		/// identical) and is symmetric about a common median.</li>
		/// <li>The values that X<sub>i</sub> and Y<sub>i</sub> represent are
		/// ordered, so the comparisons greater than, less than, and equal to are
		/// meaningful.</li>
		/// </ul>
		/// </p>
		/// </summary>
		/// <param name="x"> the first sample </param>
		/// <param name="y"> the second sample </param>
		/// <returns> wilcoxonSignedRank statistic (the larger of W+ and W-) </returns>
		/// <exception cref="NullArgumentException"> if {@code x} or {@code y} are {@code null}. </exception>
		/// <exception cref="NoDataException"> if {@code x} or {@code y} are zero-length. </exception>
		/// <exception cref="DimensionMismatchException"> if {@code x} and {@code y} do not
		/// have the same length. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double wilcoxonSignedRank(final double[] x, final double[] y) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException, mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double wilcoxonSignedRank(double[] x, double[] y)
		{

			ensureDataConformance(x, y);

			// throws IllegalArgumentException if x and y are not correctly
			// specified
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] z = calculateDifferences(x, y);
			double[] z = calculateDifferences(x, y);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] zAbs = calculateAbsoluteDifferences(z);
			double[] zAbs = calculateAbsoluteDifferences(z);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] ranks = naturalRanking.rank(zAbs);
			double[] ranks = naturalRanking.rank(zAbs);

			double Wplus = 0;

			for (int i = 0; i < z.Length; ++i)
			{
				if (z[i] > 0)
				{
					Wplus += ranks[i];
				}
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = x.length;
			int N = x.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double Wminus = (((double)(N * (N + 1))) / 2.0) - Wplus;
			double Wminus = (((double)(N * (N + 1))) / 2.0) - Wplus;

			return FastMath.max(Wplus, Wminus);
		}

		/// <summary>
		/// Algorithm inspired by
		/// http://www.fon.hum.uva.nl/Service/Statistics/Signed_Rank_Algorihms.html#C
		/// by Rob van Son, Institute of Phonetic Sciences & IFOTT,
		/// University of Amsterdam
		/// </summary>
		/// <param name="Wmax"> largest Wilcoxon signed rank value </param>
		/// <param name="N"> number of subjects (corresponding to x.length) </param>
		/// <returns> two-sided exact p-value </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double calculateExactPValue(final double Wmax, final int N)
		private double calculateExactPValue(double Wmax, int N)
		{

			// Total number of outcomes (equal to 2^N but a lot faster)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = 1 << N;
			int m = 1 << N;

			int largerRankSums = 0;

			for (int i = 0; i < m; ++i)
			{
				int rankSum = 0;

				// Generate all possible rank sums
				for (int j = 0; j < N; ++j)
				{

					// (i >> j) & 1 extract i's j-th bit from the right
					if (((i >> j) & 1) == 1)
					{
						rankSum += j + 1;
					}
				}

				if (rankSum >= Wmax)
				{
					++largerRankSums;
				}
			}

			/*
			 * largerRankSums / m gives the one-sided p-value, so it's multiplied
			 * with 2 to get the two-sided p-value
			 */
			return 2 * ((double) largerRankSums) / ((double) m);
		}

		/// <param name="Wmin"> smallest Wilcoxon signed rank value </param>
		/// <param name="N"> number of subjects (corresponding to x.length) </param>
		/// <returns> two-sided asymptotic p-value </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double calculateAsymptoticPValue(final double Wmin, final int N)
		private double calculateAsymptoticPValue(double Wmin, int N)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ES = (double)(N * (N + 1)) / 4.0;
			double ES = (double)(N * (N + 1)) / 4.0;

			/* Same as (but saves computations):
			 * final double VarW = ((double) (N * (N + 1) * (2*N + 1))) / 24;
			 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double VarS = ES * ((double)(2 * N + 1) / 6.0);
			double VarS = ES * ((double)(2 * N + 1) / 6.0);

			// - 0.5 is a continuity correction
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z = (Wmin - ES - 0.5) / mathlib.util.FastMath.sqrt(VarS);
			double z = (Wmin - ES - 0.5) / FastMath.sqrt(VarS);

			// No try-catch or advertised exception because args are valid
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.distribution.NormalDistribution standardNormal = new mathlib.distribution.NormalDistribution(0, 1);
			NormalDistribution standardNormal = new NormalDistribution(0, 1);

			return 2 * standardNormal.cumulativeProbability(z);
		}

		/// <summary>
		/// Returns the <i>observed significance level</i>, or <a href=
		/// "http://www.cas.lancs.ac.uk/glossary_v1.1/hyptest.html#pvalue">
		/// p-value</a>, associated with a <a
		/// href="http://en.wikipedia.org/wiki/Wilcoxon_signed-rank_test">
		/// Wilcoxon signed ranked statistic</a> comparing mean for two related
		/// samples or repeated measurements on a single sample.
		/// <p>
		/// Let X<sub>i</sub> denote the i'th individual of the first sample and
		/// Y<sub>i</sub> the related i'th individual in the second sample. Let
		/// Z<sub>i</sub> = Y<sub>i</sub> - X<sub>i</sub>.
		/// </p>
		/// <p>
		/// <strong>Preconditions</strong>:
		/// <ul>
		/// <li>The differences Z<sub>i</sub> must be independent.</li>
		/// <li>Each Z<sub>i</sub> comes from a continuous population (they must be
		/// identical) and is symmetric about a common median.</li>
		/// <li>The values that X<sub>i</sub> and Y<sub>i</sub> represent are
		/// ordered, so the comparisons greater than, less than, and equal to are
		/// meaningful.</li>
		/// </ul>
		/// </p>
		/// </summary>
		/// <param name="x"> the first sample </param>
		/// <param name="y"> the second sample </param>
		/// <param name="exactPValue">
		///            if the exact p-value is wanted (only works for x.length <= 30,
		///            if true and x.length > 30, this is ignored because
		///            calculations may take too long) </param>
		/// <returns> p-value </returns>
		/// <exception cref="NullArgumentException"> if {@code x} or {@code y} are {@code null}. </exception>
		/// <exception cref="NoDataException"> if {@code x} or {@code y} are zero-length. </exception>
		/// <exception cref="DimensionMismatchException"> if {@code x} and {@code y} do not
		/// have the same length. </exception>
		/// <exception cref="NumberIsTooLargeException"> if {@code exactPValue} is {@code true}
		/// and {@code x.length} > 30 </exception>
		/// <exception cref="ConvergenceException"> if the p-value can not be computed due to
		/// a convergence error </exception>
		/// <exception cref="MaxCountExceededException"> if the maximum number of iterations
		/// is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double wilcoxonSignedRankTest(final double[] x, final double[] y, final boolean exactPValue) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException, mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooLargeException, mathlib.exception.ConvergenceException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double wilcoxonSignedRankTest(double[] x, double[] y, bool exactPValue)
		{

			ensureDataConformance(x, y);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = x.length;
			int N = x.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double Wmax = wilcoxonSignedRank(x, y);
			double Wmax = wilcoxonSignedRank(x, y);

			if (exactPValue && N > 30)
			{
				throw new NumberIsTooLargeException(N, 30, true);
			}

			if (exactPValue)
			{
				return calculateExactPValue(Wmax, N);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double Wmin = ((double)(N*(N+1)) / 2.0) - Wmax;
				double Wmin = ((double)(N * (N + 1)) / 2.0) - Wmax;
				return calculateAsymptoticPValue(Wmin, N);
			}
		}
	}

}
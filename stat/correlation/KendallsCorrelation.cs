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
namespace mathlib.stat.correlation
{

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using BlockRealMatrix = mathlib.linear.BlockRealMatrix;
	using MatrixUtils = mathlib.linear.MatrixUtils;
	using RealMatrix = mathlib.linear.RealMatrix;
	using FastMath = mathlib.util.FastMath;
	using mathlib.util;


	/// <summary>
	/// Implementation of Kendall's Tau-b rank correlation</a>.
	/// <p>
	/// A pair of observations (x<sub>1</sub>, y<sub>1</sub>) and
	/// (x<sub>2</sub>, y<sub>2</sub>) are considered <i>concordant</i> if
	/// x<sub>1</sub> &lt; x<sub>2</sub> and y<sub>1</sub> &lt; y<sub>2</sub>
	/// or x<sub>2</sub> &lt; x<sub>1</sub> and y<sub>2</sub> &lt; y<sub>1</sub>.
	/// The pair is <i>discordant</i> if x<sub>1</sub> &lt; x<sub>2</sub> and
	/// y<sub>2</sub> &lt; y<sub>1</sub> or x<sub>2</sub> &lt; x<sub>1</sub> and
	/// y<sub>1</sub> &lt; y<sub>2</sub>.  If either x<sub>1</sub> = x<sub>2</sub>
	/// or y<sub>1</sub> = y<sub>2</sub>, the pair is neither concordant nor
	/// discordant.
	/// <p>
	/// Kendall's Tau-b is defined as:
	/// <pre>
	/// tau<sub>b</sub> = (n<sub>c</sub> - n<sub>d</sub>) / sqrt((n<sub>0</sub> - n<sub>1</sub>) * (n<sub>0</sub> - n<sub>2</sub>))
	/// </pre>
	/// <p>
	/// where:
	/// <ul>
	///     <li>n<sub>0</sub> = n * (n - 1) / 2</li>
	///     <li>n<sub>c</sub> = Number of concordant pairs</li>
	///     <li>n<sub>d</sub> = Number of discordant pairs</li>
	///     <li>n<sub>1</sub> = sum of t<sub>i</sub> * (t<sub>i</sub> - 1) / 2 for all i</li>
	///     <li>n<sub>2</sub> = sum of u<sub>j</sub> * (u<sub>j</sub> - 1) / 2 for all j</li>
	///     <li>t<sub>i</sub> = Number of tied values in the i<sup>th</sup> group of ties in x</li>
	///     <li>u<sub>j</sub> = Number of tied values in the j<sup>th</sup> group of ties in y</li>
	/// </ul>
	/// <p>
	/// This implementation uses the O(n log n) algorithm described in
	/// William R. Knight's 1966 paper "A Computer Method for Calculating
	/// Kendall's Tau with Ungrouped Data" in the Journal of the American
	/// Statistical Association.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Kendall_tau_rank_correlation_coefficient">
	/// Kendall tau rank correlation coefficient (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://www.jstor.org/stable/2282833">A Computer
	/// Method for Calculating Kendall's Tau with Ungrouped Data</a>
	/// 
	/// @version $Id: KendallsCorrelation.java 1548907 2013-12-07 13:08:06Z tn $
	/// @since 3.3 </seealso>
	public class KendallsCorrelation
	{

		/// <summary>
		/// correlation matrix </summary>
		private readonly RealMatrix correlationMatrix;

		/// <summary>
		/// Create a KendallsCorrelation instance without data.
		/// </summary>
		public KendallsCorrelation()
		{
			correlationMatrix = null;
		}

		/// <summary>
		/// Create a KendallsCorrelation from a rectangular array
		/// whose columns represent values of variables to be correlated.
		/// </summary>
		/// <param name="data"> rectangular array with columns representing variables </param>
		/// <exception cref="IllegalArgumentException"> if the input data array is not
		/// rectangular with at least two rows and two columns. </exception>
		public KendallsCorrelation(double[][] data) : this(MatrixUtils.createRealMatrix(data))
		{
		}

		/// <summary>
		/// Create a KendallsCorrelation from a RealMatrix whose columns
		/// represent variables to be correlated.
		/// </summary>
		/// <param name="matrix"> matrix with columns representing variables to correlate </param>
		public KendallsCorrelation(RealMatrix matrix)
		{
			correlationMatrix = computeCorrelationMatrix(matrix);
		}

		/// <summary>
		/// Returns the correlation matrix.
		/// </summary>
		/// <returns> correlation matrix </returns>
		public virtual RealMatrix CorrelationMatrix
		{
			get
			{
				return correlationMatrix;
			}
		}

		/// <summary>
		/// Computes the Kendall's Tau rank correlation matrix for the columns of
		/// the input matrix.
		/// </summary>
		/// <param name="matrix"> matrix with columns representing variables to correlate </param>
		/// <returns> correlation matrix </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.linear.RealMatrix computeCorrelationMatrix(final mathlib.linear.RealMatrix matrix)
		public virtual RealMatrix computeCorrelationMatrix(RealMatrix matrix)
		{
			int nVars = matrix.ColumnDimension;
			RealMatrix outMatrix = new BlockRealMatrix(nVars, nVars);
			for (int i = 0; i < nVars; i++)
			{
				for (int j = 0; j < i; j++)
				{
					double corr = correlation(matrix.getColumn(i), matrix.getColumn(j));
					outMatrix.setEntry(i, j, corr);
					outMatrix.setEntry(j, i, corr);
				}
				outMatrix.setEntry(i, i, 1d);
			}
			return outMatrix;
		}

		/// <summary>
		/// Computes the Kendall's Tau rank correlation matrix for the columns of
		/// the input rectangular array.  The columns of the array represent values
		/// of variables to be correlated.
		/// </summary>
		/// <param name="matrix"> matrix with columns representing variables to correlate </param>
		/// <returns> correlation matrix </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.linear.RealMatrix computeCorrelationMatrix(final double[][] matrix)
		public virtual RealMatrix computeCorrelationMatrix(double[][] matrix)
		{
		   return computeCorrelationMatrix(new BlockRealMatrix(matrix));
		}

		/// <summary>
		/// Computes the Kendall's Tau rank correlation coefficient between the two arrays.
		/// </summary>
		/// <param name="xArray"> first data array </param>
		/// <param name="yArray"> second data array </param>
		/// <returns> Returns Kendall's Tau rank correlation coefficient for the two arrays </returns>
		/// <exception cref="DimensionMismatchException"> if the arrays lengths do not match </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double correlation(final double[] xArray, final double[] yArray) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double correlation(double[] xArray, double[] yArray)
		{

			if (xArray.Length != yArray.Length)
			{
				throw new DimensionMismatchException(xArray.Length, yArray.Length);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = xArray.length;
			int n = xArray.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long numPairs = sum(n - 1);
			long numPairs = sum(n - 1);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") mathlib.util.Pair<Double, Double>[] pairs = new mathlib.util.Pair[n];
			Pair<double?, double?>[] pairs = new Pair[n];
			for (int i = 0; i < n; i++)
			{
				pairs[i] = new Pair<double?, double?>(xArray[i], yArray[i]);
			}

			Arrays.sort(pairs, new ComparatorAnonymousInnerClassHelper(this));

			long tiedXPairs = 0;
			long tiedXYPairs = 0;
			long consecutiveXTies = 1;
			long consecutiveXYTies = 1;
			Pair<double?, double?> prev = pairs[0];
			for (int i = 1; i < n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.util.Pair<Double, Double> curr = pairs[i];
				Pair<double?, double?> curr = pairs[i];
				if (curr.First.Equals(prev.First))
				{
					consecutiveXTies++;
					if (curr.Second.Equals(prev.Second))
					{
						consecutiveXYTies++;
					}
					else
					{
						tiedXYPairs += sum(consecutiveXYTies - 1);
						consecutiveXYTies = 1;
					}
				}
				else
				{
					tiedXPairs += sum(consecutiveXTies - 1);
					consecutiveXTies = 1;
					tiedXYPairs += sum(consecutiveXYTies - 1);
					consecutiveXYTies = 1;
				}
				prev = curr;
			}
			tiedXPairs += sum(consecutiveXTies - 1);
			tiedXYPairs += sum(consecutiveXYTies - 1);

			int swaps = 0;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") mathlib.util.Pair<Double, Double>[] pairsDestination = new mathlib.util.Pair[n];
			Pair<double?, double?>[] pairsDestination = new Pair[n];
			for (int segmentSize = 1; segmentSize < n; segmentSize <<= 1)
			{
				for (int offset = 0; offset < n; offset += 2 * segmentSize)
				{
					int i = offset;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iEnd = mathlib.util.FastMath.min(i + segmentSize, n);
					int iEnd = FastMath.min(i + segmentSize, n);
					int j = iEnd;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jEnd = mathlib.util.FastMath.min(j + segmentSize, n);
					int jEnd = FastMath.min(j + segmentSize, n);

					int copyLocation = offset;
					while (i < iEnd || j < jEnd)
					{
						if (i < iEnd)
						{
							if (j < jEnd)
							{
								if (pairs[i].Second.compareTo(pairs[j].Second) <= 0)
								{
									pairsDestination[copyLocation] = pairs[i];
									i++;
								}
								else
								{
									pairsDestination[copyLocation] = pairs[j];
									j++;
									swaps += iEnd - i;
								}
							}
							else
							{
								pairsDestination[copyLocation] = pairs[i];
								i++;
							}
						}
						else
						{
							pairsDestination[copyLocation] = pairs[j];
							j++;
						}
						copyLocation++;
					}
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.util.Pair<Double, Double>[] pairsTemp = pairs;
				Pair<double?, double?>[] pairsTemp = pairs;
				pairs = pairsDestination;
				pairsDestination = pairsTemp;
			}

			long tiedYPairs = 0;
			long consecutiveYTies = 1;
			prev = pairs[0];
			for (int i = 1; i < n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.util.Pair<Double, Double> curr = pairs[i];
				Pair<double?, double?> curr = pairs[i];
				if (curr.Second.Equals(prev.Second))
				{
					consecutiveYTies++;
				}
				else
				{
					tiedYPairs += sum(consecutiveYTies - 1);
					consecutiveYTies = 1;
				}
				prev = curr;
			}
			tiedYPairs += sum(consecutiveYTies - 1);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long concordantMinusDiscordant = numPairs - tiedXPairs - tiedYPairs + tiedXYPairs - 2 * swaps;
			long concordantMinusDiscordant = numPairs - tiedXPairs - tiedYPairs + tiedXYPairs - 2 * swaps;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double nonTiedPairsMultiplied = (numPairs - tiedXPairs) * (double)(numPairs - tiedYPairs);
			double nonTiedPairsMultiplied = (numPairs - tiedXPairs) * (double)(numPairs - tiedYPairs);
			return concordantMinusDiscordant / FastMath.sqrt(nonTiedPairsMultiplied);
		}

		private class ComparatorAnonymousInnerClassHelper : IComparer<Pair<double?, double?>>
		{
			private readonly KendallsCorrelation outerInstance;

			public ComparatorAnonymousInnerClassHelper(KendallsCorrelation outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual int Compare(Pair<double?, double?> pair1, Pair<double?, double?> pair2)
			{
				int compareFirst = pair1.First.compareTo(pair2.First);
				return compareFirst != 0 ? compareFirst : pair1.Second.compareTo(pair2.Second);
			}
		}

		/// <summary>
		/// Returns the sum of the number from 1 .. n according to Gauss' summation formula:
		/// \[ \sum\limits_{k=1}^n k = \frac{n(n + 1)}{2} \]
		/// </summary>
		/// <param name="n"> the summation end </param>
		/// <returns> the sum of the number from 1 to n </returns>
		private static long sum(long n)
		{
			return n * (n + 1) / 2l;
		}
	}

}
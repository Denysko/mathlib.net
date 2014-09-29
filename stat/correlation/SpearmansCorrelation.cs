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

namespace mathlib.stat.correlation
{


	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using BlockRealMatrix = mathlib.linear.BlockRealMatrix;
	using RealMatrix = mathlib.linear.RealMatrix;
	using NaNStrategy = mathlib.stat.ranking.NaNStrategy;
	using NaturalRanking = mathlib.stat.ranking.NaturalRanking;
	using RankingAlgorithm = mathlib.stat.ranking.RankingAlgorithm;

	/// <summary>
	/// Spearman's rank correlation. This implementation performs a rank
	/// transformation on the input data and then computes <seealso cref="PearsonsCorrelation"/>
	/// on the ranked data.
	/// <p>
	/// By default, ranks are computed using <seealso cref="NaturalRanking"/> with default
	/// strategies for handling NaNs and ties in the data (NaNs maximal, ties averaged).
	/// The ranking algorithm can be set using a constructor argument.
	/// 
	/// @since 2.0
	/// @version $Id: SpearmansCorrelation.java 1461822 2013-03-27 19:44:22Z tn $
	/// </summary>
	public class SpearmansCorrelation
	{

		/// <summary>
		/// Input data </summary>
		private readonly RealMatrix data;

		/// <summary>
		/// Ranking algorithm </summary>
		private readonly RankingAlgorithm rankingAlgorithm;

		/// <summary>
		/// Rank correlation </summary>
		private readonly PearsonsCorrelation rankCorrelation;

		/// <summary>
		/// Create a SpearmansCorrelation without data.
		/// </summary>
		public SpearmansCorrelation() : this(new NaturalRanking())
		{
		}

		/// <summary>
		/// Create a SpearmansCorrelation with the given ranking algorithm.
		/// <p>
		/// From version 4.0 onwards this constructor will throw an exception
		/// if the provided <seealso cref="NaturalRanking"/> uses a <seealso cref="NaNStrategy#REMOVED"/> strategy.
		/// </summary>
		/// <param name="rankingAlgorithm"> ranking algorithm
		/// @since 3.1 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SpearmansCorrelation(final mathlib.stat.ranking.RankingAlgorithm rankingAlgorithm)
		public SpearmansCorrelation(RankingAlgorithm rankingAlgorithm)
		{
			data = null;
			this.rankingAlgorithm = rankingAlgorithm;
			rankCorrelation = null;
		}

		/// <summary>
		/// Create a SpearmansCorrelation from the given data matrix.
		/// </summary>
		/// <param name="dataMatrix"> matrix of data with columns representing
		/// variables to correlate </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SpearmansCorrelation(final mathlib.linear.RealMatrix dataMatrix)
		public SpearmansCorrelation(RealMatrix dataMatrix) : this(dataMatrix, new NaturalRanking())
		{
		}

		/// <summary>
		/// Create a SpearmansCorrelation with the given input data matrix
		/// and ranking algorithm.
		/// <p>
		/// From version 4.0 onwards this constructor will throw an exception
		/// if the provided <seealso cref="NaturalRanking"/> uses a <seealso cref="NaNStrategy#REMOVED"/> strategy.
		/// </summary>
		/// <param name="dataMatrix"> matrix of data with columns representing
		/// variables to correlate </param>
		/// <param name="rankingAlgorithm"> ranking algorithm </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SpearmansCorrelation(final mathlib.linear.RealMatrix dataMatrix, final mathlib.stat.ranking.RankingAlgorithm rankingAlgorithm)
		public SpearmansCorrelation(RealMatrix dataMatrix, RankingAlgorithm rankingAlgorithm)
		{
			this.rankingAlgorithm = rankingAlgorithm;
			this.data = rankTransform(dataMatrix);
			rankCorrelation = new PearsonsCorrelation(data);
		}

		/// <summary>
		/// Calculate the Spearman Rank Correlation Matrix.
		/// </summary>
		/// <returns> Spearman Rank Correlation Matrix </returns>
		public virtual RealMatrix CorrelationMatrix
		{
			get
			{
				return rankCorrelation.CorrelationMatrix;
			}
		}

		/// <summary>
		/// Returns a <seealso cref="PearsonsCorrelation"/> instance constructed from the
		/// ranked input data. That is,
		/// <code>new SpearmansCorrelation(matrix).getRankCorrelation()</code>
		/// is equivalent to
		/// <code>new PearsonsCorrelation(rankTransform(matrix))</code> where
		/// <code>rankTransform(matrix)</code> is the result of applying the
		/// configured <code>RankingAlgorithm</code> to each of the columns of
		/// <code>matrix.</code>
		/// </summary>
		/// <returns> PearsonsCorrelation among ranked column data </returns>
		public virtual PearsonsCorrelation RankCorrelation
		{
			get
			{
				return rankCorrelation;
			}
		}

		/// <summary>
		/// Computes the Spearman's rank correlation matrix for the columns of the
		/// input matrix.
		/// </summary>
		/// <param name="matrix"> matrix with columns representing variables to correlate </param>
		/// <returns> correlation matrix </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.linear.RealMatrix computeCorrelationMatrix(final mathlib.linear.RealMatrix matrix)
		public virtual RealMatrix computeCorrelationMatrix(RealMatrix matrix)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix matrixCopy = rankTransform(matrix);
			RealMatrix matrixCopy = rankTransform(matrix);
			return (new PearsonsCorrelation()).computeCorrelationMatrix(matrixCopy);
		}

		/// <summary>
		/// Computes the Spearman's rank correlation matrix for the columns of the
		/// input rectangular array.  The columns of the array represent values
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
		/// Computes the Spearman's rank correlation coefficient between the two arrays.
		/// </summary>
		/// <param name="xArray"> first data array </param>
		/// <param name="yArray"> second data array </param>
		/// <returns> Returns Spearman's rank correlation coefficient for the two arrays </returns>
		/// <exception cref="DimensionMismatchException"> if the arrays lengths do not match </exception>
		/// <exception cref="MathIllegalArgumentException"> if the array length is less than 2 </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double correlation(final double[] xArray, final double[] yArray)
		public virtual double correlation(double[] xArray, double[] yArray)
		{
			if (xArray.Length != yArray.Length)
			{
				throw new DimensionMismatchException(xArray.Length, yArray.Length);
			}
			else if (xArray.Length < 2)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.INSUFFICIENT_DIMENSION, xArray.Length, 2);
			}
			else
			{
				double[] x = xArray;
				double[] y = yArray;
				if (rankingAlgorithm is NaturalRanking && NaNStrategy.REMOVED == ((NaturalRanking) rankingAlgorithm).NanStrategy)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Set<Integer> nanPositions = new java.util.HashSet<Integer>();
					Set<int?> nanPositions = new HashSet<int?>();

					nanPositions.addAll(getNaNPositions(xArray));
					nanPositions.addAll(getNaNPositions(yArray));

					x = removeValues(xArray, nanPositions);
					y = removeValues(yArray, nanPositions);
				}
				return (new PearsonsCorrelation()).correlation(rankingAlgorithm.rank(x), rankingAlgorithm.rank(y));
			}
		}

		/// <summary>
		/// Applies rank transform to each of the columns of <code>matrix</code>
		/// using the current <code>rankingAlgorithm</code>.
		/// </summary>
		/// <param name="matrix"> matrix to transform </param>
		/// <returns> a rank-transformed matrix </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private mathlib.linear.RealMatrix rankTransform(final mathlib.linear.RealMatrix matrix)
		private RealMatrix rankTransform(RealMatrix matrix)
		{
			RealMatrix transformed = null;

			if (rankingAlgorithm is NaturalRanking && ((NaturalRanking) rankingAlgorithm).NanStrategy == NaNStrategy.REMOVED)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Set<Integer> nanPositions = new java.util.HashSet<Integer>();
				Set<int?> nanPositions = new HashSet<int?>();
				for (int i = 0; i < matrix.ColumnDimension; i++)
				{
					nanPositions.addAll(getNaNPositions(matrix.getColumn(i)));
				}

				// if we have found NaN values, we have to update the matrix size
				if (!nanPositions.Empty)
				{
					transformed = new BlockRealMatrix(matrix.RowDimension - nanPositions.size(), matrix.ColumnDimension);
					for (int i = 0; i < transformed.ColumnDimension; i++)
					{
						transformed.setColumn(i, removeValues(matrix.getColumn(i), nanPositions));
					}
				}
			}

			if (transformed == null)
			{
				transformed = matrix.copy();
			}

			for (int i = 0; i < transformed.ColumnDimension; i++)
			{
				transformed.setColumn(i, rankingAlgorithm.rank(transformed.getColumn(i)));
			}

			return transformed;
		}

		/// <summary>
		/// Returns a list containing the indices of NaN values in the input array.
		/// </summary>
		/// <param name="input"> the input array </param>
		/// <returns> a list of NaN positions in the input array </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private java.util.List<Integer> getNaNPositions(final double[] input)
		private IList<int?> getNaNPositions(double[] input)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Integer> positions = new java.util.ArrayList<Integer>();
			IList<int?> positions = new List<int?>();
			for (int i = 0; i < input.Length; i++)
			{
				if (double.IsNaN(input[i]))
				{
					positions.Add(i);
				}
			}
			return positions;
		}

		/// <summary>
		/// Removes all values from the input array at the specified indices.
		/// </summary>
		/// <param name="input"> the input array </param>
		/// <param name="indices"> a set containing the indices to be removed </param>
		/// <returns> the input array without the values at the specified indices </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double[] removeValues(final double[] input, final java.util.Set<Integer> indices)
		private double[] removeValues(double[] input, Set<int?> indices)
		{
			if (indices.Empty)
			{
				return input;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] result = new double[input.length - indices.size()];
			double[] result = new double[input.Length - indices.size()];
			for (int i = 0, j = 0; i < input.Length; i++)
			{
				if (!indices.contains(i))
				{
					result[j++] = input[i];
				}
			}
			return result;
		}
	}

}
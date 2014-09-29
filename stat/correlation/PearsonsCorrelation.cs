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

	using TDistribution = mathlib.distribution.TDistribution;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using RealMatrix = mathlib.linear.RealMatrix;
	using BlockRealMatrix = mathlib.linear.BlockRealMatrix;
	using SimpleRegression = mathlib.stat.regression.SimpleRegression;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Computes Pearson's product-moment correlation coefficients for pairs of arrays
	/// or columns of a matrix.
	/// 
	/// <p>The constructors that take <code>RealMatrix</code> or
	/// <code>double[][]</code> arguments generate correlation matrices.  The
	/// columns of the input matrices are assumed to represent variable values.
	/// Correlations are given by the formula</p>
	/// 
	/// <p><code>cor(X, Y) = &Sigma;[(x<sub>i</sub> - E(X))(y<sub>i</sub> - E(Y))] / [(n - 1)s(X)s(Y)]</code>
	/// where <code>E(X)</code> is the mean of <code>X</code>, <code>E(Y)</code>
	/// is the mean of the <code>Y</code> values and s(X), s(Y) are standard deviations.</p>
	/// 
	/// <p>To compute the correlation coefficient for a single pair of arrays, use <seealso cref="#PearsonsCorrelation()"/>
	/// to construct an instance with no data and then <seealso cref="#correlation(double[], double[])"/>.
	/// Correlation matrices can also be computed directly from an instance with no data using
	/// <seealso cref="#computeCorrelationMatrix(double[][])"/>. In order to use <seealso cref="#getCorrelationMatrix()"/>,
	/// <seealso cref="#getCorrelationPValues()"/>,  or <seealso cref="#getCorrelationStandardErrors()"/>; however, one of the
	/// constructors supplying data or a covariance matrix must be used to create the instance.</p>
	/// 
	/// @version $Id: PearsonsCorrelation.java 1540395 2013-11-09 21:32:06Z psteitz $
	/// @since 2.0
	/// </summary>
	public class PearsonsCorrelation
	{

		/// <summary>
		/// correlation matrix </summary>
		private readonly RealMatrix correlationMatrix;

		/// <summary>
		/// number of observations </summary>
		private readonly int nObs;

		/// <summary>
		/// Create a PearsonsCorrelation instance without data.
		/// </summary>
		public PearsonsCorrelation() : base()
		{
			correlationMatrix = null;
			nObs = 0;
		}

		/// <summary>
		/// Create a PearsonsCorrelation from a rectangular array
		/// whose columns represent values of variables to be correlated.
		/// 
		/// Throws MathIllegalArgumentException if the input array does not have at least
		/// two columns and two rows.  Pairwise correlations are set to NaN if one
		/// of the correlates has zero variance.
		/// </summary>
		/// <param name="data"> rectangular array with columns representing variables </param>
		/// <exception cref="MathIllegalArgumentException"> if the input data array is not
		/// rectangular with at least two rows and two columns. </exception>
		/// <seealso cref= #correlation(double[], double[]) </seealso>
		public PearsonsCorrelation(double[][] data) : this(new BlockRealMatrix(data))
		{
		}

		/// <summary>
		/// Create a PearsonsCorrelation from a RealMatrix whose columns
		/// represent variables to be correlated.
		/// 
		/// Throws MathIllegalArgumentException if the matrix does not have at least
		/// two columns and two rows.  Pairwise correlations are set to NaN if one
		/// of the correlates has zero variance.
		/// </summary>
		/// <param name="matrix"> matrix with columns representing variables to correlate </param>
		/// <exception cref="MathIllegalArgumentException"> if the matrix does not contain sufficient data </exception>
		/// <seealso cref= #correlation(double[], double[]) </seealso>
		public PearsonsCorrelation(RealMatrix matrix)
		{
			nObs = matrix.RowDimension;
			correlationMatrix = computeCorrelationMatrix(matrix);
		}

		/// <summary>
		/// Create a PearsonsCorrelation from a <seealso cref="Covariance"/>.  The correlation
		/// matrix is computed by scaling the Covariance's covariance matrix.
		/// The Covariance instance must have been created from a data matrix with
		/// columns representing variable values.
		/// </summary>
		/// <param name="covariance"> Covariance instance </param>
		public PearsonsCorrelation(Covariance covariance)
		{
			RealMatrix covarianceMatrix = covariance.CovarianceMatrix;
			if (covarianceMatrix == null)
			{
				throw new NullArgumentException(LocalizedFormats.COVARIANCE_MATRIX);
			}
			nObs = covariance.N;
			correlationMatrix = covarianceToCorrelation(covarianceMatrix);
		}

		/// <summary>
		/// Create a PearsonsCorrelation from a covariance matrix. The correlation
		/// matrix is computed by scaling the covariance matrix.
		/// </summary>
		/// <param name="covarianceMatrix"> covariance matrix </param>
		/// <param name="numberOfObservations"> the number of observations in the dataset used to compute
		/// the covariance matrix </param>
		public PearsonsCorrelation(RealMatrix covarianceMatrix, int numberOfObservations)
		{
			nObs = numberOfObservations;
			correlationMatrix = covarianceToCorrelation(covarianceMatrix);
		}

		/// <summary>
		/// Returns the correlation matrix.
		/// 
		/// <p>This method will return null if the argumentless constructor was used
		/// to create this instance, even if <seealso cref="#computeCorrelationMatrix(double[][])"/>
		/// has been called before it is activated.</p>
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
		/// Returns a matrix of standard errors associated with the estimates
		/// in the correlation matrix.<br/>
		/// <code>getCorrelationStandardErrors().getEntry(i,j)</code> is the standard
		/// error associated with <code>getCorrelationMatrix.getEntry(i,j)</code>
		/// 
		/// <p>The formula used to compute the standard error is <br/>
		/// <code>SE<sub>r</sub> = ((1 - r<sup>2</sup>) / (n - 2))<sup>1/2</sup></code>
		/// where <code>r</code> is the estimated correlation coefficient and
		/// <code>n</code> is the number of observations in the source dataset.</p>
		/// 
		/// <p>To use this method, one of the constructors that supply an input
		/// matrix must have been used to create this instance.</p>
		/// </summary>
		/// <returns> matrix of correlation standard errors </returns>
		/// <exception cref="NullPointerException"> if this instance was created with no data </exception>
		public virtual RealMatrix CorrelationStandardErrors
		{
			get
			{
				int nVars = correlationMatrix.ColumnDimension;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] out = new double[nVars][nVars];
				double[][] @out = RectangularArrays.ReturnRectangularDoubleArray(nVars, nVars);
				for (int i = 0; i < nVars; i++)
				{
					for (int j = 0; j < nVars; j++)
					{
						double r = correlationMatrix.getEntry(i, j);
						@out[i][j] = FastMath.sqrt((1 - r * r) / (nObs - 2));
					}
				}
				return new BlockRealMatrix(@out);
			}
		}

		/// <summary>
		/// Returns a matrix of p-values associated with the (two-sided) null
		/// hypothesis that the corresponding correlation coefficient is zero.
		/// 
		/// <p><code>getCorrelationPValues().getEntry(i,j)</code> is the probability
		/// that a random variable distributed as <code>t<sub>n-2</sub></code> takes
		/// a value with absolute value greater than or equal to <br>
		/// <code>|r|((n - 2) / (1 - r<sup>2</sup>))<sup>1/2</sup></code></p>
		/// 
		/// <p>The values in the matrix are sometimes referred to as the
		/// <i>significance</i> of the corresponding correlation coefficients.</p>
		/// 
		/// <p>To use this method, one of the constructors that supply an input
		/// matrix must have been used to create this instance.</p>
		/// </summary>
		/// <returns> matrix of p-values </returns>
		/// <exception cref="mathlib.exception.MaxCountExceededException">
		/// if an error occurs estimating probabilities </exception>
		/// <exception cref="NullPointerException"> if this instance was created with no data </exception>
		public virtual RealMatrix CorrelationPValues
		{
			get
			{
				TDistribution tDistribution = new TDistribution(nObs - 2);
				int nVars = correlationMatrix.ColumnDimension;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] out = new double[nVars][nVars];
				double[][] @out = RectangularArrays.ReturnRectangularDoubleArray(nVars, nVars);
				for (int i = 0; i < nVars; i++)
				{
					for (int j = 0; j < nVars; j++)
					{
						if (i == j)
						{
							@out[i][j] = 0d;
						}
						else
						{
							double r = correlationMatrix.getEntry(i, j);
							double t = FastMath.abs(r * FastMath.sqrt((nObs - 2) / (1 - r * r)));
							@out[i][j] = 2 * tDistribution.cumulativeProbability(-t);
						}
					}
				}
				return new BlockRealMatrix(@out);
			}
		}


		/// <summary>
		/// Computes the correlation matrix for the columns of the
		/// input matrix, using <seealso cref="#correlation(double[], double[])"/>.
		/// 
		/// Throws MathIllegalArgumentException if the matrix does not have at least
		/// two columns and two rows.  Pairwise correlations are set to NaN if one
		/// of the correlates has zero variance.
		/// </summary>
		/// <param name="matrix"> matrix with columns representing variables to correlate </param>
		/// <returns> correlation matrix </returns>
		/// <exception cref="MathIllegalArgumentException"> if the matrix does not contain sufficient data </exception>
		/// <seealso cref= #correlation(double[], double[]) </seealso>
		public virtual RealMatrix computeCorrelationMatrix(RealMatrix matrix)
		{
			checkSufficientData(matrix);
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
		/// Computes the correlation matrix for the columns of the
		/// input rectangular array.  The columns of the array represent values
		/// of variables to be correlated.
		/// 
		/// Throws MathIllegalArgumentException if the matrix does not have at least
		/// two columns and two rows or if the array is not rectangular. Pairwise
		/// correlations are set to NaN if one of the correlates has zero variance.
		/// </summary>
		/// <param name="data"> matrix with columns representing variables to correlate </param>
		/// <returns> correlation matrix </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array does not contain sufficient data </exception>
		/// <seealso cref= #correlation(double[], double[]) </seealso>
		public virtual RealMatrix computeCorrelationMatrix(double[][] data)
		{
		   return computeCorrelationMatrix(new BlockRealMatrix(data));
		}

		/// <summary>
		/// Computes the Pearson's product-moment correlation coefficient between two arrays.
		/// 
		/// <p>Throws MathIllegalArgumentException if the arrays do not have the same length
		/// or their common length is less than 2.  Returns {@code NaN} if either of the arrays
		/// has zero variance (i.e., if one of the arrays does not contain at least two distinct
		/// values).</p>
		/// </summary>
		/// <param name="xArray"> first data array </param>
		/// <param name="yArray"> second data array </param>
		/// <returns> Returns Pearson's correlation coefficient for the two arrays </returns>
		/// <exception cref="DimensionMismatchException"> if the arrays lengths do not match </exception>
		/// <exception cref="MathIllegalArgumentException"> if there is insufficient data </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double correlation(final double[] xArray, final double[] yArray)
		public virtual double correlation(double[] xArray, double[] yArray)
		{
			SimpleRegression regression = new SimpleRegression();
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
				for (int i = 0; i < xArray.Length; i++)
				{
					regression.addData(xArray[i], yArray[i]);
				}
				return regression.R;
			}
		}

		/// <summary>
		/// Derives a correlation matrix from a covariance matrix.
		/// 
		/// <p>Uses the formula <br/>
		/// <code>r(X,Y) = cov(X,Y)/s(X)s(Y)</code> where
		/// <code>r(&middot,&middot;)</code> is the correlation coefficient and
		/// <code>s(&middot;)</code> means standard deviation.</p>
		/// </summary>
		/// <param name="covarianceMatrix"> the covariance matrix </param>
		/// <returns> correlation matrix </returns>
		public virtual RealMatrix covarianceToCorrelation(RealMatrix covarianceMatrix)
		{
			int nVars = covarianceMatrix.ColumnDimension;
			RealMatrix outMatrix = new BlockRealMatrix(nVars, nVars);
			for (int i = 0; i < nVars; i++)
			{
				double sigma = FastMath.sqrt(covarianceMatrix.getEntry(i, i));
				outMatrix.setEntry(i, i, 1d);
				for (int j = 0; j < i; j++)
				{
					double entry = covarianceMatrix.getEntry(i, j) / (sigma * FastMath.sqrt(covarianceMatrix.getEntry(j, j)));
					outMatrix.setEntry(i, j, entry);
					outMatrix.setEntry(j, i, entry);
				}
			}
			return outMatrix;
		}

		/// <summary>
		/// Throws MathIllegalArgumentException if the matrix does not have at least
		/// two columns and two rows.
		/// </summary>
		/// <param name="matrix"> matrix to check for sufficiency </param>
		/// <exception cref="MathIllegalArgumentException"> if there is insufficient data </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void checkSufficientData(final mathlib.linear.RealMatrix matrix)
		private void checkSufficientData(RealMatrix matrix)
		{
			int nRows = matrix.RowDimension;
			int nCols = matrix.ColumnDimension;
			if (nRows < 2 || nCols < 2)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.INSUFFICIENT_ROWS_AND_COLUMNS, nRows, nCols);
			}
		}
	}

}
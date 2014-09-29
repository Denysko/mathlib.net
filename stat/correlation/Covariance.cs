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

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using RealMatrix = mathlib.linear.RealMatrix;
	using BlockRealMatrix = mathlib.linear.BlockRealMatrix;
	using Mean = mathlib.stat.descriptive.moment.Mean;
	using Variance = mathlib.stat.descriptive.moment.Variance;

	/// <summary>
	/// Computes covariances for pairs of arrays or columns of a matrix.
	/// 
	/// <p>The constructors that take <code>RealMatrix</code> or
	/// <code>double[][]</code> arguments generate covariance matrices.  The
	/// columns of the input matrices are assumed to represent variable values.</p>
	/// 
	/// <p>The constructor argument <code>biasCorrected</code> determines whether or
	/// not computed covariances are bias-corrected.</p>
	/// 
	/// <p>Unbiased covariances are given by the formula</p>
	/// <code>cov(X, Y) = &Sigma;[(x<sub>i</sub> - E(X))(y<sub>i</sub> - E(Y))] / (n - 1)</code>
	/// where <code>E(X)</code> is the mean of <code>X</code> and <code>E(Y)</code>
	/// is the mean of the <code>Y</code> values.
	/// 
	/// <p>Non-bias-corrected estimates use <code>n</code> in place of <code>n - 1</code>
	/// 
	/// @version $Id: Covariance.java 1453271 2013-03-06 10:29:51Z luc $
	/// @since 2.0
	/// </summary>
	public class Covariance
	{

		/// <summary>
		/// covariance matrix </summary>
		private readonly RealMatrix covarianceMatrix;

		/// <summary>
		/// Create an empty covariance matrix.
		/// </summary>
		/// <summary>
		/// Number of observations (length of covariate vectors) </summary>
		private readonly int n;

		/// <summary>
		/// Create a Covariance with no data
		/// </summary>
		public Covariance() : base()
		{
			covarianceMatrix = null;
			n = 0;
		}

		/// <summary>
		/// Create a Covariance matrix from a rectangular array
		/// whose columns represent covariates.
		/// 
		/// <p>The <code>biasCorrected</code> parameter determines whether or not
		/// covariance estimates are bias-corrected.</p>
		/// 
		/// <p>The input array must be rectangular with at least one column
		/// and two rows.</p>
		/// </summary>
		/// <param name="data"> rectangular array with columns representing covariates </param>
		/// <param name="biasCorrected"> true means covariances are bias-corrected </param>
		/// <exception cref="MathIllegalArgumentException"> if the input data array is not
		/// rectangular with at least two rows and one column. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if the input data array is not
		/// rectangular with at least one row and one column. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Covariance(double[][] data, boolean biasCorrected) throws mathlib.exception.MathIllegalArgumentException, mathlib.exception.NotStrictlyPositiveException
		public Covariance(double[][] data, bool biasCorrected) : this(new BlockRealMatrix(data), biasCorrected)
		{
		}

		/// <summary>
		/// Create a Covariance matrix from a rectangular array
		/// whose columns represent covariates.
		/// 
		/// <p>The input array must be rectangular with at least one column
		/// and two rows</p>
		/// </summary>
		/// <param name="data"> rectangular array with columns representing covariates </param>
		/// <exception cref="MathIllegalArgumentException"> if the input data array is not
		/// rectangular with at least two rows and one column. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if the input data array is not
		/// rectangular with at least one row and one column. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Covariance(double[][] data) throws mathlib.exception.MathIllegalArgumentException, mathlib.exception.NotStrictlyPositiveException
		public Covariance(double[][] data) : this(data, true)
		{
		}

		/// <summary>
		/// Create a covariance matrix from a matrix whose columns
		/// represent covariates.
		/// 
		/// <p>The <code>biasCorrected</code> parameter determines whether or not
		/// covariance estimates are bias-corrected.</p>
		/// 
		/// <p>The matrix must have at least one column and two rows</p>
		/// </summary>
		/// <param name="matrix"> matrix with columns representing covariates </param>
		/// <param name="biasCorrected"> true means covariances are bias-corrected </param>
		/// <exception cref="MathIllegalArgumentException"> if the input matrix does not have
		/// at least two rows and one column </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Covariance(mathlib.linear.RealMatrix matrix, boolean biasCorrected) throws mathlib.exception.MathIllegalArgumentException
		public Covariance(RealMatrix matrix, bool biasCorrected)
		{
		   checkSufficientData(matrix);
		   n = matrix.RowDimension;
		   covarianceMatrix = computeCovarianceMatrix(matrix, biasCorrected);
		}

		/// <summary>
		/// Create a covariance matrix from a matrix whose columns
		/// represent covariates.
		/// 
		/// <p>The matrix must have at least one column and two rows</p>
		/// </summary>
		/// <param name="matrix"> matrix with columns representing covariates </param>
		/// <exception cref="MathIllegalArgumentException"> if the input matrix does not have
		/// at least two rows and one column </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Covariance(mathlib.linear.RealMatrix matrix) throws mathlib.exception.MathIllegalArgumentException
		public Covariance(RealMatrix matrix) : this(matrix, true)
		{
		}

		/// <summary>
		/// Returns the covariance matrix
		/// </summary>
		/// <returns> covariance matrix </returns>
		public virtual RealMatrix CovarianceMatrix
		{
			get
			{
				return covarianceMatrix;
			}
		}

		/// <summary>
		/// Returns the number of observations (length of covariate vectors)
		/// </summary>
		/// <returns> number of observations </returns>
		public virtual int N
		{
			get
			{
				return n;
			}
		}

		/// <summary>
		/// Compute a covariance matrix from a matrix whose columns represent
		/// covariates. </summary>
		/// <param name="matrix"> input matrix (must have at least one column and two rows) </param>
		/// <param name="biasCorrected"> determines whether or not covariance estimates are bias-corrected </param>
		/// <returns> covariance matrix </returns>
		/// <exception cref="MathIllegalArgumentException"> if the matrix does not contain sufficient data </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected mathlib.linear.RealMatrix computeCovarianceMatrix(mathlib.linear.RealMatrix matrix, boolean biasCorrected) throws mathlib.exception.MathIllegalArgumentException
		protected internal virtual RealMatrix computeCovarianceMatrix(RealMatrix matrix, bool biasCorrected)
		{
			int dimension = matrix.ColumnDimension;
			Variance variance = new Variance(biasCorrected);
			RealMatrix outMatrix = new BlockRealMatrix(dimension, dimension);
			for (int i = 0; i < dimension; i++)
			{
				for (int j = 0; j < i; j++)
				{
				  double cov = covariance(matrix.getColumn(i), matrix.getColumn(j), biasCorrected);
				  outMatrix.setEntry(i, j, cov);
				  outMatrix.setEntry(j, i, cov);
				}
				outMatrix.setEntry(i, i, variance.evaluate(matrix.getColumn(i)));
			}
			return outMatrix;
		}

		/// <summary>
		/// Create a covariance matrix from a matrix whose columns represent
		/// covariates. Covariances are computed using the bias-corrected formula. </summary>
		/// <param name="matrix"> input matrix (must have at least one column and two rows) </param>
		/// <returns> covariance matrix </returns>
		/// <exception cref="MathIllegalArgumentException"> if matrix does not contain sufficient data </exception>
		/// <seealso cref= #Covariance </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected mathlib.linear.RealMatrix computeCovarianceMatrix(mathlib.linear.RealMatrix matrix) throws mathlib.exception.MathIllegalArgumentException
		protected internal virtual RealMatrix computeCovarianceMatrix(RealMatrix matrix)
		{
			return computeCovarianceMatrix(matrix, true);
		}

		/// <summary>
		/// Compute a covariance matrix from a rectangular array whose columns represent
		/// covariates. </summary>
		/// <param name="data"> input array (must have at least one column and two rows) </param>
		/// <param name="biasCorrected"> determines whether or not covariance estimates are bias-corrected </param>
		/// <returns> covariance matrix </returns>
		/// <exception cref="MathIllegalArgumentException"> if the data array does not contain sufficient
		/// data </exception>
		/// <exception cref="NotStrictlyPositiveException"> if the input data array is not
		/// rectangular with at least one row and one column. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected mathlib.linear.RealMatrix computeCovarianceMatrix(double[][] data, boolean biasCorrected) throws mathlib.exception.MathIllegalArgumentException, mathlib.exception.NotStrictlyPositiveException
		protected internal virtual RealMatrix computeCovarianceMatrix(double[][] data, bool biasCorrected)
		{
			return computeCovarianceMatrix(new BlockRealMatrix(data), biasCorrected);
		}

		/// <summary>
		/// Create a covariance matrix from a rectangular array whose columns represent
		/// covariates. Covariances are computed using the bias-corrected formula. </summary>
		/// <param name="data"> input array (must have at least one column and two rows) </param>
		/// <returns> covariance matrix </returns>
		/// <exception cref="MathIllegalArgumentException"> if the data array does not contain sufficient data </exception>
		/// <exception cref="NotStrictlyPositiveException"> if the input data array is not
		/// rectangular with at least one row and one column. </exception>
		/// <seealso cref= #Covariance </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected mathlib.linear.RealMatrix computeCovarianceMatrix(double[][] data) throws mathlib.exception.MathIllegalArgumentException, mathlib.exception.NotStrictlyPositiveException
		protected internal virtual RealMatrix computeCovarianceMatrix(double[][] data)
		{
			return computeCovarianceMatrix(data, true);
		}

		/// <summary>
		/// Computes the covariance between the two arrays.
		/// 
		/// <p>Array lengths must match and the common length must be at least 2.</p>
		/// </summary>
		/// <param name="xArray"> first data array </param>
		/// <param name="yArray"> second data array </param>
		/// <param name="biasCorrected"> if true, returned value will be bias-corrected </param>
		/// <returns> returns the covariance for the two arrays </returns>
		/// <exception cref="MathIllegalArgumentException"> if the arrays lengths do not match or
		/// there is insufficient data </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double covariance(final double[] xArray, final double[] yArray, boolean biasCorrected) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double covariance(double[] xArray, double[] yArray, bool biasCorrected)
		{
			Mean mean = new Mean();
			double result = 0d;
			int length = xArray.Length;
			if (length != yArray.Length)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.DIMENSIONS_MISMATCH_SIMPLE, length, yArray.Length);
			}
			else if (length < 2)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.INSUFFICIENT_OBSERVED_POINTS_IN_SAMPLE, length, 2);
			}
			else
			{
				double xMean = mean.evaluate(xArray);
				double yMean = mean.evaluate(yArray);
				for (int i = 0; i < length; i++)
				{
					double xDev = xArray[i] - xMean;
					double yDev = yArray[i] - yMean;
					result += (xDev * yDev - result) / (i + 1);
				}
			}
			return biasCorrected ? result * ((double) length / (double)(length - 1)) : result;
		}

		/// <summary>
		/// Computes the covariance between the two arrays, using the bias-corrected
		/// formula.
		/// 
		/// <p>Array lengths must match and the common length must be at least 2.</p>
		/// </summary>
		/// <param name="xArray"> first data array </param>
		/// <param name="yArray"> second data array </param>
		/// <returns> returns the covariance for the two arrays </returns>
		/// <exception cref="MathIllegalArgumentException"> if the arrays lengths do not match or
		/// there is insufficient data </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double covariance(final double[] xArray, final double[] yArray) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double covariance(double[] xArray, double[] yArray)
		{
			return covariance(xArray, yArray, true);
		}

		/// <summary>
		/// Throws MathIllegalArgumentException if the matrix does not have at least
		/// one column and two rows. </summary>
		/// <param name="matrix"> matrix to check </param>
		/// <exception cref="MathIllegalArgumentException"> if the matrix does not contain sufficient data
		/// to compute covariance </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkSufficientData(final mathlib.linear.RealMatrix matrix) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void checkSufficientData(RealMatrix matrix)
		{
			int nRows = matrix.RowDimension;
			int nCols = matrix.ColumnDimension;
			if (nRows < 2 || nCols < 1)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.INSUFFICIENT_ROWS_AND_COLUMNS, nRows, nCols);
			}
		}
	}

}
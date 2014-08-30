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
namespace org.apache.commons.math3.stat.correlation
{

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MathUnsupportedOperationException = org.apache.commons.math3.exception.MathUnsupportedOperationException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using MatrixUtils = org.apache.commons.math3.linear.MatrixUtils;
	using RealMatrix = org.apache.commons.math3.linear.RealMatrix;

	/// <summary>
	/// Covariance implementation that does not require input data to be
	/// stored in memory. The size of the covariance matrix is specified in the
	/// constructor. Specific elements of the matrix are incrementally updated with
	/// calls to incrementRow() or increment Covariance().
	/// 
	/// <p>This class is based on a paper written by Philippe P&eacute;bay:
	/// <a href="http://prod.sandia.gov/techlib/access-control.cgi/2008/086212.pdf">
	/// Formulas for Robust, One-Pass Parallel Computation of Covariances and
	/// Arbitrary-Order Statistical Moments</a>, 2008, Technical Report SAND2008-6212,
	/// Sandia National Laboratories.</p>
	/// 
	/// <p>Note: the underlying covariance matrix is symmetric, thus only the
	/// upper triangular part of the matrix is stored and updated each increment.</p>
	/// 
	/// @version $Id: StorelessCovariance.java 1519851 2013-09-03 21:16:35Z tn $
	/// @since 3.0
	/// </summary>
	public class StorelessCovariance : Covariance
	{

		/// <summary>
		/// the square covariance matrix (upper triangular part) </summary>
		private StorelessBivariateCovariance[] covMatrix;

		/// <summary>
		/// dimension of the square covariance matrix </summary>
		private int dimension;

		/// <summary>
		/// Create a bias corrected covariance matrix with a given dimension.
		/// </summary>
		/// <param name="dim"> the dimension of the square covariance matrix </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public StorelessCovariance(final int dim)
		public StorelessCovariance(int dim) : this(dim, true)
		{
		}

		/// <summary>
		/// Create a covariance matrix with a given number of rows and columns and the
		/// indicated bias correction.
		/// </summary>
		/// <param name="dim"> the dimension of the covariance matrix </param>
		/// <param name="biasCorrected"> if <code>true</code> the covariance estimate is corrected
		/// for bias, i.e. n-1 in the denominator, otherwise there is no bias correction,
		/// i.e. n in the denominator. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public StorelessCovariance(final int dim, final boolean biasCorrected)
		public StorelessCovariance(int dim, bool biasCorrected)
		{
			dimension = dim;
			covMatrix = new StorelessBivariateCovariance[dimension * (dimension + 1) / 2];
			initializeMatrix(biasCorrected);
		}

		/// <summary>
		/// Initialize the internal two-dimensional array of
		/// <seealso cref="StorelessBivariateCovariance"/> instances.
		/// </summary>
		/// <param name="biasCorrected"> if the covariance estimate shall be corrected for bias </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void initializeMatrix(final boolean biasCorrected)
		private void initializeMatrix(bool biasCorrected)
		{
			for (int i = 0; i < dimension; i++)
			{
				for (int j = 0; j < dimension; j++)
				{
					setElement(i, j, new StorelessBivariateCovariance(biasCorrected));
				}
			}
		}

		/// <summary>
		/// Returns the index (i, j) translated into the one-dimensional
		/// array used to store the upper triangular part of the symmetric
		/// covariance matrix.
		/// </summary>
		/// <param name="i"> the row index </param>
		/// <param name="j"> the column index </param>
		/// <returns> the corresponding index in the matrix array </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private int indexOf(final int i, final int j)
		private int IndexOf(int i, int j)
		{
			return j < i ? i * (i + 1) / 2 + j : j * (j + 1) / 2 + i;
		}

		/// <summary>
		/// Gets the element at index (i, j) from the covariance matrix </summary>
		/// <param name="i"> the row index </param>
		/// <param name="j"> the column index </param>
		/// <returns> the <seealso cref="StorelessBivariateCovariance"/> element at the given index </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private StorelessBivariateCovariance getElement(final int i, final int j)
		private StorelessBivariateCovariance getElement(int i, int j)
		{
			return covMatrix[IndexOf(i, j)];
		}

		/// <summary>
		/// Sets the covariance element at index (i, j) in the covariance matrix </summary>
		/// <param name="i"> the row index </param>
		/// <param name="j"> the column index </param>
		/// <param name="cov"> the <seealso cref="StorelessBivariateCovariance"/> element to be set </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void setElement(final int i, final int j, final StorelessBivariateCovariance cov)
		private void setElement(int i, int j, StorelessBivariateCovariance cov)
		{
			covMatrix[IndexOf(i, j)] = cov;
		}

		/// <summary>
		/// Get the covariance for an individual element of the covariance matrix.
		/// </summary>
		/// <param name="xIndex"> row index in the covariance matrix </param>
		/// <param name="yIndex"> column index in the covariance matrix </param>
		/// <returns> the covariance of the given element </returns>
		/// <exception cref="NumberIsTooSmallException"> if the number of observations
		/// in the cell is &lt; 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getCovariance(final int xIndex, final int yIndex) throws org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double getCovariance(int xIndex, int yIndex)
		{

			return getElement(xIndex, yIndex).Result;

		}

		/// <summary>
		/// Increment the covariance matrix with one row of data.
		/// </summary>
		/// <param name="data"> array representing one row of data. </param>
		/// <exception cref="DimensionMismatchException"> if the length of <code>rowData</code>
		/// does not match with the covariance matrix </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void increment(final double[] data) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void increment(double[] data)
		{

			int length = data.Length;
			if (length != dimension)
			{
				throw new DimensionMismatchException(length, dimension);
			}

			// only update the upper triangular part of the covariance matrix
			// as only these parts are actually stored
			for (int i = 0; i < length; i++)
			{
				for (int j = i; j < length; j++)
				{
					getElement(i, j).increment(data[i], data[j]);
				}
			}

		}

		/// <summary>
		/// Appends {@code sc} to this, effectively aggregating the computations in {@code sc}
		/// with this.  After invoking this method, covariances returned should be close
		/// to what would have been obtained by performing all of the <seealso cref="#increment(double[])"/>
		/// operations in {@code sc} directly on this.
		/// </summary>
		/// <param name="sc"> externally computed StorelessCovariance to add to this </param>
		/// <exception cref="DimensionMismatchException"> if the dimension of sc does not match this
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void append(StorelessCovariance sc) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual void append(StorelessCovariance sc)
		{
			if (sc.dimension != dimension)
			{
				throw new DimensionMismatchException(sc.dimension, dimension);
			}

			// only update the upper triangular part of the covariance matrix
			// as only these parts are actually stored
			for (int i = 0; i < dimension; i++)
			{
				for (int j = i; j < dimension; j++)
				{
					getElement(i, j).append(sc.getElement(i, j));
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="NumberIsTooSmallException"> if the number of observations
		/// in a cell is &lt; 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.apache.commons.math3.linear.RealMatrix getCovarianceMatrix() throws org.apache.commons.math3.exception.NumberIsTooSmallException
		public override RealMatrix CovarianceMatrix
		{
			get
			{
				return MatrixUtils.createRealMatrix(Data);
			}
		}

		/// <summary>
		/// Return the covariance matrix as two-dimensional array.
		/// </summary>
		/// <returns> a two-dimensional double array of covariance values </returns>
		/// <exception cref="NumberIsTooSmallException"> if the number of observations
		/// for a cell is &lt; 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[][] getData() throws org.apache.commons.math3.exception.NumberIsTooSmallException
		public virtual double[][] Data
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[][] data = new double[dimension][dimension];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] data = new double[dimension][dimension];
				double[][] data = RectangularArrays.ReturnRectangularDoubleArray(dimension, dimension);
				for (int i = 0; i < dimension; i++)
				{
					for (int j = 0; j < dimension; j++)
					{
						data[i][j] = getElement(i, j).Result;
					}
				}
				return data;
			}
		}

		/// <summary>
		/// This <seealso cref="Covariance"/> method is not supported by a <seealso cref="StorelessCovariance"/>,
		/// since the number of bivariate observations does not have to be the same for different
		/// pairs of covariates - i.e., N as defined in <seealso cref="Covariance#getN()"/> is undefined.
		/// </summary>
		/// <returns> nothing as this implementation always throws a
		/// <seealso cref="MathUnsupportedOperationException"/> </returns>
		/// <exception cref="MathUnsupportedOperationException"> in all cases </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int getN() throws org.apache.commons.math3.exception.MathUnsupportedOperationException
		public override int N
		{
			get
			{
				throw new MathUnsupportedOperationException();
			}
		}
	}

}
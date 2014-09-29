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
namespace mathlib.stat.regression
{

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using InsufficientDataException = mathlib.exception.InsufficientDataException;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NoDataException = mathlib.exception.NoDataException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using NonSquareMatrixException = mathlib.linear.NonSquareMatrixException;
	using RealMatrix = mathlib.linear.RealMatrix;
	using Array2DRowRealMatrix = mathlib.linear.Array2DRowRealMatrix;
	using RealVector = mathlib.linear.RealVector;
	using ArrayRealVector = mathlib.linear.ArrayRealVector;
	using Variance = mathlib.stat.descriptive.moment.Variance;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Abstract base class for implementations of MultipleLinearRegression.
	/// @version $Id: AbstractMultipleLinearRegression.java 1547633 2013-12-03 23:03:06Z tn $
	/// @since 2.0
	/// </summary>
	public abstract class AbstractMultipleLinearRegression : MultipleLinearRegression
	{

		/// <summary>
		/// X sample data. </summary>
		private RealMatrix xMatrix;

		/// <summary>
		/// Y sample data. </summary>
		private RealVector yVector;

		/// <summary>
		/// Whether or not the regression model includes an intercept.  True means no intercept. </summary>
		private bool noIntercept = false;

		/// <returns> the X sample data. </returns>
		protected internal virtual RealMatrix X
		{
			get
			{
				return xMatrix;
			}
		}

		/// <returns> the Y sample data. </returns>
		protected internal virtual RealVector Y
		{
			get
			{
				return yVector;
			}
		}

		/// <returns> true if the model has no intercept term; false otherwise
		/// @since 2.2 </returns>
		public virtual bool NoIntercept
		{
			get
			{
				return noIntercept;
			}
			set
			{
				this.noIntercept = value;
			}
		}


		/// <summary>
		/// <p>Loads model x and y sample data from a flat input array, overriding any previous sample.
		/// </p>
		/// <p>Assumes that rows are concatenated with y values first in each row.  For example, an input
		/// <code>data</code> array containing the sequence of values (1, 2, 3, 4, 5, 6, 7, 8, 9) with
		/// <code>nobs = 3</code> and <code>nvars = 2</code> creates a regression dataset with two
		/// independent variables, as below:
		/// <pre>
		///   y   x[0]  x[1]
		///   --------------
		///   1     2     3
		///   4     5     6
		///   7     8     9
		/// </pre>
		/// </p>
		/// <p>Note that there is no need to add an initial unitary column (column of 1's) when
		/// specifying a model including an intercept term.  If <seealso cref="#isNoIntercept()"/> is <code>true</code>,
		/// the X matrix will be created without an initial column of "1"s; otherwise this column will
		/// be added.
		/// </p>
		/// <p>Throws IllegalArgumentException if any of the following preconditions fail:
		/// <ul><li><code>data</code> cannot be null</li>
		/// <li><code>data.length = nobs * (nvars + 1)</li>
		/// <li><code>nobs > nvars</code></li></ul>
		/// </p>
		/// </summary>
		/// <param name="data"> input data array </param>
		/// <param name="nobs"> number of observations (rows) </param>
		/// <param name="nvars"> number of independent variables (columns, not counting y) </param>
		/// <exception cref="NullArgumentException"> if the data array is null </exception>
		/// <exception cref="DimensionMismatchException"> if the length of the data array is not equal
		/// to <code>nobs * (nvars + 1)</code> </exception>
		/// <exception cref="InsufficientDataException"> if <code>nobs</code> is less than
		/// <code>nvars + 1</code> </exception>
		public virtual void newSampleData(double[] data, int nobs, int nvars)
		{
			if (data == null)
			{
				throw new NullArgumentException();
			}
			if (data.Length != nobs * (nvars + 1))
			{
				throw new DimensionMismatchException(data.Length, nobs * (nvars + 1));
			}
			if (nobs <= nvars)
			{
				throw new InsufficientDataException(LocalizedFormats.INSUFFICIENT_OBSERVED_POINTS_IN_SAMPLE, nobs, nvars + 1);
			}
			double[] y = new double[nobs];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int cols = noIntercept ? nvars: nvars + 1;
			int cols = noIntercept ? nvars: nvars + 1;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] x = new double[nobs][cols];
			double[][] x = RectangularArrays.ReturnRectangularDoubleArray(nobs, cols);
			int pointer = 0;
			for (int i = 0; i < nobs; i++)
			{
				y[i] = data[pointer++];
				if (!noIntercept)
				{
					x[i][0] = 1.0d;
				}
				for (int j = noIntercept ? 0 : 1; j < cols; j++)
				{
					x[i][j] = data[pointer++];
				}
			}
			this.xMatrix = new Array2DRowRealMatrix(x);
			this.yVector = new ArrayRealVector(y);
		}

		/// <summary>
		/// Loads new y sample data, overriding any previous data.
		/// </summary>
		/// <param name="y"> the array representing the y sample </param>
		/// <exception cref="NullArgumentException"> if y is null </exception>
		/// <exception cref="NoDataException"> if y is empty </exception>
		protected internal virtual void newYSampleData(double[] y)
		{
			if (y == null)
			{
				throw new NullArgumentException();
			}
			if (y.Length == 0)
			{
				throw new NoDataException();
			}
			this.yVector = new ArrayRealVector(y);
		}

		/// <summary>
		/// <p>Loads new x sample data, overriding any previous data.
		/// </p>
		/// The input <code>x</code> array should have one row for each sample
		/// observation, with columns corresponding to independent variables.
		/// For example, if <pre>
		/// <code> x = new double[][] {{1, 2}, {3, 4}, {5, 6}} </code></pre>
		/// then <code>setXSampleData(x) </code> results in a model with two independent
		/// variables and 3 observations:
		/// <pre>
		///   x[0]  x[1]
		///   ----------
		///     1    2
		///     3    4
		///     5    6
		/// </pre>
		/// </p>
		/// <p>Note that there is no need to add an initial unitary column (column of 1's) when
		/// specifying a model including an intercept term.
		/// </p> </summary>
		/// <param name="x"> the rectangular array representing the x sample </param>
		/// <exception cref="NullArgumentException"> if x is null </exception>
		/// <exception cref="NoDataException"> if x is empty </exception>
		/// <exception cref="DimensionMismatchException"> if x is not rectangular </exception>
		protected internal virtual void newXSampleData(double[][] x)
		{
			if (x == null)
			{
				throw new NullArgumentException();
			}
			if (x.Length == 0)
			{
				throw new NoDataException();
			}
			if (noIntercept)
			{
				this.xMatrix = new Array2DRowRealMatrix(x, true);
			} // Augment design matrix with initial unitary column
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nVars = x[0].length;
				int nVars = x[0].Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] xAug = new double[x.length][nVars + 1];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] xAug = new double[x.Length][nVars + 1];
				double[][] xAug = RectangularArrays.ReturnRectangularDoubleArray(x.Length, nVars + 1);
				for (int i = 0; i < x.Length; i++)
				{
					if (x[i].Length != nVars)
					{
						throw new DimensionMismatchException(x[i].Length, nVars);
					}
					xAug[i][0] = 1.0d;
					Array.Copy(x[i], 0, xAug[i], 1, nVars);
				}
				this.xMatrix = new Array2DRowRealMatrix(xAug, false);
			}
		}

		/// <summary>
		/// Validates sample data.  Checks that
		/// <ul><li>Neither x nor y is null or empty;</li>
		/// <li>The length (i.e. number of rows) of x equals the length of y</li>
		/// <li>x has at least one more row than it has columns (i.e. there is
		/// sufficient data to estimate regression coefficients for each of the
		/// columns in x plus an intercept.</li>
		/// </ul>
		/// </summary>
		/// <param name="x"> the [n,k] array representing the x data </param>
		/// <param name="y"> the [n,1] array representing the y data </param>
		/// <exception cref="NullArgumentException"> if {@code x} or {@code y} is null </exception>
		/// <exception cref="DimensionMismatchException"> if {@code x} and {@code y} do not
		/// have the same length </exception>
		/// <exception cref="NoDataException"> if {@code x} or {@code y} are zero-length </exception>
		/// <exception cref="MathIllegalArgumentException"> if the number of rows of {@code x}
		/// is not larger than the number of columns + 1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void validateSampleData(double[][] x, double[] y) throws mathlib.exception.MathIllegalArgumentException
		protected internal virtual void validateSampleData(double[][] x, double[] y)
		{
			if ((x == null) || (y == null))
			{
				throw new NullArgumentException();
			}
			if (x.Length != y.Length)
			{
				throw new DimensionMismatchException(y.Length, x.Length);
			}
			if (x.Length == 0) // Must be no y data either
			{
				throw new NoDataException();
			}
			if (x[0].Length + 1 > x.Length)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.NOT_ENOUGH_DATA_FOR_NUMBER_OF_PREDICTORS, x.Length, x[0].Length);
			}
		}

		/// <summary>
		/// Validates that the x data and covariance matrix have the same
		/// number of rows and that the covariance matrix is square.
		/// </summary>
		/// <param name="x"> the [n,k] array representing the x sample </param>
		/// <param name="covariance"> the [n,n] array representing the covariance matrix </param>
		/// <exception cref="DimensionMismatchException"> if the number of rows in x is not equal
		/// to the number of rows in covariance </exception>
		/// <exception cref="NonSquareMatrixException"> if the covariance matrix is not square </exception>
		protected internal virtual void validateCovarianceData(double[][] x, double[][] covariance)
		{
			if (x.Length != covariance.Length)
			{
				throw new DimensionMismatchException(x.Length, covariance.Length);
			}
			if (covariance.Length > 0 && covariance.Length != covariance[0].Length)
			{
				throw new NonSquareMatrixException(covariance.Length, covariance[0].Length);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual double[] estimateRegressionParameters()
		{
			RealVector b = calculateBeta();
			return b.toArray();
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual double[] estimateResiduals()
		{
			RealVector b = calculateBeta();
			RealVector e = yVector.subtract(xMatrix.operate(b));
			return e.toArray();
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual double[][] estimateRegressionParametersVariance()
		{
			return calculateBetaVariance().Data;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual double[] estimateRegressionParametersStandardErrors()
		{
			double[][] betaVariance = estimateRegressionParametersVariance();
			double sigma = calculateErrorVariance();
			int length = betaVariance[0].Length;
			double[] result = new double[length];
			for (int i = 0; i < length; i++)
			{
				result[i] = FastMath.sqrt(sigma * betaVariance[i][i]);
			}
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual double estimateRegressandVariance()
		{
			return calculateYVariance();
		}

		/// <summary>
		/// Estimates the variance of the error.
		/// </summary>
		/// <returns> estimate of the error variance
		/// @since 2.2 </returns>
		public virtual double estimateErrorVariance()
		{
			return calculateErrorVariance();

		}

		/// <summary>
		/// Estimates the standard error of the regression.
		/// </summary>
		/// <returns> regression standard error
		/// @since 2.2 </returns>
		public virtual double estimateRegressionStandardError()
		{
			return FastMath.sqrt(estimateErrorVariance());
		}

		/// <summary>
		/// Calculates the beta of multiple linear regression in matrix notation.
		/// </summary>
		/// <returns> beta </returns>
		protected internal abstract RealVector calculateBeta();

		/// <summary>
		/// Calculates the beta variance of multiple linear regression in matrix
		/// notation.
		/// </summary>
		/// <returns> beta variance </returns>
		protected internal abstract RealMatrix calculateBetaVariance();


		/// <summary>
		/// Calculates the variance of the y values.
		/// </summary>
		/// <returns> Y variance </returns>
		protected internal virtual double calculateYVariance()
		{
			return (new Variance()).evaluate(yVector.toArray());
		}

		/// <summary>
		/// <p>Calculates the variance of the error term.</p>
		/// Uses the formula <pre>
		/// var(u) = u &middot; u / (n - k)
		/// </pre>
		/// where n and k are the row and column dimensions of the design
		/// matrix X.
		/// </summary>
		/// <returns> error variance estimate
		/// @since 2.2 </returns>
		protected internal virtual double calculateErrorVariance()
		{
			RealVector residuals = calculateResiduals();
			return residuals.dotProduct(residuals) / (xMatrix.RowDimension - xMatrix.ColumnDimension);
		}

		/// <summary>
		/// Calculates the residuals of multiple linear regression in matrix
		/// notation.
		/// 
		/// <pre>
		/// u = y - X * b
		/// </pre>
		/// </summary>
		/// <returns> The residuals [n,1] matrix </returns>
		protected internal virtual RealVector calculateResiduals()
		{
			RealVector b = calculateBeta();
			return yVector.subtract(xMatrix.operate(b));
		}

	}

}
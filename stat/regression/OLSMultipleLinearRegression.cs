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

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using Array2DRowRealMatrix = mathlib.linear.Array2DRowRealMatrix;
	using LUDecomposition = mathlib.linear.LUDecomposition;
	using QRDecomposition = mathlib.linear.QRDecomposition;
	using RealMatrix = mathlib.linear.RealMatrix;
	using RealVector = mathlib.linear.RealVector;
	using SecondMoment = mathlib.stat.descriptive.moment.SecondMoment;

	/// <summary>
	/// <p>Implements ordinary least squares (OLS) to estimate the parameters of a
	/// multiple linear regression model.</p>
	/// 
	/// <p>The regression coefficients, <code>b</code>, satisfy the normal equations:
	/// <pre><code> X<sup>T</sup> X b = X<sup>T</sup> y </code></pre></p>
	/// 
	/// <p>To solve the normal equations, this implementation uses QR decomposition
	/// of the <code>X</code> matrix. (See <seealso cref="QRDecomposition"/> for details on the
	/// decomposition algorithm.) The <code>X</code> matrix, also known as the <i>design matrix,</i>
	/// has rows corresponding to sample observations and columns corresponding to independent
	/// variables.  When the model is estimated using an intercept term (i.e. when
	/// <seealso cref="#isNoIntercept() isNoIntercept"/> is false as it is by default), the <code>X</code>
	/// matrix includes an initial column identically equal to 1.  We solve the normal equations
	/// as follows:
	/// <pre><code> X<sup>T</sup>X b = X<sup>T</sup> y
	/// (QR)<sup>T</sup> (QR) b = (QR)<sup>T</sup>y
	/// R<sup>T</sup> (Q<sup>T</sup>Q) R b = R<sup>T</sup> Q<sup>T</sup> y
	/// R<sup>T</sup> R b = R<sup>T</sup> Q<sup>T</sup> y
	/// (R<sup>T</sup>)<sup>-1</sup> R<sup>T</sup> R b = (R<sup>T</sup>)<sup>-1</sup> R<sup>T</sup> Q<sup>T</sup> y
	/// R b = Q<sup>T</sup> y </code></pre></p>
	/// 
	/// <p>Given <code>Q</code> and <code>R</code>, the last equation is solved by back-substitution.</p>
	/// 
	/// @version $Id: OLSMultipleLinearRegression.java 1591624 2014-05-01 11:54:06Z tn $
	/// @since 2.0
	/// </summary>
	public class OLSMultipleLinearRegression : AbstractMultipleLinearRegression
	{

		/// <summary>
		/// Cached QR decomposition of X matrix </summary>
		private QRDecomposition qr = null;

		/// <summary>
		/// Singularity threshold for QR decomposition </summary>
		private readonly double threshold;

		/// <summary>
		/// Create an empty OLSMultipleLinearRegression instance.
		/// </summary>
		public OLSMultipleLinearRegression() : this(0d)
		{
		}

		/// <summary>
		/// Create an empty OLSMultipleLinearRegression instance, using the given
		/// singularity threshold for the QR decomposition.
		/// </summary>
		/// <param name="threshold"> the singularity threshold
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public OLSMultipleLinearRegression(final double threshold)
		public OLSMultipleLinearRegression(double threshold)
		{
			this.threshold = threshold;
		}

		/// <summary>
		/// Loads model x and y sample data, overriding any previous sample.
		/// 
		/// Computes and caches QR decomposition of the X matrix. </summary>
		/// <param name="y"> the [n,1] array representing the y sample </param>
		/// <param name="x"> the [n,k] array representing the x sample </param>
		/// <exception cref="MathIllegalArgumentException"> if the x and y array data are not
		///             compatible for the regression </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void newSampleData(double[] y, double[][] x) throws mathlib.exception.MathIllegalArgumentException
		public virtual void newSampleData(double[] y, double[][] x)
		{
			validateSampleData(x, y);
			newYSampleData(y);
			newXSampleData(x);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>This implementation computes and caches the QR decomposition of the X matrix.</p>
		/// </summary>
		public override void newSampleData(double[] data, int nobs, int nvars)
		{
			base.newSampleData(data, nobs, nvars);
			qr = new QRDecomposition(X, threshold);
		}

		/// <summary>
		/// <p>Compute the "hat" matrix.
		/// </p>
		/// <p>The hat matrix is defined in terms of the design matrix X
		///  by X(X<sup>T</sup>X)<sup>-1</sup>X<sup>T</sup>
		/// </p>
		/// <p>The implementation here uses the QR decomposition to compute the
		/// hat matrix as Q I<sub>p</sub>Q<sup>T</sup> where I<sub>p</sub> is the
		/// p-dimensional identity matrix augmented by 0's.  This computational
		/// formula is from "The Hat Matrix in Regression and ANOVA",
		/// David C. Hoaglin and Roy E. Welsch,
		/// <i>The American Statistician</i>, Vol. 32, No. 1 (Feb., 1978), pp. 17-22.
		/// </p>
		/// <p>Data for the model must have been successfully loaded using one of
		/// the {@code newSampleData} methods before invoking this method; otherwise
		/// a {@code NullPointerException} will be thrown.</p>
		/// </summary>
		/// <returns> the hat matrix </returns>
		/// <exception cref="NullPointerException"> unless method {@code newSampleData} has been
		/// called beforehand. </exception>
		public virtual RealMatrix calculateHat()
		{
			// Create augmented identity matrix
			RealMatrix Q = qr.Q;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int p = qr.getR().getColumnDimension();
			int p = qr.R.ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = Q.getColumnDimension();
			int n = Q.ColumnDimension;
			// No try-catch or advertised NotStrictlyPositiveException - NPE above if n < 3
			Array2DRowRealMatrix augI = new Array2DRowRealMatrix(n, n);
			double[][] augIData = augI.DataRef;
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					if (i == j && i < p)
					{
						augIData[i][j] = 1d;
					}
					else
					{
						augIData[i][j] = 0d;
					}
				}
			}

			// Compute and return Hat matrix
			// No DME advertised - args valid if we get here
			return Q.multiply(augI).multiply(Q.transpose());
		}

		/// <summary>
		/// <p>Returns the sum of squared deviations of Y from its mean.</p>
		/// 
		/// <p>If the model has no intercept term, <code>0</code> is used for the
		/// mean of Y - i.e., what is returned is the sum of the squared Y values.</p>
		/// 
		/// <p>The value returned by this method is the SSTO value used in
		/// the <seealso cref="#calculateRSquared() R-squared"/> computation.</p>
		/// </summary>
		/// <returns> SSTO - the total sum of squares </returns>
		/// <exception cref="MathIllegalArgumentException"> if the sample has not been set or does
		/// not contain at least 3 observations </exception>
		/// <seealso cref= #isNoIntercept()
		/// @since 2.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double calculateTotalSumOfSquares() throws mathlib.exception.MathIllegalArgumentException
		public virtual double calculateTotalSumOfSquares()
		{
			if (NoIntercept)
			{
				return StatUtils.sumSq(Y.toArray());
			}
			else
			{
				return (new SecondMoment()).evaluate(Y.toArray());
			}
		}

		/// <summary>
		/// Returns the sum of squared residuals.
		/// </summary>
		/// <returns> residual sum of squares
		/// @since 2.2 </returns>
		public virtual double calculateResidualSumOfSquares()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealVector residuals = calculateResiduals();
			RealVector residuals = calculateResiduals();
			// No advertised DME, args are valid
			return residuals.dotProduct(residuals);
		}

		/// <summary>
		/// Returns the R-Squared statistic, defined by the formula <pre>
		/// R<sup>2</sup> = 1 - SSR / SSTO
		/// </pre>
		/// where SSR is the <seealso cref="#calculateResidualSumOfSquares() sum of squared residuals"/>
		/// and SSTO is the <seealso cref="#calculateTotalSumOfSquares() total sum of squares"/>
		/// </summary>
		/// <returns> R-square statistic </returns>
		/// <exception cref="MathIllegalArgumentException"> if the sample has not been set or does
		/// not contain at least 3 observations
		/// @since 2.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double calculateRSquared() throws mathlib.exception.MathIllegalArgumentException
		public virtual double calculateRSquared()
		{
			return 1 - calculateResidualSumOfSquares() / calculateTotalSumOfSquares();
		}

		/// <summary>
		/// <p>Returns the adjusted R-squared statistic, defined by the formula <pre>
		/// R<sup>2</sup><sub>adj</sub> = 1 - [SSR (n - 1)] / [SSTO (n - p)]
		/// </pre>
		/// where SSR is the <seealso cref="#calculateResidualSumOfSquares() sum of squared residuals"/>,
		/// SSTO is the <seealso cref="#calculateTotalSumOfSquares() total sum of squares"/>, n is the number
		/// of observations and p is the number of parameters estimated (including the intercept).</p>
		/// 
		/// <p>If the regression is estimated without an intercept term, what is returned is <pre>
		/// <code> 1 - (1 - <seealso cref="#calculateRSquared()"/>) * (n / (n - p)) </code>
		/// </pre></p>
		/// </summary>
		/// <returns> adjusted R-Squared statistic </returns>
		/// <exception cref="MathIllegalArgumentException"> if the sample has not been set or does
		/// not contain at least 3 observations </exception>
		/// <seealso cref= #isNoIntercept()
		/// @since 2.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double calculateAdjustedRSquared() throws mathlib.exception.MathIllegalArgumentException
		public virtual double calculateAdjustedRSquared()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double n = getX().getRowDimension();
			double n = X.RowDimension;
			if (NoIntercept)
			{
				return 1 - (1 - calculateRSquared()) * (n / (n - X.ColumnDimension));
			}
			else
			{
				return 1 - (calculateResidualSumOfSquares() * (n - 1)) / (calculateTotalSumOfSquares() * (n - X.ColumnDimension));
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>This implementation computes and caches the QR decomposition of the X matrix
		/// once it is successfully loaded.</p>
		/// </summary>
		protected internal override void newXSampleData(double[][] x)
		{
			base.newXSampleData(x);
			qr = new QRDecomposition(X);
		}

		/// <summary>
		/// Calculates the regression coefficients using OLS.
		/// 
		/// <p>Data for the model must have been successfully loaded using one of
		/// the {@code newSampleData} methods before invoking this method; otherwise
		/// a {@code NullPointerException} will be thrown.</p>
		/// </summary>
		/// <returns> beta </returns>
		protected internal override RealVector calculateBeta()
		{
			return qr.Solver.solve(Y);
		}

		/// <summary>
		/// <p>Calculates the variance-covariance matrix of the regression parameters.
		/// </p>
		/// <p>Var(b) = (X<sup>T</sup>X)<sup>-1</sup>
		/// </p>
		/// <p>Uses QR decomposition to reduce (X<sup>T</sup>X)<sup>-1</sup>
		/// to (R<sup>T</sup>R)<sup>-1</sup>, with only the top p rows of
		/// R included, where p = the length of the beta vector.</p>
		/// 
		/// <p>Data for the model must have been successfully loaded using one of
		/// the {@code newSampleData} methods before invoking this method; otherwise
		/// a {@code NullPointerException} will be thrown.</p>
		/// </summary>
		/// <returns> The beta variance-covariance matrix </returns>
		protected internal override RealMatrix calculateBetaVariance()
		{
			int p = X.ColumnDimension;
			RealMatrix Raug = qr.R.getSubMatrix(0, p - 1, 0, p - 1);
			RealMatrix Rinv = (new LUDecomposition(Raug)).Solver.Inverse;
			return Rinv.multiply(Rinv.transpose());
		}

	}

}
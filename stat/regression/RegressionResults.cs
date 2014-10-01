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

	using FastMath = mathlib.util.FastMath;
	using MathArrays = mathlib.util.MathArrays;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;

	/// <summary>
	/// Results of a Multiple Linear Regression model fit.
	/// 
	/// @version $Id: RegressionResults.java 1392342 2012-10-01 14:08:52Z psteitz $
	/// @since 3.0
	/// </summary>
	[Serializable]
	public class RegressionResults
	{

		/// <summary>
		/// INDEX of Sum of Squared Errors </summary>
		private const int SSE_IDX = 0;
		/// <summary>
		/// INDEX of Sum of Squares of Model </summary>
		private const int SST_IDX = 1;
		/// <summary>
		/// INDEX of R-Squared of regression </summary>
		private const int RSQ_IDX = 2;
		/// <summary>
		/// INDEX of Mean Squared Error </summary>
		private const int MSE_IDX = 3;
		/// <summary>
		/// INDEX of Adjusted R Squared </summary>
		private const int ADJRSQ_IDX = 4;
		/// <summary>
		/// UID </summary>
		private const long serialVersionUID = 1L;
		/// <summary>
		/// regression slope parameters </summary>
		private readonly double[] parameters;
		/// <summary>
		/// variance covariance matrix of parameters </summary>
		private readonly double[][] varCovData;
		/// <summary>
		/// boolean flag for variance covariance matrix in symm compressed storage </summary>
		private readonly bool isSymmetricVCD;
		/// <summary>
		/// rank of the solution </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") private final int rank;
		private readonly int rank;
		/// <summary>
		/// number of observations on which results are based </summary>
		private readonly long nobs;
		/// <summary>
		/// boolean flag indicator of whether a constant was included </summary>
		private readonly bool containsConstant;
		/// <summary>
		/// array storing global results, SSE, MSE, RSQ, adjRSQ </summary>
		private readonly double[] globalFitInfo;

		/// <summary>
		///  Set the default constructor to private access
		///  to prevent inadvertent instantiation
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") private RegressionResults()
		private RegressionResults()
		{
			this.parameters = null;
			this.varCovData = null;
			this.rank = -1;
			this.nobs = -1;
			this.containsConstant = false;
			this.isSymmetricVCD = false;
			this.globalFitInfo = null;
		}

		/// <summary>
		/// Constructor for Regression Results.
		/// </summary>
		/// <param name="parameters"> a double array with the regression slope estimates </param>
		/// <param name="varcov"> the variance covariance matrix, stored either in a square matrix
		/// or as a compressed </param>
		/// <param name="isSymmetricCompressed"> a flag which denotes that the variance covariance
		/// matrix is in symmetric compressed format </param>
		/// <param name="nobs"> the number of observations of the regression estimation </param>
		/// <param name="rank"> the number of independent variables in the regression </param>
		/// <param name="sumy"> the sum of the independent variable </param>
		/// <param name="sumysq"> the sum of the squared independent variable </param>
		/// <param name="sse"> sum of squared errors </param>
		/// <param name="containsConstant"> true model has constant,  false model does not have constant </param>
		/// <param name="copyData"> if true a deep copy of all input data is made, if false only references
		/// are copied and the RegressionResults become mutable </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RegressionResults(final double[] parameters, final double[][] varcov, final boolean isSymmetricCompressed, final long nobs, final int rank, final double sumy, final double sumysq, final double sse, final boolean containsConstant, final boolean copyData)
		public RegressionResults(double[] parameters, double[][] varcov, bool isSymmetricCompressed, long nobs, int rank, double sumy, double sumysq, double sse, bool containsConstant, bool copyData)
		{
			if (copyData)
			{
				this.parameters = MathArrays.copyOf(parameters);
				this.varCovData = new double[varcov.Length][];
				for (int i = 0; i < varcov.Length; i++)
				{
					this.varCovData[i] = MathArrays.copyOf(varcov[i]);
				}
			}
			else
			{
				this.parameters = parameters;
				this.varCovData = varcov;
			}
			this.isSymmetricVCD = isSymmetricCompressed;
			this.nobs = nobs;
			this.rank = rank;
			this.containsConstant = containsConstant;
			this.globalFitInfo = new double[5];
			Arrays.fill(this.globalFitInfo, double.NaN);

			if (rank > 0)
			{
				this.globalFitInfo[SST_IDX] = containsConstant ? (sumysq - sumy * sumy / nobs) : sumysq;
			}

			this.globalFitInfo[SSE_IDX] = sse;
			this.globalFitInfo[MSE_IDX] = this.globalFitInfo[SSE_IDX] / (nobs - rank);
			this.globalFitInfo[RSQ_IDX] = 1.0 - this.globalFitInfo[SSE_IDX] / this.globalFitInfo[SST_IDX];

			if (!containsConstant)
			{
				this.globalFitInfo[ADJRSQ_IDX] = 1.0 - (1.0 - this.globalFitInfo[RSQ_IDX]) * ((double) nobs / ((double)(nobs - rank)));
			}
			else
			{
				this.globalFitInfo[ADJRSQ_IDX] = 1.0 - (sse * (nobs - 1.0)) / (globalFitInfo[SST_IDX] * (nobs - rank));
			}
		}

		/// <summary>
		/// <p>Returns the parameter estimate for the regressor at the given index.</p>
		/// 
		/// <p>A redundant regressor will have its redundancy flag set, as well as
		///  a parameters estimated equal to {@code Double.NaN}</p>
		/// </summary>
		/// <param name="index"> Index. </param>
		/// <returns> the parameters estimated for regressor at index. </returns>
		/// <exception cref="OutOfRangeException"> if {@code index} is not in the interval
		/// {@code [0, number of parameters)}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getParameterEstimate(int index) throws mathlib.exception.OutOfRangeException
		public virtual double getParameterEstimate(int index)
		{
			if (parameters == null)
			{
				return double.NaN;
			}
			if (index < 0 || index >= this.parameters.Length)
			{
				throw new OutOfRangeException(index, 0, this.parameters.Length - 1);
			}
			return this.parameters[index];
		}

		/// <summary>
		/// <p>Returns a copy of the regression parameters estimates.</p>
		/// 
		/// <p>The parameter estimates are returned in the natural order of the data.</p>
		/// 
		/// <p>A redundant regressor will have its redundancy flag set, as will
		///  a parameter estimate equal to {@code Double.NaN}.</p>
		/// </summary>
		/// <returns> array of parameter estimates, null if no estimation occurred </returns>
		public virtual double[] ParameterEstimates
		{
			get
			{
				if (this.parameters == null)
				{
					return null;
				}
				return MathArrays.copyOf(parameters);
			}
		}

		/// <summary>
		/// Returns the <a href="http://www.xycoon.com/standerrorb(1).htm">standard
		/// error of the parameter estimate at index</a>,
		/// usually denoted s(b<sub>index</sub>).
		/// </summary>
		/// <param name="index"> Index. </param>
		/// <returns> the standard errors associated with parameters estimated at index. </returns>
		/// <exception cref="OutOfRangeException"> if {@code index} is not in the interval
		/// {@code [0, number of parameters)}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getStdErrorOfEstimate(int index) throws mathlib.exception.OutOfRangeException
		public virtual double getStdErrorOfEstimate(int index)
		{
			if (parameters == null)
			{
				return double.NaN;
			}
			if (index < 0 || index >= this.parameters.Length)
			{
				throw new OutOfRangeException(index, 0, this.parameters.Length - 1);
			}
			double @var = this.getVcvElement(index, index);
			if (!double.IsNaN(@var) && @var > double.MinValue)
			{
				return FastMath.sqrt(@var);
			}
			return double.NaN;
		}

		/// <summary>
		/// <p>Returns the <a href="http://www.xycoon.com/standerrorb(1).htm">standard
		/// error of the parameter estimates</a>,
		/// usually denoted s(b<sub>i</sub>).</p>
		/// 
		/// <p>If there are problems with an ill conditioned design matrix then the regressor
		/// which is redundant will be assigned <code>Double.NaN</code>. </p>
		/// </summary>
		/// <returns> an array standard errors associated with parameters estimates,
		///  null if no estimation occurred </returns>
		public virtual double[] StdErrorOfEstimates
		{
			get
			{
				if (parameters == null)
				{
					return null;
				}
				double[] se = new double[this.parameters.Length];
				for (int i = 0; i < this.parameters.Length; i++)
				{
					double @var = this.getVcvElement(i, i);
					if (!double.IsNaN(@var) && @var > double.MinValue)
					{
						se[i] = FastMath.sqrt(@var);
						continue;
					}
					se[i] = double.NaN;
				}
				return se;
			}
		}

		/// <summary>
		/// <p>Returns the covariance between regression parameters i and j.</p>
		/// 
		/// <p>If there are problems with an ill conditioned design matrix then the covariance
		/// which involves redundant columns will be assigned {@code Double.NaN}. </p>
		/// </summary>
		/// <param name="i"> {@code i}th regression parameter. </param>
		/// <param name="j"> {@code j}th regression parameter. </param>
		/// <returns> the covariance of the parameter estimates. </returns>
		/// <exception cref="OutOfRangeException"> if {@code i} or {@code j} is not in the
		/// interval {@code [0, number of parameters)}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getCovarianceOfParameters(int i, int j) throws mathlib.exception.OutOfRangeException
		public virtual double getCovarianceOfParameters(int i, int j)
		{
			if (parameters == null)
			{
				return double.NaN;
			}
			if (i < 0 || i >= this.parameters.Length)
			{
				throw new OutOfRangeException(i, 0, this.parameters.Length - 1);
			}
			if (j < 0 || j >= this.parameters.Length)
			{
				throw new OutOfRangeException(j, 0, this.parameters.Length - 1);
			}
			return this.getVcvElement(i, j);
		}

		/// <summary>
		/// <p>Returns the number of parameters estimated in the model.</p>
		/// 
		/// <p>This is the maximum number of regressors, some techniques may drop
		/// redundant parameters</p>
		/// </summary>
		/// <returns> number of regressors, -1 if not estimated </returns>
		public virtual int NumberOfParameters
		{
			get
			{
				if (this.parameters == null)
				{
					return -1;
				}
				return this.parameters.Length;
			}
		}

		/// <summary>
		/// Returns the number of observations added to the regression model.
		/// </summary>
		/// <returns> Number of observations, -1 if an error condition prevents estimation </returns>
		public virtual long N
		{
			get
			{
				return this.nobs;
			}
		}

		/// <summary>
		/// <p>Returns the sum of squared deviations of the y values about their mean.</p>
		/// 
		/// <p>This is defined as SSTO
		/// <a href="http://www.xycoon.com/SumOfSquares.htm">here</a>.</p>
		/// 
		/// <p>If {@code n < 2}, this returns {@code Double.NaN}.</p>
		/// </summary>
		/// <returns> sum of squared deviations of y values </returns>
		public virtual double TotalSumSquares
		{
			get
			{
				return this.globalFitInfo[SST_IDX];
			}
		}

		/// <summary>
		/// <p>Returns the sum of squared deviations of the predicted y values about
		/// their mean (which equals the mean of y).</p>
		/// 
		/// <p>This is usually abbreviated SSR or SSM.  It is defined as SSM
		/// <a href="http://www.xycoon.com/SumOfSquares.htm">here</a></p>
		/// 
		/// <p><strong>Preconditions</strong>: <ul>
		/// <li>At least two observations (with at least two different x values)
		/// must have been added before invoking this method. If this method is
		/// invoked before a model can be estimated, <code>Double.NaN</code> is
		/// returned.
		/// </li></ul></p>
		/// </summary>
		/// <returns> sum of squared deviations of predicted y values </returns>
		public virtual double RegressionSumSquares
		{
			get
			{
				return this.globalFitInfo[SST_IDX] - this.globalFitInfo[SSE_IDX];
			}
		}

		/// <summary>
		/// <p>Returns the <a href="http://www.xycoon.com/SumOfSquares.htm">
		/// sum of squared errors</a> (SSE) associated with the regression
		/// model.</p>
		/// 
		/// <p>The return value is constrained to be non-negative - i.e., if due to
		/// rounding errors the computational formula returns a negative result,
		/// 0 is returned.</p>
		/// 
		/// <p><strong>Preconditions</strong>: <ul>
		/// <li>numberOfParameters data pairs
		/// must have been added before invoking this method. If this method is
		/// invoked before a model can be estimated, <code>Double,NaN</code> is
		/// returned.
		/// </li></ul></p>
		/// </summary>
		/// <returns> sum of squared errors associated with the regression model </returns>
		public virtual double ErrorSumSquares
		{
			get
			{
				return this.globalFitInfo[SSE_IDX];
			}
		}

		/// <summary>
		/// <p>Returns the sum of squared errors divided by the degrees of freedom,
		/// usually abbreviated MSE.</p>
		/// 
		/// <p>If there are fewer than <strong>numberOfParameters + 1</strong> data pairs in the model,
		/// or if there is no variation in <code>x</code>, this returns
		/// <code>Double.NaN</code>.</p>
		/// </summary>
		/// <returns> sum of squared deviations of y values </returns>
		public virtual double MeanSquareError
		{
			get
			{
				return this.globalFitInfo[MSE_IDX];
			}
		}

		/// <summary>
		/// <p>Returns the <a href="http://www.xycoon.com/coefficient1.htm">
		/// coefficient of multiple determination</a>,
		/// usually denoted r-square.</p>
		/// 
		/// <p><strong>Preconditions</strong>: <ul>
		/// <li>At least numberOfParameters observations (with at least numberOfParameters different x values)
		/// must have been added before invoking this method. If this method is
		/// invoked before a model can be estimated, {@code Double,NaN} is
		/// returned.
		/// </li></ul></p>
		/// </summary>
		/// <returns> r-square, a double in the interval [0, 1] </returns>
		public virtual double RSquared
		{
			get
			{
				return this.globalFitInfo[RSQ_IDX];
			}
		}

		/// <summary>
		/// <p>Returns the adjusted R-squared statistic, defined by the formula <pre>
		/// R<sup>2</sup><sub>adj</sub> = 1 - [SSR (n - 1)] / [SSTO (n - p)]
		/// </pre>
		/// where SSR is the sum of squared residuals},
		/// SSTO is the total sum of squares}, n is the number
		/// of observations and p is the number of parameters estimated (including the intercept).</p>
		/// 
		/// <p>If the regression is estimated without an intercept term, what is returned is <pre>
		/// <code> 1 - (1 - <seealso cref="#getRSquared()"/> ) * (n / (n - p)) </code>
		/// </pre></p>
		/// </summary>
		/// <returns> adjusted R-Squared statistic </returns>
		public virtual double AdjustedRSquared
		{
			get
			{
				return this.globalFitInfo[ADJRSQ_IDX];
			}
		}

		/// <summary>
		/// Returns true if the regression model has been computed including an intercept.
		/// In this case, the coefficient of the intercept is the first element of the
		/// <seealso cref="#getParameterEstimates() parameter estimates"/>. </summary>
		/// <returns> true if the model has an intercept term </returns>
		public virtual bool hasIntercept()
		{
			return this.containsConstant;
		}

		/// <summary>
		/// Gets the i-jth element of the variance-covariance matrix.
		/// </summary>
		/// <param name="i"> first variable index </param>
		/// <param name="j"> second variable index </param>
		/// <returns> the requested variance-covariance matrix entry </returns>
		private double getVcvElement(int i, int j)
		{
			if (this.isSymmetricVCD)
			{
				if (this.varCovData.Length > 1)
				{
					//could be stored in upper or lower triangular
					if (i == j)
					{
						return varCovData[i][i];
					}
					else if (i >= varCovData[j].Length)
					{
						return varCovData[i][j];
					}
					else
					{
						return varCovData[j][i];
					}
				} //could be in single array
				else
				{
					if (i > j)
					{
						return varCovData[0][(i + 1) * i / 2 + j];
					}
					else
					{
						return varCovData[0][(j + 1) * j / 2 + i];
					}
				}
			}
			else
			{
				return this.varCovData[i][j];
			}
		}
	}

}
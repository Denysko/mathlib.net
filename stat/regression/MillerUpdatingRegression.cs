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

	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using FastMath = mathlib.util.FastMath;
	using Precision = mathlib.util.Precision;
	using MathArrays = mathlib.util.MathArrays;

	/// <summary>
	/// This class is a concrete implementation of the <seealso cref="UpdatingMultipleLinearRegression"/> interface.
	/// 
	/// <p>The algorithm is described in: <pre>
	/// Algorithm AS 274: Least Squares Routines to Supplement Those of Gentleman
	/// Author(s): Alan J. Miller
	/// Source: Journal of the Royal Statistical Society.
	/// Series C (Applied Statistics), Vol. 41, No. 2
	/// (1992), pp. 458-478
	/// Published by: Blackwell Publishing for the Royal Statistical Society
	/// Stable URL: http://www.jstor.org/stable/2347583 </pre></p>
	/// 
	/// <p>This method for multiple regression forms the solution to the OLS problem
	/// by updating the QR decomposition as described by Gentleman.</p>
	/// 
	/// @version $Id: MillerUpdatingRegression.java 1547633 2013-12-03 23:03:06Z tn $
	/// @since 3.0
	/// </summary>
	public class MillerUpdatingRegression : UpdatingMultipleLinearRegression
	{

		/// <summary>
		/// number of variables in regression </summary>
		private readonly int nvars;
		/// <summary>
		/// diagonals of cross products matrix </summary>
		private readonly double[] d;
		/// <summary>
		/// the elements of the R`Y </summary>
		private readonly double[] rhs;
		/// <summary>
		/// the off diagonal portion of the R matrix </summary>
		private readonly double[] r;
		/// <summary>
		/// the tolerance for each of the variables </summary>
		private readonly double[] tol;
		/// <summary>
		/// residual sum of squares for all nested regressions </summary>
		private readonly double[] rss;
		/// <summary>
		/// order of the regressors </summary>
		private readonly int[] vorder;
		/// <summary>
		/// scratch space for tolerance calc </summary>
		private readonly double[] work_tolset;
		/// <summary>
		/// number of observations entered </summary>
		private long nobs = 0;
		/// <summary>
		/// sum of squared errors of largest regression </summary>
		private double sserr = 0.0;
		/// <summary>
		/// has rss been called? </summary>
		private bool rss_set = false;
		/// <summary>
		/// has the tolerance setting method been called </summary>
		private bool tol_set = false;
		/// <summary>
		/// flags for variables with linear dependency problems </summary>
		private readonly bool[] lindep;
		/// <summary>
		/// singular x values </summary>
		private readonly double[] x_sing;
		/// <summary>
		/// workspace for singularity method </summary>
		private readonly double[] work_sing;
		/// <summary>
		/// summation of Y variable </summary>
		private double sumy = 0.0;
		/// <summary>
		/// summation of squared Y values </summary>
		private double sumsqy = 0.0;
		/// <summary>
		/// boolean flag whether a regression constant is added </summary>
		private bool hasIntercept_Renamed;
		/// <summary>
		/// zero tolerance </summary>
		private readonly double epsilon;
		/// <summary>
		///  Set the default constructor to private access
		///  to prevent inadvertent instantiation
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") private MillerUpdatingRegression()
		private MillerUpdatingRegression() : this(-1, false, double.NaN)
		{
		}

		/// <summary>
		/// This is the augmented constructor for the MillerUpdatingRegression class.
		/// </summary>
		/// <param name="numberOfVariables"> number of regressors to expect, not including constant </param>
		/// <param name="includeConstant"> include a constant automatically </param>
		/// <param name="errorTolerance">  zero tolerance, how machine zero is determined </param>
		/// <exception cref="ModelSpecificationException"> if {@code numberOfVariables is less than 1} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MillerUpdatingRegression(int numberOfVariables, boolean includeConstant, double errorTolerance) throws ModelSpecificationException
		public MillerUpdatingRegression(int numberOfVariables, bool includeConstant, double errorTolerance)
		{
			if (numberOfVariables < 1)
			{
				throw new ModelSpecificationException(LocalizedFormats.NO_REGRESSORS);
			}
			if (includeConstant)
			{
				this.nvars = numberOfVariables + 1;
			}
			else
			{
				this.nvars = numberOfVariables;
			}
			this.hasIntercept_Renamed = includeConstant;
			this.nobs = 0;
			this.d = new double[this.nvars];
			this.rhs = new double[this.nvars];
			this.r = new double[this.nvars * (this.nvars - 1) / 2];
			this.tol = new double[this.nvars];
			this.rss = new double[this.nvars];
			this.vorder = new int[this.nvars];
			this.x_sing = new double[this.nvars];
			this.work_sing = new double[this.nvars];
			this.work_tolset = new double[this.nvars];
			this.lindep = new bool[this.nvars];
			for (int i = 0; i < this.nvars; i++)
			{
				vorder[i] = i;
			}
			if (errorTolerance > 0)
			{
				this.epsilon = errorTolerance;
			}
			else
			{
				this.epsilon = -errorTolerance;
			}
		}

		/// <summary>
		/// Primary constructor for the MillerUpdatingRegression.
		/// </summary>
		/// <param name="numberOfVariables"> maximum number of potential regressors </param>
		/// <param name="includeConstant"> include a constant automatically </param>
		/// <exception cref="ModelSpecificationException"> if {@code numberOfVariables is less than 1} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MillerUpdatingRegression(int numberOfVariables, boolean includeConstant) throws ModelSpecificationException
		public MillerUpdatingRegression(int numberOfVariables, bool includeConstant) : this(numberOfVariables, includeConstant, Precision.EPSILON)
		{
		}

		/// <summary>
		/// A getter method which determines whether a constant is included. </summary>
		/// <returns> true regression has an intercept, false no intercept </returns>
		public virtual bool hasIntercept()
		{
			return this.hasIntercept_Renamed;
		}

		/// <summary>
		/// Gets the number of observations added to the regression model. </summary>
		/// <returns> number of observations </returns>
		public virtual long N
		{
			get
			{
				return this.nobs;
			}
		}

		/// <summary>
		/// Adds an observation to the regression model. </summary>
		/// <param name="x"> the array with regressor values </param>
		/// <param name="y">  the value of dependent variable given these regressors </param>
		/// <exception cref="ModelSpecificationException"> if the length of {@code x} does not equal
		/// the number of independent variables in the model </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addObservation(final double[] x, final double y) throws ModelSpecificationException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void addObservation(double[] x, double y)
		{

			if ((!this.hasIntercept_Renamed && x.Length != nvars) || (this.hasIntercept_Renamed && x.Length + 1 != nvars))
			{
				throw new ModelSpecificationException(LocalizedFormats.INVALID_REGRESSION_OBSERVATION, x.Length, nvars);
			}
			if (!this.hasIntercept_Renamed)
			{
				include(MathArrays.copyOf(x, x.Length), 1.0, y);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tmp = new double[x.length + 1];
				double[] tmp = new double[x.Length + 1];
				Array.Copy(x, 0, tmp, 1, x.Length);
				tmp[0] = 1.0;
				include(tmp, 1.0, y);
			}
			++nobs;

		}

		/// <summary>
		/// Adds multiple observations to the model. </summary>
		/// <param name="x"> observations on the regressors </param>
		/// <param name="y"> observations on the regressand </param>
		/// <exception cref="ModelSpecificationException"> if {@code x} is not rectangular, does not match
		/// the length of {@code y} or does not contain sufficient data to estimate the model </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addObservations(double[][] x, double[] y) throws ModelSpecificationException
		public virtual void addObservations(double[][] x, double[] y)
		{
			if ((x == null) || (y == null) || (x.Length != y.Length))
			{
				throw new ModelSpecificationException(LocalizedFormats.DIMENSIONS_MISMATCH_SIMPLE, (x == null) ? 0 : x.Length, (y == null) ? 0 : y.Length);
			}
			if (x.Length == 0) // Must be no y data either
			{
				throw new ModelSpecificationException(LocalizedFormats.NO_DATA);
			}
			if (x[0].Length + 1 > x.Length)
			{
				throw new ModelSpecificationException(LocalizedFormats.NOT_ENOUGH_DATA_FOR_NUMBER_OF_PREDICTORS, x.Length, x[0].Length);
			}
			for (int i = 0; i < x.Length; i++)
			{
				addObservation(x[i], y[i]);
			}
		}

		/// <summary>
		/// The include method is where the QR decomposition occurs. This statement forms all
		/// intermediate data which will be used for all derivative measures.
		/// According to the miller paper, note that in the original implementation the x vector
		/// is overwritten. In this implementation, the include method is passed a copy of the
		/// original data vector so that there is no contamination of the data. Additionally,
		/// this method differs slightly from Gentleman's method, in that the assumption is
		/// of dense design matrices, there is some advantage in using the original gentleman algorithm
		/// on sparse matrices.
		/// </summary>
		/// <param name="x"> observations on the regressors </param>
		/// <param name="wi"> weight of the this observation (-1,1) </param>
		/// <param name="yi"> observation on the regressand </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void include(final double[] x, final double wi, final double yi)
		private void include(double[] x, double wi, double yi)
		{
			int nextr = 0;
			double w = wi;
			double y = yi;
			double xi;
			double di;
			double wxi;
			double dpi;
			double xk;
			double _w;
			this.rss_set = false;
			sumy = smartAdd(yi, sumy);
			sumsqy = smartAdd(sumsqy, yi * yi);
			for (int i = 0; i < x.Length; i++)
			{
				if (w == 0.0)
				{
					return;
				}
				xi = x[i];

				if (xi == 0.0)
				{
					nextr += nvars - i - 1;
					continue;
				}
				di = d[i];
				wxi = w * xi;
				_w = w;
				if (di != 0.0)
				{
					dpi = smartAdd(di, wxi * xi);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tmp = wxi * xi / di;
					double tmp = wxi * xi / di;
					if (FastMath.abs(tmp) > Precision.EPSILON)
					{
						w = (di * w) / dpi;
					}
				}
				else
				{
					dpi = wxi * xi;
					w = 0.0;
				}
				d[i] = dpi;
				for (int k = i + 1; k < nvars; k++)
				{
					xk = x[k];
					x[k] = smartAdd(xk, -xi * r[nextr]);
					if (di != 0.0)
					{
						r[nextr] = smartAdd(di * r[nextr], (_w * xi) * xk) / dpi;
					}
					else
					{
						r[nextr] = xk / xi;
					}
					++nextr;
				}
				xk = y;
				y = smartAdd(xk, -xi * rhs[i]);
				if (di != 0.0)
				{
					rhs[i] = smartAdd(di * rhs[i], wxi * xk) / dpi;
				}
				else
				{
					rhs[i] = xk / xi;
				}
			}
			sserr = smartAdd(sserr, w * y * y);
		}

		/// <summary>
		/// Adds to number a and b such that the contamination due to
		/// numerical smallness of one addend does not corrupt the sum. </summary>
		/// <param name="a"> - an addend </param>
		/// <param name="b"> - an addend </param>
		/// <returns> the sum of the a and b </returns>
		private double smartAdd(double a, double b)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double _a = mathlib.util.FastMath.abs(a);
			double _a = FastMath.abs(a);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double _b = mathlib.util.FastMath.abs(b);
			double _b = FastMath.abs(b);
			if (_a > _b)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double eps = _a * mathlib.util.Precision.EPSILON;
				double eps = _a * Precision.EPSILON;
				if (_b > eps)
				{
					return a + b;
				}
				return a;
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double eps = _b * mathlib.util.Precision.EPSILON;
				double eps = _b * Precision.EPSILON;
				if (_a > eps)
				{
					return a + b;
				}
				return b;
			}
		}

		/// <summary>
		/// As the name suggests,  clear wipes the internals and reorders everything in the
		/// canonical order.
		/// </summary>
		public virtual void clear()
		{
			Arrays.fill(this.d, 0.0);
			Arrays.fill(this.rhs, 0.0);
			Arrays.fill(this.r, 0.0);
			Arrays.fill(this.tol, 0.0);
			Arrays.fill(this.rss, 0.0);
			Arrays.fill(this.work_tolset, 0.0);
			Arrays.fill(this.work_sing, 0.0);
			Arrays.fill(this.x_sing, 0.0);
			Arrays.fill(this.lindep, false);
			for (int i = 0; i < nvars; i++)
			{
				this.vorder[i] = i;
			}
			this.nobs = 0;
			this.sserr = 0.0;
			this.sumy = 0.0;
			this.sumsqy = 0.0;
			this.rss_set = false;
			this.tol_set = false;
		}

		/// <summary>
		/// This sets up tolerances for singularity testing.
		/// </summary>
		private void tolset()
		{
			int pos;
			double total;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double eps = this.epsilon;
			double eps = this.epsilon;
			for (int i = 0; i < nvars; i++)
			{
				this.work_tolset[i] = FastMath.sqrt(d[i]);
			}
			tol[0] = eps * this.work_tolset[0];
			for (int col = 1; col < nvars; col++)
			{
				pos = col - 1;
				total = work_tolset[col];
				for (int row = 0; row < col; row++)
				{
					total += FastMath.abs(r[pos]) * work_tolset[row];
					pos += nvars - row - 2;
				}
				tol[col] = eps * total;
			}
			tol_set = true;
		}

		/// <summary>
		/// The regcf method conducts the linear regression and extracts the
		/// parameter vector. Notice that the algorithm can do subset regression
		/// with no alteration.
		/// </summary>
		/// <param name="nreq"> how many of the regressors to include (either in canonical
		/// order, or in the current reordered state) </param>
		/// <returns> an array with the estimated slope coefficients </returns>
		/// <exception cref="ModelSpecificationException"> if {@code nreq} is less than 1
		/// or greater than the number of independent variables </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private double[] regcf(int nreq) throws ModelSpecificationException
		private double[] regcf(int nreq)
		{
			int nextr;
			if (nreq < 1)
			{
				throw new ModelSpecificationException(LocalizedFormats.NO_REGRESSORS);
			}
			if (nreq > this.nvars)
			{
				throw new ModelSpecificationException(LocalizedFormats.TOO_MANY_REGRESSORS, nreq, this.nvars);
			}
			if (!this.tol_set)
			{
				tolset();
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] ret = new double[nreq];
			double[] ret = new double[nreq];
			bool rankProblem = false;
			for (int i = nreq - 1; i > -1; i--)
			{
				if (FastMath.sqrt(d[i]) < tol[i])
				{
					ret[i] = 0.0;
					d[i] = 0.0;
					rankProblem = true;
				}
				else
				{
					ret[i] = rhs[i];
					nextr = i * (nvars + nvars - i - 1) / 2;
					for (int j = i + 1; j < nreq; j++)
					{
						ret[i] = smartAdd(ret[i], -r[nextr] * ret[j]);
						++nextr;
					}
				}
			}
			if (rankProblem)
			{
				for (int i = 0; i < nreq; i++)
				{
					if (this.lindep[i])
					{
						ret[i] = double.NaN;
					}
				}
			}
			return ret;
		}

		/// <summary>
		/// The method which checks for singularities and then eliminates the offending
		/// columns.
		/// </summary>
		private void singcheck()
		{
			int pos;
			for (int i = 0; i < nvars; i++)
			{
				work_sing[i] = FastMath.sqrt(d[i]);
			}
			for (int col = 0; col < nvars; col++)
			{
				// Set elements within R to zero if they are less than tol(col) in
				// absolute value after being scaled by the square root of their row
				// multiplier
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double temp = tol[col];
				double temp = tol[col];
				pos = col - 1;
				for (int row = 0; row < col - 1; row++)
				{
					if (FastMath.abs(r[pos]) * work_sing[row] < temp)
					{
						r[pos] = 0.0;
					}
					pos += nvars - row - 2;
				}
				// If diagonal element is near zero, set it to zero, set appropriate
				// element of LINDEP, and use INCLUD to augment the projections in
				// the lower rows of the orthogonalization.
				lindep[col] = false;
				if (work_sing[col] < temp)
				{
					lindep[col] = true;
					if (col < nvars - 1)
					{
						Arrays.fill(x_sing, 0.0);
						int _pi = col * (nvars + nvars - col - 1) / 2;
						for (int _xi = col + 1; _xi < nvars; _xi++, _pi++)
						{
							x_sing[_xi] = r[_pi];
							r[_pi] = 0.0;
						}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = rhs[col];
						double y = rhs[col];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double weight = d[col];
						double weight = d[col];
						d[col] = 0.0;
						rhs[col] = 0.0;
						this.include(x_sing, weight, y);
					}
					else
					{
						sserr += d[col] * rhs[col] * rhs[col];
					}
				}
			}
		}

		/// <summary>
		/// Calculates the sum of squared errors for the full regression
		/// and all subsets in the following manner: <pre>
		/// rss[] ={
		/// ResidualSumOfSquares_allNvars,
		/// ResidualSumOfSquares_FirstNvars-1,
		/// ResidualSumOfSquares_FirstNvars-2,
		/// ..., ResidualSumOfSquares_FirstVariable} </pre>
		/// </summary>
		private void ss()
		{
			double total = sserr;
			rss[nvars - 1] = sserr;
			for (int i = nvars - 1; i > 0; i--)
			{
				total += d[i] * rhs[i] * rhs[i];
				rss[i - 1] = total;
			}
			rss_set = true;
		}

		/// <summary>
		/// Calculates the cov matrix assuming only the first nreq variables are
		/// included in the calculation. The returned array contains a symmetric
		/// matrix stored in lower triangular form. The matrix will have
		/// ( nreq + 1 ) * nreq / 2 elements. For illustration <pre>
		/// cov =
		/// {
		///  cov_00,
		///  cov_10, cov_11,
		///  cov_20, cov_21, cov22,
		///  ...
		/// } </pre>
		/// </summary>
		/// <param name="nreq"> how many of the regressors to include (either in canonical
		/// order, or in the current reordered state) </param>
		/// <returns> an array with the variance covariance of the included
		/// regressors in lower triangular form </returns>
		private double[] cov(int nreq)
		{
			if (this.nobs <= nreq)
			{
				return null;
			}
			double rnk = 0.0;
			for (int i = 0; i < nreq; i++)
			{
				if (!this.lindep[i])
				{
					rnk += 1.0;
				}
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double var = rss[nreq - 1] / (nobs - rnk);
			double @var = rss[nreq - 1] / (nobs - rnk);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] rinv = new double[nreq * (nreq - 1) / 2];
			double[] rinv = new double[nreq * (nreq - 1) / 2];
			inverse(rinv, nreq);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] covmat = new double[nreq * (nreq + 1) / 2];
			double[] covmat = new double[nreq * (nreq + 1) / 2];
			Arrays.fill(covmat, double.NaN);
			int pos2;
			int pos1;
			int start = 0;
			double total = 0;
			for (int row = 0; row < nreq; row++)
			{
				pos2 = start;
				if (!this.lindep[row])
				{
					for (int col = row; col < nreq; col++)
					{
						if (!this.lindep[col])
						{
							pos1 = start + col - row;
							if (row == col)
							{
								total = 1.0 / d[col];
							}
							else
							{
								total = rinv[pos1 - 1] / d[col];
							}
							for (int k = col + 1; k < nreq; k++)
							{
								if (!this.lindep[k])
								{
									total += rinv[pos1] * rinv[pos2] / d[k];
								}
								++pos1;
								++pos2;
							}
							covmat[(col + 1) * col / 2 + row] = total * @var;
						}
						else
						{
							pos2 += nreq - col - 1;
						}
					}
				}
				start += nreq - row - 1;
			}
			return covmat;
		}

		/// <summary>
		/// This internal method calculates the inverse of the upper-triangular portion
		/// of the R matrix. </summary>
		/// <param name="rinv">  the storage for the inverse of r </param>
		/// <param name="nreq"> how many of the regressors to include (either in canonical
		/// order, or in the current reordered state) </param>
		private void inverse(double[] rinv, int nreq)
		{
			int pos = nreq * (nreq - 1) / 2 - 1;
			int pos1 = -1;
			int pos2 = -1;
			double total = 0.0;
			Arrays.fill(rinv, double.NaN);
			for (int row = nreq - 1; row > 0; --row)
			{
				if (!this.lindep[row])
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int start = (row - 1) * (nvars + nvars - row) / 2;
					int start = (row - 1) * (nvars + nvars - row) / 2;
					for (int col = nreq; col > row; --col)
					{
						pos1 = start;
						pos2 = pos;
						total = 0.0;
						for (int k = row; k < col - 1; k++)
						{
							pos2 += nreq - k - 1;
							if (!this.lindep[k])
							{
								total += -r[pos1] * rinv[pos2];
							}
							++pos1;
						}
						rinv[pos] = total - r[pos1];
						--pos;
					}
				}
				else
				{
					pos -= nreq - row;
				}
			}
		}

		/// <summary>
		/// In the original algorithm only the partial correlations of the regressors
		/// is returned to the user. In this implementation, we have <pre>
		/// corr =
		/// {
		///   corrxx - lower triangular
		///   corrxy - bottom row of the matrix
		/// }
		/// Replaces subroutines PCORR and COR of:
		/// ALGORITHM AS274  APPL. STATIST. (1992) VOL.41, NO. 2 </pre>
		/// 
		/// <p>Calculate partial correlations after the variables in rows
		/// 1, 2, ..., IN have been forced into the regression.
		/// If IN = 1, and the first row of R represents a constant in the
		/// model, then the usual simple correlations are returned.</p>
		/// 
		/// <p>If IN = 0, the value returned in array CORMAT for the correlation
		/// of variables Xi & Xj is: <pre>
		/// sum ( Xi.Xj ) / Sqrt ( sum (Xi^2) . sum (Xj^2) )</pre></p>
		/// 
		/// <p>On return, array CORMAT contains the upper triangle of the matrix of
		/// partial correlations stored by rows, excluding the 1's on the diagonal.
		/// e.g. if IN = 2, the consecutive elements returned are:
		/// (3,4) (3,5) ... (3,ncol), (4,5) (4,6) ... (4,ncol), etc.
		/// Array YCORR stores the partial correlations with the Y-variable
		/// starting with YCORR(IN+1) = partial correlation with the variable in
		/// position (IN+1). </p>
		/// </summary>
		/// <param name="in"> how many of the regressors to include (either in canonical
		/// order, or in the current reordered state) </param>
		/// <returns> an array with the partial correlations of the remainder of
		/// regressors with each other and the regressand, in lower triangular form </returns>
		public virtual double[] getPartialCorrelations(int @in)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] output = new double[(nvars - in + 1) * (nvars - in) / 2];
			double[] output = new double[(nvars - @in + 1) * (nvars - @in) / 2];
			int pos;
			int pos1;
			int pos2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rms_off = -in;
			int rms_off = -@in;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int wrk_off = -(in + 1);
			int wrk_off = -(@in + 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] rms = new double[nvars - in];
			double[] rms = new double[nvars - @in];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] work = new double[nvars - in - 1];
			double[] work = new double[nvars - @in - 1];
			double sumxx;
			double sumxy;
			double sumyy;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offXX = (nvars - in) * (nvars - in - 1) / 2;
			int offXX = (nvars - @in) * (nvars - @in - 1) / 2;
			if (@in < -1 || @in >= nvars)
			{
				return null;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nvm = nvars - 1;
			int nvm = nvars - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int base_pos = r.length - (nvm - in) * (nvm - in + 1) / 2;
			int base_pos = r.Length - (nvm - @in) * (nvm - @in + 1) / 2;
			if (d[@in] > 0.0)
			{
				rms[@in + rms_off] = 1.0 / FastMath.sqrt(d[@in]);
			}
			for (int col = @in + 1; col < nvars; col++)
			{
				pos = base_pos + col - 1 - @in;
				sumxx = d[col];
				for (int row = @in; row < col; row++)
				{
					sumxx += d[row] * r[pos] * r[pos];
					pos += nvars - row - 2;
				}
				if (sumxx > 0.0)
				{
					rms[col + rms_off] = 1.0 / FastMath.sqrt(sumxx);
				}
				else
				{
					rms[col + rms_off] = 0.0;
				}
			}
			sumyy = sserr;
			for (int row = @in; row < nvars; row++)
			{
				sumyy += d[row] * rhs[row] * rhs[row];
			}
			if (sumyy > 0.0)
			{
				sumyy = 1.0 / FastMath.sqrt(sumyy);
			}
			pos = 0;
			for (int col1 = @in; col1 < nvars; col1++)
			{
				sumxy = 0.0;
				Arrays.fill(work, 0.0);
				pos1 = base_pos + col1 - @in - 1;
				for (int row = @in; row < col1; row++)
				{
					pos2 = pos1 + 1;
					for (int col2 = col1 + 1; col2 < nvars; col2++)
					{
						work[col2 + wrk_off] += d[row] * r[pos1] * r[pos2];
						pos2++;
					}
					sumxy += d[row] * r[pos1] * rhs[row];
					pos1 += nvars - row - 2;
				}
				pos2 = pos1 + 1;
				for (int col2 = col1 + 1; col2 < nvars; col2++)
				{
					work[col2 + wrk_off] += d[col1] * r[pos2];
					++pos2;
					output[(col2 - 1 - @in) * (col2 - @in) / 2 + col1 - @in] = work[col2 + wrk_off] * rms[col1 + rms_off] * rms[col2 + rms_off];
					++pos;
				}
				sumxy += d[col1] * rhs[col1];
				output[col1 + rms_off + offXX] = sumxy * rms[col1 + rms_off] * sumyy;
			}

			return output;
		}

		/// <summary>
		/// ALGORITHM AS274 APPL. STATIST. (1992) VOL.41, NO. 2.
		/// Move variable from position FROM to position TO in an
		/// orthogonal reduction produced by AS75.1.
		/// </summary>
		/// <param name="from"> initial position </param>
		/// <param name="to"> destination </param>
		private void vmove(int from, int to)
		{
			double d1;
			double d2;
			double X;
			double d1new;
			double d2new;
			double cbar;
			double sbar;
			double Y;
			int first;
			int inc;
			int m1;
			int m2;
			int mp1;
			int pos;
			bool bSkipTo40 = false;
			if (from == to)
			{
				return;
			}
			if (!this.rss_set)
			{
				ss();
			}
			int count = 0;
			if (from < to)
			{
				first = from;
				inc = 1;
				count = to - from;
			}
			else
			{
				first = from - 1;
				inc = -1;
				count = from - to;
			}

			int m = first;
			int idx = 0;
			while (idx < count)
			{
				m1 = m * (nvars + nvars - m - 1) / 2;
				m2 = m1 + nvars - m - 1;
				mp1 = m + 1;

				d1 = d[m];
				d2 = d[mp1];
				// Special cases.
				if (d1 > this.epsilon || d2 > this.epsilon)
				{
					X = r[m1];
					if (FastMath.abs(X) * FastMath.sqrt(d1) < tol[mp1])
					{
						X = 0.0;
					}
					if (d1 < this.epsilon || FastMath.abs(X) < this.epsilon)
					{
						d[m] = d2;
						d[mp1] = d1;
						r[m1] = 0.0;
						for (int col = m + 2; col < nvars; col++)
						{
							++m1;
							X = r[m1];
							r[m1] = r[m2];
							r[m2] = X;
							++m2;
						}
						X = rhs[m];
						rhs[m] = rhs[mp1];
						rhs[mp1] = X;
						bSkipTo40 = true;
						//break;
					}
					else if (d2 < this.epsilon)
					{
						d[m] = d1 * X * X;
						r[m1] = 1.0 / X;
						for (int _i = m1 + 1; _i < m1 + nvars - m - 1; _i++)
						{
							r[_i] /= X;
						}
						rhs[m] /= X;
						bSkipTo40 = true;
						//break;
					}
					if (!bSkipTo40)
					{
						d1new = d2 + d1 * X * X;
						cbar = d2 / d1new;
						sbar = X * d1 / d1new;
						d2new = d1 * cbar;
						d[m] = d1new;
						d[mp1] = d2new;
						r[m1] = sbar;
						for (int col = m + 2; col < nvars; col++)
						{
							++m1;
							Y = r[m1];
							r[m1] = cbar * r[m2] + sbar * Y;
							r[m2] = Y - X * r[m2];
							++m2;
						}
						Y = rhs[m];
						rhs[m] = cbar * rhs[mp1] + sbar * Y;
						rhs[mp1] = Y - X * rhs[mp1];
					}
				}
				if (m > 0)
				{
					pos = m;
					for (int row = 0; row < m; row++)
					{
						X = r[pos];
						r[pos] = r[pos - 1];
						r[pos - 1] = X;
						pos += nvars - row - 2;
					}
				}
				// Adjust variable order (VORDER), the tolerances (TOL) and
				// the vector of residual sums of squares (RSS).
				m1 = vorder[m];
				vorder[m] = vorder[mp1];
				vorder[mp1] = m1;
				X = tol[m];
				tol[m] = tol[mp1];
				tol[mp1] = X;
				rss[m] = rss[mp1] + d[mp1] * rhs[mp1] * rhs[mp1];

				m += inc;
				++idx;
			}
		}

		/// <summary>
		/// ALGORITHM AS274  APPL. STATIST. (1992) VOL.41, NO. 2
		/// 
		/// <p> Re-order the variables in an orthogonal reduction produced by
		/// AS75.1 so that the N variables in LIST start at position POS1,
		/// though will not necessarily be in the same order as in LIST.
		/// Any variables in VORDER before position POS1 are not moved.
		/// Auxiliary routine called: VMOVE. </p>
		/// 
		/// <p>This internal method reorders the regressors.</p>
		/// </summary>
		/// <param name="list"> the regressors to move </param>
		/// <param name="pos1"> where the list will be placed </param>
		/// <returns> -1 error, 0 everything ok </returns>
		private int reorderRegressors(int[] list, int pos1)
		{
			int next;
			int i;
			int l;
			if (list.Length < 1 || list.Length > nvars + 1 - pos1)
			{
				return -1;
			}
			next = pos1;
			i = pos1;
			while (i < nvars)
			{
				l = vorder[i];
				for (int j = 0; j < list.Length; j++)
				{
					if (l == list[j] && i > next)
					{
						this.vmove(i, next);
						++next;
						if (next >= list.Length + pos1)
						{
							return 0;
						}
						else
						{
							break;
						}
					}
				}
				++i;
			}
			return 0;
		}

		/// <summary>
		/// Gets the diagonal of the Hat matrix also known as the leverage matrix.
		/// </summary>
		/// <param name="row_data"> returns the diagonal of the hat matrix for this observation </param>
		/// <returns> the diagonal element of the hatmatrix </returns>
		public virtual double getDiagonalOfHatMatrix(double[] row_data)
		{
			double[] wk = new double[this.nvars];
			int pos;
			double total;

			if (row_data.Length > nvars)
			{
				return double.NaN;
			}
			double[] xrow;
			if (this.hasIntercept_Renamed)
			{
				xrow = new double[row_data.Length + 1];
				xrow[0] = 1.0;
				Array.Copy(row_data, 0, xrow, 1, row_data.Length);
			}
			else
			{
				xrow = row_data;
			}
			double hii = 0.0;
			for (int col = 0; col < xrow.Length; col++)
			{
				if (FastMath.sqrt(d[col]) < tol[col])
				{
					wk[col] = 0.0;
				}
				else
				{
					pos = col - 1;
					total = xrow[col];
					for (int row = 0; row < col; row++)
					{
						total = smartAdd(total, -wk[row] * r[pos]);
						pos += nvars - row - 2;
					}
					wk[col] = total;
					hii = smartAdd(hii, (total * total) / d[col]);
				}
			}
			return hii;
		}

		/// <summary>
		/// Gets the order of the regressors, useful if some type of reordering
		/// has been called. Calling regress with int[]{} args will trigger
		/// a reordering.
		/// </summary>
		/// <returns> int[] with the current order of the regressors </returns>
		public virtual int[] OrderOfRegressors
		{
			get
			{
				return MathArrays.copyOf(vorder);
			}
		}

		/// <summary>
		/// Conducts a regression on the data in the model, using all regressors.
		/// </summary>
		/// <returns> RegressionResults the structure holding all regression results </returns>
		/// <exception cref="ModelSpecificationException"> - thrown if number of observations is
		/// less than the number of variables </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RegressionResults regress() throws ModelSpecificationException
		public virtual RegressionResults regress()
		{
			return regress(this.nvars);
		}

		/// <summary>
		/// Conducts a regression on the data in the model, using a subset of regressors.
		/// </summary>
		/// <param name="numberOfRegressors"> many of the regressors to include (either in canonical
		/// order, or in the current reordered state) </param>
		/// <returns> RegressionResults the structure holding all regression results </returns>
		/// <exception cref="ModelSpecificationException"> - thrown if number of observations is
		/// less than the number of variables or number of regressors requested
		/// is greater than the regressors in the model </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RegressionResults regress(int numberOfRegressors) throws ModelSpecificationException
		public virtual RegressionResults regress(int numberOfRegressors)
		{
			if (this.nobs <= numberOfRegressors)
			{
			   throw new ModelSpecificationException(LocalizedFormats.NOT_ENOUGH_DATA_FOR_NUMBER_OF_PREDICTORS, this.nobs, numberOfRegressors);
			}
			if (numberOfRegressors > this.nvars)
			{
				throw new ModelSpecificationException(LocalizedFormats.TOO_MANY_REGRESSORS, numberOfRegressors, this.nvars);
			}

			tolset();
			singcheck();

			double[] beta = this.regcf(numberOfRegressors);

			ss();

			double[] cov = this.cov(numberOfRegressors);

			int rnk = 0;
			for (int i = 0; i < this.lindep.Length; i++)
			{
				if (!this.lindep[i])
				{
					++rnk;
				}
			}

			bool needsReorder = false;
			for (int i = 0; i < numberOfRegressors; i++)
			{
				if (this.vorder[i] != i)
				{
					needsReorder = true;
					break;
				}
			}
			if (!needsReorder)
			{
				return new RegressionResults(beta, new double[][]{cov}, true, this.nobs, rnk, this.sumy, this.sumsqy, this.sserr, this.hasIntercept_Renamed, false);
			}
			else
			{
				double[] betaNew = new double[beta.Length];
				double[] covNew = new double[cov.Length];

				int[] newIndices = new int[beta.Length];
				for (int i = 0; i < nvars; i++)
				{
					for (int j = 0; j < numberOfRegressors; j++)
					{
						if (this.vorder[j] == i)
						{
							betaNew[i] = beta[j];
							newIndices[i] = j;
						}
					}
				}

				int idx1 = 0;
				int idx2;
				int _i;
				int _j;
				for (int i = 0; i < beta.Length; i++)
				{
					_i = newIndices[i];
					for (int j = 0; j <= i; j++, idx1++)
					{
						_j = newIndices[j];
						if (_i > _j)
						{
							idx2 = _i * (_i + 1) / 2 + _j;
						}
						else
						{
							idx2 = _j * (_j + 1) / 2 + _i;
						}
						covNew[idx1] = cov[idx2];
					}
				}
				return new RegressionResults(betaNew, new double[][]{covNew}, true, this.nobs, rnk, this.sumy, this.sumsqy, this.sserr, this.hasIntercept_Renamed, false);
			}
		}

		/// <summary>
		/// Conducts a regression on the data in the model, using regressors in array
		/// Calling this method will change the internal order of the regressors
		/// and care is required in interpreting the hatmatrix.
		/// </summary>
		/// <param name="variablesToInclude"> array of variables to include in regression </param>
		/// <returns> RegressionResults the structure holding all regression results </returns>
		/// <exception cref="ModelSpecificationException"> - thrown if number of observations is
		/// less than the number of variables, the number of regressors requested
		/// is greater than the regressors in the model or a regressor index in
		/// regressor array does not exist </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RegressionResults regress(int[] variablesToInclude) throws ModelSpecificationException
		public virtual RegressionResults regress(int[] variablesToInclude)
		{
			if (variablesToInclude.Length > this.nvars)
			{
				throw new ModelSpecificationException(LocalizedFormats.TOO_MANY_REGRESSORS, variablesToInclude.Length, this.nvars);
			}
			if (this.nobs <= this.nvars)
			{
				throw new ModelSpecificationException(LocalizedFormats.NOT_ENOUGH_DATA_FOR_NUMBER_OF_PREDICTORS, this.nobs, this.nvars);
			}
			Arrays.sort(variablesToInclude);
			int iExclude = 0;
			for (int i = 0; i < variablesToInclude.Length; i++)
			{
				if (i >= this.nvars)
				{
					throw new ModelSpecificationException(LocalizedFormats.INDEX_LARGER_THAN_MAX, i, this.nvars);
				}
				if (i > 0 && variablesToInclude[i] == variablesToInclude[i - 1])
				{
					variablesToInclude[i] = -1;
					++iExclude;
				}
			}
			int[] series;
			if (iExclude > 0)
			{
				int j = 0;
				series = new int[variablesToInclude.Length - iExclude];
				for (int i = 0; i < variablesToInclude.Length; i++)
				{
					if (variablesToInclude[i] > -1)
					{
						series[j] = variablesToInclude[i];
						++j;
					}
				}
			}
			else
			{
				series = variablesToInclude;
			}

			reorderRegressors(series, 0);
			tolset();
			singcheck();

			double[] beta = this.regcf(series.Length);

			ss();

			double[] cov = this.cov(series.Length);

			int rnk = 0;
			for (int i = 0; i < this.lindep.Length; i++)
			{
				if (!this.lindep[i])
				{
					++rnk;
				}
			}

			bool needsReorder = false;
			for (int i = 0; i < this.nvars; i++)
			{
				if (this.vorder[i] != series[i])
				{
					needsReorder = true;
					break;
				}
			}
			if (!needsReorder)
			{
				return new RegressionResults(beta, new double[][]{cov}, true, this.nobs, rnk, this.sumy, this.sumsqy, this.sserr, this.hasIntercept_Renamed, false);
			}
			else
			{
				double[] betaNew = new double[beta.Length];
				int[] newIndices = new int[beta.Length];
				for (int i = 0; i < series.Length; i++)
				{
					for (int j = 0; j < this.vorder.Length; j++)
					{
						if (this.vorder[j] == series[i])
						{
							betaNew[i] = beta[j];
							newIndices[i] = j;
						}
					}
				}
				double[] covNew = new double[cov.Length];
				int idx1 = 0;
				int idx2;
				int _i;
				int _j;
				for (int i = 0; i < beta.Length; i++)
				{
					_i = newIndices[i];
					for (int j = 0; j <= i; j++, idx1++)
					{
						_j = newIndices[j];
						if (_i > _j)
						{
							idx2 = _i * (_i + 1) / 2 + _j;
						}
						else
						{
							idx2 = _j * (_j + 1) / 2 + _i;
						}
						covNew[idx1] = cov[idx2];
					}
				}
				return new RegressionResults(betaNew, new double[][]{covNew}, true, this.nobs, rnk, this.sumy, this.sumsqy, this.sserr, this.hasIntercept_Renamed, false);
			}
		}
	}

}
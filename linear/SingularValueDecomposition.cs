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
namespace mathlib.linear
{

	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using FastMath = mathlib.util.FastMath;
	using Precision = mathlib.util.Precision;

	/// <summary>
	/// Calculates the compact Singular Value Decomposition of a matrix.
	/// <p>
	/// The Singular Value Decomposition of matrix A is a set of three matrices: U,
	/// &Sigma; and V such that A = U &times; &Sigma; &times; V<sup>T</sup>. Let A be
	/// a m &times; n matrix, then U is a m &times; p orthogonal matrix, &Sigma; is a
	/// p &times; p diagonal matrix with positive or null elements, V is a p &times;
	/// n orthogonal matrix (hence V<sup>T</sup> is also orthogonal) where
	/// p=min(m,n).
	/// </p>
	/// <p>This class is similar to the class with similar name from the
	/// <a href="http://math.nist.gov/javanumerics/jama/">JAMA</a> library, with the
	/// following changes:</p>
	/// <ul>
	///   <li>the {@code norm2} method which has been renamed as {@link #getNorm()
	///   getNorm},</li>
	///   <li>the {@code cond} method which has been renamed as {@link
	///   #getConditionNumber() getConditionNumber},</li>
	///   <li>the {@code rank} method which has been renamed as {@link #getRank()
	///   getRank},</li>
	///   <li>a <seealso cref="#getUT() getUT"/> method has been added,</li>
	///   <li>a <seealso cref="#getVT() getVT"/> method has been added,</li>
	///   <li>a <seealso cref="#getSolver() getSolver"/> method has been added,</li>
	///   <li>a <seealso cref="#getCovariance(double) getCovariance"/> method has been added.</li>
	/// </ul> </summary>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/SingularValueDecomposition.html">MathWorld</a> </seealso>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Singular_value_decomposition">Wikipedia</a>
	/// @version $Id: SingularValueDecomposition.java 1538368 2013-11-03 13:57:37Z erans $
	/// @since 2.0 (changed to concrete class in 3.0) </seealso>
	public class SingularValueDecomposition
	{
		/// <summary>
		/// Relative threshold for small singular values. </summary>
		private static readonly double EPS = 0x1.0p - 52;
		/// <summary>
		/// Absolute threshold for small singular values. </summary>
		private static readonly double TINY = 0x1.0p - 966;
		/// <summary>
		/// Computed singular values. </summary>
		private readonly double[] singularValues;
		/// <summary>
		/// max(row dimension, column dimension). </summary>
		private readonly int m;
		/// <summary>
		/// min(row dimension, column dimension). </summary>
		private readonly int n;
		/// <summary>
		/// Indicator for transposed matrix. </summary>
		private readonly bool transposed;
		/// <summary>
		/// Cached value of U matrix. </summary>
		private readonly RealMatrix cachedU;
		/// <summary>
		/// Cached value of transposed U matrix. </summary>
		private RealMatrix cachedUt;
		/// <summary>
		/// Cached value of S (diagonal) matrix. </summary>
		private RealMatrix cachedS;
		/// <summary>
		/// Cached value of V matrix. </summary>
		private readonly RealMatrix cachedV;
		/// <summary>
		/// Cached value of transposed V matrix. </summary>
		private RealMatrix cachedVt;
		/// <summary>
		/// Tolerance value for small singular values, calculated once we have
		/// populated "singularValues".
		/// 
		/// </summary>
		private readonly double tol;

		/// <summary>
		/// Calculates the compact Singular Value Decomposition of the given matrix.
		/// </summary>
		/// <param name="matrix"> Matrix to decompose. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SingularValueDecomposition(final RealMatrix matrix)
		public SingularValueDecomposition(RealMatrix matrix)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] A;
			double[][] A;

			 // "m" is always the largest dimension.
			if (matrix.RowDimension < matrix.ColumnDimension)
			{
				transposed = true;
				A = matrix.transpose().Data;
				m = matrix.ColumnDimension;
				n = matrix.RowDimension;
			}
			else
			{
				transposed = false;
				A = matrix.Data;
				m = matrix.RowDimension;
				n = matrix.ColumnDimension;
			}

			singularValues = new double[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] U = new double[m][n];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] U = new double[m][n];
			double[][] U = RectangularArrays.ReturnRectangularDoubleArray(m, n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] V = new double[n][n];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] V = new double[n][n];
			double[][] V = RectangularArrays.ReturnRectangularDoubleArray(n, n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] e = new double[n];
			double[] e = new double[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] work = new double[m];
			double[] work = new double[m];
			// Reduce A to bidiagonal form, storing the diagonal elements
			// in s and the super-diagonal elements in e.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nct = mathlib.util.FastMath.min(m - 1, n);
			int nct = FastMath.min(m - 1, n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nrt = mathlib.util.FastMath.max(0, n - 2);
			int nrt = FastMath.max(0, n - 2);
			for (int k = 0; k < FastMath.max(nct, nrt); k++)
			{
				if (k < nct)
				{
					// Compute the transformation for the k-th column and
					// place the k-th diagonal in s[k].
					// Compute 2-norm of k-th column without under/overflow.
					singularValues[k] = 0;
					for (int i = k; i < m; i++)
					{
						singularValues[k] = FastMath.hypot(singularValues[k], A[i][k]);
					}
					if (singularValues[k] != 0)
					{
						if (A[k][k] < 0)
						{
							singularValues[k] = -singularValues[k];
						}
						for (int i = k; i < m; i++)
						{
							A[i][k] /= singularValues[k];
						}
						A[k][k] += 1;
					}
					singularValues[k] = -singularValues[k];
				}
				for (int j = k + 1; j < n; j++)
				{
					if (k < nct && singularValues[k] != 0)
					{
						// Apply the transformation.
						double t = 0;
						for (int i = k; i < m; i++)
						{
							t += A[i][k] * A[i][j];
						}
						t = -t / A[k][k];
						for (int i = k; i < m; i++)
						{
							A[i][j] += t * A[i][k];
						}
					}
					// Place the k-th row of A into e for the
					// subsequent calculation of the row transformation.
					e[j] = A[k][j];
				}
				if (k < nct)
				{
					// Place the transformation in U for subsequent back
					// multiplication.
					for (int i = k; i < m; i++)
					{
						U[i][k] = A[i][k];
					}
				}
				if (k < nrt)
				{
					// Compute the k-th row transformation and place the
					// k-th super-diagonal in e[k].
					// Compute 2-norm without under/overflow.
					e[k] = 0;
					for (int i = k + 1; i < n; i++)
					{
						e[k] = FastMath.hypot(e[k], e[i]);
					}
					if (e[k] != 0)
					{
						if (e[k + 1] < 0)
						{
							e[k] = -e[k];
						}
						for (int i = k + 1; i < n; i++)
						{
							e[i] /= e[k];
						}
						e[k + 1] += 1;
					}
					e[k] = -e[k];
					if (k + 1 < m && e[k] != 0)
					{
						// Apply the transformation.
						for (int i = k + 1; i < m; i++)
						{
							work[i] = 0;
						}
						for (int j = k + 1; j < n; j++)
						{
							for (int i = k + 1; i < m; i++)
							{
								work[i] += e[j] * A[i][j];
							}
						}
						for (int j = k + 1; j < n; j++)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = -e[j] / e[k + 1];
							double t = -e[j] / e[k + 1];
							for (int i = k + 1; i < m; i++)
							{
								A[i][j] += t * work[i];
							}
						}
					}

					// Place the transformation in V for subsequent
					// back multiplication.
					for (int i = k + 1; i < n; i++)
					{
						V[i][k] = e[i];
					}
				}
			}
			// Set up the final bidiagonal matrix or order p.
			int p = n;
			if (nct < n)
			{
				singularValues[nct] = A[nct][nct];
			}
			if (m < p)
			{
				singularValues[p - 1] = 0;
			}
			if (nrt + 1 < p)
			{
				e[nrt] = A[nrt][p - 1];
			}
			e[p - 1] = 0;

			// Generate U.
			for (int j = nct; j < n; j++)
			{
				for (int i = 0; i < m; i++)
				{
					U[i][j] = 0;
				}
				U[j][j] = 1;
			}
			for (int k = nct - 1; k >= 0; k--)
			{
				if (singularValues[k] != 0)
				{
					for (int j = k + 1; j < n; j++)
					{
						double t = 0;
						for (int i = k; i < m; i++)
						{
							t += U[i][k] * U[i][j];
						}
						t = -t / U[k][k];
						for (int i = k; i < m; i++)
						{
							U[i][j] += t * U[i][k];
						}
					}
					for (int i = k; i < m; i++)
					{
						U[i][k] = -U[i][k];
					}
					U[k][k] = 1 + U[k][k];
					for (int i = 0; i < k - 1; i++)
					{
						U[i][k] = 0;
					}
				}
				else
				{
					for (int i = 0; i < m; i++)
					{
						U[i][k] = 0;
					}
					U[k][k] = 1;
				}
			}

			// Generate V.
			for (int k = n - 1; k >= 0; k--)
			{
				if (k < nrt && e[k] != 0)
				{
					for (int j = k + 1; j < n; j++)
					{
						double t = 0;
						for (int i = k + 1; i < n; i++)
						{
							t += V[i][k] * V[i][j];
						}
						t = -t / V[k + 1][k];
						for (int i = k + 1; i < n; i++)
						{
							V[i][j] += t * V[i][k];
						}
					}
				}
				for (int i = 0; i < n; i++)
				{
					V[i][k] = 0;
				}
				V[k][k] = 1;
			}

			// Main iteration loop for the singular values.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pp = p - 1;
			int pp = p - 1;
			int iter = 0;
			while (p > 0)
			{
				int k;
				int kase;
				// Here is where a test for too many iterations would go.
				// This section of the program inspects for
				// negligible elements in the s and e arrays.  On
				// completion the variables kase and k are set as follows.
				// kase = 1     if s(p) and e[k-1] are negligible and k<p
				// kase = 2     if s(k) is negligible and k<p
				// kase = 3     if e[k-1] is negligible, k<p, and
				//              s(k), ..., s(p) are not negligible (qr step).
				// kase = 4     if e(p-1) is negligible (convergence).
				for (k = p - 2; k >= 0; k--)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double threshold = TINY + EPS * (mathlib.util.FastMath.abs(singularValues[k]) + mathlib.util.FastMath.abs(singularValues[k + 1]));
					double threshold = TINY + EPS * (FastMath.abs(singularValues[k]) + FastMath.abs(singularValues[k + 1]));

					// the following condition is written this way in order
					// to break out of the loop when NaN occurs, writing it
					// as "if (FastMath.abs(e[k]) <= threshold)" would loop
					// indefinitely in case of NaNs because comparison on NaNs
					// always return false, regardless of what is checked
					// see issue MATH-947
					if (!(FastMath.abs(e[k]) > threshold))
					{
						e[k] = 0;
						break;
					}

				}

				if (k == p - 2)
				{
					kase = 4;
				}
				else
				{
					int ks;
					for (ks = p - 1; ks >= k; ks--)
					{
						if (ks == k)
						{
							break;
						}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = (ks != p ? mathlib.util.FastMath.abs(e[ks]) : 0) + (ks != k + 1 ? mathlib.util.FastMath.abs(e[ks - 1]) : 0);
						double t = (ks != p ? FastMath.abs(e[ks]) : 0) + (ks != k + 1 ? FastMath.abs(e[ks - 1]) : 0);
						if (FastMath.abs(singularValues[ks]) <= TINY + EPS * t)
						{
							singularValues[ks] = 0;
							break;
						}
					}
					if (ks == k)
					{
						kase = 3;
					}
					else if (ks == p - 1)
					{
						kase = 1;
					}
					else
					{
						kase = 2;
						k = ks;
					}
				}
				k++;
				// Perform the task indicated by kase.
				switch (kase)
				{
					// Deflate negligible s(p).
					case 1:
					{
						double f = e[p - 2];
						e[p - 2] = 0;
						for (int j = p - 2; j >= k; j--)
						{
							double t = FastMath.hypot(singularValues[j], f);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cs = singularValues[j] / t;
							double cs = singularValues[j] / t;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sn = f / t;
							double sn = f / t;
							singularValues[j] = t;
							if (j != k)
							{
								f = -sn * e[j - 1];
								e[j - 1] = cs * e[j - 1];
							}

							for (int i = 0; i < n; i++)
							{
								t = cs * V[i][j] + sn * V[i][p - 1];
								V[i][p - 1] = -sn * V[i][j] + cs * V[i][p - 1];
								V[i][j] = t;
							}
						}
					}
					break;
					// Split at negligible s(k).
					case 2:
					{
						double f = e[k - 1];
						e[k - 1] = 0;
						for (int j = k; j < p; j++)
						{
							double t = FastMath.hypot(singularValues[j], f);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cs = singularValues[j] / t;
							double cs = singularValues[j] / t;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sn = f / t;
							double sn = f / t;
							singularValues[j] = t;
							f = -sn * e[j];
							e[j] = cs * e[j];

							for (int i = 0; i < m; i++)
							{
								t = cs * U[i][j] + sn * U[i][k - 1];
								U[i][k - 1] = -sn * U[i][j] + cs * U[i][k - 1];
								U[i][j] = t;
							}
						}
					}
					break;
					// Perform one qr step.
					case 3:
					{
						// Calculate the shift.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double maxPm1Pm2 = mathlib.util.FastMath.max(mathlib.util.FastMath.abs(singularValues[p - 1]), mathlib.util.FastMath.abs(singularValues[p - 2]));
						double maxPm1Pm2 = FastMath.max(FastMath.abs(singularValues[p - 1]), FastMath.abs(singularValues[p - 2]));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double scale = mathlib.util.FastMath.max(mathlib.util.FastMath.max(mathlib.util.FastMath.max(maxPm1Pm2, mathlib.util.FastMath.abs(e[p - 2])), mathlib.util.FastMath.abs(singularValues[k])), mathlib.util.FastMath.abs(e[k]));
						double scale = FastMath.max(FastMath.max(FastMath.max(maxPm1Pm2, FastMath.abs(e[p - 2])), FastMath.abs(singularValues[k])), FastMath.abs(e[k]));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sp = singularValues[p - 1] / scale;
						double sp = singularValues[p - 1] / scale;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double spm1 = singularValues[p - 2] / scale;
						double spm1 = singularValues[p - 2] / scale;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double epm1 = e[p - 2] / scale;
						double epm1 = e[p - 2] / scale;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sk = singularValues[k] / scale;
						double sk = singularValues[k] / scale;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ek = e[k] / scale;
						double ek = e[k] / scale;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b = ((spm1 + sp) * (spm1 - sp) + epm1 * epm1) / 2.0;
						double b = ((spm1 + sp) * (spm1 - sp) + epm1 * epm1) / 2.0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c = (sp * epm1) * (sp * epm1);
						double c = (sp * epm1) * (sp * epm1);
						double shift = 0;
						if (b != 0 || c != 0)
						{
							shift = FastMath.sqrt(b * b + c);
							if (b < 0)
							{
								shift = -shift;
							}
							shift = c / (b + shift);
						}
						double f = (sk + sp) * (sk - sp) + shift;
						double g = sk * ek;
						// Chase zeros.
						for (int j = k; j < p - 1; j++)
						{
							double t = FastMath.hypot(f, g);
							double cs = f / t;
							double sn = g / t;
							if (j != k)
							{
								e[j - 1] = t;
							}
							f = cs * singularValues[j] + sn * e[j];
							e[j] = cs * e[j] - sn * singularValues[j];
							g = sn * singularValues[j + 1];
							singularValues[j + 1] = cs * singularValues[j + 1];

							for (int i = 0; i < n; i++)
							{
								t = cs * V[i][j] + sn * V[i][j + 1];
								V[i][j + 1] = -sn * V[i][j] + cs * V[i][j + 1];
								V[i][j] = t;
							}
							t = FastMath.hypot(f, g);
							cs = f / t;
							sn = g / t;
							singularValues[j] = t;
							f = cs * e[j] + sn * singularValues[j + 1];
							singularValues[j + 1] = -sn * e[j] + cs * singularValues[j + 1];
							g = sn * e[j + 1];
							e[j + 1] = cs * e[j + 1];
							if (j < m - 1)
							{
								for (int i = 0; i < m; i++)
								{
									t = cs * U[i][j] + sn * U[i][j + 1];
									U[i][j + 1] = -sn * U[i][j] + cs * U[i][j + 1];
									U[i][j] = t;
								}
							}
						}
						e[p - 2] = f;
						iter++;
					}
					break;
					// Convergence.
					default:
					{
						// Make the singular values positive.
						if (singularValues[k] <= 0)
						{
							singularValues[k] = singularValues[k] < 0 ? - singularValues[k] : 0;

							for (int i = 0; i <= pp; i++)
							{
								V[i][k] = -V[i][k];
							}
						}
						// Order the singular values.
						while (k < pp)
						{
							if (singularValues[k] >= singularValues[k + 1])
							{
								break;
							}
							double t = singularValues[k];
							singularValues[k] = singularValues[k + 1];
							singularValues[k + 1] = t;
							if (k < n - 1)
							{
								for (int i = 0; i < n; i++)
								{
									t = V[i][k + 1];
									V[i][k + 1] = V[i][k];
									V[i][k] = t;
								}
							}
							if (k < m - 1)
							{
								for (int i = 0; i < m; i++)
								{
									t = U[i][k + 1];
									U[i][k + 1] = U[i][k];
									U[i][k] = t;
								}
							}
							k++;
						}
						iter = 0;
						p--;
					}
					break;
				}
			}

			// Set the small value tolerance used to calculate rank and pseudo-inverse
			tol = FastMath.max(m * singularValues[0] * EPS, FastMath.sqrt(Precision.SAFE_MIN));

			if (!transposed)
			{
				cachedU = MatrixUtils.createRealMatrix(U);
				cachedV = MatrixUtils.createRealMatrix(V);
			}
			else
			{
				cachedU = MatrixUtils.createRealMatrix(V);
				cachedV = MatrixUtils.createRealMatrix(U);
			}
		}

		/// <summary>
		/// Returns the matrix U of the decomposition.
		/// <p>U is an orthogonal matrix, i.e. its transpose is also its inverse.</p> </summary>
		/// <returns> the U matrix </returns>
		/// <seealso cref= #getUT() </seealso>
		public virtual RealMatrix U
		{
			get
			{
				// return the cached matrix
				return cachedU;
    
			}
		}

		/// <summary>
		/// Returns the transpose of the matrix U of the decomposition.
		/// <p>U is an orthogonal matrix, i.e. its transpose is also its inverse.</p> </summary>
		/// <returns> the U matrix (or null if decomposed matrix is singular) </returns>
		/// <seealso cref= #getU() </seealso>
		public virtual RealMatrix UT
		{
			get
			{
				if (cachedUt == null)
				{
					cachedUt = U.transpose();
				}
				// return the cached matrix
				return cachedUt;
			}
		}

		/// <summary>
		/// Returns the diagonal matrix &Sigma; of the decomposition.
		/// <p>&Sigma; is a diagonal matrix. The singular values are provided in
		/// non-increasing order, for compatibility with Jama.</p> </summary>
		/// <returns> the &Sigma; matrix </returns>
		public virtual RealMatrix S
		{
			get
			{
				if (cachedS == null)
				{
					// cache the matrix for subsequent calls
					cachedS = MatrixUtils.createRealDiagonalMatrix(singularValues);
				}
				return cachedS;
			}
		}

		/// <summary>
		/// Returns the diagonal elements of the matrix &Sigma; of the decomposition.
		/// <p>The singular values are provided in non-increasing order, for
		/// compatibility with Jama.</p> </summary>
		/// <returns> the diagonal elements of the &Sigma; matrix </returns>
		public virtual double[] SingularValues
		{
			get
			{
				return singularValues.clone();
			}
		}

		/// <summary>
		/// Returns the matrix V of the decomposition.
		/// <p>V is an orthogonal matrix, i.e. its transpose is also its inverse.</p> </summary>
		/// <returns> the V matrix (or null if decomposed matrix is singular) </returns>
		/// <seealso cref= #getVT() </seealso>
		public virtual RealMatrix V
		{
			get
			{
				// return the cached matrix
				return cachedV;
			}
		}

		/// <summary>
		/// Returns the transpose of the matrix V of the decomposition.
		/// <p>V is an orthogonal matrix, i.e. its transpose is also its inverse.</p> </summary>
		/// <returns> the V matrix (or null if decomposed matrix is singular) </returns>
		/// <seealso cref= #getV() </seealso>
		public virtual RealMatrix VT
		{
			get
			{
				if (cachedVt == null)
				{
					cachedVt = V.transpose();
				}
				// return the cached matrix
				return cachedVt;
			}
		}

		/// <summary>
		/// Returns the n &times; n covariance matrix.
		/// <p>The covariance matrix is V &times; J &times; V<sup>T</sup>
		/// where J is the diagonal matrix of the inverse of the squares of
		/// the singular values.</p> </summary>
		/// <param name="minSingularValue"> value below which singular values are ignored
		/// (a 0 or negative value implies all singular value will be used) </param>
		/// <returns> covariance matrix </returns>
		/// <exception cref="IllegalArgumentException"> if minSingularValue is larger than
		/// the largest singular value, meaning all singular values are ignored </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RealMatrix getCovariance(final double minSingularValue)
		public virtual RealMatrix getCovariance(double minSingularValue)
		{
			// get the number of singular values to consider
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int p = singularValues.length;
			int p = singularValues.Length;
			int dimension = 0;
			while (dimension < p && singularValues[dimension] >= minSingularValue)
			{
				++dimension;
			}

			if (dimension == 0)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.TOO_LARGE_CUTOFF_SINGULAR_VALUE, minSingularValue, singularValues[0], true);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] data = new double[dimension][p];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] data = new double[dimension][p];
			double[][] data = RectangularArrays.ReturnRectangularDoubleArray(dimension, p);
			VT.walkInOptimizedOrder(new DefaultRealMatrixPreservingVisitorAnonymousInnerClassHelper(this, data), 0, dimension - 1, 0, p - 1);

			RealMatrix jv = new Array2DRowRealMatrix(data, false);
			return jv.transpose().multiply(jv);
		}

		private class DefaultRealMatrixPreservingVisitorAnonymousInnerClassHelper : DefaultRealMatrixPreservingVisitor
		{
			private readonly SingularValueDecomposition outerInstance;

			private double[][] data;

			public DefaultRealMatrixPreservingVisitorAnonymousInnerClassHelper(SingularValueDecomposition outerInstance, double[][] data)
			{
				this.outerInstance = outerInstance;
				this.data = data;
			}

					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void visit(final int row, final int column, final double value)
			public override void visit(int row, int column, double value)
			{
				data[row][column] = value / outerInstance.singularValues[row];
			}
		}

		/// <summary>
		/// Returns the L<sub>2</sub> norm of the matrix.
		/// <p>The L<sub>2</sub> norm is max(|A &times; u|<sub>2</sub> /
		/// |u|<sub>2</sub>), where |.|<sub>2</sub> denotes the vectorial 2-norm
		/// (i.e. the traditional euclidian norm).</p> </summary>
		/// <returns> norm </returns>
		public virtual double Norm
		{
			get
			{
				return singularValues[0];
			}
		}

		/// <summary>
		/// Return the condition number of the matrix. </summary>
		/// <returns> condition number of the matrix </returns>
		public virtual double ConditionNumber
		{
			get
			{
				return singularValues[0] / singularValues[n - 1];
			}
		}

		/// <summary>
		/// Computes the inverse of the condition number.
		/// In cases of rank deficiency, the {@link #getConditionNumber() condition
		/// number} will become undefined.
		/// </summary>
		/// <returns> the inverse of the condition number. </returns>
		public virtual double InverseConditionNumber
		{
			get
			{
				return singularValues[n - 1] / singularValues[0];
			}
		}

		/// <summary>
		/// Return the effective numerical matrix rank.
		/// <p>The effective numerical rank is the number of non-negligible
		/// singular values. The threshold used to identify non-negligible
		/// terms is max(m,n) &times; ulp(s<sub>1</sub>) where ulp(s<sub>1</sub>)
		/// is the least significant bit of the largest singular value.</p> </summary>
		/// <returns> effective numerical matrix rank </returns>
		public virtual int Rank
		{
			get
			{
				int r = 0;
				for (int i = 0; i < singularValues.Length; i++)
				{
					if (singularValues[i] > tol)
					{
						r++;
					}
				}
				return r;
			}
		}

		/// <summary>
		/// Get a solver for finding the A &times; X = B solution in least square sense. </summary>
		/// <returns> a solver </returns>
		public virtual DecompositionSolver Solver
		{
			get
			{
				return new Solver(singularValues, UT, V, Rank == m, tol);
			}
		}

		/// <summary>
		/// Specialized solver. </summary>
		private class Solver : DecompositionSolver
		{
			/// <summary>
			/// Pseudo-inverse of the initial matrix. </summary>
			internal readonly RealMatrix pseudoInverse;
			/// <summary>
			/// Singularity indicator. </summary>
			internal bool nonSingular;

			/// <summary>
			/// Build a solver from decomposed matrix.
			/// </summary>
			/// <param name="singularValues"> Singular values. </param>
			/// <param name="uT"> U<sup>T</sup> matrix of the decomposition. </param>
			/// <param name="v"> V matrix of the decomposition. </param>
			/// <param name="nonSingular"> Singularity indicator. </param>
			/// <param name="tol"> tolerance for singular values </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Solver(final double[] singularValues, final RealMatrix uT, final RealMatrix v, final boolean nonSingular, final double tol)
			internal Solver(double[] singularValues, RealMatrix uT, RealMatrix v, bool nonSingular, double tol)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] suT = uT.getData();
				double[][] suT = uT.Data;
				for (int i = 0; i < singularValues.Length; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a;
					double a;
					if (singularValues[i] > tol)
					{
						a = 1 / singularValues[i];
					}
					else
					{
						a = 0;
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] suTi = suT[i];
					double[] suTi = suT[i];
					for (int j = 0; j < suTi.Length; ++j)
					{
						suTi[j] *= a;
					}
				}
				pseudoInverse = v.multiply(new Array2DRowRealMatrix(suT, false));
				this.nonSingular = nonSingular;
			}

			/// <summary>
			/// Solve the linear equation A &times; X = B in least square sense.
			/// <p>
			/// The m&times;n matrix A may not be square, the solution X is such that
			/// ||A &times; X - B|| is minimal.
			/// </p> </summary>
			/// <param name="b"> Right-hand side of the equation A &times; X = B </param>
			/// <returns> a vector X that minimizes the two norm of A &times; X - B </returns>
			/// <exception cref="mathlib.exception.DimensionMismatchException">
			/// if the matrices dimensions do not match. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RealVector solve(final RealVector b)
			public virtual RealVector solve(RealVector b)
			{
				return pseudoInverse.operate(b);
			}

			/// <summary>
			/// Solve the linear equation A &times; X = B in least square sense.
			/// <p>
			/// The m&times;n matrix A may not be square, the solution X is such that
			/// ||A &times; X - B|| is minimal.
			/// </p>
			/// </summary>
			/// <param name="b"> Right-hand side of the equation A &times; X = B </param>
			/// <returns> a matrix X that minimizes the two norm of A &times; X - B </returns>
			/// <exception cref="mathlib.exception.DimensionMismatchException">
			/// if the matrices dimensions do not match. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RealMatrix solve(final RealMatrix b)
			public virtual RealMatrix solve(RealMatrix b)
			{
				return pseudoInverse.multiply(b);
			}

			/// <summary>
			/// Check if the decomposed matrix is non-singular.
			/// </summary>
			/// <returns> {@code true} if the decomposed matrix is non-singular. </returns>
			public virtual bool NonSingular
			{
				get
				{
					return nonSingular;
				}
			}

			/// <summary>
			/// Get the pseudo-inverse of the decomposed matrix.
			/// </summary>
			/// <returns> the inverse matrix. </returns>
			public virtual RealMatrix Inverse
			{
				get
				{
					return pseudoInverse;
				}
			}
		}
	}

}
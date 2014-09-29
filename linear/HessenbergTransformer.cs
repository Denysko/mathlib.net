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

	using FastMath = mathlib.util.FastMath;
	using Precision = mathlib.util.Precision;

	/// <summary>
	/// Class transforming a general real matrix to Hessenberg form.
	/// <p>A m &times; m matrix A can be written as the product of three matrices: A = P
	/// &times; H &times; P<sup>T</sup> with P an orthogonal matrix and H a Hessenberg
	/// matrix. Both P and H are m &times; m matrices.</p>
	/// <p>Transformation to Hessenberg form is often not a goal by itself, but it is an
	/// intermediate step in more general decomposition algorithms like
	/// <seealso cref="EigenDecomposition eigen decomposition"/>. This class is therefore
	/// intended for internal use by the library and is not public. As a consequence
	/// of this explicitly limited scope, many methods directly returns references to
	/// internal arrays, not copies.</p>
	/// <p>This class is based on the method orthes in class EigenvalueDecomposition
	/// from the <a href="http://math.nist.gov/javanumerics/jama/">JAMA</a> library.</p>
	/// </summary>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/HessenbergDecomposition.html">MathWorld</a> </seealso>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Householder_transformation">Householder Transformations</a>
	/// @version $Id: HessenbergTransformer.java 1538368 2013-11-03 13:57:37Z erans $
	/// @since 3.1 </seealso>
	internal class HessenbergTransformer
	{
		/// <summary>
		/// Householder vectors. </summary>
		private readonly double[][] householderVectors;
		/// <summary>
		/// Temporary storage vector. </summary>
		private readonly double[] ort;
		/// <summary>
		/// Cached value of P. </summary>
		private RealMatrix cachedP;
		/// <summary>
		/// Cached value of Pt. </summary>
		private RealMatrix cachedPt;
		/// <summary>
		/// Cached value of H. </summary>
		private RealMatrix cachedH;

		/// <summary>
		/// Build the transformation to Hessenberg form of a general matrix.
		/// </summary>
		/// <param name="matrix"> matrix to transform </param>
		/// <exception cref="NonSquareMatrixException"> if the matrix is not square </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public HessenbergTransformer(final RealMatrix matrix)
		public HessenbergTransformer(RealMatrix matrix)
		{
			if (!matrix.Square)
			{
				throw new NonSquareMatrixException(matrix.RowDimension, matrix.ColumnDimension);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = matrix.getRowDimension();
			int m = matrix.RowDimension;
			householderVectors = matrix.Data;
			ort = new double[m];
			cachedP = null;
			cachedPt = null;
			cachedH = null;

			// transform matrix
			transform();
		}

		/// <summary>
		/// Returns the matrix P of the transform.
		/// <p>P is an orthogonal matrix, i.e. its inverse is also its transpose.</p>
		/// </summary>
		/// <returns> the P matrix </returns>
		public virtual RealMatrix P
		{
			get
			{
				if (cachedP == null)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int n = householderVectors.length;
					int n = householderVectors.Length;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int high = n - 1;
					int high = n - 1;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[][] pa = new double[n][n];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] pa = new double[n][n];
					double[][] pa = RectangularArrays.ReturnRectangularDoubleArray(n, n);
    
					for (int i = 0; i < n; i++)
					{
						for (int j = 0; j < n; j++)
						{
							pa[i][j] = (i == j) ? 1 : 0;
						}
					}
    
					for (int m = high - 1; m >= 1; m--)
					{
						if (householderVectors[m][m - 1] != 0.0)
						{
							for (int i = m + 1; i <= high; i++)
							{
								ort[i] = householderVectors[i][m - 1];
							}
    
							for (int j = m; j <= high; j++)
							{
								double g = 0.0;
    
								for (int i = m; i <= high; i++)
								{
									g += ort[i] * pa[i][j];
								}
    
								// Double division avoids possible underflow
								g = (g / ort[m]) / householderVectors[m][m - 1];
    
								for (int i = m; i <= high; i++)
								{
									pa[i][j] += g * ort[i];
								}
							}
						}
					}
    
					cachedP = MatrixUtils.createRealMatrix(pa);
				}
				return cachedP;
			}
		}

		/// <summary>
		/// Returns the transpose of the matrix P of the transform.
		/// <p>P is an orthogonal matrix, i.e. its inverse is also its transpose.</p>
		/// </summary>
		/// <returns> the transpose of the P matrix </returns>
		public virtual RealMatrix PT
		{
			get
			{
				if (cachedPt == null)
				{
					cachedPt = P.transpose();
				}
    
				// return the cached matrix
				return cachedPt;
			}
		}

		/// <summary>
		/// Returns the Hessenberg matrix H of the transform.
		/// </summary>
		/// <returns> the H matrix </returns>
		public virtual RealMatrix H
		{
			get
			{
				if (cachedH == null)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int m = householderVectors.length;
					int m = householderVectors.Length;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[][] h = new double[m][m];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] h = new double[m][m];
					double[][] h = RectangularArrays.ReturnRectangularDoubleArray(m, m);
					for (int i = 0; i < m; ++i)
					{
						if (i > 0)
						{
							// copy the entry of the lower sub-diagonal
							h[i][i - 1] = householderVectors[i][i - 1];
						}
    
						// copy upper triangular part of the matrix
						for (int j = i; j < m; ++j)
						{
							h[i][j] = householderVectors[i][j];
						}
					}
					cachedH = MatrixUtils.createRealMatrix(h);
				}
    
				// return the cached matrix
				return cachedH;
			}
		}

		/// <summary>
		/// Get the Householder vectors of the transform.
		/// <p>Note that since this class is only intended for internal use, it returns
		/// directly a reference to its internal arrays, not a copy.</p>
		/// </summary>
		/// <returns> the main diagonal elements of the B matrix </returns>
		internal virtual double[][] HouseholderVectorsRef
		{
			get
			{
				return householderVectors;
			}
		}

		/// <summary>
		/// Transform original matrix to Hessenberg form.
		/// <p>Transformation is done using Householder transforms.</p>
		/// </summary>
		private void transform()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = householderVectors.length;
			int n = householderVectors.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int high = n - 1;
			int high = n - 1;

			for (int m = 1; m <= high - 1; m++)
			{
				// Scale column.
				double scale = 0;
				for (int i = m; i <= high; i++)
				{
					scale += FastMath.abs(householderVectors[i][m - 1]);
				}

				if (!Precision.Equals(scale, 0))
				{
					// Compute Householder transformation.
					double h = 0;
					for (int i = high; i >= m; i--)
					{
						ort[i] = householderVectors[i][m - 1] / scale;
						h += ort[i] * ort[i];
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double g = (ort[m] > 0) ? -mathlib.util.FastMath.sqrt(h) : mathlib.util.FastMath.sqrt(h);
					double g = (ort[m] > 0) ? - FastMath.sqrt(h) : FastMath.sqrt(h);

					h -= ort[m] * g;
					ort[m] -= g;

					// Apply Householder similarity transformation
					// H = (I - u*u' / h) * H * (I - u*u' / h)

					for (int j = m; j < n; j++)
					{
						double f = 0;
						for (int i = high; i >= m; i--)
						{
							f += ort[i] * householderVectors[i][j];
						}
						f /= h;
						for (int i = m; i <= high; i++)
						{
							householderVectors[i][j] -= f * ort[i];
						}
					}

					for (int i = 0; i <= high; i++)
					{
						double f = 0;
						for (int j = high; j >= m; j--)
						{
							f += ort[j] * householderVectors[i][j];
						}
						f /= h;
						for (int j = m; j <= high; j++)
						{
							householderVectors[i][j] -= f * ort[j];
						}
					}

					ort[m] = scale * ort[m];
					householderVectors[m][m - 1] = scale * g;
				}
			}
		}
	}

}
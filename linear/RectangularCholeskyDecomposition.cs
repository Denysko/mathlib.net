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

namespace org.apache.commons.math3.linear
{

	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Calculates the rectangular Cholesky decomposition of a matrix.
	/// <p>The rectangular Cholesky decomposition of a real symmetric positive
	/// semidefinite matrix A consists of a rectangular matrix B with the same
	/// number of rows such that: A is almost equal to BB<sup>T</sup>, depending
	/// on a user-defined tolerance. In a sense, this is the square root of A.</p>
	/// <p>The difference with respect to the regular <seealso cref="CholeskyDecomposition"/>
	/// is that rows/columns may be permuted (hence the rectangular shape instead
	/// of the traditional triangular shape) and there is a threshold to ignore
	/// small diagonal elements. This is used for example to generate {@link
	/// org.apache.commons.math3.random.CorrelatedRandomVectorGenerator correlated
	/// random n-dimensions vectors} in a p-dimension subspace (p < n).
	/// In other words, it allows generating random vectors from a covariance
	/// matrix that is only positive semidefinite, and not positive definite.</p>
	/// <p>Rectangular Cholesky decomposition is <em>not</em> suited for solving
	/// linear systems, so it does not provide any {@link DecompositionSolver
	/// decomposition solver}.</p>
	/// </summary>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/CholeskyDecomposition.html">MathWorld</a> </seealso>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Cholesky_decomposition">Wikipedia</a>
	/// @version $Id: RectangularCholeskyDecomposition.java 1422313 2012-12-15 18:53:41Z psteitz $
	/// @since 2.0 (changed to concrete class in 3.0) </seealso>
	public class RectangularCholeskyDecomposition
	{

		/// <summary>
		/// Permutated Cholesky root of the symmetric positive semidefinite matrix. </summary>
		private readonly RealMatrix root;

		/// <summary>
		/// Rank of the symmetric positive semidefinite matrix. </summary>
		private int rank;

		/// <summary>
		/// Decompose a symmetric positive semidefinite matrix.
		/// <p>
		/// <b>Note:</b> this constructor follows the linpack method to detect dependent
		/// columns by proceeding with the Cholesky algorithm until a nonpositive diagonal
		/// element is encountered.
		/// </summary>
		/// <seealso cref= <a href="http://eprints.ma.man.ac.uk/1193/01/covered/MIMS_ep2008_56.pdf">
		/// Analysis of the Cholesky Decomposition of a Semi-definite Matrix</a>
		/// </seealso>
		/// <param name="matrix"> Symmetric positive semidefinite matrix. </param>
		/// <exception cref="NonPositiveDefiniteMatrixException"> if the matrix is not
		/// positive semidefinite.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RectangularCholeskyDecomposition(RealMatrix matrix) throws NonPositiveDefiniteMatrixException
		public RectangularCholeskyDecomposition(RealMatrix matrix) : this(matrix, 0)
		{
		}

		/// <summary>
		/// Decompose a symmetric positive semidefinite matrix.
		/// </summary>
		/// <param name="matrix"> Symmetric positive semidefinite matrix. </param>
		/// <param name="small"> Diagonal elements threshold under which columns are
		/// considered to be dependent on previous ones and are discarded. </param>
		/// <exception cref="NonPositiveDefiniteMatrixException"> if the matrix is not
		/// positive semidefinite. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RectangularCholeskyDecomposition(RealMatrix matrix, double small) throws NonPositiveDefiniteMatrixException
		public RectangularCholeskyDecomposition(RealMatrix matrix, double small)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int order = matrix.getRowDimension();
			int order = matrix.RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] c = matrix.getData();
			double[][] c = matrix.Data;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] b = new double[order][order];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] b = new double[order][order];
			double[][] b = RectangularArrays.ReturnRectangularDoubleArray(order, order);

			int[] index = new int[order];
			for (int i = 0; i < order; ++i)
			{
				index[i] = i;
			}

			int r = 0;
			for (bool loop = true; loop;)
			{

				// find maximal diagonal element
				int swapR = r;
				for (int i = r + 1; i < order; ++i)
				{
					int ii = index[i];
					int isr = index[swapR];
					if (c[ii][ii] > c[isr][isr])
					{
						swapR = i;
					}
				}


				// swap elements
				if (swapR != r)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int tmpIndex = index[r];
					int tmpIndex = index[r];
					index[r] = index[swapR];
					index[swapR] = tmpIndex;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tmpRow = b[r];
					double[] tmpRow = b[r];
					b[r] = b[swapR];
					b[swapR] = tmpRow;
				}

				// check diagonal element
				int ir = index[r];
				if (c[ir][ir] <= small)
				{

					if (r == 0)
					{
						throw new NonPositiveDefiniteMatrixException(c[ir][ir], ir, small);
					}

					// check remaining diagonal elements
					for (int i = r; i < order; ++i)
					{
						if (c[index[i]][index[i]] < -small)
						{
							// there is at least one sufficiently negative diagonal element,
							// the symmetric positive semidefinite matrix is wrong
							throw new NonPositiveDefiniteMatrixException(c[index[i]][index[i]], i, small);
						}
					}

					// all remaining diagonal elements are close to zero, we consider we have
					// found the rank of the symmetric positive semidefinite matrix
					loop = false;

				}
				else
				{

					// transform the matrix
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sqrt = org.apache.commons.math3.util.FastMath.sqrt(c[ir][ir]);
					double sqrt = FastMath.sqrt(c[ir][ir]);
					b[r][r] = sqrt;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double inverse = 1 / sqrt;
					double inverse = 1 / sqrt;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double inverse2 = 1 / c[ir][ir];
					double inverse2 = 1 / c[ir][ir];
					for (int i = r + 1; i < order; ++i)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ii = index[i];
						int ii = index[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double e = inverse * c[ii][ir];
						double e = inverse * c[ii][ir];
						b[i][r] = e;
						c[ii][ii] -= c[ii][ir] * c[ii][ir] * inverse2;
						for (int j = r + 1; j < i; ++j)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ij = index[j];
							int ij = index[j];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double f = c[ii][ij] - e * b[j][r];
							double f = c[ii][ij] - e * b[j][r];
							c[ii][ij] = f;
							c[ij][ii] = f;
						}
					}

					// prepare next iteration
					loop = ++r < order;
				}
			}

			// build the root matrix
			rank = r;
			root = MatrixUtils.createRealMatrix(order, r);
			for (int i = 0; i < order; ++i)
			{
				for (int j = 0; j < r; ++j)
				{
					root.setEntry(index[i], j, b[i][j]);
				}
			}

		}

		/// <summary>
		/// Get the root of the covariance matrix.
		/// The root is the rectangular matrix <code>B</code> such that
		/// the covariance matrix is equal to <code>B.B<sup>T</sup></code> </summary>
		/// <returns> root of the square matrix </returns>
		/// <seealso cref= #getRank() </seealso>
		public virtual RealMatrix RootMatrix
		{
			get
			{
				return root;
			}
		}

		/// <summary>
		/// Get the rank of the symmetric positive semidefinite matrix.
		/// The r is the number of independent rows in the symmetric positive semidefinite
		/// matrix, it is also the number of columns of the rectangular
		/// matrix of the decomposition. </summary>
		/// <returns> r of the square matrix. </returns>
		/// <seealso cref= #getRootMatrix() </seealso>
		public virtual int Rank
		{
			get
			{
				return rank;
			}
		}

	}

}
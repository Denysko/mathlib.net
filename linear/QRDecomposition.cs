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

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using FastMath = mathlib.util.FastMath;


	/// <summary>
	/// Calculates the QR-decomposition of a matrix.
	/// <p>The QR-decomposition of a matrix A consists of two matrices Q and R
	/// that satisfy: A = QR, Q is orthogonal (Q<sup>T</sup>Q = I), and R is
	/// upper triangular. If A is m&times;n, Q is m&times;m and R m&times;n.</p>
	/// <p>This class compute the decomposition using Householder reflectors.</p>
	/// <p>For efficiency purposes, the decomposition in packed form is transposed.
	/// This allows inner loop to iterate inside rows, which is much more cache-efficient
	/// in Java.</p>
	/// <p>This class is based on the class with similar name from the
	/// <a href="http://math.nist.gov/javanumerics/jama/">JAMA</a> library, with the
	/// following changes:</p>
	/// <ul>
	///   <li>a <seealso cref="#getQT() getQT"/> method has been added,</li>
	///   <li>the {@code solve} and {@code isFullRank} methods have been replaced
	///   by a <seealso cref="#getSolver() getSolver"/> method and the equivalent methods
	///   provided by the returned <seealso cref="DecompositionSolver"/>.</li>
	/// </ul>
	/// </summary>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/QRDecomposition.html">MathWorld</a> </seealso>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/QR_decomposition">Wikipedia</a>
	/// 
	/// @version $Id: QRDecomposition.java 1570994 2014-02-23 11:10:41Z luc $
	/// @since 1.2 (changed to concrete class in 3.0) </seealso>
	public class QRDecomposition
	{
		/// <summary>
		/// A packed TRANSPOSED representation of the QR decomposition.
		/// <p>The elements BELOW the diagonal are the elements of the UPPER triangular
		/// matrix R, and the rows ABOVE the diagonal are the Householder reflector vectors
		/// from which an explicit form of Q can be recomputed if desired.</p>
		/// </summary>
		private double[][] qrt;
		/// <summary>
		/// The diagonal elements of R. </summary>
		private double[] rDiag;
		/// <summary>
		/// Cached value of Q. </summary>
		private RealMatrix cachedQ;
		/// <summary>
		/// Cached value of QT. </summary>
		private RealMatrix cachedQT;
		/// <summary>
		/// Cached value of R. </summary>
		private RealMatrix cachedR;
		/// <summary>
		/// Cached value of H. </summary>
		private RealMatrix cachedH;
		/// <summary>
		/// Singularity threshold. </summary>
		private readonly double threshold;

		/// <summary>
		/// Calculates the QR-decomposition of the given matrix.
		/// The singularity threshold defaults to zero.
		/// </summary>
		/// <param name="matrix"> The matrix to decompose.
		/// </param>
		/// <seealso cref= #QRDecomposition(RealMatrix,double) </seealso>
		public QRDecomposition(RealMatrix matrix) : this(matrix, 0d)
		{
		}

		/// <summary>
		/// Calculates the QR-decomposition of the given matrix.
		/// </summary>
		/// <param name="matrix"> The matrix to decompose. </param>
		/// <param name="threshold"> Singularity threshold. </param>
		public QRDecomposition(RealMatrix matrix, double threshold)
		{
			this.threshold = threshold;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = matrix.getRowDimension();
			int m = matrix.RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = matrix.getColumnDimension();
			int n = matrix.ColumnDimension;
			qrt = matrix.transpose().Data;
			rDiag = new double[FastMath.min(m, n)];
			cachedQ = null;
			cachedQT = null;
			cachedR = null;
			cachedH = null;

			decompose(qrt);

		}

		/// <summary>
		/// Decompose matrix. </summary>
		/// <param name="matrix"> transposed matrix
		/// @since 3.2 </param>
		protected internal virtual void decompose(double[][] matrix)
		{
			for (int minor = 0; minor < FastMath.min(qrt.Length, qrt[0].Length); minor++)
			{
				performHouseholderReflection(minor, qrt);
			}
		}

		/// <summary>
		/// Perform Householder reflection for a minor A(minor, minor) of A. </summary>
		/// <param name="minor"> minor index </param>
		/// <param name="matrix"> transposed matrix
		/// @since 3.2 </param>
		protected internal virtual void performHouseholderReflection(int minor, double[][] matrix)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] qrtMinor = qrt[minor];
			double[] qrtMinor = qrt[minor];

			/*
			 * Let x be the first column of the minor, and a^2 = |x|^2.
			 * x will be in the positions qr[minor][minor] through qr[m][minor].
			 * The first column of the transformed minor will be (a,0,0,..)'
			 * The sign of a is chosen to be opposite to the sign of the first
			 * component of x. Let's find a:
			 */
			double xNormSqr = 0;
			for (int row = minor; row < qrtMinor.Length; row++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c = qrtMinor[row];
				double c = qrtMinor[row];
				xNormSqr += c * c;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a = (qrtMinor[minor] > 0) ? -mathlib.util.FastMath.sqrt(xNormSqr) : mathlib.util.FastMath.sqrt(xNormSqr);
			double a = (qrtMinor[minor] > 0) ? - FastMath.sqrt(xNormSqr) : FastMath.sqrt(xNormSqr);
			rDiag[minor] = a;

			if (a != 0.0)
			{

				/*
				 * Calculate the normalized reflection vector v and transform
				 * the first column. We know the norm of v beforehand: v = x-ae
				 * so |v|^2 = <x-ae,x-ae> = <x,x>-2a<x,e>+a^2<e,e> =
				 * a^2+a^2-2a<x,e> = 2a*(a - <x,e>).
				 * Here <x, e> is now qr[minor][minor].
				 * v = x-ae is stored in the column at qr:
				 */
				qrtMinor[minor] -= a; // now |v|^2 = -2a*(qr[minor][minor])

				/*
				 * Transform the rest of the columns of the minor:
				 * They will be transformed by the matrix H = I-2vv'/|v|^2.
				 * If x is a column vector of the minor, then
				 * Hx = (I-2vv'/|v|^2)x = x-2vv'x/|v|^2 = x - 2<x,v>/|v|^2 v.
				 * Therefore the transformation is easily calculated by
				 * subtracting the column vector (2<x,v>/|v|^2)v from x.
				 *
				 * Let 2<x,v>/|v|^2 = alpha. From above we have
				 * |v|^2 = -2a*(qr[minor][minor]), so
				 * alpha = -<x,v>/(a*qr[minor][minor])
				 */
				for (int col = minor + 1; col < qrt.Length; col++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] qrtCol = qrt[col];
					double[] qrtCol = qrt[col];
					double alpha = 0;
					for (int row = minor; row < qrtCol.Length; row++)
					{
						alpha -= qrtCol[row] * qrtMinor[row];
					}
					alpha /= a * qrtMinor[minor];

					// Subtract the column vector alpha*v from x.
					for (int row = minor; row < qrtCol.Length; row++)
					{
						qrtCol[row] -= alpha * qrtMinor[row];
					}
				}
			}
		}


		/// <summary>
		/// Returns the matrix R of the decomposition.
		/// <p>R is an upper-triangular matrix</p> </summary>
		/// <returns> the R matrix </returns>
		public virtual RealMatrix R
		{
			get
			{
    
				if (cachedR == null)
				{
    
					// R is supposed to be m x n
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int n = qrt.length;
					int n = qrt.Length;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int m = qrt[0].length;
					int m = qrt[0].Length;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] ra = new double[m][n];
					double[][] ra = RectangularArrays.ReturnRectangularDoubleArray(m, n);
					// copy the diagonal from rDiag and the upper triangle of qr
					for (int row = FastMath.min(m, n) - 1; row >= 0; row--)
					{
						ra[row][row] = rDiag[row];
						for (int col = row + 1; col < n; col++)
						{
							ra[row][col] = qrt[col][row];
						}
					}
					cachedR = MatrixUtils.createRealMatrix(ra);
				}
    
				// return the cached matrix
				return cachedR;
			}
		}

		/// <summary>
		/// Returns the matrix Q of the decomposition.
		/// <p>Q is an orthogonal matrix</p> </summary>
		/// <returns> the Q matrix </returns>
		public virtual RealMatrix Q
		{
			get
			{
				if (cachedQ == null)
				{
					cachedQ = QT.transpose();
				}
				return cachedQ;
			}
		}

		/// <summary>
		/// Returns the transpose of the matrix Q of the decomposition.
		/// <p>Q is an orthogonal matrix</p> </summary>
		/// <returns> the transpose of the Q matrix, Q<sup>T</sup> </returns>
		public virtual RealMatrix QT
		{
			get
			{
				if (cachedQT == null)
				{
    
					// QT is supposed to be m x m
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int n = qrt.length;
					int n = qrt.Length;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int m = qrt[0].length;
					int m = qrt[0].Length;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] qta = new double[m][m];
					double[][] qta = RectangularArrays.ReturnRectangularDoubleArray(m, m);
    
					/*
					 * Q = Q1 Q2 ... Q_m, so Q is formed by first constructing Q_m and then
					 * applying the Householder transformations Q_(m-1),Q_(m-2),...,Q1 in
					 * succession to the result
					 */
					for (int minor = m - 1; minor >= FastMath.min(m, n); minor--)
					{
						qta[minor][minor] = 1.0d;
					}
    
					for (int minor = FastMath.min(m, n) - 1; minor >= 0; minor--)
					{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[] qrtMinor = qrt[minor];
						double[] qrtMinor = qrt[minor];
						qta[minor][minor] = 1.0d;
						if (qrtMinor[minor] != 0.0)
						{
							for (int col = minor; col < m; col++)
							{
								double alpha = 0;
								for (int row = minor; row < m; row++)
								{
									alpha -= qta[col][row] * qrtMinor[row];
								}
								alpha /= rDiag[minor] * qrtMinor[minor];
    
								for (int row = minor; row < m; row++)
								{
									qta[col][row] += -alpha * qrtMinor[row];
								}
							}
						}
					}
					cachedQT = MatrixUtils.createRealMatrix(qta);
				}
    
				// return the cached matrix
				return cachedQT;
			}
		}

		/// <summary>
		/// Returns the Householder reflector vectors.
		/// <p>H is a lower trapezoidal matrix whose columns represent
		/// each successive Householder reflector vector. This matrix is used
		/// to compute Q.</p> </summary>
		/// <returns> a matrix containing the Householder reflector vectors </returns>
		public virtual RealMatrix H
		{
			get
			{
				if (cachedH == null)
				{
    
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int n = qrt.length;
					int n = qrt.Length;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int m = qrt[0].length;
					int m = qrt[0].Length;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] ha = new double[m][n];
					double[][] ha = RectangularArrays.ReturnRectangularDoubleArray(m, n);
					for (int i = 0; i < m; ++i)
					{
						for (int j = 0; j < FastMath.min(i + 1, n); ++j)
						{
							ha[i][j] = qrt[j][i] / -rDiag[j];
						}
					}
					cachedH = MatrixUtils.createRealMatrix(ha);
				}
    
				// return the cached matrix
				return cachedH;
			}
		}

		/// <summary>
		/// Get a solver for finding the A &times; X = B solution in least square sense.
		/// <p>
		/// Least Square sense means a solver can be computed for an overdetermined system,
		/// (i.e. a system with more equations than unknowns, which corresponds to a tall A
		/// matrix with more rows than columns). In any case, if the matrix is singular
		/// within the tolerance set at {@link QRDecomposition#QRDecomposition(RealMatrix,
		/// double) construction}, an error will be triggered when
		/// the <seealso cref="DecompositionSolver#solve(RealVector) solve"/> method will be called.
		/// </p> </summary>
		/// <returns> a solver </returns>
		public virtual DecompositionSolver Solver
		{
			get
			{
				return new Solver(qrt, rDiag, threshold);
			}
		}

		/// <summary>
		/// Specialized solver. </summary>
		private class Solver : DecompositionSolver
		{
			/// <summary>
			/// A packed TRANSPOSED representation of the QR decomposition.
			/// <p>The elements BELOW the diagonal are the elements of the UPPER triangular
			/// matrix R, and the rows ABOVE the diagonal are the Householder reflector vectors
			/// from which an explicit form of Q can be recomputed if desired.</p>
			/// </summary>
			internal readonly double[][] qrt;
			/// <summary>
			/// The diagonal elements of R. </summary>
			internal readonly double[] rDiag;
			/// <summary>
			/// Singularity threshold. </summary>
			internal readonly double threshold;

			/// <summary>
			/// Build a solver from decomposed matrix.
			/// </summary>
			/// <param name="qrt"> Packed TRANSPOSED representation of the QR decomposition. </param>
			/// <param name="rDiag"> Diagonal elements of R. </param>
			/// <param name="threshold"> Singularity threshold. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Solver(final double[][] qrt, final double[] rDiag, final double threshold)
			internal Solver(double[][] qrt, double[] rDiag, double threshold)
			{
				this.qrt = qrt;
				this.rDiag = rDiag;
				this.threshold = threshold;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual bool NonSingular
			{
				get
				{
					foreach (double diag in rDiag)
					{
						if (FastMath.abs(diag) <= threshold)
						{
							return false;
						}
					}
					return true;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual RealVector solve(RealVector b)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = qrt.length;
				int n = qrt.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = qrt[0].length;
				int m = qrt[0].Length;
				if (b.Dimension != m)
				{
					throw new DimensionMismatchException(b.Dimension, m);
				}
				if (!NonSingular)
				{
					throw new SingularMatrixException();
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] x = new double[n];
				double[] x = new double[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y = b.toArray();
				double[] y = b.toArray();

				// apply Householder transforms to solve Q.y = b
				for (int minor = 0; minor < FastMath.min(m, n); minor++)
				{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] qrtMinor = qrt[minor];
					double[] qrtMinor = qrt[minor];
					double dotProduct = 0;
					for (int row = minor; row < m; row++)
					{
						dotProduct += y[row] * qrtMinor[row];
					}
					dotProduct /= rDiag[minor] * qrtMinor[minor];

					for (int row = minor; row < m; row++)
					{
						y[row] += dotProduct * qrtMinor[row];
					}
				}

				// solve triangular system R.x = y
				for (int row = rDiag.Length - 1; row >= 0; --row)
				{
					y[row] /= rDiag[row];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yRow = y[row];
					double yRow = y[row];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] qrtRow = qrt[row];
					double[] qrtRow = qrt[row];
					x[row] = yRow;
					for (int i = 0; i < row; i++)
					{
						y[i] -= yRow * qrtRow[i];
					}
				}

				return new ArrayRealVector(x, false);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual RealMatrix solve(RealMatrix b)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = qrt.length;
				int n = qrt.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = qrt[0].length;
				int m = qrt[0].Length;
				if (b.RowDimension != m)
				{
					throw new DimensionMismatchException(b.RowDimension, m);
				}
				if (!NonSingular)
				{
					throw new SingularMatrixException();
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columns = b.getColumnDimension();
				int columns = b.ColumnDimension;
				const int blockSize = BlockRealMatrix.BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int cBlocks = (columns + blockSize - 1) / blockSize;
				int cBlocks = (columns + blockSize - 1) / blockSize;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] xBlocks = BlockRealMatrix.createBlocksLayout(n, columns);
				double[][] xBlocks = BlockRealMatrix.createBlocksLayout(n, columns);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] y = new double[b.getRowDimension()][blockSize];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] y = new double[b.RowDimension][blockSize];
				double[][] y = RectangularArrays.ReturnRectangularDoubleArray(b.RowDimension, blockSize);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] alpha = new double[blockSize];
				double[] alpha = new double[blockSize];

				for (int kBlock = 0; kBlock < cBlocks; ++kBlock)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int kStart = kBlock * blockSize;
					int kStart = kBlock * blockSize;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int kEnd = mathlib.util.FastMath.min(kStart + blockSize, columns);
					int kEnd = FastMath.min(kStart + blockSize, columns);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int kWidth = kEnd - kStart;
					int kWidth = kEnd - kStart;

					// get the right hand side vector
					b.copySubMatrix(0, m - 1, kStart, kEnd - 1, y);

					// apply Householder transforms to solve Q.y = b
					for (int minor = 0; minor < FastMath.min(m, n); minor++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] qrtMinor = qrt[minor];
						double[] qrtMinor = qrt[minor];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double factor = 1.0 / (rDiag[minor] * qrtMinor[minor]);
						double factor = 1.0 / (rDiag[minor] * qrtMinor[minor]);

						Arrays.fill(alpha, 0, kWidth, 0.0);
						for (int row = minor; row < m; ++row)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d = qrtMinor[row];
							double d = qrtMinor[row];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yRow = y[row];
							double[] yRow = y[row];
							for (int k = 0; k < kWidth; ++k)
							{
								alpha[k] += d * yRow[k];
							}
						}
						for (int k = 0; k < kWidth; ++k)
						{
							alpha[k] *= factor;
						}

						for (int row = minor; row < m; ++row)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d = qrtMinor[row];
							double d = qrtMinor[row];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yRow = y[row];
							double[] yRow = y[row];
							for (int k = 0; k < kWidth; ++k)
							{
								yRow[k] += alpha[k] * d;
							}
						}
					}

					// solve triangular system R.x = y
					for (int j = rDiag.Length - 1; j >= 0; --j)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jBlock = j / blockSize;
						int jBlock = j / blockSize;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jStart = jBlock * blockSize;
						int jStart = jBlock * blockSize;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double factor = 1.0 / rDiag[j];
						double factor = 1.0 / rDiag[j];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yJ = y[j];
						double[] yJ = y[j];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] xBlock = xBlocks[jBlock * cBlocks + kBlock];
						double[] xBlock = xBlocks[jBlock * cBlocks + kBlock];
						int index = (j - jStart) * kWidth;
						for (int k = 0; k < kWidth; ++k)
						{
							yJ[k] *= factor;
							xBlock[index++] = yJ[k];
						}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] qrtJ = qrt[j];
						double[] qrtJ = qrt[j];
						for (int i = 0; i < j; ++i)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rIJ = qrtJ[i];
							double rIJ = qrtJ[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yI = y[i];
							double[] yI = y[i];
							for (int k = 0; k < kWidth; ++k)
							{
								yI[k] -= yJ[k] * rIJ;
							}
						}
					}
				}

				return new BlockRealMatrix(n, columns, xBlocks, false);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			/// <exception cref="SingularMatrixException"> if the decomposed matrix is singular. </exception>
			public virtual RealMatrix Inverse
			{
				get
				{
					return solve(MatrixUtils.createRealIdentityMatrix(qrt[0].Length));
				}
			}
		}
	}

}
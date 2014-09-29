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
	/// Calculates the LUP-decomposition of a square matrix.
	/// <p>The LUP-decomposition of a matrix A consists of three matrices L, U and
	/// P that satisfy: P&times;A = L&times;U. L is lower triangular (with unit
	/// diagonal terms), U is upper triangular and P is a permutation matrix. All
	/// matrices are m&times;m.</p>
	/// <p>As shown by the presence of the P matrix, this decomposition is
	/// implemented using partial pivoting.</p>
	/// <p>This class is based on the class with similar name from the
	/// <a href="http://math.nist.gov/javanumerics/jama/">JAMA</a> library.</p>
	/// <ul>
	///   <li>a <seealso cref="#getP() getP"/> method has been added,</li>
	///   <li>the {@code det} method has been renamed as {@link #getDeterminant()
	///   getDeterminant},</li>
	///   <li>the {@code getDoublePivot} method has been removed (but the int based
	///   <seealso cref="#getPivot() getPivot"/> method has been kept),</li>
	///   <li>the {@code solve} and {@code isNonSingular} methods have been replaced
	///   by a <seealso cref="#getSolver() getSolver"/> method and the equivalent methods
	///   provided by the returned <seealso cref="DecompositionSolver"/>.</li>
	/// </ul>
	/// </summary>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/LUDecomposition.html">MathWorld</a> </seealso>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/LU_decomposition">Wikipedia</a>
	/// @version $Id: LUDecomposition.java 1566017 2014-02-08 14:13:34Z tn $
	/// @since 2.0 (changed to concrete class in 3.0) </seealso>
	public class LUDecomposition
	{
		/// <summary>
		/// Default bound to determine effective singularity in LU decomposition. </summary>
		private const double DEFAULT_TOO_SMALL = 1e-11;
		/// <summary>
		/// Entries of LU decomposition. </summary>
		private readonly double[][] lu;
		/// <summary>
		/// Pivot permutation associated with LU decomposition. </summary>
		private readonly int[] pivot;
		/// <summary>
		/// Parity of the permutation associated with the LU decomposition. </summary>
		private bool even;
		/// <summary>
		/// Singularity indicator. </summary>
		private bool singular;
		/// <summary>
		/// Cached value of L. </summary>
		private RealMatrix cachedL;
		/// <summary>
		/// Cached value of U. </summary>
		private RealMatrix cachedU;
		/// <summary>
		/// Cached value of P. </summary>
		private RealMatrix cachedP;

		/// <summary>
		/// Calculates the LU-decomposition of the given matrix.
		/// This constructor uses 1e-11 as default value for the singularity
		/// threshold.
		/// </summary>
		/// <param name="matrix"> Matrix to decompose. </param>
		/// <exception cref="NonSquareMatrixException"> if matrix is not square. </exception>
		public LUDecomposition(RealMatrix matrix) : this(matrix, DEFAULT_TOO_SMALL)
		{
		}

		/// <summary>
		/// Calculates the LU-decomposition of the given matrix. </summary>
		/// <param name="matrix"> The matrix to decompose. </param>
		/// <param name="singularityThreshold"> threshold (based on partial row norm)
		/// under which a matrix is considered singular </param>
		/// <exception cref="NonSquareMatrixException"> if matrix is not square </exception>
		public LUDecomposition(RealMatrix matrix, double singularityThreshold)
		{
			if (!matrix.Square)
			{
				throw new NonSquareMatrixException(matrix.RowDimension, matrix.ColumnDimension);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = matrix.getColumnDimension();
			int m = matrix.ColumnDimension;
			lu = matrix.Data;
			pivot = new int[m];
			cachedL = null;
			cachedU = null;
			cachedP = null;

			// Initialize permutation array and parity
			for (int row = 0; row < m; row++)
			{
				pivot[row] = row;
			}
			even = true;
			singular = false;

			// Loop over columns
			for (int col = 0; col < m; col++)
			{

				// upper
				for (int row = 0; row < col; row++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] luRow = lu[row];
					double[] luRow = lu[row];
					double sum = luRow[col];
					for (int i = 0; i < row; i++)
					{
						sum -= luRow[i] * lu[i][col];
					}
					luRow[col] = sum;
				}

				// lower
				int max = col; // permutation row
				double largest = double.NegativeInfinity;
				for (int row = col; row < m; row++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] luRow = lu[row];
					double[] luRow = lu[row];
					double sum = luRow[col];
					for (int i = 0; i < col; i++)
					{
						sum -= luRow[i] * lu[i][col];
					}
					luRow[col] = sum;

					// maintain best permutation choice
					if (FastMath.abs(sum) > largest)
					{
						largest = FastMath.abs(sum);
						max = row;
					}
				}

				// Singularity check
				if (FastMath.abs(lu[max][col]) < singularityThreshold)
				{
					singular = true;
					return;
				}

				// Pivot if necessary
				if (max != col)
				{
					double tmp = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] luMax = lu[max];
					double[] luMax = lu[max];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] luCol = lu[col];
					double[] luCol = lu[col];
					for (int i = 0; i < m; i++)
					{
						tmp = luMax[i];
						luMax[i] = luCol[i];
						luCol[i] = tmp;
					}
					int temp = pivot[max];
					pivot[max] = pivot[col];
					pivot[col] = temp;
					even = !even;
				}

				// Divide the lower elements by the "winning" diagonal elt.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double luDiag = lu[col][col];
				double luDiag = lu[col][col];
				for (int row = col + 1; row < m; row++)
				{
					lu[row][col] /= luDiag;
				}
			}
		}

		/// <summary>
		/// Returns the matrix L of the decomposition.
		/// <p>L is a lower-triangular matrix</p> </summary>
		/// <returns> the L matrix (or null if decomposed matrix is singular) </returns>
		public virtual RealMatrix L
		{
			get
			{
				if ((cachedL == null) && !singular)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int m = pivot.length;
					int m = pivot.Length;
					cachedL = MatrixUtils.createRealMatrix(m, m);
					for (int i = 0; i < m; ++i)
					{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[] luI = lu[i];
						double[] luI = lu[i];
						for (int j = 0; j < i; ++j)
						{
							cachedL.setEntry(i, j, luI[j]);
						}
						cachedL.setEntry(i, i, 1.0);
					}
				}
				return cachedL;
			}
		}

		/// <summary>
		/// Returns the matrix U of the decomposition.
		/// <p>U is an upper-triangular matrix</p> </summary>
		/// <returns> the U matrix (or null if decomposed matrix is singular) </returns>
		public virtual RealMatrix U
		{
			get
			{
				if ((cachedU == null) && !singular)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int m = pivot.length;
					int m = pivot.Length;
					cachedU = MatrixUtils.createRealMatrix(m, m);
					for (int i = 0; i < m; ++i)
					{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[] luI = lu[i];
						double[] luI = lu[i];
						for (int j = i; j < m; ++j)
						{
							cachedU.setEntry(i, j, luI[j]);
						}
					}
				}
				return cachedU;
			}
		}

		/// <summary>
		/// Returns the P rows permutation matrix.
		/// <p>P is a sparse matrix with exactly one element set to 1.0 in
		/// each row and each column, all other elements being set to 0.0.</p>
		/// <p>The positions of the 1 elements are given by the {@link #getPivot()
		/// pivot permutation vector}.</p> </summary>
		/// <returns> the P rows permutation matrix (or null if decomposed matrix is singular) </returns>
		/// <seealso cref= #getPivot() </seealso>
		public virtual RealMatrix P
		{
			get
			{
				if ((cachedP == null) && !singular)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int m = pivot.length;
					int m = pivot.Length;
					cachedP = MatrixUtils.createRealMatrix(m, m);
					for (int i = 0; i < m; ++i)
					{
						cachedP.setEntry(i, pivot[i], 1.0);
					}
				}
				return cachedP;
			}
		}

		/// <summary>
		/// Returns the pivot permutation vector. </summary>
		/// <returns> the pivot permutation vector </returns>
		/// <seealso cref= #getP() </seealso>
		public virtual int[] Pivot
		{
			get
			{
				return pivot.clone();
			}
		}

		/// <summary>
		/// Return the determinant of the matrix </summary>
		/// <returns> determinant of the matrix </returns>
		public virtual double Determinant
		{
			get
			{
				if (singular)
				{
					return 0;
				}
				else
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int m = pivot.length;
					int m = pivot.Length;
					double determinant = even ? 1 : -1;
					for (int i = 0; i < m; i++)
					{
						determinant *= lu[i][i];
					}
					return determinant;
				}
			}
		}

		/// <summary>
		/// Get a solver for finding the A &times; X = B solution in exact linear
		/// sense. </summary>
		/// <returns> a solver </returns>
		public virtual DecompositionSolver Solver
		{
			get
			{
				return new Solver(lu, pivot, singular);
			}
		}

		/// <summary>
		/// Specialized solver. </summary>
		private class Solver : DecompositionSolver
		{

			/// <summary>
			/// Entries of LU decomposition. </summary>
			internal readonly double[][] lu;

			/// <summary>
			/// Pivot permutation associated with LU decomposition. </summary>
			internal readonly int[] pivot;

			/// <summary>
			/// Singularity indicator. </summary>
			internal readonly bool singular;

			/// <summary>
			/// Build a solver from decomposed matrix. </summary>
			/// <param name="lu"> entries of LU decomposition </param>
			/// <param name="pivot"> pivot permutation associated with LU decomposition </param>
			/// <param name="singular"> singularity indicator </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Solver(final double[][] lu, final int[] pivot, final boolean singular)
			internal Solver(double[][] lu, int[] pivot, bool singular)
			{
				this.lu = lu;
				this.pivot = pivot;
				this.singular = singular;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual bool NonSingular
			{
				get
				{
					return !singular;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual RealVector solve(RealVector b)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = pivot.length;
				int m = pivot.Length;
				if (b.Dimension != m)
				{
					throw new DimensionMismatchException(b.Dimension, m);
				}
				if (singular)
				{
					throw new SingularMatrixException();
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] bp = new double[m];
				double[] bp = new double[m];

				// Apply permutations to b
				for (int row = 0; row < m; row++)
				{
					bp[row] = b.getEntry(pivot[row]);
				}

				// Solve LY = b
				for (int col = 0; col < m; col++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bpCol = bp[col];
					double bpCol = bp[col];
					for (int i = col + 1; i < m; i++)
					{
						bp[i] -= bpCol * lu[i][col];
					}
				}

				// Solve UX = Y
				for (int col = m - 1; col >= 0; col--)
				{
					bp[col] /= lu[col][col];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bpCol = bp[col];
					double bpCol = bp[col];
					for (int i = 0; i < col; i++)
					{
						bp[i] -= bpCol * lu[i][col];
					}
				}

				return new ArrayRealVector(bp, false);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual RealMatrix solve(RealMatrix b)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = pivot.length;
				int m = pivot.Length;
				if (b.RowDimension != m)
				{
					throw new DimensionMismatchException(b.RowDimension, m);
				}
				if (singular)
				{
					throw new SingularMatrixException();
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nColB = b.getColumnDimension();
				int nColB = b.ColumnDimension;

				// Apply permutations to b
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] bp = new double[m][nColB];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] bp = new double[m][nColB];
				double[][] bp = RectangularArrays.ReturnRectangularDoubleArray(m, nColB);
				for (int row = 0; row < m; row++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] bpRow = bp[row];
					double[] bpRow = bp[row];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pRow = pivot[row];
					int pRow = pivot[row];
					for (int col = 0; col < nColB; col++)
					{
						bpRow[col] = b.getEntry(pRow, col);
					}
				}

				// Solve LY = b
				for (int col = 0; col < m; col++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] bpCol = bp[col];
					double[] bpCol = bp[col];
					for (int i = col + 1; i < m; i++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] bpI = bp[i];
						double[] bpI = bp[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double luICol = lu[i][col];
						double luICol = lu[i][col];
						for (int j = 0; j < nColB; j++)
						{
							bpI[j] -= bpCol[j] * luICol;
						}
					}
				}

				// Solve UX = Y
				for (int col = m - 1; col >= 0; col--)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] bpCol = bp[col];
					double[] bpCol = bp[col];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double luDiag = lu[col][col];
					double luDiag = lu[col][col];
					for (int j = 0; j < nColB; j++)
					{
						bpCol[j] /= luDiag;
					}
					for (int i = 0; i < col; i++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] bpI = bp[i];
						double[] bpI = bp[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double luICol = lu[i][col];
						double luICol = lu[i][col];
						for (int j = 0; j < nColB; j++)
						{
							bpI[j] -= bpCol[j] * luICol;
						}
					}
				}

				return new Array2DRowRealMatrix(bp, false);
			}

			/// <summary>
			/// Get the inverse of the decomposed matrix.
			/// </summary>
			/// <returns> the inverse matrix. </returns>
			/// <exception cref="SingularMatrixException"> if the decomposed matrix is singular. </exception>
			public virtual RealMatrix Inverse
			{
				get
				{
					return solve(MatrixUtils.createRealIdentityMatrix(pivot.Length));
				}
			}
		}
	}

}
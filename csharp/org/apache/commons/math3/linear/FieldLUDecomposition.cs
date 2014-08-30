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

	using org.apache.commons.math3;
	using org.apache.commons.math3;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MathArrays = org.apache.commons.math3.util.MathArrays;

	/// <summary>
	/// Calculates the LUP-decomposition of a square matrix.
	/// <p>The LUP-decomposition of a matrix A consists of three matrices
	/// L, U and P that satisfy: PA = LU, L is lower triangular, and U is
	/// upper triangular and P is a permutation matrix. All matrices are
	/// m&times;m.</p>
	/// <p>Since <seealso cref="FieldElement field elements"/> do not provide an ordering
	/// operator, the permutation matrix is computed here only in order to avoid
	/// a zero pivot element, no attempt is done to get the largest pivot
	/// element.</p>
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
	/// @param <T> the type of the field elements </param>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/LUDecomposition.html">MathWorld</a> </seealso>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/LU_decomposition">Wikipedia</a>
	/// @version $Id: FieldLUDecomposition.java 1449528 2013-02-24 19:06:20Z luc $
	/// @since 2.0 (changed to concrete class in 3.0) </seealso>
	public class FieldLUDecomposition<T> where T : org.apache.commons.math3.FieldElement<T>
	{

		/// <summary>
		/// Field to which the elements belong. </summary>
		private readonly Field<T> field;

		/// <summary>
		/// Entries of LU decomposition. </summary>
		private T[][] lu;

		/// <summary>
		/// Pivot permutation associated with LU decomposition. </summary>
		private int[] pivot;

		/// <summary>
		/// Parity of the permutation associated with the LU decomposition. </summary>
		private bool even;

		/// <summary>
		/// Singularity indicator. </summary>
		private bool singular;

		/// <summary>
		/// Cached value of L. </summary>
		private FieldMatrix<T> cachedL;

		/// <summary>
		/// Cached value of U. </summary>
		private FieldMatrix<T> cachedU;

		/// <summary>
		/// Cached value of P. </summary>
		private FieldMatrix<T> cachedP;

		/// <summary>
		/// Calculates the LU-decomposition of the given matrix. </summary>
		/// <param name="matrix"> The matrix to decompose. </param>
		/// <exception cref="NonSquareMatrixException"> if matrix is not square </exception>
		public FieldLUDecomposition(FieldMatrix<T> matrix)
		{
			if (!matrix.Square)
			{
				throw new NonSquareMatrixException(matrix.RowDimension, matrix.ColumnDimension);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = matrix.getColumnDimension();
			int m = matrix.ColumnDimension;
			field = matrix.Field;
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

				T sum = field.Zero;

				// upper
				for (int row = 0; row < col; row++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] luRow = lu[row];
					T[] luRow = lu[row];
					sum = luRow[col];
					for (int i = 0; i < row; i++)
					{
						sum = sum.subtract(luRow[i].multiply(lu[i][col]));
					}
					luRow[col] = sum;
				}

				// lower
				int nonZero = col; // permutation row
				for (int row = col; row < m; row++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] luRow = lu[row];
					T[] luRow = lu[row];
					sum = luRow[col];
					for (int i = 0; i < col; i++)
					{
						sum = sum.subtract(luRow[i].multiply(lu[i][col]));
					}
					luRow[col] = sum;

					if (lu[nonZero][col].Equals(field.Zero))
					{
						// try to select a better permutation choice
						++nonZero;
					}
				}

				// Singularity check
				if (nonZero >= m)
				{
					singular = true;
					return;
				}

				// Pivot if necessary
				if (nonZero != col)
				{
					T tmp = field.Zero;
					for (int i = 0; i < m; i++)
					{
						tmp = lu[nonZero][i];
						lu[nonZero][i] = lu[col][i];
						lu[col][i] = tmp;
					}
					int temp = pivot[nonZero];
					pivot[nonZero] = pivot[col];
					pivot[col] = temp;
					even = !even;
				}

				// Divide the lower elements by the "winning" diagonal elt.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T luDiag = lu[col][col];
				T luDiag = lu[col][col];
				for (int row = col + 1; row < m; row++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] luRow = lu[row];
					T[] luRow = lu[row];
					luRow[col] = luRow[col].divide(luDiag);
				}
			}

		}

		/// <summary>
		/// Returns the matrix L of the decomposition.
		/// <p>L is a lower-triangular matrix</p> </summary>
		/// <returns> the L matrix (or null if decomposed matrix is singular) </returns>
		public virtual FieldMatrix<T> L
		{
			get
			{
				if ((cachedL == null) && !singular)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int m = pivot.length;
					int m = pivot.Length;
					cachedL = new Array2DRowFieldMatrix<T>(field, m, m);
					for (int i = 0; i < m; ++i)
					{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T[] luI = lu[i];
						T[] luI = lu[i];
						for (int j = 0; j < i; ++j)
						{
							cachedL.setEntry(i, j, luI[j]);
						}
						cachedL.setEntry(i, i, field.One);
					}
				}
				return cachedL;
			}
		}

		/// <summary>
		/// Returns the matrix U of the decomposition.
		/// <p>U is an upper-triangular matrix</p> </summary>
		/// <returns> the U matrix (or null if decomposed matrix is singular) </returns>
		public virtual FieldMatrix<T> U
		{
			get
			{
				if ((cachedU == null) && !singular)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int m = pivot.length;
					int m = pivot.Length;
					cachedU = new Array2DRowFieldMatrix<T>(field, m, m);
					for (int i = 0; i < m; ++i)
					{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T[] luI = lu[i];
						T[] luI = lu[i];
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
		public virtual FieldMatrix<T> P
		{
			get
			{
				if ((cachedP == null) && !singular)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int m = pivot.length;
					int m = pivot.Length;
					cachedP = new Array2DRowFieldMatrix<T>(field, m, m);
					for (int i = 0; i < m; ++i)
					{
						cachedP.setEntry(i, pivot[i], field.One);
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
		/// Return the determinant of the matrix. </summary>
		/// <returns> determinant of the matrix </returns>
		public virtual T Determinant
		{
			get
			{
				if (singular)
				{
					return field.Zero;
				}
				else
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int m = pivot.length;
					int m = pivot.Length;
					T determinant = even ? field.One : field.Zero.subtract(field.One);
					for (int i = 0; i < m; i++)
					{
						determinant = determinant.multiply(lu[i][i]);
					}
					return determinant;
				}
			}
		}

		/// <summary>
		/// Get a solver for finding the A &times; X = B solution in exact linear sense. </summary>
		/// <returns> a solver </returns>
		public virtual FieldDecompositionSolver<T> Solver
		{
			get
			{
				return new Solver<T>(field, lu, pivot, singular);
			}
		}

		/// <summary>
		/// Specialized solver. </summary>
		private class Solver<T> : FieldDecompositionSolver<T> where T : org.apache.commons.math3.FieldElement<T>
		{

			/// <summary>
			/// Field to which the elements belong. </summary>
			internal readonly Field<T> field;

			/// <summary>
			/// Entries of LU decomposition. </summary>
			internal readonly T[][] lu;

			/// <summary>
			/// Pivot permutation associated with LU decomposition. </summary>
			internal readonly int[] pivot;

			/// <summary>
			/// Singularity indicator. </summary>
			internal readonly bool singular;

			/// <summary>
			/// Build a solver from decomposed matrix. </summary>
			/// <param name="field"> field to which the matrix elements belong </param>
			/// <param name="lu"> entries of LU decomposition </param>
			/// <param name="pivot"> pivot permutation associated with LU decomposition </param>
			/// <param name="singular"> singularity indicator </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Solver(final org.apache.commons.math3.Field<T> field, final T[][] lu, final int[] pivot, final boolean singular)
			internal Solver(Field<T> field, T[][] lu, int[] pivot, bool singular)
			{
				this.field = field;
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
			public virtual FieldVector<T> solve(FieldVector<T> b)
			{
				try
				{
					return solve((ArrayFieldVector<T>) b);
				}
				catch (System.InvalidCastException cce)
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

					// Apply permutations to b
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] bp = org.apache.commons.math3.util.MathArrays.buildArray(field, m);
					T[] bp = MathArrays.buildArray(field, m);
					for (int row = 0; row < m; row++)
					{
						bp[row] = b.getEntry(pivot[row]);
					}

					// Solve LY = b
					for (int col = 0; col < m; col++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T bpCol = bp[col];
						T bpCol = bp[col];
						for (int i = col + 1; i < m; i++)
						{
							bp[i] = bp[i].subtract(bpCol.multiply(lu[i][col]));
						}
					}

					// Solve UX = Y
					for (int col = m - 1; col >= 0; col--)
					{
						bp[col] = bp[col].divide(lu[col][col]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T bpCol = bp[col];
						T bpCol = bp[col];
						for (int i = 0; i < col; i++)
						{
							bp[i] = bp[i].subtract(bpCol.multiply(lu[i][col]));
						}
					}

					return new ArrayFieldVector<T>(field, bp, false);

				}
			}

			/// <summary>
			/// Solve the linear equation A &times; X = B.
			/// <p>The A matrix is implicit here. It is </p> </summary>
			/// <param name="b"> right-hand side of the equation A &times; X = B </param>
			/// <returns> a vector X such that A &times; X = B </returns>
			/// <exception cref="DimensionMismatchException"> if the matrices dimensions do not match. </exception>
			/// <exception cref="SingularMatrixException"> if the decomposed matrix is singular. </exception>
			public virtual ArrayFieldVector<T> solve(ArrayFieldVector<T> b)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = pivot.length;
				int m = pivot.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = b.getDimension();
				int length = b.Dimension;
				if (length != m)
				{
					throw new DimensionMismatchException(length, m);
				}
				if (singular)
				{
					throw new SingularMatrixException();
				}

				// Apply permutations to b
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] bp = org.apache.commons.math3.util.MathArrays.buildArray(field, m);
				T[] bp = MathArrays.buildArray(field, m);
				for (int row = 0; row < m; row++)
				{
					bp[row] = b.getEntry(pivot[row]);
				}

				// Solve LY = b
				for (int col = 0; col < m; col++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T bpCol = bp[col];
					T bpCol = bp[col];
					for (int i = col + 1; i < m; i++)
					{
						bp[i] = bp[i].subtract(bpCol.multiply(lu[i][col]));
					}
				}

				// Solve UX = Y
				for (int col = m - 1; col >= 0; col--)
				{
					bp[col] = bp[col].divide(lu[col][col]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T bpCol = bp[col];
					T bpCol = bp[col];
					for (int i = 0; i < col; i++)
					{
						bp[i] = bp[i].subtract(bpCol.multiply(lu[i][col]));
					}
				}

				return new ArrayFieldVector<T>(bp, false);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual FieldMatrix<T> solve(FieldMatrix<T> b)
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
//ORIGINAL LINE: final T[][] bp = org.apache.commons.math3.util.MathArrays.buildArray(field, m, nColB);
				T[][] bp = MathArrays.buildArray(field, m, nColB);
				for (int row = 0; row < m; row++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] bpRow = bp[row];
					T[] bpRow = bp[row];
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
//ORIGINAL LINE: final T[] bpCol = bp[col];
					T[] bpCol = bp[col];
					for (int i = col + 1; i < m; i++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] bpI = bp[i];
						T[] bpI = bp[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T luICol = lu[i][col];
						T luICol = lu[i][col];
						for (int j = 0; j < nColB; j++)
						{
							bpI[j] = bpI[j].subtract(bpCol[j].multiply(luICol));
						}
					}
				}

				// Solve UX = Y
				for (int col = m - 1; col >= 0; col--)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] bpCol = bp[col];
					T[] bpCol = bp[col];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T luDiag = lu[col][col];
					T luDiag = lu[col][col];
					for (int j = 0; j < nColB; j++)
					{
						bpCol[j] = bpCol[j].divide(luDiag);
					}
					for (int i = 0; i < col; i++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] bpI = bp[i];
						T[] bpI = bp[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T luICol = lu[i][col];
						T luICol = lu[i][col];
						for (int j = 0; j < nColB; j++)
						{
							bpI[j] = bpI[j].subtract(bpCol[j].multiply(luICol));
						}
					}
				}

				return new Array2DRowFieldMatrix<T>(field, bp, false);

			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual FieldMatrix<T> Inverse
			{
				get
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int m = pivot.length;
					int m = pivot.Length;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T one = field.getOne();
					T one = field.One;
					FieldMatrix<T> identity = new Array2DRowFieldMatrix<T>(field, m, m);
					for (int i = 0; i < m; ++i)
					{
						identity.setEntry(i, i, one);
					}
					return solve(identity);
				}
			}
		}
	}

}
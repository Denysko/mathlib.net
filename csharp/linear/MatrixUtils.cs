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
	using MathArithmeticException = org.apache.commons.math3.exception.MathArithmeticException;
	using NoDataException = org.apache.commons.math3.exception.NoDataException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using ZeroException = org.apache.commons.math3.exception.ZeroException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using BigFraction = org.apache.commons.math3.fraction.BigFraction;
	using Fraction = org.apache.commons.math3.fraction.Fraction;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathArrays = org.apache.commons.math3.util.MathArrays;
	using MathUtils = org.apache.commons.math3.util.MathUtils;
	using Precision = org.apache.commons.math3.util.Precision;

	/// <summary>
	/// A collection of static methods that operate on or return matrices.
	/// 
	/// @version $Id: MatrixUtils.java 1533638 2013-10-18 21:19:18Z tn $
	/// </summary>
	public class MatrixUtils
	{

		/// <summary>
		/// The default format for <seealso cref="RealMatrix"/> objects.
		/// @since 3.1
		/// </summary>
		public static readonly RealMatrixFormat DEFAULT_FORMAT = RealMatrixFormat.Instance;

		/// <summary>
		/// A format for <seealso cref="RealMatrix"/> objects compatible with octave.
		/// @since 3.1
		/// </summary>
		public static readonly RealMatrixFormat OCTAVE_FORMAT = new RealMatrixFormat("[", "]", "", "", "; ", ", ");

		/// <summary>
		/// Private constructor.
		/// </summary>
		private MatrixUtils() : base()
		{
		}

		/// <summary>
		/// Returns a <seealso cref="RealMatrix"/> with specified dimensions.
		/// <p>The type of matrix returned depends on the dimension. Below
		/// 2<sup>12</sup> elements (i.e. 4096 elements or 64&times;64 for a
		/// square matrix) which can be stored in a 32kB array, a {@link
		/// Array2DRowRealMatrix} instance is built. Above this threshold a {@link
		/// BlockRealMatrix} instance is built.</p>
		/// <p>The matrix elements are all set to 0.0.</p> </summary>
		/// <param name="rows"> number of rows of the matrix </param>
		/// <param name="columns"> number of columns of the matrix </param>
		/// <returns>  RealMatrix with specified dimensions </returns>
		/// <seealso cref= #createRealMatrix(double[][]) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static RealMatrix createRealMatrix(final int rows, final int columns)
		public static RealMatrix createRealMatrix(int rows, int columns)
		{
			return (rows * columns <= 4096) ? new Array2DRowRealMatrix(rows, columns) : new BlockRealMatrix(rows, columns);
		}

		/// <summary>
		/// Returns a <seealso cref="FieldMatrix"/> with specified dimensions.
		/// <p>The type of matrix returned depends on the dimension. Below
		/// 2<sup>12</sup> elements (i.e. 4096 elements or 64&times;64 for a
		/// square matrix), a <seealso cref="FieldMatrix"/> instance is built. Above
		/// this threshold a <seealso cref="BlockFieldMatrix"/> instance is built.</p>
		/// <p>The matrix elements are all set to field.getZero().</p> </summary>
		/// @param <T> the type of the field elements </param>
		/// <param name="field"> field to which the matrix elements belong </param>
		/// <param name="rows"> number of rows of the matrix </param>
		/// <param name="columns"> number of columns of the matrix </param>
		/// <returns>  FieldMatrix with specified dimensions </returns>
		/// <seealso cref= #createFieldMatrix(FieldElement[][])
		/// @since 2.0 </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends org.apache.commons.math3.FieldElement<T>> FieldMatrix<T> createFieldMatrix(final org.apache.commons.math3.Field<T> field, final int rows, final int columns)
		public static FieldMatrix<T> createFieldMatrix<T>(Field<T> field, int rows, int columns) where T : org.apache.commons.math3.FieldElement<T>
		{
			return (rows * columns <= 4096) ? new Array2DRowFieldMatrix<T>(field, rows, columns) : new BlockFieldMatrix<T>(field, rows, columns);
		}

		/// <summary>
		/// Returns a <seealso cref="RealMatrix"/> whose entries are the the values in the
		/// the input array.
		/// <p>The type of matrix returned depends on the dimension. Below
		/// 2<sup>12</sup> elements (i.e. 4096 elements or 64&times;64 for a
		/// square matrix) which can be stored in a 32kB array, a {@link
		/// Array2DRowRealMatrix} instance is built. Above this threshold a {@link
		/// BlockRealMatrix} instance is built.</p>
		/// <p>The input array is copied, not referenced.</p>
		/// </summary>
		/// <param name="data"> input array </param>
		/// <returns>  RealMatrix containing the values of the array </returns>
		/// <exception cref="org.apache.commons.math3.exception.DimensionMismatchException">
		/// if {@code data} is not rectangular (not all rows have the same length). </exception>
		/// <exception cref="NoDataException"> if a row or column is empty. </exception>
		/// <exception cref="NullArgumentException"> if either {@code data} or {@code data[0]}
		/// is {@code null}. </exception>
		/// <exception cref="DimensionMismatchException"> if {@code data} is not rectangular. </exception>
		/// <seealso cref= #createRealMatrix(int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static RealMatrix createRealMatrix(double[][] data) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NoDataException
		public static RealMatrix createRealMatrix(double[][] data)
		{
			if (data == null || data[0] == null)
			{
				throw new NullArgumentException();
			}
			return (data.Length * data[0].Length <= 4096) ? new Array2DRowRealMatrix(data) : new BlockRealMatrix(data);
		}

		/// <summary>
		/// Returns a <seealso cref="FieldMatrix"/> whose entries are the the values in the
		/// the input array.
		/// <p>The type of matrix returned depends on the dimension. Below
		/// 2<sup>12</sup> elements (i.e. 4096 elements or 64&times;64 for a
		/// square matrix), a <seealso cref="FieldMatrix"/> instance is built. Above
		/// this threshold a <seealso cref="BlockFieldMatrix"/> instance is built.</p>
		/// <p>The input array is copied, not referenced.</p> </summary>
		/// @param <T> the type of the field elements </param>
		/// <param name="data"> input array </param>
		/// <returns> a matrix containing the values of the array. </returns>
		/// <exception cref="org.apache.commons.math3.exception.DimensionMismatchException">
		/// if {@code data} is not rectangular (not all rows have the same length). </exception>
		/// <exception cref="NoDataException"> if a row or column is empty. </exception>
		/// <exception cref="NullArgumentException"> if either {@code data} or {@code data[0]}
		/// is {@code null}. </exception>
		/// <seealso cref= #createFieldMatrix(Field, int, int)
		/// @since 2.0 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <T extends org.apache.commons.math3.FieldElement<T>> FieldMatrix<T> createFieldMatrix(T[][] data) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NullArgumentException
		public static FieldMatrix<T> createFieldMatrix<T>(T[][] data) where T : org.apache.commons.math3.FieldElement<T>
		{
			if (data == null || data[0] == null)
			{
				throw new NullArgumentException();
			}
			return (data.Length * data[0].Length <= 4096) ? new Array2DRowFieldMatrix<T>(data) : new BlockFieldMatrix<T>(data);
		}

		/// <summary>
		/// Returns <code>dimension x dimension</code> identity matrix.
		/// </summary>
		/// <param name="dimension"> dimension of identity matrix to generate </param>
		/// <returns> identity matrix </returns>
		/// <exception cref="IllegalArgumentException"> if dimension is not positive
		/// @since 1.1 </exception>
		public static RealMatrix createRealIdentityMatrix(int dimension)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix m = createRealMatrix(dimension, dimension);
			RealMatrix m = createRealMatrix(dimension, dimension);
			for (int i = 0; i < dimension; ++i)
			{
				m.setEntry(i, i, 1.0);
			}
			return m;
		}

		/// <summary>
		/// Returns <code>dimension x dimension</code> identity matrix.
		/// </summary>
		/// @param <T> the type of the field elements </param>
		/// <param name="field"> field to which the elements belong </param>
		/// <param name="dimension"> dimension of identity matrix to generate </param>
		/// <returns> identity matrix </returns>
		/// <exception cref="IllegalArgumentException"> if dimension is not positive
		/// @since 2.0 </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends org.apache.commons.math3.FieldElement<T>> FieldMatrix<T> createFieldIdentityMatrix(final org.apache.commons.math3.Field<T> field, final int dimension)
		public static FieldMatrix<T> createFieldIdentityMatrix<T>(Field<T> field, int dimension) where T : org.apache.commons.math3.FieldElement<T>
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T zero = field.getZero();
			T zero = field.Zero;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T one = field.getOne();
			T one = field.One;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[][] d = org.apache.commons.math3.util.MathArrays.buildArray(field, dimension, dimension);
			T[][] d = MathArrays.buildArray(field, dimension, dimension);
			for (int row = 0; row < dimension; row++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] dRow = d[row];
				T[] dRow = d[row];
				Arrays.fill(dRow, zero);
				dRow[row] = one;
			}
			return new Array2DRowFieldMatrix<T>(field, d, false);
		}

		/// <summary>
		/// Returns a diagonal matrix with specified elements.
		/// </summary>
		/// <param name="diagonal"> diagonal elements of the matrix (the array elements
		/// will be copied) </param>
		/// <returns> diagonal matrix
		/// @since 2.0 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static RealMatrix createRealDiagonalMatrix(final double[] diagonal)
		public static RealMatrix createRealDiagonalMatrix(double[] diagonal)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix m = createRealMatrix(diagonal.length, diagonal.length);
			RealMatrix m = createRealMatrix(diagonal.Length, diagonal.Length);
			for (int i = 0; i < diagonal.Length; ++i)
			{
				m.setEntry(i, i, diagonal[i]);
			}
			return m;
		}

		/// <summary>
		/// Returns a diagonal matrix with specified elements.
		/// </summary>
		/// @param <T> the type of the field elements </param>
		/// <param name="diagonal"> diagonal elements of the matrix (the array elements
		/// will be copied) </param>
		/// <returns> diagonal matrix
		/// @since 2.0 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends org.apache.commons.math3.FieldElement<T>> FieldMatrix<T> createFieldDiagonalMatrix(final T[] diagonal)
		public static FieldMatrix<T> createFieldDiagonalMatrix<T>(T[] diagonal) where T : org.apache.commons.math3.FieldElement<T>
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldMatrix<T> m = createFieldMatrix(diagonal[0].getField(), diagonal.length, diagonal.length);
			FieldMatrix<T> m = createFieldMatrix(diagonal[0].Field, diagonal.Length, diagonal.Length);
			for (int i = 0; i < diagonal.Length; ++i)
			{
				m.setEntry(i, i, diagonal[i]);
			}
			return m;
		}

		/// <summary>
		/// Creates a <seealso cref="RealVector"/> using the data from the input array.
		/// </summary>
		/// <param name="data"> the input data </param>
		/// <returns> a data.length RealVector </returns>
		/// <exception cref="NoDataException"> if {@code data} is empty. </exception>
		/// <exception cref="NullArgumentException"> if {@code data} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static RealVector createRealVector(double[] data) throws org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NullArgumentException
		public static RealVector createRealVector(double[] data)
		{
			if (data == null)
			{
				throw new NullArgumentException();
			}
			return new ArrayRealVector(data, true);
		}

		/// <summary>
		/// Creates a <seealso cref="FieldVector"/> using the data from the input array.
		/// </summary>
		/// @param <T> the type of the field elements </param>
		/// <param name="data"> the input data </param>
		/// <returns> a data.length FieldVector </returns>
		/// <exception cref="NoDataException"> if {@code data} is empty. </exception>
		/// <exception cref="NullArgumentException"> if {@code data} is {@code null}. </exception>
		/// <exception cref="ZeroException"> if {@code data} has 0 elements </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <T extends org.apache.commons.math3.FieldElement<T>> FieldVector<T> createFieldVector(final T[] data) throws org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.ZeroException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static FieldVector<T> createFieldVector<T>(T[] data) where T : org.apache.commons.math3.FieldElement<T>
		{
			if (data == null)
			{
				throw new NullArgumentException();
			}
			if (data.Length == 0)
			{
				throw new ZeroException(LocalizedFormats.VECTOR_MUST_HAVE_AT_LEAST_ONE_ELEMENT);
			}
			return new ArrayFieldVector<T>(data[0].Field, data, true);
		}

		/// <summary>
		/// Create a row <seealso cref="RealMatrix"/> using the data from the input
		/// array.
		/// </summary>
		/// <param name="rowData"> the input row data </param>
		/// <returns> a 1 x rowData.length RealMatrix </returns>
		/// <exception cref="NoDataException"> if {@code rowData} is empty. </exception>
		/// <exception cref="NullArgumentException"> if {@code rowData} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static RealMatrix createRowRealMatrix(double[] rowData) throws org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NullArgumentException
		public static RealMatrix createRowRealMatrix(double[] rowData)
		{
			if (rowData == null)
			{
				throw new NullArgumentException();
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = rowData.length;
			int nCols = rowData.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix m = createRealMatrix(1, nCols);
			RealMatrix m = createRealMatrix(1, nCols);
			for (int i = 0; i < nCols; ++i)
			{
				m.setEntry(0, i, rowData[i]);
			}
			return m;
		}

		/// <summary>
		/// Create a row <seealso cref="FieldMatrix"/> using the data from the input
		/// array.
		/// </summary>
		/// @param <T> the type of the field elements </param>
		/// <param name="rowData"> the input row data </param>
		/// <returns> a 1 x rowData.length FieldMatrix </returns>
		/// <exception cref="NoDataException"> if {@code rowData} is empty. </exception>
		/// <exception cref="NullArgumentException"> if {@code rowData} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <T extends org.apache.commons.math3.FieldElement<T>> FieldMatrix<T> createRowFieldMatrix(final T[] rowData) throws org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static FieldMatrix<T> createRowFieldMatrix<T>(T[] rowData) where T : org.apache.commons.math3.FieldElement<T>
		{
			if (rowData == null)
			{
				throw new NullArgumentException();
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = rowData.length;
			int nCols = rowData.Length;
			if (nCols == 0)
			{
				throw new NoDataException(LocalizedFormats.AT_LEAST_ONE_COLUMN);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldMatrix<T> m = createFieldMatrix(rowData[0].getField(), 1, nCols);
			FieldMatrix<T> m = createFieldMatrix(rowData[0].Field, 1, nCols);
			for (int i = 0; i < nCols; ++i)
			{
				m.setEntry(0, i, rowData[i]);
			}
			return m;
		}

		/// <summary>
		/// Creates a column <seealso cref="RealMatrix"/> using the data from the input
		/// array.
		/// </summary>
		/// <param name="columnData">  the input column data </param>
		/// <returns> a columnData x 1 RealMatrix </returns>
		/// <exception cref="NoDataException"> if {@code columnData} is empty. </exception>
		/// <exception cref="NullArgumentException"> if {@code columnData} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static RealMatrix createColumnRealMatrix(double[] columnData) throws org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NullArgumentException
		public static RealMatrix createColumnRealMatrix(double[] columnData)
		{
			if (columnData == null)
			{
				throw new NullArgumentException();
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = columnData.length;
			int nRows = columnData.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix m = createRealMatrix(nRows, 1);
			RealMatrix m = createRealMatrix(nRows, 1);
			for (int i = 0; i < nRows; ++i)
			{
				m.setEntry(i, 0, columnData[i]);
			}
			return m;
		}

		/// <summary>
		/// Creates a column <seealso cref="FieldMatrix"/> using the data from the input
		/// array.
		/// </summary>
		/// @param <T> the type of the field elements </param>
		/// <param name="columnData">  the input column data </param>
		/// <returns> a columnData x 1 FieldMatrix </returns>
		/// <exception cref="NoDataException"> if {@code data} is empty. </exception>
		/// <exception cref="NullArgumentException"> if {@code columnData} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <T extends org.apache.commons.math3.FieldElement<T>> FieldMatrix<T> createColumnFieldMatrix(final T[] columnData) throws org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static FieldMatrix<T> createColumnFieldMatrix<T>(T[] columnData) where T : org.apache.commons.math3.FieldElement<T>
		{
			if (columnData == null)
			{
				throw new NullArgumentException();
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = columnData.length;
			int nRows = columnData.Length;
			if (nRows == 0)
			{
				throw new NoDataException(LocalizedFormats.AT_LEAST_ONE_ROW);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldMatrix<T> m = createFieldMatrix(columnData[0].getField(), nRows, 1);
			FieldMatrix<T> m = createFieldMatrix(columnData[0].Field, nRows, 1);
			for (int i = 0; i < nRows; ++i)
			{
				m.setEntry(i, 0, columnData[i]);
			}
			return m;
		}

		/// <summary>
		/// Checks whether a matrix is symmetric, within a given relative tolerance.
		/// </summary>
		/// <param name="matrix"> Matrix to check. </param>
		/// <param name="relativeTolerance"> Tolerance of the symmetry check. </param>
		/// <param name="raiseException"> If {@code true}, an exception will be raised if
		/// the matrix is not symmetric. </param>
		/// <returns> {@code true} if {@code matrix} is symmetric. </returns>
		/// <exception cref="NonSquareMatrixException"> if the matrix is not square. </exception>
		/// <exception cref="NonSymmetricMatrixException"> if the matrix is not symmetric. </exception>
		private static bool isSymmetricInternal(RealMatrix matrix, double relativeTolerance, bool raiseException)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rows = matrix.getRowDimension();
			int rows = matrix.RowDimension;
			if (rows != matrix.ColumnDimension)
			{
				if (raiseException)
				{
					throw new NonSquareMatrixException(rows, matrix.ColumnDimension);
				}
				else
				{
					return false;
				}
			}
			for (int i = 0; i < rows; i++)
			{
				for (int j = i + 1; j < rows; j++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double mij = matrix.getEntry(i, j);
					double mij = matrix.getEntry(i, j);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double mji = matrix.getEntry(j, i);
					double mji = matrix.getEntry(j, i);
					if (FastMath.abs(mij - mji) > FastMath.max(FastMath.abs(mij), FastMath.abs(mji)) * relativeTolerance)
					{
						if (raiseException)
						{
							throw new NonSymmetricMatrixException(i, j, relativeTolerance);
						}
						else
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Checks whether a matrix is symmetric.
		/// </summary>
		/// <param name="matrix"> Matrix to check. </param>
		/// <param name="eps"> Relative tolerance. </param>
		/// <exception cref="NonSquareMatrixException"> if the matrix is not square. </exception>
		/// <exception cref="NonSymmetricMatrixException"> if the matrix is not symmetric.
		/// @since 3.1 </exception>
		public static void checkSymmetric(RealMatrix matrix, double eps)
		{
			isSymmetricInternal(matrix, eps, true);
		}

		/// <summary>
		/// Checks whether a matrix is symmetric.
		/// </summary>
		/// <param name="matrix"> Matrix to check. </param>
		/// <param name="eps"> Relative tolerance. </param>
		/// <returns> {@code true} if {@code matrix} is symmetric.
		/// @since 3.1 </returns>
		public static bool isSymmetric(RealMatrix matrix, double eps)
		{
			return isSymmetricInternal(matrix, eps, false);
		}

		/// <summary>
		/// Check if matrix indices are valid.
		/// </summary>
		/// <param name="m"> Matrix. </param>
		/// <param name="row"> Row index to check. </param>
		/// <param name="column"> Column index to check. </param>
		/// <exception cref="OutOfRangeException"> if {@code row} or {@code column} is not
		/// a valid index. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void checkMatrixIndex(final AnyMatrix m, final int row, final int column) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void checkMatrixIndex(AnyMatrix m, int row, int column)
		{
			checkRowIndex(m, row);
			checkColumnIndex(m, column);
		}

		/// <summary>
		/// Check if a row index is valid.
		/// </summary>
		/// <param name="m"> Matrix. </param>
		/// <param name="row"> Row index to check. </param>
		/// <exception cref="OutOfRangeException"> if {@code row} is not a valid index. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void checkRowIndex(final AnyMatrix m, final int row) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void checkRowIndex(AnyMatrix m, int row)
		{
			if (row < 0 || row >= m.RowDimension)
			{
				throw new OutOfRangeException(LocalizedFormats.ROW_INDEX, row, 0, m.RowDimension - 1);
			}
		}

		/// <summary>
		/// Check if a column index is valid.
		/// </summary>
		/// <param name="m"> Matrix. </param>
		/// <param name="column"> Column index to check. </param>
		/// <exception cref="OutOfRangeException"> if {@code column} is not a valid index. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void checkColumnIndex(final AnyMatrix m, final int column) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void checkColumnIndex(AnyMatrix m, int column)
		{
			if (column < 0 || column >= m.ColumnDimension)
			{
				throw new OutOfRangeException(LocalizedFormats.COLUMN_INDEX, column, 0, m.ColumnDimension - 1);
			}
		}

		/// <summary>
		/// Check if submatrix ranges indices are valid.
		/// Rows and columns are indicated counting from 0 to {@code n - 1}.
		/// </summary>
		/// <param name="m"> Matrix. </param>
		/// <param name="startRow"> Initial row index. </param>
		/// <param name="endRow"> Final row index. </param>
		/// <param name="startColumn"> Initial column index. </param>
		/// <param name="endColumn"> Final column index. </param>
		/// <exception cref="OutOfRangeException"> if the indices are invalid. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code endRow < startRow} or
		/// {@code endColumn < startColumn}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void checkSubMatrixIndex(final AnyMatrix m, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void checkSubMatrixIndex(AnyMatrix m, int startRow, int endRow, int startColumn, int endColumn)
		{
			checkRowIndex(m, startRow);
			checkRowIndex(m, endRow);
			if (endRow < startRow)
			{
				throw new NumberIsTooSmallException(LocalizedFormats.INITIAL_ROW_AFTER_FINAL_ROW, endRow, startRow, false);
			}

			checkColumnIndex(m, startColumn);
			checkColumnIndex(m, endColumn);
			if (endColumn < startColumn)
			{
				throw new NumberIsTooSmallException(LocalizedFormats.INITIAL_COLUMN_AFTER_FINAL_COLUMN, endColumn, startColumn, false);
			}


		}

		/// <summary>
		/// Check if submatrix ranges indices are valid.
		/// Rows and columns are indicated counting from 0 to n-1.
		/// </summary>
		/// <param name="m"> Matrix. </param>
		/// <param name="selectedRows"> Array of row indices. </param>
		/// <param name="selectedColumns"> Array of column indices. </param>
		/// <exception cref="NullArgumentException"> if {@code selectedRows} or
		/// {@code selectedColumns} are {@code null}. </exception>
		/// <exception cref="NoDataException"> if the row or column selections are empty (zero
		/// length). </exception>
		/// <exception cref="OutOfRangeException"> if row or column selections are not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void checkSubMatrixIndex(final AnyMatrix m, final int[] selectedRows, final int[] selectedColumns) throws org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void checkSubMatrixIndex(AnyMatrix m, int[] selectedRows, int[] selectedColumns)
		{
			if (selectedRows == null)
			{
				throw new NullArgumentException();
			}
			if (selectedColumns == null)
			{
				throw new NullArgumentException();
			}
			if (selectedRows.Length == 0)
			{
				throw new NoDataException(LocalizedFormats.EMPTY_SELECTED_ROW_INDEX_ARRAY);
			}
			if (selectedColumns.Length == 0)
			{
				throw new NoDataException(LocalizedFormats.EMPTY_SELECTED_COLUMN_INDEX_ARRAY);
			}

			foreach (int row in selectedRows)
			{
				checkRowIndex(m, row);
			}
			foreach (int column in selectedColumns)
			{
				checkColumnIndex(m, column);
			}
		}

		/// <summary>
		/// Check if matrices are addition compatible.
		/// </summary>
		/// <param name="left"> Left hand side matrix. </param>
		/// <param name="right"> Right hand side matrix. </param>
		/// <exception cref="MatrixDimensionMismatchException"> if the matrices are not addition
		/// compatible. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void checkAdditionCompatible(final AnyMatrix left, final AnyMatrix right) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void checkAdditionCompatible(AnyMatrix left, AnyMatrix right)
		{
			if ((left.RowDimension != right.RowDimension) || (left.ColumnDimension != right.ColumnDimension))
			{
				throw new MatrixDimensionMismatchException(left.RowDimension, left.ColumnDimension, right.RowDimension, right.ColumnDimension);
			}
		}

		/// <summary>
		/// Check if matrices are subtraction compatible
		/// </summary>
		/// <param name="left"> Left hand side matrix. </param>
		/// <param name="right"> Right hand side matrix. </param>
		/// <exception cref="MatrixDimensionMismatchException"> if the matrices are not addition
		/// compatible. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void checkSubtractionCompatible(final AnyMatrix left, final AnyMatrix right) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void checkSubtractionCompatible(AnyMatrix left, AnyMatrix right)
		{
			if ((left.RowDimension != right.RowDimension) || (left.ColumnDimension != right.ColumnDimension))
			{
				throw new MatrixDimensionMismatchException(left.RowDimension, left.ColumnDimension, right.RowDimension, right.ColumnDimension);
			}
		}

		/// <summary>
		/// Check if matrices are multiplication compatible
		/// </summary>
		/// <param name="left"> Left hand side matrix. </param>
		/// <param name="right"> Right hand side matrix. </param>
		/// <exception cref="DimensionMismatchException"> if matrices are not multiplication
		/// compatible. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void checkMultiplicationCompatible(final AnyMatrix left, final AnyMatrix right) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void checkMultiplicationCompatible(AnyMatrix left, AnyMatrix right)
		{

			if (left.ColumnDimension != right.RowDimension)
			{
				throw new DimensionMismatchException(left.ColumnDimension, right.RowDimension);
			}
		}

		/// <summary>
		/// Convert a <seealso cref="FieldMatrix"/>/<seealso cref="Fraction"/> matrix to a <seealso cref="RealMatrix"/>. </summary>
		/// <param name="m"> Matrix to convert. </param>
		/// <returns> the converted matrix. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Array2DRowRealMatrix fractionMatrixToRealMatrix(final FieldMatrix<org.apache.commons.math3.fraction.Fraction> m)
		public static Array2DRowRealMatrix fractionMatrixToRealMatrix(FieldMatrix<Fraction> m)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FractionMatrixConverter converter = new FractionMatrixConverter();
			FractionMatrixConverter converter = new FractionMatrixConverter();
			m.walkInOptimizedOrder(converter);
			return converter.ConvertedMatrix;
		}

		/// <summary>
		/// Converter for <seealso cref="FieldMatrix"/>/<seealso cref="Fraction"/>. </summary>
		private class FractionMatrixConverter : DefaultFieldMatrixPreservingVisitor<Fraction>
		{
			/// <summary>
			/// Converted array. </summary>
			internal double[][] data;
			/// <summary>
			/// Simple constructor. </summary>
			public FractionMatrixConverter() : base(Fraction.ZERO)
			{
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override void start(int rows, int columns, int startRow, int endRow, int startColumn, int endColumn)
			{
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: data = new double[rows][columns];
				data = RectangularArrays.ReturnRectangularDoubleArray(rows, columns);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override void visit(int row, int column, Fraction value)
			{
				data[row][column] = (double)value;
			}

			/// <summary>
			/// Get the converted matrix.
			/// </summary>
			/// <returns> the converted matrix. </returns>
			internal virtual Array2DRowRealMatrix ConvertedMatrix
			{
				get
				{
					return new Array2DRowRealMatrix(data, false);
				}
			}

		}

		/// <summary>
		/// Convert a <seealso cref="FieldMatrix"/>/<seealso cref="BigFraction"/> matrix to a <seealso cref="RealMatrix"/>.
		/// </summary>
		/// <param name="m"> Matrix to convert. </param>
		/// <returns> the converted matrix. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Array2DRowRealMatrix bigFractionMatrixToRealMatrix(final FieldMatrix<org.apache.commons.math3.fraction.BigFraction> m)
		public static Array2DRowRealMatrix bigFractionMatrixToRealMatrix(FieldMatrix<BigFraction> m)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BigFractionMatrixConverter converter = new BigFractionMatrixConverter();
			BigFractionMatrixConverter converter = new BigFractionMatrixConverter();
			m.walkInOptimizedOrder(converter);
			return converter.ConvertedMatrix;
		}

		/// <summary>
		/// Converter for <seealso cref="FieldMatrix"/>/<seealso cref="BigFraction"/>. </summary>
		private class BigFractionMatrixConverter : DefaultFieldMatrixPreservingVisitor<BigFraction>
		{
			/// <summary>
			/// Converted array. </summary>
			internal double[][] data;
			/// <summary>
			/// Simple constructor. </summary>
			public BigFractionMatrixConverter() : base(BigFraction.ZERO)
			{
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override void start(int rows, int columns, int startRow, int endRow, int startColumn, int endColumn)
			{
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: data = new double[rows][columns];
				data = RectangularArrays.ReturnRectangularDoubleArray(rows, columns);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override void visit(int row, int column, BigFraction value)
			{
				data[row][column] = (double)value;
			}

			/// <summary>
			/// Get the converted matrix.
			/// </summary>
			/// <returns> the converted matrix. </returns>
			internal virtual Array2DRowRealMatrix ConvertedMatrix
			{
				get
				{
					return new Array2DRowRealMatrix(data, false);
				}
			}
		}

		/// <summary>
		/// Serialize a <seealso cref="RealVector"/>.
		/// <p>
		/// This method is intended to be called from within a private
		/// <code>writeObject</code> method (after a call to
		/// <code>oos.defaultWriteObject()</code>) in a class that has a
		/// <seealso cref="RealVector"/> field, which should be declared <code>transient</code>.
		/// This way, the default handling does not serialize the vector (the {@link
		/// RealVector} interface is not serializable by default) but this method does
		/// serialize it specifically.
		/// </p>
		/// <p>
		/// The following example shows how a simple class with a name and a real vector
		/// should be written:
		/// <pre><code>
		/// public class NamedVector implements Serializable {
		/// 
		///     private final String name;
		///     private final transient RealVector coefficients;
		/// 
		///     // omitted constructors, getters ...
		/// 
		///     private void writeObject(ObjectOutputStream oos) throws IOException {
		///         oos.defaultWriteObject();  // takes care of name field
		///         MatrixUtils.serializeRealVector(coefficients, oos);
		///     }
		/// 
		///     private void readObject(ObjectInputStream ois) throws ClassNotFoundException, IOException {
		///         ois.defaultReadObject();  // takes care of name field
		///         MatrixUtils.deserializeRealVector(this, "coefficients", ois);
		///     }
		/// 
		/// }
		/// </code></pre>
		/// </p>
		/// </summary>
		/// <param name="vector"> real vector to serialize </param>
		/// <param name="oos"> stream where the real vector should be written </param>
		/// <exception cref="IOException"> if object cannot be written to stream </exception>
		/// <seealso cref= #deserializeRealVector(Object, String, ObjectInputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void serializeRealVector(final RealVector vector, final java.io.ObjectOutputStream oos) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void serializeRealVector(RealVector vector, ObjectOutputStream oos)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = vector.getDimension();
			int n = vector.Dimension;
			oos.writeInt(n);
			for (int i = 0; i < n; ++i)
			{
				oos.writeDouble(vector.getEntry(i));
			}
		}

		/// <summary>
		/// Deserialize  a <seealso cref="RealVector"/> field in a class.
		/// <p>
		/// This method is intended to be called from within a private
		/// <code>readObject</code> method (after a call to
		/// <code>ois.defaultReadObject()</code>) in a class that has a
		/// <seealso cref="RealVector"/> field, which should be declared <code>transient</code>.
		/// This way, the default handling does not deserialize the vector (the {@link
		/// RealVector} interface is not serializable by default) but this method does
		/// deserialize it specifically.
		/// </p> </summary>
		/// <param name="instance"> instance in which the field must be set up </param>
		/// <param name="fieldName"> name of the field within the class (may be private and final) </param>
		/// <param name="ois"> stream from which the real vector should be read </param>
		/// <exception cref="ClassNotFoundException"> if a class in the stream cannot be found </exception>
		/// <exception cref="IOException"> if object cannot be read from the stream </exception>
		/// <seealso cref= #serializeRealVector(RealVector, ObjectOutputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void deserializeRealVector(final Object instance, final String fieldName, final java.io.ObjectInputStream ois) throws ClassNotFoundException, java.io.IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void deserializeRealVector(object instance, string fieldName, ObjectInputStream ois)
		{
			try
			{

				// read the vector data
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = ois.readInt();
				int n = ois.readInt();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] data = new double[n];
				double[] data = new double[n];
				for (int i = 0; i < n; ++i)
				{
					data[i] = ois.readDouble();
				}

				// create the instance
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector vector = new ArrayRealVector(data, false);
				RealVector vector = new ArrayRealVector(data, false);

				// set up the field
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.lang.reflect.Field f = instance.getClass().getDeclaredField(fieldName);
				System.Reflection.FieldInfo f = instance.GetType().getDeclaredField(fieldName);
				f.Accessible = true;
				f.set(instance, vector);

			}
			catch (NoSuchFieldException nsfe)
			{
				IOException ioe = new IOException();
				ioe.initCause(nsfe);
				throw ioe;
			}
			catch (IllegalAccessException iae)
			{
				IOException ioe = new IOException();
				ioe.initCause(iae);
				throw ioe;
			}

		}

		/// <summary>
		/// Serialize a <seealso cref="RealMatrix"/>.
		/// <p>
		/// This method is intended to be called from within a private
		/// <code>writeObject</code> method (after a call to
		/// <code>oos.defaultWriteObject()</code>) in a class that has a
		/// <seealso cref="RealMatrix"/> field, which should be declared <code>transient</code>.
		/// This way, the default handling does not serialize the matrix (the {@link
		/// RealMatrix} interface is not serializable by default) but this method does
		/// serialize it specifically.
		/// </p>
		/// <p>
		/// The following example shows how a simple class with a name and a real matrix
		/// should be written:
		/// <pre><code>
		/// public class NamedMatrix implements Serializable {
		/// 
		///     private final String name;
		///     private final transient RealMatrix coefficients;
		/// 
		///     // omitted constructors, getters ...
		/// 
		///     private void writeObject(ObjectOutputStream oos) throws IOException {
		///         oos.defaultWriteObject();  // takes care of name field
		///         MatrixUtils.serializeRealMatrix(coefficients, oos);
		///     }
		/// 
		///     private void readObject(ObjectInputStream ois) throws ClassNotFoundException, IOException {
		///         ois.defaultReadObject();  // takes care of name field
		///         MatrixUtils.deserializeRealMatrix(this, "coefficients", ois);
		///     }
		/// 
		/// }
		/// </code></pre>
		/// </p>
		/// </summary>
		/// <param name="matrix"> real matrix to serialize </param>
		/// <param name="oos"> stream where the real matrix should be written </param>
		/// <exception cref="IOException"> if object cannot be written to stream </exception>
		/// <seealso cref= #deserializeRealMatrix(Object, String, ObjectInputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void serializeRealMatrix(final RealMatrix matrix, final java.io.ObjectOutputStream oos) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void serializeRealMatrix(RealMatrix matrix, ObjectOutputStream oos)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = matrix.getRowDimension();
			int n = matrix.RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = matrix.getColumnDimension();
			int m = matrix.ColumnDimension;
			oos.writeInt(n);
			oos.writeInt(m);
			for (int i = 0; i < n; ++i)
			{
				for (int j = 0; j < m; ++j)
				{
					oos.writeDouble(matrix.getEntry(i, j));
				}
			}
		}

		/// <summary>
		/// Deserialize  a <seealso cref="RealMatrix"/> field in a class.
		/// <p>
		/// This method is intended to be called from within a private
		/// <code>readObject</code> method (after a call to
		/// <code>ois.defaultReadObject()</code>) in a class that has a
		/// <seealso cref="RealMatrix"/> field, which should be declared <code>transient</code>.
		/// This way, the default handling does not deserialize the matrix (the {@link
		/// RealMatrix} interface is not serializable by default) but this method does
		/// deserialize it specifically.
		/// </p> </summary>
		/// <param name="instance"> instance in which the field must be set up </param>
		/// <param name="fieldName"> name of the field within the class (may be private and final) </param>
		/// <param name="ois"> stream from which the real matrix should be read </param>
		/// <exception cref="ClassNotFoundException"> if a class in the stream cannot be found </exception>
		/// <exception cref="IOException"> if object cannot be read from the stream </exception>
		/// <seealso cref= #serializeRealMatrix(RealMatrix, ObjectOutputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void deserializeRealMatrix(final Object instance, final String fieldName, final java.io.ObjectInputStream ois) throws ClassNotFoundException, java.io.IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void deserializeRealMatrix(object instance, string fieldName, ObjectInputStream ois)
		{
			try
			{

				// read the matrix data
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = ois.readInt();
				int n = ois.readInt();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = ois.readInt();
				int m = ois.readInt();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] data = new double[n][m];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] data = new double[n][m];
				double[][] data = RectangularArrays.ReturnRectangularDoubleArray(n, m);
				for (int i = 0; i < n; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dataI = data[i];
					double[] dataI = data[i];
					for (int j = 0; j < m; ++j)
					{
						dataI[j] = ois.readDouble();
					}
				}

				// create the instance
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix matrix = new Array2DRowRealMatrix(data, false);
				RealMatrix matrix = new Array2DRowRealMatrix(data, false);

				// set up the field
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.lang.reflect.Field f = instance.getClass().getDeclaredField(fieldName);
				System.Reflection.FieldInfo f = instance.GetType().getDeclaredField(fieldName);
				f.Accessible = true;
				f.set(instance, matrix);

			}
			catch (NoSuchFieldException nsfe)
			{
				IOException ioe = new IOException();
				ioe.initCause(nsfe);
				throw ioe;
			}
			catch (IllegalAccessException iae)
			{
				IOException ioe = new IOException();
				ioe.initCause(iae);
				throw ioe;
			}
		}

		/// <summary>
		///Solve  a  system of composed of a Lower Triangular Matrix
		/// <seealso cref="RealMatrix"/>.
		/// <p>
		/// This method is called to solve systems of equations which are
		/// of the lower triangular form. The matrix <seealso cref="RealMatrix"/>
		/// is assumed, though not checked, to be in lower triangular form.
		/// The vector <seealso cref="RealVector"/> is overwritten with the solution.
		/// The matrix is checked that it is square and its dimensions match
		/// the length of the vector.
		/// </p> </summary>
		/// <param name="rm"> RealMatrix which is lower triangular </param>
		/// <param name="b">  RealVector this is overwritten </param>
		/// <exception cref="DimensionMismatchException"> if the matrix and vector are not
		/// conformable </exception>
		/// <exception cref="NonSquareMatrixException"> if the matrix {@code rm} is not square </exception>
		/// <exception cref="MathArithmeticException"> if the absolute value of one of the diagonal
		/// coefficient of {@code rm} is lower than <seealso cref="Precision#SAFE_MIN"/> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void solveLowerTriangularSystem(RealMatrix rm, RealVector b) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathArithmeticException, NonSquareMatrixException
		public static void solveLowerTriangularSystem(RealMatrix rm, RealVector b)
		{
			if ((rm == null) || (b == null) || (rm.RowDimension != b.Dimension))
			{
				throw new DimensionMismatchException((rm == null) ? 0 : rm.RowDimension, (b == null) ? 0 : b.Dimension);
			}
			if (rm.ColumnDimension != rm.RowDimension)
			{
				throw new NonSquareMatrixException(rm.RowDimension, rm.ColumnDimension);
			}
			int rows = rm.RowDimension;
			for (int i = 0 ; i < rows ; i++)
			{
				double diag = rm.getEntry(i, i);
				if (FastMath.abs(diag) < Precision.SAFE_MIN)
				{
					throw new MathArithmeticException(LocalizedFormats.ZERO_DENOMINATOR);
				}
				double bi = b.getEntry(i) / diag;
				b.setEntry(i, bi);
				for (int j = i + 1; j < rows; j++)
				{
					b.setEntry(j, b.getEntry(j) - bi * rm.getEntry(j,i));
				}
			}
		}

		/// <summary>
		/// Solver a  system composed  of an Upper Triangular Matrix
		/// <seealso cref="RealMatrix"/>.
		/// <p>
		/// This method is called to solve systems of equations which are
		/// of the lower triangular form. The matrix <seealso cref="RealMatrix"/>
		/// is assumed, though not checked, to be in upper triangular form.
		/// The vector <seealso cref="RealVector"/> is overwritten with the solution.
		/// The matrix is checked that it is square and its dimensions match
		/// the length of the vector.
		/// </p> </summary>
		/// <param name="rm"> RealMatrix which is upper triangular </param>
		/// <param name="b">  RealVector this is overwritten </param>
		/// <exception cref="DimensionMismatchException"> if the matrix and vector are not
		/// conformable </exception>
		/// <exception cref="NonSquareMatrixException"> if the matrix {@code rm} is not
		/// square </exception>
		/// <exception cref="MathArithmeticException"> if the absolute value of one of the diagonal
		/// coefficient of {@code rm} is lower than <seealso cref="Precision#SAFE_MIN"/> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void solveUpperTriangularSystem(RealMatrix rm, RealVector b) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathArithmeticException, NonSquareMatrixException
		public static void solveUpperTriangularSystem(RealMatrix rm, RealVector b)
		{
			if ((rm == null) || (b == null) || (rm.RowDimension != b.Dimension))
			{
				throw new DimensionMismatchException((rm == null) ? 0 : rm.RowDimension, (b == null) ? 0 : b.Dimension);
			}
			if (rm.ColumnDimension != rm.RowDimension)
			{
				throw new NonSquareMatrixException(rm.RowDimension, rm.ColumnDimension);
			}
			int rows = rm.RowDimension;
			for (int i = rows - 1 ; i > -1 ; i--)
			{
				double diag = rm.getEntry(i, i);
				if (FastMath.abs(diag) < Precision.SAFE_MIN)
				{
					throw new MathArithmeticException(LocalizedFormats.ZERO_DENOMINATOR);
				}
				double bi = b.getEntry(i) / diag;
				b.setEntry(i, bi);
				for (int j = i - 1; j > -1; j--)
				{
					b.setEntry(j, b.getEntry(j) - bi * rm.getEntry(j,i));
				}
			}
		}

		/// <summary>
		/// Computes the inverse of the given matrix by splitting it into
		/// 4 sub-matrices.
		/// </summary>
		/// <param name="m"> Matrix whose inverse must be computed. </param>
		/// <param name="splitIndex"> Index that determines the "split" line and
		/// column.
		/// The element corresponding to this index will part of the
		/// upper-left sub-matrix. </param>
		/// <returns> the inverse of {@code m}. </returns>
		/// <exception cref="NonSquareMatrixException"> if {@code m} is not square. </exception>
		public static RealMatrix blockInverse(RealMatrix m, int splitIndex)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = m.getRowDimension();
			int n = m.RowDimension;
			if (m.ColumnDimension != n)
			{
				throw new NonSquareMatrixException(m.RowDimension, m.ColumnDimension);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int splitIndex1 = splitIndex + 1;
			int splitIndex1 = splitIndex + 1;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix a = m.getSubMatrix(0, splitIndex, 0, splitIndex);
			RealMatrix a = m.getSubMatrix(0, splitIndex, 0, splitIndex);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix b = m.getSubMatrix(0, splitIndex, splitIndex1, n - 1);
			RealMatrix b = m.getSubMatrix(0, splitIndex, splitIndex1, n - 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix c = m.getSubMatrix(splitIndex1, n - 1, 0, splitIndex);
			RealMatrix c = m.getSubMatrix(splitIndex1, n - 1, 0, splitIndex);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix d = m.getSubMatrix(splitIndex1, n - 1, splitIndex1, n - 1);
			RealMatrix d = m.getSubMatrix(splitIndex1, n - 1, splitIndex1, n - 1);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SingularValueDecomposition aDec = new SingularValueDecomposition(a);
			SingularValueDecomposition aDec = new SingularValueDecomposition(a);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DecompositionSolver aSolver = aDec.getSolver();
			DecompositionSolver aSolver = aDec.Solver;
			if (!aSolver.NonSingular)
			{
				throw new SingularMatrixException();
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix aInv = aSolver.getInverse();
			RealMatrix aInv = aSolver.Inverse;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SingularValueDecomposition dDec = new SingularValueDecomposition(d);
			SingularValueDecomposition dDec = new SingularValueDecomposition(d);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DecompositionSolver dSolver = dDec.getSolver();
			DecompositionSolver dSolver = dDec.Solver;
			if (!dSolver.NonSingular)
			{
				throw new SingularMatrixException();
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix dInv = dSolver.getInverse();
			RealMatrix dInv = dSolver.Inverse;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix tmp1 = a.subtract(b.multiply(dInv).multiply(c));
			RealMatrix tmp1 = a.subtract(b.multiply(dInv).multiply(c));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SingularValueDecomposition tmp1Dec = new SingularValueDecomposition(tmp1);
			SingularValueDecomposition tmp1Dec = new SingularValueDecomposition(tmp1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DecompositionSolver tmp1Solver = tmp1Dec.getSolver();
			DecompositionSolver tmp1Solver = tmp1Dec.Solver;
			if (!tmp1Solver.NonSingular)
			{
				throw new SingularMatrixException();
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix result00 = tmp1Solver.getInverse();
			RealMatrix result00 = tmp1Solver.Inverse;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix tmp2 = d.subtract(c.multiply(aInv).multiply(b));
			RealMatrix tmp2 = d.subtract(c.multiply(aInv).multiply(b));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SingularValueDecomposition tmp2Dec = new SingularValueDecomposition(tmp2);
			SingularValueDecomposition tmp2Dec = new SingularValueDecomposition(tmp2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DecompositionSolver tmp2Solver = tmp2Dec.getSolver();
			DecompositionSolver tmp2Solver = tmp2Dec.Solver;
			if (!tmp2Solver.NonSingular)
			{
				throw new SingularMatrixException();
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix result11 = tmp2Solver.getInverse();
			RealMatrix result11 = tmp2Solver.Inverse;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix result01 = aInv.multiply(b).multiply(result11).scalarMultiply(-1);
			RealMatrix result01 = aInv.multiply(b).multiply(result11).scalarMultiply(-1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix result10 = dInv.multiply(c).multiply(result00).scalarMultiply(-1);
			RealMatrix result10 = dInv.multiply(c).multiply(result00).scalarMultiply(-1);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix result = new Array2DRowRealMatrix(n, n);
			RealMatrix result = new Array2DRowRealMatrix(n, n);
			result.setSubMatrix(result00.Data, 0, 0);
			result.setSubMatrix(result01.Data, 0, splitIndex1);
			result.setSubMatrix(result10.Data, splitIndex1, 0);
			result.setSubMatrix(result11.Data, splitIndex1, splitIndex1);

			return result;
		}

		/// <summary>
		/// Computes the inverse of the given matrix.
		/// <p>
		/// By default, the inverse of the matrix is computed using the QR-decomposition,
		/// unless a more efficient method can be determined for the input matrix.
		/// <p>
		/// Note: this method will use a singularity threshold of 0,
		/// use <seealso cref="#inverse(RealMatrix, double)"/> if a different threshold is needed.
		/// </summary>
		/// <param name="matrix"> Matrix whose inverse shall be computed </param>
		/// <returns> the inverse of {@code matrix} </returns>
		/// <exception cref="NullArgumentException"> if {@code matrix} is {@code null} </exception>
		/// <exception cref="SingularMatrixException"> if m is singular </exception>
		/// <exception cref="NonSquareMatrixException"> if matrix is not square
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static RealMatrix inverse(RealMatrix matrix) throws org.apache.commons.math3.exception.NullArgumentException, SingularMatrixException, NonSquareMatrixException
		public static RealMatrix inverse(RealMatrix matrix)
		{
			return inverse(matrix, 0);
		}

		/// <summary>
		/// Computes the inverse of the given matrix.
		/// <p>
		/// By default, the inverse of the matrix is computed using the QR-decomposition,
		/// unless a more efficient method can be determined for the input matrix.
		/// </summary>
		/// <param name="matrix"> Matrix whose inverse shall be computed </param>
		/// <param name="threshold"> Singularity threshold </param>
		/// <returns> the inverse of {@code m} </returns>
		/// <exception cref="NullArgumentException"> if {@code matrix} is {@code null} </exception>
		/// <exception cref="SingularMatrixException"> if matrix is singular </exception>
		/// <exception cref="NonSquareMatrixException"> if matrix is not square
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static RealMatrix inverse(RealMatrix matrix, double threshold) throws org.apache.commons.math3.exception.NullArgumentException, SingularMatrixException, NonSquareMatrixException
		public static RealMatrix inverse(RealMatrix matrix, double threshold)
		{

			MathUtils.checkNotNull(matrix);

			if (!matrix.Square)
			{
				throw new NonSquareMatrixException(matrix.RowDimension, matrix.ColumnDimension);
			}

			if (matrix is DiagonalMatrix)
			{
				return ((DiagonalMatrix) matrix).inverse(threshold);
			}
			else
			{
				QRDecomposition decomposition = new QRDecomposition(matrix, threshold);
				return decomposition.Solver.Inverse;
			}
		}
	}

}
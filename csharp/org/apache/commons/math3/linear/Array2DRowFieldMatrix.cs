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

namespace org.apache.commons.math3.linear
{

	using org.apache.commons.math3;
	using org.apache.commons.math3;
	using NoDataException = org.apache.commons.math3.exception.NoDataException;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MathIllegalStateException = org.apache.commons.math3.exception.MathIllegalStateException;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using MathArrays = org.apache.commons.math3.util.MathArrays;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// Implementation of FieldMatrix<T> using a <seealso cref="FieldElement"/>[][] array to store entries.
	/// <p>
	/// As specified in the <seealso cref="FieldMatrix"/> interface, matrix element indexing
	/// is 0-based -- e.g., <code>getEntry(0, 0)</code>
	/// returns the element in the first row, first column of the matrix.</li></ul>
	/// </p>
	/// </summary>
	/// @param <T> the type of the field elements
	/// @version $Id: Array2DRowFieldMatrix.java 1449528 2013-02-24 19:06:20Z luc $ </param>
	[Serializable]
	public class Array2DRowFieldMatrix<T> : AbstractFieldMatrix<T> where T : org.apache.commons.math3.FieldElement<T>
	{
		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 7260756672015356458L;
		/// <summary>
		/// Entries of the matrix </summary>
		private T[][] data;

		/// <summary>
		/// Creates a matrix with no data </summary>
		/// <param name="field"> field to which the elements belong </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Array2DRowFieldMatrix(final org.apache.commons.math3.Field<T> field)
		public Array2DRowFieldMatrix(Field<T> field) : base(field)
		{
		}

		/// <summary>
		/// Create a new {@code FieldMatrix<T>} with the supplied row and column dimensions.
		/// </summary>
		/// <param name="field"> Field to which the elements belong. </param>
		/// <param name="rowDimension"> Number of rows in the new matrix. </param>
		/// <param name="columnDimension"> Number of columns in the new matrix. </param>
		/// <exception cref="NotStrictlyPositiveException"> if row or column dimension is not positive. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array2DRowFieldMatrix(final org.apache.commons.math3.Field<T> field, final int rowDimension, final int columnDimension) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public Array2DRowFieldMatrix(Field<T> field, int rowDimension, int columnDimension) : base(field, rowDimension, columnDimension)
		{
			data = MathArrays.buildArray(field, rowDimension, columnDimension);
		}

		/// <summary>
		/// Create a new {@code FieldMatrix<T>} using the input array as the underlying
		/// data array.
		/// <p>The input array is copied, not referenced. This constructor has
		/// the same effect as calling <seealso cref="#Array2DRowFieldMatrix(FieldElement[][], boolean)"/>
		/// with the second argument set to {@code true}.</p>
		/// </summary>
		/// <param name="d"> Data for the new matrix. </param>
		/// <exception cref="DimensionMismatchException"> if {@code d} is not rectangular. </exception>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <exception cref="NoDataException"> if there are not at least one row and one column. </exception>
		/// <seealso cref= #Array2DRowFieldMatrix(FieldElement[][], boolean) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array2DRowFieldMatrix(final T[][] d) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoDataException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public Array2DRowFieldMatrix(T[][] d) : this(extractField(d), d)
		{
		}

		/// <summary>
		/// Create a new {@code FieldMatrix<T>} using the input array as the underlying
		/// data array.
		/// <p>The input array is copied, not referenced. This constructor has
		/// the same effect as calling <seealso cref="#Array2DRowFieldMatrix(FieldElement[][], boolean)"/>
		/// with the second argument set to {@code true}.</p>
		/// </summary>
		/// <param name="field"> Field to which the elements belong. </param>
		/// <param name="d"> Data for the new matrix. </param>
		/// <exception cref="DimensionMismatchException"> if {@code d} is not rectangular. </exception>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <exception cref="NoDataException"> if there are not at least one row and one column. </exception>
		/// <seealso cref= #Array2DRowFieldMatrix(FieldElement[][], boolean) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array2DRowFieldMatrix(final org.apache.commons.math3.Field<T> field, final T[][] d) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoDataException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public Array2DRowFieldMatrix(Field<T> field, T[][] d) : base(field)
		{
			copyIn(d);
		}

		/// <summary>
		/// Create a new {@code FieldMatrix<T>} using the input array as the underlying
		/// data array.
		/// <p>If an array is built specially in order to be embedded in a
		/// {@code FieldMatrix<T>} and not used directly, the {@code copyArray} may be
		/// set to {@code false}. This will prevent the copying and improve
		/// performance as no new array will be built and no data will be copied.</p>
		/// </summary>
		/// <param name="d"> Data for the new matrix. </param>
		/// <param name="copyArray"> Whether to copy or reference the input array. </param>
		/// <exception cref="DimensionMismatchException"> if {@code d} is not rectangular. </exception>
		/// <exception cref="NoDataException"> if there are not at least one row and one column. </exception>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <seealso cref= #Array2DRowFieldMatrix(FieldElement[][]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array2DRowFieldMatrix(final T[][] d, final boolean copyArray) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public Array2DRowFieldMatrix(T[][] d, bool copyArray) : this(extractField(d), d, copyArray)
		{
		}

		/// <summary>
		/// Create a new {@code FieldMatrix<T>} using the input array as the underlying
		/// data array.
		/// <p>If an array is built specially in order to be embedded in a
		/// {@code FieldMatrix<T>} and not used directly, the {@code copyArray} may be
		/// set to {@code false}. This will prevent the copying and improve
		/// performance as no new array will be built and no data will be copied.</p>
		/// </summary>
		/// <param name="field"> Field to which the elements belong. </param>
		/// <param name="d"> Data for the new matrix. </param>
		/// <param name="copyArray"> Whether to copy or reference the input array. </param>
		/// <exception cref="DimensionMismatchException"> if {@code d} is not rectangular. </exception>
		/// <exception cref="NoDataException"> if there are not at least one row and one column. </exception>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <seealso cref= #Array2DRowFieldMatrix(FieldElement[][]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array2DRowFieldMatrix(final org.apache.commons.math3.Field<T> field, final T[][] d, final boolean copyArray) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public Array2DRowFieldMatrix(Field<T> field, T[][] d, bool copyArray) : base(field)
		{
			if (copyArray)
			{
				copyIn(d);
			}
			else
			{
				MathUtils.checkNotNull(d);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = d.length;
				int nRows = d.Length;
				if (nRows == 0)
				{
					throw new NoDataException(LocalizedFormats.AT_LEAST_ONE_ROW);
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = d[0].length;
				int nCols = d[0].Length;
				if (nCols == 0)
				{
					throw new NoDataException(LocalizedFormats.AT_LEAST_ONE_COLUMN);
				}
				for (int r = 1; r < nRows; r++)
				{
					if (d[r].Length != nCols)
					{
						throw new DimensionMismatchException(nCols, d[r].Length);
					}
				}
				data = d;
			}
		}

		/// <summary>
		/// Create a new (column) {@code FieldMatrix<T>} using {@code v} as the
		/// data for the unique column of the created matrix.
		/// The input array is copied.
		/// </summary>
		/// <param name="v"> Column vector holding data for new matrix. </param>
		/// <exception cref="NoDataException"> if v is empty </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array2DRowFieldMatrix(final T[] v) throws org.apache.commons.math3.exception.NoDataException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public Array2DRowFieldMatrix(T[] v) : this(extractField(v), v)
		{
		}

		/// <summary>
		/// Create a new (column) {@code FieldMatrix<T>} using {@code v} as the
		/// data for the unique column of the created matrix.
		/// The input array is copied.
		/// </summary>
		/// <param name="field"> Field to which the elements belong. </param>
		/// <param name="v"> Column vector holding data for new matrix. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Array2DRowFieldMatrix(final org.apache.commons.math3.Field<T> field, final T[] v)
		public Array2DRowFieldMatrix(Field<T> field, T[] v) : base(field)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = v.length;
			int nRows = v.Length;
			data = MathArrays.buildArray(Field, nRows, 1);
			for (int row = 0; row < nRows; row++)
			{
				data[row][0] = v[row];
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public FieldMatrix<T> createMatrix(final int rowDimension, final int columnDimension) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override FieldMatrix<T> createMatrix(int rowDimension, int columnDimension)
		{
			return new Array2DRowFieldMatrix<T>(Field, rowDimension, columnDimension);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override FieldMatrix<T> copy()
		{
			return new Array2DRowFieldMatrix<T>(Field, copyOut(), false);
		}

		/// <summary>
		/// Add {@code m} to this matrix.
		/// </summary>
		/// <param name="m"> Matrix to be added. </param>
		/// <returns> {@code this} + m. </returns>
		/// <exception cref="MatrixDimensionMismatchException"> if {@code m} is not the same
		/// size as this matrix. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array2DRowFieldMatrix<T> add(final Array2DRowFieldMatrix<T> m) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual Array2DRowFieldMatrix<T> add(Array2DRowFieldMatrix<T> m)
		{
			// safety check
			checkAdditionCompatible(m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rowCount = getRowDimension();
			int rowCount = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnCount = getColumnDimension();
			int columnCount = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[][] outData = org.apache.commons.math3.util.MathArrays.buildArray(getField(), rowCount, columnCount);
			T[][] outData = MathArrays.buildArray(Field, rowCount, columnCount);
			for (int row = 0; row < rowCount; row++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] dataRow = data[row];
				T[] dataRow = data[row];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] mRow = m.data[row];
				T[] mRow = m.data[row];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] outDataRow = outData[row];
				T[] outDataRow = outData[row];
				for (int col = 0; col < columnCount; col++)
				{
					outDataRow[col] = dataRow[col].add(mRow[col]);
				}
			}

			return new Array2DRowFieldMatrix<T>(Field, outData, false);
		}

		/// <summary>
		/// Subtract {@code m} from this matrix.
		/// </summary>
		/// <param name="m"> Matrix to be subtracted. </param>
		/// <returns> {@code this} + m. </returns>
		/// <exception cref="MatrixDimensionMismatchException"> if {@code m} is not the same
		/// size as this matrix. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array2DRowFieldMatrix<T> subtract(final Array2DRowFieldMatrix<T> m) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual Array2DRowFieldMatrix<T> subtract(Array2DRowFieldMatrix<T> m)
		{
			// safety check
			checkSubtractionCompatible(m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rowCount = getRowDimension();
			int rowCount = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnCount = getColumnDimension();
			int columnCount = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[][] outData = org.apache.commons.math3.util.MathArrays.buildArray(getField(), rowCount, columnCount);
			T[][] outData = MathArrays.buildArray(Field, rowCount, columnCount);
			for (int row = 0; row < rowCount; row++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] dataRow = data[row];
				T[] dataRow = data[row];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] mRow = m.data[row];
				T[] mRow = m.data[row];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] outDataRow = outData[row];
				T[] outDataRow = outData[row];
				for (int col = 0; col < columnCount; col++)
				{
					outDataRow[col] = dataRow[col].subtract(mRow[col]);
				}
			}

			return new Array2DRowFieldMatrix<T>(Field, outData, false);

		}

		/// <summary>
		/// Postmultiplying this matrix by {@code m}.
		/// </summary>
		/// <param name="m"> Matrix to postmultiply by. </param>
		/// <returns> {@code this} * m. </returns>
		/// <exception cref="DimensionMismatchException"> if the number of columns of this
		/// matrix is not equal to the number of rows of {@code m}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array2DRowFieldMatrix<T> multiply(final Array2DRowFieldMatrix<T> m) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual Array2DRowFieldMatrix<T> multiply(Array2DRowFieldMatrix<T> m)
		{
			// safety check
			checkMultiplicationCompatible(m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = this.getRowDimension();
			int nRows = this.RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = m.getColumnDimension();
			int nCols = m.ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nSum = this.getColumnDimension();
			int nSum = this.ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[][] outData = org.apache.commons.math3.util.MathArrays.buildArray(getField(), nRows, nCols);
			T[][] outData = MathArrays.buildArray(Field, nRows, nCols);
			for (int row = 0; row < nRows; row++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] dataRow = data[row];
				T[] dataRow = data[row];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] outDataRow = outData[row];
				T[] outDataRow = outData[row];
				for (int col = 0; col < nCols; col++)
				{
					T sum = Field.Zero;
					for (int i = 0; i < nSum; i++)
					{
						sum = sum.add(dataRow[i].multiply(m.data[i][col]));
					}
					outDataRow[col] = sum;
				}
			}

			return new Array2DRowFieldMatrix<T>(Field, outData, false);

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override T[][] Data
		{
			get
			{
				return copyOut();
			}
		}

		/// <summary>
		/// Get a reference to the underlying data array.
		/// This methods returns internal data, <strong>not</strong> fresh copy of it.
		/// </summary>
		/// <returns> the 2-dimensional array of entries. </returns>
		public virtual T[][] DataRef
		{
			get
			{
				return data;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setSubMatrix(final T[][] subMatrix, final int row, final int column) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void setSubMatrix(T[][] subMatrix, int row, int column)
		{
			if (data == null)
			{
				if (row > 0)
				{
					throw new MathIllegalStateException(LocalizedFormats.FIRST_ROWS_NOT_INITIALIZED_YET, row);
				}
				if (column > 0)
				{
					throw new MathIllegalStateException(LocalizedFormats.FIRST_COLUMNS_NOT_INITIALIZED_YET, column);
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = subMatrix.length;
				int nRows = subMatrix.Length;
				if (nRows == 0)
				{
					throw new NoDataException(LocalizedFormats.AT_LEAST_ONE_ROW);
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = subMatrix[0].length;
				int nCols = subMatrix[0].Length;
				if (nCols == 0)
				{
					throw new NoDataException(LocalizedFormats.AT_LEAST_ONE_COLUMN);
				}
				data = MathArrays.buildArray(Field, subMatrix.Length, nCols);
				for (int i = 0; i < data.Length; ++i)
				{
					if (subMatrix[i].Length != nCols)
					{
						throw new DimensionMismatchException(nCols, subMatrix[i].Length);
					}
					Array.Copy(subMatrix[i], 0, data[i + row], column, nCols);
				}
			}
			else
			{
				base.setSubMatrix(subMatrix, row, column);
			}

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public T getEntry(final int row, final int column) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override T getEntry(int row, int column)
		{
			checkRowIndex(row);
			checkColumnIndex(column);

			return data[row][column];
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setEntry(final int row, final int column, final T value) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void setEntry(int row, int column, T value)
		{
			checkRowIndex(row);
			checkColumnIndex(column);

			data[row][column] = value;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void addToEntry(final int row, final int column, final T increment) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void addToEntry(int row, int column, T increment)
		{
			checkRowIndex(row);
			checkColumnIndex(column);

			data[row][column] = data[row][column].add(increment);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void multiplyEntry(final int row, final int column, final T factor) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void multiplyEntry(int row, int column, T factor)
		{
			checkRowIndex(row);
			checkColumnIndex(column);

			data[row][column] = data[row][column].multiply(factor);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int RowDimension
		{
			get
			{
				return (data == null) ? 0 : data.Length;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int ColumnDimension
		{
			get
			{
				return ((data == null) || (data[0] == null)) ? 0 : data[0].Length;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public T[] operate(final T[] v) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override T[] operate(T[] v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = this.getRowDimension();
			int nRows = this.RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = this.getColumnDimension();
			int nCols = this.ColumnDimension;
			if (v.Length != nCols)
			{
				throw new DimensionMismatchException(v.Length, nCols);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] out = org.apache.commons.math3.util.MathArrays.buildArray(getField(), nRows);
			T[] @out = MathArrays.buildArray(Field, nRows);
			for (int row = 0; row < nRows; row++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] dataRow = data[row];
				T[] dataRow = data[row];
				T sum = Field.Zero;
				for (int i = 0; i < nCols; i++)
				{
					sum = sum.add(dataRow[i].multiply(v[i]));
				}
				@out[row] = sum;
			}
			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public T[] preMultiply(final T[] v) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override T[] preMultiply(T[] v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
			if (v.Length != nRows)
			{
				throw new DimensionMismatchException(v.Length, nRows);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] out = org.apache.commons.math3.util.MathArrays.buildArray(getField(), nCols);
			T[] @out = MathArrays.buildArray(Field, nCols);
			for (int col = 0; col < nCols; ++col)
			{
				T sum = Field.Zero;
				for (int i = 0; i < nRows; ++i)
				{
					sum = sum.add(data[i][col].multiply(v[i]));
				}
				@out[col] = sum;
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public T walkInRowOrder(final FieldMatrixChangingVisitor<T> visitor)
		public override T walkInRowOrder(FieldMatrixChangingVisitor<T> visitor)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rows = getRowDimension();
			int rows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columns = getColumnDimension();
			int columns = ColumnDimension;
			visitor.start(rows, columns, 0, rows - 1, 0, columns - 1);
			for (int i = 0; i < rows; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] rowI = data[i];
				T[] rowI = data[i];
				for (int j = 0; j < columns; ++j)
				{
					rowI[j] = visitor.visit(i, j, rowI[j]);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public T walkInRowOrder(final FieldMatrixPreservingVisitor<T> visitor)
		public override T walkInRowOrder(FieldMatrixPreservingVisitor<T> visitor)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rows = getRowDimension();
			int rows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columns = getColumnDimension();
			int columns = ColumnDimension;
			visitor.start(rows, columns, 0, rows - 1, 0, columns - 1);
			for (int i = 0; i < rows; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] rowI = data[i];
				T[] rowI = data[i];
				for (int j = 0; j < columns; ++j)
				{
					visitor.visit(i, j, rowI[j]);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public T walkInRowOrder(final FieldMatrixChangingVisitor<T> visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override T walkInRowOrder(FieldMatrixChangingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			checkSubMatrixIndex(startRow, endRow, startColumn, endColumn);
			visitor.start(RowDimension, ColumnDimension, startRow, endRow, startColumn, endColumn);
			for (int i = startRow; i <= endRow; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] rowI = data[i];
				T[] rowI = data[i];
				for (int j = startColumn; j <= endColumn; ++j)
				{
					rowI[j] = visitor.visit(i, j, rowI[j]);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public T walkInRowOrder(final FieldMatrixPreservingVisitor<T> visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override T walkInRowOrder(FieldMatrixPreservingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			checkSubMatrixIndex(startRow, endRow, startColumn, endColumn);
			visitor.start(RowDimension, ColumnDimension, startRow, endRow, startColumn, endColumn);
			for (int i = startRow; i <= endRow; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] rowI = data[i];
				T[] rowI = data[i];
				for (int j = startColumn; j <= endColumn; ++j)
				{
					visitor.visit(i, j, rowI[j]);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public T walkInColumnOrder(final FieldMatrixChangingVisitor<T> visitor)
		public override T walkInColumnOrder(FieldMatrixChangingVisitor<T> visitor)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rows = getRowDimension();
			int rows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columns = getColumnDimension();
			int columns = ColumnDimension;
			visitor.start(rows, columns, 0, rows - 1, 0, columns - 1);
			for (int j = 0; j < columns; ++j)
			{
				for (int i = 0; i < rows; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] rowI = data[i];
					T[] rowI = data[i];
					rowI[j] = visitor.visit(i, j, rowI[j]);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public T walkInColumnOrder(final FieldMatrixPreservingVisitor<T> visitor)
		public override T walkInColumnOrder(FieldMatrixPreservingVisitor<T> visitor)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rows = getRowDimension();
			int rows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columns = getColumnDimension();
			int columns = ColumnDimension;
			visitor.start(rows, columns, 0, rows - 1, 0, columns - 1);
			for (int j = 0; j < columns; ++j)
			{
				for (int i = 0; i < rows; ++i)
				{
					visitor.visit(i, j, data[i][j]);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public T walkInColumnOrder(final FieldMatrixChangingVisitor<T> visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override T walkInColumnOrder(FieldMatrixChangingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
		checkSubMatrixIndex(startRow, endRow, startColumn, endColumn);
			visitor.start(RowDimension, ColumnDimension, startRow, endRow, startColumn, endColumn);
			for (int j = startColumn; j <= endColumn; ++j)
			{
				for (int i = startRow; i <= endRow; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] rowI = data[i];
					T[] rowI = data[i];
					rowI[j] = visitor.visit(i, j, rowI[j]);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public T walkInColumnOrder(final FieldMatrixPreservingVisitor<T> visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override T walkInColumnOrder(FieldMatrixPreservingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			checkSubMatrixIndex(startRow, endRow, startColumn, endColumn);
			visitor.start(RowDimension, ColumnDimension, startRow, endRow, startColumn, endColumn);
			for (int j = startColumn; j <= endColumn; ++j)
			{
				for (int i = startRow; i <= endRow; ++i)
				{
					visitor.visit(i, j, data[i][j]);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// Get a fresh copy of the underlying data array.
		/// </summary>
		/// <returns> a copy of the underlying data array. </returns>
		private T[][] copyOut()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = this.getRowDimension();
			int nRows = this.RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[][] out = org.apache.commons.math3.util.MathArrays.buildArray(getField(), nRows, getColumnDimension());
			T[][] @out = MathArrays.buildArray(Field, nRows, ColumnDimension);
			// can't copy 2-d array in one shot, otherwise get row references
			for (int i = 0; i < nRows; i++)
			{
				Array.Copy(data[i], 0, @out[i], 0, data[i].Length);
			}
			return @out;
		}

		/// <summary>
		/// Replace data with a fresh copy of the input array.
		/// </summary>
		/// <param name="in"> Data to copy. </param>
		/// <exception cref="NoDataException"> if the input array is empty. </exception>
		/// <exception cref="DimensionMismatchException"> if the input array is not rectangular. </exception>
		/// <exception cref="NullArgumentException"> if the input array is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void copyIn(final T[][] in) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void copyIn(T[][] @in)
		{
			setSubMatrix(@in, 0, 0);
		}
	}

}
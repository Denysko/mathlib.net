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

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MathIllegalStateException = org.apache.commons.math3.exception.MathIllegalStateException;
	using NoDataException = org.apache.commons.math3.exception.NoDataException;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// Implementation of <seealso cref="RealMatrix"/> using a {@code double[][]} array to
	/// store entries.
	/// 
	/// @version $Id: Array2DRowRealMatrix.java 1459082 2013-03-20 22:24:09Z tn $
	/// </summary>
	[Serializable]
	public class Array2DRowRealMatrix : AbstractRealMatrix
	{
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -1067294169172445528L;

		/// <summary>
		/// Entries of the matrix. </summary>
		private double[][] data;

		/// <summary>
		/// Creates a matrix with no data
		/// </summary>
		public Array2DRowRealMatrix()
		{
		}

		/// <summary>
		/// Create a new RealMatrix with the supplied row and column dimensions.
		/// </summary>
		/// <param name="rowDimension"> Number of rows in the new matrix. </param>
		/// <param name="columnDimension"> Number of columns in the new matrix. </param>
		/// <exception cref="NotStrictlyPositiveException"> if the row or column dimension is
		/// not positive. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array2DRowRealMatrix(final int rowDimension, final int columnDimension) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public Array2DRowRealMatrix(int rowDimension, int columnDimension) : base(rowDimension, columnDimension)
		{
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: data = new double[rowDimension][columnDimension];
			data = RectangularArrays.ReturnRectangularDoubleArray(rowDimension, columnDimension);
		}

		/// <summary>
		/// Create a new {@code RealMatrix} using the input array as the underlying
		/// data array.
		/// <p>The input array is copied, not referenced. This constructor has
		/// the same effect as calling <seealso cref="#Array2DRowRealMatrix(double[][], boolean)"/>
		/// with the second argument set to {@code true}.</p>
		/// </summary>
		/// <param name="d"> Data for the new matrix. </param>
		/// <exception cref="DimensionMismatchException"> if {@code d} is not rectangular. </exception>
		/// <exception cref="NoDataException"> if {@code d} row or column dimension is zero. </exception>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <seealso cref= #Array2DRowRealMatrix(double[][], boolean) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array2DRowRealMatrix(final double[][] d) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public Array2DRowRealMatrix(double[][] d)
		{
			copyIn(d);
		}

		/// <summary>
		/// Create a new RealMatrix using the input array as the underlying
		/// data array.
		/// If an array is built specially in order to be embedded in a
		/// RealMatrix and not used directly, the {@code copyArray} may be
		/// set to {@code false}. This will prevent the copying and improve
		/// performance as no new array will be built and no data will be copied.
		/// </summary>
		/// <param name="d"> Data for new matrix. </param>
		/// <param name="copyArray"> if {@code true}, the input array will be copied,
		/// otherwise it will be referenced. </param>
		/// <exception cref="DimensionMismatchException"> if {@code d} is not rectangular. </exception>
		/// <exception cref="NoDataException"> if {@code d} row or column dimension is zero. </exception>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <seealso cref= #Array2DRowRealMatrix(double[][]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array2DRowRealMatrix(final double[][] d, final boolean copyArray) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public Array2DRowRealMatrix(double[][] d, bool copyArray)
		{
			if (copyArray)
			{
				copyIn(d);
			}
			else
			{
				if (d == null)
				{
					throw new NullArgumentException();
				}
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
						throw new DimensionMismatchException(d[r].Length, nCols);
					}
				}
				data = d;
			}
		}

		/// <summary>
		/// Create a new (column) RealMatrix using {@code v} as the
		/// data for the unique column of the created matrix.
		/// The input array is copied.
		/// </summary>
		/// <param name="v"> Column vector holding data for new matrix. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Array2DRowRealMatrix(final double[] v)
		public Array2DRowRealMatrix(double[] v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = v.length;
			int nRows = v.Length;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: data = new double[nRows][1];
			data = RectangularArrays.ReturnRectangularDoubleArray(nRows, 1);
			for (int row = 0; row < nRows; row++)
			{
				data[row][0] = v[row];
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealMatrix createMatrix(final int rowDimension, final int columnDimension) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealMatrix createMatrix(int rowDimension, int columnDimension)
		{
			return new Array2DRowRealMatrix(rowDimension, columnDimension);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override RealMatrix copy()
		{
			return new Array2DRowRealMatrix(copyOut(), false);
		}

		/// <summary>
		/// Compute the sum of {@code this} and {@code m}.
		/// </summary>
		/// <param name="m"> Matrix to be added. </param>
		/// <returns> {@code this + m}. </returns>
		/// <exception cref="MatrixDimensionMismatchException"> if {@code m} is not the same
		/// size as {@code this}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array2DRowRealMatrix add(final Array2DRowRealMatrix m) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual Array2DRowRealMatrix add(Array2DRowRealMatrix m)
		{
			// Safety check.
			MatrixUtils.checkAdditionCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rowCount = getRowDimension();
			int rowCount = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnCount = getColumnDimension();
			int columnCount = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] outData = new double[rowCount][columnCount];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] outData = new double[rowCount][columnCount];
			double[][] outData = RectangularArrays.ReturnRectangularDoubleArray(rowCount, columnCount);
			for (int row = 0; row < rowCount; row++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dataRow = data[row];
				double[] dataRow = data[row];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] mRow = m.data[row];
				double[] mRow = m.data[row];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outDataRow = outData[row];
				double[] outDataRow = outData[row];
				for (int col = 0; col < columnCount; col++)
				{
					outDataRow[col] = dataRow[col] + mRow[col];
				}
			}

			return new Array2DRowRealMatrix(outData, false);
		}

		/// <summary>
		/// Returns {@code this} minus {@code m}.
		/// </summary>
		/// <param name="m"> Matrix to be subtracted. </param>
		/// <returns> {@code this - m} </returns>
		/// <exception cref="MatrixDimensionMismatchException"> if {@code m} is not the same
		/// size as {@code this}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array2DRowRealMatrix subtract(final Array2DRowRealMatrix m) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual Array2DRowRealMatrix subtract(Array2DRowRealMatrix m)
		{
			MatrixUtils.checkSubtractionCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rowCount = getRowDimension();
			int rowCount = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnCount = getColumnDimension();
			int columnCount = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] outData = new double[rowCount][columnCount];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] outData = new double[rowCount][columnCount];
			double[][] outData = RectangularArrays.ReturnRectangularDoubleArray(rowCount, columnCount);
			for (int row = 0; row < rowCount; row++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dataRow = data[row];
				double[] dataRow = data[row];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] mRow = m.data[row];
				double[] mRow = m.data[row];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outDataRow = outData[row];
				double[] outDataRow = outData[row];
				for (int col = 0; col < columnCount; col++)
				{
					outDataRow[col] = dataRow[col] - mRow[col];
				}
			}

			return new Array2DRowRealMatrix(outData, false);
		}

		/// <summary>
		/// Returns the result of postmultiplying {@code this} by {@code m}.
		/// </summary>
		/// <param name="m"> matrix to postmultiply by </param>
		/// <returns> {@code this * m} </returns>
		/// <exception cref="DimensionMismatchException"> if
		/// {@code columnDimension(this) != rowDimension(m)} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Array2DRowRealMatrix multiply(final Array2DRowRealMatrix m) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual Array2DRowRealMatrix multiply(Array2DRowRealMatrix m)
		{
			MatrixUtils.checkMultiplicationCompatible(this, m);

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
//ORIGINAL LINE: final double[][] outData = new double[nRows][nCols];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] outData = new double[nRows][nCols];
			double[][] outData = RectangularArrays.ReturnRectangularDoubleArray(nRows, nCols);
			// Will hold a column of "m".
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] mCol = new double[nSum];
			double[] mCol = new double[nSum];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] mData = m.data;
			double[][] mData = m.data;

			// Multiply.
			for (int col = 0; col < nCols; col++)
			{
				// Copy all elements of column "col" of "m" so that
				// will be in contiguous memory.
				for (int mRow = 0; mRow < nSum; mRow++)
				{
					mCol[mRow] = mData[mRow][col];
				}

				for (int row = 0; row < nRows; row++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dataRow = data[row];
					double[] dataRow = data[row];
					double sum = 0;
					for (int i = 0; i < nSum; i++)
					{
						sum += dataRow[i] * mCol[i];
					}
					outData[row][col] = sum;
				}
			}

			return new Array2DRowRealMatrix(outData, false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double[][] Data
		{
			get
			{
				return copyOut();
			}
		}

		/// <summary>
		/// Get a reference to the underlying data array.
		/// </summary>
		/// <returns> 2-dimensional array of entries. </returns>
		public virtual double[][] DataRef
		{
			get
			{
				return data;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setSubMatrix(final double[][] subMatrix, final int row, final int column) throws org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void setSubMatrix(double[][] subMatrix, int row, int column)
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
				MathUtils.checkNotNull(subMatrix);
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
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: data = new double[subMatrix.Length][nCols];
				data = RectangularArrays.ReturnRectangularDoubleArray(subMatrix.Length, nCols);
				for (int i = 0; i < data.Length; ++i)
				{
					if (subMatrix[i].Length != nCols)
					{
						throw new DimensionMismatchException(subMatrix[i].Length, nCols);
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
//ORIGINAL LINE: @Override public double getEntry(final int row, final int column) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double getEntry(int row, int column)
		{
			MatrixUtils.checkMatrixIndex(this, row, column);
			return data[row][column];
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setEntry(final int row, final int column, final double value) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void setEntry(int row, int column, double value)
		{
			MatrixUtils.checkMatrixIndex(this, row, column);
			data[row][column] = value;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void addToEntry(final int row, final int column, final double increment) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void addToEntry(int row, int column, double increment)
		{
			MatrixUtils.checkMatrixIndex(this, row, column);
			data[row][column] += increment;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void multiplyEntry(final int row, final int column, final double factor) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void multiplyEntry(int row, int column, double factor)
		{
			MatrixUtils.checkMatrixIndex(this, row, column);
			data[row][column] *= factor;
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
//ORIGINAL LINE: @Override public double[] operate(final double[] v) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double[] operate(double[] v)
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
//ORIGINAL LINE: final double[] out = new double[nRows];
			double[] @out = new double[nRows];
			for (int row = 0; row < nRows; row++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dataRow = data[row];
				double[] dataRow = data[row];
				double sum = 0;
				for (int i = 0; i < nCols; i++)
				{
					sum += dataRow[i] * v[i];
				}
				@out[row] = sum;
			}
			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double[] preMultiply(final double[] v) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double[] preMultiply(double[] v)
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
//ORIGINAL LINE: final double[] out = new double[nCols];
			double[] @out = new double[nCols];
			for (int col = 0; col < nCols; ++col)
			{
				double sum = 0;
				for (int i = 0; i < nRows; ++i)
				{
					sum += data[i][col] * v[i];
				}
				@out[col] = sum;
			}

			return @out;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public double walkInRowOrder(final RealMatrixChangingVisitor visitor)
		public override double walkInRowOrder(RealMatrixChangingVisitor visitor)
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
//ORIGINAL LINE: final double[] rowI = data[i];
				double[] rowI = data[i];
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
//ORIGINAL LINE: @Override public double walkInRowOrder(final RealMatrixPreservingVisitor visitor)
		public override double walkInRowOrder(RealMatrixPreservingVisitor visitor)
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
//ORIGINAL LINE: final double[] rowI = data[i];
				double[] rowI = data[i];
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
//ORIGINAL LINE: @Override public double walkInRowOrder(final RealMatrixChangingVisitor visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double walkInRowOrder(RealMatrixChangingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			MatrixUtils.checkSubMatrixIndex(this, startRow, endRow, startColumn, endColumn);
			visitor.start(RowDimension, ColumnDimension, startRow, endRow, startColumn, endColumn);
			for (int i = startRow; i <= endRow; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] rowI = data[i];
				double[] rowI = data[i];
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
//ORIGINAL LINE: @Override public double walkInRowOrder(final RealMatrixPreservingVisitor visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double walkInRowOrder(RealMatrixPreservingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			MatrixUtils.checkSubMatrixIndex(this, startRow, endRow, startColumn, endColumn);
			visitor.start(RowDimension, ColumnDimension, startRow, endRow, startColumn, endColumn);
			for (int i = startRow; i <= endRow; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] rowI = data[i];
				double[] rowI = data[i];
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
//ORIGINAL LINE: @Override public double walkInColumnOrder(final RealMatrixChangingVisitor visitor)
		public override double walkInColumnOrder(RealMatrixChangingVisitor visitor)
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
//ORIGINAL LINE: final double[] rowI = data[i];
					double[] rowI = data[i];
					rowI[j] = visitor.visit(i, j, rowI[j]);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public double walkInColumnOrder(final RealMatrixPreservingVisitor visitor)
		public override double walkInColumnOrder(RealMatrixPreservingVisitor visitor)
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
//ORIGINAL LINE: @Override public double walkInColumnOrder(final RealMatrixChangingVisitor visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double walkInColumnOrder(RealMatrixChangingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			MatrixUtils.checkSubMatrixIndex(this, startRow, endRow, startColumn, endColumn);
			visitor.start(RowDimension, ColumnDimension, startRow, endRow, startColumn, endColumn);
			for (int j = startColumn; j <= endColumn; ++j)
			{
				for (int i = startRow; i <= endRow; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] rowI = data[i];
					double[] rowI = data[i];
					rowI[j] = visitor.visit(i, j, rowI[j]);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double walkInColumnOrder(final RealMatrixPreservingVisitor visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double walkInColumnOrder(RealMatrixPreservingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			MatrixUtils.checkSubMatrixIndex(this, startRow, endRow, startColumn, endColumn);
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
		private double[][] copyOut()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = this.getRowDimension();
			int nRows = this.RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] out = new double[nRows][this.getColumnDimension()];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] out = new double[nRows][this.ColumnDimension];
			double[][] @out = RectangularArrays.ReturnRectangularDoubleArray(nRows, this.ColumnDimension);
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
//ORIGINAL LINE: private void copyIn(final double[][] in) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void copyIn(double[][] @in)
		{
			setSubMatrix(@in, 0, 0);
		}
	}

}
using System.Collections.Generic;
using System.Text;

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


	using NoDataException = org.apache.commons.math3.exception.NoDataException;
	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using MathUtils = org.apache.commons.math3.util.MathUtils;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Basic implementation of RealMatrix methods regardless of the underlying storage.
	/// <p>All the methods implemented here use <seealso cref="#getEntry(int, int)"/> to access
	/// matrix elements. Derived class can provide faster implementations.</p>
	/// 
	/// @version $Id: AbstractRealMatrix.java 1459534 2013-03-21 21:24:45Z tn $
	/// @since 2.0
	/// </summary>
	public abstract class AbstractRealMatrix : RealLinearOperator, RealMatrix
	{

		/// <summary>
		/// Default format. </summary>
		private static readonly RealMatrixFormat DEFAULT_FORMAT = RealMatrixFormat.getInstance(Locale.US);
		static AbstractRealMatrix()
		{
			// set the minimum fraction digits to 1 to keep compatibility
			DEFAULT_FORMAT.Format.MinimumFractionDigits = 1;
		}

		/// <summary>
		/// Creates a matrix with no data
		/// </summary>
		protected internal AbstractRealMatrix()
		{
		}

		/// <summary>
		/// Create a new RealMatrix with the supplied row and column dimensions.
		/// </summary>
		/// <param name="rowDimension">  the number of rows in the new matrix </param>
		/// <param name="columnDimension">  the number of columns in the new matrix </param>
		/// <exception cref="NotStrictlyPositiveException"> if row or column dimension is not positive </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AbstractRealMatrix(final int rowDimension, final int columnDimension) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal AbstractRealMatrix(int rowDimension, int columnDimension)
		{
			if (rowDimension < 1)
			{
				throw new NotStrictlyPositiveException(rowDimension);
			}
			if (columnDimension < 1)
			{
				throw new NotStrictlyPositiveException(columnDimension);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealMatrix add(RealMatrix m) throws MatrixDimensionMismatchException
		public virtual RealMatrix add(RealMatrix m)
		{
			MatrixUtils.checkAdditionCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rowCount = getRowDimension();
			int rowCount = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnCount = getColumnDimension();
			int columnCount = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix out = createMatrix(rowCount, columnCount);
			RealMatrix @out = createMatrix(rowCount, columnCount);
			for (int row = 0; row < rowCount; ++row)
			{
				for (int col = 0; col < columnCount; ++col)
				{
					@out.setEntry(row, col, getEntry(row, col) + m.getEntry(row, col));
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealMatrix subtract(final RealMatrix m) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealMatrix subtract(RealMatrix m)
		{
			MatrixUtils.checkSubtractionCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rowCount = getRowDimension();
			int rowCount = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnCount = getColumnDimension();
			int columnCount = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix out = createMatrix(rowCount, columnCount);
			RealMatrix @out = createMatrix(rowCount, columnCount);
			for (int row = 0; row < rowCount; ++row)
			{
				for (int col = 0; col < columnCount; ++col)
				{
					@out.setEntry(row, col, getEntry(row, col) - m.getEntry(row, col));
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RealMatrix scalarAdd(final double d)
		public virtual RealMatrix scalarAdd(double d)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rowCount = getRowDimension();
			int rowCount = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnCount = getColumnDimension();
			int columnCount = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix out = createMatrix(rowCount, columnCount);
			RealMatrix @out = createMatrix(rowCount, columnCount);
			for (int row = 0; row < rowCount; ++row)
			{
				for (int col = 0; col < columnCount; ++col)
				{
					@out.setEntry(row, col, getEntry(row, col) + d);
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RealMatrix scalarMultiply(final double d)
		public virtual RealMatrix scalarMultiply(double d)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rowCount = getRowDimension();
			int rowCount = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnCount = getColumnDimension();
			int columnCount = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix out = createMatrix(rowCount, columnCount);
			RealMatrix @out = createMatrix(rowCount, columnCount);
			for (int row = 0; row < rowCount; ++row)
			{
				for (int col = 0; col < columnCount; ++col)
				{
					@out.setEntry(row, col, getEntry(row, col) * d);
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealMatrix multiply(final RealMatrix m) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealMatrix multiply(RealMatrix m)
		{
			MatrixUtils.checkMultiplicationCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = m.getColumnDimension();
			int nCols = m.ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nSum = getColumnDimension();
			int nSum = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix out = createMatrix(nRows, nCols);
			RealMatrix @out = createMatrix(nRows, nCols);
			for (int row = 0; row < nRows; ++row)
			{
				for (int col = 0; col < nCols; ++col)
				{
					double sum = 0;
					for (int i = 0; i < nSum; ++i)
					{
						sum += getEntry(row, i) * m.getEntry(i, col);
					}
					@out.setEntry(row, col, sum);
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealMatrix preMultiply(final RealMatrix m) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealMatrix preMultiply(RealMatrix m)
		{
			return m.multiply(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealMatrix power(final int p) throws org.apache.commons.math3.exception.NotPositiveException, NonSquareMatrixException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealMatrix power(int p)
		{
			if (p < 0)
			{
				throw new NotPositiveException(LocalizedFormats.NOT_POSITIVE_EXPONENT, p);
			}

			if (!Square)
			{
				throw new NonSquareMatrixException(RowDimension, ColumnDimension);
			}

			if (p == 0)
			{
				return MatrixUtils.createRealIdentityMatrix(this.RowDimension);
			}

			if (p == 1)
			{
				return this.copy();
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int power = p - 1;
			int power = p - 1;

			/*
			 * Only log_2(p) operations is used by doing as follows:
			 * 5^214 = 5^128 * 5^64 * 5^16 * 5^4 * 5^2
			 *
			 * In general, the same approach is used for A^p.
			 */

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] binaryRepresentation = Integer.toBinaryString(power).toCharArray();
			char[] binaryRepresentation = int.toBinaryString(power).ToCharArray();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<Integer> nonZeroPositions = new java.util.ArrayList<Integer>();
			List<int?> nonZeroPositions = new List<int?>();
			int maxI = -1;

			for (int i = 0; i < binaryRepresentation.Length; ++i)
			{
				if (binaryRepresentation[i] == '1')
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pos = binaryRepresentation.length - i - 1;
					int pos = binaryRepresentation.Length - i - 1;
					nonZeroPositions.Add(pos);

					// The positions are taken in turn, so maxI is only changed once
					if (maxI == -1)
					{
						maxI = pos;
					}
				}
			}

			RealMatrix[] results = new RealMatrix[maxI + 1];
			results[0] = this.copy();

			for (int i = 1; i <= maxI; ++i)
			{
				results[i] = results[i - 1].multiply(results[i - 1]);
			}

			RealMatrix result = this.copy();

			foreach (int? i in nonZeroPositions)
			{
				result = result.multiply(results[i]);
			}

			return result;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double[][] Data
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[][] data = new double[getRowDimension()][getColumnDimension()];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] data = new double[RowDimension][ColumnDimension];
				double[][] data = RectangularArrays.ReturnRectangularDoubleArray(RowDimension, ColumnDimension);
    
				for (int i = 0; i < data.Length; ++i)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[] dataI = data[i];
					double[] dataI = data[i];
					for (int j = 0; j < dataI.Length; ++j)
					{
						dataI[j] = getEntry(i, j);
					}
				}
    
				return data;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double Norm
		{
			get
			{
				return walkInColumnOrder(new RealMatrixPreservingVisitorAnonymousInnerClassHelper(this));
			}
		}

		private class RealMatrixPreservingVisitorAnonymousInnerClassHelper : RealMatrixPreservingVisitor
		{
			private readonly AbstractRealMatrix outerInstance;

			public RealMatrixPreservingVisitorAnonymousInnerClassHelper(AbstractRealMatrix outerInstance)
			{
				this.outerInstance = outerInstance;
			}


					/// <summary>
					/// Last row index. </summary>
			private double endRow;

			/// <summary>
			/// Sum of absolute values on one column. </summary>
			private double columnSum;

			/// <summary>
			/// Maximal sum across all columns. </summary>
			private double maxColSum;

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void start(final int rows, final int columns, final int startRow, final int endRow, final int startColumn, final int endColumn)
			public virtual void start(int rows, int columns, int startRow, int endRow, int startColumn, int endColumn)
			{
				this.endRow = endRow;
				columnSum = 0;
				maxColSum = 0;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visit(final int row, final int column, final double value)
			public virtual void visit(int row, int column, double value)
			{
				columnSum += FastMath.abs(value);
				if (row == endRow)
				{
					maxColSum = FastMath.max(maxColSum, columnSum);
					columnSum = 0;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual double end()
			{
				return maxColSum;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double FrobeniusNorm
		{
			get
			{
				return walkInOptimizedOrder(new RealMatrixPreservingVisitorAnonymousInnerClassHelper2(this));
			}
		}

		private class RealMatrixPreservingVisitorAnonymousInnerClassHelper2 : RealMatrixPreservingVisitor
		{
			private readonly AbstractRealMatrix outerInstance;

			public RealMatrixPreservingVisitorAnonymousInnerClassHelper2(AbstractRealMatrix outerInstance)
			{
				this.outerInstance = outerInstance;
			}


					/// <summary>
					/// Sum of squared entries. </summary>
			private double sum;

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void start(final int rows, final int columns, final int startRow, final int endRow, final int startColumn, final int endColumn)
			public virtual void start(int rows, int columns, int startRow, int endRow, int startColumn, int endColumn)
			{
				sum = 0;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visit(final int row, final int column, final double value)
			public virtual void visit(int row, int column, double value)
			{
				sum += value * value;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual double end()
			{
				return FastMath.sqrt(sum);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealMatrix getSubMatrix(final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealMatrix getSubMatrix(int startRow, int endRow, int startColumn, int endColumn)
		{
			MatrixUtils.checkSubMatrixIndex(this, startRow, endRow, startColumn, endColumn);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix subMatrix = createMatrix(endRow - startRow + 1, endColumn - startColumn + 1);
			RealMatrix subMatrix = createMatrix(endRow - startRow + 1, endColumn - startColumn + 1);
			for (int i = startRow; i <= endRow; ++i)
			{
				for (int j = startColumn; j <= endColumn; ++j)
				{
					subMatrix.setEntry(i - startRow, j - startColumn, getEntry(i, j));
				}
			}

			return subMatrix;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealMatrix getSubMatrix(final int[] selectedRows, final int[] selectedColumns) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealMatrix getSubMatrix(int[] selectedRows, int[] selectedColumns)
		{
			MatrixUtils.checkSubMatrixIndex(this, selectedRows, selectedColumns);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix subMatrix = createMatrix(selectedRows.length, selectedColumns.length);
			RealMatrix subMatrix = createMatrix(selectedRows.Length, selectedColumns.Length);
			subMatrix.walkInOptimizedOrder(new DefaultRealMatrixChangingVisitorAnonymousInnerClassHelper(this, selectedRows, selectedColumns));

			return subMatrix;
		}

		private class DefaultRealMatrixChangingVisitorAnonymousInnerClassHelper : DefaultRealMatrixChangingVisitor
		{
			private readonly AbstractRealMatrix outerInstance;

			private int[] selectedRows;
			private int[] selectedColumns;

			public DefaultRealMatrixChangingVisitorAnonymousInnerClassHelper(AbstractRealMatrix outerInstance, int[] selectedRows, int[] selectedColumns)
			{
				this.outerInstance = outerInstance;
				this.selectedRows = selectedRows;
				this.selectedColumns = selectedColumns;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public double visit(final int row, final int column, final double value)
			public override double visit(int row, int column, double value)
			{
				return outerInstance.getEntry(selectedRows[row], selectedColumns[column]);
			}

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void copySubMatrix(final int startRow, final int endRow, final int startColumn, final int endColumn, final double[][] destination) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void copySubMatrix(int startRow, int endRow, int startColumn, int endColumn, double[][] destination)
		{
			MatrixUtils.checkSubMatrixIndex(this, startRow, endRow, startColumn, endColumn);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rowsCount = endRow + 1 - startRow;
			int rowsCount = endRow + 1 - startRow;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnsCount = endColumn + 1 - startColumn;
			int columnsCount = endColumn + 1 - startColumn;
			if ((destination.Length < rowsCount) || (destination[0].Length < columnsCount))
			{
				throw new MatrixDimensionMismatchException(destination.Length, destination[0].Length, rowsCount, columnsCount);
			}

			for (int i = 1; i < rowsCount; i++)
			{
				if (destination[i].Length < columnsCount)
				{
					throw new MatrixDimensionMismatchException(destination.Length, destination[i].Length, rowsCount, columnsCount);
				}
			}

			walkInOptimizedOrder(new DefaultRealMatrixPreservingVisitorAnonymousInnerClassHelper(this, startRow, endRow, startColumn, endColumn, destination), startRow, endRow, startColumn, endColumn);
		}

		private class DefaultRealMatrixPreservingVisitorAnonymousInnerClassHelper : DefaultRealMatrixPreservingVisitor
		{
			private readonly AbstractRealMatrix outerInstance;

			private int startRow;
			private int endRow;
			private int startColumn;
			private int endColumn;
			private double[][] destination;

			public DefaultRealMatrixPreservingVisitorAnonymousInnerClassHelper(AbstractRealMatrix outerInstance, int startRow, int endRow, int startColumn, int endColumn, double[][] destination)
			{
				this.outerInstance = outerInstance;
				this.startRow = startRow;
				this.endRow = endRow;
				this.startColumn = startColumn;
				this.endColumn = endColumn;
				this.destination = destination;
			}


					/// <summary>
					/// Initial row index. </summary>
			private int startRow;

			/// <summary>
			/// Initial column index. </summary>
			private int startColumn;

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void start(final int rows, final int columns, final int startRow, final int endRow, final int startColumn, final int endColumn)
			public override void start(int rows, int columns, int startRow, int endRow, int startColumn, int endColumn)
			{
				this.startRow = startRow;
				this.startColumn = startColumn;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void visit(final int row, final int column, final double value)
			public override void visit(int row, int column, double value)
			{
				destination[row - startRow][column - startColumn] = value;
			}

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void copySubMatrix(int[] selectedRows, int[] selectedColumns, double[][] destination) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoDataException, MatrixDimensionMismatchException
		public virtual void copySubMatrix(int[] selectedRows, int[] selectedColumns, double[][] destination)
		{
			MatrixUtils.checkSubMatrixIndex(this, selectedRows, selectedColumns);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = selectedColumns.length;
			int nCols = selectedColumns.Length;
			if ((destination.Length < selectedRows.Length) || (destination[0].Length < nCols))
			{
				throw new MatrixDimensionMismatchException(destination.Length, destination[0].Length, selectedRows.Length, selectedColumns.Length);
			}

			for (int i = 0; i < selectedRows.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] destinationI = destination[i];
				double[] destinationI = destination[i];
				if (destinationI.Length < nCols)
				{
					throw new MatrixDimensionMismatchException(destination.Length, destinationI.Length, selectedRows.Length, selectedColumns.Length);
				}
				for (int j = 0; j < selectedColumns.Length; j++)
				{
					destinationI[j] = getEntry(selectedRows[i], selectedColumns[j]);
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSubMatrix(final double[][] subMatrix, final int row, final int column) throws org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setSubMatrix(double[][] subMatrix, int row, int column)
		{
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

			for (int r = 1; r < nRows; ++r)
			{
				if (subMatrix[r].Length != nCols)
				{
					throw new DimensionMismatchException(nCols, subMatrix[r].Length);
				}
			}

			MatrixUtils.checkRowIndex(this, row);
			MatrixUtils.checkColumnIndex(this, column);
			MatrixUtils.checkRowIndex(this, nRows + row - 1);
			MatrixUtils.checkColumnIndex(this, nCols + column - 1);

			for (int i = 0; i < nRows; ++i)
			{
				for (int j = 0; j < nCols; ++j)
				{
					setEntry(row + i, column + j, subMatrix[i][j]);
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealMatrix getRowMatrix(final int row) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealMatrix getRowMatrix(int row)
		{
			MatrixUtils.checkRowIndex(this, row);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix out = createMatrix(1, nCols);
			RealMatrix @out = createMatrix(1, nCols);
			for (int i = 0; i < nCols; ++i)
			{
				@out.setEntry(0, i, getEntry(row, i));
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setRowMatrix(final int row, final RealMatrix matrix) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setRowMatrix(int row, RealMatrix matrix)
		{
			MatrixUtils.checkRowIndex(this, row);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
			if ((matrix.RowDimension != 1) || (matrix.ColumnDimension != nCols))
			{
				throw new MatrixDimensionMismatchException(matrix.RowDimension, matrix.ColumnDimension, 1, nCols);
			}
			for (int i = 0; i < nCols; ++i)
			{
				setEntry(row, i, matrix.getEntry(0, i));
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealMatrix getColumnMatrix(final int column) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealMatrix getColumnMatrix(int column)
		{
			MatrixUtils.checkColumnIndex(this, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix out = createMatrix(nRows, 1);
			RealMatrix @out = createMatrix(nRows, 1);
			for (int i = 0; i < nRows; ++i)
			{
				@out.setEntry(i, 0, getEntry(i, column));
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setColumnMatrix(final int column, final RealMatrix matrix) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setColumnMatrix(int column, RealMatrix matrix)
		{
			MatrixUtils.checkColumnIndex(this, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
			if ((matrix.RowDimension != nRows) || (matrix.ColumnDimension != 1))
			{
				throw new MatrixDimensionMismatchException(matrix.RowDimension, matrix.ColumnDimension, nRows, 1);
			}
			for (int i = 0; i < nRows; ++i)
			{
				setEntry(i, column, matrix.getEntry(i, 0));
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector getRowVector(final int row) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealVector getRowVector(int row)
		{
			return new ArrayRealVector(getRow(row), false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setRowVector(final int row, final RealVector vector) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setRowVector(int row, RealVector vector)
		{
			MatrixUtils.checkRowIndex(this, row);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
			if (vector.Dimension != nCols)
			{
				throw new MatrixDimensionMismatchException(1, vector.Dimension, 1, nCols);
			}
			for (int i = 0; i < nCols; ++i)
			{
				setEntry(row, i, vector.getEntry(i));
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector getColumnVector(final int column) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealVector getColumnVector(int column)
		{
			return new ArrayRealVector(getColumn(column), false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setColumnVector(final int column, final RealVector vector) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setColumnVector(int column, RealVector vector)
		{
			MatrixUtils.checkColumnIndex(this, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
			if (vector.Dimension != nRows)
			{
				throw new MatrixDimensionMismatchException(vector.Dimension, 1, nRows, 1);
			}
			for (int i = 0; i < nRows; ++i)
			{
				setEntry(i, column, vector.getEntry(i));
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] getRow(final int row) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double[] getRow(int row)
		{
			MatrixUtils.checkRowIndex(this, row);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] out = new double[nCols];
			double[] @out = new double[nCols];
			for (int i = 0; i < nCols; ++i)
			{
				@out[i] = getEntry(row, i);
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setRow(final int row, final double[] array) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setRow(int row, double[] array)
		{
			MatrixUtils.checkRowIndex(this, row);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
			if (array.Length != nCols)
			{
				throw new MatrixDimensionMismatchException(1, array.Length, 1, nCols);
			}
			for (int i = 0; i < nCols; ++i)
			{
				setEntry(row, i, array[i]);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] getColumn(final int column) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double[] getColumn(int column)
		{
			MatrixUtils.checkColumnIndex(this, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] out = new double[nRows];
			double[] @out = new double[nRows];
			for (int i = 0; i < nRows; ++i)
			{
				@out[i] = getEntry(i, column);
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setColumn(final int column, final double[] array) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setColumn(int column, double[] array)
		{
			MatrixUtils.checkColumnIndex(this, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
			if (array.Length != nRows)
			{
				throw new MatrixDimensionMismatchException(array.Length, 1, nRows, 1);
			}
			for (int i = 0; i < nRows; ++i)
			{
				setEntry(i, column, array[i]);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addToEntry(int row, int column, double increment) throws org.apache.commons.math3.exception.OutOfRangeException
		public virtual void addToEntry(int row, int column, double increment)
		{
			MatrixUtils.checkMatrixIndex(this, row, column);
			setEntry(row, column, getEntry(row, column) + increment);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void multiplyEntry(int row, int column, double factor) throws org.apache.commons.math3.exception.OutOfRangeException
		public virtual void multiplyEntry(int row, int column, double factor)
		{
			MatrixUtils.checkMatrixIndex(this, row, column);
			setEntry(row, column, getEntry(row, column) * factor);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual RealMatrix transpose()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix out = createMatrix(nCols, nRows);
			RealMatrix @out = createMatrix(nCols, nRows);
			walkInOptimizedOrder(new DefaultRealMatrixPreservingVisitorAnonymousInnerClassHelper2(this, @out));

			return @out;
		}

		private class DefaultRealMatrixPreservingVisitorAnonymousInnerClassHelper2 : DefaultRealMatrixPreservingVisitor
		{
			private readonly AbstractRealMatrix outerInstance;

			private org.apache.commons.math3.linear.RealMatrix @out;

			public DefaultRealMatrixPreservingVisitorAnonymousInnerClassHelper2(AbstractRealMatrix outerInstance, org.apache.commons.math3.linear.RealMatrix @out)
			{
				this.outerInstance = outerInstance;
				this.@out = @out;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void visit(final int row, final int column, final double value)
			public override void visit(int row, int column, double value)
			{
				@out.setEntry(column, row, value);
			}

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual bool Square
		{
			get
			{
				return ColumnDimension == RowDimension;
			}
		}

		/// <summary>
		/// Returns the number of rows of this matrix.
		/// </summary>
		/// <returns> the number of rows. </returns>
		public override abstract int RowDimension {get;}

		/// <summary>
		/// Returns the number of columns of this matrix.
		/// </summary>
		/// <returns> the number of columns. </returns>
		public override abstract int ColumnDimension {get;}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getTrace() throws NonSquareMatrixException
		public virtual double Trace
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int nRows = getRowDimension();
				int nRows = RowDimension;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int nCols = getColumnDimension();
				int nCols = ColumnDimension;
				if (nRows != nCols)
				{
					throw new NonSquareMatrixException(nRows, nCols);
				}
				double trace = 0;
				for (int i = 0; i < nRows; ++i)
				{
					trace += getEntry(i, i);
				}
				return trace;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] operate(final double[] v) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double[] operate(double[] v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
			if (v.Length != nCols)
			{
				throw new DimensionMismatchException(v.Length, nCols);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] out = new double[nRows];
			double[] @out = new double[nRows];
			for (int row = 0; row < nRows; ++row)
			{
				double sum = 0;
				for (int i = 0; i < nCols; ++i)
				{
					sum += getEntry(row, i) * v[i];
				}
				@out[row] = sum;
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector operate(final RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealVector operate(RealVector v)
		{
			try
			{
				return new ArrayRealVector(operate(((ArrayRealVector) v).DataRef), false);
			}
			catch (System.InvalidCastException cce)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
				int nRows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
				int nCols = ColumnDimension;
				if (v.Dimension != nCols)
				{
					throw new DimensionMismatchException(v.Dimension, nCols);
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] out = new double[nRows];
				double[] @out = new double[nRows];
				for (int row = 0; row < nRows; ++row)
				{
					double sum = 0;
					for (int i = 0; i < nCols; ++i)
					{
						sum += getEntry(row, i) * v.getEntry(i);
					}
					@out[row] = sum;
				}

				return new ArrayRealVector(@out, false);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] preMultiply(final double[] v) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double[] preMultiply(double[] v)
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
					sum += getEntry(i, col) * v[i];
				}
				@out[col] = sum;
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector preMultiply(final RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealVector preMultiply(RealVector v)
		{
			try
			{
				return new ArrayRealVector(preMultiply(((ArrayRealVector) v).DataRef), false);
			}
			catch (System.InvalidCastException cce)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
				int nRows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
				int nCols = ColumnDimension;
				if (v.Dimension != nRows)
				{
					throw new DimensionMismatchException(v.Dimension, nRows);
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] out = new double[nCols];
				double[] @out = new double[nCols];
				for (int col = 0; col < nCols; ++col)
				{
					double sum = 0;
					for (int i = 0; i < nRows; ++i)
					{
						sum += getEntry(i, col) * v.getEntry(i);
					}
					@out[col] = sum;
				}

				return new ArrayRealVector(@out, false);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double walkInRowOrder(final RealMatrixChangingVisitor visitor)
		public virtual double walkInRowOrder(RealMatrixChangingVisitor visitor)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rows = getRowDimension();
			int rows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columns = getColumnDimension();
			int columns = ColumnDimension;
			visitor.start(rows, columns, 0, rows - 1, 0, columns - 1);
			for (int row = 0; row < rows; ++row)
			{
				for (int column = 0; column < columns; ++column)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double oldValue = getEntry(row, column);
					double oldValue = getEntry(row, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double newValue = visitor.visit(row, column, oldValue);
					double newValue = visitor.visit(row, column, oldValue);
					setEntry(row, column, newValue);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double walkInRowOrder(final RealMatrixPreservingVisitor visitor)
		public virtual double walkInRowOrder(RealMatrixPreservingVisitor visitor)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rows = getRowDimension();
			int rows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columns = getColumnDimension();
			int columns = ColumnDimension;
			visitor.start(rows, columns, 0, rows - 1, 0, columns - 1);
			for (int row = 0; row < rows; ++row)
			{
				for (int column = 0; column < columns; ++column)
				{
					visitor.visit(row, column, getEntry(row, column));
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double walkInRowOrder(final RealMatrixChangingVisitor visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double walkInRowOrder(RealMatrixChangingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			MatrixUtils.checkSubMatrixIndex(this, startRow, endRow, startColumn, endColumn);
			visitor.start(RowDimension, ColumnDimension, startRow, endRow, startColumn, endColumn);
			for (int row = startRow; row <= endRow; ++row)
			{
				for (int column = startColumn; column <= endColumn; ++column)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double oldValue = getEntry(row, column);
					double oldValue = getEntry(row, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double newValue = visitor.visit(row, column, oldValue);
					double newValue = visitor.visit(row, column, oldValue);
					setEntry(row, column, newValue);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double walkInRowOrder(final RealMatrixPreservingVisitor visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double walkInRowOrder(RealMatrixPreservingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			MatrixUtils.checkSubMatrixIndex(this, startRow, endRow, startColumn, endColumn);
			visitor.start(RowDimension, ColumnDimension, startRow, endRow, startColumn, endColumn);
			for (int row = startRow; row <= endRow; ++row)
			{
				for (int column = startColumn; column <= endColumn; ++column)
				{
					visitor.visit(row, column, getEntry(row, column));
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double walkInColumnOrder(final RealMatrixChangingVisitor visitor)
		public virtual double walkInColumnOrder(RealMatrixChangingVisitor visitor)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rows = getRowDimension();
			int rows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columns = getColumnDimension();
			int columns = ColumnDimension;
			visitor.start(rows, columns, 0, rows - 1, 0, columns - 1);
			for (int column = 0; column < columns; ++column)
			{
				for (int row = 0; row < rows; ++row)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double oldValue = getEntry(row, column);
					double oldValue = getEntry(row, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double newValue = visitor.visit(row, column, oldValue);
					double newValue = visitor.visit(row, column, oldValue);
					setEntry(row, column, newValue);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double walkInColumnOrder(final RealMatrixPreservingVisitor visitor)
		public virtual double walkInColumnOrder(RealMatrixPreservingVisitor visitor)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rows = getRowDimension();
			int rows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columns = getColumnDimension();
			int columns = ColumnDimension;
			visitor.start(rows, columns, 0, rows - 1, 0, columns - 1);
			for (int column = 0; column < columns; ++column)
			{
				for (int row = 0; row < rows; ++row)
				{
					visitor.visit(row, column, getEntry(row, column));
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double walkInColumnOrder(final RealMatrixChangingVisitor visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double walkInColumnOrder(RealMatrixChangingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			MatrixUtils.checkSubMatrixIndex(this, startRow, endRow, startColumn, endColumn);
			visitor.start(RowDimension, ColumnDimension, startRow, endRow, startColumn, endColumn);
			for (int column = startColumn; column <= endColumn; ++column)
			{
				for (int row = startRow; row <= endRow; ++row)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double oldValue = getEntry(row, column);
					double oldValue = getEntry(row, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double newValue = visitor.visit(row, column, oldValue);
					double newValue = visitor.visit(row, column, oldValue);
					setEntry(row, column, newValue);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double walkInColumnOrder(final RealMatrixPreservingVisitor visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double walkInColumnOrder(RealMatrixPreservingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			MatrixUtils.checkSubMatrixIndex(this, startRow, endRow, startColumn, endColumn);
			visitor.start(RowDimension, ColumnDimension, startRow, endRow, startColumn, endColumn);
			for (int column = startColumn; column <= endColumn; ++column)
			{
				for (int row = startRow; row <= endRow; ++row)
				{
					visitor.visit(row, column, getEntry(row, column));
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double walkInOptimizedOrder(final RealMatrixChangingVisitor visitor)
		public virtual double walkInOptimizedOrder(RealMatrixChangingVisitor visitor)
		{
			return walkInRowOrder(visitor);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double walkInOptimizedOrder(final RealMatrixPreservingVisitor visitor)
		public virtual double walkInOptimizedOrder(RealMatrixPreservingVisitor visitor)
		{
			return walkInRowOrder(visitor);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double walkInOptimizedOrder(final RealMatrixChangingVisitor visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double walkInOptimizedOrder(RealMatrixChangingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			return walkInRowOrder(visitor, startRow, endRow, startColumn, endColumn);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double walkInOptimizedOrder(final RealMatrixPreservingVisitor visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double walkInOptimizedOrder(RealMatrixPreservingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			return walkInRowOrder(visitor, startRow, endRow, startColumn, endColumn);
		}

		/// <summary>
		/// Get a string representation for this matrix. </summary>
		/// <returns> a string representation for this matrix </returns>
		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder res = new StringBuilder();
			StringBuilder res = new StringBuilder();
			string fullClassName = this.GetType().Name;
			string shortClassName = fullClassName.Substring(fullClassName.LastIndexOf('.') + 1);
			res.Append(shortClassName);
			res.Append(DEFAULT_FORMAT.format(this));
			return res.ToString();
		}

		/// <summary>
		/// Returns true iff <code>object</code> is a
		/// <code>RealMatrix</code> instance with the same dimensions as this
		/// and all corresponding matrix entries are equal.
		/// </summary>
		/// <param name="object"> the object to test equality against. </param>
		/// <returns> true if object equals this </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public boolean equals(final Object object)
		public override bool Equals(object @object)
		{
			if (@object == this)
			{
				return true;
			}
			if (@object is RealMatrix == false)
			{
				return false;
			}
			RealMatrix m = (RealMatrix) @object;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
			if (m.ColumnDimension != nCols || m.RowDimension != nRows)
			{
				return false;
			}
			for (int row = 0; row < nRows; ++row)
			{
				for (int col = 0; col < nCols; ++col)
				{
					if (getEntry(row, col) != m.getEntry(row, col))
					{
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Computes a hashcode for the matrix.
		/// </summary>
		/// <returns> hashcode for matrix </returns>
		public override int GetHashCode()
		{
			int ret = 7;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
			ret = ret * 31 + nRows;
			ret = ret * 31 + nCols;
			for (int row = 0; row < nRows; ++row)
			{
				for (int col = 0; col < nCols; ++col)
				{
				   ret = ret * 31 + (11 * (row + 1) + 17 * (col + 1)) * MathUtils.hash(getEntry(row, col));
				}
			}
			return ret;
		}


		/*
		 * Empty implementations of these methods are provided in order to allow for
		 * the use of the @Override tag with Java 1.5.
		 */

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract RealMatrix createMatrix(int rowDimension, int columnDimension) throws org.apache.commons.math3.exception.NotStrictlyPositiveException;
		public abstract RealMatrix createMatrix(int rowDimension, int columnDimension);

		/// <summary>
		/// {@inheritDoc} </summary>
		public abstract RealMatrix copy();

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract double getEntry(int row, int column) throws org.apache.commons.math3.exception.OutOfRangeException;
		public abstract double getEntry(int row, int column);

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void setEntry(int row, int column, double value) throws org.apache.commons.math3.exception.OutOfRangeException;
		public abstract void setEntry(int row, int column, double value);
	}

}
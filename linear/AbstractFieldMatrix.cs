using System;
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

namespace mathlib.linear
{

	using mathlib;
	using mathlib;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using NoDataException = mathlib.exception.NoDataException;
	using NotPositiveException = mathlib.exception.NotPositiveException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using MathArrays = mathlib.util.MathArrays;

	/// <summary>
	/// Basic implementation of <seealso cref="FieldMatrix"/> methods regardless of the underlying storage.
	/// <p>All the methods implemented here use <seealso cref="#getEntry(int, int)"/> to access
	/// matrix elements. Derived class can provide faster implementations. </p>
	/// </summary>
	/// @param <T> Type of the field elements.
	/// 
	/// @version $Id: AbstractFieldMatrix.java 1454876 2013-03-10 16:41:08Z luc $
	/// @since 2.0 </param>
	public abstract class AbstractFieldMatrix<T> : FieldMatrix<T> where T : mathlib.FieldElement<T>
	{
		/// <summary>
		/// Field to which the elements belong. </summary>
		private readonly Field<T> field;

		/// <summary>
		/// Constructor for use with Serializable
		/// </summary>
		protected internal AbstractFieldMatrix()
		{
			field = null;
		}

		/// <summary>
		/// Creates a matrix with no data </summary>
		/// <param name="field"> field to which the elements belong </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractFieldMatrix(final mathlib.Field<T> field)
		protected internal AbstractFieldMatrix(Field<T> field)
		{
			this.field = field;
		}

		/// <summary>
		/// Create a new FieldMatrix<T> with the supplied row and column dimensions.
		/// </summary>
		/// <param name="field"> Field to which the elements belong. </param>
		/// <param name="rowDimension"> Number of rows in the new matrix. </param>
		/// <param name="columnDimension"> Number of columns in the new matrix. </param>
		/// <exception cref="NotStrictlyPositiveException"> if row or column dimension is not
		/// positive. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AbstractFieldMatrix(final mathlib.Field<T> field, final int rowDimension, final int columnDimension) throws mathlib.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal AbstractFieldMatrix(Field<T> field, int rowDimension, int columnDimension)
		{
			if (rowDimension <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.DIMENSION, rowDimension);
			}
			if (columnDimension <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.DIMENSION, columnDimension);
			}
			this.field = field;
		}

		/// <summary>
		/// Get the elements type from an array.
		/// </summary>
		/// @param <T> Type of the field elements. </param>
		/// <param name="d"> Data array. </param>
		/// <returns> the field to which the array elements belong. </returns>
		/// <exception cref="NullArgumentException"> if the array is {@code null}. </exception>
		/// <exception cref="NoDataException"> if the array is empty. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static <T extends mathlib.FieldElement<T>> mathlib.Field<T> extractField(final T[][] d) throws mathlib.exception.NoDataException, mathlib.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal static Field<T> extractField<T>(T[][] d) where T : mathlib.FieldElement<T>
		{
			if (d == null)
			{
				throw new NullArgumentException();
			}
			if (d.Length == 0)
			{
				throw new NoDataException(LocalizedFormats.AT_LEAST_ONE_ROW);
			}
			if (d[0].Length == 0)
			{
				throw new NoDataException(LocalizedFormats.AT_LEAST_ONE_COLUMN);
			}
			return d[0][0].Field;
		}

		/// <summary>
		/// Get the elements type from an array.
		/// </summary>
		/// @param <T> Type of the field elements. </param>
		/// <param name="d"> Data array. </param>
		/// <returns> the field to which the array elements belong. </returns>
		/// <exception cref="NoDataException"> if array is empty. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static <T extends mathlib.FieldElement<T>> mathlib.Field<T> extractField(final T[] d) throws mathlib.exception.NoDataException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal static Field<T> extractField<T>(T[] d) where T : mathlib.FieldElement<T>
		{
			if (d.Length == 0)
			{
				throw new NoDataException(LocalizedFormats.AT_LEAST_ONE_ROW);
			}
			return d[0].Field;
		}

		/// <summary>
		/// Build an array of elements.
		/// <p>
		/// Complete arrays are filled with field.getZero()
		/// </p> </summary>
		/// @param <T> Type of the field elements </param>
		/// <param name="field"> field to which array elements belong </param>
		/// <param name="rows"> number of rows </param>
		/// <param name="columns"> number of columns (may be negative to build partial
		/// arrays in the same way <code>new Field[rows][]</code> works) </param>
		/// <returns> a new array </returns>
		/// @deprecated as of 3.2, replaced by <seealso cref="MathArrays#buildArray(Field, int, int)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.2, replaced by <seealso cref="MathArrays#buildArray(mathlib.Field, int, int)"/>") protected static <T extends mathlib.FieldElement<T>> T[][] buildArray(final mathlib.Field<T> field, final int rows, final int columns)
		[Obsolete]//("as of 3.2, replaced by <seealso cref="MathArrays#buildArray(mathlib.Field, int, int)"/>")]
		protected internal static T[][] buildArray<T>(Field<T> field, int rows, int columns) where T : mathlib.FieldElement<T>
		{
			return MathArrays.buildArray(field, rows, columns);
		}

		/// <summary>
		/// Build an array of elements.
		/// <p>
		/// Arrays are filled with field.getZero()
		/// </p> </summary>
		/// @param <T> the type of the field elements </param>
		/// <param name="field"> field to which array elements belong </param>
		/// <param name="length"> of the array </param>
		/// <returns> a new array </returns>
		/// @deprecated as of 3.2, replaced by <seealso cref="MathArrays#buildArray(Field, int)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.2, replaced by <seealso cref="MathArrays#buildArray(mathlib.Field, int)"/>") protected static <T extends mathlib.FieldElement<T>> T[] buildArray(final mathlib.Field<T> field, final int length)
		[Obsolete]//("as of 3.2, replaced by <seealso cref="MathArrays#buildArray(mathlib.Field, int)"/>")]
		protected internal static T[] buildArray<T>(Field<T> field, int length) where T : mathlib.FieldElement<T>
		{
			return MathArrays.buildArray(field, length);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Field<T> Field
		{
			get
			{
				return field;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract FieldMatrix<T> createMatrix(final int rowDimension, final int columnDimension) throws mathlib.exception.NotStrictlyPositiveException;
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public abstract FieldMatrix<T> createMatrix(int rowDimension, int columnDimension);

		/// <summary>
		/// {@inheritDoc} </summary>
		public abstract FieldMatrix<T> copy();

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldMatrix<T> add(FieldMatrix<T> m) throws MatrixDimensionMismatchException
		public virtual FieldMatrix<T> add(FieldMatrix<T> m)
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
//ORIGINAL LINE: final FieldMatrix<T> out = createMatrix(rowCount, columnCount);
			FieldMatrix<T> @out = createMatrix(rowCount, columnCount);
			for (int row = 0; row < rowCount; ++row)
			{
				for (int col = 0; col < columnCount; ++col)
				{
					@out.setEntry(row, col, getEntry(row, col).add(m.getEntry(row, col)));
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldMatrix<T> subtract(final FieldMatrix<T> m) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual FieldMatrix<T> subtract(FieldMatrix<T> m)
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
//ORIGINAL LINE: final FieldMatrix<T> out = createMatrix(rowCount, columnCount);
			FieldMatrix<T> @out = createMatrix(rowCount, columnCount);
			for (int row = 0; row < rowCount; ++row)
			{
				for (int col = 0; col < columnCount; ++col)
				{
					@out.setEntry(row, col, getEntry(row, col).subtract(m.getEntry(row, col)));
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldMatrix<T> scalarAdd(final T d)
		public virtual FieldMatrix<T> scalarAdd(T d)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rowCount = getRowDimension();
			int rowCount = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnCount = getColumnDimension();
			int columnCount = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldMatrix<T> out = createMatrix(rowCount, columnCount);
			FieldMatrix<T> @out = createMatrix(rowCount, columnCount);
			for (int row = 0; row < rowCount; ++row)
			{
				for (int col = 0; col < columnCount; ++col)
				{
					@out.setEntry(row, col, getEntry(row, col).add(d));
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldMatrix<T> scalarMultiply(final T d)
		public virtual FieldMatrix<T> scalarMultiply(T d)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rowCount = getRowDimension();
			int rowCount = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnCount = getColumnDimension();
			int columnCount = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldMatrix<T> out = createMatrix(rowCount, columnCount);
			FieldMatrix<T> @out = createMatrix(rowCount, columnCount);
			for (int row = 0; row < rowCount; ++row)
			{
				for (int col = 0; col < columnCount; ++col)
				{
					@out.setEntry(row, col, getEntry(row, col).multiply(d));
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldMatrix<T> multiply(final FieldMatrix<T> m) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual FieldMatrix<T> multiply(FieldMatrix<T> m)
		{
			// safety check
			checkMultiplicationCompatible(m);

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
//ORIGINAL LINE: final FieldMatrix<T> out = createMatrix(nRows, nCols);
			FieldMatrix<T> @out = createMatrix(nRows, nCols);
			for (int row = 0; row < nRows; ++row)
			{
				for (int col = 0; col < nCols; ++col)
				{
					T sum = field.Zero;
					for (int i = 0; i < nSum; ++i)
					{
						sum = sum.add(getEntry(row, i).multiply(m.getEntry(i, col)));
					}
					@out.setEntry(row, col, sum);
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldMatrix<T> preMultiply(final FieldMatrix<T> m) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual FieldMatrix<T> preMultiply(FieldMatrix<T> m)
		{
			return m.multiply(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldMatrix<T> power(final int p) throws NonSquareMatrixException, mathlib.exception.NotPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual FieldMatrix<T> power(int p)
		{
			if (p < 0)
			{
				throw new NotPositiveException(p);
			}

			if (!Square)
			{
				throw new NonSquareMatrixException(RowDimension, ColumnDimension);
			}

			if (p == 0)
			{
				return MatrixUtils.createFieldIdentityMatrix(this.Field, this.RowDimension);
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

			for (int i = 0; i < binaryRepresentation.Length; ++i)
			{
				if (binaryRepresentation[i] == '1')
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pos = binaryRepresentation.length - i - 1;
					int pos = binaryRepresentation.Length - i - 1;
					nonZeroPositions.Add(pos);
				}
			}

			List<FieldMatrix<T>> results = new List<FieldMatrix<T>>(binaryRepresentation.Length);

			results.Insert(0, this.copy());

			for (int i = 1; i < binaryRepresentation.Length; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldMatrix<T> s = results.get(i - 1);
				FieldMatrix<T> s = results[i - 1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldMatrix<T> r = s.multiply(s);
				FieldMatrix<T> r = s.multiply(s);
				results.Insert(i, r);
			}

			FieldMatrix<T> result = this.copy();

			foreach (int? i in nonZeroPositions)
			{
				result = result.multiply(results[i]);
			}

			return result;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual T[][] Data
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T[][] data = mathlib.util.MathArrays.buildArray(field, getRowDimension(), getColumnDimension());
				T[][] data = MathArrays.buildArray(field, RowDimension, ColumnDimension);
    
				for (int i = 0; i < data.Length; ++i)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T[] dataI = data[i];
					T[] dataI = data[i];
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldMatrix<T> getSubMatrix(final int startRow, final int endRow, final int startColumn, final int endColumn) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual FieldMatrix<T> getSubMatrix(int startRow, int endRow, int startColumn, int endColumn)
		{
			checkSubMatrixIndex(startRow, endRow, startColumn, endColumn);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldMatrix<T> subMatrix = createMatrix(endRow - startRow + 1, endColumn - startColumn + 1);
			FieldMatrix<T> subMatrix = createMatrix(endRow - startRow + 1, endColumn - startColumn + 1);
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
//ORIGINAL LINE: public FieldMatrix<T> getSubMatrix(final int[] selectedRows, final int[] selectedColumns) throws mathlib.exception.NoDataException, mathlib.exception.NullArgumentException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual FieldMatrix<T> getSubMatrix(int[] selectedRows, int[] selectedColumns)
		{

			// safety checks
			checkSubMatrixIndex(selectedRows, selectedColumns);

			// copy entries
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldMatrix<T> subMatrix = createMatrix(selectedRows.length, selectedColumns.length);
			FieldMatrix<T> subMatrix = createMatrix(selectedRows.Length, selectedColumns.Length);
			subMatrix.walkInOptimizedOrder(new DefaultFieldMatrixChangingVisitorAnonymousInnerClassHelper(this, field.Zero, selectedRows, selectedColumns));

			return subMatrix;

		}

		private class DefaultFieldMatrixChangingVisitorAnonymousInnerClassHelper : DefaultFieldMatrixChangingVisitor<T>
		{
			private readonly AbstractFieldMatrix outerInstance;

			private int[] selectedRows;
			private int[] selectedColumns;

			public DefaultFieldMatrixChangingVisitorAnonymousInnerClassHelper(AbstractFieldMatrix outerInstance, T getZero, int[] selectedRows, int[] selectedColumns) : base(getZero)
			{
				this.outerInstance = outerInstance;
				this.selectedRows = selectedRows;
				this.selectedColumns = selectedColumns;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public T visit(final int row, final int column, final T value)
			public override T visit(int row, int column, T value)
			{
				return outerInstance.getEntry(selectedRows[row], selectedColumns[column]);
			}

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void copySubMatrix(final int startRow, final int endRow, final int startColumn, final int endColumn, final T[][] destination) throws MatrixDimensionMismatchException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void copySubMatrix(int startRow, int endRow, int startColumn, int endColumn, T[][] destination)
		{
			// safety checks
			checkSubMatrixIndex(startRow, endRow, startColumn, endColumn);
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

			// copy entries
			walkInOptimizedOrder(new DefaultFieldMatrixPreservingVisitorAnonymousInnerClassHelper(this, field.Zero, startRow, endRow, startColumn, endColumn, destination), startRow, endRow, startColumn, endColumn);

		}

		private class DefaultFieldMatrixPreservingVisitorAnonymousInnerClassHelper : DefaultFieldMatrixPreservingVisitor<T>
		{
			private readonly AbstractFieldMatrix outerInstance;

			private int startRow;
			private int endRow;
			private int startColumn;
			private int endColumn;
			private T[][] destination;

			public DefaultFieldMatrixPreservingVisitorAnonymousInnerClassHelper(AbstractFieldMatrix outerInstance, T getZero, int startRow, int endRow, int startColumn, int endColumn, T[][] destination) : base(getZero)
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
//ORIGINAL LINE: @Override public void visit(final int row, final int column, final T value)
			public override void visit(int row, int column, T value)
			{
				destination[row - startRow][column - startColumn] = value;
			}

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void copySubMatrix(int[] selectedRows, int[] selectedColumns, T[][] destination) throws MatrixDimensionMismatchException, mathlib.exception.NoDataException, mathlib.exception.NullArgumentException, mathlib.exception.OutOfRangeException
		public virtual void copySubMatrix(int[] selectedRows, int[] selectedColumns, T[][] destination)
		{
			// safety checks
			checkSubMatrixIndex(selectedRows, selectedColumns);
			if ((destination.Length < selectedRows.Length) || (destination[0].Length < selectedColumns.Length))
			{
				throw new MatrixDimensionMismatchException(destination.Length, destination[0].Length, selectedRows.Length, selectedColumns.Length);
			}

			// copy entries
			for (int i = 0; i < selectedRows.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] destinationI = destination[i];
				T[] destinationI = destination[i];
				for (int j = 0; j < selectedColumns.Length; j++)
				{
					destinationI[j] = getEntry(selectedRows[i], selectedColumns[j]);
				}
			}

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSubMatrix(final T[][] subMatrix, final int row, final int column) throws mathlib.exception.DimensionMismatchException, mathlib.exception.OutOfRangeException, mathlib.exception.NoDataException, mathlib.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setSubMatrix(T[][] subMatrix, int row, int column)
		{
			if (subMatrix == null)
			{
				throw new NullArgumentException();
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

			for (int r = 1; r < nRows; ++r)
			{
				if (subMatrix[r].Length != nCols)
				{
					throw new DimensionMismatchException(nCols, subMatrix[r].Length);
				}
			}

			checkRowIndex(row);
			checkColumnIndex(column);
			checkRowIndex(nRows + row - 1);
			checkColumnIndex(nCols + column - 1);

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
//ORIGINAL LINE: public FieldMatrix<T> getRowMatrix(final int row) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual FieldMatrix<T> getRowMatrix(int row)
		{
			checkRowIndex(row);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldMatrix<T> out = createMatrix(1, nCols);
			FieldMatrix<T> @out = createMatrix(1, nCols);
			for (int i = 0; i < nCols; ++i)
			{
				@out.setEntry(0, i, getEntry(row, i));
			}

			return @out;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setRowMatrix(final int row, final FieldMatrix<T> matrix) throws mathlib.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setRowMatrix(int row, FieldMatrix<T> matrix)
		{
			checkRowIndex(row);
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
//ORIGINAL LINE: public FieldMatrix<T> getColumnMatrix(final int column) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual FieldMatrix<T> getColumnMatrix(int column)
		{

			checkColumnIndex(column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldMatrix<T> out = createMatrix(nRows, 1);
			FieldMatrix<T> @out = createMatrix(nRows, 1);
			for (int i = 0; i < nRows; ++i)
			{
				@out.setEntry(i, 0, getEntry(i, column));
			}

			return @out;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setColumnMatrix(final int column, final FieldMatrix<T> matrix) throws mathlib.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setColumnMatrix(int column, FieldMatrix<T> matrix)
		{
			checkColumnIndex(column);
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
//ORIGINAL LINE: public FieldVector<T> getRowVector(final int row) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual FieldVector<T> getRowVector(int row)
		{
			return new ArrayFieldVector<T>(field, getRow(row), false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setRowVector(final int row, final FieldVector<T> vector) throws mathlib.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setRowVector(int row, FieldVector<T> vector)
		{
			checkRowIndex(row);
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
//ORIGINAL LINE: public FieldVector<T> getColumnVector(final int column) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual FieldVector<T> getColumnVector(int column)
		{
			return new ArrayFieldVector<T>(field, getColumn(column), false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setColumnVector(final int column, final FieldVector<T> vector) throws mathlib.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setColumnVector(int column, FieldVector<T> vector)
		{

			checkColumnIndex(column);
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
//ORIGINAL LINE: public T[] getRow(final int row) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T[] getRow(int row)
		{
			checkRowIndex(row);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] out = mathlib.util.MathArrays.buildArray(field, nCols);
			T[] @out = MathArrays.buildArray(field, nCols);
			for (int i = 0; i < nCols; ++i)
			{
				@out[i] = getEntry(row, i);
			}

			return @out;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setRow(final int row, final T[] array) throws mathlib.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setRow(int row, T[] array)
		{
			checkRowIndex(row);
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
//ORIGINAL LINE: public T[] getColumn(final int column) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T[] getColumn(int column)
		{
			checkColumnIndex(column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] out = mathlib.util.MathArrays.buildArray(field, nRows);
			T[] @out = MathArrays.buildArray(field, nRows);
			for (int i = 0; i < nRows; ++i)
			{
				@out[i] = getEntry(i, column);
			}

			return @out;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setColumn(final int column, final T[] array) throws mathlib.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setColumn(int column, T[] array)
		{
			checkColumnIndex(column);
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
//ORIGINAL LINE: public abstract T getEntry(int row, int column) throws mathlib.exception.OutOfRangeException;
		public abstract T getEntry(int row, int column);

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void setEntry(int row, int column, T value) throws mathlib.exception.OutOfRangeException;
		public abstract void setEntry(int row, int column, T value);

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void addToEntry(int row, int column, T increment) throws mathlib.exception.OutOfRangeException;
		public abstract void addToEntry(int row, int column, T increment);

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void multiplyEntry(int row, int column, T factor) throws mathlib.exception.OutOfRangeException;
		public abstract void multiplyEntry(int row, int column, T factor);

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual FieldMatrix<T> transpose()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldMatrix<T> out = createMatrix(nCols, nRows);
			FieldMatrix<T> @out = createMatrix(nCols, nRows);
			walkInOptimizedOrder(new DefaultFieldMatrixPreservingVisitorAnonymousInnerClassHelper2(this, field.Zero, @out));

			return @out;
		}

		private class DefaultFieldMatrixPreservingVisitorAnonymousInnerClassHelper2 : DefaultFieldMatrixPreservingVisitor<T>
		{
			private readonly AbstractFieldMatrix outerInstance;

			private mathlib.linear.FieldMatrix<T> @out;

			public DefaultFieldMatrixPreservingVisitorAnonymousInnerClassHelper2(AbstractFieldMatrix outerInstance, T getZero, mathlib.linear.FieldMatrix<T> @out) : base(getZero)
			{
				this.outerInstance = outerInstance;
				this.@out = @out;
			}

					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void visit(final int row, final int column, final T value)
			public override void visit(int row, int column, T value)
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
		/// {@inheritDoc} </summary>
		public abstract int RowDimension {get;}

		/// <summary>
		/// {@inheritDoc} </summary>
		public abstract int ColumnDimension {get;}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T getTrace() throws NonSquareMatrixException
		public virtual T Trace
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
				T trace = field.Zero;
				for (int i = 0; i < nRows; ++i)
				{
					trace = trace.add(getEntry(i, i));
				}
				return trace;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T[] operate(final T[] v) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T[] operate(T[] v)
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
//ORIGINAL LINE: final T[] out = mathlib.util.MathArrays.buildArray(field, nRows);
			T[] @out = MathArrays.buildArray(field, nRows);
			for (int row = 0; row < nRows; ++row)
			{
				T sum = field.Zero;
				for (int i = 0; i < nCols; ++i)
				{
					sum = sum.add(getEntry(row, i).multiply(v[i]));
				}
				@out[row] = sum;
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> operate(final FieldVector<T> v) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual FieldVector<T> operate(FieldVector<T> v)
		{
			try
			{
				return new ArrayFieldVector<T>(field, operate(((ArrayFieldVector<T>) v).DataRef), false);
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
//ORIGINAL LINE: final T[] out = mathlib.util.MathArrays.buildArray(field, nRows);
				T[] @out = MathArrays.buildArray(field, nRows);
				for (int row = 0; row < nRows; ++row)
				{
					T sum = field.Zero;
					for (int i = 0; i < nCols; ++i)
					{
						sum = sum.add(getEntry(row, i).multiply(v.getEntry(i)));
					}
					@out[row] = sum;
				}

				return new ArrayFieldVector<T>(field, @out, false);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T[] preMultiply(final T[] v) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T[] preMultiply(T[] v)
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
//ORIGINAL LINE: final T[] out = mathlib.util.MathArrays.buildArray(field, nCols);
			T[] @out = MathArrays.buildArray(field, nCols);
			for (int col = 0; col < nCols; ++col)
			{
				T sum = field.Zero;
				for (int i = 0; i < nRows; ++i)
				{
					sum = sum.add(getEntry(i, col).multiply(v[i]));
				}
				@out[col] = sum;
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector<T> preMultiply(final FieldVector<T> v) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual FieldVector<T> preMultiply(FieldVector<T> v)
		{
			try
			{
				return new ArrayFieldVector<T>(field, preMultiply(((ArrayFieldVector<T>) v).DataRef), false);
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
//ORIGINAL LINE: final T[] out = mathlib.util.MathArrays.buildArray(field, nCols);
				T[] @out = MathArrays.buildArray(field, nCols);
				for (int col = 0; col < nCols; ++col)
				{
					T sum = field.Zero;
					for (int i = 0; i < nRows; ++i)
					{
						sum = sum.add(getEntry(i, col).multiply(v.getEntry(i)));
					}
					@out[col] = sum;
				}

				return new ArrayFieldVector<T>(field, @out, false);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T walkInRowOrder(final FieldMatrixChangingVisitor<T> visitor)
		public virtual T walkInRowOrder(FieldMatrixChangingVisitor<T> visitor)
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
//ORIGINAL LINE: final T oldValue = getEntry(row, column);
					T oldValue = getEntry(row, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T newValue = visitor.visit(row, column, oldValue);
					T newValue = visitor.visit(row, column, oldValue);
					setEntry(row, column, newValue);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T walkInRowOrder(final FieldMatrixPreservingVisitor<T> visitor)
		public virtual T walkInRowOrder(FieldMatrixPreservingVisitor<T> visitor)
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
//ORIGINAL LINE: public T walkInRowOrder(final FieldMatrixChangingVisitor<T> visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T walkInRowOrder(FieldMatrixChangingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			checkSubMatrixIndex(startRow, endRow, startColumn, endColumn);
			visitor.start(RowDimension, ColumnDimension, startRow, endRow, startColumn, endColumn);
			for (int row = startRow; row <= endRow; ++row)
			{
				for (int column = startColumn; column <= endColumn; ++column)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T oldValue = getEntry(row, column);
					T oldValue = getEntry(row, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T newValue = visitor.visit(row, column, oldValue);
					T newValue = visitor.visit(row, column, oldValue);
					setEntry(row, column, newValue);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T walkInRowOrder(final FieldMatrixPreservingVisitor<T> visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T walkInRowOrder(FieldMatrixPreservingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			checkSubMatrixIndex(startRow, endRow, startColumn, endColumn);
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
//ORIGINAL LINE: public T walkInColumnOrder(final FieldMatrixChangingVisitor<T> visitor)
		public virtual T walkInColumnOrder(FieldMatrixChangingVisitor<T> visitor)
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
//ORIGINAL LINE: final T oldValue = getEntry(row, column);
					T oldValue = getEntry(row, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T newValue = visitor.visit(row, column, oldValue);
					T newValue = visitor.visit(row, column, oldValue);
					setEntry(row, column, newValue);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T walkInColumnOrder(final FieldMatrixPreservingVisitor<T> visitor)
		public virtual T walkInColumnOrder(FieldMatrixPreservingVisitor<T> visitor)
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
//ORIGINAL LINE: public T walkInColumnOrder(final FieldMatrixChangingVisitor<T> visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T walkInColumnOrder(FieldMatrixChangingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			checkSubMatrixIndex(startRow, endRow, startColumn, endColumn);
			visitor.start(RowDimension, ColumnDimension, startRow, endRow, startColumn, endColumn);
			for (int column = startColumn; column <= endColumn; ++column)
			{
				for (int row = startRow; row <= endRow; ++row)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T oldValue = getEntry(row, column);
					T oldValue = getEntry(row, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T newValue = visitor.visit(row, column, oldValue);
					T newValue = visitor.visit(row, column, oldValue);
					setEntry(row, column, newValue);
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T walkInColumnOrder(final FieldMatrixPreservingVisitor<T> visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T walkInColumnOrder(FieldMatrixPreservingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			checkSubMatrixIndex(startRow, endRow, startColumn, endColumn);
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
//ORIGINAL LINE: public T walkInOptimizedOrder(final FieldMatrixChangingVisitor<T> visitor)
		public virtual T walkInOptimizedOrder(FieldMatrixChangingVisitor<T> visitor)
		{
			return walkInRowOrder(visitor);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T walkInOptimizedOrder(final FieldMatrixPreservingVisitor<T> visitor)
		public virtual T walkInOptimizedOrder(FieldMatrixPreservingVisitor<T> visitor)
		{
			return walkInRowOrder(visitor);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T walkInOptimizedOrder(final FieldMatrixChangingVisitor<T> visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T walkInOptimizedOrder(FieldMatrixChangingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			return walkInRowOrder(visitor, startRow, endRow, startColumn, endColumn);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T walkInOptimizedOrder(final FieldMatrixPreservingVisitor<T> visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T walkInOptimizedOrder(FieldMatrixPreservingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			return walkInRowOrder(visitor, startRow, endRow, startColumn, endColumn);
		}

		/// <summary>
		/// Get a string representation for this matrix. </summary>
		/// <returns> a string representation for this matrix </returns>
		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer res = new StringBuffer();
			StringBuilder res = new StringBuilder();
			string fullClassName = this.GetType().Name;
			string shortClassName = fullClassName.Substring(fullClassName.LastIndexOf('.') + 1);
			res.Append(shortClassName).Append("{");

			for (int i = 0; i < nRows; ++i)
			{
				if (i > 0)
				{
					res.Append(",");
				}
				res.Append("{");
				for (int j = 0; j < nCols; ++j)
				{
					if (j > 0)
					{
						res.Append(",");
					}
					res.Append(getEntry(i, j));
				}
				res.Append("}");
			}

			res.Append("}");
			return res.ToString();
		}

		/// <summary>
		/// Returns true iff <code>object</code> is a
		/// <code>FieldMatrix</code> instance with the same dimensions as this
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
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (object instanceof FieldMatrix<?>== false)
			if (@object is FieldMatrix<?>== false)
			{
				return false;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: FieldMatrix<?> m = (FieldMatrix<?>) object;
			FieldMatrix<?> m = (FieldMatrix<?>) @object;
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
					if (!getEntry(row, col).Equals(m.getEntry(row, col)))
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
			int ret = 322562;
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
				   ret = ret * 31 + (11 * (row + 1) + 17 * (col + 1)) * getEntry(row, col).GetHashCode();
				}
			}
			return ret;
		}

		/// <summary>
		/// Check if a row index is valid.
		/// </summary>
		/// <param name="row"> Row index to check. </param>
		/// <exception cref="OutOfRangeException"> if {@code index} is not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkRowIndex(final int row) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual void checkRowIndex(int row)
		{
			if (row < 0 || row >= RowDimension)
			{
				throw new OutOfRangeException(LocalizedFormats.ROW_INDEX, row, 0, RowDimension - 1);
			}
		}

		/// <summary>
		/// Check if a column index is valid.
		/// </summary>
		/// <param name="column"> Column index to check. </param>
		/// <exception cref="OutOfRangeException"> if {@code index} is not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkColumnIndex(final int column) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual void checkColumnIndex(int column)
		{
			if (column < 0 || column >= ColumnDimension)
			{
				throw new OutOfRangeException(LocalizedFormats.COLUMN_INDEX, column, 0, ColumnDimension - 1);
			}
		}

		/// <summary>
		/// Check if submatrix ranges indices are valid.
		/// Rows and columns are indicated counting from 0 to n-1.
		/// </summary>
		/// <param name="startRow"> Initial row index. </param>
		/// <param name="endRow"> Final row index. </param>
		/// <param name="startColumn"> Initial column index. </param>
		/// <param name="endColumn"> Final column index. </param>
		/// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code endRow < startRow} or
		/// {@code endColumn < startColumn}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkSubMatrixIndex(final int startRow, final int endRow, final int startColumn, final int endColumn) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual void checkSubMatrixIndex(int startRow, int endRow, int startColumn, int endColumn)
		{
			checkRowIndex(startRow);
			checkRowIndex(endRow);
			if (endRow < startRow)
			{
				throw new NumberIsTooSmallException(LocalizedFormats.INITIAL_ROW_AFTER_FINAL_ROW, endRow, startRow, true);
			}

			checkColumnIndex(startColumn);
			checkColumnIndex(endColumn);
			if (endColumn < startColumn)
			{
				throw new NumberIsTooSmallException(LocalizedFormats.INITIAL_COLUMN_AFTER_FINAL_COLUMN, endColumn, startColumn, true);
			}
		}

		/// <summary>
		/// Check if submatrix ranges indices are valid.
		/// Rows and columns are indicated counting from 0 to n-1.
		/// </summary>
		/// <param name="selectedRows"> Array of row indices. </param>
		/// <param name="selectedColumns"> Array of column indices. </param>
		/// <exception cref="NullArgumentException"> if the arrays are {@code null}. </exception>
		/// <exception cref="NoDataException"> if the arrays have zero length. </exception>
		/// <exception cref="OutOfRangeException"> if row or column selections are not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkSubMatrixIndex(final int[] selectedRows, final int[] selectedColumns) throws mathlib.exception.NoDataException, mathlib.exception.NullArgumentException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual void checkSubMatrixIndex(int[] selectedRows, int[] selectedColumns)
		{
			if (selectedRows == null || selectedColumns == null)
			{
				throw new NullArgumentException();
			}
			if (selectedRows.Length == 0 || selectedColumns.Length == 0)
			{
				throw new NoDataException();
			}

			foreach (int row in selectedRows)
			{
				checkRowIndex(row);
			}
			foreach (int column in selectedColumns)
			{
				checkColumnIndex(column);
			}
		}

		/// <summary>
		/// Check if a matrix is addition compatible with the instance.
		/// </summary>
		/// <param name="m"> Matrix to check. </param>
		/// <exception cref="MatrixDimensionMismatchException"> if the matrix is not
		/// addition-compatible with instance. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkAdditionCompatible(final FieldMatrix<T> m) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual void checkAdditionCompatible(FieldMatrix<T> m)
		{
			if ((RowDimension != m.RowDimension) || (ColumnDimension != m.ColumnDimension))
			{
				throw new MatrixDimensionMismatchException(m.RowDimension, m.ColumnDimension, RowDimension, ColumnDimension);
			}
		}

		/// <summary>
		/// Check if a matrix is subtraction compatible with the instance.
		/// </summary>
		/// <param name="m"> Matrix to check. </param>
		/// <exception cref="MatrixDimensionMismatchException"> if the matrix is not
		/// subtraction-compatible with instance. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkSubtractionCompatible(final FieldMatrix<T> m) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual void checkSubtractionCompatible(FieldMatrix<T> m)
		{
			if ((RowDimension != m.RowDimension) || (ColumnDimension != m.ColumnDimension))
			{
				throw new MatrixDimensionMismatchException(m.RowDimension, m.ColumnDimension, RowDimension, ColumnDimension);
			}
		}

		/// <summary>
		/// Check if a matrix is multiplication compatible with the instance.
		/// </summary>
		/// <param name="m"> Matrix to check. </param>
		/// <exception cref="DimensionMismatchException"> if the matrix is not
		/// multiplication-compatible with instance. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkMultiplicationCompatible(final FieldMatrix<T> m) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual void checkMultiplicationCompatible(FieldMatrix<T> m)
		{
			if (ColumnDimension != m.RowDimension)
			{
				throw new DimensionMismatchException(m.RowDimension, ColumnDimension);
			}
		}
	}

}
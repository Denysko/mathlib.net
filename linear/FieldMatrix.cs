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

	/// <summary>
	/// Interface defining field-valued matrix with basic algebraic operations.
	/// <p>
	/// Matrix element indexing is 0-based -- e.g., <code>getEntry(0, 0)</code>
	/// returns the element in the first row, first column of the matrix.</p>
	/// </summary>
	/// @param <T> the type of the field elements
	/// @version $Id: FieldMatrix.java 1416643 2012-12-03 19:37:14Z tn $ </param>
	public interface FieldMatrix<T> : AnyMatrix where T : mathlib.FieldElement<T>
	{
		/// <summary>
		/// Get the type of field elements of the matrix.
		/// </summary>
		/// <returns> the type of field elements of the matrix. </returns>
		Field<T> Field {get;}

		/// <summary>
		/// Create a new FieldMatrix<T> of the same type as the instance with
		/// the supplied row and column dimensions.
		/// </summary>
		/// <param name="rowDimension">  the number of rows in the new matrix </param>
		/// <param name="columnDimension">  the number of columns in the new matrix </param>
		/// <returns> a new matrix of the same type as the instance </returns>
		/// <exception cref="NotStrictlyPositiveException"> if row or column dimension is not
		/// positive.
		/// @since 2.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldMatrix<T> createMatrix(final int rowDimension, final int columnDimension) throws mathlib.exception.NotStrictlyPositiveException;
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		FieldMatrix<T> createMatrix(int rowDimension, int columnDimension);

		/// <summary>
		/// Make a (deep) copy of this.
		/// </summary>
		/// <returns> a copy of this matrix. </returns>
		FieldMatrix<T> copy();

		/// <summary>
		/// Compute the sum of this and m.
		/// </summary>
		/// <param name="m"> Matrix to be added. </param>
		/// <returns> {@code this} + {@code m}. </returns>
		/// <exception cref="MatrixDimensionMismatchException"> if {@code m} is not the same
		/// size as {@code this} matrix. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldMatrix<T> add(FieldMatrix<T> m) throws MatrixDimensionMismatchException;
		FieldMatrix<T> add(FieldMatrix<T> m);

		/// <summary>
		/// Subtract {@code m} from this matrix.
		/// </summary>
		/// <param name="m"> Matrix to be subtracted. </param>
		/// <returns> {@code this} - {@code m}. </returns>
		/// <exception cref="MatrixDimensionMismatchException"> if {@code m} is not the same
		/// size as {@code this} matrix. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldMatrix<T> subtract(FieldMatrix<T> m) throws MatrixDimensionMismatchException;
		FieldMatrix<T> subtract(FieldMatrix<T> m);

		 /// <summary>
		 /// Increment each entry of this matrix.
		 /// </summary>
		 /// <param name="d"> Value to be added to each entry. </param>
		 /// <returns> {@code d} + {@code this}. </returns>
		FieldMatrix<T> scalarAdd(T d);

		/// <summary>
		/// Multiply each entry by {@code d}.
		/// </summary>
		/// <param name="d"> Value to multiply all entries by. </param>
		/// <returns> {@code d} * {@code this}. </returns>
		FieldMatrix<T> scalarMultiply(T d);

		/// <summary>
		/// Postmultiply this matrix by {@code m}.
		/// </summary>
		/// <param name="m">  Matrix to postmultiply by. </param>
		/// <returns> {@code this} * {@code m}. </returns>
		/// <exception cref="DimensionMismatchException"> if the number of columns of
		/// {@code this} matrix is not equal to the number of rows of matrix
		/// {@code m}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldMatrix<T> multiply(FieldMatrix<T> m) throws mathlib.exception.DimensionMismatchException;
		FieldMatrix<T> multiply(FieldMatrix<T> m);

		/// <summary>
		/// Premultiply this matrix by {@code m}.
		/// </summary>
		/// <param name="m"> Matrix to premultiply by. </param>
		/// <returns> {@code m} * {@code this}. </returns>
		/// <exception cref="DimensionMismatchException"> if the number of columns of {@code m}
		/// differs from the number of rows of {@code this} matrix. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldMatrix<T> preMultiply(FieldMatrix<T> m) throws mathlib.exception.DimensionMismatchException;
		FieldMatrix<T> preMultiply(FieldMatrix<T> m);

		/// <summary>
		/// Returns the result multiplying this with itself <code>p</code> times.
		/// Depending on the type of the field elements, T, instability for high
		/// powers might occur.
		/// </summary>
		/// <param name="p"> raise this to power p </param>
		/// <returns> this^p </returns>
		/// <exception cref="NotPositiveException"> if {@code p < 0} </exception>
		/// <exception cref="NonSquareMatrixException"> if {@code this matrix} is not square </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldMatrix<T> power(final int p) throws NonSquareMatrixException, mathlib.exception.NotPositiveException;
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		FieldMatrix<T> power(int p);

		/// <summary>
		/// Returns matrix entries as a two-dimensional array.
		/// </summary>
		/// <returns> a 2-dimensional array of entries. </returns>
		T[][] Data {get;}

		/// <summary>
		/// Get a submatrix. Rows and columns are indicated
		/// counting from 0 to n - 1.
		/// </summary>
		/// <param name="startRow"> Initial row index </param>
		/// <param name="endRow"> Final row index (inclusive) </param>
		/// <param name="startColumn"> Initial column index </param>
		/// <param name="endColumn"> Final column index (inclusive) </param>
		/// <returns> the matrix containing the data of the specified rows and columns. </returns>
		/// <exception cref="NumberIsTooSmallException"> is {@code endRow < startRow} of
		/// {@code endColumn < startColumn}. </exception>
		/// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldMatrix<T> getSubMatrix(int startRow, int endRow, int startColumn, int endColumn) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException;
	   FieldMatrix<T> getSubMatrix(int startRow, int endRow, int startColumn, int endColumn);

	   /// <summary>
	   /// Get a submatrix. Rows and columns are indicated
	   /// counting from 0 to n - 1.
	   /// </summary>
	   /// <param name="selectedRows"> Array of row indices. </param>
	   /// <param name="selectedColumns"> Array of column indices. </param>
	   /// <returns> the matrix containing the data in the
	   /// specified rows and columns. </returns>
	   /// <exception cref="NoDataException"> if {@code selectedRows} or
	   /// {@code selectedColumns} is empty </exception>
	   /// <exception cref="NullArgumentException"> if {@code selectedRows} or
	   /// {@code selectedColumns} is {@code null}. </exception>
	   /// <exception cref="OutOfRangeException"> if row or column selections are not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldMatrix<T> getSubMatrix(int[] selectedRows, int[] selectedColumns) throws mathlib.exception.NoDataException, mathlib.exception.NullArgumentException, mathlib.exception.OutOfRangeException;
	   FieldMatrix<T> getSubMatrix(int[] selectedRows, int[] selectedColumns);

	   /// <summary>
	   /// Copy a submatrix. Rows and columns are indicated
	   /// counting from 0 to n-1.
	   /// </summary>
	   /// <param name="startRow"> Initial row index. </param>
	   /// <param name="endRow"> Final row index (inclusive). </param>
	   /// <param name="startColumn"> Initial column index. </param>
	   /// <param name="endColumn"> Final column index (inclusive). </param>
	   /// <param name="destination"> The arrays where the submatrix data should be copied
	   /// (if larger than rows/columns counts, only the upper-left part will be used). </param>
	   /// <exception cref="MatrixDimensionMismatchException"> if the dimensions of
	   /// {@code destination} do not match those of {@code this}. </exception>
	   /// <exception cref="NumberIsTooSmallException"> is {@code endRow < startRow} of
	   /// {@code endColumn < startColumn}. </exception>
	   /// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
	   /// <exception cref="IllegalArgumentException"> if the destination array is too small. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void copySubMatrix(int startRow, int endRow, int startColumn, int endColumn, T[][] destination) throws MatrixDimensionMismatchException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException;
		void copySubMatrix(int startRow, int endRow, int startColumn, int endColumn, T[][] destination);

	  /// <summary>
	  /// Copy a submatrix. Rows and columns are indicated
	  /// counting from 0 to n - 1.
	  /// </summary>
	  /// <param name="selectedRows"> Array of row indices. </param>
	  /// <param name="selectedColumns"> Array of column indices. </param>
	  /// <param name="destination"> Arrays where the submatrix data should be copied
	  /// (if larger than rows/columns counts, only the upper-left part will be used) </param>
	  /// <exception cref="MatrixDimensionMismatchException"> if the dimensions of
	  /// {@code destination} do not match those of {@code this}. </exception>
	  /// <exception cref="NoDataException"> if {@code selectedRows} or
	  /// {@code selectedColumns} is empty </exception>
	  /// <exception cref="NullArgumentException"> if {@code selectedRows} or
	  /// {@code selectedColumns} is {@code null}. </exception>
	  /// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void copySubMatrix(int[] selectedRows, int[] selectedColumns, T[][] destination) throws MatrixDimensionMismatchException, mathlib.exception.NoDataException, mathlib.exception.NullArgumentException, mathlib.exception.OutOfRangeException;
	  void copySubMatrix(int[] selectedRows, int[] selectedColumns, T[][] destination);

		/// <summary>
		/// Replace the submatrix starting at {@code (row, column)} using data in the
		/// input {@code subMatrix} array. Indexes are 0-based.
		/// <p>
		/// Example:<br>
		/// Starting with
		/// 
		/// <pre>
		/// 1  2  3  4
		/// 5  6  7  8
		/// 9  0  1  2
		/// </pre>
		/// 
		/// and <code>subMatrix = {{3, 4} {5,6}}</code>, invoking
		/// <code>setSubMatrix(subMatrix,1,1))</code> will result in
		/// 
		/// <pre>
		/// 1  2  3  4
		/// 5  3  4  8
		/// 9  5  6  2
		/// </pre>
		/// 
		/// </p>
		/// </summary>
		/// <param name="subMatrix"> Array containing the submatrix replacement data. </param>
		/// <param name="row"> Row coordinate of the top-left element to be replaced. </param>
		/// <param name="column"> Column coordinate of the top-left element to be replaced. </param>
		/// <exception cref="OutOfRangeException"> if {@code subMatrix} does not fit into this
		/// matrix from element in {@code (row, column)}. </exception>
		/// <exception cref="NoDataException"> if a row or column of {@code subMatrix} is empty. </exception>
		/// <exception cref="DimensionMismatchException"> if {@code subMatrix} is not
		/// rectangular (not all rows have the same length). </exception>
		/// <exception cref="NullArgumentException"> if {@code subMatrix} is {@code null}.
		/// @since 2.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setSubMatrix(T[][] subMatrix, int row, int column) throws mathlib.exception.DimensionMismatchException, mathlib.exception.OutOfRangeException, mathlib.exception.NoDataException, mathlib.exception.NullArgumentException;
		void setSubMatrix(T[][] subMatrix, int row, int column);

	   /// <summary>
	   /// Get the entries in row number {@code row}
	   /// as a row matrix.
	   /// </summary>
	   /// <param name="row"> Row to be fetched. </param>
	   /// <returns> a row matrix. </returns>
	   /// <exception cref="OutOfRangeException"> if the specified row index is invalid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldMatrix<T> getRowMatrix(int row) throws mathlib.exception.OutOfRangeException;
	   FieldMatrix<T> getRowMatrix(int row);

	   /// <summary>
	   /// Set the entries in row number {@code row}
	   /// as a row matrix.
	   /// </summary>
	   /// <param name="row"> Row to be set. </param>
	   /// <param name="matrix"> Row matrix (must have one row and the same number
	   /// of columns as the instance). </param>
	   /// <exception cref="OutOfRangeException"> if the specified row index is invalid. </exception>
	   /// <exception cref="MatrixDimensionMismatchException">
	   /// if the matrix dimensions do not match one instance row. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setRowMatrix(int row, FieldMatrix<T> matrix) throws MatrixDimensionMismatchException, mathlib.exception.OutOfRangeException;
	   void setRowMatrix(int row, FieldMatrix<T> matrix);

	   /// <summary>
	   /// Get the entries in column number {@code column}
	   /// as a column matrix.
	   /// </summary>
	   /// <param name="column"> Column to be fetched. </param>
	   /// <returns> a column matrix. </returns>
	   /// <exception cref="OutOfRangeException"> if the specified column index is invalid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldMatrix<T> getColumnMatrix(int column) throws mathlib.exception.OutOfRangeException;
	   FieldMatrix<T> getColumnMatrix(int column);

	   /// <summary>
	   /// Set the entries in column number {@code column}
	   /// as a column matrix.
	   /// </summary>
	   /// <param name="column"> Column to be set. </param>
	   /// <param name="matrix"> column matrix (must have one column and the same
	   /// number of rows as the instance). </param>
	   /// <exception cref="OutOfRangeException"> if the specified column index is invalid. </exception>
	   /// <exception cref="MatrixDimensionMismatchException"> if the matrix dimensions do
	   /// not match one instance column. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setColumnMatrix(int column, FieldMatrix<T> matrix) throws MatrixDimensionMismatchException, mathlib.exception.OutOfRangeException;
	   void setColumnMatrix(int column, FieldMatrix<T> matrix);

	   /// <summary>
	   /// Get the entries in row number {@code row}
	   /// as a vector.
	   /// </summary>
	   /// <param name="row"> Row to be fetched </param>
	   /// <returns> a row vector. </returns>
	   /// <exception cref="OutOfRangeException"> if the specified row index is invalid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> getRowVector(int row) throws mathlib.exception.OutOfRangeException;
	   FieldVector<T> getRowVector(int row);

	   /// <summary>
	   /// Set the entries in row number {@code row}
	   /// as a vector.
	   /// </summary>
	   /// <param name="row"> Row to be set. </param>
	   /// <param name="vector"> row vector (must have the same number of columns
	   /// as the instance). </param>
	   /// <exception cref="OutOfRangeException"> if the specified row index is invalid. </exception>
	   /// <exception cref="MatrixDimensionMismatchException"> if the vector dimension does not
	   /// match one instance row. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setRowVector(int row, FieldVector<T> vector) throws MatrixDimensionMismatchException, mathlib.exception.OutOfRangeException;
	   void setRowVector(int row, FieldVector<T> vector);

	   /// <summary>
	   /// Returns the entries in column number {@code column}
	   /// as a vector.
	   /// </summary>
	   /// <param name="column"> Column to be fetched. </param>
	   /// <returns> a column vector. </returns>
	   /// <exception cref="OutOfRangeException"> if the specified column index is invalid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> getColumnVector(int column) throws mathlib.exception.OutOfRangeException;
	   FieldVector<T> getColumnVector(int column);

	   /// <summary>
	   /// Set the entries in column number {@code column}
	   /// as a vector.
	   /// </summary>
	   /// <param name="column"> Column to be set. </param>
	   /// <param name="vector"> Column vector (must have the same number of rows
	   /// as the instance). </param>
	   /// <exception cref="OutOfRangeException"> if the specified column index is invalid. </exception>
	   /// <exception cref="MatrixDimensionMismatchException"> if the vector dimension does not
	   /// match one instance column. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setColumnVector(int column, FieldVector<T> vector) throws MatrixDimensionMismatchException, mathlib.exception.OutOfRangeException;
	   void setColumnVector(int column, FieldVector<T> vector);

		/// <summary>
		/// Get the entries in row number {@code row} as an array.
		/// </summary>
		/// <param name="row"> Row to be fetched. </param>
		/// <returns> array of entries in the row. </returns>
		/// <exception cref="OutOfRangeException"> if the specified row index is not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T[] getRow(int row) throws mathlib.exception.OutOfRangeException;
		T[] getRow(int row);

		/// <summary>
		/// Set the entries in row number {@code row}
		/// as a row matrix.
		/// </summary>
		/// <param name="row"> Row to be set. </param>
		/// <param name="array"> Row matrix (must have the same number of columns as
		/// the instance). </param>
		/// <exception cref="OutOfRangeException"> if the specified row index is invalid. </exception>
		/// <exception cref="MatrixDimensionMismatchException"> if the array size does not match
		/// one instance row. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setRow(int row, T[] array) throws MatrixDimensionMismatchException, mathlib.exception.OutOfRangeException;
		void setRow(int row, T[] array);

		/// <summary>
		/// Get the entries in column number {@code col} as an array.
		/// </summary>
		/// <param name="column"> the column to be fetched </param>
		/// <returns> array of entries in the column </returns>
		/// <exception cref="OutOfRangeException"> if the specified column index is not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T[] getColumn(int column) throws mathlib.exception.OutOfRangeException;
		T[] getColumn(int column);

		/// <summary>
		/// Set the entries in column number {@code column}
		/// as a column matrix.
		/// </summary>
		/// <param name="column"> the column to be set </param>
		/// <param name="array"> column array (must have the same number of rows as the instance) </param>
		/// <exception cref="OutOfRangeException"> if the specified column index is invalid. </exception>
		/// <exception cref="MatrixDimensionMismatchException"> if the array size does not match
		/// one instance column. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setColumn(int column, T[] array) throws MatrixDimensionMismatchException, mathlib.exception.OutOfRangeException;
		void setColumn(int column, T[] array);

		/// <summary>
		/// Returns the entry in the specified row and column.
		/// </summary>
		/// <param name="row">  row location of entry to be fetched </param>
		/// <param name="column">  column location of entry to be fetched </param>
		/// <returns> matrix entry in row,column </returns>
		/// <exception cref="OutOfRangeException"> if the row or column index is not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T getEntry(int row, int column) throws mathlib.exception.OutOfRangeException;
		T getEntry(int row, int column);

		/// <summary>
		/// Set the entry in the specified row and column.
		/// </summary>
		/// <param name="row">  row location of entry to be set </param>
		/// <param name="column">  column location of entry to be set </param>
		/// <param name="value"> matrix entry to be set in row,column </param>
		/// <exception cref="OutOfRangeException"> if the row or column index is not valid.
		/// @since 2.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setEntry(int row, int column, T value) throws mathlib.exception.OutOfRangeException;
		void setEntry(int row, int column, T value);

		/// <summary>
		/// Change an entry in the specified row and column.
		/// </summary>
		/// <param name="row"> Row location of entry to be set. </param>
		/// <param name="column"> Column location of entry to be set. </param>
		/// <param name="increment"> Value to add to the current matrix entry in
		/// {@code (row, column)}. </param>
		/// <exception cref="OutOfRangeException"> if the row or column index is not valid.
		/// @since 2.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void addToEntry(int row, int column, T increment) throws mathlib.exception.OutOfRangeException;
		void addToEntry(int row, int column, T increment);

		/// <summary>
		/// Change an entry in the specified row and column.
		/// </summary>
		/// <param name="row"> Row location of entry to be set. </param>
		/// <param name="column"> Column location of entry to be set. </param>
		/// <param name="factor"> Multiplication factor for the current matrix entry
		/// in {@code (row,column)} </param>
		/// <exception cref="OutOfRangeException"> if the row or column index is not valid.
		/// @since 2.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void multiplyEntry(int row, int column, T factor) throws mathlib.exception.OutOfRangeException;
		void multiplyEntry(int row, int column, T factor);

		/// <summary>
		/// Returns the transpose of this matrix.
		/// </summary>
		/// <returns> transpose matrix </returns>
		FieldMatrix<T> transpose();

		/// <summary>
		/// Returns the <a href="http://mathworld.wolfram.com/MatrixTrace.html">
		/// trace</a> of the matrix (the sum of the elements on the main diagonal).
		/// </summary>
		/// <returns> trace </returns>
		/// <exception cref="NonSquareMatrixException"> if the matrix is not square. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T getTrace() throws NonSquareMatrixException;
		T Trace {get;}

		/// <summary>
		/// Returns the result of multiplying this by the vector {@code v}.
		/// </summary>
		/// <param name="v"> the vector to operate on </param>
		/// <returns> {@code this * v} </returns>
		/// <exception cref="DimensionMismatchException"> if the number of columns of
		/// {@code this} matrix is not equal to the size of the vector {@code v}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T[] operate(T[] v) throws mathlib.exception.DimensionMismatchException;
		T[] operate(T[] v);

		/// <summary>
		/// Returns the result of multiplying this by the vector {@code v}.
		/// </summary>
		/// <param name="v"> the vector to operate on </param>
		/// <returns> {@code this * v} </returns>
		/// <exception cref="DimensionMismatchException"> if the number of columns of
		/// {@code this} matrix is not equal to the size of the vector {@code v}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> operate(FieldVector<T> v) throws mathlib.exception.DimensionMismatchException;
		FieldVector<T> operate(FieldVector<T> v);

		/// <summary>
		/// Returns the (row) vector result of premultiplying this by the vector
		/// {@code v}.
		/// </summary>
		/// <param name="v"> the row vector to premultiply by </param>
		/// <returns> {@code v * this} </returns>
		/// <exception cref="DimensionMismatchException"> if the number of rows of {@code this}
		/// matrix is not equal to the size of the vector {@code v} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T[] preMultiply(T[] v) throws mathlib.exception.DimensionMismatchException;
		T[] preMultiply(T[] v);

		/// <summary>
		/// Returns the (row) vector result of premultiplying this by the vector
		/// {@code v}.
		/// </summary>
		/// <param name="v"> the row vector to premultiply by </param>
		/// <returns> {@code v * this} </returns>
		/// <exception cref="DimensionMismatchException"> if the number of rows of {@code this}
		/// matrix is not equal to the size of the vector {@code v} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: FieldVector<T> preMultiply(FieldVector<T> v) throws mathlib.exception.DimensionMismatchException;
		FieldVector<T> preMultiply(FieldVector<T> v);

		/// <summary>
		/// Visit (and possibly change) all matrix entries in row order.
		/// <p>Row order starts at upper left and iterating through all elements
		/// of a row from left to right before going to the leftmost element
		/// of the next row.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="FieldMatrixChangingVisitor#end()"/> at the end
		/// of the walk </returns>
		T walkInRowOrder(FieldMatrixChangingVisitor<T> visitor);

		/// <summary>
		/// Visit (but don't change) all matrix entries in row order.
		/// <p>Row order starts at upper left and iterating through all elements
		/// of a row from left to right before going to the leftmost element
		/// of the next row.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="FieldMatrixPreservingVisitor#end()"/> at the end
		/// of the walk </returns>
		T walkInRowOrder(FieldMatrixPreservingVisitor<T> visitor);

		/// <summary>
		/// Visit (and possibly change) some matrix entries in row order.
		/// <p>Row order starts at upper left and iterating through all elements
		/// of a row from left to right before going to the leftmost element
		/// of the next row.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <param name="startRow"> Initial row index </param>
		/// <param name="endRow"> Final row index (inclusive) </param>
		/// <param name="startColumn"> Initial column index </param>
		/// <param name="endColumn"> Final column index </param>
		/// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code endRow < startRow} or
		/// {@code endColumn < startColumn}. </exception>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="FieldMatrixChangingVisitor#end()"/> at the end
		/// of the walk </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T walkInRowOrder(FieldMatrixChangingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn) throws mathlib.exception.OutOfRangeException, mathlib.exception.NumberIsTooSmallException;
		T walkInRowOrder(FieldMatrixChangingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn);

		/// <summary>
		/// Visit (but don't change) some matrix entries in row order.
		/// <p>Row order starts at upper left and iterating through all elements
		/// of a row from left to right before going to the leftmost element
		/// of the next row.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <param name="startRow"> Initial row index </param>
		/// <param name="endRow"> Final row index (inclusive) </param>
		/// <param name="startColumn"> Initial column index </param>
		/// <param name="endColumn"> Final column index </param>
		/// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code endRow < startRow} or
		/// {@code endColumn < startColumn}. </exception>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="FieldMatrixPreservingVisitor#end()"/> at the end
		/// of the walk </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T walkInRowOrder(FieldMatrixPreservingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn) throws mathlib.exception.OutOfRangeException, mathlib.exception.NumberIsTooSmallException;
		T walkInRowOrder(FieldMatrixPreservingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn);

		/// <summary>
		/// Visit (and possibly change) all matrix entries in column order.
		/// <p>Column order starts at upper left and iterating through all elements
		/// of a column from top to bottom before going to the topmost element
		/// of the next column.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="FieldMatrixChangingVisitor#end()"/> at the end
		/// of the walk </returns>
		T walkInColumnOrder(FieldMatrixChangingVisitor<T> visitor);

		/// <summary>
		/// Visit (but don't change) all matrix entries in column order.
		/// <p>Column order starts at upper left and iterating through all elements
		/// of a column from top to bottom before going to the topmost element
		/// of the next column.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="FieldMatrixPreservingVisitor#end()"/> at the end
		/// of the walk </returns>
		T walkInColumnOrder(FieldMatrixPreservingVisitor<T> visitor);

		/// <summary>
		/// Visit (and possibly change) some matrix entries in column order.
		/// <p>Column order starts at upper left and iterating through all elements
		/// of a column from top to bottom before going to the topmost element
		/// of the next column.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <param name="startRow"> Initial row index </param>
		/// <param name="endRow"> Final row index (inclusive) </param>
		/// <param name="startColumn"> Initial column index </param>
		/// <param name="endColumn"> Final column index </param>
		/// <exception cref="NumberIsTooSmallException"> if {@code endRow < startRow} or
		/// {@code endColumn < startColumn}. </exception>
		/// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="FieldMatrixChangingVisitor#end()"/> at the end
		/// of the walk </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T walkInColumnOrder(FieldMatrixChangingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException;
		T walkInColumnOrder(FieldMatrixChangingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn);

		/// <summary>
		/// Visit (but don't change) some matrix entries in column order.
		/// <p>Column order starts at upper left and iterating through all elements
		/// of a column from top to bottom before going to the topmost element
		/// of the next column.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <param name="startRow"> Initial row index </param>
		/// <param name="endRow"> Final row index (inclusive) </param>
		/// <param name="startColumn"> Initial column index </param>
		/// <param name="endColumn"> Final column index </param>
		/// <exception cref="NumberIsTooSmallException"> if {@code endRow < startRow} or
		/// {@code endColumn < startColumn}. </exception>
		/// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="FieldMatrixPreservingVisitor#end()"/> at the end
		/// of the walk </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T walkInColumnOrder(FieldMatrixPreservingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException;
		T walkInColumnOrder(FieldMatrixPreservingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn);

		/// <summary>
		/// Visit (and possibly change) all matrix entries using the fastest possible order.
		/// <p>The fastest walking order depends on the exact matrix class. It may be
		/// different from traditional row or column orders.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="FieldMatrixChangingVisitor#end()"/> at the end
		/// of the walk </returns>
		T walkInOptimizedOrder(FieldMatrixChangingVisitor<T> visitor);

		/// <summary>
		/// Visit (but don't change) all matrix entries using the fastest possible order.
		/// <p>The fastest walking order depends on the exact matrix class. It may be
		/// different from traditional row or column orders.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="FieldMatrixPreservingVisitor#end()"/> at the end
		/// of the walk </returns>
		T walkInOptimizedOrder(FieldMatrixPreservingVisitor<T> visitor);

		/// <summary>
		/// Visit (and possibly change) some matrix entries using the fastest possible order.
		/// <p>The fastest walking order depends on the exact matrix class. It may be
		/// different from traditional row or column orders.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <param name="startRow"> Initial row index </param>
		/// <param name="endRow"> Final row index (inclusive) </param>
		/// <param name="startColumn"> Initial column index </param>
		/// <param name="endColumn"> Final column index (inclusive) </param>
		/// <exception cref="NumberIsTooSmallException"> if {@code endRow < startRow} or
		/// {@code endColumn < startColumn}. </exception>
		/// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="FieldMatrixChangingVisitor#end()"/> at the end
		/// of the walk </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T walkInOptimizedOrder(FieldMatrixChangingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException;
		T walkInOptimizedOrder(FieldMatrixChangingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn);

		/// <summary>
		/// Visit (but don't change) some matrix entries using the fastest possible order.
		/// <p>The fastest walking order depends on the exact matrix class. It may be
		/// different from traditional row or column orders.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <param name="startRow"> Initial row index </param>
		/// <param name="endRow"> Final row index (inclusive) </param>
		/// <param name="startColumn"> Initial column index </param>
		/// <param name="endColumn"> Final column index (inclusive) </param>
		/// <exception cref="NumberIsTooSmallException"> if {@code endRow < startRow} or
		/// {@code endColumn < startColumn}. </exception>
		/// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(FieldMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(FieldMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="FieldMatrixPreservingVisitor#end()"/> at the end
		/// of the walk </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T walkInOptimizedOrder(FieldMatrixPreservingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException;
		T walkInOptimizedOrder(FieldMatrixPreservingVisitor<T> visitor, int startRow, int endRow, int startColumn, int endColumn);
	}

}
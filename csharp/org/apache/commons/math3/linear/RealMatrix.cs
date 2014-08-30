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
	using NoDataException = org.apache.commons.math3.exception.NoDataException;
	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;

	/// <summary>
	/// Interface defining a real-valued matrix with basic algebraic operations.
	/// <p>
	/// Matrix element indexing is 0-based -- e.g., <code>getEntry(0, 0)</code>
	/// returns the element in the first row, first column of the matrix.</p>
	/// 
	/// @version $Id: RealMatrix.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public interface RealMatrix : AnyMatrix
	{

		/// <summary>
		/// Create a new RealMatrix of the same type as the instance with the
		/// supplied
		/// row and column dimensions.
		/// </summary>
		/// <param name="rowDimension"> the number of rows in the new matrix </param>
		/// <param name="columnDimension"> the number of columns in the new matrix </param>
		/// <returns> a new matrix of the same type as the instance </returns>
		/// <exception cref="NotStrictlyPositiveException"> if row or column dimension is not
		/// positive.
		/// @since 2.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RealMatrix createMatrix(int rowDimension, int columnDimension) throws org.apache.commons.math3.exception.NotStrictlyPositiveException;
		RealMatrix createMatrix(int rowDimension, int columnDimension);

		/// <summary>
		/// Returns a (deep) copy of this.
		/// </summary>
		/// <returns> matrix copy </returns>
		RealMatrix copy();

		/// <summary>
		/// Returns the sum of {@code this} and {@code m}.
		/// </summary>
		/// <param name="m"> matrix to be added </param>
		/// <returns> {@code this + m} </returns>
		/// <exception cref="MatrixDimensionMismatchException"> if {@code m} is not the same
		/// size as {@code this}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RealMatrix add(RealMatrix m) throws MatrixDimensionMismatchException;
		RealMatrix add(RealMatrix m);

		/// <summary>
		/// Returns {@code this} minus {@code m}.
		/// </summary>
		/// <param name="m"> matrix to be subtracted </param>
		/// <returns> {@code this - m} </returns>
		/// <exception cref="MatrixDimensionMismatchException"> if {@code m} is not the same
		/// size as {@code this}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RealMatrix subtract(RealMatrix m) throws MatrixDimensionMismatchException;
		RealMatrix subtract(RealMatrix m);

		/// <summary>
		/// Returns the result of adding {@code d} to each entry of {@code this}.
		/// </summary>
		/// <param name="d"> value to be added to each entry </param>
		/// <returns> {@code d + this} </returns>
		RealMatrix scalarAdd(double d);

		/// <summary>
		/// Returns the result of multiplying each entry of {@code this} by
		/// {@code d}.
		/// </summary>
		/// <param name="d"> value to multiply all entries by </param>
		/// <returns> {@code d * this} </returns>
		RealMatrix scalarMultiply(double d);

		/// <summary>
		/// Returns the result of postmultiplying {@code this} by {@code m}.
		/// </summary>
		/// <param name="m"> matrix to postmultiply by </param>
		/// <returns> {@code this * m} </returns>
		/// <exception cref="DimensionMismatchException"> if
		/// {@code columnDimension(this) != rowDimension(m)} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RealMatrix multiply(RealMatrix m) throws org.apache.commons.math3.exception.DimensionMismatchException;
		RealMatrix multiply(RealMatrix m);

		/// <summary>
		/// Returns the result of premultiplying {@code this} by {@code m}.
		/// </summary>
		/// <param name="m"> matrix to premultiply by </param>
		/// <returns> {@code m * this} </returns>
		/// <exception cref="DimensionMismatchException"> if
		/// {@code rowDimension(this) != columnDimension(m)} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RealMatrix preMultiply(RealMatrix m) throws org.apache.commons.math3.exception.DimensionMismatchException;
		RealMatrix preMultiply(RealMatrix m);

		/// <summary>
		/// Returns the result of multiplying {@code this} with itself {@code p}
		/// times. Depending on the underlying storage, instability for high powers
		/// might occur.
		/// </summary>
		/// <param name="p"> raise {@code this} to power {@code p} </param>
		/// <returns> {@code this^p} </returns>
		/// <exception cref="NotPositiveException"> if {@code p < 0} </exception>
		/// <exception cref="NonSquareMatrixException"> if the matrix is not square </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RealMatrix power(final int p) throws org.apache.commons.math3.exception.NotPositiveException, NonSquareMatrixException;
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		RealMatrix power(int p);

		/// <summary>
		/// Returns matrix entries as a two-dimensional array.
		/// </summary>
		/// <returns> 2-dimensional array of entries </returns>
		double[][] Data {get;}

		/// <summary>
		/// Returns the <a href="http://mathworld.wolfram.com/MaximumAbsoluteRowSumNorm.html">
		/// maximum absolute row sum norm</a> of the matrix.
		/// </summary>
		/// <returns> norm </returns>
		double Norm {get;}

		/// <summary>
		/// Returns the <a href="http://mathworld.wolfram.com/FrobeniusNorm.html">
		/// Frobenius norm</a> of the matrix.
		/// </summary>
		/// <returns> norm </returns>
		double FrobeniusNorm {get;}

		/// <summary>
		/// Gets a submatrix. Rows and columns are indicated
		/// counting from 0 to n-1.
		/// </summary>
		/// <param name="startRow"> Initial row index </param>
		/// <param name="endRow"> Final row index (inclusive) </param>
		/// <param name="startColumn"> Initial column index </param>
		/// <param name="endColumn"> Final column index (inclusive) </param>
		/// <returns> The subMatrix containing the data of the
		/// specified rows and columns. </returns>
		/// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code endRow < startRow} or
		/// {@code endColumn < startColumn}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RealMatrix getSubMatrix(int startRow, int endRow, int startColumn, int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException;
		RealMatrix getSubMatrix(int startRow, int endRow, int startColumn, int endColumn);

		/// <summary>
		/// Gets a submatrix. Rows and columns are indicated counting from 0 to n-1.
		/// </summary>
		/// <param name="selectedRows"> Array of row indices. </param>
		/// <param name="selectedColumns"> Array of column indices. </param>
		/// <returns> The subMatrix containing the data in the specified rows and
		/// columns </returns>
		/// <exception cref="NullArgumentException"> if the row or column selections are
		/// {@code null} </exception>
		/// <exception cref="NoDataException"> if the row or column selections are empty (zero
		/// length). </exception>
		/// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RealMatrix getSubMatrix(int[] selectedRows, int[] selectedColumns) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.OutOfRangeException;
		RealMatrix getSubMatrix(int[] selectedRows, int[] selectedColumns);

		/// <summary>
		/// Copy a submatrix. Rows and columns are indicated counting from 0 to n-1.
		/// </summary>
		/// <param name="startRow"> Initial row index </param>
		/// <param name="endRow"> Final row index (inclusive) </param>
		/// <param name="startColumn"> Initial column index </param>
		/// <param name="endColumn"> Final column index (inclusive) </param>
		/// <param name="destination"> The arrays where the submatrix data should be copied
		/// (if larger than rows/columns counts, only the upper-left part will be
		/// used) </param>
		/// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code endRow < startRow} or
		/// {@code endColumn < startColumn}. </exception>
		/// <exception cref="MatrixDimensionMismatchException"> if the destination array is too
		/// small. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void copySubMatrix(int startRow, int endRow, int startColumn, int endColumn, double[][] destination) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException, MatrixDimensionMismatchException;
		void copySubMatrix(int startRow, int endRow, int startColumn, int endColumn, double[][] destination);

		/// <summary>
		/// Copy a submatrix. Rows and columns are indicated counting from 0 to n-1.
		/// </summary>
		/// <param name="selectedRows"> Array of row indices. </param>
		/// <param name="selectedColumns"> Array of column indices. </param>
		/// <param name="destination"> The arrays where the submatrix data should be copied
		/// (if larger than rows/columns counts, only the upper-left part will be
		/// used) </param>
		/// <exception cref="NullArgumentException"> if the row or column selections are
		/// {@code null} </exception>
		/// <exception cref="NoDataException"> if the row or column selections are empty (zero
		/// length). </exception>
		/// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
		/// <exception cref="MatrixDimensionMismatchException"> if the destination array is too
		/// small. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void copySubMatrix(int[] selectedRows, int[] selectedColumns, double[][] destination) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoDataException, MatrixDimensionMismatchException;
		void copySubMatrix(int[] selectedRows, int[] selectedColumns, double[][] destination);

	   /// <summary>
	   /// Replace the submatrix starting at {@code row, column} using data in the
	   /// input {@code subMatrix} array. Indexes are 0-based.
	   /// <p>
	   /// Example:<br>
	   /// Starting with <pre>
	   /// 1  2  3  4
	   /// 5  6  7  8
	   /// 9  0  1  2
	   /// </pre>
	   /// and <code>subMatrix = {{3, 4} {5,6}}</code>, invoking
	   /// {@code setSubMatrix(subMatrix,1,1))} will result in <pre>
	   /// 1  2  3  4
	   /// 5  3  4  8
	   /// 9  5  6  2
	   /// </pre></p>
	   /// </summary>
	   /// <param name="subMatrix">  array containing the submatrix replacement data </param>
	   /// <param name="row">  row coordinate of the top, left element to be replaced </param>
	   /// <param name="column">  column coordinate of the top, left element to be replaced </param>
	   /// <exception cref="NoDataException"> if {@code subMatrix} is empty. </exception>
	   /// <exception cref="OutOfRangeException"> if {@code subMatrix} does not fit into
	   /// this matrix from element in {@code (row, column)}. </exception>
	   /// <exception cref="DimensionMismatchException"> if {@code subMatrix} is not rectangular
	   /// (not all rows have the same length) or empty. </exception>
	   /// <exception cref="NullArgumentException"> if {@code subMatrix} is {@code null}.
	   /// @since 2.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setSubMatrix(double[][] subMatrix, int row, int column) throws org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NullArgumentException;
		void setSubMatrix(double[][] subMatrix, int row, int column);

	   /// <summary>
	   /// Get the entries at the given row index as a row matrix.  Row indices start
	   /// at 0.
	   /// </summary>
	   /// <param name="row"> Row to be fetched. </param>
	   /// <returns> row Matrix. </returns>
	   /// <exception cref="OutOfRangeException"> if the specified row index is invalid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RealMatrix getRowMatrix(int row) throws org.apache.commons.math3.exception.OutOfRangeException;
	   RealMatrix getRowMatrix(int row);

		/// <summary>
		/// Sets the specified {@code row} of {@code this} matrix to the entries of
		/// the specified row {@code matrix}. Row indices start at 0.
		/// </summary>
		/// <param name="row"> Row to be set. </param>
		/// <param name="matrix"> Row matrix to be copied (must have one row and the same
		/// number of columns as the instance). </param>
		/// <exception cref="OutOfRangeException"> if the specified row index is invalid. </exception>
		/// <exception cref="MatrixDimensionMismatchException"> if the row dimension of the
		/// {@code matrix} is not {@code 1}, or the column dimensions of {@code this}
		/// and {@code matrix} do not match. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setRowMatrix(int row, RealMatrix matrix) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException;
		void setRowMatrix(int row, RealMatrix matrix);

		/// <summary>
		/// Get the entries at the given column index as a column matrix. Column
		/// indices start at 0.
		/// </summary>
		/// <param name="column"> Column to be fetched. </param>
		/// <returns> column Matrix. </returns>
		/// <exception cref="OutOfRangeException"> if the specified column index is invalid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RealMatrix getColumnMatrix(int column) throws org.apache.commons.math3.exception.OutOfRangeException;
		RealMatrix getColumnMatrix(int column);

		/// <summary>
		/// Sets the specified {@code column} of {@code this} matrix to the entries
		/// of the specified column {@code matrix}. Column indices start at 0.
		/// </summary>
		/// <param name="column"> Column to be set. </param>
		/// <param name="matrix"> Column matrix to be copied (must have one column and the
		/// same number of rows as the instance). </param>
		/// <exception cref="OutOfRangeException"> if the specified column index is invalid. </exception>
		/// <exception cref="MatrixDimensionMismatchException"> if the column dimension of the
		/// {@code matrix} is not {@code 1}, or the row dimensions of {@code this}
		/// and {@code matrix} do not match. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setColumnMatrix(int column, RealMatrix matrix) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException;
		void setColumnMatrix(int column, RealMatrix matrix);

		/// <summary>
		/// Returns the entries in row number {@code row} as a vector. Row indices
		/// start at 0.
		/// </summary>
		/// <param name="row"> Row to be fetched. </param>
		/// <returns> a row vector. </returns>
		/// <exception cref="OutOfRangeException"> if the specified row index is invalid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RealVector getRowVector(int row) throws org.apache.commons.math3.exception.OutOfRangeException;
		RealVector getRowVector(int row);

		/// <summary>
		/// Sets the specified {@code row} of {@code this} matrix to the entries of
		/// the specified {@code vector}. Row indices start at 0.
		/// </summary>
		/// <param name="row"> Row to be set. </param>
		/// <param name="vector"> row vector to be copied (must have the same number of
		/// column as the instance). </param>
		/// <exception cref="OutOfRangeException"> if the specified row index is invalid. </exception>
		/// <exception cref="MatrixDimensionMismatchException"> if the {@code vector} dimension
		/// does not match the column dimension of {@code this} matrix. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setRowVector(int row, RealVector vector) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException;
		void setRowVector(int row, RealVector vector);

		/// <summary>
		/// Get the entries at the given column index as a vector. Column indices
		/// start at 0.
		/// </summary>
		/// <param name="column"> Column to be fetched. </param>
		/// <returns> a column vector. </returns>
		/// <exception cref="OutOfRangeException"> if the specified column index is invalid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RealVector getColumnVector(int column) throws org.apache.commons.math3.exception.OutOfRangeException;
		RealVector getColumnVector(int column);

		/// <summary>
		/// Sets the specified {@code column} of {@code this} matrix to the entries
		/// of the specified {@code vector}. Column indices start at 0.
		/// </summary>
		/// <param name="column"> Column to be set. </param>
		/// <param name="vector"> column vector to be copied (must have the same number of
		/// rows as the instance). </param>
		/// <exception cref="OutOfRangeException"> if the specified column index is invalid. </exception>
		/// <exception cref="MatrixDimensionMismatchException"> if the {@code vector} dimension
		/// does not match the row dimension of {@code this} matrix. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setColumnVector(int column, RealVector vector) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException;
		void setColumnVector(int column, RealVector vector);

		/// <summary>
		/// Get the entries at the given row index. Row indices start at 0.
		/// </summary>
		/// <param name="row"> Row to be fetched. </param>
		/// <returns> the array of entries in the row. </returns>
		/// <exception cref="OutOfRangeException"> if the specified row index is not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double[] getRow(int row) throws org.apache.commons.math3.exception.OutOfRangeException;
		double[] getRow(int row);

		/// <summary>
		/// Sets the specified {@code row} of {@code this} matrix to the entries
		/// of the specified {@code array}. Row indices start at 0.
		/// </summary>
		/// <param name="row"> Row to be set. </param>
		/// <param name="array"> Row matrix to be copied (must have the same number of
		/// columns as the instance) </param>
		/// <exception cref="OutOfRangeException"> if the specified row index is invalid. </exception>
		/// <exception cref="MatrixDimensionMismatchException"> if the {@code array} length does
		/// not match the column dimension of {@code this} matrix. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setRow(int row, double[] array) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException;
		void setRow(int row, double[] array);

		/// <summary>
		/// Get the entries at the given column index as an array. Column indices
		/// start at 0.
		/// </summary>
		/// <param name="column"> Column to be fetched. </param>
		/// <returns> the array of entries in the column. </returns>
		/// <exception cref="OutOfRangeException"> if the specified column index is not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double[] getColumn(int column) throws org.apache.commons.math3.exception.OutOfRangeException;
		double[] getColumn(int column);

		/// <summary>
		/// Sets the specified {@code column} of {@code this} matrix to the entries
		/// of the specified {@code array}. Column indices start at 0.
		/// </summary>
		/// <param name="column"> Column to be set. </param>
		/// <param name="array"> Column array to be copied (must have the same number of
		/// rows as the instance). </param>
		/// <exception cref="OutOfRangeException"> if the specified column index is invalid. </exception>
		/// <exception cref="MatrixDimensionMismatchException"> if the {@code array} length does
		/// not match the row dimension of {@code this} matrix. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setColumn(int column, double[] array) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException;
		void setColumn(int column, double[] array);

		/// <summary>
		/// Get the entry in the specified row and column. Row and column indices
		/// start at 0.
		/// </summary>
		/// <param name="row"> Row index of entry to be fetched. </param>
		/// <param name="column"> Column index of entry to be fetched. </param>
		/// <returns> the matrix entry at {@code (row, column)}. </returns>
		/// <exception cref="OutOfRangeException"> if the row or column index is not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double getEntry(int row, int column) throws org.apache.commons.math3.exception.OutOfRangeException;
		double getEntry(int row, int column);

		/// <summary>
		/// Set the entry in the specified row and column. Row and column indices
		/// start at 0.
		/// </summary>
		/// <param name="row"> Row index of entry to be set. </param>
		/// <param name="column"> Column index of entry to be set. </param>
		/// <param name="value"> the new value of the entry. </param>
		/// <exception cref="OutOfRangeException"> if the row or column index is not valid
		/// @since 2.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setEntry(int row, int column, double value) throws org.apache.commons.math3.exception.OutOfRangeException;
		void setEntry(int row, int column, double value);

		/// <summary>
		/// Adds (in place) the specified value to the specified entry of
		/// {@code this} matrix. Row and column indices start at 0.
		/// </summary>
		/// <param name="row"> Row index of the entry to be modified. </param>
		/// <param name="column"> Column index of the entry to be modified. </param>
		/// <param name="increment"> value to add to the matrix entry. </param>
		/// <exception cref="OutOfRangeException"> if the row or column index is not valid.
		/// @since 2.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void addToEntry(int row, int column, double increment) throws org.apache.commons.math3.exception.OutOfRangeException;
		void addToEntry(int row, int column, double increment);

		/// <summary>
		/// Multiplies (in place) the specified entry of {@code this} matrix by the
		/// specified value. Row and column indices start at 0.
		/// </summary>
		/// <param name="row"> Row index of the entry to be modified. </param>
		/// <param name="column"> Column index of the entry to be modified. </param>
		/// <param name="factor"> Multiplication factor for the matrix entry. </param>
		/// <exception cref="OutOfRangeException"> if the row or column index is not valid.
		/// @since 2.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void multiplyEntry(int row, int column, double factor) throws org.apache.commons.math3.exception.OutOfRangeException;
		void multiplyEntry(int row, int column, double factor);

		/// <summary>
		/// Returns the transpose of this matrix.
		/// </summary>
		/// <returns> transpose matrix </returns>
		RealMatrix transpose();

		/// <summary>
		/// Returns the <a href="http://mathworld.wolfram.com/MatrixTrace.html">
		/// trace</a> of the matrix (the sum of the elements on the main diagonal).
		/// </summary>
		/// <returns> the trace. </returns>
		/// <exception cref="NonSquareMatrixException"> if the matrix is not square. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double getTrace() throws NonSquareMatrixException;
		double Trace {get;}

		/// <summary>
		/// Returns the result of multiplying this by the vector {@code v}.
		/// </summary>
		/// <param name="v"> the vector to operate on </param>
		/// <returns> {@code this * v} </returns>
		/// <exception cref="DimensionMismatchException"> if the length of {@code v} does not
		/// match the column dimension of {@code this}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double[] operate(double[] v) throws org.apache.commons.math3.exception.DimensionMismatchException;
		double[] operate(double[] v);

		/// <summary>
		/// Returns the result of multiplying this by the vector {@code v}.
		/// </summary>
		/// <param name="v"> the vector to operate on </param>
		/// <returns> {@code this * v} </returns>
		/// <exception cref="DimensionMismatchException"> if the dimension of {@code v} does not
		/// match the column dimension of {@code this}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RealVector operate(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException;
		RealVector operate(RealVector v);

		/// <summary>
		/// Returns the (row) vector result of premultiplying this by the vector {@code v}.
		/// </summary>
		/// <param name="v"> the row vector to premultiply by </param>
		/// <returns> {@code v * this} </returns>
		/// <exception cref="DimensionMismatchException"> if the length of {@code v} does not
		/// match the row dimension of {@code this}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double[] preMultiply(double[] v) throws org.apache.commons.math3.exception.DimensionMismatchException;
		double[] preMultiply(double[] v);

		/// <summary>
		/// Returns the (row) vector result of premultiplying this by the vector {@code v}.
		/// </summary>
		/// <param name="v"> the row vector to premultiply by </param>
		/// <returns> {@code v * this} </returns>
		/// <exception cref="DimensionMismatchException"> if the dimension of {@code v} does not
		/// match the row dimension of {@code this}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RealVector preMultiply(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException;
		RealVector preMultiply(RealVector v);

		/// <summary>
		/// Visit (and possibly change) all matrix entries in row order.
		/// <p>Row order starts at upper left and iterating through all elements
		/// of a row from left to right before going to the leftmost element
		/// of the next row.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="RealMatrixChangingVisitor#end()"/> at the end
		/// of the walk </returns>
		double walkInRowOrder(RealMatrixChangingVisitor visitor);

		/// <summary>
		/// Visit (but don't change) all matrix entries in row order.
		/// <p>Row order starts at upper left and iterating through all elements
		/// of a row from left to right before going to the leftmost element
		/// of the next row.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="RealMatrixPreservingVisitor#end()"/> at the end
		/// of the walk </returns>
		double walkInRowOrder(RealMatrixPreservingVisitor visitor);

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
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="RealMatrixChangingVisitor#end()"/> at the end
		/// of the walk </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double walkInRowOrder(RealMatrixChangingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException;
		double walkInRowOrder(RealMatrixChangingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn);

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
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="RealMatrixPreservingVisitor#end()"/> at the end
		/// of the walk </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double walkInRowOrder(RealMatrixPreservingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException;
		double walkInRowOrder(RealMatrixPreservingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn);

		/// <summary>
		/// Visit (and possibly change) all matrix entries in column order.
		/// <p>Column order starts at upper left and iterating through all elements
		/// of a column from top to bottom before going to the topmost element
		/// of the next column.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="RealMatrixChangingVisitor#end()"/> at the end
		/// of the walk </returns>
		double walkInColumnOrder(RealMatrixChangingVisitor visitor);

		/// <summary>
		/// Visit (but don't change) all matrix entries in column order.
		/// <p>Column order starts at upper left and iterating through all elements
		/// of a column from top to bottom before going to the topmost element
		/// of the next column.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="RealMatrixPreservingVisitor#end()"/> at the end
		/// of the walk </returns>
		double walkInColumnOrder(RealMatrixPreservingVisitor visitor);

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
		/// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code endRow < startRow} or
		/// {@code endColumn < startColumn}. </exception>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="RealMatrixChangingVisitor#end()"/> at the end
		/// of the walk </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double walkInColumnOrder(RealMatrixChangingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException;
		double walkInColumnOrder(RealMatrixChangingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn);

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
		/// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code endRow < startRow} or
		/// {@code endColumn < startColumn}. </exception>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="RealMatrixPreservingVisitor#end()"/> at the end
		/// of the walk </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double walkInColumnOrder(RealMatrixPreservingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException;
		double walkInColumnOrder(RealMatrixPreservingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn);

		/// <summary>
		/// Visit (and possibly change) all matrix entries using the fastest possible order.
		/// <p>The fastest walking order depends on the exact matrix class. It may be
		/// different from traditional row or column orders.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="RealMatrixChangingVisitor#end()"/> at the end
		/// of the walk </returns>
		double walkInOptimizedOrder(RealMatrixChangingVisitor visitor);

		/// <summary>
		/// Visit (but don't change) all matrix entries using the fastest possible order.
		/// <p>The fastest walking order depends on the exact matrix class. It may be
		/// different from traditional row or column orders.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="RealMatrixPreservingVisitor#end()"/> at the end
		/// of the walk </returns>
		double walkInOptimizedOrder(RealMatrixPreservingVisitor visitor);

		/// <summary>
		/// Visit (and possibly change) some matrix entries using the fastest possible order.
		/// <p>The fastest walking order depends on the exact matrix class. It may be
		/// different from traditional row or column orders.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <param name="startRow"> Initial row index </param>
		/// <param name="endRow"> Final row index (inclusive) </param>
		/// <param name="startColumn"> Initial column index </param>
		/// <param name="endColumn"> Final column index (inclusive) </param>
		/// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code endRow < startRow} or
		/// {@code endColumn < startColumn}. </exception>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="RealMatrixChangingVisitor#end()"/> at the end
		/// of the walk </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double walkInOptimizedOrder(RealMatrixChangingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException;
		double walkInOptimizedOrder(RealMatrixChangingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn);

		/// <summary>
		/// Visit (but don't change) some matrix entries using the fastest possible order.
		/// <p>The fastest walking order depends on the exact matrix class. It may be
		/// different from traditional row or column orders.</p> </summary>
		/// <param name="visitor"> visitor used to process all matrix entries </param>
		/// <param name="startRow"> Initial row index </param>
		/// <param name="endRow"> Final row index (inclusive) </param>
		/// <param name="startColumn"> Initial column index </param>
		/// <param name="endColumn"> Final column index (inclusive) </param>
		/// <exception cref="OutOfRangeException"> if the indices are not valid. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code endRow < startRow} or
		/// {@code endColumn < startColumn}. </exception>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInRowOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInColumnOrder(RealMatrixPreservingVisitor, int, int, int, int) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixPreservingVisitor) </seealso>
		/// <seealso cref= #walkInOptimizedOrder(RealMatrixChangingVisitor, int, int, int, int) </seealso>
		/// <returns> the value returned by <seealso cref="RealMatrixPreservingVisitor#end()"/> at the end
		/// of the walk </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double walkInOptimizedOrder(RealMatrixPreservingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException;
		double walkInOptimizedOrder(RealMatrixPreservingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn);
	}

}
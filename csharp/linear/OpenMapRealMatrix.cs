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
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using OpenIntToDoubleHashMap = org.apache.commons.math3.util.OpenIntToDoubleHashMap;

	/// <summary>
	/// Sparse matrix implementation based on an open addressed map.
	/// 
	/// <p>
	///  Caveat: This implementation assumes that, for any {@code x},
	///  the equality {@code x * 0d == 0d} holds. But it is is not true for
	///  {@code NaN}. Moreover, zero entries will lose their sign.
	///  Some operations (that involve {@code NaN} and/or infinities) may
	///  thus give incorrect results.
	/// </p>
	/// @version $Id: OpenMapRealMatrix.java 1569825 2014-02-19 17:19:59Z luc $
	/// @since 2.0
	/// </summary>
	[Serializable]
	public class OpenMapRealMatrix : AbstractRealMatrix, SparseRealMatrix
	{
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -5962461716457143437L;
		/// <summary>
		/// Number of rows of the matrix. </summary>
		private readonly int rows;
		/// <summary>
		/// Number of columns of the matrix. </summary>
		private readonly int columns;
		/// <summary>
		/// Storage for (sparse) matrix elements. </summary>
		private readonly OpenIntToDoubleHashMap entries;

		/// <summary>
		/// Build a sparse matrix with the supplied row and column dimensions.
		/// </summary>
		/// <param name="rowDimension"> Number of rows of the matrix. </param>
		/// <param name="columnDimension"> Number of columns of the matrix. </param>
		/// <exception cref="NotStrictlyPositiveException"> if row or column dimension is not
		/// positive. </exception>
		/// <exception cref="NumberIsTooLargeException"> if the total number of entries of the
		/// matrix is larger than {@code Integer.MAX_VALUE}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public OpenMapRealMatrix(int rowDimension, int columnDimension) throws org.apache.commons.math3.exception.NotStrictlyPositiveException, org.apache.commons.math3.exception.NumberIsTooLargeException
		public OpenMapRealMatrix(int rowDimension, int columnDimension) : base(rowDimension, columnDimension)
		{
			long lRow = rowDimension;
			long lCol = columnDimension;
			if (lRow * lCol >= int.MaxValue)
			{
				throw new NumberIsTooLargeException(lRow * lCol, int.MaxValue, false);
			}
			this.rows = rowDimension;
			this.columns = columnDimension;
			this.entries = new OpenIntToDoubleHashMap(0.0);
		}

		/// <summary>
		/// Build a matrix by copying another one.
		/// </summary>
		/// <param name="matrix"> matrix to copy. </param>
		public OpenMapRealMatrix(OpenMapRealMatrix matrix)
		{
			this.rows = matrix.rows;
			this.columns = matrix.columns;
			this.entries = new OpenIntToDoubleHashMap(matrix.entries);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override OpenMapRealMatrix copy()
		{
			return new OpenMapRealMatrix(this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="NumberIsTooLargeException"> if the total number of entries of the
		/// matrix is larger than {@code Integer.MAX_VALUE}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public OpenMapRealMatrix createMatrix(int rowDimension, int columnDimension) throws org.apache.commons.math3.exception.NotStrictlyPositiveException, org.apache.commons.math3.exception.NumberIsTooLargeException
		public override OpenMapRealMatrix createMatrix(int rowDimension, int columnDimension)
		{
			return new OpenMapRealMatrix(rowDimension, columnDimension);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int ColumnDimension
		{
			get
			{
				return columns;
			}
		}

		/// <summary>
		/// Compute the sum of this matrix and {@code m}.
		/// </summary>
		/// <param name="m"> Matrix to be added. </param>
		/// <returns> {@code this} + {@code m}. </returns>
		/// <exception cref="MatrixDimensionMismatchException"> if {@code m} is not the same
		/// size as {@code this}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public OpenMapRealMatrix add(OpenMapRealMatrix m) throws MatrixDimensionMismatchException
		public virtual OpenMapRealMatrix add(OpenMapRealMatrix m)
		{

			MatrixUtils.checkAdditionCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final OpenMapRealMatrix out = new OpenMapRealMatrix(this);
			OpenMapRealMatrix @out = new OpenMapRealMatrix(this);
			for (OpenIntToDoubleHashMap.Iterator iterator = m.entries.GetEnumerator(); iterator.hasNext();)
			{
				iterator.advance();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int row = iterator.key() / columns;
				int row = iterator.key() / columns;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int col = iterator.key() - row * columns;
				int col = iterator.key() - row * columns;
				@out.setEntry(row, col, getEntry(row, col) + iterator.value());
			}

			return @out;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public OpenMapRealMatrix subtract(final RealMatrix m) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override OpenMapRealMatrix subtract(RealMatrix m)
		{
			try
			{
				return subtract((OpenMapRealMatrix) m);
			}
			catch (System.InvalidCastException cce)
			{
				return (OpenMapRealMatrix) base.subtract(m);
			}
		}

		/// <summary>
		/// Subtract {@code m} from this matrix.
		/// </summary>
		/// <param name="m"> Matrix to be subtracted. </param>
		/// <returns> {@code this} - {@code m}. </returns>
		/// <exception cref="MatrixDimensionMismatchException"> if {@code m} is not the same
		/// size as {@code this}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public OpenMapRealMatrix subtract(OpenMapRealMatrix m) throws MatrixDimensionMismatchException
		public virtual OpenMapRealMatrix subtract(OpenMapRealMatrix m)
		{
			MatrixUtils.checkAdditionCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final OpenMapRealMatrix out = new OpenMapRealMatrix(this);
			OpenMapRealMatrix @out = new OpenMapRealMatrix(this);
			for (OpenIntToDoubleHashMap.Iterator iterator = m.entries.GetEnumerator(); iterator.hasNext();)
			{
				iterator.advance();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int row = iterator.key() / columns;
				int row = iterator.key() / columns;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int col = iterator.key() - row * columns;
				int col = iterator.key() - row * columns;
				@out.setEntry(row, col, getEntry(row, col) - iterator.value());
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="NumberIsTooLargeException"> if {@code m} is an
		/// {@code OpenMapRealMatrix}, and the total number of entries of the product
		/// is larger than {@code Integer.MAX_VALUE}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealMatrix multiply(final RealMatrix m) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealMatrix multiply(RealMatrix m)
		{
			try
			{
				return multiply((OpenMapRealMatrix) m);
			}
			catch (System.InvalidCastException cce)
			{

				MatrixUtils.checkMultiplicationCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int outCols = m.getColumnDimension();
				int outCols = m.ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BlockRealMatrix out = new BlockRealMatrix(rows, outCols);
				BlockRealMatrix @out = new BlockRealMatrix(rows, outCols);
				for (OpenIntToDoubleHashMap.Iterator iterator = entries.GetEnumerator(); iterator.hasNext();)
				{
					iterator.advance();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double value = iterator.value();
					double value = iterator.value();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int key = iterator.key();
					int key = iterator.key();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i = key / columns;
					int i = key / columns;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int k = key % columns;
					int k = key % columns;
					for (int j = 0; j < outCols; ++j)
					{
						@out.addToEntry(i, j, value * m.getEntry(k, j));
					}
				}

				return @out;
			}
		}

		/// <summary>
		/// Postmultiply this matrix by {@code m}.
		/// </summary>
		/// <param name="m"> Matrix to postmultiply by. </param>
		/// <returns> {@code this} * {@code m}. </returns>
		/// <exception cref="DimensionMismatchException"> if the number of rows of {@code m}
		/// differ from the number of columns of {@code this} matrix. </exception>
		/// <exception cref="NumberIsTooLargeException"> if the total number of entries of the
		/// product is larger than {@code Integer.MAX_VALUE}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public OpenMapRealMatrix multiply(OpenMapRealMatrix m) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NumberIsTooLargeException
		public virtual OpenMapRealMatrix multiply(OpenMapRealMatrix m)
		{
			// Safety check.
			MatrixUtils.checkMultiplicationCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int outCols = m.getColumnDimension();
			int outCols = m.ColumnDimension;
			OpenMapRealMatrix @out = new OpenMapRealMatrix(rows, outCols);
			for (OpenIntToDoubleHashMap.Iterator iterator = entries.GetEnumerator(); iterator.hasNext();)
			{
				iterator.advance();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double value = iterator.value();
				double value = iterator.value();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int key = iterator.key();
				int key = iterator.key();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i = key / columns;
				int i = key / columns;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int k = key % columns;
				int k = key % columns;
				for (int j = 0; j < outCols; ++j)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rightKey = m.computeKey(k, j);
					int rightKey = m.computeKey(k, j);
					if (m.entries.containsKey(rightKey))
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int outKey = out.computeKey(i, j);
						int outKey = @out.computeKey(i, j);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double outValue = out.entries.get(outKey) + value * m.entries.get(rightKey);
						double outValue = @out.entries.get(outKey) + value * m.entries.get(rightKey);
						if (outValue == 0.0)
						{
							@out.entries.remove(outKey);
						}
						else
						{
							@out.entries.put(outKey, outValue);
						}
					}
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double getEntry(int row, int column) throws org.apache.commons.math3.exception.OutOfRangeException
		public override double getEntry(int row, int column)
		{
			MatrixUtils.checkRowIndex(this, row);
			MatrixUtils.checkColumnIndex(this, column);
			return entries.get(computeKey(row, column));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int RowDimension
		{
			get
			{
				return rows;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setEntry(int row, int column, double value) throws org.apache.commons.math3.exception.OutOfRangeException
		public override void setEntry(int row, int column, double value)
		{
			MatrixUtils.checkRowIndex(this, row);
			MatrixUtils.checkColumnIndex(this, column);
			if (value == 0.0)
			{
				entries.remove(computeKey(row, column));
			}
			else
			{
				entries.put(computeKey(row, column), value);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void addToEntry(int row, int column, double increment) throws org.apache.commons.math3.exception.OutOfRangeException
		public override void addToEntry(int row, int column, double increment)
		{
			MatrixUtils.checkRowIndex(this, row);
			MatrixUtils.checkColumnIndex(this, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int key = computeKey(row, column);
			int key = computeKey(row, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double value = entries.get(key) + increment;
			double value = entries.get(key) + increment;
			if (value == 0.0)
			{
				entries.remove(key);
			}
			else
			{
				entries.put(key, value);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void multiplyEntry(int row, int column, double factor) throws org.apache.commons.math3.exception.OutOfRangeException
		public override void multiplyEntry(int row, int column, double factor)
		{
			MatrixUtils.checkRowIndex(this, row);
			MatrixUtils.checkColumnIndex(this, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int key = computeKey(row, column);
			int key = computeKey(row, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double value = entries.get(key) * factor;
			double value = entries.get(key) * factor;
			if (value == 0.0)
			{
				entries.remove(key);
			}
			else
			{
				entries.put(key, value);
			}
		}

		/// <summary>
		/// Compute the key to access a matrix element </summary>
		/// <param name="row"> row index of the matrix element </param>
		/// <param name="column"> column index of the matrix element </param>
		/// <returns> key within the map to access the matrix element </returns>
		private int computeKey(int row, int column)
		{
			return row * columns + column;
		}


	}

}
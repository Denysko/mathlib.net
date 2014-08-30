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
	using org.apache.commons.math3.util;

	/// <summary>
	/// Sparse matrix implementation based on an open addressed map.
	/// 
	/// <p>
	///  Caveat: This implementation assumes that, for any {@code x},
	///  the equality {@code x * 0d == 0d} holds. But it is is not true for
	///  {@code NaN}. Moreover, zero entries will lose their sign.
	///  Some operations (that involve {@code NaN} and/or infinities) may
	///  thus give incorrect results.
	/// </p> </summary>
	/// @param <T> the type of the field elements
	/// @version $Id: SparseFieldMatrix.java 1569825 2014-02-19 17:19:59Z luc $
	/// @since 2.0 </param>
	public class SparseFieldMatrix<T> : AbstractFieldMatrix<T> where T : org.apache.commons.math3.FieldElement<T>
	{

		/// <summary>
		/// Storage for (sparse) matrix elements. </summary>
		private readonly OpenIntToFieldHashMap<T> entries;
		/// <summary>
		/// Row dimension. </summary>
		private readonly int rows;
		/// <summary>
		/// Column dimension. </summary>
		private readonly int columns;

		/// <summary>
		/// Create a matrix with no data.
		/// </summary>
		/// <param name="field"> Field to which the elements belong. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseFieldMatrix(final org.apache.commons.math3.Field<T> field)
		public SparseFieldMatrix(Field<T> field) : base(field)
		{
			rows = 0;
			columns = 0;
			entries = new OpenIntToFieldHashMap<T>(field);
		}

		/// <summary>
		/// Create a new SparseFieldMatrix<T> with the supplied row and column
		/// dimensions.
		/// </summary>
		/// <param name="field"> Field to which the elements belong. </param>
		/// <param name="rowDimension"> Number of rows in the new matrix. </param>
		/// <param name="columnDimension"> Number of columns in the new matrix. </param>
		/// <exception cref="org.apache.commons.math3.exception.NotStrictlyPositiveException">
		/// if row or column dimension is not positive. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseFieldMatrix(final org.apache.commons.math3.Field<T> field, final int rowDimension, final int columnDimension)
		public SparseFieldMatrix(Field<T> field, int rowDimension, int columnDimension) : base(field, rowDimension, columnDimension)
		{
			this.rows = rowDimension;
			this.columns = columnDimension;
			entries = new OpenIntToFieldHashMap<T>(field);
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="other"> Instance to copy. </param>
		public SparseFieldMatrix(SparseFieldMatrix<T> other) : base(other.Field, other.RowDimension, other.ColumnDimension)
		{
			rows = other.RowDimension;
			columns = other.ColumnDimension;
			entries = new OpenIntToFieldHashMap<T>(other.entries);
		}

		/// <summary>
		/// Generic copy constructor.
		/// </summary>
		/// <param name="other"> Instance to copy. </param>
		public SparseFieldMatrix(FieldMatrix<T> other) : base(other.Field, other.RowDimension, other.ColumnDimension)
		{
			rows = other.RowDimension;
			columns = other.ColumnDimension;
			entries = new OpenIntToFieldHashMap<T>(Field);
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					setEntry(i, j, other.getEntry(i, j));
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override void addToEntry(int row, int column, T increment)
		{
			checkRowIndex(row);
			checkColumnIndex(column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int key = computeKey(row, column);
			int key = computeKey(row, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T value = entries.get(key).add(increment);
			T value = entries.get(key).add(increment);
			if (Field.Zero.Equals(value))
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
		public override FieldMatrix<T> copy()
		{
			return new SparseFieldMatrix<T>(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override FieldMatrix<T> createMatrix(int rowDimension, int columnDimension)
		{
			return new SparseFieldMatrix<T>(Field, rowDimension, columnDimension);
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
		/// {@inheritDoc} </summary>
		public override T getEntry(int row, int column)
		{
			checkRowIndex(row);
			checkColumnIndex(column);
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
		public override void multiplyEntry(int row, int column, T factor)
		{
			checkRowIndex(row);
			checkColumnIndex(column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int key = computeKey(row, column);
			int key = computeKey(row, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T value = entries.get(key).multiply(factor);
			T value = entries.get(key).multiply(factor);
			if (Field.Zero.Equals(value))
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
		public override void setEntry(int row, int column, T value)
		{
			checkRowIndex(row);
			checkColumnIndex(column);
			if (Field.Zero.Equals(value))
			{
				entries.remove(computeKey(row, column));
			}
			else
			{
				entries.put(computeKey(row, column), value);
			}
		}

		/// <summary>
		/// Compute the key to access a matrix element.
		/// </summary>
		/// <param name="row"> Row index of the matrix element. </param>
		/// <param name="column"> Column index of the matrix element. </param>
		/// <returns> the key within the map to access the matrix element. </returns>
		private int computeKey(int row, int column)
		{
			return row * columns + column;
		}
	}

}
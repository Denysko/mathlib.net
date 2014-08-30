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
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathUtils = org.apache.commons.math3.util.MathUtils;
	using Precision = org.apache.commons.math3.util.Precision;

	/// <summary>
	/// Implementation of a diagonal matrix.
	/// 
	/// @version $Id: DiagonalMatrix.java 1533638 2013-10-18 21:19:18Z tn $
	/// @since 3.1.1
	/// </summary>
	[Serializable]
	public class DiagonalMatrix : AbstractRealMatrix
	{
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 20121229L;
		/// <summary>
		/// Entries of the diagonal. </summary>
		private readonly double[] data;

		/// <summary>
		/// Creates a matrix with the supplied dimension.
		/// </summary>
		/// <param name="dimension"> Number of rows and columns in the new matrix. </param>
		/// <exception cref="NotStrictlyPositiveException"> if the dimension is
		/// not positive. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DiagonalMatrix(final int dimension) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public DiagonalMatrix(int dimension) : base(dimension, dimension)
		{
			data = new double[dimension];
		}

		/// <summary>
		/// Creates a matrix using the input array as the underlying data.
		/// <br/>
		/// The input array is copied, not referenced.
		/// </summary>
		/// <param name="d"> Data for the new matrix. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DiagonalMatrix(final double[] d)
		public DiagonalMatrix(double[] d) : this(d, true)
		{
		}

		/// <summary>
		/// Creates a matrix using the input array as the underlying data.
		/// <br/>
		/// If an array is created specially in order to be embedded in a
		/// this instance and not used directly, the {@code copyArray} may be
		/// set to {@code false}.
		/// This will prevent the copying and improve performance as no new
		/// array will be built and no data will be copied.
		/// </summary>
		/// <param name="d"> Data for new matrix. </param>
		/// <param name="copyArray"> if {@code true}, the input array will be copied,
		/// otherwise it will be referenced. </param>
		/// <exception cref="NullArgumentException"> if d is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DiagonalMatrix(final double[] d, final boolean copyArray) throws org.apache.commons.math3.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public DiagonalMatrix(double[] d, bool copyArray)
		{
			MathUtils.checkNotNull(d);
			data = copyArray ? d.clone() : d;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="DimensionMismatchException"> if the requested dimensions are not equal. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealMatrix createMatrix(final int rowDimension, final int columnDimension) throws org.apache.commons.math3.exception.NotStrictlyPositiveException, org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealMatrix createMatrix(int rowDimension, int columnDimension)
		{
			if (rowDimension != columnDimension)
			{
				throw new DimensionMismatchException(rowDimension, columnDimension);
			}

			return new DiagonalMatrix(rowDimension);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override RealMatrix copy()
		{
			return new DiagonalMatrix(data);
		}

		/// <summary>
		/// Compute the sum of {@code this} and {@code m}.
		/// </summary>
		/// <param name="m"> Matrix to be added. </param>
		/// <returns> {@code this + m}. </returns>
		/// <exception cref="MatrixDimensionMismatchException"> if {@code m} is not the same
		/// size as {@code this}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DiagonalMatrix add(final DiagonalMatrix m) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DiagonalMatrix add(DiagonalMatrix m)
		{
			// Safety check.
			MatrixUtils.checkAdditionCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = getRowDimension();
			int dim = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outData = new double[dim];
			double[] outData = new double[dim];
			for (int i = 0; i < dim; i++)
			{
				outData[i] = data[i] + m.data[i];
			}

			return new DiagonalMatrix(outData, false);
		}

		/// <summary>
		/// Returns {@code this} minus {@code m}.
		/// </summary>
		/// <param name="m"> Matrix to be subtracted. </param>
		/// <returns> {@code this - m} </returns>
		/// <exception cref="MatrixDimensionMismatchException"> if {@code m} is not the same
		/// size as {@code this}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DiagonalMatrix subtract(final DiagonalMatrix m) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DiagonalMatrix subtract(DiagonalMatrix m)
		{
			MatrixUtils.checkSubtractionCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = getRowDimension();
			int dim = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outData = new double[dim];
			double[] outData = new double[dim];
			for (int i = 0; i < dim; i++)
			{
				outData[i] = data[i] - m.data[i];
			}

			return new DiagonalMatrix(outData, false);
		}

		/// <summary>
		/// Returns the result of postmultiplying {@code this} by {@code m}.
		/// </summary>
		/// <param name="m"> matrix to postmultiply by </param>
		/// <returns> {@code this * m} </returns>
		/// <exception cref="DimensionMismatchException"> if
		/// {@code columnDimension(this) != rowDimension(m)} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DiagonalMatrix multiply(final DiagonalMatrix m) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DiagonalMatrix multiply(DiagonalMatrix m)
		{
			MatrixUtils.checkMultiplicationCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = getRowDimension();
			int dim = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outData = new double[dim];
			double[] outData = new double[dim];
			for (int i = 0; i < dim; i++)
			{
				outData[i] = data[i] * m.data[i];
			}

			return new DiagonalMatrix(outData, false);
		}

		/// <summary>
		/// Returns the result of postmultiplying {@code this} by {@code m}.
		/// </summary>
		/// <param name="m"> matrix to postmultiply by </param>
		/// <returns> {@code this * m} </returns>
		/// <exception cref="DimensionMismatchException"> if
		/// {@code columnDimension(this) != rowDimension(m)} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealMatrix multiply(final RealMatrix m) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealMatrix multiply(RealMatrix m)
		{
			if (m is DiagonalMatrix)
			{
				return multiply((DiagonalMatrix) m);
			}
			else
			{
				MatrixUtils.checkMultiplicationCompatible(this, m);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = m.getRowDimension();
				int nRows = m.RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = m.getColumnDimension();
				int nCols = m.ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] product = new double[nRows][nCols];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] product = new double[nRows][nCols];
				double[][] product = RectangularArrays.ReturnRectangularDoubleArray(nRows, nCols);
				for (int r = 0; r < nRows; r++)
				{
					for (int c = 0; c < nCols; c++)
					{
						product[r][c] = data[r] * m.getEntry(r, c);
					}
				}
				return new Array2DRowRealMatrix(product, false);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double[][] Data
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int dim = getRowDimension();
				int dim = RowDimension;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[][] out = new double[dim][dim];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] out = new double[dim][dim];
				double[][] @out = RectangularArrays.ReturnRectangularDoubleArray(dim, dim);
    
				for (int i = 0; i < dim; i++)
				{
					@out[i][i] = data[i];
				}
    
				return @out;
			}
		}

		/// <summary>
		/// Gets a reference to the underlying data array.
		/// </summary>
		/// <returns> 1-dimensional array of entries. </returns>
		public virtual double[] DataRef
		{
			get
			{
				return data;
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
			return row == column ? data[row] : 0;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="NumberIsTooLargeException"> if {@code row != column} and value is non-zero. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setEntry(final int row, final int column, final double value) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void setEntry(int row, int column, double value)
		{
			if (row == column)
			{
				MatrixUtils.checkRowIndex(this, row);
				data[row] = value;
			}
			else
			{
				ensureZero(value);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="NumberIsTooLargeException"> if {@code row != column} and increment is non-zero. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void addToEntry(final int row, final int column, final double increment) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void addToEntry(int row, int column, double increment)
		{
			if (row == column)
			{
				MatrixUtils.checkRowIndex(this, row);
				data[row] += increment;
			}
			else
			{
				ensureZero(increment);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void multiplyEntry(final int row, final int column, final double factor) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void multiplyEntry(int row, int column, double factor)
		{
			// we don't care about non-diagonal elements for multiplication
			if (row == column)
			{
				MatrixUtils.checkRowIndex(this, row);
				data[row] *= factor;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int RowDimension
		{
			get
			{
				return data.Length;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int ColumnDimension
		{
			get
			{
				return data.Length;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double[] operate(final double[] v) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double[] operate(double[] v)
		{
			return multiply(new DiagonalMatrix(v, false)).DataRef;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double[] preMultiply(final double[] v) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double[] preMultiply(double[] v)
		{
			return operate(v);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector preMultiply(final RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealVector preMultiply(RealVector v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vectorData;
			double[] vectorData;
			if (v is ArrayRealVector)
			{
				vectorData = ((ArrayRealVector) v).DataRef;
			}
			else
			{
				vectorData = v.toArray();
			}
			return MatrixUtils.createRealVector(preMultiply(vectorData));
		}

		/// <summary>
		/// Ensure a value is zero. </summary>
		/// <param name="value"> value to check </param>
		/// <exception cref="NumberIsTooLargeException"> if value is not zero </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void ensureZero(final double value) throws org.apache.commons.math3.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void ensureZero(double value)
		{
			if (!Precision.Equals(0.0, value, 1))
			{
				throw new NumberIsTooLargeException(FastMath.abs(value), 0, true);
			}
		}

		/// <summary>
		/// Computes the inverse of this diagonal matrix.
		/// <p>
		/// Note: this method will use a singularity threshold of 0,
		/// use <seealso cref="#inverse(double)"/> if a different threshold is needed.
		/// </summary>
		/// <returns> the inverse of {@code m} </returns>
		/// <exception cref="SingularMatrixException"> if the matrix is singular
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DiagonalMatrix inverse() throws SingularMatrixException
		public virtual DiagonalMatrix inverse()
		{
			return inverse(0);
		}

		/// <summary>
		/// Computes the inverse of this diagonal matrix.
		/// </summary>
		/// <param name="threshold"> Singularity threshold. </param>
		/// <returns> the inverse of {@code m} </returns>
		/// <exception cref="SingularMatrixException"> if the matrix is singular
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DiagonalMatrix inverse(double threshold) throws SingularMatrixException
		public virtual DiagonalMatrix inverse(double threshold)
		{
			if (isSingular(threshold))
			{
				throw new SingularMatrixException();
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] result = new double[data.length];
			double[] result = new double[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				result[i] = 1.0 / data[i];
			}
			return new DiagonalMatrix(result, false);
		}

		/// <summary>
		/// Returns whether this diagonal matrix is singular, i.e. any diagonal entry
		/// is equal to {@code 0} within the given threshold.
		/// </summary>
		/// <param name="threshold"> Singularity threshold. </param>
		/// <returns> {@code true} if the matrix is singular, {@code false} otherwise
		/// @since 3.3 </returns>
		public virtual bool isSingular(double threshold)
		{
			for (int i = 0; i < data.Length; i++)
			{
				if (Precision.Equals(data[i], 0.0, threshold))
				{
					return true;
				}
			}
			return false;
		}
	}

}
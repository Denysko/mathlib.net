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
	using FastMath = org.apache.commons.math3.util.FastMath;


	/// <summary>
	/// Calculates the Cholesky decomposition of a matrix.
	/// <p>The Cholesky decomposition of a real symmetric positive-definite
	/// matrix A consists of a lower triangular matrix L with same size such
	/// that: A = LL<sup>T</sup>. In a sense, this is the square root of A.</p>
	/// <p>This class is based on the class with similar name from the
	/// <a href="http://math.nist.gov/javanumerics/jama/">JAMA</a> library, with the
	/// following changes:</p>
	/// <ul>
	///   <li>a <seealso cref="#getLT() getLT"/> method has been added,</li>
	///   <li>the {@code isspd} method has been removed, since the constructor of
	///   this class throws a <seealso cref="NonPositiveDefiniteMatrixException"/> when a
	///   matrix cannot be decomposed,</li>
	///   <li>a <seealso cref="#getDeterminant() getDeterminant"/> method has been added,</li>
	///   <li>the {@code solve} method has been replaced by a {@link #getSolver()
	///   getSolver} method and the equivalent method provided by the returned
	///   <seealso cref="DecompositionSolver"/>.</li>
	/// </ul>
	/// </summary>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/CholeskyDecomposition.html">MathWorld</a> </seealso>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Cholesky_decomposition">Wikipedia</a>
	/// @version $Id: CholeskyDecomposition.java 1566017 2014-02-08 14:13:34Z tn $
	/// @since 2.0 (changed to concrete class in 3.0) </seealso>
	public class CholeskyDecomposition
	{
		/// <summary>
		/// Default threshold above which off-diagonal elements are considered too different
		/// and matrix not symmetric.
		/// </summary>
		public const double DEFAULT_RELATIVE_SYMMETRY_THRESHOLD = 1.0e-15;
		/// <summary>
		/// Default threshold below which diagonal elements are considered null
		/// and matrix not positive definite.
		/// </summary>
		public const double DEFAULT_ABSOLUTE_POSITIVITY_THRESHOLD = 1.0e-10;
		/// <summary>
		/// Row-oriented storage for L<sup>T</sup> matrix data. </summary>
		private double[][] lTData;
		/// <summary>
		/// Cached value of L. </summary>
		private RealMatrix cachedL;
		/// <summary>
		/// Cached value of LT. </summary>
		private RealMatrix cachedLT;

		/// <summary>
		/// Calculates the Cholesky decomposition of the given matrix.
		/// <p>
		/// Calling this constructor is equivalent to call {@link
		/// #CholeskyDecomposition(RealMatrix, double, double)} with the
		/// thresholds set to the default values {@link
		/// #DEFAULT_RELATIVE_SYMMETRY_THRESHOLD} and {@link
		/// #DEFAULT_ABSOLUTE_POSITIVITY_THRESHOLD}
		/// </p> </summary>
		/// <param name="matrix"> the matrix to decompose </param>
		/// <exception cref="NonSquareMatrixException"> if the matrix is not square. </exception>
		/// <exception cref="NonSymmetricMatrixException"> if the matrix is not symmetric. </exception>
		/// <exception cref="NonPositiveDefiniteMatrixException"> if the matrix is not
		/// strictly positive definite. </exception>
		/// <seealso cref= #CholeskyDecomposition(RealMatrix, double, double) </seealso>
		/// <seealso cref= #DEFAULT_RELATIVE_SYMMETRY_THRESHOLD </seealso>
		/// <seealso cref= #DEFAULT_ABSOLUTE_POSITIVITY_THRESHOLD </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public CholeskyDecomposition(final RealMatrix matrix)
		public CholeskyDecomposition(RealMatrix matrix) : this(matrix, DEFAULT_RELATIVE_SYMMETRY_THRESHOLD, DEFAULT_ABSOLUTE_POSITIVITY_THRESHOLD)
		{
		}

		/// <summary>
		/// Calculates the Cholesky decomposition of the given matrix. </summary>
		/// <param name="matrix"> the matrix to decompose </param>
		/// <param name="relativeSymmetryThreshold"> threshold above which off-diagonal
		/// elements are considered too different and matrix not symmetric </param>
		/// <param name="absolutePositivityThreshold"> threshold below which diagonal
		/// elements are considered null and matrix not positive definite </param>
		/// <exception cref="NonSquareMatrixException"> if the matrix is not square. </exception>
		/// <exception cref="NonSymmetricMatrixException"> if the matrix is not symmetric. </exception>
		/// <exception cref="NonPositiveDefiniteMatrixException"> if the matrix is not
		/// strictly positive definite. </exception>
		/// <seealso cref= #CholeskyDecomposition(RealMatrix) </seealso>
		/// <seealso cref= #DEFAULT_RELATIVE_SYMMETRY_THRESHOLD </seealso>
		/// <seealso cref= #DEFAULT_ABSOLUTE_POSITIVITY_THRESHOLD </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public CholeskyDecomposition(final RealMatrix matrix, final double relativeSymmetryThreshold, final double absolutePositivityThreshold)
		public CholeskyDecomposition(RealMatrix matrix, double relativeSymmetryThreshold, double absolutePositivityThreshold)
		{
			if (!matrix.Square)
			{
				throw new NonSquareMatrixException(matrix.RowDimension, matrix.ColumnDimension);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int order = matrix.getRowDimension();
			int order = matrix.RowDimension;
			lTData = matrix.Data;
			cachedL = null;
			cachedLT = null;

			// check the matrix before transformation
			for (int i = 0; i < order; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] lI = lTData[i];
				double[] lI = lTData[i];

				// check off-diagonal elements (and reset them to 0)
				for (int j = i + 1; j < order; ++j)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] lJ = lTData[j];
					double[] lJ = lTData[j];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lIJ = lI[j];
					double lIJ = lI[j];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lJI = lJ[i];
					double lJI = lJ[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double maxDelta = relativeSymmetryThreshold * org.apache.commons.math3.util.FastMath.max(org.apache.commons.math3.util.FastMath.abs(lIJ), org.apache.commons.math3.util.FastMath.abs(lJI));
					double maxDelta = relativeSymmetryThreshold * FastMath.max(FastMath.abs(lIJ), FastMath.abs(lJI));
					if (FastMath.abs(lIJ - lJI) > maxDelta)
					{
						throw new NonSymmetricMatrixException(i, j, relativeSymmetryThreshold);
					}
					lJ[i] = 0;
				}
			}

			// transform the matrix
			for (int i = 0; i < order; ++i)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] ltI = lTData[i];
				double[] ltI = lTData[i];

				// check diagonal element
				if (ltI[i] <= absolutePositivityThreshold)
				{
					throw new NonPositiveDefiniteMatrixException(ltI[i], i, absolutePositivityThreshold);
				}

				ltI[i] = FastMath.sqrt(ltI[i]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double inverse = 1.0 / ltI[i];
				double inverse = 1.0 / ltI[i];

				for (int q = order - 1; q > i; --q)
				{
					ltI[q] *= inverse;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] ltQ = lTData[q];
					double[] ltQ = lTData[q];
					for (int p = q; p < order; ++p)
					{
						ltQ[p] -= ltI[q] * ltI[p];
					}
				}
			}
		}

		/// <summary>
		/// Returns the matrix L of the decomposition.
		/// <p>L is an lower-triangular matrix</p> </summary>
		/// <returns> the L matrix </returns>
		public virtual RealMatrix L
		{
			get
			{
				if (cachedL == null)
				{
					cachedL = LT.transpose();
				}
				return cachedL;
			}
		}

		/// <summary>
		/// Returns the transpose of the matrix L of the decomposition.
		/// <p>L<sup>T</sup> is an upper-triangular matrix</p> </summary>
		/// <returns> the transpose of the matrix L of the decomposition </returns>
		public virtual RealMatrix LT
		{
			get
			{
    
				if (cachedLT == null)
				{
					cachedLT = MatrixUtils.createRealMatrix(lTData);
				}
    
				// return the cached matrix
				return cachedLT;
			}
		}

		/// <summary>
		/// Return the determinant of the matrix </summary>
		/// <returns> determinant of the matrix </returns>
		public virtual double Determinant
		{
			get
			{
				double determinant = 1.0;
				for (int i = 0; i < lTData.Length; ++i)
				{
					double lTii = lTData[i][i];
					determinant *= lTii * lTii;
				}
				return determinant;
			}
		}

		/// <summary>
		/// Get a solver for finding the A &times; X = B solution in least square sense. </summary>
		/// <returns> a solver </returns>
		public virtual DecompositionSolver Solver
		{
			get
			{
				return new Solver(lTData);
			}
		}

		/// <summary>
		/// Specialized solver. </summary>
		private class Solver : DecompositionSolver
		{
			/// <summary>
			/// Row-oriented storage for L<sup>T</sup> matrix data. </summary>
			internal readonly double[][] lTData;

			/// <summary>
			/// Build a solver from decomposed matrix. </summary>
			/// <param name="lTData"> row-oriented storage for L<sup>T</sup> matrix data </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Solver(final double[][] lTData)
			internal Solver(double[][] lTData)
			{
				this.lTData = lTData;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual bool NonSingular
			{
				get
				{
					// if we get this far, the matrix was positive definite, hence non-singular
					return true;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RealVector solve(final RealVector b)
			public virtual RealVector solve(RealVector b)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = lTData.length;
				int m = lTData.Length;
				if (b.Dimension != m)
				{
					throw new DimensionMismatchException(b.Dimension, m);
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] x = b.toArray();
				double[] x = b.toArray();

				// Solve LY = b
				for (int j = 0; j < m; j++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] lJ = lTData[j];
					double[] lJ = lTData[j];
					x[j] /= lJ[j];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xJ = x[j];
					double xJ = x[j];
					for (int i = j + 1; i < m; i++)
					{
						x[i] -= xJ * lJ[i];
					}
				}

				// Solve LTX = Y
				for (int j = m - 1; j >= 0; j--)
				{
					x[j] /= lTData[j][j];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xJ = x[j];
					double xJ = x[j];
					for (int i = 0; i < j; i++)
					{
						x[i] -= xJ * lTData[i][j];
					}
				}

				return new ArrayRealVector(x, false);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual RealMatrix solve(RealMatrix b)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = lTData.length;
				int m = lTData.Length;
				if (b.RowDimension != m)
				{
					throw new DimensionMismatchException(b.RowDimension, m);
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nColB = b.getColumnDimension();
				int nColB = b.ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] x = b.getData();
				double[][] x = b.Data;

				// Solve LY = b
				for (int j = 0; j < m; j++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] lJ = lTData[j];
					double[] lJ = lTData[j];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lJJ = lJ[j];
					double lJJ = lJ[j];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] xJ = x[j];
					double[] xJ = x[j];
					for (int k = 0; k < nColB; ++k)
					{
						xJ[k] /= lJJ;
					}
					for (int i = j + 1; i < m; i++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] xI = x[i];
						double[] xI = x[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lJI = lJ[i];
						double lJI = lJ[i];
						for (int k = 0; k < nColB; ++k)
						{
							xI[k] -= xJ[k] * lJI;
						}
					}
				}

				// Solve LTX = Y
				for (int j = m - 1; j >= 0; j--)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lJJ = lTData[j][j];
					double lJJ = lTData[j][j];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] xJ = x[j];
					double[] xJ = x[j];
					for (int k = 0; k < nColB; ++k)
					{
						xJ[k] /= lJJ;
					}
					for (int i = 0; i < j; i++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] xI = x[i];
						double[] xI = x[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lIJ = lTData[i][j];
						double lIJ = lTData[i][j];
						for (int k = 0; k < nColB; ++k)
						{
							xI[k] -= xJ[k] * lIJ;
						}
					}
				}

				return new Array2DRowRealMatrix(x);
			}

			/// <summary>
			/// Get the inverse of the decomposed matrix.
			/// </summary>
			/// <returns> the inverse matrix. </returns>
			public virtual RealMatrix Inverse
			{
				get
				{
					return solve(MatrixUtils.createRealIdentityMatrix(lTData.Length));
				}
			}
		}
	}

}
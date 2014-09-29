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

namespace mathlib.linear
{

	using Complex = mathlib.complex.Complex;
	using MathArithmeticException = mathlib.exception.MathArithmeticException;
	using MathUnsupportedOperationException = mathlib.exception.MathUnsupportedOperationException;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using Precision = mathlib.util.Precision;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Calculates the eigen decomposition of a real matrix.
	/// <p>The eigen decomposition of matrix A is a set of two matrices:
	/// V and D such that A = V &times; D &times; V<sup>T</sup>.
	/// A, V and D are all m &times; m matrices.</p>
	/// <p>This class is similar in spirit to the <code>EigenvalueDecomposition</code>
	/// class from the <a href="http://math.nist.gov/javanumerics/jama/">JAMA</a>
	/// library, with the following changes:</p>
	/// <ul>
	///   <li>a <seealso cref="#getVT() getVt"/> method has been added,</li>
	///   <li>two <seealso cref="#getRealEigenvalue(int) getRealEigenvalue"/> and {@link #getImagEigenvalue(int)
	///   getImagEigenvalue} methods to pick up a single eigenvalue have been added,</li>
	///   <li>a <seealso cref="#getEigenvector(int) getEigenvector"/> method to pick up a single
	///   eigenvector has been added,</li>
	///   <li>a <seealso cref="#getDeterminant() getDeterminant"/> method has been added.</li>
	///   <li>a <seealso cref="#getSolver() getSolver"/> method has been added.</li>
	/// </ul>
	/// <p>
	/// As of 3.1, this class supports general real matrices (both symmetric and non-symmetric):
	/// </p>
	/// <p>
	/// If A is symmetric, then A = V*D*V' where the eigenvalue matrix D is diagonal and the eigenvector
	/// matrix V is orthogonal, i.e. A = V.multiply(D.multiply(V.transpose())) and
	/// V.multiply(V.transpose()) equals the identity matrix.
	/// </p>
	/// <p>
	/// If A is not symmetric, then the eigenvalue matrix D is block diagonal with the real eigenvalues
	/// in 1-by-1 blocks and any complex eigenvalues, lambda + i*mu, in 2-by-2 blocks:
	/// <pre>
	///    [lambda, mu    ]
	///    [   -mu, lambda]
	/// </pre>
	/// The columns of V represent the eigenvectors in the sense that A*V = V*D,
	/// i.e. A.multiply(V) equals V.multiply(D).
	/// The matrix V may be badly conditioned, or even singular, so the validity of the equation
	/// A = V*D*inverse(V) depends upon the condition of V.
	/// </p>
	/// <p>
	/// This implementation is based on the paper by A. Drubrulle, R.S. Martin and
	/// J.H. Wilkinson "The Implicit QL Algorithm" in Wilksinson and Reinsch (1971)
	/// Handbook for automatic computation, vol. 2, Linear algebra, Springer-Verlag,
	/// New-York
	/// </p> </summary>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/EigenDecomposition.html">MathWorld</a> </seealso>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Eigendecomposition_of_a_matrix">Wikipedia</a>
	/// @version $Id: EigenDecomposition.java 1538368 2013-11-03 13:57:37Z erans $
	/// @since 2.0 (changed to concrete class in 3.0) </seealso>
	public class EigenDecomposition
	{
		/// <summary>
		/// Internally used epsilon criteria. </summary>
		private const double EPSILON = 1e-12;
		/// <summary>
		/// Maximum number of iterations accepted in the implicit QL transformation </summary>
		private sbyte maxIter = 30;
		/// <summary>
		/// Main diagonal of the tridiagonal matrix. </summary>
		private double[] main;
		/// <summary>
		/// Secondary diagonal of the tridiagonal matrix. </summary>
		private double[] secondary;
		/// <summary>
		/// Transformer to tridiagonal (may be null if matrix is already
		/// tridiagonal).
		/// </summary>
		private TriDiagonalTransformer transformer;
		/// <summary>
		/// Real part of the realEigenvalues. </summary>
		private double[] realEigenvalues;
		/// <summary>
		/// Imaginary part of the realEigenvalues. </summary>
		private double[] imagEigenvalues;
		/// <summary>
		/// Eigenvectors. </summary>
		private ArrayRealVector[] eigenvectors;
		/// <summary>
		/// Cached value of V. </summary>
		private RealMatrix cachedV;
		/// <summary>
		/// Cached value of D. </summary>
		private RealMatrix cachedD;
		/// <summary>
		/// Cached value of Vt. </summary>
		private RealMatrix cachedVt;
		/// <summary>
		/// Whether the matrix is symmetric. </summary>
		private readonly bool isSymmetric;

		/// <summary>
		/// Calculates the eigen decomposition of the given real matrix.
		/// <p>
		/// Supports decomposition of a general matrix since 3.1.
		/// </summary>
		/// <param name="matrix"> Matrix to decompose. </param>
		/// <exception cref="MaxCountExceededException"> if the algorithm fails to converge. </exception>
		/// <exception cref="MathArithmeticException"> if the decomposition of a general matrix
		/// results in a matrix with zero norm
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public EigenDecomposition(final RealMatrix matrix) throws mathlib.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public EigenDecomposition(RealMatrix matrix)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double symTol = 10 * matrix.getRowDimension() * matrix.getColumnDimension() * mathlib.util.Precision.EPSILON;
			double symTol = 10 * matrix.RowDimension * matrix.ColumnDimension * Precision.EPSILON;
			isSymmetric = MatrixUtils.isSymmetric(matrix, symTol);
			if (isSymmetric)
			{
				transformToTridiagonal(matrix);
				findEigenVectors(transformer.Q.Data);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SchurTransformer t = transformToSchur(matrix);
				SchurTransformer t = transformToSchur(matrix);
				findEigenVectorsFromSchur(t);
			}
		}

		/// <summary>
		/// Calculates the eigen decomposition of the given real matrix.
		/// </summary>
		/// <param name="matrix"> Matrix to decompose. </param>
		/// <param name="splitTolerance"> Dummy parameter (present for backward
		/// compatibility only). </param>
		/// <exception cref="MathArithmeticException">  if the decomposition of a general matrix
		/// results in a matrix with zero norm </exception>
		/// <exception cref="MaxCountExceededException"> if the algorithm fails to converge. </exception>
		/// @deprecated in 3.1 (to be removed in 4.0) due to unused parameter 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("in 3.1 (to be removed in 4.0) due to unused parameter") public EigenDecomposition(final RealMatrix matrix, final double splitTolerance) throws mathlib.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		[Obsolete("in 3.1 (to be removed in 4.0) due to unused parameter")]
		public EigenDecomposition(RealMatrix matrix, double splitTolerance) : this(matrix)
		{
		}

		/// <summary>
		/// Calculates the eigen decomposition of the symmetric tridiagonal
		/// matrix.  The Householder matrix is assumed to be the identity matrix.
		/// </summary>
		/// <param name="main"> Main diagonal of the symmetric tridiagonal form. </param>
		/// <param name="secondary"> Secondary of the tridiagonal form. </param>
		/// <exception cref="MaxCountExceededException"> if the algorithm fails to converge.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public EigenDecomposition(final double[] main, final double[] secondary)
		public EigenDecomposition(double[] main, double[] secondary)
		{
			isSymmetric = true;
			this.main = main.clone();
			this.secondary = secondary.clone();
			transformer = null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = main.length;
			int size = main.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] z = new double[size][size];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] z = new double[size][size];
			double[][] z = RectangularArrays.ReturnRectangularDoubleArray(size, size);
			for (int i = 0; i < size; i++)
			{
				z[i][i] = 1.0;
			}
			findEigenVectors(z);
		}

		/// <summary>
		/// Calculates the eigen decomposition of the symmetric tridiagonal
		/// matrix.  The Householder matrix is assumed to be the identity matrix.
		/// </summary>
		/// <param name="main"> Main diagonal of the symmetric tridiagonal form. </param>
		/// <param name="secondary"> Secondary of the tridiagonal form. </param>
		/// <param name="splitTolerance"> Dummy parameter (present for backward
		/// compatibility only). </param>
		/// <exception cref="MaxCountExceededException"> if the algorithm fails to converge. </exception>
		/// @deprecated in 3.1 (to be removed in 4.0) due to unused parameter 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("in 3.1 (to be removed in 4.0) due to unused parameter") public EigenDecomposition(final double[] main, final double[] secondary, final double splitTolerance)
		[Obsolete("in 3.1 (to be removed in 4.0) due to unused parameter")]
		public EigenDecomposition(double[] main, double[] secondary, double splitTolerance) : this(main, secondary)
		{
		}

		/// <summary>
		/// Gets the matrix V of the decomposition.
		/// V is an orthogonal matrix, i.e. its transpose is also its inverse.
		/// The columns of V are the eigenvectors of the original matrix.
		/// No assumption is made about the orientation of the system axes formed
		/// by the columns of V (e.g. in a 3-dimension space, V can form a left-
		/// or right-handed system).
		/// </summary>
		/// <returns> the V matrix. </returns>
		public virtual RealMatrix V
		{
			get
			{
    
				if (cachedV == null)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int m = eigenvectors.length;
					int m = eigenvectors.Length;
					cachedV = MatrixUtils.createRealMatrix(m, m);
					for (int k = 0; k < m; ++k)
					{
						cachedV.setColumnVector(k, eigenvectors[k]);
					}
				}
				// return the cached matrix
				return cachedV;
			}
		}

		/// <summary>
		/// Gets the block diagonal matrix D of the decomposition.
		/// D is a block diagonal matrix.
		/// Real eigenvalues are on the diagonal while complex values are on
		/// 2x2 blocks { {real +imaginary}, {-imaginary, real} }.
		/// </summary>
		/// <returns> the D matrix.
		/// </returns>
		/// <seealso cref= #getRealEigenvalues() </seealso>
		/// <seealso cref= #getImagEigenvalues() </seealso>
		public virtual RealMatrix D
		{
			get
			{
    
				if (cachedD == null)
				{
					// cache the matrix for subsequent calls
					cachedD = MatrixUtils.createRealDiagonalMatrix(realEigenvalues);
    
					for (int i = 0; i < imagEigenvalues.Length; i++)
					{
						if (Precision.compareTo(imagEigenvalues[i], 0.0, EPSILON) > 0)
						{
							cachedD.setEntry(i, i + 1, imagEigenvalues[i]);
						}
						else if (Precision.compareTo(imagEigenvalues[i], 0.0, EPSILON) < 0)
						{
							cachedD.setEntry(i, i - 1, imagEigenvalues[i]);
						}
					}
				}
				return cachedD;
			}
		}

		/// <summary>
		/// Gets the transpose of the matrix V of the decomposition.
		/// V is an orthogonal matrix, i.e. its transpose is also its inverse.
		/// The columns of V are the eigenvectors of the original matrix.
		/// No assumption is made about the orientation of the system axes formed
		/// by the columns of V (e.g. in a 3-dimension space, V can form a left-
		/// or right-handed system).
		/// </summary>
		/// <returns> the transpose of the V matrix. </returns>
		public virtual RealMatrix VT
		{
			get
			{
    
				if (cachedVt == null)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int m = eigenvectors.length;
					int m = eigenvectors.Length;
					cachedVt = MatrixUtils.createRealMatrix(m, m);
					for (int k = 0; k < m; ++k)
					{
						cachedVt.setRowVector(k, eigenvectors[k]);
					}
				}
    
				// return the cached matrix
				return cachedVt;
			}
		}

		/// <summary>
		/// Returns whether the calculated eigen values are complex or real.
		/// <p>The method performs a zero check for each element of the
		/// <seealso cref="#getImagEigenvalues()"/> array and returns {@code true} if any
		/// element is not equal to zero.
		/// </summary>
		/// <returns> {@code true} if the eigen values are complex, {@code false} otherwise
		/// @since 3.1 </returns>
		public virtual bool hasComplexEigenvalues()
		{
			for (int i = 0; i < imagEigenvalues.Length; i++)
			{
				if (!Precision.Equals(imagEigenvalues[i], 0.0, EPSILON))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Gets a copy of the real parts of the eigenvalues of the original matrix.
		/// </summary>
		/// <returns> a copy of the real parts of the eigenvalues of the original matrix.
		/// </returns>
		/// <seealso cref= #getD() </seealso>
		/// <seealso cref= #getRealEigenvalue(int) </seealso>
		/// <seealso cref= #getImagEigenvalues() </seealso>
		public virtual double[] RealEigenvalues
		{
			get
			{
				return realEigenvalues.clone();
			}
		}

		/// <summary>
		/// Returns the real part of the i<sup>th</sup> eigenvalue of the original
		/// matrix.
		/// </summary>
		/// <param name="i"> index of the eigenvalue (counting from 0) </param>
		/// <returns> real part of the i<sup>th</sup> eigenvalue of the original
		/// matrix.
		/// </returns>
		/// <seealso cref= #getD() </seealso>
		/// <seealso cref= #getRealEigenvalues() </seealso>
		/// <seealso cref= #getImagEigenvalue(int) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double getRealEigenvalue(final int i)
		public virtual double getRealEigenvalue(int i)
		{
			return realEigenvalues[i];
		}

		/// <summary>
		/// Gets a copy of the imaginary parts of the eigenvalues of the original
		/// matrix.
		/// </summary>
		/// <returns> a copy of the imaginary parts of the eigenvalues of the original
		/// matrix.
		/// </returns>
		/// <seealso cref= #getD() </seealso>
		/// <seealso cref= #getImagEigenvalue(int) </seealso>
		/// <seealso cref= #getRealEigenvalues() </seealso>
		public virtual double[] ImagEigenvalues
		{
			get
			{
				return imagEigenvalues.clone();
			}
		}

		/// <summary>
		/// Gets the imaginary part of the i<sup>th</sup> eigenvalue of the original
		/// matrix.
		/// </summary>
		/// <param name="i"> Index of the eigenvalue (counting from 0). </param>
		/// <returns> the imaginary part of the i<sup>th</sup> eigenvalue of the original
		/// matrix.
		/// </returns>
		/// <seealso cref= #getD() </seealso>
		/// <seealso cref= #getImagEigenvalues() </seealso>
		/// <seealso cref= #getRealEigenvalue(int) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double getImagEigenvalue(final int i)
		public virtual double getImagEigenvalue(int i)
		{
			return imagEigenvalues[i];
		}

		/// <summary>
		/// Gets a copy of the i<sup>th</sup> eigenvector of the original matrix.
		/// </summary>
		/// <param name="i"> Index of the eigenvector (counting from 0). </param>
		/// <returns> a copy of the i<sup>th</sup> eigenvector of the original matrix. </returns>
		/// <seealso cref= #getD() </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RealVector getEigenvector(final int i)
		public virtual RealVector getEigenvector(int i)
		{
			return eigenvectors[i].copy();
		}

		/// <summary>
		/// Computes the determinant of the matrix.
		/// </summary>
		/// <returns> the determinant of the matrix. </returns>
		public virtual double Determinant
		{
			get
			{
				double determinant = 1;
				foreach (double lambda in realEigenvalues)
				{
					determinant *= lambda;
				}
				return determinant;
			}
		}

		/// <summary>
		/// Computes the square-root of the matrix.
		/// This implementation assumes that the matrix is symmetric and positive
		/// definite.
		/// </summary>
		/// <returns> the square-root of the matrix. </returns>
		/// <exception cref="MathUnsupportedOperationException"> if the matrix is not
		/// symmetric or not positive definite.
		/// @since 3.1 </exception>
		public virtual RealMatrix SquareRoot
		{
			get
			{
				if (!isSymmetric)
				{
					throw new MathUnsupportedOperationException();
				}
    
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[] sqrtEigenValues = new double[realEigenvalues.length];
				double[] sqrtEigenValues = new double[realEigenvalues.Length];
				for (int i = 0; i < realEigenvalues.Length; i++)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double eigen = realEigenvalues[i];
					double eigen = realEigenvalues[i];
					if (eigen <= 0)
					{
						throw new MathUnsupportedOperationException();
					}
					sqrtEigenValues[i] = FastMath.sqrt(eigen);
				}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final RealMatrix sqrtEigen = MatrixUtils.createRealDiagonalMatrix(sqrtEigenValues);
				RealMatrix sqrtEigen = MatrixUtils.createRealDiagonalMatrix(sqrtEigenValues);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final RealMatrix v = getV();
				RealMatrix v = V;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final RealMatrix vT = getVT();
				RealMatrix vT = VT;
    
				return v.multiply(sqrtEigen).multiply(vT);
			}
		}

		/// <summary>
		/// Gets a solver for finding the A &times; X = B solution in exact
		/// linear sense.
		/// <p>
		/// Since 3.1, eigen decomposition of a general matrix is supported,
		/// but the <seealso cref="DecompositionSolver"/> only supports real eigenvalues.
		/// </summary>
		/// <returns> a solver </returns>
		/// <exception cref="MathUnsupportedOperationException"> if the decomposition resulted in
		/// complex eigenvalues </exception>
		public virtual DecompositionSolver Solver
		{
			get
			{
				if (hasComplexEigenvalues())
				{
					throw new MathUnsupportedOperationException();
				}
				return new Solver(realEigenvalues, imagEigenvalues, eigenvectors);
			}
		}

		/// <summary>
		/// Specialized solver. </summary>
		private class Solver : DecompositionSolver
		{
			/// <summary>
			/// Real part of the realEigenvalues. </summary>
			internal double[] realEigenvalues;
			/// <summary>
			/// Imaginary part of the realEigenvalues. </summary>
			internal double[] imagEigenvalues;
			/// <summary>
			/// Eigenvectors. </summary>
			internal readonly ArrayRealVector[] eigenvectors;

			/// <summary>
			/// Builds a solver from decomposed matrix.
			/// </summary>
			/// <param name="realEigenvalues"> Real parts of the eigenvalues. </param>
			/// <param name="imagEigenvalues"> Imaginary parts of the eigenvalues. </param>
			/// <param name="eigenvectors"> Eigenvectors. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Solver(final double[] realEigenvalues, final double[] imagEigenvalues, final ArrayRealVector[] eigenvectors)
			internal Solver(double[] realEigenvalues, double[] imagEigenvalues, ArrayRealVector[] eigenvectors)
			{
				this.realEigenvalues = realEigenvalues;
				this.imagEigenvalues = imagEigenvalues;
				this.eigenvectors = eigenvectors;
			}

			/// <summary>
			/// Solves the linear equation A &times; X = B for symmetric matrices A.
			/// <p>
			/// This method only finds exact linear solutions, i.e. solutions for
			/// which ||A &times; X - B|| is exactly 0.
			/// </p>
			/// </summary>
			/// <param name="b"> Right-hand side of the equation A &times; X = B. </param>
			/// <returns> a Vector X that minimizes the two norm of A &times; X - B.
			/// </returns>
			/// <exception cref="DimensionMismatchException"> if the matrices dimensions do not match. </exception>
			/// <exception cref="SingularMatrixException"> if the decomposed matrix is singular. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RealVector solve(final RealVector b)
			public virtual RealVector solve(RealVector b)
			{
				if (!NonSingular)
				{
					throw new SingularMatrixException();
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = realEigenvalues.length;
				int m = realEigenvalues.Length;
				if (b.Dimension != m)
				{
					throw new DimensionMismatchException(b.Dimension, m);
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] bp = new double[m];
				double[] bp = new double[m];
				for (int i = 0; i < m; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ArrayRealVector v = eigenvectors[i];
					ArrayRealVector v = eigenvectors[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vData = v.getDataRef();
					double[] vData = v.DataRef;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s = v.dotProduct(b) / realEigenvalues[i];
					double s = v.dotProduct(b) / realEigenvalues[i];
					for (int j = 0; j < m; ++j)
					{
						bp[j] += s * vData[j];
					}
				}

				return new ArrayRealVector(bp, false);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual RealMatrix solve(RealMatrix b)
			{

				if (!NonSingular)
				{
					throw new SingularMatrixException();
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = realEigenvalues.length;
				int m = realEigenvalues.Length;
				if (b.RowDimension != m)
				{
					throw new DimensionMismatchException(b.RowDimension, m);
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nColB = b.getColumnDimension();
				int nColB = b.ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] bp = new double[m][nColB];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] bp = new double[m][nColB];
				double[][] bp = RectangularArrays.ReturnRectangularDoubleArray(m, nColB);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tmpCol = new double[m];
				double[] tmpCol = new double[m];
				for (int k = 0; k < nColB; ++k)
				{
					for (int i = 0; i < m; ++i)
					{
						tmpCol[i] = b.getEntry(i, k);
						bp[i][k] = 0;
					}
					for (int i = 0; i < m; ++i)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ArrayRealVector v = eigenvectors[i];
						ArrayRealVector v = eigenvectors[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vData = v.getDataRef();
						double[] vData = v.DataRef;
						double s = 0;
						for (int j = 0; j < m; ++j)
						{
							s += v.getEntry(j) * tmpCol[j];
						}
						s /= realEigenvalues[i];
						for (int j = 0; j < m; ++j)
						{
							bp[j][k] += s * vData[j];
						}
					}
				}

				return new Array2DRowRealMatrix(bp, false);

			}

			/// <summary>
			/// Checks whether the decomposed matrix is non-singular.
			/// </summary>
			/// <returns> true if the decomposed matrix is non-singular. </returns>
			public virtual bool NonSingular
			{
				get
				{
					double largestEigenvalueNorm = 0.0;
					// Looping over all values (in case they are not sorted in decreasing
					// order of their norm).
					for (int i = 0; i < realEigenvalues.Length; ++i)
					{
						largestEigenvalueNorm = FastMath.max(largestEigenvalueNorm, eigenvalueNorm(i));
					}
					// Corner case: zero matrix, all exactly 0 eigenvalues
					if (largestEigenvalueNorm == 0.0)
					{
						return false;
					}
					for (int i = 0; i < realEigenvalues.Length; ++i)
					{
						// Looking for eigenvalues that are 0, where we consider anything much much smaller
						// than the largest eigenvalue to be effectively 0.
						if (Precision.Equals(eigenvalueNorm(i) / largestEigenvalueNorm, 0, EPSILON))
						{
							return false;
						}
					}
					return true;
				}
			}

			/// <param name="i"> which eigenvalue to find the norm of </param>
			/// <returns> the norm of ith (complex) eigenvalue. </returns>
			internal virtual double eigenvalueNorm(int i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double re = realEigenvalues[i];
				double re = realEigenvalues[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double im = imagEigenvalues[i];
				double im = imagEigenvalues[i];
				return FastMath.sqrt(re * re + im * im);
			}

			/// <summary>
			/// Get the inverse of the decomposed matrix.
			/// </summary>
			/// <returns> the inverse matrix. </returns>
			/// <exception cref="SingularMatrixException"> if the decomposed matrix is singular. </exception>
			public virtual RealMatrix Inverse
			{
				get
				{
					if (!NonSingular)
					{
						throw new SingularMatrixException();
					}
    
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int m = realEigenvalues.length;
					int m = realEigenvalues.Length;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[][] invData = new double[m][m];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] invData = new double[m][m];
					double[][] invData = RectangularArrays.ReturnRectangularDoubleArray(m, m);
    
					for (int i = 0; i < m; ++i)
					{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[] invI = invData[i];
						double[] invI = invData[i];
						for (int j = 0; j < m; ++j)
						{
							double invIJ = 0;
							for (int k = 0; k < m; ++k)
							{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[] vK = eigenvectors[k].getDataRef();
								double[] vK = eigenvectors[k].DataRef;
								invIJ += vK[i] * vK[j] / realEigenvalues[k];
							}
							invI[j] = invIJ;
						}
					}
					return MatrixUtils.createRealMatrix(invData);
				}
			}
		}

		/// <summary>
		/// Transforms the matrix to tridiagonal form.
		/// </summary>
		/// <param name="matrix"> Matrix to transform. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void transformToTridiagonal(final RealMatrix matrix)
		private void transformToTridiagonal(RealMatrix matrix)
		{
			// transform the matrix to tridiagonal
			transformer = new TriDiagonalTransformer(matrix);
			main = transformer.MainDiagonalRef;
			secondary = transformer.SecondaryDiagonalRef;
		}

		/// <summary>
		/// Find eigenvalues and eigenvectors (Dubrulle et al., 1971)
		/// </summary>
		/// <param name="householderMatrix"> Householder matrix of the transformation
		/// to tridiagonal form. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void findEigenVectors(final double[][] householderMatrix)
		private void findEigenVectors(double[][] householderMatrix)
		{
			final double[][]z = householderMatrix.clone();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = main.length;
			int n = main.Length;
			realEigenvalues = new double[n];
			imagEigenvalues = new double[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] e = new double[n];
			double[] e = new double[n];
			for (int i = 0; i < n - 1; i++)
			{
				realEigenvalues[i] = main[i];
				e[i] = secondary[i];
			}
			realEigenvalues[n - 1] = main[n - 1];
			e[n - 1] = 0;

			// Determine the largest main and secondary value in absolute term.
			double maxAbsoluteValue = 0;
			for (int i = 0; i < n; i++)
			{
				if (FastMath.abs(realEigenvalues[i]) > maxAbsoluteValue)
				{
					maxAbsoluteValue = FastMath.abs(realEigenvalues[i]);
				}
				if (FastMath.abs(e[i]) > maxAbsoluteValue)
				{
					maxAbsoluteValue = FastMath.abs(e[i]);
				}
			}
			// Make null any main and secondary value too small to be significant
			if (maxAbsoluteValue != 0)
			{
				for (int i = 0; i < n; i++)
				{
					if (FastMath.abs(realEigenvalues[i]) <= Precision.EPSILON * maxAbsoluteValue)
					{
						realEigenvalues[i] = 0;
					}
					if (FastMath.abs(e[i]) <= Precision.EPSILON * maxAbsoluteValue)
					{
						e[i] = 0;
					}
				}
			}

			for (int j = 0; j < n; j++)
			{
				int its = 0;
				int m;
				do
				{
					for (m = j; m < n - 1; m++)
					{
						double delta = FastMath.abs(realEigenvalues[m]) + FastMath.abs(realEigenvalues[m + 1]);
						if (FastMath.abs(e[m]) + delta == delta)
						{
							break;
						}
					}
					if (m != j)
					{
						if (its == maxIter)
						{
							throw new MaxCountExceededException(LocalizedFormats.CONVERGENCE_FAILED, maxIter);
						}
						its++;
						double q = (realEigenvalues[j + 1] - realEigenvalues[j]) / (2 * e[j]);
						double t = FastMath.sqrt(1 + q * q);
						if (q < 0.0)
						{
							q = realEigenvalues[m] - realEigenvalues[j] + e[j] / (q - t);
						}
						else
						{
							q = realEigenvalues[m] - realEigenvalues[j] + e[j] / (q + t);
						}
						double u = 0.0;
						double s = 1.0;
						double c = 1.0;
						int i;
						for (i = m - 1; i >= j; i--)
						{
							double p = s * e[i];
							double h = c * e[i];
							if (FastMath.abs(p) >= FastMath.abs(q))
							{
								c = q / p;
								t = FastMath.sqrt(c * c + 1.0);
								e[i + 1] = p * t;
								s = 1.0 / t;
								c *= s;
							}
							else
							{
								s = p / q;
								t = FastMath.sqrt(s * s + 1.0);
								e[i + 1] = q * t;
								c = 1.0 / t;
								s *= c;
							}
							if (e[i + 1] == 0.0)
							{
								realEigenvalues[i + 1] -= u;
								e[m] = 0.0;
								break;
							}
							q = realEigenvalues[i + 1] - u;
							t = (realEigenvalues[i] - q) * s + 2.0 * c * h;
							u = s * t;
							realEigenvalues[i + 1] = q + u;
							q = c * t - h;
							for (int ia = 0; ia < n; ia++)
							{
								p = z[ia][i + 1];
								z[ia][i + 1] = s * z[ia][i] + c * p;
								z[ia][i] = c * z[ia][i] - s * p;
							}
						}
						if (t == 0.0 && i >= j)
						{
							continue;
						}
						realEigenvalues[j] -= u;
						e[j] = q;
						e[m] = 0.0;
					}
				} while (m != j);
			}

			//Sort the eigen values (and vectors) in increase order
			for (int i = 0; i < n; i++)
			{
				int k = i;
				double p = realEigenvalues[i];
				for (int j = i + 1; j < n; j++)
				{
					if (realEigenvalues[j] > p)
					{
						k = j;
						p = realEigenvalues[j];
					}
				}
				if (k != i)
				{
					realEigenvalues[k] = realEigenvalues[i];
					realEigenvalues[i] = p;
					for (int j = 0; j < n; j++)
					{
						p = z[j][i];
						z[j][i] = z[j][k];
						z[j][k] = p;
					}
				}
			}

			// Determine the largest eigen value in absolute term.
			maxAbsoluteValue = 0;
			for (int i = 0; i < n; i++)
			{
				if (FastMath.abs(realEigenvalues[i]) > maxAbsoluteValue)
				{
					maxAbsoluteValue = FastMath.abs(realEigenvalues[i]);
				}
			}
			// Make null any eigen value too small to be significant
			if (maxAbsoluteValue != 0.0)
			{
				for (int i = 0; i < n; i++)
				{
					if (FastMath.abs(realEigenvalues[i]) < Precision.EPSILON * maxAbsoluteValue)
					{
						realEigenvalues[i] = 0;
					}
				}
			}
			eigenvectors = new ArrayRealVector[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tmp = new double[n];
			double[] tmp = new double[n];
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					tmp[j] = z[j][i];
				}
				eigenvectors[i] = new ArrayRealVector(tmp);
			}
		}

		/// <summary>
		/// Transforms the matrix to Schur form and calculates the eigenvalues.
		/// </summary>
		/// <param name="matrix"> Matrix to transform. </param>
		/// <returns> the <seealso cref="SchurTransformer Shur transform"/> for this matrix </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private SchurTransformer transformToSchur(final RealMatrix matrix)
		private SchurTransformer transformToSchur(RealMatrix matrix)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SchurTransformer schurTransform = new SchurTransformer(matrix);
			SchurTransformer schurTransform = new SchurTransformer(matrix);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] matT = schurTransform.getT().getData();
			double[][] matT = schurTransform.T.Data;

			realEigenvalues = new double[matT.Length];
			imagEigenvalues = new double[matT.Length];

			for (int i = 0; i < realEigenvalues.Length; i++)
			{
				if (i == (realEigenvalues.Length - 1) || Precision.Equals(matT[i + 1][i], 0.0, EPSILON))
				{
					realEigenvalues[i] = matT[i][i];
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = matT[i + 1][i + 1];
					double x = matT[i + 1][i + 1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double p = 0.5 * (matT[i][i] - x);
					double p = 0.5 * (matT[i][i] - x);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z = mathlib.util.FastMath.sqrt(mathlib.util.FastMath.abs(p * p + matT[i + 1][i] * matT[i][i + 1]));
					double z = FastMath.sqrt(FastMath.abs(p * p + matT[i + 1][i] * matT[i][i + 1]));
					realEigenvalues[i] = x + p;
					imagEigenvalues[i] = z;
					realEigenvalues[i + 1] = x + p;
					imagEigenvalues[i + 1] = -z;
					i++;
				}
			}
			return schurTransform;
		}

		/// <summary>
		/// Performs a division of two complex numbers.
		/// </summary>
		/// <param name="xr"> real part of the first number </param>
		/// <param name="xi"> imaginary part of the first number </param>
		/// <param name="yr"> real part of the second number </param>
		/// <param name="yi"> imaginary part of the second number </param>
		/// <returns> result of the complex division </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private mathlib.complex.Complex cdiv(final double xr, final double xi, final double yr, final double yi)
		private Complex cdiv(double xr, double xi, double yr, double yi)
		{
			return (new Complex(xr, xi)).divide(new Complex(yr, yi));
		}

		/// <summary>
		/// Find eigenvectors from a matrix transformed to Schur form.
		/// </summary>
		/// <param name="schur"> the schur transformation of the matrix </param>
		/// <exception cref="MathArithmeticException"> if the Schur form has a norm of zero </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void findEigenVectorsFromSchur(final SchurTransformer schur) throws mathlib.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void findEigenVectorsFromSchur(SchurTransformer schur)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] matrixT = schur.getT().getData();
			double[][] matrixT = schur.T.Data;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] matrixP = schur.getP().getData();
			double[][] matrixP = schur.P.Data;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = matrixT.length;
			int n = matrixT.Length;

			// compute matrix norm
			double norm = 0.0;
			for (int i = 0; i < n; i++)
			{
			   for (int j = FastMath.max(i - 1, 0); j < n; j++)
			   {
				   norm += FastMath.abs(matrixT[i][j]);
			   }
			}

			// we can not handle a matrix with zero norm
			if (Precision.Equals(norm, 0.0, EPSILON))
			{
			   throw new MathArithmeticException(LocalizedFormats.ZERO_NORM);
			}

			// Backsubstitute to find vectors of upper triangular form

			double r = 0.0;
			double s = 0.0;
			double z = 0.0;

			for (int idx = n - 1; idx >= 0; idx--)
			{
				double p = realEigenvalues[idx];
				double q = imagEigenvalues[idx];

				if (Precision.Equals(q, 0.0))
				{
					// Real vector
					int l = idx;
					matrixT[idx][idx] = 1.0;
					for (int i = idx - 1; i >= 0; i--)
					{
						double w = matrixT[i][i] - p;
						r = 0.0;
						for (int j = l; j <= idx; j++)
						{
							r += matrixT[i][j] * matrixT[j][idx];
						}
						if (Precision.compareTo(imagEigenvalues[i], 0.0, EPSILON) < 0)
						{
							z = w;
							s = r;
						}
						else
						{
							l = i;
							if (Precision.Equals(imagEigenvalues[i], 0.0))
							{
								if (w != 0.0)
								{
									matrixT[i][idx] = -r / w;
								}
								else
								{
									matrixT[i][idx] = -r / (Precision.EPSILON * norm);
								}
							}
							else
							{
								// Solve real equations
								double x = matrixT[i][i + 1];
								double y = matrixT[i + 1][i];
								q = (realEigenvalues[i] - p) * (realEigenvalues[i] - p) + imagEigenvalues[i] * imagEigenvalues[i];
								double t = (x * s - z * r) / q;
								matrixT[i][idx] = t;
								if (FastMath.abs(x) > FastMath.abs(z))
								{
									matrixT[i + 1][idx] = (-r - w * t) / x;
								}
								else
								{
									matrixT[i + 1][idx] = (-s - y * t) / z;
								}
							}

							// Overflow control
							double t = FastMath.abs(matrixT[i][idx]);
							if ((Precision.EPSILON * t) * t > 1)
							{
								for (int j = i; j <= idx; j++)
								{
									matrixT[j][idx] /= t;
								}
							}
						}
					}
				}
				else if (q < 0.0)
				{
					// Complex vector
					int l = idx - 1;

					// Last vector component imaginary so matrix is triangular
					if (FastMath.abs(matrixT[idx][idx - 1]) > FastMath.abs(matrixT[idx - 1][idx]))
					{
						matrixT[idx - 1][idx - 1] = q / matrixT[idx][idx - 1];
						matrixT[idx - 1][idx] = -(matrixT[idx][idx] - p) / matrixT[idx][idx - 1];
					}
					else
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.complex.Complex result = cdiv(0.0, -matrixT[idx - 1][idx], matrixT[idx - 1][idx - 1] - p, q);
						Complex result = cdiv(0.0, -matrixT[idx - 1][idx], matrixT[idx - 1][idx - 1] - p, q);
						matrixT[idx - 1][idx - 1] = result.Real;
						matrixT[idx - 1][idx] = result.Imaginary;
					}

					matrixT[idx][idx - 1] = 0.0;
					matrixT[idx][idx] = 1.0;

					for (int i = idx - 2; i >= 0; i--)
					{
						double ra = 0.0;
						double sa = 0.0;
						for (int j = l; j <= idx; j++)
						{
							ra += matrixT[i][j] * matrixT[j][idx - 1];
							sa += matrixT[i][j] * matrixT[j][idx];
						}
						double w = matrixT[i][i] - p;

						if (Precision.compareTo(imagEigenvalues[i], 0.0, EPSILON) < 0)
						{
							z = w;
							r = ra;
							s = sa;
						}
						else
						{
							l = i;
							if (Precision.Equals(imagEigenvalues[i], 0.0))
							{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.complex.Complex c = cdiv(-ra, -sa, w, q);
								Complex c = cdiv(-ra, -sa, w, q);
								matrixT[i][idx - 1] = c.Real;
								matrixT[i][idx] = c.Imaginary;
							}
							else
							{
								// Solve complex equations
								double x = matrixT[i][i + 1];
								double y = matrixT[i + 1][i];
								double vr = (realEigenvalues[i] - p) * (realEigenvalues[i] - p) + imagEigenvalues[i] * imagEigenvalues[i] - q * q;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double vi = (realEigenvalues[i] - p) * 2.0 * q;
								double vi = (realEigenvalues[i] - p) * 2.0 * q;
								if (Precision.Equals(vr, 0.0) && Precision.Equals(vi, 0.0))
								{
									vr = Precision.EPSILON * norm * (FastMath.abs(w) + FastMath.abs(q) + FastMath.abs(x) + FastMath.abs(y) + FastMath.abs(z));
								}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.complex.Complex c = cdiv(x * r - z * ra + q * sa, x * s - z * sa - q * ra, vr, vi);
								Complex c = cdiv(x * r - z * ra + q * sa, x * s - z * sa - q * ra, vr, vi);
								matrixT[i][idx - 1] = c.Real;
								matrixT[i][idx] = c.Imaginary;

								if (FastMath.abs(x) > (FastMath.abs(z) + FastMath.abs(q)))
								{
									matrixT[i + 1][idx - 1] = (-ra - w * matrixT[i][idx - 1] + q * matrixT[i][idx]) / x;
									matrixT[i + 1][idx] = (-sa - w * matrixT[i][idx] - q * matrixT[i][idx - 1]) / x;
								}
								else
								{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.complex.Complex c2 = cdiv(-r - y * matrixT[i][idx - 1], -s - y * matrixT[i][idx], z, q);
									Complex c2 = cdiv(-r - y * matrixT[i][idx - 1], -s - y * matrixT[i][idx], z, q);
									matrixT[i + 1][idx - 1] = c2.Real;
									matrixT[i + 1][idx] = c2.Imaginary;
								}
							}

							// Overflow control
							double t = FastMath.max(FastMath.abs(matrixT[i][idx - 1]), FastMath.abs(matrixT[i][idx]));
							if ((Precision.EPSILON * t) * t > 1)
							{
								for (int j = i; j <= idx; j++)
								{
									matrixT[j][idx - 1] /= t;
									matrixT[j][idx] /= t;
								}
							}
						}
					}
				}
			}

			// Back transformation to get eigenvectors of original matrix
			for (int j = n - 1; j >= 0; j--)
			{
				for (int i = 0; i <= n - 1; i++)
				{
					z = 0.0;
					for (int k = 0; k <= FastMath.min(j, n - 1); k++)
					{
						z += matrixP[i][k] * matrixT[k][j];
					}
					matrixP[i][j] = z;
				}
			}

			eigenvectors = new ArrayRealVector[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tmp = new double[n];
			double[] tmp = new double[n];
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					tmp[j] = matrixP[j][i];
				}
				eigenvectors[i] = new ArrayRealVector(tmp);
			}
		}
	}

}
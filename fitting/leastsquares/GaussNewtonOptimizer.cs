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
namespace org.apache.commons.math3.fitting.leastsquares
{

	using ConvergenceException = org.apache.commons.math3.exception.ConvergenceException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using ArrayRealVector = org.apache.commons.math3.linear.ArrayRealVector;
	using CholeskyDecomposition = org.apache.commons.math3.linear.CholeskyDecomposition;
	using LUDecomposition = org.apache.commons.math3.linear.LUDecomposition;
	using MatrixUtils = org.apache.commons.math3.linear.MatrixUtils;
	using NonPositiveDefiniteMatrixException = org.apache.commons.math3.linear.NonPositiveDefiniteMatrixException;
	using QRDecomposition = org.apache.commons.math3.linear.QRDecomposition;
	using RealMatrix = org.apache.commons.math3.linear.RealMatrix;
	using RealVector = org.apache.commons.math3.linear.RealVector;
	using SingularMatrixException = org.apache.commons.math3.linear.SingularMatrixException;
	using SingularValueDecomposition = org.apache.commons.math3.linear.SingularValueDecomposition;
	using org.apache.commons.math3.optim;
	using Incrementor = org.apache.commons.math3.util.Incrementor;
	using org.apache.commons.math3.util;

	/// <summary>
	/// Gauss-Newton least-squares solver.
	/// <p> This class solve a least-square problem by
	/// solving the normal equations of the linearized problem at each iteration. Either LU
	/// decomposition or Cholesky decomposition can be used to solve the normal equations,
	/// or QR decomposition or SVD decomposition can be used to solve the linear system. LU
	/// decomposition is faster but QR decomposition is more robust for difficult problems,
	/// and SVD can compute a solution for rank-deficient problems.
	/// </p>
	/// 
	/// @version $Id: GaussNewtonOptimizer.java 1573351 2014-03-02 19:54:43Z luc $
	/// @since 3.3
	/// </summary>
	public class GaussNewtonOptimizer : LeastSquaresOptimizer
	{

		/// <summary>
		/// The decomposition algorithm to use to solve the normal equations. </summary>
		//TODO move to linear package and expand options?
		public enum Decomposition
		{
			/// <summary>
			/// Solve by forming the normal equations (J<sup>T</sup>Jx=J<sup>T</sup>r) and
			/// using the <seealso cref="LUDecomposition"/>.
			/// 
			/// <p> Theoretically this method takes mn<sup>2</sup>/2 operations to compute the
			/// normal matrix and n<sup>3</sup>/3 operations (m > n) to solve the system using
			/// the LU decomposition. </p>
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			LU
			{
				protected org.apache.commons.math3.linear.RealVector solve(final org.apache.commons.math3.linear.RealMatrix jacobian,
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
										   final org.apache.commons.math3.linear.RealVector residuals)
										   {
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					try
					{
//JAVA TO C# CONVERTER TODO TASK: Enum values must be single integer values in .NET:
						final org.apache.commons.math3.util.Pair<org.apache.commons.math3.linear.RealMatrix, org.apache.commons.math3.linear.RealVector> normalEquation = computeNormalMatrix(jacobian, residuals);
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain fields in .NET:
//						final org.apache.commons.math3.linear.RealMatrix normal = normalEquation.getFirst();
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain fields in .NET:
//						final org.apache.commons.math3.linear.RealVector jTr = normalEquation.getSecond();
						return = jTr
					}
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
					catch (org.apache.commons.math3.linear.SingularMatrixException e)
					{
//JAVA TO C# CONVERTER TODO TASK: Enum values must be single integer values in .NET:
						throw new org.apache.commons.math3.exception.ConvergenceException(org.apache.commons.math3.exception.util.LocalizedFormats.UNABLE_TO_SOLVE_SINGULAR_PROBLEM, e);
					}
										   }
			},
			/// <summary>
			/// Solve the linear least squares problem (Jx=r) using the {@link
			/// QRDecomposition}.
			/// 
			/// <p> Theoretically this method takes mn<sup>2</sup> - n<sup>3</sup>/3 operations
			/// (m > n) and has better numerical accuracy than any method that forms the normal
			/// equations. </p>
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			QR
			{
			},
			/// <summary>
			/// Solve by forming the normal equations (J<sup>T</sup>Jx=J<sup>T</sup>r) and
			/// using the <seealso cref="CholeskyDecomposition"/>.
			/// 
			/// <p> Theoretically this method takes mn<sup>2</sup>/2 operations to compute the
			/// normal matrix and n<sup>3</sup>/6 operations (m > n) to solve the system using
			/// the Cholesky decomposition. </p>
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			CHOLESKY
			{
			},
			/// <summary>
			/// Solve the linear least squares problem using the {@link
			/// SingularValueDecomposition}.
			/// 
			/// <p> This method is slower, but can provide a solution for rank deficient and
			/// nearly singular systems.
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
			SVD
			{
			}

			/// <summary>
			/// Solve the linear least squares problem Jx=r.
			/// </summary>
			/// <param name="jacobian">  the Jacobian matrix, J. the number of rows >= the number or
			///                  columns. </param>
			/// <param name="residuals"> the computed residuals, r. </param>
			/// <returns> the solution x, to the linear least squares problem Jx=r. </returns>
			/// <exception cref="ConvergenceException"> if the matrix properties (e.g. singular) do not
			///                              permit a solution. </exception>
//JAVA TO C# CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			protected abstract org.apache.commons.math3.linear.RealVector solve(org.apache.commons.math3.linear.RealMatrix jacobian, org.apache.commons.math3.linear.RealVector residuals);
		}

		/// <summary>
		/// The singularity threshold for matrix decompositions. Determines when a {@link
		/// ConvergenceException} is thrown. The current value was the default value for {@link
		/// LUDecomposition}.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain fields in .NET:
//		private static final double SINGULARITY_THRESHOLD = 1e-11;

		/// <summary>
		/// Indicator for using LU decomposition. </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain fields in .NET:
//		private final Decomposition decomposition;

		/// <summary>
		/// Creates a Gauss Newton optimizer.
		/// <p/>
		/// The default for the algorithm is to solve the normal equations using QR
		/// decomposition.
		/// </summary>

		/// <summary>
		/// Create a Gauss Newton optimizer that uses the given decomposition algorithm to
		/// solve the normal equations.
		/// </summary>
		/// <param name="decomposition"> the <seealso cref="Decomposition"/> algorithm. </param>

		/// <summary>
		/// Get the matrix decomposition algorithm used to solve the normal equations.
		/// </summary>
		/// <returns> the matrix <seealso cref="Decomposition"/> algoritm. </returns>

		/// <summary>
		/// Configure the decomposition algorithm.
		/// </summary>
		/// <param name="newDecomposition"> the <seealso cref="Decomposition"/> algorithm to use. </param>
		/// <returns> a new instance. </returns>

		/// <summary>
		/// {@inheritDoc} </summary>


		/// <summary>
		/// Compute the normal matrix, J<sup>T</sup>J.
		/// </summary>
		/// <param name="jacobian">  the m by n jacobian matrix, J. Input. </param>
		/// <param name="residuals"> the m by 1 residual vector, r. Input. </param>
		/// <returns>  the n by n normal matrix and  the n by 1 J<sup>Tr vector. </returns>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain methods in .NET:
//		private static org.apache.commons.math3.util.Pair<org.apache.commons.math3.linear.RealMatrix, org.apache.commons.math3.linear.RealVector> computeNormalMatrix(final org.apache.commons.math3.linear.RealMatrix jacobian, final org.apache.commons.math3.linear.RealVector residuals)
	//	{
	//		//since the normal matrix is symmetric, we only need to compute half of it.
	//		final int nR = jacobian.getRowDimension();
	//		final int nC = jacobian.getColumnDimension();
	//		//allocate space for return values
	//		final RealMatrix normal = MatrixUtils.createRealMatrix(nC, nC);
	//		final RealVector jTr = new ArrayRealVector(nC);
	//		//for each measurement
	//		for (int i = 0; i < nR; ++i)
	//		{
	//			//compute JTr for measurement i
	//			for (int j = 0; j < nC; j++)
	//			{
	//				jTr.setEntry(j, jTr.getEntry(j) + residuals.getEntry(i) * jacobian.getEntry(i, j));
	//			}
	//
	//			// add the the contribution to the normal matrix for measurement i
	//			for (int k = 0; k < nC; ++k)
	//			{
	//				//only compute the upper triangular part
	//				for (int l = k; l < nC; ++l)
	//				{
	//					normal.setEntry(k, l, normal.getEntry(k, l) + jacobian.getEntry(i, k) * jacobian.getEntry(i, l));
	//				}
	//			}
	//		}
	//		//copy the upper triangular part to the lower triangular part.
	//		for (int i = 0; i < nC; i++)
	//		{
	//			for (int j = 0; j < i; j++)
	//			{
	//				normal.setEntry(i, j, normal.getEntry(j, i));
	//			}
	//		}
	//		return new Pair<RealMatrix, RealVector>(normal, jTr);
	//	}

	}

}
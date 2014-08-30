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


	/// <summary>
	/// Interface handling decomposition algorithms that can solve A &times; X = B.
	/// <p>Decomposition algorithms decompose an A matrix has a product of several specific
	/// matrices from which they can solve A &times; X = B in least squares sense: they find X
	/// such that ||A &times; X - B|| is minimal.</p>
	/// <p>Some solvers like <seealso cref="FieldLUDecomposition"/> can only find the solution for
	/// square matrices and when the solution is an exact linear solution, i.e. when
	/// ||A &times; X - B|| is exactly 0. Other solvers can also find solutions
	/// with non-square matrix A and with non-null minimal norm. If an exact linear
	/// solution exists it is also the minimal norm solution.</p>
	/// </summary>
	/// @param <T> the type of the field elements
	/// @version $Id: FieldDecompositionSolver.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0 </param>
	public interface FieldDecompositionSolver<T> where T : org.apache.commons.math3.FieldElement<T>
	{

		/// <summary>
		/// Solve the linear equation A &times; X = B for matrices A.
		/// <p>The A matrix is implicit, it is provided by the underlying
		/// decomposition algorithm.</p> </summary>
		/// <param name="b"> right-hand side of the equation A &times; X = B </param>
		/// <returns> a vector X that minimizes the two norm of A &times; X - B </returns>
		/// <exception cref="org.apache.commons.math3.exception.DimensionMismatchException">
		/// if the matrices dimensions do not match. </exception>
		/// <exception cref="SingularMatrixException">
		/// if the decomposed matrix is singular. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: FieldVector<T> solve(final FieldVector<T> b);
		FieldVector<T> solve(FieldVector<T> b);

		/// <summary>
		/// Solve the linear equation A &times; X = B for matrices A.
		/// <p>The A matrix is implicit, it is provided by the underlying
		/// decomposition algorithm.</p> </summary>
		/// <param name="b"> right-hand side of the equation A &times; X = B </param>
		/// <returns> a matrix X that minimizes the two norm of A &times; X - B </returns>
		/// <exception cref="org.apache.commons.math3.exception.DimensionMismatchException">
		/// if the matrices dimensions do not match. </exception>
		/// <exception cref="SingularMatrixException">
		/// if the decomposed matrix is singular. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: FieldMatrix<T> solve(final FieldMatrix<T> b);
		FieldMatrix<T> solve(FieldMatrix<T> b);

		/// <summary>
		/// Check if the decomposed matrix is non-singular. </summary>
		/// <returns> true if the decomposed matrix is non-singular </returns>
		bool NonSingular {get;}

		/// <summary>
		/// Get the inverse (or pseudo-inverse) of the decomposed matrix. </summary>
		/// <returns> inverse matrix </returns>
		/// <exception cref="SingularMatrixException">
		/// if the decomposed matrix is singular. </exception>
		FieldMatrix<T> Inverse {get;}
	}

}
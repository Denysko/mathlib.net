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

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;

	/// <summary>
	/// This class defines a linear operator operating on real ({@code double})
	/// vector spaces. No direct access to the coefficients of the underlying matrix
	/// is provided.
	/// 
	/// The motivation for such an interface is well stated by
	/// <a href="#BARR1994">Barrett et al. (1994)</a>:
	/// <blockquote>
	///  We restrict ourselves to iterative methods, which work by repeatedly
	///  improving an approximate solution until it is accurate enough. These
	///  methods access the coefficient matrix A of the linear system only via the
	///  matrix-vector product y = A &middot; x
	///  (and perhaps z = A<sup>T</sup> &middot; x). Thus the user need only
	///  supply a subroutine for computing y (and perhaps z) given x, which permits
	///  full exploitation of the sparsity or other special structure of A.
	/// </blockquote>
	/// <br/>
	/// 
	/// <dl>
	///  <dt><a name="BARR1994">Barret et al. (1994)</a></dt>
	///  <dd>
	///   R. Barrett, M. Berry, T. F. Chan, J. Demmel, J. M. Donato, J. Dongarra,
	///   V. Eijkhout, R. Pozo, C. Romine and H. Van der Vorst,
	///   <em>Templates for the Solution of Linear Systems: Building Blocks for
	///   Iterative Methods</em>, SIAM
	///  </dd>
	/// </dl>
	/// 
	/// @version $Id: RealLinearOperator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0
	/// </summary>
	public abstract class RealLinearOperator
	{
		/// <summary>
		/// Returns the dimension of the codomain of this operator.
		/// </summary>
		/// <returns> the number of rows of the underlying matrix </returns>
		public abstract int RowDimension {get;}

		/// <summary>
		/// Returns the dimension of the domain of this operator.
		/// </summary>
		/// <returns> the number of columns of the underlying matrix </returns>
		public abstract int ColumnDimension {get;}

		/// <summary>
		/// Returns the result of multiplying {@code this} by the vector {@code x}.
		/// </summary>
		/// <param name="x"> the vector to operate on </param>
		/// <returns> the product of {@code this} instance with {@code x} </returns>
		/// <exception cref="DimensionMismatchException"> if the column dimension does not match
		/// the size of {@code x} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract RealVector operate(final RealVector x) throws mathlib.exception.DimensionMismatchException;
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public abstract RealVector operate(RealVector x);

		/// <summary>
		/// Returns the result of multiplying the transpose of {@code this} operator
		/// by the vector {@code x} (optional operation). The default implementation
		/// throws an <seealso cref="UnsupportedOperationException"/>. Users overriding this
		/// method must also override <seealso cref="#isTransposable()"/>.
		/// </summary>
		/// <param name="x"> the vector to operate on </param>
		/// <returns> the product of the transpose of {@code this} instance with
		/// {@code x} </returns>
		/// <exception cref="mathlib.exception.DimensionMismatchException">
		/// if the row dimension does not match the size of {@code x} </exception>
		/// <exception cref="UnsupportedOperationException"> if this operation is not supported
		/// by {@code this} operator </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector operateTranspose(final RealVector x) throws mathlib.exception.DimensionMismatchException, UnsupportedOperationException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealVector operateTranspose(RealVector x)
		{
			throw new System.NotSupportedException();
		}

		/// <summary>
		/// Returns {@code true} if this operator supports
		/// <seealso cref="#operateTranspose(RealVector)"/>. If {@code true} is returned,
		/// <seealso cref="#operateTranspose(RealVector)"/> should not throw
		/// {@code UnsupportedOperationException}. The default implementation returns
		/// {@code false}.
		/// </summary>
		/// <returns> {@code false} </returns>
		public virtual bool Transposable
		{
			get
			{
				return false;
			}
		}
	}

}
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

	using Sqrt = org.apache.commons.math3.analysis.function.Sqrt;
	using MathArrays = org.apache.commons.math3.util.MathArrays;

	/// <summary>
	/// This class implements the standard Jacobi (diagonal) preconditioner. For a
	/// matrix A<sub>ij</sub>, this preconditioner is
	/// M = diag(1 / A<sub>11</sub>, 1 / A<sub>22</sub>, &hellip;).
	/// 
	/// @version $Id: JacobiPreconditioner.java 1422195 2012-12-15 06:45:18Z psteitz $
	/// @since 3.0
	/// </summary>
	public class JacobiPreconditioner : RealLinearOperator
	{

		/// <summary>
		/// The diagonal coefficients of the preconditioner. </summary>
		private readonly ArrayRealVector diag;

		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="diag"> the diagonal coefficients of the linear operator to be
		/// preconditioned </param>
		/// <param name="deep"> {@code true} if a deep copy of the above array should be
		/// performed </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public JacobiPreconditioner(final double[] diag, final boolean deep)
		public JacobiPreconditioner(double[] diag, bool deep)
		{
			this.diag = new ArrayRealVector(diag, deep);
		}

		/// <summary>
		/// Creates a new instance of this class. This method extracts the diagonal
		/// coefficients of the specified linear operator. If {@code a} does not
		/// extend <seealso cref="AbstractRealMatrix"/>, then the coefficients of the
		/// underlying matrix are not accessible, coefficient extraction is made by
		/// matrix-vector products with the basis vectors (and might therefore take
		/// some time). With matrices, direct entry access is carried out.
		/// </summary>
		/// <param name="a"> the linear operator for which the preconditioner should be built </param>
		/// <returns> the diagonal preconditioner made of the inverse of the diagonal
		/// coefficients of the specified linear operator </returns>
		/// <exception cref="NonSquareOperatorException"> if {@code a} is not square </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JacobiPreconditioner create(final RealLinearOperator a) throws NonSquareOperatorException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static JacobiPreconditioner create(RealLinearOperator a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = a.getColumnDimension();
			int n = a.ColumnDimension;
			if (a.RowDimension != n)
			{
				throw new NonSquareOperatorException(a.RowDimension, n);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] diag = new double[n];
			double[] diag = new double[n];
			if (a is AbstractRealMatrix)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final AbstractRealMatrix m = (AbstractRealMatrix) a;
				AbstractRealMatrix m = (AbstractRealMatrix) a;
				for (int i = 0; i < n; i++)
				{
					diag[i] = m.getEntry(i, i);
				}
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ArrayRealVector x = new ArrayRealVector(n);
				ArrayRealVector x = new ArrayRealVector(n);
				for (int i = 0; i < n; i++)
				{
					x.set(0.0);
					x.setEntry(i, 1.0);
					diag[i] = a.operate(x).getEntry(i);
				}
			}
			return new JacobiPreconditioner(diag, false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int ColumnDimension
		{
			get
			{
				return diag.Dimension;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int RowDimension
		{
			get
			{
				return diag.Dimension;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public RealVector operate(final RealVector x)
		public override RealVector operate(RealVector x)
		{
			// Dimension check is carried out by ebeDivide
			return new ArrayRealVector(MathArrays.ebeDivide(x.toArray(), diag.toArray()), false);
		}

		/// <summary>
		/// Returns the square root of {@code this} diagonal operator. More
		/// precisely, this method returns
		/// P = diag(1 / &radic;A<sub>11</sub>, 1 / &radic;A<sub>22</sub>, &hellip;).
		/// </summary>
		/// <returns> the square root of {@code this} preconditioner
		/// @since 3.1 </returns>
		public virtual RealLinearOperator sqrt()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector sqrtDiag = diag.map(new org.apache.commons.math3.analysis.function.Sqrt());
			RealVector sqrtDiag = diag.map(new Sqrt());
			return new RealLinearOperatorAnonymousInnerClassHelper(this, sqrtDiag);
		}

		private class RealLinearOperatorAnonymousInnerClassHelper : RealLinearOperator
		{
			private readonly JacobiPreconditioner outerInstance;

			private org.apache.commons.math3.linear.RealVector sqrtDiag;

			public RealLinearOperatorAnonymousInnerClassHelper(JacobiPreconditioner outerInstance, org.apache.commons.math3.linear.RealVector sqrtDiag)
			{
				this.outerInstance = outerInstance;
				this.sqrtDiag = sqrtDiag;
			}

					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public RealVector operate(final RealVector x)
			public override RealVector operate(RealVector x)
			{
				return new ArrayRealVector(MathArrays.ebeDivide(x.toArray(), sqrtDiag.toArray()), false);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override int RowDimension
			{
				get
				{
					return sqrtDiag.Dimension;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override int ColumnDimension
			{
				get
				{
					return sqrtDiag.Dimension;
				}
			}
		}
	}

}
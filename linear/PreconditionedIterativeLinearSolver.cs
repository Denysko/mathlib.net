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
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using IterationManager = mathlib.util.IterationManager;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// <p>
	/// This abstract class defines preconditioned iterative solvers. When A is
	/// ill-conditioned, instead of solving system A &middot; x = b directly, it is
	/// preferable to solve either
	/// <center>
	/// (M &middot; A) &middot; x = M &middot; b
	/// </center>
	/// (left preconditioning), or
	/// <center>
	/// (A &middot; M) &middot; y = b, &nbsp;&nbsp;&nbsp;&nbsp;followed by
	/// M &middot; y = x
	/// </center>
	/// (right preconditioning), where M approximates in some way A<sup>-1</sup>,
	/// while matrix-vector products of the type M &middot; y remain comparatively
	/// easy to compute. In this library, M (not M<sup>-1</sup>!) is called the
	/// <em>preconditionner</em>.
	/// </p>
	/// <p>
	/// Concrete implementations of this abstract class must be provided with the
	/// preconditioner M, as a <seealso cref="RealLinearOperator"/>.
	/// </p>
	/// 
	/// @version $Id: PreconditionedIterativeLinearSolver.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0
	/// </summary>
	public abstract class PreconditionedIterativeLinearSolver : IterativeLinearSolver
	{

		/// <summary>
		/// Creates a new instance of this class, with default iteration manager.
		/// </summary>
		/// <param name="maxIterations"> the maximum number of iterations </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PreconditionedIterativeLinearSolver(final int maxIterations)
		public PreconditionedIterativeLinearSolver(int maxIterations) : base(maxIterations)
		{
		}

		/// <summary>
		/// Creates a new instance of this class, with custom iteration manager.
		/// </summary>
		/// <param name="manager"> the custom iteration manager </param>
		/// <exception cref="NullArgumentException"> if {@code manager} is {@code null} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PreconditionedIterativeLinearSolver(final mathlib.util.IterationManager manager) throws mathlib.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public PreconditionedIterativeLinearSolver(IterationManager manager) : base(manager)
		{
		}

		/// <summary>
		/// Returns an estimate of the solution to the linear system A &middot; x =
		/// b.
		/// </summary>
		/// <param name="a"> the linear operator A of the system </param>
		/// <param name="m"> the preconditioner, M (can be {@code null}) </param>
		/// <param name="b"> the right-hand side vector </param>
		/// <param name="x0"> the initial guess of the solution </param>
		/// <returns> a new vector containing the solution </returns>
		/// <exception cref="NullArgumentException"> if one of the parameters is {@code null} </exception>
		/// <exception cref="NonSquareOperatorException"> if {@code a} or {@code m} is not
		/// square </exception>
		/// <exception cref="DimensionMismatchException"> if {@code m}, {@code b} or
		/// {@code x0} have dimensions inconsistent with {@code a} </exception>
		/// <exception cref="MaxCountExceededException"> at exhaustion of the iteration count,
		/// unless a custom
		/// <seealso cref="mathlib.util.Incrementor.MaxCountExceededCallback callback"/>
		/// has been set at construction of the <seealso cref="IterationManager"/> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector solve(final RealLinearOperator a, final RealLinearOperator m, final RealVector b, final RealVector x0) throws mathlib.exception.NullArgumentException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealVector solve(RealLinearOperator a, RealLinearOperator m, RealVector b, RealVector x0)
		{
			MathUtils.checkNotNull(x0);
			return solveInPlace(a, m, b, x0.copy());
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector solve(final RealLinearOperator a, final RealVector b) throws mathlib.exception.NullArgumentException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealVector solve(RealLinearOperator a, RealVector b)
		{
			MathUtils.checkNotNull(a);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector x = new ArrayRealVector(a.getColumnDimension());
			RealVector x = new ArrayRealVector(a.ColumnDimension);
			x.set(0.0);
			return solveInPlace(a, null, b, x);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector solve(final RealLinearOperator a, final RealVector b, final RealVector x0) throws mathlib.exception.NullArgumentException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealVector solve(RealLinearOperator a, RealVector b, RealVector x0)
		{
			MathUtils.checkNotNull(x0);
			return solveInPlace(a, null, b, x0.copy());
		}

		/// <summary>
		/// Performs all dimension checks on the parameters of
		/// <seealso cref="#solve(RealLinearOperator, RealLinearOperator, RealVector, RealVector) solve"/>
		/// and
		/// <seealso cref="#solveInPlace(RealLinearOperator, RealLinearOperator, RealVector, RealVector) solveInPlace"/>,
		/// and throws an exception if one of the checks fails.
		/// </summary>
		/// <param name="a"> the linear operator A of the system </param>
		/// <param name="m"> the preconditioner, M (can be {@code null}) </param>
		/// <param name="b"> the right-hand side vector </param>
		/// <param name="x0"> the initial guess of the solution </param>
		/// <exception cref="NullArgumentException"> if one of the parameters is {@code null} </exception>
		/// <exception cref="NonSquareOperatorException"> if {@code a} or {@code m} is not
		/// square </exception>
		/// <exception cref="DimensionMismatchException"> if {@code m}, {@code b} or
		/// {@code x0} have dimensions inconsistent with {@code a} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static void checkParameters(final RealLinearOperator a, final RealLinearOperator m, final RealVector b, final RealVector x0) throws mathlib.exception.NullArgumentException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal static void checkParameters(RealLinearOperator a, RealLinearOperator m, RealVector b, RealVector x0)
		{
			checkParameters(a, b, x0);
			if (m != null)
			{
				if (m.ColumnDimension != m.RowDimension)
				{
					throw new NonSquareOperatorException(m.ColumnDimension, m.RowDimension);
				}
				if (m.RowDimension != a.RowDimension)
				{
					throw new DimensionMismatchException(m.RowDimension, a.RowDimension);
				}
			}
		}

		/// <summary>
		/// Returns an estimate of the solution to the linear system A &middot; x =
		/// b.
		/// </summary>
		/// <param name="a"> the linear operator A of the system </param>
		/// <param name="m"> the preconditioner, M (can be {@code null}) </param>
		/// <param name="b"> the right-hand side vector </param>
		/// <returns> a new vector containing the solution </returns>
		/// <exception cref="NullArgumentException"> if one of the parameters is {@code null} </exception>
		/// <exception cref="NonSquareOperatorException"> if {@code a} or {@code m} is not
		/// square </exception>
		/// <exception cref="DimensionMismatchException"> if {@code m} or {@code b} have
		/// dimensions inconsistent with {@code a} </exception>
		/// <exception cref="MaxCountExceededException"> at exhaustion of the iteration count,
		/// unless a custom
		/// <seealso cref="mathlib.util.Incrementor.MaxCountExceededCallback callback"/>
		/// has been set at construction of the <seealso cref="IterationManager"/> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector solve(RealLinearOperator a, RealLinearOperator m, RealVector b) throws mathlib.exception.NullArgumentException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException, mathlib.exception.MaxCountExceededException
		public virtual RealVector solve(RealLinearOperator a, RealLinearOperator m, RealVector b)
		{
			MathUtils.checkNotNull(a);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector x = new ArrayRealVector(a.getColumnDimension());
			RealVector x = new ArrayRealVector(a.ColumnDimension);
			return solveInPlace(a, m, b, x);
		}

		/// <summary>
		/// Returns an estimate of the solution to the linear system A &middot; x =
		/// b. The solution is computed in-place (initial guess is modified).
		/// </summary>
		/// <param name="a"> the linear operator A of the system </param>
		/// <param name="m"> the preconditioner, M (can be {@code null}) </param>
		/// <param name="b"> the right-hand side vector </param>
		/// <param name="x0"> the initial guess of the solution </param>
		/// <returns> a reference to {@code x0} (shallow copy) updated with the
		/// solution </returns>
		/// <exception cref="NullArgumentException"> if one of the parameters is {@code null} </exception>
		/// <exception cref="NonSquareOperatorException"> if {@code a} or {@code m} is not
		/// square </exception>
		/// <exception cref="DimensionMismatchException"> if {@code m}, {@code b} or
		/// {@code x0} have dimensions inconsistent with {@code a} </exception>
		/// <exception cref="MaxCountExceededException"> at exhaustion of the iteration count,
		/// unless a custom
		/// <seealso cref="mathlib.util.Incrementor.MaxCountExceededCallback callback"/>
		/// has been set at construction of the <seealso cref="IterationManager"/> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract RealVector solveInPlace(RealLinearOperator a, RealLinearOperator m, RealVector b, RealVector x0) throws mathlib.exception.NullArgumentException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException, mathlib.exception.MaxCountExceededException;
		public abstract RealVector solveInPlace(RealLinearOperator a, RealLinearOperator m, RealVector b, RealVector x0);

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector solveInPlace(final RealLinearOperator a, final RealVector b, final RealVector x0) throws mathlib.exception.NullArgumentException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealVector solveInPlace(RealLinearOperator a, RealVector b, RealVector x0)
		{
			return solveInPlace(a, null, b, x0);
		}
	}

}
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
	using MaxCountExceededException = org.apache.commons.math3.exception.MaxCountExceededException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using IterationManager = org.apache.commons.math3.util.IterationManager;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// This abstract class defines an iterative solver for the linear system A
	/// &middot; x = b. In what follows, the <em>residual</em> r is defined as r = b
	/// - A &middot; x, where A is the linear operator of the linear system, b is the
	/// right-hand side vector, and x the current estimate of the solution.
	/// 
	/// @version $Id: IterativeLinearSolver.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0
	/// </summary>
	public abstract class IterativeLinearSolver
	{

		/// <summary>
		/// The object in charge of managing the iterations. </summary>
		private readonly IterationManager manager;

		/// <summary>
		/// Creates a new instance of this class, with default iteration manager.
		/// </summary>
		/// <param name="maxIterations"> the maximum number of iterations </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public IterativeLinearSolver(final int maxIterations)
		public IterativeLinearSolver(int maxIterations)
		{
			this.manager = new IterationManager(maxIterations);
		}

		/// <summary>
		/// Creates a new instance of this class, with custom iteration manager.
		/// </summary>
		/// <param name="manager"> the custom iteration manager </param>
		/// <exception cref="NullArgumentException"> if {@code manager} is {@code null} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public IterativeLinearSolver(final org.apache.commons.math3.util.IterationManager manager) throws org.apache.commons.math3.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public IterativeLinearSolver(IterationManager manager)
		{
			MathUtils.checkNotNull(manager);
			this.manager = manager;
		}

		/// <summary>
		/// Performs all dimension checks on the parameters of
		/// <seealso cref="#solve(RealLinearOperator, RealVector, RealVector) solve"/> and
		/// <seealso cref="#solveInPlace(RealLinearOperator, RealVector, RealVector) solveInPlace"/>,
		/// and throws an exception if one of the checks fails.
		/// </summary>
		/// <param name="a"> the linear operator A of the system </param>
		/// <param name="b"> the right-hand side vector </param>
		/// <param name="x0"> the initial guess of the solution </param>
		/// <exception cref="NullArgumentException"> if one of the parameters is {@code null} </exception>
		/// <exception cref="NonSquareOperatorException"> if {@code a} is not square </exception>
		/// <exception cref="DimensionMismatchException"> if {@code b} or {@code x0} have
		/// dimensions inconsistent with {@code a} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static void checkParameters(final RealLinearOperator a, final RealVector b, final RealVector x0) throws org.apache.commons.math3.exception.NullArgumentException, NonSquareOperatorException, org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal static void checkParameters(RealLinearOperator a, RealVector b, RealVector x0)
		{
			MathUtils.checkNotNull(a);
			MathUtils.checkNotNull(b);
			MathUtils.checkNotNull(x0);
			if (a.RowDimension != a.ColumnDimension)
			{
				throw new NonSquareOperatorException(a.RowDimension, a.ColumnDimension);
			}
			if (b.Dimension != a.RowDimension)
			{
				throw new DimensionMismatchException(b.Dimension, a.RowDimension);
			}
			if (x0.Dimension != a.ColumnDimension)
			{
				throw new DimensionMismatchException(x0.Dimension, a.ColumnDimension);
			}
		}

		/// <summary>
		/// Returns the iteration manager attached to this solver.
		/// </summary>
		/// <returns> the manager </returns>
		public virtual IterationManager IterationManager
		{
			get
			{
				return manager;
			}
		}

		/// <summary>
		/// Returns an estimate of the solution to the linear system A &middot; x =
		/// b.
		/// </summary>
		/// <param name="a"> the linear operator A of the system </param>
		/// <param name="b"> the right-hand side vector </param>
		/// <returns> a new vector containing the solution </returns>
		/// <exception cref="NullArgumentException"> if one of the parameters is {@code null} </exception>
		/// <exception cref="NonSquareOperatorException"> if {@code a} is not square </exception>
		/// <exception cref="DimensionMismatchException"> if {@code b} has dimensions
		/// inconsistent with {@code a} </exception>
		/// <exception cref="MaxCountExceededException"> at exhaustion of the iteration count,
		/// unless a custom
		/// <seealso cref="org.apache.commons.math3.util.Incrementor.MaxCountExceededCallback callback"/>
		/// has been set at construction of the <seealso cref="IterationManager"/> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector solve(final RealLinearOperator a, final RealVector b) throws org.apache.commons.math3.exception.NullArgumentException, NonSquareOperatorException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealVector solve(RealLinearOperator a, RealVector b)
		{
			MathUtils.checkNotNull(a);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector x = new ArrayRealVector(a.getColumnDimension());
			RealVector x = new ArrayRealVector(a.ColumnDimension);
			x.set(0.0);
			return solveInPlace(a, b, x);
		}

		/// <summary>
		/// Returns an estimate of the solution to the linear system A &middot; x =
		/// b.
		/// </summary>
		/// <param name="a"> the linear operator A of the system </param>
		/// <param name="b"> the right-hand side vector </param>
		/// <param name="x0"> the initial guess of the solution </param>
		/// <returns> a new vector containing the solution </returns>
		/// <exception cref="NullArgumentException"> if one of the parameters is {@code null} </exception>
		/// <exception cref="NonSquareOperatorException"> if {@code a} is not square </exception>
		/// <exception cref="DimensionMismatchException"> if {@code b} or {@code x0} have
		/// dimensions inconsistent with {@code a} </exception>
		/// <exception cref="MaxCountExceededException"> at exhaustion of the iteration count,
		/// unless a custom
		/// <seealso cref="org.apache.commons.math3.util.Incrementor.MaxCountExceededCallback callback"/>
		/// has been set at construction of the <seealso cref="IterationManager"/> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector solve(RealLinearOperator a, RealVector b, RealVector x0) throws org.apache.commons.math3.exception.NullArgumentException, NonSquareOperatorException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MaxCountExceededException
		public virtual RealVector solve(RealLinearOperator a, RealVector b, RealVector x0)
		{
			MathUtils.checkNotNull(x0);
			return solveInPlace(a, b, x0.copy());
		}

		/// <summary>
		/// Returns an estimate of the solution to the linear system A &middot; x =
		/// b. The solution is computed in-place (initial guess is modified).
		/// </summary>
		/// <param name="a"> the linear operator A of the system </param>
		/// <param name="b"> the right-hand side vector </param>
		/// <param name="x0"> initial guess of the solution </param>
		/// <returns> a reference to {@code x0} (shallow copy) updated with the
		/// solution </returns>
		/// <exception cref="NullArgumentException"> if one of the parameters is {@code null} </exception>
		/// <exception cref="NonSquareOperatorException"> if {@code a} is not square </exception>
		/// <exception cref="DimensionMismatchException"> if {@code b} or {@code x0} have
		/// dimensions inconsistent with {@code a} </exception>
		/// <exception cref="MaxCountExceededException"> at exhaustion of the iteration count,
		/// unless a custom
		/// <seealso cref="org.apache.commons.math3.util.Incrementor.MaxCountExceededCallback callback"/>
		/// has been set at construction of the <seealso cref="IterationManager"/> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract RealVector solveInPlace(RealLinearOperator a, RealVector b, RealVector x0) throws org.apache.commons.math3.exception.NullArgumentException, NonSquareOperatorException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MaxCountExceededException;
		public abstract RealVector solveInPlace(RealLinearOperator a, RealVector b, RealVector x0);
	}

}
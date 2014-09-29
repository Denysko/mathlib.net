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

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using ExceptionContext = mathlib.exception.util.ExceptionContext;
	using FastMath = mathlib.util.FastMath;
	using IterationManager = mathlib.util.IterationManager;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// <p>
	/// Implementation of the SYMMLQ iterative linear solver proposed by <a
	/// href="#PAIG1975">Paige and Saunders (1975)</a>. This implementation is
	/// largely based on the FORTRAN code by Pr. Michael A. Saunders, available <a
	/// href="http://www.stanford.edu/group/SOL/software/symmlq/f77/">here</a>.
	/// </p>
	/// <p>
	/// SYMMLQ is designed to solve the system of linear equations A &middot; x = b
	/// where A is an n &times; n self-adjoint linear operator (defined as a
	/// <seealso cref="RealLinearOperator"/>), and b is a given vector. The operator A is not
	/// required to be positive definite. If A is known to be definite, the method of
	/// conjugate gradients might be preferred, since it will require about the same
	/// number of iterations as SYMMLQ but slightly less work per iteration.
	/// </p>
	/// <p>
	/// SYMMLQ is designed to solve the system (A - shift &middot; I) &middot; x = b,
	/// where shift is a specified scalar value. If shift and b are suitably chosen,
	/// the computed vector x may approximate an (unnormalized) eigenvector of A, as
	/// in the methods of inverse iteration and/or Rayleigh-quotient iteration.
	/// Again, the linear operator (A - shift &middot; I) need not be positive
	/// definite (but <em>must</em> be self-adjoint). The work per iteration is very
	/// slightly less if shift = 0.
	/// </p>
	/// <h3>Preconditioning</h3>
	/// <p>
	/// Preconditioning may reduce the number of iterations required. The solver may
	/// be provided with a positive definite preconditioner
	/// M = P<sup>T</sup> &middot; P
	/// that is known to approximate
	/// (A - shift &middot; I)<sup>-1</sup> in some sense, where matrix-vector
	/// products of the form M &middot; y = x can be computed efficiently. Then
	/// SYMMLQ will implicitly solve the system of equations
	/// P &middot; (A - shift &middot; I) &middot; P<sup>T</sup> &middot;
	/// x<sub>hat</sub> = P &middot; b, i.e.
	/// A<sub>hat</sub> &middot; x<sub>hat</sub> = b<sub>hat</sub>,
	/// where
	/// A<sub>hat</sub> = P &middot; (A - shift &middot; I) &middot; P<sup>T</sup>,
	/// b<sub>hat</sub> = P &middot; b,
	/// and return the solution
	/// x = P<sup>T</sup> &middot; x<sub>hat</sub>.
	/// The associated residual is
	/// r<sub>hat</sub> = b<sub>hat</sub> - A<sub>hat</sub> &middot; x<sub>hat</sub>
	///                 = P &middot; [b - (A - shift &middot; I) &middot; x]
	///                 = P &middot; r.
	/// </p>
	/// <p>
	/// In the case of preconditioning, the <seealso cref="IterativeLinearSolverEvent"/>s that
	/// this solver fires are such that
	/// <seealso cref="IterativeLinearSolverEvent#getNormOfResidual()"/> returns the norm of
	/// the <em>preconditioned</em>, updated residual, ||P &middot; r||, not the norm
	/// of the <em>true</em> residual ||r||.
	/// </p>
	/// <h3><a id="stopcrit">Default stopping criterion</a></h3>
	/// <p>
	/// A default stopping criterion is implemented. The iterations stop when || rhat
	/// || &le; &delta; || Ahat || || xhat ||, where xhat is the current estimate of
	/// the solution of the transformed system, rhat the current estimate of the
	/// corresponding residual, and &delta; a user-specified tolerance.
	/// </p>
	/// <h3>Iteration count</h3>
	/// <p>
	/// In the present context, an iteration should be understood as one evaluation
	/// of the matrix-vector product A &middot; x. The initialization phase therefore
	/// counts as one iteration. If the user requires checks on the symmetry of A,
	/// this entails one further matrix-vector product in the initial phase. This
	/// further product is <em>not</em> accounted for in the iteration count. In
	/// other words, the number of iterations required to reach convergence will be
	/// identical, whether checks have been required or not.
	/// </p>
	/// <p>
	/// The present definition of the iteration count differs from that adopted in
	/// the original FOTRAN code, where the initialization phase was <em>not</em>
	/// taken into account.
	/// </p>
	/// <h3><a id="initguess">Initial guess of the solution</a></h3>
	/// <p>
	/// The {@code x} parameter in
	/// <ul>
	/// <li><seealso cref="#solve(RealLinearOperator, RealVector, RealVector)"/>,</li>
	/// <li><seealso cref="#solve(RealLinearOperator, RealLinearOperator, RealVector, RealVector)"/>},</li>
	/// <li><seealso cref="#solveInPlace(RealLinearOperator, RealVector, RealVector)"/>,</li>
	/// <li><seealso cref="#solveInPlace(RealLinearOperator, RealLinearOperator, RealVector, RealVector)"/>,</li>
	/// <li><seealso cref="#solveInPlace(RealLinearOperator, RealLinearOperator, RealVector, RealVector, boolean, double)"/>,</li>
	/// </ul>
	/// should not be considered as an initial guess, as it is set to zero in the
	/// initial phase. If x<sub>0</sub> is known to be a good approximation to x, one
	/// should compute r<sub>0</sub> = b - A &middot; x, solve A &middot; dx = r0,
	/// and set x = x<sub>0</sub> + dx.
	/// </p>
	/// <h3><a id="context">Exception context</a></h3>
	/// <p>
	/// Besides standard <seealso cref="DimensionMismatchException"/>, this class might throw
	/// <seealso cref="NonSelfAdjointOperatorException"/> if the linear operator or the
	/// preconditioner are not symmetric. In this case, the <seealso cref="ExceptionContext"/>
	/// provides more information
	/// <ul>
	/// <li>key {@code "operator"} points to the offending linear operator, say L,</li>
	/// <li>key {@code "vector1"} points to the first offending vector, say x,
	/// <li>key {@code "vector2"} points to the second offending vector, say y, such
	/// that x<sup>T</sup> &middot; L &middot; y &ne; y<sup>T</sup> &middot; L
	/// &middot; x (within a certain accuracy).</li>
	/// </ul>
	/// </p>
	/// <p>
	/// <seealso cref="NonPositiveDefiniteOperatorException"/> might also be thrown in case the
	/// preconditioner is not positive definite. The relevant keys to the
	/// <seealso cref="ExceptionContext"/> are
	/// <ul>
	/// <li>key {@code "operator"}, which points to the offending linear operator,
	/// say L,</li>
	/// <li>key {@code "vector"}, which points to the offending vector, say x, such
	/// that x<sup>T</sup> &middot; L &middot; x < 0.</li>
	/// </ul>
	/// </p>
	/// <h3>References</h3>
	/// <dl>
	/// <dt><a id="PAIG1975">Paige and Saunders (1975)</a></dt>
	/// <dd>C. C. Paige and M. A. Saunders, <a
	/// href="http://www.stanford.edu/group/SOL/software/symmlq/PS75.pdf"><em>
	/// Solution of Sparse Indefinite Systems of Linear Equations</em></a>, SIAM
	/// Journal on Numerical Analysis 12(4): 617-629, 1975</dd>
	/// </dl>
	/// 
	/// @version $Id: SymmLQ.java 1505938 2013-07-23 08:50:10Z luc $
	/// @since 3.0
	/// </summary>
	public class SymmLQ : PreconditionedIterativeLinearSolver
	{

		/*
		 * IMPLEMENTATION NOTES
		 * --------------------
		 * The implementation follows as closely as possible the notations of Paige
		 * and Saunders (1975). Attention must be paid to the fact that some
		 * quantities which are relevant to iteration k can only be computed in
		 * iteration (k+1). Therefore, minute attention must be paid to the index of
		 * each state variable of this algorithm.
		 *
		 * 1. Preconditioning
		 *    ---------------
		 * The Lanczos iterations associated with Ahat and bhat read
		 *   beta[1] = ||P * b||
		 *   v[1] = P * b / beta[1]
		 *   beta[k+1] * v[k+1] = Ahat * v[k] - alpha[k] * v[k] - beta[k] * v[k-1]
		 *                      = P * (A - shift * I) * P' * v[k] - alpha[k] * v[k]
		 *                        - beta[k] * v[k-1]
		 * Multiplying both sides by P', we get
		 *   beta[k+1] * (P' * v)[k+1] = M * (A - shift * I) * (P' * v)[k]
		 *                               - alpha[k] * (P' * v)[k]
		 *                               - beta[k] * (P' * v[k-1]),
		 * and
		 *   alpha[k+1] = v[k+1]' * Ahat * v[k+1]
		 *              = v[k+1]' * P * (A - shift * I) * P' * v[k+1]
		 *              = (P' * v)[k+1]' * (A - shift * I) * (P' * v)[k+1].
		 *
		 * In other words, the Lanczos iterations are unchanged, except for the fact
		 * that we really compute (P' * v) instead of v. It can easily be checked
		 * that all other formulas are unchanged. It must be noted that P is never
		 * explicitly used, only matrix-vector products involving are invoked.
		 *
		 * 2. Accounting for the shift parameter
		 *    ----------------------------------
		 * Is trivial: each time A.operate(x) is invoked, one must subtract shift * x
		 * to the result.
		 *
		 * 3. Accounting for the goodb flag
		 *    -----------------------------
		 * When goodb is set to true, the component of xL along b is computed
		 * separately. From Paige and Saunders (1975), equation (5.9), we have
		 *   wbar[k+1] = s[k] * wbar[k] - c[k] * v[k+1],
		 *   wbar[1] = v[1].
		 * Introducing wbar2[k] = wbar[k] - s[1] * ... * s[k-1] * v[1], it can
		 * easily be verified by induction that wbar2 follows the same recursive
		 * relation
		 *   wbar2[k+1] = s[k] * wbar2[k] - c[k] * v[k+1],
		 *   wbar2[1] = 0,
		 * and we then have
		 *   w[k] = c[k] * wbar2[k] + s[k] * v[k+1]
		 *          + s[1] * ... * s[k-1] * c[k] * v[1].
		 * Introducing w2[k] = w[k] - s[1] * ... * s[k-1] * c[k] * v[1], we find,
		 * from (5.10)
		 *   xL[k] = zeta[1] * w[1] + ... + zeta[k] * w[k]
		 *         = zeta[1] * w2[1] + ... + zeta[k] * w2[k]
		 *           + (s[1] * c[2] * zeta[2] + ...
		 *           + s[1] * ... * s[k-1] * c[k] * zeta[k]) * v[1]
		 *         = xL2[k] + bstep[k] * v[1],
		 * where xL2[k] is defined by
		 *   xL2[0] = 0,
		 *   xL2[k+1] = xL2[k] + zeta[k+1] * w2[k+1],
		 * and bstep is defined by
		 *   bstep[1] = 0,
		 *   bstep[k] = bstep[k-1] + s[1] * ... * s[k-1] * c[k] * zeta[k].
		 * We also have, from (5.11)
		 *   xC[k] = xL[k-1] + zbar[k] * wbar[k]
		 *         = xL2[k-1] + zbar[k] * wbar2[k]
		 *           + (bstep[k-1] + s[1] * ... * s[k-1] * zbar[k]) * v[1].
		 */

		/// <summary>
		/// <p>
		/// A simple container holding the non-final variables used in the
		/// iterations. Making the current state of the solver visible from the
		/// outside is necessary, because during the iterations, {@code x} does not
		/// <em>exactly</em> hold the current estimate of the solution. Indeed,
		/// {@code x} needs in general to be moved from the LQ point to the CG point.
		/// Besides, additional upudates must be carried out in case {@code goodb} is
		/// set to {@code true}.
		/// </p>
		/// <p>
		/// In all subsequent comments, the description of the state variables refer
		/// to their value after a call to <seealso cref="#update()"/>. In these comments, k is
		/// the current number of evaluations of matrix-vector products.
		/// </p>
		/// </summary>
		private class State
		{
			/// <summary>
			/// The cubic root of <seealso cref="#MACH_PREC"/>. </summary>
			internal static readonly double CBRT_MACH_PREC;

			/// <summary>
			/// The machine precision. </summary>
			internal static readonly double MACH_PREC;

			/// <summary>
			/// Reference to the linear operator. </summary>
			internal readonly RealLinearOperator a;

			/// <summary>
			/// Reference to the right-hand side vector. </summary>
			internal readonly RealVector b;

			/// <summary>
			/// {@code true} if symmetry of matrix and conditioner must be checked. </summary>
			internal readonly bool check;

			/// <summary>
			/// The value of the custom tolerance &delta; for the default stopping
			/// criterion.
			/// </summary>
			internal readonly double delta;

			/// <summary>
			/// The value of beta[k+1]. </summary>
			internal double beta;

			/// <summary>
			/// The value of beta[1]. </summary>
			internal double beta1;

			/// <summary>
			/// The value of bstep[k-1]. </summary>
			internal double bstep;

			/// <summary>
			/// The estimate of the norm of P * rC[k]. </summary>
			internal double cgnorm;

			/// <summary>
			/// The value of dbar[k+1] = -beta[k+1] * c[k-1]. </summary>
			internal double dbar;

			/// <summary>
			/// The value of gamma[k] * zeta[k]. Was called {@code rhs1} in the
			/// initial code.
			/// </summary>
			internal double gammaZeta;

			/// <summary>
			/// The value of gbar[k]. </summary>
			internal double gbar;

			/// <summary>
			/// The value of max(|alpha[1]|, gamma[1], ..., gamma[k-1]). </summary>
			internal double gmax;

			/// <summary>
			/// The value of min(|alpha[1]|, gamma[1], ..., gamma[k-1]). </summary>
			internal double gmin;

			/// <summary>
			/// Copy of the {@code goodb} parameter. </summary>
			internal readonly bool goodb;

			/// <summary>
			/// {@code true} if the default convergence criterion is verified. </summary>
			internal bool hasConverged_Renamed;

			/// <summary>
			/// The estimate of the norm of P * rL[k-1]. </summary>
			internal double lqnorm;

			/// <summary>
			/// Reference to the preconditioner, M. </summary>
			internal readonly RealLinearOperator m;

			/// <summary>
			/// The value of (-eps[k+1] * zeta[k-1]). Was called {@code rhs2} in the
			/// initial code.
			/// </summary>
			internal double minusEpsZeta;

			/// <summary>
			/// The value of M * b. </summary>
			internal readonly RealVector mb;

			/// <summary>
			/// The value of beta[k]. </summary>
			internal double oldb;

			/// <summary>
			/// The value of beta[k] * M^(-1) * P' * v[k]. </summary>
			internal RealVector r1;

			/// <summary>
			/// The value of beta[k+1] * M^(-1) * P' * v[k+1]. </summary>
			internal RealVector r2;

			/// <summary>
			/// The value of the updated, preconditioned residual P * r. This value is
			/// given by {@code min(}<seealso cref="#cgnorm"/>{@code , }<seealso cref="#lqnorm"/>{@code )}.
			/// </summary>
			internal double rnorm;

			/// <summary>
			/// Copy of the {@code shift} parameter. </summary>
			internal readonly double shift;

			/// <summary>
			/// The value of s[1] * ... * s[k-1]. </summary>
			internal double snprod;

			/// <summary>
			/// An estimate of the square of the norm of A * V[k], based on Paige and
			/// Saunders (1975), equation (3.3).
			/// </summary>
			internal double tnorm;

			/// <summary>
			/// The value of P' * wbar[k] or P' * (wbar[k] - s[1] * ... * s[k-1] *
			/// v[1]) if {@code goodb} is {@code true}. Was called {@code w} in the
			/// initial code.
			/// </summary>
			internal RealVector wbar;

			/// <summary>
			/// A reference to the vector to be updated with the solution. Contains
			/// the value of xL[k-1] if {@code goodb} is {@code false}, (xL[k-1] -
			/// bstep[k-1] * v[1]) otherwise.
			/// </summary>
			internal readonly RealVector xL;

			/// <summary>
			/// The value of beta[k+1] * P' * v[k+1]. </summary>
			internal RealVector y;

			/// <summary>
			/// The value of zeta[1]^2 + ... + zeta[k-1]^2. </summary>
			internal double ynorm2;

			/// <summary>
			/// The value of {@code b == 0} (exact floating-point equality). </summary>
			internal bool bIsNull;

			static State()
			{
				MACH_PREC = FastMath.ulp(1.0);
				CBRT_MACH_PREC = FastMath.cbrt(MACH_PREC);
			}

			/// <summary>
			/// Creates and inits to k = 1 a new instance of this class.
			/// </summary>
			/// <param name="a"> the linear operator A of the system </param>
			/// <param name="m"> the preconditioner, M (can be {@code null}) </param>
			/// <param name="b"> the right-hand side vector </param>
			/// <param name="goodb"> usually {@code false}, except if {@code x} is expected
			/// to contain a large multiple of {@code b} </param>
			/// <param name="shift"> the amount to be subtracted to all diagonal elements of
			/// A </param>
			/// <param name="delta"> the &delta; parameter for the default stopping criterion </param>
			/// <param name="check"> {@code true} if self-adjointedness of both matrix and
			/// preconditioner should be checked </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public State(final RealLinearOperator a, final RealLinearOperator m, final RealVector b, final boolean goodb, final double shift, final double delta, final boolean check)
			public State(RealLinearOperator a, RealLinearOperator m, RealVector b, bool goodb, double shift, double delta, bool check)
			{
				this.a = a;
				this.m = m;
				this.b = b;
				this.xL = new ArrayRealVector(b.Dimension);
				this.goodb = goodb;
				this.shift = shift;
				this.mb = m == null ? b : m.operate(b);
				this.hasConverged_Renamed = false;
				this.check = check;
				this.delta = delta;
			}

			/// <summary>
			/// Performs a symmetry check on the specified linear operator, and throws an
			/// exception in case this check fails. Given a linear operator L, and a
			/// vector x, this method checks that
			/// x' &middot; L &middot; y = y' &middot; L &middot; x
			/// (within a given accuracy), where y = L &middot; x.
			/// </summary>
			/// <param name="l"> the linear operator L </param>
			/// <param name="x"> the candidate vector x </param>
			/// <param name="y"> the candidate vector y = L &middot; x </param>
			/// <param name="z"> the vector z = L &middot; y </param>
			/// <exception cref="NonSelfAdjointOperatorException"> when the test fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void checkSymmetry(final RealLinearOperator l, final RealVector x, final RealVector y, final RealVector z) throws NonSelfAdjointOperatorException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			internal static void checkSymmetry(RealLinearOperator l, RealVector x, RealVector y, RealVector z)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s = y.dotProduct(y);
				double s = y.dotProduct(y);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = x.dotProduct(z);
				double t = x.dotProduct(z);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double epsa = (s + MACH_PREC) * CBRT_MACH_PREC;
				double epsa = (s + MACH_PREC) * CBRT_MACH_PREC;
				if (FastMath.abs(s - t) > epsa)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final NonSelfAdjointOperatorException e;
					NonSelfAdjointOperatorException e;
					e = new NonSelfAdjointOperatorException();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.exception.util.ExceptionContext context = e.getContext();
					ExceptionContext context = e.Context;
					context.setValue(SymmLQ.OPERATOR, l);
					context.setValue(SymmLQ.VECTOR1, x);
					context.setValue(SymmLQ.VECTOR2, y);
					context.setValue(SymmLQ.THRESHOLD, Convert.ToDouble(epsa));
					throw e;
				}
			}

			/// <summary>
			/// Throws a new <seealso cref="NonPositiveDefiniteOperatorException"/> with
			/// appropriate context.
			/// </summary>
			/// <param name="l"> the offending linear operator </param>
			/// <param name="v"> the offending vector </param>
			/// <exception cref="NonPositiveDefiniteOperatorException"> in any circumstances </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void throwNPDLOException(final RealLinearOperator l, final RealVector v) throws NonPositiveDefiniteOperatorException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			internal static void throwNPDLOException(RealLinearOperator l, RealVector v)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final NonPositiveDefiniteOperatorException e;
				NonPositiveDefiniteOperatorException e;
				e = new NonPositiveDefiniteOperatorException();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.exception.util.ExceptionContext context = e.getContext();
				ExceptionContext context = e.Context;
				context.setValue(OPERATOR, l);
				context.setValue(VECTOR, v);
				throw e;
			}

			/// <summary>
			/// A clone of the BLAS {@code DAXPY} function, which carries out the
			/// operation y &larr; a &middot; x + y. This is for internal use only: no
			/// dimension checks are provided.
			/// </summary>
			/// <param name="a"> the scalar by which {@code x} is to be multiplied </param>
			/// <param name="x"> the vector to be added to {@code y} </param>
			/// <param name="y"> the vector to be incremented </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void daxpy(final double a, final RealVector x, final RealVector y)
			internal static void daxpy(double a, RealVector x, RealVector y)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = x.getDimension();
				int n = x.Dimension;
				for (int i = 0; i < n; i++)
				{
					y.setEntry(i, a * x.getEntry(i) + y.getEntry(i));
				}
			}

			/// <summary>
			/// A BLAS-like function, for the operation z &larr; a &middot; x + b
			/// &middot; y + z. This is for internal use only: no dimension checks are
			/// provided.
			/// </summary>
			/// <param name="a"> the scalar by which {@code x} is to be multiplied </param>
			/// <param name="x"> the first vector to be added to {@code z} </param>
			/// <param name="b"> the scalar by which {@code y} is to be multiplied </param>
			/// <param name="y"> the second vector to be added to {@code z} </param>
			/// <param name="z"> the vector to be incremented </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void daxpbypz(final double a, final RealVector x, final double b, final RealVector y, final RealVector z)
			internal static void daxpbypz(double a, RealVector x, double b, RealVector y, RealVector z)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = z.getDimension();
				int n = z.Dimension;
				for (int i = 0; i < n; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double zi;
					double zi;
					zi = a * x.getEntry(i) + b * y.getEntry(i) + z.getEntry(i);
					z.setEntry(i, zi);
				}
			}

			/// <summary>
			/// <p>
			/// Move to the CG point if it seems better. In this version of SYMMLQ,
			/// the convergence tests involve only cgnorm, so we're unlikely to stop
			/// at an LQ point, except if the iteration limit interferes.
			/// </p>
			/// <p>
			/// Additional upudates are also carried out in case {@code goodb} is set
			/// to {@code true}.
			/// </p>
			/// </summary>
			/// <param name="x"> the vector to be updated with the refined value of xL </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: void refineSolution(final RealVector x)
			 internal virtual void refineSolution(RealVector x)
			 {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = this.xL.getDimension();
				int n = this.xL.Dimension;
				if (lqnorm < cgnorm)
				{
					if (!goodb)
					{
						x.setSubVector(0, this.xL);
					}
					else
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double step = bstep / beta1;
						double step = bstep / beta1;
						for (int i = 0; i < n; i++)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bi = mb.getEntry(i);
							double bi = mb.getEntry(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xi = this.xL.getEntry(i);
							double xi = this.xL.getEntry(i);
							x.setEntry(i, xi + step * bi);
						}
					}
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double anorm = mathlib.util.FastMath.sqrt(tnorm);
					double anorm = FastMath.sqrt(tnorm);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double diag = gbar == 0.0 ? anorm * MACH_PREC : gbar;
					double diag = gbar == 0.0 ? anorm * MACH_PREC : gbar;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double zbar = gammaZeta / diag;
					double zbar = gammaZeta / diag;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double step = (bstep + snprod * zbar) / beta1;
					double step = (bstep + snprod * zbar) / beta1;
					// ynorm = FastMath.sqrt(ynorm2 + zbar * zbar);
					if (!goodb)
					{
						for (int i = 0; i < n; i++)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xi = this.xL.getEntry(i);
							double xi = this.xL.getEntry(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double wi = wbar.getEntry(i);
							double wi = wbar.getEntry(i);
							x.setEntry(i, xi + zbar * wi);
						}
					}
					else
					{
						for (int i = 0; i < n; i++)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xi = this.xL.getEntry(i);
							double xi = this.xL.getEntry(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double wi = wbar.getEntry(i);
							double wi = wbar.getEntry(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bi = mb.getEntry(i);
							double bi = mb.getEntry(i);
							x.setEntry(i, xi + zbar * wi + step * bi);
						}
					}
				}
			 }

			/// <summary>
			/// Performs the initial phase of the SYMMLQ algorithm. On return, the
			/// value of the state variables of {@code this} object correspond to k =
			/// 1.
			/// </summary>
			 internal virtual void init()
			 {
				this.xL.set(0.0);
				/*
				 * Set up y for the first Lanczos vector. y and beta1 will be zero
				 * if b = 0.
				 */
				this.r1 = this.b.copy();
				this.y = this.m == null ? this.b.copy() : this.m.operate(this.r1);
				if ((this.m != null) && this.check)
				{
					checkSymmetry(this.m, this.r1, this.y, this.m.operate(this.y));
				}

				this.beta1 = this.r1.dotProduct(this.y);
				if (this.beta1 < 0.0)
				{
					throwNPDLOException(this.m, this.y);
				}
				if (this.beta1 == 0.0)
				{
					/* If b = 0 exactly, stop with x = 0. */
					this.bIsNull = true;
					return;
				}
				this.bIsNull = false;
				this.beta1 = FastMath.sqrt(this.beta1);
				/* At this point
				 *   r1 = b,
				 *   y = M * b,
				 *   beta1 = beta[1].
				 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector v = this.y.mapMultiply(1.0 / this.beta1);
				RealVector v = this.y.mapMultiply(1.0 / this.beta1);
				this.y = this.a.operate(v);
				if (this.check)
				{
					checkSymmetry(this.a, v, this.y, this.a.operate(this.y));
				}
				/*
				 * Set up y for the second Lanczos vector. y and beta will be zero
				 * or very small if b is an eigenvector.
				 */
				daxpy(-this.shift, v, this.y);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double alpha = v.dotProduct(this.y);
				double alpha = v.dotProduct(this.y);
				daxpy(-alpha / this.beta1, this.r1, this.y);
				/*
				 * At this point
				 *   alpha = alpha[1]
				 *   y     = beta[2] * M^(-1) * P' * v[2]
				 */
				/* Make sure r2 will be orthogonal to the first v. */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double vty = v.dotProduct(this.y);
				double vty = v.dotProduct(this.y);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double vtv = v.dotProduct(v);
				double vtv = v.dotProduct(v);
				daxpy(-vty / vtv, v, this.y);
				this.r2 = this.y.copy();
				if (this.m != null)
				{
					this.y = this.m.operate(this.r2);
				}
				this.oldb = this.beta1;
				this.beta = this.r2.dotProduct(this.y);
				if (this.beta < 0.0)
				{
					throwNPDLOException(this.m, this.y);
				}
				this.beta = FastMath.sqrt(this.beta);
				/*
				 * At this point
				 *   oldb = beta[1]
				 *   beta = beta[2]
				 *   y  = beta[2] * P' * v[2]
				 *   r2 = beta[2] * M^(-1) * P' * v[2]
				 */
				this.cgnorm = this.beta1;
				this.gbar = alpha;
				this.dbar = this.beta;
				this.gammaZeta = this.beta1;
				this.minusEpsZeta = 0.0;
				this.bstep = 0.0;
				this.snprod = 1.0;
				this.tnorm = alpha * alpha + this.beta * this.beta;
				this.ynorm2 = 0.0;
				this.gmax = FastMath.abs(alpha) + MACH_PREC;
				this.gmin = this.gmax;

				if (this.goodb)
				{
					this.wbar = new ArrayRealVector(this.a.RowDimension);
					this.wbar.set(0.0);
				}
				else
				{
					this.wbar = v;
				}
				updateNorms();
			 }

			/// <summary>
			/// Performs the next iteration of the algorithm. The iteration count
			/// should be incremented prior to calling this method. On return, the
			/// value of the state variables of {@code this} object correspond to the
			/// current iteration count {@code k}.
			/// </summary>
			internal virtual void update()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector v = y.mapMultiply(1.0 / beta);
				RealVector v = y.mapMultiply(1.0 / beta);
				y = a.operate(v);
				daxpbypz(-shift, v, -beta / oldb, r1, y);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double alpha = v.dotProduct(y);
				double alpha = v.dotProduct(y);
				/*
				 * At this point
				 *   v     = P' * v[k],
				 *   y     = (A - shift * I) * P' * v[k] - beta[k] * M^(-1) * P' * v[k-1],
				 *   alpha = v'[k] * P * (A - shift * I) * P' * v[k]
				 *           - beta[k] * v[k]' * P * M^(-1) * P' * v[k-1]
				 *         = v'[k] * P * (A - shift * I) * P' * v[k]
				 *           - beta[k] * v[k]' * v[k-1]
				 *         = alpha[k].
				 */
				daxpy(-alpha / beta, r2, y);
				/*
				 * At this point
				 *   y = (A - shift * I) * P' * v[k] - alpha[k] * M^(-1) * P' * v[k]
				 *       - beta[k] * M^(-1) * P' * v[k-1]
				 *     = M^(-1) * P' * (P * (A - shift * I) * P' * v[k] -alpha[k] * v[k]
				 *       - beta[k] * v[k-1])
				 *     = beta[k+1] * M^(-1) * P' * v[k+1],
				 * from Paige and Saunders (1975), equation (3.2).
				 *
				 * WATCH-IT: the two following lines work only because y is no longer
				 * updated up to the end of the present iteration, and is
				 * reinitialized at the beginning of the next iteration.
				 */
				r1 = r2;
				r2 = y;
				if (m != null)
				{
					y = m.operate(r2);
				}
				oldb = beta;
				beta = r2.dotProduct(y);
				if (beta < 0.0)
				{
					throwNPDLOException(m, y);
				}
				beta = FastMath.sqrt(beta);
				/*
				 * At this point
				 *   r1 = beta[k] * M^(-1) * P' * v[k],
				 *   r2 = beta[k+1] * M^(-1) * P' * v[k+1],
				 *   y  = beta[k+1] * P' * v[k+1],
				 *   oldb = beta[k],
				 *   beta = beta[k+1].
				 */
				tnorm += alpha * alpha + oldb * oldb + beta * beta;
				/*
				 * Compute the next plane rotation for Q. See Paige and Saunders
				 * (1975), equation (5.6), with
				 *   gamma = gamma[k-1],
				 *   c     = c[k-1],
				 *   s     = s[k-1].
				 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double gamma = mathlib.util.FastMath.sqrt(gbar * gbar + oldb * oldb);
				double gamma = FastMath.sqrt(gbar * gbar + oldb * oldb);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c = gbar / gamma;
				double c = gbar / gamma;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s = oldb / gamma;
				double s = oldb / gamma;
				/*
				 * The relations
				 *   gbar[k] = s[k-1] * (-c[k-2] * beta[k]) - c[k-1] * alpha[k]
				 *           = s[k-1] * dbar[k] - c[k-1] * alpha[k],
				 *   delta[k] = c[k-1] * dbar[k] + s[k-1] * alpha[k],
				 * are not stated in Paige and Saunders (1975), but can be retrieved
				 * by expanding the (k, k-1) and (k, k) coefficients of the matrix in
				 * equation (5.5).
				 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double deltak = c * dbar + s * alpha;
				double deltak = c * dbar + s * alpha;
				gbar = s * dbar - c * alpha;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double eps = s * beta;
				double eps = s * beta;
				dbar = -c * beta;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double zeta = gammaZeta / gamma;
				double zeta = gammaZeta / gamma;
				/*
				 * At this point
				 *   gbar   = gbar[k]
				 *   deltak = delta[k]
				 *   eps    = eps[k+1]
				 *   dbar   = dbar[k+1]
				 *   zeta   = zeta[k-1]
				 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double zetaC = zeta * c;
				double zetaC = zeta * c;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double zetaS = zeta * s;
				double zetaS = zeta * s;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = xL.getDimension();
				int n = xL.Dimension;
				for (int i = 0; i < n; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xi = xL.getEntry(i);
					double xi = xL.getEntry(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double vi = v.getEntry(i);
					double vi = v.getEntry(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double wi = wbar.getEntry(i);
					double wi = wbar.getEntry(i);
					xL.setEntry(i, xi + wi * zetaC + vi * zetaS);
					wbar.setEntry(i, wi * s - vi * c);
				}
				/*
				 * At this point
				 *   x = xL[k-1],
				 *   ptwbar = P' wbar[k],
				 * see Paige and Saunders (1975), equations (5.9) and (5.10).
				 */
				bstep += snprod * c * zeta;
				snprod *= s;
				gmax = FastMath.max(gmax, gamma);
				gmin = FastMath.min(gmin, gamma);
				ynorm2 += zeta * zeta;
				gammaZeta = minusEpsZeta - deltak * zeta;
				minusEpsZeta = -eps * zeta;
				/*
				 * At this point
				 *   snprod       = s[1] * ... * s[k-1],
				 *   gmax         = max(|alpha[1]|, gamma[1], ..., gamma[k-1]),
				 *   gmin         = min(|alpha[1]|, gamma[1], ..., gamma[k-1]),
				 *   ynorm2       = zeta[1]^2 + ... + zeta[k-1]^2,
				 *   gammaZeta    = gamma[k] * zeta[k],
				 *   minusEpsZeta = -eps[k+1] * zeta[k-1].
				 * The relation for gammaZeta can be retrieved from Paige and
				 * Saunders (1975), equation (5.4a), last line of the vector
				 * gbar[k] * zbar[k] = -eps[k] * zeta[k-2] - delta[k] * zeta[k-1].
				 */
				updateNorms();
			}

			/// <summary>
			/// Computes the norms of the residuals, and checks for convergence.
			/// Updates <seealso cref="#lqnorm"/> and <seealso cref="#cgnorm"/>.
			/// </summary>
			internal virtual void updateNorms()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double anorm = mathlib.util.FastMath.sqrt(tnorm);
				double anorm = FastMath.sqrt(tnorm);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ynorm = mathlib.util.FastMath.sqrt(ynorm2);
				double ynorm = FastMath.sqrt(ynorm2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double epsa = anorm * MACH_PREC;
				double epsa = anorm * MACH_PREC;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double epsx = anorm * ynorm * MACH_PREC;
				double epsx = anorm * ynorm * MACH_PREC;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double epsr = anorm * ynorm * delta;
				double epsr = anorm * ynorm * delta;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double diag = gbar == 0.0 ? epsa : gbar;
				double diag = gbar == 0.0 ? epsa : gbar;
				lqnorm = FastMath.sqrt(gammaZeta * gammaZeta + minusEpsZeta * minusEpsZeta);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double qrnorm = snprod * beta1;
				double qrnorm = snprod * beta1;
				cgnorm = qrnorm * beta / FastMath.abs(diag);

				/*
				 * Estimate cond(A). In this version we look at the diagonals of L
				 * in the factorization of the tridiagonal matrix, T = L * Q.
				 * Sometimes, T[k] can be misleadingly ill-conditioned when T[k+1]
				 * is not, so we must be careful not to overestimate acond.
				 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double acond;
				double acond;
				if (lqnorm <= cgnorm)
				{
					acond = gmax / gmin;
				}
				else
				{
					acond = gmax / FastMath.min(gmin, FastMath.abs(diag));
				}
				if (acond * MACH_PREC >= 0.1)
				{
					throw new IllConditionedOperatorException(acond);
				}
				if (beta1 <= epsx)
				{
					/*
					 * x has converged to an eigenvector of A corresponding to the
					 * eigenvalue shift.
					 */
					throw new SingularOperatorException();
				}
				rnorm = FastMath.min(cgnorm, lqnorm);
				hasConverged_Renamed = (cgnorm <= epsx) || (cgnorm <= epsr);
			}

			/// <summary>
			/// Returns {@code true} if the default stopping criterion is fulfilled.
			/// </summary>
			/// <returns> {@code true} if convergence of the iterations has occurred </returns>
			internal virtual bool hasConverged()
			{
				return hasConverged_Renamed;
			}

			/// <summary>
			/// Returns {@code true} if the right-hand side vector is zero exactly.
			/// </summary>
			/// <returns> the boolean value of {@code b == 0} </returns>
			internal virtual bool bEqualsNullVector()
			{
				return bIsNull;
			}

			/// <summary>
			/// Returns {@code true} if {@code beta} is essentially zero. This method
			/// is used to check for early stop of the iterations.
			/// </summary>
			/// <returns> {@code true} if {@code beta < }<seealso cref="#MACH_PREC"/> </returns>
			internal virtual bool betaEqualsZero()
			{
				return beta < MACH_PREC;
			}

			/// <summary>
			/// Returns the norm of the updated, preconditioned residual.
			/// </summary>
			/// <returns> the norm of the residual, ||P * r|| </returns>
			internal virtual double NormOfResidual
			{
				get
				{
					return rnorm;
				}
			}
		}

		/// <summary>
		/// Key for the exception context. </summary>
		private const string OPERATOR = "operator";

		/// <summary>
		/// Key for the exception context. </summary>
		private const string THRESHOLD = "threshold";

		/// <summary>
		/// Key for the exception context. </summary>
		private const string VECTOR = "vector";

		/// <summary>
		/// Key for the exception context. </summary>
		private const string VECTOR1 = "vector1";

		/// <summary>
		/// Key for the exception context. </summary>
		private const string VECTOR2 = "vector2";

		/// <summary>
		/// {@code true} if symmetry of matrix and conditioner must be checked. </summary>
		private readonly bool check;

		/// <summary>
		/// The value of the custom tolerance &delta; for the default stopping
		/// criterion.
		/// </summary>
		private readonly double delta;

		/// <summary>
		/// Creates a new instance of this class, with <a href="#stopcrit">default
		/// stopping criterion</a>. Note that setting {@code check} to {@code true}
		/// entails an extra matrix-vector product in the initial phase.
		/// </summary>
		/// <param name="maxIterations"> the maximum number of iterations </param>
		/// <param name="delta"> the &delta; parameter for the default stopping criterion </param>
		/// <param name="check"> {@code true} if self-adjointedness of both matrix and
		/// preconditioner should be checked </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SymmLQ(final int maxIterations, final double delta, final boolean check)
		public SymmLQ(int maxIterations, double delta, bool check) : base(maxIterations)
		{
			this.delta = delta;
			this.check = check;
		}

		/// <summary>
		/// Creates a new instance of this class, with <a href="#stopcrit">default
		/// stopping criterion</a> and custom iteration manager. Note that setting
		/// {@code check} to {@code true} entails an extra matrix-vector product in
		/// the initial phase.
		/// </summary>
		/// <param name="manager"> the custom iteration manager </param>
		/// <param name="delta"> the &delta; parameter for the default stopping criterion </param>
		/// <param name="check"> {@code true} if self-adjointedness of both matrix and
		/// preconditioner should be checked </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SymmLQ(final mathlib.util.IterationManager manager, final double delta, final boolean check)
		public SymmLQ(IterationManager manager, double delta, bool check) : base(manager)
		{
			this.delta = delta;
			this.check = check;
		}

		/// <summary>
		/// Returns {@code true} if symmetry of the matrix, and symmetry as well as
		/// positive definiteness of the preconditioner should be checked.
		/// </summary>
		/// <returns> {@code true} if the tests are to be performed </returns>
		public bool Check
		{
			get
			{
				return check;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="NonSelfAdjointOperatorException"> if <seealso cref="#getCheck()"/> is
		/// {@code true}, and {@code a} or {@code m} is not self-adjoint </exception>
		/// <exception cref="NonPositiveDefiniteOperatorException"> if {@code m} is not
		/// positive definite </exception>
		/// <exception cref="IllConditionedOperatorException"> if {@code a} is ill-conditioned </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector solve(final RealLinearOperator a, final RealLinearOperator m, final RealVector b) throws mathlib.exception.NullArgumentException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException, mathlib.exception.MaxCountExceededException, NonSelfAdjointOperatorException, NonPositiveDefiniteOperatorException, IllConditionedOperatorException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealVector solve(RealLinearOperator a, RealLinearOperator m, RealVector b)
		{
			MathUtils.checkNotNull(a);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector x = new ArrayRealVector(a.getColumnDimension());
			RealVector x = new ArrayRealVector(a.ColumnDimension);
			return solveInPlace(a, m, b, x, false, 0.0);
		}

		/// <summary>
		/// Returns an estimate of the solution to the linear system (A - shift
		/// &middot; I) &middot; x = b.
		/// <p>
		/// If the solution x is expected to contain a large multiple of {@code b}
		/// (as in Rayleigh-quotient iteration), then better precision may be
		/// achieved with {@code goodb} set to {@code true}; this however requires an
		/// extra call to the preconditioner.
		/// </p>
		/// <p>
		/// {@code shift} should be zero if the system A &middot; x = b is to be
		/// solved. Otherwise, it could be an approximation to an eigenvalue of A,
		/// such as the Rayleigh quotient b<sup>T</sup> &middot; A &middot; b /
		/// (b<sup>T</sup> &middot; b) corresponding to the vector b. If b is
		/// sufficiently like an eigenvector corresponding to an eigenvalue near
		/// shift, then the computed x may have very large components. When
		/// normalized, x may be closer to an eigenvector than b.
		/// </p>
		/// </summary>
		/// <param name="a"> the linear operator A of the system </param>
		/// <param name="m"> the preconditioner, M (can be {@code null}) </param>
		/// <param name="b"> the right-hand side vector </param>
		/// <param name="goodb"> usually {@code false}, except if {@code x} is expected to
		/// contain a large multiple of {@code b} </param>
		/// <param name="shift"> the amount to be subtracted to all diagonal elements of A </param>
		/// <returns> a reference to {@code x} (shallow copy) </returns>
		/// <exception cref="NullArgumentException"> if one of the parameters is {@code null} </exception>
		/// <exception cref="NonSquareOperatorException"> if {@code a} or {@code m} is not square </exception>
		/// <exception cref="DimensionMismatchException"> if {@code m} or {@code b} have dimensions
		/// inconsistent with {@code a} </exception>
		/// <exception cref="MaxCountExceededException"> at exhaustion of the iteration count,
		/// unless a custom
		/// <seealso cref="mathlib.util.Incrementor.MaxCountExceededCallback callback"/>
		/// has been set at construction of the <seealso cref="IterationManager"/> </exception>
		/// <exception cref="NonSelfAdjointOperatorException"> if <seealso cref="#getCheck()"/> is
		/// {@code true}, and {@code a} or {@code m} is not self-adjoint </exception>
		/// <exception cref="NonPositiveDefiniteOperatorException"> if {@code m} is not
		/// positive definite </exception>
		/// <exception cref="IllConditionedOperatorException"> if {@code a} is ill-conditioned </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector solve(final RealLinearOperator a, final RealLinearOperator m, final RealVector b, final boolean goodb, final double shift) throws mathlib.exception.NullArgumentException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException, mathlib.exception.MaxCountExceededException, NonSelfAdjointOperatorException, NonPositiveDefiniteOperatorException, IllConditionedOperatorException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealVector solve(RealLinearOperator a, RealLinearOperator m, RealVector b, bool goodb, double shift)
		{
			MathUtils.checkNotNull(a);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector x = new ArrayRealVector(a.getColumnDimension());
			RealVector x = new ArrayRealVector(a.ColumnDimension);
			return solveInPlace(a, m, b, x, goodb, shift);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <param name="x"> not meaningful in this implementation; should not be considered
		/// as an initial guess (<a href="#initguess">more</a>) </param>
		/// <exception cref="NonSelfAdjointOperatorException"> if <seealso cref="#getCheck()"/> is
		/// {@code true}, and {@code a} or {@code m} is not self-adjoint </exception>
		/// <exception cref="NonPositiveDefiniteOperatorException"> if {@code m} is not positive
		/// definite </exception>
		/// <exception cref="IllConditionedOperatorException"> if {@code a} is ill-conditioned </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector solve(final RealLinearOperator a, final RealLinearOperator m, final RealVector b, final RealVector x) throws mathlib.exception.NullArgumentException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException, NonSelfAdjointOperatorException, NonPositiveDefiniteOperatorException, IllConditionedOperatorException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealVector solve(RealLinearOperator a, RealLinearOperator m, RealVector b, RealVector x)
		{
			MathUtils.checkNotNull(x);
			return solveInPlace(a, m, b, x.copy(), false, 0.0);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="NonSelfAdjointOperatorException"> if <seealso cref="#getCheck()"/> is
		/// {@code true}, and {@code a} is not self-adjoint </exception>
		/// <exception cref="IllConditionedOperatorException"> if {@code a} is ill-conditioned </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector solve(final RealLinearOperator a, final RealVector b) throws mathlib.exception.NullArgumentException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException, NonSelfAdjointOperatorException, IllConditionedOperatorException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealVector solve(RealLinearOperator a, RealVector b)
		{
			MathUtils.checkNotNull(a);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector x = new ArrayRealVector(a.getColumnDimension());
			RealVector x = new ArrayRealVector(a.ColumnDimension);
			x.set(0.0);
			return solveInPlace(a, null, b, x, false, 0.0);
		}

		/// <summary>
		/// Returns the solution to the system (A - shift &middot; I) &middot; x = b.
		/// <p>
		/// If the solution x is expected to contain a large multiple of {@code b}
		/// (as in Rayleigh-quotient iteration), then better precision may be
		/// achieved with {@code goodb} set to {@code true}.
		/// </p>
		/// <p>
		/// {@code shift} should be zero if the system A &middot; x = b is to be
		/// solved. Otherwise, it could be an approximation to an eigenvalue of A,
		/// such as the Rayleigh quotient b<sup>T</sup> &middot; A &middot; b /
		/// (b<sup>T</sup> &middot; b) corresponding to the vector b. If b is
		/// sufficiently like an eigenvector corresponding to an eigenvalue near
		/// shift, then the computed x may have very large components. When
		/// normalized, x may be closer to an eigenvector than b.
		/// </p>
		/// </summary>
		/// <param name="a"> the linear operator A of the system </param>
		/// <param name="b"> the right-hand side vector </param>
		/// <param name="goodb"> usually {@code false}, except if {@code x} is expected to
		/// contain a large multiple of {@code b} </param>
		/// <param name="shift"> the amount to be subtracted to all diagonal elements of A </param>
		/// <returns> a reference to {@code x} </returns>
		/// <exception cref="NullArgumentException"> if one of the parameters is {@code null} </exception>
		/// <exception cref="NonSquareOperatorException"> if {@code a} is not square </exception>
		/// <exception cref="DimensionMismatchException"> if {@code b} has dimensions
		/// inconsistent with {@code a} </exception>
		/// <exception cref="MaxCountExceededException"> at exhaustion of the iteration count,
		/// unless a custom
		/// <seealso cref="mathlib.util.Incrementor.MaxCountExceededCallback callback"/>
		/// has been set at construction of the <seealso cref="IterationManager"/> </exception>
		/// <exception cref="NonSelfAdjointOperatorException"> if <seealso cref="#getCheck()"/> is
		/// {@code true}, and {@code a} is not self-adjoint </exception>
		/// <exception cref="IllConditionedOperatorException"> if {@code a} is ill-conditioned </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector solve(final RealLinearOperator a, final RealVector b, final boolean goodb, final double shift) throws mathlib.exception.NullArgumentException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException, NonSelfAdjointOperatorException, IllConditionedOperatorException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealVector solve(RealLinearOperator a, RealVector b, bool goodb, double shift)
		{
			MathUtils.checkNotNull(a);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector x = new ArrayRealVector(a.getColumnDimension());
			RealVector x = new ArrayRealVector(a.ColumnDimension);
			return solveInPlace(a, null, b, x, goodb, shift);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <param name="x"> not meaningful in this implementation; should not be considered
		/// as an initial guess (<a href="#initguess">more</a>) </param>
		/// <exception cref="NonSelfAdjointOperatorException"> if <seealso cref="#getCheck()"/> is
		/// {@code true}, and {@code a} is not self-adjoint </exception>
		/// <exception cref="IllConditionedOperatorException"> if {@code a} is ill-conditioned </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector solve(final RealLinearOperator a, final RealVector b, final RealVector x) throws mathlib.exception.NullArgumentException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException, NonSelfAdjointOperatorException, IllConditionedOperatorException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealVector solve(RealLinearOperator a, RealVector b, RealVector x)
		{
			MathUtils.checkNotNull(x);
			return solveInPlace(a, null, b, x.copy(), false, 0.0);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <param name="x"> the vector to be updated with the solution; {@code x} should
		/// not be considered as an initial guess (<a href="#initguess">more</a>) </param>
		/// <exception cref="NonSelfAdjointOperatorException"> if <seealso cref="#getCheck()"/> is
		/// {@code true}, and {@code a} or {@code m} is not self-adjoint </exception>
		/// <exception cref="NonPositiveDefiniteOperatorException"> if {@code m} is not
		/// positive definite </exception>
		/// <exception cref="IllConditionedOperatorException"> if {@code a} is ill-conditioned </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector solveInPlace(final RealLinearOperator a, final RealLinearOperator m, final RealVector b, final RealVector x) throws mathlib.exception.NullArgumentException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException, NonSelfAdjointOperatorException, NonPositiveDefiniteOperatorException, IllConditionedOperatorException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealVector solveInPlace(RealLinearOperator a, RealLinearOperator m, RealVector b, RealVector x)
		{
			return solveInPlace(a, m, b, x, false, 0.0);
		}

		/// <summary>
		/// Returns an estimate of the solution to the linear system (A - shift
		/// &middot; I) &middot; x = b. The solution is computed in-place.
		/// <p>
		/// If the solution x is expected to contain a large multiple of {@code b}
		/// (as in Rayleigh-quotient iteration), then better precision may be
		/// achieved with {@code goodb} set to {@code true}; this however requires an
		/// extra call to the preconditioner.
		/// </p>
		/// <p>
		/// {@code shift} should be zero if the system A &middot; x = b is to be
		/// solved. Otherwise, it could be an approximation to an eigenvalue of A,
		/// such as the Rayleigh quotient b<sup>T</sup> &middot; A &middot; b /
		/// (b<sup>T</sup> &middot; b) corresponding to the vector b. If b is
		/// sufficiently like an eigenvector corresponding to an eigenvalue near
		/// shift, then the computed x may have very large components. When
		/// normalized, x may be closer to an eigenvector than b.
		/// </p>
		/// </summary>
		/// <param name="a"> the linear operator A of the system </param>
		/// <param name="m"> the preconditioner, M (can be {@code null}) </param>
		/// <param name="b"> the right-hand side vector </param>
		/// <param name="x"> the vector to be updated with the solution; {@code x} should
		/// not be considered as an initial guess (<a href="#initguess">more</a>) </param>
		/// <param name="goodb"> usually {@code false}, except if {@code x} is expected to
		/// contain a large multiple of {@code b} </param>
		/// <param name="shift"> the amount to be subtracted to all diagonal elements of A </param>
		/// <returns> a reference to {@code x} (shallow copy). </returns>
		/// <exception cref="NullArgumentException"> if one of the parameters is {@code null} </exception>
		/// <exception cref="NonSquareOperatorException"> if {@code a} or {@code m} is not square </exception>
		/// <exception cref="DimensionMismatchException"> if {@code m}, {@code b} or {@code x}
		/// have dimensions inconsistent with {@code a}. </exception>
		/// <exception cref="MaxCountExceededException"> at exhaustion of the iteration count,
		/// unless a custom
		/// <seealso cref="mathlib.util.Incrementor.MaxCountExceededCallback callback"/>
		/// has been set at construction of the <seealso cref="IterationManager"/> </exception>
		/// <exception cref="NonSelfAdjointOperatorException"> if <seealso cref="#getCheck()"/> is
		/// {@code true}, and {@code a} or {@code m} is not self-adjoint </exception>
		/// <exception cref="NonPositiveDefiniteOperatorException"> if {@code m} is not positive
		/// definite </exception>
		/// <exception cref="IllConditionedOperatorException"> if {@code a} is ill-conditioned </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector solveInPlace(final RealLinearOperator a, final RealLinearOperator m, final RealVector b, final RealVector x, final boolean goodb, final double shift) throws mathlib.exception.NullArgumentException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException, NonSelfAdjointOperatorException, NonPositiveDefiniteOperatorException, IllConditionedOperatorException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealVector solveInPlace(RealLinearOperator a, RealLinearOperator m, RealVector b, RealVector x, bool goodb, double shift)
		{
			checkParameters(a, m, b, x);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.util.IterationManager manager = getIterationManager();
			IterationManager manager = IterationManager;
			/* Initialization counts as an iteration. */
			manager.resetIterationCount();
			manager.incrementIterationCount();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final State state;
			State state;
			state = new State(a, m, b, goodb, shift, delta, check);
			state.init();
			state.refineSolution(x);
			IterativeLinearSolverEvent @event;
			@event = new DefaultIterativeLinearSolverEvent(this, manager.Iterations, x, b, state.NormOfResidual);
			if (state.bEqualsNullVector())
			{
				/* If b = 0 exactly, stop with x = 0. */
				manager.fireTerminationEvent(@event);
				return x;
			}
			/* Cause termination if beta is essentially zero. */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean earlyStop;
			bool earlyStop;
			earlyStop = state.betaEqualsZero() || state.hasConverged();
			manager.fireInitializationEvent(@event);
			if (!earlyStop)
			{
				do
				{
					manager.incrementIterationCount();
					@event = new DefaultIterativeLinearSolverEvent(this, manager.Iterations, x, b, state.NormOfResidual);
					manager.fireIterationStartedEvent(@event);
					state.update();
					state.refineSolution(x);
					@event = new DefaultIterativeLinearSolverEvent(this, manager.Iterations, x, b, state.NormOfResidual);
					manager.fireIterationPerformedEvent(@event);
				} while (!state.hasConverged());
			}
			@event = new DefaultIterativeLinearSolverEvent(this, manager.Iterations, x, b, state.NormOfResidual);
			manager.fireTerminationEvent(@event);
			return x;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <param name="x"> the vector to be updated with the solution; {@code x} should
		/// not be considered as an initial guess (<a href="#initguess">more</a>) </param>
		/// <exception cref="NonSelfAdjointOperatorException"> if <seealso cref="#getCheck()"/> is
		/// {@code true}, and {@code a} is not self-adjoint </exception>
		/// <exception cref="IllConditionedOperatorException"> if {@code a} is ill-conditioned </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector solveInPlace(final RealLinearOperator a, final RealVector b, final RealVector x) throws mathlib.exception.NullArgumentException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException, NonSelfAdjointOperatorException, IllConditionedOperatorException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealVector solveInPlace(RealLinearOperator a, RealVector b, RealVector x)
		{
			return solveInPlace(a, null, b, x, false, 0.0);
		}
	}

}
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
	using IterationManager = mathlib.util.IterationManager;

	/// <summary>
	/// <p>
	/// This is an implementation of the conjugate gradient method for
	/// <seealso cref="RealLinearOperator"/>. It follows closely the template by <a
	/// href="#BARR1994">Barrett et al. (1994)</a> (figure 2.5). The linear system at
	/// hand is A &middot; x = b, and the residual is r = b - A &middot; x.
	/// </p>
	/// <h3><a id="stopcrit">Default stopping criterion</a></h3>
	/// <p>
	/// A default stopping criterion is implemented. The iterations stop when || r ||
	/// &le; &delta; || b ||, where b is the right-hand side vector, r the current
	/// estimate of the residual, and &delta; a user-specified tolerance. It should
	/// be noted that r is the so-called <em>updated</em> residual, which might
	/// differ from the true residual due to rounding-off errors (see e.g. <a
	/// href="#STRA2002">Strakos and Tichy, 2002</a>).
	/// </p>
	/// <h3>Iteration count</h3>
	/// <p>
	/// In the present context, an iteration should be understood as one evaluation
	/// of the matrix-vector product A &middot; x. The initialization phase therefore
	/// counts as one iteration.
	/// </p>
	/// <h3><a id="context">Exception context</a></h3>
	/// <p>
	/// Besides standard <seealso cref="DimensionMismatchException"/>, this class might throw
	/// <seealso cref="NonPositiveDefiniteOperatorException"/> if the linear operator or
	/// the preconditioner are not positive definite. In this case, the
	/// <seealso cref="ExceptionContext"/> provides some more information
	/// <ul>
	/// <li>key {@code "operator"} points to the offending linear operator, say L,</li>
	/// <li>key {@code "vector"} points to the offending vector, say x, such that
	/// x<sup>T</sup> &middot; L &middot; x < 0.</li>
	/// </ul>
	/// </p>
	/// <h3>References</h3>
	/// <dl>
	/// <dt><a id="BARR1994">Barret et al. (1994)</a></dt>
	/// <dd>R. Barrett, M. Berry, T. F. Chan, J. Demmel, J. M. Donato, J. Dongarra,
	/// V. Eijkhout, R. Pozo, C. Romine and H. Van der Vorst,
	/// <a href="http://www.netlib.org/linalg/html_templates/Templates.html"><em>
	/// Templates for the Solution of Linear Systems: Building Blocks for Iterative
	/// Methods</em></a>, SIAM</dd>
	/// <dt><a id="STRA2002">Strakos and Tichy (2002)
	/// <dt>
	/// <dd>Z. Strakos and P. Tichy, <a
	/// href="http://etna.mcs.kent.edu/vol.13.2002/pp56-80.dir/pp56-80.pdf">
	/// <em>On error estimation in the conjugate gradient method and why it works
	/// in finite precision computations</em></a>, Electronic Transactions on
	/// Numerical Analysis 13: 56-80, 2002</dd>
	/// </dl>
	/// 
	/// @version $Id: ConjugateGradient.java 1562755 2014-01-30 09:37:08Z luc $
	/// @since 3.0
	/// </summary>
	public class ConjugateGradient : PreconditionedIterativeLinearSolver
	{

		/// <summary>
		/// Key for the <a href="#context">exception context</a>. </summary>
		public const string OPERATOR = "operator";

		/// <summary>
		/// Key for the <a href="#context">exception context</a>. </summary>
		public const string VECTOR = "vector";

		/// <summary>
		/// {@code true} if positive-definiteness of matrix and preconditioner should
		/// be checked.
		/// </summary>
		private bool check;

		/// <summary>
		/// The value of &delta;, for the default stopping criterion. </summary>
		private readonly double delta;

		/// <summary>
		/// Creates a new instance of this class, with <a href="#stopcrit">default
		/// stopping criterion</a>.
		/// </summary>
		/// <param name="maxIterations"> the maximum number of iterations </param>
		/// <param name="delta"> the &delta; parameter for the default stopping criterion </param>
		/// <param name="check"> {@code true} if positive definiteness of both matrix and
		/// preconditioner should be checked </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ConjugateGradient(final int maxIterations, final double delta, final boolean check)
		public ConjugateGradient(int maxIterations, double delta, bool check) : base(maxIterations)
		{
			this.delta = delta;
			this.check = check;
		}

		/// <summary>
		/// Creates a new instance of this class, with <a href="#stopcrit">default
		/// stopping criterion</a> and custom iteration manager.
		/// </summary>
		/// <param name="manager"> the custom iteration manager </param>
		/// <param name="delta"> the &delta; parameter for the default stopping criterion </param>
		/// <param name="check"> {@code true} if positive definiteness of both matrix and
		/// preconditioner should be checked </param>
		/// <exception cref="NullArgumentException"> if {@code manager} is {@code null} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ConjugateGradient(final mathlib.util.IterationManager manager, final double delta, final boolean check) throws mathlib.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public ConjugateGradient(IterationManager manager, double delta, bool check) : base(manager)
		{
			this.delta = delta;
			this.check = check;
		}

		/// <summary>
		/// Returns {@code true} if positive-definiteness should be checked for both
		/// matrix and preconditioner.
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
		/// <exception cref="NonPositiveDefiniteOperatorException"> if {@code a} or {@code m} is
		/// not positive definite </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector solveInPlace(final RealLinearOperator a, final RealLinearOperator m, final RealVector b, final RealVector x0) throws mathlib.exception.NullArgumentException, NonPositiveDefiniteOperatorException, NonSquareOperatorException, mathlib.exception.DimensionMismatchException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealVector solveInPlace(RealLinearOperator a, RealLinearOperator m, RealVector b, RealVector x0)
		{
			checkParameters(a, m, b, x0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.util.IterationManager manager = getIterationManager();
			IterationManager manager = IterationManager;
			// Initialization of default stopping criterion
			manager.resetIterationCount();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rmax = delta * b.getNorm();
			double rmax = delta * b.Norm;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector bro = RealVector.unmodifiableRealVector(b);
			RealVector bro = RealVector.unmodifiableRealVector(b);

			// Initialization phase counts as one iteration.
			manager.incrementIterationCount();
			// p and x are constructed as copies of x0, since presumably, the type
			// of x is optimized for the calculation of the matrix-vector product
			// A.x.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector x = x0;
			RealVector x = x0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector xro = RealVector.unmodifiableRealVector(x);
			RealVector xro = RealVector.unmodifiableRealVector(x);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector p = x.copy();
			RealVector p = x.copy();
			RealVector q = a.operate(p);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector r = b.combine(1, -1, q);
			RealVector r = b.combine(1, -1, q);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealVector rro = RealVector.unmodifiableRealVector(r);
			RealVector rro = RealVector.unmodifiableRealVector(r);
			double rnorm = r.Norm;
			RealVector z;
			if (m == null)
			{
				z = r;
			}
			else
			{
				z = null;
			}
			IterativeLinearSolverEvent evt;
			evt = new DefaultIterativeLinearSolverEvent(this, manager.Iterations, xro, bro, rro, rnorm);
			manager.fireInitializationEvent(evt);
			if (rnorm <= rmax)
			{
				manager.fireTerminationEvent(evt);
				return x;
			}
			double rhoPrev = 0.0;
			while (true)
			{
				manager.incrementIterationCount();
				evt = new DefaultIterativeLinearSolverEvent(this, manager.Iterations, xro, bro, rro, rnorm);
				manager.fireIterationStartedEvent(evt);
				if (m != null)
				{
					z = m.operate(r);
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rhoNext = r.dotProduct(z);
				double rhoNext = r.dotProduct(z);
				if (check && (rhoNext <= 0.0))
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final NonPositiveDefiniteOperatorException e;
					NonPositiveDefiniteOperatorException e;
					e = new NonPositiveDefiniteOperatorException();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.exception.util.ExceptionContext context = e.getContext();
					ExceptionContext context = e.Context;
					context.setValue(OPERATOR, m);
					context.setValue(VECTOR, r);
					throw e;
				}
				if (manager.Iterations == 2)
				{
					p.setSubVector(0, z);
				}
				else
				{
					p.combineToSelf(rhoNext / rhoPrev, 1.0, z);
				}
				q = a.operate(p);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double pq = p.dotProduct(q);
				double pq = p.dotProduct(q);
				if (check && (pq <= 0.0))
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final NonPositiveDefiniteOperatorException e;
					NonPositiveDefiniteOperatorException e;
					e = new NonPositiveDefiniteOperatorException();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.exception.util.ExceptionContext context = e.getContext();
					ExceptionContext context = e.Context;
					context.setValue(OPERATOR, a);
					context.setValue(VECTOR, p);
					throw e;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double alpha = rhoNext / pq;
				double alpha = rhoNext / pq;
				x.combineToSelf(1.0, alpha, p);
				r.combineToSelf(1.0, -alpha, q);
				rhoPrev = rhoNext;
				rnorm = r.Norm;
				evt = new DefaultIterativeLinearSolverEvent(this, manager.Iterations, xro, bro, rro, rnorm);
				manager.fireIterationPerformedEvent(evt);
				if (rnorm <= rmax)
				{
					manager.fireTerminationEvent(evt);
					return x;
				}
			}
		}
	}

}
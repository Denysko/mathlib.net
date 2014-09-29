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

	using IterationEvent = mathlib.util.IterationEvent;
	using MathUnsupportedOperationException = mathlib.exception.MathUnsupportedOperationException;

	/// <summary>
	/// This is the base class for all events occurring during the iterations of a
	/// <seealso cref="IterativeLinearSolver"/>.
	/// 
	/// @version $Id: IterativeLinearSolverEvent.java 1505934 2013-07-23 08:38:03Z luc $
	/// @since 3.0
	/// </summary>
	public abstract class IterativeLinearSolverEvent : IterationEvent
	{
		/// <summary>
		/// Serialization identifier. </summary>
		private const long serialVersionUID = 20120129L;

		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="source"> the iterative algorithm on which the event initially
		/// occurred </param>
		/// <param name="iterations"> the number of iterations performed at the time
		/// {@code this} event is created </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public IterativeLinearSolverEvent(final Object source, final int iterations)
		public IterativeLinearSolverEvent(object source, int iterations) : base(source, iterations)
		{
		}

		/// <summary>
		/// Returns the current right-hand side of the linear system to be solved.
		/// This method should return an unmodifiable view, or a deep copy of the
		/// actual right-hand side vector, in order not to compromise subsequent
		/// iterations of the source <seealso cref="IterativeLinearSolver"/>.
		/// </summary>
		/// <returns> the right-hand side vector, b </returns>
		public abstract RealVector RightHandSideVector {get;}

		/// <summary>
		/// Returns the norm of the residual. The returned value is not required to
		/// be <em>exact</em>. Instead, the norm of the so-called <em>updated</em>
		/// residual (if available) should be returned. For example, the
		/// <seealso cref="ConjugateGradient conjugate gradient"/> method computes a sequence
		/// of residuals, the norm of which is cheap to compute. However, due to
		/// accumulation of round-off errors, this residual might differ from the
		/// true residual after some iterations. See e.g. A. Greenbaum and
		/// Z. Strakos, <em>Predicting the Behavior of Finite Precision Lanzos and
		/// Conjugate Gradient Computations</em>, Technical Report 538, Department of
		/// Computer Science, New York University, 1991 (available
		/// <a href="http://www.archive.org/details/predictingbehavi00gree">here</a>).
		/// </summary>
		/// <returns> the norm of the residual, ||r|| </returns>
		public abstract double NormOfResidual {get;}

		/// <summary>
		/// <p>
		/// Returns the residual. This is an optional operation, as all iterative
		/// linear solvers do not provide cheap estimate of the updated residual
		/// vector, in which case
		/// </p>
		/// <ul>
		/// <li>this method should throw a
		/// <seealso cref="MathUnsupportedOperationException"/>,</li>
		/// <li><seealso cref="#providesResidual()"/> returns {@code false}.</li>
		/// </ul>
		/// <p>
		/// The default implementation throws a
		/// <seealso cref="MathUnsupportedOperationException"/>. If this method is overriden,
		/// then <seealso cref="#providesResidual()"/> should be overriden as well.
		/// </p>
		/// </summary>
		/// <returns> the updated residual, r </returns>
		public virtual RealVector Residual
		{
			get
			{
				throw new MathUnsupportedOperationException();
			}
		}

		/// <summary>
		/// Returns the current estimate of the solution to the linear system to be
		/// solved. This method should return an unmodifiable view, or a deep copy of
		/// the actual current solution, in order not to compromise subsequent
		/// iterations of the source <seealso cref="IterativeLinearSolver"/>.
		/// </summary>
		/// <returns> the solution, x </returns>
		public abstract RealVector Solution {get;}

		/// <summary>
		/// Returns {@code true} if <seealso cref="#getResidual()"/> is supported. The default
		/// implementation returns {@code false}.
		/// </summary>
		/// <returns> {@code false} if <seealso cref="#getResidual()"/> throws a
		/// <seealso cref="MathUnsupportedOperationException"/> </returns>
		public virtual bool providesResidual()
		{
			return false;
		}
	}

}
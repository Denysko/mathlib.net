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
namespace org.apache.commons.math3.optim
{

	using TooManyEvaluationsException = org.apache.commons.math3.exception.TooManyEvaluationsException;
	using TooManyIterationsException = org.apache.commons.math3.exception.TooManyIterationsException;
	using Incrementor = org.apache.commons.math3.util.Incrementor;

	/// <summary>
	/// Base class for implementing optimization problems. It contains the boiler-plate code
	/// for counting the number of evaluations of the objective function and the number of
	/// iterations of the algorithm, and storing the convergence checker.
	/// </summary>
	/// @param <PAIR> Type of the point/value pair returned by the optimization algorithm.
	/// @version $Id: AbstractOptimizationProblem.java 1571015 2014-02-23 14:00:48Z luc $
	/// @since 3.3 </param>
	public abstract class AbstractOptimizationProblem<PAIR> : OptimizationProblem<PAIR>
	{

		/// <summary>
		/// Callback to use for the evaluation counter. </summary>
		private static readonly MaxEvalCallback MAX_EVAL_CALLBACK = new MaxEvalCallback();
		/// <summary>
		/// Callback to use for the iteration counter. </summary>
		private static readonly MaxIterCallback MAX_ITER_CALLBACK = new MaxIterCallback();

		/// <summary>
		/// max evaluations </summary>
		private readonly int maxEvaluations;
		/// <summary>
		/// max iterations </summary>
		private readonly int maxIterations;
		/// <summary>
		/// Convergence checker. </summary>
		private readonly ConvergenceChecker<PAIR> checker;

		/// <summary>
		/// Create an <seealso cref="AbstractOptimizationProblem"/> from the given data.
		/// </summary>
		/// <param name="maxEvaluations"> the number of allowed model function evaluations. </param>
		/// <param name="maxIterations">  the number of allowed iterations. </param>
		/// <param name="checker">        the convergence checker. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractOptimizationProblem(final int maxEvaluations, final int maxIterations, final ConvergenceChecker<PAIR> checker)
		protected internal AbstractOptimizationProblem(int maxEvaluations, int maxIterations, ConvergenceChecker<PAIR> checker)
		{
			this.maxEvaluations = maxEvaluations;
			this.maxIterations = maxIterations;
			this.checker = checker;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Incrementor EvaluationCounter
		{
			get
			{
				return new Incrementor(this.maxEvaluations, MAX_EVAL_CALLBACK);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Incrementor IterationCounter
		{
			get
			{
				return new Incrementor(this.maxIterations, MAX_ITER_CALLBACK);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual ConvergenceChecker<PAIR> ConvergenceChecker
		{
			get
			{
				return checker;
			}
		}

		/// <summary>
		/// Defines the action to perform when reaching the maximum number of evaluations. </summary>
		private class MaxEvalCallback : Incrementor.MaxCountExceededCallback
		{
			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			/// <exception cref="TooManyEvaluationsException"> </exception>
			public virtual void trigger(int max)
			{
				throw new TooManyEvaluationsException(max);
			}
		}

		/// <summary>
		/// Defines the action to perform when reaching the maximum number of evaluations. </summary>
		private class MaxIterCallback : Incrementor.MaxCountExceededCallback
		{
			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			/// <exception cref="TooManyIterationsException"> </exception>
			public virtual void trigger(int max)
			{
				throw new TooManyIterationsException(max);
			}
		}

	}

}
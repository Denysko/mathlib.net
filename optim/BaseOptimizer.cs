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
namespace mathlib.optim
{

	using Incrementor = mathlib.util.Incrementor;
	using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;
	using TooManyIterationsException = mathlib.exception.TooManyIterationsException;

	/// <summary>
	/// Base class for implementing optimizers.
	/// It contains the boiler-plate code for counting the number of evaluations
	/// of the objective function and the number of iterations of the algorithm,
	/// and storing the convergence checker.
	/// <em>It is not a "user" class.</em>
	/// </summary>
	/// @param <PAIR> Type of the point/value pair returned by the optimization
	/// algorithm.
	/// 
	/// @version $Id: BaseOptimizer.java 1542541 2013-11-16 17:48:36Z tn $
	/// @since 3.1 </param>
	public abstract class BaseOptimizer<PAIR>
	{
		/// <summary>
		/// Evaluations counter. </summary>
		protected internal readonly Incrementor evaluations;
		/// <summary>
		/// Iterations counter. </summary>
		protected internal readonly Incrementor iterations;
		/// <summary>
		/// Convergence checker. </summary>
		private readonly ConvergenceChecker<PAIR> checker;

		/// <param name="checker"> Convergence checker. </param>
		protected internal BaseOptimizer(ConvergenceChecker<PAIR> checker) : this(checker, 0, int.MaxValue)
		{
		}

		/// <param name="checker"> Convergence checker. </param>
		/// <param name="maxEval"> Maximum number of objective function evaluations. </param>
		/// <param name="maxIter"> Maximum number of algorithm iterations. </param>
		protected internal BaseOptimizer(ConvergenceChecker<PAIR> checker, int maxEval, int maxIter)
		{
			this.checker = checker;

			evaluations = new Incrementor(maxEval, new MaxEvalCallback());
			iterations = new Incrementor(maxIter, new MaxIterCallback());
		}

		/// <summary>
		/// Gets the maximal number of function evaluations.
		/// </summary>
		/// <returns> the maximal number of function evaluations. </returns>
		public virtual int MaxEvaluations
		{
			get
			{
				return evaluations.MaximalCount;
			}
		}

		/// <summary>
		/// Gets the number of evaluations of the objective function.
		/// The number of evaluations corresponds to the last call to the
		/// {@code optimize} method. It is 0 if the method has not been
		/// called yet.
		/// </summary>
		/// <returns> the number of evaluations of the objective function. </returns>
		public virtual int Evaluations
		{
			get
			{
				return evaluations.Count;
			}
		}

		/// <summary>
		/// Gets the maximal number of iterations.
		/// </summary>
		/// <returns> the maximal number of iterations. </returns>
		public virtual int MaxIterations
		{
			get
			{
				return iterations.MaximalCount;
			}
		}

		/// <summary>
		/// Gets the number of iterations performed by the algorithm.
		/// The number iterations corresponds to the last call to the
		/// {@code optimize} method. It is 0 if the method has not been
		/// called yet.
		/// </summary>
		/// <returns> the number of evaluations of the objective function. </returns>
		public virtual int Iterations
		{
			get
			{
				return iterations.Count;
			}
		}

		/// <summary>
		/// Gets the convergence checker.
		/// </summary>
		/// <returns> the object used to check for convergence. </returns>
		public virtual ConvergenceChecker<PAIR> ConvergenceChecker
		{
			get
			{
				return checker;
			}
		}

		/// <summary>
		/// Stores data and performs the optimization.
		/// <p>
		/// The list of parameters is open-ended so that sub-classes can extend it
		/// with arguments specific to their concrete implementations.
		/// <p>
		/// When the method is called multiple times, instance data is overwritten
		/// only when actually present in the list of arguments: when not specified,
		/// data set in a previous call is retained (and thus is optional in
		/// subsequent calls).
		/// <p>
		/// Important note: Subclasses <em>must</em> override
		/// <seealso cref="#parseOptimizationData(OptimizationData[])"/> if they need to register
		/// their own options; but then, they <em>must</em> also call
		/// {@code super.parseOptimizationData(optData)} within that method.
		/// </summary>
		/// <param name="optData"> Optimization data.
		/// This method will register the following data:
		/// <ul>
		///  <li><seealso cref="MaxEval"/></li>
		///  <li><seealso cref="MaxIter"/></li>
		/// </ul> </param>
		/// <returns> a point/value pair that satisfies the convergence criteria. </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of
		/// evaluations is exceeded. </exception>
		/// <exception cref="TooManyIterationsException"> if the maximal number of
		/// iterations is exceeded. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PAIR optimize(OptimizationData... optData) throws mathlib.exception.TooManyEvaluationsException, mathlib.exception.TooManyIterationsException
		public virtual PAIR optimize(params OptimizationData[] optData)
		{
			// Parse options.
			parseOptimizationData(optData);

			// Reset counters.
			evaluations.resetCount();
			iterations.resetCount();
			// Perform optimization.
			return doOptimize();
		}

		/// <summary>
		/// Performs the optimization.
		/// </summary>
		/// <returns> a point/value pair that satisfies the convergence criteria. </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of
		/// evaluations is exceeded. </exception>
		/// <exception cref="TooManyIterationsException"> if the maximal number of
		/// iterations is exceeded. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PAIR optimize() throws mathlib.exception.TooManyEvaluationsException, mathlib.exception.TooManyIterationsException
		public virtual PAIR optimize()
		{
			// Reset counters.
			evaluations.resetCount();
			iterations.resetCount();
			// Perform optimization.
			return doOptimize();
		}

		/// <summary>
		/// Performs the bulk of the optimization algorithm.
		/// </summary>
		/// <returns> the point/value pair giving the optimal value of the
		/// objective function. </returns>
		protected internal abstract PAIR doOptimize();

		/// <summary>
		/// Increment the evaluation count.
		/// </summary>
		/// <exception cref="TooManyEvaluationsException"> if the allowed evaluations
		/// have been exhausted. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void incrementEvaluationCount() throws mathlib.exception.TooManyEvaluationsException
		protected internal virtual void incrementEvaluationCount()
		{
			evaluations.incrementCount();
		}

		/// <summary>
		/// Increment the iteration count.
		/// </summary>
		/// <exception cref="TooManyIterationsException"> if the allowed iterations
		/// have been exhausted. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void incrementIterationCount() throws mathlib.exception.TooManyIterationsException
		protected internal virtual void incrementIterationCount()
		{
			iterations.incrementCount();
		}

		/// <summary>
		/// Scans the list of (required and optional) optimization data that
		/// characterize the problem.
		/// </summary>
		/// <param name="optData"> Optimization data.
		/// The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="MaxEval"/></li>
		///  <li><seealso cref="MaxIter"/></li>
		/// </ul> </param>
		protected internal virtual void parseOptimizationData(params OptimizationData[] optData)
		{
			// The existing values (as set by the previous call) are reused if
			// not provided in the argument list.
			foreach (OptimizationData data in optData)
			{
				if (data is MaxEval)
				{
					evaluations.MaximalCount = ((MaxEval) data).MaxEval;
					continue;
				}
				if (data is MaxIter)
				{
					iterations.MaximalCount = ((MaxIter) data).MaxIter;
					continue;
				}
			}
		}

		/// <summary>
		/// Defines the action to perform when reaching the maximum number
		/// of evaluations.
		/// </summary>
		private class MaxEvalCallback : Incrementor.MaxCountExceededCallback
		{
			/// <summary>
			/// {@inheritDoc} </summary>
			/// <exception cref="TooManyEvaluationsException"> </exception>
			public virtual void trigger(int max)
			{
				throw new TooManyEvaluationsException(max);
			}
		}

		/// <summary>
		/// Defines the action to perform when reaching the maximum number
		/// of evaluations.
		/// </summary>
		private class MaxIterCallback : Incrementor.MaxCountExceededCallback
		{
			/// <summary>
			/// {@inheritDoc} </summary>
			/// <exception cref="TooManyIterationsException"> </exception>
			public virtual void trigger(int max)
			{
				throw new TooManyIterationsException(max);
			}
		}
	}

}
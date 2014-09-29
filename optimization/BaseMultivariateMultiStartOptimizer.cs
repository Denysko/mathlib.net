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

namespace mathlib.optimization
{


	using MultivariateFunction = mathlib.analysis.MultivariateFunction;
	using MathIllegalStateException = mathlib.exception.MathIllegalStateException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using RandomVectorGenerator = mathlib.random.RandomVectorGenerator;

	/// <summary>
	/// Base class for all implementations of a multi-start optimizer.
	/// 
	/// This interface is mainly intended to enforce the internal coherence of
	/// Commons-Math. Users of the API are advised to base their code on
	/// <seealso cref="MultivariateMultiStartOptimizer"/> or on
	/// <seealso cref="DifferentiableMultivariateMultiStartOptimizer"/>.
	/// </summary>
	/// @param <FUNC> Type of the objective function to be optimized.
	/// 
	/// @version $Id: BaseMultivariateMultiStartOptimizer.java 1591835 2014-05-02 09:04:01Z tn $ </param>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public class BaseMultivariateMultiStartOptimizer<FUNC> : BaseMultivariateOptimizer<FUNC> where FUNC : mathlib.analysis.MultivariateFunction
	{
		/// <summary>
		/// Underlying classical optimizer. </summary>
		private readonly BaseMultivariateOptimizer<FUNC> optimizer;
		/// <summary>
		/// Maximal number of evaluations allowed. </summary>
		private int maxEvaluations;
		/// <summary>
		/// Number of evaluations already performed for all starts. </summary>
		private int totalEvaluations;
		/// <summary>
		/// Number of starts to go. </summary>
		private int starts;
		/// <summary>
		/// Random generator for multi-start. </summary>
		private RandomVectorGenerator generator;
		/// <summary>
		/// Found optima. </summary>
		private PointValuePair[] optima;

		/// <summary>
		/// Create a multi-start optimizer from a single-start optimizer.
		/// </summary>
		/// <param name="optimizer"> Single-start optimizer to wrap. </param>
		/// <param name="starts"> Number of starts to perform. If {@code starts == 1},
		/// the {@link #optimize(int,MultivariateFunction,GoalType,double[])
		/// optimize} will return the same solution as {@code optimizer} would. </param>
		/// <param name="generator"> Random vector generator to use for restarts. </param>
		/// <exception cref="NullArgumentException"> if {@code optimizer} or {@code generator}
		/// is {@code null}. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if {@code starts < 1}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected BaseMultivariateMultiStartOptimizer(final BaseMultivariateOptimizer<FUNC> optimizer, final int starts, final mathlib.random.RandomVectorGenerator generator)
		protected internal BaseMultivariateMultiStartOptimizer(BaseMultivariateOptimizer<FUNC> optimizer, int starts, RandomVectorGenerator generator)
		{
			if (optimizer == null || generator == null)
			{
				throw new NullArgumentException();
			}
			if (starts < 1)
			{
				throw new NotStrictlyPositiveException(starts);
			}

			this.optimizer = optimizer;
			this.starts = starts;
			this.generator = generator;
		}

		/// <summary>
		/// Get all the optima found during the last call to {@link
		/// #optimize(int,MultivariateFunction,GoalType,double[]) optimize}.
		/// The optimizer stores all the optima found during a set of
		/// restarts. The {@link #optimize(int,MultivariateFunction,GoalType,double[])
		/// optimize} method returns the best point only. This method
		/// returns all the points found at the end of each starts,
		/// including the best one already returned by the {@link
		/// #optimize(int,MultivariateFunction,GoalType,double[]) optimize} method.
		/// <br/>
		/// The returned array as one element for each start as specified
		/// in the constructor. It is ordered with the results from the
		/// runs that did converge first, sorted from best to worst
		/// objective value (i.e in ascending order if minimizing and in
		/// descending order if maximizing), followed by and null elements
		/// corresponding to the runs that did not converge. This means all
		/// elements will be null if the {@link #optimize(int,MultivariateFunction,GoalType,double[])
		/// optimize} method did throw an exception.
		/// This also means that if the first element is not {@code null}, it
		/// is the best point found across all starts.
		/// </summary>
		/// <returns> an array containing the optima. </returns>
		/// <exception cref="MathIllegalStateException"> if {@link
		/// #optimize(int,MultivariateFunction,GoalType,double[]) optimize}
		/// has not been called. </exception>
		public virtual PointValuePair[] Optima
		{
			get
			{
				if (optima == null)
				{
					throw new MathIllegalStateException(LocalizedFormats.NO_OPTIMUM_COMPUTED_YET);
				}
				return optima.clone();
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int MaxEvaluations
		{
			get
			{
				return maxEvaluations;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int Evaluations
		{
			get
			{
				return totalEvaluations;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual ConvergenceChecker<PointValuePair> ConvergenceChecker
		{
			get
			{
				return optimizer.ConvergenceChecker;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PointValuePair optimize(int maxEval, final FUNC f, final GoalType goal, double[] startPoint)
		public virtual PointValuePair optimize(int maxEval, FUNC f, GoalType goal, double[] startPoint)
		{
			maxEvaluations = maxEval;
			Exception lastException = null;
			optima = new PointValuePair[starts];
			totalEvaluations = 0;

			// Multi-start loop.
			for (int i = 0; i < starts; ++i)
			{
				// CHECKSTYLE: stop IllegalCatch
				try
				{
					optima[i] = optimizer.optimize(maxEval - totalEvaluations, f, goal, i == 0 ? startPoint : generator.nextVector());
				}
				catch (Exception mue)
				{
					lastException = mue;
					optima[i] = null;
				}
				// CHECKSTYLE: resume IllegalCatch

				totalEvaluations += optimizer.Evaluations;
			}

			sortPairs(goal);

			if (optima[0] == null)
			{
				throw lastException; // cannot be null if starts >=1
			}

			// Return the found point given the best objective function value.
			return optima[0];
		}

		/// <summary>
		/// Sort the optima from best to worst, followed by {@code null} elements.
		/// </summary>
		/// <param name="goal"> Goal type. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void sortPairs(final GoalType goal)
		private void sortPairs(GoalType goal)
		{
			Arrays.sort(optima, new ComparatorAnonymousInnerClassHelper(this, goal));
		}

		private class ComparatorAnonymousInnerClassHelper : IComparer<PointValuePair>
		{
			private readonly BaseMultivariateMultiStartOptimizer outerInstance;

			private GoalType goal;

			public ComparatorAnonymousInnerClassHelper(BaseMultivariateMultiStartOptimizer outerInstance, GoalType goal)
			{
				this.outerInstance = outerInstance;
				this.goal = goal;
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public int compare(final PointValuePair o1, final PointValuePair o2)
			public virtual int Compare(PointValuePair o1, PointValuePair o2)
			{
				if (o1 == null)
				{
					return (o2 == null) ? 0 : 1;
				}
				else if (o2 == null)
				{
					return -1;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double v1 = o1.getValue();
				double v1 = o1.Value;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double v2 = o2.getValue();
				double v2 = o2.Value;
				return (goal == GoalType.MINIMIZE) ? v1.CompareTo(v2) : v2.CompareTo(v1);
			}
		}
	}

}
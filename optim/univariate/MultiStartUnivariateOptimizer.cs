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

namespace mathlib.optim.univariate
{

	using MathIllegalStateException = mathlib.exception.MathIllegalStateException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using GoalType = mathlib.optim.nonlinear.scalar.GoalType;

	/// <summary>
	/// Special implementation of the <seealso cref="UnivariateOptimizer"/> interface
	/// adding multi-start features to an existing optimizer.
	/// <br/>
	/// This class wraps an optimizer in order to use it several times in
	/// turn with different starting points (trying to avoid being trapped
	/// in a local extremum when looking for a global one).
	/// 
	/// @version $Id: MultiStartUnivariateOptimizer.java 1435539 2013-01-19 13:27:24Z tn $
	/// @since 3.0
	/// </summary>
	public class MultiStartUnivariateOptimizer : UnivariateOptimizer
	{
		/// <summary>
		/// Underlying classical optimizer. </summary>
		private readonly UnivariateOptimizer optimizer;
		/// <summary>
		/// Number of evaluations already performed for all starts. </summary>
		private int totalEvaluations;
		/// <summary>
		/// Number of starts to go. </summary>
		private int starts;
		/// <summary>
		/// Random generator for multi-start. </summary>
		private RandomGenerator generator;
		/// <summary>
		/// Found optima. </summary>
		private UnivariatePointValuePair[] optima;
		/// <summary>
		/// Optimization data. </summary>
		private OptimizationData[] optimData;
		/// <summary>
		/// Location in <seealso cref="#optimData"/> where the updated maximum
		/// number of evaluations will be stored.
		/// </summary>
		private int maxEvalIndex = -1;
		/// <summary>
		/// Location in <seealso cref="#optimData"/> where the updated start value
		/// will be stored.
		/// </summary>
		private int searchIntervalIndex = -1;

		/// <summary>
		/// Create a multi-start optimizer from a single-start optimizer.
		/// </summary>
		/// <param name="optimizer"> Single-start optimizer to wrap. </param>
		/// <param name="starts"> Number of starts to perform. If {@code starts == 1},
		/// the {@code optimize} methods will return the same solution as
		/// {@code optimizer} would. </param>
		/// <param name="generator"> Random generator to use for restarts. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code starts < 1}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MultiStartUnivariateOptimizer(final UnivariateOptimizer optimizer, final int starts, final mathlib.random.RandomGenerator generator)
		public MultiStartUnivariateOptimizer(UnivariateOptimizer optimizer, int starts, RandomGenerator generator) : base(optimizer.ConvergenceChecker)
		{

			if (starts < 1)
			{
				throw new NotStrictlyPositiveException(starts);
			}

			this.optimizer = optimizer;
			this.starts = starts;
			this.generator = generator;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int Evaluations
		{
			get
			{
				return totalEvaluations;
			}
		}

		/// <summary>
		/// Gets all the optima found during the last call to {@code optimize}.
		/// The optimizer stores all the optima found during a set of
		/// restarts. The {@code optimize} method returns the best point only.
		/// This method returns all the points found at the end of each starts,
		/// including the best one already returned by the {@code optimize} method.
		/// <br/>
		/// The returned array as one element for each start as specified
		/// in the constructor. It is ordered with the results from the
		/// runs that did converge first, sorted from best to worst
		/// objective value (i.e in ascending order if minimizing and in
		/// descending order if maximizing), followed by {@code null} elements
		/// corresponding to the runs that did not converge. This means all
		/// elements will be {@code null} if the {@code optimize} method did throw
		/// an exception.
		/// This also means that if the first element is not {@code null}, it is
		/// the best point found across all starts.
		/// </summary>
		/// <returns> an array containing the optima. </returns>
		/// <exception cref="MathIllegalStateException"> if {@link #optimize(OptimizationData[])
		/// optimize} has not been called. </exception>
		public virtual UnivariatePointValuePair[] Optima
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
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="MathIllegalStateException"> if {@code optData} does not contain an
		/// instance of <seealso cref="MaxEval"/> or <seealso cref="SearchInterval"/>. </exception>
		public override UnivariatePointValuePair optimize(params OptimizationData[] optData)
		{
			// Store arguments in order to pass them to the internal optimizer.
		   optimData = optData;
			// Set up base class and perform computations.
			return base.optimize(optData);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override UnivariatePointValuePair doOptimize()
		{
			// Remove all instances of "MaxEval" and "SearchInterval" from the
			// array that will be passed to the internal optimizer.
			// The former is to enforce smaller numbers of allowed evaluations
			// (according to how many have been used up already), and the latter
			// to impose a different start value for each start.
			for (int i = 0; i < optimData.Length; i++)
			{
				if (optimData[i] is MaxEval)
				{
					optimData[i] = null;
					maxEvalIndex = i;
					continue;
				}
				if (optimData[i] is SearchInterval)
				{
					optimData[i] = null;
					searchIntervalIndex = i;
					continue;
				}
			}
			if (maxEvalIndex == -1)
			{
				throw new MathIllegalStateException();
			}
			if (searchIntervalIndex == -1)
			{
				throw new MathIllegalStateException();
			}

			Exception lastException = null;
			optima = new UnivariatePointValuePair[starts];
			totalEvaluations = 0;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int maxEval = getMaxEvaluations();
			int maxEval = MaxEvaluations;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double min = getMin();
			double min = Min;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double max = getMax();
			double max = Max;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double startValue = getStartValue();
			double startValue = StartValue;

			// Multi-start loop.
			for (int i = 0; i < starts; i++)
			{
				// CHECKSTYLE: stop IllegalCatch
				try
				{
					// Decrease number of allowed evaluations.
					optimData[maxEvalIndex] = new MaxEval(maxEval - totalEvaluations);
					// New start value.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s = (i == 0) ? startValue : min + generator.nextDouble() * (max - min);
					double s = (i == 0) ? startValue : min + generator.NextDouble() * (max - min);
					optimData[searchIntervalIndex] = new SearchInterval(min, max, s);
					// Optimize.
					optima[i] = optimizer.optimize(optimData);
				}
				catch (Exception mue)
				{
					lastException = mue;
					optima[i] = null;
				}
				// CHECKSTYLE: resume IllegalCatch

				totalEvaluations += optimizer.Evaluations;
			}

			sortPairs(GoalType);

			if (optima[0] == null)
			{
				throw lastException; // Cannot be null if starts >= 1.
			}

			// Return the point with the best objective function value.
			return optima[0];
		}

		/// <summary>
		/// Sort the optima from best to worst, followed by {@code null} elements.
		/// </summary>
		/// <param name="goal"> Goal type. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void sortPairs(final mathlib.optim.nonlinear.scalar.GoalType goal)
		private void sortPairs(GoalType goal)
		{
			Arrays.sort(optima, new ComparatorAnonymousInnerClassHelper(this, goal));
		}

		private class ComparatorAnonymousInnerClassHelper : IComparer<UnivariatePointValuePair>
		{
			private readonly MultiStartUnivariateOptimizer outerInstance;

			private GoalType goal;

			public ComparatorAnonymousInnerClassHelper(MultiStartUnivariateOptimizer outerInstance, GoalType goal)
			{
				this.outerInstance = outerInstance;
				this.goal = goal;
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public int compare(final UnivariatePointValuePair o1, final UnivariatePointValuePair o2)
			public virtual int Compare(UnivariatePointValuePair o1, UnivariatePointValuePair o2)
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
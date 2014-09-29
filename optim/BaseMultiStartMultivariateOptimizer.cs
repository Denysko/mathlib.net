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
namespace mathlib.optim
{

	using MathIllegalStateException = mathlib.exception.MathIllegalStateException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;
	using RandomVectorGenerator = mathlib.random.RandomVectorGenerator;

	/// <summary>
	/// Base class multi-start optimizer for a multivariate function.
	/// <br/>
	/// This class wraps an optimizer in order to use it several times in
	/// turn with different starting points (trying to avoid being trapped
	/// in a local extremum when looking for a global one).
	/// <em>It is not a "user" class.</em>
	/// </summary>
	/// @param <PAIR> Type of the point/value pair returned by the optimization
	/// algorithm.
	/// 
	/// @version $Id: BaseMultiStartMultivariateOptimizer.java 1591835 2014-05-02 09:04:01Z tn $
	/// @since 3.0 </param>
	public abstract class BaseMultiStartMultivariateOptimizer<PAIR> : BaseMultivariateOptimizer<PAIR>
	{
		/// <summary>
		/// Underlying classical optimizer. </summary>
		private readonly BaseMultivariateOptimizer<PAIR> optimizer;
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
		private int initialGuessIndex = -1;

		/// <summary>
		/// Create a multi-start optimizer from a single-start optimizer.
		/// <p>
		/// Note that if there are bounds constraints (see <seealso cref="#getLowerBound()"/>
		/// and <seealso cref="#getUpperBound()"/>), then a simple rejection algorithm is used
		/// at each restart. This implies that the random vector generator should have
		/// a good probability to generate vectors in the bounded domain, otherwise the
		/// rejection algorithm will hit the <seealso cref="#getMaxEvaluations()"/> count without
		/// generating a proper restart point. Users must be take great care of the <a
		/// href="http://en.wikipedia.org/wiki/Curse_of_dimensionality">curse of dimensionality</a>.
		/// </p> </summary>
		/// <param name="optimizer"> Single-start optimizer to wrap. </param>
		/// <param name="starts"> Number of starts to perform. If {@code starts == 1},
		/// the <seealso cref="#optimize(OptimizationData[]) optimize"/> will return the
		/// same solution as the given {@code optimizer} would return. </param>
		/// <param name="generator"> Random vector generator to use for restarts. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code starts < 1}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BaseMultiStartMultivariateOptimizer(final BaseMultivariateOptimizer<PAIR> optimizer, final int starts, final mathlib.random.RandomVectorGenerator generator)
		public BaseMultiStartMultivariateOptimizer(BaseMultivariateOptimizer<PAIR> optimizer, int starts, RandomVectorGenerator generator) : base(optimizer.ConvergenceChecker)
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
		/// <br/>
		/// The behaviour is undefined if this method is called before
		/// {@code optimize}; it will likely throw {@code NullPointerException}.
		/// </summary>
		/// <returns> an array containing the optima sorted from best to worst. </returns>
		public abstract PAIR[] Optima {get;}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="MathIllegalStateException"> if {@code optData} does not contain an
		/// instance of <seealso cref="MaxEval"/> or <seealso cref="InitialGuess"/>. </exception>
		public override PAIR optimize(params OptimizationData[] optData)
		{
			// Store arguments in order to pass them to the internal optimizer.
		   optimData = optData;
			// Set up base class and perform computations.
			return base.optimize(optData);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override PAIR doOptimize()
		{
			// Remove all instances of "MaxEval" and "InitialGuess" from the
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
				}
				if (optimData[i] is InitialGuess)
				{
					optimData[i] = null;
					initialGuessIndex = i;
					continue;
				}
			}
			if (maxEvalIndex == -1)
			{
				throw new MathIllegalStateException();
			}
			if (initialGuessIndex == -1)
			{
				throw new MathIllegalStateException();
			}

			Exception lastException = null;
			totalEvaluations = 0;
			clear();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int maxEval = getMaxEvaluations();
			int maxEval = MaxEvaluations;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] min = getLowerBound();
			double[] min = LowerBound;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] max = getUpperBound();
			double[] max = UpperBound;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] startPoint = getStartPoint();
			double[] startPoint = StartPoint;

			// Multi-start loop.
			for (int i = 0; i < starts; i++)
			{
				// CHECKSTYLE: stop IllegalCatch
				try
				{
					// Decrease number of allowed evaluations.
					optimData[maxEvalIndex] = new MaxEval(maxEval - totalEvaluations);
					// New start value.
					double[] s = null;
					if (i == 0)
					{
						s = startPoint;
					}
					else
					{
						int attempts = 0;
						while (s == null)
						{
							if (attempts++ >= MaxEvaluations)
							{
								throw new TooManyEvaluationsException(MaxEvaluations);
							}
							s = generator.nextVector();
							for (int k = 0; s != null && k < s.Length; ++k)
							{
								if ((min != null && s[k] < min[k]) || (max != null && s[k] > max[k]))
								{
									// reject the vector
									s = null;
								}
							}
						}
					}
					optimData[initialGuessIndex] = new InitialGuess(s);
					// Optimize.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final PAIR result = optimizer.optimize(optimData);
					PAIR result = optimizer.optimize(optimData);
					store(result);
				}
				catch (Exception mue)
				{
					lastException = mue;
				}
				// CHECKSTYLE: resume IllegalCatch

				totalEvaluations += optimizer.Evaluations;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final PAIR[] optima = getOptima();
			PAIR[] optima = Optima;
			if (optima.Length == 0)
			{
				// All runs failed.
				throw lastException; // Cannot be null if starts >= 1.
			}

			// Return the best optimum.
			return optima[0];
		}

		/// <summary>
		/// Method that will be called in order to store each found optimum.
		/// </summary>
		/// <param name="optimum"> Result of an optimization run. </param>
		protected internal abstract void store(PAIR optimum);
		/// <summary>
		/// Method that will called in order to clear all stored optima.
		/// </summary>
		protected internal abstract void clear();
	}

}
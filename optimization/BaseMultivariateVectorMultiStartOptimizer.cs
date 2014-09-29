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


	using MultivariateVectorFunction = mathlib.analysis.MultivariateVectorFunction;
	using ConvergenceException = mathlib.exception.ConvergenceException;
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
	/// <seealso cref="DifferentiableMultivariateVectorMultiStartOptimizer"/>.
	/// </summary>
	/// @param <FUNC> Type of the objective function to be optimized.
	/// 
	/// @version $Id: BaseMultivariateVectorMultiStartOptimizer.java 1591835 2014-05-02 09:04:01Z tn $ </param>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public class BaseMultivariateVectorMultiStartOptimizer<FUNC> : BaseMultivariateVectorOptimizer<FUNC> where FUNC : mathlib.analysis.MultivariateVectorFunction
	{
		/// <summary>
		/// Underlying classical optimizer. </summary>
		private readonly BaseMultivariateVectorOptimizer<FUNC> optimizer;
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
		private PointVectorValuePair[] optima;

		/// <summary>
		/// Create a multi-start optimizer from a single-start optimizer.
		/// </summary>
		/// <param name="optimizer"> Single-start optimizer to wrap. </param>
		/// <param name="starts"> Number of starts to perform. If {@code starts == 1},
		/// the {@link #optimize(int,MultivariateVectorFunction,double[],double[],double[])
		/// optimize} will return the same solution as {@code optimizer} would. </param>
		/// <param name="generator"> Random vector generator to use for restarts. </param>
		/// <exception cref="NullArgumentException"> if {@code optimizer} or {@code generator}
		/// is {@code null}. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if {@code starts < 1}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected BaseMultivariateVectorMultiStartOptimizer(final BaseMultivariateVectorOptimizer<FUNC> optimizer, final int starts, final mathlib.random.RandomVectorGenerator generator)
		protected internal BaseMultivariateVectorMultiStartOptimizer(BaseMultivariateVectorOptimizer<FUNC> optimizer, int starts, RandomVectorGenerator generator)
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
		/// #optimize(int,MultivariateVectorFunction,double[],double[],double[]) optimize}.
		/// The optimizer stores all the optima found during a set of
		/// restarts. The {@link #optimize(int,MultivariateVectorFunction,double[],double[],double[])
		/// optimize} method returns the best point only. This method
		/// returns all the points found at the end of each starts, including
		/// the best one already returned by the {@link
		/// #optimize(int,MultivariateVectorFunction,double[],double[],double[]) optimize} method.
		/// <br/>
		/// The returned array as one element for each start as specified
		/// in the constructor. It is ordered with the results from the
		/// runs that did converge first, sorted from best to worst
		/// objective value (i.e. in ascending order if minimizing and in
		/// descending order if maximizing), followed by and null elements
		/// corresponding to the runs that did not converge. This means all
		/// elements will be null if the {@link
		/// #optimize(int,MultivariateVectorFunction,double[],double[],double[]) optimize} method did
		/// throw a <seealso cref="ConvergenceException"/>). This also means that if
		/// the first element is not {@code null}, it is the best point found
		/// across all starts.
		/// </summary>
		/// <returns> array containing the optima </returns>
		/// <exception cref="MathIllegalStateException"> if {@link
		/// #optimize(int,MultivariateVectorFunction,double[],double[],double[]) optimize} has not been
		/// called. </exception>
		public virtual PointVectorValuePair[] Optima
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
		public virtual ConvergenceChecker<PointVectorValuePair> ConvergenceChecker
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
//ORIGINAL LINE: public PointVectorValuePair optimize(int maxEval, final FUNC f, double[] target, double[] weights, double[] startPoint)
		public virtual PointVectorValuePair optimize(int maxEval, FUNC f, double[] target, double[] weights, double[] startPoint)
		{
			maxEvaluations = maxEval;
			Exception lastException = null;
			optima = new PointVectorValuePair[starts];
			totalEvaluations = 0;

			// Multi-start loop.
			for (int i = 0; i < starts; ++i)
			{

				// CHECKSTYLE: stop IllegalCatch
				try
				{
					optima[i] = optimizer.optimize(maxEval - totalEvaluations, f, target, weights, i == 0 ? startPoint : generator.nextVector());
				}
				catch (ConvergenceException oe)
				{
					optima[i] = null;
				}
				catch (Exception mue)
				{
					lastException = mue;
					optima[i] = null;
				}
				// CHECKSTYLE: resume IllegalCatch

				totalEvaluations += optimizer.Evaluations;
			}

			sortPairs(target, weights);

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
		/// <param name="target"> Target value for the objective functions at optimum. </param>
		/// <param name="weights"> Weights for the least-squares cost computation. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void sortPairs(final double[] target, final double[] weights)
		private void sortPairs(double[] target, double[] weights)
		{
			Arrays.sort(optima, new ComparatorAnonymousInnerClassHelper(this, target, weights));
		}

		private class ComparatorAnonymousInnerClassHelper : IComparer<PointVectorValuePair>
		{
			private readonly BaseMultivariateVectorMultiStartOptimizer outerInstance;

			private double[] target;
			private double[] weights;

			public ComparatorAnonymousInnerClassHelper(BaseMultivariateVectorMultiStartOptimizer outerInstance, double[] target, double[] weights)
			{
				this.outerInstance = outerInstance;
				this.target = target;
				this.weights = weights;
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public int compare(final PointVectorValuePair o1, final PointVectorValuePair o2)
			public virtual int Compare(PointVectorValuePair o1, PointVectorValuePair o2)
			{
				if (o1 == null)
				{
					return (o2 == null) ? 0 : 1;
				}
				else if (o2 == null)
				{
					return -1;
				}
				return weightedResidual(o1).CompareTo(weightedResidual(o2));
			}
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double weightedResidual(final PointVectorValuePair pv)
			private double weightedResidual(PointVectorValuePair pv)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] value = pv.getValueRef();
				double[] value = pv.ValueRef;
				double sum = 0;
				for (int i = 0; i < value.Length; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ri = value[i] - target[i];
					double ri = value[i] - target[i];
					sum += weights[i] * ri * ri;
				}
				return sum;
			}
		}
	}

}
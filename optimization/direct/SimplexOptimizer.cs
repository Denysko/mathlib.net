using System;
using System.Collections.Generic;

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

namespace mathlib.optimization.direct
{

	using MultivariateFunction = mathlib.analysis.MultivariateFunction;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using mathlib.optimization;

	/// <summary>
	/// This class implements simplex-based direct search optimization.
	/// 
	/// <p>
	///  Direct search methods only use objective function values, they do
	///  not need derivatives and don't either try to compute approximation
	///  of the derivatives. According to a 1996 paper by Margaret H. Wright
	///  (<a href="http://cm.bell-labs.com/cm/cs/doc/96/4-02.ps.gz">Direct
	///  Search Methods: Once Scorned, Now Respectable</a>), they are used
	///  when either the computation of the derivative is impossible (noisy
	///  functions, unpredictable discontinuities) or difficult (complexity,
	///  computation cost). In the first cases, rather than an optimum, a
	///  <em>not too bad</em> point is desired. In the latter cases, an
	///  optimum is desired but cannot be reasonably found. In all cases
	///  direct search methods can be useful.
	/// </p>
	/// <p>
	///  Simplex-based direct search methods are based on comparison of
	///  the objective function values at the vertices of a simplex (which is a
	///  set of n+1 points in dimension n) that is updated by the algorithms
	///  steps.
	/// <p>
	/// <p>
	///  The <seealso cref="#setSimplex(AbstractSimplex) setSimplex"/> method <em>must</em>
	///  be called prior to calling the {@code optimize} method.
	/// </p>
	/// <p>
	///  Each call to {@link #optimize(int,MultivariateFunction,GoalType,double[])
	///  optimize} will re-use the start configuration of the current simplex and
	///  move it such that its first vertex is at the provided start point of the
	///  optimization. If the {@code optimize} method is called to solve a different
	///  problem and the number of parameters change, the simplex must be
	///  re-initialized to one with the appropriate dimensions.
	/// </p>
	/// <p>
	///  Convergence is checked by providing the <em>worst</em> points of
	///  previous and current simplex to the convergence checker, not the best
	///  ones.
	/// </p>
	/// <p>
	/// This simplex optimizer implementation does not directly support constrained
	/// optimization with simple bounds, so for such optimizations, either a more
	/// dedicated method must be used like <seealso cref="CMAESOptimizer"/> or {@link
	/// BOBYQAOptimizer}, or the optimized method must be wrapped in an adapter like
	/// <seealso cref="MultivariateFunctionMappingAdapter"/> or {@link
	/// MultivariateFunctionPenaltyAdapter}.
	/// </p>
	/// </summary>
	/// <seealso cref= AbstractSimplex </seealso>
	/// <seealso cref= MultivariateFunctionMappingAdapter </seealso>
	/// <seealso cref= MultivariateFunctionPenaltyAdapter </seealso>
	/// <seealso cref= CMAESOptimizer </seealso>
	/// <seealso cref= BOBYQAOptimizer
	/// @version $Id: SimplexOptimizer.java 1504724 2013-07-18 23:41:20Z sebb $ </seealso>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.0 
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("boxing") @Deprecated public class SimplexOptimizer extends BaseAbstractMultivariateOptimizer<mathlib.analysis.MultivariateFunction> implements mathlib.optimization.MultivariateOptimizer
	[Obsolete]
	public class SimplexOptimizer : BaseAbstractMultivariateOptimizer<MultivariateFunction>, MultivariateOptimizer // deprecated anyway
	{
		/// <summary>
		/// Simplex. </summary>
		private AbstractSimplex simplex;

		/// <summary>
		/// Constructor using a default {@link SimpleValueChecker convergence
		/// checker}. </summary>
		/// @deprecated See <seealso cref="SimpleValueChecker#SimpleValueChecker()"/> 
		[Obsolete]//("See <seealso cref="SimpleValueChecker#SimpleValueChecker()"/>")]
		public SimplexOptimizer() : this(new SimpleValueChecker())
		{
		}

		/// <param name="checker"> Convergence checker. </param>
		public SimplexOptimizer(ConvergenceChecker<PointValuePair> checker) : base(checker)
		{
		}

		/// <param name="rel"> Relative threshold. </param>
		/// <param name="abs"> Absolute threshold. </param>
		public SimplexOptimizer(double rel, double abs) : this(new SimpleValueChecker(rel, abs))
		{
		}

		/// <summary>
		/// Set the simplex algorithm.
		/// </summary>
		/// <param name="simplex"> Simplex. </param>
		/// @deprecated As of 3.1. The initial simplex can now be passed as an
		/// argument of the <seealso cref="#optimize(int,MultivariateFunction,GoalType,OptimizationData[])"/>
		/// method. 
		[Obsolete("As of 3.1. The initial simplex can now be passed as an")]
		public virtual AbstractSimplex Simplex
		{
			set
			{
				parseOptimizationData(value);
			}
		}

		/// <summary>
		/// Optimize an objective function.
		/// </summary>
		/// <param name="maxEval"> Allowed number of evaluations of the objective function. </param>
		/// <param name="f"> Objective function. </param>
		/// <param name="goalType"> Optimization type. </param>
		/// <param name="optData"> Optimization data. The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="mathlib.optimization.InitialGuess InitialGuess"/></li>
		///  <li><seealso cref="AbstractSimplex"/></li>
		/// </ul> </param>
		/// <returns> the point/value pair giving the optimal value for objective
		/// function. </returns>
		protected internal override PointValuePair optimizeInternal(int maxEval, MultivariateFunction f, GoalType goalType, params OptimizationData[] optData)
		{
			// Scan "optData" for the input specific to this optimizer.
			parseOptimizationData(optData);

			// The parent's method will retrieve the common parameters from
			// "optData" and call "doOptimize".
			return base.optimizeInternal(maxEval, f, goalType, optData);
		}

		/// <summary>
		/// Scans the list of (required and optional) optimization data that
		/// characterize the problem.
		/// </summary>
		/// <param name="optData"> Optimization data. The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="AbstractSimplex"/></li>
		/// </ul> </param>
		private void parseOptimizationData(params OptimizationData[] optData)
		{
			// The existing values (as set by the previous call) are reused if
			// not provided in the argument list.
			foreach (OptimizationData data in optData)
			{
				if (data is AbstractSimplex)
				{
					simplex = (AbstractSimplex) data;
					continue;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override PointValuePair doOptimize()
		{
			if (simplex == null)
			{
				throw new NullArgumentException();
			}

			// Indirect call to "computeObjectiveValue" in order to update the
			// evaluations counter.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.analysis.MultivariateFunction evalFunc = new mathlib.analysis.MultivariateFunction()
			MultivariateFunction evalFunc = new MultivariateFunctionAnonymousInnerClassHelper(this);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isMinim = getGoalType() == mathlib.optimization.GoalType.MINIMIZE;
			bool isMinim = GoalType == GoalType.MINIMIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Comparator<mathlib.optimization.PointValuePair> comparator = new java.util.Comparator<mathlib.optimization.PointValuePair>()
			IComparer<PointValuePair> comparator = new ComparatorAnonymousInnerClassHelper(this, isMinim);

			// Initialize search.
			simplex.build(StartPoint);
			simplex.evaluate(evalFunc, comparator);

			PointValuePair[] previous = null;
			int iteration = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.optimization.ConvergenceChecker<mathlib.optimization.PointValuePair> checker = getConvergenceChecker();
			ConvergenceChecker<PointValuePair> checker = ConvergenceChecker;
			while (true)
			{
				if (iteration > 0)
				{
					bool converged = true;
					for (int i = 0; i < simplex.Size; i++)
					{
						PointValuePair prev = previous[i];
						converged = converged && checker.converged(iteration, prev, simplex.getPoint(i));
					}
					if (converged)
					{
						// We have found an optimum.
						return simplex.getPoint(0);
					}
				}

				// We still need to search.
				previous = simplex.Points;
				simplex.iterate(evalFunc, comparator);
				++iteration;
			}
		}

		private class MultivariateFunctionAnonymousInnerClassHelper : MultivariateFunction
		{
			private readonly SimplexOptimizer outerInstance;

			public MultivariateFunctionAnonymousInnerClassHelper(SimplexOptimizer outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual double value(double[] point)
			{
				return outerInstance.computeObjectiveValue(point);
			}
		}

		private class ComparatorAnonymousInnerClassHelper : IComparer<PointValuePair>
		{
			private readonly SimplexOptimizer outerInstance;

			private bool isMinim;

			public ComparatorAnonymousInnerClassHelper(SimplexOptimizer outerInstance, bool isMinim)
			{
				this.outerInstance = outerInstance;
				this.isMinim = isMinim;
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public int compare(final mathlib.optimization.PointValuePair o1, final mathlib.optimization.PointValuePair o2)
			public virtual int Compare(PointValuePair o1, PointValuePair o2)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double v1 = o1.getValue();
				double v1 = o1.Value;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double v2 = o2.getValue();
				double v2 = o2.Value;
				return isMinim ? v1.CompareTo(v2) : v2.CompareTo(v1);
			}
		}
	}

}
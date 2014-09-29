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

namespace mathlib.optimization.univariate
{

	using Incrementor = mathlib.util.Incrementor;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using UnivariateFunction = mathlib.analysis.UnivariateFunction;
	using mathlib.optimization;

	/// <summary>
	/// Provide a default implementation for several functions useful to generic
	/// optimizers.
	/// 
	/// @version $Id: BaseAbstractUnivariateOptimizer.java 1422230 2012-12-15 12:11:13Z erans $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 2.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public abstract class BaseAbstractUnivariateOptimizer : UnivariateOptimizer
	{
		public abstract UnivariatePointValuePair optimize(int maxEval, FUNC f, GoalType goalType, double min, double max, double startValue);
		public abstract UnivariatePointValuePair optimize(int maxEval, FUNC f, GoalType goalType, double min, double max);
		/// <summary>
		/// Convergence checker. </summary>
		private readonly ConvergenceChecker<UnivariatePointValuePair> checker;
		/// <summary>
		/// Evaluations counter. </summary>
		private readonly Incrementor evaluations = new Incrementor();
		/// <summary>
		/// Optimization type </summary>
		private GoalType goal;
		/// <summary>
		/// Lower end of search interval. </summary>
		private double searchMin;
		/// <summary>
		/// Higher end of search interval. </summary>
		private double searchMax;
		/// <summary>
		/// Initial guess . </summary>
		private double searchStart;
		/// <summary>
		/// Function to optimize. </summary>
		private UnivariateFunction function;

		/// <param name="checker"> Convergence checking procedure. </param>
		protected internal BaseAbstractUnivariateOptimizer(ConvergenceChecker<UnivariatePointValuePair> checker)
		{
			this.checker = checker;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int MaxEvaluations
		{
			get
			{
				return evaluations.MaximalCount;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int Evaluations
		{
			get
			{
				return evaluations.Count;
			}
		}

		/// <returns> the optimization type. </returns>
		public virtual GoalType GoalType
		{
			get
			{
				return goal;
			}
		}
		/// <returns> the lower end of the search interval. </returns>
		public virtual double Min
		{
			get
			{
				return searchMin;
			}
		}
		/// <returns> the higher end of the search interval. </returns>
		public virtual double Max
		{
			get
			{
				return searchMax;
			}
		}
		/// <returns> the initial guess. </returns>
		public virtual double StartValue
		{
			get
			{
				return searchStart;
			}
		}

		/// <summary>
		/// Compute the objective function value.
		/// </summary>
		/// <param name="point"> Point at which the objective function must be evaluated. </param>
		/// <returns> the objective function value at specified point. </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of evaluations
		/// is exceeded. </exception>
		protected internal virtual double computeObjectiveValue(double point)
		{
			try
			{
				evaluations.incrementCount();
			}
			catch (MaxCountExceededException e)
			{
				throw new TooManyEvaluationsException(e.Max);
			}
			return function.value(point);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual UnivariatePointValuePair optimize(int maxEval, UnivariateFunction f, GoalType goalType, double min, double max, double startValue)
		{
			// Checks.
			if (f == null)
			{
				throw new NullArgumentException();
			}
			if (goalType == null)
			{
				throw new NullArgumentException();
			}

			// Reset.
			searchMin = min;
			searchMax = max;
			searchStart = startValue;
			goal = goalType;
			function = f;
			evaluations.MaximalCount = maxEval;
			evaluations.resetCount();

			// Perform computation.
			return doOptimize();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual UnivariatePointValuePair optimize(int maxEval, UnivariateFunction f, GoalType goalType, double min, double max)
		{
			return optimize(maxEval, f, goalType, min, max, min + 0.5 * (max - min));
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual ConvergenceChecker<UnivariatePointValuePair> ConvergenceChecker
		{
			get
			{
				return checker;
			}
		}

		/// <summary>
		/// Method for implementing actual optimization algorithms in derived
		/// classes.
		/// </summary>
		/// <returns> the optimum and its corresponding function value. </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of evaluations
		/// is exceeded. </exception>
		protected internal abstract UnivariatePointValuePair doOptimize();
	}

}
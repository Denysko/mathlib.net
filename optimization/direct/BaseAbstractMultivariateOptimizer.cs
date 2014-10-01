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

namespace mathlib.optimization.direct
{

	using Incrementor = mathlib.util.Incrementor;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;
	using MultivariateFunction = mathlib.analysis.MultivariateFunction;
	using mathlib.optimization;
	using mathlib.optimization;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;

	/// <summary>
	/// Base class for implementing optimizers for multivariate scalar functions.
	/// This base class handles the boiler-plate methods associated to thresholds,
	/// evaluations counting, initial guess and simple bounds settings.
	/// </summary>
	/// @param <FUNC> Type of the objective function to be optimized.
	/// 
	/// @version $Id: BaseAbstractMultivariateOptimizer.java 1422313 2012-12-15 18:53:41Z psteitz $ </param>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 2.2 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public abstract class BaseAbstractMultivariateOptimizer<FUNC> : BaseMultivariateOptimizer<FUNC> where FUNC : mathlib.analysis.MultivariateFunction
	{
		/// <summary>
		/// Evaluations counter. </summary>
		protected internal readonly Incrementor evaluations = new Incrementor();
		/// <summary>
		/// Convergence checker. </summary>
		private ConvergenceChecker<PointValuePair> checker;
		/// <summary>
		/// Type of optimization. </summary>
		private GoalType goal;
		/// <summary>
		/// Initial guess. </summary>
		private double[] start;
		/// <summary>
		/// Lower bounds. </summary>
		private double[] lowerBound;
		/// <summary>
		/// Upper bounds. </summary>
		private double[] upperBound;
		/// <summary>
		/// Objective function. </summary>
		private MultivariateFunction function;

		/// <summary>
		/// Simple constructor with default settings.
		/// The convergence check is set to a <seealso cref="SimpleValueChecker"/>. </summary>
		/// @deprecated See <seealso cref="SimpleValueChecker#SimpleValueChecker()"/> 
		[Obsolete]//("See <seealso cref="SimpleValueChecker#SimpleValueChecker()"/>")]
		protected internal BaseAbstractMultivariateOptimizer() : this(new SimpleValueChecker())
		{
		}
		/// <param name="checker"> Convergence checker. </param>
		protected internal BaseAbstractMultivariateOptimizer(ConvergenceChecker<PointValuePair> checker)
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

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual ConvergenceChecker<PointValuePair> ConvergenceChecker
		{
			get
			{
				return checker;
			}
		}

		/// <summary>
		/// Compute the objective function value.
		/// </summary>
		/// <param name="point"> Point at which the objective function must be evaluated. </param>
		/// <returns> the objective function value at the specified point. </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of
		/// evaluations is exceeded. </exception>
		protected internal virtual double computeObjectiveValue(double[] point)
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
		/// {@inheritDoc}
		/// </summary>
		/// @deprecated As of 3.1. Please use
		/// <seealso cref="#optimize(int,MultivariateFunction,GoalType,OptimizationData[])"/>
		/// instead. 
		[Obsolete("As of 3.1. Please use")]
		public virtual PointValuePair optimize(int maxEval, FUNC f, GoalType goalType, double[] startPoint)
		{
			return optimizeInternal(maxEval, f, goalType, new InitialGuess(startPoint));
		}

		/// <summary>
		/// Optimize an objective function.
		/// </summary>
		/// <param name="maxEval"> Allowed number of evaluations of the objective function. </param>
		/// <param name="f"> Objective function. </param>
		/// <param name="goalType"> Optimization type. </param>
		/// <param name="optData"> Optimization data. The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="InitialGuess"/></li>
		///  <li><seealso cref="SimpleBounds"/></li>
		/// </ul> </param>
		/// <returns> the point/value pair giving the optimal value of the objective
		/// function.
		/// @since 3.1 </returns>
		public virtual PointValuePair optimize(int maxEval, FUNC f, GoalType goalType, params OptimizationData[] optData)
		{
			return optimizeInternal(maxEval, f, goalType, optData);
		}

		/// <summary>
		/// Optimize an objective function.
		/// </summary>
		/// <param name="f"> Objective function. </param>
		/// <param name="goalType"> Type of optimization goal: either
		/// <seealso cref="GoalType#MAXIMIZE"/> or <seealso cref="GoalType#MINIMIZE"/>. </param>
		/// <param name="startPoint"> Start point for optimization. </param>
		/// <param name="maxEval"> Maximum number of function evaluations. </param>
		/// <returns> the point/value pair giving the optimal value for objective
		/// function. </returns>
		/// <exception cref="mathlib.exception.DimensionMismatchException">
		/// if the start point dimension is wrong. </exception>
		/// <exception cref="mathlib.exception.TooManyEvaluationsException">
		/// if the maximal number of evaluations is exceeded. </exception>
		/// <exception cref="mathlib.exception.NullArgumentException"> if
		/// any argument is {@code null}. </exception>
		/// @deprecated As of 3.1. Please use
		/// <seealso cref="#optimize(int,MultivariateFunction,GoalType,OptimizationData[])"/>
		/// instead. 
		[Obsolete("As of 3.1. Please use")]
		protected internal virtual PointValuePair optimizeInternal(int maxEval, FUNC f, GoalType goalType, double[] startPoint)
		{
			return optimizeInternal(maxEval, f, goalType, new InitialGuess(startPoint));
		}

		/// <summary>
		/// Optimize an objective function.
		/// </summary>
		/// <param name="maxEval"> Allowed number of evaluations of the objective function. </param>
		/// <param name="f"> Objective function. </param>
		/// <param name="goalType"> Optimization type. </param>
		/// <param name="optData"> Optimization data. The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="InitialGuess"/></li>
		///  <li><seealso cref="SimpleBounds"/></li>
		/// </ul> </param>
		/// <returns> the point/value pair giving the optimal value of the objective
		/// function. </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of
		/// evaluations is exceeded.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected mathlib.optimization.PointValuePair optimizeInternal(int maxEval, FUNC f, mathlib.optimization.GoalType goalType, mathlib.optimization.OptimizationData... optData) throws mathlib.exception.TooManyEvaluationsException
		protected internal virtual PointValuePair optimizeInternal(int maxEval, FUNC f, GoalType goalType, params OptimizationData[] optData)
		{
			// Set internal state.
			evaluations.MaximalCount = maxEval;
			evaluations.resetCount();
			function = f;
			goal = goalType;
			// Retrieve other settings.
			parseOptimizationData(optData);
			// Check input consistency.
			checkParameters();
			// Perform computation.
			return doOptimize();
		}

		/// <summary>
		/// Scans the list of (required and optional) optimization data that
		/// characterize the problem.
		/// </summary>
		/// <param name="optData"> Optimization data. The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="InitialGuess"/></li>
		///  <li><seealso cref="SimpleBounds"/></li>
		/// </ul> </param>
		private void parseOptimizationData(params OptimizationData[] optData)
		{
			// The existing values (as set by the previous call) are reused if
			// not provided in the argument list.
			foreach (OptimizationData data in optData)
			{
				if (data is InitialGuess)
				{
					start = ((InitialGuess) data).InitialGuess;
					continue;
				}
				if (data is SimpleBounds)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.optimization.SimpleBounds bounds = (mathlib.optimization.SimpleBounds) data;
					SimpleBounds bounds = (SimpleBounds) data;
					lowerBound = bounds.Lower;
					upperBound = bounds.Upper;
					continue;
				}
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

		/// <returns> the initial guess. </returns>
		public virtual double[] StartPoint
		{
			get
			{
				return start == null ? null : start.clone();
			}
		}
		/// <returns> the lower bounds.
		/// @since 3.1 </returns>
		public virtual double[] LowerBound
		{
			get
			{
				return lowerBound == null ? null : lowerBound.clone();
			}
		}
		/// <returns> the upper bounds.
		/// @since 3.1 </returns>
		public virtual double[] UpperBound
		{
			get
			{
				return upperBound == null ? null : upperBound.clone();
			}
		}

		/// <summary>
		/// Perform the bulk of the optimization algorithm.
		/// </summary>
		/// <returns> the point/value pair giving the optimal value of the
		/// objective function. </returns>
		protected internal abstract PointValuePair doOptimize();

		/// <summary>
		/// Check parameters consistency.
		/// </summary>
		private void checkParameters()
		{
			if (start != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = start.length;
				int dim = start.Length;
				if (lowerBound != null)
				{
					if (lowerBound.Length != dim)
					{
						throw new DimensionMismatchException(lowerBound.Length, dim);
					}
					for (int i = 0; i < dim; i++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double v = start[i];
						double v = start[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lo = lowerBound[i];
						double lo = lowerBound[i];
						if (v < lo)
						{
							throw new NumberIsTooSmallException(v, lo, true);
						}
					}
				}
				if (upperBound != null)
				{
					if (upperBound.Length != dim)
					{
						throw new DimensionMismatchException(upperBound.Length, dim);
					}
					for (int i = 0; i < dim; i++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double v = start[i];
						double v = start[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double hi = upperBound[i];
						double hi = upperBound[i];
						if (v > hi)
						{
							throw new NumberIsTooLargeException(v, hi, true);
						}
					}
				}

				// If the bounds were not specified, the allowed interval is
				// assumed to be [-inf, +inf].
				if (lowerBound == null)
				{
					lowerBound = new double[dim];
					for (int i = 0; i < dim; i++)
					{
						lowerBound[i] = double.NegativeInfinity;
					}
				}
				if (upperBound == null)
				{
					upperBound = new double[dim];
					for (int i = 0; i < dim; i++)
					{
						upperBound[i] = double.PositiveInfinity;
					}
				}
			}
		}
	}

}
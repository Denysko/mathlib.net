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

namespace org.apache.commons.math3.optimization.direct
{

	using Incrementor = org.apache.commons.math3.util.Incrementor;
	using MaxCountExceededException = org.apache.commons.math3.exception.MaxCountExceededException;
	using TooManyEvaluationsException = org.apache.commons.math3.exception.TooManyEvaluationsException;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using MultivariateVectorFunction = org.apache.commons.math3.analysis.MultivariateVectorFunction;
	using org.apache.commons.math3.optimization;
	using org.apache.commons.math3.optimization;
	using RealMatrix = org.apache.commons.math3.linear.RealMatrix;

	/// <summary>
	/// Base class for implementing optimizers for multivariate scalar functions.
	/// This base class handles the boiler-plate methods associated to thresholds
	/// settings, iterations and evaluations counting.
	/// </summary>
	/// @param <FUNC> the type of the objective function to be optimized
	/// 
	/// @version $Id: BaseAbstractMultivariateVectorOptimizer.java 1499808 2013-07-04 17:00:42Z sebb $ </param>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public abstract class BaseAbstractMultivariateVectorOptimizer<FUNC> : BaseMultivariateVectorOptimizer<FUNC> where FUNC : org.apache.commons.math3.analysis.MultivariateVectorFunction
	{
		/// <summary>
		/// Evaluations counter. </summary>
		protected internal readonly Incrementor evaluations = new Incrementor();
		/// <summary>
		/// Convergence checker. </summary>
		private ConvergenceChecker<PointVectorValuePair> checker;
		/// <summary>
		/// Target value for the objective functions at optimum. </summary>
		private double[] target;
		/// <summary>
		/// Weight matrix. </summary>
		private RealMatrix weightMatrix;
		/// <summary>
		/// Weight for the least squares cost computation.
		/// @deprecated
		/// </summary>
		[Obsolete]
		private double[] weight;
		/// <summary>
		/// Initial guess. </summary>
		private double[] start;
		/// <summary>
		/// Objective function. </summary>
		private FUNC function;

		/// <summary>
		/// Simple constructor with default settings.
		/// The convergence check is set to a <seealso cref="SimpleVectorValueChecker"/>. </summary>
		/// @deprecated See <seealso cref="SimpleVectorValueChecker#SimpleVectorValueChecker()"/> 
		[Obsolete("See <seealso cref="SimpleVectorValueChecker#SimpleVectorValueChecker()"/>")]
		protected internal BaseAbstractMultivariateVectorOptimizer() : this(new SimpleVectorValueChecker())
		{
		}
		/// <param name="checker"> Convergence checker. </param>
		protected internal BaseAbstractMultivariateVectorOptimizer(ConvergenceChecker<PointVectorValuePair> checker)
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
		public virtual ConvergenceChecker<PointVectorValuePair> ConvergenceChecker
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
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of evaluations is
		/// exceeded. </exception>
		protected internal virtual double[] computeObjectiveValue(double[] point)
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
		/// <seealso cref="#optimize(int,MultivariateVectorFunction,OptimizationData[])"/>
		/// instead. 
		[Obsolete("As of 3.1. Please use")]
		public virtual PointVectorValuePair optimize(int maxEval, FUNC f, double[] t, double[] w, double[] startPoint)
		{
			return optimizeInternal(maxEval, f, t, w, startPoint);
		}

		/// <summary>
		/// Optimize an objective function.
		/// </summary>
		/// <param name="maxEval"> Allowed number of evaluations of the objective function. </param>
		/// <param name="f"> Objective function. </param>
		/// <param name="optData"> Optimization data. The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="Target"/></li>
		///  <li><seealso cref="Weight"/></li>
		///  <li><seealso cref="InitialGuess"/></li>
		/// </ul> </param>
		/// <returns> the point/value pair giving the optimal value of the objective
		/// function. </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of
		/// evaluations is exceeded. </exception>
		/// <exception cref="DimensionMismatchException"> if the initial guess, target, and weight
		/// arguments have inconsistent dimensions.
		/// 
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.apache.commons.math3.optimization.PointVectorValuePair optimize(int maxEval, FUNC f, org.apache.commons.math3.optimization.OptimizationData... optData) throws org.apache.commons.math3.exception.TooManyEvaluationsException, org.apache.commons.math3.exception.DimensionMismatchException
		protected internal virtual PointVectorValuePair optimize(int maxEval, FUNC f, params OptimizationData[] optData)
		{
			return optimizeInternal(maxEval, f, optData);
		}

		/// <summary>
		/// Optimize an objective function.
		/// Optimization is considered to be a weighted least-squares minimization.
		/// The cost function to be minimized is
		/// <code>&sum;weight<sub>i</sub>(objective<sub>i</sub> - target<sub>i</sub>)<sup>2</sup></code>
		/// </summary>
		/// <param name="f"> Objective function. </param>
		/// <param name="t"> Target value for the objective functions at optimum. </param>
		/// <param name="w"> Weights for the least squares cost computation. </param>
		/// <param name="startPoint"> Start point for optimization. </param>
		/// <returns> the point/value pair giving the optimal value for objective
		/// function. </returns>
		/// <param name="maxEval"> Maximum number of function evaluations. </param>
		/// <exception cref="org.apache.commons.math3.exception.DimensionMismatchException">
		/// if the start point dimension is wrong. </exception>
		/// <exception cref="org.apache.commons.math3.exception.TooManyEvaluationsException">
		/// if the maximal number of evaluations is exceeded. </exception>
		/// <exception cref="org.apache.commons.math3.exception.NullArgumentException"> if
		/// any argument is {@code null}. </exception>
		/// @deprecated As of 3.1. Please use
		/// <seealso cref="#optimizeInternal(int,MultivariateVectorFunction,OptimizationData[])"/>
		/// instead. 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("As of 3.1. Please use") protected org.apache.commons.math3.optimization.PointVectorValuePair optimizeInternal(final int maxEval, final FUNC f, final double[] t, final double[] w, final double[] startPoint)
		[Obsolete("As of 3.1. Please use")]
		protected internal virtual PointVectorValuePair optimizeInternal(int maxEval, FUNC f, double[] t, double[] w, double[] startPoint)
		{
			// Checks.
			if (f == null)
			{
				throw new NullArgumentException();
			}
			if (t == null)
			{
				throw new NullArgumentException();
			}
			if (w == null)
			{
				throw new NullArgumentException();
			}
			if (startPoint == null)
			{
				throw new NullArgumentException();
			}
			if (t.Length != w.Length)
			{
				throw new DimensionMismatchException(t.Length, w.Length);
			}

			return optimizeInternal(maxEval, f, new Target(t), new Weight(w), new InitialGuess(startPoint));
		}

		/// <summary>
		/// Optimize an objective function.
		/// </summary>
		/// <param name="maxEval"> Allowed number of evaluations of the objective function. </param>
		/// <param name="f"> Objective function. </param>
		/// <param name="optData"> Optimization data. The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="Target"/></li>
		///  <li><seealso cref="Weight"/></li>
		///  <li><seealso cref="InitialGuess"/></li>
		/// </ul> </param>
		/// <returns> the point/value pair giving the optimal value of the objective
		/// function. </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of
		/// evaluations is exceeded. </exception>
		/// <exception cref="DimensionMismatchException"> if the initial guess, target, and weight
		/// arguments have inconsistent dimensions.
		/// 
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.apache.commons.math3.optimization.PointVectorValuePair optimizeInternal(int maxEval, FUNC f, org.apache.commons.math3.optimization.OptimizationData... optData) throws org.apache.commons.math3.exception.TooManyEvaluationsException, org.apache.commons.math3.exception.DimensionMismatchException
		protected internal virtual PointVectorValuePair optimizeInternal(int maxEval, FUNC f, params OptimizationData[] optData)
		{
			// Set internal state.
			evaluations.MaximalCount = maxEval;
			evaluations.resetCount();
			function = f;
			// Retrieve other settings.
			parseOptimizationData(optData);
			// Check input consistency.
			checkParameters();
			// Allow subclasses to reset their own internal state.
			setUp();
			// Perform computation.
			return doOptimize();
		}

		/// <summary>
		/// Gets the initial values of the optimized parameters.
		/// </summary>
		/// <returns> the initial guess. </returns>
		public virtual double[] StartPoint
		{
			get
			{
				return start.clone();
			}
		}

		/// <summary>
		/// Gets the weight matrix of the observations.
		/// </summary>
		/// <returns> the weight matrix.
		/// @since 3.1 </returns>
		public virtual RealMatrix Weight
		{
			get
			{
				return weightMatrix.copy();
			}
		}
		/// <summary>
		/// Gets the observed values to be matched by the objective vector
		/// function.
		/// </summary>
		/// <returns> the target values.
		/// @since 3.1 </returns>
		public virtual double[] Target
		{
			get
			{
				return target.clone();
			}
		}

		/// <summary>
		/// Gets the objective vector function.
		/// Note that this access bypasses the evaluation counter.
		/// </summary>
		/// <returns> the objective vector function.
		/// @since 3.1 </returns>
		protected internal virtual FUNC ObjectiveFunction
		{
			get
			{
				return function;
			}
		}

		/// <summary>
		/// Perform the bulk of the optimization algorithm.
		/// </summary>
		/// <returns> the point/value pair giving the optimal value for the
		/// objective function. </returns>
		protected internal abstract PointVectorValuePair doOptimize();

		/// <returns> a reference to the <seealso cref="#target array"/>. </returns>
		/// @deprecated As of 3.1. 
		[Obsolete("As of 3.1.")]
		protected internal virtual double[] TargetRef
		{
			get
			{
				return target;
			}
		}
		/// <returns> a reference to the <seealso cref="#weight array"/>. </returns>
		/// @deprecated As of 3.1. 
		[Obsolete("As of 3.1.")]
		protected internal virtual double[] WeightRef
		{
			get
			{
				return weight;
			}
		}

		/// <summary>
		/// Method which a subclass <em>must</em> override whenever its internal
		/// state depend on the <seealso cref="OptimizationData input"/> parsed by this base
		/// class.
		/// It will be called after the parsing step performed in the
		/// {@link #optimize(int,MultivariateVectorFunction,OptimizationData[])
		/// optimize} method and just before <seealso cref="#doOptimize()"/>.
		/// 
		/// @since 3.1
		/// </summary>
		protected internal virtual void setUp()
		{
			// XXX Temporary code until the new internal data is used everywhere.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = target.length;
			int dim = target.Length;
			weight = new double[dim];
			for (int i = 0; i < dim; i++)
			{
				weight[i] = weightMatrix.getEntry(i, i);
			}
		}

		/// <summary>
		/// Scans the list of (required and optional) optimization data that
		/// characterize the problem.
		/// </summary>
		/// <param name="optData"> Optimization data. The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="Target"/></li>
		///  <li><seealso cref="Weight"/></li>
		///  <li><seealso cref="InitialGuess"/></li>
		/// </ul> </param>
		private void parseOptimizationData(params OptimizationData[] optData)
		{
			// The existing values (as set by the previous call) are reused if
			// not provided in the argument list.
			foreach (OptimizationData data in optData)
			{
				if (data is Target)
				{
					target = ((Target) data).Target;
					continue;
				}
				if (data is Weight)
				{
					weightMatrix = ((Weight) data).Weight;
					continue;
				}
				if (data is InitialGuess)
				{
					start = ((InitialGuess) data).InitialGuess;
					continue;
				}
			}
		}

		/// <summary>
		/// Check parameters consistency.
		/// </summary>
		/// <exception cref="DimensionMismatchException"> if <seealso cref="#target"/> and
		/// <seealso cref="#weightMatrix"/> have inconsistent dimensions. </exception>
		private void checkParameters()
		{
			if (target.Length != weightMatrix.ColumnDimension)
			{
				throw new DimensionMismatchException(target.Length, weightMatrix.ColumnDimension);
			}
		}
	}

}
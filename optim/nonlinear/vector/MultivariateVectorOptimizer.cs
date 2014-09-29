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

namespace mathlib.optim.nonlinear.vector
{

	using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MultivariateVectorFunction = mathlib.analysis.MultivariateVectorFunction;
	using mathlib.optim;
	using mathlib.optim;
	using RealMatrix = mathlib.linear.RealMatrix;

	/// <summary>
	/// Base class for a multivariate vector function optimizer.
	/// 
	/// @version $Id: MultivariateVectorOptimizer.java 1503290 2013-07-15 15:16:29Z sebb $
	/// @since 3.1
	/// </summary>
	public abstract class MultivariateVectorOptimizer : BaseMultivariateOptimizer<PointVectorValuePair>
	{
		/// <summary>
		/// Target values for the model function at optimum. </summary>
		private double[] target;
		/// <summary>
		/// Weight matrix. </summary>
		private RealMatrix weightMatrix;
		/// <summary>
		/// Model function. </summary>
		private MultivariateVectorFunction model;

		/// <param name="checker"> Convergence checker. </param>
		protected internal MultivariateVectorOptimizer(ConvergenceChecker<PointVectorValuePair> checker) : base(checker)
		{
		}

		/// <summary>
		/// Computes the objective function value.
		/// This method <em>must</em> be called by subclasses to enforce the
		/// evaluation counter limit.
		/// </summary>
		/// <param name="params"> Point at which the objective function must be evaluated. </param>
		/// <returns> the objective function value at the specified point. </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of evaluations
		/// (of the model vector function) is exceeded. </exception>
		protected internal virtual double[] computeObjectiveValue(double[] @params)
		{
			base.incrementEvaluationCount();
			return model.value(@params);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <param name="optData"> Optimization data. In addition to those documented in
		/// {@link BaseMultivariateOptimizer#parseOptimizationData(OptimizationData[])
		/// BaseMultivariateOptimizer}, this method will register the following data:
		/// <ul>
		///  <li><seealso cref="Target"/></li>
		///  <li><seealso cref="Weight"/></li>
		///  <li><seealso cref="ModelFunction"/></li>
		/// </ul> </param>
		/// <returns> {@inheritDoc} </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of
		/// evaluations is exceeded. </exception>
		/// <exception cref="DimensionMismatchException"> if the initial guess, target, and weight
		/// arguments have inconsistent dimensions. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public mathlib.optim.PointVectorValuePair optimize(mathlib.optim.OptimizationData... optData) throws mathlib.exception.TooManyEvaluationsException, mathlib.exception.DimensionMismatchException
		public override PointVectorValuePair optimize(params OptimizationData[] optData)
		{
			// Set up base class and perform computation.
			return base.optimize(optData);
		}

		/// <summary>
		/// Gets the weight matrix of the observations.
		/// </summary>
		/// <returns> the weight matrix. </returns>
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
		/// <returns> the target values. </returns>
		public virtual double[] Target
		{
			get
			{
				return target.clone();
			}
		}

		/// <summary>
		/// Gets the number of observed values.
		/// </summary>
		/// <returns> the length of the target vector. </returns>
		public virtual int TargetSize
		{
			get
			{
				return target.Length;
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
		///  <li><seealso cref="ModelFunction"/></li>
		/// </ul> </param>
		protected internal override void parseOptimizationData(params OptimizationData[] optData)
		{
			// Allow base class to register its own data.
			base.parseOptimizationData(optData);

			// The existing values (as set by the previous call) are reused if
			// not provided in the argument list.
			foreach (OptimizationData data in optData)
			{
				if (data is ModelFunction)
				{
					model = ((ModelFunction) data).ModelFunction;
					continue;
				}
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
			}

			// Check input consistency.
			checkParameters();
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
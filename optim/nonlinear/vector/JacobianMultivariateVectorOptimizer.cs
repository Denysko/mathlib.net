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
namespace mathlib.optim.nonlinear.vector
{

	using MultivariateMatrixFunction = mathlib.analysis.MultivariateMatrixFunction;
	using mathlib.optim;
	using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;

	/// <summary>
	/// Base class for implementing optimizers for multivariate vector
	/// differentiable functions.
	/// It contains boiler-plate code for dealing with Jacobian evaluation.
	/// It assumes that the rows of the Jacobian matrix iterate on the model
	/// functions while the columns iterate on the parameters; thus, the numbers
	/// of rows is equal to the dimension of the <seealso cref="Target"/> while the
	/// number of columns is equal to the dimension of the
	/// <seealso cref="mathlib.optim.InitialGuess InitialGuess"/>.
	/// 
	/// @version $Id: JacobianMultivariateVectorOptimizer.java 1515242 2013-08-18 23:27:29Z erans $
	/// @since 3.1 </summary>
	/// @deprecated All classes and interfaces in this package are deprecated.
	/// The optimizers that were provided here were moved to the
	/// <seealso cref="mathlib.fitting.leastsquares"/> package
	/// (cf. MATH-1008). 
	[Obsolete("All classes and interfaces in this package are deprecated.")]
	public abstract class JacobianMultivariateVectorOptimizer : MultivariateVectorOptimizer
	{
		/// <summary>
		/// Jacobian of the model function.
		/// </summary>
		private MultivariateMatrixFunction jacobian;

		/// <param name="checker"> Convergence checker. </param>
		protected internal JacobianMultivariateVectorOptimizer(ConvergenceChecker<PointVectorValuePair> checker) : base(checker)
		{
		}

		/// <summary>
		/// Computes the Jacobian matrix.
		/// </summary>
		/// <param name="params"> Point at which the Jacobian must be evaluated. </param>
		/// <returns> the Jacobian at the specified point. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected double[][] computeJacobian(final double[] params)
		protected internal virtual double[][] computeJacobian(double[] @params)
		{
			return jacobian.value(@params);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <param name="optData"> Optimization data. In addition to those documented in
		/// <seealso cref="MultivariateVectorOptimizer#optimize(OptimizationData...)"/>
		/// MultivariateOptimizer}, this method will register the following data:
		/// <ul>
		///  <li><seealso cref="ModelFunctionJacobian"/></li>
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
		/// Scans the list of (required and optional) optimization data that
		/// characterize the problem.
		/// </summary>
		/// <param name="optData"> Optimization data.
		/// The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="ModelFunctionJacobian"/></li>
		/// </ul> </param>
		protected internal override void parseOptimizationData(params OptimizationData[] optData)
		{
			// Allow base class to register its own data.
			base.parseOptimizationData(optData);

			// The existing values (as set by the previous call) are reused if
			// not provided in the argument list.
			foreach (OptimizationData data in optData)
			{
				if (data is ModelFunctionJacobian)
				{
					jacobian = ((ModelFunctionJacobian) data).ModelFunctionJacobian;
					// If more data must be parsed, this statement _must_ be
					// changed to "continue".
					break;
				}
			}
		}
	}

}
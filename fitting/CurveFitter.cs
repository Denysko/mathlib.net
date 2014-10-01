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
namespace mathlib.fitting
{

	using MultivariateVectorFunction = mathlib.analysis.MultivariateVectorFunction;
	using MultivariateMatrixFunction = mathlib.analysis.MultivariateMatrixFunction;
	using ParametricUnivariateFunction = mathlib.analysis.ParametricUnivariateFunction;
	using MaxEval = mathlib.optim.MaxEval;
	using InitialGuess = mathlib.optim.InitialGuess;
	using PointVectorValuePair = mathlib.optim.PointVectorValuePair;
	using MultivariateVectorOptimizer = mathlib.optim.nonlinear.vector.MultivariateVectorOptimizer;
	using ModelFunction = mathlib.optim.nonlinear.vector.ModelFunction;
	using ModelFunctionJacobian = mathlib.optim.nonlinear.vector.ModelFunctionJacobian;
	using Target = mathlib.optim.nonlinear.vector.Target;
	using Weight = mathlib.optim.nonlinear.vector.Weight;

	/// <summary>
	/// Fitter for parametric univariate real functions y = f(x).
	/// <br/>
	/// When a univariate real function y = f(x) does depend on some
	/// unknown parameters p<sub>0</sub>, p<sub>1</sub> ... p<sub>n-1</sub>,
	/// this class can be used to find these parameters. It does this
	/// by <em>fitting</em> the curve so it remains very close to a set of
	/// observed points (x<sub>0</sub>, y<sub>0</sub>), (x<sub>1</sub>,
	/// y<sub>1</sub>) ... (x<sub>k-1</sub>, y<sub>k-1</sub>). This fitting
	/// is done by finding the parameters values that minimizes the objective
	/// function &sum;(y<sub>i</sub>-f(x<sub>i</sub>))<sup>2</sup>. This is
	/// really a least squares problem.
	/// </summary>
	/// @param <T> Function to use for the fit.
	/// 
	/// @version $Id: CurveFitter.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0 </param>
	/// @deprecated As of 3.3. Please use <seealso cref="AbstractCurveFitter"/> and
	/// <seealso cref="WeightedObservedPoints"/> instead. 
	[Obsolete]//("As of 3.3. Please use <seealso cref="AbstractCurveFitter"/> and")]
	public class CurveFitter<T> where T : mathlib.analysis.ParametricUnivariateFunction
	{
		/// <summary>
		/// Optimizer to use for the fitting. </summary>
		private readonly MultivariateVectorOptimizer optimizer;
		/// <summary>
		/// Observed points. </summary>
		private readonly IList<WeightedObservedPoint> observations;

		/// <summary>
		/// Simple constructor.
		/// </summary>
		/// <param name="optimizer"> Optimizer to use for the fitting.
		/// @since 3.1 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public CurveFitter(final mathlib.optim.nonlinear.vector.MultivariateVectorOptimizer optimizer)
		public CurveFitter(MultivariateVectorOptimizer optimizer)
		{
			this.optimizer = optimizer;
			observations = new List<WeightedObservedPoint>();
		}

		/// <summary>
		/// Add an observed (x,y) point to the sample with unit weight.
		/// <p>Calling this method is equivalent to call
		/// {@code addObservedPoint(1.0, x, y)}.</p> </summary>
		/// <param name="x"> abscissa of the point </param>
		/// <param name="y"> observed value of the point at x, after fitting we should
		/// have f(x) as close as possible to this value </param>
		/// <seealso cref= #addObservedPoint(double, double, double) </seealso>
		/// <seealso cref= #addObservedPoint(WeightedObservedPoint) </seealso>
		/// <seealso cref= #getObservations() </seealso>
		public virtual void addObservedPoint(double x, double y)
		{
			addObservedPoint(1.0, x, y);
		}

		/// <summary>
		/// Add an observed weighted (x,y) point to the sample. </summary>
		/// <param name="weight"> weight of the observed point in the fit </param>
		/// <param name="x"> abscissa of the point </param>
		/// <param name="y"> observed value of the point at x, after fitting we should
		/// have f(x) as close as possible to this value </param>
		/// <seealso cref= #addObservedPoint(double, double) </seealso>
		/// <seealso cref= #addObservedPoint(WeightedObservedPoint) </seealso>
		/// <seealso cref= #getObservations() </seealso>
		public virtual void addObservedPoint(double weight, double x, double y)
		{
			observations.Add(new WeightedObservedPoint(weight, x, y));
		}

		/// <summary>
		/// Add an observed weighted (x,y) point to the sample. </summary>
		/// <param name="observed"> observed point to add </param>
		/// <seealso cref= #addObservedPoint(double, double) </seealso>
		/// <seealso cref= #addObservedPoint(double, double, double) </seealso>
		/// <seealso cref= #getObservations() </seealso>
		public virtual void addObservedPoint(WeightedObservedPoint observed)
		{
			observations.Add(observed);
		}

		/// <summary>
		/// Get the observed points. </summary>
		/// <returns> observed points </returns>
		/// <seealso cref= #addObservedPoint(double, double) </seealso>
		/// <seealso cref= #addObservedPoint(double, double, double) </seealso>
		/// <seealso cref= #addObservedPoint(WeightedObservedPoint) </seealso>
		public virtual WeightedObservedPoint[] Observations
		{
			get
			{
				return observations.ToArray();
			}
		}

		/// <summary>
		/// Remove all observations.
		/// </summary>
		public virtual void clearObservations()
		{
			observations.Clear();
		}

		/// <summary>
		/// Fit a curve.
		/// This method compute the coefficients of the curve that best
		/// fit the sample of observed points previously given through calls
		/// to the {@link #addObservedPoint(WeightedObservedPoint)
		/// addObservedPoint} method.
		/// </summary>
		/// <param name="f"> parametric function to fit. </param>
		/// <param name="initialGuess"> first guess of the function parameters. </param>
		/// <returns> the fitted parameters. </returns>
		/// <exception cref="mathlib.exception.DimensionMismatchException">
		/// if the start point dimension is wrong. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double[] fit(T f, final double[] initialGuess)
		public virtual double[] fit(T f, double[] initialGuess)
		{
			return fit(int.MaxValue, f, initialGuess);
		}

		/// <summary>
		/// Fit a curve.
		/// This method compute the coefficients of the curve that best
		/// fit the sample of observed points previously given through calls
		/// to the {@link #addObservedPoint(WeightedObservedPoint)
		/// addObservedPoint} method.
		/// </summary>
		/// <param name="f"> parametric function to fit. </param>
		/// <param name="initialGuess"> first guess of the function parameters. </param>
		/// <param name="maxEval"> Maximum number of function evaluations. </param>
		/// <returns> the fitted parameters. </returns>
		/// <exception cref="mathlib.exception.TooManyEvaluationsException">
		/// if the number of allowed evaluations is exceeded. </exception>
		/// <exception cref="mathlib.exception.DimensionMismatchException">
		/// if the start point dimension is wrong.
		/// @since 3.0 </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double[] fit(int maxEval, T f, final double[] initialGuess)
		public virtual double[] fit(int maxEval, T f, double[] initialGuess)
		{
			// Prepare least squares problem.
			double[] target = new double[observations.Count];
			double[] weights = new double[observations.Count];
			int i = 0;
			foreach (WeightedObservedPoint point in observations)
			{
				target[i] = point.Y;
				weights[i] = point.Weight;
				++i;
			}

			// Input to the optimizer: the model and its Jacobian.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TheoreticalValuesFunction model = new TheoreticalValuesFunction(f);
			TheoreticalValuesFunction model = new TheoreticalValuesFunction(this, f);

			// Perform the fit.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.optim.PointVectorValuePair optimum = optimizer.optimize(new mathlib.optim.MaxEval(maxEval), model.getModelFunction(), model.getModelFunctionJacobian(), new mathlib.optim.nonlinear.vector.Target(target), new mathlib.optim.nonlinear.vector.Weight(weights), new mathlib.optim.InitialGuess(initialGuess));
			PointVectorValuePair optimum = optimizer.optimize(new MaxEval(maxEval), model.ModelFunction, model.ModelFunctionJacobian, new Target(target), new Weight(weights), new InitialGuess(initialGuess));
			// Extract the coefficients.
			return optimum.PointRef;
		}

		/// <summary>
		/// Vectorial function computing function theoretical values. </summary>
		private class TheoreticalValuesFunction
		{
			private readonly CurveFitter outerInstance;

			/// <summary>
			/// Function to fit. </summary>
			internal readonly ParametricUnivariateFunction f;

			/// <param name="f"> function to fit. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public TheoreticalValuesFunction(final mathlib.analysis.ParametricUnivariateFunction f)
			public TheoreticalValuesFunction(CurveFitter outerInstance, ParametricUnivariateFunction f)
			{
				this.outerInstance = outerInstance;
				this.f = f;
			}

			/// <returns> the model function values. </returns>
			public virtual ModelFunction ModelFunction
			{
				get
				{
					return new ModelFunction(new MultivariateVectorFunctionAnonymousInnerClassHelper(this));
				}
			}

			private class MultivariateVectorFunctionAnonymousInnerClassHelper : MultivariateVectorFunction
			{
				private readonly TheoreticalValuesFunction outerInstance;

				public MultivariateVectorFunctionAnonymousInnerClassHelper(TheoreticalValuesFunction outerInstance)
				{
					this.outerInstance = outerInstance;
				}

								/// <summary>
								/// {@inheritDoc} </summary>
				public virtual double[] value(double[] point)
				{
					// compute the residuals
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] values = new double[observations.size()];
					double[] values = new double[outerInstance.outerInstance.observations.Count];
					int i = 0;
					foreach (WeightedObservedPoint observed in outerInstance.outerInstance.observations)
					{
						values[i++] = outerInstance.f.value(observed.X, point);
					}

					return values;
				}
			}

			/// <returns> the model function Jacobian. </returns>
			public virtual ModelFunctionJacobian ModelFunctionJacobian
			{
				get
				{
					return new ModelFunctionJacobian(new MultivariateMatrixFunctionAnonymousInnerClassHelper(this));
				}
			}

			private class MultivariateMatrixFunctionAnonymousInnerClassHelper : MultivariateMatrixFunction
			{
				private readonly TheoreticalValuesFunction outerInstance;

				public MultivariateMatrixFunctionAnonymousInnerClassHelper(TheoreticalValuesFunction outerInstance)
				{
					this.outerInstance = outerInstance;
				}

				public virtual double[][] value(double[] point)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] jacobian = new double[observations.size()][];
					double[][] jacobian = new double[outerInstance.outerInstance.observations.Count][];
					int i = 0;
					foreach (WeightedObservedPoint observed in outerInstance.outerInstance.observations)
					{
						jacobian[i++] = outerInstance.f.gradient(observed.X, point);
					}
					return jacobian;
				}
			}
		}
	}

}
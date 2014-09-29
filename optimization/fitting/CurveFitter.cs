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

namespace mathlib.optimization.fitting
{


	using DifferentiableMultivariateVectorFunction = mathlib.analysis.DifferentiableMultivariateVectorFunction;
	using MultivariateMatrixFunction = mathlib.analysis.MultivariateMatrixFunction;
	using ParametricUnivariateFunction = mathlib.analysis.ParametricUnivariateFunction;
	using DerivativeStructure = mathlib.analysis.differentiation.DerivativeStructure;
	using MultivariateDifferentiableVectorFunction = mathlib.analysis.differentiation.MultivariateDifferentiableVectorFunction;

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
	/// @version $Id: CurveFitter.java 1591835 2014-05-02 09:04:01Z tn $ </param>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 2.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public class CurveFitter<T> where T : mathlib.analysis.ParametricUnivariateFunction
		/// <summary>
		/// Optimizer to use for the fitting. </summary>
		/// @deprecated as of 3.1 replaced by <seealso cref="#optimizer"/> 
	{
		[Obsolete("as of 3.1 replaced by <seealso cref="#optimizer"/>")]
		private readonly DifferentiableMultivariateVectorOptimizer oldOptimizer;

		/// <summary>
		/// Optimizer to use for the fitting. </summary>
		private readonly MultivariateDifferentiableVectorOptimizer optimizer;

		/// <summary>
		/// Observed points. </summary>
		private readonly IList<WeightedObservedPoint> observations;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="optimizer"> optimizer to use for the fitting </param>
		/// @deprecated as of 3.1 replaced by <seealso cref="#CurveFitter(MultivariateDifferentiableVectorOptimizer)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.1 replaced by <seealso cref="#CurveFitter(mathlib.optimization.MultivariateDifferentiableVectorOptimizer)"/>") public CurveFitter(final mathlib.optimization.DifferentiableMultivariateVectorOptimizer optimizer)
		[Obsolete("as of 3.1 replaced by <seealso cref="#CurveFitter(mathlib.optimization.MultivariateDifferentiableVectorOptimizer)"/>")]
		public CurveFitter(DifferentiableMultivariateVectorOptimizer optimizer)
		{
			this.oldOptimizer = optimizer;
			this.optimizer = null;
			observations = new List<WeightedObservedPoint>();
		}

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="optimizer"> optimizer to use for the fitting
		/// @since 3.1 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public CurveFitter(final mathlib.optimization.MultivariateDifferentiableVectorOptimizer optimizer)
		public CurveFitter(MultivariateDifferentiableVectorOptimizer optimizer)
		{
			this.oldOptimizer = null;
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
			// prepare least squares problem
			double[] target = new double[observations.Count];
			double[] weights = new double[observations.Count];
			int i = 0;
			foreach (WeightedObservedPoint point in observations)
			{
				target[i] = point.Y;
				weights[i] = point.Weight;
				++i;
			}

			// perform the fit
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.optimization.PointVectorValuePair optimum;
			PointVectorValuePair optimum;
			if (optimizer == null)
			{
				// to be removed in 4.0
				optimum = oldOptimizer.optimize(maxEval, new OldTheoreticalValuesFunction(this, f), target, weights, initialGuess);
			}
			else
			{
				optimum = optimizer.optimize(maxEval, new TheoreticalValuesFunction(this, f), target, weights, initialGuess);
			}

			// extract the coefficients
			return optimum.PointRef;
		}

		/// <summary>
		/// Vectorial function computing function theoretical values. </summary>
		[Obsolete]
		private class OldTheoreticalValuesFunction : DifferentiableMultivariateVectorFunction
		{
			private readonly CurveFitter outerInstance;

			/// <summary>
			/// Function to fit. </summary>
			internal readonly ParametricUnivariateFunction f;

			/// <summary>
			/// Simple constructor. </summary>
			/// <param name="f"> function to fit. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public OldTheoreticalValuesFunction(final mathlib.analysis.ParametricUnivariateFunction f)
			public OldTheoreticalValuesFunction(CurveFitter outerInstance, ParametricUnivariateFunction f)
			{
				this.outerInstance = outerInstance;
				this.f = f;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual MultivariateMatrixFunction jacobian()
			{
				return new MultivariateMatrixFunctionAnonymousInnerClassHelper(this);
			}

			private class MultivariateMatrixFunctionAnonymousInnerClassHelper : MultivariateMatrixFunction
			{
				private readonly OldTheoreticalValuesFunction outerInstance;

				public MultivariateMatrixFunctionAnonymousInnerClassHelper(OldTheoreticalValuesFunction outerInstance)
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

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual double[] value(double[] point)
			{
				// compute the residuals
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] values = new double[observations.size()];
				double[] values = new double[outerInstance.observations.Count];
				int i = 0;
				foreach (WeightedObservedPoint observed in outerInstance.observations)
				{
					values[i++] = f.value(observed.X, point);
				}

				return values;
			}
		}

		/// <summary>
		/// Vectorial function computing function theoretical values. </summary>
		private class TheoreticalValuesFunction : MultivariateDifferentiableVectorFunction
		{
			private readonly CurveFitter outerInstance;


			/// <summary>
			/// Function to fit. </summary>
			internal readonly ParametricUnivariateFunction f;

			/// <summary>
			/// Simple constructor. </summary>
			/// <param name="f"> function to fit. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public TheoreticalValuesFunction(final mathlib.analysis.ParametricUnivariateFunction f)
			public TheoreticalValuesFunction(CurveFitter outerInstance, ParametricUnivariateFunction f)
			{
				this.outerInstance = outerInstance;
				this.f = f;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual double[] value(double[] point)
			{
				// compute the residuals
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] values = new double[observations.size()];
				double[] values = new double[outerInstance.observations.Count];
				int i = 0;
				foreach (WeightedObservedPoint observed in outerInstance.observations)
				{
					values[i++] = f.value(observed.X, point);
				}

				return values;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual DerivativeStructure[] value(DerivativeStructure[] point)
			{

				// extract parameters
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] parameters = new double[point.length];
				double[] parameters = new double[point.Length];
				for (int k = 0; k < point.Length; ++k)
				{
					parameters[k] = point[k].Value;
				}

				// compute the residuals
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.analysis.differentiation.DerivativeStructure[] values = new mathlib.analysis.differentiation.DerivativeStructure[observations.size()];
				DerivativeStructure[] values = new DerivativeStructure[outerInstance.observations.Count];
				int i = 0;
				foreach (WeightedObservedPoint observed in outerInstance.observations)
				{

					// build the DerivativeStructure by adding first the value as a constant
					// and then adding derivatives
					DerivativeStructure vi = new DerivativeStructure(point.Length, 1, f.value(observed.X, parameters));
					for (int k = 0; k < point.Length; ++k)
					{
						vi = vi.add(new DerivativeStructure(point.Length, 1, k, 0.0));
					}

					values[i++] = vi;

				}

				return values;
			}

		}

	}

}
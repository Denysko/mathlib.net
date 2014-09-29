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
	using LeastSquaresOptimizer = mathlib.fitting.leastsquares.LeastSquaresOptimizer;
	using LeastSquaresProblem = mathlib.fitting.leastsquares.LeastSquaresProblem;
	using LevenbergMarquardtOptimizer = mathlib.fitting.leastsquares.LevenbergMarquardtOptimizer;

	/// <summary>
	/// Base class that contains common code for fitting parametric univariate
	/// real functions <code>y = f(p<sub>i</sub>;x)</code>, where {@code x} is
	/// the independent variable and the <code>p<sub>i</sub></code> are the
	/// <em>parameters</em>.
	/// <br/>
	/// A fitter will find the optimal values of the parameters by
	/// <em>fitting</em> the curve so it remains very close to a set of
	/// {@code N} observed points <code>(x<sub>k</sub>, y<sub>k</sub>)</code>,
	/// {@code 0 <= k < N}.
	/// <br/>
	/// An algorithm usually performs the fit by finding the parameter
	/// values that minimizes the objective function
	/// <pre><code>
	///  &sum;y<sub>k</sub> - f(x<sub>k</sub>)<sup>2</sup>,
	/// </code></pre>
	/// which is actually a least-squares problem.
	/// This class contains boilerplate code for calling the
	/// <seealso cref="#fit(Collection)"/> method for obtaining the parameters.
	/// The problem setup, such as the choice of optimization algorithm
	/// for fitting a specific function is delegated to subclasses.
	/// 
	/// @version $Id: AbstractCurveFitter.java 1569358 2014-02-18 14:33:20Z luc $
	/// @since 3.3
	/// </summary>
	public abstract class AbstractCurveFitter
	{
		/// <summary>
		/// Fits a curve.
		/// This method computes the coefficients of the curve that best
		/// fit the sample of observed points.
		/// </summary>
		/// <param name="points"> Observations. </param>
		/// <returns> the fitted parameters. </returns>
		public virtual double[] fit(ICollection<WeightedObservedPoint> points)
		{
			// Perform the fit.
			return Optimizer.optimize(getProblem(points)).Point.toArray();
		}

		/// <summary>
		/// Creates an optimizer set up to fit the appropriate curve.
		/// <p>
		/// The default implementation uses a {@link LevenbergMarquardtOptimizer
		/// Levenberg-Marquardt} optimizer.
		/// </p> </summary>
		/// <returns> the optimizer to use for fitting the curve to the
		/// given {@code points}. </returns>
		protected internal virtual LeastSquaresOptimizer Optimizer
		{
			get
			{
				return new LevenbergMarquardtOptimizer();
			}
		}

		/// <summary>
		/// Creates a least squares problem corresponding to the appropriate curve.
		/// </summary>
		/// <param name="points"> Sample points. </param>
		/// <returns> the least squares problem to use for fitting the curve to the
		/// given {@code points}. </returns>
		protected internal abstract LeastSquaresProblem getProblem(ICollection<WeightedObservedPoint> points);

		/// <summary>
		/// Vector function for computing function theoretical values.
		/// </summary>
		protected internal class TheoreticalValuesFunction
		{
			/// <summary>
			/// Function to fit. </summary>
			internal readonly ParametricUnivariateFunction f;
			/// <summary>
			/// Observations. </summary>
			internal readonly double[] points;

			/// <param name="f"> function to fit. </param>
			/// <param name="observations"> Observations. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public TheoreticalValuesFunction(final mathlib.analysis.ParametricUnivariateFunction f, final java.util.Collection<WeightedObservedPoint> observations)
			public TheoreticalValuesFunction(ParametricUnivariateFunction f, ICollection<WeightedObservedPoint> observations)
			{
				this.f = f;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = observations.size();
				int len = observations.Count;
				this.points = new double[len];
				int i = 0;
				foreach (WeightedObservedPoint obs in observations)
				{
					this.points[i++] = obs.X;
				}
			}

			/// <returns> the model function values. </returns>
			public virtual MultivariateVectorFunction ModelFunction
			{
				get
				{
					return new MultivariateVectorFunctionAnonymousInnerClassHelper(this);
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
				public virtual double[] value(double[] p)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = points.length;
					int len = outerInstance.points.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] values = new double[len];
					double[] values = new double[len];
					for (int i = 0; i < len; i++)
					{
						values[i] = outerInstance.f.value(outerInstance.points[i], p);
					}

					return values;
				}
			}

			/// <returns> the model function Jacobian. </returns>
			public virtual MultivariateMatrixFunction ModelFunctionJacobian
			{
				get
				{
					return new MultivariateMatrixFunctionAnonymousInnerClassHelper(this);
				}
			}

			private class MultivariateMatrixFunctionAnonymousInnerClassHelper : MultivariateMatrixFunction
			{
				private readonly TheoreticalValuesFunction outerInstance;

				public MultivariateMatrixFunctionAnonymousInnerClassHelper(TheoreticalValuesFunction outerInstance)
				{
					this.outerInstance = outerInstance;
				}

				public virtual double[][] value(double[] p)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = points.length;
					int len = outerInstance.points.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] jacobian = new double[len][];
					double[][] jacobian = new double[len][];
					for (int i = 0; i < len; i++)
					{
						jacobian[i] = outerInstance.f.gradient(outerInstance.points[i], p);
					}
					return jacobian;
				}
			}
		}
	}

}
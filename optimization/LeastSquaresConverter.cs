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

	using MultivariateFunction = mathlib.analysis.MultivariateFunction;
	using MultivariateVectorFunction = mathlib.analysis.MultivariateVectorFunction;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using RealMatrix = mathlib.linear.RealMatrix;

	/// <summary>
	/// This class converts {@link MultivariateVectorFunction vectorial
	/// objective functions} to <seealso cref="MultivariateFunction scalar objective functions"/>
	/// when the goal is to minimize them.
	/// <p>
	/// This class is mostly used when the vectorial objective function represents
	/// a theoretical result computed from a point set applied to a model and
	/// the models point must be adjusted to fit the theoretical result to some
	/// reference observations. The observations may be obtained for example from
	/// physical measurements whether the model is built from theoretical
	/// considerations.
	/// </p>
	/// <p>
	/// This class computes a possibly weighted squared sum of the residuals, which is
	/// a scalar value. The residuals are the difference between the theoretical model
	/// (i.e. the output of the vectorial objective function) and the observations. The
	/// class implements the <seealso cref="MultivariateFunction"/> interface and can therefore be
	/// minimized by any optimizer supporting scalar objectives functions.This is one way
	/// to perform a least square estimation. There are other ways to do this without using
	/// this converter, as some optimization algorithms directly support vectorial objective
	/// functions.
	/// </p>
	/// <p>
	/// This class support combination of residuals with or without weights and correlations.
	/// </p>
	/// </summary>
	/// <seealso cref= MultivariateFunction </seealso>
	/// <seealso cref= MultivariateVectorFunction
	/// @version $Id: LeastSquaresConverter.java 1591835 2014-05-02 09:04:01Z tn $ </seealso>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 2.0 

	[Obsolete]
	public class LeastSquaresConverter : MultivariateFunction
	{

		/// <summary>
		/// Underlying vectorial function. </summary>
		private readonly MultivariateVectorFunction function;

		/// <summary>
		/// Observations to be compared to objective function to compute residuals. </summary>
		private readonly double[] observations;

		/// <summary>
		/// Optional weights for the residuals. </summary>
		private readonly double[] weights;

		/// <summary>
		/// Optional scaling matrix (weight and correlations) for the residuals. </summary>
		private readonly RealMatrix scale;

		/// <summary>
		/// Build a simple converter for uncorrelated residuals with the same weight. </summary>
		/// <param name="function"> vectorial residuals function to wrap </param>
		/// <param name="observations"> observations to be compared to objective function to compute residuals </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LeastSquaresConverter(final mathlib.analysis.MultivariateVectorFunction function, final double[] observations)
		public LeastSquaresConverter(MultivariateVectorFunction function, double[] observations)
		{
			this.function = function;
			this.observations = observations.clone();
			this.weights = null;
			this.scale = null;
		}

		/// <summary>
		/// Build a simple converter for uncorrelated residuals with the specific weights.
		/// <p>
		/// The scalar objective function value is computed as:
		/// <pre>
		/// objective = &sum;weight<sub>i</sub>(observation<sub>i</sub>-objective<sub>i</sub>)<sup>2</sup>
		/// </pre>
		/// </p>
		/// <p>
		/// Weights can be used for example to combine residuals with different standard
		/// deviations. As an example, consider a residuals array in which even elements
		/// are angular measurements in degrees with a 0.01&deg; standard deviation and
		/// odd elements are distance measurements in meters with a 15m standard deviation.
		/// In this case, the weights array should be initialized with value
		/// 1.0/(0.01<sup>2</sup>) in the even elements and 1.0/(15.0<sup>2</sup>) in the
		/// odd elements (i.e. reciprocals of variances).
		/// </p>
		/// <p>
		/// The array computed by the objective function, the observations array and the
		/// weights array must have consistent sizes or a <seealso cref="DimensionMismatchException"/>
		/// will be triggered while computing the scalar objective.
		/// </p> </summary>
		/// <param name="function"> vectorial residuals function to wrap </param>
		/// <param name="observations"> observations to be compared to objective function to compute residuals </param>
		/// <param name="weights"> weights to apply to the residuals </param>
		/// <exception cref="DimensionMismatchException"> if the observations vector and the weights
		/// vector dimensions do not match (objective function dimension is checked only when
		/// the <seealso cref="#value(double[])"/> method is called) </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LeastSquaresConverter(final mathlib.analysis.MultivariateVectorFunction function, final double[] observations, final double[] weights)
		public LeastSquaresConverter(MultivariateVectorFunction function, double[] observations, double[] weights)
		{
			if (observations.Length != weights.Length)
			{
				throw new DimensionMismatchException(observations.Length, weights.Length);
			}
			this.function = function;
			this.observations = observations.clone();
			this.weights = weights.clone();
			this.scale = null;
		}

		/// <summary>
		/// Build a simple converter for correlated residuals with the specific weights.
		/// <p>
		/// The scalar objective function value is computed as:
		/// <pre>
		/// objective = y<sup>T</sup>y with y = scale&times;(observation-objective)
		/// </pre>
		/// </p>
		/// <p>
		/// The array computed by the objective function, the observations array and the
		/// the scaling matrix must have consistent sizes or a <seealso cref="DimensionMismatchException"/>
		/// will be triggered while computing the scalar objective.
		/// </p> </summary>
		/// <param name="function"> vectorial residuals function to wrap </param>
		/// <param name="observations"> observations to be compared to objective function to compute residuals </param>
		/// <param name="scale"> scaling matrix </param>
		/// <exception cref="DimensionMismatchException"> if the observations vector and the scale
		/// matrix dimensions do not match (objective function dimension is checked only when
		/// the <seealso cref="#value(double[])"/> method is called) </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LeastSquaresConverter(final mathlib.analysis.MultivariateVectorFunction function, final double[] observations, final mathlib.linear.RealMatrix scale)
		public LeastSquaresConverter(MultivariateVectorFunction function, double[] observations, RealMatrix scale)
		{
			if (observations.Length != scale.ColumnDimension)
			{
				throw new DimensionMismatchException(observations.Length, scale.ColumnDimension);
			}
			this.function = function;
			this.observations = observations.clone();
			this.weights = null;
			this.scale = scale.copy();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double value(final double[] point)
		public virtual double value(double[] point)
		{
			// compute residuals
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] residuals = function.value(point);
			double[] residuals = function.value(point);
			if (residuals.Length != observations.Length)
			{
				throw new DimensionMismatchException(residuals.Length, observations.Length);
			}
			for (int i = 0; i < residuals.Length; ++i)
			{
				residuals[i] -= observations[i];
			}

			// compute sum of squares
			double sumSquares = 0;
			if (weights != null)
			{
				for (int i = 0; i < residuals.Length; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ri = residuals[i];
					double ri = residuals[i];
					sumSquares += weights[i] * ri * ri;
				}
			}
			else if (scale != null)
			{
				foreach (double yi in scale.operate(residuals))
				{
					sumSquares += yi * yi;
				}
			}
			else
			{
				foreach (double ri in residuals)
				{
					sumSquares += ri * ri;
				}
			}

			return sumSquares;
		}
	}

}
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
namespace mathlib.fitting
{

	using PolynomialFunction = mathlib.analysis.polynomials.PolynomialFunction;
	using MultivariateVectorOptimizer = mathlib.optim.nonlinear.vector.MultivariateVectorOptimizer;

	/// <summary>
	/// Polynomial fitting is a very simple case of <seealso cref="CurveFitter curve fitting"/>.
	/// The estimated coefficients are the polynomial coefficients (see the
	/// <seealso cref="#fit(double[]) fit"/> method).
	/// 
	/// @version $Id: PolynomialFitter.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0 </summary>
	/// @deprecated As of 3.3. Please use <seealso cref="PolynomialCurveFitter"/> and
	/// <seealso cref="WeightedObservedPoints"/> instead. 
	[Obsolete("As of 3.3. Please use <seealso cref="PolynomialCurveFitter"/> and")]
	public class PolynomialFitter : CurveFitter<PolynomialFunction.Parametric>
	{
		/// <summary>
		/// Simple constructor.
		/// </summary>
		/// <param name="optimizer"> Optimizer to use for the fitting. </param>
		public PolynomialFitter(MultivariateVectorOptimizer optimizer) : base(optimizer)
		{
		}

		/// <summary>
		/// Get the coefficients of the polynomial fitting the weighted data points.
		/// The degree of the fitting polynomial is {@code guess.length - 1}.
		/// </summary>
		/// <param name="guess"> First guess for the coefficients. They must be sorted in
		/// increasing order of the polynomial's degree. </param>
		/// <param name="maxEval"> Maximum number of evaluations of the polynomial. </param>
		/// <returns> the coefficients of the polynomial that best fits the observed points. </returns>
		/// <exception cref="mathlib.exception.TooManyEvaluationsException"> if
		/// the number of evaluations exceeds {@code maxEval}. </exception>
		/// <exception cref="mathlib.exception.ConvergenceException">
		/// if the algorithm failed to converge. </exception>
		public virtual double[] fit(int maxEval, double[] guess)
		{
			return fit(maxEval, new PolynomialFunction.Parametric(), guess);
		}

		/// <summary>
		/// Get the coefficients of the polynomial fitting the weighted data points.
		/// The degree of the fitting polynomial is {@code guess.length - 1}.
		/// </summary>
		/// <param name="guess"> First guess for the coefficients. They must be sorted in
		/// increasing order of the polynomial's degree. </param>
		/// <returns> the coefficients of the polynomial that best fits the observed points. </returns>
		/// <exception cref="mathlib.exception.ConvergenceException">
		/// if the algorithm failed to converge. </exception>
		public virtual double[] fit(double[] guess)
		{
			return fit(new PolynomialFunction.Parametric(), guess);
		}
	}

}
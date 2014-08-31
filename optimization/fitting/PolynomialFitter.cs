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

namespace org.apache.commons.math3.optimization.fitting
{

	using PolynomialFunction = org.apache.commons.math3.analysis.polynomials.PolynomialFunction;

	/// <summary>
	/// Polynomial fitting is a very simple case of <seealso cref="CurveFitter curve fitting"/>.
	/// The estimated coefficients are the polynomial coefficients (see the
	/// <seealso cref="#fit(double[]) fit"/> method).
	/// 
	/// @version $Id: PolynomialFitter.java 1422313 2012-12-15 18:53:41Z psteitz $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 2.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public class PolynomialFitter : CurveFitter<PolynomialFunction.Parametric>
		/// <summary>
		/// Polynomial degree.
		/// @deprecated
		/// </summary>
	{
		[Obsolete]
		private readonly int degree;

		/// <summary>
		/// Simple constructor.
		/// <p>The polynomial fitter built this way are complete polynomials,
		/// ie. a n-degree polynomial has n+1 coefficients.</p>
		/// </summary>
		/// <param name="degree"> Maximal degree of the polynomial. </param>
		/// <param name="optimizer"> Optimizer to use for the fitting. </param>
		/// @deprecated Since 3.1 (to be removed in 4.0). Please use
		/// <seealso cref="#PolynomialFitter(DifferentiableMultivariateVectorOptimizer)"/> instead. 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("Since 3.1 (to be removed in 4.0). Please use") public PolynomialFitter(int degree, final org.apache.commons.math3.optimization.DifferentiableMultivariateVectorOptimizer optimizer)
		[Obsolete("Since 3.1 (to be removed in 4.0). Please use")]
		public PolynomialFitter(int degree, DifferentiableMultivariateVectorOptimizer optimizer) : base(optimizer)
		{
			this.degree = degree;
		}

		/// <summary>
		/// Simple constructor.
		/// </summary>
		/// <param name="optimizer"> Optimizer to use for the fitting.
		/// @since 3.1 </param>
		public PolynomialFitter(DifferentiableMultivariateVectorOptimizer optimizer) : base(optimizer)
		{
			degree = -1; // To avoid compilation error until the instance variable is removed.
		}

		/// <summary>
		/// Get the polynomial fitting the weighted (x, y) points.
		/// </summary>
		/// <returns> the coefficients of the polynomial that best fits the observed points. </returns>
		/// <exception cref="org.apache.commons.math3.exception.ConvergenceException">
		/// if the algorithm failed to converge. </exception>
		/// @deprecated Since 3.1 (to be removed in 4.0). Please use <seealso cref="#fit(double[])"/> instead. 
		[Obsolete("Since 3.1 (to be removed in 4.0). Please use <seealso cref="#fit(double[])"/> instead.")]
		public virtual double[] fit()
		{
			return fit(new PolynomialFunction.Parametric(), new double[degree + 1]);
		}

		/// <summary>
		/// Get the coefficients of the polynomial fitting the weighted data points.
		/// The degree of the fitting polynomial is {@code guess.length - 1}.
		/// </summary>
		/// <param name="guess"> First guess for the coefficients. They must be sorted in
		/// increasing order of the polynomial's degree. </param>
		/// <param name="maxEval"> Maximum number of evaluations of the polynomial. </param>
		/// <returns> the coefficients of the polynomial that best fits the observed points. </returns>
		/// <exception cref="org.apache.commons.math3.exception.TooManyEvaluationsException"> if
		/// the number of evaluations exceeds {@code maxEval}. </exception>
		/// <exception cref="org.apache.commons.math3.exception.ConvergenceException">
		/// if the algorithm failed to converge.
		/// @since 3.1 </exception>
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
		/// <exception cref="org.apache.commons.math3.exception.ConvergenceException">
		/// if the algorithm failed to converge.
		/// @since 3.1 </exception>
		public virtual double[] fit(double[] guess)
		{
			return fit(new PolynomialFunction.Parametric(), guess);
		}
	}

}
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

	using PolynomialFunction = mathlib.analysis.polynomials.PolynomialFunction;
	using MathInternalError = mathlib.exception.MathInternalError;
	using LeastSquaresBuilder = mathlib.fitting.leastsquares.LeastSquaresBuilder;
	using LeastSquaresProblem = mathlib.fitting.leastsquares.LeastSquaresProblem;
	using DiagonalMatrix = mathlib.linear.DiagonalMatrix;

	/// <summary>
	/// Fits points to a {@link
	/// mathlib.analysis.polynomials.PolynomialFunction.Parametric polynomial}
	/// function.
	/// <br/>
	/// The size of the <seealso cref="#withStartPoint(double[]) initial guess"/> array defines the
	/// degree of the polynomial to be fitted.
	/// They must be sorted in increasing order of the polynomial's degree.
	/// The optimal values of the coefficients will be returned in the same order.
	/// 
	/// @version $Id: PolynomialCurveFitter.java 1571640 2014-02-25 10:27:21Z erans $
	/// @since 3.3
	/// </summary>
	public class PolynomialCurveFitter : AbstractCurveFitter
	{
		/// <summary>
		/// Parametric function to be fitted. </summary>
		private static readonly PolynomialFunction.Parametric FUNCTION = new PolynomialFunction.Parametric();
		/// <summary>
		/// Initial guess. </summary>
		private readonly double[] initialGuess;
		/// <summary>
		/// Maximum number of iterations of the optimization algorithm. </summary>
		private readonly int maxIter;

		/// <summary>
		/// Contructor used by the factory methods.
		/// </summary>
		/// <param name="initialGuess"> Initial guess. </param>
		/// <param name="maxIter"> Maximum number of iterations of the optimization algorithm. </param>
		/// <exception cref="MathInternalError"> if {@code initialGuess} is {@code null}. </exception>
		private PolynomialCurveFitter(double[] initialGuess, int maxIter)
		{
			this.initialGuess = initialGuess;
			this.maxIter = maxIter;
		}

		/// <summary>
		/// Creates a default curve fitter.
		/// Zero will be used as initial guess for the coefficients, and the maximum
		/// number of iterations of the optimization algorithm is set to
		/// <seealso cref="Integer#MAX_VALUE"/>.
		/// </summary>
		/// <param name="degree"> Degree of the polynomial to be fitted. </param>
		/// <returns> a curve fitter.
		/// </returns>
		/// <seealso cref= #withStartPoint(double[]) </seealso>
		/// <seealso cref= #withMaxIterations(int) </seealso>
		public static PolynomialCurveFitter create(int degree)
		{
			return new PolynomialCurveFitter(new double[degree + 1], int.MaxValue);
		}

		/// <summary>
		/// Configure the start point (initial guess). </summary>
		/// <param name="newStart"> new start point (initial guess) </param>
		/// <returns> a new instance. </returns>
		public virtual PolynomialCurveFitter withStartPoint(double[] newStart)
		{
			return new PolynomialCurveFitter(newStart.clone(), maxIter);
		}

		/// <summary>
		/// Configure the maximum number of iterations. </summary>
		/// <param name="newMaxIter"> maximum number of iterations </param>
		/// <returns> a new instance. </returns>
		public virtual PolynomialCurveFitter withMaxIterations(int newMaxIter)
		{
			return new PolynomialCurveFitter(initialGuess, newMaxIter);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override LeastSquaresProblem getProblem(ICollection<WeightedObservedPoint> observations)
		{
			// Prepare least-squares problem.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = observations.size();
			int len = observations.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] target = new double[len];
			double[] target = new double[len];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] weights = new double[len];
			double[] weights = new double[len];

			int i = 0;
			foreach (WeightedObservedPoint obs in observations)
			{
				target[i] = obs.Y;
				weights[i] = obs.Weight;
				++i;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final AbstractCurveFitter.TheoreticalValuesFunction model = new AbstractCurveFitter.TheoreticalValuesFunction(FUNCTION, observations);
			AbstractCurveFitter.TheoreticalValuesFunction model = new AbstractCurveFitter.TheoreticalValuesFunction(FUNCTION, observations);

			if (initialGuess == null)
			{
				throw new MathInternalError();
			}

			// Return a new least squares problem set up to fit a polynomial curve to the
			// observed points.
			return (new LeastSquaresBuilder()).maxEvaluations(int.MaxValue).maxIterations(maxIter).start(initialGuess).target(target).weight(new DiagonalMatrix(weights)).model(model.ModelFunction, model.ModelFunctionJacobian).build();

		}

	}

}
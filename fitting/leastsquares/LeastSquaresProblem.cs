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
namespace mathlib.fitting.leastsquares
{

	using RealMatrix = mathlib.linear.RealMatrix;
	using RealVector = mathlib.linear.RealVector;
	using mathlib.optim;

	/// <summary>
	/// The data necessary to define a non-linear least squares problem.
	/// <p>
	/// Includes the observed values, computed model function, and
	/// convergence/divergence criteria. Weights are implicit in {@link
	/// Evaluation#getResiduals()} and <seealso cref="Evaluation#getJacobian()"/>.
	/// </p>
	/// <p>
	/// Instances are typically either created progressively using a {@link
	/// LeastSquaresBuilder builder} or created at once using a {@link LeastSquaresFactory
	/// factory}.
	/// </p> </summary>
	/// <seealso cref= LeastSquaresBuilder </seealso>
	/// <seealso cref= LeastSquaresFactory </seealso>
	/// <seealso cref= LeastSquaresAdapter
	/// 
	/// @version $Id: LeastSquaresProblem.java 1571306 2014-02-24 14:57:44Z luc $
	/// @since 3.3 </seealso>
	public interface LeastSquaresProblem : OptimizationProblem<LeastSquaresProblem_Evaluation>
	{

		/// <summary>
		/// Gets the initial guess.
		/// </summary>
		/// <returns> the initial guess values. </returns>
		RealVector Start {get;}

		/// <summary>
		/// Get the number of observations (rows in the Jacobian) in this problem.
		/// </summary>
		/// <returns> the number of scalar observations </returns>
		int ObservationSize {get;}

		/// <summary>
		/// Get the number of parameters (columns in the Jacobian) in this problem.
		/// </summary>
		/// <returns> the number of scalar parameters </returns>
		int ParameterSize {get;}

		/// <summary>
		/// Evaluate the model at the specified point.
		/// 
		/// </summary>
		/// <param name="point"> the parameter values. </param>
		/// <returns> the model's value and derivative at the given point. </returns>
		/// <exception cref="mathlib.exception.TooManyEvaluationsException">
		///          if the maximal number of evaluations (of the model vector function) is
		///          exceeded. </exception>
		LeastSquaresProblem_Evaluation evaluate(RealVector point);

		/// <summary>
		/// An evaluation of a <seealso cref="LeastSquaresProblem"/> at a particular point. This class
		/// also computes several quantities derived from the value and its Jacobian.
		/// </summary>
	}

	public interface LeastSquaresProblem_Evaluation
	{

		/// <summary>
		/// Get the covariance matrix of the optimized parameters. <br/> Note that this
		/// operation involves the inversion of the <code>J<sup>T</sup>J</code> matrix,
		/// where {@code J} is the Jacobian matrix. The {@code threshold} parameter is a
		/// way for the caller to specify that the result of this computation should be
		/// considered meaningless, and thus trigger an exception.
		/// 
		/// </summary>
		/// <param name="threshold"> Singularity threshold. </param>
		/// <returns> the covariance matrix. </returns>
		/// <exception cref="mathlib.linear.SingularMatrixException">
		///          if the covariance matrix cannot be computed (singular problem). </exception>
		RealMatrix getCovariances(double threshold);

		/// <summary>
		/// Get an estimate of the standard deviation of the parameters. The returned
		/// values are the square root of the diagonal coefficients of the covariance
		/// matrix, {@code sd(a[i]) ~= sqrt(C[i][i])}, where {@code a[i]} is the optimized
		/// value of the {@code i}-th parameter, and {@code C} is the covariance matrix.
		/// 
		/// </summary>
		/// <param name="covarianceSingularityThreshold"> Singularity threshold (see {@link
		///                                       #getCovariances(double) computeCovariances}). </param>
		/// <returns> an estimate of the standard deviation of the optimized parameters </returns>
		/// <exception cref="mathlib.linear.SingularMatrixException">
		///          if the covariance matrix cannot be computed. </exception>
		RealVector getSigma(double covarianceSingularityThreshold);

		/// <summary>
		/// Get the normalized cost. It is the square-root of the sum of squared of
		/// the residuals, divided by the number of measurements.
		/// </summary>
		/// <returns> the cost. </returns>
		double RMS {get;}

		/// <summary>
		/// Get the weighted Jacobian matrix.
		/// </summary>
		/// <returns> the weighted Jacobian: W<sup>1/2</sup> J. </returns>
		/// <exception cref="mathlib.exception.DimensionMismatchException">
		/// if the Jacobian dimension does not match problem dimension. </exception>
		RealMatrix Jacobian {get;}

		/// <summary>
		/// Get the cost.
		/// </summary>
		/// <returns> the cost. </returns>
		/// <seealso cref= #getResiduals() </seealso>
		double Cost {get;}

		/// <summary>
		/// Get the weighted residuals. The residual is the difference between the
		/// observed (target) values and the model (objective function) value. There is one
		/// residual for each element of the vector-valued function. The raw residuals are
		/// then multiplied by the square root of the weight matrix.
		/// </summary>
		/// <returns> the weighted residuals: W<sup>1/2</sup> K. </returns>
		/// <exception cref="mathlib.exception.DimensionMismatchException">
		/// if the residuals have the wrong length. </exception>
		RealVector Residuals {get;}

		/// <summary>
		/// Get the abscissa (independent variables) of this evaluation.
		/// </summary>
		/// <returns> the point provided to <seealso cref="#evaluate(RealVector)"/>. </returns>
		RealVector Point {get;}
	}

}
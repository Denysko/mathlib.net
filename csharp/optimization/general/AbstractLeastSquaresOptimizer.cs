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

namespace org.apache.commons.math3.optimization.general
{

	using DifferentiableMultivariateVectorFunction = org.apache.commons.math3.analysis.DifferentiableMultivariateVectorFunction;
	using FunctionUtils = org.apache.commons.math3.analysis.FunctionUtils;
	using DerivativeStructure = org.apache.commons.math3.analysis.differentiation.DerivativeStructure;
	using MultivariateDifferentiableVectorFunction = org.apache.commons.math3.analysis.differentiation.MultivariateDifferentiableVectorFunction;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using ArrayRealVector = org.apache.commons.math3.linear.ArrayRealVector;
	using RealMatrix = org.apache.commons.math3.linear.RealMatrix;
	using DiagonalMatrix = org.apache.commons.math3.linear.DiagonalMatrix;
	using DecompositionSolver = org.apache.commons.math3.linear.DecompositionSolver;
	using MatrixUtils = org.apache.commons.math3.linear.MatrixUtils;
	using QRDecomposition = org.apache.commons.math3.linear.QRDecomposition;
	using EigenDecomposition = org.apache.commons.math3.linear.EigenDecomposition;
	using org.apache.commons.math3.optimization;
	using org.apache.commons.math3.optimization.direct;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Base class for implementing least squares optimizers.
	/// It handles the boilerplate methods associated to thresholds settings,
	/// Jacobian and error estimation.
	/// <br/>
	/// This class constructs the Jacobian matrix of the function argument in method
	/// {@link BaseAbstractMultivariateVectorOptimizer#optimize(int,
	/// org.apache.commons.math3.analysis.MultivariateVectorFunction,OptimizationData[])
	/// optimize} and assumes that the rows of that matrix iterate on the model
	/// functions while the columns iterate on the parameters; thus, the numbers
	/// of rows is equal to the dimension of the
	/// <seealso cref="org.apache.commons.math3.optimization.Target Target"/> while
	/// the number of columns is equal to the dimension of the
	/// <seealso cref="org.apache.commons.math3.optimization.InitialGuess InitialGuess"/>.
	/// 
	/// @version $Id: AbstractLeastSquaresOptimizer.java 1591835 2014-05-02 09:04:01Z tn $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 1.2 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public abstract class AbstractLeastSquaresOptimizer : BaseAbstractMultivariateVectorOptimizer<DifferentiableMultivariateVectorFunction>, DifferentiableMultivariateVectorOptimizer
		/// <summary>
		/// Singularity threshold (cf. <seealso cref="#getCovariances(double)"/>). </summary>
		/// @deprecated As of 3.1. 
	{
		public abstract PointVectorValuePair optimize(int maxEval, FUNC f, double[] target, double[] weight, double[] startPoint);
		[Obsolete("As of 3.1.")]
		private const double DEFAULT_SINGULARITY_THRESHOLD = 1e-14;
		/// <summary>
		/// Jacobian matrix of the weighted residuals.
		/// This matrix is in canonical form just after the calls to
		/// <seealso cref="#updateJacobian()"/>, but may be modified by the solver
		/// in the derived class (the {@link LevenbergMarquardtOptimizer
		/// Levenberg-Marquardt optimizer} does this). </summary>
		/// @deprecated As of 3.1. To be removed in 4.0. Please use
		/// <seealso cref="#computeWeightedJacobian(double[])"/> instead. 
		[Obsolete("As of 3.1. To be removed in 4.0. Please use")]
		protected internal double[][] weightedResidualJacobian;
		/// <summary>
		/// Number of columns of the jacobian matrix. </summary>
		/// @deprecated As of 3.1. 
		[Obsolete("As of 3.1.")]
		protected internal int cols;
		/// <summary>
		/// Number of rows of the jacobian matrix. </summary>
		/// @deprecated As of 3.1. 
		[Obsolete("As of 3.1.")]
		protected internal int rows;
		/// <summary>
		/// Current point. </summary>
		/// @deprecated As of 3.1. 
		[Obsolete("As of 3.1.")]
		protected internal double[] point;
		/// <summary>
		/// Current objective function value. </summary>
		/// @deprecated As of 3.1. 
		[Obsolete("As of 3.1.")]
		protected internal double[] objective;
		/// <summary>
		/// Weighted residuals </summary>
		/// @deprecated As of 3.1. 
		[Obsolete("As of 3.1.")]
		protected internal double[] weightedResiduals;
		/// <summary>
		/// Cost value (square root of the sum of the residuals). </summary>
		/// @deprecated As of 3.1. Field to become "private" in 4.0.
		/// Please use <seealso cref="#setCost(double)"/>. 
		[Obsolete("As of 3.1. Field to become "private" in 4.0.")]
		protected internal double cost;
		/// <summary>
		/// Objective function derivatives. </summary>
		private MultivariateDifferentiableVectorFunction jF;
		/// <summary>
		/// Number of evaluations of the Jacobian. </summary>
		private int jacobianEvaluations;
		/// <summary>
		/// Square-root of the weight matrix. </summary>
		private RealMatrix weightMatrixSqrt;

		/// <summary>
		/// Simple constructor with default settings.
		/// The convergence check is set to a {@link
		/// org.apache.commons.math3.optimization.SimpleVectorValueChecker}. </summary>
		/// @deprecated See <seealso cref="org.apache.commons.math3.optimization.SimpleValueChecker#SimpleValueChecker()"/> 
		[Obsolete("See <seealso cref="org.apache.commons.math3.optimization.SimpleValueChecker#SimpleValueChecker()"/>")]
		protected internal AbstractLeastSquaresOptimizer()
		{
		}

		/// <param name="checker"> Convergence checker. </param>
		protected internal AbstractLeastSquaresOptimizer(ConvergenceChecker<PointVectorValuePair> checker) : base(checker)
		{
		}

		/// <returns> the number of evaluations of the Jacobian function. </returns>
		public virtual int JacobianEvaluations
		{
			get
			{
				return jacobianEvaluations;
			}
		}

		/// <summary>
		/// Update the jacobian matrix.
		/// </summary>
		/// <exception cref="DimensionMismatchException"> if the Jacobian dimension does not
		/// match problem dimension. </exception>
		/// @deprecated As of 3.1. Please use <seealso cref="#computeWeightedJacobian(double[])"/>
		/// instead. 
		[Obsolete("As of 3.1. Please use <seealso cref="#computeWeightedJacobian(double[])"/>")]
		protected internal virtual void updateJacobian()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.RealMatrix weightedJacobian = computeWeightedJacobian(point);
			RealMatrix weightedJacobian = computeWeightedJacobian(point);
			weightedResidualJacobian = weightedJacobian.scalarMultiply(-1).Data;
		}

		/// <summary>
		/// Computes the Jacobian matrix.
		/// </summary>
		/// <param name="params"> Model parameters at which to compute the Jacobian. </param>
		/// <returns> the weighted Jacobian: W<sup>1/2</sup> J. </returns>
		/// <exception cref="DimensionMismatchException"> if the Jacobian dimension does not
		/// match problem dimension.
		/// @since 3.1 </exception>
		protected internal virtual RealMatrix computeWeightedJacobian(double[] @params)
		{
			++jacobianEvaluations;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.differentiation.DerivativeStructure[] dsPoint = new org.apache.commons.math3.analysis.differentiation.DerivativeStructure[params.length];
			DerivativeStructure[] dsPoint = new DerivativeStructure[@params.Length];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nC = params.length;
			int nC = @params.Length;
			for (int i = 0; i < nC; ++i)
			{
				dsPoint[i] = new DerivativeStructure(nC, 1, i, @params[i]);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.differentiation.DerivativeStructure[] dsValue = jF.value(dsPoint);
			DerivativeStructure[] dsValue = jF.value(dsPoint);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nR = getTarget().length;
			int nR = Target.Length;
			if (dsValue.Length != nR)
			{
				throw new DimensionMismatchException(dsValue.Length, nR);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] jacobianData = new double[nR][nC];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] jacobianData = new double[nR][nC];
			double[][] jacobianData = RectangularArrays.ReturnRectangularDoubleArray(nR, nC);
			for (int i = 0; i < nR; ++i)
			{
				int[] orders = new int[nC];
				for (int j = 0; j < nC; ++j)
				{
					orders[j] = 1;
					jacobianData[i][j] = dsValue[i].getPartialDerivative(orders);
					orders[j] = 0;
				}
			}

			return weightMatrixSqrt.multiply(MatrixUtils.createRealMatrix(jacobianData));
		}

		/// <summary>
		/// Update the residuals array and cost function value. </summary>
		/// <exception cref="DimensionMismatchException"> if the dimension does not match the
		/// problem dimension. </exception>
		/// <exception cref="org.apache.commons.math3.exception.TooManyEvaluationsException">
		/// if the maximal number of evaluations is exceeded. </exception>
		/// @deprecated As of 3.1. Please use <seealso cref="#computeResiduals(double[])"/>,
		/// <seealso cref="#computeObjectiveValue(double[])"/>, <seealso cref="#computeCost(double[])"/>
		/// and <seealso cref="#setCost(double)"/> instead. 
		[Obsolete("As of 3.1. Please use <seealso cref="#computeResiduals(double[])"/>,")]
		protected internal virtual void updateResidualsAndCost()
		{
			objective = computeObjectiveValue(point);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] res = computeResiduals(objective);
			double[] res = computeResiduals(objective);

			// Compute cost.
			cost = computeCost(res);

			// Compute weighted residuals.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector residuals = new org.apache.commons.math3.linear.ArrayRealVector(res);
			ArrayRealVector residuals = new ArrayRealVector(res);
			weightedResiduals = weightMatrixSqrt.operate(residuals).toArray();
		}

		/// <summary>
		/// Computes the cost.
		/// </summary>
		/// <param name="residuals"> Residuals. </param>
		/// <returns> the cost. </returns>
		/// <seealso cref= #computeResiduals(double[])
		/// @since 3.1 </seealso>
		protected internal virtual double computeCost(double[] residuals)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector r = new org.apache.commons.math3.linear.ArrayRealVector(residuals);
			ArrayRealVector r = new ArrayRealVector(residuals);
			return FastMath.sqrt(r.dotProduct(Weight.operate(r)));
		}

		/// <summary>
		/// Get the Root Mean Square value.
		/// Get the Root Mean Square value, i.e. the root of the arithmetic
		/// mean of the square of all weighted residuals. This is related to the
		/// criterion that is minimized by the optimizer as follows: if
		/// <em>c</em> if the criterion, and <em>n</em> is the number of
		/// measurements, then the RMS is <em>sqrt (c/n)</em>.
		/// </summary>
		/// <returns> RMS value </returns>
		public virtual double RMS
		{
			get
			{
				return FastMath.sqrt(ChiSquare / rows);
			}
		}

		/// <summary>
		/// Get a Chi-Square-like value assuming the N residuals follow N
		/// distinct normal distributions centered on 0 and whose variances are
		/// the reciprocal of the weights. </summary>
		/// <returns> chi-square value </returns>
		public virtual double ChiSquare
		{
			get
			{
				return cost * cost;
			}
		}

		/// <summary>
		/// Gets the square-root of the weight matrix.
		/// </summary>
		/// <returns> the square-root of the weight matrix.
		/// @since 3.1 </returns>
		public virtual RealMatrix WeightSquareRoot
		{
			get
			{
				return weightMatrixSqrt.copy();
			}
		}

		/// <summary>
		/// Sets the cost.
		/// </summary>
		/// <param name="cost"> Cost value.
		/// @since 3.1 </param>
		protected internal virtual double Cost
		{
			set
			{
				this.cost = value;
			}
		}

		/// <summary>
		/// Get the covariance matrix of the optimized parameters.
		/// </summary>
		/// <returns> the covariance matrix. </returns>
		/// <exception cref="org.apache.commons.math3.linear.SingularMatrixException">
		/// if the covariance matrix cannot be computed (singular problem). </exception>
		/// <seealso cref= #getCovariances(double) </seealso>
		/// @deprecated As of 3.1. Please use <seealso cref="#computeCovariances(double[],double)"/>
		/// instead. 
		[Obsolete("As of 3.1. Please use <seealso cref="#computeCovariances(double[] ,double)"/>")]
		public virtual double[][] Covariances
		{
			get
			{
				return getCovariances(DEFAULT_SINGULARITY_THRESHOLD);
			}
		}

		/// <summary>
		/// Get the covariance matrix of the optimized parameters.
		/// <br/>
		/// Note that this operation involves the inversion of the
		/// <code>J<sup>T</sup>J</code> matrix, where {@code J} is the
		/// Jacobian matrix.
		/// The {@code threshold} parameter is a way for the caller to specify
		/// that the result of this computation should be considered meaningless,
		/// and thus trigger an exception.
		/// </summary>
		/// <param name="threshold"> Singularity threshold. </param>
		/// <returns> the covariance matrix. </returns>
		/// <exception cref="org.apache.commons.math3.linear.SingularMatrixException">
		/// if the covariance matrix cannot be computed (singular problem). </exception>
		/// @deprecated As of 3.1. Please use <seealso cref="#computeCovariances(double[],double)"/>
		/// instead. 
		[Obsolete("As of 3.1. Please use <seealso cref="#computeCovariances(double[] ,double)"/>")]
		public virtual double[][] getCovariances(double threshold)
		{
			return computeCovariances(point, threshold);
		}

		/// <summary>
		/// Get the covariance matrix of the optimized parameters.
		/// <br/>
		/// Note that this operation involves the inversion of the
		/// <code>J<sup>T</sup>J</code> matrix, where {@code J} is the
		/// Jacobian matrix.
		/// The {@code threshold} parameter is a way for the caller to specify
		/// that the result of this computation should be considered meaningless,
		/// and thus trigger an exception.
		/// </summary>
		/// <param name="params"> Model parameters. </param>
		/// <param name="threshold"> Singularity threshold. </param>
		/// <returns> the covariance matrix. </returns>
		/// <exception cref="org.apache.commons.math3.linear.SingularMatrixException">
		/// if the covariance matrix cannot be computed (singular problem).
		/// @since 3.1 </exception>
		public virtual double[][] computeCovariances(double[] @params, double threshold)
		{
			// Set up the Jacobian.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.RealMatrix j = computeWeightedJacobian(params);
			RealMatrix j = computeWeightedJacobian(@params);

			// Compute transpose(J)J.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.RealMatrix jTj = j.transpose().multiply(j);
			RealMatrix jTj = j.transpose().multiply(j);

			// Compute the covariances matrix.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.DecompositionSolver solver = new org.apache.commons.math3.linear.QRDecomposition(jTj, threshold).getSolver();
			DecompositionSolver solver = (new QRDecomposition(jTj, threshold)).Solver;
			return solver.Inverse.Data;
		}

		/// <summary>
		/// <p>
		/// Returns an estimate of the standard deviation of each parameter. The
		/// returned values are the so-called (asymptotic) standard errors on the
		/// parameters, defined as {@code sd(a[i]) = sqrt(S / (n - m) * C[i][i])},
		/// where {@code a[i]} is the optimized value of the {@code i}-th parameter,
		/// {@code S} is the minimized value of the sum of squares objective function
		/// (as returned by <seealso cref="#getChiSquare()"/>), {@code n} is the number of
		/// observations, {@code m} is the number of parameters and {@code C} is the
		/// covariance matrix.
		/// </p>
		/// <p>
		/// See also
		/// <a href="http://en.wikipedia.org/wiki/Least_squares">Wikipedia</a>,
		/// or
		/// <a href="http://mathworld.wolfram.com/LeastSquaresFitting.html">MathWorld</a>,
		/// equations (34) and (35) for a particular case.
		/// </p>
		/// </summary>
		/// <returns> an estimate of the standard deviation of the optimized parameters </returns>
		/// <exception cref="org.apache.commons.math3.linear.SingularMatrixException">
		/// if the covariance matrix cannot be computed. </exception>
		/// <exception cref="NumberIsTooSmallException"> if the number of degrees of freedom is not
		/// positive, i.e. the number of measurements is less or equal to the number of
		/// parameters. </exception>
		/// @deprecated as of version 3.1, <seealso cref="#computeSigma(double[],double)"/> should be used
		/// instead. It should be emphasized that {@code guessParametersErrors} and
		/// {@code computeSigma} are <em>not</em> strictly equivalent. 
		[Obsolete("as of version 3.1, <seealso cref="#computeSigma(double[] ,double)"/> should be used")]
		public virtual double[] guessParametersErrors()
		{
			if (rows <= cols)
			{
				throw new NumberIsTooSmallException(LocalizedFormats.NO_DEGREES_OF_FREEDOM, rows, cols, false);
			}
			double[] errors = new double[cols];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c = org.apache.commons.math3.util.FastMath.sqrt(getChiSquare() / (rows - cols));
			double c = FastMath.sqrt(ChiSquare / (rows - cols));
			double[][] covar = computeCovariances(point, 1e-14);
			for (int i = 0; i < errors.Length; ++i)
			{
				errors[i] = FastMath.sqrt(covar[i][i]) * c;
			}
			return errors;
		}

		/// <summary>
		/// Computes an estimate of the standard deviation of the parameters. The
		/// returned values are the square root of the diagonal coefficients of the
		/// covariance matrix, {@code sd(a[i]) ~= sqrt(C[i][i])}, where {@code a[i]}
		/// is the optimized value of the {@code i}-th parameter, and {@code C} is
		/// the covariance matrix.
		/// </summary>
		/// <param name="params"> Model parameters. </param>
		/// <param name="covarianceSingularityThreshold"> Singularity threshold (see
		/// <seealso cref="#computeCovariances(double[],double) computeCovariances"/>). </param>
		/// <returns> an estimate of the standard deviation of the optimized parameters </returns>
		/// <exception cref="org.apache.commons.math3.linear.SingularMatrixException">
		/// if the covariance matrix cannot be computed.
		/// @since 3.1 </exception>
		public virtual double[] computeSigma(double[] @params, double covarianceSingularityThreshold)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nC = params.length;
			int nC = @params.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] sig = new double[nC];
			double[] sig = new double[nC];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] cov = computeCovariances(params, covarianceSingularityThreshold);
			double[][] cov = computeCovariances(@params, covarianceSingularityThreshold);
			for (int i = 0; i < nC; ++i)
			{
				sig[i] = FastMath.sqrt(cov[i][i]);
			}
			return sig;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// @deprecated As of 3.1. Please use
		/// {@link BaseAbstractMultivariateVectorOptimizer#optimize(int,
		/// org.apache.commons.math3.analysis.MultivariateVectorFunction,OptimizationData[])
		/// optimize(int,MultivariateDifferentiableVectorFunction,OptimizationData...)}
		/// instead. 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override @Deprecated public org.apache.commons.math3.optimization.PointVectorValuePair optimize(int maxEval, final org.apache.commons.math3.analysis.DifferentiableMultivariateVectorFunction f, final double[] target, final double[] weights, final double[] startPoint)
		[Obsolete]
		public override PointVectorValuePair optimize(int maxEval, DifferentiableMultivariateVectorFunction f, double[] target, double[] weights, double[] startPoint)
		{
			return optimizeInternal(maxEval, FunctionUtils.toMultivariateDifferentiableVectorFunction(f), new Target(target), new Weight(weights), new InitialGuess(startPoint));
		}

		/// <summary>
		/// Optimize an objective function.
		/// Optimization is considered to be a weighted least-squares minimization.
		/// The cost function to be minimized is
		/// <code>&sum;weight<sub>i</sub>(objective<sub>i</sub> - target<sub>i</sub>)<sup>2</sup></code>
		/// </summary>
		/// <param name="f"> Objective function. </param>
		/// <param name="target"> Target value for the objective functions at optimum. </param>
		/// <param name="weights"> Weights for the least squares cost computation. </param>
		/// <param name="startPoint"> Start point for optimization. </param>
		/// <returns> the point/value pair giving the optimal value for objective
		/// function. </returns>
		/// <param name="maxEval"> Maximum number of function evaluations. </param>
		/// <exception cref="org.apache.commons.math3.exception.DimensionMismatchException">
		/// if the start point dimension is wrong. </exception>
		/// <exception cref="org.apache.commons.math3.exception.TooManyEvaluationsException">
		/// if the maximal number of evaluations is exceeded. </exception>
		/// <exception cref="org.apache.commons.math3.exception.NullArgumentException"> if
		/// any argument is {@code null}. </exception>
		/// @deprecated As of 3.1. Please use
		/// {@link BaseAbstractMultivariateVectorOptimizer#optimize(int,
		/// org.apache.commons.math3.analysis.MultivariateVectorFunction,OptimizationData[])
		/// optimize(int,MultivariateDifferentiableVectorFunction,OptimizationData...)}
		/// instead. 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("As of 3.1. Please use") public org.apache.commons.math3.optimization.PointVectorValuePair optimize(final int maxEval, final org.apache.commons.math3.analysis.differentiation.MultivariateDifferentiableVectorFunction f, final double[] target, final double[] weights, final double[] startPoint)
		[Obsolete("As of 3.1. Please use")]
		public virtual PointVectorValuePair optimize(int maxEval, MultivariateDifferentiableVectorFunction f, double[] target, double[] weights, double[] startPoint)
		{
			return optimizeInternal(maxEval, f, new Target(target), new Weight(weights), new InitialGuess(startPoint));
		}

		/// <summary>
		/// Optimize an objective function.
		/// Optimization is considered to be a weighted least-squares minimization.
		/// The cost function to be minimized is
		/// <code>&sum;weight<sub>i</sub>(objective<sub>i</sub> - target<sub>i</sub>)<sup>2</sup></code>
		/// </summary>
		/// <param name="maxEval"> Allowed number of evaluations of the objective function. </param>
		/// <param name="f"> Objective function. </param>
		/// <param name="optData"> Optimization data. The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="Target"/></li>
		///  <li><seealso cref="Weight"/></li>
		///  <li><seealso cref="InitialGuess"/></li>
		/// </ul> </param>
		/// <returns> the point/value pair giving the optimal value of the objective
		/// function. </returns>
		/// <exception cref="org.apache.commons.math3.exception.TooManyEvaluationsException"> if
		/// the maximal number of evaluations is exceeded. </exception>
		/// <exception cref="DimensionMismatchException"> if the target, and weight arguments
		/// have inconsistent dimensions. </exception>
		/// <seealso cref= BaseAbstractMultivariateVectorOptimizer#optimizeInternal(int,
		/// org.apache.commons.math3.analysis.MultivariateVectorFunction,OptimizationData[])
		/// @since 3.1 </seealso>
		/// @deprecated As of 3.1. Override is necessary only until this class's generic
		/// argument is changed to {@code MultivariateDifferentiableVectorFunction}. 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("As of 3.1. Override is necessary only until this class's generic") protected org.apache.commons.math3.optimization.PointVectorValuePair optimizeInternal(final int maxEval, final org.apache.commons.math3.analysis.differentiation.MultivariateDifferentiableVectorFunction f, org.apache.commons.math3.optimization.OptimizationData... optData)
		[Obsolete("As of 3.1. Override is necessary only until this class's generic")]
		protected internal virtual PointVectorValuePair optimizeInternal(int maxEval, MultivariateDifferentiableVectorFunction f, params OptimizationData[] optData)
		{
			// XXX Conversion will be removed when the generic argument of the
			// base class becomes "MultivariateDifferentiableVectorFunction".
			return base.optimizeInternal(maxEval, FunctionUtils.toDifferentiableMultivariateVectorFunction(f), optData);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override void setUp()
		{
			base.setUp();

			// Reset counter.
			jacobianEvaluations = 0;

			// Square-root of the weight matrix.
			weightMatrixSqrt = squareRoot(Weight);

			// Store least squares problem characteristics.
			// XXX The conversion won't be necessary when the generic argument of
			// the base class becomes "MultivariateDifferentiableVectorFunction".
			// XXX "jF" is not strictly necessary anymore but is currently more
			// efficient than converting the value returned from "getObjectiveFunction()"
			// every time it is used.
			jF = FunctionUtils.toMultivariateDifferentiableVectorFunction((DifferentiableMultivariateVectorFunction) ObjectiveFunction);

			// Arrays shared with "private" and "protected" methods.
			point = StartPoint;
			rows = Target.Length;
			cols = point.Length;
		}

		/// <summary>
		/// Computes the residuals.
		/// The residual is the difference between the observed (target)
		/// values and the model (objective function) value.
		/// There is one residual for each element of the vector-valued
		/// function.
		/// </summary>
		/// <param name="objectiveValue"> Value of the the objective function. This is
		/// the value returned from a call to
		/// <seealso cref="#computeObjectiveValue(double[]) computeObjectiveValue"/>
		/// (whose array argument contains the model parameters). </param>
		/// <returns> the residuals. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code params} has a wrong
		/// length.
		/// @since 3.1 </exception>
		protected internal virtual double[] computeResiduals(double[] objectiveValue)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] target = getTarget();
			double[] target = Target;
			if (objectiveValue.Length != target.Length)
			{
				throw new DimensionMismatchException(target.Length, objectiveValue.Length);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] residuals = new double[target.length];
			double[] residuals = new double[target.Length];
			for (int i = 0; i < target.Length; i++)
			{
				residuals[i] = target[i] - objectiveValue[i];
			}

			return residuals;
		}

		/// <summary>
		/// Computes the square-root of the weight matrix.
		/// </summary>
		/// <param name="m"> Symmetric, positive-definite (weight) matrix. </param>
		/// <returns> the square-root of the weight matrix. </returns>
		private RealMatrix squareRoot(RealMatrix m)
		{
			if (m is DiagonalMatrix)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = m.getRowDimension();
				int dim = m.RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.RealMatrix sqrtM = new org.apache.commons.math3.linear.DiagonalMatrix(dim);
				RealMatrix sqrtM = new DiagonalMatrix(dim);
				for (int i = 0; i < dim; i++)
				{
				   sqrtM.setEntry(i, i, FastMath.sqrt(m.getEntry(i, i)));
				}
				return sqrtM;
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.EigenDecomposition dec = new org.apache.commons.math3.linear.EigenDecomposition(m);
				EigenDecomposition dec = new EigenDecomposition(m);
				return dec.SquareRoot;
			}
		}
	}

}
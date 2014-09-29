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
namespace mathlib.optim.nonlinear.vector.jacobian
{

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;
	using ArrayRealVector = mathlib.linear.ArrayRealVector;
	using RealMatrix = mathlib.linear.RealMatrix;
	using DiagonalMatrix = mathlib.linear.DiagonalMatrix;
	using DecompositionSolver = mathlib.linear.DecompositionSolver;
	using MatrixUtils = mathlib.linear.MatrixUtils;
	using QRDecomposition = mathlib.linear.QRDecomposition;
	using EigenDecomposition = mathlib.linear.EigenDecomposition;
	using mathlib.optim;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Base class for implementing least-squares optimizers.
	/// It provides methods for error estimation.
	/// 
	/// @version $Id: AbstractLeastSquaresOptimizer.java 1515242 2013-08-18 23:27:29Z erans $
	/// @since 3.1 </summary>
	/// @deprecated All classes and interfaces in this package are deprecated.
	/// The optimizers that were provided here were moved to the
	/// <seealso cref="mathlib.fitting.leastsquares"/> package
	/// (cf. MATH-1008). 
	[Obsolete("All classes and interfaces in this package are deprecated.")]
	public abstract class AbstractLeastSquaresOptimizer : JacobianMultivariateVectorOptimizer
	{
		/// <summary>
		/// Square-root of the weight matrix. </summary>
		private RealMatrix weightMatrixSqrt;
		/// <summary>
		/// Cost value (square root of the sum of the residuals). </summary>
		private double cost;

		/// <param name="checker"> Convergence checker. </param>
		protected internal AbstractLeastSquaresOptimizer(ConvergenceChecker<PointVectorValuePair> checker) : base(checker)
		{
		}

		/// <summary>
		/// Computes the weighted Jacobian matrix.
		/// </summary>
		/// <param name="params"> Model parameters at which to compute the Jacobian. </param>
		/// <returns> the weighted Jacobian: W<sup>1/2</sup> J. </returns>
		/// <exception cref="DimensionMismatchException"> if the Jacobian dimension does not
		/// match problem dimension. </exception>
		protected internal virtual RealMatrix computeWeightedJacobian(double[] @params)
		{
			return weightMatrixSqrt.multiply(MatrixUtils.createRealMatrix(computeJacobian(@params)));
		}

		/// <summary>
		/// Computes the cost.
		/// </summary>
		/// <param name="residuals"> Residuals. </param>
		/// <returns> the cost. </returns>
		/// <seealso cref= #computeResiduals(double[]) </seealso>
		protected internal virtual double computeCost(double[] residuals)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.ArrayRealVector r = new mathlib.linear.ArrayRealVector(residuals);
			ArrayRealVector r = new ArrayRealVector(residuals);
			return FastMath.sqrt(r.dotProduct(Weight.operate(r)));
		}

		/// <summary>
		/// Gets the root-mean-square (RMS) value.
		/// 
		/// The RMS the root of the arithmetic mean of the square of all weighted
		/// residuals.
		/// This is related to the criterion that is minimized by the optimizer
		/// as follows: If <em>c</em> if the criterion, and <em>n</em> is the
		/// number of measurements, then the RMS is <em>sqrt (c/n)</em>.
		/// </summary>
		/// <returns> the RMS value. </returns>
		public virtual double RMS
		{
			get
			{
				return FastMath.sqrt(ChiSquare / TargetSize);
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
		/// <returns> the square-root of the weight matrix. </returns>
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
		/// <param name="cost"> Cost value. </param>
		protected internal virtual double Cost
		{
			set
			{
				this.cost = value;
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
		/// <param name="params"> Model parameters. </param>
		/// <param name="threshold"> Singularity threshold. </param>
		/// <returns> the covariance matrix. </returns>
		/// <exception cref="mathlib.linear.SingularMatrixException">
		/// if the covariance matrix cannot be computed (singular problem). </exception>
		public virtual double[][] computeCovariances(double[] @params, double threshold)
		{
			// Set up the Jacobian.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix j = computeWeightedJacobian(params);
			RealMatrix j = computeWeightedJacobian(@params);

			// Compute transpose(J)J.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix jTj = j.transpose().multiply(j);
			RealMatrix jTj = j.transpose().multiply(j);

			// Compute the covariances matrix.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.DecompositionSolver solver = new mathlib.linear.QRDecomposition(jTj, threshold).getSolver();
			DecompositionSolver solver = (new QRDecomposition(jTj, threshold)).Solver;
			return solver.Inverse.Data;
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
		/// <exception cref="mathlib.linear.SingularMatrixException">
		/// if the covariance matrix cannot be computed. </exception>
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
		/// {@inheritDoc}
		/// </summary>
		/// <param name="optData"> Optimization data. In addition to those documented in
		/// {@link JacobianMultivariateVectorOptimizer#parseOptimizationData(OptimizationData[])
		/// JacobianMultivariateVectorOptimizer}, this method will register the following data:
		/// <ul>
		///  <li><seealso cref="mathlib.optim.nonlinear.vector.Weight"/></li>
		/// </ul> </param>
		/// <returns> {@inheritDoc} </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of
		/// evaluations is exceeded. </exception>
		/// <exception cref="DimensionMismatchException"> if the initial guess, target, and weight
		/// arguments have inconsistent dimensions. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public mathlib.optim.PointVectorValuePair optimize(mathlib.optim.OptimizationData... optData) throws mathlib.exception.TooManyEvaluationsException
		public override PointVectorValuePair optimize(params OptimizationData[] optData)
		{
			// Set up base class and perform computation.
			return base.optimize(optData);
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
		/// length. </exception>
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
		/// Scans the list of (required and optional) optimization data that
		/// characterize the problem.
		/// If the weight matrix is specified, the <seealso cref="#weightMatrixSqrt"/>
		/// field is recomputed.
		/// </summary>
		/// <param name="optData"> Optimization data. The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="Weight"/></li>
		/// </ul> </param>
		protected internal override void parseOptimizationData(params OptimizationData[] optData)
		{
			// Allow base class to register its own data.
			base.parseOptimizationData(optData);

			// The existing values (as set by the previous call) are reused if
			// not provided in the argument list.
			foreach (OptimizationData data in optData)
			{
				if (data is Weight)
				{
					weightMatrixSqrt = squareRoot(((Weight) data).Weight);
					// If more data must be parsed, this statement _must_ be
					// changed to "continue".
					break;
				}
			}
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
//ORIGINAL LINE: final mathlib.linear.RealMatrix sqrtM = new mathlib.linear.DiagonalMatrix(dim);
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
//ORIGINAL LINE: final mathlib.linear.EigenDecomposition dec = new mathlib.linear.EigenDecomposition(m);
				EigenDecomposition dec = new EigenDecomposition(m);
				return dec.SquareRoot;
			}
		}
	}

}
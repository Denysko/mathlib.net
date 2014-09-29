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

namespace mathlib.optimization.general
{

	using ConvergenceException = mathlib.exception.ConvergenceException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using MathInternalError = mathlib.exception.MathInternalError;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using ArrayRealVector = mathlib.linear.ArrayRealVector;
	using BlockRealMatrix = mathlib.linear.BlockRealMatrix;
	using DecompositionSolver = mathlib.linear.DecompositionSolver;
	using LUDecomposition = mathlib.linear.LUDecomposition;
	using QRDecomposition = mathlib.linear.QRDecomposition;
	using RealMatrix = mathlib.linear.RealMatrix;
	using SingularMatrixException = mathlib.linear.SingularMatrixException;
	using mathlib.optimization;

	/// <summary>
	/// Gauss-Newton least-squares solver.
	/// <p>
	/// This class solve a least-square problem by solving the normal equations
	/// of the linearized problem at each iteration. Either LU decomposition or
	/// QR decomposition can be used to solve the normal equations. LU decomposition
	/// is faster but QR decomposition is more robust for difficult problems.
	/// </p>
	/// 
	/// @version $Id: GaussNewtonOptimizer.java 1423687 2012-12-18 21:56:18Z erans $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 2.0
	///  
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public class GaussNewtonOptimizer : AbstractLeastSquaresOptimizer
	{
		/// <summary>
		/// Indicator for using LU decomposition. </summary>
		private readonly bool useLU;

		/// <summary>
		/// Simple constructor with default settings.
		/// The normal equations will be solved using LU decomposition and the
		/// convergence check is set to a <seealso cref="SimpleVectorValueChecker"/>
		/// with default tolerances. </summary>
		/// @deprecated See <seealso cref="SimpleVectorValueChecker#SimpleVectorValueChecker()"/> 
		[Obsolete("See <seealso cref="SimpleVectorValueChecker#SimpleVectorValueChecker()"/>")]
		public GaussNewtonOptimizer() : this(true)
		{
		}

		/// <summary>
		/// Simple constructor with default settings.
		/// The normal equations will be solved using LU decomposition.
		/// </summary>
		/// <param name="checker"> Convergence checker. </param>
		public GaussNewtonOptimizer(ConvergenceChecker<PointVectorValuePair> checker) : this(true, checker)
		{
		}

		/// <summary>
		/// Simple constructor with default settings.
		/// The convergence check is set to a <seealso cref="SimpleVectorValueChecker"/>
		/// with default tolerances.
		/// </summary>
		/// <param name="useLU"> If {@code true}, the normal equations will be solved
		/// using LU decomposition, otherwise they will be solved using QR
		/// decomposition. </param>
		/// @deprecated See <seealso cref="SimpleVectorValueChecker#SimpleVectorValueChecker()"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("See <seealso cref="SimpleVectorValueChecker#SimpleVectorValueChecker()"/>") public GaussNewtonOptimizer(final boolean useLU)
		[Obsolete("See <seealso cref="SimpleVectorValueChecker#SimpleVectorValueChecker()"/>")]
		public GaussNewtonOptimizer(bool useLU) : this(useLU, new SimpleVectorValueChecker())
		{
		}

		/// <param name="useLU"> If {@code true}, the normal equations will be solved
		/// using LU decomposition, otherwise they will be solved using QR
		/// decomposition. </param>
		/// <param name="checker"> Convergence checker. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public GaussNewtonOptimizer(final boolean useLU, mathlib.optimization.ConvergenceChecker<mathlib.optimization.PointVectorValuePair> checker)
		public GaussNewtonOptimizer(bool useLU, ConvergenceChecker<PointVectorValuePair> checker) : base(checker)
		{
			this.useLU = useLU;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override PointVectorValuePair doOptimize()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.optimization.ConvergenceChecker<mathlib.optimization.PointVectorValuePair> checker = getConvergenceChecker();
			ConvergenceChecker<PointVectorValuePair> checker = ConvergenceChecker;

			// Computation will be useless without a checker (see "for-loop").
			if (checker == null)
			{
				throw new NullArgumentException();
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] targetValues = getTarget();
			double[] targetValues = Target;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nR = targetValues.length;
			int nR = targetValues.Length; // Number of observed data.

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix weightMatrix = getWeight();
			RealMatrix weightMatrix = Weight;
			// Diagonal of the weight matrix.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] residualsWeights = new double[nR];
			double[] residualsWeights = new double[nR];
			for (int i = 0; i < nR; i++)
			{
				residualsWeights[i] = weightMatrix.getEntry(i, i);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] currentPoint = getStartPoint();
			double[] currentPoint = StartPoint;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nC = currentPoint.length;
			int nC = currentPoint.Length;

			// iterate until convergence is reached
			PointVectorValuePair current = null;
			int iter = 0;
			for (bool converged = false; !converged;)
			{
				++iter;

				// evaluate the objective function and its jacobian
				PointVectorValuePair previous = current;
				// Value of the objective function at "currentPoint".
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] currentObjective = computeObjectiveValue(currentPoint);
				double[] currentObjective = computeObjectiveValue(currentPoint);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] currentResiduals = computeResiduals(currentObjective);
				double[] currentResiduals = computeResiduals(currentObjective);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix weightedJacobian = computeWeightedJacobian(currentPoint);
				RealMatrix weightedJacobian = computeWeightedJacobian(currentPoint);
				current = new PointVectorValuePair(currentPoint, currentObjective);

				// build the linear problem
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] b = new double[nC];
				double[] b = new double[nC];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] a = new double[nC][nC];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] a = new double[nC][nC];
				double[][] a = RectangularArrays.ReturnRectangularDoubleArray(nC, nC);
				for (int i = 0; i < nR; ++i)
				{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] grad = weightedJacobian.getRow(i);
					double[] grad = weightedJacobian.getRow(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double weight = residualsWeights[i];
					double weight = residualsWeights[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double residual = currentResiduals[i];
					double residual = currentResiduals[i];

					// compute the normal equation
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double wr = weight * residual;
					double wr = weight * residual;
					for (int j = 0; j < nC; ++j)
					{
						b[j] += wr * grad[j];
					}

					// build the contribution matrix for measurement i
					for (int k = 0; k < nC; ++k)
					{
						double[] ak = a[k];
						double wgk = weight * grad[k];
						for (int l = 0; l < nC; ++l)
						{
							ak[l] += wgk * grad[l];
						}
					}
				}

				try
				{
					// solve the linearized least squares problem
					RealMatrix mA = new BlockRealMatrix(a);
					DecompositionSolver solver = useLU ? (new LUDecomposition(mA)).Solver : (new QRDecomposition(mA)).Solver;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dX = solver.solve(new mathlib.linear.ArrayRealVector(b, false)).toArray();
					double[] dX = solver.solve(new ArrayRealVector(b, false)).toArray();
					// update the estimated parameters
					for (int i = 0; i < nC; ++i)
					{
						currentPoint[i] += dX[i];
					}
				}
				catch (SingularMatrixException e)
				{
					throw new ConvergenceException(LocalizedFormats.UNABLE_TO_SOLVE_SINGULAR_PROBLEM);
				}

				// Check convergence.
				if (previous != null)
				{
					converged = checker.converged(iter, previous, current);
					if (converged)
					{
						cost = computeCost(currentResiduals);
						// Update (deprecated) "point" field.
						point = current.Point;
						return current;
					}
				}
			}
			// Must never happen.
			throw new MathInternalError();
		}
	}

}
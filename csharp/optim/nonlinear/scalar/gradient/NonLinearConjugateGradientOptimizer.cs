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

namespace org.apache.commons.math3.optim.nonlinear.scalar.gradient
{

	using UnivariateSolver = org.apache.commons.math3.analysis.solvers.UnivariateSolver;
	using MathInternalError = org.apache.commons.math3.exception.MathInternalError;
	using TooManyEvaluationsException = org.apache.commons.math3.exception.TooManyEvaluationsException;
	using MathUnsupportedOperationException = org.apache.commons.math3.exception.MathUnsupportedOperationException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using org.apache.commons.math3.optim;


	/// <summary>
	/// Non-linear conjugate gradient optimizer.
	/// <br/>
	/// This class supports both the Fletcher-Reeves and the Polak-Ribière
	/// update formulas for the conjugate search directions.
	/// It also supports optional preconditioning.
	/// <br/>
	/// Constraints are not supported: the call to
	/// <seealso cref="#optimize(OptimizationData[]) optimize"/> will throw
	/// <seealso cref="MathUnsupportedOperationException"/> if bounds are passed to it.
	/// 
	/// @version $Id: NonLinearConjugateGradientOptimizer.java 1573316 2014-03-02 14:54:37Z erans $
	/// @since 2.0
	/// </summary>
	public class NonLinearConjugateGradientOptimizer : GradientMultivariateOptimizer
	{
		/// <summary>
		/// Update formula for the beta parameter. </summary>
		private readonly Formula updateFormula;
		/// <summary>
		/// Preconditioner (may be null). </summary>
		private readonly Preconditioner preconditioner;
		/// <summary>
		/// Line search algorithm. </summary>
		private readonly LineSearch line;

		/// <summary>
		/// Available choices of update formulas for the updating the parameter
		/// that is used to compute the successive conjugate search directions.
		/// For non-linear conjugate gradients, there are
		/// two formulas:
		/// <ul>
		///   <li>Fletcher-Reeves formula</li>
		///   <li>Polak-Ribière formula</li>
		/// </ul>
		/// 
		/// On the one hand, the Fletcher-Reeves formula is guaranteed to converge
		/// if the start point is close enough of the optimum whether the
		/// Polak-Ribière formula may not converge in rare cases. On the
		/// other hand, the Polak-Ribière formula is often faster when it
		/// does converge. Polak-Ribière is often used.
		/// 
		/// @since 2.0
		/// </summary>
		public enum Formula
		{
			/// <summary>
			/// Fletcher-Reeves formula. </summary>
			FLETCHER_REEVES,
			/// <summary>
			/// Polak-Ribière formula. </summary>
			POLAK_RIBIERE
		}

		/// <summary>
		/// The initial step is a factor with respect to the search direction
		/// (which itself is roughly related to the gradient of the function).
		/// <br/>
		/// It is used to find an interval that brackets the optimum in line
		/// search.
		/// 
		/// @since 3.1 </summary>
		/// @deprecated As of v3.3, this class is not used anymore.
		/// This setting is replaced by the {@code initialBracketingRange}
		/// argument to the new constructors. 
		[Obsolete("As of v3.3, this class is not used anymore.")]
		public class BracketingStep : OptimizationData
		{
			/// <summary>
			/// Initial step. </summary>
			internal readonly double initialStep;

			/// <param name="step"> Initial step for the bracket search. </param>
			public BracketingStep(double step)
			{
				initialStep = step;
			}

			/// <summary>
			/// Gets the initial step.
			/// </summary>
			/// <returns> the initial step. </returns>
			public virtual double BracketingStep
			{
				get
				{
					return initialStep;
				}
			}
		}

		/// <summary>
		/// Constructor with default tolerances for the line search (1e-8) and
		/// <seealso cref="IdentityPreconditioner preconditioner"/>.
		/// </summary>
		/// <param name="updateFormula"> formula to use for updating the &beta; parameter,
		/// must be one of <seealso cref="Formula#FLETCHER_REEVES"/> or
		/// <seealso cref="Formula#POLAK_RIBIERE"/>. </param>
		/// <param name="checker"> Convergence checker. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NonLinearConjugateGradientOptimizer(final Formula updateFormula, org.apache.commons.math3.optim.ConvergenceChecker<org.apache.commons.math3.optim.PointValuePair> checker)
		public NonLinearConjugateGradientOptimizer(Formula updateFormula, ConvergenceChecker<PointValuePair> checker) : this(updateFormula, checker, 1e-8, 1e-8, 1e-8, new IdentityPreconditioner())
		{
		}

		/// <summary>
		/// Constructor with default <seealso cref="IdentityPreconditioner preconditioner"/>.
		/// </summary>
		/// <param name="updateFormula"> formula to use for updating the &beta; parameter,
		/// must be one of <seealso cref="Formula#FLETCHER_REEVES"/> or
		/// <seealso cref="Formula#POLAK_RIBIERE"/>. </param>
		/// <param name="checker"> Convergence checker. </param>
		/// <param name="lineSearchSolver"> Solver to use during line search. </param>
		/// @deprecated as of 3.3. Please use
		/// <seealso cref="#NonLinearConjugateGradientOptimizer(Formula,ConvergenceChecker,double,double,double)"/> instead. 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3. Please use") public NonLinearConjugateGradientOptimizer(final Formula updateFormula, org.apache.commons.math3.optim.ConvergenceChecker<org.apache.commons.math3.optim.PointValuePair> checker, final org.apache.commons.math3.analysis.solvers.UnivariateSolver lineSearchSolver)
		[Obsolete("as of 3.3. Please use")]
		public NonLinearConjugateGradientOptimizer(Formula updateFormula, ConvergenceChecker<PointValuePair> checker, UnivariateSolver lineSearchSolver) : this(updateFormula, checker, lineSearchSolver, new IdentityPreconditioner())
		{
		}

		/// <summary>
		/// Constructor with default <seealso cref="IdentityPreconditioner preconditioner"/>.
		/// </summary>
		/// <param name="updateFormula"> formula to use for updating the &beta; parameter,
		/// must be one of <seealso cref="Formula#FLETCHER_REEVES"/> or
		/// <seealso cref="Formula#POLAK_RIBIERE"/>. </param>
		/// <param name="checker"> Convergence checker. </param>
		/// <param name="relativeTolerance"> Relative threshold for line search. </param>
		/// <param name="absoluteTolerance"> Absolute threshold for line search. </param>
		/// <param name="initialBracketingRange"> Extent of the initial interval used to
		/// find an interval that brackets the optimum in order to perform the
		/// line search.
		/// </param>
		/// <seealso cref= LineSearch#LineSearch(MultivariateOptimizer,double,double,double)
		/// @since 3.3 </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NonLinearConjugateGradientOptimizer(final Formula updateFormula, org.apache.commons.math3.optim.ConvergenceChecker<org.apache.commons.math3.optim.PointValuePair> checker, double relativeTolerance, double absoluteTolerance, double initialBracketingRange)
		public NonLinearConjugateGradientOptimizer(Formula updateFormula, ConvergenceChecker<PointValuePair> checker, double relativeTolerance, double absoluteTolerance, double initialBracketingRange) : this(updateFormula, checker, relativeTolerance, absoluteTolerance, initialBracketingRange, new IdentityPreconditioner())
		{
		}

		/// <param name="updateFormula"> formula to use for updating the &beta; parameter,
		/// must be one of <seealso cref="Formula#FLETCHER_REEVES"/> or
		/// <seealso cref="Formula#POLAK_RIBIERE"/>. </param>
		/// <param name="checker"> Convergence checker. </param>
		/// <param name="lineSearchSolver"> Solver to use during line search. </param>
		/// <param name="preconditioner"> Preconditioner. </param>
		/// @deprecated as of 3.3. Please use
		/// <seealso cref="#NonLinearConjugateGradientOptimizer(Formula,ConvergenceChecker,double,double,double,Preconditioner)"/> instead. 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3. Please use") public NonLinearConjugateGradientOptimizer(final Formula updateFormula, org.apache.commons.math3.optim.ConvergenceChecker<org.apache.commons.math3.optim.PointValuePair> checker, final org.apache.commons.math3.analysis.solvers.UnivariateSolver lineSearchSolver, final Preconditioner preconditioner)
		[Obsolete("as of 3.3. Please use")]
		public NonLinearConjugateGradientOptimizer(Formula updateFormula, ConvergenceChecker<PointValuePair> checker, UnivariateSolver lineSearchSolver, Preconditioner preconditioner) : this(updateFormula, checker, lineSearchSolver.RelativeAccuracy, lineSearchSolver.AbsoluteAccuracy, lineSearchSolver.AbsoluteAccuracy, preconditioner)
		{
		}

		/// <param name="updateFormula"> formula to use for updating the &beta; parameter,
		/// must be one of <seealso cref="Formula#FLETCHER_REEVES"/> or
		/// <seealso cref="Formula#POLAK_RIBIERE"/>. </param>
		/// <param name="checker"> Convergence checker. </param>
		/// <param name="preconditioner"> Preconditioner. </param>
		/// <param name="relativeTolerance"> Relative threshold for line search. </param>
		/// <param name="absoluteTolerance"> Absolute threshold for line search. </param>
		/// <param name="initialBracketingRange"> Extent of the initial interval used to
		/// find an interval that brackets the optimum in order to perform the
		/// line search.
		/// </param>
		/// <seealso cref= LineSearch#LineSearch(MultivariateOptimizer,double,double,double)
		/// @since 3.3 </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NonLinearConjugateGradientOptimizer(final Formula updateFormula, org.apache.commons.math3.optim.ConvergenceChecker<org.apache.commons.math3.optim.PointValuePair> checker, double relativeTolerance, double absoluteTolerance, double initialBracketingRange, final Preconditioner preconditioner)
		public NonLinearConjugateGradientOptimizer(Formula updateFormula, ConvergenceChecker<PointValuePair> checker, double relativeTolerance, double absoluteTolerance, double initialBracketingRange, Preconditioner preconditioner) : base(checker)
		{

			this.updateFormula = updateFormula;
			this.preconditioner = preconditioner;
			line = new LineSearch(this, relativeTolerance, absoluteTolerance, initialBracketingRange);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.apache.commons.math3.optim.PointValuePair optimize(org.apache.commons.math3.optim.OptimizationData... optData) throws org.apache.commons.math3.exception.TooManyEvaluationsException
		public override PointValuePair optimize(params OptimizationData[] optData)
		{
			// Set up base class and perform computation.
			return base.optimize(optData);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override PointValuePair doOptimize()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.ConvergenceChecker<org.apache.commons.math3.optim.PointValuePair> checker = getConvergenceChecker();
			ConvergenceChecker<PointValuePair> checker = ConvergenceChecker;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] point = getStartPoint();
			double[] point = StartPoint;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.nonlinear.scalar.GoalType goal = getGoalType();
			GoalType goal = GoalType;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = point.length;
			int n = point.Length;
			double[] r = computeObjectiveGradient(point);
			if (goal == GoalType.MINIMIZE)
			{
				for (int i = 0; i < n; i++)
				{
					r[i] = -r[i];
				}
			}

			// Initial search direction.
			double[] steepestDescent = preconditioner.precondition(point, r);
			double[] searchDirection = steepestDescent.clone();

			double delta = 0;
			for (int i = 0; i < n; ++i)
			{
				delta += r[i] * searchDirection[i];
			}

			PointValuePair current = null;
			while (true)
			{
				incrementIterationCount();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double objective = computeObjectiveValue(point);
				double objective = computeObjectiveValue(point);
				PointValuePair previous = current;
				current = new PointValuePair(point, objective);
				if (previous != null && checker.converged(Iterations, previous, current))
				{
					// We have found an optimum.
					return current;
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double step = line.search(point, searchDirection).getPoint();
				double step = line.search(point, searchDirection).Point;

				// Validate new point.
				for (int i = 0; i < point.Length; ++i)
				{
					point[i] += step * searchDirection[i];
				}

				r = computeObjectiveGradient(point);
				if (goal == GoalType.MINIMIZE)
				{
					for (int i = 0; i < n; ++i)
					{
						r[i] = -r[i];
					}
				}

				// Compute beta.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double deltaOld = delta;
				double deltaOld = delta;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] newSteepestDescent = preconditioner.precondition(point, r);
				double[] newSteepestDescent = preconditioner.precondition(point, r);
				delta = 0;
				for (int i = 0; i < n; ++i)
				{
					delta += r[i] * newSteepestDescent[i];
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double beta;
				double beta;
				switch (updateFormula)
				{
				case org.apache.commons.math3.optim.nonlinear.scalar.gradient.NonLinearConjugateGradientOptimizer.Formula.FLETCHER_REEVES:
					beta = delta / deltaOld;
					break;
				case org.apache.commons.math3.optim.nonlinear.scalar.gradient.NonLinearConjugateGradientOptimizer.Formula.POLAK_RIBIERE:
					double deltaMid = 0;
					for (int i = 0; i < r.Length; ++i)
					{
						deltaMid += r[i] * steepestDescent[i];
					}
					beta = (delta - deltaMid) / deltaOld;
					break;
				default:
					// Should never happen.
					throw new MathInternalError();
				}
				steepestDescent = newSteepestDescent;

				// Compute conjugate search direction.
				if (Iterations % n == 0 || beta < 0)
				{
					// Break conjugation: reset search direction.
					searchDirection = steepestDescent.clone();
				}
				else
				{
					// Compute new conjugate search direction.
					for (int i = 0; i < n; ++i)
					{
						searchDirection[i] = steepestDescent[i] + beta * searchDirection[i];
					}
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		protected internal override void parseOptimizationData(params OptimizationData[] optData)
		{
			// Allow base class to register its own data.
			base.parseOptimizationData(optData);

			checkParameters();
		}

		/// <summary>
		/// Default identity preconditioner. </summary>
		public class IdentityPreconditioner : Preconditioner
		{
			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual double[] precondition(double[] variables, double[] r)
			{
				return r.clone();
			}
		}

		// Class is not used anymore (cf. MATH-1092). However, it might
		// be interesting to create a class similar to "LineSearch", but
		// that will take advantage that the model's gradient is available.
	//     /**
	//      * Internal class for line search.
	//      * <p>
	//      * The function represented by this class is the dot product of
	//      * the objective function gradient and the search direction. Its
	//      * value is zero when the gradient is orthogonal to the search
	//      * direction, i.e. when the objective function value is a local
	//      * extremum along the search direction.
	//      * </p>
	//      */
	//     private class LineSearchFunction implements UnivariateFunction {
	//         /** Current point. */
	//         private final double[] currentPoint;
	//         /** Search direction. */
	//         private final double[] searchDirection;

	//         /**
	//          * @param point Current point.
	//          * @param direction Search direction.
	//          */
	//         public LineSearchFunction(double[] point,
	//                                   double[] direction) {
	//             currentPoint = point.clone();
	//             searchDirection = direction.clone();
	//         }

	//         /** {@inheritDoc} */
	//         public double value(double x) {
	//             // current point in the search direction
	//             final double[] shiftedPoint = currentPoint.clone();
	//             for (int i = 0; i < shiftedPoint.length; ++i) {
	//                 shiftedPoint[i] += x * searchDirection[i];
	//             }

	//             // gradient of the objective function
	//             final double[] gradient = computeObjectiveGradient(shiftedPoint);

	//             // dot product with the search direction
	//             double dotProduct = 0;
	//             for (int i = 0; i < gradient.length; ++i) {
	//                 dotProduct += gradient[i] * searchDirection[i];
	//             }

	//             return dotProduct;
	//         }
	//     }

		/// <exception cref="MathUnsupportedOperationException"> if bounds were passed to the
		/// <seealso cref="#optimize(OptimizationData[]) optimize"/> method. </exception>
		private void checkParameters()
		{
			if (LowerBound != null || UpperBound != null)
			{
				throw new MathUnsupportedOperationException(LocalizedFormats.CONSTRAINT);
			}
		}
	}

}
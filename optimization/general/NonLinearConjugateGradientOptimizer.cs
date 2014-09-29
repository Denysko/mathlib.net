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

	using MathIllegalStateException = mathlib.exception.MathIllegalStateException;
	using UnivariateFunction = mathlib.analysis.UnivariateFunction;
	using BrentSolver = mathlib.analysis.solvers.BrentSolver;
	using UnivariateSolver = mathlib.analysis.solvers.UnivariateSolver;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using mathlib.optimization;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Non-linear conjugate gradient optimizer.
	/// <p>
	/// This class supports both the Fletcher-Reeves and the Polak-Ribi&egrave;re
	/// update formulas for the conjugate search directions. It also supports
	/// optional preconditioning.
	/// </p>
	/// 
	/// @version $Id: NonLinearConjugateGradientOptimizer.java 1462503 2013-03-29 15:48:27Z luc $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 2.0
	///  
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public class NonLinearConjugateGradientOptimizer : AbstractScalarDifferentiableOptimizer
	{
		/// <summary>
		/// Update formula for the beta parameter. </summary>
		private readonly ConjugateGradientFormula updateFormula;
		/// <summary>
		/// Preconditioner (may be null). </summary>
		private readonly Preconditioner preconditioner;
		/// <summary>
		/// solver to use in the line search (may be null). </summary>
		private readonly UnivariateSolver solver;
		/// <summary>
		/// Initial step used to bracket the optimum in line search. </summary>
		private double initialStep;
		/// <summary>
		/// Current point. </summary>
		private double[] point;

		/// <summary>
		/// Constructor with default <seealso cref="SimpleValueChecker checker"/>,
		/// <seealso cref="BrentSolver line search solver"/> and
		/// <seealso cref="IdentityPreconditioner preconditioner"/>.
		/// </summary>
		/// <param name="updateFormula"> formula to use for updating the &beta; parameter,
		/// must be one of <seealso cref="ConjugateGradientFormula#FLETCHER_REEVES"/> or {@link
		/// ConjugateGradientFormula#POLAK_RIBIERE}. </param>
		/// @deprecated See <seealso cref="SimpleValueChecker#SimpleValueChecker()"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("See <seealso cref="SimpleValueChecker#SimpleValueChecker()"/>") public NonLinearConjugateGradientOptimizer(final ConjugateGradientFormula updateFormula)
		[Obsolete("See <seealso cref="SimpleValueChecker#SimpleValueChecker()"/>")]
		public NonLinearConjugateGradientOptimizer(ConjugateGradientFormula updateFormula) : this(updateFormula, new SimpleValueChecker())
		{
		}

		/// <summary>
		/// Constructor with default <seealso cref="BrentSolver line search solver"/> and
		/// <seealso cref="IdentityPreconditioner preconditioner"/>.
		/// </summary>
		/// <param name="updateFormula"> formula to use for updating the &beta; parameter,
		/// must be one of <seealso cref="ConjugateGradientFormula#FLETCHER_REEVES"/> or {@link
		/// ConjugateGradientFormula#POLAK_RIBIERE}. </param>
		/// <param name="checker"> Convergence checker. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NonLinearConjugateGradientOptimizer(final ConjugateGradientFormula updateFormula, mathlib.optimization.ConvergenceChecker<mathlib.optimization.PointValuePair> checker)
		public NonLinearConjugateGradientOptimizer(ConjugateGradientFormula updateFormula, ConvergenceChecker<PointValuePair> checker) : this(updateFormula, checker, new BrentSolver(), new IdentityPreconditioner())
		{
		}


		/// <summary>
		/// Constructor with default <seealso cref="IdentityPreconditioner preconditioner"/>.
		/// </summary>
		/// <param name="updateFormula"> formula to use for updating the &beta; parameter,
		/// must be one of <seealso cref="ConjugateGradientFormula#FLETCHER_REEVES"/> or {@link
		/// ConjugateGradientFormula#POLAK_RIBIERE}. </param>
		/// <param name="checker"> Convergence checker. </param>
		/// <param name="lineSearchSolver"> Solver to use during line search. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NonLinearConjugateGradientOptimizer(final ConjugateGradientFormula updateFormula, mathlib.optimization.ConvergenceChecker<mathlib.optimization.PointValuePair> checker, final mathlib.analysis.solvers.UnivariateSolver lineSearchSolver)
		public NonLinearConjugateGradientOptimizer(ConjugateGradientFormula updateFormula, ConvergenceChecker<PointValuePair> checker, UnivariateSolver lineSearchSolver) : this(updateFormula, checker, lineSearchSolver, new IdentityPreconditioner())
		{
		}

		/// <param name="updateFormula"> formula to use for updating the &beta; parameter,
		/// must be one of <seealso cref="ConjugateGradientFormula#FLETCHER_REEVES"/> or {@link
		/// ConjugateGradientFormula#POLAK_RIBIERE}. </param>
		/// <param name="checker"> Convergence checker. </param>
		/// <param name="lineSearchSolver"> Solver to use during line search. </param>
		/// <param name="preconditioner"> Preconditioner. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NonLinearConjugateGradientOptimizer(final ConjugateGradientFormula updateFormula, mathlib.optimization.ConvergenceChecker<mathlib.optimization.PointValuePair> checker, final mathlib.analysis.solvers.UnivariateSolver lineSearchSolver, final Preconditioner preconditioner)
		public NonLinearConjugateGradientOptimizer(ConjugateGradientFormula updateFormula, ConvergenceChecker<PointValuePair> checker, UnivariateSolver lineSearchSolver, Preconditioner preconditioner) : base(checker)
		{

			this.updateFormula = updateFormula;
			solver = lineSearchSolver;
			this.preconditioner = preconditioner;
			initialStep = 1.0;
		}

		/// <summary>
		/// Set the initial step used to bracket the optimum in line search.
		/// <p>
		/// The initial step is a factor with respect to the search direction,
		/// which itself is roughly related to the gradient of the function
		/// </p> </summary>
		/// <param name="initialStep"> initial step used to bracket the optimum in line search,
		/// if a non-positive value is used, the initial step is reset to its
		/// default value of 1.0 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setInitialStep(final double initialStep)
		public virtual double InitialStep
		{
			set
			{
				if (value <= 0)
				{
					this.initialStep = 1.0;
				}
				else
				{
					this.initialStep = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override PointValuePair doOptimize()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.optimization.ConvergenceChecker<mathlib.optimization.PointValuePair> checker = getConvergenceChecker();
			ConvergenceChecker<PointValuePair> checker = ConvergenceChecker;
			point = StartPoint;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.optimization.GoalType goal = getGoalType();
			GoalType goal = GoalType;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = point.length;
			int n = point.Length;
			double[] r = computeObjectiveGradient(point);
			if (goal == GoalType.MINIMIZE)
			{
				for (int i = 0; i < n; ++i)
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
			int iter = 0;
			int maxEval = MaxEvaluations;
			while (true)
			{
				++iter;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double objective = computeObjectiveValue(point);
				double objective = computeObjectiveValue(point);
				PointValuePair previous = current;
				current = new PointValuePair(point, objective);
				if (previous != null && checker.converged(iter, previous, current))
				{
					// We have found an optimum.
					return current;
				}

				// Find the optimal step in the search direction.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.analysis.UnivariateFunction lsf = new LineSearchFunction(searchDirection);
				UnivariateFunction lsf = new LineSearchFunction(this, searchDirection);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double uB = findUpperBound(lsf, 0, initialStep);
				double uB = findUpperBound(lsf, 0, initialStep);
				// XXX Last parameters is set to a value close to zero in order to
				// work around the divergence problem in the "testCircleFitting"
				// unit test (see MATH-439).
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double step = solver.solve(maxEval, lsf, 0, uB, 1e-15);
				double step = solver.solve(maxEval, lsf, 0, uB, 1e-15);
				maxEval -= solver.Evaluations; // Subtract used up evaluations.

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
				if (updateFormula == ConjugateGradientFormula.FLETCHER_REEVES)
				{
					beta = delta / deltaOld;
				}
				else
				{
					double deltaMid = 0;
					for (int i = 0; i < r.Length; ++i)
					{
						deltaMid += r[i] * steepestDescent[i];
					}
					beta = (delta - deltaMid) / deltaOld;
				}
				steepestDescent = newSteepestDescent;

				// Compute conjugate search direction.
				if (iter % n == 0 || beta < 0)
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
		/// Find the upper bound b ensuring bracketing of a root between a and b.
		/// </summary>
		/// <param name="f"> function whose root must be bracketed. </param>
		/// <param name="a"> lower bound of the interval. </param>
		/// <param name="h"> initial step to try. </param>
		/// <returns> b such that f(a) and f(b) have opposite signs. </returns>
		/// <exception cref="MathIllegalStateException"> if no bracket can be found. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double findUpperBound(final mathlib.analysis.UnivariateFunction f, final double a, final double h)
		private double findUpperBound(UnivariateFunction f, double a, double h)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yA = f.value(a);
			double yA = f.value(a);
			double yB = yA;
			for (double step = h; step < double.MaxValue; step *= FastMath.max(2, yA / yB))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b = a + step;
				double b = a + step;
				yB = f.value(b);
				if (yA * yB <= 0)
				{
					return b;
				}
			}
			throw new MathIllegalStateException(LocalizedFormats.UNABLE_TO_BRACKET_OPTIMUM_IN_LINE_SEARCH);
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

		/// <summary>
		/// Internal class for line search.
		/// <p>
		/// The function represented by this class is the dot product of
		/// the objective function gradient and the search direction. Its
		/// value is zero when the gradient is orthogonal to the search
		/// direction, i.e. when the objective function value is a local
		/// extremum along the search direction.
		/// </p>
		/// </summary>
		private class LineSearchFunction : UnivariateFunction
		{
			private readonly NonLinearConjugateGradientOptimizer outerInstance;

			/// <summary>
			/// Search direction. </summary>
			internal readonly double[] searchDirection;

			/// <summary>
			/// Simple constructor. </summary>
			/// <param name="searchDirection"> search direction </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LineSearchFunction(final double[] searchDirection)
			public LineSearchFunction(NonLinearConjugateGradientOptimizer outerInstance, double[] searchDirection)
			{
				this.outerInstance = outerInstance;
				this.searchDirection = searchDirection;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual double value(double x)
			{
				// current point in the search direction
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] shiftedPoint = point.clone();
				double[] shiftedPoint = outerInstance.point.clone();
				for (int i = 0; i < shiftedPoint.Length; ++i)
				{
					shiftedPoint[i] += x * searchDirection[i];
				}

				// gradient of the objective function
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] gradient = computeObjectiveGradient(shiftedPoint);
				double[] gradient = outerInstance.computeObjectiveGradient(shiftedPoint);

				// dot product with the search direction
				double dotProduct = 0;
				for (int i = 0; i < gradient.Length; ++i)
				{
					dotProduct += gradient[i] * searchDirection[i];
				}

				return dotProduct;
			}
		}
	}

}
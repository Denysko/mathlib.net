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
namespace mathlib.optim.linear
{


	using TooManyIterationsException = mathlib.exception.TooManyIterationsException;
	using FastMath = mathlib.util.FastMath;
	using Precision = mathlib.util.Precision;

	/// <summary>
	/// Solves a linear problem using the "Two-Phase Simplex" method.
	/// <p>
	/// The <seealso cref="SimplexSolver"/> supports the following <seealso cref="OptimizationData"/> data provided
	/// as arguments to <seealso cref="#optimize(OptimizationData...)"/>:
	/// <ul>
	///   <li>objective function: <seealso cref="LinearObjectiveFunction"/> - mandatory</li>
	///   <li>linear constraints <seealso cref="LinearConstraintSet"/> - mandatory</li>
	///   <li>type of optimization: <seealso cref="mathlib.optim.nonlinear.scalar.GoalType GoalType"/>
	///    - optional, default: <seealso cref="mathlib.optim.nonlinear.scalar.GoalType#MINIMIZE MINIMIZE"/></li>
	///   <li>whether to allow negative values as solution: <seealso cref="NonNegativeConstraint"/> - optional, default: true</li>
	///   <li>pivot selection rule: <seealso cref="PivotSelectionRule"/> - optional, default <seealso cref="PivotSelectionRule#DANTZIG"/></li>
	///   <li>callback for the best solution: <seealso cref="SolutionCallback"/> - optional</li>
	///   <li>maximum number of iterations: <seealso cref="mathlib.optim.MaxIter"/> - optional, default: <seealso cref="Integer#MAX_VALUE"/></li>
	/// </ul>
	/// <p>
	/// <b>Note:</b> Depending on the problem definition, the default convergence criteria
	/// may be too strict, resulting in <seealso cref="NoFeasibleSolutionException"/> or
	/// <seealso cref="TooManyIterationsException"/>. In such a case it is advised to adjust these
	/// criteria with more appropriate values, e.g. relaxing the epsilon value.
	/// <p>
	/// Default convergence criteria:
	/// <ul>
	///   <li>Algorithm convergence: 1e-6</li>
	///   <li>Floating-point comparisons: 10 ulp</li>
	///   <li>Cut-Off value: 1e-10</li>
	/// </ul>
	/// <p>
	/// The cut-off value has been introduced to handle the case of very small pivot elements
	/// in the Simplex tableau, as these may lead to numerical instabilities and degeneracy.
	/// Potential pivot elements smaller than this value will be treated as if they were zero
	/// and are thus not considered by the pivot selection mechanism. The default value is safe
	/// for many problems, but may need to be adjusted in case of very small coefficients
	/// used in either the <seealso cref="LinearConstraint"/> or <seealso cref="LinearObjectiveFunction"/>.
	/// 
	/// @version $Id: SimplexSolver.java 1553855 2013-12-28 15:55:24Z erans $
	/// @since 2.0
	/// </summary>
	public class SimplexSolver : LinearOptimizer
	{
		/// <summary>
		/// Default amount of error to accept in floating point comparisons (as ulps). </summary>
		internal const int DEFAULT_ULPS = 10;

		/// <summary>
		/// Default cut-off value. </summary>
		internal const double DEFAULT_CUT_OFF = 1e-10;

		/// <summary>
		/// Default amount of error to accept for algorithm convergence. </summary>
		private const double DEFAULT_EPSILON = 1.0e-6;

		/// <summary>
		/// Amount of error to accept for algorithm convergence. </summary>
		private readonly double epsilon;

		/// <summary>
		/// Amount of error to accept in floating point comparisons (as ulps). </summary>
		private readonly int maxUlps;

		/// <summary>
		/// Cut-off value for entries in the tableau: values smaller than the cut-off
		/// are treated as zero to improve numerical stability.
		/// </summary>
		private readonly double cutOff;

		/// <summary>
		/// The pivot selection method to use. </summary>
		private PivotSelectionRule pivotSelection;

		/// <summary>
		/// The solution callback to access the best solution found so far in case
		/// the optimizer fails to find an optimal solution within the iteration limits.
		/// </summary>
		private SolutionCallback solutionCallback;

		/// <summary>
		/// Builds a simplex solver with default settings.
		/// </summary>
		public SimplexSolver() : this(DEFAULT_EPSILON, DEFAULT_ULPS, DEFAULT_CUT_OFF)
		{
		}

		/// <summary>
		/// Builds a simplex solver with a specified accepted amount of error.
		/// </summary>
		/// <param name="epsilon"> Amount of error to accept for algorithm convergence. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SimplexSolver(final double epsilon)
		public SimplexSolver(double epsilon) : this(epsilon, DEFAULT_ULPS, DEFAULT_CUT_OFF)
		{
		}

		/// <summary>
		/// Builds a simplex solver with a specified accepted amount of error.
		/// </summary>
		/// <param name="epsilon"> Amount of error to accept for algorithm convergence. </param>
		/// <param name="maxUlps"> Amount of error to accept in floating point comparisons. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SimplexSolver(final double epsilon, final int maxUlps)
		public SimplexSolver(double epsilon, int maxUlps) : this(epsilon, maxUlps, DEFAULT_CUT_OFF)
		{
		}

		/// <summary>
		/// Builds a simplex solver with a specified accepted amount of error.
		/// </summary>
		/// <param name="epsilon"> Amount of error to accept for algorithm convergence. </param>
		/// <param name="maxUlps"> Amount of error to accept in floating point comparisons. </param>
		/// <param name="cutOff"> Values smaller than the cutOff are treated as zero. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SimplexSolver(final double epsilon, final int maxUlps, final double cutOff)
		public SimplexSolver(double epsilon, int maxUlps, double cutOff)
		{
			this.epsilon = epsilon;
			this.maxUlps = maxUlps;
			this.cutOff = cutOff;
			this.pivotSelection = PivotSelectionRule.DANTZIG;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <param name="optData"> Optimization data. In addition to those documented in
		/// {@link LinearOptimizer#optimize(OptimizationData...)
		/// LinearOptimizer}, this method will register the following data:
		/// <ul>
		///  <li><seealso cref="SolutionCallback"/></li>
		///  <li><seealso cref="PivotSelectionRule"/></li>
		/// </ul>
		/// </param>
		/// <returns> {@inheritDoc} </returns>
		/// <exception cref="TooManyIterationsException"> if the maximal number of iterations is exceeded. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public mathlib.optim.PointValuePair optimize(mathlib.optim.OptimizationData... optData) throws mathlib.exception.TooManyIterationsException
		public override PointValuePair optimize(params OptimizationData[] optData)
		{
			// Set up base class and perform computation.
			return base.optimize(optData);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <param name="optData"> Optimization data.
		/// In addition to those documented in
		/// {@link LinearOptimizer#parseOptimizationData(OptimizationData[])
		/// LinearOptimizer}, this method will register the following data:
		/// <ul>
		///  <li><seealso cref="SolutionCallback"/></li>
		///  <li><seealso cref="PivotSelectionRule"/></li>
		/// </ul> </param>
		protected internal override void parseOptimizationData(params OptimizationData[] optData)
		{
			// Allow base class to register its own data.
			base.parseOptimizationData(optData);

			// reset the callback before parsing
			solutionCallback = null;

			foreach (OptimizationData data in optData)
			{
				if (data is SolutionCallback)
				{
					solutionCallback = (SolutionCallback) data;
					continue;
				}
				if (data is PivotSelectionRule)
				{
					pivotSelection = (PivotSelectionRule) data;
					continue;
				}
			}
		}

		/// <summary>
		/// Returns the column with the most negative coefficient in the objective function row.
		/// </summary>
		/// <param name="tableau"> Simple tableau for the problem. </param>
		/// <returns> the column with the most negative coefficient. </returns>
		private int? getPivotColumn(SimplexTableau tableau)
		{
			double minValue = 0;
			int? minPos = null;
			for (int i = tableau.NumObjectiveFunctions; i < tableau.Width - 1; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double entry = tableau.getEntry(0, i);
				double entry = tableau.getEntry(0, i);
				// check if the entry is strictly smaller than the current minimum
				// do not use a ulp/epsilon check
				if (entry < minValue)
				{
					minValue = entry;
					minPos = i;

					// Bland's rule: chose the entering column with the lowest index
					if (pivotSelection == PivotSelectionRule.BLAND && isValidPivotColumn(tableau, i))
					{
						break;
					}
				}
			}
			return minPos;
		}

		/// <summary>
		/// Checks whether the given column is valid pivot column, i.e. will result
		/// in a valid pivot row.
		/// <p>
		/// When applying Bland's rule to select the pivot column, it may happen that
		/// there is no corresponding pivot row. This method will check if the selected
		/// pivot column will return a valid pivot row.
		/// </summary>
		/// <param name="tableau"> simplex tableau for the problem </param>
		/// <param name="col"> the column to test </param>
		/// <returns> {@code true} if the pivot column is valid, {@code false} otherwise </returns>
		private bool isValidPivotColumn(SimplexTableau tableau, int col)
		{
			for (int i = tableau.NumObjectiveFunctions; i < tableau.Height; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double entry = tableau.getEntry(i, col);
				double entry = tableau.getEntry(i, col);

				// do the same check as in getPivotRow
				if (Precision.compareTo(entry, 0d, cutOff) > 0)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns the row with the minimum ratio as given by the minimum ratio test (MRT).
		/// </summary>
		/// <param name="tableau"> Simplex tableau for the problem. </param>
		/// <param name="col"> Column to test the ratio of (see <seealso cref="#getPivotColumn(SimplexTableau)"/>). </param>
		/// <returns> the row with the minimum ratio. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Integer getPivotRow(SimplexTableau tableau, final int col)
		private int? getPivotRow(SimplexTableau tableau, int col)
		{
			// create a list of all the rows that tie for the lowest score in the minimum ratio test
			IList<int?> minRatioPositions = new List<int?>();
			double minRatio = double.MaxValue;
			for (int i = tableau.NumObjectiveFunctions; i < tableau.Height; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rhs = tableau.getEntry(i, tableau.getWidth() - 1);
				double rhs = tableau.getEntry(i, tableau.Width - 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double entry = tableau.getEntry(i, col);
				double entry = tableau.getEntry(i, col);

				// only consider pivot elements larger than the cutOff threshold
				// selecting others may lead to degeneracy or numerical instabilities
				if (Precision.compareTo(entry, 0d, cutOff) > 0)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ratio = mathlib.util.FastMath.abs(rhs / entry);
					double ratio = FastMath.abs(rhs / entry);
					// check if the entry is strictly equal to the current min ratio
					// do not use a ulp/epsilon check
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int cmp = Double.compare(ratio, minRatio);
					int cmp = ratio.CompareTo(minRatio);
					if (cmp == 0)
					{
						minRatioPositions.Add(i);
					}
					else if (cmp < 0)
					{
						minRatio = ratio;
						minRatioPositions.Clear();
						minRatioPositions.Add(i);
					}
				}
			}

			if (minRatioPositions.Count == 0)
			{
				return null;
			}
			else if (minRatioPositions.Count > 1)
			{
				// there's a degeneracy as indicated by a tie in the minimum ratio test

				// 1. check if there's an artificial variable that can be forced out of the basis
				if (tableau.NumArtificialVariables > 0)
				{
					foreach (int? row in minRatioPositions)
					{
						for (int i = 0; i < tableau.NumArtificialVariables; i++)
						{
							int column = i + tableau.ArtificialVariableOffset;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double entry = tableau.getEntry(row, column);
							double entry = tableau.getEntry(row, column);
							if (Precision.Equals(entry, 1d, maxUlps) && row.Equals(tableau.getBasicRow(column)))
							{
								return row;
							}
						}
					}
				}

				// 2. apply Bland's rule to prevent cycling:
				//    take the row for which the corresponding basic variable has the smallest index
				//
				// see http://www.stanford.edu/class/msande310/blandrule.pdf
				// see http://en.wikipedia.org/wiki/Bland%27s_rule (not equivalent to the above paper)

				int? minRow = null;
				int minIndex = tableau.Width;
				foreach (int? row in minRatioPositions)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int basicVar = tableau.getBasicVariable(row);
					int basicVar = tableau.getBasicVariable(row);
					if (basicVar < minIndex)
					{
						minIndex = basicVar;
						minRow = row;
					}
				}
				return minRow;
			}
			return minRatioPositions[0];
		}

		/// <summary>
		/// Runs one iteration of the Simplex method on the given model.
		/// </summary>
		/// <param name="tableau"> Simple tableau for the problem. </param>
		/// <exception cref="TooManyIterationsException"> if the allowed number of iterations has been exhausted. </exception>
		/// <exception cref="UnboundedSolutionException"> if the model is found not to have a bounded solution. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void doIteration(final SimplexTableau tableau) throws mathlib.exception.TooManyIterationsException, UnboundedSolutionException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual void doIteration(SimplexTableau tableau)
		{

			incrementIterationCount();

			int? pivotCol = getPivotColumn(tableau);
			int? pivotRow = getPivotRow(tableau, pivotCol);
			if (pivotRow == null)
			{
				throw new UnboundedSolutionException();
			}

			tableau.performRowOperations(pivotCol, pivotRow);
		}

		/// <summary>
		/// Solves Phase 1 of the Simplex method.
		/// </summary>
		/// <param name="tableau"> Simple tableau for the problem. </param>
		/// <exception cref="TooManyIterationsException"> if the allowed number of iterations has been exhausted. </exception>
		/// <exception cref="UnboundedSolutionException"> if the model is found not to have a bounded solution. </exception>
		/// <exception cref="NoFeasibleSolutionException"> if there is no feasible solution? </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void solvePhase1(final SimplexTableau tableau) throws mathlib.exception.TooManyIterationsException, UnboundedSolutionException, NoFeasibleSolutionException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual void solvePhase1(SimplexTableau tableau)
		{

			// make sure we're in Phase 1
			if (tableau.NumArtificialVariables == 0)
			{
				return;
			}

			while (!tableau.Optimal)
			{
				doIteration(tableau);
			}

			// if W is not zero then we have no feasible solution
			if (!Precision.Equals(tableau.getEntry(0, tableau.RhsOffset), 0d, epsilon))
			{
				throw new NoFeasibleSolutionException();
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public mathlib.optim.PointValuePair doOptimize() throws mathlib.exception.TooManyIterationsException, UnboundedSolutionException, NoFeasibleSolutionException
		public override PointValuePair doOptimize()
		{

			// reset the tableau to indicate a non-feasible solution in case
			// we do not pass phase 1 successfully
			if (solutionCallback != null)
			{
				solutionCallback.Tableau = null;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SimplexTableau tableau = new SimplexTableau(getFunction(), getConstraints(), getGoalType(), isRestrictedToNonNegative(), epsilon, maxUlps);
			SimplexTableau tableau = new SimplexTableau(Function, Constraints, GoalType, RestrictedToNonNegative, epsilon, maxUlps);

			solvePhase1(tableau);
			tableau.dropPhase1Objective();

			// after phase 1, we are sure to have a feasible solution
			if (solutionCallback != null)
			{
				solutionCallback.Tableau = tableau;
			}

			while (!tableau.Optimal)
			{
				doIteration(tableau);
			}

			// check that the solution respects the nonNegative restriction in case
			// the epsilon/cutOff values are too large for the actual linear problem
			// (e.g. with very small constraint coefficients), the solver might actually
			// find a non-valid solution (with negative coefficients).
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.optim.PointValuePair solution = tableau.getSolution();
			PointValuePair solution = tableau.Solution;
			if (RestrictedToNonNegative)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] coeff = solution.getPoint();
				double[] coeff = solution.Point;
				for (int i = 0; i < coeff.Length; i++)
				{
					if (Precision.compareTo(coeff[i], 0, epsilon) < 0)
					{
						throw new NoFeasibleSolutionException();
					}
				}
			}
			return solution;
		}
	}

}
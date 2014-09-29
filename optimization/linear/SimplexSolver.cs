using System;
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

namespace mathlib.optimization.linear
{


	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using Precision = mathlib.util.Precision;


	/// <summary>
	/// Solves a linear problem using the Two-Phase Simplex Method.
	/// 
	/// @version $Id: SimplexSolver.java 1524213 2013-09-17 20:32:50Z tn $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 2.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public class SimplexSolver : AbstractLinearOptimizer
	{

		/// <summary>
		/// Default amount of error to accept for algorithm convergence. </summary>
		private const double DEFAULT_EPSILON = 1.0e-6;

		/// <summary>
		/// Default amount of error to accept in floating point comparisons (as ulps). </summary>
		private const int DEFAULT_ULPS = 10;

		/// <summary>
		/// Amount of error to accept for algorithm convergence. </summary>
		private readonly double epsilon;

		/// <summary>
		/// Amount of error to accept in floating point comparisons (as ulps). </summary>
		private readonly int maxUlps;

		/// <summary>
		/// Build a simplex solver with default settings.
		/// </summary>
		public SimplexSolver() : this(DEFAULT_EPSILON, DEFAULT_ULPS)
		{
		}

		/// <summary>
		/// Build a simplex solver with a specified accepted amount of error </summary>
		/// <param name="epsilon"> the amount of error to accept for algorithm convergence </param>
		/// <param name="maxUlps"> amount of error to accept in floating point comparisons </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SimplexSolver(final double epsilon, final int maxUlps)
		public SimplexSolver(double epsilon, int maxUlps)
		{
			this.epsilon = epsilon;
			this.maxUlps = maxUlps;
		}

		/// <summary>
		/// Returns the column with the most negative coefficient in the objective function row. </summary>
		/// <param name="tableau"> simple tableau for the problem </param>
		/// <returns> column with the most negative coefficient </returns>
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
				}
			}
			return minPos;
		}

		/// <summary>
		/// Returns the row with the minimum ratio as given by the minimum ratio test (MRT). </summary>
		/// <param name="tableau"> simple tableau for the problem </param>
		/// <param name="col"> the column to test the ratio of.  See <seealso cref="#getPivotColumn(SimplexTableau)"/> </param>
		/// <returns> row with the minimum ratio </returns>
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

				if (Precision.compareTo(entry, 0d, maxUlps) > 0)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ratio = rhs / entry;
					double ratio = rhs / entry;
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
						minRatioPositions = new List<int?>();
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
				//
				// Additional heuristic: if we did not get a solution after half of maxIterations
				//                       revert to the simple case of just returning the top-most row
				// This heuristic is based on empirical data gathered while investigating MATH-828.
				if (Iterations < MaxIterations / 2)
				{
					int? minRow = null;
					int minIndex = tableau.Width;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int varStart = tableau.getNumObjectiveFunctions();
					int varStart = tableau.NumObjectiveFunctions;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int varEnd = tableau.getWidth() - 1;
					int varEnd = tableau.Width - 1;
					foreach (int? row in minRatioPositions)
					{
						for (int i = varStart; i < varEnd && !row.Equals(minRow); i++)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Integer basicRow = tableau.getBasicRow(i);
							int? basicRow = tableau.getBasicRow(i);
							if (basicRow != null && basicRow.Equals(row) && i < minIndex)
							{
								minIndex = i;
								minRow = row;
							}
						}
					}
					return minRow;
				}
			}
			return minRatioPositions[0];
		}

		/// <summary>
		/// Runs one iteration of the Simplex method on the given model. </summary>
		/// <param name="tableau"> simple tableau for the problem </param>
		/// <exception cref="MaxCountExceededException"> if the maximal iteration count has been exceeded </exception>
		/// <exception cref="UnboundedSolutionException"> if the model is found not to have a bounded solution </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void doIteration(final SimplexTableau tableau) throws mathlib.exception.MaxCountExceededException, UnboundedSolutionException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual void doIteration(SimplexTableau tableau)
		{

			incrementIterationsCounter();

			int? pivotCol = getPivotColumn(tableau);
			int? pivotRow = getPivotRow(tableau, pivotCol);
			if (pivotRow == null)
			{
				throw new UnboundedSolutionException();
			}

			// set the pivot element to 1
			double pivotVal = tableau.getEntry(pivotRow, pivotCol);
			tableau.divideRow(pivotRow, pivotVal);

			// set the rest of the pivot column to 0
			for (int i = 0; i < tableau.Height; i++)
			{
				if (i != pivotRow)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double multiplier = tableau.getEntry(i, pivotCol);
					double multiplier = tableau.getEntry(i, pivotCol);
					tableau.subtractRow(i, pivotRow, multiplier);
				}
			}
		}

		/// <summary>
		/// Solves Phase 1 of the Simplex method. </summary>
		/// <param name="tableau"> simple tableau for the problem </param>
		/// <exception cref="MaxCountExceededException"> if the maximal iteration count has been exceeded </exception>
		/// <exception cref="UnboundedSolutionException"> if the model is found not to have a bounded solution </exception>
		/// <exception cref="NoFeasibleSolutionException"> if there is no feasible solution </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void solvePhase1(final SimplexTableau tableau) throws mathlib.exception.MaxCountExceededException, UnboundedSolutionException, NoFeasibleSolutionException
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
//ORIGINAL LINE: @Override public mathlib.optimization.PointValuePair doOptimize() throws mathlib.exception.MaxCountExceededException, UnboundedSolutionException, NoFeasibleSolutionException
		public override PointValuePair doOptimize()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SimplexTableau tableau = new SimplexTableau(getFunction(), getConstraints(), getGoalType(), restrictToNonNegative(), epsilon, maxUlps);
			SimplexTableau tableau = new SimplexTableau(Function, Constraints, GoalType, restrictToNonNegative(), epsilon, maxUlps);

			solvePhase1(tableau);
			tableau.dropPhase1Objective();

			while (!tableau.Optimal)
			{
				doIteration(tableau);
			}
			return tableau.Solution;
		}

	}

}
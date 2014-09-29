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
namespace mathlib.optim.linear
{


	using Array2DRowRealMatrix = mathlib.linear.Array2DRowRealMatrix;
	using MatrixUtils = mathlib.linear.MatrixUtils;
	using RealVector = mathlib.linear.RealVector;
	using GoalType = mathlib.optim.nonlinear.scalar.GoalType;
	using Precision = mathlib.util.Precision;

	/// <summary>
	/// A tableau for use in the Simplex method.
	/// 
	/// <p>
	/// Example:
	/// <pre>
	///   W |  Z |  x1 |  x2 |  x- | s1 |  s2 |  a1 |  RHS
	/// ---------------------------------------------------
	///  -1    0    0     0     0     0     0     1     0   &lt;= phase 1 objective
	///   0    1   -15   -10    0     0     0     0     0   &lt;= phase 2 objective
	///   0    0    1     0     0     1     0     0     2   &lt;= constraint 1
	///   0    0    0     1     0     0     1     0     3   &lt;= constraint 2
	///   0    0    1     1     0     0     0     1     4   &lt;= constraint 3
	/// </pre>
	/// W: Phase 1 objective function</br>
	/// Z: Phase 2 objective function</br>
	/// x1 &amp; x2: Decision variables</br>
	/// x-: Extra decision variable to allow for negative values</br>
	/// s1 &amp; s2: Slack/Surplus variables</br>
	/// a1: Artificial variable</br>
	/// RHS: Right hand side</br>
	/// </p>
	/// @version $Id: SimplexTableau.java 1554544 2013-12-31 15:40:35Z tn $
	/// @since 2.0
	/// </summary>
	[Serializable]
	internal class SimplexTableau
	{

		/// <summary>
		/// Column label for negative vars. </summary>
		private const string NEGATIVE_VAR_COLUMN_LABEL = "x-";

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -1369660067587938365L;

		/// <summary>
		/// Linear objective function. </summary>
		private readonly LinearObjectiveFunction f;

		/// <summary>
		/// Linear constraints. </summary>
		private readonly IList<LinearConstraint> constraints;

		/// <summary>
		/// Whether to restrict the variables to non-negative values. </summary>
		private readonly bool restrictToNonNegative;

		/// <summary>
		/// The variables each column represents </summary>
		private readonly IList<string> columnLabels = new List<string>();

		/// <summary>
		/// Simple tableau. </summary>
		[NonSerialized]
		private Array2DRowRealMatrix tableau;

		/// <summary>
		/// Number of decision variables. </summary>
		private readonly int numDecisionVariables;

		/// <summary>
		/// Number of slack variables. </summary>
		private readonly int numSlackVariables;

		/// <summary>
		/// Number of artificial variables. </summary>
		private int numArtificialVariables;

		/// <summary>
		/// Amount of error to accept when checking for optimality. </summary>
		private readonly double epsilon;

		/// <summary>
		/// Amount of error to accept in floating point comparisons. </summary>
		private readonly int maxUlps;

		/// <summary>
		/// Maps basic variables to row they are basic in. </summary>
		private int[] basicVariables;

		/// <summary>
		/// Maps rows to their corresponding basic variables. </summary>
		private int[] basicRows;

		/// <summary>
		/// Builds a tableau for a linear problem.
		/// </summary>
		/// <param name="f"> Linear objective function. </param>
		/// <param name="constraints"> Linear constraints. </param>
		/// <param name="goalType"> Optimization goal: either <seealso cref="GoalType#MAXIMIZE"/>
		/// or <seealso cref="GoalType#MINIMIZE"/>. </param>
		/// <param name="restrictToNonNegative"> Whether to restrict the variables to non-negative values. </param>
		/// <param name="epsilon"> Amount of error to accept when checking for optimality. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: SimplexTableau(final LinearObjectiveFunction f, final java.util.Collection<LinearConstraint> constraints, final mathlib.optim.nonlinear.scalar.GoalType goalType, final boolean restrictToNonNegative, final double epsilon)
		internal SimplexTableau(LinearObjectiveFunction f, ICollection<LinearConstraint> constraints, GoalType goalType, bool restrictToNonNegative, double epsilon) : this(f, constraints, goalType, restrictToNonNegative, epsilon, SimplexSolver.DEFAULT_ULPS)
		{
		}

		/// <summary>
		/// Build a tableau for a linear problem. </summary>
		/// <param name="f"> linear objective function </param>
		/// <param name="constraints"> linear constraints </param>
		/// <param name="goalType"> type of optimization goal: either <seealso cref="GoalType#MAXIMIZE"/> or <seealso cref="GoalType#MINIMIZE"/> </param>
		/// <param name="restrictToNonNegative"> whether to restrict the variables to non-negative values </param>
		/// <param name="epsilon"> amount of error to accept when checking for optimality </param>
		/// <param name="maxUlps"> amount of error to accept in floating point comparisons </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: SimplexTableau(final LinearObjectiveFunction f, final java.util.Collection<LinearConstraint> constraints, final mathlib.optim.nonlinear.scalar.GoalType goalType, final boolean restrictToNonNegative, final double epsilon, final int maxUlps)
		internal SimplexTableau(LinearObjectiveFunction f, ICollection<LinearConstraint> constraints, GoalType goalType, bool restrictToNonNegative, double epsilon, int maxUlps)
		{
			this.f = f;
			this.constraints = normalizeConstraints(constraints);
			this.restrictToNonNegative = restrictToNonNegative;
			this.epsilon = epsilon;
			this.maxUlps = maxUlps;
			this.numDecisionVariables = f.Coefficients.Dimension + (restrictToNonNegative ? 0 : 1);
			this.numSlackVariables = getConstraintTypeCounts(Relationship.LEQ) + getConstraintTypeCounts(Relationship.GEQ);
			this.numArtificialVariables = getConstraintTypeCounts(Relationship.EQ) + getConstraintTypeCounts(Relationship.GEQ);
			this.tableau = createTableau(goalType == GoalType.MAXIMIZE);
			// initialize the basic variables for phase 1:
			//   we know that only slack or artificial variables can be basic
			initializeBasicVariables(SlackVariableOffset);
			initializeColumnLabels();
		}

		/// <summary>
		/// Initialize the labels for the columns.
		/// </summary>
		protected internal virtual void initializeColumnLabels()
		{
		  if (NumObjectiveFunctions == 2)
		  {
			columnLabels.Add("W");
		  }
		  columnLabels.Add("Z");
		  for (int i = 0; i < OriginalNumDecisionVariables; i++)
		  {
			columnLabels.Add("x" + i);
		  }
		  if (!restrictToNonNegative)
		  {
			columnLabels.Add(NEGATIVE_VAR_COLUMN_LABEL);
		  }
		  for (int i = 0; i < NumSlackVariables; i++)
		  {
			columnLabels.Add("s" + i);
		  }
		  for (int i = 0; i < NumArtificialVariables; i++)
		  {
			columnLabels.Add("a" + i);
		  }
		  columnLabels.Add("RHS");
		}

		/// <summary>
		/// Create the tableau by itself. </summary>
		/// <param name="maximize"> if true, goal is to maximize the objective function </param>
		/// <returns> created tableau </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected mathlib.linear.Array2DRowRealMatrix createTableau(final boolean maximize)
		protected internal virtual Array2DRowRealMatrix createTableau(bool maximize)
		{

			// create a matrix of the correct size
			int width = numDecisionVariables + numSlackVariables + numArtificialVariables + NumObjectiveFunctions + 1; // + 1 is for RHS
			int height = constraints.Count + NumObjectiveFunctions;
			Array2DRowRealMatrix matrix = new Array2DRowRealMatrix(height, width);

			// initialize the objective function rows
			if (NumObjectiveFunctions == 2)
			{
				matrix.setEntry(0, 0, -1);
			}

			int zIndex = (NumObjectiveFunctions == 1) ? 0 : 1;
			matrix.setEntry(zIndex, zIndex, maximize ? 1 : -1);
			RealVector objectiveCoefficients = maximize ? f.Coefficients.mapMultiply(-1) : f.Coefficients;
			copyArray(objectiveCoefficients.toArray(), matrix.DataRef[zIndex]);
			matrix.setEntry(zIndex, width - 1, maximize ? f.ConstantTerm : -1 * f.ConstantTerm);

			if (!restrictToNonNegative)
			{
				matrix.setEntry(zIndex, SlackVariableOffset - 1, getInvertedCoefficientSum(objectiveCoefficients));
			}

			// initialize the constraint rows
			int slackVar = 0;
			int artificialVar = 0;
			for (int i = 0; i < constraints.Count; i++)
			{
				LinearConstraint constraint = constraints[i];
				int row = NumObjectiveFunctions + i;

				// decision variable coefficients
				copyArray(constraint.Coefficients.toArray(), matrix.DataRef[row]);

				// x-
				if (!restrictToNonNegative)
				{
					matrix.setEntry(row, SlackVariableOffset - 1, getInvertedCoefficientSum(constraint.Coefficients));
				}

				// RHS
				matrix.setEntry(row, width - 1, constraint.Value);

				// slack variables
				if (constraint.Relationship == Relationship.LEQ)
				{
					matrix.setEntry(row, SlackVariableOffset + slackVar++, 1); // slack
				}
				else if (constraint.Relationship == Relationship.GEQ)
				{
					matrix.setEntry(row, SlackVariableOffset + slackVar++, -1); // excess
				}

				// artificial variables
				if ((constraint.Relationship == Relationship.EQ) || (constraint.Relationship == Relationship.GEQ))
				{
					matrix.setEntry(0, ArtificialVariableOffset + artificialVar, 1);
					matrix.setEntry(row, ArtificialVariableOffset + artificialVar++, 1);
					matrix.setRowVector(0, matrix.getRowVector(0).subtract(matrix.getRowVector(row)));
				}
			}

			return matrix;
		}

		/// <summary>
		/// Get new versions of the constraints which have positive right hand sides. </summary>
		/// <param name="originalConstraints"> original (not normalized) constraints </param>
		/// <returns> new versions of the constraints </returns>
		public virtual IList<LinearConstraint> normalizeConstraints(ICollection<LinearConstraint> originalConstraints)
		{
			IList<LinearConstraint> normalized = new List<LinearConstraint>(originalConstraints.Count);
			foreach (LinearConstraint constraint in originalConstraints)
			{
				normalized.Add(normalize(constraint));
			}
			return normalized;
		}

		/// <summary>
		/// Get a new equation equivalent to this one with a positive right hand side. </summary>
		/// <param name="constraint"> reference constraint </param>
		/// <returns> new equation </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private LinearConstraint normalize(final LinearConstraint constraint)
		private LinearConstraint normalize(LinearConstraint constraint)
		{
			if (constraint.Value < 0)
			{
				return new LinearConstraint(constraint.Coefficients.mapMultiply(-1), constraint.Relationship.oppositeRelationship(), -1 * constraint.Value);
			}
			return new LinearConstraint(constraint.Coefficients, constraint.Relationship, constraint.Value);
		}

		/// <summary>
		/// Get the number of objective functions in this tableau. </summary>
		/// <returns> 2 for Phase 1.  1 for Phase 2. </returns>
		protected internal int NumObjectiveFunctions
		{
			get
			{
				return this.numArtificialVariables > 0 ? 2 : 1;
			}
		}

		/// <summary>
		/// Get a count of constraints corresponding to a specified relationship. </summary>
		/// <param name="relationship"> relationship to count </param>
		/// <returns> number of constraint with the specified relationship </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private int getConstraintTypeCounts(final Relationship relationship)
		private int getConstraintTypeCounts(Relationship relationship)
		{
			int count = 0;
			foreach (LinearConstraint constraint in constraints)
			{
				if (constraint.Relationship == relationship)
				{
					++count;
				}
			}
			return count;
		}

		/// <summary>
		/// Get the -1 times the sum of all coefficients in the given array. </summary>
		/// <param name="coefficients"> coefficients to sum </param>
		/// <returns> the -1 times the sum of all coefficients in the given array. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected static double getInvertedCoefficientSum(final mathlib.linear.RealVector coefficients)
		protected internal static double getInvertedCoefficientSum(RealVector coefficients)
		{
			double sum = 0;
			foreach (double coefficient in coefficients.toArray())
			{
				sum -= coefficient;
			}
			return sum;
		}

		/// <summary>
		/// Checks whether the given column is basic. </summary>
		/// <param name="col"> index of the column to check </param>
		/// <returns> the row that the variable is basic in.  null if the column is not basic </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected Integer getBasicRow(final int col)
		protected internal virtual int? getBasicRow(int col)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int row = basicVariables[col];
			int row = basicVariables[col];
			return row == -1 ? null : row;
		}

		/// <summary>
		/// Returns the variable that is basic in this row. </summary>
		/// <param name="row"> the index of the row to check </param>
		/// <returns> the variable that is basic for this row. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected int getBasicVariable(final int row)
		protected internal virtual int getBasicVariable(int row)
		{
			return basicRows[row];
		}

		/// <summary>
		/// Initializes the basic variable / row mapping. </summary>
		/// <param name="startColumn"> the column to start </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void initializeBasicVariables(final int startColumn)
		private void initializeBasicVariables(int startColumn)
		{
			basicVariables = new int[Width - 1];
			basicRows = new int[Height];

			Arrays.fill(basicVariables, -1);

			for (int i = startColumn; i < Width - 1; i++)
			{
				int? row = findBasicRow(i);
				if (row != null)
				{
					basicVariables[i] = row;
					basicRows[row] = i;
				}
			}
		}

		/// <summary>
		/// Returns the row in which the given column is basic. </summary>
		/// <param name="col"> index of the column </param>
		/// <returns> the row that the variable is basic in, or {@code null} if the variable is not basic. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Integer findBasicRow(final int col)
		private int? findBasicRow(int col)
		{
			int? row = null;
			for (int i = 0; i < Height; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double entry = getEntry(i, col);
				double entry = getEntry(i, col);
				if (Precision.Equals(entry, 1d, maxUlps) && (row == null))
				{
					row = i;
				}
				else if (!Precision.Equals(entry, 0d, maxUlps))
				{
					return null;
				}
			}
			return row;
		}

		/// <summary>
		/// Removes the phase 1 objective function, positive cost non-artificial variables,
		/// and the non-basic artificial variables from this tableau.
		/// </summary>
		protected internal virtual void dropPhase1Objective()
		{
			if (NumObjectiveFunctions == 1)
			{
				return;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Set<Integer> columnsToDrop = new java.util.TreeSet<Integer>();
			Set<int?> columnsToDrop = new SortedSet<int?>();
			columnsToDrop.add(0);

			// positive cost non-artificial variables
			for (int i = NumObjectiveFunctions; i < ArtificialVariableOffset; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double entry = getEntry(0, i);
				double entry = getEntry(0, i);
				if (Precision.compareTo(entry, 0d, epsilon) > 0)
				{
					columnsToDrop.add(i);
				}
			}

			// non-basic artificial variables
			for (int i = 0; i < NumArtificialVariables; i++)
			{
				int col = i + ArtificialVariableOffset;
				if (getBasicRow(col) == null)
				{
					columnsToDrop.add(col);
				}
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] matrix = new double[getHeight() - 1][getWidth() - columnsToDrop.size()];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] matrix = new double[Height - 1][Width - columnsToDrop.size()];
			double[][] matrix = RectangularArrays.ReturnRectangularDoubleArray(Height - 1, Width - columnsToDrop.size());
			for (int i = 1; i < Height; i++)
			{
				int col = 0;
				for (int j = 0; j < Width; j++)
				{
					if (!columnsToDrop.contains(j))
					{
						matrix[i - 1][col++] = getEntry(i, j);
					}
				}
			}

			// remove the columns in reverse order so the indices are correct
			int?[] drop = columnsToDrop.toArray(new int?[columnsToDrop.size()]);
			for (int i = drop.Length - 1; i >= 0; i--)
			{
				columnLabels.RemoveAt((int) drop[i]);
			}

			this.tableau = new Array2DRowRealMatrix(matrix);
			this.numArtificialVariables = 0;
			// need to update the basic variable mappings as row/columns have been dropped
			initializeBasicVariables(NumObjectiveFunctions);
		}

		/// <param name="src"> the source array </param>
		/// <param name="dest"> the destination array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void copyArray(final double[] src, final double[] dest)
		private void copyArray(double[] src, double[] dest)
		{
			Array.Copy(src, 0, dest, NumObjectiveFunctions, src.Length);
		}

		/// <summary>
		/// Returns whether the problem is at an optimal state. </summary>
		/// <returns> whether the model has been solved </returns>
		internal virtual bool Optimal
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[] objectiveFunctionRow = getRow(0);
				double[] objectiveFunctionRow = getRow(0);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int end = getRhsOffset();
				int end = RhsOffset;
				for (int i = NumObjectiveFunctions; i < end; i++)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double entry = objectiveFunctionRow[i];
					double entry = objectiveFunctionRow[i];
					if (Precision.compareTo(entry, 0d, epsilon) < 0)
					{
						return false;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Get the current solution. </summary>
		/// <returns> current solution </returns>
		protected internal virtual PointValuePair Solution
		{
			get
			{
				int negativeVarColumn = columnLabels.IndexOf(NEGATIVE_VAR_COLUMN_LABEL);
				int? negativeVarBasicRow = negativeVarColumn > 0 ? getBasicRow(negativeVarColumn) : null;
				double mostNegative = negativeVarBasicRow == null ? 0 : getEntry(negativeVarBasicRow, RhsOffset);
    
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.Set<Integer> usedBasicRows = new java.util.HashSet<Integer>();
				Set<int?> usedBasicRows = new HashSet<int?>();
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[] coefficients = new double[getOriginalNumDecisionVariables()];
				double[] coefficients = new double[OriginalNumDecisionVariables];
				for (int i = 0; i < coefficients.Length; i++)
				{
					int colIndex = columnLabels.IndexOf("x" + i);
					if (colIndex < 0)
					{
						coefficients[i] = 0;
						continue;
					}
					int? basicRow = getBasicRow(colIndex);
					if (basicRow != null && basicRow == 0)
					{
						// if the basic row is found to be the objective function row
						// set the coefficient to 0 -> this case handles unconstrained
						// variables that are still part of the objective function
						coefficients[i] = 0;
					}
					else if (usedBasicRows.contains(basicRow))
					{
						// if multiple variables can take a given value
						// then we choose the first and set the rest equal to 0
						coefficients[i] = 0 - (restrictToNonNegative ? 0 : mostNegative);
					}
					else
					{
						usedBasicRows.add(basicRow);
						coefficients[i] = (basicRow == null ? 0 : getEntry(basicRow, RhsOffset)) - (restrictToNonNegative ? 0 : mostNegative);
					}
				}
				return new PointValuePair(coefficients, f.value(coefficients));
			}
		}

		/// <summary>
		/// Perform the row operations of the simplex algorithm with the selected
		/// pivot column and row. </summary>
		/// <param name="pivotCol"> the pivot column </param>
		/// <param name="pivotRow"> the pivot row </param>
		protected internal virtual void performRowOperations(int pivotCol, int pivotRow)
		{
			// set the pivot element to 1
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double pivotVal = getEntry(pivotRow, pivotCol);
			double pivotVal = getEntry(pivotRow, pivotCol);
			divideRow(pivotRow, pivotVal);

			// set the rest of the pivot column to 0
			for (int i = 0; i < Height; i++)
			{
				if (i != pivotRow)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double multiplier = getEntry(i, pivotCol);
					double multiplier = getEntry(i, pivotCol);
					if (multiplier != 0.0)
					{
						subtractRow(i, pivotRow, multiplier);
					}
				}
			}

			// update the basic variable mappings
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int previousBasicVariable = getBasicVariable(pivotRow);
			int previousBasicVariable = getBasicVariable(pivotRow);
			basicVariables[previousBasicVariable] = -1;
			basicVariables[pivotCol] = pivotRow;
			basicRows[pivotRow] = pivotCol;
		}

		/// <summary>
		/// Divides one row by a given divisor.
		/// <p>
		/// After application of this operation, the following will hold:
		/// <pre>dividendRow = dividendRow / divisor</pre>
		/// </summary>
		/// <param name="dividendRowIndex"> index of the row </param>
		/// <param name="divisor"> value of the divisor </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected void divideRow(final int dividendRowIndex, final double divisor)
		protected internal virtual void divideRow(int dividendRowIndex, double divisor)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dividendRow = getRow(dividendRowIndex);
			double[] dividendRow = getRow(dividendRowIndex);
			for (int j = 0; j < Width; j++)
			{
				dividendRow[j] /= divisor;
			}
		}

		/// <summary>
		/// Subtracts a multiple of one row from another.
		/// <p>
		/// After application of this operation, the following will hold:
		/// <pre>minuendRow = minuendRow - multiple * subtrahendRow</pre>
		/// </summary>
		/// <param name="minuendRowIndex"> row index </param>
		/// <param name="subtrahendRowIndex"> row index </param>
		/// <param name="multiplier"> multiplication factor </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected void subtractRow(final int minuendRowIndex, final int subtrahendRowIndex, final double multiplier)
		protected internal virtual void subtractRow(int minuendRowIndex, int subtrahendRowIndex, double multiplier)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] minuendRow = getRow(minuendRowIndex);
			double[] minuendRow = getRow(minuendRowIndex);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] subtrahendRow = getRow(subtrahendRowIndex);
			double[] subtrahendRow = getRow(subtrahendRowIndex);
			for (int i = 0; i < Width; i++)
			{
				minuendRow[i] -= subtrahendRow[i] * multiplier;
			}
		}

		/// <summary>
		/// Get the width of the tableau. </summary>
		/// <returns> width of the tableau </returns>
		protected internal int Width
		{
			get
			{
				return tableau.ColumnDimension;
			}
		}

		/// <summary>
		/// Get the height of the tableau. </summary>
		/// <returns> height of the tableau </returns>
		protected internal int Height
		{
			get
			{
				return tableau.RowDimension;
			}
		}

		/// <summary>
		/// Get an entry of the tableau. </summary>
		/// <param name="row"> row index </param>
		/// <param name="column"> column index </param>
		/// <returns> entry at (row, column) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected final double getEntry(final int row, final int column)
		protected internal double getEntry(int row, int column)
		{
			return tableau.getEntry(row, column);
		}

		/// <summary>
		/// Set an entry of the tableau. </summary>
		/// <param name="row"> row index </param>
		/// <param name="column"> column index </param>
		/// <param name="value"> for the entry </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected final void setEntry(final int row, final int column, final double value)
		protected internal void setEntry(int row, int column, double value)
		{
			tableau.setEntry(row, column, value);
		}

		/// <summary>
		/// Get the offset of the first slack variable. </summary>
		/// <returns> offset of the first slack variable </returns>
		protected internal int SlackVariableOffset
		{
			get
			{
				return NumObjectiveFunctions + numDecisionVariables;
			}
		}

		/// <summary>
		/// Get the offset of the first artificial variable. </summary>
		/// <returns> offset of the first artificial variable </returns>
		protected internal int ArtificialVariableOffset
		{
			get
			{
				return NumObjectiveFunctions + numDecisionVariables + numSlackVariables;
			}
		}

		/// <summary>
		/// Get the offset of the right hand side. </summary>
		/// <returns> offset of the right hand side </returns>
		protected internal int RhsOffset
		{
			get
			{
				return Width - 1;
			}
		}

		/// <summary>
		/// Get the number of decision variables.
		/// <p>
		/// If variables are not restricted to positive values, this will include 1 extra decision variable to represent
		/// the absolute value of the most negative variable.
		/// </summary>
		/// <returns> number of decision variables </returns>
		/// <seealso cref= #getOriginalNumDecisionVariables() </seealso>
		protected internal int NumDecisionVariables
		{
			get
			{
				return numDecisionVariables;
			}
		}

		/// <summary>
		/// Get the original number of decision variables. </summary>
		/// <returns> original number of decision variables </returns>
		/// <seealso cref= #getNumDecisionVariables() </seealso>
		protected internal int OriginalNumDecisionVariables
		{
			get
			{
				return f.Coefficients.Dimension;
			}
		}

		/// <summary>
		/// Get the number of slack variables. </summary>
		/// <returns> number of slack variables </returns>
		protected internal int NumSlackVariables
		{
			get
			{
				return numSlackVariables;
			}
		}

		/// <summary>
		/// Get the number of artificial variables. </summary>
		/// <returns> number of artificial variables </returns>
		protected internal int NumArtificialVariables
		{
			get
			{
				return numArtificialVariables;
			}
		}

		/// <summary>
		/// Get the row from the tableau. </summary>
		/// <param name="row"> the row index </param>
		/// <returns> the reference to the underlying row data </returns>
		protected internal double[] getRow(int row)
		{
			return tableau.DataRef[row];
		}

		/// <summary>
		/// Get the tableau data. </summary>
		/// <returns> tableau data </returns>
		protected internal double[][] Data
		{
			get
			{
				return tableau.Data;
			}
		}

		public override bool Equals(object other)
		{

		  if (this == other)
		  {
			return true;
		  }

		  if (other is SimplexTableau)
		  {
			  SimplexTableau rhs = (SimplexTableau) other;
			  return (restrictToNonNegative == rhs.restrictToNonNegative) && (numDecisionVariables == rhs.numDecisionVariables) && (numSlackVariables == rhs.numSlackVariables) && (numArtificialVariables == rhs.numArtificialVariables) && (epsilon == rhs.epsilon) && (maxUlps == rhs.maxUlps) && f.Equals(rhs.f) && constraints.Equals(rhs.constraints) && tableau.Equals(rhs.tableau);
		  }
		  return false;
		}

		public override int GetHashCode()
		{
			return Convert.ToBoolean(restrictToNonNegative).GetHashCode() ^ numDecisionVariables ^ numSlackVariables ^ numArtificialVariables ^ Convert.ToDouble(epsilon).GetHashCode() ^ maxUlps ^ f.GetHashCode() ^ constraints.GetHashCode() ^ tableau.GetHashCode();
		}

		/// <summary>
		/// Serialize the instance. </summary>
		/// <param name="oos"> stream where object should be written </param>
		/// <exception cref="IOException"> if object cannot be written to stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream oos) throws java.io.IOException
		private void writeObject(ObjectOutputStream oos)
		{
			oos.defaultWriteObject();
			MatrixUtils.serializeRealMatrix(tableau, oos);
		}

		/// <summary>
		/// Deserialize the instance. </summary>
		/// <param name="ois"> stream from which the object should be read </param>
		/// <exception cref="ClassNotFoundException"> if a class in the stream cannot be found </exception>
		/// <exception cref="IOException"> if object cannot be read from the stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream ois) throws ClassNotFoundException, java.io.IOException
		private void readObject(ObjectInputStream ois)
		{
			ois.defaultReadObject();
			MatrixUtils.deserializeRealMatrix(this, "tableau", ois);
		}
	}

}
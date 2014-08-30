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
namespace org.apache.commons.math3.optim.linear
{


	/// <summary>
	/// A constraint for a linear optimization problem indicating whether all
	/// variables must be restricted to non-negative values.
	/// 
	/// @version $Id: SolutionCallback.java 1560541 2014-01-22 22:12:42Z tn $
	/// @since 3.3
	/// </summary>
	public class SolutionCallback : OptimizationData
	{
		/// <summary>
		/// The SimplexTableau used by the SimplexSolver. </summary>
		private SimplexTableau tableau;

		/// <summary>
		/// Set the simplex tableau used during the optimization once a feasible
		/// solution has been found.
		/// </summary>
		/// <param name="tableau"> the simplex tableau containing a feasible solution </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: void setTableau(final SimplexTableau tableau)
		internal virtual SimplexTableau Tableau
		{
			set
			{
				this.tableau = value;
			}
		}

		/// <summary>
		/// Retrieve the best solution found so far.
		/// <p>
		/// <b>Note:</b> the returned solution may not be optimal, e.g. in case
		/// the optimizer did reach the iteration limits.
		/// </summary>
		/// <returns> the best solution found so far by the optimizer, or {@code null} if
		/// no feasible solution could be found </returns>
		public virtual PointValuePair Solution
		{
			get
			{
				return tableau != null ? tableau.Solution : null;
			}
		}

		/// <summary>
		/// Returns if the found solution is optimal. </summary>
		/// <returns> {@code true} if the solution is optimal, {@code false} otherwise </returns>
		public virtual bool SolutionOptimal
		{
			get
			{
				return tableau != null ? tableau.Optimal : false;
			}
		}
	}

}
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

namespace mathlib.optimization
{

	using RealMatrix = mathlib.linear.RealMatrix;
	using DiagonalMatrix = mathlib.linear.DiagonalMatrix;
	using NonSquareMatrixException = mathlib.linear.NonSquareMatrixException;

	/// <summary>
	/// Weight matrix of the residuals between model and observations.
	/// <br/>
	/// Immutable class.
	/// 
	/// @version $Id: Weight.java 1591835 2014-05-02 09:04:01Z tn $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.1 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public class Weight : OptimizationData
	{
		/// <summary>
		/// Weight matrix. </summary>
		private readonly RealMatrix weightMatrix;

		/// <summary>
		/// Creates a diagonal weight matrix.
		/// </summary>
		/// <param name="weight"> List of the values of the diagonal. </param>
		public Weight(double[] weight)
		{
			weightMatrix = new DiagonalMatrix(weight);
		}

		/// <param name="weight"> Weight matrix. </param>
		/// <exception cref="NonSquareMatrixException"> if the argument is not
		/// a square matrix. </exception>
		public Weight(RealMatrix weight)
		{
			if (weight.ColumnDimension != weight.RowDimension)
			{
				throw new NonSquareMatrixException(weight.ColumnDimension, weight.RowDimension);
			}

			weightMatrix = weight.copy();
		}

		/// <summary>
		/// Gets the initial guess.
		/// </summary>
		/// <returns> the initial guess. </returns>
		public virtual RealMatrix Weight
		{
			get
			{
				return weightMatrix.copy();
			}
		}
	}

}
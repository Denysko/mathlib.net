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
namespace mathlib.linear
{

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;

	/// <summary>
	/// Exception to be thrown when a symmetric matrix is expected.
	/// 
	/// @since 3.0
	/// @version $Id: NonSymmetricMatrixException.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class NonSymmetricMatrixException : MathIllegalArgumentException
	{
		/// <summary>
		/// Serializable version Id. </summary>
		private const long serialVersionUID = -7518495577824189882L;
		/// <summary>
		/// Row. </summary>
		private readonly int row;
		/// <summary>
		/// Column. </summary>
		private readonly int column;
		/// <summary>
		/// Threshold. </summary>
		private readonly double threshold;

		/// <summary>
		/// Construct an exception.
		/// </summary>
		/// <param name="row"> Row index. </param>
		/// <param name="column"> Column index. </param>
		/// <param name="threshold"> Relative symmetry threshold. </param>
		public NonSymmetricMatrixException(int row, int column, double threshold) : base(LocalizedFormats.NON_SYMMETRIC_MATRIX, row, column, threshold)
		{
			this.row = row;
			this.column = column;
			this.threshold = threshold;
		}

		/// <returns> the row index of the entry. </returns>
		public virtual int Row
		{
			get
			{
				return row;
			}
		}
		/// <returns> the column index of the entry. </returns>
		public virtual int Column
		{
			get
			{
				return column;
			}
		}
		/// <returns> the relative symmetry threshold. </returns>
		public virtual double Threshold
		{
			get
			{
				return threshold;
			}
		}
	}

}
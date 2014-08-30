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
namespace org.apache.commons.math3.linear
{

	using MultiDimensionMismatchException = org.apache.commons.math3.exception.MultiDimensionMismatchException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// Exception to be thrown when either the number of rows or the number of
	/// columns of a matrix do not match the expected values.
	/// 
	/// @since 3.0
	/// @version $Id: MatrixDimensionMismatchException.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class MatrixDimensionMismatchException : MultiDimensionMismatchException
	{
		/// <summary>
		/// Serializable version Id. </summary>
		private const long serialVersionUID = -8415396756375798143L;

		/// <summary>
		/// Construct an exception from the mismatched dimensions.
		/// </summary>
		/// <param name="wrongRowDim"> Wrong row dimension. </param>
		/// <param name="wrongColDim"> Wrong column dimension. </param>
		/// <param name="expectedRowDim"> Expected row dimension. </param>
		/// <param name="expectedColDim"> Expected column dimension. </param>
		public MatrixDimensionMismatchException(int wrongRowDim, int wrongColDim, int expectedRowDim, int expectedColDim) : base(LocalizedFormats.DIMENSIONS_MISMATCH_2x2, new int?[] {wrongRowDim, wrongColDim}, new int?[] {expectedRowDim, expectedColDim})
		{
		}

		/// <returns> the expected row dimension. </returns>
		public virtual int WrongRowDimension
		{
			get
			{
				return getWrongDimension(0);
			}
		}
		/// <returns> the expected row dimension. </returns>
		public virtual int ExpectedRowDimension
		{
			get
			{
				return getExpectedDimension(0);
			}
		}
		/// <returns> the wrong column dimension. </returns>
		public virtual int WrongColumnDimension
		{
			get
			{
				return getWrongDimension(1);
			}
		}
		/// <returns> the expected column dimension. </returns>
		public virtual int ExpectedColumnDimension
		{
			get
			{
				return getExpectedDimension(1);
			}
		}
	}

}
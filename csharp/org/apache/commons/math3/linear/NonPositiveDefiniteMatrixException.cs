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

	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using ExceptionContext = org.apache.commons.math3.exception.util.ExceptionContext;

	/// <summary>
	/// Exception to be thrown when a positive definite matrix is expected.
	/// 
	/// @since 3.0
	/// @version $Id: NonPositiveDefiniteMatrixException.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class NonPositiveDefiniteMatrixException : NumberIsTooSmallException
	{
		/// <summary>
		/// Serializable version Id. </summary>
		private const long serialVersionUID = 1641613838113738061L;
		/// <summary>
		/// Index (diagonal element). </summary>
		private readonly int index;
		/// <summary>
		/// Threshold. </summary>
		private readonly double threshold;

		/// <summary>
		/// Construct an exception.
		/// </summary>
		/// <param name="wrong"> Value that fails the positivity check. </param>
		/// <param name="index"> Row (and column) index. </param>
		/// <param name="threshold"> Absolute positivity threshold. </param>
		public NonPositiveDefiniteMatrixException(double wrong, int index, double threshold) : base(wrong, threshold, false)
		{
			this.index = index;
			this.threshold = threshold;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.exception.util.ExceptionContext context = getContext();
			ExceptionContext context = Context;
			context.addMessage(LocalizedFormats.NOT_POSITIVE_DEFINITE_MATRIX);
			context.addMessage(LocalizedFormats.ARRAY_ELEMENT, wrong, index);
		}

		/// <returns> the row index. </returns>
		public virtual int Row
		{
			get
			{
				return index;
			}
		}
		/// <returns> the column index. </returns>
		public virtual int Column
		{
			get
			{
				return index;
			}
		}
		/// <returns> the absolute positivity threshold. </returns>
		public virtual double Threshold
		{
			get
			{
				return threshold;
			}
		}
	}

}
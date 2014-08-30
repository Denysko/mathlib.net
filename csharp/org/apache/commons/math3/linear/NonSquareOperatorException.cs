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

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// Exception to be thrown when a square linear operator is expected.
	/// 
	/// @since 3.0
	/// @version $Id: NonSquareOperatorException.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class NonSquareOperatorException : DimensionMismatchException
	{
		/// <summary>
		/// Serializable version Id. </summary>
		private const long serialVersionUID = -4145007524150846242L;

		/// <summary>
		/// Construct an exception from the mismatched dimensions.
		/// </summary>
		/// <param name="wrong"> Row dimension. </param>
		/// <param name="expected"> Column dimension. </param>
		public NonSquareOperatorException(int wrong, int expected) : base(LocalizedFormats.NON_SQUARE_OPERATOR, wrong, expected)
		{
		}
	}

}
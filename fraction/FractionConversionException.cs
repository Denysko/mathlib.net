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

namespace mathlib.fraction
{

	using ConvergenceException = mathlib.exception.ConvergenceException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;

	/// <summary>
	/// Error thrown when a double value cannot be converted to a fraction
	/// in the allowed number of iterations.
	/// 
	/// @version $Id: FractionConversionException.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2
	/// </summary>
	public class FractionConversionException : ConvergenceException
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -4661812640132576263L;

		/// <summary>
		/// Constructs an exception with specified formatted detail message.
		/// Message formatting is delegated to <seealso cref="java.text.MessageFormat"/>. </summary>
		/// <param name="value"> double value to convert </param>
		/// <param name="maxIterations"> maximal number of iterations allowed </param>
		public FractionConversionException(double value, int maxIterations) : base(LocalizedFormats.FAILED_FRACTION_CONVERSION, value, maxIterations)
		{
		}

		/// <summary>
		/// Constructs an exception with specified formatted detail message.
		/// Message formatting is delegated to <seealso cref="java.text.MessageFormat"/>. </summary>
		/// <param name="value"> double value to convert </param>
		/// <param name="p"> current numerator </param>
		/// <param name="q"> current denominator </param>
		public FractionConversionException(double value, long p, long q) : base(LocalizedFormats.FRACTION_CONVERSION_OVERFLOW, value, p, q)
		{
		}

	}

}
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
namespace mathlib.exception
{

    using Localizable = mathlib.exception.util.Localizable;

	/// <summary>
	/// Exception to be thrown when the argument is negative.
	/// 
	/// @since 2.2
	/// @version $Id: NotPositiveException.java 1504729 2013-07-18 23:54:52Z sebb $
	/// </summary>
	public class NotPositiveException : NumberIsTooSmallException
	{
		/// <summary>
		/// Serializable version Id. </summary>
		private const long serialVersionUID = -2250556892093726375L;

		/// <summary>
		/// Construct the exception.
		/// </summary>
		/// <param name="value"> Argument. </param>
		public NotPositiveException(Number value) : base(value, INTEGER_ZERO, true)
		{
		}
		/// <summary>
		/// Construct the exception with a specific context.
		/// </summary>
		/// <param name="specific"> Specific context where the error occurred. </param>
		/// <param name="value"> Argument. </param>
		public NotPositiveException(Localizable specific, Number value) : base(specific, value, INTEGER_ZERO, true)
		{
		}
	}

}
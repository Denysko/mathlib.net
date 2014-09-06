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
    using LocalizedFormats = mathlib.exception.util.LocalizedFormats;

	/// <summary>
	/// Exception to be thrown when a number is too large.
	/// 
	/// @since 2.2
	/// @version $Id: NumberIsTooLargeException.java 1364378 2012-07-22 17:42:38Z tn $
	/// </summary>
	public class NumberIsTooLargeException : MathIllegalNumberException
	{
		/// <summary>
		/// Serializable version Id. </summary>
		private const long serialVersionUID = 4330003017885151975L;
		/// <summary>
		/// Higher bound.
		/// </summary>
		private readonly Number max;
		/// <summary>
		/// Whether the maximum is included in the allowed range.
		/// </summary>
		private readonly bool boundIsAllowed;

		/// <summary>
		/// Construct the exception.
		/// </summary>
		/// <param name="wrong"> Value that is larger than the maximum. </param>
		/// <param name="max"> Maximum. </param>
		/// <param name="boundIsAllowed"> if true the maximum is included in the allowed range. </param>
		public NumberIsTooLargeException(Number wrong, Number max, bool boundIsAllowed) : this(boundIsAllowed ? LocalizedFormats.NUMBER_TOO_LARGE : LocalizedFormats.NUMBER_TOO_LARGE_BOUND_EXCLUDED, wrong, max, boundIsAllowed)
		{
		}
		/// <summary>
		/// Construct the exception with a specific context.
		/// </summary>
		/// <param name="specific"> Specific context pattern. </param>
		/// <param name="wrong"> Value that is larger than the maximum. </param>
		/// <param name="max"> Maximum. </param>
		/// <param name="boundIsAllowed"> if true the maximum is included in the allowed range. </param>
		public NumberIsTooLargeException(Localizable specific, Number wrong, Number max, bool boundIsAllowed) : base(specific, wrong, max)
		{

			this.max = max;
			this.boundIsAllowed = boundIsAllowed;
		}

		/// <returns> {@code true} if the maximum is included in the allowed range. </returns>
		public virtual bool BoundIsAllowed
		{
			get
			{
				return boundIsAllowed;
			}
		}

		/// <returns> the maximum. </returns>
		public virtual Number Max
		{
			get
			{
				return max;
			}
		}
	}

}
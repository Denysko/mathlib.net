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
namespace org.apache.commons.math3.exception
{

	using Localizable = org.apache.commons.math3.exception.util.Localizable;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// Exception to be thrown when zero is provided where it is not allowed.
	/// 
	/// @since 2.2
	/// @version $Id: ZeroException.java 1504729 2013-07-18 23:54:52Z sebb $
	/// </summary>
	public class ZeroException : MathIllegalNumberException
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -1960874856936000015L;

		/// <summary>
		/// Construct the exception.
		/// </summary>
		public ZeroException() : this(LocalizedFormats.ZERO_NOT_ALLOWED)
		{
		}

		/// <summary>
		/// Construct the exception with a specific context.
		/// </summary>
		/// <param name="specific"> Specific context pattern. </param>
		/// <param name="arguments"> Arguments. </param>
		public ZeroException(Localizable specific, params object[] arguments) : base(specific, INTEGER_ZERO, arguments)
		{
		}
	}

}
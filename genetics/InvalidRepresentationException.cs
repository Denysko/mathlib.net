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
namespace mathlib.genetics
{

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using Localizable = mathlib.exception.util.Localizable;

	/// <summary>
	/// Exception indicating that the representation of a chromosome is not valid.
	/// 
	/// @version $Id: InvalidRepresentationException.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0
	/// </summary>
	public class InvalidRepresentationException : MathIllegalArgumentException
	{

		/// <summary>
		/// Serialization version id </summary>
		private const long serialVersionUID = 1L;

		/// <summary>
		/// Construct an InvalidRepresentationException with a specialized message.
		/// </summary>
		/// <param name="pattern"> Message pattern. </param>
		/// <param name="args"> Arguments. </param>
		public InvalidRepresentationException(Localizable pattern, params object[] args) : base(pattern, args)
		{
		}

	}

}
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

namespace org.apache.commons.math3.geometry.euclidean.threed
{

	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using Localizable = org.apache.commons.math3.exception.util.Localizable;

	/// <summary>
	/// This class represents exceptions thrown while building rotations
	/// from matrices.
	/// 
	/// @version $Id: NotARotationMatrixException.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2
	/// </summary>

	public class NotARotationMatrixException : MathIllegalArgumentException
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 5647178478658937642L;

		/// <summary>
		/// Simple constructor.
		/// Build an exception by translating and formating a message </summary>
		/// <param name="specifier"> format specifier (to be translated) </param>
		/// <param name="parts"> to insert in the format (no translation)
		/// @since 2.2 </param>
		public NotARotationMatrixException(Localizable specifier, params object[] parts) : base(specifier, parts)
		{
		}

	}

}
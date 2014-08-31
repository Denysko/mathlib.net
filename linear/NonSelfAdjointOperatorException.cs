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

	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// Exception to be thrown when a self-adjoint <seealso cref="RealLinearOperator"/>
	/// is expected.
	/// Since the coefficients of the matrix are not accessible, the most
	/// general definition is used to check that A is not self-adjoint, i.e.
	/// there exist x and y such as {@code | x' A y - y' A x | >= eps},
	/// where {@code eps} is a user-specified tolerance, and {@code x'}
	/// denotes the transpose of {@code x}.
	/// In the terminology of this exception, {@code A} is the "offending"
	/// linear operator, {@code x} and {@code y} are the first and second
	/// "offending" vectors, respectively.
	/// 
	/// @version $Id: NonSelfAdjointOperatorException.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0
	/// </summary>
	public class NonSelfAdjointOperatorException : MathIllegalArgumentException
	{
		/// <summary>
		/// Serializable version Id. </summary>
		private const long serialVersionUID = 1784999305030258247L;

		/// <summary>
		/// Creates a new instance of this class. </summary>
		public NonSelfAdjointOperatorException() : base(LocalizedFormats.NON_SELF_ADJOINT_OPERATOR)
		{
		}
	}

}
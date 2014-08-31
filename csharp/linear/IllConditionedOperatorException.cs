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
	/// An exception to be thrown when the condition number of a
	/// <seealso cref="RealLinearOperator"/> is too high.
	/// 
	/// @version $Id: IllConditionedOperatorException.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0
	/// </summary>
	public class IllConditionedOperatorException : MathIllegalArgumentException
	{
		/// <summary>
		/// Serializable version Id. </summary>
		private const long serialVersionUID = -7883263944530490135L;

		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="cond"> An estimate of the condition number of the offending linear
		/// operator. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public IllConditionedOperatorException(final double cond)
		public IllConditionedOperatorException(double cond) : base(LocalizedFormats.ILL_CONDITIONED_OPERATOR, cond)
		{
		}
	}

}
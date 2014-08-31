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
namespace org.apache.commons.math3.ode
{

	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// Exception to be thrown when a parameter is unknown.
	/// 
	/// @since 3.1
	/// @version $Id: UnknownParameterException.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class UnknownParameterException : MathIllegalArgumentException
	{

		/// <summary>
		/// Serializable version Id. </summary>
		private const long serialVersionUID = 20120902L;

		/// <summary>
		/// Parameter name. </summary>
		private readonly string name;

		/// <summary>
		/// Construct an exception from the unknown parameter.
		/// </summary>
		/// <param name="name"> parameter name. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public UnknownParameterException(final String name)
		public UnknownParameterException(string name) : base(LocalizedFormats.UNKNOWN_PARAMETER)
		{
			this.name = name;
		}

		/// <returns> the name of the unknown parameter. </returns>
		public virtual string Name
		{
			get
			{
				return name;
			}
		}

	}

}
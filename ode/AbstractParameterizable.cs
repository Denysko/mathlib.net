using System.Collections.Generic;

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
namespace mathlib.ode
{


	/// <summary>
	/// This abstract class provides boilerplate parameters list.
	/// 
	/// @version $Id: AbstractParameterizable.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0
	/// </summary>

	public abstract class AbstractParameterizable : Parameterizable
	{

	   /// <summary>
	   /// List of the parameters names. </summary>
		private readonly ICollection<string> parametersNames;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="names"> names of the supported parameters </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractParameterizable(final String... names)
		protected internal AbstractParameterizable(params string[] names)
		{
			parametersNames = new List<string>();
			foreach (String name in names)
			{
				parametersNames.Add(name);
			}
		}

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="names"> names of the supported parameters </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractParameterizable(final java.util.Collection<String> names)
		protected internal AbstractParameterizable(ICollection<string> names)
		{
			parametersNames = new List<string>();
			parametersNames.addAll(names);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual ICollection<string> ParametersNames
		{
			get
			{
				return parametersNames;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean isSupported(final String name)
		public virtual bool isSupported(string name)
		{
			foreach (String supportedName in parametersNames)
			{
				if (supportedName.Equals(name))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Check if a parameter is supported and throw an IllegalArgumentException if not. </summary>
		/// <param name="name"> name of the parameter to check </param>
		/// <exception cref="UnknownParameterException"> if the parameter is not supported </exception>
		/// <seealso cref= #isSupported(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void complainIfNotSupported(final String name) throws UnknownParameterException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void complainIfNotSupported(string name)
		{
			if (!isSupported(name))
			{
				throw new UnknownParameterException(name);
			}
		}

	}

}
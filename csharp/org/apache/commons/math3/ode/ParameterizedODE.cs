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


	/// <summary>
	/// Interface to compute by finite difference Jacobian matrix for some parameter
	///  when computing <seealso cref="JacobianMatrices partial derivatives equations"/>.
	/// 
	/// @version $Id: ParameterizedODE.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0
	/// </summary>

	public interface ParameterizedODE : Parameterizable
	{

		/// <summary>
		/// Get parameter value from its name. </summary>
		/// <param name="name"> parameter name </param>
		/// <returns> parameter value </returns>
		/// <exception cref="UnknownParameterException"> if parameter is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double getParameter(String name) throws UnknownParameterException;
		double getParameter(string name);

		/// <summary>
		/// Set the value for a given parameter. </summary>
		/// <param name="name"> parameter name </param>
		/// <param name="value"> parameter value </param>
		/// <exception cref="UnknownParameterException"> if parameter is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setParameter(String name, double value) throws UnknownParameterException;
		void setParameter(string name, double value);

	}

}
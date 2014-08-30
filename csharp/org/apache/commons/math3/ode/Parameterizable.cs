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
namespace org.apache.commons.math3.ode
{

	/// <summary>
	/// This interface enables to process any parameterizable object.
	/// 
	/// @version $Id: Parameterizable.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0
	/// </summary>

	public interface Parameterizable
	{

		/// <summary>
		/// Get the names of the supported parameters. </summary>
		/// <returns> parameters names </returns>
		/// <seealso cref= #isSupported(String) </seealso>
		ICollection<string> ParametersNames {get;}

		/// <summary>
		/// Check if a parameter is supported.
		/// <p>Supported parameters are those listed by <seealso cref="#getParametersNames()"/>.</p> </summary>
		/// <param name="name"> parameter name to check </param>
		/// <returns> true if the parameter is supported </returns>
		/// <seealso cref= #getParametersNames() </seealso>
		bool isSupported(string name);

	}

}
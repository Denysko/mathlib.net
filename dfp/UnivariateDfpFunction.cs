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
namespace mathlib.dfp
{

	/// <summary>
	/// An interface representing a univariate <seealso cref="Dfp"/> function.
	/// 
	/// @version $Id: UnivariateDfpFunction.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public interface UnivariateDfpFunction
	{

		/// <summary>
		/// Compute the value of the function.
		/// </summary>
		/// <param name="x"> Point at which the function value should be computed. </param>
		/// <returns> the value. </returns>
		/// <exception cref="IllegalArgumentException"> when the activated method itself can
		/// ascertain that preconditions, specified in the API expressed at the
		/// level of the activated method, have been violated.  In the vast
		/// majority of cases where Commons-Math throws IllegalArgumentException,
		/// it is the result of argument checking of actual parameters immediately
		/// passed to a method. </exception>
		Dfp value(Dfp x);

	}

}
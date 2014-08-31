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
namespace org.apache.commons.math3.analysis.differentiation
{

	/// <summary>
	/// Interface defining the function differentiation operation.
	/// @version $Id: UnivariateVectorFunctionDifferentiator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.1
	/// </summary>
	public interface UnivariateVectorFunctionDifferentiator
	{

		/// <summary>
		/// Create an implementation of a {@link UnivariateDifferentiableVectorFunction
		/// differential} from a regular <seealso cref="UnivariateVectorFunction vector function"/>. </summary>
		/// <param name="function"> function to differentiate </param>
		/// <returns> differential function </returns>
		UnivariateDifferentiableVectorFunction differentiate(UnivariateVectorFunction function);

	}

}
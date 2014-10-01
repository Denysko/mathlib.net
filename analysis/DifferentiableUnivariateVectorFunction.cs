using System;

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
namespace mathlib.analysis
{

	/// <summary>
	/// Extension of <seealso cref="UnivariateVectorFunction"/> representing a differentiable univariate vectorial function.
	/// 
	/// @version $Id: DifferentiableUnivariateVectorFunction.java 1499808 2013-07-04 17:00:42Z sebb $
	/// @since 2.0 </summary>
	/// @deprecated as of 3.1 replaced by <seealso cref="mathlib.analysis.differentiation.UnivariateDifferentiableVectorFunction"/> 
	[Obsolete]//("as of 3.1 replaced by <seealso cref=mathlib.analysis.differentiation.UnivariateDifferentiableVectorFunction"/>")]
	public interface DifferentiableUnivariateVectorFunction : UnivariateVectorFunction
	{

		/// <summary>
		/// Returns the derivative of the function
		/// </summary>
		/// <returns>  the derivative function </returns>
		UnivariateVectorFunction derivative();

	}

}
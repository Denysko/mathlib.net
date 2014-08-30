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

namespace org.apache.commons.math3.analysis
{

	/// <summary>
	/// An interface representing a multivariate real function.
	/// 
	/// @version $Id: MultivariateFunction.java 1364387 2012-07-22 18:14:11Z tn $
	/// @since 2.0
	/// </summary>
	public interface MultivariateFunction
	{

		/// <summary>
		/// Compute the value for the function at the given point.
		/// </summary>
		/// <param name="point"> Point at which the function must be evaluated. </param>
		/// <returns> the function value for the given point. </returns>
		/// <exception cref="org.apache.commons.math3.exception.DimensionMismatchException">
		/// if the parameter's dimension is wrong for the function being evaluated. </exception>
		/// <exception cref="org.apache.commons.math3.exception.MathIllegalArgumentException">
		/// when the activated method itself can ascertain that preconditions,
		/// specified in the API expressed at the level of the activated method,
		/// have been violated.  In the vast majority of cases where Commons Math
		/// throws this exception, it is the result of argument checking of actual
		/// parameters immediately passed to a method. </exception>
		double value(double[] point);
	}

}
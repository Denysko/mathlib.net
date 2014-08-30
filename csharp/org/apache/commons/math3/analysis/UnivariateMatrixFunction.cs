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
	/// An interface representing a univariate matrix function.
	/// 
	/// @version $Id: UnivariateMatrixFunction.java 1364387 2012-07-22 18:14:11Z tn $
	/// @since 2.0
	/// </summary>
	public interface UnivariateMatrixFunction
	{

		/// <summary>
		/// Compute the value for the function. </summary>
		/// <param name="x"> the point for which the function value should be computed </param>
		/// <returns> the value </returns>
		double[][] value(double x);

	}

}
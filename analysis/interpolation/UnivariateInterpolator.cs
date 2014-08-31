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
namespace org.apache.commons.math3.analysis.interpolation
{

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;

	/// <summary>
	/// Interface representing a univariate real interpolating function.
	/// 
	/// @version $Id: UnivariateInterpolator.java 1455194 2013-03-11 15:45:54Z luc $
	/// </summary>
	public interface UnivariateInterpolator
	{
		/// <summary>
		/// Compute an interpolating function for the dataset.
		/// </summary>
		/// <param name="xval"> Arguments for the interpolation points. </param>
		/// <param name="yval"> Values for the interpolation points. </param>
		/// <returns> a function which interpolates the dataset. </returns>
		/// <exception cref="MathIllegalArgumentException">
		/// if the arguments violate assumptions made by the interpolation
		/// algorithm. </exception>
		/// <exception cref="DimensionMismatchException"> if arrays lengthes do not match </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: org.apache.commons.math3.analysis.UnivariateFunction interpolate(double xval[] , double yval[]) throws org.apache.commons.math3.exception.MathIllegalArgumentException, org.apache.commons.math3.exception.DimensionMismatchException;
		UnivariateFunction interpolate(double[] xval, double[] yval);
	}

}
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
namespace mathlib.analysis.interpolation
{

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NoDataException = mathlib.exception.NoDataException;
	using NullArgumentException = mathlib.exception.NullArgumentException;

	/// <summary>
	/// Interface representing a univariate real interpolating function.
	/// 
	/// @since 2.1
	/// @version $Id: MultivariateInterpolator.java 1455194 2013-03-11 15:45:54Z luc $
	/// </summary>
	public interface MultivariateInterpolator
	{

		/// <summary>
		/// Computes an interpolating function for the data set.
		/// </summary>
		/// <param name="xval"> the arguments for the interpolation points.
		/// {@code xval[i][0]} is the first component of interpolation point
		/// {@code i}, {@code xval[i][1]} is the second component, and so on
		/// until {@code xval[i][d-1]}, the last component of that interpolation
		/// point (where {@code d} is thus the dimension of the space). </param>
		/// <param name="yval"> the values for the interpolation points </param>
		/// <returns> a function which interpolates the data set </returns>
		/// <exception cref="MathIllegalArgumentException"> if the arguments violate assumptions
		/// made by the interpolation algorithm. </exception>
		/// <exception cref="DimensionMismatchException"> when the array dimensions are not consistent. </exception>
		/// <exception cref="NoDataException"> if an array has zero-length. </exception>
		/// <exception cref="NullArgumentException"> if the arguments are {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: mathlib.analysis.MultivariateFunction interpolate(double[][] xval, double[] yval) throws mathlib.exception.MathIllegalArgumentException, mathlib.exception.DimensionMismatchException, mathlib.exception.NoDataException, mathlib.exception.NullArgumentException;
		MultivariateFunction interpolate(double[][] xval, double[] yval);
	}

}
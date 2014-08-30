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
	using NoDataException = org.apache.commons.math3.exception.NoDataException;
	using NonMonotonicSequenceException = org.apache.commons.math3.exception.NonMonotonicSequenceException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;

	/// <summary>
	/// Interface representing a bivariate real interpolating function where the
	/// sample points must be specified on a regular grid.
	/// 
	/// @version $Id: BivariateGridInterpolator.java 1455194 2013-03-11 15:45:54Z luc $
	/// </summary>
	public interface BivariateGridInterpolator
	{
		/// <summary>
		/// Compute an interpolating function for the dataset.
		/// </summary>
		/// <param name="xval"> All the x-coordinates of the interpolation points, sorted
		/// in increasing order. </param>
		/// <param name="yval"> All the y-coordinates of the interpolation points, sorted
		/// in increasing order. </param>
		/// <param name="fval"> The values of the interpolation points on all the grid knots:
		/// {@code fval[i][j] = f(xval[i], yval[j])}. </param>
		/// <returns> a function which interpolates the dataset. </returns>
		/// <exception cref="NoDataException"> if any of the arrays has zero length. </exception>
		/// <exception cref="DimensionMismatchException"> if the array lengths are inconsistent. </exception>
		/// <exception cref="NonMonotonicSequenceException"> if the array is not sorted. </exception>
		/// <exception cref="NumberIsTooSmallException"> if the number of points is too small for
		/// the order of the interpolation </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: org.apache.commons.math3.analysis.BivariateFunction interpolate(double[] xval, double[] yval, double[][] fval) throws org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NonMonotonicSequenceException, org.apache.commons.math3.exception.NumberIsTooSmallException;
		BivariateFunction interpolate(double[] xval, double[] yval, double[][] fval);
	}

}
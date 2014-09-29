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
namespace mathlib.analysis.interpolation
{

	using PolynomialFunctionLagrangeForm = mathlib.analysis.polynomials.PolynomialFunctionLagrangeForm;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using NonMonotonicSequenceException = mathlib.exception.NonMonotonicSequenceException;

	/// <summary>
	/// Implements the <a href="http://mathworld.wolfram.com/NevillesAlgorithm.html">
	/// Neville's Algorithm</a> for interpolation of real univariate functions. For
	/// reference, see <b>Introduction to Numerical Analysis</b>, ISBN 038795452X,
	/// chapter 2.
	/// <p>
	/// The actual code of Neville's algorithm is in PolynomialFunctionLagrangeForm,
	/// this class provides an easy-to-use interface to it.</p>
	/// 
	/// @version $Id: NevilleInterpolator.java 1379904 2012-09-01 23:54:52Z erans $
	/// @since 1.2
	/// </summary>
	[Serializable]
	public class NevilleInterpolator : UnivariateInterpolator
	{

		/// <summary>
		/// serializable version identifier </summary>
		internal const long serialVersionUID = 3003707660147873733L;

		/// <summary>
		/// Computes an interpolating function for the data set.
		/// </summary>
		/// <param name="x"> Interpolating points. </param>
		/// <param name="y"> Interpolating values. </param>
		/// <returns> a function which interpolates the data set </returns>
		/// <exception cref="DimensionMismatchException"> if the array lengths are different. </exception>
		/// <exception cref="NumberIsTooSmallException"> if the number of points is less than 2. </exception>
		/// <exception cref="NonMonotonicSequenceException"> if two abscissae have the same
		/// value. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public mathlib.analysis.polynomials.PolynomialFunctionLagrangeForm interpolate(double x[] , double y[]) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.NonMonotonicSequenceException
		public virtual PolynomialFunctionLagrangeForm interpolate(double[] x, double[] y)
		{
			return new PolynomialFunctionLagrangeForm(x, y);
		}
	}

}
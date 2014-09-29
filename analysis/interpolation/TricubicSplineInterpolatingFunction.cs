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
	using NoDataException = mathlib.exception.NoDataException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using NonMonotonicSequenceException = mathlib.exception.NonMonotonicSequenceException;
	using MathArrays = mathlib.util.MathArrays;

	/// <summary>
	/// Function that implements the
	/// <a href="http://en.wikipedia.org/wiki/Tricubic_interpolation">
	/// tricubic spline interpolation</a>, as proposed in
	/// <quote>
	///  Tricubic interpolation in three dimensions<br/>
	///  F. Lekien and J. Marsden<br/>
	///  <em>Int. J. Numer. Meth. Engng</em> 2005; <b>63</b>:455-471
	/// </quote>
	/// 
	/// @since 2.2
	/// @version $Id: TricubicSplineInterpolatingFunction.java 1385314 2012-09-16 16:35:49Z tn $
	/// </summary>
	public class TricubicSplineInterpolatingFunction : TrivariateFunction
	{
		/// <summary>
		/// Matrix to compute the spline coefficients from the function values
		/// and function derivatives values
		/// </summary>
		private static readonly double[][] AINV = new double[][] {new double[] {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {-3,3,0,0,0,0,0,0,-2,-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {2,-2,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-2,-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,-2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {-3,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,-2,0,-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,-3,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-2,0,-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {9,-9,-9,9,0,0,0,0,6,3,-6,-3,0,0,0,0,6,-6,3,-3,0,0,0,0,0,0,0,0,0,0,0,0,4,2,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {-6,6,6,-6,0,0,0,0,-3,-3,3,3,0,0,0,0,-4,4,-2,2,0,0,0,0,0,0,0,0,0,0,0,0,-2,-2,-1,-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {2,0,-2,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,2,0,-2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {-6,6,6,-6,0,0,0,0,-4,-2,4,2,0,0,0,0,-3,3,-3,3,0,0,0,0,0,0,0,0,0,0,0,0,-2,-1,-2,-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {4,-4,-4,4,0,0,0,0,2,2,-2,-2,0,0,0,0,2,-2,2,-2,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-2,-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,-2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-3,3,0,0,0,0,0,0,-2,-1,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,-2,0,0,0,0,0,0,1,1,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-3,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-2,0,-1,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-3,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,-2,0,-1,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,-9,-9,9,0,0,0,0,0,0,0,0,0,0,0,0,6,3,-6,-3,0,0,0,0,6,-6,3,-3,0,0,0,0,4,2,2,1,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-6,6,6,-6,0,0,0,0,0,0,0,0,0,0,0,0,-3,-3,3,3,0,0,0,0,-4,4,-2,2,0,0,0,0,-2,-2,-1,-1,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,-2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,-2,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-6,6,6,-6,0,0,0,0,0,0,0,0,0,0,0,0,-4,-2,4,2,0,0,0,0,-3,3,-3,3,0,0,0,0,-2,-1,-2,-1,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,-4,-4,4,0,0,0,0,0,0,0,0,0,0,0,0,2,2,-2,-2,0,0,0,0,2,-2,2,-2,0,0,0,0,1,1,1,1,0,0,0,0}, new double[] {-3,0,0,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-2,0,0,0,-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,-3,0,0,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-2,0,0,0,-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {9,-9,0,0,-9,9,0,0,6,3,0,0,-6,-3,0,0,0,0,0,0,0,0,0,0,6,-6,0,0,3,-3,0,0,0,0,0,0,0,0,0,0,4,2,0,0,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {-6,6,0,0,6,-6,0,0,-3,-3,0,0,3,3,0,0,0,0,0,0,0,0,0,0,-4,4,0,0,-2,2,0,0,0,0,0,0,0,0,0,0,-2,-2,0,0,-1,-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-3,0,0,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-2,0,0,0,-1,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-3,0,0,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-2,0,0,0,-1,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,-9,0,0,-9,9,0,0,0,0,0,0,0,0,0,0,6,3,0,0,-6,-3,0,0,0,0,0,0,0,0,0,0,6,-6,0,0,3,-3,0,0,4,2,0,0,2,1,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-6,6,0,0,6,-6,0,0,0,0,0,0,0,0,0,0,-3,-3,0,0,3,3,0,0,0,0,0,0,0,0,0,0,-4,4,0,0,-2,2,0,0,-2,-2,0,0,-1,-1,0,0}, new double[] {9,0,-9,0,-9,0,9,0,0,0,0,0,0,0,0,0,6,0,3,0,-6,0,-3,0,6,0,-6,0,3,0,-3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,2,0,2,0,1,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,9,0,-9,0,-9,0,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,0,3,0,-6,0,-3,0,6,0,-6,0,3,0,-3,0,0,0,0,0,0,0,0,0,4,0,2,0,2,0,1,0}, new double[] {-27,27,27,-27,27,-27,-27,27,-18,-9,18,9,18,9,-18,-9,-18,18,-9,9,18,-18,9,-9,-18,18,18,-18,-9,9,9,-9,-12,-6,-6,-3,12,6,6,3,-12,-6,12,6,-6,-3,6,3,-12,12,-6,6,-6,6,-3,3,-8,-4,-4,-2,-4,-2,-2,-1}, new double[] {18,-18,-18,18,-18,18,18,-18,9,9,-9,-9,-9,-9,9,9,12,-12,6,-6,-12,12,-6,6,12,-12,-12,12,6,-6,-6,6,6,6,3,3,-6,-6,-3,-3,6,6,-6,-6,3,3,-3,-3,8,-8,4,-4,4,-4,2,-2,4,4,2,2,2,2,1,1}, new double[] {-6,0,6,0,6,0,-6,0,0,0,0,0,0,0,0,0,-3,0,-3,0,3,0,3,0,-4,0,4,0,-2,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-2,0,-2,0,-1,0,-1,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,-6,0,6,0,6,0,-6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-3,0,-3,0,3,0,3,0,-4,0,4,0,-2,0,2,0,0,0,0,0,0,0,0,0,-2,0,-2,0,-1,0,-1,0}, new double[] {18,-18,-18,18,-18,18,18,-18,12,6,-12,-6,-12,-6,12,6,9,-9,9,-9,-9,9,-9,9,12,-12,-12,12,6,-6,-6,6,6,3,6,3,-6,-3,-6,-3,8,4,-8,-4,4,2,-4,-2,6,-6,6,-6,3,-3,3,-3,4,2,4,2,2,1,2,1}, new double[] {-12,12,12,-12,12,-12,-12,12,-6,-6,6,6,6,6,-6,-6,-6,6,-6,6,6,-6,6,-6,-8,8,8,-8,-4,4,4,-4,-3,-3,-3,-3,3,3,3,3,-4,-4,4,4,-2,-2,2,2,-4,4,-4,4,-2,2,-2,2,-2,-2,-2,-2,-1,-1,-1,-1}, new double[] {2,0,0,0,-2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,2,0,0,0,-2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {-6,6,0,0,6,-6,0,0,-4,-2,0,0,4,2,0,0,0,0,0,0,0,0,0,0,-3,3,0,0,-3,3,0,0,0,0,0,0,0,0,0,0,-2,-1,0,0,-2,-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {4,-4,0,0,-4,4,0,0,2,2,0,0,-2,-2,0,0,0,0,0,0,0,0,0,0,2,-2,0,0,2,-2,0,0,0,0,0,0,0,0,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,-2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,-2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-6,6,0,0,6,-6,0,0,0,0,0,0,0,0,0,0,-4,-2,0,0,4,2,0,0,0,0,0,0,0,0,0,0,-3,3,0,0,-3,3,0,0,-2,-1,0,0,-2,-1,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,-4,0,0,-4,4,0,0,0,0,0,0,0,0,0,0,2,2,0,0,-2,-2,0,0,0,0,0,0,0,0,0,0,2,-2,0,0,2,-2,0,0,1,1,0,0,1,1,0,0}, new double[] {-6,0,6,0,6,0,-6,0,0,0,0,0,0,0,0,0,-4,0,-2,0,4,0,2,0,-3,0,3,0,-3,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-2,0,-1,0,-2,0,-1,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,-6,0,6,0,6,0,-6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-4,0,-2,0,4,0,2,0,-3,0,3,0,-3,0,3,0,0,0,0,0,0,0,0,0,-2,0,-1,0,-2,0,-1,0}, new double[] {18,-18,-18,18,-18,18,18,-18,12,6,-12,-6,-12,-6,12,6,12,-12,6,-6,-12,12,-6,6,9,-9,-9,9,9,-9,-9,9,8,4,4,2,-8,-4,-4,-2,6,3,-6,-3,6,3,-6,-3,6,-6,3,-3,6,-6,3,-3,4,2,2,1,4,2,2,1}, new double[] {-12,12,12,-12,12,-12,-12,12,-6,-6,6,6,6,6,-6,-6,-8,8,-4,4,8,-8,4,-4,-6,6,6,-6,-6,6,6,-6,-4,-4,-2,-2,4,4,2,2,-3,-3,3,3,-3,-3,3,3,-4,4,-2,2,-4,4,-2,2,-2,-2,-1,-1,-2,-2,-1,-1}, new double[] {4,0,-4,0,-4,0,4,0,0,0,0,0,0,0,0,0,2,0,2,0,-2,0,-2,0,2,0,-2,0,2,0,-2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,1,0,1,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,4,0,-4,0,-4,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,2,0,-2,0,-2,0,2,0,-2,0,2,0,-2,0,0,0,0,0,0,0,0,0,1,0,1,0,1,0,1,0}, new double[] {-12,12,12,-12,12,-12,-12,12,-8,-4,8,4,8,4,-8,-4,-6,6,-6,6,6,-6,6,-6,-6,6,6,-6,-6,6,6,-6,-4,-2,-4,-2,4,2,4,2,-4,-2,4,2,-4,-2,4,2,-3,3,-3,3,-3,3,-3,3,-2,-1,-2,-1,-2,-1,-2,-1}, new double[] {8,-8,-8,8,-8,8,8,-8,4,4,-4,-4,-4,-4,4,4,4,-4,4,-4,-4,4,-4,4,4,-4,-4,4,4,-4,-4,4,2,2,2,2,-2,-2,-2,-2,2,2,-2,-2,2,2,-2,-2,2,-2,2,-2,2,-2,2,-2,1,1,1,1,1,1,1,1}};

		/// <summary>
		/// Samples x-coordinates </summary>
		private readonly double[] xval;
		/// <summary>
		/// Samples y-coordinates </summary>
		private readonly double[] yval;
		/// <summary>
		/// Samples z-coordinates </summary>
		private readonly double[] zval;
		/// <summary>
		/// Set of cubic splines pacthing the whole data grid </summary>
		private readonly TricubicSplineFunction[][][] splines;

		/// <param name="x"> Sample values of the x-coordinate, in increasing order. </param>
		/// <param name="y"> Sample values of the y-coordinate, in increasing order. </param>
		/// <param name="z"> Sample values of the y-coordinate, in increasing order. </param>
		/// <param name="f"> Values of the function on every grid point. </param>
		/// <param name="dFdX"> Values of the partial derivative of function with respect to x on every grid point. </param>
		/// <param name="dFdY"> Values of the partial derivative of function with respect to y on every grid point. </param>
		/// <param name="dFdZ"> Values of the partial derivative of function with respect to z on every grid point. </param>
		/// <param name="d2FdXdY"> Values of the cross partial derivative of function on every grid point. </param>
		/// <param name="d2FdXdZ"> Values of the cross partial derivative of function on every grid point. </param>
		/// <param name="d2FdYdZ"> Values of the cross partial derivative of function on every grid point. </param>
		/// <param name="d3FdXdYdZ"> Values of the cross partial derivative of function on every grid point. </param>
		/// <exception cref="NoDataException"> if any of the arrays has zero length. </exception>
		/// <exception cref="DimensionMismatchException"> if the various arrays do not contain the expected number of elements. </exception>
		/// <exception cref="NonMonotonicSequenceException"> if {@code x}, {@code y} or {@code z} are not strictly increasing. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TricubicSplineInterpolatingFunction(double[] x, double[] y, double[] z, double[][][] f, double[][][] dFdX, double[][][] dFdY, double[][][] dFdZ, double[][][] d2FdXdY, double[][][] d2FdXdZ, double[][][] d2FdYdZ, double[][][] d3FdXdYdZ) throws mathlib.exception.NoDataException, mathlib.exception.DimensionMismatchException, mathlib.exception.NonMonotonicSequenceException
		public TricubicSplineInterpolatingFunction(double[] x, double[] y, double[] z, double[][][] f, double[][][] dFdX, double[][][] dFdY, double[][][] dFdZ, double[][][] d2FdXdY, double[][][] d2FdXdZ, double[][][] d2FdYdZ, double[][][] d3FdXdYdZ)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int xLen = x.length;
			int xLen = x.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int yLen = y.length;
			int yLen = y.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int zLen = z.length;
			int zLen = z.Length;

			if (xLen == 0 || yLen == 0 || z.Length == 0 || f.Length == 0 || f[0].Length == 0)
			{
				throw new NoDataException();
			}
			if (xLen != f.Length)
			{
				throw new DimensionMismatchException(xLen, f.Length);
			}
			if (xLen != dFdX.Length)
			{
				throw new DimensionMismatchException(xLen, dFdX.Length);
			}
			if (xLen != dFdY.Length)
			{
				throw new DimensionMismatchException(xLen, dFdY.Length);
			}
			if (xLen != dFdZ.Length)
			{
				throw new DimensionMismatchException(xLen, dFdZ.Length);
			}
			if (xLen != d2FdXdY.Length)
			{
				throw new DimensionMismatchException(xLen, d2FdXdY.Length);
			}
			if (xLen != d2FdXdZ.Length)
			{
				throw new DimensionMismatchException(xLen, d2FdXdZ.Length);
			}
			if (xLen != d2FdYdZ.Length)
			{
				throw new DimensionMismatchException(xLen, d2FdYdZ.Length);
			}
			if (xLen != d3FdXdYdZ.Length)
			{
				throw new DimensionMismatchException(xLen, d3FdXdYdZ.Length);
			}

			MathArrays.checkOrder(x);
			MathArrays.checkOrder(y);
			MathArrays.checkOrder(z);

			xval = x.clone();
			yval = y.clone();
			zval = z.clone();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lastI = xLen - 1;
			int lastI = xLen - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lastJ = yLen - 1;
			int lastJ = yLen - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lastK = zLen - 1;
			int lastK = zLen - 1;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: splines = new TricubicSplineFunction[lastI][lastJ][lastK];
			splines = RectangularArrays.ReturnRectangularTricubicSplineFunctionArray(lastI, lastJ, lastK);

			for (int i = 0; i < lastI; i++)
			{
				if (f[i].Length != yLen)
				{
					throw new DimensionMismatchException(f[i].Length, yLen);
				}
				if (dFdX[i].Length != yLen)
				{
					throw new DimensionMismatchException(dFdX[i].Length, yLen);
				}
				if (dFdY[i].Length != yLen)
				{
					throw new DimensionMismatchException(dFdY[i].Length, yLen);
				}
				if (dFdZ[i].Length != yLen)
				{
					throw new DimensionMismatchException(dFdZ[i].Length, yLen);
				}
				if (d2FdXdY[i].Length != yLen)
				{
					throw new DimensionMismatchException(d2FdXdY[i].Length, yLen);
				}
				if (d2FdXdZ[i].Length != yLen)
				{
					throw new DimensionMismatchException(d2FdXdZ[i].Length, yLen);
				}
				if (d2FdYdZ[i].Length != yLen)
				{
					throw new DimensionMismatchException(d2FdYdZ[i].Length, yLen);
				}
				if (d3FdXdYdZ[i].Length != yLen)
				{
					throw new DimensionMismatchException(d3FdXdYdZ[i].Length, yLen);
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ip1 = i + 1;
				int ip1 = i + 1;
				for (int j = 0; j < lastJ; j++)
				{
					if (f[i][j].Length != zLen)
					{
						throw new DimensionMismatchException(f[i][j].Length, zLen);
					}
					if (dFdX[i][j].Length != zLen)
					{
						throw new DimensionMismatchException(dFdX[i][j].Length, zLen);
					}
					if (dFdY[i][j].Length != zLen)
					{
						throw new DimensionMismatchException(dFdY[i][j].Length, zLen);
					}
					if (dFdZ[i][j].Length != zLen)
					{
						throw new DimensionMismatchException(dFdZ[i][j].Length, zLen);
					}
					if (d2FdXdY[i][j].Length != zLen)
					{
						throw new DimensionMismatchException(d2FdXdY[i][j].Length, zLen);
					}
					if (d2FdXdZ[i][j].Length != zLen)
					{
						throw new DimensionMismatchException(d2FdXdZ[i][j].Length, zLen);
					}
					if (d2FdYdZ[i][j].Length != zLen)
					{
						throw new DimensionMismatchException(d2FdYdZ[i][j].Length, zLen);
					}
					if (d3FdXdYdZ[i][j].Length != zLen)
					{
						throw new DimensionMismatchException(d3FdXdYdZ[i][j].Length, zLen);
					}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jp1 = j + 1;
					int jp1 = j + 1;
					for (int k = 0; k < lastK; k++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int kp1 = k + 1;
						int kp1 = k + 1;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] beta = new double[] { f[i][j][k], f[ip1][j][k], f[i][jp1][k], f[ip1][jp1][k], f[i][j][kp1], f[ip1][j][kp1], f[i][jp1][kp1], f[ip1][jp1][kp1], dFdX[i][j][k], dFdX[ip1][j][k], dFdX[i][jp1][k], dFdX[ip1][jp1][k], dFdX[i][j][kp1], dFdX[ip1][j][kp1], dFdX[i][jp1][kp1], dFdX[ip1][jp1][kp1], dFdY[i][j][k], dFdY[ip1][j][k], dFdY[i][jp1][k], dFdY[ip1][jp1][k], dFdY[i][j][kp1], dFdY[ip1][j][kp1], dFdY[i][jp1][kp1], dFdY[ip1][jp1][kp1], dFdZ[i][j][k], dFdZ[ip1][j][k], dFdZ[i][jp1][k], dFdZ[ip1][jp1][k], dFdZ[i][j][kp1], dFdZ[ip1][j][kp1], dFdZ[i][jp1][kp1], dFdZ[ip1][jp1][kp1], d2FdXdY[i][j][k], d2FdXdY[ip1][j][k], d2FdXdY[i][jp1][k], d2FdXdY[ip1][jp1][k], d2FdXdY[i][j][kp1], d2FdXdY[ip1][j][kp1], d2FdXdY[i][jp1][kp1], d2FdXdY[ip1][jp1][kp1], d2FdXdZ[i][j][k], d2FdXdZ[ip1][j][k], d2FdXdZ[i][jp1][k], d2FdXdZ[ip1][jp1][k], d2FdXdZ[i][j][kp1], d2FdXdZ[ip1][j][kp1], d2FdXdZ[i][jp1][kp1], d2FdXdZ[ip1][jp1][kp1], d2FdYdZ[i][j][k], d2FdYdZ[ip1][j][k], d2FdYdZ[i][jp1][k], d2FdYdZ[ip1][jp1][k], d2FdYdZ[i][j][kp1], d2FdYdZ[ip1][j][kp1], d2FdYdZ[i][jp1][kp1], d2FdYdZ[ip1][jp1][kp1], d3FdXdYdZ[i][j][k], d3FdXdYdZ[ip1][j][k], d3FdXdYdZ[i][jp1][k], d3FdXdYdZ[ip1][jp1][k], d3FdXdYdZ[i][j][kp1], d3FdXdYdZ[ip1][j][kp1], d3FdXdYdZ[i][jp1][kp1], d3FdXdYdZ[ip1][jp1][kp1]};
						double[] beta = new double[] {f[i][j][k], f[ip1][j][k], f[i][jp1][k], f[ip1][jp1][k], f[i][j][kp1], f[ip1][j][kp1], f[i][jp1][kp1], f[ip1][jp1][kp1], dFdX[i][j][k], dFdX[ip1][j][k], dFdX[i][jp1][k], dFdX[ip1][jp1][k], dFdX[i][j][kp1], dFdX[ip1][j][kp1], dFdX[i][jp1][kp1], dFdX[ip1][jp1][kp1], dFdY[i][j][k], dFdY[ip1][j][k], dFdY[i][jp1][k], dFdY[ip1][jp1][k], dFdY[i][j][kp1], dFdY[ip1][j][kp1], dFdY[i][jp1][kp1], dFdY[ip1][jp1][kp1], dFdZ[i][j][k], dFdZ[ip1][j][k], dFdZ[i][jp1][k], dFdZ[ip1][jp1][k], dFdZ[i][j][kp1], dFdZ[ip1][j][kp1], dFdZ[i][jp1][kp1], dFdZ[ip1][jp1][kp1], d2FdXdY[i][j][k], d2FdXdY[ip1][j][k], d2FdXdY[i][jp1][k], d2FdXdY[ip1][jp1][k], d2FdXdY[i][j][kp1], d2FdXdY[ip1][j][kp1], d2FdXdY[i][jp1][kp1], d2FdXdY[ip1][jp1][kp1], d2FdXdZ[i][j][k], d2FdXdZ[ip1][j][k], d2FdXdZ[i][jp1][k], d2FdXdZ[ip1][jp1][k], d2FdXdZ[i][j][kp1], d2FdXdZ[ip1][j][kp1], d2FdXdZ[i][jp1][kp1], d2FdXdZ[ip1][jp1][kp1], d2FdYdZ[i][j][k], d2FdYdZ[ip1][j][k], d2FdYdZ[i][jp1][k], d2FdYdZ[ip1][jp1][k], d2FdYdZ[i][j][kp1], d2FdYdZ[ip1][j][kp1], d2FdYdZ[i][jp1][kp1], d2FdYdZ[ip1][jp1][kp1], d3FdXdYdZ[i][j][k], d3FdXdYdZ[ip1][j][k], d3FdXdYdZ[i][jp1][k], d3FdXdYdZ[ip1][jp1][k], d3FdXdYdZ[i][j][kp1], d3FdXdYdZ[ip1][j][kp1], d3FdXdYdZ[i][jp1][kp1], d3FdXdYdZ[ip1][jp1][kp1]};

						splines[i][j][k] = new TricubicSplineFunction(computeSplineCoefficients(beta));
					}
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="OutOfRangeException"> if any of the variables is outside its interpolation range. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double value(double x, double y, double z) throws mathlib.exception.OutOfRangeException
		public virtual double value(double x, double y, double z)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i = searchIndex(x, xval);
			int i = searchIndex(x, xval);
			if (i == -1)
			{
				throw new OutOfRangeException(x, xval[0], xval[xval.Length - 1]);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int j = searchIndex(y, yval);
			int j = searchIndex(y, yval);
			if (j == -1)
			{
				throw new OutOfRangeException(y, yval[0], yval[yval.Length - 1]);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int k = searchIndex(z, zval);
			int k = searchIndex(z, zval);
			if (k == -1)
			{
				throw new OutOfRangeException(z, zval[0], zval[zval.Length - 1]);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xN = (x - xval[i]) / (xval[i + 1] - xval[i]);
			double xN = (x - xval[i]) / (xval[i + 1] - xval[i]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yN = (y - yval[j]) / (yval[j + 1] - yval[j]);
			double yN = (y - yval[j]) / (yval[j + 1] - yval[j]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double zN = (z - zval[k]) / (zval[k + 1] - zval[k]);
			double zN = (z - zval[k]) / (zval[k + 1] - zval[k]);

			return splines[i][j][k].value(xN, yN, zN);
		}

		/// <param name="c"> Coordinate. </param>
		/// <param name="val"> Coordinate samples. </param>
		/// <returns> the index in {@code val} corresponding to the interval containing {@code c}, or {@code -1}
		///   if {@code c} is out of the range defined by the end values of {@code val}. </returns>
		private int searchIndex(double c, double[] val)
		{
			if (c < val[0])
			{
				return -1;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int max = val.length;
			int max = val.Length;
			for (int i = 1; i < max; i++)
			{
				if (c <= val[i])
				{
					return i - 1;
				}
			}

			return -1;
		}

		/// <summary>
		/// Compute the spline coefficients from the list of function values and
		/// function partial derivatives values at the four corners of a grid
		/// element. They must be specified in the following order:
		/// <ul>
		///  <li>f(0,0,0)</li>
		///  <li>f(1,0,0)</li>
		///  <li>f(0,1,0)</li>
		///  <li>f(1,1,0)</li>
		///  <li>f(0,0,1)</li>
		///  <li>f(1,0,1)</li>
		///  <li>f(0,1,1)</li>
		///  <li>f(1,1,1)</li>
		/// 
		///  <li>f<sub>x</sub>(0,0,0)</li>
		///  <li>... <em>(same order as above)</em></li>
		///  <li>f<sub>x</sub>(1,1,1)</li>
		/// 
		///  <li>f<sub>y</sub>(0,0,0)</li>
		///  <li>... <em>(same order as above)</em></li>
		///  <li>f<sub>y</sub>(1,1,1)</li>
		/// 
		///  <li>f<sub>z</sub>(0,0,0)</li>
		///  <li>... <em>(same order as above)</em></li>
		///  <li>f<sub>z</sub>(1,1,1)</li>
		/// 
		///  <li>f<sub>xy</sub>(0,0,0)</li>
		///  <li>... <em>(same order as above)</em></li>
		///  <li>f<sub>xy</sub>(1,1,1)</li>
		/// 
		///  <li>f<sub>xz</sub>(0,0,0)</li>
		///  <li>... <em>(same order as above)</em></li>
		///  <li>f<sub>xz</sub>(1,1,1)</li>
		/// 
		///  <li>f<sub>yz</sub>(0,0,0)</li>
		///  <li>... <em>(same order as above)</em></li>
		///  <li>f<sub>yz</sub>(1,1,1)</li>
		/// 
		///  <li>f<sub>xyz</sub>(0,0,0)</li>
		///  <li>... <em>(same order as above)</em></li>
		///  <li>f<sub>xyz</sub>(1,1,1)</li>
		/// </ul>
		/// where the subscripts indicate the partial derivative with respect to
		/// the corresponding variable(s).
		/// </summary>
		/// <param name="beta"> List of function values and function partial derivatives values. </param>
		/// <returns> the spline coefficients. </returns>
		private double[] computeSplineCoefficients(double[] beta)
		{
			const int sz = 64;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] a = new double[sz];
			double[] a = new double[sz];

			for (int i = 0; i < sz; i++)
			{
				double result = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] row = AINV[i];
				double[] row = AINV[i];
				for (int j = 0; j < sz; j++)
				{
					result += row[j] * beta[j];
				}
				a[i] = result;
			}

			return a;
		}
	}

	/// <summary>
	/// 3D-spline function.
	/// 
	/// @version $Id: TricubicSplineInterpolatingFunction.java 1385314 2012-09-16 16:35:49Z tn $
	/// </summary>
	internal class TricubicSplineFunction : TrivariateFunction
	{
		/// <summary>
		/// Number of points. </summary>
		private const short N = 4;
		/// <summary>
		/// Coefficients </summary>
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: private readonly double[][][] a = new double[N][N][N];
		private readonly double[][][] a = RectangularArrays.ReturnRectangularDoubleArray(N, N, N);

		/// <param name="aV"> List of spline coefficients. </param>
		public TricubicSplineFunction(double[] aV)
		{
			for (int i = 0; i < N; i++)
			{
				for (int j = 0; j < N; j++)
				{
					for (int k = 0; k < N; k++)
					{
						a[i][j][k] = aV[i + N * (j + N * k)];
					}
				}
			}
		}

		/// <param name="x"> x-coordinate of the interpolation point. </param>
		/// <param name="y"> y-coordinate of the interpolation point. </param>
		/// <param name="z"> z-coordinate of the interpolation point. </param>
		/// <returns> the interpolated value. </returns>
		/// <exception cref="OutOfRangeException"> if {@code x}, {@code y} or
		/// {@code z} are not in the interval {@code [0, 1]}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double value(double x, double y, double z) throws mathlib.exception.OutOfRangeException
		public virtual double value(double x, double y, double z)
		{
			if (x < 0 || x > 1)
			{
				throw new OutOfRangeException(x, 0, 1);
			}
			if (y < 0 || y > 1)
			{
				throw new OutOfRangeException(y, 0, 1);
			}
			if (z < 0 || z > 1)
			{
				throw new OutOfRangeException(z, 0, 1);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x2 = x * x;
			double x2 = x * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x3 = x2 * x;
			double x3 = x2 * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pX = { 1, x, x2, x3 };
			double[] pX = new double[] {1, x, x2, x3};

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y2 = y * y;
			double y2 = y * y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y3 = y2 * y;
			double y3 = y2 * y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pY = { 1, y, y2, y3 };
			double[] pY = new double[] {1, y, y2, y3};

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z2 = z * z;
			double z2 = z * z;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z3 = z2 * z;
			double z3 = z2 * z;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pZ = { 1, z, z2, z3 };
			double[] pZ = new double[] {1, z, z2, z3};

			double result = 0;
			for (int i = 0; i < N; i++)
			{
				for (int j = 0; j < N; j++)
				{
					for (int k = 0; k < N; k++)
					{
						result += a[i][j][k] * pX[i] * pY[j] * pZ[k];
					}
				}
			}

			return result;
		}
	}

}
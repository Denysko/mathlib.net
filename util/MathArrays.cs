using System;
using System.Collections.Generic;

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

namespace mathlib.util
{


	using mathlib;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;
	using UniformIntegerDistribution = mathlib.distribution.UniformIntegerDistribution;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MathArithmeticException = mathlib.exception.MathArithmeticException;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using MathInternalError = mathlib.exception.MathInternalError;
	using NoDataException = mathlib.exception.NoDataException;
	using NonMonotonicSequenceException = mathlib.exception.NonMonotonicSequenceException;
	using NotPositiveException = mathlib.exception.NotPositiveException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;

	/// <summary>
	/// Arrays utilities.
	/// 
	/// @since 3.0
	/// @version $Id: MathArrays.java 1591835 2014-05-02 09:04:01Z tn $
	/// </summary>
	public class MathArrays
	{
		/// <summary>
		/// Factor used for splitting double numbers: n = 2^27 + 1 (i.e. {@value}). </summary>
		private const int SPLIT_FACTOR = 0x8000001;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private MathArrays()
		{
		}

		/// <summary>
		/// Real-valued function that operate on an array or a part of it.
		/// @since 3.1
		/// </summary>
		public interface Function
		{
			/// <summary>
			/// Operates on an entire array.
			/// </summary>
			/// <param name="array"> Array to operate on. </param>
			/// <returns> the result of the operation. </returns>
			double evaluate(double[] array);
			/// <param name="array"> Array to operate on. </param>
			/// <param name="startIndex"> Index of the first element to take into account. </param>
			/// <param name="numElements"> Number of elements to take into account. </param>
			/// <returns> the result of the operation. </returns>
			double evaluate(double[] array, int startIndex, int numElements);
		}

		/// <summary>
		/// Create a copy of an array scaled by a value.
		/// </summary>
		/// <param name="arr"> Array to scale. </param>
		/// <param name="val"> Scalar. </param>
		/// <returns> scaled copy of array with each entry multiplied by val.
		/// @since 3.2 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static double[] scale(double val, final double[] arr)
		public static double[] scale(double val, double[] arr)
		{
			double[] newArr = new double[arr.Length];
			for (int i = 0; i < arr.Length; i++)
			{
				newArr[i] = arr[i] * val;
			}
			return newArr;
		}

		/// <summary>
		/// <p>Multiply each element of an array by a value.</p>
		/// 
		/// <p>The array is modified in place (no copy is created).</p>
		/// </summary>
		/// <param name="arr"> Array to scale </param>
		/// <param name="val"> Scalar
		/// @since 3.2 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static void scaleInPlace(double val, final double[] arr)
		public static void scaleInPlace(double val, double[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] *= val;
			}
		}

		/// <summary>
		/// Creates an array whose contents will be the element-by-element
		/// addition of the arguments.
		/// </summary>
		/// <param name="a"> First term of the addition. </param>
		/// <param name="b"> Second term of the addition. </param>
		/// <returns> a new array {@code r} where {@code r[i] = a[i] + b[i]}. </returns>
		/// <exception cref="DimensionMismatchException"> if the array lengths differ.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double[] ebeAdd(double[] a, double[] b) throws org.apache.commons.math3.exception.DimensionMismatchException
		public static double[] ebeAdd(double[] a, double[] b)
		{
			if (a.Length != b.Length)
			{
				throw new DimensionMismatchException(a.Length, b.Length);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] result = a.clone();
			double[] result = a.clone();
			for (int i = 0; i < a.Length; i++)
			{
				result[i] += b[i];
			}
			return result;
		}
		/// <summary>
		/// Creates an array whose contents will be the element-by-element
		/// subtraction of the second argument from the first.
		/// </summary>
		/// <param name="a"> First term. </param>
		/// <param name="b"> Element to be subtracted. </param>
		/// <returns> a new array {@code r} where {@code r[i] = a[i] - b[i]}. </returns>
		/// <exception cref="DimensionMismatchException"> if the array lengths differ.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double[] ebeSubtract(double[] a, double[] b) throws org.apache.commons.math3.exception.DimensionMismatchException
		public static double[] ebeSubtract(double[] a, double[] b)
		{
			if (a.Length != b.Length)
			{
				throw new DimensionMismatchException(a.Length, b.Length);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] result = a.clone();
			double[] result = a.clone();
			for (int i = 0; i < a.Length; i++)
			{
				result[i] -= b[i];
			}
			return result;
		}
		/// <summary>
		/// Creates an array whose contents will be the element-by-element
		/// multiplication of the arguments.
		/// </summary>
		/// <param name="a"> First factor of the multiplication. </param>
		/// <param name="b"> Second factor of the multiplication. </param>
		/// <returns> a new array {@code r} where {@code r[i] = a[i] * b[i]}. </returns>
		/// <exception cref="DimensionMismatchException"> if the array lengths differ.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double[] ebeMultiply(double[] a, double[] b) throws org.apache.commons.math3.exception.DimensionMismatchException
		public static double[] ebeMultiply(double[] a, double[] b)
		{
			if (a.Length != b.Length)
			{
				throw new DimensionMismatchException(a.Length, b.Length);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] result = a.clone();
			double[] result = a.clone();
			for (int i = 0; i < a.Length; i++)
			{
				result[i] *= b[i];
			}
			return result;
		}
		/// <summary>
		/// Creates an array whose contents will be the element-by-element
		/// division of the first argument by the second.
		/// </summary>
		/// <param name="a"> Numerator of the division. </param>
		/// <param name="b"> Denominator of the division. </param>
		/// <returns> a new array {@code r} where {@code r[i] = a[i] / b[i]}. </returns>
		/// <exception cref="DimensionMismatchException"> if the array lengths differ.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double[] ebeDivide(double[] a, double[] b) throws org.apache.commons.math3.exception.DimensionMismatchException
		public static double[] ebeDivide(double[] a, double[] b)
		{
			if (a.Length != b.Length)
			{
				throw new DimensionMismatchException(a.Length, b.Length);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] result = a.clone();
			double[] result = a.clone();
			for (int i = 0; i < a.Length; i++)
			{
				result[i] /= b[i];
			}
			return result;
		}

		/// <summary>
		/// Calculates the L<sub>1</sub> (sum of abs) distance between two points.
		/// </summary>
		/// <param name="p1"> the first point </param>
		/// <param name="p2"> the second point </param>
		/// <returns> the L<sub>1</sub> distance between the two points </returns>
		public static double distance1(double[] p1, double[] p2)
		{
			double sum = 0;
			for (int i = 0; i < p1.Length; i++)
			{
				sum += FastMath.abs(p1[i] - p2[i]);
			}
			return sum;
		}

		/// <summary>
		/// Calculates the L<sub>1</sub> (sum of abs) distance between two points.
		/// </summary>
		/// <param name="p1"> the first point </param>
		/// <param name="p2"> the second point </param>
		/// <returns> the L<sub>1</sub> distance between the two points </returns>
		public static int distance1(int[] p1, int[] p2)
		{
		  int sum = 0;
		  for (int i = 0; i < p1.Length; i++)
		  {
			  sum += FastMath.abs(p1[i] - p2[i]);
		  }
		  return sum;
		}

		/// <summary>
		/// Calculates the L<sub>2</sub> (Euclidean) distance between two points.
		/// </summary>
		/// <param name="p1"> the first point </param>
		/// <param name="p2"> the second point </param>
		/// <returns> the L<sub>2</sub> distance between the two points </returns>
		public static double distance(double[] p1, double[] p2)
		{
			double sum = 0;
			for (int i = 0; i < p1.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dp = p1[i] - p2[i];
				double dp = p1[i] - p2[i];
				sum += dp * dp;
			}
			return FastMath.sqrt(sum);
		}

		/// <summary>
		/// Calculates the L<sub>2</sub> (Euclidean) distance between two points.
		/// </summary>
		/// <param name="p1"> the first point </param>
		/// <param name="p2"> the second point </param>
		/// <returns> the L<sub>2</sub> distance between the two points </returns>
		public static double distance(int[] p1, int[] p2)
		{
		  double sum = 0;
		  for (int i = 0; i < p1.Length; i++)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dp = p1[i] - p2[i];
			  double dp = p1[i] - p2[i];
			  sum += dp * dp;
		  }
		  return FastMath.sqrt(sum);
		}

		/// <summary>
		/// Calculates the L<sub>&infin;</sub> (max of abs) distance between two points.
		/// </summary>
		/// <param name="p1"> the first point </param>
		/// <param name="p2"> the second point </param>
		/// <returns> the L<sub>&infin;</sub> distance between the two points </returns>
		public static double distanceInf(double[] p1, double[] p2)
		{
			double max = 0;
			for (int i = 0; i < p1.Length; i++)
			{
				max = FastMath.max(max, FastMath.abs(p1[i] - p2[i]));
			}
			return max;
		}

		/// <summary>
		/// Calculates the L<sub>&infin;</sub> (max of abs) distance between two points.
		/// </summary>
		/// <param name="p1"> the first point </param>
		/// <param name="p2"> the second point </param>
		/// <returns> the L<sub>&infin;</sub> distance between the two points </returns>
		public static int distanceInf(int[] p1, int[] p2)
		{
			int max = 0;
			for (int i = 0; i < p1.Length; i++)
			{
				max = FastMath.max(max, FastMath.abs(p1[i] - p2[i]));
			}
			return max;
		}

		/// <summary>
		/// Specification of ordering direction.
		/// </summary>
		public enum OrderDirection
		{
			/// <summary>
			/// Constant for increasing direction. </summary>
			INCREASING,
			/// <summary>
			/// Constant for decreasing direction. </summary>
			DECREASING
		}

		/// <summary>
		/// Check that an array is monotonically increasing or decreasing.
		/// </summary>
		/// @param <T> the type of the elements in the specified array </param>
		/// <param name="val"> Values. </param>
		/// <param name="dir"> Ordering direction. </param>
		/// <param name="strict"> Whether the order should be strict. </param>
		/// <returns> {@code true} if sorted, {@code false} otherwise. </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public static <T extends Comparable<? base T>> boolean isMonotonic(T[] val, OrderDirection dir, boolean strict)
		public static bool isMonotonic<T>(T[] val, OrderDirection dir, bool strict) where T : Comparable<? base T>
		{
			T previous = val[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int max = val.length;
			int max = val.Length;
			for (int i = 1; i < max; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int comp;
				int comp;
				switch (dir)
				{
				case org.apache.commons.math3.util.MathArrays.OrderDirection.INCREASING:
					comp = previous.compareTo(val[i]);
					if (strict)
					{
						if (comp >= 0)
						{
							return false;
						}
					}
					else
					{
						if (comp > 0)
						{
							return false;
						}
					}
					break;
				case org.apache.commons.math3.util.MathArrays.OrderDirection.DECREASING:
					comp = val[i].compareTo(previous);
					if (strict)
					{
						if (comp >= 0)
						{
							return false;
						}
					}
					else
					{
						if (comp > 0)
						{
						   return false;
						}
					}
					break;
				default:
					// Should never happen.
					throw new MathInternalError();
				}

				previous = val[i];
			}
			return true;
		}

		/// <summary>
		/// Check that an array is monotonically increasing or decreasing.
		/// </summary>
		/// <param name="val"> Values. </param>
		/// <param name="dir"> Ordering direction. </param>
		/// <param name="strict"> Whether the order should be strict. </param>
		/// <returns> {@code true} if sorted, {@code false} otherwise. </returns>
		public static bool isMonotonic(double[] val, OrderDirection dir, bool strict)
		{
			return checkOrder(val, dir, strict, false);
		}

		/// <summary>
		/// Check that the given array is sorted.
		/// </summary>
		/// <param name="val"> Values. </param>
		/// <param name="dir"> Ordering direction. </param>
		/// <param name="strict"> Whether the order should be strict. </param>
		/// <param name="abort"> Whether to throw an exception if the check fails. </param>
		/// <returns> {@code true} if the array is sorted. </returns>
		/// <exception cref="NonMonotonicSequenceException"> if the array is not sorted
		/// and {@code abort} is {@code true}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean checkOrder(double[] val, OrderDirection dir, boolean strict, boolean abort) throws org.apache.commons.math3.exception.NonMonotonicSequenceException
		public static bool checkOrder(double[] val, OrderDirection dir, bool strict, bool abort)
		{
			double previous = val[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int max = val.length;
			int max = val.Length;

			int index;
			for (index = 1; index < max; index++)
			{
				switch (dir)
				{
				case org.apache.commons.math3.util.MathArrays.OrderDirection.INCREASING:
					if (strict)
					{
						if (val[index] <= previous)
						{
							goto ITEMBreak;
						}
					}
					else
					{
						if (val[index] < previous)
						{
							goto ITEMBreak;
						}
					}
					break;
				case org.apache.commons.math3.util.MathArrays.OrderDirection.DECREASING:
					if (strict)
					{
						if (val[index] >= previous)
						{
							goto ITEMBreak;
						}
					}
					else
					{
						if (val[index] > previous)
						{
							goto ITEMBreak;
						}
					}
					break;
				default:
					// Should never happen.
					throw new MathInternalError();
				}

				previous = val[index];
				ITEMContinue:;
			}
			ITEMBreak:

			if (index == max)
			{
				// Loop completed.
				return true;
			}

			// Loop early exit means wrong ordering.
			if (abort)
			{
				throw new NonMonotonicSequenceException(val[index], previous, index, dir, strict);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Check that the given array is sorted.
		/// </summary>
		/// <param name="val"> Values. </param>
		/// <param name="dir"> Ordering direction. </param>
		/// <param name="strict"> Whether the order should be strict. </param>
		/// <exception cref="NonMonotonicSequenceException"> if the array is not sorted.
		/// @since 2.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void checkOrder(double[] val, OrderDirection dir, boolean strict) throws org.apache.commons.math3.exception.NonMonotonicSequenceException
		public static void checkOrder(double[] val, OrderDirection dir, bool strict)
		{
			checkOrder(val, dir, strict, true);
		}

		/// <summary>
		/// Check that the given array is sorted in strictly increasing order.
		/// </summary>
		/// <param name="val"> Values. </param>
		/// <exception cref="NonMonotonicSequenceException"> if the array is not sorted.
		/// @since 2.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void checkOrder(double[] val) throws org.apache.commons.math3.exception.NonMonotonicSequenceException
		public static void checkOrder(double[] val)
		{
			checkOrder(val, OrderDirection.INCREASING, true);
		}

		/// <summary>
		/// Throws DimensionMismatchException if the input array is not rectangular.
		/// </summary>
		/// <param name="in"> array to be tested </param>
		/// <exception cref="NullArgumentException"> if input array is null </exception>
		/// <exception cref="DimensionMismatchException"> if input array is not rectangular
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void checkRectangular(final long[][] in) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void checkRectangular(long[][] @in)
		{
			MathUtils.checkNotNull(@in);
			for (int i = 1; i < @in.Length; i++)
			{
				if (@in[i].Length != @in[0].Length)
				{
					throw new DimensionMismatchException(LocalizedFormats.DIFFERENT_ROWS_LENGTHS, @in[i].Length, @in[0].Length);
				}
			}
		}

		/// <summary>
		/// Check that all entries of the input array are strictly positive.
		/// </summary>
		/// <param name="in"> Array to be tested </param>
		/// <exception cref="NotStrictlyPositiveException"> if any entries of the array are not
		/// strictly positive.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void checkPositive(final double[] in) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void checkPositive(double[] @in)
		{
			for (int i = 0; i < @in.Length; i++)
			{
				if (@in[i] <= 0)
				{
					throw new NotStrictlyPositiveException(@in[i]);
				}
			}
		}

		/// <summary>
		/// Check that all entries of the input array are >= 0.
		/// </summary>
		/// <param name="in"> Array to be tested </param>
		/// <exception cref="NotPositiveException"> if any array entries are less than 0.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void checkNonNegative(final long[] in) throws org.apache.commons.math3.exception.NotPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void checkNonNegative(long[] @in)
		{
			for (int i = 0; i < @in.Length; i++)
			{
				if (@in[i] < 0)
				{
					throw new NotPositiveException(@in[i]);
				}
			}
		}

		/// <summary>
		/// Check all entries of the input array are >= 0.
		/// </summary>
		/// <param name="in"> Array to be tested </param>
		/// <exception cref="NotPositiveException"> if any array entries are less than 0.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void checkNonNegative(final long[][] in) throws org.apache.commons.math3.exception.NotPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void checkNonNegative(long[][] @in)
		{
			for (int i = 0; i < @in.Length; i++)
			{
				for (int j = 0; j < @in[i].Length; j++)
				{
					if (@in[i][j] < 0)
					{
						throw new NotPositiveException(@in[i][j]);
					}
				}
			}
		}

		/// <summary>
		/// Returns the Cartesian norm (2-norm), handling both overflow and underflow.
		/// Translation of the minpack enorm subroutine.
		/// 
		/// The redistribution policy for MINPACK is available
		/// <a href="http://www.netlib.org/minpack/disclaimer">here</a>, for
		/// convenience, it is reproduced below.</p>
		/// 
		/// <table border="0" width="80%" cellpadding="10" align="center" bgcolor="#E0E0E0">
		/// <tr><td>
		///    Minpack Copyright Notice (1999) University of Chicago.
		///    All rights reserved
		/// </td></tr>
		/// <tr><td>
		/// Redistribution and use in source and binary forms, with or without
		/// modification, are permitted provided that the following conditions
		/// are met:
		/// <ol>
		///  <li>Redistributions of source code must retain the above copyright
		///      notice, this list of conditions and the following disclaimer.</li>
		/// <li>Redistributions in binary form must reproduce the above
		///     copyright notice, this list of conditions and the following
		///     disclaimer in the documentation and/or other materials provided
		///     with the distribution.</li>
		/// <li>The end-user documentation included with the redistribution, if any,
		///     must include the following acknowledgment:
		///     {@code This product includes software developed by the University of
		///           Chicago, as Operator of Argonne National Laboratory.}
		///     Alternately, this acknowledgment may appear in the software itself,
		///     if and wherever such third-party acknowledgments normally appear.</li>
		/// <li><strong>WARRANTY DISCLAIMER. THE SOFTWARE IS SUPPLIED "AS IS"
		///     WITHOUT WARRANTY OF ANY KIND. THE COPYRIGHT HOLDER, THE
		///     UNITED STATES, THE UNITED STATES DEPARTMENT OF ENERGY, AND
		///     THEIR EMPLOYEES: (1) DISCLAIM ANY WARRANTIES, EXPRESS OR
		///     IMPLIED, INCLUDING BUT NOT LIMITED TO ANY IMPLIED WARRANTIES
		///     OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, TITLE
		///     OR NON-INFRINGEMENT, (2) DO NOT ASSUME ANY LEGAL LIABILITY
		///     OR RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS, OR
		///     USEFULNESS OF THE SOFTWARE, (3) DO NOT REPRESENT THAT USE OF
		///     THE SOFTWARE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS, (4)
		///     DO NOT WARRANT THAT THE SOFTWARE WILL FUNCTION
		///     UNINTERRUPTED, THAT IT IS ERROR-FREE OR THAT ANY ERRORS WILL
		///     BE CORRECTED.</strong></li>
		/// <li><strong>LIMITATION OF LIABILITY. IN NO EVENT WILL THE COPYRIGHT
		///     HOLDER, THE UNITED STATES, THE UNITED STATES DEPARTMENT OF
		///     ENERGY, OR THEIR EMPLOYEES: BE LIABLE FOR ANY INDIRECT,
		///     INCIDENTAL, CONSEQUENTIAL, SPECIAL OR PUNITIVE DAMAGES OF
		///     ANY KIND OR NATURE, INCLUDING BUT NOT LIMITED TO LOSS OF
		///     PROFITS OR LOSS OF DATA, FOR ANY REASON WHATSOEVER, WHETHER
		///     SUCH LIABILITY IS ASSERTED ON THE BASIS OF CONTRACT, TORT
		///     (INCLUDING NEGLIGENCE OR STRICT LIABILITY), OR OTHERWISE,
		///     EVEN IF ANY OF SAID PARTIES HAS BEEN WARNED OF THE
		///     POSSIBILITY OF SUCH LOSS OR DAMAGES.</strong></li>
		/// <ol></td></tr>
		/// </table>
		/// </summary>
		/// <param name="v"> Vector of doubles. </param>
		/// <returns> the 2-norm of the vector.
		/// @since 2.2 </returns>
		public static double safeNorm(double[] v)
		{
			double rdwarf = 3.834e-20;
			double rgiant = 1.304e+19;
			double s1 = 0;
			double s2 = 0;
			double s3 = 0;
			double x1max = 0;
			double x3max = 0;
			double floatn = v.Length;
			double agiant = rgiant / floatn;
			for (int i = 0; i < v.Length; i++)
			{
				double xabs = FastMath.abs(v[i]);
				if (xabs < rdwarf || xabs > agiant)
				{
					if (xabs > rdwarf)
					{
						if (xabs > x1max)
						{
							double r = x1max / xabs;
							s1 = 1 + s1 * r * r;
							x1max = xabs;
						}
						else
						{
							double r = xabs / x1max;
							s1 += r * r;
						}
					}
					else
					{
						if (xabs > x3max)
						{
							double r = x3max / xabs;
							s3 = 1 + s3 * r * r;
							x3max = xabs;
						}
						else
						{
							if (xabs != 0)
							{
								double r = xabs / x3max;
								s3 += r * r;
							}
						}
					}
				}
				else
				{
					s2 += xabs * xabs;
				}
			}
			double norm;
			if (s1 != 0)
			{
				norm = x1max * Math.Sqrt(s1 + (s2 / x1max) / x1max);
			}
			else
			{
				if (s2 == 0)
				{
					norm = x3max * Math.Sqrt(s3);
				}
				else
				{
					if (s2 >= x3max)
					{
						norm = Math.Sqrt(s2 * (1 + (x3max / s2) * (x3max * s3)));
					}
					else
					{
						norm = Math.Sqrt(x3max * ((s2 / x3max) + (x3max * s3)));
					}
				}
			}
			return norm;
		}

		/// <summary>
		/// Sort an array in ascending order in place and perform the same reordering
		/// of entries on other arrays. For example, if
		/// {@code x = [3, 1, 2], y = [1, 2, 3]} and {@code z = [0, 5, 7]}, then
		/// {@code sortInPlace(x, y, z)} will update {@code x} to {@code [1, 2, 3]},
		/// {@code y} to {@code [2, 3, 1]} and {@code z} to {@code [5, 7, 0]}.
		/// </summary>
		/// <param name="x"> Array to be sorted and used as a pattern for permutation
		/// of the other arrays. </param>
		/// <param name="yList"> Set of arrays whose permutations of entries will follow
		/// those performed on {@code x}. </param>
		/// <exception cref="DimensionMismatchException"> if any {@code y} is not the same
		/// size as {@code x}. </exception>
		/// <exception cref="NullArgumentException"> if {@code x} or any {@code y} is null.
		/// @since 3.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void sortInPlace(double[] x, double[]... yList) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NullArgumentException
		public static void sortInPlace(double[] x, params double[][] yList)
		{
			sortInPlace(x, OrderDirection.INCREASING, yList);
		}

		/// <summary>
		/// Sort an array in place and perform the same reordering of entries on
		/// other arrays.  This method works the same as the other
		/// <seealso cref="#sortInPlace(double[], double[][]) sortInPlace"/> method, but
		/// allows the order of the sort to be provided in the {@code dir}
		/// parameter.
		/// </summary>
		/// <param name="x"> Array to be sorted and used as a pattern for permutation
		/// of the other arrays. </param>
		/// <param name="dir"> Order direction. </param>
		/// <param name="yList"> Set of arrays whose permutations of entries will follow
		/// those performed on {@code x}. </param>
		/// <exception cref="DimensionMismatchException"> if any {@code y} is not the same
		/// size as {@code x}. </exception>
		/// <exception cref="NullArgumentException"> if {@code x} or any {@code y} is null
		/// @since 3.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void sortInPlace(double[] x, final OrderDirection dir, double[]... yList) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void sortInPlace(double[] x, OrderDirection dir, params double[][] yList)
		{

			// Consistency checks.
			if (x == null)
			{
				throw new NullArgumentException();
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int yListLen = yList.length;
			int yListLen = yList.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = x.length;
			int len = x.Length;

			for (int j = 0; j < yListLen; j++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y = yList[j];
				double[] y = yList[j];
				if (y == null)
				{
					throw new NullArgumentException();
				}
				if (y.Length != len)
				{
					throw new DimensionMismatchException(y.Length, len);
				}
			}

			// Associate each abscissa "x[i]" with its index "i".
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Pair<Double, Integer>> list = new java.util.ArrayList<Pair<Double, Integer>>(len);
			IList<Pair<double?, int?>> list = new List<Pair<double?, int?>>(len);
			for (int i = 0; i < len; i++)
			{
				list.Add(new Pair<double?, int?>(x[i], i));
			}

			// Create comparators for increasing and decreasing orders.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Comparator<Pair<Double, Integer>> comp = dir == MathArrays.OrderDirection.INCREASING ? new java.util.Comparator<Pair<Double, Integer>>()
			IComparer<Pair<double?, int?>> comp = dir == MathArrays.OrderDirection.INCREASING ? new ComparatorAnonymousInnerClassHelper()
			: new ComparatorAnonymousInnerClassHelper2();

			// Sort.
			list.Sort(comp);

			// Modify the original array so that its elements are in
			// the prescribed order.
			// Retrieve indices of original locations.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] indices = new int[len];
			int[] indices = new int[len];
			for (int i = 0; i < len; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Pair<Double, Integer> e = list.get(i);
				Pair<double?, int?> e = list[i];
				x[i] = e.Key;
				indices[i] = e.Value;
			}

			// In each of the associated arrays, move the
			// elements to their new location.
			for (int j = 0; j < yListLen; j++)
			{
				// Input array will be modified in place.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yInPlace = yList[j];
				double[] yInPlace = yList[j];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yOrig = yInPlace.clone();
				double[] yOrig = yInPlace.clone();

				for (int i = 0; i < len; i++)
				{
					yInPlace[i] = yOrig[indices[i]];
				}
			}
		}

		private class ComparatorAnonymousInnerClassHelper : IComparer<Pair<double?, int?>>
		{
			public ComparatorAnonymousInnerClassHelper()
			{
			}

			public virtual int Compare(Pair<double?, int?> o1, Pair<double?, int?> o2)
			{
				return o1.Key.compareTo(o2.Key);
			}
		}

		private class ComparatorAnonymousInnerClassHelper2 : IComparer<Pair<double?, int?>>
		{
			public ComparatorAnonymousInnerClassHelper2()
			{
			}

			public virtual int Compare(Pair<double?, int?> o1, Pair<double?, int?> o2)
			{
				return o2.Key.compareTo(o1.Key);
			}
		}

		/// <summary>
		/// Creates a copy of the {@code source} array.
		/// </summary>
		/// <param name="source"> Array to be copied. </param>
		/// <returns> the copied array. </returns>
		 public static int[] copyOf(int[] source)
		 {
			 return copyOf(source, source.Length);
		 }

		/// <summary>
		/// Creates a copy of the {@code source} array.
		/// </summary>
		/// <param name="source"> Array to be copied. </param>
		/// <returns> the copied array. </returns>
		 public static double[] copyOf(double[] source)
		 {
			 return copyOf(source, source.Length);
		 }

		/// <summary>
		/// Creates a copy of the {@code source} array.
		/// </summary>
		/// <param name="source"> Array to be copied. </param>
		/// <param name="len"> Number of entries to copy. If smaller then the source
		/// length, the copy will be truncated, if larger it will padded with
		/// zeroes. </param>
		/// <returns> the copied array. </returns>
		public static int[] copyOf(int[] source, int len)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] output = new int[len];
			 int[] output = new int[len];
			 Array.Copy(source, 0, output, 0, FastMath.min(len, source.Length));
			 return output;
		}

		/// <summary>
		/// Creates a copy of the {@code source} array.
		/// </summary>
		/// <param name="source"> Array to be copied. </param>
		/// <param name="len"> Number of entries to copy. If smaller then the source
		/// length, the copy will be truncated, if larger it will padded with
		/// zeroes. </param>
		/// <returns> the copied array. </returns>
		public static double[] copyOf(double[] source, int len)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] output = new double[len];
			 double[] output = new double[len];
			 Array.Copy(source, 0, output, 0, FastMath.min(len, source.Length));
			 return output;
		}

		/// <summary>
		/// Compute a linear combination accurately.
		/// This method computes the sum of the products
		/// <code>a<sub>i</sub> b<sub>i</sub></code> to high accuracy.
		/// It does so by using specific multiplication and addition algorithms to
		/// preserve accuracy and reduce cancellation effects.
		/// <br/>
		/// It is based on the 2005 paper
		/// <a href="http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.2.1547">
		/// Accurate Sum and Dot Product</a> by Takeshi Ogita, Siegfried M. Rump,
		/// and Shin'ichi Oishi published in SIAM J. Sci. Comput.
		/// </summary>
		/// <param name="a"> Factors. </param>
		/// <param name="b"> Factors. </param>
		/// <returns> <code>&Sigma;<sub>i</sub> a<sub>i</sub> b<sub>i</sub></code>. </returns>
		/// <exception cref="DimensionMismatchException"> if arrays dimensions don't match </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double linearCombination(final double[] a, final double[] b) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double linearCombination(double[] a, double[] b)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = a.length;
			int len = a.Length;
			if (len != b.Length)
			{
				throw new DimensionMismatchException(len, b.Length);
			}

			if (len == 1)
			{
				// Revert to scalar multiplication.
				return a[0] * b[0];
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] prodHigh = new double[len];
			double[] prodHigh = new double[len];
			double prodLowSum = 0;

			for (int i = 0; i < len; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ai = a[i];
				double ai = a[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ca = SPLIT_FACTOR * ai;
				double ca = SPLIT_FACTOR * ai;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double aHigh = ca - (ca - ai);
				double aHigh = ca - (ca - ai);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double aLow = ai - aHigh;
				double aLow = ai - aHigh;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bi = b[i];
				double bi = b[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cb = SPLIT_FACTOR * bi;
				double cb = SPLIT_FACTOR * bi;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bHigh = cb - (cb - bi);
				double bHigh = cb - (cb - bi);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bLow = bi - bHigh;
				double bLow = bi - bHigh;
				prodHigh[i] = ai * bi;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prodLow = aLow * bLow - (((prodHigh[i] - aHigh * bHigh) - aLow * bHigh) - aHigh * bLow);
				double prodLow = aLow * bLow - (((prodHigh[i] - aHigh * bHigh) - aLow * bHigh) - aHigh * bLow);
				prodLowSum += prodLow;
			}


//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prodHighCur = prodHigh[0];
			double prodHighCur = prodHigh[0];
			double prodHighNext = prodHigh[1];
			double sHighPrev = prodHighCur + prodHighNext;
			double sPrime = sHighPrev - prodHighNext;
			double sLowSum = (prodHighNext - (sHighPrev - sPrime)) + (prodHighCur - sPrime);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lenMinusOne = len - 1;
			int lenMinusOne = len - 1;
			for (int i = 1; i < lenMinusOne; i++)
			{
				prodHighNext = prodHigh[i + 1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sHighCur = sHighPrev + prodHighNext;
				double sHighCur = sHighPrev + prodHighNext;
				sPrime = sHighCur - prodHighNext;
				sLowSum += (prodHighNext - (sHighCur - sPrime)) + (sHighPrev - sPrime);
				sHighPrev = sHighCur;
			}

			double result = sHighPrev + (prodLowSum + sLowSum);

			if (double.IsNaN(result))
			{
				// either we have split infinite numbers or some coefficients were NaNs,
				// just rely on the naive implementation and let IEEE754 handle this
				result = 0;
				for (int i = 0; i < len; ++i)
				{
					result += a[i] * b[i];
				}
			}

			return result;
		}

		/// <summary>
		/// Compute a linear combination accurately.
		/// <p>
		/// This method computes a<sub>1</sub>&times;b<sub>1</sub> +
		/// a<sub>2</sub>&times;b<sub>2</sub> to high accuracy. It does
		/// so by using specific multiplication and addition algorithms to
		/// preserve accuracy and reduce cancellation effects. It is based
		/// on the 2005 paper <a
		/// href="http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.2.1547">
		/// Accurate Sum and Dot Product</a> by Takeshi Ogita,
		/// Siegfried M. Rump, and Shin'ichi Oishi published in SIAM J. Sci. Comput.
		/// </p> </summary>
		/// <param name="a1"> first factor of the first term </param>
		/// <param name="b1"> second factor of the first term </param>
		/// <param name="a2"> first factor of the second term </param>
		/// <param name="b2"> second factor of the second term </param>
		/// <returns> a<sub>1</sub>&times;b<sub>1</sub> +
		/// a<sub>2</sub>&times;b<sub>2</sub> </returns>
		/// <seealso cref= #linearCombination(double, double, double, double, double, double) </seealso>
		/// <seealso cref= #linearCombination(double, double, double, double, double, double, double, double) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static double linearCombination(final double a1, final double b1, final double a2, final double b2)
		public static double linearCombination(double a1, double b1, double a2, double b2)
		{

			// the code below is split in many additions/subtractions that may
			// appear redundant. However, they should NOT be simplified, as they
			// use IEEE754 floating point arithmetic rounding properties.
			// as an example, the expression "ca1 - (ca1 - a1)" is NOT the same as "a1"
			// The variable naming conventions are that xyzHigh contains the most significant
			// bits of xyz and xyzLow contains its least significant bits. So theoretically
			// xyz is the sum xyzHigh + xyzLow, but in many cases below, this sum cannot
			// be represented in only one double precision number so we preserve two numbers
			// to hold it as long as we can, combining the high and low order bits together
			// only at the end, after cancellation may have occurred on high order bits

			// split a1 and b1 as two 26 bits numbers
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ca1 = SPLIT_FACTOR * a1;
			double ca1 = SPLIT_FACTOR * a1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a1High = ca1 - (ca1 - a1);
			double a1High = ca1 - (ca1 - a1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a1Low = a1 - a1High;
			double a1Low = a1 - a1High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cb1 = SPLIT_FACTOR * b1;
			double cb1 = SPLIT_FACTOR * b1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b1High = cb1 - (cb1 - b1);
			double b1High = cb1 - (cb1 - b1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b1Low = b1 - b1High;
			double b1Low = b1 - b1High;

			// accurate multiplication a1 * b1
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod1High = a1 * b1;
			double prod1High = a1 * b1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod1Low = a1Low * b1Low - (((prod1High - a1High * b1High) - a1Low * b1High) - a1High * b1Low);
			double prod1Low = a1Low * b1Low - (((prod1High - a1High * b1High) - a1Low * b1High) - a1High * b1Low);

			// split a2 and b2 as two 26 bits numbers
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ca2 = SPLIT_FACTOR * a2;
			double ca2 = SPLIT_FACTOR * a2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a2High = ca2 - (ca2 - a2);
			double a2High = ca2 - (ca2 - a2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a2Low = a2 - a2High;
			double a2Low = a2 - a2High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cb2 = SPLIT_FACTOR * b2;
			double cb2 = SPLIT_FACTOR * b2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b2High = cb2 - (cb2 - b2);
			double b2High = cb2 - (cb2 - b2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b2Low = b2 - b2High;
			double b2Low = b2 - b2High;

			// accurate multiplication a2 * b2
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod2High = a2 * b2;
			double prod2High = a2 * b2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod2Low = a2Low * b2Low - (((prod2High - a2High * b2High) - a2Low * b2High) - a2High * b2Low);
			double prod2Low = a2Low * b2Low - (((prod2High - a2High * b2High) - a2Low * b2High) - a2High * b2Low);

			// accurate addition a1 * b1 + a2 * b2
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s12High = prod1High + prod2High;
			double s12High = prod1High + prod2High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s12Prime = s12High - prod2High;
			double s12Prime = s12High - prod2High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s12Low = (prod2High - (s12High - s12Prime)) + (prod1High - s12Prime);
			double s12Low = (prod2High - (s12High - s12Prime)) + (prod1High - s12Prime);

			// final rounding, s12 may have suffered many cancellations, we try
			// to recover some bits from the extra words we have saved up to now
			double result = s12High + (prod1Low + prod2Low + s12Low);

			if (double.IsNaN(result))
			{
				// either we have split infinite numbers or some coefficients were NaNs,
				// just rely on the naive implementation and let IEEE754 handle this
				result = a1 * b1 + a2 * b2;
			}

			return result;
		}

		/// <summary>
		/// Compute a linear combination accurately.
		/// <p>
		/// This method computes a<sub>1</sub>&times;b<sub>1</sub> +
		/// a<sub>2</sub>&times;b<sub>2</sub> + a<sub>3</sub>&times;b<sub>3</sub>
		/// to high accuracy. It does so by using specific multiplication and
		/// addition algorithms to preserve accuracy and reduce cancellation effects.
		/// It is based on the 2005 paper <a
		/// href="http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.2.1547">
		/// Accurate Sum and Dot Product</a> by Takeshi Ogita,
		/// Siegfried M. Rump, and Shin'ichi Oishi published in SIAM J. Sci. Comput.
		/// </p> </summary>
		/// <param name="a1"> first factor of the first term </param>
		/// <param name="b1"> second factor of the first term </param>
		/// <param name="a2"> first factor of the second term </param>
		/// <param name="b2"> second factor of the second term </param>
		/// <param name="a3"> first factor of the third term </param>
		/// <param name="b3"> second factor of the third term </param>
		/// <returns> a<sub>1</sub>&times;b<sub>1</sub> +
		/// a<sub>2</sub>&times;b<sub>2</sub> + a<sub>3</sub>&times;b<sub>3</sub> </returns>
		/// <seealso cref= #linearCombination(double, double, double, double) </seealso>
		/// <seealso cref= #linearCombination(double, double, double, double, double, double, double, double) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static double linearCombination(final double a1, final double b1, final double a2, final double b2, final double a3, final double b3)
		public static double linearCombination(double a1, double b1, double a2, double b2, double a3, double b3)
		{

			// the code below is split in many additions/subtractions that may
			// appear redundant. However, they should NOT be simplified, as they
			// do use IEEE754 floating point arithmetic rounding properties.
			// as an example, the expression "ca1 - (ca1 - a1)" is NOT the same as "a1"
			// The variables naming conventions are that xyzHigh contains the most significant
			// bits of xyz and xyzLow contains its least significant bits. So theoretically
			// xyz is the sum xyzHigh + xyzLow, but in many cases below, this sum cannot
			// be represented in only one double precision number so we preserve two numbers
			// to hold it as long as we can, combining the high and low order bits together
			// only at the end, after cancellation may have occurred on high order bits

			// split a1 and b1 as two 26 bits numbers
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ca1 = SPLIT_FACTOR * a1;
			double ca1 = SPLIT_FACTOR * a1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a1High = ca1 - (ca1 - a1);
			double a1High = ca1 - (ca1 - a1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a1Low = a1 - a1High;
			double a1Low = a1 - a1High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cb1 = SPLIT_FACTOR * b1;
			double cb1 = SPLIT_FACTOR * b1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b1High = cb1 - (cb1 - b1);
			double b1High = cb1 - (cb1 - b1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b1Low = b1 - b1High;
			double b1Low = b1 - b1High;

			// accurate multiplication a1 * b1
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod1High = a1 * b1;
			double prod1High = a1 * b1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod1Low = a1Low * b1Low - (((prod1High - a1High * b1High) - a1Low * b1High) - a1High * b1Low);
			double prod1Low = a1Low * b1Low - (((prod1High - a1High * b1High) - a1Low * b1High) - a1High * b1Low);

			// split a2 and b2 as two 26 bits numbers
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ca2 = SPLIT_FACTOR * a2;
			double ca2 = SPLIT_FACTOR * a2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a2High = ca2 - (ca2 - a2);
			double a2High = ca2 - (ca2 - a2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a2Low = a2 - a2High;
			double a2Low = a2 - a2High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cb2 = SPLIT_FACTOR * b2;
			double cb2 = SPLIT_FACTOR * b2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b2High = cb2 - (cb2 - b2);
			double b2High = cb2 - (cb2 - b2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b2Low = b2 - b2High;
			double b2Low = b2 - b2High;

			// accurate multiplication a2 * b2
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod2High = a2 * b2;
			double prod2High = a2 * b2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod2Low = a2Low * b2Low - (((prod2High - a2High * b2High) - a2Low * b2High) - a2High * b2Low);
			double prod2Low = a2Low * b2Low - (((prod2High - a2High * b2High) - a2Low * b2High) - a2High * b2Low);

			// split a3 and b3 as two 26 bits numbers
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ca3 = SPLIT_FACTOR * a3;
			double ca3 = SPLIT_FACTOR * a3;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a3High = ca3 - (ca3 - a3);
			double a3High = ca3 - (ca3 - a3);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a3Low = a3 - a3High;
			double a3Low = a3 - a3High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cb3 = SPLIT_FACTOR * b3;
			double cb3 = SPLIT_FACTOR * b3;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b3High = cb3 - (cb3 - b3);
			double b3High = cb3 - (cb3 - b3);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b3Low = b3 - b3High;
			double b3Low = b3 - b3High;

			// accurate multiplication a3 * b3
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod3High = a3 * b3;
			double prod3High = a3 * b3;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod3Low = a3Low * b3Low - (((prod3High - a3High * b3High) - a3Low * b3High) - a3High * b3Low);
			double prod3Low = a3Low * b3Low - (((prod3High - a3High * b3High) - a3Low * b3High) - a3High * b3Low);

			// accurate addition a1 * b1 + a2 * b2
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s12High = prod1High + prod2High;
			double s12High = prod1High + prod2High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s12Prime = s12High - prod2High;
			double s12Prime = s12High - prod2High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s12Low = (prod2High - (s12High - s12Prime)) + (prod1High - s12Prime);
			double s12Low = (prod2High - (s12High - s12Prime)) + (prod1High - s12Prime);

			// accurate addition a1 * b1 + a2 * b2 + a3 * b3
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s123High = s12High + prod3High;
			double s123High = s12High + prod3High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s123Prime = s123High - prod3High;
			double s123Prime = s123High - prod3High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s123Low = (prod3High - (s123High - s123Prime)) + (s12High - s123Prime);
			double s123Low = (prod3High - (s123High - s123Prime)) + (s12High - s123Prime);

			// final rounding, s123 may have suffered many cancellations, we try
			// to recover some bits from the extra words we have saved up to now
			double result = s123High + (prod1Low + prod2Low + prod3Low + s12Low + s123Low);

			if (double.IsNaN(result))
			{
				// either we have split infinite numbers or some coefficients were NaNs,
				// just rely on the naive implementation and let IEEE754 handle this
				result = a1 * b1 + a2 * b2 + a3 * b3;
			}

			return result;
		}

		/// <summary>
		/// Compute a linear combination accurately.
		/// <p>
		/// This method computes a<sub>1</sub>&times;b<sub>1</sub> +
		/// a<sub>2</sub>&times;b<sub>2</sub> + a<sub>3</sub>&times;b<sub>3</sub> +
		/// a<sub>4</sub>&times;b<sub>4</sub>
		/// to high accuracy. It does so by using specific multiplication and
		/// addition algorithms to preserve accuracy and reduce cancellation effects.
		/// It is based on the 2005 paper <a
		/// href="http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.2.1547">
		/// Accurate Sum and Dot Product</a> by Takeshi Ogita,
		/// Siegfried M. Rump, and Shin'ichi Oishi published in SIAM J. Sci. Comput.
		/// </p> </summary>
		/// <param name="a1"> first factor of the first term </param>
		/// <param name="b1"> second factor of the first term </param>
		/// <param name="a2"> first factor of the second term </param>
		/// <param name="b2"> second factor of the second term </param>
		/// <param name="a3"> first factor of the third term </param>
		/// <param name="b3"> second factor of the third term </param>
		/// <param name="a4"> first factor of the third term </param>
		/// <param name="b4"> second factor of the third term </param>
		/// <returns> a<sub>1</sub>&times;b<sub>1</sub> +
		/// a<sub>2</sub>&times;b<sub>2</sub> + a<sub>3</sub>&times;b<sub>3</sub> +
		/// a<sub>4</sub>&times;b<sub>4</sub> </returns>
		/// <seealso cref= #linearCombination(double, double, double, double) </seealso>
		/// <seealso cref= #linearCombination(double, double, double, double, double, double) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static double linearCombination(final double a1, final double b1, final double a2, final double b2, final double a3, final double b3, final double a4, final double b4)
		public static double linearCombination(double a1, double b1, double a2, double b2, double a3, double b3, double a4, double b4)
		{

			// the code below is split in many additions/subtractions that may
			// appear redundant. However, they should NOT be simplified, as they
			// do use IEEE754 floating point arithmetic rounding properties.
			// as an example, the expression "ca1 - (ca1 - a1)" is NOT the same as "a1"
			// The variables naming conventions are that xyzHigh contains the most significant
			// bits of xyz and xyzLow contains its least significant bits. So theoretically
			// xyz is the sum xyzHigh + xyzLow, but in many cases below, this sum cannot
			// be represented in only one double precision number so we preserve two numbers
			// to hold it as long as we can, combining the high and low order bits together
			// only at the end, after cancellation may have occurred on high order bits

			// split a1 and b1 as two 26 bits numbers
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ca1 = SPLIT_FACTOR * a1;
			double ca1 = SPLIT_FACTOR * a1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a1High = ca1 - (ca1 - a1);
			double a1High = ca1 - (ca1 - a1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a1Low = a1 - a1High;
			double a1Low = a1 - a1High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cb1 = SPLIT_FACTOR * b1;
			double cb1 = SPLIT_FACTOR * b1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b1High = cb1 - (cb1 - b1);
			double b1High = cb1 - (cb1 - b1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b1Low = b1 - b1High;
			double b1Low = b1 - b1High;

			// accurate multiplication a1 * b1
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod1High = a1 * b1;
			double prod1High = a1 * b1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod1Low = a1Low * b1Low - (((prod1High - a1High * b1High) - a1Low * b1High) - a1High * b1Low);
			double prod1Low = a1Low * b1Low - (((prod1High - a1High * b1High) - a1Low * b1High) - a1High * b1Low);

			// split a2 and b2 as two 26 bits numbers
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ca2 = SPLIT_FACTOR * a2;
			double ca2 = SPLIT_FACTOR * a2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a2High = ca2 - (ca2 - a2);
			double a2High = ca2 - (ca2 - a2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a2Low = a2 - a2High;
			double a2Low = a2 - a2High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cb2 = SPLIT_FACTOR * b2;
			double cb2 = SPLIT_FACTOR * b2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b2High = cb2 - (cb2 - b2);
			double b2High = cb2 - (cb2 - b2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b2Low = b2 - b2High;
			double b2Low = b2 - b2High;

			// accurate multiplication a2 * b2
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod2High = a2 * b2;
			double prod2High = a2 * b2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod2Low = a2Low * b2Low - (((prod2High - a2High * b2High) - a2Low * b2High) - a2High * b2Low);
			double prod2Low = a2Low * b2Low - (((prod2High - a2High * b2High) - a2Low * b2High) - a2High * b2Low);

			// split a3 and b3 as two 26 bits numbers
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ca3 = SPLIT_FACTOR * a3;
			double ca3 = SPLIT_FACTOR * a3;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a3High = ca3 - (ca3 - a3);
			double a3High = ca3 - (ca3 - a3);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a3Low = a3 - a3High;
			double a3Low = a3 - a3High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cb3 = SPLIT_FACTOR * b3;
			double cb3 = SPLIT_FACTOR * b3;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b3High = cb3 - (cb3 - b3);
			double b3High = cb3 - (cb3 - b3);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b3Low = b3 - b3High;
			double b3Low = b3 - b3High;

			// accurate multiplication a3 * b3
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod3High = a3 * b3;
			double prod3High = a3 * b3;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod3Low = a3Low * b3Low - (((prod3High - a3High * b3High) - a3Low * b3High) - a3High * b3Low);
			double prod3Low = a3Low * b3Low - (((prod3High - a3High * b3High) - a3Low * b3High) - a3High * b3Low);

			// split a4 and b4 as two 26 bits numbers
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ca4 = SPLIT_FACTOR * a4;
			double ca4 = SPLIT_FACTOR * a4;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a4High = ca4 - (ca4 - a4);
			double a4High = ca4 - (ca4 - a4);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a4Low = a4 - a4High;
			double a4Low = a4 - a4High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cb4 = SPLIT_FACTOR * b4;
			double cb4 = SPLIT_FACTOR * b4;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b4High = cb4 - (cb4 - b4);
			double b4High = cb4 - (cb4 - b4);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b4Low = b4 - b4High;
			double b4Low = b4 - b4High;

			// accurate multiplication a4 * b4
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod4High = a4 * b4;
			double prod4High = a4 * b4;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double prod4Low = a4Low * b4Low - (((prod4High - a4High * b4High) - a4Low * b4High) - a4High * b4Low);
			double prod4Low = a4Low * b4Low - (((prod4High - a4High * b4High) - a4Low * b4High) - a4High * b4Low);

			// accurate addition a1 * b1 + a2 * b2
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s12High = prod1High + prod2High;
			double s12High = prod1High + prod2High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s12Prime = s12High - prod2High;
			double s12Prime = s12High - prod2High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s12Low = (prod2High - (s12High - s12Prime)) + (prod1High - s12Prime);
			double s12Low = (prod2High - (s12High - s12Prime)) + (prod1High - s12Prime);

			// accurate addition a1 * b1 + a2 * b2 + a3 * b3
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s123High = s12High + prod3High;
			double s123High = s12High + prod3High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s123Prime = s123High - prod3High;
			double s123Prime = s123High - prod3High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s123Low = (prod3High - (s123High - s123Prime)) + (s12High - s123Prime);
			double s123Low = (prod3High - (s123High - s123Prime)) + (s12High - s123Prime);

			// accurate addition a1 * b1 + a2 * b2 + a3 * b3 + a4 * b4
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s1234High = s123High + prod4High;
			double s1234High = s123High + prod4High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s1234Prime = s1234High - prod4High;
			double s1234Prime = s1234High - prod4High;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s1234Low = (prod4High - (s1234High - s1234Prime)) + (s123High - s1234Prime);
			double s1234Low = (prod4High - (s1234High - s1234Prime)) + (s123High - s1234Prime);

			// final rounding, s1234 may have suffered many cancellations, we try
			// to recover some bits from the extra words we have saved up to now
			double result = s1234High + (prod1Low + prod2Low + prod3Low + prod4Low + s12Low + s123Low + s1234Low);

			if (double.IsNaN(result))
			{
				// either we have split infinite numbers or some coefficients were NaNs,
				// just rely on the naive implementation and let IEEE754 handle this
				result = a1 * b1 + a2 * b2 + a3 * b3 + a4 * b4;
			}

			return result;
		}

		/// <summary>
		/// Returns true iff both arguments are null or have same dimensions and all
		/// their elements are equal as defined by
		/// <seealso cref="Precision#equals(float,float)"/>.
		/// </summary>
		/// <param name="x"> first array </param>
		/// <param name="y"> second array </param>
		/// <returns> true if the values are both null or have same dimension
		/// and equal elements. </returns>
		public static bool Equals(float[] x, float[] y)
		{
			if ((x == null) || (y == null))
			{
				return !((x == null) ^ (y == null));
			}
			if (x.Length != y.Length)
			{
				return false;
			}
			for (int i = 0; i < x.Length; ++i)
			{
				if (!Precision.Equals(x[i], y[i]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns true iff both arguments are null or have same dimensions and all
		/// their elements are equal as defined by
		/// <seealso cref="Precision#equalsIncludingNaN(double,double) this method"/>.
		/// </summary>
		/// <param name="x"> first array </param>
		/// <param name="y"> second array </param>
		/// <returns> true if the values are both null or have same dimension and
		/// equal elements
		/// @since 2.2 </returns>
		public static bool equalsIncludingNaN(float[] x, float[] y)
		{
			if ((x == null) || (y == null))
			{
				return !((x == null) ^ (y == null));
			}
			if (x.Length != y.Length)
			{
				return false;
			}
			for (int i = 0; i < x.Length; ++i)
			{
				if (!Precision.equalsIncludingNaN(x[i], y[i]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns {@code true} iff both arguments are {@code null} or have same
		/// dimensions and all their elements are equal as defined by
		/// <seealso cref="Precision#equals(double,double)"/>.
		/// </summary>
		/// <param name="x"> First array. </param>
		/// <param name="y"> Second array. </param>
		/// <returns> {@code true} if the values are both {@code null} or have same
		/// dimension and equal elements. </returns>
		public static bool Equals(double[] x, double[] y)
		{
			if ((x == null) || (y == null))
			{
				return !((x == null) ^ (y == null));
			}
			if (x.Length != y.Length)
			{
				return false;
			}
			for (int i = 0; i < x.Length; ++i)
			{
				if (!Precision.Equals(x[i], y[i]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns {@code true} iff both arguments are {@code null} or have same
		/// dimensions and all their elements are equal as defined by
		/// <seealso cref="Precision#equalsIncludingNaN(double,double) this method"/>.
		/// </summary>
		/// <param name="x"> First array. </param>
		/// <param name="y"> Second array. </param>
		/// <returns> {@code true} if the values are both {@code null} or have same
		/// dimension and equal elements.
		/// @since 2.2 </returns>
		public static bool equalsIncludingNaN(double[] x, double[] y)
		{
			if ((x == null) || (y == null))
			{
				return !((x == null) ^ (y == null));
			}
			if (x.Length != y.Length)
			{
				return false;
			}
			for (int i = 0; i < x.Length; ++i)
			{
				if (!Precision.equalsIncludingNaN(x[i], y[i]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Normalizes an array to make it sum to a specified value.
		/// Returns the result of the transformation
		/// <pre>
		///    x |-> x * normalizedSum / sum
		/// </pre>
		/// applied to each non-NaN element x of the input array, where sum is the
		/// sum of the non-NaN entries in the input array.
		/// <p>
		/// Throws IllegalArgumentException if {@code normalizedSum} is infinite
		/// or NaN and ArithmeticException if the input array contains any infinite elements
		/// or sums to 0.
		/// <p>
		/// Ignores (i.e., copies unchanged to the output array) NaNs in the input array.
		/// </summary>
		/// <param name="values"> Input array to be normalized </param>
		/// <param name="normalizedSum"> Target sum for the normalized array </param>
		/// <returns> the normalized array. </returns>
		/// <exception cref="MathArithmeticException"> if the input array contains infinite
		/// elements or sums to zero. </exception>
		/// <exception cref="MathIllegalArgumentException"> if the target sum is infinite or {@code NaN}.
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double[] normalizeArray(double[] values, double normalizedSum) throws org.apache.commons.math3.exception.MathIllegalArgumentException, org.apache.commons.math3.exception.MathArithmeticException
		public static double[] normalizeArray(double[] values, double normalizedSum)
		{
			if (double.IsInfinity(normalizedSum))
			{
				throw new MathIllegalArgumentException(LocalizedFormats.NORMALIZE_INFINITE);
			}
			if (double.IsNaN(normalizedSum))
			{
				throw new MathIllegalArgumentException(LocalizedFormats.NORMALIZE_NAN);
			}
			double sum = 0d;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = values.length;
			int len = values.Length;
			double[] @out = new double[len];
			for (int i = 0; i < len; i++)
			{
				if (double.IsInfinity(values[i]))
				{
					throw new MathIllegalArgumentException(LocalizedFormats.INFINITE_ARRAY_ELEMENT, values[i], i);
				}
				if (!double.IsNaN(values[i]))
				{
					sum += values[i];
				}
			}
			if (sum == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.ARRAY_SUMS_TO_ZERO);
			}
			for (int i = 0; i < len; i++)
			{
				if (double.IsNaN(values[i]))
				{
					@out[i] = double.NaN;
				}
				else
				{
					@out[i] = values[i] * normalizedSum / sum;
				}
			}
			return @out;
		}

		/// <summary>
		/// Build an array of elements.
		/// <p>
		/// Arrays are filled with field.getZero()
		/// </summary>
		/// @param <T> the type of the field elements </param>
		/// <param name="field"> field to which array elements belong </param>
		/// <param name="length"> of the array </param>
		/// <returns> a new array
		/// @since 3.2 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T> T[] buildArray(final org.apache.commons.math3.Field<T> field, final int length)
		public static T[] buildArray<T>(Field<T> field, int length)
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") T[] array = (T[]) Array.newInstance(field.getRuntimeClass(), length);
			T[] array = (T[]) Array.newInstance(field.RuntimeClass, length); // OK because field must be correct class
			Arrays.fill(array, field.Zero);
			return array;
		}

		/// <summary>
		/// Build a double dimension  array of elements.
		/// <p>
		/// Arrays are filled with field.getZero()
		/// </summary>
		/// @param <T> the type of the field elements </param>
		/// <param name="field"> field to which array elements belong </param>
		/// <param name="rows"> number of rows in the array </param>
		/// <param name="columns"> number of columns (may be negative to build partial
		/// arrays in the same way <code>new Field[rows][]</code> works) </param>
		/// <returns> a new array
		/// @since 3.2 </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> T[][] buildArray(final org.apache.commons.math3.Field<T> field, final int rows, final int columns)
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static T[][] buildArray<T>(Field<T> field, int rows, int columns)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[][] array;
			T[][] array;
			if (columns < 0)
			{
				T[] dummyRow = buildArray(field, 0);
				array = (T[][]) Array.newInstance(dummyRow.GetType(), rows);
			}
			else
			{
				array = (T[][]) Array.newInstance(field.RuntimeClass, new int[] {rows, columns});
				for (int i = 0; i < rows; ++i)
				{
					Arrays.fill(array[i], field.Zero);
				}
			}
			return array;
		}

		/// <summary>
		/// Calculates the <a href="http://en.wikipedia.org/wiki/Convolution">
		/// convolution</a> between two sequences.
		/// <p>
		/// The solution is obtained via straightforward computation of the
		/// convolution sum (and not via FFT). Whenever the computation needs
		/// an element that would be located at an index outside the input arrays,
		/// the value is assumed to be zero.
		/// </summary>
		/// <param name="x"> First sequence.
		/// Typically, this sequence will represent an input signal to a system. </param>
		/// <param name="h"> Second sequence.
		/// Typically, this sequence will represent the impulse response of the system. </param>
		/// <returns> the convolution of {@code x} and {@code h}.
		/// This array's length will be {@code x.length + h.length - 1}. </returns>
		/// <exception cref="NullArgumentException"> if either {@code x} or {@code h} is {@code null}. </exception>
		/// <exception cref="NoDataException"> if either {@code x} or {@code h} is empty.
		/// 
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double[] convolve(double[] x, double[] h) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoDataException
		public static double[] convolve(double[] x, double[] h)
		{
			MathUtils.checkNotNull(x);
			MathUtils.checkNotNull(h);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int xLen = x.length;
			int xLen = x.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int hLen = h.length;
			int hLen = h.Length;

			if (xLen == 0 || hLen == 0)
			{
				throw new NoDataException();
			}

			// initialize the output array
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int totalLength = xLen + hLen - 1;
			int totalLength = xLen + hLen - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y = new double[totalLength];
			double[] y = new double[totalLength];

			// straightforward implementation of the convolution sum
			for (int n = 0; n < totalLength; n++)
			{
				double yn = 0;
				int k = FastMath.max(0, n + 1 - xLen);
				int j = n - k;
				while (k < hLen && j >= 0)
				{
					yn += x[j--] * h[k++];
				}
				y[n] = yn;
			}

			return y;
		}

		/// <summary>
		/// Specification for indicating that some operation applies
		/// before or after a given index.
		/// </summary>
		public enum Position
		{
			/// <summary>
			/// Designates the beginning of the array (near index 0). </summary>
			HEAD,
			/// <summary>
			/// Designates the end of the array. </summary>
			TAIL
		}

		/// <summary>
		/// Shuffle the entries of the given array.
		/// The {@code start} and {@code pos} parameters select which portion
		/// of the array is randomized and which is left untouched.
		/// </summary>
		/// <seealso cref= #shuffle(int[],int,Position,RandomGenerator)
		/// </seealso>
		/// <param name="list"> Array whose entries will be shuffled (in-place). </param>
		/// <param name="start"> Index at which shuffling begins. </param>
		/// <param name="pos"> Shuffling is performed for index positions between
		/// {@code start} and either the end (if <seealso cref="Position#TAIL"/>)
		/// or the beginning (if <seealso cref="Position#HEAD"/>) of the array. </param>
		public static void shuffle(int[] list, int start, Position pos)
		{
			shuffle(list, start, pos, new Well19937c());
		}

		/// <summary>
		/// Shuffle the entries of the given array, using the
		/// <a href="http://en.wikipedia.org/wiki/Fisher–Yates_shuffle#The_modern_algorithm">
		/// Fisher�"Yates</a> algorithm.
		/// The {@code start} and {@code pos} parameters select which portion
		/// of the array is randomized and which is left untouched.
		/// </summary>
		/// <param name="list"> Array whose entries will be shuffled (in-place). </param>
		/// <param name="start"> Index at which shuffling begins. </param>
		/// <param name="pos"> Shuffling is performed for index positions between
		/// {@code start} and either the end (if <seealso cref="Position#TAIL"/>)
		/// or the beginning (if <seealso cref="Position#HEAD"/>) of the array. </param>
		/// <param name="rng"> Random number generator. </param>
		public static void shuffle(int[] list, int start, Position pos, RandomGenerator rng)
		{
			switch (pos)
			{
			case org.apache.commons.math3.util.MathArrays.Position.TAIL:
			{
				for (int i = list.Length - 1; i >= start; i--)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int target;
					int target;
					if (i == start)
					{
						target = start;
					}
					else
					{
						// NumberIsTooLargeException cannot occur.
						target = (new UniformIntegerDistribution(rng, start, i)).sample();
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int temp = list[target];
					int temp = list[target];
					list[target] = list[i];
					list[i] = temp;
				}
			}
				break;
			case org.apache.commons.math3.util.MathArrays.Position.HEAD:
			{
				for (int i = 0; i <= start; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int target;
					int target;
					if (i == start)
					{
						target = start;
					}
					else
					{
						// NumberIsTooLargeException cannot occur.
						target = (new UniformIntegerDistribution(rng, i, start)).sample();
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int temp = list[target];
					int temp = list[target];
					list[target] = list[i];
					list[i] = temp;
				}
			}
				break;
			default:
				throw new MathInternalError(); // Should never happen.
			}
		}

		/// <summary>
		/// Shuffle the entries of the given array.
		/// </summary>
		/// <seealso cref= #shuffle(int[],int,Position,RandomGenerator)
		/// </seealso>
		/// <param name="list"> Array whose entries will be shuffled (in-place). </param>
		/// <param name="rng"> Random number generator. </param>
		public static void shuffle(int[] list, RandomGenerator rng)
		{
			shuffle(list, 0, Position.TAIL, rng);
		}

		/// <summary>
		/// Shuffle the entries of the given array.
		/// </summary>
		/// <seealso cref= #shuffle(int[],int,Position,RandomGenerator)
		/// </seealso>
		/// <param name="list"> Array whose entries will be shuffled (in-place). </param>
		public static void shuffle(int[] list)
		{
			shuffle(list, new Well19937c());
		}

		/// <summary>
		/// Returns an array representing the natural number {@code n}.
		/// </summary>
		/// <param name="n"> Natural number. </param>
		/// <returns> an array whose entries are the numbers 0, 1, ..., {@code n}-1.
		/// If {@code n == 0}, the returned array is empty. </returns>
		public static int[] natural(int n)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] a = new int[n];
			int[] a = new int[n];
			for (int i = 0; i < n; i++)
			{
				a[i] = i;
			}
			return a;
		}
		/// <summary>
		/// This method is used
		/// to verify that the input parameters designate a subarray of positive length.
		/// <p>
		/// <ul>
		/// <li>returns <code>true</code> iff the parameters designate a subarray of
		/// positive length</li>
		/// <li>throws <code>MathIllegalArgumentException</code> if the array is null or
		/// or the indices are invalid</li>
		/// <li>returns <code>false</li> if the array is non-null, but
		/// <code>length</code> is 0.
		/// </ul></p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> true if the parameters are valid and designate a subarray of positive length </returns>
		/// <exception cref="MathIllegalArgumentException"> if the indices are invalid or the array is null
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean verifyValues(final double[] values, final int begin, final int length) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static bool verifyValues(double[] values, int begin, int length)
		{
			return verifyValues(values, begin, length, false);
		}

		/// <summary>
		/// This method is used
		/// to verify that the input parameters designate a subarray of positive length.
		/// <p>
		/// <ul>
		/// <li>returns <code>true</code> iff the parameters designate a subarray of
		/// non-negative length</li>
		/// <li>throws <code>IllegalArgumentException</code> if the array is null or
		/// or the indices are invalid</li>
		/// <li>returns <code>false</li> if the array is non-null, but
		/// <code>length</code> is 0 unless <code>allowEmpty</code> is <code>true</code>
		/// </ul></p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <param name="allowEmpty"> if <code>true</code> then zero length arrays are allowed </param>
		/// <returns> true if the parameters are valid </returns>
		/// <exception cref="MathIllegalArgumentException"> if the indices are invalid or the array is null
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean verifyValues(final double[] values, final int begin, final int length, final boolean allowEmpty) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static bool verifyValues(double[] values, int begin, int length, bool allowEmpty)
		{

			if (values == null)
			{
				throw new NullArgumentException(LocalizedFormats.INPUT_ARRAY);
			}

			if (begin < 0)
			{
				throw new NotPositiveException(LocalizedFormats.START_POSITION, Convert.ToInt32(begin));
			}

			if (length < 0)
			{
				throw new NotPositiveException(LocalizedFormats.LENGTH, Convert.ToInt32(length));
			}

			if (begin + length > values.Length)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.SUBARRAY_ENDS_AFTER_ARRAY_END, Convert.ToInt32(begin + length), Convert.ToInt32(values.Length), true);
			}

			if (length == 0 && !allowEmpty)
			{
				return false;
			}

			return true;

		}

		/// <summary>
		/// This method is used
		/// to verify that the begin and length parameters designate a subarray of positive length
		/// and the weights are all non-negative, non-NaN, finite, and not all zero.
		/// <p>
		/// <ul>
		/// <li>returns <code>true</code> iff the parameters designate a subarray of
		/// positive length and the weights array contains legitimate values.</li>
		/// <li>throws <code>IllegalArgumentException</code> if any of the following are true:
		/// <ul><li>the values array is null</li>
		///     <li>the weights array is null</li>
		///     <li>the weights array does not have the same length as the values array</li>
		///     <li>the weights array contains one or more infinite values</li>
		///     <li>the weights array contains one or more NaN values</li>
		///     <li>the weights array contains negative values</li>
		///     <li>the start and length arguments do not determine a valid array</li></ul>
		/// </li>
		/// <li>returns <code>false</li> if the array is non-null, but
		/// <code>length</code> is 0.
		/// </ul></p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="weights"> the weights array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> true if the parameters are valid and designate a subarray of positive length </returns>
		/// <exception cref="MathIllegalArgumentException"> if the indices are invalid or the array is null
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean verifyValues(final double[] values, final double[] weights, final int begin, final int length) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static bool verifyValues(double[] values, double[] weights, int begin, int length)
		{
			return verifyValues(values, weights, begin, length, false);
		}

		/// <summary>
		/// This method is used
		/// to verify that the begin and length parameters designate a subarray of positive length
		/// and the weights are all non-negative, non-NaN, finite, and not all zero.
		/// <p>
		/// <ul>
		/// <li>returns <code>true</code> iff the parameters designate a subarray of
		/// non-negative length and the weights array contains legitimate values.</li>
		/// <li>throws <code>MathIllegalArgumentException</code> if any of the following are true:
		/// <ul><li>the values array is null</li>
		///     <li>the weights array is null</li>
		///     <li>the weights array does not have the same length as the values array</li>
		///     <li>the weights array contains one or more infinite values</li>
		///     <li>the weights array contains one or more NaN values</li>
		///     <li>the weights array contains negative values</li>
		///     <li>the start and length arguments do not determine a valid array</li></ul>
		/// </li>
		/// <li>returns <code>false</li> if the array is non-null, but
		/// <code>length</code> is 0 unless <code>allowEmpty</code> is <code>true</code>.
		/// </ul></p>
		/// </summary>
		/// <param name="values"> the input array. </param>
		/// <param name="weights"> the weights array. </param>
		/// <param name="begin"> index of the first array element to include. </param>
		/// <param name="length"> the number of elements to include. </param>
		/// <param name="allowEmpty"> if {@code true} than allow zero length arrays to pass. </param>
		/// <returns> {@code true} if the parameters are valid. </returns>
		/// <exception cref="NullArgumentException"> if either of the arrays are null </exception>
		/// <exception cref="MathIllegalArgumentException"> if the array indices are not valid,
		/// the weights array contains NaN, infinite or negative elements, or there
		/// are no positive weights.
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean verifyValues(final double[] values, final double[] weights, final int begin, final int length, final boolean allowEmpty) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static bool verifyValues(double[] values, double[] weights, int begin, int length, bool allowEmpty)
		{

			if (weights == null || values == null)
			{
				throw new NullArgumentException(LocalizedFormats.INPUT_ARRAY);
			}

			if (weights.Length != values.Length)
			{
				throw new DimensionMismatchException(weights.Length, values.Length);
			}

			bool containsPositiveWeight = false;
			for (int i = begin; i < begin + length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double weight = weights[i];
				double weight = weights[i];
				if (double.IsNaN(weight))
				{
					throw new MathIllegalArgumentException(LocalizedFormats.NAN_ELEMENT_AT_INDEX, Convert.ToInt32(i));
				}
				if (double.IsInfinity(weight))
				{
					throw new MathIllegalArgumentException(LocalizedFormats.INFINITE_ARRAY_ELEMENT, Convert.ToDouble(weight), Convert.ToInt32(i));
				}
				if (weight < 0)
				{
					throw new MathIllegalArgumentException(LocalizedFormats.NEGATIVE_ELEMENT_AT_INDEX, Convert.ToInt32(i), Convert.ToDouble(weight));
				}
				if (!containsPositiveWeight && weight > 0.0)
				{
					containsPositiveWeight = true;
				}
			}

			if (!containsPositiveWeight)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.WEIGHT_AT_LEAST_ONE_NON_ZERO);
			}

			return verifyValues(values, begin, length, allowEmpty);
		}
	}

}
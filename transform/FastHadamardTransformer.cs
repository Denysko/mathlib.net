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
namespace mathlib.transform
{

	using FunctionUtils = mathlib.analysis.FunctionUtils;
	using UnivariateFunction = mathlib.analysis.UnivariateFunction;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using ArithmeticUtils = mathlib.util.ArithmeticUtils;

	/// <summary>
	/// Implements the <a href="http://www.archive.chipcenter.com/dsp/DSP000517F1.html">Fast Hadamard Transform</a> (FHT).
	/// Transformation of an input vector x to the output vector y.
	/// <p>
	/// In addition to transformation of real vectors, the Hadamard transform can
	/// transform integer vectors into integer vectors. However, this integer transform
	/// cannot be inverted directly. Due to a scaling factor it may lead to rational results.
	/// As an example, the inverse transform of integer vector (0, 1, 0, 1) is rational
	/// vector (1/2, -1/2, 0, 0).
	/// 
	/// @version $Id: FastHadamardTransformer.java 1385310 2012-09-16 16:32:10Z tn $
	/// @since 2.0
	/// </summary>
	[Serializable]
	public class FastHadamardTransformer : RealTransformer
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		internal const long serialVersionUID = 20120211L;

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="MathIllegalArgumentException"> if the length of the data array is
		/// not a power of two </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double[] transform(final double[] f, final TransformType type)
		public virtual double[] transform(double[] f, TransformType type)
		{
			if (type == TransformType.FORWARD)
			{
				return fht(f);
			}
			return TransformUtils.scaleArray(fht(f), 1.0 / f.Length);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="mathlib.exception.NonMonotonicSequenceException">
		///   if the lower bound is greater than, or equal to the upper bound </exception>
		/// <exception cref="mathlib.exception.NotStrictlyPositiveException">
		///   if the number of sample points is negative </exception>
		/// <exception cref="MathIllegalArgumentException"> if the number of sample points is not a power of two </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double[] transform(final mathlib.analysis.UnivariateFunction f, final double min, final double max, final int n, final TransformType type)
		public virtual double[] transform(UnivariateFunction f, double min, double max, int n, TransformType type)
		{

			return transform(FunctionUtils.sample(f, min, max, n), type);
		}

		/// <summary>
		/// Returns the forward transform of the specified integer data set.The
		/// integer transform cannot be inverted directly, due to a scaling factor
		/// which may lead to double results.
		/// </summary>
		/// <param name="f"> the integer data array to be transformed (signal) </param>
		/// <returns> the integer transformed array (spectrum) </returns>
		/// <exception cref="MathIllegalArgumentException"> if the length of the data array is not a power of two </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public int[] transform(final int[] f)
		public virtual int[] transform(int[] f)
		{
			return fht(f);
		}

		/// <summary>
		/// The FHT (Fast Hadamard Transformation) which uses only subtraction and
		/// addition. Requires {@code N * log2(N) = n * 2^n} additions.
		/// 
		/// <h3>Short Table of manual calculation for N=8</h3>
		/// <ol>
		/// <li><b>x</b> is the input vector to be transformed,</li>
		/// <li><b>y</b> is the output vector (Fast Hadamard transform of <b>x</b>),</li>
		/// <li>a and b are helper rows.</li>
		/// </ol>
		/// <table align="center" border="1" cellpadding="3">
		/// <tbody align="center">
		/// <tr>
		///     <th>x</th>
		///     <th>a</th>
		///     <th>b</th>
		///     <th>y</th>
		/// </tr>
		/// <tr>
		///     <th>x<sub>0</sub></th>
		///     <td>a<sub>0</sub> = x<sub>0</sub> + x<sub>1</sub></td>
		///     <td>b<sub>0</sub> = a<sub>0</sub> + a<sub>1</sub></td>
		///     <td>y<sub>0</sub> = b<sub>0</sub >+ b<sub>1</sub></td>
		/// </tr>
		/// <tr>
		///     <th>x<sub>1</sub></th>
		///     <td>a<sub>1</sub> = x<sub>2</sub> + x<sub>3</sub></td>
		///     <td>b<sub>0</sub> = a<sub>2</sub> + a<sub>3</sub></td>
		///     <td>y<sub>0</sub> = b<sub>2</sub> + b<sub>3</sub></td>
		/// </tr>
		/// <tr>
		///     <th>x<sub>2</sub></th>
		///     <td>a<sub>2</sub> = x<sub>4</sub> + x<sub>5</sub></td>
		///     <td>b<sub>0</sub> = a<sub>4</sub> + a<sub>5</sub></td>
		///     <td>y<sub>0</sub> = b<sub>4</sub> + b<sub>5</sub></td>
		/// </tr>
		/// <tr>
		///     <th>x<sub>3</sub></th>
		///     <td>a<sub>3</sub> = x<sub>6</sub> + x<sub>7</sub></td>
		///     <td>b<sub>0</sub> = a<sub>6</sub> + a<sub>7</sub></td>
		///     <td>y<sub>0</sub> = b<sub>6</sub> + b<sub>7</sub></td>
		/// </tr>
		/// <tr>
		///     <th>x<sub>4</sub></th>
		///     <td>a<sub>0</sub> = x<sub>0</sub> - x<sub>1</sub></td>
		///     <td>b<sub>0</sub> = a<sub>0</sub> - a<sub>1</sub></td>
		///     <td>y<sub>0</sub> = b<sub>0</sub> - b<sub>1</sub></td>
		/// </tr>
		/// <tr>
		///     <th>x<sub>5</sub></th>
		///     <td>a<sub>1</sub> = x<sub>2</sub> - x<sub>3</sub></td>
		///     <td>b<sub>0</sub> = a<sub>2</sub> - a<sub>3</sub></td>
		///     <td>y<sub>0</sub> = b<sub>2</sub> - b<sub>3</sub></td>
		/// </tr>
		/// <tr>
		///     <th>x<sub>6</sub></th>
		///     <td>a<sub>2</sub> = x<sub>4</sub> - x<sub>5</sub></td>
		///     <td>b<sub>0</sub> = a<sub>4</sub> - a<sub>5</sub></td>
		///     <td>y<sub>0</sub> = b<sub>4</sub> - b<sub>5</sub></td>
		/// </tr>
		/// <tr>
		///     <th>x<sub>7</sub></th>
		///     <td>a<sub>3</sub> = x<sub>6</sub> - x<sub>7</sub></td>
		///     <td>b<sub>0</sub> = a<sub>6</sub> - a<sub>7</sub></td>
		///     <td>y<sub>0</sub> = b<sub>6</sub> - b<sub>7</sub></td>
		/// </tr>
		/// </tbody>
		/// </table>
		/// 
		/// <h3>How it works</h3>
		/// <ol>
		/// <li>Construct a matrix with {@code N} rows and {@code n + 1} columns,
		/// {@code hadm[n+1][N]}.<br/>
		/// <em>(If I use [x][y] it always means [row-offset][column-offset] of a
		/// Matrix with n rows and m columns. Its entries go from M[0][0]
		/// to M[n][N])</em></li>
		/// <li>Place the input vector {@code x[N]} in the first column of the
		/// matrix {@code hadm}.</li>
		/// <li>The entries of the submatrix {@code D_top} are calculated as follows
		///     <ul>
		///         <li>{@code D_top} goes from entry {@code [0][1]} to
		///         {@code [N / 2 - 1][n + 1]},</li>
		///         <li>the columns of {@code D_top} are the pairwise mutually
		///         exclusive sums of the previous column.</li>
		///     </ul>
		/// </li>
		/// <li>The entries of the submatrix {@code D_bottom} are calculated as
		/// follows
		///     <ul>
		///         <li>{@code D_bottom} goes from entry {@code [N / 2][1]} to
		///         {@code [N][n + 1]},</li>
		///         <li>the columns of {@code D_bottom} are the pairwise differences
		///         of the previous column.</li>
		///     </ul>
		/// </li>
		/// <li>The consputation of {@code D_top} and {@code D_bottom} are best
		/// understood with the above example (for {@code N = 8}).
		/// <li>The output vector {@code y} is now in the last column of
		/// {@code hadm}.</li>
		/// <li><em>Algorithm from <a href="http://www.archive.chipcenter.com/dsp/DSP000517F1.html">chipcenter</a>.</em></li>
		/// </ol>
		/// <h3>Visually</h3>
		/// <table border="1" align="center" cellpadding="3">
		/// <tbody align="center">
		/// <tr>
		///     <td></td><th>0</th><th>1</th><th>2</th><th>3</th>
		///     <th>&hellip;</th>
		///     <th>n + 1</th>
		/// </tr>
		/// <tr>
		///     <th>0</th>
		///     <td>x<sub>0</sub></td>
		///     <td colspan="5" rowspan="5" align="center" valign="middle">
		///         &uarr;<br/>
		///         &larr; D<sub>top</sub> &rarr;<br/>
		///         &darr;
		///     </td>
		/// </tr>
		/// <tr><th>1</th><td>x<sub>1</sub></td></tr>
		/// <tr><th>2</th><td>x<sub>2</sub></td></tr>
		/// <tr><th>&hellip;</th><td>&hellip;</td></tr>
		/// <tr><th>N / 2 - 1</th><td>x<sub>N/2-1</sub></td></tr>
		/// <tr>
		///     <th>N / 2</th>
		///     <td>x<sub>N/2</sub></td>
		///     <td colspan="5" rowspan="5" align="center" valign="middle">
		///         &uarr;<br/>
		///         &larr; D<sub>bottom</sub> &rarr;<br/>
		///         &darr;
		///     </td>
		/// </tr>
		/// <tr><th>N / 2 + 1</th><td>x<sub>N/2+1</sub></td></tr>
		/// <tr><th>N / 2 + 2</th><td>x<sub>N/2+2</sub></td></tr>
		/// <tr><th>&hellip;</th><td>&hellip;</td></tr>
		/// <tr><th>N</th><td>x<sub>N</sub></td></tr>
		/// </tbody>
		/// </table>
		/// </summary>
		/// <param name="x"> the real data array to be transformed </param>
		/// <returns> the real transformed array, {@code y} </returns>
		/// <exception cref="MathIllegalArgumentException"> if the length of the data array is not a power of two </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected double[] fht(double[] x) throws mathlib.exception.MathIllegalArgumentException
		protected internal virtual double[] fht(double[] x)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = x.length;
			int n = x.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int halfN = n / 2;
			int halfN = n / 2;

			if (!ArithmeticUtils.isPowerOfTwo(n))
			{
				throw new MathIllegalArgumentException(LocalizedFormats.NOT_POWER_OF_TWO, Convert.ToInt32(n));
			}

			/*
			 * Instead of creating a matrix with p+1 columns and n rows, we use two
			 * one dimension arrays which we are used in an alternating way.
			 */
			double[] yPrevious = new double[n];
			double[] yCurrent = x.clone();

			// iterate from left to right (column)
			for (int j = 1; j < n; j <<= 1)
			{

				// switch columns
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yTmp = yCurrent;
				double[] yTmp = yCurrent;
				yCurrent = yPrevious;
				yPrevious = yTmp;

				// iterate from top to bottom (row)
				for (int i = 0; i < halfN; ++i)
				{
					// Dtop: the top part works with addition
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int twoI = 2 * i;
					int twoI = 2 * i;
					yCurrent[i] = yPrevious[twoI] + yPrevious[twoI + 1];
				}
				for (int i = halfN; i < n; ++i)
				{
					// Dbottom: the bottom part works with subtraction
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int twoI = 2 * i;
					int twoI = 2 * i;
					yCurrent[i] = yPrevious[twoI - n] - yPrevious[twoI - n + 1];
				}
			}

			return yCurrent;

		}

		/// <summary>
		/// Returns the forward transform of the specified integer data set. The FHT
		/// (Fast Hadamard Transform) uses only subtraction and addition.
		/// </summary>
		/// <param name="x"> the integer data array to be transformed </param>
		/// <returns> the integer transformed array, {@code y} </returns>
		/// <exception cref="MathIllegalArgumentException"> if the length of the data array is not a power of two </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int[] fht(int[] x) throws mathlib.exception.MathIllegalArgumentException
		protected internal virtual int[] fht(int[] x)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = x.length;
			int n = x.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int halfN = n / 2;
			int halfN = n / 2;

			if (!ArithmeticUtils.isPowerOfTwo(n))
			{
				throw new MathIllegalArgumentException(LocalizedFormats.NOT_POWER_OF_TWO, Convert.ToInt32(n));
			}

			/*
			 * Instead of creating a matrix with p+1 columns and n rows, we use two
			 * one dimension arrays which we are used in an alternating way.
			 */
			int[] yPrevious = new int[n];
			int[] yCurrent = x.clone();

			// iterate from left to right (column)
			for (int j = 1; j < n; j <<= 1)
			{

				// switch columns
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] yTmp = yCurrent;
				int[] yTmp = yCurrent;
				yCurrent = yPrevious;
				yPrevious = yTmp;

				// iterate from top to bottom (row)
				for (int i = 0; i < halfN; ++i)
				{
					// Dtop: the top part works with addition
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int twoI = 2 * i;
					int twoI = 2 * i;
					yCurrent[i] = yPrevious[twoI] + yPrevious[twoI + 1];
				}
				for (int i = halfN; i < n; ++i)
				{
					// Dbottom: the bottom part works with subtraction
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int twoI = 2 * i;
					int twoI = 2 * i;
					yCurrent[i] = yPrevious[twoI - n] - yPrevious[twoI - n + 1];
				}
			}

			// return the last computed output vector y
			return yCurrent;

		}

	}

}
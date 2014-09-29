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
	using Complex = mathlib.complex.Complex;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using ArithmeticUtils = mathlib.util.ArithmeticUtils;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Implements the Fast Sine Transform for transformation of one-dimensional real
	/// data sets. For reference, see James S. Walker, <em>Fast Fourier
	/// Transforms</em>, chapter 3 (ISBN 0849371635).
	/// <p>
	/// There are several variants of the discrete sine transform. The present
	/// implementation corresponds to DST-I, with various normalization conventions,
	/// which are specified by the parameter <seealso cref="DstNormalization"/>.
	/// <strong>It should be noted that regardless to the convention, the first
	/// element of the dataset to be transformed must be zero.</strong>
	/// <p>
	/// DST-I is equivalent to DFT of an <em>odd extension</em> of the data series.
	/// More precisely, if x<sub>0</sub>, &hellip;, x<sub>N-1</sub> is the data set
	/// to be sine transformed, the extended data set x<sub>0</sub><sup>&#35;</sup>,
	/// &hellip;, x<sub>2N-1</sub><sup>&#35;</sup> is defined as follows
	/// <ul>
	/// <li>x<sub>0</sub><sup>&#35;</sup> = x<sub>0</sub> = 0,</li>
	/// <li>x<sub>k</sub><sup>&#35;</sup> = x<sub>k</sub> if 1 &le; k &lt; N,</li>
	/// <li>x<sub>N</sub><sup>&#35;</sup> = 0,</li>
	/// <li>x<sub>k</sub><sup>&#35;</sup> = -x<sub>2N-k</sub> if N + 1 &le; k &lt;
	/// 2N.</li>
	/// </ul>
	/// <p>
	/// Then, the standard DST-I y<sub>0</sub>, &hellip;, y<sub>N-1</sub> of the real
	/// data set x<sub>0</sub>, &hellip;, x<sub>N-1</sub> is equal to <em>half</em>
	/// of i (the pure imaginary number) times the N first elements of the DFT of the
	/// extended data set x<sub>0</sub><sup>&#35;</sup>, &hellip;,
	/// x<sub>2N-1</sub><sup>&#35;</sup> <br />
	/// y<sub>n</sub> = (i / 2) &sum;<sub>k=0</sub><sup>2N-1</sup>
	/// x<sub>k</sub><sup>&#35;</sup> exp[-2&pi;i nk / (2N)]
	/// &nbsp;&nbsp;&nbsp;&nbsp;k = 0, &hellip;, N-1.
	/// <p>
	/// The present implementation of the discrete sine transform as a fast sine
	/// transform requires the length of the data to be a power of two. Besides,
	/// it implicitly assumes that the sampled function is odd. In particular, the
	/// first element of the data set must be 0, which is enforced in
	/// <seealso cref="#transform(UnivariateFunction, double, double, int, TransformType)"/>,
	/// after sampling.
	/// 
	/// @version $Id: FastSineTransformer.java 1385310 2012-09-16 16:32:10Z tn $
	/// @since 1.2
	/// </summary>
	[Serializable]
	public class FastSineTransformer : RealTransformer
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		internal const long serialVersionUID = 20120211L;

		/// <summary>
		/// The type of DST to be performed. </summary>
		private readonly DstNormalization normalization;

		/// <summary>
		/// Creates a new instance of this class, with various normalization conventions.
		/// </summary>
		/// <param name="normalization"> the type of normalization to be applied to the transformed data </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FastSineTransformer(final DstNormalization normalization)
		public FastSineTransformer(DstNormalization normalization)
		{
			this.normalization = normalization;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The first element of the specified data set is required to be {@code 0}.
		/// </summary>
		/// <exception cref="MathIllegalArgumentException"> if the length of the data array is
		///   not a power of two, or the first element of the data array is not zero </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double[] transform(final double[] f, final TransformType type)
		public virtual double[] transform(double[] f, TransformType type)
		{
			if (normalization == DstNormalization.ORTHOGONAL_DST_I)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s = mathlib.util.FastMath.sqrt(2.0 / f.length);
				double s = FastMath.sqrt(2.0 / f.Length);
				return TransformUtils.scaleArray(fst(f), s);
			}
			if (type == TransformType.FORWARD)
			{
				return fst(f);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s = 2.0 / f.length;
			double s = 2.0 / f.Length;
			return TransformUtils.scaleArray(fst(f), s);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// This implementation enforces {@code f(x) = 0.0} at {@code x = 0.0}.
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

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] data = mathlib.analysis.FunctionUtils.sample(f, min, max, n);
			double[] data = FunctionUtils.sample(f, min, max, n);
			data[0] = 0.0;
			return transform(data, type);
		}

		/// <summary>
		/// Perform the FST algorithm (including inverse). The first element of the
		/// data set is required to be {@code 0}.
		/// </summary>
		/// <param name="f"> the real data array to be transformed </param>
		/// <returns> the real transformed array </returns>
		/// <exception cref="MathIllegalArgumentException"> if the length of the data array is
		///   not a power of two, or the first element of the data array is not zero </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected double[] fst(double[] f) throws mathlib.exception.MathIllegalArgumentException
		protected internal virtual double[] fst(double[] f)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] transformed = new double[f.length];
			double[] transformed = new double[f.Length];

			if (!ArithmeticUtils.isPowerOfTwo(f.Length))
			{
				throw new MathIllegalArgumentException(LocalizedFormats.NOT_POWER_OF_TWO_CONSIDER_PADDING, Convert.ToInt32(f.Length));
			}
			if (f[0] != 0.0)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.FIRST_ELEMENT_NOT_ZERO, Convert.ToDouble(f[0]));
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = f.length;
			int n = f.Length;
			if (n == 1) // trivial case
			{
				transformed[0] = 0.0;
				return transformed;
			}

			// construct a new array and perform FFT on it
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] x = new double[n];
			double[] x = new double[n];
			x[0] = 0.0;
			x[n >> 1] = 2.0 * f[n >> 1];
			for (int i = 1; i < (n >> 1); i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a = mathlib.util.FastMath.sin(i * mathlib.util.FastMath.PI / n) * (f[i] + f[n - i]);
				double a = FastMath.sin(i * FastMath.PI / n) * (f[i] + f[n - i]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b = 0.5 * (f[i] - f[n - i]);
				double b = 0.5 * (f[i] - f[n - i]);
				x[i] = a + b;
				x[n - i] = a - b;
			}
			FastFourierTransformer transformer;
			transformer = new FastFourierTransformer(DftNormalization.STANDARD);
			Complex[] y = transformer.transform(x, TransformType.FORWARD);

			// reconstruct the FST result for the original array
			transformed[0] = 0.0;
			transformed[1] = 0.5 * y[0].Real;
			for (int i = 1; i < (n >> 1); i++)
			{
				transformed[2 * i] = -y[i].Imaginary;
				transformed[2 * i + 1] = y[i].Real + transformed[2 * i - 1];
			}

			return transformed;
		}
	}

}
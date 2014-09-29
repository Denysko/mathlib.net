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
	/// Implements the Fast Cosine Transform for transformation of one-dimensional
	/// real data sets. For reference, see James S. Walker, <em>Fast Fourier
	/// Transforms</em>, chapter 3 (ISBN 0849371635).
	/// <p>
	/// There are several variants of the discrete cosine transform. The present
	/// implementation corresponds to DCT-I, with various normalization conventions,
	/// which are specified by the parameter <seealso cref="DctNormalization"/>.
	/// <p>
	/// DCT-I is equivalent to DFT of an <em>even extension</em> of the data series.
	/// More precisely, if x<sub>0</sub>, &hellip;, x<sub>N-1</sub> is the data set
	/// to be cosine transformed, the extended data set
	/// x<sub>0</sub><sup>&#35;</sup>, &hellip;, x<sub>2N-3</sub><sup>&#35;</sup>
	/// is defined as follows
	/// <ul>
	/// <li>x<sub>k</sub><sup>&#35;</sup> = x<sub>k</sub> if 0 &le; k &lt; N,</li>
	/// <li>x<sub>k</sub><sup>&#35;</sup> = x<sub>2N-2-k</sub>
	/// if N &le; k &lt; 2N - 2.</li>
	/// </ul>
	/// <p>
	/// Then, the standard DCT-I y<sub>0</sub>, &hellip;, y<sub>N-1</sub> of the real
	/// data set x<sub>0</sub>, &hellip;, x<sub>N-1</sub> is equal to <em>half</em>
	/// of the N first elements of the DFT of the extended data set
	/// x<sub>0</sub><sup>&#35;</sup>, &hellip;, x<sub>2N-3</sub><sup>&#35;</sup>
	/// <br/>
	/// y<sub>n</sub> = (1 / 2) &sum;<sub>k=0</sub><sup>2N-3</sup>
	/// x<sub>k</sub><sup>&#35;</sup> exp[-2&pi;i nk / (2N - 2)]
	/// &nbsp;&nbsp;&nbsp;&nbsp;k = 0, &hellip;, N-1.
	/// <p>
	/// The present implementation of the discrete cosine transform as a fast cosine
	/// transform requires the length of the data set to be a power of two plus one
	/// (N&nbsp;=&nbsp;2<sup>n</sup>&nbsp;+&nbsp;1). Besides, it implicitly assumes
	/// that the sampled function is even.
	/// 
	/// @version $Id: FastCosineTransformer.java 1385310 2012-09-16 16:32:10Z tn $
	/// @since 1.2
	/// </summary>
	[Serializable]
	public class FastCosineTransformer : RealTransformer
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		internal const long serialVersionUID = 20120212L;

		/// <summary>
		/// The type of DCT to be performed. </summary>
		private readonly DctNormalization normalization;

		/// <summary>
		/// Creates a new instance of this class, with various normalization
		/// conventions.
		/// </summary>
		/// <param name="normalization"> the type of normalization to be applied to the
		/// transformed data </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FastCosineTransformer(final DctNormalization normalization)
		public FastCosineTransformer(DctNormalization normalization)
		{
			this.normalization = normalization;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="MathIllegalArgumentException"> if the length of the data array is
		/// not a power of two plus one </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] transform(final double[] f, final TransformType type) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double[] transform(double[] f, TransformType type)
		{
			if (type == TransformType.FORWARD)
			{
				if (normalization == DctNormalization.ORTHOGONAL_DCT_I)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s = mathlib.util.FastMath.sqrt(2.0 / (f.length - 1));
					double s = FastMath.sqrt(2.0 / (f.Length - 1));
					return TransformUtils.scaleArray(fct(f), s);
				}
				return fct(f);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s2 = 2.0 / (f.length - 1);
			double s2 = 2.0 / (f.Length - 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s1;
			double s1;
			if (normalization == DctNormalization.ORTHOGONAL_DCT_I)
			{
				s1 = FastMath.sqrt(s2);
			}
			else
			{
				s1 = s2;
			}
			return TransformUtils.scaleArray(fct(f), s1);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="mathlib.exception.NonMonotonicSequenceException">
		/// if the lower bound is greater than, or equal to the upper bound </exception>
		/// <exception cref="mathlib.exception.NotStrictlyPositiveException">
		/// if the number of sample points is negative </exception>
		/// <exception cref="MathIllegalArgumentException"> if the number of sample points is
		/// not a power of two plus one </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] transform(final mathlib.analysis.UnivariateFunction f, final double min, final double max, final int n, final TransformType type) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double[] transform(UnivariateFunction f, double min, double max, int n, TransformType type)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] data = mathlib.analysis.FunctionUtils.sample(f, min, max, n);
			double[] data = FunctionUtils.sample(f, min, max, n);
			return transform(data, type);
		}

		/// <summary>
		/// Perform the FCT algorithm (including inverse).
		/// </summary>
		/// <param name="f"> the real data array to be transformed </param>
		/// <returns> the real transformed array </returns>
		/// <exception cref="MathIllegalArgumentException"> if the length of the data array is
		/// not a power of two plus one </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected double[] fct(double[] f) throws mathlib.exception.MathIllegalArgumentException
		protected internal virtual double[] fct(double[] f)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] transformed = new double[f.length];
			double[] transformed = new double[f.Length];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = f.length - 1;
			int n = f.Length - 1;
			if (!ArithmeticUtils.isPowerOfTwo(n))
			{
				throw new MathIllegalArgumentException(LocalizedFormats.NOT_POWER_OF_TWO_PLUS_ONE, Convert.ToInt32(f.Length));
			}
			if (n == 1) // trivial case
			{
				transformed[0] = 0.5 * (f[0] + f[1]);
				transformed[1] = 0.5 * (f[0] - f[1]);
				return transformed;
			}

			// construct a new array and perform FFT on it
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] x = new double[n];
			double[] x = new double[n];
			x[0] = 0.5 * (f[0] + f[n]);
			x[n >> 1] = f[n >> 1];
			// temporary variable for transformed[1]
			double t1 = 0.5 * (f[0] - f[n]);
			for (int i = 1; i < (n >> 1); i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a = 0.5 * (f[i] + f[n - i]);
				double a = 0.5 * (f[i] + f[n - i]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b = mathlib.util.FastMath.sin(i * mathlib.util.FastMath.PI / n) * (f[i] - f[n - i]);
				double b = FastMath.sin(i * FastMath.PI / n) * (f[i] - f[n - i]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c = mathlib.util.FastMath.cos(i * mathlib.util.FastMath.PI / n) * (f[i] - f[n - i]);
				double c = FastMath.cos(i * FastMath.PI / n) * (f[i] - f[n - i]);
				x[i] = a - b;
				x[n - i] = a + b;
				t1 += c;
			}
			FastFourierTransformer transformer;
			transformer = new FastFourierTransformer(DftNormalization.STANDARD);
			Complex[] y = transformer.transform(x, TransformType.FORWARD);

			// reconstruct the FCT result for the original array
			transformed[0] = y[0].Real;
			transformed[1] = t1;
			for (int i = 1; i < (n >> 1); i++)
			{
				transformed[2 * i] = y[i].Real;
				transformed[2 * i + 1] = transformed[2 * i - 1] - y[i].Imaginary;
			}
			transformed[n] = y[n >> 1].Real;

			return transformed;
		}
	}

}
using System;
using System.Diagnostics;

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
namespace org.apache.commons.math3.transform
{


	using FunctionUtils = org.apache.commons.math3.analysis.FunctionUtils;
	using UnivariateFunction = org.apache.commons.math3.analysis.UnivariateFunction;
	using Complex = org.apache.commons.math3.complex.Complex;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using MathIllegalStateException = org.apache.commons.math3.exception.MathIllegalStateException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using ArithmeticUtils = org.apache.commons.math3.util.ArithmeticUtils;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathArrays = org.apache.commons.math3.util.MathArrays;

	/// <summary>
	/// Implements the Fast Fourier Transform for transformation of one-dimensional
	/// real or complex data sets. For reference, see <em>Applied Numerical Linear
	/// Algebra</em>, ISBN 0898713897, chapter 6.
	/// <p>
	/// There are several variants of the discrete Fourier transform, with various
	/// normalization conventions, which are specified by the parameter
	/// <seealso cref="DftNormalization"/>.
	/// <p>
	/// The current implementation of the discrete Fourier transform as a fast
	/// Fourier transform requires the length of the data set to be a power of 2.
	/// This greatly simplifies and speeds up the code. Users can pad the data with
	/// zeros to meet this requirement. There are other flavors of FFT, for
	/// reference, see S. Winograd,
	/// <i>On computing the discrete Fourier transform</i>, Mathematics of
	/// Computation, 32 (1978), 175 - 199.
	/// </summary>
	/// <seealso cref= DftNormalization
	/// @version $Id: FastFourierTransformer.java 1385310 2012-09-16 16:32:10Z tn $
	/// @since 1.2 </seealso>
	[Serializable]
	public class FastFourierTransformer
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		internal const long serialVersionUID = 20120210L;

		/// <summary>
		/// {@code W_SUB_N_R[i]} is the real part of
		/// {@code exp(- 2 * i * pi / n)}:
		/// {@code W_SUB_N_R[i] = cos(2 * pi/ n)}, where {@code n = 2^i}.
		/// </summary>
		private static readonly double[] W_SUB_N_R = new double[] {0x1.0p0, -0x1.0p0, 0x1.1a62633145c07p - 54, 0x1.6a09e667f3bcdp - 1, 0x1.d906bcf328d46p - 1, 0x1.f6297cff75cbp - 1, 0x1.fd88da3d12526p - 1, 0x1.ff621e3796d7ep - 1, 0x1.ffd886084cd0dp - 1, 0x1.fff62169b92dbp - 1, 0x1.fffd8858e8a92p - 1, 0x1.ffff621621d02p - 1, 0x1.ffffd88586ee6p - 1, 0x1.fffff62161a34p - 1, 0x1.fffffd8858675p - 1, 0x1.ffffff621619cp - 1, 0x1.ffffffd885867p - 1, 0x1.fffffff62161ap - 1, 0x1.fffffffd88586p - 1, 0x1.ffffffff62162p - 1, 0x1.ffffffffd8858p - 1, 0x1.fffffffff6216p - 1, 0x1.fffffffffd886p - 1, 0x1.ffffffffff621p - 1, 0x1.ffffffffffd88p - 1, 0x1.fffffffffff62p - 1, 0x1.fffffffffffd9p - 1, 0x1.ffffffffffff6p - 1, 0x1.ffffffffffffep - 1, 0x1.fffffffffffffp - 1, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0, 0x1.0p0};

		/// <summary>
		/// {@code W_SUB_N_I[i]} is the imaginary part of
		/// {@code exp(- 2 * i * pi / n)}:
		/// {@code W_SUB_N_I[i] = -sin(2 * pi/ n)}, where {@code n = 2^i}.
		/// </summary>
		private static readonly double[] W_SUB_N_I = new double[] {0x1.1a62633145c07p - 52, -0x1.1a62633145c07p - 53, -0x1.0p0, -0x1.6a09e667f3bccp - 1, -0x1.87de2a6aea963p - 2, -0x1.8f8b83c69a60ap - 3, -0x1.917a6bc29b42cp - 4, -0x1.91f65f10dd814p - 5, -0x1.92155f7a3667ep - 6, -0x1.921d1fcdec784p - 7, -0x1.921f0fe670071p - 8, -0x1.921f8becca4bap - 9, -0x1.921faaee6472dp - 10, -0x1.921fb2aecb36p - 11, -0x1.921fb49ee4ea6p - 12, -0x1.921fb51aeb57bp - 13, -0x1.921fb539ecf31p - 14, -0x1.921fb541ad59ep - 15, -0x1.921fb5439d73ap - 16, -0x1.921fb544197ap - 17, -0x1.921fb544387bap - 18, -0x1.921fb544403c1p - 19, -0x1.921fb544422c2p - 20, -0x1.921fb54442a83p - 21, -0x1.921fb54442c73p - 22, -0x1.921fb54442cefp - 23, -0x1.921fb54442d0ep - 24, -0x1.921fb54442d15p - 25, -0x1.921fb54442d17p - 26, -0x1.921fb54442d18p - 27, -0x1.921fb54442d18p - 28, -0x1.921fb54442d18p - 29, -0x1.921fb54442d18p - 30, -0x1.921fb54442d18p - 31, -0x1.921fb54442d18p - 32, -0x1.921fb54442d18p - 33, -0x1.921fb54442d18p - 34, -0x1.921fb54442d18p - 35, -0x1.921fb54442d18p - 36, -0x1.921fb54442d18p - 37, -0x1.921fb54442d18p - 38, -0x1.921fb54442d18p - 39, -0x1.921fb54442d18p - 40, -0x1.921fb54442d18p - 41, -0x1.921fb54442d18p - 42, -0x1.921fb54442d18p - 43, -0x1.921fb54442d18p - 44, -0x1.921fb54442d18p - 45, -0x1.921fb54442d18p - 46, -0x1.921fb54442d18p - 47, -0x1.921fb54442d18p - 48, -0x1.921fb54442d18p - 49, -0x1.921fb54442d18p - 50, -0x1.921fb54442d18p - 51, -0x1.921fb54442d18p - 52, -0x1.921fb54442d18p - 53, -0x1.921fb54442d18p - 54, -0x1.921fb54442d18p - 55, -0x1.921fb54442d18p - 56, -0x1.921fb54442d18p - 57, -0x1.921fb54442d18p - 58, -0x1.921fb54442d18p - 59, -0x1.921fb54442d18p - 60};

		/// <summary>
		/// The type of DFT to be performed. </summary>
		private readonly DftNormalization normalization;

		/// <summary>
		/// Creates a new instance of this class, with various normalization
		/// conventions.
		/// </summary>
		/// <param name="normalization"> the type of normalization to be applied to the
		/// transformed data </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FastFourierTransformer(final DftNormalization normalization)
		public FastFourierTransformer(DftNormalization normalization)
		{
			this.normalization = normalization;
		}

		/// <summary>
		/// Performs identical index bit reversal shuffles on two arrays of identical
		/// size. Each element in the array is swapped with another element based on
		/// the bit-reversal of the index. For example, in an array with length 16,
		/// item at binary index 0011 (decimal 3) would be swapped with the item at
		/// binary index 1100 (decimal 12).
		/// </summary>
		/// <param name="a"> the first array to be shuffled </param>
		/// <param name="b"> the second array to be shuffled </param>
		private static void bitReversalShuffle2(double[] a, double[] b)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = a.length;
			int n = a.Length;
			Debug.Assert(b.Length == n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int halfOfN = n >> 1;
			int halfOfN = n >> 1;

			int j = 0;
			for (int i = 0; i < n; i++)
			{
				if (i < j)
				{
					// swap indices i & j
					double temp = a[i];
					a[i] = a[j];
					a[j] = temp;

					temp = b[i];
					b[i] = b[j];
					b[j] = temp;
				}

				int k = halfOfN;
				while (k <= j && k > 0)
				{
					j -= k;
					k >>= 1;
				}
				j += k;
			}
		}

		/// <summary>
		/// Applies the proper normalization to the specified transformed data.
		/// </summary>
		/// <param name="dataRI"> the unscaled transformed data </param>
		/// <param name="normalization"> the normalization to be applied </param>
		/// <param name="type"> the type of transform (forward, inverse) which resulted in the specified data </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void normalizeTransformedData(final double[][] dataRI, final DftNormalization normalization, final TransformType type)
		private static void normalizeTransformedData(double[][] dataRI, DftNormalization normalization, TransformType type)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dataR = dataRI[0];
			double[] dataR = dataRI[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dataI = dataRI[1];
			double[] dataI = dataRI[1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = dataR.length;
			int n = dataR.Length;
			Debug.Assert(dataI.Length == n);

			switch (normalization)
			{
				case org.apache.commons.math3.transform.DftNormalization.STANDARD:
					if (type == TransformType.INVERSE)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double scaleFactor = 1.0 / ((double) n);
						double scaleFactor = 1.0 / ((double) n);
						for (int i = 0; i < n; i++)
						{
							dataR[i] *= scaleFactor;
							dataI[i] *= scaleFactor;
						}
					}
					break;
				case org.apache.commons.math3.transform.DftNormalization.UNITARY:
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double scaleFactor = 1.0 / org.apache.commons.math3.util.FastMath.sqrt(n);
					double scaleFactor = 1.0 / FastMath.sqrt(n);
					for (int i = 0; i < n; i++)
					{
						dataR[i] *= scaleFactor;
						dataI[i] *= scaleFactor;
					}
					break;
				default:
					/*
					 * This should never occur in normal conditions. However this
					 * clause has been added as a safeguard if other types of
					 * normalizations are ever implemented, and the corresponding
					 * test is forgotten in the present switch.
					 */
					throw new MathIllegalStateException();
			}
		}

		/// <summary>
		/// Computes the standard transform of the specified complex data. The
		/// computation is done in place. The input data is laid out as follows
		/// <ul>
		///   <li>{@code dataRI[0][i]} is the real part of the {@code i}-th data point,</li>
		///   <li>{@code dataRI[1][i]} is the imaginary part of the {@code i}-th data point.</li>
		/// </ul>
		/// </summary>
		/// <param name="dataRI"> the two dimensional array of real and imaginary parts of the data </param>
		/// <param name="normalization"> the normalization to be applied to the transformed data </param>
		/// <param name="type"> the type of transform (forward, inverse) to be performed </param>
		/// <exception cref="DimensionMismatchException"> if the number of rows of the specified
		///   array is not two, or the array is not rectangular </exception>
		/// <exception cref="MathIllegalArgumentException"> if the number of data points is not
		///   a power of two </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static void transformInPlace(final double[][] dataRI, final DftNormalization normalization, final TransformType type)
		public static void transformInPlace(double[][] dataRI, DftNormalization normalization, TransformType type)
		{

			if (dataRI.Length != 2)
			{
				throw new DimensionMismatchException(dataRI.Length, 2);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dataR = dataRI[0];
			double[] dataR = dataRI[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dataI = dataRI[1];
			double[] dataI = dataRI[1];
			if (dataR.Length != dataI.Length)
			{
				throw new DimensionMismatchException(dataI.Length, dataR.Length);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = dataR.length;
			int n = dataR.Length;
			if (!ArithmeticUtils.isPowerOfTwo(n))
			{
				throw new MathIllegalArgumentException(LocalizedFormats.NOT_POWER_OF_TWO_CONSIDER_PADDING, Convert.ToInt32(n));
			}

			if (n == 1)
			{
				return;
			}
			else if (n == 2)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcR0 = dataR[0];
				double srcR0 = dataR[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcI0 = dataI[0];
				double srcI0 = dataI[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcR1 = dataR[1];
				double srcR1 = dataR[1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcI1 = dataI[1];
				double srcI1 = dataI[1];

				// X_0 = x_0 + x_1
				dataR[0] = srcR0 + srcR1;
				dataI[0] = srcI0 + srcI1;
				// X_1 = x_0 - x_1
				dataR[1] = srcR0 - srcR1;
				dataI[1] = srcI0 - srcI1;

				normalizeTransformedData(dataRI, normalization, type);
				return;
			}

			bitReversalShuffle2(dataR, dataI);

			// Do 4-term DFT.
			if (type == TransformType.INVERSE)
			{
				for (int i0 = 0; i0 < n; i0 += 4)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i1 = i0 + 1;
					int i1 = i0 + 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i2 = i0 + 2;
					int i2 = i0 + 2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i3 = i0 + 3;
					int i3 = i0 + 3;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcR0 = dataR[i0];
					double srcR0 = dataR[i0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcI0 = dataI[i0];
					double srcI0 = dataI[i0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcR1 = dataR[i2];
					double srcR1 = dataR[i2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcI1 = dataI[i2];
					double srcI1 = dataI[i2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcR2 = dataR[i1];
					double srcR2 = dataR[i1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcI2 = dataI[i1];
					double srcI2 = dataI[i1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcR3 = dataR[i3];
					double srcR3 = dataR[i3];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcI3 = dataI[i3];
					double srcI3 = dataI[i3];

					// 4-term DFT
					// X_0 = x_0 + x_1 + x_2 + x_3
					dataR[i0] = srcR0 + srcR1 + srcR2 + srcR3;
					dataI[i0] = srcI0 + srcI1 + srcI2 + srcI3;
					// X_1 = x_0 - x_2 + j * (x_3 - x_1)
					dataR[i1] = srcR0 - srcR2 + (srcI3 - srcI1);
					dataI[i1] = srcI0 - srcI2 + (srcR1 - srcR3);
					// X_2 = x_0 - x_1 + x_2 - x_3
					dataR[i2] = srcR0 - srcR1 + srcR2 - srcR3;
					dataI[i2] = srcI0 - srcI1 + srcI2 - srcI3;
					// X_3 = x_0 - x_2 + j * (x_1 - x_3)
					dataR[i3] = srcR0 - srcR2 + (srcI1 - srcI3);
					dataI[i3] = srcI0 - srcI2 + (srcR3 - srcR1);
				}
			}
			else
			{
				for (int i0 = 0; i0 < n; i0 += 4)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i1 = i0 + 1;
					int i1 = i0 + 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i2 = i0 + 2;
					int i2 = i0 + 2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i3 = i0 + 3;
					int i3 = i0 + 3;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcR0 = dataR[i0];
					double srcR0 = dataR[i0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcI0 = dataI[i0];
					double srcI0 = dataI[i0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcR1 = dataR[i2];
					double srcR1 = dataR[i2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcI1 = dataI[i2];
					double srcI1 = dataI[i2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcR2 = dataR[i1];
					double srcR2 = dataR[i1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcI2 = dataI[i1];
					double srcI2 = dataI[i1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcR3 = dataR[i3];
					double srcR3 = dataR[i3];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double srcI3 = dataI[i3];
					double srcI3 = dataI[i3];

					// 4-term DFT
					// X_0 = x_0 + x_1 + x_2 + x_3
					dataR[i0] = srcR0 + srcR1 + srcR2 + srcR3;
					dataI[i0] = srcI0 + srcI1 + srcI2 + srcI3;
					// X_1 = x_0 - x_2 + j * (x_3 - x_1)
					dataR[i1] = srcR0 - srcR2 + (srcI1 - srcI3);
					dataI[i1] = srcI0 - srcI2 + (srcR3 - srcR1);
					// X_2 = x_0 - x_1 + x_2 - x_3
					dataR[i2] = srcR0 - srcR1 + srcR2 - srcR3;
					dataI[i2] = srcI0 - srcI1 + srcI2 - srcI3;
					// X_3 = x_0 - x_2 + j * (x_1 - x_3)
					dataR[i3] = srcR0 - srcR2 + (srcI3 - srcI1);
					dataI[i3] = srcI0 - srcI2 + (srcR1 - srcR3);
				}
			}

			int lastN0 = 4;
			int lastLogN0 = 2;
			while (lastN0 < n)
			{
				int n0 = lastN0 << 1;
				int logN0 = lastLogN0 + 1;
				double wSubN0R = W_SUB_N_R[logN0];
				double wSubN0I = W_SUB_N_I[logN0];
				if (type == TransformType.INVERSE)
				{
					wSubN0I = -wSubN0I;
				}

				// Combine even/odd transforms of size lastN0 into a transform of size N0 (lastN0 * 2).
				for (int destEvenStartIndex = 0; destEvenStartIndex < n; destEvenStartIndex += n0)
				{
					int destOddStartIndex = destEvenStartIndex + lastN0;

					double wSubN0ToRR = 1;
					double wSubN0ToRI = 0;

					for (int r = 0; r < lastN0; r++)
					{
						double grR = dataR[destEvenStartIndex + r];
						double grI = dataI[destEvenStartIndex + r];
						double hrR = dataR[destOddStartIndex + r];
						double hrI = dataI[destOddStartIndex + r];

						// dest[destEvenStartIndex + r] = Gr + WsubN0ToR * Hr
						dataR[destEvenStartIndex + r] = grR + wSubN0ToRR * hrR - wSubN0ToRI * hrI;
						dataI[destEvenStartIndex + r] = grI + wSubN0ToRR * hrI + wSubN0ToRI * hrR;
						// dest[destOddStartIndex + r] = Gr - WsubN0ToR * Hr
						dataR[destOddStartIndex + r] = grR - (wSubN0ToRR * hrR - wSubN0ToRI * hrI);
						dataI[destOddStartIndex + r] = grI - (wSubN0ToRR * hrI + wSubN0ToRI * hrR);

						// WsubN0ToR *= WsubN0R
						double nextWsubN0ToRR = wSubN0ToRR * wSubN0R - wSubN0ToRI * wSubN0I;
						double nextWsubN0ToRI = wSubN0ToRR * wSubN0I + wSubN0ToRI * wSubN0R;
						wSubN0ToRR = nextWsubN0ToRR;
						wSubN0ToRI = nextWsubN0ToRI;
					}
				}

				lastN0 = n0;
				lastLogN0 = logN0;
			}

			normalizeTransformedData(dataRI, normalization, type);
		}

		/// <summary>
		/// Returns the (forward, inverse) transform of the specified real data set.
		/// </summary>
		/// <param name="f"> the real data array to be transformed </param>
		/// <param name="type"> the type of transform (forward, inverse) to be performed </param>
		/// <returns> the complex transformed array </returns>
		/// <exception cref="MathIllegalArgumentException"> if the length of the data array is not a power of two </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.complex.Complex[] transform(final double[] f, final TransformType type)
		public virtual Complex[] transform(double[] f, TransformType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] dataRI = new double[][] { org.apache.commons.math3.util.MathArrays.copyOf(f, f.length), new double[f.length] };
			double[][] dataRI = new double[][] {MathArrays.copyOf(f, f.Length), new double[f.Length]};

			transformInPlace(dataRI, normalization, type);

			return TransformUtils.createComplexArray(dataRI);
		}

		/// <summary>
		/// Returns the (forward, inverse) transform of the specified real function,
		/// sampled on the specified interval.
		/// </summary>
		/// <param name="f"> the function to be sampled and transformed </param>
		/// <param name="min"> the (inclusive) lower bound for the interval </param>
		/// <param name="max"> the (exclusive) upper bound for the interval </param>
		/// <param name="n"> the number of sample points </param>
		/// <param name="type"> the type of transform (forward, inverse) to be performed </param>
		/// <returns> the complex transformed array </returns>
		/// <exception cref="org.apache.commons.math3.exception.NumberIsTooLargeException">
		///   if the lower bound is greater than, or equal to the upper bound </exception>
		/// <exception cref="org.apache.commons.math3.exception.NotStrictlyPositiveException">
		///   if the number of sample points {@code n} is negative </exception>
		/// <exception cref="MathIllegalArgumentException"> if the number of sample points
		///   {@code n} is not a power of two </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.complex.Complex[] transform(final org.apache.commons.math3.analysis.UnivariateFunction f, final double min, final double max, final int n, final TransformType type)
		public virtual Complex[] transform(UnivariateFunction f, double min, double max, int n, TransformType type)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] data = org.apache.commons.math3.analysis.FunctionUtils.sample(f, min, max, n);
			double[] data = FunctionUtils.sample(f, min, max, n);
			return transform(data, type);
		}

		/// <summary>
		/// Returns the (forward, inverse) transform of the specified complex data set.
		/// </summary>
		/// <param name="f"> the complex data array to be transformed </param>
		/// <param name="type"> the type of transform (forward, inverse) to be performed </param>
		/// <returns> the complex transformed array </returns>
		/// <exception cref="MathIllegalArgumentException"> if the length of the data array is not a power of two </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.complex.Complex[] transform(final org.apache.commons.math3.complex.Complex[] f, final TransformType type)
		public virtual Complex[] transform(Complex[] f, TransformType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] dataRI = TransformUtils.createRealImaginaryArray(f);
			double[][] dataRI = TransformUtils.createRealImaginaryArray(f);

			transformInPlace(dataRI, normalization, type);

			return TransformUtils.createComplexArray(dataRI);
		}

		/// <summary>
		/// Performs a multi-dimensional Fourier transform on a given array. Use
		/// <seealso cref="#transform(Complex[], TransformType)"/> in a row-column
		/// implementation in any number of dimensions with
		/// O(N&times;log(N)) complexity with
		/// N = n<sub>1</sub> &times; n<sub>2</sub> &times;n<sub>3</sub> &times; ...
		/// &times; n<sub>d</sub>, where n<sub>k</sub> is the number of elements in
		/// dimension k, and d is the total number of dimensions.
		/// </summary>
		/// <param name="mdca"> Multi-Dimensional Complex Array, i.e. {@code Complex[][][][]} </param>
		/// <param name="type"> the type of transform (forward, inverse) to be performed </param>
		/// <returns> transform of {@code mdca} as a Multi-Dimensional Complex Array, i.e. {@code Complex[][][][]} </returns>
		/// <exception cref="IllegalArgumentException"> if any dimension is not a power of two </exception>
		/// @deprecated see MATH-736 
		[Obsolete("see MATH-736")]
		public virtual object mdfft(object mdca, TransformType type)
		{
			MultiDimensionalComplexMatrix mdcm = (MultiDimensionalComplexMatrix) (new MultiDimensionalComplexMatrix(mdca)).clone();
			int[] dimensionSize = mdcm.DimensionSizes;
			//cycle through each dimension
			for (int i = 0; i < dimensionSize.Length; i++)
			{
				mdfft(mdcm, type, i, new int[0]);
			}
			return mdcm.Array;
		}

		/// <summary>
		/// Performs one dimension of a multi-dimensional Fourier transform.
		/// </summary>
		/// <param name="mdcm"> input matrix </param>
		/// <param name="type"> the type of transform (forward, inverse) to be performed </param>
		/// <param name="d"> index of the dimension to process </param>
		/// <param name="subVector"> recursion subvector </param>
		/// <exception cref="IllegalArgumentException"> if any dimension is not a power of two </exception>
		/// @deprecated see MATH-736 
		[Obsolete("see MATH-736")]
		private void mdfft(MultiDimensionalComplexMatrix mdcm, TransformType type, int d, int[] subVector)
		{

			int[] dimensionSize = mdcm.DimensionSizes;
			//if done
			if (subVector.Length == dimensionSize.Length)
			{
				Complex[] temp = new Complex[dimensionSize[d]];
				for (int i = 0; i < dimensionSize[d]; i++)
				{
					//fft along dimension d
					subVector[d] = i;
					temp[i] = mdcm.get(subVector);
				}

				temp = transform(temp, type);

				for (int i = 0; i < dimensionSize[d]; i++)
				{
					subVector[d] = i;
					mdcm.set(temp[i], subVector);
				}
			}
			else
			{
				int[] vector = new int[subVector.Length + 1];
				Array.Copy(subVector, 0, vector, 0, subVector.Length);
				if (subVector.Length == d)
				{
					//value is not important once the recursion is done.
					//then an fft will be applied along the dimension d.
					vector[d] = 0;
					mdfft(mdcm, type, d, vector);
				}
				else
				{
					for (int i = 0; i < dimensionSize[subVector.Length]; i++)
					{
						vector[subVector.Length] = i;
						//further split along the next dimension
						mdfft(mdcm, type, d, vector);
					}
				}
			}
		}

		/// <summary>
		/// Complex matrix implementation. Not designed for synchronized access may
		/// eventually be replaced by jsr-83 of the java community process
		/// http://jcp.org/en/jsr/detail?id=83
		/// may require additional exception throws for other basic requirements.
		/// </summary>
		/// @deprecated see MATH-736 
		[Obsolete("see MATH-736")]
		private class MultiDimensionalComplexMatrix : ICloneable
		{

			/// <summary>
			/// Size in all dimensions. </summary>
			protected internal int[] dimensionSize;

			/// <summary>
			/// Storage array. </summary>
			protected internal object multiDimensionalComplexArray;

			/// <summary>
			/// Simple constructor.
			/// </summary>
			/// <param name="multiDimensionalComplexArray"> array containing the matrix
			/// elements </param>
			public MultiDimensionalComplexMatrix(object multiDimensionalComplexArray)
			{

				this.multiDimensionalComplexArray = multiDimensionalComplexArray;

				// count dimensions
				int numOfDimensions = 0;
				for (object lastDimension = multiDimensionalComplexArray; lastDimension is object[];)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] array = (Object[]) lastDimension;
					object[] array = (object[]) lastDimension;
					numOfDimensions++;
					lastDimension = array[0];
				}

				// allocate array with exact count
				dimensionSize = new int[numOfDimensions];

				// fill array
				numOfDimensions = 0;
				for (object lastDimension = multiDimensionalComplexArray; lastDimension is object[];)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] array = (Object[]) lastDimension;
					object[] array = (object[]) lastDimension;
					dimensionSize[numOfDimensions++] = array.Length;
					lastDimension = array[0];
				}

			}

			/// <summary>
			/// Get a matrix element.
			/// </summary>
			/// <param name="vector"> indices of the element </param>
			/// <returns> matrix element </returns>
			/// <exception cref="DimensionMismatchException"> if dimensions do not match </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.complex.Complex get(int... vector) throws org.apache.commons.math3.exception.DimensionMismatchException
			public virtual Complex get(params int[] vector)
			{

				if (vector == null)
				{
					if (dimensionSize.Length > 0)
					{
						throw new DimensionMismatchException(0, dimensionSize.Length);
					}
					return null;
				}
				if (vector.Length != dimensionSize.Length)
				{
					throw new DimensionMismatchException(vector.Length, dimensionSize.Length);
				}

				object lastDimension = multiDimensionalComplexArray;

				for (int i = 0; i < dimensionSize.Length; i++)
				{
					lastDimension = ((object[]) lastDimension)[vector[i]];
				}
				return (Complex) lastDimension;
			}

			/// <summary>
			/// Set a matrix element.
			/// </summary>
			/// <param name="magnitude"> magnitude of the element </param>
			/// <param name="vector"> indices of the element </param>
			/// <returns> the previous value </returns>
			/// <exception cref="DimensionMismatchException"> if dimensions do not match </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.complex.Complex set(org.apache.commons.math3.complex.Complex magnitude, int... vector) throws org.apache.commons.math3.exception.DimensionMismatchException
			public virtual Complex set(Complex magnitude, params int[] vector)
			{

				if (vector == null)
				{
					if (dimensionSize.Length > 0)
					{
						throw new DimensionMismatchException(0, dimensionSize.Length);
					}
					return null;
				}
				if (vector.Length != dimensionSize.Length)
				{
					throw new DimensionMismatchException(vector.Length, dimensionSize.Length);
				}

				object[] lastDimension = (object[]) multiDimensionalComplexArray;
				for (int i = 0; i < dimensionSize.Length - 1; i++)
				{
					lastDimension = (object[]) lastDimension[vector[i]];
				}

				Complex lastValue = (Complex) lastDimension[vector[dimensionSize.Length - 1]];
				lastDimension[vector[dimensionSize.Length - 1]] = magnitude;

				return lastValue;
			}

			/// <summary>
			/// Get the size in all dimensions.
			/// </summary>
			/// <returns> size in all dimensions </returns>
			public virtual int[] DimensionSizes
			{
				get
				{
					return dimensionSize.clone();
				}
			}

			/// <summary>
			/// Get the underlying storage array.
			/// </summary>
			/// <returns> underlying storage array </returns>
			public virtual object Array
			{
				get
				{
					return multiDimensionalComplexArray;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override object clone()
			{
				MultiDimensionalComplexMatrix mdcm = new MultiDimensionalComplexMatrix(Array.newInstance(typeof(Complex), dimensionSize));
				clone(mdcm);
				return mdcm;
			}

			/// <summary>
			/// Copy contents of current array into mdcm.
			/// </summary>
			/// <param name="mdcm"> array where to copy data </param>
			internal virtual void clone(MultiDimensionalComplexMatrix mdcm)
			{

				int[] vector = new int[dimensionSize.Length];
				int size = 1;
				for (int i = 0; i < dimensionSize.Length; i++)
				{
					size *= dimensionSize[i];
				}
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: int[][] vectorList = new int[size][dimensionSize.Length];
				int[][] vectorList = RectangularArrays.ReturnRectangularIntArray(size, dimensionSize.Length);
				foreach (int[] nextVector in vectorList)
				{
					Array.Copy(vector, 0, nextVector, 0, dimensionSize.Length);
					for (int i = 0; i < dimensionSize.Length; i++)
					{
						vector[i]++;
						if (vector[i] < dimensionSize[i])
						{
							break;
						}
						else
						{
							vector[i] = 0;
						}
					}
				}

				foreach (int[] nextVector in vectorList)
				{
					mdcm.set(get(nextVector), nextVector);
				}
			}
		}
	}

}
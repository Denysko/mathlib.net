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
namespace org.apache.commons.math3.transform
{

	using Complex = org.apache.commons.math3.complex.Complex;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// Useful functions for the implementation of various transforms.
	/// 
	/// @version $Id: TransformUtils.java 1385310 2012-09-16 16:32:10Z tn $
	/// @since 3.0
	/// </summary>
	public class TransformUtils
	{
		/// <summary>
		/// Table of the powers of 2 to facilitate binary search lookup.
		/// </summary>
		/// <seealso cref= #exactLog2(int) </seealso>
		private static readonly int[] POWERS_OF_TWO = new int[] {0x00000001, 0x00000002, 0x00000004, 0x00000008, 0x00000010, 0x00000020, 0x00000040, 0x00000080, 0x00000100, 0x00000200, 0x00000400, 0x00000800, 0x00001000, 0x00002000, 0x00004000, 0x00008000, 0x00010000, 0x00020000, 0x00040000, 0x00080000, 0x00100000, 0x00200000, 0x00400000, 0x00800000, 0x01000000, 0x02000000, 0x04000000, 0x08000000, 0x10000000, 0x20000000, 0x40000000};

		/// <summary>
		/// Private constructor. </summary>
		private TransformUtils() : base()
		{
		}

		/// <summary>
		/// Multiply every component in the given real array by the
		/// given real number. The change is made in place.
		/// </summary>
		/// <param name="f"> the real array to be scaled </param>
		/// <param name="d"> the real scaling coefficient </param>
		/// <returns> a reference to the scaled array </returns>
		public static double[] scaleArray(double[] f, double d)
		{

			for (int i = 0; i < f.Length; i++)
			{
				f[i] *= d;
			}
			return f;
		}

		/// <summary>
		/// Multiply every component in the given complex array by the
		/// given real number. The change is made in place.
		/// </summary>
		/// <param name="f"> the complex array to be scaled </param>
		/// <param name="d"> the real scaling coefficient </param>
		/// <returns> a reference to the scaled array </returns>
		public static Complex[] scaleArray(Complex[] f, double d)
		{

			for (int i = 0; i < f.Length; i++)
			{
				f[i] = new Complex(d * f[i].Real, d * f[i].Imaginary);
			}
			return f;
		}


		/// <summary>
		/// Builds a new two dimensional array of {@code double} filled with the real
		/// and imaginary parts of the specified <seealso cref="Complex"/> numbers. In the
		/// returned array {@code dataRI}, the data is laid out as follows
		/// <ul>
		/// <li>{@code dataRI[0][i] = dataC[i].getReal()},</li>
		/// <li>{@code dataRI[1][i] = dataC[i].getImaginary()}.</li>
		/// </ul>
		/// </summary>
		/// <param name="dataC"> the array of <seealso cref="Complex"/> data to be transformed </param>
		/// <returns> a two dimensional array filled with the real and imaginary parts
		///   of the specified complex input </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static double[][] createRealImaginaryArray(final org.apache.commons.math3.complex.Complex[] dataC)
		public static double[][] createRealImaginaryArray(Complex[] dataC)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] dataRI = new double[2][dataC.length];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] dataRI = new double[2][dataC.Length];
			double[][] dataRI = RectangularArrays.ReturnRectangularDoubleArray(2, dataC.Length);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dataR = dataRI[0];
			double[] dataR = dataRI[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dataI = dataRI[1];
			double[] dataI = dataRI[1];
			for (int i = 0; i < dataC.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.complex.Complex c = dataC[i];
				Complex c = dataC[i];
				dataR[i] = c.Real;
				dataI[i] = c.Imaginary;
			}
			return dataRI;
		}

		/// <summary>
		/// Builds a new array of <seealso cref="Complex"/> from the specified two dimensional
		/// array of real and imaginary parts. In the returned array {@code dataC},
		/// the data is laid out as follows
		/// <ul>
		/// <li>{@code dataC[i].getReal() = dataRI[0][i]},</li>
		/// <li>{@code dataC[i].getImaginary() = dataRI[1][i]}.</li>
		/// </ul>
		/// </summary>
		/// <param name="dataRI"> the array of real and imaginary parts to be transformed </param>
		/// <returns> an array of <seealso cref="Complex"/> with specified real and imaginary parts. </returns>
		/// <exception cref="DimensionMismatchException"> if the number of rows of the specified
		///   array is not two, or the array is not rectangular </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.apache.commons.math3.complex.Complex[] createComplexArray(final double[][] dataRI) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static Complex[] createComplexArray(double[][] dataRI)
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.complex.Complex[] c = new org.apache.commons.math3.complex.Complex[n];
			Complex[] c = new Complex[n];
			for (int i = 0; i < n; i++)
			{
				c[i] = new Complex(dataR[i], dataI[i]);
			}
			return c;
		}


		/// <summary>
		/// Returns the base-2 logarithm of the specified {@code int}. Throws an
		/// exception if {@code n} is not a power of two.
		/// </summary>
		/// <param name="n"> the {@code int} whose base-2 logarithm is to be evaluated </param>
		/// <returns> the base-2 logarithm of {@code n} </returns>
		/// <exception cref="MathIllegalArgumentException"> if {@code n} is not a power of two </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int exactLog2(final int n) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static int exactLog2(int n)
		{

			int index = Arrays.binarySearch(TransformUtils.POWERS_OF_TWO, n);
			if (index < 0)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.NOT_POWER_OF_TWO_CONSIDER_PADDING, Convert.ToInt32(n));
			}
			return index;
		}
	}

}
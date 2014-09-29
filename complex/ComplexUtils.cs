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

namespace mathlib.complex
{

    using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
    using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
    using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Static implementations of common
	/// <seealso cref="mathlib.complex.Complex"/> utilities functions.
	/// 
	/// @version $Id: ComplexUtils.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class ComplexUtils
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		private ComplexUtils()
		{
		}

		/// <summary>
		/// Creates a complex number from the given polar representation.
		/// <p>
		/// The value returned is <code>r&middot;e<sup>i&middot;theta</sup></code>,
		/// computed as <code>r&middot;cos(theta) + r&middot;sin(theta)i</code></p>
		/// <p>
		/// If either <code>r</code> or <code>theta</code> is NaN, or
		/// <code>theta</code> is infinite, <seealso cref="Complex#NaN"/> is returned.</p>
		/// <p>
		/// If <code>r</code> is infinite and <code>theta</code> is finite,
		/// infinite or NaN values may be returned in parts of the result, following
		/// the rules for double arithmetic.<pre>
		/// Examples:
		/// <code>
		/// polar2Complex(INFINITY, &pi;/4) = INFINITY + INFINITY i
		/// polar2Complex(INFINITY, 0) = INFINITY + NaN i
		/// polar2Complex(INFINITY, -&pi;/4) = INFINITY - INFINITY i
		/// polar2Complex(INFINITY, 5&pi;/4) = -INFINITY - INFINITY i </code></pre></p>
		/// </summary>
		/// <param name="r"> the modulus of the complex number to create </param>
		/// <param name="theta">  the argument of the complex number to create </param>
		/// <returns> <code>r&middot;e<sup>i&middot;theta</sup></code> </returns>
		/// <exception cref="MathIllegalArgumentException"> if {@code r} is negative.
		/// @since 1.1 </exception>
		public static Complex Polar2Complex(double r, double theta)
		{
			if (r < 0)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.NEGATIVE_COMPLEX_MODULE, r);
			}
			return new Complex(r * FastMath.Cos(theta), r * FastMath.Sin(theta));
		}

		/// <summary>
		/// Convert an array of primitive doubles to an array of {@code Complex} objects.
		/// </summary>
		/// <param name="real"> Array of numbers to be converted to their {@code Complex}
		/// equivalent. </param>
		/// <returns> an array of {@code Complex} objects.
		/// 
		/// @since 3.1 </returns>
		public static Complex[] ConvertToComplex(double[] real)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Complex c[] = new Complex[real.length];
			Complex[] c = new Complex[real.Length];
			for (int i = 0; i < real.Length; i++)
			{
				c[i] = new Complex(real[i], 0);
			}

			return c;
		}
	}

}
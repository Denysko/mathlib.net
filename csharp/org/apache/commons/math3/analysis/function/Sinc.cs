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

namespace org.apache.commons.math3.analysis.function
{

	using DerivativeStructure = org.apache.commons.math3.analysis.differentiation.DerivativeStructure;
	using UnivariateDifferentiableFunction = org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// <a href="http://en.wikipedia.org/wiki/Sinc_function">Sinc</a> function,
	/// defined by
	/// <pre><code>
	///   sinc(x) = 1            if x = 0,
	///             sin(x) / x   otherwise.
	/// </code></pre>
	/// 
	/// @since 3.0
	/// @version $Id: Sinc.java 1455194 2013-03-11 15:45:54Z luc $
	/// </summary>
	public class Sinc : UnivariateDifferentiableFunction, DifferentiableUnivariateFunction
	{
		/// <summary>
		/// Value below which the computations are done using Taylor series.
		/// <p>
		/// The Taylor series for sinc even order derivatives are:
		/// <pre>
		/// d^(2n)sinc/dx^(2n)     = Sum_(k>=0) (-1)^(n+k) / ((2k)!(2n+2k+1)) x^(2k)
		///                        = (-1)^n     [ 1/(2n+1) - x^2/(4n+6) + x^4/(48n+120) - x^6/(1440n+5040) + O(x^8) ]
		/// </pre>
		/// </p>
		/// <p>
		/// The Taylor series for sinc odd order derivatives are:
		/// <pre>
		/// d^(2n+1)sinc/dx^(2n+1) = Sum_(k>=0) (-1)^(n+k+1) / ((2k+1)!(2n+2k+3)) x^(2k+1)
		///                        = (-1)^(n+1) [ x/(2n+3) - x^3/(12n+30) + x^5/(240n+840) - x^7/(10080n+45360) + O(x^9) ]
		/// </pre>
		/// </p>
		/// <p>
		/// So the ratio of the fourth term with respect to the first term
		/// is always smaller than x^6/720, for all derivative orders.
		/// This implies that neglecting this term and using only the first three terms induces
		/// a relative error bounded by x^6/720. The SHORTCUT value is chosen such that this
		/// relative error is below double precision accuracy when |x| <= SHORTCUT.
		/// </p>
		/// </summary>
		private const double SHORTCUT = 6.0e-3;
		/// <summary>
		/// For normalized sinc function. </summary>
		private readonly bool normalized;

		/// <summary>
		/// The sinc function, {@code sin(x) / x}.
		/// </summary>
		public Sinc() : this(false)
		{
		}

		/// <summary>
		/// Instantiates the sinc function.
		/// </summary>
		/// <param name="normalized"> If {@code true}, the function is
		/// <code> sin(&pi;x) / &pi;x</code>, otherwise {@code sin(x) / x}. </param>
		public Sinc(bool normalized)
		{
			this.normalized = normalized;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double value(final double x)
		public virtual double value(double x)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double scaledX = normalized ? org.apache.commons.math3.util.FastMath.PI * x : x;
			double scaledX = normalized ? FastMath.PI * x : x;
			if (FastMath.abs(scaledX) <= SHORTCUT)
			{
				// use Taylor series
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double scaledX2 = scaledX * scaledX;
				double scaledX2 = scaledX * scaledX;
				return ((scaledX2 - 20) * scaledX2 + 120) / 120;
			}
			else
			{
				// use definition expression
				return FastMath.sin(scaledX) / scaledX;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// @deprecated as of 3.1, replaced by <seealso cref="#value(DerivativeStructure)"/> 
		[Obsolete("as of 3.1, replaced by <seealso cref="#value(org.apache.commons.math3.analysis.differentiation.DerivativeStructure)"/>")]
		public virtual UnivariateFunction derivative()
		{
			return FunctionUtils.toDifferentiableUnivariateFunction(this).derivative();
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.analysis.differentiation.DerivativeStructure value(final org.apache.commons.math3.analysis.differentiation.DerivativeStructure t) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure value(DerivativeStructure t)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double scaledX = (normalized ? org.apache.commons.math3.util.FastMath.PI : 1) * t.getValue();
			double scaledX = (normalized ? FastMath.PI : 1) * t.Value;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double scaledX2 = scaledX * scaledX;
			double scaledX2 = scaledX * scaledX;

			double[] f = new double[t.Order + 1];

			if (FastMath.abs(scaledX) <= SHORTCUT)
			{

				for (int i = 0; i < f.Length; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int k = i / 2;
					int k = i / 2;
					if ((i & 0x1) == 0)
					{
						// even derivation order
						f[i] = (((k & 0x1) == 0) ? 1 : -1) * (1.0 / (i + 1) - scaledX2 * (1.0 / (2 * i + 6) - scaledX2 / (24 * i + 120)));
					}
					else
					{
						// odd derivation order
						f[i] = (((k & 0x1) == 0) ? - scaledX : scaledX) * (1.0 / (i + 2) - scaledX2 * (1.0 / (6 * i + 24) - scaledX2 / (120 * i + 720)));
					}
				}

			}
			else
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double inv = 1 / scaledX;
				double inv = 1 / scaledX;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cos = org.apache.commons.math3.util.FastMath.cos(scaledX);
				double cos = FastMath.cos(scaledX);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sin = org.apache.commons.math3.util.FastMath.sin(scaledX);
				double sin = FastMath.sin(scaledX);

				f[0] = inv * sin;

				// the nth order derivative of sinc has the form:
				// dn(sinc(x)/dxn = [S_n(x) sin(x) + C_n(x) cos(x)] / x^(n+1)
				// where S_n(x) is an even polynomial with degree n-1 or n (depending on parity)
				// and C_n(x) is an odd polynomial with degree n-1 or n (depending on parity)
				// S_0(x) = 1, S_1(x) = -1, S_2(x) = -x^2 + 2, S_3(x) = 3x^2 - 6...
				// C_0(x) = 0, C_1(x) = x, C_2(x) = -2x, C_3(x) = -x^3 + 6x...
				// the general recurrence relations for S_n and C_n are:
				// S_n(x) = x S_(n-1)'(x) - n S_(n-1)(x) - x C_(n-1)(x)
				// C_n(x) = x C_(n-1)'(x) - n C_(n-1)(x) + x S_(n-1)(x)
				// as per polynomials parity, we can store both S_n and C_n in the same array
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] sc = new double[f.length];
				double[] sc = new double[f.Length];
				sc[0] = 1;

				double coeff = inv;
				for (int n = 1; n < f.Length; ++n)
				{

					double s = 0;
					double c = 0;

					// update and evaluate polynomials S_n(x) and C_n(x)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int kStart;
					int kStart;
					if ((n & 0x1) == 0)
					{
						// even derivation order, S_n is degree n and C_n is degree n-1
						sc[n] = 0;
						kStart = n;
					}
					else
					{
						// odd derivation order, S_n is degree n-1 and C_n is degree n
						sc[n] = sc[n - 1];
						c = sc[n];
						kStart = n - 1;
					}

					// in this loop, k is always even
					for (int k = kStart; k > 1; k -= 2)
					{

						// sine part
						sc[k] = (k - n) * sc[k] - sc[k - 1];
						s = s * scaledX2 + sc[k];

						// cosine part
						sc[k - 1] = (k - 1 - n) * sc[k - 1] + sc[k - 2];
						c = c * scaledX2 + sc[k - 1];

					}
					sc[0] *= -n;
					s = s * scaledX2 + sc[0];

					coeff *= inv;
					f[n] = coeff * (s * sin + c * scaledX * cos);

				}

			}

			if (normalized)
			{
				double scale = FastMath.PI;
				for (int i = 1; i < f.Length; ++i)
				{
					f[i] *= scale;
					scale *= FastMath.PI;
				}
			}

			return t.compose(f);

		}

	}

}
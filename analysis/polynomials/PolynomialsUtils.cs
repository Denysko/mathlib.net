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
namespace mathlib.analysis.polynomials
{


	using BigFraction = mathlib.fraction.BigFraction;
	using CombinatoricsUtils = mathlib.util.CombinatoricsUtils;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// A collection of static methods that operate on or return polynomials.
	/// 
	/// @version $Id: PolynomialsUtils.java 1517203 2013-08-24 21:55:35Z psteitz $
	/// @since 2.0
	/// </summary>
	public class PolynomialsUtils
	{

		/// <summary>
		/// Coefficients for Chebyshev polynomials. </summary>
		private static readonly IList<BigFraction> CHEBYSHEV_COEFFICIENTS;

		/// <summary>
		/// Coefficients for Hermite polynomials. </summary>
		private static readonly IList<BigFraction> HERMITE_COEFFICIENTS;

		/// <summary>
		/// Coefficients for Laguerre polynomials. </summary>
		private static readonly IList<BigFraction> LAGUERRE_COEFFICIENTS;

		/// <summary>
		/// Coefficients for Legendre polynomials. </summary>
		private static readonly IList<BigFraction> LEGENDRE_COEFFICIENTS;

		/// <summary>
		/// Coefficients for Jacobi polynomials. </summary>
		private static readonly IDictionary<JacobiKey, IList<BigFraction>> JACOBI_COEFFICIENTS;

		static PolynomialsUtils()
		{

			// initialize recurrence for Chebyshev polynomials
			// T0(X) = 1, T1(X) = 0 + 1 * X
			CHEBYSHEV_COEFFICIENTS = new List<BigFraction>();
			CHEBYSHEV_COEFFICIENTS.Add(BigFraction.ONE);
			CHEBYSHEV_COEFFICIENTS.Add(BigFraction.ZERO);
			CHEBYSHEV_COEFFICIENTS.Add(BigFraction.ONE);

			// initialize recurrence for Hermite polynomials
			// H0(X) = 1, H1(X) = 0 + 2 * X
			HERMITE_COEFFICIENTS = new List<BigFraction>();
			HERMITE_COEFFICIENTS.Add(BigFraction.ONE);
			HERMITE_COEFFICIENTS.Add(BigFraction.ZERO);
			HERMITE_COEFFICIENTS.Add(BigFraction.TWO);

			// initialize recurrence for Laguerre polynomials
			// L0(X) = 1, L1(X) = 1 - 1 * X
			LAGUERRE_COEFFICIENTS = new List<BigFraction>();
			LAGUERRE_COEFFICIENTS.Add(BigFraction.ONE);
			LAGUERRE_COEFFICIENTS.Add(BigFraction.ONE);
			LAGUERRE_COEFFICIENTS.Add(BigFraction.MINUS_ONE);

			// initialize recurrence for Legendre polynomials
			// P0(X) = 1, P1(X) = 0 + 1 * X
			LEGENDRE_COEFFICIENTS = new List<BigFraction>();
			LEGENDRE_COEFFICIENTS.Add(BigFraction.ONE);
			LEGENDRE_COEFFICIENTS.Add(BigFraction.ZERO);
			LEGENDRE_COEFFICIENTS.Add(BigFraction.ONE);

			// initialize map for Jacobi polynomials
			JACOBI_COEFFICIENTS = new Dictionary<JacobiKey, IList<BigFraction>>();

		}

		/// <summary>
		/// Private constructor, to prevent instantiation.
		/// </summary>
		private PolynomialsUtils()
		{
		}

		/// <summary>
		/// Create a Chebyshev polynomial of the first kind.
		/// <p><a href="http://mathworld.wolfram.com/ChebyshevPolynomialoftheFirstKind.html">Chebyshev
		/// polynomials of the first kind</a> are orthogonal polynomials.
		/// They can be defined by the following recurrence relations:
		/// <pre>
		///  T<sub>0</sub>(X)   = 1
		///  T<sub>1</sub>(X)   = X
		///  T<sub>k+1</sub>(X) = 2X T<sub>k</sub>(X) - T<sub>k-1</sub>(X)
		/// </pre></p> </summary>
		/// <param name="degree"> degree of the polynomial </param>
		/// <returns> Chebyshev polynomial of specified degree </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static PolynomialFunction createChebyshevPolynomial(final int degree)
		public static PolynomialFunction createChebyshevPolynomial(int degree)
		{
			return buildPolynomial(degree, CHEBYSHEV_COEFFICIENTS, new RecurrenceCoefficientsGeneratorAnonymousInnerClassHelper());
		}

		private class RecurrenceCoefficientsGeneratorAnonymousInnerClassHelper : RecurrenceCoefficientsGenerator
		{
			public RecurrenceCoefficientsGeneratorAnonymousInnerClassHelper()
			{
			}

			private readonly BigFraction[] coeffs = new BigFraction[] {BigFraction.ZERO, BigFraction.TWO, BigFraction.ONE};
			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual BigFraction[] generate(int k)
			{
				return coeffs;
			}
		}

		/// <summary>
		/// Create a Hermite polynomial.
		/// <p><a href="http://mathworld.wolfram.com/HermitePolynomial.html">Hermite
		/// polynomials</a> are orthogonal polynomials.
		/// They can be defined by the following recurrence relations:
		/// <pre>
		///  H<sub>0</sub>(X)   = 1
		///  H<sub>1</sub>(X)   = 2X
		///  H<sub>k+1</sub>(X) = 2X H<sub>k</sub>(X) - 2k H<sub>k-1</sub>(X)
		/// </pre></p>
		/// </summary>
		/// <param name="degree"> degree of the polynomial </param>
		/// <returns> Hermite polynomial of specified degree </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static PolynomialFunction createHermitePolynomial(final int degree)
		public static PolynomialFunction createHermitePolynomial(int degree)
		{
			return buildPolynomial(degree, HERMITE_COEFFICIENTS, new RecurrenceCoefficientsGeneratorAnonymousInnerClassHelper2());
		}

		private class RecurrenceCoefficientsGeneratorAnonymousInnerClassHelper2 : RecurrenceCoefficientsGenerator
		{
			public RecurrenceCoefficientsGeneratorAnonymousInnerClassHelper2()
			{
			}

					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual BigFraction[] generate(int k)
			{
				return new BigFraction[] {BigFraction.ZERO, BigFraction.TWO, new BigFraction(2 * k)};
			}
		}

		/// <summary>
		/// Create a Laguerre polynomial.
		/// <p><a href="http://mathworld.wolfram.com/LaguerrePolynomial.html">Laguerre
		/// polynomials</a> are orthogonal polynomials.
		/// They can be defined by the following recurrence relations:
		/// <pre>
		///        L<sub>0</sub>(X)   = 1
		///        L<sub>1</sub>(X)   = 1 - X
		///  (k+1) L<sub>k+1</sub>(X) = (2k + 1 - X) L<sub>k</sub>(X) - k L<sub>k-1</sub>(X)
		/// </pre></p> </summary>
		/// <param name="degree"> degree of the polynomial </param>
		/// <returns> Laguerre polynomial of specified degree </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static PolynomialFunction createLaguerrePolynomial(final int degree)
		public static PolynomialFunction createLaguerrePolynomial(int degree)
		{
			return buildPolynomial(degree, LAGUERRE_COEFFICIENTS, new RecurrenceCoefficientsGeneratorAnonymousInnerClassHelper3());
		}

		private class RecurrenceCoefficientsGeneratorAnonymousInnerClassHelper3 : RecurrenceCoefficientsGenerator
		{
			public RecurrenceCoefficientsGeneratorAnonymousInnerClassHelper3()
			{
			}

					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual BigFraction[] generate(int k)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int kP1 = k + 1;
				int kP1 = k + 1;
				return new BigFraction[] {new BigFraction(2 * k + 1, kP1), new BigFraction(-1, kP1), new BigFraction(k, kP1)};
			}
		}

		/// <summary>
		/// Create a Legendre polynomial.
		/// <p><a href="http://mathworld.wolfram.com/LegendrePolynomial.html">Legendre
		/// polynomials</a> are orthogonal polynomials.
		/// They can be defined by the following recurrence relations:
		/// <pre>
		///        P<sub>0</sub>(X)   = 1
		///        P<sub>1</sub>(X)   = X
		///  (k+1) P<sub>k+1</sub>(X) = (2k+1) X P<sub>k</sub>(X) - k P<sub>k-1</sub>(X)
		/// </pre></p> </summary>
		/// <param name="degree"> degree of the polynomial </param>
		/// <returns> Legendre polynomial of specified degree </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static PolynomialFunction createLegendrePolynomial(final int degree)
		public static PolynomialFunction createLegendrePolynomial(int degree)
		{
			return buildPolynomial(degree, LEGENDRE_COEFFICIENTS, new RecurrenceCoefficientsGeneratorAnonymousInnerClassHelper4());
		}

		private class RecurrenceCoefficientsGeneratorAnonymousInnerClassHelper4 : RecurrenceCoefficientsGenerator
		{
			public RecurrenceCoefficientsGeneratorAnonymousInnerClassHelper4()
			{
			}

					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual BigFraction[] generate(int k)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int kP1 = k + 1;
				int kP1 = k + 1;
				return new BigFraction[] {BigFraction.ZERO, new BigFraction(k + kP1, kP1), new BigFraction(k, kP1)};
			}
		}

		/// <summary>
		/// Create a Jacobi polynomial.
		/// <p><a href="http://mathworld.wolfram.com/JacobiPolynomial.html">Jacobi
		/// polynomials</a> are orthogonal polynomials.
		/// They can be defined by the following recurrence relations:
		/// <pre>
		///        P<sub>0</sub><sup>vw</sup>(X)   = 1
		///        P<sub>-1</sub><sup>vw</sup>(X)  = 0
		///  2k(k + v + w)(2k + v + w - 2) P<sub>k</sub><sup>vw</sup>(X) =
		///  (2k + v + w - 1)[(2k + v + w)(2k + v + w - 2) X + v<sup>2</sup> - w<sup>2</sup>] P<sub>k-1</sub><sup>vw</sup>(X)
		///  - 2(k + v - 1)(k + w - 1)(2k + v + w) P<sub>k-2</sub><sup>vw</sup>(X)
		/// </pre></p> </summary>
		/// <param name="degree"> degree of the polynomial </param>
		/// <param name="v"> first exponent </param>
		/// <param name="w"> second exponent </param>
		/// <returns> Jacobi polynomial of specified degree </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static PolynomialFunction createJacobiPolynomial(final int degree, final int v, final int w)
		public static PolynomialFunction createJacobiPolynomial(int degree, int v, int w)
		{

			// select the appropriate list
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final JacobiKey key = new JacobiKey(v, w);
			JacobiKey key = new JacobiKey(v, w);

			if (!JACOBI_COEFFICIENTS.ContainsKey(key))
			{

				// allocate a new list for v, w
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<mathlib.fraction.BigFraction> list = new java.util.ArrayList<mathlib.fraction.BigFraction>();
				IList<BigFraction> list = new List<BigFraction>();
				JACOBI_COEFFICIENTS[key] = list;

				// Pv,w,0(x) = 1;
				list.Add(BigFraction.ONE);

				// P1(x) = (v - w) / 2 + (2 + v + w) * X / 2
				list.Add(new BigFraction(v - w, 2));
				list.Add(new BigFraction(2 + v + w, 2));

			}

			return buildPolynomial(degree, JACOBI_COEFFICIENTS[key], new RecurrenceCoefficientsGeneratorAnonymousInnerClassHelper5(v, w));

		}

		private class RecurrenceCoefficientsGeneratorAnonymousInnerClassHelper5 : RecurrenceCoefficientsGenerator
		{
			private int v;
			private int w;

			public RecurrenceCoefficientsGeneratorAnonymousInnerClassHelper5(int v, int w)
			{
				this.v = v;
				this.w = w;
			}

					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual BigFraction[] generate(int k)
			{
				k++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int kvw = k + v + w;
				int kvw = k + v + w;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int twoKvw = kvw + k;
				int twoKvw = kvw + k;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int twoKvwM1 = twoKvw - 1;
				int twoKvwM1 = twoKvw - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int twoKvwM2 = twoKvw - 2;
				int twoKvwM2 = twoKvw - 2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int den = 2 * k * kvw * twoKvwM2;
				int den = 2 * k * kvw * twoKvwM2;

				return new BigFraction[] {new BigFraction(twoKvwM1 * (v * v - w * w), den), new BigFraction(twoKvwM1 * twoKvw * twoKvwM2, den), new BigFraction(2 * (k + v - 1) * (k + w - 1) * twoKvw, den)};
			}
		}

		/// <summary>
		/// Inner class for Jacobi polynomials keys. </summary>
		private class JacobiKey
		{

			/// <summary>
			/// First exponent. </summary>
			internal readonly int v;

			/// <summary>
			/// Second exponent. </summary>
			internal readonly int w;

			/// <summary>
			/// Simple constructor. </summary>
			/// <param name="v"> first exponent </param>
			/// <param name="w"> second exponent </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public JacobiKey(final int v, final int w)
			public JacobiKey(int v, int w)
			{
				this.v = v;
				this.w = w;
			}

			/// <summary>
			/// Get hash code. </summary>
			/// <returns> hash code </returns>
			public override int GetHashCode()
			{
				return (v << 16) ^ w;
			}

			/// <summary>
			/// Check if the instance represent the same key as another instance. </summary>
			/// <param name="key"> other key </param>
			/// <returns> true if the instance and the other key refer to the same polynomial </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public boolean equals(final Object key)
			public override bool Equals(object key)
			{

				if ((key == null) || !(key is JacobiKey))
				{
					return false;
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final JacobiKey otherK = (JacobiKey) key;
				JacobiKey otherK = (JacobiKey) key;
				return (v == otherK.v) && (w == otherK.w);

			}
		}

		/// <summary>
		/// Compute the coefficients of the polynomial <code>P<sub>s</sub>(x)</code>
		/// whose values at point {@code x} will be the same as the those from the
		/// original polynomial <code>P(x)</code> when computed at {@code x + shift}.
		/// Thus, if <code>P(x) = &Sigma;<sub>i</sub> a<sub>i</sub> x<sup>i</sup></code>,
		/// then
		/// <pre>
		///  <table>
		///   <tr>
		///    <td><code>P<sub>s</sub>(x)</td>
		///    <td>= &Sigma;<sub>i</sub> b<sub>i</sub> x<sup>i</sup></code></td>
		///   </tr>
		///   <tr>
		///    <td></td>
		///    <td>= &Sigma;<sub>i</sub> a<sub>i</sub> (x + shift)<sup>i</sup></code></td>
		///   </tr>
		///  </table>
		/// </pre>
		/// </summary>
		/// <param name="coefficients"> Coefficients of the original polynomial. </param>
		/// <param name="shift"> Shift value. </param>
		/// <returns> the coefficients <code>b<sub>i</sub></code> of the shifted
		/// polynomial. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static double[] shift(final double[] coefficients, final double shift)
		public static double[] shift(double[] coefficients, double shift)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dp1 = coefficients.length;
			int dp1 = coefficients.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] newCoefficients = new double[dp1];
			double[] newCoefficients = new double[dp1];

			// Pascal triangle.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[][] coeff = new int[dp1][dp1];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: int[][] coeff = new int[dp1][dp1];
			int[][] coeff = RectangularArrays.ReturnRectangularIntArray(dp1, dp1);
			for (int i = 0; i < dp1; i++)
			{
				for (int j = 0; j <= i; j++)
				{
					coeff[i][j] = (int) CombinatoricsUtils.binomialCoefficient(i, j);
				}
			}

			// First polynomial coefficient.
			for (int i = 0; i < dp1; i++)
			{
				newCoefficients[0] += coefficients[i] * FastMath.pow(shift, i);
			}

			// Superior order.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int d = dp1 - 1;
			int d = dp1 - 1;
			for (int i = 0; i < d; i++)
			{
				for (int j = i; j < d; j++)
				{
					newCoefficients[i + 1] += coeff[j + 1][j - i] * coefficients[j + 1] * FastMath.pow(shift, j - i);
				}
			}

			return newCoefficients;
		}


		/// <summary>
		/// Get the coefficients array for a given degree. </summary>
		/// <param name="degree"> degree of the polynomial </param>
		/// <param name="coefficients"> list where the computed coefficients are stored </param>
		/// <param name="generator"> recurrence coefficients generator </param>
		/// <returns> coefficients array </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static PolynomialFunction buildPolynomial(final int degree, final java.util.List<mathlib.fraction.BigFraction> coefficients, final RecurrenceCoefficientsGenerator generator)
		private static PolynomialFunction buildPolynomial(int degree, IList<BigFraction> coefficients, RecurrenceCoefficientsGenerator generator)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int maxDegree = (int) mathlib.util.FastMath.floor(mathlib.util.FastMath.sqrt(2 * coefficients.size())) - 1;
			int maxDegree = (int) FastMath.floor(FastMath.sqrt(2 * coefficients.Count)) - 1;
			lock (typeof(PolynomialsUtils))
			{
				if (degree > maxDegree)
				{
					computeUpToDegree(degree, maxDegree, generator, coefficients);
				}
			}

			// coefficient  for polynomial 0 is  l [0]
			// coefficients for polynomial 1 are l [1] ... l [2] (degrees 0 ... 1)
			// coefficients for polynomial 2 are l [3] ... l [5] (degrees 0 ... 2)
			// coefficients for polynomial 3 are l [6] ... l [9] (degrees 0 ... 3)
			// coefficients for polynomial 4 are l[10] ... l[14] (degrees 0 ... 4)
			// coefficients for polynomial 5 are l[15] ... l[20] (degrees 0 ... 5)
			// coefficients for polynomial 6 are l[21] ... l[27] (degrees 0 ... 6)
			// ...
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int start = degree * (degree + 1) / 2;
			int start = degree * (degree + 1) / 2;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] a = new double[degree + 1];
			double[] a = new double[degree + 1];
			for (int i = 0; i <= degree; ++i)
			{
				a[i] = (double)coefficients[start + i];
			}

			// build the polynomial
			return new PolynomialFunction(a);

		}

		/// <summary>
		/// Compute polynomial coefficients up to a given degree. </summary>
		/// <param name="degree"> maximal degree </param>
		/// <param name="maxDegree"> current maximal degree </param>
		/// <param name="generator"> recurrence coefficients generator </param>
		/// <param name="coefficients"> list where the computed coefficients should be appended </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void computeUpToDegree(final int degree, final int maxDegree, final RecurrenceCoefficientsGenerator generator, final java.util.List<mathlib.fraction.BigFraction> coefficients)
		private static void computeUpToDegree(int degree, int maxDegree, RecurrenceCoefficientsGenerator generator, IList<BigFraction> coefficients)
		{

			int startK = (maxDegree - 1) * maxDegree / 2;
			for (int k = maxDegree; k < degree; ++k)
			{

				// start indices of two previous polynomials Pk(X) and Pk-1(X)
				int startKm1 = startK;
				startK += k;

				// Pk+1(X) = (a[0] + a[1] X) Pk(X) - a[2] Pk-1(X)
				BigFraction[] ai = generator.generate(k);

				BigFraction ck = coefficients[startK];
				BigFraction ckm1 = coefficients[startKm1];

				// degree 0 coefficient
				coefficients.Add(ck.multiply(ai[0]).subtract(ckm1.multiply(ai[2])));

				// degree 1 to degree k-1 coefficients
				for (int i = 1; i < k; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction ckPrev = ck;
					BigFraction ckPrev = ck;
					ck = coefficients[startK + i];
					ckm1 = coefficients[startKm1 + i];
					coefficients.Add(ck.multiply(ai[0]).add(ckPrev.multiply(ai[1])).subtract(ckm1.multiply(ai[2])));
				}

				// degree k coefficient
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction ckPrev = ck;
				BigFraction ckPrev = ck;
				ck = coefficients[startK + k];
				coefficients.Add(ck.multiply(ai[0]).add(ckPrev.multiply(ai[1])));

				// degree k+1 coefficient
				coefficients.Add(ck.multiply(ai[1]));

			}

		}

		/// <summary>
		/// Interface for recurrence coefficients generation. </summary>
		private interface RecurrenceCoefficientsGenerator
		{
			/// <summary>
			/// Generate recurrence coefficients. </summary>
			/// <param name="k"> highest degree of the polynomials used in the recurrence </param>
			/// <returns> an array of three coefficients such that
			/// P<sub>k+1</sub>(X) = (a[0] + a[1] X) P<sub>k</sub>(X) - a[2] P<sub>k-1</sub>(X) </returns>
			BigFraction[] generate(int k);
		}

	}

}
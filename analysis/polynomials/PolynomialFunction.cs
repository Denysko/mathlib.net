using System;
using System.Text;

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
namespace org.apache.commons.math3.analysis.polynomials
{


	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using NoDataException = org.apache.commons.math3.exception.NoDataException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using DerivativeStructure = org.apache.commons.math3.analysis.differentiation.DerivativeStructure;
	using UnivariateDifferentiableFunction = org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// Immutable representation of a real polynomial function with real coefficients.
	/// <p>
	/// <a href="http://mathworld.wolfram.com/HornersMethod.html">Horner's Method</a>
	/// is used to evaluate the function.</p>
	/// 
	/// @version $Id: PolynomialFunction.java 1455194 2013-03-11 15:45:54Z luc $
	/// </summary>
	[Serializable]
	public class PolynomialFunction : UnivariateDifferentiableFunction, DifferentiableUnivariateFunction
	{
		/// <summary>
		/// Serialization identifier
		/// </summary>
		private const long serialVersionUID = -7726511984200295583L;
		/// <summary>
		/// The coefficients of the polynomial, ordered by degree -- i.e.,
		/// coefficients[0] is the constant term and coefficients[n] is the
		/// coefficient of x^n where n is the degree of the polynomial.
		/// </summary>
		private readonly double[] coefficients;

		/// <summary>
		/// Construct a polynomial with the given coefficients.  The first element
		/// of the coefficients array is the constant term.  Higher degree
		/// coefficients follow in sequence.  The degree of the resulting polynomial
		/// is the index of the last non-null element of the array, or 0 if all elements
		/// are null.
		/// <p>
		/// The constructor makes a copy of the input array and assigns the copy to
		/// the coefficients property.</p>
		/// </summary>
		/// <param name="c"> Polynomial coefficients. </param>
		/// <exception cref="NullArgumentException"> if {@code c} is {@code null}. </exception>
		/// <exception cref="NoDataException"> if {@code c} is empty. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PolynomialFunction(double c[]) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoDataException
		public PolynomialFunction(double[] c) : base()
		{
			MathUtils.checkNotNull(c);
			int n = c.Length;
			if (n == 0)
			{
				throw new NoDataException(LocalizedFormats.EMPTY_POLYNOMIALS_COEFFICIENTS_ARRAY);
			}
			while ((n > 1) && (c[n - 1] == 0))
			{
				--n;
			}
			this.coefficients = new double[n];
			Array.Copy(c, 0, this.coefficients, 0, n);
		}

		/// <summary>
		/// Compute the value of the function for the given argument.
		/// <p>
		///  The value returned is <br/>
		///  <code>coefficients[n] * x^n + ... + coefficients[1] * x  + coefficients[0]</code>
		/// </p>
		/// </summary>
		/// <param name="x"> Argument for which the function value should be computed. </param>
		/// <returns> the value of the polynomial at the given point. </returns>
		/// <seealso cref= UnivariateFunction#value(double) </seealso>
		public virtual double value(double x)
		{
		   return evaluate(coefficients, x);
		}

		/// <summary>
		/// Returns the degree of the polynomial.
		/// </summary>
		/// <returns> the degree of the polynomial. </returns>
		public virtual int degree()
		{
			return coefficients.Length - 1;
		}

		/// <summary>
		/// Returns a copy of the coefficients array.
		/// <p>
		/// Changes made to the returned copy will not affect the coefficients of
		/// the polynomial.</p>
		/// </summary>
		/// <returns> a fresh copy of the coefficients array. </returns>
		public virtual double[] Coefficients
		{
			get
			{
				return coefficients.clone();
			}
		}

		/// <summary>
		/// Uses Horner's Method to evaluate the polynomial with the given coefficients at
		/// the argument.
		/// </summary>
		/// <param name="coefficients"> Coefficients of the polynomial to evaluate. </param>
		/// <param name="argument"> Input value. </param>
		/// <returns> the value of the polynomial. </returns>
		/// <exception cref="NoDataException"> if {@code coefficients} is empty. </exception>
		/// <exception cref="NullArgumentException"> if {@code coefficients} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static double evaluate(double[] coefficients, double argument) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoDataException
		protected internal static double evaluate(double[] coefficients, double argument)
		{
			MathUtils.checkNotNull(coefficients);
			int n = coefficients.Length;
			if (n == 0)
			{
				throw new NoDataException(LocalizedFormats.EMPTY_POLYNOMIALS_COEFFICIENTS_ARRAY);
			}
			double result = coefficients[n - 1];
			for (int j = n - 2; j >= 0; j--)
			{
				result = argument * result + coefficients[j];
			}
			return result;
		}


		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1 </summary>
		/// <exception cref="NoDataException"> if {@code coefficients} is empty. </exception>
		/// <exception cref="NullArgumentException"> if {@code coefficients} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.analysis.differentiation.DerivativeStructure value(final org.apache.commons.math3.analysis.differentiation.DerivativeStructure t) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoDataException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure value(DerivativeStructure t)
		{
			MathUtils.checkNotNull(coefficients);
			int n = coefficients.Length;
			if (n == 0)
			{
				throw new NoDataException(LocalizedFormats.EMPTY_POLYNOMIALS_COEFFICIENTS_ARRAY);
			}
			DerivativeStructure result = new DerivativeStructure(t.FreeParameters, t.Order, coefficients[n - 1]);
			for (int j = n - 2; j >= 0; j--)
			{
				result = result.multiply(t).add(coefficients[j]);
			}
			return result;
		}

		/// <summary>
		/// Add a polynomial to the instance.
		/// </summary>
		/// <param name="p"> Polynomial to add. </param>
		/// <returns> a new polynomial which is the sum of the instance and {@code p}. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PolynomialFunction add(final PolynomialFunction p)
		public virtual PolynomialFunction add(PolynomialFunction p)
		{
			// identify the lowest degree polynomial
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lowLength = org.apache.commons.math3.util.FastMath.min(coefficients.length, p.coefficients.length);
			int lowLength = FastMath.min(coefficients.Length, p.coefficients.Length);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int highLength = org.apache.commons.math3.util.FastMath.max(coefficients.length, p.coefficients.length);
			int highLength = FastMath.max(coefficients.Length, p.coefficients.Length);

			// build the coefficients array
			double[] newCoefficients = new double[highLength];
			for (int i = 0; i < lowLength; ++i)
			{
				newCoefficients[i] = coefficients[i] + p.coefficients[i];
			}
			Array.Copy((coefficients.Length < p.coefficients.Length) ? p.coefficients : coefficients, lowLength, newCoefficients, lowLength, highLength - lowLength);

			return new PolynomialFunction(newCoefficients);
		}

		/// <summary>
		/// Subtract a polynomial from the instance.
		/// </summary>
		/// <param name="p"> Polynomial to subtract. </param>
		/// <returns> a new polynomial which is the difference the instance minus {@code p}. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PolynomialFunction subtract(final PolynomialFunction p)
		public virtual PolynomialFunction subtract(PolynomialFunction p)
		{
			// identify the lowest degree polynomial
			int lowLength = FastMath.min(coefficients.Length, p.coefficients.Length);
			int highLength = FastMath.max(coefficients.Length, p.coefficients.Length);

			// build the coefficients array
			double[] newCoefficients = new double[highLength];
			for (int i = 0; i < lowLength; ++i)
			{
				newCoefficients[i] = coefficients[i] - p.coefficients[i];
			}
			if (coefficients.Length < p.coefficients.Length)
			{
				for (int i = lowLength; i < highLength; ++i)
				{
					newCoefficients[i] = -p.coefficients[i];
				}
			}
			else
			{
				Array.Copy(coefficients, lowLength, newCoefficients, lowLength, highLength - lowLength);
			}

			return new PolynomialFunction(newCoefficients);
		}

		/// <summary>
		/// Negate the instance.
		/// </summary>
		/// <returns> a new polynomial. </returns>
		public virtual PolynomialFunction negate()
		{
			double[] newCoefficients = new double[coefficients.Length];
			for (int i = 0; i < coefficients.Length; ++i)
			{
				newCoefficients[i] = -coefficients[i];
			}
			return new PolynomialFunction(newCoefficients);
		}

		/// <summary>
		/// Multiply the instance by a polynomial.
		/// </summary>
		/// <param name="p"> Polynomial to multiply by. </param>
		/// <returns> a new polynomial. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PolynomialFunction multiply(final PolynomialFunction p)
		public virtual PolynomialFunction multiply(PolynomialFunction p)
		{
			double[] newCoefficients = new double[coefficients.Length + p.coefficients.Length - 1];

			for (int i = 0; i < newCoefficients.Length; ++i)
			{
				newCoefficients[i] = 0.0;
				for (int j = FastMath.max(0, i + 1 - p.coefficients.Length); j < FastMath.min(coefficients.Length, i + 1); ++j)
				{
					newCoefficients[i] += coefficients[j] * p.coefficients[i - j];
				}
			}

			return new PolynomialFunction(newCoefficients);
		}

		/// <summary>
		/// Returns the coefficients of the derivative of the polynomial with the given coefficients.
		/// </summary>
		/// <param name="coefficients"> Coefficients of the polynomial to differentiate. </param>
		/// <returns> the coefficients of the derivative or {@code null} if coefficients has length 1. </returns>
		/// <exception cref="NoDataException"> if {@code coefficients} is empty. </exception>
		/// <exception cref="NullArgumentException"> if {@code coefficients} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static double[] differentiate(double[] coefficients) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoDataException
		protected internal static double[] differentiate(double[] coefficients)
		{
			MathUtils.checkNotNull(coefficients);
			int n = coefficients.Length;
			if (n == 0)
			{
				throw new NoDataException(LocalizedFormats.EMPTY_POLYNOMIALS_COEFFICIENTS_ARRAY);
			}
			if (n == 1)
			{
				return new double[]{0};
			}
			double[] result = new double[n - 1];
			for (int i = n - 1; i > 0; i--)
			{
				result[i - 1] = i * coefficients[i];
			}
			return result;
		}

		/// <summary>
		/// Returns the derivative as a <seealso cref="PolynomialFunction"/>.
		/// </summary>
		/// <returns> the derivative polynomial. </returns>
		public virtual PolynomialFunction polynomialDerivative()
		{
			return new PolynomialFunction(differentiate(coefficients));
		}

		/// <summary>
		/// Returns the derivative as a <seealso cref="UnivariateFunction"/>.
		/// </summary>
		/// <returns> the derivative function. </returns>
		public virtual UnivariateFunction derivative()
		{
			return polynomialDerivative();
		}

		/// <summary>
		/// Returns a string representation of the polynomial.
		/// 
		/// <p>The representation is user oriented. Terms are displayed lowest
		/// degrees first. The multiplications signs, coefficients equals to
		/// one and null terms are not displayed (except if the polynomial is 0,
		/// in which case the 0 constant term is displayed). Addition of terms
		/// with negative coefficients are replaced by subtraction of terms
		/// with positive coefficients except for the first displayed term
		/// (i.e. we display <code>-3</code> for a constant negative polynomial,
		/// but <code>1 - 3 x + x^2</code> if the negative coefficient is not
		/// the first one displayed).</p>
		/// </summary>
		/// <returns> a string representation of the polynomial. </returns>
		public override string ToString()
		{
			StringBuilder s = new StringBuilder();
			if (coefficients[0] == 0.0)
			{
				if (coefficients.Length == 1)
				{
					return "0";
				}
			}
			else
			{
				s.Append(ToString(coefficients[0]));
			}

			for (int i = 1; i < coefficients.Length; ++i)
			{
				if (coefficients[i] != 0)
				{
					if (s.Length > 0)
					{
						if (coefficients[i] < 0)
						{
							s.Append(" - ");
						}
						else
						{
							s.Append(" + ");
						}
					}
					else
					{
						if (coefficients[i] < 0)
						{
							s.Append("-");
						}
					}

					double absAi = FastMath.abs(coefficients[i]);
					if ((absAi - 1) != 0)
					{
						s.Append(ToString(absAi));
						s.Append(' ');
					}

					s.Append("x");
					if (i > 1)
					{
						s.Append('^');
						s.Append(Convert.ToString(i));
					}
				}
			}

			return s.ToString();
		}

		/// <summary>
		/// Creates a string representing a coefficient, removing ".0" endings.
		/// </summary>
		/// <param name="coeff"> Coefficient. </param>
		/// <returns> a string representation of {@code coeff}. </returns>
		private static string ToString(double coeff)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String c = Double.toString(coeff);
			string c = Convert.ToString(coeff);
			if (c.EndsWith(".0"))
			{
				return c.Substring(0, c.Length - 2);
			}
			else
			{
				return c;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + Arrays.GetHashCode(coefficients);
			return result;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is PolynomialFunction))
			{
				return false;
			}
			PolynomialFunction other = (PolynomialFunction) obj;
			if (!Arrays.Equals(coefficients, other.coefficients))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Dedicated parametric polynomial class.
		/// 
		/// @since 3.0
		/// </summary>
		public class Parametric : ParametricUnivariateFunction
		{
			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual double[] gradient(double x, params double[] parameters)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] gradient = new double[parameters.length];
				double[] gradient = new double[parameters.Length];
				double xn = 1.0;
				for (int i = 0; i < parameters.Length; ++i)
				{
					gradient[i] = xn;
					xn *= x;
				}
				return gradient;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double value(final double x, final double... parameters) throws org.apache.commons.math3.exception.NoDataException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			public virtual double value(double x, params double[] parameters)
			{
				return PolynomialFunction.evaluate(parameters, x);
			}
		}
	}

}
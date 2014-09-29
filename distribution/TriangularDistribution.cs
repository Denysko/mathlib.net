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

namespace mathlib.distribution
{

	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using FastMath = mathlib.util.FastMath;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;

	/// <summary>
	/// Implementation of the triangular real distribution.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Triangular_distribution">
	/// Triangular distribution (Wikipedia)</a>
	/// 
	/// @version $Id: TriangularDistribution.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0 </seealso>
	public class TriangularDistribution : AbstractRealDistribution
	{
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 20120112L;
		/// <summary>
		/// Lower limit of this distribution (inclusive). </summary>
		private readonly double a;
		/// <summary>
		/// Upper limit of this distribution (inclusive). </summary>
		private readonly double b;
		/// <summary>
		/// Mode of this distribution. </summary>
		private readonly double c;
		/// <summary>
		/// Inverse cumulative probability accuracy. </summary>
		private readonly double solverAbsoluteAccuracy;

		/// <summary>
		/// Creates a triangular real distribution using the given lower limit,
		/// upper limit, and mode.
		/// </summary>
		/// <param name="a"> Lower limit of this distribution (inclusive). </param>
		/// <param name="b"> Upper limit of this distribution (inclusive). </param>
		/// <param name="c"> Mode of this distribution. </param>
		/// <exception cref="NumberIsTooLargeException"> if {@code a >= b} or if {@code c > b}. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code c < a}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TriangularDistribution(double a, double c, double b) throws mathlib.exception.NumberIsTooLargeException, mathlib.exception.NumberIsTooSmallException
		public TriangularDistribution(double a, double c, double b) : this(new Well19937c(), a, c, b)
		{
		}

		/// <summary>
		/// Creates a triangular distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="a"> Lower limit of this distribution (inclusive). </param>
		/// <param name="b"> Upper limit of this distribution (inclusive). </param>
		/// <param name="c"> Mode of this distribution. </param>
		/// <exception cref="NumberIsTooLargeException"> if {@code a >= b} or if {@code c > b}. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code c < a}.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TriangularDistribution(mathlib.random.RandomGenerator rng, double a, double c, double b) throws mathlib.exception.NumberIsTooLargeException, mathlib.exception.NumberIsTooSmallException
		public TriangularDistribution(RandomGenerator rng, double a, double c, double b) : base(rng)
		{

			if (a >= b)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.LOWER_BOUND_NOT_BELOW_UPPER_BOUND, a, b, false);
			}
			if (c < a)
			{
				throw new NumberIsTooSmallException(LocalizedFormats.NUMBER_TOO_SMALL, c, a, true);
			}
			if (c > b)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.NUMBER_TOO_LARGE, c, b, true);
			}

			this.a = a;
			this.c = c;
			this.b = b;
			solverAbsoluteAccuracy = FastMath.max(FastMath.ulp(a), FastMath.ulp(b));
		}

		/// <summary>
		/// Returns the mode {@code c} of this distribution.
		/// </summary>
		/// <returns> the mode {@code c} of this distribution </returns>
		public virtual double Mode
		{
			get
			{
				return c;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <p>
		/// For this distribution, the returned value is not really meaningful,
		/// since exact formulas are implemented for the computation of the
		/// <seealso cref="#inverseCumulativeProbability(double)"/> (no solver is invoked).
		/// </p>
		/// <p>
		/// For lower limit {@code a} and upper limit {@code b}, the current
		/// implementation returns {@code max(ulp(a), ulp(b)}.
		/// </p>
		/// </summary>
		protected internal override double SolverAbsoluteAccuracy
		{
			get
			{
				return solverAbsoluteAccuracy;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For lower limit {@code a}, upper limit {@code b} and mode {@code c}, the
		/// PDF is given by
		/// <ul>
		/// <li>{@code 2 * (x - a) / [(b - a) * (c - a)]} if {@code a <= x < c},</li>
		/// <li>{@code 2 / (b - a)} if {@code x = c},</li>
		/// <li>{@code 2 * (b - x) / [(b - a) * (b - c)]} if {@code c < x <= b},</li>
		/// <li>{@code 0} otherwise.
		/// </ul>
		/// </summary>
		public override double density(double x)
		{
			if (x < a)
			{
				return 0;
			}
			if (a <= x && x < c)
			{
				double divident = 2 * (x - a);
				double divisor = (b - a) * (c - a);
				return divident / divisor;
			}
			if (x == c)
			{
				return 2 / (b - a);
			}
			if (c < x && x <= b)
			{
				double divident = 2 * (b - x);
				double divisor = (b - a) * (b - c);
				return divident / divisor;
			}
			return 0;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For lower limit {@code a}, upper limit {@code b} and mode {@code c}, the
		/// CDF is given by
		/// <ul>
		/// <li>{@code 0} if {@code x < a},</li>
		/// <li>{@code (x - a)^2 / [(b - a) * (c - a)]} if {@code a <= x < c},</li>
		/// <li>{@code (c - a) / (b - a)} if {@code x = c},</li>
		/// <li>{@code 1 - (b - x)^2 / [(b - a) * (b - c)]} if {@code c < x <= b},</li>
		/// <li>{@code 1} if {@code x > b}.</li>
		/// </ul>
		/// </summary>
		public override double cumulativeProbability(double x)
		{
			if (x < a)
			{
				return 0;
			}
			if (a <= x && x < c)
			{
				double divident = (x - a) * (x - a);
				double divisor = (b - a) * (c - a);
				return divident / divisor;
			}
			if (x == c)
			{
				return (c - a) / (b - a);
			}
			if (c < x && x <= b)
			{
				double divident = (b - x) * (b - x);
				double divisor = (b - a) * (b - c);
				return 1 - (divident / divisor);
			}
			return 1;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For lower limit {@code a}, upper limit {@code b}, and mode {@code c},
		/// the mean is {@code (a + b + c) / 3}.
		/// </summary>
		public override double NumericalMean
		{
			get
			{
				return (a + b + c) / 3;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For lower limit {@code a}, upper limit {@code b}, and mode {@code c},
		/// the variance is {@code (a^2 + b^2 + c^2 - a * b - a * c - b * c) / 18}.
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
				return (a * a + b * b + c * c - a * b - a * c - b * c) / 18;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The lower bound of the support is equal to the lower limit parameter
		/// {@code a} of the distribution.
		/// </summary>
		/// <returns> lower bound of the support </returns>
		public override double SupportLowerBound
		{
			get
			{
				return a;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The upper bound of the support is equal to the upper limit parameter
		/// {@code b} of the distribution.
		/// </summary>
		/// <returns> upper bound of the support </returns>
		public override double SupportUpperBound
		{
			get
			{
				return b;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool SupportLowerBoundInclusive
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool SupportUpperBoundInclusive
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The support of this distribution is connected.
		/// </summary>
		/// <returns> {@code true} </returns>
		public override bool SupportConnected
		{
			get
			{
				return true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double inverseCumulativeProbability(double p) throws mathlib.exception.OutOfRangeException
		public override double inverseCumulativeProbability(double p)
		{
			if (p < 0 || p > 1)
			{
				throw new OutOfRangeException(p, 0, 1);
			}
			if (p == 0)
			{
				return a;
			}
			if (p == 1)
			{
				return b;
			}
			if (p < (c - a) / (b - a))
			{
				return a + FastMath.sqrt(p * (b - a) * (c - a));
			}
			return b - FastMath.sqrt((1 - p) * (b - a) * (b - c));
		}
	}

}
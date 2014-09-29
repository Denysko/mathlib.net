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

	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using FastMath = mathlib.util.FastMath;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;

	/// <summary>
	/// Implementation of the Zipf distribution.
	/// </summary>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/ZipfDistribution.html">Zipf distribution (MathWorld)</a>
	/// @version $Id: ZipfDistribution.java 1533974 2013-10-20 20:42:41Z psteitz $ </seealso>
	public class ZipfDistribution : AbstractIntegerDistribution
	{
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -140627372283420404L;
		/// <summary>
		/// Number of elements. </summary>
		private readonly int numberOfElements;
		/// <summary>
		/// Exponent parameter of the distribution. </summary>
		private readonly double exponent;
		/// <summary>
		/// Cached numerical mean </summary>
		private double numericalMean = double.NaN;
		/// <summary>
		/// Whether or not the numerical mean has been calculated </summary>
		private bool numericalMeanIsCalculated = false;
		/// <summary>
		/// Cached numerical variance </summary>
		private double numericalVariance = double.NaN;
		/// <summary>
		/// Whether or not the numerical variance has been calculated </summary>
		private bool numericalVarianceIsCalculated = false;

		/// <summary>
		/// Create a new Zipf distribution with the given number of elements and
		/// exponent.
		/// </summary>
		/// <param name="numberOfElements"> Number of elements. </param>
		/// <param name="exponent"> Exponent. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code numberOfElements <= 0}
		/// or {@code exponent <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ZipfDistribution(final int numberOfElements, final double exponent)
		public ZipfDistribution(int numberOfElements, double exponent) : this(new Well19937c(), numberOfElements, exponent)
		{
		}

		/// <summary>
		/// Creates a Zipf distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="numberOfElements"> Number of elements. </param>
		/// <param name="exponent"> Exponent. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code numberOfElements <= 0}
		/// or {@code exponent <= 0}.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ZipfDistribution(mathlib.random.RandomGenerator rng, int numberOfElements, double exponent) throws mathlib.exception.NotStrictlyPositiveException
		public ZipfDistribution(RandomGenerator rng, int numberOfElements, double exponent) : base(rng)
		{

			if (numberOfElements <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.DIMENSION, numberOfElements);
			}
			if (exponent <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.EXPONENT, exponent);
			}

			this.numberOfElements = numberOfElements;
			this.exponent = exponent;
		}

		/// <summary>
		/// Get the number of elements (e.g. corpus size) for the distribution.
		/// </summary>
		/// <returns> the number of elements </returns>
		public virtual int NumberOfElements
		{
			get
			{
				return numberOfElements;
			}
		}

		/// <summary>
		/// Get the exponent characterizing the distribution.
		/// </summary>
		/// <returns> the exponent </returns>
		public virtual double Exponent
		{
			get
			{
				return exponent;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double probability(final int x)
		public override double probability(int x)
		{
			if (x <= 0 || x > numberOfElements)
			{
				return 0.0;
			}

			return (1.0 / FastMath.pow(x, exponent)) / generalizedHarmonic(numberOfElements, exponent);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double logProbability(int x)
		{
			if (x <= 0 || x > numberOfElements)
			{
				return double.NegativeInfinity;
			}

			return -FastMath.log(x) * exponent - FastMath.log(generalizedHarmonic(numberOfElements, exponent));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double cumulativeProbability(final int x)
		public override double cumulativeProbability(int x)
		{
			if (x <= 0)
			{
				return 0.0;
			}
			else if (x >= numberOfElements)
			{
				return 1.0;
			}

			return generalizedHarmonic(x, exponent) / generalizedHarmonic(numberOfElements, exponent);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For number of elements {@code N} and exponent {@code s}, the mean is
		/// {@code Hs1 / Hs}, where
		/// <ul>
		///  <li>{@code Hs1 = generalizedHarmonic(N, s - 1)},</li>
		///  <li>{@code Hs = generalizedHarmonic(N, s)}.</li>
		/// </ul>
		/// </summary>
		public override double NumericalMean
		{
			get
			{
				if (!numericalMeanIsCalculated)
				{
					numericalMean = calculateNumericalMean();
					numericalMeanIsCalculated = true;
				}
				return numericalMean;
			}
		}

		/// <summary>
		/// Used by <seealso cref="#getNumericalMean()"/>.
		/// </summary>
		/// <returns> the mean of this distribution </returns>
		protected internal virtual double calculateNumericalMean()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = getNumberOfElements();
			int N = NumberOfElements;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s = getExponent();
			double s = Exponent;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double Hs1 = generalizedHarmonic(N, s - 1);
			double Hs1 = generalizedHarmonic(N, s - 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double Hs = generalizedHarmonic(N, s);
			double Hs = generalizedHarmonic(N, s);

			return Hs1 / Hs;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For number of elements {@code N} and exponent {@code s}, the mean is
		/// {@code (Hs2 / Hs) - (Hs1^2 / Hs^2)}, where
		/// <ul>
		///  <li>{@code Hs2 = generalizedHarmonic(N, s - 2)},</li>
		///  <li>{@code Hs1 = generalizedHarmonic(N, s - 1)},</li>
		///  <li>{@code Hs = generalizedHarmonic(N, s)}.</li>
		/// </ul>
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
				if (!numericalVarianceIsCalculated)
				{
					numericalVariance = calculateNumericalVariance();
					numericalVarianceIsCalculated = true;
				}
				return numericalVariance;
			}
		}

		/// <summary>
		/// Used by <seealso cref="#getNumericalVariance()"/>.
		/// </summary>
		/// <returns> the variance of this distribution </returns>
		protected internal virtual double calculateNumericalVariance()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = getNumberOfElements();
			int N = NumberOfElements;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s = getExponent();
			double s = Exponent;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double Hs2 = generalizedHarmonic(N, s - 2);
			double Hs2 = generalizedHarmonic(N, s - 2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double Hs1 = generalizedHarmonic(N, s - 1);
			double Hs1 = generalizedHarmonic(N, s - 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double Hs = generalizedHarmonic(N, s);
			double Hs = generalizedHarmonic(N, s);

			return (Hs2 / Hs) - ((Hs1 * Hs1) / (Hs * Hs));
		}

		/// <summary>
		/// Calculates the Nth generalized harmonic number. See
		/// <a href="http://mathworld.wolfram.com/HarmonicSeries.html">Harmonic
		/// Series</a>.
		/// </summary>
		/// <param name="n"> Term in the series to calculate (must be larger than 1) </param>
		/// <param name="m"> Exponent (special case {@code m = 1} is the harmonic series). </param>
		/// <returns> the n<sup>th</sup> generalized harmonic number. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double generalizedHarmonic(final int n, final double m)
		private double generalizedHarmonic(int n, double m)
		{
			double value = 0;
			for (int k = n; k > 0; --k)
			{
				value += 1.0 / FastMath.pow(k, m);
			}
			return value;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The lower bound of the support is always 1 no matter the parameters.
		/// </summary>
		/// <returns> lower bound of the support (always 1) </returns>
		public override int SupportLowerBound
		{
			get
			{
				return 1;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The upper bound of the support is the number of elements.
		/// </summary>
		/// <returns> upper bound of the support </returns>
		public override int SupportUpperBound
		{
			get
			{
				return NumberOfElements;
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
	}


}
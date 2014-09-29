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
namespace mathlib.distribution
{


	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MathArithmeticException = mathlib.exception.MathArithmeticException;
	using NotANumberException = mathlib.exception.NotANumberException;
	using NotFiniteNumberException = mathlib.exception.NotFiniteNumberException;
	using NotPositiveException = mathlib.exception.NotPositiveException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;
	using mathlib.util;

	/// <summary>
	/// <p>Implementation of a real-valued <seealso cref="EnumeratedDistribution"/>.
	/// 
	/// <p>Values with zero-probability are allowed but they do not extend the
	/// support.<br/>
	/// Duplicate values are allowed. Probabilities of duplicate values are combined
	/// when computing cumulative probabilities and statistics.</p>
	/// 
	/// @version $Id: EnumeratedRealDistribution.java 1566274 2014-02-09 11:21:28Z tn $
	/// @since 3.2
	/// </summary>
	public class EnumeratedRealDistribution : AbstractRealDistribution
	{

		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 20130308L;

		/// <summary>
		/// <seealso cref="EnumeratedDistribution"/> (using the <seealso cref="Double"/> wrapper)
		/// used to generate the pmf.
		/// </summary>
		protected internal readonly EnumeratedDistribution<double?> innerDistribution;

		/// <summary>
		/// Create a discrete distribution using the given probability mass function
		/// enumeration.
		/// </summary>
		/// <param name="singletons"> array of random variable values. </param>
		/// <param name="probabilities"> array of probabilities. </param>
		/// <exception cref="DimensionMismatchException"> if
		/// {@code singletons.length != probabilities.length} </exception>
		/// <exception cref="NotPositiveException"> if any of the probabilities are negative. </exception>
		/// <exception cref="NotFiniteNumberException"> if any of the probabilities are infinite. </exception>
		/// <exception cref="NotANumberException"> if any of the probabilities are NaN. </exception>
		/// <exception cref="MathArithmeticException"> all of the probabilities are 0. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public EnumeratedRealDistribution(final double[] singletons, final double[] probabilities) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NotPositiveException, mathlib.exception.MathArithmeticException, mathlib.exception.NotFiniteNumberException, mathlib.exception.NotANumberException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public EnumeratedRealDistribution(double[] singletons, double[] probabilities) : this(new Well19937c(), singletons, probabilities)
		{
		}

		/// <summary>
		/// Create a discrete distribution using the given random number generator
		/// and probability mass function enumeration.
		/// </summary>
		/// <param name="rng"> random number generator. </param>
		/// <param name="singletons"> array of random variable values. </param>
		/// <param name="probabilities"> array of probabilities. </param>
		/// <exception cref="DimensionMismatchException"> if
		/// {@code singletons.length != probabilities.length} </exception>
		/// <exception cref="NotPositiveException"> if any of the probabilities are negative. </exception>
		/// <exception cref="NotFiniteNumberException"> if any of the probabilities are infinite. </exception>
		/// <exception cref="NotANumberException"> if any of the probabilities are NaN. </exception>
		/// <exception cref="MathArithmeticException"> all of the probabilities are 0. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public EnumeratedRealDistribution(final mathlib.random.RandomGenerator rng, final double[] singletons, final double[] probabilities) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NotPositiveException, mathlib.exception.MathArithmeticException, mathlib.exception.NotFiniteNumberException, mathlib.exception.NotANumberException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public EnumeratedRealDistribution(RandomGenerator rng, double[] singletons, double[] probabilities) : base(rng)
		{
			if (singletons.Length != probabilities.Length)
			{
				throw new DimensionMismatchException(probabilities.Length, singletons.Length);
			}

			IList<Pair<double?, double?>> samples = new List<Pair<double?, double?>>(singletons.Length);

			for (int i = 0; i < singletons.Length; i++)
			{
				samples.Add(new Pair<double?, double?>(singletons[i], probabilities[i]));
			}

			innerDistribution = new EnumeratedDistribution<double?>(rng, samples);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public double probability(final double x)
		public override double probability(double x)
		{
			return innerDistribution.probability(x);
		}

		/// <summary>
		/// For a random variable {@code X} whose values are distributed according to
		/// this distribution, this method returns {@code P(X = x)}. In other words,
		/// this method represents the probability mass function (PMF) for the
		/// distribution.
		/// </summary>
		/// <param name="x"> the point at which the PMF is evaluated </param>
		/// <returns> the value of the probability mass function at point {@code x} </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double density(final double x)
		public override double density(double x)
		{
			return probability(x);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double cumulativeProbability(final double x)
		public override double cumulativeProbability(double x)
		{
			double probability = 0;

			foreach (Pair<double?, double?> sample in innerDistribution.Pmf)
			{
				if (sample.Key <= x)
				{
					probability += sample.Value;
				}
			}

			return probability;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double inverseCumulativeProbability(final double p) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double inverseCumulativeProbability(double p)
		{
			if (p < 0.0 || p > 1.0)
			{
				throw new OutOfRangeException(p, 0, 1);
			}

			double probability = 0;
			double x = SupportLowerBound;
			foreach (Pair<double?, double?> sample in innerDistribution.Pmf)
			{
				if (sample.Value == 0.0)
				{
					continue;
				}

				probability += sample.Value;
				x = sample.Key;

				if (probability >= p)
				{
					break;
				}
			}

			return x;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <returns> {@code sum(singletons[i] * probabilities[i])} </returns>
		public override double NumericalMean
		{
			get
			{
				double mean = 0;
    
				foreach (Pair<double?, double?> sample in innerDistribution.Pmf)
				{
					mean += sample.Value * sample.Key;
				}
    
				return mean;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <returns> {@code sum((singletons[i] - mean) ^ 2 * probabilities[i])} </returns>
		public override double NumericalVariance
		{
			get
			{
				double mean = 0;
				double meanOfSquares = 0;
    
				foreach (Pair<double?, double?> sample in innerDistribution.Pmf)
				{
					mean += sample.Value * sample.Key;
					meanOfSquares += sample.Value * sample.Key * sample.Key;
				}
    
				return meanOfSquares - mean * mean;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// Returns the lowest value with non-zero probability.
		/// </summary>
		/// <returns> the lowest value with non-zero probability. </returns>
		public override double SupportLowerBound
		{
			get
			{
				double min = double.PositiveInfinity;
				foreach (Pair<double?, double?> sample in innerDistribution.Pmf)
				{
					if (sample.Key < min && sample.Value > 0)
					{
						min = sample.Key;
					}
				}
    
				return min;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// Returns the highest value with non-zero probability.
		/// </summary>
		/// <returns> the highest value with non-zero probability. </returns>
		public override double SupportUpperBound
		{
			get
			{
				double max = double.NegativeInfinity;
				foreach (Pair<double?, double?> sample in innerDistribution.Pmf)
				{
					if (sample.Key > max && sample.Value > 0)
					{
						max = sample.Key;
					}
				}
    
				return max;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The support of this distribution includes the lower bound.
		/// </summary>
		/// <returns> {@code true} </returns>
		public override bool SupportLowerBoundInclusive
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The support of this distribution includes the upper bound.
		/// </summary>
		/// <returns> {@code true} </returns>
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

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double sample()
		{
			return innerDistribution.sample();
		}
	}

}
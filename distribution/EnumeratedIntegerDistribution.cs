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
	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;
	using mathlib.util;

	/// <summary>
	/// <p>Implementation of an integer-valued <seealso cref="EnumeratedDistribution"/>.</p>
	/// 
	/// <p>Values with zero-probability are allowed but they do not extend the
	/// support.<br/>
	/// Duplicate values are allowed. Probabilities of duplicate values are combined
	/// when computing cumulative probabilities and statistics.</p>
	/// 
	/// @version $Id: EnumeratedIntegerDistribution.java 1456769 2013-03-15 04:51:34Z psteitz $
	/// @since 3.2
	/// </summary>
	public class EnumeratedIntegerDistribution : AbstractIntegerDistribution
	{

		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 20130308L;

		/// <summary>
		/// <seealso cref="EnumeratedDistribution"/> instance (using the <seealso cref="Integer"/> wrapper)
		/// used to generate the pmf.
		/// </summary>
		protected internal readonly EnumeratedDistribution<int?> innerDistribution;

		/// <summary>
		/// Create a discrete distribution using the given probability mass function
		/// definition.
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
//ORIGINAL LINE: public EnumeratedIntegerDistribution(final int[] singletons, final double[] probabilities) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NotPositiveException, mathlib.exception.MathArithmeticException, mathlib.exception.NotFiniteNumberException, mathlib.exception.NotANumberException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public EnumeratedIntegerDistribution(int[] singletons, double[] probabilities) : this(new Well19937c(), singletons, probabilities)
		{
		}

		/// <summary>
		/// Create a discrete distribution using the given random number generator
		/// and probability mass function definition.
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
//ORIGINAL LINE: public EnumeratedIntegerDistribution(final mathlib.random.RandomGenerator rng, final int[] singletons, final double[] probabilities) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NotPositiveException, mathlib.exception.MathArithmeticException, mathlib.exception.NotFiniteNumberException, mathlib.exception.NotANumberException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public EnumeratedIntegerDistribution(RandomGenerator rng, int[] singletons, double[] probabilities) : base(rng)
		{
			if (singletons.Length != probabilities.Length)
			{
				throw new DimensionMismatchException(probabilities.Length, singletons.Length);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<mathlib.util.Pair<Integer, Double>> samples = new java.util.ArrayList<mathlib.util.Pair<Integer, Double>>(singletons.length);
			IList<Pair<int?, double?>> samples = new List<Pair<int?, double?>>(singletons.Length);

			for (int i = 0; i < singletons.Length; i++)
			{
				samples.Add(new Pair<int?, double?>(singletons[i], probabilities[i]));
			}

			innerDistribution = new EnumeratedDistribution<int?>(rng, samples);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double probability(final int x)
		public override double probability(int x)
		{
			return innerDistribution.probability(x);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double cumulativeProbability(final int x)
		public override double cumulativeProbability(int x)
		{
			double probability = 0;

			foreach (Pair<int?, double?> sample in innerDistribution.Pmf)
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
		/// <returns> {@code sum(singletons[i] * probabilities[i])} </returns>
		public override double NumericalMean
		{
			get
			{
				double mean = 0;
    
				foreach (Pair<int?, double?> sample in innerDistribution.Pmf)
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
    
				foreach (Pair<int?, double?> sample in innerDistribution.Pmf)
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
		public override int SupportLowerBound
		{
			get
			{
				int min = int.MaxValue;
				foreach (Pair<int?, double?> sample in innerDistribution.Pmf)
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
		public override int SupportUpperBound
		{
			get
			{
				int max = int.MinValue;
				foreach (Pair<int?, double?> sample in innerDistribution.Pmf)
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
		public override int sample()
		{
			return innerDistribution.sample();
		}
	}

}
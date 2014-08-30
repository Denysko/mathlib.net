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
namespace org.apache.commons.math3.distribution
{

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using MathArithmeticException = org.apache.commons.math3.exception.MathArithmeticException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using RandomGenerator = org.apache.commons.math3.random.RandomGenerator;
	using Well19937c = org.apache.commons.math3.random.Well19937c;
	using org.apache.commons.math3.util;

	/// <summary>
	/// Class for representing <a href="http://en.wikipedia.org/wiki/Mixture_model">
	/// mixture model</a> distributions.
	/// </summary>
	/// @param <T> Type of the mixture components.
	/// 
	/// @version $Id: MixtureMultivariateRealDistribution.java 1517418 2013-08-26 03:18:55Z dbrosius $
	/// @since 3.1 </param>
	public class MixtureMultivariateRealDistribution<T> : AbstractMultivariateRealDistribution where T : MultivariateRealDistribution
	{
		/// <summary>
		/// Normalized weight of each mixture component. </summary>
		private readonly double[] weight;
		/// <summary>
		/// Mixture components. </summary>
		private readonly IList<T> distribution;

		/// <summary>
		/// Creates a mixture model from a list of distributions and their
		/// associated weights.
		/// </summary>
		/// <param name="components"> List of (weight, distribution) pairs from which to sample. </param>
		public MixtureMultivariateRealDistribution(IList<Pair<double?, T>> components) : this(new Well19937c(), components)
		{
		}

		/// <summary>
		/// Creates a mixture model from a list of distributions and their
		/// associated weights.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="components"> Distributions from which to sample. </param>
		/// <exception cref="NotPositiveException"> if any of the weights is negative. </exception>
		/// <exception cref="DimensionMismatchException"> if not all components have the same
		/// number of variables. </exception>
		public MixtureMultivariateRealDistribution(RandomGenerator rng, IList<Pair<double?, T>> components) : base(rng, components[0].Second.Dimension)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numComp = components.size();
			int numComp = components.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = getDimension();
			int dim = Dimension;
			double weightSum = 0;
			for (int i = 0; i < numComp; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.util.Pair<Double, T> comp = components.get(i);
				Pair<double?, T> comp = components[i];
				if (comp.Second.Dimension != dim)
				{
					throw new DimensionMismatchException(comp.Second.Dimension, dim);
				}
				if (comp.First < 0)
				{
					throw new NotPositiveException(comp.First);
				}
				weightSum += comp.First;
			}

			// Check for overflow.
			if (double.IsInfinity(weightSum))
			{
				throw new MathArithmeticException(LocalizedFormats.OVERFLOW);
			}

			// Store each distribution and its normalized weight.
			distribution = new List<T>();
			weight = new double[numComp];
			for (int i = 0; i < numComp; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.util.Pair<Double, T> comp = components.get(i);
				Pair<double?, T> comp = components[i];
				weight[i] = comp.First / weightSum;
				distribution.Add(comp.Second);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double density(final double[] values)
		public override double density(double[] values)
		{
			double p = 0;
			for (int i = 0; i < weight.Length; i++)
			{
				p += weight[i] * distribution[i].density(values);
			}
			return p;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double[] sample()
		{
			// Sampled values.
			double[] vals = null;

			// Determine which component to sample from.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double randomValue = random.nextDouble();
			double randomValue = random.NextDouble();
			double sum = 0;

			for (int i = 0; i < weight.Length; i++)
			{
				sum += weight[i];
				if (randomValue <= sum)
				{
					// pick model i
					vals = distribution[i].sample();
					break;
				}
			}

			if (vals == null)
			{
				// This should never happen, but it ensures we won't return a null in
				// case the loop above has some floating point inequality problem on
				// the final iteration.
				vals = distribution[weight.Length - 1].sample();
			}

			return vals;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override void reseedRandomGenerator(long seed)
		{
			// Seed needs to be propagated to underlying components
			// in order to maintain consistency between runs.
			base.reseedRandomGenerator(seed);

			for (int i = 0; i < distribution.Count; i++)
			{
				// Make each component's seed different in order to avoid
				// using the same sequence of random numbers.
				distribution[i].reseedRandomGenerator(i + 1 + seed);
			}
		}

		/// <summary>
		/// Gets the distributions that make up the mixture model.
		/// </summary>
		/// <returns> the component distributions and associated weights. </returns>
		public virtual IList<Pair<double?, T>> Components
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.List<org.apache.commons.math3.util.Pair<Double, T>> list = new java.util.ArrayList<org.apache.commons.math3.util.Pair<Double, T>>(weight.length);
				IList<Pair<double?, T>> list = new List<Pair<double?, T>>(weight.Length);
    
				for (int i = 0; i < weight.Length; i++)
				{
					list.Add(new Pair<double?, T>(weight[i], distribution[i]));
				}
    
				return list;
			}
		}
	}

}
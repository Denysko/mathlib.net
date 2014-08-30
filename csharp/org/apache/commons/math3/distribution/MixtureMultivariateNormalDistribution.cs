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
	using RandomGenerator = org.apache.commons.math3.random.RandomGenerator;
	using org.apache.commons.math3.util;

	/// <summary>
	/// Multivariate normal mixture distribution.
	/// This class is mainly syntactic sugar.
	/// </summary>
	/// <seealso cref= MixtureMultivariateRealDistribution
	/// @version $Id: MixtureMultivariateNormalDistribution.java 1517418 2013-08-26 03:18:55Z dbrosius $
	/// @since 3.2 </seealso>
	public class MixtureMultivariateNormalDistribution : MixtureMultivariateRealDistribution<MultivariateNormalDistribution>
	{
		/// <summary>
		/// Creates a multivariate normal mixture distribution.
		/// </summary>
		/// <param name="weights"> Weights of each component. </param>
		/// <param name="means"> Mean vector for each component. </param>
		/// <param name="covariances"> Covariance matrix for each component. </param>
		public MixtureMultivariateNormalDistribution(double[] weights, double[][] means, double[][][] covariances) : base(createComponents(weights, means, covariances))
		{
		}

		/// <summary>
		/// Creates a mixture model from a list of distributions and their
		/// associated weights.
		/// </summary>
		/// <param name="components"> List of (weight, distribution) pairs from which to sample. </param>
		public MixtureMultivariateNormalDistribution(IList<Pair<double?, MultivariateNormalDistribution>> components) : base(components)
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MixtureMultivariateNormalDistribution(org.apache.commons.math3.random.RandomGenerator rng, java.util.List<org.apache.commons.math3.util.Pair<Double, MultivariateNormalDistribution>> components) throws org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.DimensionMismatchException
		public MixtureMultivariateNormalDistribution(RandomGenerator rng, IList<Pair<double?, MultivariateNormalDistribution>> components) : base(rng, components)
		{
		}

		/// <param name="weights"> Weights of each component. </param>
		/// <param name="means"> Mean vector for each component. </param>
		/// <param name="covariances"> Covariance matrix for each component. </param>
		/// <returns> the list of components. </returns>
		private static IList<Pair<double?, MultivariateNormalDistribution>> createComponents(double[] weights, double[][] means, double[][][] covariances)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.apache.commons.math3.util.Pair<Double, MultivariateNormalDistribution>> mvns = new java.util.ArrayList<org.apache.commons.math3.util.Pair<Double, MultivariateNormalDistribution>>(weights.length);
			IList<Pair<double?, MultivariateNormalDistribution>> mvns = new List<Pair<double?, MultivariateNormalDistribution>>(weights.Length);

			for (int i = 0; i < weights.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MultivariateNormalDistribution dist = new MultivariateNormalDistribution(means[i], covariances[i]);
				MultivariateNormalDistribution dist = new MultivariateNormalDistribution(means[i], covariances[i]);

				mvns.Add(new Pair<double?, MultivariateNormalDistribution>(weights[i], dist));
			}

			return mvns;
		}
	}

}
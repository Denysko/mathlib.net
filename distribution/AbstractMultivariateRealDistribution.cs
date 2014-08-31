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

	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using RandomGenerator = org.apache.commons.math3.random.RandomGenerator;

	/// <summary>
	/// Base class for multivariate probability distributions.
	/// 
	/// @version $Id: AbstractMultivariateRealDistribution.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.1
	/// </summary>
	public abstract class AbstractMultivariateRealDistribution : MultivariateRealDistribution
	{
		public abstract double density(double[] x);
		/// <summary>
		/// RNG instance used to generate samples from the distribution. </summary>
		protected internal readonly RandomGenerator random;
		/// <summary>
		/// The number of dimensions or columns in the multivariate distribution. </summary>
		private readonly int dimension;

		/// <param name="rng"> Random number generator. </param>
		/// <param name="n"> Number of dimensions. </param>
		protected internal AbstractMultivariateRealDistribution(RandomGenerator rng, int n)
		{
			random = rng;
			dimension = n;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual void reseedRandomGenerator(long seed)
		{
			random.Seed = seed;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int Dimension
		{
			get
			{
				return dimension;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public abstract double[] sample();

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double[][] sample(final int sampleSize)
		public virtual double[][] sample(int sampleSize)
		{
			if (sampleSize <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.NUMBER_OF_SAMPLES, sampleSize);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] out = new double[sampleSize][dimension];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] out = new double[sampleSize][dimension];
			double[][] @out = RectangularArrays.ReturnRectangularDoubleArray(sampleSize, dimension);
			for (int i = 0; i < sampleSize; i++)
			{
				@out[i] = sample();
			}
			return @out;
		}
	}

}
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

namespace mathlib.random
{

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;

	/// <summary>
	/// A <seealso cref="RandomVectorGenerator"/> that generates vectors with uncorrelated
	/// components. Components of generated vectors follow (independent) Gaussian
	/// distributions, with parameters supplied in the constructor.
	/// 
	/// @version $Id: UncorrelatedRandomVectorGenerator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2
	/// </summary>

	public class UncorrelatedRandomVectorGenerator : RandomVectorGenerator
	{

		/// <summary>
		/// Underlying scalar generator. </summary>
		private readonly NormalizedRandomGenerator generator;

		/// <summary>
		/// Mean vector. </summary>
		private readonly double[] mean;

		/// <summary>
		/// Standard deviation vector. </summary>
		private readonly double[] standardDeviation;

	  /// <summary>
	  /// Simple constructor.
	  /// <p>Build an uncorrelated random vector generator from
	  /// its mean and standard deviation vectors.</p> </summary>
	  /// <param name="mean"> expected mean values for each component </param>
	  /// <param name="standardDeviation"> standard deviation for each component </param>
	  /// <param name="generator"> underlying generator for uncorrelated normalized
	  /// components </param>
	  public UncorrelatedRandomVectorGenerator(double[] mean, double[] standardDeviation, NormalizedRandomGenerator generator)
	  {
		if (mean.Length != standardDeviation.Length)
		{
			throw new DimensionMismatchException(mean.Length, standardDeviation.Length);
		}
		this.mean = mean.clone();
		this.standardDeviation = standardDeviation.clone();
		this.generator = generator;
	  }

	  /// <summary>
	  /// Simple constructor.
	  /// <p>Build a null mean random and unit standard deviation
	  /// uncorrelated vector generator</p> </summary>
	  /// <param name="dimension"> dimension of the vectors to generate </param>
	  /// <param name="generator"> underlying generator for uncorrelated normalized
	  /// components </param>
	  public UncorrelatedRandomVectorGenerator(int dimension, NormalizedRandomGenerator generator)
	  {
		mean = new double[dimension];
		standardDeviation = new double[dimension];
		Arrays.fill(standardDeviation, 1.0);
		this.generator = generator;
	  }

	  /// <summary>
	  /// Generate an uncorrelated random vector. </summary>
	  /// <returns> a random vector as a newly built array of double </returns>
	  public virtual double[] nextVector()
	  {

		double[] random = new double[mean.Length];
		for (int i = 0; i < random.Length; ++i)
		{
		  random[i] = mean[i] + standardDeviation[i] * generator.nextNormalizedDouble();
		}

		return random;

	  }

	}

}
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

	/// <summary>
	/// Base interface for multivariate distributions on the reals.
	/// 
	/// This is based largely on the RealDistribution interface, but cumulative
	/// distribution functions are not required because they are often quite
	/// difficult to compute for multivariate distributions.
	/// 
	/// @version $Id: MultivariateRealDistribution.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.1
	/// </summary>
	public interface MultivariateRealDistribution
	{
		/// <summary>
		/// Returns the probability density function (PDF) of this distribution
		/// evaluated at the specified point {@code x}. In general, the PDF is the
		/// derivative of the cumulative distribution function. If the derivative
		/// does not exist at {@code x}, then an appropriate replacement should be
		/// returned, e.g. {@code Double.POSITIVE_INFINITY}, {@code Double.NaN}, or
		/// the limit inferior or limit superior of the difference quotient.
		/// </summary>
		/// <param name="x"> Point at which the PDF is evaluated. </param>
		/// <returns> the value of the probability density function at point {@code x}. </returns>
		double density(double[] x);

		/// <summary>
		/// Reseeds the random generator used to generate samples.
		/// </summary>
		/// <param name="seed"> Seed with which to initialize the random number generator. </param>
		void reseedRandomGenerator(long seed);

		/// <summary>
		/// Gets the number of random variables of the distribution.
		/// It is the size of the array returned by the <seealso cref="#sample() sample"/>
		/// method.
		/// </summary>
		/// <returns> the number of variables. </returns>
		int Dimension {get;}

		/// <summary>
		/// Generates a random value vector sampled from this distribution.
		/// </summary>
		/// <returns> a random value vector. </returns>
		double[] sample();

		/// <summary>
		/// Generates a list of a random value vectors from the distribution.
		/// </summary>
		/// <param name="sampleSize"> the number of random vectors to generate. </param>
		/// <returns> an array representing the random samples. </returns>
		/// <exception cref="mathlib.exception.NotStrictlyPositiveException">
		/// if {@code sampleSize} is not positive.
		/// </exception>
		/// <seealso cref= #sample() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double[][] sample(int sampleSize) throws mathlib.exception.NotStrictlyPositiveException;
		double[][] sample(int sampleSize);
	}

}
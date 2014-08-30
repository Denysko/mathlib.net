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

	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;

	/// <summary>
	/// Interface for distributions on the integers.
	/// 
	/// @version $Id: IntegerDistribution.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public interface IntegerDistribution
	{
		/// <summary>
		/// For a random variable {@code X} whose values are distributed according
		/// to this distribution, this method returns {@code P(X = x)}. In other
		/// words, this method represents the probability mass function (PMF)
		/// for the distribution.
		/// </summary>
		/// <param name="x"> the point at which the PMF is evaluated </param>
		/// <returns> the value of the probability mass function at {@code x} </returns>
		double probability(int x);

		/// <summary>
		/// For a random variable {@code X} whose values are distributed according
		/// to this distribution, this method returns {@code P(X <= x)}.  In other
		/// words, this method represents the (cumulative) distribution function
		/// (CDF) for this distribution.
		/// </summary>
		/// <param name="x"> the point at which the CDF is evaluated </param>
		/// <returns> the probability that a random variable with this
		/// distribution takes a value less than or equal to {@code x} </returns>
		double cumulativeProbability(int x);

		/// <summary>
		/// For a random variable {@code X} whose values are distributed according
		/// to this distribution, this method returns {@code P(x0 < X <= x1)}.
		/// </summary>
		/// <param name="x0"> the exclusive lower bound </param>
		/// <param name="x1"> the inclusive upper bound </param>
		/// <returns> the probability that a random variable with this distribution
		/// will take a value between {@code x0} and {@code x1},
		/// excluding the lower and including the upper endpoint </returns>
		/// <exception cref="NumberIsTooLargeException"> if {@code x0 > x1} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double cumulativeProbability(int x0, int x1) throws org.apache.commons.math3.exception.NumberIsTooLargeException;
		double cumulativeProbability(int x0, int x1);

		/// <summary>
		/// Computes the quantile function of this distribution.
		/// For a random variable {@code X} distributed according to this distribution,
		/// the returned value is
		/// <ul>
		/// <li><code>inf{x in Z | P(X<=x) >= p}</code> for {@code 0 < p <= 1},</li>
		/// <li><code>inf{x in Z | P(X<=x) > 0}</code> for {@code p = 0}.</li>
		/// </ul>
		/// If the result exceeds the range of the data type {@code int},
		/// then {@code Integer.MIN_VALUE} or {@code Integer.MAX_VALUE} is returned.
		/// </summary>
		/// <param name="p"> the cumulative probability </param>
		/// <returns> the smallest {@code p}-quantile of this distribution
		/// (largest 0-quantile for {@code p = 0}) </returns>
		/// <exception cref="OutOfRangeException"> if {@code p < 0} or {@code p > 1} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int inverseCumulativeProbability(double p) throws org.apache.commons.math3.exception.OutOfRangeException;
		int inverseCumulativeProbability(double p);

		/// <summary>
		/// Use this method to get the numerical value of the mean of this
		/// distribution.
		/// </summary>
		/// <returns> the mean or {@code Double.NaN} if it is not defined </returns>
		double NumericalMean {get;}

		/// <summary>
		/// Use this method to get the numerical value of the variance of this
		/// distribution.
		/// </summary>
		/// <returns> the variance (possibly {@code Double.POSITIVE_INFINITY} or
		/// {@code Double.NaN} if it is not defined) </returns>
		double NumericalVariance {get;}

		/// <summary>
		/// Access the lower bound of the support. This method must return the same
		/// value as {@code inverseCumulativeProbability(0)}. In other words, this
		/// method must return
		/// <p><code>inf {x in Z | P(X <= x) > 0}</code>.</p>
		/// </summary>
		/// <returns> lower bound of the support ({@code Integer.MIN_VALUE}
		/// for negative infinity) </returns>
		int SupportLowerBound {get;}

		/// <summary>
		/// Access the upper bound of the support. This method must return the same
		/// value as {@code inverseCumulativeProbability(1)}. In other words, this
		/// method must return
		/// <p><code>inf {x in R | P(X <= x) = 1}</code>.</p>
		/// </summary>
		/// <returns> upper bound of the support ({@code Integer.MAX_VALUE}
		/// for positive infinity) </returns>
		int SupportUpperBound {get;}

		/// <summary>
		/// Use this method to get information about whether the support is
		/// connected, i.e. whether all integers between the lower and upper bound of
		/// the support are included in the support.
		/// </summary>
		/// <returns> whether the support is connected or not </returns>
		bool SupportConnected {get;}

		/// <summary>
		/// Reseed the random generator used to generate samples.
		/// </summary>
		/// <param name="seed"> the new seed
		/// @since 3.0 </param>
		void reseedRandomGenerator(long seed);

		/// <summary>
		/// Generate a random value sampled from this distribution.
		/// </summary>
		/// <returns> a random value
		/// @since 3.0 </returns>
		int sample();

		/// <summary>
		/// Generate a random sample from the distribution.
		/// </summary>
		/// <param name="sampleSize"> the number of random values to generate </param>
		/// <returns> an array representing the random sample </returns>
		/// <exception cref="org.apache.commons.math3.exception.NotStrictlyPositiveException">
		/// if {@code sampleSize} is not positive
		/// @since 3.0 </exception>
		int[] sample(int sampleSize);
	}

}
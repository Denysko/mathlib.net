using System;

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
	using OutOfRangeException = mathlib.exception.OutOfRangeException;

	/// <summary>
	/// Base interface for distributions on the reals.
	/// 
	/// @version $Id: RealDistribution.java 1499808 2013-07-04 17:00:42Z sebb $
	/// @since 3.0
	/// </summary>
	public interface RealDistribution
	{
		/// <summary>
		/// For a random variable {@code X} whose values are distributed according
		/// to this distribution, this method returns {@code P(X = x)}. In other
		/// words, this method represents the probability mass function (PMF)
		/// for the distribution.
		/// </summary>
		/// <param name="x"> the point at which the PMF is evaluated </param>
		/// <returns> the value of the probability mass function at point {@code x} </returns>
		double probability(double x);

		/// <summary>
		/// Returns the probability density function (PDF) of this distribution
		/// evaluated at the specified point {@code x}. In general, the PDF is
		/// the derivative of the <seealso cref="#cumulativeProbability(double) CDF"/>.
		/// If the derivative does not exist at {@code x}, then an appropriate
		/// replacement should be returned, e.g. {@code Double.POSITIVE_INFINITY},
		/// {@code Double.NaN}, or  the limit inferior or limit superior of the
		/// difference quotient.
		/// </summary>
		/// <param name="x"> the point at which the PDF is evaluated </param>
		/// <returns> the value of the probability density function at point {@code x} </returns>
		double density(double x);

		/// <summary>
		/// For a random variable {@code X} whose values are distributed according
		/// to this distribution, this method returns {@code P(X <= x)}. In other
		/// words, this method represents the (cumulative) distribution function
		/// (CDF) for this distribution.
		/// </summary>
		/// <param name="x"> the point at which the CDF is evaluated </param>
		/// <returns> the probability that a random variable with this
		/// distribution takes a value less than or equal to {@code x} </returns>
		double cumulativeProbability(double x);

		/// <summary>
		/// For a random variable {@code X} whose values are distributed according
		/// to this distribution, this method returns {@code P(x0 < X <= x1)}.
		/// </summary>
		/// <param name="x0"> the exclusive lower bound </param>
		/// <param name="x1"> the inclusive upper bound </param>
		/// <returns> the probability that a random variable with this distribution
		/// takes a value between {@code x0} and {@code x1},
		/// excluding the lower and including the upper endpoint </returns>
		/// <exception cref="NumberIsTooLargeException"> if {@code x0 > x1}
		/// </exception>
		/// @deprecated As of 3.1. In 4.0, this method will be renamed
		/// {@code probability(double x0, double x1)}. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("As of 3.1. In 4.0, this method will be renamed") double cumulativeProbability(double x0, double x1) throws mathlib.exception.NumberIsTooLargeException;
		[Obsolete("As of 3.1. In 4.0, this method will be renamed")]
		double cumulativeProbability(double x0, double x1);

		/// <summary>
		/// Computes the quantile function of this distribution. For a random
		/// variable {@code X} distributed according to this distribution, the
		/// returned value is
		/// <ul>
		/// <li><code>inf{x in R | P(X<=x) >= p}</code> for {@code 0 < p <= 1},</li>
		/// <li><code>inf{x in R | P(X<=x) > 0}</code> for {@code p = 0}.</li>
		/// </ul>
		/// </summary>
		/// <param name="p"> the cumulative probability </param>
		/// <returns> the smallest {@code p}-quantile of this distribution
		/// (largest 0-quantile for {@code p = 0}) </returns>
		/// <exception cref="OutOfRangeException"> if {@code p < 0} or {@code p > 1} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double inverseCumulativeProbability(double p) throws mathlib.exception.OutOfRangeException;
		double inverseCumulativeProbability(double p);

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
		/// <returns> the variance (possibly {@code Double.POSITIVE_INFINITY} as
		/// for certain cases in <seealso cref="TDistribution"/>) or {@code Double.NaN} if it
		/// is not defined </returns>
		double NumericalVariance {get;}

		/// <summary>
		/// Access the lower bound of the support. This method must return the same
		/// value as {@code inverseCumulativeProbability(0)}. In other words, this
		/// method must return
		/// <p><code>inf {x in R | P(X <= x) > 0}</code>.</p>
		/// </summary>
		/// <returns> lower bound of the support (might be
		/// {@code Double.NEGATIVE_INFINITY}) </returns>
		double SupportLowerBound {get;}

		/// <summary>
		/// Access the upper bound of the support. This method must return the same
		/// value as {@code inverseCumulativeProbability(1)}. In other words, this
		/// method must return
		/// <p><code>inf {x in R | P(X <= x) = 1}</code>.</p>
		/// </summary>
		/// <returns> upper bound of the support (might be
		/// {@code Double.POSITIVE_INFINITY}) </returns>
		double SupportUpperBound {get;}

		/// <summary>
		/// Whether or not the lower bound of support is in the domain of the density
		/// function.  Returns true iff {@code getSupporLowerBound()} is finite and
		/// {@code density(getSupportLowerBound())} returns a non-NaN, non-infinite
		/// value.
		/// </summary>
		/// <returns> true if the lower bound of support is finite and the density
		/// function returns a non-NaN, non-infinite value there </returns>
		/// @deprecated to be removed in 4.0 
		[Obsolete("to be removed in 4.0")]
		bool SupportLowerBoundInclusive {get;}

		/// <summary>
		/// Whether or not the upper bound of support is in the domain of the density
		/// function.  Returns true iff {@code getSupportUpperBound()} is finite and
		/// {@code density(getSupportUpperBound())} returns a non-NaN, non-infinite
		/// value.
		/// </summary>
		/// <returns> true if the upper bound of support is finite and the density
		/// function returns a non-NaN, non-infinite value there </returns>
		/// @deprecated to be removed in 4.0 
		[Obsolete("to be removed in 4.0")]
		bool SupportUpperBoundInclusive {get;}

		/// <summary>
		/// Use this method to get information about whether the support is connected,
		/// i.e. whether all values between the lower and upper bound of the support
		/// are included in the support.
		/// </summary>
		/// <returns> whether the support is connected or not </returns>
		bool SupportConnected {get;}

		/// <summary>
		/// Reseed the random generator used to generate samples.
		/// </summary>
		/// <param name="seed"> the new seed </param>
		void reseedRandomGenerator(long seed);

		/// <summary>
		/// Generate a random value sampled from this distribution.
		/// </summary>
		/// <returns> a random value. </returns>
		double sample();

		/// <summary>
		/// Generate a random sample from the distribution.
		/// </summary>
		/// <param name="sampleSize"> the number of random values to generate </param>
		/// <returns> an array representing the random sample </returns>
		/// <exception cref="mathlib.exception.NotStrictlyPositiveException">
		/// if {@code sampleSize} is not positive </exception>
		double[] sample(int sampleSize);
	}

}
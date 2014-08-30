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
namespace org.apache.commons.math3.stat.interval
{

	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;

	/// <summary>
	/// Abstract base class to generate confidence intervals for a binomial proportion.
	/// </summary>
	/// <seealso cref= <a
	///      href="http://en.wikipedia.org/wiki/Binomial_proportion_confidence_interval">Binomial
	///      proportion confidence interval (Wikipedia)</a>
	/// @version $Id: BinomialConfidenceInterval.java 1560531 2014-01-22 22:00:37Z tn $
	/// @since 3.3 </seealso>
	public interface BinomialConfidenceInterval
	{

		/// <summary>
		/// Create a confidence interval for the true probability of success
		/// of an unknown binomial distribution with the given observed number
		/// of trials, successes and confidence level.
		/// <p>
		/// Preconditions:
		/// <ul>
		/// <li>{@code numberOfTrials} must be positive</li>
		/// <li>{@code numberOfSuccesses} may not exceed {@code numberOfTrials}</li>
		/// <li>{@code confidenceLevel} must be strictly between 0 and 1 (exclusive)</li>
		/// </ul>
		/// </p>
		/// </summary>
		/// <param name="numberOfTrials"> number of trials </param>
		/// <param name="numberOfSuccesses"> number of successes </param>
		/// <param name="confidenceLevel"> desired probability that the true probability of
		///        success falls within the returned interval </param>
		/// <returns> Confidence interval containing the probability of success with
		///         probability {@code confidenceLevel} </returns>
		/// <exception cref="NotStrictlyPositiveException"> if {@code numberOfTrials <= 0}. </exception>
		/// <exception cref="NotPositiveException"> if {@code numberOfSuccesses < 0}. </exception>
		/// <exception cref="NumberIsTooLargeException"> if {@code numberOfSuccesses > numberOfTrials}. </exception>
		/// <exception cref="OutOfRangeException"> if {@code confidenceLevel} is not in the interval {@code (0, 1)}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ConfidenceInterval createInterval(int numberOfTrials, int numberOfSuccesses, double confidenceLevel) throws org.apache.commons.math3.exception.NotStrictlyPositiveException, org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.NumberIsTooLargeException, org.apache.commons.math3.exception.OutOfRangeException;
		ConfidenceInterval createInterval(int numberOfTrials, int numberOfSuccesses, double confidenceLevel);

	}

}
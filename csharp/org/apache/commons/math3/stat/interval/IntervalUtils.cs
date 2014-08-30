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

	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// Factory methods to generate confidence intervals for a binomial proportion.
	/// The supported methods are:
	/// <ul>
	/// <li>Agresti-Coull interval</li>
	/// <li>Clopper-Pearson method (exact method)</li>
	/// <li>Normal approximation (based on central limit theorem)</li>
	/// <li>Wilson score interval</li>
	/// </ul>
	/// 
	/// @version $Id: IntervalUtils.java 1560551 2014-01-22 22:35:21Z tn $
	/// @since 3.3
	/// </summary>
	public sealed class IntervalUtils
	{

		/// <summary>
		/// Singleton Agresti-Coull instance. </summary>
		private static readonly BinomialConfidenceInterval AGRESTI_COULL = new AgrestiCoullInterval();

		/// <summary>
		/// Singleton Clopper-Pearson instance. </summary>
		private static readonly BinomialConfidenceInterval CLOPPER_PEARSON = new ClopperPearsonInterval();

		/// <summary>
		/// Singleton NormalApproximation instance. </summary>
		private static readonly BinomialConfidenceInterval NORMAL_APPROXIMATION = new NormalApproximationInterval();

		/// <summary>
		/// Singleton Wilson score instance. </summary>
		private static readonly BinomialConfidenceInterval WILSON_SCORE = new WilsonScoreInterval();

		/// <summary>
		/// Prevent instantiation.
		/// </summary>
		private IntervalUtils()
		{
		}

		/// <summary>
		/// Create an Agresti-Coull binomial confidence interval for the true
		/// probability of success of an unknown binomial distribution with the given
		/// observed number of trials, successes and confidence level.
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
		public static ConfidenceInterval getAgrestiCoullInterval(int numberOfTrials, int numberOfSuccesses, double confidenceLevel)
		{
			return AGRESTI_COULL.createInterval(numberOfTrials, numberOfSuccesses, confidenceLevel);
		}

		/// <summary>
		/// Create a Clopper-Pearson binomial confidence interval for the true
		/// probability of success of an unknown binomial distribution with the given
		/// observed number of trials, successes and confidence level.
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
		public static ConfidenceInterval getClopperPearsonInterval(int numberOfTrials, int numberOfSuccesses, double confidenceLevel)
		{
			return CLOPPER_PEARSON.createInterval(numberOfTrials, numberOfSuccesses, confidenceLevel);
		}

		/// <summary>
		/// Create a binomial confidence interval for the true probability of success
		/// of an unknown binomial distribution with the given observed number of
		/// trials, successes and confidence level using the Normal approximation to
		/// the binomial distribution.
		/// </summary>
		/// <param name="numberOfTrials"> number of trials </param>
		/// <param name="numberOfSuccesses"> number of successes </param>
		/// <param name="confidenceLevel"> desired probability that the true probability of
		///        success falls within the interval </param>
		/// <returns> Confidence interval containing the probability of success with
		///         probability {@code confidenceLevel} </returns>
		public static ConfidenceInterval getNormalApproximationInterval(int numberOfTrials, int numberOfSuccesses, double confidenceLevel)
		{
			return NORMAL_APPROXIMATION.createInterval(numberOfTrials, numberOfSuccesses, confidenceLevel);
		}

		/// <summary>
		/// Create a Wilson score binomial confidence interval for the true
		/// probability of success of an unknown binomial distribution with the given
		/// observed number of trials, successes and confidence level.
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
		public static ConfidenceInterval getWilsonScoreInterval(int numberOfTrials, int numberOfSuccesses, double confidenceLevel)
		{
			return WILSON_SCORE.createInterval(numberOfTrials, numberOfSuccesses, confidenceLevel);
		}

		/// <summary>
		/// Verifies that parameters satisfy preconditions.
		/// </summary>
		/// <param name="numberOfTrials"> number of trials (must be positive) </param>
		/// <param name="numberOfSuccesses"> number of successes (must not exceed numberOfTrials) </param>
		/// <param name="confidenceLevel"> confidence level (must be strictly between 0 and 1) </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code numberOfTrials <= 0}. </exception>
		/// <exception cref="NotPositiveException"> if {@code numberOfSuccesses < 0}. </exception>
		/// <exception cref="NumberIsTooLargeException"> if {@code numberOfSuccesses > numberOfTrials}. </exception>
		/// <exception cref="OutOfRangeException"> if {@code confidenceLevel} is not in the interval {@code (0, 1)}. </exception>
		internal static void checkParameters(int numberOfTrials, int numberOfSuccesses, double confidenceLevel)
		{
			if (numberOfTrials <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.NUMBER_OF_TRIALS, numberOfTrials);
			}
			if (numberOfSuccesses < 0)
			{
				throw new NotPositiveException(LocalizedFormats.NEGATIVE_NUMBER_OF_SUCCESSES, numberOfSuccesses);
			}
			if (numberOfSuccesses > numberOfTrials)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.NUMBER_OF_SUCCESS_LARGER_THAN_POPULATION_SIZE, numberOfSuccesses, numberOfTrials, true);
			}
			if (confidenceLevel <= 0 || confidenceLevel >= 1)
			{
				throw new OutOfRangeException(LocalizedFormats.OUT_OF_BOUNDS_CONFIDENCE_LEVEL, confidenceLevel, 0, 1);
			}
		}

	}

}
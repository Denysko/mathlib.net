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
namespace org.apache.commons.math3.stat.inference
{

	using BinomialDistribution = org.apache.commons.math3.distribution.BinomialDistribution;
	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using MathInternalError = org.apache.commons.math3.exception.MathInternalError;
	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// Implements binomial test statistics.
	/// <p>
	/// Exact test for the statistical significance of deviations from a
	/// theoretically expected distribution of observations into two categories.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Binomial_test">Binomial test (Wikipedia)</a>
	/// @version $Id: BinomialTest.java 1532638 2013-10-16 04:29:31Z psteitz $
	/// @since 3.3 </seealso>
	public class BinomialTest
	{

		/// <summary>
		/// Returns whether the null hypothesis can be rejected with the given confidence level.
		/// <p>
		/// <strong>Preconditions</strong>:
		/// <ul>
		/// <li>Number of trials must be &ge; 0.</li>
		/// <li>Number of successes must be &ge; 0.</li>
		/// <li>Number of successes must be &le; number of trials.</li>
		/// <li>Probability must be &ge; 0 and &le; 1.</li>
		/// </ul>
		/// </summary>
		/// <param name="numberOfTrials"> number of trials performed </param>
		/// <param name="numberOfSuccesses"> number of successes observed </param>
		/// <param name="probability"> assumed probability of a single trial under the null hypothesis </param>
		/// <param name="alternativeHypothesis"> type of hypothesis being evaluated (one- or two-sided) </param>
		/// <param name="alpha"> significance level of the test </param>
		/// <returns> true if the null hypothesis can be rejected with confidence {@code 1 - alpha} </returns>
		/// <exception cref="NotPositiveException"> if {@code numberOfTrials} or {@code numberOfSuccesses} is negative </exception>
		/// <exception cref="OutOfRangeException"> if {@code probability} is not between 0 and 1 </exception>
		/// <exception cref="MathIllegalArgumentException"> if {@code numberOfTrials} &lt; {@code numberOfSuccesses} or
		/// if {@code alternateHypothesis} is null. </exception>
		/// <seealso cref= AlternativeHypothesis </seealso>
		public virtual bool binomialTest(int numberOfTrials, int numberOfSuccesses, double probability, AlternativeHypothesis alternativeHypothesis, double alpha)
		{
			double pValue = binomialTest(numberOfTrials, numberOfSuccesses, probability, alternativeHypothesis);
			return pValue < alpha;
		}

		/// <summary>
		/// Returns the <i>observed significance level</i>, or
		/// <a href="http://www.cas.lancs.ac.uk/glossary_v1.1/hyptest.html#pvalue">p-value</a>,
		/// associated with a <a href="http://en.wikipedia.org/wiki/Binomial_test"> Binomial test</a>.
		/// <p>
		/// The number returned is the smallest significance level at which one can reject the null hypothesis.
		/// The form of the hypothesis depends on {@code alternativeHypothesis}.</p>
		/// <p>
		/// The p-Value represents the likelihood of getting a result at least as extreme as the sample,
		/// given the provided {@code probability} of success on a single trial. For single-sided tests,
		/// this value can be directly derived from the Binomial distribution. For the two-sided test,
		/// the implementation works as follows: we start by looking at the most extreme cases
		/// (0 success and n success where n is the number of trials from the sample) and determine their likelihood.
		/// The lower value is added to the p-Value (if both values are equal, both are added). Then we continue with
		/// the next extreme value, until we added the value for the actual observed sample.</p>
		/// <p>
		/// <strong>Preconditions</strong>:
		/// <ul>
		/// <li>Number of trials must be &ge; 0.</li>
		/// <li>Number of successes must be &ge; 0.</li>
		/// <li>Number of successes must be &le; number of trials.</li>
		/// <li>Probability must be &ge; 0 and &le; 1.</li>
		/// </ul></p>
		/// </summary>
		/// <param name="numberOfTrials"> number of trials performed </param>
		/// <param name="numberOfSuccesses"> number of successes observed </param>
		/// <param name="probability"> assumed probability of a single trial under the null hypothesis </param>
		/// <param name="alternativeHypothesis"> type of hypothesis being evaluated (one- or two-sided) </param>
		/// <returns> p-value </returns>
		/// <exception cref="NotPositiveException"> if {@code numberOfTrials} or {@code numberOfSuccesses} is negative </exception>
		/// <exception cref="OutOfRangeException"> if {@code probability} is not between 0 and 1 </exception>
		/// <exception cref="MathIllegalArgumentException"> if {@code numberOfTrials} &lt; {@code numberOfSuccesses} or
		/// if {@code alternateHypothesis} is null. </exception>
		/// <seealso cref= AlternativeHypothesis </seealso>
		public virtual double binomialTest(int numberOfTrials, int numberOfSuccesses, double probability, AlternativeHypothesis alternativeHypothesis)
		{
			if (numberOfTrials < 0)
			{
				throw new NotPositiveException(numberOfTrials);
			}
			if (numberOfSuccesses < 0)
			{
				throw new NotPositiveException(numberOfSuccesses);
			}
			if (probability < 0 || probability > 1)
			{
				throw new OutOfRangeException(probability, 0, 1);
			}
			if (numberOfTrials < numberOfSuccesses)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.BINOMIAL_INVALID_PARAMETERS_ORDER, numberOfTrials, numberOfSuccesses);
			}
			if (alternativeHypothesis == null)
			{
				throw new NullArgumentException();
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.distribution.BinomialDistribution distribution = new org.apache.commons.math3.distribution.BinomialDistribution(numberOfTrials, probability);
			BinomialDistribution distribution = new BinomialDistribution(numberOfTrials, probability);
			switch (alternativeHypothesis)
			{
			case org.apache.commons.math3.stat.inference.AlternativeHypothesis.GREATER_THAN:
				return 1 - distribution.cumulativeProbability(numberOfSuccesses - 1);
			case org.apache.commons.math3.stat.inference.AlternativeHypothesis.LESS_THAN:
				return distribution.cumulativeProbability(numberOfSuccesses);
			case org.apache.commons.math3.stat.inference.AlternativeHypothesis.TWO_SIDED:
				int criticalValueLow = 0;
				int criticalValueHigh = numberOfTrials;
				double pTotal = 0;

				while (true)
				{
					double pLow = distribution.probability(criticalValueLow);
					double pHigh = distribution.probability(criticalValueHigh);

					if (pLow == pHigh)
					{
						pTotal += 2 * pLow;
						criticalValueLow++;
						criticalValueHigh--;
					}
					else if (pLow < pHigh)
					{
						pTotal += pLow;
						criticalValueLow++;
					}
					else
					{
						pTotal += pHigh;
						criticalValueHigh--;
					}

					if (criticalValueLow > numberOfSuccesses || criticalValueHigh < numberOfSuccesses)
					{
						break;
					}
				}
				return pTotal;
			default:
				throw new MathInternalError(LocalizedFormats.OUT_OF_RANGE_SIMPLE, alternativeHypothesis, AlternativeHypothesis.TWO_SIDED, AlternativeHypothesis.LESS_THAN);
			}
		}
	}

}
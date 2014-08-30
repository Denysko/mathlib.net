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

	using NormalDistribution = org.apache.commons.math3.distribution.NormalDistribution;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Implements the normal approximation method for creating a binomial proportion confidence interval.
	/// </summary>
	/// <seealso cref= <a
	///      href="http://en.wikipedia.org/wiki/Binomial_proportion_confidence_interval#Normal_approximation_interval">
	///      Normal approximation interval (Wikipedia)</a>
	/// @version $Id: NormalApproximationInterval.java 1560928 2014-01-24 09:58:43Z luc $
	/// @since 3.3 </seealso>
	public class NormalApproximationInterval : BinomialConfidenceInterval
	{

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual ConfidenceInterval createInterval(int numberOfTrials, int numberOfSuccesses, double confidenceLevel)
		{
			IntervalUtils.checkParameters(numberOfTrials, numberOfSuccesses, confidenceLevel);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double mean = (double) numberOfSuccesses / (double) numberOfTrials;
			double mean = (double) numberOfSuccesses / (double) numberOfTrials;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double alpha = (1.0 - confidenceLevel) / 2;
			double alpha = (1.0 - confidenceLevel) / 2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.distribution.NormalDistribution normalDistribution = new org.apache.commons.math3.distribution.NormalDistribution();
			NormalDistribution normalDistribution = new NormalDistribution();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double difference = normalDistribution.inverseCumulativeProbability(1 - alpha) * org.apache.commons.math3.util.FastMath.sqrt(1.0 / numberOfTrials * mean * (1 - mean));
			double difference = normalDistribution.inverseCumulativeProbability(1 - alpha) * FastMath.sqrt(1.0 / numberOfTrials * mean * (1 - mean));
			return new ConfidenceInterval(mean - difference, mean + difference, confidenceLevel);
		}

	}

}
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
namespace mathlib.stat.interval
{

	using NormalDistribution = mathlib.distribution.NormalDistribution;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Implements the Wilson score method for creating a binomial proportion confidence interval.
	/// </summary>
	/// <seealso cref= <a
	///      href="http://en.wikipedia.org/wiki/Binomial_proportion_confidence_interval#Wilson_score_interval">
	///      Wilson score interval (Wikipedia)</a>
	/// @version $Id: WilsonScoreInterval.java 1560928 2014-01-24 09:58:43Z luc $
	/// @since 3.3 </seealso>
	public class WilsonScoreInterval : BinomialConfidenceInterval
	{

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual ConfidenceInterval createInterval(int numberOfTrials, int numberOfSuccesses, double confidenceLevel)
		{
			IntervalUtils.checkParameters(numberOfTrials, numberOfSuccesses, confidenceLevel);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double alpha = (1.0 - confidenceLevel) / 2;
			double alpha = (1.0 - confidenceLevel) / 2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.distribution.NormalDistribution normalDistribution = new mathlib.distribution.NormalDistribution();
			NormalDistribution normalDistribution = new NormalDistribution();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z = normalDistribution.inverseCumulativeProbability(1 - alpha);
			double z = normalDistribution.inverseCumulativeProbability(1 - alpha);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double zSquared = mathlib.util.FastMath.pow(z, 2);
			double zSquared = FastMath.pow(z, 2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double mean = (double) numberOfSuccesses / (double) numberOfTrials;
			double mean = (double) numberOfSuccesses / (double) numberOfTrials;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double factor = 1.0 / (1 + (1.0 / numberOfTrials) * zSquared);
			double factor = 1.0 / (1 + (1.0 / numberOfTrials) * zSquared);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double modifiedSuccessRatio = mean + (1.0 / (2 * numberOfTrials)) * zSquared;
			double modifiedSuccessRatio = mean + (1.0 / (2 * numberOfTrials)) * zSquared;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double difference = z * mathlib.util.FastMath.sqrt(1.0 / numberOfTrials * mean * (1 - mean) + (1.0 / (4 * mathlib.util.FastMath.pow(numberOfTrials, 2)) * zSquared));
			double difference = z * FastMath.sqrt(1.0 / numberOfTrials * mean * (1 - mean) + (1.0 / (4 * FastMath.pow(numberOfTrials, 2)) * zSquared));

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lowerBound = factor * (modifiedSuccessRatio - difference);
			double lowerBound = factor * (modifiedSuccessRatio - difference);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double upperBound = factor * (modifiedSuccessRatio + difference);
			double upperBound = factor * (modifiedSuccessRatio + difference);
			return new ConfidenceInterval(lowerBound, upperBound, confidenceLevel);
		}

	}

}
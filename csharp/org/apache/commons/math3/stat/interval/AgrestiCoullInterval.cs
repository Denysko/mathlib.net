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
	/// Implements the Agresti-Coull method for creating a binomial proportion confidence interval.
	/// </summary>
	/// <seealso cref= <a
	///      href="http://en.wikipedia.org/wiki/Binomial_proportion_confidence_interval#Agresti-Coull_Interval">
	///      Agresti-Coull interval (Wikipedia)</a>
	/// @version $Id: AgrestiCoullInterval.java 1560928 2014-01-24 09:58:43Z luc $
	/// @since 3.3 </seealso>
	public class AgrestiCoullInterval : BinomialConfidenceInterval
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
//ORIGINAL LINE: final org.apache.commons.math3.distribution.NormalDistribution normalDistribution = new org.apache.commons.math3.distribution.NormalDistribution();
			NormalDistribution normalDistribution = new NormalDistribution();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z = normalDistribution.inverseCumulativeProbability(1 - alpha);
			double z = normalDistribution.inverseCumulativeProbability(1 - alpha);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double zSquared = org.apache.commons.math3.util.FastMath.pow(z, 2);
			double zSquared = FastMath.pow(z, 2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double modifiedNumberOfTrials = numberOfTrials + zSquared;
			double modifiedNumberOfTrials = numberOfTrials + zSquared;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double modifiedSuccessesRatio = (1.0 / modifiedNumberOfTrials) * (numberOfSuccesses + 0.5 * zSquared);
			double modifiedSuccessesRatio = (1.0 / modifiedNumberOfTrials) * (numberOfSuccesses + 0.5 * zSquared);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double difference = z * org.apache.commons.math3.util.FastMath.sqrt(1.0 / modifiedNumberOfTrials * modifiedSuccessesRatio * (1 - modifiedSuccessesRatio));
			double difference = z * FastMath.sqrt(1.0 / modifiedNumberOfTrials * modifiedSuccessesRatio * (1 - modifiedSuccessesRatio));
			return new ConfidenceInterval(modifiedSuccessesRatio - difference, modifiedSuccessesRatio + difference, confidenceLevel);
		}

	}

}
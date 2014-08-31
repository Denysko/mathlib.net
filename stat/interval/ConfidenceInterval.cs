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

	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// Represents an interval estimate of a population parameter.
	/// 
	/// @version $Id: ConfidenceInterval.java 1560531 2014-01-22 22:00:37Z tn $
	/// @since 3.3
	/// </summary>
	public class ConfidenceInterval
	{

		/// <summary>
		/// Lower endpoint of the interval </summary>
		private double lowerBound;

		/// <summary>
		/// Upper endpoint of the interval </summary>
		private double upperBound;

		/// <summary>
		/// The asserted probability that the interval contains the population
		/// parameter
		/// </summary>
		private double confidenceLevel;

		/// <summary>
		/// Create a confidence interval with the given bounds and confidence level.
		/// <p>
		/// Preconditions:
		/// <ul>
		/// <li>{@code lower} must be strictly less than {@code upper}</li>
		/// <li>{@code confidenceLevel} must be strictly between 0 and 1 (exclusive)</li>
		/// </ul>
		/// </p>
		/// </summary>
		/// <param name="lowerBound"> lower endpoint of the interval </param>
		/// <param name="upperBound"> upper endpoint of the interval </param>
		/// <param name="confidenceLevel"> coverage probability </param>
		/// <exception cref="MathIllegalArgumentException"> if the preconditions are not met </exception>
		public ConfidenceInterval(double lowerBound, double upperBound, double confidenceLevel)
		{
			checkParameters(lowerBound, upperBound, confidenceLevel);
			this.lowerBound = lowerBound;
			this.upperBound = upperBound;
			this.confidenceLevel = confidenceLevel;
		}

		/// <returns> the lower endpoint of the interval </returns>
		public virtual double LowerBound
		{
			get
			{
				return lowerBound;
			}
		}

		/// <returns> the upper endpoint of the interval </returns>
		public virtual double UpperBound
		{
			get
			{
				return upperBound;
			}
		}

		/// <returns> the asserted probability that the interval contains the
		///         population parameter </returns>
		public virtual double ConfidenceLevel
		{
			get
			{
				return confidenceLevel;
			}
		}

		/// <returns> String representation of the confidence interval </returns>
		public override string ToString()
		{
			return "[" + lowerBound + ";" + upperBound + "] (confidence level:" + confidenceLevel + ")";
		}

		/// <summary>
		/// Verifies that (lower, upper) is a valid non-empty interval and confidence
		/// is strictly between 0 and 1.
		/// </summary>
		/// <param name="lower"> lower endpoint </param>
		/// <param name="upper"> upper endpoint </param>
		/// <param name="confidence"> confidence level </param>
		private void checkParameters(double lower, double upper, double confidence)
		{
			if (lower >= upper)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.LOWER_BOUND_NOT_BELOW_UPPER_BOUND, lower, upper);
			}
			if (confidence <= 0 || confidence >= 1)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.OUT_OF_BOUNDS_CONFIDENCE_LEVEL, confidence, 0, 1);
			}
		}
	}

}
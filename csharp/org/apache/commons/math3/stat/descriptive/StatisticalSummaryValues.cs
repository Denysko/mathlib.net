using System;
using System.Text;

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
namespace org.apache.commons.math3.stat.descriptive
{

	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathUtils = org.apache.commons.math3.util.MathUtils;
	using Precision = org.apache.commons.math3.util.Precision;

	/// <summary>
	///  Value object representing the results of a univariate statistical summary.
	/// 
	/// @version $Id: StatisticalSummaryValues.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class StatisticalSummaryValues : StatisticalSummary
	{

		/// <summary>
		/// Serialization id </summary>
		private const long serialVersionUID = -5108854841843722536L;

		/// <summary>
		/// The sample mean </summary>
		private readonly double mean;

		/// <summary>
		/// The sample variance </summary>
		private readonly double variance;

		/// <summary>
		/// The number of observations in the sample </summary>
		private readonly long n;

		/// <summary>
		/// The maximum value </summary>
		private readonly double max;

		/// <summary>
		/// The minimum value </summary>
		private readonly double min;

		/// <summary>
		/// The sum of the sample values </summary>
		private readonly double sum;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="mean">  the sample mean </param>
		/// <param name="variance">  the sample variance </param>
		/// <param name="n">  the number of observations in the sample </param>
		/// <param name="max">  the maximum value </param>
		/// <param name="min">  the minimum value </param>
		/// <param name="sum">  the sum of the values </param>
		public StatisticalSummaryValues(double mean, double variance, long n, double max, double min, double sum) : base()
		{
			this.mean = mean;
			this.variance = variance;
			this.n = n;
			this.max = max;
			this.min = min;
			this.sum = sum;
		}

		/// <returns> Returns the max. </returns>
		public virtual double Max
		{
			get
			{
				return max;
			}
		}

		/// <returns> Returns the mean. </returns>
		public virtual double Mean
		{
			get
			{
				return mean;
			}
		}

		/// <returns> Returns the min. </returns>
		public virtual double Min
		{
			get
			{
				return min;
			}
		}

		/// <returns> Returns the number of values. </returns>
		public virtual long N
		{
			get
			{
				return n;
			}
		}

		/// <returns> Returns the sum. </returns>
		public virtual double Sum
		{
			get
			{
				return sum;
			}
		}

		/// <returns> Returns the standard deviation </returns>
		public virtual double StandardDeviation
		{
			get
			{
				return FastMath.sqrt(variance);
			}
		}

		/// <returns> Returns the variance. </returns>
		public virtual double Variance
		{
			get
			{
				return variance;
			}
		}

		/// <summary>
		/// Returns true iff <code>object</code> is a
		/// <code>StatisticalSummaryValues</code> instance and all statistics have
		///  the same values as this.
		/// </summary>
		/// <param name="object"> the object to test equality against. </param>
		/// <returns> true if object equals this </returns>
		public override bool Equals(object @object)
		{
			if (@object == this)
			{
				return true;
			}
			if (@object is StatisticalSummaryValues == false)
			{
				return false;
			}
			StatisticalSummaryValues stat = (StatisticalSummaryValues) @object;
			return Precision.equalsIncludingNaN(stat.Max, Max) && Precision.equalsIncludingNaN(stat.Mean, Mean) && Precision.equalsIncludingNaN(stat.Min, Min) && Precision.equalsIncludingNaN(stat.N, N) && Precision.equalsIncludingNaN(stat.Sum, Sum) && Precision.equalsIncludingNaN(stat.Variance, Variance);
		}

		/// <summary>
		/// Returns hash code based on values of statistics
		/// </summary>
		/// <returns> hash code </returns>
		public override int GetHashCode()
		{
			int result = 31 + MathUtils.hash(Max);
			result = result * 31 + MathUtils.hash(Mean);
			result = result * 31 + MathUtils.hash(Min);
			result = result * 31 + MathUtils.hash(N);
			result = result * 31 + MathUtils.hash(Sum);
			result = result * 31 + MathUtils.hash(Variance);
			return result;
		}

		/// <summary>
		/// Generates a text report displaying values of statistics.
		/// Each statistic is displayed on a separate line.
		/// </summary>
		/// <returns> String with line feeds displaying statistics </returns>
		public override string ToString()
		{
			StringBuilder outBuffer = new StringBuilder();
			string endl = "\n";
			outBuffer.Append("StatisticalSummaryValues:").Append(endl);
			outBuffer.Append("n: ").Append(N).Append(endl);
			outBuffer.Append("min: ").Append(Min).Append(endl);
			outBuffer.Append("max: ").Append(Max).Append(endl);
			outBuffer.Append("mean: ").Append(Mean).Append(endl);
			outBuffer.Append("std dev: ").Append(StandardDeviation).Append(endl);
			outBuffer.Append("variance: ").Append(Variance).Append(endl);
			outBuffer.Append("sum: ").Append(Sum).Append(endl);
			return outBuffer.ToString();
		}

	}

}
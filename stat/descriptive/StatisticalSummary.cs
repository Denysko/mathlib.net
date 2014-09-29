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
namespace mathlib.stat.descriptive
{

	/// <summary>
	///  Reporting interface for basic univariate statistics.
	/// 
	/// @version $Id: StatisticalSummary.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public interface StatisticalSummary
	{

		/// <summary>
		/// Returns the <a href="http://www.xycoon.com/arithmetic_mean.htm">
		/// arithmetic mean </a> of the available values </summary>
		/// <returns> The mean or Double.NaN if no values have been added. </returns>
		double Mean {get;}
		/// <summary>
		/// Returns the variance of the available values. </summary>
		/// <returns> The variance, Double.NaN if no values have been added
		/// or 0.0 for a single value set. </returns>
		double Variance {get;}
		/// <summary>
		/// Returns the standard deviation of the available values. </summary>
		/// <returns> The standard deviation, Double.NaN if no values have been added
		/// or 0.0 for a single value set. </returns>
		double StandardDeviation {get;}
		/// <summary>
		/// Returns the maximum of the available values </summary>
		/// <returns> The max or Double.NaN if no values have been added. </returns>
		double Max {get;}
		/// <summary>
		/// Returns the minimum of the available values </summary>
		/// <returns> The min or Double.NaN if no values have been added. </returns>
		double Min {get;}
		/// <summary>
		/// Returns the number of available values </summary>
		/// <returns> The number of available values </returns>
		long N {get;}
		/// <summary>
		/// Returns the sum of the values that have been added to Univariate. </summary>
		/// <returns> The sum or Double.NaN if no values have been added </returns>
		double Sum {get;}

	}

}
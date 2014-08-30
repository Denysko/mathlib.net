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

	using RealMatrix = org.apache.commons.math3.linear.RealMatrix;

	/// <summary>
	///  Reporting interface for basic multivariate statistics.
	/// 
	/// @since 1.2
	/// @version $Id: StatisticalMultivariateSummary.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public interface StatisticalMultivariateSummary
	{

		/// <summary>
		/// Returns the dimension of the data </summary>
		/// <returns> The dimension of the data </returns>
		int Dimension {get;}

		/// <summary>
		/// Returns an array whose i<sup>th</sup> entry is the
		/// mean of the i<sup>th</sup> entries of the arrays
		/// that correspond to each multivariate sample
		/// </summary>
		/// <returns> the array of component means </returns>
		double[] Mean {get;}

		/// <summary>
		/// Returns the covariance of the available values. </summary>
		/// <returns> The covariance, null if no multivariate sample
		/// have been added or a zeroed matrix for a single value set. </returns>
		RealMatrix Covariance {get;}

		/// <summary>
		/// Returns an array whose i<sup>th</sup> entry is the
		/// standard deviation of the i<sup>th</sup> entries of the arrays
		/// that correspond to each multivariate sample
		/// </summary>
		/// <returns> the array of component standard deviations </returns>
		double[] StandardDeviation {get;}

		/// <summary>
		/// Returns an array whose i<sup>th</sup> entry is the
		/// maximum of the i<sup>th</sup> entries of the arrays
		/// that correspond to each multivariate sample
		/// </summary>
		/// <returns> the array of component maxima </returns>
		double[] Max {get;}

		/// <summary>
		/// Returns an array whose i<sup>th</sup> entry is the
		/// minimum of the i<sup>th</sup> entries of the arrays
		/// that correspond to each multivariate sample
		/// </summary>
		/// <returns> the array of component minima </returns>
		double[] Min {get;}

		/// <summary>
		/// Returns the number of available values </summary>
		/// <returns> The number of available values </returns>
		long N {get;}

		/// <summary>
		/// Returns an array whose i<sup>th</sup> entry is the
		/// geometric mean of the i<sup>th</sup> entries of the arrays
		/// that correspond to each multivariate sample
		/// </summary>
		/// <returns> the array of component geometric means </returns>
		double[] GeometricMean {get;}

		/// <summary>
		/// Returns an array whose i<sup>th</sup> entry is the
		/// sum of the i<sup>th</sup> entries of the arrays
		/// that correspond to each multivariate sample
		/// </summary>
		/// <returns> the array of component sums </returns>
		double[] Sum {get;}

		/// <summary>
		/// Returns an array whose i<sup>th</sup> entry is the
		/// sum of squares of the i<sup>th</sup> entries of the arrays
		/// that correspond to each multivariate sample
		/// </summary>
		/// <returns> the array of component sums of squares </returns>
		double[] SumSq {get;}

		/// <summary>
		/// Returns an array whose i<sup>th</sup> entry is the
		/// sum of logs of the i<sup>th</sup> entries of the arrays
		/// that correspond to each multivariate sample
		/// </summary>
		/// <returns> the array of component log sums </returns>
		double[] SumLog {get;}

	}

}
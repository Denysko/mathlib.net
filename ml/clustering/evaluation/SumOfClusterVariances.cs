using System.Collections.Generic;

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

namespace mathlib.ml.clustering.evaluation
{

	using mathlib.ml.clustering;
	using DistanceMeasure = mathlib.ml.distance.DistanceMeasure;
	using Variance = mathlib.stat.descriptive.moment.Variance;

	/// <summary>
	/// Computes the sum of intra-cluster distance variances according to the formula:
	/// <pre>
	/// \( score = \sum\limits_{i=1}^n \sigma_i^2 \)
	/// </pre>
	/// where n is the number of clusters and \( \sigma_i^2 \) is the variance of
	/// intra-cluster distances of cluster \( c_i \).
	/// </summary>
	/// @param <T> the type of the clustered points
	/// @version $Id: SumOfClusterVariances.java 1542545 2013-11-16 18:48:48Z tn $
	/// @since 3.3 </param>
	public class SumOfClusterVariances<T> : ClusterEvaluator<T> where T : mathlib.ml.clustering.Clusterable
	{

		/// 
		/// <param name="measure"> the distance measure to use </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SumOfClusterVariances(final mathlib.ml.distance.DistanceMeasure measure)
		public SumOfClusterVariances(DistanceMeasure measure) : base(measure)
		{
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public double score(final java.util.List<? extends mathlib.ml.clustering.Cluster<T>> clusters)
		public override double score<T1>(IList<T1> clusters) where T1 : mathlib.ml.clustering.Cluster<T>
		{
			double varianceSum = 0.0;
			foreach (Cluster<T> cluster in clusters)
			{
				if (!cluster.Points.Empty)
				{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.ml.clustering.Clusterable center = centroidOf(cluster);
					Clusterable center = centroidOf(cluster);

					// compute the distance variance of the current cluster
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.stat.descriptive.moment.Variance stat = new mathlib.stat.descriptive.moment.Variance();
					Variance stat = new Variance();
					foreach (T point in cluster.Points)
					{
						stat.increment(distance(point, center));
					}
					varianceSum += stat.Result;

				}
			}
			return varianceSum;
		}

	}

}
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
	using mathlib.ml.clustering;
	using DistanceMeasure = mathlib.ml.distance.DistanceMeasure;
	using EuclideanDistance = mathlib.ml.distance.EuclideanDistance;

	/// <summary>
	/// Base class for cluster evaluation methods.
	/// </summary>
	/// @param <T> type of the clustered points
	/// @version $Id: ClusterEvaluator.java 1542545 2013-11-16 18:48:48Z tn $
	/// @since 3.3 </param>
	public abstract class ClusterEvaluator<T> where T : mathlib.ml.clustering.Clusterable
	{

		/// <summary>
		/// The distance measure to use when evaluating the cluster. </summary>
		private readonly DistanceMeasure measure;

		/// <summary>
		/// Creates a new cluster evaluator with an <seealso cref="EuclideanDistance"/>
		/// as distance measure.
		/// </summary>
		public ClusterEvaluator() : this(new EuclideanDistance())
		{
		}

		/// <summary>
		/// Creates a new cluster evaluator with the given distance measure. </summary>
		/// <param name="measure"> the distance measure to use </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ClusterEvaluator(final mathlib.ml.distance.DistanceMeasure measure)
		public ClusterEvaluator(DistanceMeasure measure)
		{
			this.measure = measure;
		}

		/// <summary>
		/// Computes the evaluation score for the given list of clusters. </summary>
		/// <param name="clusters"> the clusters to evaluate </param>
		/// <returns> the computed score </returns>
		public abstract double score<T1>(IList<T1> clusters) where T1 : mathlib.ml.clustering.Cluster<T>;

		/// <summary>
		/// Returns whether the first evaluation score is considered to be better
		/// than the second one by this evaluator.
		/// <p>
		/// Specific implementations shall override this method if the returned scores
		/// do not follow the same ordering, i.e. smaller score is better.
		/// </summary>
		/// <param name="score1"> the first score </param>
		/// <param name="score2"> the second score </param>
		/// <returns> {@code true} if the first score is considered to be better, {@code false} otherwise </returns>
		public virtual bool isBetterScore(double score1, double score2)
		{
			return score1 < score2;
		}

		/// <summary>
		/// Calculates the distance between two <seealso cref="Clusterable"/> instances
		/// with the configured <seealso cref="DistanceMeasure"/>.
		/// </summary>
		/// <param name="p1"> the first clusterable </param>
		/// <param name="p2"> the second clusterable </param>
		/// <returns> the distance between the two clusterables </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected double distance(final mathlib.ml.clustering.Clusterable p1, final mathlib.ml.clustering.Clusterable p2)
		protected internal virtual double distance(Clusterable p1, Clusterable p2)
		{
			return measure.compute(p1.Point, p2.Point);
		}

		/// <summary>
		/// Computes the centroid for a cluster.
		/// </summary>
		/// <param name="cluster"> the cluster </param>
		/// <returns> the computed centroid for the cluster,
		/// or {@code null} if the cluster does not contain any points </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected mathlib.ml.clustering.Clusterable centroidOf(final mathlib.ml.clustering.Cluster<T> cluster)
		protected internal virtual Clusterable centroidOf(Cluster<T> cluster)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<T> points = cluster.getPoints();
			IList<T> points = cluster.Points;
			if (points.Count == 0)
			{
				return null;
			}

			// in case the cluster is of type CentroidCluster, no need to compute the centroid
			if (cluster is CentroidCluster)
			{
				return ((CentroidCluster<T>) cluster).Center;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dimension = points.get(0).getPoint().length;
			int dimension = points[0].Point.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] centroid = new double[dimension];
			double[] centroid = new double[dimension];
			foreach (T p in points)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] point = p.getPoint();
				double[] point = p.Point;
				for (int i = 0; i < centroid.Length; i++)
				{
					centroid[i] += point[i];
				}
			}
			for (int i = 0; i < centroid.Length; i++)
			{
				centroid[i] /= points.Count;
			}
			return new DoublePoint(centroid);
		}

	}

}
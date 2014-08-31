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
namespace org.apache.commons.math3.ml.clustering
{


	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using DistanceMeasure = org.apache.commons.math3.ml.distance.DistanceMeasure;
	using EuclideanDistance = org.apache.commons.math3.ml.distance.EuclideanDistance;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// DBSCAN (density-based spatial clustering of applications with noise) algorithm.
	/// <p>
	/// The DBSCAN algorithm forms clusters based on the idea of density connectivity, i.e.
	/// a point p is density connected to another point q, if there exists a chain of
	/// points p<sub>i</sub>, with i = 1 .. n and p<sub>1</sub> = p and p<sub>n</sub> = q,
	/// such that each pair &lt;p<sub>i</sub>, p<sub>i+1</sub>&gt; is directly density-reachable.
	/// A point q is directly density-reachable from point p if it is in the &epsilon;-neighborhood
	/// of this point.
	/// <p>
	/// Any point that is not density-reachable from a formed cluster is treated as noise, and
	/// will thus not be present in the result.
	/// <p>
	/// The algorithm requires two parameters:
	/// <ul>
	///   <li>eps: the distance that defines the &epsilon;-neighborhood of a point
	///   <li>minPoints: the minimum number of density-connected points required to form a cluster
	/// </ul>
	/// </summary>
	/// @param <T> type of the points to cluster </param>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/DBSCAN">DBSCAN (wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://www.dbs.ifi.lmu.de/Publikationen/Papers/KDD-96.final.frame.pdf">
	/// A Density-Based Algorithm for Discovering Clusters in Large Spatial Databases with Noise</a>
	/// @version $Id: DBSCANClusterer.java 1503290 2013-07-15 15:16:29Z sebb $
	/// @since 3.2 </seealso>
	public class DBSCANClusterer<T> : Clusterer<T> where T : Clusterable
	{

		/// <summary>
		/// Maximum radius of the neighborhood to be considered. </summary>
		private readonly double eps;

		/// <summary>
		/// Minimum number of points needed for a cluster. </summary>
		private readonly int minPts;

		/// <summary>
		/// Status of a point during the clustering process. </summary>
		private enum PointStatus
		{
			/// <summary>
			/// The point has is considered to be noise. </summary>
			NOISE,
			/// <summary>
			/// The point is already part of a cluster. </summary>
			PART_OF_CLUSTER
		}

		/// <summary>
		/// Creates a new instance of a DBSCANClusterer.
		/// <p>
		/// The euclidean distance will be used as default distance measure.
		/// </summary>
		/// <param name="eps"> maximum radius of the neighborhood to be considered </param>
		/// <param name="minPts"> minimum number of points needed for a cluster </param>
		/// <exception cref="NotPositiveException"> if {@code eps < 0.0} or {@code minPts < 0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DBSCANClusterer(final double eps, final int minPts) throws org.apache.commons.math3.exception.NotPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public DBSCANClusterer(double eps, int minPts) : this(eps, minPts, new EuclideanDistance())
		{
		}

		/// <summary>
		/// Creates a new instance of a DBSCANClusterer.
		/// </summary>
		/// <param name="eps"> maximum radius of the neighborhood to be considered </param>
		/// <param name="minPts"> minimum number of points needed for a cluster </param>
		/// <param name="measure"> the distance measure to use </param>
		/// <exception cref="NotPositiveException"> if {@code eps < 0.0} or {@code minPts < 0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DBSCANClusterer(final double eps, final int minPts, final org.apache.commons.math3.ml.distance.DistanceMeasure measure) throws org.apache.commons.math3.exception.NotPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public DBSCANClusterer(double eps, int minPts, DistanceMeasure measure) : base(measure)
		{

			if (eps < 0.0d)
			{
				throw new NotPositiveException(eps);
			}
			if (minPts < 0)
			{
				throw new NotPositiveException(minPts);
			}
			this.eps = eps;
			this.minPts = minPts;
		}

		/// <summary>
		/// Returns the maximum radius of the neighborhood to be considered. </summary>
		/// <returns> maximum radius of the neighborhood </returns>
		public virtual double Eps
		{
			get
			{
				return eps;
			}
		}

		/// <summary>
		/// Returns the minimum number of points needed for a cluster. </summary>
		/// <returns> minimum number of points needed for a cluster </returns>
		public virtual int MinPts
		{
			get
			{
				return minPts;
			}
		}

		/// <summary>
		/// Performs DBSCAN cluster analysis.
		/// </summary>
		/// <param name="points"> the points to cluster </param>
		/// <returns> the list of clusters </returns>
		/// <exception cref="NullArgumentException"> if the data points are null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public java.util.List<Cluster<T>> cluster(final java.util.Collection<T> points) throws org.apache.commons.math3.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override IList<Cluster<T>> cluster(ICollection<T> points)
		{

			// sanity checks
			MathUtils.checkNotNull(points);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Cluster<T>> clusters = new java.util.ArrayList<Cluster<T>>();
			IList<Cluster<T>> clusters = new List<Cluster<T>>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<Clusterable, PointStatus> visited = new java.util.HashMap<Clusterable, PointStatus>();
			IDictionary<Clusterable, PointStatus> visited = new Dictionary<Clusterable, PointStatus>();

			foreach (T point in points)
			{
				if (visited[point] != null)
				{
					continue;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<T> neighbors = getNeighbors(point, points);
				IList<T> neighbors = getNeighbors(point, points);
				if (neighbors.Count >= minPts)
				{
					// DBSCAN does not care about center points
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Cluster<T> cluster = new Cluster<T>();
					Cluster<T> cluster = new Cluster<T>();
					clusters.Add(expandCluster(cluster, point, neighbors, points, visited));
				}
				else
				{
					visited[point] = PointStatus.NOISE;
				}
			}

			return clusters;
		}

		/// <summary>
		/// Expands the cluster to include density-reachable items.
		/// </summary>
		/// <param name="cluster"> Cluster to expand </param>
		/// <param name="point"> Point to add to cluster </param>
		/// <param name="neighbors"> List of neighbors </param>
		/// <param name="points"> the data set </param>
		/// <param name="visited"> the set of already visited points </param>
		/// <returns> the expanded cluster </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Cluster<T> expandCluster(final Cluster<T> cluster, final T point, final java.util.List<T> neighbors, final java.util.Collection<T> points, final java.util.Map<Clusterable, PointStatus> visited)
		private Cluster<T> expandCluster(Cluster<T> cluster, T point, IList<T> neighbors, ICollection<T> points, IDictionary<Clusterable, PointStatus> visited)
		{
			cluster.addPoint(point);
			visited[point] = PointStatus.PART_OF_CLUSTER;

			IList<T> seeds = new List<T>(neighbors);
			int index = 0;
			while (index < seeds.Count)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T current = seeds.get(index);
				T current = seeds[index];
				PointStatus pStatus = visited[current];
				// only check non-visited points
				if (pStatus == null)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<T> currentNeighbors = getNeighbors(current, points);
					IList<T> currentNeighbors = getNeighbors(current, points);
					if (currentNeighbors.Count >= minPts)
					{
						seeds = merge(seeds, currentNeighbors);
					}
				}

				if (pStatus != PointStatus.PART_OF_CLUSTER)
				{
					visited[current] = PointStatus.PART_OF_CLUSTER;
					cluster.addPoint(current);
				}

				index++;
			}
			return cluster;
		}

		/// <summary>
		/// Returns a list of density-reachable neighbors of a {@code point}.
		/// </summary>
		/// <param name="point"> the point to look for </param>
		/// <param name="points"> possible neighbors </param>
		/// <returns> the List of neighbors </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private java.util.List<T> getNeighbors(final T point, final java.util.Collection<T> points)
		private IList<T> getNeighbors(T point, ICollection<T> points)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<T> neighbors = new java.util.ArrayList<T>();
			IList<T> neighbors = new List<T>();
			foreach (T neighbor in points)
			{
				if (point != neighbor && distance(neighbor, point) <= eps)
				{
					neighbors.Add(neighbor);
				}
			}
			return neighbors;
		}

		/// <summary>
		/// Merges two lists together.
		/// </summary>
		/// <param name="one"> first list </param>
		/// <param name="two"> second list </param>
		/// <returns> merged lists </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private java.util.List<T> merge(final java.util.List<T> one, final java.util.List<T> two)
		private IList<T> merge(IList<T> one, IList<T> two)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Set<T> oneSet = new java.util.HashSet<T>(one);
			Set<T> oneSet = new HashSet<T>(one);
			foreach (T item in two)
			{
				if (!oneSet.contains(item))
				{
					one.Add(item);
				}
			}
			return one;
		}
	}

}
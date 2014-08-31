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


	using ConvergenceException = org.apache.commons.math3.exception.ConvergenceException;
	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using DistanceMeasure = org.apache.commons.math3.ml.distance.DistanceMeasure;
	using EuclideanDistance = org.apache.commons.math3.ml.distance.EuclideanDistance;
	using JDKRandomGenerator = org.apache.commons.math3.random.JDKRandomGenerator;
	using RandomGenerator = org.apache.commons.math3.random.RandomGenerator;
	using Variance = org.apache.commons.math3.stat.descriptive.moment.Variance;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// Clustering algorithm based on David Arthur and Sergei Vassilvitski k-means++ algorithm. </summary>
	/// @param <T> type of the points to cluster </param>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/K-means%2B%2B">K-means++ (wikipedia)</a>
	/// @version $Id: KMeansPlusPlusClusterer.java 1503290 2013-07-15 15:16:29Z sebb $
	/// @since 3.2 </seealso>
	public class KMeansPlusPlusClusterer<T> : Clusterer<T> where T : Clusterable
	{

		/// <summary>
		/// Strategies to use for replacing an empty cluster. </summary>
		public enum EmptyClusterStrategy
		{

			/// <summary>
			/// Split the cluster with largest distance variance. </summary>
			LARGEST_VARIANCE,

			/// <summary>
			/// Split the cluster with largest number of points. </summary>
			LARGEST_POINTS_NUMBER,

			/// <summary>
			/// Create a cluster around the point farthest from its centroid. </summary>
			FARTHEST_POINT,

			/// <summary>
			/// Generate an error. </summary>
			ERROR

		}

		/// <summary>
		/// The number of clusters. </summary>
		private readonly int k;

		/// <summary>
		/// The maximum number of iterations. </summary>
		private readonly int maxIterations;

		/// <summary>
		/// Random generator for choosing initial centers. </summary>
		private readonly RandomGenerator random;

		/// <summary>
		/// Selected strategy for empty clusters. </summary>
		private readonly EmptyClusterStrategy emptyStrategy;

		/// <summary>
		/// Build a clusterer.
		/// <p>
		/// The default strategy for handling empty clusters that may appear during
		/// algorithm iterations is to split the cluster with largest distance variance.
		/// <p>
		/// The euclidean distance will be used as default distance measure.
		/// </summary>
		/// <param name="k"> the number of clusters to split the data into </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public KMeansPlusPlusClusterer(final int k)
		public KMeansPlusPlusClusterer(int k) : this(k, -1)
		{
		}

		/// <summary>
		/// Build a clusterer.
		/// <p>
		/// The default strategy for handling empty clusters that may appear during
		/// algorithm iterations is to split the cluster with largest distance variance.
		/// <p>
		/// The euclidean distance will be used as default distance measure.
		/// </summary>
		/// <param name="k"> the number of clusters to split the data into </param>
		/// <param name="maxIterations"> the maximum number of iterations to run the algorithm for.
		///   If negative, no maximum will be used. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public KMeansPlusPlusClusterer(final int k, final int maxIterations)
		public KMeansPlusPlusClusterer(int k, int maxIterations) : this(k, maxIterations, new EuclideanDistance())
		{
		}

		/// <summary>
		/// Build a clusterer.
		/// <p>
		/// The default strategy for handling empty clusters that may appear during
		/// algorithm iterations is to split the cluster with largest distance variance.
		/// </summary>
		/// <param name="k"> the number of clusters to split the data into </param>
		/// <param name="maxIterations"> the maximum number of iterations to run the algorithm for.
		///   If negative, no maximum will be used. </param>
		/// <param name="measure"> the distance measure to use </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public KMeansPlusPlusClusterer(final int k, final int maxIterations, final org.apache.commons.math3.ml.distance.DistanceMeasure measure)
		public KMeansPlusPlusClusterer(int k, int maxIterations, DistanceMeasure measure) : this(k, maxIterations, measure, new JDKRandomGenerator())
		{
		}

		/// <summary>
		/// Build a clusterer.
		/// <p>
		/// The default strategy for handling empty clusters that may appear during
		/// algorithm iterations is to split the cluster with largest distance variance.
		/// </summary>
		/// <param name="k"> the number of clusters to split the data into </param>
		/// <param name="maxIterations"> the maximum number of iterations to run the algorithm for.
		///   If negative, no maximum will be used. </param>
		/// <param name="measure"> the distance measure to use </param>
		/// <param name="random"> random generator to use for choosing initial centers </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public KMeansPlusPlusClusterer(final int k, final int maxIterations, final org.apache.commons.math3.ml.distance.DistanceMeasure measure, final org.apache.commons.math3.random.RandomGenerator random)
		public KMeansPlusPlusClusterer(int k, int maxIterations, DistanceMeasure measure, RandomGenerator random) : this(k, maxIterations, measure, random, EmptyClusterStrategy.LARGEST_VARIANCE)
		{
		}

		/// <summary>
		/// Build a clusterer.
		/// </summary>
		/// <param name="k"> the number of clusters to split the data into </param>
		/// <param name="maxIterations"> the maximum number of iterations to run the algorithm for.
		///   If negative, no maximum will be used. </param>
		/// <param name="measure"> the distance measure to use </param>
		/// <param name="random"> random generator to use for choosing initial centers </param>
		/// <param name="emptyStrategy"> strategy to use for handling empty clusters that
		/// may appear during algorithm iterations </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public KMeansPlusPlusClusterer(final int k, final int maxIterations, final org.apache.commons.math3.ml.distance.DistanceMeasure measure, final org.apache.commons.math3.random.RandomGenerator random, final EmptyClusterStrategy emptyStrategy)
		public KMeansPlusPlusClusterer(int k, int maxIterations, DistanceMeasure measure, RandomGenerator random, EmptyClusterStrategy emptyStrategy) : base(measure)
		{
			this.k = k;
			this.maxIterations = maxIterations;
			this.random = random;
			this.emptyStrategy = emptyStrategy;
		}

		/// <summary>
		/// Return the number of clusters this instance will use. </summary>
		/// <returns> the number of clusters </returns>
		public virtual int K
		{
			get
			{
				return k;
			}
		}

		/// <summary>
		/// Returns the maximum number of iterations this instance will use. </summary>
		/// <returns> the maximum number of iterations, or -1 if no maximum is set </returns>
		public virtual int MaxIterations
		{
			get
			{
				return maxIterations;
			}
		}

		/// <summary>
		/// Returns the random generator this instance will use. </summary>
		/// <returns> the random generator </returns>
		public virtual RandomGenerator RandomGenerator
		{
			get
			{
				return random;
			}
		}

		/// <summary>
		/// Returns the <seealso cref="EmptyClusterStrategy"/> used by this instance. </summary>
		/// <returns> the <seealso cref="EmptyClusterStrategy"/> </returns>
		public virtual EmptyClusterStrategy EmptyClusterStrategy
		{
			get
			{
				return emptyStrategy;
			}
		}

		/// <summary>
		/// Runs the K-means++ clustering algorithm.
		/// </summary>
		/// <param name="points"> the points to cluster </param>
		/// <returns> a list of clusters containing the points </returns>
		/// <exception cref="MathIllegalArgumentException"> if the data points are null or the number
		///     of clusters is larger than the number of data points </exception>
		/// <exception cref="ConvergenceException"> if an empty cluster is encountered and the
		/// <seealso cref="#emptyStrategy"/> is set to {@code ERROR} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public java.util.List<CentroidCluster<T>> cluster(final java.util.Collection<T> points) throws org.apache.commons.math3.exception.MathIllegalArgumentException, org.apache.commons.math3.exception.ConvergenceException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override IList<CentroidCluster<T>> cluster(ICollection<T> points)
		{

			// sanity checks
			MathUtils.checkNotNull(points);

			// number of clusters has to be smaller or equal the number of data points
			if (points.Count < k)
			{
				throw new NumberIsTooSmallException(points.Count, k, false);
			}

			// create the initial clusters
			IList<CentroidCluster<T>> clusters = chooseInitialCenters(points);

			// create an array containing the latest assignment of a point to a cluster
			// no need to initialize the array, as it will be filled with the first assignment
			int[] assignments = new int[points.Count];
			assignPointsToClusters(clusters, points, assignments);

			// iterate through updating the centers until we're done
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int max = (maxIterations < 0) ? Integer.MAX_VALUE : maxIterations;
			int max = (maxIterations < 0) ? int.MaxValue : maxIterations;
			for (int count = 0; count < max; count++)
			{
				bool emptyCluster = false;
				IList<CentroidCluster<T>> newClusters = new List<CentroidCluster<T>>();
				foreach (CentroidCluster<T> cluster in clusters)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Clusterable newCenter;
					Clusterable newCenter;
					if (cluster.Points.Empty)
					{
						switch (emptyStrategy)
						{
							case org.apache.commons.math3.ml.clustering.KMeansPlusPlusClusterer.EmptyClusterStrategy.LARGEST_VARIANCE:
								newCenter = getPointFromLargestVarianceCluster(clusters);
								break;
							case org.apache.commons.math3.ml.clustering.KMeansPlusPlusClusterer.EmptyClusterStrategy.LARGEST_POINTS_NUMBER:
								newCenter = getPointFromLargestNumberCluster(clusters);
								break;
							case org.apache.commons.math3.ml.clustering.KMeansPlusPlusClusterer.EmptyClusterStrategy.FARTHEST_POINT:
								newCenter = getFarthestPoint(clusters);
								break;
							default :
								throw new ConvergenceException(LocalizedFormats.EMPTY_CLUSTER_IN_K_MEANS);
						}
						emptyCluster = true;
					}
					else
					{
						newCenter = centroidOf(cluster.Points, cluster.Center.Point.length);
					}
					newClusters.Add(new CentroidCluster<T>(newCenter));
				}
				int changes = assignPointsToClusters(newClusters, points, assignments);
				clusters = newClusters;

				// if there were no more changes in the point-to-cluster assignment
				// and there are no empty clusters left, return the current clusters
				if (changes == 0 && !emptyCluster)
				{
					return clusters;
				}
			}
			return clusters;
		}

		/// <summary>
		/// Adds the given points to the closest <seealso cref="Cluster"/>.
		/// </summary>
		/// <param name="clusters"> the <seealso cref="Cluster"/>s to add the points to </param>
		/// <param name="points"> the points to add to the given <seealso cref="Cluster"/>s </param>
		/// <param name="assignments"> points assignments to clusters </param>
		/// <returns> the number of points assigned to different clusters as the iteration before </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private int assignPointsToClusters(final java.util.List<CentroidCluster<T>> clusters, final java.util.Collection<T> points, final int[] assignments)
		private int assignPointsToClusters(IList<CentroidCluster<T>> clusters, ICollection<T> points, int[] assignments)
		{
			int assignedDifferently = 0;
			int pointIndex = 0;
			foreach (T p in points)
			{
				int clusterIndex = getNearestCluster(clusters, p);
				if (clusterIndex != assignments[pointIndex])
				{
					assignedDifferently++;
				}

				CentroidCluster<T> cluster = clusters[clusterIndex];
				cluster.addPoint(p);
				assignments[pointIndex++] = clusterIndex;
			}

			return assignedDifferently;
		}

		/// <summary>
		/// Use K-means++ to choose the initial centers.
		/// </summary>
		/// <param name="points"> the points to choose the initial centers from </param>
		/// <returns> the initial centers </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private java.util.List<CentroidCluster<T>> chooseInitialCenters(final java.util.Collection<T> points)
		private IList<CentroidCluster<T>> chooseInitialCenters(ICollection<T> points)
		{

			// Convert to list for indexed access. Make it unmodifiable, since removal of items
			// would screw up the logic of this method.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<T> pointList = java.util.Collections.unmodifiableList(new java.util.ArrayList<T> (points));
			IList<T> pointList = Collections.unmodifiableList(new List<T> (points));

			// The number of points in the list.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numPoints = pointList.size();
			int numPoints = pointList.Count;

			// Set the corresponding element in this array to indicate when
			// elements of pointList are no longer available.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean[] taken = new boolean[numPoints];
			bool[] taken = new bool[numPoints];

			// The resulting list of initial centers.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<CentroidCluster<T>> resultSet = new java.util.ArrayList<CentroidCluster<T>>();
			IList<CentroidCluster<T>> resultSet = new List<CentroidCluster<T>>();

			// Choose one center uniformly at random from among the data points.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int firstPointIndex = random.nextInt(numPoints);
			int firstPointIndex = random.Next(numPoints);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T firstPoint = pointList.get(firstPointIndex);
			T firstPoint = pointList[firstPointIndex];

			resultSet.Add(new CentroidCluster<T>(firstPoint));

			// Must mark it as taken
			taken[firstPointIndex] = true;

			// To keep track of the minimum distance squared of elements of
			// pointList to elements of resultSet.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] minDistSquared = new double[numPoints];
			double[] minDistSquared = new double[numPoints];

			// Initialize the elements.  Since the only point in resultSet is firstPoint,
			// this is very easy.
			for (int i = 0; i < numPoints; i++)
			{
				if (i != firstPointIndex) // That point isn't considered
				{
					double d = distance(firstPoint, pointList[i]);
					minDistSquared[i] = d * d;
				}
			}

			while (resultSet.Count < k)
			{

				// Sum up the squared distances for the points in pointList not
				// already taken.
				double distSqSum = 0.0;

				for (int i = 0; i < numPoints; i++)
				{
					if (!taken[i])
					{
						distSqSum += minDistSquared[i];
					}
				}

				// Add one new data point as a center. Each point x is chosen with
				// probability proportional to D(x)2
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double r = random.nextDouble() * distSqSum;
				double r = random.NextDouble() * distSqSum;

				// The index of the next point to be added to the resultSet.
				int nextPointIndex = -1;

				// Sum through the squared min distances again, stopping when
				// sum >= r.
				double sum = 0.0;
				for (int i = 0; i < numPoints; i++)
				{
					if (!taken[i])
					{
						sum += minDistSquared[i];
						if (sum >= r)
						{
							nextPointIndex = i;
							break;
						}
					}
				}

				// If it's not set to >= 0, the point wasn't found in the previous
				// for loop, probably because distances are extremely small.  Just pick
				// the last available point.
				if (nextPointIndex == -1)
				{
					for (int i = numPoints - 1; i >= 0; i--)
					{
						if (!taken[i])
						{
							nextPointIndex = i;
							break;
						}
					}
				}

				// We found one.
				if (nextPointIndex >= 0)
				{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T p = pointList.get(nextPointIndex);
					T p = pointList[nextPointIndex];

					resultSet.Add(new CentroidCluster<T> (p));

					// Mark it as taken.
					taken[nextPointIndex] = true;

					if (resultSet.Count < k)
					{
						// Now update elements of minDistSquared.  We only have to compute
						// the distance to the new center to do this.
						for (int j = 0; j < numPoints; j++)
						{
							// Only have to worry about the points still not taken.
							if (!taken[j])
							{
								double d = distance(p, pointList[j]);
								double d2 = d * d;
								if (d2 < minDistSquared[j])
								{
									minDistSquared[j] = d2;
								}
							}
						}
					}

				}
				else
				{
					// None found --
					// Break from the while loop to prevent
					// an infinite loop.
					break;
				}
			}

			return resultSet;
		}

		/// <summary>
		/// Get a random point from the <seealso cref="Cluster"/> with the largest distance variance.
		/// </summary>
		/// <param name="clusters"> the <seealso cref="Cluster"/>s to search </param>
		/// <returns> a random point from the selected cluster </returns>
		/// <exception cref="ConvergenceException"> if clusters are all empty </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private T getPointFromLargestVarianceCluster(final java.util.Collection<CentroidCluster<T>> clusters) throws org.apache.commons.math3.exception.ConvergenceException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private T getPointFromLargestVarianceCluster(ICollection<CentroidCluster<T>> clusters)
		{

			double maxVariance = double.NegativeInfinity;
			Cluster<T> selected = null;
			foreach (CentroidCluster<T> cluster in clusters)
			{
				if (!cluster.Points.Empty)
				{

					// compute the distance variance of the current cluster
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Clusterable center = cluster.getCenter();
					Clusterable center = cluster.Center;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.stat.descriptive.moment.Variance stat = new org.apache.commons.math3.stat.descriptive.moment.Variance();
					Variance stat = new Variance();
					foreach (T point in cluster.Points)
					{
						stat.increment(distance(point, center));
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double variance = stat.getResult();
					double variance = stat.Result;

					// select the cluster with the largest variance
					if (variance > maxVariance)
					{
						maxVariance = variance;
						selected = cluster;
					}

				}
			}

			// did we find at least one non-empty cluster ?
			if (selected == null)
			{
				throw new ConvergenceException(LocalizedFormats.EMPTY_CLUSTER_IN_K_MEANS);
			}

			// extract a random point from the cluster
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<T> selectedPoints = selected.getPoints();
			IList<T> selectedPoints = selected.Points;
			return selectedPoints.Remove(random.Next(selectedPoints.Count));

		}

		/// <summary>
		/// Get a random point from the <seealso cref="Cluster"/> with the largest number of points
		/// </summary>
		/// <param name="clusters"> the <seealso cref="Cluster"/>s to search </param>
		/// <returns> a random point from the selected cluster </returns>
		/// <exception cref="ConvergenceException"> if clusters are all empty </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private T getPointFromLargestNumberCluster(final java.util.Collection<? extends Cluster<T>> clusters) throws org.apache.commons.math3.exception.ConvergenceException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private T getPointFromLargestNumberCluster<T1>(ICollection<T1> clusters) where T1 : Cluster<T>
		{

			int maxNumber = 0;
			Cluster<T> selected = null;
			foreach (Cluster<T> cluster in clusters)
			{

				// get the number of points of the current cluster
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int number = cluster.getPoints().size();
				int number = cluster.Points.size();

				// select the cluster with the largest number of points
				if (number > maxNumber)
				{
					maxNumber = number;
					selected = cluster;
				}

			}

			// did we find at least one non-empty cluster ?
			if (selected == null)
			{
				throw new ConvergenceException(LocalizedFormats.EMPTY_CLUSTER_IN_K_MEANS);
			}

			// extract a random point from the cluster
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<T> selectedPoints = selected.getPoints();
			IList<T> selectedPoints = selected.Points;
			return selectedPoints.Remove(random.Next(selectedPoints.Count));

		}

		/// <summary>
		/// Get the point farthest to its cluster center
		/// </summary>
		/// <param name="clusters"> the <seealso cref="Cluster"/>s to search </param>
		/// <returns> point farthest to its cluster center </returns>
		/// <exception cref="ConvergenceException"> if clusters are all empty </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private T getFarthestPoint(final java.util.Collection<CentroidCluster<T>> clusters) throws org.apache.commons.math3.exception.ConvergenceException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private T getFarthestPoint(ICollection<CentroidCluster<T>> clusters)
		{

			double maxDistance = double.NegativeInfinity;
			Cluster<T> selectedCluster = null;
			int selectedPoint = -1;
			foreach (CentroidCluster<T> cluster in clusters)
			{

				// get the farthest point
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Clusterable center = cluster.getCenter();
				Clusterable center = cluster.Center;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<T> points = cluster.getPoints();
				IList<T> points = cluster.Points;
				for (int i = 0; i < points.Count; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double distance = distance(points.get(i), center);
					double distance = distance(points[i], center);
					if (distance > maxDistance)
					{
						maxDistance = distance;
						selectedCluster = cluster;
						selectedPoint = i;
					}
				}

			}

			// did we find at least one non-empty cluster ?
			if (selectedCluster == null)
			{
				throw new ConvergenceException(LocalizedFormats.EMPTY_CLUSTER_IN_K_MEANS);
			}

			return selectedCluster.Points.Remove(selectedPoint);

		}

		/// <summary>
		/// Returns the nearest <seealso cref="Cluster"/> to the given point
		/// </summary>
		/// <param name="clusters"> the <seealso cref="Cluster"/>s to search </param>
		/// <param name="point"> the point to find the nearest <seealso cref="Cluster"/> for </param>
		/// <returns> the index of the nearest <seealso cref="Cluster"/> to the given point </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private int getNearestCluster(final java.util.Collection<CentroidCluster<T>> clusters, final T point)
		private int getNearestCluster(ICollection<CentroidCluster<T>> clusters, T point)
		{
			double minDistance = double.MaxValue;
			int clusterIndex = 0;
			int minCluster = 0;
			foreach (CentroidCluster<T> c in clusters)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double distance = distance(point, c.getCenter());
				double distance = distance(point, c.Center);
				if (distance < minDistance)
				{
					minDistance = distance;
					minCluster = clusterIndex;
				}
				clusterIndex++;
			}
			return minCluster;
		}

		/// <summary>
		/// Computes the centroid for a set of points.
		/// </summary>
		/// <param name="points"> the set of points </param>
		/// <param name="dimension"> the point dimension </param>
		/// <returns> the computed centroid for the set of points </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Clusterable centroidOf(final java.util.Collection<T> points, final int dimension)
		private Clusterable centroidOf(ICollection<T> points, int dimension)
		{
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
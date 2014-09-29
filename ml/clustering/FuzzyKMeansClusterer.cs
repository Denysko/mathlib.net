using System;
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
namespace mathlib.ml.clustering
{


	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using MathIllegalStateException = mathlib.exception.MathIllegalStateException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using MatrixUtils = mathlib.linear.MatrixUtils;
	using RealMatrix = mathlib.linear.RealMatrix;
	using DistanceMeasure = mathlib.ml.distance.DistanceMeasure;
	using EuclideanDistance = mathlib.ml.distance.EuclideanDistance;
	using JDKRandomGenerator = mathlib.random.JDKRandomGenerator;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using FastMath = mathlib.util.FastMath;
	using MathArrays = mathlib.util.MathArrays;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// Fuzzy K-Means clustering algorithm.
	/// <p>
	/// The Fuzzy K-Means algorithm is a variation of the classical K-Means algorithm, with the
	/// major difference that a single data point is not uniquely assigned to a single cluster.
	/// Instead, each point i has a set of weights u<sub>ij</sub> which indicate the degree of membership
	/// to the cluster j.
	/// <p>
	/// The algorithm then tries to minimize the objective function:
	/// <pre>
	/// J = &#8721;<sub>i=1..C</sub>&#8721;<sub>k=1..N</sub> u<sub>ik</sub><sup>m</sup>d<sub>ik</sub><sup>2</sup>
	/// </pre>
	/// with d<sub>ik</sub> being the distance between data point i and the cluster center k.
	/// <p>
	/// The algorithm requires two parameters:
	/// <ul>
	///   <li>k: the number of clusters
	///   <li>fuzziness: determines the level of cluster fuzziness, larger values lead to fuzzier clusters
	/// </ul>
	/// Additional, optional parameters:
	/// <ul>
	///   <li>maxIterations: the maximum number of iterations
	///   <li>epsilon: the convergence criteria, default is 1e-3
	/// </ul>
	/// <p>
	/// The fuzzy variant of the K-Means algorithm is more robust with regard to the selection
	/// of the initial cluster centers.
	/// </summary>
	/// @param <T> type of the points to cluster
	/// @version $Id: FuzzyKMeansClusterer.java 1512043 2013-08-08 21:27:57Z tn $
	/// @since 3.3 </param>
	public class FuzzyKMeansClusterer<T> : Clusterer<T> where T : Clusterable
	{

		/// <summary>
		/// The default value for the convergence criteria. </summary>
		private const double DEFAULT_EPSILON = 1e-3;

		/// <summary>
		/// The number of clusters. </summary>
		private readonly int k;

		/// <summary>
		/// The maximum number of iterations. </summary>
		private readonly int maxIterations;

		/// <summary>
		/// The fuzziness factor. </summary>
		private readonly double fuzziness;

		/// <summary>
		/// The convergence criteria. </summary>
		private readonly double epsilon;

		/// <summary>
		/// Random generator for choosing initial centers. </summary>
		private readonly RandomGenerator random;

		/// <summary>
		/// The membership matrix. </summary>
		private double[][] membershipMatrix;

		/// <summary>
		/// The list of points used in the last call to <seealso cref="#cluster(Collection)"/>. </summary>
		private IList<T> points;

		/// <summary>
		/// The list of clusters resulting from the last call to <seealso cref="#cluster(Collection)"/>. </summary>
		private IList<CentroidCluster<T>> clusters;

		/// <summary>
		/// Creates a new instance of a FuzzyKMeansClusterer.
		/// <p>
		/// The euclidean distance will be used as default distance measure.
		/// </summary>
		/// <param name="k"> the number of clusters to split the data into </param>
		/// <param name="fuzziness"> the fuzziness factor, must be &gt; 1.0 </param>
		/// <exception cref="NumberIsTooSmallException"> if {@code fuzziness <= 1.0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FuzzyKMeansClusterer(final int k, final double fuzziness) throws mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public FuzzyKMeansClusterer(int k, double fuzziness) : this(k, fuzziness, -1, new EuclideanDistance())
		{
		}

		/// <summary>
		/// Creates a new instance of a FuzzyKMeansClusterer.
		/// </summary>
		/// <param name="k"> the number of clusters to split the data into </param>
		/// <param name="fuzziness"> the fuzziness factor, must be &gt; 1.0 </param>
		/// <param name="maxIterations"> the maximum number of iterations to run the algorithm for.
		///   If negative, no maximum will be used. </param>
		/// <param name="measure"> the distance measure to use </param>
		/// <exception cref="NumberIsTooSmallException"> if {@code fuzziness <= 1.0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FuzzyKMeansClusterer(final int k, final double fuzziness, final int maxIterations, final mathlib.ml.distance.DistanceMeasure measure) throws mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public FuzzyKMeansClusterer(int k, double fuzziness, int maxIterations, DistanceMeasure measure) : this(k, fuzziness, maxIterations, measure, DEFAULT_EPSILON, new JDKRandomGenerator())
		{
		}

		/// <summary>
		/// Creates a new instance of a FuzzyKMeansClusterer.
		/// </summary>
		/// <param name="k"> the number of clusters to split the data into </param>
		/// <param name="fuzziness"> the fuzziness factor, must be &gt; 1.0 </param>
		/// <param name="maxIterations"> the maximum number of iterations to run the algorithm for.
		///   If negative, no maximum will be used. </param>
		/// <param name="measure"> the distance measure to use </param>
		/// <param name="epsilon"> the convergence criteria (default is 1e-3) </param>
		/// <param name="random"> random generator to use for choosing initial centers </param>
		/// <exception cref="NumberIsTooSmallException"> if {@code fuzziness <= 1.0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FuzzyKMeansClusterer(final int k, final double fuzziness, final int maxIterations, final mathlib.ml.distance.DistanceMeasure measure, final double epsilon, final mathlib.random.RandomGenerator random) throws mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public FuzzyKMeansClusterer(int k, double fuzziness, int maxIterations, DistanceMeasure measure, double epsilon, RandomGenerator random) : base(measure)
		{


			if (fuzziness <= 1.0d)
			{
				throw new NumberIsTooSmallException(fuzziness, 1.0, false);
			}
			this.k = k;
			this.fuzziness = fuzziness;
			this.maxIterations = maxIterations;
			this.epsilon = epsilon;
			this.random = random;

			this.membershipMatrix = null;
			this.points = null;
			this.clusters = null;
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
		/// Returns the fuzziness factor used by this instance. </summary>
		/// <returns> the fuzziness factor </returns>
		public virtual double Fuzziness
		{
			get
			{
				return fuzziness;
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
		/// Returns the convergence criteria used by this instance. </summary>
		/// <returns> the convergence criteria </returns>
		public virtual double Epsilon
		{
			get
			{
				return epsilon;
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
		/// Returns the {@code nxk} membership matrix, where {@code n} is the number
		/// of data points and {@code k} the number of clusters.
		/// <p>
		/// The element U<sub>i,j</sub> represents the membership value for data point {@code i}
		/// to cluster {@code j}.
		/// </summary>
		/// <returns> the membership matrix </returns>
		/// <exception cref="MathIllegalStateException"> if <seealso cref="#cluster(Collection)"/> has not been called before </exception>
		public virtual RealMatrix MembershipMatrix
		{
			get
			{
				if (membershipMatrix == null)
				{
					throw new MathIllegalStateException();
				}
				return MatrixUtils.createRealMatrix(membershipMatrix);
			}
		}

		/// <summary>
		/// Returns an unmodifiable list of the data points used in the last
		/// call to <seealso cref="#cluster(Collection)"/>. </summary>
		/// <returns> the list of data points, or {@code null} if <seealso cref="#cluster(Collection)"/> has
		///   not been called before. </returns>
		public virtual IList<T> DataPoints
		{
			get
			{
				return points;
			}
		}

		/// <summary>
		/// Returns the list of clusters resulting from the last call to <seealso cref="#cluster(Collection)"/>. </summary>
		/// <returns> the list of clusters, or {@code null} if <seealso cref="#cluster(Collection)"/> has
		///   not been called before. </returns>
		public virtual IList<CentroidCluster<T>> Clusters
		{
			get
			{
				return clusters;
			}
		}

		/// <summary>
		/// Get the value of the objective function. </summary>
		/// <returns> the objective function evaluation as double value </returns>
		/// <exception cref="MathIllegalStateException"> if <seealso cref="#cluster(Collection)"/> has not been called before </exception>
		public virtual double ObjectiveFunctionValue
		{
			get
			{
				if (points == null || clusters == null)
				{
					throw new MathIllegalStateException();
				}
    
				int i = 0;
				double objFunction = 0.0;
				foreach (T point in points)
				{
					int j = 0;
					foreach (CentroidCluster<T> cluster in clusters)
					{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double dist = distance(point, cluster.getCenter());
						double dist = distance(point, cluster.Center);
						objFunction += (dist * dist) * FastMath.pow(membershipMatrix[i][j], fuzziness);
						j++;
					}
					i++;
				}
				return objFunction;
			}
		}

		/// <summary>
		/// Performs Fuzzy K-Means cluster analysis.
		/// </summary>
		/// <param name="dataPoints"> the points to cluster </param>
		/// <returns> the list of clusters </returns>
		/// <exception cref="MathIllegalArgumentException"> if the data points are null or the number
		///     of clusters is larger than the number of data points </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public java.util.List<CentroidCluster<T>> cluster(final java.util.Collection<T> dataPoints) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override IList<CentroidCluster<T>> cluster(ICollection<T> dataPoints)
		{

			// sanity checks
			MathUtils.checkNotNull(dataPoints);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = dataPoints.size();
			int size = dataPoints.Count;

			// number of clusters has to be smaller or equal the number of data points
			if (size < k)
			{
				throw new NumberIsTooSmallException(size, k, false);
			}

			// copy the input collection to an unmodifiable list with indexed access
			points = Collections.unmodifiableList(new List<T>(dataPoints));
			clusters = new List<CentroidCluster<T>>();
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: membershipMatrix = new double[size][k];
			membershipMatrix = RectangularArrays.ReturnRectangularDoubleArray(size, k);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] oldMatrix = new double[size][k];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] oldMatrix = new double[size][k];
			double[][] oldMatrix = RectangularArrays.ReturnRectangularDoubleArray(size, k);

			// if no points are provided, return an empty list of clusters
			if (size == 0)
			{
				return clusters;
			}

			initializeMembershipMatrix();

			// there is at least one point
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pointDimension = points.get(0).getPoint().length;
			int pointDimension = points[0].Point.Length;
			for (int i = 0; i < k; i++)
			{
				clusters.Add(new CentroidCluster<T>(new DoublePoint(new double[pointDimension])));
			}

			int iteration = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int max = (maxIterations < 0) ? Integer.MAX_VALUE : maxIterations;
			int max = (maxIterations < 0) ? int.MaxValue : maxIterations;
			double difference = 0.0;

			do
			{
				saveMembershipMatrix(oldMatrix);
				updateClusterCenters();
				updateMembershipMatrix();
				difference = calculateMaxMembershipChange(oldMatrix);
			} while (difference > epsilon && ++iteration < max);

			return clusters;
		}

		/// <summary>
		/// Update the cluster centers.
		/// </summary>
		private void updateClusterCenters()
		{
			int j = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<CentroidCluster<T>> newClusters = new java.util.ArrayList<CentroidCluster<T>>(k);
			IList<CentroidCluster<T>> newClusters = new List<CentroidCluster<T>>(k);
			foreach (CentroidCluster<T> cluster in clusters)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Clusterable center = cluster.getCenter();
				Clusterable center = cluster.Center;
				int i = 0;
				double[] arr = new double[center.Point.Length];
				double sum = 0.0;
				foreach (T point in points)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double u = mathlib.util.FastMath.pow(membershipMatrix[i][j], fuzziness);
					double u = FastMath.pow(membershipMatrix[i][j], fuzziness);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pointArr = point.getPoint();
					double[] pointArr = point.Point;
					for (int idx = 0; idx < arr.Length; idx++)
					{
						arr[idx] += u * pointArr[idx];
					}
					sum += u;
					i++;
				}
				MathArrays.scaleInPlace(1.0 / sum, arr);
				newClusters.Add(new CentroidCluster<T>(new DoublePoint(arr)));
				j++;
			}
			clusters.Clear();
			clusters = newClusters;
		}

		/// <summary>
		/// Updates the membership matrix and assigns the points to the cluster with
		/// the highest membership.
		/// </summary>
		private void updateMembershipMatrix()
		{
			for (int i = 0; i < points.Count; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T point = points.get(i);
				T point = points[i];
				double maxMembership = 0.0;
				int newCluster = -1;
				for (int j = 0; j < clusters.Count; j++)
				{
					double sum = 0.0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double distA = mathlib.util.FastMath.abs(distance(point, clusters.get(j).getCenter()));
					double distA = FastMath.abs(distance(point, clusters[j].Center));

					foreach (CentroidCluster<T> c in clusters)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double distB = mathlib.util.FastMath.abs(distance(point, c.getCenter()));
						double distB = FastMath.abs(distance(point, c.Center));
						sum += FastMath.pow(distA / distB, 2.0 / (fuzziness - 1.0));
					}

					membershipMatrix[i][j] = 1.0 / sum;

					if (membershipMatrix[i][j] > maxMembership)
					{
						maxMembership = membershipMatrix[i][j];
						newCluster = j;
					}
				}
				clusters[newCluster].addPoint(point);
			}
		}

		/// <summary>
		/// Initialize the membership matrix with random values.
		/// </summary>
		private void initializeMembershipMatrix()
		{
			for (int i = 0; i < points.Count; i++)
			{
				for (int j = 0; j < k; j++)
				{
					membershipMatrix[i][j] = random.NextDouble();
				}
				membershipMatrix[i] = MathArrays.normalizeArray(membershipMatrix[i], 1.0);
			}
		}

		/// <summary>
		/// Calculate the maximum element-by-element change of the membership matrix
		/// for the current iteration.
		/// </summary>
		/// <param name="matrix"> the membership matrix of the previous iteration </param>
		/// <returns> the maximum membership matrix change </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double calculateMaxMembershipChange(final double[][] matrix)
		private double calculateMaxMembershipChange(double[][] matrix)
		{
			double maxMembership = 0.0;
			for (int i = 0; i < points.Count; i++)
			{
				for (int j = 0; j < clusters.Count; j++)
				{
					double v = FastMath.abs(membershipMatrix[i][j] - matrix[i][j]);
					maxMembership = FastMath.max(v, maxMembership);
				}
			}
			return maxMembership;
		}

		/// <summary>
		/// Copy the membership matrix into the provided matrix.
		/// </summary>
		/// <param name="matrix"> the place to store the membership matrix </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void saveMembershipMatrix(final double[][] matrix)
		private void saveMembershipMatrix(double[][] matrix)
		{
			for (int i = 0; i < points.Count; i++)
			{
				Array.Copy(membershipMatrix[i], 0, matrix[i], 0, clusters.Count);
			}
		}

	}

}
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


	using ConvergenceException = mathlib.exception.ConvergenceException;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using mathlib.ml.clustering.evaluation;
	using mathlib.ml.clustering.evaluation;

	/// <summary>
	/// A wrapper around a k-means++ clustering algorithm which performs multiple trials
	/// and returns the best solution. </summary>
	/// @param <T> type of the points to cluster
	/// @version $Id: MultiKMeansPlusPlusClusterer.java 1542545 2013-11-16 18:48:48Z tn $
	/// @since 3.2 </param>
	public class MultiKMeansPlusPlusClusterer<T> : Clusterer<T> where T : Clusterable
	{

		/// <summary>
		/// The underlying k-means clusterer. </summary>
		private readonly KMeansPlusPlusClusterer<T> clusterer;

		/// <summary>
		/// The number of trial runs. </summary>
		private readonly int numTrials;

		/// <summary>
		/// The cluster evaluator to use. </summary>
		private readonly ClusterEvaluator<T> evaluator;

		/// <summary>
		/// Build a clusterer. </summary>
		/// <param name="clusterer"> the k-means clusterer to use </param>
		/// <param name="numTrials"> number of trial runs </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MultiKMeansPlusPlusClusterer(final KMeansPlusPlusClusterer<T> clusterer, final int numTrials)
		public MultiKMeansPlusPlusClusterer(KMeansPlusPlusClusterer<T> clusterer, int numTrials) : this(clusterer, numTrials, new SumOfClusterVariances<T>(clusterer.DistanceMeasure))
		{
		}

		/// <summary>
		/// Build a clusterer. </summary>
		/// <param name="clusterer"> the k-means clusterer to use </param>
		/// <param name="numTrials"> number of trial runs </param>
		/// <param name="evaluator"> the cluster evaluator to use
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MultiKMeansPlusPlusClusterer(final KMeansPlusPlusClusterer<T> clusterer, final int numTrials, final mathlib.ml.clustering.evaluation.ClusterEvaluator<T> evaluator)
		public MultiKMeansPlusPlusClusterer(KMeansPlusPlusClusterer<T> clusterer, int numTrials, ClusterEvaluator<T> evaluator) : base(clusterer.DistanceMeasure)
		{
			this.clusterer = clusterer;
			this.numTrials = numTrials;
			this.evaluator = evaluator;
		}

		/// <summary>
		/// Returns the embedded k-means clusterer used by this instance. </summary>
		/// <returns> the embedded clusterer </returns>
		public virtual KMeansPlusPlusClusterer<T> Clusterer
		{
			get
			{
				return clusterer;
			}
		}

		/// <summary>
		/// Returns the number of trials this instance will do. </summary>
		/// <returns> the number of trials </returns>
		public virtual int NumTrials
		{
			get
			{
				return numTrials;
			}
		}

		/// <summary>
		/// Returns the <seealso cref="ClusterEvaluator"/> used to determine the "best" clustering. </summary>
		/// <returns> the used <seealso cref="ClusterEvaluator"/>
		/// @since 3.3 </returns>
		public virtual ClusterEvaluator<T> ClusterEvaluator
		{
			get
			{
			   return evaluator;
			}
		}

		/// <summary>
		/// Runs the K-means++ clustering algorithm.
		/// </summary>
		/// <param name="points"> the points to cluster </param>
		/// <returns> a list of clusters containing the points </returns>
		/// <exception cref="MathIllegalArgumentException"> if the data points are null or the number
		///   of clusters is larger than the number of data points </exception>
		/// <exception cref="ConvergenceException"> if an empty cluster is encountered and the
		///   underlying <seealso cref="KMeansPlusPlusClusterer"/> has its
		///   <seealso cref="KMeansPlusPlusClusterer.EmptyClusterStrategy"/> is set to {@code ERROR}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public java.util.List<CentroidCluster<T>> cluster(final java.util.Collection<T> points) throws mathlib.exception.MathIllegalArgumentException, mathlib.exception.ConvergenceException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override IList<CentroidCluster<T>> cluster(ICollection<T> points)
		{

			// at first, we have not found any clusters list yet
			IList<CentroidCluster<T>> best = null;
			double bestVarianceSum = double.PositiveInfinity;

			// do several clustering trials
			for (int i = 0; i < numTrials; ++i)
			{

				// compute a clusters list
				IList<CentroidCluster<T>> clusters = clusterer.cluster(points);

				// compute the variance of the current list
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double varianceSum = evaluator.score(clusters);
				double varianceSum = evaluator.score(clusters);

				if (evaluator.isBetterScore(varianceSum, bestVarianceSum))
				{
					// this one is the best we have found so far, remember it
					best = clusters;
					bestVarianceSum = varianceSum;
				}

			}

			// return the best clusters list found
			return best;

		}

	}

}
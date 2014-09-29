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
	using DistanceMeasure = mathlib.ml.distance.DistanceMeasure;

	/// <summary>
	/// Base class for clustering algorithms.
	/// </summary>
	/// @param <T> the type of points that can be clustered
	/// @version $Id: Clusterer.java 1519184 2013-08-31 15:22:59Z tn $
	/// @since 3.2 </param>
	public abstract class Clusterer<T> where T : Clusterable
	{

		/// <summary>
		/// The distance measure to use. </summary>
		private DistanceMeasure measure;

		/// <summary>
		/// Build a new clusterer with the given <seealso cref="DistanceMeasure"/>.
		/// </summary>
		/// <param name="measure"> the distance measure to use </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected Clusterer(final mathlib.ml.distance.DistanceMeasure measure)
		protected internal Clusterer(DistanceMeasure measure)
		{
			this.measure = measure;
		}

		/// <summary>
		/// Perform a cluster analysis on the given set of <seealso cref="Clusterable"/> instances.
		/// </summary>
		/// <param name="points"> the set of <seealso cref="Clusterable"/> instances </param>
		/// <returns> a <seealso cref="List"/> of clusters </returns>
		/// <exception cref="MathIllegalArgumentException"> if points are null or the number of
		///   data points is not compatible with this clusterer </exception>
		/// <exception cref="ConvergenceException"> if the algorithm has not yet converged after
		///   the maximum number of iterations has been exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract java.util.List<? extends Cluster<T>> cluster(java.util.Collection<T> points) throws mathlib.exception.MathIllegalArgumentException, mathlib.exception.ConvergenceException;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract java.util.List<? extends Cluster<T>> cluster(java.util.Collection<T> points) throws mathlib.exception.MathIllegalArgumentException, mathlib.exception.ConvergenceException;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public abstract IList<?> cluster(ICollection<T> points) where ? : Cluster<T>;

		/// <summary>
		/// Returns the <seealso cref="DistanceMeasure"/> instance used by this clusterer.
		/// </summary>
		/// <returns> the distance measure </returns>
		public virtual DistanceMeasure DistanceMeasure
		{
			get
			{
				return measure;
			}
		}

		/// <summary>
		/// Calculates the distance between two <seealso cref="Clusterable"/> instances
		/// with the configured <seealso cref="DistanceMeasure"/>.
		/// </summary>
		/// <param name="p1"> the first clusterable </param>
		/// <param name="p2"> the second clusterable </param>
		/// <returns> the distance between the two clusterables </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected double distance(final Clusterable p1, final Clusterable p2)
		protected internal virtual double distance(Clusterable p1, Clusterable p2)
		{
			return measure.compute(p1.Point, p2.Point);
		}

	}

}
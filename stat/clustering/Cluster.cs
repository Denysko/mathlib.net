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

namespace mathlib.stat.clustering
{


	/// <summary>
	/// Cluster holding a set of <seealso cref="Clusterable"/> points. </summary>
	/// @param <T> the type of points that can be clustered
	/// @version $Id: Cluster.java 1461871 2013-03-27 22:01:25Z tn $
	/// @since 2.0 </param>
	/// @deprecated As of 3.2 (to be removed in 4.0),
	/// use <seealso cref="mathlib.ml.clustering.Cluster"/> instead 
	[Obsolete("As of 3.2 (to be removed in 4.0),"), Serializable]
	public class Cluster<T> where T : Clusterable<T>
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -3442297081515880464L;

		/// <summary>
		/// The points contained in this cluster. </summary>
		private readonly IList<T> points;

		/// <summary>
		/// Center of the cluster. </summary>
		private readonly T center;

		/// <summary>
		/// Build a cluster centered at a specified point. </summary>
		/// <param name="center"> the point which is to be the center of this cluster </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Cluster(final T center)
		public Cluster(T center)
		{
			this.center = center;
			points = new List<T>();
		}

		/// <summary>
		/// Add a point to this cluster. </summary>
		/// <param name="point"> point to add </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void addPoint(final T point)
		public virtual void addPoint(T point)
		{
			points.Add(point);
		}

		/// <summary>
		/// Get the points contained in the cluster. </summary>
		/// <returns> points contained in the cluster </returns>
		public virtual IList<T> Points
		{
			get
			{
				return points;
			}
		}

		/// <summary>
		/// Get the point chosen to be the center of this cluster. </summary>
		/// <returns> chosen cluster center </returns>
		public virtual T Center
		{
			get
			{
				return center;
			}
		}

	}

}
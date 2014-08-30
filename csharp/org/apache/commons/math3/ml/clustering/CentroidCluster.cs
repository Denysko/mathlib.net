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

	/// <summary>
	/// A Cluster used by centroid-based clustering algorithms.
	/// <p>
	/// Defines additionally a cluster center which may not necessarily be a member
	/// of the original data set.
	/// </summary>
	/// @param <T> the type of points that can be clustered
	/// @version $Id: CentroidCluster.java 1519184 2013-08-31 15:22:59Z tn $
	/// @since 3.2 </param>
	public class CentroidCluster<T> : Cluster<T> where T : Clusterable
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -3075288519071812288L;

		/// <summary>
		/// Center of the cluster. </summary>
		private readonly Clusterable center;

		/// <summary>
		/// Build a cluster centered at a specified point. </summary>
		/// <param name="center"> the point which is to be the center of this cluster </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public CentroidCluster(final Clusterable center)
		public CentroidCluster(Clusterable center) : base()
		{
			this.center = center;
		}

		/// <summary>
		/// Get the point chosen to be the center of this cluster. </summary>
		/// <returns> chosen cluster center </returns>
		public virtual Clusterable Center
		{
			get
			{
				return center;
			}
		}

	}

}
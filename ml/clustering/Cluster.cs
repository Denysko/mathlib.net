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

namespace org.apache.commons.math3.ml.clustering
{


	/// <summary>
	/// Cluster holding a set of <seealso cref="Clusterable"/> points. </summary>
	/// @param <T> the type of points that can be clustered
	/// @version $Id: Cluster.java 1461862 2013-03-27 21:48:10Z tn $
	/// @since 3.2 </param>
	[Serializable]
	public class Cluster<T> where T : Clusterable
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -3442297081515880464L;

		/// <summary>
		/// The points contained in this cluster. </summary>
		private readonly IList<T> points;

		/// <summary>
		/// Build a cluster centered at a specified point.
		/// </summary>
		public Cluster()
		{
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

	}

}
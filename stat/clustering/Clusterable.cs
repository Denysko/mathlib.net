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
	/// Interface for points that can be clustered together. </summary>
	/// @param <T> the type of point that can be clustered
	/// @version $Id: Clusterable.java 1461871 2013-03-27 22:01:25Z tn $
	/// @since 2.0 </param>
	/// @deprecated As of 3.2 (to be removed in 4.0),
	/// use <seealso cref="mathlib.ml.clustering.Clusterable"/> instead 
	[Obsolete("As of 3.2 (to be removed in 4.0),")]
	public interface Clusterable<T>
	{

		/// <summary>
		/// Returns the distance from the given point.
		/// </summary>
		/// <param name="p"> the point to compute the distance from </param>
		/// <returns> the distance from the given point </returns>
		double distanceFrom(T p);

		/// <summary>
		/// Returns the centroid of the given Collection of points.
		/// </summary>
		/// <param name="p"> the Collection of points to compute the centroid of </param>
		/// <returns> the centroid of the given Collection of Points </returns>
		T centroidOf(ICollection<T> p);

	}

}
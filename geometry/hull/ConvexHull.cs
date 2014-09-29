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
namespace mathlib.geometry.hull
{

	using InsufficientDataException = mathlib.exception.InsufficientDataException;
	using mathlib.geometry;
	using mathlib.geometry.partitioning;

	/// <summary>
	/// This class represents a convex hull.
	/// </summary>
	/// @param <S> Space type. </param>
	/// @param <P> Point type.
	/// @version $Id: ConvexHull.java 1562624 2014-01-29 22:57:04Z tn $
	/// @since 3.3 </param>
	public interface ConvexHull<S, P> : Serializable where S : mathlib.geometry.Space where P : mathlib.geometry.Point<S>
	{

		/// <summary>
		/// Get the vertices of the convex hull. </summary>
		/// <returns> vertices of the convex hull </returns>
		P[] Vertices {get;}

		/// <summary>
		/// Returns a new region that is enclosed by the convex hull. </summary>
		/// <returns> the region enclosed by the convex hull </returns>
		/// <exception cref="InsufficientDataException"> if the number of vertices is not enough to
		/// build a region in the respective space </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: mathlib.geometry.partitioning.Region<S> createRegion() throws mathlib.exception.InsufficientDataException;
		Region<S> createRegion();
	}

}
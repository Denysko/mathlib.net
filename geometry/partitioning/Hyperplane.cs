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
namespace org.apache.commons.math3.geometry.partitioning
{

	using org.apache.commons.math3.geometry;

	/// <summary>
	/// This interface represents an hyperplane of a space.
	/// 
	/// <p>The most prominent place where hyperplane appears in space
	/// partitioning is as cutters. Each partitioning node in a {@link
	/// BSPTree BSP tree} has a cut <seealso cref="SubHyperplane sub-hyperplane"/>
	/// which is either an hyperplane or a part of an hyperplane. In an
	/// n-dimensions euclidean space, an hyperplane is an (n-1)-dimensions
	/// hyperplane (for example a traditional plane in the 3D euclidean
	/// space). They can be more exotic objects in specific fields, for
	/// example a circle on the surface of the unit sphere.</p>
	/// 
	/// <p>
	/// Note that this interface is <em>not</em> intended to be implemented
	/// by Apache Commons Math users, it is only intended to be implemented
	/// within the library itself. New methods may be added even for minor
	/// versions, which breaks compatibility for external implementations.
	/// </p>
	/// </summary>
	/// @param <S> Type of the space.
	/// 
	/// @version $Id: Hyperplane.java 1566416 2014-02-09 20:56:55Z luc $
	/// @since 3.0 </param>
	public interface Hyperplane<S> where S : org.apache.commons.math3.geometry.Space
	{

		/// <summary>
		/// Copy the instance.
		/// <p>The instance created is completely independant of the original
		/// one. A deep copy is used, none of the underlying objects are
		/// shared (except for immutable objects).</p> </summary>
		/// <returns> a new hyperplane, copy of the instance </returns>
		Hyperplane<S> copySelf();

		/// <summary>
		/// Get the offset (oriented distance) of a point.
		/// <p>The offset is 0 if the point is on the underlying hyperplane,
		/// it is positive if the point is on one particular side of the
		/// hyperplane, and it is negative if the point is on the other side,
		/// according to the hyperplane natural orientation.</p> </summary>
		/// <param name="point"> point to check </param>
		/// <returns> offset of the point </returns>
		double getOffset(Point<S> point);

		/// <summary>
		/// Project a point to the hyperplane. </summary>
		/// <param name="point"> point to project </param>
		/// <returns> projected point
		/// @since 3.3 </returns>
		Point<S> project(Point<S> point);

		/// <summary>
		/// Get the tolerance below which points are considered to belong to the hyperplane. </summary>
		/// <returns> tolerance below which points are considered to belong to the hyperplane
		/// @since 3.3 </returns>
		double Tolerance {get;}

		/// <summary>
		/// Check if the instance has the same orientation as another hyperplane.
		/// <p>This method is expected to be called on parallel hyperplanes. The
		/// method should <em>not</em> re-check for parallelism, only for
		/// orientation, typically by testing something like the sign of the
		/// dot-products of normals.</p> </summary>
		/// <param name="other"> other hyperplane to check against the instance </param>
		/// <returns> true if the instance and the other hyperplane have
		/// the same orientation </returns>
		bool sameOrientationAs(Hyperplane<S> other);

		/// <summary>
		/// Build a sub-hyperplane covering the whole hyperplane. </summary>
		/// <returns> a sub-hyperplane covering the whole hyperplane </returns>
		SubHyperplane<S> wholeHyperplane();

		/// <summary>
		/// Build a region covering the whole space. </summary>
		/// <returns> a region containing the instance </returns>
		Region<S> wholeSpace();

	}

}
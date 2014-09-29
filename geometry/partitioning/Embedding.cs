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
namespace mathlib.geometry.partitioning
{

	using mathlib.geometry;

	/// <summary>
	/// This interface defines mappers between a space and one of its sub-spaces.
	/// 
	/// <p>Sub-spaces are the lower dimensions subsets of a n-dimensions
	/// space. The (n-1)-dimension sub-spaces are specific sub-spaces known
	/// as <seealso cref="Hyperplane hyperplanes"/>. This interface can be used regardless
	/// of the dimensions differences. As an example, {@link
	/// mathlib.geometry.euclidean.threed.Line Line} in 3D
	/// implements Embedding<{@link
	/// mathlib.geometry.euclidean.threed.Vector3D Vector3D}, {link
	/// mathlib.geometry.euclidean.oned.Vector1D Vector1D>, i.e. it
	/// maps directly dimensions 3 and 1.</p>
	/// 
	/// <p>In the 3D euclidean space, hyperplanes are 2D planes, and the 1D
	/// sub-spaces are lines.</p>
	/// 
	/// <p>
	/// Note that this interface is <em>not</em> intended to be implemented
	/// by Apache Commons Math users, it is only intended to be implemented
	/// within the library itself. New methods may be added even for minor
	/// versions, which breaks compatibility for external implementations.
	/// </p>
	/// </summary>
	/// @param <S> Type of the embedding space. </param>
	/// @param <T> Type of the embedded sub-space.
	/// </param>
	/// <seealso cref= Hyperplane
	/// @version $Id: Embedding.java 1591835 2014-05-02 09:04:01Z tn $
	/// @since 3.0 </seealso>
	public interface Embedding<S, T> where S : mathlib.geometry.Space where T : mathlib.geometry.Space
	{

		/// <summary>
		/// Transform a space point into a sub-space point. </summary>
		/// <param name="point"> n-dimension point of the space </param>
		/// <returns> (n-1)-dimension point of the sub-space corresponding to
		/// the specified space point </returns>
		/// <seealso cref= #toSpace </seealso>
		Point<T> toSubSpace(Point<S> point);

		/// <summary>
		/// Transform a sub-space point into a space point. </summary>
		/// <param name="point"> (n-1)-dimension point of the sub-space </param>
		/// <returns> n-dimension point of the space corresponding to the
		/// specified sub-space point </returns>
		/// <seealso cref= #toSubSpace </seealso>
		Point<S> toSpace(Point<T> point);

	}

}
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
	/// This interface represents an inversible affine transform in a space.
	/// <p>Inversible affine transform include for example scalings,
	/// translations, rotations.</p>
	/// 
	/// <p>Transforms are dimension-specific. The consistency rules between
	/// the three {@code apply} methods are the following ones for a
	/// transformed defined for dimension D:</p>
	/// <ul>
	///   <li>
	///     the transform can be applied to a point in the
	///     D-dimension space using its <seealso cref="#apply(Point)"/>
	///     method
	///   </li>
	///   <li>
	///     the transform can be applied to a (D-1)-dimension
	///     hyperplane in the D-dimension space using its
	///     <seealso cref="#apply(Hyperplane)"/> method
	///   </li>
	///   <li>
	///     the transform can be applied to a (D-2)-dimension
	///     sub-hyperplane in a (D-1)-dimension hyperplane using
	///     its <seealso cref="#apply(SubHyperplane, Hyperplane, Hyperplane)"/>
	///     method
	///   </li>
	/// </ul>
	/// </summary>
	/// @param <S> Type of the embedding space. </param>
	/// @param <T> Type of the embedded sub-space.
	/// 
	/// @version $Id: Transform.java 1554646 2014-01-01 17:22:53Z luc $
	/// @since 3.0 </param>
	public interface Transform<S, T> where S : mathlib.geometry.Space where T : mathlib.geometry.Space
	{

		/// <summary>
		/// Transform a point of a space. </summary>
		/// <param name="point"> point to transform </param>
		/// <returns> a new object representing the transformed point </returns>
		Point<S> apply(Point<S> point);

		/// <summary>
		/// Transform an hyperplane of a space. </summary>
		/// <param name="hyperplane"> hyperplane to transform </param>
		/// <returns> a new object representing the transformed hyperplane </returns>
		Hyperplane<S> apply(Hyperplane<S> hyperplane);

		/// <summary>
		/// Transform a sub-hyperplane embedded in an hyperplane. </summary>
		/// <param name="sub"> sub-hyperplane to transform </param>
		/// <param name="original"> hyperplane in which the sub-hyperplane is
		/// defined (this is the original hyperplane, the transform has
		/// <em>not</em> been applied to it) </param>
		/// <param name="transformed"> hyperplane in which the sub-hyperplane is
		/// defined (this is the transformed hyperplane, the transform
		/// <em>has</em> been applied to it) </param>
		/// <returns> a new object representing the transformed sub-hyperplane </returns>
		SubHyperplane<T> apply(SubHyperplane<T> sub, Hyperplane<S> original, Hyperplane<S> transformed);

	}

}
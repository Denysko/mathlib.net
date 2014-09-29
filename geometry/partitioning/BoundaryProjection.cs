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
	/// Class holding the result of point projection on region boundary.
	/// <p>This class is a simple placeholder, it does not provide any
	/// processing methods.</p>
	/// <p>Instances of this class are guaranteed to be immutable</p> </summary>
	/// @param <S> Type of the space.
	/// @version $Id: BoundaryProjection.java 1560335 2014-01-22 12:39:06Z luc $
	/// @since 3.3 </param>
	/// <seealso cref= AbstractRegion#projectToBoundary(Point) </seealso>
	public class BoundaryProjection<S> where S : mathlib.geometry.Space
	{

		/// <summary>
		/// Original point. </summary>
		private readonly Point<S> original;

		/// <summary>
		/// Projected point. </summary>
		private readonly Point<S> projected;

		/// <summary>
		/// Offset of the point with respect to the boundary it is projected on. </summary>
		private readonly double offset;

		/// <summary>
		/// Constructor from raw elements. </summary>
		/// <param name="original"> original point </param>
		/// <param name="projected"> projected point </param>
		/// <param name="offset"> offset of the point with respect to the boundary it is projected on </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BoundaryProjection(final mathlib.geometry.Point<S> original, final mathlib.geometry.Point<S> projected, final double offset)
		public BoundaryProjection(Point<S> original, Point<S> projected, double offset)
		{
			this.original = original;
			this.projected = projected;
			this.offset = offset;
		}

		/// <summary>
		/// Get the original point. </summary>
		/// <returns> original point </returns>
		public virtual Point<S> Original
		{
			get
			{
				return original;
			}
		}

		/// <summary>
		/// Projected point. </summary>
		/// <returns> projected point, or null if there are no boundary </returns>
		public virtual Point<S> Projected
		{
			get
			{
				return projected;
			}
		}

		/// <summary>
		/// Offset of the point with respect to the boundary it is projected on.
		/// <p>
		/// The offset with respect to the boundary is negative if the {@link
		/// #getOriginal() original point} is inside the region, and positive otherwise.
		/// </p>
		/// <p>
		/// If there are no boundary, the value is set to either {@code
		/// Double.POSITIVE_INFINITY} if the region is empty (i.e. all points are
		/// outside of the region) or {@code Double.NEGATIVE_INFINITY} if the region
		/// covers the whole space (i.e. all points are inside of the region).
		/// </p> </summary>
		/// <returns> offset of the point with respect to the boundary it is projected on </returns>
		public virtual double Offset
		{
			get
			{
				return offset;
			}
		}

	}

}
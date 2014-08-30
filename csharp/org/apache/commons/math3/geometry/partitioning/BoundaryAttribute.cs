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

	/// <summary>
	/// Class holding boundary attributes.
	/// <p>This class is used for the attributes associated with the
	/// nodes of region boundary shell trees returned by the {@link
	/// Region#getTree(boolean) Region.getTree(includeBoundaryAttributes)}
	/// when the boolean {@code includeBoundaryAttributes} parameter is
	/// set to {@code true}. It contains the parts of the node cut
	/// sub-hyperplane that belong to the boundary.</p>
	/// <p>This class is a simple placeholder, it does not provide any
	/// processing methods.</p> </summary>
	/// @param <S> Type of the space. </param>
	/// <seealso cref= Region#getTree
	/// @version $Id: BoundaryAttribute.java 1560336 2014-01-22 12:39:14Z luc $
	/// @since 3.0 </seealso>
	public class BoundaryAttribute<S> where S : org.apache.commons.math3.geometry.Space
	{

		/// <summary>
		/// Part of the node cut sub-hyperplane that belongs to the
		/// boundary and has the outside of the region on the plus side of
		/// its underlying hyperplane (may be null).
		/// </summary>
		private readonly SubHyperplane<S> plusOutside;

		/// <summary>
		/// Part of the node cut sub-hyperplane that belongs to the
		/// boundary and has the inside of the region on the plus side of
		/// its underlying hyperplane (may be null).
		/// </summary>
		private readonly SubHyperplane<S> plusInside;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="plusOutside"> part of the node cut sub-hyperplane that
		/// belongs to the boundary and has the outside of the region on
		/// the plus side of its underlying hyperplane (may be null) </param>
		/// <param name="plusInside"> part of the node cut sub-hyperplane that
		/// belongs to the boundary and has the inside of the region on the
		/// plus side of its underlying hyperplane (may be null) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BoundaryAttribute(final SubHyperplane<S> plusOutside, final SubHyperplane<S> plusInside)
		public BoundaryAttribute(SubHyperplane<S> plusOutside, SubHyperplane<S> plusInside)
		{
			this.plusOutside = plusOutside;
			this.plusInside = plusInside;
		}

		/// <summary>
		/// Get the part of the node cut sub-hyperplane that belongs to the
		/// boundary and has the outside of the region on the plus side of
		/// its underlying hyperplane. </summary>
		/// <returns> part of the node cut sub-hyperplane that belongs to the
		/// boundary and has the outside of the region on the plus side of
		/// its underlying hyperplane </returns>
		public virtual SubHyperplane<S> PlusOutside
		{
			get
			{
				return plusOutside;
			}
		}

		/// <summary>
		/// Get the part of the node cut sub-hyperplane that belongs to the
		/// boundary and has the inside of the region on the plus side of
		/// its underlying hyperplane. </summary>
		/// <returns> part of the node cut sub-hyperplane that belongs to the
		/// boundary and has the inside of the region on the plus side of
		/// its underlying hyperplane </returns>
		public virtual SubHyperplane<S> PlusInside
		{
			get
			{
				return plusInside;
			}
		}

	}

}
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
namespace org.apache.commons.math3.geometry.partitioning
{


	using org.apache.commons.math3.geometry;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Local tree visitor to compute projection on boundary. </summary>
	/// @param <S> Type of the space. </param>
	/// @param <T> Type of the sub-space.
	/// @version $Id: BoundaryProjector.java 1560115 2014-01-21 17:49:13Z luc $
	/// @since 3.3 </param>
	internal class BoundaryProjector<S, T> : BSPTreeVisitor<S> where S : org.apache.commons.math3.geometry.Space where T : org.apache.commons.math3.geometry.Space
	{

		/// <summary>
		/// Original point. </summary>
		private readonly Point<S> original;

		/// <summary>
		/// Current best projected point. </summary>
		private Point<S> projected;

		/// <summary>
		/// Leaf node closest to the test point. </summary>
		private BSPTree<S> leaf;

		/// <summary>
		/// Current offset. </summary>
		private double offset;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="original"> original point </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BoundaryProjector(final org.apache.commons.math3.geometry.Point<S> original)
		public BoundaryProjector(Point<S> original)
		{
			this.original = original;
			this.projected = null;
			this.leaf = null;
			this.offset = double.PositiveInfinity;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BSPTreeVisitor_Order visitOrder(final BSPTree<S> node)
		public virtual BSPTreeVisitor_Order visitOrder(BSPTree<S> node)
		{
			// we want to visit the tree so that the first encountered
			// leaf is the one closest to the test point
			if (node.Cut.Hyperplane.getOffset(original) <= 0)
			{
				return BSPTreeVisitor_Order.MINUS_SUB_PLUS;
			}
			else
			{
				return BSPTreeVisitor_Order.PLUS_SUB_MINUS;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visitInternalNode(final BSPTree<S> node)
		public virtual void visitInternalNode(BSPTree<S> node)
		{

			// project the point on the cut sub-hyperplane
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Hyperplane<S> hyperplane = node.getCut().getHyperplane();
			Hyperplane<S> hyperplane = node.Cut.Hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double signedOffset = hyperplane.getOffset(original);
			double signedOffset = hyperplane.getOffset(original);
			if (FastMath.abs(signedOffset) < offset)
			{

				// project point
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.Point<S> regular = hyperplane.project(original);
				Point<S> regular = hyperplane.project(original);

				// get boundary parts
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Region<T>> boundaryParts = boundaryRegions(node);
				IList<Region<T>> boundaryParts = boundaryRegions(node);

				// check if regular projection really belongs to the boundary
				bool regularFound = false;
				foreach (Region<T> part in boundaryParts)
				{
					if (!regularFound && belongsToPart(regular, hyperplane, part))
					{
						// the projected point lies in the boundary
						projected = regular;
						offset = FastMath.abs(signedOffset);
						regularFound = true;
					}
				}

				if (!regularFound)
				{
					// the regular projected point is not on boundary,
					// so we have to check further if a singular point
					// (i.e. a vertex in 2D case) is a possible projection
					foreach (Region<T> part in boundaryParts)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.Point<S> spI = singularProjection(regular, hyperplane, part);
						Point<S> spI = singularProjection(regular, hyperplane, part);
						if (spI != null)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double distance = original.distance(spI);
							double distance = original.distance(spI);
							if (distance < offset)
							{
								projected = spI;
								offset = distance;
							}
						}
					}

				}

			}

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visitLeafNode(final BSPTree<S> node)
		public virtual void visitLeafNode(BSPTree<S> node)
		{
			if (leaf == null)
			{
				// this is the first leaf we visit,
				// it is the closest one to the original point
				leaf = node;
			}
		}

		/// <summary>
		/// Get the projection. </summary>
		/// <returns> projection </returns>
		public virtual BoundaryProjection<S> Projection
		{
			get
			{
    
				// fix offset sign
				offset = FastMath.copySign(offset, (bool?) leaf.Attribute ? - 1 : +1);
    
				return new BoundaryProjection<S>(original, projected, offset);
    
			}
		}

		/// <summary>
		/// Extract the regions of the boundary on an internal node. </summary>
		/// <param name="node"> internal node </param>
		/// <returns> regions in the node sub-hyperplane </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private java.util.List<Region<T>> boundaryRegions(final BSPTree<S> node)
		private IList<Region<T>> boundaryRegions(BSPTree<S> node)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Region<T>> regions = new java.util.ArrayList<Region<T>>(2);
			IList<Region<T>> regions = new List<Region<T>>(2);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final BoundaryAttribute<S> ba = (BoundaryAttribute<S>) node.getAttribute();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			BoundaryAttribute<S> ba = (BoundaryAttribute<S>) node.Attribute;
			addRegion(ba.PlusInside, regions);
			addRegion(ba.PlusOutside, regions);

			return regions;

		}

		/// <summary>
		/// Add a boundary region to a list. </summary>
		/// <param name="sub"> sub-hyperplane defining the region </param>
		/// <param name="list"> to fill up </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void addRegion(final SubHyperplane<S> sub, final java.util.List<Region<T>> list)
		private void addRegion(SubHyperplane<S> sub, IList<Region<T>> list)
		{
			if (sub != null)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final Region<T> region = ((AbstractSubHyperplane<S, T>) sub).getRemainingRegion();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
				Region<T> region = ((AbstractSubHyperplane<S, T>) sub).RemainingRegion;
				if (region != null)
				{
					list.Add(region);
				}
			}
		}

		/// <summary>
		/// Check if a projected point lies on a boundary part. </summary>
		/// <param name="point"> projected point to check </param>
		/// <param name="hyperplane"> hyperplane into which the point was projected </param>
		/// <param name="part"> boundary part </param>
		/// <returns> true if point lies on the boundary part </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private boolean belongsToPart(final org.apache.commons.math3.geometry.Point<S> point, final Hyperplane<S> hyperplane, final Region<T> part)
		private bool belongsToPart(Point<S> point, Hyperplane<S> hyperplane, Region<T> part)
		{

			// there is a non-null sub-space, we can dive into smaller dimensions
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final Embedding<S, T> embedding = (Embedding<S, T>) hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			Embedding<S, T> embedding = (Embedding<S, T>) hyperplane;
			return part.checkPoint(embedding.toSubSpace(point)) != Region_Location.OUTSIDE;

		}

		/// <summary>
		/// Get the projection to the closest boundary singular point. </summary>
		/// <param name="point"> projected point to check </param>
		/// <param name="hyperplane"> hyperplane into which the point was projected </param>
		/// <param name="part"> boundary part </param>
		/// <returns> projection to a singular point of boundary part (may be null) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private org.apache.commons.math3.geometry.Point<S> singularProjection(final org.apache.commons.math3.geometry.Point<S> point, final Hyperplane<S> hyperplane, final Region<T> part)
		private Point<S> singularProjection(Point<S> point, Hyperplane<S> hyperplane, Region<T> part)
		{

			// there is a non-null sub-space, we can dive into smaller dimensions
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final Embedding<S, T> embedding = (Embedding<S, T>) hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			Embedding<S, T> embedding = (Embedding<S, T>) hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BoundaryProjection<T> bp = part.projectToBoundary(embedding.toSubSpace(point));
			BoundaryProjection<T> bp = part.projectToBoundary(embedding.toSubSpace(point));

			// back to initial dimension
			return (bp.Projected == null) ? null : embedding.toSpace(bp.Projected);

		}

	}

}
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
namespace org.apache.commons.math3.geometry.euclidean.threed
{


	using org.apache.commons.math3.geometry;
	using Euclidean1D = org.apache.commons.math3.geometry.euclidean.oned.Euclidean1D;
	using Euclidean2D = org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D;
	using SubLine = org.apache.commons.math3.geometry.euclidean.twod.SubLine;
	using Vector2D = org.apache.commons.math3.geometry.euclidean.twod.Vector2D;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// This class represents a 3D region: a set of polyhedrons.
	/// @version $Id: PolyhedronsSet.java 1590560 2014-04-28 06:39:01Z luc $
	/// @since 3.0
	/// </summary>
	public class PolyhedronsSet : AbstractRegion<Euclidean3D, Euclidean2D>
	{

		/// <summary>
		/// Default value for tolerance. </summary>
		private const double DEFAULT_TOLERANCE = 1.0e-10;

		/// <summary>
		/// Build a polyhedrons set representing the whole real line. </summary>
		/// <param name="tolerance"> tolerance below which points are considered identical
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PolyhedronsSet(final double tolerance)
		public PolyhedronsSet(double tolerance) : base(tolerance)
		{
		}

		/// <summary>
		/// Build a polyhedrons set from a BSP tree.
		/// <p>The leaf nodes of the BSP tree <em>must</em> have a
		/// {@code Boolean} attribute representing the inside status of
		/// the corresponding cell (true for inside cells, false for outside
		/// cells). In order to avoid building too many small objects, it is
		/// recommended to use the predefined constants
		/// {@code Boolean.TRUE} and {@code Boolean.FALSE}</p>
		/// <p>
		/// This constructor is aimed at expert use, as building the tree may
		/// be a difficult task. It is not intended for general use and for
		/// performances reasons does not check thoroughly its input, as this would
		/// require walking the full tree each time. Failing to provide a tree with
		/// the proper attributes, <em>will</em> therefore generate problems like
		/// <seealso cref="NullPointerException"/> or <seealso cref="ClassCastException"/> only later on.
		/// This limitation is known and explains why this constructor is for expert
		/// use only. The caller does have the responsibility to provided correct arguments.
		/// </p> </summary>
		/// <param name="tree"> inside/outside BSP tree representing the region </param>
		/// <param name="tolerance"> tolerance below which points are considered identical
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PolyhedronsSet(final org.apache.commons.math3.geometry.partitioning.BSPTree<Euclidean3D> tree, final double tolerance)
		public PolyhedronsSet(BSPTree<Euclidean3D> tree, double tolerance) : base(tree, tolerance)
		{
		}

		/// <summary>
		/// Build a polyhedrons set from a Boundary REPresentation (B-rep).
		/// <p>The boundary is provided as a collection of {@link
		/// SubHyperplane sub-hyperplanes}. Each sub-hyperplane has the
		/// interior part of the region on its minus side and the exterior on
		/// its plus side.</p>
		/// <p>The boundary elements can be in any order, and can form
		/// several non-connected sets (like for example polyhedrons with holes
		/// or a set of disjoint polyhedrons considered as a whole). In
		/// fact, the elements do not even need to be connected together
		/// (their topological connections are not used here). However, if the
		/// boundary does not really separate an inside open from an outside
		/// open (open having here its topological meaning), then subsequent
		/// calls to the <seealso cref="Region#checkPoint(Point) checkPoint"/> method will
		/// not be meaningful anymore.</p>
		/// <p>If the boundary is empty, the region will represent the whole
		/// space.</p> </summary>
		/// <param name="boundary"> collection of boundary elements, as a
		/// collection of <seealso cref="SubHyperplane SubHyperplane"/> objects </param>
		/// <param name="tolerance"> tolerance below which points are considered identical
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PolyhedronsSet(final java.util.Collection<org.apache.commons.math3.geometry.partitioning.SubHyperplane<Euclidean3D>> boundary, final double tolerance)
		public PolyhedronsSet(ICollection<SubHyperplane<Euclidean3D>> boundary, double tolerance) : base(boundary, tolerance)
		{
		}

		/// <summary>
		/// Build a parallellepipedic box. </summary>
		/// <param name="xMin"> low bound along the x direction </param>
		/// <param name="xMax"> high bound along the x direction </param>
		/// <param name="yMin"> low bound along the y direction </param>
		/// <param name="yMax"> high bound along the y direction </param>
		/// <param name="zMin"> low bound along the z direction </param>
		/// <param name="zMax"> high bound along the z direction </param>
		/// <param name="tolerance"> tolerance below which points are considered identical
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PolyhedronsSet(final double xMin, final double xMax, final double yMin, final double yMax, final double zMin, final double zMax, final double tolerance)
		public PolyhedronsSet(double xMin, double xMax, double yMin, double yMax, double zMin, double zMax, double tolerance) : base(buildBoundary(xMin, xMax, yMin, yMax, zMin, zMax, tolerance), tolerance)
		{
		}

		/// <summary>
		/// Build a polyhedrons set representing the whole real line. </summary>
		/// @deprecated as of 3.3, replaced with <seealso cref="#PolyhedronsSet(double)"/> 
		[Obsolete("as of 3.3, replaced with <seealso cref="#PolyhedronsSet(double)"/>")]
		public PolyhedronsSet() : this(DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Build a polyhedrons set from a BSP tree.
		/// <p>The leaf nodes of the BSP tree <em>must</em> have a
		/// {@code Boolean} attribute representing the inside status of
		/// the corresponding cell (true for inside cells, false for outside
		/// cells). In order to avoid building too many small objects, it is
		/// recommended to use the predefined constants
		/// {@code Boolean.TRUE} and {@code Boolean.FALSE}</p> </summary>
		/// <param name="tree"> inside/outside BSP tree representing the region </param>
		/// @deprecated as of 3.3, replaced with <seealso cref="#PolyhedronsSet(BSPTree, double)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3, replaced with <seealso cref="#PolyhedronsSet(org.apache.commons.math3.geometry.partitioning.BSPTree, double)"/>") public PolyhedronsSet(final org.apache.commons.math3.geometry.partitioning.BSPTree<Euclidean3D> tree)
		[Obsolete("as of 3.3, replaced with <seealso cref="#PolyhedronsSet(org.apache.commons.math3.geometry.partitioning.BSPTree, double)"/>")]
		public PolyhedronsSet(BSPTree<Euclidean3D> tree) : this(tree, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Build a polyhedrons set from a Boundary REPresentation (B-rep).
		/// <p>The boundary is provided as a collection of {@link
		/// SubHyperplane sub-hyperplanes}. Each sub-hyperplane has the
		/// interior part of the region on its minus side and the exterior on
		/// its plus side.</p>
		/// <p>The boundary elements can be in any order, and can form
		/// several non-connected sets (like for example polyhedrons with holes
		/// or a set of disjoint polyhedrons considered as a whole). In
		/// fact, the elements do not even need to be connected together
		/// (their topological connections are not used here). However, if the
		/// boundary does not really separate an inside open from an outside
		/// open (open having here its topological meaning), then subsequent
		/// calls to the <seealso cref="Region#checkPoint(Point) checkPoint"/> method will
		/// not be meaningful anymore.</p>
		/// <p>If the boundary is empty, the region will represent the whole
		/// space.</p> </summary>
		/// <param name="boundary"> collection of boundary elements, as a
		/// collection of <seealso cref="SubHyperplane SubHyperplane"/> objects </param>
		/// @deprecated as of 3.3, replaced with <seealso cref="#PolyhedronsSet(Collection, double)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3, replaced with <seealso cref="#PolyhedronsSet(java.util.Collection, double)"/>") public PolyhedronsSet(final java.util.Collection<org.apache.commons.math3.geometry.partitioning.SubHyperplane<Euclidean3D>> boundary)
		[Obsolete("as of 3.3, replaced with <seealso cref="#PolyhedronsSet(java.util.Collection, double)"/>")]
		public PolyhedronsSet(ICollection<SubHyperplane<Euclidean3D>> boundary) : this(boundary, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Build a parallellepipedic box. </summary>
		/// <param name="xMin"> low bound along the x direction </param>
		/// <param name="xMax"> high bound along the x direction </param>
		/// <param name="yMin"> low bound along the y direction </param>
		/// <param name="yMax"> high bound along the y direction </param>
		/// <param name="zMin"> low bound along the z direction </param>
		/// <param name="zMax"> high bound along the z direction </param>
		/// @deprecated as of 3.3, replaced with {@link #PolyhedronsSet(double, double,
		/// double, double, double, double, double)} 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3, replaced with {@link #PolyhedronsSet(double, double,") public PolyhedronsSet(final double xMin, final double xMax, final double yMin, final double yMax, final double zMin, final double zMax)
		[Obsolete("as of 3.3, replaced with {@link #PolyhedronsSet(double, double,")]
		public PolyhedronsSet(double xMin, double xMax, double yMin, double yMax, double zMin, double zMax) : this(xMin, xMax, yMin, yMax, zMin, zMax, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Build a parallellepipedic box boundary. </summary>
		/// <param name="xMin"> low bound along the x direction </param>
		/// <param name="xMax"> high bound along the x direction </param>
		/// <param name="yMin"> low bound along the y direction </param>
		/// <param name="yMax"> high bound along the y direction </param>
		/// <param name="zMin"> low bound along the z direction </param>
		/// <param name="zMax"> high bound along the z direction </param>
		/// <param name="tolerance"> tolerance below which points are considered identical </param>
		/// <returns> boundary tree
		/// @since 3.3 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static org.apache.commons.math3.geometry.partitioning.BSPTree<Euclidean3D> buildBoundary(final double xMin, final double xMax, final double yMin, final double yMax, final double zMin, final double zMax, final double tolerance)
		private static BSPTree<Euclidean3D> buildBoundary(double xMin, double xMax, double yMin, double yMax, double zMin, double zMax, double tolerance)
		{
			if ((xMin >= xMax - tolerance) || (yMin >= yMax - tolerance) || (zMin >= zMax - tolerance))
			{
				// too thin box, build an empty polygons set
				return new BSPTree<Euclidean3D>(false);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane pxMin = new Plane(new Vector3D(xMin, 0, 0), Vector3D.MINUS_I, tolerance);
			Plane pxMin = new Plane(new Vector3D(xMin, 0, 0), Vector3D.MINUS_I, tolerance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane pxMax = new Plane(new Vector3D(xMax, 0, 0), Vector3D.PLUS_I, tolerance);
			Plane pxMax = new Plane(new Vector3D(xMax, 0, 0), Vector3D.PLUS_I, tolerance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane pyMin = new Plane(new Vector3D(0, yMin, 0), Vector3D.MINUS_J, tolerance);
			Plane pyMin = new Plane(new Vector3D(0, yMin, 0), Vector3D.MINUS_J, tolerance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane pyMax = new Plane(new Vector3D(0, yMax, 0), Vector3D.PLUS_J, tolerance);
			Plane pyMax = new Plane(new Vector3D(0, yMax, 0), Vector3D.PLUS_J, tolerance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane pzMin = new Plane(new Vector3D(0, 0, zMin), Vector3D.MINUS_K, tolerance);
			Plane pzMin = new Plane(new Vector3D(0, 0, zMin), Vector3D.MINUS_K, tolerance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane pzMax = new Plane(new Vector3D(0, 0, zMax), Vector3D.PLUS_K, tolerance);
			Plane pzMax = new Plane(new Vector3D(0, 0, zMax), Vector3D.PLUS_K, tolerance);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final org.apache.commons.math3.geometry.partitioning.Region<Euclidean3D> boundary = new org.apache.commons.math3.geometry.partitioning.RegionFactory<Euclidean3D>().buildConvex(pxMin, pxMax, pyMin, pyMax, pzMin, pzMax);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			Region<Euclidean3D> boundary = (new RegionFactory<Euclidean3D>()).buildConvex(pxMin, pxMax, pyMin, pyMax, pzMin, pzMax);
			return boundary.getTree(false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public PolyhedronsSet buildNew(final org.apache.commons.math3.geometry.partitioning.BSPTree<Euclidean3D> tree)
		public override PolyhedronsSet buildNew(BSPTree<Euclidean3D> tree)
		{
			return new PolyhedronsSet(tree, Tolerance);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override void computeGeometricalProperties()
		{

			// compute the contribution of all boundary facets
			getTree(true).visit(new FacetsContributionVisitor(this));

			if (Size < 0)
			{
				// the polyhedrons set as a finite outside
				// surrounded by an infinite inside
				Size = double.PositiveInfinity;
				Barycenter = (Point<Euclidean3D>) Vector3D.NaN_Renamed;
			}
			else
			{
				// the polyhedrons set is finite, apply the remaining scaling factors
				Size = Size / 3.0;
				Barycenter = (Point<Euclidean3D>) new Vector3D(1.0 / (4 * Size), (Vector3D) Barycenter);
			}

		}

		/// <summary>
		/// Visitor computing geometrical properties. </summary>
		private class FacetsContributionVisitor : BSPTreeVisitor<Euclidean3D>
		{
			private readonly PolyhedronsSet outerInstance;


			/// <summary>
			/// Simple constructor. </summary>
			public FacetsContributionVisitor(PolyhedronsSet outerInstance)
			{
				this.outerInstance = outerInstance;
				outerInstance.Size = 0;
				outerInstance.Barycenter = (Point<Euclidean3D>) new Vector3D(0, 0, 0);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.partitioning.BSPTreeVisitor_Order visitOrder(final org.apache.commons.math3.geometry.partitioning.BSPTree<Euclidean3D> node)
			public virtual org.apache.commons.math3.geometry.partitioning.BSPTreeVisitor_Order visitOrder(BSPTree<Euclidean3D> node)
			{
				return org.apache.commons.math3.geometry.partitioning.BSPTreeVisitor_Order.MINUS_SUB_PLUS;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visitInternalNode(final org.apache.commons.math3.geometry.partitioning.BSPTree<Euclidean3D> node)
			public virtual void visitInternalNode(BSPTree<Euclidean3D> node)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final org.apache.commons.math3.geometry.partitioning.BoundaryAttribute<Euclidean3D> attribute = (org.apache.commons.math3.geometry.partitioning.BoundaryAttribute<Euclidean3D>) node.getAttribute();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
				BoundaryAttribute<Euclidean3D> attribute = (BoundaryAttribute<Euclidean3D>) node.Attribute;
				if (attribute.PlusOutside != null)
				{
					addContribution(attribute.PlusOutside, false);
				}
				if (attribute.PlusInside != null)
				{
					addContribution(attribute.PlusInside, true);
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visitLeafNode(final org.apache.commons.math3.geometry.partitioning.BSPTree<Euclidean3D> node)
			public virtual void visitLeafNode(BSPTree<Euclidean3D> node)
			{
			}

			/// <summary>
			/// Add he contribution of a boundary facet. </summary>
			/// <param name="facet"> boundary facet </param>
			/// <param name="reversed"> if true, the facet has the inside on its plus side </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void addContribution(final org.apache.commons.math3.geometry.partitioning.SubHyperplane<Euclidean3D> facet, final boolean reversed)
			internal virtual void addContribution(SubHyperplane<Euclidean3D> facet, bool reversed)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.partitioning.Region<org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D> polygon = ((SubPlane) facet).getRemainingRegion();
				Region<Euclidean2D> polygon = ((SubPlane) facet).RemainingRegion;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double area = polygon.getSize();
				double area = polygon.Size;

				if (double.IsInfinity(area))
				{
					outerInstance.Size = double.PositiveInfinity;
					outerInstance.Barycenter = (Point<Euclidean3D>) Vector3D.NaN_Renamed;
				}
				else
				{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane plane = (Plane) facet.getHyperplane();
					Plane plane = (Plane) facet.Hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D facetB = plane.toSpace(polygon.getBarycenter());
					Vector3D facetB = plane.toSpace(polygon.Barycenter);
					double scaled = area * facetB.dotProduct(plane.Normal);
					if (reversed)
					{
						scaled = -scaled;
					}

					outerInstance.Size = outerInstance.Size + scaled;
					outerInstance.Barycenter = (Point<Euclidean3D>) new Vector3D(1.0, (Vector3D) outerInstance.Barycenter, scaled, facetB);

				}

			}

		}

		/// <summary>
		/// Get the first sub-hyperplane crossed by a semi-infinite line. </summary>
		/// <param name="point"> start point of the part of the line considered </param>
		/// <param name="line"> line to consider (contains point) </param>
		/// <returns> the first sub-hyperplaned crossed by the line after the
		/// given point, or null if the line does not intersect any
		/// sub-hyperplaned </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.partitioning.SubHyperplane<Euclidean3D> firstIntersection(final Vector3D point, final Line line)
		public virtual SubHyperplane<Euclidean3D> firstIntersection(Vector3D point, Line line)
		{
			return recurseFirstIntersection(getTree(true), point, line);
		}

		/// <summary>
		/// Get the first sub-hyperplane crossed by a semi-infinite line. </summary>
		/// <param name="node"> current node </param>
		/// <param name="point"> start point of the part of the line considered </param>
		/// <param name="line"> line to consider (contains point) </param>
		/// <returns> the first sub-hyperplaned crossed by the line after the
		/// given point, or null if the line does not intersect any
		/// sub-hyperplaned </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private org.apache.commons.math3.geometry.partitioning.SubHyperplane<Euclidean3D> recurseFirstIntersection(final org.apache.commons.math3.geometry.partitioning.BSPTree<Euclidean3D> node, final Vector3D point, final Line line)
		private SubHyperplane<Euclidean3D> recurseFirstIntersection(BSPTree<Euclidean3D> node, Vector3D point, Line line)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.partitioning.SubHyperplane<Euclidean3D> cut = node.getCut();
			SubHyperplane<Euclidean3D> cut = node.Cut;
			if (cut == null)
			{
				return null;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.partitioning.BSPTree<Euclidean3D> minus = node.getMinus();
			BSPTree<Euclidean3D> minus = node.Minus;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.partitioning.BSPTree<Euclidean3D> plus = node.getPlus();
			BSPTree<Euclidean3D> plus = node.Plus;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane plane = (Plane) cut.getHyperplane();
			Plane plane = (Plane) cut.Hyperplane;

			// establish search order
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double offset = plane.getOffset((org.apache.commons.math3.geometry.Point<Euclidean3D>) point);
			double offset = plane.getOffset((Point<Euclidean3D>) point);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean in = org.apache.commons.math3.util.FastMath.abs(offset) < 1.0e-10;
			bool @in = FastMath.abs(offset) < 1.0e-10;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.partitioning.BSPTree<Euclidean3D> near;
			BSPTree<Euclidean3D> near;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.partitioning.BSPTree<Euclidean3D> far;
			BSPTree<Euclidean3D> far;
			if (offset < 0)
			{
				near = minus;
				far = plus;
			}
			else
			{
				near = plus;
				far = minus;
			}

			if (@in)
			{
				// search in the cut hyperplane
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.partitioning.SubHyperplane<Euclidean3D> facet = boundaryFacet(point, node);
				SubHyperplane<Euclidean3D> facet = boundaryFacet(point, node);
				if (facet != null)
				{
					return facet;
				}
			}

			// search in the near branch
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.partitioning.SubHyperplane<Euclidean3D> crossed = recurseFirstIntersection(near, point, line);
			SubHyperplane<Euclidean3D> crossed = recurseFirstIntersection(near, point, line);
			if (crossed != null)
			{
				return crossed;
			}

			if (!@in)
			{
				// search in the cut hyperplane
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D hit3D = plane.intersection(line);
				Vector3D hit3D = plane.intersection(line);
				if (hit3D != null)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.partitioning.SubHyperplane<Euclidean3D> facet = boundaryFacet(hit3D, node);
					SubHyperplane<Euclidean3D> facet = boundaryFacet(hit3D, node);
					if (facet != null)
					{
						return facet;
					}
				}
			}

			// search in the far branch
			return recurseFirstIntersection(far, point, line);

		}

		/// <summary>
		/// Check if a point belongs to the boundary part of a node. </summary>
		/// <param name="point"> point to check </param>
		/// <param name="node"> node containing the boundary facet to check </param>
		/// <returns> the boundary facet this points belongs to (or null if it
		/// does not belong to any boundary facet) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private org.apache.commons.math3.geometry.partitioning.SubHyperplane<Euclidean3D> boundaryFacet(final Vector3D point, final org.apache.commons.math3.geometry.partitioning.BSPTree<Euclidean3D> node)
		private SubHyperplane<Euclidean3D> boundaryFacet(Vector3D point, BSPTree<Euclidean3D> node)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D point2D = ((Plane) node.getCut().getHyperplane()).toSubSpace((org.apache.commons.math3.geometry.Point<Euclidean3D>) point);
			Vector2D point2D = ((Plane) node.Cut.Hyperplane).toSubSpace((Point<Euclidean3D>) point);
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final org.apache.commons.math3.geometry.partitioning.BoundaryAttribute<Euclidean3D> attribute = (org.apache.commons.math3.geometry.partitioning.BoundaryAttribute<Euclidean3D>) node.getAttribute();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			BoundaryAttribute<Euclidean3D> attribute = (BoundaryAttribute<Euclidean3D>) node.Attribute;
			if ((attribute.PlusOutside != null) && (((SubPlane) attribute.PlusOutside).RemainingRegion.checkPoint(point2D) == org.apache.commons.math3.geometry.partitioning.Region_Location.INSIDE))
			{
				return attribute.PlusOutside;
			}
			if ((attribute.PlusInside != null) && (((SubPlane) attribute.PlusInside).RemainingRegion.checkPoint(point2D) == org.apache.commons.math3.geometry.partitioning.Region_Location.INSIDE))
			{
				return attribute.PlusInside;
			}
			return null;
		}

		/// <summary>
		/// Rotate the region around the specified point.
		/// <p>The instance is not modified, a new instance is created.</p> </summary>
		/// <param name="center"> rotation center </param>
		/// <param name="rotation"> vectorial rotation operator </param>
		/// <returns> a new instance representing the rotated region </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PolyhedronsSet rotate(final Vector3D center, final Rotation rotation)
		public virtual PolyhedronsSet rotate(Vector3D center, Rotation rotation)
		{
			return (PolyhedronsSet) applyTransform(new RotationTransform(center, rotation));
		}

		/// <summary>
		/// 3D rotation as a Transform. </summary>
		private class RotationTransform : Transform<Euclidean3D, Euclidean2D>
		{

			/// <summary>
			/// Center point of the rotation. </summary>
			internal Vector3D center;

			/// <summary>
			/// Vectorial rotation. </summary>
			internal Rotation rotation;

			/// <summary>
			/// Cached original hyperplane. </summary>
			internal Plane cachedOriginal;

			/// <summary>
			/// Cached 2D transform valid inside the cached original hyperplane. </summary>
			internal Transform<Euclidean2D, Euclidean1D> cachedTransform;

			/// <summary>
			/// Build a rotation transform. </summary>
			/// <param name="center"> center point of the rotation </param>
			/// <param name="rotation"> vectorial rotation </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RotationTransform(final Vector3D center, final Rotation rotation)
			public RotationTransform(Vector3D center, Rotation rotation)
			{
				this.center = center;
				this.rotation = rotation;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector3D apply(final org.apache.commons.math3.geometry.Point<Euclidean3D> point)
			public virtual Vector3D apply(Point<Euclidean3D> point)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D delta = ((Vector3D) point).subtract(center);
				Vector3D delta = ((Vector3D) point).subtract(center);
				return new Vector3D(1.0, center, 1.0, rotation.applyTo(delta));
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Plane apply(final org.apache.commons.math3.geometry.partitioning.Hyperplane<Euclidean3D> hyperplane)
			public virtual Plane apply(Hyperplane<Euclidean3D> hyperplane)
			{
				return ((Plane) hyperplane).rotate(center, rotation);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.partitioning.SubHyperplane<org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D> apply(final org.apache.commons.math3.geometry.partitioning.SubHyperplane<org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D> sub, final org.apache.commons.math3.geometry.partitioning.Hyperplane<Euclidean3D> original, final org.apache.commons.math3.geometry.partitioning.Hyperplane<Euclidean3D> transformed)
			public virtual SubHyperplane<Euclidean2D> apply(SubHyperplane<Euclidean2D> sub, Hyperplane<Euclidean3D> original, Hyperplane<Euclidean3D> transformed)
			{
				if (original != cachedOriginal)
				{
					// we have changed hyperplane, reset the in-hyperplane transform

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane oPlane = (Plane) original;
					Plane oPlane = (Plane) original;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane tPlane = (Plane) transformed;
					Plane tPlane = (Plane) transformed;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D p00 = oPlane.getOrigin();
					Vector3D p00 = oPlane.Origin;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D p10 = oPlane.toSpace((org.apache.commons.math3.geometry.Point<org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D>) new org.apache.commons.math3.geometry.euclidean.twod.Vector2D(1.0, 0.0));
					Vector3D p10 = oPlane.toSpace((Point<Euclidean2D>) new Vector2D(1.0, 0.0));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D p01 = oPlane.toSpace((org.apache.commons.math3.geometry.Point<org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D>) new org.apache.commons.math3.geometry.euclidean.twod.Vector2D(0.0, 1.0));
					Vector3D p01 = oPlane.toSpace((Point<Euclidean2D>) new Vector2D(0.0, 1.0));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D tP00 = tPlane.toSubSpace((org.apache.commons.math3.geometry.Point<Euclidean3D>) apply(p00));
					Vector2D tP00 = tPlane.toSubSpace((Point<Euclidean3D>) apply(p00));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D tP10 = tPlane.toSubSpace((org.apache.commons.math3.geometry.Point<Euclidean3D>) apply(p10));
					Vector2D tP10 = tPlane.toSubSpace((Point<Euclidean3D>) apply(p10));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D tP01 = tPlane.toSubSpace((org.apache.commons.math3.geometry.Point<Euclidean3D>) apply(p01));
					Vector2D tP01 = tPlane.toSubSpace((Point<Euclidean3D>) apply(p01));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.awt.geom.AffineTransform at = new java.awt.geom.AffineTransform(tP10.getX() - tP00.getX(), tP10.getY() - tP00.getY(), tP01.getX() - tP00.getX(), tP01.getY() - tP00.getY(), tP00.getX(), tP00.getY());
					AffineTransform at = new AffineTransform(tP10.X - tP00.X, tP10.Y - tP00.Y, tP01.X - tP00.X, tP01.Y - tP00.Y, tP00.X, tP00.Y);

					cachedOriginal = (Plane) original;
					cachedTransform = org.apache.commons.math3.geometry.euclidean.twod.Line.getTransform(at);

				}
				return ((SubLine) sub).applyTransform(cachedTransform);
			}

		}

		/// <summary>
		/// Translate the region by the specified amount.
		/// <p>The instance is not modified, a new instance is created.</p> </summary>
		/// <param name="translation"> translation to apply </param>
		/// <returns> a new instance representing the translated region </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PolyhedronsSet translate(final Vector3D translation)
		public virtual PolyhedronsSet translate(Vector3D translation)
		{
			return (PolyhedronsSet) applyTransform(new TranslationTransform(translation));
		}

		/// <summary>
		/// 3D translation as a transform. </summary>
		private class TranslationTransform : Transform<Euclidean3D, Euclidean2D>
		{

			/// <summary>
			/// Translation vector. </summary>
			internal Vector3D translation;

			/// <summary>
			/// Cached original hyperplane. </summary>
			internal Plane cachedOriginal;

			/// <summary>
			/// Cached 2D transform valid inside the cached original hyperplane. </summary>
			internal Transform<Euclidean2D, Euclidean1D> cachedTransform;

			/// <summary>
			/// Build a translation transform. </summary>
			/// <param name="translation"> translation vector </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public TranslationTransform(final Vector3D translation)
			public TranslationTransform(Vector3D translation)
			{
				this.translation = translation;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector3D apply(final org.apache.commons.math3.geometry.Point<Euclidean3D> point)
			public virtual Vector3D apply(Point<Euclidean3D> point)
			{
				return new Vector3D(1.0, (Vector3D) point, 1.0, translation);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Plane apply(final org.apache.commons.math3.geometry.partitioning.Hyperplane<Euclidean3D> hyperplane)
			public virtual Plane apply(Hyperplane<Euclidean3D> hyperplane)
			{
				return ((Plane) hyperplane).translate(translation);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.partitioning.SubHyperplane<org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D> apply(final org.apache.commons.math3.geometry.partitioning.SubHyperplane<org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D> sub, final org.apache.commons.math3.geometry.partitioning.Hyperplane<Euclidean3D> original, final org.apache.commons.math3.geometry.partitioning.Hyperplane<Euclidean3D> transformed)
			public virtual SubHyperplane<Euclidean2D> apply(SubHyperplane<Euclidean2D> sub, Hyperplane<Euclidean3D> original, Hyperplane<Euclidean3D> transformed)
			{
				if (original != cachedOriginal)
				{
					// we have changed hyperplane, reset the in-hyperplane transform

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane oPlane = (Plane) original;
					Plane oPlane = (Plane) original;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane tPlane = (Plane) transformed;
					Plane tPlane = (Plane) transformed;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D shift = tPlane.toSubSpace((org.apache.commons.math3.geometry.Point<Euclidean3D>) apply(oPlane.getOrigin()));
					Vector2D shift = tPlane.toSubSpace((Point<Euclidean3D>) apply(oPlane.Origin));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.awt.geom.AffineTransform at = java.awt.geom.AffineTransform.getTranslateInstance(shift.getX(), shift.getY());
					AffineTransform at = AffineTransform.getTranslateInstance(shift.X, shift.Y);

					cachedOriginal = (Plane) original;
					cachedTransform = org.apache.commons.math3.geometry.euclidean.twod.Line.getTransform(at);

				}

				return ((SubLine) sub).applyTransform(cachedTransform);

			}

		}

	}

}
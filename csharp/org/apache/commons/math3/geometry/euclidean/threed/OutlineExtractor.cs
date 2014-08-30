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
	using Euclidean2D = org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D;
	using PolygonsSet = org.apache.commons.math3.geometry.euclidean.twod.PolygonsSet;
	using Vector2D = org.apache.commons.math3.geometry.euclidean.twod.Vector2D;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Extractor for <seealso cref="PolygonsSet polyhedrons sets"/> outlines.
	/// <p>This class extracts the 2D outlines from {{@link PolygonsSet
	/// polyhedrons sets} in a specified projection plane.</p>
	/// @version $Id: OutlineExtractor.java 1555174 2014-01-03 18:06:20Z luc $
	/// @since 3.0
	/// </summary>
	public class OutlineExtractor
	{

		/// <summary>
		/// Abscissa axis of the projection plane. </summary>
		private Vector3D u;

		/// <summary>
		/// Ordinate axis of the projection plane. </summary>
		private Vector3D v;

		/// <summary>
		/// Normal of the projection plane (viewing direction). </summary>
		private Vector3D w;

		/// <summary>
		/// Build an extractor for a specific projection plane. </summary>
		/// <param name="u"> abscissa axis of the projection point </param>
		/// <param name="v"> ordinate axis of the projection point </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public OutlineExtractor(final Vector3D u, final Vector3D v)
		public OutlineExtractor(Vector3D u, Vector3D v)
		{
			this.u = u;
			this.v = v;
			w = Vector3D.crossProduct(u, v);
		}

		/// <summary>
		/// Extract the outline of a polyhedrons set. </summary>
		/// <param name="polyhedronsSet"> polyhedrons set whose outline must be extracted </param>
		/// <returns> an outline, as an array of loops. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.euclidean.twod.Vector2D[][] getOutline(final PolyhedronsSet polyhedronsSet)
		public virtual Vector2D[][] getOutline(PolyhedronsSet polyhedronsSet)
		{

			// project all boundary facets into one polygons set
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BoundaryProjector projector = new BoundaryProjector(polyhedronsSet.getTolerance());
			BoundaryProjector projector = new BoundaryProjector(this, polyhedronsSet.Tolerance);
			polyhedronsSet.getTree(true).visit(projector);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.PolygonsSet projected = projector.getProjected();
			PolygonsSet projected = projector.Projected;

			// Remove the spurious intermediate vertices from the outline
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D[][] outline = projected.getVertices();
			Vector2D[][] outline = projected.Vertices;
			for (int i = 0; i < outline.Length; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D[] rawLoop = outline[i];
				Vector2D[] rawLoop = outline[i];
				int end = rawLoop.Length;
				int j = 0;
				while (j < end)
				{
					if (pointIsBetween(rawLoop, end, j))
					{
						// the point should be removed
						for (int k = j; k < (end - 1); ++k)
						{
							rawLoop[k] = rawLoop[k + 1];
						}
						--end;
					}
					else
					{
						// the point remains in the loop
						++j;
					}
				}
				if (end != rawLoop.Length)
				{
					// resize the array
					outline[i] = new Vector2D[end];
					Array.Copy(rawLoop, 0, outline[i], 0, end);
				}
			}

			return outline;

		}

		/// <summary>
		/// Check if a point is geometrically between its neighbor in an array.
		/// <p>The neighbors are computed considering the array is a loop
		/// (i.e. point at index (n-1) is before point at index 0)</p> </summary>
		/// <param name="loop"> points array </param>
		/// <param name="n"> number of points to consider in the array </param>
		/// <param name="i"> index of the point to check (must be between 0 and n-1) </param>
		/// <returns> true if the point is exactly between its neighbors </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private boolean pointIsBetween(final org.apache.commons.math3.geometry.euclidean.twod.Vector2D[] loop, final int n, final int i)
		private bool pointIsBetween(Vector2D[] loop, int n, int i)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D previous = loop[(i + n - 1) % n];
			Vector2D previous = loop[(i + n - 1) % n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D current = loop[i];
			Vector2D current = loop[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D next = loop[(i + 1) % n];
			Vector2D next = loop[(i + 1) % n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx1 = current.getX() - previous.getX();
			double dx1 = current.X - previous.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dy1 = current.getY() - previous.getY();
			double dy1 = current.Y - previous.Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx2 = next.getX() - current.getX();
			double dx2 = next.X - current.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dy2 = next.getY() - current.getY();
			double dy2 = next.Y - current.Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cross = dx1 * dy2 - dx2 * dy1;
			double cross = dx1 * dy2 - dx2 * dy1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dot = dx1 * dx2 + dy1 * dy2;
			double dot = dx1 * dx2 + dy1 * dy2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1d2 = org.apache.commons.math3.util.FastMath.sqrt((dx1 * dx1 + dy1 * dy1) * (dx2 * dx2 + dy2 * dy2));
			double d1d2 = FastMath.sqrt((dx1 * dx1 + dy1 * dy1) * (dx2 * dx2 + dy2 * dy2));
			return (FastMath.abs(cross) <= (1.0e-6 * d1d2)) && (dot >= 0.0);
		}

		/// <summary>
		/// Visitor projecting the boundary facets on a plane. </summary>
		private class BoundaryProjector : BSPTreeVisitor<Euclidean3D>
		{
			private readonly OutlineExtractor outerInstance;


			/// <summary>
			/// Projection of the polyhedrons set on the plane. </summary>
			internal PolygonsSet projected;

			/// <summary>
			/// Tolerance below which points are considered identical. </summary>
			internal readonly double tolerance;

			/// <summary>
			/// Simple constructor. </summary>
			/// <param name="tolerance"> tolerance below which points are considered identical </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BoundaryProjector(final double tolerance)
			public BoundaryProjector(OutlineExtractor outerInstance, double tolerance)
			{
				this.outerInstance = outerInstance;
				this.projected = new PolygonsSet(new BSPTree<Euclidean2D>(false), tolerance);
				this.tolerance = tolerance;
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

				// extract the vertices of the facet
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final org.apache.commons.math3.geometry.partitioning.AbstractSubHyperplane<Euclidean3D, org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D> absFacet = (org.apache.commons.math3.geometry.partitioning.AbstractSubHyperplane<Euclidean3D, org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D>) facet;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
				AbstractSubHyperplane<Euclidean3D, Euclidean2D> absFacet = (AbstractSubHyperplane<Euclidean3D, Euclidean2D>) facet;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane plane = (Plane) facet.getHyperplane();
				Plane plane = (Plane) facet.Hyperplane;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double scal = plane.getNormal().dotProduct(w);
				double scal = plane.Normal.dotProduct(outerInstance.w);
				if (FastMath.abs(scal) > 1.0e-3)
				{
					Vector2D[][] vertices = ((PolygonsSet) absFacet.RemainingRegion).Vertices;

					if ((scal < 0) ^ reversed)
					{
						// the facet is seen from the inside,
						// we need to invert its boundary orientation
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D[][] newVertices = new org.apache.commons.math3.geometry.euclidean.twod.Vector2D[vertices.length][];
						Vector2D[][] newVertices = new Vector2D[vertices.Length][];
						for (int i = 0; i < vertices.Length; ++i)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D[] loop = vertices[i];
							Vector2D[] loop = vertices[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D[] newLoop = new org.apache.commons.math3.geometry.euclidean.twod.Vector2D[loop.length];
							Vector2D[] newLoop = new Vector2D[loop.Length];
							if (loop[0] == null)
							{
								newLoop[0] = null;
								for (int j = 1; j < loop.Length; ++j)
								{
									newLoop[j] = loop[loop.Length - j];
								}
							}
							else
							{
								for (int j = 0; j < loop.Length; ++j)
								{
									newLoop[j] = loop[loop.Length - (j + 1)];
								}
							}
							newVertices[i] = newLoop;
						}

						// use the reverted vertices
						vertices = newVertices;

					}

					// compute the projection of the facet in the outline plane
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<org.apache.commons.math3.geometry.partitioning.SubHyperplane<org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D>> edges = new java.util.ArrayList<org.apache.commons.math3.geometry.partitioning.SubHyperplane<org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D>>();
					List<SubHyperplane<Euclidean2D>> edges = new List<SubHyperplane<Euclidean2D>>();
					foreach (Vector2D[] loop in vertices)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean closed = loop[0] != null;
						bool closed = loop[0] != null;
						int previous = closed ? (loop.Length - 1) : 1;
						Vector3D previous3D = plane.toSpace((Point<Euclidean2D>) loop[previous]);
						int current = (previous + 1) % loop.Length;
						Vector2D pPoint = new Vector2D(previous3D.dotProduct(outerInstance.u), previous3D.dotProduct(outerInstance.v));
						while (current < loop.Length)
						{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D current3D = plane.toSpace((org.apache.commons.math3.geometry.Point<org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D>) loop[current]);
							Vector3D current3D = plane.toSpace((Point<Euclidean2D>) loop[current]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D cPoint = new org.apache.commons.math3.geometry.euclidean.twod.Vector2D(current3D.dotProduct(u), current3D.dotProduct(v));
							Vector2D cPoint = new Vector2D(current3D.dotProduct(outerInstance.u), current3D.dotProduct(outerInstance.v));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Line line = new org.apache.commons.math3.geometry.euclidean.twod.Line(pPoint, cPoint, tolerance);
							org.apache.commons.math3.geometry.euclidean.twod.Line line = new org.apache.commons.math3.geometry.euclidean.twod.Line(pPoint, cPoint, tolerance);
							SubHyperplane<Euclidean2D> edge = line.wholeHyperplane();

							if (closed || (previous != 1))
							{
								// the previous point is a real vertex
								// it defines one bounding point of the edge
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double angle = line.getAngle() + 0.5 * org.apache.commons.math3.util.FastMath.PI;
								double angle = line.Angle + 0.5 * FastMath.PI;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Line l = new org.apache.commons.math3.geometry.euclidean.twod.Line(pPoint, angle, tolerance);
								org.apache.commons.math3.geometry.euclidean.twod.Line l = new org.apache.commons.math3.geometry.euclidean.twod.Line(pPoint, angle, tolerance);
								edge = edge.Split(l).Plus;
							}

							if (closed || (current != (loop.Length - 1)))
							{
								// the current point is a real vertex
								// it defines one bounding point of the edge
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double angle = line.getAngle() + 0.5 * org.apache.commons.math3.util.FastMath.PI;
								double angle = line.Angle + 0.5 * FastMath.PI;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Line l = new org.apache.commons.math3.geometry.euclidean.twod.Line(cPoint, angle, tolerance);
								org.apache.commons.math3.geometry.euclidean.twod.Line l = new org.apache.commons.math3.geometry.euclidean.twod.Line(cPoint, angle, tolerance);
								edge = edge.Split(l).Minus;
							}

							edges.Add(edge);

							previous = current++;
							previous3D = current3D;
							pPoint = cPoint;

						}
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.PolygonsSet projectedFacet = new org.apache.commons.math3.geometry.euclidean.twod.PolygonsSet(edges, tolerance);
					PolygonsSet projectedFacet = new PolygonsSet(edges, tolerance);

					// add the contribution of the facet to the global outline
					projected = (PolygonsSet) (new RegionFactory<Euclidean2D>()).union(projected, projectedFacet);

				}
			}

			/// <summary>
			/// Get the projection of the polyhedrons set on the plane. </summary>
			/// <returns> projection of the polyhedrons set on the plane </returns>
			public virtual PolygonsSet Projected
			{
				get
				{
					return projected;
				}
			}

		}

	}

}
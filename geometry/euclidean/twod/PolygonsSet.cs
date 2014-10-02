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
namespace mathlib.geometry.euclidean.twod
{


	using MathInternalError = mathlib.exception.MathInternalError;
	using mathlib.geometry;
	using Euclidean1D = mathlib.geometry.euclidean.oned.Euclidean1D;
	using Interval = mathlib.geometry.euclidean.oned.Interval;
	using IntervalsSet = mathlib.geometry.euclidean.oned.IntervalsSet;
	using Vector1D = mathlib.geometry.euclidean.oned.Vector1D;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using Side = mathlib.geometry.partitioning.Side;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning.utilities;
	using OrderedTuple = mathlib.geometry.partitioning.utilities.OrderedTuple;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// This class represents a 2D region: a set of polygons.
	/// @version $Id: PolygonsSet.java 1590560 2014-04-28 06:39:01Z luc $
	/// @since 3.0
	/// </summary>
	public class PolygonsSet : AbstractRegion<Euclidean2D, Euclidean1D>
	{

		/// <summary>
		/// Default value for tolerance. </summary>
		private const double DEFAULT_TOLERANCE = 1.0e-10;

		/// <summary>
		/// Vertices organized as boundary loops. </summary>
		private Vector2D[][] vertices;

		/// <summary>
		/// Build a polygons set representing the whole plane. </summary>
		/// <param name="tolerance"> tolerance below which points are considered identical
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PolygonsSet(final double tolerance)
		public PolygonsSet(double tolerance) : base(tolerance)
		{
		}

		/// <summary>
		/// Build a polygons set from a BSP tree.
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
//ORIGINAL LINE: public PolygonsSet(final mathlib.geometry.partitioning.BSPTree<Euclidean2D> tree, final double tolerance)
		public PolygonsSet(BSPTree<Euclidean2D> tree, double tolerance) : base(tree, tolerance)
		{
		}

		/// <summary>
		/// Build a polygons set from a Boundary REPresentation (B-rep).
		/// <p>The boundary is provided as a collection of {@link
		/// SubHyperplane sub-hyperplanes}. Each sub-hyperplane has the
		/// interior part of the region on its minus side and the exterior on
		/// its plus side.</p>
		/// <p>The boundary elements can be in any order, and can form
		/// several non-connected sets (like for example polygons with holes
		/// or a set of disjoint polygons considered as a whole). In
		/// fact, the elements do not even need to be connected together
		/// (their topological connections are not used here). However, if the
		/// boundary does not really separate an inside open from an outside
		/// open (open having here its topological meaning), then subsequent
		/// calls to the {@link
		/// mathlib.geometry.partitioning.Region#checkPoint(mathlib.geometry.Point)
		/// checkPoint} method will not be meaningful anymore.</p>
		/// <p>If the boundary is empty, the region will represent the whole
		/// space.</p> </summary>
		/// <param name="boundary"> collection of boundary elements, as a
		/// collection of <seealso cref="SubHyperplane SubHyperplane"/> objects </param>
		/// <param name="tolerance"> tolerance below which points are considered identical
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PolygonsSet(final java.util.Collection<mathlib.geometry.partitioning.SubHyperplane<Euclidean2D>> boundary, final double tolerance)
		public PolygonsSet(ICollection<SubHyperplane<Euclidean2D>> boundary, double tolerance) : base(boundary, tolerance)
		{
		}

		/// <summary>
		/// Build a parallellepipedic box. </summary>
		/// <param name="xMin"> low bound along the x direction </param>
		/// <param name="xMax"> high bound along the x direction </param>
		/// <param name="yMin"> low bound along the y direction </param>
		/// <param name="yMax"> high bound along the y direction </param>
		/// <param name="tolerance"> tolerance below which points are considered identical
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PolygonsSet(final double xMin, final double xMax, final double yMin, final double yMax, final double tolerance)
		public PolygonsSet(double xMin, double xMax, double yMin, double yMax, double tolerance) : base(boxBoundary(xMin, xMax, yMin, yMax, tolerance), tolerance)
		{
		}

		/// <summary>
		/// Build a polygon from a simple list of vertices.
		/// <p>The boundary is provided as a list of points considering to
		/// represent the vertices of a simple loop. The interior part of the
		/// region is on the left side of this path and the exterior is on its
		/// right side.</p>
		/// <p>This constructor does not handle polygons with a boundary
		/// forming several disconnected paths (such as polygons with holes).</p>
		/// <p>For cases where this simple constructor applies, it is expected to
		/// be numerically more robust than the {@link #PolygonsSet(Collection) general
		/// constructor} using <seealso cref="SubHyperplane subhyperplanes"/>.</p>
		/// <p>If the list is empty, the region will represent the whole
		/// space.</p>
		/// <p>
		/// Polygons with thin pikes or dents are inherently difficult to handle because
		/// they involve lines with almost opposite directions at some vertices. Polygons
		/// whose vertices come from some physical measurement with noise are also
		/// difficult because an edge that should be straight may be broken in lots of
		/// different pieces with almost equal directions. In both cases, computing the
		/// lines intersections is not numerically robust due to the almost 0 or almost
		/// &pi; angle. Such cases need to carefully adjust the {@code hyperplaneThickness}
		/// parameter. A too small value would often lead to completely wrong polygons
		/// with large area wrongly identified as inside or outside. Large values are
		/// often much safer. As a rule of thumb, a value slightly below the size of the
		/// most accurate detail needed is a good value for the {@code hyperplaneThickness}
		/// parameter.
		/// </p> </summary>
		/// <param name="hyperplaneThickness"> tolerance below which points are considered to
		/// belong to the hyperplane (which is therefore more a slab) </param>
		/// <param name="vertices"> vertices of the simple loop boundary </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PolygonsSet(final double hyperplaneThickness, final Vector2D... vertices)
		public PolygonsSet(double hyperplaneThickness, params Vector2D[] vertices) : base(verticesToTree(hyperplaneThickness, vertices), hyperplaneThickness)
		{
		}

		/// <summary>
		/// Build a polygons set representing the whole real line. </summary>
		/// @deprecated as of 3.3, replaced with <seealso cref="#PolygonsSet(double)"/> 
		[Obsolete]//("as of 3.3, replaced with <seealso cref="#PolygonsSet(double)"/>")]
		public PolygonsSet() : this(DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Build a polygons set from a BSP tree.
		/// <p>The leaf nodes of the BSP tree <em>must</em> have a
		/// {@code Boolean} attribute representing the inside status of
		/// the corresponding cell (true for inside cells, false for outside
		/// cells). In order to avoid building too many small objects, it is
		/// recommended to use the predefined constants
		/// {@code Boolean.TRUE} and {@code Boolean.FALSE}</p> </summary>
		/// <param name="tree"> inside/outside BSP tree representing the region </param>
		/// @deprecated as of 3.3, replaced with <seealso cref="#PolygonsSet(BSPTree, double)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3, replaced with <seealso cref="#PolygonsSet(mathlib.geometry.partitioning.BSPTree, double)"/>") public PolygonsSet(final mathlib.geometry.partitioning.BSPTree<Euclidean2D> tree)
		[Obsolete]//("as of 3.3, replaced with <seealso cref="#PolygonsSet(mathlib.geometry.partitioning.BSPTree, double)"/>")]
		public PolygonsSet(BSPTree<Euclidean2D> tree) : this(tree, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Build a polygons set from a Boundary REPresentation (B-rep).
		/// <p>The boundary is provided as a collection of {@link
		/// SubHyperplane sub-hyperplanes}. Each sub-hyperplane has the
		/// interior part of the region on its minus side and the exterior on
		/// its plus side.</p>
		/// <p>The boundary elements can be in any order, and can form
		/// several non-connected sets (like for example polygons with holes
		/// or a set of disjoint polygons considered as a whole). In
		/// fact, the elements do not even need to be connected together
		/// (their topological connections are not used here). However, if the
		/// boundary does not really separate an inside open from an outside
		/// open (open having here its topological meaning), then subsequent
		/// calls to the {@link
		/// mathlib.geometry.partitioning.Region#checkPoint(mathlib.geometry.Point)
		/// checkPoint} method will not be meaningful anymore.</p>
		/// <p>If the boundary is empty, the region will represent the whole
		/// space.</p> </summary>
		/// <param name="boundary"> collection of boundary elements, as a
		/// collection of <seealso cref="SubHyperplane SubHyperplane"/> objects </param>
		/// @deprecated as of 3.3, replaced with <seealso cref="#PolygonsSet(Collection, double)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3, replaced with <seealso cref="#PolygonsSet(java.util.Collection, double)"/>") public PolygonsSet(final java.util.Collection<mathlib.geometry.partitioning.SubHyperplane<Euclidean2D>> boundary)
		[Obsolete]//("as of 3.3, replaced with <seealso cref="#PolygonsSet(java.util.Collection, double)"/>")]
		public PolygonsSet(ICollection<SubHyperplane<Euclidean2D>> boundary) : this(boundary, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Build a parallellepipedic box. </summary>
		/// <param name="xMin"> low bound along the x direction </param>
		/// <param name="xMax"> high bound along the x direction </param>
		/// <param name="yMin"> low bound along the y direction </param>
		/// <param name="yMax"> high bound along the y direction </param>
		/// @deprecated as of 3.3, replaced with <seealso cref="#PolygonsSet(double, double, double, double, double)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3, replaced with <seealso cref="#PolygonsSet(double, double, double, double, double)"/>") public PolygonsSet(final double xMin, final double xMax, final double yMin, final double yMax)
		[Obsolete]//("as of 3.3, replaced with <seealso cref="#PolygonsSet(double, double, double, double, double)"/>")]
		public PolygonsSet(double xMin, double xMax, double yMin, double yMax) : this(xMin, xMax, yMin, yMax, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Create a list of hyperplanes representing the boundary of a box. </summary>
		/// <param name="xMin"> low bound along the x direction </param>
		/// <param name="xMax"> high bound along the x direction </param>
		/// <param name="yMin"> low bound along the y direction </param>
		/// <param name="yMax"> high bound along the y direction </param>
		/// <param name="tolerance"> tolerance below which points are considered identical </param>
		/// <returns> boundary of the box </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static Line[] boxBoundary(final double xMin, final double xMax, final double yMin, final double yMax, final double tolerance)
		private static Line[] boxBoundary(double xMin, double xMax, double yMin, double yMax, double tolerance)
		{
			if ((xMin >= xMax - tolerance) || (yMin >= yMax - tolerance))
			{
				// too thin box, build an empty polygons set
				return null;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D minMin = new Vector2D(xMin, yMin);
			Vector2D minMin = new Vector2D(xMin, yMin);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D minMax = new Vector2D(xMin, yMax);
			Vector2D minMax = new Vector2D(xMin, yMax);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D maxMin = new Vector2D(xMax, yMin);
			Vector2D maxMin = new Vector2D(xMax, yMin);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D maxMax = new Vector2D(xMax, yMax);
			Vector2D maxMax = new Vector2D(xMax, yMax);
			return new Line[] {new Line(minMin, maxMin, tolerance), new Line(maxMin, maxMax, tolerance), new Line(maxMax, minMax, tolerance), new Line(minMax, minMin, tolerance)};
		}

		/// <summary>
		/// Build the BSP tree of a polygons set from a simple list of vertices.
		/// <p>The boundary is provided as a list of points considering to
		/// represent the vertices of a simple loop. The interior part of the
		/// region is on the left side of this path and the exterior is on its
		/// right side.</p>
		/// <p>This constructor does not handle polygons with a boundary
		/// forming several disconnected paths (such as polygons with holes).</p>
		/// <p>For cases where this simple constructor applies, it is expected to
		/// be numerically more robust than the {@link #PolygonsSet(Collection) general
		/// constructor} using <seealso cref="SubHyperplane subhyperplanes"/>.</p> </summary>
		/// <param name="hyperplaneThickness"> tolerance below which points are consider to
		/// belong to the hyperplane (which is therefore more a slab) </param>
		/// <param name="vertices"> vertices of the simple loop boundary </param>
		/// <returns> the BSP tree of the input vertices </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static mathlib.geometry.partitioning.BSPTree<Euclidean2D> verticesToTree(final double hyperplaneThickness, final Vector2D... vertices)
		private static BSPTree<Euclidean2D> verticesToTree(double hyperplaneThickness, params Vector2D[] vertices)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = vertices.length;
			int n = vertices.Length;
			if (n == 0)
			{
				// the tree represents the whole space
				return new BSPTree<Euclidean2D>(true);
			}

			// build the vertices
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vertex[] vArray = new Vertex[n];
			Vertex[] vArray = new Vertex[n];
			for (int i = 0; i < n; ++i)
			{
				vArray[i] = new Vertex(vertices[i]);
			}

			// build the edges
			IList<Edge> edges = new List<Edge>(n);
			for (int i = 0; i < n; ++i)
			{

				// get the endpoints of the edge
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vertex start = vArray[i];
				Vertex start = vArray[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vertex end = vArray[(i + 1) % n];
				Vertex end = vArray[(i + 1) % n];

				// get the line supporting the edge, taking care not to recreate it
				// if it was already created earlier due to another edge being aligned
				// with the current one
				Line line = start.sharedLineWith(end);
				if (line == null)
				{
					line = new Line(start.Location, end.Location, hyperplaneThickness);
				}

				// create the edge and store it
				edges.Add(new Edge(start, end, line));

				// check if another vertex also happens to be on this line
				foreach (Vertex vertex in vArray)
				{
					if (vertex != start && vertex != end && FastMath.abs(line.getOffset((Point<Euclidean2D>) vertex.Location)) <= hyperplaneThickness)
					{
						vertex.bindWith(line);
					}
				}

			}

			// build the tree top-down
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<Euclidean2D> tree = new mathlib.geometry.partitioning.BSPTree<Euclidean2D>();
			BSPTree<Euclidean2D> tree = new BSPTree<Euclidean2D>();
			insertEdges(hyperplaneThickness, tree, edges);

			return tree;

		}

		/// <summary>
		/// Recursively build a tree by inserting cut sub-hyperplanes. </summary>
		/// <param name="hyperplaneThickness"> tolerance below which points are consider to
		/// belong to the hyperplane (which is therefore more a slab) </param>
		/// <param name="node"> current tree node (it is a leaf node at the beginning
		/// of the call) </param>
		/// <param name="edges"> list of edges to insert in the cell defined by this node
		/// (excluding edges not belonging to the cell defined by this node) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void insertEdges(final double hyperplaneThickness, final mathlib.geometry.partitioning.BSPTree<Euclidean2D> node, final java.util.List<Edge> edges)
		private static void insertEdges(double hyperplaneThickness, BSPTree<Euclidean2D> node, IList<Edge> edges)
		{

			// find an edge with an hyperplane that can be inserted in the node
			int index = 0;
			Edge inserted = null;
			while (inserted == null && index < edges.Count)
			{
				inserted = edges[index++];
				if (inserted.Node == null)
				{
					if (node.insertCut(inserted.Line))
					{
						inserted.Node = node;
					}
					else
					{
						inserted = null;
					}
				}
				else
				{
					inserted = null;
				}
			}

			if (inserted == null)
			{
				// no suitable edge was found, the node remains a leaf node
				// we need to set its inside/outside boolean indicator
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<Euclidean2D> parent = node.getParent();
				BSPTree<Euclidean2D> parent = node.Parent;
				if (parent == null || node == parent.Minus)
				{
					node.Attribute = true;
				}
				else
				{
					node.Attribute = false;
				}
				return;
			}

			// we have split the node by inserting an edge as a cut sub-hyperplane
			// distribute the remaining edges in the two sub-trees
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Edge> plusList = new java.util.ArrayList<Edge>();
			IList<Edge> plusList = new List<Edge>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Edge> minusList = new java.util.ArrayList<Edge>();
			IList<Edge> minusList = new List<Edge>();
			foreach (Edge edge in edges)
			{
				if (edge != inserted)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double startOffset = inserted.getLine().getOffset((mathlib.geometry.Point<Euclidean2D>) edge.getStart().getLocation());
					double startOffset = inserted.Line.getOffset((Point<Euclidean2D>) edge.Start.Location);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double endOffset = inserted.getLine().getOffset((mathlib.geometry.Point<Euclidean2D>) edge.getEnd().getLocation());
					double endOffset = inserted.Line.getOffset((Point<Euclidean2D>) edge.End.Location);
					Side startSide = (FastMath.abs(startOffset) <= hyperplaneThickness) ? Side.HYPER : ((startOffset < 0) ? Side.MINUS : Side.PLUS);
					Side endSide = (FastMath.abs(endOffset) <= hyperplaneThickness) ? Side.HYPER : ((endOffset < 0) ? Side.MINUS : Side.PLUS);
					switch (startSide)
					{
						case Side.PLUS:
							if (endSide == Side.MINUS)
							{
								// we need to insert a split point on the hyperplane
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vertex splitPoint = edge.split(inserted.getLine());
								Vertex splitPoint = edge.Split(inserted.Line);
								minusList.Add(splitPoint.Outgoing);
								plusList.Add(splitPoint.Incoming);
							}
							else
							{
								plusList.Add(edge);
							}
							break;
						case Side.MINUS:
							if (endSide == Side.PLUS)
							{
								// we need to insert a split point on the hyperplane
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vertex splitPoint = edge.split(inserted.getLine());
								Vertex splitPoint = edge.Split(inserted.Line);
								minusList.Add(splitPoint.Incoming);
								plusList.Add(splitPoint.Outgoing);
							}
							else
							{
								minusList.Add(edge);
							}
							break;
						default:
							if (endSide == Side.PLUS)
							{
								plusList.Add(edge);
							}
							else if (endSide == Side.MINUS)
							{
								minusList.Add(edge);
							}
							break;
					}
				}
			}

			// recurse through lower levels
			if (plusList.Count > 0)
			{
				insertEdges(hyperplaneThickness, node.Plus, plusList);
			}
			else
			{
				node.Plus.Attribute = false;
			}
			if (minusList.Count > 0)
			{
				insertEdges(hyperplaneThickness, node.Minus, minusList);
			}
			else
			{
				node.Minus.Attribute = true;
			}

		}

		/// <summary>
		/// Internal class for holding vertices while they are processed to build a BSP tree. </summary>
		private class Vertex
		{

			/// <summary>
			/// Vertex location. </summary>
			internal readonly Vector2D location;

			/// <summary>
			/// Incoming edge. </summary>
			internal Edge incoming;

			/// <summary>
			/// Outgoing edge. </summary>
			internal Edge outgoing;

			/// <summary>
			/// Lines bound with this vertex. </summary>
			internal readonly IList<Line> lines;

			/// <summary>
			/// Build a non-processed vertex not owned by any node yet. </summary>
			/// <param name="location"> vertex location </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vertex(final Vector2D location)
			public Vertex(Vector2D location)
			{
				this.location = location;
				this.incoming = null;
				this.outgoing = null;
				this.lines = new List<Line>();
			}

			/// <summary>
			/// Get Vertex location. </summary>
			/// <returns> vertex location </returns>
			public virtual Vector2D Location
			{
				get
				{
					return location;
				}
			}

			/// <summary>
			/// Bind a line considered to contain this vertex. </summary>
			/// <param name="line"> line to bind with this vertex </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void bindWith(final Line line)
			public virtual void bindWith(Line line)
			{
				lines.Add(line);
			}

			/// <summary>
			/// Get the common line bound with both the instance and another vertex, if any.
			/// <p>
			/// When two vertices are both bound to the same line, this means they are
			/// already handled by node associated with this line, so there is no need
			/// to create a cut hyperplane for them.
			/// </p> </summary>
			/// <param name="vertex"> other vertex to check instance against </param>
			/// <returns> line bound with both the instance and another vertex, or null if the
			/// two vertices do not share a line yet </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Line sharedLineWith(final Vertex vertex)
			public virtual Line sharedLineWith(Vertex vertex)
			{
				foreach (Line line1 in lines)
				{
					foreach (Line line2 in vertex.lines)
					{
						if (line1 == line2)
						{
							return line1;
						}
					}
				}
				return null;
			}

			/// <summary>
			/// Set incoming edge.
			/// <p>
			/// The line supporting the incoming edge is automatically bound
			/// with the instance.
			/// </p> </summary>
			/// <param name="incoming"> incoming edge </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setIncoming(final Edge incoming)
			public virtual Edge Incoming
			{
				set
				{
					this.incoming = value;
					bindWith(value.Line);
				}
				get
				{
					return incoming;
				}
			}


			/// <summary>
			/// Set outgoing edge.
			/// <p>
			/// The line supporting the outgoing edge is automatically bound
			/// with the instance.
			/// </p> </summary>
			/// <param name="outgoing"> outgoing edge </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setOutgoing(final Edge outgoing)
			public virtual Edge Outgoing
			{
				set
				{
					this.outgoing = value;
					bindWith(value.Line);
				}
				get
				{
					return outgoing;
				}
			}


		}

		/// <summary>
		/// Internal class for holding edges while they are processed to build a BSP tree. </summary>
		private class Edge
		{

			/// <summary>
			/// Start vertex. </summary>
			internal readonly Vertex start;

			/// <summary>
			/// End vertex. </summary>
			internal readonly Vertex end;

			/// <summary>
			/// Line supporting the edge. </summary>
			internal readonly Line line;

			/// <summary>
			/// Node whose cut hyperplane contains this edge. </summary>
			internal BSPTree<Euclidean2D> node;

			/// <summary>
			/// Build an edge not contained in any node yet. </summary>
			/// <param name="start"> start vertex </param>
			/// <param name="end"> end vertex </param>
			/// <param name="line"> line supporting the edge </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Edge(final Vertex start, final Vertex end, final Line line)
			public Edge(Vertex start, Vertex end, Line line)
			{

				this.start = start;
				this.end = end;
				this.line = line;
				this.node = null;

				// connect the vertices back to the edge
				start.Outgoing = this;
				end.Incoming = this;

			}

			/// <summary>
			/// Get start vertex. </summary>
			/// <returns> start vertex </returns>
			public virtual Vertex Start
			{
				get
				{
					return start;
				}
			}

			/// <summary>
			/// Get end vertex. </summary>
			/// <returns> end vertex </returns>
			public virtual Vertex End
			{
				get
				{
					return end;
				}
			}

			/// <summary>
			/// Get the line supporting this edge. </summary>
			/// <returns> line supporting this edge </returns>
			public virtual Line Line
			{
				get
				{
					return line;
				}
			}

			/// <summary>
			/// Set the node whose cut hyperplane contains this edge. </summary>
			/// <param name="node"> node whose cut hyperplane contains this edge </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setNode(final mathlib.geometry.partitioning.BSPTree<Euclidean2D> node)
			public virtual BSPTree<Euclidean2D> Node
			{
				set
				{
					this.node = value;
				}
				get
				{
					return node;
				}
			}


			/// <summary>
			/// Split the edge.
			/// <p>
			/// Once split, this edge is not referenced anymore by the vertices,
			/// it is replaced by the two half-edges and an intermediate splitting
			/// vertex is introduced to connect these two halves.
			/// </p> </summary>
			/// <param name="splitLine"> line splitting the edge in two halves </param>
			/// <returns> split vertex (its incoming and outgoing edges are the two halves) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vertex split(final Line splitLine)
			public virtual Vertex Split(Line splitLine)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vertex splitVertex = new Vertex(line.intersection(splitLine));
				Vertex splitVertex = new Vertex(line.intersection(splitLine));
				splitVertex.bindWith(splitLine);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Edge startHalf = new Edge(start, splitVertex, line);
				Edge startHalf = new Edge(start, splitVertex, line);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Edge endHalf = new Edge(splitVertex, end, line);
				Edge endHalf = new Edge(splitVertex, end, line);
				startHalf.node = node;
				endHalf.node = node;
				return splitVertex;
			}

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public PolygonsSet buildNew(final mathlib.geometry.partitioning.BSPTree<Euclidean2D> tree)
		public override PolygonsSet buildNew(BSPTree<Euclidean2D> tree)
		{
			return new PolygonsSet(tree, Tolerance);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override void computeGeometricalProperties()
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D[][] v = getVertices();
			Vector2D[][] v = Vertices;

			if (v.Length == 0)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<Euclidean2D> tree = getTree(false);
				BSPTree<Euclidean2D> tree = getTree(false);
				if (tree.Cut == null && (bool?) tree.Attribute)
				{
					// the instance covers the whole space
					Size = double.PositiveInfinity;
					Barycenter = (Point<Euclidean2D>) Vector2D.NaN_Renamed;
				}
				else
				{
					Size = 0;
					Barycenter = (Point<Euclidean2D>) new Vector2D(0, 0);
				}
			}
			else if (v[0][0] == null)
			{
				// there is at least one open-loop: the polygon is infinite
				Size = double.PositiveInfinity;
				Barycenter = (Point<Euclidean2D>) Vector2D.NaN_Renamed;
			}
			else
			{
				// all loops are closed, we compute some integrals around the shape

				double sum = 0;
				double sumX = 0;
				double sumY = 0;

				foreach (Vector2D[] loop in v)
				{
					double x1 = loop[loop.Length - 1].X;
					double y1 = loop[loop.Length - 1].Y;
					foreach (Vector2D point in loop)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x0 = x1;
						double x0 = x1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y0 = y1;
						double y0 = y1;
						x1 = point.X;
						y1 = point.Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double factor = x0 * y1 - y0 * x1;
						double factor = x0 * y1 - y0 * x1;
						sum += factor;
						sumX += factor * (x0 + x1);
						sumY += factor * (y0 + y1);
					}
				}

				if (sum < 0)
				{
					// the polygon as a finite outside surrounded by an infinite inside
					Size = double.PositiveInfinity;
					Barycenter = (Point<Euclidean2D>) Vector2D.NaN_Renamed;
				}
				else
				{
					Size = sum / 2;
					Barycenter = (Point<Euclidean2D>) new Vector2D(sumX / (3 * sum), sumY / (3 * sum));
				}

			}

		}

		/// <summary>
		/// Get the vertices of the polygon.
		/// <p>The polygon boundary can be represented as an array of loops,
		/// each loop being itself an array of vertices.</p>
		/// <p>In order to identify open loops which start and end by
		/// infinite edges, the open loops arrays start with a null point. In
		/// this case, the first non null point and the last point of the
		/// array do not represent real vertices, they are dummy points
		/// intended only to get the direction of the first and last edge. An
		/// open loop consisting of a single infinite line will therefore be
		/// represented by a three elements array with one null point
		/// followed by two dummy points. The open loops are always the first
		/// ones in the loops array.</p>
		/// <p>If the polygon has no boundary at all, a zero length loop
		/// array will be returned.</p>
		/// <p>All line segments in the various loops have the inside of the
		/// region on their left side and the outside on their right side
		/// when moving in the underlying line direction. This means that
		/// closed loops surrounding finite areas obey the direct
		/// trigonometric orientation.</p> </summary>
		/// <returns> vertices of the polygon, organized as oriented boundary
		/// loops with the open loops first (the returned value is guaranteed
		/// to be non-null) </returns>
		public virtual Vector2D[][] Vertices
		{
			get
			{
				if (vertices == null)
				{
					if (getTree(false).Cut == null)
					{
						vertices = new Vector2D[0][];
					}
					else
					{
    
						// sort the segments according to their start point
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final SegmentsBuilder visitor = new SegmentsBuilder();
						SegmentsBuilder visitor = new SegmentsBuilder();
						getTree(true).visit(visitor);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final mathlib.geometry.partitioning.utilities.AVLTree<ComparableSegment> sorted = visitor.getSorted();
						AVLTree<ComparableSegment> sorted = visitor.Sorted;
    
						// identify the loops, starting from the open ones
						// (their start segments are naturally at the sorted set beginning)
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.ArrayList<java.util.List<ComparableSegment>> loops = new java.util.ArrayList<java.util.List<ComparableSegment>>();
						List<IList<ComparableSegment>> loops = new List<IList<ComparableSegment>>();
						while (!sorted.Empty)
						{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final mathlib.geometry.partitioning.utilities.AVLTree<ComparableSegment>.Node node = sorted.getSmallest();
							AVLTree<ComparableSegment>.Node node = sorted.Smallest;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.List<ComparableSegment> loop = followLoop(node, sorted);
							IList<ComparableSegment> loop = followLoop(node, sorted);
							if (loop != null)
							{
								loops.Add(loop);
							}
						}
    
						// transform the loops in an array of arrays of points
						vertices = new Vector2D[loops.Count][];
						int i = 0;
    
						foreach (IList<ComparableSegment> loop in loops)
						{
							if (loop.size() < 2)
							{
								// single infinite line
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Line line = loop.get(0).getLine();
								Line line = loop.get(0).Line;
								vertices[i++] = new Vector2D[] {null, line.toSpace((Point<Euclidean1D>) new Vector1D(-float.MaxValue)), line.toSpace((Point<Euclidean1D>) new Vector1D(+float.MaxValue))};
							}
							else if (loop.get(0).Start == null)
							{
								// open loop with at least one real point
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Vector2D[] array = new Vector2D[loop.size() + 2];
								Vector2D[] array = new Vector2D[loop.size() + 2];
								int j = 0;
								foreach (Segment segment in loop)
								{
    
									if (j == 0)
									{
										// null point and first dummy point
										double x = segment.Line.toSubSpace((Point<Euclidean2D>) segment.End).X;
										x -= FastMath.max(1.0, FastMath.abs(x / 2));
										array[j++] = null;
										array[j++] = segment.Line.toSpace((Point<Euclidean1D>) new Vector1D(x));
									}
    
									if (j < (array.Length - 1))
									{
										// current point
										array[j++] = segment.End;
									}
    
									if (j == (array.Length - 1))
									{
										// last dummy point
										double x = segment.Line.toSubSpace((Point<Euclidean2D>) segment.Start).X;
										x += FastMath.max(1.0, FastMath.abs(x / 2));
										array[j++] = segment.Line.toSpace((Point<Euclidean1D>) new Vector1D(x));
									}
    
								}
								vertices[i++] = array;
							}
							else
							{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Vector2D[] array = new Vector2D[loop.size()];
								Vector2D[] array = new Vector2D[loop.size()];
								int j = 0;
								foreach (Segment segment in loop)
								{
									array[j++] = segment.Start;
								}
								vertices[i++] = array;
							}
						}
    
					}
				}
    
				return vertices.clone();
    
			}
		}

		/// <summary>
		/// Follow a boundary loop. </summary>
		/// <param name="node"> node containing the segment starting the loop </param>
		/// <param name="sorted"> set of segments belonging to the boundary, sorted by
		/// start points (contains {@code node}) </param>
		/// <returns> a list of connected sub-hyperplanes starting at
		/// {@code node} </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private java.util.List<ComparableSegment> followLoop(final mathlib.geometry.partitioning.utilities.AVLTree<ComparableSegment>.Node node, final mathlib.geometry.partitioning.utilities.AVLTree<ComparableSegment> sorted)
		private IList<ComparableSegment> followLoop(AVLTree<ComparableSegment>.Node node, AVLTree<ComparableSegment> sorted)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<ComparableSegment> loop = new java.util.ArrayList<ComparableSegment>();
			List<ComparableSegment> loop = new List<ComparableSegment>();
			ComparableSegment segment = node.Element;
			loop.Add(segment);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D globalStart = segment.getStart();
			Vector2D globalStart = segment.Start;
			Vector2D end = segment.End;
			node.delete();

			// is this an open or a closed loop ?
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean open = segment.getStart() == null;
			bool open = segment.Start == null;

			while ((end != null) && (open || (globalStart.distance((Point<Euclidean2D>) end) > 1.0e-10)))
			{

				// search the sub-hyperplane starting where the previous one ended
				AVLTree<ComparableSegment>.Node selectedNode = null;
				ComparableSegment selectedSegment = null;
				double selectedDistance = double.PositiveInfinity;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ComparableSegment lowerLeft = new ComparableSegment(end, -1.0e-10, -1.0e-10);
				ComparableSegment lowerLeft = new ComparableSegment(end, -1.0e-10, -1.0e-10);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ComparableSegment upperRight = new ComparableSegment(end, +1.0e-10, +1.0e-10);
				ComparableSegment upperRight = new ComparableSegment(end, +1.0e-10, +1.0e-10);
				for (AVLTree<ComparableSegment>.Node n = sorted.getNotSmaller(lowerLeft); (n != null) && (n.Element.compareTo(upperRight) <= 0); n = n.Next)
				{
					segment = n.Element;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double distance = end.distance((mathlib.geometry.Point<Euclidean2D>) segment.getStart());
					double distance = end.distance((Point<Euclidean2D>) segment.Start);
					if (distance < selectedDistance)
					{
						selectedNode = n;
						selectedSegment = segment;
						selectedDistance = distance;
					}
				}

				if (selectedDistance > 1.0e-10)
				{
					// this is a degenerated loop, it probably comes from a very
					// tiny region with some segments smaller than the threshold, we
					// simply ignore it
					return null;
				}

				end = selectedSegment.End;
				loop.Add(selectedSegment);
				selectedNode.delete();

			}

			if ((loop.Count == 2) && !open)
			{
				// this is a degenerated infinitely thin loop, we simply ignore it
				return null;
			}

			if ((end == null) && !open)
			{
				throw new MathInternalError();
			}

			return loop;

		}

		/// <summary>
		/// Private extension of Segment allowing comparison. </summary>
		private class ComparableSegment : Segment, IComparable<ComparableSegment>
		{

			/// <summary>
			/// Sorting key. </summary>
			internal OrderedTuple sortingKey;

			/// <summary>
			/// Build a segment. </summary>
			/// <param name="start"> start point of the segment </param>
			/// <param name="end"> end point of the segment </param>
			/// <param name="line"> line containing the segment </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ComparableSegment(final Vector2D start, final Vector2D end, final Line line)
			public ComparableSegment(Vector2D start, Vector2D end, Line line) : base(start, end, line)
			{
				sortingKey = (start == null) ? new OrderedTuple(double.NegativeInfinity, double.NegativeInfinity) : new OrderedTuple(start.X, start.Y);
			}

			/// <summary>
			/// Build a dummy segment.
			/// <p>
			/// The object built is not a real segment, only the sorting key is used to
			/// allow searching in the neighborhood of a point. This is an horrible hack ...
			/// </p> </summary>
			/// <param name="start"> start point of the segment </param>
			/// <param name="dx"> abscissa offset from the start point </param>
			/// <param name="dy"> ordinate offset from the start point </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ComparableSegment(final Vector2D start, final double dx, final double dy)
			public ComparableSegment(Vector2D start, double dx, double dy) : base(null, null, null)
			{
				sortingKey = new OrderedTuple(start.X + dx, start.Y + dy);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public int compareTo(final ComparableSegment o)
			public virtual int CompareTo(ComparableSegment o)
			{
				return sortingKey.CompareTo(o.sortingKey);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public boolean equals(final Object other)
			public override bool Equals(object other)
			{
				if (this == other)
				{
					return true;
				}
				else if (other is ComparableSegment)
				{
					return compareTo((ComparableSegment) other) == 0;
				}
				else
				{
					return false;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override int GetHashCode()
			{
				return Start.GetHashCode() ^ End.GetHashCode() ^ Line.GetHashCode() ^ sortingKey.GetHashCode();
			}

		}

		/// <summary>
		/// Visitor building segments. </summary>
		private class SegmentsBuilder : BSPTreeVisitor<Euclidean2D>
		{

			/// <summary>
			/// Sorted segments. </summary>
			internal AVLTree<ComparableSegment> sorted;

			/// <summary>
			/// Simple constructor. </summary>
			public SegmentsBuilder()
			{
				sorted = new AVLTree<ComparableSegment>();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.geometry.partitioning.BSPTreeVisitor_Order visitOrder(final mathlib.geometry.partitioning.BSPTree<Euclidean2D> node)
			public virtual mathlib.geometry.partitioning.BSPTreeVisitor_Order visitOrder(BSPTree<Euclidean2D> node)
			{
				return mathlib.geometry.partitioning.BSPTreeVisitor_Order.MINUS_SUB_PLUS;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visitInternalNode(final mathlib.geometry.partitioning.BSPTree<Euclidean2D> node)
			public virtual void visitInternalNode(BSPTree<Euclidean2D> node)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final mathlib.geometry.partitioning.BoundaryAttribute<Euclidean2D> attribute = (mathlib.geometry.partitioning.BoundaryAttribute<Euclidean2D>) node.getAttribute();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
				BoundaryAttribute<Euclidean2D> attribute = (BoundaryAttribute<Euclidean2D>) node.Attribute;
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
//ORIGINAL LINE: public void visitLeafNode(final mathlib.geometry.partitioning.BSPTree<Euclidean2D> node)
			public virtual void visitLeafNode(BSPTree<Euclidean2D> node)
			{
			}

			/// <summary>
			/// Add the contribution of a boundary facet. </summary>
			/// <param name="sub"> boundary facet </param>
			/// <param name="reversed"> if true, the facet has the inside on its plus side </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void addContribution(final mathlib.geometry.partitioning.SubHyperplane<Euclidean2D> sub, final boolean reversed)
			internal virtual void addContribution(SubHyperplane<Euclidean2D> sub, bool reversed)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final mathlib.geometry.partitioning.AbstractSubHyperplane<Euclidean2D, mathlib.geometry.euclidean.oned.Euclidean1D> absSub = (mathlib.geometry.partitioning.AbstractSubHyperplane<Euclidean2D, mathlib.geometry.euclidean.oned.Euclidean1D>) sub;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
				AbstractSubHyperplane<Euclidean2D, Euclidean1D> absSub = (AbstractSubHyperplane<Euclidean2D, Euclidean1D>) sub;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Line line = (Line) sub.getHyperplane();
				Line line = (Line) sub.Hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<mathlib.geometry.euclidean.oned.Interval> intervals = ((mathlib.geometry.euclidean.oned.IntervalsSet) absSub.getRemainingRegion()).asList();
				IList<Interval> intervals = ((IntervalsSet) absSub.RemainingRegion).asList();
				foreach (Interval i in intervals)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D start = Double.isInfinite(i.getInf()) ? null : (Vector2D) line.toSpace((mathlib.geometry.Point<mathlib.geometry.euclidean.oned.Euclidean1D>) new mathlib.geometry.euclidean.oned.Vector1D(i.getInf()));
					Vector2D start = double.IsInfinity(i.Inf) ? null : (Vector2D) line.toSpace((Point<Euclidean1D>) new Vector1D(i.Inf));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D end = Double.isInfinite(i.getSup()) ? null : (Vector2D) line.toSpace((mathlib.geometry.Point<mathlib.geometry.euclidean.oned.Euclidean1D>) new mathlib.geometry.euclidean.oned.Vector1D(i.getSup()));
					Vector2D end = double.IsInfinity(i.Sup) ? null : (Vector2D) line.toSpace((Point<Euclidean1D>) new Vector1D(i.Sup));
					if (reversed)
					{
						sorted.insert(new ComparableSegment(end, start, line.Reverse));
					}
					else
					{
						sorted.insert(new ComparableSegment(start, end, line));
					}
				}
			}

			/// <summary>
			/// Get the sorted segments. </summary>
			/// <returns> sorted segments </returns>
			public virtual AVLTree<ComparableSegment> Sorted
			{
				get
				{
					return sorted;
				}
			}

		}

	}

}
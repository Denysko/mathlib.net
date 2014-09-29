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
namespace mathlib.geometry.spherical.twod
{


	using MathIllegalStateException = mathlib.exception.MathIllegalStateException;
	using mathlib.geometry.enclosing;
	using mathlib.geometry.enclosing;
	using Euclidean3D = mathlib.geometry.euclidean.threed.Euclidean3D;
	using Rotation = mathlib.geometry.euclidean.threed.Rotation;
	using SphereGenerator = mathlib.geometry.euclidean.threed.SphereGenerator;
	using Vector3D = mathlib.geometry.euclidean.threed.Vector3D;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using Sphere1D = mathlib.geometry.spherical.oned.Sphere1D;
	using FastMath = mathlib.util.FastMath;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// This class represents a region on the 2-sphere: a set of spherical polygons.
	/// @version $Id: SphericalPolygonsSet.java 1591835 2014-05-02 09:04:01Z tn $
	/// @since 3.3
	/// </summary>
	public class SphericalPolygonsSet : AbstractRegion<Sphere2D, Sphere1D>
	{

		/// <summary>
		/// Boundary defined as an array of closed loops start vertices. </summary>
		private IList<Vertex> loops;

		/// <summary>
		/// Build a polygons set representing the whole real 2-sphere. </summary>
		/// <param name="tolerance"> below which points are consider to be identical </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SphericalPolygonsSet(final double tolerance)
		public SphericalPolygonsSet(double tolerance) : base(tolerance)
		{
		}

		/// <summary>
		/// Build a polygons set representing a hemisphere. </summary>
		/// <param name="pole"> pole of the hemisphere (the pole is in the inside half) </param>
		/// <param name="tolerance"> below which points are consider to be identical </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SphericalPolygonsSet(final mathlib.geometry.euclidean.threed.Vector3D pole, final double tolerance)
		public SphericalPolygonsSet(Vector3D pole, double tolerance) : base(new BSPTree<Sphere2D>((new Circle(pole, tolerance)).wholeHyperplane(), new BSPTree<Sphere2D>(false), new BSPTree<Sphere2D>(true), null), tolerance)
		{
		}

		/// <summary>
		/// Build a polygons set representing a regular polygon. </summary>
		/// <param name="center"> center of the polygon (the center is in the inside half) </param>
		/// <param name="meridian"> point defining the reference meridian for first polygon vertex </param>
		/// <param name="outsideRadius"> distance of the vertices to the center </param>
		/// <param name="n"> number of sides of the polygon </param>
		/// <param name="tolerance"> below which points are consider to be identical </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SphericalPolygonsSet(final mathlib.geometry.euclidean.threed.Vector3D center, final mathlib.geometry.euclidean.threed.Vector3D meridian, final double outsideRadius, final int n, final double tolerance)
		public SphericalPolygonsSet(Vector3D center, Vector3D meridian, double outsideRadius, int n, double tolerance) : this(tolerance, createRegularPolygonVertices(center, meridian, outsideRadius, n))
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
		/// <param name="tolerance"> below which points are consider to be identical </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SphericalPolygonsSet(final mathlib.geometry.partitioning.BSPTree<Sphere2D> tree, final double tolerance)
		public SphericalPolygonsSet(BSPTree<Sphere2D> tree, double tolerance) : base(tree, tolerance)
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
		/// <param name="tolerance"> below which points are consider to be identical </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SphericalPolygonsSet(final java.util.Collection<mathlib.geometry.partitioning.SubHyperplane<Sphere2D>> boundary, final double tolerance)
		public SphericalPolygonsSet(ICollection<SubHyperplane<Sphere2D>> boundary, double tolerance) : base(boundary, tolerance)
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
		/// be numerically more robust than the {@link #SphericalPolygonsSet(Collection,
		/// double) general constructor} using <seealso cref="SubHyperplane subhyperplanes"/>.</p>
		/// <p>If the list is empty, the region will represent the whole
		/// space.</p>
		/// <p>
		/// Polygons with thin pikes or dents are inherently difficult to handle because
		/// they involve circles with almost opposite directions at some vertices. Polygons
		/// whose vertices come from some physical measurement with noise are also
		/// difficult because an edge that should be straight may be broken in lots of
		/// different pieces with almost equal directions. In both cases, computing the
		/// circles intersections is not numerically robust due to the almost 0 or almost
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
//ORIGINAL LINE: public SphericalPolygonsSet(final double hyperplaneThickness, final S2Point... vertices)
		public SphericalPolygonsSet(double hyperplaneThickness, params S2Point[] vertices) : base(verticesToTree(hyperplaneThickness, vertices), hyperplaneThickness)
		{
		}

		/// <summary>
		/// Build the vertices representing a regular polygon. </summary>
		/// <param name="center"> center of the polygon (the center is in the inside half) </param>
		/// <param name="meridian"> point defining the reference meridian for first polygon vertex </param>
		/// <param name="outsideRadius"> distance of the vertices to the center </param>
		/// <param name="n"> number of sides of the polygon </param>
		/// <returns> vertices array </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static S2Point[] createRegularPolygonVertices(final mathlib.geometry.euclidean.threed.Vector3D center, final mathlib.geometry.euclidean.threed.Vector3D meridian, final double outsideRadius, final int n)
		private static S2Point[] createRegularPolygonVertices(Vector3D center, Vector3D meridian, double outsideRadius, int n)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final S2Point[] array = new S2Point[n];
			S2Point[] array = new S2Point[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.euclidean.threed.Rotation r0 = new mathlib.geometry.euclidean.threed.Rotation(mathlib.geometry.euclidean.threed.Vector3D.crossProduct(center, meridian), outsideRadius);
			Rotation r0 = new Rotation(Vector3D.crossProduct(center, meridian), outsideRadius);
			array[0] = new S2Point(r0.applyTo(center));

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.euclidean.threed.Rotation r = new mathlib.geometry.euclidean.threed.Rotation(center, mathlib.util.MathUtils.TWO_PI / n);
			Rotation r = new Rotation(center, MathUtils.TWO_PI / n);
			for (int i = 1; i < n; ++i)
			{
				array[i] = new S2Point(r.applyTo(array[i - 1].Vector));
			}

			return array;
		}

		/// <summary>
		/// Build the BSP tree of a polygons set from a simple list of vertices.
		/// <p>The boundary is provided as a list of points considering to
		/// represent the vertices of a simple loop. The interior part of the
		/// region is on the left side of this path and the exterior is on its
		/// right side.</p>
		/// <p>This constructor does not handle polygons with a boundary
		/// forming several disconnected paths (such as polygons with holes).</p>
		/// <p>This constructor handles only polygons with edges strictly shorter
		/// than \( \pi \). If longer edges are needed, they need to be broken up
		/// in smaller sub-edges so this constraint holds.</p>
		/// <p>For cases where this simple constructor applies, it is expected to
		/// be numerically more robust than the {@link #PolygonsSet(Collection) general
		/// constructor} using <seealso cref="SubHyperplane subhyperplanes"/>.</p> </summary>
		/// <param name="hyperplaneThickness"> tolerance below which points are consider to
		/// belong to the hyperplane (which is therefore more a slab) </param>
		/// <param name="vertices"> vertices of the simple loop boundary </param>
		/// <returns> the BSP tree of the input vertices </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static mathlib.geometry.partitioning.BSPTree<Sphere2D> verticesToTree(final double hyperplaneThickness, final S2Point... vertices)
		private static BSPTree<Sphere2D> verticesToTree(double hyperplaneThickness, params S2Point[] vertices)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = vertices.length;
			int n = vertices.Length;
			if (n == 0)
			{
				// the tree represents the whole space
				return new BSPTree<Sphere2D>(true);
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
			Vertex end = vArray[n - 1];
			for (int i = 0; i < n; ++i)
			{

				// get the endpoints of the edge
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vertex start = end;
				Vertex start = end;
				end = vArray[i];

				// get the circle supporting the edge, taking care not to recreate it
				// if it was already created earlier due to another edge being aligned
				// with the current one
				Circle circle = start.sharedCircleWith(end);
				if (circle == null)
				{
					circle = new Circle(start.Location, end.Location, hyperplaneThickness);
				}

				// create the edge and store it
				edges.Add(new Edge(start, end, Vector3D.angle(start.Location.Vector, end.Location.Vector), circle));

				// check if another vertex also happens to be on this circle
				foreach (Vertex vertex in vArray)
				{
					if (vertex != start && vertex != end && FastMath.abs(circle.getOffset(vertex.Location)) <= hyperplaneThickness)
					{
						vertex.bindWith(circle);
					}
				}

			}

			// build the tree top-down
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<Sphere2D> tree = new mathlib.geometry.partitioning.BSPTree<Sphere2D>();
			BSPTree<Sphere2D> tree = new BSPTree<Sphere2D>();
			insertEdges(hyperplaneThickness, tree, edges);

			return tree;

		}

		/// <summary>
		/// Recursively build a tree by inserting cut sub-hyperplanes. </summary>
		/// <param name="hyperplaneThickness"> tolerance below which points are considered to
		/// belong to the hyperplane (which is therefore more a slab) </param>
		/// <param name="node"> current tree node (it is a leaf node at the beginning
		/// of the call) </param>
		/// <param name="edges"> list of edges to insert in the cell defined by this node
		/// (excluding edges not belonging to the cell defined by this node) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void insertEdges(final double hyperplaneThickness, final mathlib.geometry.partitioning.BSPTree<Sphere2D> node, final java.util.List<Edge> edges)
		private static void insertEdges(double hyperplaneThickness, BSPTree<Sphere2D> node, IList<Edge> edges)
		{

			// find an edge with an hyperplane that can be inserted in the node
			int index = 0;
			Edge inserted = null;
			while (inserted == null && index < edges.Count)
			{
				inserted = edges[index++];
				if (!node.insertCut(inserted.Circle))
				{
					inserted = null;
				}
			}

			if (inserted == null)
			{
				// no suitable edge was found, the node remains a leaf node
				// we need to set its inside/outside boolean indicator
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<Sphere2D> parent = node.getParent();
				BSPTree<Sphere2D> parent = node.Parent;
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
//ORIGINAL LINE: final java.util.List<Edge> outsideList = new java.util.ArrayList<Edge>();
			IList<Edge> outsideList = new List<Edge>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Edge> insideList = new java.util.ArrayList<Edge>();
			IList<Edge> insideList = new List<Edge>();
			foreach (Edge edge in edges)
			{
				if (edge != inserted)
				{
					edge.Split(inserted.Circle, outsideList, insideList);
				}
			}

			// recurse through lower levels
			if (outsideList.Count > 0)
			{
				insertEdges(hyperplaneThickness, node.Plus, outsideList);
			}
			else
			{
				node.Plus.Attribute = false;
			}
			if (insideList.Count > 0)
			{
				insertEdges(hyperplaneThickness, node.Minus, insideList);
			}
			else
			{
				node.Minus.Attribute = true;
			}

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public SphericalPolygonsSet buildNew(final mathlib.geometry.partitioning.BSPTree<Sphere2D> tree)
		public override SphericalPolygonsSet buildNew(BSPTree<Sphere2D> tree)
		{
			return new SphericalPolygonsSet(tree, Tolerance);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="MathIllegalStateException"> if the tolerance setting does not allow to build
		/// a clean non-ambiguous boundary </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void computeGeometricalProperties() throws mathlib.exception.MathIllegalStateException
		protected internal override void computeGeometricalProperties()
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<Sphere2D> tree = getTree(true);
			BSPTree<Sphere2D> tree = getTree(true);

			if (tree.Cut == null)
			{

				// the instance has a single cell without any boundaries

				if (tree.Cut == null && (bool?) tree.Attribute)
				{
					// the instance covers the whole space
					Size = 4 * FastMath.PI;
					Barycenter = new S2Point(0, 0);
				}
				else
				{
					Size = 0;
					Barycenter = S2Point.NaN_Renamed;
				}

			}
			else
			{

				// the instance has a boundary
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final PropertiesComputer pc = new PropertiesComputer(getTolerance());
				PropertiesComputer pc = new PropertiesComputer(Tolerance);
				tree.visit(pc);
				Size = pc.Area;
				Barycenter = pc.Barycenter;

			}

		}

		/// <summary>
		/// Get the boundary loops of the polygon.
		/// <p>The polygon boundary can be represented as a list of closed loops,
		/// each loop being given by exactly one of its vertices. From each loop
		/// start vertex, one can follow the loop by finding the outgoing edge,
		/// then the end vertex, then the next outgoing edge ... until the start
		/// vertex of the loop (exactly the same instance) is found again once
		/// the full loop has been visited.</p>
		/// <p>If the polygon has no boundary at all, a zero length loop
		/// array will be returned.</p>
		/// <p>If the polygon is a simple one-piece polygon, then the returned
		/// array will contain a single vertex.
		/// </p>
		/// <p>All edges in the various loops have the inside of the region on
		/// their left side (i.e. toward their pole) and the outside on their
		/// right side (i.e. away from their pole) when moving in the underlying
		/// circle direction. This means that the closed loops obey the direct
		/// trigonometric orientation.</p> </summary>
		/// <returns> boundary of the polygon, organized as an unmodifiable list of loops start vertices. </returns>
		/// <exception cref="MathIllegalStateException"> if the tolerance setting does not allow to build
		/// a clean non-ambiguous boundary </exception>
		/// <seealso cref= Vertex </seealso>
		/// <seealso cref= Edge </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.List<Vertex> getBoundaryLoops() throws mathlib.exception.MathIllegalStateException
		public virtual IList<Vertex> BoundaryLoops
		{
			get
			{
    
				if (loops == null)
				{
					if (getTree(false).Cut == null)
					{
						loops = Collections.emptyList();
					}
					else
					{
    
						// sort the arcs according to their start point
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<Sphere2D> root = getTree(true);
						BSPTree<Sphere2D> root = getTree(true);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final EdgesBuilder visitor = new EdgesBuilder(root, getTolerance());
						EdgesBuilder visitor = new EdgesBuilder(root, Tolerance);
						root.visit(visitor);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.List<Edge> edges = visitor.getEdges();
						IList<Edge> edges = visitor.Edges;
    
    
						// convert the list of all edges into a list of start vertices
						loops = new List<Vertex>();
						while (edges.Count > 0)
						{
    
							// this is an edge belonging to a new loop, store it
							Edge edge = edges[0];
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Vertex startVertex = edge.getStart();
							Vertex startVertex = edge.Start;
							loops.Add(startVertex);
    
							// remove all remaining edges in the same loop
							do
							{
    
								// remove one edge
								for (final IEnumerator<Edge> iterator = edges.GetEnumerator(); iterator.hasNext();)
								{
									if (iterator.next() == edge)
									{
										iterator.remove();
										break;
									}
								}
    
								// go to next edge following the boundary loop
								edge = edge.End.Outgoing;
    
							} while (edge.Start != startVertex);
    
						}
    
					}
				}
    
				return Collections.unmodifiableList(loops);
    
			}
		}

		/// <summary>
		/// Get a spherical cap enclosing the polygon.
		/// <p>
		/// This method is intended as a first test to quickly identify points
		/// that are guaranteed to be outside of the region, hence performing a full
		/// <seealso cref="#checkPoint(mathlib.geometry.Vector) checkPoint"/>
		/// only if the point status remains undecided after the quick check. It is
		/// is therefore mostly useful to speed up computation for small polygons with
		/// complex shapes (say a country boundary on Earth), as the spherical cap will
		/// be small and hence will reliably identify a large part of the sphere as outside,
		/// whereas the full check can be more computing intensive. A typical use case is
		/// therefore:
		/// </p>
		/// <pre>
		///   // compute region, plus an enclosing spherical cap
		///   SphericalPolygonsSet complexShape = ...;
		///   EnclosingBall<Sphere2D, S2Point> cap = complexShape.getEnclosingCap();
		/// 
		///   // check lots of points
		///   for (Vector3D p : points) {
		/// 
		///     final Location l;
		///     if (cap.contains(p)) {
		///       // we cannot be sure where the point is
		///       // we need to perform the full computation
		///       l = complexShape.checkPoint(v);
		///     } else {
		///       // no need to do further computation,
		///       // we already know the point is outside
		///       l = Location.OUTSIDE;
		///     }
		/// 
		///     // use l ...
		/// 
		///   }
		/// </pre>
		/// <p>
		/// In the special cases of empty or whole sphere polygons, special
		/// spherical caps are returned, with angular radius set to negative
		/// or positive infinity so the {@link
		/// EnclosingBall#contains(mathlib.geometry.Point) ball.contains(point)}
		/// method return always false or true.
		/// </p>
		/// <p>
		/// This method is <em>not</em> guaranteed to return the smallest enclosing cap.
		/// </p> </summary>
		/// <returns> a spherical cap enclosing the polygon </returns>
		public virtual EnclosingBall<Sphere2D, S2Point> EnclosingCap
		{
			get
			{
    
				// handle special cases first
				if (Empty)
				{
					return new EnclosingBall<Sphere2D, S2Point>(S2Point.PLUS_K, double.NegativeInfinity);
				}
				if (Full)
				{
					return new EnclosingBall<Sphere2D, S2Point>(S2Point.PLUS_K, double.PositiveInfinity);
				}
    
				// as the polygons is neither empty nor full, it has some boundaries and cut hyperplanes
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<Sphere2D> root = getTree(false);
				BSPTree<Sphere2D> root = getTree(false);
				if (isEmpty(root.Minus) && isFull(root.Plus))
				{
					// the polygon covers an hemisphere, and its boundary is one 2π long edge
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Circle circle = (Circle) root.getCut().getHyperplane();
					Circle circle = (Circle) root.Cut.Hyperplane;
					return new EnclosingBall<Sphere2D, S2Point>((new S2Point(circle.Pole)).negate(), 0.5 * FastMath.PI);
				}
				if (isFull(root.Minus) && isEmpty(root.Plus))
				{
					// the polygon covers an hemisphere, and its boundary is one 2π long edge
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Circle circle = (Circle) root.getCut().getHyperplane();
					Circle circle = (Circle) root.Cut.Hyperplane;
					return new EnclosingBall<Sphere2D, S2Point>(new S2Point(circle.Pole), 0.5 * FastMath.PI);
				}
    
				// gather some inside points, to be used by the encloser
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.List<mathlib.geometry.euclidean.threed.Vector3D> points = getInsidePoints();
				IList<Vector3D> points = InsidePoints;
    
				// extract points from the boundary loops, to be used by the encloser as well
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.List<Vertex> boundary = getBoundaryLoops();
				IList<Vertex> boundary = BoundaryLoops;
				foreach (Vertex loopStart in boundary)
				{
					int count = 0;
					for (Vertex v = loopStart; count == 0 || v != loopStart; v = v.Outgoing.End)
					{
						++count;
						points.Add(v.Location.Vector);
					}
				}
    
				// find the smallest enclosing 3D sphere
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final mathlib.geometry.euclidean.threed.SphereGenerator generator = new mathlib.geometry.euclidean.threed.SphereGenerator();
				SphereGenerator generator = new SphereGenerator();
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final mathlib.geometry.enclosing.WelzlEncloser<mathlib.geometry.euclidean.threed.Euclidean3D, mathlib.geometry.euclidean.threed.Vector3D> encloser = new mathlib.geometry.enclosing.WelzlEncloser<mathlib.geometry.euclidean.threed.Euclidean3D, mathlib.geometry.euclidean.threed.Vector3D>(getTolerance(), generator);
				WelzlEncloser<Euclidean3D, Vector3D> encloser = new WelzlEncloser<Euclidean3D, Vector3D>(Tolerance, generator);
				EnclosingBall<Euclidean3D, Vector3D> enclosing3D = encloser.enclose(points);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final mathlib.geometry.euclidean.threed.Vector3D[] support3D = enclosing3D.getSupport();
				Vector3D[] support3D = enclosing3D.Support;
    
				// convert to 3D sphere to spherical cap
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double r = enclosing3D.getRadius();
				double r = enclosing3D.Radius;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double h = enclosing3D.getCenter().getNorm();
				double h = enclosing3D.Center.Norm;
				if (h < Tolerance)
				{
					// the 3D sphere is centered on the unit sphere and covers it
					// fall back to a crude approximation, based only on outside convex cells
					EnclosingBall<Sphere2D, S2Point> enclosingS2 = new EnclosingBall<Sphere2D, S2Point>(S2Point.PLUS_K, double.PositiveInfinity);
					foreach (Vector3D outsidePoint in OutsidePoints)
					{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final S2Point outsideS2 = new S2Point(outsidePoint);
						S2Point outsideS2 = new S2Point(outsidePoint);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final mathlib.geometry.partitioning.BoundaryProjection<Sphere2D> projection = projectToBoundary(outsideS2);
						BoundaryProjection<Sphere2D> projection = projectToBoundary(outsideS2);
						if (FastMath.PI - projection.Offset < enclosingS2.Radius)
						{
							enclosingS2 = new EnclosingBall<Sphere2D, S2Point>(outsideS2.negate(), FastMath.PI - projection.Offset, (S2Point) projection.Projected);
						}
					}
					return enclosingS2;
				}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final S2Point[] support = new S2Point[support3D.length];
				S2Point[] support = new S2Point[support3D.Length];
				for (int i = 0; i < support3D.Length; ++i)
				{
					support[i] = new S2Point(support3D[i]);
				}
    
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final mathlib.geometry.enclosing.EnclosingBall<Sphere2D, S2Point> enclosingS2 = new mathlib.geometry.enclosing.EnclosingBall<Sphere2D, S2Point>(new S2Point(enclosing3D.getCenter()), mathlib.util.FastMath.acos((1 + h * h - r * r) / (2 * h)), support);
				EnclosingBall<Sphere2D, S2Point> enclosingS2 = new EnclosingBall<Sphere2D, S2Point>(new S2Point(enclosing3D.Center), FastMath.acos((1 + h * h - r * r) / (2 * h)), support);
    
				return enclosingS2;
    
			}
		}

		/// <summary>
		/// Gather some inside points. </summary>
		/// <returns> list of points known to be strictly in all inside convex cells </returns>
		private IList<Vector3D> InsidePoints
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final PropertiesComputer pc = new PropertiesComputer(getTolerance());
				PropertiesComputer pc = new PropertiesComputer(Tolerance);
				getTree(true).visit(pc);
				return pc.ConvexCellsInsidePoints;
			}
		}

		/// <summary>
		/// Gather some outside points. </summary>
		/// <returns> list of points known to be strictly in all outside convex cells </returns>
		private IList<Vector3D> OutsidePoints
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final SphericalPolygonsSet complement = (SphericalPolygonsSet) new mathlib.geometry.partitioning.RegionFactory<Sphere2D>().getComplement(this);
				SphericalPolygonsSet complement = (SphericalPolygonsSet) (new RegionFactory<Sphere2D>()).getComplement(this);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final PropertiesComputer pc = new PropertiesComputer(getTolerance());
				PropertiesComputer pc = new PropertiesComputer(Tolerance);
				complement.getTree(true).visit(pc);
				return pc.ConvexCellsInsidePoints;
			}
		}

	}

}
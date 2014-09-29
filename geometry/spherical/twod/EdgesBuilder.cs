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
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using Vector3D = mathlib.geometry.euclidean.threed.Vector3D;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using Arc = mathlib.geometry.spherical.oned.Arc;
	using ArcsSet = mathlib.geometry.spherical.oned.ArcsSet;
	using S1Point = mathlib.geometry.spherical.oned.S1Point;

	/// <summary>
	/// Visitor building edges.
	/// @version $Id: EdgesBuilder.java 1561506 2014-01-26 15:31:18Z luc $
	/// @since 3.3
	/// </summary>
	internal class EdgesBuilder : BSPTreeVisitor<Sphere2D>
	{

		/// <summary>
		/// Root of the tree. </summary>
		private readonly BSPTree<Sphere2D> root;

		/// <summary>
		/// Tolerance below which points are consider to be identical. </summary>
		private readonly double tolerance;

		/// <summary>
		/// Built edges and their associated nodes. </summary>
		private readonly IDictionary<Edge, BSPTree<Sphere2D>> edgeToNode;

		/// <summary>
		/// Reversed map. </summary>
		private readonly IDictionary<BSPTree<Sphere2D>, IList<Edge>> nodeToEdgesList;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="root"> tree root </param>
		/// <param name="tolerance"> below which points are consider to be identical </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public EdgesBuilder(final mathlib.geometry.partitioning.BSPTree<Sphere2D> root, final double tolerance)
		public EdgesBuilder(BSPTree<Sphere2D> root, double tolerance)
		{
			this.root = root;
			this.tolerance = tolerance;
			this.edgeToNode = new IdentityHashMap<Edge, BSPTree<Sphere2D>>();
			this.nodeToEdgesList = new IdentityHashMap<BSPTree<Sphere2D>, IList<Edge>>();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.geometry.partitioning.BSPTreeVisitor_Order visitOrder(final mathlib.geometry.partitioning.BSPTree<Sphere2D> node)
		public virtual mathlib.geometry.partitioning.BSPTreeVisitor_Order visitOrder(BSPTree<Sphere2D> node)
		{
			return mathlib.geometry.partitioning.BSPTreeVisitor_Order.MINUS_SUB_PLUS;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visitInternalNode(final mathlib.geometry.partitioning.BSPTree<Sphere2D> node)
		public virtual void visitInternalNode(BSPTree<Sphere2D> node)
		{
			nodeToEdgesList[node] = new List<Edge>();
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final mathlib.geometry.partitioning.BoundaryAttribute<Sphere2D> attribute = (mathlib.geometry.partitioning.BoundaryAttribute<Sphere2D>) node.getAttribute();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			BoundaryAttribute<Sphere2D> attribute = (BoundaryAttribute<Sphere2D>) node.Attribute;
			if (attribute.PlusOutside != null)
			{
				addContribution((SubCircle) attribute.PlusOutside, false, node);
			}
			if (attribute.PlusInside != null)
			{
				addContribution((SubCircle) attribute.PlusInside, true, node);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visitLeafNode(final mathlib.geometry.partitioning.BSPTree<Sphere2D> node)
		public virtual void visitLeafNode(BSPTree<Sphere2D> node)
		{
		}

		/// <summary>
		/// Add the contribution of a boundary edge. </summary>
		/// <param name="sub"> boundary facet </param>
		/// <param name="reversed"> if true, the facet has the inside on its plus side </param>
		/// <param name="node"> node to which the edge belongs </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void addContribution(final SubCircle sub, final boolean reversed, final mathlib.geometry.partitioning.BSPTree<Sphere2D> node)
		private void addContribution(SubCircle sub, bool reversed, BSPTree<Sphere2D> node)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Circle circle = (Circle) sub.getHyperplane();
			Circle circle = (Circle) sub.Hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<mathlib.geometry.spherical.oned.Arc> arcs = ((mathlib.geometry.spherical.oned.ArcsSet) sub.getRemainingRegion()).asList();
			IList<Arc> arcs = ((ArcsSet) sub.RemainingRegion).asList();
			foreach (Arc a in arcs)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vertex start = new Vertex((S2Point) circle.toSpace(new mathlib.geometry.spherical.oned.S1Point(a.getInf())));
				Vertex start = new Vertex((S2Point) circle.toSpace(new S1Point(a.Inf)));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vertex end = new Vertex((S2Point) circle.toSpace(new mathlib.geometry.spherical.oned.S1Point(a.getSup())));
				Vertex end = new Vertex((S2Point) circle.toSpace(new S1Point(a.Sup)));
				start.bindWith(circle);
				end.bindWith(circle);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Edge edge;
				Edge edge;
				if (reversed)
				{
					edge = new Edge(end, start, a.Size, circle.Reverse);
				}
				else
				{
					edge = new Edge(start, end, a.Size, circle);
				}
				edgeToNode[edge] = node;
				nodeToEdgesList[node].Add(edge);
			}
		}

		/// <summary>
		/// Get the edge that should naturally follow another one. </summary>
		/// <param name="previous"> edge to be continued </param>
		/// <returns> other edge, starting where the previous one ends (they
		/// have not been connected yet) </returns>
		/// <exception cref="MathIllegalStateException"> if there is not a single other edge </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Edge getFollowingEdge(final Edge previous) throws mathlib.exception.MathIllegalStateException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private Edge getFollowingEdge(Edge previous)
		{

			// get the candidate nodes
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final S2Point point = previous.getEnd().getLocation();
			S2Point point = previous.End.Location;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<mathlib.geometry.partitioning.BSPTree<Sphere2D>> candidates = root.getCloseCuts(point, tolerance);
			IList<BSPTree<Sphere2D>> candidates = root.getCloseCuts(point, tolerance);

			// the following edge we are looking for must start from one of the candidates nodes
			double closest = tolerance;
			Edge following = null;
			foreach (BSPTree<Sphere2D> node in candidates)
			{
				foreach (Edge edge in nodeToEdgesList[node])
				{
					if (edge != previous && edge.Start.Incoming == null)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.euclidean.threed.Vector3D edgeStart = edge.getStart().getLocation().getVector();
						Vector3D edgeStart = edge.Start.Location.Vector;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double gap = mathlib.geometry.euclidean.threed.Vector3D.angle(point.getVector(), edgeStart);
						double gap = Vector3D.angle(point.Vector, edgeStart);
						if (gap <= closest)
						{
							closest = gap;
							following = edge;
						}
					}
				}
			}

			if (following == null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.euclidean.threed.Vector3D previousStart = previous.getStart().getLocation().getVector();
				Vector3D previousStart = previous.Start.Location.Vector;
				if (Vector3D.angle(point.Vector, previousStart) <= tolerance)
				{
					// the edge connects back to itself
					return previous;
				}

				// this should never happen
				throw new MathIllegalStateException(LocalizedFormats.OUTLINE_BOUNDARY_LOOP_OPEN);

			}

			return following;

		}

		/// <summary>
		/// Get the boundary edges. </summary>
		/// <returns> boundary edges </returns>
		/// <exception cref="MathIllegalStateException"> if there is not a single other edge </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.List<Edge> getEdges() throws mathlib.exception.MathIllegalStateException
		public virtual IList<Edge> Edges
		{
			get
			{
    
				// connect the edges
				foreach (Edge previous in edgeToNode.Keys)
				{
					previous.NextEdge = getFollowingEdge(previous);
				}
    
				return new List<Edge>(edgeToNode.Keys);
    
			}
		}

	}

}
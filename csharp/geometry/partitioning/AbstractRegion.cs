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
namespace org.apache.commons.math3.geometry.partitioning
{


	using MathInternalError = org.apache.commons.math3.exception.MathInternalError;
	using org.apache.commons.math3.geometry;
	using org.apache.commons.math3.geometry;

	/// <summary>
	/// Abstract class for all regions, independently of geometry type or dimension.
	/// </summary>
	/// @param <S> Type of the space. </param>
	/// @param <T> Type of the sub-space.
	/// 
	/// @version $Id: AbstractRegion.java 1566358 2014-02-09 19:17:55Z luc $
	/// @since 3.0 </param>
	public abstract class AbstractRegion<S, T> : Region<S> where S : org.apache.commons.math3.geometry.Space where T : org.apache.commons.math3.geometry.Space
	{

		/// <summary>
		/// Inside/Outside BSP tree. </summary>
		private BSPTree<S> tree;

		/// <summary>
		/// Tolerance below which points are considered to belong to hyperplanes. </summary>
		private readonly double tolerance;

		/// <summary>
		/// Size of the instance. </summary>
		private double size;

		/// <summary>
		/// Barycenter. </summary>
		private Point<S> barycenter;

		/// <summary>
		/// Build a region representing the whole space. </summary>
		/// <param name="tolerance"> tolerance below which points are considered identical. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractRegion(final double tolerance)
		protected internal AbstractRegion(double tolerance)
		{
			this.tree = new BSPTree<S>(true);
			this.tolerance = tolerance;
		}

		/// <summary>
		/// Build a region from an inside/outside BSP tree.
		/// <p>The leaf nodes of the BSP tree <em>must</em> have a
		/// {@code Boolean} attribute representing the inside status of
		/// the corresponding cell (true for inside cells, false for outside
		/// cells). In order to avoid building too many small objects, it is
		/// recommended to use the predefined constants
		/// {@code Boolean.TRUE} and {@code Boolean.FALSE}. The
		/// tree also <em>must</em> have either null internal nodes or
		/// internal nodes representing the boundary as specified in the
		/// <seealso cref="#getTree getTree"/> method).</p> </summary>
		/// <param name="tree"> inside/outside BSP tree representing the region </param>
		/// <param name="tolerance"> tolerance below which points are considered identical. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractRegion(final BSPTree<S> tree, final double tolerance)
		protected internal AbstractRegion(BSPTree<S> tree, double tolerance)
		{
			this.tree = tree;
			this.tolerance = tolerance;
		}

		/// <summary>
		/// Build a Region from a Boundary REPresentation (B-rep).
		/// <p>The boundary is provided as a collection of {@link
		/// SubHyperplane sub-hyperplanes}. Each sub-hyperplane has the
		/// interior part of the region on its minus side and the exterior on
		/// its plus side.</p>
		/// <p>The boundary elements can be in any order, and can form
		/// several non-connected sets (like for example polygons with holes
		/// or a set of disjoints polyhedrons considered as a whole). In
		/// fact, the elements do not even need to be connected together
		/// (their topological connections are not used here). However, if the
		/// boundary does not really separate an inside open from an outside
		/// open (open having here its topological meaning), then subsequent
		/// calls to the <seealso cref="#checkPoint(Point) checkPoint"/> method will not be
		/// meaningful anymore.</p>
		/// <p>If the boundary is empty, the region will represent the whole
		/// space.</p> </summary>
		/// <param name="boundary"> collection of boundary elements, as a
		/// collection of <seealso cref="SubHyperplane SubHyperplane"/> objects </param>
		/// <param name="tolerance"> tolerance below which points are considered identical. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractRegion(final java.util.Collection<SubHyperplane<S>> boundary, final double tolerance)
		protected internal AbstractRegion(ICollection<SubHyperplane<S>> boundary, double tolerance)
		{

			this.tolerance = tolerance;

			if (boundary.Count == 0)
			{

				// the tree represents the whole space
				tree = new BSPTree<S>(true);

			}
			else
			{

				// sort the boundary elements in decreasing size order
				// (we don't want equal size elements to be removed, so
				// we use a trick to fool the TreeSet)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.TreeSet<SubHyperplane<S>> ordered = new java.util.TreeSet<SubHyperplane<S>>(new java.util.Comparator<SubHyperplane<S>>()
				SortedSet<SubHyperplane<S>> ordered = new SortedSet<SubHyperplane<S>>(new ComparatorAnonymousInnerClassHelper(this));
				ordered.addAll(boundary);

				// build the tree top-down
				tree = new BSPTree<S>();
				insertCuts(tree, ordered);

				// set up the inside/outside flags
				tree.visit(new BSPTreeVisitorAnonymousInnerClassHelper(this));

			}

		}

		private class ComparatorAnonymousInnerClassHelper : IComparer<SubHyperplane<S>>
		{
			private readonly AbstractRegion outerInstance;

			public ComparatorAnonymousInnerClassHelper(AbstractRegion outerInstance)
			{
				this.outerInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public int compare(final SubHyperplane<S> o1, final SubHyperplane<S> o2)
			public virtual int Compare(SubHyperplane<S> o1, SubHyperplane<S> o2)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double size1 = o1.getSize();
				double size1 = o1.Size;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double size2 = o2.getSize();
				double size2 = o2.Size;
				return (size2 < size1) ? - 1 : ((o1 == o2) ? 0 : +1);
			}
		}

		private class BSPTreeVisitorAnonymousInnerClassHelper : BSPTreeVisitor<S>
		{
			private readonly AbstractRegion outerInstance;

			public BSPTreeVisitorAnonymousInnerClassHelper(AbstractRegion outerInstance)
			{
				this.outerInstance = outerInstance;
			}


						/// <summary>
						/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BSPTreeVisitor_Order visitOrder(final BSPTree<S> node)
			public virtual BSPTreeVisitor_Order visitOrder(BSPTree<S> node)
			{
				return BSPTreeVisitor_Order.PLUS_SUB_MINUS;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visitInternalNode(final BSPTree<S> node)
			public virtual void visitInternalNode(BSPTree<S> node)
			{
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visitLeafNode(final BSPTree<S> node)
			public virtual void visitLeafNode(BSPTree<S> node)
			{
				if (node.Parent == null || node == node.Parent.Minus)
				{
					node.Attribute = true;
				}
				else
				{
					node.Attribute = false;
				}
			}
		}

		/// <summary>
		/// Build a convex region from an array of bounding hyperplanes. </summary>
		/// <param name="hyperplanes"> array of bounding hyperplanes (if null, an
		/// empty region will be built) </param>
		/// <param name="tolerance"> tolerance below which points are considered identical. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public AbstractRegion(final Hyperplane<S>[] hyperplanes, final double tolerance)
		public AbstractRegion(Hyperplane<S>[] hyperplanes, double tolerance)
		{
			this.tolerance = tolerance;
			if ((hyperplanes == null) || (hyperplanes.Length == 0))
			{
				tree = new BSPTree<S>(false);
			}
			else
			{

				// use the first hyperplane to build the right class
				tree = hyperplanes[0].wholeSpace().getTree(false);

				// chop off parts of the space
				BSPTree<S> node = tree;
				node.Attribute = true;
				foreach (Hyperplane<S> hyperplane in hyperplanes)
				{
					if (node.insertCut(hyperplane))
					{
						node.Attribute = null;
						node.Plus.Attribute = false;
						node = node.Minus;
						node.Attribute = true;
					}
				}

			}

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public abstract AbstractRegion<S, T> buildNew(BSPTree<S> newTree);

		/// <summary>
		/// Get the tolerance below which points are considered to belong to hyperplanes. </summary>
		/// <returns> tolerance below which points are considered to belong to hyperplanes </returns>
		public virtual double Tolerance
		{
			get
			{
				return tolerance;
			}
		}

		/// <summary>
		/// Recursively build a tree by inserting cut sub-hyperplanes. </summary>
		/// <param name="node"> current tree node (it is a leaf node at the beginning
		/// of the call) </param>
		/// <param name="boundary"> collection of edges belonging to the cell defined
		/// by the node </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void insertCuts(final BSPTree<S> node, final java.util.Collection<SubHyperplane<S>> boundary)
		private void insertCuts(BSPTree<S> node, ICollection<SubHyperplane<S>> boundary)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<SubHyperplane<S>> iterator = boundary.iterator();
			IEnumerator<SubHyperplane<S>> iterator = boundary.GetEnumerator();

			// build the current level
			Hyperplane<S> inserted = null;
			while ((inserted == null) && iterator.MoveNext())
			{
				inserted = iterator.Current.Hyperplane;
				if (!node.insertCut(inserted.copySelf()))
				{
					inserted = null;
				}
			}

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (!iterator.hasNext())
			{
				return;
			}

			// distribute the remaining edges in the two sub-trees
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<SubHyperplane<S>> plusList = new java.util.ArrayList<SubHyperplane<S>>();
			List<SubHyperplane<S>> plusList = new List<SubHyperplane<S>>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<SubHyperplane<S>> minusList = new java.util.ArrayList<SubHyperplane<S>>();
			List<SubHyperplane<S>> minusList = new List<SubHyperplane<S>>();
			while (iterator.MoveNext())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SubHyperplane<S> other = iterator.Current;
				SubHyperplane<S> other = iterator.Current;
				switch (other.side(inserted))
				{
				case PLUS:
					plusList.Add(other);
					break;
				case MINUS:
					minusList.Add(other);
					break;
				case BOTH:
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SubHyperplane_SplitSubHyperplane<S> split = other.split(inserted);
					SubHyperplane_SplitSubHyperplane<S> split = other.Split(inserted);
					plusList.Add(split.Plus);
					minusList.Add(split.Minus);
					break;
				default:
					// ignore the sub-hyperplanes belonging to the cut hyperplane
			break;
				}
			}

			// recurse through lower levels
			insertCuts(node.Plus, plusList);
			insertCuts(node.Minus, minusList);

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual AbstractRegion<S, T> copySelf()
		{
			return buildNew(tree.copySelf());
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual bool Empty
		{
			get
			{
				return isEmpty(tree);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean isEmpty(final BSPTree<S> node)
		public virtual bool isEmpty(BSPTree<S> node)
		{

			// we use a recursive function rather than the BSPTreeVisitor
			// interface because we can stop visiting the tree as soon as we
			// have found an inside cell

			if (node.Cut == null)
			{
				// if we find an inside node, the region is not empty
				return !((bool?) node.Attribute);
			}

			// check both sides of the sub-tree
			return isEmpty(node.Minus) && isEmpty(node.Plus);

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual bool Full
		{
			get
			{
				return isFull(tree);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean isFull(final BSPTree<S> node)
		public virtual bool isFull(BSPTree<S> node)
		{

			// we use a recursive function rather than the BSPTreeVisitor
			// interface because we can stop visiting the tree as soon as we
			// have found an outside cell

			if (node.Cut == null)
			{
				// if we find an outside node, the region does not cover full space
				return (bool?) node.Attribute;
			}

			// check both sides of the sub-tree
			return isFull(node.Minus) && isFull(node.Plus);

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean contains(final Region<S> region)
		public virtual bool contains(Region<S> region)
		{
			return (new RegionFactory<S>()).difference(region, this).Empty;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.3
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BoundaryProjection<S> projectToBoundary(final org.apache.commons.math3.geometry.Point<S> point)
		public virtual BoundaryProjection<S> projectToBoundary(Point<S> point)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BoundaryProjector<S, T> projector = new BoundaryProjector<S, T>(point);
			BoundaryProjector<S, T> projector = new BoundaryProjector<S, T>(point);
			getTree(true).visit(projector);
			return projector.Projection;
		}

		/// <summary>
		/// Check a point with respect to the region. </summary>
		/// <param name="point"> point to check </param>
		/// <returns> a code representing the point status: either {@link
		/// Location#INSIDE}, <seealso cref="Location#OUTSIDE"/> or <seealso cref="Location#BOUNDARY"/> </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.partitioning.Region_Location checkPoint(final org.apache.commons.math3.geometry.Vector<S> point)
		public virtual Region_Location checkPoint(Vector<S> point)
		{
			return checkPoint((Point<S>) point);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.partitioning.Region_Location checkPoint(final org.apache.commons.math3.geometry.Point<S> point)
		public virtual Region_Location checkPoint(Point<S> point)
		{
			return checkPoint(tree, point);
		}

		/// <summary>
		/// Check a point with respect to the region starting at a given node. </summary>
		/// <param name="node"> root node of the region </param>
		/// <param name="point"> point to check </param>
		/// <returns> a code representing the point status: either {@link
		/// Region.Location#INSIDE INSIDE}, {@link Region.Location#OUTSIDE
		/// OUTSIDE} or <seealso cref="Region.Location#BOUNDARY BOUNDARY"/> </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected org.apache.commons.math3.geometry.partitioning.Region_Location checkPoint(final BSPTree<S> node, final org.apache.commons.math3.geometry.Vector<S> point)
		protected internal virtual Region_Location checkPoint(BSPTree<S> node, Vector<S> point)
		{
			return checkPoint(node, (Point<S>) point);
		}

		/// <summary>
		/// Check a point with respect to the region starting at a given node. </summary>
		/// <param name="node"> root node of the region </param>
		/// <param name="point"> point to check </param>
		/// <returns> a code representing the point status: either {@link
		/// Region.Location#INSIDE INSIDE}, {@link Region.Location#OUTSIDE
		/// OUTSIDE} or <seealso cref="Region.Location#BOUNDARY BOUNDARY"/> </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected org.apache.commons.math3.geometry.partitioning.Region_Location checkPoint(final BSPTree<S> node, final org.apache.commons.math3.geometry.Point<S> point)
		protected internal virtual Region_Location checkPoint(BSPTree<S> node, Point<S> point)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BSPTree<S> cell = node.getCell(point, tolerance);
			BSPTree<S> cell = node.getCell(point, tolerance);
			if (cell.Cut == null)
			{
				// the point is in the interior of a cell, just check the attribute
				return ((bool?) cell.Attribute) ? Region_Location.INSIDE : Region_Location.OUTSIDE;
			}

			// the point is on a cut-sub-hyperplane, is it on a boundary ?
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.partitioning.Region_Location minusCode = checkPoint(cell.getMinus(), point);
			Region_Location minusCode = checkPoint(cell.Minus, point);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.partitioning.Region_Location plusCode = checkPoint(cell.getPlus(), point);
			Region_Location plusCode = checkPoint(cell.Plus, point);
			return (minusCode == plusCode) ? minusCode : Region_Location.BOUNDARY;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BSPTree<S> getTree(final boolean includeBoundaryAttributes)
		public virtual BSPTree<S> getTree(bool includeBoundaryAttributes)
		{
			if (includeBoundaryAttributes && (tree.Cut != null) && (tree.Attribute == null))
			{
				// we need to compute the boundary attributes
				tree.visit(new BoundaryBuilder<S>());
			}
			return tree;
		}

		/// <summary>
		/// Visitor building boundary shell tree.
		/// <p>
		/// The boundary shell is represented as <seealso cref="BoundaryAttribute boundary attributes"/>
		/// at each internal node.
		/// </p>
		/// </summary>
		private class BoundaryBuilder<S> : BSPTreeVisitor<S> where S : org.apache.commons.math3.geometry.Space
		{

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual BSPTreeVisitor_Order visitOrder(BSPTree<S> node)
			{
				return BSPTreeVisitor_Order.PLUS_MINUS_SUB;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual void visitInternalNode(BSPTree<S> node)
			{

				SubHyperplane<S> plusOutside = null;
				SubHyperplane<S> plusInside = null;

				// characterize the cut sub-hyperplane,
				// first with respect to the plus sub-tree
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final SubHyperplane<S>[] plusChar = (SubHyperplane<S>[]) Array.newInstance(SubHyperplane.class, 2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
				SubHyperplane<S>[] plusChar = (SubHyperplane<S>[]) Array.newInstance(typeof(SubHyperplane), 2);
				characterize(node.Plus, node.Cut.copySelf(), plusChar);

				if (plusChar[0] != null && !plusChar[0].Empty)
				{
					// plusChar[0] corresponds to a subset of the cut sub-hyperplane known to have
					// outside cells on its plus side, we want to check if parts of this subset
					// do have inside cells on their minus side
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final SubHyperplane<S>[] minusChar = (SubHyperplane<S>[]) Array.newInstance(SubHyperplane.class, 2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
					SubHyperplane<S>[] minusChar = (SubHyperplane<S>[]) Array.newInstance(typeof(SubHyperplane), 2);
					characterize(node.Minus, plusChar[0], minusChar);
					if (minusChar[1] != null && !minusChar[1].Empty)
					{
						// this part belongs to the boundary,
						// it has the outside on its plus side and the inside on its minus side
						plusOutside = minusChar[1];
					}
				}

				if (plusChar[1] != null && !plusChar[1].Empty)
				{
					// plusChar[1] corresponds to a subset of the cut sub-hyperplane known to have
					// inside cells on its plus side, we want to check if parts of this subset
					// do have outside cells on their minus side
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final SubHyperplane<S>[] minusChar = (SubHyperplane<S>[]) Array.newInstance(SubHyperplane.class, 2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
					SubHyperplane<S>[] minusChar = (SubHyperplane<S>[]) Array.newInstance(typeof(SubHyperplane), 2);
					characterize(node.Minus, plusChar[1], minusChar);
					if (minusChar[0] != null && !minusChar[0].Empty)
					{
						// this part belongs to the boundary,
						// it has the inside on its plus side and the outside on its minus side
						plusInside = minusChar[0];
					}
				}

				// set the boundary attribute at non-leaf nodes
				node.Attribute = new BoundaryAttribute<S>(plusOutside, plusInside);

			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual void visitLeafNode(BSPTree<S> node)
			{
			}

			/// <summary>
			/// Filter the parts of an hyperplane belonging to the boundary.
			/// <p>The filtering consist in splitting the specified
			/// sub-hyperplane into several parts lying in inside and outside
			/// cells of the tree. The principle is to call this method twice for
			/// each cut sub-hyperplane in the tree, once on the plus node and
			/// once on the minus node. The parts that have the same flag
			/// (inside/inside or outside/outside) do not belong to the boundary
			/// while parts that have different flags (inside/outside or
			/// outside/inside) do belong to the boundary.</p> </summary>
			/// <param name="node"> current BSP tree node </param>
			/// <param name="sub"> sub-hyperplane to characterize </param>
			/// <param name="characterization"> placeholder where to put the characterized parts </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void characterize(final BSPTree<S> node, final SubHyperplane<S> sub, final SubHyperplane<S>[] characterization)
			internal virtual void characterize(BSPTree<S> node, SubHyperplane<S> sub, SubHyperplane<S>[] characterization)
			{
				if (node.Cut == null)
				{
					// we have reached a leaf node
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean inside = (Boolean) node.getAttribute();
					bool inside = (bool?) node.Attribute;
					if (inside)
					{
						if (characterization[1] == null)
						{
							characterization[1] = sub;
						}
						else
						{
							characterization[1] = characterization[1].reunite(sub);
						}
					}
					else
					{
						if (characterization[0] == null)
						{
							characterization[0] = sub;
						}
						else
						{
							characterization[0] = characterization[0].reunite(sub);
						}
					}
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Hyperplane<S> hyperplane = node.getCut().getHyperplane();
					Hyperplane<S> hyperplane = node.Cut.Hyperplane;
					switch (sub.side(hyperplane))
					{
					case PLUS:
						characterize(node.Plus, sub, characterization);
						break;
					case MINUS:
						characterize(node.Minus, sub, characterization);
						break;
					case BOTH:
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SubHyperplane_SplitSubHyperplane<S> split = sub.split(hyperplane);
						SubHyperplane_SplitSubHyperplane<S> split = sub.Split(hyperplane);
						characterize(node.Plus, split.Plus, characterization);
						characterize(node.Minus, split.Minus, characterization);
						break;
					default:
						// this should not happen
						throw new MathInternalError();
					}
				}
			}

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double BoundarySize
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final BoundarySizeVisitor<S> visitor = new BoundarySizeVisitor<S>();
				BoundarySizeVisitor<S> visitor = new BoundarySizeVisitor<S>();
				getTree(true).visit(visitor);
				return visitor.Size;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double Size
		{
			get
			{
				if (barycenter == null)
				{
					computeGeometricalProperties();
				}
				return size;
			}
			set
			{
				this.size = value;
			}
		}

		/// <summary>
		/// Set the size of the instance. </summary>
		/// <param name="size"> size of the instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected void setSize(final double size)

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Point<S> Barycenter
		{
			get
			{
				if (barycenter == null)
				{
					computeGeometricalProperties();
				}
				return barycenter;
			}
			set
			{
				Barycenter = (Point<S>) value;
			}
		}

		/// <summary>
		/// Set the barycenter of the instance. </summary>
		/// <param name="barycenter"> barycenter of the instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected void setBarycenter(final org.apache.commons.math3.geometry.Vector<S> barycenter)

		/// <summary>
		/// Set the barycenter of the instance. </summary>
		/// <param name="barycenter"> barycenter of the instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected void setBarycenter(final org.apache.commons.math3.geometry.Point<S> barycenter)
		protected internal virtual Point<S> Barycenter
		{
			set
			{
				this.barycenter = value;
			}
		}

		/// <summary>
		/// Compute some geometrical properties.
		/// <p>The properties to compute are the barycenter and the size.</p>
		/// </summary>
		protected internal abstract void computeGeometricalProperties();

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Side side(final Hyperplane<S> hyperplane)
		public virtual Side side(Hyperplane<S> hyperplane)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Sides sides = new Sides();
			Sides sides = new Sides();
			recurseSides(tree, hyperplane.wholeHyperplane(), sides);
			return sides.plusFound() ? (sides.minusFound() ? Side.BOTH : Side.PLUS) : (sides.minusFound() ? Side.MINUS : Side.HYPER);
		}

		/// <summary>
		/// Search recursively for inside leaf nodes on each side of the given hyperplane.
		/// 
		/// <p>The algorithm used here is directly derived from the one
		/// described in section III (<i>Binary Partitioning of a BSP
		/// Tree</i>) of the Bruce Naylor, John Amanatides and William
		/// Thibault paper <a
		/// href="http://www.cs.yorku.ca/~amana/research/bsptSetOp.pdf">Merging
		/// BSP Trees Yields Polyhedral Set Operations</a> Proc. Siggraph
		/// '90, Computer Graphics 24(4), August 1990, pp 115-124, published
		/// by the Association for Computing Machinery (ACM)..</p>
		/// </summary>
		/// <param name="node"> current BSP tree node </param>
		/// <param name="sub"> sub-hyperplane </param>
		/// <param name="sides"> object holding the sides found </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void recurseSides(final BSPTree<S> node, final SubHyperplane<S> sub, final Sides sides)
		private void recurseSides(BSPTree<S> node, SubHyperplane<S> sub, Sides sides)
		{

			if (node.Cut == null)
			{
				if ((bool?) node.Attribute)
				{
					// this is an inside cell expanding across the hyperplane
					sides.rememberPlusFound();
					sides.rememberMinusFound();
				}
				return;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Hyperplane<S> hyperplane = node.getCut().getHyperplane();
			Hyperplane<S> hyperplane = node.Cut.Hyperplane;
			switch (sub.side(hyperplane))
			{
			case PLUS :
				// the sub-hyperplane is entirely in the plus sub-tree
				if (node.Cut.side(sub.Hyperplane) == Side.PLUS)
				{
					if (!isEmpty(node.Minus))
					{
						sides.rememberPlusFound();
					}
				}
				else
				{
					if (!isEmpty(node.Minus))
					{
						sides.rememberMinusFound();
					}
				}
				if (!(sides.plusFound() && sides.minusFound()))
				{
					recurseSides(node.Plus, sub, sides);
				}
				break;
			case MINUS :
				// the sub-hyperplane is entirely in the minus sub-tree
				if (node.Cut.side(sub.Hyperplane) == Side.PLUS)
				{
					if (!isEmpty(node.Plus))
					{
						sides.rememberPlusFound();
					}
				}
				else
				{
					if (!isEmpty(node.Plus))
					{
						sides.rememberMinusFound();
					}
				}
				if (!(sides.plusFound() && sides.minusFound()))
				{
					recurseSides(node.Minus, sub, sides);
				}
				break;
			case BOTH :
				// the sub-hyperplane extends in both sub-trees
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SubHyperplane_SplitSubHyperplane<S> split = sub.split(hyperplane);
				SubHyperplane_SplitSubHyperplane<S> split = sub.Split(hyperplane);

				// explore first the plus sub-tree
				recurseSides(node.Plus, split.Plus, sides);

				// if needed, explore the minus sub-tree
				if (!(sides.plusFound() && sides.minusFound()))
				{
					recurseSides(node.Minus, split.Minus, sides);
				}
				break;
			default :
				// the sub-hyperplane and the cut sub-hyperplane share the same hyperplane
				if (node.Cut.Hyperplane.sameOrientationAs(sub.Hyperplane))
				{
					if ((node.Plus.Cut != null) || ((bool?) node.Plus.Attribute))
					{
						sides.rememberPlusFound();
					}
					if ((node.Minus.Cut != null) || ((bool?) node.Minus.Attribute))
					{
						sides.rememberMinusFound();
					}
				}
				else
				{
					if ((node.Plus.Cut != null) || ((bool?) node.Plus.Attribute))
					{
						sides.rememberMinusFound();
					}
					if ((node.Minus.Cut != null) || ((bool?) node.Minus.Attribute))
					{
						sides.rememberPlusFound();
					}
				}
			break;
			}

		}

		/// <summary>
		/// Utility class holding the already found sides. </summary>
		private sealed class Sides
		{

			/// <summary>
			/// Indicator of inside leaf nodes found on the plus side. </summary>
			internal bool plusFound_Renamed;

			/// <summary>
			/// Indicator of inside leaf nodes found on the plus side. </summary>
			internal bool minusFound_Renamed;

			/// <summary>
			/// Simple constructor.
			/// </summary>
			public Sides()
			{
				plusFound_Renamed = false;
				minusFound_Renamed = false;
			}

			/// <summary>
			/// Remember the fact that inside leaf nodes have been found on the plus side.
			/// </summary>
			public void rememberPlusFound()
			{
				plusFound_Renamed = true;
			}

			/// <summary>
			/// Check if inside leaf nodes have been found on the plus side. </summary>
			/// <returns> true if inside leaf nodes have been found on the plus side </returns>
			public bool plusFound()
			{
				return plusFound_Renamed;
			}

			/// <summary>
			/// Remember the fact that inside leaf nodes have been found on the minus side.
			/// </summary>
			public void rememberMinusFound()
			{
				minusFound_Renamed = true;
			}

			/// <summary>
			/// Check if inside leaf nodes have been found on the minus side. </summary>
			/// <returns> true if inside leaf nodes have been found on the minus side </returns>
			public bool minusFound()
			{
				return minusFound_Renamed;
			}

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SubHyperplane<S> intersection(final SubHyperplane<S> sub)
		public virtual SubHyperplane<S> intersection(SubHyperplane<S> sub)
		{
			return recurseIntersection(tree, sub);
		}

		/// <summary>
		/// Recursively compute the parts of a sub-hyperplane that are
		/// contained in the region. </summary>
		/// <param name="node"> current BSP tree node </param>
		/// <param name="sub"> sub-hyperplane traversing the region </param>
		/// <returns> filtered sub-hyperplane </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private SubHyperplane<S> recurseIntersection(final BSPTree<S> node, final SubHyperplane<S> sub)
		private SubHyperplane<S> recurseIntersection(BSPTree<S> node, SubHyperplane<S> sub)
		{

			if (node.Cut == null)
			{
				return (bool?) node.Attribute ? sub.copySelf() : null;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Hyperplane<S> hyperplane = node.getCut().getHyperplane();
			Hyperplane<S> hyperplane = node.Cut.Hyperplane;
			switch (sub.side(hyperplane))
			{
			case PLUS :
				return recurseIntersection(node.Plus, sub);
			case MINUS :
				return recurseIntersection(node.Minus, sub);
			case BOTH :
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SubHyperplane_SplitSubHyperplane<S> split = sub.split(hyperplane);
				SubHyperplane_SplitSubHyperplane<S> split = sub.Split(hyperplane);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SubHyperplane<S> plus = recurseIntersection(node.getPlus(), split.getPlus());
				SubHyperplane<S> plus = recurseIntersection(node.Plus, split.Plus);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SubHyperplane<S> minus = recurseIntersection(node.getMinus(), split.getMinus());
				SubHyperplane<S> minus = recurseIntersection(node.Minus, split.Minus);
				if (plus == null)
				{
					return minus;
				}
				else if (minus == null)
				{
					return plus;
				}
				else
				{
					return plus.reunite(minus);
				}
			default :
				return recurseIntersection(node.Plus, recurseIntersection(node.Minus, sub));
			}

		}

		/// <summary>
		/// Transform a region.
		/// <p>Applying a transform to a region consist in applying the
		/// transform to all the hyperplanes of the underlying BSP tree and
		/// of the boundary (and also to the sub-hyperplanes embedded in
		/// these hyperplanes) and to the barycenter. The instance is not
		/// modified, a new instance is built.</p> </summary>
		/// <param name="transform"> transform to apply </param>
		/// <returns> a new region, resulting from the application of the
		/// transform to the instance </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public AbstractRegion<S, T> applyTransform(final Transform<S, T> transform)
		public virtual AbstractRegion<S, T> applyTransform(Transform<S, T> transform)
		{
			return buildNew(recurseTransform(getTree(false), transform));
		}

		/// <summary>
		/// Recursively transform an inside/outside BSP-tree. </summary>
		/// <param name="node"> current BSP tree node </param>
		/// <param name="transform"> transform to apply </param>
		/// <returns> a new tree </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private BSPTree<S> recurseTransform(final BSPTree<S> node, final Transform<S, T> transform)
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private BSPTree<S> recurseTransform(BSPTree<S> node, Transform<S, T> transform)
		{

			if (node.Cut == null)
			{
				return new BSPTree<S>(node.Attribute);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SubHyperplane<S> sub = node.getCut();
			SubHyperplane<S> sub = node.Cut;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SubHyperplane<S> tSub = ((AbstractSubHyperplane<S, T>) sub).applyTransform(transform);
			SubHyperplane<S> tSub = ((AbstractSubHyperplane<S, T>) sub).applyTransform(transform);
			BoundaryAttribute<S> attribute = (BoundaryAttribute<S>) node.Attribute;
			if (attribute != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SubHyperplane<S> tPO = (attribute.getPlusOutside() == null) ? null : ((AbstractSubHyperplane<S, T>) attribute.getPlusOutside()).applyTransform(transform);
				SubHyperplane<S> tPO = (attribute.PlusOutside == null) ? null : ((AbstractSubHyperplane<S, T>) attribute.PlusOutside).applyTransform(transform);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SubHyperplane<S> tPI = (attribute.getPlusInside() == null) ? null : ((AbstractSubHyperplane<S, T>) attribute.getPlusInside()).applyTransform(transform);
				SubHyperplane<S> tPI = (attribute.PlusInside == null) ? null : ((AbstractSubHyperplane<S, T>) attribute.PlusInside).applyTransform(transform);
				attribute = new BoundaryAttribute<S>(tPO, tPI);
			}

			return new BSPTree<S>(tSub, recurseTransform(node.Plus, transform), recurseTransform(node.Minus, transform), attribute);

		}

	}

}
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
namespace mathlib.geometry.euclidean.oned
{


	using mathlib.geometry;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using Precision = mathlib.util.Precision;

	/// <summary>
	/// This class represents a 1D region: a set of intervals.
	/// @version $Id: IntervalsSet.java 1563714 2014-02-02 20:55:14Z tn $
	/// @since 3.0
	/// </summary>
	public class IntervalsSet : AbstractRegion<Euclidean1D, Euclidean1D>, IEnumerable<double[]>
	{

		/// <summary>
		/// Default value for tolerance. </summary>
		private const double DEFAULT_TOLERANCE = 1.0e-10;

		/// <summary>
		/// Build an intervals set representing the whole real line. </summary>
		/// <param name="tolerance"> tolerance below which points are considered identical.
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public IntervalsSet(final double tolerance)
		public IntervalsSet(double tolerance) : base(tolerance)
		{
		}

		/// <summary>
		/// Build an intervals set corresponding to a single interval. </summary>
		/// <param name="lower"> lower bound of the interval, must be lesser or equal
		/// to {@code upper} (may be {@code Double.NEGATIVE_INFINITY}) </param>
		/// <param name="upper"> upper bound of the interval, must be greater or equal
		/// to {@code lower} (may be {@code Double.POSITIVE_INFINITY}) </param>
		/// <param name="tolerance"> tolerance below which points are considered identical.
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public IntervalsSet(final double lower, final double upper, final double tolerance)
		public IntervalsSet(double lower, double upper, double tolerance) : base(buildTree(lower, upper, tolerance), tolerance)
		{
		}

		/// <summary>
		/// Build an intervals set from an inside/outside BSP tree.
		/// <p>The leaf nodes of the BSP tree <em>must</em> have a
		/// {@code Boolean} attribute representing the inside status of
		/// the corresponding cell (true for inside cells, false for outside
		/// cells). In order to avoid building too many small objects, it is
		/// recommended to use the predefined constants
		/// {@code Boolean.TRUE} and {@code Boolean.FALSE}</p> </summary>
		/// <param name="tree"> inside/outside BSP tree representing the intervals set </param>
		/// <param name="tolerance"> tolerance below which points are considered identical.
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public IntervalsSet(final mathlib.geometry.partitioning.BSPTree<Euclidean1D> tree, final double tolerance)
		public IntervalsSet(BSPTree<Euclidean1D> tree, double tolerance) : base(tree, tolerance)
		{
		}

		/// <summary>
		/// Build an intervals set from a Boundary REPresentation (B-rep).
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
		/// calls to the {@link
		/// mathlib.geometry.partitioning.Region#checkPoint(mathlib.geometry.Point)
		/// checkPoint} method will not be meaningful anymore.</p>
		/// <p>If the boundary is empty, the region will represent the whole
		/// space.</p> </summary>
		/// <param name="boundary"> collection of boundary elements </param>
		/// <param name="tolerance"> tolerance below which points are considered identical.
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public IntervalsSet(final java.util.Collection<mathlib.geometry.partitioning.SubHyperplane<Euclidean1D>> boundary, final double tolerance)
		public IntervalsSet(ICollection<SubHyperplane<Euclidean1D>> boundary, double tolerance) : base(boundary, tolerance)
		{
		}

		/// <summary>
		/// Build an intervals set representing the whole real line. </summary>
		/// @deprecated as of 3.1 replaced with <seealso cref="#IntervalsSet(double)"/> 
		[Obsolete]//("as of 3.1 replaced with <seealso cref="#IntervalsSet(double)"/>")]
		public IntervalsSet() : this(DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Build an intervals set corresponding to a single interval. </summary>
		/// <param name="lower"> lower bound of the interval, must be lesser or equal
		/// to {@code upper} (may be {@code Double.NEGATIVE_INFINITY}) </param>
		/// <param name="upper"> upper bound of the interval, must be greater or equal
		/// to {@code lower} (may be {@code Double.POSITIVE_INFINITY}) </param>
		/// @deprecated as of 3.3 replaced with <seealso cref="#IntervalsSet(double, double, double)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3 replaced with <seealso cref="#IntervalsSet(double, double, double)"/>") public IntervalsSet(final double lower, final double upper)
		[Obsolete]//("as of 3.3 replaced with <seealso cref="#IntervalsSet(double, double, double)"/>")]
		public IntervalsSet(double lower, double upper) : this(lower, upper, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Build an intervals set from an inside/outside BSP tree.
		/// <p>The leaf nodes of the BSP tree <em>must</em> have a
		/// {@code Boolean} attribute representing the inside status of
		/// the corresponding cell (true for inside cells, false for outside
		/// cells). In order to avoid building too many small objects, it is
		/// recommended to use the predefined constants
		/// {@code Boolean.TRUE} and {@code Boolean.FALSE}</p> </summary>
		/// <param name="tree"> inside/outside BSP tree representing the intervals set </param>
		/// @deprecated as of 3.3, replaced with <seealso cref="#IntervalsSet(BSPTree, double)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3, replaced with <seealso cref="#IntervalsSet(mathlib.geometry.partitioning.BSPTree, double)"/>") public IntervalsSet(final mathlib.geometry.partitioning.BSPTree<Euclidean1D> tree)
		[Obsolete]//("as of 3.3, replaced with <seealso cref="#IntervalsSet(mathlib.geometry.partitioning.BSPTree, double)"/>")]
		public IntervalsSet(BSPTree<Euclidean1D> tree) : this(tree, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Build an intervals set from a Boundary REPresentation (B-rep).
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
		/// calls to the {@link
		/// mathlib.geometry.partitioning.Region#checkPoint(mathlib.geometry.Point)
		/// checkPoint} method will not be meaningful anymore.</p>
		/// <p>If the boundary is empty, the region will represent the whole
		/// space.</p> </summary>
		/// <param name="boundary"> collection of boundary elements </param>
		/// @deprecated as of 3.3, replaced with <seealso cref="#IntervalsSet(Collection, double)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3, replaced with <seealso cref="#IntervalsSet(java.util.Collection, double)"/>") public IntervalsSet(final java.util.Collection<mathlib.geometry.partitioning.SubHyperplane<Euclidean1D>> boundary)
		[Obsolete]//("as of 3.3, replaced with <seealso cref="#IntervalsSet(java.util.Collection, double)"/>")]
		public IntervalsSet(ICollection<SubHyperplane<Euclidean1D>> boundary) : this(boundary, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Build an inside/outside tree representing a single interval. </summary>
		/// <param name="lower"> lower bound of the interval, must be lesser or equal
		/// to {@code upper} (may be {@code Double.NEGATIVE_INFINITY}) </param>
		/// <param name="upper"> upper bound of the interval, must be greater or equal
		/// to {@code lower} (may be {@code Double.POSITIVE_INFINITY}) </param>
		/// <param name="tolerance"> tolerance below which points are considered identical. </param>
		/// <returns> the built tree </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static mathlib.geometry.partitioning.BSPTree<Euclidean1D> buildTree(final double lower, final double upper, final double tolerance)
		private static BSPTree<Euclidean1D> buildTree(double lower, double upper, double tolerance)
		{
			if (double.IsInfinity(lower) && (lower < 0))
			{
				if (double.IsInfinity(upper) && (upper > 0))
				{
					// the tree must cover the whole real line
					return new BSPTree<Euclidean1D>(true);
				}
				// the tree must be open on the negative infinity side
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.SubHyperplane<Euclidean1D> upperCut = new OrientedPoint(new Vector1D(upper), true, tolerance).wholeHyperplane();
				SubHyperplane<Euclidean1D> upperCut = (new OrientedPoint(new Vector1D(upper), true, tolerance)).wholeHyperplane();
				return new BSPTree<Euclidean1D>(upperCut, new BSPTree<Euclidean1D>(false), new BSPTree<Euclidean1D>(true), null);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.SubHyperplane<Euclidean1D> lowerCut = new OrientedPoint(new Vector1D(lower), false, tolerance).wholeHyperplane();
			SubHyperplane<Euclidean1D> lowerCut = (new OrientedPoint(new Vector1D(lower), false, tolerance)).wholeHyperplane();
			if (double.IsInfinity(upper) && (upper > 0))
			{
				// the tree must be open on the positive infinity side
				return new BSPTree<Euclidean1D>(lowerCut, new BSPTree<Euclidean1D>(false), new BSPTree<Euclidean1D>(true), null);
			}

			// the tree must be bounded on the two sides
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.SubHyperplane<Euclidean1D> upperCut = new OrientedPoint(new Vector1D(upper), true, tolerance).wholeHyperplane();
			SubHyperplane<Euclidean1D> upperCut = (new OrientedPoint(new Vector1D(upper), true, tolerance)).wholeHyperplane();
			return new BSPTree<Euclidean1D>(lowerCut, new BSPTree<Euclidean1D>(false), new BSPTree<Euclidean1D>(upperCut, new BSPTree<Euclidean1D>(false), new BSPTree<Euclidean1D>(true), null), null);

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public IntervalsSet buildNew(final mathlib.geometry.partitioning.BSPTree<Euclidean1D> tree)
		public override IntervalsSet buildNew(BSPTree<Euclidean1D> tree)
		{
			return new IntervalsSet(tree, Tolerance);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override void computeGeometricalProperties()
		{
			if (getTree(false).Cut == null)
			{
				Barycenter = (Point<Euclidean1D>) Vector1D.NaN_Renamed;
				Size = ((bool?) getTree(false).Attribute) ? double.PositiveInfinity : 0;
			}
			else
			{
				double size = 0.0;
				double sum = 0.0;
				foreach (Interval interval in asList())
				{
					size += interval.Size;
					sum += interval.Size * interval.Barycenter;
				}
				Size = size;
				if (double.IsInfinity(size))
				{
					Barycenter = (Point<Euclidean1D>) Vector1D.NaN_Renamed;
				}
				else if (size >= Precision.SAFE_MIN)
				{
					Barycenter = (Point<Euclidean1D>) new Vector1D(sum / size);
				}
				else
				{
					Barycenter = (Point<Euclidean1D>)((OrientedPoint) getTree(false).Cut.Hyperplane).Location;
				}
			}
		}

		/// <summary>
		/// Get the lowest value belonging to the instance. </summary>
		/// <returns> lowest value belonging to the instance
		/// ({@code Double.NEGATIVE_INFINITY} if the instance doesn't
		/// have any low bound, {@code Double.POSITIVE_INFINITY} if the
		/// instance is empty) </returns>
		public virtual double Inf
		{
			get
			{
				BSPTree<Euclidean1D> node = getTree(false);
				double inf = double.PositiveInfinity;
				while (node.Cut != null)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final OrientedPoint op = (OrientedPoint) node.getCut().getHyperplane();
					OrientedPoint op = (OrientedPoint) node.Cut.Hyperplane;
					inf = op.Location.X;
					node = op.Direct ? node.Minus : node.Plus;
				}
				return ((bool?) node.Attribute) ? double.NegativeInfinity : inf;
			}
		}

		/// <summary>
		/// Get the highest value belonging to the instance. </summary>
		/// <returns> highest value belonging to the instance
		/// ({@code Double.POSITIVE_INFINITY} if the instance doesn't
		/// have any high bound, {@code Double.NEGATIVE_INFINITY} if the
		/// instance is empty) </returns>
		public virtual double Sup
		{
			get
			{
				BSPTree<Euclidean1D> node = getTree(false);
				double sup = double.NegativeInfinity;
				while (node.Cut != null)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final OrientedPoint op = (OrientedPoint) node.getCut().getHyperplane();
					OrientedPoint op = (OrientedPoint) node.Cut.Hyperplane;
					sup = op.Location.X;
					node = op.Direct ? node.Plus : node.Minus;
				}
				return ((bool?) node.Attribute) ? double.PositiveInfinity : sup;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.3
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public mathlib.geometry.partitioning.BoundaryProjection<Euclidean1D> projectToBoundary(final mathlib.geometry.Point<Euclidean1D> point)
		public override BoundaryProjection<Euclidean1D> projectToBoundary(Point<Euclidean1D> point)
		{

			// get position of test point
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = ((Vector1D) point).getX();
			double x = ((Vector1D) point).X;

			double previous = double.NegativeInfinity;
			foreach (double[] a in this)
			{
				if (x < a[0])
				{
					// the test point lies between the previous and the current intervals
					// offset will be positive
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double previousOffset = x - previous;
					double previousOffset = x - previous;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double currentOffset = a[0] - x;
					double currentOffset = a[0] - x;
					if (previousOffset < currentOffset)
					{
						return new BoundaryProjection<Euclidean1D>(point, finiteOrNullPoint(previous), previousOffset);
					}
					else
					{
						return new BoundaryProjection<Euclidean1D>(point, finiteOrNullPoint(a[0]), currentOffset);
					}
				}
				else if (x <= a[1])
				{
					// the test point lies within the current interval
					// offset will be negative
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double offset0 = a[0] - x;
					double offset0 = a[0] - x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double offset1 = x - a[1];
					double offset1 = x - a[1];
					if (offset0 < offset1)
					{
						return new BoundaryProjection<Euclidean1D>(point, finiteOrNullPoint(a[1]), offset1);
					}
					else
					{
						return new BoundaryProjection<Euclidean1D>(point, finiteOrNullPoint(a[0]), offset0);
					}
				}
				previous = a[1];
			}

			// the test point if past the last sub-interval
			return new BoundaryProjection<Euclidean1D>(point, finiteOrNullPoint(previous), x - previous);

		}

		/// <summary>
		/// Build a finite point. </summary>
		/// <param name="x"> abscissa of the point </param>
		/// <returns> a new point for finite abscissa, null otherwise </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Vector1D finiteOrNullPoint(final double x)
		private Vector1D finiteOrNullPoint(double x)
		{
			return double.IsInfinity(x) ? null : new Vector1D(x);
		}

		/// <summary>
		/// Build an ordered list of intervals representing the instance.
		/// <p>This method builds this intervals set as an ordered list of
		/// <seealso cref="Interval Interval"/> elements. If the intervals set has no
		/// lower limit, the first interval will have its low bound equal to
		/// {@code Double.NEGATIVE_INFINITY}. If the intervals set has
		/// no upper limit, the last interval will have its upper bound equal
		/// to {@code Double.POSITIVE_INFINITY}. An empty tree will
		/// build an empty list while a tree representing the whole real line
		/// will build a one element list with both bounds being
		/// infinite.</p> </summary>
		/// <returns> a new ordered list containing <seealso cref="Interval Interval"/>
		/// elements </returns>
		public virtual IList<Interval> asList()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Interval> list = new java.util.ArrayList<Interval>();
			IList<Interval> list = new List<Interval>();
			foreach (double[] a in this)
			{
				list.Add(new Interval(a[0], a[1]));
			}
			return list;
		}

		/// <summary>
		/// Get the first leaf node of a tree. </summary>
		/// <param name="root"> tree root </param>
		/// <returns> first leaf node </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private mathlib.geometry.partitioning.BSPTree<Euclidean1D> getFirstLeaf(final mathlib.geometry.partitioning.BSPTree<Euclidean1D> root)
		private BSPTree<Euclidean1D> getFirstLeaf(BSPTree<Euclidean1D> root)
		{

			if (root.Cut == null)
			{
				return root;
			}

			// find the smallest internal node
			BSPTree<Euclidean1D> smallest = null;
			for (BSPTree<Euclidean1D> n = root; n != null; n = previousInternalNode(n))
			{
				smallest = n;
			}

			return leafBefore(smallest);

		}

		/// <summary>
		/// Get the node corresponding to the first interval boundary. </summary>
		/// <returns> smallest internal node,
		/// or null if there are no internal nodes (i.e. the set is either empty or covers the real line) </returns>
		private BSPTree<Euclidean1D> FirstIntervalBoundary
		{
			get
			{
    
				// start search at the tree root
				BSPTree<Euclidean1D> node = getTree(false);
				if (node.Cut == null)
				{
					return null;
				}
    
				// walk tree until we find the smallest internal node
				node = getFirstLeaf(node).Parent;
    
				// walk tree until we find an interval boundary
				while (node != null && !(isIntervalStart(node) || isIntervalEnd(node)))
				{
					node = nextInternalNode(node);
				}
    
				return node;
    
			}
		}

		/// <summary>
		/// Check if an internal node corresponds to the start abscissa of an interval. </summary>
		/// <param name="node"> internal node to check </param>
		/// <returns> true if the node corresponds to the start abscissa of an interval </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private boolean isIntervalStart(final mathlib.geometry.partitioning.BSPTree<Euclidean1D> node)
		private bool isIntervalStart(BSPTree<Euclidean1D> node)
		{

			if ((bool?) leafBefore(node).Attribute)
			{
				// it has an inside cell before it, it may end an interval but not start it
				return false;
			}

			if (!(bool?) leafAfter(node).Attribute)
			{
				// it has an outside cell after it, it is a dummy cut away from real intervals
				return false;
			}

			// the cell has an outside before and an inside after it
			// it is the start of an interval
			return true;

		}

		/// <summary>
		/// Check if an internal node corresponds to the end abscissa of an interval. </summary>
		/// <param name="node"> internal node to check </param>
		/// <returns> true if the node corresponds to the end abscissa of an interval </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private boolean isIntervalEnd(final mathlib.geometry.partitioning.BSPTree<Euclidean1D> node)
		private bool isIntervalEnd(BSPTree<Euclidean1D> node)
		{

			if (!(bool?) leafBefore(node).Attribute)
			{
				// it has an outside cell before it, it may start an interval but not end it
				return false;
			}

			if ((bool?) leafAfter(node).Attribute)
			{
				// it has an inside cell after it, it is a dummy cut in the middle of an interval
				return false;
			}

			// the cell has an inside before and an outside after it
			// it is the end of an interval
			return true;

		}

		/// <summary>
		/// Get the next internal node. </summary>
		/// <param name="node"> current internal node </param>
		/// <returns> next internal node in ascending order, or null
		/// if this is the last internal node </returns>
		private BSPTree<Euclidean1D> nextInternalNode(BSPTree<Euclidean1D> node)
		{

			if (childAfter(node).Cut != null)
			{
				// the next node is in the sub-tree
				return leafAfter(node).Parent;
			}

			// there is nothing left deeper in the tree, we backtrack
			while (isAfterParent(node))
			{
				node = node.Parent;
			}
			return node.Parent;

		}

		/// <summary>
		/// Get the previous internal node. </summary>
		/// <param name="node"> current internal node </param>
		/// <returns> previous internal node in ascending order, or null
		/// if this is the first internal node </returns>
		private BSPTree<Euclidean1D> previousInternalNode(BSPTree<Euclidean1D> node)
		{

			if (childBefore(node).Cut != null)
			{
				// the next node is in the sub-tree
				return leafBefore(node).Parent;
			}

			// there is nothing left deeper in the tree, we backtrack
			while (isBeforeParent(node))
			{
				node = node.Parent;
			}
			return node.Parent;

		}

		/// <summary>
		/// Find the leaf node just before an internal node. </summary>
		/// <param name="node"> internal node at which the sub-tree starts </param>
		/// <returns> leaf node just before the internal node </returns>
		private BSPTree<Euclidean1D> leafBefore(BSPTree<Euclidean1D> node)
		{

			node = childBefore(node);
			while (node.Cut != null)
			{
				node = childAfter(node);
			}

			return node;

		}

		/// <summary>
		/// Find the leaf node just after an internal node. </summary>
		/// <param name="node"> internal node at which the sub-tree starts </param>
		/// <returns> leaf node just after the internal node </returns>
		private BSPTree<Euclidean1D> leafAfter(BSPTree<Euclidean1D> node)
		{

			node = childAfter(node);
			while (node.Cut != null)
			{
				node = childBefore(node);
			}

			return node;

		}

		/// <summary>
		/// Check if a node is the child before its parent in ascending order. </summary>
		/// <param name="node"> child node considered </param>
		/// <returns> true is the node has a parent end is before it in ascending order </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private boolean isBeforeParent(final mathlib.geometry.partitioning.BSPTree<Euclidean1D> node)
		private bool isBeforeParent(BSPTree<Euclidean1D> node)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<Euclidean1D> parent = node.getParent();
			BSPTree<Euclidean1D> parent = node.Parent;
			if (parent == null)
			{
				return false;
			}
			else
			{
				return node == childBefore(parent);
			}
		}

		/// <summary>
		/// Check if a node is the child after its parent in ascending order. </summary>
		/// <param name="node"> child node considered </param>
		/// <returns> true is the node has a parent end is after it in ascending order </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private boolean isAfterParent(final mathlib.geometry.partitioning.BSPTree<Euclidean1D> node)
		private bool isAfterParent(BSPTree<Euclidean1D> node)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<Euclidean1D> parent = node.getParent();
			BSPTree<Euclidean1D> parent = node.Parent;
			if (parent == null)
			{
				return false;
			}
			else
			{
				return node == childAfter(parent);
			}
		}

		/// <summary>
		/// Find the child node just before an internal node. </summary>
		/// <param name="node"> internal node at which the sub-tree starts </param>
		/// <returns> child node just before the internal node </returns>
		private BSPTree<Euclidean1D> childBefore(BSPTree<Euclidean1D> node)
		{
			if (isDirect(node))
			{
				// smaller abscissas are on minus side, larger abscissas are on plus side
				return node.Minus;
			}
			else
			{
				// smaller abscissas are on plus side, larger abscissas are on minus side
				return node.Plus;
			}
		}

		/// <summary>
		/// Find the child node just after an internal node. </summary>
		/// <param name="node"> internal node at which the sub-tree starts </param>
		/// <returns> child node just after the internal node </returns>
		private BSPTree<Euclidean1D> childAfter(BSPTree<Euclidean1D> node)
		{
			if (isDirect(node))
			{
				// smaller abscissas are on minus side, larger abscissas are on plus side
				return node.Plus;
			}
			else
			{
				// smaller abscissas are on plus side, larger abscissas are on minus side
				return node.Minus;
			}
		}

		/// <summary>
		/// Check if an internal node has a direct oriented point. </summary>
		/// <param name="node"> internal node to check </param>
		/// <returns> true if the oriented point is direct </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private boolean isDirect(final mathlib.geometry.partitioning.BSPTree<Euclidean1D> node)
		private bool isDirect(BSPTree<Euclidean1D> node)
		{
			return ((OrientedPoint) node.Cut.Hyperplane).Direct;
		}

		/// <summary>
		/// Get the abscissa of an internal node. </summary>
		/// <param name="node"> internal node to check </param>
		/// <returns> abscissa </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double getAngle(final mathlib.geometry.partitioning.BSPTree<Euclidean1D> node)
		private double getAngle(BSPTree<Euclidean1D> node)
		{
			return ((OrientedPoint) node.Cut.Hyperplane).Location.X;
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// The iterator returns the limit values of sub-intervals in ascending order.
		/// </p>
		/// <p>
		/// The iterator does <em>not</em> support the optional {@code remove} operation.
		/// </p>
		/// @since 3.3
		/// </summary>
		public virtual IEnumerator<double[]> GetEnumerator()
		{
			return new SubIntervalsIterator(this);
		}

		/// <summary>
		/// Local iterator for sub-intervals. </summary>
		private class SubIntervalsIterator : IEnumerator<double[]>
		{
			private readonly IntervalsSet outerInstance;


			/// <summary>
			/// Current node. </summary>
			internal BSPTree<Euclidean1D> current;

			/// <summary>
			/// Sub-interval no yet returned. </summary>
			internal double[] pending;

			/// <summary>
			/// Simple constructor.
			/// </summary>
			public SubIntervalsIterator(IntervalsSet outerInstance)
			{
				this.outerInstance = outerInstance;

				current = outerInstance.FirstIntervalBoundary;

				if (current == null)
				{
					// all the leaf tree nodes share the same inside/outside status
					if ((bool?) outerInstance.getFirstLeaf(outerInstance.getTree(false)).Attribute)
					{
						// it is an inside node, it represents the full real line
						pending = new double[] {double.NegativeInfinity, double.PositiveInfinity};
					}
					else
					{
						pending = null;
					}
				}
				else if (outerInstance.isIntervalEnd(current))
				{
					// the first boundary is an interval end,
					// so the first interval starts at infinity
					pending = new double[] {double.NegativeInfinity, outerInstance.getAngle(current)};
				}
				else
				{
					selectPending();
				}
			}

			/// <summary>
			/// Walk the tree to select the pending sub-interval.
			/// </summary>
			internal virtual void selectPending()
			{

				// look for the start of the interval
				BSPTree<Euclidean1D> start = current;
				while (start != null && !outerInstance.isIntervalStart(start))
				{
					start = outerInstance.nextInternalNode(start);
				}

				if (start == null)
				{
					// we have exhausted the iterator
					current = null;
					pending = null;
					return;
				}

				// look for the end of the interval
				BSPTree<Euclidean1D> end = start;
				while (end != null && !outerInstance.isIntervalEnd(end))
				{
					end = outerInstance.nextInternalNode(end);
				}

				if (end != null)
				{

					// we have identified the interval
					pending = new double[] {outerInstance.getAngle(start), outerInstance.getAngle(end)};

					// prepare search for next interval
					current = end;

				}
				else
				{

					// the final interval is open toward infinity
					pending = new double[] {outerInstance.getAngle(start), double.PositiveInfinity};

					// there won't be any other intervals
					current = null;

				}

			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual bool hasNext()
			{
				return pending != null;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual double[] next()
			{
				if (pending == null)
				{
					throw new NoSuchElementException();
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] next = pending;
				double[] next = pending;
				selectPending();
				return next;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual void remove()
			{
				throw new System.NotSupportedException();
			}

		}

	}

}
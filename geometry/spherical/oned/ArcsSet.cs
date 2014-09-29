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
namespace mathlib.geometry.spherical.oned
{


	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using MathInternalError = mathlib.exception.MathInternalError;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using mathlib.geometry;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using Side = mathlib.geometry.partitioning.Side;
	using mathlib.geometry.partitioning;
	using FastMath = mathlib.util.FastMath;
	using MathUtils = mathlib.util.MathUtils;
	using Precision = mathlib.util.Precision;

	/// <summary>
	/// This class represents a region of a circle: a set of arcs.
	/// <p>
	/// Note that due to the wrapping around \(2 \pi\), barycenter is
	/// ill-defined here. It was defined only in order to fulfill
	/// the requirements of the {@link
	/// mathlib.geometry.partitioning.Region Region}
	/// interface, but its use is discouraged.
	/// </p>
	/// @version $Id: ArcsSet.java 1563714 2014-02-02 20:55:14Z tn $
	/// @since 3.3
	/// </summary>
	public class ArcsSet : AbstractRegion<Sphere1D, Sphere1D>, IEnumerable<double[]>
	{

		/// <summary>
		/// Build an arcs set representing the whole circle. </summary>
		/// <param name="tolerance"> tolerance below which close sub-arcs are merged together </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ArcsSet(final double tolerance)
		public ArcsSet(double tolerance) : base(tolerance)
		{
		}

		/// <summary>
		/// Build an arcs set corresponding to a single arc.
		/// <p>
		/// If either {@code lower} is equals to {@code upper} or
		/// the interval exceeds \( 2 \pi \), the arc is considered
		/// to be the full circle and its initial defining boundaries
		/// will be forgotten. {@code lower} is not allowed to be greater
		/// than {@code upper} (an exception is thrown in this case).
		/// </p> </summary>
		/// <param name="lower"> lower bound of the arc </param>
		/// <param name="upper"> upper bound of the arc </param>
		/// <param name="tolerance"> tolerance below which close sub-arcs are merged together </param>
		/// <exception cref="NumberIsTooLargeException"> if lower is greater than upper </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArcsSet(final double lower, final double upper, final double tolerance) throws mathlib.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public ArcsSet(double lower, double upper, double tolerance) : base(buildTree(lower, upper, tolerance), tolerance)
		{
		}

		/// <summary>
		/// Build an arcs set from an inside/outside BSP tree.
		/// <p>The leaf nodes of the BSP tree <em>must</em> have a
		/// {@code Boolean} attribute representing the inside status of
		/// the corresponding cell (true for inside cells, false for outside
		/// cells). In order to avoid building too many small objects, it is
		/// recommended to use the predefined constants
		/// {@code Boolean.TRUE} and {@code Boolean.FALSE}</p> </summary>
		/// <param name="tree"> inside/outside BSP tree representing the arcs set </param>
		/// <param name="tolerance"> tolerance below which close sub-arcs are merged together </param>
		/// <exception cref="InconsistentStateAt2PiWrapping"> if the tree leaf nodes are not
		/// consistent across the \( 0, 2 \pi \) crossing </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArcsSet(final mathlib.geometry.partitioning.BSPTree<Sphere1D> tree, final double tolerance) throws InconsistentStateAt2PiWrapping
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public ArcsSet(BSPTree<Sphere1D> tree, double tolerance) : base(tree, tolerance)
		{
			check2PiConsistency();
		}

		/// <summary>
		/// Build an arcs set from a Boundary REPresentation (B-rep).
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
		/// <param name="tolerance"> tolerance below which close sub-arcs are merged together </param>
		/// <exception cref="InconsistentStateAt2PiWrapping"> if the tree leaf nodes are not
		/// consistent across the \( 0, 2 \pi \) crossing </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArcsSet(final java.util.Collection<mathlib.geometry.partitioning.SubHyperplane<Sphere1D>> boundary, final double tolerance) throws InconsistentStateAt2PiWrapping
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public ArcsSet(ICollection<SubHyperplane<Sphere1D>> boundary, double tolerance) : base(boundary, tolerance)
		{
			check2PiConsistency();
		}

		/// <summary>
		/// Build an inside/outside tree representing a single arc. </summary>
		/// <param name="lower"> lower angular bound of the arc </param>
		/// <param name="upper"> upper angular bound of the arc </param>
		/// <param name="tolerance"> tolerance below which close sub-arcs are merged together </param>
		/// <returns> the built tree </returns>
		/// <exception cref="NumberIsTooLargeException"> if lower is greater than upper </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static mathlib.geometry.partitioning.BSPTree<Sphere1D> buildTree(final double lower, final double upper, final double tolerance) throws mathlib.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private static BSPTree<Sphere1D> buildTree(double lower, double upper, double tolerance)
		{

			if (Precision.Equals(lower, upper, 0) || (upper - lower) >= MathUtils.TWO_PI)
			{
				// the tree must cover the whole circle
				return new BSPTree<Sphere1D>(true);
			}
			else if (lower > upper)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.ENDPOINTS_NOT_AN_INTERVAL, lower, upper, true);
			}

			// this is a regular arc, covering only part of the circle
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double normalizedLower = mathlib.util.MathUtils.normalizeAngle(lower, mathlib.util.FastMath.PI);
			double normalizedLower = MathUtils.normalizeAngle(lower, FastMath.PI);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double normalizedUpper = normalizedLower + (upper - lower);
			double normalizedUpper = normalizedLower + (upper - lower);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.SubHyperplane<Sphere1D> lowerCut = new LimitAngle(new S1Point(normalizedLower), false, tolerance).wholeHyperplane();
			SubHyperplane<Sphere1D> lowerCut = (new LimitAngle(new S1Point(normalizedLower), false, tolerance)).wholeHyperplane();

			if (normalizedUpper <= MathUtils.TWO_PI)
			{
				// simple arc starting after 0 and ending before 2 \pi
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.SubHyperplane<Sphere1D> upperCut = new LimitAngle(new S1Point(normalizedUpper), true, tolerance).wholeHyperplane();
				SubHyperplane<Sphere1D> upperCut = (new LimitAngle(new S1Point(normalizedUpper), true, tolerance)).wholeHyperplane();
				return new BSPTree<Sphere1D>(lowerCut, new BSPTree<Sphere1D>(false), new BSPTree<Sphere1D>(upperCut, new BSPTree<Sphere1D>(false), new BSPTree<Sphere1D>(true), null), null);
			}
			else
			{
				// arc wrapping around 2 \pi
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.SubHyperplane<Sphere1D> upperCut = new LimitAngle(new S1Point(normalizedUpper - mathlib.util.MathUtils.TWO_PI), true, tolerance).wholeHyperplane();
				SubHyperplane<Sphere1D> upperCut = (new LimitAngle(new S1Point(normalizedUpper - MathUtils.TWO_PI), true, tolerance)).wholeHyperplane();
				return new BSPTree<Sphere1D>(lowerCut, new BSPTree<Sphere1D>(upperCut, new BSPTree<Sphere1D>(false), new BSPTree<Sphere1D>(true), null), new BSPTree<Sphere1D>(true), null);
			}

		}

		/// <summary>
		/// Check consistency. </summary>
		/// <exception cref="InconsistentStateAt2PiWrapping"> if the tree leaf nodes are not
		/// consistent across the \( 0, 2 \pi \) crossing </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void check2PiConsistency() throws InconsistentStateAt2PiWrapping
		private void check2PiConsistency()
		{

			// start search at the tree root
			BSPTree<Sphere1D> root = getTree(false);
			if (root.Cut == null)
			{
				return;
			}

			// find the inside/outside state before the smallest internal node
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Boolean stateBefore = (Boolean) getFirstLeaf(root).getAttribute();
			bool? stateBefore = (bool?) getFirstLeaf(root).Attribute;

			// find the inside/outside state after the largest internal node
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Boolean stateAfter = (Boolean) getLastLeaf(root).getAttribute();
			bool? stateAfter = (bool?) getLastLeaf(root).Attribute;

			if (stateBefore ^ stateAfter)
			{
				throw new InconsistentStateAt2PiWrapping();
			}

		}

		/// <summary>
		/// Get the first leaf node of a tree. </summary>
		/// <param name="root"> tree root </param>
		/// <returns> first leaf node (i.e. node corresponding to the region just after 0.0 radians) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private mathlib.geometry.partitioning.BSPTree<Sphere1D> getFirstLeaf(final mathlib.geometry.partitioning.BSPTree<Sphere1D> root)
		private BSPTree<Sphere1D> getFirstLeaf(BSPTree<Sphere1D> root)
		{

			if (root.Cut == null)
			{
				return root;
			}

			// find the smallest internal node
			BSPTree<Sphere1D> smallest = null;
			for (BSPTree<Sphere1D> n = root; n != null; n = previousInternalNode(n))
			{
				smallest = n;
			}

			return leafBefore(smallest);

		}

		/// <summary>
		/// Get the last leaf node of a tree. </summary>
		/// <param name="root"> tree root </param>
		/// <returns> last leaf node (i.e. node corresponding to the region just before \( 2 \pi \) radians) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private mathlib.geometry.partitioning.BSPTree<Sphere1D> getLastLeaf(final mathlib.geometry.partitioning.BSPTree<Sphere1D> root)
		private BSPTree<Sphere1D> getLastLeaf(BSPTree<Sphere1D> root)
		{

			if (root.Cut == null)
			{
				return root;
			}

			// find the largest internal node
			BSPTree<Sphere1D> largest = null;
			for (BSPTree<Sphere1D> n = root; n != null; n = nextInternalNode(n))
			{
				largest = n;
			}

			return leafAfter(largest);

		}

		/// <summary>
		/// Get the node corresponding to the first arc start. </summary>
		/// <returns> smallest internal node (i.e. first after 0.0 radians, in trigonometric direction),
		/// or null if there are no internal nodes (i.e. the set is either empty or covers the full circle) </returns>
		private BSPTree<Sphere1D> FirstArcStart
		{
			get
			{
    
				// start search at the tree root
				BSPTree<Sphere1D> node = getTree(false);
				if (node.Cut == null)
				{
					return null;
				}
    
				// walk tree until we find the smallest internal node
				node = getFirstLeaf(node).Parent;
    
				// walk tree until we find an arc start
				while (node != null && !isArcStart(node))
				{
					node = nextInternalNode(node);
				}
    
				return node;
    
			}
		}

		/// <summary>
		/// Check if an internal node corresponds to the start angle of an arc. </summary>
		/// <param name="node"> internal node to check </param>
		/// <returns> true if the node corresponds to the start angle of an arc </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private boolean isArcStart(final mathlib.geometry.partitioning.BSPTree<Sphere1D> node)
		private bool isArcStart(BSPTree<Sphere1D> node)
		{

			if ((bool?) leafBefore(node).Attribute)
			{
				// it has an inside cell before it, it may end an arc but not start it
				return false;
			}

			if (!(bool?) leafAfter(node).Attribute)
			{
				// it has an outside cell after it, it is a dummy cut away from real arcs
				return false;
			}

			// the cell has an outside before and an inside after it
			// it is the start of an arc
			return true;

		}

		/// <summary>
		/// Check if an internal node corresponds to the end angle of an arc. </summary>
		/// <param name="node"> internal node to check </param>
		/// <returns> true if the node corresponds to the end angle of an arc </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private boolean isArcEnd(final mathlib.geometry.partitioning.BSPTree<Sphere1D> node)
		private bool isArcEnd(BSPTree<Sphere1D> node)
		{

			if (!(bool?) leafBefore(node).Attribute)
			{
				// it has an outside cell before it, it may start an arc but not end it
				return false;
			}

			if ((bool?) leafAfter(node).Attribute)
			{
				// it has an inside cell after it, it is a dummy cut in the middle of an arc
				return false;
			}

			// the cell has an inside before and an outside after it
			// it is the end of an arc
			return true;

		}

		/// <summary>
		/// Get the next internal node. </summary>
		/// <param name="node"> current internal node </param>
		/// <returns> next internal node in trigonometric order, or null
		/// if this is the last internal node </returns>
		private BSPTree<Sphere1D> nextInternalNode(BSPTree<Sphere1D> node)
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
		/// <returns> previous internal node in trigonometric order, or null
		/// if this is the first internal node </returns>
		private BSPTree<Sphere1D> previousInternalNode(BSPTree<Sphere1D> node)
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
		private BSPTree<Sphere1D> leafBefore(BSPTree<Sphere1D> node)
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
		private BSPTree<Sphere1D> leafAfter(BSPTree<Sphere1D> node)
		{

			node = childAfter(node);
			while (node.Cut != null)
			{
				node = childBefore(node);
			}

			return node;

		}

		/// <summary>
		/// Check if a node is the child before its parent in trigonometric order. </summary>
		/// <param name="node"> child node considered </param>
		/// <returns> true is the node has a parent end is before it in trigonometric order </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private boolean isBeforeParent(final mathlib.geometry.partitioning.BSPTree<Sphere1D> node)
		private bool isBeforeParent(BSPTree<Sphere1D> node)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<Sphere1D> parent = node.getParent();
			BSPTree<Sphere1D> parent = node.Parent;
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
		/// Check if a node is the child after its parent in trigonometric order. </summary>
		/// <param name="node"> child node considered </param>
		/// <returns> true is the node has a parent end is after it in trigonometric order </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private boolean isAfterParent(final mathlib.geometry.partitioning.BSPTree<Sphere1D> node)
		private bool isAfterParent(BSPTree<Sphere1D> node)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<Sphere1D> parent = node.getParent();
			BSPTree<Sphere1D> parent = node.Parent;
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
		private BSPTree<Sphere1D> childBefore(BSPTree<Sphere1D> node)
		{
			if (isDirect(node))
			{
				// smaller angles are on minus side, larger angles are on plus side
				return node.Minus;
			}
			else
			{
				// smaller angles are on plus side, larger angles are on minus side
				return node.Plus;
			}
		}

		/// <summary>
		/// Find the child node just after an internal node. </summary>
		/// <param name="node"> internal node at which the sub-tree starts </param>
		/// <returns> child node just after the internal node </returns>
		private BSPTree<Sphere1D> childAfter(BSPTree<Sphere1D> node)
		{
			if (isDirect(node))
			{
				// smaller angles are on minus side, larger angles are on plus side
				return node.Plus;
			}
			else
			{
				// smaller angles are on plus side, larger angles are on minus side
				return node.Minus;
			}
		}

		/// <summary>
		/// Check if an internal node has a direct limit angle. </summary>
		/// <param name="node"> internal node to check </param>
		/// <returns> true if the limit angle is direct </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private boolean isDirect(final mathlib.geometry.partitioning.BSPTree<Sphere1D> node)
		private bool isDirect(BSPTree<Sphere1D> node)
		{
			return ((LimitAngle) node.Cut.Hyperplane).Direct;
		}

		/// <summary>
		/// Get the limit angle of an internal node. </summary>
		/// <param name="node"> internal node to check </param>
		/// <returns> limit angle </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double getAngle(final mathlib.geometry.partitioning.BSPTree<Sphere1D> node)
		private double getAngle(BSPTree<Sphere1D> node)
		{
			return ((LimitAngle) node.Cut.Hyperplane).Location.Alpha;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public ArcsSet buildNew(final mathlib.geometry.partitioning.BSPTree<Sphere1D> tree)
		public override ArcsSet buildNew(BSPTree<Sphere1D> tree)
		{
			return new ArcsSet(tree, Tolerance);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override void computeGeometricalProperties()
		{
			if (getTree(false).Cut == null)
			{
				Barycenter = S1Point.NaN_Renamed;
				Size = ((bool?) getTree(false).Attribute) ? MathUtils.TWO_PI : 0;
			}
			else
			{
				double size = 0.0;
				double sum = 0.0;
				foreach (double[] a in this)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double length = a[1] - a[0];
					double length = a[1] - a[0];
					size += length;
					sum += length * (a[0] + a[1]);
				}
				Size = size;
				if (Precision.Equals(size, MathUtils.TWO_PI, 0))
				{
					Barycenter = S1Point.NaN_Renamed;
				}
				else if (size >= Precision.SAFE_MIN)
				{
					Barycenter = new S1Point(sum / (2 * size));
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LimitAngle limit = (LimitAngle) getTree(false).getCut().getHyperplane();
					LimitAngle limit = (LimitAngle) getTree(false).Cut.Hyperplane;
					Barycenter = limit.Location;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.3
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public mathlib.geometry.partitioning.BoundaryProjection<Sphere1D> projectToBoundary(final mathlib.geometry.Point<Sphere1D> point)
		public override BoundaryProjection<Sphere1D> projectToBoundary(Point<Sphere1D> point)
		{

			// get position of test point
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double alpha = ((S1Point) point).getAlpha();
			double alpha = ((S1Point) point).Alpha;

			bool wrapFirst = false;
			double first = double.NaN;
			double previous = double.NaN;
			foreach (double[] a in this)
			{

				if (double.IsNaN(first))
				{
					// remember the first angle in case we need it later
					first = a[0];
				}

				if (!wrapFirst)
				{
					if (alpha < a[0])
					{
						// the test point lies between the previous and the current arcs
						// offset will be positive
						if (double.IsNaN(previous))
						{
							// we need to wrap around the circle
							wrapFirst = true;
						}
						else
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double previousOffset = alpha - previous;
							double previousOffset = alpha - previous;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double currentOffset = a[0] - alpha;
							double currentOffset = a[0] - alpha;
							if (previousOffset < currentOffset)
							{
								return new BoundaryProjection<Sphere1D>(point, new S1Point(previous), previousOffset);
							}
							else
							{
								return new BoundaryProjection<Sphere1D>(point, new S1Point(a[0]), currentOffset);
							}
						}
					}
					else if (alpha <= a[1])
					{
						// the test point lies within the current arc
						// offset will be negative
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double offset0 = a[0] - alpha;
						double offset0 = a[0] - alpha;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double offset1 = alpha - a[1];
						double offset1 = alpha - a[1];
						if (offset0 < offset1)
						{
							return new BoundaryProjection<Sphere1D>(point, new S1Point(a[1]), offset1);
						}
						else
						{
							return new BoundaryProjection<Sphere1D>(point, new S1Point(a[0]), offset0);
						}
					}
				}
				previous = a[1];
			}

			if (double.IsNaN(previous))
			{

				// there are no points at all in the arcs set
				return new BoundaryProjection<Sphere1D>(point, null, MathUtils.TWO_PI);

			}
			else
			{

				// the test point if before first arc and after last arc,
				// somewhere around the 0/2 \pi crossing
				if (wrapFirst)
				{
					// the test point is between 0 and first
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double previousOffset = alpha - (previous - mathlib.util.MathUtils.TWO_PI);
					double previousOffset = alpha - (previous - MathUtils.TWO_PI);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double currentOffset = first - alpha;
					double currentOffset = first - alpha;
					if (previousOffset < currentOffset)
					{
						return new BoundaryProjection<Sphere1D>(point, new S1Point(previous), previousOffset);
					}
					else
					{
						return new BoundaryProjection<Sphere1D>(point, new S1Point(first), currentOffset);
					}
				}
				else
				{
					// the test point is between last and 2\pi
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double previousOffset = alpha - previous;
					double previousOffset = alpha - previous;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double currentOffset = first + mathlib.util.MathUtils.TWO_PI - alpha;
					double currentOffset = first + MathUtils.TWO_PI - alpha;
					if (previousOffset < currentOffset)
					{
						return new BoundaryProjection<Sphere1D>(point, new S1Point(previous), previousOffset);
					}
					else
					{
						return new BoundaryProjection<Sphere1D>(point, new S1Point(first), currentOffset);
					}
				}

			}

		}

		/// <summary>
		/// Build an ordered list of arcs representing the instance.
		/// <p>This method builds this arcs set as an ordered list of
		/// <seealso cref="Arc Arc"/> elements. An empty tree will build an empty list
		/// while a tree representing the whole circle will build a one
		/// element list with bounds set to \( 0 and 2 \pi \).</p> </summary>
		/// <returns> a new ordered list containing <seealso cref="Arc Arc"/> elements </returns>
		public virtual IList<Arc> asList()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Arc> list = new java.util.ArrayList<Arc>();
			IList<Arc> list = new List<Arc>();
			foreach (double[] a in this)
			{
				list.Add(new Arc(a[0], a[1], Tolerance));
			}
			return list;
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// The iterator returns the limit angles pairs of sub-arcs in trigonometric order.
		/// </p>
		/// <p>
		/// The iterator does <em>not</em> support the optional {@code remove} operation.
		/// </p>
		/// </summary>
		public virtual IEnumerator<double[]> GetEnumerator()
		{
			return new SubArcsIterator(this);
		}

		/// <summary>
		/// Local iterator for sub-arcs. </summary>
		private class SubArcsIterator : IEnumerator<double[]>
		{
			private readonly ArcsSet outerInstance;


			/// <summary>
			/// Start of the first arc. </summary>
			internal readonly BSPTree<Sphere1D> firstStart;

			/// <summary>
			/// Current node. </summary>
			internal BSPTree<Sphere1D> current;

			/// <summary>
			/// Sub-arc no yet returned. </summary>
			internal double[] pending;

			/// <summary>
			/// Simple constructor.
			/// </summary>
			public SubArcsIterator(ArcsSet outerInstance)
			{
				this.outerInstance = outerInstance;

				firstStart = outerInstance.FirstArcStart;
				current = firstStart;

				if (firstStart == null)
				{
					// all the leaf tree nodes share the same inside/outside status
					if ((bool?) outerInstance.getFirstLeaf(outerInstance.getTree(false)).Attribute)
					{
						// it is an inside node, it represents the full circle
						pending = new double[] {0, MathUtils.TWO_PI};
					}
					else
					{
						pending = null;
					}
				}
				else
				{
					selectPending();
				}
			}

			/// <summary>
			/// Walk the tree to select the pending sub-arc.
			/// </summary>
			internal virtual void selectPending()
			{

				// look for the start of the arc
				BSPTree<Sphere1D> start = current;
				while (start != null && !outerInstance.isArcStart(start))
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

				// look for the end of the arc
				BSPTree<Sphere1D> end = start;
				while (end != null && !outerInstance.isArcEnd(end))
				{
					end = outerInstance.nextInternalNode(end);
				}

				if (end != null)
				{

					// we have identified the arc
					pending = new double[] {outerInstance.getAngle(start), outerInstance.getAngle(end)};

					// prepare search for next arc
					current = end;

				}
				else
				{

					// the final arc wraps around 2\pi, its end is before the first start
					end = firstStart;
					while (end != null && !outerInstance.isArcEnd(end))
					{
						end = outerInstance.previousInternalNode(end);
					}
					if (end == null)
					{
						// this should never happen
						throw new MathInternalError();
					}

					// we have identified the last arc
					pending = new double[] {outerInstance.getAngle(start), outerInstance.getAngle(end) + MathUtils.TWO_PI};

					// there won't be any other arcs
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

		/// <summary>
		/// Compute the relative position of the instance with respect
		/// to an arc.
		/// <p>
		/// The <seealso cref="Side#MINUS"/> side of the arc is the one covered by the arc.
		/// </p> </summary>
		/// <param name="arc"> arc to check instance against </param>
		/// <returns> one of <seealso cref="Side#PLUS"/>, <seealso cref="Side#MINUS"/>, <seealso cref="Side#BOTH"/>
		/// or <seealso cref="Side#HYPER"/> </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.geometry.partitioning.Side side(final Arc arc)
		public virtual Side side(Arc arc)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double reference = mathlib.util.FastMath.PI + arc.getInf();
			double reference = FastMath.PI + arc.Inf;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double arcLength = arc.getSup() - arc.getInf();
			double arcLength = arc.Sup - arc.Inf;

			bool inMinus = false;
			bool inPlus = false;
			foreach (double[] a in this)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double syncedStart = mathlib.util.MathUtils.normalizeAngle(a[0], reference) - arc.getInf();
				double syncedStart = MathUtils.normalizeAngle(a[0], reference) - arc.Inf;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double arcOffset = a[0] - syncedStart;
				double arcOffset = a[0] - syncedStart;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double syncedEnd = a[1] - arcOffset;
				double syncedEnd = a[1] - arcOffset;
				if (syncedStart <= arcLength - Tolerance || syncedEnd >= MathUtils.TWO_PI + Tolerance)
				{
					inMinus = true;
				}
				if (syncedEnd >= arcLength + Tolerance)
				{
					inPlus = true;
				}
			}

			if (inMinus)
			{
				if (inPlus)
				{
					return Side.BOTH;
				}
				else
				{
					return Side.MINUS;
				}
			}
			else
			{
				if (inPlus)
				{
					return Side.PLUS;
				}
				else
				{
					return Side.HYPER;
				}
			}

		}

		/// <summary>
		/// Split the instance in two parts by an arc. </summary>
		/// <param name="arc"> splitting arc </param>
		/// <returns> an object containing both the part of the instance
		/// on the plus side of the arc and the part of the
		/// instance on the minus side of the arc </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Split split(final Arc arc)
		public virtual Split Split(Arc arc)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Double> minus = new java.util.ArrayList<Double>();
			IList<double?> minus = new List<double?>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Double> plus = new java.util.ArrayList<Double>();
			IList<double?> plus = new List<double?>();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double reference = mathlib.util.FastMath.PI + arc.getInf();
			double reference = FastMath.PI + arc.Inf;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double arcLength = arc.getSup() - arc.getInf();
			double arcLength = arc.Sup - arc.Inf;

			foreach (double[] a in this)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double syncedStart = mathlib.util.MathUtils.normalizeAngle(a[0], reference) - arc.getInf();
				double syncedStart = MathUtils.normalizeAngle(a[0], reference) - arc.Inf;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double arcOffset = a[0] - syncedStart;
				double arcOffset = a[0] - syncedStart;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double syncedEnd = a[1] - arcOffset;
				double syncedEnd = a[1] - arcOffset;
				if (syncedStart < arcLength)
				{
					// the start point a[0] is in the minus part of the arc
					minus.Add(a[0]);
					if (syncedEnd > arcLength)
					{
						// the end point a[1] is past the end of the arc
						// so we leave the minus part and enter the plus part
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double minusToPlus = arcLength + arcOffset;
						double minusToPlus = arcLength + arcOffset;
						minus.Add(minusToPlus);
						plus.Add(minusToPlus);
						if (syncedEnd > MathUtils.TWO_PI)
						{
							// in fact the end point a[1] goes far enough that we
							// leave the plus part of the arc and enter the minus part again
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double plusToMinus = mathlib.util.MathUtils.TWO_PI + arcOffset;
							double plusToMinus = MathUtils.TWO_PI + arcOffset;
							plus.Add(plusToMinus);
							minus.Add(plusToMinus);
							minus.Add(a[1]);
						}
						else
						{
							// the end point a[1] is in the plus part of the arc
							plus.Add(a[1]);
						}
					}
					else
					{
						// the end point a[1] is in the minus part of the arc
						minus.Add(a[1]);
					}
				}
				else
				{
					// the start point a[0] is in the plus part of the arc
					plus.Add(a[0]);
					if (syncedEnd > MathUtils.TWO_PI)
					{
						// the end point a[1] wraps around to the start of the arc
						// so we leave the plus part and enter the minus part
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double plusToMinus = mathlib.util.MathUtils.TWO_PI + arcOffset;
						double plusToMinus = MathUtils.TWO_PI + arcOffset;
						plus.Add(plusToMinus);
						minus.Add(plusToMinus);
						if (syncedEnd > MathUtils.TWO_PI + arcLength)
						{
							// in fact the end point a[1] goes far enough that we
							// leave the minus part of the arc and enter the plus part again
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double minusToPlus = mathlib.util.MathUtils.TWO_PI + arcLength + arcOffset;
							double minusToPlus = MathUtils.TWO_PI + arcLength + arcOffset;
							minus.Add(minusToPlus);
							plus.Add(minusToPlus);
							plus.Add(a[1]);
						}
						else
						{
							// the end point a[1] is in the minus part of the arc
							minus.Add(a[1]);
						}
					}
					else
					{
						// the end point a[1] is in the plus part of the arc
						plus.Add(a[1]);
					}
				}
			}

			return new Split(createSplitPart(plus), createSplitPart(minus));

		}

		/// <summary>
		/// Add an arc limit to a BSP tree under construction. </summary>
		/// <param name="tree"> BSP tree under construction </param>
		/// <param name="alpha"> arc limit </param>
		/// <param name="isStart"> if true, the limit is the start of an arc </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void addArcLimit(final mathlib.geometry.partitioning.BSPTree<Sphere1D> tree, final double alpha, final boolean isStart)
		private void addArcLimit(BSPTree<Sphere1D> tree, double alpha, bool isStart)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LimitAngle limit = new LimitAngle(new S1Point(alpha), !isStart, getTolerance());
			LimitAngle limit = new LimitAngle(new S1Point(alpha), !isStart, Tolerance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<Sphere1D> node = tree.getCell(limit.getLocation(), getTolerance());
			BSPTree<Sphere1D> node = tree.getCell(limit.Location, Tolerance);
			if (node.Cut != null)
			{
				// this should never happen
				throw new MathInternalError();
			}

			node.insertCut(limit);
			node.Attribute = null;
			node.Plus.Attribute = false;
			node.Minus.Attribute = true;

		}

		/// <summary>
		/// Create a split part.
		/// <p>
		/// As per construction, the list of limit angles is known to have
		/// an even number of entries, with start angles at even indices and
		/// end angles at odd indices.
		/// </p> </summary>
		/// <param name="limits"> limit angles of the split part </param>
		/// <returns> split part (may be null) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private ArcsSet createSplitPart(final java.util.List<Double> limits)
		private ArcsSet createSplitPart(IList<double?> limits)
		{
			if (limits.Count == 0)
			{
				return null;
			}
			else
			{

				// collapse close limit angles
				for (int i = 0; i < limits.Count; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int j = (i + 1) % limits.size();
					int j = (i + 1) % limits.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lA = limits.get(i);
					double lA = limits[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lB = mathlib.util.MathUtils.normalizeAngle(limits.get(j), lA);
					double lB = MathUtils.normalizeAngle(limits[j], lA);
					if (FastMath.abs(lB - lA) <= Tolerance)
					{
						// the two limits are too close to each other, we remove both of them
						if (j > 0)
						{
							// regular case, the two entries are consecutive ones
							limits.RemoveAt(j);
							limits.RemoveAt(i);
							i = i - 1;
						}
						else
						{
							// special case, i the the last entry and j is the first entry
							// we have wrapped around list end
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lEnd = limits.remove(limits.size() - 1);
							double lEnd = limits.Remove(limits.Count - 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lStart = limits.remove(0);
							double lStart = limits.Remove(0);
							if (limits.Count == 0)
							{
								// the ends were the only limits, is it a full circle or an empty circle?
								if (lEnd - lStart > FastMath.PI)
								{
									// it was full circle
									return new ArcsSet(new BSPTree<Sphere1D>(true), Tolerance);
								}
								else
								{
									// it was an empty circle
									return null;
								}
							}
							else
							{
								// we have removed the first interval start, so our list
								// currently starts with an interval end, which is wrong
								// we need to move this interval end to the end of the list
								limits.Add(limits.Remove(0) + MathUtils.TWO_PI);
							}
						}
					}
				}

				// build the tree by adding all angular sectors
				BSPTree<Sphere1D> tree = new BSPTree<Sphere1D>(false);
				for (int i = 0; i < limits.Count - 1; i += 2)
				{
					addArcLimit(tree, limits[i], true);
					addArcLimit(tree, limits[i + 1], false);
				}

				if (tree.Cut == null)
				{
					// we did not insert anything
					return null;
				}

				return new ArcsSet(tree, Tolerance);

			}
		}

		/// <summary>
		/// Class holding the results of the <seealso cref="#split split"/> method.
		/// </summary>
		public class Split
		{

			/// <summary>
			/// Part of the arcs set on the plus side of the splitting arc. </summary>
			internal readonly ArcsSet plus;

			/// <summary>
			/// Part of the arcs set on the minus side of the splitting arc. </summary>
			internal readonly ArcsSet minus;

			/// <summary>
			/// Build a Split from its parts. </summary>
			/// <param name="plus"> part of the arcs set on the plus side of the
			/// splitting arc </param>
			/// <param name="minus"> part of the arcs set on the minus side of the
			/// splitting arc </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Split(final ArcsSet plus, final ArcsSet minus)
			internal Split(ArcsSet plus, ArcsSet minus)
			{
				this.plus = plus;
				this.minus = minus;
			}

			/// <summary>
			/// Get the part of the arcs set on the plus side of the splitting arc. </summary>
			/// <returns> part of the arcs set on the plus side of the splitting arc </returns>
			public virtual ArcsSet Plus
			{
				get
				{
					return plus;
				}
			}

			/// <summary>
			/// Get the part of the arcs set on the minus side of the splitting arc. </summary>
			/// <returns> part of the arcs set on the minus side of the splitting arc </returns>
			public virtual ArcsSet Minus
			{
				get
				{
					return minus;
				}
			}

		}

		/// <summary>
		/// Specialized exception for inconsistent BSP tree state inconsistency.
		/// <p>
		/// This exception is thrown at <seealso cref="ArcsSet"/> construction time when the
		/// <seealso cref="mathlib.geometry.partitioning.Region.Location inside/outside"/>
		/// state is not consistent at the 0, \(2 \pi \) crossing.
		/// </p>
		/// </summary>
		public class InconsistentStateAt2PiWrapping : MathIllegalArgumentException
		{

			/// <summary>
			/// Serializable UID. </summary>
			internal const long serialVersionUID = 20140107L;

			/// <summary>
			/// Simple constructor.
			/// </summary>
			public InconsistentStateAt2PiWrapping() : base(LocalizedFormats.INCONSISTENT_STATE_AT_2_PI_WRAPPING)
			{
			}

		}

	}

}
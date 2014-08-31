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
namespace org.apache.commons.math3.geometry.euclidean.twod
{


	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using org.apache.commons.math3.geometry;
	using IntervalsSet = org.apache.commons.math3.geometry.euclidean.oned.IntervalsSet;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;

	/// <summary>
	/// This class represent a tree of nested 2D boundary loops.
	/// 
	/// <p>This class is used for piecewise polygons construction.
	/// Polygons are built using the outline edges as
	/// representative of boundaries, the orientation of these lines are
	/// meaningful. However, we want to allow the user to specify its
	/// outline loops without having to take care of this orientation. This
	/// class is devoted to correct mis-oriented loops.<p>
	/// 
	/// <p>Orientation is computed assuming the piecewise polygon is finite,
	/// i.e. the outermost loops have their exterior side facing points at
	/// infinity, and hence are oriented counter-clockwise. The orientation of
	/// internal loops is computed as the reverse of the orientation of
	/// their immediate surrounding loop.</p>
	/// 
	/// @version $Id: NestedLoops.java 1555174 2014-01-03 18:06:20Z luc $
	/// @since 3.0
	/// </summary>
	internal class NestedLoops
	{

		/// <summary>
		/// Boundary loop. </summary>
		private Vector2D[] loop;

		/// <summary>
		/// Surrounded loops. </summary>
		private List<NestedLoops> surrounded;

		/// <summary>
		/// Polygon enclosing a finite region. </summary>
		private Region<Euclidean2D> polygon;

		/// <summary>
		/// Indicator for original loop orientation. </summary>
		private bool originalIsClockwise;

		/// <summary>
		/// Tolerance below which points are considered identical. </summary>
		private readonly double tolerance;

		/// <summary>
		/// Simple Constructor.
		/// <p>Build an empty tree of nested loops. This instance will become
		/// the root node of a complete tree, it is not associated with any
		/// loop by itself, the outermost loops are in the root tree child
		/// nodes.</p> </summary>
		/// <param name="tolerance"> tolerance below which points are considered identical
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NestedLoops(final double tolerance)
		public NestedLoops(double tolerance)
		{
			this.surrounded = new List<NestedLoops>();
			this.tolerance = tolerance;
		}

		/// <summary>
		/// Constructor.
		/// <p>Build a tree node with neither parent nor children</p> </summary>
		/// <param name="loop"> boundary loop (will be reversed in place if needed) </param>
		/// <param name="tolerance"> tolerance below which points are considered identical </param>
		/// <exception cref="MathIllegalArgumentException"> if an outline has an open boundary loop
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private NestedLoops(final Vector2D[] loop, final double tolerance) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private NestedLoops(Vector2D[] loop, double tolerance)
		{

			if (loop[0] == null)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.OUTLINE_BOUNDARY_LOOP_OPEN);
			}

			this.loop = loop;
			this.surrounded = new List<NestedLoops>();
			this.tolerance = tolerance;

			// build the polygon defined by the loop
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<org.apache.commons.math3.geometry.partitioning.SubHyperplane<Euclidean2D>> edges = new java.util.ArrayList<org.apache.commons.math3.geometry.partitioning.SubHyperplane<Euclidean2D>>();
			List<SubHyperplane<Euclidean2D>> edges = new List<SubHyperplane<Euclidean2D>>();
			Vector2D current = loop[loop.Length - 1];
			for (int i = 0; i < loop.Length; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D previous = current;
				Vector2D previous = current;
				current = loop[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Line line = new Line(previous, current, tolerance);
				Line line = new Line(previous, current, tolerance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.oned.IntervalsSet region = new org.apache.commons.math3.geometry.euclidean.oned.IntervalsSet(line.toSubSpace((org.apache.commons.math3.geometry.Point<Euclidean2D>) previous).getX(), line.toSubSpace((org.apache.commons.math3.geometry.Point<Euclidean2D>) current).getX(), tolerance);
				IntervalsSet region = new IntervalsSet(line.toSubSpace((Point<Euclidean2D>) previous).X, line.toSubSpace((Point<Euclidean2D>) current).X, tolerance);
				edges.Add(new SubLine(line, region));
			}
			polygon = new PolygonsSet(edges, tolerance);

			// ensure the polygon encloses a finite region of the plane
			if (double.IsInfinity(polygon.Size))
			{
				polygon = (new RegionFactory<Euclidean2D>()).getComplement(polygon);
				originalIsClockwise = false;
			}
			else
			{
				originalIsClockwise = true;
			}

		}

		/// <summary>
		/// Add a loop in a tree. </summary>
		/// <param name="bLoop"> boundary loop (will be reversed in place if needed) </param>
		/// <exception cref="MathIllegalArgumentException"> if an outline has crossing
		/// boundary loops or open boundary loops </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void add(final Vector2D[] bLoop) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void add(Vector2D[] bLoop)
		{
			add(new NestedLoops(bLoop, tolerance));
		}

		/// <summary>
		/// Add a loop in a tree. </summary>
		/// <param name="node"> boundary loop (will be reversed in place if needed) </param>
		/// <exception cref="MathIllegalArgumentException"> if an outline has boundary
		/// loops that cross each other </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void add(final NestedLoops node) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void add(NestedLoops node)
		{

			// check if we can go deeper in the tree
			foreach (NestedLoops child in surrounded)
			{
				if (child.polygon.contains(node.polygon))
				{
					child.add(node);
					return;
				}
			}

			// check if we can absorb some of the instance children
			for (final IEnumerator<NestedLoops> iterator = surrounded.GetEnumerator(); iterator.hasNext();)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final NestedLoops child = iterator.next();
				NestedLoops child = iterator.next();
				if (node.polygon.contains(child.polygon))
				{
					node.surrounded.Add(child);
					iterator.remove();
				}
			}

			// we should be separate from the remaining children
			RegionFactory<Euclidean2D> factory = new RegionFactory<Euclidean2D>();
			foreach (NestedLoops child in surrounded)
			{
				if (!factory.intersection(node.polygon, child.polygon).Empty)
				{
					throw new MathIllegalArgumentException(LocalizedFormats.CROSSING_BOUNDARY_LOOPS);
				}
			}

			surrounded.Add(node);

		}

		/// <summary>
		/// Correct the orientation of the loops contained in the tree.
		/// <p>This is this method that really inverts the loops that where
		/// provided through the <seealso cref="#add(Vector2D[]) add"/> method if
		/// they are mis-oriented</p>
		/// </summary>
		public virtual void correctOrientation()
		{
			foreach (NestedLoops child in surrounded)
			{
				child.ClockWise = true;
			}
		}

		/// <summary>
		/// Set the loop orientation. </summary>
		/// <param name="clockwise"> if true, the loop should be set to clockwise
		/// orientation </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void setClockWise(final boolean clockwise)
		private bool ClockWise
		{
			set
			{
    
				if (originalIsClockwise ^ value)
				{
					// we need to inverse the original loop
					int min = -1;
					int max = loop.Length;
					while (++min < --max)
					{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Vector2D tmp = loop[min];
						Vector2D tmp = loop[min];
						loop[min] = loop[max];
						loop[max] = tmp;
					}
				}
    
				// go deeper in the tree
				foreach (NestedLoops child in surrounded)
				{
					child.ClockWise = !value;
				}
    
			}
		}

	}

}
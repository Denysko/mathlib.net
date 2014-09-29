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
namespace mathlib.geometry.euclidean.twod.hull
{


	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Implements Andrew's monotone chain method to generate the convex hull of a finite set of
	/// points in the two-dimensional euclidean space.
	/// <p>
	/// The runtime complexity is O(n log n), with n being the number of input points. If the
	/// point set is already sorted (by x-coordinate), the runtime complexity is O(n).
	/// <p>
	/// The implementation is not sensitive to collinear points on the hull. The parameter
	/// {@code includeCollinearPoints} allows to control the behavior with regard to collinear points.
	/// If {@code true}, all points on the boundary of the hull will be added to the hull vertices,
	/// otherwise only the extreme points will be present. By default, collinear points are not added
	/// as hull vertices.
	/// <p>
	/// The {@code tolerance} parameter (default: 1e-10) is used as epsilon criteria to determine
	/// identical and collinear points.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikibooks.org/wiki/Algorithm_Implementation/Geometry/Convex_hull/Monotone_chain">
	/// Andrew's monotone chain algorithm (Wikibooks)</a>
	/// @since 3.3
	/// @version $Id: MonotoneChain.java 1568752 2014-02-16 12:19:51Z tn $ </seealso>
	public class MonotoneChain : AbstractConvexHullGenerator2D
	{

		/// <summary>
		/// Create a new MonotoneChain instance.
		/// </summary>
		public MonotoneChain() : this(false)
		{
		}

		/// <summary>
		/// Create a new MonotoneChain instance. </summary>
		/// <param name="includeCollinearPoints"> whether collinear points shall be added as hull vertices </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MonotoneChain(final boolean includeCollinearPoints)
		public MonotoneChain(bool includeCollinearPoints) : base(includeCollinearPoints)
		{
		}

		/// <summary>
		/// Create a new MonotoneChain instance. </summary>
		/// <param name="includeCollinearPoints"> whether collinear points shall be added as hull vertices </param>
		/// <param name="tolerance"> tolerance below which points are considered identical </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MonotoneChain(final boolean includeCollinearPoints, final double tolerance)
		public MonotoneChain(bool includeCollinearPoints, double tolerance) : base(includeCollinearPoints, tolerance)
		{
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public java.util.Collection<mathlib.geometry.euclidean.twod.Vector2D> findHullVertices(final java.util.Collection<mathlib.geometry.euclidean.twod.Vector2D> points)
		public override ICollection<Vector2D> findHullVertices(ICollection<Vector2D> points)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<mathlib.geometry.euclidean.twod.Vector2D> pointsSortedByXAxis = new java.util.ArrayList<mathlib.geometry.euclidean.twod.Vector2D>(points);
			IList<Vector2D> pointsSortedByXAxis = new List<Vector2D>(points);

			// sort the points in increasing order on the x-axis
			pointsSortedByXAxis.Sort(new ComparatorAnonymousInnerClassHelper(this));

			// build lower hull
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<mathlib.geometry.euclidean.twod.Vector2D> lowerHull = new java.util.ArrayList<mathlib.geometry.euclidean.twod.Vector2D>();
			IList<Vector2D> lowerHull = new List<Vector2D>();
			foreach (Vector2D p in pointsSortedByXAxis)
			{
				updateHull(p, lowerHull);
			}

			// build upper hull
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<mathlib.geometry.euclidean.twod.Vector2D> upperHull = new java.util.ArrayList<mathlib.geometry.euclidean.twod.Vector2D>();
			IList<Vector2D> upperHull = new List<Vector2D>();
			for (int idx = pointsSortedByXAxis.Count - 1; idx >= 0; idx--)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.euclidean.twod.Vector2D p = pointsSortedByXAxis.get(idx);
				Vector2D p = pointsSortedByXAxis[idx];
				updateHull(p, upperHull);
			}

			// concatenate the lower and upper hulls
			// the last point of each list is omitted as it is repeated at the beginning of the other list
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<mathlib.geometry.euclidean.twod.Vector2D> hullVertices = new java.util.ArrayList<mathlib.geometry.euclidean.twod.Vector2D>(lowerHull.size() + upperHull.size() - 2);
			IList<Vector2D> hullVertices = new List<Vector2D>(lowerHull.Count + upperHull.Count - 2);
			for (int idx = 0; idx < lowerHull.Count - 1; idx++)
			{
				hullVertices.Add(lowerHull[idx]);
			}
			for (int idx = 0; idx < upperHull.Count - 1; idx++)
			{
				hullVertices.Add(upperHull[idx]);
			}

			// special case: if the lower and upper hull may contain only 1 point if all are identical
			if (hullVertices.Count == 0 && lowerHull.Count > 0)
			{
				hullVertices.Add(lowerHull[0]);
			}

			return hullVertices;
		}

		private class ComparatorAnonymousInnerClassHelper : IComparer<Vector2D>
		{
			private readonly MonotoneChain outerInstance;

			public ComparatorAnonymousInnerClassHelper(MonotoneChain outerInstance)
			{
				this.outerInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public int compare(final mathlib.geometry.euclidean.twod.Vector2D o1, final mathlib.geometry.euclidean.twod.Vector2D o2)
			public virtual int Compare(Vector2D o1, Vector2D o2)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int diff = (int) mathlib.util.FastMath.signum(o1.getX() - o2.getX());
				int diff = (int) FastMath.signum(o1.X - o2.X);
				if (diff == 0)
				{
					return (int) FastMath.signum(o1.Y - o2.Y);
				}
				else
				{
					return diff;
				}
			}
		}

		/// <summary>
		/// Update the partial hull with the current point.
		/// </summary>
		/// <param name="point"> the current point </param>
		/// <param name="hull"> the partial hull </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void updateHull(final mathlib.geometry.euclidean.twod.Vector2D point, final java.util.List<mathlib.geometry.euclidean.twod.Vector2D> hull)
		private void updateHull(Vector2D point, IList<Vector2D> hull)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tolerance = getTolerance();
			double tolerance = Tolerance;

			if (hull.Count == 1)
			{
				// ensure that we do not add an identical point
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.euclidean.twod.Vector2D p1 = hull.get(0);
				Vector2D p1 = hull[0];
				if (p1.distance(point) < tolerance)
				{
					return;
				}
			}

			while (hull.Count >= 2)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = hull.size();
				int size = hull.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.euclidean.twod.Vector2D p1 = hull.get(size - 2);
				Vector2D p1 = hull[size - 2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.euclidean.twod.Vector2D p2 = hull.get(size - 1);
				Vector2D p2 = hull[size - 1];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double offset = new mathlib.geometry.euclidean.twod.Line(p1, p2, tolerance).getOffset(point);
				double offset = (new Line(p1, p2, tolerance)).getOffset(point);
				if (FastMath.abs(offset) < tolerance)
				{
					// the point is collinear to the line (p1, p2)

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double distanceToCurrent = p1.distance(point);
					double distanceToCurrent = p1.distance(point);
					if (distanceToCurrent < tolerance || p2.distance(point) < tolerance)
					{
						// the point is assumed to be identical to either p1 or p2
						return;
					}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double distanceToLast = p1.distance(p2);
					double distanceToLast = p1.distance(p2);
					if (IncludeCollinearPoints)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = distanceToCurrent < distanceToLast ? size - 1 : size;
						int index = distanceToCurrent < distanceToLast ? size - 1 : size;
						hull.Insert(index, point);
					}
					else
					{
						if (distanceToCurrent > distanceToLast)
						{
							hull.RemoveAt(size - 1);
						}
						hull.Add(point);
					}
					return;
				}
				else if (offset > 0)
				{
					hull.RemoveAt(size - 1);
				}
				else
				{
					break;
				}
			}
			hull.Add(point);
		}

	}

}
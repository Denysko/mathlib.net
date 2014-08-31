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
namespace org.apache.commons.math3.geometry.euclidean.twod.hull
{


	/// <summary>
	/// A simple heuristic to improve the performance of convex hull algorithms.
	/// <p>
	/// The heuristic is based on the idea of a convex quadrilateral, which is formed by
	/// four points with the lowest and highest x / y coordinates. Any point that lies inside
	/// this quadrilateral can not be part of the convex hull and can thus be safely discarded
	/// before generating the convex hull itself.
	/// <p>
	/// The complexity of the operation is O(n), and may greatly improve the time it takes to
	/// construct the convex hull afterwards, depending on the point distribution.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Convex_hull_algorithms#Akl-Toussaint_heuristic">
	/// Akl-Toussaint heuristic (Wikipedia)</a>
	/// @since 3.3
	/// @version $Id: AklToussaintHeuristic.java 1563687 2014-02-02 17:56:13Z tn $ </seealso>
	public sealed class AklToussaintHeuristic
	{

		/// <summary>
		/// Hide utility constructor. </summary>
		private AklToussaintHeuristic()
		{
		}

		/// <summary>
		/// Returns a point set that is reduced by all points for which it is safe to assume
		/// that they are not part of the convex hull.
		/// </summary>
		/// <param name="points"> the original point set </param>
		/// <returns> a reduced point set, useful as input for convex hull algorithms </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static java.util.Collection<org.apache.commons.math3.geometry.euclidean.twod.Vector2D> reducePoints(final java.util.Collection<org.apache.commons.math3.geometry.euclidean.twod.Vector2D> points)
		public static ICollection<Vector2D> reducePoints(ICollection<Vector2D> points)
		{

			// find the leftmost point
			int size = 0;
			Vector2D minX = null;
			Vector2D maxX = null;
			Vector2D minY = null;
			Vector2D maxY = null;
			foreach (Vector2D p in points)
			{
				if (minX == null || p.X < minX.X)
				{
					minX = p;
				}
				if (maxX == null || p.X > maxX.X)
				{
					maxX = p;
				}
				if (minY == null || p.Y < minY.Y)
				{
					minY = p;
				}
				if (maxY == null || p.Y > maxY.Y)
				{
					maxY = p;
				}
				size++;
			}

			if (size < 4)
			{
				return points;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.apache.commons.math3.geometry.euclidean.twod.Vector2D> quadrilateral = buildQuadrilateral(minY, maxX, maxY, minX);
			IList<Vector2D> quadrilateral = buildQuadrilateral(minY, maxX, maxY, minX);
			// if the quadrilateral is not well formed, e.g. only 2 points, do not attempt to reduce
			if (quadrilateral.Count < 3)
			{
				return points;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.apache.commons.math3.geometry.euclidean.twod.Vector2D> reducedPoints = new java.util.ArrayList<org.apache.commons.math3.geometry.euclidean.twod.Vector2D>(quadrilateral);
			IList<Vector2D> reducedPoints = new List<Vector2D>(quadrilateral);
			foreach (Vector2D p in points)
			{
				// check all points if they are within the quadrilateral
				// in which case they can not be part of the convex hull
				if (!insideQuadrilateral(p, quadrilateral))
				{
					reducedPoints.Add(p);
				}
			}

			return reducedPoints;
		}

		/// <summary>
		/// Build the convex quadrilateral with the found corner points (with min/max x/y coordinates).
		/// </summary>
		/// <param name="points"> the respective points with min/max x/y coordinate </param>
		/// <returns> the quadrilateral </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static java.util.List<org.apache.commons.math3.geometry.euclidean.twod.Vector2D> buildQuadrilateral(final org.apache.commons.math3.geometry.euclidean.twod.Vector2D... points)
		private static IList<Vector2D> buildQuadrilateral(params Vector2D[] points)
		{
			IList<Vector2D> quadrilateral = new List<Vector2D>();
			foreach (Vector2D p in points)
			{
				if (!quadrilateral.Contains(p))
				{
					quadrilateral.Add(p);
				}
			}
			return quadrilateral;
		}

		/// <summary>
		/// Checks if the given point is located within the convex quadrilateral. </summary>
		/// <param name="point"> the point to check </param>
		/// <param name="quadrilateralPoints"> the convex quadrilateral, represented by 4 points </param>
		/// <returns> {@code true} if the point is inside the quadrilateral, {@code false} otherwise </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static boolean insideQuadrilateral(final org.apache.commons.math3.geometry.euclidean.twod.Vector2D point, final java.util.List<org.apache.commons.math3.geometry.euclidean.twod.Vector2D> quadrilateralPoints)
		private static bool insideQuadrilateral(Vector2D point, IList<Vector2D> quadrilateralPoints)
		{

			Vector2D p1 = quadrilateralPoints[0];
			Vector2D p2 = quadrilateralPoints[1];

			if (point.Equals(p1) || point.Equals(p2))
			{
				return true;
			}

			// get the location of the point relative to the first two vertices
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double last = point.crossProduct(p1, p2);
			double last = point.crossProduct(p1, p2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = quadrilateralPoints.size();
			int size = quadrilateralPoints.Count;
			// loop through the rest of the vertices
			for (int i = 1; i < size; i++)
			{
				p1 = p2;
				p2 = quadrilateralPoints[(i + 1) == size ? 0 : i + 1];

				if (point.Equals(p1) || point.Equals(p2))
				{
					return true;
				}

				// do side of line test: multiply the last location with this location
				// if they are the same sign then the operation will yield a positive result
				// -x * -y = +xy, x * y = +xy, -x * y = -xy, x * -y = -xy
				if (last * point.crossProduct(p1, p2) < 0)
				{
					return false;
				}
			}
			return true;
		}

	}

}
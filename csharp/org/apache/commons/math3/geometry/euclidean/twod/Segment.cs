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

	using org.apache.commons.math3.geometry;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Simple container for a two-points segment.
	/// @version $Id: Segment.java 1554651 2014-01-01 17:27:48Z luc $
	/// @since 3.0
	/// </summary>
	public class Segment
	{

		/// <summary>
		/// Start point of the segment. </summary>
		private readonly Vector2D start;

		/// <summary>
		/// End point of the segment. </summary>
		private readonly Vector2D end;

		/// <summary>
		/// Line containing the segment. </summary>
		private readonly Line line;

		/// <summary>
		/// Build a segment. </summary>
		/// <param name="start"> start point of the segment </param>
		/// <param name="end"> end point of the segment </param>
		/// <param name="line"> line containing the segment </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Segment(final Vector2D start, final Vector2D end, final Line line)
		public Segment(Vector2D start, Vector2D end, Line line)
		{
			this.start = start;
			this.end = end;
			this.line = line;
		}

		/// <summary>
		/// Get the start point of the segment. </summary>
		/// <returns> start point of the segment </returns>
		public virtual Vector2D Start
		{
			get
			{
				return start;
			}
		}

		/// <summary>
		/// Get the end point of the segment. </summary>
		/// <returns> end point of the segment </returns>
		public virtual Vector2D End
		{
			get
			{
				return end;
			}
		}

		/// <summary>
		/// Get the line containing the segment. </summary>
		/// <returns> line containing the segment </returns>
		public virtual Line Line
		{
			get
			{
				return line;
			}
		}

		/// <summary>
		/// Calculates the shortest distance from a point to this line segment.
		/// <p>
		/// If the perpendicular extension from the point to the line does not
		/// cross in the bounds of the line segment, the shortest distance to
		/// the two end points will be returned.
		/// </p>
		/// 
		/// Algorithm adapted from:
		/// <a href="http://www.codeguru.com/forum/printthread.php?s=cc8cf0596231f9a7dba4da6e77c29db3&t=194400&pp=15&page=1">
		/// Thread @ Codeguru</a>
		/// </summary>
		/// <param name="p"> to check </param>
		/// <returns> distance between the instance and the point
		/// @since 3.1 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double distance(final Vector2D p)
		public virtual double distance(Vector2D p)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double deltaX = end.getX() - start.getX();
			double deltaX = end.X - start.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double deltaY = end.getY() - start.getY();
			double deltaY = end.Y - start.Y;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double r = ((p.getX() - start.getX()) * deltaX + (p.getY() - start.getY()) * deltaY) / (deltaX * deltaX + deltaY * deltaY);
			double r = ((p.X - start.X) * deltaX + (p.Y - start.Y) * deltaY) / (deltaX * deltaX + deltaY * deltaY);

			// r == 0 => P = startPt
			// r == 1 => P = endPt
			// r < 0 => P is on the backward extension of the segment
			// r > 1 => P is on the forward extension of the segment
			// 0 < r < 1 => P is on the segment

			// if point isn't on the line segment, just return the shortest distance to the end points
			if (r < 0 || r > 1)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dist1 = getStart().distance((org.apache.commons.math3.geometry.Point<Euclidean2D>) p);
				double dist1 = Start.distance((Point<Euclidean2D>) p);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dist2 = getEnd().distance((org.apache.commons.math3.geometry.Point<Euclidean2D>) p);
				double dist2 = End.distance((Point<Euclidean2D>) p);

				return FastMath.min(dist1, dist2);
			}
			else
			{
				// find point on line and see if it is in the line segment
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double px = start.getX() + r * deltaX;
				double px = start.X + r * deltaX;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double py = start.getY() + r * deltaY;
				double py = start.Y + r * deltaY;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D interPt = new Vector2D(px, py);
				Vector2D interPt = new Vector2D(px, py);
				return interPt.distance((Point<Euclidean2D>) p);
			}
		}
	}

}
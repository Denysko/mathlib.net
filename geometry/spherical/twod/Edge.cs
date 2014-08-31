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
namespace org.apache.commons.math3.geometry.spherical.twod
{

	using Vector3D = org.apache.commons.math3.geometry.euclidean.threed.Vector3D;
	using Arc = org.apache.commons.math3.geometry.spherical.oned.Arc;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// Spherical polygons boundary edge. </summary>
	/// <seealso cref= SphericalPolygonsSet#getBoundaryLoops() </seealso>
	/// <seealso cref= Vertex
	/// @version $Id: Edge.java 1561506 2014-01-26 15:31:18Z luc $
	/// @since 3.3 </seealso>
	public class Edge
	{

		/// <summary>
		/// Start vertex. </summary>
		private readonly Vertex start;

		/// <summary>
		/// End vertex. </summary>
		private Vertex end;

		/// <summary>
		/// Length of the arc. </summary>
		private readonly double length;

		/// <summary>
		/// Circle supporting the edge. </summary>
		private readonly Circle circle;

		/// <summary>
		/// Build an edge not contained in any node yet. </summary>
		/// <param name="start"> start vertex </param>
		/// <param name="end"> end vertex </param>
		/// <param name="length"> length of the arc (it can be greater than \( \pi \)) </param>
		/// <param name="circle"> circle supporting the edge </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: Edge(final Vertex start, final Vertex end, final double length, final Circle circle)
		internal Edge(Vertex start, Vertex end, double length, Circle circle)
		{

			this.start = start;
			this.end = end;
			this.length = length;
			this.circle = circle;

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
		/// Get the length of the arc. </summary>
		/// <returns> length of the arc (can be greater than \( \pi \)) </returns>
		public virtual double Length
		{
			get
			{
				return length;
			}
		}

		/// <summary>
		/// Get the circle supporting this edge. </summary>
		/// <returns> circle supporting this edge </returns>
		public virtual Circle Circle
		{
			get
			{
				return circle;
			}
		}

		/// <summary>
		/// Get an intermediate point.
		/// <p>
		/// The angle along the edge should normally be between 0 and <seealso cref="#getLength()"/>
		/// in order to remain within edge limits. However, there are no checks on the
		/// value of the angle, so user can rebuild the full circle on which an edge is
		/// defined if they want.
		/// </p> </summary>
		/// <param name="alpha"> angle along the edge, counted from <seealso cref="#getStart()"/> </param>
		/// <returns> an intermediate point </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.euclidean.threed.Vector3D getPointAt(final double alpha)
		public virtual Vector3D getPointAt(double alpha)
		{
			return circle.getPointAt(alpha + circle.getPhase(start.Location.Vector));
		}

		/// <summary>
		/// Connect the instance with a following edge. </summary>
		/// <param name="next"> edge following the instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: void setNextEdge(final Edge next)
		internal virtual Edge NextEdge
		{
			set
			{
				end = value.Start;
				end.Incoming = this;
				end.bindWith(Circle);
			}
		}

		/// <summary>
		/// Split the edge.
		/// <p>
		/// Once split, this edge is not referenced anymore by the vertices,
		/// it is replaced by the two or three sub-edges and intermediate splitting
		/// vertices are introduced to connect these sub-edges together.
		/// </p> </summary>
		/// <param name="splitCircle"> circle splitting the edge in several parts </param>
		/// <param name="outsideList"> list where to put parts that are outside of the split circle </param>
		/// <param name="insideList"> list where to put parts that are inside the split circle </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: void split(final Circle splitCircle, final java.util.List<Edge> outsideList, final java.util.List<Edge> insideList)
		internal virtual void Split(Circle splitCircle, IList<Edge> outsideList, IList<Edge> insideList)
		{

			// get the inside arc, synchronizing its phase with the edge itself
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double edgeStart = circle.getPhase(start.getLocation().getVector());
			double edgeStart = circle.getPhase(start.Location.Vector);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.spherical.oned.Arc arc = circle.getInsideArc(splitCircle);
			Arc arc = circle.getInsideArc(splitCircle);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double arcRelativeStart = org.apache.commons.math3.util.MathUtils.normalizeAngle(arc.getInf(), edgeStart + org.apache.commons.math3.util.FastMath.PI) - edgeStart;
			double arcRelativeStart = MathUtils.normalizeAngle(arc.Inf, edgeStart + FastMath.PI) - edgeStart;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double arcRelativeEnd = arcRelativeStart + arc.getSize();
			double arcRelativeEnd = arcRelativeStart + arc.Size;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double unwrappedEnd = arcRelativeEnd - org.apache.commons.math3.util.MathUtils.TWO_PI;
			double unwrappedEnd = arcRelativeEnd - MathUtils.TWO_PI;

			// build the sub-edges
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tolerance = circle.getTolerance();
			double tolerance = circle.Tolerance;
			Vertex previousVertex = start;
			if (unwrappedEnd >= length - tolerance)
			{

				// the edge is entirely contained inside the circle
				// we don't split anything
				insideList.Add(this);

			}
			else
			{

				// there are at least some parts of the edge that should be outside
				// (even is they are later be filtered out as being too small)
				double alreadyManagedLength = 0;
				if (unwrappedEnd >= 0)
				{
					// the start of the edge is inside the circle
					previousVertex = addSubEdge(previousVertex, new Vertex(new S2Point(circle.getPointAt(edgeStart + unwrappedEnd))), unwrappedEnd, insideList, splitCircle);
					alreadyManagedLength = unwrappedEnd;
				}

				if (arcRelativeStart >= length - tolerance)
				{
					// the edge ends while still outside of the circle
					if (unwrappedEnd >= 0)
					{
						previousVertex = addSubEdge(previousVertex, end, length - alreadyManagedLength, outsideList, splitCircle);
					}
					else
					{
						// the edge is entirely outside of the circle
						// we don't split anything
						outsideList.Add(this);
					}
				}
				else
				{
					// the edge is long enough to enter inside the circle
					previousVertex = addSubEdge(previousVertex, new Vertex(new S2Point(circle.getPointAt(edgeStart + arcRelativeStart))), arcRelativeStart - alreadyManagedLength, outsideList, splitCircle);
					alreadyManagedLength = arcRelativeStart;

					if (arcRelativeEnd >= length - tolerance)
					{
						// the edge ends while still inside of the circle
						previousVertex = addSubEdge(previousVertex, end, length - alreadyManagedLength, insideList, splitCircle);
					}
					else
					{
						// the edge is long enough to exit outside of the circle
						previousVertex = addSubEdge(previousVertex, new Vertex(new S2Point(circle.getPointAt(edgeStart + arcRelativeStart))), arcRelativeStart - alreadyManagedLength, insideList, splitCircle);
						alreadyManagedLength = arcRelativeStart;
						previousVertex = addSubEdge(previousVertex, end, length - alreadyManagedLength, outsideList, splitCircle);
					}
				}

			}

		}

		/// <summary>
		/// Add a sub-edge to a list if long enough.
		/// <p>
		/// If the length of the sub-edge to add is smaller than the <seealso cref="Circle#getTolerance()"/>
		/// tolerance of the support circle, it will be ignored.
		/// </p> </summary>
		/// <param name="subStart"> start of the sub-edge </param>
		/// <param name="subEnd"> end of the sub-edge </param>
		/// <param name="subLength"> length of the sub-edge </param>
		/// <param name="splitCircle"> circle splitting the edge in several parts </param>
		/// <param name="list"> list where to put the sub-edge </param>
		/// <returns> end vertex of the edge ({@code subEnd} if the edge was long enough and really
		/// added, {@code subStart} if the edge was too small and therefore ignored) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Vertex addSubEdge(final Vertex subStart, final Vertex subEnd, final double subLength, final java.util.List<Edge> list, final Circle splitCircle)
		private Vertex addSubEdge(Vertex subStart, Vertex subEnd, double subLength, IList<Edge> list, Circle splitCircle)
		{

			if (subLength <= circle.Tolerance)
			{
				// the edge is too short, we ignore it
				return subStart;
			}

			// really add the edge
			subEnd.bindWith(splitCircle);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Edge edge = new Edge(subStart, subEnd, subLength, circle);
			Edge edge = new Edge(subStart, subEnd, subLength, circle);
			list.Add(edge);
			return subEnd;

		}

	}

}
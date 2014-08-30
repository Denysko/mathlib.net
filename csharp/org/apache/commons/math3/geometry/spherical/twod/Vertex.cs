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


	/// <summary>
	/// Spherical polygons boundary vertex. </summary>
	/// <seealso cref= SphericalPolygonsSet#getBoundaryLoops() </seealso>
	/// <seealso cref= Edge
	/// @version $Id: Vertex.java 1561506 2014-01-26 15:31:18Z luc $
	/// @since 3.3 </seealso>
	public class Vertex
	{

		/// <summary>
		/// Vertex location. </summary>
		private readonly S2Point location;

		/// <summary>
		/// Incoming edge. </summary>
		private Edge incoming;

		/// <summary>
		/// Outgoing edge. </summary>
		private Edge outgoing;

		/// <summary>
		/// Circles bound with this vertex. </summary>
		private readonly IList<Circle> circles;

		/// <summary>
		/// Build a non-processed vertex not owned by any node yet. </summary>
		/// <param name="location"> vertex location </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: Vertex(final S2Point location)
		internal Vertex(S2Point location)
		{
			this.location = location;
			this.incoming = null;
			this.outgoing = null;
			this.circles = new List<Circle>();
		}

		/// <summary>
		/// Get Vertex location. </summary>
		/// <returns> vertex location </returns>
		public virtual S2Point Location
		{
			get
			{
				return location;
			}
		}

		/// <summary>
		/// Bind a circle considered to contain this vertex. </summary>
		/// <param name="circle"> circle to bind with this vertex </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: void bindWith(final Circle circle)
		internal virtual void bindWith(Circle circle)
		{
			circles.Add(circle);
		}

		/// <summary>
		/// Get the common circle bound with both the instance and another vertex, if any.
		/// <p>
		/// When two vertices are both bound to the same circle, this means they are
		/// already handled by node associated with this circle, so there is no need
		/// to create a cut hyperplane for them.
		/// </p> </summary>
		/// <param name="vertex"> other vertex to check instance against </param>
		/// <returns> circle bound with both the instance and another vertex, or null if the
		/// two vertices do not share a circle yet </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: Circle sharedCircleWith(final Vertex vertex)
		internal virtual Circle sharedCircleWith(Vertex vertex)
		{
			foreach (Circle circle1 in circles)
			{
				foreach (Circle circle2 in vertex.circles)
				{
					if (circle1 == circle2)
					{
						return circle1;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Set incoming edge.
		/// <p>
		/// The circle supporting the incoming edge is automatically bound
		/// with the instance.
		/// </p> </summary>
		/// <param name="incoming"> incoming edge </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: void setIncoming(final Edge incoming)
		internal virtual Edge Incoming
		{
			set
			{
				this.incoming = value;
				bindWith(value.Circle);
			}
			get
			{
				return incoming;
			}
		}


		/// <summary>
		/// Set outgoing edge.
		/// <p>
		/// The circle supporting the outgoing edge is automatically bound
		/// with the instance.
		/// </p> </summary>
		/// <param name="outgoing"> outgoing edge </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: void setOutgoing(final Edge outgoing)
		internal virtual Edge Outgoing
		{
			set
			{
				this.outgoing = value;
				bindWith(value.Circle);
			}
			get
			{
				return outgoing;
			}
		}


	}

}
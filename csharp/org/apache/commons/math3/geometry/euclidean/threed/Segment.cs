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
namespace org.apache.commons.math3.geometry.euclidean.threed
{


	/// <summary>
	/// Simple container for a two-points segment.
	/// @version $Id: Segment.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0
	/// </summary>
	public class Segment
	{

		/// <summary>
		/// Start point of the segment. </summary>
		private readonly Vector3D start;

		/// <summary>
		/// End point of the segments. </summary>
		private readonly Vector3D end;

		/// <summary>
		/// Line containing the segment. </summary>
		private readonly Line line;

		/// <summary>
		/// Build a segment. </summary>
		/// <param name="start"> start point of the segment </param>
		/// <param name="end"> end point of the segment </param>
		/// <param name="line"> line containing the segment </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Segment(final Vector3D start, final Vector3D end, final Line line)
		public Segment(Vector3D start, Vector3D end, Line line)
		{
			this.start = start;
			this.end = end;
			this.line = line;
		}

		/// <summary>
		/// Get the start point of the segment. </summary>
		/// <returns> start point of the segment </returns>
		public virtual Vector3D Start
		{
			get
			{
				return start;
			}
		}

		/// <summary>
		/// Get the end point of the segment. </summary>
		/// <returns> end point of the segment </returns>
		public virtual Vector3D End
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

	}

}
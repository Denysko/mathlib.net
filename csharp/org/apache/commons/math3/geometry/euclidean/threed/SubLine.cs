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
namespace org.apache.commons.math3.geometry.euclidean.threed
{


	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using org.apache.commons.math3.geometry;
	using Euclidean1D = org.apache.commons.math3.geometry.euclidean.oned.Euclidean1D;
	using Interval = org.apache.commons.math3.geometry.euclidean.oned.Interval;
	using IntervalsSet = org.apache.commons.math3.geometry.euclidean.oned.IntervalsSet;
	using Vector1D = org.apache.commons.math3.geometry.euclidean.oned.Vector1D;
	using Region_Location = org.apache.commons.math3.geometry.partitioning.Region_Location;

	/// <summary>
	/// This class represents a subset of a <seealso cref="Line"/>.
	/// @version $Id: SubLine.java 1555176 2014-01-03 18:07:59Z luc $
	/// @since 3.0
	/// </summary>
	public class SubLine
	{

		/// <summary>
		/// Default value for tolerance. </summary>
		private const double DEFAULT_TOLERANCE = 1.0e-10;

		/// <summary>
		/// Underlying line. </summary>
		private readonly Line line;

		/// <summary>
		/// Remaining region of the hyperplane. </summary>
		private readonly IntervalsSet remainingRegion;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="line"> underlying line </param>
		/// <param name="remainingRegion"> remaining region of the line </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SubLine(final Line line, final org.apache.commons.math3.geometry.euclidean.oned.IntervalsSet remainingRegion)
		public SubLine(Line line, IntervalsSet remainingRegion)
		{
			this.line = line;
			this.remainingRegion = remainingRegion;
		}

		/// <summary>
		/// Create a sub-line from two endpoints. </summary>
		/// <param name="start"> start point </param>
		/// <param name="end"> end point </param>
		/// <param name="tolerance"> tolerance below which points are considered identical </param>
		/// <exception cref="MathIllegalArgumentException"> if the points are equal
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SubLine(final Vector3D start, final Vector3D end, final double tolerance) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public SubLine(Vector3D start, Vector3D end, double tolerance) : this(new Line(start, end, tolerance), buildIntervalSet(start, end, tolerance))
		{
		}

		/// <summary>
		/// Create a sub-line from two endpoints. </summary>
		/// <param name="start"> start point </param>
		/// <param name="end"> end point </param>
		/// <exception cref="MathIllegalArgumentException"> if the points are equal </exception>
		/// @deprecated as of 3.3, replaced with <seealso cref="#SubLine(Vector3D, Vector3D, double)"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SubLine(final Vector3D start, final Vector3D end) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public SubLine(Vector3D start, Vector3D end) : this(start, end, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Create a sub-line from a segment. </summary>
		/// <param name="segment"> single segment forming the sub-line </param>
		/// <exception cref="MathIllegalArgumentException"> if the segment endpoints are equal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SubLine(final Segment segment) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public SubLine(Segment segment) : this(segment.Line, buildIntervalSet(segment.Start, segment.End, segment.Line.Tolerance))
		{
		}

		/// <summary>
		/// Get the endpoints of the sub-line.
		/// <p>
		/// A subline may be any arbitrary number of disjoints segments, so the endpoints
		/// are provided as a list of endpoint pairs. Each element of the list represents
		/// one segment, and each segment contains a start point at index 0 and an end point
		/// at index 1. If the sub-line is unbounded in the negative infinity direction,
		/// the start point of the first segment will have infinite coordinates. If the
		/// sub-line is unbounded in the positive infinity direction, the end point of the
		/// last segment will have infinite coordinates. So a sub-line covering the whole
		/// line will contain just one row and both elements of this row will have infinite
		/// coordinates. If the sub-line is empty, the returned list will contain 0 segments.
		/// </p> </summary>
		/// <returns> list of segments endpoints </returns>
		public virtual IList<Segment> Segments
		{
			get
			{
    
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.List<org.apache.commons.math3.geometry.euclidean.oned.Interval> list = remainingRegion.asList();
				IList<Interval> list = remainingRegion.asList();
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.List<Segment> segments = new java.util.ArrayList<Segment>(list.size());
				IList<Segment> segments = new List<Segment>(list.Count);
    
				foreach (Interval interval in list)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Vector3D start = line.toSpace((org.apache.commons.math3.geometry.Point<org.apache.commons.math3.geometry.euclidean.oned.Euclidean1D>) new org.apache.commons.math3.geometry.euclidean.oned.Vector1D(interval.getInf()));
					Vector3D start = line.toSpace((Point<Euclidean1D>) new Vector1D(interval.Inf));
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Vector3D end = line.toSpace((org.apache.commons.math3.geometry.Point<org.apache.commons.math3.geometry.euclidean.oned.Euclidean1D>) new org.apache.commons.math3.geometry.euclidean.oned.Vector1D(interval.getSup()));
					Vector3D end = line.toSpace((Point<Euclidean1D>) new Vector1D(interval.Sup));
					segments.Add(new Segment(start, end, line));
				}
    
				return segments;
    
			}
		}

		/// <summary>
		/// Get the intersection of the instance and another sub-line.
		/// <p>
		/// This method is related to the {@link Line#intersection(Line)
		/// intersection} method in the <seealso cref="Line Line"/> class, but in addition
		/// to compute the point along infinite lines, it also checks the point
		/// lies on both sub-line ranges.
		/// </p> </summary>
		/// <param name="subLine"> other sub-line which may intersect instance </param>
		/// <param name="includeEndPoints"> if true, endpoints are considered to belong to
		/// instance (i.e. they are closed sets) and may be returned, otherwise endpoints
		/// are considered to not belong to instance (i.e. they are open sets) and intersection
		/// occurring on endpoints lead to null being returned </param>
		/// <returns> the intersection point if there is one, null if the sub-lines don't intersect </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector3D intersection(final SubLine subLine, final boolean includeEndPoints)
		public virtual Vector3D intersection(SubLine subLine, bool includeEndPoints)
		{

			// compute the intersection on infinite line
			Vector3D v1D = line.intersection(subLine.line);
			if (v1D == null)
			{
				return null;
			}

			// check location of point with respect to first sub-line
			Region_Location loc1 = remainingRegion.checkPoint((Point<Euclidean1D>) line.toSubSpace((Point<Euclidean3D>) v1D));

			// check location of point with respect to second sub-line
			Region_Location loc2 = subLine.remainingRegion.checkPoint((Point<Euclidean1D>) subLine.line.toSubSpace((Point<Euclidean3D>) v1D));

			if (includeEndPoints)
			{
				return ((loc1 != Region_Location.OUTSIDE) && (loc2 != Region_Location.OUTSIDE)) ? v1D : null;
			}
			else
			{
				return ((loc1 == Region_Location.INSIDE) && (loc2 == Region_Location.INSIDE)) ? v1D : null;
			}

		}

		/// <summary>
		/// Build an interval set from two points. </summary>
		/// <param name="start"> start point </param>
		/// <param name="end"> end point </param>
		/// <returns> an interval set </returns>
		/// <param name="tolerance"> tolerance below which points are considered identical </param>
		/// <exception cref="MathIllegalArgumentException"> if the points are equal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static org.apache.commons.math3.geometry.euclidean.oned.IntervalsSet buildIntervalSet(final Vector3D start, final Vector3D end, final double tolerance) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private static IntervalsSet buildIntervalSet(Vector3D start, Vector3D end, double tolerance)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Line line = new Line(start, end, tolerance);
			Line line = new Line(start, end, tolerance);
			return new IntervalsSet(line.toSubSpace((Point<Euclidean3D>) start).X, line.toSubSpace((Point<Euclidean3D>) end).X, tolerance);
		}

	}

}
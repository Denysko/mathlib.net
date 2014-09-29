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
namespace mathlib.geometry.euclidean.twod
{


	using mathlib.geometry;
	using Euclidean1D = mathlib.geometry.euclidean.oned.Euclidean1D;
	using Interval = mathlib.geometry.euclidean.oned.Interval;
	using IntervalsSet = mathlib.geometry.euclidean.oned.IntervalsSet;
	using OrientedPoint = mathlib.geometry.euclidean.oned.OrientedPoint;
	using Vector1D = mathlib.geometry.euclidean.oned.Vector1D;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using Region_Location = mathlib.geometry.partitioning.Region_Location;
	using Side = mathlib.geometry.partitioning.Side;
	using mathlib.geometry.partitioning;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// This class represents a sub-hyperplane for <seealso cref="Line"/>.
	/// @version $Id: SubLine.java 1555176 2014-01-03 18:07:59Z luc $
	/// @since 3.0
	/// </summary>
	public class SubLine : AbstractSubHyperplane<Euclidean2D, Euclidean1D>
	{

		/// <summary>
		/// Default value for tolerance. </summary>
		private const double DEFAULT_TOLERANCE = 1.0e-10;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="hyperplane"> underlying hyperplane </param>
		/// <param name="remainingRegion"> remaining region of the hyperplane </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SubLine(final mathlib.geometry.partitioning.Hyperplane<Euclidean2D> hyperplane, final mathlib.geometry.partitioning.Region<mathlib.geometry.euclidean.oned.Euclidean1D> remainingRegion)
		public SubLine(Hyperplane<Euclidean2D> hyperplane, Region<Euclidean1D> remainingRegion) : base(hyperplane, remainingRegion)
		{
		}

		/// <summary>
		/// Create a sub-line from two endpoints. </summary>
		/// <param name="start"> start point </param>
		/// <param name="end"> end point </param>
		/// <param name="tolerance"> tolerance below which points are considered identical
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SubLine(final Vector2D start, final Vector2D end, final double tolerance)
		public SubLine(Vector2D start, Vector2D end, double tolerance) : base(new Line(start, end, tolerance), buildIntervalSet(start, end, tolerance))
		{
		}

		/// <summary>
		/// Create a sub-line from two endpoints. </summary>
		/// <param name="start"> start point </param>
		/// <param name="end"> end point </param>
		/// @deprecated as of 3.3, replaced with <seealso cref="#SubLine(Vector2D, Vector2D, double)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3, replaced with <seealso cref="#SubLine(Vector2D, Vector2D, double)"/>") public SubLine(final Vector2D start, final Vector2D end)
		[Obsolete("as of 3.3, replaced with <seealso cref="#SubLine(Vector2D, Vector2D, double)"/>")]
		public SubLine(Vector2D start, Vector2D end) : this(start, end, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Create a sub-line from a segment. </summary>
		/// <param name="segment"> single segment forming the sub-line </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SubLine(final Segment segment)
		public SubLine(Segment segment) : base(segment.Line, buildIntervalSet(segment.Start, segment.End, segment.Line.Tolerance))
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
	//ORIGINAL LINE: final Line line = (Line) getHyperplane();
				Line line = (Line) Hyperplane;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.List<mathlib.geometry.euclidean.oned.Interval> list = ((mathlib.geometry.euclidean.oned.IntervalsSet) getRemainingRegion()).asList();
				IList<Interval> list = ((IntervalsSet) RemainingRegion).asList();
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.List<Segment> segments = new java.util.ArrayList<Segment>(list.size());
				IList<Segment> segments = new List<Segment>(list.Count);
    
				foreach (Interval interval in list)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Vector2D start = line.toSpace((mathlib.geometry.Point<mathlib.geometry.euclidean.oned.Euclidean1D>) new mathlib.geometry.euclidean.oned.Vector1D(interval.getInf()));
					Vector2D start = line.toSpace((Point<Euclidean1D>) new Vector1D(interval.Inf));
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Vector2D end = line.toSpace((mathlib.geometry.Point<mathlib.geometry.euclidean.oned.Euclidean1D>) new mathlib.geometry.euclidean.oned.Vector1D(interval.getSup()));
					Vector2D end = line.toSpace((Point<Euclidean1D>) new Vector1D(interval.Sup));
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
//ORIGINAL LINE: public Vector2D intersection(final SubLine subLine, final boolean includeEndPoints)
		public virtual Vector2D intersection(SubLine subLine, bool includeEndPoints)
		{

			// retrieve the underlying lines
			Line line1 = (Line) Hyperplane;
			Line line2 = (Line) subLine.Hyperplane;

			// compute the intersection on infinite line
			Vector2D v2D = line1.intersection(line2);
			if (v2D == null)
			{
				return null;
			}

			// check location of point with respect to first sub-line
			Region_Location loc1 = RemainingRegion.checkPoint(line1.toSubSpace((Point<Euclidean2D>) v2D));

			// check location of point with respect to second sub-line
			Region_Location loc2 = subLine.RemainingRegion.checkPoint(line2.toSubSpace((Point<Euclidean2D>) v2D));

			if (includeEndPoints)
			{
				return ((loc1 != Region_Location.OUTSIDE) && (loc2 != Region_Location.OUTSIDE)) ? v2D : null;
			}
			else
			{
				return ((loc1 == Region_Location.INSIDE) && (loc2 == Region_Location.INSIDE)) ? v2D : null;
			}

		}

		/// <summary>
		/// Build an interval set from two points. </summary>
		/// <param name="start"> start point </param>
		/// <param name="end"> end point </param>
		/// <param name="tolerance"> tolerance below which points are considered identical </param>
		/// <returns> an interval set </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static mathlib.geometry.euclidean.oned.IntervalsSet buildIntervalSet(final Vector2D start, final Vector2D end, final double tolerance)
		private static IntervalsSet buildIntervalSet(Vector2D start, Vector2D end, double tolerance)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Line line = new Line(start, end, tolerance);
			Line line = new Line(start, end, tolerance);
			return new IntervalsSet(line.toSubSpace((Point<Euclidean2D>) start).X, line.toSubSpace((Point<Euclidean2D>) end).X, tolerance);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected mathlib.geometry.partitioning.AbstractSubHyperplane<Euclidean2D, mathlib.geometry.euclidean.oned.Euclidean1D> buildNew(final mathlib.geometry.partitioning.Hyperplane<Euclidean2D> hyperplane, final mathlib.geometry.partitioning.Region<mathlib.geometry.euclidean.oned.Euclidean1D> remainingRegion)
		protected internal override AbstractSubHyperplane<Euclidean2D, Euclidean1D> buildNew(Hyperplane<Euclidean2D> hyperplane, Region<Euclidean1D> remainingRegion)
		{
			return new SubLine(hyperplane, remainingRegion);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public mathlib.geometry.partitioning.Side side(final mathlib.geometry.partitioning.Hyperplane<Euclidean2D> hyperplane)
		public override Side side(Hyperplane<Euclidean2D> hyperplane)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Line thisLine = (Line) getHyperplane();
			Line thisLine = (Line) Hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Line otherLine = (Line) hyperplane;
			Line otherLine = (Line) hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D crossing = thisLine.intersection(otherLine);
			Vector2D crossing = thisLine.intersection(otherLine);

			if (crossing == null)
			{
				// the lines are parallel,
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double global = otherLine.getOffset(thisLine);
				double global = otherLine.getOffset(thisLine);
				return (global < -1.0e-10) ? Side.MINUS : ((global > 1.0e-10) ? Side.PLUS : Side.HYPER);
			}

			// the lines do intersect
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean direct = mathlib.util.FastMath.sin(thisLine.getAngle() - otherLine.getAngle()) < 0;
			bool direct = FastMath.sin(thisLine.Angle - otherLine.Angle) < 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.euclidean.oned.Vector1D x = thisLine.toSubSpace((mathlib.geometry.Point<Euclidean2D>) crossing);
			Vector1D x = thisLine.toSubSpace((Point<Euclidean2D>) crossing);
			return RemainingRegion.side(new OrientedPoint(x, direct, thisLine.Tolerance));

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public mathlib.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Euclidean2D> split(final mathlib.geometry.partitioning.Hyperplane<Euclidean2D> hyperplane)
		public override mathlib.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Euclidean2D> Split(Hyperplane<Euclidean2D> hyperplane)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Line thisLine = (Line) getHyperplane();
			Line thisLine = (Line) Hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Line otherLine = (Line) hyperplane;
			Line otherLine = (Line) hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D crossing = thisLine.intersection(otherLine);
			Vector2D crossing = thisLine.intersection(otherLine);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tolerance = thisLine.getTolerance();
			double tolerance = thisLine.Tolerance;

			if (crossing == null)
			{
				// the lines are parallel
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double global = otherLine.getOffset(thisLine);
				double global = otherLine.getOffset(thisLine);
				return (global < -1.0e-10) ? new mathlib.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Euclidean2D>(null, this) : new mathlib.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Euclidean2D>(this, null);
			}

			// the lines do intersect
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean direct = mathlib.util.FastMath.sin(thisLine.getAngle() - otherLine.getAngle()) < 0;
			bool direct = FastMath.sin(thisLine.Angle - otherLine.Angle) < 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.euclidean.oned.Vector1D x = thisLine.toSubSpace((mathlib.geometry.Point<Euclidean2D>) crossing);
			Vector1D x = thisLine.toSubSpace((Point<Euclidean2D>) crossing);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.SubHyperplane<mathlib.geometry.euclidean.oned.Euclidean1D> subPlus = new mathlib.geometry.euclidean.oned.OrientedPoint(x, !direct, tolerance).wholeHyperplane();
			SubHyperplane<Euclidean1D> subPlus = (new OrientedPoint(x, !direct, tolerance)).wholeHyperplane();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.SubHyperplane<mathlib.geometry.euclidean.oned.Euclidean1D> subMinus = new mathlib.geometry.euclidean.oned.OrientedPoint(x, direct, tolerance).wholeHyperplane();
			SubHyperplane<Euclidean1D> subMinus = (new OrientedPoint(x, direct, tolerance)).wholeHyperplane();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.oned.Euclidean1D> splitTree = getRemainingRegion().getTree(false).split(subMinus);
			BSPTree<Euclidean1D> splitTree = RemainingRegion.getTree(false).Split(subMinus);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.oned.Euclidean1D> plusTree = getRemainingRegion().isEmpty(splitTree.getPlus()) ? new mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.oned.Euclidean1D>(Boolean.FALSE) : new mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.oned.Euclidean1D>(subPlus, new mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.oned.Euclidean1D>(Boolean.FALSE), splitTree.getPlus(), null);
			BSPTree<Euclidean1D> plusTree = RemainingRegion.isEmpty(splitTree.Plus) ? new BSPTree<Euclidean1D>(false) : new BSPTree<Euclidean1D>(subPlus, new BSPTree<Euclidean1D>(false), splitTree.Plus, null);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.oned.Euclidean1D> minusTree = getRemainingRegion().isEmpty(splitTree.getMinus()) ? new mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.oned.Euclidean1D>(Boolean.FALSE) : new mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.oned.Euclidean1D>(subMinus, new mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.oned.Euclidean1D>(Boolean.FALSE), splitTree.getMinus(), null);
			BSPTree<Euclidean1D> minusTree = RemainingRegion.isEmpty(splitTree.Minus) ? new BSPTree<Euclidean1D>(false) : new BSPTree<Euclidean1D>(subMinus, new BSPTree<Euclidean1D>(false), splitTree.Minus, null);

			return new mathlib.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Euclidean2D>(new SubLine(thisLine.copySelf(), new IntervalsSet(plusTree, tolerance)), new SubLine(thisLine.copySelf(), new IntervalsSet(minusTree, tolerance)));

		}

	}

}
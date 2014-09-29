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
namespace mathlib.geometry.euclidean.threed
{

	using mathlib.geometry;
	using Euclidean1D = mathlib.geometry.euclidean.oned.Euclidean1D;
	using Vector1D = mathlib.geometry.euclidean.oned.Vector1D;
	using Euclidean2D = mathlib.geometry.euclidean.twod.Euclidean2D;
	using Vector2D = mathlib.geometry.euclidean.twod.Vector2D;
	using PolygonsSet = mathlib.geometry.euclidean.twod.PolygonsSet;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using Side = mathlib.geometry.partitioning.Side;
	using mathlib.geometry.partitioning;

	/// <summary>
	/// This class represents a sub-hyperplane for <seealso cref="Plane"/>.
	/// @version $Id: SubPlane.java 1555174 2014-01-03 18:06:20Z luc $
	/// @since 3.0
	/// </summary>
	public class SubPlane : AbstractSubHyperplane<Euclidean3D, Euclidean2D>
	{

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="hyperplane"> underlying hyperplane </param>
		/// <param name="remainingRegion"> remaining region of the hyperplane </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SubPlane(final mathlib.geometry.partitioning.Hyperplane<Euclidean3D> hyperplane, final mathlib.geometry.partitioning.Region<mathlib.geometry.euclidean.twod.Euclidean2D> remainingRegion)
		public SubPlane(Hyperplane<Euclidean3D> hyperplane, Region<Euclidean2D> remainingRegion) : base(hyperplane, remainingRegion)
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected mathlib.geometry.partitioning.AbstractSubHyperplane<Euclidean3D, mathlib.geometry.euclidean.twod.Euclidean2D> buildNew(final mathlib.geometry.partitioning.Hyperplane<Euclidean3D> hyperplane, final mathlib.geometry.partitioning.Region<mathlib.geometry.euclidean.twod.Euclidean2D> remainingRegion)
		protected internal override AbstractSubHyperplane<Euclidean3D, Euclidean2D> buildNew(Hyperplane<Euclidean3D> hyperplane, Region<Euclidean2D> remainingRegion)
		{
			return new SubPlane(hyperplane, remainingRegion);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override Side side(Hyperplane<Euclidean3D> hyperplane)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane otherPlane = (Plane) hyperplane;
			Plane otherPlane = (Plane) hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane thisPlane = (Plane) getHyperplane();
			Plane thisPlane = (Plane) Hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Line inter = otherPlane.intersection(thisPlane);
			Line inter = otherPlane.intersection(thisPlane);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tolerance = thisPlane.getTolerance();
			double tolerance = thisPlane.Tolerance;

			if (inter == null)
			{
				// the hyperplanes are parallel,
				// any point can be used to check their relative position
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double global = otherPlane.getOffset(thisPlane);
				double global = otherPlane.getOffset(thisPlane);
				return (global < -1.0e-10) ? Side.MINUS : ((global > 1.0e-10) ? Side.PLUS : Side.HYPER);
			}

			// create a 2D line in the otherPlane canonical 2D frame such that:
			//   - the line is the crossing line of the two planes in 3D
			//   - the line splits the otherPlane in two half planes with an
			//     orientation consistent with the orientation of the instance
			//     (i.e. the 3D half space on the plus side (resp. minus side)
			//      of the instance contains the 2D half plane on the plus side
			//      (resp. minus side) of the 2D line
			Vector2D p = thisPlane.toSubSpace((Point<Euclidean3D>) inter.toSpace((Point<Euclidean1D>) Vector1D.ZERO));
			Vector2D q = thisPlane.toSubSpace((Point<Euclidean3D>) inter.toSpace((Point<Euclidean1D>) Vector1D.ONE));
			Vector3D crossP = Vector3D.crossProduct(inter.Direction, thisPlane.Normal);
			if (crossP.dotProduct(otherPlane.Normal) < 0)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.euclidean.twod.Vector2D tmp = p;
				Vector2D tmp = p;
				p = q;
				q = tmp;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.euclidean.twod.Line line2D = new mathlib.geometry.euclidean.twod.Line(p, q, tolerance);
			mathlib.geometry.euclidean.twod.Line line2D = new mathlib.geometry.euclidean.twod.Line(p, q, tolerance);

			// check the side on the 2D plane
			return RemainingRegion.side(line2D);

		}

		/// <summary>
		/// Split the instance in two parts by an hyperplane. </summary>
		/// <param name="hyperplane"> splitting hyperplane </param>
		/// <returns> an object containing both the part of the instance
		/// on the plus side of the instance and the part of the
		/// instance on the minus side of the instance </returns>
		public override mathlib.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Euclidean3D> Split(Hyperplane<Euclidean3D> hyperplane)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane otherPlane = (Plane) hyperplane;
			Plane otherPlane = (Plane) hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane thisPlane = (Plane) getHyperplane();
			Plane thisPlane = (Plane) Hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Line inter = otherPlane.intersection(thisPlane);
			Line inter = otherPlane.intersection(thisPlane);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tolerance = thisPlane.getTolerance();
			double tolerance = thisPlane.Tolerance;

			if (inter == null)
			{
				// the hyperplanes are parallel
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double global = otherPlane.getOffset(thisPlane);
				double global = otherPlane.getOffset(thisPlane);
				return (global < -1.0e-10) ? new mathlib.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Euclidean3D>(null, this) : new mathlib.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Euclidean3D>(this, null);
			}

			// the hyperplanes do intersect
			Vector2D p = thisPlane.toSubSpace((Point<Euclidean3D>) inter.toSpace((Point<Euclidean1D>) Vector1D.ZERO));
			Vector2D q = thisPlane.toSubSpace((Point<Euclidean3D>) inter.toSpace((Point<Euclidean1D>) Vector1D.ONE));
			Vector3D crossP = Vector3D.crossProduct(inter.Direction, thisPlane.Normal);
			if (crossP.dotProduct(otherPlane.Normal) < 0)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.euclidean.twod.Vector2D tmp = p;
				Vector2D tmp = p;
				p = q;
				q = tmp;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.SubHyperplane<mathlib.geometry.euclidean.twod.Euclidean2D> l2DMinus = new mathlib.geometry.euclidean.twod.Line(p, q, tolerance).wholeHyperplane();
			SubHyperplane<Euclidean2D> l2DMinus = (new mathlib.geometry.euclidean.twod.Line(p, q, tolerance)).wholeHyperplane();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.SubHyperplane<mathlib.geometry.euclidean.twod.Euclidean2D> l2DPlus = new mathlib.geometry.euclidean.twod.Line(q, p, tolerance).wholeHyperplane();
			SubHyperplane<Euclidean2D> l2DPlus = (new mathlib.geometry.euclidean.twod.Line(q, p, tolerance)).wholeHyperplane();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.twod.Euclidean2D> splitTree = getRemainingRegion().getTree(false).split(l2DMinus);
			BSPTree<Euclidean2D> splitTree = RemainingRegion.getTree(false).Split(l2DMinus);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.twod.Euclidean2D> plusTree = getRemainingRegion().isEmpty(splitTree.getPlus()) ? new mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.twod.Euclidean2D>(Boolean.FALSE) : new mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.twod.Euclidean2D>(l2DPlus, new mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.twod.Euclidean2D>(Boolean.FALSE), splitTree.getPlus(), null);
			BSPTree<Euclidean2D> plusTree = RemainingRegion.isEmpty(splitTree.Plus) ? new BSPTree<Euclidean2D>(false) : new BSPTree<Euclidean2D>(l2DPlus, new BSPTree<Euclidean2D>(false), splitTree.Plus, null);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.twod.Euclidean2D> minusTree = getRemainingRegion().isEmpty(splitTree.getMinus()) ? new mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.twod.Euclidean2D>(Boolean.FALSE) : new mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.twod.Euclidean2D>(l2DMinus, new mathlib.geometry.partitioning.BSPTree<mathlib.geometry.euclidean.twod.Euclidean2D>(Boolean.FALSE), splitTree.getMinus(), null);
			BSPTree<Euclidean2D> minusTree = RemainingRegion.isEmpty(splitTree.Minus) ? new BSPTree<Euclidean2D>(false) : new BSPTree<Euclidean2D>(l2DMinus, new BSPTree<Euclidean2D>(false), splitTree.Minus, null);

			return new mathlib.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Euclidean3D>(new SubPlane(thisPlane.copySelf(), new PolygonsSet(plusTree, tolerance)), new SubPlane(thisPlane.copySelf(), new PolygonsSet(minusTree, tolerance)));

		}

	}

}
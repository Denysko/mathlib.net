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
namespace mathlib.geometry.spherical.twod
{

	using Vector3D = mathlib.geometry.euclidean.threed.Vector3D;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using Side = mathlib.geometry.partitioning.Side;
	using Arc = mathlib.geometry.spherical.oned.Arc;
	using ArcsSet = mathlib.geometry.spherical.oned.ArcsSet;
	using Sphere1D = mathlib.geometry.spherical.oned.Sphere1D;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// This class represents a sub-hyperplane for <seealso cref="Circle"/>.
	/// @version $Id: SubCircle.java 1555088 2014-01-03 13:47:35Z luc $
	/// @since 3.3
	/// </summary>
	public class SubCircle : AbstractSubHyperplane<Sphere2D, Sphere1D>
	{

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="hyperplane"> underlying hyperplane </param>
		/// <param name="remainingRegion"> remaining region of the hyperplane </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SubCircle(final mathlib.geometry.partitioning.Hyperplane<Sphere2D> hyperplane, final mathlib.geometry.partitioning.Region<mathlib.geometry.spherical.oned.Sphere1D> remainingRegion)
		public SubCircle(Hyperplane<Sphere2D> hyperplane, Region<Sphere1D> remainingRegion) : base(hyperplane, remainingRegion)
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected mathlib.geometry.partitioning.AbstractSubHyperplane<Sphere2D, mathlib.geometry.spherical.oned.Sphere1D> buildNew(final mathlib.geometry.partitioning.Hyperplane<Sphere2D> hyperplane, final mathlib.geometry.partitioning.Region<mathlib.geometry.spherical.oned.Sphere1D> remainingRegion)
		protected internal override AbstractSubHyperplane<Sphere2D, Sphere1D> buildNew(Hyperplane<Sphere2D> hyperplane, Region<Sphere1D> remainingRegion)
		{
			return new SubCircle(hyperplane, remainingRegion);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public mathlib.geometry.partitioning.Side side(final mathlib.geometry.partitioning.Hyperplane<Sphere2D> hyperplane)
		public override Side side(Hyperplane<Sphere2D> hyperplane)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Circle thisCircle = (Circle) getHyperplane();
			Circle thisCircle = (Circle) Hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Circle otherCircle = (Circle) hyperplane;
			Circle otherCircle = (Circle) hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double angle = mathlib.geometry.euclidean.threed.Vector3D.angle(thisCircle.getPole(), otherCircle.getPole());
			double angle = Vector3D.angle(thisCircle.Pole, otherCircle.Pole);

			if (angle < thisCircle.Tolerance || angle > FastMath.PI - thisCircle.Tolerance)
			{
				// the two circles are aligned or opposite
				return Side.HYPER;
			}
			else
			{
				// the two circles intersect each other
				return ((ArcsSet) RemainingRegion).side(thisCircle.getInsideArc(otherCircle));
			}

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public mathlib.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Sphere2D> split(final mathlib.geometry.partitioning.Hyperplane<Sphere2D> hyperplane)
		public override mathlib.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Sphere2D> Split(Hyperplane<Sphere2D> hyperplane)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Circle thisCircle = (Circle) getHyperplane();
			Circle thisCircle = (Circle) Hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Circle otherCircle = (Circle) hyperplane;
			Circle otherCircle = (Circle) hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double angle = mathlib.geometry.euclidean.threed.Vector3D.angle(thisCircle.getPole(), otherCircle.getPole());
			double angle = Vector3D.angle(thisCircle.Pole, otherCircle.Pole);

			if (angle < thisCircle.Tolerance)
			{
				// the two circles are aligned
				return new mathlib.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Sphere2D>(null, this);
			}
			else if (angle > FastMath.PI - thisCircle.Tolerance)
			{
				// the two circles are opposite
				return new mathlib.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Sphere2D>(this, null);
			}
			else
			{
				// the two circles intersect each other
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.spherical.oned.Arc arc = thisCircle.getInsideArc(otherCircle);
				Arc arc = thisCircle.getInsideArc(otherCircle);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.spherical.oned.ArcsSet.Split split = ((mathlib.geometry.spherical.oned.ArcsSet) getRemainingRegion()).split(arc);
				ArcsSet.Split split = ((ArcsSet) RemainingRegion).Split(arc);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.spherical.oned.ArcsSet plus = split.getPlus();
				ArcsSet plus = split.Plus;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.spherical.oned.ArcsSet minus = split.getMinus();
				ArcsSet minus = split.Minus;
				return new mathlib.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Sphere2D>(plus == null ? null : new SubCircle(thisCircle.copySelf(), plus), minus == null ? null : new SubCircle(thisCircle.copySelf(), minus));
			}

		}

	}

}
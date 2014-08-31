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
namespace org.apache.commons.math3.geometry.euclidean.oned
{

	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using Side = org.apache.commons.math3.geometry.partitioning.Side;

	/// <summary>
	/// This class represents sub-hyperplane for <seealso cref="OrientedPoint"/>.
	/// <p>An hyperplane in 1D is a simple point, its orientation being a
	/// boolean.</p>
	/// <p>Instances of this class are guaranteed to be immutable.</p>
	/// @version $Id: SubOrientedPoint.java 1563714 2014-02-02 20:55:14Z tn $
	/// @since 3.0
	/// </summary>
	public class SubOrientedPoint : AbstractSubHyperplane<Euclidean1D, Euclidean1D>
	{

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="hyperplane"> underlying hyperplane </param>
		/// <param name="remainingRegion"> remaining region of the hyperplane </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SubOrientedPoint(final org.apache.commons.math3.geometry.partitioning.Hyperplane<Euclidean1D> hyperplane, final org.apache.commons.math3.geometry.partitioning.Region<Euclidean1D> remainingRegion)
		public SubOrientedPoint(Hyperplane<Euclidean1D> hyperplane, Region<Euclidean1D> remainingRegion) : base(hyperplane, remainingRegion)
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double Size
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool Empty
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected org.apache.commons.math3.geometry.partitioning.AbstractSubHyperplane<Euclidean1D, Euclidean1D> buildNew(final org.apache.commons.math3.geometry.partitioning.Hyperplane<Euclidean1D> hyperplane, final org.apache.commons.math3.geometry.partitioning.Region<Euclidean1D> remainingRegion)
		protected internal override AbstractSubHyperplane<Euclidean1D, Euclidean1D> buildNew(Hyperplane<Euclidean1D> hyperplane, Region<Euclidean1D> remainingRegion)
		{
			return new SubOrientedPoint(hyperplane, remainingRegion);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public org.apache.commons.math3.geometry.partitioning.Side side(final org.apache.commons.math3.geometry.partitioning.Hyperplane<Euclidean1D> hyperplane)
		public override Side side(Hyperplane<Euclidean1D> hyperplane)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double global = hyperplane.getOffset(((OrientedPoint) getHyperplane()).getLocation());
			double global = hyperplane.getOffset(((OrientedPoint) Hyperplane).Location);
			return (global < -1.0e-10) ? Side.MINUS : ((global > 1.0e-10) ? Side.PLUS : Side.HYPER);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public org.apache.commons.math3.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Euclidean1D> split(final org.apache.commons.math3.geometry.partitioning.Hyperplane<Euclidean1D> hyperplane)
		public override org.apache.commons.math3.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Euclidean1D> Split(Hyperplane<Euclidean1D> hyperplane)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double global = hyperplane.getOffset(((OrientedPoint) getHyperplane()).getLocation());
			double global = hyperplane.getOffset(((OrientedPoint) Hyperplane).Location);
			return (global < -1.0e-10) ? new org.apache.commons.math3.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Euclidean1D>(null, this) : new org.apache.commons.math3.geometry.partitioning.SubHyperplane_SplitSubHyperplane<Euclidean1D>(this, null);
		}

	}

}
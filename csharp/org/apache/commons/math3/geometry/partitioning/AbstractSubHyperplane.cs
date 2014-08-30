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
namespace org.apache.commons.math3.geometry.partitioning
{

	/// <summary>
	/// This class implements the dimension-independent parts of <seealso cref="SubHyperplane"/>.
	/// 
	/// <p>sub-hyperplanes are obtained when parts of an {@link
	/// Hyperplane hyperplane} are chopped off by other hyperplanes that
	/// intersect it. The remaining part is a convex region. Such objects
	/// appear in <seealso cref="BSPTree BSP trees"/> as the intersection of a cut
	/// hyperplane with the convex region which it splits, the chopping
	/// hyperplanes are the cut hyperplanes closer to the tree root.</p>
	/// </summary>
	/// @param <S> Type of the embedding space. </param>
	/// @param <T> Type of the embedded sub-space.
	/// 
	/// @version $Id: AbstractSubHyperplane.java 1555050 2014-01-03 11:22:04Z luc $
	/// @since 3.0 </param>
	public abstract class AbstractSubHyperplane<S, T> : SubHyperplane<S> where S : org.apache.commons.math3.geometry.Space where T : org.apache.commons.math3.geometry.Space
	{

		/// <summary>
		/// Underlying hyperplane. </summary>
		private readonly Hyperplane<S> hyperplane;

		/// <summary>
		/// Remaining region of the hyperplane. </summary>
		private readonly Region<T> remainingRegion;

		/// <summary>
		/// Build a sub-hyperplane from an hyperplane and a region. </summary>
		/// <param name="hyperplane"> underlying hyperplane </param>
		/// <param name="remainingRegion"> remaining region of the hyperplane </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractSubHyperplane(final Hyperplane<S> hyperplane, final Region<T> remainingRegion)
		protected internal AbstractSubHyperplane(Hyperplane<S> hyperplane, Region<T> remainingRegion)
		{
			this.hyperplane = hyperplane;
			this.remainingRegion = remainingRegion;
		}

		/// <summary>
		/// Build a sub-hyperplane from an hyperplane and a region. </summary>
		/// <param name="hyper"> underlying hyperplane </param>
		/// <param name="remaining"> remaining region of the hyperplane </param>
		/// <returns> a new sub-hyperplane </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected abstract AbstractSubHyperplane<S, T> buildNew(final Hyperplane<S> hyper, final Region<T> remaining);
		protected internal abstract AbstractSubHyperplane<S, T> buildNew(Hyperplane<S> hyper, Region<T> remaining);

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual AbstractSubHyperplane<S, T> copySelf()
		{
			return buildNew(hyperplane.copySelf(), remainingRegion);
		}

		/// <summary>
		/// Get the underlying hyperplane. </summary>
		/// <returns> underlying hyperplane </returns>
		public virtual Hyperplane<S> Hyperplane
		{
			get
			{
				return hyperplane;
			}
		}

		/// <summary>
		/// Get the remaining region of the hyperplane.
		/// <p>The returned region is expressed in the canonical hyperplane
		/// frame and has the hyperplane dimension. For example a chopped
		/// hyperplane in the 3D euclidean is a 2D plane and the
		/// corresponding region is a convex 2D polygon.</p> </summary>
		/// <returns> remaining region of the hyperplane </returns>
		public virtual Region<T> RemainingRegion
		{
			get
			{
				return remainingRegion;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double Size
		{
			get
			{
				return remainingRegion.Size;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public AbstractSubHyperplane<S, T> reunite(final SubHyperplane<S> other)
		public virtual AbstractSubHyperplane<S, T> reunite(SubHyperplane<S> other)
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") AbstractSubHyperplane<S, T> o = (AbstractSubHyperplane<S, T>) other;
			AbstractSubHyperplane<S, T> o = (AbstractSubHyperplane<S, T>) other;
			return buildNew(hyperplane, (new RegionFactory<T>()).union(remainingRegion, o.remainingRegion));
		}

		/// <summary>
		/// Apply a transform to the instance.
		/// <p>The instance must be a (D-1)-dimension sub-hyperplane with
		/// respect to the transform <em>not</em> a (D-2)-dimension
		/// sub-hyperplane the transform knows how to transform by
		/// itself. The transform will consist in transforming first the
		/// hyperplane and then the all region using the various methods
		/// provided by the transform.</p> </summary>
		/// <param name="transform"> D-dimension transform to apply </param>
		/// <returns> the transformed instance </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public AbstractSubHyperplane<S, T> applyTransform(final Transform<S, T> transform)
		public virtual AbstractSubHyperplane<S, T> applyTransform(Transform<S, T> transform)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Hyperplane<S> tHyperplane = transform.apply(hyperplane);
			Hyperplane<S> tHyperplane = transform.apply(hyperplane);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BSPTree<T> tTree = recurseTransform(remainingRegion.getTree(false), tHyperplane, transform);
			BSPTree<T> tTree = recurseTransform(remainingRegion.getTree(false), tHyperplane, transform);
			return buildNew(tHyperplane, remainingRegion.buildNew(tTree));
		}

		/// <summary>
		/// Recursively transform a BSP-tree from a sub-hyperplane. </summary>
		/// <param name="node"> current BSP tree node </param>
		/// <param name="transformed"> image of the instance hyperplane by the transform </param>
		/// <param name="transform"> transform to apply </param>
		/// <returns> a new tree </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private BSPTree<T> recurseTransform(final BSPTree<T> node, final Hyperplane<S> transformed, final Transform<S, T> transform)
		private BSPTree<T> recurseTransform(BSPTree<T> node, Hyperplane<S> transformed, Transform<S, T> transform)
		{
			if (node.Cut == null)
			{
				return new BSPTree<T>(node.Attribute);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") BoundaryAttribute<T> attribute = (BoundaryAttribute<T>) node.getAttribute();
			BoundaryAttribute<T> attribute = (BoundaryAttribute<T>) node.Attribute;
			if (attribute != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SubHyperplane<T> tPO = (attribute.getPlusOutside() == null) ? null : transform.apply(attribute.getPlusOutside(), hyperplane, transformed);
				SubHyperplane<T> tPO = (attribute.PlusOutside == null) ? null : transform.apply(attribute.PlusOutside, hyperplane, transformed);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SubHyperplane<T> tPI = (attribute.getPlusInside() == null) ? null : transform.apply(attribute.getPlusInside(), hyperplane, transformed);
				SubHyperplane<T> tPI = (attribute.PlusInside == null) ? null : transform.apply(attribute.PlusInside, hyperplane, transformed);
				attribute = new BoundaryAttribute<T>(tPO, tPI);
			}

			return new BSPTree<T>(transform.apply(node.Cut, hyperplane, transformed), recurseTransform(node.Plus, transformed, transform), recurseTransform(node.Minus, transformed, transform), attribute);

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public abstract Side side(Hyperplane<S> hyper);

		/// <summary>
		/// {@inheritDoc} </summary>
		public abstract SubHyperplane_SplitSubHyperplane<S> Split(Hyperplane<S> hyper);

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual bool Empty
		{
			get
			{
				return remainingRegion.Empty;
			}
		}

	}

}
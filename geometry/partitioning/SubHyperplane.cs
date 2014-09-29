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
namespace mathlib.geometry.partitioning
{

	/// <summary>
	/// This interface represents the remaining parts of an hyperplane after
	/// other parts have been chopped off.
	/// 
	/// <p>sub-hyperplanes are obtained when parts of an {@link
	/// Hyperplane hyperplane} are chopped off by other hyperplanes that
	/// intersect it. The remaining part is a convex region. Such objects
	/// appear in <seealso cref="BSPTree BSP trees"/> as the intersection of a cut
	/// hyperplane with the convex region which it splits, the chopping
	/// hyperplanes are the cut hyperplanes closer to the tree root.</p>
	/// 
	/// <p>
	/// Note that this interface is <em>not</em> intended to be implemented
	/// by Apache Commons Math users, it is only intended to be implemented
	/// within the library itself. New methods may be added even for minor
	/// versions, which breaks compatibility for external implementations.
	/// </p>
	/// </summary>
	/// @param <S> Type of the embedding space.
	/// 
	/// @version $Id: SubHyperplane.java 1566416 2014-02-09 20:56:55Z luc $
	/// @since 3.0 </param>
	public interface SubHyperplane<S> where S : mathlib.geometry.Space
	{

		/// <summary>
		/// Copy the instance.
		/// <p>The instance created is completely independent of the original
		/// one. A deep copy is used, none of the underlying objects are
		/// shared (except for the nodes attributes and immutable
		/// objects).</p> </summary>
		/// <returns> a new sub-hyperplane, copy of the instance </returns>
		SubHyperplane<S> copySelf();

		/// <summary>
		/// Get the underlying hyperplane. </summary>
		/// <returns> underlying hyperplane </returns>
		Hyperplane<S> Hyperplane {get;}

		/// <summary>
		/// Check if the instance is empty. </summary>
		/// <returns> true if the instance is empty </returns>
		bool Empty {get;}

		/// <summary>
		/// Get the size of the instance. </summary>
		/// <returns> the size of the instance (this is a length in 1D, an area
		/// in 2D, a volume in 3D ...) </returns>
		double Size {get;}

		/// <summary>
		/// Compute the relative position of the instance with respect
		/// to an hyperplane. </summary>
		/// <param name="hyperplane"> hyperplane to check instance against </param>
		/// <returns> one of <seealso cref="Side#PLUS"/>, <seealso cref="Side#MINUS"/>, <seealso cref="Side#BOTH"/>,
		/// <seealso cref="Side#HYPER"/> </returns>
		Side side(Hyperplane<S> hyperplane);

		/// <summary>
		/// Split the instance in two parts by an hyperplane. </summary>
		/// <param name="hyperplane"> splitting hyperplane </param>
		/// <returns> an object containing both the part of the instance
		/// on the plus side of the hyperplane and the part of the
		/// instance on the minus side of the hyperplane </returns>
		SubHyperplane_SplitSubHyperplane<S> Split(Hyperplane<S> hyperplane);

		/// <summary>
		/// Compute the union of the instance and another sub-hyperplane. </summary>
		/// <param name="other"> other sub-hyperplane to union (<em>must</em> be in the
		/// same hyperplane as the instance) </param>
		/// <returns> a new sub-hyperplane, union of the instance and other </returns>
		SubHyperplane<S> reunite(SubHyperplane<S> other);

		/// <summary>
		/// Class holding the results of the <seealso cref="#split split"/> method. </summary>
		/// @param <U> Type of the embedding space. </param>

	}

	public class SubHyperplane_SplitSubHyperplane<U> where U : mathlib.geometry.Space
	{

		/// <summary>
		/// Part of the sub-hyperplane on the plus side of the splitting hyperplane. </summary>
		private readonly SubHyperplane<U> plus;

		/// <summary>
		/// Part of the sub-hyperplane on the minus side of the splitting hyperplane. </summary>
		private readonly SubHyperplane<U> minus;

		/// <summary>
		/// Build a SubHyperplane.SplitSubHyperplane from its parts. </summary>
		/// <param name="plus"> part of the sub-hyperplane on the plus side of the
		/// splitting hyperplane </param>
		/// <param name="minus"> part of the sub-hyperplane on the minus side of the
		/// splitting hyperplane </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SubHyperplane_SplitSubHyperplane(final SubHyperplane<U> plus, final SubHyperplane<U> minus)
		public SubHyperplane_SplitSubHyperplane(SubHyperplane<U> plus, SubHyperplane<U> minus)
		{
			this.plus = plus;
			this.minus = minus;
		}

		/// <summary>
		/// Get the part of the sub-hyperplane on the plus side of the splitting hyperplane. </summary>
		/// <returns> part of the sub-hyperplane on the plus side of the splitting hyperplane </returns>
		public virtual SubHyperplane<U> Plus
		{
			get
			{
				return plus;
			}
		}

		/// <summary>
		/// Get the part of the sub-hyperplane on the minus side of the splitting hyperplane. </summary>
		/// <returns> part of the sub-hyperplane on the minus side of the splitting hyperplane </returns>
		public virtual SubHyperplane<U> Minus
		{
			get
			{
				return minus;
			}
		}

	}

}
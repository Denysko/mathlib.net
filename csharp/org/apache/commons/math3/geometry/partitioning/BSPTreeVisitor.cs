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
	/// This interface is used to visit <seealso cref="BSPTree BSP tree"/> nodes.
	/// 
	/// <p>Navigation through <seealso cref="BSPTree BSP trees"/> can be done using
	/// two different point of views:</p>
	/// <ul>
	///   <li>
	///     the first one is in a node-oriented way using the {@link
	///     BSPTree#getPlus}, <seealso cref="BSPTree#getMinus"/> and {@link
	///     BSPTree#getParent} methods. Terminal nodes without associated
	///     <seealso cref="SubHyperplane sub-hyperplanes"/> can be visited this way,
	///     there is no constraint in the visit order, and it is possible
	///     to visit either all nodes or only a subset of the nodes
	///   </li>
	///   <li>
	///     the second one is in a sub-hyperplane-oriented way using
	///     classes implementing this interface which obeys the visitor
	///     design pattern. The visit order is provided by the visitor as
	///     each node is first encountered. Each node is visited exactly
	///     once.
	///   </li>
	/// </ul>
	/// </summary>
	/// @param <S> Type of the space.
	/// </param>
	/// <seealso cref= BSPTree </seealso>
	/// <seealso cref= SubHyperplane
	/// 
	/// @version $Id: BSPTreeVisitor.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0 </seealso>
	public interface BSPTreeVisitor<S> where S : org.apache.commons.math3.geometry.Space
	{

		/// <summary>
		/// Enumerate for visit order with respect to plus sub-tree, minus sub-tree and cut sub-hyperplane. </summary>

		/// <summary>
		/// Determine the visit order for this node.
		/// <p>Before attempting to visit an internal node, this method is
		/// called to determine the desired ordering of the visit. It is
		/// guaranteed that this method will be called before {@link
		/// #visitInternalNode visitInternalNode} for a given node, it will be
		/// called exactly once for each internal node.</p> </summary>
		/// <param name="node"> BSP node guaranteed to have a non null cut sub-hyperplane </param>
		/// <returns> desired visit order, must be one of
		/// <seealso cref="Order#PLUS_MINUS_SUB"/>, <seealso cref="Order#PLUS_SUB_MINUS"/>,
		/// <seealso cref="Order#MINUS_PLUS_SUB"/>, <seealso cref="Order#MINUS_SUB_PLUS"/>,
		/// <seealso cref="Order#SUB_PLUS_MINUS"/>, <seealso cref="Order#SUB_MINUS_PLUS"/> </returns>
		BSPTreeVisitor_Order visitOrder(BSPTree<S> node);

		/// <summary>
		/// Visit a BSP tree node node having a non-null sub-hyperplane.
		/// <p>It is guaranteed that this method will be called after {@link
		/// #visitOrder visitOrder} has been called for a given node,
		/// it wil be called exactly once for each internal node.</p> </summary>
		/// <param name="node"> BSP node guaranteed to have a non null cut sub-hyperplane </param>
		/// <seealso cref= #visitLeafNode </seealso>
		void visitInternalNode(BSPTree<S> node);

		/// <summary>
		/// Visit a leaf BSP tree node node having a null sub-hyperplane. </summary>
		/// <param name="node"> leaf BSP node having a null sub-hyperplane </param>
		/// <seealso cref= #visitInternalNode </seealso>
		void visitLeafNode(BSPTree<S> node);

	}

	public enum BSPTreeVisitor_Order
	{
		/// <summary>
		/// Indicator for visit order plus sub-tree, then minus sub-tree,
		/// and last cut sub-hyperplane.
		/// </summary>
		PLUS_MINUS_SUB,

		/// <summary>
		/// Indicator for visit order plus sub-tree, then cut sub-hyperplane,
		/// and last minus sub-tree.
		/// </summary>
		PLUS_SUB_MINUS,

		/// <summary>
		/// Indicator for visit order minus sub-tree, then plus sub-tree,
		/// and last cut sub-hyperplane.
		/// </summary>
		MINUS_PLUS_SUB,

		/// <summary>
		/// Indicator for visit order minus sub-tree, then cut sub-hyperplane,
		/// and last plus sub-tree.
		/// </summary>
		MINUS_SUB_PLUS,

		/// <summary>
		/// Indicator for visit order cut sub-hyperplane, then plus sub-tree,
		/// and last minus sub-tree.
		/// </summary>
		SUB_PLUS_MINUS,

		/// <summary>
		/// Indicator for visit order cut sub-hyperplane, then minus sub-tree,
		/// and last plus sub-tree.
		/// </summary>
		SUB_MINUS_PLUS
	}

}
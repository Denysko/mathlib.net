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
namespace org.apache.commons.math3.geometry.partitioning.utilities
{

	/// <summary>
	/// This class implements AVL trees.
	/// 
	/// <p>The purpose of this class is to sort elements while allowing
	/// duplicate elements (i.e. such that {@code a.equals(b)} is
	/// true). The {@code SortedSet} interface does not allow this, so
	/// a specific class is needed. Null elements are not allowed.</p>
	/// 
	/// <p>Since the {@code equals} method is not sufficient to
	/// differentiate elements, the <seealso cref="#delete delete"/> method is
	/// implemented using the equality operator.</p>
	/// 
	/// <p>In order to clearly mark the methods provided here do not have
	/// the same semantics as the ones specified in the
	/// {@code SortedSet} interface, different names are used
	/// ({@code add} has been replaced by <seealso cref="#insert insert"/> and
	/// {@code remove} has been replaced by {@link #delete
	/// delete}).</p>
	/// 
	/// <p>This class is based on the C implementation Georg Kraml has put
	/// in the public domain. Unfortunately, his <a
	/// href="www.purists.org/georg/avltree/index.html">page</a> seems not
	/// to exist any more.</p>
	/// </summary>
	/// @param <T> the type of the elements
	/// 
	/// @version $Id: AVLTree.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0 </param>
	public class AVLTree<T> where T : Comparable<T>
	{

		/// <summary>
		/// Top level node. </summary>
		private Node top;

		/// <summary>
		/// Build an empty tree.
		/// </summary>
		public AVLTree()
		{
			top = null;
		}

		/// <summary>
		/// Insert an element in the tree. </summary>
		/// <param name="element"> element to insert (silently ignored if null) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void insert(final T element)
		public virtual void insert(T element)
		{
			if (element != null)
			{
				if (top == null)
				{
					top = new Node(this, element, null);
				}
				else
				{
					top.insert(element);
				}
			}
		}

		/// <summary>
		/// Delete an element from the tree.
		/// <p>The element is deleted only if there is a node {@code n}
		/// containing exactly the element instance specified, i.e. for which
		/// {@code n.getElement() == element}. This is purposely
		/// <em>different</em> from the specification of the
		/// {@code java.util.Set} {@code remove} method (in fact,
		/// this is the reason why a specific class has been developed).</p> </summary>
		/// <param name="element"> element to delete (silently ignored if null) </param>
		/// <returns> true if the element was deleted from the tree </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean delete(final T element)
		public virtual bool delete(T element)
		{
			if (element != null)
			{
				for (Node node = getNotSmaller(element); node != null; node = node.Next)
				{
					// loop over all elements neither smaller nor larger
					// than the specified one
					if (node.element == element)
					{
						node.delete();
						return true;
					}
					else if (node.element.compareTo(element) > 0)
					{
						// all the remaining elements are known to be larger,
						// the element is not in the tree
						return false;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Check if the tree is empty. </summary>
		/// <returns> true if the tree is empty </returns>
		public virtual bool Empty
		{
			get
			{
				return top == null;
			}
		}


		/// <summary>
		/// Get the number of elements of the tree. </summary>
		/// <returns> number of elements contained in the tree </returns>
		public virtual int size()
		{
			return (top == null) ? 0 : top.size();
		}

		/// <summary>
		/// Get the node whose element is the smallest one in the tree. </summary>
		/// <returns> the tree node containing the smallest element in the tree
		/// or null if the tree is empty </returns>
		/// <seealso cref= #getLargest </seealso>
		/// <seealso cref= #getNotSmaller </seealso>
		/// <seealso cref= #getNotLarger </seealso>
		/// <seealso cref= Node#getPrevious </seealso>
		/// <seealso cref= Node#getNext </seealso>
		public virtual Node Smallest
		{
			get
			{
				return (top == null) ? null : top.Smallest;
			}
		}

		/// <summary>
		/// Get the node whose element is the largest one in the tree. </summary>
		/// <returns> the tree node containing the largest element in the tree
		/// or null if the tree is empty </returns>
		/// <seealso cref= #getSmallest </seealso>
		/// <seealso cref= #getNotSmaller </seealso>
		/// <seealso cref= #getNotLarger </seealso>
		/// <seealso cref= Node#getPrevious </seealso>
		/// <seealso cref= Node#getNext </seealso>
		public virtual Node Largest
		{
			get
			{
				return (top == null) ? null : top.Largest;
			}
		}

		/// <summary>
		/// Get the node whose element is not smaller than the reference object. </summary>
		/// <param name="reference"> reference object (may not be in the tree) </param>
		/// <returns> the tree node containing the smallest element not smaller
		/// than the reference object or null if either the tree is empty or
		/// all its elements are smaller than the reference object </returns>
		/// <seealso cref= #getSmallest </seealso>
		/// <seealso cref= #getLargest </seealso>
		/// <seealso cref= #getNotLarger </seealso>
		/// <seealso cref= Node#getPrevious </seealso>
		/// <seealso cref= Node#getNext </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Node getNotSmaller(final T reference)
		public virtual Node getNotSmaller(T reference)
		{
			Node candidate = null;
			for (Node node = top; node != null;)
			{
				if (node.element.compareTo(reference) < 0)
				{
					if (node.right == null)
					{
						return candidate;
					}
					node = node.right;
				}
				else
				{
					candidate = node;
					if (node.left == null)
					{
						return candidate;
					}
					node = node.left;
				}
			}
			return null;
		}

		/// <summary>
		/// Get the node whose element is not larger than the reference object. </summary>
		/// <param name="reference"> reference object (may not be in the tree) </param>
		/// <returns> the tree node containing the largest element not larger
		/// than the reference object (in which case the node is guaranteed
		/// not to be empty) or null if either the tree is empty or all its
		/// elements are larger than the reference object </returns>
		/// <seealso cref= #getSmallest </seealso>
		/// <seealso cref= #getLargest </seealso>
		/// <seealso cref= #getNotSmaller </seealso>
		/// <seealso cref= Node#getPrevious </seealso>
		/// <seealso cref= Node#getNext </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Node getNotLarger(final T reference)
		public virtual Node getNotLarger(T reference)
		{
			Node candidate = null;
			for (Node node = top; node != null;)
			{
				if (node.element.compareTo(reference) > 0)
				{
					if (node.left == null)
					{
						return candidate;
					}
					node = node.left;
				}
				else
				{
					candidate = node;
					if (node.right == null)
					{
						return candidate;
					}
					node = node.right;
				}
			}
			return null;
		}

		/// <summary>
		/// Enum for tree skew factor. </summary>
		private enum Skew
		{
			/// <summary>
			/// Code for left high trees. </summary>
			LEFT_HIGH,

			/// <summary>
			/// Code for right high trees. </summary>
			RIGHT_HIGH,

			/// <summary>
			/// Code for Skew.BALANCED trees. </summary>
			BALANCED
		}

		/// <summary>
		/// This class implements AVL trees nodes.
		/// <p>AVL tree nodes implement all the logical structure of the
		/// tree. Nodes are created by the <seealso cref="AVLTree AVLTree"/> class.</p>
		/// <p>The nodes are not independant from each other but must obey
		/// specific balancing constraints and the tree structure is
		/// rearranged as elements are inserted or deleted from the tree. The
		/// creation, modification and tree-related navigation methods have
		/// therefore restricted access. Only the order-related navigation,
		/// reading and delete methods are public.</p> </summary>
		/// <seealso cref= AVLTree </seealso>
		public class Node
		{
			private readonly AVLTree outerInstance;


			/// <summary>
			/// Element contained in the current node. </summary>
			internal T element;

			/// <summary>
			/// Left sub-tree. </summary>
			internal Node left;

			/// <summary>
			/// Right sub-tree. </summary>
			internal Node right;

			/// <summary>
			/// Parent tree. </summary>
			internal Node parent;

			/// <summary>
			/// Skew factor. </summary>
			internal Skew skew;

			/// <summary>
			/// Build a node for a specified element. </summary>
			/// <param name="element"> element </param>
			/// <param name="parent"> parent node </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: Node(final T element, final Node parent)
			internal Node(AVLTree outerInstance, T element, Node parent)
			{
				this.outerInstance = outerInstance;
				this.element = element;
				left = null;
				right = null;
				this.parent = parent;
				skew = Skew.BALANCED;
			}

			/// <summary>
			/// Get the contained element. </summary>
			/// <returns> element contained in the node </returns>
			public virtual T Element
			{
				get
				{
					return element;
				}
			}

			/// <summary>
			/// Get the number of elements of the tree rooted at this node. </summary>
			/// <returns> number of elements contained in the tree rooted at this node </returns>
			internal virtual int size()
			{
				return 1 + ((left == null) ? 0 : left.size()) + ((right == null) ? 0 : right.size());
			}

			/// <summary>
			/// Get the node whose element is the smallest one in the tree
			/// rooted at this node. </summary>
			/// <returns> the tree node containing the smallest element in the
			/// tree rooted at this node or null if the tree is empty </returns>
			/// <seealso cref= #getLargest </seealso>
			internal virtual Node Smallest
			{
				get
				{
					Node node = this;
					while (node.left != null)
					{
						node = node.left;
					}
					return node;
				}
			}

			/// <summary>
			/// Get the node whose element is the largest one in the tree
			/// rooted at this node. </summary>
			/// <returns> the tree node containing the largest element in the
			/// tree rooted at this node or null if the tree is empty </returns>
			/// <seealso cref= #getSmallest </seealso>
			internal virtual Node Largest
			{
				get
				{
					Node node = this;
					while (node.right != null)
					{
						node = node.right;
					}
					return node;
				}
			}

			/// <summary>
			/// Get the node containing the next smaller or equal element. </summary>
			/// <returns> node containing the next smaller or equal element or
			/// null if there is no smaller or equal element in the tree </returns>
			/// <seealso cref= #getNext </seealso>
			public virtual Node Previous
			{
				get
				{
    
					if (left != null)
					{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Node node = left.getLargest();
						Node node = left.Largest;
						if (node != null)
						{
							return node;
						}
					}
    
					for (Node node = this; node.parent != null; node = node.parent)
					{
						if (node != node.parent.left)
						{
							return node.parent;
						}
					}
    
					return null;
    
				}
			}

			/// <summary>
			/// Get the node containing the next larger or equal element. </summary>
			/// <returns> node containing the next larger or equal element (in
			/// which case the node is guaranteed not to be empty) or null if
			/// there is no larger or equal element in the tree </returns>
			/// <seealso cref= #getPrevious </seealso>
			public virtual Node Next
			{
				get
				{
    
					if (right != null)
					{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Node node = right.getSmallest();
						Node node = right.Smallest;
						if (node != null)
						{
							return node;
						}
					}
    
					for (Node node = this; node.parent != null; node = node.parent)
					{
						if (node != node.parent.right)
						{
							return node.parent;
						}
					}
    
					return null;
    
				}
			}

			/// <summary>
			/// Insert an element in a sub-tree. </summary>
			/// <param name="newElement"> element to insert </param>
			/// <returns> true if the parent tree should be re-Skew.BALANCED </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: boolean insert(final T newElement)
			internal virtual bool insert(T newElement)
			{
				if (newElement.compareTo(this.element) < 0)
				{
					// the inserted element is smaller than the node
					if (left == null)
					{
						left = new Node(outerInstance, newElement, this);
						return rebalanceLeftGrown();
					}
					return left.insert(newElement) ? rebalanceLeftGrown() : false;
				}

				// the inserted element is equal to or greater than the node
				if (right == null)
				{
					right = new Node(outerInstance, newElement, this);
					return rebalanceRightGrown();
				}
				return right.insert(newElement) ? rebalanceRightGrown() : false;

			}

			/// <summary>
			/// Delete the node from the tree.
			/// </summary>
			public virtual void delete()
			{
				if ((parent == null) && (left == null) && (right == null))
				{
					// this was the last node, the tree is now empty
					element = null;
					outerInstance.top = null;
				}
				else
				{

					Node node;
					Node child;
					bool leftShrunk;
					if ((left == null) && (right == null))
					{
						node = this;
						element = null;
						leftShrunk = node == node.parent.left;
						child = null;
					}
					else
					{
						node = (left != null) ? left.Largest : right.Smallest;
						element = node.element;
						leftShrunk = node == node.parent.left;
						child = (node.left != null) ? node.left : node.right;
					}

					node = node.parent;
					if (leftShrunk)
					{
						node.left = child;
					}
					else
					{
						node.right = child;
					}
					if (child != null)
					{
						child.parent = node;
					}

					while (leftShrunk ? node.rebalanceLeftShrunk() : node.rebalanceRightShrunk())
					{
						if (node.parent == null)
						{
							return;
						}
						leftShrunk = node == node.parent.left;
						node = node.parent;
					}

				}
			}

			/// <summary>
			/// Re-balance the instance as left sub-tree has grown. </summary>
			/// <returns> true if the parent tree should be reSkew.BALANCED too </returns>
			internal virtual bool rebalanceLeftGrown()
			{
				switch (skew)
				{
				case org.apache.commons.math3.geometry.partitioning.utilities.AVLTree.Skew.LEFT_HIGH:
					if (left.skew == Skew.LEFT_HIGH)
					{
						rotateCW();
						skew = Skew.BALANCED;
						right.skew = Skew.BALANCED;
					}
					else
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Skew s = left.right.skew;
						Skew s = left.right.skew;
						left.rotateCCW();
						rotateCW();
						switch (s)
						{
						case org.apache.commons.math3.geometry.partitioning.utilities.AVLTree.Skew.LEFT_HIGH:
							left.skew = Skew.BALANCED;
							right.skew = Skew.RIGHT_HIGH;
							break;
						case org.apache.commons.math3.geometry.partitioning.utilities.AVLTree.Skew.RIGHT_HIGH:
							left.skew = Skew.LEFT_HIGH;
							right.skew = Skew.BALANCED;
							break;
						default:
							left.skew = Skew.BALANCED;
							right.skew = Skew.BALANCED;
						break;
						}
						skew = Skew.BALANCED;
					}
					return false;
				case org.apache.commons.math3.geometry.partitioning.utilities.AVLTree.Skew.RIGHT_HIGH:
					skew = Skew.BALANCED;
					return false;
				default:
					skew = Skew.LEFT_HIGH;
					return true;
				}
			}

			/// <summary>
			/// Re-balance the instance as right sub-tree has grown. </summary>
			/// <returns> true if the parent tree should be reSkew.BALANCED too </returns>
			internal virtual bool rebalanceRightGrown()
			{
				switch (skew)
				{
				case org.apache.commons.math3.geometry.partitioning.utilities.AVLTree.Skew.LEFT_HIGH:
					skew = Skew.BALANCED;
					return false;
				case org.apache.commons.math3.geometry.partitioning.utilities.AVLTree.Skew.RIGHT_HIGH:
					if (right.skew == Skew.RIGHT_HIGH)
					{
						rotateCCW();
						skew = Skew.BALANCED;
						left.skew = Skew.BALANCED;
					}
					else
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Skew s = right.left.skew;
						Skew s = right.left.skew;
						right.rotateCW();
						rotateCCW();
						switch (s)
						{
						case org.apache.commons.math3.geometry.partitioning.utilities.AVLTree.Skew.LEFT_HIGH:
							left.skew = Skew.BALANCED;
							right.skew = Skew.RIGHT_HIGH;
							break;
						case org.apache.commons.math3.geometry.partitioning.utilities.AVLTree.Skew.RIGHT_HIGH:
							left.skew = Skew.LEFT_HIGH;
							right.skew = Skew.BALANCED;
							break;
						default:
							left.skew = Skew.BALANCED;
							right.skew = Skew.BALANCED;
						break;
						}
						skew = Skew.BALANCED;
					}
					return false;
				default:
					skew = Skew.RIGHT_HIGH;
					return true;
				}
			}

			/// <summary>
			/// Re-balance the instance as left sub-tree has shrunk. </summary>
			/// <returns> true if the parent tree should be reSkew.BALANCED too </returns>
			internal virtual bool rebalanceLeftShrunk()
			{
				switch (skew)
				{
				case org.apache.commons.math3.geometry.partitioning.utilities.AVLTree.Skew.LEFT_HIGH:
					skew = Skew.BALANCED;
					return true;
				case org.apache.commons.math3.geometry.partitioning.utilities.AVLTree.Skew.RIGHT_HIGH:
					if (right.skew == Skew.RIGHT_HIGH)
					{
						rotateCCW();
						skew = Skew.BALANCED;
						left.skew = Skew.BALANCED;
						return true;
					}
					else if (right.skew == Skew.BALANCED)
					{
						rotateCCW();
						skew = Skew.LEFT_HIGH;
						left.skew = Skew.RIGHT_HIGH;
						return false;
					}
					else
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Skew s = right.left.skew;
						Skew s = right.left.skew;
						right.rotateCW();
						rotateCCW();
						switch (s)
						{
						case org.apache.commons.math3.geometry.partitioning.utilities.AVLTree.Skew.LEFT_HIGH:
							left.skew = Skew.BALANCED;
							right.skew = Skew.RIGHT_HIGH;
							break;
						case org.apache.commons.math3.geometry.partitioning.utilities.AVLTree.Skew.RIGHT_HIGH:
							left.skew = Skew.LEFT_HIGH;
							right.skew = Skew.BALANCED;
							break;
						default:
							left.skew = Skew.BALANCED;
							right.skew = Skew.BALANCED;
						break;
						}
						skew = Skew.BALANCED;
						return true;
					}
				default:
					skew = Skew.RIGHT_HIGH;
					return false;
				}
			}

			/// <summary>
			/// Re-balance the instance as right sub-tree has shrunk. </summary>
			/// <returns> true if the parent tree should be reSkew.BALANCED too </returns>
			internal virtual bool rebalanceRightShrunk()
			{
				switch (skew)
				{
				case org.apache.commons.math3.geometry.partitioning.utilities.AVLTree.Skew.RIGHT_HIGH:
					skew = Skew.BALANCED;
					return true;
				case org.apache.commons.math3.geometry.partitioning.utilities.AVLTree.Skew.LEFT_HIGH:
					if (left.skew == Skew.LEFT_HIGH)
					{
						rotateCW();
						skew = Skew.BALANCED;
						right.skew = Skew.BALANCED;
						return true;
					}
					else if (left.skew == Skew.BALANCED)
					{
						rotateCW();
						skew = Skew.RIGHT_HIGH;
						right.skew = Skew.LEFT_HIGH;
						return false;
					}
					else
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Skew s = left.right.skew;
						Skew s = left.right.skew;
						left.rotateCCW();
						rotateCW();
						switch (s)
						{
						case org.apache.commons.math3.geometry.partitioning.utilities.AVLTree.Skew.LEFT_HIGH:
							left.skew = Skew.BALANCED;
							right.skew = Skew.RIGHT_HIGH;
							break;
						case org.apache.commons.math3.geometry.partitioning.utilities.AVLTree.Skew.RIGHT_HIGH:
							left.skew = Skew.LEFT_HIGH;
							right.skew = Skew.BALANCED;
							break;
						default:
							left.skew = Skew.BALANCED;
							right.skew = Skew.BALANCED;
						break;
						}
						skew = Skew.BALANCED;
						return true;
					}
				default:
					skew = Skew.LEFT_HIGH;
					return false;
				}
			}

			/// <summary>
			/// Perform a clockwise rotation rooted at the instance.
			/// <p>The skew factor are not updated by this method, they
			/// <em>must</em> be updated by the caller</p>
			/// </summary>
			internal virtual void rotateCW()
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T tmpElt = element;
				T tmpElt = element;
				element = left.element;
				left.element = tmpElt;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node tmpNode = left;
				Node tmpNode = left;
				left = tmpNode.left;
				tmpNode.left = tmpNode.right;
				tmpNode.right = right;
				right = tmpNode;

				if (left != null)
				{
					left.parent = this;
				}
				if (right.right != null)
				{
					right.right.parent = right;
				}

			}

			/// <summary>
			/// Perform a counter-clockwise rotation rooted at the instance.
			/// <p>The skew factor are not updated by this method, they
			/// <em>must</em> be updated by the caller</p>
			/// </summary>
			internal virtual void rotateCCW()
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T tmpElt = element;
				T tmpElt = element;
				element = right.element;
				right.element = tmpElt;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Node tmpNode = right;
				Node tmpNode = right;
				right = tmpNode.right;
				tmpNode.right = tmpNode.left;
				tmpNode.left = left;
				left = tmpNode;

				if (right != null)
				{
					right.parent = this;
				}
				if (left.left != null)
				{
					left.left.parent = left;
				}

			}

		}

	}

}
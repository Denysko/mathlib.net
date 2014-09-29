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
	/// This class is a factory for <seealso cref="Region"/>.
	/// </summary>
	/// @param <S> Type of the space.
	/// 
	/// @version $Id: RegionFactory.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0 </param>
	public class RegionFactory<S> where S : mathlib.geometry.Space
	{

		/// <summary>
		/// Visitor removing internal nodes attributes. </summary>
		private readonly NodesCleaner nodeCleaner;

		/// <summary>
		/// Simple constructor.
		/// </summary>
		public RegionFactory()
		{
			nodeCleaner = new NodesCleaner(this);
		}

		/// <summary>
		/// Build a convex region from a collection of bounding hyperplanes. </summary>
		/// <param name="hyperplanes"> collection of bounding hyperplanes </param>
		/// <returns> a new convex region, or null if the collection is empty </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Region<S> buildConvex(final Hyperplane<S>... hyperplanes)
		public virtual Region<S> buildConvex(params Hyperplane<S>[] hyperplanes)
		{
			if ((hyperplanes == null) || (hyperplanes.Length == 0))
			{
				return null;
			}

			// use the first hyperplane to build the right class
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Region<S> region = hyperplanes[0].wholeSpace();
			Region<S> region = hyperplanes[0].wholeSpace();

			// chop off parts of the space
			BSPTree<S> node = region.getTree(false);
			node.Attribute = true;
			foreach (Hyperplane<S> hyperplane in hyperplanes)
			{
				if (node.insertCut(hyperplane))
				{
					node.Attribute = null;
					node.Plus.Attribute = false;
					node = node.Minus;
					node.Attribute = true;
				}
			}

			return region;

		}

		/// <summary>
		/// Compute the union of two regions. </summary>
		/// <param name="region1"> first region (will be unusable after the operation as
		/// parts of it will be reused in the new region) </param>
		/// <param name="region2"> second region (will be unusable after the operation as
		/// parts of it will be reused in the new region) </param>
		/// <returns> a new region, result of {@code region1 union region2} </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Region<S> union(final Region<S> region1, final Region<S> region2)
		public virtual Region<S> union(Region<S> region1, Region<S> region2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BSPTree<S> tree = region1.getTree(false).merge(region2.getTree(false), new UnionMerger());
			BSPTree<S> tree = region1.getTree(false).merge(region2.getTree(false), new UnionMerger(this));
			tree.visit(nodeCleaner);
			return region1.buildNew(tree);
		}

		/// <summary>
		/// Compute the intersection of two regions. </summary>
		/// <param name="region1"> first region (will be unusable after the operation as
		/// parts of it will be reused in the new region) </param>
		/// <param name="region2"> second region (will be unusable after the operation as
		/// parts of it will be reused in the new region) </param>
		/// <returns> a new region, result of {@code region1 intersection region2} </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Region<S> intersection(final Region<S> region1, final Region<S> region2)
		public virtual Region<S> intersection(Region<S> region1, Region<S> region2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BSPTree<S> tree = region1.getTree(false).merge(region2.getTree(false), new IntersectionMerger());
			BSPTree<S> tree = region1.getTree(false).merge(region2.getTree(false), new IntersectionMerger(this));
			tree.visit(nodeCleaner);
			return region1.buildNew(tree);
		}

		/// <summary>
		/// Compute the symmetric difference (exclusive or) of two regions. </summary>
		/// <param name="region1"> first region (will be unusable after the operation as
		/// parts of it will be reused in the new region) </param>
		/// <param name="region2"> second region (will be unusable after the operation as
		/// parts of it will be reused in the new region) </param>
		/// <returns> a new region, result of {@code region1 xor region2} </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Region<S> xor(final Region<S> region1, final Region<S> region2)
		public virtual Region<S> xor(Region<S> region1, Region<S> region2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BSPTree<S> tree = region1.getTree(false).merge(region2.getTree(false), new XorMerger());
			BSPTree<S> tree = region1.getTree(false).merge(region2.getTree(false), new XorMerger(this));
			tree.visit(nodeCleaner);
			return region1.buildNew(tree);
		}

		/// <summary>
		/// Compute the difference of two regions. </summary>
		/// <param name="region1"> first region (will be unusable after the operation as
		/// parts of it will be reused in the new region) </param>
		/// <param name="region2"> second region (will be unusable after the operation as
		/// parts of it will be reused in the new region) </param>
		/// <returns> a new region, result of {@code region1 minus region2} </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Region<S> difference(final Region<S> region1, final Region<S> region2)
		public virtual Region<S> difference(Region<S> region1, Region<S> region2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BSPTree<S> tree = region1.getTree(false).merge(region2.getTree(false), new DifferenceMerger());
			BSPTree<S> tree = region1.getTree(false).merge(region2.getTree(false), new DifferenceMerger(this));
			tree.visit(nodeCleaner);
			return region1.buildNew(tree);
		}

		/// <summary>
		/// Get the complement of the region (exchanged interior/exterior). </summary>
		/// <param name="region"> region to complement, it will not modified, a new
		/// region independent region will be built </param>
		/// <returns> a new region, complement of the specified one </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Region<S> getComplement(final Region<S> region)
		public virtual Region<S> getComplement(Region<S> region)
		{
			return region.buildNew(recurseComplement(region.getTree(false)));
		}

		/// <summary>
		/// Recursively build the complement of a BSP tree. </summary>
		/// <param name="node"> current node of the original tree </param>
		/// <returns> new tree, complement of the node </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private BSPTree<S> recurseComplement(final BSPTree<S> node)
		private BSPTree<S> recurseComplement(BSPTree<S> node)
		{
			if (node.Cut == null)
			{
				return new BSPTree<S>(((bool?) node.Attribute) ? false : true);
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") BoundaryAttribute<S> attribute = (BoundaryAttribute<S>) node.getAttribute();
			BoundaryAttribute<S> attribute = (BoundaryAttribute<S>) node.Attribute;
			if (attribute != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SubHyperplane<S> plusOutside = (attribute.getPlusInside() == null) ? null : attribute.getPlusInside().copySelf();
				SubHyperplane<S> plusOutside = (attribute.PlusInside == null) ? null : attribute.PlusInside.copySelf();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SubHyperplane<S> plusInside = (attribute.getPlusOutside() == null) ? null : attribute.getPlusOutside().copySelf();
				SubHyperplane<S> plusInside = (attribute.PlusOutside == null) ? null : attribute.PlusOutside.copySelf();
				attribute = new BoundaryAttribute<S>(plusOutside, plusInside);
			}

			return new BSPTree<S>(node.Cut.copySelf(), recurseComplement(node.Plus), recurseComplement(node.Minus), attribute);

		}

		/// <summary>
		/// BSP tree leaf merger computing union of two regions. </summary>
		private class UnionMerger : BSPTree.LeafMerger<S>
		{
			private readonly RegionFactory outerInstance;

			public UnionMerger(RegionFactory outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BSPTree<S> merge(final BSPTree<S> leaf, final BSPTree<S> tree, final BSPTree<S> parentTree, final boolean isPlusChild, final boolean leafFromInstance)
			public virtual BSPTree<S> merge(BSPTree<S> leaf, BSPTree<S> tree, BSPTree<S> parentTree, bool isPlusChild, bool leafFromInstance)
			{
				if ((bool?) leaf.Attribute)
				{
					// the leaf node represents an inside cell
					leaf.insertInTree(parentTree, isPlusChild);
					return leaf;
				}
				// the leaf node represents an outside cell
				tree.insertInTree(parentTree, isPlusChild);
				return tree;
			}
		}

		/// <summary>
		/// BSP tree leaf merger computing union of two regions. </summary>
		private class IntersectionMerger : BSPTree.LeafMerger<S>
		{
			private readonly RegionFactory outerInstance;

			public IntersectionMerger(RegionFactory outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BSPTree<S> merge(final BSPTree<S> leaf, final BSPTree<S> tree, final BSPTree<S> parentTree, final boolean isPlusChild, final boolean leafFromInstance)
			public virtual BSPTree<S> merge(BSPTree<S> leaf, BSPTree<S> tree, BSPTree<S> parentTree, bool isPlusChild, bool leafFromInstance)
			{
				if ((bool?) leaf.Attribute)
				{
					// the leaf node represents an inside cell
					tree.insertInTree(parentTree, isPlusChild);
					return tree;
				}
				// the leaf node represents an outside cell
				leaf.insertInTree(parentTree, isPlusChild);
				return leaf;
			}
		}

		/// <summary>
		/// BSP tree leaf merger computing union of two regions. </summary>
		private class XorMerger : BSPTree.LeafMerger<S>
		{
			private readonly RegionFactory outerInstance;

			public XorMerger(RegionFactory outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BSPTree<S> merge(final BSPTree<S> leaf, final BSPTree<S> tree, final BSPTree<S> parentTree, final boolean isPlusChild, final boolean leafFromInstance)
			public virtual BSPTree<S> merge(BSPTree<S> leaf, BSPTree<S> tree, BSPTree<S> parentTree, bool isPlusChild, bool leafFromInstance)
			{
				BSPTree<S> t = tree;
				if ((bool?) leaf.Attribute)
				{
					// the leaf node represents an inside cell
					t = outerInstance.recurseComplement(t);
				}
				t.insertInTree(parentTree, isPlusChild);
				return t;
			}
		}

		/// <summary>
		/// BSP tree leaf merger computing union of two regions. </summary>
		private class DifferenceMerger : BSPTree.LeafMerger<S>
		{
			private readonly RegionFactory outerInstance;

			public DifferenceMerger(RegionFactory outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BSPTree<S> merge(final BSPTree<S> leaf, final BSPTree<S> tree, final BSPTree<S> parentTree, final boolean isPlusChild, final boolean leafFromInstance)
			public virtual BSPTree<S> merge(BSPTree<S> leaf, BSPTree<S> tree, BSPTree<S> parentTree, bool isPlusChild, bool leafFromInstance)
			{
				if ((bool?) leaf.Attribute)
				{
					// the leaf node represents an inside cell
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BSPTree<S> argTree = recurseComplement(leafFromInstance ? tree : leaf);
					BSPTree<S> argTree = outerInstance.recurseComplement(leafFromInstance ? tree : leaf);
					argTree.insertInTree(parentTree, isPlusChild);
					return argTree;
				}
				// the leaf node represents an outside cell
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BSPTree<S> instanceTree = leafFromInstance ? leaf : tree;
				BSPTree<S> instanceTree = leafFromInstance ? leaf : tree;
				instanceTree.insertInTree(parentTree, isPlusChild);
				return instanceTree;
			}
		}

		/// <summary>
		/// Visitor removing internal nodes attributes. </summary>
		private class NodesCleaner : BSPTreeVisitor<S>
		{
			private readonly RegionFactory outerInstance;

			public NodesCleaner(RegionFactory outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BSPTreeVisitor_Order visitOrder(final BSPTree<S> node)
			public virtual BSPTreeVisitor_Order visitOrder(BSPTree<S> node)
			{
				return BSPTreeVisitor_Order.PLUS_SUB_MINUS;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visitInternalNode(final BSPTree<S> node)
			public virtual void visitInternalNode(BSPTree<S> node)
			{
				node.Attribute = null;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visitLeafNode(final BSPTree<S> node)
			public virtual void visitLeafNode(BSPTree<S> node)
			{
			}

		}

	}

}
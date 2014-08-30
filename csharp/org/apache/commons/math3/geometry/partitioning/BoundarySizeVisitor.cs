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
	/// Visitor computing the boundary size. </summary>
	/// @param <S> Type of the space.
	/// @version $Id: BoundarySizeVisitor.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0 </param>
	internal class BoundarySizeVisitor<S> : BSPTreeVisitor<S> where S : org.apache.commons.math3.geometry.Space
	{

		/// <summary>
		/// Size of the boundary. </summary>
		private double boundarySize;

		/// <summary>
		/// Simple constructor.
		/// </summary>
		public BoundarySizeVisitor()
		{
			boundarySize = 0;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BSPTreeVisitor_Order visitOrder(final BSPTree<S> node)
		public virtual BSPTreeVisitor_Order visitOrder(BSPTree<S> node)
		{
			return BSPTreeVisitor_Order.MINUS_SUB_PLUS;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visitInternalNode(final BSPTree<S> node)
		public virtual void visitInternalNode(BSPTree<S> node)
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final BoundaryAttribute<S> attribute = (BoundaryAttribute<S>) node.getAttribute();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			BoundaryAttribute<S> attribute = (BoundaryAttribute<S>) node.Attribute;
			if (attribute.PlusOutside != null)
			{
				boundarySize += attribute.PlusOutside.Size;
			}
			if (attribute.PlusInside != null)
			{
				boundarySize += attribute.PlusInside.Size;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visitLeafNode(final BSPTree<S> node)
		public virtual void visitLeafNode(BSPTree<S> node)
		{
		}

		/// <summary>
		/// Get the size of the boundary. </summary>
		/// <returns> size of the boundary </returns>
		public virtual double Size
		{
			get
			{
				return boundarySize;
			}
		}

	}

}
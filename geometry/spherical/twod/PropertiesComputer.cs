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
namespace org.apache.commons.math3.geometry.spherical.twod
{


	using MathInternalError = org.apache.commons.math3.exception.MathInternalError;
	using Vector3D = org.apache.commons.math3.geometry.euclidean.threed.Vector3D;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// Visitor computing geometrical properties.
	/// @version $Id: PropertiesComputer.java 1567599 2014-02-12 11:12:45Z luc $
	/// @since 3.3
	/// </summary>
	internal class PropertiesComputer : BSPTreeVisitor<Sphere2D>
	{

		/// <summary>
		/// Tolerance below which points are consider to be identical. </summary>
		private readonly double tolerance;

		/// <summary>
		/// Summed area. </summary>
		private double summedArea;

		/// <summary>
		/// Summed barycenter. </summary>
		private Vector3D summedBarycenter;

		/// <summary>
		/// List of points strictly inside convex cells. </summary>
		private readonly IList<Vector3D> convexCellsInsidePoints;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="tolerance"> below which points are consider to be identical </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PropertiesComputer(final double tolerance)
		public PropertiesComputer(double tolerance)
		{
			this.tolerance = tolerance;
			this.summedArea = 0;
			this.summedBarycenter = Vector3D.ZERO;
			this.convexCellsInsidePoints = new List<Vector3D>();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.partitioning.BSPTreeVisitor_Order visitOrder(final org.apache.commons.math3.geometry.partitioning.BSPTree<Sphere2D> node)
		public virtual org.apache.commons.math3.geometry.partitioning.BSPTreeVisitor_Order visitOrder(BSPTree<Sphere2D> node)
		{
			return org.apache.commons.math3.geometry.partitioning.BSPTreeVisitor_Order.MINUS_SUB_PLUS;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visitInternalNode(final org.apache.commons.math3.geometry.partitioning.BSPTree<Sphere2D> node)
		public virtual void visitInternalNode(BSPTree<Sphere2D> node)
		{
			// nothing to do here
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void visitLeafNode(final org.apache.commons.math3.geometry.partitioning.BSPTree<Sphere2D> node)
		public virtual void visitLeafNode(BSPTree<Sphere2D> node)
		{
			if ((bool?) node.Attribute)
			{

				// transform this inside leaf cell into a simple convex polygon
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SphericalPolygonsSet convex = new SphericalPolygonsSet(node.pruneAroundConvexCell(Boolean.TRUE, Boolean.FALSE, null), tolerance);
				SphericalPolygonsSet convex = new SphericalPolygonsSet(node.pruneAroundConvexCell(true, false, null), tolerance);

				// extract the start of the single loop boundary of the convex cell
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Vertex> boundary = convex.getBoundaryLoops();
				IList<Vertex> boundary = convex.BoundaryLoops;
				if (boundary.Count != 1)
				{
					// this should never happen
					throw new MathInternalError();
				}

				// compute the geometrical properties of the convex cell
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double area = convexCellArea(boundary.get(0));
				double area = convexCellArea(boundary[0]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.threed.Vector3D barycenter = convexCellBarycenter(boundary.get(0));
				Vector3D barycenter = convexCellBarycenter(boundary[0]);
				convexCellsInsidePoints.Add(barycenter);

				// add the cell contribution to the global properties
				summedArea += area;
				summedBarycenter = new Vector3D(1, summedBarycenter, area, barycenter);

			}
		}

		/// <summary>
		/// Compute convex cell area. </summary>
		/// <param name="start"> start vertex of the convex cell boundary </param>
		/// <returns> area </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double convexCellArea(final Vertex start)
		private double convexCellArea(Vertex start)
		{

			int n = 0;
			double sum = 0;

			// loop around the cell
			for (Edge e = start.Outgoing; n == 0 || e.Start != start; e = e.End.Outgoing)
			{

				// find path interior angle at vertex
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.threed.Vector3D previousPole = e.getCircle().getPole();
				Vector3D previousPole = e.Circle.Pole;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.threed.Vector3D nextPole = e.getEnd().getOutgoing().getCircle().getPole();
				Vector3D nextPole = e.End.Outgoing.Circle.Pole;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.threed.Vector3D point = e.getEnd().getLocation().getVector();
				Vector3D point = e.End.Location.Vector;
				double alpha = FastMath.atan2(Vector3D.dotProduct(nextPole, Vector3D.crossProduct(point, previousPole)), -Vector3D.dotProduct(nextPole, previousPole));
				if (alpha < 0)
				{
					alpha += MathUtils.TWO_PI;
				}
				sum += alpha;
				n++;
			}

			// compute area using extended Girard theorem
			// see Spherical Trigonometry: For the Use of Colleges and Schools by I. Todhunter
			// article 99 in chapter VIII Area Of a Spherical Triangle. Spherical Excess.
			// book available from project Gutenberg at http://www.gutenberg.org/ebooks/19770
			return sum - (n - 2) * FastMath.PI;

		}

		/// <summary>
		/// Compute convex cell barycenter. </summary>
		/// <param name="start"> start vertex of the convex cell boundary </param>
		/// <returns> barycenter </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private org.apache.commons.math3.geometry.euclidean.threed.Vector3D convexCellBarycenter(final Vertex start)
		private Vector3D convexCellBarycenter(Vertex start)
		{

			int n = 0;
			Vector3D sumB = Vector3D.ZERO;

			// loop around the cell
			for (Edge e = start.Outgoing; n == 0 || e.Start != start; e = e.End.Outgoing)
			{
				sumB = new Vector3D(1, sumB, e.Length, e.Circle.Pole);
				n++;
			}

			return sumB.normalize();

		}

		/// <summary>
		/// Get the area. </summary>
		/// <returns> area </returns>
		public virtual double Area
		{
			get
			{
				return summedArea;
			}
		}

		/// <summary>
		/// Get the barycenter. </summary>
		/// <returns> barycenter </returns>
		public virtual S2Point Barycenter
		{
			get
			{
				if (summedBarycenter.NormSq == 0)
				{
					return S2Point.NaN_Renamed;
				}
				else
				{
					return new S2Point(summedBarycenter);
				}
			}
		}

		/// <summary>
		/// Get the points strictly inside convex cells. </summary>
		/// <returns> points strictly inside convex cells </returns>
		public virtual IList<Vector3D> ConvexCellsInsidePoints
		{
			get
			{
				return convexCellsInsidePoints;
			}
		}

	}

}
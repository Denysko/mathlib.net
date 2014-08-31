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
namespace org.apache.commons.math3.geometry.euclidean.twod
{

	using BigFraction = org.apache.commons.math3.fraction.BigFraction;
	using org.apache.commons.math3.geometry.enclosing;
	using org.apache.commons.math3.geometry.enclosing;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Class generating an enclosing ball from its support points.
	/// @version $Id: DiskGenerator.java 1564921 2014-02-05 20:42:58Z luc $
	/// @since 3.3
	/// </summary>
	public class DiskGenerator : SupportBallGenerator<Euclidean2D, Vector2D>
	{

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.enclosing.EnclosingBall<Euclidean2D, Vector2D> ballOnSupport(final java.util.List<Vector2D> support)
		public virtual EnclosingBall<Euclidean2D, Vector2D> ballOnSupport(IList<Vector2D> support)
		{

			if (support.Count < 1)
			{
				return new EnclosingBall<Euclidean2D, Vector2D>(Vector2D.ZERO, double.NegativeInfinity);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D vA = support.get(0);
				Vector2D vA = support[0];
				if (support.Count < 2)
				{
					return new EnclosingBall<Euclidean2D, Vector2D>(vA, 0, vA);
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D vB = support.get(1);
					Vector2D vB = support[1];
					if (support.Count < 3)
					{
						return new EnclosingBall<Euclidean2D, Vector2D>(new Vector2D(0.5, vA, 0.5, vB), 0.5 * vA.distance(vB), vA, vB);
					}
					else
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D vC = support.get(2);
						Vector2D vC = support[2];
						// a disk is 2D can be defined as:
						// (1)   (x - x_0)^2 + (y - y_0)^2 = r^2
						// which can be written:
						// (2)   (x^2 + y^2) - 2 x_0 x - 2 y_0 y + (x_0^2 + y_0^2 - r^2) = 0
						// or simply:
						// (3)   (x^2 + y^2) + a x + b y + c = 0
						// with disk center coordinates -a/2, -b/2
						// If the disk exists, a, b and c are a non-zero solution to
						// [ (x^2  + y^2 )   x    y   1 ]   [ 1 ]   [ 0 ]
						// [ (xA^2 + yA^2)   xA   yA  1 ]   [ a ]   [ 0 ]
						// [ (xB^2 + yB^2)   xB   yB  1 ] * [ b ] = [ 0 ]
						// [ (xC^2 + yC^2)   xC   yC  1 ]   [ c ]   [ 0 ]
						// So the determinant of the matrix is zero. Computing this determinant
						// by expanding it using the minors m_ij of first row leads to
						// (4)   m_11 (x^2 + y^2) - m_12 x + m_13 y - m_14 = 0
						// So by identifying equations (2) and (4) we get the coordinates
						// of center as:
						//      x_0 = +m_12 / (2 m_11)
						//      y_0 = -m_13 / (2 m_11)
						// Note that the minors m_11, m_12 and m_13 all have the last column
						// filled with 1.0, hence simplifying the computation
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.fraction.BigFraction[] c2 = new org.apache.commons.math3.fraction.BigFraction[] { new org.apache.commons.math3.fraction.BigFraction(vA.getX()), new org.apache.commons.math3.fraction.BigFraction(vB.getX()), new org.apache.commons.math3.fraction.BigFraction(vC.getX()) };
						BigFraction[] c2 = new BigFraction[] {new BigFraction(vA.X), new BigFraction(vB.X), new BigFraction(vC.X)};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.fraction.BigFraction[] c3 = new org.apache.commons.math3.fraction.BigFraction[] { new org.apache.commons.math3.fraction.BigFraction(vA.getY()), new org.apache.commons.math3.fraction.BigFraction(vB.getY()), new org.apache.commons.math3.fraction.BigFraction(vC.getY()) };
						BigFraction[] c3 = new BigFraction[] {new BigFraction(vA.Y), new BigFraction(vB.Y), new BigFraction(vC.Y)};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.fraction.BigFraction[] c1 = new org.apache.commons.math3.fraction.BigFraction[] { c2[0].multiply(c2[0]).add(c3[0].multiply(c3[0])), c2[1].multiply(c2[1]).add(c3[1].multiply(c3[1])), c2[2].multiply(c2[2]).add(c3[2].multiply(c3[2])) };
						BigFraction[] c1 = new BigFraction[] {c2[0].multiply(c2[0]).add(c3[0].multiply(c3[0])), c2[1].multiply(c2[1]).add(c3[1].multiply(c3[1])), c2[2].multiply(c2[2]).add(c3[2].multiply(c3[2]))};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.fraction.BigFraction twoM11 = minor(c2, c3).multiply(2);
						BigFraction twoM11 = minor(c2, c3).multiply(2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.fraction.BigFraction m12 = minor(c1, c3);
						BigFraction m12 = minor(c1, c3);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.fraction.BigFraction m13 = minor(c1, c2);
						BigFraction m13 = minor(c1, c2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.fraction.BigFraction centerX = m12.divide(twoM11);
						BigFraction centerX = m12.divide(twoM11);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.fraction.BigFraction centerY = m13.divide(twoM11).negate();
						BigFraction centerY = m13.divide(twoM11).negate();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.fraction.BigFraction dx = c2[0].subtract(centerX);
						BigFraction dx = c2[0].subtract(centerX);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.fraction.BigFraction dy = c3[0].subtract(centerY);
						BigFraction dy = c3[0].subtract(centerY);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.fraction.BigFraction r2 = dx.multiply(dx).add(dy.multiply(dy));
						BigFraction r2 = dx.multiply(dx).add(dy.multiply(dy));
						return new EnclosingBall<Euclidean2D, Vector2D>(new Vector2D((double)centerX, (double)centerY), FastMath.sqrt((double)r2), vA, vB, vC);
					}
				}
			}
		}

		/// <summary>
		/// Compute a dimension 3 minor, when 3<sup>d</sup> column is known to be filled with 1.0. </summary>
		/// <param name="c1"> first column </param>
		/// <param name="c2"> second column </param>
		/// <returns> value of the minor computed has an exact fraction </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private org.apache.commons.math3.fraction.BigFraction minor(final org.apache.commons.math3.fraction.BigFraction[] c1, final org.apache.commons.math3.fraction.BigFraction[] c2)
		private BigFraction minor(BigFraction[] c1, BigFraction[] c2)
		{
			return c2[0].multiply(c1[2].subtract(c1[1])).add(c2[1].multiply(c1[0].subtract(c1[2]))).add(c2[2].multiply(c1[1].subtract(c1[0])));
		}

	}

}
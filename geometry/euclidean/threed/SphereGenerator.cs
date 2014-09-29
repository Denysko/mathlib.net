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
namespace mathlib.geometry.euclidean.threed
{


	using BigFraction = mathlib.fraction.BigFraction;
	using mathlib.geometry.enclosing;
	using mathlib.geometry.enclosing;
	using DiskGenerator = mathlib.geometry.euclidean.twod.DiskGenerator;
	using Euclidean2D = mathlib.geometry.euclidean.twod.Euclidean2D;
	using Vector2D = mathlib.geometry.euclidean.twod.Vector2D;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Class generating an enclosing ball from its support points.
	/// @version $Id: SphereGenerator.java 1564921 2014-02-05 20:42:58Z luc $
	/// @since 3.3
	/// </summary>
	public class SphereGenerator : SupportBallGenerator<Euclidean3D, Vector3D>
	{

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.geometry.enclosing.EnclosingBall<Euclidean3D, Vector3D> ballOnSupport(final java.util.List<Vector3D> support)
		public virtual EnclosingBall<Euclidean3D, Vector3D> ballOnSupport(IList<Vector3D> support)
		{

			if (support.Count < 1)
			{
				return new EnclosingBall<Euclidean3D, Vector3D>(Vector3D.ZERO, double.NegativeInfinity);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D vA = support.get(0);
				Vector3D vA = support[0];
				if (support.Count < 2)
				{
					return new EnclosingBall<Euclidean3D, Vector3D>(vA, 0, vA);
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D vB = support.get(1);
					Vector3D vB = support[1];
					if (support.Count < 3)
					{
						return new EnclosingBall<Euclidean3D, Vector3D>(new Vector3D(0.5, vA, 0.5, vB), 0.5 * vA.distance(vB), vA, vB);
					}
					else
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D vC = support.get(2);
						Vector3D vC = support[2];
						if (support.Count < 4)
						{

							// delegate to 2D disk generator
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane p = new Plane(vA, vB, vC, 1.0e-10 * (vA.getNorm1() + vB.getNorm1() + vC.getNorm1()));
							Plane p = new Plane(vA, vB, vC, 1.0e-10 * (vA.Norm1 + vB.Norm1 + vC.Norm1));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.enclosing.EnclosingBall<mathlib.geometry.euclidean.twod.Euclidean2D, mathlib.geometry.euclidean.twod.Vector2D> disk = new mathlib.geometry.euclidean.twod.DiskGenerator().ballOnSupport(java.util.Arrays.asList(p.toSubSpace(vA), p.toSubSpace(vB), p.toSubSpace(vC)));
							EnclosingBall<Euclidean2D, Vector2D> disk = (new DiskGenerator()).ballOnSupport(Arrays.asList(p.toSubSpace(vA), p.toSubSpace(vB), p.toSubSpace(vC)));

							// convert back to 3D
							return new EnclosingBall<Euclidean3D, Vector3D>(p.toSpace(disk.Center), disk.Radius, vA, vB, vC);

						}
						else
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D vD = support.get(3);
							Vector3D vD = support[3];
							// a sphere is 3D can be defined as:
							// (1)   (x - x_0)^2 + (y - y_0)^2 + (z - z_0)^2 = r^2
							// which can be written:
							// (2)   (x^2 + y^2 + z^2) - 2 x_0 x - 2 y_0 y - 2 z_0 z + (x_0^2 + y_0^2 + z_0^2 - r^2) = 0
							// or simply:
							// (3)   (x^2 + y^2 + z^2) + a x + b y + c z + d = 0
							// with sphere center coordinates -a/2, -b/2, -c/2
							// If the sphere exists, a b, c and d are a non zero solution to
							// [ (x^2  + y^2  + z^2)    x    y   z    1 ]   [ 1 ]   [ 0 ]
							// [ (xA^2 + yA^2 + zA^2)   xA   yA  zA   1 ]   [ a ]   [ 0 ]
							// [ (xB^2 + yB^2 + zB^2)   xB   yB  zB   1 ] * [ b ] = [ 0 ]
							// [ (xC^2 + yC^2 + zC^2)   xC   yC  zC   1 ]   [ c ]   [ 0 ]
							// [ (xD^2 + yD^2 + zD^2)   xD   yD  zD   1 ]   [ d ]   [ 0 ]
							// So the determinant of the matrix is zero. Computing this determinant
							// by expanding it using the minors m_ij of first row leads to
							// (4)   m_11 (x^2 + y^2 + z^2) - m_12 x + m_13 y - m_14 z + m_15 = 0
							// So by identifying equations (2) and (4) we get the coordinates
							// of center as:
							//      x_0 = +m_12 / (2 m_11)
							//      y_0 = -m_13 / (2 m_11)
							//      z_0 = +m_14 / (2 m_11)
							// Note that the minors m_11, m_12, m_13 and m_14 all have the last column
							// filled with 1.0, hence simplifying the computation
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction[] c2 = new mathlib.fraction.BigFraction[] { new mathlib.fraction.BigFraction(vA.getX()), new mathlib.fraction.BigFraction(vB.getX()), new mathlib.fraction.BigFraction(vC.getX()), new mathlib.fraction.BigFraction(vD.getX()) };
							BigFraction[] c2 = new BigFraction[] {new BigFraction(vA.X), new BigFraction(vB.X), new BigFraction(vC.X), new BigFraction(vD.X)};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction[] c3 = new mathlib.fraction.BigFraction[] { new mathlib.fraction.BigFraction(vA.getY()), new mathlib.fraction.BigFraction(vB.getY()), new mathlib.fraction.BigFraction(vC.getY()), new mathlib.fraction.BigFraction(vD.getY()) };
							BigFraction[] c3 = new BigFraction[] {new BigFraction(vA.Y), new BigFraction(vB.Y), new BigFraction(vC.Y), new BigFraction(vD.Y)};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction[] c4 = new mathlib.fraction.BigFraction[] { new mathlib.fraction.BigFraction(vA.getZ()), new mathlib.fraction.BigFraction(vB.getZ()), new mathlib.fraction.BigFraction(vC.getZ()), new mathlib.fraction.BigFraction(vD.getZ()) };
							BigFraction[] c4 = new BigFraction[] {new BigFraction(vA.Z), new BigFraction(vB.Z), new BigFraction(vC.Z), new BigFraction(vD.Z)};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction[] c1 = new mathlib.fraction.BigFraction[] { c2[0].multiply(c2[0]).add(c3[0].multiply(c3[0])).add(c4[0].multiply(c4[0])), c2[1].multiply(c2[1]).add(c3[1].multiply(c3[1])).add(c4[1].multiply(c4[1])), c2[2].multiply(c2[2]).add(c3[2].multiply(c3[2])).add(c4[2].multiply(c4[2])), c2[3].multiply(c2[3]).add(c3[3].multiply(c3[3])).add(c4[3].multiply(c4[3])) };
							BigFraction[] c1 = new BigFraction[] {c2[0].multiply(c2[0]).add(c3[0].multiply(c3[0])).add(c4[0].multiply(c4[0])), c2[1].multiply(c2[1]).add(c3[1].multiply(c3[1])).add(c4[1].multiply(c4[1])), c2[2].multiply(c2[2]).add(c3[2].multiply(c3[2])).add(c4[2].multiply(c4[2])), c2[3].multiply(c2[3]).add(c3[3].multiply(c3[3])).add(c4[3].multiply(c4[3]))};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction twoM11 = minor(c2, c3, c4).multiply(2);
							BigFraction twoM11 = minor(c2, c3, c4).multiply(2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction m12 = minor(c1, c3, c4);
							BigFraction m12 = minor(c1, c3, c4);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction m13 = minor(c1, c2, c4);
							BigFraction m13 = minor(c1, c2, c4);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction m14 = minor(c1, c2, c3);
							BigFraction m14 = minor(c1, c2, c3);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction centerX = m12.divide(twoM11);
							BigFraction centerX = m12.divide(twoM11);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction centerY = m13.divide(twoM11).negate();
							BigFraction centerY = m13.divide(twoM11).negate();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction centerZ = m14.divide(twoM11);
							BigFraction centerZ = m14.divide(twoM11);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction dx = c2[0].subtract(centerX);
							BigFraction dx = c2[0].subtract(centerX);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction dy = c3[0].subtract(centerY);
							BigFraction dy = c3[0].subtract(centerY);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction dz = c4[0].subtract(centerZ);
							BigFraction dz = c4[0].subtract(centerZ);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction r2 = dx.multiply(dx).add(dy.multiply(dy)).add(dz.multiply(dz));
							BigFraction r2 = dx.multiply(dx).add(dy.multiply(dy)).add(dz.multiply(dz));
							return new EnclosingBall<Euclidean3D, Vector3D>(new Vector3D((double)centerX, (double)centerY, (double)centerZ), FastMath.sqrt((double)r2), vA, vB, vC, vD);
						}
					}
				}
			}
		}

		/// <summary>
		/// Compute a dimension 4 minor, when 4<sup>th</sup> column is known to be filled with 1.0. </summary>
		/// <param name="c1"> first column </param>
		/// <param name="c2"> second column </param>
		/// <param name="c3"> third column </param>
		/// <returns> value of the minor computed has an exact fraction </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private mathlib.fraction.BigFraction minor(final mathlib.fraction.BigFraction[] c1, final mathlib.fraction.BigFraction[] c2, final mathlib.fraction.BigFraction[] c3)
		private BigFraction minor(BigFraction[] c1, BigFraction[] c2, BigFraction[] c3)
		{
			return c2[0].multiply(c3[1]).multiply(c1[2].subtract(c1[3])).add(c2[0].multiply(c3[2]).multiply(c1[3].subtract(c1[1]))).add(c2[0].multiply(c3[3]).multiply(c1[1].subtract(c1[2]))).add(c2[1].multiply(c3[0]).multiply(c1[3].subtract(c1[2]))).add(c2[1].multiply(c3[2]).multiply(c1[0].subtract(c1[3]))).add(c2[1].multiply(c3[3]).multiply(c1[2].subtract(c1[0]))).add(c2[2].multiply(c3[0]).multiply(c1[1].subtract(c1[3]))).add(c2[2].multiply(c3[1]).multiply(c1[3].subtract(c1[0]))).add(c2[2].multiply(c3[3]).multiply(c1[0].subtract(c1[1]))).add(c2[3].multiply(c3[0]).multiply(c1[2].subtract(c1[1]))).add(c2[3].multiply(c3[1]).multiply(c1[0].subtract(c1[2]))).add(c2[3].multiply(c3[2]).multiply(c1[1].subtract(c1[0])));
		}

	}

}
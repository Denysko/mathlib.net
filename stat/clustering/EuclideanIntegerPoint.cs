using System;
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

namespace mathlib.stat.clustering
{


	using MathArrays = mathlib.util.MathArrays;

	/// <summary>
	/// A simple implementation of <seealso cref="Clusterable"/> for points with integer coordinates.
	/// @version $Id: EuclideanIntegerPoint.java 1461871 2013-03-27 22:01:25Z tn $
	/// @since 2.0 </summary>
	/// @deprecated As of 3.2 (to be removed in 4.0),
	/// use <seealso cref="mathlib.ml.clustering.DoublePoint"/> instead 
	[Obsolete("As of 3.2 (to be removed in 4.0),"), Serializable]
	public class EuclideanIntegerPoint : Clusterable<EuclideanIntegerPoint>
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 3946024775784901369L;

		/// <summary>
		/// Point coordinates. </summary>
		private readonly int[] point;

		/// <summary>
		/// Build an instance wrapping an integer array.
		/// <p>The wrapped array is referenced, it is <em>not</em> copied.</p> </summary>
		/// <param name="point"> the n-dimensional point in integer space </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public EuclideanIntegerPoint(final int[] point)
		public EuclideanIntegerPoint(int[] point)
		{
			this.point = point;
		}

		/// <summary>
		/// Get the n-dimensional point in integer space. </summary>
		/// <returns> a reference (not a copy!) to the wrapped array </returns>
		public virtual int[] Point
		{
			get
			{
				return point;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double distanceFrom(final EuclideanIntegerPoint p)
		public virtual double distanceFrom(EuclideanIntegerPoint p)
		{
			return MathArrays.distance(point, p.Point);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public EuclideanIntegerPoint centroidOf(final java.util.Collection<EuclideanIntegerPoint> points)
		public virtual EuclideanIntegerPoint centroidOf(ICollection<EuclideanIntegerPoint> points)
		{
			int[] centroid = new int[Point.Length];
			foreach (EuclideanIntegerPoint p in points)
			{
				for (int i = 0; i < centroid.Length; i++)
				{
					centroid[i] += p.Point[i];
				}
			}
			for (int i = 0; i < centroid.Length; i++)
			{
				centroid[i] /= points.Count;
			}
			return new EuclideanIntegerPoint(centroid);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public boolean equals(final Object other)
		public override bool Equals(object other)
		{
			if (!(other is EuclideanIntegerPoint))
			{
				return false;
			}
			return Arrays.Equals(point, ((EuclideanIntegerPoint) other).point);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int GetHashCode()
		{
			return Arrays.GetHashCode(point);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 2.1
		/// </summary>
		public override string ToString()
		{
			return Arrays.ToString(point);
		}

	}

}
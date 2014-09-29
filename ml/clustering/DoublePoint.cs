using System;

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

namespace mathlib.ml.clustering
{


	/// <summary>
	/// A simple implementation of <seealso cref="Clusterable"/> for points with double coordinates.
	/// @version $Id: DoublePoint.java 1461862 2013-03-27 21:48:10Z tn $
	/// @since 3.2
	/// </summary>
	[Serializable]
	public class DoublePoint : Clusterable
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 3946024775784901369L;

		/// <summary>
		/// Point coordinates. </summary>
		private readonly double[] point;

		/// <summary>
		/// Build an instance wrapping an double array.
		/// <p>
		/// The wrapped array is referenced, it is <em>not</em> copied.
		/// </summary>
		/// <param name="point"> the n-dimensional point in double space </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DoublePoint(final double[] point)
		public DoublePoint(double[] point)
		{
			this.point = point;
		}

		/// <summary>
		/// Build an instance wrapping an integer array.
		/// <p>
		/// The wrapped array is copied to an internal double array.
		/// </summary>
		/// <param name="point"> the n-dimensional point in integer space </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DoublePoint(final int[] point)
		public DoublePoint(int[] point)
		{
			this.point = new double[point.Length];
			for (int i = 0; i < point.Length; i++)
			{
				this.point[i] = point[i];
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double[] Point
		{
			get
			{
				return point;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public boolean equals(final Object other)
		public override bool Equals(object other)
		{
			if (!(other is DoublePoint))
			{
				return false;
			}
			return Arrays.Equals(point, ((DoublePoint) other).point);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int GetHashCode()
		{
			return Arrays.GetHashCode(point);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return Arrays.ToString(point);
		}

	}

}
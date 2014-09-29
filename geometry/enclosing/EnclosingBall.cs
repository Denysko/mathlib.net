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
namespace mathlib.geometry.enclosing
{

	using mathlib.geometry;

	/// <summary>
	/// This class represents a ball enclosing some points. </summary>
	/// @param <S> Space type. </param>
	/// @param <P> Point type.
	/// @version $Id: EnclosingBall.java 1562220 2014-01-28 20:29:27Z luc $ </param>
	/// <seealso cref= Space </seealso>
	/// <seealso cref= Point </seealso>
	/// <seealso cref= Encloser
	/// @since 3.3 </seealso>
	[Serializable]
	public class EnclosingBall<S, P> where S : mathlib.geometry.Space where P : mathlib.geometry.Point<S>
	{

		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 20140126L;

		/// <summary>
		/// Center of the ball. </summary>
		private readonly P center;

		/// <summary>
		/// Radius of the ball. </summary>
		private readonly double radius;

		/// <summary>
		/// Support points used to define the ball. </summary>
		private readonly P[] support;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="center"> center of the ball </param>
		/// <param name="radius"> radius of the ball </param>
		/// <param name="support"> support points used to define the ball </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public EnclosingBall(final P center, final double radius, final P... support)
		public EnclosingBall(P center, double radius, params P[] support)
		{
			this.center = center;
			this.radius = radius;
			this.support = support.clone();
		}

		/// <summary>
		/// Get the center of the ball. </summary>
		/// <returns> center of the ball </returns>
		public virtual P Center
		{
			get
			{
				return center;
			}
		}

		/// <summary>
		/// Get the radius of the ball. </summary>
		/// <returns> radius of the ball (can be negative if the ball is empty) </returns>
		public virtual double Radius
		{
			get
			{
				return radius;
			}
		}

		/// <summary>
		/// Get the support points used to define the ball. </summary>
		/// <returns> support points used to define the ball </returns>
		public virtual P[] Support
		{
			get
			{
				return support.clone();
			}
		}

		/// <summary>
		/// Get the number of support points used to define the ball. </summary>
		/// <returns> number of support points used to define the ball </returns>
		public virtual int SupportSize
		{
			get
			{
				return support.Length;
			}
		}

		/// <summary>
		/// Check if a point is within the ball or at boundary. </summary>
		/// <param name="point"> point to test </param>
		/// <returns> true if the point is within the ball or at boundary </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean contains(final P point)
		public virtual bool contains(P point)
		{
			return point.distance(center) <= radius;
		}

		/// <summary>
		/// Check if a point is within an enlarged ball or at boundary. </summary>
		/// <param name="point"> point to test </param>
		/// <param name="margin"> margin to consider </param>
		/// <returns> true if the point is within the ball enlarged
		/// by the margin or at boundary </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean contains(final P point, final double margin)
		public virtual bool contains(P point, double margin)
		{
			return point.distance(center) <= radius + margin;
		}

	}

}
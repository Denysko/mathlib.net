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
namespace org.apache.commons.math3.geometry.euclidean.oned
{

	using Region_Location = org.apache.commons.math3.geometry.partitioning.Region_Location;


	/// <summary>
	/// This class represents a 1D interval. </summary>
	/// <seealso cref= IntervalsSet
	/// @version $Id: Interval.java 1422195 2012-12-15 06:45:18Z psteitz $
	/// @since 3.0 </seealso>
	public class Interval
	{

		/// <summary>
		/// The lower bound of the interval. </summary>
		private readonly double lower;

		/// <summary>
		/// The upper bound of the interval. </summary>
		private readonly double upper;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="lower"> lower bound of the interval </param>
		/// <param name="upper"> upper bound of the interval </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Interval(final double lower, final double upper)
		public Interval(double lower, double upper)
		{
			this.lower = lower;
			this.upper = upper;
		}

		/// <summary>
		/// Get the lower bound of the interval. </summary>
		/// <returns> lower bound of the interval
		/// @since 3.1 </returns>
		public virtual double Inf
		{
			get
			{
				return lower;
			}
		}

		/// <summary>
		/// Get the lower bound of the interval. </summary>
		/// <returns> lower bound of the interval </returns>
		/// @deprecated as of 3.1, replaced by <seealso cref="#getInf()"/> 
		[Obsolete("as of 3.1, replaced by <seealso cref="#getInf()"/>")]
		public virtual double Lower
		{
			get
			{
				return Inf;
			}
		}

		/// <summary>
		/// Get the upper bound of the interval. </summary>
		/// <returns> upper bound of the interval
		/// @since 3.1 </returns>
		public virtual double Sup
		{
			get
			{
				return upper;
			}
		}

		/// <summary>
		/// Get the upper bound of the interval. </summary>
		/// <returns> upper bound of the interval </returns>
		/// @deprecated as of 3.1, replaced by <seealso cref="#getSup()"/> 
		[Obsolete("as of 3.1, replaced by <seealso cref="#getSup()"/>")]
		public virtual double Upper
		{
			get
			{
				return Sup;
			}
		}

		/// <summary>
		/// Get the size of the interval. </summary>
		/// <returns> size of the interval
		/// @since 3.1 </returns>
		public virtual double Size
		{
			get
			{
				return upper - lower;
			}
		}

		/// <summary>
		/// Get the length of the interval. </summary>
		/// <returns> length of the interval </returns>
		/// @deprecated as of 3.1, replaced by <seealso cref="#getSize()"/> 
		[Obsolete("as of 3.1, replaced by <seealso cref="#getSize()"/>")]
		public virtual double Length
		{
			get
			{
				return Size;
			}
		}

		/// <summary>
		/// Get the barycenter of the interval. </summary>
		/// <returns> barycenter of the interval
		/// @since 3.1 </returns>
		public virtual double Barycenter
		{
			get
			{
				return 0.5 * (lower + upper);
			}
		}

		/// <summary>
		/// Get the midpoint of the interval. </summary>
		/// <returns> midpoint of the interval </returns>
		/// @deprecated as of 3.1, replaced by <seealso cref="#getBarycenter()"/> 
		[Obsolete("as of 3.1, replaced by <seealso cref="#getBarycenter()"/>")]
		public virtual double MidPoint
		{
			get
			{
				return Barycenter;
			}
		}

		/// <summary>
		/// Check a point with respect to the interval. </summary>
		/// <param name="point"> point to check </param>
		/// <param name="tolerance"> tolerance below which points are considered to
		/// belong to the boundary </param>
		/// <returns> a code representing the point status: either {@link
		/// Location#INSIDE}, <seealso cref="Location#OUTSIDE"/> or <seealso cref="Location#BOUNDARY"/>
		/// @since 3.1 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.partitioning.Region_Location checkPoint(final double point, final double tolerance)
		public virtual Region_Location checkPoint(double point, double tolerance)
		{
			if (point < lower - tolerance || point > upper + tolerance)
			{
				return Region_Location.OUTSIDE;
			}
			else if (point > lower + tolerance && point < upper - tolerance)
			{
				return Region_Location.INSIDE;
			}
			else
			{
				return Region_Location.BOUNDARY;
			}
		}

	}

}
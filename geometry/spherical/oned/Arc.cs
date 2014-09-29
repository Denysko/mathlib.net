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
namespace mathlib.geometry.spherical.oned
{

	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using Region_Location = mathlib.geometry.partitioning.Region_Location;
	using FastMath = mathlib.util.FastMath;
	using MathUtils = mathlib.util.MathUtils;
	using Precision = mathlib.util.Precision;


	/// <summary>
	/// This class represents an arc on a circle. </summary>
	/// <seealso cref= ArcsSet
	/// @version $Id: Arc.java 1554654 2014-01-01 17:30:06Z luc $
	/// @since 3.3 </seealso>
	public class Arc
	{

		/// <summary>
		/// The lower angular bound of the arc. </summary>
		private readonly double lower;

		/// <summary>
		/// The upper angular bound of the arc. </summary>
		private readonly double upper;

		/// <summary>
		/// Middle point of the arc. </summary>
		private readonly double middle;

		/// <summary>
		/// Tolerance below which angles are considered identical. </summary>
		private readonly double tolerance;

		/// <summary>
		/// Simple constructor.
		/// <p>
		/// If either {@code lower} is equals to {@code upper} or
		/// the interval exceeds \( 2 \pi \), the arc is considered
		/// to be the full circle and its initial defining boundaries
		/// will be forgotten. {@code lower} is not allowed to be
		/// greater than {@code upper} (an exception is thrown in this case).
		/// {@code lower} will be canonicalized between 0 and \( 2 \pi \), and
		/// upper shifted accordingly, so the <seealso cref="#getInf()"/> and <seealso cref="#getSup()"/>
		/// may not return the value used at instance construction.
		/// </p> </summary>
		/// <param name="lower"> lower angular bound of the arc </param>
		/// <param name="upper"> upper angular bound of the arc </param>
		/// <param name="tolerance"> tolerance below which angles are considered identical </param>
		/// <exception cref="NumberIsTooLargeException"> if lower is greater than upper </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Arc(final double lower, final double upper, final double tolerance) throws mathlib.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public Arc(double lower, double upper, double tolerance)
		{
			this.tolerance = tolerance;
			if (Precision.Equals(lower, upper, 0) || (upper - lower) >= MathUtils.TWO_PI)
			{
				// the arc must cover the whole circle
				this.lower = 0;
				this.upper = MathUtils.TWO_PI;
				this.middle = FastMath.PI;
			}
			else if (lower <= upper)
			{
				this.lower = MathUtils.normalizeAngle(lower, FastMath.PI);
				this.upper = this.lower + (upper - lower);
				this.middle = 0.5 * (this.lower + this.upper);
			}
			else
			{
				throw new NumberIsTooLargeException(LocalizedFormats.ENDPOINTS_NOT_AN_INTERVAL, lower, upper, true);
			}
		}

		/// <summary>
		/// Get the lower angular bound of the arc. </summary>
		/// <returns> lower angular bound of the arc,
		/// always between 0 and \( 2 \pi \) </returns>
		public virtual double Inf
		{
			get
			{
				return lower;
			}
		}

		/// <summary>
		/// Get the upper angular bound of the arc. </summary>
		/// <returns> upper angular bound of the arc,
		/// always between <seealso cref="#getInf()"/> and <seealso cref="#getInf()"/> \( + 2 \pi \) </returns>
		public virtual double Sup
		{
			get
			{
				return upper;
			}
		}

		/// <summary>
		/// Get the angular size of the arc. </summary>
		/// <returns> angular size of the arc </returns>
		public virtual double Size
		{
			get
			{
				return upper - lower;
			}
		}

		/// <summary>
		/// Get the barycenter of the arc. </summary>
		/// <returns> barycenter of the arc </returns>
		public virtual double Barycenter
		{
			get
			{
				return middle;
			}
		}

		/// <summary>
		/// Get the tolerance below which angles are considered identical. </summary>
		/// <returns> tolerance below which angles are considered identical </returns>
		public virtual double Tolerance
		{
			get
			{
				return tolerance;
			}
		}

		/// <summary>
		/// Check a point with respect to the arc. </summary>
		/// <param name="point"> point to check </param>
		/// <returns> a code representing the point status: either {@link
		/// Location#INSIDE}, <seealso cref="Location#OUTSIDE"/> or <seealso cref="Location#BOUNDARY"/> </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.geometry.partitioning.Region_Location checkPoint(final double point)
		public virtual Region_Location checkPoint(double point)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double normalizedPoint = mathlib.util.MathUtils.normalizeAngle(point, middle);
			double normalizedPoint = MathUtils.normalizeAngle(point, middle);
			if (normalizedPoint < lower - tolerance || normalizedPoint > upper + tolerance)
			{
				return Region_Location.OUTSIDE;
			}
			else if (normalizedPoint > lower + tolerance && normalizedPoint < upper - tolerance)
			{
				return Region_Location.INSIDE;
			}
			else
			{
				return (Size >= MathUtils.TWO_PI - tolerance) ? Region_Location.INSIDE : Region_Location.BOUNDARY;
			}
		}

	}

}
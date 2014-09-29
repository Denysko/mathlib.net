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
namespace mathlib.optim
{

	using mathlib.util;

	/// <summary>
	/// This class holds a point and the vectorial value of an objective function at
	/// that point.
	/// </summary>
	/// <seealso cref= PointValuePair </seealso>
	/// <seealso cref= mathlib.analysis.MultivariateVectorFunction
	/// @version $Id: PointVectorValuePair.java 1435539 2013-01-19 13:27:24Z tn $
	/// @since 3.0 </seealso>
	[Serializable]
	public class PointVectorValuePair : Pair<double[], double[]>
	{
		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 20120513L;

		/// <summary>
		/// Builds a point/objective function value pair.
		/// </summary>
		/// <param name="point"> Point coordinates. This instance will store
		/// a copy of the array, not the array passed as argument. </param>
		/// <param name="value"> Value of the objective function at the point. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PointVectorValuePair(final double[] point, final double[] value)
		public PointVectorValuePair(double[] point, double[] value) : this(point, value, true)
		{
		}

		/// <summary>
		/// Build a point/objective function value pair.
		/// </summary>
		/// <param name="point"> Point coordinates. </param>
		/// <param name="value"> Value of the objective function at the point. </param>
		/// <param name="copyArray"> if {@code true}, the input arrays will be copied,
		/// otherwise they will be referenced. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public PointVectorValuePair(final double[] point, final double[] value, final boolean copyArray)
		public PointVectorValuePair(double[] point, double[] value, bool copyArray) : base(copyArray ? ((point == null) ? null : point.clone()) : point, copyArray ? ((value == null) ? null : value.clone()) : value)
		{
		}

		/// <summary>
		/// Gets the point.
		/// </summary>
		/// <returns> a copy of the stored point. </returns>
		public virtual double[] Point
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[] p = getKey();
				double[] p = Key;
				return p == null ? null : p.clone();
			}
		}

		/// <summary>
		/// Gets a reference to the point.
		/// </summary>
		/// <returns> a reference to the internal array storing the point. </returns>
		public virtual double[] PointRef
		{
			get
			{
				return Key;
			}
		}

		/// <summary>
		/// Gets the value of the objective function.
		/// </summary>
		/// <returns> a copy of the stored value of the objective function. </returns>
		public override double[] Value
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[] v = base.getValue();
				double[] v = base.Value;
				return v == null ? null : v.clone();
			}
		}

		/// <summary>
		/// Gets a reference to the value of the objective function.
		/// </summary>
		/// <returns> a reference to the internal array storing the value of
		/// the objective function. </returns>
		public virtual double[] ValueRef
		{
			get
			{
				return base.Value;
			}
		}

		/// <summary>
		/// Replace the instance with a data transfer object for serialization. </summary>
		/// <returns> data transfer object that will be serialized </returns>
		private object writeReplace()
		{
			return new DataTransferObject(Key, Value);
		}

		/// <summary>
		/// Internal class used only for serialization. </summary>
		[Serializable]
		private class DataTransferObject
		{
			/// <summary>
			/// Serializable UID. </summary>
			internal const long serialVersionUID = 20120513L;
			/// <summary>
			/// Point coordinates.
			/// @Serial
			/// </summary>
			internal readonly double[] point;
			/// <summary>
			/// Value of the objective function at the point.
			/// @Serial
			/// </summary>
			internal readonly double[] value;

			/// <summary>
			/// Simple constructor. </summary>
			/// <param name="point"> Point coordinates. </param>
			/// <param name="value"> Value of the objective function at the point. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DataTransferObject(final double[] point, final double[] value)
			public DataTransferObject(double[] point, double[] value)
			{
				this.point = point.clone();
				this.value = value.clone();
			}

			/// <summary>
			/// Replace the deserialized data transfer object with a <seealso cref="PointValuePair"/>. </summary>
			/// <returns> replacement <seealso cref="PointValuePair"/> </returns>
			internal virtual object readResolve()
			{
				return new PointVectorValuePair(point, value, false);
			}
		}
	}

}
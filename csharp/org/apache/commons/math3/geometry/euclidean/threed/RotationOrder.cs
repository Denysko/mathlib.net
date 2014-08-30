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

namespace org.apache.commons.math3.geometry.euclidean.threed
{

	/// <summary>
	/// This class is a utility representing a rotation order specification
	/// for Cardan or Euler angles specification.
	/// 
	/// This class cannot be instanciated by the user. He can only use one
	/// of the twelve predefined supported orders as an argument to either
	/// the <seealso cref="Rotation#Rotation(RotationOrder,double,double,double)"/>
	/// constructor or the <seealso cref="Rotation#getAngles"/> method.
	/// 
	/// @version $Id: RotationOrder.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2
	/// </summary>
	public sealed class RotationOrder
	{

		/// <summary>
		/// Set of Cardan angles.
		/// this ordered set of rotations is around X, then around Y, then
		/// around Z
		/// </summary>
		public static readonly RotationOrder XYZ = new RotationOrder("XYZ", Vector3D.PLUS_I, Vector3D.PLUS_J, Vector3D.PLUS_K);

		/// <summary>
		/// Set of Cardan angles.
		/// this ordered set of rotations is around X, then around Z, then
		/// around Y
		/// </summary>
		public static readonly RotationOrder XZY = new RotationOrder("XZY", Vector3D.PLUS_I, Vector3D.PLUS_K, Vector3D.PLUS_J);

		/// <summary>
		/// Set of Cardan angles.
		/// this ordered set of rotations is around Y, then around X, then
		/// around Z
		/// </summary>
		public static readonly RotationOrder YXZ = new RotationOrder("YXZ", Vector3D.PLUS_J, Vector3D.PLUS_I, Vector3D.PLUS_K);

		/// <summary>
		/// Set of Cardan angles.
		/// this ordered set of rotations is around Y, then around Z, then
		/// around X
		/// </summary>
		public static readonly RotationOrder YZX = new RotationOrder("YZX", Vector3D.PLUS_J, Vector3D.PLUS_K, Vector3D.PLUS_I);

		/// <summary>
		/// Set of Cardan angles.
		/// this ordered set of rotations is around Z, then around X, then
		/// around Y
		/// </summary>
		public static readonly RotationOrder ZXY = new RotationOrder("ZXY", Vector3D.PLUS_K, Vector3D.PLUS_I, Vector3D.PLUS_J);

		/// <summary>
		/// Set of Cardan angles.
		/// this ordered set of rotations is around Z, then around Y, then
		/// around X
		/// </summary>
		public static readonly RotationOrder ZYX = new RotationOrder("ZYX", Vector3D.PLUS_K, Vector3D.PLUS_J, Vector3D.PLUS_I);

		/// <summary>
		/// Set of Euler angles.
		/// this ordered set of rotations is around X, then around Y, then
		/// around X
		/// </summary>
		public static readonly RotationOrder XYX = new RotationOrder("XYX", Vector3D.PLUS_I, Vector3D.PLUS_J, Vector3D.PLUS_I);

		/// <summary>
		/// Set of Euler angles.
		/// this ordered set of rotations is around X, then around Z, then
		/// around X
		/// </summary>
		public static readonly RotationOrder XZX = new RotationOrder("XZX", Vector3D.PLUS_I, Vector3D.PLUS_K, Vector3D.PLUS_I);

		/// <summary>
		/// Set of Euler angles.
		/// this ordered set of rotations is around Y, then around X, then
		/// around Y
		/// </summary>
		public static readonly RotationOrder YXY = new RotationOrder("YXY", Vector3D.PLUS_J, Vector3D.PLUS_I, Vector3D.PLUS_J);

		/// <summary>
		/// Set of Euler angles.
		/// this ordered set of rotations is around Y, then around Z, then
		/// around Y
		/// </summary>
		public static readonly RotationOrder YZY = new RotationOrder("YZY", Vector3D.PLUS_J, Vector3D.PLUS_K, Vector3D.PLUS_J);

		/// <summary>
		/// Set of Euler angles.
		/// this ordered set of rotations is around Z, then around X, then
		/// around Z
		/// </summary>
		public static readonly RotationOrder ZXZ = new RotationOrder("ZXZ", Vector3D.PLUS_K, Vector3D.PLUS_I, Vector3D.PLUS_K);

		/// <summary>
		/// Set of Euler angles.
		/// this ordered set of rotations is around Z, then around Y, then
		/// around Z
		/// </summary>
		public static readonly RotationOrder ZYZ = new RotationOrder("ZYZ", Vector3D.PLUS_K, Vector3D.PLUS_J, Vector3D.PLUS_K);

		/// <summary>
		/// Name of the rotations order. </summary>
		private readonly string name;

		/// <summary>
		/// Axis of the first rotation. </summary>
		private readonly Vector3D a1;

		/// <summary>
		/// Axis of the second rotation. </summary>
		private readonly Vector3D a2;

		/// <summary>
		/// Axis of the third rotation. </summary>
		private readonly Vector3D a3;

		/// <summary>
		/// Private constructor.
		/// This is a utility class that cannot be instantiated by the user,
		/// so its only constructor is private. </summary>
		/// <param name="name"> name of the rotation order </param>
		/// <param name="a1"> axis of the first rotation </param>
		/// <param name="a2"> axis of the second rotation </param>
		/// <param name="a3"> axis of the third rotation </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private RotationOrder(final String name, final Vector3D a1, final Vector3D a2, final Vector3D a3)
		private RotationOrder(string name, Vector3D a1, Vector3D a2, Vector3D a3)
		{
			this.name = name;
			this.a1 = a1;
			this.a2 = a2;
			this.a3 = a3;
		}

		/// <summary>
		/// Get a string representation of the instance. </summary>
		/// <returns> a string representation of the instance (in fact, its name) </returns>
		public override string ToString()
		{
			return name;
		}

		/// <summary>
		/// Get the axis of the first rotation. </summary>
		/// <returns> axis of the first rotation </returns>
		public Vector3D A1
		{
			get
			{
				return a1;
			}
		}

		/// <summary>
		/// Get the axis of the second rotation. </summary>
		/// <returns> axis of the second rotation </returns>
		public Vector3D A2
		{
			get
			{
				return a2;
			}
		}

		/// <summary>
		/// Get the axis of the second rotation. </summary>
		/// <returns> axis of the second rotation </returns>
		public Vector3D A3
		{
			get
			{
				return a3;
			}
		}

	}

}
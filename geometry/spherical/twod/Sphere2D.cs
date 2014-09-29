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

namespace mathlib.geometry.spherical.twod
{

	using Sphere1D = mathlib.geometry.spherical.oned.Sphere1D;

	/// <summary>
	/// This class implements a two-dimensional sphere (i.e. the regular sphere).
	/// <p>
	/// We use here the topologists definition of the 2-sphere (see
	/// <a href="http://mathworld.wolfram.com/Sphere.html">Sphere</a> on
	/// MathWorld), i.e. the 2-sphere is the two-dimensional surface
	/// defined in 3D as x<sup>2</sup>+y<sup>2</sup>+z<sup>2</sup>=1.
	/// </p>
	/// @version $Id: Sphere2D.java 1554648 2014-01-01 17:24:37Z luc $
	/// @since 3.3
	/// </summary>
	[Serializable]
	public class Sphere2D : Space
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 20131218L;

		/// <summary>
		/// Private constructor for the singleton.
		/// </summary>
		private Sphere2D()
		{
		}

		/// <summary>
		/// Get the unique instance. </summary>
		/// <returns> the unique instance </returns>
		public static Sphere2D Instance
		{
			get
			{
				return LazyHolder.INSTANCE;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int Dimension
		{
			get
			{
				return 2;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Sphere1D SubSpace
		{
			get
			{
				return Sphere1D.Instance;
			}
		}

		// CHECKSTYLE: stop HideUtilityClassConstructor
		/// <summary>
		/// Holder for the instance.
		/// <p>We use here the Initialization On Demand Holder Idiom.</p>
		/// </summary>
		private class LazyHolder
		{
			/// <summary>
			/// Cached field instance. </summary>
			internal static readonly Sphere2D INSTANCE = new Sphere2D();
		}
		// CHECKSTYLE: resume HideUtilityClassConstructor

		/// <summary>
		/// Handle deserialization of the singleton. </summary>
		/// <returns> the singleton instance </returns>
		private object readResolve()
		{
			// return the singleton instance
			return LazyHolder.INSTANCE;
		}

	}

}
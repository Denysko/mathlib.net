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

namespace org.apache.commons.math3.geometry.euclidean.twod
{

	using Euclidean1D = org.apache.commons.math3.geometry.euclidean.oned.Euclidean1D;

	/// <summary>
	/// This class implements a two-dimensional space.
	/// @version $Id: Euclidean2D.java 1562610 2014-01-29 22:17:16Z tn $
	/// @since 3.0
	/// </summary>
	[Serializable]
	public class Euclidean2D : Space
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 4793432849757649566L;

		/// <summary>
		/// Private constructor for the singleton.
		/// </summary>
		private Euclidean2D()
		{
		}

		/// <summary>
		/// Get the unique instance. </summary>
		/// <returns> the unique instance </returns>
		public static Euclidean2D Instance
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
		public virtual Euclidean1D SubSpace
		{
			get
			{
				return Euclidean1D.Instance;
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
			internal static readonly Euclidean2D INSTANCE = new Euclidean2D();
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
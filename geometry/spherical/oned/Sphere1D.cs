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

namespace org.apache.commons.math3.geometry.spherical.oned
{

	using MathUnsupportedOperationException = org.apache.commons.math3.exception.MathUnsupportedOperationException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// This class implements a one-dimensional sphere (i.e. a circle).
	/// <p>
	/// We use here the topologists definition of the 1-sphere (see
	/// <a href="http://mathworld.wolfram.com/Sphere.html">Sphere</a> on
	/// MathWorld), i.e. the 1-sphere is the one-dimensional closed curve
	/// defined in 2D as x<sup>2</sup>+y<sup>2</sup>=1.
	/// </p>
	/// @version $Id: Sphere1D.java 1571737 2014-02-25 16:42:07Z luc $
	/// @since 3.3
	/// </summary>
	[Serializable]
	public class Sphere1D : Space
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 20131218L;

		/// <summary>
		/// Private constructor for the singleton.
		/// </summary>
		private Sphere1D()
		{
		}

		/// <summary>
		/// Get the unique instance. </summary>
		/// <returns> the unique instance </returns>
		public static Sphere1D Instance
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
				return 1;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// As the 1-dimension sphere does not have proper sub-spaces,
		/// this method always throws a <seealso cref="NoSubSpaceException"/>
		/// </p> </summary>
		/// <returns> nothing </returns>
		/// <exception cref="NoSubSpaceException"> in all cases </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.Space getSubSpace() throws NoSubSpaceException
		public virtual Space SubSpace
		{
			get
			{
				throw new NoSubSpaceException();
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
			internal static readonly Sphere1D INSTANCE = new Sphere1D();
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

		/// <summary>
		/// Specialized exception for inexistent sub-space.
		/// <p>
		/// This exception is thrown when attempting to get the sub-space of a one-dimensional space
		/// </p>
		/// </summary>
		public class NoSubSpaceException : MathUnsupportedOperationException
		{

			/// <summary>
			/// Serializable UID. </summary>
			internal const long serialVersionUID = 20140225L;

			/// <summary>
			/// Simple constructor.
			/// </summary>
			public NoSubSpaceException() : base(LocalizedFormats.NOT_SUPPORTED_IN_DIMENSION_N, 1)
			{
			}

		}

	}

}
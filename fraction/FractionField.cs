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

namespace mathlib.fraction
{

	using mathlib;
	using mathlib;

	/// <summary>
	/// Representation of the fractional numbers field.
	/// <p>
	/// This class is a singleton.
	/// </p> </summary>
	/// <seealso cref= Fraction
	/// @version $Id: FractionField.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0 </seealso>
	[Serializable]
	public class FractionField : Field<Fraction>
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -1257768487499119313L;

		/// <summary>
		/// Private constructor for the singleton.
		/// </summary>
		private FractionField()
		{
		}

		/// <summary>
		/// Get the unique instance. </summary>
		/// <returns> the unique instance </returns>
		public static FractionField Instance
		{
			get
			{
				return LazyHolder.INSTANCE;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Fraction One
		{
			get
			{
				return Fraction.ONE;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Fraction Zero
		{
			get
			{
				return Fraction.ZERO;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Type RuntimeClass
		{
			get
			{
				return typeof(Fraction);
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
			internal static readonly FractionField INSTANCE = new FractionField();
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
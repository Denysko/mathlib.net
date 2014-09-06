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
namespace mathlib.util
{

    using mathlib;

	/// <summary>
	/// The field of double precision floating-point numbers.
	/// 
	/// @since 3.1
	/// @version $Id: Decimal64Field.java 1306177 2012-03-28 05:40:46Z celestin $ </summary>
	/// <seealso cref= Decimal64 </seealso>
	public class Decimal64Field : Field<Decimal64>
	{

		/// <summary>
		/// The unique instance of this class. </summary>
		private static readonly Decimal64Field INSTANCE = new Decimal64Field();

		/// <summary>
		/// Default constructor. </summary>
		private Decimal64Field()
		{
			// Do nothing
		}

		/// <summary>
		/// Returns the unique instance of this class.
		/// </summary>
		/// <returns> the unique instance of this class </returns>
		public static Decimal64Field Instance
		{
			get
			{
				return INSTANCE;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Decimal64 Zero
		{
			get
			{
				return Decimal64.ZERO;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Decimal64 One
		{
			get
			{
				return Decimal64.ONE;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Type RuntimeClass
		{
			get
			{
				return typeof(Decimal64);
			}
		}
	}

}
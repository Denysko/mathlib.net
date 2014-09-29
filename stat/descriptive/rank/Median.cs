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
namespace mathlib.stat.descriptive.rank
{

	using NullArgumentException = mathlib.exception.NullArgumentException;


	/// <summary>
	/// Returns the median of the available values.  This is the same as the 50th percentile.
	/// See <seealso cref="Percentile"/> for a description of the algorithm used.
	/// <p>
	/// <strong>Note that this implementation is not synchronized.</strong> If
	/// multiple threads access an instance of this class concurrently, and at least
	/// one of the threads invokes the <code>increment()</code> or
	/// <code>clear()</code> method, it must be synchronized externally.</p>
	/// 
	/// @version $Id: Median.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class Median : Percentile
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -3961477041290915687L;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Median() : base(50.0)
		{
			// No try-catch or advertised exception - arg is valid
		}

		/// <summary>
		/// Copy constructor, creates a new {@code Median} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code Median} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Median(Median original) throws mathlib.exception.NullArgumentException
		public Median(Median original) : base(original)
		{
		}

	}

}
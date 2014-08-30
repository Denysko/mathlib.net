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

namespace org.apache.commons.math3.ode.events
{

	using FastMath = org.apache.commons.math3.util.FastMath;
	using Precision = org.apache.commons.math3.util.Precision;


	/// <summary>
	/// Transformer for <seealso cref="EventHandler#g(double, double[]) g functions"/>. </summary>
	/// <seealso cref= EventFilter </seealso>
	/// <seealso cref= FilterType
	/// @version $Id: Transformer.java 1503290 2013-07-15 15:16:29Z sebb $
	/// @since 3.2 </seealso>
	internal enum Transformer
	{

		/// <summary>
		/// Transformer computing transformed = 0.
		/// <p>
		/// This transformer is used when we initialize the filter, until we get at
		/// least one non-zero value to select the proper transformer.
		/// </p>
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		UNINITIALIZED
		{
			/// <summary>
			///  {@inheritDoc} </summary>
		},

		/// <summary>
		/// Transformer computing transformed = g.
		/// <p>
		/// When this transformer is applied, the roots of the original function
		/// are preserved, with the same {@code increasing/decreasing} status.
		/// </p>
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		PLUS
		{
			/// <summary>
			///  {@inheritDoc} </summary>
		},

		/// <summary>
		/// Transformer computing transformed = -g.
		/// <p>
		/// When this transformer is applied, the roots of the original function
		/// are preserved, with reversed {@code increasing/decreasing} status.
		/// </p>
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		MINUS
		{
			/// <summary>
			///  {@inheritDoc} </summary>
		},

		/// <summary>
		/// Transformer computing transformed = min(-<seealso cref="Precision#SAFE_MIN"/>, -g, +g).
		/// <p>
		/// When this transformer is applied, the transformed function is
		/// guaranteed to be always strictly negative (i.e. there are no roots).
		/// </p>
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		MIN
		{
			/// <summary>
			///  {@inheritDoc} </summary>
		},

		/// <summary>
		/// Transformer computing transformed = max(+<seealso cref="Precision#SAFE_MIN"/>, -g, +g).
		/// <p>
		/// When this transformer is applied, the transformed function is
		/// guaranteed to be always strictly positive (i.e. there are no roots).
		/// </p>
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		MAX
		{
			/// <summary>
			///  {@inheritDoc} </summary>
		}

		/// <summary>
		/// Transform value of function g. </summary>
		/// <param name="g"> raw value of function g </param>
		/// <returns> transformed value of function g </returns>
		protected = double g

	}

}
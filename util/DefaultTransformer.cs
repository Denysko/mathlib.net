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

    using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
    using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
    using NullArgumentException = mathlib.exception.NullArgumentException;

	/// <summary>
	/// A Default NumberTransformer for java.lang.Numbers and Numeric Strings. This
	/// provides some simple conversion capabilities to turn any java.lang.Number
	/// into a primitive double or to turn a String representation of a Number into
	/// a double.
	/// 
	/// @version $Id: DefaultTransformer.java 1517416 2013-08-26 03:04:38Z dbrosius $
	/// </summary>
	[Serializable]
	public class DefaultTransformer : NumberTransformer
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 4019938025047800455L;

		/// <param name="o">  the object that gets transformed. </param>
		/// <returns> a double primitive representation of the Object o. </returns>
		/// <exception cref="NullArgumentException"> if Object <code>o</code> is {@code null}. </exception>
		/// <exception cref="MathIllegalArgumentException"> if Object <code>o</code>
		/// cannot successfully be transformed </exception>
		/// <seealso cref= <a href="http://commons.apache.org/collections/api-release/org/apache/commons/collections/Transformer.html">Commons Collections Transformer</a> </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double transform(Object o) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.MathIllegalArgumentException
		public virtual double transform(object o)
		{

			if (o == null)
			{
				throw new NullArgumentException(LocalizedFormats.OBJECT_TRANSFORMATION);
			}

			if (o is Number)
			{
				return (double)((Number)o);
			}

			try
			{
				return Convert.ToDouble(o.ToString());
			}
			catch (NumberFormatException e)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.CANNOT_TRANSFORM_TO_DOUBLE, o.ToString());
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool Equals(object other)
		{
			if (this == other)
			{
				return true;
			}
			return other is DefaultTransformer;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int GetHashCode()
		{
			// some arbitrary number ...
			return 401993047;
		}

	}

}
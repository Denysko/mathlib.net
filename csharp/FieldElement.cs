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
namespace org.apache.commons.math3
{

	using MathArithmeticException = org.apache.commons.math3.exception.MathArithmeticException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;


	/// <summary>
	/// Interface representing <a href="http://mathworld.wolfram.com/Field.html">field</a> elements. </summary>
	/// @param <T> the type of the field elements </param>
	/// <seealso cref= Field
	/// @version $Id: FieldElement.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0 </seealso>
	public interface FieldElement<T>
	{

		/// <summary>
		/// Compute this + a. </summary>
		/// <param name="a"> element to add </param>
		/// <returns> a new element representing this + a </returns>
		/// <exception cref="NullArgumentException"> if {@code addend} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T add(T a) throws org.apache.commons.math3.exception.NullArgumentException;
		T add(T a);

		/// <summary>
		/// Compute this - a. </summary>
		/// <param name="a"> element to subtract </param>
		/// <returns> a new element representing this - a </returns>
		/// <exception cref="NullArgumentException"> if {@code a} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T subtract(T a) throws org.apache.commons.math3.exception.NullArgumentException;
		T subtract(T a);

		/// <summary>
		/// Returns the additive inverse of {@code this} element. </summary>
		/// <returns> the opposite of {@code this}. </returns>
		T negate();

		/// <summary>
		/// Compute n &times; this. Multiplication by an integer number is defined
		/// as the following sum
		/// <center>
		/// n &times; this = &sum;<sub>i=1</sub><sup>n</sup> this.
		/// </center> </summary>
		/// <param name="n"> Number of times {@code this} must be added to itself. </param>
		/// <returns> A new element representing n &times; this. </returns>
		T multiply(int n);

		/// <summary>
		/// Compute this &times; a. </summary>
		/// <param name="a"> element to multiply </param>
		/// <returns> a new element representing this &times; a </returns>
		/// <exception cref="NullArgumentException"> if {@code a} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T multiply(T a) throws org.apache.commons.math3.exception.NullArgumentException;
		T multiply(T a);

		/// <summary>
		/// Compute this &divide; a. </summary>
		/// <param name="a"> element to add </param>
		/// <returns> a new element representing this &divide; a </returns>
		/// <exception cref="NullArgumentException"> if {@code a} is {@code null}. </exception>
		/// <exception cref="MathArithmeticException"> if {@code a} is zero </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T divide(T a) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.MathArithmeticException;
		T divide(T a);

		/// <summary>
		/// Returns the multiplicative inverse of {@code this} element. </summary>
		/// <returns> the inverse of {@code this}. </returns>
		/// <exception cref="MathArithmeticException"> if {@code this} is zero </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: T reciprocal() throws org.apache.commons.math3.exception.MathArithmeticException;
		T reciprocal();

		/// <summary>
		/// Get the <seealso cref="Field"/> to which the instance belongs. </summary>
		/// <returns> <seealso cref="Field"/> to which the instance belongs </returns>
		Field<T> Field {get;}
	}

}
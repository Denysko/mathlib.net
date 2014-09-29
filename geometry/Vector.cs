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
namespace mathlib.geometry
{

	using MathArithmeticException = mathlib.exception.MathArithmeticException;

	/// <summary>
	/// This interface represents a generic vector in a vectorial space or a point in an affine space. </summary>
	/// @param <S> Type of the space.
	/// @version $Id: Vector.java 1555175 2014-01-03 18:07:22Z luc $ </param>
	/// <seealso cref= Space </seealso>
	/// <seealso cref= Point
	/// @since 3.0 </seealso>
	public interface Vector<S> : Point<S> where S : Space
	{

		/// <summary>
		/// Get the null vector of the vectorial space or origin point of the affine space. </summary>
		/// <returns> null vector of the vectorial space or origin point of the affine space </returns>
		Vector<S> Zero {get;}

		/// <summary>
		/// Get the L<sub>1</sub> norm for the vector. </summary>
		/// <returns> L<sub>1</sub> norm for the vector </returns>
		double Norm1 {get;}

		/// <summary>
		/// Get the L<sub>2</sub> norm for the vector. </summary>
		/// <returns> Euclidean norm for the vector </returns>
		double Norm {get;}

		/// <summary>
		/// Get the square of the norm for the vector. </summary>
		/// <returns> square of the Euclidean norm for the vector </returns>
		double NormSq {get;}

		/// <summary>
		/// Get the L<sub>&infin;</sub> norm for the vector. </summary>
		/// <returns> L<sub>&infin;</sub> norm for the vector </returns>
		double NormInf {get;}

		/// <summary>
		/// Add a vector to the instance. </summary>
		/// <param name="v"> vector to add </param>
		/// <returns> a new vector </returns>
		Vector<S> add(Vector<S> v);

		/// <summary>
		/// Add a scaled vector to the instance. </summary>
		/// <param name="factor"> scale factor to apply to v before adding it </param>
		/// <param name="v"> vector to add </param>
		/// <returns> a new vector </returns>
		Vector<S> add(double factor, Vector<S> v);

		/// <summary>
		/// Subtract a vector from the instance. </summary>
		/// <param name="v"> vector to subtract </param>
		/// <returns> a new vector </returns>
		Vector<S> subtract(Vector<S> v);

		/// <summary>
		/// Subtract a scaled vector from the instance. </summary>
		/// <param name="factor"> scale factor to apply to v before subtracting it </param>
		/// <param name="v"> vector to subtract </param>
		/// <returns> a new vector </returns>
		Vector<S> subtract(double factor, Vector<S> v);

		/// <summary>
		/// Get the opposite of the instance. </summary>
		/// <returns> a new vector which is opposite to the instance </returns>
		Vector<S> negate();

		/// <summary>
		/// Get a normalized vector aligned with the instance. </summary>
		/// <returns> a new normalized vector </returns>
		/// <exception cref="MathArithmeticException"> if the norm is zero </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Vector<S> normalize() throws mathlib.exception.MathArithmeticException;
		Vector<S> normalize();

		/// <summary>
		/// Multiply the instance by a scalar. </summary>
		/// <param name="a"> scalar </param>
		/// <returns> a new vector </returns>
		Vector<S> scalarMultiply(double a);

		/// <summary>
		/// Returns true if any coordinate of this vector is infinite and none are NaN;
		/// false otherwise </summary>
		/// <returns>  true if any coordinate of this vector is infinite and none are NaN;
		/// false otherwise </returns>
		bool Infinite {get;}

		/// <summary>
		/// Compute the distance between the instance and another vector according to the L<sub>1</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>q.subtract(p).getNorm1()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v"> second vector </param>
		/// <returns> the distance between the instance and p according to the L<sub>1</sub> norm </returns>
		double distance1(Vector<S> v);

		/// <summary>
		/// Compute the distance between the instance and another vector according to the L<sub>2</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>q.subtract(p).getNorm()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v"> second vector </param>
		/// <returns> the distance between the instance and p according to the L<sub>2</sub> norm </returns>
		double distance(Vector<S> v);

		/// <summary>
		/// Compute the distance between the instance and another vector according to the L<sub>&infin;</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>q.subtract(p).getNormInf()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v"> second vector </param>
		/// <returns> the distance between the instance and p according to the L<sub>&infin;</sub> norm </returns>
		double distanceInf(Vector<S> v);

		/// <summary>
		/// Compute the square of the distance between the instance and another vector.
		/// <p>Calling this method is equivalent to calling:
		/// <code>q.subtract(p).getNormSq()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v"> second vector </param>
		/// <returns> the square of the distance between the instance and p </returns>
		double distanceSq(Vector<S> v);

		/// <summary>
		/// Compute the dot-product of the instance and another vector. </summary>
		/// <param name="v"> second vector </param>
		/// <returns> the dot product this.v </returns>
		double dotProduct(Vector<S> v);

		/// <summary>
		/// Get a string representation of this vector. </summary>
		/// <param name="format"> the custom format for components </param>
		/// <returns> a string representation of this vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: String toString(final java.text.NumberFormat format);
		string ToString(NumberFormat format);

	}

}
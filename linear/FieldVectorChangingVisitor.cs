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
namespace mathlib.linear
{

	using mathlib;

	/// <summary>
	/// This interface defines a visitor for the entries of a vector. Visitors
	/// implementing this interface may alter the entries of the vector being
	/// visited.
	/// </summary>
	/// @param <T> the type of the field elements
	/// @version $Id: FieldVectorChangingVisitor.java 1570536 2014-02-21 11:26:09Z luc $
	/// @since 3.3 </param>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public interface FieldVectorChangingVisitor<T extends mathlib.FieldElement<?>>
	public interface FieldVectorChangingVisitor<T> where T : mathlib.FieldElement<?>
	{
		/// <summary>
		/// Start visiting a vector. This method is called once, before any entry
		/// of the vector is visited.
		/// </summary>
		/// <param name="dimension"> the size of the vector </param>
		/// <param name="start"> the index of the first entry to be visited </param>
		/// <param name="end"> the index of the last entry to be visited (inclusive) </param>
		void start(int dimension, int start, int end);

		/// <summary>
		/// Visit one entry of the vector.
		/// </summary>
		/// <param name="index"> the index of the entry being visited </param>
		/// <param name="value"> the value of the entry being visited </param>
		/// <returns> the new value of the entry being visited </returns>
		T visit(int index, T value);

		/// <summary>
		/// End visiting a vector. This method is called once, after all entries of
		/// the vector have been visited.
		/// </summary>
		/// <returns> the value returned after visiting all entries </returns>
		T end();
	}

}
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

namespace org.apache.commons.math3.linear
{

	using org.apache.commons.math3;

	/// <summary>
	/// Default implementation of the <seealso cref="FieldMatrixPreservingVisitor"/> interface.
	/// <p>
	/// This class is a convenience to create custom visitors without defining all
	/// methods. This class provides default implementations that do nothing.
	/// </p>
	/// </summary>
	/// @param <T> the type of the field elements
	/// @version $Id: DefaultFieldMatrixPreservingVisitor.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0 </param>
	public class DefaultFieldMatrixPreservingVisitor<T> : FieldMatrixPreservingVisitor<T> where T : org.apache.commons.math3.FieldElement<T>
	{
		/// <summary>
		/// Zero element of the field. </summary>
		private readonly T zero;

		/// <summary>
		/// Build a new instance. </summary>
		/// <param name="zero"> additive identity of the field </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DefaultFieldMatrixPreservingVisitor(final T zero)
		public DefaultFieldMatrixPreservingVisitor(T zero)
		{
			this.zero = zero;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual void start(int rows, int columns, int startRow, int endRow, int startColumn, int endColumn)
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual void visit(int row, int column, T value)
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual T end()
		{
			return zero;
		}
	}

}
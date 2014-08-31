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
namespace org.apache.commons.math3.geometry
{

	using MathUnsupportedOperationException = org.apache.commons.math3.exception.MathUnsupportedOperationException;

	/// <summary>
	/// This interface represents a generic space, with affine and vectorial counterparts.
	/// @version $Id: Space.java 1416643 2012-12-03 19:37:14Z tn $ </summary>
	/// <seealso cref= Vector
	/// @since 3.0 </seealso>
	public interface Space : Serializable
	{

		/// <summary>
		/// Get the dimension of the space. </summary>
		/// <returns> dimension of the space </returns>
		int Dimension {get;}

		/// <summary>
		/// Get the n-1 dimension subspace of this space. </summary>
		/// <returns> n-1 dimension sub-space of this space </returns>
		/// <seealso cref= #getDimension() </seealso>
		/// <exception cref="MathUnsupportedOperationException"> for dimension-1 spaces
		/// which do not have sub-spaces </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Space getSubSpace() throws org.apache.commons.math3.exception.MathUnsupportedOperationException;
		Space SubSpace {get;}

	}

}
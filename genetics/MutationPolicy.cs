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
namespace mathlib.genetics
{

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;

	/// <summary>
	/// Algorithm used to mutate a chromosome.
	/// 
	/// @since 2.0
	/// @version $Id: MutationPolicy.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public interface MutationPolicy
	{

		/// <summary>
		/// Mutate the given chromosome. </summary>
		/// <param name="original"> the original chromosome. </param>
		/// <returns> the mutated chromosome. </returns>
		/// <exception cref="MathIllegalArgumentException"> if the given chromosome is not compatible with this <seealso cref="MutationPolicy"/> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Chromosome mutate(Chromosome original) throws mathlib.exception.MathIllegalArgumentException;
		Chromosome mutate(Chromosome original);
	}

}
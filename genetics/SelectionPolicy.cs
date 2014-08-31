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
namespace org.apache.commons.math3.genetics
{

	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;

	/// <summary>
	/// Algorithm used to select a chromosome pair from a population.
	/// 
	/// @since 2.0
	/// @version $Id: SelectionPolicy.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public interface SelectionPolicy
	{
		/// <summary>
		/// Select two chromosomes from the population. </summary>
		/// <param name="population"> the population from which the chromosomes are choosen. </param>
		/// <returns> the selected chromosomes. </returns>
		/// <exception cref="MathIllegalArgumentException"> if the population is not compatible with this <seealso cref="SelectionPolicy"/> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ChromosomePair select(Population population) throws org.apache.commons.math3.exception.MathIllegalArgumentException;
		ChromosomePair select(Population population);
	}

}
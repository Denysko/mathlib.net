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

	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;


	/// <summary>
	/// A collection of chromosomes that facilitates generational evolution.
	/// 
	/// @since 2.0
	/// @version $Id: Population.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public interface Population : IEnumerable<Chromosome>
	{
		/// <summary>
		/// Access the current population size. </summary>
		/// <returns> the current population size. </returns>
		int PopulationSize {get;}

		/// <summary>
		/// Access the maximum population size. </summary>
		/// <returns> the maximum population size. </returns>
		int PopulationLimit {get;}

		/// <summary>
		/// Start the population for the next generation. </summary>
		/// <returns> the beginnings of the next generation. </returns>
		Population nextGeneration();

		/// <summary>
		/// Add the given chromosome to the population. </summary>
		/// <param name="chromosome"> the chromosome to add. </param>
		/// <exception cref="NumberIsTooLargeException"> if the population would exceed the population limit when adding
		///   this chromosome </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void addChromosome(Chromosome chromosome) throws mathlib.exception.NumberIsTooLargeException;
		void addChromosome(Chromosome chromosome);

		/// <summary>
		/// Access the fittest chromosome in this population. </summary>
		/// <returns> the fittest chromosome. </returns>
		Chromosome FittestChromosome {get;}
	}

}
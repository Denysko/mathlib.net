using System.Collections.Generic;

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


	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Population of chromosomes which uses elitism (certain percentage of the best
	/// chromosomes is directly copied to the next generation).
	/// 
	/// @version $Id: ElitisticListPopulation.java 1550977 2013-12-14 22:07:02Z tn $
	/// @since 2.0
	/// </summary>
	public class ElitisticListPopulation : ListPopulation
	{

		/// <summary>
		/// percentage of chromosomes copied to the next generation </summary>
		private double elitismRate = 0.9;

		/// <summary>
		/// Creates a new <seealso cref="ElitisticListPopulation"/> instance.
		/// </summary>
		/// <param name="chromosomes"> list of chromosomes in the population </param>
		/// <param name="populationLimit"> maximal size of the population </param>
		/// <param name="elitismRate"> how many best chromosomes will be directly transferred to the next generation [in %] </param>
		/// <exception cref="NullArgumentException"> if the list of chromosomes is {@code null} </exception>
		/// <exception cref="NotPositiveException"> if the population limit is not a positive number (&lt; 1) </exception>
		/// <exception cref="NumberIsTooLargeException"> if the list of chromosomes exceeds the population limit </exception>
		/// <exception cref="OutOfRangeException"> if the elitism rate is outside the [0, 1] range </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ElitisticListPopulation(final java.util.List<Chromosome> chromosomes, final int populationLimit, final double elitismRate) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.NumberIsTooLargeException, org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public ElitisticListPopulation(IList<Chromosome> chromosomes, int populationLimit, double elitismRate) : base(chromosomes, populationLimit)
		{

			ElitismRate = elitismRate;
		}

		/// <summary>
		/// Creates a new <seealso cref="ElitisticListPopulation"/> instance and initializes its inner chromosome list.
		/// </summary>
		/// <param name="populationLimit"> maximal size of the population </param>
		/// <param name="elitismRate"> how many best chromosomes will be directly transferred to the next generation [in %] </param>
		/// <exception cref="NotPositiveException"> if the population limit is not a positive number (&lt; 1) </exception>
		/// <exception cref="OutOfRangeException"> if the elitism rate is outside the [0, 1] range </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ElitisticListPopulation(final int populationLimit, final double elitismRate) throws org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public ElitisticListPopulation(int populationLimit, double elitismRate) : base(populationLimit)
		{

			ElitismRate = elitismRate;
		}

		/// <summary>
		/// Start the population for the next generation. The <code><seealso cref="#elitismRate"/></code>
		/// percents of the best chromosomes are directly copied to the next generation.
		/// </summary>
		/// <returns> the beginnings of the next generation. </returns>
		public override Population nextGeneration()
		{
			// initialize a new generation with the same parameters
			ElitisticListPopulation nextGeneration = new ElitisticListPopulation(PopulationLimit, ElitismRate);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Chromosome> oldChromosomes = getChromosomeList();
			IList<Chromosome> oldChromosomes = ChromosomeList;
			oldChromosomes.Sort();

			// index of the last "not good enough" chromosome
			int boundIndex = (int) FastMath.ceil((1.0 - ElitismRate) * oldChromosomes.Count);
			for (int i = boundIndex; i < oldChromosomes.Count; i++)
			{
				nextGeneration.addChromosome(oldChromosomes[i]);
			}
			return nextGeneration;
		}

		/// <summary>
		/// Sets the elitism rate, i.e. how many best chromosomes will be directly transferred to the next generation [in %].
		/// </summary>
		/// <param name="elitismRate"> how many best chromosomes will be directly transferred to the next generation [in %] </param>
		/// <exception cref="OutOfRangeException"> if the elitism rate is outside the [0, 1] range </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setElitismRate(final double elitismRate) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double ElitismRate
		{
			set
			{
				if (value < 0 || value > 1)
				{
					throw new OutOfRangeException(LocalizedFormats.ELITISM_RATE, value, 0, 1);
				}
				this.elitismRate = value;
			}
			get
			{
				return this.elitismRate;
			}
		}


	}

}
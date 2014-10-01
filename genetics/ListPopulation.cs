using System;
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
namespace mathlib.genetics
{


	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using NotPositiveException = mathlib.exception.NotPositiveException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;

	/// <summary>
	/// Population of chromosomes represented by a <seealso cref="List"/>.
	/// 
	/// @since 2.0
	/// @version $Id: ListPopulation.java 1422195 2012-12-15 06:45:18Z psteitz $
	/// </summary>
	public abstract class ListPopulation : Population
	{
		public abstract Population nextGeneration();

		/// <summary>
		/// List of chromosomes </summary>
		private IList<Chromosome> chromosomes;

		/// <summary>
		/// maximal size of the population </summary>
		private int populationLimit;

		/// <summary>
		/// Creates a new ListPopulation instance and initializes its inner chromosome list.
		/// </summary>
		/// <param name="populationLimit"> maximal size of the population </param>
		/// <exception cref="NotPositiveException"> if the population limit is not a positive number (&lt; 1) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ListPopulation(final int populationLimit) throws mathlib.exception.NotPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public ListPopulation(int populationLimit) : this(Collections.emptyList<Chromosome> (), populationLimit)
		{
		}

		/// <summary>
		/// Creates a new ListPopulation instance.
		/// <p>
		/// Note: the chromosomes of the specified list are added to the population.
		/// </summary>
		/// <param name="chromosomes"> list of chromosomes to be added to the population </param>
		/// <param name="populationLimit"> maximal size of the population </param>
		/// <exception cref="NullArgumentException"> if the list of chromosomes is {@code null} </exception>
		/// <exception cref="NotPositiveException"> if the population limit is not a positive number (&lt; 1) </exception>
		/// <exception cref="NumberIsTooLargeException"> if the list of chromosomes exceeds the population limit </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ListPopulation(final java.util.List<Chromosome> chromosomes, final int populationLimit) throws mathlib.exception.NullArgumentException, mathlib.exception.NotPositiveException, mathlib.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public ListPopulation(IList<Chromosome> chromosomes, int populationLimit)
		{

			if (chromosomes == null)
			{
				throw new NullArgumentException();
			}
			if (populationLimit <= 0)
			{
				throw new NotPositiveException(LocalizedFormats.POPULATION_LIMIT_NOT_POSITIVE, populationLimit);
			}
			if (chromosomes.Count > populationLimit)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.LIST_OF_CHROMOSOMES_BIGGER_THAN_POPULATION_SIZE, chromosomes.Count, populationLimit, false);
			}
			this.populationLimit = populationLimit;
			this.chromosomes = new List<Chromosome>(populationLimit);
			this.chromosomes.AddRange(chromosomes);
		}

		/// <summary>
		/// Sets the list of chromosomes.
		/// <p>
		/// Note: this method removed all existing chromosomes in the population and adds all chromosomes
		/// of the specified list to the population.
		/// </summary>
		/// <param name="chromosomes"> the list of chromosomes </param>
		/// <exception cref="NullArgumentException"> if the list of chromosomes is {@code null} </exception>
		/// <exception cref="NumberIsTooLargeException"> if the list of chromosomes exceeds the population limit </exception>
		/// @deprecated use <seealso cref="#addChromosomes(Collection)"/> instead 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("use <seealso cref="#addChromosomes(java.util.Collection)"/> instead") public void setChromosomes(final java.util.List<Chromosome> chromosomes) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		[Obsolete]//("use <seealso cref="#addChromosomes(java.util.Collection)"/> instead")]
		public virtual IList<Chromosome> Chromosomes
		{
			set
			{
    
				if (value == null)
				{
					throw new NullArgumentException();
				}
				if (value.Count > populationLimit)
				{
					throw new NumberIsTooLargeException(LocalizedFormats.LIST_OF_CHROMOSOMES_BIGGER_THAN_POPULATION_SIZE, value.Count, populationLimit, false);
				}
				this.chromosomes.Clear();
				this.chromosomes.AddRange(value);
			}
			get
			{
				return Collections.unmodifiableList(chromosomes);
			}
		}

		/// <summary>
		/// Add a <seealso cref="Collection"/> of chromosomes to this <seealso cref="Population"/>. </summary>
		/// <param name="chromosomeColl"> a <seealso cref="Collection"/> of chromosomes </param>
		/// <exception cref="NumberIsTooLargeException"> if the population would exceed the population limit when
		/// adding this chromosome
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addChromosomes(final java.util.Collection<Chromosome> chromosomeColl) throws mathlib.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void addChromosomes(ICollection<Chromosome> chromosomeColl)
		{
			if (chromosomes.Count + chromosomeColl.Count > populationLimit)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.LIST_OF_CHROMOSOMES_BIGGER_THAN_POPULATION_SIZE, chromosomes.Count, populationLimit, false);
			}
			this.chromosomes.AddRange(chromosomeColl);
		}


		/// <summary>
		/// Access the list of chromosomes. </summary>
		/// <returns> the list of chromosomes
		/// @since 3.1 </returns>
		protected internal virtual IList<Chromosome> ChromosomeList
		{
			get
			{
				return chromosomes;
			}
		}

		/// <summary>
		/// Add the given chromosome to the population.
		/// </summary>
		/// <param name="chromosome"> the chromosome to add. </param>
		/// <exception cref="NumberIsTooLargeException"> if the population would exceed the {@code populationLimit} after
		///   adding this chromosome </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addChromosome(final Chromosome chromosome) throws mathlib.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void addChromosome(Chromosome chromosome)
		{
			if (chromosomes.Count >= populationLimit)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.LIST_OF_CHROMOSOMES_BIGGER_THAN_POPULATION_SIZE, chromosomes.Count, populationLimit, false);
			}
			this.chromosomes.Add(chromosome);
		}

		/// <summary>
		/// Access the fittest chromosome in this population. </summary>
		/// <returns> the fittest chromosome. </returns>
		public virtual Chromosome FittestChromosome
		{
			get
			{
				// best so far
				Chromosome bestChromosome = this.chromosomes[0];
				foreach (Chromosome chromosome in this.chromosomes)
				{
					if (chromosome.CompareTo(bestChromosome) > 0)
					{
						// better chromosome found
						bestChromosome = chromosome;
					}
				}
				return bestChromosome;
			}
		}

		/// <summary>
		/// Access the maximum population size. </summary>
		/// <returns> the maximum population size. </returns>
		public virtual int PopulationLimit
		{
			get
			{
				return this.populationLimit;
			}
			set
			{
				if (value <= 0)
				{
					throw new NotPositiveException(LocalizedFormats.POPULATION_LIMIT_NOT_POSITIVE, value);
				}
				if (value < chromosomes.Count)
				{
					throw new NumberIsTooSmallException(value, chromosomes.Count, true);
				}
				this.populationLimit = value;
			}
		}

		/// <summary>
		/// Sets the maximal population size. </summary>
		/// <param name="populationLimit"> maximal population size. </param>
		/// <exception cref="NotPositiveException"> if the population limit is not a positive number (&lt; 1) </exception>
		/// <exception cref="NumberIsTooSmallException"> if the new population size is smaller than the current number
		///   of chromosomes in the population </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setPopulationLimit(final int populationLimit) throws mathlib.exception.NotPositiveException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:

		/// <summary>
		/// Access the current population size. </summary>
		/// <returns> the current population size. </returns>
		public virtual int PopulationSize
		{
			get
			{
				return this.chromosomes.Count;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override string ToString()
		{
			return this.chromosomes.ToString();
		}

		/// <summary>
		/// Returns an iterator over the unmodifiable list of chromosomes.
		/// <p>Any call to <seealso cref="Iterator#remove()"/> will result in a <seealso cref="UnsupportedOperationException"/>.</p>
		/// </summary>
		/// <returns> chromosome iterator </returns>
		public virtual IEnumerator<Chromosome> GetEnumerator()
		{
			return Chromosomes.GetEnumerator();
		}
	}

}
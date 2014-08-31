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


	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// Tournament selection scheme. Each of the two selected chromosomes is selected
	/// based on n-ary tournament -- this is done by drawing <seealso cref="#arity"/> random
	/// chromosomes without replacement from the population, and then selecting the
	/// fittest chromosome among them.
	/// 
	/// @since 2.0
	/// @version $Id: TournamentSelection.java 1550977 2013-12-14 22:07:02Z tn $
	/// </summary>
	public class TournamentSelection : SelectionPolicy
	{

		/// <summary>
		/// number of chromosomes included in the tournament selections </summary>
		private int arity;

		/// <summary>
		/// Creates a new TournamentSelection instance.
		/// </summary>
		/// <param name="arity"> how many chromosomes will be drawn to the tournament </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public TournamentSelection(final int arity)
		public TournamentSelection(int arity)
		{
			this.arity = arity;
		}

		/// <summary>
		/// Select two chromosomes from the population. Each of the two selected
		/// chromosomes is selected based on n-ary tournament -- this is done by
		/// drawing <seealso cref="#arity"/> random chromosomes without replacement from the
		/// population, and then selecting the fittest chromosome among them.
		/// </summary>
		/// <param name="population"> the population from which the chromosomes are chosen. </param>
		/// <returns> the selected chromosomes. </returns>
		/// <exception cref="MathIllegalArgumentException"> if the tournament arity is bigger than the population size </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ChromosomePair select(final Population population) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual ChromosomePair select(Population population)
		{
			return new ChromosomePair(tournament((ListPopulation) population), tournament((ListPopulation) population));
		}

		/// <summary>
		/// Helper for <seealso cref="#select(Population)"/>. Draw <seealso cref="#arity"/> random chromosomes without replacement from the
		/// population, and then select the fittest chromosome among them.
		/// </summary>
		/// <param name="population"> the population from which the chromosomes are chosen. </param>
		/// <returns> the selected chromosome. </returns>
		/// <exception cref="MathIllegalArgumentException"> if the tournament arity is bigger than the population size </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Chromosome tournament(final ListPopulation population) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private Chromosome tournament(ListPopulation population)
		{
			if (population.PopulationSize < this.arity)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.TOO_LARGE_TOURNAMENT_ARITY, arity, population.PopulationSize);
			}
			// auxiliary population
			ListPopulation tournamentPopulation = new ListPopulationAnonymousInnerClassHelper(this, this.arity);

			// create a copy of the chromosome list
			IList<Chromosome> chromosomes = new List<Chromosome> (population.Chromosomes);
			for (int i = 0; i < this.arity; i++)
			{
				// select a random individual and add it to the tournament
				int rind = GeneticAlgorithm.RandomGenerator.Next(chromosomes.Count);
				tournamentPopulation.addChromosome(chromosomes[rind]);
				// do not select it again
				chromosomes.RemoveAt(rind);
			}
			// the winner takes it all
			return tournamentPopulation.FittestChromosome;
		}

		private class ListPopulationAnonymousInnerClassHelper : ListPopulation
		{
			private readonly TournamentSelection outerInstance;

			public ListPopulationAnonymousInnerClassHelper(TournamentSelection outerInstance, int arity) : base(arity)
			{
				this.outerInstance = outerInstance;
			}

			public override Population nextGeneration()
			{
				// not useful here
				return null;
			}
		}

		/// <summary>
		/// Gets the arity (number of chromosomes drawn to the tournament).
		/// </summary>
		/// <returns> arity of the tournament </returns>
		public virtual int Arity
		{
			get
			{
				return arity;
			}
			set
			{
				this.arity = value;
			}
		}

		/// <summary>
		/// Sets the arity (number of chromosomes drawn to the tournament).
		/// </summary>
		/// <param name="arity"> arity of the tournament </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setArity(final int arity)

	}

}
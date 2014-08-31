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

	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using RandomGenerator = org.apache.commons.math3.random.RandomGenerator;
	using JDKRandomGenerator = org.apache.commons.math3.random.JDKRandomGenerator;

	/// <summary>
	/// Implementation of a genetic algorithm. All factors that govern the operation
	/// of the algorithm can be configured for a specific problem.
	/// 
	/// @since 2.0
	/// @version $Id: GeneticAlgorithm.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class GeneticAlgorithm
	{

		/// <summary>
		/// Static random number generator shared by GA implementation classes. Set the randomGenerator seed to get
		/// reproducible results. Use <seealso cref="#setRandomGenerator(RandomGenerator)"/> to supply an alternative to the default
		/// JDK-provided PRNG.
		/// </summary>
		//@GuardedBy("this")
		private static RandomGenerator randomGenerator = new JDKRandomGenerator();

		/// <summary>
		/// the crossover policy used by the algorithm. </summary>
		private readonly CrossoverPolicy crossoverPolicy;

		/// <summary>
		/// the rate of crossover for the algorithm. </summary>
		private readonly double crossoverRate;

		/// <summary>
		/// the mutation policy used by the algorithm. </summary>
		private readonly MutationPolicy mutationPolicy;

		/// <summary>
		/// the rate of mutation for the algorithm. </summary>
		private readonly double mutationRate;

		/// <summary>
		/// the selection policy used by the algorithm. </summary>
		private readonly SelectionPolicy selectionPolicy;

		/// <summary>
		/// the number of generations evolved to reach <seealso cref="StoppingCondition"/> in the last run. </summary>
		private int generationsEvolved = 0;

		/// <summary>
		/// Create a new genetic algorithm. </summary>
		/// <param name="crossoverPolicy"> The <seealso cref="CrossoverPolicy"/> </param>
		/// <param name="crossoverRate"> The crossover rate as a percentage (0-1 inclusive) </param>
		/// <param name="mutationPolicy"> The <seealso cref="MutationPolicy"/> </param>
		/// <param name="mutationRate"> The mutation rate as a percentage (0-1 inclusive) </param>
		/// <param name="selectionPolicy"> The <seealso cref="SelectionPolicy"/> </param>
		/// <exception cref="OutOfRangeException"> if the crossover or mutation rate is outside the [0, 1] range </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GeneticAlgorithm(final CrossoverPolicy crossoverPolicy, final double crossoverRate, final MutationPolicy mutationPolicy, final double mutationRate, final SelectionPolicy selectionPolicy) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public GeneticAlgorithm(CrossoverPolicy crossoverPolicy, double crossoverRate, MutationPolicy mutationPolicy, double mutationRate, SelectionPolicy selectionPolicy)
		{

			if (crossoverRate < 0 || crossoverRate > 1)
			{
				throw new OutOfRangeException(LocalizedFormats.CROSSOVER_RATE, crossoverRate, 0, 1);
			}
			if (mutationRate < 0 || mutationRate > 1)
			{
				throw new OutOfRangeException(LocalizedFormats.MUTATION_RATE, mutationRate, 0, 1);
			}
			this.crossoverPolicy = crossoverPolicy;
			this.crossoverRate = crossoverRate;
			this.mutationPolicy = mutationPolicy;
			this.mutationRate = mutationRate;
			this.selectionPolicy = selectionPolicy;
		}

		/// <summary>
		/// Set the (static) random generator.
		/// </summary>
		/// <param name="random"> random generator </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static synchronized void setRandomGenerator(final org.apache.commons.math3.random.RandomGenerator random)
		public static RandomGenerator RandomGenerator
		{
			set
			{
				lock (typeof(GeneticAlgorithm))
				{
					randomGenerator = value;
				}
			}
			get
			{
				lock (typeof(GeneticAlgorithm))
				{
					return randomGenerator;
				}
			}
		}


		/// <summary>
		/// Evolve the given population. Evolution stops when the stopping condition
		/// is satisfied. Updates the <seealso cref="#getGenerationsEvolved() generationsEvolved"/>
		/// property with the number of generations evolved before the StoppingCondition
		/// is satisfied.
		/// </summary>
		/// <param name="initial"> the initial, seed population. </param>
		/// <param name="condition"> the stopping condition used to stop evolution. </param>
		/// <returns> the population that satisfies the stopping condition. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Population evolve(final Population initial, final StoppingCondition condition)
		public virtual Population evolve(Population initial, StoppingCondition condition)
		{
			Population current = initial;
			generationsEvolved = 0;
			while (!condition.isSatisfied(current))
			{
				current = nextGeneration(current);
				generationsEvolved++;
			}
			return current;
		}

		/// <summary>
		/// Evolve the given population into the next generation.
		/// <p>
		/// <ol>
		///  <li>Get nextGeneration population to fill from <code>current</code>
		///      generation, using its nextGeneration method</li>
		///  <li>Loop until new generation is filled:</li>
		///  <ul><li>Apply configured SelectionPolicy to select a pair of parents
		///          from <code>current</code></li>
		///      <li>With probability = <seealso cref="#getCrossoverRate()"/>, apply
		///          configured <seealso cref="CrossoverPolicy"/> to parents</li>
		///      <li>With probability = <seealso cref="#getMutationRate()"/>, apply
		///          configured <seealso cref="MutationPolicy"/> to each of the offspring</li>
		///      <li>Add offspring individually to nextGeneration,
		///          space permitting</li>
		///  </ul>
		///  <li>Return nextGeneration</li>
		/// </ol>
		/// </summary>
		/// <param name="current"> the current population. </param>
		/// <returns> the population for the next generation. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Population nextGeneration(final Population current)
		public virtual Population nextGeneration(Population current)
		{
			Population nextGeneration = current.nextGeneration();

			RandomGenerator randGen = RandomGenerator;

			while (nextGeneration.PopulationSize < nextGeneration.PopulationLimit)
			{
				// select parent chromosomes
				ChromosomePair pair = SelectionPolicy.select(current);

				// crossover?
				if (randGen.NextDouble() < CrossoverRate)
				{
					// apply crossover policy to create two offspring
					pair = CrossoverPolicy.crossover(pair.First, pair.Second);
				}

				// mutation?
				if (randGen.NextDouble() < MutationRate)
				{
					// apply mutation policy to the chromosomes
					pair = new ChromosomePair(MutationPolicy.mutate(pair.First), MutationPolicy.mutate(pair.Second));
				}

				// add the first chromosome to the population
				nextGeneration.addChromosome(pair.First);
				// is there still a place for the second chromosome?
				if (nextGeneration.PopulationSize < nextGeneration.PopulationLimit)
				{
					// add the second chromosome to the population
					nextGeneration.addChromosome(pair.Second);
				}
			}

			return nextGeneration;
		}

		/// <summary>
		/// Returns the crossover policy. </summary>
		/// <returns> crossover policy </returns>
		public virtual CrossoverPolicy CrossoverPolicy
		{
			get
			{
				return crossoverPolicy;
			}
		}

		/// <summary>
		/// Returns the crossover rate. </summary>
		/// <returns> crossover rate </returns>
		public virtual double CrossoverRate
		{
			get
			{
				return crossoverRate;
			}
		}

		/// <summary>
		/// Returns the mutation policy. </summary>
		/// <returns> mutation policy </returns>
		public virtual MutationPolicy MutationPolicy
		{
			get
			{
				return mutationPolicy;
			}
		}

		/// <summary>
		/// Returns the mutation rate. </summary>
		/// <returns> mutation rate </returns>
		public virtual double MutationRate
		{
			get
			{
				return mutationRate;
			}
		}

		/// <summary>
		/// Returns the selection policy. </summary>
		/// <returns> selection policy </returns>
		public virtual SelectionPolicy SelectionPolicy
		{
			get
			{
				return selectionPolicy;
			}
		}

		/// <summary>
		/// Returns the number of generations evolved to reach <seealso cref="StoppingCondition"/> in the last run.
		/// </summary>
		/// <returns> number of generations evolved
		/// @since 2.1 </returns>
		public virtual int GenerationsEvolved
		{
			get
			{
				return generationsEvolved;
			}
		}

	}

}
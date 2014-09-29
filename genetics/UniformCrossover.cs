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


	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using RandomGenerator = mathlib.random.RandomGenerator;

	/// <summary>
	/// Perform Uniform Crossover [UX] on the specified chromosomes. A fixed mixing
	/// ratio is used to combine genes from the first and second parents, e.g. using a
	/// ratio of 0.5 would result in approximately 50% of genes coming from each
	/// parent. This is typically a poor method of crossover, but empirical evidence
	/// suggests that it is more exploratory and results in a larger part of the
	/// problem space being searched.
	/// <p>
	/// This crossover policy evaluates each gene of the parent chromosomes by chosing a
	/// uniform random number {@code p} in the range [0, 1]. If {@code p} &lt; {@code ratio},
	/// the parent genes are swapped. This means with a ratio of 0.7, 30% of the genes from the
	/// first parent and 70% from the second parent will be selected for the first offspring (and
	/// vice versa for the second offspring).
	/// <p>
	/// This policy works only on <seealso cref="AbstractListChromosome"/>, and therefore it
	/// is parameterized by T. Moreover, the chromosomes must have same lengths.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Crossover_%28genetic_algorithm%29">Crossover techniques (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://www.obitko.com/tutorials/genetic-algorithms/crossover-mutation.php">Crossover (Obitko.com)</a> </seealso>
	/// <seealso cref= <a href="http://www.tomaszgwiazda.com/uniformX.htm">Uniform crossover</a> </seealso>
	/// @param <T> generic type of the <seealso cref="AbstractListChromosome"/>s for crossover
	/// @since 3.1
	/// @version $Id: UniformCrossover.java 1551014 2013-12-15 10:56:49Z tn $ </param>
	public class UniformCrossover<T> : CrossoverPolicy
	{

		/// <summary>
		/// The mixing ratio. </summary>
		private readonly double ratio;

		/// <summary>
		/// Creates a new <seealso cref="UniformCrossover"/> policy using the given mixing ratio.
		/// </summary>
		/// <param name="ratio"> the mixing ratio </param>
		/// <exception cref="OutOfRangeException"> if the mixing ratio is outside the [0, 1] range </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public UniformCrossover(final double ratio) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public UniformCrossover(double ratio)
		{
			if (ratio < 0.0d || ratio > 1.0d)
			{
				throw new OutOfRangeException(LocalizedFormats.CROSSOVER_RATE, ratio, 0.0d, 1.0d);
			}
			this.ratio = ratio;
		}

		/// <summary>
		/// Returns the mixing ratio used by this <seealso cref="CrossoverPolicy"/>.
		/// </summary>
		/// <returns> the mixing ratio </returns>
		public virtual double Ratio
		{
			get
			{
				return ratio;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="MathIllegalArgumentException"> iff one of the chromosomes is
		///   not an instance of <seealso cref="AbstractListChromosome"/> </exception>
		/// <exception cref="DimensionMismatchException"> if the length of the two chromosomes is different </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public ChromosomePair crossover(final Chromosome first, final Chromosome second) throws mathlib.exception.DimensionMismatchException, mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual ChromosomePair crossover(Chromosome first, Chromosome second)
		{

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (!(first instanceof AbstractListChromosome<?> && second instanceof AbstractListChromosome<?>))
			if (!(first is AbstractListChromosome<?> && second is AbstractListChromosome<?>))
			{
				throw new MathIllegalArgumentException(LocalizedFormats.INVALID_FIXED_LENGTH_CHROMOSOME);
			}
			return mate((AbstractListChromosome<T>) first, (AbstractListChromosome<T>) second);
		}

		/// <summary>
		/// Helper for <seealso cref="#crossover(Chromosome, Chromosome)"/>. Performs the actual crossover.
		/// </summary>
		/// <param name="first"> the first chromosome </param>
		/// <param name="second"> the second chromosome </param>
		/// <returns> the pair of new chromosomes that resulted from the crossover </returns>
		/// <exception cref="DimensionMismatchException"> if the length of the two chromosomes is different </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private ChromosomePair mate(final AbstractListChromosome<T> first, final AbstractListChromosome<T> second) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private ChromosomePair mate(AbstractListChromosome<T> first, AbstractListChromosome<T> second)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = first.getLength();
			int length = first.Length;
			if (length != second.Length)
			{
				throw new DimensionMismatchException(second.Length, length);
			}

			// array representations of the parents
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<T> parent1Rep = first.getRepresentation();
			IList<T> parent1Rep = first.Representation;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<T> parent2Rep = second.getRepresentation();
			IList<T> parent2Rep = second.Representation;
			// and of the children
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<T> child1Rep = new java.util.ArrayList<T>(length);
			IList<T> child1Rep = new List<T>(length);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<T> child2Rep = new java.util.ArrayList<T>(length);
			IList<T> child2Rep = new List<T>(length);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.random.RandomGenerator random = GeneticAlgorithm.getRandomGenerator();
			RandomGenerator random = GeneticAlgorithm.RandomGenerator;

			for (int index = 0; index < length; index++)
			{

				if (random.NextDouble() < ratio)
				{
					// swap the bits -> take other parent
					child1Rep.Add(parent2Rep[index]);
					child2Rep.Add(parent1Rep[index]);
				}
				else
				{
					child1Rep.Add(parent1Rep[index]);
					child2Rep.Add(parent2Rep[index]);
				}
			}

			return new ChromosomePair(first.newFixedLengthChromosome(child1Rep), second.newFixedLengthChromosome(child2Rep));
		}
	}

}
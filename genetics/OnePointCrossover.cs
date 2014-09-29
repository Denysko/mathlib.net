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
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;


	/// <summary>
	/// One point crossover policy. A random crossover point is selected and the
	/// first part from each parent is copied to the corresponding child, and the
	/// second parts are copied crosswise.
	/// 
	/// Example:
	/// <pre>
	/// -C- denotes a crossover point
	///                   -C-                                 -C-
	/// p1 = (1 0 1 0 0 1  | 0 1 1)    X    p2 = (0 1 1 0 1 0  | 1 1 1)
	///      \------------/ \-----/              \------------/ \-----/
	///            ||         (*)                       ||        (**)
	///            VV         (**)                      VV        (*)
	///      /------------\ /-----\              /------------\ /-----\
	/// c1 = (1 0 1 0 0 1  | 1 1 1)    X    c2 = (0 1 1 0 1 0  | 0 1 1)
	/// </pre>
	/// 
	/// This policy works only on <seealso cref="AbstractListChromosome"/>, and therefore it
	/// is parameterized by T. Moreover, the chromosomes must have same lengths.
	/// </summary>
	/// @param <T> generic type of the <seealso cref="AbstractListChromosome"/>s for crossover
	/// @since 2.0
	/// @version $Id: OnePointCrossover.java 1551014 2013-12-15 10:56:49Z tn $
	///  </param>
	public class OnePointCrossover<T> : CrossoverPolicy
	{

		/// <summary>
		/// Performs one point crossover. A random crossover point is selected and the
		/// first part from each parent is copied to the corresponding child, and the
		/// second parts are copied crosswise.
		/// 
		/// Example:
		/// <pre>
		/// -C- denotes a crossover point
		///                   -C-                                 -C-
		/// p1 = (1 0 1 0 0 1  | 0 1 1)    X    p2 = (0 1 1 0 1 0  | 1 1 1)
		///      \------------/ \-----/              \------------/ \-----/
		///            ||         (*)                       ||        (**)
		///            VV         (**)                      VV        (*)
		///      /------------\ /-----\              /------------\ /-----\
		/// c1 = (1 0 1 0 0 1  | 1 1 1)    X    c2 = (0 1 1 0 1 0  | 0 1 1)
		/// </pre>
		/// </summary>
		/// <param name="first"> first parent (p1) </param>
		/// <param name="second"> second parent (p2) </param>
		/// <returns> pair of two children (c1,c2) </returns>
		/// <exception cref="MathIllegalArgumentException"> iff one of the chromosomes is
		///   not an instance of <seealso cref="AbstractListChromosome"/> </exception>
		/// <exception cref="DimensionMismatchException"> if the length of the two chromosomes is different </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public ChromosomePair crossover(final Chromosome first, final Chromosome second) throws mathlib.exception.DimensionMismatchException, mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual ChromosomePair crossover(Chromosome first, Chromosome second) // OK because of instanceof checks
		{

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (! (first instanceof AbstractListChromosome<?> && second instanceof AbstractListChromosome<?>))
			if (!(first is AbstractListChromosome<?> && second is AbstractListChromosome<?>))
			{
				throw new MathIllegalArgumentException(LocalizedFormats.INVALID_FIXED_LENGTH_CHROMOSOME);
			}
			return crossover((AbstractListChromosome<T>) first, (AbstractListChromosome<T>) second);
		}


		/// <summary>
		/// Helper for <seealso cref="#crossover(Chromosome, Chromosome)"/>. Performs the actual crossover.
		/// </summary>
		/// <param name="first"> the first chromosome. </param>
		/// <param name="second"> the second chromosome. </param>
		/// <returns> the pair of new chromosomes that resulted from the crossover. </returns>
		/// <exception cref="DimensionMismatchException"> if the length of the two chromosomes is different </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private ChromosomePair crossover(final AbstractListChromosome<T> first, final AbstractListChromosome<T> second) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private ChromosomePair crossover(AbstractListChromosome<T> first, AbstractListChromosome<T> second)
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

			// select a crossover point at random (0 and length makes no sense)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int crossoverIndex = 1 + (GeneticAlgorithm.getRandomGenerator().nextInt(length-2));
			int crossoverIndex = 1 + (GeneticAlgorithm.RandomGenerator.Next(length - 2));

			// copy the first part
			for (int i = 0; i < crossoverIndex; i++)
			{
				child1Rep.Add(parent1Rep[i]);
				child2Rep.Add(parent2Rep[i]);
			}
			// and switch the second part
			for (int i = crossoverIndex; i < length; i++)
			{
				child1Rep.Add(parent2Rep[i]);
				child2Rep.Add(parent1Rep[i]);
			}

			return new ChromosomePair(first.newFixedLengthChromosome(child1Rep), second.newFixedLengthChromosome(child2Rep));
		}

	}

}
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
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using RandomGenerator = mathlib.random.RandomGenerator;

	/// <summary>
	/// N-point crossover policy. For each iteration a random crossover point is
	/// selected and the first part from each parent is copied to the corresponding
	/// child, and the second parts are copied crosswise.
	/// 
	/// Example (2-point crossover):
	/// <pre>
	/// -C- denotes a crossover point
	///           -C-       -C-                         -C-        -C-
	/// p1 = (1 0  | 1 0 0 1 | 0 1 1)    X    p2 = (0 1  | 1 0 1 0  | 1 1 1)
	///      \----/ \-------/ \-----/              \----/ \--------/ \-----/
	///        ||      (*)       ||                  ||      (**)       ||
	///        VV      (**)      VV                  VV      (*)        VV
	///      /----\ /--------\ /-----\             /----\ /--------\ /-----\
	/// c1 = (1 0  | 1 0 1 0  | 0 1 1)    X   c2 = (0 1  | 1 0 0 1  | 0 1 1)
	/// </pre>
	/// 
	/// This policy works only on <seealso cref="AbstractListChromosome"/>, and therefore it
	/// is parameterized by T. Moreover, the chromosomes must have same lengths.
	/// </summary>
	/// @param <T> generic type of the <seealso cref="AbstractListChromosome"/>s for crossover
	/// @since 3.1
	/// @version $Id: NPointCrossover.java 1551014 2013-12-15 10:56:49Z tn $ </param>
	public class NPointCrossover<T> : CrossoverPolicy
	{

		/// <summary>
		/// The number of crossover points. </summary>
		private readonly int crossoverPoints;

		/// <summary>
		/// Creates a new <seealso cref="NPointCrossover"/> policy using the given number of points.
		/// <p>
		/// <b>Note</b>: the number of crossover points must be &lt; <code>chromosome length - 1</code>.
		/// This condition can only be checked at runtime, as the chromosome length is not known in advance.
		/// </summary>
		/// <param name="crossoverPoints"> the number of crossover points </param>
		/// <exception cref="NotStrictlyPositiveException"> if the number of {@code crossoverPoints} is not strictly positive </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public NPointCrossover(final int crossoverPoints) throws mathlib.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public NPointCrossover(int crossoverPoints)
		{
			if (crossoverPoints <= 0)
			{
				throw new NotStrictlyPositiveException(crossoverPoints);
			}
			this.crossoverPoints = crossoverPoints;
		}

		/// <summary>
		/// Returns the number of crossover points used by this <seealso cref="CrossoverPolicy"/>.
		/// </summary>
		/// <returns> the number of crossover points </returns>
		public virtual int CrossoverPoints
		{
			get
			{
				return crossoverPoints;
			}
		}

		/// <summary>
		/// Performs a N-point crossover. N random crossover points are selected and are used
		/// to divide the parent chromosomes into segments. The segments are copied in alternate
		/// order from the two parents to the corresponding child chromosomes.
		/// 
		/// Example (2-point crossover):
		/// <pre>
		/// -C- denotes a crossover point
		///           -C-       -C-                         -C-        -C-
		/// p1 = (1 0  | 1 0 0 1 | 0 1 1)    X    p2 = (0 1  | 1 0 1 0  | 1 1 1)
		///      \----/ \-------/ \-----/              \----/ \--------/ \-----/
		///        ||      (*)       ||                  ||      (**)       ||
		///        VV      (**)      VV                  VV      (*)        VV
		///      /----\ /--------\ /-----\             /----\ /--------\ /-----\
		/// c1 = (1 0  | 1 0 1 0  | 0 1 1)    X   c2 = (0 1  | 1 0 0 1  | 0 1 1)
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
		/// <exception cref="NumberIsTooLargeException"> if the number of crossoverPoints is too large for the actual chromosomes </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private ChromosomePair mate(final AbstractListChromosome<T> first, final AbstractListChromosome<T> second) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooLargeException
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
			if (crossoverPoints >= length)
			{
				throw new NumberIsTooLargeException(crossoverPoints, length, false);
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

			IList<T> c1 = child1Rep;
			IList<T> c2 = child2Rep;

			int remainingPoints = crossoverPoints;
			int lastIndex = 0;
			for (int i = 0; i < crossoverPoints; i++, remainingPoints--)
			{
				// select the next crossover point at random
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int crossoverIndex = 1 + lastIndex + random.nextInt(length - lastIndex - remainingPoints);
				int crossoverIndex = 1 + lastIndex + random.Next(length - lastIndex - remainingPoints);

				// copy the current segment
				for (int j = lastIndex; j < crossoverIndex; j++)
				{
					c1.Add(parent1Rep[j]);
					c2.Add(parent2Rep[j]);
				}

				// swap the children for the next segment
				IList<T> tmp = c1;
				c1 = c2;
				c2 = tmp;

				lastIndex = crossoverIndex;
			}

			// copy the last segment
			for (int j = lastIndex; j < length; j++)
			{
				c1.Add(parent1Rep[j]);
				c2.Add(parent2Rep[j]);
			}

			return new ChromosomePair(first.newFixedLengthChromosome(child1Rep), second.newFixedLengthChromosome(child2Rep));
		}
	}

}
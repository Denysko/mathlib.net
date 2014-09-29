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
	using RandomGenerator = mathlib.random.RandomGenerator;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Order 1 Crossover [OX1] builds offspring from <b>ordered</b> chromosomes by copying a
	/// consecutive slice from one parent, and filling up the remaining genes from the other
	/// parent as they appear.
	/// <p>
	/// This policy works by applying the following rules:
	/// <ol>
	///   <li>select a random slice of consecutive genes from parent 1</li>
	///   <li>copy the slice to child 1 and mark out the genes in parent 2</li>
	///   <li>starting from the right side of the slice, copy genes from parent 2 as they
	///       appear to child 1 if they are not yet marked out.</li>
	/// </ol>
	/// <p>
	/// Example (random sublist from index 3 to 7, underlined):
	/// <pre>
	/// p1 = (8 4 7 3 6 2 5 1 9 0)   X   c1 = (0 4 7 3 6 2 5 1 8 9)
	///             ---------                        ---------
	/// p2 = (0 1 2 3 4 5 6 7 8 9)   X   c2 = (8 1 2 3 4 5 6 7 9 0)
	/// </pre>
	/// <p>
	/// This policy works only on <seealso cref="AbstractListChromosome"/>, and therefore it
	/// is parameterized by T. Moreover, the chromosomes must have same lengths.
	/// </summary>
	/// <seealso cref= <a href="http://www.rubicite.com/Tutorials/GeneticAlgorithms/CrossoverOperators/Order1CrossoverOperator.aspx">
	/// Order 1 Crossover Operator</a>
	/// </seealso>
	/// @param <T> generic type of the <seealso cref="AbstractListChromosome"/>s for crossover
	/// @since 3.1
	/// @version $Id: OrderedCrossover.java 1385297 2012-09-16 16:05:57Z tn $ </param>
	public class OrderedCrossover<T> : CrossoverPolicy
	{

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
//ORIGINAL LINE: protected ChromosomePair mate(final AbstractListChromosome<T> first, final AbstractListChromosome<T> second) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual ChromosomePair mate(AbstractListChromosome<T> first, AbstractListChromosome<T> second)
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
//ORIGINAL LINE: final java.util.List<T> child1 = new java.util.ArrayList<T>(length);
			IList<T> child1 = new List<T>(length);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<T> child2 = new java.util.ArrayList<T>(length);
			IList<T> child2 = new List<T>(length);
			// sets of already inserted items for quick access
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Set<T> child1Set = new java.util.HashSet<T>(length);
			Set<T> child1Set = new HashSet<T>(length);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Set<T> child2Set = new java.util.HashSet<T>(length);
			Set<T> child2Set = new HashSet<T>(length);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.random.RandomGenerator random = GeneticAlgorithm.getRandomGenerator();
			RandomGenerator random = GeneticAlgorithm.RandomGenerator;
			// choose random points, making sure that lb < ub.
			int a = random.Next(length);
			int b;
			do
			{
				b = random.Next(length);
			} while (a == b);
			// determine the lower and upper bounds
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lb = mathlib.util.FastMath.min(a, b);
			int lb = FastMath.min(a, b);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ub = mathlib.util.FastMath.max(a, b);
			int ub = FastMath.max(a, b);

			// add the subLists that are between lb and ub
			child1.AddRange(parent1Rep.subList(lb, ub + 1));
			child1Set.addAll(child1);
			child2.AddRange(parent2Rep.subList(lb, ub + 1));
			child2Set.addAll(child2);

			// iterate over every item in the parents
			for (int i = 1; i <= length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int idx = (ub + i) % length;
				int idx = (ub + i) % length;

				// retrieve the current item in each parent
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T item1 = parent1Rep.get(idx);
				T item1 = parent1Rep[idx];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T item2 = parent2Rep.get(idx);
				T item2 = parent2Rep[idx];

				// if the first child already contains the item in the second parent add it
				if (!child1Set.contains(item2))
				{
					child1.Add(item2);
					child1Set.add(item2);
				}

				// if the second child already contains the item in the first parent add it
				if (!child2Set.contains(item1))
				{
					child2.Add(item1);
					child2Set.add(item1);
				}
			}

			// rotate so that the original slice is in the same place as in the parents.
			Collections.rotate(child1, lb);
			Collections.rotate(child2, lb);

			return new ChromosomePair(first.newFixedLengthChromosome(child1), second.newFixedLengthChromosome(child2));
		}
	}

}
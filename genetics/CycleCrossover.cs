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


	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// Cycle Crossover [CX] builds offspring from <b>ordered</b> chromosomes by identifying cycles
	/// between two parent chromosomes. To form the children, the cycles are copied from the
	/// respective parents.
	/// <p>
	/// To form a cycle the following procedure is applied:
	/// <ol>
	///   <li>start with the first gene of parent 1</li>
	///   <li>look at the gene at the same position of parent 2</li>
	///   <li>go to the position with the same gene in parent 1</li>
	///   <li>add this gene index to the cycle</li>
	///   <li>repeat the steps 2-5 until we arrive at the starting gene of this cycle</li>
	/// </ol>
	/// The indices that form a cycle are then used to form the children in alternating order, i.e.
	/// in cycle 1, the genes of parent 1 are copied to child 1, while in cycle 2 the genes of parent 1
	/// are copied to child 2, and so forth ...
	/// </p>
	/// 
	/// Example (zero-start cycle):
	/// <pre>
	/// p1 = (8 4 7 3 6 2 5 1 9 0)    X   c1 = (8 1 2 3 4 5 6 7 9 0)
	/// p2 = (0 1 2 3 4 5 6 7 8 9)    X   c2 = (0 4 7 3 6 2 5 1 8 9)
	/// 
	/// cycle 1: 8 0 9
	/// cycle 2: 4 1 7 2 5 6
	/// cycle 3: 3
	/// </pre>
	/// 
	/// This policy works only on <seealso cref="AbstractListChromosome"/>, and therefore it
	/// is parameterized by T. Moreover, the chromosomes must have same lengths.
	/// </summary>
	/// <seealso cref= <a href="http://www.rubicite.com/Tutorials/GeneticAlgorithms/CrossoverOperators/CycleCrossoverOperator.aspx">
	/// Cycle Crossover Operator</a>
	/// </seealso>
	/// @param <T> generic type of the <seealso cref="AbstractListChromosome"/>s for crossover
	/// @since 3.1
	/// @version $Id: CycleCrossover.java 1385297 2012-09-16 16:05:57Z tn $ </param>
	public class CycleCrossover<T> : CrossoverPolicy
	{

		/// <summary>
		/// If the start index shall be chosen randomly. </summary>
		private readonly bool randomStart;

		/// <summary>
		/// Creates a new <seealso cref="CycleCrossover"/> policy.
		/// </summary>
		public CycleCrossover() : this(false)
		{
		}

		/// <summary>
		/// Creates a new <seealso cref="CycleCrossover"/> policy using the given {@code randomStart} behavior.
		/// </summary>
		/// <param name="randomStart"> whether the start index shall be chosen randomly or be set to 0 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public CycleCrossover(final boolean randomStart)
		public CycleCrossover(bool randomStart)
		{
			this.randomStart = randomStart;
		}

		/// <summary>
		/// Returns whether the starting index is chosen randomly or set to zero.
		/// </summary>
		/// <returns> {@code true} if the starting index is chosen randomly, {@code false} otherwise </returns>
		public virtual bool RandomStart
		{
			get
			{
				return randomStart;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="MathIllegalArgumentException"> if the chromosomes are not an instance of <seealso cref="AbstractListChromosome"/> </exception>
		/// <exception cref="DimensionMismatchException"> if the length of the two chromosomes is different </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public ChromosomePair crossover(final Chromosome first, final Chromosome second) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathIllegalArgumentException
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
//ORIGINAL LINE: protected ChromosomePair mate(final AbstractListChromosome<T> first, final AbstractListChromosome<T> second) throws org.apache.commons.math3.exception.DimensionMismatchException
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
			// and of the children: do a crossover copy to simplify the later processing
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<T> child1Rep = new java.util.ArrayList<T>(second.getRepresentation());
			IList<T> child1Rep = new List<T>(second.Representation);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<T> child2Rep = new java.util.ArrayList<T>(first.getRepresentation());
			IList<T> child2Rep = new List<T>(first.Representation);

			// the set of all visited indices so far
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Set<Integer> visitedIndices = new java.util.HashSet<Integer>(length);
			Set<int?> visitedIndices = new HashSet<int?>(length);
			// the indices of the current cycle
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Integer> indices = new java.util.ArrayList<Integer>(length);
			IList<int?> indices = new List<int?>(length);

			// determine the starting index
			int idx = randomStart ? GeneticAlgorithm.RandomGenerator.Next(length) : 0;
			int cycle = 1;

			while (visitedIndices.size() < length)
			{
				indices.Add(idx);

				T item = parent2Rep[idx];
				idx = parent1Rep.IndexOf(item);

				while (idx != indices[0])
				{
					// add that index to the cycle indices
					indices.Add(idx);
					// get the item in the second parent at that index
					item = parent2Rep[idx];
					// get the index of that item in the first parent
					idx = parent1Rep.IndexOf(item);
				}

				// for even cycles: swap the child elements on the indices found in this cycle
				if (cycle++ % 2 != 0)
				{
					foreach (int i in indices)
					{
						T tmp = child1Rep[i];
						child1Rep[i] = child2Rep[i];
						child2Rep[i] = tmp;
					}
				}

				visitedIndices.addAll(indices);
				// find next starting index: last one + 1 until we find an unvisited index
				idx = (indices[0] + 1) % length;
				while (visitedIndices.contains(idx) && visitedIndices.size() < length)
				{
					idx++;
					if (idx >= length)
					{
						idx = 0;
					}
				}
				indices.Clear();
			}

			return new ChromosomePair(first.newFixedLengthChromosome(child1Rep), second.newFixedLengthChromosome(child2Rep));
		}
	}

}
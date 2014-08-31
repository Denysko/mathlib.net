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
	/// Mutation for <seealso cref="BinaryChromosome"/>s. Randomly changes one gene.
	/// 
	/// @version $Id: BinaryMutation.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0
	/// </summary>
	public class BinaryMutation : MutationPolicy
	{

		/// <summary>
		/// Mutate the given chromosome. Randomly changes one gene.
		/// </summary>
		/// <param name="original"> the original chromosome. </param>
		/// <returns> the mutated chromosome. </returns>
		/// <exception cref="MathIllegalArgumentException"> if <code>original</code> is not an instance of <seealso cref="BinaryChromosome"/>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Chromosome mutate(Chromosome original) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		public virtual Chromosome mutate(Chromosome original)
		{
			if (!(original is BinaryChromosome))
			{
				throw new MathIllegalArgumentException(LocalizedFormats.INVALID_BINARY_CHROMOSOME);
			}

			BinaryChromosome origChrom = (BinaryChromosome) original;
			IList<int?> newRepr = new List<int?>(origChrom.Representation);

			// randomly select a gene
			int geneIndex = GeneticAlgorithm.RandomGenerator.Next(origChrom.Length);
			// and change it
			newRepr[geneIndex] = origChrom.Representation[geneIndex] == 0 ? 1 : 0;

			Chromosome newChrom = origChrom.newFixedLengthChromosome(newRepr);
			return newChrom;
		}

	}

}
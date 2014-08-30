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
	/// Mutation operator for <seealso cref="RandomKey"/>s. Changes a randomly chosen element
	/// of the array representation to a random value uniformly distributed in [0,1].
	/// 
	/// @since 2.0
	/// @version $Id: RandomKeyMutation.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class RandomKeyMutation : MutationPolicy
	{

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="MathIllegalArgumentException"> if <code>original</code> is not a <seealso cref="RandomKey"/> instance </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Chromosome mutate(final Chromosome original) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual Chromosome mutate(Chromosome original)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (!(original instanceof RandomKey<?>))
			if (!(original is RandomKey<?>))
			{
				throw new MathIllegalArgumentException(LocalizedFormats.RANDOMKEY_MUTATION_WRONG_CLASS, original.GetType().Name);
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RandomKey<?> originalRk = (RandomKey<?>) original;
			RandomKey<?> originalRk = (RandomKey<?>) original;
			IList<double?> repr = originalRk.Representation;
			int rInd = GeneticAlgorithm.RandomGenerator.Next(repr.Count);

			IList<double?> newRepr = new List<double?> (repr);
			newRepr[rInd] = GeneticAlgorithm.RandomGenerator.NextDouble();

			return originalRk.newFixedLengthChromosome(newRepr);
		}

	}

}
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


	/// <summary>
	/// Chromosome represented by a vector of 0s and 1s.
	/// 
	/// @version $Id: BinaryChromosome.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0
	/// </summary>
	public abstract class BinaryChromosome : AbstractListChromosome<int?>
	{

		/// <summary>
		/// Constructor. </summary>
		/// <param name="representation"> list of {0,1} values representing the chromosome </param>
		/// <exception cref="InvalidRepresentationException"> iff the <code>representation</code> can not represent a valid chromosome </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BinaryChromosome(java.util.List<Integer> representation) throws InvalidRepresentationException
		public BinaryChromosome(IList<int?> representation) : base(representation)
		{
		}

		/// <summary>
		/// Constructor. </summary>
		/// <param name="representation"> array of {0,1} values representing the chromosome </param>
		/// <exception cref="InvalidRepresentationException"> iff the <code>representation</code> can not represent a valid chromosome </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BinaryChromosome(Integer[] representation) throws InvalidRepresentationException
		public BinaryChromosome(int?[] representation) : base(representation)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void checkValidity(java.util.List<Integer> chromosomeRepresentation) throws InvalidRepresentationException
		protected internal override void checkValidity(IList<int?> chromosomeRepresentation)
		{
			foreach (int i in chromosomeRepresentation)
			{
				if (i < 0 || i >1)
				{
					throw new InvalidRepresentationException(LocalizedFormats.INVALID_BINARY_DIGIT, i);
				}
			}
		}

		/// <summary>
		/// Returns a representation of a random binary array of length <code>length</code>. </summary>
		/// <param name="length"> length of the array </param>
		/// <returns> a random binary array of length <code>length</code> </returns>
		public static IList<int?> randomBinaryRepresentation(int length)
		{
			// random binary list
			IList<int?> rList = new List<int?> (length);
			for (int j = 0; j < length; j++)
			{
				rList.Add(GeneticAlgorithm.RandomGenerator.Next(2));
			}
			return rList;
		}

		protected internal override bool isSame(Chromosome another)
		{
			// type check
			if (!(another is BinaryChromosome))
			{
				return false;
			}
			BinaryChromosome anotherBc = (BinaryChromosome) another;
			// size check
			if (Length != anotherBc.Length)
			{
				return false;
			}

			for (int i = 0; i < Representation.Count; i++)
			{
				if (!(Representation[i].Equals(anotherBc.Representation[i])))
				{
					return false;
				}
			}
			// all is ok
			return true;
		}
	}

}
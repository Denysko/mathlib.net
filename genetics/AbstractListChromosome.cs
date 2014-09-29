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


	/// <summary>
	/// Chromosome represented by an immutable list of a fixed length.
	/// </summary>
	/// @param <T> type of the representation list
	/// @version $Id: AbstractListChromosome.java 1561509 2014-01-26 15:47:40Z tn $
	/// @since 2.0 </param>
	public abstract class AbstractListChromosome<T> : Chromosome
	{

		/// <summary>
		/// List representing the chromosome </summary>
		private readonly IList<T> representation;

		/// <summary>
		/// Constructor, copying the input representation. </summary>
		/// <param name="representation"> inner representation of the chromosome </param>
		/// <exception cref="InvalidRepresentationException"> iff the <code>representation</code> can not represent a valid chromosome </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AbstractListChromosome(final java.util.List<T> representation) throws InvalidRepresentationException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public AbstractListChromosome(IList<T> representation) : this(representation, true)
		{
		}

		/// <summary>
		/// Constructor, copying the input representation. </summary>
		/// <param name="representation"> inner representation of the chromosome </param>
		/// <exception cref="InvalidRepresentationException"> iff the <code>representation</code> can not represent a valid chromosome </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AbstractListChromosome(final T[] representation) throws InvalidRepresentationException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public AbstractListChromosome(T[] representation) : this(Arrays.asList(representation))
		{
		}

		/// <summary>
		/// Constructor. </summary>
		/// <param name="representation"> inner representation of the chromosome </param>
		/// <param name="copyList"> if {@code true}, the representation will be copied, otherwise it will be referenced.
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public AbstractListChromosome(final java.util.List<T> representation, final boolean copyList)
		public AbstractListChromosome(IList<T> representation, bool copyList)
		{
			checkValidity(representation);
			this.representation = Collections.unmodifiableList(copyList ? new List<T>(representation) : representation);
		}

		/// <summary>
		/// Asserts that <code>representation</code> can represent a valid chromosome.
		/// </summary>
		/// <param name="chromosomeRepresentation"> representation of the chromosome </param>
		/// <exception cref="InvalidRepresentationException"> iff the <code>representation</code> can not represent a valid chromosome </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void checkValidity(java.util.List<T> chromosomeRepresentation) throws InvalidRepresentationException;
		protected internal abstract void checkValidity(IList<T> chromosomeRepresentation);

		/// <summary>
		/// Returns the (immutable) inner representation of the chromosome. </summary>
		/// <returns> the representation of the chromosome </returns>
		protected internal virtual IList<T> Representation
		{
			get
			{
				return representation;
			}
		}

		/// <summary>
		/// Returns the length of the chromosome. </summary>
		/// <returns> the length of the chromosome </returns>
		public virtual int Length
		{
			get
			{
				return Representation.Count;
			}
		}

		/// <summary>
		/// Creates a new instance of the same class as <code>this</code> is, with a given <code>arrayRepresentation</code>.
		/// This is needed in crossover and mutation operators, where we need a new instance of the same class, but with
		/// different array representation.
		/// <p>
		/// Usually, this method just calls a constructor of the class.
		/// </summary>
		/// <param name="chromosomeRepresentation"> the inner array representation of the new chromosome. </param>
		/// <returns> new instance extended from FixedLengthChromosome with the given arrayRepresentation </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public abstract AbstractListChromosome<T> newFixedLengthChromosome(final java.util.List<T> chromosomeRepresentation);
		public abstract AbstractListChromosome<T> newFixedLengthChromosome(IList<T> chromosomeRepresentation);

		public override string ToString()
		{
			return string.Format("(f={0} {1})", Fitness, Representation);
		}
	}

}
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
	/// Random Key chromosome is used for permutation representation. It is a vector
	/// of a fixed length of real numbers in [0,1] interval. The index of the i-th
	/// smallest value in the vector represents an i-th member of the permutation.
	/// <p>
	/// For example, the random key [0.2, 0.3, 0.8, 0.1] corresponds to the
	/// permutation of indices (3,0,1,2). If the original (unpermuted) sequence would
	/// be (a,b,c,d), this would mean the sequence (d,a,b,c).
	/// <p>
	/// With this representation, common operators like n-point crossover can be
	/// used, because any such chromosome represents a valid permutation.
	/// <p>
	/// Since the chromosome (and thus its arrayRepresentation) is immutable, the
	/// array representation is sorted only once in the constructor.
	/// <p>
	/// For details, see:
	/// <ul>
	///   <li>Bean, J.C.: Genetic algorithms and random keys for sequencing and
	///       optimization. ORSA Journal on Computing 6 (1994) 154-160</li>
	///   <li>Rothlauf, F.: Representations for Genetic and Evolutionary Algorithms.
	///       Volume 104 of Studies in Fuzziness and Soft Computing. Physica-Verlag,
	///       Heidelberg (2002)</li>
	/// </ul>
	/// </summary>
	/// @param <T> type of the permuted objects
	/// @since 2.0
	/// @version $Id: RandomKey.java 1549093 2013-12-08 18:23:09Z tn $ </param>
	public abstract class RandomKey<T> : AbstractListChromosome<double?>, PermutationChromosome<T>
	{
		public abstract double fitness();

		/// <summary>
		/// Cache of sorted representation (unmodifiable). </summary>
		private readonly IList<double?> sortedRepresentation;

		/// <summary>
		/// Base sequence [0,1,...,n-1], permuted according to the representation (unmodifiable).
		/// </summary>
		private readonly IList<int?> baseSeqPermutation;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="representation"> list of [0,1] values representing the permutation </param>
		/// <exception cref="InvalidRepresentationException"> iff the <code>representation</code> can not represent a valid chromosome </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RandomKey(final java.util.List<Double> representation) throws InvalidRepresentationException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public RandomKey(IList<double?> representation) : base(representation)
		{
			// store the sorted representation
			IList<double?> sortedRepr = new List<double?> (Representation);
			sortedRepr.Sort();
			sortedRepresentation = Collections.unmodifiableList(sortedRepr);
			// store the permutation of [0,1,...,n-1] list for toString() and isSame() methods
			baseSeqPermutation = Collections.unmodifiableList(decodeGeneric(baseSequence(Length), Representation, sortedRepresentation));
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="representation"> array of [0,1] values representing the permutation </param>
		/// <exception cref="InvalidRepresentationException"> iff the <code>representation</code> can not represent a valid chromosome </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RandomKey(final Double[] representation) throws InvalidRepresentationException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public RandomKey(double?[] representation) : this(Arrays.asList(representation))
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public java.util.List<T> decode(final java.util.List<T> sequence)
		public virtual IList<T> decode(IList<T> sequence)
		{
			return decodeGeneric(sequence, Representation, sortedRepresentation);
		}

		/// <summary>
		/// Decodes a permutation represented by <code>representation</code> and
		/// returns a (generic) list with the permuted values.
		/// </summary>
		/// @param <S> generic type of the sequence values </param>
		/// <param name="sequence"> the unpermuted sequence </param>
		/// <param name="representation"> representation of the permutation ([0,1] vector) </param>
		/// <param name="sortedRepr"> sorted <code>representation</code> </param>
		/// <returns> list with the sequence values permuted according to the representation </returns>
		/// <exception cref="DimensionMismatchException"> iff the length of the <code>sequence</code>,
		///   <code>representation</code> or <code>sortedRepr</code> lists are not equal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static <S> java.util.List<S> decodeGeneric(final java.util.List<S> sequence, java.util.List<Double> representation, final java.util.List<Double> sortedRepr) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private static IList<S> decodeGeneric<S>(IList<S> sequence, IList<double?> representation, IList<double?> sortedRepr)
		{

			int l = sequence.Count;

			// the size of the three lists must be equal
			if (representation.Count != l)
			{
				throw new DimensionMismatchException(representation.Count, l);
			}
			if (sortedRepr.Count != l)
			{
				throw new DimensionMismatchException(sortedRepr.Count, l);
			}

			// do not modify the original representation
			IList<double?> reprCopy = new List<double?> (representation);

			// now find the indices in the original repr and use them for permuting
			IList<S> res = new List<S> (l);
			for (int i = 0; i < l; i++)
			{
				int index = reprCopy.IndexOf(sortedRepr[i]);
				res.Add(sequence[index]);
				reprCopy[index] = null;
			}
			return res;
		}

		/// <summary>
		/// Returns <code>true</code> iff <code>another</code> is a RandomKey and
		/// encodes the same permutation.
		/// </summary>
		/// <param name="another"> chromosome to compare </param>
		/// <returns> true iff chromosomes encode the same permutation </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected boolean isSame(final Chromosome another)
		protected internal override bool isSame(Chromosome another)
		{
			// type check
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (! (another instanceof RandomKey<?>))
			if (!(another is RandomKey<?>))
			{
				return false;
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: RandomKey<?> anotherRk = (RandomKey<?>) another;
			RandomKey<?> anotherRk = (RandomKey<?>) another;
			// size check
			if (Length != anotherRk.Length)
			{
				return false;
			}

			// two different representations can still encode the same permutation
			// the ordering is what counts
			IList<int?> thisPerm = this.baseSeqPermutation;
			IList<int?> anotherPerm = anotherRk.baseSeqPermutation;

			for (int i = 0; i < Length; i++)
			{
				if (thisPerm[i] != anotherPerm[i])
				{
					return false;
				}
			}
			// the permutations are the same
			return true;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void checkValidity(final java.util.List<Double> chromosomeRepresentation) throws InvalidRepresentationException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal override void checkValidity(IList<double?> chromosomeRepresentation)
		{

			foreach (double val in chromosomeRepresentation)
			{
				if (val < 0 || val > 1)
				{
					throw new InvalidRepresentationException(LocalizedFormats.OUT_OF_RANGE_SIMPLE, val, 0, 1);
				}
			}
		}


		/// <summary>
		/// Generates a representation corresponding to a random permutation of
		/// length l which can be passed to the RandomKey constructor.
		/// </summary>
		/// <param name="l"> length of the permutation </param>
		/// <returns> representation of a random permutation </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static final java.util.List<Double> randomPermutation(final int l)
		public static IList<double?> randomPermutation(int l)
		{
			IList<double?> repr = new List<double?>(l);
			for (int i = 0; i < l; i++)
			{
				repr.Add(GeneticAlgorithm.RandomGenerator.NextDouble());
			}
			return repr;
		}

		/// <summary>
		/// Generates a representation corresponding to an identity permutation of
		/// length l which can be passed to the RandomKey constructor.
		/// </summary>
		/// <param name="l"> length of the permutation </param>
		/// <returns> representation of an identity permutation </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static final java.util.List<Double> identityPermutation(final int l)
		public static IList<double?> identityPermutation(int l)
		{
			IList<double?> repr = new List<double?>(l);
			for (int i = 0; i < l; i++)
			{
				repr.Add((double)i / l);
			}
			return repr;
		}

		/// <summary>
		/// Generates a representation of a permutation corresponding to the
		/// <code>data</code> sorted by <code>comparator</code>. The
		/// <code>data</code> is not modified during the process.
		/// 
		/// This is useful if you want to inject some permutations to the initial
		/// population.
		/// </summary>
		/// @param <S> type of the data </param>
		/// <param name="data"> list of data determining the order </param>
		/// <param name="comparator"> how the data will be compared </param>
		/// <returns> list representation of the permutation corresponding to the parameters </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <S> java.util.List<Double> comparatorPermutation(final java.util.List<S> data, final java.util.Comparator<S> comparator)
		public static IList<double?> comparatorPermutation<S>(IList<S> data, IComparer<S> comparator)
		{
			IList<S> sortedData = new List<S>(data);
			sortedData.Sort(comparator);

			return inducedPermutation(data, sortedData);
		}

		/// <summary>
		/// Generates a representation of a permutation corresponding to a
		/// permutation which yields <code>permutedData</code> when applied to
		/// <code>originalData</code>.
		/// 
		/// This method can be viewed as an inverse to <seealso cref="#decode(List)"/>.
		/// </summary>
		/// @param <S> type of the data </param>
		/// <param name="originalData"> the original, unpermuted data </param>
		/// <param name="permutedData"> the data, somehow permuted </param>
		/// <returns> representation of a permutation corresponding to the permutation
		///   <code>originalData -> permutedData</code> </returns>
		/// <exception cref="DimensionMismatchException"> iff the length of <code>originalData</code>
		///   and <code>permutedData</code> lists are not equal </exception>
		/// <exception cref="MathIllegalArgumentException"> iff the <code>permutedData</code> and
		///   <code>originalData</code> lists contain different data </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <S> java.util.List<Double> inducedPermutation(final java.util.List<S> originalData, final java.util.List<S> permutedData) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static IList<double?> inducedPermutation<S>(IList<S> originalData, IList<S> permutedData)
		{

			if (originalData.Count != permutedData.Count)
			{
				throw new DimensionMismatchException(permutedData.Count, originalData.Count);
			}
			int l = originalData.Count;

			IList<S> origDataCopy = new List<S> (originalData);

			double?[] res = new double?[l];
			for (int i = 0; i < l; i++)
			{
				int index = origDataCopy.IndexOf(permutedData[i]);
				if (index == -1)
				{
					throw new MathIllegalArgumentException(LocalizedFormats.DIFFERENT_ORIG_AND_PERMUTED_DATA);
				}
				res[index] = (double) i / l;
				origDataCopy[index] = null;
			}
			return Arrays.asList(res);
		}

		public override string ToString()
		{
			return string.Format("(f={0} pi=({1}))", Fitness, baseSeqPermutation);
		}

		/// <summary>
		/// Helper for constructor. Generates a list of natural numbers (0,1,...,l-1).
		/// </summary>
		/// <param name="l"> length of list to generate </param>
		/// <returns> list of integers from 0 to l-1 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static java.util.List<Integer> baseSequence(final int l)
		private static IList<int?> baseSequence(int l)
		{
			IList<int?> baseSequence = new List<int?> (l);
			for (int i = 0; i < l; i++)
			{
				baseSequence.Add(i);
			}
			return baseSequence;
		}
	}

}
using System;
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
namespace mathlib.distribution
{


	using MathArithmeticException = mathlib.exception.MathArithmeticException;
	using NotANumberException = mathlib.exception.NotANumberException;
	using NotFiniteNumberException = mathlib.exception.NotFiniteNumberException;
	using NotPositiveException = mathlib.exception.NotPositiveException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;
	using MathArrays = mathlib.util.MathArrays;
	using mathlib.util;

	/// <summary>
	/// <p>A generic implementation of a
	/// <a href="http://en.wikipedia.org/wiki/Probability_distribution#Discrete_probability_distribution">
	/// discrete probability distribution (Wikipedia)</a> over a finite sample space,
	/// based on an enumerated list of &lt;value, probability&gt; pairs.  Input probabilities must all be non-negative,
	/// but zero values are allowed and their sum does not have to equal one. Constructors will normalize input
	/// probabilities to make them sum to one.</p>
	/// 
	/// <p>The list of <value, probability> pairs does not, strictly speaking, have to be a function and it can
	/// contain null values.  The pmf created by the constructor will combine probabilities of equal values and
	/// will treat null values as equal.  For example, if the list of pairs &lt;"dog", 0.2&gt;, &lt;null, 0.1&gt;,
	/// &lt;"pig", 0.2&gt;, &lt;"dog", 0.1&gt;, &lt;null, 0.4&gt; is provided to the constructor, the resulting
	/// pmf will assign mass of 0.5 to null, 0.3 to "dog" and 0.2 to null.</p>
	/// </summary>
	/// @param <T> type of the elements in the sample space.
	/// @version $Id: EnumeratedDistribution.java 1456769 2013-03-15 04:51:34Z psteitz $
	/// @since 3.2 </param>
	[Serializable]
	public class EnumeratedDistribution<T>
	{

		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 20123308L;

		/// <summary>
		/// RNG instance used to generate samples from the distribution.
		/// </summary>
		protected internal readonly RandomGenerator random;

		/// <summary>
		/// List of random variable values.
		/// </summary>
		private readonly IList<T> singletons;
		/// <summary>
		/// Probabilities of respective random variable values. For i = 0, ..., singletons.size() - 1,
		/// probability[i] is the probability that a random variable following this distribution takes
		/// the value singletons[i].
		/// </summary>
		private readonly double[] probabilities;

		/// <summary>
		/// Create an enumerated distribution using the given probability mass function
		/// enumeration.
		/// </summary>
		/// <param name="pmf"> probability mass function enumerated as a list of <T, probability>
		/// pairs. </param>
		/// <exception cref="NotPositiveException"> if any of the probabilities are negative. </exception>
		/// <exception cref="NotFiniteNumberException"> if any of the probabilities are infinite. </exception>
		/// <exception cref="NotANumberException"> if any of the probabilities are NaN. </exception>
		/// <exception cref="MathArithmeticException"> all of the probabilities are 0. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public EnumeratedDistribution(final java.util.List<mathlib.util.Pair<T, Double>> pmf) throws mathlib.exception.NotPositiveException, mathlib.exception.MathArithmeticException, mathlib.exception.NotFiniteNumberException, mathlib.exception.NotANumberException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public EnumeratedDistribution(IList<Pair<T, double?>> pmf) : this(new Well19937c(), pmf)
		{
		}

		/// <summary>
		/// Create an enumerated distribution using the given random number generator
		/// and probability mass function enumeration.
		/// </summary>
		/// <param name="rng"> random number generator. </param>
		/// <param name="pmf"> probability mass function enumerated as a list of <T, probability>
		/// pairs. </param>
		/// <exception cref="NotPositiveException"> if any of the probabilities are negative. </exception>
		/// <exception cref="NotFiniteNumberException"> if any of the probabilities are infinite. </exception>
		/// <exception cref="NotANumberException"> if any of the probabilities are NaN. </exception>
		/// <exception cref="MathArithmeticException"> all of the probabilities are 0. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public EnumeratedDistribution(final mathlib.random.RandomGenerator rng, final java.util.List<mathlib.util.Pair<T, Double>> pmf) throws mathlib.exception.NotPositiveException, mathlib.exception.MathArithmeticException, mathlib.exception.NotFiniteNumberException, mathlib.exception.NotANumberException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public EnumeratedDistribution(RandomGenerator rng, IList<Pair<T, double?>> pmf)
		{
			random = rng;

			singletons = new List<T>(pmf.Count);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] probs = new double[pmf.size()];
			double[] probs = new double[pmf.Count];

			for (int i = 0; i < pmf.Count; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.util.Pair<T, Double> sample = pmf.get(i);
				Pair<T, double?> sample = pmf[i];
				singletons.Add(sample.Key);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double p = sample.getValue();
				double p = sample.Value;
				if (p < 0)
				{
					throw new NotPositiveException(sample.Value);
				}
				if (double.IsInfinity(p))
				{
					throw new NotFiniteNumberException(p);
				}
				if (double.IsNaN(p))
				{
					throw new NotANumberException();
				}
				probs[i] = p;
			}

			probabilities = MathArrays.normalizeArray(probs, 1.0);
		}

		/// <summary>
		/// Reseed the random generator used to generate samples.
		/// </summary>
		/// <param name="seed"> the new seed </param>
		public virtual void reseedRandomGenerator(long seed)
		{
			random.Seed = seed;
		}

		/// <summary>
		/// <p>For a random variable {@code X} whose values are distributed according to
		/// this distribution, this method returns {@code P(X = x)}. In other words,
		/// this method represents the probability mass function (PMF) for the
		/// distribution.</p>
		/// 
		/// <p>Note that if {@code x1} and {@code x2} satisfy {@code x1.equals(x2)},
		/// or both are null, then {@code probability(x1) = probability(x2)}.</p>
		/// </summary>
		/// <param name="x"> the point at which the PMF is evaluated </param>
		/// <returns> the value of the probability mass function at {@code x} </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: double probability(final T x)
		internal virtual double probability(T x)
		{
			double probability = 0;

			for (int i = 0; i < probabilities.Length; i++)
			{
				if ((x == null && singletons[i] == null) || (x != null && x.Equals(singletons[i])))
				{
					probability += probabilities[i];
				}
			}

			return probability;
		}

		/// <summary>
		/// <p>Return the probability mass function as a list of <value, probability> pairs.</p>
		/// 
		/// <p>Note that if duplicate and / or null values were provided to the constructor
		/// when creating this EnumeratedDistribution, the returned list will contain these
		/// values.  If duplicates values exist, what is returned will not represent
		/// a pmf (i.e., it is up to the caller to consolidate duplicate mass points).</p>
		/// </summary>
		/// <returns> the probability mass function. </returns>
		public virtual IList<Pair<T, double?>> Pmf
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.List<mathlib.util.Pair<T, Double>> samples = new java.util.ArrayList<mathlib.util.Pair<T, Double>>(probabilities.length);
				IList<Pair<T, double?>> samples = new List<Pair<T, double?>>(probabilities.Length);
    
				for (int i = 0; i < probabilities.Length; i++)
				{
					samples.Add(new Pair<T, double?>(singletons[i], probabilities[i]));
				}
    
				return samples;
			}
		}

		/// <summary>
		/// Generate a random value sampled from this distribution.
		/// </summary>
		/// <returns> a random value. </returns>
		public virtual T sample()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double randomValue = random.nextDouble();
			double randomValue = random.NextDouble();
			double sum = 0;

			for (int i = 0; i < probabilities.Length; i++)
			{
				sum += probabilities[i];
				if (randomValue < sum)
				{
					return singletons[i];
				}
			}

			/* This should never happen, but it ensures we will return a correct
			 * object in case the loop above has some floating point inequality
			 * problem on the final iteration. */
			return singletons[singletons.Count - 1];
		}

		/// <summary>
		/// Generate a random sample from the distribution.
		/// </summary>
		/// <param name="sampleSize"> the number of random values to generate. </param>
		/// <returns> an array representing the random sample. </returns>
		/// <exception cref="NotStrictlyPositiveException"> if {@code sampleSize} is not
		/// positive. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object[] sample(int sampleSize) throws mathlib.exception.NotStrictlyPositiveException
		public virtual object[] sample(int sampleSize)
		{
			if (sampleSize <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.NUMBER_OF_SAMPLES, sampleSize);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] out = new Object[sampleSize];
			object[] @out = new object[sampleSize];

			for (int i = 0; i < sampleSize; i++)
			{
				@out[i] = sample();
			}

			return @out;

		}

		/// <summary>
		/// Generate a random sample from the distribution.
		/// <p>
		/// If the requested samples fit in the specified array, it is returned
		/// therein. Otherwise, a new array is allocated with the runtime type of
		/// the specified array and the size of this collection.
		/// </summary>
		/// <param name="sampleSize"> the number of random values to generate. </param>
		/// <param name="array"> the array to populate. </param>
		/// <returns> an array representing the random sample. </returns>
		/// <exception cref="NotStrictlyPositiveException"> if {@code sampleSize} is not positive. </exception>
		/// <exception cref="NullArgumentException"> if {@code array} is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T[] sample(int sampleSize, final T[] array) throws mathlib.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T[] sample(int sampleSize, T[] array)
		{
			if (sampleSize <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.NUMBER_OF_SAMPLES, sampleSize);
			}

			if (array == null)
			{
				throw new NullArgumentException(LocalizedFormats.INPUT_ARRAY);
			}

			T[] @out;
			if (array.Length < sampleSize)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final T[] unchecked = (T[]) Array.newInstance(array.getClass().getComponentType(), sampleSize);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
				T[] @unchecked = (T[]) Array.newInstance(array.GetType().GetElementType(), sampleSize); // safe as both are of type T
				@out = @unchecked;
			}
			else
			{
				@out = array;
			}

			for (int i = 0; i < sampleSize; i++)
			{
				@out[i] = sample();
			}

			return @out;

		}

	}

}
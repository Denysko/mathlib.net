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

namespace mathlib.random
{


	using IntegerDistribution = mathlib.distribution.IntegerDistribution;
	using RealDistribution = mathlib.distribution.RealDistribution;
	using NotANumberException = mathlib.exception.NotANumberException;
	using NotFiniteNumberException = mathlib.exception.NotFiniteNumberException;
	using NotPositiveException = mathlib.exception.NotPositiveException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;

	/// <summary>
	/// Generates random deviates and other random data using a <seealso cref="RandomGenerator"/>
	/// instance to generate non-secure data and a <seealso cref="java.security.SecureRandom"/>
	/// instance to provide data for the <code>nextSecureXxx</code> methods. If no
	/// <code>RandomGenerator</code> is provided in the constructor, the default is
	/// to use a <seealso cref="Well19937c"/> generator. To plug in a different
	/// implementation, either implement <code>RandomGenerator</code> directly or
	/// extend <seealso cref="AbstractRandomGenerator"/>.
	/// <p>
	/// Supports reseeding the underlying pseudo-random number generator (PRNG). The
	/// <code>SecurityProvider</code> and <code>Algorithm</code> used by the
	/// <code>SecureRandom</code> instance can also be reset.
	/// </p>
	/// <p>
	/// For details on the default PRNGs, see <seealso cref="java.util.Random"/> and
	/// <seealso cref="java.security.SecureRandom"/>.
	/// </p>
	/// <p>
	/// <strong>Usage Notes</strong>:
	/// <ul>
	/// <li>
	/// Instance variables are used to maintain <code>RandomGenerator</code> and
	/// <code>SecureRandom</code> instances used in data generation. Therefore, to
	/// generate a random sequence of values or strings, you should use just
	/// <strong>one</strong> <code>RandomDataGenerator</code> instance repeatedly.</li>
	/// <li>
	/// The "secure" methods are *much* slower. These should be used only when a
	/// cryptographically secure random sequence is required. A secure random
	/// sequence is a sequence of pseudo-random values which, in addition to being
	/// well-dispersed (so no subsequence of values is an any more likely than other
	/// subsequence of the the same length), also has the additional property that
	/// knowledge of values generated up to any point in the sequence does not make
	/// it any easier to predict subsequent values.</li>
	/// <li>
	/// When a new <code>RandomDataGenerator</code> is created, the underlying random
	/// number generators are <strong>not</strong> initialized. If you do not
	/// explicitly seed the default non-secure generator, it is seeded with the
	/// current time in milliseconds plus the system identity hash code on first use.
	/// The same holds for the secure generator. If you provide a <code>RandomGenerator</code>
	/// to the constructor, however, this generator is not reseeded by the constructor
	/// nor is it reseeded on first use.</li>
	/// <li>
	/// The <code>reSeed</code> and <code>reSeedSecure</code> methods delegate to the
	/// corresponding methods on the underlying <code>RandomGenerator</code> and
	/// <code>SecureRandom</code> instances. Therefore, <code>reSeed(long)</code>
	/// fully resets the initial state of the non-secure random number generator (so
	/// that reseeding with a specific value always results in the same subsequent
	/// random sequence); whereas reSeedSecure(long) does <strong>not</strong>
	/// reinitialize the secure random number generator (so secure sequences started
	/// with calls to reseedSecure(long) won't be identical).</li>
	/// <li>
	/// This implementation is not synchronized. The underlying <code>RandomGenerator</code>
	/// or <code>SecureRandom</code> instances are not protected by synchronization and
	/// are not guaranteed to be thread-safe.  Therefore, if an instance of this class
	/// is concurrently utilized by multiple threads, it is the responsibility of
	/// client code to synchronize access to seeding and data generation methods.
	/// </li>
	/// </ul>
	/// </p> </summary>
	/// @deprecated to be removed in 4.0.  Use <seealso cref="RandomDataGenerator"/> instead
	/// @version $Id: RandomDataImpl.java 1499808 2013-07-04 17:00:42Z sebb $ 
	[Obsolete("to be removed in 4.0.  Use <seealso cref="RandomDataGenerator"/> instead"), Serializable]
	public class RandomDataImpl : RandomData
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -626730818244969716L;

		/// <summary>
		/// RandomDataGenerator delegate </summary>
		private readonly RandomDataGenerator @delegate;

		/// <summary>
		/// Construct a RandomDataImpl, using a default random generator as the source
		/// of randomness.
		/// 
		/// <p>The default generator is a <seealso cref="Well19937c"/> seeded
		/// with {@code System.currentTimeMillis() + System.identityHashCode(this))}.
		/// The generator is initialized and seeded on first use.</p>
		/// </summary>
		public RandomDataImpl()
		{
			@delegate = new RandomDataGenerator();
		}

		/// <summary>
		/// Construct a RandomDataImpl using the supplied <seealso cref="RandomGenerator"/> as
		/// the source of (non-secure) random data.
		/// </summary>
		/// <param name="rand"> the source of (non-secure) random data
		/// (may be null, resulting in the default generator)
		/// @since 1.1 </param>
		public RandomDataImpl(RandomGenerator rand)
		{
			@delegate = new RandomDataGenerator(rand);
		}

		/// <returns> the delegate object. </returns>
		/// @deprecated To be removed in 4.0. 
		[Obsolete("To be removed in 4.0.")]
		internal virtual RandomDataGenerator Delegate
		{
			get
			{
				return @delegate;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// <strong>Algorithm Description:</strong> hex strings are generated using a
		/// 2-step process.
		/// <ol>
		/// <li>{@code len / 2 + 1} binary bytes are generated using the underlying
		/// Random</li>
		/// <li>Each binary byte is translated into 2 hex digits</li>
		/// </ol>
		/// </p>
		/// </summary>
		/// <param name="len"> the desired string length. </param>
		/// <returns> the random string. </returns>
		/// <exception cref="NotStrictlyPositiveException"> if {@code len <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String nextHexString(int len) throws mathlib.exception.NotStrictlyPositiveException
		public virtual string nextHexString(int len)
		{
			return @delegate.nextHexString(len);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextInt(int lower, int upper) throws mathlib.exception.NumberIsTooLargeException
		public virtual int nextInt(int lower, int upper)
		{
		   return @delegate.Next(lower, upper);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long nextLong(long lower, long upper) throws mathlib.exception.NumberIsTooLargeException
		public virtual long nextLong(long lower, long upper)
		{
			return @delegate.nextLong(lower, upper);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// <strong>Algorithm Description:</strong> hex strings are generated in
		/// 40-byte segments using a 3-step process.
		/// <ol>
		/// <li>
		/// 20 random bytes are generated using the underlying
		/// <code>SecureRandom</code>.</li>
		/// <li>
		/// SHA-1 hash is applied to yield a 20-byte binary digest.</li>
		/// <li>
		/// Each byte of the binary digest is converted to 2 hex digits.</li>
		/// </ol>
		/// </p>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String nextSecureHexString(int len) throws mathlib.exception.NotStrictlyPositiveException
		public virtual string nextSecureHexString(int len)
		{
			return @delegate.nextSecureHexString(len);
		}

		/// <summary>
		///  {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextSecureInt(int lower, int upper) throws mathlib.exception.NumberIsTooLargeException
		public virtual int nextSecureInt(int lower, int upper)
		{
			return @delegate.nextSecureInt(lower, upper);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long nextSecureLong(long lower, long upper) throws mathlib.exception.NumberIsTooLargeException
		public virtual long nextSecureLong(long lower, long upper)
		{
			return @delegate.nextSecureLong(lower,upper);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// <strong>Algorithm Description</strong>:
		/// <ul><li> For small means, uses simulation of a Poisson process
		/// using Uniform deviates, as described
		/// <a href="http://irmi.epfl.ch/cmos/Pmmi/interactive/rng7.htm"> here.</a>
		/// The Poisson process (and hence value returned) is bounded by 1000 * mean.</li>
		/// 
		/// <li> For large means, uses the rejection algorithm described in <br/>
		/// Devroye, Luc. (1981).<i>The Computer Generation of Poisson Random Variables</i>
		/// <strong>Computing</strong> vol. 26 pp. 197-207.</li></ul></p>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long nextPoisson(double mean) throws mathlib.exception.NotStrictlyPositiveException
		public virtual long nextPoisson(double mean)
		{
			return @delegate.nextPoisson(mean);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double nextGaussian(double mu, double sigma) throws mathlib.exception.NotStrictlyPositiveException
		public virtual double nextGaussian(double mu, double sigma)
		{
			return @delegate.nextGaussian(mu,sigma);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <p>
		/// <strong>Algorithm Description</strong>: Uses the Algorithm SA (Ahrens)
		/// from p. 876 in:
		/// [1]: Ahrens, J. H. and Dieter, U. (1972). Computer methods for
		/// sampling from the exponential and normal distributions.
		/// Communications of the ACM, 15, 873-882.
		/// </p>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double nextExponential(double mean) throws mathlib.exception.NotStrictlyPositiveException
		public virtual double nextExponential(double mean)
		{
			return @delegate.nextExponential(mean);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <p>
		/// <strong>Algorithm Description</strong>: scales the output of
		/// Random.nextDouble(), but rejects 0 values (i.e., will generate another
		/// random double if Random.nextDouble() returns 0). This is necessary to
		/// provide a symmetric output interval (both endpoints excluded).
		/// </p>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double nextUniform(double lower, double upper) throws mathlib.exception.NumberIsTooLargeException, mathlib.exception.NotFiniteNumberException, mathlib.exception.NotANumberException
		public virtual double nextUniform(double lower, double upper)
		{
			return @delegate.nextUniform(lower, upper);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <p>
		/// <strong>Algorithm Description</strong>: if the lower bound is excluded,
		/// scales the output of Random.nextDouble(), but rejects 0 values (i.e.,
		/// will generate another random double if Random.nextDouble() returns 0).
		/// This is necessary to provide a symmetric output interval (both
		/// endpoints excluded).
		/// </p>
		/// @since 3.0
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double nextUniform(double lower, double upper, boolean lowerInclusive) throws mathlib.exception.NumberIsTooLargeException, mathlib.exception.NotFiniteNumberException, mathlib.exception.NotANumberException
		public virtual double nextUniform(double lower, double upper, bool lowerInclusive)
		{
			return @delegate.nextUniform(lower, upper, lowerInclusive);
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="mathlib.distribution.BetaDistribution Beta Distribution"/>.
		/// This implementation uses <seealso cref="#nextInversionDeviate(RealDistribution) inversion"/>
		/// to generate random values.
		/// </summary>
		/// <param name="alpha"> first distribution shape parameter </param>
		/// <param name="beta"> second distribution shape parameter </param>
		/// <returns> random value sampled from the beta(alpha, beta) distribution
		/// @since 2.2 </returns>
		public virtual double nextBeta(double alpha, double beta)
		{
			return @delegate.nextBeta(alpha, beta);
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="mathlib.distribution.BinomialDistribution Binomial Distribution"/>.
		/// This implementation uses <seealso cref="#nextInversionDeviate(RealDistribution) inversion"/>
		/// to generate random values.
		/// </summary>
		/// <param name="numberOfTrials"> number of trials of the Binomial distribution </param>
		/// <param name="probabilityOfSuccess"> probability of success of the Binomial distribution </param>
		/// <returns> random value sampled from the Binomial(numberOfTrials, probabilityOfSuccess) distribution
		/// @since 2.2 </returns>
		public virtual int nextBinomial(int numberOfTrials, double probabilityOfSuccess)
		{
			return @delegate.nextBinomial(numberOfTrials, probabilityOfSuccess);
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="mathlib.distribution.CauchyDistribution Cauchy Distribution"/>.
		/// This implementation uses <seealso cref="#nextInversionDeviate(RealDistribution) inversion"/>
		/// to generate random values.
		/// </summary>
		/// <param name="median"> the median of the Cauchy distribution </param>
		/// <param name="scale"> the scale parameter of the Cauchy distribution </param>
		/// <returns> random value sampled from the Cauchy(median, scale) distribution
		/// @since 2.2 </returns>
		public virtual double nextCauchy(double median, double scale)
		{
			return @delegate.nextCauchy(median, scale);
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="mathlib.distribution.ChiSquaredDistribution ChiSquare Distribution"/>.
		/// This implementation uses <seealso cref="#nextInversionDeviate(RealDistribution) inversion"/>
		/// to generate random values.
		/// </summary>
		/// <param name="df"> the degrees of freedom of the ChiSquare distribution </param>
		/// <returns> random value sampled from the ChiSquare(df) distribution
		/// @since 2.2 </returns>
		public virtual double nextChiSquare(double df)
		{
		   return @delegate.nextChiSquare(df);
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="mathlib.distribution.FDistribution F Distribution"/>.
		/// This implementation uses <seealso cref="#nextInversionDeviate(RealDistribution) inversion"/>
		/// to generate random values.
		/// </summary>
		/// <param name="numeratorDf"> the numerator degrees of freedom of the F distribution </param>
		/// <param name="denominatorDf"> the denominator degrees of freedom of the F distribution </param>
		/// <returns> random value sampled from the F(numeratorDf, denominatorDf) distribution </returns>
		/// <exception cref="NotStrictlyPositiveException"> if
		/// {@code numeratorDf <= 0} or {@code denominatorDf <= 0}.
		/// @since 2.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double nextF(double numeratorDf, double denominatorDf) throws mathlib.exception.NotStrictlyPositiveException
		public virtual double nextF(double numeratorDf, double denominatorDf)
		{
			return @delegate.nextF(numeratorDf, denominatorDf);
		}

		/// <summary>
		/// <p>Generates a random value from the
		/// <seealso cref="mathlib.distribution.GammaDistribution Gamma Distribution"/>.</p>
		/// 
		/// <p>This implementation uses the following algorithms: </p>
		/// 
		/// <p>For 0 < shape < 1: <br/>
		/// Ahrens, J. H. and Dieter, U., <i>Computer methods for
		/// sampling from gamma, beta, Poisson and binomial distributions.</i>
		/// Computing, 12, 223-246, 1974.</p>
		/// 
		/// <p>For shape >= 1: <br/>
		/// Marsaglia and Tsang, <i>A Simple Method for Generating
		/// Gamma Variables.</i> ACM Transactions on Mathematical Software,
		/// Volume 26 Issue 3, September, 2000.</p>
		/// </summary>
		/// <param name="shape"> the median of the Gamma distribution </param>
		/// <param name="scale"> the scale parameter of the Gamma distribution </param>
		/// <returns> random value sampled from the Gamma(shape, scale) distribution </returns>
		/// <exception cref="NotStrictlyPositiveException"> if {@code shape <= 0} or
		/// {@code scale <= 0}.
		/// @since 2.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double nextGamma(double shape, double scale) throws mathlib.exception.NotStrictlyPositiveException
		public virtual double nextGamma(double shape, double scale)
		{
			return @delegate.nextGamma(shape, scale);
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="mathlib.distribution.HypergeometricDistribution Hypergeometric Distribution"/>.
		/// This implementation uses <seealso cref="#nextInversionDeviate(IntegerDistribution) inversion"/>
		/// to generate random values.
		/// </summary>
		/// <param name="populationSize"> the population size of the Hypergeometric distribution </param>
		/// <param name="numberOfSuccesses"> number of successes in the population of the Hypergeometric distribution </param>
		/// <param name="sampleSize"> the sample size of the Hypergeometric distribution </param>
		/// <returns> random value sampled from the Hypergeometric(numberOfSuccesses, sampleSize) distribution </returns>
		/// <exception cref="NumberIsTooLargeException">  if {@code numberOfSuccesses > populationSize},
		/// or {@code sampleSize > populationSize}. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if {@code populationSize <= 0}. </exception>
		/// <exception cref="NotPositiveException">  if {@code numberOfSuccesses < 0}.
		/// @since 2.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextHypergeometric(int populationSize, int numberOfSuccesses, int sampleSize) throws mathlib.exception.NotPositiveException, mathlib.exception.NotStrictlyPositiveException, mathlib.exception.NumberIsTooLargeException
		public virtual int nextHypergeometric(int populationSize, int numberOfSuccesses, int sampleSize)
		{
			return @delegate.nextHypergeometric(populationSize, numberOfSuccesses, sampleSize);
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="mathlib.distribution.PascalDistribution Pascal Distribution"/>.
		/// This implementation uses <seealso cref="#nextInversionDeviate(IntegerDistribution) inversion"/>
		/// to generate random values.
		/// </summary>
		/// <param name="r"> the number of successes of the Pascal distribution </param>
		/// <param name="p"> the probability of success of the Pascal distribution </param>
		/// <returns> random value sampled from the Pascal(r, p) distribution
		/// @since 2.2 </returns>
		/// <exception cref="NotStrictlyPositiveException"> if the number of successes is not positive </exception>
		/// <exception cref="OutOfRangeException"> if the probability of success is not in the
		/// range {@code [0, 1]}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextPascal(int r, double p) throws mathlib.exception.NotStrictlyPositiveException, mathlib.exception.OutOfRangeException
		public virtual int nextPascal(int r, double p)
		{
			return @delegate.nextPascal(r, p);
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="mathlib.distribution.TDistribution T Distribution"/>.
		/// This implementation uses <seealso cref="#nextInversionDeviate(RealDistribution) inversion"/>
		/// to generate random values.
		/// </summary>
		/// <param name="df"> the degrees of freedom of the T distribution </param>
		/// <returns> random value from the T(df) distribution
		/// @since 2.2 </returns>
		/// <exception cref="NotStrictlyPositiveException"> if {@code df <= 0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double nextT(double df) throws mathlib.exception.NotStrictlyPositiveException
		public virtual double nextT(double df)
		{
			return @delegate.nextT(df);
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="mathlib.distribution.WeibullDistribution Weibull Distribution"/>.
		/// This implementation uses <seealso cref="#nextInversionDeviate(RealDistribution) inversion"/>
		/// to generate random values.
		/// </summary>
		/// <param name="shape"> the shape parameter of the Weibull distribution </param>
		/// <param name="scale"> the scale parameter of the Weibull distribution </param>
		/// <returns> random value sampled from the Weibull(shape, size) distribution
		/// @since 2.2 </returns>
		/// <exception cref="NotStrictlyPositiveException"> if {@code shape <= 0} or
		/// {@code scale <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double nextWeibull(double shape, double scale) throws mathlib.exception.NotStrictlyPositiveException
		public virtual double nextWeibull(double shape, double scale)
		{
			return @delegate.nextWeibull(shape, scale);
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="mathlib.distribution.ZipfDistribution Zipf Distribution"/>.
		/// This implementation uses <seealso cref="#nextInversionDeviate(IntegerDistribution) inversion"/>
		/// to generate random values.
		/// </summary>
		/// <param name="numberOfElements"> the number of elements of the ZipfDistribution </param>
		/// <param name="exponent"> the exponent of the ZipfDistribution </param>
		/// <returns> random value sampled from the Zipf(numberOfElements, exponent) distribution
		/// @since 2.2 </returns>
		/// <exception cref="NotStrictlyPositiveException"> if {@code numberOfElements <= 0}
		/// or {@code exponent <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextZipf(int numberOfElements, double exponent) throws mathlib.exception.NotStrictlyPositiveException
		public virtual int nextZipf(int numberOfElements, double exponent)
		{
			return @delegate.nextZipf(numberOfElements, exponent);
		}


		/// <summary>
		/// Reseeds the random number generator with the supplied seed.
		/// <p>
		/// Will create and initialize if null.
		/// </p>
		/// </summary>
		/// <param name="seed">
		///            the seed value to use </param>
		public virtual void reSeed(long seed)
		{
			@delegate.reSeed(seed);
		}

		/// <summary>
		/// Reseeds the secure random number generator with the current time in
		/// milliseconds.
		/// <p>
		/// Will create and initialize if null.
		/// </p>
		/// </summary>
		public virtual void reSeedSecure()
		{
			@delegate.reSeedSecure();
		}

		/// <summary>
		/// Reseeds the secure random number generator with the supplied seed.
		/// <p>
		/// Will create and initialize if null.
		/// </p>
		/// </summary>
		/// <param name="seed">
		///            the seed value to use </param>
		public virtual void reSeedSecure(long seed)
		{
			@delegate.reSeedSecure(seed);
		}

		/// <summary>
		/// Reseeds the random number generator with
		/// {@code System.currentTimeMillis() + System.identityHashCode(this))}.
		/// </summary>
		public virtual void reSeed()
		{
			@delegate.reSeed();
		}

		/// <summary>
		/// Sets the PRNG algorithm for the underlying SecureRandom instance using
		/// the Security Provider API. The Security Provider API is defined in <a
		/// href =
		/// "http://java.sun.com/j2se/1.3/docs/guide/security/CryptoSpec.html#AppA">
		/// Java Cryptography Architecture API Specification & Reference.</a>
		/// <p>
		/// <strong>USAGE NOTE:</strong> This method carries <i>significant</i>
		/// overhead and may take several seconds to execute.
		/// </p>
		/// </summary>
		/// <param name="algorithm">
		///            the name of the PRNG algorithm </param>
		/// <param name="provider">
		///            the name of the provider </param>
		/// <exception cref="NoSuchAlgorithmException">
		///             if the specified algorithm is not available </exception>
		/// <exception cref="NoSuchProviderException">
		///             if the specified provider is not installed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSecureAlgorithm(String algorithm, String provider) throws java.security.NoSuchAlgorithmException, java.security.NoSuchProviderException
		public virtual void setSecureAlgorithm(string algorithm, string provider)
		{
		   @delegate.setSecureAlgorithm(algorithm, provider);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <p>
		/// Uses a 2-cycle permutation shuffle. The shuffling process is described <a
		/// href="http://www.maths.abdn.ac.uk/~igc/tch/mx4002/notes/node83.html">
		/// here</a>.
		/// </p>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int[] nextPermutation(int n, int k) throws mathlib.exception.NotStrictlyPositiveException, mathlib.exception.NumberIsTooLargeException
		public virtual int[] nextPermutation(int n, int k)
		{
			return @delegate.nextPermutation(n, k);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <p>
		/// <strong>Algorithm Description</strong>: Uses a 2-cycle permutation
		/// shuffle to generate a random permutation of <code>c.size()</code> and
		/// then returns the elements whose indexes correspond to the elements of the
		/// generated permutation. This technique is described, and proven to
		/// generate random samples <a
		/// href="http://www.maths.abdn.ac.uk/~igc/tch/mx4002/notes/node83.html">
		/// here</a>
		/// </p>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object[] nextSample(java.util.Collection<?> c, int k) throws mathlib.exception.NotStrictlyPositiveException, mathlib.exception.NumberIsTooLargeException
		public virtual object[] nextSample<T1>(ICollection<T1> c, int k)
		{
			return @delegate.nextSample(c, k);
		}

		/// <summary>
		/// Generate a random deviate from the given distribution using the
		/// <a href="http://en.wikipedia.org/wiki/Inverse_transform_sampling"> inversion method.</a>
		/// </summary>
		/// <param name="distribution"> Continuous distribution to generate a random value from </param>
		/// <returns> a random value sampled from the given distribution </returns>
		/// <exception cref="MathIllegalArgumentException"> if the underlynig distribution throws one
		/// @since 2.2 </exception>
		/// @deprecated use the distribution's sample() method 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("use the distribution's sample() method") public double nextInversionDeviate(mathlib.distribution.RealDistribution distribution) throws mathlib.exception.MathIllegalArgumentException
		[Obsolete("use the distribution's sample() method")]
		public virtual double nextInversionDeviate(RealDistribution distribution)
		{
			return distribution.inverseCumulativeProbability(nextUniform(0, 1));

		}

		/// <summary>
		/// Generate a random deviate from the given distribution using the
		/// <a href="http://en.wikipedia.org/wiki/Inverse_transform_sampling"> inversion method.</a>
		/// </summary>
		/// <param name="distribution"> Integer distribution to generate a random value from </param>
		/// <returns> a random value sampled from the given distribution </returns>
		/// <exception cref="MathIllegalArgumentException"> if the underlynig distribution throws one
		/// @since 2.2 </exception>
		/// @deprecated use the distribution's sample() method 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("use the distribution's sample() method") public int nextInversionDeviate(mathlib.distribution.IntegerDistribution distribution) throws mathlib.exception.MathIllegalArgumentException
		[Obsolete("use the distribution's sample() method")]
		public virtual int nextInversionDeviate(IntegerDistribution distribution)
		{
			return distribution.inverseCumulativeProbability(nextUniform(0, 1));
		}

	}

}
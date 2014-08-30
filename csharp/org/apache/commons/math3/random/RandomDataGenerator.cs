using System;
using System.Collections.Generic;
using System.Text;

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

namespace org.apache.commons.math3.random
{


	using BetaDistribution = org.apache.commons.math3.distribution.BetaDistribution;
	using BinomialDistribution = org.apache.commons.math3.distribution.BinomialDistribution;
	using CauchyDistribution = org.apache.commons.math3.distribution.CauchyDistribution;
	using ChiSquaredDistribution = org.apache.commons.math3.distribution.ChiSquaredDistribution;
	using ExponentialDistribution = org.apache.commons.math3.distribution.ExponentialDistribution;
	using FDistribution = org.apache.commons.math3.distribution.FDistribution;
	using GammaDistribution = org.apache.commons.math3.distribution.GammaDistribution;
	using HypergeometricDistribution = org.apache.commons.math3.distribution.HypergeometricDistribution;
	using PascalDistribution = org.apache.commons.math3.distribution.PascalDistribution;
	using PoissonDistribution = org.apache.commons.math3.distribution.PoissonDistribution;
	using TDistribution = org.apache.commons.math3.distribution.TDistribution;
	using WeibullDistribution = org.apache.commons.math3.distribution.WeibullDistribution;
	using ZipfDistribution = org.apache.commons.math3.distribution.ZipfDistribution;
	using UniformIntegerDistribution = org.apache.commons.math3.distribution.UniformIntegerDistribution;
	using MathInternalError = org.apache.commons.math3.exception.MathInternalError;
	using NotANumberException = org.apache.commons.math3.exception.NotANumberException;
	using NotFiniteNumberException = org.apache.commons.math3.exception.NotFiniteNumberException;
	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using MathArrays = org.apache.commons.math3.util.MathArrays;

	/// <summary>
	/// Implements the <seealso cref="RandomData"/> interface using a <seealso cref="RandomGenerator"/>
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
	/// <strong>one</strong> <code>RandomDataImpl</code> instance repeatedly.</li>
	/// <li>
	/// The "secure" methods are *much* slower. These should be used only when a
	/// cryptographically secure random sequence is required. A secure random
	/// sequence is a sequence of pseudo-random values which, in addition to being
	/// well-dispersed (so no subsequence of values is an any more likely than other
	/// subsequence of the the same length), also has the additional property that
	/// knowledge of values generated up to any point in the sequence does not make
	/// it any easier to predict subsequent values.</li>
	/// <li>
	/// When a new <code>RandomDataImpl</code> is created, the underlying random
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
	/// </p>
	/// @since 3.1
	/// @version $Id: RandomDataGenerator.java 1538368 2013-11-03 13:57:37Z erans $
	/// </summary>
	[Serializable]
	public class RandomDataGenerator : RandomData
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -626730818244969716L;

		/// <summary>
		/// underlying random number generator </summary>
		private RandomGenerator rand = null;

		/// <summary>
		/// underlying secure random number generator </summary>
		private RandomGenerator secRand = null;

		/// <summary>
		/// Construct a RandomDataGenerator, using a default random generator as the source
		/// of randomness.
		/// 
		/// <p>The default generator is a <seealso cref="Well19937c"/> seeded
		/// with {@code System.currentTimeMillis() + System.identityHashCode(this))}.
		/// The generator is initialized and seeded on first use.</p>
		/// </summary>
		public RandomDataGenerator()
		{
		}

		/// <summary>
		/// Construct a RandomDataGenerator using the supplied <seealso cref="RandomGenerator"/> as
		/// the source of (non-secure) random data.
		/// </summary>
		/// <param name="rand"> the source of (non-secure) random data
		/// (may be null, resulting in the default generator) </param>
		public RandomDataGenerator(RandomGenerator rand)
		{
			this.rand = rand;
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
//ORIGINAL LINE: public String nextHexString(int len) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public virtual string nextHexString(int len)
		{
			if (len <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.LENGTH, len);
			}

			// Get a random number generator
			RandomGenerator ran = RandomGenerator;

			// Initialize output buffer
			StringBuilder outBuffer = new StringBuilder();

			// Get int(len/2)+1 random bytes
			sbyte[] randomBytes = new sbyte[(len / 2) + 1];
			ran.nextBytes(randomBytes);

			// Convert each byte to 2 hex digits
			for (int i = 0; i < randomBytes.Length; i++)
			{
				int? c = Convert.ToInt32(randomBytes[i]);

				/*
				 * Add 128 to byte value to make interval 0-255 before doing hex
				 * conversion. This guarantees <= 2 hex digits from toHexString()
				 * toHexString would otherwise add 2^32 to negative arguments.
				 */
				string hex = (int)c + 128.ToString("x");

				// Make sure we add 2 hex digits for each byte
				if (hex.Length == 1)
				{
					hex = "0" + hex;
				}
				outBuffer.Append(hex);
			}
			return outBuffer.ToString().Substring(0, len);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextInt(final int lower, final int upper) throws org.apache.commons.math3.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual int nextInt(int lower, int upper)
		{
			return (new UniformIntegerDistribution(RandomGenerator, lower, upper)).sample();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long nextLong(final long lower, final long upper) throws org.apache.commons.math3.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual long nextLong(long lower, long upper)
		{
			if (lower >= upper)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.LOWER_BOUND_NOT_BELOW_UPPER_BOUND, lower, upper, false);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long max = (upper - lower) + 1;
			long max = (upper - lower) + 1;
			if (max <= 0)
			{
				// the range is too wide to fit in a positive long (larger than 2^63); as it covers
				// more than half the long range, we use directly a simple rejection method
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RandomGenerator rng = getRandomGenerator();
				RandomGenerator rng = RandomGenerator;
				while (true)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long r = rng.nextLong();
					long r = rng.nextLong();
					if (r >= lower && r <= upper)
					{
						return r;
					}
				}
			}
			else if (max < int.MaxValue)
			{
				// we can shift the range and generate directly a positive int
				return lower + RandomGenerator.Next((int) max);
			}
			else
			{
				// we can shift the range and generate directly a positive long
				return lower + nextLong(RandomGenerator, max);
			}
		}

		/// <summary>
		/// Returns a pseudorandom, uniformly distributed <tt>long</tt> value
		/// between 0 (inclusive) and the specified value (exclusive), drawn from
		/// this random number generator's sequence.
		/// </summary>
		/// <param name="rng"> random generator to use </param>
		/// <param name="n"> the bound on the random number to be returned.  Must be
		/// positive. </param>
		/// <returns>  a pseudorandom, uniformly distributed <tt>long</tt>
		/// value between 0 (inclusive) and n (exclusive). </returns>
		/// <exception cref="IllegalArgumentException">  if n is not positive. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static long nextLong(final RandomGenerator rng, final long n) throws IllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private static long nextLong(RandomGenerator rng, long n)
		{
			if (n > 0)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] byteArray = new byte[8];
				sbyte[] byteArray = new sbyte[8];
				long bits;
				long val;
				do
				{
					rng.nextBytes(byteArray);
					bits = 0;
					foreach (sbyte b in byteArray)
					{
						bits = (bits << 8) | (((long) b) & 0xffL);
					}
					bits &= 0x7fffffffffffffffL;
					val = bits % n;
				} while (bits - val + (n - 1) < 0);
				return val;
			}
			throw new NotStrictlyPositiveException(n);
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
		/// </p> </summary>
		/// <exception cref="NotStrictlyPositiveException"> if {@code len <= 0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String nextSecureHexString(int len) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public virtual string nextSecureHexString(int len)
		{
			if (len <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.LENGTH, len);
			}

			// Get SecureRandom and setup Digest provider
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RandomGenerator secRan = getSecRan();
			RandomGenerator secRan = SecRan;
			MessageDigest alg = null;
			try
			{
				alg = MessageDigest.getInstance("SHA-1");
			}
			catch (NoSuchAlgorithmException ex)
			{
				// this should never happen
				throw new MathInternalError(ex);
			}
			alg.reset();

			// Compute number of iterations required (40 bytes each)
			int numIter = (len / 40) + 1;

			StringBuilder outBuffer = new StringBuilder();
			for (int iter = 1; iter < numIter + 1; iter++)
			{
				sbyte[] randomBytes = new sbyte[40];
				secRan.nextBytes(randomBytes);
				alg.update(randomBytes);

				// Compute hash -- will create 20-byte binary hash
				sbyte[] hash = alg.digest();

				// Loop over the hash, converting each byte to 2 hex digits
				for (int i = 0; i < hash.Length; i++)
				{
					int? c = Convert.ToInt32(hash[i]);

					/*
					 * Add 128 to byte value to make interval 0-255 This guarantees
					 * <= 2 hex digits from toHexString() toHexString would
					 * otherwise add 2^32 to negative arguments
					 */
					string hex = (int)c + 128.ToString("x");

					// Keep strings uniform length -- guarantees 40 bytes
					if (hex.Length == 1)
					{
						hex = "0" + hex;
					}
					outBuffer.Append(hex);
				}
			}
			return outBuffer.ToString().Substring(0, len);
		}

		/// <summary>
		///  {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextSecureInt(final int lower, final int upper) throws org.apache.commons.math3.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual int nextSecureInt(int lower, int upper)
		{
			return (new UniformIntegerDistribution(SecRan, lower, upper)).sample();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long nextSecureLong(final long lower, final long upper) throws org.apache.commons.math3.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual long nextSecureLong(long lower, long upper)
		{
			if (lower >= upper)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.LOWER_BOUND_NOT_BELOW_UPPER_BOUND, lower, upper, false);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RandomGenerator rng = getSecRan();
			RandomGenerator rng = SecRan;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long max = (upper - lower) + 1;
			long max = (upper - lower) + 1;
			if (max <= 0)
			{
				// the range is too wide to fit in a positive long (larger than 2^63); as it covers
				// more than half the long range, we use directly a simple rejection method
				while (true)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long r = rng.nextLong();
					long r = rng.nextLong();
					if (r >= lower && r <= upper)
					{
						return r;
					}
				}
			}
			else if (max < int.MaxValue)
			{
				// we can shift the range and generate directly a positive int
				return lower + rng.Next((int) max);
			}
			else
			{
				// we can shift the range and generate directly a positive long
				return lower + nextLong(rng, max);
			}
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
		/// <strong>Computing</strong> vol. 26 pp. 197-207.</li></ul></p> </summary>
		/// <exception cref="NotStrictlyPositiveException"> if {@code len <= 0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long nextPoisson(double mean) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public virtual long nextPoisson(double mean)
		{
			return (new PoissonDistribution(RandomGenerator, mean, PoissonDistribution.DEFAULT_EPSILON, PoissonDistribution.DEFAULT_MAX_ITERATIONS)).sample();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double nextGaussian(double mu, double sigma) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public virtual double nextGaussian(double mu, double sigma)
		{
			if (sigma <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.STANDARD_DEVIATION, sigma);
			}
			return sigma * RandomGenerator.nextGaussian() + mu;
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
//ORIGINAL LINE: public double nextExponential(double mean) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public virtual double nextExponential(double mean)
		{
			return (new ExponentialDistribution(RandomGenerator, mean, ExponentialDistribution.DEFAULT_INVERSE_ABSOLUTE_ACCURACY)).sample();
		}

		/// <summary>
		/// <p>Generates a random value from the
		/// <seealso cref="org.apache.commons.math3.distribution.GammaDistribution Gamma Distribution"/>.</p>
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
		/// {@code scale <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double nextGamma(double shape, double scale) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public virtual double nextGamma(double shape, double scale)
		{
			return (new GammaDistribution(RandomGenerator,shape, scale, GammaDistribution.DEFAULT_INVERSE_ABSOLUTE_ACCURACY)).sample();
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="HypergeometricDistribution Hypergeometric Distribution"/>.
		/// </summary>
		/// <param name="populationSize"> the population size of the Hypergeometric distribution </param>
		/// <param name="numberOfSuccesses"> number of successes in the population of the Hypergeometric distribution </param>
		/// <param name="sampleSize"> the sample size of the Hypergeometric distribution </param>
		/// <returns> random value sampled from the Hypergeometric(numberOfSuccesses, sampleSize) distribution </returns>
		/// <exception cref="NumberIsTooLargeException">  if {@code numberOfSuccesses > populationSize},
		/// or {@code sampleSize > populationSize}. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if {@code populationSize <= 0}. </exception>
		/// <exception cref="NotPositiveException">  if {@code numberOfSuccesses < 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextHypergeometric(int populationSize, int numberOfSuccesses, int sampleSize) throws org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.NotStrictlyPositiveException, org.apache.commons.math3.exception.NumberIsTooLargeException
		public virtual int nextHypergeometric(int populationSize, int numberOfSuccesses, int sampleSize)
		{
			return (new HypergeometricDistribution(RandomGenerator,populationSize, numberOfSuccesses, sampleSize)).sample();
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="PascalDistribution Pascal Distribution"/>.
		/// </summary>
		/// <param name="r"> the number of successes of the Pascal distribution </param>
		/// <param name="p"> the probability of success of the Pascal distribution </param>
		/// <returns> random value sampled from the Pascal(r, p) distribution </returns>
		/// <exception cref="NotStrictlyPositiveException"> if the number of successes is not positive </exception>
		/// <exception cref="OutOfRangeException"> if the probability of success is not in the
		/// range {@code [0, 1]}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextPascal(int r, double p) throws org.apache.commons.math3.exception.NotStrictlyPositiveException, org.apache.commons.math3.exception.OutOfRangeException
		public virtual int nextPascal(int r, double p)
		{
			return (new PascalDistribution(RandomGenerator, r, p)).sample();
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="TDistribution T Distribution"/>.
		/// </summary>
		/// <param name="df"> the degrees of freedom of the T distribution </param>
		/// <returns> random value from the T(df) distribution </returns>
		/// <exception cref="NotStrictlyPositiveException"> if {@code df <= 0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double nextT(double df) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public virtual double nextT(double df)
		{
			return (new TDistribution(RandomGenerator, df, TDistribution.DEFAULT_INVERSE_ABSOLUTE_ACCURACY)).sample();
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="WeibullDistribution Weibull Distribution"/>.
		/// </summary>
		/// <param name="shape"> the shape parameter of the Weibull distribution </param>
		/// <param name="scale"> the scale parameter of the Weibull distribution </param>
		/// <returns> random value sampled from the Weibull(shape, size) distribution </returns>
		/// <exception cref="NotStrictlyPositiveException"> if {@code shape <= 0} or
		/// {@code scale <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double nextWeibull(double shape, double scale) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public virtual double nextWeibull(double shape, double scale)
		{
			return (new WeibullDistribution(RandomGenerator, shape, scale, WeibullDistribution.DEFAULT_INVERSE_ABSOLUTE_ACCURACY)).sample();
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="ZipfDistribution Zipf Distribution"/>.
		/// </summary>
		/// <param name="numberOfElements"> the number of elements of the ZipfDistribution </param>
		/// <param name="exponent"> the exponent of the ZipfDistribution </param>
		/// <returns> random value sampled from the Zipf(numberOfElements, exponent) distribution </returns>
		/// <exception cref="NotStrictlyPositiveException"> if {@code numberOfElements <= 0}
		/// or {@code exponent <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextZipf(int numberOfElements, double exponent) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public virtual int nextZipf(int numberOfElements, double exponent)
		{
			return (new ZipfDistribution(RandomGenerator, numberOfElements, exponent)).sample();
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="BetaDistribution Beta Distribution"/>.
		/// </summary>
		/// <param name="alpha"> first distribution shape parameter </param>
		/// <param name="beta"> second distribution shape parameter </param>
		/// <returns> random value sampled from the beta(alpha, beta) distribution </returns>
		public virtual double nextBeta(double alpha, double beta)
		{
			return (new BetaDistribution(RandomGenerator, alpha, beta, BetaDistribution.DEFAULT_INVERSE_ABSOLUTE_ACCURACY)).sample();
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="BinomialDistribution Binomial Distribution"/>.
		/// </summary>
		/// <param name="numberOfTrials"> number of trials of the Binomial distribution </param>
		/// <param name="probabilityOfSuccess"> probability of success of the Binomial distribution </param>
		/// <returns> random value sampled from the Binomial(numberOfTrials, probabilityOfSuccess) distribution </returns>
		public virtual int nextBinomial(int numberOfTrials, double probabilityOfSuccess)
		{
			return (new BinomialDistribution(RandomGenerator, numberOfTrials, probabilityOfSuccess)).sample();
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="CauchyDistribution Cauchy Distribution"/>.
		/// </summary>
		/// <param name="median"> the median of the Cauchy distribution </param>
		/// <param name="scale"> the scale parameter of the Cauchy distribution </param>
		/// <returns> random value sampled from the Cauchy(median, scale) distribution </returns>
		public virtual double nextCauchy(double median, double scale)
		{
			return (new CauchyDistribution(RandomGenerator, median, scale, CauchyDistribution.DEFAULT_INVERSE_ABSOLUTE_ACCURACY)).sample();
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="ChiSquaredDistribution ChiSquare Distribution"/>.
		/// </summary>
		/// <param name="df"> the degrees of freedom of the ChiSquare distribution </param>
		/// <returns> random value sampled from the ChiSquare(df) distribution </returns>
		public virtual double nextChiSquare(double df)
		{
			return (new ChiSquaredDistribution(RandomGenerator, df, ChiSquaredDistribution.DEFAULT_INVERSE_ABSOLUTE_ACCURACY)).sample();
		}

		/// <summary>
		/// Generates a random value from the <seealso cref="FDistribution F Distribution"/>.
		/// </summary>
		/// <param name="numeratorDf"> the numerator degrees of freedom of the F distribution </param>
		/// <param name="denominatorDf"> the denominator degrees of freedom of the F distribution </param>
		/// <returns> random value sampled from the F(numeratorDf, denominatorDf) distribution </returns>
		/// <exception cref="NotStrictlyPositiveException"> if
		/// {@code numeratorDf <= 0} or {@code denominatorDf <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double nextF(double numeratorDf, double denominatorDf) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public virtual double nextF(double numeratorDf, double denominatorDf)
		{
			return (new FDistribution(RandomGenerator, numeratorDf, denominatorDf, FDistribution.DEFAULT_INVERSE_ABSOLUTE_ACCURACY)).sample();
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <p>
		/// <strong>Algorithm Description</strong>: scales the output of
		/// Random.nextDouble(), but rejects 0 values (i.e., will generate another
		/// random double if Random.nextDouble() returns 0). This is necessary to
		/// provide a symmetric output interval (both endpoints excluded).
		/// </p> </summary>
		/// <exception cref="NumberIsTooLargeException"> if {@code lower >= upper} </exception>
		/// <exception cref="NotFiniteNumberException"> if one of the bounds is infinite </exception>
		/// <exception cref="NotANumberException"> if one of the bounds is NaN </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double nextUniform(double lower, double upper) throws org.apache.commons.math3.exception.NumberIsTooLargeException, org.apache.commons.math3.exception.NotFiniteNumberException, org.apache.commons.math3.exception.NotANumberException
		public virtual double nextUniform(double lower, double upper)
		{
			return nextUniform(lower, upper, false);
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
		/// </summary>
		/// <exception cref="NumberIsTooLargeException"> if {@code lower >= upper} </exception>
		/// <exception cref="NotFiniteNumberException"> if one of the bounds is infinite </exception>
		/// <exception cref="NotANumberException"> if one of the bounds is NaN </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double nextUniform(double lower, double upper, boolean lowerInclusive) throws org.apache.commons.math3.exception.NumberIsTooLargeException, org.apache.commons.math3.exception.NotFiniteNumberException, org.apache.commons.math3.exception.NotANumberException
		public virtual double nextUniform(double lower, double upper, bool lowerInclusive)
		{

			if (lower >= upper)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.LOWER_BOUND_NOT_BELOW_UPPER_BOUND, lower, upper, false);
			}

			if (double.IsInfinity(lower))
			{
				throw new NotFiniteNumberException(LocalizedFormats.INFINITE_BOUND, lower);
			}
			if (double.IsInfinity(upper))
			{
				throw new NotFiniteNumberException(LocalizedFormats.INFINITE_BOUND, upper);
			}

			if (double.IsNaN(lower) || double.IsNaN(upper))
			{
				throw new NotANumberException();
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RandomGenerator generator = getRandomGenerator();
			RandomGenerator generator = RandomGenerator;

			// ensure nextDouble() isn't 0.0
			double u = generator.NextDouble();
			while (!lowerInclusive && u <= 0.0)
			{
				u = generator.NextDouble();
			}

			return u * upper + (1.0 - u) * lower;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// This method calls {@link MathArrays#shuffle(int[],RandomGenerator)
		/// MathArrays.shuffle} in order to create a random shuffle of the set
		/// of natural numbers {@code { 0, 1, ..., n - 1 }}.
		/// </summary>
		/// <exception cref="NumberIsTooLargeException"> if {@code k > n}. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if {@code k <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int[] nextPermutation(int n, int k) throws org.apache.commons.math3.exception.NumberIsTooLargeException, org.apache.commons.math3.exception.NotStrictlyPositiveException
		public virtual int[] nextPermutation(int n, int k)
		{
			if (k > n)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.PERMUTATION_EXCEEDS_N, k, n, true);
			}
			if (k <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.PERMUTATION_SIZE, k);
			}

			int[] index = MathArrays.natural(n);
			MathArrays.shuffle(index, RandomGenerator);

			// Return a new array containing the first "k" entries of "index".
			return MathArrays.copyOf(index, k);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// This method calls <seealso cref="#nextPermutation(int,int) nextPermutation(c.size(), k)"/>
		/// in order to sample the collection.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object[] nextSample(java.util.Collection<?> c, int k) throws org.apache.commons.math3.exception.NumberIsTooLargeException, org.apache.commons.math3.exception.NotStrictlyPositiveException
		public virtual object[] nextSample<T1>(ICollection<T1> c, int k)
		{

			int len = c.Count;
			if (k > len)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.SAMPLE_SIZE_EXCEEDS_COLLECTION_SIZE, k, len, true);
			}
			if (k <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.NUMBER_OF_SAMPLES, k);
			}

			object[] objects = c.ToArray();
			int[] index = nextPermutation(len, k);
			object[] result = new object[k];
			for (int i = 0; i < k; i++)
			{
				result[i] = objects[index[i]];
			}
			return result;
		}



		/// <summary>
		/// Reseeds the random number generator with the supplied seed.
		/// <p>
		/// Will create and initialize if null.
		/// </p>
		/// </summary>
		/// <param name="seed"> the seed value to use </param>
		public virtual void reSeed(long seed)
		{
		   RandomGenerator.Seed = seed;
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
			SecRan.Seed = System.currentTimeMillis();
		}

		/// <summary>
		/// Reseeds the secure random number generator with the supplied seed.
		/// <p>
		/// Will create and initialize if null.
		/// </p>
		/// </summary>
		/// <param name="seed"> the seed value to use </param>
		public virtual void reSeedSecure(long seed)
		{
			SecRan.Seed = seed;
		}

		/// <summary>
		/// Reseeds the random number generator with
		/// {@code System.currentTimeMillis() + System.identityHashCode(this))}.
		/// </summary>
		public virtual void reSeed()
		{
			RandomGenerator.Seed = System.currentTimeMillis() + System.identityHashCode(this);
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
		/// <param name="algorithm"> the name of the PRNG algorithm </param>
		/// <param name="provider"> the name of the provider </param>
		/// <exception cref="NoSuchAlgorithmException"> if the specified algorithm is not available </exception>
		/// <exception cref="NoSuchProviderException"> if the specified provider is not installed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSecureAlgorithm(String algorithm, String provider) throws java.security.NoSuchAlgorithmException, java.security.NoSuchProviderException
		public virtual void setSecureAlgorithm(string algorithm, string provider)
		{
			secRand = RandomGeneratorFactory.createRandomGenerator(SecureRandom.getInstance(algorithm, provider));
		}

		/// <summary>
		/// Returns the RandomGenerator used to generate non-secure random data.
		/// <p>
		/// Creates and initializes a default generator if null. Uses a <seealso cref="Well19937c"/>
		/// generator with {@code System.currentTimeMillis() + System.identityHashCode(this))}
		/// as the default seed.
		/// </p>
		/// </summary>
		/// <returns> the Random used to generate random data
		/// @since 3.2 </returns>
		public virtual RandomGenerator RandomGenerator
		{
			get
			{
				if (rand == null)
				{
					initRan();
				}
				return rand;
			}
		}

		/// <summary>
		/// Sets the default generator to a <seealso cref="Well19937c"/> generator seeded with
		/// {@code System.currentTimeMillis() + System.identityHashCode(this))}.
		/// </summary>
		private void initRan()
		{
			rand = new Well19937c(System.currentTimeMillis() + System.identityHashCode(this));
		}

		/// <summary>
		/// Returns the SecureRandom used to generate secure random data.
		/// <p>
		/// Creates and initializes if null.  Uses
		/// {@code System.currentTimeMillis() + System.identityHashCode(this)} as the default seed.
		/// </p>
		/// </summary>
		/// <returns> the SecureRandom used to generate secure random data, wrapped in a
		/// <seealso cref="RandomGenerator"/>. </returns>
		private RandomGenerator SecRan
		{
			get
			{
				if (secRand == null)
				{
					secRand = RandomGeneratorFactory.createRandomGenerator(new SecureRandom());
					secRand.Seed = System.currentTimeMillis() + System.identityHashCode(this);
				}
				return secRand;
			}
		}
	}

}
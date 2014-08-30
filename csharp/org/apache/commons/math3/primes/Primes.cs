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
namespace org.apache.commons.math3.primes
{

	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;


	/// <summary>
	/// Methods related to prime numbers in the range of <code>int</code>:
	/// <ul>
	/// <li>primality test</li>
	/// <li>prime number generation</li>
	/// <li>factorization</li>
	/// </ul>
	/// 
	/// @version $Id: Primes.java 1538368 2013-11-03 13:57:37Z erans $
	/// @since 3.2
	/// </summary>
	public class Primes
	{

		/// <summary>
		/// Hide utility class.
		/// </summary>
		private Primes()
		{
		}

		/// <summary>
		/// Primality test: tells if the argument is a (provable) prime or not.
		/// <p>
		/// It uses the Miller-Rabin probabilistic test in such a way that a result is guaranteed:
		/// it uses the firsts prime numbers as successive base (see Handbook of applied cryptography
		/// by Menezes, table 4.1).
		/// </summary>
		/// <param name="n"> number to test. </param>
		/// <returns> true if n is prime. (All numbers &lt; 2 return false). </returns>
		public static bool isPrime(int n)
		{
			if (n < 2)
			{
				return false;
			}

			foreach (int p in SmallPrimes.PRIMES)
			{
				if (0 == (n % p))
				{
					return n == p;
				}
			}
			return SmallPrimes.millerRabinPrimeTest(n);
		}

		/// <summary>
		/// Return the smallest prime greater than or equal to n.
		/// </summary>
		/// <param name="n"> a positive number. </param>
		/// <returns> the smallest prime greater than or equal to n. </returns>
		/// <exception cref="MathIllegalArgumentException"> if n &lt; 0. </exception>
		public static int nextPrime(int n)
		{
			if (n < 0)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.NUMBER_TOO_SMALL, n, 0);
			}
			if (n == 2)
			{
				return 2;
			}
			n |= 1; //make sure n is odd
			if (n == 1)
			{
				return 2;
			}

			if (isPrime(n))
			{
				return n;
			}

			// prepare entry in the +2, +4 loop:
			// n should not be a multiple of 3
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rem = n % 3;
			int rem = n % 3;
			if (0 == rem) // if n % 3 == 0
			{
				n += 2; // n % 3 == 2
			} // if n % 3 == 1
			else if (1 == rem)
			{
				// if (isPrime(n)) return n;
				n += 4; // n % 3 == 2
			}
			while (true) // this loop skips all multiple of 3
			{
				if (isPrime(n))
				{
					return n;
				}
				n += 2; // n % 3 == 1
				if (isPrime(n))
				{
					return n;
				}
				n += 4; // n % 3 == 2
			}
		}

		/// <summary>
		/// Prime factors decomposition
		/// </summary>
		/// <param name="n"> number to factorize: must be &ge; 2 </param>
		/// <returns> list of prime factors of n </returns>
		/// <exception cref="MathIllegalArgumentException"> if n &lt; 2. </exception>
		public static IList<int?> primeFactors(int n)
		{

			if (n < 2)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.NUMBER_TOO_SMALL, n, 2);
			}
			// slower than trial div unless we do an awful lot of computation
			// (then it finally gets JIT-compiled efficiently
			// List<Integer> out = PollardRho.primeFactors(n);
			return SmallPrimes.trialDivision(n);

		}

	}

}
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


	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Implementation of the Pollard's rho factorization algorithm.
	/// @version $Id: PollardRho.java 1538368 2013-11-03 13:57:37Z erans $
	/// @since 3.2
	/// </summary>
	internal class PollardRho
	{

		/// <summary>
		/// Hide utility class.
		/// </summary>
		private PollardRho()
		{
		}

		/// <summary>
		/// Factorization using Pollard's rho algorithm. </summary>
		/// <param name="n"> number to factors, must be &gt; 0 </param>
		/// <returns> the list of prime factors of n. </returns>
		public static IList<int?> primeFactors(int n)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Integer> factors = new java.util.ArrayList<Integer>();
			IList<int?> factors = new List<int?>();

			n = SmallPrimes.smallTrialDivision(n, factors);
			if (1 == n)
			{
				return factors;
			}

			if (SmallPrimes.millerRabinPrimeTest(n))
			{
				factors.Add(n);
				return factors;
			}

			int divisor = rhoBrent(n);
			factors.Add(divisor);
			factors.Add(n / divisor);
			return factors;
		}

		/// <summary>
		/// Implementation of the Pollard's rho factorization algorithm.
		/// <p>
		/// This implementation follows the paper "An improved Monte Carlo factorization algorithm"
		/// by Richard P. Brent. This avoids the triple computation of f(x) typically found in Pollard's
		/// rho implementations. It also batches several gcd computation into 1.
		/// <p>
		/// The backtracking is not implemented as we deal only with semi-primes.
		/// </summary>
		/// <param name="n"> number to factor, must be semi-prime. </param>
		/// <returns> a prime factor of n. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: static int rhoBrent(final int n)
		internal static int rhoBrent(int n)
		{
			const int x0 = 2;
			const int m = 25;
			int cst = SmallPrimes.PRIMES_LAST;
			int y = x0;
			int r = 1;
			do
			{
				int x = y;
				for (int i = 0; i < r; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long y2 = ((long) y) * y;
					long y2 = ((long) y) * y;
					y = (int)((y2 + cst) % n);
				}
				int k = 0;
				do
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int bound = org.apache.commons.math3.util.FastMath.min(m, r - k);
					int bound = FastMath.min(m, r - k);
					int q = 1;
					for (int i = -3; i < bound; i++) //start at -3 to ensure we enter this loop at least 3 times
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long y2 = ((long) y) * y;
						long y2 = ((long) y) * y;
						y = (int)((y2 + cst) % n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long divisor = org.apache.commons.math3.util.FastMath.abs(x - y);
						long divisor = FastMath.abs(x - y);
						if (0 == divisor)
						{
							cst += SmallPrimes.PRIMES_LAST;
							k = -m;
							y = x0;
							r = 1;
							break;
						}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long prod = divisor * q;
						long prod = divisor * q;
						q = (int)(prod % n);
						if (0 == q)
						{
							return gcdPositive(FastMath.abs((int) divisor), n);
						}
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int out = gcdPositive(org.apache.commons.math3.util.FastMath.abs(q), n);
					int @out = gcdPositive(FastMath.abs(q), n);
					if (1 != @out)
					{
						return @out;
					}
					k += m;
				} while (k < r);
				r = 2 * r;
			} while (true);
		}

		/// <summary>
		/// Gcd between two positive numbers.
		/// <p>
		/// Gets the greatest common divisor of two numbers, using the "binary gcd" method,
		/// which avoids division and modulo operations. See Knuth 4.5.2 algorithm B.
		/// This algorithm is due to Josef Stein (1961).
		/// </p>
		/// Special cases:
		/// <ul>
		/// <li>The result of {@code gcd(x, x)}, {@code gcd(0, x)} and {@code gcd(x, 0)} is the value of {@code x}.</li>
		/// <li>The invocation {@code gcd(0, 0)} is the only one which returns {@code 0}.</li>
		/// </ul>
		/// </summary>
		/// <param name="a"> first number, must be &ge; 0 </param>
		/// <param name="b"> second number, must be &ge; 0 </param>
		/// <returns> gcd(a,b) </returns>
		internal static int gcdPositive(int a, int b)
		{
			// both a and b must be positive, it is not checked here
			// gdc(a,0) = a
			if (a == 0)
			{
				return b;
			}
			else if (b == 0)
			{
				return a;
			}

			// make a and b odd, keep in mind the common power of twos
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int aTwos = Integer.numberOfTrailingZeros(a);
			int aTwos = int.numberOfTrailingZeros(a);
			a >>= aTwos;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int bTwos = Integer.numberOfTrailingZeros(b);
			int bTwos = int.numberOfTrailingZeros(b);
			b >>= bTwos;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int shift = org.apache.commons.math3.util.FastMath.min(aTwos, bTwos);
			int shift = FastMath.min(aTwos, bTwos);

			// a and b >0
			// if a > b then gdc(a,b) = gcd(a-b,b)
			// if a < b then gcd(a,b) = gcd(b-a,a)
			// so next a is the absolute difference and next b is the minimum of current values
			while (a != b)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int delta = a - b;
				int delta = a - b;
				b = FastMath.min(a, b);
				a = FastMath.abs(delta);
				// for speed optimization:
				// remove any power of two in a as b is guaranteed to be odd throughout all iterations
				a >>= int.numberOfTrailingZeros(a);
			}

			// gcd(a,a) = a, just "add" the common power of twos
			return a << shift;
		}
	}

}
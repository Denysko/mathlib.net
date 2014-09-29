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

	using NullArgumentException = mathlib.exception.NullArgumentException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// <p>This class provides a stable normalized random generator. It samples from a stable
	/// distribution with location parameter 0 and scale 1.</p>
	/// 
	/// <p>The implementation uses the Chambers-Mallows-Stuck method as described in
	/// <i>Handbook of computational statistics: concepts and methods</i> by
	/// James E. Gentle, Wolfgang H&auml;rdle, Yuichi Mori.</p>
	/// 
	/// @version $Id: StableRandomGenerator.java 1538368 2013-11-03 13:57:37Z erans $
	/// @since 3.0
	/// </summary>
	public class StableRandomGenerator : NormalizedRandomGenerator
	{
		/// <summary>
		/// Underlying generator. </summary>
		private readonly RandomGenerator generator;

		/// <summary>
		/// stability parameter </summary>
		private readonly double alpha;

		/// <summary>
		/// skewness parameter </summary>
		private readonly double beta;

		/// <summary>
		/// cache of expression value used in generation </summary>
		private readonly double zeta;

		/// <summary>
		/// Create a new generator.
		/// </summary>
		/// <param name="generator"> underlying random generator to use </param>
		/// <param name="alpha"> Stability parameter. Must be in range (0, 2] </param>
		/// <param name="beta"> Skewness parameter. Must be in range [-1, 1] </param>
		/// <exception cref="NullArgumentException"> if generator is null </exception>
		/// <exception cref="OutOfRangeException"> if {@code alpha <= 0} or {@code alpha > 2}
		/// or {@code beta < -1} or {@code beta > 1} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public StableRandomGenerator(final RandomGenerator generator, final double alpha, final double beta) throws mathlib.exception.NullArgumentException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public StableRandomGenerator(RandomGenerator generator, double alpha, double beta)
		{
			if (generator == null)
			{
				throw new NullArgumentException();
			}

			if (!(alpha > 0d && alpha <= 2d))
			{
				throw new OutOfRangeException(LocalizedFormats.OUT_OF_RANGE_LEFT, alpha, 0, 2);
			}

			if (!(beta >= -1d && beta <= 1d))
			{
				throw new OutOfRangeException(LocalizedFormats.OUT_OF_RANGE_SIMPLE, beta, -1, 1);
			}

			this.generator = generator;
			this.alpha = alpha;
			this.beta = beta;
			if (alpha < 2d && beta != 0d)
			{
				zeta = beta * FastMath.tan(FastMath.PI * alpha / 2);
			}
			else
			{
				zeta = 0d;
			}
		}

		/// <summary>
		/// Generate a random scalar with zero location and unit scale.
		/// </summary>
		/// <returns> a random scalar with zero location and unit scale </returns>
		public virtual double nextNormalizedDouble()
		{
			// we need 2 uniform random numbers to calculate omega and phi
			double omega = -FastMath.log(generator.NextDouble());
			double phi = FastMath.PI * (generator.NextDouble() - 0.5);

			// Normal distribution case (Box-Muller algorithm)
			if (alpha == 2d)
			{
				return FastMath.sqrt(2d * omega) * FastMath.sin(phi);
			}

			double x;
			// when beta = 0, zeta is zero as well
			// Thus we can exclude it from the formula
			if (beta == 0d)
			{
				// Cauchy distribution case
				if (alpha == 1d)
				{
					x = FastMath.tan(phi);
				}
				else
				{
					x = FastMath.pow(omega * FastMath.cos((1 - alpha) * phi), 1d / alpha - 1d) * FastMath.sin(alpha * phi) / FastMath.pow(FastMath.cos(phi), 1d / alpha);
				}
			}
			else
			{
				// Generic stable distribution
				double cosPhi = FastMath.cos(phi);
				// to avoid rounding errors around alpha = 1
				if (FastMath.abs(alpha - 1d) > 1e-8)
				{
					double alphaPhi = alpha * phi;
					double invAlphaPhi = phi - alphaPhi;
					x = (FastMath.sin(alphaPhi) + zeta * FastMath.cos(alphaPhi)) / cosPhi * (FastMath.cos(invAlphaPhi) + zeta * FastMath.sin(invAlphaPhi)) / FastMath.pow(omega * cosPhi, (1 - alpha) / alpha);
				}
				else
				{
					double betaPhi = FastMath.PI / 2 + beta * phi;
					x = 2d / FastMath.PI * (betaPhi * FastMath.tan(phi) - beta * FastMath.log(FastMath.PI / 2d * omega * cosPhi / betaPhi));

					if (alpha != 1d)
					{
						x += beta * FastMath.tan(FastMath.PI * alpha / 2);
					}
				}
			}
			return x;
		}
	}

}
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

	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// This class implements a normalized uniform random generator.
	/// <p>Since it is a normalized random generator, it generates values
	/// from a uniform distribution with mean equal to 0 and standard
	/// deviation equal to 1. Generated values fall in the range
	/// [-&#x0221A;3, +&#x0221A;3].</p>
	/// 
	/// @since 1.2
	/// 
	/// @version $Id: UniformRandomGenerator.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>

	public class UniformRandomGenerator : NormalizedRandomGenerator
	{

		/// <summary>
		/// Square root of three. </summary>
		private static readonly double SQRT3 = FastMath.sqrt(3.0);

		/// <summary>
		/// Underlying generator. </summary>
		private readonly RandomGenerator generator;

		/// <summary>
		/// Create a new generator. </summary>
		/// <param name="generator"> underlying random generator to use </param>
		public UniformRandomGenerator(RandomGenerator generator)
		{
			this.generator = generator;
		}

		/// <summary>
		/// Generate a random scalar with null mean and unit standard deviation.
		/// <p>The number generated is uniformly distributed between -&sqrt;(3)
		/// and +&sqrt;(3).</p> </summary>
		/// <returns> a random scalar with null mean and unit standard deviation </returns>
		public virtual double nextNormalizedDouble()
		{
			return SQRT3 * (2 * generator.NextDouble() - 1.0);
		}

	}

}
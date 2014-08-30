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

	/// <summary>
	/// This class is a gaussian normalized random generator for scalars.
	/// <p>This class is a simple wrapper around the {@link
	/// RandomGenerator#nextGaussian} method.</p>
	/// @version $Id: GaussianRandomGenerator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2
	/// </summary>

	public class GaussianRandomGenerator : NormalizedRandomGenerator
	{

		/// <summary>
		/// Underlying generator. </summary>
		private readonly RandomGenerator generator;

		/// <summary>
		/// Create a new generator. </summary>
		/// <param name="generator"> underlying random generator to use </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public GaussianRandomGenerator(final RandomGenerator generator)
		public GaussianRandomGenerator(RandomGenerator generator)
		{
			this.generator = generator;
		}

		/// <summary>
		/// Generate a random scalar with null mean and unit standard deviation. </summary>
		/// <returns> a random scalar with null mean and unit standard deviation </returns>
		public virtual double nextNormalizedDouble()
		{
			return generator.nextGaussian();
		}

	}

}
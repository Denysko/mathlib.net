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
	/// Generate random vectors isotropically located on the surface of a sphere.
	/// 
	/// @since 2.1
	/// @version $Id: UnitSphereRandomVectorGenerator.java 1444500 2013-02-10 08:10:40Z tn $
	/// </summary>

	public class UnitSphereRandomVectorGenerator : RandomVectorGenerator
	{
		/// <summary>
		/// RNG used for generating the individual components of the vectors.
		/// </summary>
		private readonly RandomGenerator rand;
		/// <summary>
		/// Space dimension.
		/// </summary>
		private readonly int dimension;

		/// <param name="dimension"> Space dimension. </param>
		/// <param name="rand"> RNG for the individual components of the vectors. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public UnitSphereRandomVectorGenerator(final int dimension, final RandomGenerator rand)
		public UnitSphereRandomVectorGenerator(int dimension, RandomGenerator rand)
		{
			this.dimension = dimension;
			this.rand = rand;
		}
		/// <summary>
		/// Create an object that will use a default RNG (<seealso cref="MersenneTwister"/>),
		/// in order to generate the individual components.
		/// </summary>
		/// <param name="dimension"> Space dimension. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public UnitSphereRandomVectorGenerator(final int dimension)
		public UnitSphereRandomVectorGenerator(int dimension) : this(dimension, new MersenneTwister())
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double[] nextVector()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] v = new double[dimension];
			double[] v = new double[dimension];

			// See http://mathworld.wolfram.com/SpherePointPicking.html for example.
			// Pick a point by choosing a standard Gaussian for each element, and then
			// normalizing to unit length.
			double normSq = 0;
			for (int i = 0; i < dimension; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double comp = rand.nextGaussian();
				double comp = rand.nextGaussian();
				v[i] = comp;
				normSq += comp * comp;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double f = 1 / org.apache.commons.math3.util.FastMath.sqrt(normSq);
			double f = 1 / FastMath.sqrt(normSq);
			for (int i = 0; i < dimension; i++)
			{
				v[i] *= f;
			}

			return v;
		}
	}

}
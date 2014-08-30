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

namespace org.apache.commons.math3.ml.neuralnet
{

	using RealDistribution = org.apache.commons.math3.distribution.RealDistribution;
	using UniformRealDistribution = org.apache.commons.math3.distribution.UniformRealDistribution;
	using UnivariateFunction = org.apache.commons.math3.analysis.UnivariateFunction;
	using Constant = org.apache.commons.math3.analysis.function.Constant;
	using RandomGenerator = org.apache.commons.math3.random.RandomGenerator;

	/// <summary>
	/// Creates functions that will select the initial values of a neuron's
	/// features.
	/// 
	/// @version $Id: FeatureInitializerFactory.java 1566092 2014-02-08 18:48:29Z tn $
	/// @since 3.3
	/// </summary>
	public class FeatureInitializerFactory
	{
		/// <summary>
		/// Class contains only static methods. </summary>
		private FeatureInitializerFactory()
		{
		}

		/// <summary>
		/// Uniform sampling of the given range.
		/// </summary>
		/// <param name="min"> Lower bound of the range. </param>
		/// <param name="max"> Upper bound of the range. </param>
		/// <param name="rng"> Random number generator used to draw samples from a
		/// uniform distribution. </param>
		/// <returns> an initializer such that the features will be initialized with
		/// values within the given range. </returns>
		/// <exception cref="org.apache.commons.math3.exception.NumberIsTooLargeException">
		/// if {@code min >= max}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static FeatureInitializer uniform(final org.apache.commons.math3.random.RandomGenerator rng, final double min, final double max)
		public static FeatureInitializer uniform(RandomGenerator rng, double min, double max)
		{
			return randomize(new UniformRealDistribution(rng, min, max), function(new Constant(0), 0, 0));
		}

		/// <summary>
		/// Uniform sampling of the given range.
		/// </summary>
		/// <param name="min"> Lower bound of the range. </param>
		/// <param name="max"> Upper bound of the range. </param>
		/// <returns> an initializer such that the features will be initialized with
		/// values within the given range. </returns>
		/// <exception cref="org.apache.commons.math3.exception.NumberIsTooLargeException">
		/// if {@code min >= max}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static FeatureInitializer uniform(final double min, final double max)
		public static FeatureInitializer uniform(double min, double max)
		{
			return randomize(new UniformRealDistribution(min, max), function(new Constant(0), 0, 0));
		}

		/// <summary>
		/// Creates an initializer from a univariate function {@code f(x)}.
		/// The argument {@code x} is set to {@code init} at the first call
		/// and will be incremented at each call.
		/// </summary>
		/// <param name="f"> Function. </param>
		/// <param name="init"> Initial value. </param>
		/// <param name="inc"> Increment </param>
		/// <returns> the initializer. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static FeatureInitializer function(final org.apache.commons.math3.analysis.UnivariateFunction f, final double init, final double inc)
		public static FeatureInitializer function(UnivariateFunction f, double init, double inc)
		{
			return new FeatureInitializerAnonymousInnerClassHelper(f, init, inc);
		}

		private class FeatureInitializerAnonymousInnerClassHelper : FeatureInitializer
		{
			private UnivariateFunction f;
			private double init;
			private double inc;

			public FeatureInitializerAnonymousInnerClassHelper(UnivariateFunction f, double init, double inc)
			{
				this.f = f;
				this.init = init;
				this.inc = inc;
			}

					/// <summary>
					/// Argument. </summary>
			private double arg = init;

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual double value()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double result = f.value(arg);
				double result = f.value(arg);
				arg += inc;
				return result;
			}
		}

		/// <summary>
		/// Adds some amount of random data to the given initializer.
		/// </summary>
		/// <param name="random"> Random variable distribution. </param>
		/// <param name="orig"> Original initializer. </param>
		/// <returns> an initializer whose <seealso cref="FeatureInitializer#value() value"/>
		/// method will return {@code orig.value() + random.sample()}. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static FeatureInitializer randomize(final org.apache.commons.math3.distribution.RealDistribution random, final FeatureInitializer orig)
		public static FeatureInitializer randomize(RealDistribution random, FeatureInitializer orig)
		{
			return new FeatureInitializerAnonymousInnerClassHelper2(random, orig);
		}

		private class FeatureInitializerAnonymousInnerClassHelper2 : FeatureInitializer
		{
			private RealDistribution random;
			private org.apache.commons.math3.ml.neuralnet.FeatureInitializer orig;

			public FeatureInitializerAnonymousInnerClassHelper2(RealDistribution random, org.apache.commons.math3.ml.neuralnet.FeatureInitializer orig)
			{
				this.random = random;
				this.orig = orig;
			}

					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual double value()
			{
				return orig.value() + random.sample();
			}
		}
	}

}
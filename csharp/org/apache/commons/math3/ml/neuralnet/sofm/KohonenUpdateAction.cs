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

namespace org.apache.commons.math3.ml.neuralnet.sofm
{

	using DistanceMeasure = org.apache.commons.math3.ml.distance.DistanceMeasure;
	using ArrayRealVector = org.apache.commons.math3.linear.ArrayRealVector;
	using Gaussian = org.apache.commons.math3.analysis.function.Gaussian;

	/// <summary>
	/// Update formula for <a href="http://en.wikipedia.org/wiki/Kohonen">
	/// Kohonen's Self-Organizing Map</a>.
	/// <br/>
	/// The <seealso cref="#update(Network,double[]) update"/> method modifies the
	/// features {@code w} of the "winning" neuron and its neighbours
	/// according to the following rule:
	/// <code>
	///  w<sub>new</sub> = w<sub>old</sub> + &alpha; e<sup>(-d / &sigma;)</sup> * (sample - w<sub>old</sub>)
	/// </code>
	/// where
	/// <ul>
	///  <li>&alpha; is the current <em>learning rate</em>, </li>
	///  <li>&sigma; is the current <em>neighbourhood size</em>, and</li>
	///  <li>{@code d} is the number of links to traverse in order to reach
	///   the neuron from the winning neuron.</li>
	/// </ul>
	/// <br/>
	/// This class is thread-safe as long as the arguments passed to the
	/// {@link #KohonenUpdateAction(DistanceMeasure,LearningFactorFunction,
	/// NeighbourhoodSizeFunction) constructor} are instances of thread-safe
	/// classes.
	/// <br/>
	/// Each call to the <seealso cref="#update(Network,double[]) update"/> method
	/// will increment the internal counter used to compute the current
	/// values for
	/// <ul>
	///  <li>the <em>learning rate</em>, and</li>
	///  <li>the <em>neighbourhood size</em>.</li>
	/// </ul>
	/// Consequently, the function instances that compute those values (passed
	/// to the constructor of this class) must take into account whether this
	/// class's instance will be shared by multiple threads, as this will impact
	/// the training process.
	/// 
	/// @version $Id: KohonenUpdateAction.java 1566092 2014-02-08 18:48:29Z tn $
	/// @since 3.3
	/// </summary>
	public class KohonenUpdateAction : UpdateAction
	{
		/// <summary>
		/// Distance function. </summary>
		private readonly DistanceMeasure distance;
		/// <summary>
		/// Learning factor update function. </summary>
		private readonly LearningFactorFunction learningFactor;
		/// <summary>
		/// Neighbourhood size update function. </summary>
		private readonly NeighbourhoodSizeFunction neighbourhoodSize;
		/// <summary>
		/// Number of calls to <seealso cref="#update(Network,double[])"/>. </summary>
		private readonly AtomicLong numberOfCalls = new AtomicLong(-1);

		/// <param name="distance"> Distance function. </param>
		/// <param name="learningFactor"> Learning factor update function. </param>
		/// <param name="neighbourhoodSize"> Neighbourhood size update function. </param>
		public KohonenUpdateAction(DistanceMeasure distance, LearningFactorFunction learningFactor, NeighbourhoodSizeFunction neighbourhoodSize)
		{
			this.distance = distance;
			this.learningFactor = learningFactor;
			this.neighbourhoodSize = neighbourhoodSize;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual void update(Network net, double[] features)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long numCalls = numberOfCalls.incrementAndGet();
			long numCalls = numberOfCalls.incrementAndGet();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double currentLearning = learningFactor.value(numCalls);
			double currentLearning = learningFactor.value(numCalls);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.ml.neuralnet.Neuron best = findAndUpdateBestNeuron(net, features, currentLearning);
			Neuron best = findAndUpdateBestNeuron(net, features, currentLearning);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int currentNeighbourhood = neighbourhoodSize.value(numCalls);
			int currentNeighbourhood = neighbourhoodSize.value(numCalls);
			// The farther away the neighbour is from the winning neuron, the
			// smaller the learning rate will become.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.function.Gaussian neighbourhoodDecay = new org.apache.commons.math3.analysis.function.Gaussian(currentLearning, 0, 1d / currentNeighbourhood);
			Gaussian neighbourhoodDecay = new Gaussian(currentLearning, 0, 1d / currentNeighbourhood);

			if (currentNeighbourhood > 0)
			{
				// Initial set of neurons only contains the winning neuron.
				ICollection<Neuron> neighbours = new HashSet<Neuron>();
				neighbours.Add(best);
				// Winning neuron must be excluded from the neighbours.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.HashSet<org.apache.commons.math3.ml.neuralnet.Neuron> exclude = new java.util.HashSet<org.apache.commons.math3.ml.neuralnet.Neuron>();
				HashSet<Neuron> exclude = new HashSet<Neuron>();
				exclude.Add(best);

				int radius = 1;
				do
				{
					// Retrieve immediate neighbours of the current set of neurons.
					neighbours = net.getNeighbours(neighbours, exclude);

					// Update all the neighbours.
					foreach (Neuron n in neighbours)
					{
						updateNeighbouringNeuron(n, features, neighbourhoodDecay.value(radius));
					}

					// Add the neighbours to the exclude list so that they will
					// not be update more than once per training step.
					exclude.addAll(neighbours);
					++radius;
				} while (radius <= currentNeighbourhood);
			}
		}

		/// <summary>
		/// Retrieves the number of calls to the <seealso cref="#update(Network,double[]) update"/>
		/// method.
		/// </summary>
		/// <returns> the current number of calls. </returns>
		public virtual long NumberOfCalls
		{
			get
			{
				return numberOfCalls.get();
			}
		}

		/// <summary>
		/// Atomically updates the given neuron.
		/// </summary>
		/// <param name="n"> Neuron to be updated. </param>
		/// <param name="features"> Training data. </param>
		/// <param name="learningRate"> Learning factor. </param>
		private void updateNeighbouringNeuron(Neuron n, double[] features, double learningRate)
		{
			while (true)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] expect = n.getFeatures();
				double[] expect = n.Features;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] update = computeFeatures(expect, features, learningRate);
				double[] update = computeFeatures(expect, features, learningRate);
				if (n.compareAndSetFeatures(expect, update))
				{
					break;
				}
			}
		}

		/// <summary>
		/// Searches for the neuron whose features are closest to the given
		/// sample, and atomically updates its features.
		/// </summary>
		/// <param name="net"> Network. </param>
		/// <param name="features"> Sample data. </param>
		/// <param name="learningRate"> Current learning factor. </param>
		/// <returns> the winning neuron. </returns>
		private Neuron findAndUpdateBestNeuron(Network net, double[] features, double learningRate)
		{
			while (true)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.ml.neuralnet.Neuron best = org.apache.commons.math3.ml.neuralnet.MapUtils.findBest(features, net, distance);
				Neuron best = MapUtils.findBest(features, net, distance);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] expect = best.getFeatures();
				double[] expect = best.Features;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] update = computeFeatures(expect, features, learningRate);
				double[] update = computeFeatures(expect, features, learningRate);
				if (best.compareAndSetFeatures(expect, update))
				{
					return best;
				}

				// If another thread modified the state of the winning neuron,
				// it may not be the best match anymore for the given training
				// sample: Hence, the winner search is performed again.
			}
		}

		/// <summary>
		/// Computes the new value of the features set.
		/// </summary>
		/// <param name="current"> Current values of the features. </param>
		/// <param name="sample"> Training data. </param>
		/// <param name="learningRate"> Learning factor. </param>
		/// <returns> the new values for the features. </returns>
		private double[] computeFeatures(double[] current, double[] sample, double learningRate)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector c = new org.apache.commons.math3.linear.ArrayRealVector(current, false);
			ArrayRealVector c = new ArrayRealVector(current, false);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector s = new org.apache.commons.math3.linear.ArrayRealVector(sample, false);
			ArrayRealVector s = new ArrayRealVector(sample, false);
			// c + learningRate * (s - c)
			return s.subtract(c).mapMultiplyToSelf(learningRate).add(c).toArray();
		}
	}

}
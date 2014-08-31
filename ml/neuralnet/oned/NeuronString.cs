using System;

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

namespace org.apache.commons.math3.ml.neuralnet.oned
{

	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;

	/// <summary>
	/// Neural network with the topology of a one-dimensional line.
	/// Each neuron defines one point on the line.
	/// 
	/// @version $Id: NeuronString.java 1566396 2014-02-09 20:36:24Z tn $
	/// @since 3.3
	/// </summary>
	[Serializable]
	public class NeuronString
	{
		/// <summary>
		/// Underlying network. </summary>
		private readonly Network network;
		/// <summary>
		/// Number of neurons. </summary>
		private readonly int size;
		/// <summary>
		/// Wrap. </summary>
		private readonly bool wrap;

		/// <summary>
		/// Mapping of the 1D coordinate to the neuron identifiers
		/// (attributed by the <seealso cref="#network"/> instance).
		/// </summary>
		private readonly long[] identifiers;

		/// <summary>
		/// Constructor with restricted access, solely used for deserialization.
		/// </summary>
		/// <param name="wrap"> Whether to wrap the dimension (i.e the first and last
		/// neurons will be linked together). </param>
		/// <param name="featuresList"> Arrays that will initialize the features sets of
		/// the network's neurons. </param>
		/// <exception cref="NumberIsTooSmallException"> if {@code num < 2}. </exception>
		internal NeuronString(bool wrap, double[][] featuresList)
		{
			size = featuresList.Length;

			if (size < 2)
			{
				throw new NumberIsTooSmallException(size, 2, true);
			}

			this.wrap = wrap;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int fLen = featuresList[0].length;
			int fLen = featuresList[0].Length;
			network = new Network(0, fLen);
			identifiers = new long[size];

			// Add neurons.
			for (int i = 0; i < size; i++)
			{
				identifiers[i] = network.createNeuron(featuresList[i]);
			}

			// Add links.
			createLinks();
		}

		/// <summary>
		/// Creates a one-dimensional network:
		/// Each neuron not located on the border of the mesh has two
		/// neurons linked to it.
		/// <br/>
		/// The links are bi-directional.
		/// Neurons created successively are neighbours (i.e. there are
		/// links between them).
		/// <br/>
		/// The topology of the network can also be a circle (if the
		/// dimension is wrapped).
		/// </summary>
		/// <param name="num"> Number of neurons. </param>
		/// <param name="wrap"> Whether to wrap the dimension (i.e the first and last
		/// neurons will be linked together). </param>
		/// <param name="featureInit"> Arrays that will initialize the features sets of
		/// the network's neurons. </param>
		/// <exception cref="NumberIsTooSmallException"> if {@code num < 2}. </exception>
		public NeuronString(int num, bool wrap, FeatureInitializer[] featureInit)
		{
			if (num < 2)
			{
				throw new NumberIsTooSmallException(num, 2, true);
			}

			size = num;
			this.wrap = wrap;
			identifiers = new long[num];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int fLen = featureInit.length;
			int fLen = featureInit.Length;
			network = new Network(0, fLen);

			// Add neurons.
			for (int i = 0; i < num; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] features = new double[fLen];
				double[] features = new double[fLen];
				for (int fIndex = 0; fIndex < fLen; fIndex++)
				{
					features[fIndex] = featureInit[fIndex].value();
				}
				identifiers[i] = network.createNeuron(features);
			}

			// Add links.
			createLinks();
		}

		/// <summary>
		/// Retrieves the underlying network.
		/// A reference is returned (enabling, for example, the network to be
		/// trained).
		/// This also implies that calling methods that modify the <seealso cref="Network"/>
		/// topology may cause this class to become inconsistent.
		/// </summary>
		/// <returns> the network. </returns>
		public virtual Network Network
		{
			get
			{
				return network;
			}
		}

		/// <summary>
		/// Gets the number of neurons.
		/// </summary>
		/// <returns> the number of neurons. </returns>
		public virtual int Size
		{
			get
			{
				return size;
			}
		}

		/// <summary>
		/// Retrieves the features set from the neuron at location
		/// {@code i} in the map.
		/// </summary>
		/// <param name="i"> Neuron index. </param>
		/// <returns> the features of the neuron at index {@code i}. </returns>
		/// <exception cref="OutOfRangeException"> if {@code i} is out of range. </exception>
		public virtual double[] getFeatures(int i)
		{
			if (i < 0 || i >= size)
			{
				throw new OutOfRangeException(i, 0, size - 1);
			}

			return network.getNeuron(identifiers[i]).Features;
		}

		/// <summary>
		/// Creates the neighbour relationships between neurons.
		/// </summary>
		private void createLinks()
		{
			for (int i = 0; i < size - 1; i++)
			{
				network.addLink(network.getNeuron(i), network.getNeuron(i + 1));
			}
			for (int i = size - 1; i > 0; i--)
			{
				network.addLink(network.getNeuron(i), network.getNeuron(i - 1));
			}
			if (wrap)
			{
				network.addLink(network.getNeuron(0), network.getNeuron(size - 1));
				network.addLink(network.getNeuron(size - 1), network.getNeuron(0));
			}
		}

		/// <summary>
		/// Prevents proxy bypass.
		/// </summary>
		/// <param name="in"> Input stream. </param>
		private void readObject(ObjectInputStream @in)
		{
			throw new IllegalStateException();
		}

		/// <summary>
		/// Custom serialization.
		/// </summary>
		/// <returns> the proxy instance that will be actually serialized. </returns>
		private object writeReplace()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] featuresList = new double[size][];
			double[][] featuresList = new double[size][];
			for (int i = 0; i < size; i++)
			{
				featuresList[i] = getFeatures(i);
			}

			return new SerializationProxy(wrap, featuresList);
		}

		/// <summary>
		/// Serialization.
		/// </summary>
		[Serializable]
		private class SerializationProxy
		{
			/// <summary>
			/// Serializable. </summary>
			internal const long serialVersionUID = 20130226L;
			/// <summary>
			/// Wrap. </summary>
			internal readonly bool wrap;
			/// <summary>
			/// Neurons' features. </summary>
			internal readonly double[][] featuresList;

			/// <param name="wrap"> Whether the dimension is wrapped. </param>
			/// <param name="featuresList"> List of neurons features.
			/// {@code neuronList}. </param>
			internal SerializationProxy(bool wrap, double[][] featuresList)
			{
				this.wrap = wrap;
				this.featuresList = featuresList;
			}

			/// <summary>
			/// Custom serialization.
			/// </summary>
			/// <returns> the <seealso cref="Neuron"/> for which this instance is the proxy. </returns>
			internal virtual object readResolve()
			{
				return new NeuronString(wrap, featuresList);
			}
		}
	}

}
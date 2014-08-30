using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

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

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MathIllegalStateException = org.apache.commons.math3.exception.MathIllegalStateException;

	/// <summary>
	/// Neural network, composed of <seealso cref="Neuron"/> instances and the links
	/// between them.
	/// 
	/// Although updating a neuron's state is thread-safe, modifying the
	/// network's topology (adding or removing links) is not.
	/// 
	/// @version $Id: Network.java 1566092 2014-02-08 18:48:29Z tn $
	/// @since 3.3
	/// </summary>
	[Serializable]
	public class Network : IEnumerable<Neuron>
	{
		/// <summary>
		/// Serializable. </summary>
		private const long serialVersionUID = 20130207L;
		/// <summary>
		/// Neurons. </summary>
		private readonly ConcurrentDictionary<long?, Neuron> neuronMap = new ConcurrentDictionary<long?, Neuron>();
		/// <summary>
		/// Next available neuron identifier. </summary>
		private readonly AtomicLong nextId;
		/// <summary>
		/// Neuron's features set size. </summary>
		private readonly int featureSize;
		/// <summary>
		/// Links. </summary>
		private readonly ConcurrentDictionary<long?, Set<long?>> linkMap = new ConcurrentDictionary<long?, Set<long?>>();

		/// <summary>
		/// Comparator that prescribes an order of the neurons according
		/// to the increasing order of their identifier.
		/// </summary>
		[Serializable]
		public class NeuronIdentifierComparator : IComparer<Neuron>
		{
			/// <summary>
			/// Version identifier. </summary>
			internal const long serialVersionUID = 20130207L;

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual int Compare(Neuron a, Neuron b)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long aId = a.getIdentifier();
				long aId = a.Identifier;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long bId = b.getIdentifier();
				long bId = b.Identifier;
				return aId < bId ? - 1 : aId > bId ? 1 : 0;
			}
		}

		/// <summary>
		/// Constructor with restricted access, solely used for deserialization.
		/// </summary>
		/// <param name="nextId"> Next available identifier. </param>
		/// <param name="featureSize"> Number of features. </param>
		/// <param name="neuronList"> Neurons. </param>
		/// <param name="neighbourIdList"> Links associated to each of the neurons in
		/// {@code neuronList}. </param>
		/// <exception cref="MathIllegalStateException"> if an inconsistency is detected
		/// (which probably means that the serialized form has been corrupted). </exception>
		internal Network(long nextId, int featureSize, Neuron[] neuronList, long[][] neighbourIdList)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numNeurons = neuronList.length;
			int numNeurons = neuronList.Length;
			if (numNeurons != neighbourIdList.Length)
			{
				throw new MathIllegalStateException();
			}

			for (int i = 0; i < numNeurons; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Neuron n = neuronList[i];
				Neuron n = neuronList[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long id = n.getIdentifier();
				long id = n.Identifier;
				if (id >= nextId)
				{
					throw new MathIllegalStateException();
				}
				neuronMap[id] = n;
				linkMap[id] = new HashSet<long?>();
			}

			for (int i = 0; i < numNeurons; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long aId = neuronList[i].getIdentifier();
				long aId = neuronList[i].Identifier;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Set<Long> aLinks = linkMap.get(aId);
				Set<long?> aLinks = linkMap[aId];
				foreach (long? bId in neighbourIdList[i])
				{
					if (neuronMap[bId] == null)
					{
						throw new MathIllegalStateException();
					}
					addLinkToLinkSet(aLinks, bId);
				}
			}

			this.nextId = new AtomicLong(nextId);
			this.featureSize = featureSize;
		}

		/// <param name="initialIdentifier"> Identifier for the first neuron that
		/// will be added to this network. </param>
		/// <param name="featureSize"> Size of the neuron's features. </param>
		public Network(long initialIdentifier, int featureSize)
		{
			nextId = new AtomicLong(initialIdentifier);
			this.featureSize = featureSize;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual IEnumerator<Neuron> iterator()
		{
			return neuronMap.Values.GetEnumerator();
		}

		/// <summary>
		/// Creates a list of the neurons, sorted in a custom order.
		/// </summary>
		/// <param name="comparator"> <seealso cref="Comparator"/> used for sorting the neurons. </param>
		/// <returns> a list of neurons, sorted in the order prescribed by the
		/// given {@code comparator}. </returns>
		/// <seealso cref= NeuronIdentifierComparator </seealso>
		public virtual ICollection<Neuron> getNeurons(IComparer<Neuron> comparator)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Neuron> neurons = new java.util.ArrayList<Neuron>();
			IList<Neuron> neurons = new List<Neuron>();
			neurons.AddRange(neuronMap.Values);

			neurons.Sort(comparator);

			return neurons;
		}

		/// <summary>
		/// Creates a neuron and assigns it a unique identifier.
		/// </summary>
		/// <param name="features"> Initial values for the neuron's features. </param>
		/// <returns> the neuron's identifier. </returns>
		/// <exception cref="DimensionMismatchException"> if the length of {@code features}
		/// is different from the expected size (as set by the
		/// <seealso cref="#Network(long,int) constructor"/>). </exception>
		public virtual long createNeuron(double[] features)
		{
			if (features.Length != featureSize)
			{
				throw new DimensionMismatchException(features.Length, featureSize);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long id = createNextId();
			long id = createNextId();
			neuronMap[id] = new Neuron(id, features);
			linkMap[id] = new HashSet<long?>();
			return id;
		}

		/// <summary>
		/// Deletes a neuron.
		/// Links from all neighbours to the removed neuron will also be
		/// <seealso cref="#deleteLink(Neuron,Neuron) deleted"/>.
		/// </summary>
		/// <param name="neuron"> Neuron to be removed from this network. </param>
		/// <exception cref="NoSuchElementException"> if {@code n} does not belong to
		/// this network. </exception>
		public virtual void deleteNeuron(Neuron neuron)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Collection<Neuron> neighbours = getNeighbours(neuron);
			ICollection<Neuron> neighbours = getNeighbours(neuron);

			// Delete links to from neighbours.
			foreach (Neuron n in neighbours)
			{
				deleteLink(n, neuron);
			}

			// Remove neuron.
			neuronMap.Remove(neuron.Identifier);
		}

		/// <summary>
		/// Gets the size of the neurons' features set.
		/// </summary>
		/// <returns> the size of the features set. </returns>
		public virtual int FeaturesSize
		{
			get
			{
				return featureSize;
			}
		}

		/// <summary>
		/// Adds a link from neuron {@code a} to neuron {@code b}.
		/// Note: the link is not bi-directional; if a bi-directional link is
		/// required, an additional call must be made with {@code a} and
		/// {@code b} exchanged in the argument list.
		/// </summary>
		/// <param name="a"> Neuron. </param>
		/// <param name="b"> Neuron. </param>
		/// <exception cref="NoSuchElementException"> if the neurons do not exist in the
		/// network. </exception>
		public virtual void addLink(Neuron a, Neuron b)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long aId = a.getIdentifier();
			long aId = a.Identifier;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long bId = b.getIdentifier();
			long bId = b.Identifier;

			// Check that the neurons belong to this network.
			if (a != getNeuron(aId))
			{
				throw new NoSuchElementException(Convert.ToString(aId));
			}
			if (b != getNeuron(bId))
			{
				throw new NoSuchElementException(Convert.ToString(bId));
			}

			// Add link from "a" to "b".
			addLinkToLinkSet(linkMap[aId], bId);
		}

		/// <summary>
		/// Adds a link to neuron {@code id} in given {@code linkSet}.
		/// Note: no check verifies that the identifier indeed belongs
		/// to this network.
		/// </summary>
		/// <param name="linkSet"> Neuron identifier. </param>
		/// <param name="id"> Neuron identifier. </param>
		private void addLinkToLinkSet(Set<long?> linkSet, long id)
		{
			linkSet.add(id);
		}

		/// <summary>
		/// Deletes the link between neurons {@code a} and {@code b}.
		/// </summary>
		/// <param name="a"> Neuron. </param>
		/// <param name="b"> Neuron. </param>
		/// <exception cref="NoSuchElementException"> if the neurons do not exist in the
		/// network. </exception>
		public virtual void deleteLink(Neuron a, Neuron b)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long aId = a.getIdentifier();
			long aId = a.Identifier;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long bId = b.getIdentifier();
			long bId = b.Identifier;

			// Check that the neurons belong to this network.
			if (a != getNeuron(aId))
			{
				throw new NoSuchElementException(Convert.ToString(aId));
			}
			if (b != getNeuron(bId))
			{
				throw new NoSuchElementException(Convert.ToString(bId));
			}

			// Delete link from "a" to "b".
			deleteLinkFromLinkSet(linkMap[aId], bId);
		}

		/// <summary>
		/// Deletes a link to neuron {@code id} in given {@code linkSet}.
		/// Note: no check verifies that the identifier indeed belongs
		/// to this network.
		/// </summary>
		/// <param name="linkSet"> Neuron identifier. </param>
		/// <param name="id"> Neuron identifier. </param>
		private void deleteLinkFromLinkSet(Set<long?> linkSet, long id)
		{
			linkSet.remove(id);
		}

		/// <summary>
		/// Retrieves the neuron with the given (unique) {@code id}.
		/// </summary>
		/// <param name="id"> Identifier. </param>
		/// <returns> the neuron associated with the given {@code id}. </returns>
		/// <exception cref="NoSuchElementException"> if the neuron does not exist in the
		/// network. </exception>
		public virtual Neuron getNeuron(long id)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Neuron n = neuronMap.get(id);
			Neuron n = neuronMap[id];
			if (n == null)
			{
				throw new NoSuchElementException(Convert.ToString(id));
			}
			return n;
		}

		/// <summary>
		/// Retrieves the neurons in the neighbourhood of any neuron in the
		/// {@code neurons} list. </summary>
		/// <param name="neurons"> Neurons for which to retrieve the neighbours. </param>
		/// <returns> the list of neighbours. </returns>
		/// <seealso cref= #getNeighbours(Iterable,Iterable) </seealso>
		public virtual ICollection<Neuron> getNeighbours(IEnumerable<Neuron> neurons)
		{
			return getNeighbours(neurons, null);
		}

		/// <summary>
		/// Retrieves the neurons in the neighbourhood of any neuron in the
		/// {@code neurons} list.
		/// The {@code exclude} list allows to retrieve the "concentric"
		/// neighbourhoods by removing the neurons that belong to the inner
		/// "circles".
		/// </summary>
		/// <param name="neurons"> Neurons for which to retrieve the neighbours. </param>
		/// <param name="exclude"> Neurons to exclude from the returned list.
		/// Can be {@code null}. </param>
		/// <returns> the list of neighbours. </returns>
		public virtual ICollection<Neuron> getNeighbours(IEnumerable<Neuron> neurons, IEnumerable<Neuron> exclude)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Set<Long> idList = new java.util.HashSet<Long>();
			Set<long?> idList = new HashSet<long?>();

			foreach (Neuron n in neurons)
			{
				idList.addAll(linkMap[n.Identifier]);
			}
			if (exclude != null)
			{
				foreach (Neuron n in exclude)
				{
					idList.remove(n.Identifier);
				}
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Neuron> neuronList = new java.util.ArrayList<Neuron>();
			IList<Neuron> neuronList = new List<Neuron>();
			foreach (long? id in idList)
			{
				neuronList.Add(getNeuron(id));
			}

			return neuronList;
		}

		/// <summary>
		/// Retrieves the neighbours of the given neuron.
		/// </summary>
		/// <param name="neuron"> Neuron for which to retrieve the neighbours. </param>
		/// <returns> the list of neighbours. </returns>
		/// <seealso cref= #getNeighbours(Neuron,Iterable) </seealso>
		public virtual ICollection<Neuron> getNeighbours(Neuron neuron)
		{
			return getNeighbours(neuron, null);
		}

		/// <summary>
		/// Retrieves the neighbours of the given neuron.
		/// </summary>
		/// <param name="neuron"> Neuron for which to retrieve the neighbours. </param>
		/// <param name="exclude"> Neurons to exclude from the returned list.
		/// Can be {@code null}. </param>
		/// <returns> the list of neighbours. </returns>
		public virtual ICollection<Neuron> getNeighbours(Neuron neuron, IEnumerable<Neuron> exclude)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Set<Long> idList = linkMap.get(neuron.getIdentifier());
			Set<long?> idList = linkMap[neuron.Identifier];
			if (exclude != null)
			{
				foreach (Neuron n in exclude)
				{
					idList.remove(n.Identifier);
				}
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Neuron> neuronList = new java.util.ArrayList<Neuron>();
			IList<Neuron> neuronList = new List<Neuron>();
			foreach (long? id in idList)
			{
				neuronList.Add(getNeuron(id));
			}

			return neuronList;
		}

		/// <summary>
		/// Creates a neuron identifier.
		/// </summary>
		/// <returns> a value that will serve as a unique identifier. </returns>
		private long? createNextId()
		{
			return nextId.AndIncrement;
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
//ORIGINAL LINE: final Neuron[] neuronList = neuronMap.values().toArray(new Neuron[0]);
			Neuron[] neuronList = neuronMap.Values.toArray(new Neuron[0]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long[][] neighbourIdList = new long[neuronList.length][];
			long[][] neighbourIdList = new long[neuronList.Length][];

			for (int i = 0; i < neuronList.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Collection<Neuron> neighbours = getNeighbours(neuronList[i]);
				ICollection<Neuron> neighbours = getNeighbours(neuronList[i]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long[] neighboursId = new long[neighbours.size()];
				long[] neighboursId = new long[neighbours.Count];
				int count = 0;
				foreach (Neuron n in neighbours)
				{
					neighboursId[count] = n.Identifier;
					++count;
				}
				neighbourIdList[i] = neighboursId;
			}

			return new SerializationProxy(nextId.get(), featureSize, neuronList, neighbourIdList);
		}

		/// <summary>
		/// Serialization.
		/// </summary>
		[Serializable]
		private class SerializationProxy
		{
			/// <summary>
			/// Serializable. </summary>
			internal const long serialVersionUID = 20130207L;
			/// <summary>
			/// Next identifier. </summary>
			internal readonly long nextId;
			/// <summary>
			/// Number of features. </summary>
			internal readonly int featureSize;
			/// <summary>
			/// Neurons. </summary>
			internal readonly Neuron[] neuronList;
			/// <summary>
			/// Links. </summary>
			internal readonly long[][] neighbourIdList;

			/// <param name="nextId"> Next available identifier. </param>
			/// <param name="featureSize"> Number of features. </param>
			/// <param name="neuronList"> Neurons. </param>
			/// <param name="neighbourIdList"> Links associated to each of the neurons in
			/// {@code neuronList}. </param>
			internal SerializationProxy(long nextId, int featureSize, Neuron[] neuronList, long[][] neighbourIdList)
			{
				this.nextId = nextId;
				this.featureSize = featureSize;
				this.neuronList = neuronList;
				this.neighbourIdList = neighbourIdList;
			}

			/// <summary>
			/// Custom serialization.
			/// </summary>
			/// <returns> the <seealso cref="Network"/> for which this instance is the proxy. </returns>
			internal virtual object readResolve()
			{
				return new Network(nextId, featureSize, neuronList, neighbourIdList);
			}
		}
	}

}
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

namespace mathlib.ml.neuralnet
{

	using Precision = mathlib.util.Precision;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;


	/// <summary>
	/// Describes a neuron element of a neural network.
	/// 
	/// This class aims to be thread-safe.
	/// 
	/// @version $Id: Neuron.java 1566092 2014-02-08 18:48:29Z tn $
	/// @since 3.3
	/// </summary>
	[Serializable]
	public class Neuron
	{
		/// <summary>
		/// Serializable. </summary>
		private const long serialVersionUID = 20130207L;
		/// <summary>
		/// Identifier. </summary>
		private readonly long identifier;
		/// <summary>
		/// Length of the feature set. </summary>
		private readonly int size;
		/// <summary>
		/// Neuron data. </summary>
		private readonly AtomicReference<double[]> features;

		/// <summary>
		/// Creates a neuron.
		/// The size of the feature set is fixed to the length of the given
		/// argument.
		/// <br/>
		/// Constructor is package-private: Neurons must be
		/// <seealso cref="Network#createNeuron(double[]) created"/> by the network
		/// instance to which they will belong.
		/// </summary>
		/// <param name="identifier"> Identifier (assigned by the <seealso cref="Network"/>). </param>
		/// <param name="features"> Initial values of the feature set. </param>
		internal Neuron(long identifier, double[] features)
		{
			this.identifier = identifier;
			this.size = features.Length;
			this.features = new AtomicReference<double[]>(features.clone());
		}

		/// <summary>
		/// Gets the neuron's identifier.
		/// </summary>
		/// <returns> the identifier. </returns>
		public virtual long Identifier
		{
			get
			{
				return identifier;
			}
		}

		/// <summary>
		/// Gets the length of the feature set.
		/// </summary>
		/// <returns> the number of features. </returns>
		public virtual int Size
		{
			get
			{
				return size;
			}
		}

		/// <summary>
		/// Gets the neuron's features.
		/// </summary>
		/// <returns> a copy of the neuron's features. </returns>
		public virtual double[] Features
		{
			get
			{
				return features.get().clone();
			}
		}

		/// <summary>
		/// Tries to atomically update the neuron's features.
		/// Update will be performed only if the expected values match the
		/// current values.<br/>
		/// In effect, when concurrent threads call this method, the state
		/// could be modified by one, so that it does not correspond to the
		/// the state assumed by another.
		/// Typically, a caller <seealso cref="#getFeatures() retrieves the current state"/>,
		/// and uses it to compute the new state.
		/// During this computation, another thread might have done the same
		/// thing, and updated the state: If the current thread were to proceed
		/// with its own update, it would overwrite the new state (which might
		/// already have been used by yet other threads).
		/// To prevent this, the method does not perform the update when a
		/// concurrent modification has been detected, and returns {@code false}.
		/// When this happens, the caller should fetch the new current state,
		/// redo its computation, and call this method again.
		/// </summary>
		/// <param name="expect"> Current values of the features, as assumed by the caller.
		/// Update will never succeed if the contents of this array does not match
		/// the values returned by <seealso cref="#getFeatures()"/>. </param>
		/// <param name="update"> Features's new values. </param>
		/// <returns> {@code true} if the update was successful, {@code false}
		/// otherwise. </returns>
		/// <exception cref="DimensionMismatchException"> if the length of {@code update} is
		/// not the same as specified in the {@link #Neuron(long,double[])
		/// constructor}. </exception>
		public virtual bool compareAndSetFeatures(double[] expect, double[] update)
		{
			if (update.Length != size)
			{
				throw new DimensionMismatchException(update.Length, size);
			}

			// Get the internal reference. Note that this must not be a copy;
			// otherwise the "compareAndSet" below will always fail.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] current = features.get();
			double[] current = features.get();
			if (!containSameValues(current, expect))
			{
				// Some other thread already modified the state.
				return false;
			}

			if (features.compareAndSet(current, update.clone()))
			{
				// The current thread could atomically update the state.
				return true;
			}
			else
			{
				// Some other thread came first.
				return false;
			}
		}

		/// <summary>
		/// Checks whether the contents of both arrays is the same.
		/// </summary>
		/// <param name="current"> Current values. </param>
		/// <param name="expect"> Expected values. </param>
		/// <exception cref="DimensionMismatchException"> if the length of {@code expected}
		/// is not the same as specified in the {@link #Neuron(long,double[])
		/// constructor}. </exception>
		/// <returns> {@code true} if the arrays contain the same values. </returns>
		private bool containSameValues(double[] current, double[] expect)
		{
			if (expect.Length != size)
			{
				throw new DimensionMismatchException(expect.Length, size);
			}

			for (int i = 0; i < size; i++)
			{
				if (!Precision.Equals(current[i], expect[i]))
				{
					return false;
				}
			}
			return true;
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
			return new SerializationProxy(identifier, features.get());
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
			/// Features. </summary>
			internal readonly double[] features;
			/// <summary>
			/// Identifier. </summary>
			internal readonly long identifier;

			/// <param name="identifier"> Identifier. </param>
			/// <param name="features"> Features. </param>
			internal SerializationProxy(long identifier, double[] features)
			{
				this.identifier = identifier;
				this.features = features;
			}

			/// <summary>
			/// Custom serialization.
			/// </summary>
			/// <returns> the <seealso cref="Neuron"/> for which this instance is the proxy. </returns>
			internal virtual object readResolve()
			{
				return new Neuron(identifier, features);
			}
		}
	}

}
using System;
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

namespace mathlib.ml.neuralnet.twod
{

	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using MathInternalError = mathlib.exception.MathInternalError;

	/// <summary>
	/// Neural network with the topology of a two-dimensional surface.
	/// Each neuron defines one surface element.
	/// <br/>
	/// This network is primarily intended to represent a
	/// <a href="http://en.wikipedia.org/wiki/Kohonen">
	///  Self Organizing Feature Map</a>.
	/// </summary>
	/// <seealso cref= mathlib.ml.neuralnet.sofm
	/// @version $Id: NeuronSquareMesh2D.java 1566396 2014-02-09 20:36:24Z tn $
	/// @since 3.3 </seealso>
	[Serializable]
	public class NeuronSquareMesh2D
	{
		/// <summary>
		/// Underlying network. </summary>
		private readonly Network network;
		/// <summary>
		/// Number of rows. </summary>
		private readonly int numberOfRows;
		/// <summary>
		/// Number of columns. </summary>
		private readonly int numberOfColumns;
		/// <summary>
		/// Wrap. </summary>
		private readonly bool wrapRows;
		/// <summary>
		/// Wrap. </summary>
		private readonly bool wrapColumns;
		/// <summary>
		/// Neighbourhood type. </summary>
		private readonly SquareNeighbourhood neighbourhood;
		/// <summary>
		/// Mapping of the 2D coordinates (in the rectangular mesh) to
		/// the neuron identifiers (attributed by the <seealso cref="#network"/>
		/// instance).
		/// </summary>
		private readonly long[][] identifiers;

		/// <summary>
		/// Constructor with restricted access, solely used for deserialization.
		/// </summary>
		/// <param name="wrapRowDim"> Whether to wrap the first dimension (i.e the first
		/// and last neurons will be linked together). </param>
		/// <param name="wrapColDim"> Whether to wrap the second dimension (i.e the first
		/// and last neurons will be linked together). </param>
		/// <param name="neighbourhoodType"> Neighbourhood type. </param>
		/// <param name="featuresList"> Arrays that will initialize the features sets of
		/// the network's neurons. </param>
		/// <exception cref="NumberIsTooSmallException"> if {@code numRows < 2} or
		/// {@code numCols < 2}. </exception>
		internal NeuronSquareMesh2D(bool wrapRowDim, bool wrapColDim, SquareNeighbourhood neighbourhoodType, double[][][] featuresList)
		{
			numberOfRows = featuresList.Length;
			numberOfColumns = featuresList[0].Length;

			if (numberOfRows < 2)
			{
				throw new NumberIsTooSmallException(numberOfRows, 2, true);
			}
			if (numberOfColumns < 2)
			{
				throw new NumberIsTooSmallException(numberOfColumns, 2, true);
			}

			wrapRows = wrapRowDim;
			wrapColumns = wrapColDim;
			neighbourhood = neighbourhoodType;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int fLen = featuresList[0][0].length;
			int fLen = featuresList[0][0].Length;
			network = new Network(0, fLen);
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: identifiers = new long[numberOfRows][numberOfColumns];
			identifiers = RectangularArrays.ReturnRectangularLongArray(numberOfRows, numberOfColumns);

			// Add neurons.
			for (int i = 0; i < numberOfRows; i++)
			{
				for (int j = 0; j < numberOfColumns; j++)
				{
					identifiers[i][j] = network.createNeuron(featuresList[i][j]);
				}
			}

			// Add links.
			createLinks();
		}

		/// <summary>
		/// Creates a two-dimensional network composed of square cells:
		/// Each neuron not located on the border of the mesh has four
		/// neurons linked to it.
		/// <br/>
		/// The links are bi-directional.
		/// <br/>
		/// The topology of the network can also be a cylinder (if one
		/// of the dimensions is wrapped) or a torus (if both dimensions
		/// are wrapped).
		/// </summary>
		/// <param name="numRows"> Number of neurons in the first dimension. </param>
		/// <param name="wrapRowDim"> Whether to wrap the first dimension (i.e the first
		/// and last neurons will be linked together). </param>
		/// <param name="numCols"> Number of neurons in the second dimension. </param>
		/// <param name="wrapColDim"> Whether to wrap the second dimension (i.e the first
		/// and last neurons will be linked together). </param>
		/// <param name="neighbourhoodType"> Neighbourhood type. </param>
		/// <param name="featureInit"> Array of functions that will initialize the
		/// corresponding element of the features set of each newly created
		/// neuron. In particular, the size of this array defines the size of
		/// feature set. </param>
		/// <exception cref="NumberIsTooSmallException"> if {@code numRows < 2} or
		/// {@code numCols < 2}. </exception>
		public NeuronSquareMesh2D(int numRows, bool wrapRowDim, int numCols, bool wrapColDim, SquareNeighbourhood neighbourhoodType, FeatureInitializer[] featureInit)
		{
			if (numRows < 2)
			{
				throw new NumberIsTooSmallException(numRows, 2, true);
			}
			if (numCols < 2)
			{
				throw new NumberIsTooSmallException(numCols, 2, true);
			}

			numberOfRows = numRows;
			wrapRows = wrapRowDim;
			numberOfColumns = numCols;
			wrapColumns = wrapColDim;
			neighbourhood = neighbourhoodType;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: identifiers = new long[numberOfRows][numberOfColumns];
			identifiers = RectangularArrays.ReturnRectangularLongArray(numberOfRows, numberOfColumns);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int fLen = featureInit.length;
			int fLen = featureInit.Length;
			network = new Network(0, fLen);

			// Add neurons.
			for (int i = 0; i < numRows; i++)
			{
				for (int j = 0; j < numCols; j++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] features = new double[fLen];
					double[] features = new double[fLen];
					for (int fIndex = 0; fIndex < fLen; fIndex++)
					{
						features[fIndex] = featureInit[fIndex].value();
					}
					identifiers[i][j] = network.createNeuron(features);
				}
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
		/// Gets the number of neurons in each row of this map.
		/// </summary>
		/// <returns> the number of rows. </returns>
		public virtual int NumberOfRows
		{
			get
			{
				return numberOfRows;
			}
		}

		/// <summary>
		/// Gets the number of neurons in each column of this map.
		/// </summary>
		/// <returns> the number of column. </returns>
		public virtual int NumberOfColumns
		{
			get
			{
				return numberOfColumns;
			}
		}

		/// <summary>
		/// Retrieves the neuron at location {@code (i, j)} in the map.
		/// </summary>
		/// <param name="i"> Row index. </param>
		/// <param name="j"> Column index. </param>
		/// <returns> the neuron at {@code (i, j)}. </returns>
		/// <exception cref="OutOfRangeException"> if {@code i} or {@code j} is
		/// out of range. </exception>
		public virtual Neuron getNeuron(int i, int j)
		{
			if (i < 0 || i >= numberOfRows)
			{
				throw new OutOfRangeException(i, 0, numberOfRows - 1);
			}
			if (j < 0 || j >= numberOfColumns)
			{
				throw new OutOfRangeException(j, 0, numberOfColumns - 1);
			}

			return network.getNeuron(identifiers[i][j]);
		}

		/// <summary>
		/// Creates the neighbour relationships between neurons.
		/// </summary>
		private void createLinks()
		{
			// "linkEnd" will store the identifiers of the "neighbours".
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Long> linkEnd = new java.util.ArrayList<Long>();
			IList<long?> linkEnd = new List<long?>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iLast = numberOfRows - 1;
			int iLast = numberOfRows - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jLast = numberOfColumns - 1;
			int jLast = numberOfColumns - 1;
			for (int i = 0; i < numberOfRows; i++)
			{
				for (int j = 0; j < numberOfColumns; j++)
				{
					linkEnd.Clear();

					switch (neighbourhood)
					{

					case SquareNeighbourhood.MOORE:
						// Add links to "diagonal" neighbours.
						if (i > 0)
						{
							if (j > 0)
							{
								linkEnd.Add(identifiers[i - 1][j - 1]);
							}
							if (j < jLast)
							{
								linkEnd.Add(identifiers[i - 1][j + 1]);
							}
						}
						if (i < iLast)
						{
							if (j > 0)
							{
								linkEnd.Add(identifiers[i + 1][j - 1]);
							}
							if (j < jLast)
							{
								linkEnd.Add(identifiers[i + 1][j + 1]);
							}
						}
						if (wrapRows)
						{
							if (i == 0)
							{
								if (j > 0)
								{
									linkEnd.Add(identifiers[iLast][j - 1]);
								}
								if (j < jLast)
								{
									linkEnd.Add(identifiers[iLast][j + 1]);
								}
							}
							else if (i == iLast)
							{
								if (j > 0)
								{
									linkEnd.Add(identifiers[0][j - 1]);
								}
								if (j < jLast)
								{
									linkEnd.Add(identifiers[0][j + 1]);
								}
							}
						}
						if (wrapColumns)
						{
							if (j == 0)
							{
								if (i > 0)
								{
									linkEnd.Add(identifiers[i - 1][jLast]);
								}
								if (i < iLast)
								{
									linkEnd.Add(identifiers[i + 1][jLast]);
								}
							}
							else if (j == jLast)
							{
								 if (i > 0)
								 {
									 linkEnd.Add(identifiers[i - 1][0]);
								 }
								 if (i < iLast)
								 {
									 linkEnd.Add(identifiers[i + 1][0]);
								 }
							}
						}
						if (wrapRows && wrapColumns)
						{
							if (i == 0 && j == 0)
							{
								linkEnd.Add(identifiers[iLast][jLast]);
							}
							else if (i == 0 && j == jLast)
							{
								linkEnd.Add(identifiers[iLast][0]);
							}
							else if (i == iLast && j == 0)
							{
								linkEnd.Add(identifiers[0][jLast]);
							}
							else if (i == iLast && j == jLast)
							{
								linkEnd.Add(identifiers[0][0]);
							}
						}

						// Case falls through since the "Moore" neighbourhood
						// also contains the neurons that belong to the "Von
						// Neumann" neighbourhood.

						// fallthru (CheckStyle)
						goto case VON_NEUMANN;
					case SquareNeighbourhood.VON_NEUMANN:
						// Links to preceding and following "row".
						if (i > 0)
						{
							linkEnd.Add(identifiers[i - 1][j]);
						}
						if (i < iLast)
						{
							linkEnd.Add(identifiers[i + 1][j]);
						}
						if (wrapRows)
						{
							if (i == 0)
							{
								linkEnd.Add(identifiers[iLast][j]);
							}
							else if (i == iLast)
							{
								linkEnd.Add(identifiers[0][j]);
							}
						}

						// Links to preceding and following "column".
						if (j > 0)
						{
							linkEnd.Add(identifiers[i][j - 1]);
						}
						if (j < jLast)
						{
							linkEnd.Add(identifiers[i][j + 1]);
						}
						if (wrapColumns)
						{
							if (j == 0)
							{
								linkEnd.Add(identifiers[i][jLast]);
							}
							else if (j == jLast)
							{
								linkEnd.Add(identifiers[i][0]);
							}
						}
						break;

					default:
						throw new MathInternalError(); // Cannot happen.
					}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.ml.neuralnet.Neuron aNeuron = network.getNeuron(identifiers[i][j]);
					Neuron aNeuron = network.getNeuron(identifiers[i][j]);
					foreach (long b in linkEnd)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.ml.neuralnet.Neuron bNeuron = network.getNeuron(b);
						Neuron bNeuron = network.getNeuron(b);
						// Link to all neighbours.
						// The reverse links will be added as the loop proceeds.
						network.addLink(aNeuron, bNeuron);
					}
				}
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
//ORIGINAL LINE: final double[][][] featuresList = new double[numberOfRows][numberOfColumns][];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][][] featuresList = new double[numberOfRows][numberOfColumns][];
			double[][][] featuresList = RectangularArrays.ReturnRectangularDoubleArray(numberOfRows, numberOfColumns, -1);
			for (int i = 0; i < numberOfRows; i++)
			{
				for (int j = 0; j < numberOfColumns; j++)
				{
					featuresList[i][j] = getNeuron(i, j).Features;
				}
			}

			return new SerializationProxy(wrapRows, wrapColumns, neighbourhood, featuresList);
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
			internal readonly bool wrapRows;
			/// <summary>
			/// Wrap. </summary>
			internal readonly bool wrapColumns;
			/// <summary>
			/// Neighbourhood type. </summary>
			internal readonly SquareNeighbourhood neighbourhood;
			/// <summary>
			/// Neurons' features. </summary>
			internal readonly double[][][] featuresList;

			/// <param name="wrapRows"> Whether the row dimension is wrapped. </param>
			/// <param name="wrapColumns"> Whether the column dimension is wrapped. </param>
			/// <param name="neighbourhood"> Neighbourhood type. </param>
			/// <param name="featuresList"> List of neurons features.
			/// {@code neuronList}. </param>
			internal SerializationProxy(bool wrapRows, bool wrapColumns, SquareNeighbourhood neighbourhood, double[][][] featuresList)
			{
				this.wrapRows = wrapRows;
				this.wrapColumns = wrapColumns;
				this.neighbourhood = neighbourhood;
				this.featuresList = featuresList;
			}

			/// <summary>
			/// Custom serialization.
			/// </summary>
			/// <returns> the <seealso cref="Neuron"/> for which this instance is the proxy. </returns>
			internal virtual object readResolve()
			{
				return new NeuronSquareMesh2D(wrapRows, wrapColumns, neighbourhood, featuresList);
			}
		}
	}

}
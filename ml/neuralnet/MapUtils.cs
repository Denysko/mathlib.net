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

namespace mathlib.ml.neuralnet
{

	using DistanceMeasure = mathlib.ml.distance.DistanceMeasure;
	using NeuronSquareMesh2D = mathlib.ml.neuralnet.twod.NeuronSquareMesh2D;
	using NoDataException = mathlib.exception.NoDataException;
	using mathlib.util;

	/// <summary>
	/// Utilities for network maps.
	/// 
	/// @version $Id: MapUtils.java 1566092 2014-02-08 18:48:29Z tn $
	/// @since 3.3
	/// </summary>
	public class MapUtils
	{
		/// <summary>
		/// Class contains only static methods.
		/// </summary>
		private MapUtils()
		{
		}

		/// <summary>
		/// Finds the neuron that best matches the given features.
		/// </summary>
		/// <param name="features"> Data. </param>
		/// <param name="neurons"> List of neurons to scan. If the list is empty
		/// {@code null} will be returned. </param>
		/// <param name="distance"> Distance function. The neuron's features are
		/// passed as the first argument to <seealso cref="DistanceMeasure#compute(double[],double[])"/>. </param>
		/// <returns> the neuron whose features are closest to the given data. </returns>
		/// <exception cref="mathlib.exception.DimensionMismatchException">
		/// if the size of the input is not compatible with the neurons features
		/// size. </exception>
		public static Neuron findBest(double[] features, IEnumerable<Neuron> neurons, DistanceMeasure distance)
		{
			Neuron best = null;
			double min = double.PositiveInfinity;
			foreach (Neuron n in neurons)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d = distance.compute(n.getFeatures(), features);
				double d = distance.compute(n.Features, features);
				if (d < min)
				{
					min = d;
					best = n;
				}
			}

			return best;
		}

		/// <summary>
		/// Finds the two neurons that best match the given features.
		/// </summary>
		/// <param name="features"> Data. </param>
		/// <param name="neurons"> List of neurons to scan. If the list is empty
		/// {@code null} will be returned. </param>
		/// <param name="distance"> Distance function. The neuron's features are
		/// passed as the first argument to <seealso cref="DistanceMeasure#compute(double[],double[])"/>. </param>
		/// <returns> the two neurons whose features are closest to the given data. </returns>
		/// <exception cref="mathlib.exception.DimensionMismatchException">
		/// if the size of the input is not compatible with the neurons features
		/// size. </exception>
		public static Pair<Neuron, Neuron> findBestAndSecondBest(double[] features, IEnumerable<Neuron> neurons, DistanceMeasure distance)
		{
			Neuron[] best = new Neuron[] {null, null};
			double[] min = new double[] {double.PositiveInfinity, double.PositiveInfinity};
			foreach (Neuron n in neurons)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d = distance.compute(n.getFeatures(), features);
				double d = distance.compute(n.Features, features);
				if (d < min[0])
				{
					// Replace second best with old best.
					min[1] = min[0];
					best[1] = best[0];

					// Store current as new best.
					min[0] = d;
					best[0] = n;
				}
				else if (d < min[1])
				{
					// Replace old second best with current.
					min[1] = d;
					best[1] = n;
				}
			}

			return new Pair<Neuron, Neuron>(best[0], best[1]);
		}

		/// <summary>
		/// Computes the <a href="http://en.wikipedia.org/wiki/U-Matrix">
		///  U-matrix</a> of a two-dimensional map.
		/// </summary>
		/// <param name="map"> Network. </param>
		/// <param name="distance"> Function to use for computing the average
		/// distance from a neuron to its neighbours. </param>
		/// <returns> the matrix of average distances. </returns>
		public static double[][] computeU(NeuronSquareMesh2D map, DistanceMeasure distance)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numRows = map.getNumberOfRows();
			int numRows = map.NumberOfRows;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numCols = map.getNumberOfColumns();
			int numCols = map.NumberOfColumns;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] uMatrix = new double[numRows][numCols];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] uMatrix = new double[numRows][numCols];
			double[][] uMatrix = RectangularArrays.ReturnRectangularDoubleArray(numRows, numCols);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Network net = map.getNetwork();
			Network net = map.Network;

			for (int i = 0; i < numRows; i++)
			{
				for (int j = 0; j < numCols; j++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Neuron neuron = map.getNeuron(i, j);
					Neuron neuron = map.getNeuron(i, j);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Collection<Neuron> neighbours = net.getNeighbours(neuron);
					ICollection<Neuron> neighbours = net.getNeighbours(neuron);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] features = neuron.getFeatures();
					double[] features = neuron.Features;

					double d = 0;
					int count = 0;
					foreach (Neuron n in neighbours)
					{
						++count;
						d += distance.compute(features, n.Features);
					}

					uMatrix[i][j] = d / count;
				}
			}

			return uMatrix;
		}

		/// <summary>
		/// Computes the "hit" histogram of a two-dimensional map.
		/// </summary>
		/// <param name="data"> Feature vectors. </param>
		/// <param name="map"> Network. </param>
		/// <param name="distance"> Function to use for determining the best matching unit. </param>
		/// <returns> the number of hits for each neuron in the map. </returns>
		public static int[][] computeHitHistogram(IEnumerable<double[]> data, NeuronSquareMesh2D map, DistanceMeasure distance)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.HashMap<Neuron, Integer> hit = new java.util.HashMap<Neuron, Integer>();
			Dictionary<Neuron, int?> hit = new Dictionary<Neuron, int?>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Network net = map.getNetwork();
			Network net = map.Network;

			foreach (double[] f in data)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Neuron best = findBest(f, net, distance);
				Neuron best = findBest(f, net, distance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Integer count = hit.get(best);
				int? count = hit[best];
				if (count == null)
				{
					hit[best] = 1;
				}
				else
				{
					hit[best] = count + 1;
				}
			}

			// Copy the histogram data into a 2D map.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numRows = map.getNumberOfRows();
			int numRows = map.NumberOfRows;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numCols = map.getNumberOfColumns();
			int numCols = map.NumberOfColumns;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[][] histo = new int[numRows][numCols];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: int[][] histo = new int[numRows][numCols];
			int[][] histo = RectangularArrays.ReturnRectangularIntArray(numRows, numCols);

			for (int i = 0; i < numRows; i++)
			{
				for (int j = 0; j < numCols; j++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Neuron neuron = map.getNeuron(i, j);
					Neuron neuron = map.getNeuron(i, j);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Integer count = hit.get(neuron);
					int? count = hit[neuron];
					if (count == null)
					{
						histo[i][j] = 0;
					}
					else
					{
						histo[i][j] = count;
					}
				}
			}

			return histo;
		}

		/// <summary>
		/// Computes the quantization error.
		/// The quantization error is the average distance between a feature vector
		/// and its "best matching unit" (closest neuron).
		/// </summary>
		/// <param name="data"> Feature vectors. </param>
		/// <param name="neurons"> List of neurons to scan. </param>
		/// <param name="distance"> Distance function. </param>
		/// <returns> the error. </returns>
		/// <exception cref="NoDataException"> if {@code data} is empty. </exception>
		public static double computeQuantizationError(IEnumerable<double[]> data, IEnumerable<Neuron> neurons, DistanceMeasure distance)
		{
			double d = 0;
			int count = 0;
			foreach (double[] f in data)
			{
				++count;
				d += distance.compute(f, findBest(f, neurons, distance).Features);
			}

			if (count == 0)
			{
				throw new NoDataException();
			}

			return d / count;
		}

		/// <summary>
		/// Computes the topographic error.
		/// The topographic error is the proportion of data for which first and
		/// second best matching units are not adjacent in the map.
		/// </summary>
		/// <param name="data"> Feature vectors. </param>
		/// <param name="net"> Network. </param>
		/// <param name="distance"> Distance function. </param>
		/// <returns> the error. </returns>
		/// <exception cref="NoDataException"> if {@code data} is empty. </exception>
		public static double computeTopographicError(IEnumerable<double[]> data, Network net, DistanceMeasure distance)
		{
			int notAdjacentCount = 0;
			int count = 0;
			foreach (double[] f in data)
			{
				++count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.util.Pair<Neuron, Neuron> p = findBestAndSecondBest(f, net, distance);
				Pair<Neuron, Neuron> p = findBestAndSecondBest(f, net, distance);
				if (!net.getNeighbours(p.First).Contains(p.Second))
				{
					// Increment count if first and second best matching units
					// are not neighbours.
					++notAdjacentCount;
				}
			}

			if (count == 0)
			{
				throw new NoDataException();
			}

			return ((double) notAdjacentCount) / count;
		}
	}

}
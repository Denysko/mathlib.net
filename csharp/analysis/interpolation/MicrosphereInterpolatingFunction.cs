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
namespace org.apache.commons.math3.analysis.interpolation
{


	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NoDataException = org.apache.commons.math3.exception.NoDataException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using ArrayRealVector = org.apache.commons.math3.linear.ArrayRealVector;
	using RealVector = org.apache.commons.math3.linear.RealVector;
	using UnitSphereRandomVectorGenerator = org.apache.commons.math3.random.UnitSphereRandomVectorGenerator;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Interpolating function that implements the
	/// <a href="http://www.dudziak.com/microsphere.php">Microsphere Projection</a>.
	/// 
	/// @version $Id: MicrosphereInterpolatingFunction.java 1455194 2013-03-11 15:45:54Z luc $
	/// </summary>
	public class MicrosphereInterpolatingFunction : MultivariateFunction
	{
		/// <summary>
		/// Space dimension.
		/// </summary>
		private readonly int dimension;
		/// <summary>
		/// Internal accounting data for the interpolation algorithm.
		/// Each element of the list corresponds to one surface element of
		/// the microsphere.
		/// </summary>
		private readonly IList<MicrosphereSurfaceElement> microsphere;
		/// <summary>
		/// Exponent used in the power law that computes the weights of the
		/// sample data.
		/// </summary>
		private readonly double brightnessExponent;
		/// <summary>
		/// Sample data.
		/// </summary>
		private readonly IDictionary<RealVector, double?> samples;

		/// <summary>
		/// Class for storing the accounting data needed to perform the
		/// microsphere projection.
		/// </summary>
		private class MicrosphereSurfaceElement
		{
			/// <summary>
			/// Normal vector characterizing a surface element. </summary>
			internal readonly RealVector normal_Renamed;
			/// <summary>
			/// Illumination received from the brightest sample. </summary>
			internal double brightestIllumination;
			/// <summary>
			/// Brightest sample. </summary>
			internal KeyValuePair<RealVector, double?> brightestSample;

			/// <param name="n"> Normal vector characterizing a surface element
			/// of the microsphere. </param>
			internal MicrosphereSurfaceElement(double[] n)
			{
				normal_Renamed = new ArrayRealVector(n);
			}

			/// <summary>
			/// Return the normal vector. </summary>
			/// <returns> the normal vector </returns>
			internal virtual RealVector normal()
			{
				return normal_Renamed;
			}

			/// <summary>
			/// Reset "illumination" and "sampleIndex".
			/// </summary>
			internal virtual void reset()
			{
				brightestIllumination = 0;
				brightestSample = null;
			}

			/// <summary>
			/// Store the illumination and index of the brightest sample. </summary>
			/// <param name="illuminationFromSample"> illumination received from sample </param>
			/// <param name="sample"> current sample illuminating the element </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: void store(final double illuminationFromSample, final java.util.Map.Entry<org.apache.commons.math3.linear.RealVector, Double> sample)
			internal virtual void store(double illuminationFromSample, KeyValuePair<RealVector, double?> sample)
			{
				if (illuminationFromSample > this.brightestIllumination)
				{
					this.brightestIllumination = illuminationFromSample;
					this.brightestSample = sample;
				}
			}

			/// <summary>
			/// Get the illumination of the element. </summary>
			/// <returns> the illumination. </returns>
			internal virtual double illumination()
			{
				return brightestIllumination;
			}

			/// <summary>
			/// Get the sample illuminating the element the most. </summary>
			/// <returns> the sample. </returns>
			internal virtual KeyValuePair<RealVector, double?> sample()
			{
				return brightestSample;
			}
		}

		/// <param name="xval"> Arguments for the interpolation points.
		/// {@code xval[i][0]} is the first component of interpolation point
		/// {@code i}, {@code xval[i][1]} is the second component, and so on
		/// until {@code xval[i][d-1]}, the last component of that interpolation
		/// point (where {@code dimension} is thus the dimension of the sampled
		/// space). </param>
		/// <param name="yval"> Values for the interpolation points. </param>
		/// <param name="brightnessExponent"> Brightness dimming factor. </param>
		/// <param name="microsphereElements"> Number of surface elements of the
		/// microsphere. </param>
		/// <param name="rand"> Unit vector generator for creating the microsphere. </param>
		/// <exception cref="DimensionMismatchException"> if the lengths of {@code yval} and
		/// {@code xval} (equal to {@code n}, the number of interpolation points)
		/// do not match, or the the arrays {@code xval[0]} ... {@code xval[n]},
		/// have lengths different from {@code dimension}. </exception>
		/// <exception cref="NoDataException"> if there an array has zero-length. </exception>
		/// <exception cref="NullArgumentException"> if an argument is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MicrosphereInterpolatingFunction(double[][] xval, double[] yval, int brightnessExponent, int microsphereElements, org.apache.commons.math3.random.UnitSphereRandomVectorGenerator rand) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NullArgumentException
		public MicrosphereInterpolatingFunction(double[][] xval, double[] yval, int brightnessExponent, int microsphereElements, UnitSphereRandomVectorGenerator rand)
		{
			if (xval == null || yval == null)
			{
				throw new NullArgumentException();
			}
			if (xval.Length == 0)
			{
				throw new NoDataException();
			}
			if (xval.Length != yval.Length)
			{
				throw new DimensionMismatchException(xval.Length, yval.Length);
			}
			if (xval[0] == null)
			{
				throw new NullArgumentException();
			}

			dimension = xval[0].Length;
			this.brightnessExponent = brightnessExponent;

			// Copy data samples.
			samples = new Dictionary<RealVector, double?>(yval.Length);
			for (int i = 0; i < xval.Length; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] xvalI = xval[i];
				double[] xvalI = xval[i];
				if (xvalI == null)
				{
					throw new NullArgumentException();
				}
				if (xvalI.Length != dimension)
				{
					throw new DimensionMismatchException(xvalI.Length, dimension);
				}

				samples[new ArrayRealVector(xvalI)] = yval[i];
			}

			microsphere = new List<MicrosphereSurfaceElement>(microsphereElements);
			// Generate the microsphere, assuming that a fairly large number of
			// randomly generated normals will represent a sphere.
			for (int i = 0; i < microsphereElements; i++)
			{
				microsphere.Add(new MicrosphereSurfaceElement(rand.nextVector()));
			}
		}

		/// <param name="point"> Interpolation point. </param>
		/// <returns> the interpolated value. </returns>
		/// <exception cref="DimensionMismatchException"> if point dimension does not math sample </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double value(double[] point) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual double value(double[] point)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.RealVector p = new org.apache.commons.math3.linear.ArrayRealVector(point);
			RealVector p = new ArrayRealVector(point);

			// Reset.
			foreach (MicrosphereSurfaceElement md in microsphere)
			{
				md.reset();
			}

			// Compute contribution of each sample points to the microsphere elements illumination
			foreach (KeyValuePair<RealVector, double?> sd in samples)
			{

				// Vector between interpolation point and current sample point.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.RealVector diff = sd.getKey().subtract(p);
				RealVector diff = sd.Key.subtract(p);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double diffNorm = diff.getNorm();
				double diffNorm = diff.Norm;

				if (FastMath.abs(diffNorm) < FastMath.ulp(1d))
				{
					// No need to interpolate, as the interpolation point is
					// actually (very close to) one of the sampled points.
					return sd.Value;
				}

				foreach (MicrosphereSurfaceElement md in microsphere)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double w = org.apache.commons.math3.util.FastMath.pow(diffNorm, -brightnessExponent);
					double w = FastMath.pow(diffNorm, -brightnessExponent);
					md.store(cosAngle(diff, md.normal()) * w, sd);
				}

			}

			// Interpolation calculation.
			double value = 0;
			double totalWeight = 0;
			foreach (MicrosphereSurfaceElement md in microsphere)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double iV = md.illumination();
				double iV = md.illumination();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map.Entry<org.apache.commons.math3.linear.RealVector, Double> sd = md.sample();
				KeyValuePair<RealVector, double?> sd = md.sample();
				if (sd != null)
				{
					value += iV * sd.Value;
					totalWeight += iV;
				}
			}

			return value / totalWeight;
		}

		/// <summary>
		/// Compute the cosine of the angle between 2 vectors.
		/// </summary>
		/// <param name="v"> Vector. </param>
		/// <param name="w"> Vector. </param>
		/// <returns> the cosine of the angle between {@code v} and {@code w}. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double cosAngle(final org.apache.commons.math3.linear.RealVector v, final org.apache.commons.math3.linear.RealVector w)
		private double cosAngle(RealVector v, RealVector w)
		{
			return v.dotProduct(w) / (v.Norm * w.Norm);
		}
	}

}
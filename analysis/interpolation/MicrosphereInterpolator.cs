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
namespace mathlib.analysis.interpolation
{

	using NotPositiveException = mathlib.exception.NotPositiveException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using NoDataException = mathlib.exception.NoDataException;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using UnitSphereRandomVectorGenerator = mathlib.random.UnitSphereRandomVectorGenerator;

	/// <summary>
	/// Interpolator that implements the algorithm described in
	/// <em>William Dudziak</em>'s
	/// <a href="http://www.dudziak.com/microsphere.pdf">MS thesis</a>.
	/// 
	/// @since 2.1
	/// @version $Id: MicrosphereInterpolator.java 1379904 2012-09-01 23:54:52Z erans $
	/// </summary>
	public class MicrosphereInterpolator : MultivariateInterpolator
	{
		/// <summary>
		/// Default number of surface elements that composes the microsphere.
		/// </summary>
		public const int DEFAULT_MICROSPHERE_ELEMENTS = 2000;
		/// <summary>
		/// Default exponent used the weights calculation.
		/// </summary>
		public const int DEFAULT_BRIGHTNESS_EXPONENT = 2;
		/// <summary>
		/// Number of surface elements of the microsphere.
		/// </summary>
		private readonly int microsphereElements;
		/// <summary>
		/// Exponent used in the power law that computes the weights of the
		/// sample data.
		/// </summary>
		private readonly int brightnessExponent;

		/// <summary>
		/// Create a microsphere interpolator with default settings.
		/// Calling this constructor is equivalent to call {@link
		/// #MicrosphereInterpolator(int, int)
		/// MicrosphereInterpolator(MicrosphereInterpolator.DEFAULT_MICROSPHERE_ELEMENTS,
		/// MicrosphereInterpolator.DEFAULT_BRIGHTNESS_EXPONENT)}.
		/// </summary>
		public MicrosphereInterpolator() : this(DEFAULT_MICROSPHERE_ELEMENTS, DEFAULT_BRIGHTNESS_EXPONENT)
		{
		}

		/// <summary>
		/// Create a microsphere interpolator. </summary>
		/// <param name="elements"> Number of surface elements of the microsphere. </param>
		/// <param name="exponent"> Exponent used in the power law that computes the
		/// weights (distance dimming factor) of the sample data. </param>
		/// <exception cref="NotPositiveException"> if {@code exponent < 0}. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if {@code elements <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MicrosphereInterpolator(final int elements, final int exponent) throws mathlib.exception.NotPositiveException, mathlib.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public MicrosphereInterpolator(int elements, int exponent)
		{
			if (exponent < 0)
			{
				throw new NotPositiveException(exponent);
			}
			if (elements <= 0)
			{
				throw new NotStrictlyPositiveException(elements);
			}

			microsphereElements = elements;
			brightnessExponent = exponent;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public mathlib.analysis.MultivariateFunction interpolate(final double[][] xval, final double[] yval) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NoDataException, mathlib.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual MultivariateFunction interpolate(double[][] xval, double[] yval)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.random.UnitSphereRandomVectorGenerator rand = new mathlib.random.UnitSphereRandomVectorGenerator(xval[0].length);
			UnitSphereRandomVectorGenerator rand = new UnitSphereRandomVectorGenerator(xval[0].Length);
			return new MicrosphereInterpolatingFunction(xval, yval, brightnessExponent, microsphereElements, rand);
		}
	}

}
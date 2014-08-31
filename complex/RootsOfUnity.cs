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
namespace mathlib.complex
{

	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using MathIllegalStateException = org.apache.commons.math3.exception.MathIllegalStateException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using ZeroException = org.apache.commons.math3.exception.ZeroException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// A helper class for the computation and caching of the {@code n}-th roots of
	/// unity.
	/// 
	/// @version $Id: RootsOfUnity.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0
	/// </summary>
	[Serializable]
	public class RootsOfUnity
	{

		/// <summary>
		/// Serializable version id. </summary>
		private const long serialVersionUID = 20120201L;

		/// <summary>
		/// Number of roots of unity. </summary>
		private int omegaCount;

		/// <summary>
		/// Real part of the roots. </summary>
		private double[] omegaReal;

		/// <summary>
		/// Imaginary part of the {@code n}-th roots of unity, for positive values
		/// of {@code n}. In this array, the roots are stored in counter-clockwise
		/// order.
		/// </summary>
		private double[] omegaImaginaryCounterClockwise;

		/// <summary>
		/// Imaginary part of the {@code n}-th roots of unity, for negative values
		/// of {@code n}. In this array, the roots are stored in clockwise order.
		/// </summary>
		private double[] omegaImaginaryClockwise;

		/// <summary>
		/// {@code true} if <seealso cref="#computeRoots(int)"/> was called with a positive
		/// value of its argument {@code n}. In this case, counter-clockwise ordering
		/// of the roots of unity should be used.
		/// </summary>
		private bool isCounterClockWise;

		/// <summary>
		/// Build an engine for computing the {@code n}-th roots of unity.
		/// </summary>
		public RootsOfUnity()
		{

			omegaCount = 0;
			omegaReal = null;
			omegaImaginaryCounterClockwise = null;
			omegaImaginaryClockwise = null;
			isCounterClockWise = true;
		}

		/// <summary>
		/// Returns {@code true} if <seealso cref="#computeRoots(int)"/> was called with a
		/// positive value of its argument {@code n}. If {@code true}, then
		/// counter-clockwise ordering of the roots of unity should be used.
		/// </summary>
		/// <returns> {@code true} if the roots of unity are stored in
		/// counter-clockwise order </returns>
		/// <exception cref="MathIllegalStateException"> if no roots of unity have been computed
		/// yet </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized boolean isCounterClockWise() throws org.apache.commons.math3.exception.MathIllegalStateException
		public virtual bool CounterClockWise
		{
			get
			{
				lock (this)
				{
            
					if (omegaCount == 0)
					{
						throw new MathIllegalStateException(LocalizedFormats.ROOTS_OF_UNITY_NOT_COMPUTED_YET);
					}
					return isCounterClockWise;
				}
			}
		}

		/// <summary>
		/// <p>
		/// Computes the {@code n}-th roots of unity. The roots are stored in
		/// {@code omega[]}, such that {@code omega[k] = w ^ k}, where
		/// {@code k = 0, ..., n - 1}, {@code w = exp(2 * pi * i / n)} and
		/// {@code i = sqrt(-1)}.
		/// </p>
		/// <p>
		/// Note that {@code n} can be positive of negative
		/// </p>
		/// <ul>
		/// <li>{@code abs(n)} is always the number of roots of unity.</li>
		/// <li>If {@code n > 0}, then the roots are stored in counter-clockwise order.</li>
		/// <li>If {@code n < 0}, then the roots are stored in clockwise order.</p>
		/// </ul>
		/// </summary>
		/// <param name="n"> the (signed) number of roots of unity to be computed </param>
		/// <exception cref="ZeroException"> if {@code n = 0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void computeRoots(int n) throws org.apache.commons.math3.exception.ZeroException
		public virtual void computeRoots(int n)
		{
			lock (this)
			{
        
				if (n == 0)
				{
					throw new ZeroException(LocalizedFormats.CANNOT_COMPUTE_0TH_ROOT_OF_UNITY);
				}
        
				isCounterClockWise = n > 0;
        
				// avoid repetitive calculations
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int absN = org.apache.commons.math3.util.FastMath.abs(n);
				int absN = FastMath.abs(n);
        
				if (absN == omegaCount)
				{
					return;
				}
        
				// calculate everything from scratch
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = 2.0 * org.apache.commons.math3.util.FastMath.PI / absN;
				double t = 2.0 * FastMath.PI / absN;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cosT = org.apache.commons.math3.util.FastMath.cos(t);
				double cosT = FastMath.cos(t);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sinT = org.apache.commons.math3.util.FastMath.sin(t);
				double sinT = FastMath.sin(t);
				omegaReal = new double[absN];
				omegaImaginaryCounterClockwise = new double[absN];
				omegaImaginaryClockwise = new double[absN];
				omegaReal[0] = 1.0;
				omegaImaginaryCounterClockwise[0] = 0.0;
				omegaImaginaryClockwise[0] = 0.0;
				for (int i = 1; i < absN; i++)
				{
					omegaReal[i] = omegaReal[i - 1] * cosT - omegaImaginaryCounterClockwise[i - 1] * sinT;
					omegaImaginaryCounterClockwise[i] = omegaReal[i - 1] * sinT + omegaImaginaryCounterClockwise[i - 1] * cosT;
					omegaImaginaryClockwise[i] = -omegaImaginaryCounterClockwise[i];
				}
				omegaCount = absN;
			}
		}

		/// <summary>
		/// Get the real part of the {@code k}-th {@code n}-th root of unity.
		/// </summary>
		/// <param name="k"> index of the {@code n}-th root of unity </param>
		/// <returns> real part of the {@code k}-th {@code n}-th root of unity </returns>
		/// <exception cref="MathIllegalStateException"> if no roots of unity have been
		/// computed yet </exception>
		/// <exception cref="MathIllegalArgumentException"> if {@code k} is out of range </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized double getReal(int k) throws org.apache.commons.math3.exception.MathIllegalStateException, org.apache.commons.math3.exception.MathIllegalArgumentException
		public virtual double getReal(int k)
		{
			lock (this)
			{
        
				if (omegaCount == 0)
				{
					throw new MathIllegalStateException(LocalizedFormats.ROOTS_OF_UNITY_NOT_COMPUTED_YET);
				}
				if ((k < 0) || (k >= omegaCount))
				{
					throw new OutOfRangeException(LocalizedFormats.OUT_OF_RANGE_ROOT_OF_UNITY_INDEX, Convert.ToInt32(k), Convert.ToInt32(0), Convert.ToInt32(omegaCount - 1));
				}
        
				return omegaReal[k];
			}
		}

		/// <summary>
		/// Get the imaginary part of the {@code k}-th {@code n}-th root of unity.
		/// </summary>
		/// <param name="k"> index of the {@code n}-th root of unity </param>
		/// <returns> imaginary part of the {@code k}-th {@code n}-th root of unity </returns>
		/// <exception cref="MathIllegalStateException"> if no roots of unity have been
		/// computed yet </exception>
		/// <exception cref="OutOfRangeException"> if {@code k} is out of range </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized double getImaginary(int k) throws org.apache.commons.math3.exception.MathIllegalStateException, org.apache.commons.math3.exception.OutOfRangeException
		public virtual double getImaginary(int k)
		{
			lock (this)
			{
        
				if (omegaCount == 0)
				{
					throw new MathIllegalStateException(LocalizedFormats.ROOTS_OF_UNITY_NOT_COMPUTED_YET);
				}
				if ((k < 0) || (k >= omegaCount))
				{
					throw new OutOfRangeException(LocalizedFormats.OUT_OF_RANGE_ROOT_OF_UNITY_INDEX, Convert.ToInt32(k), Convert.ToInt32(0), Convert.ToInt32(omegaCount - 1));
				}
        
				return isCounterClockWise ? omegaImaginaryCounterClockwise[k] : omegaImaginaryClockwise[k];
			}
		}

		/// <summary>
		/// Returns the number of roots of unity currently stored. If
		/// <seealso cref="#computeRoots(int)"/> was called with {@code n}, then this method
		/// returns {@code abs(n)}. If no roots of unity have been computed yet, this
		/// method returns 0.
		/// </summary>
		/// <returns> the number of roots of unity currently stored </returns>
		public virtual int NumberOfRoots
		{
			get
			{
				lock (this)
				{
					return omegaCount;
				}
			}
		}
	}

}
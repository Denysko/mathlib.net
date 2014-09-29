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
namespace mathlib.linear
{


	using UnivariateFunction = mathlib.analysis.UnivariateFunction;
	using NotPositiveException = mathlib.exception.NotPositiveException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using MathUtils = mathlib.util.MathUtils;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// This class implements the <seealso cref="RealVector"/> interface with a double array.
	/// @version $Id: ArrayRealVector.java 1538368 2013-11-03 13:57:37Z erans $
	/// @since 2.0
	/// </summary>
	[Serializable]
	public class ArrayRealVector : RealVector
	{
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -1097961340710804027L;
		/// <summary>
		/// Default format. </summary>
		private static readonly RealVectorFormat DEFAULT_FORMAT = RealVectorFormat.Instance;

		/// <summary>
		/// Entries of the vector. </summary>
		private double[] data;

		/// <summary>
		/// Build a 0-length vector.
		/// Zero-length vectors may be used to initialized construction of vectors
		/// by data gathering. We start with zero-length and use either the {@link
		/// #ArrayRealVector(ArrayRealVector, ArrayRealVector)} constructor
		/// or one of the {@code append} method (<seealso cref="#append(double)"/>,
		/// <seealso cref="#append(ArrayRealVector)"/>) to gather data into this vector.
		/// </summary>
		public ArrayRealVector()
		{
			data = new double[0];
		}

		/// <summary>
		/// Construct a vector of zeroes.
		/// </summary>
		/// <param name="size"> Size of the vector. </param>
		public ArrayRealVector(int size)
		{
			data = new double[size];
		}

		/// <summary>
		/// Construct a vector with preset values.
		/// </summary>
		/// <param name="size"> Size of the vector </param>
		/// <param name="preset"> All entries will be set with this value. </param>
		public ArrayRealVector(int size, double preset)
		{
			data = new double[size];
			Arrays.fill(data, preset);
		}

		/// <summary>
		/// Construct a vector from an array, copying the input array.
		/// </summary>
		/// <param name="d"> Array. </param>
		public ArrayRealVector(double[] d)
		{
			data = d.clone();
		}

		/// <summary>
		/// Create a new ArrayRealVector using the input array as the underlying
		/// data array.
		/// If an array is built specially in order to be embedded in a
		/// ArrayRealVector and not used directly, the {@code copyArray} may be
		/// set to {@code false}. This will prevent the copying and improve
		/// performance as no new array will be built and no data will be copied.
		/// </summary>
		/// <param name="d"> Data for the new vector. </param>
		/// <param name="copyArray"> if {@code true}, the input array will be copied,
		/// otherwise it will be referenced. </param>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <seealso cref= #ArrayRealVector(double[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayRealVector(double[] d, boolean copyArray) throws mathlib.exception.NullArgumentException
		public ArrayRealVector(double[] d, bool copyArray)
		{
			if (d == null)
			{
				throw new NullArgumentException();
			}
			data = copyArray ? d.clone() : d;
		}

		/// <summary>
		/// Construct a vector from part of a array.
		/// </summary>
		/// <param name="d"> Array. </param>
		/// <param name="pos"> Position of first entry. </param>
		/// <param name="size"> Number of entries to copy. </param>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <exception cref="NumberIsTooLargeException"> if the size of {@code d} is less
		/// than {@code pos + size}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayRealVector(double[] d, int pos, int size) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooLargeException
		public ArrayRealVector(double[] d, int pos, int size)
		{
			if (d == null)
			{
				throw new NullArgumentException();
			}
			if (d.Length < pos + size)
			{
				throw new NumberIsTooLargeException(pos + size, d.Length, true);
			}
			data = new double[size];
			Array.Copy(d, pos, data, 0, size);
		}

		/// <summary>
		/// Construct a vector from an array.
		/// </summary>
		/// <param name="d"> Array of {@code Double}s. </param>
		public ArrayRealVector(double?[] d)
		{
			data = new double[d.Length];
			for (int i = 0; i < d.Length; i++)
			{
				data[i] = (double)d[i];
			}
		}

		/// <summary>
		/// Construct a vector from part of an array.
		/// </summary>
		/// <param name="d"> Array. </param>
		/// <param name="pos"> Position of first entry. </param>
		/// <param name="size"> Number of entries to copy. </param>
		/// <exception cref="NullArgumentException"> if {@code d} is {@code null}. </exception>
		/// <exception cref="NumberIsTooLargeException"> if the size of {@code d} is less
		/// than {@code pos + size}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayRealVector(Double[] d, int pos, int size) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooLargeException
		public ArrayRealVector(double?[] d, int pos, int size)
		{
			if (d == null)
			{
				throw new NullArgumentException();
			}
			if (d.Length < pos + size)
			{
				throw new NumberIsTooLargeException(pos + size, d.Length, true);
			}
			data = new double[size];
			for (int i = pos; i < pos + size; i++)
			{
				data[i - pos] = (double)d[i];
			}
		}

		/// <summary>
		/// Construct a vector from another vector, using a deep copy.
		/// </summary>
		/// <param name="v"> vector to copy. </param>
		/// <exception cref="NullArgumentException"> if {@code v} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayRealVector(RealVector v) throws mathlib.exception.NullArgumentException
		public ArrayRealVector(RealVector v)
		{
			if (v == null)
			{
				throw new NullArgumentException();
			}
			data = new double[v.Dimension];
			for (int i = 0; i < data.Length; ++i)
			{
				data[i] = v.getEntry(i);
			}
		}

		/// <summary>
		/// Construct a vector from another vector, using a deep copy.
		/// </summary>
		/// <param name="v"> Vector to copy. </param>
		/// <exception cref="NullArgumentException"> if {@code v} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayRealVector(ArrayRealVector v) throws mathlib.exception.NullArgumentException
		public ArrayRealVector(ArrayRealVector v) : this(v, true)
		{
		}

		/// <summary>
		/// Construct a vector from another vector.
		/// </summary>
		/// <param name="v"> Vector to copy. </param>
		/// <param name="deep"> If {@code true} perform a deep copy, otherwise perform a
		/// shallow copy. </param>
		public ArrayRealVector(ArrayRealVector v, bool deep)
		{
			data = deep ? v.data.clone() : v.data;
		}

		/// <summary>
		/// Construct a vector by appending one vector to another vector. </summary>
		/// <param name="v1"> First vector (will be put in front of the new vector). </param>
		/// <param name="v2"> Second vector (will be put at back of the new vector). </param>
		public ArrayRealVector(ArrayRealVector v1, ArrayRealVector v2)
		{
			data = new double[v1.data.Length + v2.data.Length];
			Array.Copy(v1.data, 0, data, 0, v1.data.Length);
			Array.Copy(v2.data, 0, data, v1.data.Length, v2.data.Length);
		}

		/// <summary>
		/// Construct a vector by appending one vector to another vector. </summary>
		/// <param name="v1"> First vector (will be put in front of the new vector). </param>
		/// <param name="v2"> Second vector (will be put at back of the new vector). </param>
		public ArrayRealVector(ArrayRealVector v1, RealVector v2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int l1 = v1.data.length;
			int l1 = v1.data.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int l2 = v2.getDimension();
			int l2 = v2.Dimension;
			data = new double[l1 + l2];
			Array.Copy(v1.data, 0, data, 0, l1);
			for (int i = 0; i < l2; ++i)
			{
				data[l1 + i] = v2.getEntry(i);
			}
		}

		/// <summary>
		/// Construct a vector by appending one vector to another vector. </summary>
		/// <param name="v1"> First vector (will be put in front of the new vector). </param>
		/// <param name="v2"> Second vector (will be put at back of the new vector). </param>
		public ArrayRealVector(RealVector v1, ArrayRealVector v2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int l1 = v1.getDimension();
			int l1 = v1.Dimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int l2 = v2.data.length;
			int l2 = v2.data.Length;
			data = new double[l1 + l2];
			for (int i = 0; i < l1; ++i)
			{
				data[i] = v1.getEntry(i);
			}
			Array.Copy(v2.data, 0, data, l1, l2);
		}

		/// <summary>
		/// Construct a vector by appending one vector to another vector. </summary>
		/// <param name="v1"> First vector (will be put in front of the new vector). </param>
		/// <param name="v2"> Second vector (will be put at back of the new vector). </param>
		public ArrayRealVector(ArrayRealVector v1, double[] v2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int l1 = v1.getDimension();
			int l1 = v1.Dimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int l2 = v2.length;
			int l2 = v2.Length;
			data = new double[l1 + l2];
			Array.Copy(v1.data, 0, data, 0, l1);
			Array.Copy(v2, 0, data, l1, l2);
		}

		/// <summary>
		/// Construct a vector by appending one vector to another vector. </summary>
		/// <param name="v1"> First vector (will be put in front of the new vector). </param>
		/// <param name="v2"> Second vector (will be put at back of the new vector). </param>
		public ArrayRealVector(double[] v1, ArrayRealVector v2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int l1 = v1.length;
			int l1 = v1.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int l2 = v2.getDimension();
			int l2 = v2.Dimension;
			data = new double[l1 + l2];
			Array.Copy(v1, 0, data, 0, l1);
			Array.Copy(v2.data, 0, data, l1, l2);
		}

		/// <summary>
		/// Construct a vector by appending one vector to another vector. </summary>
		/// <param name="v1"> first vector (will be put in front of the new vector) </param>
		/// <param name="v2"> second vector (will be put at back of the new vector) </param>
		public ArrayRealVector(double[] v1, double[] v2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int l1 = v1.length;
			int l1 = v1.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int l2 = v2.length;
			int l2 = v2.Length;
			data = new double[l1 + l2];
			Array.Copy(v1, 0, data, 0, l1);
			Array.Copy(v2, 0, data, l1, l2);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override ArrayRealVector copy()
		{
			return new ArrayRealVector(this, true);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public ArrayRealVector add(RealVector v) throws mathlib.exception.DimensionMismatchException
		public override ArrayRealVector add(RealVector v)
		{
			if (v is ArrayRealVector)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vData = ((ArrayRealVector) v).data;
				double[] vData = ((ArrayRealVector) v).data;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = vData.length;
				int dim = vData.Length;
				checkVectorDimensions(dim);
				ArrayRealVector result = new ArrayRealVector(dim);
				double[] resultData = result.data;
				for (int i = 0; i < dim; i++)
				{
					resultData[i] = data[i] + vData[i];
				}
				return result;
			}
			else
			{
				checkVectorDimensions(v);
				double[] @out = data.clone();
				IEnumerator<Entry> it = v.GetEnumerator();
				while (it.MoveNext())
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Entry e = it.Current;
					Entry e = it.Current;
					@out[e.Index] += e.Value;
				}
				return new ArrayRealVector(@out, false);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public ArrayRealVector subtract(RealVector v) throws mathlib.exception.DimensionMismatchException
		public override ArrayRealVector subtract(RealVector v)
		{
			if (v is ArrayRealVector)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vData = ((ArrayRealVector) v).data;
				double[] vData = ((ArrayRealVector) v).data;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = vData.length;
				int dim = vData.Length;
				checkVectorDimensions(dim);
				ArrayRealVector result = new ArrayRealVector(dim);
				double[] resultData = result.data;
				for (int i = 0; i < dim; i++)
				{
					resultData[i] = data[i] - vData[i];
				}
				return result;
			}
			else
			{
				checkVectorDimensions(v);
				double[] @out = data.clone();
				IEnumerator<Entry> it = v.GetEnumerator();
				while (it.MoveNext())
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Entry e = it.Current;
					Entry e = it.Current;
					@out[e.Index] -= e.Value;
				}
				return new ArrayRealVector(@out, false);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override ArrayRealVector map(UnivariateFunction function)
		{
			return copy().mapToSelf(function);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override ArrayRealVector mapToSelf(UnivariateFunction function)
		{
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = function.value(data[i]);
			}
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override RealVector mapAddToSelf(double d)
		{
			for (int i = 0; i < data.Length; i++)
			{
				data[i] += d;
			}
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override RealVector mapSubtractToSelf(double d)
		{
			for (int i = 0; i < data.Length; i++)
			{
				data[i] -= d;
			}
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override RealVector mapMultiplyToSelf(double d)
		{
			for (int i = 0; i < data.Length; i++)
			{
				data[i] *= d;
			}
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override RealVector mapDivideToSelf(double d)
		{
			for (int i = 0; i < data.Length; i++)
			{
				data[i] /= d;
			}
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public ArrayRealVector ebeMultiply(RealVector v) throws mathlib.exception.DimensionMismatchException
		public override ArrayRealVector ebeMultiply(RealVector v)
		{
			if (v is ArrayRealVector)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vData = ((ArrayRealVector) v).data;
				double[] vData = ((ArrayRealVector) v).data;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = vData.length;
				int dim = vData.Length;
				checkVectorDimensions(dim);
				ArrayRealVector result = new ArrayRealVector(dim);
				double[] resultData = result.data;
				for (int i = 0; i < dim; i++)
				{
					resultData[i] = data[i] * vData[i];
				}
				return result;
			}
			else
			{
				checkVectorDimensions(v);
				double[] @out = data.clone();
				for (int i = 0; i < data.Length; i++)
				{
					@out[i] *= v.getEntry(i);
				}
				return new ArrayRealVector(@out, false);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public ArrayRealVector ebeDivide(RealVector v) throws mathlib.exception.DimensionMismatchException
		public override ArrayRealVector ebeDivide(RealVector v)
		{
			if (v is ArrayRealVector)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vData = ((ArrayRealVector) v).data;
				double[] vData = ((ArrayRealVector) v).data;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = vData.length;
				int dim = vData.Length;
				checkVectorDimensions(dim);
				ArrayRealVector result = new ArrayRealVector(dim);
				double[] resultData = result.data;
				for (int i = 0; i < dim; i++)
				{
					resultData[i] = data[i] / vData[i];
				}
				return result;
			}
			else
			{
				checkVectorDimensions(v);
				double[] @out = data.clone();
				for (int i = 0; i < data.Length; i++)
				{
					@out[i] /= v.getEntry(i);
				}
				return new ArrayRealVector(@out, false);
			}
		}

		/// <summary>
		/// Get a reference to the underlying data array.
		/// This method does not make a fresh copy of the underlying data.
		/// </summary>
		/// <returns> the array of entries. </returns>
		public virtual double[] DataRef
		{
			get
			{
				return data;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double dotProduct(RealVector v) throws mathlib.exception.DimensionMismatchException
		public override double dotProduct(RealVector v)
		{
			if (v is ArrayRealVector)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vData = ((ArrayRealVector) v).data;
				double[] vData = ((ArrayRealVector) v).data;
				checkVectorDimensions(vData.Length);
				double dot = 0;
				for (int i = 0; i < data.Length; i++)
				{
					dot += data[i] * vData[i];
				}
				return dot;
			}
			return base.dotProduct(v);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double Norm
		{
			get
			{
				double sum = 0;
				foreach (double a in data)
				{
					sum += a * a;
				}
				return FastMath.sqrt(sum);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double L1Norm
		{
			get
			{
				double sum = 0;
				foreach (double a in data)
				{
					sum += FastMath.abs(a);
				}
				return sum;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double LInfNorm
		{
			get
			{
				double max = 0;
				foreach (double a in data)
				{
					max = FastMath.max(max, FastMath.abs(a));
				}
				return max;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double getDistance(RealVector v) throws mathlib.exception.DimensionMismatchException
		public override double getDistance(RealVector v)
		{
			if (v is ArrayRealVector)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vData = ((ArrayRealVector) v).data;
				double[] vData = ((ArrayRealVector) v).data;
				checkVectorDimensions(vData.Length);
				double sum = 0;
				for (int i = 0; i < data.Length; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double delta = data[i] - vData[i];
					double delta = data[i] - vData[i];
					sum += delta * delta;
				}
				return FastMath.sqrt(sum);
			}
			else
			{
				checkVectorDimensions(v);
				double sum = 0;
				for (int i = 0; i < data.Length; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double delta = data[i] - v.getEntry(i);
					double delta = data[i] - v.getEntry(i);
					sum += delta * delta;
				}
				return FastMath.sqrt(sum);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double getL1Distance(RealVector v) throws mathlib.exception.DimensionMismatchException
		public override double getL1Distance(RealVector v)
		{
			if (v is ArrayRealVector)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vData = ((ArrayRealVector) v).data;
				double[] vData = ((ArrayRealVector) v).data;
				checkVectorDimensions(vData.Length);
				double sum = 0;
				for (int i = 0; i < data.Length; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double delta = data[i] - vData[i];
					double delta = data[i] - vData[i];
					sum += FastMath.abs(delta);
				}
				return sum;
			}
			else
			{
				checkVectorDimensions(v);
				double sum = 0;
				for (int i = 0; i < data.Length; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double delta = data[i] - v.getEntry(i);
					double delta = data[i] - v.getEntry(i);
					sum += FastMath.abs(delta);
				}
				return sum;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double getLInfDistance(RealVector v) throws mathlib.exception.DimensionMismatchException
		public override double getLInfDistance(RealVector v)
		{
			if (v is ArrayRealVector)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vData = ((ArrayRealVector) v).data;
				double[] vData = ((ArrayRealVector) v).data;
				checkVectorDimensions(vData.Length);
				double max = 0;
				for (int i = 0; i < data.Length; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double delta = data[i] - vData[i];
					double delta = data[i] - vData[i];
					max = FastMath.max(max, FastMath.abs(delta));
				}
				return max;
			}
			else
			{
				checkVectorDimensions(v);
				double max = 0;
				for (int i = 0; i < data.Length; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double delta = data[i] - v.getEntry(i);
					double delta = data[i] - v.getEntry(i);
					max = FastMath.max(max, FastMath.abs(delta));
				}
				return max;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override RealMatrix outerProduct(RealVector v)
		{
			if (v is ArrayRealVector)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vData = ((ArrayRealVector) v).data;
				double[] vData = ((ArrayRealVector) v).data;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = data.length;
				int m = data.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = vData.length;
				int n = vData.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix out = MatrixUtils.createRealMatrix(m, n);
				RealMatrix @out = MatrixUtils.createRealMatrix(m, n);
				for (int i = 0; i < m; i++)
				{
					for (int j = 0; j < n; j++)
					{
						@out.setEntry(i, j, data[i] * vData[j]);
					}
				}
				return @out;
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = data.length;
				int m = data.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = v.getDimension();
				int n = v.Dimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix out = MatrixUtils.createRealMatrix(m, n);
				RealMatrix @out = MatrixUtils.createRealMatrix(m, n);
				for (int i = 0; i < m; i++)
				{
					for (int j = 0; j < n; j++)
					{
						@out.setEntry(i, j, data[i] * v.getEntry(j));
					}
				}
				return @out;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double getEntry(int index) throws mathlib.exception.OutOfRangeException
		public override double getEntry(int index)
		{
			try
			{
				return data[index];
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new OutOfRangeException(LocalizedFormats.INDEX, index, 0, Dimension - 1);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int Dimension
		{
			get
			{
				return data.Length;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override RealVector append(RealVector v)
		{
			try
			{
				return new ArrayRealVector(this, (ArrayRealVector) v);
			}
			catch (System.InvalidCastException cce)
			{
				return new ArrayRealVector(this, v);
			}
		}

		/// <summary>
		/// Construct a vector by appending a vector to this vector.
		/// </summary>
		/// <param name="v"> Vector to append to this one. </param>
		/// <returns> a new vector. </returns>
		public virtual ArrayRealVector append(ArrayRealVector v)
		{
			return new ArrayRealVector(this, v);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override RealVector append(double @in)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] out = new double[data.length + 1];
			double[] @out = new double[data.Length + 1];
			Array.Copy(data, 0, @out, 0, data.Length);
			@out[data.Length] = @in;
			return new ArrayRealVector(@out, false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector getSubVector(int index, int n) throws mathlib.exception.OutOfRangeException, mathlib.exception.NotPositiveException
		public override RealVector getSubVector(int index, int n)
		{
			if (n < 0)
			{
				throw new NotPositiveException(LocalizedFormats.NUMBER_OF_ELEMENTS_SHOULD_BE_POSITIVE, n);
			}
			ArrayRealVector @out = new ArrayRealVector(n);
			try
			{
				Array.Copy(data, index, @out.data, 0, n);
			}
			catch (System.IndexOutOfRangeException e)
			{
				checkIndex(index);
				checkIndex(index + n - 1);
			}
			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setEntry(int index, double value) throws mathlib.exception.OutOfRangeException
		public override void setEntry(int index, double value)
		{
			try
			{
				data[index] = value;
			}
			catch (System.IndexOutOfRangeException e)
			{
				checkIndex(index);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void addToEntry(int index, double increment) throws mathlib.exception.OutOfRangeException
		public override void addToEntry(int index, double increment)
		{
			try
			{
			data[index] += increment;
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new OutOfRangeException(LocalizedFormats.INDEX, index, 0, data.Length - 1);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setSubVector(int index, RealVector v) throws mathlib.exception.OutOfRangeException
		public override void setSubVector(int index, RealVector v)
		{
			if (v is ArrayRealVector)
			{
				setSubVector(index, ((ArrayRealVector) v).data);
			}
			else
			{
				try
				{
					for (int i = index; i < index + v.Dimension; ++i)
					{
						data[i] = v.getEntry(i - index);
					}
				}
				catch (System.IndexOutOfRangeException e)
				{
					checkIndex(index);
					checkIndex(index + v.Dimension - 1);
				}
			}
		}

		/// <summary>
		/// Set a set of consecutive elements.
		/// </summary>
		/// <param name="index"> Index of first element to be set. </param>
		/// <param name="v"> Vector containing the values to set. </param>
		/// <exception cref="OutOfRangeException"> if the index is inconsistent with the vector
		/// size. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSubVector(int index, double[] v) throws mathlib.exception.OutOfRangeException
		public virtual void setSubVector(int index, double[] v)
		{
			try
			{
				Array.Copy(v, 0, data, index, v.Length);
			}
			catch (System.IndexOutOfRangeException e)
			{
				checkIndex(index);
				checkIndex(index + v.Length - 1);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override void set(double value)
		{
			Arrays.fill(data, value);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double[] toArray()
		{
			return data.clone();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return DEFAULT_FORMAT.format(this);
		}

		/// <summary>
		/// Check if instance and specified vectors have the same dimension.
		/// </summary>
		/// <param name="v"> Vector to compare instance with. </param>
		/// <exception cref="DimensionMismatchException"> if the vectors do not
		/// have the same dimension. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void checkVectorDimensions(RealVector v) throws mathlib.exception.DimensionMismatchException
		protected internal override void checkVectorDimensions(RealVector v)
		{
			checkVectorDimensions(v.Dimension);
		}

		/// <summary>
		/// Check if instance dimension is equal to some expected value.
		/// </summary>
		/// <param name="n"> Expected dimension. </param>
		/// <exception cref="DimensionMismatchException"> if the dimension is
		/// inconsistent with vector size. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void checkVectorDimensions(int n) throws mathlib.exception.DimensionMismatchException
		protected internal override void checkVectorDimensions(int n)
		{
			if (data.Length != n)
			{
				throw new DimensionMismatchException(data.Length, n);
			}
		}

		/// <summary>
		/// Check if any coordinate of this vector is {@code NaN}.
		/// </summary>
		/// <returns> {@code true} if any coordinate of this vector is {@code NaN},
		/// {@code false} otherwise. </returns>
		public override bool NaN
		{
			get
			{
				foreach (double v in data)
				{
					if (double.IsNaN(v))
					{
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Check whether any coordinate of this vector is infinite and none
		/// are {@code NaN}.
		/// </summary>
		/// <returns> {@code true} if any coordinate of this vector is infinite and
		/// none are {@code NaN}, {@code false} otherwise. </returns>
		public override bool Infinite
		{
			get
			{
				if (NaN)
				{
					return false;
				}
    
				foreach (double v in data)
				{
					if (double.IsInfinity(v))
					{
						return true;
					}
				}
    
				return false;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool Equals(object other)
		{
			if (this == other)
			{
				return true;
			}

			if (!(other is RealVector))
			{
				return false;
			}

			RealVector rhs = (RealVector) other;
			if (data.Length != rhs.Dimension)
			{
				return false;
			}

			if (rhs.NaN)
			{
				return this.NaN;
			}

			for (int i = 0; i < data.Length; ++i)
			{
				if (data[i] != rhs.getEntry(i))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// {@inheritDoc} All {@code NaN} values have the same hash code.
		/// </summary>
		public override int GetHashCode()
		{
			if (NaN)
			{
				return 9;
			}
			return MathUtils.hash(data);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public ArrayRealVector combine(double a, double b, RealVector y) throws mathlib.exception.DimensionMismatchException
		public override ArrayRealVector combine(double a, double b, RealVector y)
		{
			return copy().combineToSelf(a, b, y);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public ArrayRealVector combineToSelf(double a, double b, RealVector y) throws mathlib.exception.DimensionMismatchException
		public override ArrayRealVector combineToSelf(double a, double b, RealVector y)
		{
			if (y is ArrayRealVector)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yData = ((ArrayRealVector) y).data;
				double[] yData = ((ArrayRealVector) y).data;
				checkVectorDimensions(yData.Length);
				for (int i = 0; i < this.data.Length; i++)
				{
					data[i] = a * data[i] + b * yData[i];
				}
			}
			else
			{
				checkVectorDimensions(y);
				for (int i = 0; i < this.data.Length; i++)
				{
					data[i] = a * data[i] + b * y.getEntry(i);
				}
			}
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public double walkInDefaultOrder(final RealVectorPreservingVisitor visitor)
		public override double walkInDefaultOrder(RealVectorPreservingVisitor visitor)
		{
			visitor.start(data.Length, 0, data.Length - 1);
			for (int i = 0; i < data.Length; i++)
			{
				visitor.visit(i, data[i]);
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double walkInDefaultOrder(final RealVectorPreservingVisitor visitor, final int start, final int end) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double walkInDefaultOrder(RealVectorPreservingVisitor visitor, int start, int end)
		{
			checkIndices(start, end);
			visitor.start(data.Length, start, end);
			for (int i = start; i <= end; i++)
			{
				visitor.visit(i, data[i]);
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// In this implementation, the optimized order is the default order.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public double walkInOptimizedOrder(final RealVectorPreservingVisitor visitor)
		public override double walkInOptimizedOrder(RealVectorPreservingVisitor visitor)
		{
			return walkInDefaultOrder(visitor);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// In this implementation, the optimized order is the default order.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double walkInOptimizedOrder(final RealVectorPreservingVisitor visitor, final int start, final int end) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double walkInOptimizedOrder(RealVectorPreservingVisitor visitor, int start, int end)
		{
			return walkInDefaultOrder(visitor, start, end);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public double walkInDefaultOrder(final RealVectorChangingVisitor visitor)
		public override double walkInDefaultOrder(RealVectorChangingVisitor visitor)
		{
			visitor.start(data.Length, 0, data.Length - 1);
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = visitor.visit(i, data[i]);
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double walkInDefaultOrder(final RealVectorChangingVisitor visitor, final int start, final int end) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double walkInDefaultOrder(RealVectorChangingVisitor visitor, int start, int end)
		{
			checkIndices(start, end);
			visitor.start(data.Length, start, end);
			for (int i = start; i <= end; i++)
			{
				data[i] = visitor.visit(i, data[i]);
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// In this implementation, the optimized order is the default order.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public double walkInOptimizedOrder(final RealVectorChangingVisitor visitor)
		public override double walkInOptimizedOrder(RealVectorChangingVisitor visitor)
		{
			return walkInDefaultOrder(visitor);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// In this implementation, the optimized order is the default order.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double walkInOptimizedOrder(final RealVectorChangingVisitor visitor, final int start, final int end) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double walkInOptimizedOrder(RealVectorChangingVisitor visitor, int start, int end)
		{
			return walkInDefaultOrder(visitor, start, end);
		}
	}

}
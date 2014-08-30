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
namespace org.apache.commons.math3.linear
{

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MathArithmeticException = org.apache.commons.math3.exception.MathArithmeticException;
	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using OpenIntToDoubleHashMap = org.apache.commons.math3.util.OpenIntToDoubleHashMap;
	using Iterator = org.apache.commons.math3.util.OpenIntToDoubleHashMap.Iterator;

	/// <summary>
	/// This class implements the <seealso cref="RealVector"/> interface with a
	/// <seealso cref="OpenIntToDoubleHashMap"/> backing store.
	/// <p>
	///  Caveat: This implementation assumes that, for any {@code x},
	///  the equality {@code x * 0d == 0d} holds. But it is is not true for
	///  {@code NaN}. Moreover, zero entries will lose their sign.
	///  Some operations (that involve {@code NaN} and/or infinities) may
	///  thus give incorrect results, like multiplications, divisions or
	///  functions mapping.
	/// </p>
	/// @version $Id: OpenMapRealVector.java 1570254 2014-02-20 16:16:19Z luc $
	/// @since 2.0
	/// </summary>
	[Serializable]
	public class OpenMapRealVector : SparseRealVector
	{
		/// <summary>
		/// Default Tolerance for having a value considered zero. </summary>
		public const double DEFAULT_ZERO_TOLERANCE = 1.0e-12;
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 8772222695580707260L;
		/// <summary>
		/// Entries of the vector. </summary>
		private readonly OpenIntToDoubleHashMap entries;
		/// <summary>
		/// Dimension of the vector. </summary>
		private readonly int virtualSize;
		/// <summary>
		/// Tolerance for having a value considered zero. </summary>
		private readonly double epsilon;

		/// <summary>
		/// Build a 0-length vector.
		/// Zero-length vectors may be used to initialized construction of vectors
		/// by data gathering. We start with zero-length and use either the {@link
		/// #OpenMapRealVector(OpenMapRealVector, int)} constructor
		/// or one of the {@code append} method (<seealso cref="#append(double)"/>,
		/// <seealso cref="#append(RealVector)"/>) to gather data into this vector.
		/// </summary>
		public OpenMapRealVector() : this(0, DEFAULT_ZERO_TOLERANCE)
		{
		}

		/// <summary>
		/// Construct a vector of zeroes.
		/// </summary>
		/// <param name="dimension"> Size of the vector. </param>
		public OpenMapRealVector(int dimension) : this(dimension, DEFAULT_ZERO_TOLERANCE)
		{
		}

		/// <summary>
		/// Construct a vector of zeroes, specifying zero tolerance.
		/// </summary>
		/// <param name="dimension"> Size of the vector. </param>
		/// <param name="epsilon"> Tolerance below which a value considered zero. </param>
		public OpenMapRealVector(int dimension, double epsilon)
		{
			virtualSize = dimension;
			entries = new OpenIntToDoubleHashMap(0.0);
			this.epsilon = epsilon;
		}

		/// <summary>
		/// Build a resized vector, for use with append.
		/// </summary>
		/// <param name="v"> Original vector. </param>
		/// <param name="resize"> Amount to add. </param>
		protected internal OpenMapRealVector(OpenMapRealVector v, int resize)
		{
			virtualSize = v.Dimension + resize;
			entries = new OpenIntToDoubleHashMap(v.entries);
			epsilon = v.epsilon;
		}

		/// <summary>
		/// Build a vector with known the sparseness (for advanced use only).
		/// </summary>
		/// <param name="dimension"> Size of the vector. </param>
		/// <param name="expectedSize"> The expected number of non-zero entries. </param>
		public OpenMapRealVector(int dimension, int expectedSize) : this(dimension, expectedSize, DEFAULT_ZERO_TOLERANCE)
		{
		}

		/// <summary>
		/// Build a vector with known the sparseness and zero tolerance
		/// setting (for advanced use only).
		/// </summary>
		/// <param name="dimension"> Size of the vector. </param>
		/// <param name="expectedSize"> Expected number of non-zero entries. </param>
		/// <param name="epsilon"> Tolerance below which a value is considered zero. </param>
		public OpenMapRealVector(int dimension, int expectedSize, double epsilon)
		{
			virtualSize = dimension;
			entries = new OpenIntToDoubleHashMap(expectedSize, 0.0);
			this.epsilon = epsilon;
		}

		/// <summary>
		/// Create from an array.
		/// Only non-zero entries will be stored.
		/// </summary>
		/// <param name="values"> Set of values to create from. </param>
		public OpenMapRealVector(double[] values) : this(values, DEFAULT_ZERO_TOLERANCE)
		{
		}

		/// <summary>
		/// Create from an array, specifying zero tolerance.
		/// Only non-zero entries will be stored.
		/// </summary>
		/// <param name="values"> Set of values to create from. </param>
		/// <param name="epsilon"> Tolerance below which a value is considered zero. </param>
		public OpenMapRealVector(double[] values, double epsilon)
		{
			virtualSize = values.Length;
			entries = new OpenIntToDoubleHashMap(0.0);
			this.epsilon = epsilon;
			for (int key = 0; key < values.Length; key++)
			{
				double value = values[key];
				if (!isDefaultValue(value))
				{
					entries.put(key, value);
				}
			}
		}

		/// <summary>
		/// Create from an array.
		/// Only non-zero entries will be stored.
		/// </summary>
		/// <param name="values"> The set of values to create from </param>
		public OpenMapRealVector(double?[] values) : this(values, DEFAULT_ZERO_TOLERANCE)
		{
		}

		/// <summary>
		/// Create from an array.
		/// Only non-zero entries will be stored.
		/// </summary>
		/// <param name="values"> Set of values to create from. </param>
		/// <param name="epsilon"> Tolerance below which a value is considered zero. </param>
		public OpenMapRealVector(double?[] values, double epsilon)
		{
			virtualSize = values.Length;
			entries = new OpenIntToDoubleHashMap(0.0);
			this.epsilon = epsilon;
			for (int key = 0; key < values.Length; key++)
			{
				double value = (double)values[key];
				if (!isDefaultValue(value))
				{
					entries.put(key, value);
				}
			}
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="v"> Instance to copy from. </param>
		public OpenMapRealVector(OpenMapRealVector v)
		{
			virtualSize = v.Dimension;
			entries = new OpenIntToDoubleHashMap(v.Entries);
			epsilon = v.epsilon;
		}

		/// <summary>
		/// Generic copy constructor.
		/// </summary>
		/// <param name="v"> Instance to copy from. </param>
		public OpenMapRealVector(RealVector v)
		{
			virtualSize = v.Dimension;
			entries = new OpenIntToDoubleHashMap(0.0);
			epsilon = DEFAULT_ZERO_TOLERANCE;
			for (int key = 0; key < virtualSize; key++)
			{
				double value = v.getEntry(key);
				if (!isDefaultValue(value))
				{
					entries.put(key, value);
				}
			}
		}

		/// <summary>
		/// Get the entries of this instance.
		/// </summary>
		/// <returns> the entries of this instance. </returns>
		private OpenIntToDoubleHashMap Entries
		{
			get
			{
				return entries;
			}
		}

		/// <summary>
		/// Determine if this value is within epsilon of zero.
		/// </summary>
		/// <param name="value"> Value to test </param>
		/// <returns> {@code true} if this value is within epsilon to zero,
		/// {@code false} otherwise.
		/// @since 2.1 </returns>
		protected internal virtual bool isDefaultValue(double value)
		{
			return FastMath.abs(value) < epsilon;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector add(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public override RealVector add(RealVector v)
		{
			checkVectorDimensions(v.Dimension);
			if (v is OpenMapRealVector)
			{
				return add((OpenMapRealVector) v);
			}
			else
			{
				return base.add(v);
			}
		}

		/// <summary>
		/// Optimized method to add two OpenMapRealVectors.
		/// It copies the larger vector, then iterates over the smaller.
		/// </summary>
		/// <param name="v"> Vector to add. </param>
		/// <returns> the sum of {@code this} and {@code v}. </returns>
		/// <exception cref="DimensionMismatchException"> if the dimensions do not match. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public OpenMapRealVector add(OpenMapRealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual OpenMapRealVector add(OpenMapRealVector v)
		{
			checkVectorDimensions(v.Dimension);
			bool copyThis = entries.size() > v.entries.size();
			OpenMapRealVector res = copyThis ? this.copy() : v.copy();
			OpenIntToDoubleHashMap.Iterator iter = copyThis ? v.entries.GetEnumerator() : entries.GetEnumerator();
			OpenIntToDoubleHashMap randomAccess = copyThis ? entries : v.entries;
			while (iter.hasNext())
			{
				iter.advance();
				int key = iter.key();
				if (randomAccess.containsKey(key))
				{
					res.setEntry(key, randomAccess.get(key) + iter.value());
				}
				else
				{
					res.setEntry(key, iter.value());
				}
			}
			return res;
		}

		/// <summary>
		/// Optimized method to append a OpenMapRealVector. </summary>
		/// <param name="v"> vector to append </param>
		/// <returns> The result of appending {@code v} to self </returns>
		public virtual OpenMapRealVector append(OpenMapRealVector v)
		{
			OpenMapRealVector res = new OpenMapRealVector(this, v.Dimension);
			OpenIntToDoubleHashMap.Iterator iter = v.entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				res.setEntry(iter.key() + virtualSize, iter.value());
			}
			return res;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override OpenMapRealVector append(RealVector v)
		{
			if (v is OpenMapRealVector)
			{
				return append((OpenMapRealVector) v);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final OpenMapRealVector res = new OpenMapRealVector(this, v.getDimension());
				OpenMapRealVector res = new OpenMapRealVector(this, v.Dimension);
				for (int i = 0; i < v.Dimension; i++)
				{
					res.setEntry(i + virtualSize, v.getEntry(i));
				}
				return res;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override OpenMapRealVector append(double d)
		{
			OpenMapRealVector res = new OpenMapRealVector(this, 1);
			res.setEntry(virtualSize, d);
			return res;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 2.1
		/// </summary>
		public override OpenMapRealVector copy()
		{
			return new OpenMapRealVector(this);
		}

		/// <summary>
		/// Computes the dot product.
		/// Note that the computation is now performed in the parent class: no
		/// performance improvement is to be expected from this overloaded
		/// method.
		/// The previous implementation was buggy and cannot be easily fixed
		/// (see MATH-795).
		/// </summary>
		/// <param name="v"> Vector. </param>
		/// <returns> the dot product of this vector with {@code v}. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this} vector.
		/// </exception>
		/// @deprecated as of 3.1 (to be removed in 4.0). The computation is
		/// performed by the parent class. The method must be kept to maintain
		/// backwards compatibility. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.1 (to be removed in 4.0). The computation is") public double dotProduct(OpenMapRealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		[Obsolete("as of 3.1 (to be removed in 4.0). The computation is")]
		public virtual double dotProduct(OpenMapRealVector v)
		{
			return dotProduct((RealVector) v);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public OpenMapRealVector ebeDivide(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public override OpenMapRealVector ebeDivide(RealVector v)
		{
			checkVectorDimensions(v.Dimension);
			OpenMapRealVector res = new OpenMapRealVector(this);
			/*
			 * MATH-803: it is not sufficient to loop through non zero entries of
			 * this only. Indeed, if this[i] = 0d and v[i] = 0d, then
			 * this[i] / v[i] = NaN, and not 0d.
			 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = getDimension();
			int n = Dimension;
			for (int i = 0; i < n; i++)
			{
				res.setEntry(i, this.getEntry(i) / v.getEntry(i));
			}
			return res;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public OpenMapRealVector ebeMultiply(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public override OpenMapRealVector ebeMultiply(RealVector v)
		{
			checkVectorDimensions(v.Dimension);
			OpenMapRealVector res = new OpenMapRealVector(this);
			OpenIntToDoubleHashMap.Iterator iter = entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				res.setEntry(iter.key(), iter.value() * v.getEntry(iter.key()));
			}
			return res;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public OpenMapRealVector getSubVector(int index, int n) throws org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.OutOfRangeException
		public override OpenMapRealVector getSubVector(int index, int n)
		{
			checkIndex(index);
			if (n < 0)
			{
				throw new NotPositiveException(LocalizedFormats.NUMBER_OF_ELEMENTS_SHOULD_BE_POSITIVE, n);
			}
			checkIndex(index + n - 1);
			OpenMapRealVector res = new OpenMapRealVector(n);
			int end = index + n;
			OpenIntToDoubleHashMap.Iterator iter = entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				int key = iter.key();
				if (key >= index && key < end)
				{
					res.setEntry(key - index, iter.value());
				}
			}
			return res;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int Dimension
		{
			get
			{
				return virtualSize;
			}
		}

		/// <summary>
		/// Optimized method to compute distance.
		/// </summary>
		/// <param name="v"> Vector to compute distance to. </param>
		/// <returns> the distance from {@code this} and {@code v}. </returns>
		/// <exception cref="DimensionMismatchException"> if the dimensions do not match. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getDistance(OpenMapRealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual double getDistance(OpenMapRealVector v)
		{
			checkVectorDimensions(v.Dimension);
			OpenIntToDoubleHashMap.Iterator iter = entries.GetEnumerator();
			double res = 0;
			while (iter.hasNext())
			{
				iter.advance();
				int key = iter.key();
				double delta;
				delta = iter.value() - v.getEntry(key);
				res += delta * delta;
			}
			iter = v.Entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				int key = iter.key();
				if (!entries.containsKey(key))
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double value = iter.value();
					double value = iter.value();
					res += value * value;
				}
			}
			return FastMath.sqrt(res);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double getDistance(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public override double getDistance(RealVector v)
		{
			checkVectorDimensions(v.Dimension);
			if (v is OpenMapRealVector)
			{
				return getDistance((OpenMapRealVector) v);
			}
			else
			{
				return base.getDistance(v);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double getEntry(int index) throws org.apache.commons.math3.exception.OutOfRangeException
		public override double getEntry(int index)
		{
			checkIndex(index);
			return entries.get(index);
		}

		/// <summary>
		/// Distance between two vectors.
		/// This method computes the distance consistent with
		/// L<sub>1</sub> norm, i.e. the sum of the absolute values of
		/// elements differences.
		/// </summary>
		/// <param name="v"> Vector to which distance is requested. </param>
		/// <returns> distance between this vector and {@code v}. </returns>
		/// <exception cref="DimensionMismatchException"> if the dimensions do not match. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getL1Distance(OpenMapRealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual double getL1Distance(OpenMapRealVector v)
		{
			checkVectorDimensions(v.Dimension);
			double max = 0;
			OpenIntToDoubleHashMap.Iterator iter = entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				double delta = FastMath.abs(iter.value() - v.getEntry(iter.key()));
				max += delta;
			}
			iter = v.Entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				int key = iter.key();
				if (!entries.containsKey(key))
				{
					double delta = FastMath.abs(iter.value());
					max += FastMath.abs(delta);
				}
			}
			return max;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double getL1Distance(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public override double getL1Distance(RealVector v)
		{
			checkVectorDimensions(v.Dimension);
			if (v is OpenMapRealVector)
			{
				return getL1Distance((OpenMapRealVector) v);
			}
			else
			{
				return base.getL1Distance(v);
			}
		}

		/// <summary>
		/// Optimized method to compute LInfDistance.
		/// </summary>
		/// <param name="v"> Vector to compute distance from. </param>
		/// <returns> the LInfDistance. </returns>
		/// <exception cref="DimensionMismatchException"> if the dimensions do not match. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private double getLInfDistance(OpenMapRealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		private double getLInfDistance(OpenMapRealVector v)
		{
			checkVectorDimensions(v.Dimension);
			double max = 0;
			OpenIntToDoubleHashMap.Iterator iter = entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				double delta = FastMath.abs(iter.value() - v.getEntry(iter.key()));
				if (delta > max)
				{
					max = delta;
				}
			}
			iter = v.Entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				int key = iter.key();
				if (!entries.containsKey(key) && iter.value() > max)
				{
					max = iter.value();
				}
			}
			return max;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double getLInfDistance(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public override double getLInfDistance(RealVector v)
		{
			checkVectorDimensions(v.Dimension);
			if (v is OpenMapRealVector)
			{
				return getLInfDistance((OpenMapRealVector) v);
			}
			else
			{
				return base.getLInfDistance(v);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool Infinite
		{
			get
			{
				bool infiniteFound = false;
				OpenIntToDoubleHashMap.Iterator iter = entries.GetEnumerator();
				while (iter.hasNext())
				{
					iter.advance();
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double value = iter.value();
					double value = iter.value();
					if (double.IsNaN(value))
					{
						return false;
					}
					if (double.IsInfinity(value))
					{
						infiniteFound = true;
					}
				}
				return infiniteFound;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool NaN
		{
			get
			{
				OpenIntToDoubleHashMap.Iterator iter = entries.GetEnumerator();
				while (iter.hasNext())
				{
					iter.advance();
					if (double.IsNaN(iter.value()))
					{
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override OpenMapRealVector mapAdd(double d)
		{
			return copy().mapAddToSelf(d);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override OpenMapRealVector mapAddToSelf(double d)
		{
			for (int i = 0; i < virtualSize; i++)
			{
				setEntry(i, getEntry(i) + d);
			}
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setEntry(int index, double value) throws org.apache.commons.math3.exception.OutOfRangeException
		public override void setEntry(int index, double value)
		{
			checkIndex(index);
			if (!isDefaultValue(value))
			{
				entries.put(index, value);
			}
			else if (entries.containsKey(index))
			{
				entries.remove(index);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setSubVector(int index, RealVector v) throws org.apache.commons.math3.exception.OutOfRangeException
		public override void setSubVector(int index, RealVector v)
		{
			checkIndex(index);
			checkIndex(index + v.Dimension - 1);
			for (int i = 0; i < v.Dimension; i++)
			{
				setEntry(i + index, v.getEntry(i));
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override void set(double value)
		{
			for (int i = 0; i < virtualSize; i++)
			{
				setEntry(i, value);
			}
		}

		/// <summary>
		/// Optimized method to subtract OpenMapRealVectors.
		/// </summary>
		/// <param name="v"> Vector to subtract from {@code this}. </param>
		/// <returns> the difference of {@code this} and {@code v}. </returns>
		/// <exception cref="DimensionMismatchException"> if the dimensions do not match. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public OpenMapRealVector subtract(OpenMapRealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual OpenMapRealVector subtract(OpenMapRealVector v)
		{
			checkVectorDimensions(v.Dimension);
			OpenMapRealVector res = copy();
			OpenIntToDoubleHashMap.Iterator iter = v.Entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				int key = iter.key();
				if (entries.containsKey(key))
				{
					res.setEntry(key, entries.get(key) - iter.value());
				}
				else
				{
					res.setEntry(key, -iter.value());
				}
			}
			return res;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector subtract(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public override RealVector subtract(RealVector v)
		{
			checkVectorDimensions(v.Dimension);
			if (v is OpenMapRealVector)
			{
				return subtract((OpenMapRealVector) v);
			}
			else
			{
				return base.subtract(v);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public OpenMapRealVector unitVector() throws org.apache.commons.math3.exception.MathArithmeticException
		public override OpenMapRealVector unitVector()
		{
			OpenMapRealVector res = copy();
			res.unitize();
			return res;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void unitize() throws org.apache.commons.math3.exception.MathArithmeticException
		public override void unitize()
		{
			double norm = Norm;
			if (isDefaultValue(norm))
			{
				throw new MathArithmeticException(LocalizedFormats.ZERO_NORM);
			}
			OpenIntToDoubleHashMap.Iterator iter = entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				entries.put(iter.key(), iter.value() / norm);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double[] toArray()
		{
			double[] res = new double[virtualSize];
			OpenIntToDoubleHashMap.Iterator iter = entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				res[iter.key()] = iter.value();
			}
			return res;
		}

		/// <summary>
		/// {@inheritDoc}
		/// Implementation Note: This works on exact values, and as a result
		/// it is possible for {@code a.subtract(b)} to be the zero vector, while
		/// {@code a.hashCode() != b.hashCode()}.
		/// </summary>
		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			long temp;
			temp = double.doubleToLongBits(epsilon);
			result = prime * result + (int)(temp ^ ((long)((ulong)temp >> 32)));
			result = prime * result + virtualSize;
			OpenIntToDoubleHashMap.Iterator iter = entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				temp = double.doubleToLongBits(iter.value());
				result = prime * result + (int)(temp ^ (temp >> 32));
			}
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// Implementation Note: This performs an exact comparison, and as a result
		/// it is possible for {@code a.subtract(b}} to be the zero vector, while
		/// {@code  a.equals(b) == false}.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is OpenMapRealVector))
			{
				return false;
			}
			OpenMapRealVector other = (OpenMapRealVector) obj;
			if (virtualSize != other.virtualSize)
			{
				return false;
			}
			if (double.doubleToLongBits(epsilon) != double.doubleToLongBits(other.epsilon))
			{
				return false;
			}
			OpenIntToDoubleHashMap.Iterator iter = entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				double test = other.getEntry(iter.key());
				if (double.doubleToLongBits(test) != double.doubleToLongBits(iter.value()))
				{
					return false;
				}
			}
			iter = other.Entries.GetEnumerator();
			while (iter.hasNext())
			{
				iter.advance();
				double test = iter.value();
				if (double.doubleToLongBits(test) != double.doubleToLongBits(getEntry(iter.key())))
				{
					return false;
				}
			}
			return true;
		}

		/// 
		/// <returns> the percentage of none zero elements as a decimal percent.
		/// @since 2.2 </returns>
		public virtual double Sparsity
		{
			get
			{
				return (double)entries.size() / (double)Dimension;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override IEnumerator<Entry> sparseIterator()
		{
			return new OpenMapSparseIterator(this);
		}

		/// <summary>
		/// Implementation of {@code Entry} optimized for OpenMap.
		/// This implementation does not allow arbitrary calls to {@code setIndex}
		/// since the order in which entries are returned is undefined.
		/// </summary>
		protected internal class OpenMapEntry : Entry
		{
			private readonly OpenMapRealVector outerInstance;

			/// <summary>
			/// Iterator pointing to the entry. </summary>
			internal readonly OpenIntToDoubleHashMap.Iterator iter;

			/// <summary>
			/// Build an entry from an iterator point to an element.
			/// </summary>
			/// <param name="iter"> Iterator pointing to the entry. </param>
			protected internal OpenMapEntry(OpenMapRealVector outerInstance, OpenIntToDoubleHashMap.Iterator iter) : base(outerInstance)
			{
				this.outerInstance = outerInstance;
				this.iter = iter;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override double Value
			{
				get
				{
					return iter.value();
				}
				set
				{
					outerInstance.entries.put(iter.key(), value);
				}
			}


			/// <summary>
			/// {@inheritDoc} </summary>
			public override int Index
			{
				get
				{
					return iter.key();
				}
			}

		}

		/// <summary>
		/// Iterator class to do iteration over just the non-zero elements.
		/// This implementation is fail-fast, so cannot be used to modify
		/// any zero element.
		/// </summary>
		protected internal class OpenMapSparseIterator : IEnumerator<Entry>
		{
			private readonly OpenMapRealVector outerInstance;

			/// <summary>
			/// Underlying iterator. </summary>
			internal readonly OpenIntToDoubleHashMap.Iterator iter;
			/// <summary>
			/// Current entry. </summary>
			internal readonly Entry current;

			/// <summary>
			/// Simple constructor. </summary>
			protected internal OpenMapSparseIterator(OpenMapRealVector outerInstance)
			{
				this.outerInstance = outerInstance;
				iter = outerInstance.entries.GetEnumerator();
				current = new OpenMapEntry(outerInstance, iter);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual bool hasNext()
			{
				return iter.hasNext();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual Entry next()
			{
				iter.advance();
				return current;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual void remove()
			{
				throw new System.NotSupportedException("Not supported");
			}
		}
	}

}
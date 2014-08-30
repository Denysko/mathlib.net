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


	using MathUnsupportedOperationException = org.apache.commons.math3.exception.MathUnsupportedOperationException;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using MathArithmeticException = org.apache.commons.math3.exception.MathArithmeticException;
	using FunctionUtils = org.apache.commons.math3.analysis.FunctionUtils;
	using Add = org.apache.commons.math3.analysis.function.Add;
	using Multiply = org.apache.commons.math3.analysis.function.Multiply;
	using Divide = org.apache.commons.math3.analysis.function.Divide;
	using UnivariateFunction = org.apache.commons.math3.analysis.UnivariateFunction;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Class defining a real-valued vector with basic algebraic operations.
	/// <p>
	/// vector element indexing is 0-based -- e.g., {@code getEntry(0)}
	/// returns the first element of the vector.
	/// </p>
	/// <p>
	/// The {@code code map} and {@code mapToSelf} methods operate
	/// on vectors element-wise, i.e. they perform the same operation (adding a scalar,
	/// applying a function ...) on each element in turn. The {@code map}
	/// versions create a new vector to hold the result and do not change the instance.
	/// The {@code mapToSelf} version uses the instance itself to store the
	/// results, so the instance is changed by this method. In all cases, the result
	/// vector is returned by the methods, allowing the <i>fluent API</i>
	/// style, like this:
	/// </p>
	/// <pre>
	///   RealVector result = v.mapAddToSelf(3.4).mapToSelf(new Tan()).mapToSelf(new Power(2.3));
	/// </pre>
	/// 
	/// @version $Id: RealVector.java 1570510 2014-02-21 10:16:52Z luc $
	/// @since 2.1
	/// </summary>
	public abstract class RealVector
	{
		/// <summary>
		/// Returns the size of the vector.
		/// </summary>
		/// <returns> the size of this vector. </returns>
		public abstract int Dimension {get;}

		/// <summary>
		/// Return the entry at the specified index.
		/// </summary>
		/// <param name="index"> Index location of entry to be fetched. </param>
		/// <returns> the vector entry at {@code index}. </returns>
		/// <exception cref="OutOfRangeException"> if the index is not valid. </exception>
		/// <seealso cref= #setEntry(int, double) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract double getEntry(int index) throws org.apache.commons.math3.exception.OutOfRangeException;
		public abstract double getEntry(int index);

		/// <summary>
		/// Set a single element.
		/// </summary>
		/// <param name="index"> element index. </param>
		/// <param name="value"> new value for the element. </param>
		/// <exception cref="OutOfRangeException"> if the index is not valid. </exception>
		/// <seealso cref= #getEntry(int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void setEntry(int index, double value) throws org.apache.commons.math3.exception.OutOfRangeException;
		public abstract void setEntry(int index, double value);

		/// <summary>
		/// Change an entry at the specified index.
		/// </summary>
		/// <param name="index"> Index location of entry to be set. </param>
		/// <param name="increment"> Value to add to the vector entry. </param>
		/// <exception cref="OutOfRangeException"> if the index is not valid.
		/// @since 3.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addToEntry(int index, double increment) throws org.apache.commons.math3.exception.OutOfRangeException
		public virtual void addToEntry(int index, double increment)
		{
			setEntry(index, getEntry(index) + increment);
		}

		/// <summary>
		/// Construct a new vector by appending a vector to this vector.
		/// </summary>
		/// <param name="v"> vector to append to this one. </param>
		/// <returns> a new vector. </returns>
		public abstract RealVector append(RealVector v);

		/// <summary>
		/// Construct a new vector by appending a double to this vector.
		/// </summary>
		/// <param name="d"> double to append. </param>
		/// <returns> a new vector. </returns>
		public abstract RealVector append(double d);

		/// <summary>
		/// Get a subvector from consecutive elements.
		/// </summary>
		/// <param name="index"> index of first element. </param>
		/// <param name="n"> number of elements to be retrieved. </param>
		/// <returns> a vector containing n elements. </returns>
		/// <exception cref="OutOfRangeException"> if the index is not valid. </exception>
		/// <exception cref="NotPositiveException"> if the number of elements is not positive. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract RealVector getSubVector(int index, int n) throws org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.OutOfRangeException;
		public abstract RealVector getSubVector(int index, int n);

		/// <summary>
		/// Set a sequence of consecutive elements.
		/// </summary>
		/// <param name="index"> index of first element to be set. </param>
		/// <param name="v"> vector containing the values to set. </param>
		/// <exception cref="OutOfRangeException"> if the index is not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void setSubVector(int index, RealVector v) throws org.apache.commons.math3.exception.OutOfRangeException;
		public abstract void setSubVector(int index, RealVector v);

		/// <summary>
		/// Check whether any coordinate of this vector is {@code NaN}.
		/// </summary>
		/// <returns> {@code true} if any coordinate of this vector is {@code NaN},
		/// {@code false} otherwise. </returns>
		public abstract bool NaN {get;}

		/// <summary>
		/// Check whether any coordinate of this vector is infinite and none are {@code NaN}.
		/// </summary>
		/// <returns> {@code true} if any coordinate of this vector is infinite and
		/// none are {@code NaN}, {@code false} otherwise. </returns>
		public abstract bool Infinite {get;}

		/// <summary>
		/// Check if instance and specified vectors have the same dimension.
		/// </summary>
		/// <param name="v"> Vector to compare instance with. </param>
		/// <exception cref="DimensionMismatchException"> if the vectors do not
		/// have the same dimension. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkVectorDimensions(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		protected internal virtual void checkVectorDimensions(RealVector v)
		{
			checkVectorDimensions(v.Dimension);
		}

		/// <summary>
		/// Check if instance dimension is equal to some expected value.
		/// </summary>
		/// <param name="n"> Expected dimension. </param>
		/// <exception cref="DimensionMismatchException"> if the dimension is
		/// inconsistent with the vector size. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkVectorDimensions(int n) throws org.apache.commons.math3.exception.DimensionMismatchException
		protected internal virtual void checkVectorDimensions(int n)
		{
			int d = Dimension;
			if (d != n)
			{
				throw new DimensionMismatchException(d, n);
			}
		}

		/// <summary>
		/// Check if an index is valid.
		/// </summary>
		/// <param name="index"> Index to check. </param>
		/// <exception cref="OutOfRangeException"> if {@code index} is not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkIndex(final int index) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual void checkIndex(int index)
		{
			if (index < 0 || index >= Dimension)
			{
				throw new OutOfRangeException(LocalizedFormats.INDEX, index, 0, Dimension - 1);
			}
		}

		/// <summary>
		/// Checks that the indices of a subvector are valid.
		/// </summary>
		/// <param name="start"> the index of the first entry of the subvector </param>
		/// <param name="end"> the index of the last entry of the subvector (inclusive) </param>
		/// <exception cref="OutOfRangeException"> if {@code start} of {@code end} are not valid </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code end < start}
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkIndices(final int start, final int end) throws org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual void checkIndices(int start, int end)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = getDimension();
			int dim = Dimension;
			if ((start < 0) || (start >= dim))
			{
				throw new OutOfRangeException(LocalizedFormats.INDEX, start, 0, dim - 1);
			}
			if ((end < 0) || (end >= dim))
			{
				throw new OutOfRangeException(LocalizedFormats.INDEX, end, 0, dim - 1);
			}
			if (end < start)
			{
				// TODO Use more specific error message
				throw new NumberIsTooSmallException(LocalizedFormats.INITIAL_ROW_AFTER_FINAL_ROW, end, start, false);
			}
		}

		/// <summary>
		/// Compute the sum of this vector and {@code v}.
		/// Returns a new vector. Does not change instance data.
		/// </summary>
		/// <param name="v"> Vector to be added. </param>
		/// <returns> {@code this} + {@code v}. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this} vector. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector add(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual RealVector add(RealVector v)
		{
			checkVectorDimensions(v);
			RealVector result = v.copy();
			IEnumerator<Entry> it = iterator();
			while (it.MoveNext())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Entry e = it.Current;
				Entry e = it.Current;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = e.getIndex();
				int index = e.Index;
				result.setEntry(index, e.Value + result.getEntry(index));
			}
			return result;
		}

		/// <summary>
		/// Subtract {@code v} from this vector.
		/// Returns a new vector. Does not change instance data.
		/// </summary>
		/// <param name="v"> Vector to be subtracted. </param>
		/// <returns> {@code this} - {@code v}. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this} vector. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector subtract(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual RealVector subtract(RealVector v)
		{
			checkVectorDimensions(v);
			RealVector result = v.mapMultiply(-1d);
			IEnumerator<Entry> it = iterator();
			while (it.MoveNext())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Entry e = it.Current;
				Entry e = it.Current;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = e.getIndex();
				int index = e.Index;
				result.setEntry(index, e.Value + result.getEntry(index));
			}
			return result;
		}

		/// <summary>
		/// Add a value to each entry.
		/// Returns a new vector. Does not change instance data.
		/// </summary>
		/// <param name="d"> Value to be added to each entry. </param>
		/// <returns> {@code this} + {@code d}. </returns>
		public virtual RealVector mapAdd(double d)
		{
			return copy().mapAddToSelf(d);
		}

		/// <summary>
		/// Add a value to each entry.
		/// The instance is changed in-place.
		/// </summary>
		/// <param name="d"> Value to be added to each entry. </param>
		/// <returns> {@code this}. </returns>
		public virtual RealVector mapAddToSelf(double d)
		{
			if (d != 0)
			{
				return mapToSelf(FunctionUtils.fix2ndArgument(new Add(), d));
			}
			return this;
		}

		/// <summary>
		/// Returns a (deep) copy of this vector.
		/// </summary>
		/// <returns> a vector copy. </returns>
		public abstract RealVector copy();

		/// <summary>
		/// Compute the dot product of this vector with {@code v}.
		/// </summary>
		/// <param name="v"> Vector with which dot product should be computed </param>
		/// <returns> the scalar dot product between this instance and {@code v}. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this} vector. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double dotProduct(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual double dotProduct(RealVector v)
		{
			checkVectorDimensions(v);
			double d = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = getDimension();
			int n = Dimension;
			for (int i = 0; i < n; i++)
			{
				d += getEntry(i) * v.getEntry(i);
			}
			return d;
		}

		/// <summary>
		/// Computes the cosine of the angle between this vector and the
		/// argument.
		/// </summary>
		/// <param name="v"> Vector. </param>
		/// <returns> the cosine of the angle between this vector and {@code v}. </returns>
		/// <exception cref="MathArithmeticException"> if {@code this} or {@code v} is the null
		/// vector </exception>
		/// <exception cref="DimensionMismatchException"> if the dimensions of {@code this} and
		/// {@code v} do not match </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double cosine(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathArithmeticException
		public virtual double cosine(RealVector v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double norm = getNorm();
			double norm = Norm;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double vNorm = v.getNorm();
			double vNorm = v.Norm;

			if (norm == 0 || vNorm == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.ZERO_NORM);
			}
			return dotProduct(v) / (norm * vNorm);
		}

		/// <summary>
		/// Element-by-element division.
		/// </summary>
		/// <param name="v"> Vector by which instance elements must be divided. </param>
		/// <returns> a vector containing this[i] / v[i] for all i. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this} vector. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract RealVector ebeDivide(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException;
		public abstract RealVector ebeDivide(RealVector v);

		/// <summary>
		/// Element-by-element multiplication.
		/// </summary>
		/// <param name="v"> Vector by which instance elements must be multiplied </param>
		/// <returns> a vector containing this[i] * v[i] for all i. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this} vector. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract RealVector ebeMultiply(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException;
		public abstract RealVector ebeMultiply(RealVector v);

		/// <summary>
		/// Distance between two vectors.
		/// <p>This method computes the distance consistent with the
		/// L<sub>2</sub> norm, i.e. the square root of the sum of
		/// element differences, or Euclidean distance.</p>
		/// </summary>
		/// <param name="v"> Vector to which distance is requested. </param>
		/// <returns> the distance between two vectors. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this} vector. </exception>
		/// <seealso cref= #getL1Distance(RealVector) </seealso>
		/// <seealso cref= #getLInfDistance(RealVector) </seealso>
		/// <seealso cref= #getNorm() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getDistance(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual double getDistance(RealVector v)
		{
			checkVectorDimensions(v);
			double d = 0;
			IEnumerator<Entry> it = iterator();
			while (it.MoveNext())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Entry e = it.Current;
				Entry e = it.Current;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double diff = e.getValue() - v.getEntry(e.getIndex());
				double diff = e.Value - v.getEntry(e.Index);
				d += diff * diff;
			}
			return FastMath.sqrt(d);
		}

		/// <summary>
		/// Returns the L<sub>2</sub> norm of the vector.
		/// <p>The L<sub>2</sub> norm is the root of the sum of
		/// the squared elements.</p>
		/// </summary>
		/// <returns> the norm. </returns>
		/// <seealso cref= #getL1Norm() </seealso>
		/// <seealso cref= #getLInfNorm() </seealso>
		/// <seealso cref= #getDistance(RealVector) </seealso>
		public virtual double Norm
		{
			get
			{
				double sum = 0;
				IEnumerator<Entry> it = iterator();
				while (it.MoveNext())
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Entry e = it.Current;
					Entry e = it.Current;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double value = e.getValue();
					double value = e.Value;
					sum += value * value;
				}
				return FastMath.sqrt(sum);
			}
		}

		/// <summary>
		/// Returns the L<sub>1</sub> norm of the vector.
		/// <p>The L<sub>1</sub> norm is the sum of the absolute
		/// values of the elements.</p>
		/// </summary>
		/// <returns> the norm. </returns>
		/// <seealso cref= #getNorm() </seealso>
		/// <seealso cref= #getLInfNorm() </seealso>
		/// <seealso cref= #getL1Distance(RealVector) </seealso>
		public virtual double L1Norm
		{
			get
			{
				double norm = 0;
				IEnumerator<Entry> it = iterator();
				while (it.MoveNext())
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Entry e = it.Current;
					Entry e = it.Current;
					norm += FastMath.abs(e.Value);
				}
				return norm;
			}
		}

		/// <summary>
		/// Returns the L<sub>&infin;</sub> norm of the vector.
		/// <p>The L<sub>&infin;</sub> norm is the max of the absolute
		/// values of the elements.</p>
		/// </summary>
		/// <returns> the norm. </returns>
		/// <seealso cref= #getNorm() </seealso>
		/// <seealso cref= #getL1Norm() </seealso>
		/// <seealso cref= #getLInfDistance(RealVector) </seealso>
		public virtual double LInfNorm
		{
			get
			{
				double norm = 0;
				IEnumerator<Entry> it = iterator();
				while (it.MoveNext())
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Entry e = it.Current;
					Entry e = it.Current;
					norm = FastMath.max(norm, FastMath.abs(e.Value));
				}
				return norm;
			}
		}

		/// <summary>
		/// Distance between two vectors.
		/// <p>This method computes the distance consistent with
		/// L<sub>1</sub> norm, i.e. the sum of the absolute values of
		/// the elements differences.</p>
		/// </summary>
		/// <param name="v"> Vector to which distance is requested. </param>
		/// <returns> the distance between two vectors. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this} vector. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getL1Distance(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual double getL1Distance(RealVector v)
		{
			checkVectorDimensions(v);
			double d = 0;
			IEnumerator<Entry> it = iterator();
			while (it.MoveNext())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Entry e = it.Current;
				Entry e = it.Current;
				d += FastMath.abs(e.Value - v.getEntry(e.Index));
			}
			return d;
		}

		/// <summary>
		/// Distance between two vectors.
		/// <p>This method computes the distance consistent with
		/// L<sub>&infin;</sub> norm, i.e. the max of the absolute values of
		/// element differences.</p>
		/// </summary>
		/// <param name="v"> Vector to which distance is requested. </param>
		/// <returns> the distance between two vectors. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this} vector. </exception>
		/// <seealso cref= #getDistance(RealVector) </seealso>
		/// <seealso cref= #getL1Distance(RealVector) </seealso>
		/// <seealso cref= #getLInfNorm() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getLInfDistance(RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual double getLInfDistance(RealVector v)
		{
			checkVectorDimensions(v);
			double d = 0;
			IEnumerator<Entry> it = iterator();
			while (it.MoveNext())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Entry e = it.Current;
				Entry e = it.Current;
				d = FastMath.max(FastMath.abs(e.Value - v.getEntry(e.Index)), d);
			}
			return d;
		}

		/// <summary>
		/// Get the index of the minimum entry.
		/// </summary>
		/// <returns> the index of the minimum entry or -1 if vector length is 0
		/// or all entries are {@code NaN}. </returns>
		public virtual int MinIndex
		{
			get
			{
				int minIndex = -1;
				double minValue = double.PositiveInfinity;
				IEnumerator<Entry> iterator = iterator();
				while (iterator.MoveNext())
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Entry entry = iterator.Current;
					Entry entry = iterator.Current;
					if (entry.Value <= minValue)
					{
						minIndex = entry.Index;
						minValue = entry.Value;
					}
				}
				return minIndex;
			}
		}

		/// <summary>
		/// Get the value of the minimum entry.
		/// </summary>
		/// <returns> the value of the minimum entry or {@code NaN} if all
		/// entries are {@code NaN}. </returns>
		public virtual double MinValue
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int minIndex = getMinIndex();
				int minIndex = MinIndex;
				return minIndex < 0 ? double.NaN : getEntry(minIndex);
			}
		}

		/// <summary>
		/// Get the index of the maximum entry.
		/// </summary>
		/// <returns> the index of the maximum entry or -1 if vector length is 0
		/// or all entries are {@code NaN} </returns>
		public virtual int MaxIndex
		{
			get
			{
				int maxIndex = -1;
				double maxValue = double.NegativeInfinity;
				IEnumerator<Entry> iterator = iterator();
				while (iterator.MoveNext())
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Entry entry = iterator.Current;
					Entry entry = iterator.Current;
					if (entry.Value >= maxValue)
					{
						maxIndex = entry.Index;
						maxValue = entry.Value;
					}
				}
				return maxIndex;
			}
		}

		/// <summary>
		/// Get the value of the maximum entry.
		/// </summary>
		/// <returns> the value of the maximum entry or {@code NaN} if all
		/// entries are {@code NaN}. </returns>
		public virtual double MaxValue
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int maxIndex = getMaxIndex();
				int maxIndex = MaxIndex;
				return maxIndex < 0 ? double.NaN : getEntry(maxIndex);
			}
		}


		/// <summary>
		/// Multiply each entry by the argument. Returns a new vector.
		/// Does not change instance data.
		/// </summary>
		/// <param name="d"> Multiplication factor. </param>
		/// <returns> {@code this} * {@code d}. </returns>
		public virtual RealVector mapMultiply(double d)
		{
			return copy().mapMultiplyToSelf(d);
		}

		/// <summary>
		/// Multiply each entry.
		/// The instance is changed in-place.
		/// </summary>
		/// <param name="d"> Multiplication factor. </param>
		/// <returns> {@code this}. </returns>
		public virtual RealVector mapMultiplyToSelf(double d)
		{
			return mapToSelf(FunctionUtils.fix2ndArgument(new Multiply(), d));
		}

		/// <summary>
		/// Subtract a value from each entry. Returns a new vector.
		/// Does not change instance data.
		/// </summary>
		/// <param name="d"> Value to be subtracted. </param>
		/// <returns> {@code this} - {@code d}. </returns>
		public virtual RealVector mapSubtract(double d)
		{
			return copy().mapSubtractToSelf(d);
		}

		/// <summary>
		/// Subtract a value from each entry.
		/// The instance is changed in-place.
		/// </summary>
		/// <param name="d"> Value to be subtracted. </param>
		/// <returns> {@code this}. </returns>
		public virtual RealVector mapSubtractToSelf(double d)
		{
			return mapAddToSelf(-d);
		}

		/// <summary>
		/// Divide each entry by the argument. Returns a new vector.
		/// Does not change instance data.
		/// </summary>
		/// <param name="d"> Value to divide by. </param>
		/// <returns> {@code this} / {@code d}. </returns>
		public virtual RealVector mapDivide(double d)
		{
			return copy().mapDivideToSelf(d);
		}

		/// <summary>
		/// Divide each entry by the argument.
		/// The instance is changed in-place.
		/// </summary>
		/// <param name="d"> Value to divide by. </param>
		/// <returns> {@code this}. </returns>
		public virtual RealVector mapDivideToSelf(double d)
		{
			return mapToSelf(FunctionUtils.fix2ndArgument(new Divide(), d));
		}

		/// <summary>
		/// Compute the outer product.
		/// </summary>
		/// <param name="v"> Vector with which outer product should be computed. </param>
		/// <returns> the matrix outer product between this instance and {@code v}. </returns>
		public virtual RealMatrix outerProduct(RealVector v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = this.getDimension();
			int m = this.Dimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = v.getDimension();
			int n = v.Dimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix product;
			RealMatrix product;
			if (v is SparseRealVector || this is SparseRealVector)
			{
				product = new OpenMapRealMatrix(m, n);
			}
			else
			{
				product = new Array2DRowRealMatrix(m, n);
			}
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					product.setEntry(i, j, this.getEntry(i) * v.getEntry(j));
				}
			}
			return product;
		}

		/// <summary>
		/// Find the orthogonal projection of this vector onto another vector.
		/// </summary>
		/// <param name="v"> vector onto which instance must be projected. </param>
		/// <returns> projection of the instance onto {@code v}. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code v} is not the same size as
		/// {@code this} vector. </exception>
		/// <exception cref="MathArithmeticException"> if {@code this} or {@code v} is the null
		/// vector </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector projection(final RealVector v) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual RealVector projection(RealVector v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double norm2 = v.dotProduct(v);
			double norm2 = v.dotProduct(v);
			if (norm2 == 0.0)
			{
				throw new MathArithmeticException(LocalizedFormats.ZERO_NORM);
			}
			return v.mapMultiply(dotProduct(v) / v.dotProduct(v));
		}

		/// <summary>
		/// Set all elements to a single value.
		/// </summary>
		/// <param name="value"> Single value to set for all elements. </param>
		public virtual void set(double value)
		{
			IEnumerator<Entry> it = iterator();
			while (it.MoveNext())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Entry e = it.Current;
				Entry e = it.Current;
				e.Value = value;
			}
		}

		/// <summary>
		/// Convert the vector to an array of {@code double}s.
		/// The array is independent from this vector data: the elements
		/// are copied.
		/// </summary>
		/// <returns> an array containing a copy of the vector elements. </returns>
		public virtual double[] toArray()
		{
			int dim = Dimension;
			double[] values = new double[dim];
			for (int i = 0; i < dim; i++)
			{
				values[i] = getEntry(i);
			}
			return values;
		}

		/// <summary>
		/// Creates a unit vector pointing in the direction of this vector.
		/// The instance is not changed by this method.
		/// </summary>
		/// <returns> a unit vector pointing in direction of this vector. </returns>
		/// <exception cref="MathArithmeticException"> if the norm is zero. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector unitVector() throws org.apache.commons.math3.exception.MathArithmeticException
		public virtual RealVector unitVector()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double norm = getNorm();
			double norm = Norm;
			if (norm == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.ZERO_NORM);
			}
			return mapDivide(norm);
		}

		/// <summary>
		/// Converts this vector into a unit vector.
		/// The instance itself is changed by this method.
		/// </summary>
		/// <exception cref="MathArithmeticException"> if the norm is zero. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void unitize() throws org.apache.commons.math3.exception.MathArithmeticException
		public virtual void unitize()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double norm = getNorm();
			double norm = Norm;
			if (norm == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.ZERO_NORM);
			}
			mapDivideToSelf(Norm);
		}

		/// <summary>
		/// Create a sparse iterator over the vector, which may omit some entries.
		/// The ommitted entries are either exact zeroes (for dense implementations)
		/// or are the entries which are not stored (for real sparse vectors).
		/// No guarantees are made about order of iteration.
		/// 
		/// <p>Note: derived classes are required to return an <seealso cref="Iterator"/> that
		/// returns non-null <seealso cref="Entry"/> objects as long as <seealso cref="Iterator#hasNext()"/>
		/// returns {@code true}.</p>
		/// </summary>
		/// <returns> a sparse iterator. </returns>
		public virtual IEnumerator<Entry> sparseIterator()
		{
			return new SparseEntryIterator(this);
		}

		/// <summary>
		/// Generic dense iterator. Iteration is in increasing order
		/// of the vector index.
		/// 
		/// <p>Note: derived classes are required to return an <seealso cref="Iterator"/> that
		/// returns non-null <seealso cref="Entry"/> objects as long as <seealso cref="Iterator#hasNext()"/>
		/// returns {@code true}.</p>
		/// </summary>
		/// <returns> a dense iterator. </returns>
		public virtual IEnumerator<Entry> iterator()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = getDimension();
			int dim = Dimension;
			return new IteratorAnonymousInnerClassHelper(this, dim);
		}

		private class IteratorAnonymousInnerClassHelper : IEnumerator<Entry>
		{
			private readonly RealVector outerInstance;

			private int dim;

			public IteratorAnonymousInnerClassHelper(RealVector outerInstance, int dim)
			{
				this.outerInstance = outerInstance;
				this.dim = dim;
				i = 0;
				e = new Entry(outerInstance);
			}


					/// <summary>
					/// Current index. </summary>
			private int i;

			/// <summary>
			/// Current entry. </summary>
			private Entry e;

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual bool hasNext()
			{
				return i < dim;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual Entry next()
			{
				if (i < dim)
				{
					e.Index = i++;
					return e;
				}
				else
				{
					throw new NoSuchElementException();
				}
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			/// <exception cref="MathUnsupportedOperationException"> in all circumstances. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void remove() throws org.apache.commons.math3.exception.MathUnsupportedOperationException
			public virtual void remove()
			{
				throw new MathUnsupportedOperationException();
			}
		}

		/// <summary>
		/// Acts as if implemented as:
		/// <pre>
		///  return copy().mapToSelf(function);
		/// </pre>
		/// Returns a new vector. Does not change instance data.
		/// </summary>
		/// <param name="function"> Function to apply to each entry. </param>
		/// <returns> a new vector. </returns>
		public virtual RealVector map(UnivariateFunction function)
		{
			return copy().mapToSelf(function);
		}

		/// <summary>
		/// Acts as if it is implemented as:
		/// <pre>
		///  Entry e = null;
		///  for(Iterator<Entry> it = iterator(); it.hasNext(); e = it.next()) {
		///      e.setValue(function.value(e.getValue()));
		///  }
		/// </pre>
		/// Entries of this vector are modified in-place by this method.
		/// </summary>
		/// <param name="function"> Function to apply to each entry. </param>
		/// <returns> a reference to this vector. </returns>
		public virtual RealVector mapToSelf(UnivariateFunction function)
		{
			IEnumerator<Entry> it = iterator();
			while (it.MoveNext())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Entry e = it.Current;
				Entry e = it.Current;
				e.Value = function.value(e.Value);
			}
			return this;
		}

		/// <summary>
		/// Returns a new vector representing {@code a * this + b * y}, the linear
		/// combination of {@code this} and {@code y}.
		/// Returns a new vector. Does not change instance data.
		/// </summary>
		/// <param name="a"> Coefficient of {@code this}. </param>
		/// <param name="b"> Coefficient of {@code y}. </param>
		/// <param name="y"> Vector with which {@code this} is linearly combined. </param>
		/// <returns> a vector containing {@code a * this[i] + b * y[i]} for all
		/// {@code i}. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code y} is not the same size as
		/// {@code this} vector. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector combine(double a, double b, RealVector y) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual RealVector combine(double a, double b, RealVector y)
		{
			return copy().combineToSelf(a, b, y);
		}

		/// <summary>
		/// Updates {@code this} with the linear combination of {@code this} and
		/// {@code y}.
		/// </summary>
		/// <param name="a"> Weight of {@code this}. </param>
		/// <param name="b"> Weight of {@code y}. </param>
		/// <param name="y"> Vector with which {@code this} is linearly combined. </param>
		/// <returns> {@code this}, with components equal to
		/// {@code a * this[i] + b * y[i]} for all {@code i}. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code y} is not the same size as
		/// {@code this} vector. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RealVector combineToSelf(double a, double b, RealVector y) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual RealVector combineToSelf(double a, double b, RealVector y)
		{
			checkVectorDimensions(y);
			for (int i = 0; i < Dimension; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xi = getEntry(i);
				double xi = getEntry(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yi = y.getEntry(i);
				double yi = y.getEntry(i);
				setEntry(i, a * xi + b * yi);
			}
			return this;
		}

		/// <summary>
		/// Visits (but does not alter) all entries of this vector in default order
		/// (increasing index).
		/// </summary>
		/// <param name="visitor"> the visitor to be used to process the entries of this
		/// vector </param>
		/// <returns> the value returned by <seealso cref="RealVectorPreservingVisitor#end()"/>
		/// at the end of the walk
		/// @since 3.1 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double walkInDefaultOrder(final RealVectorPreservingVisitor visitor)
		public virtual double walkInDefaultOrder(RealVectorPreservingVisitor visitor)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = getDimension();
			int dim = Dimension;
			visitor.start(dim, 0, dim - 1);
			for (int i = 0; i < dim; i++)
			{
				visitor.visit(i, getEntry(i));
			}
			return visitor.end();
		}

		/// <summary>
		/// Visits (but does not alter) some entries of this vector in default order
		/// (increasing index).
		/// </summary>
		/// <param name="visitor"> visitor to be used to process the entries of this vector </param>
		/// <param name="start"> the index of the first entry to be visited </param>
		/// <param name="end"> the index of the last entry to be visited (inclusive) </param>
		/// <returns> the value returned by <seealso cref="RealVectorPreservingVisitor#end()"/>
		/// at the end of the walk </returns>
		/// <exception cref="NumberIsTooSmallException"> if {@code end < start}. </exception>
		/// <exception cref="OutOfRangeException"> if the indices are not valid.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double walkInDefaultOrder(final RealVectorPreservingVisitor visitor, final int start, final int end) throws org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double walkInDefaultOrder(RealVectorPreservingVisitor visitor, int start, int end)
		{
			checkIndices(start, end);
			visitor.start(Dimension, start, end);
			for (int i = start; i <= end; i++)
			{
				visitor.visit(i, getEntry(i));
			}
			return visitor.end();
		}

		/// <summary>
		/// Visits (but does not alter) all entries of this vector in optimized
		/// order. The order in which the entries are visited is selected so as to
		/// lead to the most efficient implementation; it might depend on the
		/// concrete implementation of this abstract class.
		/// </summary>
		/// <param name="visitor"> the visitor to be used to process the entries of this
		/// vector </param>
		/// <returns> the value returned by <seealso cref="RealVectorPreservingVisitor#end()"/>
		/// at the end of the walk
		/// @since 3.1 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double walkInOptimizedOrder(final RealVectorPreservingVisitor visitor)
		public virtual double walkInOptimizedOrder(RealVectorPreservingVisitor visitor)
		{
			return walkInDefaultOrder(visitor);
		}

		/// <summary>
		/// Visits (but does not alter) some entries of this vector in optimized
		/// order. The order in which the entries are visited is selected so as to
		/// lead to the most efficient implementation; it might depend on the
		/// concrete implementation of this abstract class.
		/// </summary>
		/// <param name="visitor"> visitor to be used to process the entries of this vector </param>
		/// <param name="start"> the index of the first entry to be visited </param>
		/// <param name="end"> the index of the last entry to be visited (inclusive) </param>
		/// <returns> the value returned by <seealso cref="RealVectorPreservingVisitor#end()"/>
		/// at the end of the walk </returns>
		/// <exception cref="NumberIsTooSmallException"> if {@code end < start}. </exception>
		/// <exception cref="OutOfRangeException"> if the indices are not valid.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double walkInOptimizedOrder(final RealVectorPreservingVisitor visitor, final int start, final int end) throws org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double walkInOptimizedOrder(RealVectorPreservingVisitor visitor, int start, int end)
		{
			return walkInDefaultOrder(visitor, start, end);
		}

		/// <summary>
		/// Visits (and possibly alters) all entries of this vector in default order
		/// (increasing index).
		/// </summary>
		/// <param name="visitor"> the visitor to be used to process and modify the entries
		/// of this vector </param>
		/// <returns> the value returned by <seealso cref="RealVectorChangingVisitor#end()"/>
		/// at the end of the walk
		/// @since 3.1 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double walkInDefaultOrder(final RealVectorChangingVisitor visitor)
		public virtual double walkInDefaultOrder(RealVectorChangingVisitor visitor)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = getDimension();
			int dim = Dimension;
			visitor.start(dim, 0, dim - 1);
			for (int i = 0; i < dim; i++)
			{
				setEntry(i, visitor.visit(i, getEntry(i)));
			}
			return visitor.end();
		}

		/// <summary>
		/// Visits (and possibly alters) some entries of this vector in default order
		/// (increasing index).
		/// </summary>
		/// <param name="visitor"> visitor to be used to process the entries of this vector </param>
		/// <param name="start"> the index of the first entry to be visited </param>
		/// <param name="end"> the index of the last entry to be visited (inclusive) </param>
		/// <returns> the value returned by <seealso cref="RealVectorChangingVisitor#end()"/>
		/// at the end of the walk </returns>
		/// <exception cref="NumberIsTooSmallException"> if {@code end < start}. </exception>
		/// <exception cref="OutOfRangeException"> if the indices are not valid.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double walkInDefaultOrder(final RealVectorChangingVisitor visitor, final int start, final int end) throws org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double walkInDefaultOrder(RealVectorChangingVisitor visitor, int start, int end)
		{
			checkIndices(start, end);
			visitor.start(Dimension, start, end);
			for (int i = start; i <= end; i++)
			{
				setEntry(i, visitor.visit(i, getEntry(i)));
			}
			return visitor.end();
		}

		/// <summary>
		/// Visits (and possibly alters) all entries of this vector in optimized
		/// order. The order in which the entries are visited is selected so as to
		/// lead to the most efficient implementation; it might depend on the
		/// concrete implementation of this abstract class.
		/// </summary>
		/// <param name="visitor"> the visitor to be used to process the entries of this
		/// vector </param>
		/// <returns> the value returned by <seealso cref="RealVectorChangingVisitor#end()"/>
		/// at the end of the walk
		/// @since 3.1 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double walkInOptimizedOrder(final RealVectorChangingVisitor visitor)
		public virtual double walkInOptimizedOrder(RealVectorChangingVisitor visitor)
		{
			return walkInDefaultOrder(visitor);
		}

		/// <summary>
		/// Visits (and possibly change) some entries of this vector in optimized
		/// order. The order in which the entries are visited is selected so as to
		/// lead to the most efficient implementation; it might depend on the
		/// concrete implementation of this abstract class.
		/// </summary>
		/// <param name="visitor"> visitor to be used to process the entries of this vector </param>
		/// <param name="start"> the index of the first entry to be visited </param>
		/// <param name="end"> the index of the last entry to be visited (inclusive) </param>
		/// <returns> the value returned by <seealso cref="RealVectorChangingVisitor#end()"/>
		/// at the end of the walk </returns>
		/// <exception cref="NumberIsTooSmallException"> if {@code end < start}. </exception>
		/// <exception cref="OutOfRangeException"> if the indices are not valid.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double walkInOptimizedOrder(final RealVectorChangingVisitor visitor, final int start, final int end) throws org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double walkInOptimizedOrder(RealVectorChangingVisitor visitor, int start, int end)
		{
			return walkInDefaultOrder(visitor, start, end);
		}

		/// <summary>
		/// An entry in the vector. </summary>
		protected internal class Entry
		{
			private readonly RealVector outerInstance;

			/// <summary>
			/// Index of this entry. </summary>
			internal int index;

			/// <summary>
			/// Simple constructor. </summary>
			public Entry(RealVector outerInstance)
			{
				this.outerInstance = outerInstance;
				Index = 0;
			}

			/// <summary>
			/// Get the value of the entry.
			/// </summary>
			/// <returns> the value of the entry. </returns>
			public virtual double Value
			{
				get
				{
					return outerInstance.getEntry(Index);
				}
				set
				{
					outerInstance.setEntry(Index, value);
				}
			}


			/// <summary>
			/// Get the index of the entry.
			/// </summary>
			/// <returns> the index of the entry. </returns>
			public virtual int Index
			{
				get
				{
					return index;
				}
				set
				{
					this.index = value;
				}
			}

		}

		/// <summary>
		/// <p>
		/// Test for the equality of two real vectors. If all coordinates of two real
		/// vectors are exactly the same, and none are {@code NaN}, the two real
		/// vectors are considered to be equal. {@code NaN} coordinates are
		/// considered to affect globally the vector and be equals to each other -
		/// i.e, if either (or all) coordinates of the real vector are equal to
		/// {@code NaN}, the real vector is equal to a vector with all {@code NaN}
		/// coordinates.
		/// </p>
		/// <p>
		/// This method <em>must</em> be overriden by concrete subclasses of
		/// <seealso cref="RealVector"/> (the current implementation throws an exception).
		/// </p>
		/// </summary>
		/// <param name="other"> Object to test for equality. </param>
		/// <returns> {@code true} if two vector objects are equal, {@code false} if
		/// {@code other} is null, not an instance of {@code RealVector}, or
		/// not equal to this {@code RealVector} instance. </returns>
		/// <exception cref="MathUnsupportedOperationException"> if this method is not
		/// overridden. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public boolean equals(Object other) throws org.apache.commons.math3.exception.MathUnsupportedOperationException
		public override bool Equals(object other)
		{
			throw new MathUnsupportedOperationException();
		}

		/// <summary>
		/// {@inheritDoc}. This method <em>must</em> be overriden by concrete
		/// subclasses of <seealso cref="RealVector"/> (current implementation throws an
		/// exception).
		/// </summary>
		/// <exception cref="MathUnsupportedOperationException"> if this method is not
		/// overridden. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int hashCode() throws org.apache.commons.math3.exception.MathUnsupportedOperationException
		public override int GetHashCode()
		{
			throw new MathUnsupportedOperationException();
		}

		/// <summary>
		/// This class should rarely be used, but is here to provide
		/// a default implementation of sparseIterator(), which is implemented
		/// by walking over the entries, skipping those that are zero.
		/// 
		/// Concrete subclasses which are SparseVector implementations should
		/// make their own sparse iterator, rather than using this one.
		/// 
		/// This implementation might be useful for ArrayRealVector, when expensive
		/// operations which preserve the default value are to be done on the entries,
		/// and the fraction of non-default values is small (i.e. someone took a
		/// SparseVector, and passed it into the copy-constructor of ArrayRealVector)
		/// 
		/// </summary>
		protected internal class SparseEntryIterator : IEnumerator<Entry>
		{
			private readonly RealVector outerInstance;

			/// <summary>
			/// Dimension of the vector. </summary>
			internal readonly int dim;
			/// <summary>
			/// Last entry returned by <seealso cref="#next()"/>. </summary>
			internal Entry current;
			/// <summary>
			/// Next entry for <seealso cref="#next()"/> to return. </summary>
			internal Entry next_Renamed;

			/// <summary>
			/// Simple constructor. </summary>
			protected internal SparseEntryIterator(RealVector outerInstance)
			{
				this.outerInstance = outerInstance;
				dim = outerInstance.Dimension;
				current = new Entry(outerInstance);
				next_Renamed = new Entry(outerInstance);
				if (next_Renamed.Value == 0)
				{
					advance(next_Renamed);
				}
			}

			/// <summary>
			/// Advance an entry up to the next nonzero one.
			/// </summary>
			/// <param name="e"> entry to advance. </param>
			protected internal virtual void advance(Entry e)
			{
				if (e == null)
				{
					return;
				}
				do
				{
					e.Index = e.Index + 1;
				} while (e.Index < dim && e.Value == 0);
				if (e.Index >= dim)
				{
					e.Index = -1;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual bool hasNext()
			{
				return next_Renamed.Index >= 0;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual Entry next()
			{
				int index = next_Renamed.Index;
				if (index < 0)
				{
					throw new NoSuchElementException();
				}
				current.Index = index;
				advance(next_Renamed);
				return current;
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			/// <exception cref="MathUnsupportedOperationException"> in all circumstances. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void remove() throws org.apache.commons.math3.exception.MathUnsupportedOperationException
			public virtual void remove()
			{
				throw new MathUnsupportedOperationException();
			}
		}

		/// <summary>
		/// Returns an unmodifiable view of the specified vector.
		/// The returned vector has read-only access. An attempt to modify it will
		/// result in a <seealso cref="MathUnsupportedOperationException"/>. However, the
		/// returned vector is <em>not</em> immutable, since any modification of
		/// {@code v} will also change the returned view.
		/// For example, in the following piece of code
		/// <pre>
		///     RealVector v = new ArrayRealVector(2);
		///     RealVector w = RealVector.unmodifiableRealVector(v);
		///     v.setEntry(0, 1.2);
		///     v.setEntry(1, -3.4);
		/// </pre>
		/// the changes will be seen in the {@code w} view of {@code v}.
		/// </summary>
		/// <param name="v"> Vector for which an unmodifiable view is to be returned. </param>
		/// <returns> an unmodifiable view of {@code v}. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static RealVector unmodifiableRealVector(final RealVector v)
		public static RealVector unmodifiableRealVector(RealVector v)
		{
			/// <summary>
			/// This anonymous class is an implementation of <seealso cref="RealVector"/>
			/// with read-only access.
			/// It wraps any <seealso cref="RealVector"/>, and exposes all methods which
			/// do not modify it. Invoking methods which should normally result
			/// in the modification of the calling <seealso cref="RealVector"/> results in
			/// a <seealso cref="MathUnsupportedOperationException"/>. It should be noted
			/// that <seealso cref="UnmodifiableVector"/> is <em>not</em> immutable.
			/// </summary>
			return new RealVectorAnonymousInnerClassHelper(v);
		}

		private class RealVectorAnonymousInnerClassHelper : RealVector
		{
			private org.apache.commons.math3.linear.RealVector v;

			public RealVectorAnonymousInnerClassHelper(org.apache.commons.math3.linear.RealVector v)
			{
				this.v = v;
			}

					/// <summary>
					/// {@inheritDoc}
					/// </summary>
					/// <exception cref="MathUnsupportedOperationException"> in all circumstances. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector mapToSelf(org.apache.commons.math3.analysis.UnivariateFunction function) throws org.apache.commons.math3.exception.MathUnsupportedOperationException
			public override RealVector mapToSelf(UnivariateFunction function)
			{
				throw new MathUnsupportedOperationException();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override RealVector map(UnivariateFunction function)
			{
				return v.map(function);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override IEnumerator<Entry> iterator()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<Entry> i = v.iterator();
				IEnumerator<Entry> i = v.GetEnumerator();
				return new IteratorAnonymousInnerClassHelper(this, i);
			}

			private class IteratorAnonymousInnerClassHelper : IEnumerator<Entry>
			{
				private readonly RealVectorAnonymousInnerClassHelper outerInstance;

				private IEnumerator<Entry> i;

				public IteratorAnonymousInnerClassHelper(RealVectorAnonymousInnerClassHelper outerInstance, IEnumerator<Entry> i)
				{
					this.outerInstance = outerInstance;
					this.i = i;
					e = new UnmodifiableEntry();
				}

							/// <summary>
							/// The current entry. </summary>
				private readonly UnmodifiableEntry e;

				/// <summary>
				/// {@inheritDoc} </summary>
				public virtual bool hasNext()
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					return i.hasNext();
				}

				/// <summary>
				/// {@inheritDoc} </summary>
				public virtual Entry next()
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					e.Index = i.next().Index;
					return e;
				}

				/// <summary>
				/// {@inheritDoc}
				/// </summary>
				/// <exception cref="MathUnsupportedOperationException"> in all
				/// circumstances. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void remove() throws org.apache.commons.math3.exception.MathUnsupportedOperationException
				public virtual void remove()
				{
					throw new MathUnsupportedOperationException();
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override IEnumerator<Entry> sparseIterator()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<Entry> i = v.sparseIterator();
				IEnumerator<Entry> i = v.sparseIterator();

				return new IteratorAnonymousInnerClassHelper2(this, i);
			}

			private class IteratorAnonymousInnerClassHelper2 : IEnumerator<Entry>
			{
				private readonly RealVectorAnonymousInnerClassHelper outerInstance;

				private IEnumerator<Entry> i;

				public IteratorAnonymousInnerClassHelper2(RealVectorAnonymousInnerClassHelper outerInstance, IEnumerator<Entry> i)
				{
					this.outerInstance = outerInstance;
					this.i = i;
					e = new UnmodifiableEntry();
				}

							/// <summary>
							/// The current entry. </summary>
				private readonly UnmodifiableEntry e;

				/// <summary>
				/// {@inheritDoc} </summary>
				public virtual bool hasNext()
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					return i.hasNext();
				}

				/// <summary>
				/// {@inheritDoc} </summary>
				public virtual Entry next()
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					e.Index = i.next().Index;
					return e;
				}

				/// <summary>
				/// {@inheritDoc}
				/// </summary>
				/// <exception cref="MathUnsupportedOperationException"> in all
				/// circumstances. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void remove() throws org.apache.commons.math3.exception.MathUnsupportedOperationException
				public virtual void remove()
				{
					throw new MathUnsupportedOperationException();
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override RealVector copy()
			{
				return v.copy();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector add(RealVector w) throws org.apache.commons.math3.exception.DimensionMismatchException
			public override RealVector add(RealVector w)
			{
				return v.add(w);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector subtract(RealVector w) throws org.apache.commons.math3.exception.DimensionMismatchException
			public override RealVector subtract(RealVector w)
			{
				return v.subtract(w);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override RealVector mapAdd(double d)
			{
				return v.mapAdd(d);
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			/// <exception cref="MathUnsupportedOperationException"> in all
			/// circumstances. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector mapAddToSelf(double d) throws org.apache.commons.math3.exception.MathUnsupportedOperationException
			public override RealVector mapAddToSelf(double d)
			{
				throw new MathUnsupportedOperationException();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override RealVector mapSubtract(double d)
			{
				return v.mapSubtract(d);
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			/// <exception cref="MathUnsupportedOperationException"> in all
			/// circumstances. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector mapSubtractToSelf(double d) throws org.apache.commons.math3.exception.MathUnsupportedOperationException
			public override RealVector mapSubtractToSelf(double d)
			{
				throw new MathUnsupportedOperationException();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override RealVector mapMultiply(double d)
			{
				return v.mapMultiply(d);
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			/// <exception cref="MathUnsupportedOperationException"> in all
			/// circumstances. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector mapMultiplyToSelf(double d) throws org.apache.commons.math3.exception.MathUnsupportedOperationException
			public override RealVector mapMultiplyToSelf(double d)
			{
				throw new MathUnsupportedOperationException();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override RealVector mapDivide(double d)
			{
				return v.mapDivide(d);
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			/// <exception cref="MathUnsupportedOperationException"> in all
			/// circumstances. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector mapDivideToSelf(double d) throws org.apache.commons.math3.exception.MathUnsupportedOperationException
			public override RealVector mapDivideToSelf(double d)
			{
				throw new MathUnsupportedOperationException();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector ebeMultiply(RealVector w) throws org.apache.commons.math3.exception.DimensionMismatchException
			public override RealVector ebeMultiply(RealVector w)
			{
				return v.ebeMultiply(w);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector ebeDivide(RealVector w) throws org.apache.commons.math3.exception.DimensionMismatchException
			public override RealVector ebeDivide(RealVector w)
			{
				return v.ebeDivide(w);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double dotProduct(RealVector w) throws org.apache.commons.math3.exception.DimensionMismatchException
			public override double dotProduct(RealVector w)
			{
				return v.dotProduct(w);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double cosine(RealVector w) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathArithmeticException
			public override double cosine(RealVector w)
			{
				return v.cosine(w);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override double Norm
			{
				get
				{
					return v.Norm;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override double L1Norm
			{
				get
				{
					return v.L1Norm;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override double LInfNorm
			{
				get
				{
					return v.LInfNorm;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double getDistance(RealVector w) throws org.apache.commons.math3.exception.DimensionMismatchException
			public override double getDistance(RealVector w)
			{
				return v.getDistance(w);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double getL1Distance(RealVector w) throws org.apache.commons.math3.exception.DimensionMismatchException
			public override double getL1Distance(RealVector w)
			{
				return v.getL1Distance(w);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double getLInfDistance(RealVector w) throws org.apache.commons.math3.exception.DimensionMismatchException
			public override double getLInfDistance(RealVector w)
			{
				return v.getLInfDistance(w);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector unitVector() throws org.apache.commons.math3.exception.MathArithmeticException
			public override RealVector unitVector()
			{
				return v.unitVector();
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			/// <exception cref="MathUnsupportedOperationException"> in all
			/// circumstances. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void unitize() throws org.apache.commons.math3.exception.MathUnsupportedOperationException
			public override void unitize()
			{
				throw new MathUnsupportedOperationException();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override RealMatrix outerProduct(RealVector w)
			{
				return v.outerProduct(w);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double getEntry(int index) throws org.apache.commons.math3.exception.OutOfRangeException
			public override double getEntry(int index)
			{
				return v.getEntry(index);
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			/// <exception cref="MathUnsupportedOperationException"> in all
			/// circumstances. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setEntry(int index, double value) throws org.apache.commons.math3.exception.MathUnsupportedOperationException
			public override void setEntry(int index, double value)
			{
				throw new MathUnsupportedOperationException();
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			/// <exception cref="MathUnsupportedOperationException"> in all
			/// circumstances. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void addToEntry(int index, double value) throws org.apache.commons.math3.exception.MathUnsupportedOperationException
			public override void addToEntry(int index, double value)
			{
				throw new MathUnsupportedOperationException();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override int Dimension
			{
				get
				{
					return v.Dimension;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override RealVector append(RealVector w)
			{
				return v.append(w);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override RealVector append(double d)
			{
				return v.append(d);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector getSubVector(int index, int n) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NotPositiveException
			public override RealVector getSubVector(int index, int n)
			{
				return v.getSubVector(index, n);
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			/// <exception cref="MathUnsupportedOperationException"> in all
			/// circumstances. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setSubVector(int index, RealVector w) throws org.apache.commons.math3.exception.MathUnsupportedOperationException
			public override void setSubVector(int index, RealVector w)
			{
				throw new MathUnsupportedOperationException();
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			/// <exception cref="MathUnsupportedOperationException"> in all
			/// circumstances. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void set(double value) throws org.apache.commons.math3.exception.MathUnsupportedOperationException
			public override void set(double value)
			{
				throw new MathUnsupportedOperationException();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override double[] toArray()
			{
				return v.toArray();
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override bool NaN
			{
				get
				{
					return v.NaN;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override bool Infinite
			{
				get
				{
					return v.Infinite;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector combine(double a, double b, RealVector y) throws org.apache.commons.math3.exception.DimensionMismatchException
			public override RealVector combine(double a, double b, RealVector y)
			{
				return v.combine(a, b, y);
			}

			/// <summary>
			/// {@inheritDoc}
			/// </summary>
			/// <exception cref="MathUnsupportedOperationException"> in all
			/// circumstances. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector combineToSelf(double a, double b, RealVector y) throws org.apache.commons.math3.exception.MathUnsupportedOperationException
			public override RealVector combineToSelf(double a, double b, RealVector y)
			{
				throw new MathUnsupportedOperationException();
			}

			/// <summary>
			/// An entry in the vector. </summary>
			internal class UnmodifiableEntry : Entry
			{
				private readonly RealVector.RealVectorAnonymousInnerClassHelper outerInstance;

				public UnmodifiableEntry(RealVector.RealVectorAnonymousInnerClassHelper outerInstance) : base(outerInstance.outerInstance)
				{
					this.outerInstance = outerInstance;
				}

				/// <summary>
				/// {@inheritDoc} </summary>
				public override double Value
				{
					get
					{
						return outerInstance.v.getEntry(Index);
					}
					set
					{
						throw new MathUnsupportedOperationException();
					}
				}

				/// <summary>
				/// {@inheritDoc}
				/// </summary>
				/// <exception cref="MathUnsupportedOperationException"> in all
				/// circumstances. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setValue(double value) throws org.apache.commons.math3.exception.MathUnsupportedOperationException
			}
		}
	}

}
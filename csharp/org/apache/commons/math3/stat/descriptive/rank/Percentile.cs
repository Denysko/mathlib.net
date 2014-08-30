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
namespace org.apache.commons.math3.stat.descriptive.rank
{


	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// Provides percentile computation.
	/// <p>
	/// There are several commonly used methods for estimating percentiles (a.k.a.
	/// quantiles) based on sample data.  For large samples, the different methods
	/// agree closely, but when sample sizes are small, different methods will give
	/// significantly different results.  The algorithm implemented here works as follows:
	/// <ol>
	/// <li>Let <code>n</code> be the length of the (sorted) array and
	/// <code>0 < p <= 100</code> be the desired percentile.</li>
	/// <li>If <code> n = 1 </code> return the unique array element (regardless of
	/// the value of <code>p</code>); otherwise </li>
	/// <li>Compute the estimated percentile position
	/// <code> pos = p * (n + 1) / 100</code> and the difference, <code>d</code>
	/// between <code>pos</code> and <code>floor(pos)</code> (i.e. the fractional
	/// part of <code>pos</code>).</li>
	/// <li> If <code>pos < 1</code> return the smallest element in the array.</li>
	/// <li> Else if <code>pos >= n</code> return the largest element in the array.</li>
	/// <li> Else let <code>lower</code> be the element in position
	/// <code>floor(pos)</code> in the array and let <code>upper</code> be the
	/// next element in the array.  Return <code>lower + d * (upper - lower)</code>
	/// </li>
	/// </ol></p>
	/// <p>
	/// To compute percentiles, the data must be at least partially ordered.  Input
	/// arrays are copied and recursively partitioned using an ordering definition.
	/// The ordering used by <code>Arrays.sort(double[])</code> is the one determined
	/// by <seealso cref="java.lang.Double#compareTo(Double)"/>.  This ordering makes
	/// <code>Double.NaN</code> larger than any other value (including
	/// <code>Double.POSITIVE_INFINITY</code>).  Therefore, for example, the median
	/// (50th percentile) of
	/// <code>{0, 1, 2, 3, 4, Double.NaN}</code> evaluates to <code>2.5.</code></p>
	/// <p>
	/// Since percentile estimation usually involves interpolation between array
	/// elements, arrays containing  <code>NaN</code> or infinite values will often
	/// result in <code>NaN</code> or infinite values returned.</p>
	/// <p>
	/// Since 2.2, Percentile uses only selection instead of complete sorting
	/// and caches selection algorithm state between calls to the various
	/// {@code evaluate} methods. This greatly improves efficiency, both for a single
	/// percentile and multiple percentile computations. To maximize performance when
	/// multiple percentiles are computed based on the same data, users should set the
	/// data array once using either one of the <seealso cref="#evaluate(double[], double)"/> or
	/// <seealso cref="#setData(double[])"/> methods and thereafter <seealso cref="#evaluate(double)"/>
	/// with just the percentile provided.
	/// </p>
	/// <p>
	/// <strong>Note that this implementation is not synchronized.</strong> If
	/// multiple threads access an instance of this class concurrently, and at least
	/// one of the threads invokes the <code>increment()</code> or
	/// <code>clear()</code> method, it must be synchronized externally.</p>
	/// 
	/// @version $Id: Percentile.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class Percentile : AbstractUnivariateStatistic
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -8091216485095130416L;

		/// <summary>
		/// Minimum size under which we use a simple insertion sort rather than Hoare's select. </summary>
		private const int MIN_SELECT_SIZE = 15;

		/// <summary>
		/// Maximum number of partitioning pivots cached (each level double the number of pivots). </summary>
		private const int MAX_CACHED_LEVELS = 10;

		/// <summary>
		/// Determines what percentile is computed when evaluate() is activated
		/// with no quantile argument 
		/// </summary>
		private double quantile = 0.0;

		/// <summary>
		/// Cached pivots. </summary>
		private int[] cachedPivots;

		/// <summary>
		/// Constructs a Percentile with a default quantile
		/// value of 50.0.
		/// </summary>
		public Percentile() : this(50.0)
		{
			// No try-catch or advertised exception here - arg is valid
		}

		/// <summary>
		/// Constructs a Percentile with the specific quantile value. </summary>
		/// <param name="p"> the quantile </param>
		/// <exception cref="MathIllegalArgumentException">  if p is not greater than 0 and less
		/// than or equal to 100 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Percentile(final double p) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public Percentile(double p)
		{
			Quantile = p;
			cachedPivots = null;
		}

		/// <summary>
		/// Copy constructor, creates a new {@code Percentile} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code Percentile} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Percentile(Percentile original) throws org.apache.commons.math3.exception.NullArgumentException
		public Percentile(Percentile original)
		{
			copy(original, this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void setData(final double[] values)
		public override double[] Data
		{
			set
			{
				if (value == null)
				{
					cachedPivots = null;
				}
				else
				{
					cachedPivots = new int[(0x1 << MAX_CACHED_LEVELS) - 1];
					Arrays.fill(cachedPivots, -1);
				}
				base.Data = value;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setData(final double[] values, final int begin, final int length) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void setData(double[] values, int begin, int length)
		{
			if (values == null)
			{
				cachedPivots = null;
			}
			else
			{
				cachedPivots = new int[(0x1 << MAX_CACHED_LEVELS) - 1];
				Arrays.fill(cachedPivots, -1);
			}
			base.setData(values, begin, length);
		}

		/// <summary>
		/// Returns the result of evaluating the statistic over the stored data.
		/// <p>
		/// The stored array is the one which was set by previous calls to
		/// <seealso cref="#setData(double[])"/>
		/// </p> </summary>
		/// <param name="p"> the percentile value to compute </param>
		/// <returns> the value of the statistic applied to the stored data </returns>
		/// <exception cref="MathIllegalArgumentException"> if p is not a valid quantile value
		/// (p must be greater than 0 and less than or equal to 100) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double p) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double p)
		{
			return evaluate(DataRef, p);
		}

		/// <summary>
		/// Returns an estimate of the <code>p</code>th percentile of the values
		/// in the <code>values</code> array.
		/// <p>
		/// Calls to this method do not modify the internal <code>quantile</code>
		/// state of this statistic.</p>
		/// <p>
		/// <ul>
		/// <li>Returns <code>Double.NaN</code> if <code>values</code> has length
		/// <code>0</code></li>
		/// <li>Returns (for any value of <code>p</code>) <code>values[0]</code>
		///  if <code>values</code> has length <code>1</code></li>
		/// <li>Throws <code>MathIllegalArgumentException</code> if <code>values</code>
		/// is null or p is not a valid quantile value (p must be greater than 0
		/// and less than or equal to 100) </li>
		/// </ul></p>
		/// <p>
		/// See <seealso cref="Percentile"/> for a description of the percentile estimation
		/// algorithm used.</p>
		/// </summary>
		/// <param name="values"> input array of values </param>
		/// <param name="p"> the percentile value to compute </param>
		/// <returns> the percentile value or Double.NaN if the array is empty </returns>
		/// <exception cref="MathIllegalArgumentException"> if <code>values</code> is null
		///     or p is invalid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double p) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values, double p)
		{
			test(values, 0, 0);
			return evaluate(values, 0, values.Length, p);
		}

		/// <summary>
		/// Returns an estimate of the <code>quantile</code>th percentile of the
		/// designated values in the <code>values</code> array.  The quantile
		/// estimated is determined by the <code>quantile</code> property.
		/// <p>
		/// <ul>
		/// <li>Returns <code>Double.NaN</code> if <code>length = 0</code></li>
		/// <li>Returns (for any value of <code>quantile</code>)
		/// <code>values[begin]</code> if <code>length = 1 </code></li>
		/// <li>Throws <code>MathIllegalArgumentException</code> if <code>values</code>
		/// is null, or <code>start</code> or <code>length</code> is invalid</li>
		/// </ul></p>
		/// <p>
		/// See <seealso cref="Percentile"/> for a description of the percentile estimation
		/// algorithm used.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="start"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the percentile value </returns>
		/// <exception cref="MathIllegalArgumentException"> if the parameters are not valid
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values, final int start, final int length) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double[] values, int start, int length)
		{
			return evaluate(values, start, length, quantile);
		}

		 /// <summary>
		 /// Returns an estimate of the <code>p</code>th percentile of the values
		 /// in the <code>values</code> array, starting with the element in (0-based)
		 /// position <code>begin</code> in the array and including <code>length</code>
		 /// values.
		 /// <p>
		 /// Calls to this method do not modify the internal <code>quantile</code>
		 /// state of this statistic.</p>
		 /// <p>
		 /// <ul>
		 /// <li>Returns <code>Double.NaN</code> if <code>length = 0</code></li>
		 /// <li>Returns (for any value of <code>p</code>) <code>values[begin]</code>
		 ///  if <code>length = 1 </code></li>
		 /// <li>Throws <code>MathIllegalArgumentException</code> if <code>values</code>
		 ///  is null , <code>begin</code> or <code>length</code> is invalid, or
		 /// <code>p</code> is not a valid quantile value (p must be greater than 0
		 /// and less than or equal to 100)</li>
		 /// </ul></p>
		 /// <p>
		 /// See <seealso cref="Percentile"/> for a description of the percentile estimation
		 /// algorithm used.</p>
		 /// </summary>
		 /// <param name="values"> array of input values </param>
		 /// <param name="p">  the percentile to compute </param>
		 /// <param name="begin">  the first (0-based) element to include in the computation </param>
		 /// <param name="length">  the number of array elements to include </param>
		 /// <returns>  the percentile value </returns>
		 /// <exception cref="MathIllegalArgumentException"> if the parameters are not valid or the
		 /// input array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final int begin, final int length, final double p) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values, int begin, int length, double p)
		{

			test(values, begin, length);

			if ((p > 100) || (p <= 0))
			{
				throw new OutOfRangeException(LocalizedFormats.OUT_OF_BOUNDS_QUANTILE_VALUE, p, 0, 100);
			}
			if (length == 0)
			{
				return double.NaN;
			}
			if (length == 1)
			{
				return values[begin]; // always return single value for n = 1
			}
			double n = length;
			double pos = p * (n + 1) / 100;
			double fpos = FastMath.floor(pos);
			int intPos = (int) fpos;
			double dif = pos - fpos;
			double[] work;
			int[] pivotsHeap;
			if (values == DataRef)
			{
				work = DataRef;
				pivotsHeap = cachedPivots;
			}
			else
			{
				work = new double[length];
				Array.Copy(values, begin, work, 0, length);
				pivotsHeap = new int[(0x1 << MAX_CACHED_LEVELS) - 1];
				Arrays.fill(pivotsHeap, -1);
			}

			if (pos < 1)
			{
				return select(work, pivotsHeap, 0);
			}
			if (pos >= n)
			{
				return select(work, pivotsHeap, length - 1);
			}
			double lower = select(work, pivotsHeap, intPos - 1);
			double upper = select(work, pivotsHeap, intPos);
			return lower + dif * (upper - lower);
		}

		/// <summary>
		/// Select the k<sup>th</sup> smallest element from work array </summary>
		/// <param name="work"> work array (will be reorganized during the call) </param>
		/// <param name="pivotsHeap"> set of pivot index corresponding to elements that
		/// are already at their sorted location, stored as an implicit heap
		/// (i.e. a sorted binary tree stored in a flat array, where the
		/// children of a node at index n are at indices 2n+1 for the left
		/// child and 2n+2 for the right child, with 0-based indices) </param>
		/// <param name="k"> index of the desired element </param>
		/// <returns> k<sup>th</sup> smallest element </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double select(final double[] work, final int[] pivotsHeap, final int k)
		private double select(double[] work, int[] pivotsHeap, int k)
		{

			int begin = 0;
			int end = work.Length;
			int node = 0;

			while (end - begin > MIN_SELECT_SIZE)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pivot;
				int pivot;
				if ((node < pivotsHeap.Length) && (pivotsHeap[node] >= 0))
				{
					// the pivot has already been found in a previous call
					// and the array has already been partitioned around it
					pivot = pivotsHeap[node];
				}
				else
				{
					// select a pivot and partition work array around it
					pivot = partition(work, begin, end, medianOf3(work, begin, end));
					if (node < pivotsHeap.Length)
					{
						pivotsHeap[node] = pivot;
					}
				}

				if (k == pivot)
				{
					// the pivot was exactly the element we wanted
					return work[k];
				}
				else if (k < pivot)
				{
					// the element is in the left partition
					end = pivot;
					node = FastMath.min(2 * node + 1, pivotsHeap.Length); // the min is here to avoid integer overflow
				}
				else
				{
					// the element is in the right partition
					begin = pivot + 1;
					node = FastMath.min(2 * node + 2, pivotsHeap.Length); // the min is here to avoid integer overflow
				}

			}

			// the element is somewhere in the small sub-array
			// sort the sub-array using insertion sort
			insertionSort(work, begin, end);
			return work[k];

		}

		/// <summary>
		/// Select a pivot index as the median of three </summary>
		/// <param name="work"> data array </param>
		/// <param name="begin"> index of the first element of the slice </param>
		/// <param name="end"> index after the last element of the slice </param>
		/// <returns> the index of the median element chosen between the
		/// first, the middle and the last element of the array slice </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: int medianOf3(final double[] work, final int begin, final int end)
		internal virtual int medianOf3(double[] work, int begin, int end)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int inclusiveEnd = end - 1;
			int inclusiveEnd = end - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int middle = begin + (inclusiveEnd - begin) / 2;
			int middle = begin + (inclusiveEnd - begin) / 2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double wBegin = work[begin];
			double wBegin = work[begin];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double wMiddle = work[middle];
			double wMiddle = work[middle];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double wEnd = work[inclusiveEnd];
			double wEnd = work[inclusiveEnd];

			if (wBegin < wMiddle)
			{
				if (wMiddle < wEnd)
				{
					return middle;
				}
				else
				{
					return (wBegin < wEnd) ? inclusiveEnd : begin;
				}
			}
			else
			{
				if (wBegin < wEnd)
				{
					return begin;
				}
				else
				{
					return (wMiddle < wEnd) ? inclusiveEnd : middle;
				}
			}

		}

		/// <summary>
		/// Partition an array slice around a pivot
		/// <p>
		/// Partitioning exchanges array elements such that all elements
		/// smaller than pivot are before it and all elements larger than
		/// pivot are after it
		/// </p> </summary>
		/// <param name="work"> data array </param>
		/// <param name="begin"> index of the first element of the slice </param>
		/// <param name="end"> index after the last element of the slice </param>
		/// <param name="pivot"> initial index of the pivot </param>
		/// <returns> index of the pivot after partition </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private int partition(final double[] work, final int begin, final int end, final int pivot)
		private int partition(double[] work, int begin, int end, int pivot)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double value = work[pivot];
			double value = work[pivot];
			work[pivot] = work[begin];

			int i = begin + 1;
			int j = end - 1;
			while (i < j)
			{
				while ((i < j) && (work[j] > value))
				{
					--j;
				}
				while ((i < j) && (work[i] < value))
				{
					++i;
				}

				if (i < j)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tmp = work[i];
					double tmp = work[i];
					work[i++] = work[j];
					work[j--] = tmp;
				}
			}

			if ((i >= end) || (work[i] > value))
			{
				--i;
			}
			work[begin] = work[i];
			work[i] = value;
			return i;

		}

		/// <summary>
		/// Sort in place a (small) array slice using insertion sort </summary>
		/// <param name="work"> array to sort </param>
		/// <param name="begin"> index of the first element of the slice to sort </param>
		/// <param name="end"> index after the last element of the slice to sort </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void insertionSort(final double[] work, final int begin, final int end)
		private void insertionSort(double[] work, int begin, int end)
		{
			for (int j = begin + 1; j < end; j++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double saved = work[j];
				double saved = work[j];
				int i = j - 1;
				while ((i >= begin) && (saved < work[i]))
				{
					work[i + 1] = work[i];
					i--;
				}
				work[i + 1] = saved;
			}
		}

		/// <summary>
		/// Returns the value of the quantile field (determines what percentile is
		/// computed when evaluate() is called with no quantile argument).
		/// </summary>
		/// <returns> quantile </returns>
		public virtual double Quantile
		{
			get
			{
				return quantile;
			}
			set
			{
				if (value <= 0 || value > 100)
				{
					throw new OutOfRangeException(LocalizedFormats.OUT_OF_BOUNDS_QUANTILE_VALUE, value, 0, 100);
				}
				quantile = value;
			}
		}

		/// <summary>
		/// Sets the value of the quantile field (determines what percentile is
		/// computed when evaluate() is called with no quantile argument).
		/// </summary>
		/// <param name="p"> a value between 0 < p <= 100 </param>
		/// <exception cref="MathIllegalArgumentException">  if p is not greater than 0 and less
		/// than or equal to 100 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setQuantile(final double p) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override Percentile copy()
		{
			Percentile result = new Percentile();
			//No try-catch or advertised exception because args are guaranteed non-null
			copy(this, result);
			return result;
		}

		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> Percentile to copy </param>
		/// <param name="dest"> Percentile to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(Percentile source, Percentile dest) throws org.apache.commons.math3.exception.NullArgumentException
		public static void copy(Percentile source, Percentile dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			dest.Data = source.DataRef;
			if (source.cachedPivots != null)
			{
				Array.Copy(source.cachedPivots, 0, dest.cachedPivots, 0, source.cachedPivots.Length);
			}
			dest.quantile = source.quantile;
		}

	}

}
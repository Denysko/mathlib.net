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
namespace org.apache.commons.math3.stat.descriptive
{

	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using MathUtils = org.apache.commons.math3.util.MathUtils;
	using Precision = org.apache.commons.math3.util.Precision;

	/// 
	/// <summary>
	/// Abstract implementation of the <seealso cref="StorelessUnivariateStatistic"/> interface.
	/// <p>
	/// Provides default <code>evaluate()</code> and <code>incrementAll(double[])</code>
	/// implementations.</p>
	/// <p>
	/// <strong>Note that these implementations are not synchronized.</strong></p>
	/// 
	/// @version $Id: AbstractStorelessUnivariateStatistic.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public abstract class AbstractStorelessUnivariateStatistic : AbstractUnivariateStatistic, StorelessUnivariateStatistic
	{
		public abstract long N {get;}

		/// <summary>
		/// This default implementation calls <seealso cref="#clear"/>, then invokes
		/// <seealso cref="#increment"/> in a loop over the the input array, and then uses
		/// <seealso cref="#getResult"/> to compute the return value.
		/// <p>
		/// Note that this implementation changes the internal state of the
		/// statistic.  Its side effects are the same as invoking <seealso cref="#clear"/> and
		/// then <seealso cref="#incrementAll(double[])"/>.</p>
		/// <p>
		/// Implementations may override this method with a more efficient and
		/// possibly more accurate implementation that works directly with the
		/// input array.</p>
		/// <p>
		/// If the array is null, a MathIllegalArgumentException is thrown.</p> </summary>
		/// <param name="values"> input array </param>
		/// <returns> the value of the statistic applied to the input array </returns>
		/// <exception cref="MathIllegalArgumentException"> if values is null </exception>
		/// <seealso cref= org.apache.commons.math3.stat.descriptive.UnivariateStatistic#evaluate(double[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double[] values)
		{
			if (values == null)
			{
				throw new NullArgumentException(LocalizedFormats.INPUT_ARRAY);
			}
			return evaluate(values, 0, values.Length);
		}

		/// <summary>
		/// This default implementation calls <seealso cref="#clear"/>, then invokes
		/// <seealso cref="#increment"/> in a loop over the specified portion of the input
		/// array, and then uses <seealso cref="#getResult"/> to compute the return value.
		/// <p>
		/// Note that this implementation changes the internal state of the
		/// statistic.  Its side effects are the same as invoking <seealso cref="#clear"/> and
		/// then <seealso cref="#incrementAll(double[], int, int)"/>.</p>
		/// <p>
		/// Implementations may override this method with a more efficient and
		/// possibly more accurate implementation that works directly with the
		/// input array.</p>
		/// <p>
		/// If the array is null or the index parameters are not valid, an
		/// MathIllegalArgumentException is thrown.</p> </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> the index of the first element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the value of the statistic applied to the included array entries </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the indices are not valid </exception>
		/// <seealso cref= org.apache.commons.math3.stat.descriptive.UnivariateStatistic#evaluate(double[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values, final int begin, final int length) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double[] values, int begin, int length)
		{
			if (test(values, begin, length))
			{
				clear();
				incrementAll(values, begin, length);
			}
			return Result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override abstract StorelessUnivariateStatistic copy();

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public abstract void clear();

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public abstract double Result {get;}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public abstract void increment(final double d);
		public abstract void increment(double d);

		/// <summary>
		/// This default implementation just calls <seealso cref="#increment"/> in a loop over
		/// the input array.
		/// <p>
		/// Throws IllegalArgumentException if the input values array is null.</p>
		/// </summary>
		/// <param name="values"> values to add </param>
		/// <exception cref="MathIllegalArgumentException"> if values is null </exception>
		/// <seealso cref= org.apache.commons.math3.stat.descriptive.StorelessUnivariateStatistic#incrementAll(double[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void incrementAll(double[] values) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		public virtual void incrementAll(double[] values)
		{
			if (values == null)
			{
				throw new NullArgumentException(LocalizedFormats.INPUT_ARRAY);
			}
			incrementAll(values, 0, values.Length);
		}

		/// <summary>
		/// This default implementation just calls <seealso cref="#increment"/> in a loop over
		/// the specified portion of the input array.
		/// <p>
		/// Throws IllegalArgumentException if the input values array is null.</p>
		/// </summary>
		/// <param name="values">  array holding values to add </param>
		/// <param name="begin">   index of the first array element to add </param>
		/// <param name="length">  number of array elements to add </param>
		/// <exception cref="MathIllegalArgumentException"> if values is null </exception>
		/// <seealso cref= org.apache.commons.math3.stat.descriptive.StorelessUnivariateStatistic#incrementAll(double[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void incrementAll(double[] values, int begin, int length) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		public virtual void incrementAll(double[] values, int begin, int length)
		{
			if (test(values, begin, length))
			{
				int k = begin + length;
				for (int i = begin; i < k; i++)
				{
					increment(values[i]);
				}
			}
		}

		/// <summary>
		/// Returns true iff <code>object</code> is an
		/// <code>AbstractStorelessUnivariateStatistic</code> returning the same
		/// values as this for <code>getResult()</code> and <code>getN()</code> </summary>
		/// <param name="object"> object to test equality against. </param>
		/// <returns> true if object returns the same value as this </returns>
		public override bool Equals(object @object)
		{
			if (@object == this)
			{
				return true;
			}
		   if (@object is AbstractStorelessUnivariateStatistic == false)
		   {
				return false;
		   }
			AbstractStorelessUnivariateStatistic stat = (AbstractStorelessUnivariateStatistic) @object;
			return Precision.equalsIncludingNaN(stat.Result, this.Result) && Precision.equalsIncludingNaN(stat.N, this.N);
		}

		/// <summary>
		/// Returns hash code based on getResult() and getN()
		/// </summary>
		/// <returns> hash code </returns>
		public override int GetHashCode()
		{
			return 31 * (31 + MathUtils.hash(Result)) + MathUtils.hash(N);
		}

	}

}
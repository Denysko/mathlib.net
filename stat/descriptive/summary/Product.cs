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
namespace mathlib.stat.descriptive.summary
{

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using FastMath = mathlib.util.FastMath;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// Returns the product of the available values.
	/// <p>
	/// If there are no values in the dataset, then 1 is returned.
	///  If any of the values are
	/// <code>NaN</code>, then <code>NaN</code> is returned.</p>
	/// <p>
	/// <strong>Note that this implementation is not synchronized.</strong> If
	/// multiple threads access an instance of this class concurrently, and at least
	/// one of the threads invokes the <code>increment()</code> or
	/// <code>clear()</code> method, it must be synchronized externally.</p>
	/// 
	/// @version $Id: Product.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class Product : AbstractStorelessUnivariateStatistic, WeightedEvaluation
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 2824226005990582538L;

		/// <summary>
		///The number of values that have been added </summary>
		private long n;

		/// <summary>
		/// The current Running Product.
		/// </summary>
		private double value;

		/// <summary>
		/// Create a Product instance
		/// </summary>
		public Product()
		{
			n = 0;
			value = 1;
		}

		/// <summary>
		/// Copy constructor, creates a new {@code Product} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code Product} instance to copy </param>
		/// <exception cref="NullArgumentException">  if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Product(Product original) throws mathlib.exception.NullArgumentException
		public Product(Product original)
		{
			copy(original, this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void increment(final double d)
		public override void increment(double d)
		{
			value *= d;
			n++;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double Result
		{
			get
			{
				return value;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override long N
		{
			get
			{
				return n;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override void clear()
		{
			value = 1;
			n = 0;
		}

		/// <summary>
		/// Returns the product of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.
		/// <p>
		/// Throws <code>MathIllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the product of the values or 1 if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the array index
		///  parameters are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double evaluate(double[] values, int begin, int length)
		{
			double product = double.NaN;
			if (test(values, begin, length, true))
			{
				product = 1.0;
				for (int i = begin; i < begin + length; i++)
				{
					product *= values[i];
				}
			}
			return product;
		}

		/// <summary>
		/// <p>Returns the weighted product of the entries in the specified portion of
		/// the input array, or <code>Double.NaN</code> if the designated subarray
		/// is empty.</p>
		/// 
		/// <p>Throws <code>MathIllegalArgumentException</code> if any of the following are true:
		/// <ul><li>the values array is null</li>
		///     <li>the weights array is null</li>
		///     <li>the weights array does not have the same length as the values array</li>
		///     <li>the weights array contains one or more infinite values</li>
		///     <li>the weights array contains one or more NaN values</li>
		///     <li>the weights array contains negative values</li>
		///     <li>the start and length arguments do not determine a valid array</li>
		/// </ul></p>
		/// 
		/// <p>Uses the formula, <pre>
		///    weighted product = &prod;values[i]<sup>weights[i]</sup>
		/// </pre>
		/// that is, the weights are applied as exponents when computing the weighted product.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="weights"> the weights array </param>
		/// <param name="begin"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the product of the values or 1 if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the parameters are not valid
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double[] weights, final int begin, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values, double[] weights, int begin, int length)
		{
			double product = double.NaN;
			if (test(values, weights, begin, length, true))
			{
				product = 1.0;
				for (int i = begin; i < begin + length; i++)
				{
					product *= FastMath.pow(values[i], weights[i]);
				}
			}
			return product;
		}

		/// <summary>
		/// <p>Returns the weighted product of the entries in the input array.</p>
		/// 
		/// <p>Throws <code>MathIllegalArgumentException</code> if any of the following are true:
		/// <ul><li>the values array is null</li>
		///     <li>the weights array is null</li>
		///     <li>the weights array does not have the same length as the values array</li>
		///     <li>the weights array contains one or more infinite values</li>
		///     <li>the weights array contains one or more NaN values</li>
		///     <li>the weights array contains negative values</li>
		/// </ul></p>
		/// 
		/// <p>Uses the formula, <pre>
		///    weighted product = &prod;values[i]<sup>weights[i]</sup>
		/// </pre>
		/// that is, the weights are applied as exponents when computing the weighted product.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="weights"> the weights array </param>
		/// <returns> the product of the values or Double.NaN if length = 0 </returns>
		/// <exception cref="MathIllegalArgumentException"> if the parameters are not valid
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double[] weights) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values, double[] weights)
		{
			return evaluate(values, weights, 0, values.Length);
		}


		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override Product copy()
		{
			Product result = new Product();
			// No try-catch or advertised exception because args are valid
			copy(this, result);
			return result;
		}

		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> Product to copy </param>
		/// <param name="dest"> Product to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(Product source, Product dest) throws mathlib.exception.NullArgumentException
		public static void copy(Product source, Product dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			dest.Data = source.DataRef;
			dest.n = source.n;
			dest.value = source.value;
		}

	}

}
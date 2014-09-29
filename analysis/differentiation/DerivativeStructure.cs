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
namespace mathlib.analysis.differentiation
{

	using mathlib;
	using mathlib;
	using mathlib;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MathArithmeticException = mathlib.exception.MathArithmeticException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using FastMath = mathlib.util.FastMath;
	using MathArrays = mathlib.util.MathArrays;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// Class representing both the value and the differentials of a function.
	/// <p>This class is the workhorse of the differentiation package.</p>
	/// <p>This class is an implementation of the extension to Rall's
	/// numbers described in Dan Kalman's paper <a
	/// href="http://www1.american.edu/cas/mathstat/People/kalman/pdffiles/mmgautodiff.pdf">Doubly
	/// Recursive Multivariate Automatic Differentiation</a>, Mathematics Magazine, vol. 75,
	/// no. 3, June 2002.</p>. Rall's numbers are an extension to the real numbers used
	/// throughout mathematical expressions; they hold the derivative together with the
	/// value of a function. Dan Kalman's derivative structures hold all partial derivatives
	/// up to any specified order, with respect to any number of free parameters. Rall's
	/// numbers therefore can be seen as derivative structures for order one derivative and
	/// one free parameter, and real numbers can be seen as derivative structures with zero
	/// order derivative and no free parameters.</p>
	/// <p><seealso cref="DerivativeStructure"/> instances can be used directly thanks to
	/// the arithmetic operators to the mathematical functions provided as
	/// methods by this class (+, -, *, /, %, sin, cos ...).</p>
	/// <p>Implementing complex expressions by hand using these classes is
	/// a tedious and error-prone task but has the advantage of having no limitation
	/// on the derivation order despite no requiring users to compute the derivatives by
	/// themselves. Implementing complex expression can also be done by developing computation
	/// code using standard primitive double values and to use {@link
	/// UnivariateFunctionDifferentiator differentiators} to create the {@link
	/// DerivativeStructure}-based instances. This method is simpler but may be limited in
	/// the accuracy and derivation orders and may be computationally intensive (this is
	/// typically the case for {@link FiniteDifferencesDifferentiator finite differences
	/// differentiator}.</p>
	/// <p>Instances of this class are guaranteed to be immutable.</p> </summary>
	/// <seealso cref= DSCompiler
	/// @version $Id: DerivativeStructure.java 1517789 2013-08-27 11:15:50Z luc $
	/// @since 3.1 </seealso>
	[Serializable]
	public class DerivativeStructure : RealFieldElement<DerivativeStructure>
	{

		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 20120730L;

		/// <summary>
		/// Compiler for the current dimensions. </summary>
		[NonSerialized]
		private DSCompiler compiler;

		/// <summary>
		/// Combined array holding all values. </summary>
		private readonly double[] data;

		/// <summary>
		/// Build an instance with all values and derivatives set to 0. </summary>
		/// <param name="compiler"> compiler to use for computation </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private DerivativeStructure(final DSCompiler compiler)
		private DerivativeStructure(DSCompiler compiler)
		{
			this.compiler = compiler;
			this.data = new double[compiler.Size];
		}

		/// <summary>
		/// Build an instance with all values and derivatives set to 0. </summary>
		/// <param name="parameters"> number of free parameters </param>
		/// <param name="order"> derivation order </param>
		/// <exception cref="NumberIsTooLargeException"> if order is too large </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure(final int parameters, final int order) throws mathlib.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public DerivativeStructure(int parameters, int order) : this(DSCompiler.getCompiler(parameters, order))
		{
		}

		/// <summary>
		/// Build an instance representing a constant value. </summary>
		/// <param name="parameters"> number of free parameters </param>
		/// <param name="order"> derivation order </param>
		/// <param name="value"> value of the constant </param>
		/// <exception cref="NumberIsTooLargeException"> if order is too large </exception>
		/// <seealso cref= #DerivativeStructure(int, int, int, double) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure(final int parameters, final int order, final double value) throws mathlib.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public DerivativeStructure(int parameters, int order, double value) : this(parameters, order)
		{
			this.data[0] = value;
		}

		/// <summary>
		/// Build an instance representing a variable.
		/// <p>Instances built using this constructor are considered
		/// to be the free variables with respect to which differentials
		/// are computed. As such, their differential with respect to
		/// themselves is +1.</p> </summary>
		/// <param name="parameters"> number of free parameters </param>
		/// <param name="order"> derivation order </param>
		/// <param name="index"> index of the variable (from 0 to {@code parameters - 1}) </param>
		/// <param name="value"> value of the variable </param>
		/// <exception cref="NumberIsTooLargeException"> if {@code index >= parameters}. </exception>
		/// <seealso cref= #DerivativeStructure(int, int, double) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure(final int parameters, final int order, final int index, final double value) throws mathlib.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public DerivativeStructure(int parameters, int order, int index, double value) : this(parameters, order, value)
		{

			if (index >= parameters)
			{
				throw new NumberIsTooLargeException(index, parameters, false);
			}

			if (order > 0)
			{
				// the derivative of the variable with respect to itself is 1.
				data[DSCompiler.getCompiler(index, order).Size] = 1.0;
			}

		}

		/// <summary>
		/// Linear combination constructor.
		/// The derivative structure built will be a1 * ds1 + a2 * ds2 </summary>
		/// <param name="a1"> first scale factor </param>
		/// <param name="ds1"> first base (unscaled) derivative structure </param>
		/// <param name="a2"> second scale factor </param>
		/// <param name="ds2"> second base (unscaled) derivative structure </param>
		/// <exception cref="DimensionMismatchException"> if number of free parameters or orders are inconsistent </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure(final double a1, final DerivativeStructure ds1, final double a2, final DerivativeStructure ds2) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public DerivativeStructure(double a1, DerivativeStructure ds1, double a2, DerivativeStructure ds2) : this(ds1.compiler)
		{
			compiler.checkCompatibility(ds2.compiler);
			compiler.linearCombination(a1, ds1.data, 0, a2, ds2.data, 0, data, 0);
		}

		/// <summary>
		/// Linear combination constructor.
		/// The derivative structure built will be a1 * ds1 + a2 * ds2 + a3 * ds3 </summary>
		/// <param name="a1"> first scale factor </param>
		/// <param name="ds1"> first base (unscaled) derivative structure </param>
		/// <param name="a2"> second scale factor </param>
		/// <param name="ds2"> second base (unscaled) derivative structure </param>
		/// <param name="a3"> third scale factor </param>
		/// <param name="ds3"> third base (unscaled) derivative structure </param>
		/// <exception cref="DimensionMismatchException"> if number of free parameters or orders are inconsistent </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure(final double a1, final DerivativeStructure ds1, final double a2, final DerivativeStructure ds2, final double a3, final DerivativeStructure ds3) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public DerivativeStructure(double a1, DerivativeStructure ds1, double a2, DerivativeStructure ds2, double a3, DerivativeStructure ds3) : this(ds1.compiler)
		{
			compiler.checkCompatibility(ds2.compiler);
			compiler.checkCompatibility(ds3.compiler);
			compiler.linearCombination(a1, ds1.data, 0, a2, ds2.data, 0, a3, ds3.data, 0, data, 0);
		}

		/// <summary>
		/// Linear combination constructor.
		/// The derivative structure built will be a1 * ds1 + a2 * ds2 + a3 * ds3 + a4 * ds4 </summary>
		/// <param name="a1"> first scale factor </param>
		/// <param name="ds1"> first base (unscaled) derivative structure </param>
		/// <param name="a2"> second scale factor </param>
		/// <param name="ds2"> second base (unscaled) derivative structure </param>
		/// <param name="a3"> third scale factor </param>
		/// <param name="ds3"> third base (unscaled) derivative structure </param>
		/// <param name="a4"> fourth scale factor </param>
		/// <param name="ds4"> fourth base (unscaled) derivative structure </param>
		/// <exception cref="DimensionMismatchException"> if number of free parameters or orders are inconsistent </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure(final double a1, final DerivativeStructure ds1, final double a2, final DerivativeStructure ds2, final double a3, final DerivativeStructure ds3, final double a4, final DerivativeStructure ds4) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public DerivativeStructure(double a1, DerivativeStructure ds1, double a2, DerivativeStructure ds2, double a3, DerivativeStructure ds3, double a4, DerivativeStructure ds4) : this(ds1.compiler)
		{
			compiler.checkCompatibility(ds2.compiler);
			compiler.checkCompatibility(ds3.compiler);
			compiler.checkCompatibility(ds4.compiler);
			compiler.linearCombination(a1, ds1.data, 0, a2, ds2.data, 0, a3, ds3.data, 0, a4, ds4.data, 0, data, 0);
		}

		/// <summary>
		/// Build an instance from all its derivatives. </summary>
		/// <param name="parameters"> number of free parameters </param>
		/// <param name="order"> derivation order </param>
		/// <param name="derivatives"> derivatives sorted according to
		/// <seealso cref="DSCompiler#getPartialDerivativeIndex(int...)"/> </param>
		/// <exception cref="DimensionMismatchException"> if derivatives array does not match the
		/// <seealso cref="DSCompiler#getSize() size"/> expected by the compiler </exception>
		/// <exception cref="NumberIsTooLargeException"> if order is too large </exception>
		/// <seealso cref= #getAllDerivatives() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure(final int parameters, final int order, final double... derivatives) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public DerivativeStructure(int parameters, int order, params double[] derivatives) : this(parameters, order)
		{
			if (derivatives.Length != data.Length)
			{
				throw new DimensionMismatchException(derivatives.Length, data.Length);
			}
			Array.Copy(derivatives, 0, data, 0, data.Length);
		}

		/// <summary>
		/// Copy constructor. </summary>
		/// <param name="ds"> instance to copy </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private DerivativeStructure(final DerivativeStructure ds)
		private DerivativeStructure(DerivativeStructure ds)
		{
			this.compiler = ds.compiler;
			this.data = ds.data.clone();
		}

		/// <summary>
		/// Get the number of free parameters. </summary>
		/// <returns> number of free parameters </returns>
		public virtual int FreeParameters
		{
			get
			{
				return compiler.FreeParameters;
			}
		}

		/// <summary>
		/// Get the derivation order. </summary>
		/// <returns> derivation order </returns>
		public virtual int Order
		{
			get
			{
				return compiler.Order;
			}
		}

		/// <summary>
		/// Create a constant compatible with instance order and number of parameters.
		/// <p>
		/// This method is a convenience factory method, it simply calls
		/// {@code new DerivativeStructure(getFreeParameters(), getOrder(), c)}
		/// </p> </summary>
		/// <param name="c"> value of the constant </param>
		/// <returns> a constant compatible with instance order and number of parameters </returns>
		/// <seealso cref= #DerivativeStructure(int, int, double)
		/// @since 3.3 </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DerivativeStructure createConstant(final double c)
		public virtual DerivativeStructure createConstant(double c)
		{
			return new DerivativeStructure(FreeParameters, Order, c);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual double Real
		{
			get
			{
				return data[0];
			}
		}

		/// <summary>
		/// Get the value part of the derivative structure. </summary>
		/// <returns> value part of the derivative structure </returns>
		/// <seealso cref= #getPartialDerivative(int...) </seealso>
		public virtual double Value
		{
			get
			{
				return data[0];
			}
		}

		/// <summary>
		/// Get a partial derivative. </summary>
		/// <param name="orders"> derivation orders with respect to each variable (if all orders are 0,
		/// the value is returned) </param>
		/// <returns> partial derivative </returns>
		/// <seealso cref= #getValue() </seealso>
		/// <exception cref="DimensionMismatchException"> if the numbers of variables does not
		/// match the instance </exception>
		/// <exception cref="NumberIsTooLargeException"> if sum of derivation orders is larger
		/// than the instance limits </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getPartialDerivative(final int... orders) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double getPartialDerivative(params int[] orders)
		{
			return data[compiler.getPartialDerivativeIndex(orders)];
		}

		/// <summary>
		/// Get all partial derivatives. </summary>
		/// <returns> a fresh copy of partial derivatives, in an array sorted according to
		/// <seealso cref="DSCompiler#getPartialDerivativeIndex(int...)"/> </returns>
		public virtual double[] AllDerivatives
		{
			get
			{
				return data.clone();
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DerivativeStructure add(final double a)
		public virtual DerivativeStructure add(double a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure ds = new DerivativeStructure(this);
			DerivativeStructure ds = new DerivativeStructure(this);
			ds.data[0] += a;
			return ds;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure add(final DerivativeStructure a) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure add(DerivativeStructure a)
		{
			compiler.checkCompatibility(a.compiler);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure ds = new DerivativeStructure(this);
			DerivativeStructure ds = new DerivativeStructure(this);
			compiler.add(data, 0, a.data, 0, ds.data, 0);
			return ds;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DerivativeStructure subtract(final double a)
		public virtual DerivativeStructure subtract(double a)
		{
			return add(-a);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure subtract(final DerivativeStructure a) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure subtract(DerivativeStructure a)
		{
			compiler.checkCompatibility(a.compiler);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure ds = new DerivativeStructure(this);
			DerivativeStructure ds = new DerivativeStructure(this);
			compiler.subtract(data, 0, a.data, 0, ds.data, 0);
			return ds;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DerivativeStructure multiply(final int n)
		public virtual DerivativeStructure multiply(int n)
		{
			return multiply((double) n);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DerivativeStructure multiply(final double a)
		public virtual DerivativeStructure multiply(double a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure ds = new DerivativeStructure(this);
			DerivativeStructure ds = new DerivativeStructure(this);
			for (int i = 0; i < ds.data.Length; ++i)
			{
				ds.data[i] *= a;
			}
			return ds;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure multiply(final DerivativeStructure a) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure multiply(DerivativeStructure a)
		{
			compiler.checkCompatibility(a.compiler);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.multiply(data, 0, a.data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DerivativeStructure divide(final double a)
		public virtual DerivativeStructure divide(double a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure ds = new DerivativeStructure(this);
			DerivativeStructure ds = new DerivativeStructure(this);
			for (int i = 0; i < ds.data.Length; ++i)
			{
				ds.data[i] /= a;
			}
			return ds;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure divide(final DerivativeStructure a) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure divide(DerivativeStructure a)
		{
			compiler.checkCompatibility(a.compiler);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.divide(data, 0, a.data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DerivativeStructure remainder(final double a)
		public virtual DerivativeStructure remainder(double a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure ds = new DerivativeStructure(this);
			DerivativeStructure ds = new DerivativeStructure(this);
			ds.data[0] = FastMath.IEEEremainder(ds.data[0], a);
			return ds;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure remainder(final DerivativeStructure a) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure remainder(DerivativeStructure a)
		{
			compiler.checkCompatibility(a.compiler);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.remainder(data, 0, a.data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual DerivativeStructure negate()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure ds = new DerivativeStructure(compiler);
			DerivativeStructure ds = new DerivativeStructure(compiler);
			for (int i = 0; i < ds.data.Length; ++i)
			{
				ds.data[i] = -data[i];
			}
			return ds;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure abs()
		{
			if (double.doubleToLongBits(data[0]) < 0)
			{
				// we use the bits representation to also handle -0.0
				return negate();
			}
			else
			{
				return this;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure ceil()
		{
			return new DerivativeStructure(compiler.FreeParameters, compiler.Order, FastMath.ceil(data[0]));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure floor()
		{
			return new DerivativeStructure(compiler.FreeParameters, compiler.Order, FastMath.floor(data[0]));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure rint()
		{
			return new DerivativeStructure(compiler.FreeParameters, compiler.Order, FastMath.rint(data[0]));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual long round()
		{
			return FastMath.round(data[0]);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure signum()
		{
			return new DerivativeStructure(compiler.FreeParameters, compiler.Order, FastMath.signum(data[0]));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DerivativeStructure copySign(final DerivativeStructure sign)
		public virtual DerivativeStructure copySign(DerivativeStructure sign)
		{
			long m = double.doubleToLongBits(data[0]);
			long s = double.doubleToLongBits(sign.data[0]);
			if ((m >= 0 && s >= 0) || (m < 0 && s < 0)) // Sign is currently OK
			{
				return this;
			}
			return negate(); // flip sign
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DerivativeStructure copySign(final double sign)
		public virtual DerivativeStructure copySign(double sign)
		{
			long m = double.doubleToLongBits(data[0]);
			long s = double.doubleToLongBits(sign);
			if ((m >= 0 && s >= 0) || (m < 0 && s < 0)) // Sign is currently OK
			{
				return this;
			}
			return negate(); // flip sign
		}

		/// <summary>
		/// Return the exponent of the instance value, removing the bias.
		/// <p>
		/// For double numbers of the form 2<sup>x</sup>, the unbiased
		/// exponent is exactly x.
		/// </p> </summary>
		/// <returns> exponent for instance in IEEE754 representation, without bias </returns>
		public virtual int Exponent
		{
			get
			{
				return FastMath.getExponent(data[0]);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DerivativeStructure scalb(final int n)
		public virtual DerivativeStructure scalb(int n)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure ds = new DerivativeStructure(compiler);
			DerivativeStructure ds = new DerivativeStructure(compiler);
			for (int i = 0; i < ds.data.Length; ++i)
			{
				ds.data[i] = FastMath.scalb(data[i], n);
			}
			return ds;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure hypot(final DerivativeStructure y) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure hypot(DerivativeStructure y)
		{

			compiler.checkCompatibility(y.compiler);

			if (double.IsInfinity(data[0]) || double.IsInfinity(y.data[0]))
			{
				return new DerivativeStructure(compiler.FreeParameters, compiler.FreeParameters, double.PositiveInfinity);
			}
			else if (double.IsNaN(data[0]) || double.IsNaN(y.data[0]))
			{
				return new DerivativeStructure(compiler.FreeParameters, compiler.FreeParameters, double.NaN);
			}
			else
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int expX = getExponent();
				int expX = Exponent;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int expY = y.getExponent();
				int expY = y.Exponent;
				if (expX > expY + 27)
				{
					// y is neglectible with respect to x
					return abs();
				}
				else if (expY > expX + 27)
				{
					// x is neglectible with respect to y
					return y.abs();
				}
				else
				{

					// find an intermediate scale to avoid both overflow and underflow
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int middleExp = (expX + expY) / 2;
					int middleExp = (expX + expY) / 2;

					// scale parameters without losing precision
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure scaledX = scalb(-middleExp);
					DerivativeStructure scaledX = scalb(-middleExp);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure scaledY = y.scalb(-middleExp);
					DerivativeStructure scaledY = y.scalb(-middleExp);

					// compute scaled hypotenuse
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure scaledH = scaledX.multiply(scaledX).add(scaledY.multiply(scaledY)).sqrt();
					DerivativeStructure scaledH = scaledX.multiply(scaledX).add(scaledY.multiply(scaledY)).sqrt();

					// remove scaling
					return scaledH.scalb(middleExp);

				}

			}
		}

		/// <summary>
		/// Returns the hypotenuse of a triangle with sides {@code x} and {@code y}
		/// - sqrt(<i>x</i><sup>2</sup>&nbsp;+<i>y</i><sup>2</sup>)<br/>
		/// avoiding intermediate overflow or underflow.
		/// 
		/// <ul>
		/// <li> If either argument is infinite, then the result is positive infinity.</li>
		/// <li> else, if either argument is NaN then the result is NaN.</li>
		/// </ul>
		/// </summary>
		/// <param name="x"> a value </param>
		/// <param name="y"> a value </param>
		/// <returns> sqrt(<i>x</i><sup>2</sup>&nbsp;+<i>y</i><sup>2</sup>) </returns>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DerivativeStructure hypot(final DerivativeStructure x, final DerivativeStructure y) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static DerivativeStructure hypot(DerivativeStructure x, DerivativeStructure y)
		{
			return x.hypot(y);
		}

		/// <summary>
		/// Compute composition of the instance by a univariate function. </summary>
		/// <param name="f"> array of value and derivatives of the function at
		/// the current point (i.e. [f(<seealso cref="#getValue()"/>),
		/// f'(<seealso cref="#getValue()"/>), f''(<seealso cref="#getValue()"/>)...]). </param>
		/// <returns> f(this) </returns>
		/// <exception cref="DimensionMismatchException"> if the number of derivatives
		/// in the array is not equal to <seealso cref="#getOrder() order"/> + 1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure compose(final double... f) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure compose(params double[] f)
		{
			if (f.Length != Order + 1)
			{
				throw new DimensionMismatchException(f.Length, Order + 1);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.compose(data, 0, f, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual DerivativeStructure reciprocal()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.pow(data, 0, -1, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure sqrt()
		{
			return rootN(2);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure cbrt()
		{
			return rootN(3);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DerivativeStructure rootN(final int n)
		public virtual DerivativeStructure rootN(int n)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.rootN(data, 0, n, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Field<DerivativeStructure> Field
		{
			get
			{
				return new FieldAnonymousInnerClassHelper(this);
			}
		}

		private class FieldAnonymousInnerClassHelper : Field<DerivativeStructure>
		{
			private readonly DerivativeStructure outerInstance;

			public FieldAnonymousInnerClassHelper(DerivativeStructure outerInstance)
			{
				this.outerInstance = outerInstance;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual DerivativeStructure Zero
			{
				get
				{
					return new DerivativeStructure(outerInstance.compiler.FreeParameters, outerInstance.compiler.Order, 0.0);
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual DerivativeStructure One
			{
				get
				{
					return new DerivativeStructure(outerInstance.compiler.FreeParameters, outerInstance.compiler.Order, 1.0);
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual Type RuntimeClass
			{
				get
				{
					return typeof(DerivativeStructure);
				}
			}

		}

		/// <summary>
		/// Compute a<sup>x</sup> where a is a double and x a <seealso cref="DerivativeStructure"/> </summary>
		/// <param name="a"> number to exponentiate </param>
		/// <param name="x"> power to apply </param>
		/// <returns> a<sup>x</sup>
		/// @since 3.3 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static DerivativeStructure pow(final double a, final DerivativeStructure x)
		public static DerivativeStructure pow(double a, DerivativeStructure x)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(x.compiler);
			DerivativeStructure result = new DerivativeStructure(x.compiler);
			x.compiler.pow(a, x.data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DerivativeStructure pow(final double p)
		public virtual DerivativeStructure pow(double p)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.pow(data, 0, p, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DerivativeStructure pow(final int n)
		public virtual DerivativeStructure pow(int n)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.pow(data, 0, n, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure pow(final DerivativeStructure e) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure pow(DerivativeStructure e)
		{
			compiler.checkCompatibility(e.compiler);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.pow(data, 0, e.data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure exp()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.exp(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure expm1()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.expm1(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure log()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.log(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure log1p()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.log1p(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// Base 10 logarithm. </summary>
		/// <returns> base 10 logarithm of the instance </returns>
		public virtual DerivativeStructure log10()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.log10(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure cos()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.cos(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure sin()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.sin(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure tan()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.tan(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure acos()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.acos(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure asin()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.asin(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure atan()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.atan(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure atan2(final DerivativeStructure x) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure atan2(DerivativeStructure x)
		{
			compiler.checkCompatibility(x.compiler);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.atan2(data, 0, x.data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// Two arguments arc tangent operation. </summary>
		/// <param name="y"> first argument of the arc tangent </param>
		/// <param name="x"> second argument of the arc tangent </param>
		/// <returns> atan2(y, x) </returns>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DerivativeStructure atan2(final DerivativeStructure y, final DerivativeStructure x) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static DerivativeStructure atan2(DerivativeStructure y, DerivativeStructure x)
		{
			return y.atan2(x);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure cosh()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.cosh(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure sinh()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.sinh(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure tanh()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.tanh(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure acosh()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.acosh(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure asinh()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.asinh(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual DerivativeStructure atanh()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure result = new DerivativeStructure(compiler);
			DerivativeStructure result = new DerivativeStructure(compiler);
			compiler.atanh(data, 0, result.data, 0);
			return result;
		}

		/// <summary>
		/// Convert radians to degrees, with error of less than 0.5 ULP </summary>
		///  <returns> instance converted into degrees </returns>
		public virtual DerivativeStructure toDegrees()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure ds = new DerivativeStructure(compiler);
			DerivativeStructure ds = new DerivativeStructure(compiler);
			for (int i = 0; i < ds.data.Length; ++i)
			{
				ds.data[i] = FastMath.toDegrees(data[i]);
			}
			return ds;
		}

		/// <summary>
		/// Convert degrees to radians, with error of less than 0.5 ULP </summary>
		///  <returns> instance converted into radians </returns>
		public virtual DerivativeStructure toRadians()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure ds = new DerivativeStructure(compiler);
			DerivativeStructure ds = new DerivativeStructure(compiler);
			for (int i = 0; i < ds.data.Length; ++i)
			{
				ds.data[i] = FastMath.toRadians(data[i]);
			}
			return ds;
		}

		/// <summary>
		/// Evaluate Taylor expansion a derivative structure. </summary>
		/// <param name="delta"> parameters offsets (&Delta;x, &Delta;y, ...) </param>
		/// <returns> value of the Taylor expansion at x + &Delta;x, y + &Delta;y, ... </returns>
		/// <exception cref="MathArithmeticException"> if factorials becomes too large </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double taylor(final double... delta) throws mathlib.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double taylor(params double[] delta)
		{
			return compiler.taylor(data, 0, delta);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure linearCombination(final DerivativeStructure[] a, final DerivativeStructure[] b) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure linearCombination(DerivativeStructure[] a, DerivativeStructure[] b)
		{

			// compute an accurate value, taking care of cancellations
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] aDouble = new double[a.length];
			double[] aDouble = new double[a.Length];
			for (int i = 0; i < a.Length; ++i)
			{
				aDouble[i] = a[i].Value;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] bDouble = new double[b.length];
			double[] bDouble = new double[b.Length];
			for (int i = 0; i < b.Length; ++i)
			{
				bDouble[i] = b[i].Value;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double accurateValue = mathlib.util.MathArrays.linearCombination(aDouble, bDouble);
			double accurateValue = MathArrays.linearCombination(aDouble, bDouble);

			// compute a simple value, with all partial derivatives
			DerivativeStructure simpleValue = a[0].Field.Zero;
			for (int i = 0; i < a.Length; ++i)
			{
				simpleValue = simpleValue.add(a[i].multiply(b[i]));
			}

			// create a result with accurate value and all derivatives (not necessarily as accurate as the value)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] all = simpleValue.getAllDerivatives();
			double[] all = simpleValue.AllDerivatives;
			all[0] = accurateValue;
			return new DerivativeStructure(simpleValue.FreeParameters, simpleValue.Order, all);

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure linearCombination(final double[] a, final DerivativeStructure[] b) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure linearCombination(double[] a, DerivativeStructure[] b)
		{

			// compute an accurate value, taking care of cancellations
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] bDouble = new double[b.length];
			double[] bDouble = new double[b.Length];
			for (int i = 0; i < b.Length; ++i)
			{
				bDouble[i] = b[i].Value;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double accurateValue = mathlib.util.MathArrays.linearCombination(a, bDouble);
			double accurateValue = MathArrays.linearCombination(a, bDouble);

			// compute a simple value, with all partial derivatives
			DerivativeStructure simpleValue = b[0].Field.Zero;
			for (int i = 0; i < a.Length; ++i)
			{
				simpleValue = simpleValue.add(b[i].multiply(a[i]));
			}

			// create a result with accurate value and all derivatives (not necessarily as accurate as the value)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] all = simpleValue.getAllDerivatives();
			double[] all = simpleValue.AllDerivatives;
			all[0] = accurateValue;
			return new DerivativeStructure(simpleValue.FreeParameters, simpleValue.Order, all);

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure linearCombination(final DerivativeStructure a1, final DerivativeStructure b1, final DerivativeStructure a2, final DerivativeStructure b2) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure linearCombination(DerivativeStructure a1, DerivativeStructure b1, DerivativeStructure a2, DerivativeStructure b2)
		{

			// compute an accurate value, taking care of cancellations
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double accurateValue = mathlib.util.MathArrays.linearCombination(a1.getValue(), b1.getValue(), a2.getValue(), b2.getValue());
			double accurateValue = MathArrays.linearCombination(a1.Value, b1.Value, a2.Value, b2.Value);

			// compute a simple value, with all partial derivatives
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure simpleValue = a1.multiply(b1).add(a2.multiply(b2));
			DerivativeStructure simpleValue = a1.multiply(b1).add(a2.multiply(b2));

			// create a result with accurate value and all derivatives (not necessarily as accurate as the value)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] all = simpleValue.getAllDerivatives();
			double[] all = simpleValue.AllDerivatives;
			all[0] = accurateValue;
			return new DerivativeStructure(FreeParameters, Order, all);

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure linearCombination(final double a1, final DerivativeStructure b1, final double a2, final DerivativeStructure b2) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure linearCombination(double a1, DerivativeStructure b1, double a2, DerivativeStructure b2)
		{

			// compute an accurate value, taking care of cancellations
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double accurateValue = mathlib.util.MathArrays.linearCombination(a1, b1.getValue(), a2, b2.getValue());
			double accurateValue = MathArrays.linearCombination(a1, b1.Value, a2, b2.Value);

			// compute a simple value, with all partial derivatives
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure simpleValue = b1.multiply(a1).add(b2.multiply(a2));
			DerivativeStructure simpleValue = b1.multiply(a1).add(b2.multiply(a2));

			// create a result with accurate value and all derivatives (not necessarily as accurate as the value)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] all = simpleValue.getAllDerivatives();
			double[] all = simpleValue.AllDerivatives;
			all[0] = accurateValue;
			return new DerivativeStructure(FreeParameters, Order, all);

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure linearCombination(final DerivativeStructure a1, final DerivativeStructure b1, final DerivativeStructure a2, final DerivativeStructure b2, final DerivativeStructure a3, final DerivativeStructure b3) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure linearCombination(DerivativeStructure a1, DerivativeStructure b1, DerivativeStructure a2, DerivativeStructure b2, DerivativeStructure a3, DerivativeStructure b3)
		{

			// compute an accurate value, taking care of cancellations
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double accurateValue = mathlib.util.MathArrays.linearCombination(a1.getValue(), b1.getValue(), a2.getValue(), b2.getValue(), a3.getValue(), b3.getValue());
			double accurateValue = MathArrays.linearCombination(a1.Value, b1.Value, a2.Value, b2.Value, a3.Value, b3.Value);

			// compute a simple value, with all partial derivatives
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure simpleValue = a1.multiply(b1).add(a2.multiply(b2)).add(a3.multiply(b3));
			DerivativeStructure simpleValue = a1.multiply(b1).add(a2.multiply(b2)).add(a3.multiply(b3));

			// create a result with accurate value and all derivatives (not necessarily as accurate as the value)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] all = simpleValue.getAllDerivatives();
			double[] all = simpleValue.AllDerivatives;
			all[0] = accurateValue;
			return new DerivativeStructure(FreeParameters, Order, all);

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure linearCombination(final double a1, final DerivativeStructure b1, final double a2, final DerivativeStructure b2, final double a3, final DerivativeStructure b3) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure linearCombination(double a1, DerivativeStructure b1, double a2, DerivativeStructure b2, double a3, DerivativeStructure b3)
		{

			// compute an accurate value, taking care of cancellations
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double accurateValue = mathlib.util.MathArrays.linearCombination(a1, b1.getValue(), a2, b2.getValue(), a3, b3.getValue());
			double accurateValue = MathArrays.linearCombination(a1, b1.Value, a2, b2.Value, a3, b3.Value);

			// compute a simple value, with all partial derivatives
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure simpleValue = b1.multiply(a1).add(b2.multiply(a2)).add(b3.multiply(a3));
			DerivativeStructure simpleValue = b1.multiply(a1).add(b2.multiply(a2)).add(b3.multiply(a3));

			// create a result with accurate value and all derivatives (not necessarily as accurate as the value)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] all = simpleValue.getAllDerivatives();
			double[] all = simpleValue.AllDerivatives;
			all[0] = accurateValue;
			return new DerivativeStructure(FreeParameters, Order, all);

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure linearCombination(final DerivativeStructure a1, final DerivativeStructure b1, final DerivativeStructure a2, final DerivativeStructure b2, final DerivativeStructure a3, final DerivativeStructure b3, final DerivativeStructure a4, final DerivativeStructure b4) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure linearCombination(DerivativeStructure a1, DerivativeStructure b1, DerivativeStructure a2, DerivativeStructure b2, DerivativeStructure a3, DerivativeStructure b3, DerivativeStructure a4, DerivativeStructure b4)
		{

			// compute an accurate value, taking care of cancellations
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double accurateValue = mathlib.util.MathArrays.linearCombination(a1.getValue(), b1.getValue(), a2.getValue(), b2.getValue(), a3.getValue(), b3.getValue(), a4.getValue(), b4.getValue());
			double accurateValue = MathArrays.linearCombination(a1.Value, b1.Value, a2.Value, b2.Value, a3.Value, b3.Value, a4.Value, b4.Value);

			// compute a simple value, with all partial derivatives
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure simpleValue = a1.multiply(b1).add(a2.multiply(b2)).add(a3.multiply(b3)).add(a4.multiply(b4));
			DerivativeStructure simpleValue = a1.multiply(b1).add(a2.multiply(b2)).add(a3.multiply(b3)).add(a4.multiply(b4));

			// create a result with accurate value and all derivatives (not necessarily as accurate as the value)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] all = simpleValue.getAllDerivatives();
			double[] all = simpleValue.AllDerivatives;
			all[0] = accurateValue;
			return new DerivativeStructure(FreeParameters, Order, all);

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <exception cref="DimensionMismatchException"> if number of free parameters
		/// or orders do not match
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure linearCombination(final double a1, final DerivativeStructure b1, final double a2, final DerivativeStructure b2, final double a3, final DerivativeStructure b3, final double a4, final DerivativeStructure b4) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure linearCombination(double a1, DerivativeStructure b1, double a2, DerivativeStructure b2, double a3, DerivativeStructure b3, double a4, DerivativeStructure b4)
		{

			// compute an accurate value, taking care of cancellations
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double accurateValue = mathlib.util.MathArrays.linearCombination(a1, b1.getValue(), a2, b2.getValue(), a3, b3.getValue(), a4, b4.getValue());
			double accurateValue = MathArrays.linearCombination(a1, b1.Value, a2, b2.Value, a3, b3.Value, a4, b4.Value);

			// compute a simple value, with all partial derivatives
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure simpleValue = b1.multiply(a1).add(b2.multiply(a2)).add(b3.multiply(a3)).add(b4.multiply(a4));
			DerivativeStructure simpleValue = b1.multiply(a1).add(b2.multiply(a2)).add(b3.multiply(a3)).add(b4.multiply(a4));

			// create a result with accurate value and all derivatives (not necessarily as accurate as the value)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] all = simpleValue.getAllDerivatives();
			double[] all = simpleValue.AllDerivatives;
			all[0] = accurateValue;
			return new DerivativeStructure(FreeParameters, Order, all);

		}

		/// <summary>
		/// Test for the equality of two derivative structures.
		/// <p>
		/// Derivative structures are considered equal if they have the same number
		/// of free parameters, the same derivation order, and the same derivatives.
		/// </p> </summary>
		/// <param name="other"> Object to test for equality to this </param>
		/// <returns> true if two derivative structures are equal
		/// @since 3.2 </returns>
		public override bool Equals(object other)
		{

			if (this == other)
			{
				return true;
			}

			if (other is DerivativeStructure)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure rhs = (DerivativeStructure)other;
				DerivativeStructure rhs = (DerivativeStructure)other;
				return (FreeParameters == rhs.FreeParameters) && (Order == rhs.Order) && MathArrays.Equals(data, rhs.data);
			}

			return false;

		}

		/// <summary>
		/// Get a hashCode for the derivative structure. </summary>
		/// <returns> a hash code value for this object
		/// @since 3.2 </returns>
		public override int GetHashCode()
		{
			return 227 + 229 * FreeParameters + 233 * Order + 239 * MathUtils.hash(data);
		}

		/// <summary>
		/// Replace the instance with a data transfer object for serialization. </summary>
		/// <returns> data transfer object that will be serialized </returns>
		private object writeReplace()
		{
			return new DataTransferObject(compiler.FreeParameters, compiler.Order, data);
		}

		/// <summary>
		/// Internal class used only for serialization. </summary>
		[Serializable]
		private class DataTransferObject
		{

			/// <summary>
			/// Serializable UID. </summary>
			internal const long serialVersionUID = 20120730L;

			/// <summary>
			/// Number of variables.
			/// @serial
			/// </summary>
			internal readonly int variables;

			/// <summary>
			/// Derivation order.
			/// @serial
			/// </summary>
			internal readonly int order;

			/// <summary>
			/// Partial derivatives.
			/// @serial
			/// </summary>
			internal readonly double[] data;

			/// <summary>
			/// Simple constructor. </summary>
			/// <param name="variables"> number of variables </param>
			/// <param name="order"> derivation order </param>
			/// <param name="data"> partial derivatives </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DataTransferObject(final int variables, final int order, final double[] data)
			public DataTransferObject(int variables, int order, double[] data)
			{
				this.variables = variables;
				this.order = order;
				this.data = data;
			}

			/// <summary>
			/// Replace the deserialized data transfer object with a <seealso cref="DerivativeStructure"/>. </summary>
			/// <returns> replacement <seealso cref="DerivativeStructure"/> </returns>
			internal virtual object readResolve()
			{
				return new DerivativeStructure(variables, order, data);
			}

		}

	}

}
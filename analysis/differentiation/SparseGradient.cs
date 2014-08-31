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
namespace org.apache.commons.math3.analysis.differentiation
{


	using org.apache.commons.math3;
	using org.apache.commons.math3;
	using org.apache.commons.math3;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathArrays = org.apache.commons.math3.util.MathArrays;
	using MathUtils = org.apache.commons.math3.util.MathUtils;
	using Precision = org.apache.commons.math3.util.Precision;

	/// <summary>
	/// First derivative computation with large number of variables.
	/// <p>
	/// This class plays a similar role to <seealso cref="DerivativeStructure"/>, with
	/// a focus on efficiency when dealing with large number of independent variables
	/// and most computation depend only on a few of them, and when only first derivative
	/// is desired. When these conditions are met, this class should be much faster than
	/// <seealso cref="DerivativeStructure"/> and use less memory.
	/// </p>
	/// 
	/// @version $Id: SparseGradient.java 1536147 2013-10-27 14:39:16Z luc $
	/// @since 3.3
	/// </summary>
	[Serializable]
	public class SparseGradient : RealFieldElement<SparseGradient>
	{

		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 20131025L;

		/// <summary>
		/// Value of the calculation. </summary>
		private double value;

		/// <summary>
		/// Stored derivative, each key representing a different independent variable. </summary>
		private readonly IDictionary<int?, double?> derivatives;

		/// <summary>
		/// Internal constructor. </summary>
		/// <param name="value"> value of the function </param>
		/// <param name="derivatives"> derivatives map, a deep copy will be performed,
		/// so the map given here will remain safe from changes in the new instance,
		/// may be null to create an empty derivatives map, i.e. a constant value </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private SparseGradient(final double value, final java.util.Map<Integer, Double> derivatives)
		private SparseGradient(double value, IDictionary<int?, double?> derivatives)
		{
			this.value = value;
			this.derivatives = new Dictionary<int?, double?>();
			if (derivatives != null)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
				this.derivatives.putAll(derivatives);
			}
		}

		/// <summary>
		/// Internal constructor. </summary>
		/// <param name="value"> value of the function </param>
		/// <param name="scale"> scaling factor to apply to all derivatives </param>
		/// <param name="derivatives"> derivatives map, a deep copy will be performed,
		/// so the map given here will remain safe from changes in the new instance,
		/// may be null to create an empty derivatives map, i.e. a constant value </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private SparseGradient(final double value, final double scale, final java.util.Map<Integer, Double> derivatives)
		private SparseGradient(double value, double scale, IDictionary<int?, double?> derivatives)
		{
			this.value = value;
			this.derivatives = new Dictionary<int?, double?>();
			if (derivatives != null)
			{
				foreach (KeyValuePair<int?, double?> entry in derivatives)
				{
					this.derivatives[entry.Key] = scale * entry.Value;
				}
			}
		}

		/// <summary>
		/// Factory method creating a constant. </summary>
		/// <param name="value"> value of the constant </param>
		/// <returns> a new instance </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static SparseGradient createConstant(final double value)
		public static SparseGradient createConstant(double value)
		{
			return new SparseGradient(value, Collections.emptyMap<int?, double?> ());
		}

		/// <summary>
		/// Factory method creating an independent variable. </summary>
		/// <param name="idx"> index of the variable </param>
		/// <param name="value"> value of the variable </param>
		/// <returns> a new instance </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static SparseGradient createVariable(final int idx, final double value)
		public static SparseGradient createVariable(int idx, double value)
		{
			return new SparseGradient(value, Collections.singletonMap(idx, 1.0));
		}

		/// <summary>
		/// Find the number of variables. </summary>
		/// <returns> number of variables </returns>
		public virtual int numVars()
		{
			return derivatives.Count;
		}

		/// <summary>
		/// Get the derivative with respect to a particular index variable.
		/// </summary>
		/// <param name="index"> index to differentiate with. </param>
		/// <returns> derivative with respect to a particular index variable </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double getDerivative(final int index)
		public virtual double getDerivative(int index)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Double out = derivatives.get(index);
			double? @out = derivatives[index];
			return (@out == null) ? 0.0 : @out;
		}

		/// <summary>
		/// Get the value of the function. </summary>
		/// <returns> value of the function. </returns>
		public virtual double Value
		{
			get
			{
				return value;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double Real
		{
			get
			{
				return value;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient add(final SparseGradient a)
		public virtual SparseGradient add(SparseGradient a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SparseGradient out = new SparseGradient(value + a.value, derivatives);
			SparseGradient @out = new SparseGradient(value + a.value, derivatives);
			foreach (KeyValuePair<int?, double?> entry in a.derivatives)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int id = entry.getKey();
				int id = entry.Key;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Double old = out.derivatives.get(id);
				double? old = @out.derivatives[id];
				if (old == null)
				{
					@out.derivatives[id] = entry.Value;
				}
				else
				{
					@out.derivatives[id] = old + entry.Value;
				}
			}

			return @out;
		}

		/// <summary>
		/// Add in place.
		/// <p>
		/// This method is designed to be faster when used multiple times in a loop.
		/// </p>
		/// <p>
		/// The instance is changed here, in order to not change the
		/// instance the <seealso cref="#add(SparseGradient)"/> method should
		/// be used.
		/// </p> </summary>
		/// <param name="a"> instance to add </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void addInPlace(final SparseGradient a)
		public virtual void addInPlace(SparseGradient a)
		{
			value += a.value;
			foreach (KeyValuePair<int?, double?> entry in a.derivatives)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int id = entry.getKey();
				int id = entry.Key;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Double old = derivatives.get(id);
				double? old = derivatives[id];
				if (old == null)
				{
					derivatives[id] = entry.Value;
				}
				else
				{
					derivatives[id] = old + entry.Value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient add(final double c)
		public virtual SparseGradient add(double c)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SparseGradient out = new SparseGradient(value + c, derivatives);
			SparseGradient @out = new SparseGradient(value + c, derivatives);
			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient subtract(final SparseGradient a)
		public virtual SparseGradient subtract(SparseGradient a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SparseGradient out = new SparseGradient(value - a.value, derivatives);
			SparseGradient @out = new SparseGradient(value - a.value, derivatives);
			foreach (KeyValuePair<int?, double?> entry in a.derivatives)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int id = entry.getKey();
				int id = entry.Key;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Double old = out.derivatives.get(id);
				double? old = @out.derivatives[id];
				if (old == null)
				{
					@out.derivatives[id] = -entry.Value;
				}
				else
				{
					@out.derivatives[id] = old - entry.Value;
				}
			}
			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient subtract(double c)
		{
			return new SparseGradient(value - c, derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient multiply(final SparseGradient a)
		public virtual SparseGradient multiply(SparseGradient a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SparseGradient out = new SparseGradient(value * a.value, java.util.Collections.emptyMap<Integer, Double> ());
			SparseGradient @out = new SparseGradient(value * a.value, Collections.emptyMap<int?, double?> ());

			// Derivatives.
			foreach (KeyValuePair<int?, double?> entry in derivatives)
			{
				@out.derivatives[entry.Key] = a.value * entry.Value;
			}
			foreach (KeyValuePair<int?, double?> entry in a.derivatives)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int id = entry.getKey();
				int id = entry.Key;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Double old = out.derivatives.get(id);
				double? old = @out.derivatives[id];
				if (old == null)
				{
					@out.derivatives[id] = value * entry.Value;
				}
				else
				{
					@out.derivatives[id] = old + value * entry.Value;
				}
			}
			return @out;
		}

		/// <summary>
		/// Multiply in place.
		/// <p>
		/// This method is designed to be faster when used multiple times in a loop.
		/// </p>
		/// <p>
		/// The instance is changed here, in order to not change the
		/// instance the <seealso cref="#add(SparseGradient)"/> method should
		/// be used.
		/// </p> </summary>
		/// <param name="a"> instance to multiply </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void multiplyInPlace(final SparseGradient a)
		public virtual void multiplyInPlace(SparseGradient a)
		{
			// Derivatives.
			foreach (KeyValuePair<int?, double?> entry in derivatives)
			{
				derivatives[entry.Key] = a.value * entry.Value;
			}
			foreach (KeyValuePair<int?, double?> entry in a.derivatives)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int id = entry.getKey();
				int id = entry.Key;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Double old = derivatives.get(id);
				double? old = derivatives[id];
				if (old == null)
				{
					derivatives[id] = value * entry.Value;
				}
				else
				{
					derivatives[id] = old + value * entry.Value;
				}
			}
			value *= a.value;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient multiply(final double c)
		public virtual SparseGradient multiply(double c)
		{
			return new SparseGradient(value * c, c, derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient multiply(final int n)
		public virtual SparseGradient multiply(int n)
		{
			return new SparseGradient(value * n, n, derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient divide(final SparseGradient a)
		public virtual SparseGradient divide(SparseGradient a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SparseGradient out = new SparseGradient(value / a.value, java.util.Collections.emptyMap<Integer, Double> ());
			SparseGradient @out = new SparseGradient(value / a.value, Collections.emptyMap<int?, double?> ());

			// Derivatives.
			foreach (KeyValuePair<int?, double?> entry in derivatives)
			{
				@out.derivatives[entry.Key] = entry.Value / a.value;
			}
			foreach (KeyValuePair<int?, double?> entry in a.derivatives)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int id = entry.getKey();
				int id = entry.Key;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Double old = out.derivatives.get(id);
				double? old = @out.derivatives[id];
				if (old == null)
				{
					@out.derivatives[id] = -@out.value / a.value * entry.Value;
				}
				else
				{
					@out.derivatives[id] = old - @out.value / a.value * entry.Value;
				}
			}
			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient divide(final double c)
		public virtual SparseGradient divide(double c)
		{
			return new SparseGradient(value / c, 1.0 / c, derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient negate()
		{
			return new SparseGradient(-value, -1.0, derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Field<SparseGradient> Field
		{
			get
			{
				return new FieldAnonymousInnerClassHelper(this);
			}
		}

		private class FieldAnonymousInnerClassHelper : Field<SparseGradient>
		{
			private readonly SparseGradient outerInstance;

			public FieldAnonymousInnerClassHelper(SparseGradient outerInstance)
			{
				this.outerInstance = outerInstance;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual SparseGradient Zero
			{
				get
				{
					return createConstant(0);
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual SparseGradient One
			{
				get
				{
					return createConstant(1);
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual Type RuntimeClass
			{
				get
				{
					return typeof(SparseGradient);
				}
			}

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient remainder(final double a)
		public virtual SparseGradient remainder(double a)
		{
			return new SparseGradient(FastMath.IEEEremainder(value, a), derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient remainder(final SparseGradient a)
		public virtual SparseGradient remainder(SparseGradient a)
		{

			// compute k such that lhs % rhs = lhs - k rhs
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rem = org.apache.commons.math3.util.FastMath.IEEEremainder(value, a.value);
			double rem = FastMath.IEEEremainder(value, a.value);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double k = org.apache.commons.math3.util.FastMath.rint((value - rem) / a.value);
			double k = FastMath.rint((value - rem) / a.value);

			return subtract(a.multiply(k));

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient abs()
		{
			if (double.doubleToLongBits(value) < 0)
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
		/// {@inheritDoc} </summary>
		public virtual SparseGradient ceil()
		{
			return createConstant(FastMath.ceil(value));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient floor()
		{
			return createConstant(FastMath.floor(value));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient rint()
		{
			return createConstant(FastMath.rint(value));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual long round()
		{
			return FastMath.round(value);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient signum()
		{
			return createConstant(FastMath.signum(value));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient copySign(final SparseGradient sign)
		public virtual SparseGradient copySign(SparseGradient sign)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long m = Double.doubleToLongBits(value);
			long m = double.doubleToLongBits(value);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long s = Double.doubleToLongBits(sign.value);
			long s = double.doubleToLongBits(sign.value);
			if ((m >= 0 && s >= 0) || (m < 0 && s < 0)) // Sign is currently OK
			{
				return this;
			}
			return negate(); // flip sign
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient copySign(final double sign)
		public virtual SparseGradient copySign(double sign)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long m = Double.doubleToLongBits(value);
			long m = double.doubleToLongBits(value);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long s = Double.doubleToLongBits(sign);
			long s = double.doubleToLongBits(sign);
			if ((m >= 0 && s >= 0) || (m < 0 && s < 0)) // Sign is currently OK
			{
				return this;
			}
			return negate(); // flip sign
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient scalb(final int n)
		public virtual SparseGradient scalb(int n)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SparseGradient out = new SparseGradient(org.apache.commons.math3.util.FastMath.scalb(value, n), java.util.Collections.emptyMap<Integer, Double> ());
			SparseGradient @out = new SparseGradient(FastMath.scalb(value, n), Collections.emptyMap<int?, double?> ());
			foreach (KeyValuePair<int?, double?> entry in derivatives)
			{
				@out.derivatives[entry.Key] = FastMath.scalb(entry.Value, n);
			}
			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient hypot(final SparseGradient y)
		public virtual SparseGradient hypot(SparseGradient y)
		{
			if (double.IsInfinity(value) || double.IsInfinity(y.value))
			{
				return createConstant(double.PositiveInfinity);
			}
			else if (double.IsNaN(value) || double.IsNaN(y.value))
			{
				return createConstant(double.NaN);
			}
			else
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int expX = org.apache.commons.math3.util.FastMath.getExponent(value);
				int expX = FastMath.getExponent(value);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int expY = org.apache.commons.math3.util.FastMath.getExponent(y.value);
				int expY = FastMath.getExponent(y.value);
				if (expX > expY + 27)
				{
					// y is negligible with respect to x
					return abs();
				}
				else if (expY > expX + 27)
				{
					// x is negligible with respect to y
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
//ORIGINAL LINE: final SparseGradient scaledX = scalb(-middleExp);
					SparseGradient scaledX = scalb(-middleExp);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SparseGradient scaledY = y.scalb(-middleExp);
					SparseGradient scaledY = y.scalb(-middleExp);

					// compute scaled hypotenuse
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SparseGradient scaledH = scaledX.multiply(scaledX).add(scaledY.multiply(scaledY)).sqrt();
					SparseGradient scaledH = scaledX.multiply(scaledX).add(scaledY.multiply(scaledY)).sqrt();

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
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static SparseGradient hypot(final SparseGradient x, final SparseGradient y)
		public static SparseGradient hypot(SparseGradient x, SparseGradient y)
		{
			return x.hypot(y);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient reciprocal()
		{
			return new SparseGradient(1.0 / value, -1.0 / (value * value), derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient sqrt()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sqrt = org.apache.commons.math3.util.FastMath.sqrt(value);
			double sqrt = FastMath.sqrt(value);
			return new SparseGradient(sqrt, 0.5 / sqrt, derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient cbrt()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cbrt = org.apache.commons.math3.util.FastMath.cbrt(value);
			double cbrt = FastMath.cbrt(value);
			return new SparseGradient(cbrt, 1.0 / (3 * cbrt * cbrt), derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient rootN(final int n)
		public virtual SparseGradient rootN(int n)
		{
			if (n == 2)
			{
				return sqrt();
			}
			else if (n == 3)
			{
				return cbrt();
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double root = org.apache.commons.math3.util.FastMath.pow(value, 1.0 / n);
				double root = FastMath.pow(value, 1.0 / n);
				return new SparseGradient(root, 1.0 / (n * FastMath.pow(root, n - 1)), derivatives);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient pow(final double p)
		public virtual SparseGradient pow(double p)
		{
			return new SparseGradient(FastMath.pow(value, p), p * FastMath.pow(value, p - 1), derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient pow(final int n)
		public virtual SparseGradient pow(int n)
		{
			if (n == 0)
			{
				return Field.One;
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double valueNm1 = org.apache.commons.math3.util.FastMath.pow(value, n - 1);
				double valueNm1 = FastMath.pow(value, n - 1);
				return new SparseGradient(value * valueNm1, n * valueNm1, derivatives);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient pow(final SparseGradient e)
		public virtual SparseGradient pow(SparseGradient e)
		{
			return log().multiply(e).exp();
		}

		/// <summary>
		/// Compute a<sup>x</sup> where a is a double and x a <seealso cref="SparseGradient"/> </summary>
		/// <param name="a"> number to exponentiate </param>
		/// <param name="x"> power to apply </param>
		/// <returns> a<sup>x</sup> </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static SparseGradient pow(final double a, final SparseGradient x)
		public static SparseGradient pow(double a, SparseGradient x)
		{
			if (a == 0)
			{
				if (x.value == 0)
				{
					return x.compose(1.0, double.NegativeInfinity);
				}
				else if (x.value < 0)
				{
					return x.compose(double.NaN, double.NaN);
				}
				else
				{
					return x.Field.Zero;
				}
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ax = org.apache.commons.math3.util.FastMath.pow(a, x.value);
				double ax = FastMath.pow(a, x.value);
				return new SparseGradient(ax, ax * FastMath.log(a), x.derivatives);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient exp()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double e = org.apache.commons.math3.util.FastMath.exp(value);
			double e = FastMath.exp(value);
			return new SparseGradient(e, e, derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient expm1()
		{
			return new SparseGradient(FastMath.expm1(value), FastMath.exp(value), derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient log()
		{
			return new SparseGradient(FastMath.log(value), 1.0 / value, derivatives);
		}

		/// <summary>
		/// Base 10 logarithm. </summary>
		/// <returns> base 10 logarithm of the instance </returns>
		public virtual SparseGradient log10()
		{
			return new SparseGradient(FastMath.log10(value), 1.0 / (FastMath.log(10.0) * value), derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient log1p()
		{
			return new SparseGradient(FastMath.log1p(value), 1.0 / (1.0 + value), derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient cos()
		{
			return new SparseGradient(FastMath.cos(value), -FastMath.sin(value), derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient sin()
		{
			return new SparseGradient(FastMath.sin(value), FastMath.cos(value), derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient tan()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = org.apache.commons.math3.util.FastMath.tan(value);
			double t = FastMath.tan(value);
			return new SparseGradient(t, 1 + t * t, derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient acos()
		{
			return new SparseGradient(FastMath.acos(value), -1.0 / FastMath.sqrt(1 - value * value), derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient asin()
		{
			return new SparseGradient(FastMath.asin(value), 1.0 / FastMath.sqrt(1 - value * value), derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient atan()
		{
			return new SparseGradient(FastMath.atan(value), 1.0 / (1 + value * value), derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient atan2(final SparseGradient x)
		public virtual SparseGradient atan2(SparseGradient x)
		{

			// compute r = sqrt(x^2+y^2)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SparseGradient r = multiply(this).add(x.multiply(x)).sqrt();
			SparseGradient r = multiply(this).add(x.multiply(x)).sqrt();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SparseGradient a;
			SparseGradient a;
			if (x.value >= 0)
			{

				// compute atan2(y, x) = 2 atan(y / (r + x))
				a = divide(r.add(x)).atan().multiply(2);

			}
			else
			{

				// compute atan2(y, x) = +/- pi - 2 atan(y / (r - x))
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SparseGradient tmp = divide(r.subtract(x)).atan().multiply(-2);
				SparseGradient tmp = divide(r.subtract(x)).atan().multiply(-2);
				a = tmp.add(tmp.value <= 0 ? - FastMath.PI : FastMath.PI);

			}

			// fix value to take special cases (+0/+0, +0/-0, -0/+0, -0/-0, +/-infinity) correctly
			a.value = FastMath.atan2(value, x.value);

			return a;

		}

		/// <summary>
		/// Two arguments arc tangent operation. </summary>
		/// <param name="y"> first argument of the arc tangent </param>
		/// <param name="x"> second argument of the arc tangent </param>
		/// <returns> atan2(y, x) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static SparseGradient atan2(final SparseGradient y, final SparseGradient x)
		public static SparseGradient atan2(SparseGradient y, SparseGradient x)
		{
			return y.atan2(x);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient cosh()
		{
			return new SparseGradient(FastMath.cosh(value), FastMath.sinh(value), derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient sinh()
		{
			return new SparseGradient(FastMath.sinh(value), FastMath.cosh(value), derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient tanh()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = org.apache.commons.math3.util.FastMath.tanh(value);
			double t = FastMath.tanh(value);
			return new SparseGradient(t, 1 - t * t, derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient acosh()
		{
			return new SparseGradient(FastMath.acosh(value), 1.0 / FastMath.sqrt(value * value - 1.0), derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient asinh()
		{
			return new SparseGradient(FastMath.asinh(value), 1.0 / FastMath.sqrt(value * value + 1.0), derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SparseGradient atanh()
		{
			return new SparseGradient(FastMath.atanh(value), 1.0 / (1.0 - value * value), derivatives);
		}

		/// <summary>
		/// Convert radians to degrees, with error of less than 0.5 ULP </summary>
		///  <returns> instance converted into degrees </returns>
		public virtual SparseGradient toDegrees()
		{
			return new SparseGradient(FastMath.toDegrees(value), FastMath.toDegrees(1.0), derivatives);
		}

		/// <summary>
		/// Convert degrees to radians, with error of less than 0.5 ULP </summary>
		///  <returns> instance converted into radians </returns>
		public virtual SparseGradient toRadians()
		{
			return new SparseGradient(FastMath.toRadians(value), FastMath.toRadians(1.0), derivatives);
		}

		/// <summary>
		/// Evaluate Taylor expansion of a sparse gradient. </summary>
		/// <param name="delta"> parameters offsets (&Delta;x, &Delta;y, ...) </param>
		/// <returns> value of the Taylor expansion at x + &Delta;x, y + &Delta;y, ... </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double taylor(final double... delta)
		public virtual double taylor(params double[] delta)
		{
			double y = value;
			for (int i = 0; i < delta.Length; ++i)
			{
				y += delta[i] * getDerivative(i);
			}
			return y;
		}

		/// <summary>
		/// Compute composition of the instance by a univariate function. </summary>
		/// <param name="f0"> value of the function at (i.e. f(<seealso cref="#getValue()"/>)) </param>
		/// <param name="f1"> first derivative of the function at
		/// the current point (i.e. f'(<seealso cref="#getValue()"/>)) </param>
		/// <returns> f(this) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient compose(final double f0, final double f1)
		public virtual SparseGradient compose(double f0, double f1)
		{
			return new SparseGradient(f0, f1, derivatives);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SparseGradient linearCombination(final SparseGradient[] a, final SparseGradient[] b) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual SparseGradient linearCombination(SparseGradient[] a, SparseGradient[] b)
		{

			// compute a simple value, with all partial derivatives
			SparseGradient @out = a[0].Field.Zero;
			for (int i = 0; i < a.Length; ++i)
			{
				@out = @out.add(a[i].multiply(b[i]));
			}

			// recompute an accurate value, taking care of cancellations
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
			@out.value = MathArrays.linearCombination(aDouble, bDouble);

			return @out;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient linearCombination(final double[] a, final SparseGradient[] b)
		public virtual SparseGradient linearCombination(double[] a, SparseGradient[] b)
		{

			// compute a simple value, with all partial derivatives
			SparseGradient @out = b[0].Field.Zero;
			for (int i = 0; i < a.Length; ++i)
			{
				@out = @out.add(b[i].multiply(a[i]));
			}

			// recompute an accurate value, taking care of cancellations
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] bDouble = new double[b.length];
			double[] bDouble = new double[b.Length];
			for (int i = 0; i < b.Length; ++i)
			{
				bDouble[i] = b[i].Value;
			}
			@out.value = MathArrays.linearCombination(a, bDouble);

			return @out;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient linearCombination(final SparseGradient a1, final SparseGradient b1, final SparseGradient a2, final SparseGradient b2)
		public virtual SparseGradient linearCombination(SparseGradient a1, SparseGradient b1, SparseGradient a2, SparseGradient b2)
		{

			// compute a simple value, with all partial derivatives
			SparseGradient @out = a1.multiply(b1).add(a2.multiply(b2));

			// recompute an accurate value, taking care of cancellations
			@out.value = MathArrays.linearCombination(a1.value, b1.value, a2.value, b2.value);

			return @out;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient linearCombination(final double a1, final SparseGradient b1, final double a2, final SparseGradient b2)
		public virtual SparseGradient linearCombination(double a1, SparseGradient b1, double a2, SparseGradient b2)
		{

			// compute a simple value, with all partial derivatives
			SparseGradient @out = b1.multiply(a1).add(b2.multiply(a2));

			// recompute an accurate value, taking care of cancellations
			@out.value = MathArrays.linearCombination(a1, b1.value, a2, b2.value);

			return @out;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient linearCombination(final SparseGradient a1, final SparseGradient b1, final SparseGradient a2, final SparseGradient b2, final SparseGradient a3, final SparseGradient b3)
		public virtual SparseGradient linearCombination(SparseGradient a1, SparseGradient b1, SparseGradient a2, SparseGradient b2, SparseGradient a3, SparseGradient b3)
		{

			// compute a simple value, with all partial derivatives
			SparseGradient @out = a1.multiply(b1).add(a2.multiply(b2)).add(a3.multiply(b3));

			// recompute an accurate value, taking care of cancellations
			@out.value = MathArrays.linearCombination(a1.value, b1.value, a2.value, b2.value, a3.value, b3.value);

			return @out;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient linearCombination(final double a1, final SparseGradient b1, final double a2, final SparseGradient b2, final double a3, final SparseGradient b3)
		public virtual SparseGradient linearCombination(double a1, SparseGradient b1, double a2, SparseGradient b2, double a3, SparseGradient b3)
		{

			// compute a simple value, with all partial derivatives
			SparseGradient @out = b1.multiply(a1).add(b2.multiply(a2)).add(b3.multiply(a3));

			// recompute an accurate value, taking care of cancellations
			@out.value = MathArrays.linearCombination(a1, b1.value, a2, b2.value, a3, b3.value);

			return @out;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient linearCombination(final SparseGradient a1, final SparseGradient b1, final SparseGradient a2, final SparseGradient b2, final SparseGradient a3, final SparseGradient b3, final SparseGradient a4, final SparseGradient b4)
		public virtual SparseGradient linearCombination(SparseGradient a1, SparseGradient b1, SparseGradient a2, SparseGradient b2, SparseGradient a3, SparseGradient b3, SparseGradient a4, SparseGradient b4)
		{

			// compute a simple value, with all partial derivatives
			SparseGradient @out = a1.multiply(b1).add(a2.multiply(b2)).add(a3.multiply(b3)).add(a4.multiply(b4));

			// recompute an accurate value, taking care of cancellations
			@out.value = MathArrays.linearCombination(a1.value, b1.value, a2.value, b2.value, a3.value, b3.value, a4.value, b4.value);

			return @out;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SparseGradient linearCombination(final double a1, final SparseGradient b1, final double a2, final SparseGradient b2, final double a3, final SparseGradient b3, final double a4, final SparseGradient b4)
		public virtual SparseGradient linearCombination(double a1, SparseGradient b1, double a2, SparseGradient b2, double a3, SparseGradient b3, double a4, SparseGradient b4)
		{

			// compute a simple value, with all partial derivatives
			SparseGradient @out = b1.multiply(a1).add(b2.multiply(a2)).add(b3.multiply(a3)).add(b4.multiply(a4));

			// recompute an accurate value, taking care of cancellations
			@out.value = MathArrays.linearCombination(a1, b1.value, a2, b2.value, a3, b3.value, a4, b4.value);

			return @out;

		}

		/// <summary>
		/// Test for the equality of two sparse gradients.
		/// <p>
		/// Sparse gradients are considered equal if they have the same value
		/// and the same derivatives.
		/// </p> </summary>
		/// <param name="other"> Object to test for equality to this </param>
		/// <returns> true if two sparse gradients are equal </returns>
		public override bool Equals(object other)
		{

			if (this == other)
			{
				return true;
			}

			if (other is SparseGradient)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SparseGradient rhs = (SparseGradient)other;
				SparseGradient rhs = (SparseGradient)other;
				if (!Precision.Equals(value, rhs.value, 1))
				{
					return false;
				}
				if (derivatives.Count != rhs.derivatives.Count)
				{
					return false;
				}
				foreach (KeyValuePair<int?, double?> entry in derivatives)
				{
					if (!rhs.derivatives.ContainsKey(entry.Key))
					{
						return false;
					}
					if (!Precision.Equals(entry.Value, rhs.derivatives[entry.Key], 1))
					{
						return false;
					}
				}
				return true;
			}

			return false;

		}

		/// <summary>
		/// Get a hashCode for the derivative structure. </summary>
		/// <returns> a hash code value for this object
		/// @since 3.2 </returns>
		public override int GetHashCode()
		{
			return 743 + 809 * MathUtils.hash(value) + 167 * derivatives.GetHashCode();
		}

	}

}
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

namespace mathlib.stat.descriptive.moment
{

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// <p>Computes the semivariance of a set of values with respect to a given cutoff value.
	/// We define the <i>downside semivariance</i> of a set of values <code>x</code>
	/// against the <i>cutoff value</i> <code>cutoff</code> to be <br/>
	/// <code>&Sigma; (x[i] - target)<sup>2</sup> / df</code> <br/>
	/// where the sum is taken over all <code>i</code> such that <code>x[i] < cutoff</code>
	/// and <code>df</code> is the length of <code>x</code> (non-bias-corrected) or
	/// one less than this number (bias corrected).  The <i>upside semivariance</i>
	/// is defined similarly, with the sum taken over values of <code>x</code> that
	/// exceed the cutoff value.</p>
	/// 
	/// <p>The cutoff value defaults to the mean, bias correction defaults to <code>true</code>
	/// and the "variance direction" (upside or downside) defaults to downside.  The variance direction
	/// and bias correction may be set using property setters or their values can provided as
	/// parameters to <seealso cref="#evaluate(double[], double, Direction, boolean, int, int)"/>.</p>
	/// 
	/// <p>If the input array is null, <code>evaluate</code> methods throw
	/// <code>IllegalArgumentException.</code>  If the array has length 1, <code>0</code>
	/// is returned, regardless of the value of the <code>cutoff.</code>
	/// 
	/// <p><strong>Note that this class is not intended to be threadsafe.</strong> If
	/// multiple threads access an instance of this class concurrently, and one or
	/// more of these threads invoke property setters, external synchronization must
	/// be provided to ensure correct results.</p>
	/// 
	/// @since 2.1
	/// @version $Id: SemiVariance.java 1385386 2012-09-16 22:11:15Z psteitz $
	/// </summary>
	[Serializable]
	public class SemiVariance : AbstractUnivariateStatistic
	{

		/// <summary>
		/// The UPSIDE Direction is used to specify that the observations above the
		/// cutoff point will be used to calculate SemiVariance.
		/// </summary>
		public const Direction UPSIDE_VARIANCE = Direction.UPSIDE;

		/// <summary>
		/// The DOWNSIDE Direction is used to specify that the observations below
		/// the cutoff point will be used to calculate SemiVariance
		/// </summary>
		public const Direction DOWNSIDE_VARIANCE = Direction.DOWNSIDE;

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -2653430366886024994L;

		/// <summary>
		/// Determines whether or not bias correction is applied when computing the
		/// value of the statisic.  True means that bias is corrected.
		/// </summary>
		private bool biasCorrected = true;

		/// <summary>
		/// Determines whether to calculate downside or upside SemiVariance.
		/// </summary>
		private Direction varianceDirection = Direction.DOWNSIDE;

		/// <summary>
		/// Constructs a SemiVariance with default (true) <code>biasCorrected</code>
		/// property and default (Downside) <code>varianceDirection</code> property.
		/// </summary>
		public SemiVariance()
		{
		}

		/// <summary>
		/// Constructs a SemiVariance with the specified <code>biasCorrected</code>
		/// property and default (Downside) <code>varianceDirection</code> property.
		/// </summary>
		/// <param name="biasCorrected">  setting for bias correction - true means
		/// bias will be corrected and is equivalent to using the argumentless
		/// constructor </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SemiVariance(final boolean biasCorrected)
		public SemiVariance(bool biasCorrected)
		{
			this.biasCorrected = biasCorrected;
		}


		/// <summary>
		/// Constructs a SemiVariance with the specified <code>Direction</code> property
		/// and default (true) <code>biasCorrected</code> property
		/// </summary>
		/// <param name="direction">  setting for the direction of the SemiVariance
		/// to calculate </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SemiVariance(final Direction direction)
		public SemiVariance(Direction direction)
		{
			this.varianceDirection = direction;
		}


		/// <summary>
		/// Constructs a SemiVariance with the specified <code>isBiasCorrected</code>
		/// property and the specified <code>Direction</code> property.
		/// </summary>
		/// <param name="corrected">  setting for bias correction - true means
		/// bias will be corrected and is equivalent to using the argumentless
		/// constructor
		/// </param>
		/// <param name="direction">  setting for the direction of the SemiVariance
		/// to calculate </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SemiVariance(final boolean corrected, final Direction direction)
		public SemiVariance(bool corrected, Direction direction)
		{
			this.biasCorrected = corrected;
			this.varianceDirection = direction;
		}


		/// <summary>
		/// Copy constructor, creates a new {@code SemiVariance} identical
		/// to the {@code original}
		/// </summary>
		/// <param name="original"> the {@code SemiVariance} instance to copy </param>
		/// <exception cref="NullArgumentException">  if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SemiVariance(final SemiVariance original) throws mathlib.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public SemiVariance(SemiVariance original)
		{
			copy(original, this);
		}


		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override SemiVariance copy()
		{
			SemiVariance result = new SemiVariance();
			// No try-catch or advertised exception because args are guaranteed non-null
			copy(this, result);
			return result;
		}


		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> SemiVariance to copy </param>
		/// <param name="dest"> SemiVariance to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(final SemiVariance source, SemiVariance dest) throws mathlib.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static void copy(SemiVariance source, SemiVariance dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			dest.Data = source.DataRef;
			dest.biasCorrected = source.biasCorrected;
			dest.varianceDirection = source.varianceDirection;
		}

		/// <summary>
		/// <p>Returns the <seealso cref="SemiVariance"/> of the designated values against the mean, using
		/// instance properties varianceDirection and biasCorrection.</p>
		///  
		/// <p>Returns <code>NaN</code> if the array is empty and throws
		/// <code>IllegalArgumentException</code> if the array is null.</p>
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="start"> index of the first array element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the SemiVariance </returns>
		/// <exception cref="MathIllegalArgumentException"> if the parameters are not valid
		///   </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double evaluate(final double[] values, final int start, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		  public override double evaluate(double[] values, int start, int length)
		  {
			double m = (new Mean()).evaluate(values, start, length);
			return evaluate(values, m, varianceDirection, biasCorrected, 0, values.Length);
		  }


		  /// <summary>
		  /// This method calculates <seealso cref="SemiVariance"/> for the entire array against the mean, using
		  /// the current value of the biasCorrection instance property.
		  /// </summary>
		  /// <param name="values"> the input array </param>
		  /// <param name="direction"> the <seealso cref="Direction"/> of the semivariance </param>
		  /// <returns> the SemiVariance </returns>
		  /// <exception cref="MathIllegalArgumentException"> if values is null
		  ///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, Direction direction) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		  public virtual double evaluate(double[] values, Direction direction)
		  {
			  double m = (new Mean()).evaluate(values);
			  return evaluate(values, m, direction, biasCorrected, 0, values.Length);
		  }

		  /// <summary>
		  /// <p>Returns the <seealso cref="SemiVariance"/> of the designated values against the cutoff, using
		  /// instance properties variancDirection and biasCorrection.</p>
		  /// 
		  /// <p>Returns <code>NaN</code> if the array is empty and throws
		  /// <code>MathIllegalArgumentException</code> if the array is null.</p>
		  /// </summary>
		  /// <param name="values"> the input array </param>
		  /// <param name="cutoff"> the reference point </param>
		  /// <returns> the SemiVariance </returns>
		  /// <exception cref="MathIllegalArgumentException"> if values is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double cutoff) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		  public virtual double evaluate(double[] values, double cutoff)
		  {
			  return evaluate(values, cutoff, varianceDirection, biasCorrected, 0, values.Length);
		  }

		  /// <summary>
		  /// <p>Returns the <seealso cref="SemiVariance"/> of the designated values against the cutoff in the
		  /// given direction, using the current value of the biasCorrection instance property.</p>
		  /// 
		  /// <p>Returns <code>NaN</code> if the array is empty and throws
		  /// <code>MathIllegalArgumentException</code> if the array is null.</p>
		  /// </summary>
		  /// <param name="values"> the input array </param>
		  /// <param name="cutoff"> the reference point </param>
		  /// <param name="direction"> the <seealso cref="Direction"/> of the semivariance </param>
		  /// <returns> the SemiVariance </returns>
		  /// <exception cref="MathIllegalArgumentException"> if values is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double cutoff, final Direction direction) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		  public virtual double evaluate(double[] values, double cutoff, Direction direction)
		  {
			  return evaluate(values, cutoff, direction, biasCorrected, 0, values.Length);
		  }


		 /// <summary>
		 /// <p>Returns the <seealso cref="SemiVariance"/> of the designated values against the cutoff
		 /// in the given direction with the provided bias correction.</p>
		 /// 
		 /// <p>Returns <code>NaN</code> if the array is empty and throws
		 /// <code>IllegalArgumentException</code> if the array is null.</p>
		 /// </summary>
		 /// <param name="values"> the input array </param>
		 /// <param name="cutoff"> the reference point </param>
		 /// <param name="direction"> the <seealso cref="Direction"/> of the semivariance </param>
		 /// <param name="corrected"> the BiasCorrection flag </param>
		 /// <param name="start"> index of the first array element to include </param>
		 /// <param name="length"> the number of elements to include </param>
		 /// <returns> the SemiVariance </returns>
		 /// <exception cref="MathIllegalArgumentException"> if the parameters are not valid
		 ///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(final double[] values, final double cutoff, final Direction direction, final boolean corrected, final int start, final int length) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double evaluate(double[] values, double cutoff, Direction direction, bool corrected, int start, int length)
		{

			test(values, start, length);
			if (values.Length == 0)
			{
				return double.NaN;
			}
			else
			{
				if (values.Length == 1)
				{
					return 0.0;
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean booleanDirection = direction.getDirection();
					bool booleanDirection = direction.Direction;

					double dev = 0.0;
					double sumsq = 0.0;
					for (int i = start; i < length; i++)
					{
						if ((values[i] > cutoff) == booleanDirection)
						{
						   dev = values[i] - cutoff;
						   sumsq += dev * dev;
						}
					}

					if (corrected)
					{
						return sumsq / (length - 1.0);
					}
					else
					{
						return sumsq / length;
					}
				}
			}
		}

		/// <summary>
		/// Returns true iff biasCorrected property is set to true.
		/// </summary>
		/// <returns> the value of biasCorrected. </returns>
		public virtual bool BiasCorrected
		{
			get
			{
				return biasCorrected;
			}
			set
			{
				this.biasCorrected = value;
			}
		}


		/// <summary>
		/// Returns the varianceDirection property.
		/// </summary>
		/// <returns> the varianceDirection </returns>
		public virtual Direction VarianceDirection
		{
			get
			{
				return varianceDirection;
			}
			set
			{
				this.varianceDirection = value;
			}
		}


		/// <summary>
		/// The direction of the semivariance - either upside or downside. The direction
		/// is represented by boolean, with true corresponding to UPSIDE semivariance.
		/// </summary>
		public enum Direction
		{
			/// <summary>
			/// The UPSIDE Direction is used to specify that the observations above the
			/// cutoff point will be used to calculate SemiVariance
			/// </summary>
			UPSIDE = true,

			/// <summary>
			/// The DOWNSIDE Direction is used to specify that the observations below
			/// the cutoff point will be used to calculate SemiVariance
			/// </summary>
			DOWNSIDE = false

			/// <summary>
			///   boolean value  UPSIDE <-> true
			/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain fields in .NET:
//			private boolean direction;

			/// <summary>
			/// Create a Direction with the given value.
			/// </summary>
			/// <param name="b"> boolean value representing the Direction. True corresponds to UPSIDE. </param>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain methods in .NET:
//			Direction(boolean b)
	//		{
	//			direction = b;
	//		}

			/// <summary>
			/// Returns the value of this Direction. True corresponds to UPSIDE.
			/// </summary>
			/// <returns> true if direction is UPSIDE; false otherwise </returns>
		}
	public static partial class EnumExtensionMethods
	{
			internal bool getDirection(this Direction instanceJavaToDotNetTempPropertyGetDirection)
			{
				return direction;
			}
	}
	}

}
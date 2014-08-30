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

namespace org.apache.commons.math3.distribution
{

	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using Beta = org.apache.commons.math3.special.Beta;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using RandomGenerator = org.apache.commons.math3.random.RandomGenerator;
	using Well19937c = org.apache.commons.math3.random.Well19937c;

	/// <summary>
	/// Implementation of the F-distribution.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/F-distribution">F-distribution (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/F-Distribution.html">F-distribution (MathWorld)</a>
	/// @version $Id: FDistribution.java 1534358 2013-10-21 20:13:52Z tn $ </seealso>
	public class FDistribution : AbstractRealDistribution
	{
		/// <summary>
		/// Default inverse cumulative probability accuracy.
		/// @since 2.1
		/// </summary>
		public const double DEFAULT_INVERSE_ABSOLUTE_ACCURACY = 1e-9;
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -8516354193418641566L;
		/// <summary>
		/// The numerator degrees of freedom. </summary>
		private readonly double numeratorDegreesOfFreedom;
		/// <summary>
		/// The numerator degrees of freedom. </summary>
		private readonly double denominatorDegreesOfFreedom;
		/// <summary>
		/// Inverse cumulative probability accuracy. </summary>
		private readonly double solverAbsoluteAccuracy;
		/// <summary>
		/// Cached numerical variance </summary>
		private double numericalVariance = double.NaN;
		/// <summary>
		/// Whether or not the numerical variance has been calculated </summary>
		private bool numericalVarianceIsCalculated = false;

		/// <summary>
		/// Creates an F distribution using the given degrees of freedom.
		/// </summary>
		/// <param name="numeratorDegreesOfFreedom"> Numerator degrees of freedom. </param>
		/// <param name="denominatorDegreesOfFreedom"> Denominator degrees of freedom. </param>
		/// <exception cref="NotStrictlyPositiveException"> if
		/// {@code numeratorDegreesOfFreedom <= 0} or
		/// {@code denominatorDegreesOfFreedom <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FDistribution(double numeratorDegreesOfFreedom, double denominatorDegreesOfFreedom) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public FDistribution(double numeratorDegreesOfFreedom, double denominatorDegreesOfFreedom) : this(numeratorDegreesOfFreedom, denominatorDegreesOfFreedom, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Creates an F distribution using the given degrees of freedom
		/// and inverse cumulative probability accuracy.
		/// </summary>
		/// <param name="numeratorDegreesOfFreedom"> Numerator degrees of freedom. </param>
		/// <param name="denominatorDegreesOfFreedom"> Denominator degrees of freedom. </param>
		/// <param name="inverseCumAccuracy"> the maximum absolute error in inverse
		/// cumulative probability estimates. </param>
		/// <exception cref="NotStrictlyPositiveException"> if
		/// {@code numeratorDegreesOfFreedom <= 0} or
		/// {@code denominatorDegreesOfFreedom <= 0}.
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FDistribution(double numeratorDegreesOfFreedom, double denominatorDegreesOfFreedom, double inverseCumAccuracy) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public FDistribution(double numeratorDegreesOfFreedom, double denominatorDegreesOfFreedom, double inverseCumAccuracy) : this(new Well19937c(), numeratorDegreesOfFreedom, denominatorDegreesOfFreedom, inverseCumAccuracy)
		{
		}

		/// <summary>
		/// Creates an F distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="numeratorDegreesOfFreedom"> Numerator degrees of freedom. </param>
		/// <param name="denominatorDegreesOfFreedom"> Denominator degrees of freedom. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code numeratorDegreesOfFreedom <= 0} or
		/// {@code denominatorDegreesOfFreedom <= 0}.
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FDistribution(org.apache.commons.math3.random.RandomGenerator rng, double numeratorDegreesOfFreedom, double denominatorDegreesOfFreedom) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public FDistribution(RandomGenerator rng, double numeratorDegreesOfFreedom, double denominatorDegreesOfFreedom) : this(rng, numeratorDegreesOfFreedom, denominatorDegreesOfFreedom, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Creates an F distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="numeratorDegreesOfFreedom"> Numerator degrees of freedom. </param>
		/// <param name="denominatorDegreesOfFreedom"> Denominator degrees of freedom. </param>
		/// <param name="inverseCumAccuracy"> the maximum absolute error in inverse
		/// cumulative probability estimates. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code numeratorDegreesOfFreedom <= 0} or
		/// {@code denominatorDegreesOfFreedom <= 0}.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FDistribution(org.apache.commons.math3.random.RandomGenerator rng, double numeratorDegreesOfFreedom, double denominatorDegreesOfFreedom, double inverseCumAccuracy) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public FDistribution(RandomGenerator rng, double numeratorDegreesOfFreedom, double denominatorDegreesOfFreedom, double inverseCumAccuracy) : base(rng)
		{

			if (numeratorDegreesOfFreedom <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.DEGREES_OF_FREEDOM, numeratorDegreesOfFreedom);
			}
			if (denominatorDegreesOfFreedom <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.DEGREES_OF_FREEDOM, denominatorDegreesOfFreedom);
			}
			this.numeratorDegreesOfFreedom = numeratorDegreesOfFreedom;
			this.denominatorDegreesOfFreedom = denominatorDegreesOfFreedom;
			solverAbsoluteAccuracy = inverseCumAccuracy;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// @since 2.1
		/// </summary>
		public override double density(double x)
		{
			return FastMath.exp(logDensity(x));
		}

		/// <summary>
		/// {@inheritDoc} * </summary>
		public override double logDensity(double x)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double nhalf = numeratorDegreesOfFreedom / 2;
			double nhalf = numeratorDegreesOfFreedom / 2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double mhalf = denominatorDegreesOfFreedom / 2;
			double mhalf = denominatorDegreesOfFreedom / 2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double logx = org.apache.commons.math3.util.FastMath.log(x);
			double logx = FastMath.log(x);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double logn = org.apache.commons.math3.util.FastMath.log(numeratorDegreesOfFreedom);
			double logn = FastMath.log(numeratorDegreesOfFreedom);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double logm = org.apache.commons.math3.util.FastMath.log(denominatorDegreesOfFreedom);
			double logm = FastMath.log(denominatorDegreesOfFreedom);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lognxm = org.apache.commons.math3.util.FastMath.log(numeratorDegreesOfFreedom * x + denominatorDegreesOfFreedom);
			double lognxm = FastMath.log(numeratorDegreesOfFreedom * x + denominatorDegreesOfFreedom);
			return nhalf * logn + nhalf * logx - logx + mhalf * logm - nhalf * lognxm - mhalf * lognxm - Beta.logBeta(nhalf, mhalf);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The implementation of this method is based on
		/// <ul>
		///  <li>
		///   <a href="http://mathworld.wolfram.com/F-Distribution.html">
		///   F-Distribution</a>, equation (4).
		///  </li>
		/// </ul>
		/// </summary>
		public override double cumulativeProbability(double x)
		{
			double ret;
			if (x <= 0)
			{
				ret = 0;
			}
			else
			{
				double n = numeratorDegreesOfFreedom;
				double m = denominatorDegreesOfFreedom;

				ret = Beta.regularizedBeta((n * x) / (m + n * x), 0.5 * n, 0.5 * m);
			}
			return ret;
		}

		/// <summary>
		/// Access the numerator degrees of freedom.
		/// </summary>
		/// <returns> the numerator degrees of freedom. </returns>
		public virtual double NumeratorDegreesOfFreedom
		{
			get
			{
				return numeratorDegreesOfFreedom;
			}
		}

		/// <summary>
		/// Access the denominator degrees of freedom.
		/// </summary>
		/// <returns> the denominator degrees of freedom. </returns>
		public virtual double DenominatorDegreesOfFreedom
		{
			get
			{
				return denominatorDegreesOfFreedom;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override double SolverAbsoluteAccuracy
		{
			get
			{
				return solverAbsoluteAccuracy;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For denominator degrees of freedom parameter {@code b}, the mean is
		/// <ul>
		///  <li>if {@code b > 2} then {@code b / (b - 2)},</li>
		///  <li>else undefined ({@code Double.NaN}).
		/// </ul>
		/// </summary>
		public override double NumericalMean
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double denominatorDF = getDenominatorDegreesOfFreedom();
				double denominatorDF = DenominatorDegreesOfFreedom;
    
				if (denominatorDF > 2)
				{
					return denominatorDF / (denominatorDF - 2);
				}
    
				return double.NaN;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For numerator degrees of freedom parameter {@code a} and denominator
		/// degrees of freedom parameter {@code b}, the variance is
		/// <ul>
		///  <li>
		///    if {@code b > 4} then
		///    {@code [2 * b^2 * (a + b - 2)] / [a * (b - 2)^2 * (b - 4)]},
		///  </li>
		///  <li>else undefined ({@code Double.NaN}).
		/// </ul>
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
				if (!numericalVarianceIsCalculated)
				{
					numericalVariance = calculateNumericalVariance();
					numericalVarianceIsCalculated = true;
				}
				return numericalVariance;
			}
		}

		/// <summary>
		/// used by <seealso cref="#getNumericalVariance()"/>
		/// </summary>
		/// <returns> the variance of this distribution </returns>
		protected internal virtual double calculateNumericalVariance()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double denominatorDF = getDenominatorDegreesOfFreedom();
			double denominatorDF = DenominatorDegreesOfFreedom;

			if (denominatorDF > 4)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double numeratorDF = getNumeratorDegreesOfFreedom();
				double numeratorDF = NumeratorDegreesOfFreedom;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double denomDFMinusTwo = denominatorDF - 2;
				double denomDFMinusTwo = denominatorDF - 2;

				return (2 * (denominatorDF * denominatorDF) * (numeratorDF + denominatorDF - 2)) / ((numeratorDF * (denomDFMinusTwo * denomDFMinusTwo) * (denominatorDF - 4)));
			}

			return double.NaN;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The lower bound of the support is always 0 no matter the parameters.
		/// </summary>
		/// <returns> lower bound of the support (always 0) </returns>
		public override double SupportLowerBound
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The upper bound of the support is always positive infinity
		/// no matter the parameters.
		/// </summary>
		/// <returns> upper bound of the support (always Double.POSITIVE_INFINITY) </returns>
		public override double SupportUpperBound
		{
			get
			{
				return double.PositiveInfinity;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool SupportLowerBoundInclusive
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool SupportUpperBoundInclusive
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The support of this distribution is connected.
		/// </summary>
		/// <returns> {@code true} </returns>
		public override bool SupportConnected
		{
			get
			{
				return true;
			}
		}
	}

}
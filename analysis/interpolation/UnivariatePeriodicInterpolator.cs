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
namespace mathlib.analysis.interpolation
{

	using MathUtils = mathlib.util.MathUtils;
	using MathArrays = mathlib.util.MathArrays;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NonMonotonicSequenceException = mathlib.exception.NonMonotonicSequenceException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;

	/// <summary>
	/// Adapter for classes implementing the <seealso cref="UnivariateInterpolator"/>
	/// interface.
	/// The data to be interpolated is assumed to be periodic. Thus values that are
	/// outside of the range can be passed to the interpolation function: They will
	/// be wrapped into the initial range before being passed to the class that
	/// actually computes the interpolation.
	/// 
	/// @version $Id: UnivariatePeriodicInterpolator.java 1459739 2013-03-22 11:58:11Z erans $
	/// </summary>
	public class UnivariatePeriodicInterpolator : UnivariateInterpolator
	{
		/// <summary>
		/// Default number of extension points of the samples array. </summary>
		public const int DEFAULT_EXTEND = 5;
		/// <summary>
		/// Interpolator. </summary>
		private readonly UnivariateInterpolator interpolator;
		/// <summary>
		/// Period. </summary>
		private readonly double period;
		/// <summary>
		/// Number of extension points. </summary>
		private readonly int extend;

		/// <summary>
		/// Builds an interpolator.
		/// </summary>
		/// <param name="interpolator"> Interpolator. </param>
		/// <param name="period"> Period. </param>
		/// <param name="extend"> Number of points to be appended at the beginning and
		/// end of the sample arrays in order to avoid interpolation failure at
		/// the (periodic) boundaries of the orginal interval. The value is the
		/// number of sample points which the original {@code interpolator} needs
		/// on each side of the interpolated point. </param>
		public UnivariatePeriodicInterpolator(UnivariateInterpolator interpolator, double period, int extend)
		{
			this.interpolator = interpolator;
			this.period = period;
			this.extend = extend;
		}

		/// <summary>
		/// Builds an interpolator.
		/// Uses <seealso cref="#DEFAULT_EXTEND"/> as the number of extension points on each side
		/// of the original abscissae range.
		/// </summary>
		/// <param name="interpolator"> Interpolator. </param>
		/// <param name="period"> Period. </param>
		public UnivariatePeriodicInterpolator(UnivariateInterpolator interpolator, double period) : this(interpolator, period, DEFAULT_EXTEND)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="NumberIsTooSmallException"> if the number of extension points
		/// is larger than the size of {@code xval}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public mathlib.analysis.UnivariateFunction interpolate(double[] xval, double[] yval) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.NonMonotonicSequenceException
		public virtual UnivariateFunction interpolate(double[] xval, double[] yval)
		{
			if (xval.Length < extend)
			{
				throw new NumberIsTooSmallException(xval.Length, extend, true);
			}

			MathArrays.checkOrder(xval);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double offset = xval[0];
			double offset = xval[0];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = xval.length + extend * 2;
			int len = xval.Length + extend * 2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] x = new double[len];
			double[] x = new double[len];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y = new double[len];
			double[] y = new double[len];
			for (int i = 0; i < xval.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = i + extend;
				int index = i + extend;
				x[index] = MathUtils.reduce(xval[i], period, offset);
				y[index] = yval[i];
			}

			// Wrap to enable interpolation at the boundaries.
			for (int i = 0; i < extend; i++)
			{
				int index = xval.Length - extend + i;
				x[i] = MathUtils.reduce(xval[index], period, offset) - period;
				y[i] = yval[index];

				index = len - extend + i;
				x[index] = MathUtils.reduce(xval[i], period, offset) + period;
				y[index] = yval[i];
			}

			MathArrays.sortInPlace(x, y);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.analysis.UnivariateFunction f = interpolator.interpolate(x, y);
			UnivariateFunction f = interpolator.interpolate(x, y);
			return new UnivariateFunctionAnonymousInnerClassHelper(this, offset, x, f);
		}

		private class UnivariateFunctionAnonymousInnerClassHelper : UnivariateFunction
		{
			private readonly UnivariatePeriodicInterpolator outerInstance;

			private double offset;
			private double[] x;
			private UnivariateFunction f;

			public UnivariateFunctionAnonymousInnerClassHelper(UnivariatePeriodicInterpolator outerInstance, double offset, double[] x, UnivariateFunction f)
			{
				this.outerInstance = outerInstance;
				this.offset = offset;
				this.x = x;
				this.f = f;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double value(final double x) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			public virtual double value(double x)
			{
				return f.value(MathUtils.reduce(x, outerInstance.period, offset));
			}
		}
	}

}
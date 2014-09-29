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

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NotPositiveException = mathlib.exception.NotPositiveException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Univariate functions differentiator using finite differences.
	/// <p>
	/// This class creates some wrapper objects around regular
	/// <seealso cref="UnivariateFunction univariate functions"/> (or {@link
	/// UnivariateVectorFunction univariate vector functions} or {@link
	/// UnivariateMatrixFunction univariate matrix functions}). These
	/// wrapper objects compute derivatives in addition to function
	/// value.
	/// </p>
	/// <p>
	/// The wrapper objects work by calling the underlying function on
	/// a sampling grid around the current point and performing polynomial
	/// interpolation. A finite differences scheme with n points is
	/// theoretically able to compute derivatives up to order n-1, but
	/// it is generally better to have a slight margin. The step size must
	/// also be small enough in order for the polynomial approximation to
	/// be good in the current point neighborhood, but it should not be too
	/// small because numerical instability appears quickly (there are several
	/// differences of close points). Choosing the number of points and
	/// the step size is highly problem dependent.
	/// </p>
	/// <p>
	/// As an example of good and bad settings, lets consider the quintic
	/// polynomial function {@code f(x) = (x-1)*(x-0.5)*x*(x+0.5)*(x+1)}.
	/// Since it is a polynomial, finite differences with at least 6 points
	/// should theoretically recover the exact same polynomial and hence
	/// compute accurate derivatives for any order. However, due to numerical
	/// errors, we get the following results for a 7 points finite differences
	/// for abscissae in the [-10, 10] range:
	/// <ul>
	///   <li>step size = 0.25, second order derivative error about 9.97e-10</li>
	///   <li>step size = 0.25, fourth order derivative error about 5.43e-8</li>
	///   <li>step size = 1.0e-6, second order derivative error about 148</li>
	///   <li>step size = 1.0e-6, fourth order derivative error about 6.35e+14</li>
	/// </ul>
	/// This example shows that the small step size is really bad, even simply
	/// for second order derivative!
	/// </p>
	/// @version $Id: FiniteDifferencesDifferentiator.java 1420666 2012-12-12 13:33:20Z erans $
	/// @since 3.1
	/// </summary>
	[Serializable]
	public class FiniteDifferencesDifferentiator : UnivariateFunctionDifferentiator, UnivariateVectorFunctionDifferentiator, UnivariateMatrixFunctionDifferentiator
	{

		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 20120917L;

		/// <summary>
		/// Number of points to use. </summary>
		private readonly int nbPoints;

		/// <summary>
		/// Step size. </summary>
		private readonly double stepSize;

		/// <summary>
		/// Half sample span. </summary>
		private readonly double halfSampleSpan;

		/// <summary>
		/// Lower bound for independent variable. </summary>
		private readonly double tMin;

		/// <summary>
		/// Upper bound for independent variable. </summary>
		private readonly double tMax;

		/// <summary>
		/// Build a differentiator with number of points and step size when independent variable is unbounded.
		/// <p>
		/// Beware that wrong settings for the finite differences differentiator
		/// can lead to highly unstable and inaccurate results, especially for
		/// high derivation orders. Using very small step sizes is often a
		/// <em>bad</em> idea.
		/// </p> </summary>
		/// <param name="nbPoints"> number of points to use </param>
		/// <param name="stepSize"> step size (gap between each point) </param>
		/// <exception cref="NotPositiveException"> if {@code stepsize <= 0} (note that
		/// <seealso cref="NotPositiveException"/> extends <seealso cref="NumberIsTooSmallException"/>) </exception>
		/// <exception cref="NumberIsTooSmallException"> {@code nbPoint <= 1} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FiniteDifferencesDifferentiator(final int nbPoints, final double stepSize) throws mathlib.exception.NotPositiveException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public FiniteDifferencesDifferentiator(int nbPoints, double stepSize) : this(nbPoints, stepSize, double.NegativeInfinity, double.PositiveInfinity)
		{
		}

		/// <summary>
		/// Build a differentiator with number of points and step size when independent variable is bounded.
		/// <p>
		/// When the independent variable is bounded (tLower &lt; t &lt; tUpper), the sampling
		/// points used for differentiation will be adapted to ensure the constraint holds
		/// even near the boundaries. This means the sample will not be centered anymore in
		/// these cases. At an extreme case, computing derivatives exactly at the lower bound
		/// will lead the sample to be entirely on the right side of the derivation point.
		/// </p>
		/// <p>
		/// Note that the boundaries are considered to be excluded for function evaluation.
		/// </p>
		/// <p>
		/// Beware that wrong settings for the finite differences differentiator
		/// can lead to highly unstable and inaccurate results, especially for
		/// high derivation orders. Using very small step sizes is often a
		/// <em>bad</em> idea.
		/// </p> </summary>
		/// <param name="nbPoints"> number of points to use </param>
		/// <param name="stepSize"> step size (gap between each point) </param>
		/// <param name="tLower"> lower bound for independent variable (may be {@code Double.NEGATIVE_INFINITY}
		/// if there are no lower bounds) </param>
		/// <param name="tUpper"> upper bound for independent variable (may be {@code Double.POSITIVE_INFINITY}
		/// if there are no upper bounds) </param>
		/// <exception cref="NotPositiveException"> if {@code stepsize <= 0} (note that
		/// <seealso cref="NotPositiveException"/> extends <seealso cref="NumberIsTooSmallException"/>) </exception>
		/// <exception cref="NumberIsTooSmallException"> {@code nbPoint <= 1} </exception>
		/// <exception cref="NumberIsTooLargeException"> {@code stepSize * (nbPoints - 1) >= tUpper - tLower} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FiniteDifferencesDifferentiator(final int nbPoints, final double stepSize, final double tLower, final double tUpper) throws mathlib.exception.NotPositiveException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public FiniteDifferencesDifferentiator(int nbPoints, double stepSize, double tLower, double tUpper)
		{

			if (nbPoints <= 1)
			{
				throw new NumberIsTooSmallException(stepSize, 1, false);
			}
			this.nbPoints = nbPoints;

			if (stepSize <= 0)
			{
				throw new NotPositiveException(stepSize);
			}
			this.stepSize = stepSize;

			halfSampleSpan = 0.5 * stepSize * (nbPoints - 1);
			if (2 * halfSampleSpan >= tUpper - tLower)
			{
				throw new NumberIsTooLargeException(2 * halfSampleSpan, tUpper - tLower, false);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double safety = mathlib.util.FastMath.ulp(halfSampleSpan);
			double safety = FastMath.ulp(halfSampleSpan);
			this.tMin = tLower + halfSampleSpan + safety;
			this.tMax = tUpper - halfSampleSpan - safety;

		}

		/// <summary>
		/// Get the number of points to use. </summary>
		/// <returns> number of points to use </returns>
		public virtual int NbPoints
		{
			get
			{
				return nbPoints;
			}
		}

		/// <summary>
		/// Get the step size. </summary>
		/// <returns> step size </returns>
		public virtual double StepSize
		{
			get
			{
				return stepSize;
			}
		}

		/// <summary>
		/// Evaluate derivatives from a sample.
		/// <p>
		/// Evaluation is done using divided differences.
		/// </p> </summary>
		/// <param name="t"> evaluation abscissa value and derivatives </param>
		/// <param name="t0"> first sample point abscissa </param>
		/// <param name="y"> function values sample {@code y[i] = f(t[i]) = f(t0 + i * stepSize)} </param>
		/// <returns> value and derivatives at {@code t} </returns>
		/// <exception cref="NumberIsTooLargeException"> if the requested derivation order
		/// is larger or equal to the number of points </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private DerivativeStructure evaluate(final DerivativeStructure t, final double t0, final double[] y) throws mathlib.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private DerivativeStructure evaluate(DerivativeStructure t, double t0, double[] y)
		{

			// create divided differences diagonal arrays
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] top = new double[nbPoints];
			double[] top = new double[nbPoints];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] bottom = new double[nbPoints];
			double[] bottom = new double[nbPoints];

			for (int i = 0; i < nbPoints; ++i)
			{

				// update the bottom diagonal of the divided differences array
				bottom[i] = y[i];
				for (int j = 1; j <= i; ++j)
				{
					bottom[i - j] = (bottom[i - j + 1] - bottom[i - j]) / (j * stepSize);
				}

				// update the top diagonal of the divided differences array
				top[i] = bottom[0];

			}

			// evaluate interpolation polynomial (represented by top diagonal) at t
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int order = t.getOrder();
			int order = t.Order;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int parameters = t.getFreeParameters();
			int parameters = t.FreeParameters;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] derivatives = t.getAllDerivatives();
			double[] derivatives = t.AllDerivatives;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dt0 = t.getValue() - t0;
			double dt0 = t.Value - t0;
			DerivativeStructure interpolation = new DerivativeStructure(parameters, order, 0.0);
			DerivativeStructure monomial = null;
			for (int i = 0; i < nbPoints; ++i)
			{
				if (i == 0)
				{
					// start with monomial(t) = 1
					monomial = new DerivativeStructure(parameters, order, 1.0);
				}
				else
				{
					// monomial(t) = (t - t0) * (t - t1) * ... * (t - t(i-1))
					derivatives[0] = dt0 - (i - 1) * stepSize;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure deltaX = new DerivativeStructure(parameters, order, derivatives);
					DerivativeStructure deltaX = new DerivativeStructure(parameters, order, derivatives);
					monomial = monomial.multiply(deltaX);
				}
				interpolation = interpolation.add(monomial.multiply(top[i]));
			}

			return interpolation;

		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>The returned object cannot compute derivatives to arbitrary orders. The
		/// value function will throw a <seealso cref="NumberIsTooLargeException"/> if the requested
		/// derivation order is larger or equal to the number of points.
		/// </p>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public UnivariateDifferentiableFunction differentiate(final mathlib.analysis.UnivariateFunction function)
		public virtual UnivariateDifferentiableFunction differentiate(UnivariateFunction function)
		{
			return new UnivariateDifferentiableFunctionAnonymousInnerClassHelper(this, function);
		}

		private class UnivariateDifferentiableFunctionAnonymousInnerClassHelper : UnivariateDifferentiableFunction
		{
			private readonly FiniteDifferencesDifferentiator outerInstance;

			private UnivariateFunction function;

			public UnivariateDifferentiableFunctionAnonymousInnerClassHelper(FiniteDifferencesDifferentiator outerInstance, UnivariateFunction function)
			{
				this.outerInstance = outerInstance;
				this.function = function;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double value(final double x) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			public virtual double value(double x)
			{
				return function.value(x);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure value(final DerivativeStructure t) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			public virtual DerivativeStructure value(DerivativeStructure t)
			{

				// check we can achieve the requested derivation order with the sample
				if (t.Order >= outerInstance.nbPoints)
				{
					throw new NumberIsTooLargeException(t.Order, outerInstance.nbPoints, false);
				}

				// compute sample position, trying to be centered if possible
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t0 = mathlib.util.FastMath.max(mathlib.util.FastMath.min(t.getValue(), tMax), tMin) - halfSampleSpan;
				double t0 = FastMath.max(FastMath.min(t.Value, outerInstance.tMax), outerInstance.tMin) - outerInstance.halfSampleSpan;

				// compute sample points
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y = new double[nbPoints];
				double[] y = new double[outerInstance.nbPoints];
				for (int i = 0; i < outerInstance.nbPoints; ++i)
				{
					y[i] = function.value(t0 + i * outerInstance.stepSize);
				}

				// evaluate derivatives
				return outerInstance.evaluate(t, t0, y);

			}

		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>The returned object cannot compute derivatives to arbitrary orders. The
		/// value function will throw a <seealso cref="NumberIsTooLargeException"/> if the requested
		/// derivation order is larger or equal to the number of points.
		/// </p>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public UnivariateDifferentiableVectorFunction differentiate(final mathlib.analysis.UnivariateVectorFunction function)
		public virtual UnivariateDifferentiableVectorFunction differentiate(UnivariateVectorFunction function)
		{
			return new UnivariateDifferentiableVectorFunctionAnonymousInnerClassHelper(this, function);
		}

		private class UnivariateDifferentiableVectorFunctionAnonymousInnerClassHelper : UnivariateDifferentiableVectorFunction
		{
			private readonly FiniteDifferencesDifferentiator outerInstance;

			private UnivariateVectorFunction function;

			public UnivariateDifferentiableVectorFunctionAnonymousInnerClassHelper(FiniteDifferencesDifferentiator outerInstance, UnivariateVectorFunction function)
			{
				this.outerInstance = outerInstance;
				this.function = function;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] value(final double x) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			public virtual double[] value(double x)
			{
				return function.value(x);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure[] value(final DerivativeStructure t) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			public virtual DerivativeStructure[] value(DerivativeStructure t)
			{

				// check we can achieve the requested derivation order with the sample
				if (t.Order >= outerInstance.nbPoints)
				{
					throw new NumberIsTooLargeException(t.Order, outerInstance.nbPoints, false);
				}

				// compute sample position, trying to be centered if possible
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t0 = mathlib.util.FastMath.max(mathlib.util.FastMath.min(t.getValue(), tMax), tMin) - halfSampleSpan;
				double t0 = FastMath.max(FastMath.min(t.Value, outerInstance.tMax), outerInstance.tMin) - outerInstance.halfSampleSpan;

				// compute sample points
				double[][] y = null;
				for (int i = 0; i < outerInstance.nbPoints; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] v = function.value(t0 + i * stepSize);
					double[] v = function.value(t0 + i * outerInstance.stepSize);
					if (i == 0)
					{
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: y = new double[v.Length][outerInstance.nbPoints];
						y = RectangularArrays.ReturnRectangularDoubleArray(v.Length, outerInstance.nbPoints);
					}
					for (int j = 0; j < v.Length; ++j)
					{
						y[j][i] = v[j];
					}
				}

				// evaluate derivatives
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure[] value = new DerivativeStructure[y.length];
				DerivativeStructure[] value = new DerivativeStructure[y.Length];
				for (int j = 0; j < value.Length; ++j)
				{
					value[j] = outerInstance.evaluate(t, t0, y[j]);
				}

				return value;

			}

		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>The returned object cannot compute derivatives to arbitrary orders. The
		/// value function will throw a <seealso cref="NumberIsTooLargeException"/> if the requested
		/// derivation order is larger or equal to the number of points.
		/// </p>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public UnivariateDifferentiableMatrixFunction differentiate(final mathlib.analysis.UnivariateMatrixFunction function)
		public virtual UnivariateDifferentiableMatrixFunction differentiate(UnivariateMatrixFunction function)
		{
			return new UnivariateDifferentiableMatrixFunctionAnonymousInnerClassHelper(this, function);
		}

		private class UnivariateDifferentiableMatrixFunctionAnonymousInnerClassHelper : UnivariateDifferentiableMatrixFunction
		{
			private readonly FiniteDifferencesDifferentiator outerInstance;

			private UnivariateMatrixFunction function;

			public UnivariateDifferentiableMatrixFunctionAnonymousInnerClassHelper(FiniteDifferencesDifferentiator outerInstance, UnivariateMatrixFunction function)
			{
				this.outerInstance = outerInstance;
				this.function = function;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[][] value(final double x) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			public virtual double[][] value(double x)
			{
				return function.value(x);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DerivativeStructure[][] value(final DerivativeStructure t) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			public virtual DerivativeStructure[][] value(DerivativeStructure t)
			{

				// check we can achieve the requested derivation order with the sample
				if (t.Order >= outerInstance.nbPoints)
				{
					throw new NumberIsTooLargeException(t.Order, outerInstance.nbPoints, false);
				}

				// compute sample position, trying to be centered if possible
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t0 = mathlib.util.FastMath.max(mathlib.util.FastMath.min(t.getValue(), tMax), tMin) - halfSampleSpan;
				double t0 = FastMath.max(FastMath.min(t.Value, outerInstance.tMax), outerInstance.tMin) - outerInstance.halfSampleSpan;

				// compute sample points
				double[][][] y = null;
				for (int i = 0; i < outerInstance.nbPoints; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] v = function.value(t0 + i * stepSize);
					double[][] v = function.value(t0 + i * outerInstance.stepSize);
					if (i == 0)
					{
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: y = new double[v.Length][v[0].Length][outerInstance.nbPoints];
						y = RectangularArrays.ReturnRectangularDoubleArray(v.Length, v[0].Length, outerInstance.nbPoints);
					}
					for (int j = 0; j < v.Length; ++j)
					{
						for (int k = 0; k < v[j].Length; ++k)
						{
							y[j][k][i] = v[j][k];
						}
					}
				}

				// evaluate derivatives
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure[][] value = new DerivativeStructure[y.length][y[0].length];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: DerivativeStructure[][] value = new DerivativeStructure[y.Length][y[0].Length];
				DerivativeStructure[][] value = RectangularArrays.ReturnRectangularDerivativeStructureArray(y.Length, y[0].Length);
				for (int j = 0; j < value.Length; ++j)
				{
					for (int k = 0; k < y[j].Length; ++k)
					{
						value[j][k] = outerInstance.evaluate(t, t0, y[j][k]);
					}
				}

				return value;

			}

		}

	}

}
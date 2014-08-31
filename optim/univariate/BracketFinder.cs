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
namespace org.apache.commons.math3.optim.univariate
{

	using FastMath = org.apache.commons.math3.util.FastMath;
	using Incrementor = org.apache.commons.math3.util.Incrementor;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using TooManyEvaluationsException = org.apache.commons.math3.exception.TooManyEvaluationsException;
	using MaxCountExceededException = org.apache.commons.math3.exception.MaxCountExceededException;
	using UnivariateFunction = org.apache.commons.math3.analysis.UnivariateFunction;
	using GoalType = org.apache.commons.math3.optim.nonlinear.scalar.GoalType;

	/// <summary>
	/// Provide an interval that brackets a local optimum of a function.
	/// This code is based on a Python implementation (from <em>SciPy</em>,
	/// module {@code optimize.py} v0.5).
	/// 
	/// @version $Id: BracketFinder.java 1573316 2014-03-02 14:54:37Z erans $
	/// @since 2.2
	/// </summary>
	public class BracketFinder
	{
		/// <summary>
		/// Tolerance to avoid division by zero. </summary>
		private const double EPS_MIN = 1e-21;
		/// <summary>
		/// Golden section.
		/// </summary>
		private const double GOLD = 1.618034;
		/// <summary>
		/// Factor for expanding the interval.
		/// </summary>
		private readonly double growLimit;
		/// <summary>
		/// Counter for function evaluations.
		/// </summary>
		private readonly Incrementor evaluations = new Incrementor();
		/// <summary>
		/// Lower bound of the bracket.
		/// </summary>
		private double lo;
		/// <summary>
		/// Higher bound of the bracket.
		/// </summary>
		private double hi;
		/// <summary>
		/// Point inside the bracket.
		/// </summary>
		private double mid;
		/// <summary>
		/// Function value at <seealso cref="#lo"/>.
		/// </summary>
		private double fLo;
		/// <summary>
		/// Function value at <seealso cref="#hi"/>.
		/// </summary>
		private double fHi;
		/// <summary>
		/// Function value at <seealso cref="#mid"/>.
		/// </summary>
		private double fMid;

		/// <summary>
		/// Constructor with default values {@code 100, 50} (see the
		/// <seealso cref="#BracketFinder(double,int) other constructor"/>).
		/// </summary>
		public BracketFinder() : this(100, 50)
		{
		}

		/// <summary>
		/// Create a bracketing interval finder.
		/// </summary>
		/// <param name="growLimit"> Expanding factor. </param>
		/// <param name="maxEvaluations"> Maximum number of evaluations allowed for finding
		/// a bracketing interval. </param>
		public BracketFinder(double growLimit, int maxEvaluations)
		{
			if (growLimit <= 0)
			{
				throw new NotStrictlyPositiveException(growLimit);
			}
			if (maxEvaluations <= 0)
			{
				throw new NotStrictlyPositiveException(maxEvaluations);
			}

			this.growLimit = growLimit;
			evaluations.MaximalCount = maxEvaluations;
		}

		/// <summary>
		/// Search new points that bracket a local optimum of the function.
		/// </summary>
		/// <param name="func"> Function whose optimum should be bracketed. </param>
		/// <param name="goal"> <seealso cref="GoalType Goal type"/>. </param>
		/// <param name="xA"> Initial point. </param>
		/// <param name="xB"> Initial point. </param>
		/// <exception cref="TooManyEvaluationsException"> if the maximum number of evaluations
		/// is exceeded. </exception>
		public virtual void search(UnivariateFunction func, GoalType goal, double xA, double xB)
		{
			evaluations.resetCount();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isMinim = goal == org.apache.commons.math3.optim.nonlinear.scalar.GoalType.MINIMIZE;
			bool isMinim = goal == GoalType.MINIMIZE;

			double fA = eval(func, xA);
			double fB = eval(func, xB);
			if (isMinim ? fA < fB : fA > fB)
			{

				double tmp = xA;
				xA = xB;
				xB = tmp;

				tmp = fA;
				fA = fB;
				fB = tmp;
			}

			double xC = xB + GOLD * (xB - xA);
			double fC = eval(func, xC);

			while (isMinim ? fC < fB : fC > fB)
			{
				double tmp1 = (xB - xA) * (fB - fC);
				double tmp2 = (xB - xC) * (fB - fA);

				double val = tmp2 - tmp1;
				double denom = FastMath.abs(val) < EPS_MIN ? 2 * EPS_MIN : 2 * val;

				double w = xB - ((xB - xC) * tmp2 - (xB - xA) * tmp1) / denom;
				double wLim = xB + growLimit * (xC - xB);

				double fW;
				if ((w - xC) * (xB - w) > 0)
				{
					fW = eval(func, w);
					if (isMinim ? fW < fC : fW > fC)
					{
						xA = xB;
						xB = w;
						fA = fB;
						fB = fW;
						break;
					}
					else if (isMinim ? fW > fB : fW < fB)
					{
						xC = w;
						fC = fW;
						break;
					}
					w = xC + GOLD * (xC - xB);
					fW = eval(func, w);
				}
				else if ((w - wLim) * (wLim - xC) >= 0)
				{
					w = wLim;
					fW = eval(func, w);
				}
				else if ((w - wLim) * (xC - w) > 0)
				{
					fW = eval(func, w);
					if (isMinim ? fW < fC : fW > fC)
					{
						xB = xC;
						xC = w;
						w = xC + GOLD * (xC - xB);
						fB = fC;
						fC = fW;
						fW = eval(func, w);
					}
				}
				else
				{
					w = xC + GOLD * (xC - xB);
					fW = eval(func, w);
				}

				xA = xB;
				fA = fB;
				xB = xC;
				fB = fC;
				xC = w;
				fC = fW;
			}

			lo = xA;
			fLo = fA;
			mid = xB;
			fMid = fB;
			hi = xC;
			fHi = fC;

			if (lo > hi)
			{
				double tmp = lo;
				lo = hi;
				hi = tmp;

				tmp = fLo;
				fLo = fHi;
				fHi = tmp;
			}
		}

		/// <returns> the number of evalutations. </returns>
		public virtual int MaxEvaluations
		{
			get
			{
				return evaluations.MaximalCount;
			}
		}

		/// <returns> the number of evalutations. </returns>
		public virtual int Evaluations
		{
			get
			{
				return evaluations.Count;
			}
		}

		/// <returns> the lower bound of the bracket. </returns>
		/// <seealso cref= #getFLo() </seealso>
		public virtual double Lo
		{
			get
			{
				return lo;
			}
		}

		/// <summary>
		/// Get function value at <seealso cref="#getLo()"/>. </summary>
		/// <returns> function value at <seealso cref="#getLo()"/> </returns>
		public virtual double FLo
		{
			get
			{
				return fLo;
			}
		}

		/// <returns> the higher bound of the bracket. </returns>
		/// <seealso cref= #getFHi() </seealso>
		public virtual double Hi
		{
			get
			{
				return hi;
			}
		}

		/// <summary>
		/// Get function value at <seealso cref="#getHi()"/>. </summary>
		/// <returns> function value at <seealso cref="#getHi()"/> </returns>
		public virtual double FHi
		{
			get
			{
				return fHi;
			}
		}

		/// <returns> a point in the middle of the bracket. </returns>
		/// <seealso cref= #getFMid() </seealso>
		public virtual double Mid
		{
			get
			{
				return mid;
			}
		}

		/// <summary>
		/// Get function value at <seealso cref="#getMid()"/>. </summary>
		/// <returns> function value at <seealso cref="#getMid()"/> </returns>
		public virtual double FMid
		{
			get
			{
				return fMid;
			}
		}

		/// <param name="f"> Function. </param>
		/// <param name="x"> Argument. </param>
		/// <returns> {@code f(x)} </returns>
		/// <exception cref="TooManyEvaluationsException"> if the maximal number of evaluations is
		/// exceeded. </exception>
		private double eval(UnivariateFunction f, double x)
		{
			try
			{
				evaluations.incrementCount();
			}
			catch (MaxCountExceededException e)
			{
				throw new TooManyEvaluationsException(e.Max);
			}
			return f.value(x);
		}
	}

}
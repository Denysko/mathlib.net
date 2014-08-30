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

namespace org.apache.commons.math3.optimization.direct
{

	using MultivariateFunction = org.apache.commons.math3.analysis.MultivariateFunction;
	using UnivariateFunction = org.apache.commons.math3.analysis.UnivariateFunction;
	using Logit = org.apache.commons.math3.analysis.function.Logit;
	using Sigmoid = org.apache.commons.math3.analysis.function.Sigmoid;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// <p>Adapter for mapping bounded <seealso cref="MultivariateFunction"/> to unbounded ones.</p>
	/// 
	/// <p>
	/// This adapter can be used to wrap functions subject to simple bounds on
	/// parameters so they can be used by optimizers that do <em>not</em> directly
	/// support simple bounds.
	/// </p>
	/// <p>
	/// The principle is that the user function that will be wrapped will see its
	/// parameters bounded as required, i.e when its {@code value} method is called
	/// with argument array {@code point}, the elements array will fulfill requirement
	/// {@code lower[i] <= point[i] <= upper[i]} for all i. Some of the components
	/// may be unbounded or bounded only on one side if the corresponding bound is
	/// set to an infinite value. The optimizer will not manage the user function by
	/// itself, but it will handle this adapter and it is this adapter that will take
	/// care the bounds are fulfilled. The adapter <seealso cref="#value(double[])"/> method will
	/// be called by the optimizer with unbound parameters, and the adapter will map
	/// the unbounded value to the bounded range using appropriate functions like
	/// <seealso cref="Sigmoid"/> for double bounded elements for example.
	/// </p>
	/// <p>
	/// As the optimizer sees only unbounded parameters, it should be noted that the
	/// start point or simplex expected by the optimizer should be unbounded, so the
	/// user is responsible for converting his bounded point to unbounded by calling
	/// <seealso cref="#boundedToUnbounded(double[])"/> before providing them to the optimizer.
	/// For the same reason, the point returned by the {@link
	/// org.apache.commons.math3.optimization.BaseMultivariateOptimizer#optimize(int,
	/// MultivariateFunction, org.apache.commons.math3.optimization.GoalType, double[])}
	/// method is unbounded. So to convert this point to bounded, users must call
	/// <seealso cref="#unboundedToBounded(double[])"/> by themselves!</p>
	/// <p>
	/// This adapter is only a poor man solution to simple bounds optimization constraints
	/// that can be used with simple optimizers like <seealso cref="SimplexOptimizer"/> with {@link
	/// NelderMeadSimplex} or <seealso cref="MultiDirectionalSimplex"/>. A better solution is to use
	/// an optimizer that directly supports simple bounds like <seealso cref="CMAESOptimizer"/> or
	/// <seealso cref="BOBYQAOptimizer"/>. One caveat of this poor man solution is that behavior near
	/// the bounds may be numerically unstable as bounds are mapped from infinite values.
	/// Another caveat is that convergence values are evaluated by the optimizer with respect
	/// to unbounded variables, so there will be scales differences when converted to bounded
	/// variables.
	/// </p>
	/// </summary>
	/// <seealso cref= MultivariateFunctionPenaltyAdapter
	/// 
	/// @version $Id: MultivariateFunctionMappingAdapter.java 1422230 2012-12-15 12:11:13Z erans $ </seealso>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.0 

	[Obsolete]
	public class MultivariateFunctionMappingAdapter : MultivariateFunction
	{

		/// <summary>
		/// Underlying bounded function. </summary>
		private readonly MultivariateFunction bounded;

		/// <summary>
		/// Mapping functions. </summary>
		private readonly Mapper[] mappers;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="bounded"> bounded function </param>
		/// <param name="lower"> lower bounds for each element of the input parameters array
		/// (some elements may be set to {@code Double.NEGATIVE_INFINITY} for
		/// unbounded values) </param>
		/// <param name="upper"> upper bounds for each element of the input parameters array
		/// (some elements may be set to {@code Double.POSITIVE_INFINITY} for
		/// unbounded values) </param>
		/// <exception cref="DimensionMismatchException"> if lower and upper bounds are not
		/// consistent, either according to dimension or to values </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MultivariateFunctionMappingAdapter(final org.apache.commons.math3.analysis.MultivariateFunction bounded, final double[] lower, final double[] upper)
		public MultivariateFunctionMappingAdapter(MultivariateFunction bounded, double[] lower, double[] upper)
		{

			// safety checks
			MathUtils.checkNotNull(lower);
			MathUtils.checkNotNull(upper);
			if (lower.Length != upper.Length)
			{
				throw new DimensionMismatchException(lower.Length, upper.Length);
			}
			for (int i = 0; i < lower.Length; ++i)
			{
				// note the following test is written in such a way it also fails for NaN
				if (!(upper[i] >= lower[i]))
				{
					throw new NumberIsTooSmallException(upper[i], lower[i], true);
				}
			}

			this.bounded = bounded;
			this.mappers = new Mapper[lower.Length];
			for (int i = 0; i < mappers.Length; ++i)
			{
				if (double.IsInfinity(lower[i]))
				{
					if (double.IsInfinity(upper[i]))
					{
						// element is unbounded, no transformation is needed
						mappers[i] = new NoBoundsMapper();
					}
					else
					{
						// element is simple-bounded on the upper side
						mappers[i] = new UpperBoundMapper(upper[i]);
					}
				}
				else
				{
					if (double.IsInfinity(upper[i]))
					{
						// element is simple-bounded on the lower side
						mappers[i] = new LowerBoundMapper(lower[i]);
					}
					else
					{
						// element is double-bounded
						mappers[i] = new LowerUpperBoundMapper(lower[i], upper[i]);
					}
				}
			}

		}

		/// <summary>
		/// Map an array from unbounded to bounded. </summary>
		/// <param name="point"> unbounded value </param>
		/// <returns> bounded value </returns>
		public virtual double[] unboundedToBounded(double[] point)
		{

			// map unbounded input point to bounded point
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] mapped = new double[mappers.length];
			double[] mapped = new double[mappers.Length];
			for (int i = 0; i < mappers.Length; ++i)
			{
				mapped[i] = mappers[i].unboundedToBounded(point[i]);
			}

			return mapped;

		}

		/// <summary>
		/// Map an array from bounded to unbounded. </summary>
		/// <param name="point"> bounded value </param>
		/// <returns> unbounded value </returns>
		public virtual double[] boundedToUnbounded(double[] point)
		{

			// map bounded input point to unbounded point
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] mapped = new double[mappers.length];
			double[] mapped = new double[mappers.Length];
			for (int i = 0; i < mappers.Length; ++i)
			{
				mapped[i] = mappers[i].boundedToUnbounded(point[i]);
			}

			return mapped;

		}

		/// <summary>
		/// Compute the underlying function value from an unbounded point.
		/// <p>
		/// This method simply bounds the unbounded point using the mappings
		/// set up at construction and calls the underlying function using
		/// the bounded point.
		/// </p> </summary>
		/// <param name="point"> unbounded value </param>
		/// <returns> underlying function value </returns>
		/// <seealso cref= #unboundedToBounded(double[]) </seealso>
		public virtual double value(double[] point)
		{
			return bounded.value(unboundedToBounded(point));
		}

		/// <summary>
		/// Mapping interface. </summary>
		private interface Mapper
		{

			/// <summary>
			/// Map a value from unbounded to bounded. </summary>
			/// <param name="y"> unbounded value </param>
			/// <returns> bounded value </returns>
			double unboundedToBounded(double y);

			/// <summary>
			/// Map a value from bounded to unbounded. </summary>
			/// <param name="x"> bounded value </param>
			/// <returns> unbounded value </returns>
			double boundedToUnbounded(double x);

		}

		/// <summary>
		/// Local class for no bounds mapping. </summary>
		private class NoBoundsMapper : Mapper
		{

			/// <summary>
			/// Simple constructor.
			/// </summary>
			public NoBoundsMapper()
			{
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double unboundedToBounded(final double y)
			public virtual double unboundedToBounded(double y)
			{
				return y;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double boundedToUnbounded(final double x)
			public virtual double boundedToUnbounded(double x)
			{
				return x;
			}

		}

		/// <summary>
		/// Local class for lower bounds mapping. </summary>
		private class LowerBoundMapper : Mapper
		{

			/// <summary>
			/// Low bound. </summary>
			internal readonly double lower;

			/// <summary>
			/// Simple constructor. </summary>
			/// <param name="lower"> lower bound </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LowerBoundMapper(final double lower)
			public LowerBoundMapper(double lower)
			{
				this.lower = lower;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double unboundedToBounded(final double y)
			public virtual double unboundedToBounded(double y)
			{
				return lower + FastMath.exp(y);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double boundedToUnbounded(final double x)
			public virtual double boundedToUnbounded(double x)
			{
				return FastMath.log(x - lower);
			}

		}

		/// <summary>
		/// Local class for upper bounds mapping. </summary>
		private class UpperBoundMapper : Mapper
		{

			/// <summary>
			/// Upper bound. </summary>
			internal readonly double upper;

			/// <summary>
			/// Simple constructor. </summary>
			/// <param name="upper"> upper bound </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public UpperBoundMapper(final double upper)
			public UpperBoundMapper(double upper)
			{
				this.upper = upper;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double unboundedToBounded(final double y)
			public virtual double unboundedToBounded(double y)
			{
				return upper - FastMath.exp(-y);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double boundedToUnbounded(final double x)
			public virtual double boundedToUnbounded(double x)
			{
				return -FastMath.log(upper - x);
			}

		}

		/// <summary>
		/// Local class for lower and bounds mapping. </summary>
		private class LowerUpperBoundMapper : Mapper
		{

			/// <summary>
			/// Function from unbounded to bounded. </summary>
			internal readonly UnivariateFunction boundingFunction;

			/// <summary>
			/// Function from bounded to unbounded. </summary>
			internal readonly UnivariateFunction unboundingFunction;

			/// <summary>
			/// Simple constructor. </summary>
			/// <param name="lower"> lower bound </param>
			/// <param name="upper"> upper bound </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LowerUpperBoundMapper(final double lower, final double upper)
			public LowerUpperBoundMapper(double lower, double upper)
			{
				boundingFunction = new Sigmoid(lower, upper);
				unboundingFunction = new Logit(lower, upper);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double unboundedToBounded(final double y)
			public virtual double unboundedToBounded(double y)
			{
				return boundingFunction.value(y);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double boundedToUnbounded(final double x)
			public virtual double boundedToUnbounded(double x)
			{
				return unboundingFunction.value(x);
			}

		}

	}

}
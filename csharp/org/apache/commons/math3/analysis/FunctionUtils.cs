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

namespace org.apache.commons.math3.analysis
{

	using DerivativeStructure = org.apache.commons.math3.analysis.differentiation.DerivativeStructure;
	using MultivariateDifferentiableFunction = org.apache.commons.math3.analysis.differentiation.MultivariateDifferentiableFunction;
	using MultivariateDifferentiableVectorFunction = org.apache.commons.math3.analysis.differentiation.MultivariateDifferentiableVectorFunction;
	using UnivariateDifferentiableFunction = org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction;
	using Identity = org.apache.commons.math3.analysis.function.Identity;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// Utilities for manipulating function objects.
	/// 
	/// @version $Id: FunctionUtils.java 1499808 2013-07-04 17:00:42Z sebb $
	/// @since 3.0
	/// </summary>
	public class FunctionUtils
	{
		/// <summary>
		/// Class only contains static methods.
		/// </summary>
		private FunctionUtils()
		{
		}

		/// <summary>
		/// Composes functions.
		/// <br/>
		/// The functions in the argument list are composed sequentially, in the
		/// given order.  For example, compose(f1,f2,f3) acts like f1(f2(f3(x))).
		/// </summary>
		/// <param name="f"> List of functions. </param>
		/// <returns> the composite function. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static UnivariateFunction compose(final UnivariateFunction... f)
		public static UnivariateFunction compose(params UnivariateFunction[] f)
		{
			return new UnivariateFunctionAnonymousInnerClassHelper(f);
		}

		private class UnivariateFunctionAnonymousInnerClassHelper : UnivariateFunction
		{
			private org.apache.commons.math3.analysis.UnivariateFunction[] f;

			public UnivariateFunctionAnonymousInnerClassHelper(org.apache.commons.math3.analysis.UnivariateFunction[] f)
			{
				this.f = f;
			}

					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual double value(double x)
			{
				double r = x;
				for (int i = f.Length - 1; i >= 0; i--)
				{
					r = f[i].value(r);
				}
				return r;
			}
		}

		/// <summary>
		/// Composes functions.
		/// <br/>
		/// The functions in the argument list are composed sequentially, in the
		/// given order.  For example, compose(f1,f2,f3) acts like f1(f2(f3(x))).
		/// </summary>
		/// <param name="f"> List of functions. </param>
		/// <returns> the composite function.
		/// @since 3.1 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction compose(final org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction... f)
		public static UnivariateDifferentiableFunction compose(params UnivariateDifferentiableFunction[] f)
		{
			return new UnivariateDifferentiableFunctionAnonymousInnerClassHelper(f);
		}

		private class UnivariateDifferentiableFunctionAnonymousInnerClassHelper : UnivariateDifferentiableFunction
		{
			private UnivariateDifferentiableFunction[] f;

			public UnivariateDifferentiableFunctionAnonymousInnerClassHelper(UnivariateDifferentiableFunction[] f)
			{
				this.f = f;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double value(final double t)
			public virtual double value(double t)
			{
				double r = t;
				for (int i = f.Length - 1; i >= 0; i--)
				{
					r = f[i].value(r);
				}
				return r;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.analysis.differentiation.DerivativeStructure value(final org.apache.commons.math3.analysis.differentiation.DerivativeStructure t)
			public virtual DerivativeStructure value(DerivativeStructure t)
			{
				DerivativeStructure r = t;
				for (int i = f.Length - 1; i >= 0; i--)
				{
					r = f[i].value(r);
				}
				return r;
			}

		}

		/// <summary>
		/// Composes functions.
		/// <br/>
		/// The functions in the argument list are composed sequentially, in the
		/// given order.  For example, compose(f1,f2,f3) acts like f1(f2(f3(x))).
		/// </summary>
		/// <param name="f"> List of functions. </param>
		/// <returns> the composite function. </returns>
		/// @deprecated as of 3.1 replaced by <seealso cref="#compose(UnivariateDifferentiableFunction...)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.1 replaced by <seealso cref="#compose(org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction...)"/>") public static DifferentiableUnivariateFunction compose(final DifferentiableUnivariateFunction... f)
		[Obsolete("as of 3.1 replaced by <seealso cref="#compose(org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction...)"/>")]
		public static DifferentiableUnivariateFunction compose(params DifferentiableUnivariateFunction[] f)
		{
			return new DifferentiableUnivariateFunctionAnonymousInnerClassHelper(f);
		}

		private class DifferentiableUnivariateFunctionAnonymousInnerClassHelper : DifferentiableUnivariateFunction
		{
			private org.apache.commons.math3.analysis.DifferentiableUnivariateFunction[] f;

			public DifferentiableUnivariateFunctionAnonymousInnerClassHelper(org.apache.commons.math3.analysis.DifferentiableUnivariateFunction[] f)
			{
				this.f = f;
			}

					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual double value(double x)
			{
				double r = x;
				for (int i = f.Length - 1; i >= 0; i--)
				{
					r = f[i].value(r);
				}
				return r;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual UnivariateFunction derivative()
			{
				return new UnivariateFunctionAnonymousInnerClassHelper2(this);
			}

			private class UnivariateFunctionAnonymousInnerClassHelper2 : UnivariateFunction
			{
				private readonly DifferentiableUnivariateFunctionAnonymousInnerClassHelper outerInstance;

				public UnivariateFunctionAnonymousInnerClassHelper2(DifferentiableUnivariateFunctionAnonymousInnerClassHelper outerInstance)
				{
					this.outerInstance = outerInstance;
				}

							/// <summary>
							/// {@inheritDoc} </summary>
				public virtual double value(double x)
				{
					double p = 1;
					double r = x;
					for (int i = outerInstance.f.Length - 1; i >= 0; i--)
					{
						p *= outerInstance.f[i].derivative().value(r);
						r = outerInstance.f[i].value(r);
					}
					return p;
				}
			}
		}

		/// <summary>
		/// Adds functions.
		/// </summary>
		/// <param name="f"> List of functions. </param>
		/// <returns> a function that computes the sum of the functions. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static UnivariateFunction add(final UnivariateFunction... f)
		public static UnivariateFunction add(params UnivariateFunction[] f)
		{
			return new UnivariateFunctionAnonymousInnerClassHelper3(f);
		}

		private class UnivariateFunctionAnonymousInnerClassHelper3 : UnivariateFunction
		{
			private org.apache.commons.math3.analysis.UnivariateFunction[] f;

			public UnivariateFunctionAnonymousInnerClassHelper3(org.apache.commons.math3.analysis.UnivariateFunction[] f)
			{
				this.f = f;
			}

					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual double value(double x)
			{
				double r = f[0].value(x);
				for (int i = 1; i < f.Length; i++)
				{
					r += f[i].value(x);
				}
				return r;
			}
		}

		/// <summary>
		/// Adds functions.
		/// </summary>
		/// <param name="f"> List of functions. </param>
		/// <returns> a function that computes the sum of the functions.
		/// @since 3.1 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction add(final org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction... f)
		public static UnivariateDifferentiableFunction add(params UnivariateDifferentiableFunction[] f)
		{
			return new UnivariateDifferentiableFunctionAnonymousInnerClassHelper2(f);
		}

		private class UnivariateDifferentiableFunctionAnonymousInnerClassHelper2 : UnivariateDifferentiableFunction
		{
			private UnivariateDifferentiableFunction[] f;

			public UnivariateDifferentiableFunctionAnonymousInnerClassHelper2(UnivariateDifferentiableFunction[] f)
			{
				this.f = f;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double value(final double t)
			public virtual double value(double t)
			{
				double r = f[0].value(t);
				for (int i = 1; i < f.Length; i++)
				{
					r += f[i].value(t);
				}
				return r;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			/// <exception cref="DimensionMismatchException"> if functions are not consistent with each other </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.analysis.differentiation.DerivativeStructure value(final org.apache.commons.math3.analysis.differentiation.DerivativeStructure t) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			public virtual DerivativeStructure value(DerivativeStructure t)
			{
				DerivativeStructure r = f[0].value(t);
				for (int i = 1; i < f.Length; i++)
				{
					r = r.add(f[i].value(t));
				}
				return r;
			}

		}

		/// <summary>
		/// Adds functions.
		/// </summary>
		/// <param name="f"> List of functions. </param>
		/// <returns> a function that computes the sum of the functions. </returns>
		/// @deprecated as of 3.1 replaced by <seealso cref="#add(UnivariateDifferentiableFunction...)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.1 replaced by <seealso cref="#add(org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction...)"/>") public static DifferentiableUnivariateFunction add(final DifferentiableUnivariateFunction... f)
		[Obsolete("as of 3.1 replaced by <seealso cref="#add(org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction...)"/>")]
		public static DifferentiableUnivariateFunction add(params DifferentiableUnivariateFunction[] f)
		{
			return new DifferentiableUnivariateFunctionAnonymousInnerClassHelper2(f);
		}

		private class DifferentiableUnivariateFunctionAnonymousInnerClassHelper2 : DifferentiableUnivariateFunction
		{
			private org.apache.commons.math3.analysis.DifferentiableUnivariateFunction[] f;

			public DifferentiableUnivariateFunctionAnonymousInnerClassHelper2(org.apache.commons.math3.analysis.DifferentiableUnivariateFunction[] f)
			{
				this.f = f;
			}

					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual double value(double x)
			{
				double r = f[0].value(x);
				for (int i = 1; i < f.Length; i++)
				{
					r += f[i].value(x);
				}
				return r;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual UnivariateFunction derivative()
			{
				return new UnivariateFunctionAnonymousInnerClassHelper4(this);
			}

			private class UnivariateFunctionAnonymousInnerClassHelper4 : UnivariateFunction
			{
				private readonly DifferentiableUnivariateFunctionAnonymousInnerClassHelper2 outerInstance;

				public UnivariateFunctionAnonymousInnerClassHelper4(DifferentiableUnivariateFunctionAnonymousInnerClassHelper2 outerInstance)
				{
					this.outerInstance = outerInstance;
				}

							/// <summary>
							/// {@inheritDoc} </summary>
				public virtual double value(double x)
				{
					double r = outerInstance.f[0].derivative().value(x);
					for (int i = 1; i < outerInstance.f.Length; i++)
					{
						r += outerInstance.f[i].derivative().value(x);
					}
					return r;
				}
			}
		}

		/// <summary>
		/// Multiplies functions.
		/// </summary>
		/// <param name="f"> List of functions. </param>
		/// <returns> a function that computes the product of the functions. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static UnivariateFunction multiply(final UnivariateFunction... f)
		public static UnivariateFunction multiply(params UnivariateFunction[] f)
		{
			return new UnivariateFunctionAnonymousInnerClassHelper5(f);
		}

		private class UnivariateFunctionAnonymousInnerClassHelper5 : UnivariateFunction
		{
			private org.apache.commons.math3.analysis.UnivariateFunction[] f;

			public UnivariateFunctionAnonymousInnerClassHelper5(org.apache.commons.math3.analysis.UnivariateFunction[] f)
			{
				this.f = f;
			}

					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual double value(double x)
			{
				double r = f[0].value(x);
				for (int i = 1; i < f.Length; i++)
				{
					r *= f[i].value(x);
				}
				return r;
			}
		}

		/// <summary>
		/// Multiplies functions.
		/// </summary>
		/// <param name="f"> List of functions. </param>
		/// <returns> a function that computes the product of the functions.
		/// @since 3.1 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction multiply(final org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction... f)
		public static UnivariateDifferentiableFunction multiply(params UnivariateDifferentiableFunction[] f)
		{
			return new UnivariateDifferentiableFunctionAnonymousInnerClassHelper3(f);
		}

		private class UnivariateDifferentiableFunctionAnonymousInnerClassHelper3 : UnivariateDifferentiableFunction
		{
			private UnivariateDifferentiableFunction[] f;

			public UnivariateDifferentiableFunctionAnonymousInnerClassHelper3(UnivariateDifferentiableFunction[] f)
			{
				this.f = f;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double value(final double t)
			public virtual double value(double t)
			{
				double r = f[0].value(t);
				for (int i = 1; i < f.Length; i++)
				{
					r *= f[i].value(t);
				}
				return r;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.analysis.differentiation.DerivativeStructure value(final org.apache.commons.math3.analysis.differentiation.DerivativeStructure t)
			public virtual DerivativeStructure value(DerivativeStructure t)
			{
				DerivativeStructure r = f[0].value(t);
				for (int i = 1; i < f.Length; i++)
				{
					r = r.multiply(f[i].value(t));
				}
				return r;
			}

		}

		/// <summary>
		/// Multiplies functions.
		/// </summary>
		/// <param name="f"> List of functions. </param>
		/// <returns> a function that computes the product of the functions. </returns>
		/// @deprecated as of 3.1 replaced by <seealso cref="#multiply(UnivariateDifferentiableFunction...)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.1 replaced by <seealso cref="#multiply(org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction...)"/>") public static DifferentiableUnivariateFunction multiply(final DifferentiableUnivariateFunction... f)
		[Obsolete("as of 3.1 replaced by <seealso cref="#multiply(org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction...)"/>")]
		public static DifferentiableUnivariateFunction multiply(params DifferentiableUnivariateFunction[] f)
		{
			return new DifferentiableUnivariateFunctionAnonymousInnerClassHelper3(f);
		}

		private class DifferentiableUnivariateFunctionAnonymousInnerClassHelper3 : DifferentiableUnivariateFunction
		{
			private org.apache.commons.math3.analysis.DifferentiableUnivariateFunction[] f;

			public DifferentiableUnivariateFunctionAnonymousInnerClassHelper3(org.apache.commons.math3.analysis.DifferentiableUnivariateFunction[] f)
			{
				this.f = f;
			}

					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual double value(double x)
			{
				double r = f[0].value(x);
				for (int i = 1; i < f.Length; i++)
				{
					r *= f[i].value(x);
				}
				return r;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual UnivariateFunction derivative()
			{
				return new UnivariateFunctionAnonymousInnerClassHelper6(this);
			}

			private class UnivariateFunctionAnonymousInnerClassHelper6 : UnivariateFunction
			{
				private readonly DifferentiableUnivariateFunctionAnonymousInnerClassHelper3 outerInstance;

				public UnivariateFunctionAnonymousInnerClassHelper6(DifferentiableUnivariateFunctionAnonymousInnerClassHelper3 outerInstance)
				{
					this.outerInstance = outerInstance;
				}

							/// <summary>
							/// {@inheritDoc} </summary>
				public virtual double value(double x)
				{
					double sum = 0;
					for (int i = 0; i < outerInstance.f.Length; i++)
					{
						double prod = outerInstance.f[i].derivative().value(x);
						for (int j = 0; j < outerInstance.f.Length; j++)
						{
							if (i != j)
							{
								prod *= outerInstance.f[j].value(x);
							}
						}
						sum += prod;
					}
					return sum;
				}
			}
		}

		/// <summary>
		/// Returns the univariate function <br/>
		/// {@code h(x) = combiner(f(x), g(x))}.
		/// </summary>
		/// <param name="combiner"> Combiner function. </param>
		/// <param name="f"> Function. </param>
		/// <param name="g"> Function. </param>
		/// <returns> the composite function. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static UnivariateFunction combine(final BivariateFunction combiner, final UnivariateFunction f, final UnivariateFunction g)
		public static UnivariateFunction combine(BivariateFunction combiner, UnivariateFunction f, UnivariateFunction g)
		{
			return new UnivariateFunctionAnonymousInnerClassHelper7(combiner, f, g);
		}

		private class UnivariateFunctionAnonymousInnerClassHelper7 : UnivariateFunction
		{
			private org.apache.commons.math3.analysis.BivariateFunction combiner;
			private org.apache.commons.math3.analysis.UnivariateFunction f;
			private org.apache.commons.math3.analysis.UnivariateFunction g;

			public UnivariateFunctionAnonymousInnerClassHelper7(org.apache.commons.math3.analysis.BivariateFunction combiner, org.apache.commons.math3.analysis.UnivariateFunction f, org.apache.commons.math3.analysis.UnivariateFunction g)
			{
				this.combiner = combiner;
				this.f = f;
				this.g = g;
			}

					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual double value(double x)
			{
				return combiner.value(f.value(x), g.value(x));
			}
		}

		/// <summary>
		/// Returns a MultivariateFunction h(x[]) defined by <pre> <code>
		/// h(x[]) = combiner(...combiner(combiner(initialValue,f(x[0])),f(x[1]))...),f(x[x.length-1]))
		/// </code></pre>
		/// </summary>
		/// <param name="combiner"> Combiner function. </param>
		/// <param name="f"> Function. </param>
		/// <param name="initialValue"> Initial value. </param>
		/// <returns> a collector function. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static MultivariateFunction collector(final BivariateFunction combiner, final UnivariateFunction f, final double initialValue)
		public static MultivariateFunction collector(BivariateFunction combiner, UnivariateFunction f, double initialValue)
		{
			return new MultivariateFunctionAnonymousInnerClassHelper(combiner, f, initialValue);
		}

		private class MultivariateFunctionAnonymousInnerClassHelper : MultivariateFunction
		{
			private org.apache.commons.math3.analysis.BivariateFunction combiner;
			private org.apache.commons.math3.analysis.UnivariateFunction f;
			private double initialValue;

			public MultivariateFunctionAnonymousInnerClassHelper(org.apache.commons.math3.analysis.BivariateFunction combiner, org.apache.commons.math3.analysis.UnivariateFunction f, double initialValue)
			{
				this.combiner = combiner;
				this.f = f;
				this.initialValue = initialValue;
			}

					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual double value(double[] point)
			{
				double result = combiner.value(initialValue, f.value(point[0]));
				for (int i = 1; i < point.Length; i++)
				{
					result = combiner.value(result, f.value(point[i]));
				}
				return result;
			}
		}

		/// <summary>
		/// Returns a MultivariateFunction h(x[]) defined by <pre> <code>
		/// h(x[]) = combiner(...combiner(combiner(initialValue,x[0]),x[1])...),x[x.length-1])
		/// </code></pre>
		/// </summary>
		/// <param name="combiner"> Combiner function. </param>
		/// <param name="initialValue"> Initial value. </param>
		/// <returns> a collector function. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static MultivariateFunction collector(final BivariateFunction combiner, final double initialValue)
		public static MultivariateFunction collector(BivariateFunction combiner, double initialValue)
		{
			return collector(combiner, new Identity(), initialValue);
		}

		/// <summary>
		/// Creates a unary function by fixing the first argument of a binary function.
		/// </summary>
		/// <param name="f"> Binary function. </param>
		/// <param name="fixed"> Value to which the first argument of {@code f} is set. </param>
		/// <returns> the unary function h(x) = f(fixed, x) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static UnivariateFunction fix1stArgument(final BivariateFunction f, final double fixed)
		public static UnivariateFunction fix1stArgument(BivariateFunction f, double @fixed)
		{
			return new UnivariateFunctionAnonymousInnerClassHelper8(f, @fixed);
		}

		private class UnivariateFunctionAnonymousInnerClassHelper8 : UnivariateFunction
		{
			private org.apache.commons.math3.analysis.BivariateFunction f;
			private double @fixed;

			public UnivariateFunctionAnonymousInnerClassHelper8(org.apache.commons.math3.analysis.BivariateFunction f, double @fixed)
			{
				this.f = f;
				this.@fixed = @fixed;
			}

					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual double value(double x)
			{
				return f.value(@fixed, x);
			}
		}
		/// <summary>
		/// Creates a unary function by fixing the second argument of a binary function.
		/// </summary>
		/// <param name="f"> Binary function. </param>
		/// <param name="fixed"> Value to which the second argument of {@code f} is set. </param>
		/// <returns> the unary function h(x) = f(x, fixed) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static UnivariateFunction fix2ndArgument(final BivariateFunction f, final double fixed)
		public static UnivariateFunction fix2ndArgument(BivariateFunction f, double @fixed)
		{
			return new UnivariateFunctionAnonymousInnerClassHelper9(f, @fixed);
		}

		private class UnivariateFunctionAnonymousInnerClassHelper9 : UnivariateFunction
		{
			private org.apache.commons.math3.analysis.BivariateFunction f;
			private double @fixed;

			public UnivariateFunctionAnonymousInnerClassHelper9(org.apache.commons.math3.analysis.BivariateFunction f, double @fixed)
			{
				this.f = f;
				this.@fixed = @fixed;
			}

					/// <summary>
					/// {@inheritDoc} </summary>
			public virtual double value(double x)
			{
				return f.value(x, @fixed);
			}
		}

		/// <summary>
		/// Samples the specified univariate real function on the specified interval.
		/// <br/>
		/// The interval is divided equally into {@code n} sections and sample points
		/// are taken from {@code min} to {@code max - (max - min) / n}; therefore
		/// {@code f} is not sampled at the upper bound {@code max}.
		/// </summary>
		/// <param name="f"> Function to be sampled </param>
		/// <param name="min"> Lower bound of the interval (included). </param>
		/// <param name="max"> Upper bound of the interval (excluded). </param>
		/// <param name="n"> Number of sample points. </param>
		/// <returns> the array of samples. </returns>
		/// <exception cref="NumberIsTooLargeException"> if the lower bound {@code min} is
		/// greater than, or equal to the upper bound {@code max}. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if the number of sample points
		/// {@code n} is negative. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double[] sample(UnivariateFunction f, double min, double max, int n) throws org.apache.commons.math3.exception.NumberIsTooLargeException, org.apache.commons.math3.exception.NotStrictlyPositiveException
		public static double[] sample(UnivariateFunction f, double min, double max, int n)
		{

			if (n <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.NOT_POSITIVE_NUMBER_OF_SAMPLES, Convert.ToInt32(n));
			}
			if (min >= max)
			{
				throw new NumberIsTooLargeException(min, max, false);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] s = new double[n];
			double[] s = new double[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double h = (max - min) / n;
			double h = (max - min) / n;
			for (int i = 0; i < n; i++)
			{
				s[i] = f.value(min + i * h);
			}
			return s;
		}

		/// <summary>
		/// Convert a <seealso cref="UnivariateDifferentiableFunction"/> into a <seealso cref="DifferentiableUnivariateFunction"/>. </summary>
		/// <param name="f"> function to convert </param>
		/// <returns> converted function </returns>
		/// @deprecated this conversion method is temporary in version 3.1, as the {@link
		/// DifferentiableUnivariateFunction} interface itself is deprecated 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("this conversion method is temporary in version 3.1, as the {@link") public static DifferentiableUnivariateFunction toDifferentiableUnivariateFunction(final org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction f)
		[Obsolete("this conversion method is temporary in version 3.1, as the {@link")]
		public static DifferentiableUnivariateFunction toDifferentiableUnivariateFunction(UnivariateDifferentiableFunction f)
		{
			return new DifferentiableUnivariateFunctionAnonymousInnerClassHelper4(f);
		}

		private class DifferentiableUnivariateFunctionAnonymousInnerClassHelper4 : DifferentiableUnivariateFunction
		{
			private UnivariateDifferentiableFunction f;

			public DifferentiableUnivariateFunctionAnonymousInnerClassHelper4(UnivariateDifferentiableFunction f)
			{
				this.f = f;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double value(final double x)
			public virtual double value(double x)
			{
				return f.value(x);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual UnivariateFunction derivative()
			{
				return new UnivariateFunctionAnonymousInnerClassHelper10(this);
			}

			private class UnivariateFunctionAnonymousInnerClassHelper10 : UnivariateFunction
			{
				private readonly DifferentiableUnivariateFunctionAnonymousInnerClassHelper4 outerInstance;

				public UnivariateFunctionAnonymousInnerClassHelper10(DifferentiableUnivariateFunctionAnonymousInnerClassHelper4 outerInstance)
				{
					this.outerInstance = outerInstance;
				}

							/// <summary>
							/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double value(final double x)
				public virtual double value(double x)
				{
					return outerInstance.f.value(new DerivativeStructure(1, 1, 0, x)).getPartialDerivative(1);
				}
			}

		}

		/// <summary>
		/// Convert a <seealso cref="DifferentiableUnivariateFunction"/> into a <seealso cref="UnivariateDifferentiableFunction"/>.
		/// <p>
		/// Note that the converted function is able to handle <seealso cref="DerivativeStructure"/> up to order one.
		/// If the function is called with higher order, a <seealso cref="NumberIsTooLargeException"/> will be thrown.
		/// </p> </summary>
		/// <param name="f"> function to convert </param>
		/// <returns> converted function </returns>
		/// @deprecated this conversion method is temporary in version 3.1, as the {@link
		/// DifferentiableUnivariateFunction} interface itself is deprecated 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("this conversion method is temporary in version 3.1, as the {@link") public static org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction toUnivariateDifferential(final DifferentiableUnivariateFunction f)
		[Obsolete("this conversion method is temporary in version 3.1, as the {@link")]
		public static UnivariateDifferentiableFunction toUnivariateDifferential(DifferentiableUnivariateFunction f)
		{
			return new UnivariateDifferentiableFunctionAnonymousInnerClassHelper4(f);
		}

		private class UnivariateDifferentiableFunctionAnonymousInnerClassHelper4 : UnivariateDifferentiableFunction
		{
			private org.apache.commons.math3.analysis.DifferentiableUnivariateFunction f;

			public UnivariateDifferentiableFunctionAnonymousInnerClassHelper4(org.apache.commons.math3.analysis.DifferentiableUnivariateFunction f)
			{
				this.f = f;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double value(final double x)
			public virtual double value(double x)
			{
				return f.value(x);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			/// <exception cref="NumberIsTooLargeException"> if derivation order is greater than 1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.analysis.differentiation.DerivativeStructure value(final org.apache.commons.math3.analysis.differentiation.DerivativeStructure t) throws org.apache.commons.math3.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			public virtual DerivativeStructure value(DerivativeStructure t)
			{
				switch (t.Order)
				{
					case 0 :
						return new DerivativeStructure(t.FreeParameters, 0, f.value(t.Value));
					case 1 :
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int parameters = t.getFreeParameters();
						int parameters = t.FreeParameters;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] derivatives = new double[parameters + 1];
						double[] derivatives = new double[parameters + 1];
						derivatives[0] = f.value(t.Value);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fPrime = f.derivative().value(t.getValue());
						double fPrime = f.derivative().value(t.Value);
						int[] orders = new int[parameters];
						for (int i = 0; i < parameters; ++i)
						{
							orders[i] = 1;
							derivatives[i + 1] = fPrime * t.getPartialDerivative(orders);
							orders[i] = 0;
						}
						return new DerivativeStructure(parameters, 1, derivatives);
					}
					default :
						throw new NumberIsTooLargeException(t.Order, 1, true);
				}
			}

		}

		/// <summary>
		/// Convert a <seealso cref="MultivariateDifferentiableFunction"/> into a <seealso cref="DifferentiableMultivariateFunction"/>. </summary>
		/// <param name="f"> function to convert </param>
		/// <returns> converted function </returns>
		/// @deprecated this conversion method is temporary in version 3.1, as the {@link
		/// DifferentiableMultivariateFunction} interface itself is deprecated 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("this conversion method is temporary in version 3.1, as the {@link") public static DifferentiableMultivariateFunction toDifferentiableMultivariateFunction(final org.apache.commons.math3.analysis.differentiation.MultivariateDifferentiableFunction f)
		[Obsolete("this conversion method is temporary in version 3.1, as the {@link")]
		public static DifferentiableMultivariateFunction toDifferentiableMultivariateFunction(MultivariateDifferentiableFunction f)
		{
			return new DifferentiableMultivariateFunctionAnonymousInnerClassHelper(f);
		}

		private class DifferentiableMultivariateFunctionAnonymousInnerClassHelper : DifferentiableMultivariateFunction
		{
			private MultivariateDifferentiableFunction f;

			public DifferentiableMultivariateFunctionAnonymousInnerClassHelper(MultivariateDifferentiableFunction f)
			{
				this.f = f;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double value(final double[] x)
			public virtual double value(double[] x)
			{
				return f.value(x);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MultivariateFunction partialDerivative(final int k)
			public virtual MultivariateFunction partialDerivative(int k)
			{
				return new MultivariateFunctionAnonymousInnerClassHelper2(this, k);
			}

			private class MultivariateFunctionAnonymousInnerClassHelper2 : MultivariateFunction
			{
				private readonly DifferentiableMultivariateFunctionAnonymousInnerClassHelper outerInstance;

				private int k;

				public MultivariateFunctionAnonymousInnerClassHelper2(DifferentiableMultivariateFunctionAnonymousInnerClassHelper outerInstance, int k)
				{
					this.outerInstance = outerInstance;
					this.k = k;
				}

							/// <summary>
							/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double value(final double[] x)
				public virtual double value(double[] x)
				{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = x.length;
					int n = x.Length;

					// delegate computation to underlying function
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.differentiation.DerivativeStructure[] dsX = new org.apache.commons.math3.analysis.differentiation.DerivativeStructure[n];
					DerivativeStructure[] dsX = new DerivativeStructure[n];
					for (int i = 0; i < n; ++i)
					{
						if (i == k)
						{
							dsX[i] = new DerivativeStructure(1, 1, 0, x[i]);
						}
						else
						{
							dsX[i] = new DerivativeStructure(1, 1, x[i]);
						}
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.differentiation.DerivativeStructure y = f.value(dsX);
					DerivativeStructure y = outerInstance.f.value(dsX);

					// extract partial derivative
					return y.getPartialDerivative(1);

				}
			}

			public virtual MultivariateVectorFunction gradient()
			{
				return new MultivariateVectorFunctionAnonymousInnerClassHelper(this);
			}

			private class MultivariateVectorFunctionAnonymousInnerClassHelper : MultivariateVectorFunction
			{
				private readonly DifferentiableMultivariateFunctionAnonymousInnerClassHelper outerInstance;

				public MultivariateVectorFunctionAnonymousInnerClassHelper(DifferentiableMultivariateFunctionAnonymousInnerClassHelper outerInstance)
				{
					this.outerInstance = outerInstance;
				}

							/// <summary>
							/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double[] value(final double[] x)
				public virtual double[] value(double[] x)
				{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = x.length;
					int n = x.Length;

					// delegate computation to underlying function
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.differentiation.DerivativeStructure[] dsX = new org.apache.commons.math3.analysis.differentiation.DerivativeStructure[n];
					DerivativeStructure[] dsX = new DerivativeStructure[n];
					for (int i = 0; i < n; ++i)
					{
						dsX[i] = new DerivativeStructure(n, 1, i, x[i]);
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.differentiation.DerivativeStructure y = f.value(dsX);
					DerivativeStructure y = outerInstance.f.value(dsX);

					// extract gradient
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] gradient = new double[n];
					double[] gradient = new double[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] orders = new int[n];
					int[] orders = new int[n];
					for (int i = 0; i < n; ++i)
					{
						orders[i] = 1;
						gradient[i] = y.getPartialDerivative(orders);
						orders[i] = 0;
					}

					return gradient;

				}
			}

		}

		/// <summary>
		/// Convert a <seealso cref="DifferentiableMultivariateFunction"/> into a <seealso cref="MultivariateDifferentiableFunction"/>.
		/// <p>
		/// Note that the converted function is able to handle <seealso cref="DerivativeStructure"/> elements
		/// that all have the same number of free parameters and order, and with order at most 1.
		/// If the function is called with inconsistent numbers of free parameters or higher order, a
		/// <seealso cref="DimensionMismatchException"/> or a <seealso cref="NumberIsTooLargeException"/> will be thrown.
		/// </p> </summary>
		/// <param name="f"> function to convert </param>
		/// <returns> converted function </returns>
		/// @deprecated this conversion method is temporary in version 3.1, as the {@link
		/// DifferentiableMultivariateFunction} interface itself is deprecated 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("this conversion method is temporary in version 3.1, as the {@link") public static org.apache.commons.math3.analysis.differentiation.MultivariateDifferentiableFunction toMultivariateDifferentiableFunction(final DifferentiableMultivariateFunction f)
		[Obsolete("this conversion method is temporary in version 3.1, as the {@link")]
		public static MultivariateDifferentiableFunction toMultivariateDifferentiableFunction(DifferentiableMultivariateFunction f)
		{
			return new MultivariateDifferentiableFunctionAnonymousInnerClassHelper(f);
		}

		private class MultivariateDifferentiableFunctionAnonymousInnerClassHelper : MultivariateDifferentiableFunction
		{
			private org.apache.commons.math3.analysis.DifferentiableMultivariateFunction f;

			public MultivariateDifferentiableFunctionAnonymousInnerClassHelper(org.apache.commons.math3.analysis.DifferentiableMultivariateFunction f)
			{
				this.f = f;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double value(final double[] x)
			public virtual double value(double[] x)
			{
				return f.value(x);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			/// <exception cref="NumberIsTooLargeException"> if derivation order is higher than 1 </exception>
			/// <exception cref="DimensionMismatchException"> if numbers of free parameters are inconsistent </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.analysis.differentiation.DerivativeStructure value(final org.apache.commons.math3.analysis.differentiation.DerivativeStructure[] t) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			public virtual DerivativeStructure value(DerivativeStructure[] t)
			{

				// check parameters and orders limits
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int parameters = t[0].getFreeParameters();
				int parameters = t[0].FreeParameters;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int order = t[0].getOrder();
				int order = t[0].Order;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = t.length;
				int n = t.Length;
				if (order > 1)
				{
					throw new NumberIsTooLargeException(order, 1, true);
				}

				// check all elements in the array are consistent
				for (int i = 0; i < n; ++i)
				{
					if (t[i].FreeParameters != parameters)
					{
						throw new DimensionMismatchException(t[i].FreeParameters, parameters);
					}

					if (t[i].Order != order)
					{
						throw new DimensionMismatchException(t[i].Order, order);
					}
				}

				// delegate computation to underlying function
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] point = new double[n];
				double[] point = new double[n];
				for (int i = 0; i < n; ++i)
				{
					point[i] = t[i].Value;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double value = f.value(point);
				double value = f.value(point);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] gradient = f.gradient().value(point);
				double[] gradient = f.gradient().value(point);

				// merge value and gradient into one DerivativeStructure
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] derivatives = new double[parameters + 1];
				double[] derivatives = new double[parameters + 1];
				derivatives[0] = value;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] orders = new int[parameters];
				int[] orders = new int[parameters];
				for (int i = 0; i < parameters; ++i)
				{
					orders[i] = 1;
					for (int j = 0; j < n; ++j)
					{
						derivatives[i + 1] += gradient[j] * t[j].getPartialDerivative(orders);
					}
					orders[i] = 0;
				}

				return new DerivativeStructure(parameters, order, derivatives);

			}

		}

		/// <summary>
		/// Convert a <seealso cref="MultivariateDifferentiableVectorFunction"/> into a <seealso cref="DifferentiableMultivariateVectorFunction"/>. </summary>
		/// <param name="f"> function to convert </param>
		/// <returns> converted function </returns>
		/// @deprecated this conversion method is temporary in version 3.1, as the {@link
		/// DifferentiableMultivariateVectorFunction} interface itself is deprecated 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("this conversion method is temporary in version 3.1, as the {@link") public static DifferentiableMultivariateVectorFunction toDifferentiableMultivariateVectorFunction(final org.apache.commons.math3.analysis.differentiation.MultivariateDifferentiableVectorFunction f)
		[Obsolete("this conversion method is temporary in version 3.1, as the {@link")]
		public static DifferentiableMultivariateVectorFunction toDifferentiableMultivariateVectorFunction(MultivariateDifferentiableVectorFunction f)
		{
			return new DifferentiableMultivariateVectorFunctionAnonymousInnerClassHelper(f);
		}

		private class DifferentiableMultivariateVectorFunctionAnonymousInnerClassHelper : DifferentiableMultivariateVectorFunction
		{
			private MultivariateDifferentiableVectorFunction f;

			public DifferentiableMultivariateVectorFunctionAnonymousInnerClassHelper(MultivariateDifferentiableVectorFunction f)
			{
				this.f = f;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double[] value(final double[] x)
			public virtual double[] value(double[] x)
			{
				return f.value(x);
			}

			public virtual MultivariateMatrixFunction jacobian()
			{
				return new MultivariateMatrixFunctionAnonymousInnerClassHelper(this);
			}

			private class MultivariateMatrixFunctionAnonymousInnerClassHelper : MultivariateMatrixFunction
			{
				private readonly DifferentiableMultivariateVectorFunctionAnonymousInnerClassHelper outerInstance;

				public MultivariateMatrixFunctionAnonymousInnerClassHelper(DifferentiableMultivariateVectorFunctionAnonymousInnerClassHelper outerInstance)
				{
					this.outerInstance = outerInstance;
				}

							/// <summary>
							/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double[][] value(final double[] x)
				public virtual double[][] value(double[] x)
				{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = x.length;
					int n = x.Length;

					// delegate computation to underlying function
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.differentiation.DerivativeStructure[] dsX = new org.apache.commons.math3.analysis.differentiation.DerivativeStructure[n];
					DerivativeStructure[] dsX = new DerivativeStructure[n];
					for (int i = 0; i < n; ++i)
					{
						dsX[i] = new DerivativeStructure(n, 1, i, x[i]);
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.differentiation.DerivativeStructure[] y = f.value(dsX);
					DerivativeStructure[] y = outerInstance.f.value(dsX);

					// extract Jacobian
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] jacobian = new double[y.length][n];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] jacobian = new double[y.Length][n];
					double[][] jacobian = RectangularArrays.ReturnRectangularDoubleArray(y.Length, n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] orders = new int[n];
					int[] orders = new int[n];
					for (int i = 0; i < y.Length; ++i)
					{
						for (int j = 0; j < n; ++j)
						{
							orders[j] = 1;
							jacobian[i][j] = y[i].getPartialDerivative(orders);
							orders[j] = 0;
						}
					}

					return jacobian;

				}
			}

		}

		/// <summary>
		/// Convert a <seealso cref="DifferentiableMultivariateVectorFunction"/> into a <seealso cref="MultivariateDifferentiableVectorFunction"/>.
		/// <p>
		/// Note that the converted function is able to handle <seealso cref="DerivativeStructure"/> elements
		/// that all have the same number of free parameters and order, and with order at most 1.
		/// If the function is called with inconsistent numbers of free parameters or higher order, a
		/// <seealso cref="DimensionMismatchException"/> or a <seealso cref="NumberIsTooLargeException"/> will be thrown.
		/// </p> </summary>
		/// <param name="f"> function to convert </param>
		/// <returns> converted function </returns>
		/// @deprecated this conversion method is temporary in version 3.1, as the {@link
		/// DifferentiableMultivariateFunction} interface itself is deprecated 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("this conversion method is temporary in version 3.1, as the {@link") public static org.apache.commons.math3.analysis.differentiation.MultivariateDifferentiableVectorFunction toMultivariateDifferentiableVectorFunction(final DifferentiableMultivariateVectorFunction f)
		[Obsolete("this conversion method is temporary in version 3.1, as the {@link")]
		public static MultivariateDifferentiableVectorFunction toMultivariateDifferentiableVectorFunction(DifferentiableMultivariateVectorFunction f)
		{
			return new MultivariateDifferentiableVectorFunctionAnonymousInnerClassHelper(f);
		}

		private class MultivariateDifferentiableVectorFunctionAnonymousInnerClassHelper : MultivariateDifferentiableVectorFunction
		{
			private org.apache.commons.math3.analysis.DifferentiableMultivariateVectorFunction f;

			public MultivariateDifferentiableVectorFunctionAnonymousInnerClassHelper(org.apache.commons.math3.analysis.DifferentiableMultivariateVectorFunction f)
			{
				this.f = f;
			}


					/// <summary>
					/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double[] value(final double[] x)
			public virtual double[] value(double[] x)
			{
				return f.value(x);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			/// <exception cref="NumberIsTooLargeException"> if derivation order is higher than 1 </exception>
			/// <exception cref="DimensionMismatchException"> if numbers of free parameters are inconsistent </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.analysis.differentiation.DerivativeStructure[] value(final org.apache.commons.math3.analysis.differentiation.DerivativeStructure[] t) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			public virtual DerivativeStructure[] value(DerivativeStructure[] t)
			{

				// check parameters and orders limits
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int parameters = t[0].getFreeParameters();
				int parameters = t[0].FreeParameters;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int order = t[0].getOrder();
				int order = t[0].Order;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = t.length;
				int n = t.Length;
				if (order > 1)
				{
					throw new NumberIsTooLargeException(order, 1, true);
				}

				// check all elements in the array are consistent
				for (int i = 0; i < n; ++i)
				{
					if (t[i].FreeParameters != parameters)
					{
						throw new DimensionMismatchException(t[i].FreeParameters, parameters);
					}

					if (t[i].Order != order)
					{
						throw new DimensionMismatchException(t[i].Order, order);
					}
				}

				// delegate computation to underlying function
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] point = new double[n];
				double[] point = new double[n];
				for (int i = 0; i < n; ++i)
				{
					point[i] = t[i].Value;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] value = f.value(point);
				double[] value = f.value(point);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] jacobian = f.jacobian().value(point);
				double[][] jacobian = f.jacobian().value(point);

				// merge value and Jacobian into a DerivativeStructure array
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.differentiation.DerivativeStructure[] merged = new org.apache.commons.math3.analysis.differentiation.DerivativeStructure[value.length];
				DerivativeStructure[] merged = new DerivativeStructure[value.Length];
				for (int k = 0; k < merged.Length; ++k)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] derivatives = new double[parameters + 1];
					double[] derivatives = new double[parameters + 1];
					derivatives[0] = value[k];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] orders = new int[parameters];
					int[] orders = new int[parameters];
					for (int i = 0; i < parameters; ++i)
					{
						orders[i] = 1;
						for (int j = 0; j < n; ++j)
						{
							derivatives[i + 1] += jacobian[k][j] * t[j].getPartialDerivative(orders);
						}
						orders[i] = 0;
					}
					merged[k] = new DerivativeStructure(parameters, order, derivatives);
				}

				return merged;

			}

		}

	}

}
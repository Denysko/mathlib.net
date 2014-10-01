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

namespace mathlib.analysis.function
{

	using DerivativeStructure = mathlib.analysis.differentiation.DerivativeStructure;
	using UnivariateDifferentiableFunction = mathlib.analysis.differentiation.UnivariateDifferentiableFunction;

	/// <summary>
	/// Constant function.
	/// 
	/// @since 3.0
	/// @version $Id: Constant.java 1424087 2012-12-19 20:32:50Z luc $
	/// </summary>
	public class Constant : UnivariateDifferentiableFunction, DifferentiableUnivariateFunction
	{
		/// <summary>
		/// Constant. </summary>
		private readonly double c;

		/// <param name="c"> Constant. </param>
		public Constant(double c)
		{
			this.c = c;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double value(double x)
		{
			return c;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// @deprecated as of 3.1, replaced by <seealso cref="#value(DerivativeStructure)"/> 
		[Obsolete]//("as of 3.1, replaced by <seealso cref="#value(mathlib.analysis.differentiation.DerivativeStructure)"/>")]
		public virtual DifferentiableUnivariateFunction derivative()
		{
			return new Constant(0);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.analysis.differentiation.DerivativeStructure value(final mathlib.analysis.differentiation.DerivativeStructure t)
		public virtual DerivativeStructure value(DerivativeStructure t)
		{
			return new DerivativeStructure(t.FreeParameters, t.Order, c);
		}

	}

}
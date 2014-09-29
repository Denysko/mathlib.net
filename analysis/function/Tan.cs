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
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Tangent function.
	/// 
	/// @since 3.0
	/// @version $Id: Tan.java 1383441 2012-09-11 14:56:39Z luc $
	/// </summary>
	public class Tan : UnivariateDifferentiableFunction, DifferentiableUnivariateFunction
	{
		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double value(double x)
		{
			return FastMath.tan(x);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// @deprecated as of 3.1, replaced by <seealso cref="#value(DerivativeStructure)"/> 
		[Obsolete("as of 3.1, replaced by <seealso cref="#value(mathlib.analysis.differentiation.DerivativeStructure)"/>")]
		public virtual UnivariateFunction derivative()
		{
			return FunctionUtils.toDifferentiableUnivariateFunction(this).derivative();
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.analysis.differentiation.DerivativeStructure value(final mathlib.analysis.differentiation.DerivativeStructure t)
		public virtual DerivativeStructure value(DerivativeStructure t)
		{
			return t.tan();
		}

	}

}
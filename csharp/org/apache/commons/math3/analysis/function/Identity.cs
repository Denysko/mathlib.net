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

namespace org.apache.commons.math3.analysis.function
{

	using DerivativeStructure = org.apache.commons.math3.analysis.differentiation.DerivativeStructure;
	using UnivariateDifferentiableFunction = org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction;

	/// <summary>
	/// Identity function.
	/// 
	/// @since 3.0
	/// @version $Id: Identity.java 1424087 2012-12-19 20:32:50Z luc $
	/// </summary>
	public class Identity : UnivariateDifferentiableFunction, DifferentiableUnivariateFunction
	{
		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double value(double x)
		{
			return x;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// @deprecated as of 3.1, replaced by <seealso cref="#value(DerivativeStructure)"/> 
		[Obsolete("as of 3.1, replaced by <seealso cref="#value(org.apache.commons.math3.analysis.differentiation.DerivativeStructure)"/>")]
		public virtual DifferentiableUnivariateFunction derivative()
		{
			return new Constant(1);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.analysis.differentiation.DerivativeStructure value(final org.apache.commons.math3.analysis.differentiation.DerivativeStructure t)
		public virtual DerivativeStructure value(DerivativeStructure t)
		{
			return t;
		}

	}

}
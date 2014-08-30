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

namespace org.apache.commons.math3.optimization
{

	using MultivariateDifferentiableVectorFunction = org.apache.commons.math3.analysis.differentiation.MultivariateDifferentiableVectorFunction;
	using RandomVectorGenerator = org.apache.commons.math3.random.RandomVectorGenerator;

	/// <summary>
	/// Special implementation of the <seealso cref="MultivariateDifferentiableVectorOptimizer"/>
	/// interface adding multi-start features to an existing optimizer.
	/// 
	/// This class wraps a classical optimizer to use it several times in
	/// turn with different starting points in order to avoid being trapped
	/// into a local extremum when looking for a global one.
	/// 
	/// @version $Id: MultivariateDifferentiableVectorMultiStartOptimizer.java 1422230 2012-12-15 12:11:13Z erans $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.1 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public class MultivariateDifferentiableVectorMultiStartOptimizer : BaseMultivariateVectorMultiStartOptimizer<MultivariateDifferentiableVectorFunction>, MultivariateDifferentiableVectorOptimizer
	{
		/// <summary>
		/// Create a multi-start optimizer from a single-start optimizer.
		/// </summary>
		/// <param name="optimizer"> Single-start optimizer to wrap. </param>
		/// <param name="starts"> Number of starts to perform (including the
		/// first one), multi-start is disabled if value is less than or
		/// equal to 1. </param>
		/// <param name="generator"> Random vector generator to use for restarts. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MultivariateDifferentiableVectorMultiStartOptimizer(final MultivariateDifferentiableVectorOptimizer optimizer, final int starts, final org.apache.commons.math3.random.RandomVectorGenerator generator)
		public MultivariateDifferentiableVectorMultiStartOptimizer(MultivariateDifferentiableVectorOptimizer optimizer, int starts, RandomVectorGenerator generator) : base(optimizer, starts, generator)
		{
		}
	}

}
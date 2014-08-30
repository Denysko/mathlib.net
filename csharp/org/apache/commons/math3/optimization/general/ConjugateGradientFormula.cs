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

namespace org.apache.commons.math3.optimization.general
{

	/// <summary>
	/// Available choices of update formulas for the &beta; parameter
	/// in <seealso cref="NonLinearConjugateGradientOptimizer"/>.
	/// <p>
	/// The &beta; parameter is used to compute the successive conjugate
	/// search directions. For non-linear conjugate gradients, there are
	/// two formulas to compute &beta;:
	/// <ul>
	///   <li>Fletcher-Reeves formula</li>
	///   <li>Polak-Ribi&egrave;re formula</li>
	/// </ul>
	/// On the one hand, the Fletcher-Reeves formula is guaranteed to converge
	/// if the start point is close enough of the optimum whether the
	/// Polak-Ribi&egrave;re formula may not converge in rare cases. On the
	/// other hand, the Polak-Ribi&egrave;re formula is often faster when it
	/// does converge. Polak-Ribi&egrave;re is often used.
	/// <p> </summary>
	/// <seealso cref= NonLinearConjugateGradientOptimizer
	/// @version $Id: ConjugateGradientFormula.java 1422230 2012-12-15 12:11:13Z erans $ </seealso>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 2.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public enum ConjugateGradientFormula
	{

		/// <summary>
		/// Fletcher-Reeves formula. </summary>
		FLETCHER_REEVES,

		/// <summary>
		/// Polak-Ribi&egrave;re formula. </summary>
		POLAK_RIBIERE

	}

}
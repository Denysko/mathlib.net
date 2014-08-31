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

namespace org.apache.commons.math3.optimization.linear
{

	using MathIllegalStateException = org.apache.commons.math3.exception.MathIllegalStateException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// This class represents exceptions thrown by optimizers when a solution escapes to infinity.
	/// 
	/// @version $Id: UnboundedSolutionException.java 1422230 2012-12-15 12:11:13Z erans $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 2.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public class UnboundedSolutionException : MathIllegalStateException
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 940539497277290619L;

		/// <summary>
		/// Simple constructor using a default message.
		/// </summary>
		public UnboundedSolutionException() : base(LocalizedFormats.UNBOUNDED_SOLUTION)
		{
		}

	}

}
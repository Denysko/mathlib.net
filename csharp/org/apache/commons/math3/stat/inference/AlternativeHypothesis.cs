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
namespace org.apache.commons.math3.stat.inference
{

	/// <summary>
	/// Represents an alternative hypothesis for a hypothesis test.
	/// 
	/// @version $Id: AlternativeHypothesis.java 1531128 2013-10-10 22:09:25Z tn $
	/// @since 3.3
	/// </summary>
	public enum AlternativeHypothesis
	{

		/// <summary>
		/// Represents a two-sided test. H0: p=p0, H1: p &ne; p0
		/// </summary>
		TWO_SIDED,

		/// <summary>
		/// Represents a right-sided test. H0: p &le; p0, H1: p &gt; p0.
		/// </summary>
		GREATER_THAN,

		/// <summary>
		/// Represents a left-sided test. H0: p &ge; p0, H1: p &lt; p0.
		/// </summary>
		LESS_THAN
	}

}
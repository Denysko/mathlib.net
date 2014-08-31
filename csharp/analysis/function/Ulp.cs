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

	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// {@code ulp} function.
	/// 
	/// @since 3.0
	/// @version $Id: Ulp.java 1364377 2012-07-22 17:39:16Z tn $
	/// </summary>
	public class Ulp : UnivariateFunction
	{
		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double value(double x)
		{
			return FastMath.ulp(x);
		}
	}

}
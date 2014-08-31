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
namespace org.apache.commons.math3.ml.distance
{

	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Calculates the Canberra distance between two points.
	/// 
	/// @version $Id: CanberraDistance.java 1519184 2013-08-31 15:22:59Z tn $
	/// @since 3.2
	/// </summary>
	public class CanberraDistance : DistanceMeasure
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -6972277381587032228L;

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double compute(double[] a, double[] b)
		{
			double sum = 0;
			for (int i = 0; i < a.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double num = org.apache.commons.math3.util.FastMath.abs(a[i] - b[i]);
				double num = FastMath.abs(a[i] - b[i]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double denom = org.apache.commons.math3.util.FastMath.abs(a[i]) + org.apache.commons.math3.util.FastMath.abs(b[i]);
				double denom = FastMath.abs(a[i]) + FastMath.abs(b[i]);
				sum += num == 0.0 && denom == 0.0 ? 0.0 : num / denom;
			}
			return sum;
		}

	}

}
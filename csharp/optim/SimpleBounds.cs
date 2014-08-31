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
namespace org.apache.commons.math3.optim
{

	/// <summary>
	/// Simple optimization constraints: lower and upper bounds.
	/// The valid range of the parameters is an interval that can be infinite
	/// (in one or both directions).
	/// <br/>
	/// Immutable class.
	/// 
	/// @version $Id: SimpleBounds.java 1591835 2014-05-02 09:04:01Z tn $
	/// @since 3.1
	/// </summary>
	public class SimpleBounds : OptimizationData
	{
		/// <summary>
		/// Lower bounds. </summary>
		private readonly double[] lower;
		/// <summary>
		/// Upper bounds. </summary>
		private readonly double[] upper;

		/// <param name="lB"> Lower bounds. </param>
		/// <param name="uB"> Upper bounds. </param>
		public SimpleBounds(double[] lB, double[] uB)
		{
			lower = lB.clone();
			upper = uB.clone();
		}

		/// <summary>
		/// Gets the lower bounds.
		/// </summary>
		/// <returns> the lower bounds. </returns>
		public virtual double[] Lower
		{
			get
			{
				return lower.clone();
			}
		}
		/// <summary>
		/// Gets the upper bounds.
		/// </summary>
		/// <returns> the upper bounds. </returns>
		public virtual double[] Upper
		{
			get
			{
				return upper.clone();
			}
		}

		/// <summary>
		/// Factory method that creates instance of this class that represents
		/// unbounded ranges.
		/// </summary>
		/// <param name="dim"> Number of parameters. </param>
		/// <returns> a new instance suitable for passing to an optimizer that
		/// requires bounds specification. </returns>
		public static SimpleBounds unbounded(int dim)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] lB = new double[dim];
			double[] lB = new double[dim];
			Arrays.fill(lB, double.NegativeInfinity);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] uB = new double[dim];
			double[] uB = new double[dim];
			Arrays.fill(uB, double.PositiveInfinity);

			return new SimpleBounds(lB, uB);
		}
	}

}
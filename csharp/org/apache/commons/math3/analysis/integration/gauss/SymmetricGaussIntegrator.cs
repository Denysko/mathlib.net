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
namespace org.apache.commons.math3.analysis.integration.gauss
{

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NonMonotonicSequenceException = org.apache.commons.math3.exception.NonMonotonicSequenceException;
	using org.apache.commons.math3.util;

	/// <summary>
	/// This class's implements <seealso cref="#integrate(UnivariateFunction) integrate"/>
	/// method assuming that the integral is symmetric about 0.
	/// This allows to reduce numerical errors.
	/// 
	/// @since 3.3
	/// @version $Id: SymmetricGaussIntegrator.java 1509234 2013-08-01 13:48:57Z erans $
	/// </summary>
	public class SymmetricGaussIntegrator : GaussIntegrator
	{
		/// <summary>
		/// Creates an integrator from the given {@code points} and {@code weights}.
		/// The integration interval is defined by the first and last value of
		/// {@code points} which must be sorted in increasing order.
		/// </summary>
		/// <param name="points"> Integration points. </param>
		/// <param name="weights"> Weights of the corresponding integration nodes. </param>
		/// <exception cref="NonMonotonicSequenceException"> if the {@code points} are not
		/// sorted in increasing order. </exception>
		/// <exception cref="DimensionMismatchException"> if points and weights don't have the same length </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SymmetricGaussIntegrator(double[] points, double[] weights) throws org.apache.commons.math3.exception.NonMonotonicSequenceException, org.apache.commons.math3.exception.DimensionMismatchException
		public SymmetricGaussIntegrator(double[] points, double[] weights) : base(points, weights)
		{
		}

		/// <summary>
		/// Creates an integrator from the given pair of points (first element of
		/// the pair) and weights (second element of the pair.
		/// </summary>
		/// <param name="pointsAndWeights"> Integration points and corresponding weights. </param>
		/// <exception cref="NonMonotonicSequenceException"> if the {@code points} are not
		/// sorted in increasing order.
		/// </exception>
		/// <seealso cref= #SymmetricGaussIntegrator(double[], double[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SymmetricGaussIntegrator(org.apache.commons.math3.util.Pair<double[] , double[]> pointsAndWeights) throws org.apache.commons.math3.exception.NonMonotonicSequenceException
		public SymmetricGaussIntegrator(Pair<double[], double[]> pointsAndWeights) : this(pointsAndWeights.First, pointsAndWeights.Second)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double integrate(UnivariateFunction f)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ruleLength = getNumberOfPoints();
			int ruleLength = NumberOfPoints;

			if (ruleLength == 1)
			{
				return getWeight(0) * f.value(0d);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iMax = ruleLength / 2;
			int iMax = ruleLength / 2;
			double s = 0;
			double c = 0;
			for (int i = 0; i < iMax; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double p = getPoint(i);
				double p = getPoint(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double w = getWeight(i);
				double w = getWeight(i);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double f1 = f.value(p);
				double f1 = f.value(p);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double f2 = f.value(-p);
				double f2 = f.value(-p);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = w * (f1 + f2) - c;
				double y = w * (f1 + f2) - c;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = s + y;
				double t = s + y;

				c = (t - s) - y;
				s = t;
			}

			if (ruleLength % 2 != 0)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double w = getWeight(iMax);
				double w = getWeight(iMax);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = w * f.value(0d) - c;
				double y = w * f.value(0d) - c;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = s + y;
				double t = s + y;

				s = t;
			}

			return s;
		}
	}

}
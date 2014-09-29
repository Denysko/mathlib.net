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
namespace mathlib.stat.correlation
{

	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;

	/// <summary>
	/// Bivariate Covariance implementation that does not require input data to be
	/// stored in memory.
	/// 
	/// <p>This class is based on a paper written by Philippe P&eacute;bay:
	/// <a href="http://prod.sandia.gov/techlib/access-control.cgi/2008/086212.pdf">
	/// Formulas for Robust, One-Pass Parallel Computation of Covariances and
	/// Arbitrary-Order Statistical Moments</a>, 2008, Technical Report SAND2008-6212,
	/// Sandia National Laboratories. It computes the covariance for a pair of variables.
	/// Use <seealso cref="StorelessCovariance"/> to estimate an entire covariance matrix.</p>
	/// 
	/// <p>Note: This class is package private as it is only used internally in
	/// the <seealso cref="StorelessCovariance"/> class.</p>
	/// 
	/// @version $Id: StorelessBivariateCovariance.java 1488337 2013-05-31 17:47:53Z psteitz $
	/// @since 3.0
	/// </summary>
	internal class StorelessBivariateCovariance
	{

		/// <summary>
		/// the mean of variable x </summary>
		private double meanX;

		/// <summary>
		/// the mean of variable y </summary>
		private double meanY;

		/// <summary>
		/// number of observations </summary>
		private double n;

		/// <summary>
		/// the running covariance estimate </summary>
		private double covarianceNumerator;

		/// <summary>
		/// flag for bias correction </summary>
		private bool biasCorrected;

		/// <summary>
		/// Create an empty <seealso cref="StorelessBivariateCovariance"/> instance with
		/// bias correction.
		/// </summary>
		public StorelessBivariateCovariance() : this(true)
		{
		}

		/// <summary>
		/// Create an empty <seealso cref="StorelessBivariateCovariance"/> instance.
		/// </summary>
		/// <param name="biasCorrection"> if <code>true</code> the covariance estimate is corrected
		/// for bias, i.e. n-1 in the denominator, otherwise there is no bias correction,
		/// i.e. n in the denominator. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public StorelessBivariateCovariance(final boolean biasCorrection)
		public StorelessBivariateCovariance(bool biasCorrection)
		{
			meanX = meanY = 0.0;
			n = 0;
			covarianceNumerator = 0.0;
			biasCorrected = biasCorrection;
		}

		/// <summary>
		/// Update the covariance estimation with a pair of variables (x, y).
		/// </summary>
		/// <param name="x"> the x value </param>
		/// <param name="y"> the y value </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void increment(final double x, final double y)
		public virtual void increment(double x, double y)
		{
			n++;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double deltaX = x - meanX;
			double deltaX = x - meanX;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double deltaY = y - meanY;
			double deltaY = y - meanY;
			meanX += deltaX / n;
			meanY += deltaY / n;
			covarianceNumerator += ((n - 1.0) / n) * deltaX * deltaY;
		}

		/// <summary>
		/// Appends another bivariate covariance calculation to this.
		/// After this operation, statistics returned should be close to what would
		/// have been obtained by by performing all of the <seealso cref="#increment(double, double)"/>
		/// operations in {@code cov} directly on this.
		/// </summary>
		/// <param name="cov"> StorelessBivariateCovariance instance to append. </param>
		public virtual void append(StorelessBivariateCovariance cov)
		{
			double oldN = n;
			n += cov.n;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double deltaX = cov.meanX - meanX;
			double deltaX = cov.meanX - meanX;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double deltaY = cov.meanY - meanY;
			double deltaY = cov.meanY - meanY;
			meanX += deltaX * cov.n / n;
			meanY += deltaY * cov.n / n;
			covarianceNumerator += cov.covarianceNumerator + oldN * cov.n / n * deltaX * deltaY;
		}

		/// <summary>
		/// Returns the number of observations.
		/// </summary>
		/// <returns> number of observations </returns>
		public virtual double N
		{
			get
			{
				return n;
			}
		}

		/// <summary>
		/// Return the current covariance estimate.
		/// </summary>
		/// <returns> the current covariance </returns>
		/// <exception cref="NumberIsTooSmallException"> if the number of observations
		/// is &lt; 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getResult() throws mathlib.exception.NumberIsTooSmallException
		public virtual double Result
		{
			get
			{
				if (n < 2)
				{
					throw new NumberIsTooSmallException(LocalizedFormats.INSUFFICIENT_DIMENSION, n, 2, true);
				}
				if (biasCorrected)
				{
					return covarianceNumerator / (n - 1d);
				}
				else
				{
					return covarianceNumerator / n;
				}
			}
		}
	}


}
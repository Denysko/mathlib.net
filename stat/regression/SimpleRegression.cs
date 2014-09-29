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

namespace mathlib.stat.regression
{

	using TDistribution = mathlib.distribution.TDistribution;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NoDataException = mathlib.exception.NoDataException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using FastMath = mathlib.util.FastMath;
	using Precision = mathlib.util.Precision;

	/// <summary>
	/// Estimates an ordinary least squares regression model
	/// with one independent variable.
	/// <p>
	/// <code> y = intercept + slope * x  </code></p>
	/// <p>
	/// Standard errors for <code>intercept</code> and <code>slope</code> are
	/// available as well as ANOVA, r-square and Pearson's r statistics.</p>
	/// <p>
	/// Observations (x,y pairs) can be added to the model one at a time or they
	/// can be provided in a 2-dimensional array.  The observations are not stored
	/// in memory, so there is no limit to the number of observations that can be
	/// added to the model.</p>
	/// <p>
	/// <strong>Usage Notes</strong>: <ul>
	/// <li> When there are fewer than two observations in the model, or when
	/// there is no variation in the x values (i.e. all x values are the same)
	/// all statistics return <code>NaN</code>. At least two observations with
	/// different x coordinates are required to estimate a bivariate regression
	/// model.
	/// </li>
	/// <li> Getters for the statistics always compute values based on the current
	/// set of observations -- i.e., you can get statistics, then add more data
	/// and get updated statistics without using a new instance.  There is no
	/// "compute" method that updates all statistics.  Each of the getters performs
	/// the necessary computations to return the requested statistic.
	/// </li>
	/// <li> The intercept term may be suppressed by passing {@code false} to
	/// the <seealso cref="#SimpleRegression(boolean)"/> constructor.  When the
	/// {@code hasIntercept} property is false, the model is estimated without a
	/// constant term and <seealso cref="#getIntercept()"/> returns {@code 0}.</li>
	/// </ul></p>
	/// 
	/// @version $Id: SimpleRegression.java 1519851 2013-09-03 21:16:35Z tn $
	/// </summary>
	[Serializable]
	public class SimpleRegression : UpdatingMultipleLinearRegression
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -3004689053607543335L;

		/// <summary>
		/// sum of x values </summary>
		private double sumX = 0d;

		/// <summary>
		/// total variation in x (sum of squared deviations from xbar) </summary>
		private double sumXX = 0d;

		/// <summary>
		/// sum of y values </summary>
		private double sumY = 0d;

		/// <summary>
		/// total variation in y (sum of squared deviations from ybar) </summary>
		private double sumYY = 0d;

		/// <summary>
		/// sum of products </summary>
		private double sumXY = 0d;

		/// <summary>
		/// number of observations </summary>
		private long n = 0;

		/// <summary>
		/// mean of accumulated x values, used in updating formulas </summary>
		private double xbar = 0;

		/// <summary>
		/// mean of accumulated y values, used in updating formulas </summary>
		private double ybar = 0;

		/// <summary>
		/// include an intercept or not </summary>
		private readonly bool hasIntercept_Renamed;
		// ---------------------Public methods--------------------------------------

		/// <summary>
		/// Create an empty SimpleRegression instance
		/// </summary>
		public SimpleRegression() : this(true)
		{
		}
		/// <summary>
		/// Create a SimpleRegression instance, specifying whether or not to estimate
		/// an intercept.
		/// 
		/// <p>Use {@code false} to estimate a model with no intercept.  When the
		/// {@code hasIntercept} property is false, the model is estimated without a
		/// constant term and <seealso cref="#getIntercept()"/> returns {@code 0}.</p>
		/// </summary>
		/// <param name="includeIntercept"> whether or not to include an intercept term in
		/// the regression model </param>
		public SimpleRegression(bool includeIntercept) : base()
		{
			hasIntercept_Renamed = includeIntercept;
		}

		/// <summary>
		/// Adds the observation (x,y) to the regression data set.
		/// <p>
		/// Uses updating formulas for means and sums of squares defined in
		/// "Algorithms for Computing the Sample Variance: Analysis and
		/// Recommendations", Chan, T.F., Golub, G.H., and LeVeque, R.J.
		/// 1983, American Statistician, vol. 37, pp. 242-247, referenced in
		/// Weisberg, S. "Applied Linear Regression". 2nd Ed. 1985.</p>
		/// 
		/// </summary>
		/// <param name="x"> independent variable value </param>
		/// <param name="y"> dependent variable value </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void addData(final double x,final double y)
		public virtual void addData(double x, double y)
		{
			if (n == 0)
			{
				xbar = x;
				ybar = y;
			}
			else
			{
				if (hasIntercept_Renamed)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fact1 = 1.0 + n;
					double fact1 = 1.0 + n;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fact2 = n / (1.0 + n);
					double fact2 = n / (1.0 + n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = x - xbar;
					double dx = x - xbar;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dy = y - ybar;
					double dy = y - ybar;
					sumXX += dx * dx * fact2;
					sumYY += dy * dy * fact2;
					sumXY += dx * dy * fact2;
					xbar += dx / fact1;
					ybar += dy / fact1;
				}
			}
			if (!hasIntercept_Renamed)
			{
				sumXX += x * x;
				sumYY += y * y;
				sumXY += x * y;
			}
			sumX += x;
			sumY += y;
			n++;
		}

		/// <summary>
		/// Appends data from another regression calculation to this one.
		/// 
		/// <p>The mean update formulae are based on a paper written by Philippe
		/// P&eacute;bay:
		/// <a
		/// href="http://prod.sandia.gov/techlib/access-control.cgi/2008/086212.pdf">
		/// Formulas for Robust, One-Pass Parallel Computation of Covariances and
		/// Arbitrary-Order Statistical Moments</a>, 2008, Technical Report
		/// SAND2008-6212, Sandia National Laboratories.</p>
		/// </summary>
		/// <param name="reg"> model to append data from
		/// @since 3.3 </param>
		public virtual void append(SimpleRegression reg)
		{
			if (n == 0)
			{
				xbar = reg.xbar;
				ybar = reg.ybar;
				sumXX = reg.sumXX;
				sumYY = reg.sumYY;
				sumXY = reg.sumXY;
			}
			else
			{
				if (hasIntercept_Renamed)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fact1 = reg.n / (double)(reg.n + n);
					double fact1 = reg.n / (double)(reg.n + n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fact2 = n * reg.n / (double)(reg.n + n);
					double fact2 = n * reg.n / (double)(reg.n + n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = reg.xbar - xbar;
					double dx = reg.xbar - xbar;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dy = reg.ybar - ybar;
					double dy = reg.ybar - ybar;
					sumXX += reg.sumXX + dx * dx * fact2;
					sumYY += reg.sumYY + dy * dy * fact2;
					sumXY += reg.sumXY + dx * dy * fact2;
					xbar += dx * fact1;
					ybar += dy * fact1;
				}
				else
				{
					sumXX += reg.sumXX;
					sumYY += reg.sumYY;
					sumXY += reg.sumXY;
				}
			}
			sumX += reg.sumX;
			sumY += reg.sumY;
			n += reg.n;
		}

		/// <summary>
		/// Removes the observation (x,y) from the regression data set.
		/// <p>
		/// Mirrors the addData method.  This method permits the use of
		/// SimpleRegression instances in streaming mode where the regression
		/// is applied to a sliding "window" of observations, however the caller is
		/// responsible for maintaining the set of observations in the window.</p>
		/// 
		/// The method has no effect if there are no points of data (i.e. n=0)
		/// </summary>
		/// <param name="x"> independent variable value </param>
		/// <param name="y"> dependent variable value </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void removeData(final double x,final double y)
		public virtual void removeData(double x, double y)
		{
			if (n > 0)
			{
				if (hasIntercept_Renamed)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fact1 = n - 1.0;
					double fact1 = n - 1.0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fact2 = n / (n - 1.0);
					double fact2 = n / (n - 1.0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = x - xbar;
					double dx = x - xbar;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dy = y - ybar;
					double dy = y - ybar;
					sumXX -= dx * dx * fact2;
					sumYY -= dy * dy * fact2;
					sumXY -= dx * dy * fact2;
					xbar -= dx / fact1;
					ybar -= dy / fact1;
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fact1 = n - 1.0;
					double fact1 = n - 1.0;
					sumXX -= x * x;
					sumYY -= y * y;
					sumXY -= x * y;
					xbar -= x / fact1;
					ybar -= y / fact1;
				}
				 sumX -= x;
				 sumY -= y;
				 n--;
			}
		}

		/// <summary>
		/// Adds the observations represented by the elements in
		/// <code>data</code>.
		/// <p>
		/// <code>(data[0][0],data[0][1])</code> will be the first observation, then
		/// <code>(data[1][0],data[1][1])</code>, etc.</p>
		/// <p>
		/// This method does not replace data that has already been added.  The
		/// observations represented by <code>data</code> are added to the existing
		/// dataset.</p>
		/// <p>
		/// To replace all data, use <code>clear()</code> before adding the new
		/// data.</p>
		/// </summary>
		/// <param name="data"> array of observations to be added </param>
		/// <exception cref="ModelSpecificationException"> if the length of {@code data[i]} is not
		/// greater than or equal to 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addData(final double[][] data) throws ModelSpecificationException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void addData(double[][] data)
		{
			for (int i = 0; i < data.Length; i++)
			{
				if (data[i].Length < 2)
				{
				   throw new ModelSpecificationException(LocalizedFormats.INVALID_REGRESSION_OBSERVATION, data[i].Length, 2);
				}
				addData(data[i][0], data[i][1]);
			}
		}

		/// <summary>
		/// Adds one observation to the regression model.
		/// </summary>
		/// <param name="x"> the independent variables which form the design matrix </param>
		/// <param name="y"> the dependent or response variable </param>
		/// <exception cref="ModelSpecificationException"> if the length of {@code x} does not equal
		/// the number of independent variables in the model </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addObservation(final double[] x,final double y) throws ModelSpecificationException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void addObservation(double[] x, double y)
		{
			if (x == null || x.Length == 0)
			{
				throw new ModelSpecificationException(LocalizedFormats.INVALID_REGRESSION_OBSERVATION,x != null?x.Length:0, 1);
			}
			addData(x[0], y);
		}

		/// <summary>
		/// Adds a series of observations to the regression model. The lengths of
		/// x and y must be the same and x must be rectangular.
		/// </summary>
		/// <param name="x"> a series of observations on the independent variables </param>
		/// <param name="y"> a series of observations on the dependent variable
		/// The length of x and y must be the same </param>
		/// <exception cref="ModelSpecificationException"> if {@code x} is not rectangular, does not match
		/// the length of {@code y} or does not contain sufficient data to estimate the model </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addObservations(final double[][] x,final double[] y) throws ModelSpecificationException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void addObservations(double[][] x, double[] y)
		{
			if ((x == null) || (y == null) || (x.Length != y.Length))
			{
				throw new ModelSpecificationException(LocalizedFormats.DIMENSIONS_MISMATCH_SIMPLE, (x == null) ? 0 : x.Length, (y == null) ? 0 : y.Length);
			}
			bool obsOk = true;
			for (int i = 0 ; i < x.Length; i++)
			{
				if (x[i] == null || x[i].Length == 0)
				{
					obsOk = false;
				}
			}
			if (!obsOk)
			{
				throw new ModelSpecificationException(LocalizedFormats.NOT_ENOUGH_DATA_FOR_NUMBER_OF_PREDICTORS, 0, 1);
			}
			for (int i = 0 ; i < x.Length ; i++)
			{
				addData(x[i][0], y[i]);
			}
		}

		/// <summary>
		/// Removes observations represented by the elements in <code>data</code>.
		/// <p>
		/// If the array is larger than the current n, only the first n elements are
		/// processed.  This method permits the use of SimpleRegression instances in
		/// streaming mode where the regression is applied to a sliding "window" of
		/// observations, however the caller is responsible for maintaining the set
		/// of observations in the window.</p>
		/// <p>
		/// To remove all data, use <code>clear()</code>.</p>
		/// </summary>
		/// <param name="data"> array of observations to be removed </param>
		public virtual void removeData(double[][] data)
		{
			for (int i = 0; i < data.Length && n > 0; i++)
			{
				removeData(data[i][0], data[i][1]);
			}
		}

		/// <summary>
		/// Clears all data from the model.
		/// </summary>
		public virtual void clear()
		{
			sumX = 0d;
			sumXX = 0d;
			sumY = 0d;
			sumYY = 0d;
			sumXY = 0d;
			n = 0;
		}

		/// <summary>
		/// Returns the number of observations that have been added to the model.
		/// </summary>
		/// <returns> n number of observations that have been added. </returns>
		public virtual long N
		{
			get
			{
				return n;
			}
		}

		/// <summary>
		/// Returns the "predicted" <code>y</code> value associated with the
		/// supplied <code>x</code> value,  based on the data that has been
		/// added to the model when this method is activated.
		/// <p>
		/// <code> predict(x) = intercept + slope * x </code></p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>At least two observations (with at least two different x values)
		/// must have been added before invoking this method. If this method is
		/// invoked before a model can be estimated, <code>Double,NaN</code> is
		/// returned.
		/// </li></ul></p>
		/// </summary>
		/// <param name="x"> input <code>x</code> value </param>
		/// <returns> predicted <code>y</code> value </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double predict(final double x)
		public virtual double predict(double x)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b1 = getSlope();
			double b1 = Slope;
			if (hasIntercept_Renamed)
			{
				return getIntercept(b1) + b1 * x;
			}
			return b1 * x;
		}

		/// <summary>
		/// Returns the intercept of the estimated regression line, if
		/// <seealso cref="#hasIntercept()"/> is true; otherwise 0.
		/// <p>
		/// The least squares estimate of the intercept is computed using the
		/// <a href="http://www.xycoon.com/estimation4.htm">normal equations</a>.
		/// The intercept is sometimes denoted b0.</p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>At least two observations (with at least two different x values)
		/// must have been added before invoking this method. If this method is
		/// invoked before a model can be estimated, <code>Double,NaN</code> is
		/// returned.
		/// </li></ul></p>
		/// </summary>
		/// <returns> the intercept of the regression line if the model includes an
		/// intercept; 0 otherwise </returns>
		/// <seealso cref= #SimpleRegression(boolean) </seealso>
		public virtual double Intercept
		{
			get
			{
				return hasIntercept_Renamed ? getIntercept(Slope) : 0.0;
			}
		}

		/// <summary>
		/// Returns true if the model includes an intercept term.
		/// </summary>
		/// <returns> true if the regression includes an intercept; false otherwise </returns>
		/// <seealso cref= #SimpleRegression(boolean) </seealso>
		public virtual bool hasIntercept()
		{
			return hasIntercept_Renamed;
		}

		/// <summary>
		/// Returns the slope of the estimated regression line.
		/// <p>
		/// The least squares estimate of the slope is computed using the
		/// <a href="http://www.xycoon.com/estimation4.htm">normal equations</a>.
		/// The slope is sometimes denoted b1.</p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>At least two observations (with at least two different x values)
		/// must have been added before invoking this method. If this method is
		/// invoked before a model can be estimated, <code>Double.NaN</code> is
		/// returned.
		/// </li></ul></p>
		/// </summary>
		/// <returns> the slope of the regression line </returns>
		public virtual double Slope
		{
			get
			{
				if (n < 2)
				{
					return double.NaN; //not enough data
				}
				if (FastMath.abs(sumXX) < 10 * double.MinValue)
				{
					return double.NaN; //not enough variation in x
				}
				return sumXY / sumXX;
			}
		}

		/// <summary>
		/// Returns the <a href="http://www.xycoon.com/SumOfSquares.htm">
		/// sum of squared errors</a> (SSE) associated with the regression
		/// model.
		/// <p>
		/// The sum is computed using the computational formula</p>
		/// <p>
		/// <code>SSE = SYY - (SXY * SXY / SXX)</code></p>
		/// <p>
		/// where <code>SYY</code> is the sum of the squared deviations of the y
		/// values about their mean, <code>SXX</code> is similarly defined and
		/// <code>SXY</code> is the sum of the products of x and y mean deviations.
		/// </p><p>
		/// The sums are accumulated using the updating algorithm referenced in
		/// <seealso cref="#addData"/>.</p>
		/// <p>
		/// The return value is constrained to be non-negative - i.e., if due to
		/// rounding errors the computational formula returns a negative result,
		/// 0 is returned.</p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>At least two observations (with at least two different x values)
		/// must have been added before invoking this method. If this method is
		/// invoked before a model can be estimated, <code>Double,NaN</code> is
		/// returned.
		/// </li></ul></p>
		/// </summary>
		/// <returns> sum of squared errors associated with the regression model </returns>
		public virtual double SumSquaredErrors
		{
			get
			{
				return FastMath.max(0d, sumYY - sumXY * sumXY / sumXX);
			}
		}

		/// <summary>
		/// Returns the sum of squared deviations of the y values about their mean.
		/// <p>
		/// This is defined as SSTO
		/// <a href="http://www.xycoon.com/SumOfSquares.htm">here</a>.</p>
		/// <p>
		/// If <code>n < 2</code>, this returns <code>Double.NaN</code>.</p>
		/// </summary>
		/// <returns> sum of squared deviations of y values </returns>
		public virtual double TotalSumSquares
		{
			get
			{
				if (n < 2)
				{
					return double.NaN;
				}
				return sumYY;
			}
		}

		/// <summary>
		/// Returns the sum of squared deviations of the x values about their mean.
		/// 
		/// If <code>n < 2</code>, this returns <code>Double.NaN</code>.</p>
		/// </summary>
		/// <returns> sum of squared deviations of x values </returns>
		public virtual double XSumSquares
		{
			get
			{
				if (n < 2)
				{
					return double.NaN;
				}
				return sumXX;
			}
		}

		/// <summary>
		/// Returns the sum of crossproducts, x<sub>i</sub>*y<sub>i</sub>.
		/// </summary>
		/// <returns> sum of cross products </returns>
		public virtual double SumOfCrossProducts
		{
			get
			{
				return sumXY;
			}
		}

		/// <summary>
		/// Returns the sum of squared deviations of the predicted y values about
		/// their mean (which equals the mean of y).
		/// <p>
		/// This is usually abbreviated SSR or SSM.  It is defined as SSM
		/// <a href="http://www.xycoon.com/SumOfSquares.htm">here</a></p>
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>At least two observations (with at least two different x values)
		/// must have been added before invoking this method. If this method is
		/// invoked before a model can be estimated, <code>Double.NaN</code> is
		/// returned.
		/// </li></ul></p>
		/// </summary>
		/// <returns> sum of squared deviations of predicted y values </returns>
		public virtual double RegressionSumSquares
		{
			get
			{
				return getRegressionSumSquares(Slope);
			}
		}

		/// <summary>
		/// Returns the sum of squared errors divided by the degrees of freedom,
		/// usually abbreviated MSE.
		/// <p>
		/// If there are fewer than <strong>three</strong> data pairs in the model,
		/// or if there is no variation in <code>x</code>, this returns
		/// <code>Double.NaN</code>.</p>
		/// </summary>
		/// <returns> sum of squared deviations of y values </returns>
		public virtual double MeanSquareError
		{
			get
			{
				if (n < 3)
				{
					return double.NaN;
				}
				return hasIntercept_Renamed ? (SumSquaredErrors / (n - 2)) : (SumSquaredErrors / (n - 1));
			}
		}

		/// <summary>
		/// Returns <a href="http://mathworld.wolfram.com/CorrelationCoefficient.html">
		/// Pearson's product moment correlation coefficient</a>,
		/// usually denoted r.
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>At least two observations (with at least two different x values)
		/// must have been added before invoking this method. If this method is
		/// invoked before a model can be estimated, <code>Double,NaN</code> is
		/// returned.
		/// </li></ul></p>
		/// </summary>
		/// <returns> Pearson's r </returns>
		public virtual double R
		{
			get
			{
				double b1 = Slope;
				double result = FastMath.sqrt(RSquare);
				if (b1 < 0)
				{
					result = -result;
				}
				return result;
			}
		}

		/// <summary>
		/// Returns the <a href="http://www.xycoon.com/coefficient1.htm">
		/// coefficient of determination</a>,
		/// usually denoted r-square.
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>At least two observations (with at least two different x values)
		/// must have been added before invoking this method. If this method is
		/// invoked before a model can be estimated, <code>Double,NaN</code> is
		/// returned.
		/// </li></ul></p>
		/// </summary>
		/// <returns> r-square </returns>
		public virtual double RSquare
		{
			get
			{
				double ssto = TotalSumSquares;
				return (ssto - SumSquaredErrors) / ssto;
			}
		}

		/// <summary>
		/// Returns the <a href="http://www.xycoon.com/standarderrorb0.htm">
		/// standard error of the intercept estimate</a>,
		/// usually denoted s(b0).
		/// <p>
		/// If there are fewer that <strong>three</strong> observations in the
		/// model, or if there is no variation in x, this returns
		/// <code>Double.NaN</code>.</p> Additionally, a <code>Double.NaN</code> is
		/// returned when the intercept is constrained to be zero
		/// </summary>
		/// <returns> standard error associated with intercept estimate </returns>
		public virtual double InterceptStdErr
		{
			get
			{
				if (!hasIntercept_Renamed)
				{
					return double.NaN;
				}
				return FastMath.sqrt(MeanSquareError * ((1d / n) + (xbar * xbar) / sumXX));
			}
		}

		/// <summary>
		/// Returns the <a href="http://www.xycoon.com/standerrorb(1).htm">standard
		/// error of the slope estimate</a>,
		/// usually denoted s(b1).
		/// <p>
		/// If there are fewer that <strong>three</strong> data pairs in the model,
		/// or if there is no variation in x, this returns <code>Double.NaN</code>.
		/// </p>
		/// </summary>
		/// <returns> standard error associated with slope estimate </returns>
		public virtual double SlopeStdErr
		{
			get
			{
				return FastMath.sqrt(MeanSquareError / sumXX);
			}
		}

		/// <summary>
		/// Returns the half-width of a 95% confidence interval for the slope
		/// estimate.
		/// <p>
		/// The 95% confidence interval is</p>
		/// <p>
		/// <code>(getSlope() - getSlopeConfidenceInterval(),
		/// getSlope() + getSlopeConfidenceInterval())</code></p>
		/// <p>
		/// If there are fewer that <strong>three</strong> observations in the
		/// model, or if there is no variation in x, this returns
		/// <code>Double.NaN</code>.</p>
		/// <p>
		/// <strong>Usage Note</strong>:<br>
		/// The validity of this statistic depends on the assumption that the
		/// observations included in the model are drawn from a
		/// <a href="http://mathworld.wolfram.com/BivariateNormalDistribution.html">
		/// Bivariate Normal Distribution</a>.</p>
		/// </summary>
		/// <returns> half-width of 95% confidence interval for the slope estimate </returns>
		/// <exception cref="OutOfRangeException"> if the confidence interval can not be computed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getSlopeConfidenceInterval() throws mathlib.exception.OutOfRangeException
		public virtual double SlopeConfidenceInterval
		{
			get
			{
				return getSlopeConfidenceInterval(0.05d);
			}
		}

		/// <summary>
		/// Returns the half-width of a (100-100*alpha)% confidence interval for
		/// the slope estimate.
		/// <p>
		/// The (100-100*alpha)% confidence interval is </p>
		/// <p>
		/// <code>(getSlope() - getSlopeConfidenceInterval(),
		/// getSlope() + getSlopeConfidenceInterval())</code></p>
		/// <p>
		/// To request, for example, a 99% confidence interval, use
		/// <code>alpha = .01</code></p>
		/// <p>
		/// <strong>Usage Note</strong>:<br>
		/// The validity of this statistic depends on the assumption that the
		/// observations included in the model are drawn from a
		/// <a href="http://mathworld.wolfram.com/BivariateNormalDistribution.html">
		/// Bivariate Normal Distribution</a>.</p>
		/// <p>
		/// <strong> Preconditions:</strong><ul>
		/// <li>If there are fewer that <strong>three</strong> observations in the
		/// model, or if there is no variation in x, this returns
		/// <code>Double.NaN</code>.
		/// </li>
		/// <li><code>(0 < alpha < 1)</code>; otherwise an
		/// <code>OutOfRangeException</code> is thrown.
		/// </li></ul></p>
		/// </summary>
		/// <param name="alpha"> the desired significance level </param>
		/// <returns> half-width of 95% confidence interval for the slope estimate </returns>
		/// <exception cref="OutOfRangeException"> if the confidence interval can not be computed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getSlopeConfidenceInterval(final double alpha) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double getSlopeConfidenceInterval(double alpha)
		{
			if (n < 3)
			{
				return double.NaN;
			}
			if (alpha >= 1 || alpha <= 0)
			{
				throw new OutOfRangeException(LocalizedFormats.SIGNIFICANCE_LEVEL, alpha, 0, 1);
			}
			// No advertised NotStrictlyPositiveException here - will return NaN above
			TDistribution distribution = new TDistribution(n - 2);
			return SlopeStdErr * distribution.inverseCumulativeProbability(1d - alpha / 2d);
		}

		/// <summary>
		/// Returns the significance level of the slope (equiv) correlation.
		/// <p>
		/// Specifically, the returned value is the smallest <code>alpha</code>
		/// such that the slope confidence interval with significance level
		/// equal to <code>alpha</code> does not include <code>0</code>.
		/// On regression output, this is often denoted <code>Prob(|t| > 0)</code>
		/// </p><p>
		/// <strong>Usage Note</strong>:<br>
		/// The validity of this statistic depends on the assumption that the
		/// observations included in the model are drawn from a
		/// <a href="http://mathworld.wolfram.com/BivariateNormalDistribution.html">
		/// Bivariate Normal Distribution</a>.</p>
		/// <p>
		/// If there are fewer that <strong>three</strong> observations in the
		/// model, or if there is no variation in x, this returns
		/// <code>Double.NaN</code>.</p>
		/// </summary>
		/// <returns> significance level for slope/correlation </returns>
		/// <exception cref="mathlib.exception.MaxCountExceededException">
		/// if the significance level can not be computed. </exception>
		public virtual double Significance
		{
			get
			{
				if (n < 3)
				{
					return double.NaN;
				}
				// No advertised NotStrictlyPositiveException here - will return NaN above
				TDistribution distribution = new TDistribution(n - 2);
				return 2d * (1.0 - distribution.cumulativeProbability(FastMath.abs(Slope) / SlopeStdErr));
			}
		}

		// ---------------------Private methods-----------------------------------

		/// <summary>
		/// Returns the intercept of the estimated regression line, given the slope.
		/// <p>
		/// Will return <code>NaN</code> if slope is <code>NaN</code>.</p>
		/// </summary>
		/// <param name="slope"> current slope </param>
		/// <returns> the intercept of the regression line </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double getIntercept(final double slope)
		private double getIntercept(double slope)
		{
		  if (hasIntercept_Renamed)
		  {
			return (sumY - slope * sumX) / n;
		  }
		  return 0.0;
		}

		/// <summary>
		/// Computes SSR from b1.
		/// </summary>
		/// <param name="slope"> regression slope estimate </param>
		/// <returns> sum of squared deviations of predicted y values </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double getRegressionSumSquares(final double slope)
		private double getRegressionSumSquares(double slope)
		{
			return slope * slope * sumXX;
		}

		/// <summary>
		/// Performs a regression on data present in buffers and outputs a RegressionResults object.
		/// 
		/// <p>If there are fewer than 3 observations in the model and {@code hasIntercept} is true
		/// a {@code NoDataException} is thrown.  If there is no intercept term, the model must
		/// contain at least 2 observations.</p>
		/// </summary>
		/// <returns> RegressionResults acts as a container of regression output </returns>
		/// <exception cref="ModelSpecificationException"> if the model is not correctly specified </exception>
		/// <exception cref="NoDataException"> if there is not sufficient data in the model to
		/// estimate the regression parameters </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RegressionResults regress() throws ModelSpecificationException, mathlib.exception.NoDataException
		public virtual RegressionResults regress()
		{
			if (hasIntercept_Renamed)
			{
			  if (n < 3)
			  {
				  throw new NoDataException(LocalizedFormats.NOT_ENOUGH_DATA_REGRESSION);
			  }
			  if (FastMath.abs(sumXX) > Precision.SAFE_MIN)
			  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] params = new double[]{ getIntercept(), getSlope() };
				  double[] @params = new double[]{Intercept, Slope};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double mse = getMeanSquareError();
				  double mse = MeanSquareError;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double _syy = sumYY + sumY * sumY / n;
				  double _syy = sumYY + sumY * sumY / n;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vcv = new double[]{ mse * (xbar *xbar /sumXX + 1.0 / n), -xbar*mse/sumXX, mse/sumXX };
				  double[] vcv = new double[]{mse * (xbar * xbar / sumXX + 1.0 / n), -xbar * mse / sumXX, mse / sumXX};
				  return new RegressionResults(@params, new double[][]{vcv}, true, n, 2, sumY, _syy, SumSquaredErrors,true,false);
			  }
			  else
			  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] params = new double[]{ sumY / n, Double.NaN };
				  double[] @params = new double[]{sumY / n, double.NaN};
				  //final double mse = getMeanSquareError();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vcv = new double[]{ ybar / (n - 1.0), Double.NaN, Double.NaN };
				  double[] vcv = new double[]{ybar / (n - 1.0), double.NaN, double.NaN};
				  return new RegressionResults(@params, new double[][]{vcv}, true, n, 1, sumY, sumYY, SumSquaredErrors,true,false);
			  }
			}
			else
			{
			  if (n < 2)
			  {
				  throw new NoDataException(LocalizedFormats.NOT_ENOUGH_DATA_REGRESSION);
			  }
			  if (!double.IsNaN(sumXX))
			  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vcv = new double[]{ getMeanSquareError() / sumXX };
			  double[] vcv = new double[]{MeanSquareError / sumXX};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] params = new double[]{ sumXY/sumXX };
			  double[] @params = new double[]{sumXY / sumXX};
			  return new RegressionResults(@params, new double[][]{vcv}, true, n, 1, sumY, sumYY, SumSquaredErrors,false,false);
			  }
			  else
			  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vcv = new double[]{Double.NaN };
			  double[] vcv = new double[]{double.NaN};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] params = new double[]{ Double.NaN };
			  double[] @params = new double[]{double.NaN};
			  return new RegressionResults(@params, new double[][]{vcv}, true, n, 1, double.NaN, double.NaN, double.NaN,false,false);
			  }
			}
		}

		/// <summary>
		/// Performs a regression on data present in buffers including only regressors
		/// indexed in variablesToInclude and outputs a RegressionResults object </summary>
		/// <param name="variablesToInclude"> an array of indices of regressors to include </param>
		/// <returns> RegressionResults acts as a container of regression output </returns>
		/// <exception cref="MathIllegalArgumentException"> if the variablesToInclude array is null or zero length </exception>
		/// <exception cref="OutOfRangeException"> if a requested variable is not present in model </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public RegressionResults regress(int[] variablesToInclude) throws mathlib.exception.MathIllegalArgumentException
		public virtual RegressionResults regress(int[] variablesToInclude)
		{
			if (variablesToInclude == null || variablesToInclude.Length == 0)
			{
			  throw new MathIllegalArgumentException(LocalizedFormats.ARRAY_ZERO_LENGTH_OR_NULL_NOT_ALLOWED);
			}
			if (variablesToInclude.Length > 2 || (variablesToInclude.Length > 1 && !hasIntercept_Renamed))
			{
				throw new ModelSpecificationException(LocalizedFormats.ARRAY_SIZE_EXCEEDS_MAX_VARIABLES, (variablesToInclude.Length > 1 && !hasIntercept_Renamed) ? 1 : 2);
			}

			if (hasIntercept_Renamed)
			{
				if (variablesToInclude.Length == 2)
				{
					if (variablesToInclude[0] == 1)
					{
						throw new ModelSpecificationException(LocalizedFormats.NOT_INCREASING_SEQUENCE);
					}
					else if (variablesToInclude[0] != 0)
					{
						throw new OutOfRangeException(variablesToInclude[0], 0,1);
					}
					if (variablesToInclude[1] != 1)
					{
						 throw new OutOfRangeException(variablesToInclude[0], 0,1);
					}
					return regress();
				}
				else
				{
					if (variablesToInclude[0] != 1 && variablesToInclude[0] != 0)
					{
						 throw new OutOfRangeException(variablesToInclude[0],0,1);
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double _mean = sumY * sumY / n;
					double _mean = sumY * sumY / n;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double _syy = sumYY + _mean;
					double _syy = sumYY + _mean;
					if (variablesToInclude[0] == 0)
					{
						//just the mean
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vcv = new double[]{ sumYY/(((n-1)*n)) };
						double[] vcv = new double[]{sumYY / (((n - 1) * n))};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] params = new double[]{ ybar };
						double[] @params = new double[]{ybar};
						return new RegressionResults(@params, new double[][]{vcv}, true, n, 1, sumY, _syy + _mean, sumYY,true,false);

					}
					else if (variablesToInclude[0] == 1)
					{
						//final double _syy = sumYY + sumY * sumY / ((double) n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double _sxx = sumXX + sumX * sumX / n;
						double _sxx = sumXX + sumX * sumX / n;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double _sxy = sumXY + sumX * sumY / n;
						double _sxy = sumXY + sumX * sumY / n;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double _sse = mathlib.util.FastMath.max(0d, _syy - _sxy * _sxy / _sxx);
						double _sse = FastMath.max(0d, _syy - _sxy * _sxy / _sxx);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double _mse = _sse/((n-1));
						double _mse = _sse / ((n - 1));
						if (!double.IsNaN(_sxx))
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vcv = new double[]{ _mse / _sxx };
							double[] vcv = new double[]{_mse / _sxx};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] params = new double[]{ _sxy/_sxx };
							double[] @params = new double[]{_sxy / _sxx};
							return new RegressionResults(@params, new double[][]{vcv}, true, n, 1, sumY, _syy, _sse,false,false);
						}
						else
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vcv = new double[]{Double.NaN };
							double[] vcv = new double[]{double.NaN};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] params = new double[]{ Double.NaN };
							double[] @params = new double[]{double.NaN};
							return new RegressionResults(@params, new double[][]{vcv}, true, n, 1, double.NaN, double.NaN, double.NaN,false,false);
						}
					}
				}
			}
			else
			{
				if (variablesToInclude[0] != 0)
				{
					throw new OutOfRangeException(variablesToInclude[0],0,0);
				}
				return regress();
			}

			return null;
		}
	}

}
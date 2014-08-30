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
namespace org.apache.commons.math3.optim.linear
{

	using MultivariateFunction = org.apache.commons.math3.analysis.MultivariateFunction;
	using MatrixUtils = org.apache.commons.math3.linear.MatrixUtils;
	using RealVector = org.apache.commons.math3.linear.RealVector;
	using ArrayRealVector = org.apache.commons.math3.linear.ArrayRealVector;

	/// <summary>
	/// An objective function for a linear optimization problem.
	/// <p>
	/// A linear objective function has one the form:
	/// <pre>
	/// c<sub>1</sub>x<sub>1</sub> + ... c<sub>n</sub>x<sub>n</sub> + d
	/// </pre>
	/// The c<sub>i</sub> and d are the coefficients of the equation,
	/// the x<sub>i</sub> are the coordinates of the current point.
	/// </p>
	/// 
	/// @version $Id: LinearObjectiveFunction.java 1435539 2013-01-19 13:27:24Z tn $
	/// @since 2.0
	/// </summary>
	[Serializable]
	public class LinearObjectiveFunction : MultivariateFunction, OptimizationData
	{
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -4531815507568396090L;
		/// <summary>
		/// Coefficients of the linear equation (c<sub>i</sub>). </summary>
		[NonSerialized]
		private readonly RealVector coefficients;
		/// <summary>
		/// Constant term of the linear equation. </summary>
		private readonly double constantTerm;

		/// <param name="coefficients"> Coefficients for the linear equation being optimized. </param>
		/// <param name="constantTerm"> Constant term of the linear equation. </param>
		public LinearObjectiveFunction(double[] coefficients, double constantTerm) : this(new ArrayRealVector(coefficients), constantTerm)
		{
		}

		/// <param name="coefficients"> Coefficients for the linear equation being optimized. </param>
		/// <param name="constantTerm"> Constant term of the linear equation. </param>
		public LinearObjectiveFunction(RealVector coefficients, double constantTerm)
		{
			this.coefficients = coefficients;
			this.constantTerm = constantTerm;
		}

		/// <summary>
		/// Gets the coefficients of the linear equation being optimized.
		/// </summary>
		/// <returns> coefficients of the linear equation being optimized. </returns>
		public virtual RealVector Coefficients
		{
			get
			{
				return coefficients;
			}
		}

		/// <summary>
		/// Gets the constant of the linear equation being optimized.
		/// </summary>
		/// <returns> constant of the linear equation being optimized. </returns>
		public virtual double ConstantTerm
		{
			get
			{
				return constantTerm;
			}
		}

		/// <summary>
		/// Computes the value of the linear equation at the current point.
		/// </summary>
		/// <param name="point"> Point at which linear equation must be evaluated. </param>
		/// <returns> the value of the linear equation at the current point. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double value(final double[] point)
		public virtual double value(double[] point)
		{
			return value(new ArrayRealVector(point, false));
		}

		/// <summary>
		/// Computes the value of the linear equation at the current point.
		/// </summary>
		/// <param name="point"> Point at which linear equation must be evaluated. </param>
		/// <returns> the value of the linear equation at the current point. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double value(final org.apache.commons.math3.linear.RealVector point)
		public virtual double value(RealVector point)
		{
			return coefficients.dotProduct(point) + constantTerm;
		}

		public override bool Equals(object other)
		{
			if (this == other)
			{
				return true;
			}
			if (other is LinearObjectiveFunction)
			{
				LinearObjectiveFunction rhs = (LinearObjectiveFunction) other;
			  return (constantTerm == rhs.constantTerm) && coefficients.Equals(rhs.coefficients);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return Convert.ToDouble(constantTerm).GetHashCode() ^ coefficients.GetHashCode();
		}

		/// <summary>
		/// Serialize the instance. </summary>
		/// <param name="oos"> stream where object should be written </param>
		/// <exception cref="IOException"> if object cannot be written to stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream oos) throws java.io.IOException
		private void writeObject(ObjectOutputStream oos)
		{
			oos.defaultWriteObject();
			MatrixUtils.serializeRealVector(coefficients, oos);
		}

		/// <summary>
		/// Deserialize the instance. </summary>
		/// <param name="ois"> stream from which the object should be read </param>
		/// <exception cref="ClassNotFoundException"> if a class in the stream cannot be found </exception>
		/// <exception cref="IOException"> if object cannot be read from the stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream ois) throws ClassNotFoundException, java.io.IOException
		private void readObject(ObjectInputStream ois)
		{
			ois.defaultReadObject();
			MatrixUtils.deserializeRealVector(this, "coefficients", ois);
		}
	}

}
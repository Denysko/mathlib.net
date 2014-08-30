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

	using MatrixUtils = org.apache.commons.math3.linear.MatrixUtils;
	using RealVector = org.apache.commons.math3.linear.RealVector;
	using ArrayRealVector = org.apache.commons.math3.linear.ArrayRealVector;

	/// <summary>
	/// A linear constraint for a linear optimization problem.
	/// <p>
	/// A linear constraint has one of the forms:
	/// <ul>
	///   <li>c<sub>1</sub>x<sub>1</sub> + ... c<sub>n</sub>x<sub>n</sub> = v</li>
	///   <li>c<sub>1</sub>x<sub>1</sub> + ... c<sub>n</sub>x<sub>n</sub> &lt;= v</li>
	///   <li>c<sub>1</sub>x<sub>1</sub> + ... c<sub>n</sub>x<sub>n</sub> >= v</li>
	///   <li>l<sub>1</sub>x<sub>1</sub> + ... l<sub>n</sub>x<sub>n</sub> + l<sub>cst</sub> =
	///       r<sub>1</sub>x<sub>1</sub> + ... r<sub>n</sub>x<sub>n</sub> + r<sub>cst</sub></li>
	///   <li>l<sub>1</sub>x<sub>1</sub> + ... l<sub>n</sub>x<sub>n</sub> + l<sub>cst</sub> &lt;=
	///       r<sub>1</sub>x<sub>1</sub> + ... r<sub>n</sub>x<sub>n</sub> + r<sub>cst</sub></li>
	///   <li>l<sub>1</sub>x<sub>1</sub> + ... l<sub>n</sub>x<sub>n</sub> + l<sub>cst</sub> >=
	///       r<sub>1</sub>x<sub>1</sub> + ... r<sub>n</sub>x<sub>n</sub> + r<sub>cst</sub></li>
	/// </ul>
	/// The c<sub>i</sub>, l<sub>i</sub> or r<sub>i</sub> are the coefficients of the constraints, the x<sub>i</sub>
	/// are the coordinates of the current point and v is the value of the constraint.
	/// </p>
	/// 
	/// @version $Id: LinearConstraint.java 1435539 2013-01-19 13:27:24Z tn $
	/// @since 2.0
	/// </summary>
	[Serializable]
	public class LinearConstraint
	{
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -764632794033034092L;
		/// <summary>
		/// Coefficients of the constraint (left hand side). </summary>
		[NonSerialized]
		private readonly RealVector coefficients;
		/// <summary>
		/// Relationship between left and right hand sides (=, &lt;=, >=). </summary>
		private readonly Relationship relationship;
		/// <summary>
		/// Value of the constraint (right hand side). </summary>
		private readonly double value;

		/// <summary>
		/// Build a constraint involving a single linear equation.
		/// <p>
		/// A linear constraint with a single linear equation has one of the forms:
		/// <ul>
		///   <li>c<sub>1</sub>x<sub>1</sub> + ... c<sub>n</sub>x<sub>n</sub> = v</li>
		///   <li>c<sub>1</sub>x<sub>1</sub> + ... c<sub>n</sub>x<sub>n</sub> &lt;= v</li>
		///   <li>c<sub>1</sub>x<sub>1</sub> + ... c<sub>n</sub>x<sub>n</sub> >= v</li>
		/// </ul>
		/// </p> </summary>
		/// <param name="coefficients"> The coefficients of the constraint (left hand side) </param>
		/// <param name="relationship"> The type of (in)equality used in the constraint </param>
		/// <param name="value"> The value of the constraint (right hand side) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LinearConstraint(final double[] coefficients, final Relationship relationship, final double value)
		public LinearConstraint(double[] coefficients, Relationship relationship, double value) : this(new ArrayRealVector(coefficients), relationship, value)
		{
		}

		/// <summary>
		/// Build a constraint involving a single linear equation.
		/// <p>
		/// A linear constraint with a single linear equation has one of the forms:
		/// <ul>
		///   <li>c<sub>1</sub>x<sub>1</sub> + ... c<sub>n</sub>x<sub>n</sub> = v</li>
		///   <li>c<sub>1</sub>x<sub>1</sub> + ... c<sub>n</sub>x<sub>n</sub> &lt;= v</li>
		///   <li>c<sub>1</sub>x<sub>1</sub> + ... c<sub>n</sub>x<sub>n</sub> >= v</li>
		/// </ul>
		/// </p> </summary>
		/// <param name="coefficients"> The coefficients of the constraint (left hand side) </param>
		/// <param name="relationship"> The type of (in)equality used in the constraint </param>
		/// <param name="value"> The value of the constraint (right hand side) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LinearConstraint(final org.apache.commons.math3.linear.RealVector coefficients, final Relationship relationship, final double value)
		public LinearConstraint(RealVector coefficients, Relationship relationship, double value)
		{
			this.coefficients = coefficients;
			this.relationship = relationship;
			this.value = value;
		}

		/// <summary>
		/// Build a constraint involving two linear equations.
		/// <p>
		/// A linear constraint with two linear equation has one of the forms:
		/// <ul>
		///   <li>l<sub>1</sub>x<sub>1</sub> + ... l<sub>n</sub>x<sub>n</sub> + l<sub>cst</sub> =
		///       r<sub>1</sub>x<sub>1</sub> + ... r<sub>n</sub>x<sub>n</sub> + r<sub>cst</sub></li>
		///   <li>l<sub>1</sub>x<sub>1</sub> + ... l<sub>n</sub>x<sub>n</sub> + l<sub>cst</sub> &lt;=
		///       r<sub>1</sub>x<sub>1</sub> + ... r<sub>n</sub>x<sub>n</sub> + r<sub>cst</sub></li>
		///   <li>l<sub>1</sub>x<sub>1</sub> + ... l<sub>n</sub>x<sub>n</sub> + l<sub>cst</sub> >=
		///       r<sub>1</sub>x<sub>1</sub> + ... r<sub>n</sub>x<sub>n</sub> + r<sub>cst</sub></li>
		/// </ul>
		/// </p> </summary>
		/// <param name="lhsCoefficients"> The coefficients of the linear expression on the left hand side of the constraint </param>
		/// <param name="lhsConstant"> The constant term of the linear expression on the left hand side of the constraint </param>
		/// <param name="relationship"> The type of (in)equality used in the constraint </param>
		/// <param name="rhsCoefficients"> The coefficients of the linear expression on the right hand side of the constraint </param>
		/// <param name="rhsConstant"> The constant term of the linear expression on the right hand side of the constraint </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LinearConstraint(final double[] lhsCoefficients, final double lhsConstant, final Relationship relationship, final double[] rhsCoefficients, final double rhsConstant)
		public LinearConstraint(double[] lhsCoefficients, double lhsConstant, Relationship relationship, double[] rhsCoefficients, double rhsConstant)
		{
			double[] sub = new double[lhsCoefficients.Length];
			for (int i = 0; i < sub.Length; ++i)
			{
				sub[i] = lhsCoefficients[i] - rhsCoefficients[i];
			}
			this.coefficients = new ArrayRealVector(sub, false);
			this.relationship = relationship;
			this.value = rhsConstant - lhsConstant;
		}

		/// <summary>
		/// Build a constraint involving two linear equations.
		/// <p>
		/// A linear constraint with two linear equation has one of the forms:
		/// <ul>
		///   <li>l<sub>1</sub>x<sub>1</sub> + ... l<sub>n</sub>x<sub>n</sub> + l<sub>cst</sub> =
		///       r<sub>1</sub>x<sub>1</sub> + ... r<sub>n</sub>x<sub>n</sub> + r<sub>cst</sub></li>
		///   <li>l<sub>1</sub>x<sub>1</sub> + ... l<sub>n</sub>x<sub>n</sub> + l<sub>cst</sub> &lt;=
		///       r<sub>1</sub>x<sub>1</sub> + ... r<sub>n</sub>x<sub>n</sub> + r<sub>cst</sub></li>
		///   <li>l<sub>1</sub>x<sub>1</sub> + ... l<sub>n</sub>x<sub>n</sub> + l<sub>cst</sub> >=
		///       r<sub>1</sub>x<sub>1</sub> + ... r<sub>n</sub>x<sub>n</sub> + r<sub>cst</sub></li>
		/// </ul>
		/// </p> </summary>
		/// <param name="lhsCoefficients"> The coefficients of the linear expression on the left hand side of the constraint </param>
		/// <param name="lhsConstant"> The constant term of the linear expression on the left hand side of the constraint </param>
		/// <param name="relationship"> The type of (in)equality used in the constraint </param>
		/// <param name="rhsCoefficients"> The coefficients of the linear expression on the right hand side of the constraint </param>
		/// <param name="rhsConstant"> The constant term of the linear expression on the right hand side of the constraint </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LinearConstraint(final org.apache.commons.math3.linear.RealVector lhsCoefficients, final double lhsConstant, final Relationship relationship, final org.apache.commons.math3.linear.RealVector rhsCoefficients, final double rhsConstant)
		public LinearConstraint(RealVector lhsCoefficients, double lhsConstant, Relationship relationship, RealVector rhsCoefficients, double rhsConstant)
		{
			this.coefficients = lhsCoefficients.subtract(rhsCoefficients);
			this.relationship = relationship;
			this.value = rhsConstant - lhsConstant;
		}

		/// <summary>
		/// Gets the coefficients of the constraint (left hand side).
		/// </summary>
		/// <returns> the coefficients of the constraint (left hand side). </returns>
		public virtual RealVector Coefficients
		{
			get
			{
				return coefficients;
			}
		}

		/// <summary>
		/// Gets the relationship between left and right hand sides.
		/// </summary>
		/// <returns> the relationship between left and right hand sides. </returns>
		public virtual Relationship Relationship
		{
			get
			{
				return relationship;
			}
		}

		/// <summary>
		/// Gets the value of the constraint (right hand side).
		/// </summary>
		/// <returns> the value of the constraint (right hand side). </returns>
		public virtual double Value
		{
			get
			{
				return value;
			}
		}

		public override bool Equals(object other)
		{
			if (this == other)
			{
				return true;
			}
			if (other is LinearConstraint)
			{
				LinearConstraint rhs = (LinearConstraint) other;
				return relationship == rhs.relationship && value == rhs.value && coefficients.Equals(rhs.coefficients);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return relationship.GetHashCode() ^ Convert.ToDouble(value).GetHashCode() ^ coefficients.GetHashCode();
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
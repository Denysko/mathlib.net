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
namespace mathlib.linear
{

	using MathUnsupportedOperationException = mathlib.exception.MathUnsupportedOperationException;

	/// <summary>
	/// A default concrete implementation of the abstract class
	/// <seealso cref="IterativeLinearSolverEvent"/>.
	/// 
	/// @version $Id: DefaultIterativeLinearSolverEvent.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class DefaultIterativeLinearSolverEvent : IterativeLinearSolverEvent
	{

		private const long serialVersionUID = 20120129L;

		/// <summary>
		/// The right-hand side vector. </summary>
		private readonly RealVector b;

		/// <summary>
		/// The current estimate of the residual. </summary>
		private readonly RealVector r;

		/// <summary>
		/// The current estimate of the norm of the residual. </summary>
		private readonly double rnorm;

		/// <summary>
		/// The current estimate of the solution. </summary>
		private readonly RealVector x;

		/// <summary>
		/// Creates a new instance of this class. This implementation does
		/// <em>not</em> deep copy the specified vectors {@code x}, {@code b},
		/// {@code r}. Therefore the user must make sure that these vectors are
		/// either unmodifiable views or deep copies of the same vectors actually
		/// used by the {@code source}. Failure to do so may compromise subsequent
		/// iterations of the {@code source}. If the residual vector {@code r} is
		/// {@code null}, then <seealso cref="#getResidual()"/> throws a
		/// <seealso cref="MathUnsupportedOperationException"/>, and
		/// <seealso cref="#providesResidual()"/> returns {@code false}.
		/// </summary>
		/// <param name="source"> the iterative solver which fired this event </param>
		/// <param name="iterations"> the number of iterations performed at the time
		/// {@code this} event is created </param>
		/// <param name="x"> the current estimate of the solution </param>
		/// <param name="b"> the right-hand side vector </param>
		/// <param name="r"> the current estimate of the residual (can be {@code null}) </param>
		/// <param name="rnorm"> the norm of the current estimate of the residual </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DefaultIterativeLinearSolverEvent(final Object source, final int iterations, final RealVector x, final RealVector b, final RealVector r, final double rnorm)
		public DefaultIterativeLinearSolverEvent(object source, int iterations, RealVector x, RealVector b, RealVector r, double rnorm) : base(source, iterations)
		{
			this.x = x;
			this.b = b;
			this.r = r;
			this.rnorm = rnorm;
		}

		/// <summary>
		/// Creates a new instance of this class. This implementation does
		/// <em>not</em> deep copy the specified vectors {@code x}, {@code b}.
		/// Therefore the user must make sure that these vectors are either
		/// unmodifiable views or deep copies of the same vectors actually used by
		/// the {@code source}. Failure to do so may compromise subsequent iterations
		/// of the {@code source}. Callling <seealso cref="#getResidual()"/> on instances
		/// returned by this constructor throws a
		/// <seealso cref="MathUnsupportedOperationException"/>, while
		/// <seealso cref="#providesResidual()"/> returns {@code false}.
		/// </summary>
		/// <param name="source"> the iterative solver which fired this event </param>
		/// <param name="iterations"> the number of iterations performed at the time
		/// {@code this} event is created </param>
		/// <param name="x"> the current estimate of the solution </param>
		/// <param name="b"> the right-hand side vector </param>
		/// <param name="rnorm"> the norm of the current estimate of the residual </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DefaultIterativeLinearSolverEvent(final Object source, final int iterations, final RealVector x, final RealVector b, final double rnorm)
		public DefaultIterativeLinearSolverEvent(object source, int iterations, RealVector x, RealVector b, double rnorm) : base(source, iterations)
		{
			this.x = x;
			this.b = b;
			this.r = null;
			this.rnorm = rnorm;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double NormOfResidual
		{
			get
			{
				return rnorm;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// This implementation throws an <seealso cref="MathUnsupportedOperationException"/>
		/// if no residual vector {@code r} was provided at construction time.
		/// </summary>
		public override RealVector Residual
		{
			get
			{
				if (r != null)
				{
					return r;
				}
				throw new MathUnsupportedOperationException();
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override RealVector RightHandSideVector
		{
			get
			{
				return b;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override RealVector Solution
		{
			get
			{
				return x;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// This implementation returns {@code true} if a non-{@code null} value was
		/// specified for the residual vector {@code r} at construction time.
		/// </summary>
		/// <returns> {@code true} if {@code r != null} </returns>
		public override bool providesResidual()
		{
			return r != null;
		}
	}

}
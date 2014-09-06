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
namespace mathlib.exception
{

    using Localizable = mathlib.exception.util.Localizable;
    using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
    using ExceptionContext = mathlib.exception.util.ExceptionContext;
    using ExceptionContextProvider = mathlib.exception.util.ExceptionContextProvider;

	/// <summary>
	/// Base class for all exceptions that signal a mismatch between the
	/// current state and the user's expectations.
	/// 
	/// @since 2.2
	/// @version $Id: MathIllegalStateException.java 1364378 2012-07-22 17:42:38Z tn $
	/// </summary>
	public class MathIllegalStateException : IllegalStateException, ExceptionContextProvider
	{
		/// <summary>
		/// Serializable version Id. </summary>
		private const long SerialVersionUID = -6024911025449780478L;
		/// <summary>
		/// Context. </summary>
		private readonly ExceptionContext context;

		/// <summary>
		/// Simple constructor.
		/// </summary>
		/// <param name="pattern"> Message pattern explaining the cause of the error. </param>
		/// <param name="args"> Arguments. </param>
		public MathIllegalStateException(Localizable pattern, params object[] args)
		{
			context = new ExceptionContext(this);
			context.addMessage(pattern, args);
		}

		/// <summary>
		/// Simple constructor.
		/// </summary>
		/// <param name="cause"> Root cause. </param>
		/// <param name="pattern"> Message pattern explaining the cause of the error. </param>
		/// <param name="args"> Arguments. </param>
		public MathIllegalStateException(Exception cause, Localizable pattern, params object[] args) : base(cause)
		{
			context = new ExceptionContext(this);
			context.addMessage(pattern, args);
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public MathIllegalStateException() : this(LocalizedFormats.ILLEGAL_STATE)
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual ExceptionContext Context
		{
			get
			{
				return context;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string Message
		{
			get
			{
				return context.Message;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string LocalizedMessage
		{
			get
			{
				return context.LocalizedMessage;
			}
		}
	}

}
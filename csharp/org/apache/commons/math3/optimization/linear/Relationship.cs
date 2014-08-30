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

namespace org.apache.commons.math3.optimization.linear
{

	/// <summary>
	/// Types of relationships between two cells in a Solver <seealso cref="LinearConstraint"/>.
	/// @version $Id: Relationship.java 1422230 2012-12-15 12:11:13Z erans $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 2.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public enum Relationship
	{

		/// <summary>
		/// Equality relationship. </summary>
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		EQ("="),

		/// <summary>
		/// Lesser than or equal relationship. </summary>
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		LEQ("<="),

		/// <summary>
		/// Greater than or equal relationship. </summary>
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
		GEQ(">=");

		/// <summary>
		/// Display string for the relationship. </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain fields in .NET:
//		private final String stringValue;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="stringValue"> display string for the relationship </param>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain methods in .NET:
//		private Relationship(String stringValue)
	//	{
	//		this.stringValue = stringValue;
	//	}


		/// <summary>
		/// Get the relationship obtained when multiplying all coefficients by -1. </summary>
		/// <returns> relationship obtained when multiplying all coefficients by -1 </returns>

	}
	public static partial class EnumExtensionMethods
	{
		public override static string ToString(this Relationship instance)
		{
			return stringValue;
		}
		public static Relationship oppositeRelationship(this Relationship instance)
		{
			switch (instance)
			{
			case LEQ :
				return GEQ;
			case GEQ :
				return LEQ;
			default :
				return EQ;
			}
		}
	}

}
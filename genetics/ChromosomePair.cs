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
namespace mathlib.genetics
{

	/// <summary>
	/// A pair of <seealso cref="Chromosome"/> objects.
	/// @since 2.0
	/// 
	/// @version $Id: ChromosomePair.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class ChromosomePair
	{
		/// <summary>
		/// the first chromosome in the pair. </summary>
		private readonly Chromosome first;

		/// <summary>
		/// the second chromosome in the pair. </summary>
		private readonly Chromosome second;

		/// <summary>
		/// Create a chromosome pair.
		/// </summary>
		/// <param name="c1"> the first chromosome. </param>
		/// <param name="c2"> the second chromosome. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ChromosomePair(final Chromosome c1, final Chromosome c2)
		public ChromosomePair(Chromosome c1, Chromosome c2) : base()
		{
			first = c1;
			second = c2;
		}

		/// <summary>
		/// Access the first chromosome.
		/// </summary>
		/// <returns> the first chromosome. </returns>
		public virtual Chromosome First
		{
			get
			{
				return first;
			}
		}

		/// <summary>
		/// Access the second chromosome.
		/// </summary>
		/// <returns> the second chromosome. </returns>
		public virtual Chromosome Second
		{
			get
			{
				return second;
			}
		}

		public override string ToString()
		{
			return string.Format("({0},{1})", First, Second);
		}
	}

}
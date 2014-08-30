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
namespace org.apache.commons.math3.genetics
{

	/// <summary>
	/// Individual in a population. Chromosomes are compared based on their fitness.
	/// <p>
	/// The chromosomes are IMMUTABLE, and so their fitness is also immutable and
	/// therefore it can be cached.
	/// 
	/// @since 2.0
	/// @version $Id: Chromosome.java 1549094 2013-12-08 18:23:44Z tn $
	/// </summary>
	public abstract class Chromosome : IComparable<Chromosome>, Fitness
	{
		public abstract double fitness();
		/// <summary>
		/// Value assigned when no fitness has been computed yet. </summary>
		private static readonly double NO_FITNESS = double.NegativeInfinity;

		/// <summary>
		/// Cached value of the fitness of this chromosome. </summary>
		private double fitness = NO_FITNESS;

		/// <summary>
		/// Access the fitness of this chromosome. The bigger the fitness, the better the chromosome.
		/// <p>
		/// Computation of fitness is usually very time-consuming task, therefore the fitness is cached.
		/// </summary>
		/// <returns> the fitness </returns>
		public virtual double Fitness
		{
			get
			{
				if (this.fitness == NO_FITNESS)
				{
					// no cache - compute the fitness
					this.fitness = fitness();
				}
				return this.fitness;
			}
		}

		/// <summary>
		/// Compares two chromosomes based on their fitness. The bigger the fitness, the better the chromosome.
		/// </summary>
		/// <param name="another"> another chromosome to compare
		/// @return
		/// <ul>
		///   <li>-1 if <code>another</code> is better than <code>this</code></li>
		///   <li>1 if <code>another</code> is worse than <code>this</code></li>
		///   <li>0 if the two chromosomes have the same fitness</li>
		/// </ul> </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public int compareTo(final Chromosome another)
		public virtual int CompareTo(Chromosome another)
		{
			return Fitness.CompareTo(another.Fitness);
		}

		/// <summary>
		/// Returns <code>true</code> iff <code>another</code> has the same representation and therefore the same fitness. By
		/// default, it returns false -- override it in your implementation if you need it.
		/// </summary>
		/// <param name="another"> chromosome to compare </param>
		/// <returns> true if <code>another</code> is equivalent to this chromosome </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected boolean isSame(final Chromosome another)
		protected internal virtual bool isSame(Chromosome another)
		{
			return false;
		}

		/// <summary>
		/// Searches the <code>population</code> for another chromosome with the same representation. If such chromosome is
		/// found, it is returned, if no such chromosome exists, returns <code>null</code>.
		/// </summary>
		/// <param name="population"> Population to search </param>
		/// <returns> Chromosome with the same representation, or <code>null</code> if no such chromosome exists. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected Chromosome findSameChromosome(final Population population)
		protected internal virtual Chromosome findSameChromosome(Population population)
		{
			foreach (Chromosome anotherChr in population)
			{
				if (this.isSame(anotherChr))
				{
					return anotherChr;
				}
			}
			return null;
		}

		/// <summary>
		/// Searches the population for a chromosome representing the same solution, and if it finds one,
		/// updates the fitness to its value.
		/// </summary>
		/// <param name="population"> Population to search </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void searchForFitnessUpdate(final Population population)
		public virtual void searchForFitnessUpdate(Population population)
		{
			Chromosome sameChromosome = findSameChromosome(population);
			if (sameChromosome != null)
			{
				fitness = sameChromosome.Fitness;
			}
		}

	}

}
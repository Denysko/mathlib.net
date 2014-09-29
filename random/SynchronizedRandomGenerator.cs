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
namespace mathlib.random
{

	/// <summary>
	/// Any <seealso cref="RandomGenerator"/> implementation can be thread-safe if it
	/// is used through an instance of this class.
	/// This is achieved by enclosing calls to the methods of the actual
	/// generator inside the overridden {@code synchronized} methods of this
	/// class.
	/// 
	/// @since 3.1
	/// @version $Id: SynchronizedRandomGenerator.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class SynchronizedRandomGenerator : RandomGenerator
	{
		/// <summary>
		/// Object to which all calls will be delegated. </summary>
		private readonly RandomGenerator wrapped;

		/// <summary>
		/// Creates a synchronized wrapper for the given {@code RandomGenerator}
		/// instance.
		/// </summary>
		/// <param name="rng"> Generator whose methods will be called through
		/// their corresponding overridden synchronized version.
		/// To ensure thread-safety, the wrapped generator <em>must</em>
		/// not be used directly. </param>
		public SynchronizedRandomGenerator(RandomGenerator rng)
		{
			wrapped = rng;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual int Seed
		{
			set
			{
				lock (this)
				{
					wrapped.Seed = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual int[] Seed
		{
			set
			{
				lock (this)
				{
					wrapped.Seed = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual long Seed
		{
			set
			{
				lock (this)
				{
					wrapped.Seed = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual void nextBytes(sbyte[] bytes)
		{
			lock (this)
			{
				wrapped.nextBytes(bytes);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual int nextInt()
		{
			lock (this)
			{
				return wrapped.Next();
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual int nextInt(int n)
		{
			lock (this)
			{
				return wrapped.Next(n);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual long nextLong()
		{
			lock (this)
			{
				return wrapped.nextLong();
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual bool nextBoolean()
		{
			lock (this)
			{
				return wrapped.nextBoolean();
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual float nextFloat()
		{
			lock (this)
			{
				return wrapped.nextFloat();
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual double nextDouble()
		{
			lock (this)
			{
				return wrapped.NextDouble();
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual double nextGaussian()
		{
			lock (this)
			{
				return wrapped.nextGaussian();
			}
		}
	}

}
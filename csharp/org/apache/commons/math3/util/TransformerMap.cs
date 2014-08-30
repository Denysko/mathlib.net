using System;
using System.Collections.Generic;

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
namespace org.apache.commons.math3.util
{


	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;

	/// <summary>
	/// This TansformerMap automates the transformation of mixed object types.
	/// It provides a means to set NumberTransformers that will be selected
	/// based on the Class of the object handed to the Maps
	/// <code>double transform(Object o)</code> method.
	/// @version $Id: TransformerMap.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class TransformerMap : NumberTransformer
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 4605318041528645258L;

		/// <summary>
		/// A default Number Transformer for Numbers and numeric Strings.
		/// </summary>
		private NumberTransformer defaultTransformer = null;

		/// <summary>
		/// The internal Map.
		/// </summary>
		private IDictionary<Type, NumberTransformer> map = null;

		/// <summary>
		/// Build a map containing only the default transformer.
		/// </summary>
		public TransformerMap()
		{
			map = new Dictionary<Type, NumberTransformer>();
			defaultTransformer = new DefaultTransformer();
		}

		/// <summary>
		/// Tests if a Class is present in the TransformerMap. </summary>
		/// <param name="key"> Class to check </param>
		/// <returns> true|false </returns>
		public virtual bool containsClass(Type key)
		{
			return map.ContainsKey(key);
		}

		/// <summary>
		/// Tests if a NumberTransformer is present in the TransformerMap. </summary>
		/// <param name="value"> NumberTransformer to check </param>
		/// <returns> true|false </returns>
		public virtual bool containsTransformer(NumberTransformer value)
		{
			return map.ContainsValue(value);
		}

		/// <summary>
		/// Returns the Transformer that is mapped to a class
		/// if mapping is not present, this returns null. </summary>
		/// <param name="key"> The Class of the object </param>
		/// <returns> the mapped NumberTransformer or null. </returns>
		public virtual NumberTransformer getTransformer(Type key)
		{
			return map[key];
		}

		/// <summary>
		/// Sets a Class to Transformer Mapping in the Map. If
		/// the Class is already present, this overwrites that
		/// mapping. </summary>
		/// <param name="key"> The Class </param>
		/// <param name="transformer"> The NumberTransformer </param>
		/// <returns> the replaced transformer if one is present </returns>
		public virtual NumberTransformer putTransformer(Type key, NumberTransformer transformer)
		{
			return map[key] = transformer;
		}

		/// <summary>
		/// Removes a Class to Transformer Mapping in the Map. </summary>
		/// <param name="key"> The Class </param>
		/// <returns> the removed transformer if one is present or
		/// null if none was present. </returns>
		public virtual NumberTransformer removeTransformer(Type key)
		{
			return map.Remove(key);
		}

		/// <summary>
		/// Clears all the Class to Transformer mappings.
		/// </summary>
		public virtual void clear()
		{
			map.Clear();
		}

		/// <summary>
		/// Returns the Set of Classes used as keys in the map. </summary>
		/// <returns> Set of Classes </returns>
		public virtual Set<Type> classes()
		{
			return map.Keys;
		}

		/// <summary>
		/// Returns the Set of NumberTransformers used as values
		/// in the map. </summary>
		/// <returns> Set of NumberTransformers </returns>
		public virtual ICollection<NumberTransformer> transformers()
		{
			return map.Values;
		}

		/// <summary>
		/// Attempts to transform the Object against the map of
		/// NumberTransformers. Otherwise it returns Double.NaN.
		/// </summary>
		/// <param name="o"> the Object to be transformed. </param>
		/// <returns> the double value of the Object. </returns>
		/// <exception cref="MathIllegalArgumentException"> if the Object can not be
		/// transformed into a Double. </exception>
		/// <seealso cref= org.apache.commons.math3.util.NumberTransformer#transform(java.lang.Object) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double transform(Object o) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		public virtual double transform(object o)
		{
			double value = double.NaN;

			if (o is Number || o is string)
			{
				value = defaultTransformer.transform(o);
			}
			else
			{
				NumberTransformer trans = getTransformer(o.GetType());
				if (trans != null)
				{
					value = trans.transform(o);
				}
			}

			return value;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool Equals(object other)
		{
			if (this == other)
			{
				return true;
			}
			if (other is TransformerMap)
			{
				TransformerMap rhs = (TransformerMap) other;
				if (!defaultTransformer.Equals(rhs.defaultTransformer))
				{
					return false;
				}
				if (map.Count != rhs.map.Count)
				{
					return false;
				}
				foreach (KeyValuePair<Type, NumberTransformer> entry in map)
				{
					if (!entry.Value.Equals(rhs.map[entry.Key]))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int GetHashCode()
		{
			int hash = defaultTransformer.GetHashCode();
			foreach (NumberTransformer t in map.Values)
			{
				hash = hash * 31 + t.GetHashCode();
			}
			return hash;
		}

	}

}
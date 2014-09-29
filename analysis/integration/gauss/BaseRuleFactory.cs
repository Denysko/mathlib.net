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
namespace mathlib.analysis.integration.gauss
{

    using mathlib.util;
    using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
    using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
    using LocalizedFormats = mathlib.exception.util.LocalizedFormats;

    /// <summary>
    /// Base class for rules that determines the integration nodes and their
    /// weights.
    /// Subclasses must implement the <seealso cref="#computeRule(int) computeRule"/> method.
    /// </summary>
    /// @param <T> Type of the number used to represent the points and weights of
    /// the quadrature rules.
    /// 
    /// @since 3.1
    /// @version $Id: BaseRuleFactory.java 1455194 2013-03-11 15:45:54Z luc $ </param>
    public abstract class BaseRuleFactory<T> where T : Number
    {
        /// <summary>
        /// List of points and weights, indexed by the order of the rule. </summary>
        private readonly IDictionary<int?, Pair<T[], T[]>> pointsAndWeights = new SortedDictionary<int?, Pair<T[], T[]>>();
        /// <summary>
        /// Cache for double-precision rules. </summary>
        private readonly IDictionary<int?, Pair<double[], double[]>> pointsAndWeightsDouble = new SortedDictionary<int?, Pair<double[], double[]>>();

        /// <summary>
        /// Gets a copy of the quadrature rule with the given number of integration
        /// points.
        /// </summary>
        /// <param name="numberOfPoints"> Number of integration points. </param>
        /// <returns> a copy of the integration rule. </returns>
        /// <exception cref="NotStrictlyPositiveException"> if {@code numberOfPoints < 1}. </exception>
        /// <exception cref="DimensionMismatchException"> if the elements of the rule pair do not
        /// have the same length. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public mathlib.util.Pair<double[] , double[]> getRule(int numberOfPoints) throws mathlib.exception.NotStrictlyPositiveException, mathlib.exception.DimensionMismatchException
        public virtual Pair<double[], double[]> getRule(int numberOfPoints)
        {

            if (numberOfPoints <= 0)
            {
                throw new NotStrictlyPositiveException(LocalizedFormats.NUMBER_OF_POINTS, numberOfPoints);
            }

            // Try to obtain the rule from the cache.
            Pair<double[], double[]> cached = pointsAndWeightsDouble[numberOfPoints];

            if (cached == null)
            {
                // Rule not computed yet.

                // Compute the rule.
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final mathlib.util.Pair<T[] , T[]> rule = getRuleInternal(numberOfPoints);
                Pair<T[], T[]> rule = getRuleInternal(numberOfPoints);
                cached = convertToDouble(rule);

                // Cache it.
                pointsAndWeightsDouble[numberOfPoints] = cached;
            }

            // Return a copy.
            return new Pair<double[], double[]>(cached.First.clone(), cached.Second.clone());
        }

        /// <summary>
        /// Gets a rule.
        /// Synchronization ensures that rules will be computed and added to the
        /// cache at most once.
        /// The returned rule is a reference into the cache.
        /// </summary>
        /// <param name="numberOfPoints"> Order of the rule to be retrieved. </param>
        /// <returns> the points and weights corresponding to the given order. </returns>
        /// <exception cref="DimensionMismatchException"> if the elements of the rule pair do not
        /// have the same length. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected synchronized mathlib.util.Pair<T[] , T[]> getRuleInternal(int numberOfPoints) throws mathlib.exception.DimensionMismatchException
        protected internal virtual Pair<T[], T[]> getRuleInternal(int numberOfPoints)
        {
            lock (this)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final mathlib.util.Pair<T[] , T[]> rule = pointsAndWeights.get(numberOfPoints);
                Pair<T[], T[]> rule = pointsAndWeights[numberOfPoints];
                if (rule == null)
                {
                    addRule(computeRule(numberOfPoints));
                    // The rule should be available now.
                    return getRuleInternal(numberOfPoints);
                }
                return rule;
            }
        }

        /// <summary>
        /// Stores a rule.
        /// </summary>
        /// <param name="rule"> Rule to be stored. </param>
        /// <exception cref="DimensionMismatchException"> if the elements of the pair do not
        /// have the same length. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected void addRule(mathlib.util.Pair<T[] , T[]> rule) throws mathlib.exception.DimensionMismatchException
        protected internal virtual void addRule(Pair<T[], T[]> rule)
        {
            if (rule.First.Length != rule.Second.Length)
            {
                throw new DimensionMismatchException(rule.First.Length, rule.Second.Length);
            }

            pointsAndWeights[rule.First.Length] = rule;
        }

        /// <summary>
        /// Computes the rule for the given order.
        /// </summary>
        /// <param name="numberOfPoints"> Order of the rule to be computed. </param>
        /// <returns> the computed rule. </returns>
        /// <exception cref="DimensionMismatchException"> if the elements of the pair do not
        /// have the same length. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected abstract mathlib.util.Pair<T[] , T[]> computeRule(int numberOfPoints) throws mathlib.exception.DimensionMismatchException;
        protected internal abstract Pair<T[], T[]> computeRule(int numberOfPoints);

        /// <summary>
        /// Converts the from the actual {@code Number} type to {@code double}
        /// </summary>
        /// @param <T> Type of the number used to represent the points and
        /// weights of the quadrature rules. </param>
        /// <param name="rule"> Points and weights. </param>
        /// <returns> points and weights as {@code double}s. </returns>
        private static Pair<double[], double[]> convertToDouble<T>(Pair<T[], T[]> rule) where T : Number
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final T[] pT = rule.getFirst();
            T[] pT = rule.First;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final T[] wT = rule.getSecond();
            T[] wT = rule.Second;

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int len = pT.length;
            int len = pT.Length;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double[] pD = new double[len];
            double[] pD = new double[len];
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double[] wD = new double[len];
            double[] wD = new double[len];

            for (int i = 0; i < len; i++)
            {
                pD[i] = (double)pT[i];
                wD[i] = (double)wT[i];
            }

            return new Pair<double[], double[]>(pD, wD);
        }
    }

}
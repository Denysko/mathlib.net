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
namespace mathlib.optim.linear
{
    /// <summary>
    /// Types of relationships between two cells in a Solver <seealso cref="LinearConstraint"/>.
    /// </summary>
    public static class Relationship
    {
        /// <summary>
        /// Display string for the relationship. 
        /// </summary>
        private static string stringValue;

        /// <summary>
        /// Simple constructor.
        /// </summary>
        /// <param name="stringValue"> Display string for the relationship. </param>
        Relationship(string Value)
        {
            stringValue = Value;
        }


        public override static string ToString()
        {
            return stringValue;
        }

        /// <summary>
        /// Gets the relationship obtained when multiplying all coefficients by -1.
        /// </summary>
        /// <returns> the opposite relationship. </returns>
        public static RelationshipEnum oppositeRelationship(RelationshipEnum instance)
        {
            switch (instance)
            {
                case EnumRelationship.LEQ:
                    return EnumRelationship.GEQ;
                case EnumRelationship.GEQ:
                    return EnumRelationship.LEQ;
                default:
                    return EnumRelationship.EQ;
            }
        }
    }
}
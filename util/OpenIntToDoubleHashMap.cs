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

namespace mathlib.util
{


    /// <summary>
    /// Open addressed map from int to double.
    /// <p>This class provides a dedicated map from integers to doubles with a
    /// much smaller memory overhead than standard <code>java.util.Map</code>.</p>
    /// <p>This class is not synchronized. The specialized iterators returned by
    /// <seealso cref="#iterator()"/> are fail-fast: they throw a
    /// <code>ConcurrentModificationException</code> when they detect the map has been
    /// modified during iteration.</p>
    /// @version $Id: OpenIntToDoubleHashMap.java 1421448 2012-12-13 19:45:57Z tn $
    /// @since 2.0
    /// </summary>
    [Serializable]
    public class OpenIntToDoubleHashMap
    {

        /// <summary>
        /// Status indicator for free table entries. </summary>
        protected internal const sbyte FREE = 0;

        /// <summary>
        /// Status indicator for full table entries. </summary>
        protected internal const sbyte FULL = 1;

        /// <summary>
        /// Status indicator for removed table entries. </summary>
        protected internal const sbyte REMOVED = 2;

        /// <summary>
        /// Serializable version identifier </summary>
        private const long serialVersionUID = -3646337053166149105L;

        /// <summary>
        /// Load factor for the map. </summary>
        private const float LOAD_FACTOR = 0.5f;

        /// <summary>
        /// Default starting size.
        /// <p>This must be a power of two for bit mask to work properly. </p>
        /// </summary>
        private const int DEFAULT_EXPECTED_SIZE = 16;

        /// <summary>
        /// Multiplier for size growth when map fills up.
        /// <p>This must be a power of two for bit mask to work properly. </p>
        /// </summary>
        private const int RESIZE_MULTIPLIER = 2;

        /// <summary>
        /// Number of bits to perturb the index when probing for collision resolution. </summary>
        private const int PERTURB_SHIFT = 5;

        /// <summary>
        /// Keys table. </summary>
        private int[] keys;

        /// <summary>
        /// Values table. </summary>
        private double[] values;

        /// <summary>
        /// States table. </summary>
        private sbyte[] states;

        /// <summary>
        /// Return value for missing entries. </summary>
        private readonly double missingEntries;

        /// <summary>
        /// Current size of the map. </summary>
        private int size_Renamed;

        /// <summary>
        /// Bit mask for hash values. </summary>
        private int mask;

        /// <summary>
        /// Modifications count. </summary>
        [NonSerialized]
        private int count;

        /// <summary>
        /// Build an empty map with default size and using NaN for missing entries.
        /// </summary>
        public OpenIntToDoubleHashMap()
            : this(DEFAULT_EXPECTED_SIZE, double.NaN)
        {
        }

        /// <summary>
        /// Build an empty map with default size </summary>
        /// <param name="missingEntries"> value to return when a missing entry is fetched </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public OpenIntToDoubleHashMap(final double missingEntries)
        public OpenIntToDoubleHashMap(double missingEntries)
            : this(DEFAULT_EXPECTED_SIZE, missingEntries)
        {
        }

        /// <summary>
        /// Build an empty map with specified size and using NaN for missing entries. </summary>
        /// <param name="expectedSize"> expected number of elements in the map </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public OpenIntToDoubleHashMap(final int expectedSize)
        public OpenIntToDoubleHashMap(int expectedSize)
            : this(expectedSize, double.NaN)
        {
        }

        /// <summary>
        /// Build an empty map with specified size. </summary>
        /// <param name="expectedSize"> expected number of elements in the map </param>
        /// <param name="missingEntries"> value to return when a missing entry is fetched </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public OpenIntToDoubleHashMap(final int expectedSize, final double missingEntries)
        public OpenIntToDoubleHashMap(int expectedSize, double missingEntries)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int capacity = computeCapacity(expectedSize);
            int capacity = computeCapacity(expectedSize);
            keys = new int[capacity];
            values = new double[capacity];
            states = new sbyte[capacity];
            this.missingEntries = missingEntries;
            mask = capacity - 1;
        }

        /// <summary>
        /// Copy constructor. </summary>
        /// <param name="source"> map to copy </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public OpenIntToDoubleHashMap(final OpenIntToDoubleHashMap source)
        public OpenIntToDoubleHashMap(OpenIntToDoubleHashMap source)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int length = source.keys.length;
            int length = source.keys.Length;
            keys = new int[length];
            Array.Copy(source.keys, 0, keys, 0, length);
            values = new double[length];
            Array.Copy(source.values, 0, values, 0, length);
            states = new sbyte[length];
            Array.Copy(source.states, 0, states, 0, length);
            missingEntries = source.missingEntries;
            size_Renamed = source.size_Renamed;
            mask = source.mask;
            count = source.count;
        }

        /// <summary>
        /// Compute the capacity needed for a given size. </summary>
        /// <param name="expectedSize"> expected size of the map </param>
        /// <returns> capacity to use for the specified size </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: private static int computeCapacity(final int expectedSize)
        private static int computeCapacity(int expectedSize)
        {
            if (expectedSize == 0)
            {
                return 1;
            }
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int capacity = (int) FastMath.ceil(expectedSize / LOAD_FACTOR);
            int capacity = (int)FastMath.ceil(expectedSize / LOAD_FACTOR);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int powerOfTwo = Integer.highestOneBit(capacity);
            int powerOfTwo = int.highestOneBit(capacity);
            if (powerOfTwo == capacity)
            {
                return capacity;
            }
            return nextPowerOfTwo(capacity);
        }

        /// <summary>
        /// Find the smallest power of two greater than the input value </summary>
        /// <param name="i"> input value </param>
        /// <returns> smallest power of two greater than the input value </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: private static int nextPowerOfTwo(final int i)
        private static int nextPowerOfTwo(int i)
        {
            return int.highestOneBit(i) << 1;
        }

        /// <summary>
        /// Get the stored value associated with the given key </summary>
        /// <param name="key"> key associated with the data </param>
        /// <returns> data associated with the key </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public double get(final int key)
        public virtual double get(int key)
        {

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int hash = hashOf(key);
            int hash = hashOf(key);
            int index = hash & mask;
            if (containsKey(key, index))
            {
                return values[index];
            }

            if (states[index] == FREE)
            {
                return missingEntries;
            }

            int j = index;
            for (int perturb = perturb(hash); states[index] != FREE; perturb >>= PERTURB_SHIFT)
            {
                j = probe(perturb, j);
                index = j & mask;
                if (containsKey(key, index))
                {
                    return values[index];
                }
            }

            return missingEntries;

        }

        /// <summary>
        /// Check if a value is associated with a key. </summary>
        /// <param name="key"> key to check </param>
        /// <returns> true if a value is associated with key </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public boolean containsKey(final int key)
        public virtual bool containsKey(int key)
        {

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int hash = hashOf(key);
            int hash = hashOf(key);
            int index = hash & mask;
            if (containsKey(key, index))
            {
                return true;
            }

            if (states[index] == FREE)
            {
                return false;
            }

            int j = index;
            for (int perturb = perturb(hash); states[index] != FREE; perturb >>= PERTURB_SHIFT)
            {
                j = probe(perturb, j);
                index = j & mask;
                if (containsKey(key, index))
                {
                    return true;
                }
            }

            return false;

        }

        /// <summary>
        /// Get an iterator over map elements.
        /// <p>The specialized iterators returned are fail-fast: they throw a
        /// <code>ConcurrentModificationException</code> when they detect the map
        /// has been modified during iteration.</p> </summary>
        /// <returns> iterator over the map elements </returns>
        public virtual Iterator iterator()
        {
            return new Iterator(this);
        }

        /// <summary>
        /// Perturb the hash for starting probing. </summary>
        /// <param name="hash"> initial hash </param>
        /// <returns> perturbed hash </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: private static int perturb(final int hash)
        private static int perturb(int hash)
        {
            return hash & 0x7fffffff;
        }

        /// <summary>
        /// Find the index at which a key should be inserted </summary>
        /// <param name="key"> key to lookup </param>
        /// <returns> index at which key should be inserted </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: private int findInsertionIndex(final int key)
        private int findInsertionIndex(int key)
        {
            return findInsertionIndex(keys, states, key, mask);
        }

        /// <summary>
        /// Find the index at which a key should be inserted </summary>
        /// <param name="keys"> keys table </param>
        /// <param name="states"> states table </param>
        /// <param name="key"> key to lookup </param>
        /// <param name="mask"> bit mask for hash values </param>
        /// <returns> index at which key should be inserted </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: private static int findInsertionIndex(final int[] keys, final byte[] states, final int key, final int mask)
        private static int findInsertionIndex(int[] keys, sbyte[] states, int key, int mask)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int hash = hashOf(key);
            int hash = hashOf(key);
            int index = hash & mask;
            if (states[index] == FREE)
            {
                return index;
            }
            else if (states[index] == FULL && keys[index] == key)
            {
                return changeIndexSign(index);
            }

            int perturb = perturb(hash);
            int j = index;
            if (states[index] == FULL)
            {
                while (true)
                {
                    j = probe(perturb, j);
                    index = j & mask;
                    perturb >>= PERTURB_SHIFT;

                    if (states[index] != FULL || keys[index] == key)
                    {
                        break;
                    }
                }
            }

            if (states[index] == FREE)
            {
                return index;
            }
            else if (states[index] == FULL)
            {
                // due to the loop exit condition,
                // if (states[index] == FULL) then keys[index] == key
                return changeIndexSign(index);
            }

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int firstRemoved = index;
            int firstRemoved = index;
            while (true)
            {
                j = probe(perturb, j);
                index = j & mask;

                if (states[index] == FREE)
                {
                    return firstRemoved;
                }
                else if (states[index] == FULL && keys[index] == key)
                {
                    return changeIndexSign(index);
                }

                perturb >>= PERTURB_SHIFT;

            }

        }

        /// <summary>
        /// Compute next probe for collision resolution </summary>
        /// <param name="perturb"> perturbed hash </param>
        /// <param name="j"> previous probe </param>
        /// <returns> next probe </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: private static int probe(final int perturb, final int j)
        private static int probe(int perturb, int j)
        {
            return (j << 2) + j + perturb + 1;
        }

        /// <summary>
        /// Change the index sign </summary>
        /// <param name="index"> initial index </param>
        /// <returns> changed index </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: private static int changeIndexSign(final int index)
        private static int changeIndexSign(int index)
        {
            return -index - 1;
        }

        /// <summary>
        /// Get the number of elements stored in the map. </summary>
        /// <returns> number of elements stored in the map </returns>
        public virtual int size()
        {
            return size_Renamed;
        }


        /// <summary>
        /// Remove the value associated with a key. </summary>
        /// <param name="key"> key to which the value is associated </param>
        /// <returns> removed value </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public double remove(final int key)
        public virtual double remove(int key)
        {

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int hash = hashOf(key);
            int hash = hashOf(key);
            int index = hash & mask;
            if (containsKey(key, index))
            {
                return doRemove(index);
            }

            if (states[index] == FREE)
            {
                return missingEntries;
            }

            int j = index;
            for (int perturb = perturb(hash); states[index] != FREE; perturb >>= PERTURB_SHIFT)
            {
                j = probe(perturb, j);
                index = j & mask;
                if (containsKey(key, index))
                {
                    return doRemove(index);
                }
            }

            return missingEntries;

        }

        /// <summary>
        /// Check if the tables contain an element associated with specified key
        /// at specified index. </summary>
        /// <param name="key"> key to check </param>
        /// <param name="index"> index to check </param>
        /// <returns> true if an element is associated with key at index </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: private boolean containsKey(final int key, final int index)
        private bool containsKey(int key, int index)
        {
            return (key != 0 || states[index] == FULL) && keys[index] == key;
        }

        /// <summary>
        /// Remove an element at specified index. </summary>
        /// <param name="index"> index of the element to remove </param>
        /// <returns> removed value </returns>
        private double doRemove(int index)
        {
            keys[index] = 0;
            states[index] = REMOVED;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double previous = values[index];
            double previous = values[index];
            values[index] = missingEntries;
            --size_Renamed;
            ++count;
            return previous;
        }

        /// <summary>
        /// Put a value associated with a key in the map. </summary>
        /// <param name="key"> key to which value is associated </param>
        /// <param name="value"> value to put in the map </param>
        /// <returns> previous value associated with the key </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public double put(final int key, final double value)
        public virtual double put(int key, double value)
        {
            int index = findInsertionIndex(key);
            double previous = missingEntries;
            bool newMapping = true;
            if (index < 0)
            {
                index = changeIndexSign(index);
                previous = values[index];
                newMapping = false;
            }
            keys[index] = key;
            states[index] = FULL;
            values[index] = value;
            if (newMapping)
            {
                ++size_Renamed;
                if (shouldGrowTable())
                {
                    growTable();
                }
                ++count;
            }
            return previous;

        }

        /// <summary>
        /// Grow the tables.
        /// </summary>
        private void growTable()
        {

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int oldLength = states.length;
            int oldLength = states.Length;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int[] oldKeys = keys;
            int[] oldKeys = keys;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double[] oldValues = values;
            double[] oldValues = values;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final byte[] oldStates = states;
            sbyte[] oldStates = states;

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int newLength = RESIZE_MULTIPLIER * oldLength;
            int newLength = RESIZE_MULTIPLIER * oldLength;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int[] newKeys = new int[newLength];
            int[] newKeys = new int[newLength];
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double[] newValues = new double[newLength];
            double[] newValues = new double[newLength];
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final byte[] newStates = new byte[newLength];
            sbyte[] newStates = new sbyte[newLength];
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int newMask = newLength - 1;
            int newMask = newLength - 1;
            for (int i = 0; i < oldLength; ++i)
            {
                if (oldStates[i] == FULL)
                {
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final int key = oldKeys[i];
                    int key = oldKeys[i];
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final int index = findInsertionIndex(newKeys, newStates, key, newMask);
                    int index = findInsertionIndex(newKeys, newStates, key, newMask);
                    newKeys[index] = key;
                    newValues[index] = oldValues[i];
                    newStates[index] = FULL;
                }
            }

            mask = newMask;
            keys = newKeys;
            values = newValues;
            states = newStates;

        }

        /// <summary>
        /// Check if tables should grow due to increased size. </summary>
        /// <returns> true if  tables should grow </returns>
        private bool shouldGrowTable()
        {
            return size_Renamed > (mask + 1) * LOAD_FACTOR;
        }

        /// <summary>
        /// Compute the hash value of a key </summary>
        /// <param name="key"> key to hash </param>
        /// <returns> hash value of the key </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: private static int hashOf(final int key)
        private static int hashOf(int key)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int h = key ^ ((key >>> 20) ^ (key >>> 12));
            int h = key ^ (((int)((uint)key >> 20)) ^ ((int)((uint)key >> 12)));
            return h ^ ((int)((uint)h >> 7)) ^ ((int)((uint)h >> 4));
        }


        /// <summary>
        /// Iterator class for the map. </summary>
        public class Iterator
        {
            private readonly OpenIntToDoubleHashMap outerInstance;


            /// <summary>
            /// Reference modification count. </summary>
            internal readonly int referenceCount;

            /// <summary>
            /// Index of current element. </summary>
            internal int current;

            /// <summary>
            /// Index of next element. </summary>
            internal int next;

            /// <summary>
            /// Simple constructor.
            /// </summary>
            internal Iterator(OpenIntToDoubleHashMap outerInstance)
            {
                this.outerInstance = outerInstance;

                // preserve the modification count of the map to detect concurrent modifications later
                referenceCount = outerInstance.count;

                // initialize current index
                next = -1;
                try
                {
                    advance();
                } // NOPMD
                catch (NoSuchElementException nsee)
                {
                    // ignored
                }

            }

            /// <summary>
            /// Check if there is a next element in the map. </summary>
            /// <returns> true if there is a next element </returns>
            public virtual bool hasNext()
            {
                return next >= 0;
            }

            /// <summary>
            /// Get the key of current entry. </summary>
            /// <returns> key of current entry </returns>
            /// <exception cref="ConcurrentModificationException"> if the map is modified during iteration </exception>
            /// <exception cref="NoSuchElementException"> if there is no element left in the map </exception>
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: public int key() throws java.util.ConcurrentModificationException, java.util.NoSuchElementException
            public virtual int key()
            {
                if (referenceCount != outerInstance.count)
                {
                    throw new ConcurrentModificationException();
                }
                if (current < 0)
                {
                    throw new NoSuchElementException();
                }
                return outerInstance.keys[current];
            }

            /// <summary>
            /// Get the value of current entry. </summary>
            /// <returns> value of current entry </returns>
            /// <exception cref="ConcurrentModificationException"> if the map is modified during iteration </exception>
            /// <exception cref="NoSuchElementException"> if there is no element left in the map </exception>
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: public double value() throws java.util.ConcurrentModificationException, java.util.NoSuchElementException
            public virtual double value()
            {
                if (referenceCount != outerInstance.count)
                {
                    throw new ConcurrentModificationException();
                }
                if (current < 0)
                {
                    throw new NoSuchElementException();
                }
                return outerInstance.values[current];
            }

            /// <summary>
            /// Advance iterator one step further. </summary>
            /// <exception cref="ConcurrentModificationException"> if the map is modified during iteration </exception>
            /// <exception cref="NoSuchElementException"> if there is no element left in the map </exception>
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: public void advance() throws java.util.ConcurrentModificationException, java.util.NoSuchElementException
            public virtual void advance()
            {

                if (referenceCount != outerInstance.count)
                {
                    throw new ConcurrentModificationException();
                }

                // advance on step
                current = next;

                // prepare next step
                try
                {
                    while (outerInstance.states[++next] != FULL) // NOPMD
                    {
                        // nothing to do
                    }
                }
                catch (System.IndexOutOfRangeException e)
                {
                    next = -2;
                    if (current < 0)
                    {
                        throw new NoSuchElementException();
                    }
                }

            }

        }

        /// <summary>
        /// Read a serialized object. </summary>
        /// <param name="stream"> input stream </param>
        /// <exception cref="IOException"> if object cannot be read </exception>
        /// <exception cref="ClassNotFoundException"> if the class corresponding
        /// to the serialized object cannot be found </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private void readObject(final java.io.ObjectInputStream stream) throws java.io.IOException, ClassNotFoundException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        private void readObject(ObjectInputStream stream)
        {
            stream.defaultReadObject();
            count = 0;
        }


    }

}
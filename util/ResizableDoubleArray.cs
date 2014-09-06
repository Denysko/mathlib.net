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


	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using MathIllegalStateException = mathlib.exception.MathIllegalStateException;
	using MathInternalError = mathlib.exception.MathInternalError;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;

	/// <summary>
	/// <p>
	/// A variable length <seealso cref="DoubleArray"/> implementation that automatically
	/// handles expanding and contracting its internal storage array as elements
	/// are added and removed.
	/// </p>
	/// <h3>Important note: Usage should not assume that this class is thread-safe
	/// even though some of the methods are {@code synchronized}.
	/// This qualifier will be dropped in the next major release (4.0).</h3>
	/// <p>
	/// The internal storage array starts with capacity determined by the
	/// {@code initialCapacity} property, which can be set by the constructor.
	/// The default initial capacity is 16.  Adding elements using
	/// <seealso cref="#addElement(double)"/> appends elements to the end of the array.
	/// When there are no open entries at the end of the internal storage array,
	/// the array is expanded.  The size of the expanded array depends on the
	/// {@code expansionMode} and {@code expansionFactor} properties.
	/// The {@code expansionMode} determines whether the size of the array is
	/// multiplied by the {@code expansionFactor}
	/// (<seealso cref="ExpansionMode#MULTIPLICATIVE"/>) or if the expansion is additive
	/// (<seealso cref="ExpansionMode#ADDITIVE"/> -- {@code expansionFactor} storage
	/// locations added).
	/// The default {@code expansionMode} is {@code MULTIPLICATIVE} and the default
	/// {@code expansionFactor} is 2.
	/// </p>
	/// <p>
	/// The <seealso cref="#addElementRolling(double)"/> method adds a new element to the end
	/// of the internal storage array and adjusts the "usable window" of the
	/// internal array forward by one position (effectively making what was the
	/// second element the first, and so on).  Repeated activations of this method
	/// (or activation of <seealso cref="#discardFrontElements(int)"/>) will effectively orphan
	/// the storage locations at the beginning of the internal storage array.  To
	/// reclaim this storage, each time one of these methods is activated, the size
	/// of the internal storage array is compared to the number of addressable
	/// elements (the {@code numElements} property) and if the difference
	/// is too large, the internal array is contracted to size
	/// {@code numElements + 1}.  The determination of when the internal
	/// storage array is "too large" depends on the {@code expansionMode} and
	/// {@code contractionFactor} properties.  If  the {@code expansionMode}
	/// is {@code MULTIPLICATIVE}, contraction is triggered when the
	/// ratio between storage array length and {@code numElements} exceeds
	/// {@code contractionFactor.}  If the {@code expansionMode}
	/// is {@code ADDITIVE}, the number of excess storage locations
	/// is compared to {@code contractionFactor}.
	/// </p>
	/// <p>
	/// To avoid cycles of expansions and contractions, the
	/// {@code expansionFactor} must not exceed the {@code contractionFactor}.
	/// Constructors and mutators for both of these properties enforce this
	/// requirement, throwing a {@code MathIllegalArgumentException} if it is
	/// violated.
	/// </p>
	/// @version $Id: ResizableDoubleArray.java 1591835 2014-05-02 09:04:01Z tn $
	/// </summary>
	[Serializable]
	public class ResizableDoubleArray : DoubleArray
	{
		/// <summary>
		/// Additive expansion mode. </summary>
		/// @deprecated As of 3.1. Please use <seealso cref="ExpansionMode#ADDITIVE"/> instead. 
		[Obsolete("As of 3.1. Please use <seealso cref="ExpansionMode#ADDITIVE"/> instead.")]
		public const int ADDITIVE_MODE = 1;
		/// <summary>
		/// Multiplicative expansion mode. </summary>
		/// @deprecated As of 3.1. Please use <seealso cref="ExpansionMode#MULTIPLICATIVE"/> instead. 
		[Obsolete("As of 3.1. Please use <seealso cref="ExpansionMode#MULTIPLICATIVE"/> instead.")]
		public const int MULTIPLICATIVE_MODE = 0;
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -3485529955529426875L;

		/// <summary>
		/// Default value for initial capacity. </summary>
		private const int DEFAULT_INITIAL_CAPACITY = 16;
		/// <summary>
		/// Default value for array size modifier. </summary>
		private const double DEFAULT_EXPANSION_FACTOR = 2.0;
		/// <summary>
		/// Default value for the difference between <seealso cref="#contractionCriterion"/>
		/// and <seealso cref="#expansionFactor"/>.
		/// </summary>
		private const double DEFAULT_CONTRACTION_DELTA = 0.5;

		/// <summary>
		/// The contraction criteria determines when the internal array will be
		/// contracted to fit the number of elements contained in the element
		///  array + 1.
		/// </summary>
		private double contractionCriterion = 2.5;

		/// <summary>
		/// The expansion factor of the array.  When the array needs to be expanded,
		/// the new array size will be
		/// {@code internalArray.length * expansionFactor}
		/// if {@code expansionMode} is set to MULTIPLICATIVE_MODE, or
		/// {@code internalArray.length + expansionFactor} if
		/// {@code expansionMode} is set to ADDITIVE_MODE.
		/// </summary>
		private double expansionFactor = 2.0;

		/// <summary>
		/// Determines whether array expansion by {@code expansionFactor}
		/// is additive or multiplicative.
		/// </summary>
		private ExpansionMode expansionMode = ExpansionMode.MULTIPLICATIVE;

		/// <summary>
		/// The internal storage array.
		/// </summary>
		private double[] internalArray;

		/// <summary>
		/// The number of addressable elements in the array.  Note that this
		/// has nothing to do with the length of the internal storage array.
		/// </summary>
		private int numElements = 0;

		/// <summary>
		/// The position of the first addressable element in the internal storage
		/// array.  The addressable elements in the array are
		/// {@code internalArray[startIndex],...,internalArray[startIndex + numElements - 1]}.
		/// </summary>
		private int startIndex = 0;

		/// <summary>
		/// Specification of expansion algorithm.
		/// @since 3.1
		/// </summary>
		public enum ExpansionMode
		{
			/// <summary>
			/// Multiplicative expansion mode. </summary>
			MULTIPLICATIVE,
			/// <summary>
			/// Additive expansion mode. </summary>
			ADDITIVE
		}

		/// <summary>
		/// Creates an instance with default properties.
		/// <ul>
		///  <li>{@code initialCapacity = 16}</li>
		///  <li>{@code expansionMode = MULTIPLICATIVE}</li>
		///  <li>{@code expansionFactor = 2.0}</li>
		///  <li>{@code contractionCriterion = 2.5}</li>
		/// </ul>
		/// </summary>
		public ResizableDoubleArray() : this(DEFAULT_INITIAL_CAPACITY)
		{
		}

		/// <summary>
		/// Creates an instance with the specified initial capacity.
		/// Other properties take default values:
		/// <ul>
		///  <li>{@code expansionMode = MULTIPLICATIVE}</li>
		///  <li>{@code expansionFactor = 2.0}</li>
		///  <li>{@code contractionCriterion = 2.5}</li>
		/// </ul> </summary>
		/// <param name="initialCapacity"> Initial size of the internal storage array. </param>
		/// <exception cref="MathIllegalArgumentException"> if {@code initialCapacity <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ResizableDoubleArray(int initialCapacity) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		public ResizableDoubleArray(int initialCapacity) : this(initialCapacity, DEFAULT_EXPANSION_FACTOR)
		{
		}

		/// <summary>
		/// Creates an instance from an existing {@code double[]} with the
		/// initial capacity and numElements corresponding to the size of
		/// the supplied {@code double[]} array.
		/// If the supplied array is null, a new empty array with the default
		/// initial capacity will be created.
		/// The input array is copied, not referenced.
		/// Other properties take default values:
		/// <ul>
		///  <li>{@code initialCapacity = 16}</li>
		///  <li>{@code expansionMode = MULTIPLICATIVE}</li>
		///  <li>{@code expansionFactor = 2.0}</li>
		///  <li>{@code contractionCriterion = 2.5}</li>
		/// </ul>
		/// </summary>
		/// <param name="initialArray"> initial array
		/// @since 2.2 </param>
		public ResizableDoubleArray(double[] initialArray) : this(DEFAULT_INITIAL_CAPACITY, DEFAULT_EXPANSION_FACTOR, DEFAULT_CONTRACTION_DELTA + DEFAULT_EXPANSION_FACTOR, ExpansionMode.MULTIPLICATIVE, initialArray)
		{
		}

		/// <summary>
		/// Creates an instance with the specified initial capacity
		/// and expansion factor.
		/// The remaining properties take default values:
		/// <ul>
		///  <li>{@code expansionMode = MULTIPLICATIVE}</li>
		///  <li>{@code contractionCriterion = 0.5 + expansionFactor}</li>
		/// </ul>
		/// <br/>
		/// Throws IllegalArgumentException if the following conditions are
		/// not met:
		/// <ul>
		///  <li>{@code initialCapacity > 0}</li>
		///  <li>{@code expansionFactor > 1}</li>
		/// </ul>
		/// </summary>
		/// <param name="initialCapacity"> Initial size of the internal storage array. </param>
		/// <param name="expansionFactor"> The array will be expanded based on this
		/// parameter. </param>
		/// <exception cref="MathIllegalArgumentException"> if parameters are not valid. </exception>
		/// @deprecated As of 3.1. Please use
		/// <seealso cref="#ResizableDoubleArray(int,double)"/> instead. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("As of 3.1. Please use") public ResizableDoubleArray(int initialCapacity, float expansionFactor) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		[Obsolete("As of 3.1. Please use")]
		public ResizableDoubleArray(int initialCapacity, float expansionFactor) : this(initialCapacity, (double) expansionFactor)
		{
		}

		/// <summary>
		/// Creates an instance with the specified initial capacity
		/// and expansion factor.
		/// The remaining properties take default values:
		/// <ul>
		///  <li>{@code expansionMode = MULTIPLICATIVE}</li>
		///  <li>{@code contractionCriterion = 0.5 + expansionFactor}</li>
		/// </ul>
		/// <br/>
		/// Throws IllegalArgumentException if the following conditions are
		/// not met:
		/// <ul>
		///  <li>{@code initialCapacity > 0}</li>
		///  <li>{@code expansionFactor > 1}</li>
		/// </ul>
		/// </summary>
		/// <param name="initialCapacity"> Initial size of the internal storage array. </param>
		/// <param name="expansionFactor"> The array will be expanded based on this
		/// parameter. </param>
		/// <exception cref="MathIllegalArgumentException"> if parameters are not valid.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ResizableDoubleArray(int initialCapacity, double expansionFactor) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		public ResizableDoubleArray(int initialCapacity, double expansionFactor) : this(initialCapacity, expansionFactor, DEFAULT_CONTRACTION_DELTA + expansionFactor)
		{
		}

		/// <summary>
		/// Creates an instance with the specified initialCapacity,
		/// expansionFactor, and contractionCriterion.
		/// The expansion mode will default to {@code MULTIPLICATIVE}.
		/// <br/>
		/// Throws IllegalArgumentException if the following conditions are
		/// not met:
		/// <ul>
		///  <li>{@code initialCapacity > 0}</li>
		///  <li>{@code expansionFactor > 1}</li>
		///  <li>{@code contractionCriterion >= expansionFactor}</li>
		/// </ul>
		/// </summary>
		/// <param name="initialCapacity"> Initial size of the internal storage array.. </param>
		/// <param name="expansionFactor"> The array will be expanded based on this
		/// parameter. </param>
		/// <param name="contractionCriteria"> Contraction criteria. </param>
		/// <exception cref="MathIllegalArgumentException"> if parameters are not valid. </exception>
		/// @deprecated As of 3.1. Please use
		/// <seealso cref="#ResizableDoubleArray(int,double,double)"/> instead. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("As of 3.1. Please use") public ResizableDoubleArray(int initialCapacity, float expansionFactor, float contractionCriteria) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		[Obsolete("As of 3.1. Please use")]
		public ResizableDoubleArray(int initialCapacity, float expansionFactor, float contractionCriteria) : this(initialCapacity, (double) expansionFactor, (double) contractionCriteria)
		{
		}

		/// <summary>
		/// Creates an instance with the specified initial capacity,
		/// expansion factor, and contraction criteria.
		/// The expansion mode will default to {@code MULTIPLICATIVE}.
		/// <br/>
		/// Throws IllegalArgumentException if the following conditions are
		/// not met:
		/// <ul>
		///  <li>{@code initialCapacity > 0}</li>
		///  <li>{@code expansionFactor > 1}</li>
		///  <li>{@code contractionCriterion >= expansionFactor}</li>
		/// </ul>
		/// </summary>
		/// <param name="initialCapacity"> Initial size of the internal storage array.. </param>
		/// <param name="expansionFactor"> The array will be expanded based on this
		/// parameter. </param>
		/// <param name="contractionCriterion"> Contraction criterion. </param>
		/// <exception cref="MathIllegalArgumentException"> if the parameters are not valid.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ResizableDoubleArray(int initialCapacity, double expansionFactor, double contractionCriterion) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		public ResizableDoubleArray(int initialCapacity, double expansionFactor, double contractionCriterion) : this(initialCapacity, expansionFactor, contractionCriterion, ExpansionMode.MULTIPLICATIVE, null)
		{
		}

		/// <summary>
		/// <p>
		/// Create a ResizableArray with the specified properties.</p>
		/// <p>
		/// Throws IllegalArgumentException if the following conditions are
		/// not met:
		/// <ul>
		/// <li><code>initialCapacity > 0</code></li>
		/// <li><code>expansionFactor > 1</code></li>
		/// <li><code>contractionFactor >= expansionFactor</code></li>
		/// <li><code>expansionMode in {MULTIPLICATIVE_MODE, ADDITIVE_MODE}</code>
		/// </li>
		/// </ul></p>
		/// </summary>
		/// <param name="initialCapacity"> the initial size of the internal storage array </param>
		/// <param name="expansionFactor"> the array will be expanded based on this
		///                        parameter </param>
		/// <param name="contractionCriteria"> the contraction Criteria </param>
		/// <param name="expansionMode">  the expansion mode </param>
		/// <exception cref="MathIllegalArgumentException"> if parameters are not valid </exception>
		/// @deprecated As of 3.1. Please use
		/// <seealso cref="#ResizableDoubleArray(int,double,double,ExpansionMode,double[])"/>
		/// instead. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("As of 3.1. Please use") public ResizableDoubleArray(int initialCapacity, float expansionFactor, float contractionCriteria, int expansionMode) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		[Obsolete("As of 3.1. Please use")]
		public ResizableDoubleArray(int initialCapacity, float expansionFactor, float contractionCriteria, int expansionMode) : this(initialCapacity, expansionFactor, contractionCriteria, expansionMode == ADDITIVE_MODE ? ExpansionMode.ADDITIVE : ExpansionMode.MULTIPLICATIVE, null)
		{
			// XXX Just ot retain the expected failure in a unit test.
			// With the new "enum", that test will become obsolete.
			ExpansionMode = expansionMode;
		}

		/// <summary>
		/// Creates an instance with the specified properties.
		/// <br/>
		/// Throws MathIllegalArgumentException if the following conditions are
		/// not met:
		/// <ul>
		///  <li>{@code initialCapacity > 0}</li>
		///  <li>{@code expansionFactor > 1}</li>
		///  <li>{@code contractionCriterion >= expansionFactor}</li>
		/// </ul>
		/// </summary>
		/// <param name="initialCapacity"> Initial size of the internal storage array. </param>
		/// <param name="expansionFactor"> The array will be expanded based on this
		/// parameter. </param>
		/// <param name="contractionCriterion"> Contraction criteria. </param>
		/// <param name="expansionMode"> Expansion mode. </param>
		/// <param name="data"> Initial contents of the array. </param>
		/// <exception cref="MathIllegalArgumentException"> if the parameters are not valid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ResizableDoubleArray(int initialCapacity, double expansionFactor, double contractionCriterion, ExpansionMode expansionMode, double... data) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		public ResizableDoubleArray(int initialCapacity, double expansionFactor, double contractionCriterion, ExpansionMode expansionMode, params double[] data)
		{
			if (initialCapacity <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.INITIAL_CAPACITY_NOT_POSITIVE, initialCapacity);
			}
			checkContractExpand(contractionCriterion, expansionFactor);

			this.expansionFactor = expansionFactor;
			this.contractionCriterion = contractionCriterion;
			this.expansionMode = expansionMode;
			internalArray = new double[initialCapacity];
			numElements = 0;
			startIndex = 0;

			if (data != null)
			{
				addElements(data);
			}
		}

		/// <summary>
		/// Copy constructor.  Creates a new ResizableDoubleArray that is a deep,
		/// fresh copy of the original. Needs to acquire synchronization lock
		/// on original.  Original may not be null; otherwise a <seealso cref="NullArgumentException"/>
		/// is thrown.
		/// </summary>
		/// <param name="original"> array to copy </param>
		/// <exception cref="NullArgumentException"> if original is null
		/// @since 2.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ResizableDoubleArray(ResizableDoubleArray original) throws org.apache.commons.math3.exception.NullArgumentException
		public ResizableDoubleArray(ResizableDoubleArray original)
		{
			MathUtils.checkNotNull(original);
			copy(original, this);
		}

		/// <summary>
		/// Adds an element to the end of this expandable array.
		/// </summary>
		/// <param name="value"> Value to be added to end of array. </param>
		public virtual void addElement(double value)
		{
			lock (this)
			{
				if (internalArray.Length <= startIndex + numElements)
				{
					expand();
				}
				internalArray[startIndex + numElements++] = value;
			}
		}

		/// <summary>
		/// Adds several element to the end of this expandable array.
		/// </summary>
		/// <param name="values"> Values to be added to end of array.
		/// @since 2.2 </param>
		public virtual void addElements(double[] values)
		{
			lock (this)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tempArray = new double[numElements + values.length + 1];
				double[] tempArray = new double[numElements + values.Length + 1];
				Array.Copy(internalArray, startIndex, tempArray, 0, numElements);
				Array.Copy(values, 0, tempArray, numElements, values.Length);
				internalArray = tempArray;
				startIndex = 0;
				numElements += values.Length;
			}
		}

		/// <summary>
		/// <p>
		/// Adds an element to the end of the array and removes the first
		/// element in the array.  Returns the discarded first element.
		/// The effect is similar to a push operation in a FIFO queue.
		/// </p>
		/// <p>
		/// Example: If the array contains the elements 1, 2, 3, 4 (in that order)
		/// and addElementRolling(5) is invoked, the result is an array containing
		/// the entries 2, 3, 4, 5 and the value returned is 1.
		/// </p>
		/// </summary>
		/// <param name="value"> Value to be added to the array. </param>
		/// <returns> the value which has been discarded or "pushed" out of the array
		/// by this rolling insert. </returns>
		public virtual double addElementRolling(double value)
		{
			lock (this)
			{
				double discarded = internalArray[startIndex];
        
				if ((startIndex + (numElements + 1)) > internalArray.Length)
				{
					expand();
				}
				// Increment the start index
				startIndex += 1;
        
				// Add the new value
				internalArray[startIndex + (numElements - 1)] = value;
        
				// Check the contraction criterion.
				if (shouldContract())
				{
					contract();
				}
				return discarded;
			}
		}

		/// <summary>
		/// Substitutes <code>value</code> for the most recently added value.
		/// Returns the value that has been replaced. If the array is empty (i.e.
		/// if <seealso cref="#numElements"/> is zero), an IllegalStateException is thrown.
		/// </summary>
		/// <param name="value"> New value to substitute for the most recently added value </param>
		/// <returns> the value that has been replaced in the array. </returns>
		/// <exception cref="MathIllegalStateException"> if the array is empty
		/// @since 2.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized double substituteMostRecentElement(double value) throws org.apache.commons.math3.exception.MathIllegalStateException
		public virtual double substituteMostRecentElement(double value)
		{
			lock (this)
			{
				if (numElements < 1)
				{
					throw new MathIllegalStateException(LocalizedFormats.CANNOT_SUBSTITUTE_ELEMENT_FROM_EMPTY_ARRAY);
				}
        
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int substIndex = startIndex + (numElements - 1);
				int substIndex = startIndex + (numElements - 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double discarded = internalArray[substIndex];
				double discarded = internalArray[substIndex];
        
				internalArray[substIndex] = value;
        
				return discarded;
			}
		}

		/// <summary>
		/// Checks the expansion factor and the contraction criterion and throws an
		/// IllegalArgumentException if the contractionCriteria is less than the
		/// expansionCriteria
		/// </summary>
		/// <param name="expansion"> factor to be checked </param>
		/// <param name="contraction"> criteria to be checked </param>
		/// <exception cref="MathIllegalArgumentException"> if the contractionCriteria is less than
		/// the expansionCriteria. </exception>
		/// @deprecated As of 3.1. Please use
		/// <seealso cref="#checkContractExpand(double,double)"/> instead. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("As of 3.1. Please use") protected void checkContractExpand(float contraction, float expansion) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		[Obsolete("As of 3.1. Please use")]
		protected internal virtual void checkContractExpand(float contraction, float expansion)
		{
			checkContractExpand((double) contraction, (double) expansion);
		}

		/// <summary>
		/// Checks the expansion factor and the contraction criterion and raises
		/// an exception if the contraction criterion is smaller than the
		/// expansion criterion.
		/// </summary>
		/// <param name="contraction"> Criterion to be checked. </param>
		/// <param name="expansion"> Factor to be checked. </param>
		/// <exception cref="NumberIsTooSmallException"> if {@code contraction < expansion}. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code contraction <= 1}. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code expansion <= 1 }.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkContractExpand(double contraction, double expansion) throws org.apache.commons.math3.exception.NumberIsTooSmallException
		protected internal virtual void checkContractExpand(double contraction, double expansion)
		{
			if (contraction < expansion)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.exception.NumberIsTooSmallException e = new org.apache.commons.math3.exception.NumberIsTooSmallException(contraction, 1, true);
				NumberIsTooSmallException e = new NumberIsTooSmallException(contraction, 1, true);
				e.Context.addMessage(LocalizedFormats.CONTRACTION_CRITERIA_SMALLER_THAN_EXPANSION_FACTOR, contraction, expansion);
				throw e;
			}

			if (contraction <= 1)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.exception.NumberIsTooSmallException e = new org.apache.commons.math3.exception.NumberIsTooSmallException(contraction, 1, false);
				NumberIsTooSmallException e = new NumberIsTooSmallException(contraction, 1, false);
				e.Context.addMessage(LocalizedFormats.CONTRACTION_CRITERIA_SMALLER_THAN_ONE, contraction);
				throw e;
			}

			if (expansion <= 1)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.exception.NumberIsTooSmallException e = new org.apache.commons.math3.exception.NumberIsTooSmallException(contraction, 1, false);
				NumberIsTooSmallException e = new NumberIsTooSmallException(contraction, 1, false);
				e.Context.addMessage(LocalizedFormats.EXPANSION_FACTOR_SMALLER_THAN_ONE, expansion);
				throw e;
			}
		}

		/// <summary>
		/// Clear the array contents, resetting the number of elements to zero.
		/// </summary>
		public virtual void clear()
		{
			lock (this)
			{
				numElements = 0;
				startIndex = 0;
			}
		}

		/// <summary>
		/// Contracts the storage array to the (size of the element set) + 1 - to
		/// avoid a zero length array. This function also resets the startIndex to
		/// zero.
		/// </summary>
		public virtual void contract()
		{
			lock (this)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tempArray = new double[numElements + 1];
				double[] tempArray = new double[numElements + 1];
        
				// Copy and swap - copy only the element array from the src array.
				Array.Copy(internalArray, startIndex, tempArray, 0, numElements);
				internalArray = tempArray;
        
				// Reset the start index to zero
				startIndex = 0;
			}
		}

		/// <summary>
		/// Discards the <code>i</code> initial elements of the array.  For example,
		/// if the array contains the elements 1,2,3,4, invoking
		/// <code>discardFrontElements(2)</code> will cause the first two elements
		/// to be discarded, leaving 3,4 in the array.  Throws illegalArgumentException
		/// if i exceeds numElements.
		/// </summary>
		/// <param name="i">  the number of elements to discard from the front of the array </param>
		/// <exception cref="MathIllegalArgumentException"> if i is greater than numElements.
		/// @since 2.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void discardFrontElements(int i) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		public virtual void discardFrontElements(int i)
		{
			lock (this)
			{
				discardExtremeElements(i,true);
			}
		}

		/// <summary>
		/// Discards the <code>i</code> last elements of the array.  For example,
		/// if the array contains the elements 1,2,3,4, invoking
		/// <code>discardMostRecentElements(2)</code> will cause the last two elements
		/// to be discarded, leaving 1,2 in the array.  Throws illegalArgumentException
		/// if i exceeds numElements.
		/// </summary>
		/// <param name="i">  the number of elements to discard from the end of the array </param>
		/// <exception cref="MathIllegalArgumentException"> if i is greater than numElements.
		/// @since 2.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void discardMostRecentElements(int i) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		public virtual void discardMostRecentElements(int i)
		{
			lock (this)
			{
				discardExtremeElements(i,false);
			}
		}

		/// <summary>
		/// Discards the <code>i</code> first or last elements of the array,
		/// depending on the value of <code>front</code>.
		/// For example, if the array contains the elements 1,2,3,4, invoking
		/// <code>discardExtremeElements(2,false)</code> will cause the last two elements
		/// to be discarded, leaving 1,2 in the array.
		/// For example, if the array contains the elements 1,2,3,4, invoking
		/// <code>discardExtremeElements(2,true)</code> will cause the first two elements
		/// to be discarded, leaving 3,4 in the array.
		/// Throws illegalArgumentException
		/// if i exceeds numElements.
		/// </summary>
		/// <param name="i">  the number of elements to discard from the front/end of the array </param>
		/// <param name="front"> true if elements are to be discarded from the front
		/// of the array, false if elements are to be discarded from the end
		/// of the array </param>
		/// <exception cref="MathIllegalArgumentException"> if i is greater than numElements.
		/// @since 2.0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private synchronized void discardExtremeElements(int i, boolean front) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		private void discardExtremeElements(int i, bool front)
		{
			lock (this)
			{
				if (i > numElements)
				{
					throw new MathIllegalArgumentException(LocalizedFormats.TOO_MANY_ELEMENTS_TO_DISCARD_FROM_ARRAY, i, numElements);
				}
			   else if (i < 0)
			   {
				   throw new MathIllegalArgumentException(LocalizedFormats.CANNOT_DISCARD_NEGATIVE_NUMBER_OF_ELEMENTS, i);
			   }
				else
				{
					// "Subtract" this number of discarded from numElements
					numElements -= i;
					if (front)
					{
						startIndex += i;
					}
				}
				if (shouldContract())
				{
					contract();
				}
			}
		}

		/// <summary>
		/// Expands the internal storage array using the expansion factor.
		/// <p>
		/// if <code>expansionMode</code> is set to MULTIPLICATIVE_MODE,
		/// the new array size will be <code>internalArray.length * expansionFactor.</code>
		/// If <code>expansionMode</code> is set to ADDITIVE_MODE,  the length
		/// after expansion will be <code>internalArray.length + expansionFactor</code>
		/// </p>
		/// </summary>
		protected internal virtual void expand()
		{
			lock (this)
			{
				// notice the use of FastMath.ceil(), this guarantees that we will always
				// have an array of at least currentSize + 1.   Assume that the
				// current initial capacity is 1 and the expansion factor
				// is 1.000000000000000001.  The newly calculated size will be
				// rounded up to 2 after the multiplication is performed.
				int newSize = 0;
				if (expansionMode == ExpansionMode.MULTIPLICATIVE)
				{
					newSize = (int) FastMath.ceil(internalArray.Length * expansionFactor);
				}
				else
				{
					newSize = (int)(internalArray.Length + FastMath.round(expansionFactor));
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tempArray = new double[newSize];
				double[] tempArray = new double[newSize];
        
				// Copy and swap
				Array.Copy(internalArray, 0, tempArray, 0, internalArray.Length);
				internalArray = tempArray;
			}
		}

		/// <summary>
		/// Expands the internal storage array to the specified size.
		/// </summary>
		/// <param name="size"> Size of the new internal storage array. </param>
		private void expandTo(int size)
		{
			lock (this)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tempArray = new double[size];
				double[] tempArray = new double[size];
				// Copy and swap
				Array.Copy(internalArray, 0, tempArray, 0, internalArray.Length);
				internalArray = tempArray;
			}
		}

		/// <summary>
		/// The contraction criteria defines when the internal array will contract
		/// to store only the number of elements in the element array.
		/// If  the <code>expansionMode</code> is <code>MULTIPLICATIVE_MODE</code>,
		/// contraction is triggered when the ratio between storage array length
		/// and <code>numElements</code> exceeds <code>contractionFactor</code>.
		/// If the <code>expansionMode</code> is <code>ADDITIVE_MODE</code>, the
		/// number of excess storage locations is compared to
		/// <code>contractionFactor.</code>
		/// </summary>
		/// <returns> the contraction criteria used to reclaim memory. </returns>
		/// @deprecated As of 3.1. Please use <seealso cref="#getContractionCriterion()"/>
		/// instead. 
		[Obsolete("As of 3.1. Please use <seealso cref="#getContractionCriterion()"/>")]
		public virtual float ContractionCriteria
		{
			get
			{
				return (float) ContractionCriterion;
			}
			set
			{
				checkContractExpand(value, ExpansionFactor);
				lock (this)
				{
					this.contractionCriterion = value;
				}
			}
		}

		/// <summary>
		/// The contraction criterion defines when the internal array will contract
		/// to store only the number of elements in the element array.
		/// If  the <code>expansionMode</code> is <code>MULTIPLICATIVE_MODE</code>,
		/// contraction is triggered when the ratio between storage array length
		/// and <code>numElements</code> exceeds <code>contractionFactor</code>.
		/// If the <code>expansionMode</code> is <code>ADDITIVE_MODE</code>, the
		/// number of excess storage locations is compared to
		/// <code>contractionFactor.</code>
		/// </summary>
		/// <returns> the contraction criterion used to reclaim memory.
		/// @since 3.1 </returns>
		public virtual double ContractionCriterion
		{
			get
			{
				return contractionCriterion;
			}
		}

		/// <summary>
		/// Returns the element at the specified index
		/// </summary>
		/// <param name="index"> index to fetch a value from </param>
		/// <returns> value stored at the specified index </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code> is less than
		/// zero or is greater than <code>getNumElements() - 1</code>. </exception>
		public virtual double getElement(int index)
		{
			lock (this)
			{
				if (index >= numElements)
				{
					throw new System.IndexOutOfRangeException(index);
				}
				else if (index >= 0)
				{
					return internalArray[startIndex + index];
				}
				else
				{
					throw new System.IndexOutOfRangeException(index);
				}
			}
		}

		 /// <summary>
		 /// Returns a double array containing the elements of this
		 /// <code>ResizableArray</code>.  This method returns a copy, not a
		 /// reference to the underlying array, so that changes made to the returned
		 ///  array have no effect on this <code>ResizableArray.</code> </summary>
		 /// <returns> the double array. </returns>
		public virtual double[] Elements
		{
			get
			{
				lock (this)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[] elementArray = new double[numElements];
					double[] elementArray = new double[numElements];
					Array.Copy(internalArray, startIndex, elementArray, 0, numElements);
					return elementArray;
				}
			}
		}

		/// <summary>
		/// The expansion factor controls the size of a new array when an array
		/// needs to be expanded.  The <code>expansionMode</code>
		/// determines whether the size of the array is multiplied by the
		/// <code>expansionFactor</code> (MULTIPLICATIVE_MODE) or if
		/// the expansion is additive (ADDITIVE_MODE -- <code>expansionFactor</code>
		/// storage locations added).  The default <code>expansionMode</code> is
		/// MULTIPLICATIVE_MODE and the default <code>expansionFactor</code>
		/// is 2.0.
		/// </summary>
		/// <returns> the expansion factor of this expandable double array </returns>
		/// @deprecated As of 3.1. Return type will be changed to "double" in 4.0. 
		[Obsolete("As of 3.1. Return type will be changed to "double" in 4.0.")]
		public virtual float ExpansionFactor
		{
			get
			{
				return (float) expansionFactor;
			}
			set
			{
				checkContractExpand(ContractionCriterion, value);
				// The check above verifies that the expansion factor is > 1.0;
				lock (this)
				{
					this.expansionFactor = value;
				}
			}
		}

		/// <summary>
		/// The expansion mode determines whether the internal storage
		/// array grows additively or multiplicatively when it is expanded.
		/// </summary>
		/// <returns> the expansion mode. </returns>
		/// @deprecated As of 3.1. Return value to be changed to
		/// <seealso cref="ExpansionMode"/> in 4.0. 
		[Obsolete("As of 3.1. Return value to be changed to")]
		public virtual int ExpansionMode
		{
			get
			{
				switch (expansionMode)
				{
				case org.apache.commons.math3.util.ResizableDoubleArray.ExpansionMode.MULTIPLICATIVE:
					return MULTIPLICATIVE_MODE;
				case org.apache.commons.math3.util.ResizableDoubleArray.ExpansionMode.ADDITIVE:
					return ADDITIVE_MODE;
				default:
					throw new MathInternalError(); // Should never happen.
				}
			}
			set
			{
				if (value != MULTIPLICATIVE_MODE && value != ADDITIVE_MODE)
				{
					throw new MathIllegalArgumentException(LocalizedFormats.UNSUPPORTED_EXPANSION_MODE, value, MULTIPLICATIVE_MODE, "MULTIPLICATIVE_MODE", ADDITIVE_MODE, "ADDITIVE_MODE");
				}
				lock (this)
				{
					if (value == MULTIPLICATIVE_MODE)
					{
						ExpansionMode = ExpansionMode.MULTIPLICATIVE;
					}
					else if (value == ADDITIVE_MODE)
					{
						ExpansionMode = ExpansionMode.ADDITIVE;
					}
				}
			}
		}

		/// <summary>
		/// Notice the package scope on this method.   This method is simply here
		/// for the JUnit test, it allows us check if the expansion is working
		/// properly after a number of expansions.  This is not meant to be a part
		/// of the public interface of this class.
		/// </summary>
		/// <returns> the length of the internal storage array. </returns>
		/// @deprecated As of 3.1. Please use <seealso cref="#getCapacity()"/> instead. 
		[Obsolete("As of 3.1. Please use <seealso cref="#getCapacity()"/> instead.")]
		internal virtual int InternalLength
		{
			get
			{
				lock (this)
				{
					return internalArray.Length;
				}
			}
		}

		/// <summary>
		/// Gets the currently allocated size of the internal data structure used
		/// for storing elements.
		/// This is not to be confused with {@link #getNumElements() the number of
		/// elements actually stored}.
		/// </summary>
		/// <returns> the length of the internal array.
		/// @since 3.1 </returns>
		public virtual int Capacity
		{
			get
			{
				return internalArray.Length;
			}
		}

		/// <summary>
		/// Returns the number of elements currently in the array.  Please note
		/// that this is different from the length of the internal storage array.
		/// </summary>
		/// <returns> the number of elements. </returns>
		public virtual int NumElements
		{
			get
			{
				lock (this)
				{
					return numElements;
				}
			}
			set
			{
				lock (this)
				{
					// If index is negative thrown an error.
					if (value < 0)
					{
						throw new MathIllegalArgumentException(LocalizedFormats.INDEX_NOT_POSITIVE, value);
					}
            
					// Test the new num elements, check to see if the array needs to be
					// expanded to accommodate this new number of elements.
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int newSize = startIndex + value;
					int newSize = startIndex + value;
					if (newSize > internalArray.Length)
					{
						expandTo(newSize);
					}
            
					// Set the new number of elements to new value.
					numElements = value;
				}
			}
		}

		/// <summary>
		/// Returns the internal storage array.  Note that this method returns
		/// a reference to the internal storage array, not a copy, and to correctly
		/// address elements of the array, the <code>startIndex</code> is
		/// required (available via the <seealso cref="#start"/> method).  This method should
		/// only be used in cases where copying the internal array is not practical.
		/// The <seealso cref="#getElements"/> method should be used in all other cases.
		/// 
		/// </summary>
		/// <returns> the internal storage array used by this object
		/// @since 2.0 </returns>
		/// @deprecated As of 3.1. 
		[Obsolete("As of 3.1.")]
		public virtual double[] InternalValues
		{
			get
			{
				lock (this)
				{
					return internalArray;
				}
			}
		}

		/// <summary>
		/// Provides <em>direct</em> access to the internal storage array.
		/// Please note that this method returns a reference to this object's
		/// storage array, not a copy.
		/// <br/>
		/// To correctly address elements of the array, the "start index" is
		/// required (available via the <seealso cref="#getStartIndex() getStartIndex"/>
		/// method.
		/// <br/>
		/// This method should only be used to avoid copying the internal array.
		/// The returned value <em>must</em> be used for reading only; other
		/// uses could lead to this object becoming inconsistent.
		/// <br/>
		/// The <seealso cref="#getElements"/> method has no such limitation since it
		/// returns a copy of this array's addressable elements.
		/// </summary>
		/// <returns> the internal storage array used by this object.
		/// @since 3.1 </returns>
		protected internal virtual double[] ArrayRef
		{
			get
			{
				return internalArray;
			}
		}

		/// <summary>
		/// Returns the "start index" of the internal array.
		/// This index is the position of the first addressable element in the
		/// internal storage array.
		/// The addressable elements in the array are at indices contained in
		/// the interval [<seealso cref="#getStartIndex()"/>,
		///               <seealso cref="#getStartIndex()"/> + <seealso cref="#getNumElements()"/> - 1].
		/// </summary>
		/// <returns> the start index.
		/// @since 3.1 </returns>
		protected internal virtual int StartIndex
		{
			get
			{
				return startIndex;
			}
		}

		/// <summary>
		/// Sets the contraction criteria.
		/// </summary>
		/// <param name="contractionCriteria"> contraction criteria </param>
		/// <exception cref="MathIllegalArgumentException"> if the contractionCriteria is less than
		///         the expansionCriteria. </exception>
		/// @deprecated As of 3.1 (to be removed in 4.0 as field will become "final"). 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("As of 3.1 (to be removed in 4.0 as field will become "final").") public void setContractionCriteria(float contractionCriteria) throws org.apache.commons.math3.exception.MathIllegalArgumentException

		/// <summary>
		/// Performs an operation on the addressable elements of the array.
		/// </summary>
		/// <param name="f"> Function to be applied on this array. </param>
		/// <returns> the result.
		/// @since 3.1 </returns>
		public virtual double compute(MathArrays.Function f)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] array;
			double[] array;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int start;
			int start;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int num;
			int num;
			lock (this)
			{
				array = internalArray;
				start = startIndex;
				num = numElements;
			}
			return f.evaluate(array, start, num);
		}

		/// <summary>
		/// Sets the element at the specified index.  If the specified index is greater than
		/// <code>getNumElements() - 1</code>, the <code>numElements</code> property
		/// is increased to <code>index +1</code> and additional storage is allocated
		/// (if necessary) for the new element and all  (uninitialized) elements
		/// between the new element and the previous end of the array).
		/// </summary>
		/// <param name="index"> index to store a value in </param>
		/// <param name="value"> value to store at the specified index </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if {@code index < 0}. </exception>
		public virtual void setElement(int index, double value)
		{
			lock (this)
			{
				if (index < 0)
				{
					throw new System.IndexOutOfRangeException(index);
				}
				if (index + 1 > numElements)
				{
					numElements = index + 1;
				}
				if ((startIndex + index) >= internalArray.Length)
				{
					expandTo(startIndex + (index + 1));
				}
				internalArray[startIndex + index] = value;
			}
		}

		/// <summary>
		/// Sets the expansionFactor.  Throws IllegalArgumentException if the
		/// the following conditions are not met:
		/// <ul>
		/// <li><code>expansionFactor > 1</code></li>
		/// <li><code>contractionFactor >= expansionFactor</code></li>
		/// </ul> </summary>
		/// <param name="expansionFactor"> the new expansion factor value. </param>
		/// <exception cref="MathIllegalArgumentException"> if expansionFactor is <= 1 or greater
		/// than contractionFactor </exception>
		/// @deprecated As of 3.1 (to be removed in 4.0 as field will become "final"). 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("As of 3.1 (to be removed in 4.0 as field will become "final").") public void setExpansionFactor(float expansionFactor) throws org.apache.commons.math3.exception.MathIllegalArgumentException

		/// <summary>
		/// Sets the <code>expansionMode</code>. The specified value must be one of
		/// ADDITIVE_MODE, MULTIPLICATIVE_MODE.
		/// </summary>
		/// <param name="expansionMode"> The expansionMode to set. </param>
		/// <exception cref="MathIllegalArgumentException"> if the specified mode value is not valid. </exception>
		/// @deprecated As of 3.1. Please use <seealso cref="#setExpansionMode(ExpansionMode)"/> instead. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("As of 3.1. Please use <seealso cref="#setExpansionMode(ExpansionMode)"/> instead.") public void setExpansionMode(int expansionMode) throws org.apache.commons.math3.exception.MathIllegalArgumentException

		/// <summary>
		/// Sets the <seealso cref="ExpansionMode expansion mode"/>.
		/// </summary>
		/// <param name="expansionMode"> Expansion mode to use for resizing the array. </param>
		/// @deprecated As of 3.1 (to be removed in 4.0 as field will become "final"). 
		[Obsolete("As of 3.1 (to be removed in 4.0 as field will become "final").")]
		public virtual ExpansionMode ExpansionMode
		{
			set
			{
				this.expansionMode = value;
			}
		}

		/// <summary>
		/// Sets the initial capacity.  Should only be invoked by constructors.
		/// </summary>
		/// <param name="initialCapacity"> of the array </param>
		/// <exception cref="MathIllegalArgumentException"> if <code>initialCapacity</code> is not
		/// positive. </exception>
		/// @deprecated As of 3.1, this is a no-op. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("As of 3.1, this is a no-op.") protected void setInitialCapacity(int initialCapacity) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		[Obsolete("As of 3.1, this is a no-op.")]
		protected internal virtual int InitialCapacity
		{
			set
			{
				// Body removed in 3.1.
			}
		}

		/// <summary>
		/// This function allows you to control the number of elements contained
		/// in this array, and can be used to "throw out" the last n values in an
		/// array. This function will also expand the internal array as needed.
		/// </summary>
		/// <param name="i"> a new number of elements </param>
		/// <exception cref="MathIllegalArgumentException"> if <code>i</code> is negative. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void setNumElements(int i) throws org.apache.commons.math3.exception.MathIllegalArgumentException

		/// <summary>
		/// Returns true if the internal storage array has too many unused
		/// storage positions.
		/// </summary>
		/// <returns> true if array satisfies the contraction criteria </returns>
		private bool shouldContract()
		{
			lock (this)
			{
				if (expansionMode == ExpansionMode.MULTIPLICATIVE)
				{
					return (internalArray.Length / ((float) numElements)) > contractionCriterion;
				}
				else
				{
					return (internalArray.Length - numElements) > contractionCriterion;
				}
			}
		}

		/// <summary>
		/// Returns the starting index of the internal array.  The starting index is
		/// the position of the first addressable element in the internal storage
		/// array.  The addressable elements in the array are <code>
		/// internalArray[startIndex],...,internalArray[startIndex + numElements -1]
		/// </code>
		/// </summary>
		/// <returns> the starting index. </returns>
		/// @deprecated As of 3.1. 
		[Obsolete("As of 3.1.")]
		public virtual int start()
		{
			lock (this)
			{
				return startIndex;
			}
		}

		/// <summary>
		/// <p>Copies source to dest, copying the underlying data, so dest is
		/// a new, independent copy of source.  Does not contract before
		/// the copy.</p>
		/// 
		/// <p>Obtains synchronization locks on both source and dest
		/// (in that order) before performing the copy.</p>
		/// 
		/// <p>Neither source nor dest may be null; otherwise a <seealso cref="NullArgumentException"/>
		/// is thrown</p>
		/// </summary>
		/// <param name="source"> ResizableDoubleArray to copy </param>
		/// <param name="dest"> ResizableArray to replace with a copy of the source array </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null
		/// @since 2.0
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(ResizableDoubleArray source, ResizableDoubleArray dest) throws org.apache.commons.math3.exception.NullArgumentException
		public static void copy(ResizableDoubleArray source, ResizableDoubleArray dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			lock (source)
			{
			   lock (dest)
			   {
				   dest.contractionCriterion = source.contractionCriterion;
				   dest.expansionFactor = source.expansionFactor;
				   dest.expansionMode = source.expansionMode;
				   dest.internalArray = new double[source.internalArray.Length];
				   Array.Copy(source.internalArray, 0, dest.internalArray, 0, dest.internalArray.Length);
				   dest.numElements = source.numElements;
				   dest.startIndex = source.startIndex;
			   }
			}
		}

		/// <summary>
		/// Returns a copy of the ResizableDoubleArray.  Does not contract before
		/// the copy, so the returned object is an exact copy of this.
		/// </summary>
		/// <returns> a new ResizableDoubleArray with the same data and configuration
		/// properties as this
		/// @since 2.0 </returns>
		public virtual ResizableDoubleArray copy()
		{
			lock (this)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ResizableDoubleArray result = new ResizableDoubleArray();
				ResizableDoubleArray result = new ResizableDoubleArray();
				copy(this, result);
				return result;
			}
		}

		/// <summary>
		/// Returns true iff object is a ResizableDoubleArray with the same properties
		/// as this and an identical internal storage array.
		/// </summary>
		/// <param name="object"> object to be compared for equality with this </param>
		/// <returns> true iff object is a ResizableDoubleArray with the same data and
		/// properties as this
		/// @since 2.0 </returns>
		public override bool Equals(object @object)
		{
			if (@object == this)
			{
				return true;
			}
			if (@object is ResizableDoubleArray == false)
			{
				return false;
			}
			lock (this)
			{
				lock (@object)
				{
					bool result = true;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ResizableDoubleArray other = (ResizableDoubleArray) object;
					ResizableDoubleArray other = (ResizableDoubleArray) @object;
					result = result && (other.contractionCriterion == contractionCriterion);
					result = result && (other.expansionFactor == expansionFactor);
					result = result && (other.expansionMode == expansionMode);
					result = result && (other.numElements == numElements);
					result = result && (other.startIndex == startIndex);
					if (!result)
					{
						return false;
					}
					else
					{
						return Arrays.Equals(internalArray, other.internalArray);
					}
				}
			}
		}

		/// <summary>
		/// Returns a hash code consistent with equals.
		/// </summary>
		/// <returns> the hash code representing this {@code ResizableDoubleArray}.
		/// @since 2.0 </returns>
		public override int GetHashCode()
		{
			lock (this)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] hashData = new int[6];
				int[] hashData = new int[6];
				hashData[0] = Convert.ToDouble(expansionFactor).GetHashCode();
				hashData[1] = Convert.ToDouble(contractionCriterion).GetHashCode();
				hashData[2] = expansionMode.GetHashCode();
				hashData[3] = Arrays.GetHashCode(internalArray);
				hashData[4] = numElements;
				hashData[5] = startIndex;
				return Arrays.GetHashCode(hashData);
			}
		}

	}

}
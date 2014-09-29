using System;
using System.Text;

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

namespace mathlib.fraction
{


	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using MathParseException = mathlib.exception.MathParseException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;

	/// <summary>
	/// Formats a BigFraction number in proper format or improper format.
	/// <p>
	/// The number format for each of the whole number, numerator and,
	/// denominator can be configured.
	/// </p>
	/// 
	/// @since 2.0
	/// @version $Id: BigFractionFormat.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class BigFractionFormat : AbstractFormat
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -2932167925527338976L;

		/// <summary>
		/// Create an improper formatting instance with the default number format
		/// for the numerator and denominator.
		/// </summary>
		public BigFractionFormat()
		{
		}

		/// <summary>
		/// Create an improper formatting instance with a custom number format for
		/// both the numerator and denominator. </summary>
		/// <param name="format"> the custom format for both the numerator and denominator. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFractionFormat(final java.text.NumberFormat format)
		public BigFractionFormat(NumberFormat format) : base(format)
		{
		}

		/// <summary>
		/// Create an improper formatting instance with a custom number format for
		/// the numerator and a custom number format for the denominator. </summary>
		/// <param name="numeratorFormat"> the custom format for the numerator. </param>
		/// <param name="denominatorFormat"> the custom format for the denominator. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFractionFormat(final java.text.NumberFormat numeratorFormat, final java.text.NumberFormat denominatorFormat)
		public BigFractionFormat(NumberFormat numeratorFormat, NumberFormat denominatorFormat) : base(numeratorFormat, denominatorFormat)
		{
		}

		/// <summary>
		/// Get the set of locales for which complex formats are available.  This
		/// is the same set as the <seealso cref="NumberFormat"/> set. </summary>
		/// <returns> available complex format locales. </returns>
		public static Locale[] AvailableLocales
		{
			get
			{
				return NumberFormat.AvailableLocales;
			}
		}

		/// <summary>
		/// This static method calls formatBigFraction() on a default instance of
		/// BigFractionFormat.
		/// </summary>
		/// <param name="f"> BigFraction object to format </param>
		/// <returns> A formatted BigFraction in proper form. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static String formatBigFraction(final BigFraction f)
		public static string formatBigFraction(BigFraction f)
		{
			return ImproperInstance.format(f);
		}

		/// <summary>
		/// Returns the default complex format for the current locale. </summary>
		/// <returns> the default complex format. </returns>
		public static BigFractionFormat ImproperInstance
		{
			get
			{
				return getImproperInstance(Locale.Default);
			}
		}

		/// <summary>
		/// Returns the default complex format for the given locale. </summary>
		/// <param name="locale"> the specific locale used by the format. </param>
		/// <returns> the complex format specific to the given locale. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static BigFractionFormat getImproperInstance(final java.util.Locale locale)
		public static BigFractionFormat getImproperInstance(Locale locale)
		{
			return new BigFractionFormat(getDefaultNumberFormat(locale));
		}

		/// <summary>
		/// Returns the default complex format for the current locale. </summary>
		/// <returns> the default complex format. </returns>
		public static BigFractionFormat ProperInstance
		{
			get
			{
				return getProperInstance(Locale.Default);
			}
		}

		/// <summary>
		/// Returns the default complex format for the given locale. </summary>
		/// <param name="locale"> the specific locale used by the format. </param>
		/// <returns> the complex format specific to the given locale. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static BigFractionFormat getProperInstance(final java.util.Locale locale)
		public static BigFractionFormat getProperInstance(Locale locale)
		{
			return new ProperBigFractionFormat(getDefaultNumberFormat(locale));
		}

		/// <summary>
		/// Formats a <seealso cref="BigFraction"/> object to produce a string.  The BigFraction is
		/// output in improper format.
		/// </summary>
		/// <param name="BigFraction"> the object to format. </param>
		/// <param name="toAppendTo"> where the text is to be appended </param>
		/// <param name="pos"> On input: an alignment field, if desired. On output: the
		///            offsets of the alignment field </param>
		/// <returns> the value passed in as toAppendTo. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public StringBuffer format(final BigFraction BigFraction, final StringBuffer toAppendTo, final java.text.FieldPosition pos)
		public virtual StringBuilder format(BigFraction BigFraction, StringBuilder toAppendTo, FieldPosition pos)
		{

			pos.BeginIndex = 0;
			pos.EndIndex = 0;

			NumeratorFormat.format(BigFraction.Numerator, toAppendTo, pos);
			toAppendTo.Append(" / ");
			DenominatorFormat.format(BigFraction.Denominator, toAppendTo, pos);

			return toAppendTo;
		}

		/// <summary>
		/// Formats an object and appends the result to a StringBuffer.
		/// <code>obj</code> must be either a  <seealso cref="BigFraction"/> object or a
		/// <seealso cref="BigInteger"/> object or a <seealso cref="Number"/> object. Any other type of
		/// object will result in an <seealso cref="IllegalArgumentException"/> being thrown.
		/// </summary>
		/// <param name="obj"> the object to format. </param>
		/// <param name="toAppendTo"> where the text is to be appended </param>
		/// <param name="pos"> On input: an alignment field, if desired. On output: the
		///            offsets of the alignment field </param>
		/// <returns> the value passed in as toAppendTo. </returns>
		/// <seealso cref= java.text.Format#format(java.lang.Object, java.lang.StringBuffer, java.text.FieldPosition) </seealso>
		/// <exception cref="MathIllegalArgumentException"> if <code>obj</code> is not a valid type. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public StringBuffer format(final Object obj, final StringBuffer toAppendTo, final java.text.FieldPosition pos)
		public override StringBuilder format(object obj, StringBuilder toAppendTo, FieldPosition pos)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer ret;
			StringBuilder ret;
			if (obj is BigFraction)
			{
				ret = format((BigFraction) obj, toAppendTo, pos);
			}
			else if (obj is System.Numerics.BigInteger)
			{
				ret = format(new BigFraction((System.Numerics.BigInteger) obj), toAppendTo, pos);
			}
			else if (obj is Number)
			{
				ret = format(new BigFraction((double)((Number) obj)), toAppendTo, pos);
			}
			else
			{
				throw new MathIllegalArgumentException(LocalizedFormats.CANNOT_FORMAT_OBJECT_TO_FRACTION);
			}

			return ret;
		}

		/// <summary>
		/// Parses a string to produce a <seealso cref="BigFraction"/> object. </summary>
		/// <param name="source"> the string to parse </param>
		/// <returns> the parsed <seealso cref="BigFraction"/> object. </returns>
		/// <exception cref="MathParseException"> if the beginning of the specified string
		///            cannot be parsed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public BigFraction parse(final String source) throws mathlib.exception.MathParseException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override BigFraction parse(string source)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.ParsePosition parsePosition = new java.text.ParsePosition(0);
			ParsePosition parsePosition = new ParsePosition(0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BigFraction result = parse(source, parsePosition);
			BigFraction result = parse(source, parsePosition);
			if (parsePosition.Index == 0)
			{
				throw new MathParseException(source, parsePosition.ErrorIndex, typeof(BigFraction));
			}
			return result;
		}

		/// <summary>
		/// Parses a string to produce a <seealso cref="BigFraction"/> object.
		/// This method expects the string to be formatted as an improper BigFraction. </summary>
		/// <param name="source"> the string to parse </param>
		/// <param name="pos"> input/output parsing parameter. </param>
		/// <returns> the parsed <seealso cref="BigFraction"/> object. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public BigFraction parse(final String source, final java.text.ParsePosition pos)
		public override BigFraction parse(string source, ParsePosition pos)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int initialIndex = pos.getIndex();
			int initialIndex = pos.Index;

			// parse whitespace
			parseAndIgnoreWhitespace(source, pos);

			// parse numerator
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.math.BigInteger num = parseNextBigInteger(source, pos);
			System.Numerics.BigInteger num = parseNextBigInteger(source, pos);
			if (num == null)
			{
				// invalid integer number
				// set index back to initial, error index should already be set
				// character examined.
				pos.Index = initialIndex;
				return null;
			}

			// parse '/'
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int startIndex = pos.getIndex();
			int startIndex = pos.Index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char c = parseNextCharacter(source, pos);
			char c = parseNextCharacter(source, pos);
			switch (c)
			{
			case 0 :
				// no '/'
				// return num as a BigFraction
				return new BigFraction(num);
			case '/' :
				// found '/', continue parsing denominator
				break;
			default :
				// invalid '/'
				// set index back to initial, error index should be the last
				// character examined.
				pos.Index = initialIndex;
				pos.ErrorIndex = startIndex;
				return null;
			}

			// parse whitespace
			parseAndIgnoreWhitespace(source, pos);

			// parse denominator
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.math.BigInteger den = parseNextBigInteger(source, pos);
			System.Numerics.BigInteger den = parseNextBigInteger(source, pos);
			if (den == null)
			{
				// invalid integer number
				// set index back to initial, error index should already be set
				// character examined.
				pos.Index = initialIndex;
				return null;
			}

			return new BigFraction(num, den);
		}

		/// <summary>
		/// Parses a string to produce a <code>BigInteger</code>. </summary>
		/// <param name="source"> the string to parse </param>
		/// <param name="pos"> input/output parsing parameter. </param>
		/// <returns> a parsed <code>BigInteger</code> or null if string does not
		/// contain a BigInteger at the specified position </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected java.math.BigInteger parseNextBigInteger(final String source, final java.text.ParsePosition pos)
		protected internal virtual System.Numerics.BigInteger parseNextBigInteger(string source, ParsePosition pos)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int start = pos.getIndex();
			int start = pos.Index;
			 int end = (source[start] == '-') ? (start + 1) : start;
			 while ((end < source.Length) && char.IsDigit(source[end]))
			 {
				 ++end;
			 }

			 try
			 {
				 System.Numerics.BigInteger n = new System.Numerics.BigInteger(source.Substring(start, end - start));
				 pos.Index = end;
				 return n;
			 }
			 catch (NumberFormatException nfe)
			 {
				 pos.ErrorIndex = start;
				 return null;
			 }

		}

	}

}
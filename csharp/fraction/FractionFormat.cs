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

namespace org.apache.commons.math3.fraction
{


	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using MathParseException = org.apache.commons.math3.exception.MathParseException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// Formats a Fraction number in proper format or improper format.  The number
	/// format for each of the whole number, numerator and, denominator can be
	/// configured.
	/// 
	/// @since 1.1
	/// @version $Id: FractionFormat.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class FractionFormat : AbstractFormat
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 3008655719530972611L;

		/// <summary>
		/// Create an improper formatting instance with the default number format
		/// for the numerator and denominator.
		/// </summary>
		public FractionFormat()
		{
		}

		/// <summary>
		/// Create an improper formatting instance with a custom number format for
		/// both the numerator and denominator. </summary>
		/// <param name="format"> the custom format for both the numerator and denominator. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FractionFormat(final java.text.NumberFormat format)
		public FractionFormat(NumberFormat format) : base(format)
		{
		}

		/// <summary>
		/// Create an improper formatting instance with a custom number format for
		/// the numerator and a custom number format for the denominator. </summary>
		/// <param name="numeratorFormat"> the custom format for the numerator. </param>
		/// <param name="denominatorFormat"> the custom format for the denominator. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FractionFormat(final java.text.NumberFormat numeratorFormat, final java.text.NumberFormat denominatorFormat)
		public FractionFormat(NumberFormat numeratorFormat, NumberFormat denominatorFormat) : base(numeratorFormat, denominatorFormat)
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
		/// This static method calls formatFraction() on a default instance of
		/// FractionFormat.
		/// </summary>
		/// <param name="f"> Fraction object to format </param>
		/// <returns> a formatted fraction in proper form. </returns>
		public static string formatFraction(Fraction f)
		{
			return ImproperInstance.format(f);
		}

		/// <summary>
		/// Returns the default complex format for the current locale. </summary>
		/// <returns> the default complex format. </returns>
		public static FractionFormat ImproperInstance
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
//ORIGINAL LINE: public static FractionFormat getImproperInstance(final java.util.Locale locale)
		public static FractionFormat getImproperInstance(Locale locale)
		{
			return new FractionFormat(getDefaultNumberFormat(locale));
		}

		/// <summary>
		/// Returns the default complex format for the current locale. </summary>
		/// <returns> the default complex format. </returns>
		public static FractionFormat ProperInstance
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
//ORIGINAL LINE: public static FractionFormat getProperInstance(final java.util.Locale locale)
		public static FractionFormat getProperInstance(Locale locale)
		{
			return new ProperFractionFormat(getDefaultNumberFormat(locale));
		}

		/// <summary>
		/// Create a default number format.  The default number format is based on
		/// <seealso cref="NumberFormat#getNumberInstance(java.util.Locale)"/> with the only
		/// customizing is the maximum number of fraction digits, which is set to 0. </summary>
		/// <returns> the default number format. </returns>
		protected internal static NumberFormat DefaultNumberFormat
		{
			get
			{
				return getDefaultNumberFormat(Locale.Default);
			}
		}

		/// <summary>
		/// Formats a <seealso cref="Fraction"/> object to produce a string.  The fraction is
		/// output in improper format.
		/// </summary>
		/// <param name="fraction"> the object to format. </param>
		/// <param name="toAppendTo"> where the text is to be appended </param>
		/// <param name="pos"> On input: an alignment field, if desired. On output: the
		///            offsets of the alignment field </param>
		/// <returns> the value passed in as toAppendTo. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public StringBuffer format(final Fraction fraction, final StringBuffer toAppendTo, final java.text.FieldPosition pos)
		public virtual StringBuilder format(Fraction fraction, StringBuilder toAppendTo, FieldPosition pos)
		{

			pos.BeginIndex = 0;
			pos.EndIndex = 0;

			NumeratorFormat.format(fraction.Numerator, toAppendTo, pos);
			toAppendTo.Append(" / ");
			DenominatorFormat.format(fraction.Denominator, toAppendTo, pos);

			return toAppendTo;
		}

		/// <summary>
		/// Formats an object and appends the result to a StringBuffer. <code>obj</code> must be either a
		/// <seealso cref="Fraction"/> object or a <seealso cref="Number"/> object.  Any other type of
		/// object will result in an <seealso cref="IllegalArgumentException"/> being thrown.
		/// </summary>
		/// <param name="obj"> the object to format. </param>
		/// <param name="toAppendTo"> where the text is to be appended </param>
		/// <param name="pos"> On input: an alignment field, if desired. On output: the
		///            offsets of the alignment field </param>
		/// <returns> the value passed in as toAppendTo. </returns>
		/// <seealso cref= java.text.Format#format(java.lang.Object, java.lang.StringBuffer, java.text.FieldPosition) </seealso>
		/// <exception cref="FractionConversionException"> if the number cannot be converted to a fraction </exception>
		/// <exception cref="MathIllegalArgumentException"> if <code>obj</code> is not a valid type. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public StringBuffer format(final Object obj, final StringBuffer toAppendTo, final java.text.FieldPosition pos) throws FractionConversionException, org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override StringBuilder format(object obj, StringBuilder toAppendTo, FieldPosition pos)
		{
			StringBuilder ret = null;

			if (obj is Fraction)
			{
				ret = format((Fraction) obj, toAppendTo, pos);
			}
			else if (obj is Number)
			{
				ret = format(new Fraction((double)((Number) obj)), toAppendTo, pos);
			}
			else
			{
				throw new MathIllegalArgumentException(LocalizedFormats.CANNOT_FORMAT_OBJECT_TO_FRACTION);
			}

			return ret;
		}

		/// <summary>
		/// Parses a string to produce a <seealso cref="Fraction"/> object. </summary>
		/// <param name="source"> the string to parse </param>
		/// <returns> the parsed <seealso cref="Fraction"/> object. </returns>
		/// <exception cref="MathParseException"> if the beginning of the specified string
		///            cannot be parsed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Fraction parse(final String source) throws org.apache.commons.math3.exception.MathParseException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override Fraction parse(string source)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.ParsePosition parsePosition = new java.text.ParsePosition(0);
			ParsePosition parsePosition = new ParsePosition(0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Fraction result = parse(source, parsePosition);
			Fraction result = parse(source, parsePosition);
			if (parsePosition.Index == 0)
			{
				throw new MathParseException(source, parsePosition.ErrorIndex, typeof(Fraction));
			}
			return result;
		}

		/// <summary>
		/// Parses a string to produce a <seealso cref="Fraction"/> object.  This method
		/// expects the string to be formatted as an improper fraction. </summary>
		/// <param name="source"> the string to parse </param>
		/// <param name="pos"> input/output parsing parameter. </param>
		/// <returns> the parsed <seealso cref="Fraction"/> object. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public Fraction parse(final String source, final java.text.ParsePosition pos)
		public override Fraction parse(string source, ParsePosition pos)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int initialIndex = pos.getIndex();
			int initialIndex = pos.Index;

			// parse whitespace
			parseAndIgnoreWhitespace(source, pos);

			// parse numerator
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Number num = getNumeratorFormat().parse(source, pos);
			Number num = NumeratorFormat.parse(source, pos);
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
				// return num as a fraction
				return new Fraction((int)num, 1);
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
//ORIGINAL LINE: final Number den = getDenominatorFormat().parse(source, pos);
			Number den = DenominatorFormat.parse(source, pos);
			if (den == null)
			{
				// invalid integer number
				// set index back to initial, error index should already be set
				// character examined.
				pos.Index = initialIndex;
				return null;
			}

			return new Fraction((int)num, (int)den);
		}

	}

}
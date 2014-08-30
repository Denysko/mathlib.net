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


	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;

	/// <summary>
	/// Formats a BigFraction number in proper format.  The number format for each of
	/// the whole number, numerator and, denominator can be configured.
	/// <p>
	/// Minus signs are only allowed in the whole number part - i.e.,
	/// "-3 1/2" is legitimate and denotes -7/2, but "-3 -1/2" is invalid and
	/// will result in a <code>ParseException</code>.</p>
	/// 
	/// @since 1.1
	/// @version $Id: ProperBigFractionFormat.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class ProperBigFractionFormat : BigFractionFormat
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -6337346779577272307L;

		/// <summary>
		/// The format used for the whole number. </summary>
		private NumberFormat wholeFormat;

		/// <summary>
		/// Create a proper formatting instance with the default number format for
		/// the whole, numerator, and denominator.
		/// </summary>
		public ProperBigFractionFormat() : this(DefaultNumberFormat)
		{
		}

		/// <summary>
		/// Create a proper formatting instance with a custom number format for the
		/// whole, numerator, and denominator. </summary>
		/// <param name="format"> the custom format for the whole, numerator, and
		///        denominator. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ProperBigFractionFormat(final java.text.NumberFormat format)
		public ProperBigFractionFormat(NumberFormat format) : this(format, (NumberFormat)format.clone(), (NumberFormat)format.clone())
		{
		}

		/// <summary>
		/// Create a proper formatting instance with a custom number format for each
		/// of the whole, numerator, and denominator. </summary>
		/// <param name="wholeFormat"> the custom format for the whole. </param>
		/// <param name="numeratorFormat"> the custom format for the numerator. </param>
		/// <param name="denominatorFormat"> the custom format for the denominator. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ProperBigFractionFormat(final java.text.NumberFormat wholeFormat, final java.text.NumberFormat numeratorFormat, final java.text.NumberFormat denominatorFormat)
		public ProperBigFractionFormat(NumberFormat wholeFormat, NumberFormat numeratorFormat, NumberFormat denominatorFormat) : base(numeratorFormat, denominatorFormat)
		{
			WholeFormat = wholeFormat;
		}

		/// <summary>
		/// Formats a <seealso cref="BigFraction"/> object to produce a string.  The BigFraction
		/// is output in proper format.
		/// </summary>
		/// <param name="fraction"> the object to format. </param>
		/// <param name="toAppendTo"> where the text is to be appended </param>
		/// <param name="pos"> On input: an alignment field, if desired. On output: the
		///            offsets of the alignment field </param>
		/// <returns> the value passed in as toAppendTo. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public StringBuffer format(final BigFraction fraction, final StringBuffer toAppendTo, final java.text.FieldPosition pos)
		public override StringBuilder format(BigFraction fraction, StringBuilder toAppendTo, FieldPosition pos)
		{

			pos.BeginIndex = 0;
			pos.EndIndex = 0;

			System.Numerics.BigInteger num = fraction.Numerator;
			System.Numerics.BigInteger den = fraction.Denominator;
			System.Numerics.BigInteger whole = num / den;
			num = num.remainder(den);

			if (!System.Numerics.BigInteger.ZERO.Equals(whole))
			{
				WholeFormat.format(whole, toAppendTo, pos);
				toAppendTo.Append(' ');
				if (num.compareTo(System.Numerics.BigInteger.ZERO) < 0)
				{
					num = -num;
				}
			}
			NumeratorFormat.format(num, toAppendTo, pos);
			toAppendTo.Append(" / ");
			DenominatorFormat.format(den, toAppendTo, pos);

			return toAppendTo;
		}

		/// <summary>
		/// Access the whole format. </summary>
		/// <returns> the whole format. </returns>
		public virtual NumberFormat WholeFormat
		{
			get
			{
				return wholeFormat;
			}
			set
			{
				if (value == null)
				{
					throw new NullArgumentException(LocalizedFormats.WHOLE_FORMAT);
				}
				this.wholeFormat = value;
			}
		}

		/// <summary>
		/// Parses a string to produce a <seealso cref="BigFraction"/> object.  This method
		/// expects the string to be formatted as a proper BigFraction.
		/// <p>
		/// Minus signs are only allowed in the whole number part - i.e.,
		/// "-3 1/2" is legitimate and denotes -7/2, but "-3 -1/2" is invalid and
		/// will result in a <code>ParseException</code>.</p>
		/// </summary>
		/// <param name="source"> the string to parse </param>
		/// <param name="pos"> input/ouput parsing parameter. </param>
		/// <returns> the parsed <seealso cref="BigFraction"/> object. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public BigFraction parse(final String source, final java.text.ParsePosition pos)
		public override BigFraction parse(string source, ParsePosition pos)
		{
			// try to parse improper BigFraction
			BigFraction ret = base.parse(source, pos);
			if (ret != null)
			{
				return ret;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int initialIndex = pos.getIndex();
			int initialIndex = pos.Index;

			// parse whitespace
			parseAndIgnoreWhitespace(source, pos);

			// parse whole
			System.Numerics.BigInteger whole = parseNextBigInteger(source, pos);
			if (whole == null)
			{
				// invalid integer number
				// set index back to initial, error index should already be set
				// character examined.
				pos.Index = initialIndex;
				return null;
			}

			// parse whitespace
			parseAndIgnoreWhitespace(source, pos);

			// parse numerator
			System.Numerics.BigInteger num = parseNextBigInteger(source, pos);
			if (num == null)
			{
				// invalid integer number
				// set index back to initial, error index should already be set
				// character examined.
				pos.Index = initialIndex;
				return null;
			}

			if (num.compareTo(System.Numerics.BigInteger.ZERO) < 0)
			{
				// minus signs should be leading, invalid expression
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

			if (den.compareTo(System.Numerics.BigInteger.ZERO) < 0)
			{
				// minus signs must be leading, invalid
				pos.Index = initialIndex;
				return null;
			}

			bool wholeIsNeg = whole.compareTo(System.Numerics.BigInteger.ZERO) < 0;
			if (wholeIsNeg)
			{
				whole = -whole;
			}
			num = whole * den + num;
			if (wholeIsNeg)
			{
				num = -num;
			}

			return new BigFraction(num, den);

		}

		/// <summary>
		/// Modify the whole format. </summary>
		/// <param name="format"> The new whole format value. </param>
		/// <exception cref="NullArgumentException"> if {@code format} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setWholeFormat(final java.text.NumberFormat format)
	}

}
using System.Collections.Generic;
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

namespace mathlib.linear
{


	using MathParseException = mathlib.exception.MathParseException;
	using CompositeFormat = mathlib.util.CompositeFormat;

	/// <summary>
	/// Formats a vector in components list format "{v0; v1; ...; vk-1}".
	/// <p>The prefix and suffix "{" and "}" and the separator "; " can be replaced by
	/// any user-defined strings. The number format for components can be configured.</p>
	/// <p>White space is ignored at parse time, even if it is in the prefix, suffix
	/// or separator specifications. So even if the default separator does include a space
	/// character that is used at format time, both input string "{1;1;1}" and
	/// " { 1 ; 1 ; 1 } " will be parsed without error and the same vector will be
	/// returned. In the second case, however, the parse position after parsing will be
	/// just after the closing curly brace, i.e. just before the trailing space.</p>
	/// 
	/// @version $Id: RealVectorFormat.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0
	/// </summary>
	public class RealVectorFormat
	{

		/// <summary>
		/// The default prefix: "{". </summary>
		private const string DEFAULT_PREFIX = "{";
		/// <summary>
		/// The default suffix: "}". </summary>
		private const string DEFAULT_SUFFIX = "}";
		/// <summary>
		/// The default separator: ", ". </summary>
		private const string DEFAULT_SEPARATOR = "; ";
		/// <summary>
		/// Prefix. </summary>
		private readonly string prefix;
		/// <summary>
		/// Suffix. </summary>
		private readonly string suffix;
		/// <summary>
		/// Separator. </summary>
		private readonly string separator;
		/// <summary>
		/// Trimmed prefix. </summary>
		private readonly string trimmedPrefix;
		/// <summary>
		/// Trimmed suffix. </summary>
		private readonly string trimmedSuffix;
		/// <summary>
		/// Trimmed separator. </summary>
		private readonly string trimmedSeparator;
		/// <summary>
		/// The format used for components. </summary>
		private readonly NumberFormat format_Renamed;

		/// <summary>
		/// Create an instance with default settings.
		/// <p>The instance uses the default prefix, suffix and separator:
		/// "{", "}", and "; " and the default number format for components.</p>
		/// </summary>
		public RealVectorFormat() : this(DEFAULT_PREFIX, DEFAULT_SUFFIX, DEFAULT_SEPARATOR, CompositeFormat.DefaultNumberFormat)
		{
		}

		/// <summary>
		/// Create an instance with a custom number format for components. </summary>
		/// <param name="format"> the custom format for components. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RealVectorFormat(final java.text.NumberFormat format)
		public RealVectorFormat(NumberFormat format) : this(DEFAULT_PREFIX, DEFAULT_SUFFIX, DEFAULT_SEPARATOR, format)
		{
		}

		/// <summary>
		/// Create an instance with custom prefix, suffix and separator. </summary>
		/// <param name="prefix"> prefix to use instead of the default "{" </param>
		/// <param name="suffix"> suffix to use instead of the default "}" </param>
		/// <param name="separator"> separator to use instead of the default "; " </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RealVectorFormat(final String prefix, final String suffix, final String separator)
		public RealVectorFormat(string prefix, string suffix, string separator) : this(prefix, suffix, separator, CompositeFormat.DefaultNumberFormat)
		{
		}

		/// <summary>
		/// Create an instance with custom prefix, suffix, separator and format
		/// for components. </summary>
		/// <param name="prefix"> prefix to use instead of the default "{" </param>
		/// <param name="suffix"> suffix to use instead of the default "}" </param>
		/// <param name="separator"> separator to use instead of the default "; " </param>
		/// <param name="format"> the custom format for components. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RealVectorFormat(final String prefix, final String suffix, final String separator, final java.text.NumberFormat format)
		public RealVectorFormat(string prefix, string suffix, string separator, NumberFormat format)
		{
			this.prefix = prefix;
			this.suffix = suffix;
			this.separator = separator;
			trimmedPrefix = prefix.Trim();
			trimmedSuffix = suffix.Trim();
			trimmedSeparator = separator.Trim();
			this.format_Renamed = format;
		}

		/// <summary>
		/// Get the set of locales for which real vectors formats are available.
		/// <p>This is the same set as the <seealso cref="NumberFormat"/> set.</p> </summary>
		/// <returns> available real vector format locales. </returns>
		public static Locale[] AvailableLocales
		{
			get
			{
				return NumberFormat.AvailableLocales;
			}
		}

		/// <summary>
		/// Get the format prefix. </summary>
		/// <returns> format prefix. </returns>
		public virtual string Prefix
		{
			get
			{
				return prefix;
			}
		}

		/// <summary>
		/// Get the format suffix. </summary>
		/// <returns> format suffix. </returns>
		public virtual string Suffix
		{
			get
			{
				return suffix;
			}
		}

		/// <summary>
		/// Get the format separator between components. </summary>
		/// <returns> format separator. </returns>
		public virtual string Separator
		{
			get
			{
				return separator;
			}
		}

		/// <summary>
		/// Get the components format. </summary>
		/// <returns> components format. </returns>
		public virtual NumberFormat Format
		{
			get
			{
				return format_Renamed;
			}
		}

		/// <summary>
		/// Returns the default real vector format for the current locale. </summary>
		/// <returns> the default real vector format. </returns>
		public static RealVectorFormat Instance
		{
			get
			{
				return getInstance(Locale.Default);
			}
		}

		/// <summary>
		/// Returns the default real vector format for the given locale. </summary>
		/// <param name="locale"> the specific locale used by the format. </param>
		/// <returns> the real vector format specific to the given locale. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static RealVectorFormat getInstance(final java.util.Locale locale)
		public static RealVectorFormat getInstance(Locale locale)
		{
			return new RealVectorFormat(CompositeFormat.getDefaultNumberFormat(locale));
		}

		/// <summary>
		/// This method calls <seealso cref="#format(RealVector,StringBuffer,FieldPosition)"/>.
		/// </summary>
		/// <param name="v"> RealVector object to format. </param>
		/// <returns> a formatted vector. </returns>
		public virtual string format(RealVector v)
		{
			return format(v, new StringBuilder(), new FieldPosition(0)).ToString();
		}

		/// <summary>
		/// Formats a <seealso cref="RealVector"/> object to produce a string. </summary>
		/// <param name="vector"> the object to format. </param>
		/// <param name="toAppendTo"> where the text is to be appended </param>
		/// <param name="pos"> On input: an alignment field, if desired. On output: the
		///            offsets of the alignment field </param>
		/// <returns> the value passed in as toAppendTo. </returns>
		public virtual StringBuilder format(RealVector vector, StringBuilder toAppendTo, FieldPosition pos)
		{

			pos.BeginIndex = 0;
			pos.EndIndex = 0;

			// format prefix
			toAppendTo.Append(prefix);

			// format components
			for (int i = 0; i < vector.Dimension; ++i)
			{
				if (i > 0)
				{
					toAppendTo.Append(separator);
				}
				CompositeFormat.formatDouble(vector.getEntry(i), format_Renamed, toAppendTo, pos);
			}

			// format suffix
			toAppendTo.Append(suffix);

			return toAppendTo;
		}

		/// <summary>
		/// Parse a string to produce a <seealso cref="RealVector"/> object.
		/// </summary>
		/// <param name="source"> String to parse. </param>
		/// <returns> the parsed <seealso cref="RealVector"/> object. </returns>
		/// <exception cref="MathParseException"> if the beginning of the specified string
		/// cannot be parsed. </exception>
		public virtual ArrayRealVector parse(string source)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.ParsePosition parsePosition = new java.text.ParsePosition(0);
			ParsePosition parsePosition = new ParsePosition(0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ArrayRealVector result = parse(source, parsePosition);
			ArrayRealVector result = parse(source, parsePosition);
			if (parsePosition.Index == 0)
			{
				throw new MathParseException(source, parsePosition.ErrorIndex, typeof(ArrayRealVector));
			}
			return result;
		}

		/// <summary>
		/// Parse a string to produce a <seealso cref="RealVector"/> object.
		/// </summary>
		/// <param name="source"> String to parse. </param>
		/// <param name="pos"> input/ouput parsing parameter. </param>
		/// <returns> the parsed <seealso cref="RealVector"/> object. </returns>
		public virtual ArrayRealVector parse(string source, ParsePosition pos)
		{
			int initialIndex = pos.Index;

			// parse prefix
			CompositeFormat.parseAndIgnoreWhitespace(source, pos);
			if (!CompositeFormat.parseFixedstring(source, trimmedPrefix, pos))
			{
				return null;
			}

			// parse components
			IList<Number> components = new List<Number>();
			for (bool loop = true; loop;)
			{

				if (components.Count > 0)
				{
					CompositeFormat.parseAndIgnoreWhitespace(source, pos);
					if (!CompositeFormat.parseFixedstring(source, trimmedSeparator, pos))
					{
						loop = false;
					}
				}

				if (loop)
				{
					CompositeFormat.parseAndIgnoreWhitespace(source, pos);
					Number component = CompositeFormat.parseNumber(source, format_Renamed, pos);
					if (component != null)
					{
						components.Add(component);
					}
					else
					{
						// invalid component
						// set index back to initial, error index should already be set
						pos.Index = initialIndex;
						return null;
					}
				}

			}

			// parse suffix
			CompositeFormat.parseAndIgnoreWhitespace(source, pos);
			if (!CompositeFormat.parseFixedstring(source, trimmedSuffix, pos))
			{
				return null;
			}

			// build vector
			double[] data = new double[components.Count];
			for (int i = 0; i < data.Length; ++i)
			{
				data[i] = (double)components[i];
			}
			return new ArrayRealVector(data, false);
		}
	}

}
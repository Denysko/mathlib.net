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

namespace mathlib.geometry
{


	using CompositeFormat = mathlib.util.CompositeFormat;
	using MathParseException = mathlib.exception.MathParseException;

	/// <summary>
	/// Formats a vector in components list format "{x; y; ...}".
	/// <p>The prefix and suffix "{" and "}" and the separator "; " can be replaced by
	/// any user-defined strings. The number format for components can be configured.</p>
	/// <p>White space is ignored at parse time, even if it is in the prefix, suffix
	/// or separator specifications. So even if the default separator does include a space
	/// character that is used at format time, both input string "{1;1;1}" and
	/// " { 1 ; 1 ; 1 } " will be parsed without error and the same vector will be
	/// returned. In the second case, however, the parse position after parsing will be
	/// just after the closing curly brace, i.e. just before the trailing space.</p>
	/// <p><b>Note:</b> using "," as a separator may interfere with the grouping separator
	/// of the default <seealso cref="NumberFormat"/> for the current locale. Thus it is advised
	/// to use a <seealso cref="NumberFormat"/> instance with disabled grouping in such a case.</p>
	/// </summary>
	/// @param <S> Type of the space.
	/// @version $Id: VectorFormat.java 1467801 2013-04-14 16:19:33Z tn $
	/// @since 3.0 </param>
	public abstract class VectorFormat<S> where S : Space
	{

		/// <summary>
		/// The default prefix: "{". </summary>
		public const string DEFAULT_PREFIX = "{";

		/// <summary>
		/// The default suffix: "}". </summary>
		public const string DEFAULT_SUFFIX = "}";

		/// <summary>
		/// The default separator: ", ". </summary>
		public const string DEFAULT_SEPARATOR = "; ";

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
		protected internal VectorFormat() : this(DEFAULT_PREFIX, DEFAULT_SUFFIX, DEFAULT_SEPARATOR, CompositeFormat.DefaultNumberFormat)
		{
		}

		/// <summary>
		/// Create an instance with a custom number format for components. </summary>
		/// <param name="format"> the custom format for components. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected VectorFormat(final java.text.NumberFormat format)
		protected internal VectorFormat(NumberFormat format) : this(DEFAULT_PREFIX, DEFAULT_SUFFIX, DEFAULT_SEPARATOR, format)
		{
		}

		/// <summary>
		/// Create an instance with custom prefix, suffix and separator. </summary>
		/// <param name="prefix"> prefix to use instead of the default "{" </param>
		/// <param name="suffix"> suffix to use instead of the default "}" </param>
		/// <param name="separator"> separator to use instead of the default "; " </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected VectorFormat(final String prefix, final String suffix, final String separator)
		protected internal VectorFormat(string prefix, string suffix, string separator) : this(prefix, suffix, separator, CompositeFormat.DefaultNumberFormat)
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
//ORIGINAL LINE: protected VectorFormat(final String prefix, final String suffix, final String separator, final java.text.NumberFormat format)
		protected internal VectorFormat(string prefix, string suffix, string separator, NumberFormat format)
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
		/// Get the set of locales for which point/vector formats are available.
		/// <p>This is the same set as the <seealso cref="NumberFormat"/> set.</p> </summary>
		/// <returns> available point/vector format locales. </returns>
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
		/// Formats a <seealso cref="Vector"/> object to produce a string. </summary>
		/// <param name="vector"> the object to format. </param>
		/// <returns> a formatted string. </returns>
		public virtual string format(Vector<S> vector)
		{
			return format(vector, new StringBuilder(), new FieldPosition(0)).ToString();
		}

		/// <summary>
		/// Formats a <seealso cref="Vector"/> object to produce a string. </summary>
		/// <param name="vector"> the object to format. </param>
		/// <param name="toAppendTo"> where the text is to be appended </param>
		/// <param name="pos"> On input: an alignment field, if desired. On output: the
		///            offsets of the alignment field </param>
		/// <returns> the value passed in as toAppendTo. </returns>
		public abstract StringBuilder format(Vector<S> vector, StringBuilder toAppendTo, FieldPosition pos);

		/// <summary>
		/// Formats the coordinates of a <seealso cref="Vector"/> to produce a string. </summary>
		/// <param name="toAppendTo"> where the text is to be appended </param>
		/// <param name="pos"> On input: an alignment field, if desired. On output: the
		///            offsets of the alignment field </param>
		/// <param name="coordinates"> coordinates of the object to format. </param>
		/// <returns> the value passed in as toAppendTo. </returns>
		protected internal virtual StringBuilder format(StringBuilder toAppendTo, FieldPosition pos, params double[] coordinates)
		{

			pos.BeginIndex = 0;
			pos.EndIndex = 0;

			// format prefix
			toAppendTo.Append(prefix);

			// format components
			for (int i = 0; i < coordinates.Length; ++i)
			{
				if (i > 0)
				{
					toAppendTo.Append(separator);
				}
				CompositeFormat.formatDouble(coordinates[i], format_Renamed, toAppendTo, pos);
			}

			// format suffix
			toAppendTo.Append(suffix);

			return toAppendTo;

		}

		/// <summary>
		/// Parses a string to produce a <seealso cref="Vector"/> object. </summary>
		/// <param name="source"> the string to parse </param>
		/// <returns> the parsed <seealso cref="Vector"/> object. </returns>
		/// <exception cref="MathParseException"> if the beginning of the specified string
		/// cannot be parsed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract Vector<S> parse(String source) throws mathlib.exception.MathParseException;
		public abstract Vector<S> parse(string source);

		/// <summary>
		/// Parses a string to produce a <seealso cref="Vector"/> object. </summary>
		/// <param name="source"> the string to parse </param>
		/// <param name="pos"> input/output parsing parameter. </param>
		/// <returns> the parsed <seealso cref="Vector"/> object. </returns>
		public abstract Vector<S> parse(string source, ParsePosition pos);

		/// <summary>
		/// Parses a string to produce an array of coordinates. </summary>
		/// <param name="dimension"> dimension of the space </param>
		/// <param name="source"> the string to parse </param>
		/// <param name="pos"> input/output parsing parameter. </param>
		/// <returns> coordinates array. </returns>
		protected internal virtual double[] parseCoordinates(int dimension, string source, ParsePosition pos)
		{

			int initialIndex = pos.Index;
			double[] coordinates = new double[dimension];

			// parse prefix
			CompositeFormat.parseAndIgnoreWhitespace(source, pos);
			if (!CompositeFormat.parseFixedstring(source, trimmedPrefix, pos))
			{
				return null;
			}

			for (int i = 0; i < dimension; ++i)
			{

				// skip whitespace
				CompositeFormat.parseAndIgnoreWhitespace(source, pos);

				// parse separator
				if (i > 0 && !CompositeFormat.parseFixedstring(source, trimmedSeparator, pos))
				{
					return null;
				}

				// skip whitespace
				CompositeFormat.parseAndIgnoreWhitespace(source, pos);

				// parse coordinate
				Number c = CompositeFormat.parseNumber(source, format_Renamed, pos);
				if (c == null)
				{
					// invalid coordinate
					// set index back to initial, error index should already be set
					pos.Index = initialIndex;
					return null;
				}

				// store coordinate
				coordinates[i] = (double)c;

			}

			// parse suffix
			CompositeFormat.parseAndIgnoreWhitespace(source, pos);
			if (!CompositeFormat.parseFixedstring(source, trimmedSuffix, pos))
			{
				return null;
			}

			return coordinates;

		}

	}

}
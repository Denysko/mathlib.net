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
	/// Formats a {@code nxm} matrix in components list format
	/// "{{a<sub>0</sub><sub>0</sub>,a<sub>0</sub><sub>1</sub>, ...,
	/// a<sub>0</sub><sub>m-1</sub>},{a<sub>1</sub><sub>0</sub>,
	/// a<sub>1</sub><sub>1</sub>, ..., a<sub>1</sub><sub>m-1</sub>},{...},{
	/// a<sub>n-1</sub><sub>0</sub>, a<sub>n-1</sub><sub>1</sub>, ...,
	/// a<sub>n-1</sub><sub>m-1</sub>}}".
	/// <p>The prefix and suffix "{" and "}", the row prefix and suffix "{" and "}",
	/// the row separator "," and the column separator "," can be replaced by any
	/// user-defined strings. The number format for components can be configured.</p>
	/// 
	/// <p>White space is ignored at parse time, even if it is in the prefix, suffix
	/// or separator specifications. So even if the default separator does include a space
	/// character that is used at format time, both input string "{{1,1,1}}" and
	/// " { { 1 , 1 , 1 } } " will be parsed without error and the same matrix will be
	/// returned. In the second case, however, the parse position after parsing will be
	/// just after the closing curly brace, i.e. just before the trailing space.</p>
	/// 
	/// <p><b>Note:</b> the grouping functionality of the used <seealso cref="NumberFormat"/> is
	/// disabled to prevent problems when parsing (e.g. 1,345.34 would be a valid number
	/// but conflicts with the default column separator).</p>
	/// 
	/// @since 3.1
	/// @version $Id: RealMatrixFormat.java 1364793 2012-07-23 20:46:28Z tn $
	/// </summary>
	public class RealMatrixFormat
	{

		/// <summary>
		/// The default prefix: "{". </summary>
		private const string DEFAULT_PREFIX = "{";
		/// <summary>
		/// The default suffix: "}". </summary>
		private const string DEFAULT_SUFFIX = "}";
		/// <summary>
		/// The default row prefix: "{". </summary>
		private const string DEFAULT_ROW_PREFIX = "{";
		/// <summary>
		/// The default row suffix: "}". </summary>
		private const string DEFAULT_ROW_SUFFIX = "}";
		/// <summary>
		/// The default row separator: ",". </summary>
		private const string DEFAULT_ROW_SEPARATOR = ",";
		/// <summary>
		/// The default column separator: ",". </summary>
		private const string DEFAULT_COLUMN_SEPARATOR = ",";
		/// <summary>
		/// Prefix. </summary>
		private readonly string prefix;
		/// <summary>
		/// Suffix. </summary>
		private readonly string suffix;
		/// <summary>
		/// Row prefix. </summary>
		private readonly string rowPrefix;
		/// <summary>
		/// Row suffix. </summary>
		private readonly string rowSuffix;
		/// <summary>
		/// Row separator. </summary>
		private readonly string rowSeparator;
		/// <summary>
		/// Column separator. </summary>
		private readonly string columnSeparator;
		/// <summary>
		/// The format used for components. </summary>
		private readonly NumberFormat format_Renamed;

		/// <summary>
		/// Create an instance with default settings.
		/// <p>The instance uses the default prefix, suffix and row/column separator:
		/// "[", "]", ";" and ", " and the default number format for components.</p>
		/// </summary>
		public RealMatrixFormat() : this(DEFAULT_PREFIX, DEFAULT_SUFFIX, DEFAULT_ROW_PREFIX, DEFAULT_ROW_SUFFIX, DEFAULT_ROW_SEPARATOR, DEFAULT_COLUMN_SEPARATOR, CompositeFormat.DefaultNumberFormat)
		{
		}

		/// <summary>
		/// Create an instance with a custom number format for components. </summary>
		/// <param name="format"> the custom format for components. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RealMatrixFormat(final java.text.NumberFormat format)
		public RealMatrixFormat(NumberFormat format) : this(DEFAULT_PREFIX, DEFAULT_SUFFIX, DEFAULT_ROW_PREFIX, DEFAULT_ROW_SUFFIX, DEFAULT_ROW_SEPARATOR, DEFAULT_COLUMN_SEPARATOR, format)
		{
		}

		/// <summary>
		/// Create an instance with custom prefix, suffix and separator. </summary>
		/// <param name="prefix"> prefix to use instead of the default "{" </param>
		/// <param name="suffix"> suffix to use instead of the default "}" </param>
		/// <param name="rowPrefix"> row prefix to use instead of the default "{" </param>
		/// <param name="rowSuffix"> row suffix to use instead of the default "}" </param>
		/// <param name="rowSeparator"> tow separator to use instead of the default ";" </param>
		/// <param name="columnSeparator"> column separator to use instead of the default ", " </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RealMatrixFormat(final String prefix, final String suffix, final String rowPrefix, final String rowSuffix, final String rowSeparator, final String columnSeparator)
		public RealMatrixFormat(string prefix, string suffix, string rowPrefix, string rowSuffix, string rowSeparator, string columnSeparator) : this(prefix, suffix, rowPrefix, rowSuffix, rowSeparator, columnSeparator, CompositeFormat.DefaultNumberFormat)
		{
		}

		/// <summary>
		/// Create an instance with custom prefix, suffix, separator and format
		/// for components. </summary>
		/// <param name="prefix"> prefix to use instead of the default "{" </param>
		/// <param name="suffix"> suffix to use instead of the default "}" </param>
		/// <param name="rowPrefix"> row prefix to use instead of the default "{" </param>
		/// <param name="rowSuffix"> row suffix to use instead of the default "}" </param>
		/// <param name="rowSeparator"> tow separator to use instead of the default ";" </param>
		/// <param name="columnSeparator"> column separator to use instead of the default ", " </param>
		/// <param name="format"> the custom format for components. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RealMatrixFormat(final String prefix, final String suffix, final String rowPrefix, final String rowSuffix, final String rowSeparator, final String columnSeparator, final java.text.NumberFormat format)
		public RealMatrixFormat(string prefix, string suffix, string rowPrefix, string rowSuffix, string rowSeparator, string columnSeparator, NumberFormat format)
		{
			this.prefix = prefix;
			this.suffix = suffix;
			this.rowPrefix = rowPrefix;
			this.rowSuffix = rowSuffix;
			this.rowSeparator = rowSeparator;
			this.columnSeparator = columnSeparator;
			this.format_Renamed = format;
			// disable grouping to prevent parsing problems
			this.format_Renamed.GroupingUsed = false;
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
		/// Get the format prefix. </summary>
		/// <returns> format prefix. </returns>
		public virtual string RowPrefix
		{
			get
			{
				return rowPrefix;
			}
		}

		/// <summary>
		/// Get the format suffix. </summary>
		/// <returns> format suffix. </returns>
		public virtual string RowSuffix
		{
			get
			{
				return rowSuffix;
			}
		}

		/// <summary>
		/// Get the format separator between rows of the matrix. </summary>
		/// <returns> format separator for rows. </returns>
		public virtual string RowSeparator
		{
			get
			{
				return rowSeparator;
			}
		}

		/// <summary>
		/// Get the format separator between components. </summary>
		/// <returns> format separator between components. </returns>
		public virtual string ColumnSeparator
		{
			get
			{
				return columnSeparator;
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
		public static RealMatrixFormat Instance
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
//ORIGINAL LINE: public static RealMatrixFormat getInstance(final java.util.Locale locale)
		public static RealMatrixFormat getInstance(Locale locale)
		{
			return new RealMatrixFormat(CompositeFormat.getDefaultNumberFormat(locale));
		}

		/// <summary>
		/// This method calls <seealso cref="#format(RealMatrix,StringBuffer,FieldPosition)"/>.
		/// </summary>
		/// <param name="m"> RealMatrix object to format. </param>
		/// <returns> a formatted matrix. </returns>
		public virtual string format(RealMatrix m)
		{
			return format(m, new StringBuilder(), new FieldPosition(0)).ToString();
		}

		/// <summary>
		/// Formats a <seealso cref="RealMatrix"/> object to produce a string. </summary>
		/// <param name="matrix"> the object to format. </param>
		/// <param name="toAppendTo"> where the text is to be appended </param>
		/// <param name="pos"> On input: an alignment field, if desired. On output: the
		///            offsets of the alignment field </param>
		/// <returns> the value passed in as toAppendTo. </returns>
		public virtual StringBuilder format(RealMatrix matrix, StringBuilder toAppendTo, FieldPosition pos)
		{

			pos.BeginIndex = 0;
			pos.EndIndex = 0;

			// format prefix
			toAppendTo.Append(prefix);

			// format rows
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rows = matrix.getRowDimension();
			int rows = matrix.RowDimension;
			for (int i = 0; i < rows; ++i)
			{
				toAppendTo.Append(rowPrefix);
				for (int j = 0; j < matrix.ColumnDimension; ++j)
				{
					if (j > 0)
					{
						toAppendTo.Append(columnSeparator);
					}
					CompositeFormat.formatDouble(matrix.getEntry(i, j), format_Renamed, toAppendTo, pos);
				}
				toAppendTo.Append(rowSuffix);
				if (i < rows - 1)
				{
					toAppendTo.Append(rowSeparator);
				}
			}

			// format suffix
			toAppendTo.Append(suffix);

			return toAppendTo;
		}

		/// <summary>
		/// Parse a string to produce a <seealso cref="RealMatrix"/> object.
		/// </summary>
		/// <param name="source"> String to parse. </param>
		/// <returns> the parsed <seealso cref="RealMatrix"/> object. </returns>
		/// <exception cref="MathParseException"> if the beginning of the specified string
		/// cannot be parsed. </exception>
		public virtual RealMatrix parse(string source)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.ParsePosition parsePosition = new java.text.ParsePosition(0);
			ParsePosition parsePosition = new ParsePosition(0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RealMatrix result = parse(source, parsePosition);
			RealMatrix result = parse(source, parsePosition);
			if (parsePosition.Index == 0)
			{
				throw new MathParseException(source, parsePosition.ErrorIndex, typeof(Array2DRowRealMatrix));
			}
			return result;
		}

		/// <summary>
		/// Parse a string to produce a <seealso cref="RealMatrix"/> object.
		/// </summary>
		/// <param name="source"> String to parse. </param>
		/// <param name="pos"> input/ouput parsing parameter. </param>
		/// <returns> the parsed <seealso cref="RealMatrix"/> object. </returns>
		public virtual RealMatrix parse(string source, ParsePosition pos)
		{
			int initialIndex = pos.Index;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String trimmedPrefix = prefix.trim();
			string trimmedPrefix = prefix.Trim();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String trimmedSuffix = suffix.trim();
			string trimmedSuffix = suffix.Trim();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String trimmedRowPrefix = rowPrefix.trim();
			string trimmedRowPrefix = rowPrefix.Trim();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String trimmedRowSuffix = rowSuffix.trim();
			string trimmedRowSuffix = rowSuffix.Trim();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String trimmedColumnSeparator = columnSeparator.trim();
			string trimmedColumnSeparator = columnSeparator.Trim();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String trimmedRowSeparator = rowSeparator.trim();
			string trimmedRowSeparator = rowSeparator.Trim();

			// parse prefix
			CompositeFormat.parseAndIgnoreWhitespace(source, pos);
			if (!CompositeFormat.parseFixedstring(source, trimmedPrefix, pos))
			{
				return null;
			}

			// parse components
			IList<IList<Number>> matrix = new List<IList<Number>>();
			IList<Number> rowComponents = new List<Number>();
			for (bool loop = true; loop;)
			{

				if (rowComponents.Count > 0)
				{
					CompositeFormat.parseAndIgnoreWhitespace(source, pos);
					if (!CompositeFormat.parseFixedstring(source, trimmedColumnSeparator, pos))
					{
						if (trimmedRowSuffix.Length != 0 && !CompositeFormat.parseFixedstring(source, trimmedRowSuffix, pos))
						{
							return null;
						}
						else
						{
							CompositeFormat.parseAndIgnoreWhitespace(source, pos);
							if (CompositeFormat.parseFixedstring(source, trimmedRowSeparator, pos))
							{
								matrix.Add(rowComponents);
								rowComponents = new List<Number>();
								continue;
							}
							else
							{
								loop = false;
							}
						}
					}
				}
				else
				{
					CompositeFormat.parseAndIgnoreWhitespace(source, pos);
					if (trimmedRowPrefix.Length != 0 && !CompositeFormat.parseFixedstring(source, trimmedRowPrefix, pos))
					{
						return null;
					}
				}

				if (loop)
				{
					CompositeFormat.parseAndIgnoreWhitespace(source, pos);
					Number component = CompositeFormat.parseNumber(source, format_Renamed, pos);
					if (component != null)
					{
						rowComponents.Add(component);
					}
					else
					{
						if (rowComponents.Count == 0)
						{
							loop = false;
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

			}

			if (rowComponents.Count > 0)
			{
				matrix.Add(rowComponents);
			}

			// parse suffix
			CompositeFormat.parseAndIgnoreWhitespace(source, pos);
			if (!CompositeFormat.parseFixedstring(source, trimmedSuffix, pos))
			{
				return null;
			}

			// do not allow an empty matrix
			if (matrix.Count == 0)
			{
				pos.Index = initialIndex;
				return null;
			}

			// build vector
			double[][] data = new double[matrix.Count][];
			int row = 0;
			foreach (IList<Number> rowList in matrix)
			{
				data[row] = new double[rowList.Count];
				for (int i = 0; i < rowList.Count; i++)
				{
					data[row][i] = (double)rowList[i];
				}
				row++;
			}
			return MatrixUtils.createRealMatrix(data);
		}
	}

}
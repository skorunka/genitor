// ReSharper disable CheckNamespace
namespace Genitor.Library.Core
// ReSharper restore CheckNamespace
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.IO.Compression;
	using System.Linq;
	using System.Text;
	using System.Web;
	using System.Web.UI;

	[DebuggerStepThrough]
	public static class StringExtensions
	{
		#region Interfaces

		private interface ITextExpression
		{
			#region Public Methods and Operators

			string Eval(object o);

			#endregion
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Null-safe, invariant culture, ignore case string comparison.
		/// </summary>
		public static bool AreEqual(string s1, string s2)
		{
			return (s1 ?? string.Empty).IsCaseInsensitiveEqual(s2 ?? string.Empty);
		}

		public static string AsNullIfEmpty(this string items)
		{
			return string.IsNullOrEmpty(items) ? null : items;
		}

		public static string AsNullIfWhiteSpace(this string items)
		{
			return string.IsNullOrWhiteSpace(items) ? null : items;
		}

		/// <summary>
		/// Join strings with provided separator. Only string which are not IsNullOrWhiteSpace are joined.
		/// </summary>
		/// <param name="separator">The separator.</param>
		/// <param name="values">Strings to join.</param>
		/// <returns>Joined string.</returns>
		public static string JoinNotEmpty(string separator, params string[] values)
		{
			return string.Join(separator, values.Where(x => !string.IsNullOrWhiteSpace(x)));
		}

		/// <summary>
		/// Compresses the specified instance.
		/// </summary>
		/// <param name="instance"> The instance. </param>
		/// <returns> </returns>
		public static string Compress(this string instance)
		{
			Guard.IsNotNullOrEmpty(instance, "instance");

			var binary = Encoding.UTF8.GetBytes(instance);
			byte[] compressed;

			using (var ms = new MemoryStream())
			{
				using (var zip = new GZipStream(ms, CompressionMode.Compress))
				{
					zip.Write(binary, 0, binary.Length);
				}

				compressed = ms.ToArray();
			}

			var compressedWithLength = new byte[compressed.Length + 4];

			Buffer.BlockCopy(compressed, 0, compressedWithLength, 4, compressed.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(binary.Length), 0, compressedWithLength, 0, 4);

			return Convert.ToBase64String(compressedWithLength);
		}

		/// <summary>
		/// 	Decompresses the specified instance.
		/// </summary>
		/// <param name="instance"> The instance. </param>
		/// <returns> </returns>
		public static string Decompress(this string instance)
		{
			Guard.IsNotNullOrEmpty(instance, "instance");

			var compressed = Convert.FromBase64String(instance);
			byte[] binary;

			using (var ms = new MemoryStream())
			{
				var length = BitConverter.ToInt32(compressed, 0);
				ms.Write(compressed, 4, compressed.Length - 4);

				binary = new byte[length];

				ms.Seek(0, SeekOrigin.Begin);

				using (var zip = new GZipStream(ms, CompressionMode.Decompress))
				{
					zip.Read(binary, 0, binary.Length);
				}
			}

			return Encoding.UTF8.GetString(binary);
		}

		/// <summary>
		/// 	Determines whether this instance and another specified System.String object have the same value. Case insensitive.
		/// </summary>
		/// <param name="instance"> The string to check equality. </param>
		/// <param name="comparing"> The comparing with string. </param>
		/// <returns> <c>true</c> if the value of the comparing parameter is the same as this string; otherwise, <c>false</c> . </returns>
		public static bool EqualsInsensitive(this string instance, string comparing)
		{
			return instance.IsCaseInsensitiveEqual(comparing);
		}

		/// <summary>
		/// 	Replaces the format item in a specified System.String with the text equivalent of the value of a corresponding System.Object instance in a specified array.
		/// </summary>
		/// <param name="instance"> A string to format. </param>
		/// <param name="args"> An System.Object array containing zero or more objects to format. </param>
		/// <returns> A copy of format in which the format items have been replaced by the System.String equivalent of the corresponding instances of System.Object in args. </returns>
		public static string FormatWith(this string instance, params object[] args)
		{
			return string.Format(CultureInfo.CurrentUICulture, instance, args);
		}

		/// <summary>
		/// 	Naformatuje string s properties v data objektu.
		/// </summary>
		/// <param name="format"> Formatovaci text. </param>
		/// <param name="data"> Dynamicky vytvoreny objekt s properties k formatovani. </param>
		/// <example>
		/// 	Localizovany text = "Vitejte na {SiteName}", data = new { SiteName : "hotels.cz" } -> "Vitejte na hotels.cz"
		/// </example>
		public static string FormatWithObject(this string format, object data)
		{
			Guard.IsNotNull(format, "format");
			Guard.IsNotNull(data, "data");

			var formattedStrings = (from expression in SplitFormat(format) select expression.Eval(data)).ToArray();
			return string.Join(string.Empty, formattedStrings);
		}

		/// <summary>
		/// 	Determines whether this instance and another specified System.String object have the same value. Case insensitive.
		/// </summary>
		/// <param name="instance"> The string to check equality. </param>
		/// <param name="comparing"> The comparing with string. </param>
		/// <returns> <c>true</c> if the value of the comparing parameter is the same as this string; otherwise, <c>false</c> . </returns>
		public static bool IsCaseInsensitiveEqual(this string instance, string comparing)
		{
			return string.Compare(instance, comparing, StringComparison.OrdinalIgnoreCase) == 0;
		}

		/// <summary>
		/// 	Determines whether this instance and another specified System.String object have the same value. Case sensitive.
		/// </summary>
		/// <param name="instance"> The string to check equality. </param>
		/// <param name="comparing"> The comparing with string. </param>
		/// <returns> <c>true</c> if the value of the comparing parameter is the same as this string; otherwise, <c>false</c> . </returns>
		public static bool IsCaseSensitiveEqual(this string instance, string comparing)
		{
			return string.CompareOrdinal(instance, comparing) == 0;
		}

		public static string Random(int size)
		{
			var builder = new StringBuilder();
			var random = new Random();
			for (var i = 0; i < size; i++)
			{
				var ch = Convert.ToChar(Convert.ToInt32(Math.Floor((26 * random.NextDouble()) + 65)));
				builder.Append(ch);
			}

			return builder.ToString();
		}

		public static string[] Split(this string source, char ch)
		{
			return source.Split(new[] { ch });
		}

		public static string[] Split(this string source, string s)
		{
			return source.Split(new[] { s }, StringSplitOptions.RemoveEmptyEntries);
		}

		public static string SubText(this string text, int size, string suffix = "...")
		{
			if (string.IsNullOrWhiteSpace(text) || text.Length <= size)
			{
				return text;
			}

			var subText = text.Substring(0, size);
			var lastSpace = subText.LastIndexOf(' ');
			return (lastSpace < 2 ? subText : subText.Substring(0, lastSpace)) + suffix;
		}

		public static string ToSafeUrl(this string source)
		{
			return source.Replace(' ', '-').ToLower();
		}

		#endregion

		#region Methods

		private static int IndexOfExpressionEnd(this string format, int startIndex)
		{
			var endBraceIndex = format.IndexOf('}', startIndex);
			if (endBraceIndex == -1)
			{
				return endBraceIndex;
			}

			//// start peeking ahead until there are no more braces... }}}
			var braceCount = 0;
			for (var i = endBraceIndex + 1; i < format.Length; i++)
			{
				if (format[i] != '}')
				{
					break;
				}

				braceCount++;
			}

			return braceCount % 2 == 1 ? IndexOfExpressionEnd(format, endBraceIndex + braceCount + 1) : endBraceIndex;
		}

		private static int IndexOfExpressionStart(this string format, int startIndex)
		{
			var index = format.IndexOf('{', startIndex);
			if (index == -1)
			{
				return index;
			}

			//// peek ahead
			if (index + 1 < format.Length)
			{
				if (format[index + 1] == '{')
				{
					return IndexOfExpressionStart(format, index + 2);
				}
			}

			return index;
		}

		private static IEnumerable<ITextExpression> SplitFormat(string format)
		{
			var exprEndIndex = -1;
			int expStartIndex;

			do
			{
				expStartIndex = format.IndexOfExpressionStart(exprEndIndex + 1);
				if (expStartIndex < 0)
				{
					//// everything after last end brace index.
					if (exprEndIndex + 1 < format.Length)
					{
						yield return new LiteralFormat(format.Substring(exprEndIndex + 1));
					}

					break;
				}

				if (expStartIndex - exprEndIndex - 1 > 0)
				{
					//// everything up to next start brace index
					yield return new LiteralFormat(format.Substring(exprEndIndex + 1, expStartIndex - exprEndIndex - 1));
				}

				var endBraceIndex = format.IndexOfExpressionEnd(expStartIndex + 1);
				if (endBraceIndex < 0)
				{
					//// rest of string, no end brace (could be invalid expression)
					yield return new FormatExpression(format.Substring(expStartIndex));
				}
				else
				{
					exprEndIndex = endBraceIndex;
					//// everything from start to end brace.
					yield return new FormatExpression(format.Substring(expStartIndex, endBraceIndex - expStartIndex + 1));
				}
			}
			while (expStartIndex > -1);
		}

		#endregion

		private class FormatExpression : ITextExpression
		{
			#region Constants and Fields

			private readonly bool _invalidExpression;

			#endregion

			#region Constructors and Destructors

			public FormatExpression(string expression)
			{
				if (!expression.StartsWith("{") || !expression.EndsWith("}"))
				{
					this._invalidExpression = true;
					this.Expression = expression;
					return;
				}

				var expressionWithoutBraces = expression.Substring(1, expression.Length - 2);
				var colonIndex = expressionWithoutBraces.IndexOf(':');
				if (colonIndex < 0)
				{
					this.Expression = expressionWithoutBraces;
				}
				else
				{
					this.Expression = expressionWithoutBraces.Substring(0, colonIndex);
					this.Format = expressionWithoutBraces.Substring(colonIndex + 1);
				}
			}

			#endregion

			#region Properties

			private string Expression { get; set; }

			private string Format { get; set; }

			#endregion

			#region Public Methods and Operators

			public string Eval(object o)
			{
				if (this._invalidExpression)
				{
					throw new FormatException(Properties.ErrorTexts.InvalidExpression);
				}

				try
				{
					return string.IsNullOrEmpty(this.Format) ? (DataBinder.Eval(o, this.Expression) ?? string.Empty).ToString() : DataBinder.Eval(o, this.Expression, "{0:" + this.Format + "}");
				}
				catch (ArgumentException)
				{
					throw new FormatException();
				}
				catch (HttpException)
				{
					throw new FormatException();
				}
			}

			#endregion
		}

		private class LiteralFormat : ITextExpression
		{
			#region Constructors and Destructors

			public LiteralFormat(string literalText)
			{
				this.LiteralText = literalText;
			}

			#endregion

			#region Properties

			private string LiteralText { get; set; }

			#endregion

			#region Public Methods and Operators

			public string Eval(object o)
			{
				var literalText = this.LiteralText.Replace("{{", "{").Replace("}}", "}");
				return literalText;
			}

			#endregion
		}
	}
}
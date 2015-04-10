using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Genitor.Library.Core.Configuration
{
	public static class ConfigHandlers
	{
		public static string StringHandler(string value)
		{
			return value;
		}

		public static Regex RegexHandler(string value)
		{
			try
			{
				return new Regex(value);
			}
			catch (Exception exc)
			{
				throw new ConfigInvalidValueException("Regex(" + exc.Message + ")");
			}
		}

		public static Encoding EncodingHandler(string value)
		{
			try
			{
				return Encoding.GetEncoding(value);
			}
			catch
			{
				throw new ConfigInvalidValueException("Encoding");
			}
		}

		public static short Int16Handler(string value)
		{
			short decValue;
			if (!short.TryParse(value, NumberStyles.Currency, CultureInfo.InvariantCulture, out decValue))
				throw new ConfigInvalidValueException("Short");
			return decValue;
		}

		public static int Int32Handler(string value)
		{
			int retVal;
			NumberStyles numberStyle;

			if (value.StartsWith("0x"))
			{
				numberStyle = NumberStyles.HexNumber;
				value = value.Substring(2);
			}
			else
				numberStyle = NumberStyles.Integer;

			if (!Int32.TryParse(value, numberStyle, CultureInfo.InvariantCulture, out retVal))
				throw new ConfigInvalidValueException("Int32");

			return retVal;
		}

		public static double DoubleHandler(string value)
		{
			double doubleValue;
			if (!Double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out doubleValue))
				throw new ConfigInvalidValueException("Double");
			return doubleValue;
		}

		public static decimal DecimalHandler(string value)
		{
			decimal decValue;
			if (!Decimal.TryParse(value, NumberStyles.Currency, CultureInfo.InvariantCulture, out decValue))
				throw new ConfigInvalidValueException("Decimal");
			return decValue;
		}

		private static readonly Regex _sInt32RangeRegex = new Regex(@"^\s*(\d+)\s*(?:\-\s*(\d+))?\s*$");
		public static Range<int> Int32RangeHandler(string value)
		{
			var match = _sInt32RangeRegex.Match(value);
			int from, to;

			if (!Int32.TryParse(match.Groups[1].Value, out from))
				throw new ConfigInvalidValueException("Int32Range");

			if (match.Groups[2].Length > 0)
			{
				if (!Int32.TryParse(match.Groups[2].Value, out to))
					throw new ConfigInvalidValueException("Int32Range");
			}
			else
			{
				to = from;
			}

			return new Range<int>(from, to);
		}

		private static readonly Regex _sTimeSpanVerboseRegex = new Regex(@"^\s*(?:(\d+)(d|h|m|s|ms)\s*)+$");
		//private static readonly Regex sTimeSpanDotNetRegex = new Regex(@"^\s*(?:(?<d0>\d+)|(?<d1>\d+\.)?(?<h>\d+)\:(?<m>\d+)(?:\:(?<s>\d+)(?:\.(?<ms>\d+))?)?)\s*$");
		public static TimeSpan TimeSpanHandler(string value)
		{
			// nejdriv zkusime nas format (10h 15m)
			var match = _sTimeSpanVerboseRegex.Match(value);
			if (match.Success)
				return ParseVerboseFormat(match);

			// kdyz neprojde, zkusime .net format (10:15)
			TimeSpan result;
			if (TimeSpan.TryParse(value, out result))
				return result;

			throw new ConfigInvalidValueException("TimeSpan");
		}

		private static TimeSpan ParseVerboseFormat(Match match)
		{
			long ticks = 0;

			for (int i = 0; i < match.Groups[1].Captures.Count; ++i)
			{
				string amountStr = match.Groups[1].Captures[i].Value;
				string units = match.Groups[2].Captures[i].Value;

				int amount = Int32.Parse(amountStr);

				switch (units)
				{
					case "d": ticks += amount * TimeSpan.TicksPerDay; break;
					case "h": ticks += amount * TimeSpan.TicksPerHour; break;
					case "m": ticks += amount * TimeSpan.TicksPerMinute; break;
					case "s": ticks += amount * TimeSpan.TicksPerSecond; break;
					case "ms": ticks += amount * TimeSpan.TicksPerMillisecond; break;
				}
			}

			return new TimeSpan(ticks);
		}

		public static bool BoolHandler(string value)
		{
			value = value.Trim().ToLowerInvariant();
			if (value == "true" || value == "yes" || value == "on" || value == "1")
				return true;
			if (value == "false" || value == "no" || value == "off" || value == "0")
				return false;

			throw new ConfigInvalidValueException("Bool");
		}

		public static string FilePathHandler(string path)
		{
			path = Tools.PathUtils.MapPath(path).Replace('/', '\\');

			var dir = Path.GetDirectoryName(path);
			if (dir != null && !Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			return path;
		}

		public static string ConnectionStringHandler(string connStringName)
		{
			ConnectionStringSettings connStr = ConfigurationManager.ConnectionStrings[connStringName];
			if (connStr == null)
				throw new ConfigInvalidValueException(String.Format("Connection string named '{0}' was not found in configuration.", connStringName));

			return connStr.ConnectionString;
		}

		public static object EnumHandler(string value, Type enumType)
		{
			try
			{
				return Enum.Parse(enumType, value, true);
			}
			catch
			{
				throw new ConfigInvalidValueException("Enum(" + enumType.Name + ")");
			}
		}

		public static IList<string> CsvHandler(string value)
		{
			return value.Split(new[] { ',' });
		}

		public static IDictionary<string, string> StringDictionaryHandler(string value)
		{
			return
				value.Split(new[] { ';' })
					.Where(o => !String.IsNullOrEmpty(o))
					.Select(o => o.Split('='))
					.Where(o => o.Length > 1)
					.ToDictionary(k => k[0], v => v[1]);
		}

		public static string GetHandlerName(Type fieldType)
		{
			if (fieldType == typeof(string))
				return "StringHandler";
			if (fieldType == typeof(Regex))
				return "RegexHandler";
			if (fieldType == typeof(Encoding))
				return "EncodingHandler";
			if (fieldType == typeof(short))
				return "Int16Handler";
			if (fieldType == typeof(int))
				return "Int32Handler";
			if (fieldType == typeof(double))
				return "DoubleHandler";
			if (fieldType == typeof(decimal))
				return "DecimalHandler";
			if (fieldType == typeof(Range<int>))
				return "Int32RangeHandler";
			if (fieldType == typeof(TimeSpan))
				return "TimeSpanHandler";
			if (fieldType == typeof(bool))
				return "BoolHandler";
			if (fieldType.IsSubclassOf(typeof(Enum)))
				return "EnumHandler";
			if (fieldType == typeof(IDictionary<string, string>))
				return "StringDictionaryHandler";
			if (fieldType == typeof(IList<string>))
				return "CsvHandler";

			return null;
		}
	}
}

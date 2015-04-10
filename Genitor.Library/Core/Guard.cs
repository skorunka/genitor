namespace Genitor.Library.Core
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;

	using Genitor.Library.Properties;

	/// <summary>
	/// Helper class for argument validation.
	/// </summary>
	public static class Guard
	{
		/// <summary>
		/// Ensures the specified collection is null.
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		[DebuggerStepThrough]
		public static void IsNull(object parameter, string parameterName)
		{
			if (parameter != null)
			{
				throw new ArgumentException(parameterName, ErrorTexts.MustBeNullOrEmpty.FormatWith(parameterName));
			}
		}

		/// <summary>
		/// Ensures the specified collection is not null.
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		[DebuggerStepThrough]
		public static void IsNotNull(object parameter, string parameterName)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException(parameterName, ErrorTexts.CannotBeNull.FormatWith(parameterName));
			}
		}

		[DebuggerStepThrough]
		public static void IsTrue(bool condition, string parameterName)
		{
			if (!condition)
			{
				throw new ArgumentNullException(parameterName, ErrorTexts.IsNotTrue.FormatWith(parameterName));
			}
		}

		/// <summary>
		/// Ensures the specified string is not blank.
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		[DebuggerStepThrough]
		public static void IsNotNullOrEmpty(string parameter, string parameterName)
		{
			if (string.IsNullOrEmpty((parameter ?? string.Empty).Trim()))
			{
				throw new ArgumentException(ErrorTexts.CannotBeNullOrEmpty.FormatWith(parameterName));
			}
		}

		/// <summary>
		/// Ensures the specified array is not null or empty.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="parameter">The parameter.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		[DebuggerStepThrough]
		public static void IsNotNullOrEmpty<T>(T[] parameter, string parameterName)
		{
			IsNotNull(parameter, parameterName);

			if (parameter.Length == 0)
			{
				throw new ArgumentException(ErrorTexts.ArrayCannotBeEmpty.FormatWith(parameterName));
			}
		}

		/// <summary>
		/// Ensures the specified collection is not null or empty.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="parameter">The parameter.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		[DebuggerStepThrough]
		public static void IsNotNullOrEmpty<T>(ICollection<T> parameter, string parameterName)
		{
			IsNotNull(parameter, parameterName);

			if (parameter.Count == 0)
			{
				throw new ArgumentException(ErrorTexts.CollectionCannotBeEmpty.FormatWith(parameterName), parameterName);
			}
		}

		/// <summary>
		/// Ensures the specified value is positive integer.
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		[DebuggerStepThrough]
		public static void IsNotZeroOrNegative(int parameter, string parameterName)
		{
			if (parameter <= 0)
			{
				throw new ArgumentOutOfRangeException(parameterName, ErrorTexts.CannotBeNegativeOrZero.FormatWith(parameterName));
			}
		}

		/// <summary>
		/// Ensures the specified value is not negative integer.
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		[DebuggerStepThrough]
		public static void IsNotNegative(int parameter, string parameterName)
		{
			if (parameter < 0)
			{
				throw new ArgumentOutOfRangeException(parameterName, ErrorTexts.CannotBeNegative.FormatWith(parameterName));
			}
		}

		/// <summary>
		/// Ensures the specified value is not negative float.
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		[DebuggerStepThrough]
		public static void IsNotNegative(float parameter, string parameterName)
		{
			if (parameter < 0)
			{
				throw new ArgumentOutOfRangeException(parameterName, ErrorTexts.CannotBeNegative.FormatWith(parameterName));
			}
		}

		/// <summary>
		/// Ensures the specified value is a virtual path which starts with ~/.
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		[DebuggerStepThrough]
		public static void IsNotVirtualPath(string parameter, string parameterName)
		{
			IsNotNullOrEmpty(parameter, parameterName);

			if (!parameter.StartsWith("~/", StringComparison.Ordinal))
			{
				throw new ArgumentException(ErrorTexts.SourceMustBeAVirtualPathWhichShouldStartsWithTileAndSlash, parameterName);
			}
		}
	}
}

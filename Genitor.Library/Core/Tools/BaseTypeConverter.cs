namespace Genitor.Library.Core.Tools
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;

	public static class BaseTypeConverter
	{
		/// <summary>
		/// Converts a value to a destination type.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="destinationType">The type to convert the value to.</param>
		/// <returns>The converted value.</returns>
		public static object To(this object value, Type destinationType)
		{
			return To(value, destinationType, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts a value to a destination type.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="destinationType">The type to convert the value to.</param>
		/// <param name="culture">Culture</param>
		/// <returns>The converted value.</returns>
		public static object To(this object value, Type destinationType, System.Globalization.CultureInfo culture)
		{
			if (value != null)
			{
				var sourceType = value.GetType();

				var destinationConverter = GetCustomTypeConverter(destinationType);
				var sourceConverter = GetCustomTypeConverter(sourceType);
				if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
				{
					return destinationConverter.ConvertFrom(null, culture, value);
				}

				if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
				{
					return sourceConverter.ConvertTo(null, culture, value, destinationType);
				}

				if (destinationType.IsEnum && value is int)
				{
					return Enum.ToObject(destinationType, (int)value);
				}

				if (!destinationType.IsAssignableFrom(value.GetType()))
				{
					return Convert.ChangeType(value, destinationType, culture);
				}
			}

			return value;
		}

		private static TypeConverter GetCustomTypeConverter(Type destinationType)
		{
			if (destinationType == typeof(List<int>))
			{
				return new GenericListTypeConverter<int>();
			}

			if (destinationType == typeof(List<decimal>))
			{
				return new GenericListTypeConverter<decimal>();
			}

			if (destinationType == typeof(List<string>))
			{
				return new GenericListTypeConverter<string>();
			}

			return TypeDescriptor.GetConverter(destinationType);
		}

		/// <summary>
		/// Converts a value to a destination type.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <typeparam name="T">The type to convert the value to.</typeparam>
		/// <returns>The converted value.</returns>
		public static T To<T>(this object value)
		{
			//// return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
			return (T)To(value, typeof(T));
		}
	}
}

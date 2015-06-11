//// ReSharper disable CheckNamespace
namespace Genitor.Library.Core
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;

	public static class GeneralExtensions
	{
		public static bool IsEmpty<T>(this T value)
		{
			return value.Equals(default(T));
		}

		/// <summary>
		/// Perform a deep Copy of the object.
		/// </summary>
		/// <typeparam name="T">The type of object being copied.</typeparam>
		/// <param name="source">The object instance to copy.</param>
		/// <returns>The copied object.</returns>
		public static T Clone<T>(this T source)
		{
			if (!typeof(T).IsSerializable)
			{
				throw new ArgumentException(@"The type must be serializable.", nameof(source));
			}

			// Don't serialize a null object, simply return the default for that object
			if (ReferenceEquals(source, null))
			{
				return default(T);
			}

			IFormatter formatter = new BinaryFormatter();
			Stream stream = new MemoryStream();
			using (stream)
			{
				formatter.Serialize(stream, source);
				stream.Seek(0, SeekOrigin.Begin);
				return (T)formatter.Deserialize(stream);
			}
		}

		public static IDictionary<string, object> ToDictionary(this object @object)
		{
			var dictionary = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);

			if (@object != null)
			{
				var enumerator = TypeDescriptor.GetProperties(@object).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						var propertyDescriptor = (PropertyDescriptor)enumerator.Current;
						dictionary.Add(propertyDescriptor.Name.Replace("_", "-"), propertyDescriptor.GetValue(@object));
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = enumerator as IDisposable) != null)
					{
						disposable.Dispose();
					}
				}
			}

			return dictionary;
		}
	}
}

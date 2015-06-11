//// ReSharper disable CheckNamespace
namespace Genitor.Library.Core
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

	/// <summary>
	/// Contains extension methods of ICollection&lt;T&gt;.
	/// </summary>
	[DebuggerStepThrough]
	public static class CollectionExtensions
	{
		/// <summary>
		/// Vrati dalsich count polozek z kolekce, s tim ze v pripade dosahnuti konce zacne vracet ty ze zacatku.
		/// </summary>
		public static IEnumerable<T> GetSequence<T>(this IList<T> col, int index, int count)
		{
			var total = col.Count();
			count = Math.Min(total, count);
			var twoQueues = new List<T>(col);
			twoQueues.InsertRange(total, col);

			if (index >= total)
			{
				index = -1;
			}

			return twoQueues.Skip(index + 1).Take(count);
		}

		/// <summary>
		/// Determines whether the specified collection instance is null or empty.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance">The instance to check.</param>
		/// <returns>
		/// <c>true</c> if the specified instance is null or empty; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullOrEmpty<T>(this ICollection<T> instance)
		{
			return (instance == null) || (instance.Count == 0);
		}

		/// <summary>
		/// Determines whether the specified enumeration instance is null or empty.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance">The instance to check.</param>
		/// <returns>
		/// <c>true</c> if the specified instance is null or empty; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> instance)
		{
			return instance == null || !instance.Any();
		}

		/// <summary>
		/// Determines whether the specified collection is empty.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance">The instance.</param>
		/// <returns>
		/// <c>true</c> if the specified instance is empty; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsEmpty<T>(this ICollection<T> instance)
		{
			Guard.IsNotNull(instance, "instance");

			return instance.Count == 0;
		}

		/// <summary>
		/// Determines whether the specified enumeration is empty.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance">The instance.</param>
		/// <returns>
		/// <c>true</c> if the specified instance is empty; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsEmpty<T>(this IEnumerable<T> instance)
		{
			Guard.IsNotNull(instance, "instance");

			return !instance.Any();
		}

		public static IEnumerable<T> AsNullIfEmpty<T>(this IList<T> items)
		{
			return items.IsNullOrEmpty() ? null : items;
		}

		/// <summary>
		/// Adds the specified elements to the end of the System.Collections.Generic.ICollection&lt;T&gt;.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance">The instance to add.</param>
		/// <param name="collection"> The collection whose elements should be added to the end of the ICollection&lt;T&gt;. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
		public static void AddRange<T>(this ICollection<T> instance, IEnumerable<T> collection)
		{
			Guard.IsNotNull(instance, "instance");
			Guard.IsNotNull(collection, "collection");

			foreach (var item in collection)
			{
				instance.Add(item);
			}
		}

		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
		{
			TValue value;
			return dictionary.TryGetValue(key, out value) ? value : defaultValue;
		}

		/// <summary>
		/// ForEach extension that enumerates over all items in an <see cref="IEnumerable{T}"/> and executes an action.
		/// </summary>
		/// <typeparam name="T">The type that this extension is applicable for.</typeparam>
		/// <param name="collection">The enumerable instance that this extension operates on.</param>
		/// <param name="action">The action executed for each iten in the enumerable.</param>
		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
		{
			if (collection == null)
			{
				return;
			}

			foreach (var item in collection)
			{
				action(item);
			}
		}

		/// <summary>
		/// ForEach extension that enumerates over all items in an <see cref="IEnumerator{T}"/> and executes an action.
		/// </summary>
		/// <typeparam name="T">The type that this extension is applicable for.</typeparam>
		/// <param name="collection">The enumerator instance that this extension operates on.</param>
		/// <param name="action">The action executed for each iten in the enumerable.</param>
		public static void ForEach<T>(this IEnumerator<T> collection, Action<T> action)
		{
			if (collection == null)
			{
				return;
			}

			while (collection.MoveNext())
			{
				action(collection.Current);
			}
		}

		/// <summary>
		/// For Each extension that enumerates over a enumerable collection and attempts to execute the provided action delegate and it the action throws an exception, continues enumerating.
		/// </summary>
		/// <typeparam name="T">The type that this extension is applicable for.</typeparam>
		/// <param name="enumerator">The IEnumerable instance that the extension operates on.</param>
		/// <param name="action">The action excecuted for each item in the enumerable.</param>
		public static void TryForEach<T>(this IEnumerable<T> enumerator, Action<T> action)
		{
			if (enumerator == null)
			{
				return;
			}

			foreach (var item in enumerator)
			{
				try
				{
					action(item);
				}
				catch
				{
					// ignored
				}
			}
		}

		/// <summary>
		/// For each extension that enumerates over an enumerator and attempts to execute the provided
		/// action delegate and if the action throws an exception, continues executing.
		/// </summary>
		/// <typeparam name="T">The type that this extension is applicable for.</typeparam>
		/// <param name="enumerator">The IEnumerator instace</param>
		/// <param name="action">The action executed for each item in the enumerator.</param>
		public static void TryForEach<T>(this IEnumerator<T> enumerator, Action<T> action)
		{
			if (enumerator == null)
			{
				return;
			}

			while (enumerator.MoveNext())
			{
				try
				{
					action(enumerator.Current);
				}
				catch
				{
					// ignored
				}
			}
		}
	}
}
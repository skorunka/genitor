namespace Genitor.Library.Core.Cache
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using depot;

	/// <summary>
	/// Depot cache service implementation.
	/// </summary>
	public class DepotCacheService : ICacheService
	{
		private readonly ICache _cache;
		private static readonly IList<string> Keys = new List<string>();
		private static readonly DateTime DefaultAbsoluteExpiration = DateTime.UtcNow.AddHours(4);

		#region ctor

		public DepotCacheService(ICache cache)
		{
			this._cache = cache;
		}

		#endregion

		public T Get<T>(string key, Func<T> select, DateTime? absoluteExpiration = null)
		{
			var o = this._cache.Get<T>(key);
			if (!Equals(o, default(T)))
			{
				return o;
			}

			//// predpokladame ze _cache je thread-safe, neni nutny lock, rep mohl by byt kvuli duplicitnimu volani setu... uvidime?
			o = @select.Invoke();
			if (absoluteExpiration == DateTime.MaxValue)
			{
				this._cache.Add(key, o);
			}
			else
			{
				this._cache.Add(key, o, absoluteExpiration ?? DefaultAbsoluteExpiration);
			}

			lock (Keys) Keys.Add(key);

			return o;
		}

		public void Remove(string key, bool exactMatch = false)
		{
			var keys = exactMatch ? new List<string> { key } : Keys.Where(x => x.Equals(key, StringComparison.InvariantCultureIgnoreCase) || x.StartsWith(key + ".", StringComparison.InvariantCultureIgnoreCase)).ToList();
			foreach (var k in keys)
			{
				this._cache.Remove<object>(key);
				lock (Keys)
				{
					Keys.Remove(k);
				}
			}
		}
	}
}
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
		private static readonly IList<string> _keys = new List<string>();

		private static readonly DateTime _sDefaultAbsoluteExpiration = DateTime.Now.AddHours(4);

		#region ctor

		public DepotCacheService(ICache cache)
		{
			_cache = cache;
		}

		#endregion

		public T Get<T>(string key, Func<T> select, DateTime? absoluteExpiration = null)
		{
			var o = _cache.Get<T>(key);
			if (Equals(o, default(T))) // neni v cache
			{
				//predpokladame ze _cache je thread-safe, neni nutny lock, rep mohl by byt kvuli duplicitnimu volani setu... uvidime?
				o = select.Invoke();
				if (absoluteExpiration == DateTime.MaxValue)
					_cache.Add(key, o);
				else
					_cache.Add(key, o, absoluteExpiration ?? _sDefaultAbsoluteExpiration);
				lock (_keys)
					_keys.Add(key);
			}
			return o;
		}

		public void Remove(string key, bool exactMatch = false)
		{
			var keys = exactMatch ? new List<string> { key } : _keys.Where(x => x.Equals(key, StringComparison.InvariantCultureIgnoreCase) || x.StartsWith(key + ".", StringComparison.InvariantCultureIgnoreCase)).ToList();
			foreach (var k in keys)
			{
				_cache.Remove<object>(key);
				lock (_keys)
					_keys.Remove(k);
			}
		}
	}
}
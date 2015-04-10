namespace Genitor.Library.Core.Cache
{
	using System;

	/// <summary>
	/// Generic cache interface.
	/// </summary>
	public interface ICacheService
	{
		T Get<T>(string key, Func<T> select, DateTime? absoluteExpiration = null);

		void Remove(string key, bool exactMatch = false);
	}
}

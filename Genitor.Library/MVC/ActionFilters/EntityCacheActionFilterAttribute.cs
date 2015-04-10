// ReSharper disable CheckNamespace
namespace Genitor.Library.MVC
// ReSharper restore CheckNamespace
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using Genitor.Library.Core;
	using Genitor.Library.Core.Cache;

	public class EntityCacheActionFilterAttribute : ActionFilterAttribute
	{
		private static readonly IList<string> ActionNames = new List<string> { "Add", "Insert", "Upsert", "Delete", "Remove" };
		private readonly ICacheService _cacheService;
		private IList<string> _keys;

		#region ctors

		public EntityCacheActionFilterAttribute()
			: this((string)null)
		{
		}

		public EntityCacheActionFilterAttribute(string key)
			: this(!string.IsNullOrEmpty(key) ? new[] { key } : null)
		{
		}

		#endregion

		public EntityCacheActionFilterAttribute(IList<string> keys)
		{
			this._keys = keys;
			this._cacheService = DependencyResolver.Current.GetService<ICacheService>();
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);

			if (this._keys == null || !this._keys.Any())
			{
				this._keys = new List<string> { GetEntityKey(filterContext) };
			}

			this._keys.ForEach(x => this._cacheService.Remove(x));
		}

		private static string GetEntityKey(ActionExecutingContext filterContext)
		{
			var entityKey = filterContext.ActionDescriptor.ActionName;

			ActionNames.ForEach(x => entityKey = entityKey.Replace(x, null));

			return entityKey;
		}
	}
}
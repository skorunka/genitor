namespace Genitor.Library.MVC.Attributes
{
	using System;
	using System.Web;
	using System.Web.Mvc;

	/// <summary>
	/// Prevent loading all pages from cache, i.e. to disable Back button browser functionality after SignOut.
	/// </summary>
	public class NoCacheAttribute : ActionFilterAttribute
	{
		/// <summary>
		/// OnResultExecuting override.
		/// </summary>
		/// <param name="filterContext">Context filter</param>
		public override void OnResultExecuting(ResultExecutingContext filterContext)
		{
			filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
			filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
			filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
			filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			filterContext.HttpContext.Response.Cache.SetNoStore();

			base.OnResultExecuting(filterContext);
		}
	}
}
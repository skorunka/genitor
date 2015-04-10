namespace Genitor.Library.MVC.Attributes
{
	using System;
	using System.Web;
	using System.Web.Mvc;
	using System.Web.Security;

	/// <summary>
	/// The authorize attribute with more complex authorization.
	/// </summary>
	public class AuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
	{
		public static string GetAuthenticationUrl(Uri requestUrl, string applicationPath)
		{
			var url = string.Format(
				"{0}://{1}{2}{3}",
				requestUrl.Scheme,
				requestUrl.Host,
				requestUrl.Port == 80 ? null : ":" + requestUrl.Port,
				applicationPath);

			return string.Format("{0}?ReturnUrl={1}", FormsAuthentication.LoginUrl, HttpUtility.UrlEncode(url));
		}

		/// <summary>
		/// Check if the request is Ajax. If so, then it returns special result to inform client about it.
		/// </summary>
		/// <param name="filterContext">
		/// The filter context.
		/// </param>
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			base.OnAuthorization(filterContext);
			if (filterContext.Result is HttpUnauthorizedResult && filterContext.HttpContext.Request.IsAjaxRequest())
			{
				filterContext.Result = AjaxRequestJsonResult.Error(
					"Not signed in.",
					new
						{
							SignInUrl = GetAuthenticationUrl(filterContext.HttpContext.Request.Url, filterContext.HttpContext.Request.ApplicationPath)
						});
			}
		}
	}
}
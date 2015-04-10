namespace Genitor.Library.MVC.Attributes
{
	using System;
	using System.Web.Mvc;

	/// <summary>
	/// Ajax only request attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class AjaxOnlyAttribute : ActionFilterAttribute
	{
		/// <summary>
		/// Overrides OnActionExecuting to prevent non Ajax request calls.
		/// </summary>
		/// <param name="filterContext">Filter context.</param>
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (!filterContext.HttpContext.Request.IsAjaxRequest())
			{
				filterContext.Result = new HttpNotFoundResult();
			}

			base.OnActionExecuting(filterContext);
		}
	}
}
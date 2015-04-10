namespace Genitor.Library.MVC.Attributes
{
	using System.Linq;
	using System.Web.Mvc;

	/// <summary>
	/// The export model state to temp data.
	/// </summary>
	public class ExportModelStateToTempDataAttribute : ModelStateTempDataTransferAttribute
	{
		/// <summary>
		/// Overrides default OnActionExecuting method.
		/// </summary>
		/// <param name="filterContext">The filter context.</param>
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			//// Support for HtmlHelperExtensions.NamedValidationSummary().
			var formModel = filterContext.ActionParameters.FirstOrDefault(x => x.Value is IFormViewModel);
			if (formModel.Value != null)
			{
				filterContext.Controller.TempData[HtmlHelperExtensions.SubmittedFormName] = ((IFormViewModel)formModel.Value).FormName;
			}

			base.OnActionExecuting(filterContext);
		}

		/// <summary>
		/// Overrides default OnActionExecuted method.
		/// </summary>
		/// <param name="filterContext">The filter context.</param>
		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			// Only export when ModelState is not valid
			if (!filterContext.Controller.ViewData.ModelState.IsValid)
			{
				// Export if we are redirecting
				if ((filterContext.Result is RedirectResult) || (filterContext.Result is RedirectToRouteResult))
				{
					filterContext.Controller.TempData[ModelStateTempDataTransferAttribute.Key] = filterContext.Controller.ViewData.ModelState;
				}
			}

			base.OnActionExecuted(filterContext);
		}
	}
}
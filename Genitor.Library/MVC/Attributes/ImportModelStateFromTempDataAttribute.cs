namespace Genitor.Library.MVC.Attributes
{
	using System.Linq;
	using System.Web.Mvc;

	/// <summary>
	/// The import model state from temp data.
	/// </summary>
	public class ImportModelStateFromTempDataAttribute : ModelStateTempDataTransferAttribute
	{
		/// <summary>
		/// Gets or sets a value indicating whether to import just invalid states.
		/// </summary>
		public bool ImportJustInvalidStates { get; set; }

		/// <summary>
		/// Overrides default OnActionExecuted method.
		/// </summary>
		/// <param name="filterContext">The filter context.</param>
		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			//// Support for HtmlHelperExtensions.NamedValidationSummary().
			filterContext.Controller.ViewData[HtmlHelperExtensions.SubmittedFormName] = filterContext.Controller.TempData[HtmlHelperExtensions.SubmittedFormName];

			var modelState = filterContext.Controller.TempData[Key] as ModelStateDictionary;

			if (modelState != null)
			{
				// Only Import if we are viewing
				if (filterContext.Result is ViewResult)
				{
					if (this.ImportJustInvalidStates)
					{
						var invalidModelStates = new ModelStateDictionary();
						foreach (var ms in modelState.Where(x => x.Value.Errors.Any()))
						{
							invalidModelStates.Add(ms);
						}

						modelState = invalidModelStates;
					}

					filterContext.Controller.ViewData.ModelState.Merge(modelState);
				}
				else
				{
					// Otherwise remove it.
					filterContext.Controller.TempData.Remove(Key);
				}
			}

			base.OnActionExecuted(filterContext);
		}
	}
}
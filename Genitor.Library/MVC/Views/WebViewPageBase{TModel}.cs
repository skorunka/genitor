namespace Genitor.Library.MVC.Views
{
	using System.Web.Mvc;

	using Genitor.Library.Core.Localization;

	public abstract class WebViewPageBase<TModel> : WebViewPage<TModel>
	{
		protected string ActionName
		{
			get
			{
				return this.ViewContext.RouteData.Values["action"] as string;
			}
		}

		protected string ControllerName
		{
			get
			{
				return this.ViewContext.RouteData.Values["controller"] as string;
			}
		}

		protected Localizer T { get; private set; }

		public override void InitHelpers()
		{
			base.InitHelpers();

			this.T = DependencyResolver.Current.GetService<IText>().Get;
		}

		protected override void SetViewData(ViewDataDictionary viewData)
		{
			//// We use this to override TemplateInfo.HtmlFieldPrefix for rendering Editors etc.
			if (viewData["HtmlFieldPrefix"] != null)
			{
				viewData.TemplateInfo.HtmlFieldPrefix = viewData["HtmlFieldPrefix"] as string;
			}

			base.SetViewData(viewData);
		}
	}
}
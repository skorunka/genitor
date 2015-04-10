// ReSharper disable CheckNamespace
namespace Genitor.Library.MVC
// ReSharper restore CheckNamespace
{
	using System.Web.Mvc;

	using Microsoft.Web.WebPages.OAuth;

	public class ExternalSignInActionResult : ActionResult
	{
		public ExternalSignInActionResult(string providerName, string returnUrl)
		{
			this.ProviderName = providerName;
			this.ReturnUrl = returnUrl;
		}

		public string ProviderName { get; private set; }

		public string ReturnUrl { get; private set; }

		public override void ExecuteResult(ControllerContext context)
		{
			OAuthWebSecurity.RequestAuthentication(this.ProviderName, this.ReturnUrl);
		}
	}
}

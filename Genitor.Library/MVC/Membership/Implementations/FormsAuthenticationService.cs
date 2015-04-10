namespace Genitor.Library.MVC.Membership.Implementations
{
	using System;
	using System.Web;
	using System.Web.Security;

	using Genitor.Library.Core;

	using JetBrains.Annotations;

	[UsedImplicitly]
	public class FormsAuthenticationService : IFormsAuthenticationService
	{
		public void SignIn(string userName, bool createPersistentCookie)
		{
			Guard.IsNotNull(userName, "userName");

			FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
		}

		public bool SignIn(string userName, string password, bool createPersistentCookie)
		{
			throw new NotSupportedException("Use \"SignIn(string userName, bool createPersistentCookie)\" override.");
		}

		public void SignOut()
		{
			if (HttpContext.Current != null && HttpContext.Current.Session != null)
			{
				HttpContext.Current.Session.Abandon();
			}

			FormsAuthentication.SignOut();
		}
	}
}
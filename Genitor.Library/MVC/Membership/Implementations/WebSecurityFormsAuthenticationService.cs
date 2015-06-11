//namespace Genitor.Library.MVC.Membership.Implementations
//{
//	using System;

//	using JetBrains.Annotations;

//	using WebMatrix.WebData;

//	/// <summary>
//	/// Uses <see cref="WebSecurity"/>.
//	/// </summary>
//	[UsedImplicitly]
//	public class WebSecurityFormsAuthenticationService : IFormsAuthenticationService
//	{
//		public void SignIn(string userName, bool createPersistentCookie)
//		{
//			throw new NotSupportedException("Use \"SignIn(string userName, string password, bool createPersistentCookie)\" override.");
//		}

//		public bool SignIn(string userName, string password, bool createPersistentCookie)
//		{
//			return WebSecurity.Login(userName, password, createPersistentCookie);
//		}

//		public void SignOut()
//		{
//			WebSecurity.Logout();
//		}
//	}
//}

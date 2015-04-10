namespace Genitor.Library.MVC.Membership
{
	public interface IFormsAuthenticationService
	{
		void SignIn(string userName, bool createPersistentCookie);

		bool SignIn(string userName, string password, bool createPersistentCookie);

		void SignOut();
	}
}
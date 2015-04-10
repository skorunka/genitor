namespace Genitor.Library.MVC.Membership
{
	using System.Web.Security;

	public static class MembershipCreateStatusExtensions
	{
		/// <summary>
		/// Returns localized error text for enum state.
		/// </summary>
		public static string ErrorText(this MembershipCreateStatus createStatus)
		{
			// See http://go.microsoft.com/fwlink/?LinkID=177550 for a full list of status codes.
			switch (createStatus)
			{
				case MembershipCreateStatus.DuplicateUserName:
					return Resources.MembershipCreateStatusTexts.DuplicateUserName;

				case MembershipCreateStatus.DuplicateEmail:
					return Resources.MembershipCreateStatusTexts.DuplicateEmail;

				case MembershipCreateStatus.InvalidPassword:
					return Resources.MembershipCreateStatusTexts.InvalidPassword;

				case MembershipCreateStatus.InvalidEmail:
					return Resources.MembershipCreateStatusTexts.InvalidEmail;

				case MembershipCreateStatus.InvalidAnswer:
					return Resources.MembershipCreateStatusTexts.InvalidAnswer;

				case MembershipCreateStatus.InvalidQuestion:
					return Resources.MembershipCreateStatusTexts.InvalidQuestion;

				case MembershipCreateStatus.InvalidUserName:
					return Resources.MembershipCreateStatusTexts.InvalidUserName;

				case MembershipCreateStatus.ProviderError:
					return Resources.MembershipCreateStatusTexts.ProviderError;

				case MembershipCreateStatus.UserRejected:
					return Resources.MembershipCreateStatusTexts.UserRejected;

				default:
					return Resources.MembershipCreateStatusTexts.Default;
			}
		}
	}
}
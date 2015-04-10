namespace Genitor.Library.MVC.Membership
{
	using System.Web.Security;

	public interface IMembershipService
	{
		int MinPasswordLength { get; }

		bool PasswordResetEnabled { get; }

		bool RequiresQuestionAndAnswer { get; }

		bool ValidateUser(string userName, string password);

		MembershipCreateStatus CreateUser(string userName, string password, string email);

		bool ChangePassword(string userName, string oldPassword, string newPassword);

		string ResetPassword(string userName, string answer = null);

		string GetResetPasswordToken(string userName);

		bool ResetPasswordByToken(string token, string newPassword);

		MembershipUser GetUser(string userName, bool isOnline);

		bool DeleteUser(string userName, bool deleteAllRelatedData = true);

		int GetNumberOfUsersOnline();
	}
}
namespace Genitor.Library.MVC.Membership.Implementations
{
	using System;
	using System.Diagnostics;
	using System.Web.Security;

	using Genitor.Library.Core;

	using JetBrains.Annotations;

	using WebMatrix.WebData;

	/// <summary>
	/// Uses <see cref="WebSecurity"/>.
	/// </summary>
	[UsedImplicitly]
	public class WebSecurityMembershipService : IMembershipService
	{
		private readonly MembershipProvider _provider;

		#region ctors

		public WebSecurityMembershipService(MembershipProvider provider)
		{
			Guard.IsNotNull(provider, "provider");

			this._provider = provider;
		}

		#endregion

		public int MinPasswordLength
		{
			get { return this._provider.MinRequiredPasswordLength; }
		}

		public bool PasswordResetEnabled
		{
			get { return this._provider.EnablePasswordReset; }
		}

		public bool RequiresQuestionAndAnswer
		{
			get { return this._provider.RequiresQuestionAndAnswer; }
		}

		public bool ValidateUser(string userName, string password)
		{
			Guard.IsNotNull(userName, "userName");
			Guard.IsNotNull(password, "password");

			return this._provider.ValidateUser(userName, password);
		}

		public MembershipCreateStatus CreateUser(string userName, string password, string email)
		{
			Guard.IsNotNull(userName, "userName");
			Guard.IsNotNull(password, "password");
			Guard.IsNotNull(email, "email");

			try
			{
				WebSecurity.CreateUserAndAccount(userName, password, new { email });
			}
			catch (Exception e)
			{
				Trace.TraceError(e.ToString());

				return MembershipCreateStatus.ProviderError;
			}

			return MembershipCreateStatus.Success;
		}

		public bool ChangePassword(string userName, string oldPassword, string newPassword)
		{
			Guard.IsNotNull(userName, "userName");
			Guard.IsNotNull(oldPassword, "oldPassword");
			Guard.IsNotNull(newPassword, "newPassword");

			return WebSecurity.ChangePassword(userName, oldPassword, newPassword);
		}

		public string ResetPassword(string userName, string answer = null)
		{
			throw new NotSupportedException("Use \"GetResetPasswordToken\" and \"ResetPasswordByToken\" instead.");
		}

		public string GetResetPasswordToken(string userName)
		{
			Guard.IsNotNull(userName, "userName");

			return WebSecurity.GeneratePasswordResetToken(userName);
		}

		public bool ResetPasswordByToken(string token, string newPassword)
		{
			return WebSecurity.ResetPassword(token, newPassword);
		}

		public MembershipUser GetUser(string userName, bool isOnline)
		{
			Guard.IsNotNull(userName, "userName");

			return this._provider.GetUser(userName, isOnline);
		}

		public bool DeleteUser(string userName, bool deleteAllRelatedData = true)
		{
			Guard.IsNotNull(userName, "userName");

			return this._provider.DeleteUser(userName, deleteAllRelatedData);
		}

		public int GetNumberOfUsersOnline()
		{
			return this._provider.GetNumberOfUsersOnline();
		}
	}
}

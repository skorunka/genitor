namespace Genitor.Library.MVC.Membership.Implementations
{
	using System;
	using System.Web.Security;

	using Genitor.Library.Core;

	using JetBrains.Annotations;

	[UsedImplicitly]
	public class MembershipService : IMembershipService
	{
		private readonly MembershipProvider _provider;

		#region ctors

		public MembershipService(MembershipProvider provider)
		{
			Guard.IsNotNull(provider, "provider");

			this._provider = provider;
		}

		#endregion

		public int MinPasswordLength
		{
			get
			{
				return this._provider.MinRequiredPasswordLength;
			}
		}

		public bool PasswordResetEnabled
		{
			get
			{
				return this._provider.EnablePasswordReset;
			}
		}

		public bool RequiresQuestionAndAnswer
		{
			get
			{
				return this._provider.RequiresQuestionAndAnswer;
			}
		}

		public bool ValidateUser(string userName, string password)
		{
			Guard.IsNotNull(userName, "userName");
			Guard.IsNotNull(userName, "password");

			return this._provider.ValidateUser(userName, password);
		}

		public MembershipCreateStatus CreateUser(string userName, string password, string email)
		{
			Guard.IsNotNull(userName, "userName");
			Guard.IsNotNull(userName, "password");
			Guard.IsNotNull(userName, "email");

			MembershipCreateStatus status;
			this._provider.CreateUser(userName, password, email, null, null, true, null, out status);
			return status;
		}

		public bool ChangePassword(string userName, string oldPassword, string newPassword)
		{
			Guard.IsNotNull(userName, "userName");
			Guard.IsNotNull(userName, "oldPassword");
			Guard.IsNotNull(userName, "newPassword");

			// The underlying ChangePassword() will throw an exception rather
			// than return false in certain failure scenarios.
			try
			{
				var currentUser = this._provider.GetUser(userName, true /* userIsOnline */);
				return currentUser != null && currentUser.ChangePassword(oldPassword, newPassword);
			}
			catch (ArgumentException)
			{
				return false;
			}
			catch (MembershipPasswordException)
			{
				return false;
			}
		}

		public string ResetPassword(string userName, string answer = null)
		{
			Guard.IsNotNull(userName, "userName");

			return this._provider.ResetPassword(userName, answer);
		}

		public string GetResetPasswordToken(string userName)
		{
			throw new NotSupportedException("This method is not supported by this \"MembershipProvider\".");
		}

		public bool ResetPasswordByToken(string token, string newPassword)
		{
			throw new NotSupportedException("This method is not supported by this \"MembershipProvider\".");
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
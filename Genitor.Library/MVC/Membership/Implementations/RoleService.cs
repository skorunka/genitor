namespace Genitor.Library.MVC.Membership.Implementations
{
	using System.Collections.Generic;
	using System.Web.Security;

	using Genitor.Library.Core;

	using JetBrains.Annotations;

	[UsedImplicitly]
	public class RoleService : IRoleService
	{
		private readonly RoleProvider roleProvider;

		public RoleService(RoleProvider roleProvider)
		{
			Guard.IsNotNull(roleProvider, "roleProvider");

			this.roleProvider = roleProvider;
		}

		public void CreateRole(string roleName, bool checkIfExists = true)
		{
			if (!checkIfExists || !this.RoleExists(roleName))
			{
				this.roleProvider.CreateRole(roleName);
			}
		}

		public bool RoleExists(string roleName)
		{
			return this.roleProvider.RoleExists(roleName);
		}

		public void AddUserToRole(string userName, string roleName)
		{
			this.roleProvider.AddUsersToRoles(new[] { userName }, new[] { roleName });
		}

		public void RemoveUserFromRole(string userName, string roleName)
		{
			this.roleProvider.RemoveUsersFromRoles(new[] { userName }, new[] { roleName });
		}

		public ICollection<string> GetUsersInRole(string roleName)
		{
			return this.roleProvider.GetUsersInRole(roleName);
		}

		public bool IsUserInRole(string userName, string roleName)
		{
			return this.roleProvider.IsUserInRole(userName, roleName);
		}

		public IList<string> GetRolesForUser(string userName)
		{
			return this.roleProvider.GetRolesForUser(userName);
		}
	}
}
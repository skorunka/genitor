namespace Genitor.Library.MVC.Membership
{
	using System.Collections.Generic;

	public interface IRoleService
	{
		void CreateRole(string roleName, bool checkIfExists = true);

		bool RoleExists(string roleName);

		void AddUserToRole(string userName, string roleName);

		void RemoveUserFromRole(string userName, string roleName);

		ICollection<string> GetUsersInRole(string roleName);

		bool IsUserInRole(string userName, string roleName);

		IList<string> GetRolesForUser(string userName);
	}
}
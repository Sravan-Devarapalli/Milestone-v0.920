using System.ServiceModel;

namespace PracticeManagementService
{
	[ServiceContract]
	public interface IRoleService
	{
		/// <summary>
		/// Adds the specified user names to the specified roles for the configured applicationName.
		/// </summary>
		/// <param name="usernames">A string array of user names to be added to the specified roles.</param>
		/// <param name="roleNames">A string array of the role names to add the specified user names to.</param>
		[OperationContract]
		void AddUsersToRoles(string[] usernames, string[] roleNames);

		/// <summary>
		/// Gets the name of the application to store and retrieve role information for.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		string GetApplicationName();

		/// <summary>
		/// Sets the name of the application to store and retrieve role information for.
		/// </summary>
		/// <param name="applicationName"></param>
		[OperationContract]
		void SetApplicationName(string applicationName);

		/// <summary>
		/// Adds a new role to the data source for the configured applicationName.
		/// </summary>
		/// <param name="roleName">The name of the role to create.</param>
		[OperationContract]
		void CreateRole(string roleName);

		/// <summary>
		/// Removes a role from the data source for the configured applicationName.
		/// </summary>
		/// <param name="roleName">The name of the role to delete.</param>
		/// <param name="throwOnPopulatedRole">
		/// If true, throw an exception if roleName has one or more members and do not delete roleName.
		/// </param>
		/// <returns>true if the role was successfully deleted; otherwise, false.</returns>
		[OperationContract]
		bool DeleteRole(string roleName, bool throwOnPopulatedRole);

		/// <summary>
		/// Gets an array of user names in a role where the user name contains the specified user name to match.
		/// </summary>
		/// <param name="roleName">The role to search in.</param>
		/// <param name="usernameToMatch">The user name to search for.</param>
		/// <returns>
		/// A string array containing the names of all the users where the user name matches usernameToMatch
		/// and the user is a member of the specified role.
		/// </returns>
		[OperationContract]
		string[] FindUsersInRole(string roleName, string usernameToMatch);

		/// <summary>
		/// Gets a list of all the roles for the configured applicationName.
		/// </summary>
		/// <returns>
		/// A string array containing the names of all the roles stored in the data source for the configured
		/// applicationName.
		/// </returns>
		[OperationContract]
		string[] GetAllRoles();

		/// <summary>
		/// Gets a list of the roles that a specified user is in for the configured applicationName.
		/// </summary>
		/// <param name="username">The user to return a list of roles for.</param>
		/// <returns>
		/// A string array containing the names of all the roles that the specified user is in for the configured
		/// applicationName.
		/// </returns>
		[OperationContract]
		string[] GetRolesForUser(string username);

		/// <summary>
		/// Gets a list of users in the specified role for the configured applicationName.
		/// </summary>
		/// <param name="roleName">The name of the role to get the list of users for.</param>
		/// <returns>
		/// A string array containing the names of all the users who are members of the specified role for the
		/// configured applicationName.
		/// </returns>
		[OperationContract]
		string[] GetUsersInRole(string roleName);

		/// <summary>
		/// Gets a value indicating whether the specified user is in the specified role for the configured
		/// applicationName.
		/// </summary>
		/// <param name="username">The user name to search for.</param>
		/// <param name="roleName">The role to search in.</param>
		/// <returns>
		/// true if the specified user is in the specified role for the configured applicationName;
		/// otherwise, false
		/// </returns>
		[OperationContract]
		bool IsUserInRole(string username, string roleName);

		/// <summary>
		/// Removes the specified user names from the specified roles for the configured applicationName.
		/// </summary>
		/// <param name="usernames">A string array of user names to be removed from the specified roles.</param>
		/// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
		[OperationContract]
		void RemoveUsersFromRoles(string[] usernames, string[] roleNames);

		/// <summary>
		/// Gets a value indicating whether the specified role name already exists in the role data source for
		/// the configured applicationName.
		/// </summary>
		/// <param name="roleName">The name of the role to search for in the data source.</param>
		/// <returns>
		/// true if the role name already exists in the data source for the configured applicationName;
		/// otherwise, false.
		/// </returns>
		[OperationContract]
		bool RoleExists(string roleName);
	}
}


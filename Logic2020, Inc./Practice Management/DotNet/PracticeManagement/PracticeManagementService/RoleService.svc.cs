using System.ServiceModel.Activation;
using System.Web.Security;

namespace PracticeManagementService
{
    /// <summary>
    /// Provides an access to the Role Manager.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RoleService : IRoleService
    {
        #region Methods

        /// <summary>
        /// Adds the specified user names to the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="usernames">A string array of user names to be added to the specified roles.</param>
        /// <param name="roleNames">A string array of the role names to add the specified user names to.</param>
        public void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            Roles.AddUsersToRoles(usernames, roleNames);
        }

        /// <summary>
        /// Gets the name of the application to store and retrieve role information for.
        /// </summary>
        /// <returns></returns>
        public string GetApplicationName()
        {
            return Roles.ApplicationName;
        }

        /// <summary>
        /// Sets the name of the application to store and retrieve role information for.
        /// </summary>
        /// <param name="applicationName"></param>
        public void SetApplicationName(string applicationName)
        {
            Roles.ApplicationName = applicationName;
        }

        /// <summary>
        /// Adds a new role to the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to create.</param>
        public void CreateRole(string roleName)
        {
            Roles.CreateRole(roleName);
        }

        /// <summary>
        /// Removes a role from the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to delete.</param>
        /// <param name="throwOnPopulatedRole">
        /// If true, throw an exception if roleName has one or more members and do not delete roleName.
        /// </param>
        /// <returns>true if the role was successfully deleted; otherwise, false.</returns>
        public bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            return Roles.DeleteRole(roleName, throwOnPopulatedRole);
        }

        /// <summary>
        /// Gets an array of user names in a role where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="roleName">The role to search in.</param>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <returns>
        /// A string array containing the names of all the users where the user name matches usernameToMatch
        /// and the user is a member of the specified role.
        /// </returns>
        public string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return Roles.FindUsersInRole(roleName, usernameToMatch);
        }

        /// <summary>
        /// Gets a list of all the roles for the configured applicationName.
        /// </summary>
        /// <returns>
        /// A string array containing the names of all the roles stored in the data source for the configured
        /// applicationName.
        /// </returns>
        public string[] GetAllRoles()
        {
            return Roles.GetAllRoles();
        }

        /// <summary>
        /// Gets a list of the roles that a specified user is in for the configured applicationName.
        /// </summary>
        /// <param name="username">The user to return a list of roles for.</param>
        /// <returns>
        /// A string array containing the names of all the roles that the specified user is in for the configured
        /// applicationName.
        /// </returns>
        public string[] GetRolesForUser(string username)
        {
            return Roles.GetRolesForUser(username);
        }

        /// <summary>
        /// Gets a list of users in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to get the list of users for.</param>
        /// <returns>
        /// A string array containing the names of all the users who are members of the specified role for the
        /// configured applicationName.
        /// </returns>
        public string[] GetUsersInRole(string roleName)
        {
            return Roles.GetUsersInRole(roleName);
        }

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
        public bool IsUserInRole(string username, string roleName)
        {
            return Roles.IsUserInRole(username, roleName);
        }

        /// <summary>
        /// Removes the specified user names from the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="usernames">A string array of user names to be removed from the specified roles.</param>
        /// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
        public void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            Roles.RemoveUsersFromRoles(usernames, roleNames);
        }

        /// <summary>
        /// Gets a value indicating whether the specified role name already exists in the role data source for
        /// the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to search for in the data source.</param>
        /// <returns>
        /// true if the role name already exists in the data source for the configured applicationName;
        /// otherwise, false.
        /// </returns>
        public bool RoleExists(string roleName)
        {
            return Roles.RoleExists(roleName);
        }

        #endregion

    }
}


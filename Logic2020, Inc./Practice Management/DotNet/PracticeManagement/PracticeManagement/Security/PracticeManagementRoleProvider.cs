using System;
using System.Collections.Generic;

namespace PraticeManagement.Security
{
	/// <summary>
	/// Provides a Roles management functionality working through the WCF Services.
	/// Adds caching to the base functionality
	/// </summary>
	public class PracticeManagementRoleProvider : PracticeManagementRoleProviderBase
	{
        #region Fields

        private static readonly Dictionary<string, string[]> 
            UserInRoles = new Dictionary<string, string[]>();

        private static readonly Object ThisLock = new Object();

        #endregion

        /// <summary>
        /// Clears cache
        /// </summary>
        public void ClearCache()
        {
            lock (ThisLock)
            {
                UserInRoles.Clear();
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            ClearCache();
            base.AddUsersToRoles(usernames, roleNames);
        }

        public override void CreateRole(string roleName)
        {
            ClearCache();
            base.CreateRole(roleName);
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            ClearCache();
            return base.DeleteRole(roleName, throwOnPopulatedRole);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            ClearCache();
            base.RemoveUsersFromRoles(usernames, roleNames);
        }

        public override string[] GetRolesForUser(string username)
        {
            string[] rolesForUser;
            lock (ThisLock)
            {
                if (UserInRoles.ContainsKey(username))
                    rolesForUser = UserInRoles[username];
                else
                {
                    rolesForUser = base.GetRolesForUser(username);
                    UserInRoles.Add(username, rolesForUser);
                }
            }

            return rolesForUser;
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            lock (ThisLock)
            {
                if (UserInRoles.ContainsKey(username))
                    return Array.IndexOf(UserInRoles[username], roleName) >= 0;                
            }

            return base.IsUserInRole(username, roleName);
        }
	}
}


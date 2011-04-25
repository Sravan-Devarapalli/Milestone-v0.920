using System.Collections.Generic;
using System.ServiceModel.Activation;

using DataAccess;
using DataTransferObjects;

namespace PracticeManagementService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ProjectGroupService : IProjectGroupService
	{
		#region IProjectGroupService Members

        /// <summary>
        /// Retrives the list groups available for the specific client or project.
        /// </summary>
        /// <param name="clientId">An ID of the client to retrive the data for</param>
        /// <param name="projectId">An ID of the project to retrive the data for</param>
        /// <returns>The list of the <see cref="Project"/> objects.</returns>
        public List<ProjectGroup> GroupListAll(int? clientId, int? projectId)
        {
            var result = ProjectGroupDAL.GroupListAll(clientId, projectId);
            return result;
        }

        /// <summary>
        /// Rename group
        /// </summary>
        /// <param name="clientId">An ID of the client to retrive group</param>
        /// <param name="oldGroupName">Original name of the group</param>
        /// <param name="newGroupName">Name that will be assign to the group</param>
        /// <returns>True for successfull renaming</returns>
        public  bool UpDateProductGroup(int clientId,int groupId, string groupName, bool isActive)
        {
            var result = ProjectGroupDAL.UpDateProductGroup(clientId,groupId, groupName, isActive);
            return result;
        }

        /// <summary>
        /// Add group
        /// </summary>
        /// <param name="clientId">An ID of the client to create group</param>
        /// <param name="groupName">Name of new group</param>
        /// <returns>Uniq Id created group in DB</returns>
        public int ProjectGroupInsert(int clientId, string groupName, bool isActive)
        {
            var result = ProjectGroupDAL.ProjectGroupInsert(clientId, groupName, isActive);
            return result;
        }

        /// <summary>
        /// Delete project group
        /// </summary>
        /// <param name="groupId">An ID of the group to delete</param>
        /// <returns>True for successfull renaming</returns>
        public bool ProjectGroupDelete(int groupId)
        {
            var result = ProjectGroupDAL.ProjectGroupDelete(groupId);
            return result;
        }

        #endregion
	}
}


using System.Collections.Generic;
using System.ServiceModel;
using DataTransferObjects;

namespace PracticeManagementService
{
    [ServiceContract(Namespace = "http://www.logic2020.com")]
	public interface IProjectGroupService
	{
        /// <summary>
        /// Retrives the list groups available for the specific client or project.
        /// </summary>
        /// <param name="clientId">An ID of the client to retrive the data for</param>
        /// <param name="projectId">An ID of the project to retrive the data for</param>
        /// <returns>The list of the <see cref="Project"/> objects.</returns>
        [OperationContract]
        List<ProjectGroup> GroupListAll(int? clientId, int? projectId);

        /// <summary>
        /// Rename group
        /// </summary>
        /// <param name="clientId">An ID of the client to retrive group</param>
        /// <param name="oldGroupName">Original name of the group</param>
        /// <param name="newGroupName">Name that will be assign to the group</param>
        /// <returns>true for successfull renaming</returns>
        [OperationContract]
        bool UpDateProductGroup(int clientId,int groupId, string groupName, bool isActive);

        /// <summary>
        /// Add group
        /// </summary>
        /// <param name="clientId">An ID of the client to create group</param>
        /// <param name="groupName">Name of new group</param>
        /// <returns>Uniq Id created group in DB</returns>
        [OperationContract]
        int ProjectGroupInsert(int clientId, string groupName,bool isActive);

        /// <summary>
        /// Delete project group
        /// </summary>
        /// <param name="groupId">An ID of the group to delete</param>
        /// <returns>True for successfull renaming</returns>
        [OperationContract]
        bool ProjectGroupDelete(int groupId);
    }
}


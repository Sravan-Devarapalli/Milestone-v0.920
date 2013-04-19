using System.Collections.Generic;
using System.ServiceModel.Activation;

using DataAccess;
using DataTransferObjects;
using System;

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

        public List<ProjectGroup> ListGroupByClientAndPersonInPeriod(int clientId, int personId, DateTime startDate, DateTime endDate)
        {
            var result = ProjectGroupDAL.ListGroupByClientAndPersonInPeriod(clientId, personId, startDate,  endDate);
            return result;
        }

        /// <summary>
        /// Rename group
        /// </summary>
        /// <param name="clientId">An ID of the client to retrive group</param>
        /// <param name="oldGroupName">Original name of the group</param>
        /// <param name="newGroupName">Name that will be assign to the group</param>
        /// <returns>True for successfully renaming</returns>
        public  bool ProjectGroupUpdate(ProjectGroup projectGroup,string userLogin)
        {
            return ProjectGroupDAL.ProjectGroupUpdate(projectGroup, userLogin);
        }

        /// <summary>
        /// Add group
        /// </summary>
        /// <param name="clientId">An ID of the client to create group</param>
        /// <param name="groupName">Name of new group</param>
        /// <returns>Unique Id created group in DB</returns>
        public int ProjectGroupInsert(ProjectGroup projectGroup, string userLogin)
        {
           return ProjectGroupDAL.ProjectGroupInsert(projectGroup, userLogin);
        }

        /// <summary>
        /// Delete project group
        /// </summary>
        /// <param name="groupId">An ID of the group to delete</param>
        /// <returns>True for successfully renaming</returns>
        public bool ProjectGroupDelete(int groupId, string userLogin)
        {
            return ProjectGroupDAL.ProjectGroupDelete(groupId, userLogin);
        }

        public List<ProjectGroup> GetInternalBusinessUnits()
        {
            return ProjectGroupDAL.GetInternalBusinessUnits();
        }

        public void BusinessGroupUpdate(BusinessGroup businessGroup, string userLogin)
        {
            ProjectGroupDAL.BusinessGroupUpdate(businessGroup, userLogin);
        }

        public int BusinessGroupInsert(BusinessGroup businessGroup, string userLogin)
        {
           return  ProjectGroupDAL.BusinessGroupInsert(businessGroup, userLogin);
        }

        public void BusinessGroupDelete(int businessGroupId, string userLogin)
        {
            ProjectGroupDAL.BusinessGroupDelete(businessGroupId, userLogin);
        }

        public List<BusinessGroup> GetBusinessGroupList(int? clientId, int? businessUnitId)
        {
            return ProjectGroupDAL.GetBusinessGroupList(clientId, businessUnitId);
        }

        #endregion
	}
}


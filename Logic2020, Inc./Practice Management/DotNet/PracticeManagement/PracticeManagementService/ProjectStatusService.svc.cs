using System.Collections.Generic;
using System.ServiceModel.Activation;
using DataAccess;
using DataTransferObjects;

namespace PracticeManagementService
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class ProjectStatusService : IProjectStatusService
	{
		#region IProjectStatusService Members

		/// <summary>
		/// Retrives the list of the project statuses.
		/// </summary>
		/// <returns>The list of the <see cref="ProjectStatus"/> objects.</returns>
		public List<ProjectStatus> GetProjectStatuses()
		{
			return ProjectStatusDAL.ProjectStatusListAll();
		}

		#endregion
	}
}


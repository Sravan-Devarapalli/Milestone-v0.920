using System.Collections.Generic;
using System.ServiceModel;

using DataTransferObjects;

namespace PracticeManagementService
{
	[ServiceContract]
	public interface IProjectStatusService
	{
		/// <summary>
		/// Retrives the list of the project statuses.
		/// </summary>
		/// <returns>The list of the <see cref="ProjectStatus"/> objects.</returns>
		[OperationContract]
		List<ProjectStatus> GetProjectStatuses();
	}
}


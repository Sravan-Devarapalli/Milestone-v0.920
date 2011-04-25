using System.Collections.Generic;
using System.ServiceModel;

using DataTransferObjects;

namespace PracticeManagementService
{
	[ServiceContract]
	public interface IPersonStatusService
	{
		/// <summary>
		/// Retrives the list of the person statuses.
		/// </summary>
		/// <returns>The list of the <see cref="PersonStatus"/> objects.</returns>
		[OperationContract]
		List<PersonStatus> GetPersonStatuses();
	}
}
	

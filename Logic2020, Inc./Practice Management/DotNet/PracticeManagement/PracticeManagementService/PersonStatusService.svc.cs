using System.Collections.Generic;
using System.ServiceModel.Activation;
using DataAccess;
using DataTransferObjects;

namespace PracticeManagementService
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class PersonStatusService : IPersonStatusService
	{
		#region IPersonStatusService Members

		/// <summary>
		/// Retrives the list of the person statuses.
		/// </summary>
		/// <returns>The list of the <see cref="PersonStatus"/> objects.</returns>
		public List<PersonStatus> GetPersonStatuses()
		{
			return PersonStatusDAL.PersonStatusListAll();
		}

		#endregion
	}
}


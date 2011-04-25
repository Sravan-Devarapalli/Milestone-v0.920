using System.Collections.Generic;
using System.ServiceModel;
using DataTransferObjects;

namespace PracticeManagementService
{
	// NOTE: If you change the interface name "IPersonRoleService" here, you must also update the reference to "IPersonRoleService" in Web.config.
	[ServiceContract]
	public interface IPersonRoleService
	{
		/// <summary>
		/// Retrives a list of the avalable role for the person.
		/// </summary>
		/// <returns>The list of <see cref="PersonRole"/> objects.</returns>
		[OperationContract]
		List<PersonRole> GetPersonRoles();
	}
}


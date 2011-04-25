using System.Collections.Generic;
using System.ServiceModel.Activation;
using DataAccess;
using DataTransferObjects;

namespace PracticeManagementService
{
	// NOTE: If you change the class name "PersonRoleService" here, you must also update the reference to "PersonRoleService" in Web.config.
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class PersonRoleService : IPersonRoleService
	{
		/// <summary>
		/// Retrives a list of the avalable role for the person.
		/// </summary>
		/// <returns>The list of <see cref="PersonRole"/> objects.</returns>
		public List<PersonRole> GetPersonRoles()
		{
			return PersonRoleDAL.PersonRoleListAll();
		}
	}
}


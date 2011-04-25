using System.ServiceModel.Activation;
using System.Web.Security;
using DataAccess;
using DataTransferObjects;

namespace PracticeManagementService
{
	// NOTE: If you change the class name "AuthService" here, you must also update the reference to "AuthService" in Web.config.
	[AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Allowed)]
	public class AuthService : IAuthService
	{
		/// <summary>
		/// Verifies that the specified user name and password exist in the database.
		/// </summary>
		/// <param name="username">The name of the user to validate.</param>
		/// <param name="password">The password for the specified user.</param>
		/// <returns>
		/// true if the specified username and password are valid; otherwise, false.
		/// If the user specified does not exist in the database, the ValidateUser method returns false.
		/// </returns>
		public bool ValidateUser(string username, string password)
		{
			Person person = PersonDAL.PersonGetByAlias(username);
            if (person == null)
                return false;
		    if (person.Status != null && person.Status.Id == (int)PersonStatusType.Active)
				return Membership.ValidateUser(username, password);
			else
				return false;
		}
	}
}


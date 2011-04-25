using System.ServiceModel;

namespace PracticeManagementService
{
	[ServiceContract]
	public interface IAuthService
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
		[OperationContract]
		bool ValidateUser(string username, string password);
	}
}


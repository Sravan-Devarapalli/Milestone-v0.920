using System.ServiceModel;

using DataTransferObjects;

namespace PracticeManagementService
{
	// NOTE: If you change the interface name "IDefaultCommissionService" here, you must also update the reference to "IDefaultCommissionService" in Web.config.
	[ServiceContract]
	public interface IDefaultCommissionService
	{
		/// <summary>
		/// Retrieves the actual default sales commission for the specified person.
		/// </summary>
		/// <param name="personId">An ID of the person to retrieve the data for.</param>
		/// <returns>The list of the <see cref="DefaultCommission"/> objects.</returns>
		[OperationContract]
		DefaultCommission DefaultSalesCommissionByPerson(int personId);

		/// <summary>
		/// Retrieves the actual default management commission for the specified person.
		/// </summary>
		/// <param name="personId">An ID of the person to retrieve the data for.</param>
		/// <returns>The list of the <see cref="DefaultCommission"/> objects.</returns>
		[OperationContract]
		DefaultCommission DefaultManagementCommissionByPerson(int personId);
	}
}


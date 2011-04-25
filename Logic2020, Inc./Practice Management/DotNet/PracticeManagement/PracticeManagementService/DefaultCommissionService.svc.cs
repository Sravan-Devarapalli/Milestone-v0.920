using System.Collections.Generic;
using System.ServiceModel.Activation;
using DataAccess;
using DataTransferObjects;

namespace PracticeManagementService
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class DefaultCommissionService : IDefaultCommissionService
	{
		#region IDefaultCommissionService Members

		/// <summary>
		/// Retrieves the actual default sales commission for the specified person.
		/// </summary>
		/// <param name="personId">An ID of the person to retrieve the data for.</param>
		/// <returns>The list of the <see cref="DefaultCommission"/> objects.</returns>
		public DefaultCommission DefaultSalesCommissionByPerson(int personId)
		{
			return GetDefaultCommission(personId, CommissionType.Sales);
		}

		/// <summary>
		/// Retrieves the actual default management commission for the specified person.
		/// </summary>
		/// <param name="personId">An ID of the person to retrieve the data for.</param>
		/// <returns>The list of the <see cref="DefaultCommission"/> objects.</returns>
		public DefaultCommission DefaultManagementCommissionByPerson(int personId)
		{
			return GetDefaultCommission(personId, CommissionType.PracticeManagement);
		}

		private static DefaultCommission GetDefaultCommission(int personId, CommissionType commissionType)
		{
			List<DefaultCommission> commissions = DefaultCommissionDAL.DefaultCommissionListByPerson(personId);
			DefaultCommission result = null;

			foreach (DefaultCommission commission in commissions)
			{
				if (commission.TypeOfCommission == commissionType)
				{
					result = commission;
					break;
				}
			}

			return result;
		}

		#endregion
	}
}


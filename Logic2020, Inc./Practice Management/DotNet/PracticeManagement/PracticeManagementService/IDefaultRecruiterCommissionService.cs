using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataTransferObjects;

namespace PracticeManagementService
{
	[ServiceContract]
	public interface IDefaultRecruiterCommissionService
	{
		/// <summary>
		/// Retrieves the actual default recruiter commissions for the specified person.
		/// </summary>
		/// <param name="personId">An ID of the person to retrieve the data for.</param>
		/// <returns>The list of the <see cref="DefaultRecruiterCommission"/> objects.</returns>
		[OperationContract]
		List<DefaultRecruiterCommission> DefaultRecruiterCommissionListByPerson(int personId);

		/// <summary>
		/// Retrieves the actual default recruiter commissions for the specified person and date.
		/// </summary>
		/// <param name="personId">An ID of the person to retrieve the data for.</param>
		/// <param name="date">A date to retrieve the data for.</param>
		/// <returns>The <see cref="DefaultRecruiterCommission"/> object if found and null otherwise.</returns>
		[OperationContract]
		DefaultRecruiterCommission DefaultRecruiterCommissionGetByPersonDate(int personId, DateTime? date);

		/// <summary>
		/// Retrieves the default recruiter commissions with the specified ID.
		/// </summary>
		/// <param name="commissionHeaderId">An ID of the rtecord to be retrived.</param>
		/// <returns>The <see cref="DefaultRecruiterCommission"/> object if found and null otherwise.</returns>
		[OperationContract]
		DefaultRecruiterCommission DefaultRecruiterCommissionGetById(int commissionHeaderId);

		/// <summary>
		/// Saves the specified <see cref="DefaultRecruiterCommission"/> object to the database.
		/// </summary>
		/// <param name="commission">The <see cref="DefaultRecruiterCommission"/> data to be saved.</param>
		[OperationContract]
		int? DefaultRecruiterCommissionSave(DefaultRecruiterCommission commission);
	}
}


using System.Collections.Generic;
using System.ServiceModel;

using DataTransferObjects;

namespace PracticeManagementService
{
    [ServiceContract]
    public interface IOverheadService
    {
        /// <summary>
        /// Retrives the list of <see cref="OverheadRateType"/> objects.
        /// </summary>
        /// <returns>The list of the <see cref=""/> objects.</returns>
        [OperationContract]
        List<OverheadRateType> GetRateTypes();

        /// <summary>
        /// Retrieves an overhead rate type info by the specified ID.
        /// </summary>
        /// <param name="overheadRateTypeId">An ID of the record to be reatrieved from.</param>
        /// <returns>The <see cref="OverheadRateType"/> object if found and null otherwise.</returns>
        [OperationContract]
        OverheadRateType GetRateTypeDetail(int overheadRateTypeId);

        /// <summary>
        /// Retrieves the list of the <see cref="OverheadFixedRate"/> objects.
        /// </summary>
        /// <returns>The list of the <see cref="OverheadFixedRate"/> objects.</returns>
        [OperationContract]
        List<OverheadFixedRate> GetOverheadFixedRates(bool activeOnly);

        /// <summary>
        /// Retrieves the <see cref="OverheadFixedRate"/> object with the specified ID.
        /// </summary>
        /// <param name="overheadId">The ID of the <see cref="OverheadFixedRate"/> to be retrived.</param>
        /// <returns>The <see cref="OverheadFixedRate"/> object if found any or null otherwise.</returns>
        [OperationContract]
        OverheadFixedRate GetOverheadFixedRateDetail(int overheadId);

        /// <summary>
        /// Saves the <see cref="OverheadFixedRate"/> data into the database.
        /// </summary>
        /// <param name="overhead">The <see cref="OverheadFixedRate"/> data to be saved to.</param>
        [OperationContract]
        int? SaveOverheadFixedRateDetail(OverheadFixedRate overhead);

        /// <summary>
        /// Deactivates an overhead with the specified ID.
        /// </summary>
        /// <param name="overheadId">The ID of the overhead to be deactivated.</param>
        [OperationContract]
        void OverheadFixedRateInactivate(int overheadId);

        /// <summary>
        /// Activates an overhead with the specified ID.
        /// </summary>
        /// <param name="overheadId">The ID of the overhead to be activated.</param>
        [OperationContract]
        void OverheadFixedRateReactivate(int overheadId);

        /// <summary>
        /// Gets the Rate for each time scale type of a Overhead specified by description.
        /// </summary>
        /// <param name="OverHeadName"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        [OperationContract]
        Dictionary<int, decimal> GetMinimumLoadFactorOverheadMultipliers(string OverHeadName, ref bool isInactive);

        /// <summary>
        /// saves the Rate for each time scale type of a Overhead specified by description.
        /// </summary>
        /// <param name="OverHeadName"></param>
        /// <param name="inActive"></param>
        /// <param name="W2HourlyMultiplier"></param>
        /// <param name="W2SalaryMultiplier"></param>
        /// <param name="Hourly1099Multiplier"></param>
        [OperationContract]
        void SaveMinimumLoadFactorOverheadMultipliers(string OverHeadName, bool inActive, decimal W2HourlyMultiplier,
                                                                    decimal W2SalaryMultiplier, decimal Hourly1099Multiplier);
    }
}


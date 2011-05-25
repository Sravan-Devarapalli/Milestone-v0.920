using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using DataAccess;
using DataTransferObjects;

namespace PracticeManagementService
{
    // NOTE: If you change the class name "OverheadService" here, you must also update the reference to "OverheadService" in Web.config.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class OverheadService : IOverheadService
    {
        #region IOverheadService Members

        /// <summary>
        /// Retrives the list of <see cref="OverheadRateType"/> objects.
        /// </summary>
        /// <returns>The list of the <see cref=""/> objects.</returns>
        public List<OverheadRateType> GetRateTypes()
        {
            return OverheadRateTypeDAL.OverheadRateTypeListAll();
        }

        /// <summary>
        /// Retrieves an overhead rate type info by the specified ID.
        /// </summary>
        /// <param name="overheadRateTypeId">An ID of the record to be reatrieved from.</param>
        /// <returns>The <see cref="OverheadRateType"/> object if found and null otherwise.</returns>
        public OverheadRateType GetRateTypeDetail(int overheadRateTypeId)
        {
            return OverheadRateTypeDAL.OverheadRateTypeGetById(overheadRateTypeId);
        }

        /// <summary>
        /// Retrieves the list of the <see cref="OverheadFixedRate"/> objects.
        /// </summary>
        /// <returns>The list of the <see cref="OverheadFixedRate"/> objects.</returns>
        public List<OverheadFixedRate> GetOverheadFixedRates(bool activeOnly)
        {
            List<OverheadFixedRate> result = OverheadFixedRateDAL.OverheadFixedRateListAll(activeOnly);

            foreach (OverheadFixedRate overhead in result)
            {
                overhead.Timescales = OverheadFixedRateDAL.OverheadTimescaleList(overhead.Id.Value);
            }

            return result;
        }

        /// <summary>
        /// Retrieves the <see cref="OverheadFixedRate"/> object with the specified ID.
        /// </summary>
        /// <param name="overheadId">The ID of the <see cref="OverheadFixedRate"/> to be retrived.</param>
        /// <returns>The <see cref="OverheadFixedRate"/> object if found any or null otherwise.</returns>
        public OverheadFixedRate GetOverheadFixedRateDetail(int overheadId)
        {
            OverheadFixedRate result = OverheadFixedRateDAL.GetOverheadFixedRateDetail(overheadId);

            if (result != null)
            {
                result.Timescales = OverheadFixedRateDAL.OverheadTimescaleList(overheadId);
            }

            return result;
        }

        /// <summary>
        /// Saves the <see cref="OverheadFixedRate"/> data into the database.
        /// </summary>
        /// <param name="overhead">The <see cref="OverheadFixedRate"/> data to be saved to.</param>
        public int? SaveOverheadFixedRateDetail(OverheadFixedRate overhead)
        {
            if (!overhead.Id.HasValue)
            {
                OverheadFixedRateDAL.OverheadFixedRateInsert(overhead);
            }
            else
            {
                OverheadFixedRateDAL.OverheadFixedRateUpdate(overhead);
            }

            if (overhead.Timescales != null && overhead.Id.HasValue)
            {
                // Save the timescales for the overhead
                foreach (KeyValuePair<TimescaleType, bool> timescale in overhead.Timescales)
                {
                    OverheadFixedRateDAL.SetOverheadTimescale(overhead.Id.Value, timescale.Key, timescale.Value);
                }
            }

            return overhead.Id;
        }

        /// <summary>
        /// Deactivates an overhead with the specified ID.
        /// </summary>
        /// <param name="overheadId">The ID of the overhead to be deactivated.</param>
        public void OverheadFixedRateInactivate(int overheadId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Activates an overhead with the specified ID.
        /// </summary>
        /// <param name="overheadId">The ID of the overhead to be activated.</param>
        public void OverheadFixedRateReactivate(int overheadId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the Rate for each time scale type of a Overhead specified by description.
        /// </summary>
        /// <param name="OverHeadName"></param>
        /// <returns></returns>
        public Dictionary<int, decimal> GetMinimumLoadFactorOverheadMultipliers(string OverHeadName,ref bool isInActive)
        {
            return OverheadFixedRateDAL.GetMinimumLoadFactorOverheadMultipliers(OverHeadName, ref isInActive);
        }

        public void UpdateMinimumLoadFactorHistory(int timeScaleId, decimal rate)
        {
            OverheadFixedRateDAL.UpdateMinimumLoadFactorHistory(timeScaleId, rate);
        }
        public void UpdateMinimumLoadFactorStatus(bool inActive)
        {
            OverheadFixedRateDAL.UpdateMinimumLoadFactorStatus(inActive);
        }


        #endregion
    }
}


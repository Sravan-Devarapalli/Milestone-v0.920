using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.ServiceModel.Activation;
using System.Data;
using System.Data.Sql;
using DataAccess;
using DataAccess.Other;
using DataTransferObjects;

namespace PracticeManagementService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DefaultRecruiterCommissionService : IDefaultRecruiterCommissionService
    {
        #region IDefaultRecruiterCommissionService Members

        /// <summary>
        /// Retrieves the actual default recruiter commissions for the specified person.
        /// </summary>
        /// <param name="personId">An ID of the person to retrieve the data for.</param>
        /// <returns>The list of the <see cref="DefaultRecruiterCommission"/> objects.</returns>
        public List<DefaultRecruiterCommission> DefaultRecruiterCommissionListByPerson(int personId)
        {
            return DefaultRecruiterCommissionDAL.DefaultRecruiterCommissionListByPerson(personId);
        }

        /// <summary>
        /// Retrieves the actual default recruiter commissions for the specified person and date.
        /// </summary>
        /// <param name="personId">An ID of the person to retrieve the data for.</param>
        /// <param name="date">A date to retrieve the data for.</param>
        /// <returns>The <see cref="DefaultRecruiterCommission"/> object if found and null otherwise.</returns>
        public DefaultRecruiterCommission DefaultRecruiterCommissionGetByPersonDate(int personId, DateTime? date)
        {
            DefaultRecruiterCommission result =
                DefaultRecruiterCommissionDAL.DefaultRecruiterCommissionGetByPersonDate(personId, date);
            if (result != null)
            {
                result.Items =
                    DefaultRecruiterCommissionDAL.DefaultRecruiterCommissionItemList(
                    result.CommissionHeaderId.Value);
            }

            return result;
        }

        /// <summary>
        /// Retrieves the default recruiter commissions with the specified ID.
        /// </summary>
        /// <param name="commissionHeaderId">An ID of the rtecord to be retrived.</param>
        /// <returns>The <see cref="DefaultRecruiterCommission"/> object if found and null otherwise.</returns>
        public DefaultRecruiterCommission DefaultRecruiterCommissionGetById(int commissionHeaderId)
        {
            DefaultRecruiterCommission result =
                DefaultRecruiterCommissionDAL.DefaultRecruiterCommissionGetById(commissionHeaderId);
            if (result != null)
            {
                result.Items =
                    DefaultRecruiterCommissionDAL.DefaultRecruiterCommissionItemList(
                    result.CommissionHeaderId.Value);
            }

            return result;
        }

        /// <summary>
        /// Saves the specified <see cref="DefaultRecruiterCommission"/> object to the database.
        /// </summary>
        /// <param name="commission">The <see cref="DefaultRecruiterCommission"/> data to be saved.</param>
        public int? DefaultRecruiterCommissionSave(DefaultRecruiterCommission commission)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                connection.Open();
                var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                if (commission.CommissionHeaderId.HasValue)
                {
                    DefaultRecruiterCommissionDAL.DefaultRecruiterCommissionHeaderUpdate(commission, connection, transaction);
                }
                else
                {
                    DefaultRecruiterCommissionDAL.DefaultRecruiterCommissionHeaderInsert(commission, connection, transaction);
                }

                DefaultRecruiterCommissionDAL.DefaultRecruiterComissionDeleteItems(commission.CommissionHeaderId.Value, connection, transaction);

                if (commission.Items != null)
                {
                    foreach (DefaultRecruiterCommissionItem item in commission.Items)
                    {
                        item.CommissionHeaderId = commission.CommissionHeaderId.Value;
                        DefaultRecruiterCommissionDAL.DefaultRecruiterCommissionItemInsert(item, connection, transaction);
                    }
                }

                DefaultRecruiterCommissionDAL.DefaultRecruiterComissionCleanup(commission.CommissionHeaderId.Value, connection, transaction);

                PersonDAL.PersonEnsureIntegrity(commission.PersonId, connection, transaction);

                transaction.Commit();
                return commission.CommissionHeaderId;
            }
        }

        #endregion
    }
}


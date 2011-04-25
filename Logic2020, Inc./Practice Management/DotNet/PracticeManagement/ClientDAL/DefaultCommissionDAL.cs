using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
    /// <summary>
    /// Provides an access to the Default Commissions
    /// </summary>
    public static class DefaultCommissionDAL
    {
        #region Constants

        #region Stored Procedures

        private const string DefaultComissionGetByPersonProcedure = "dbo.DefaultComissionGetByPerson";
        private const string DefaultComissionSaveProcedure = "dbo.DefaultComissionSave";
        private const string DefaultComissionDeleteProcedure = "dbo.DefaultComissionDelete";

        #endregion

        #region Parameters

        private const string PersonIdParam = "@PersonId";
        private const string FractionOfMarginParam = "@FractionOfMargin";
        private const string TypeParam = "@Type";
        private const string MarginTypeIdParam = "@MarginTypeId";

        #endregion

        #region Columns

        private const string PersonIdColumn = "PersonId";
        private const string FractionOfMarginColumn = "FractionOfMargin";
        private const string StartDateColumn = "StartDate";
        private const string EndDateColumn = "EndDate";
        private const string TypeColumn = "Type";
        private const string MarginTypeIdColumn = "MarginTypeId";

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves actual values of the default commissions.
        /// </summary>
        /// <param name="personId">An ID of the person to retrive the data for.</param>
        /// <returns>The list of the <see cref="DefaultCommission"/> objects.</returns>
        public static List<DefaultCommission> DefaultCommissionListByPerson(int personId)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(DefaultComissionGetByPersonProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam, personId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<DefaultCommission> result = new List<DefaultCommission>();

                    ReadDefaultCommissions(reader, result);

                    return result;
                }
            }
        }

        /// <summary>
        /// Saves the default commission to the database.
        /// </summary>
        /// <param name="commission">The data to be saved.</param>
        public static void SaveDefaultCommissionDetail(DefaultCommission commission, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }
            using (SqlCommand command = new SqlCommand(DefaultComissionSaveProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam, commission.PersonId);
                command.Parameters.AddWithValue(FractionOfMarginParam, commission.FractionOfMargin);
                command.Parameters.AddWithValue(TypeParam, commission.TypeOfCommission);
                command.Parameters.AddWithValue(MarginTypeIdParam,
                    commission.MarginTypeId.HasValue ? (object)commission.MarginTypeId.Value : DBNull.Value);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                if (activeTransaction != null)
                {
                    command.Transaction = activeTransaction;
                }
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes a specified <see cref="DefaultCommission"/> from the database.
        /// </summary>
        /// <param name="commission">The data to be deleted from.</param>
        public static void DeleteDefaultCommission(DefaultCommission commission, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }
            using (SqlCommand command = new SqlCommand(DefaultComissionDeleteProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam, commission.PersonId);
                command.Parameters.AddWithValue(TypeParam, commission.TypeOfCommission);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                if (activeTransaction != null)
                {
                    command.Transaction = activeTransaction;
                }

                command.ExecuteNonQuery();
            }
        }

        private static void ReadDefaultCommissions(DbDataReader reader, List<DefaultCommission> result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(PersonIdColumn);
                int fractionOfMarginIndex = reader.GetOrdinal(FractionOfMarginColumn);
                int startDateIndex = reader.GetOrdinal(StartDateColumn);
                int endDateIndex = reader.GetOrdinal(EndDateColumn);
                int typeIndex = reader.GetOrdinal(TypeColumn);
                int marginTypeIdIndex = reader.GetOrdinal(MarginTypeIdColumn);

                while (reader.Read())
                {
                    DefaultCommission commission = new DefaultCommission();

                    commission.PersonId = reader.GetInt32(personIdIndex);
                    commission.FractionOfMargin = reader.GetDecimal(fractionOfMarginIndex);
                    commission.StartDate = reader.GetDateTime(startDateIndex);
                    commission.EndDate = reader.GetDateTime(endDateIndex);
                    commission.TypeOfCommission = (CommissionType)reader.GetInt32(typeIndex);
                    commission.MarginTypeId =
                        !reader.IsDBNull(marginTypeIdIndex) ? (int?)reader.GetInt32(marginTypeIdIndex) : null;

                    result.Add(commission);
                }
            }
        }

        #endregion
    }
}


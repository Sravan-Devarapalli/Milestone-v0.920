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
    /// Provides an access to the project's commissions
    /// </summary>
    public static class CommissionDAL
    {
        #region Constants

        #region Stored Procedures

        private const string CommissionGetByProjectTypeProcedure = "dbo.CommissionGetByProjectType";
        private const string CommissionSetProcedure = "dbo.CommissionSet";

        #endregion

        #region Parameters

        private const string CommissionIdParam = "@CommissionId";
        private const string ProjectIdParam = "@ProjectId";
        private const string CommissionTypeParam = "@CommissionType";
        private const string PersonIdParam = "@PersonId";
        private const string FractionOfMarginParam = "@FractionOfMargin";
        private const string ExpectedDatePaidParam = "@ExpectedDatePaid";
        private const string ActualDatePaidParam = "@ActualDatePaid";
        private const string MarginTypeIdParam = "@MarginTypeId";

        #endregion

        #region Columns

        private const string CommissionIdColumn = "CommissionId";
        private const string ProjectIdColumn = "ProjectId";
        private const string PersonIdColumn = "PersonId";
        private const string FractionOfMarginColumn = "FractionOfMargin";
        private const string CommissionTypeColumn = "CommissionType";
        private const string ExpectedDatePaidColumn = "ExpectedDatePaid";
        private const string ActualDatePaidColumn = "ActualDatePaid";
        private const string MarginTypeIdColumn = "MarginTypeId";

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Retrives a list of the <see cref="Commission"/> objects for the specified Project and Commission Type combination.
        /// </summary>
        /// <param name="projectId">An ID of the <see cref="Project"/> to the data be retrieved for.</param>
        /// <param name="commissionType">The <see cref="CommissionType"/> to the data be retrieved for.</param>
        /// <returns>A list of the <see cref="Commission"/> objects if found any and null otherwise.</returns>
        public static List<Commission> CommissionGetByProjectType(int projectId, CommissionType commissionType)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(CommissionGetByProjectTypeProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ProjectIdParam, projectId);
                command.Parameters.AddWithValue(CommissionTypeParam, commissionType);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<Commission> result = new List<Commission>();

                    ReadCommissions(reader, result);

                    return result;
                }
            }
        }

        /// <summary>
        /// Saves the <see cref="Commission"/> data into the database.
        /// </summary>
        /// <param name="commission">The data to be saved to.</param>
        public static void CommissionSet(Commission commission, SqlConnection connection, SqlTransaction currentTransaction)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

            using (SqlCommand command = new SqlCommand(CommissionSetProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(CommissionIdParam,
                    commission.Id.HasValue ? (object)commission.Id.Value : DBNull.Value);
                command.Parameters.AddWithValue(ProjectIdParam,
                    commission.ProjectWithMargin != null && commission.ProjectWithMargin.Id.HasValue ?
                    (object)commission.ProjectWithMargin.Id.Value : DBNull.Value);
                command.Parameters.AddWithValue(PersonIdParam,
                    commission.PersonId.HasValue ? (object)commission.PersonId.Value : DBNull.Value);
                command.Parameters.AddWithValue(FractionOfMarginParam, commission.FractionOfMargin);
                command.Parameters.AddWithValue(CommissionTypeParam, commission.TypeOfCommission);
                command.Parameters.AddWithValue(ExpectedDatePaidParam, DBNull.Value);
                command.Parameters.AddWithValue(ActualDatePaidParam, DBNull.Value);
                command.Parameters.AddWithValue(MarginTypeIdParam,
                    commission.MarginTypeId.HasValue ? (object)commission.MarginTypeId.Value : DBNull.Value);

                if (currentTransaction != null)
                {
                    command.Transaction = currentTransaction;
                }

                command.ExecuteNonQuery();
            }
        }

        private static void ReadCommissions(DbDataReader reader, List<Commission> result)
        {
            if (reader.HasRows)
            {
                int commissionIdIndex = reader.GetOrdinal(CommissionIdColumn);
                int personIdIndex = reader.GetOrdinal(PersonIdColumn);
                int fractionOfMarginIndex = reader.GetOrdinal(FractionOfMarginColumn);
                int commissionTypeIndex = reader.GetOrdinal(CommissionTypeColumn);
                int projectIdIndex = reader.GetOrdinal(ProjectIdColumn);
                int expectedDatePaidIndex = reader.GetOrdinal(ExpectedDatePaidColumn);
                int actualDatePaidIndex = reader.GetOrdinal(ActualDatePaidColumn);
                int marginTypeIdIndex = reader.GetOrdinal(MarginTypeIdColumn);

                while (reader.Read())
                {
                    Commission commission = new Commission();

                    commission.Id = reader.GetInt32(commissionIdIndex);
                    commission.PersonId = reader.GetInt32(personIdIndex);
                    commission.FractionOfMargin = reader.GetDecimal(fractionOfMarginIndex);
                    commission.TypeOfCommission = (CommissionType)reader.GetInt32(commissionTypeIndex);
                    commission.MarginTypeId =
                        !reader.IsDBNull(marginTypeIdIndex) ? (int?)reader.GetInt32(marginTypeIdIndex) : null;

                    commission.ProjectWithMargin = new Project();
                    commission.ProjectWithMargin.Id = reader.GetInt32(projectIdIndex);

                    result.Add(commission);
                }
            }
        }

        #endregion
    }
}


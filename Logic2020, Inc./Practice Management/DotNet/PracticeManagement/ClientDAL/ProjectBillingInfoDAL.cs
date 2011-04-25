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
    /// 	Provides an access to the ProjectBillingInfo table.
    /// </summary>
    public static class ProjectBillingInfoDAL
    {
        #region Constants

        private const string ProjectBillingInfoSaveProcedure = "dbo.ProjectBillingInfoSave";
        private const string ProjectBillingInfoGetByIdProcedure = "dbo.ProjectBillingInfoGetById";
        private const string ProjectBillingInfoDeleteProcedure = "dbo.ProjectBillingInfoDelete";

        private const string ProjectIdParam = "@ProjectId";
        private const string BillingContactParam = "@BillingContact";
        private const string BillingPhoneParam = "@BillingPhone";
        private const string BillingEmailParam = "@BillingEmail";
        private const string BillingTypeParam = "@BillingType";
        private const string BillingAddress1Param = "@BillingAddress1";
        private const string BillingAddress2Param = "@BillingAddress2";
        private const string BillingCityParam = "@BillingCity";
        private const string BillingStateParam = "@BillingState";
        private const string BillingZipParam = "@BillingZip";
        private const string PurchaseOrderParam = "@PurchaseOrder";

        private const string ProjectIdColumn = "ProjectId";
        private const string BillingContactColumn = "BillingContact";
        private const string BillingPhoneColumn = "BillingPhone";
        private const string BillingEmailColumn = "BillingEmail";
        private const string BillingTypeColumn = "BillingType";
        private const string BillingAddress1Column = "BillingAddress1";
        private const string BillingAddress2Column = "BillingAddress2";
        private const string BillingCityColumn = "BillingCity";
        private const string BillingStateColumn = "BillingState";
        private const string BillingZipColumn = "BillingZip";
        private const string PurchaseOrderColumn = "PurchaseOrder";

        #endregion

        #region Methods

        /// <summary>
        /// 	Saves the project billing info into the database.
        /// </summary>
        /// <param name = "info">The data to be saved to.</param>
        public static void ProjectBillingInfoSave(BillingInfo info, SqlConnection connection, SqlTransaction currentTransaction)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

            using (var command = new SqlCommand(ProjectBillingInfoSaveProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ProjectIdParam,
                                                info.Id.HasValue ? (object)info.Id.Value : DBNull.Value);
                command.Parameters.AddWithValue(BillingContactParam,
                                                !string.IsNullOrEmpty(info.BillingContact)
                                                    ? (object)info.BillingContact
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(BillingPhoneParam,
                                                !string.IsNullOrEmpty(info.BillingPhone)
                                                    ? (object)info.BillingPhone
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(BillingEmailParam,
                                                !string.IsNullOrEmpty(info.BillingEmail)
                                                    ? (object)info.BillingEmail
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(BillingTypeParam,
                                                !string.IsNullOrEmpty(info.BillingType)
                                                    ? (object)info.BillingType
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(BillingAddress1Param,
                                                !string.IsNullOrEmpty(info.BillingAddress1)
                                                    ? (object)info.BillingAddress1
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(BillingAddress2Param,
                                                !string.IsNullOrEmpty(info.BillingAddress2)
                                                    ? (object)info.BillingAddress2
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(BillingCityParam,
                                                !string.IsNullOrEmpty(info.BillingCity)
                                                    ? (object)info.BillingCity
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(BillingStateParam,
                                                !string.IsNullOrEmpty(info.BillingState)
                                                    ? (object)info.BillingState
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(BillingZipParam,
                                                !string.IsNullOrEmpty(info.BillingZip)
                                                    ? (object)info.BillingZip
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(PurchaseOrderParam,
                                                !string.IsNullOrEmpty(info.PurchaseOrder)
                                                    ? (object)info.PurchaseOrder
                                                    : DBNull.Value);

                try
                {
                    if (currentTransaction != null)
                    {
                        command.Transaction = currentTransaction;
                    }

                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new DataAccessException(ex);
                }
            }
        }

        /// <summary>
        /// 	Removes the project billing info for the specified project.
        /// </summary>
        /// <param name = "projectId">An ID of the project to delete the data for.</param>
        public static void ProjectBillingInfoDelete(int projectId, SqlConnection connection, SqlTransaction currentTransaction)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

            using (var command = new SqlCommand(ProjectBillingInfoDeleteProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ProjectIdParam, projectId);

                try
                {
                    if (currentTransaction != null)
                    {
                        command.Transaction = currentTransaction;
                    }

                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new DataAccessException(ex);
                }
            }
        }

        /// <summary>
        /// 	Retrives a billing info for the specified project.
        /// </summary>
        /// <param name = "projectId">An ID of the project to retrive the data for.</param>
        /// <returns>The <see cref = "BillingInfo" /> object of found and null otherwise.</returns>
        public static BillingInfo ProjectBillingInfoGetById(int projectId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(ProjectBillingInfoGetByIdProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ProjectIdParam, projectId);

                connection.Open();
                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var result = new List<BillingInfo>(1);
                    ReadBillingInfo(reader, result);
                    return result.Count > 0 ? result[0] : null;
                }
            }
        }

        private static void ReadBillingInfo(DbDataReader reader, List<BillingInfo> result)
        {
            if (reader.HasRows)
            {
                var projectIdIndex = reader.GetOrdinal(ProjectIdColumn);
                var billingContactIndex = reader.GetOrdinal(BillingContactColumn);
                var billingPhoneIndex = reader.GetOrdinal(BillingPhoneColumn);
                var billingEmailIndex = reader.GetOrdinal(BillingEmailColumn);
                var billingTypeIndex = reader.GetOrdinal(BillingTypeColumn);
                var billingAddress1Index = reader.GetOrdinal(BillingAddress1Column);
                var billingAddress2Index = reader.GetOrdinal(BillingAddress2Column);
                var billingCityIndex = reader.GetOrdinal(BillingCityColumn);
                var billingStateIndex = reader.GetOrdinal(BillingStateColumn);
                var billingZipIndex = reader.GetOrdinal(BillingZipColumn);
                var purchaseOrderIndex = reader.GetOrdinal(PurchaseOrderColumn);

                while (reader.Read())
                {
                    var info = new BillingInfo();

                    info.Id = reader.GetInt32(projectIdIndex);
                    info.BillingContact =
                        !reader.IsDBNull(billingContactIndex) ? reader.GetString(billingContactIndex) : string.Empty;
                    info.BillingPhone =
                        !reader.IsDBNull(billingPhoneIndex) ? reader.GetString(billingPhoneIndex) : string.Empty;
                    info.BillingEmail =
                        !reader.IsDBNull(billingEmailIndex) ? reader.GetString(billingEmailIndex) : string.Empty;
                    info.BillingType =
                        !reader.IsDBNull(billingTypeIndex) ? reader.GetString(billingTypeIndex) : string.Empty;
                    info.BillingAddress1 =
                        !reader.IsDBNull(billingAddress1Index) ? reader.GetString(billingAddress1Index) : string.Empty;
                    info.BillingAddress2 =
                        !reader.IsDBNull(billingAddress2Index) ? reader.GetString(billingAddress2Index) : string.Empty;
                    info.BillingCity =
                        !reader.IsDBNull(billingCityIndex) ? reader.GetString(billingCityIndex) : string.Empty;
                    info.BillingState =
                        !reader.IsDBNull(billingStateIndex) ? reader.GetString(billingStateIndex) : string.Empty;
                    info.BillingZip =
                        !reader.IsDBNull(billingZipIndex) ? reader.GetString(billingZipIndex) : string.Empty;
                    info.PurchaseOrder =
                        !reader.IsDBNull(purchaseOrderIndex) ? reader.GetString(purchaseOrderIndex) : string.Empty;

                    result.Add(info);
                }
            }
        }

        #endregion
    }
}

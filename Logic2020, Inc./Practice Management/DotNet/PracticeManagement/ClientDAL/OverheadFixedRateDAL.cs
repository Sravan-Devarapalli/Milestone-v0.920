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
    /// Provides an access to the OverheadFixedRate data.
    /// </summary>
    public static class OverheadFixedRateDAL
    {
        #region Constants

        #region Procedures

        private const string OverheadFixedRateListAllProcedure = "dbo.OverheadFixedRateListAll";
        private const string OverheadTimescaleListByOverheadIdProcedure = "dbo.OverheadTimescaleListByOverheadId";
        private const string OverheadTimescaleSetProcedure = "dbo.OverheadTimescaleSet";
        private const string OverheadTimescaleRemoveProcedure = "dbo.OverheadTimescaleRemove";
        private const string OverheadFixedRateGetByIdProcedure = "dbo.OverheadFixedRateGetById";
        private const string OverheadFixedRateInsertProcedure = "dbo.OverheadFixedRateInsert";
        private const string OverheadFixedRateUpdateProcedure = "dbo.OverheadFixedRateUpdate";
        private const string GetMLFMultipliersProcedure = "dbo.GetMinimumLoadFactorMultipliers";
        private const string SaveMLFMultipliersProcedure = "dbo.SaveMinimumLoadFactorOverheadMultipliers";
        private const string UpdateMinimumLoadFactorHistoryProcedure = "dbo.UpdateMinimumLoadFactorHistory";
        private const string UpdateMinimumLoadFactorStatusProcedure = "dbo.UpdateMinimumLoadFactorStatus";

        #endregion

        #region Parameters

        private const string OverheadFixedRateIdParam = "@OverheadFixedRateId";
        private const string TimescaleIdParam = "@TimescaleId";
        private const string DescriptionParam = "@Description";
        private const string RateParam = "@Rate";
        private const string StartDateParam = "@StartDate";
        private const string EndDateParam = "@EndDate";
        private const string RateTypeParam = "@RateType";
        private const string InactiveParam = "@Inactive";
        private const string ShowAllParam = "@ShowAll";
        private const string IsCogsParam = "@IsCogs";
        private const string W2HourlyMultiplierParam = "@W2HourlyMultiplier";
        private const string W2SalaryMultiplierParam = "@W2SalaryMultiplier";
        private const string Hourly1099MultiplierParam = "@Hourly1099Multiplier";


        #endregion

        #region Columns

        private const string OverheadFixedRateIdColumn = "OverheadFixedRateId";
        private const string DescriptionColumn = "Description";
        private const string RateColumn = "Rate";
        private const string StartDateColumn = "StartDate";
        private const string EndDateColumn = "EndDate";
        private const string InactiveColumn = "Inactive";
        private const string OverheadRateTypeIdColumn = "OverheadRateTypeId";
        private const string OverheadRateTypeNameColumn = "OverheadRateTypeName";
        private const string IsPercentageColumn = "IsPercentage";
        private const string HoursToCollectColumn = "HoursToCollect";
        private const string TimescaleIdColumn = "TimescaleId";
        private const string IsSetColumn = "IsSet";
        private const string IsCogsColumn = "IsCogs";

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves a list of the <see cref="OverheadFixedRate"/> objects.
        /// </summary>
        /// <returns>The list of the <see cref="OverheadFixedRate"/> objects.</returns>
        public static List<OverheadFixedRate> OverheadFixedRateListAll(bool activeOnly)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(OverheadFixedRateListAllProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ShowAllParam, !activeOnly);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<OverheadFixedRate> result = new List<OverheadFixedRate>();

                    ReadOverheadFixedRates(reader, result);
                    return result;
                }
            }
        }

        /// <summary>
        /// Gets the Rate for each time scale type of a Overhead specified by description.
        /// </summary>
        /// <param name="OverHeadName"></param>
        /// <returns></returns>
        public static Dictionary<int, decimal> GetMinimumLoadFactorOverheadMultipliers(string OverHeadName, ref bool isInActive)
        {
            var MLFMultipliers = new Dictionary<int, decimal>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(GetMLFMultipliersProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(DescriptionParam, OverHeadName);

                SqlParameter isInActiveParam = new SqlParameter(InactiveParam, SqlDbType.Bit);
                isInActiveParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(isInActiveParam);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<OverheadFixedRate> result = new List<OverheadFixedRate>();

                    ReadMLFMultipliers(reader, MLFMultipliers);
                    if (!reader.IsClosed)
                        reader.Close();
                    isInActive = (bool)isInActiveParam.Value;
                }
            }
            return MLFMultipliers;
        }

        /// <summary>
        /// saves the Rate for each time scale type of a Overhead specified by description.
        /// </summary>
        /// <param name="OverHeadName"></param>
        /// <param name="W2HourlyMultiplier"></param>
        /// <param name="W2SalaryMultiplier"></param>
        /// <param name="Hourly1099Multiplier"></param>
        public static void SaveMinimumLoadFactorOverheadMultipliers(string overHeadName, bool inActive, decimal w2HourlyMultiplier,
                                                                    decimal w2SalaryMultiplier, decimal hourly1099Multiplier)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command =
                new SqlCommand(SaveMLFMultipliersProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(InactiveParam, inActive);
                command.Parameters.AddWithValue(DescriptionParam, overHeadName);
                command.Parameters.AddWithValue(W2HourlyMultiplierParam, w2HourlyMultiplier);
                command.Parameters.AddWithValue(W2SalaryMultiplierParam, w2SalaryMultiplier);
                command.Parameters.AddWithValue(Hourly1099MultiplierParam, hourly1099Multiplier);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void UpdateMinimumLoadFactorHistory(int timeScaleId, decimal rate )
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command =
                new SqlCommand(UpdateMinimumLoadFactorHistoryProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(TimescaleIdParam, timeScaleId);
                command.Parameters.AddWithValue(RateParam, rate);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void UpdateMinimumLoadFactorStatus(bool inActive)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command =
                new SqlCommand(UpdateMinimumLoadFactorStatusProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(InactiveParam, inActive);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }


        private static void ReadMLFMultipliers(SqlDataReader reader, Dictionary<int, decimal> MLFMultipliers)
        {
            if (reader.HasRows)
            {
                int timescaleIdIndex = reader.GetOrdinal(TimescaleIdColumn);
                int RateColumnIndex = reader.GetOrdinal(RateColumn);

                while (reader.Read())
                {
                    MLFMultipliers.Add(reader.GetInt32(timescaleIdIndex), reader.GetDecimal(RateColumnIndex));
                }
            }
        }

        /// <summary>
        /// Retrievers the timescales and their applicability for the specified overhead.
        /// </summary>
        /// <param name="overheadId">An ID of the <see cref="OverheadFixedRate"/> object.</param>
        /// <returns>The map of the timescales and their applicability.</returns>
        public static Dictionary<TimescaleType, bool> OverheadTimescaleList(int overheadId)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(OverheadTimescaleListByOverheadIdProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(OverheadFixedRateIdParam, overheadId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Dictionary<TimescaleType, bool> result = new Dictionary<TimescaleType, bool>();

                    ReadOverheadTimescales(reader, result);

                    return result;
                }
            }
        }

        /// <summary>
        /// Sets the timescale available or non-available for the specified overhead.
        /// </summary>
        /// <param name="overheadId">The overhead to the flag be set for.</param>
        /// <param name="timescaleId">The timescale to the flag be set for.</param>
        /// <param name="isApplicable">The timescale's applicability.</param>
        public static void SetOverheadTimescale(int overheadId, TimescaleType timescaleId, bool isApplicable)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command =
                new SqlCommand(isApplicable ? OverheadTimescaleSetProcedure : OverheadTimescaleRemoveProcedure,
                    connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(OverheadFixedRateIdParam, overheadId);
                command.Parameters.AddWithValue(TimescaleIdParam, timescaleId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Retrives an overhead with a specified ID from the database.
        /// </summary>
        /// <param name="overheadId">The ID of the overhead to be retrieved.</param>
        /// <returns>The <see cref="OverheadFixedRate"/> if found any and null otherwise.</returns>
        public static OverheadFixedRate GetOverheadFixedRateDetail(int overheadId)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(OverheadFixedRateGetByIdProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(OverheadFixedRateIdParam, overheadId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
                {
                    List<OverheadFixedRate> result = new List<OverheadFixedRate>();

                    ReadOverheadFixedRates(reader, result);
                    return result.Count > 0 ? result[0] : null;
                }
            }
        }

        /// <summary>
        /// Inserts the <see cref="OverheadFixedRate"/> data into the database.
        /// </summary>
        /// <param name="overhead">The <see cref="OverheadFixedRate"/> data be inserted to.</param>
        public static void OverheadFixedRateInsert(OverheadFixedRate overhead)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(OverheadFixedRateInsertProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(DescriptionParam,
                    !string.IsNullOrEmpty(overhead.Description) ? (object)overhead.Description : DBNull.Value);
                command.Parameters.AddWithValue(RateParam, overhead.Rate.Value);
                command.Parameters.AddWithValue(StartDateParam, overhead.StartDate);
                command.Parameters.AddWithValue(EndDateParam,
                    overhead.EndDate.HasValue ? (object)overhead.EndDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(RateTypeParam,
                    overhead.RateType != null ? (object)overhead.RateType.Id : DBNull.Value);
                command.Parameters.AddWithValue(InactiveParam, overhead.Inactive);
                command.Parameters.AddWithValue(IsCogsParam, overhead.IsCogs);

                SqlParameter overheadFixedRateIdParam = new SqlParameter(OverheadFixedRateIdParam, SqlDbType.Int);
                overheadFixedRateIdParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(overheadFixedRateIdParam);

                connection.Open();
                command.ExecuteNonQuery();

                overhead.Id = (int)overheadFixedRateIdParam.Value;
            }
        }

        /// <summary>
        /// Updates the <see cref="OverheadFixedRate"/> data in the database.
        /// </summary>
        /// <param name="overhead">The <see cref="OverheadFixedRate"/> data to be updated.</param>
        public static void OverheadFixedRateUpdate(OverheadFixedRate overhead)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(OverheadFixedRateUpdateProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(OverheadFixedRateIdParam, overhead.Id.Value);
                command.Parameters.AddWithValue(DescriptionParam,
                    !string.IsNullOrEmpty(overhead.Description) ? (object)overhead.Description : DBNull.Value);
                command.Parameters.AddWithValue(RateParam, overhead.Rate.Value);
                command.Parameters.AddWithValue(StartDateParam, overhead.StartDate);
                command.Parameters.AddWithValue(EndDateParam,
                    overhead.EndDate.HasValue ? (object)overhead.EndDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(RateTypeParam,
                    overhead.RateType != null ? (object)overhead.RateType.Id : DBNull.Value);
                command.Parameters.AddWithValue(InactiveParam, overhead.Inactive);
                command.Parameters.AddWithValue(IsCogsParam, overhead.IsCogs);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static void ReadOverheadTimescales(DbDataReader reader, Dictionary<TimescaleType, bool> result)
        {
            if (reader.HasRows)
            {
                int timescaleIdIndex = reader.GetOrdinal(TimescaleIdColumn);
                int isSetIndex = reader.GetOrdinal(IsSetColumn);

                while (reader.Read())
                {
                    result.Add((TimescaleType)reader.GetInt32(timescaleIdIndex), reader.GetBoolean(isSetIndex));
                }
            }
        }

        private static void ReadOverheadFixedRates(DbDataReader reader, List<OverheadFixedRate> result)
        {
            if (reader.HasRows)
            {
                int overheadFixedRateIdIndex = reader.GetOrdinal(OverheadFixedRateIdColumn);
                int descriptionIndex = reader.GetOrdinal(DescriptionColumn);
                int rateIndex = reader.GetOrdinal(RateColumn);
                int startDateIndex = reader.GetOrdinal(StartDateColumn);
                int endDateIndex = reader.GetOrdinal(EndDateColumn);
                int inactiveIndex = reader.GetOrdinal(InactiveColumn);
                int overheadRateTypeIdIndex = reader.GetOrdinal(OverheadRateTypeIdColumn);
                int overheadRateTypeNameIndex = reader.GetOrdinal(OverheadRateTypeNameColumn);
                int isPercentageIndex = reader.GetOrdinal(IsPercentageColumn);
                int hoursToCollectIndex = reader.GetOrdinal(HoursToCollectColumn);
                int isCogsIndex = reader.GetOrdinal(IsCogsColumn);

                while (reader.Read())
                {
                    OverheadFixedRate rate = new OverheadFixedRate();

                    rate.Id = reader.GetInt32(overheadFixedRateIdIndex);
                    rate.Description = reader.GetString(descriptionIndex);
                    rate.Rate = reader.GetDecimal(rateIndex);
                    rate.StartDate = reader.GetDateTime(startDateIndex);
                    rate.EndDate =
                        !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null;
                    rate.Inactive = reader.GetBoolean(inactiveIndex);
                    rate.IsCogs = reader.GetBoolean(isCogsIndex);

                    rate.RateType = new OverheadRateType();
                    rate.RateType.Id = reader.GetInt32(overheadRateTypeIdIndex);
                    rate.RateType.Name = reader.GetString(overheadRateTypeNameIndex);
                    rate.RateType.IsPercentage = reader.GetBoolean(isPercentageIndex);
                    rate.RateType.HoursToCollect = reader.GetInt32(hoursToCollectIndex);

                    result.Add(rate);
                }
            }
        }

        #endregion
    }
}


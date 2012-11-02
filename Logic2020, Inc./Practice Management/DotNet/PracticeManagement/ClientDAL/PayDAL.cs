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
    /// Access pay data in database
    /// </summary>
    public static class PayDAL
    {
        #region Constants

        #region Parameters
        
        private const string AmountParam = "@Amount";
        private const string TimescaleParam = "@Timescale";
        private const string TimesPaidPerMonthParam = "@TimesPaidPerMonth";
        private const string TermsParam = "@Terms";
        private const string VacationDaysParam = "@VacationDays";
        private const string BonusAmountParam = "@BonusAmount";
        private const string BonusHoursToCollectParam = "@BonusHoursToCollect";
        private const string DefaultHoursPerDayParam = "@DefaultHoursPerDay";
        private const string OldStartDateParam = "@OLD_StartDate";
        private const string OldEndDateParam = "@OLD_EndDate";
        private const string SeniorityIdParam = "@SeniorityId";
        private const string PracticeIdParam = "@PracticeId";
        private const string SalesCommFractionOfMarginParam = "@SalesCommissionFractionOfMargin";

        #endregion

        #region Columns

        private const string PersonIdColumn = "PersonId";
        private const string StartDateColumn = "StartDate";
        private const string EndDateColumn = "EndDate";
        private const string AmountColumn = "Amount";
        private const string TimescaleNameColumn = "TimescaleName";
        private const string AmountHourlyColumn = "AmountHourly";
        private const string TimesPaidPerMonthColumn = "TimesPaidPerMonth";
        private const string TermsColumn = "Terms";
        private const string VacationDaysColumn = "VacationDays";
        private const string BonusAmountColumn = "BonusAmount";
        private const string BonusHoursToCollectColumn = "BonusHoursToCollect";
        private const string IsYearBonusColumn = "IsYearBonus";
        private const string DefaultHoursPerDayColumn = "DefaultHoursPerDay";
        private const string SeniorityIdColumn = "SeniorityId";
        private const string SeniorityNameColumn = "SeniorityName";
        private const string PracticeIdColumn = "PracticeId";
        private const string PracticeNameColumn = "PracticeName";
        private const string SalesCommFractionOfMarginColumn = "SalesCommissionFractionOfMargin";

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves a current pay for the specified person.
        /// </summary>
        /// <param name="personId">An ID of the <see cref="Person"/> to retrieve the data for.</param>
        /// <returns>The <see cref="Pay"/> object if found any or null otherwise.</returns>
        public static Pay GetCurrentByPerson(int personId)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Pay.PayGetCurrentByPersonProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<Pay> result = new List<Pay>();
                    ReadPay(reader, result);

                    return result.Count > 0 ? result[0] : null;
                }
            }
        }

        /// <summary>
        /// Retrives a payment history for the specified person.
        /// </summary>
        /// <param name="personId">An ID of the <see cref="Person"/> to retrieve the data for.</param>
        /// <returns>The list of the <see cref="Pay"/> objects.</returns>
        public static List<Pay> GetHistoryByPerson(int personId)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Pay.PayGetHistoryByPersonProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<Pay> result = new List<Pay>();
                    ReadPay(reader, result);

                    return result;
                }
            }
        }

        public static List<Pay> GetPayHistoryShortByPerson(int personId)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Pay.GetPayHistoryShortByPersonProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<Pay> result = new List<Pay>();
                    ReadPayShort(reader, result);

                    return result;
                }
            }
        }

        /// <summary>
        /// Saves a <see cref="Pay"/> object to the database.
        /// </summary>
        /// <param name="pay">The <see cref="Pay"/> object to be saved.</param>
        public static void SavePayDatail(Pay pay, SqlConnection connection = null, SqlTransaction activeTransaction = null, string user = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Pay.PaySaveProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, pay.PersonId);
                command.Parameters.AddWithValue(AmountParam, pay.Amount.Value);
                command.Parameters.AddWithValue(TimescaleParam, pay.Timescale);
                command.Parameters.AddWithValue(TimesPaidPerMonthParam,
                    pay.TimesPaidPerMonth.HasValue ? (object)pay.TimesPaidPerMonth.Value : DBNull.Value);
                command.Parameters.AddWithValue(TermsParam,
                    pay.Terms.HasValue ? (object)pay.Terms.Value : DBNull.Value);
                command.Parameters.AddWithValue(VacationDaysParam,
                    pay.VacationDays.HasValue ? (object)pay.VacationDays.Value : DBNull.Value);
                command.Parameters.AddWithValue(BonusAmountParam, pay.BonusAmount.Value);
                command.Parameters.AddWithValue(BonusHoursToCollectParam,
                    pay.BonusHoursToCollect.HasValue ? (object)pay.BonusHoursToCollect.Value : DBNull.Value);
                command.Parameters.AddWithValue(DefaultHoursPerDayParam, pay.DefaultHoursPerDay);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, pay.StartDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam,
                    pay.EndDate.HasValue ? (object)pay.EndDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(OldStartDateParam,
                    pay.OldStartDate.HasValue ? (object)pay.OldStartDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(OldEndDateParam,
                    pay.OldEndDate.HasValue ? (object)pay.OldEndDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(SeniorityIdParam,
                    pay.SeniorityId.HasValue ? (object)pay.SeniorityId.Value : DBNull.Value);
                command.Parameters.AddWithValue(PracticeIdParam,
                    pay.PracticeId.HasValue ? (object)pay.PracticeId.Value : DBNull.Value);
                command.Parameters.AddWithValue(SalesCommFractionOfMarginParam,
                    pay.SalesCommissionFractionOfMargin.HasValue ? (object)pay.SalesCommissionFractionOfMargin.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, string.IsNullOrEmpty(user) ? DBNull.Value : (object)user);
                try
                {
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
                catch (SqlException ex)
                {
                    throw new DataAccessException(ex);
                }
            }
        }

        public static void DeletePay(int personId, DateTime startDate)
        {
            var connection = new SqlConnection(DataSourceHelper.DataConnection);

            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Pay.PayDeleteProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);

                try
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new DataAccessException(ex);
                }
            }
        }

        private static void ReadPayShort(DbDataReader reader, List<Pay> result)
        {
            if (reader.HasRows)
            {
                int startDateIndex = reader.GetOrdinal(StartDateColumn);
                int endDateIndex = reader.GetOrdinal(EndDateColumn);
                int timescaleIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleColumn);


                while (reader.Read())
                {
                    Pay pay = new Pay();

                    pay.Timescale = (TimescaleType)reader.GetInt32(timescaleIndex);

                    pay.StartDate = reader.GetDateTime(startDateIndex);
                    pay.EndDate =
                        !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null;

                    result.Add(pay);
                }
            }
        }

        private static void ReadPay(DbDataReader reader, List<Pay> result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(PersonIdColumn);
                int startDateIndex = reader.GetOrdinal(StartDateColumn);
                int endDateIndex = reader.GetOrdinal(EndDateColumn);
                int amountIndex = reader.GetOrdinal(AmountColumn);
                int timescaleIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleColumn);
                int timescaleNameIndex = reader.GetOrdinal(TimescaleNameColumn);
                int amountHourlyIndex = reader.GetOrdinal(AmountHourlyColumn);
                int timesPaidPerMonthIndex = reader.GetOrdinal(TimesPaidPerMonthColumn);
                int termsIndex = reader.GetOrdinal(TermsColumn);
                int vacationDaysIndex = reader.GetOrdinal(VacationDaysColumn);
                int bonusAmountIndex = reader.GetOrdinal(BonusAmountColumn);
                int bonusHoursToCollectIndex = reader.GetOrdinal(BonusHoursToCollectColumn);
                int isYearBonusIndex = reader.GetOrdinal(IsYearBonusColumn);
                int defaultHoursPerDayIndex = reader.GetOrdinal(DefaultHoursPerDayColumn);
                int SeniorityIdIndex = reader.GetOrdinal(SeniorityIdColumn);
                int SeniorityNameIndex = reader.GetOrdinal(SeniorityNameColumn);
                int PracticeIdIndex = reader.GetOrdinal(PracticeIdColumn);
                int PracticeNameIndex = reader.GetOrdinal(PracticeNameColumn);
                int SalesCommFractionOfMarginIndex = reader.GetOrdinal(SalesCommFractionOfMarginColumn);

                while (reader.Read())
                {
                    Pay pay = new Pay();

                    pay.PersonId = reader.GetInt32(personIdIndex);
                    pay.Timescale = (TimescaleType)reader.GetInt32(timescaleIndex);
                    pay.TimescaleName = reader.GetString(timescaleNameIndex);
                    pay.Amount = reader.GetDecimal(amountIndex);
                    pay.StartDate = reader.GetDateTime(startDateIndex);
                    pay.EndDate =
                        !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null;
                    pay.AmountHourly = reader.GetDecimal(amountHourlyIndex);
                    pay.TimesPaidPerMonth =
                        !reader.IsDBNull(timesPaidPerMonthIndex) ?
                        (int?)reader.GetInt32(timesPaidPerMonthIndex) : null;
                    pay.Terms = !reader.IsDBNull(termsIndex) ? (int?)reader.GetInt32(termsIndex) : null;
                    pay.VacationDays =
                        !reader.IsDBNull(vacationDaysIndex) ? (int?)reader.GetInt32(vacationDaysIndex) : null;
                    pay.BonusAmount = reader.GetDecimal(bonusAmountIndex);
                    pay.BonusHoursToCollect =
                        !reader.IsDBNull(bonusHoursToCollectIndex) ?
                        (int?)reader.GetInt32(bonusHoursToCollectIndex) : null;
                    pay.IsYearBonus = reader.GetBoolean(isYearBonusIndex);
                    pay.DefaultHoursPerDay = reader.GetDecimal(defaultHoursPerDayIndex);
                    pay.SeniorityId = !reader.IsDBNull(SeniorityIdIndex) ? (int?)reader.GetInt32(SeniorityIdIndex) : null;
                    pay.SeniorityName = !reader.IsDBNull(SeniorityNameIndex) ? reader.GetString(SeniorityNameIndex) : string.Empty;
                    pay.PracticeId = !reader.IsDBNull(PracticeIdIndex) ? (int?)reader.GetInt32(PracticeIdIndex) : null;
                    pay.PracticeName = !reader.IsDBNull(PracticeNameIndex) ? reader.GetString(PracticeNameIndex) : string.Empty;
                    pay.SalesCommissionFractionOfMargin = !reader.IsDBNull(SalesCommFractionOfMarginIndex) ? (decimal?)reader.GetDecimal(SalesCommFractionOfMarginIndex) : null;
                    result.Add(pay);
                }
            }
        }

        public static List<Triple<DateTime, bool, bool>> IsPersonSalaryTypeListByPeriod(int personId, DateTime startDate, DateTime endDate)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Pay.IsPersonSalaryTypeListByPeriodProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<Triple<DateTime, bool, bool>> result = new List<Triple<DateTime, bool, bool>>();
                    if (reader.HasRows)
                    {
                        int dateIndex = reader.GetOrdinal(Constants.ColumnNames.DateColumn);
                        int isSalaryTypeIndex = reader.GetOrdinal(Constants.ColumnNames.IsSalaryType);
                        int isHourlyTypeIndex = reader.GetOrdinal(Constants.ColumnNames.IsHourlyType);

                        while (reader.Read())
                        {
                            Triple<DateTime, bool, bool> resultLocal = new Triple<DateTime, bool, bool>(DateTime.Now,true,true);
                            resultLocal.First = reader.GetDateTime(dateIndex);
                            resultLocal.Second = reader.GetBoolean(isSalaryTypeIndex);
                            resultLocal.Third = reader.GetBoolean(isHourlyTypeIndex);
                            result.Add(resultLocal);
                        }
                    }
                    return result;
                }
            }
        }

        #endregion
    }
}


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
    public static class CalendarDAL
    {
        #region Methods

        /// <summary>
        /// Retrieves a list of the calendar items from the database.
        /// </summary>
        /// <param name="startDate">The start of the period.</param>
        /// <param name="endDate">The end of the period.</param>
        /// <param name="personId">
        /// An ID of the person to the calendar be retrieved for.
        /// If null the company calendar will be returned.
        /// </param>
        /// <param name="practiceManagerId">
        /// An ID of the practice manager to retrieve the data for his subordinate
        /// </param>
        /// <returns>The list of the <see cref="CalendarItem"/> objects.</returns>
        public static List<CalendarItem> CalendarList(DateTime startDate, DateTime endDate, int? personId,
                                                      int? practiceManagerId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Calendar.CalendarGetProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId,
                                                personId.HasValue ? (object) personId.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeManagerIdParam,
                                                practiceManagerId.HasValue
                                                    ? (object) practiceManagerId.Value
                                                    : DBNull.Value);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<CalendarItem>();

                    ReadCalendarItems(reader, result);

                    return result;
                }
            }
        }

        /// <summary>
        /// Saves a <see cref="CalendarItem"/> object to the database.
        /// </summary>
        /// <param name="item">The data to be saved to.</param>
        public static void CalendarUpdate(CalendarItem item)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Calendar.CalendarUpdateProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.Date, item.Date);
                command.Parameters.AddWithValue(Constants.ParameterNames.DayOff, item.DayOff);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId,
                                                item.PersonId.HasValue ? (object) item.PersonId.Value : DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Retrieves a number of work days for the specified period
        /// </summary>
        /// <param name="startDate">The period start.</param>
        /// <param name="endDate">The period end.</param>
        /// <returns>A number of company work days for the period.</returns>
        public static int WorkDaysCompanyNumber(DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                var command = new SqlCommand(Constants.ProcedureNames.Calendar.WorkDaysCompanyNumberProcedure,
                                             connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);

                connection.Open();
                return (int) command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Retrieves a number of work days for the specified period and person.
        /// </summary>
        /// <param name="personId">A person to the data be retrieved for.</param>
        /// <param name="startDate">The period start.</param>
        /// <param name="endDate">The period end.</param>
        /// <returns>A number of person's work days for the period.</returns>
        public static int WorkDaysPersonNumber(int personId, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                var command = new SqlCommand(Constants.ProcedureNames.Calendar.WorkDaysPersonNumberProcedure, connection)
                )
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);

                connection.Open();
                return (int) command.ExecuteScalar();
            }
        }

        private static void ReadCalendarItems(SqlDataReader reader, List<CalendarItem> result)
        {
            if (reader.HasRows)
            {
                int dateIndex;
                int dayOffIndex;
                int companyDayOffIndex;
                int readOnlyIndex;
                GetCalendarItemIndexes(reader, out dateIndex, out dayOffIndex, out companyDayOffIndex, out readOnlyIndex);

                while (reader.Read())
                    result.Add(ReadSingleCalendarItem(reader, dateIndex, dayOffIndex, companyDayOffIndex, readOnlyIndex));
            }
        }

        /// <summary>
        /// Returns indexes required to read CalendarItem data
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dateIndex"></param>
        /// <param name="dayOffIndex"></param>
        /// <param name="companyDayOffIndex"></param>
        /// <param name="readOnlyIndex"></param>
        /// <returns>True if indexes have been initialized, false otherwise</returns>
        public static bool GetCalendarItemIndexes(SqlDataReader reader, out int dateIndex, out int dayOffIndex, out int companyDayOffIndex, out int readOnlyIndex)
        {
            try
            {
                dateIndex = reader.GetOrdinal(Constants.ColumnNames.Date);
                dayOffIndex = reader.GetOrdinal(Constants.ColumnNames.DayOff);
                companyDayOffIndex = reader.GetOrdinal(Constants.ColumnNames.CompanyDayOff);
                readOnlyIndex = reader.GetOrdinal(Constants.ColumnNames.ReadOnly);

                return true;
            }
            catch (Exception)
            {
                dateIndex = dayOffIndex = companyDayOffIndex = readOnlyIndex = -1;
            }

            return false;
        }

        public static CalendarItem ReadSingleCalendarItem(SqlDataReader reader, int dateIndex, int dayOffIndex, int companyDayOffIndex, int readOnlyIndex)
        {
            return new CalendarItem
                       {
                           Date = reader.GetDateTime(dateIndex),
                           DayOff = reader.GetBoolean(dayOffIndex),
                           CompanyDayOff = reader.GetBoolean(companyDayOffIndex),
                           ReadOnly = reader.GetBoolean(readOnlyIndex)
                       };
        }

        /// <summary>
        /// Returns No. of Company holidays in a given year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int GetCompanyHolidays(int year)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                var command = new SqlCommand(Constants.ProcedureNames.Calendar.GetCompanyHolidaysProcedure,
                                             connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.YearParam, year);

                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }

        #endregion
    }
}

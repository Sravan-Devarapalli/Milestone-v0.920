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
        public static List<CalendarItem> CalendarList(DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Calendar.CalendarGetProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<CalendarItem>();

                    ReadCalendarItems(reader, result);

                    return result;
                }
            }
        }

        public static List<CalendarItem> PersonCalendarList(DateTime startDate, DateTime endDate, int? personId, int? practiceManagerId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Calendar.PersonCalendarGetProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId,
                                                personId.HasValue ? (object)personId.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeManagerIdParam,
                                                practiceManagerId.HasValue
                                                    ? (object)practiceManagerId.Value
                                                    : DBNull.Value);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<CalendarItem>();

                    ReadPersonCalendarItems(reader, result);

                    return result;
                }
            }
        }

        /// <summary>
        /// Saves a <see cref="CalendarItem"/> object to the database.
        /// </summary>
        /// <param name="item">The data to be saved to.</param>
        public static void CalendarUpdate(CalendarItem item, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Calendar.CalendarUpdateProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.Date, item.Date);
                command.Parameters.AddWithValue(Constants.ParameterNames.DayOff, item.DayOff);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsRecurringHoliday, item.IsRecurringHoliday);
                command.Parameters.AddWithValue(Constants.ParameterNames.RecurringHolidayId, item.RecurringHolidayId.HasValue ? (object)item.RecurringHolidayId.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.HolidayDescription, string.IsNullOrEmpty(item.HolidayDescription) ? DBNull.Value : (object)item.HolidayDescription);
                command.Parameters.AddWithValue(Constants.ParameterNames.RecurringHolidayDate, item.RecurringHolidayDate.HasValue ? (object)item.RecurringHolidayDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ActualHoursParam, item.ActualHours.HasValue ? (object)item.ActualHours : DBNull.Value);

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
        public static Dictionary<string,decimal> GetCompanyWorkHoursAndDaysInGivenPeriod(DateTime startDate, DateTime endDate,bool includeCompanyHolidays)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                var command = new SqlCommand(Constants.ProcedureNames.Calendar.GetCompanyWorkHoursAndDaysInGivenPeriodProcedure,
                                             connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IncludeCompanyHolidays, includeCompanyHolidays);

                Dictionary<string, decimal> result = new Dictionary<string, decimal>();
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int workHoursIndex = reader.GetOrdinal(Constants.ColumnNames.WorkHours);
                        int workDaysIndex = reader.GetOrdinal(Constants.ColumnNames.WorkDays);

                        while (reader.Read())
                        {
                            result.Add("Hours",reader.GetDecimal(workHoursIndex));
                            result.Add("Days", reader.GetDecimal(workDaysIndex));
                        }
                    }
                }
                
                return result;
            }
        }

        public static int GetWorkingDaysForTheGivenYear(int year)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Calendar.GetWorkingDaysForTheGivenYear,connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.YearParam, year);

                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Retrieves a number of work days for the specified period and person.
        /// </summary>
        /// <param name="personId">A person to the data be retrieved for.</param>
        /// <param name="startDate">The period start.</param>
        /// <param name="endDate">The period end.</param>
        /// <returns>A number of person's work days for the period.</returns>
        public static PersonWorkingHoursDetailsWithinThePeriod GetPersonWorkingHoursDetailsWithinThePeriod(int personId, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                var command = new SqlCommand(Constants.ProcedureNames.Calendar.GetPersonWorkingHoursDetailsWithinThePeriodProcedure, connection)
                )
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                PersonWorkingHoursDetailsWithinThePeriod result = new PersonWorkingHoursDetailsWithinThePeriod();
                result.PersonId = personId;
                result.StartDate = startDate;
                result.EndDate = endDate;

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int totalWorkHoursExcludingVacationHoursIndex = reader.GetOrdinal(Constants.ColumnNames.TotalWorkHoursExcludingVacationHours);
                        int totalWorkDaysIncludingVacationDaysIndex = reader.GetOrdinal(Constants.ColumnNames.TotalWorkDaysIncludingVacationDays);
                        int vacationDaysIndex = reader.GetOrdinal(Constants.ColumnNames.VacationDays);

                        while (reader.Read())
                        {
                            result.TotalWorkHoursExcludingVacationHours = reader.GetDecimal(totalWorkHoursExcludingVacationHoursIndex);
                            result.TotalWorkDaysIncludingVacationDays = reader.GetInt32(totalWorkDaysIncludingVacationDaysIndex);
                            result.VacationDays = reader.GetInt32(vacationDaysIndex);
                        }
                    }
                }

                return result;
            }
        }

        private static void ReadCalendarItems(SqlDataReader reader, List<CalendarItem> result)
        {
            if (reader.HasRows)
            {
                int dateIndex;
                int dayOffIndex;
                int companyDayOffIndex;
                int isRecurringIndex;
                int recurringHolidayIdIndex;
                int holidayDescriptionIndex;
                int recurringHolidayDateIndex;
                GetCalendarItemIndexes(reader, out dateIndex, out dayOffIndex, out companyDayOffIndex, out isRecurringIndex, out recurringHolidayIdIndex, out holidayDescriptionIndex, out recurringHolidayDateIndex);

                while (reader.Read())
                    result.Add(ReadSingleCalendarItem(reader, dateIndex, dayOffIndex, companyDayOffIndex, isRecurringIndex, recurringHolidayIdIndex, holidayDescriptionIndex, recurringHolidayDateIndex));
            }
        }

        private static void ReadPersonCalendarItems(SqlDataReader reader, List<CalendarItem> result)
        {
            if (reader.HasRows)
            {
                int dateIndex;
                int dayOffIndex;
                int companyDayOffIndex;
                int readOnlyIndex;
                int holidayDescriptionIndex;
                int actualHoursIndex;
                int isFloatingHolidayIndex;
                int timeTypeIdIndex;
                int isUnpaidTimeTypeIndex;
                GetPersonCalendarItemIndexes(reader, out dateIndex, out dayOffIndex, out companyDayOffIndex, out readOnlyIndex, out  holidayDescriptionIndex, out actualHoursIndex, out isFloatingHolidayIndex, out timeTypeIdIndex, out isUnpaidTimeTypeIndex);

                while (reader.Read())
                    result.Add(ReadSinglePersonCalendarItem(reader, dateIndex, dayOffIndex, companyDayOffIndex, readOnlyIndex, holidayDescriptionIndex, actualHoursIndex, isFloatingHolidayIndex, timeTypeIdIndex, isUnpaidTimeTypeIndex));
            }
        }

        public static bool GetCalendarItemIndexes(SqlDataReader reader, out int dateIndex, out int dayOffIndex, out int companyDayOffIndex, out int isRecurringIndex, out int recurringHolidayIdIndex, out int holidayDescriptionIndex, out int recurringHolidayDateIndex)
        {
            try
            {
                dateIndex = reader.GetOrdinal(Constants.ColumnNames.Date);
                dayOffIndex = reader.GetOrdinal(Constants.ColumnNames.DayOff);
                companyDayOffIndex = reader.GetOrdinal(Constants.ColumnNames.CompanyDayOff);
                isRecurringIndex = reader.GetOrdinal(Constants.ColumnNames.IsRecurringColumn);
                recurringHolidayIdIndex = reader.GetOrdinal(Constants.ColumnNames.RecurringHolidayIdColumn);
                holidayDescriptionIndex = reader.GetOrdinal(Constants.ColumnNames.HolidayDescriptionColumn);
                recurringHolidayDateIndex = reader.GetOrdinal(Constants.ColumnNames.RecurringHolidayDateColumn);
                return true;
            }
            catch (Exception)
            {
                dateIndex = dayOffIndex = companyDayOffIndex = isRecurringIndex = recurringHolidayIdIndex = holidayDescriptionIndex = recurringHolidayDateIndex = -1;
            }

            return false;
        }

        public static CalendarItem ReadSingleCalendarItem(SqlDataReader reader, int dateIndex, int dayOffIndex, int companyDayOffIndex, int isRecurringIndex, int recurringHolidayIdIndex, int holidayDescriptionIndex, int recurringHolidayDateIndex)
        {
            bool isrecurring = Convert.ToBoolean(reader.IsDBNull(isRecurringIndex) ? null : (object)reader.GetBoolean(isRecurringIndex));
            int? recurringHoliday = reader.IsDBNull(recurringHolidayIdIndex) ? null : (int?)reader.GetInt32(recurringHolidayIdIndex);
            var description = reader.IsDBNull(holidayDescriptionIndex) ? string.Empty : reader.GetString(holidayDescriptionIndex);
            DateTime? recurringHolidayDate = reader.IsDBNull(recurringHolidayDateIndex) ? null : (DateTime?)reader.GetDateTime(recurringHolidayDateIndex);
            var calendarItem = new CalendarItem
            {
                Date = reader.GetDateTime(dateIndex),
                DayOff = reader.GetBoolean(dayOffIndex),
                CompanyDayOff = reader.GetBoolean(companyDayOffIndex),
                IsRecurringHoliday = isrecurring,
                RecurringHolidayId = recurringHoliday,
                HolidayDescription = description,
                RecurringHolidayDate = recurringHolidayDate
            };
            return calendarItem;
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
        public static bool GetPersonCalendarItemIndexes(SqlDataReader reader, out int dateIndex, out int dayOffIndex, out int companyDayOffIndex, out int readOnlyIndex, out int holidayDescriptionIndex, out int actualHoursIndex, out int isFloatingHolidayIndex, out int timeTypeIdIndex, out int isUnpaidTimeTypeIndex)
        {
            try
            {
                dateIndex = reader.GetOrdinal(Constants.ColumnNames.Date);
                dayOffIndex = reader.GetOrdinal(Constants.ColumnNames.DayOff);
                companyDayOffIndex = reader.GetOrdinal(Constants.ColumnNames.CompanyDayOff);
                readOnlyIndex = reader.GetOrdinal(Constants.ColumnNames.ReadOnly);
                timeTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeId);
                holidayDescriptionIndex = reader.GetOrdinal(Constants.ColumnNames.HolidayDescriptionColumn);
                actualHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ActualHours);
                isFloatingHolidayIndex = reader.GetOrdinal(Constants.ColumnNames.IsFloatingHolidayColumn);
                isUnpaidTimeTypeIndex = reader.GetOrdinal(Constants.ColumnNames.IsUnpaidTimeType);
                return true;
            }
            catch (Exception)
            {
                timeTypeIdIndex = dateIndex = dayOffIndex = companyDayOffIndex = readOnlyIndex = holidayDescriptionIndex = actualHoursIndex = isFloatingHolidayIndex = isUnpaidTimeTypeIndex = -1;
            }

            return false;
        }

        public static CalendarItem ReadSinglePersonCalendarItem(SqlDataReader reader, int dateIndex, int dayOffIndex, int companyDayOffIndex, int readOnlyIndex, int holidayDescriptionIndex, int actualHoursIndex, int isFloatingHolidayIndex, int timeTypeIdIndex, int isUnpaidTimeTypeIndex)
        {
            var calendarItem = new CalendarItem
            {
                Date = reader.GetDateTime(dateIndex),
                DayOff = reader.GetBoolean(dayOffIndex),
                CompanyDayOff = reader.GetBoolean(companyDayOffIndex),
                ReadOnly = reader.GetBoolean(readOnlyIndex),
                HolidayDescription = reader.IsDBNull(holidayDescriptionIndex) ? string.Empty : reader.GetString(holidayDescriptionIndex),
                TimeTypeId = reader.IsDBNull(timeTypeIdIndex) ? null : (int?)reader.GetInt32(timeTypeIdIndex),
                ActualHours = reader.IsDBNull(actualHoursIndex) ? null : (double?)reader.GetFloat(actualHoursIndex),
                IsFloatingHoliday = reader.GetInt32(isFloatingHolidayIndex) == 1,
                IsUnpaidTimeType = reader.GetInt32(isUnpaidTimeTypeIndex) == 1
            };


            return calendarItem;
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

        public static List<Triple<int, string, bool>> GetRecurringHolidaysList()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Calendar.GetRecurringHolidaysList, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        var list = new List<Triple<int, string, bool>>();

                        ReadRecurringHolidaysList(reader, list);

                        return list;
                    }
                }
            }
        }

        private static void ReadRecurringHolidaysList(SqlDataReader reader, List<Triple<int, string, bool>> list)
        {
            if (reader.HasRows)
            {
                int idIndex = reader.GetOrdinal(Constants.ColumnNames.Id);
                int descriptionIndex = reader.GetOrdinal(Constants.ColumnNames.DescriptionColumn);
                int isSetIndex = reader.GetOrdinal(Constants.ColumnNames.IsSetColumn);

                while (reader.Read())
                {
                    var item = new Triple<int, string, bool>(reader.GetInt32(idIndex), reader.GetString(descriptionIndex), reader.GetBoolean(isSetIndex));

                    list.Add(item);
                }
            }
        }

        public static void SetRecurringHoliday(int? id, bool isSet, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Calendar.SetRecurringHoliday, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    if (id.HasValue)
                        command.Parameters.AddWithValue(Constants.ParameterNames.IdParam, id.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsSetParam, isSet);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }

        public static Dictionary<DateTime, string> GetRecurringHolidaysInWeek(DateTime date, int personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Calendar.GetRecurringHolidaysInWeek, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, date);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        Dictionary<DateTime, String> recurringHolidaysList = new Dictionary<DateTime, string>();

                        ReadRecurringHolidaysListWithDate(reader, recurringHolidaysList);

                        return recurringHolidaysList;
                    }
                }
            }
        }

        private static void ReadRecurringHolidaysListWithDate(SqlDataReader reader, Dictionary<DateTime, string> list)
        {
            if (reader.HasRows)
            {
                int datetimeIndex = reader.GetOrdinal(Constants.ColumnNames.Date);
                int descriptionIndex = reader.GetOrdinal(Constants.ColumnNames.DescriptionColumn);

                while (reader.Read())
                {
                    DateTime date = reader.GetDateTime(datetimeIndex);
                    string description = reader.GetString(descriptionIndex);
                    list.Add(date, description);
                }
            }
        }

        public static void SaveSubstituteDay(CalendarItem item, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Calendar.SaveSubstituteDayProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.Date, item.Date);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId,
                                                item.PersonId.HasValue ? (object)item.PersonId.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                command.Parameters.AddWithValue(Constants.ParameterNames.SubstituteDayDateParam, item.SubstituteDayDate);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void DeleteSubstituteDay(int personId, DateTime substituteDayDate, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Calendar.DeleteSubstituteDayProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId,
                                                personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.SubstituteDayDateParam, substituteDayDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void SaveTimeOff(DateTime startDate, DateTime endDate, bool dayOff, int personId, double? actualHours, int timeTypeId, string userLogin, int? approvedBy)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Calendar.SaveTimeOffProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.DayOff, dayOff);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.ActualHoursParam, actualHours.HasValue ? (object)actualHours : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeId, timeTypeId);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                command.Parameters.AddWithValue(Constants.ParameterNames.ApprovedByParam, approvedBy.HasValue ? (object)approvedBy : DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static Quadruple<DateTime, DateTime, int?, string> GetTimeOffSeriesPeriod(int personId, DateTime date)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Calendar.GetTimeOffSeriesPeriod, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.DateParam, date);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        return ReadGetTimeOffSeriesPeriod(reader);
                    }
                }
            }
        }

        private static Quadruple<DateTime, DateTime, int?, string> ReadGetTimeOffSeriesPeriod(SqlDataReader reader)
        {
            if (reader.HasRows)
            {
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDateColumn);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDateColumn);
                int approvedByIdIndex = reader.GetOrdinal(Constants.ColumnNames.ApprovedByColumn);
                int approvedByNameIndex = reader.GetOrdinal(Constants.ColumnNames.ApprovedByNameColumn);

                while (reader.Read())
                {
                    return new Quadruple<DateTime, DateTime, int?, string>(reader.GetDateTime(startDateIndex),
                                                                reader.GetDateTime(endDateIndex),
                                                                reader.IsDBNull(approvedByIdIndex) ? null : (int?)reader.GetInt32(approvedByIdIndex),
                                                                reader.IsDBNull(approvedByNameIndex) ? string.Empty : reader.GetString(approvedByNameIndex));
                }
            }
            return null;
        }

        public static DateTime GetSubstituteDate(int personId, DateTime holidayDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Calendar.GetSubstituteDate, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.HolidayDateParam, holidayDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);

                    connection.Open();

                    return (DateTime)command.ExecuteScalar();
                }
            }
        }

        public static KeyValuePair<DateTime, string> GetSubstituteDayDetails(int personId, DateTime substituteDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Calendar.GetSubstituteDayDetails, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.SubstituteDayDateParam, substituteDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        return ReadSubstituteDayDetails(reader);
                    }
                }
            }
        }

        private static KeyValuePair<DateTime, string> ReadSubstituteDayDetails(SqlDataReader reader)
        {
            KeyValuePair<DateTime, string> keyval = new KeyValuePair<DateTime, string>();
            if (reader.HasRows)
            {
                int holidayDescriptionIndex = reader.GetOrdinal(Constants.ColumnNames.HolidayDescriptionColumn);
                int HolidayDateIndex = reader.GetOrdinal(Constants.ColumnNames.HolidayDateColumn);

                while (reader.Read())
                {
                    keyval = new KeyValuePair<DateTime, string>(reader.GetDateTime(HolidayDateIndex), reader.GetString(holidayDescriptionIndex));
                }
            }
            return keyval;
        }


        #endregion
    }
}


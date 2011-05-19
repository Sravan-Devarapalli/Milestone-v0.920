using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using DataAccess.Other;
using DataTransferObjects;
using DataTransferObjects.CompositeObjects;
using DataTransferObjects.ContextObjects;
using DataTransferObjects.TimeEntry;

namespace DataAccess
{
    public static class TimeEntryDAL
    {
        #region Time type

        /// <summary>
        /// Retrieves all existing time types
        /// </summary>
        /// <returns>Collection of new time types</returns>
        public static IEnumerable<TimeTypeRecord> GetAllTimeTypes()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeType.GetAll, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return new List<TimeTypeRecord>(ReadTimeTypes(reader));
                }
            }
        }

        /// <summary>
        /// Updates given time type
        /// </summary>
        /// <param name="timeType">Time type to update</param>
        public static void UpdateTimeType(TimeTypeRecord timeType)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeType.Update, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeId, timeType.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.Name, timeType.Name);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsDefault, timeType.IsDefault);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Adds new time type
        /// </summary>
        /// <param name="timeType">Time type to add</param>
        /// <returns>Id of added time type</returns>
        public static int AddTimeType(TimeTypeRecord timeType)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeType.Insert, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.Name, timeType.Name);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsDefault, timeType.IsDefault);

                connection.Open();

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Removes given time type
        /// </summary>
        /// <param name="timeType">Time type to remove</param>
        public static void RemoveTimeType(TimeTypeRecord timeType)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeType.Delete, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeId, timeType.Id);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        private static IEnumerable<TimeTypeRecord> ReadTimeTypes(DbDataReader reader)
        {
            if (reader.HasRows)
            {
                int timeTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeId);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
                int inUseIndex = reader.GetOrdinal(Constants.ColumnNames.InUse);
                int isDefaultIndex = reader.GetOrdinal(Constants.ColumnNames.IsDefault);

                while (reader.Read())
                {
                    var tt = new TimeTypeRecord
                                 {
                                     Id = reader.GetInt32(timeTypeIdIndex),
                                     Name = reader.GetString(nameIndex),
                                     IsDefault = reader.GetBoolean(isDefaultIndex)
                                 };
                    //  Make default time types marked as InUse to disallow removing them
                    tt.InUse = bool.Parse(reader.GetString(inUseIndex)) || tt.IsDefault;
                    yield return tt;
                }
            }
        }

        public static void RemoveTimeEntries(int milestonePersonId, int timeTypeId, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.RemoveTimeEntries, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.MilestonePersonId, milestonePersonId);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeId, timeTypeId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        #endregion

        #region Toggling

        public static void ToggleIsCorrect(TimeEntryRecord timeEntry)
        {
            ToggleTimeEntryProperty(timeEntry, Constants.ProcedureNames.TimeEntry.ToggleIsCorrect);
        }

        public static void ToggleIsReviewed(TimeEntryRecord timeEntry)
        {
            ToggleTimeEntryProperty(timeEntry, Constants.ProcedureNames.TimeEntry.ToggleIsReviewed);
        }

        public static void ToggleIsChargeable(TimeEntryRecord timeEntry)
        {
            ToggleTimeEntryProperty(timeEntry, Constants.ProcedureNames.TimeEntry.ToggleIsChargeable);
        }

        private static void ToggleTimeEntryProperty(TimeEntryRecord timeEntry, string procedureName)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                if (timeEntry.Id != null)
                    command.Parameters.AddWithValue(
                        Constants.ParameterNames.TimeEntryId,
                        timeEntry.Id.Value);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        #endregion

        public static void RemoveTimeEntry(TimeEntryRecord timeEntry)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.Remove, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.TimeEntryId, timeEntry.Id.Value);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Adds new time entry
        /// </summary>
        /// <param name="timeEntry">Time entry to add</param>
        public static int AddTimeEntry(TimeEntryRecord timeEntry, int defaultMpId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.Add, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.EntryDate, timeEntry.EntryDate);
                command.Parameters.AddWithValue(
                    Constants.ParameterNames.MilestonePersonId,
                    timeEntry.ParentMilestonePersonEntry.MilestonePersonId);
                command.Parameters.AddWithValue(Constants.ParameterNames.ActualHours, timeEntry.ActualHours);
                command.Parameters.AddWithValue(Constants.ParameterNames.ForecastedHours, timeEntry.ForecastedHours);
                command.Parameters.AddWithValue(Constants.ParameterNames.ModifiedBy, timeEntry.ModifiedBy.Id.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeId, timeEntry.TimeType.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.Note, timeEntry.Note);
                command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneDate, timeEntry.MilestoneDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsCorrect, timeEntry.IsCorrect);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsChargeable, timeEntry.IsChargeable);
                command.Parameters.AddWithValue(Constants.ParameterNames.DefaultMpId, defaultMpId);
                command.Parameters.AddWithValue(
                    Constants.ParameterNames.PersonId,
                    timeEntry.ParentMilestonePersonEntry.ThisPerson.Id.Value);

                var teIdParam = new SqlParameter(Constants.ParameterNames.TimeEntryId, SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(teIdParam);

                connection.Open();

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException sqlException)
                {
                    throw new Exception(sqlException.Message, sqlException);
                }

                return Convert.ToInt32(teIdParam.Value);
            }
        }

        /// <summary>
        /// Updates time entry
        /// </summary>
        /// <param name="timeEntry">Time entry to add</param>
        public static void UpdateTimeEntry(TimeEntryRecord timeEntry, int defaultMilestoneId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.Update, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.TimeEntryId, timeEntry.Id.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.EntryDate, timeEntry.EntryDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneDate, timeEntry.MilestoneDate);
                command.Parameters.AddWithValue(
                    Constants.ParameterNames.MilestonePersonId,
                    timeEntry.ParentMilestonePersonEntry.MilestonePersonId);
                command.Parameters.AddWithValue(Constants.ParameterNames.ActualHours, timeEntry.ActualHours);
                command.Parameters.AddWithValue(Constants.ParameterNames.ForecastedHours, timeEntry.ForecastedHours);
                command.Parameters.AddWithValue(Constants.ParameterNames.ModifiedBy, timeEntry.ModifiedBy.Id.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeId, timeEntry.TimeType.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.Note, timeEntry.Note);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsChargeable, timeEntry.IsChargeable);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsCorrect, timeEntry.IsCorrect);
                command.Parameters.AddWithValue(Constants.ParameterNames.DefaultMilestoneId, defaultMilestoneId);

                int personId = 0;
                if (timeEntry.ParentMilestonePersonEntry.ThisPerson != null
                    && timeEntry.ParentMilestonePersonEntry.ThisPerson.Id.HasValue
                    )
                {
                    personId = timeEntry.ParentMilestonePersonEntry.ThisPerson.Id.Value;
                }

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);

                bool? isReviewed = Utils.ReviewStatus2Bool(timeEntry.IsReviewed);
                if (isReviewed.HasValue)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsReviewed, isReviewed.Value);
                }

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Get time entries by person
        /// </summary>
        public static TimeEntryRecord[] GetTimeEntriesForPerson(Person person, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.Get, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, person.Id.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);

                connection.Open();

                using (var reader = command.ExecuteReader())
                    return ReadTimeEntries(reader);
            }
        }

        /// <summary>
        /// Get milestones by person for given time period
        /// </summary>
        public static MilestonePersonEntry[] GetCurrentMilestones(
            Person person, DateTime startDate, DateTime endDate,
            int defaultMilestoneId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.ConsultantMilestones, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, person.Id.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return ReadMilestonePersonEntries(reader).ToArray();
                }
            }
        }

        /// <summary>
        /// Get milestones by person for given time period exclusively for Time Entry page.
        /// </summary>
        public static MilestonePersonEntry[] GetTimeEntryMilestones(Person person, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.GetTimeEntryMilestones, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, person.Id.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return ReadMilestonePersonEntries(reader).ToArray();
                }
            }
        }

        /// <summary>
        /// Get milestones by person for given time period
        /// </summary>
        public static GroupedTimeEntries<Person> GetTimeEntriesByProject(TimeEntryProjectReportContext reportContext)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.TimeEntriesGetByProject, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                if (reportContext.PersonIds != null && reportContext.PersonIds.Count() > 0)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonIds, DataTransferObjects.Utils.Generic.EnumerableToCsv(reportContext.PersonIds, id => id));
                }
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, reportContext.ProjectId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, reportContext.StartDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, reportContext.EndDate);

                connection.Open();

                using (var reader = command.ExecuteReader())
                    return ReadMilestoneTimeEntryProjectPerson(reader);
            }
        }

        /// <summary>
        /// Get milestones by person for given time period
        /// </summary>
        public static GroupedTimeEntries<TimeEntryHours> GetTimeEntriesByProjectCumulative(TimeEntryPersonReportContext reportContext)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.TimeEntryHoursByManyPersonsProject, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIds, DataTransferObjects.Utils.Generic.EnumerableToCsv(reportContext.PersonIds, id => id));
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, reportContext.StartDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, reportContext.EndDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, DataTransferObjects.Utils.Generic.EnumerableToCsv(reportContext.PracticeIds, id => id));
                command.Parameters.AddWithValue(Constants.ParameterNames.TimescaleIds, DataTransferObjects.Utils.Generic.EnumerableToCsv(reportContext.PayTypeIds, id => id));

                connection.Open();

                using (var reader = command.ExecuteReader())
                    return ReadTimeEntryHours(reader);
            }
        }
        /// <summary>
        /// Get milestones by person for given time period
        /// </summary>
        public static GroupedTimeEntries<Project> GetTimeEntriesByPerson(TimeEntryPersonReportContext reportContext)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.TimeEntriesGetByManyPersons, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIds, DataTransferObjects.Utils.Generic.EnumerableToCsv(reportContext.PersonIds, id => id));
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, reportContext.StartDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, reportContext.EndDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam,DataTransferObjects.Utils.Generic.EnumerableToCsv(reportContext.PracticeIds, id => id));
                command.Parameters.AddWithValue(Constants.ParameterNames.TimescaleIds, DataTransferObjects.Utils.Generic.EnumerableToCsv(reportContext.PayTypeIds, id => id));

                connection.Open();

                using (var reader = command.ExecuteReader())
                    return ReadMilestoneTimeEntryPersonProject(reader);
            }
        }

        private static GroupedTimeEntries<Person> ReadMilestoneTimeEntryProjectPerson(SqlDataReader reader)
        {
            var result = new GroupedTimeEntries<Person>();

            if (reader.HasRows)
                while (reader.Read())
                    result.AddTimeEntry(ReadObjectPerson(reader), ReadTimeEntry(reader));

            return result;
        }

        private static GroupedTimeEntries<TimeEntryHours> ReadTimeEntryHours(SqlDataReader reader)
        {
            var result = new GroupedTimeEntries<TimeEntryHours>();

            if (reader.HasRows)
                while (reader.Read())
                    result.AddTimeEntry(ReadTeHours(reader), ReadTimeEntryMini(reader));

            return result;
        }

        private static TimeEntryHours ReadTeHours(SqlDataReader reader)
        {
            //int dateIndex;
            //int dayOffIndex;
            //int companyDayOffIndex;
            //int readOnlyIndex;
            //CalendarDAL.GetCalendarItemIndexes(reader, out dateIndex, out dayOffIndex, out companyDayOffIndex, out readOnlyIndex);

            return new TimeEntryHours
                       {
                           //Calendar = CalendarDAL.ReadSingleCalendarItem(reader, dateIndex, dayOffIndex, companyDayOffIndex, readOnlyIndex),
                           Id = reader.GetInt32(reader.GetOrdinal(Constants.ColumnNames.Id)),
                           Name = reader.GetString(reader.GetOrdinal(Constants.ColumnNames.Name))
                           +" - "+reader.GetString(reader.GetOrdinal(Constants.ColumnNames.TimeTypeName))                               
                       };
        }

        private static GroupedTimeEntries<Project> ReadMilestoneTimeEntryPersonProject(SqlDataReader reader)
        {
            var result = new GroupedTimeEntries<Project>();

            if (reader.HasRows)
                while (reader.Read())
                    result.AddTimeEntry(ReadProject(reader), ReadTimeEntryShort(reader));

            return result;
        }

        public static TimeEntryRecord[] GetAllTimeEntries(TimeEntrySelectContext selectContext)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.ListAll, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                if (selectContext.PersonIds != null && selectContext.PersonIds.Count > 0)
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonIds, string.Join(DataTransferObjects.Constants.Formatting.StringValueSeparator, selectContext.PersonIds.Select(i => i.ToString()).ToArray()));
                if (selectContext.MilestoneDateFrom.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneDateFrom, selectContext.MilestoneDateFrom.Value);
                if (selectContext.MilestoneDateTo.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneDateTo, selectContext.MilestoneDateTo.Value);
                if (selectContext.ForecastedHoursFrom.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.ForecastedHoursFrom,
                                                    selectContext.ForecastedHoursFrom.Value);
                if (selectContext.ForecastedHoursTo.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.ForecastedHoursTo, selectContext.ForecastedHoursTo.Value);
                if (selectContext.ActualHoursFrom.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.ActualHoursFrom, selectContext.ActualHoursFrom.Value);
                if (selectContext.ActualHoursTo.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.ActualHoursTo, selectContext.ActualHoursTo.Value);
                if (selectContext.ProjectIds != null && selectContext.ProjectIds.Count > 0)
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdList, string.Join(DataTransferObjects.Constants.Formatting.StringValueSeparator, selectContext.ProjectIds.Select(i => i.ToString()).ToArray()));
                if (selectContext.MilestonePersonId.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.MilestonePersonId, selectContext.MilestonePersonId.Value);
                if (selectContext.MilestoneId.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneId, selectContext.MilestoneId.Value);
                if (selectContext.TimeTypeId.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeId, selectContext.TimeTypeId.Value);
                if (selectContext.Notes != null)
                    command.Parameters.AddWithValue(Constants.ParameterNames.Notes, selectContext.Notes);
                if (selectContext.IsChargable.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsChargeable, selectContext.IsChargable.Value);
                else
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsChargeable, DBNull.Value);
                if (selectContext.IsCorrect.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsCorrect, selectContext.IsCorrect.Value);
                else
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsCorrect, DBNull.Value);
                if (selectContext.IsProjectChargeable.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsProjectChargeable, selectContext.IsProjectChargeable.Value);
                else
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsProjectChargeable, DBNull.Value);
                int? reviewed = Utils.ReviewStatus2Int(selectContext.IsReviewed);
                if (reviewed.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsReviewed, reviewed.Value);
                else
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsReviewed, DBNull.Value);
                if (selectContext.EntryDateFrom.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.EntryDateFrom, selectContext.EntryDateFrom.Value);
                if (selectContext.EntryDateTo.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.EntryDateTo, selectContext.EntryDateTo.Value);
                if (selectContext.ModifiedDateFrom.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.ModifiedDateFrom, selectContext.ModifiedDateFrom.Value);
                if (selectContext.ModifiedDateTo.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.ModifiedDateTo, selectContext.ModifiedDateTo.Value);
                if (selectContext.ModifiedBy.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.ModifiedBy, selectContext.ModifiedBy.Value);
                if (selectContext.SortExpression != null)
                    command.Parameters.AddWithValue(Constants.ParameterNames.SortExpression, selectContext.SortExpression);

                command.Parameters.AddWithValue(Constants.ParameterNames.RequesterId, selectContext.RequesterId);

                if (selectContext.PageNo.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.PageNo, selectContext.PageNo.Value);
                if (selectContext.PageSize.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.PageSize, selectContext.PageSize.Value);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return ReadPersonTimeEntries(reader);
                }
            }
        }

        public static TimeEntrySums GetTimeEntrySums(TimeEntrySelectContext selectContext)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.GetTotals, connection))
            {
                InitTimeEntryCommand(selectContext, command);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return ReadTimeEntrySums(reader);
                }
            }

            return null;
        }

        private static void InitTimeEntryCommand(TimeEntrySelectContext selectContext, SqlCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;

            if (selectContext.PersonIds != null && selectContext.PersonIds.Count > 0)
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIds, string.Join(DataTransferObjects.Constants.Formatting.StringValueSeparator, selectContext.PersonIds.Select(i => i.ToString()).ToArray()));
            if (selectContext.MilestoneDateFrom.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneDateFrom, selectContext.MilestoneDateFrom.Value);
            if (selectContext.MilestoneDateTo.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneDateTo, selectContext.MilestoneDateTo.Value);
            if (selectContext.ForecastedHoursFrom.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.ForecastedHoursFrom,
                                                selectContext.ForecastedHoursFrom.Value);
            if (selectContext.ForecastedHoursTo.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.ForecastedHoursTo, selectContext.ForecastedHoursTo.Value);
            if (selectContext.ActualHoursFrom.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.ActualHoursFrom, selectContext.ActualHoursFrom.Value);
            if (selectContext.ActualHoursTo.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.ActualHoursTo, selectContext.ActualHoursTo.Value);
            if (selectContext.ProjectIds != null && selectContext.ProjectIds.Count > 0)
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdList, string.Join(DataTransferObjects.Constants.Formatting.StringValueSeparator, selectContext.ProjectIds.Select(i => i.ToString()).ToArray()));
            if (selectContext.MilestonePersonId.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.MilestonePersonId, selectContext.MilestonePersonId.Value);
            if (selectContext.MilestoneId.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneId, selectContext.MilestoneId.Value);
            if (selectContext.TimeTypeId.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeId, selectContext.TimeTypeId.Value);
            if (selectContext.Notes != null)
                command.Parameters.AddWithValue(Constants.ParameterNames.Notes, selectContext.Notes);
            if (selectContext.IsChargable.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.IsChargeable, selectContext.IsChargable.Value);
            else
                command.Parameters.AddWithValue(Constants.ParameterNames.IsChargeable, DBNull.Value);
            if (selectContext.IsCorrect.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.IsCorrect, selectContext.IsCorrect.Value);
            else
                command.Parameters.AddWithValue(Constants.ParameterNames.IsCorrect, DBNull.Value);
            if (selectContext.IsProjectChargeable.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.IsProjectChargeable, selectContext.IsProjectChargeable.Value);
            else
                command.Parameters.AddWithValue(Constants.ParameterNames.IsProjectChargeable, DBNull.Value);
            int? reviewed = Utils.ReviewStatus2Int(selectContext.IsReviewed);
            if (reviewed.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.IsReviewed, reviewed.Value);
            else
                command.Parameters.AddWithValue(Constants.ParameterNames.IsReviewed, DBNull.Value);
            if (selectContext.EntryDateFrom.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.EntryDateFrom, selectContext.EntryDateFrom.Value);
            if (selectContext.EntryDateTo.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.EntryDateTo, selectContext.EntryDateTo.Value);
            if (selectContext.ModifiedDateFrom.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.ModifiedDateFrom, selectContext.ModifiedDateFrom.Value);
            if (selectContext.ModifiedDateTo.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.ModifiedDateTo, selectContext.ModifiedDateTo.Value);
            if (selectContext.ModifiedBy.HasValue)
                command.Parameters.AddWithValue(Constants.ParameterNames.ModifiedBy, selectContext.ModifiedBy.Value);
            command.Parameters.AddWithValue(Constants.ParameterNames.RequesterId, selectContext.RequesterId);
        }

        public static int GetTimeEntriesCount(TimeEntrySelectContext selectContext)
        {
            int timeEntriesCount = 0;
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.GetCount, connection))
            {
                InitTimeEntryCommand(selectContext, command);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        timeEntriesCount = Convert.ToInt32(reader[0]);
                    }
                }
            }

            return timeEntriesCount;
        }

        private static TimeEntrySums ReadTimeEntrySums(SqlDataReader reader)
        {
            if (reader.Read())
            {
                var actualIndex = reader.GetOrdinal(Constants.ColumnNames.TotalActualHours);
                var forecastIndex = reader.GetOrdinal(Constants.ColumnNames.TotalForecastedHours);

                return new TimeEntrySums
                    {
                        TotalActualHours = reader.IsDBNull(actualIndex) ? 0.0 : reader.GetDouble(actualIndex),
                        TotalForecastedHours = reader.IsDBNull(forecastIndex) ? 0.0 : reader.GetDouble(forecastIndex)
                    };
            }

            return null;
        }

        private static Person[] ReadObjectPersons(SqlDataReader reader)
        {
            var result = new List<Person>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    result.Add(ReadObjectPerson(reader));
                }
            }

            return result.ToArray();
        }

        private static Milestone[] ReadMilestones(SqlDataReader reader)
        {
            var result = new List<Milestone>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Project project = ReadProject(reader);
                    Milestone milestone = ReadMilestone(reader);
                    milestone.Project = project;

                    result.Add(milestone);
                }
            }

            return result.ToArray();
        }

        private static TimeEntryRecord[] ReadPersonTimeEntries(SqlDataReader reader)
        {
            var result = new List<TimeEntryRecord>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Client client = ReadClient(reader);

                    // Project details
                    Project project = ReadProject(reader);
                    project.Client = client;
                    //ProjectStatus projectStatus = ReadProjectStatus(reader);
                    //project.Status = projectStatus;


                    // Milestone details
                    Milestone milestone = ReadMilestone(reader);
                    milestone.Project = project;

                    MilestonePersonEntry entry = ReadMilestonePerson(reader);
                    entry.ParentMilestone = milestone;

                    TimeEntryRecord te = ReadTimeEntry(reader);
                    te.TimeType = ReadTimeType(reader);
                    te.ParentMilestonePersonEntry = entry;
                    te.ParentMilestonePersonEntry.ThisPerson = ReadObjectPerson(reader);

                    result.Add(te);
                }
            }

            return result.ToArray();
        }

        private static List<MilestonePersonEntry> ReadMilestonePersonEntries(DbDataReader reader)
        {
            var result = new List<MilestonePersonEntry>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Client client = ReadClient(reader);

                    // Project details
                    Project project = ReadProject(reader);
                    ProjectStatus projectStatus = ReadProjectStatus(reader);
                    project.Status = projectStatus;
                    project.Client = client;

                    // Milestone details
                    Milestone milestone = ReadMilestone(reader);
                    milestone.Project = project;
                    milestone.ConsultantsCanAdjust =
                        reader.GetBoolean(
                            reader.GetOrdinal(Constants.ColumnNames.ConsultantsCanAdjust));
                    milestone.IsChargeable =
                        reader.GetBoolean(
                            reader.GetOrdinal(Constants.ColumnNames.IsChargeable));

                    // Person on milestone
                    MilestonePersonEntry entry = ReadMilestonePerson(reader);
                    entry.ParentMilestone = milestone;

                    result.Add(entry);
                }
            }

            return result;
        }


        private static TimeEntryRecord[] ReadTimeEntries(SqlDataReader reader)
        {
            var result = new List<TimeEntryRecord>();

            if (reader.HasRows)
                while (reader.Read())
                    result.Add(ReadTimeEntry(reader));

            return result.ToArray();
        }

        private static TimeEntryRecord ReadTimeEntry(SqlDataReader reader)
        {
            var teIdIndex = reader.GetOrdinal(Constants.ParameterNames.TimeEntryId);
            var noteIndex = reader.GetOrdinal(Constants.ParameterNames.Note);
            var timeTypeIdIndex = reader.GetOrdinal(Constants.ParameterNames.TimeTypeId);
            var entryDateIndex = reader.GetOrdinal(Constants.ParameterNames.EntryDate);
            var milestoneDateIndex = reader.GetOrdinal(Constants.ParameterNames.MilestoneDate);
            var modifiedDateIndex = reader.GetOrdinal(Constants.ParameterNames.ModifiedDate);
            var actualHrsIndex = reader.GetOrdinal(Constants.ParameterNames.ActualHours);
            var forecastedHrsIndex = reader.GetOrdinal(Constants.ParameterNames.ForecastedHours);
            var milestonePersonIdIndex = reader.GetOrdinal(Constants.ParameterNames.MilestonePersonId);
            var isChargeableIndex = reader.GetOrdinal(Constants.ParameterNames.IsChargeable);
            var isCorrectIndex = reader.GetOrdinal(Constants.ParameterNames.IsCorrect);
            var isReviewedIndex = reader.GetOrdinal(Constants.ParameterNames.IsReviewed);

            var timeEntry = new TimeEntryRecord
                                {
                                    Id = reader.GetInt32(teIdIndex),
                                    Note = reader.GetString(noteIndex),
                                    EntryDate = reader.GetDateTime(entryDateIndex),
                                    MilestoneDate = reader.GetDateTime(milestoneDateIndex),
                                    TimeType = new TimeTypeRecord(reader.GetInt32(timeTypeIdIndex)),
                                    ActualHours = reader.GetFloat(actualHrsIndex),
                                    ForecastedHours = reader.GetFloat(forecastedHrsIndex),
                                    ModifiedDate = reader.GetDateTime(modifiedDateIndex),
                                    ModifiedBy = ReadModifiedBy(reader),
                                    ParentMilestonePersonEntry =
                                        new MilestonePersonEntry(reader.GetInt32(milestonePersonIdIndex)),
                                    IsChargeable = reader.GetBoolean(isChargeableIndex),
                                    IsCorrect = reader.GetBoolean(isCorrectIndex),
                                    IsReviewed = reader.IsDBNull(isReviewedIndex)
                                                     ? ReviewStatus.Pending
                                                     : Utils.Bool2ReviewStatus(reader.GetBoolean(isReviewedIndex))
                                };

            return timeEntry;
        }

        private static TimeEntryRecord ReadTimeEntryShort(SqlDataReader reader)
        {
            var noteIndex = reader.GetOrdinal(Constants.ParameterNames.Note);
            var timeTypeNameIndex = reader.GetOrdinal(Constants.ParameterNames.TimeTypeName);
            var milestoneDateIndex = reader.GetOrdinal(Constants.ParameterNames.MilestoneDate);
            var actualHrsIndex = reader.GetOrdinal(Constants.ParameterNames.ActualHours);
            var milestonePersonIdIndex = reader.GetOrdinal(Constants.ParameterNames.MilestonePersonId);
            var firstNameIndex = reader.GetOrdinal(Constants.ParameterNames.ObjectFirstName);
            var lastNameIndex = reader.GetOrdinal(Constants.ParameterNames.ObjectLastName);
            var personIdIndex = reader.GetOrdinal(Constants.ParameterNames.PersonId);

            var timeEntry = new TimeEntryRecord();
            timeEntry.Note = reader.IsDBNull(noteIndex) ? string.Empty : reader.GetString(noteIndex);
            if (!reader.IsDBNull(noteIndex))
            {
                timeEntry.MilestoneDate = reader.GetDateTime(milestoneDateIndex);
            }
            timeEntry.TimeType = new TimeTypeRecord { Name = reader.IsDBNull(noteIndex) ? string.Empty : reader.GetString(timeTypeNameIndex) };

            if (!reader.IsDBNull(actualHrsIndex))
            {
                timeEntry.ActualHours = reader.GetFloat(actualHrsIndex);
            }

            timeEntry.ParentMilestonePersonEntry = new MilestonePersonEntry(reader.GetInt32(milestonePersonIdIndex))
                                                        {
                                                            ThisPerson = new Person()
                                                            {
                                                                FirstName = reader.GetString(firstNameIndex),
                                                                LastName = reader.GetString(lastNameIndex),
                                                                Id = reader.GetInt32(personIdIndex)
                                                            }
                                                        };

            return timeEntry;
        }

        private static TimeEntryRecord ReadTimeEntryMini(SqlDataReader reader)
        {
            var milestoneDateIndex = reader.GetOrdinal(Constants.ParameterNames.MilestoneDate);
            var actualHrsIndex = reader.GetOrdinal(Constants.ParameterNames.ActualHours);
            // var timetypeIndex = reader.GetOrdinal(Constants.ParameterNames.TimeTypeName);
            var personIdIndex = reader.GetOrdinal(Constants.ParameterNames.PersonId);
            var timeEntry = new TimeEntryRecord
            {
                MilestoneDate = reader.GetDateTime(milestoneDateIndex),
                ActualHours = reader.GetDouble(actualHrsIndex),
                ParentMilestonePersonEntry = new MilestonePersonEntry() { ThisPerson = new Person() { Id = reader.GetInt32(personIdIndex) } }
                // TimeType = new TimeTypeRecord { Name = reader.GetString(timetypeIndex) }
            };

            return timeEntry;
        }

        private static MilestonePersonEntry ReadMilestonePerson(DbDataReader reader)
        {
            var entry = new MilestonePersonEntry();
            int milestonePersonIdIndex = reader.GetOrdinal(Constants.ParameterNames.MilestonePersonId);
            entry.MilestonePersonId = reader.GetInt32(milestonePersonIdIndex);
            int startDateIndex = reader.GetOrdinal(Constants.ParameterNames.StartDate);
            entry.StartDate = reader.GetDateTime(startDateIndex);
            int endDateIndex = reader.GetOrdinal(Constants.ParameterNames.EndDate);
            entry.EndDate = !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null;
            int hoursPerDayIndex = reader.GetOrdinal(Constants.ParameterNames.HoursPerDay);
            entry.HoursPerDay = reader.GetDecimal(hoursPerDayIndex);
            return entry;
        }

        private static Milestone ReadMilestone(DbDataReader reader)
        {
            var milestone = new Milestone
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal(Constants.ParameterNames.MilestoneId)),
                                    Description =
                                        reader.GetString(reader.GetOrdinal(Constants.ParameterNames.MilestoneName))
                                };
            return milestone;
        }

        private static TimeTypeRecord ReadTimeType(DbDataReader reader)
        {
            // Client details
            var tt = new TimeTypeRecord
                         {
                             Id = reader.GetInt32(reader.GetOrdinal(Constants.ParameterNames.TimeTypeId)),
                             Name = reader.GetString(reader.GetOrdinal(Constants.ParameterNames.TimeTypeName))
                         };
            return tt;
        }

        private static Client ReadClient(DbDataReader reader)
        {
            // Client details
            var client = new Client
                             {
                                 Id = reader.GetInt32(reader.GetOrdinal(Constants.ParameterNames.ClientId)),
                                 Name = reader.GetString(reader.GetOrdinal(Constants.ParameterNames.ClientName))
                             };
            return client;
        }

        private static ProjectStatus ReadProjectStatus(DbDataReader reader)
        {
            var projectStatus = new ProjectStatus
                                    {
                                        Id =
                                            reader.GetInt32(reader.GetOrdinal(Constants.ParameterNames.ProjectStatusId))
                                    };
            return projectStatus;
        }

        private static Project ReadProject(DbDataReader reader)
        {
            var project = new Project
                              {
                                  Id = reader.GetInt32(reader.GetOrdinal(Constants.ParameterNames.ProjectId)),
                                  Name = reader.GetString(reader.GetOrdinal(Constants.ParameterNames.ProjectName)),
                                  ProjectNumber = reader.GetString(reader.GetOrdinal(Constants.ParameterNames.ProjectNumber))
                              };
            try
            {

                project.Client = new Client { Id = reader.GetInt32(reader.GetOrdinal(Constants.ParameterNames.ClientId)) };
            }
            catch
            {
            }
            return project;
        }

        private static Person ReadObjectPerson(DbDataReader reader)
        {
            var person = new Person
                             {
                                 Id = reader.GetInt32(reader.GetOrdinal(Constants.ParameterNames.PersonId)),
                                 FirstName =
                                     reader.GetString(reader.GetOrdinal(Constants.ParameterNames.ObjectFirstName)),
                                 LastName = reader.GetString(reader.GetOrdinal(Constants.ParameterNames.ObjectLastName))
                             };
            return person;
        }

        private static Person ReadModifiedBy(DbDataReader reader)
        {
            var person = new Person
                             {
                                 Id = reader.GetInt32(reader.GetOrdinal(Constants.ParameterNames.ModifiedBy)),
                                 FirstName =
                                     reader.GetString(reader.GetOrdinal(Constants.ParameterNames.ModifiedFirstName)),
                                 LastName =
                                     reader.GetString(reader.GetOrdinal(Constants.ParameterNames.ModifiedLastName))
                             };
            return person;
        }

        #region Filtering

        public static Project[] GetTimeEntryProjectsByClientId(int? clientId)
        {
            var allMilestones = GetTimeEntryMilestonesByClientId(clientId);
            var result = new List<Project>(allMilestones.Length);

            foreach (var m in allMilestones)
            {
                var project = m.Project;
                if (!result.Contains(project))
                    result.Add(project);
            }

            return result.ToArray();
            
        }

        /// <summary>
        /// Gets all projects that have TE records assigned to them
        /// </summary>
        /// <returns>Projects list</returns>
        public static Project[] GetAllTimeEntryProjects()
        {
            var allMilestones = GetAllTimeEntryMilestones();
            var result = new List<Project>(allMilestones.Length);

            foreach (var m in allMilestones)
            {
                var project = m.Project;
                if (!result.Contains(project))
                    result.Add(project);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Gets all milestones that have TE records assigned to  particular clientId
        /// </summary>
        /// <returns>Milestones list</returns>
        public static Milestone[] GetTimeEntryMilestonesByClientId(int? clientId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.TimeEntryAllMilestones, connection))
            {
                if (clientId != null)
                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientId, clientId);

                command.CommandType = CommandType.StoredProcedure;
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return ReadMilestones(reader);
                }
            }
        }

        /// <summary>
        /// Gets all milestones that have TE records assigned to them
        /// </summary>
        /// <returns>Milestones list</returns>
        public static Milestone[] GetAllTimeEntryMilestones()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.TimeEntryAllMilestones, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return ReadMilestones(reader);
                }
            }
        }

        /// <summary>
        /// Gets all persons that have entered at least one TE
        /// </summary>
        /// <returns>List of persons</returns>
        public static Person[] GetAllTimeEntryPersons(DateTime entryDateFrom, DateTime entryDateTo)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.TimeEntryAllPersons, connection))
            {
                if (entryDateFrom != null)
                    command.Parameters.AddWithValue(Constants.ParameterNames.EntryStartDateParam, entryDateFrom);

                if (entryDateTo != null)
                    command.Parameters.AddWithValue(Constants.ParameterNames.EntryEndDateParam, entryDateTo);

                command.CommandType = CommandType.StoredProcedure;
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return ReadObjectPersons(reader);
                }
            }
        }

        #endregion

       
    }
}


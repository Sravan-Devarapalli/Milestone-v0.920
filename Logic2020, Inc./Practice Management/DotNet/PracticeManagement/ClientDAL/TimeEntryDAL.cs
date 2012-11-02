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

        #region Time Zone


        #region Constants

        private const string TimeZonesAllProcedure = "dbo.TimeZonesAll";
        private const string SetTimeZoneProcedure = "dbo.SetTimeZone";

        private const string IdParameter = "@Id";
        private const string GMTParameter = "@GMT";

        private const string TimeZoneIdColumn = "Id";
        private const string TimeZoneGMTColumn = "GMT";
        private const string TimeZoneGMTNameColumn = "GMTName";
        private const string TimeZoneIsActiveColumn = "IsActive";

        #endregion

        public static void SetTimeZone(Timezone timezone)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(SetTimeZoneProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue(GMTParameter, timezone.GMT);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<Timezone> TimeZonesAll()
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(TimeZonesAllProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Timezone> result = new List<Timezone>();

                        ReadTimeZones(reader, result);

                        return result;
                    }
                }
            }
        }

        private static void ReadTimeZones(SqlDataReader reader, List<Timezone> result)
        {
            if (reader.HasRows)
            {
                int timeZoneIdIndex = reader.GetOrdinal(TimeZoneIdColumn);
                int timeZoneGMTIndex = reader.GetOrdinal(TimeZoneGMTColumn);
                int timeZoneGMTNameIndex = reader.GetOrdinal(TimeZoneGMTNameColumn);
                int timeZoneIsActiveIndex = reader.GetOrdinal(TimeZoneIsActiveColumn);

                while (reader.Read())
                {
                    Timezone timeZone = new Timezone();

                    timeZone.Id = reader.GetInt32(timeZoneIdIndex);
                    timeZone.GMT = reader.GetString(timeZoneGMTIndex);
                    timeZone.GMTName = reader.GetString(timeZoneGMTNameIndex);
                    timeZone.IsActive = reader.GetBoolean(timeZoneIsActiveIndex);

                    result.Add(timeZone);
                }
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
                command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneId, reportContext.MilestoneId.HasValue ? (object)reportContext.MilestoneId : DBNull.Value);

                connection.Open();

                using (var reader = command.ExecuteReader())
                    return ReadMilestoneTimeEntryProjectPerson(reader);
            }
        }


        public static bool CheckPersonTimeEntriesAfterTerminationDate(int personId, DateTime terminationDate)
        {

            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.TimeEntry.CheckPersonTimeEntriesAfterTerminationDate, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.TerminationDate, terminationDate);
                connection.Open();
                return ((bool)command.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get milestones by person for given time period
        /// </summary>
        public static PersonTimeEntries GetTimeEntriesByPerson(TimeEntryPersonReportContext reportContext)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.TimeEntriesGetByPersonId, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, reportContext.PersonId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, reportContext.StartDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, reportContext.EndDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, reportContext.PracticeIds != null ? (object)DataTransferObjects.Utils.Generic.EnumerableToCsv(reportContext.PracticeIds, id => id) : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimescaleIds, reportContext.PayTypeIds != null ? (object)DataTransferObjects.Utils.Generic.EnumerableToCsv(reportContext.PayTypeIds, id => id) : DBNull.Value);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = ReadTimeEntryByPerson(reader);

                    reader.NextResult();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                            var lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);

                            var person = new Person()
                            {
                                FirstName = reader.GetString(firstNameIndex),
                                LastName = reader.GetString(lastNameIndex),
                                Id = reportContext.PersonId
                            };

                            result.Person = person;
                        }
                    }

                    return result;
                }
            }
        }


        public static DataSet TimeEntriesByPersonGetExcelSet(TimeEntryPersonReportContext reportContext)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.TimeEntriesGetByPersonsForExcel, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIds, DataTransferObjects.Utils.Generic.EnumerableToCsv(reportContext.PersonIds, id => id));
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, reportContext.StartDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, reportContext.EndDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, reportContext.PracticeIds != null ? (object)DataTransferObjects.Utils.Generic.EnumerableToCsv(reportContext.PracticeIds, id => id) : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimescaleIds, reportContext.PayTypeIds != null ? (object)DataTransferObjects.Utils.Generic.EnumerableToCsv(reportContext.PayTypeIds, id => id) : DBNull.Value);

                connection.Open();

                var adapter = new SqlDataAdapter(command);
                var dataset = new DataSet();
                adapter.Fill(dataset, "excelDataTable");
                return dataset;

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
                Name = reader.GetString(reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn))
                + " - " + reader.GetString(reader.GetOrdinal(Constants.ColumnNames.Name))
                + " - " + reader.GetString(reader.GetOrdinal(Constants.ColumnNames.TimeTypeName))

            };
        }

        private static PersonTimeEntries ReadTimeEntryByPerson(SqlDataReader reader)
        {
            var result = new PersonTimeEntries();

            if (reader.HasRows)
                while (reader.Read())
                {
                    int timeTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeId);
                    int timeTypeNameIndex = reader.GetOrdinal(Constants.ParameterNames.TimeTypeName);
                    int chargeCodeIdIndex = reader.GetOrdinal(Constants.ColumnNames.ChargeCodeId);
                    int clientIdIndex = reader.GetOrdinal(Constants.ParameterNames.ClientId);
                    int groupIdIndex = reader.GetOrdinal(Constants.ParameterNames.GroupIdColumn);
                    
                    var project = new Project
                    {
                        Id = (int)reader[Constants.ColumnNames.ProjectIdColumn],
                        Name = reader.GetString(reader.GetOrdinal(Constants.ParameterNames.ProjectName)),
                        ProjectNumber = reader.GetString(reader.GetOrdinal(Constants.ParameterNames.ProjectNumber))
                    };

                    var client = new Client
                    {
                        Id = reader.GetInt32(clientIdIndex),
                        Name = reader.GetString(reader.GetOrdinal(Constants.ParameterNames.ClientName))
                    };

                    var projectGroup = new ProjectGroup()
                    {
                        Id = reader.GetInt32(groupIdIndex),
                        Name = reader.GetString(reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn))
                    };

                    var timeType = new TimeTypeRecord
                    {
                        Id = reader.GetInt32(timeTypeIdIndex),
                        Name = reader.GetString(timeTypeNameIndex)
                    };

                    var chargeCode = new ChargeCode()
                    {
                        ChargeCodeId = reader.GetInt32(chargeCodeIdIndex),
                        Project = project,
                        Client = client,
                        ProjectGroup = projectGroup,
                        TimeType = timeType
                    };

                    var ter = ReadTimeEntryShort(reader);

                    ter.ChargeCode = chargeCode;

                    result.AddTimeEntry(ter);

                }

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

                    MilestonePersonEntry entry = ReadMilestonePersonEntryShort(reader);
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

            var isAllowedToEditIndex = -1;
            try
            {
                isAllowedToEditIndex = reader.GetOrdinal(Constants.ColumnNames.IsAllowedToEditColumn);
            }
            catch
            {
                isAllowedToEditIndex = -1;
            }

            var timeType = new TimeTypeRecord();
            timeType.Id = reader.GetInt32(timeTypeIdIndex);

            if (isAllowedToEditIndex != -1)
            {
                try
                {
                    timeType.IsAllowedToEdit = reader.GetBoolean(isAllowedToEditIndex);
                }
                catch
                { }
            }

            var timeEntry = new TimeEntryRecord
            {
                Id = reader.GetInt32(teIdIndex),
                Note = reader.GetString(noteIndex),
                EntryDate = reader.GetDateTime(entryDateIndex),
                ChargeCodeDate = reader.GetDateTime(milestoneDateIndex),
                TimeType = timeType,
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
            var chargeCodeDateIndex = reader.GetOrdinal(Constants.ColumnNames.ChargeCodeDate);
            int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
            int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);



            var timeEntry = new TimeEntryRecord();
            timeEntry.Note = reader.IsDBNull(noteIndex) ? string.Empty : reader.GetString(noteIndex);
            timeEntry.ChargeCodeDate = reader.GetDateTime(chargeCodeDateIndex);




            timeEntry.BillableHours = reader.GetDouble(billableHoursIndex);
            timeEntry.NonBillableHours = reader.GetDouble(nonBillableHoursIndex);


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
                ChargeCodeDate = reader.GetDateTime(milestoneDateIndex),
                ActualHours = reader.GetDouble(actualHrsIndex),
                ParentMilestonePersonEntry = new MilestonePersonEntry() { ThisPerson = new Person() { Id = reader.GetInt32(personIdIndex) } }
                // TimeType = new TimeTypeRecord { Name = reader.GetString(timetypeIndex) }
            };

            return timeEntry;
        }

        private static MilestonePersonEntry ReadMilestonePersonEntryShort(DbDataReader reader)
        {
            var entry = new MilestonePersonEntry();
            int milestonePersonIdIndex = reader.GetOrdinal(Constants.ParameterNames.MilestonePersonId);
            entry.MilestonePersonId = reader.GetInt32(milestonePersonIdIndex);
            return entry;
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

        private static Project ReadProject(DbDataReader reader, bool isReadClientId = true)
        {
            var project = new Project
            {
                Id = reader.GetInt32(reader.GetOrdinal(Constants.ParameterNames.ProjectId)),
                Name = reader.GetString(reader.GetOrdinal(Constants.ParameterNames.ProjectName)),
                ProjectNumber = reader.GetString(reader.GetOrdinal(Constants.ParameterNames.ProjectNumber))
            };

            if (isReadClientId)
                ReadClientId(project, reader);

            return project;
        }

        private static void ReadClientId(Project project, DbDataReader reader)
        {
            try
            {

                project.Client = new Client { Id = reader.GetInt32(reader.GetOrdinal(Constants.ParameterNames.ClientId)) };
            }
            catch
            {
            }
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


        #region TimeTrack Methods

        public static void DeleteTimeEntry(int clientId, int projectId, int personId, int timetypeId, DateTime startDate, DateTime endDate, string userName)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.DeleteTimeEntryProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.ClientId, clientId);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, projectId);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeId, timetypeId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userName);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void SaveTimeTrack(string timeEntriesXml, int personId, DateTime startDate, DateTime endDate, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.SaveTimeTrackProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeEntriesXmlParam, timeEntriesXml);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void SetPersonTimeEntryRecursiveSelection(int personId, int clientId, int projectGroupId, int projectId, int timeEntrySectionId, bool isRecursive, DateTime startDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.SetPersonTimeEntryRecursiveSelectionProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.ClientId, clientId);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectGroupIdParam, projectGroupId);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, projectId);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeEntrySectionIdParam, timeEntrySectionId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsRecursiveParam, isRecursive);

                connection.Open();
                command.ExecuteNonQuery();
            }

        }

        public static void SetPersonTimeEntrySelection(int personId, int clientId, int projectGroupId, int projectId, int timeEntrySectionId, bool isDelete, DateTime startDate, DateTime endDate, string userName)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.SetPersonTimeEntrySelectionProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.ClientId, clientId);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectGroupIdParam, projectGroupId);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, projectId);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeEntrySectionIdParam, timeEntrySectionId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsDeleteParam, isDelete);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userName);

                connection.Open();
                command.ExecuteNonQuery();
            }

        }

        public static Dictionary<DateTime, bool> GetIsChargeCodeTurnOffByPeriod(int personId, int clientId, int groupId, int projectId, int timeTypeId, DateTime startDate, DateTime endDate)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.TimeEntry.GetIsChargeCodeTurnOffByPeriodProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientId, clientId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectGroupIdParam, groupId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, projectId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeId, timeTypeId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        Dictionary<DateTime, bool> result = new Dictionary<DateTime, bool>();
                        if (reader.HasRows)
                        {
                            var chargeCodeDateIndex = reader.GetOrdinal(Constants.ColumnNames.ChargeCodeDate);
                            var isChargeCodeOffIndex = reader.GetOrdinal(Constants.ColumnNames.IsChargeCodeOffColumn);

                            while (reader.Read())
                            {
                                DateTime key = reader.GetDateTime(chargeCodeDateIndex);
                                bool value = reader.GetBoolean(isChargeCodeOffIndex);
                                result.Add(key, value);
                            }
                        }
                        return result;
                    }
                }
            }
        }



        #endregion

        #region Filtering

        public static Project[] GetTimeEntryProjectsByClientId(int? clientId, int? personId = null, bool showActiveAndInternalProjectsOnly = false)
        {
            var allMilestones = GetTimeEntryMilestonesByClientId(clientId, personId, showActiveAndInternalProjectsOnly);
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
        public static Milestone[] GetTimeEntryMilestonesByClientId(int? clientId, int? personId = null, bool showActiveAndInternalProjectsOnly = false)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.TimeEntryAllMilestonesByClientId, connection))
            {
                if (clientId != null)
                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientId, clientId);

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId.HasValue ? (object)personId.Value : DBNull.Value);

                command.Parameters.AddWithValue(Constants.ParameterNames.ShowAll, !showActiveAndInternalProjectsOnly);

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

        public static bool HasTimeEntriesForMilestone(int milestoneId, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.HasTimeEntriesForMilestoneBetweenOldAndNewDates, connection))
            {
                bool result = false;
                command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneIdParam, milestoneId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);

                command.CommandType = CommandType.StoredProcedure;
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = reader.GetBoolean(reader.GetOrdinal(Constants.ParameterNames.HasTimeEntries));
                        }
                    }
                    return result;
                }
            }
        }

        public static List<TimeEntrySection> PersonTimeEntriesByPeriod(int personId, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.PersonTimeEntriesByPeriod, connection))
            {
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);

                command.CommandType = CommandType.StoredProcedure;
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var timeEntries = new List<TimeEntryRecord>();
                    ReadTimeEntries(reader, timeEntries);

                    var timeEntrySections = new List<TimeEntrySection>();
                    reader.NextResult();
                    ReadTimeEntriesSections(reader, timeEntrySections, timeEntries);

                    reader.NextResult();
                    ReadTimeOffAttributes(reader, timeEntrySections);


                    return timeEntrySections;
                }
            }
        }

        private static void ReadTimeOffAttributes(SqlDataReader reader, List<TimeEntrySection> timeEntrySections)
        {
            if (reader.HasRows)
            {
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                int timeTypeIdIndex = reader.GetOrdinal(Constants.ParameterNames.TimeTypeId);
                int isPTOIndex = reader.GetOrdinal(Constants.ColumnNames.IsPTOColumn);
                int isSickLeaveIndex = reader.GetOrdinal(Constants.ColumnNames.IsSickLeaveColumn);
                int isHolidayIndex = reader.GetOrdinal(Constants.ColumnNames.IsHolidayColumn);
                int isORTIndex = reader.GetOrdinal(Constants.ColumnNames.IsORTColumn);
                int isUnpaidIndex = reader.GetOrdinal(Constants.ColumnNames.IsUnpaidColoumn);


                while (reader.Read())
                {
                    if (timeEntrySections.Any(tes => tes.SectionId == TimeEntrySectionType.Administrative && tes.Project.Id == reader.GetInt32(projectIdIndex) && (tes.TimeEntries == null || (tes.TimeEntries != null && tes.TimeEntries[0].TimeType.Id == reader.GetInt32(timeTypeIdIndex)))))
                    {
                        var tesection = timeEntrySections.First(tes => tes.SectionId == TimeEntrySectionType.Administrative && tes.Project.Id == reader.GetInt32(projectIdIndex) && (tes.TimeEntries == null || (tes.TimeEntries != null && tes.TimeEntries[0].TimeType.Id == reader.GetInt32(timeTypeIdIndex))));

                        if ((reader.GetInt32(isPTOIndex) == 1))
                        {
                            tesection = timeEntrySections.First(tes => tes.SectionId == TimeEntrySectionType.Administrative && !tes.Project.IsSickLeaveProject &&tes.Project.Id == reader.GetInt32(projectIdIndex) && (tes.TimeEntries == null || (tes.TimeEntries != null && tes.TimeEntries[0].TimeType.Id == reader.GetInt32(timeTypeIdIndex))));
                            tesection.Project.IsPTOProject = true;
                        }

                        if ((reader.GetInt32(isSickLeaveIndex) == 1))
                        {
                            tesection = timeEntrySections.First(tes => tes.SectionId == TimeEntrySectionType.Administrative && !tes.Project.IsPTOProject && tes.Project.Id == reader.GetInt32(projectIdIndex) && (tes.TimeEntries == null || (tes.TimeEntries != null && tes.TimeEntries[0].TimeType.Id == reader.GetInt32(timeTypeIdIndex))));
                            tesection.Project.IsSickLeaveProject = true;
                        }

                        tesection.Project.IsHolidayProject = (reader.GetInt32(isHolidayIndex) == 1);
                        tesection.Project.IsORTProject = (reader.GetInt32(isORTIndex) == 1);
                        tesection.Project.IsUnpaidProject = (reader.GetInt32(isUnpaidIndex) == 1);
                        tesection.Project.TimeTypeId = reader.GetInt32(timeTypeIdIndex);
                    }
                }
            }

        }



        public static void ReadTimeEntriesSections(SqlDataReader reader, List<TimeEntrySection> timeEntrySections, List<TimeEntryRecord> timeEntries)
        {
            if (reader.HasRows)
            {
                int timeEntrySectionIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeEntrySectionId);
                int chargeCodeIdIndex = reader.GetOrdinal(Constants.ColumnNames.ChargeCodeId);
                int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
                int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
                int projectGroupIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn);
                int projectGroupNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn);
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNameColumn);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
                int isRecursiveIndex = reader.GetOrdinal(Constants.ColumnNames.IsRecursive);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
                int isNoteRequiredIndex = reader.GetOrdinal(Constants.ColumnNames.IsNoteRequired);

                while (reader.Read())
                {
                    var timeEntrySection = new TimeEntrySection
                    {
                        SectionId = ((TimeEntrySectionType)Enum.Parse(typeof(TimeEntrySectionType), reader.GetInt32(timeEntrySectionIdIndex).ToString())),
                        Account = new Client { Id = reader.GetInt32(clientIdIndex), Name = reader.GetString(clientNameIndex) },
                        Project = new Project
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            Name = reader.GetString(projectNameIndex),
                            ProjectNumber = reader.GetString(projectNumberIndex),
                            EndDate = reader.IsDBNull(endDateIndex) ? null : (DateTime?)reader.GetDateTime(endDateIndex),
                            IsNoteRequired = reader.GetBoolean(isNoteRequiredIndex)
                        },
                        BusinessUnit = reader.IsDBNull(projectGroupIdIndex) ? null
                                        : new ProjectGroup { Id = reader.GetInt32(projectGroupIdIndex), Name = reader.GetString(projectGroupNameIndex) },
                        IsRecursive = reader.GetInt32(isRecursiveIndex) > 0

                    };


                    if (timeEntrySections.Any(tes => tes.SectionId != TimeEntrySectionType.Administrative && tes.Project.Id == timeEntrySection.Project.Id && tes.Account.Id == timeEntrySection.Account.Id && (timeEntrySection.BusinessUnit != null && tes.BusinessUnit.Id.Value == timeEntrySection.BusinessUnit.Id.Value)))
                    {
                        timeEntrySection = timeEntrySections.First(tes => tes.Project.Id == timeEntrySection.Project.Id && tes.Account.Id == timeEntrySection.Account.Id && (tes.BusinessUnit.Id.Value == timeEntrySection.BusinessUnit.Id.Value));
                    }
                    else
                    {
                        timeEntrySections.Add(timeEntrySection);
                    }

                    var chargeCodeId = !reader.IsDBNull(chargeCodeIdIndex) ? ((int?)reader.GetInt32(chargeCodeIdIndex)) : null;

                    var tentries = timeEntries.Where(te => te.ChargeCodeId == chargeCodeId);

                    if (tentries.Count() > 0)
                    {
                        timeEntrySection.TimeEntries = timeEntrySection.TimeEntries ?? new List<TimeEntryRecord>();

                        timeEntrySection.TimeEntries.AddRange(tentries);
                    }

                }
            }
        }

        public static void ReadTimeEntries(SqlDataReader reader, List<TimeEntryRecord> timeEntries)
        {
            if (reader.HasRows)
            {
                var teIdIndex = reader.GetOrdinal(Constants.ParameterNames.TimeEntryId);
                var noteIndex = reader.GetOrdinal(Constants.ParameterNames.Note);
                var timeTypeIdIndex = reader.GetOrdinal(Constants.ParameterNames.TimeTypeId);
                var chargeCodeIdIndex = reader.GetOrdinal(Constants.ColumnNames.ChargeCodeId);
                var chargeCodeDateIndex = reader.GetOrdinal(Constants.ColumnNames.ChargeCodeDate);
                var createDateIndex = reader.GetOrdinal(Constants.ColumnNames.CreateDateColumn);
                var modifiedDateIndex = reader.GetOrdinal(Constants.ParameterNames.ModifiedDate);
                var actualHrsIndex = reader.GetOrdinal(Constants.ParameterNames.ActualHours);
                var forecastedHrsIndex = reader.GetOrdinal(Constants.ParameterNames.ForecastedHours);
                var isChargeableIndex = reader.GetOrdinal(Constants.ParameterNames.IsChargeable);
                var isCorrectIndex = -1;
                var revieweStatusIdIndex = reader.GetOrdinal(Constants.ParameterNames.ReviewStatusId);
                int approvedByIdIndex = -1;
                int approvedByFirstNameIndex = -1;
                int approvedByLastNameIndex = -1;
                try
                {
                    approvedByIdIndex = reader.GetOrdinal(Constants.ColumnNames.ApprovedByColumn);
                    approvedByFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.ApprovedByFirstNameColumn);
                    approvedByLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.ApprovedByLastNameColumn);
                }
                catch
                {
                    approvedByIdIndex = -1;
                    approvedByFirstNameIndex = -1;
                    approvedByLastNameIndex = -1;
                }

                try
                {
                    isCorrectIndex = reader.GetOrdinal(Constants.ParameterNames.IsCorrect);
                }
                catch
                {
                    isCorrectIndex = -1;
                }

                while (reader.Read())
                {

                    var timeType = new TimeTypeRecord();
                    timeType.Id = reader.GetInt32(timeTypeIdIndex);

                    var timeEntry = new TimeEntryRecord
                    {
                        Id = reader.GetInt32(teIdIndex),
                        ChargeCodeId = reader.GetInt32(chargeCodeIdIndex),
                        Note = reader.GetString(noteIndex),
                        EntryDate = reader.GetDateTime(createDateIndex),
                        ChargeCodeDate = reader.GetDateTime(chargeCodeDateIndex),
                        TimeType = timeType,
                        ActualHours = reader.GetFloat(actualHrsIndex),
                        ForecastedHours = reader.GetFloat(forecastedHrsIndex),
                        ModifiedDate = reader.GetDateTime(modifiedDateIndex),
                        IsChargeable = reader.GetBoolean(isChargeableIndex),
                        IsReviewed = (ReviewStatus)Enum.Parse(typeof(ReviewStatus), reader.GetInt32(revieweStatusIdIndex).ToString())

                    };

                    if (isCorrectIndex >= 0)
                    {
                        timeEntry.IsCorrect = reader.GetBoolean(isCorrectIndex);
                    }

                    if (approvedByIdIndex > 0 && approvedByFirstNameIndex > 0 && approvedByLastNameIndex > 0)
                    {
                        if (!reader.IsDBNull(approvedByIdIndex) && !reader.IsDBNull(approvedByFirstNameIndex) && !reader.IsDBNull(approvedByLastNameIndex))
                        {
                            var approvedByPerson = new Person { Id = reader.GetInt32(approvedByIdIndex), FirstName = reader.GetString(approvedByFirstNameIndex), LastName = reader.GetString(approvedByLastNameIndex) };
                            timeEntry.ApprovedBy = approvedByPerson;
                        }
                    }

                    timeEntries.Add(timeEntry);
                }
            }
        }

        #endregion
    }
}


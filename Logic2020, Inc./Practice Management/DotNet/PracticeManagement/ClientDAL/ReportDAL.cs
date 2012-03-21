using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataTransferObjects.Reports;
using System.Data.SqlClient;
using DataAccess.Other;
using System.Data;
using DataTransferObjects;
using DataTransferObjects.TimeEntry;

namespace DataAccess
{
    public static class ReportDAL
    {

        public static List<TimeEntriesGroupByClientAndProject> PersonTimeEntriesDetails(int personId, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.PersonTimeEntriesDetails, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<TimeEntriesGroupByClientAndProject>();
                    ReadPersonTimeEntriesDetails(reader, result);
                    return result;
                }
            }
        }

        private static void ReadPersonTimeEntriesDetails(SqlDataReader reader, List<TimeEntriesGroupByClientAndProject> result)
        {
            if (reader.HasRows)
            {
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNameColumn);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
                int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
                int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
                int chargeCodeDateIndex = reader.GetOrdinal(Constants.ColumnNames.ChargeCodeDate);
                int timeTypeNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeName);
                int noteIndex = reader.GetOrdinal(Constants.ColumnNames.Note);
                int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
                int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
                int groupNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn);

                while (reader.Read())
                {
                    var dayTotalHoursbyWorkType = new TimeEntryByWorkType()
                    {
                        Note = !reader.IsDBNull(noteIndex) ? reader.GetString(noteIndex) : string.Empty,
                        BillableHours = reader.GetDouble(billableHoursIndex),
                        NonBillableHours = reader.GetDouble(nonBillableHoursIndex),
                        TimeType = new TimeTypeRecord()
                        {
                            Name = reader.GetString(timeTypeNameIndex)
                        }
                    };

                    var dt = new TimeEntriesGroupByDate()
                    {
                        Date = reader.GetDateTime(chargeCodeDateIndex),
                        DayTotalHoursList = new List<TimeEntryByWorkType>()
                                                {
                                                    dayTotalHoursbyWorkType
                                                }
                    };


                    var ptd = new TimeEntriesGroupByClientAndProject
                    {
                        Project = new Project()
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            Name = reader.GetString(projectNameIndex),
                            ProjectNumber = reader.GetString(projectNumberIndex),
                            Group = new ProjectGroup()
                            {
                                Name = reader.GetString(groupNameIndex)
                            }
                        }
                        ,
                        Client = new Client()
                        {
                            Id = reader.GetInt32(clientIdIndex),
                            Name = reader.GetString(clientNameIndex)
                        },
                        DayTotalHours = new List<TimeEntriesGroupByDate>() 
                        {
                            dt
                        }

                    };


                    if (result.Any(r => r.Project.Id == ptd.Project.Id && r.Client.Id == ptd.Client.Id))
                    {
                        ptd = result.First(r => r.Project.Id == ptd.Project.Id && r.Client.Id == ptd.Client.Id);

                        ptd.AddDayTotalHours(dt);
                    }
                    else
                    {
                        result.Add(ptd);
                    }

                }
            }
        }

        public static List<TimeEntriesGroupByClientAndProject> PersonTimeEntriesSummary(int personId, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.PersonTimeEntriesSummary, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<TimeEntriesGroupByClientAndProject>();
                    ReadPersonTimeEntriesSummary(reader, result);
                    return result;
                }
            }
        }

        private static void ReadPersonTimeEntriesSummary(SqlDataReader reader, List<TimeEntriesGroupByClientAndProject> result)
        {
            if (reader.HasRows)
            {

                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNameColumn);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
                int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
                int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
                int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
                int billableValueindex = reader.GetOrdinal(Constants.ColumnNames.BillableValue);
                int groupNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn);

                while (reader.Read())
                {

                    var ptd = new TimeEntriesGroupByClientAndProject
                    {
                        Project = new Project()
                        {

                            Name = reader.GetString(projectNameIndex),
                            ProjectNumber = reader.GetString(projectNumberIndex),
                            Group = new ProjectGroup()
                            {
                                Name = reader.GetString(groupNameIndex)
                            }
                        }
                        ,
                        Client = new Client()
                        {
                            Name = reader.GetString(clientNameIndex)
                        },
                        BillableHours = reader.GetDouble(billableHoursIndex),
                        NonBillableHours = reader.GetDouble(nonBillableHoursIndex),
                        BillableValue = reader.GetDouble(billableValueindex)
                    };

                    result.Add(ptd);

                }
            }
        }

        public static Quadruple<double, double, double, double> GetPersonTimeEntriesTotalsByPeriod(int personId, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetPersonTimeEntriesTotalsByPeriod, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Quadruple<double, double, double, double> result = new Quadruple<double, double, double, double>(0d, 0d, 0d, 0d);
                    if (reader.HasRows)
                    {
                        int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
                        int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
                        int billableValueindex = reader.GetOrdinal(Constants.ColumnNames.BillableValue);
                        int utlizationPercentIndex = reader.GetOrdinal(Constants.ColumnNames.UtlizationPercent);

                        while (reader.Read())
                        {
                            result.First = !reader.IsDBNull(billableHoursIndex) ? (double)reader.GetDouble(billableHoursIndex) : 0d;
                            result.Second = !reader.IsDBNull(nonBillableHoursIndex) ? (double)reader.GetDouble(nonBillableHoursIndex) : 0d;
                            result.Third = !reader.IsDBNull(billableValueindex) ? (double)reader.GetDouble(billableValueindex) : 0d;
                            result.Fourth = !reader.IsDBNull(utlizationPercentIndex) ? (int)reader.GetDouble(utlizationPercentIndex) : 0d;
                        }
                    }
                    return result;
                }
            }
        }

        public static List<PersonLevelGroupedHours> TimePeriodSummaryReportByResource(DateTime startDate, DateTime endDate, string seniorityIds, string orderByCerteria)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.TimePeriodSummaryReportByResource, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.SeniorityIdsParam, !string.IsNullOrEmpty(seniorityIds) ? seniorityIds : (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.OrderByCerteriaParam, !string.IsNullOrEmpty(orderByCerteria) ? orderByCerteria : (Object)DBNull.Value);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<PersonLevelGroupedHours>();
                    ReadTimePeriodSummaryReportByResource(reader, result);
                    return result;
                }
            }
        }

        private static void ReadTimePeriodSummaryReportByResource(SqlDataReader reader, List<PersonLevelGroupedHours> result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int personSeniorityIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonSeniorityId);
                int personSeniorityNameIndex = reader.GetOrdinal(Constants.ColumnNames.PersonSeniorityName);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
                int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
                int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
                int groupByCerteriaIndex = reader.GetOrdinal(Constants.ColumnNames.GroupByCerteria);

                while (reader.Read())
                {
                    int personId = reader.GetInt32(personIdIndex);
                    if (!result.Any(p => p.Person.Id == personId))
                    {

                        PersonLevelGroupedHours PLGH = new PersonLevelGroupedHours();
                        Person person = new Person
                                                {
                                                    Id = reader.GetInt32(personIdIndex),
                                                    FirstName = reader.GetString(firstNameIndex),
                                                    LastName = reader.GetString(lastNameIndex),
                                                    Seniority = new Seniority
                                                    {
                                                        Id = reader.GetInt32(personSeniorityIdIndex),
                                                        Name = reader.GetString(personSeniorityNameIndex)
                                                    }
                                                };
                        PLGH.Person = person;
                        GroupedHours GH = new GroupedHours();
                        GH.StartDate = reader.GetDateTime(startDateIndex);
                        GH.SetEnddate(reader.GetString(groupByCerteriaIndex));
                        GH.BillabileTotal = reader.GetDouble(billableHoursIndex);
                        GH.NonBillableTotal = reader.GetDouble(nonBillableHoursIndex);

                        //PLGH.GroupedHoursList = new List<GroupedHours>();
                        //PLGH.GroupedHoursList.Add(GH);
                        result.Add(PLGH);
                    }
                    else
                    {
                        PersonLevelGroupedHours PLGH = result.First(p => p.Person.Id == personId);
                        GroupedHours GH = new GroupedHours();
                        GH.StartDate = reader.GetDateTime(startDateIndex);
                        GH.SetEnddate(reader.GetString(groupByCerteriaIndex));
                        GH.BillabileTotal = reader.GetDouble(billableHoursIndex);
                        GH.NonBillableTotal = reader.GetDouble(nonBillableHoursIndex);

                        //PLGH.GroupedHoursList.Add(GH);
                    }
                }
            }
        }

        public static List<ProjectLevelGroupedHours> TimePeriodSummaryReportByProject(DateTime startDate, DateTime endDate, string clientIds, string personStatusIds, string orderByCerteria)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.TimePeriodSummaryReportByProject, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdsParam, !string.IsNullOrEmpty(clientIds) ? clientIds : (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonStatusIdsParam, !string.IsNullOrEmpty(personStatusIds) ? personStatusIds : (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.OrderByCerteriaParam, !string.IsNullOrEmpty(orderByCerteria) ? orderByCerteria : (Object)DBNull.Value);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<ProjectLevelGroupedHours>();
                    ReadTimePeriodSummaryReportByProject(reader, result);
                    return result;
                }
            }
        }

        private static void ReadTimePeriodSummaryReportByProject(SqlDataReader reader, List<ProjectLevelGroupedHours> result)
        {
            if (reader.HasRows)
            {
                int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientId);
                int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientName);
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                int projectNumberindex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
                int projectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusIdColumn);
                int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
                int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
                int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
                int groupByCerteriaIndex = reader.GetOrdinal(Constants.ColumnNames.GroupByCerteria);

                while (reader.Read())
                {
                    int projectId = reader.GetInt32(projectIdIndex);
                    if (!result.Any(p => p.Project.Id == projectId))
                    {

                        ProjectLevelGroupedHours plgh = new ProjectLevelGroupedHours();
                        Project project = new Project
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            Name = reader.GetString(projectNameIndex),
                            ProjectNumber = reader.GetString(projectNumberindex),
                            Client = new Client
                            {
                                Id = reader.GetInt32(clientIdIndex),
                                Name = reader.GetString(clientNameIndex)
                            },
                            Status = new ProjectStatus
                            {
                                Id = reader.GetInt32(projectStatusIdIndex),
                                Name = reader.GetString(projectStatusNameIndex)
                            }
                        };
                        plgh.Project = project;
                        GroupedHours GH = new GroupedHours();
                        GH.StartDate = reader.GetDateTime(startDateIndex);
                        GH.SetEnddate(reader.GetString(groupByCerteriaIndex));
                        GH.BillabileTotal = reader.GetDouble(billableHoursIndex);
                        GH.NonBillableTotal = reader.GetDouble(nonBillableHoursIndex);

                        plgh.GroupedHoursList = new List<GroupedHours>();
                        plgh.GroupedHoursList.Add(GH);
                        result.Add(plgh);
                    }
                    else
                    {
                        ProjectLevelGroupedHours plgh = result.First(p => p.Project.Id == projectId);
                        GroupedHours gh = new GroupedHours();
                        gh.StartDate = reader.GetDateTime(startDateIndex);
                        gh.SetEnddate(reader.GetString(groupByCerteriaIndex));
                        gh.BillabileTotal = reader.GetDouble(billableHoursIndex);
                        gh.NonBillableTotal = reader.GetDouble(nonBillableHoursIndex);

                        plgh.GroupedHoursList.Add(gh);
                    }
                }
            }
        }

        public static List<WorkTypeLevelGroupedHours> TimePeriodSummaryReportByWorkType(DateTime startDate, DateTime endDate, string timeTypeCategoryIds, string orderByCerteria)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.TimePeriodSummaryReportByWorkType, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeCategoryIdsParam, !string.IsNullOrEmpty(timeTypeCategoryIds) ? timeTypeCategoryIds : (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.OrderByCerteriaParam, !string.IsNullOrEmpty(orderByCerteria) ? orderByCerteria : (Object)DBNull.Value);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<WorkTypeLevelGroupedHours>();
                    ReadByWorkType(reader, result);
                    return result;
                }
            }
        }

        private static void ReadByWorkType(SqlDataReader reader, List<WorkTypeLevelGroupedHours> result)
        {
            if (reader.HasRows)
            {
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
                int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
                int timeTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeId);
                int timeTypeNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeName);
                int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
                int groupByCerteriaIndex = reader.GetOrdinal(Constants.ColumnNames.GroupByCerteria);
                int isDefaultIndex = reader.GetOrdinal(Constants.ColumnNames.IsDefault);
                int isInternalColumnIndex = reader.GetOrdinal(Constants.ColumnNames.IsInternalColumn);
                int isAdministrativeColumnIndex = reader.GetOrdinal(Constants.ColumnNames.IsAdministrativeColumn);
                int categoryIndex = reader.GetOrdinal(Constants.ColumnNames.Category);

                while (reader.Read())
                {
                    int workTypeId = reader.GetInt32(timeTypeIdIndex);

                    GroupedHours GH = new GroupedHours();
                    GH.StartDate = reader.GetDateTime(startDateIndex);
                    GH.SetEnddate(reader.GetString(groupByCerteriaIndex));
                    GH.BillabileTotal = reader.GetDouble(billableHoursIndex);
                    GH.NonBillableTotal = reader.GetDouble(nonBillableHoursIndex);


                    if (!result.Any(p => p.WorkType.Id == workTypeId))
                    {

                        WorkTypeLevelGroupedHours worktypeLGH = new WorkTypeLevelGroupedHours();

                        var tt = new TimeTypeRecord
                        {
                            Id = workTypeId,
                            Name = reader.GetString(timeTypeNameIndex),
                            IsDefault = reader.GetBoolean(isDefaultIndex),
                            IsInternal = reader.GetBoolean(isInternalColumnIndex),
                            IsAdministrative = reader.GetBoolean(isAdministrativeColumnIndex),
                            Category = reader.GetString(categoryIndex)
                        };

                        worktypeLGH.WorkType = tt;
                        worktypeLGH.GroupedHoursList = new List<GroupedHours>();
                        worktypeLGH.GroupedHoursList.Add(GH);
                        result.Add(worktypeLGH);
                    }
                    else
                    {
                        WorkTypeLevelGroupedHours worktypeLGH = result.First(w => w.WorkType.Id == workTypeId);
                        worktypeLGH.GroupedHoursList.Add(GH);
                    }
                }
            }
        }

        public static List<PersonLevelGroupedHours> ProjectSummaryReportByResource(string projectNumber)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ProjectSummaryReportByResource, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectNumber, projectNumber);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<PersonLevelGroupedHours>();
                    ReadProjectSummaryReportByResource(reader, result);
                    return result;
                }
            }

        }


        private static void ReadProjectSummaryReportByResource(SqlDataReader reader, List<PersonLevelGroupedHours> result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
                int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
                int billableValueindex = reader.GetOrdinal(Constants.ColumnNames.BillableValue);
                int projectRoleNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectRoleName);

                while (reader.Read())
                {
                    int personId = reader.GetInt32(personIdIndex);


                    PersonLevelGroupedHours PLGH = new PersonLevelGroupedHours();
                    Person person = new Person
                    {
                        Id = reader.GetInt32(personIdIndex),
                        FirstName = reader.GetString(firstNameIndex),
                        LastName = reader.GetString(lastNameIndex),
                        ProjectRoleName = reader.GetString(projectRoleNameIndex)
                    };
                    PLGH.BillabileHours = reader.GetDouble(billableHoursIndex);
                    PLGH.NonBillableHours = reader.GetDouble(nonBillableHoursIndex);
                    PLGH.BillableValue = reader.GetDouble(billableValueindex);

                    PLGH.Person = person;
                    result.Add(PLGH);
                }
            }
        }

        public static List<WorkTypeLevelGroupedHours> ProjectSummaryReportByWorkType(string projectNumber, string timeTypeCategoryIds, string orderByCerteria)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ProjectSummaryReportByWorkType, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectNumber, projectNumber);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeCategoryIdsParam, !string.IsNullOrEmpty(timeTypeCategoryIds) ? timeTypeCategoryIds : (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.OrderByCerteriaParam, !string.IsNullOrEmpty(orderByCerteria) ? orderByCerteria : (Object)DBNull.Value);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<WorkTypeLevelGroupedHours>();
                    ReadByWorkType(reader, result);
                    return result;
                }
            }
        }

    }
}


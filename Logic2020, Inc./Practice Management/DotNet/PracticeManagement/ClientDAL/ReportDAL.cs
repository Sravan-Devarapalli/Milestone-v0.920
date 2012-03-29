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
using System.Data.Common;

namespace DataAccess
{
    public static class ReportDAL
    {

        public static List<Project> ProjectSearchByName(string name)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Project.ProjectSearchByName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.LookedParam,
                    !string.IsNullOrEmpty(name) ? (object)name : DBNull.Value);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    var result = new List<Project>();
                    ReadProjectSearchList(reader, result);
                    return result;
                }
            }
        }

        private static void ReadProjectSearchList(DbDataReader reader, List<Project> result)
        {
            if (reader.HasRows)
            {
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNameColumn);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);


                while (reader.Read())
                {
                    Project project = new Project();

                    project.Name = reader.GetString(projectNameIndex);
                    project.ProjectNumber = reader.GetString(projectNumberIndex);

                    result.Add(project);
                }
            }
        }

        public static List<TimeEntriesGroupByClientAndProject> PersonTimeEntriesDetails(int personId, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.PersonTimeEntriesDetails, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

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
                int groupCodeIndex = reader.GetOrdinal(Constants.ColumnNames.GroupCodeColumn);
                int clientCodeIndex = reader.GetOrdinal(Constants.ColumnNames.ClientCodeColumn);
                int timeTypeCodeIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeCodeColumn);

                while (reader.Read())
                {
                    var dayTotalHoursbyWorkType = new TimeEntryByWorkType()
                    {
                        Note = !reader.IsDBNull(noteIndex) ? reader.GetString(noteIndex) : string.Empty,
                        BillableHours = reader.GetDouble(billableHoursIndex),
                        NonBillableHours = reader.GetDouble(nonBillableHoursIndex),
                        TimeType = new TimeTypeRecord()
                        {
                            Name = reader.GetString(timeTypeNameIndex),
                            Code = reader.GetString(timeTypeCodeIndex)
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
                                Name = reader.GetString(groupNameIndex),
                                Code = reader.GetString(groupCodeIndex)
                            }
                        }
                        ,
                        Client = new Client()
                        {
                            Id = reader.GetInt32(clientIdIndex),
                            Name = reader.GetString(clientNameIndex),
                            Code = reader.GetString(clientCodeIndex)
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
                command.CommandTimeout = connection.ConnectionTimeout;

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
                int groupNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn);
                int groupCodeIndex = reader.GetOrdinal(Constants.ColumnNames.GroupCodeColumn);
                int clientCodeIndex = reader.GetOrdinal(Constants.ColumnNames.ClientCodeColumn);
                int personStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusName);
                int billingTypeIndex = reader.GetOrdinal(Constants.ColumnNames.BillingType);

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
                                Name = reader.GetString(groupNameIndex),
                                Code = reader.GetString(groupCodeIndex)
                            },
                            Status = new ProjectStatus {
                                Name = reader.GetString(personStatusNameIndex)
                            }
                        },

                        Client = new Client()
                        {
                            Name = reader.GetString(clientNameIndex),
                            Code = reader.GetString(clientCodeIndex)
                        },

                        BillableHours = reader.GetDouble(billableHoursIndex),
                        NonBillableHours = reader.GetDouble(nonBillableHoursIndex),
                        BillableType = reader.GetString(billingTypeIndex)

                    };

                    result.Add(ptd);

                }
            }
        }

        public static Triple<double, double, double> GetPersonTimeEntriesTotalsByPeriod(int personId, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetPersonTimeEntriesTotalsByPeriod, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Triple<double, double, double> result = new Triple<double, double, double>(0d, 0d, 0d);
                    if (reader.HasRows)
                    {
                        int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
                        int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
                        int utlizationPercentIndex = reader.GetOrdinal(Constants.ColumnNames.UtlizationPercent);

                        while (reader.Read())
                        {
                            result.First = !reader.IsDBNull(billableHoursIndex) ? (double)reader.GetDouble(billableHoursIndex) : 0d;
                            result.Second = !reader.IsDBNull(nonBillableHoursIndex) ? (double)reader.GetDouble(nonBillableHoursIndex) : 0d;
                            result.Third = !reader.IsDBNull(utlizationPercentIndex) ? (int)reader.GetDouble(utlizationPercentIndex) : 0d;
                        }
                    }
                    return result;
                }
            }
        }

        public static List<PersonLevelGroupedHours> TimePeriodSummaryReportByResource(DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.TimePeriodSummaryReportByResource, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);

                command.CommandTimeout = connection.ConnectionTimeout;

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
                int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
                int projectNonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNonBillableHours);
                int businessDevelopmentHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessDevelopmentHours);
                int internalHoursIndex = reader.GetOrdinal(Constants.ColumnNames.InternalHours);
                int adminstrativeHoursIndex = reader.GetOrdinal(Constants.ColumnNames.AdminstrativeHours);
                int personSeniorityIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonSeniorityId);
                int personSeniorityNameIndex = reader.GetOrdinal(Constants.ColumnNames.PersonSeniorityName);
                int utlizationPercentIndex = reader.GetOrdinal(Constants.ColumnNames.UtlizationPercent);
                int timeScaleIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleColumn);

                while (reader.Read())
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
                                                },
                                                CurrentPay = new Pay
                                                {
                                                    TimescaleName = reader.IsDBNull(timeScaleIndex) ? String.Empty : reader.GetString(timeScaleIndex)
                                                },
                                                UtlizationPercent = !reader.IsDBNull(utlizationPercentIndex) ? (int)reader.GetDouble(utlizationPercentIndex) : 0d
                                            };
                    PLGH.Person = person;
                    PLGH.BillableHours = reader.GetDouble(billableHoursIndex);
                    PLGH.ProjectNonBillableHours = reader.GetDouble(projectNonBillableHoursIndex);
                    PLGH.BusinessDevelopmentHours = reader.GetDouble(businessDevelopmentHoursIndex);
                    PLGH.InternalHours = reader.GetDouble(internalHoursIndex);
                    PLGH.AdminstrativeHours = reader.GetDouble(adminstrativeHoursIndex);
                    PLGH.Person = person;
                    result.Add(PLGH);
                }
            }
        }

        public static List<ProjectLevelGroupedHours> TimePeriodSummaryReportByProject(DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.TimePeriodSummaryReportByProject, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.CommandTimeout = connection.ConnectionTimeout;

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
                int clientCodeIndex = reader.GetOrdinal(Constants.ColumnNames.ClientCodeColumn);
                int groupNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn);
                int groupCodeIndex = reader.GetOrdinal(Constants.ColumnNames.GroupCodeColumn);
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                int projectNumberindex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
                int projectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusIdColumn);
                int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
                int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
                int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
                int billingTypeIndex = reader.GetOrdinal(Constants.ColumnNames.BillingType);
                int billableHoursUntilTodayIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHoursUntilToday);
                int forecastedHoursUntilTodayIndex = reader.GetOrdinal(Constants.ColumnNames.ForecastedHoursUntilToday);
                int timeEntrySectionIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeEntrySectionId);

                while (reader.Read())
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
                            Name = reader.GetString(clientNameIndex),
                            Code = reader.GetString(clientCodeIndex)
                        },
                        Group = new ProjectGroup
                        {
                            Name = reader.GetString(groupNameIndex),
                            Code = reader.GetString(groupCodeIndex)
                        },
                        Status = new ProjectStatus
                        {
                            Id = reader.GetInt32(projectStatusIdIndex),
                            Name = reader.GetString(projectStatusNameIndex)
                        },
                        TimeEntrySectionId = reader.GetInt32(timeEntrySectionIdIndex)
                    };
                    plgh.Project = project;
                    plgh.BillableHours = reader.GetDouble(billableHoursIndex);
                    plgh.NonBillableHours = reader.GetDouble(nonBillableHoursIndex);
                    plgh.BillableHoursUntilToday = reader.GetDouble(billableHoursUntilTodayIndex);
                    plgh.ForecastedHoursUntilToday = Convert.ToDouble(reader[forecastedHoursUntilTodayIndex]);
                    plgh.BillingType = reader.GetString(billingTypeIndex);
                    result.Add(plgh);
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
                command.CommandTimeout = connection.ConnectionTimeout;

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

        public static List<PersonLevelGroupedHours> ProjectSummaryReportByResource(string projectNumber, int? milestoneId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ProjectSummaryReportByResource, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectNumber, projectNumber);
                command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneId, milestoneId.HasValue ? (object)milestoneId : DBNull.Value);

                command.CommandTimeout = connection.ConnectionTimeout;

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
                int projectRoleNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectRoleName);
                int billableHoursUntilTodayIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHoursUntilToday);
                int forecastedHoursUntilTodayIndex = reader.GetOrdinal(Constants.ColumnNames.ForecastedHoursUntilToday);
                int forecastedHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ForecastedHours);
                int billingTypeIndex = reader.GetOrdinal(Constants.ColumnNames.BillingType);

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

                    PLGH.BillableHours = reader.GetDouble(billableHoursIndex);
                    PLGH.ProjectNonBillableHours = reader.GetDouble(nonBillableHoursIndex);
                    PLGH.BillableHoursUntilToday = reader.GetDouble(billableHoursUntilTodayIndex);
                    PLGH.ForecastedHoursUntilToday = Convert.ToDouble(reader[forecastedHoursUntilTodayIndex]);
                    PLGH.BillingType = reader.GetString(billingTypeIndex);
                    PLGH.Person = person;
                    PLGH.ForecastedHours = Convert.ToDouble(reader[forecastedHoursUntilTodayIndex]);

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
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<WorkTypeLevelGroupedHours>();
                    ReadByWorkType(reader, result);
                    return result;
                }
            }
        }

        public static List<Project> GetProjectsByClientId(int clientId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Project.GetProjectsByClientId, connection))
            {

                command.Parameters.AddWithValue(Constants.ParameterNames.ClientId, clientId);
                command.CommandTimeout = connection.ConnectionTimeout;

                command.CommandType = CommandType.StoredProcedure;
                connection.Open();

                var result = new List<Project>();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var proj = ReadProject(reader);
                            result.Add(proj);
                        }
                    }
                }


                return result;
            }
        }

        private static Project ReadProject(DbDataReader reader)
        {
            int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);

            var project = new Project
            {
                Id = reader.GetInt32(reader.GetOrdinal(Constants.ParameterNames.ProjectId)),
                Name = reader.GetString(reader.GetOrdinal(Constants.ParameterNames.ProjectName)),
                ProjectNumber = reader.GetString(reader.GetOrdinal(Constants.ParameterNames.ProjectNumber)),
                Status = new ProjectStatus
                {
                    Name = reader.GetString(projectStatusNameIndex)
                }
            };


            return project;
        }

        public static List<Milestone> GetMilestonesForProject(string projectNumber)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetMilestonesForProject, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectNumber, projectNumber);
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Milestone>();
                    ReadMilestonesByProject(reader, result);
                    return result;
                }
            }
        }

        private static void ReadMilestonesByProject(SqlDataReader reader, List<Milestone> result)
        {
            if (reader.HasRows)
            {

                while (reader.Read())
                {

                    int mileStoneIdIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneId);
                    int mileStoneNameIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneName);

                    var milestone = new Milestone
                    {
                        Description = reader.GetString(mileStoneNameIndex),
                        Id = reader.GetInt32(mileStoneIdIndex)
                    };

                    result.Add(milestone);
                }
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using DataAccess.Other;
using DataTransferObjects;
using DataTransferObjects.Financials;
using DataTransferObjects.Reports;
using DataTransferObjects.Reports.ByAccount;
using DataTransferObjects.Reports.ConsultingDemand;
using DataTransferObjects.Reports.HumanCapital;
using DataTransferObjects.TimeEntry;

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
            if (!reader.HasRows) return;
            int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNameColumn);
            int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
            int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);

            while (reader.Read())
            {
                Project project = new Project()
                {
                    Name = reader.GetString(projectNameIndex),
                    ProjectNumber = reader.GetString(projectNumberIndex),
                    Client = new Client()
                    {
                        Name = reader.GetString(clientNameIndex)
                    }
                };

                result.Add(project);
            }
        }

        public static List<TimeEntriesGroupByClientAndProject> PersonTimeEntriesDetails(int personId, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                return PersonTimeEntriesDetailsWithData(personId, startDate, endDate, connection);
            }
        }

        public static List<TimeEntriesGroupByClientAndProject> PersonTimeEntriesDetailsSchedular(int? personId, DateTime startDate, DateTime endDate, SqlConnection connection)
        {
            return PersonTimeEntriesDetailsWithData(personId, startDate, endDate, connection);
        }

        private static List<TimeEntriesGroupByClientAndProject> PersonTimeEntriesDetailsWithData(int? personId, DateTime startDate, DateTime endDate, SqlConnection connection)
        {
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.PersonTimeEntriesDetails, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId.HasValue ? (object)personId.Value : DBNull.Value);
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
            if (!reader.HasRows) return;
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
            int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
            int billingTypeIndex = reader.GetOrdinal(Constants.ColumnNames.BillingType);
            int timeEntrySectionIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeEntrySectionId);
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int personFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int personLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int hourlyRateIndex = reader.GetOrdinal(Constants.ColumnNames.HourlyRate);
            int isOffshoreIndex = reader.GetOrdinal(Constants.ColumnNames.IsOffshore);
            int employeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeNumber);
            int payRateIndex = reader.GetOrdinal(Constants.ColumnNames.HourlyPayRate);
            int timeScaleNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleName);

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

                if (!reader.IsDBNull(hourlyRateIndex))
                {
                    dayTotalHoursbyWorkType.HourlyRate = reader.GetDecimal(hourlyRateIndex);
                }

                if (!reader.IsDBNull(payRateIndex))
                {
                    dayTotalHoursbyWorkType.PayRate = reader.GetDecimal(payRateIndex);
                }

                if (!reader.IsDBNull(timeScaleNameIndex))
                {
                    dayTotalHoursbyWorkType.PayType = reader.GetString(timeScaleNameIndex);
                }

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
                        },
                        Status = new ProjectStatus
                        {
                            Name = reader.GetString(projectStatusNameIndex)
                        },
                        TimeEntrySectionId = reader.GetInt32(timeEntrySectionIdIndex)
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
                            },
                    BillableType = reader.GetString(billingTypeIndex),
                    Person = new Person
                    {
                        Id = reader.GetInt32(personIdIndex),
                        FirstName = reader.GetString(personFirstNameIndex),
                        LastName = reader.GetString(personLastNameIndex),
                        EmployeeNumber = reader.GetString(employeeNumberIndex),
                        IsOffshore = reader.GetBoolean(isOffshoreIndex)
                    }
                };

                if (result.Any(r => r.Person.Id == ptd.Person.Id && r.Project.Id == ptd.Project.Id && r.Client.Id == ptd.Client.Id))
                {
                    ptd = result.First(r => r.Person.Id == ptd.Person.Id && r.Project.Id == ptd.Project.Id && r.Client.Id == ptd.Client.Id);

                    ptd.AddDayTotalHours(dt);
                }
                else
                {
                    result.Add(ptd);
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
            if (!reader.HasRows) return;
            int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNameColumn);
            int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
            int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
            int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
            int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
            int groupNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn);
            int groupCodeIndex = reader.GetOrdinal(Constants.ColumnNames.GroupCodeColumn);
            int clientCodeIndex = reader.GetOrdinal(Constants.ColumnNames.ClientCodeColumn);
            int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
            int billingTypeIndex = reader.GetOrdinal(Constants.ColumnNames.BillingType);
            int timeEntrySectionIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeEntrySectionId);
            int projectedHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectedHours);
            int billableHoursUntilTodayIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHoursUntilToday);
            int projectedHoursUntilTodayIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectedHoursUntilToday);

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
                        Status = new ProjectStatus
                        {
                            Name = reader.GetString(projectStatusNameIndex)
                        },
                        TimeEntrySectionId = reader.GetInt32(timeEntrySectionIdIndex)
                    },

                    Client = new Client()
                    {
                        Name = reader.GetString(clientNameIndex),
                        Code = reader.GetString(clientCodeIndex)
                    },

                    BillableHours = reader.GetDouble(billableHoursIndex),
                    NonBillableHours = reader.GetDouble(nonBillableHoursIndex),
                    BillableType = reader.GetString(billingTypeIndex),
                    ProjectedHours = !reader.IsDBNull(projectedHoursIndex) ? Convert.ToDouble(reader.GetDecimal(projectedHoursIndex)) : 0d,
                    ProjectedHoursUntilToday = !reader.IsDBNull(projectedHoursUntilTodayIndex) ? Convert.ToDouble(reader.GetDecimal(projectedHoursUntilTodayIndex)) : 0d,
                    BillableHoursUntilToday = reader.GetDouble(billableHoursUntilTodayIndex)
                };

                result.Add(ptd);
            }
            double grandTotal = result.Sum(t => t.TotalHours);
            grandTotal = Math.Round(grandTotal, 2);
            foreach (TimeEntriesGroupByClientAndProject cp in result)
            {
                cp.GrandTotal = grandTotal;
            }
        }

        public static PersonTimeEntriesTotals GetPersonTimeEntriesTotalsByPeriod(int personId, DateTime startDate, DateTime endDate)
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
                    PersonTimeEntriesTotals result = new PersonTimeEntriesTotals();
                    if (!reader.HasRows) return result;
                    int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
                    int billableHoursUntilTodayIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHoursUntilToday);
                    int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
                    int availableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.AvailableHours);

                    while (reader.Read())
                    {
                        result.BillableHours = !reader.IsDBNull(billableHoursIndex) ? (double)reader.GetDouble(billableHoursIndex) : 0d;
                        result.NonBillableHours = !reader.IsDBNull(nonBillableHoursIndex) ? (double)reader.GetDouble(nonBillableHoursIndex) : 0d;
                        result.AvailableHours = !reader.IsDBNull(availableHoursIndex) ? (int)reader.GetInt32(availableHoursIndex) : 0d;
                        result.BillableHoursUntilToday = !reader.IsDBNull(billableHoursUntilTodayIndex) ? (double)reader.GetDouble(billableHoursUntilTodayIndex) : 0d;
                    }
                    return result;
                }
            }
        }

        public static List<PersonLevelGroupedHours> TimePeriodSummaryReportByResource(DateTime startDate, DateTime endDate, bool includePersonsWithNoTimeEntries, string personTypes, string titleIds, string timescaleNames, string personStatusIds, string personDivisionIds)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.TimePeriodSummaryReportByResource, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IncludePersonsWithNoTimeEntriesParam, includePersonsWithNoTimeEntries);

                if (personTypes != null)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonTypesParam, personTypes);
                }

                if (titleIds != null)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.TitleIdsParam, titleIds);
                }

                if (timescaleNames != null)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.TimescaleNamesListParam, timescaleNames);
                }
                if (personStatusIds != null)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonStatusIdsParam, personStatusIds);
                }

                if (personDivisionIds != null)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonDivisionIdsParam, personDivisionIds);
                }

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
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
            int projectNonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNonBillableHours);
            int businessDevelopmentHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessDevelopmentHours);
            int internalHoursIndex = reader.GetOrdinal(Constants.ColumnNames.InternalHours);
            int billableHoursUntilTodayIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHoursUntilToday);

            int pTOHoursIndex = reader.GetOrdinal(Constants.ColumnNames.PTOHours);
            int holidayHoursIndex = reader.GetOrdinal(Constants.ColumnNames.HolidayHours);
            int juryDutyHoursIndex = reader.GetOrdinal(Constants.ColumnNames.JuryDutyHours);
            int bereavementHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BereavementHours);
            int oRTHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ORTHours);
            int unpaidHoursIndex = reader.GetOrdinal(Constants.ColumnNames.UnpaidHours);
            int sickOrSafeLeaveHoursIndex = reader.GetOrdinal(Constants.ColumnNames.SickOrSafeLeaveHours);

            int personTitleIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonTitleId);
            int personTitleNameIndex = reader.GetOrdinal(Constants.ColumnNames.PersonTitle);
            int billableUtilizationPercentIndex = reader.GetOrdinal(Constants.ColumnNames.BillableUtilizationPercent);
            int timeScaleIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleColumn);
            int isOffShoreIndex = reader.GetOrdinal(Constants.ColumnNames.IsOffshore);
            int personStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusId);
            int personStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusName);
            int divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);
            int availableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.AvailableHours);
            int employeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeNumber);
            int availableHoursUntilTodayIndex = reader.GetOrdinal(Constants.ColumnNames.AvailableHoursUntilToday);

            while (reader.Read())
            {
                PersonLevelGroupedHours PLGH = new PersonLevelGroupedHours();
                Person person = new Person
                {
                    Id = reader.GetInt32(personIdIndex),
                    FirstName = reader.GetString(firstNameIndex),
                    LastName = reader.GetString(lastNameIndex),
                    IsOffshore = reader.GetBoolean(isOffShoreIndex),
                    EmployeeNumber = reader.GetString(employeeNumberIndex),
                    Title = new Title
                    {
                        TitleId = reader.GetInt32(personTitleIdIndex),
                        TitleName = reader.GetString(personTitleNameIndex)
                    },
                    CurrentPay = new Pay
                    {
                        TimescaleName = reader.IsDBNull(timeScaleIndex) ? String.Empty : reader.GetString(timeScaleIndex)
                    },
                    Status = new PersonStatus
                    {
                        Id = reader.GetInt32(personStatusIdIndex),
                        Name = reader.GetString(personStatusNameIndex)
                    },
                    BillableUtilizationPercent = !reader.IsDBNull(billableUtilizationPercentIndex) ? reader.GetDouble(billableUtilizationPercentIndex) : 0d
                };
                if (!reader.IsDBNull(divisionIdIndex))
                {
                    person.DivisionType = (PersonDivisionType)Enum.Parse(typeof(PersonDivisionType), reader.GetInt32(divisionIdIndex).ToString());
                }
                PLGH.Person = person;
                PLGH.BillableHours = reader.GetDouble(billableHoursIndex);
                PLGH.ProjectNonBillableHours = reader.GetDouble(projectNonBillableHoursIndex);
                PLGH.BusinessDevelopmentHours = reader.GetDouble(businessDevelopmentHoursIndex);
                PLGH.InternalHours = reader.GetDouble(internalHoursIndex);
                PLGH.BillableHoursUntilToday = reader.GetDouble(billableHoursUntilTodayIndex);

                PLGH.PTOHours = reader.GetDouble(pTOHoursIndex);
                PLGH.HolidayHours = reader.GetDouble(holidayHoursIndex);
                PLGH.BereavementHours = reader.GetDouble(bereavementHoursIndex);
                PLGH.JuryDutyHours = reader.GetDouble(juryDutyHoursIndex);
                PLGH.ORTHours = reader.GetDouble(oRTHoursIndex);
                PLGH.UnpaidHours = reader.GetDouble(unpaidHoursIndex);
                PLGH.SickOrSafeLeaveHours = reader.GetDouble(sickOrSafeLeaveHoursIndex);
                PLGH.AvailableHours = reader.GetInt32(availableHoursIndex);
                PLGH.AvailableHoursUntilToday = reader.GetInt32(availableHoursUntilTodayIndex);

                PLGH.Person = person;
                result.Add(PLGH);
            }
        }

        public static GroupByAccount AccountSummaryReportByProject(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate, string projectStatusIds, string projectBillingTypes)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.AccountSummaryReportByProject, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.AccountIdParam, accountId);
                command.Parameters.AddWithValue(Constants.ParameterNames.BusinessUnitIdsParam, businessUnitIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectBillingTypesParam, projectBillingTypes ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectStatusIdsParam, projectStatusIds ?? (Object)DBNull.Value);
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new GroupByAccount();
                    var groupedByProject = new List<ProjectLevelGroupedHours>();
                    ReadTimePeriodSummaryReportByProject(reader, groupedByProject);

                    result.GroupedProjects = groupedByProject;

                    reader.NextResult();
                    ReadByAccountDetails(reader, result);
                    return result;
                }
            }
        }

        public static List<ProjectLevelGroupedHours> TimePeriodSummaryReportByProject(DateTime startDate, DateTime endDate, string clientIds, string projectStatusIds)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.TimePeriodSummaryReportByProject, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdsParam, clientIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectStatusIdsParam, projectStatusIds ?? (Object)DBNull.Value);
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
            if (!reader.HasRows) return;
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
            int forecastedHoursIndex;
            int estimatedBillingsIndex;
            int isBusinessDevelopmentIndex;

            try
            {
                isBusinessDevelopmentIndex = reader.GetOrdinal(Constants.ColumnNames.IsBusinessDevelopment);
            }
            catch
            {
                isBusinessDevelopmentIndex = -1;
            }


            int groupIdIndex = -1;

            try
            {
                groupIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn);
            }
            catch
            {
                groupIdIndex = -1;
            }

            try
            {
                forecastedHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ForecastedHours);
            }
            catch
            {
                forecastedHoursIndex = -1;
            }

            try
            {
                estimatedBillingsIndex = reader.GetOrdinal(Constants.ColumnNames.EstimatedBillings);
            }
            catch
            {
                estimatedBillingsIndex = -1;
            }

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

                if (groupIdIndex > -1)
                {
                    project.Group.Id = reader.GetInt32(groupIdIndex);
                }

                if (isBusinessDevelopmentIndex > -1)
                {
                    project.IsBusinessDevelopment = Convert.ToBoolean(reader.GetInt32(isBusinessDevelopmentIndex));
                }

                plgh.Project = project;
                plgh.BillableHours = reader.GetDouble(billableHoursIndex);
                plgh.NonBillableHours = reader.GetDouble(nonBillableHoursIndex);
                plgh.BillableHoursUntilToday = reader.GetDouble(billableHoursUntilTodayIndex);
                plgh.ForecastedHoursUntilToday = Convert.ToDouble(reader.GetDecimal(forecastedHoursUntilTodayIndex));
                plgh.BillingType = reader.GetString(billingTypeIndex);

                if (forecastedHoursIndex > -1)
                {
                    plgh.ForecastedHours = Convert.ToDouble(reader.GetDecimal(forecastedHoursIndex));
                }
                if (estimatedBillingsIndex > -1)
                {
                    plgh.EstimatedBillings = reader.GetDouble(estimatedBillingsIndex);
                }

                result.Add(plgh);
            }
        }

        public static GroupByAccount AccountSummaryReportByBusinessUnit(int accountId, string businessUnitIds, string projectStatusIds, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.AccountSummaryReportByBusinessUnit, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.AccountIdParam, accountId);
                command.Parameters.AddWithValue(Constants.ParameterNames.BusinessUnitIdsParam, businessUnitIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectStatusIdsParam, projectStatusIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);

                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new GroupByAccount();
                    var groupedBusinessUnits = new List<BusinessUnitLevelGroupedHours>();
                    ReadByBusinessUnit(reader, groupedBusinessUnits);
                    PopulateBusinessUnitTotalHoursPercent(groupedBusinessUnits);

                    reader.NextResult();
                    ReadByAccountDetails(reader, result);

                    result.GroupedBusinessUnits = groupedBusinessUnits;
                    return result;
                }
            }
        }

        private static void ReadByAccountDetails(SqlDataReader reader, GroupByAccount result)
        {
            if (!reader.HasRows) return;
            while (reader.Read())
            {
                int personsCountIndex = reader.GetOrdinal(Constants.ColumnNames.PersonsCountColumn);
                int accountNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
                int accountCodeIndex = reader.GetOrdinal(Constants.ColumnNames.ClientCodeColumn);
                int accountIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);

                int personsCount = reader.GetInt32(personsCountIndex);
                result.PersonsCount = personsCount;
                var account = new Client
                {
                    Id = reader.GetInt32(accountIdIndex),
                    Name = reader.GetString(accountNameIndex),
                    Code = reader.GetString(accountCodeIndex)
                };

                result.Account = account;
            }
        }

        private static void PopulateBusinessUnitTotalHoursPercent(List<BusinessUnitLevelGroupedHours> reportData)
        {
            double grandTotal = reportData.Sum(t => t.TotalHours);
            grandTotal = Math.Round(grandTotal, 2);

            if (!(grandTotal > 0)) return;
            foreach (BusinessUnitLevelGroupedHours buLevelGroupedHours in reportData)
            {
                buLevelGroupedHours.BusinessUnitTotalHoursPercent = Convert.ToInt32((buLevelGroupedHours.TotalHours / grandTotal) * 100);
            }
        }

        private static void ReadByBusinessUnit(SqlDataReader reader, List<BusinessUnitLevelGroupedHours> result)
        {
            if (!reader.HasRows) return;
            int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
            int billableHoursUntilTodayIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHoursUntilToday);
            int businessUnitIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn);
            int businessUnitNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn);
            int businessUnitCodeIndex = reader.GetOrdinal(Constants.ColumnNames.GroupCodeColumn);
            int businessUnitStatusIndex = reader.GetOrdinal(Constants.ColumnNames.Active);
            int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
            int businessDevelopmentHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessDevelopmentHours);
            int activeProjectsCountIndex = reader.GetOrdinal(Constants.ColumnNames.ActiveProjectsCount);
            int completedProjectsCountIndex = reader.GetOrdinal(Constants.ColumnNames.CompletedProjectsCount);
            int projectsCountIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectsCount);
            int projectedHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ForecastedHours);
            int forecastedHoursUntilTodayIndex = reader.GetOrdinal(Constants.ColumnNames.ForecastedHoursUntilToday);



            while (reader.Read())
            {
                int businessUnitId = reader.GetInt32(businessUnitIdIndex);

                var pg = new ProjectGroup
                {
                    Id = businessUnitId,
                    Name = reader.GetString(businessUnitNameIndex),
                    IsActive = reader.GetBoolean(businessUnitStatusIndex),
                    Code = reader.GetString(businessUnitCodeIndex)
                };

                BusinessUnitLevelGroupedHours buLGH = new BusinessUnitLevelGroupedHours
                {
                    BillableHours = !reader.IsDBNull(billableHoursIndex) ? reader.GetDouble(billableHoursIndex) : 0d,
                    BillableHoursUntilToday = !reader.IsDBNull(billableHoursUntilTodayIndex) ? reader.GetDouble(billableHoursUntilTodayIndex) : 0d,
                    NonBillableHours =
                        !reader.IsDBNull(nonBillableHoursIndex) ? reader.GetDouble(nonBillableHoursIndex) : 0d,
                    BusinessDevelopmentHours =
                        !reader.IsDBNull(businessDevelopmentHoursIndex)
                            ? reader.GetDouble(businessDevelopmentHoursIndex)
                            : 0d,
                    ForecastedHours =
               !reader.IsDBNull(projectedHoursIndex) ? reader.GetDouble(projectedHoursIndex) : 0d,
                    ForecastedHoursUntilToday =
                  !reader.IsDBNull(forecastedHoursUntilTodayIndex) ? reader.GetDouble(forecastedHoursUntilTodayIndex) : 0d,
                    ActiveProjectsCount = !reader.IsDBNull(activeProjectsCountIndex) ? reader.GetInt32(activeProjectsCountIndex) : 0,
                    CompletedProjectsCount = !reader.IsDBNull(completedProjectsCountIndex) ? reader.GetInt32(completedProjectsCountIndex) : 0,
                    ProjectsCount = !reader.IsDBNull(projectsCountIndex) ? reader.GetInt32(projectsCountIndex) : 0,
                    BusinessUnit = pg
                };

                result.Add(buLGH);
            }
        }

        public static List<BusinessUnitLevelGroupedHours> AccountReportGroupByBusinessUnit(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.AccountSummaryByBusinessDevelopment, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.AccountIdParam, accountId);
                command.Parameters.AddWithValue(Constants.ParameterNames.BusinessUnitIdsParam, businessUnitIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);

                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<BusinessUnitLevelGroupedHours>();
                    ReadBusinessDevelopmentDetailsGroupByBusinessUnit(reader, result);
                    return result;
                }
            }
        }

        public static List<GroupByPerson> AccountReportGroupByPerson(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.AccountSummaryByBusinessDevelopment, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.AccountIdParam, accountId);
                command.Parameters.AddWithValue(Constants.ParameterNames.BusinessUnitIdsParam, businessUnitIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);

                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<GroupByPerson>();
                    ReadBusinessDevelopmentDetailsGroupByPerson(reader, result);
                    return result;
                }
            }
        }

        private static void ReadBusinessDevelopmentDetailsGroupByPerson(SqlDataReader reader, List<GroupByPerson> result)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);

            int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);

            int chargeCodeDateIndex = reader.GetOrdinal(Constants.ColumnNames.ChargeCodeDate);
            int timeTypeNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeName);
            int noteIndex = reader.GetOrdinal(Constants.ColumnNames.Note);

            int businessUnitIdIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessUnitId);
            int businessUnitNameIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessUnitName);
            int isActiveIndex = reader.GetOrdinal(Constants.ColumnNames.Active);

            while (reader.Read())
            {
                var dayTotalHoursbyWorkType = new TimeEntryByWorkType()
                {
                    Note = !reader.IsDBNull(noteIndex) ? reader.GetString(noteIndex) : string.Empty,

                    NonBillableHours = !reader.IsDBNull(nonBillableHoursIndex) ? Convert.ToDouble(reader[nonBillableHoursIndex]) : 0d,
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

                GroupByPerson personTimeEntries = new GroupByPerson();

                int businessUnitId = reader.GetInt32(businessUnitIdIndex);
                int personId = reader.GetInt32(personIdIndex);

                Person person = new Person
                {
                    Id = personId,
                    FirstName = reader.GetString(firstNameIndex),
                    LastName = reader.GetString(lastNameIndex)
                };

                var businessUnit = new ProjectGroup()
                {
                    Id = businessUnitId,
                    Name = reader.GetString(businessUnitNameIndex),
                    IsActive = reader.GetBoolean(isActiveIndex)
                };

                GroupByBusinessUnit businessUnitTimeEntries = new GroupByBusinessUnit
                {
                    BusinessUnit = businessUnit,
                    DayTotalHours = new List<TimeEntriesGroupByDate>()
                            {
                                dt
                            }
                };

                personTimeEntries.Person = person;
                personTimeEntries.BusinessUnitLevelGroupedHoursList = new List<GroupByBusinessUnit>() {
                    businessUnitTimeEntries
                };

                if (result.Any(r => r.Person.Id == personId))
                {
                    personTimeEntries = result.First(r => r.Person.Id == personId);

                    if (personTimeEntries.BusinessUnitLevelGroupedHoursList.Any(r => r.BusinessUnit.Id == businessUnitId))
                    {
                        businessUnitTimeEntries = personTimeEntries.BusinessUnitLevelGroupedHoursList.First(r => r.BusinessUnit.Id == businessUnitId);
                        businessUnitTimeEntries.AddDayTotalHours(dt);
                    }
                    else
                    {
                        personTimeEntries.BusinessUnitLevelGroupedHoursList.Add(businessUnitTimeEntries);
                    }
                }
                else
                {
                    result.Add(personTimeEntries);
                }
            }
        }

        private static void ReadBusinessDevelopmentDetailsGroupByBusinessUnit(SqlDataReader reader, List<BusinessUnitLevelGroupedHours> result)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int employeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeNumber);

            int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);

            int chargeCodeDateIndex = reader.GetOrdinal(Constants.ColumnNames.ChargeCodeDate);
            int timeTypeNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeName);
            int timeTypeCodeIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeCodeColumn);
            int noteIndex = reader.GetOrdinal(Constants.ColumnNames.Note);

            int businessUnitIdIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessUnitId);
            int businessUnitNameIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessUnitName);
            int businessUnitCodeIndex = reader.GetOrdinal(Constants.ColumnNames.GroupCodeColumn);
            int isActiveIndex = reader.GetOrdinal(Constants.ColumnNames.Active);

            while (reader.Read())
            {
                var dayTotalHoursbyWorkType = new TimeEntryByWorkType()
                {
                    Note = !reader.IsDBNull(noteIndex) ? reader.GetString(noteIndex) : string.Empty,

                    NonBillableHours = !reader.IsDBNull(nonBillableHoursIndex) ? Convert.ToDouble(reader[nonBillableHoursIndex]) : 0d,
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

                BusinessUnitLevelGroupedHours businessUnitLGH;

                int businessUnitId = reader.GetInt32(businessUnitIdIndex);
                int personId = reader.GetInt32(personIdIndex);

                Person person = new Person
                {
                    Id = personId,
                    FirstName = reader.GetString(firstNameIndex),
                    LastName = reader.GetString(lastNameIndex),
                    EmployeeNumber = reader.GetString(employeeNumberIndex)
                };

                PersonLevelGroupedHours PLGH = new PersonLevelGroupedHours
                {
                    Person = person,
                    DayTotalHours = new List<TimeEntriesGroupByDate>()
                            {
                                dt
                            }
                };

                if (result.Any(r => r.BusinessUnit.Id == businessUnitId))
                {
                    businessUnitLGH = result.First(r => r.BusinessUnit.Id == businessUnitId);

                    if (businessUnitLGH.PersonLevelGroupedHoursList.Any(r => r.Person.Id == personId))
                    {
                        PLGH = businessUnitLGH.PersonLevelGroupedHoursList.First(r => r.Person.Id == personId);
                        PLGH.AddDayTotalHours(dt);
                    }
                    else
                    {
                        businessUnitLGH.PersonLevelGroupedHoursList.Add(PLGH);
                    }
                }
                else
                {
                    businessUnitLGH = new BusinessUnitLevelGroupedHours
                    {
                        BusinessUnit = new ProjectGroup()
                        {
                            Id = businessUnitId,
                            Name = reader.GetString(businessUnitNameIndex),
                            Code = reader.GetString(businessUnitCodeIndex),
                            IsActive = reader.GetBoolean(isActiveIndex)
                        },
                        PersonLevelGroupedHoursList = new List<PersonLevelGroupedHours> { PLGH }
                    };

                    result.Add(businessUnitLGH);
                }
            }
        }

        public static List<WorkTypeLevelGroupedHours> TimePeriodSummaryReportByWorkType(DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.TimePeriodSummaryReportByWorkType, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);

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
            if (!reader.HasRows) return;
            int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
            int timeTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeId);
            int timeTypeNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeName);
            int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
            int isDefaultIndex = reader.GetOrdinal(Constants.ColumnNames.IsDefault);
            int isInternalColumnIndex = reader.GetOrdinal(Constants.ColumnNames.IsInternalColumn);
            int isAdministrativeColumnIndex = reader.GetOrdinal(Constants.ColumnNames.IsAdministrativeColumn);
            int categoryIndex = reader.GetOrdinal(Constants.ColumnNames.Category);
            int forecastedHoursIndex = -1;
            try
            {
                forecastedHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ForecastedHours);
            }
            catch (Exception ex)
            {
                forecastedHoursIndex = -1;
            }
            while (reader.Read())
            {
                int workTypeId = reader.GetInt32(timeTypeIdIndex);

                var tt = new TimeTypeRecord
                {
                    Id = workTypeId,
                    Name = reader.GetString(timeTypeNameIndex),
                    IsDefault = reader.GetBoolean(isDefaultIndex),
                    IsInternal = reader.GetBoolean(isInternalColumnIndex),
                    IsAdministrative = reader.GetBoolean(isAdministrativeColumnIndex),
                    Category = reader.GetString(categoryIndex)
                };

                WorkTypeLevelGroupedHours worktypeLGH = new WorkTypeLevelGroupedHours
                {
                    BillableHours = !reader.IsDBNull(billableHoursIndex) ? reader.GetDouble(billableHoursIndex) : 0d,
                    NonBillableHours =
                        !reader.IsDBNull(nonBillableHoursIndex) ? reader.GetDouble(nonBillableHoursIndex) : 0d,
                    WorkType = tt
                };
                if (forecastedHoursIndex > 0)
                {
                    worktypeLGH.ForecastedHours = reader.GetDouble(forecastedHoursIndex);
                }
                result.Add(worktypeLGH);
            }
        }

        public static List<PersonLevelGroupedHours> ProjectSummaryReportByResource(string projectNumber, int? milestoneId, DateTime? startDate, DateTime? endDate, string personRoleNames)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ProjectSummaryReportByResource, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectNumber, projectNumber);
                command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneId, milestoneId.HasValue ? (object)milestoneId : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate.HasValue ? (object)startDate : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate.HasValue ? (object)endDate : DBNull.Value);
                if (personRoleNames != null)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonRoleNamesParam, personRoleNames);
                }
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
            if (!reader.HasRows) return;
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
            int isOffShoreIndex = reader.GetOrdinal(Constants.ColumnNames.IsOffshore);
            int EmployeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeNumber);
            int billRateIndex = reader.GetOrdinal(Constants.ColumnNames.BillRate);
            int estimatedBillingsIndex = reader.GetOrdinal(Constants.ColumnNames.EstimatedBillings);

            while (reader.Read())
            {
                PersonLevelGroupedHours PLGH = new PersonLevelGroupedHours();
                Person person = new Person
                {
                    Id = reader.GetInt32(personIdIndex),
                    FirstName = reader.GetString(firstNameIndex),
                    LastName = reader.GetString(lastNameIndex),
                    ProjectRoleName = reader.GetString(projectRoleNameIndex),
                    IsOffshore = reader.GetBoolean(isOffShoreIndex),
                    EmployeeNumber = reader.GetString(EmployeeNumberIndex)
                };

                PLGH.BillableHours = !reader.IsDBNull(billableHoursIndex) ? reader.GetDouble(billableHoursIndex) : 0d;
                PLGH.ProjectNonBillableHours = !reader.IsDBNull(nonBillableHoursIndex) ? reader.GetDouble(nonBillableHoursIndex) : 0d;
                PLGH.BillableHoursUntilToday = !reader.IsDBNull(billableHoursUntilTodayIndex) ? reader.GetDouble(billableHoursUntilTodayIndex) : 0d;
                PLGH.ForecastedHoursUntilToday = Convert.ToDouble(reader[forecastedHoursUntilTodayIndex]);
                PLGH.BillingType = reader.GetString(billingTypeIndex);
                PLGH.Person = person;
                PLGH.ForecastedHours = Convert.ToDouble(reader[forecastedHoursIndex]);
                PLGH.BillRate = PLGH.BillingType == "Fixed" ? -1 : Convert.ToDouble(reader[billRateIndex]);
                PLGH.EstimatedBillings = PLGH.BillingType == "Fixed" ? -1 : Convert.ToDouble(reader[estimatedBillingsIndex]);

                result.Add(PLGH);
            }
        }

        public static List<PersonLevelGroupedHours> ProjectDetailReportByResource(string projectNumber, int? milestoneId, DateTime? startDate, DateTime? endDate, string personRoleNames, bool isExport = false)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ProjectDetailReportByResource, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectNumber, projectNumber);
                command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneId, milestoneId.HasValue ? (object)milestoneId : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate.HasValue ? (object)startDate : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate.HasValue ? (object)endDate : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsExport, isExport);
                if (personRoleNames != null)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonRoleNamesParam, personRoleNames);
                }
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<PersonLevelGroupedHours>();
                    ReadProjectDetailReportByResource(reader, result);
                    return result;
                }
            }
        }

        private static void ReadProjectDetailReportByResource(SqlDataReader reader, List<PersonLevelGroupedHours> result)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
            int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
            int projectRoleNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectRoleName);
            int chargeCodeDateIndex = reader.GetOrdinal(Constants.ColumnNames.ChargeCodeDate);
            int timeTypeNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeName);
            int noteIndex = reader.GetOrdinal(Constants.ColumnNames.Note);
            int timeTypeCodeIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeCodeColumn);
            int timeEntrySectionIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeEntrySectionId);
            int forecastedHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ForecastedHours);
            int isOffShoreIndex = reader.GetOrdinal(Constants.ColumnNames.IsOffshore);
            int employeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeNumber);
            int billRateIndex = reader.GetOrdinal(Constants.ColumnNames.BillRate);

            int forecastedHoursDailyIndex;
            int billingTypeIndex;

            try
            {
                forecastedHoursDailyIndex = reader.GetOrdinal(Constants.ColumnNames.ForecastedHoursDaily);
            }
            catch
            {
                forecastedHoursDailyIndex = -1;
            }

            try
            {
                billingTypeIndex = reader.GetOrdinal(Constants.ColumnNames.BillingType);
            }
            catch
            {
                billingTypeIndex = -1;
            }

            while (reader.Read())
            {
                TimeEntriesGroupByDate dt = null;
                if (!reader.IsDBNull(chargeCodeDateIndex))
                {
                    var dayTotalHoursbyWorkType = new TimeEntryByWorkType()
                    {
                        Note = !reader.IsDBNull(noteIndex) ? reader.GetString(noteIndex) : string.Empty,
                        BillableHours = !reader.IsDBNull(billableHoursIndex) ? reader.GetDouble(billableHoursIndex) : 0d,
                        NonBillableHours = !reader.IsDBNull(nonBillableHoursIndex) ? reader.GetDouble(nonBillableHoursIndex) : 0d,
                        TimeType = new TimeTypeRecord()
                        {
                            Name = !reader.IsDBNull(timeTypeNameIndex) ? reader.GetString(timeTypeNameIndex) : string.Empty,
                            Code = !reader.IsDBNull(timeTypeCodeIndex) ? reader.GetString(timeTypeCodeIndex) : string.Empty
                        },
                        BillRate = !reader.IsDBNull(billRateIndex) ? Convert.ToDouble(reader.GetDecimal(billRateIndex)) : 0d
                    };
                    if (forecastedHoursDailyIndex > -1)
                    {
                        dayTotalHoursbyWorkType.ForecastedHoursDaily = !reader.IsDBNull(forecastedHoursDailyIndex) ? Convert.ToDouble(reader.GetDecimal(forecastedHoursDailyIndex)) : 0d;
                    }
                    if (billingTypeIndex > -1)
                    {
                        dayTotalHoursbyWorkType.BillingType = !reader.IsDBNull(billingTypeIndex) ? reader.GetString(billingTypeIndex) : string.Empty;
                    }
                    dt = new TimeEntriesGroupByDate()
                    {
                        Date = reader.GetDateTime(chargeCodeDateIndex),
                        DayTotalHoursList = new List<TimeEntryByWorkType>()
                                {
                                    dayTotalHoursbyWorkType
                                }
                    };
                }

                int personId = reader.GetInt32(personIdIndex);
                PersonLevelGroupedHours PLGH;
                if (result.Any(r => r.Person.Id == personId))
                {
                    PLGH = result.First(r => r.Person.Id == personId);
                    if (dt != null)
                        PLGH.AddDayTotalHours(dt);
                    PLGH.EstimatedBillings += ((!reader.IsDBNull(billableHoursIndex) ? reader.GetDouble(billableHoursIndex) : 0d) * Convert.ToDouble(reader[billRateIndex]));
                }
                else
                {
                    PLGH = new PersonLevelGroupedHours();
                    Person person = new Person
                    {
                        Id = reader.GetInt32(personIdIndex),
                        FirstName = reader.GetString(firstNameIndex),
                        LastName = reader.GetString(lastNameIndex),
                        ProjectRoleName = reader.GetString(projectRoleNameIndex),
                        IsOffshore = reader.GetBoolean(isOffShoreIndex),
                        EmployeeNumber = reader.GetString(employeeNumberIndex)
                    };
                    PLGH.Person = person;
                    PLGH.TimeEntrySectionId = !reader.IsDBNull(timeEntrySectionIdIndex) ? reader.GetInt32(timeEntrySectionIdIndex) : 0;
                    PLGH.ForecastedHours = Convert.ToDouble(reader[forecastedHoursIndex]);
                    PLGH.EstimatedBillings = ((!reader.IsDBNull(billableHoursIndex) ? reader.GetDouble(billableHoursIndex) : 0d) * Convert.ToDouble(reader[billRateIndex]));
                    if (dt != null)
                    {
                        PLGH.DayTotalHours = new List<TimeEntriesGroupByDate>()
                            {
                                dt
                            };
                    }
                    result.Add(PLGH);
                }
            }
        }

        public static List<WorkTypeLevelGroupedHours> ProjectSummaryReportByWorkType(string projectNumber, int? milestoneId, DateTime? startDate, DateTime? endDate, string categoryNames)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ProjectSummaryReportByWorkType, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectNumber, projectNumber);
                command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneId, milestoneId.HasValue ? (object)milestoneId : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate.HasValue ? (object)startDate : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate.HasValue ? (object)endDate : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.CategoryNamesParam, categoryNames ?? (Object)DBNull.Value);

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
                    if (!reader.HasRows) return result;
                    while (reader.Read())
                    {
                        var proj = ReadProject(reader);
                        result.Add(proj);
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
            if (!reader.HasRows) return;
            while (reader.Read())
            {
                int mileStoneIdIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneId);
                int mileStoneNameIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneName);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
                var milestone = new Milestone
                {
                    Description = reader.GetString(mileStoneNameIndex),
                    Id = reader.GetInt32(mileStoneIdIndex),
                    StartDate = reader.GetDateTime(startDateIndex),
                    ProjectedDeliveryDate = reader.GetDateTime(endDateIndex)
                };

                result.Add(milestone);
            }
        }

        public static List<PersonLevelPayCheck> TimePeriodSummaryByResourcePayCheck(DateTime startDate, DateTime endDate, bool includePersonsWithNoTimeEntries, string personTypes, string titleIds, string timescaleNames, string personStatusIds, string personDivisionIds)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.TimePeriodSummaryByResourcePayCheck, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IncludePersonsWithNoTimeEntriesParam, includePersonsWithNoTimeEntries);

                if (personTypes != null)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonTypesParam, personTypes);
                }

                if (titleIds != null)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.TitleIdsParam, titleIds);
                }

                if (timescaleNames != null)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.TimescaleNamesListParam, timescaleNames);
                }
                if (personStatusIds != null)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonStatusIdsParam, personStatusIds);
                }
                if (personDivisionIds != null)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonDivisionIdsParam, personDivisionIds);
                }

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<PersonLevelPayCheck>();
                    List<TimeTypeRecord> timeTypesList = new List<TimeTypeRecord>();
                    ReadTimeTypeRecords(reader, timeTypesList);
                    reader.NextResult();
                    ReadTimePeriodSummaryByResourcePayCheck(reader, result, timeTypesList);
                    return result;
                }
            }
        }

        private static void ReadTimeTypeRecords(SqlDataReader reader, List<TimeTypeRecord> result)
        {
            if (!reader.HasRows) return;
            int timeTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeId);
            int nameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
            int isPTOIndex = reader.GetOrdinal(Constants.ColumnNames.IsPTOColumn);
            int isHolidayIndex = reader.GetOrdinal(Constants.ColumnNames.IsHolidayColumn);
            int isORTIndex = reader.GetOrdinal(Constants.ColumnNames.IsORTColumn);
            int isSickLeaveIndex = reader.GetOrdinal(Constants.ColumnNames.IsSickLeaveColumn);
            int isUnpaidIndex = reader.GetOrdinal(Constants.ColumnNames.IsUnpaidColoumn);
            int isJuryDutyIndex = reader.GetOrdinal(Constants.ColumnNames.IsJuryDuty);
            int isBereavementIndex = reader.GetOrdinal(Constants.ColumnNames.IsBereavement);

            while (reader.Read())
            {
                TimeTypeRecord tt = new TimeTypeRecord()
                {
                    Id = reader.GetInt32(timeTypeIdIndex),
                    Name = reader.GetString(nameIndex),
                    IsPTOTimeType = reader.GetInt32(isPTOIndex) == 1,
                    IsHolidayTimeType = reader.GetInt32(isHolidayIndex) == 1,
                    IsORTTimeType = reader.GetInt32(isORTIndex) == 1,
                    IsSickLeaveTimeType = reader.GetInt32(isSickLeaveIndex) == 1,
                    IsUnpaidTimeType = reader.GetInt32(isUnpaidIndex) == 1,
                    IsJuryDutyTimeType = reader.GetInt32(isJuryDutyIndex) == 1,
                    IsBereavementTimeType = reader.GetInt32(isBereavementIndex) == 1
                };
                result.Add(tt);
            }
        }

        private static void ReadTimePeriodSummaryByResourcePayCheck(SqlDataReader reader, List<PersonLevelPayCheck> result, List<TimeTypeRecord> timeTypesList)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int timeScaleIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleColumn);
            int employeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeNumber);
            int branchIDIndex = reader.GetOrdinal(Constants.ColumnNames.BranchID);
            int deptIDIndex = reader.GetOrdinal(Constants.ColumnNames.DeptID);
            int totalHoursIndex = reader.GetOrdinal(Constants.ColumnNames.TotalHours);
            //time-off Worktypes i.e. adminstrative worktypes
            int pTOHoursIndex = reader.GetOrdinal(Constants.ColumnNames.PTOHours);
            int holidayHoursIndex = reader.GetOrdinal(Constants.ColumnNames.HolidayHours);
            int juryDutyHoursIndex = reader.GetOrdinal(Constants.ColumnNames.JuryDutyHours);
            int bereavementHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BereavementHours);
            int oRTHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ORTHours);
            int unpaidHoursIndex = reader.GetOrdinal(Constants.ColumnNames.UnpaidHours);
            int sickOrSafeLeaveHoursIndex = reader.GetOrdinal(Constants.ColumnNames.SickOrSafeLeaveHours);
            int paychexIDIndex = reader.GetOrdinal(Constants.ColumnNames.PaychexID);
            int divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);

            while (reader.Read())
            {
                PersonLevelPayCheck PLPC = new PersonLevelPayCheck();
                Person person = new Person
                {
                    Id = reader.GetInt32(personIdIndex),
                    FirstName = reader.GetString(firstNameIndex),
                    LastName = reader.GetString(lastNameIndex),
                    EmployeeNumber = reader.GetString(employeeNumberIndex),
                    CurrentPay = new Pay
                    {
                        TimescaleName = reader.IsDBNull(timeScaleIndex) ? String.Empty : reader.GetString(timeScaleIndex)
                    },
                    PaychexID = reader.IsDBNull(paychexIDIndex) ? string.Empty : reader.GetString(paychexIDIndex),
                    DivisionType = reader.IsDBNull(divisionIdIndex) ? (PersonDivisionType)Enum.Parse(typeof(PersonDivisionType), "0") : (PersonDivisionType)Enum.Parse(typeof(PersonDivisionType), reader.GetInt32(divisionIdIndex).ToString())
                };
                PLPC.Person = person;
                PLPC.BranchID = reader.GetInt32(branchIDIndex);
                PLPC.DeptID = reader.GetInt32(deptIDIndex);
                PLPC.TotalHoursExcludingTimeOff = reader.GetDouble(totalHoursIndex);
                Dictionary<string, double> workTypeLevelTimeOffHours = new Dictionary<string, double>
                    {
                        {timeTypesList.First(t => t.IsPTOTimeType).Name, reader.GetDouble(pTOHoursIndex)},
                        {timeTypesList.First(t => t.IsHolidayTimeType).Name, reader.GetDouble(holidayHoursIndex)},
                        {timeTypesList.First(t => t.IsJuryDutyTimeType).Name, reader.GetDouble(juryDutyHoursIndex)},
                        {
                            timeTypesList.First(t => t.IsBereavementTimeType).Name, reader.GetDouble(bereavementHoursIndex)
                        },
                        {timeTypesList.First(t => t.IsORTTimeType).Name, reader.GetDouble(oRTHoursIndex)},
                        {timeTypesList.First(t => t.IsUnpaidTimeType).Name, reader.GetDouble(unpaidHoursIndex)},
                        {
                            timeTypesList.First(t => t.IsSickLeaveTimeType).Name,
                            reader.GetDouble(sickOrSafeLeaveHoursIndex)
                        }
                    };
                PLPC.WorkTypeLevelTimeOffHours = workTypeLevelTimeOffHours;
                result.Add(PLPC);
            }
        }

        public static List<PersonLevelTimeEntriesHistory> TimeEntryAuditReportByPerson(DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.TimeEntryAuditReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<PersonLevelTimeEntriesHistory>();
                    var persons = new List<Person>();
                    ReadPersons(reader, persons);
                    reader.NextResult();
                    ReadPersonLevelTimeEntriesHistory(reader, result, persons);
                    return result;
                }
            }
        }

        private static void ReadPersonLevelTimeEntriesHistory(SqlDataReader reader, List<PersonLevelTimeEntriesHistory> result, List<Person> persons)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
            int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNameColumn);
            int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
            int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
            int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
            int groupIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn);
            int groupNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn);
            int timeTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeId);
            int timeTypeNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeName);
            int chargeCodeDateIndex = reader.GetOrdinal(Constants.ColumnNames.ChargeCodeDate);
            int modifiedDateIndex = reader.GetOrdinal(Constants.ColumnNames.ModifiedDate);
            int chargeCodeIdIndex = reader.GetOrdinal(Constants.ColumnNames.ChargeCodeId);
            int isChargeableIndex = reader.GetOrdinal(Constants.ColumnNames.IsChargeable);
            int originalHoursIndex = reader.GetOrdinal(Constants.ColumnNames.OriginalHours);
            int actualHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ActualHours);
            int noteIndex = reader.GetOrdinal(Constants.ColumnNames.Note);
            int phaseIndex = reader.GetOrdinal(Constants.ColumnNames.Phase);
            int timeEntrySectionIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeEntrySectionId);

            while (reader.Read())
            {
                var personId = reader.GetInt32(personIdIndex);
                PersonLevelTimeEntriesHistory personLevelTimeEntriesHistory = new PersonLevelTimeEntriesHistory();
                var timeEntryRecord = new TimeEntryRecord
                {
                    ChargeCode = new ChargeCode
                    {
                        ChargeCodeId = reader.GetInt32(chargeCodeIdIndex),
                        Client = new Client
                        {
                            Id = reader.GetInt32(clientIdIndex),
                            Name = reader.GetString(clientNameIndex)
                        },
                        Phase = reader.GetInt32(phaseIndex),
                        Project = new Project
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            Name = reader.GetString(projectNameIndex),
                            ProjectNumber = reader.GetString(projectNumberIndex)
                        },
                        ProjectGroup = new ProjectGroup
                        {
                            Id = reader.GetInt32(groupIdIndex),
                            Name = reader.GetString(groupNameIndex)
                        },
                        TimeType = new TimeTypeRecord
                        {
                            Id = reader.GetInt32(timeTypeIdIndex),
                            Name = reader.GetString(timeTypeNameIndex)
                        },
                        TimeEntrySection = (TimeEntrySectionType)reader.GetInt32(timeEntrySectionIdIndex)
                    },
                    ChargeCodeDate = reader.GetDateTime(chargeCodeDateIndex),
                    IsChargeable = reader.GetBoolean(isChargeableIndex),
                    ActualHours = Convert.ToDouble(reader[actualHoursIndex]),
                    Note = reader.GetString(noteIndex),
                    ModifiedDate = reader.GetDateTime(modifiedDateIndex),
                    OldHours = Convert.ToDouble(reader[originalHoursIndex])
                };

                if (result.Any(p => p.Person.Id == personId))
                {
                    personLevelTimeEntriesHistory = result.First(p => p.Person.Id == personId);

                    personLevelTimeEntriesHistory.TimeEntryRecords.Add(timeEntryRecord);
                }
                else
                {
                    personLevelTimeEntriesHistory = new PersonLevelTimeEntriesHistory
                    {
                        Person = persons.First(p => p.Id == personId),
                        TimeEntryRecords = new List<TimeEntryRecord> { timeEntryRecord }
                    };
                    result.Add(personLevelTimeEntriesHistory);
                }
            }
        }

        public static List<ProjectLevelTimeEntriesHistory> TimeEntryAuditReportByProject(DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.TimeEntryAuditReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<ProjectLevelTimeEntriesHistory>();
                    var persons = new List<Person>();
                    ReadPersons(reader, persons);
                    reader.NextResult();
                    ReadProjectLevelTimeEntriesHistory(reader, result, persons);
                    return result;
                }
            }
        }

        private static void ReadProjectLevelTimeEntriesHistory(SqlDataReader reader, List<ProjectLevelTimeEntriesHistory> result, List<Person> persons)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
            int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNameColumn);
            int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
            int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
            int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
            int groupIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn);
            int groupNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn);
            int timeTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeId);
            int timeTypeNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeName);
            int chargeCodeDateIndex = reader.GetOrdinal(Constants.ColumnNames.ChargeCodeDate);
            int modifiedDateIndex = reader.GetOrdinal(Constants.ColumnNames.ModifiedDate);
            int chargeCodeIdIndex = reader.GetOrdinal(Constants.ColumnNames.ChargeCodeId);
            int isChargeableIndex = reader.GetOrdinal(Constants.ColumnNames.IsChargeable);
            int originalHoursIndex = reader.GetOrdinal(Constants.ColumnNames.OriginalHours);
            int actualHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ActualHours);
            int noteIndex = reader.GetOrdinal(Constants.ColumnNames.Note);
            int phaseIndex = reader.GetOrdinal(Constants.ColumnNames.Phase);
            int timeEntrySectionIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeEntrySectionId);

            while (reader.Read())
            {
                var projectId = reader.GetInt32(projectIdIndex);
                ProjectLevelTimeEntriesHistory projectLevelTimeEntriesHistory = new ProjectLevelTimeEntriesHistory();
                var personId = reader.GetInt32(personIdIndex);

                var timeEntryRecord = new TimeEntryRecord
                {
                    ChargeCode = new ChargeCode
                    {
                        ChargeCodeId = reader.GetInt32(chargeCodeIdIndex),
                        Client = new Client
                        {
                            Id = reader.GetInt32(clientIdIndex),
                            Name = reader.GetString(clientNameIndex)
                        },
                        Phase = reader.GetInt32(phaseIndex),
                        Project = new Project
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            Name = reader.GetString(projectNameIndex),
                            ProjectNumber = reader.GetString(projectNumberIndex)
                        },
                        ProjectGroup = new ProjectGroup
                        {
                            Id = reader.GetInt32(groupIdIndex),
                            Name = reader.GetString(groupNameIndex)
                        },
                        TimeType = new TimeTypeRecord
                        {
                            Id = reader.GetInt32(timeTypeIdIndex),
                            Name = reader.GetString(timeTypeNameIndex)
                        },
                        TimeEntrySection = (TimeEntrySectionType)reader.GetInt32(timeEntrySectionIdIndex)
                    },
                    ChargeCodeDate = reader.GetDateTime(chargeCodeDateIndex),
                    IsChargeable = reader.GetBoolean(isChargeableIndex),
                    ActualHours = Convert.ToDouble(reader[actualHoursIndex]),
                    Note = reader.GetString(noteIndex),
                    ModifiedDate = reader.GetDateTime(modifiedDateIndex),
                    OldHours = Convert.ToDouble(reader[originalHoursIndex])
                };

                if (result.Any(p => p.Project.Id == projectId))
                {
                    projectLevelTimeEntriesHistory = result.First(p => p.Project.Id == projectId);
                    PersonLevelTimeEntriesHistory personLevelTimeEntriesHistory;
                    if (projectLevelTimeEntriesHistory.PersonLevelTimeEntries.Any(p => p.Person.Id == personId))
                    {
                        personLevelTimeEntriesHistory = projectLevelTimeEntriesHistory.PersonLevelTimeEntries.First(p => p.Person.Id == personId);
                        personLevelTimeEntriesHistory.TimeEntryRecords.Add(timeEntryRecord);
                    }
                    else
                    {
                        personLevelTimeEntriesHistory = new PersonLevelTimeEntriesHistory
                        {
                            Person = persons.First(p => p.Id == personId),
                            TimeEntryRecords = new List<TimeEntryRecord> { timeEntryRecord }
                        };
                        projectLevelTimeEntriesHistory.PersonLevelTimeEntries.Add(personLevelTimeEntriesHistory);
                    }
                }
                else
                {
                    projectLevelTimeEntriesHistory = new ProjectLevelTimeEntriesHistory
                    {
                        Project = new Project
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            Name = reader.GetString(projectNameIndex),
                            ProjectNumber = reader.GetString(projectNumberIndex),
                            Client = new Client
                            {
                                Name = reader.GetString(clientNameIndex)
                            },
                            Group = new ProjectGroup
                            {
                                Name = reader.GetString(groupNameIndex)
                            }
                        },
                        PersonLevelTimeEntries = new List<PersonLevelTimeEntriesHistory>
                                {
                                    new PersonLevelTimeEntriesHistory
                                        {
                                            Person = persons.First(p => p.Id == personId),
                                            TimeEntryRecords = new List<TimeEntryRecord> { timeEntryRecord }
                                        }
                                }
                    };
                    result.Add(projectLevelTimeEntriesHistory);
                }
            }
        }

        private static void ReadPersons(SqlDataReader reader, List<Person> result)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int personStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusId);
            int personStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusName);
            int timeScaleIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleColumn);
            int timescaleNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleName);
            int employeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeNumber);

            while (reader.Read())
            {
                var person = ReadBasicPersonDetails(reader, personIdIndex, firstNameIndex, lastNameIndex, personStatusIdIndex, personStatusNameIndex, timeScaleIndex, timescaleNameIndex);
                person.EmployeeNumber = reader.GetString(employeeNumberIndex);
                result.Add(person);
            }
        }

        private static Person ReadBasicPersonDetails(SqlDataReader reader, int personIdIndex, int firstNameIndex, int lastNameIndex, int personStatusIdIndex, int personStatusNameIndex, int timeScaleIndex, int timescaleNameIndex)
        {
            var person = new Person
            {
                Id = reader.GetInt32(personIdIndex),
                FirstName = reader.GetString(firstNameIndex),
                LastName = reader.GetString(lastNameIndex),
                Status = new PersonStatus
                {
                    Id = reader.GetInt32(personStatusIdIndex),
                    Name = reader.GetString(personStatusNameIndex)
                }
            };
            if (!reader.IsDBNull(timeScaleIndex))
            {
                Pay currentPay = new Pay
                {
                    Timescale = (TimescaleType)reader.GetInt32(timeScaleIndex),
                    TimescaleName = reader.GetString(timescaleNameIndex)
                };
                person.CurrentPay = currentPay;
            }

            return person;
        }

        public static List<Person> NewHireReport(DateTime startDate, DateTime endDate, string personStatusIds, string payTypeIds, string practiceIds, bool excludeInternalPractices, string personDivisionIds, string titleIds, string hireDates, string recruiterIds)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.NewHireReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonStatusIdsParam, personStatusIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeScaleIdsParam, payTypeIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExcludeInternalPractices, excludeInternalPractices);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonDivisionIdsParam, personDivisionIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.TitleIdsParam, titleIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.HireDatesParam, hireDates);
                command.Parameters.AddWithValue(Constants.ParameterNames.RecruiterIdsParam, recruiterIds);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var persons = new List<Person>();
                    ReadHumanCapitalPersons(reader, persons);
                    return persons;
                }
            }
        }

        private static void ReadHumanCapitalPersons(SqlDataReader reader, List<Person> result)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int personStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusId);
            int personStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusName);
            int timeScaleIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleColumn);
            int timescaleNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleName);
            int recruiterIdIndex = reader.GetOrdinal(Constants.ColumnNames.RecruiterIdColumn);
            int recruiterFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.RecruiterFirstNameColumn);
            int recruiterLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.RecruiterLastNameColumn);
            int personTitleIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonTitleId);
            int personTitleIndex = reader.GetOrdinal(Constants.ColumnNames.PersonTitle);
            int titleTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleTypeId);
            int titleTypeIndex = reader.GetOrdinal(Constants.ColumnNames.TitleType);
            int divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);
            int hireDateIndex = reader.GetOrdinal(Constants.ColumnNames.HireDateColumn);
            int employeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeNumber);

            int terminationDateIndex;
            try
            {
                terminationDateIndex = reader.GetOrdinal(Constants.ColumnNames.TerminationDateColumn);
            }
            catch
            {
                terminationDateIndex = -1;
            }
            int terminationReasonIdIndex;
            try
            {
                terminationReasonIdIndex = reader.GetOrdinal(Constants.ColumnNames.TerminationReasonIdColumn);
            }
            catch
            {
                terminationReasonIdIndex = -1;
            }

            int terminationReasonIndex;
            try
            {
                terminationReasonIndex = reader.GetOrdinal(Constants.ColumnNames.TerminationReasonColumn);
            }
            catch
            {
                terminationReasonIndex = -1;
            }

            while (reader.Read())
            {
                var person = ReadBasicPersonDetails(reader, personIdIndex, firstNameIndex, lastNameIndex, personStatusIdIndex, personStatusNameIndex, timeScaleIndex, timescaleNameIndex);

                if (!reader.IsDBNull(recruiterIdIndex))
                {
                    person.RecruiterId = reader.GetInt32(recruiterIdIndex);
                    person.RecruiterFirstName = reader.GetString(recruiterFirstNameIndex);
                    person.RecruiterLastName = reader.GetString(recruiterLastNameIndex);
                }
                if (!reader.IsDBNull(personTitleIdIndex))
                {
                    person.Title = new Title
                    {
                        TitleId = reader.GetInt32(personTitleIdIndex),
                        TitleName = reader.GetString(personTitleIndex),
                        TitleType = new TitleType()
                        {
                            TitleTypeId = reader.GetInt32(titleTypeIdIndex),
                            TitleTypeName = reader.GetString(titleTypeIndex)
                        }
                    };
                }

                if (!reader.IsDBNull(divisionIdIndex))
                {
                    person.DivisionType = (PersonDivisionType)reader.GetInt32(divisionIdIndex);
                }

                person.HireDate = reader.GetDateTime(hireDateIndex);
                person.EmployeeNumber = reader.GetString(employeeNumberIndex);

                if (terminationDateIndex > -1 && !reader.IsDBNull(terminationDateIndex))
                {
                    person.TerminationDate = reader.GetDateTime(terminationDateIndex);
                    if (!reader.IsDBNull(terminationReasonIdIndex))
                    {
                        person.TerminationReasonid = reader.GetInt32(terminationReasonIdIndex);
                        person.TerminationReason = reader.GetString(terminationReasonIndex);
                    }
                }
                result.Add(person);
            }
        }

        public static TerminationPersonsInRange TerminationReport(DateTime startDate, DateTime endDate, string payTypeIds, string personStatusIds, string titleIds, string terminationReasonIds, string practiceIds, bool excludeInternalPractices, string personDivisionIds, string recruiterIds, string hireDates, string terminationDates)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.TerminationReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeScaleIdsParam, payTypeIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonStatusIdsParam, personStatusIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.TitleIdsParam, titleIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.TerminationReasonIdsParam, terminationReasonIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExcludeInternalPractices, excludeInternalPractices);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonDivisionIdsParam, personDivisionIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.RecruiterIdsParam, recruiterIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.HireDatesParam, hireDates);
                command.Parameters.AddWithValue(Constants.ParameterNames.TerminationDatesParam, terminationDates);
                TerminationPersonsInRange result = new TerminationPersonsInRange();
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var persons = new List<Person>();
                    ReadHumanCapitalPersons(reader, persons);
                    result.PersonList = persons;
                    result.TerminationsW2SalaryCountInTheRange = result.PersonList.Count(p => p.CurrentPay != null && p.CurrentPay.Timescale == TimescaleType.Salary);
                    result.TerminationsW2HourlyCountInTheRange = result.PersonList.Count(p => p.CurrentPay != null && p.CurrentPay.Timescale == TimescaleType.Hourly);
                    result.Terminations1099HourlyCountInTheRange = result.PersonList.Count(p => p.CurrentPay != null && p.CurrentPay.Timescale == TimescaleType._1099Ctc);
                    result.Terminations1099PORCountInTheRange = result.PersonList.Count(p => p.CurrentPay != null && p.CurrentPay.Timescale == TimescaleType.PercRevenue);
                    return result;
                }
            }
        }

        private static void ReadTerminationPersonsInRange(SqlDataReader reader, List<TerminationPersonsInRange> result)
        {
            if (!reader.HasRows) return;
            int activePersonsAtTheBeginningIndex = reader.GetOrdinal(Constants.ColumnNames.ActivePersonsAtTheBeginning);
            int newHiredInTheRangeIndex = reader.GetOrdinal(Constants.ColumnNames.NewHiredInTheRange);
            int terminationsW2SalaryCountInTheRange = reader.GetOrdinal(Constants.ColumnNames.TerminationsW2SalaryCountInTheRange);
            int terminationsW2HourlyCountInTheRange = reader.GetOrdinal(Constants.ColumnNames.TerminationsW2HourlyCountInTheRange);
            int terminations1099HourlyCountInTheRange = reader.GetOrdinal(Constants.ColumnNames.Terminations1099HourlyCountInTheRange);
            int terminations1099PORCountInTheRange = reader.GetOrdinal(Constants.ColumnNames.Terminations1099PORCountInTheRange);
            int terminationsCountInTheRange = reader.GetOrdinal(Constants.ColumnNames.TerminationsCountInTheRange);
            int newHiredCumulativeInTheRange = reader.GetOrdinal(Constants.ColumnNames.NewHiredCumulativeInTheRange);
            int terminationsCumulativeEmployeeCountInTheRange = reader.GetOrdinal(Constants.ColumnNames.TerminationsCumulativeEmployeeCountInTheRange);
            int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
            int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);

            while (reader.Read())
            {
                TerminationPersonsInRange tpr = new TerminationPersonsInRange
                {
                    StartDate = reader.GetDateTime(startDateIndex),
                    EndDate = reader.GetDateTime(endDateIndex),
                    ActivePersonsCountAtTheBeginning = reader.GetInt32(activePersonsAtTheBeginningIndex),
                    NewHiresCountInTheRange = reader.GetInt32(newHiredInTheRangeIndex),
                    TerminationsW2SalaryCountInTheRange = reader.GetInt32(terminationsW2SalaryCountInTheRange),
                    TerminationsW2HourlyCountInTheRange = reader.GetInt32(terminationsW2HourlyCountInTheRange),
                    Terminations1099HourlyCountInTheRange = reader.GetInt32(terminations1099HourlyCountInTheRange),
                    Terminations1099PORCountInTheRange = reader.GetInt32(terminations1099PORCountInTheRange),
                    TerminationsCountInTheRange = reader.GetInt32(terminationsCountInTheRange),
                    NewHiredCumulativeInTheRange = reader.GetInt32(newHiredCumulativeInTheRange),
                    TerminationsCumulativeEmployeeCountInTheRange =
                        reader.GetInt32(terminationsCumulativeEmployeeCountInTheRange)
                };
                result.Add(tpr);
            }
        }

        public static List<TerminationPersonsInRange> TerminationReportGraph(DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.TerminationReportGraph, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                List<TerminationPersonsInRange> result = new List<TerminationPersonsInRange>();
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadTerminationPersonsInRange(reader, result);
                    return result;
                }
            }
        }

        #region ConsultingDemand

        public static List<ConsultantGroupbyTitleSkill> ConsultingDemandSummary(DateTime startDate, DateTime endDate, string titles, string skills)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetConsultantDemandForPeriod, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsSummary, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.Titles, titles == null ? DBNull.Value : (object)titles);
                command.Parameters.AddWithValue(Constants.ParameterNames.Skills, skills == null ? DBNull.Value : (object)skills);
                command.Parameters.AddWithValue(Constants.ParameterNames.OrderByCerteriaParam, Constants.ColumnNames.MonthStartDate + "," + Constants.ColumnNames.Title + "," + Constants.ColumnNames.Skill);

                List<ConsultantGroupbyTitleSkill> result = new List<ConsultantGroupbyTitleSkill>();
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadConsultantDemandSummary(reader, result);
                    return result;
                }
            }
        }

        private static void ReadConsultantDemandSummary(SqlDataReader reader, List<ConsultantGroupbyTitleSkill> result)
        {
            if (!reader.HasRows) return;
            int monthStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.MonthStartDate);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.Skill);
            int countIndex = reader.GetOrdinal(Constants.ColumnNames.Count);
            while (reader.Read())
            {
                ConsultantGroupbyTitleSkill consultant;
                string title = reader.GetString(lastNameIndex);
                string skill = reader.GetString(firstNameIndex);
                if (result.Any(p => p.Title == title && p.Skill == skill))
                {
                    consultant = result.First(p => p.Title == title && p.Skill == skill);
                }
                else
                {
                    consultant = new ConsultantGroupbyTitleSkill
                    {
                        Title = title,
                        Skill = skill,
                        MonthCount = new Dictionary<string, int>()
                    };
                    result.Add(consultant);
                }
                string month = reader.GetDateTime(monthStartDateIndex).ToString(Constants.Formatting.FullMonthYearFormat);
                consultant.MonthCount[month] = reader.GetInt32(countIndex);
            }
        }

        public static List<ConsultantGroupBySalesStage> ConsultingDemandDetailsBySalesStage(DateTime startDate, DateTime endDate, string titles, string skills, string sortColumns)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetConsultantDemandForPeriod, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsDetail, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.GroupByTitleSkill, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.ViewByTitleSkill, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.Titles, titles == null ? DBNull.Value : (object)titles);
                command.Parameters.AddWithValue(Constants.ParameterNames.Skills, skills == null ? DBNull.Value : (object)skills);
                command.Parameters.AddWithValue(Constants.ParameterNames.OrderByCerteriaParam, string.IsNullOrEmpty(sortColumns) ? Constants.ColumnNames.Title + "," + Constants.ColumnNames.Skill : sortColumns);
                List<ConsultantGroupBySalesStage> result = new List<ConsultantGroupBySalesStage>();
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadConsultantDemandDetailsBySalesStage(reader, result);
                    if (!string.IsNullOrEmpty(sortColumns) && sortColumns.ToLower().Contains("count") && !sortColumns.ToLower().Contains("account"))
                    {
                        result = sortColumns.ToLower().Contains("count desc") ? result.OrderByDescending(p => p.TotalCount).ToList() : result.OrderBy(p => p.TotalCount).ToList();
                    }
                    return result;
                }
            }
        }

        private static void ReadConsultantDemandDetailsBySalesStage(SqlDataReader reader, List<ConsultantGroupBySalesStage> result)
        {
            if (!reader.HasRows) return;
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.Skill);
            int opportunityNumberIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityNumberColumn);
            int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
            int countIndex = reader.GetOrdinal(Constants.ColumnNames.Count);
            int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.AccountName);
            int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
            int projectDescrIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectDescription);
            int resourceStartDate = reader.GetOrdinal(Constants.ColumnNames.ResourceStartDate);
            int opportunityIdIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityIdColumn);
            int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
            int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientId);
            int salesStageIndex = reader.GetOrdinal(Constants.ColumnNames.SalesStage);
            while (reader.Read())
            {
                ConsultantGroupBySalesStage consultant;
                string salesStage = reader.GetString(salesStageIndex);
                if (result.Any(c => c.SalesStage == salesStage))
                {
                    consultant = result.First(c => c.SalesStage == salesStage);
                }
                else
                {
                    consultant = new ConsultantGroupBySalesStage
                    {
                        SalesStage = salesStage,
                        ConsultantDetailsBySalesStage = new List<ConsultantDemandDetailsByMonth>()
                    };
                    result.Add(consultant);
                }
                ConsultantDemandDetailsByMonth consultantdetails = new ConsultantDemandDetailsByMonth
                {
                    Title = !reader.IsDBNull(lastNameIndex) ? reader.GetString(lastNameIndex) : string.Empty,
                    Skill = !reader.IsDBNull(firstNameIndex) ? reader.GetString(firstNameIndex) : string.Empty,
                    OpportunityId = !reader.IsDBNull(opportunityIdIndex) ? reader.GetInt32(opportunityIdIndex) : -1,
                    OpportunityNumber =
                        !reader.IsDBNull(opportunityNumberIndex)
                            ? reader.GetString(opportunityNumberIndex)
                            : string.Empty,
                    ProjectDescription =
                        !reader.IsDBNull(projectDescrIndex) ? reader.GetString(projectDescrIndex) : string.Empty,
                    ProjectId = !reader.IsDBNull(projectIdIndex) ? reader.GetInt32(projectIdIndex) : -1,
                    ProjectNumber =
                        !reader.IsDBNull(projectNumberIndex) ? reader.GetString(projectNumberIndex) : string.Empty,
                    AccountId = !reader.IsDBNull(clientIdIndex) ? reader.GetInt32(clientIdIndex) : -1,
                    Count = !reader.IsDBNull(countIndex) ? reader.GetInt32(countIndex) : -1,
                    AccountName =
                        !reader.IsDBNull(clientNameIndex) ? reader.GetString(clientNameIndex) : string.Empty,
                    ProjectName =
                        !reader.IsDBNull(projectNameIndex) ? reader.GetString(projectNameIndex) : string.Empty,
                    ResourceStartDate =
                        !reader.IsDBNull(resourceStartDate)
                            ? reader.GetDateTime(resourceStartDate)
                            : DateTime.MinValue
                };
                consultant.ConsultantDetailsBySalesStage.Add(consultantdetails);
            }
        }

        public static List<ConsultantGroupbyTitleSkill> ConsultingDemandDetailsByTitleSkill(DateTime startDate, DateTime endDate, string titles, string skills, string sortColumns)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetConsultantDemandForPeriod, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsDetail, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.GroupByTitleSkill, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.ViewByTitleSkill, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.Titles, titles == null ? DBNull.Value : (object)titles);
                command.Parameters.AddWithValue(Constants.ParameterNames.Skills, skills == null ? DBNull.Value : (object)skills);
                command.Parameters.AddWithValue(Constants.ParameterNames.OrderByCerteriaParam, string.IsNullOrEmpty(sortColumns) ? Constants.ColumnNames.Title + "," + Constants.ColumnNames.Skill : sortColumns);
                List<ConsultantGroupbyTitleSkill> result = new List<ConsultantGroupbyTitleSkill>();
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadConsultantDemandDetailsByTitle(reader, result);
                    if (!string.IsNullOrEmpty(sortColumns) && sortColumns.ToLower().Contains("count") && !sortColumns.ToLower().Contains("account"))
                    {
                        result = sortColumns.ToLower().Contains("count desc") ? result.OrderByDescending(p => p.TotalCount).ToList() : result.OrderBy(p => p.TotalCount).ToList();
                    }
                    return result;
                }
            }
        }

        private static void ReadConsultantDemandDetailsByTitle(SqlDataReader reader, List<ConsultantGroupbyTitleSkill> result)
        {
            if (!reader.HasRows) return;
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.Skill);
            int opportunityNumberIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityNumberColumn);
            int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
            int countIndex = reader.GetOrdinal(Constants.ColumnNames.Count);
            int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.AccountName);
            int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
            int projectDescrIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectDescription);
            int resourceStartDate = reader.GetOrdinal(Constants.ColumnNames.ResourceStartDate);
            int opportunityIdIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityIdColumn);
            int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
            int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientId);
            int salesStageIndex = reader.GetOrdinal(Constants.ColumnNames.SalesStage);
            while (reader.Read())
            {
                ConsultantGroupbyTitleSkill consultant;
                string title = reader.GetString(lastNameIndex);
                string skill = reader.GetString(firstNameIndex);
                if (result.Any(c => c.Title == title && c.Skill == skill))
                {
                    consultant = result.First(c => c.Title == title && c.Skill == skill);
                }
                else
                {
                    consultant = new ConsultantGroupbyTitleSkill
                    {
                        Title = title,
                        Skill = skill,
                        ConsultantDetails = new List<ConsultantDemandDetails>()
                    };
                    result.Add(consultant);
                }
                ConsultantDemandDetails consultantdetails = new ConsultantDemandDetails
                {
                    OpportunityId = !reader.IsDBNull(opportunityIdIndex) ? reader.GetInt32(opportunityIdIndex) : -1,
                    OpportunityNumber =
                        !reader.IsDBNull(opportunityNumberIndex)
                            ? reader.GetString(opportunityNumberIndex)
                            : string.Empty,
                    SalesStage =
                        !reader.IsDBNull(salesStageIndex) ? reader.GetString(salesStageIndex) : string.Empty,
                    ProjectDescription =
                        !reader.IsDBNull(projectDescrIndex) ? reader.GetString(projectDescrIndex) : string.Empty,
                    ProjectId = !reader.IsDBNull(projectIdIndex) ? reader.GetInt32(projectIdIndex) : -1,
                    ProjectNumber =
                        !reader.IsDBNull(projectNumberIndex) ? reader.GetString(projectNumberIndex) : string.Empty,
                    AccountId = !reader.IsDBNull(clientIdIndex) ? reader.GetInt32(clientIdIndex) : -1,
                    Count = !reader.IsDBNull(countIndex) ? reader.GetInt32(countIndex) : -1,
                    AccountName =
                        !reader.IsDBNull(clientNameIndex) ? reader.GetString(clientNameIndex) : string.Empty,
                    ProjectName =
                        !reader.IsDBNull(projectNameIndex) ? reader.GetString(projectNameIndex) : string.Empty,
                    ResourceStartDate =
                        !reader.IsDBNull(resourceStartDate)
                            ? reader.GetDateTime(resourceStartDate)
                            : DateTime.MinValue
                };
                consultant.ConsultantDetails.Add(consultantdetails);
            }
        }

        public static List<ConsultantGroupByMonth> ConsultingDemandDetailsByMonth(DateTime startDate, DateTime endDate, string titles, string skills, string salesStages, string sortColumns, bool isFromPipelinePopUp)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetConsultantDemandForPeriod, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsDetail, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.GroupByMonth, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.ViewByTitleSkill, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.Titles, titles == null ? DBNull.Value : (object)titles);
                command.Parameters.AddWithValue(Constants.ParameterNames.Skills, skills == null ? DBNull.Value : (object)skills);
                command.Parameters.AddWithValue(Constants.ParameterNames.SalesStages, salesStages == null ? DBNull.Value : (object)salesStages);
                command.Parameters.AddWithValue(Constants.ParameterNames.OrderByCerteriaParam, string.IsNullOrEmpty(sortColumns) ? Constants.ColumnNames.MonthStartDate + "," + Constants.ColumnNames.Title + "," + Constants.ColumnNames.Skill : sortColumns);
                List<ConsultantGroupByMonth> result = new List<ConsultantGroupByMonth>();
                List<DateTime> months = Utils.GetMonthYearWithInThePeriod(startDate, endDate);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadConsultantDemandDetailsByMonth(reader, result, isFromPipelinePopUp);
                    foreach (var month in months)
                    {
                        if (result.Any(r => r.MonthStartDate == month)) continue;
                        ConsultantGroupByMonth res = new ConsultantGroupByMonth
                        {
                            MonthStartDate = month,
                            ConsultantDetailsByMonth = new List<ConsultantDemandDetailsByMonth>()
                        };
                        result.Add(res);
                    }
                    if (!string.IsNullOrEmpty(sortColumns) && sortColumns.ToLower().IndexOf(Constants.ColumnNames.MonthStartDate.ToLower()) == 0)
                    {
                        result = sortColumns.ToLower().Contains("desc") ? result.OrderByDescending(r => r.MonthStartDate).ToList() : result.OrderBy(r => r.MonthStartDate).ToList();
                    }
                    if (!string.IsNullOrEmpty(sortColumns) && sortColumns.ToLower().Contains("count") && !sortColumns.ToLower().Contains("account"))
                    {
                        result = sortColumns.ToLower().Contains("count desc") ? result.OrderByDescending(p => p.TotalCount).ToList() : result.OrderBy(p => p.TotalCount).ToList();
                    }
                    return result;
                }
            }
        }

        private static void ReadConsultantDemandDetailsByMonth(SqlDataReader reader, List<ConsultantGroupByMonth> result, bool isFromPipelinePopUp)
        {
            if (!reader.HasRows) return;
            int monthStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.MonthStartDate);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.Skill);
            int opportunityNumberIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityNumberColumn);
            int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
            int projectDescIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectDescription);
            int countIndex = reader.GetOrdinal(Constants.ColumnNames.Count);
            int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.AccountName);
            int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
            int resourceStartDate = reader.GetOrdinal(Constants.ColumnNames.ResourceStartDate);
            int opportunityIdIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityIdColumn);
            int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
            int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientId);
            int salesStageIndex = reader.GetOrdinal(Constants.ColumnNames.SalesStage);

            while (reader.Read())
            {
                ConsultantGroupByMonth consultant;
                DateTime month = reader.GetDateTime(monthStartDateIndex);
                if (result.Any(m => m.MonthStartDate == month))
                {
                    consultant = result.First(m => m.MonthStartDate == month);
                }
                else
                {
                    consultant = new ConsultantGroupByMonth
                    {
                        MonthStartDate = month,
                        ConsultantDetailsByMonth = new List<ConsultantDemandDetailsByMonth>()
                    };
                    result.Add(consultant);
                }
                ConsultantDemandDetailsByMonth consultantdet = new ConsultantDemandDetailsByMonth
                {
                    Title = !reader.IsDBNull(lastNameIndex) ? reader.GetString(lastNameIndex) : string.Empty,
                    SalesStage =
                        !reader.IsDBNull(salesStageIndex) ? reader.GetString(salesStageIndex) : string.Empty,
                    ProjectDescription =
                        !reader.IsDBNull(projectDescIndex) ? reader.GetString(projectDescIndex) : string.Empty,
                    Skill = !reader.IsDBNull(firstNameIndex) ? reader.GetString(firstNameIndex) : string.Empty,
                    OpportunityId = !reader.IsDBNull(opportunityIdIndex) ? reader.GetInt32(opportunityIdIndex) : -1,
                    OpportunityNumber =
                        !reader.IsDBNull(opportunityNumberIndex)
                            ? reader.GetString(opportunityNumberIndex)
                            : string.Empty,
                    ProjectNumber =
                        !reader.IsDBNull(projectNumberIndex) ? reader.GetString(projectNumberIndex) : string.Empty,
                    ProjectId = !reader.IsDBNull(projectIdIndex) ? reader.GetInt32(projectIdIndex) : -1,
                    AccountId = !reader.IsDBNull(clientIdIndex) ? reader.GetInt32(clientIdIndex) : -1,
                    Count = !reader.IsDBNull(countIndex) ? reader.GetInt32(countIndex) : -1,
                    AccountName =
                        !reader.IsDBNull(clientNameIndex) ? reader.GetString(clientNameIndex) : string.Empty,
                    ProjectName =
                        !reader.IsDBNull(projectNameIndex) ? reader.GetString(projectNameIndex) : string.Empty,
                    ResourceStartDate =
                        !reader.IsDBNull(resourceStartDate)
                            ? reader.GetDateTime(resourceStartDate)
                            : DateTime.MinValue
                };
                if (isFromPipelinePopUp)
                {
                    for (int i = 0; i < consultantdet.Count; i++)
                    {
                        consultant.ConsultantDetailsByMonth.Add(consultantdet);
                    }
                }
                else
                {
                    consultant.ConsultantDetailsByMonth.Add(consultantdet);
                }
            }
        }

        public static Dictionary<string, int> ConsultingDemandGraphsByTitle(DateTime startDate, DateTime endDate, string titles, string salesStages)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetConsultantDemandForPeriod, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsGraph, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.Titles, titles == null ? DBNull.Value : (object)titles);
                command.Parameters.AddWithValue(Constants.ParameterNames.SalesStages, salesStages == null ? DBNull.Value : (object)salesStages);
                command.Parameters.AddWithValue(Constants.ParameterNames.GroupByMonth, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.ViewByTitle, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.OrderByCerteriaParam, Constants.ColumnNames.MonthStartDate);
                Dictionary<string, int> result = new Dictionary<string, int>();
                List<DateTime> months = Utils.GetMonthYearWithInThePeriod(startDate, endDate);
                foreach (DateTime monthStartDate in months)
                {
                    result.Add(monthStartDate.ToString(Constants.Formatting.MonthYearFormat), 0);
                }
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadConsultantDemandGraphs(reader, result);
                    return result;
                }
            }
        }

        private static void ReadConsultantDemandGraphs(SqlDataReader reader, Dictionary<string, int> result)
        {
            if (!reader.HasRows) return;
            int monthStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.MonthStartDate);
            int countIndex = reader.GetOrdinal(Constants.ColumnNames.Count);
            while (reader.Read())
            {
                string month = reader.GetDateTime(monthStartDateIndex).ToString(Constants.Formatting.MonthYearFormat);
                int count = reader.GetInt32(countIndex);
                if (result.ContainsKey(month))
                {
                    result[month] += count;
                }
                else
                {
                    result.Add(month, count);
                }
            }
        }

        public static Dictionary<string, int> ConsultingDemandGraphsBySkills(DateTime startDate, DateTime endDate, string skills, string salesStages)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetConsultantDemandForPeriod, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsGraph, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.Skills, skills == null ? DBNull.Value : (object)skills);
                command.Parameters.AddWithValue(Constants.ParameterNames.SalesStages, salesStages == null ? DBNull.Value : (object)salesStages);
                command.Parameters.AddWithValue(Constants.ParameterNames.GroupByMonth, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.ViewBySkill, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.OrderByCerteriaParam, Constants.ColumnNames.MonthStartDate);
                connection.Open();
                Dictionary<string, int> result = new Dictionary<string, int>();
                List<DateTime> months = Utils.GetMonthYearWithInThePeriod(startDate, endDate);
                foreach (DateTime monthStartDate in months)
                {
                    result.Add(monthStartDate.ToString(Constants.Formatting.MonthYearFormat), 0);
                }

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadConsultantDemandGraphs(reader, result);
                    return result;
                }
            }
        }

        public static List<ConsultantGroupbyTitle> ConsultingDemandTransactionReportByTitle(DateTime startDate, DateTime endDate, string titles, string sortColumns, string salesStages)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetConsultantDemandForPeriod, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsDetail, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.GroupByTitle, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.ViewBySkill, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.Titles, titles == null ? DBNull.Value : (object)titles);
                command.Parameters.AddWithValue(Constants.ParameterNames.SalesStages, salesStages == null ? DBNull.Value : (object)salesStages);
                command.Parameters.AddWithValue(Constants.ParameterNames.OrderByCerteriaParam, string.IsNullOrEmpty(sortColumns) ? Constants.ColumnNames.Title + "," + Constants.ColumnNames.Skill : sortColumns);
                List<ConsultantGroupbyTitle> result = new List<ConsultantGroupbyTitle>();
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadConsultantTransactionReportByMonthByTitle(reader, result);
                    return result;
                }
            }
        }

        private static void ReadConsultantTransactionReportByMonthByTitle(SqlDataReader reader, List<ConsultantGroupbyTitle> result)
        {
            if (!reader.HasRows) return;
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.Skill);
            int opportunityNumberIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityNumberColumn);
            int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
            int projectDescIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectDescription);
            int countIndex = reader.GetOrdinal(Constants.ColumnNames.Count);
            int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.AccountName);
            int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
            int resourceStartDate = reader.GetOrdinal(Constants.ColumnNames.ResourceStartDate);
            int opportunityIdIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityIdColumn);
            int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
            int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientId);
            int salesStageIndex = reader.GetOrdinal(Constants.ColumnNames.SalesStage);
            while (reader.Read())
            {
                ConsultantGroupbyTitle consultant;
                string title = reader.GetString(lastNameIndex);
                if (result.Any(c => c.Title == title))
                {
                    consultant = result.First(c => c.Title == title);
                }
                else
                {
                    consultant = new ConsultantGroupbyTitle
                    {
                        Title = title,
                        ConsultantDetails = new List<ConsultantDemandDetailsByMonthByTitle>()
                    };
                    result.Add(consultant);
                }
                ConsultantDemandDetailsByMonthByTitle consultantdet = new ConsultantDemandDetailsByMonthByTitle
                {
                    SalesStage =
                        !reader.IsDBNull(salesStageIndex) ? reader.GetString(salesStageIndex) : string.Empty,
                    Skill = reader.GetString(firstNameIndex),
                    OpportunityId = !reader.IsDBNull(opportunityIdIndex) ? reader.GetInt32(opportunityIdIndex) : -1,
                    ProjectId = !reader.IsDBNull(projectIdIndex) ? reader.GetInt32(projectIdIndex) : -1,
                    OpportunityNumber =
                        !reader.IsDBNull(opportunityNumberIndex)
                            ? reader.GetString(opportunityNumberIndex)
                            : string.Empty,
                    ProjectDescription =
                        !reader.IsDBNull(projectDescIndex) ? reader.GetString(projectDescIndex) : string.Empty,
                    ProjectNumber =
                        !reader.IsDBNull(projectNumberIndex) ? reader.GetString(projectNumberIndex) : string.Empty,
                    AccountId = reader.GetInt32(clientIdIndex),
                    Count = reader.GetInt32(countIndex),
                    AccountName = reader.GetString(clientNameIndex),
                    ProjectName = reader.GetString(projectNameIndex),
                    ResourceStartDate = reader.GetDateTime(resourceStartDate)
                };
                for (int i = 0; i < consultantdet.Count; i++)
                {
                    consultant.ConsultantDetails.Add(consultantdet);
                }
            }
        }

        public static List<ConsultantGroupbySkill> ConsultingDemandTransactionReportBySkill(DateTime startDate, DateTime endDate, string skills, string sortColumns, string salesStages)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetConsultantDemandForPeriod, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsDetail, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.GroupBySkill, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.ViewByTitle, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.Skills, skills == null ? DBNull.Value : (object)skills);
                command.Parameters.AddWithValue(Constants.ParameterNames.SalesStages, salesStages == null ? DBNull.Value : (object)salesStages);
                command.Parameters.AddWithValue(Constants.ParameterNames.OrderByCerteriaParam, string.IsNullOrEmpty(sortColumns) ? Constants.ColumnNames.Skill + "," + Constants.ColumnNames.Title : sortColumns);
                List<ConsultantGroupbySkill> result = new List<ConsultantGroupbySkill>();
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadConsultantTransactionReportByMonthBySkill(reader, result);
                    return result;
                }
            }
        }

        private static void ReadConsultantTransactionReportByMonthBySkill(SqlDataReader reader, List<ConsultantGroupbySkill> result)
        {
            if (!reader.HasRows) return;
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.Skill);
            int opportunityNumberIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityNumberColumn);
            int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
            int projectDescrIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectDescription);
            int countIndex = reader.GetOrdinal(Constants.ColumnNames.Count);
            int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.AccountName);
            int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
            int resourceStartDate = reader.GetOrdinal(Constants.ColumnNames.ResourceStartDate);
            int opportunityIdIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityIdColumn);
            int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
            int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientId);
            int salesStageIndex = reader.GetOrdinal(Constants.ColumnNames.SalesStage);

            while (reader.Read())
            {
                ConsultantGroupbySkill consultant;
                string skill = reader.GetString(firstNameIndex);
                if (result.Any(c => c.Skill == skill))
                {
                    consultant = result.First(c => c.Skill == skill);
                }
                else
                {
                    consultant = new ConsultantGroupbySkill
                    {
                        Skill = skill,
                        ConsultantDetails = new List<ConsultantDemandDetailsByMonthBySkill>()
                    };
                    result.Add(consultant);
                }
                ConsultantDemandDetailsByMonthBySkill consultantdet = new ConsultantDemandDetailsByMonthBySkill
                {
                    SalesStage =
                        !reader.IsDBNull(salesStageIndex) ? reader.GetString(salesStageIndex) : string.Empty,
                    Title = reader.GetString(lastNameIndex),
                    OpportunityId = !reader.IsDBNull(opportunityIdIndex) ? reader.GetInt32(opportunityIdIndex) : -1,
                    ProjectId = !reader.IsDBNull(projectIdIndex) ? reader.GetInt32(projectIdIndex) : -1,
                    AccountId = reader.GetInt32(clientIdIndex),
                    Count = reader.GetInt32(countIndex),
                    AccountName = reader.GetString(clientNameIndex),
                    ProjectName = reader.GetString(projectNameIndex),
                    ResourceStartDate = reader.GetDateTime(resourceStartDate),
                    OpportunityNumber =
                        !reader.IsDBNull(opportunityNumberIndex)
                            ? reader.GetString(opportunityNumberIndex)
                            : string.Empty,
                    ProjectDescription =
                        !reader.IsDBNull(projectDescrIndex) ? reader.GetString(projectDescrIndex) : string.Empty,
                    ProjectNumber =
                        !reader.IsDBNull(projectNumberIndex) ? reader.GetString(projectNumberIndex) : string.Empty
                };
                for (int i = 0; i < consultantdet.Count; i++)
                {
                    consultant.ConsultantDetails.Add(consultantdet);
                }
            }
        }

        public static Dictionary<string, int> ConsultingDemandGrphsGroupsByTitle(DateTime startDate, DateTime endDate, string salesStages)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetConsultantDemandForPeriod, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsGraph, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.GroupByTitle, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.ViewByTitle, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.OrderByCerteriaParam, Constants.ColumnNames.Count);
                command.Parameters.AddWithValue(Constants.ParameterNames.SalesStages, salesStages == null ? DBNull.Value : (object)salesStages);
                Dictionary<string, int> result = new Dictionary<string, int>();
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadConsultantDemandGrphsGroupsByTitle(reader, result);
                    return result;
                }
            }
        }

        private static void ReadConsultantDemandGrphsGroupsByTitle(SqlDataReader reader, Dictionary<string, int> result)
        {
            if (reader.HasRows)
            {
                int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                int countIndex = reader.GetOrdinal(Constants.ColumnNames.Count);
                while (reader.Read())
                {
                    string title = reader.GetString(titleIndex);
                    int count = reader.GetInt32(countIndex);
                    result.Add(title, count);
                }
            }
            else
            {
                result.Add(" ", 0);
            }
        }

        public static Dictionary<string, int> ConsultingDemandGrphsGroupsBySkill(DateTime startDate, DateTime endDate, string salesStages)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetConsultantDemandForPeriod, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsGraph, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.GroupBySkill, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.ViewBySkill, true);
                command.Parameters.AddWithValue(Constants.ParameterNames.OrderByCerteriaParam, Constants.ColumnNames.Count);
                command.Parameters.AddWithValue(Constants.ParameterNames.SalesStages, salesStages == null ? DBNull.Value : (object)salesStages);
                Dictionary<string, int> result = new Dictionary<string, int>();
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadConsultantDemandGrphsGroupsBySkill(reader, result);
                    return result;
                }
            }
        }

        private static void ReadConsultantDemandGrphsGroupsBySkill(SqlDataReader reader, Dictionary<string, int> result)
        {
            if (reader.HasRows)
            {
                int skillIndex = reader.GetOrdinal(Constants.ColumnNames.Skill);
                int countIndex = reader.GetOrdinal(Constants.ColumnNames.Count);
                while (reader.Read())
                {
                    string skill = reader.GetString(skillIndex);
                    int count = reader.GetInt32(countIndex);
                    result.Add(skill, count);
                }
            }
            else
            {
                result.Add(" ", 0);
            }
        }

        #endregion ConsultingDemand

        public static List<Project> GetAttainmentProjectListMultiParameters(
          string clientIds,
          bool showProjected,
          bool showCompleted,
          bool showActive,
          bool showInternal,
          bool showExperimental,
            bool showProposed,
          bool showInactive,
          bool showAtRisk,
          DateTime periodStart,
          DateTime periodEnd,
          string salespersonIdsList,
          string practiceManagerIdsList,
          string practiceIdsList,
          string projectGroupIdsList,
          ProjectCalculateRangeType includeCurentYearFinancials,
          bool excludeInternalPractices,
          string userLogin,
           bool IsMonthsColoumnsShown,
          bool IsQuarterColoumnsShown,
          bool IsYearToDateColoumnsShown,
          bool getFinancialsFromCache)
        {
            List<Project> result =
                AttainmentProjectList(
                clientIds,
                showProjected,
                showCompleted,
                showActive,
                showInternal,
                showExperimental,
                showProposed,
                showInactive,
                showAtRisk,
                periodStart,
                periodEnd,
                salespersonIdsList,
                practiceManagerIdsList,
                practiceIdsList,
                projectGroupIdsList,
                excludeInternalPractices,
                userLogin);
            if (!getFinancialsFromCache)
            {
                return LoadFinancialsAndMilestonePersonInfo(result, periodStart, periodEnd, includeCurentYearFinancials, IsMonthsColoumnsShown, IsQuarterColoumnsShown, IsYearToDateColoumnsShown, getFinancialsFromCache);
            }
            ComputedFinancialsDAL.LoadFinancialsPeriodForProjectsFromCache(result, periodStart, periodEnd, true);
            CalculateTotalFinancials(result);
            return result;
        }

        private static List<Project>
                  LoadFinancialsAndMilestonePersonInfo(
                      List<Project> result,
                      DateTime periodStart,
                      DateTime periodEnd,
                      ProjectCalculateRangeType calculatePeriodType,
            bool IsMonthsColoumnsShown,
                      bool IsQuarterColoumnsShown,
       bool IsYearToDateColoumnsShown,
          bool IsSummaryCache
                  )
        {
            LoadFinancialsPeriodForProjects(result, periodStart, periodEnd, IsMonthsColoumnsShown, IsQuarterColoumnsShown, IsYearToDateColoumnsShown, IsSummaryCache);

            CalculateTotalFinancials(result);

            return result;
        }

        public static void LoadFinancialsPeriodForProjects(
           List<Project> projects, DateTime startDate, DateTime endDate, bool IsMonthsColoumnsShown, bool IsQuarterColoumnsShown, bool IsYearToDateColoumnsShown, bool IsSummaryCache)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(
                Constants.ProcedureNames.Reports.AttainmentFinancialListByProject, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, DataTransferObjects.Utils.Generic.IdsListToString(projects));
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.CalculateQuarterValues, IsQuarterColoumnsShown);
                command.Parameters.AddWithValue(Constants.ParameterNames.CalculateYearToDateValues, IsYearToDateColoumnsShown);
                command.Parameters.AddWithValue(Constants.ParameterNames.CalculateMonthValues, IsMonthsColoumnsShown);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsSummaryCache, IsSummaryCache);

                connection.Open();

                projects.ForEach(delegate(Project project)
                {
                    if (project.ProjectedFinancialsByRange == null)
                        project.ProjectedFinancialsByRange =
                            new Dictionary<RangeType, ComputedFinancials>();
                });

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadMonthlyFinancialsForListOfProjects(reader, projects);
                }
            }
        }

        public static void ReadMonthlyFinancialsForListOfProjects(DbDataReader reader, List<Project> projects)
        {
            if (!reader.HasRows) return;
            int financialDateIndex = reader.GetOrdinal(Constants.ColumnNames.FinancialDateColumn);
            int monthEndIndex = reader.GetOrdinal(Constants.ColumnNames.MonthEnd);
            int revenueIndex = reader.GetOrdinal(Constants.ColumnNames.RevenueColumn);
            int grossMarginIndex = reader.GetOrdinal(Constants.ColumnNames.GrossMarginColumn);
            int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
            int rangeTypeIndex = reader.GetOrdinal(Constants.ColumnNames.RangeType);
            int actualRevenueIndex = -1;
            int actualGrossMarginIndex = -1;
            int previousMonthsActualRevenueIndex = -1;
            int previousMonthsActualGrossMarginIndex = -1;
            try
            {
                actualRevenueIndex = reader.GetOrdinal(Constants.ColumnNames.ActualRevenue);
                actualGrossMarginIndex = reader.GetOrdinal(Constants.ColumnNames.ActualGrossMargin);
            }
            catch { }
            try
            {
                previousMonthsActualRevenueIndex = reader.GetOrdinal(Constants.ColumnNames.PreviousMonthActualRevenue);
                previousMonthsActualGrossMarginIndex = reader.GetOrdinal(Constants.ColumnNames.PreviousMonthActualGrossMargin);
            }
            catch { }
            var quarterList = new List<QuarterRange>();
            while (reader.Read())
            {
                var project = new Project { Id = reader.GetInt32(projectIdIndex) };
                var financials =
                    ReadComputedFinancials(
                        reader,
                        financialDateIndex,
                        monthEndIndex,
                        rangeTypeIndex,
                        revenueIndex,
                        grossMarginIndex,
                        actualRevenueIndex,
                        actualGrossMarginIndex,
                        previousMonthsActualRevenueIndex,
                        previousMonthsActualGrossMarginIndex);
                var i = projects.IndexOf(project);
                projects[i].ProjectedFinancialsByRange.Add(financials.FinancialRange, financials);
                bool isCurrentYear = financials.FinancialRange.StartDate.Year == DateTime.Now.Year;
                if (quarterList.Any(p => p.ProjectId == project.Id.Value))
                {
                    if (financials.FinancialRange.Range.Substring(1) == "1" || financials.FinancialRange.Range.Substring(1) == "2" || financials.FinancialRange.Range.Substring(1) == "3")
                    {
                        quarterList.First(p => p.ProjectId == project.Id.Value && p.QuarterRangeType.Range == "Q1").FinancialsList.Add(financials);
                    }
                    else if (financials.FinancialRange.Range.Substring(1) == "4" || financials.FinancialRange.Range.Substring(1) == "5" || financials.FinancialRange.Range.Substring(1) == "6")
                    {
                        quarterList.First(p => p.ProjectId == project.Id.Value && p.QuarterRangeType.Range == "Q2").FinancialsList.Add(financials);
                    }
                    else if (financials.FinancialRange.Range.Substring(1) == "7" || financials.FinancialRange.Range.Substring(1) == "8" || financials.FinancialRange.Range.Substring(1) == "9")
                    {
                        quarterList.First(p => p.ProjectId == project.Id.Value && p.QuarterRangeType.Range == "Q3").FinancialsList.Add(financials);
                    }
                    else if (financials.FinancialRange.Range.Substring(1) == "10" || financials.FinancialRange.Range.Substring(1) == "11" || financials.FinancialRange.Range.Substring(1) == "12")
                    {
                        quarterList.First(p => p.ProjectId == project.Id.Value && p.QuarterRangeType.Range == "Q4").FinancialsList.Add(financials);
                    }
                    var item = quarterList.First(p => p.ProjectId == project.Id.Value && p.QuarterRangeType.Range == "YTD");
                    if (item.QuarterRangeType.EndDate >= financials.FinancialRange.EndDate)
                    {
                        quarterList.First(p => p.ProjectId == project.Id.Value && p.QuarterRangeType.Range == "YTD").FinancialsList.Add(financials);
                    }
                }
                else
                {
                    var rangeQ1forProject = new QuarterRange()
                    {
                        ProjectId = project.Id.Value,
                        QuarterRangeType = new RangeType()
                        {
                            Range = "Q1",
                            StartDate = new DateTime(financials.FinancialRange.StartDate.Year, 01, 01),
                            EndDate = new DateTime(financials.FinancialRange.StartDate.Year, 03, 31)
                        },
                        FinancialsList = new List<ComputedFinancials>()
                    };

                    var rangeQ2forProject = new QuarterRange()
                    {
                        ProjectId = project.Id.Value,
                        QuarterRangeType = new RangeType()
                        {
                            Range = "Q2",
                            StartDate = new DateTime(financials.FinancialRange.StartDate.Year, 04, 01),
                            EndDate = new DateTime(financials.FinancialRange.StartDate.Year, 06, 30)
                        },
                        FinancialsList = new List<ComputedFinancials>()
                    };

                    var rangeQ3forProject = new QuarterRange()
                    {
                        ProjectId = project.Id.Value,
                        QuarterRangeType = new RangeType()
                        {
                            Range = "Q3",
                            StartDate = new DateTime(financials.FinancialRange.StartDate.Year, 07, 01),
                            EndDate = new DateTime(financials.FinancialRange.StartDate.Year, 09, 30)
                        },
                        FinancialsList = new List<ComputedFinancials>()
                    };

                    var rangeQ4forProject = new QuarterRange()
                    {
                        ProjectId = project.Id.Value,
                        QuarterRangeType = new RangeType()
                        {
                            Range = "Q4",
                            StartDate = new DateTime(financials.FinancialRange.StartDate.Year, 10, 01),
                            EndDate = new DateTime(financials.FinancialRange.StartDate.Year, 12, 31)
                        },
                        FinancialsList = new List<ComputedFinancials>()
                    };
                    var ytdforProject = new QuarterRange()
                    {
                        ProjectId = project.Id.Value,
                        QuarterRangeType = new RangeType()
                        {
                            Range = "YTD",
                            StartDate = new DateTime(financials.FinancialRange.StartDate.Year, 01, 01),
                            EndDate = new DateTime(financials.FinancialRange.StartDate.Year, isCurrentYear ? DateTime.Now.Month : 12, isCurrentYear ? DateTime.DaysInMonth(financials.FinancialRange.StartDate.Year, DateTime.Now.Month) : 31)
                        },
                        FinancialsList = new List<ComputedFinancials>()
                    };

                    if (financials.FinancialRange.Range.Substring(1) == "1" || financials.FinancialRange.Range.Substring(1) == "2" || financials.FinancialRange.Range.Substring(1) == "3")
                    {
                        rangeQ1forProject.FinancialsList.Add(financials);
                    }
                    else if (financials.FinancialRange.Range.Substring(1) == "4" || financials.FinancialRange.Range.Substring(1) == "5" || financials.FinancialRange.Range.Substring(1) == "6")
                    {
                        rangeQ2forProject.FinancialsList.Add(financials);
                    }
                    else if (financials.FinancialRange.Range.Substring(1) == "7" || financials.FinancialRange.Range.Substring(1) == "8" || financials.FinancialRange.Range.Substring(1) == "9")
                    {
                        rangeQ3forProject.FinancialsList.Add(financials);
                    }
                    else if (financials.FinancialRange.Range.Substring(1) == "10" || financials.FinancialRange.Range.Substring(1) == "11" || financials.FinancialRange.Range.Substring(1) == "12")
                    {
                        rangeQ4forProject.FinancialsList.Add(financials);
                    }
                    if (ytdforProject.QuarterRangeType.EndDate >= financials.FinancialRange.EndDate)
                    {
                        ytdforProject.FinancialsList.Add(financials);
                    }
                    quarterList.Add(rangeQ1forProject);
                    quarterList.Add(rangeQ2forProject);
                    quarterList.Add(rangeQ3forProject);
                    quarterList.Add(rangeQ4forProject);
                    quarterList.Add(ytdforProject);
                }
                //q1,q2,q3,q4
            }
            foreach (var project in projects)
            {
                var quarter1 = new QuarterRange();
                var quarter2 = new QuarterRange();
                var quarter3 = new QuarterRange();
                var quarter4 = new QuarterRange();
                var ytd = new QuarterRange();
                quarter1 = quarterList.FirstOrDefault(p => p.ProjectId == project.Id.Value && p.QuarterRangeType.Range == "Q1");
                quarter2 = quarterList.FirstOrDefault(p => p.ProjectId == project.Id.Value && p.QuarterRangeType.Range == "Q2");
                quarter3 = quarterList.FirstOrDefault(p => p.ProjectId == project.Id.Value && p.QuarterRangeType.Range == "Q3");
                quarter4 = quarterList.FirstOrDefault(p => p.ProjectId == project.Id.Value && p.QuarterRangeType.Range == "Q4");
                ytd = quarterList.FirstOrDefault(p => p.ProjectId == project.Id.Value && p.QuarterRangeType.Range == "YTD");
                if (quarter1 != null)
                    project.ProjectedFinancialsByRange.Add(quarter1.QuarterRangeType, quarter1.getSummedComputedFinancials());
                if (quarter2 != null)
                    project.ProjectedFinancialsByRange.Add(quarter2.QuarterRangeType, quarter2.getSummedComputedFinancials());
                if (quarter3 != null)
                    project.ProjectedFinancialsByRange.Add(quarter3.QuarterRangeType, quarter3.getSummedComputedFinancials());
                if (quarter4 != null)
                    project.ProjectedFinancialsByRange.Add(quarter4.QuarterRangeType, quarter4.getSummedComputedFinancials());
                if (ytd != null)
                    project.ProjectedFinancialsByRange.Add(ytd.QuarterRangeType, ytd.getSummedComputedFinancials());
            }
        }

        private static ComputedFinancials ReadComputedFinancials(
          DbDataReader reader,
          int financialDateIndex,
            int monthEndIndex,
           int rangeTypeIndex,
          int revenueIndex,
          int grossMarginIndex,
          int actualRevenueIndex = -1,
          int actualGrossMarginIndex = -1,
             int previousRevenueIndex = -1,
            int previousGrossMarginIndex = -1)
        {
            return new ComputedFinancials
            {
                FinancialDate = reader.IsDBNull(financialDateIndex) ? (DateTime?)null : reader.GetDateTime(financialDateIndex),
                Revenue = reader.GetDecimal(revenueIndex),
                GrossMargin = reader.GetDecimal(grossMarginIndex),
                ActualRevenue = actualRevenueIndex > -1 && !reader.IsDBNull(actualRevenueIndex) ? reader.GetDecimal(actualRevenueIndex) : 0M,
                ActualGrossMargin = actualGrossMarginIndex > -1 && !reader.IsDBNull(actualGrossMarginIndex) ? reader.GetDecimal(actualGrossMarginIndex) : 0M,
                FinancialRange = new RangeType()
                {
                    StartDate = reader.GetDateTime(financialDateIndex),
                    EndDate = reader.GetDateTime(monthEndIndex),
                    Range = reader.GetString(rangeTypeIndex)
                },
                PreviousMonthsActualRevenueValue = previousRevenueIndex > -1 && !reader.IsDBNull(previousRevenueIndex) ? reader.GetDecimal(previousRevenueIndex) : 0M,
                PreviousMonthsActualMarginValue = previousGrossMarginIndex > -1 && !reader.IsDBNull(previousGrossMarginIndex) ? reader.GetDecimal(previousGrossMarginIndex) : 0M
            };
        }

        private static void CalculateTotalFinancials(List<Project> result)
        {
            foreach (var project in result)
            {
                var filteredFinancials = project.ProjectedFinancialsByRange.Where(mf => (mf.Key.Range != "Q1" && mf.Key.Range != "Q2" && mf.Key.Range != "Q3" && mf.Key.Range != "Q4" && mf.Key.Range != "YTD")).Select(mf => mf.Value);
                var financials = new ComputedFinancials
                {
                    FinancialDate = project.StartDate,
                    Revenue = filteredFinancials.Sum(mf => mf.Revenue),
                    GrossMargin = filteredFinancials.Sum(mf => mf.GrossMargin),
                    ActualRevenue = filteredFinancials.Sum(mf => mf.ActualRevenue),
                    ActualGrossMargin = filteredFinancials.Sum(mf => mf.ActualGrossMargin),
                };
                project.ComputedFinancials = financials;
            }
        }

        public static List<Project> AttainmentProjectList(
          string clientIdsList,
          bool showProjected,
          bool showCompleted,
          bool showActive,
          bool showInternal,
          bool showExperimental,
            bool showProposed,
          bool showInactive,
          bool showAtRisk,
          DateTime periodStart,
          DateTime periodEnd,
          string salespersonIdsList,
          string practiceManagerIdsList,
          string practiceIdsList,
          string projectGroupIdsList,
          bool excludeInternalPractices,
          string userLogin)
        {
            var projectList = new List<Project>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Reports.AttainmentProjectList, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdsParam, clientIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowProjectedParam, showProjected);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowCompletedParam, showCompleted);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowActiveParam, showActive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowInternalParam, showInternal);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowExperimentalParam, showExperimental);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowProposedParam, showProposed);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowInactiveParam, showInactive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowAtRiskParam, showAtRisk);
                    command.Parameters.AddWithValue(Constants.ParameterNames.SalespersonIdsParam, salespersonIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectOwnerIdsParam, practiceManagerIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectGroupIdsParam, projectGroupIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, periodStart);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, periodEnd);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ExcludeInternalPractices, excludeInternalPractices);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    ReadProjectsWithMilestones(reader, projectList);
                }
            }

            return projectList;
        }

        private static void ReadProjectsWithMilestones(SqlDataReader reader, List<Project> resultList)
        {
            try
            {
                if (!reader.HasRows) return;
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
                int discountIndex = reader.GetOrdinal(Constants.ColumnNames.DiscountColumn);
                int termsIndex = reader.GetOrdinal(Constants.ColumnNames.TermsColumn);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.NameColumn);
                int practiceIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                int practiceNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeNameColumn);
                int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDateColumn);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDateColumn);
                int projectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusIdColumn);
                int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
                int buyerNameIndex = reader.GetOrdinal(Constants.ColumnNames.BuyerNameColumn);
                int opportunityId = reader.GetOrdinal(Constants.ColumnNames.OpportunityIdColumn);
                int projectIsChargeableIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIsChargeable);
                int clientIsChargeableIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIsChargeable);
                int pmIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagersIdFirstNameLastName);

                int projectOwnerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectOwnerId);
                int projectOwnerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectOwnerLastName);
                int projectOwnerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectOwnerFirstName);
                int pONumberIndex = -1;

                int mileStoneIdIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneId);
                int mileStoneNameIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneName);

                int sowBudgetIndex = reader.GetOrdinal(Constants.ColumnNames.SowBudgetColumn);
                int salesPersonNameIndex = -1;
                int practiceOwnerNameIndex = -1;
                int projectGroupIdIndex = -1;
                int projectGroupNameIndex = -1;

                int businessTypeIdIndex = -1;
                int pricingListIdIndex = -1;
                int pricingListNameIndex = -1;
                int businessGroupIdIndex = -1;
                int businessGroupNameIndex = -1;
                int hasAttachments = -1;
                int seniorManagerNameIndex = -1;
                int seniorManagerIdIndex = -1;
                int isHouseAccountIndex = -1;

                try
                {
                    isHouseAccountIndex = reader.GetOrdinal(Constants.ColumnNames.IsHouseAccount);
                }
                catch
                { }
                try
                {
                    pONumberIndex = reader.GetOrdinal(Constants.ColumnNames.PONumber);
                }
                catch
                { }

                try
                {
                    seniorManagerIdIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorManagerId);
                }
                catch
                { }

                try
                {
                    seniorManagerNameIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorManagerName);
                }
                catch
                { }
                try
                {
                    businessTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessTypeId);
                }
                catch
                {
                    businessTypeIdIndex = -1;
                }
                try
                {
                    pricingListIdIndex = reader.GetOrdinal(Constants.ColumnNames.PricingListId);
                    pricingListNameIndex = reader.GetOrdinal(Constants.ColumnNames.PricingListNameColumn);
                }
                catch
                {
                    pricingListIdIndex = -1;
                    pricingListNameIndex = -1;
                }
                try
                {
                    businessGroupIdIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessGroupIdColumn);
                    businessGroupNameIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessGroupName);
                }
                catch
                {
                    businessGroupIdIndex = -1;
                    businessGroupNameIndex = -1;
                }

                try
                {
                    projectGroupIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn);
                    projectGroupNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn);
                }
                catch
                {
                    projectGroupIdIndex = -1;
                    projectGroupNameIndex = -1;
                }

                try
                {
                    practiceOwnerNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeOwnerName);
                }
                catch
                {
                    practiceOwnerNameIndex = -1;
                }
                try
                {
                    salesPersonNameIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonFullNameColumn);
                }
                catch
                {
                    salesPersonNameIndex = -1;
                }

                try
                {
                    hasAttachments = reader.GetOrdinal(Constants.ColumnNames.HasAttachmentsColumn);
                }
                catch
                {
                    hasAttachments = -1;
                }

                while (reader.Read())
                {
                    var projectId = reader.GetInt32(projectIdIndex);
                    Project project;

                    if (resultList.Any(p => p.Id.Value == projectId))
                    {
                        project = resultList.First(p => p.Id.Value == projectId);
                    }
                    else
                    {
                        project = new Project
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            Discount = reader.GetDecimal(discountIndex),
                            Terms = reader.GetInt32(termsIndex),
                            Name = reader.GetString(nameIndex),
                            StartDate =
                                !reader.IsDBNull(startDateIndex) ? (DateTime?)reader.GetDateTime(startDateIndex) : null,
                            EndDate =
                                !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null,
                            ProjectNumber = reader.GetString(projectNumberIndex),
                            BuyerName = !reader.IsDBNull(buyerNameIndex) ? reader.GetString(buyerNameIndex) : null,
                            IsChargeable = reader.GetBoolean(projectIsChargeableIndex),
                            Practice = new Practice
                            {
                                Id = reader.GetInt32(practiceIdIndex),
                                Name = reader.GetString(practiceNameIndex)
                            },

                            ProjectManagers = Utils.stringToProjectManagersList(reader.GetString(pmIndex)),
                            SowBudget = !reader.IsDBNull(sowBudgetIndex) ? (Decimal?)reader.GetDecimal(sowBudgetIndex) : null
                        };

                        if (projectGroupIdIndex >= 0)
                        {
                            try
                            {
                                var group = new ProjectGroup
                                {
                                    Id = (int)reader[projectGroupIdIndex],
                                    Name = (string)reader[projectGroupNameIndex],
                                };

                                project.Group = @group;
                            }
                            catch
                            {
                            }
                        }

                        if (pricingListIdIndex >= 0)
                        {
                            try
                            {
                                var pricingList = new PricingList
                                {
                                    PricingListId = (int)reader[pricingListIdIndex],
                                    Name = (string)reader[pricingListNameIndex],
                                };

                                project.PricingList = pricingList;
                            }
                            catch
                            {
                            }
                        }

                        if (businessGroupIdIndex >= 0)
                        {
                            try
                            {
                                var businessGroup = new BusinessGroup
                                {
                                    Id = (int)reader[businessGroupIdIndex],
                                    Name = (string)reader[businessGroupNameIndex],
                                };

                                project.BusinessGroup = businessGroup;
                            }
                            catch
                            {
                            }
                        }

                        if (businessTypeIdIndex >= 0)
                        {
                            try
                            {
                                BusinessType businessType = (BusinessType)(int)reader[businessTypeIdIndex];
                                project.BusinessType = businessType;
                            }
                            catch
                            {
                            }
                        }
                        if (seniorManagerIdIndex >= 0)
                        {
                            try
                            {
                                project.SeniorManagerId = reader.GetInt32(seniorManagerIdIndex);
                            }
                            catch
                            {
                            }
                        }
                        if (seniorManagerNameIndex >= 0)
                        {
                            try
                            {
                                project.SeniorManagerName = reader.GetString(seniorManagerNameIndex);
                            }
                            catch
                            {
                            }
                        }
                        if (pONumberIndex > -1)
                        {
                            try
                            {
                                project.PONumber = !reader.IsDBNull(pONumberIndex) ? reader.GetString(pONumberIndex) : string.Empty;
                            }
                            catch
                            {
                            }
                        }

                        if (practiceOwnerNameIndex >= 0)
                        {
                            try
                            {
                                project.Practice.PracticeOwnerName = reader.GetString(practiceOwnerNameIndex);
                            }
                            catch
                            {
                                project.Practice.PracticeOwnerName = string.Empty;
                            }
                        }

                        if (salesPersonNameIndex >= 0)
                        {
                            try
                            {
                                project.SalesPersonName = reader.GetString(salesPersonNameIndex);
                            }
                            catch
                            {
                                project.SalesPersonName = string.Empty;
                            }
                        }

                        if (hasAttachments >= 0)
                        {
                            project.HasAttachments = (int)reader[hasAttachments] == 1;
                        }

                        project.Client = new Client
                        {
                            Id = reader.GetInt32(clientIdIndex),
                            Name = reader.GetString(clientNameIndex),
                            IsChargeable = reader.GetBoolean(clientIsChargeableIndex)
                        };
                        if (isHouseAccountIndex > -1)
                        {
                            try
                            {
                                project.Client.IsHouseAccount = !reader.IsDBNull(isHouseAccountIndex) && reader.GetBoolean(isHouseAccountIndex);
                            }
                            catch
                            {
                            }
                        }

                        project.Status = new ProjectStatus
                        {
                            Id = reader.GetInt32(projectStatusIdIndex),
                            Name = reader.GetString(projectStatusNameIndex)
                        };

                        project.OpportunityId =
                            !reader.IsDBNull(opportunityId) ? (int?)reader.GetInt32(opportunityId) : null;

                        try
                        {
                            int directorIdIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorIdColumn),
                                directorLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorLastNameColumn),
                                directorFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorFirstNameColumn);
                            if (!reader.IsDBNull(directorIdIndex))
                            {
                                project.Director = new Person()
                                {
                                    Id = (int?)reader.GetInt32(directorIdIndex),
                                    FirstName = reader.GetString(directorFirstNameIndex),
                                    LastName = reader.GetString(directorLastNameIndex)
                                };
                            }
                        }
                        catch
                        {
                        }
                        resultList.Add(project);
                    }

                    if (!reader.IsDBNull(projectOwnerIdIndex))
                    {
                        project.ProjectOwner = new Person()
                        {
                            Id = reader.GetInt32(projectOwnerIdIndex),
                            FirstName = reader.GetString(projectOwnerFirstNameIndex),
                            LastName = reader.GetString(projectOwnerLastNameIndex)
                        };
                    }

                    if (reader.IsDBNull(mileStoneIdIndex)) continue;
                    var milestone = new Milestone
                    {
                        Description = reader.GetString(mileStoneNameIndex),
                        Id = reader.GetInt32(mileStoneIdIndex)
                    };
                    if (project.Milestones == null)
                    {
                        project.Milestones = new List<Milestone>();
                    }
                    project.Milestones.Add(milestone);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<AttainmentBillableutlizationReport> AttainmentBillableutlizationReport(DateTime startDate, DateTime endDate)
        {
            List<AttainmentBillableutlizationReport> result = new List<AttainmentBillableutlizationReport>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.AttainmentBillableutlizationReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadAttainmentBillableutlizationReport(reader, result);
                }
            }
            return result;
        }

        public static void ReadAttainmentBillableutlizationReport(SqlDataReader reader, List<AttainmentBillableutlizationReport> result)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
            int timescaleNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleName);
            int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
            int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
            int billableUtilizationPercentIndex = reader.GetOrdinal(Constants.ColumnNames.BillableUtilizationPercent);
            int rangeTypeIndex = reader.GetOrdinal(Constants.ColumnNames.RangeType);
            int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
            int availableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.AvailableHours);
            while (reader.Read())
            {
                AttainmentBillableutlizationReport attainmentBillableutlizationReport;
                int personId = reader.GetInt32(personIdIndex);
                if (result.Any(p => p.Person.Id == personId))
                {
                    attainmentBillableutlizationReport = result.First(p => p.Person.Id == personId);
                }
                else
                {
                    attainmentBillableutlizationReport = new AttainmentBillableutlizationReport
                    {
                        Person = new Person()
                        {
                            Id = personId,
                            LastName = reader.GetString(lastNameIndex),
                            FirstName = reader.GetString(firstNameIndex),
                            Title = new Title()
                            {
                                TitleName =
                                    !reader.IsDBNull(titleIndex)
                                        ? reader.GetString(titleIndex)
                                        : string.Empty
                            },
                            CurrentPay = new Pay()
                            {
                                TimescaleName =
                                    !reader.IsDBNull(timescaleNameIndex)
                                        ? reader.GetString(timescaleNameIndex)
                                        : string.Empty
                            }
                        },
                        BillableUtilizationList = new List<BillableUtlizationByRange>()
                    };

                    result.Add(attainmentBillableutlizationReport);
                }
                BillableUtlizationByRange billableUtlizationByRange = new BillableUtlizationByRange
                {
                    StartDate = reader.GetDateTime(startDateIndex),
                    EndDate = reader.GetDateTime(endDateIndex),
                    BillableHours = reader.GetDouble(billableHoursIndex),
                    AvailableHours = reader.GetInt32(availableHoursIndex),
                    BillableUtilization =
                        !reader.IsDBNull(billableUtilizationPercentIndex)
                            ? reader.GetDouble(billableUtilizationPercentIndex)
                            : -1,
                    RangeType = reader.GetString(rangeTypeIndex)
                };
                attainmentBillableutlizationReport.BillableUtilizationList.Add(billableUtlizationByRange);
            }
        }

        public static List<Project> ProjectAttributionReport(DateTime startDate, DateTime endDate)
        {
            List<Project> result = new List<Project>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ProjectAttributionReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadProjectAttributionReport(reader, result);
                }
            }
            return result;
        }

        public static void ReadProjectAttributionReport(SqlDataReader reader, List<Project> result)
        {
            if (!reader.HasRows) return;
            int recordTypeIndex = reader.GetOrdinal(Constants.ColumnNames.RecordType);
            int attributionTypeIndex = reader.GetOrdinal(Constants.ColumnNames.AttributionType);
            int projectStatusIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatus);
            int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
            int accountIndex = reader.GetOrdinal(Constants.ColumnNames.Account);
            int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
            int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
            int BusinessGroupIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessGroup);
            int businessUnitIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessUnit);
            int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
            int newOrExtensionIndex = reader.GetOrdinal(Constants.ColumnNames.NewOrExtension);
            int nameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
            int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
            int commissionPercentageIndex = reader.GetOrdinal(Constants.ColumnNames.CommissionPercentage);

            while (reader.Read())
            {
                Project attributionReport;
                string projectNumber = reader.GetString(projectNumberIndex);
                if (result.Any(p => p.ProjectNumber == projectNumber))
                {
                    attributionReport = result.First(p => p.ProjectNumber == projectNumber);
                }
                else
                {
                    attributionReport = new Project
                    {
                        Status = new ProjectStatus { Name = reader.GetString(projectStatusIndex) },
                        ProjectNumber = projectNumber,
                        Client = new Client { Name = reader.GetString(accountIndex) },
                        BusinessGroup = new BusinessGroup { Name = reader.GetString(BusinessGroupIndex) },
                        Group = new ProjectGroup { Name = reader.GetString(businessUnitIndex) },
                        Name = reader.GetString(projectNameIndex),
                        BusinessType = reader.IsDBNull(newOrExtensionIndex) ? (BusinessType)0 : (BusinessType)reader.GetInt32(newOrExtensionIndex),
                        AttributionList = new List<Attribution>()
                    };
                    result.Add(attributionReport);
                }
                Attribution attributionRecord = new Attribution
                {
                    StartDate =
                         reader.GetDateTime(startDateIndex),
                    EndDate = reader.GetDateTime(endDateIndex),
                    AttributionType =
                        (AttributionTypes)Enum.Parse(typeof(AttributionTypes), reader.GetString(attributionTypeIndex)),
                    AttributionRecordType =
                        (AttributionRecordTypes)
                        Enum.Parse(typeof(AttributionRecordTypes), reader.GetString(recordTypeIndex)),
                    TargetName = reader.GetString(nameIndex),
                    Title = new Title { TitleName = reader.GetString(titleIndex) },
                    CommissionPercentage = (decimal)(reader.GetDecimal(commissionPercentageIndex)) / 100
                };
                attributionReport.AttributionList.Add(attributionRecord);
            }
        }

        public static List<ResourceExceptionReport> ZeroHourlyRateExceptionReport(DateTime startDate, DateTime endDate, SqlConnection connection = null)
        {
            List<ResourceExceptionReport> result = new List<ResourceExceptionReport>();
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
                connection.Open();
            }
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ZeroHourlyRateExceptionReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadZeroHourlyRateExceptionReport(reader, result);
                }
            }
            return result;
        }

        public static void ReadZeroHourlyRateExceptionReport(SqlDataReader reader, List<ResourceExceptionReport> result)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int employeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeNumber);
            int isOffshoreIndex = reader.GetOrdinal(Constants.ColumnNames.IsOffshore);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int timescaleNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleName);
            int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
            int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
            int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
            int projectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusId);
            int milestoneIdIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneId);
            int milestoneNameIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneName);
            int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
            int projectedDeliveryDateIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectedDeliveryDate);
            int amountIndex = reader.GetOrdinal(Constants.ColumnNames.Amount);

            while (reader.Read())
            {
                ResourceExceptionReport resourceExceptionReport;
                int personId = reader.GetInt32(personIdIndex);
                int projectId = reader.GetInt32(projectIdIndex);
                if (result.Any(p => p.Person.Id == personId && p.Project.Id == projectId))
                {
                    resourceExceptionReport = result.First(p => p.Person.Id == personId && p.Project.Id == projectId);
                }
                else
                {
                    resourceExceptionReport = new ResourceExceptionReport();
                    Person person = new Person()
                    {
                        Id = personId,
                        FirstName = reader.GetString(firstNameIndex),
                        LastName = reader.GetString(lastNameIndex),
                        EmployeeNumber = reader.GetString(employeeNumberIndex),
                        IsOffshore = reader.GetBoolean(isOffshoreIndex),
                        CurrentPay = new Pay()
                        {
                            TimescaleName = reader.GetString(timescaleNameIndex)
                        }
                    };
                    Project project = new Project()
                    {
                        Id = reader.GetInt32(projectIdIndex),
                        ProjectNumber = reader.GetString(projectNumberIndex),
                        Name = reader.GetString(projectNameIndex),
                        Status = new ProjectStatus()
                        {
                            Id = reader.GetInt32(projectStatusIdIndex)
                        }
                    };
                    resourceExceptionReport.Project = project;
                    resourceExceptionReport.Person = person;
                    result.Add(resourceExceptionReport);
                    resourceExceptionReport.Project.Milestones = new List<Milestone>();
                }
                Milestone milestone = new Milestone()
                {
                    Id = reader.GetInt32(milestoneIdIndex),
                    Description = reader.GetString(milestoneNameIndex),
                    StartDate = reader.GetDateTime(startDateIndex),
                    ProjectedDeliveryDate = reader.GetDateTime(projectedDeliveryDateIndex),
                    Amount = new PracticeManagementCurrency()
                    {
                        Value = reader.GetDecimal(amountIndex)
                    }
                };
                resourceExceptionReport.Project.Milestones.Add(milestone);
            }
        }

        public static List<ResourceExceptionReport> ResourceAssignedOrUnassignedChargingExceptionReport(DateTime startDate, DateTime endDate, bool isUnassignedReport, SqlConnection connection = null)
        {
            List<ResourceExceptionReport> result = new List<ResourceExceptionReport>();
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
                connection.Open();
            }
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ResourceAssignedOrUnassignedChargingExceptionReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsUnassignedReport, isUnassignedReport);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadResourceAssignedOrUnassignedChargingExceptionReport(reader, result);
                }
            }
            return result;
        }

        public static void ReadResourceAssignedOrUnassignedChargingExceptionReport(SqlDataReader reader, List<ResourceExceptionReport> result)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int employeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeNumber);
            int isOffshoreIndex = reader.GetOrdinal(Constants.ColumnNames.IsOffshore);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int timescaleNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleName);
            int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
            int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
            int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
            int projectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusId);
            int forecastedHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ForecastedHours);
            int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
            int NonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);

            while (reader.Read())
            {
                ResourceExceptionReport resourceExceptionReport = new ResourceExceptionReport()
                {
                    Person = new Person()
                    {
                        Id = reader.GetInt32(personIdIndex),
                        FirstName = reader.GetString(firstNameIndex),
                        LastName = reader.GetString(lastNameIndex),
                        EmployeeNumber = reader.GetString(employeeNumberIndex),
                        IsOffshore = reader.GetBoolean(isOffshoreIndex),
                        CurrentPay = new Pay()
                        {
                            TimescaleName = reader.IsDBNull(timescaleNameIndex) ? "" : reader.GetString(timescaleNameIndex)
                        }
                    },
                    Project = new Project()
                    {
                        Id = reader.GetInt32(projectIdIndex),
                        ProjectNumber = reader.GetString(projectNumberIndex),
                        Name = reader.GetString(projectNameIndex),
                        Status = new ProjectStatus()
                        {
                            Id = reader.GetInt32(projectStatusIdIndex)
                        }
                    },
                    ProjectedHours = !reader.IsDBNull(forecastedHoursIndex) ? Convert.ToDouble(reader.GetDecimal(forecastedHoursIndex)) : 0d,
                    BillableHours = reader.GetDouble(billableHoursIndex),
                    NonBillableHours = reader.GetDouble(NonBillableHoursIndex)
                };
                result.Add(resourceExceptionReport);
            }
        }

        public static List<Person> RecruitingMetricsReport(DateTime startDate, DateTime endDate, SqlConnection connection = null)
        {
            List<Person> result = new List<Person>();
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
                connection.Open();
            }
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.RecruitingMetricsReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadRecruitingMetricsReport(reader, result);
                }
            }
            return result;
        }

        public static void ReadRecruitingMetricsReport(SqlDataReader reader, List<Person> result)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int employeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeNumber);

            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int PersonStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusId);
            int TitleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
            int TitleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);

            int HireDateColumnIndex = reader.GetOrdinal(Constants.ColumnNames.HireDateColumn);
            int TerminationDateColumnIndex = reader.GetOrdinal(Constants.ColumnNames.TerminationDateColumn);
            int TerminationReasonIdColumnIndex = reader.GetOrdinal(Constants.ColumnNames.TerminationReasonIdColumn);
            int TerminationReasonColumnIndex = reader.GetOrdinal(Constants.ColumnNames.TerminationReasonColumn);
            int TimeScaleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeScaleId);

            int TimescaleNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleName);
            int RecruiterIdColumnIndex = reader.GetOrdinal(Constants.ColumnNames.RecruiterIdColumn);
            int RecruiterFirstNameColumnIndex = reader.GetOrdinal(Constants.ColumnNames.RecruiterFirstNameColumn);
            int RecruiterLastNameColumnIndex = reader.GetOrdinal(Constants.ColumnNames.RecruiterLastNameColumn);
            int JobSeekerStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.JobSeekerStatusId);

            int TargetedCompanyIdIndex = reader.GetOrdinal(Constants.ColumnNames.TargetedCompanyId);
            int TargetedCompanyRecruitingMetricsNameIndex = reader.GetOrdinal(Constants.ColumnNames.TargetedCompanyRecruitingMetricsName);
            int SourceIdIndex = reader.GetOrdinal(Constants.ColumnNames.SourceId);
            int SourceRecruitingMetricsNameIndex = reader.GetOrdinal(Constants.ColumnNames.SourceRecruitingMetricsName);
            int EmployeeReferralIdIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeReferralId);
            int EmployeeReferralFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeReferralFirstName);
            int EmployeeReferralLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeReferralLastName);
            int LengthOfTenureInDaysIndex = reader.GetOrdinal(Constants.ColumnNames.LengthOfTenureInDays);

            while (reader.Read())
            {
                Person person = new Person()
                {
                    Id = reader.GetInt32(personIdIndex),
                    FirstName = reader.GetString(firstNameIndex),
                    LastName = reader.GetString(lastNameIndex),
                    EmployeeNumber = reader.GetString(employeeNumberIndex),
                    CurrentPay = new Pay()
                    {
                        Timescale = (TimescaleType)reader.GetInt32(TimeScaleIdIndex)
                    },
                    Status = new PersonStatus()
                    {
                        Id = reader.GetInt32(PersonStatusIdIndex)
                    },
                    Title = new Title()
                    {
                        TitleId = reader.GetInt32(TitleIdIndex),
                        TitleName = reader.GetString(TitleIndex)
                    },
                    HireDate = reader.GetDateTime(HireDateColumnIndex),
                    TerminationDate = !reader.IsDBNull(TerminationDateColumnIndex) ? (DateTime?)reader.GetDateTime(TerminationDateColumnIndex) : null,
                    TerminationReasonid = !reader.IsDBNull(TerminationReasonIdColumnIndex) ? (int?)reader.GetInt32(TerminationReasonIdColumnIndex) : null,
                    TerminationReason = !reader.IsDBNull(TerminationReasonColumnIndex) ? reader.GetString(TerminationReasonColumnIndex) : null,
                    RecruiterId = reader.GetInt32(RecruiterIdColumnIndex),
                    RecruiterFirstName = reader.GetString(RecruiterFirstNameColumnIndex),
                    RecruiterLastName = reader.GetString(RecruiterLastNameColumnIndex),
                    JobSeekersStatusId = !reader.IsDBNull(JobSeekerStatusIdIndex) ? (int?)reader.GetInt32(JobSeekerStatusIdIndex) : null,

                    TargetedCompanyRecruitingMetrics = new RecruitingMetrics()
                    {
                        RecruitingMetricsId = !reader.IsDBNull(TargetedCompanyIdIndex) ? (int?)reader.GetInt32(TargetedCompanyIdIndex) : null,
                        Name = !reader.IsDBNull(TargetedCompanyRecruitingMetricsNameIndex) ? reader.GetString(TargetedCompanyRecruitingMetricsNameIndex) : ""
                    },

                    SourceRecruitingMetrics = new RecruitingMetrics()
                    {
                        RecruitingMetricsId = !reader.IsDBNull(SourceIdIndex) ? (int?)reader.GetInt32(SourceIdIndex) : null,
                        Name = !reader.IsDBNull(SourceRecruitingMetricsNameIndex) ? reader.GetString(SourceRecruitingMetricsNameIndex) : ""
                    },
                    EmployeeReferralId = !reader.IsDBNull(EmployeeReferralIdIndex) ? (int?)reader.GetInt32(EmployeeReferralIdIndex) : null,
                    EmployeeReferralFirstName = !reader.IsDBNull(EmployeeReferralIdIndex) ? reader.GetString(EmployeeReferralFirstNameIndex) : "",
                    EmployeeReferralLastName = !reader.IsDBNull(EmployeeReferralIdIndex) ? reader.GetString(EmployeeReferralLastNameIndex) : "",
                    LengthOfTenture = reader.GetInt32(LengthOfTenureInDaysIndex)
                };
                result.Add(person);
            }
        }

        public static List<ProjectFeedback> ProjectFeedbackReport(string accountIds, string businessGroupIds, DateTime startDate, DateTime endDate, string projectStatus, string projectIds, string directorIds, string practiceIds, bool excludeInternalPractices, string personIds, string titleIds, string reviewStartdateMonths, string reviewEnddateMonths, string projectmanagerIds, string statusIds, bool isExport, string payTypeIds)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Project.ProjectFeedbackReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.AccountIdsParam, accountIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.BusinessGroupIds, businessGroupIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIds, projectIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectStatus, projectStatus ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ClientDirectorIds, directorIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.Practices, practiceIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExcludeInternalPractices, excludeInternalPractices);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIds, personIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.TitleIdsParam, titleIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ReviewStartDateMonths, reviewStartdateMonths ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ReviewEndDateMonths, reviewEnddateMonths ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectManagers, projectmanagerIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.Statuses, statusIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsExport, isExport);
                command.Parameters.AddWithValue(Constants.ParameterNames.PayTypeIds, payTypeIds);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    List<ProjectFeedback> result = new List<ProjectFeedback>();
                    ReadProjectFeedbackReport(reader, result);
                    return result;
                }
            }
        }

        public static void ReadProjectFeedbackReport(SqlDataReader reader, List<ProjectFeedback> result)
        {
            try
            {
                if (!reader.HasRows) return;
                int feedbackIdIndex = reader.GetOrdinal(Constants.ColumnNames.FeedbackId);
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int employeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeNumber);
                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
                int cliendIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientId);
                int cliendNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientName);
                int projectstatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusId);
                int projectstatusIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatus);
                int groupIdIndex = reader.GetOrdinal(Constants.ColumnNames.GroupId);
                int businessUnitIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessUnit);
                int businessGroupIdIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessGroupIdColumn);
                int businessGroupIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessGroup);
                int directorIdIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorIdColumn);
                int directorFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorFirstNameColumn);
                int directorLastNameColumnIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorLastNameColumn);
                int seniorManagerNameIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorManagerName);
                int projectManagerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerId);
                int projectManagerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerFirstName);
                int projectManagerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerLastName);
                int projectOwnerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectOwnerId);
                int projectOwnerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectOwnerLastName);
                int projectOwnerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectOwnerFirstName);
                int reviewStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.ReviewStartDate);
                int reviewEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.ReviewEndDate);
                int feedbackStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.FeedbackStatusId);
                int feedbackStatusIndex = reader.GetOrdinal(Constants.ColumnNames.FeedbackStatus);
                int completionCertificateByIndex = reader.GetOrdinal(Constants.ColumnNames.CompletionCertificateBy);
                int completionCertificateDateIndex = reader.GetOrdinal(Constants.ColumnNames.CompletionCertificateDate);
                int cancelationReasonIndex = reader.GetOrdinal(Constants.ColumnNames.CancelationReason);

                while (reader.Read())
                {
                    var feedbackId = reader.GetInt32(feedbackIdIndex);
                    var projectManager = new Person()
                    {
                        Id = !reader.IsDBNull(projectManagerIdIndex) ? (int?)reader.GetInt32(projectManagerIdIndex) : -1,
                        FirstName = !reader.IsDBNull(projectManagerFirstNameIndex) ? reader.GetString(projectManagerFirstNameIndex) : "",
                        LastName = !reader.IsDBNull(projectManagerLastNameIndex) ? reader.GetString(projectManagerLastNameIndex) : ""
                    };
                    if (result.Any(f => f.Id.Value == feedbackId))
                    {
                        var feedback = result.First(f => f.Id.Value == feedbackId);
                        feedback.Project.ProjectManagers.Add(projectManager);
                    }
                    else
                    {
                        ProjectFeedback feedback = new ProjectFeedback()
                        {
                            Id = feedbackId,
                            Person = new Person()
                            {
                                Id = reader.GetInt32(personIdIndex),
                                FirstName = reader.GetString(firstNameIndex),
                                LastName = reader.GetString(lastNameIndex),
                                EmployeeNumber = reader.GetString(employeeNumberIndex),
                                Title = new Title()
                                {
                                    TitleId = !reader.IsDBNull(titleIdIndex) ? reader.GetInt32(titleIdIndex) : -1,
                                    TitleName = !reader.IsDBNull(titleIndex) ? reader.GetString(titleIndex) : string.Empty
                                }
                            },
                            Project = new Project()
                            {
                                Id = reader.GetInt32(projectIdIndex),
                                Name = reader.GetString(projectNameIndex),
                                ProjectNumber = reader.GetString(projectNumberIndex),
                                Status = new ProjectStatus()
                                {
                                    Id = reader.GetInt32(projectstatusIdIndex),
                                    Name = reader.GetString(projectstatusIndex)
                                },
                                Client = new Client()
                                {
                                    Id = reader.GetInt32(cliendIdIndex),
                                    Name = reader.GetString(cliendNameIndex)
                                },
                                Group = new ProjectGroup()
                                {
                                    Id = reader.GetInt32(groupIdIndex),
                                    Name = reader.GetString(businessUnitIndex)
                                },
                                BusinessGroup = new BusinessGroup()
                                {
                                    Id = reader.GetInt32(businessGroupIdIndex),
                                    Name = reader.GetString(businessGroupIndex)
                                },
                                Director = new Person()
                                {
                                    Id = !reader.IsDBNull(directorIdIndex) ? (int?)reader.GetInt32(directorIdIndex) : null,
                                    FirstName = !reader.IsDBNull(directorFirstNameIndex) ? reader.GetString(directorFirstNameIndex) : string.Empty,
                                    LastName = !reader.IsDBNull(directorLastNameColumnIndex) ? reader.GetString(directorLastNameColumnIndex) : string.Empty
                                },
                                SeniorManagerName = !reader.IsDBNull(seniorManagerNameIndex) ? reader.GetString(seniorManagerNameIndex) : string.Empty,
                                ProjectOwner = new Person()
                                {
                                    Id = !reader.IsDBNull(projectOwnerIdIndex) ? (int?)reader.GetInt32(projectOwnerIdIndex) : null,
                                    FirstName = !reader.IsDBNull(projectOwnerFirstNameIndex) ? reader.GetString(projectOwnerFirstNameIndex) : string.Empty,
                                    LastName = !reader.IsDBNull(projectOwnerLastNameIndex) ? reader.GetString(projectOwnerLastNameIndex) : string.Empty
                                }
                            },
                            ReviewStartDate = reader.GetDateTime(reviewStartDateIndex),
                            ReviewEndDate = reader.GetDateTime(reviewEndDateIndex),
                            Status = new ProjectFeedbackStatus()
                            {
                                Id = reader.GetInt32(feedbackStatusIdIndex),
                                Name = reader.GetString(feedbackStatusIndex)
                            },
                            CompletionCertificateBy = !reader.IsDBNull(completionCertificateByIndex) ? reader.GetString(completionCertificateByIndex) : string.Empty,
                            CompletionCertificateDate = !reader.IsDBNull(completionCertificateDateIndex) ? reader.GetDateTime(completionCertificateDateIndex) : DateTime.MinValue,
                            CancelationReason = !reader.IsDBNull(cancelationReasonIndex) ? reader.GetString(cancelationReasonIndex) : string.Empty
                        };
                        if (projectManager.Id != null)
                            feedback.Project.ProjectManagers = new List<Person>()
                            {
                                projectManager
                            };
                        result.Add(feedback);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<BillingReport> BillingReportByCurrency(DateTime startDate, DateTime endDate, string practiceIds, string accountIds, string businessUnitIds, string directorIds, string salesPersonIds, string projectManagerIds, string seniorManagerIds)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.BillingReportByCurrency, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.AccountIdsParam, accountIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.BusinessUnitIdsParam, businessUnitIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.DirectorIds, directorIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.SalespersonIdsParam, salesPersonIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectManagerIds, projectManagerIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.SeniorManagerIds, seniorManagerIds ?? (Object)DBNull.Value);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    List<BillingReport> result = new List<BillingReport>();
                    ReadBillingReport(reader, result);
                    return result;
                }
            }
        }

        public static List<BillingReport> BillingReportByHours(DateTime startDate, DateTime endDate, string practiceIds, string accountIds, string businessUnitIds, string directorIds, string salesPersonIds, string projectManagerIds, string seniorManagerIds)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.BillingReportByHours, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.AccountIdsParam, accountIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.BusinessUnitIdsParam, businessUnitIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.DirectorIds, directorIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.SalespersonIdsParam, salesPersonIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectManagerIds, projectManagerIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.SeniorManagerIds, seniorManagerIds ?? (Object)DBNull.Value);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    List<BillingReport> result = new List<BillingReport>();
                    ReadBillingReport(reader, result);
                    return result;
                }
            }
        }

        public static void ReadBillingReport(SqlDataReader reader, List<BillingReport> result)
        {
            try
            {
                if (!reader.HasRows) return;
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientId);
                int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientName);
                int projectnumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
                int projectnameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                int practiceIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                int practicenameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeNameColumn);
                int salespersonIdIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonIdColumn);
                int salesPersonNameIndex = reader.GetOrdinal(Constants.ColumnNames.SalesPersonName);
                int projectManagerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerId);
                int projectManagerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerFirstName);
                int projectManagerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerLastName);
                int directorIdIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorIdColumn);
                int directorFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorFirstNameColumn);
                int directorLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorLastNameColumn);

                int poNumberIndex = reader.GetOrdinal(Constants.ColumnNames.PONumber);
                int seniorManagerIdIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorManagerId);
                int seniorManagerNameIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorManagerName);

                int forecastedhoursIndex = -1;
                int actualHoursIndex = -1;
                int forecastedhoursInRangeIndex = -1;
                int actualHoursInRangeIndex = -1;
                int sowBudgetIndex = -1;
                int actualRevenueIndex = -1;
                int actualRevenueinRangeIndex = -1;
                int revenueIndex = -1;
                try
                {
                    forecastedhoursIndex = reader.GetOrdinal(Constants.ColumnNames.ForecastedHours);
                }
                catch
                { }
                try
                {
                    actualHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ActualHours);
                }
                catch
                { }
                try
                {
                    sowBudgetIndex = reader.GetOrdinal(Constants.ColumnNames.SowBudgetColumn);
                }
                catch
                { }

                try
                {
                    actualRevenueIndex = reader.GetOrdinal(Constants.ColumnNames.ActualRevenue);
                }
                catch
                { }
                try
                {
                    actualRevenueinRangeIndex = reader.GetOrdinal(Constants.ColumnNames.ActualRevenueInRange);
                }
                catch
                { }
                try
                {
                    revenueIndex = reader.GetOrdinal(Constants.ColumnNames.RevenueColumn);
                }
                catch
                { }
                try
                {
                    forecastedhoursInRangeIndex = reader.GetOrdinal(Constants.ColumnNames.ForecastedHoursInRange);
                }
                catch
                { }
                try
                {
                    actualHoursInRangeIndex = reader.GetOrdinal(Constants.ColumnNames.ActualHoursInRange);
                }
                catch
                { }
                while (reader.Read())
                {
                    var projectManger = new Person()
                    {
                        Id = reader.IsDBNull(projectManagerIdIndex) ? null : (int?)reader.GetInt32(projectManagerIdIndex),
                        FirstName = reader.IsDBNull(projectManagerFirstNameIndex) ? "" : reader.GetString(projectManagerFirstNameIndex),
                        LastName = reader.IsDBNull(projectManagerLastNameIndex) ? "" : reader.GetString(projectManagerLastNameIndex)
                    };
                    var projectId = reader.GetInt32(projectIdIndex);
                    if (result.Any(p => p.Project.Id == projectId))
                    {
                        var billingItem = result.FirstOrDefault(p => p.Project.Id == projectId);
                        billingItem.Project.ProjectManagers.Add(projectManger);
                    }
                    else
                    {
                        var billingItem = new BillingReport();
                        var project = new Project()
                        {
                            Id = projectId,
                            Client = new Client()
                            {
                                Id = reader.GetInt32(clientIdIndex),
                                Name = reader.GetString(clientNameIndex)
                            },
                            ProjectNumber = reader.GetString(projectnumberIndex),
                            Name = reader.GetString(projectnameIndex),
                            Practice = new Practice()
                            {
                                Id = reader.GetInt32(practiceIdIndex),
                                Name = reader.GetString(practicenameIndex)
                            },
                            SalesPersonId = reader.IsDBNull(salespersonIdIndex) ? -1 : reader.GetInt32(salespersonIdIndex),
                            SalesPersonName = reader.IsDBNull(salesPersonNameIndex) ? "" : reader.GetString(salesPersonNameIndex),
                            ProjectManagers = new List<Person>(),
                            Director = new Person()
                            {
                                Id = reader.IsDBNull(directorIdIndex) ? null : (int?)reader.GetInt32(directorIdIndex),
                                FirstName = reader.IsDBNull(directorFirstNameIndex) ? "" : reader.GetString(directorFirstNameIndex),
                                LastName = reader.IsDBNull(directorLastNameIndex) ? "" : reader.GetString(directorLastNameIndex)
                            },
                            PONumber = reader.IsDBNull(poNumberIndex) ? "" : reader.GetString(poNumberIndex),
                            SeniorManagerId = reader.IsDBNull(seniorManagerIdIndex) ? -1 : reader.GetInt32(seniorManagerIdIndex),
                            SeniorManagerName = reader.IsDBNull(seniorManagerNameIndex) ? "" : reader.GetString(seniorManagerNameIndex),
                        };
                        if (projectManger.Id != null)
                            project.ProjectManagers.Add(projectManger);
                        billingItem.Project = project;
                        if (actualRevenueinRangeIndex >= 0)
                        {
                            try
                            {
                                billingItem.RangeActual = reader.GetDecimal(actualRevenueinRangeIndex);
                            }
                            catch
                            {
                            }
                        }
                        if (revenueIndex >= 0)
                        {
                            try
                            {
                                billingItem.RangeProjected = reader.GetDecimal(revenueIndex);
                            }
                            catch
                            {
                            }
                        }
                        if (forecastedhoursInRangeIndex >= 0)
                        {
                            try
                            {
                                billingItem.ForecastedHoursInRange = Convert.ToDouble(reader.GetDecimal(forecastedhoursInRangeIndex));
                            }
                            catch
                            {
                            }
                        }
                        if (actualHoursInRangeIndex >= 0)
                        {
                            try
                            {
                                billingItem.ActualHoursInRange = Convert.ToDouble(reader.GetDecimal(actualHoursInRangeIndex));
                            }
                            catch
                            {
                            }
                        }
                        if (forecastedhoursIndex >= 0)
                        {
                            try
                            {
                                billingItem.ForecastedHours = Convert.ToDouble(reader.GetDecimal(forecastedhoursIndex));
                            }
                            catch
                            {
                            }
                        }
                        if (actualHoursIndex >= 0)
                        {
                            try
                            {
                                billingItem.ActualHours = reader.GetDouble(actualHoursIndex);
                            }
                            catch
                            {
                            }
                        }
                        if (sowBudgetIndex >= 0)
                        {
                            try
                            {
                                billingItem.SOWBudget = reader.IsDBNull(sowBudgetIndex) ? 0 : reader.GetDecimal(sowBudgetIndex);
                            }
                            catch
                            {
                            }
                        }
                        if (actualRevenueIndex >= 0)
                        {
                            try
                            {
                                billingItem.ActualToDate = reader.GetDecimal(actualRevenueIndex);
                            }
                            catch
                            {
                            }
                        }
                        result.Add(billingItem);
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ProjectLevelGroupedHours> NonBillableReport(DateTime startDate, DateTime endDate, string projectNumber, string directorIds, string businessUnitIds, string practiceIds)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.NonBillableReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectNumber, projectNumber ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.DirectorIds, directorIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.BusinessUnitIdsParam, businessUnitIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIds ?? (Object)DBNull.Value);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<ProjectLevelGroupedHours>();
                    ReadNonBillableReport(reader, result);
                    return result;
                }
            }
        }

        public static void ReadNonBillableReport(SqlDataReader reader, List<ProjectLevelGroupedHours> result)
        {
            try
            {
                if (!reader.HasRows) return;
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                int projectnameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                int projectnumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
                int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientId);
                int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientName);
                int groupidIndex = reader.GetOrdinal(Constants.ColumnNames.GroupId);
                int groupnameIndex = reader.GetOrdinal(Constants.ColumnNames.GroupName);
                int personidIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int lastnameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int firstnameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int practiceIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                int practicenameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeNameColumn);
                int salesPersonNameIndex = reader.GetOrdinal(Constants.ColumnNames.SalesPerson);
                int directornameIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorName);
                int projectManagersIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagers);
                int seniorManagerNameIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorManagerName);
                int latestDateIndex = reader.GetOrdinal(Constants.ColumnNames.LatestDate);
                int billRateIndex = reader.GetOrdinal(Constants.ColumnNames.BillRate);
                int billablehoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
                int nonbillablehoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
                int flhrIndex = reader.GetOrdinal(Constants.ColumnNames.FLHR);
                int discountIndex = reader.GetOrdinal(Constants.ColumnNames.Discount);

                while (reader.Read())
                {
                    var discount = Convert.ToDouble(reader[discountIndex]);
                    var some = reader.IsDBNull(billRateIndex) ? 0 : Convert.ToDouble(reader[billRateIndex]) - Convert.ToDouble(reader[billRateIndex]) * discount / 100;
                    var pesronlevelhours = new PersonLevelGroupedHours()
                    {
                        BillRate = reader.IsDBNull(billRateIndex) ? 0 : Convert.ToDouble(reader[billRateIndex]) - Convert.ToDouble(reader[billRateIndex]) * discount / 100,
                        Person = new Person()
                        {
                            Id = reader.GetInt32(personidIndex),
                            LastName = reader.GetString(lastnameIndex),
                            FirstName = reader.GetString(firstnameIndex)
                        },
                        BillableHours = reader.IsDBNull(billablehoursIndex) ? 0 : reader.GetDouble(billablehoursIndex),
                        ProjectNonBillableHours = reader.IsDBNull(nonbillablehoursIndex) ? 0 : reader.GetDouble(nonbillablehoursIndex),
                        LatestDate = reader.GetDateTime(latestDateIndex),
                        FLHR = reader.IsDBNull(flhrIndex) ? 0 : Convert.ToDouble(reader[flhrIndex])
                    };
                    var projectId = reader.GetInt32(projectIdIndex);
                    if (result.Any(p => p.Project.Id == projectId))
                    {
                        var first = result.FirstOrDefault(p => p.Project.Id == projectId);
                        first.PersonLevelDetails.Add(pesronlevelhours);
                    }
                    else
                    {
                        var billingItem = new ProjectLevelGroupedHours()
                        {
                            Project = new Project()
                            {
                                Id = projectId,
                                Client = new Client()
                                {
                                    Id = reader.GetInt32(clientIdIndex),
                                    Name = reader.GetString(clientNameIndex)
                                },
                                ProjectNumber = reader.GetString(projectnumberIndex),
                                Name = reader.GetString(projectnameIndex),
                                Group = new ProjectGroup()
                                {
                                    Id = reader.GetInt32(groupidIndex),
                                    Name = reader.GetString(groupnameIndex)
                                },
                                Practice = new Practice()
                                {
                                    Id = reader.GetInt32(practiceIdIndex),
                                    Name = reader.GetString(practicenameIndex)
                                },
                                SalesPersonName = reader.IsDBNull(salesPersonNameIndex) ? "" : reader.GetString(salesPersonNameIndex),
                                DirectorName = reader.IsDBNull(directornameIndex) ? "" : reader.GetString(directornameIndex),
                                ProjectManagerNames = reader.IsDBNull(projectManagersIndex) ? "" : reader.GetString(projectManagersIndex),
                                SeniorManagerName = reader.IsDBNull(seniorManagerNameIndex) ? "" : reader.GetString(seniorManagerNameIndex)
                            },
                            PersonLevelDetails = new List<PersonLevelGroupedHours>()
                            {
                                pesronlevelhours
                            }
                        };
                        result.Add(billingItem);
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<BadgedResourcesByTime> BadgedResourcesByTimeReport(string payTypes, string personStatusIds, DateTime startDate, DateTime endDate, int step)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.BadgedResourcesByTimeReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.PayTypeIds, payTypes);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonStatusIdsParam, personStatusIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.Step, step);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<BadgedResourcesByTime>();
                    ReadBadgedResourceByTimeReport(reader, result);
                    return result;
                }
            }
        }

        public static void ReadBadgedResourceByTimeReport(SqlDataReader reader, List<BadgedResourcesByTime> result)
        {
            try
            {
                if (!reader.HasRows) return;
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
                int badgedOnProjectCountIndex = reader.GetOrdinal(Constants.ColumnNames.BadgedOnProjectCount);
                int badgedOnProjectExceptionCountIndex = reader.GetOrdinal(Constants.ColumnNames.BadgedOnProjectExceptionCount);
                int badgedNotOnProjectCountIndex = reader.GetOrdinal(Constants.ColumnNames.BadgedNotOnProjectCount);
                int badgedNotOnProjectExceptionCountIndex = reader.GetOrdinal(Constants.ColumnNames.BadgedNotOnProjectExceptionCount);
                int clockNotStartedCountIndex = reader.GetOrdinal(Constants.ColumnNames.ClockNotStartedCount);
                int blockedCountIndex = reader.GetOrdinal(Constants.ColumnNames.BlockedCount);
                int inBreakPeriodCountIndex = reader.GetOrdinal(Constants.ColumnNames.InBreakPeriodCount);

                while (reader.Read())
                {
                    var badgeResource = new BadgedResourcesByTime()
                    {
                        StartDate = reader.GetDateTime(startDateIndex),
                        EndDate = reader.GetDateTime(endDateIndex),
                        BadgedOnProjectCount = reader.GetInt32(badgedOnProjectCountIndex),
                        BadgedNotOnProjectCount = reader.GetInt32(badgedNotOnProjectCountIndex),
                        ClockNotStartedCount = reader.GetInt32(clockNotStartedCountIndex),
                        BlockedCount = reader.GetInt32(blockedCountIndex),
                        InBreakPeriodCount = reader.GetInt32(inBreakPeriodCountIndex),
                        BadgedNotOnProjectExceptionCount = reader.GetInt32(badgedNotOnProjectExceptionCountIndex),
                        BadgedOnProjectExceptionCount = reader.GetInt32(badgedOnProjectExceptionCountIndex)
                    };
                    result.Add(badgeResource);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<MSBadge> ListBadgeResourcesByType(string paytypes, string personStatuses, DateTime startDate, DateTime endDate, bool isNotBadged, bool isClockNotStart, bool isBlocked, bool isBreak, bool badgedOnProject, bool isBadgedException, bool isNotBadgedException)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ListBadgeResourcesByType, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.PayTypeIds, paytypes);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonStatusIdsParam, personStatuses);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsNotBadged, isNotBadged);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsClockNotStart, isClockNotStart);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsBlocked, isBlocked);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsBreak, isBreak);
                command.Parameters.AddWithValue(Constants.ParameterNames.BadgedOnProject, badgedOnProject);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsBadgedException, isBadgedException);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsNotBadgedException, isNotBadgedException);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<MSBadge>();
                    ReadBadgedResourceByType(reader, result);
                    return result;
                }
            }
        }

        public static void ReadBadgedResourceByType(SqlDataReader reader, List<MSBadge> result)
        {
            try
            {
                if (!reader.HasRows) return;
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);

                int titleIndex = -1;
                int badgeStartDateIndex = -1;
                int badgeEnddateIndex = -1;
                int blockStartdateIndex = -1;
                int blockEnddateIndex = -1;
                int breakStartdateIndex = -1;
                int breakEnddateIndex = -1;
                int clockStartDateIndex = -1;
                int clockEnddateIndex = -1;
                int clientIdIndex = -1;
                int clientNameIndex = -1;
                int projectIdIndex = -1;
                int projectNameIndex = -1;
                int projectNumberIndex = -1;
                int startDateIndex = -1;
                int endDateIndex = -1;
                int isApprovedByOpsIndex = -1;
                int deactivatedDateIndex = -1;
                int organicBreakStartIndex = -1;
                int organicBreakEndIndex = -1;
                try
                {
                    organicBreakStartIndex = reader.GetOrdinal(Constants.ColumnNames.OrganicBreakStartDate);
                }
                catch
                { }

                try
                {
                    organicBreakEndIndex = reader.GetOrdinal(Constants.ColumnNames.OrganicBreakEndDate);
                }
                catch
                { }

                try
                {
                    titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                }
                catch
                { }
                try
                {
                    badgeStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeStartDate);
                }
                catch
                { }
                try
                {
                    badgeEnddateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeEndDate);
                }
                catch
                { }
                try
                {
                    blockStartdateIndex = reader.GetOrdinal(Constants.ColumnNames.BlockStartDate);
                }
                catch
                { }
                try
                {
                    blockEnddateIndex = reader.GetOrdinal(Constants.ColumnNames.BlockEndDate);
                }
                catch
                { }
                try
                {
                    breakStartdateIndex = reader.GetOrdinal(Constants.ColumnNames.BreakStartDate);
                }
                catch
                { }
                try
                {
                    breakEnddateIndex = reader.GetOrdinal(Constants.ColumnNames.BreakEndDate);
                }
                catch
                { }

                try
                {
                    clockStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectBadgeStartDate);
                }
                catch
                { }
                try
                {
                    clockEnddateIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectBadgeEndDate);
                }
                catch
                { }
                try
                {
                    clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientId);
                }
                catch
                { }
                try
                {
                    clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientName);
                }
                catch
                { }
                try
                {
                    projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                }
                catch
                { }
                try
                {
                    projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                }
                catch
                { }
                try
                {
                    projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
                }
                catch
                { }
                try
                {
                    startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
                }
                catch
                { }
                try
                {
                    endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
                }
                catch
                { }
                try
                {
                    isApprovedByOpsIndex = reader.GetOrdinal(Constants.ColumnNames.IsApproved);
                }
                catch
                { }
                try
                {
                    deactivatedDateIndex = reader.GetOrdinal(Constants.ColumnNames.DeactivatedDate);
                }
                catch
                { }

                while (reader.Read())
                {
                    var badgeResource = new MSBadge()
                    {
                        Person = new Person()
                        {
                            Id = reader.GetInt32(personIdIndex),
                            FirstName = reader.GetString(firstNameIndex),
                            LastName = reader.GetString(lastNameIndex)
                        }
                    };
                    if (titleIndex != -1)
                        badgeResource.Person.Title = new Title()
                        {
                            TitleName = reader.GetString(titleIndex)
                        };
                    if (badgeStartDateIndex != -1)
                        badgeResource.BadgeStartDate = reader.IsDBNull(badgeStartDateIndex) ? null : (DateTime?)reader.GetDateTime(badgeStartDateIndex);
                    if (badgeEnddateIndex != -1)
                        badgeResource.BadgeEndDate = reader.IsDBNull(badgeEnddateIndex) ? null : (DateTime?)reader.GetDateTime(badgeEnddateIndex);
                    if (blockStartdateIndex != -1)
                        badgeResource.BlockStartDate = reader.IsDBNull(blockStartdateIndex) ? null : (DateTime?)reader.GetDateTime(blockStartdateIndex);
                    if (blockEnddateIndex != -1)
                        badgeResource.BlockEndDate = reader.IsDBNull(blockEnddateIndex) ? null : (DateTime?)reader.GetDateTime(blockEnddateIndex);
                    if (breakStartdateIndex != -1)
                        badgeResource.BreakStartDate = reader.IsDBNull(breakStartdateIndex) ? null : (DateTime?)reader.GetDateTime(breakStartdateIndex);
                    if (breakEnddateIndex != -1)
                        badgeResource.BreakEndDate = reader.IsDBNull(breakEnddateIndex) ? null : (DateTime?)reader.GetDateTime(breakEnddateIndex);
                    if (organicBreakStartIndex != -1)
                        badgeResource.OrganicBreakStartDate = reader.IsDBNull(organicBreakStartIndex) ? null : (DateTime?)reader.GetDateTime(organicBreakStartIndex);
                    if (organicBreakEndIndex != -1)
                        badgeResource.OrganicBreakEndDate = reader.IsDBNull(organicBreakEndIndex) ? null : (DateTime?)reader.GetDateTime(organicBreakEndIndex);
                    if (clockStartDateIndex != -1)
                        badgeResource.ProjectBadgeStartDate = reader.IsDBNull(clockStartDateIndex) ? null : (DateTime?)reader.GetDateTime(clockStartDateIndex);
                    if (clockEnddateIndex != -1)
                        badgeResource.ProjectBadgeEndDate = reader.IsDBNull(clockEnddateIndex) ? null : (DateTime?)reader.GetDateTime(clockEnddateIndex);
                    if (clientIdIndex != -1)
                    {
                        badgeResource.Project = new Project()
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            Name = reader.GetString(projectNameIndex),
                            ProjectNumber = reader.GetString(projectNumberIndex),
                            Client = new Client()
                            {
                                Id = reader.GetInt32(clientIdIndex),
                                Name = reader.GetString(clientNameIndex)
                            },
                            StartDate = reader.GetDateTime(startDateIndex),
                            EndDate = reader.GetDateTime(endDateIndex)
                        };
                    }
                    if (isApprovedByOpsIndex != -1)
                        badgeResource.IsApproved = reader.GetInt32(isApprovedByOpsIndex) == 1;
                    if (deactivatedDateIndex != -1)
                        badgeResource.DeactivatedDate = reader.IsDBNull(deactivatedDateIndex) ? null : (DateTime?)reader.GetDateTime(deactivatedDateIndex);
                    result.Add(badgeResource);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<GroupByPractice> ResourcesByPracticeReport(string paytypes, string PersonStatuses, string practices, DateTime startDate, DateTime endDate, int step)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ResourcesByPracticeReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.PayTypeIds, paytypes);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonStatusIdsParam, PersonStatuses);
                command.Parameters.AddWithValue(Constants.ParameterNames.Practices, practices);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.Step, step);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<GroupByPractice>();
                    ReadResourcesByPracticeReport(reader, result);
                    return result;
                }
            }
        }

        public static void ReadResourcesByPracticeReport(SqlDataReader reader, List<GroupByPractice> result)
        {
            try
            {
                if (!reader.HasRows) return;
                int practiceIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                int practiceNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeNameColumn);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
                int badgedOnProjectCountIndex = reader.GetOrdinal(Constants.ColumnNames.BadgedOnProjectCount);
                int badgedNotOnProjectCountIndex = reader.GetOrdinal(Constants.ColumnNames.BadgedNotOnProjectCount);
                int clockNotStartedCountIndex = reader.GetOrdinal(Constants.ColumnNames.ClockNotStartedCount);

                while (reader.Read())
                {
                    var practiceId = reader.GetInt32(practiceIdIndex);
                    var badgeResource = new BadgedResourcesByTime()
                    {
                        StartDate = reader.GetDateTime(startDateIndex),
                        EndDate = reader.GetDateTime(endDateIndex),
                        BadgedOnProjectCount = reader.GetInt32(badgedOnProjectCountIndex),
                        BadgedNotOnProjectCount = reader.GetInt32(badgedNotOnProjectCountIndex),
                        ClockNotStartedCount = reader.GetInt32(clockNotStartedCountIndex),
                        Practice = new Practice()
                        {
                            Id = practiceId,
                            Name = reader.GetString(practiceNameIndex)
                        }
                    };

                    if (result.Any(p => p.Practice.Id == practiceId))
                    {
                        var first = result.FirstOrDefault(p => p.Practice.Id == practiceId);
                        first.ResourcesCount.Add(badgeResource);
                    }
                    else
                    {
                        var groupedPractice = new GroupByPractice()
                        {
                            Practice = new Practice()
                            {
                                Id = practiceId,
                                Name = reader.GetString(practiceNameIndex)
                            },
                            ResourcesCount = new List<BadgedResourcesByTime>() { badgeResource }
                        };
                        result.Add(groupedPractice);
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<GroupbyTitle> ResourcesByTitleReport(string paytypes, string personStatuses, string titles, DateTime startDate, DateTime endDate, int step)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ResourcesByTitleReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.PayTypeIds, paytypes);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonStatusIdsParam, personStatuses);
                command.Parameters.AddWithValue(Constants.ParameterNames.TitleIds, titles);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.Step, step);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<GroupbyTitle>();
                    ReadResourcesByTitleReport(reader, result);
                    return result;
                }
            }
        }

        public static void ReadResourcesByTitleReport(SqlDataReader reader, List<GroupbyTitle> result)
        {
            try
            {
                if (!reader.HasRows) return;
                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
                int badgedOnProjectCountIndex = reader.GetOrdinal(Constants.ColumnNames.BadgedOnProjectCount);
                int badgedNotOnProjectCountIndex = reader.GetOrdinal(Constants.ColumnNames.BadgedNotOnProjectCount);
                int clockNotStartedCountIndex = reader.GetOrdinal(Constants.ColumnNames.ClockNotStartedCount);

                while (reader.Read())
                {
                    var titleId = reader.GetInt32(titleIdIndex);
                    var badgeResource = new BadgedResourcesByTime()
                    {
                        StartDate = reader.GetDateTime(startDateIndex),
                        EndDate = reader.GetDateTime(endDateIndex),
                        BadgedOnProjectCount = reader.GetInt32(badgedOnProjectCountIndex),
                        BadgedNotOnProjectCount = reader.GetInt32(badgedNotOnProjectCountIndex),
                        ClockNotStartedCount = reader.GetInt32(clockNotStartedCountIndex)
                    };

                    if (result.Any(p => p.Title.TitleId == titleId))
                    {
                        var first = result.FirstOrDefault(p => p.Title.TitleId == titleId);
                        first.ResourcesCount.Add(badgeResource);
                    }
                    else
                    {
                        var groupedTitle = new GroupbyTitle()
                        {
                            Title = new Title()
                            {
                                TitleId = titleId,
                                TitleName = reader.GetString(titleIndex)
                            },
                            ResourcesCount = new List<BadgedResourcesByTime>() { badgeResource }
                        };
                        result.Add(groupedTitle);
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<MSBadge> GetBadgeRequestNotApprovedList()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetBadgeRequestNotApprovedList, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<MSBadge>();
                    ReadBadgeRequestNotApprovedList(reader, result);
                    return result;
                }
            }
        }

        public static void ReadBadgeRequestNotApprovedList(SqlDataReader reader, List<MSBadge> result)
        {
            try
            {
                if (!reader.HasRows) return;
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
                int badgeStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeStartDate);
                int badgeEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeEndDate);
                int badgeRequestDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeRequestDate);
                int projectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusId);
                int clockEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.ClockEndDate);
                int requesterIdIndex = reader.GetOrdinal(Constants.ColumnNames.RequesterId);
                int requesterIndex = reader.GetOrdinal(Constants.ColumnNames.Requester);
                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                int MilestoneIdIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneId);
                int MilestoneDescriptionIndex = reader.GetOrdinal(Constants.ColumnNames.DescriptionColumn);

                while (reader.Read())
                {
                    var badgeResource = new MSBadge()
                    {
                        Person = new Person()
                        {
                            LastName = reader.GetString(lastNameIndex),
                            FirstName = reader.GetString(firstNameIndex),
                            Title = new Title()
                            {
                                TitleId = reader.GetInt32(titleIdIndex),
                                TitleName = reader.GetString(titleIndex)
                            }
                        },
                        Project = new Project()
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            Name = reader.GetString(projectNameIndex),
                            ProjectNumber = reader.GetString(projectNumberIndex),
                            Status = new ProjectStatus()
                            {
                                Id = reader.GetInt32(projectStatusIdIndex)
                            }
                        },
                        BadgeStartDate = reader.GetDateTime(badgeStartDateIndex),
                        BadgeEndDate = reader.GetDateTime(badgeEndDateIndex),
                        PlannedEndDate = reader.IsDBNull(badgeRequestDateIndex) ? null : (DateTime?)reader.GetDateTime(badgeRequestDateIndex),
                        ProjectBadgeEndDate = reader.IsDBNull(clockEndDateIndex) ? null : (DateTime?)reader.GetDateTime(clockEndDateIndex),
                        RequesterId = reader.IsDBNull(requesterIdIndex) ? null : (int?)reader.GetInt32(requesterIdIndex),
                        Requester = reader.IsDBNull(requesterIndex) ? string.Empty : reader.GetString(requesterIndex),
                        Milestone = new Milestone()
                        {
                            Id = reader.GetInt32(MilestoneIdIndex),
                            Description = reader.GetString(MilestoneDescriptionIndex)
                        }
                    };
                    result.Add(badgeResource);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<MSBadge> GetAllBadgeDetails(string payTypes, string personStatuses)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetAllBadgeDetails, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.PayTypeIds, payTypes);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonStatusIdsParam, personStatuses);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<MSBadge>();
                    ReadAllBadgeDetails(reader, result);
                    return result;
                }
            }
        }

        public static void ReadAllBadgeDetails(SqlDataReader reader, List<MSBadge> result)
        {
            try
            {
                if (!reader.HasRows) return;
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);

                int badgeStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeStartDate);
                int badgeEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeEndDate);
                int breakStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BreakStartDate);
                int breakEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BreakEndDate);
                int timescaleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleColumn);
                int timescaleNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleName);
                int badgeDurationIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeDuration);
                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                int organicBreakStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.OrganicBreakStartDate);
                int organicBreakEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.OrganicBreakEndDate);
                int organicBreakDurationIndex = reader.GetOrdinal(Constants.ColumnNames.OrganicBreakDuration);

                while (reader.Read())
                {
                    var badgeResource = new MSBadge()
                    {
                        Person = new Person()
                        {
                            Id = reader.GetInt32(personIdIndex),
                            LastName = reader.GetString(lastNameIndex),
                            FirstName = reader.GetString(firstNameIndex),
                            CurrentPay = new Pay()
                            {
                                TimescaleName = reader.IsDBNull(timescaleNameIndex) ? string.Empty : reader.GetString(timescaleNameIndex)
                            },
                            Title = new Title()
                            {
                                TitleId = reader.GetInt32(titleIdIndex),
                                TitleName = reader.GetString(titleIndex)
                            }
                        },
                        BadgeStartDate = reader.IsDBNull(badgeStartDateIndex) ? null : (DateTime?)reader.GetDateTime(badgeStartDateIndex),//reader.GetDateTime(badgeStartDateIndex),
                        BadgeEndDate = reader.IsDBNull(badgeEndDateIndex) ? null : (DateTime?)reader.GetDateTime(badgeEndDateIndex),
                        BreakStartDate = reader.IsDBNull(breakStartDateIndex) ? null : (DateTime?)reader.GetDateTime(breakStartDateIndex),
                        BreakEndDate = reader.IsDBNull(breakEndDateIndex) ? null : (DateTime?)reader.GetDateTime(breakEndDateIndex),
                        BadgeDuration = reader.IsDBNull(badgeDurationIndex) ? 0 : reader.GetInt32(badgeDurationIndex),
                        OrganicBreakStartDate = reader.IsDBNull(organicBreakStartDateIndex) ? null : (DateTime?)reader.GetDateTime(organicBreakStartDateIndex),
                        OrganicBreakEndDate = reader.IsDBNull(organicBreakEndDateIndex) ? null : (DateTime?)reader.GetDateTime(organicBreakEndDateIndex),
                        OrganicBreakDuration = reader.IsDBNull(organicBreakDurationIndex) ? -1 : reader.GetInt32(organicBreakDurationIndex)
                    };
                    result.Add(badgeResource);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static PersonTimeEntriesTotals UtilizationReport(int personId, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.UtilizationReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    PersonTimeEntriesTotals result = new PersonTimeEntriesTotals();
                    if (!reader.HasRows) return result;
                    int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
                    int billableHoursUntilTodayIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHoursUntilToday);
                    int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
                    int availableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.AvailableHours);
                    int projectedHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectedHours);
                    while (reader.Read())
                    {
                        result.BillableHours = !reader.IsDBNull(billableHoursIndex) ? (double)reader.GetDouble(billableHoursIndex) : 0d;
                        result.NonBillableHours = !reader.IsDBNull(nonBillableHoursIndex) ? (double)reader.GetDouble(nonBillableHoursIndex) : 0d;
                        result.AvailableHours = !reader.IsDBNull(availableHoursIndex) ? (int)reader.GetInt32(availableHoursIndex) : 0d;
                        result.BillableHoursUntilToday = !reader.IsDBNull(billableHoursUntilTodayIndex) ? (double)reader.GetDouble(billableHoursUntilTodayIndex) : 0d;
                        result.ProjectedHours = !reader.IsDBNull(projectedHoursIndex) ? (double)reader.GetDouble(projectedHoursIndex) : 0d;
                    }
                    return result;
                }
            }
        }

        public static List<ManagementMeetingReport> ManagedServiceReportByPerson(string paytypes, string personStatuses, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ManagedServiceReportByPerson, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.PayTypeIds, paytypes);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonStatusIdsParam, personStatuses);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<ManagementMeetingReport>();
                    ReadManagedServiceReportByPerson(reader, result);
                    reader.NextResult();
                    ReadManagedServiceReportByPersonRange(reader, result);
                    return result;
                }
            }
        }

        public static void ReadManagedServiceReportByPerson(SqlDataReader reader, List<ManagementMeetingReport> result)
        {
            try
            {
                if (!reader.HasRows) return;
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int timescaleIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleName);
                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                int badgeStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeStartDate);
                int badgeEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeEndDate);
                int breakStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BreakStartDate);
                int breakEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BreakEndDate);
                int blockStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BlockStartDate);
                int blockEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BlockEndDate);
                int restartDateIndex = reader.GetOrdinal(Constants.ColumnNames.RestartDate);
                int badgeDurationIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeDuration);
                int manageServiceContractIndex = reader.GetOrdinal(Constants.ColumnNames.ManageServiceContract);

                while (reader.Read())
                {
                    var title = new Title()
                            {
                                TitleId = !reader.IsDBNull(titleIdIndex) ? reader.GetInt32(titleIdIndex) : -1,
                                TitleName = !reader.IsDBNull(titleIndex) ? reader.GetString(titleIndex) : string.Empty
                            };
                    var person = new Person()
                        {
                            Id = reader.GetInt32(personIdIndex),
                            LastName = reader.GetString(lastNameIndex),
                            FirstName = reader.GetString(firstNameIndex),
                            CurrentPay = new Pay()
                            {
                                TimescaleName = reader.GetString(timescaleIndex)
                            },
                            Title = title,
                            Badge = new MSBadge()
                            {
                                BadgeStartDate = !reader.IsDBNull(badgeStartDateIndex) ? (DateTime?)reader.GetDateTime(badgeStartDateIndex) : null,
                                BadgeEndDate = !reader.IsDBNull(badgeEndDateIndex) ? (DateTime?)reader.GetDateTime(badgeEndDateIndex) : null,
                                BlockStartDate = !reader.IsDBNull(blockStartDateIndex) ? (DateTime?)reader.GetDateTime(blockStartDateIndex) : null,
                                BlockEndDate = !reader.IsDBNull(blockEndDateIndex) ? (DateTime?)reader.GetDateTime(blockEndDateIndex) : null,
                                PlannedEndDate = !reader.IsDBNull(restartDateIndex) ? (DateTime?)reader.GetDateTime(restartDateIndex) : null,
                                BadgeDuration = reader.IsDBNull(badgeDurationIndex) ? 0 : reader.GetInt32(badgeDurationIndex),
                                IsMSManagedService = reader.GetBoolean(manageServiceContractIndex),
                                BreakStartDate = !reader.IsDBNull(breakStartDateIndex) ? (DateTime?)reader.GetDateTime(breakStartDateIndex) : null,
                                BreakEndDate = !reader.IsDBNull(breakEndDateIndex) ? (DateTime?)reader.GetDateTime(breakEndDateIndex) : null
                            }
                        };
                    var managementMeetingReport = new ManagementMeetingReport()
                    {
                        Person = person,
                        Title = title
                    };
                    result.Add(managementMeetingReport);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ReadManagedServiceReportByPersonRange(SqlDataReader reader, List<ManagementMeetingReport> result)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
            int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
            int isAvailableIndex = reader.GetOrdinal(Constants.ColumnNames.IsAvailable);
            while (reader.Read())
            {
                var personId = reader.GetInt32(personIdIndex);
                if (result.Any(p => p.Person.Id == personId))
                {
                    var person = result.FirstOrDefault(p => p.Person.Id == personId);
                    if (person.Range == null)
                        person.Range = new List<AvailableRange>();
                    var range = new AvailableRange()
                    {
                        StartDate = reader.GetDateTime(startDateIndex),
                        EndDate = reader.GetDateTime(endDateIndex),
                        IsAvailable = reader.GetInt32(isAvailableIndex) == 1
                    };
                    person.Range.Add(range);
                }
            }
        }

        public static void SaveManagedParametersByPerson(string userLogin, decimal actualRevenuePerHour, decimal targetRevenuePerHour, decimal hoursUtilization, decimal targetRevenuePerAnnum)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.SaveManagedParametersByPerson, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                command.Parameters.AddWithValue(Constants.ParameterNames.ActualRevenuePerHourParam, actualRevenuePerHour);
                command.Parameters.AddWithValue(Constants.ParameterNames.TargetRevenuePerHourParam, targetRevenuePerHour);
                command.Parameters.AddWithValue(Constants.ParameterNames.HoursUtilizationParam, hoursUtilization);
                command.Parameters.AddWithValue(Constants.ParameterNames.TargetRevenuePerAnnumParam, targetRevenuePerAnnum);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static RevenueReport GetManagedParametersByPerson(string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetManagedParametersByPerson, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new RevenueReport();
                    if (!reader.HasRows)
                    {
                        result = null;
                    }
                    else
                    {
                        ReadManagedParametersByPerson(reader, result);
                    }
                    return result;
                }
            }
        }

        public static void ReadManagedParametersByPerson(SqlDataReader reader, RevenueReport result)
        {
            try
            {
                int ActualRevenuePerHourIndex = reader.GetOrdinal(Constants.ColumnNames.ActualRevenuePerHour);
                int TargetRevenuePerHourIndex = reader.GetOrdinal(Constants.ColumnNames.TargetRevenuePerHour);
                int HoursUtilizationIndex = reader.GetOrdinal(Constants.ColumnNames.HoursUtilization);
                int TargetRevenuePerAnnumIndex = reader.GetOrdinal(Constants.ColumnNames.TargetRevenuePerAnnum);

                while (reader.Read())
                {

                    result.ActualRevenuePerHour = reader.GetDecimal(ActualRevenuePerHourIndex);
                    result.TargetRevenuePerHour = reader.GetDecimal(TargetRevenuePerHourIndex);
                    result.HoursUtilization = reader.GetDecimal(HoursUtilizationIndex);
                    result.TotalRevenuePerAnnual = reader.GetDecimal(TargetRevenuePerAnnumIndex);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<GroupbyTitle> GetAveragePercentagesByTitles(string paytypes, string personStatuses, string titles, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.GetAveragePercentagesByTitles, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.PayTypeIds, paytypes);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonStatusIdsParam, personStatuses);
                command.Parameters.AddWithValue(Constants.ParameterNames.TitleIds, titles);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<GroupbyTitle>();
                    GetAveragePercentageByTiles(reader, result);
                    return result;
                }
            }
        }

        public static void GetAveragePercentageByTiles(SqlDataReader reader, List<GroupbyTitle> result)
        {
            try
            {
                if (!reader.HasRows) return;
                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
                int totalCountIndex = reader.GetOrdinal(Constants.ColumnNames.TotalCount);


                while (reader.Read())
                {
                    var titleId = reader.GetInt32(titleIdIndex);
                    var badgeResource = new BadgedResourcesByTime()
                    {
                        StartDate = reader.GetDateTime(startDateIndex),
                        EndDate = reader.GetDateTime(endDateIndex),
                        ResourceCount = reader.GetInt32(totalCountIndex)
                    };

                    if (result.Any(p => p.Title.TitleId == titleId))
                    {
                        var first = result.FirstOrDefault(p => p.Title.TitleId == titleId);
                        first.ResourcesCount.Add(badgeResource);
                    }
                    else
                    {
                        var groupedTitle = new GroupbyTitle()
                        {
                            Title = new Title()
                            {
                                TitleId = titleId,
                                TitleName = reader.GetString(titleIndex)
                            },
                            ResourcesCount = new List<BadgedResourcesByTime>() { badgeResource }
                        };
                        result.Add(groupedTitle);
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<Project> ProjectsListWithFilters(string clientIdsList, bool showProjected, bool showCompleted, bool showActive, bool showInternal, bool showExperimental, bool showProposed, bool showInactive, bool showAtRisk, DateTime periodStart, DateTime periodEnd, string salespersonIdsList, string ProjectOwnerIdsList, string practiceIdsList, string projectGroupIdsList, string divisionIdsList,
            string channelIdsList,
            string revenueTypeIdsList,
            string offeringIdsList, string userLogin)
        {
            var projectList = new List<Project>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ProjectsListWithFilters, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdsParam, clientIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowProjectedParam, showProjected);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowCompletedParam, showCompleted);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowActiveParam, showActive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowInternalParam, showInternal);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowExperimentalParam, showExperimental);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowProposedParam, showProposed);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowInactiveParam, showInactive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowAtRiskParam, showAtRisk);
                    command.Parameters.AddWithValue(Constants.ParameterNames.SalespersonIdsParam, salespersonIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectOwnerIdsParam, ProjectOwnerIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectGroupIdsParam, projectGroupIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, periodStart);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, periodEnd);
                    command.Parameters.AddWithValue(Constants.ParameterNames.DivisionIdsParam, divisionIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ChannelIdsParam, channelIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.RevenueTypeIdsParam, revenueTypeIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.OfferingIdsParam, offeringIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    ReadProjects(reader, projectList);
                }
            }
            return projectList;
        }

        private static void ReadProjects(SqlDataReader reader, List<Project> resultList)
        {
            try
            {
                if (!reader.HasRows) return;
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDateColumn);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDateColumn);
                int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
                int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
                int practiceIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                int practiceNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeNameColumn);
                int projectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusIdColumn);
                int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
                int projectManagerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerId);
                int projectManagerNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerNameColumn);
                int executiveInChargeIdIndex = reader.GetOrdinal(Constants.ColumnNames.ExecutiveInChargeId);
                int executiveInChargeNameIndex = reader.GetOrdinal(Constants.ColumnNames.ExecutiveInChargeNameColumn);
                int businessGroupIdIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessGroupIdColumn);
                int businessGroupNameIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessGroupName);
                int businessUnitIDIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessUnitId);
                int businessUnitNameIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessUnitName);
                int projectCapabilitiesIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectCapabilities);
                int projectCapabilityIdsIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectCapabilityIds);
                int divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);
                int divisionNameIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionName);
                int ChannelIdIndex = reader.GetOrdinal(Constants.ColumnNames.ChannelId);
                int ChannelNameIndex = reader.GetOrdinal(Constants.ColumnNames.ChannelName);
                int subchannelIndex = reader.GetOrdinal(Constants.ColumnNames.SubChannel);
                int revenueTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.RevenueTypeId);
                int revenueTypeNameIndex = reader.GetOrdinal(Constants.ColumnNames.RevenueName);
                int offeringIdIndex = reader.GetOrdinal(Constants.ColumnNames.OfferingId);
                int OfferingNameIndex = reader.GetOrdinal(Constants.ColumnNames.OfferName);
                int isClientTimeEntryRequiredIndex = reader.GetOrdinal(Constants.ColumnNames.IsClientTimeEntryRequired);
                int previousProjectNumberIndex = -1;
                int previousProjectIdIndex = -1;
                int outsourceIdIndex = -1;
                try
                {
                    outsourceIdIndex = reader.GetOrdinal(Constants.ColumnNames.OutsourceId);
                }
                catch { }
                try
                {
                    previousProjectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.PreviousProjectNumber);
                    previousProjectIdIndex = reader.GetOrdinal(Constants.ColumnNames.PreviousProjectId);
                }
                catch { }
                while (reader.Read())
                {
                    var project = new Project
                      {
                          Id = reader.GetInt32(projectIdIndex),
                          Name = reader.GetString(nameIndex),
                          StartDate = !reader.IsDBNull(startDateIndex) ? (DateTime?)reader.GetDateTime(startDateIndex) : null,
                          EndDate = !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null,
                          ProjectNumber = reader.GetString(projectNumberIndex),
                          Practice = new Practice() { Id = reader.GetInt32(practiceIdIndex), Name = reader.GetString(practiceNameIndex) },
                          Capabilities = !reader.IsDBNull(projectCapabilitiesIndex) ? reader.GetString(projectCapabilitiesIndex).Replace(";", ", ").TrimEnd(new char[] { ',', ' ' }) : string.Empty,
                          Group = new ProjectGroup()
                          {
                              Id = !reader.IsDBNull(businessUnitIDIndex) ? reader.GetInt32(businessUnitIDIndex) : -1,
                              Name = !reader.IsDBNull(businessUnitNameIndex) ? reader.GetString(businessUnitNameIndex) : string.Empty
                          },
                          BusinessGroup = new BusinessGroup()
                          {
                              Id = !reader.IsDBNull(businessGroupIdIndex) ? reader.GetInt32(businessGroupIdIndex) : -1,
                              Name = !reader.IsDBNull(businessGroupNameIndex) ? reader.GetString(businessGroupNameIndex) : string.Empty
                          },
                          Client = new Client()
                          {
                              Id = reader.GetInt32(clientIdIndex),
                              Name = reader.GetString(clientNameIndex)
                          },
                          Status = new ProjectStatus()
                          {
                              Id = reader.GetInt32(projectStatusIdIndex),
                              Name = reader.GetString(projectStatusNameIndex)
                          },
                          ProjectManagerId = !reader.IsDBNull(projectManagerIdIndex) ? reader.GetInt32(projectManagerIdIndex) : -1,
                          ProjectManagerNames = !reader.IsDBNull(projectManagerNameIndex) ? reader.GetString(projectManagerNameIndex) : string.Empty,
                          ExecutiveInChargeId = !reader.IsDBNull(executiveInChargeIdIndex) ? reader.GetInt32(executiveInChargeIdIndex) : -1,
                          ExecutiveInChargeName = !reader.IsDBNull(executiveInChargeNameIndex) ? reader.GetString(executiveInChargeNameIndex) : string.Empty,
                          ProjectCapabilityIds = !reader.IsDBNull(projectCapabilityIdsIndex) ? reader.GetString(projectCapabilityIdsIndex).Replace(";", ", ").TrimEnd(new char[] { ',', ' ' }) : string.Empty,
                          IsClientTimeEntryRequired = reader.GetBoolean(isClientTimeEntryRequiredIndex)
                      };

                    if (!reader.IsDBNull(divisionNameIndex))
                    {
                        project.Division = new ProjectDivision
                        {
                            Id = reader.GetInt32(divisionIdIndex),
                            Name = reader.GetString(divisionNameIndex)
                        };
                    }

                    if (!reader.IsDBNull(ChannelNameIndex))
                    {
                        project.Channel = new Channel
                        {
                            Id = reader.GetInt32(ChannelIdIndex),
                            Name = reader.GetString(ChannelNameIndex)
                        };
                    }

                    if (!reader.IsDBNull(OfferingNameIndex))
                    {
                        project.Offering = new Offering
                        {
                            Id = reader.GetInt32(offeringIdIndex),
                            Name = reader.GetString(OfferingNameIndex)
                        };
                    }

                    if (!reader.IsDBNull(revenueTypeNameIndex))
                    {
                        project.RevenueType = new Revenue
                        {
                            Id = reader.GetInt32(revenueTypeIdIndex),
                            Name = reader.GetString(revenueTypeNameIndex)
                        };
                    }
                    if (!reader.IsDBNull(subchannelIndex))
                    {
                        project.SubChannel = reader.GetString(subchannelIndex);
                    }
                    if (previousProjectNumberIndex >= 0 && previousProjectIdIndex >= 0)
                    {
                        project.PreviousProject = reader.IsDBNull(previousProjectNumberIndex) ? null : new Project { Id = reader.GetInt32(previousProjectIdIndex), ProjectNumber = reader.GetString(previousProjectNumberIndex) };
                    }
                    if (outsourceIdIndex >= 0)
                    {
                        project.OutsourceId = reader.GetInt32(outsourceIdIndex);
                    }

                    resultList.Add(project);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static List<ExpenseSummary> GetExpenseSummaryGroupedByProject(DateTime startDate, DateTime endDate, string clientIds, string divisionIds, string practiceIds, string projectIds, bool active, bool projected, bool Completed, bool proposed, bool inActive, bool experimental,bool atRisk)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ExpenseSummaryGroupedByProject, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdsParam, clientIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.DivisionIds, divisionIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIds, projectIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowProjectedParam, projected);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowCompletedParam, Completed);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowActiveParam, active);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowExperimentalParam, experimental);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowProposedParam, proposed);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowInactiveParam, inActive);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowAtRiskParam, atRisk);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<ExpenseSummary>();
                    ReadProjectExpenseSummaryGroupedByProject(reader, result);
                    reader.NextResult();
                    ReadMonthlyExpenses(reader, result, true);
                    return result;
                }
            }
        }

        public static List<ExpenseSummary> GetExpenseSummaryGroupedBytype(DateTime startDate, DateTime endDate, string expenseTypeIds)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ExpenseSummaryGroupedByType, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExpenseTypes, expenseTypeIds ?? (Object)DBNull.Value);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<ExpenseSummary>();
                    ReadProjectExpenseSummaryGroupedByType(reader, result);
                    reader.NextResult();
                    ReadMonthlyExpenses(reader, result, false);
                    return result;
                }
            }
        }

        private static void ReadProjectExpenseSummaryGroupedByProject(SqlDataReader reader, List<ExpenseSummary> resultList)
        {
            try
            {
                if (!reader.HasRows) return;
                int projectIdIndex = -1;
                int projectNameIndex = -1;
                int clientIdIndex = -1;
                int ClientNameIndex = -1;
                int practiceIdIndex = -1;
                int PracticeNameIndex = -1;
                int divisionIdIndex = -1;
                int divisionNameIndex = -1;
                int projectManagerIdIndex = -1;
                int projectManagerNameIndex = -1;
                int executiveInchargeIdIndex = -1;
                int executiveInchargeNameIndex = -1;
                int projectStatusIdIndex = -1;
                int projectStatusNameIndex = -1;
                int ProjectNumberIndex = -1;
                int groupIdIndex = -1;
                int groupNameIndex = -1;

                try
                {
                    projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                    projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                    clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
                    ClientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientName);
                    practiceIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                    PracticeNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeNameColumn);
                    divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);
                    divisionNameIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionName);
                    projectManagerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerId);
                    projectManagerNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerNameColumn);
                    executiveInchargeIdIndex = reader.GetOrdinal(Constants.ColumnNames.ExecutiveInChargeId);
                    executiveInchargeNameIndex = reader.GetOrdinal("ExecutiveInChargeName");
                    projectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusIdColumn);
                    projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
                    ProjectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
                    groupIdIndex = reader.GetOrdinal(Constants.ColumnNames.GroupId);
                    groupNameIndex = reader.GetOrdinal(Constants.ColumnNames.GroupName);
                }
                catch
                {

                }

                while (reader.Read())
                {
                    var expense = new ExpenseSummary();

                    if (projectIdIndex >= 0)
                    {
                        expense.Project = new Project()
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            Name = reader.GetString(projectNameIndex),
                            ProjectNumber = reader.GetString(ProjectNumberIndex),
                            Group = new ProjectGroup()
                            {
                                Id = reader.GetInt32(groupIdIndex),
                                Name = reader.GetString(groupNameIndex)
                            },
                            Client = new Client()
                            {
                                Id = reader.GetInt32(clientIdIndex),
                                Name = reader.GetString(ClientNameIndex)
                            },

                            Status = new ProjectStatus()
                            {
                                Id = reader.GetInt32(projectStatusIdIndex),
                                Name = reader.GetString(projectStatusNameIndex)
                            },
                            Practice = new Practice()
                            {
                                Id = reader.GetInt32(practiceIdIndex),
                                Name = reader.GetString(PracticeNameIndex)
                            },

                            Division = new ProjectDivision()
                            {
                                Id = reader.GetInt32(divisionIdIndex),
                                Name = reader.GetString(divisionNameIndex)
                            },

                            ProjectManagerId = !reader.IsDBNull(projectManagerIdIndex) ? reader.GetInt32(projectManagerIdIndex) : -1,
                            ProjectManagerNames = !reader.IsDBNull(projectManagerNameIndex) ? reader.GetString(projectManagerNameIndex) : string.Empty,
                            ExecutiveInChargeId = !reader.IsDBNull(executiveInchargeIdIndex) ? reader.GetInt32(executiveInchargeIdIndex) : -1,
                            ExecutiveInChargeName = !reader.IsDBNull(executiveInchargeNameIndex) ? reader.GetString(executiveInchargeNameIndex) : string.Empty,
                        };
                    }
                    resultList.Add(expense);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void ReadProjectExpenseSummaryGroupedByType(SqlDataReader reader, List<ExpenseSummary> resultList)
        {
            try
            {
                if (!reader.HasRows) return;

                int expenseTypeIdIndex = -1;
                int expenseTypeNameIndex = -1;

                try
                {
                    expenseTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.ExpenseTypeId);
                    expenseTypeNameIndex = reader.GetOrdinal(Constants.ColumnNames.ExpenseTypeName);
                }
                catch
                {

                }
                while (reader.Read())
                {
                    var expense = new ExpenseSummary();
                    if (expenseTypeIdIndex >= 0)
                    {
                        expense.Type = new ExpenseType()
                        {
                            Id = reader.GetInt32(expenseTypeIdIndex),
                            Name = reader.GetString(expenseTypeNameIndex)
                        };
                    }
                    resultList.Add(expense);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void ReadMonthlyExpenses(SqlDataReader reader, List<ExpenseSummary> resultList, bool isGroupByProject)
        {
            try
            {
                if (!reader.HasRows) return;

                int expenseIndex = reader.GetOrdinal(Constants.ColumnNames.Expense);
                int reimbursementIndex = reader.GetOrdinal(Constants.ColumnNames.ReimbursedExpense);
                int finacialDateIndex = reader.GetOrdinal(Constants.ColumnNames.FinancialDateColumn);
                int monthEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.MonthEnd);
                int expectedExpenseIdnex = reader.GetOrdinal(Constants.ColumnNames.ExpectedAmount);
                int expenseTypeIdIndex = -1;
                int projectIdIndex = -1;

                if (isGroupByProject)
                {
                    try
                    {
                        projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        expenseTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.ExpenseTypeId);
                    }
                    catch { }
                }

                while (reader.Read())
                {
                    ExpenseSummary expense = null;

                    if (isGroupByProject)
                    {
                        int projectId = reader.GetInt32(projectIdIndex);
                        if (resultList.Any(r => r.Project.Id == projectId))
                        {
                            expense = resultList.FirstOrDefault(r => r.Project.Id == projectId);
                            if (expense.MonthlyExpenses == null)
                            {
                                expense.MonthlyExpenses = new Dictionary<DateTime, decimal>();
                            }
                            if (expense.MonthlyReimburseExpense == null)
                            {
                                expense.MonthlyReimburseExpense = new Dictionary<DateTime, decimal>();
                            }
                            if (expense.MonthlyExpectedExpenses == null)
                            {
                                expense.MonthlyExpectedExpenses = new Dictionary<DateTime, decimal>();
                            }
                        }
                    }
                    else
                    {
                        int expenseTypeId = reader.GetInt32(expenseTypeIdIndex);
                        if (resultList.Any(r => r.Type.Id == expenseTypeId))
                        {
                            expense = resultList.FirstOrDefault(r => r.Type.Id == expenseTypeId);
                            if (expense.MonthlyExpenses == null)
                            {
                                expense.MonthlyExpenses = new Dictionary<DateTime, decimal>();
                            }
                            if (expense.MonthlyReimburseExpense == null)
                            {
                                expense.MonthlyReimburseExpense = new Dictionary<DateTime, decimal>();
                            }
                            if (expense.MonthlyExpectedExpenses == null)
                            {
                                expense.MonthlyExpectedExpenses = new Dictionary<DateTime, decimal>();
                            }
                        }
                    }

                    if (expense != null)
                    {
                        var finacialDate = reader.GetDateTime(finacialDateIndex);
                        var monthlyExpense = reader.GetDecimal(expenseIndex);
                        var monthlyReimbursement = reader.GetDecimal(reimbursementIndex);
                        var monthlyExpectedExpense = reader.GetDecimal(expectedExpenseIdnex);
                        expense.MonthlyExpenses.Add(finacialDate, monthlyExpense);
                        expense.MonthlyReimburseExpense.Add(finacialDate, monthlyReimbursement);
                        expense.MonthlyExpectedExpenses.Add(finacialDate, monthlyExpectedExpense);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ExpenseSummary> ExpenseDetailReport(DateTime startDate, DateTime endDate, int? projectId, int? expenseTypeId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ExpenseDetailReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId != null ? (object)projectId : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExpenseTypeId, expenseTypeId != null ? (object)expenseTypeId : DBNull.Value);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<ExpenseSummary>();
                    ReadExpenseDetails(reader, result);
                    return result;
                }
            }
        }

        public static List<ExpenseSummary> DetailedExpenseSummary(DateTime startDate, DateTime endDate, string clientIds, string divisionIds, string practiceIds, string projectIds, bool active, bool projected, bool Completed, bool proposed, bool inActive, bool experimental, bool atRisk, string expenseTypeIds)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ExpenseSummaryDetails, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdsParam, clientIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.DivisionIds, divisionIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIds, projectIds ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowProjectedParam, projected);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowCompletedParam, Completed);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowActiveParam, active);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowExperimentalParam, experimental);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowProposedParam, proposed);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowInactiveParam, inActive);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowAtRiskParam, atRisk);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExpenseTypes, expenseTypeIds ?? (Object)DBNull.Value);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<ExpenseSummary>();
                    ReadExpenseDetails(reader, result);
                    return result;
                }
            }
        }

        private static void ReadExpenseDetails(SqlDataReader reader, List<ExpenseSummary> resultList)
        {
            try
            {
                if (!reader.HasRows) return;

                int expenseIdIndex = reader.GetOrdinal(Constants.ColumnNames.ExpenseId);
                int expenseNameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
                int expenseTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.ExpenseTypeId);
                int expenseTypeNameIndex = reader.GetOrdinal(Constants.ColumnNames.ExpenseTypeName);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
                int milestoneIdIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneId);
                int milestoneNameIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneName);
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);

                int expenseIndex = reader.GetOrdinal(Constants.ColumnNames.Expense);
                int expectedExpenseIdnex = reader.GetOrdinal(Constants.ColumnNames.ExpectedAmount);
                int reimbursementIndex = reader.GetOrdinal(Constants.ColumnNames.ReimbursedExpense);
                int finacialDateIndex = reader.GetOrdinal(Constants.ColumnNames.FinancialDateColumn);
                int monthEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.MonthEnd);
                int monthNumberIndex = reader.GetOrdinal("MonthNumber");

                while (reader.Read())
                {
                    ExpenseSummary expense = new ExpenseSummary()
                    {
                        Project = new Project()
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            Name = reader.GetString(projectNameIndex),
                            ProjectNumber = reader.GetString(projectNumberIndex)
                        },

                        Expense = new ProjectExpense()
                        {
                            Id = reader.GetInt32(expenseIdIndex),
                            Name = reader.GetString(expenseNameIndex),
                            Milestone = new Milestone()
                            {
                                Id = reader.GetInt32(milestoneIdIndex),
                                Description = reader.GetString(milestoneNameIndex)
                            },
                            MilestoneName = reader.GetString(milestoneNameIndex),
                            ProjectId = reader.GetInt32(projectIdIndex),
                            StartDate = reader.GetDateTime(startDateIndex),
                            EndDate = reader.GetDateTime(endDateIndex),
                            Amount = reader.GetDecimal(expenseIndex),
                            ExpectedAmount = reader.GetDecimal(expectedExpenseIdnex),
                            Reimbursement = reader.GetDecimal(reimbursementIndex),
                            Type = new ExpenseType()
                            {
                                Id = reader.GetInt32(expenseTypeIdIndex),
                                Name = reader.GetString(expenseTypeNameIndex)
                            }
                        },
                        MonthStartDate = reader.GetDateTime(finacialDateIndex),
                        MonthEndDate = reader.GetDateTime(monthEndDateIndex)
                    };
                    resultList.Add(expense);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}


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
using DataTransferObjects.Reports.ByAccount;
using DataTransferObjects.Reports.HumanCapital;

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
                int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
                int billingTypeIndex = reader.GetOrdinal(Constants.ColumnNames.BillingType);
                int timeEntrySectionIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeEntrySectionId);
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int personFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int personLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int hourlyRateIndex = reader.GetOrdinal(Constants.ColumnNames.HourlyRate); ;
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
                        BillableType = reader.GetString(billingTypeIndex)

                    };
                    ptd.Person = new Person
                    {
                        Id = reader.GetInt32(personIdIndex),
                        FirstName = reader.GetString(personFirstNameIndex),
                        LastName = reader.GetString(personLastNameIndex),
                        EmployeeNumber = reader.GetString(employeeNumberIndex),
                        IsOffshore = reader.GetBoolean(isOffshoreIndex)
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
                int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
                int billingTypeIndex = reader.GetOrdinal(Constants.ColumnNames.BillingType);
                int timeEntrySectionIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeEntrySectionId);

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
                        BillableType = reader.GetString(billingTypeIndex)

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

        public static List<PersonLevelGroupedHours> TimePeriodSummaryReportByResource(DateTime startDate, DateTime endDate, bool includePersonsWithNoTimeEntries, string personTypes, string seniorityIds, string timescaleNames, string personStatusIds, string personDivisionIds)
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

                if (seniorityIds != null)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.SeniorityIdsParam, seniorityIds);
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
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
                int projectNonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNonBillableHours);
                int businessDevelopmentHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessDevelopmentHours);
                int internalHoursIndex = reader.GetOrdinal(Constants.ColumnNames.InternalHours);

                int pTOHoursIndex = reader.GetOrdinal(Constants.ColumnNames.PTOHours);
                int holidayHoursIndex = reader.GetOrdinal(Constants.ColumnNames.HolidayHours);
                int juryDutyHoursIndex = reader.GetOrdinal(Constants.ColumnNames.JuryDutyHours);
                int bereavementHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BereavementHours);
                int oRTHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ORTHours);
                int unpaidHoursIndex = reader.GetOrdinal(Constants.ColumnNames.UnpaidHours);
                int sickOrSafeLeaveHoursIndex = reader.GetOrdinal(Constants.ColumnNames.SickOrSafeLeaveHours);

                int personSeniorityIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonSeniorityId);
                int personSeniorityNameIndex = reader.GetOrdinal(Constants.ColumnNames.PersonSeniorityName);
                int utlizationPercentIndex = reader.GetOrdinal(Constants.ColumnNames.UtlizationPercent);
                int timeScaleIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleColumn);
                int isOffShoreIndex = reader.GetOrdinal(Constants.ColumnNames.IsOffshore);
                int personStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusId);
                int personStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusName);
                int divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);

                while (reader.Read())
                {
                    PersonLevelGroupedHours PLGH = new PersonLevelGroupedHours();
                    Person person = new Person
                                            {
                                                Id = reader.GetInt32(personIdIndex),
                                                FirstName = reader.GetString(firstNameIndex),
                                                LastName = reader.GetString(lastNameIndex),
                                                IsOffshore = reader.GetBoolean(isOffShoreIndex),
                                                Seniority = new Seniority
                                                {
                                                    Id = reader.GetInt32(personSeniorityIdIndex),
                                                    Name = reader.GetString(personSeniorityNameIndex)
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
                                                UtlizationPercent = !reader.IsDBNull(utlizationPercentIndex) ? (int)reader.GetDouble(utlizationPercentIndex) : 0d
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

                    PLGH.PTOHours = reader.GetDouble(pTOHoursIndex);
                    PLGH.HolidayHours = reader.GetDouble(holidayHoursIndex);
                    PLGH.BereavementHours = reader.GetDouble(bereavementHoursIndex);
                    PLGH.JuryDutyHours = reader.GetDouble(juryDutyHoursIndex);
                    PLGH.ORTHours = reader.GetDouble(oRTHoursIndex);
                    PLGH.UnpaidHours = reader.GetDouble(unpaidHoursIndex);
                    PLGH.SickOrSafeLeaveHours = reader.GetDouble(sickOrSafeLeaveHoursIndex);

                    PLGH.Person = person;
                    result.Add(PLGH);
                }
            }
        }

        public static GroupByAccount AccountSummaryReportByProject(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate, string projectStatusIds, string projectBillingTypes)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.AccountSummaryReportByProject, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.AccountIdParam, accountId);
                command.Parameters.AddWithValue(Constants.ParameterNames.BusinessUnitIdsParam, businessUnitIds != null ? businessUnitIds : (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectBillingTypesParam, projectBillingTypes != null ? projectBillingTypes : (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectStatusIdsParam, projectStatusIds != null ? projectStatusIds : (Object)DBNull.Value);
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
                command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdsParam, clientIds != null ? clientIds : (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectStatusIdsParam, projectStatusIds != null ? projectStatusIds : (Object)DBNull.Value);
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

                int groupIdIndex = -1;

                try
                {
                    groupIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn);
                }
                catch
                {
                    groupIdIndex = -1;
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

        public static GroupByAccount AccountSummaryReportByBusinessUnit(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.AccountSummaryReportByBusinessUnit, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.AccountIdParam, accountId);
                command.Parameters.AddWithValue(Constants.ParameterNames.BusinessUnitIdsParam, businessUnitIds != null ? businessUnitIds : (Object)DBNull.Value);
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
            if (reader.HasRows)
            {
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
        }

        private static void PopulateBusinessUnitTotalHoursPercent(List<BusinessUnitLevelGroupedHours> reportData)
        {
            double grandTotal = reportData.Sum(t => t.TotalHours);
            grandTotal = Math.Round(grandTotal, 2);

            if (grandTotal > 0)
            {
                foreach (BusinessUnitLevelGroupedHours buLevelGroupedHours in reportData)
                {
                    buLevelGroupedHours.BusinessUnitTotalHoursPercent = Convert.ToInt32((buLevelGroupedHours.TotalHours / grandTotal) * 100);
                }
            }
        }

        private static void ReadByBusinessUnit(SqlDataReader reader, List<BusinessUnitLevelGroupedHours> result)
        {
            if (reader.HasRows)
            {
                int billableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BillableHours);
                int businessUnitIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn);
                int businessUnitNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn);
                int businessUnitCodeIndex = reader.GetOrdinal(Constants.ColumnNames.GroupCodeColumn);
                int businessUnitStatusIndex = reader.GetOrdinal(Constants.ColumnNames.Active);
                int nonBillableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.NonBillableHours);
                int businessDevelopmentHoursIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessDevelopmentHours);
                int projectsCountIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectsCount);


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

                    BusinessUnitLevelGroupedHours buLGH = new BusinessUnitLevelGroupedHours();
                    buLGH.BillableHours = !reader.IsDBNull(billableHoursIndex) ? reader.GetDouble(billableHoursIndex) : 0d;
                    buLGH.NonBillableHours = !reader.IsDBNull(nonBillableHoursIndex) ? reader.GetDouble(nonBillableHoursIndex) : 0d;
                    buLGH.BusinessDevelopmentHours = !reader.IsDBNull(businessDevelopmentHoursIndex) ? reader.GetDouble(businessDevelopmentHoursIndex) : 0d;
                    buLGH.ProjectsCount = !reader.IsDBNull(projectsCountIndex) ? reader.GetInt32(projectsCountIndex) : 0;

                    buLGH.BusinessUnit = pg;



                    result.Add(buLGH);

                }
            }
        }

        public static List<BusinessUnitLevelGroupedHours> AccountReportGroupByBusinessUnit(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.AccountSummaryByBusinessDevelopment, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.AccountIdParam, accountId);
                command.Parameters.AddWithValue(Constants.ParameterNames.BusinessUnitIdsParam, businessUnitIds != null ? businessUnitIds : (Object)DBNull.Value);
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
                command.Parameters.AddWithValue(Constants.ParameterNames.BusinessUnitIdsParam, businessUnitIds != null ? businessUnitIds : (Object)DBNull.Value);
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
            if (reader.HasRows)
            {
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


                    GroupByBusinessUnit businessUnitTimeEntries = new GroupByBusinessUnit();

                    businessUnitTimeEntries.BusinessUnit = businessUnit;
                    businessUnitTimeEntries.DayTotalHours = new List<TimeEntriesGroupByDate>() 
                                                    {
                                                        dt
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
        }

        private static void ReadBusinessDevelopmentDetailsGroupByBusinessUnit(SqlDataReader reader, List<BusinessUnitLevelGroupedHours> result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);

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
                        LastName = reader.GetString(lastNameIndex)

                    };


                    PersonLevelGroupedHours PLGH = new PersonLevelGroupedHours();

                    PLGH.Person = person;
                    PLGH.DayTotalHours = new List<TimeEntriesGroupByDate>() 
                                                    {
                                                        dt
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
                        businessUnitLGH = new BusinessUnitLevelGroupedHours();
                        businessUnitLGH.BusinessUnit = new ProjectGroup()
                        {
                            Id = businessUnitId,
                            Name = reader.GetString(businessUnitNameIndex),
                            Code = reader.GetString(businessUnitCodeIndex),
                            IsActive = reader.GetBoolean(isActiveIndex)
                        };

                        businessUnitLGH.PersonLevelGroupedHoursList = new List<PersonLevelGroupedHours>();
                        businessUnitLGH.PersonLevelGroupedHoursList.Add(PLGH);

                        result.Add(businessUnitLGH);
                    }

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
            if (reader.HasRows)
            {
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

                    WorkTypeLevelGroupedHours worktypeLGH = new WorkTypeLevelGroupedHours();
                    worktypeLGH.BillableHours = !reader.IsDBNull(billableHoursIndex) ? reader.GetDouble(billableHoursIndex) : 0d;
                    worktypeLGH.NonBillableHours = !reader.IsDBNull(nonBillableHoursIndex) ? reader.GetDouble(nonBillableHoursIndex) : 0d;
                    worktypeLGH.WorkType = tt;
                    if (forecastedHoursIndex > 0)
                    {
                        worktypeLGH.ForecastedHours = reader.GetDouble(forecastedHoursIndex);
                    }
                    result.Add(worktypeLGH);

                }
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
                int isOffShoreIndex = reader.GetOrdinal(Constants.ColumnNames.IsOffshore);

                while (reader.Read())
                {
                    int personId = reader.GetInt32(personIdIndex);


                    PersonLevelGroupedHours PLGH = new PersonLevelGroupedHours();
                    Person person = new Person
                    {
                        Id = reader.GetInt32(personIdIndex),
                        FirstName = reader.GetString(firstNameIndex),
                        LastName = reader.GetString(lastNameIndex),
                        ProjectRoleName = reader.GetString(projectRoleNameIndex),
                        IsOffshore = reader.GetBoolean(isOffShoreIndex)
                    };

                    PLGH.BillableHours = !reader.IsDBNull(billableHoursIndex) ? reader.GetDouble(billableHoursIndex) : 0d;
                    PLGH.ProjectNonBillableHours = !reader.IsDBNull(nonBillableHoursIndex) ? reader.GetDouble(nonBillableHoursIndex) : 0d;
                    PLGH.BillableHoursUntilToday = !reader.IsDBNull(billableHoursUntilTodayIndex) ? reader.GetDouble(billableHoursUntilTodayIndex) : 0d;
                    PLGH.ForecastedHoursUntilToday = Convert.ToDouble(reader[forecastedHoursUntilTodayIndex]);
                    PLGH.BillingType = reader.GetString(billingTypeIndex);
                    PLGH.Person = person;
                    PLGH.ForecastedHours = Convert.ToDouble(reader[forecastedHoursIndex]);

                    result.Add(PLGH);
                }
            }
        }

        public static List<PersonLevelGroupedHours> ProjectDetailReportByResource(string projectNumber, int? milestoneId, DateTime? startDate, DateTime? endDate, string personRoleNames)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Reports.ProjectDetailReportByResource, connection))
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
                    ReadProjectDetailReportByResource(reader, result);
                    return result;
                }
            }

        }

        private static void ReadProjectDetailReportByResource(SqlDataReader reader, List<PersonLevelGroupedHours> result)
        {
            if (reader.HasRows)
            {
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
                                Name = reader.GetString(timeTypeNameIndex),
                                Code = reader.GetString(timeTypeCodeIndex)
                            }
                        };

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
                            IsOffshore = reader.GetBoolean(isOffShoreIndex)
                        };
                        PLGH.Person = person;
                        PLGH.TimeEntrySectionId = !reader.IsDBNull(timeEntrySectionIdIndex) ? reader.GetInt32(timeEntrySectionIdIndex) : 0;
                        PLGH.ForecastedHours = Convert.ToDouble(reader[forecastedHoursIndex]);
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
                command.Parameters.AddWithValue(Constants.ParameterNames.CategoryNamesParam, categoryNames != null ? categoryNames : (Object)DBNull.Value);

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
        }

        public static List<PersonLevelPayCheck> TimePeriodSummaryByResourcePayCheck(DateTime startDate, DateTime endDate, bool includePersonsWithNoTimeEntries, string personTypes, string seniorityIds, string timescaleNames, string personStatusIds, string personDivisionIds)
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

                if (seniorityIds != null)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.SeniorityIdsParam, seniorityIds);
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
            if (reader.HasRows)
            {
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
        }

        private static void ReadTimePeriodSummaryByResourcePayCheck(SqlDataReader reader, List<PersonLevelPayCheck> result, List<TimeTypeRecord> timeTypesList)
        {
            if (reader.HasRows)
            {
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
                    Dictionary<string, double> workTypeLevelTimeOffHours = new Dictionary<string, double>();
                    workTypeLevelTimeOffHours.Add(timeTypesList.First(t => t.IsPTOTimeType).Name, reader.GetDouble(pTOHoursIndex));
                    workTypeLevelTimeOffHours.Add(timeTypesList.First(t => t.IsHolidayTimeType).Name, reader.GetDouble(holidayHoursIndex));
                    workTypeLevelTimeOffHours.Add(timeTypesList.First(t => t.IsJuryDutyTimeType).Name, reader.GetDouble(juryDutyHoursIndex));
                    workTypeLevelTimeOffHours.Add(timeTypesList.First(t => t.IsBereavementTimeType).Name, reader.GetDouble(bereavementHoursIndex));
                    workTypeLevelTimeOffHours.Add(timeTypesList.First(t => t.IsORTTimeType).Name, reader.GetDouble(oRTHoursIndex));
                    workTypeLevelTimeOffHours.Add(timeTypesList.First(t => t.IsUnpaidTimeType).Name, reader.GetDouble(unpaidHoursIndex));
                    workTypeLevelTimeOffHours.Add(timeTypesList.First(t => t.IsSickLeaveTimeType).Name, reader.GetDouble(sickOrSafeLeaveHoursIndex));
                    PLPC.WorkTypeLevelTimeOffHours = workTypeLevelTimeOffHours;
                    result.Add(PLPC);
                }
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
            if (reader.HasRows)
            {
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
            if (reader.HasRows)
            {
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
        }

        private static void ReadPersons(SqlDataReader reader, List<Person> result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int personStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusId);
                int personStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusName);
                int timeScaleIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleColumn);
                int timescaleNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleName);

                while (reader.Read())
                {
                    var person = ReadBasicPersonDetails(reader, personIdIndex, firstNameIndex, lastNameIndex, personStatusIdIndex, personStatusNameIndex, timeScaleIndex, timescaleNameIndex);
                    result.Add(person);
                }
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

        public static List<Person> NewHireReport(DateTime startDate, DateTime endDate, string personStatusIds, string payTypeIds, string practiceIds, bool excludeInternalPractices, string personDivisionIds, string seniorityIds, string hireDates, string recruiterIds)
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
                command.Parameters.AddWithValue(Constants.ParameterNames.SeniorityIdsParam, seniorityIds);
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
            if (reader.HasRows)
            {
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
                int personSeniorityIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonSeniorityId);
                int personSeniorityNameIndex = reader.GetOrdinal(Constants.ColumnNames.PersonSeniorityName);
                int seniorityCategoryIdIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorityCategoryId);
                int seniorityCategoryIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorityCategory);
                int divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);
                int hireDateIndex = reader.GetOrdinal(Constants.ColumnNames.HireDateColumn);

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

                    var recruiterList = new List<RecruiterCommission>();
                    if (!reader.IsDBNull(recruiterIdIndex))
                    {
                        var recruiterCommission = new RecruiterCommission { Recruiter = new Person { Id = reader.GetInt32(recruiterIdIndex), FirstName = reader.GetString(recruiterFirstNameIndex), LastName = reader.GetString(recruiterLastNameIndex) } };
                        recruiterList.Add(recruiterCommission);
                    }
                    person.RecruiterCommission = recruiterList;
                    if (!reader.IsDBNull(timeScaleIndex))
                    {
                        person.Seniority = new Seniority
                        {
                            Id = reader.GetInt32(personSeniorityIdIndex),
                            Name = reader.GetString(personSeniorityNameIndex),
                            SeniorityCategory = new SeniorityCategory()
                            {
                                Id = reader.GetInt32(seniorityCategoryIdIndex),
                                Name = reader.GetString(seniorityCategoryIndex)
                            }
                        };
                    }

                    if (!reader.IsDBNull(divisionIdIndex))
                    {
                        person.DivisionType = (PersonDivisionType)reader.GetInt32(divisionIdIndex);
                    }

                    person.HireDate = reader.GetDateTime(hireDateIndex);

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
        }

        public static TerminationPersonsInRange TerminationReport(DateTime startDate, DateTime endDate, string payTypeIds, string personStatusIds, string seniorityIds, string terminationReasonIds, string practiceIds, bool excludeInternalPractices, string personDivisionIds, string recruiterIds, string hireDates, string terminationDates)
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
                command.Parameters.AddWithValue(Constants.ParameterNames.SeniorityIdsParam, seniorityIds);
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
            if (reader.HasRows)
            {
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
                    TerminationPersonsInRange tpr = new TerminationPersonsInRange();
                    tpr.StartDate = reader.GetDateTime(startDateIndex);
                    tpr.EndDate = reader.GetDateTime(endDateIndex);
                    tpr.ActivePersonsCountAtTheBeginning = reader.GetInt32(activePersonsAtTheBeginningIndex);
                    tpr.NewHiresCountInTheRange = reader.GetInt32(newHiredInTheRangeIndex);
                    tpr.TerminationsW2SalaryCountInTheRange = reader.GetInt32(terminationsW2SalaryCountInTheRange);
                    tpr.TerminationsW2HourlyCountInTheRange = reader.GetInt32(terminationsW2HourlyCountInTheRange);
                    tpr.Terminations1099HourlyCountInTheRange = reader.GetInt32(terminations1099HourlyCountInTheRange);
                    tpr.Terminations1099PORCountInTheRange = reader.GetInt32(terminations1099PORCountInTheRange);
                    tpr.TerminationsCountInTheRange = reader.GetInt32(terminationsCountInTheRange);
                    tpr.NewHiredCumulativeInTheRange = reader.GetInt32(newHiredCumulativeInTheRange);
                    tpr.TerminationsCumulativeEmployeeCountInTheRange = reader.GetInt32(terminationsCumulativeEmployeeCountInTheRange);
                    result.Add(tpr);
                }
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
                    var persons = new List<Person>();
                    ReadTerminationPersonsInRange(reader, result);
                    return result;
                }
            }
        }
    }
}


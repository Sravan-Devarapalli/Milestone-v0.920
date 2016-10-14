using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;
using DataAccess.Other;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
using DataTransferObjects.Reports;
using DataTransferObjects.TimeEntry;

namespace DataAccess
{
    /// <summary>
    /// Access person data in database
    /// </summary>
    public static class PersonDAL
    {
        #region Constants

        #region Parameters

        private const string PersonIdParam = "@PersonId";
        private const string FirstNameParam = "@FirstName";
        private const string LastNameParam = "@LastName";
        private const string HireDateParam = "@HireDate";
        private const string TerminationDateParam = "@TerminationDate";
        private const string TerminationReasonIdParam = "@TerminationReasonId";
        private const string TeleponeNumberParam = "@TelephoneNumber";
        private const string AliasParam = "@Alias";
        private const string DefaultPracticeParam = "@DefaultPractice";
        private const string ShowAllParam = "@ShowAll";
        private const string PracticeIdParam = "@PracticeId";
        private const string TimescaleIdParam = "@TimescaleId";
        private const string TimescaleIdsListParam = "@TimescaleIdsList";
        private const string PageSizeParam = "@PageSize";
        private const string PageNoParam = "@PageNo";
        private const string MilestonePersonIdParam = "@MilestonePersonId";
        private const string StartDateParam = "@StartDate";
        private const string EndDateParam = "@EndDate";
        private const string PersonStatusIdParam = "@PersonStatusId";
        private const string RoleNameParam = "@RoleName";
        private const string TitleNameParam = "@TitleNames";
        private const string PersonStatusIdsListParam = "@PersonStatusIdsList";
        private const string LookedParam = "@Looked";
        private const string EmployeeNumberParam = "@EmployeeNumber";
        private const string UserIdParam = "@UserId";
        private const string SeniorityIdParam = "@SeniorityId";
        private const string DateTodayParam = "@DateToday";
        private const string UserLoginParam = "@UserLogin";
        private const string IncludeInactiveParam = "@IncludeInactive";
        private const string RecruiterIdParam = "@RecruiterId";
        private const string RecruiterIdsListParam = "@RecruiterIdsList";
        private const string MaxSeniorityLevelParam = "@MaxSeniorityLevel";
        private const string MilestoneStartParam = "@mile_start";
        private const string MilestoneEndParam = "@mile_end";
        private const string ClientIdsListParam = "@ClientIdsList";
        private const string GroupIdsListParam = "@GroupIdsList";
        private const string SalespersonIdsListParam = "@SalespersonIdsList";
        private const string PracticeManagerIdsListParam = "@PracticeManagerIdsList";
        private const string DivisionIdsListParam = "@DivisionIdsList";
        private const string PracticeIdsListParam = "@PracticeIdsList";
        private const string SortByParam = "@SortBy";
        private const string AppicationNameParam = "@ApplicationName";
        private const string UserNameParam = "@UserName";
        private const string PasswordParam = "@Password";
        private const string PasswordSaltParam = "@PasswordSalt";
        private const string TablesToDeleteFromParam = "@TablesToDeleteFrom";
        private const string PasswordQuestionParam = "@PasswordQuestion";
        private const string PasswordAnswerParam = "@PasswordAnswer";
        private const string IsApprovedParam = "@IsApproved";
        private const string UniqueEmailParam = "@UniqueEmail";
        private const string PasswordFormatParam = "@PasswordFormat";
        private const string NewPasswordParam = "@NewPassword";
        private const string CurrentTimeUtcParam = "@CurrentTimeUtc";
        private const string OldAliasParam = "@OldAlias";
        private const string NewAliasParam = "@NewAlias";
        private const string EmailParam = "@Email";
        private const string PersonIdsParam = "@PersonIds";
        private const string ProjectedParam = "@Projected";
        private const string ActiveParam = "@Active";
        private const string TerminatedParam = "@Terminated";
        private const string TerminationPendingParam = "@TerminationPending";
        private const string AlphabetParam = "@Alphabet";
        private const string TimeScaleIdsParam = "@TimescaleIds";
        private const string PersonStatusIdsParam = "@PersonStatusIds";
        private const string IsOffshoreParam = "@IsOffshore";
        private const string PaychexIDParam = "@PaychexID";
        private const string PersonDivisionIdParam = "@PersonDivisionId";
        private const string EffectiveDateParam = "@EffectiveDate";
        private const string TitleIdParam = "@TitleId";
        private const string NumTablesDeletedFromParam = "@NumTablesDeletedFrom";
        private const string SLTApprovalParam = "@SLTApproval";
        private const string SLTPTOApprovalParam = "@SLTPTOApproval";

        //StrawMan Puppose.
        private const string AmountParam = "@Amount";

        private const string TimescaleParam = "@Timescale";
        private const string VacationDaysParam = "@VacationDays";
        private const string BonusAmountParam = "@BonusAmount";
        private const string BonusHoursToCollectParam = "@BonusHoursToCollect";

        #endregion Parameters

        #region Columns

        private const string LastLoginDateColumn = "LastLoginDate";
        private const string DescriptionColumn = "Description";
        private const string RateColumn = "Rate";
        private const string HoursToCollectColumn = "HoursToCollect";
        private const string StartDateColumn = "StartDate";
        private const string EndDateColumn = "EndDate";
        private const string PersonIdColumn = "PersonId";
        private const string UserNameColumn = "UserName";
        private const string PasswordColumn = "Password";
        private const string PasswordSaltColumn = "PasswordSalt";
        private const string FirstNameColumn = "FirstName";
        private const string PreferredFirstName = "PreferredFirstName";
        private const string LastNameColumn = "LastName";
        private const string NameColumn = "Name";
        private const string MonthColumn = "Month";
        private const string RevenueColumn = "Revenue";
        private const string CogsColumn = "Cogs";
        private const string MarginColumn = "Margin";
        private const string HireDateColumn = "HireDate";
        private const string TerminationDateColumn = "TerminationDate";
        private const string IsPercentageColumn = "IsPercentage";
        private const string PersonStatusIdColumn = "PersonStatusId";
        private const string PersonStatusNameColumn = "PersonStatusName";
        private const string OverheadRateTypeIdColumn = "OverheadRateTypeId";
        private const string OverheadRateTypeNameColumn = "OverheadRateTypeName";
        private const string EmployeeNumberColumn = "EmployeeNumber";
        private const string BillRateMultiplierColumn = "BillRateMultiplier";
        private const string AliasColumn = "Alias";
        private const string SeniorityIdColumn = "SeniorityId";
        private const string SeniorityNameColumn = "SeniorityName";
        private const string TimescaleColumn = "Timescale";
        private const string TimescaleIdColumn = "TimescaleId";
        private const string WeeklyUtilColumn = "wutil";
        private const string AvgUtilColumn = "wutilAvg";
        private const string PersonVactionDaysColumn = "PersonVactionDays";
        private const string TargetIdColumn = "TargetId";
        private const string TargetTypeColumn = "TargetType";

        private const string AmountColumn = "Amount";
        private const string TimescaleNameColumn = "TimescaleName";
        private const string AmountHourlyColumn = "AmountHourly";
        private const string VacationDaysColumn = "VacationDays";
        private const string BonusAmountColumn = "BonusAmount";
        private const string BonusHoursToCollectColumn = "BonusHoursToCollect";
        private const string IsYearBonusColumn = "IsYearBonus";
        private const string PayPersonIdColumn = "PayPersonId";
        private const string PracticeNameColumn = "PracticeName";
        private const string IsMinimumLoadFactorColumn = "IsMinimumLoadFactor";
        private const string TimeScaleChangeStatusColumn = "TimeScaleChangeStatus";

        private const string TerminationReasonIdColumn = "TerminationReasonId";
        private const string TerminationReasonColumn = "TerminationReason";
        private const string PersonStatusId = "PersonStatusId";

        #endregion Columns

        #region Functions

        private const string CompensationCoversMilestoneFunction = "dbo.CompensationCoversMilestone";
        private const string GetCurrentPayTypeFunction = "dbo.GetCurrentPayType";

        #endregion Functions

        #region Queries

        private const string OneArgUdfFunctionQuery = "SELECT {0}({1})";

        private const string CompensationCoversMilestoneQuery = "SELECT {0}({1}, DEFAULT, DEFAULT)";
        private const string CompensationCoversExtendedMilestoneQuery = "SELECT {0}({1}, {2}, {3})";

        #endregion Queries

        #endregion Constants

        /// <summary>
        /// Retrives consultans report
        /// </summary>
        /// <returns>An <see cref="Opportunity"/> object if found and null otherwise.</returns>
        public static DataSet GetPersonMilestoneWithFinancials(int personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonMilestoneWithFinancials, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam, personId);
                connection.Open();

                var adapter = new SqlDataAdapter(command);
                var dataset = new DataSet();
                adapter.Fill(dataset, "PersonMilestoneWithFinancials");

                return dataset;
            }
        }

        /// <summary>
        /// Sets the given person as default manager.
        /// </summary>
        /// <param name="personId"></param>
        public static void SetAsDefaultManager(int personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.SetDefaultManager, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.DefaultManagerId, personId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Retrives consultans report
        /// </summary>
        /// <returns>An <see cref="Opportunity"/> object if found and null otherwise.</returns>
        public static List<ConsultantUtilizationPerson> GetConsultantUtilizationWeekly(ConsultantTimelineReportContext context)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                List<ConsultantUtilizationPerson> result = null;

                using (var command = new SqlCommand(Constants.ProcedureNames.Person.ConsultantUtilizationWeeklyProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ActivePersons, context.ActivePersons);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ActiveProjects, context.ActiveProjects);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedPersons, context.ProjectedPersons);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedProjects, context.ProjectedProjects);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ExperimentalProjects, context.ExperimentalProjects);
                    command.Parameters.AddWithValue(Constants.ParameterNames.InternalProjects, context.InternalProjects);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProposedProjects, context.ProposedProjects);
                    command.Parameters.AddWithValue(Constants.ParameterNames.CompletedProjects, context.CompletedProjects);
                    command.Parameters.AddWithValue(Constants.ParameterNames.Start, context.Start);
                    command.Parameters.AddWithValue(Constants.ParameterNames.Granularity, context.Granularity);
                    command.Parameters.AddWithValue(Constants.ParameterNames.Period, context.Period);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, context.PracticeIdList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.TimescaleIds, context.TimescaleIdList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.SortId, context.SortId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.SortDirection, context.SortDirection);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ExcludeInternalPractices, context.ExcludeInternalPractices);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsSampleReport, context.IsSampleReport);

                    command.Parameters.AddWithValue(Constants.ParameterNames.DivisionIds, context.DivisionIdList);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        result = GetPersonLoadWithVactionDays(reader);
                        reader.NextResult();
                        ReadPersonsEmploymentHistory(reader, result);
                        reader.NextResult();
                        ReadPersonTimeOffDates(reader, result);
                        reader.NextResult();
                        ReadPersonAssignedDates(reader, result);
                    }
                }
                if (result != null)
                {
                    GetWeeklyUtilizationForConsultant(context, result);

                }

                return result;
            }
        }

        public static void GetWeeklyUtilizationForConsultant(ConsultantTimelineReportContext context, List<ConsultantUtilizationPerson> consultantUtilizationPersonList)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetWeeklyUtilizationForConsultant, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ActivePersons, context.ActivePersons);
                command.Parameters.AddWithValue(Constants.ParameterNames.ActiveProjects, context.ActiveProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedPersons, context.ProjectedPersons);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedProjects, context.ProjectedProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExperimentalProjects, context.ExperimentalProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProposedProjects, context.ProposedProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.InternalProjects, context.InternalProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.CompletedProjects, context.CompletedProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.Start, context.Start);
                command.Parameters.AddWithValue(Constants.ParameterNames.Granularity, context.Granularity);
                command.Parameters.AddWithValue(Constants.ParameterNames.Period, context.Period);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, context.PracticeIdList);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimescaleIds, context.TimescaleIdList);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExcludeInternalPractices, context.ExcludeInternalPractices);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsSampleReport, context.IsSampleReport);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsBadgeIncluded, context.IncludeBadgeStatus);
                command.Parameters.AddWithValue(Constants.ParameterNames.UtilizationType, context.UtilizationType);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (context.UtilizationType == 0)
                        ReadPersonsWeeklyUtlization(reader, consultantUtilizationPersonList);
                    else if (context.UtilizationType == 1)
                        ReadPersonsWeeklyUtlizationByProject(reader, consultantUtilizationPersonList);
                    reader.NextResult();
                    ReadPersonBadgedProjects(reader, consultantUtilizationPersonList);
                }
            }
        }

        private static void ReadPersonBadgedProjects(SqlDataReader reader, List<ConsultantUtilizationPerson> result)
        {
            if (result != null)
            {
                if (reader.HasRows)
                {
                    int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                    int badgeStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeStartDate);
                    int badgeEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeEndDate);

                    while (reader.Read())
                    {
                        var personId = reader.GetInt32(personIdIndex);
                        if (result.Any(p => p.Person.Id == personId))
                        {
                            var first = result.FirstOrDefault(p => p.Person.Id == personId);
                            if (first.Person.BadgedProjects == null)
                                first.Person.BadgedProjects = new List<MSBadge>();
                            first.Person.BadgedProjects.Add(new MSBadge()
                            {
                                BadgeStartDate = reader.GetDateTime(badgeStartDateIndex),
                                BadgeEndDate = reader.GetDateTime(badgeEndDateIndex)
                            });
                        }
                    }
                }
            }
        }

        public static List<ConsultantUtilizationPerson> ConsultantUtilizationDailyByPerson(int personId, ConsultantTimelineReportContext context)
        {
            List<ConsultantUtilizationPerson> result = null;
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.ConsultantUtilizationDailyByPersonProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ActiveProjects, context.ActiveProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedProjects, context.ProjectedProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.InternalProjects, context.InternalProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExperimentalProjects, context.ExperimentalProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProposedProjects, context.ProposedProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.CompletedProjects, context.CompletedProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.DaysForward, context.Period);
                command.Parameters.AddWithValue(Constants.ParameterNames.Start, context.Start);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    result = GetPersonLoad(reader);
                    reader.NextResult();
                    ReadPersonTimeOffDates(reader, result);
                }
                return result;
            }
        }

        /// <summary>
        /// reads a dataset of persons into a collection
        /// </summary>
        private static List<ConsultantUtilizationPerson> GetPersonLoad(SqlDataReader reader)
        {
            if (reader.HasRows)
            {
                int personStatusIdIndex = reader.GetOrdinal(PersonStatusId);
                int personStatusNameIndex = reader.GetOrdinal(NameColumn);
                int firstNameIndex = reader.GetOrdinal(FirstNameColumn);
                int lastNameIndex = reader.GetOrdinal(LastNameColumn);
                int employeeNumberIndex = reader.GetOrdinal(EmployeeNumberColumn);
                int weeklyLoadIndex = reader.GetOrdinal(WeeklyUtilColumn);
                int timescaleNameIndex = reader.GetOrdinal(TimescaleColumn);
                int timescaleIdIndex = reader.GetOrdinal(TimescaleIdColumn);
                int hireDateIndex = reader.GetOrdinal(HireDateColumn);
                int avgUtilIndex = reader.GetOrdinal(AvgUtilColumn);

                var res = new List<ConsultantUtilizationPerson>();
                while (reader.Read())
                {
                    var person =
                        new Person
                        {
                            Id = (int)reader[PersonIdColumn],
                            FirstName = (string)reader[firstNameIndex],
                            LastName = (string)reader[lastNameIndex],
                            EmployeeNumber = (string)reader[employeeNumberIndex],
                            Status = new PersonStatus
                            {
                                Id = (int)reader[personStatusIdIndex],
                                Name = (string)reader[personStatusNameIndex]
                            },
                            CurrentPay = new Pay
                            {
                                TimescaleName = reader.GetString(timescaleNameIndex),
                                Timescale = (TimescaleType)reader.GetInt32(timescaleIdIndex)
                            },
                            HireDate = (DateTime)reader[hireDateIndex]
                        };
                    int[] load = Utils.StringToIntArray((string)reader[weeklyLoadIndex]);
                    int avgUtil = reader.GetInt32(avgUtilIndex);
                    var consultPerson = new ConsultantUtilizationPerson()
                    {
                        Person = person,
                        WeeklyUtilization = load.ToList(),
                        Utilization = avgUtil

                    };
                    res.Add(consultPerson);
                }

                return res;
            }
            return null;
        }

        /// <summary>
        /// reads a dataset of persons into a collection
        /// </summary>
        private static List<ConsultantUtilizationPerson> GetPersonLoadWithVactionDays(SqlDataReader reader)
        {
            if (reader.HasRows)
            {
                int personStatusIdIndex = reader.GetOrdinal(PersonStatusId);
                int personStatusNameIndex = reader.GetOrdinal(NameColumn);
                int firstNameIndex = reader.GetOrdinal(FirstNameColumn);
                int lastNameIndex = reader.GetOrdinal(LastNameColumn);
                int employeeNumberIndex = reader.GetOrdinal(EmployeeNumberColumn);
                int timescaleNameIndex = reader.GetOrdinal(TimescaleColumn);
                int timescaleIdIndex = reader.GetOrdinal(TimescaleIdColumn);
                int hireDateIndex = reader.GetOrdinal(HireDateColumn);
                int avgUtilIndex = reader.GetOrdinal(AvgUtilColumn);
                int personVacationDaysIndex = reader.GetOrdinal(PersonVactionDaysColumn);
                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                int badgeStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeStartDate);
                int badgeEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeEndDate);
                int breakStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BreakStartDate);
                int breakEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BreakEndDate);
                int blockStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BlockStartDate);
                int blockEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BlockEndDate);
                int MSManageServiceIndex = reader.GetOrdinal("ManageServiceContract");
                int isInvestmentResourceIndex = reader.GetOrdinal(Constants.ColumnNames.IsInvestmentResource);
                int targetUtilizationIndex = reader.GetOrdinal(Constants.ColumnNames.TargetUtilization);
                int practiceId = -1;
                int practiceArea = -1;
                try
                {
                    practiceId = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                    practiceArea = reader.GetOrdinal(Constants.ColumnNames.PracticeNameColumn);
                }
                catch
                {
                }
                var res = new List<ConsultantUtilizationPerson>();
                while (reader.Read())
                {
                    ConsultantUtilizationPerson item = new ConsultantUtilizationPerson();
                    var person =
                        new Person
                        {
                            Id = (int)reader[PersonIdColumn],
                            FirstName = (string)reader[firstNameIndex],
                            LastName = (string)reader[lastNameIndex],
                            EmployeeNumber = (string)reader[employeeNumberIndex],
                            Status = new PersonStatus
                            {
                                Id = (int)reader[personStatusIdIndex],
                                Name = (string)reader[personStatusNameIndex]
                            },
                            CurrentPay = new Pay
                            {
                                TimescaleName = reader.GetString(timescaleNameIndex),
                                Timescale = (TimescaleType)reader.GetInt32(timescaleIdIndex)
                            },
                            HireDate = (DateTime)reader[hireDateIndex],
                            Title = new Title
                            {
                                TitleId = reader.GetInt32(titleIdIndex),
                                TitleName = reader.GetString(titleIndex)
                            },
                            Badge = new MSBadge()
                            {
                                BadgeStartDate = reader.IsDBNull(badgeStartDateIndex) ? null : (DateTime?)reader.GetDateTime(badgeStartDateIndex),
                                BadgeEndDate = reader.IsDBNull(badgeEndDateIndex) ? null : (DateTime?)reader.GetDateTime(badgeEndDateIndex),
                                BreakStartDate = reader.IsDBNull(breakStartDateIndex) ? null : (DateTime?)reader.GetDateTime(breakStartDateIndex),
                                BreakEndDate = reader.IsDBNull(breakEndDateIndex) ? null : (DateTime?)reader.GetDateTime(breakEndDateIndex),
                                BlockStartDate = reader.IsDBNull(blockStartDateIndex) ? null : (DateTime?)reader.GetDateTime(blockStartDateIndex),
                                BlockEndDate = reader.IsDBNull(blockEndDateIndex) ? null : (DateTime?)reader.GetDateTime(blockEndDateIndex),
                                IsMSManagedService = reader.IsDBNull(MSManageServiceIndex) ? false : reader.GetBoolean(MSManageServiceIndex)
                            },
                            IsInvestmentResource = reader.GetBoolean(isInvestmentResourceIndex),
                            TargetUtilization = reader.IsDBNull(targetUtilizationIndex) ? null : (int?)reader.GetInt32(targetUtilizationIndex)
                        };

                    if (practiceId > -1)
                    {
                        person.DefaultPractice = new Practice()
                        {
                            Id = reader.GetInt32(titleIdIndex),
                            Name = reader.GetString(practiceArea)
                        };
                    }

                    if (Convert.IsDBNull(reader[TerminationDateColumn]))
                    {
                        person.TerminationDate = null;
                    }
                    else
                    {
                        person.TerminationDate = (DateTime)reader[TerminationDateColumn];
                    }
                    item.WeeklyPayTypes = new List<int>();
                    item.WeeklyUtilization = new List<int>();
                    item.WeeklyVacationDays = new List<int>();
                    item.ProjectedHoursList = new List<decimal>();
                    item.TimeOffDates = new Dictionary<DateTime, double>();
                    item.Person = person;
                    item.PersonVacationDays = reader.GetInt32(personVacationDaysIndex);
                    res.Add(item);
                }

                return res;
            }
            return null;
        }

        private static void ReadPersonsWeeklyUtlization(SqlDataReader reader, List<ConsultantUtilizationPerson> consultantUtilizationPersonList)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int weeklyUtlizationIndex = reader.GetOrdinal(Constants.ColumnNames.WeeklyUtlization);
                int projectedHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectedHours);
                int availableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.AvailableHours);
                int payTypeIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleColumn);
                int vacationDaysIndex = reader.GetOrdinal(Constants.ColumnNames.VacationDays);
                while (reader.Read())
                {
                    int personid = reader.GetInt32(personIdIndex);
                    if (consultantUtilizationPersonList.Any(p => p.Person.Id == personid))
                    {
                        var record = consultantUtilizationPersonList.First(p => p.Person.Id == personid);
                        record.ProjectedHours = record.ProjectedHours + reader.GetDecimal(projectedHoursIndex);
                        record.AvailableHours = record.AvailableHours + reader.GetDecimal(availableHoursIndex);
                        record.WeeklyUtilization.Add(reader.GetInt32(weeklyUtlizationIndex));
                        record.ProjectedHoursList.Add(reader.GetDecimal(projectedHoursIndex));
                        record.WeeklyPayTypes.Add(reader.IsDBNull(payTypeIndex) ? 0 : reader.GetInt32(payTypeIndex));
                        record.WeeklyVacationDays.Add(reader.GetInt32(vacationDaysIndex));
                    }
                }
            }
        }

        private static void ReadPersonsWeeklyUtlizationByProject(SqlDataReader reader, List<ConsultantUtilizationPerson> consultantUtilizationPersonList)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
                int weeklyUtlizationIndex = reader.GetOrdinal(Constants.ColumnNames.WeeklyUtlization);
                int projectedHoursIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectedHours);
                int availableHoursIndex = reader.GetOrdinal(Constants.ColumnNames.AvailableHours);
                int payTypeIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleColumn);
                int vacationDaysIndex = reader.GetOrdinal(Constants.ColumnNames.VacationDays);
                while (reader.Read())
                {
                    int personid = reader.GetInt32(personIdIndex);
                    var startDate = reader.GetDateTime(startDateIndex);
                    var utilizationByProject = new UtilizationByProject()
                    {
                        Project = new Project()
                        {
                            Id = reader.IsDBNull(projectIdIndex) ? null : (int?)reader.GetInt32(projectIdIndex),
                            Name = reader.IsDBNull(projectNameIndex) ? string.Empty : reader.GetString(projectNameIndex),
                            ProjectNumber = reader.IsDBNull(projectNumberIndex) ? string.Empty : reader.GetString(projectNumberIndex)
                        },
                        ProjectedHours = reader.GetDecimal(projectedHoursIndex),
                        Utilization = reader.GetInt32(weeklyUtlizationIndex)
                    };

                    if (consultantUtilizationPersonList.Any(p => p.Person.Id == personid))
                    {
                        var person = consultantUtilizationPersonList.FirstOrDefault(p => p.Person.Id == personid);
                        person.ProjectedHours = person.ProjectedHours + reader.GetDecimal(projectedHoursIndex);
                        if (person.ProjectUtilization != null && person.ProjectUtilization.Any(p => p.StartDate.Date == startDate.Date))
                        {
                            var utilProject = person.ProjectUtilization.FirstOrDefault(p => p.StartDate.Date == startDate.Date);
                            utilProject.UtilizationProject.Add(utilizationByProject);
                        }
                        else
                        {
                            var utilProject = new ConsultantUtilzationByProject();
                            utilProject.StartDate = startDate;
                            utilProject.EndDate = reader.GetDateTime(endDateIndex);
                            utilProject.AvailableHours = reader.GetDecimal(availableHoursIndex);
                            person.AvailableHours = person.AvailableHours + reader.GetDecimal(availableHoursIndex);
                            utilProject.UtilizationProject = new List<UtilizationByProject>() { utilizationByProject };
                            if (person.ProjectUtilization == null)
                            {
                                person.ProjectUtilization = new List<ConsultantUtilzationByProject>() { utilProject };
                            }
                            else
                                person.ProjectUtilization.Add(utilProject);
                        }
                        person.WeeklyPayTypes.Add(reader.IsDBNull(payTypeIndex) ? 0 : reader.GetInt32(payTypeIndex));
                        person.WeeklyVacationDays.Add(reader.GetInt32(vacationDaysIndex));
                    }
                }
            }
        }

        private static void ReadPersonsEmploymentHistory(SqlDataReader reader, List<ConsultantUtilizationPerson> result)
        {
            if (result != null)
            {
                if (reader.HasRows)
                {
                    int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                    int hireDateIndex = reader.GetOrdinal(Constants.ColumnNames.HireDateColumn);
                    int terminationDateIndex = reader.GetOrdinal(Constants.ColumnNames.TerminationDateColumn);

                    while (reader.Read())
                    {
                        var employment = new Employment
                            {
                                PersonId = reader.GetInt32(personIdIndex),
                                HireDate = reader.GetDateTime(hireDateIndex),
                                TerminationDate =
                                    reader.IsDBNull(terminationDateIndex)
                                        ? null
                                        : (DateTime?)reader.GetDateTime(terminationDateIndex)
                            };

                        Person person = null;
                        if (result.Any(p => p.Person.Id == employment.PersonId))
                        {
                            person = result.First(p => p.Person.Id == employment.PersonId).Person;
                            if (person.EmploymentHistory == null)
                            {
                                person.EmploymentHistory = new List<Employment>();
                            }
                        }
                        if (person != null)
                        {
                            person.EmploymentHistory.Add(employment);
                        }
                    }
                }
            }
        }

        private static void ReadPersonTimeOffDates(SqlDataReader reader, List<ConsultantUtilizationPerson> result)
        {
            if (result != null)
            {
                if (reader.HasRows)
                {
                    int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                    int timeOffDateIndex = reader.GetOrdinal(Constants.ColumnNames.DateColumn);
                    int isTimeOffIndex = reader.GetOrdinal(Constants.ColumnNames.IsTimeOff);
                    int holidayDescriptionIndex = reader.GetOrdinal(Constants.ColumnNames.HolidayDescriptionColumn);
                    int timeOffHoursIndex = reader.GetOrdinal(Constants.ColumnNames.TimeOffHours);

                    while (reader.Read())
                    {
                        var personId = reader.GetInt32(personIdIndex);
                        var timeOffDate = reader.GetDateTime(timeOffDateIndex);
                        var isTimeOff = reader.GetInt32(isTimeOffIndex) == 1;
                        var timeOffHours = reader.GetDouble(timeOffHoursIndex);
                        string holidayDescription = reader.IsDBNull(holidayDescriptionIndex)
                                        ? string.Empty
                                        : reader.GetString(holidayDescriptionIndex);
                        if (result.Any(p => p.Person.Id == personId))
                        {
                            var record = result.First(p => p.Person.Id == personId);
                            if (record.TimeOffDates == null) record.TimeOffDates = new Dictionary<DateTime, double>();
                            if (record.CompanyHolidayDates == null) record.CompanyHolidayDates = new Dictionary<DateTime, string>();
                            if (isTimeOff)
                            {
                                record.TimeOffDates.Add(timeOffDate, timeOffHours);
                            }
                            else
                            {
                                record.CompanyHolidayDates.Add(timeOffDate, holidayDescription);
                            }
                        }
                    }
                }
            }
        }

        private static void ReadPersonAssignedDates(SqlDataReader reader, List<ConsultantUtilizationPerson> result)
        {
            if (result != null)
            {
                if (reader.HasRows)
                {
                    int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                    int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
                    int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
                    int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                    int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);

                    while (reader.Read())
                    {
                        var personId = reader.GetInt32(personIdIndex);
                        var projectNumber = reader.GetString(projectNumberIndex);
                        var milestonePerson = new Milestone()
                        {
                            StartDate = reader.GetDateTime(startDateIndex),
                            ProjectedDeliveryDate = reader.GetDateTime(endDateIndex)
                        };
                        if (result.Any(p => p.Person.Id == personId))
                        {
                            var record = result.First(p => p.Person.Id == personId);
                            if (record.Person.Projects == null)
                            {
                                record.Person.Projects = new List<Project>();
                                var project = new Project()
                                {
                                    Name = reader.GetString(projectNameIndex),
                                    ProjectNumber = reader.GetString(projectNumberIndex),
                                    Milestones = new List<Milestone>() { milestonePerson }
                                };
                                record.Person.Projects.Add(project);
                            }
                            else
                            {
                                if (record.Person.Projects.Any(p => p.ProjectNumber == projectNumber))
                                {
                                    var project = record.Person.Projects.First(p => p.ProjectNumber == projectNumber);
                                    if (project.Milestones == null)
                                    {
                                        project.Milestones = new List<Milestone>() { milestonePerson };
                                    }
                                    else
                                    {
                                        project.Milestones.Add(milestonePerson);
                                    }
                                }
                                else
                                {
                                    var project = new Project()
                                    {
                                        Name = reader.GetString(projectNameIndex),
                                        ProjectNumber = reader.GetString(projectNumberIndex),
                                        Milestones = new List<Milestone>() { milestonePerson }
                                    };
                                    record.Person.Projects.Add(project);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check's if there's compensation record covering milestone/
        /// See #886 for details.
        /// </summary>
        /// <param name="person">Person to check agains</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>True if there's such record, false otherwise</returns>
        public static bool IsCompensationCoversMilestone(Person person, DateTime? start, DateTime? end)
        {
            string query;
            bool extended = start.HasValue && end.HasValue;
            if (extended)
            {
                query = string.Format(
                    CompensationCoversExtendedMilestoneQuery,
                    CompensationCoversMilestoneFunction,
                    PersonIdParam,
                    MilestoneStartParam,
                    MilestoneEndParam);
            }
            else
            {
                query = string.Format(
                    CompensationCoversMilestoneQuery,
                    CompensationCoversMilestoneFunction,
                    PersonIdParam);
            }

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(query, connection))
            {
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam,
                                                person.Id.HasValue ? (object)person.Id.Value : DBNull.Value);

                if (extended)
                {
                    command.Parameters.AddWithValue(MilestoneStartParam, start.Value);
                    command.Parameters.AddWithValue(MilestoneEndParam, end.Value);
                }

                connection.Open();

                object result = command.ExecuteScalar();
                if (result == null)
                    return false;

                return (bool)result;
            }
        }

        /// <summary>
        /// Verifies whether a user has compensation at this moment
        /// </summary>
        /// <param name="personId">Id of the person</param>
        /// <returns>True if a person has active compensation, false otherwise</returns>
        public static bool CurrentPayExists(int personId)
        {
            var query = string.Format(
                    OneArgUdfFunctionQuery,
                    GetCurrentPayTypeFunction,
                    PersonIdParam);

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(query, connection))
            {
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam, personId);

                connection.Open();

                return !Convert.IsDBNull(command.ExecuteScalar());
            }
        }

        /// <summary>
        /// Gets all permissions for the given person
        /// </summary>
        /// <param name="person">Person to get permissions for</param>
        /// <returns>Object with the list of permissions</returns>
        public static PersonPermission GetPermissions(Person person)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Person.PermissionsGetAllProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(PersonIdParam,
                                                    person.Id.HasValue ? (object)person.Id.Value : DBNull.Value);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var permission = new PersonPermission();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    PermissionTarget pt =
                                        PersonPermission.ToEnum(reader[TargetTypeColumn]);

                                    if (Convert.IsDBNull(reader[TargetIdColumn]))
                                    {
                                        permission.AllowAll(pt);
                                    }
                                    else
                                    {
                                        permission.AddToList(pt, (int)reader[TargetIdColumn]);
                                    }
                                }
                                catch (Exception)
                                {
                                    throw new Exception(
                                        "PersonDAL.GetPermissions: unable to cast target type or Id.");
                                }
                            }
                        }

                        return permission;
                    }
                }
            }
        }

        /// <summary>
        /// Adds person to system
        /// </summary>
        /// <param name="person"><see cref="Person"/> to add</param>
        /// <param name="userName"></param>
        public static void PersonInsert(Person person, string userName, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (person.HireDate == DateTime.MinValue)
            {
                person.HireDate = DateTime.Now;
            }
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonInsertProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(FirstNameParam, !string.IsNullOrEmpty(person.FirstName) ? (object)person.FirstName : DBNull.Value);
                command.Parameters.AddWithValue(LastNameParam, !string.IsNullOrEmpty(person.LastName) ? (object)person.LastName : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PreferredFirstName, !string.IsNullOrEmpty(person.PrefferedFirstName) ? (object)person.PrefferedFirstName : DBNull.Value);
                command.Parameters.AddWithValue(HireDateParam, person.HireDate);
                command.Parameters.AddWithValue(AliasParam, !string.IsNullOrEmpty(person.Alias) ? (object)person.Alias : DBNull.Value);
                command.Parameters.AddWithValue(DefaultPracticeParam, person.DefaultPractice != null ? (object)person.DefaultPractice.Id : DBNull.Value);
                command.Parameters.AddWithValue(PersonStatusIdParam, person.Status != null ? (object)person.Status.Id : DBNull.Value);
                command.Parameters.AddWithValue(TerminationDateParam, person.TerminationDate.HasValue ? (object)person.TerminationDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(TeleponeNumberParam, !string.IsNullOrEmpty(person.TelephoneNumber) ? (object)person.TelephoneNumber : DBNull.Value);
                command.Parameters.AddWithValue(SeniorityIdParam, person.Seniority != null ? (object)person.Seniority.Id : DBNull.Value);
                command.Parameters.AddWithValue(TitleIdParam, person.Title != null ? (object)person.Title.TitleId : DBNull.Value);
                command.Parameters.AddWithValue(UserLoginParam, !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);
                command.Parameters.AddWithValue(IsOffshoreParam, person.IsOffshore);
                command.Parameters.AddWithValue(PaychexIDParam, !string.IsNullOrEmpty(person.PaychexID) ? (object)person.PaychexID : DBNull.Value);
                command.Parameters.AddWithValue(PersonDivisionIdParam, (int)person.DivisionType != 0 ? (object)((int)person.DivisionType) : DBNull.Value);
                command.Parameters.AddWithValue(TerminationReasonIdParam, person.TerminationReasonid.HasValue ? (object)person.TerminationReasonid.Value : DBNull.Value);
                command.Parameters.AddWithValue(RecruiterIdParam, person.RecruiterId.HasValue ? (object)person.RecruiterId.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.JobSeekerStatusId, person.JobSeekersStatus != JobSeekersStatus.Undefined ? (object)person.JobSeekersStatusId : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.SourceRecruitingMetricsId, person.SourceRecruitingMetrics != null ? (object)person.SourceRecruitingMetrics.RecruitingMetricsId : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.TargetRecruitingMetricsId, person.TargetedCompanyRecruitingMetrics != null ? (object)person.TargetedCompanyRecruitingMetrics.RecruitingMetricsId : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.EmployeeReferralId, person.EmployeeRefereral != null ? (object)person.EmployeeRefereral.Id.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.CohortAssignmentId, person.CohortAssignment.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.LocationId, person.Location.LocationId);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsMBO, person.IsMBO);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeLeadershipId, person.PracticeLeadership != null ? (object)person.PracticeLeadership.Id.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsInvestmentResource, person.IsInvestmentResource);
                command.Parameters.AddWithValue(Constants.ParameterNames.TargetUtilization, person.TargetUtilization != null ? (object)person.TargetUtilization : DBNull.Value);
                if (person.Manager != null)
                    command.Parameters.AddWithValue(Constants.ParameterNames.ManagerId, person.Manager.Id.Value);

                var personIdParameter = new SqlParameter(PersonIdParam, SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(personIdParameter);

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
                catch (Exception ex)
                {
                    throw ex;
                }
                person.Id = (int)personIdParameter.Value;
            }
        }

        public static void PersonValidations(Person person)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonValidationsProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(PersonIdParam, person.Id.HasValue ? (object)person.Id.Value : DBNull.Value);
                    command.Parameters.AddWithValue(FirstNameParam, !string.IsNullOrEmpty(person.FirstName) ? (object)person.FirstName : DBNull.Value);
                    command.Parameters.AddWithValue(LastNameParam, !string.IsNullOrEmpty(person.LastName) ? (object)person.LastName : DBNull.Value);
                    command.Parameters.AddWithValue(AliasParam, !string.IsNullOrEmpty(person.Alias) ? (object)person.Alias : DBNull.Value);
                    command.Parameters.AddWithValue(EmployeeNumberParam, !string.IsNullOrEmpty(person.EmployeeNumber) ? (object)person.EmployeeNumber : DBNull.Value);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Retrives <see cref="Opportunity"/> data to be exported to excel.
        /// </summary>
        /// <returns>An <see cref="Opportunity"/> object if found and null otherwise.</returns>
        public static DataSet PersonGetExcelSet()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonGetExcelSetProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();
                var adapter = new SqlDataAdapter(command);
                var dataset = new DataSet();
                adapter.Fill(dataset, "excelDataTable");
                return dataset;
            }
        }

        public static DataSet PersonGetExcelSetWithFilters(
           string practiceIdsSelected,
           string divisionIdsSelected,
           string looked,
           string recruiterIdsSelected,
           string timeScaleIdsSelected,
           bool Active,
           bool projected,
           bool terminated,
           bool terminatedPending
            )
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonReportWithFilters, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(PracticeIdsListParam, practiceIdsSelected != null ? (object)practiceIdsSelected : DBNull.Value);
                command.Parameters.AddWithValue(DivisionIdsListParam, divisionIdsSelected != null ? (object)divisionIdsSelected : DBNull.Value);
                command.Parameters.AddWithValue(LookedParam, !string.IsNullOrEmpty(looked) ? (object)looked : DBNull.Value);
                command.Parameters.AddWithValue(RecruiterIdsListParam, recruiterIdsSelected != null ? (object)recruiterIdsSelected : DBNull.Value);
                command.Parameters.AddWithValue(TimescaleIdsListParam, timeScaleIdsSelected != null ? (object)timeScaleIdsSelected : DBNull.Value);
                command.Parameters.AddWithValue(ActiveParam, Active);
                command.Parameters.AddWithValue(ProjectedParam, projected);
                command.Parameters.AddWithValue(TerminatedParam, terminated);
                command.Parameters.AddWithValue(TerminationPendingParam, terminatedPending);
                connection.Open();
                var adapter = new SqlDataAdapter(command);
                var dataset = new DataSet();
                adapter.Fill(dataset, "excelPersonDataTable");
                return dataset;
            }
        }

        /// <summary>
        /// Update person information
        /// </summary>
        /// <param name="person">contains new information to use</param>
        /// <param name="userName"></param>
        /// <remarks>
        /// <paramref name="person"/>.Id will identify
        /// </remarks>
        public static void PersonUpdate(Person person, string userName, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonUpdateProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam, person.Id.HasValue ? (object)person.Id.Value : DBNull.Value);
                command.Parameters.AddWithValue(FirstNameParam, !string.IsNullOrEmpty(person.FirstName) ? (object)person.FirstName : DBNull.Value);
                command.Parameters.AddWithValue(LastNameParam, !string.IsNullOrEmpty(person.LastName) ? (object)person.LastName : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PreferredFirstName, !string.IsNullOrEmpty(person.PrefferedFirstName) ? (object)person.PrefferedFirstName : DBNull.Value);
                command.Parameters.AddWithValue(HireDateParam, person.HireDate);
                command.Parameters.AddWithValue(TerminationDateParam, person.TerminationDate.HasValue ? (object)person.TerminationDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(AliasParam, !string.IsNullOrEmpty(person.Alias) ? (object)person.Alias : DBNull.Value);
                command.Parameters.AddWithValue(DefaultPracticeParam, person.DefaultPractice != null ? (object)person.DefaultPractice.Id : DBNull.Value);
                command.Parameters.AddWithValue(TeleponeNumberParam, !string.IsNullOrEmpty(person.TelephoneNumber) ? (object)person.TelephoneNumber : DBNull.Value);
                command.Parameters.AddWithValue(PersonStatusIdParam, person.Status != null ? (object)person.Status.Id : DBNull.Value);
                command.Parameters.AddWithValue(SeniorityIdParam, person.Seniority != null ? (object)person.Seniority.Id : DBNull.Value);
                command.Parameters.AddWithValue(UserLoginParam, !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);
                command.Parameters.AddWithValue(EmployeeNumberParam, !string.IsNullOrEmpty(person.EmployeeNumber) ? (object)person.EmployeeNumber : DBNull.Value);
                command.Parameters.AddWithValue(IsOffshoreParam, person.IsOffshore);
                command.Parameters.AddWithValue(PaychexIDParam, !string.IsNullOrEmpty(person.PaychexID) ? (object)person.PaychexID : DBNull.Value);
                command.Parameters.AddWithValue(PersonDivisionIdParam, (int)person.DivisionType != 0 ? (object)((int)person.DivisionType) : DBNull.Value);
                command.Parameters.AddWithValue(TerminationReasonIdParam, person.TerminationReasonid.HasValue ? (object)person.TerminationReasonid.Value : DBNull.Value);
                command.Parameters.AddWithValue(TitleIdParam, person.Title != null ? (object)person.Title.TitleId : DBNull.Value);
                command.Parameters.AddWithValue(RecruiterIdParam, person.RecruiterId.HasValue ? (object)person.RecruiterId.Value : DBNull.Value);
                command.Parameters.AddWithValue(SLTApprovalParam, person.SLTApproval);
                command.Parameters.AddWithValue(SLTPTOApprovalParam, person.SLTPTOApproval);
                command.Parameters.AddWithValue(Constants.ParameterNames.ManagerId, person.Manager != null ? (object)person.Manager.Id.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.JobSeekerStatusId, person.JobSeekersStatus != JobSeekersStatus.Undefined ? (object)person.JobSeekersStatusId : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.SourceRecruitingMetricsId, person.SourceRecruitingMetrics != null ? (object)person.SourceRecruitingMetrics.RecruitingMetricsId : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.TargetRecruitingMetricsId, person.TargetedCompanyRecruitingMetrics != null ? (object)person.TargetedCompanyRecruitingMetrics.RecruitingMetricsId : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.EmployeeReferralId, person.EmployeeRefereral != null ? (object)person.EmployeeRefereral.Id.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.CohortAssignmentId, person.CohortAssignment.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.LocationId, person.Location.LocationId);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsMBO, person.IsMBO);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsInvestmentResource, person.IsInvestmentResource);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeLeadershipId, person.PracticeLeadership != null ? (object)person.PracticeLeadership.Id.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.TargetUtilization, person.TargetUtilization != null ? (object)person.TargetUtilization : DBNull.Value);
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
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Lists all active persons who have recruiter role.
        /// </summary>
        /// <returns>The list of <see cref="Person"/> objects.</returns>
        public static List<Person> PersonListRecruiter()
        {
            var result = new List<Person>();

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonListRecruiterProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();

                ReadPersons(command, result);
            }

            return result;
        }

        /// <summary>
        /// List the persons who has Salesperson role
        /// </summary>
        /// <param name="person">Person to restrict permissions to</param>
        /// <param name="inactives">Determines whether inactive persons will are included into the results.</param>
        /// <returns>The list of the <see cref="Person"/> objects.</returns>
        public static List<Person> PersonListSalesperson(Person person, bool inactives)
        {
            var result = new List<Person>();

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonListSalespersonProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(IncludeInactiveParam, inactives);

                if (person != null)
                    command.Parameters.AddWithValue(PersonIdParam, person.Id);

                connection.Open();
                ReadPersons(command, result);

                return result;
            }
        }

        /// <summary>
        /// List the persons who have project manager permission
        /// </summary>
        /// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
        /// <param name="person"></param>
        /// <returns>
        /// The list of <see cref="Person"/> objects applicable to be a practice manager for the project.
        /// </returns>
        public static List<Person> PersonListProjectOwner(bool includeInactive, Person person)
        {
            var result = new List<Person>();

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonListProjectOwnerProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(IncludeInactiveParam, includeInactive);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId,
                    person == null ? (object)DBNull.Value : person.Id.Value);

                connection.Open();
                ReadPersons(command, result);

                return result;
            }
        }

        /// <summary>
        /// Retrieves a person record from the database.
        /// </summary>
        /// <param name="personId">An ID of the record.</param>
        /// <returns></returns>
        public static Person GetById(int personId)
        {
            var personList = new List<Person>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonGetByIdProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(PersonIdParam, personId);

                    connection.Open();

                    ReadPersons(command, personList);
                }
            }

            return personList.Count > 0 ? personList[0] : null;
        }

        public static List<Person> PersonListFiltered(
            int? practice,
            bool showAll,
            int pageSize,
            int pageNo,
            string looked,
            DateTime startDate,
            DateTime endDate,
            int? recruiterId,
            int? maxSeniorityLevel)
        {
            var personList = new List<Person>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonListAllSeniorityFilterProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(ShowAllParam, showAll);
                    command.Parameters.AddWithValue(PracticeIdParam,
                                                    practice.HasValue ? (object)practice.Value : DBNull.Value);
                    command.Parameters.AddWithValue(StartDateParam,
                                                    startDate != DateTime.MinValue ? (object)startDate : DBNull.Value);
                    command.Parameters.AddWithValue(EndDateParam,
                                                    endDate != DateTime.MinValue ? (object)endDate : DBNull.Value);
                    command.Parameters.AddWithValue(PageSizeParam,
                                                    pageSize > 0 && pageNo >= 0 ? (object)pageSize : DBNull.Value);
                    command.Parameters.AddWithValue(PageNoParam,
                                                    pageSize > 0 && pageNo >= 0 ? (object)pageNo : DBNull.Value);
                    command.Parameters.AddWithValue(LookedParam,
                                                    !string.IsNullOrEmpty(looked) ? (object)looked : DBNull.Value);
                    command.Parameters.AddWithValue(RecruiterIdParam,
                                                    recruiterId.HasValue ? (object)recruiterId.Value : DBNull.Value);
                    command.Parameters.AddWithValue(MaxSeniorityLevelParam,
                                                    maxSeniorityLevel.HasValue
                                                        ? (object)maxSeniorityLevel.Value
                                                        : DBNull.Value);

                    connection.Open();
                    ReadPersons(command, personList);
                }
            }
            return personList;
        }

        public static List<Person> PersonListFilteredWithCurrentPayByCommaSeparatedIdsList(
           string practiceIdsSelected,
           string divisionIdsSelected,
           bool showAll,
           int pageSize,
           int pageNo,
           string looked,
           DateTime startDate,
           DateTime endDate,
           string recruiterIdsSelected,
           int? maxSeniorityLevel,
           string sortBy,
           string timeScaleIdsSelected,
           bool projected,
           bool terminated,
           bool terminatedPending,
           char? alphabet)
        {
            var personList = new List<Person>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonListAllSeniorityFilterWithPayByCommaSeparatedIdsListProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(ShowAllParam, showAll);
                    command.Parameters.AddWithValue(PracticeIdsListParam, practiceIdsSelected != null ? (object)practiceIdsSelected : DBNull.Value);
                    command.Parameters.AddWithValue(DivisionIdsListParam, divisionIdsSelected != null ? (object)divisionIdsSelected : DBNull.Value);
                    command.Parameters.AddWithValue(StartDateParam, startDate != DateTime.MinValue ? (object)startDate : DBNull.Value);
                    command.Parameters.AddWithValue(EndDateParam, endDate != DateTime.MinValue ? (object)endDate : DBNull.Value);
                    command.Parameters.AddWithValue(PageSizeParam, pageSize > 0 && pageNo >= 0 ? (object)pageSize : DBNull.Value);
                    command.Parameters.AddWithValue(PageNoParam, pageSize > 0 && pageNo >= 0 ? (object)pageNo : DBNull.Value);
                    command.Parameters.AddWithValue(LookedParam, !string.IsNullOrEmpty(looked) ? (object)looked : DBNull.Value);
                    command.Parameters.AddWithValue(RecruiterIdsListParam, recruiterIdsSelected != null ? (object)recruiterIdsSelected : DBNull.Value);
                    command.Parameters.AddWithValue(MaxSeniorityLevelParam, maxSeniorityLevel.HasValue ? (object)maxSeniorityLevel.Value : DBNull.Value);
                    if (!string.IsNullOrEmpty(sortBy))
                    {
                        command.Parameters.AddWithValue(SortByParam, sortBy);
                    }
                    command.Parameters.AddWithValue(TimescaleIdsListParam, timeScaleIdsSelected != null ? (object)timeScaleIdsSelected : DBNull.Value);
                    command.Parameters.AddWithValue(ProjectedParam, projected);
                    command.Parameters.AddWithValue(TerminatedParam, terminated);
                    command.Parameters.AddWithValue(TerminationPendingParam, terminatedPending);
                    command.Parameters.AddWithValue(AlphabetParam, alphabet.HasValue ? (object)alphabet.Value : DBNull.Value);

                    connection.Open();
                    ReadPersonsWithCurrentPay(command, personList);
                }
            }
            return personList;
        }

        /// <summary>
        /// Retrives a short info on persons.
        /// </summary>
        /// <param name="practice">Practice filter, null meaning all practices</param>
        /// <param name="statusIds">Person status id</param>
        /// <param name="startDate">Determines a start date when persons in the list must are available.</param>
        /// <param name="endDate">Determines an end date when persons in the list must are available.</param>
        /// <returns>A list of the <see cref="Person"/> objects.</returns>
        public static List<Person> PersonListAllShort(int? practice, string statusIds, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonListAllShortProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PracticeIdParam,
                                                practice.HasValue ? (object)practice.Value : DBNull.Value);
                command.Parameters.AddWithValue(StartDateParam,
                                                startDate > DateTime.MinValue ? (object)startDate : DBNull.Value);
                command.Parameters.AddWithValue(EndDateParam,
                                                endDate > DateTime.MinValue ? (object)endDate : DBNull.Value);
                command.Parameters.AddWithValue(PersonStatusIdsListParam,
                                                !string.IsNullOrEmpty(statusIds) ? (object)statusIds : DBNull.Value);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();
                    ReadPersonsShort(reader, result);
                    return result;
                }
            }
        }

        /// <summary>
        /// Retrives a short info on persons.
        /// </summary>
        /// <param name="statusList">Comma seperated statusIds</param>
        /// <param name="personId">personId</param>
        /// <returns></returns>
        public static List<Person> GetPersonListByStatusList(string statusList, int? personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonListByStatusListProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                if (!string.IsNullOrEmpty(statusList))
                {
                    command.Parameters.AddWithValue(PersonStatusIdsListParam, statusList);
                }

                if (personId.HasValue)
                {
                    command.Parameters.AddWithValue(PersonIdParam, personId.Value);
                }
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();
                    ReadPersonsShort(reader, result);
                    return result;
                }
            }
        }

        /// <summary>
        /// Retrives a short info on persons who are not in the Administration practice.
        /// </summary>
        /// <param name="milestonePersonId">An ID of existing milestone-person association or null.</param>
        /// <param name="startDate">Determines a start date when persons in the list must are available.</param>
        /// <param name="endDate">Determines an end date when persons in the list must are available.</param>
        /// <returns>A list of the <see cref="Person"/> objects.</returns>
        public static List<Person> PersonListAllForMilestone(int? milestonePersonId, DateTime startDate,
                                                             DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonListAllForMilestoneProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(MilestonePersonIdParam,
                                                milestonePersonId.HasValue
                                                    ? (object)milestonePersonId.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(StartDateParam,
                                                startDate > DateTime.MinValue ? (object)startDate : DBNull.Value);
                command.Parameters.AddWithValue(EndDateParam,
                                                endDate > DateTime.MinValue ? (object)endDate : DBNull.Value);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();
                    ReadPersonsShort(reader, result);
                    return result;
                }
            }
        }

        /// <summary>
        /// Overrides PersonListFiltered but without last 'looked' parameter
        /// </summary>
        public static List<Person> PersonListFiltered(int? practice, bool showAll, int pageSize, int pageNo,
                                                      int? recruiterId, int? maxSeniorityLevel)
        {
            return
                PersonListFiltered(
                    practice,
                    showAll,
                    pageSize,
                    pageNo,
                    string.Empty,
                    DateTime.MinValue,
                    DateTime.MinValue,
                    recruiterId,
                    maxSeniorityLevel);
        }

        /// <summary>
        /// Retrieves the list of <see cref="Person"/>s who have some bench time.
        /// </summary>
        /// <returns>The list of the <see cref="Person"/> objects.</returns>
        public static List<Person> PersonListBenchExpense(BenchReportContext context)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonListBenchExpenseProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.Start, context.Start);
                command.Parameters.AddWithValue(Constants.ParameterNames.End, context.End);
                if (context.ActivePersons.HasValue)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.ActivePersons, context.ActivePersons.Value);
                }
                command.Parameters.AddWithValue(Constants.ParameterNames.ActiveProjects, context.ActiveProjects);
                if (context.ProjectedPersons.HasValue)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedPersons, context.ProjectedPersons.Value);
                }
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedProjects, context.ProjectedProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExperimentalProjects, context.ExperimentalProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProposedProjects, context.ProposedProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.CompletedProjects, context.CompletedProjects);
                if (context.IncludeOverheads.HasValue)
                    command.Parameters.AddWithValue(Constants.ParameterNames.IncludeOverheads, context.IncludeOverheads.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.IncludeZeroCostEmployees, context.IncludeZeroCostEmployees);
                if (context.PracticeIds != null)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, context.PracticeIds);
                }

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();
                    ReadPersonExpense(reader, result);
                    return result;
                }
            }
        }

        /// <summary>
        /// Retrieves person one-off list.
        /// </summary>
        /// <returns>The list of the <see cref="Person"/> objects.</returns>
        public static List<Person> PersonOneOffList(DateTime today)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonOneOffListProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(DateTodayParam, today);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();
                    ReadPersonsIdFirstNameLastNameAndTitle(reader, result);
                    return result;
                }
            }
        }

        private static void ReadPersonsIdFirstNameLastNameAndTitle(SqlDataReader reader, List<Person> result)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(PersonIdColumn);
            int firstNameIndex = reader.GetOrdinal(FirstNameColumn);
            int lastNameIndex = reader.GetOrdinal(LastNameColumn);
            int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
            int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);

            while (reader.Read())
            {
                var personId = reader.GetInt32(personIdIndex);
                var person = new Person
                {
                    Id = personId,
                    FirstName = reader.GetString(firstNameIndex),
                    LastName = reader.GetString(lastNameIndex),
                    Title = new Title
                    {
                        TitleId = reader.GetInt32(titleIdIndex),
                        TitleName = reader.GetString(titleIndex)
                    },
                };

                result.Add(person);
            }
        }

        /// <summary>
        /// Retrives the Person by the Alias (email)
        /// </summary>
        /// <param name="alias">A person's email</param>
        /// <returns>The <see cref="Person"/> object if found and null otherwise.</returns>
        public static Person PersonGetByAlias(string alias)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonGetByAliasProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(AliasParam,
                                                !string.IsNullOrEmpty(alias) ? (object)alias : DBNull.Value);

                connection.Open();

                var result = new List<Person>(1);
                ReadPersons(command, result);

                return result.Count > 0 ? result[0] : null;
            }
        }

        private static void ReadPersonExpense(DbDataReader reader, List<Person> result)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(PersonIdColumn);
            int lastNameIndex = reader.GetOrdinal(LastNameColumn);
            int firstNameIndex = reader.GetOrdinal(FirstNameColumn);
            int hireDateIndex = reader.GetOrdinal(HireDateColumn);
            int terminationDateIndex = reader.GetOrdinal(TerminationDateColumn);
            int monthIndex = reader.GetOrdinal(MonthColumn);
            int revenueIndex = reader.GetOrdinal(RevenueColumn);
            int cogsIndex = reader.GetOrdinal(CogsColumn);
            int marginIndex = reader.GetOrdinal(MarginColumn);
            int personStatusIdIndex = reader.GetOrdinal(PersonStatusIdColumn);
            int personStatusNameIndex = reader.GetOrdinal(PersonStatusNameColumn);
            //int benchStartDateColumnIndex = reader.GetOrdinal(BenchStartDateColumn);
            int seniorityIdIndex = reader.GetOrdinal(SeniorityIdColumn);
            //int availableFromIndex = reader.GetOrdinal(AvailableFromColumn);
            int timescaleIndex = reader.GetOrdinal(TimescaleColumn);
            int practiceNameIndex = reader.GetOrdinal(PracticeNameColumn);
            int IsCompanyInternalIndex = reader.GetOrdinal(Constants.ParameterNames.IsCompanyInternal);
            int timeScaleChangeStatusIndex = reader.GetOrdinal(TimeScaleChangeStatusColumn);

            int? currentId = null;
            while (reader.Read())
            {
                int tmpId = reader.GetInt32(personIdIndex);
                Person person;

                if (!currentId.HasValue || currentId.Value != tmpId)
                {
                    person = new Person();
                    result.Add(person);
                }
                else
                {
                    person = result[result.Count - 1];
                }
                currentId = tmpId;

                person.Id = tmpId;
                person.LastName = reader.GetString(lastNameIndex);
                person.FirstName = reader.GetString(firstNameIndex);
                person.HireDate = reader.GetDateTime(hireDateIndex);
                person.TerminationDate = reader.GetDateTime(terminationDateIndex);
                person.DefaultPractice = new Practice()
                {
                    Name = reader.GetString(practiceNameIndex),
                    IsCompanyInternal = reader.GetBoolean(IsCompanyInternalIndex)
                };

                if (person.ProjectedFinancialsByMonth == null)
                {
                    person.ProjectedFinancialsByMonth = new Dictionary<DateTime, ComputedFinancials>();
                }

                DateTime month = reader.GetDateTime(monthIndex);
                var financials = new ComputedFinancials
                {
                    Revenue = reader.GetDecimal(revenueIndex),
                    Cogs = reader.GetDecimal(cogsIndex),
                    GrossMargin = reader.GetDecimal(marginIndex),
                    Timescale = (TimescaleType)reader.GetInt32(timescaleIndex),
                    TimescaleChangeStatus = reader.GetInt32(timeScaleChangeStatusIndex)
                };

                person.Status = new PersonStatus
                {
                    Id = reader.GetInt32(personStatusIdIndex),
                    Name = reader.GetString(personStatusNameIndex)
                };

                person.Seniority =
                    !reader.IsDBNull(seniorityIdIndex)
                        ?
                        new Seniority { Id = reader.GetInt32(seniorityIdIndex) }
                        : null;

                person.ProjectedFinancialsByMonth.Add(month, financials);
            }
        }

        /// <summary>
        /// Retrives a list of overheads for the specified person.
        /// </summary>
        /// <param name="personId">An ID of the person to retrieve the data for.</param>
        /// <returns>The list of the <see cref="PersonOverhead"/> objects.</returns>
        public static List<PersonOverhead> PersonOverheadListByPerson(int personId, DateTime? effectiveDate = null)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonOverheadByPersonProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam, personId);
                command.Parameters.AddWithValue(EffectiveDateParam, (effectiveDate.HasValue) ? (object)effectiveDate.Value : DBNull.Value);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<PersonOverhead>();

                    ReadPersonOverheads(reader, result);

                    return result;
                }
            }
        }

        /// <summary>
        /// Retrives a list of overheads for the specified <see cref="Timescale"/>.
        /// </summary>
        /// <param name="timescale">The <see cref="Timescale"/> to retrive the data for.</param>
        /// <returns>The list of the <see cref="PersonOverhead"/> objects.</returns>
        public static List<PersonOverhead> PersonOverheadListByTimescale(TimescaleType timescale, DateTime? effectiveDate = null)
        {
            // Because % of Revenue type has the same list of overheads,
            //  exctract them as if it was 1099
            if (timescale == TimescaleType.PercRevenue)
                timescale = TimescaleType._1099Ctc;

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonOverheadByTimescaleProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(TimescaleIdParam, timescale);
                command.Parameters.AddWithValue(EffectiveDateParam, (effectiveDate.HasValue) ? (object)effectiveDate.Value : DBNull.Value);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<PersonOverhead>();

                    ReadPersonOverheads(reader, result);

                    return result;
                }
            }
        }

        /// <summary>
        /// Deletes the user in aspnet_users.
        /// </summary>
        /// <param name="userName">An ID of the user to the membership info be deleted for.</param>
        /// <param name="connection"></param>
        /// <param name="activeTransaction"></param>
        public static void DeleteUser(string userName, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.aspnetUsersDeleteUserProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(AppicationNameParam, "PracticeManagement");
                command.Parameters.AddWithValue(UserNameParam, userName);
                command.Parameters.AddWithValue(TablesToDeleteFromParam, 15);
                var NumTablesDeletedFrom = new SqlParameter(NumTablesDeletedFromParam, SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(NumTablesDeletedFrom);

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
        }

        /// <summary>
        /// Deletes the user in aspnet_users.
        /// </summary>
        /// <param name="userId">An ID of the user to the membership info be deleted for.</param>
        public static void Createuser(string userName, string password, string salt, string email, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.aspnetMembershipCreateUserProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(AppicationNameParam, "PracticeManagement");
                command.Parameters.AddWithValue(UserNameParam, userName);
                command.Parameters.AddWithValue(PasswordParam, password);
                command.Parameters.AddWithValue(PasswordSaltParam, salt);
                command.Parameters.AddWithValue(EmailParam, email);
                command.Parameters.AddWithValue(PasswordQuestionParam, DBNull.Value);
                command.Parameters.AddWithValue(PasswordAnswerParam, DBNull.Value);
                command.Parameters.AddWithValue(IsApprovedParam, true);
                command.Parameters.AddWithValue(UniqueEmailParam, 0);
                command.Parameters.AddWithValue(PasswordFormatParam, 1);
                command.Parameters.AddWithValue(CurrentTimeUtcParam, DateTime.UtcNow);
                command.Parameters.AddWithValue(UserIdParam, Guid.NewGuid());

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
        }

        /// <summary>
        /// Deletes the membership info for the specified user.
        /// </summary>
        /// <param name="userId">An ID of the user to the membership info be deleted for.</param>
        public static void MembershipAliasUpdate(string oldAlias, string newAlias, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.MembershipAliasUpdateProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(OldAliasParam, oldAlias);
                command.Parameters.AddWithValue(NewAliasParam, newAlias);

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
        }

        /// <summary>
        /// Deletes the membership info for the specified user.
        /// </summary>
        /// <param name="userId">An ID of the user to the membership info be deleted for.</param>
        public static void MembershipDelete(Guid userId, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.MembershipDeleteProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(UserIdParam, userId);

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
        }

        /// <summary>
        /// Sets permissions for user
        /// </summary>
        /// <param name="person">Person to set permissions for</param>
        /// <param name="permissions">Permissions to set for the user</param>
        public static void SetPermissionsForPerson(Person person, PersonPermission permissions)
        {
            //  person.Id can be null when adding new person
            //      In this case we're not adding any information about permissions
            //      By default nothing is allowed
            if (person.Id == null) return;
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PermissionsSetAllProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam, person.Id);

                command.Parameters.AddWithValue(ClientIdsListParam,
                                                (object)
                                                permissions.GetPermissionsAsStringList(PermissionTarget.Client) ??
                                                DBNull.Value);
                command.Parameters.AddWithValue(GroupIdsListParam,
                                                (object)
                                                permissions.GetPermissionsAsStringList(PermissionTarget.Group) ??
                                                DBNull.Value);
                command.Parameters.AddWithValue(SalespersonIdsListParam,
                                                (object)
                                                permissions.GetPermissionsAsStringList(PermissionTarget.Salesperson) ??
                                                DBNull.Value);
                command.Parameters.AddWithValue(PracticeManagerIdsListParam,
                                                (object)
                                                permissions.GetPermissionsAsStringList(
                                                    PermissionTarget.PracticeManager) ?? DBNull.Value);
                command.Parameters.AddWithValue(PracticeIdsListParam,
                                                (object)
                                                permissions.GetPermissionsAsStringList(PermissionTarget.Practice) ??
                                                DBNull.Value);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new DataAccessException(ex.Message, ex);
                }
            }
        }

        private static void ReadPersonOverheads(DbDataReader reader, List<PersonOverhead> result)
        {
            if (!reader.HasRows) return;
            int descriptionIndex = reader.GetOrdinal(DescriptionColumn);
            int rateIndex = reader.GetOrdinal(RateColumn);
            int hoursToCollectIndex = reader.GetOrdinal(HoursToCollectColumn);
            int startDateIndex = reader.GetOrdinal(StartDateColumn);
            int endDateIndex = reader.GetOrdinal(EndDateColumn);
            int isPercentageIndex = reader.GetOrdinal(IsPercentageColumn);
            int overheadRateTypeIdIndex = reader.GetOrdinal(OverheadRateTypeIdColumn);
            int overheadRateTypeNameIndex = reader.GetOrdinal(OverheadRateTypeNameColumn);
            int billRateMultiplierIndex = reader.GetOrdinal(BillRateMultiplierColumn);

            while (reader.Read())
            {
                var overhead = new PersonOverhead
                {
                    Name =
                        !reader.IsDBNull(descriptionIndex)
                            ? reader.GetString(descriptionIndex)
                            : null,
                    Rate = reader.GetDecimal(rateIndex),
                    HoursToCollect =
                        !reader.IsDBNull(hoursToCollectIndex)
                            ? reader.GetInt32(hoursToCollectIndex)
                            : 0,
                    StartDate =
                        !reader.IsDBNull(startDateIndex)
                            ? reader.GetDateTime(startDateIndex)
                            : DateTime.MinValue,
                    EndDate =
                        !reader.IsDBNull(endDateIndex)
                            ? (DateTime?)reader.GetDateTime(endDateIndex)
                            : null,
                    IsPercentage = reader.GetBoolean(isPercentageIndex)
                };

                if (!reader.IsDBNull(overheadRateTypeIdIndex))
                {
                    overhead.RateType = new OverheadRateType
                    {
                        Id = reader.GetInt32(overheadRateTypeIdIndex),
                        Name = reader.GetString(overheadRateTypeNameIndex)
                    };
                }

                overhead.BillRateMultiplier = reader.GetDecimal(billRateMultiplierIndex);

                try
                {
                    var IsMinimumLoadFactorIndx = reader.GetOrdinal(IsMinimumLoadFactorColumn);
                    overhead.IsMLF = reader.GetBoolean(IsMinimumLoadFactorIndx);
                }
                catch
                {
                }

                result.Add(overhead);
            }
        }

        /// <summary>
        /// reads a dataset of persons into a collection
        /// </summary>
        /// <param name="command"></param>
        /// <param name="personList"></param>
        private static void ReadPersons(SqlCommand command, ICollection<Person> personList)
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (!reader.HasRows) return;
                int aliasIndex = reader.GetOrdinal(AliasColumn);
                int personStatusIdIndex = reader.GetOrdinal(PersonStatusIdColumn);
                int personStatusNameIndex = reader.GetOrdinal(PersonStatusNameColumn);
                int seniorityIdIndex = reader.GetOrdinal(SeniorityIdColumn);
                int seniorityNameIndex = reader.GetOrdinal(SeniorityNameColumn);
                int managerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ManagerId);
                int managerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.ManagerFirstName);
                int managerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.ManagerLastName);
                int managerAliasIndex = -1;
                int telephoneNumberIndex = reader.GetOrdinal(Constants.ColumnNames.TelephoneNumber);
                int isDefManagerIndex;
                int IsWelcomeEmailSentIndex;
                int isStrawManIndex;
                int isOffshoreIndex;
                int paychexIDIndex;
                int divisionIdIndex;
                int terminationReasonIdIndex = -1;
                int titleIdIndex = -1;
                int recruterIdIndex = -1;
                int titleIndex = -1;
                int jobSeekerStatusIdIndex = -1;
                int sourceIdIndex = -1;
                int targetedCompanyIndex = -1;
                int employeeReferalIdIndex = -1;
                int employeeReferalFirstNameIndex = -1;
                int employeeReferalLastNameIndex = -1;
                int cohortAssignmentIdIndex = -1;
                int cohortAssignmentNameIndex = -1;
                int prefferedFirstNameIndex = -1;
                int locationIdIndex = -1;
                int locationCodeIndex = -1;
                int locationNameIndex = -1;
                int practiceLeadershipIdIndex = -1;
                int isMbo = -1;
                int isInvestmentResourceIndex = -1;
                int targetUtilizationIndex = -1;
                try
                {
                    isInvestmentResourceIndex = reader.GetOrdinal(Constants.ColumnNames.IsInvestmentResource);
                }
                catch
                { }
                try
                {
                    targetUtilizationIndex = reader.GetOrdinal(Constants.ColumnNames.TargetUtilization);
                }
                catch { }
                try
                {
                    practiceLeadershipIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeLeadershipId);
                }
                catch
                { }
                try
                {
                    isMbo = reader.GetOrdinal(Constants.ColumnNames.IsMBO);
                }
                catch
                { }
                try
                {
                    locationIdIndex = reader.GetOrdinal(Constants.ColumnNames.LocationId);
                    locationCodeIndex = reader.GetOrdinal(Constants.ColumnNames.LocationCode);
                    locationNameIndex = reader.GetOrdinal(Constants.ColumnNames.LocationName);
                }
                catch
                { }
                try
                {
                    prefferedFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.PreferredFirstName);
                }
                catch
                { }
                try
                {
                    cohortAssignmentIdIndex = reader.GetOrdinal(Constants.ColumnNames.CohortAssignmentId);
                    cohortAssignmentNameIndex = reader.GetOrdinal(Constants.ColumnNames.CohortAssignmentName);
                }
                catch
                { }

                try
                {
                    jobSeekerStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.JobSeekerStatusId);
                }
                catch
                { }

                try
                {
                    sourceIdIndex = reader.GetOrdinal(Constants.ColumnNames.SourceId);
                }
                catch
                { }

                try
                {
                    targetedCompanyIndex = reader.GetOrdinal(Constants.ColumnNames.TargetedCompanyId);
                }
                catch
                { }

                try
                {
                    employeeReferalIdIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeReferralId);
                    employeeReferalFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeReferralFirstName);
                    employeeReferalLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeReferralLastName);
                }
                catch
                { }

                try
                {
                    titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                    titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                }
                catch
                { }

                try
                {
                    recruterIdIndex = reader.GetOrdinal(Constants.ColumnNames.RecruiterIdColumn);
                }
                catch
                { }

                try
                {
                    terminationReasonIdIndex = reader.GetOrdinal(TerminationReasonIdColumn);
                }
                catch
                { }

                try
                {
                    isOffshoreIndex = reader.GetOrdinal(Constants.ColumnNames.IsOffshore);
                }
                catch
                {
                    isOffshoreIndex = -1;
                }

                try
                {
                    paychexIDIndex = reader.GetOrdinal(Constants.ColumnNames.PaychexID);
                }
                catch
                {
                    paychexIDIndex = -1;
                }

                try
                {
                    isStrawManIndex = reader.GetOrdinal(Constants.ColumnNames.IsStrawmanColumn);
                }
                catch
                {
                    isStrawManIndex = -1;
                }

                try
                {
                    managerAliasIndex = reader.GetOrdinal(Constants.ColumnNames.ManagerAlias);
                }
                catch
                {
                    managerAliasIndex = -1;
                }

                try
                {
                    IsWelcomeEmailSentIndex = reader.GetOrdinal(Constants.ColumnNames.IsWelcomeEmailSent);
                }
                catch
                {
                    IsWelcomeEmailSentIndex = -1;
                }

                try
                {
                    isDefManagerIndex = reader.GetOrdinal(Constants.ColumnNames.IsDefaultManager);
                }
                catch
                {
                    isDefManagerIndex = -1;
                }

                try
                {
                    divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);
                }
                catch
                {
                    divisionIdIndex = -1;
                }

                //  PracticesOwned column is not defined for each set that
                //  uses given method, so we need to know if that column exists
                int practicesOwnedIndex;
                try
                {
                    practicesOwnedIndex = reader.GetOrdinal(Constants.ColumnNames.PracticesOwned);
                }
                catch
                {
                    practicesOwnedIndex = -1;
                }

                while (reader.Read())
                {
                    var person = new Person
                    {
                        Id = (int)reader[PersonIdColumn],
                        FirstName = (string)reader[FirstNameColumn],
                        LastName = (string)reader[LastNameColumn],
                        Alias =
                            !reader.IsDBNull(aliasIndex)
                                ? reader.GetString(aliasIndex)
                                : string.Empty,
                        HireDate = (DateTime)reader[HireDateColumn],
                        TelephoneNumber = !reader.IsDBNull(telephoneNumberIndex) ? reader.GetString(telephoneNumberIndex) : string.Empty
                    };

                    if (IsWelcomeEmailSentIndex > -1)
                    {
                        person.IsWelcomeEmailSent = reader.GetBoolean(IsWelcomeEmailSentIndex);
                    }
                    if (isInvestmentResourceIndex > -1)
                    {
                        person.IsInvestmentResource = reader.GetBoolean(isInvestmentResourceIndex);
                    }
                    if (targetUtilizationIndex > -1)
                    {
                        person.TargetUtilization = reader.IsDBNull(targetUtilizationIndex) ? null : (int?)reader.GetInt32(targetUtilizationIndex);
                    }
                    if (isDefManagerIndex > -1)
                    {
                        person.IsDefaultManager = reader.GetBoolean(isDefManagerIndex);
                    }

                    if (Convert.IsDBNull(reader[TerminationDateColumn]))
                    {
                        person.TerminationDate = null;
                    }
                    else
                    {
                        person.TerminationDate = (DateTime)reader[TerminationDateColumn];
                    }

                    if (terminationReasonIdIndex != -1)
                    {
                        person.TerminationReasonid = reader.IsDBNull(terminationReasonIdIndex) ? null : (int?)reader.GetInt32(terminationReasonIdIndex);
                    }

                    if (!Convert.IsDBNull(reader["DefaultPractice"]))
                    {
                        person.DefaultPractice = new Practice
                        {
                            Id = (int)reader["DefaultPractice"],
                            Name = (string)reader["PracticeName"]
                        };
                    }
                    if (!Convert.IsDBNull(reader[EmployeeNumberColumn]))
                    {
                        person.EmployeeNumber = (string)reader[EmployeeNumberColumn];
                    }

                    if (recruterIdIndex > -1 && !reader.IsDBNull(recruterIdIndex))
                    {
                        person.RecruiterId = reader.GetInt32(recruterIdIndex);
                    }

                    if (titleIdIndex > -1 && !reader.IsDBNull(titleIdIndex))
                    {
                        person.Title = new Title() { TitleId = reader.GetInt32(titleIdIndex), TitleName = reader.GetString(titleIndex) };
                    }

                    person.Status = new PersonStatus
                    {
                        Id = reader.GetInt32(personStatusIdIndex),
                        Name = reader.GetString(personStatusNameIndex)
                    };

                    if (!reader.IsDBNull(seniorityIdIndex))
                    {
                        person.Seniority =
                            new Seniority
                            {
                                Id = reader.GetInt32(seniorityIdIndex),
                                Name = reader.GetString(seniorityNameIndex)
                            };
                    }

                    if (isStrawManIndex > -1)
                    {
                        person.IsStrawMan = reader.GetBoolean(isStrawManIndex);
                    }

                    if (isOffshoreIndex > -1)
                    {
                        person.IsOffshore = reader.GetBoolean(isOffshoreIndex);
                    }

                    if (paychexIDIndex > -1)
                    {
                        person.PaychexID = reader.IsDBNull(paychexIDIndex) ? null : reader.GetString(paychexIDIndex);
                    }

                    if (divisionIdIndex > -1)
                    {
                        person.DivisionType = reader.IsDBNull(divisionIdIndex) ? (PersonDivisionType)Enum.Parse(typeof(PersonDivisionType), "0") : (PersonDivisionType)Enum.Parse(typeof(PersonDivisionType), reader.GetInt32(divisionIdIndex).ToString());
                    }

                    if (practicesOwnedIndex >= 0 && !reader.IsDBNull(practicesOwnedIndex))
                    {
                        person.PracticesOwned = new List<Practice>();

                        var xdoc = new XmlDocument();
                        var xmlPractices = reader.GetString(practicesOwnedIndex);
                        xdoc.LoadXml(xmlPractices);

                        foreach (XmlNode xpractice in xdoc.FirstChild.ChildNodes)
                        {
                            if (xpractice.Attributes != null)
                                person.PracticesOwned.Add(
                                    new Practice
                                    {
                                        Id = Convert.ToInt32(xpractice.Attributes["Id"].Value),
                                        Name = xpractice.Attributes["Name"].Value
                                    });
                        }
                    }

                    if (!Convert.IsDBNull(reader[managerIdIndex]))
                    {
                        person.Manager = new Person
                        {
                            Id = reader.GetInt32(managerIdIndex),
                            FirstName = reader.GetString(managerFirstNameIndex),
                            LastName = reader.GetString(managerLastNameIndex)
                        };

                        if (managerAliasIndex >= 0)
                        {
                            person.Manager.Alias = reader.GetString(managerAliasIndex);
                        }
                    }

                    if (jobSeekerStatusIdIndex > -1 && !reader.IsDBNull(jobSeekerStatusIdIndex))
                    {
                        person.JobSeekersStatusId = !reader.IsDBNull(jobSeekerStatusIdIndex) ? reader.GetInt32(jobSeekerStatusIdIndex) : 0;
                    }

                    if (sourceIdIndex > -1 && !reader.IsDBNull(sourceIdIndex))
                    {
                        person.SourceRecruitingMetrics = new RecruitingMetrics() { RecruitingMetricsId = reader.GetInt32(sourceIdIndex) };
                    }

                    if (prefferedFirstNameIndex > -1)
                    {
                        person.PrefferedFirstName = !reader.IsDBNull(prefferedFirstNameIndex) ? reader.GetString(prefferedFirstNameIndex) : string.Empty;
                    }

                    if (targetedCompanyIndex > -1 && !reader.IsDBNull(targetedCompanyIndex))
                    {
                        person.TargetedCompanyRecruitingMetrics = new RecruitingMetrics() { RecruitingMetricsId = reader.GetInt32(targetedCompanyIndex) };
                    }

                    if (employeeReferalIdIndex > -1 && !reader.IsDBNull(employeeReferalIdIndex))
                    {
                        person.EmployeeRefereral = new Person()
                        {
                            Id = reader.GetInt32(employeeReferalIdIndex),
                            FirstName = reader.GetString(employeeReferalFirstNameIndex),
                            LastName = reader.GetString(employeeReferalLastNameIndex)
                        };
                    }

                    if (cohortAssignmentIdIndex > -1 && !reader.IsDBNull(cohortAssignmentIdIndex))
                    {
                        person.CohortAssignment = new CohortAssignment()
                        {
                            Id = reader.GetInt32(cohortAssignmentIdIndex),
                            Name = reader.GetString(cohortAssignmentNameIndex)
                        };
                    }

                    if (locationIdIndex > -1 && !reader.IsDBNull(locationIdIndex))
                    {
                        person.Location = new Location()
                        {
                            LocationId = reader.GetInt32(locationIdIndex),
                            LocationName = reader.GetString(locationNameIndex),
                            LocationCode = reader.GetString(locationCodeIndex)
                        };
                    }
                    if (practiceLeadershipIdIndex > -1 && !reader.IsDBNull(practiceLeadershipIdIndex))
                    {
                        person.PracticeLeadership = new Person()
                        {
                            Id = reader.GetInt32(practiceLeadershipIdIndex)
                        };
                    }
                    if (isMbo > -1)
                    {
                        person.IsMBO = reader.GetBoolean(isMbo);
                    }
                    personList.Add(person);
                }
            }
        }

        private static void ReadPersonsWithCurrentPay(SqlCommand command, ICollection<Person> personList)
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (!reader.HasRows) return;
                int aliasIndex = reader.GetOrdinal(AliasColumn);
                int personStatusIdIndex = reader.GetOrdinal(PersonStatusIdColumn);
                int personStatusNameIndex = reader.GetOrdinal(PersonStatusNameColumn);
                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int titleNameIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                int managerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ManagerId);
                int managerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.ManagerFirstName);
                int managerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.ManagerLastName);
                int telephoneNumberIndex = reader.GetOrdinal(Constants.ColumnNames.TelephoneNumber);
                int terminationDateIndex = reader.GetOrdinal(TerminationDateColumn);
                int lastLoginDateIndex = reader.GetOrdinal(LastLoginDateColumn);
                int isStrawManIndex = reader.GetOrdinal(Constants.ColumnNames.IsStrawmanColumn);
                //Pay Related columns
                int personIdIndex = reader.GetOrdinal(PersonIdColumn);
                int startDateIndex = reader.GetOrdinal(StartDateColumn);
                int endDateIndex = reader.GetOrdinal(EndDateColumn);
                int amountIndex = reader.GetOrdinal(AmountColumn);
                int timescaleIndex = reader.GetOrdinal(TimescaleColumn);
                int timescaleNameIndex = reader.GetOrdinal(TimescaleNameColumn);
                int amountHourlyIndex = reader.GetOrdinal(AmountHourlyColumn);
                int vacationDaysIndex = reader.GetOrdinal(VacationDaysColumn);
                int bonusAmountIndex = reader.GetOrdinal(BonusAmountColumn);
                int bonusHoursToCollectIndex = reader.GetOrdinal(BonusHoursToCollectColumn);
                int isYearBonusIndex = reader.GetOrdinal(IsYearBonusColumn);
                int payPersonIdIndex = reader.GetOrdinal(PayPersonIdColumn);
                int divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);
                int divisionNameIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionName);
                //  PracticesOwned column is not defined for each set that
                //  uses given method, so we need to know if that column exists
                int practicesOwnedIndex;

                try
                {
                    practicesOwnedIndex = reader.GetOrdinal(Constants.ColumnNames.PracticesOwned);
                }
                catch
                {
                    practicesOwnedIndex = -1;
                }

                while (reader.Read())
                {
                    var person = new Person
                    {
                        Id = (int)reader[PersonIdColumn],
                        FirstName = (string)reader[FirstNameColumn],
                        LastName = (string)reader[LastNameColumn],
                        Alias = !reader.IsDBNull(aliasIndex) ? reader.GetString(aliasIndex) : string.Empty,
                        HireDate = (DateTime)reader[HireDateColumn],
                        IsStrawMan = !reader.IsDBNull(isStrawManIndex) && (bool)reader.GetBoolean(isStrawManIndex),
                        TelephoneNumber = !reader.IsDBNull(telephoneNumberIndex) ? reader.GetString(telephoneNumberIndex) : string.Empty,
                        TerminationDate = !reader.IsDBNull(terminationDateIndex) ? (DateTime?)reader[terminationDateIndex] : null,
                        LastLogin = !reader.IsDBNull(lastLoginDateIndex) ? (DateTime?)reader[lastLoginDateIndex] : null,
                        DefaultPractice = !Convert.IsDBNull(reader["DefaultPractice"])
                                              ? new Practice
                                              {
                                                  Id = (int)reader["DefaultPractice"],
                                                  Name = (string)reader["PracticeName"]
                                              } : null,

                        EmployeeNumber = !Convert.IsDBNull(reader[EmployeeNumberColumn]) ? (string)reader[EmployeeNumberColumn] : null,

                        Status = new PersonStatus
                        {
                            Id = reader.GetInt32(personStatusIdIndex),
                            Name = reader.GetString(personStatusNameIndex)
                        },
                        Title = !reader.IsDBNull(titleIdIndex) ? new Title
                        {
                            TitleId = reader.GetInt32(titleIdIndex),
                            TitleName = reader.GetString(titleNameIndex)
                        } : null,
                        Manager = !Convert.IsDBNull(reader[managerIdIndex]) ? new Person
                        {
                            Id = reader.GetInt32(managerIdIndex),
                            FirstName = reader.GetString(managerFirstNameIndex),
                            LastName = reader.GetString(managerLastNameIndex)
                            // Alias = reader.GetString(managerAliasIndex)
                        } : null
                    };

                    if (!reader.IsDBNull(divisionIdIndex))
                    {
                        person.Division = new PersonDivision()
                        {
                            DivisionId = reader.GetInt32(divisionIdIndex),
                            DivisionName = reader.GetString(divisionNameIndex)
                        };
                    }

                    if (practicesOwnedIndex >= 0 && !reader.IsDBNull(practicesOwnedIndex))
                    {
                        person.PracticesOwned = new List<Practice>();

                        var xdoc = new XmlDocument();
                        var xmlPractices = reader.GetString(practicesOwnedIndex);
                        xdoc.LoadXml(xmlPractices);

                        foreach (XmlNode xpractice in xdoc.FirstChild.ChildNodes)
                        {
                            if (xpractice.Attributes != null)
                                person.PracticesOwned.Add(
                                    new Practice
                                    {
                                        Id = Convert.ToInt32(xpractice.Attributes["Id"].Value),
                                        Name = xpractice.Attributes["Name"].Value
                                    });
                        }
                    }


                    if (!reader.IsDBNull(payPersonIdIndex))
                    {
                        Pay pay = new Pay
                        {
                            PersonId = reader.GetInt32(personIdIndex),
                            Timescale = (TimescaleType)reader.GetInt32(timescaleIndex),
                            TimescaleName = reader.GetString(timescaleNameIndex),
                            Amount = reader.GetDecimal(amountIndex),
                            StartDate = reader.GetDateTime(startDateIndex),
                            EndDate =
                                !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null,
                            AmountHourly = reader.GetDecimal(amountHourlyIndex),
                            VacationDays =
                                !reader.IsDBNull(vacationDaysIndex) ? (int?)reader.GetInt32(vacationDaysIndex) : null,
                            BonusAmount = reader.GetDecimal(bonusAmountIndex),
                            BonusHoursToCollect =
                                !reader.IsDBNull(bonusHoursToCollectIndex) ?
                                    (int?)reader.GetInt32(bonusHoursToCollectIndex) : null,
                            IsYearBonus = reader.GetBoolean(isYearBonusIndex)
                        };
                        person.CurrentPay = pay;
                    }

                    personList.Add(person);
                }
            }
        }

        private static void ReadPersonsShort(SqlDataReader reader, List<Person> result)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(PersonIdColumn);
            int firstNameIndex = reader.GetOrdinal(FirstNameColumn);
            int lastNameIndex = reader.GetOrdinal(LastNameColumn);
            int isDefManagerIndex;
            int hireDateIndex;
            int terminationDateIndex;
            int isStrawManIndex;
            int personStatusIdIndex;
            int aliasIndex = -1;
            int seniorityIdColumnIndex = -1;
            int seniorityNameColumnIndex = -1;
            int preferredFirstNameIndex = -1;
            try
            {
                preferredFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.PreferredFirstName);
            }
            catch
            {

            }
            try
            {
                isDefManagerIndex = reader.GetOrdinal(Constants.ColumnNames.IsDefaultManager);
            }
            catch
            {
                isDefManagerIndex = -1;
            }

            try
            {
                personStatusIdIndex = reader.GetOrdinal(PersonStatusIdColumn);
            }
            catch
            {
                personStatusIdIndex = -1;
            }

            try
            {
                isStrawManIndex = reader.GetOrdinal(Constants.ColumnNames.IsStrawmanColumn);
            }
            catch
            {
                isStrawManIndex = -1;
            }
            try
            {
                hireDateIndex = reader.GetOrdinal(HireDateColumn);
            }
            catch
            {
                hireDateIndex = -1;
            }
            try
            {
                terminationDateIndex = reader.GetOrdinal(TerminationDateColumn);
            }
            catch
            {
                terminationDateIndex = -1;
            }

            try
            {
                aliasIndex = reader.GetOrdinal(Constants.ColumnNames.Alias);
            }
            catch
            { }
            try
            {
                seniorityIdColumnIndex = reader.GetOrdinal(SeniorityIdColumn);
                seniorityNameColumnIndex = reader.GetOrdinal(SeniorityNameColumn);
            }
            catch
            { }

            while (reader.Read())
            {
                var personId = reader.GetInt32(personIdIndex);
                var person = new Person
                {
                    Id = personId,
                    FirstName = reader.GetString(firstNameIndex),
                    LastName = reader.GetString(lastNameIndex),
                    HireDate = (hireDateIndex > -1) ? reader.GetDateTime(hireDateIndex) : DateTime.MinValue
                };
                if (preferredFirstNameIndex > -1)
                {
                    person.PrefferedFirstName = !reader.IsDBNull(preferredFirstNameIndex) ? reader.GetString(preferredFirstNameIndex) : string.Empty;
                }
                if (aliasIndex > -1 && !reader.IsDBNull(aliasIndex))
                {
                    person.Alias = reader.GetString(aliasIndex);
                }
                if (seniorityIdColumnIndex > -1 && !reader.IsDBNull(seniorityIdColumnIndex))
                {
                    person.Seniority = new Seniority
                    {
                        Id = reader.GetInt32(seniorityIdColumnIndex),
                        Name = reader.GetString(seniorityNameColumnIndex)
                    };
                }
                if (terminationDateIndex > -1)
                {
                    person.TerminationDate = !reader.IsDBNull(terminationDateIndex) ? (DateTime?)reader[terminationDateIndex] : null;
                }
                if (isStrawManIndex > -1)
                {
                    person.IsStrawMan = reader.GetBoolean(isStrawManIndex);//== 0 ? false : true
                }

                if (personStatusIdIndex > -1)
                {
                    person.Status = new PersonStatus
                    {
                        Id = reader.GetInt32(personStatusIdIndex)
                    };
                }
                if (isDefManagerIndex > -1)
                {
                    var isDefaultManager = reader.GetBoolean(isDefManagerIndex);
                    if (isDefaultManager)
                        person.Manager = new Person { Id = personId };
                }
                result.Add(person);
            }
        }

        /// <summary>
        /// Retrives a short info of persons specified by personIds.
        /// </summary>
        /// <param name="personIds"></param>
        /// <returns></returns>
        public static List<Person> GetPersonListByPersonIdList(string personIds)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetPersonListByPersonIdListProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdsParam, personIds);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();
                    ReadPersonsShort(reader, result);
                    return result;
                }
            }
        }

        public static List<Person> GetPersonListByPersonIdsAndPayTypeIds(string personIds, string paytypeIds, string practiceIds, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetPersonListByPersonIdsAndPayTypeIdsProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdsParam, personIds);
                command.Parameters.AddWithValue(TimeScaleIdsParam, paytypeIds == null ? DBNull.Value : (object)paytypeIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIds == null ? DBNull.Value : (object)practiceIds);
                command.Parameters.AddWithValue(StartDateParam, startDate);
                command.Parameters.AddWithValue(EndDateParam, endDate);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();
                    ReadPersonsShort(reader, result);
                    return result;
                }
            }
        }

        /// <summary>
        /// Retrieves a short info on persons.
        /// </summary>
        /// <param name="statusId">Person status id</param>
        /// <param name="rolename">Person role</param>
        /// <returns>A list of the <see cref="Person"/> objects</returns>
        public static List<Person> PersonListShortByRoleAndStatus(string statusIds, string rolename)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonListShortByRoleAndStatusProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(PersonStatusIdsListParam,
                                                !string.IsNullOrEmpty(statusIds) ? (object)statusIds : DBNull.Value);
                command.Parameters.AddWithValue(RoleNameParam,
                                                !string.IsNullOrEmpty(rolename) ? (object)rolename : DBNull.Value);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();
                    ReadPersonsShort(reader, result);
                    return result;
                }
            }
        }

        public static List<Person> PersonListShortByTitleAndStatus(string statusIds, string titleNames)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonListShortByTitleAndStatusProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(PersonStatusIdsListParam,
                                                !string.IsNullOrEmpty(statusIds) ? (object)statusIds : DBNull.Value);
                command.Parameters.AddWithValue(TitleNameParam,
                                                !string.IsNullOrEmpty(titleNames) ? (object)titleNames : DBNull.Value);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();
                    ReadPersonsShort(reader, result);
                    return result;
                }
            }
        }

        public static bool UserTemporaryCredentialsInsert(string userName, string password, int passwordFormat, string passwordSalt, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            int rowsEffeced = 0;

            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.UserTemporaryCredentialsInsertProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(UserNameParam, userName);
                command.Parameters.AddWithValue(PasswordParam, password);
                command.Parameters.AddWithValue(PasswordFormatParam, passwordFormat);
                command.Parameters.AddWithValue(PasswordSaltParam, passwordSalt);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                if (activeTransaction != null)
                {
                    command.Transaction = activeTransaction;
                }
                rowsEffeced = command.ExecuteNonQuery();
            }
            return rowsEffeced > 0;
        }

        public static UserCredentials GetTemporaryCredentialsByUserName(string userName)
        {
            using (SqlConnection connection = new SqlConnection(DataAccess.Other.DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Person.GetTemporaryCredentialsByUserNameProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(UserNameParam, userName);
                // command.Parameters.AddWithValue(PasswordFormatParam, passwordFormat);
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var users = new List<UserCredentials>();
                    ReadTemporaryCredentials(reader, users);
                    return users.Count > 0 ? users[0] : null;
                }
            }
        }

        public static void DeleteTemporaryCredentialsByUserName(string userName)
        {
            using (SqlConnection connection = new SqlConnection(DataAccess.Other.DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Person.DeleteTemporaryCredentialsByUserNameProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(UserNameParam, userName);
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static void ReadTemporaryCredentials(SqlDataReader reader, List<UserCredentials> users)
        {
            if (!reader.HasRows) return;
            int userNameIndex = reader.GetOrdinal(UserNameColumn);
            int passwordIndex = reader.GetOrdinal(PasswordColumn);
            int passwordSaltIndex = reader.GetOrdinal(PasswordSaltColumn);

            while (reader.Read())
            {
                var userCredentials = new UserCredentials
                {
                    UserName = reader.GetString(userNameIndex),
                    Password = reader.GetString(passwordIndex),
                    PasswordSalt = reader.GetString(passwordSaltIndex),
                };
                users.Add(userCredentials);
            }
        }

        public static void SetNewPasswordForUser(string userName, string newPassword, string passwordSalt, int passwordFormat, DateTime currentTimeUtc, string applicationName = "PracticeManagement")
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.SetNewPasswordForUserProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(AppicationNameParam, applicationName);
                command.Parameters.AddWithValue(UserNameParam, userName);
                command.Parameters.AddWithValue(PasswordFormatParam, passwordFormat);
                command.Parameters.AddWithValue(PasswordSaltParam, passwordSalt);
                command.Parameters.AddWithValue(NewPasswordParam, newPassword);
                command.Parameters.AddWithValue(CurrentTimeUtcParam, currentTimeUtc);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                command.ExecuteNonQuery();
            }
        }

        public static List<Person> PersonListByCategoryTypeAndPeriod(BudgetCategoryType categoryType, DateTime startDate, DateTime endDate)
        {
            var result = new List<Person>();

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(
                                        Constants.ProcedureNames.Person.PersonListByCategoryTypeAndPeriod,
                                        connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.CategoryTypeIdParam, (int)categoryType);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    ReadPersonsShort(reader, result);
                }
            }

            return result;
        }

        public static int PersonGetCount(string practiceIds, string divisionIdsSelected, bool showAll, string looked, string recruiterIds, string timeScaleIds, bool projected, bool terminated, bool terminatedPending, char? alphabet)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonGetCountByCommaSeparatedIdsListProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ShowAllParam, showAll);

                command.Parameters.AddWithValue(PracticeIdsListParam,
                                                    practiceIds != null ? (object)practiceIds : DBNull.Value);
                command.Parameters.AddWithValue(LookedParam,
                                                !string.IsNullOrEmpty(looked) ? (object)looked : DBNull.Value);
                command.Parameters.AddWithValue(RecruiterIdsListParam,
                                               recruiterIds != null ? (object)recruiterIds : DBNull.Value);
                command.Parameters.AddWithValue(TimescaleIdsListParam,
                                                    timeScaleIds != null ? (object)timeScaleIds : DBNull.Value);
                command.Parameters.AddWithValue(ProjectedParam, projected);
                command.Parameters.AddWithValue(TerminatedParam, terminated);
                command.Parameters.AddWithValue(TerminationPendingParam, terminatedPending);
                command.Parameters.AddWithValue(AlphabetParam, alphabet.HasValue ? (object)alphabet.Value : DBNull.Value);
                command.Parameters.AddWithValue(DivisionIdsListParam, divisionIdsSelected != null ? (object)divisionIdsSelected : DBNull.Value);

                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }

        public static void UpdateIsWelcomeEmailSentForPerson(int? personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.UpdateIsWelcomeEmailSentForPerson, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(PersonIdParam, personId.Value);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public static bool IsPersonAlreadyHavingStatus(int statusId, int? personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.IsPersonAlreadyHavingStatus, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(PersonIdParam, personId.Value);
                command.Parameters.AddWithValue(PersonStatusIdColumn, statusId);

                connection.Open();

                int count = (int)command.ExecuteNonQuery();

                if (count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static void UpdateLastPasswordChangedDateForPerson(string email)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.UpdateLastPasswordChangedDateForPersonProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(EmailParam, email);

                connection.Open();
                command.ExecuteScalar();
            }
        }

        public static List<UserPasswordsHistory> GetPasswordHistoryByUserName(string userName)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetPasswordHistoryByUserNameProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(UserNameParam, userName);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<UserPasswordsHistory>();
                    ReadPasswordHistory(reader, result);
                    return result;
                }
            }
        }

        private static void ReadPasswordHistory(SqlDataReader reader, List<UserPasswordsHistory> result)
        {
            if (!reader.HasRows) return;
            int hashedPasswordIndex = reader.GetOrdinal(PasswordColumn);
            int passwordSaltIndex = reader.GetOrdinal(PasswordSaltColumn);

            while (reader.Read())
            {
                var user = new UserPasswordsHistory
                {
                    HashedPassword = reader.GetString(hashedPasswordIndex),
                    PasswordSalt = reader.GetString(passwordSaltIndex),
                };

                result.Add(user);
            }
        }

        public static Dictionary<DateTime, bool> GetIsNoteRequiredDetailsForSelectedDateRange(DateTime start, DateTime end, int personId)
        {
            var result = new Dictionary<DateTime, bool>();

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(
                                        Constants.ProcedureNames.Person.GetNoteRequiredDetailsForSelectedDateRange,
                                        connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, start);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, end);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    ReadNoteRequiredDetails(reader, result);
                }
            }

            return result;
        }

        private static void ReadNoteRequiredDetails(SqlDataReader reader, Dictionary<DateTime, bool> result)
        {
            if (!reader.HasRows) return;
            int dateTimeIndex = reader.GetOrdinal("Date");
            int isNotesRequiredIndex = reader.GetOrdinal(Constants.ColumnNames.IsNotesRequired);

            while (reader.Read())
            {
                result.Add(reader.GetDateTime(dateTimeIndex), reader.GetInt32(isNotesRequiredIndex) == 1);
            }
        }

        public static List<Person> OwnerListAllShort(string statusIds)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.OwnerListAllShortProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(PersonStatusIdsParam, !string.IsNullOrEmpty(statusIds) ? (object)statusIds : DBNull.Value);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();
                    ReadOwnersShort(reader, result);
                    return result;
                }
            }
        }

        private static void ReadOwnersShort(SqlDataReader reader, List<Person> result)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(PersonIdColumn);
            int firstNameIndex = reader.GetOrdinal(FirstNameColumn);
            int lastNameIndex = reader.GetOrdinal(LastNameColumn);

            while (reader.Read())
            {
                var personId = reader.GetInt32(personIdIndex);
                var person = new Person
                {
                    Id = personId,
                    FirstName = reader.GetString(firstNameIndex),
                    LastName = reader.GetString(lastNameIndex)
                };

                result.Add(person);
            }
        }

        public static void SaveStrawManFromExisting(int existingPersonId, Person person, out int newPersonid, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Person.SaveStrawManFromExistingProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(FirstNameParam, person.FirstName);
                    command.Parameters.AddWithValue(LastNameParam, person.LastName);

                    var personIdParameter = new SqlParameter
                    {
                        ParameterName = PersonIdParam,
                        SqlDbType = SqlDbType.Int,
                        Value = existingPersonId,
                        Direction = ParameterDirection.InputOutput
                    };
                    command.Parameters.Add(personIdParameter);

                    command.Parameters.AddWithValue(AmountParam, person.CurrentPay.Amount.Value);
                    command.Parameters.AddWithValue(TimescaleParam, person.CurrentPay.Timescale);
                    command.Parameters.AddWithValue(VacationDaysParam, person.CurrentPay.VacationDays.HasValue ? (object)person.CurrentPay.VacationDays.Value : DBNull.Value);
                    command.Parameters.AddWithValue(PersonStatusIdParam, person.Status != null && person.Id.HasValue ? (object)person.Status.Id : (int)PersonStatusType.Active);
                    command.Parameters.AddWithValue(UserLoginParam, userLogin);

                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    newPersonid = (int)personIdParameter.Value;
                }
            }
        }

        public static void SaveStrawMan(Person person, Pay currentPay, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Person.SaveStrawManProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(FirstNameParam, person.FirstName);
                    command.Parameters.AddWithValue(LastNameParam, person.LastName);
                    command.Parameters.AddWithValue(UserLoginParam, userLogin);

                    //var personIdParameter = new SqlParameter(PersonIdParam, SqlDbType.Int) { Direction = ParameterDirection.InputOutput, Value = person.Id.HasValue ? (object)person.Id.Value : DBNull.Value };
                    var personIdParameter = new SqlParameter
                    {
                        ParameterName = PersonIdParam,
                        SqlDbType = SqlDbType.Int,
                        Value = person.Id.HasValue ? (object)person.Id.Value : DBNull.Value,
                        Direction = ParameterDirection.InputOutput
                    };

                    command.Parameters.Add(personIdParameter);

                    command.Parameters.AddWithValue(AmountParam, currentPay.Amount.Value);
                    command.Parameters.AddWithValue(TimescaleParam, currentPay.Timescale);
                    command.Parameters.AddWithValue(VacationDaysParam, currentPay.VacationDays.HasValue ? (object)currentPay.VacationDays.Value : DBNull.Value);
                    command.Parameters.AddWithValue(BonusAmountParam, currentPay.BonusAmount.Value);
                    command.Parameters.AddWithValue(BonusHoursToCollectParam, currentPay.BonusHoursToCollect.HasValue ? (object)currentPay.BonusHoursToCollect.Value : DBNull.Value);
                    command.Parameters.AddWithValue(StartDateParam, currentPay.StartDate == DateTime.MinValue ? DBNull.Value : (object)currentPay.StartDate);
                    command.Parameters.AddWithValue(PersonStatusIdParam, person.Status != null && person.Id.HasValue ? (object)person.Status.Id : (int)PersonStatusType.Active);

                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    person.Id = (int)personIdParameter.Value;
                }
            }
        }

        public static void DeleteStrawman(int personId, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Person.DeleteStrawmanProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(PersonIdParam, personId);
                    command.Parameters.AddWithValue(UserLoginParam, userLogin);

                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public static List<ConsultantDemandItem> GetConsultantswithDemand(DateTime startDate, DateTime endDate)
        {
            var consultants = new List<ConsultantDemandItem>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Person.GetConsultantDemandProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        ReadConConsultantDemandItems(reader, consultants);
                        return consultants;
                    }
                }
            }
        }

        private static void ReadConConsultantDemandItems(SqlDataReader reader, List<ConsultantDemandItem> consultants)
        {
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int objectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ObjectId);
            int objectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ObjectName);
            int objectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ObjectNumber);
            int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientId);
            int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientName);
            int objectTypeIndex = reader.GetOrdinal(Constants.ColumnNames.ObjectType);
            int quantityStringIndex = reader.GetOrdinal(Constants.ColumnNames.QuantityString);
            int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDateColumn);
            int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDateColumn);
            int objectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ObjectStatusId);
            int opportunintyDescriptionIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunintyDescription);
            int projectDescriptionIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectDescription);
            int linkedObjectIdIndex = reader.GetOrdinal(Constants.ColumnNames.LinkedObjectId);
            int linkedObjectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.LinkedObjectNumber);

            int personIdIndex = reader.GetOrdinal(PersonIdColumn);
            if (!reader.HasRows) return;
            var clients = new List<Client>();
            var persons = new List<Person>();

            while (reader.Read())
            {
                var consultant = new ConsultantDemandItem
                {
                    ObjectId = reader.GetInt32(objectIdIndex),
                    ObjectName = reader.GetString(objectNameIndex),
                    ObjectNumber = reader.GetString(objectNumberIndex),
                    ObjectStatusId = reader.GetInt32(objectStatusIdIndex),
                    QuantityString = reader.GetString(quantityStringIndex),
                    ObjectType = reader.GetInt32(objectTypeIndex),
                    StartDate = reader.GetDateTime(startDateIndex),
                    EndDate = reader.GetDateTime(endDateIndex),
                    LinkedObjectId = reader.IsDBNull(linkedObjectIdIndex) ? null : (int?)reader.GetInt32(linkedObjectIdIndex),
                    LinkedObjectNumber = reader.IsDBNull(linkedObjectNumberIndex) ? null : reader.GetString(linkedObjectNumberIndex),
                    OpportunintyDescription = !reader.IsDBNull(opportunintyDescriptionIndex) ? reader.GetString(opportunintyDescriptionIndex) : string.Empty,
                    ProjectDescription = !reader.IsDBNull(projectDescriptionIndex) ? reader.GetString(projectDescriptionIndex) : string.Empty
                };
                var clientId = reader.GetInt32(clientIdIndex);
                var client = clients.FirstOrDefault(c => c.Id.Value == clientId);
                var personId = reader.GetInt32(personIdIndex);
                var person = persons.FirstOrDefault(p => p.Id.Value == personId);
                if (client == null)
                {
                    client = new Client
                    {
                        Id = clientId,
                        Name = reader.GetString(clientNameIndex)
                    };
                    clients.Add(client);
                }
                if (person == null)
                {
                    person = new Person
                    {
                        Id = reader.GetInt32(personIdIndex),
                        LastName = reader.GetString(lastNameIndex),
                        FirstName = reader.GetString(firstNameIndex)
                    };
                    persons.Add(person);
                }
                consultant.Client = client;
                consultant.Consultant = person;
                consultants.Add(consultant);
            }
        }

        public static List<Person> GetStrawmenListAll()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetStrawManListAllProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();
                    ReadStrawmansWithCurrentPay(reader, result);
                    return result;
                }
            }
        }

        public static List<Person> GetStrawmenListAllShort(bool includeInactive)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetStrawManListAllShortProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.IncludeInactive, includeInactive);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();
                    ReadStrawmanShort(reader, result);
                    return result;
                }
            }
        }

        public static List<Person> GetStrawmanListShortFilterWithTodayPay()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetStrawmanListShortFilterWithTodayPay, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();
                    ReadStrawmanShort(reader, result);
                    return result;
                }
            }
        }

        public static Person GetStrawmanDetailsByIdWithCurrentPay(int strawmanId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetStrawmanDetailsByIdWithCurrentPayProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                connection.Open();

                command.Parameters.AddWithValue(Constants.ParameterNames.StrawmanIdParam, strawmanId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return ReadStrawmanWithCurrentPay(reader);
                }
            }
        }

        private static Person ReadStrawmanWithCurrentPay(SqlDataReader reader)
        {
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int personStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusName);
            int inUseIndex = reader.GetOrdinal(Constants.ColumnNames.InUse);
            int payPersonIdIndex = reader.GetOrdinal(Constants.ColumnNames.PayPersonIdColumn);
            int timescaleIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleColumn);
            int timescaleNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleName);
            int amountIndex = reader.GetOrdinal(Constants.ColumnNames.Amount);
            int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDateColumn);
            int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDateColumn);
            int vacationDaysIndex = reader.GetOrdinal(Constants.ColumnNames.VacationDaysColumn);
            int bonusAmountIndex = reader.GetOrdinal(BonusAmountColumn);
            int bonusHoursToCollectIndex = reader.GetOrdinal(BonusHoursToCollectColumn);
            int isYearBonusIndex = reader.GetOrdinal(IsYearBonusColumn);

            Person person = null;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    person = new Person
                    {
                        Id = reader.GetInt32(personIdIndex),
                        LastName = reader.GetString(lastNameIndex),
                        FirstName = reader.GetString(firstNameIndex),
                        InUse = reader.GetInt32(inUseIndex) == 1,
                        Status = new PersonStatus
                        {
                            Id = (int)Enum.Parse(
                                        typeof(PersonStatusType),
                                        (string)reader[personStatusNameIndex]),
                            Name = reader.GetString(personStatusNameIndex)
                        },
                    };

                    if (reader.IsDBNull(payPersonIdIndex)) continue;
                    Pay pay = new Pay
                    {
                        PersonId = reader.GetInt32(payPersonIdIndex),
                        Timescale = (TimescaleType)reader.GetInt32(timescaleIndex),
                        TimescaleName = reader.GetString(timescaleNameIndex),
                        Amount = reader.GetDecimal(amountIndex),
                        StartDate = reader.GetDateTime(startDateIndex),
                        EndDate =
                            !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null,
                        VacationDays =
                            !reader.IsDBNull(vacationDaysIndex) ? (int?)reader.GetInt32(vacationDaysIndex) : null,
                        BonusAmount = reader.GetDecimal(bonusAmountIndex),
                        BonusHoursToCollect =
                            !reader.IsDBNull(bonusHoursToCollectIndex) ?
                                (int?)reader.GetInt32(bonusHoursToCollectIndex) : null,
                        IsYearBonus = reader.GetBoolean(isYearBonusIndex),
                    };
                    person.CurrentPay = pay;
                }
            }
            return person;
        }

        private static void ReadStrawmansWithCurrentPay(SqlDataReader reader, List<Person> result)
        {
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int personStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusName);
            int inUseIndex = reader.GetOrdinal(Constants.ColumnNames.InUse);

            int payPersonIdIndex = reader.GetOrdinal(Constants.ColumnNames.PayPersonIdColumn);
            int timescaleIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleColumn);
            int timescaleNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleName);
            int amountIndex = reader.GetOrdinal(Constants.ColumnNames.Amount);
            int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDateColumn);
            int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDateColumn);
            int vacationDaysIndex = reader.GetOrdinal(Constants.ColumnNames.VacationDaysColumn);

            if (!reader.HasRows) return;
            while (reader.Read())
            {
                var person = new Person
                {
                    Id = reader.GetInt32(personIdIndex),
                    LastName = reader.GetString(lastNameIndex),
                    FirstName = reader.GetString(firstNameIndex),
                    InUse = reader.GetInt32(inUseIndex) == 1,

                    Status = new PersonStatus
                    {
                        Id = (int)Enum.Parse(
                            typeof(PersonStatusType),
                            (string)reader[personStatusNameIndex]),
                        Name = reader.GetString(personStatusNameIndex)
                    },
                };

                if (!reader.IsDBNull(payPersonIdIndex))
                {
                    Pay pay = new Pay
                    {
                        PersonId = reader.GetInt32(personIdIndex),
                        TimescaleName = reader.GetString(timescaleNameIndex),
                        Timescale = (TimescaleType)reader.GetInt32(timescaleIndex),
                        Amount = reader.GetDecimal(amountIndex),
                        StartDate = reader.GetDateTime(startDateIndex),
                        EndDate = !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null,
                        VacationDays = !reader.IsDBNull(vacationDaysIndex) ? (int?)reader.GetInt32(vacationDaysIndex) : 0
                    };
                    person.CurrentPay = pay;
                }
                result.Add(person);
            }
        }

        private static void ReadStrawmanShort(SqlDataReader reader, List<Person> result)
        {
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            if (!reader.HasRows) return;
            while (reader.Read())
            {
                var person = new Person
                {
                    Id = reader.GetInt32(personIdIndex),
                    LastName = reader.GetString(lastNameIndex),
                    FirstName = reader.GetString(firstNameIndex)
                };
                result.Add(person);
            }
        }

        public static Person GetPersonFirstLastNameById(int personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonFirstLastNameByIdProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam, personId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var person = new Person();
                    ReadFirstLastName(reader, person);
                    person.Id = personId;
                    return person;
                }
            }
        }

        private static void ReadFirstLastName(DbDataReader reader, Person person)
        {
            if (!reader.HasRows) return;
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int isStrawManIndex = reader.GetOrdinal(Constants.ColumnNames.IsStrawmanColumn);
            int timeScaleIndex = reader.GetOrdinal(TimescaleColumn);
            int isOffshoreIndex = reader.GetOrdinal(Constants.ColumnNames.IsOffshore);
            int personStatusIdIndex = reader.GetOrdinal(PersonStatusIdColumn);
            int aliasIndex = reader.GetOrdinal(Constants.ColumnNames.Alias);
            int hireDateIndex = reader.GetOrdinal(Constants.ColumnNames.HireDateColumn);
            int employeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeNumber);
            int seniorityIdIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorityIdColumn);
            int seniorityIndex = reader.GetOrdinal(Constants.ColumnNames.Seniority);
            int preferredFirstNameIndex = -1;
            try
            {
                preferredFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.PreferredFirstName);
            }
            catch
            {

            }
            while (reader.Read())
            {
                person.FirstName = reader.GetString(firstNameIndex);
                person.LastName = reader.GetString(lastNameIndex);
                person.Alias = reader.GetString(aliasIndex);
                person.HireDate = reader.GetDateTime(hireDateIndex);
                person.IsStrawMan = !reader.IsDBNull(isStrawManIndex) && reader.GetBoolean(isStrawManIndex);
                person.EmployeeNumber = reader.GetString(employeeNumberIndex);
                person.CurrentPay = new Pay
                {
                    TimescaleName = reader.IsDBNull(timeScaleIndex) ? String.Empty : reader.GetString(timeScaleIndex)
                };
                person.IsOffshore = reader.GetBoolean(isOffshoreIndex);
                person.Status = new PersonStatus
                {
                    Id = reader.GetInt32(personStatusIdIndex)
                };
                person.Seniority = new Seniority
                {
                    Id = reader.IsDBNull(seniorityIdIndex) ? -1 : reader.GetInt32(seniorityIdIndex),
                    Name = reader.IsDBNull(seniorityIndex) ? string.Empty : reader.GetString(seniorityIndex)
                };
                if (preferredFirstNameIndex > -1)
                {
                    person.PrefferedFirstName = reader.IsDBNull(preferredFirstNameIndex) ? string.Empty : reader.GetString(preferredFirstNameIndex);
                }
                if (!string.IsNullOrEmpty(person.FirstName) && !string.IsNullOrEmpty(person.LastName))
                {
                    break;
                }
            }
        }

        public static bool IsPersonHaveActiveStatusDuringThisPeriod(int personId, DateTime startDate, DateTime? endDate)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Person.IsPersonHaveActiveStatusDuringThisPeriod, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);

                if (endDate != null && endDate.HasValue)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate.Value);
                }

                connection.Open();
                return ((bool)command.ExecuteScalar());
            }
        }

        public static List<Person> PersonsListHavingActiveStatusDuringThisPeriod(DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonsListHavingActiveStatusDuringThisPeriodProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(StartDateParam, startDate);
                command.Parameters.AddWithValue(EndDateParam, endDate);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();
                    ReadPersonsShort(reader, result);
                    return result;
                }
            }
        }

        public static List<Person> GetApprovedByManagerList()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetApprovedByManagerListProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();

                    if (reader.HasRows)
                    {
                        int personIdIndex = reader.GetOrdinal(PersonIdColumn);
                        int firstNameIndex = reader.GetOrdinal(FirstNameColumn);
                        int lastNameIndex = reader.GetOrdinal(LastNameColumn);
                        while (reader.Read())
                        {
                            var personId = reader.GetInt32(personIdIndex);
                            var person = new Person
                            {
                                Id = personId,
                                FirstName = reader.GetString(firstNameIndex),
                                LastName = reader.GetString(lastNameIndex)
                            };
                            result.Add(person);
                        }
                    }
                    return result;
                }
            }
        }

        public static List<Person> GetPersonListBySearchKeyword(String looked)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetPersonListBySearchKeywordProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(LookedParam, !string.IsNullOrEmpty(looked) ? (object)looked : DBNull.Value);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();

                    if (reader.HasRows)
                    {
                        int personIdIndex = reader.GetOrdinal(PersonIdColumn);
                        int firstNameIndex = reader.GetOrdinal(FirstNameColumn);
                        int lastNameIndex = reader.GetOrdinal(LastNameColumn);
                        int timeScaleIndex = reader.GetOrdinal(TimescaleColumn);
                        int statusNameIndex = reader.GetOrdinal(PersonStatusNameColumn);

                        while (reader.Read())
                        {
                            var personId = reader.GetInt32(personIdIndex);
                            var person = new Person
                            {
                                Id = personId,
                                FirstName = reader.GetString(firstNameIndex),
                                LastName = reader.GetString(lastNameIndex),
                                Status = new PersonStatus
                                {
                                    Name = reader.GetString(statusNameIndex)
                                },
                                CurrentPay = new Pay
                                {
                                    TimescaleName = reader.IsDBNull(timeScaleIndex) ? String.Empty : reader.GetString(timeScaleIndex)
                                }
                            };
                            result.Add(person);
                        }
                    }
                    return result;
                }
            }
        }

        public static List<TerminationReason> GetTerminationReasonsList()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetTerminationReasonsList, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<TerminationReason>();

                    if (reader.HasRows)
                    {
                        int terminationReasonIdIndex = reader.GetOrdinal(TerminationReasonIdColumn);
                        int terminationReasonIndex = reader.GetOrdinal(TerminationReasonColumn);
                        int isW2SalaryRuleIndex = reader.GetOrdinal(Constants.ColumnNames.IsW2SalaryRule);
                        int isW2HourlyRuleIndex = reader.GetOrdinal(Constants.ColumnNames.IsW2HourlyRule);
                        int is1099RuleIndex = reader.GetOrdinal(Constants.ColumnNames.Is1099Rule);
                        int isContingentRuleIndex = reader.GetOrdinal(Constants.ColumnNames.IsContingentRule);
                        int isVisibleIndex = reader.GetOrdinal(Constants.ColumnNames.IsVisible);

                        while (reader.Read())
                        {
                            var terminationReason = new TerminationReason
                            {
                                Id = reader.GetInt32(terminationReasonIdIndex),
                                Name = reader.GetString(terminationReasonIndex),
                                IsW2SalaryRule = reader.GetBoolean(isW2SalaryRuleIndex),
                                IsW2HourlyRule = reader.GetBoolean(isW2HourlyRuleIndex),
                                Is1099Rule = reader.GetBoolean(is1099RuleIndex),
                                IsContigent = reader.GetBoolean(isContingentRuleIndex),
                                IsVisible = reader.GetBoolean(isVisibleIndex)
                            };

                            result.Add(terminationReason);
                        }
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// Reading hiredate and termination date for a person.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns>Person</returns>
        public static Person GetPersonHireAndTerminationDateById(int personId)
        {
            var person = new Person();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetPersonHireAndTerminationDateById, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(PersonIdParam, personId);

                    connection.Open();

                    person.Id = personId;

                    ReadPersonHireAndTerminationdate(command, person);
                }
            }

            return person;
        }

        private static void ReadPersonHireAndTerminationdate(SqlCommand command, Person person)
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (!reader.HasRows) return;
                int hireDateIndex = reader.GetOrdinal(HireDateColumn);
                int terminationDateIndex = reader.GetOrdinal(TerminationDateColumn);

                while (reader.Read())
                {
                    person.HireDate = reader.GetDateTime(hireDateIndex);
                    person.TerminationDate = !reader.IsDBNull(terminationDateIndex) ? (DateTime?)reader[terminationDateIndex] : null;
                }
            }
        }

        public static List<Person> GetPersonListWithRole(string rolename)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetPersonListWithRole, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.RoleNameParam, rolename);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Person>();
                    ReadPersonsShort(reader, result);
                    return result;
                }
            }
        }

        /// <summary>
        /// Get Person's Hire/Rehire report(Employment History)
        /// </summary>
        /// <param name="personId"></param>
        /// <returns>list of Hire, Termination dates and Termination reasons</returns>
        public static List<Employment> GetPersonEmploymentHistoryById(int personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetPersonEmploymentHistoryById, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(PersonIdParam, personId);

                    connection.Open();

                    var result = new List<Employment>();
                    var reader = command.ExecuteReader();
                    ReadPersonEmploymentHistory(reader, result);

                    return result;
                }
            }
        }

        private static void ReadPersonEmploymentHistory(SqlDataReader reader, List<Employment> result)
        {
            if (!reader.HasRows) return;
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int hireDateIndex = reader.GetOrdinal(Constants.ColumnNames.HireDateColumn);
            int terminationDateIndex = reader.GetOrdinal(Constants.ColumnNames.TerminationDateColumn);
            int terminationReasonIdIndex = reader.GetOrdinal(Constants.ColumnNames.TerminationReasonIdColumn);

            while (reader.Read())
            {
                var employment = new Employment
                {
                    PersonId = reader.GetInt32(personIdIndex),
                    HireDate = reader.GetDateTime(hireDateIndex),
                    TerminationDate =
                        reader.IsDBNull(terminationDateIndex)
                            ? null
                            : (DateTime?)reader.GetDateTime(terminationDateIndex),
                    TerminationReasonId =
                        reader.IsDBNull(terminationReasonIdIndex)
                            ? null
                            : (int?)reader.GetInt32(terminationReasonIdIndex)
                };

                result.Add(employment);
            }
        }

        public static List<TimeTypeRecord> GetPersonAdministrativeTimeTypesInRange(int personId, DateTime startDate, DateTime endDate, bool includePTO, bool includeHoliday, bool includeUnpaid, bool includeSickLeave)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetPersonAdministrativeTimeTypesInRange, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IncludePTOParam, includePTO);
                command.Parameters.AddWithValue(Constants.ParameterNames.IncludeHolidayParam, includeHoliday);
                command.Parameters.AddWithValue(Constants.ParameterNames.IncludeUnpaidParam, includeUnpaid);
                command.Parameters.AddWithValue(Constants.ParameterNames.IncludeSickLeaveParam, includeSickLeave);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<TimeTypeRecord>();
                    TimeTypeDAL.ReadTimeTypesShort(reader, result);
                    return result;
                }
            }
        }

        /// <summary>
        /// gets weather the given person in given date range has timeoff's.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="includeEmployeeTimeType"></param>
        /// <returns></returns>
        public static bool IsPersonTimeOffExistsInSelectedRangeForOtherthanGivenTimescale(int personId, DateTime startDate, DateTime endDate, int timescaleId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.IsPersonTimeOffExistsInSelectedRangeForOtherthanGivenTimescale, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimescaleId, timescaleId);

                connection.Open();

                bool result = (bool)command.ExecuteScalar();
                return result;
            }
        }

        public static PersonPassword GetPersonEncodedPassword(int personId)
        {
            using (SqlConnection connection = new SqlConnection(DataAccess.Other.DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Person.GetPersonEncodedPasswordProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(PersonIdParam, personId);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return ReadPersonEncodedPassword(reader);
                }
            }
        }

        private static PersonPassword ReadPersonEncodedPassword(SqlDataReader reader)
        {
            PersonPassword personPassword = null;
            if (reader.HasRows)
            {
                int passwordIndex = reader.GetOrdinal(PasswordColumn);

                while (reader.Read())
                {
                    personPassword = new PersonPassword
                    {
                        Password = reader.GetString(passwordIndex)
                    };
                }
            }
            return personPassword;
        }

        public static void DeletePersonEncodedPassword(int personId)
        {
            using (SqlConnection connection = new SqlConnection(DataAccess.Other.DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Person.DeletePersonEncodedPasswordProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(PersonIdParam, personId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static List<Person> GetActivePersonsByProjectId(int projectId)
        {
            var result = new List<Person>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetActivePersonsByProjectId, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, projectId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadPersonsShort(reader, result);
                }
            }
            return result;
        }

        public static Title GetPersonTitleByRange(int personId, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetPersonTitleByRange, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return ReadTitle(reader);
                }
            }
        }

        private static Title ReadTitle(SqlDataReader reader)
        {
            if (reader.HasRows)
            {
                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int titleNameIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                while (reader.Read())
                {
                    int titleId = reader.IsDBNull(titleIdIndex) ? -1 : reader.GetInt32(titleIdIndex);
                    if (titleId != -1)
                    {
                        Title title = new Title()
                        {
                            TitleId = titleId,
                            TitleName = reader.GetString(titleNameIndex)
                        };
                        return title;
                    }
                }
            }
            return null;
        }

        private static void ReadCommissions(SqlDataReader reader, List<Project> result)
        {
            if (reader.HasRows)
            {
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
                while (reader.Read())
                {
                    Project project = new Project()
                    {
                        ProjectNumber = reader.GetString(projectNumberIndex),
                        Name = reader.GetString(projectNameIndex)
                    };
                    result.Add(project);
                }
            }
        }

        public static bool CheckIfRangeWithinHireAndTermination(int personId, DateTime startDate, DateTime endDate)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Person.CheckIfRangeWithinHireAndTermination, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                connection.Open();
                return ((bool)command.ExecuteScalar());
            }
        }

        public static bool CheckIfPersonConsultantTypeInAPeriod(int personId, DateTime startDate, DateTime endDate)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                SqlCommand command = new SqlCommand(
                    Constants.ProcedureNames.Person.CheckIfPersonConsultantTypeInAPeriod, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                connection.Open();
                return ((bool)command.ExecuteScalar());
            }
        }

        public static List<Project> GetCommissionsValidationByPersonId(int personId, DateTime hireDate, DateTime? terminationDate, int personStatusId, int divisionId, bool IsReHire)
        {
            List<Project> result = new List<Project>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Person.GetCommissionsValidationByPersonId, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.HireDate, hireDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.TerminationDate, terminationDate.HasValue ? (object)terminationDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonStatusId, personStatusId);
                command.Parameters.AddWithValue(Constants.ParameterNames.DivisionId, divisionId);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsReHire, IsReHire);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadCommissions(reader, result);
                }
            }
            return result;
        }

        public static Person CheckIfValidDivision(int personId, DateTime startDate, DateTime endDate)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                SqlCommand command = new SqlCommand(
                    Constants.ProcedureNames.Person.CheckIfValidDivision, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return ReadDivision(reader);
                }
            }
        }

        private static Person ReadDivision(SqlDataReader reader)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
                int divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);
                while (reader.Read())
                {
                    Person person = new Person()
                    {
                        Id = reader.GetInt32(personIdIndex),
                        HireDate = reader.GetDateTime(startDateIndex),
                        TerminationDate = reader.IsDBNull(endDateIndex) ? (DateTime?)null : reader.GetDateTime(endDateIndex),
                        DivisionType = (PersonDivisionType)(reader.IsDBNull(divisionIdIndex) ? 0 : reader.GetInt32(divisionIdIndex))
                    };
                    return person;
                }
            }
            return null;
        }

        public static bool CheckIfPersonEntriesOverlapps(int milestoneId, int personId, DateTime startDate, DateTime endDate)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                SqlCommand command = new SqlCommand(
                    Constants.ProcedureNames.Person.CheckIfPersonEntriesOverlapps, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneId, milestoneId);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                connection.Open();
                return ((bool)command.ExecuteScalar());
            }
        }

        public static List<Person> GetPersonsByPayTypesAndByStatusIds(string statusIds, string payTypeIds)
        {
            var result = new List<Person>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetPersonsByPayTypesAndByStatusIds, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.StatusIds, statusIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.PayTypeIds, payTypeIds);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadPersonsShort(reader, result);
                }
            }
            return result;
        }

        public static List<Person> GetPTOReport(DateTime startDate, DateTime endDate, bool includeCompanyHolidays)
        {
            List<Person> result = new List<Person>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Person.GetPTOReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IncludeCompanyHolidays, includeCompanyHolidays);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadPTOReport(reader, result);
                }
            }
            return result;
        }

        private static void ReadPTOReport(SqlDataReader reader, List<Person> result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int employeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeNumber);
                int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int timeTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeId);
                int timeTypeNameIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeName);
                int timeOffStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.TimeOffStartDate);
                int timeOffEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.TimeOffEndDate);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                int projectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusId);
                int projectStatusNameColumnIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
                int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientId);
                int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientName);
                int businessUnitIdIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessUnitId);
                int businessUnitNameIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessUnitName);
                int businessGroupIdColumnIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessGroupIdColumn);
                int businessGroupNameIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessGroupName);
                int practiceIdColumnIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                int practiceAreaNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeAreaName);
                int projectManagersIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagers);
                int seniorManagerIdIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorManagerId);
                int seniorManagerNameIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorManagerName);
                int directorIdColumnIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorIdColumn);
                int directorFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorFirstNameColumn);
                int directorLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorLastNameColumn);
                while (reader.Read())
                {
                    var personId = reader.GetInt32(personIdIndex);
                    var timetype = new TimeTypeRecord()
                    {
                        Id = reader.GetInt32(timeTypeIdIndex),
                        Name = reader.GetString(timeTypeNameIndex)
                    };
                    var timeoffStartDate = reader.GetDateTime(timeOffStartDateIndex);
                    var timeoffEndDate = reader.GetDateTime(timeOffEndDateIndex);
                    var project = new Project()
                    {
                        ProjectNumber = reader.IsDBNull(projectNumberIndex) ? null : reader.GetString(projectNumberIndex),
                        Name = reader.IsDBNull(projectNameIndex) ? null : reader.GetString(projectNameIndex),
                        Status = new ProjectStatus()
                        {
                            Id = reader.IsDBNull(projectStatusIdIndex) ? -1 : reader.GetInt32(projectStatusIdIndex),
                            Name = reader.IsDBNull(projectStatusNameColumnIndex) ? null : reader.GetString(projectStatusNameColumnIndex),
                        },
                        Client = new Client()
                        {
                            Id = reader.IsDBNull(clientIdIndex) ? (int?)null : reader.GetInt32(clientIdIndex),
                            Name = reader.IsDBNull(clientNameIndex) ? null : reader.GetString(clientNameIndex)
                        },
                        Group = new ProjectGroup()
                        {
                            Id = reader.IsDBNull(businessUnitIdIndex) ? (int?)null : reader.GetInt32(businessUnitIdIndex),
                            Name = reader.IsDBNull(businessUnitNameIndex) ? null : reader.GetString(businessUnitNameIndex)
                        },
                        BusinessGroup = new BusinessGroup()
                        {
                            Id = reader.IsDBNull(businessGroupIdColumnIndex) ? (int?)null : reader.GetInt32(businessGroupIdColumnIndex),
                            Name = reader.IsDBNull(businessGroupNameIndex) ? null : reader.GetString(businessGroupNameIndex)
                        },
                        Practice = new Practice()
                        {
                            Id = reader.IsDBNull(practiceIdColumnIndex) ? -1 : reader.GetInt32(practiceIdColumnIndex),
                            Name = reader.IsDBNull(practiceAreaNameIndex) ? null : reader.GetString(practiceAreaNameIndex)
                        },
                        ProjectManagerNames = reader.IsDBNull(projectManagersIndex) ? string.Empty : reader.GetString(projectManagersIndex),
                        SeniorManagerId = reader.IsDBNull(seniorManagerIdIndex) ? -1 : reader.GetInt32(seniorManagerIdIndex),
                        SeniorManagerName = reader.IsDBNull(seniorManagerNameIndex) ? string.Empty : reader.GetString(seniorManagerNameIndex),
                        Director = new Person()
                        {
                            Id = reader.IsDBNull(directorIdColumnIndex) ? (int?)null : reader.GetInt32(directorIdColumnIndex),
                            FirstName = reader.IsDBNull(directorFirstNameIndex) ? string.Empty : reader.GetString(directorFirstNameIndex),
                            LastName = reader.IsDBNull(directorLastNameIndex) ? string.Empty : reader.GetString(directorLastNameIndex)
                        }
                    };
                    if (result.Any(p => p.Id == personId && p.TimeOffHistory.Any(t => t.TimeOffStartDate == timeoffStartDate)))
                    {
                        var personTimeoff = (result.First(p => p.Id == personId && p.TimeOffHistory.Any(t => t.TimeOffStartDate == timeoffStartDate && t.TimeOffEndDate == timeoffEndDate)));
                        var projects = personTimeoff.TimeOffHistory.First(t => t.TimeOffStartDate == timeoffStartDate).Projects;
                        projects.Add(project);
                    }
                    else if (result.Any(p => p.Id == personId && !p.TimeOffHistory.Any(t => t.TimeOffStartDate == timeoffStartDate)))
                    {
                        var personTimeOff = new PersonTimeOff()
                        {
                            TimeType = timetype,
                            TimeOffStartDate = timeoffStartDate,
                            TimeOffEndDate = timeoffEndDate,
                            Projects = new List<Project>() { project }
                        };
                        var person = result.First(p => p.Id == personId);
                        person.TimeOffHistory.Add(personTimeOff);
                    }
                    else
                    {
                        var personTimeOff = new PersonTimeOff()
                        {
                            TimeType = timetype,
                            TimeOffStartDate = timeoffStartDate,
                            TimeOffEndDate = timeoffEndDate,
                            Projects = new List<Project>() { project }
                        };
                        Person person = new Person()
                        {
                            Id = personId,
                            EmployeeNumber = reader.GetString(employeeNumberIndex),
                            FirstName = reader.GetString(firstNameIndex),
                            LastName = reader.GetString(lastNameIndex),
                            TimeOffHistory = new List<PersonTimeOff>() { personTimeOff }
                        };
                        result.Add(person);
                    }
                }
            }
        }

        #region Cohort Assignments

        public static List<CohortAssignment> GetAllCohortAssignments()
        {
            var result = new List<CohortAssignment>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetAllCohortAssignments, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadCohorts(reader, result);
                }
            }
            return result;
        }

        private static void ReadCohorts(SqlDataReader reader, List<CohortAssignment> result)
        {
            if (reader.HasRows)
            {
                int cohortAssignmentIdIndex = reader.GetOrdinal(Constants.ColumnNames.CohortAssignmentId);
                int cohortAssignmentNameIndex = reader.GetOrdinal(Constants.ColumnNames.CohortAssignmentName);
                while (reader.Read())
                {
                    CohortAssignment cohort = new CohortAssignment()
                    {
                        Id = reader.GetInt32(cohortAssignmentIdIndex),
                        Name = reader.GetString(cohortAssignmentNameIndex)
                    };
                    result.Add(cohort);
                }
            }
        }

        #endregion

        #region MSBadge

        public static List<MSBadge> GetBadgeDetailsByPersonId(int personId)
        {
            var result = new List<MSBadge>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetBadgeDetailsByPersonId, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadBadgeDetails(reader, result);
                }
            }
            return result;
        }

        private static void ReadBadgeDetails(SqlDataReader reader, List<MSBadge> result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int firstnameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int badgeStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeStartDate);
                int badgeEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeEndDate);
                int plannedEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.PlannedEndDate);
                int breakStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BreakStartDate);
                int breakEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BreakEndDate);
                int isBlockedIndex = reader.GetOrdinal(Constants.ColumnNames.IsBlocked);
                int blockStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BlockStartDate);
                int blockEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BlockEndDate);
                int isPreviousBadgeIndex = reader.GetOrdinal(Constants.ColumnNames.IsPreviousBadge);
                int previousBadgeAliasIndex = reader.GetOrdinal(Constants.ColumnNames.PreviousBadgeAlias);
                int lastBadgeStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.LastBadgeStartDate);
                int lastBadgeEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.LastBadgeEndDate);
                int isExceptionIndex = reader.GetOrdinal(Constants.ColumnNames.IsException);
                int exceptionStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.ExceptionStartDate);
                int exceptionEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.ExceptionEndDate);
                int badgeStartDateSourceIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeStartDateSource);
                int plannedEndDateSourceIndex = reader.GetOrdinal(Constants.ColumnNames.PlannedEndDateSource);
                int badgeEndDateSourceIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeEndDateSource);
                int deactivatedDateIndex = reader.GetOrdinal(Constants.ColumnNames.DeactivatedDate);
                int organicBreakStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.OrganicBreakStartDate);
                int organicBreakEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.OrganicBreakEndDate);
                int excludeInReportsIndex = reader.GetOrdinal(Constants.ColumnNames.ExcludeInReports);
                int manageServiceContractIndex = reader.GetOrdinal(Constants.ColumnNames.ManageServiceContract);
                while (reader.Read())
                {
                    var badge = new MSBadge()
                    {
                        Person = new Person()
                        {
                            Id = reader.GetInt32(personIdIndex),
                            FirstName = reader.GetString(firstnameIndex),
                            LastName = reader.GetString(lastNameIndex)
                        },
                        BadgeStartDate = reader.IsDBNull(badgeStartDateIndex) ? null : (DateTime?)reader.GetDateTime(badgeStartDateIndex),
                        BadgeEndDate = reader.IsDBNull(badgeEndDateIndex) ? null : (DateTime?)reader.GetDateTime(badgeEndDateIndex),
                        PlannedEndDate = reader.IsDBNull(plannedEndDateIndex) ? null : (DateTime?)reader.GetDateTime(plannedEndDateIndex),
                        BreakStartDate = reader.IsDBNull(breakStartDateIndex) ? null : (DateTime?)reader.GetDateTime(breakStartDateIndex),
                        BreakEndDate = reader.IsDBNull(breakEndDateIndex) ? null : (DateTime?)reader.GetDateTime(breakEndDateIndex),
                        IsBlocked = reader.GetBoolean(isBlockedIndex),
                        BlockStartDate = reader.IsDBNull(blockStartDateIndex) ? null : (DateTime?)reader.GetDateTime(blockStartDateIndex),
                        BlockEndDate = reader.IsDBNull(blockEndDateIndex) ? null : (DateTime?)reader.GetDateTime(blockEndDateIndex),
                        IsPreviousBadge = reader.GetBoolean(isPreviousBadgeIndex),
                        LastBadgeStartDate = reader.IsDBNull(lastBadgeStartDateIndex) ? null : (DateTime?)reader.GetDateTime(lastBadgeStartDateIndex),
                        LastBadgeEndDate = reader.IsDBNull(lastBadgeEndDateIndex) ? null : (DateTime?)reader.GetDateTime(lastBadgeEndDateIndex),
                        IsException = reader.GetBoolean(isExceptionIndex),
                        ExceptionStartDate = reader.IsDBNull(exceptionStartDateIndex) ? null : (DateTime?)reader.GetDateTime(exceptionStartDateIndex),
                        ExceptionEndDate = reader.IsDBNull(exceptionEndDateIndex) ? null : (DateTime?)reader.GetDateTime(exceptionEndDateIndex),
                        PreviousBadgeAlias = reader.IsDBNull(previousBadgeAliasIndex) ? string.Empty : reader.GetString(previousBadgeAliasIndex),
                        BadgeStartDateSource = reader.IsDBNull(badgeStartDateSourceIndex) ? string.Empty : reader.GetString(badgeStartDateSourceIndex),
                        PlannedEndDateSource = reader.IsDBNull(plannedEndDateSourceIndex) ? string.Empty : reader.GetString(plannedEndDateSourceIndex),
                        BadgeEndDateSource = reader.IsDBNull(badgeEndDateSourceIndex) ? string.Empty : reader.GetString(badgeEndDateSourceIndex),
                        DeactivatedDate = reader.IsDBNull(deactivatedDateIndex) ? null : (DateTime?)reader.GetDateTime(deactivatedDateIndex),
                        OrganicBreakStartDate = reader.IsDBNull(organicBreakStartDateIndex) ? null : (DateTime?)reader.GetDateTime(organicBreakStartDateIndex),
                        OrganicBreakEndDate = reader.IsDBNull(organicBreakEndDateIndex) ? null : (DateTime?)reader.GetDateTime(organicBreakEndDateIndex),
                        ExcludeInReports = reader.GetBoolean(excludeInReportsIndex),
                        IsMSManagedService = reader.GetBoolean(manageServiceContractIndex)
                    };
                    result.Add(badge);
                }
            }
        }

        public static List<MSBadge> GetLogic2020BadgeHistory(int personId)
        {
            var result = new List<MSBadge>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetLogic2020BadgeHistory, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadBadgehistoryDetails(reader, result);
                }
            }
            return result;
        }

        private static void ReadBadgehistoryDetails(SqlDataReader reader, List<MSBadge> result)
        {
            if (reader.HasRows)
            {
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
                int badgeStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeStartDate);
                int badgeEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeEndDate);
                int badgeDurationIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeDuration);
                int isApprovedIndex = reader.GetOrdinal(Constants.ColumnNames.IsApproved);
                int projectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusId);

                while (reader.Read())
                {
                    var badge = new MSBadge()
                    {
                        BadgeStartDate = reader.IsDBNull(badgeStartDateIndex) ? null : (DateTime?)reader.GetDateTime(badgeStartDateIndex),
                        BadgeEndDate = reader.IsDBNull(badgeEndDateIndex) ? null : (DateTime?)reader.GetDateTime(badgeEndDateIndex),
                        Project = new Project()
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            Name = reader.GetString(projectNameIndex),
                            ProjectNumber = reader.GetString(projectNumberIndex),
                            StartDate = reader.GetDateTime(startDateIndex),
                            EndDate = reader.GetDateTime(endDateIndex),
                            Status = new ProjectStatus()
                            {
                                Id = reader.GetInt32(projectStatusIdIndex)
                            }
                        },
                        BadgeDuration = reader.GetInt32(badgeDurationIndex),
                        IsApproved = reader.GetBoolean(isApprovedIndex)
                    };
                    result.Add(badge);
                }
            }
        }

        public static void SaveBadgeDetailsByPersonId(MSBadge msBadge)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.SaveBadgeDetailsByPersonId, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(PersonIdParam, msBadge.Person.Id.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsBlocked, msBadge.IsBlocked);
                command.Parameters.AddWithValue(Constants.ParameterNames.BlockStartDate, msBadge.BlockStartDate.HasValue ? (object)msBadge.BlockStartDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.BlockEndDate, msBadge.BlockEndDate.HasValue ? (object)msBadge.BlockEndDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsPreviousBadge, msBadge.IsPreviousBadge);
                command.Parameters.AddWithValue(Constants.ParameterNames.PreviousBadgeAlias, string.IsNullOrEmpty(msBadge.PreviousBadgeAlias) ? DBNull.Value : (object)msBadge.PreviousBadgeAlias);
                command.Parameters.AddWithValue(Constants.ParameterNames.LastBadgeStartDate, msBadge.LastBadgeStartDate.HasValue ? (object)msBadge.LastBadgeStartDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.LastBadgeEndDate, msBadge.LastBadgeEndDate.HasValue ? (object)msBadge.LastBadgeEndDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsException, msBadge.IsException);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExceptionStartDate, msBadge.ExceptionStartDate.HasValue ? (object)msBadge.ExceptionStartDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExceptionEndDate, msBadge.ExceptionEndDate.HasValue ? (object)msBadge.ExceptionEndDate.Value : DBNull.Value);

                command.Parameters.AddWithValue(Constants.ParameterNames.BadgeStartDate, msBadge.BadgeStartDate.HasValue ? (object)msBadge.BadgeStartDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.BadgeEndDate, msBadge.BadgeEndDate.HasValue ? (object)msBadge.BadgeEndDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDateSource, msBadge.BadgeStartDateSource);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDateSource, msBadge.BadgeEndDateSource);
                command.Parameters.AddWithValue(Constants.ParameterNames.BreakStartDate, msBadge.BreakStartDate.HasValue ? (object)msBadge.BreakStartDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.BreakEndDate, msBadge.BreakEndDate.HasValue ? (object)msBadge.BreakEndDate.Value : DBNull.Value);

                command.Parameters.AddWithValue(Constants.ParameterNames.DeactivatedDate, msBadge.DeactivatedDate.HasValue ? (object)msBadge.DeactivatedDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.OrganicBreakStart, msBadge.OrganicBreakStartDate.HasValue ? (object)msBadge.OrganicBreakStartDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.OrganicBreakEnd, msBadge.OrganicBreakEndDate.HasValue ? (object)msBadge.OrganicBreakEndDate.Value : DBNull.Value);

                command.Parameters.AddWithValue(Constants.ParameterNames.ExcludeFromReports, msBadge.ExcludeInReports);
                command.Parameters.AddWithValue(Constants.ParameterNames.UpdatedBy, msBadge.ModifiedById);
                command.Parameters.AddWithValue(Constants.ParameterNames.ManageServiceContract, msBadge.IsMSManagedService);
                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public static void UpdateMSBadgeDetailsByPersonId(int personId, int updatedBy)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.UpdateMSBadgeDetailsByPersonId, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(PersonIdParam, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.UpdatedBy, updatedBy);
                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public static bool CheckIfPersonInProjectForDates(int personId, DateTime startDate, DateTime endDate)
        {
            bool result;
            try
            {
                using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.CheckIfPersonInProjectForDates, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);

                    connection.Open();
                    result = Convert.ToBoolean(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                result = true;
            }

            return result;
        }

        public static bool CheckIfPersonIsRestrictedByProjectId(int personId, int projectId, DateTime chargeDate)
        {
            bool result;
            try
            {
                using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.CheckIfPersonIsRestrictedByProjectId, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, projectId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ChargeDate, chargeDate);

                    connection.Open();
                    result = Convert.ToBoolean(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                result = true;
            }

            return result;
        }

        public static PersonBadgeHistories GetBadgeHistoryByPersonId(int personId)
        {
            var result = new PersonBadgeHistories();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetBadgeHistoryByPersonId, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadBlockHistoryDetails(reader, result);
                    reader.NextResult();
                    ReadOverrideHistoryDetails(reader, result);
                    reader.NextResult();
                    ReadBadgeHistoryDetails(reader, result);
                }
            }
            return result;
        }

        private static void ReadBlockHistoryDetails(SqlDataReader reader, PersonBadgeHistories result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int firstnameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int blockStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BlockStartDate);
                int blockEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BlockEndDate);
                int modifiedDateIndex = reader.GetOrdinal(Constants.ColumnNames.ModifiedDate);
                int modifiedByIndex = reader.GetOrdinal(Constants.ColumnNames.ModifiedBy);

                while (reader.Read())
                {
                    var personId = reader.GetInt32(personIdIndex);
                    var person = new Person()
                    {
                        Id = reader.GetInt32(personIdIndex),
                        FirstName = reader.GetString(firstnameIndex),
                        LastName = reader.GetString(lastNameIndex)
                    };
                    var badge = new MSBadge()
                    {
                        BlockStartDate = reader.IsDBNull(blockStartDateIndex) ? null : (DateTime?)reader.GetDateTime(blockStartDateIndex),
                        BlockEndDate = reader.IsDBNull(blockEndDateIndex) ? null : (DateTime?)reader.GetDateTime(blockEndDateIndex),
                        ModifiedBy = reader.GetString(modifiedByIndex),
                        ModifiedDate = reader.GetDateTime(modifiedDateIndex)
                    };
                    if (result.Person == null)
                    {
                        result.Person = person;
                        result.BlockHistory = new List<MSBadge>() { badge };
                    }
                    else
                    {
                        result.BlockHistory.Add(badge);
                    }
                }
            }
        }

        private static void ReadOverrideHistoryDetails(SqlDataReader reader, PersonBadgeHistories result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int firstnameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int overrideStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.OverrideStartDate);
                int overrideEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.OverrideEndDate);
                int modifiedDateIndex = reader.GetOrdinal(Constants.ColumnNames.ModifiedDate);
                int modifiedByIndex = reader.GetOrdinal(Constants.ColumnNames.ModifiedBy);

                while (reader.Read())
                {
                    var personId = reader.GetInt32(personIdIndex);
                    var person = new Person()
                    {
                        Id = reader.GetInt32(personIdIndex),
                        FirstName = reader.GetString(firstnameIndex),
                        LastName = reader.GetString(lastNameIndex)
                    };
                    var badge = new MSBadge()
                    {
                        ExceptionStartDate = reader.IsDBNull(overrideStartDateIndex) ? null : (DateTime?)reader.GetDateTime(overrideStartDateIndex),
                        ExceptionEndDate = reader.IsDBNull(overrideEndDateIndex) ? null : (DateTime?)reader.GetDateTime(overrideEndDateIndex),
                        ModifiedBy = reader.IsDBNull(modifiedByIndex) ? string.Empty : reader.GetString(modifiedByIndex),
                        ModifiedDate = reader.GetDateTime(modifiedDateIndex)
                    };
                    if (result.Person == null)
                    {
                        result.Person = person;
                        result.OverrideHistory = new List<MSBadge>() { badge };
                    }
                    else
                    {
                        if (result.OverrideHistory == null)
                            result.OverrideHistory = new List<MSBadge>();
                        result.OverrideHistory.Add(badge);
                    }
                }
            }
        }

        private static void ReadBadgeHistoryDetails(SqlDataReader reader, PersonBadgeHistories result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int firstnameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int badgeStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeStartDate);
                int projectPlannedEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectPlannedEndDate);
                int badgeEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeEndDate);
                int BreakStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BreakStartDate);
                int BreakEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BreakEndDate);
                int BadgeStartDateSourceIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeStartDateSource);
                int ProjectPlannedEndDateSourceIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectPlannedEndDateSource);
                int badgeEndDateSourceIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeEndDateSource);
                int modifiedDateIndex = reader.GetOrdinal(Constants.ColumnNames.ModifiedDate);
                int modifiedByIndex = reader.GetOrdinal(Constants.ColumnNames.ModifiedBy);

                while (reader.Read())
                {
                    var personId = reader.GetInt32(personIdIndex);
                    var person = new Person()
                    {
                        Id = reader.GetInt32(personIdIndex),
                        FirstName = reader.GetString(firstnameIndex),
                        LastName = reader.GetString(lastNameIndex)
                    };
                    var badge = new MSBadge()
                    {
                        BadgeStartDate = reader.IsDBNull(badgeStartDateIndex) ? null : (DateTime?)reader.GetDateTime(badgeStartDateIndex),
                        BadgeEndDate = reader.IsDBNull(badgeEndDateIndex) ? null : (DateTime?)reader.GetDateTime(badgeEndDateIndex),
                        PlannedEndDate = reader.IsDBNull(projectPlannedEndDateIndex) ? null : (DateTime?)reader.GetDateTime(projectPlannedEndDateIndex),
                        BreakStartDate = reader.IsDBNull(BreakStartDateIndex) ? null : (DateTime?)reader.GetDateTime(BreakStartDateIndex),
                        BreakEndDate = reader.IsDBNull(BreakEndDateIndex) ? null : (DateTime?)reader.GetDateTime(BreakEndDateIndex),
                        BadgeStartDateSource = reader.IsDBNull(BadgeStartDateSourceIndex) ? string.Empty : reader.GetString(BadgeStartDateSourceIndex),
                        PlannedEndDateSource = reader.IsDBNull(ProjectPlannedEndDateSourceIndex) ? string.Empty : reader.GetString(ProjectPlannedEndDateSourceIndex),
                        BadgeEndDateSource = reader.IsDBNull(badgeEndDateSourceIndex) ? string.Empty : reader.GetString(badgeEndDateSourceIndex),
                        ModifiedBy = reader.IsDBNull(modifiedByIndex) ? string.Empty : reader.GetString(modifiedByIndex),
                        ModifiedDate = reader.GetDateTime(modifiedDateIndex)
                    };
                    if (result.Person == null)
                    {
                        result.Person = person;
                        result.BadgeHistory = new List<MSBadge>() { badge };
                    }
                    else
                    {
                        if (result.BadgeHistory == null)
                            result.BadgeHistory = new List<MSBadge>();
                        result.BadgeHistory.Add(badge);
                    }
                }
            }
        }

        public static bool CheckIfPersonInProjectsForThisPeriod(DateTime? modifiedEndDate, DateTime? oldEndDate, DateTime? modifiedStartDate, DateTime? oldStartDate, int personId)
        {
            bool result;
            try
            {
                using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
                using (var command = new SqlCommand(Constants.ProcedureNames.Person.CheckIfPersonInProjectsForThisPeriod, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ModifiedEndDate, modifiedEndDate.HasValue ? (object)modifiedEndDate : DBNull.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.OldEndDate, oldEndDate.HasValue ? (object)oldEndDate : DBNull.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ModifiedStartDate, modifiedStartDate.HasValue ? (object)modifiedStartDate : DBNull.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.OldStartDate, oldStartDate.HasValue ? (object)oldStartDate : DBNull.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);

                    connection.Open();
                    result = Convert.ToBoolean(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                result = true;
            }

            return result;
        }

        public static List<MSBadge> GetBadgeRecordsAfterDeactivatedDate(int personId, DateTime deactivatedDate)
        {
            var result = new List<MSBadge>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetBadgeRecordsAfterDeactivatedDate, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.DeactivatedDate, deactivatedDate);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadBadgeRecordsAfterDeactivatedDate(reader, result);
                }
            }
            return result;
        }

        private static void ReadBadgeRecordsAfterDeactivatedDate(SqlDataReader reader, List<MSBadge> result)
        {
            if (reader.HasRows)
            {

                int badgeStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeStartDate);
                int badgeEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeEndDate);

                while (reader.Read())
                {
                    var badge = new MSBadge()
                    {
                        BadgeStartDate = reader.IsDBNull(badgeStartDateIndex) ? null : (DateTime?)reader.GetDateTime(badgeStartDateIndex),
                        BadgeEndDate = reader.IsDBNull(badgeEndDateIndex) ? null : (DateTime?)reader.GetDateTime(badgeEndDateIndex),
                    };
                    result.Add(badge);
                }
            }
        }

        public static List<MSBadge> GetBadgeRecordsByProjectId(int projectId)
        {
            var result = new List<MSBadge>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetBadgeRecordsByProjectId, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, projectId);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadBadgeRecordsByProjectId(reader, result);
                }
            }
            return result;
        }

        private static void ReadBadgeRecordsByProjectId(SqlDataReader reader, List<MSBadge> result)
        {
            if (reader.HasRows)
            {
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int badgeStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeStartDate);
                int badgeEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeEndDate);
                int IsBadgeExceptionIndex = reader.GetOrdinal(Constants.ColumnNames.IsBadgeException);
                int MilestoneIdIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneId);
                int MilestoneDescriptionIndex = reader.GetOrdinal(Constants.ColumnNames.DescriptionColumn);

                while (reader.Read())
                {
                    var badge = new MSBadge()
                    {
                        Project = new Project()
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            Name = reader.GetString(projectNameIndex),
                            ProjectNumber = reader.GetString(projectNumberIndex)
                        },
                        Person = new Person()
                        {
                            Id = reader.GetInt32(personIdIndex),
                            FirstName = reader.GetString(firstNameIndex),
                            LastName = reader.GetString(lastNameIndex)
                        },
                        BadgeStartDate = reader.IsDBNull(badgeStartDateIndex) ? null : (DateTime?)reader.GetDateTime(badgeStartDateIndex),
                        BadgeEndDate = reader.IsDBNull(badgeEndDateIndex) ? null : (DateTime?)reader.GetDateTime(badgeEndDateIndex),
                        IsException = reader.GetBoolean(IsBadgeExceptionIndex),
                        Milestone = new Milestone()
                        {
                            Id = reader.GetInt32(MilestoneIdIndex),
                            Description = reader.GetString(MilestoneDescriptionIndex)
                        }
                    };
                    result.Add(badge);
                }
            }
        }

        public static bool IsPersonSalaryTypeInGivenRange(int personId, DateTime startDate, DateTime endDate)
        {
            bool result;
            try
            {
                using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
                using (var command = new SqlCommand(Constants.ProcedureNames.Person.IsPersonSalaryTypeInGivenRange, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);

                    connection.Open();
                    result = Convert.ToBoolean(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                result = true;
            }

            return result;
        }

        public static List<bool> CheckIfDatesInDeactivationHistory(int personId, DateTime startDate, DateTime endDate)
        {
            var result = new List<bool>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.CheckIfDatesInDeactivationHistory, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadBadgeBreaks(reader, result);
                }
            }
            return result;
        }

        private static void ReadBadgeBreaks(SqlDataReader reader, List<bool> result)
        {
            if (reader.HasRows)
            {
                int organicBreaksExistsIndex = reader.GetOrdinal(Constants.ColumnNames.OrganicBreaksExists);
                int badgeBreakExistsIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeBreakExists);

                while (reader.Read())
                {
                    result.Add(reader.GetBoolean(organicBreaksExistsIndex));
                    result.Add(reader.GetBoolean(badgeBreakExistsIndex));
                }
            }
        }

        #endregion

        public static List<Person> GetUsersForCF()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                return GetUsersForCF(connection);
            }
        }

        public static List<Person> GetUsersForCF(SqlConnection connection)
        {
            var result = new List<Person>();

            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetUsersForCF, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadUsersForCF(reader, result);
                    reader.NextResult();
                    ReadUsersFeedbacksForCF(reader, result);
                    reader.NextResult();
                    ReadUsersErrorFeedbacksForCF(reader, result);
                }
            }
            return result;
        }

        private static void ReadUsersForCF(SqlDataReader reader, List<Person> result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int employeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.EmployeeNumber);
                int firstnameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int preferdFirstnameIndex = reader.GetOrdinal(Constants.ColumnNames.PreferredFirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int paychexIDIndex = reader.GetOrdinal(Constants.ColumnNames.PaychexID);
                int aliasIndex = reader.GetOrdinal(Constants.ColumnNames.Alias);
                int locationCodeIndex = reader.GetOrdinal(Constants.ColumnNames.LocationCode);
                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int positionIdIndex = reader.GetOrdinal(Constants.ColumnNames.PositionId);
                //int timeScaleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeScaleId);
                int timescaleCodeIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleCode);
                int hireDateColumnIndex = reader.GetOrdinal(Constants.ColumnNames.HireDateColumn);
                int managerEmployeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ManagerEmployeeNumber);
                int cFDivisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.CFDivisionId);
                int divisionCodeIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionCode);
                int practiceLeadershipEmployeeNumberIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeLeadershipEmployeeNumber);
                int practiceLeadershipIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeLeadershipId);
                int isMBOIndex = reader.GetOrdinal(Constants.ColumnNames.IsMBO);
                int practiceDirectorIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeDirector);
                int practiceDirectorIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeDirectorId);

                while (reader.Read())
                {
                    Person person = new Person()
                    {
                        Id = reader.GetInt32(personIdIndex),
                        FirstName = reader.GetString(firstnameIndex),
                        PrefferedFirstName = reader.IsDBNull(preferdFirstnameIndex) ? string.Empty : reader.GetString(preferdFirstnameIndex),
                        LastName = reader.GetString(lastNameIndex),
                        PaychexID = reader.IsDBNull(paychexIDIndex) ? string.Empty : reader.GetString(paychexIDIndex),
                        Alias = reader.GetString(aliasIndex),
                        Location = new Location() { LocationCode = reader.IsDBNull(locationCodeIndex) ? string.Empty : reader.GetString(locationCodeIndex) },
                        Title = new Title() { TitleId = reader.GetInt32(titleIdIndex), PositionId = reader.IsDBNull(positionIdIndex) ? -1 : reader.GetInt32(positionIdIndex) },
                        CurrentPay = new Pay() { TimescaleCode = reader.GetString(timescaleCodeIndex) },
                        HireDate = reader.GetDateTime(hireDateColumnIndex),
                        Manager = new Person()
                        {
                            EmployeeNumber = reader.IsDBNull(managerEmployeeNumberIndex) ? string.Empty : reader.GetString(managerEmployeeNumberIndex)
                        },
                        EmployeeNumber = reader.GetString(employeeNumberIndex),
                        PracticeLeadership = new Person()
                        {
                            Id = reader.IsDBNull(practiceLeadershipIdIndex) ? -1 : reader.GetInt32(practiceLeadershipIdIndex),
                            EmployeeNumber = reader.IsDBNull(practiceLeadershipEmployeeNumberIndex) ? string.Empty : reader.GetString(practiceLeadershipEmployeeNumberIndex)
                        },
                        PracticeDirectorEmployeeNumber = reader.IsDBNull(practiceDirectorIndex) ? string.Empty : reader.GetString(practiceDirectorIndex),
                        PracticeDirectorId = reader.IsDBNull(practiceDirectorIdIndex) ? -1 : reader.GetInt32(practiceDirectorIdIndex),

                        IsMBO = reader.GetBoolean(isMBOIndex)
                    };
                    if (!reader.IsDBNull(cFDivisionIdIndex))
                    {
                        person.CFDivision = new DataTransferObjects.CornerStone.DivisionCF()
                        {
                            DivisionId = reader.GetInt32(cFDivisionIdIndex),
                            DivisionCode = reader.GetString(divisionCodeIndex)
                        };
                    }
                    person.ErrorFeedbacks = new List<ProjectFeedback>();
                    result.Add(person);
                }
            }
        }

        private static void ReadUsersFeedbacksForCF(SqlDataReader reader, List<Person> result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                int reviewStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.ReviewStartDate);
                int reviewEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.ReviewEndDate);
                int projectManagerUserIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerUserId);
                int engagementManagerUserIdIndex = reader.GetOrdinal(Constants.ColumnNames.EngagementManagerUserId);
                int executiveInChargeUserIdIndex = reader.GetOrdinal(Constants.ColumnNames.ExecutiveInChargeUserId);
                int projectManagerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerId);
                int engagementManagerIdIndex = reader.GetOrdinal(Constants.ColumnNames.EngagementManagerId);
                int executiveInChargeIdIndex = reader.GetOrdinal(Constants.ColumnNames.ExecutiveInChargeId);
                int countIndex = reader.GetOrdinal(Constants.ColumnNames.Count);

                while (reader.Read())
                {
                    var personId = reader.GetInt32(personIdIndex);
                    var person = result.FirstOrDefault(p => p.Id.Value == personId);
                    var feedback = new ProjectFeedback()
                    {
                        Project = new Project()
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            ProjectNumber = reader.GetString(projectNumberIndex),
                            Name = reader.GetString(projectNameIndex),
                            EngagementManagerID = reader.IsDBNull(engagementManagerIdIndex) ? -1 : reader.GetInt32(engagementManagerIdIndex),
                            ProjectManagerId = reader.IsDBNull(projectManagerIdIndex) ? -1 : reader.GetInt32(projectManagerIdIndex),
                            ExecutiveInChargeId = reader.IsDBNull(executiveInChargeIdIndex) ? -1 : reader.GetInt32(executiveInChargeIdIndex),
                            EngagementManagerUserID = reader.IsDBNull(engagementManagerUserIdIndex) ? string.Empty : reader.GetString(engagementManagerUserIdIndex),
                            ProjectManagerUserId = reader.IsDBNull(projectManagerUserIdIndex) ? string.Empty : reader.GetString(projectManagerUserIdIndex),
                            ExecutiveInChargeUserId = reader.IsDBNull(executiveInChargeUserIdIndex) ? string.Empty : reader.GetString(executiveInChargeUserIdIndex),
                        },
                        ReviewStartDate = reader.GetDateTime(reviewStartDateIndex),
                        ReviewEndDate = reader.GetDateTime(reviewEndDateIndex),
                        Count = reader.GetInt32(countIndex)
                    };
                    if (person.ProjectFeedbacks != null)
                    {
                        person.ProjectFeedbacks.Add(feedback);
                    }
                    else
                    {
                        person.ProjectFeedbacks = new List<ProjectFeedback>() { feedback };
                    }
                }
            }
        }

        private static void ReadUsersErrorFeedbacksForCF(SqlDataReader reader, List<Person> result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                int reviewStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.ReviewStartDate);
                int reviewEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.ReviewEndDate);
                //int projectManagerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerId);
                int projectManagerIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManager);

                while (reader.Read())
                {
                    var personId = reader.GetInt32(personIdIndex);
                    var person = result.FirstOrDefault(p => p.Id.Value == personId);
                    var feedback = new ProjectFeedback()
                    {
                        Project = new Project()
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            ProjectNumber = reader.GetString(projectNumberIndex),
                            Name = reader.GetString(projectNameIndex),
                            ProjectManagerNames = reader.GetString(projectManagerIndex)
                        },
                        ReviewStartDate = reader.GetDateTime(reviewStartDateIndex),
                        ReviewEndDate = reader.GetDateTime(reviewEndDateIndex)
                    };
                    feedback.Person = new Person()
                    {
                        Id = person.Id,
                        FirstName = person.FirstName,
                        LastName = person.LastName
                    };
                    if (person.ErrorFeedbacks != null)
                    {
                        person.ErrorFeedbacks.Add(feedback);
                    }
                    else
                    {
                        person.ErrorFeedbacks = new List<ProjectFeedback>() { feedback };
                    }
                }
            }
        }

        public static List<Timescale> GetSalaryPayTypes()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                return GetSalaryPayTypes(connection);
            }
        }

        public static List<Timescale> GetSalaryPayTypes(SqlConnection connection)
        {
            var result = new List<Timescale>();

            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetSalaryPayTypes, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadSalaryPayTypes(reader, result);
                }
            }
            return result;
        }

        private static void ReadSalaryPayTypes(SqlDataReader reader, List<Timescale> result)
        {
            if (reader.HasRows)
            {
                int timeScaleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeScaleId);
                int timescaleIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleColumn);
                int timescaleCodeIndex = reader.GetOrdinal(Constants.ColumnNames.TimescaleCode);

                while (reader.Read())
                {
                    var timescale = new Timescale()
                    {
                        Id = reader.GetInt32(timeScaleIdIndex),
                        Name = reader.GetString(timescaleIndex),
                        TimescaleCode = reader.IsDBNull(timescaleCodeIndex) ? string.Empty : reader.GetString(timescaleCodeIndex)
                    };
                    result.Add(timescale);
                }
            }
        }

        public static List<Person> GetPracticeLeaderships(int? divisionId)
        {
            var result = new List<Person>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetPracticeLeaderships, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.DivisionId, divisionId.HasValue ? (object)divisionId.Value : DBNull.Value);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    ReadPracticeLeaderships(reader, result);
                }
            }
            return result;
        }

        private static void ReadPracticeLeaderships(SqlDataReader reader, List<Person> result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);
                int divisionNameIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionName);

                while (reader.Read())
                {
                    var person = new Person()
                    {
                        Id = reader.GetInt32(personIdIndex),
                        FirstName = reader.GetString(firstNameIndex),
                        LastName = reader.GetString(lastNameIndex),
                        DivisionType = (PersonDivisionType)reader.GetInt32(divisionIdIndex)
                    };
                    result.Add(person);
                }
            }
        }

        public static void UpdatePersonDivision(PersonDivision division)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.UpdatePersonDivision, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.DivisionId, division.DivisionId);
                command.Parameters.AddWithValue(Constants.ParameterNames.DivisionOwnerId, division.DivisionOwner.Id);
                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public static PersonDivision GetPersonDivisionById(int divisionId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetPersonDivisionById, connection))
            {

                command.CommandTimeout = connection.ConnectionTimeout;

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.DivisionId, divisionId);
                connection.Open();
                var result = new List<PersonDivision>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return null;
                    }
                    else
                    {
                        ReadPersonDivisions(reader, result);
                        return result[0];
                    }

                }
            }
        }

        public static List<PersonDivision> GetPersonDivisions()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetPersonDivisions, connection))
            {
                command.CommandTimeout = connection.ConnectionTimeout;

                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var result = new List<PersonDivision>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        result = null;
                    }
                    else
                    {
                        ReadPersonDivisions(reader, result);
                    }
                    return result;
                }
            }
        }

        private static void ReadPersonDivisions(SqlDataReader reader, List<PersonDivision> result)
        {
            if (reader.HasRows)
            {
                int divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);
                int divisionNameIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionName);
                int InactiveIndex = reader.GetOrdinal(Constants.ColumnNames.Inactive);
                int showSetPracticeOwnerLinkIndex = -1;
                int divisionOwnerIdIndex = -1;
                int divisionOwnerFirstNameIndex = -1;
                int divisionOwnerLastNameIndex = -1;
                try
                {
                    showSetPracticeOwnerLinkIndex = reader.GetOrdinal(Constants.ColumnNames.ShowSetPracticeOwnerLink);
                }
                catch
                {
                    showSetPracticeOwnerLinkIndex = -1;
                }

                try
                {
                    divisionOwnerIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                }
                catch
                {
                    divisionOwnerIdIndex = -1;
                }

                try
                {
                    divisionOwnerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                }
                catch
                {
                    divisionOwnerFirstNameIndex = -1;
                }

                try
                {
                    divisionOwnerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                }
                catch
                {
                    divisionOwnerLastNameIndex = -1;
                }
                while (reader.Read())
                {
                    var division = new PersonDivision();
                    division.DivisionId = reader.GetInt32(divisionIdIndex);
                    division.DivisionName = reader.GetString(divisionNameIndex);
                    division.Inactive = reader.GetBoolean(InactiveIndex);
                    if (showSetPracticeOwnerLinkIndex > -1)
                    {
                        division.ShowCareerManagerLink = reader.GetBoolean(showSetPracticeOwnerLinkIndex);
                    }
                    if (divisionOwnerIdIndex > -1)
                    {
                        if (!reader.IsDBNull(divisionOwnerIdIndex))
                        {
                            division.DivisionOwner = new Person
                            {
                                Id = reader.GetInt32(divisionOwnerIdIndex),
                                FirstName = reader.GetString(divisionOwnerFirstNameIndex),
                                LastName = reader.GetString(divisionOwnerLastNameIndex)
                            };
                        }

                    }
                    result.Add(division);
                }
            }
        }

        public static List<Owner> CheckIfPersonIsOwnerForDivisionAndOrPractice(int personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.CheckIfPersonIsOwner, connection))
            {
                command.CommandTimeout = connection.ConnectionTimeout;

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);
                connection.Open();
                var result = new List<Owner>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        result = null;
                    }
                    else
                    {
                        ReadDivisionOrPractice(reader, result);
                    }
                    return result;
                }
            }
        }

        private static void ReadDivisionOrPractice(SqlDataReader reader, List<Owner> result)
        {
            if (reader.HasRows)
            {
                int targetIndex = reader.GetOrdinal(Constants.ColumnNames.TargetName);
                int isDivisionOwnerIndex = reader.GetOrdinal(Constants.ColumnNames.IsDivisionOwner);
                while (reader.Read())
                {
                    Owner owner = new Owner()
                    {
                        Target = reader.GetString(targetIndex),
                        IsDivisionOwner = reader.GetBoolean(isDivisionOwnerIndex)
                    };
                    result.Add(owner);
                }
            }
        }

        public static void SaveReportFilterValues(int currentUserId, int reportId, string reportFilters, int previousUserId, string sessionId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.SaveReportFilterValues, connection))
            {
                command.CommandTimeout = connection.ConnectionTimeout;

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.CurrentUserId, currentUserId);
                command.Parameters.AddWithValue(Constants.ParameterNames.ReportId, reportId);
                command.Parameters.AddWithValue(Constants.ParameterNames.ReportFilters, reportFilters);
                command.Parameters.AddWithValue(Constants.ParameterNames.PreviousUserId, previousUserId);
                command.Parameters.AddWithValue(Constants.ParameterNames.SessionId, sessionId);
                connection.Open();
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static string GetReportFilterValues(int currentUserId, int reportId, int previousUserId, string sessionId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetReportFilterValues, connection))
            {
                command.CommandTimeout = connection.ConnectionTimeout;

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.CurrentUserId, currentUserId);
                command.Parameters.AddWithValue(Constants.ParameterNames.ReportId, reportId);
                command.Parameters.AddWithValue(Constants.ParameterNames.PreviousUserId, previousUserId);
                command.Parameters.AddWithValue(Constants.ParameterNames.SessionId, sessionId);
                connection.Open();
                string result = null;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        result = null;
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            int reportFilterIndex = reader.GetOrdinal(Constants.ColumnNames.ReportFilters);
                            result = reader.GetString(reportFilterIndex);
                        }
                    }

                }
                return result;

            }
        }

        public static void DeleteReportFilterValues(int currentUserId, int previousUserId, string sessionId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.DeleteReportFilterValues, connection))
            {
                command.CommandTimeout = connection.ConnectionTimeout;

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(Constants.ParameterNames.CurrentUserId, currentUserId);
                command.Parameters.AddWithValue(Constants.ParameterNames.PreviousUserId, previousUserId);
                command.Parameters.AddWithValue(Constants.ParameterNames.SessionId, sessionId);
                connection.Open();
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public static Owner CheckIfPersonStatusCanChangeFromActiveToContingent(int personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.CheckIfPersonInProjectPracticeAreaAndDivision, connection))
            {
                command.CommandTimeout = connection.ConnectionTimeout;

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdParam, personId);
                connection.Open();
                Owner result = new Owner();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        result = null;
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            int IsDivisionOrPracticeOwnerIndex = reader.GetOrdinal(Constants.ColumnNames.IsDivisionOrPracticeOwner);
                            int IsAssignedToProjectIndex = reader.GetOrdinal(Constants.ColumnNames.IsAssignedToProject);
                            result.IsDivisionOwner = reader.GetBoolean(IsDivisionOrPracticeOwnerIndex);
                            result.isAssignedToProjects = reader.GetBoolean(IsAssignedToProjectIndex);
                        }
                    }

                }
                return result;
            }

        }

        public static List<ConsultantPTOHours> GetConsultantPTOEntries(DateTime startDate, DateTime endDate, bool includeActivePersons, bool includeContingentPersons, bool isW2Salary, bool isW2Hourly, string practiceIds, string divisionIds, string titleIds, int sortId, string sortDirection)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                List<ConsultantPTOHours> result = null;

                using (var command = new SqlCommand(Constants.ProcedureNames.Person.GetConsultantPTOEntries, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ActivePersons, includeActivePersons);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedPersons, includeContingentPersons);
                    command.Parameters.AddWithValue(Constants.ParameterNames.W2HourlyPersons, isW2Hourly);
                    command.Parameters.AddWithValue(Constants.ParameterNames.W2SalaryPersons, isW2Salary);
                    command.Parameters.AddWithValue(Constants.ParameterNames.SortId, sortId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.SortDirection, sortDirection);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIds);
                    command.Parameters.AddWithValue(Constants.ParameterNames.TitleIds, titleIds);
                    command.Parameters.AddWithValue(Constants.ParameterNames.DivisionIds, divisionIds);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        result = GetPersonsWithVactionDays(reader);
                        reader.NextResult();
                        ReadPTOPersonsEmploymentHistory(reader, result);
                        reader.NextResult();
                        ReadPersonPTOTimeOffDates(reader, result);
                    }
                }


                return result;
            }
        }

        private static List<ConsultantPTOHours> GetPersonsWithVactionDays(SqlDataReader reader)
        {
            if (reader.HasRows)
            {
                int personStatusIdIndex = reader.GetOrdinal(PersonStatusId);
                int personStatusNameIndex = reader.GetOrdinal(NameColumn);
                int firstNameIndex = reader.GetOrdinal(FirstNameColumn);
                int lastNameIndex = reader.GetOrdinal(LastNameColumn);
                int employeeNumberIndex = reader.GetOrdinal(EmployeeNumberColumn);
                int timescaleNameIndex = reader.GetOrdinal(TimescaleColumn);
                int timescaleIdIndex = reader.GetOrdinal(TimescaleIdColumn);
                int hireDateIndex = reader.GetOrdinal(HireDateColumn);
                int personVacationDaysIndex = reader.GetOrdinal(PersonVactionDaysColumn);
                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                int practiceId = -1;
                int practiceArea = -1;
                try
                {
                    practiceId = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                    practiceArea = reader.GetOrdinal(Constants.ColumnNames.PracticeNameColumn);
                }
                catch
                {
                }
                var res = new List<ConsultantPTOHours>();
                while (reader.Read())
                {
                    ConsultantPTOHours item = new ConsultantPTOHours();

                    var person =
                        new Person
                        {
                            Id = (int)reader[PersonIdColumn],
                            FirstName = (string)reader[firstNameIndex],
                            LastName = (string)reader[lastNameIndex],
                            EmployeeNumber = (string)reader[employeeNumberIndex],
                            Status = new PersonStatus
                            {
                                Id = (int)reader[personStatusIdIndex],
                                Name = (string)reader[personStatusNameIndex]
                            },
                            CurrentPay = new Pay
                            {
                                TimescaleName = reader.GetString(timescaleNameIndex),
                                Timescale = (TimescaleType)reader.GetInt32(timescaleIdIndex)
                            },
                            HireDate = (DateTime)reader[hireDateIndex],
                            Title = new Title
                            {
                                TitleId = reader.GetInt32(titleIdIndex),
                                TitleName = reader.GetString(titleIndex)
                            },

                        };

                    if (practiceId > -1)
                    {
                        person.DefaultPractice = new Practice()
                        {
                            Id = reader.GetInt32(titleIdIndex),
                            Name = reader.GetString(practiceArea)
                        };
                    }

                    if (Convert.IsDBNull(reader[TerminationDateColumn]))
                    {
                        person.TerminationDate = null;
                    }
                    else
                    {
                        person.TerminationDate = (DateTime)reader[TerminationDateColumn];
                    }

                    item.PTOOffDates = new SortedList<DateTime, double>();
                    item.Person = person;
                    item.PersonVacationDays = reader.GetInt32(personVacationDaysIndex);
                    res.Add(item);
                }

                return res;
            }
            return null;
        }

        private static void ReadPTOPersonsEmploymentHistory(SqlDataReader reader, List<ConsultantPTOHours> result)
        {
            if (result != null)
            {
                if (reader.HasRows)
                {
                    int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                    int hireDateIndex = reader.GetOrdinal(Constants.ColumnNames.HireDateColumn);
                    int terminationDateIndex = reader.GetOrdinal(Constants.ColumnNames.TerminationDateColumn);

                    while (reader.Read())
                    {
                        var employment = new Employment
                        {
                            PersonId = reader.GetInt32(personIdIndex),
                            HireDate = reader.GetDateTime(hireDateIndex),
                            TerminationDate =
                                reader.IsDBNull(terminationDateIndex)
                                    ? null
                                    : (DateTime?)reader.GetDateTime(terminationDateIndex)
                        };

                        Person person = null;
                        if (result.Any(p => p.Person.Id == employment.PersonId))
                        {
                            person = result.First(p => p.Person.Id == employment.PersonId).Person;
                            if (person.EmploymentHistory == null)
                            {
                                person.EmploymentHistory = new List<Employment>();
                            }
                        }
                        if (person != null)
                        {
                            person.EmploymentHistory.Add(employment);
                        }
                    }
                }
            }
        }


        private static void ReadPersonPTOTimeOffDates(SqlDataReader reader, List<ConsultantPTOHours> result)
        {
            if (result != null)
            {
                if (reader.HasRows)
                {
                    int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                    int timeOffDateIndex = reader.GetOrdinal(Constants.ColumnNames.DateColumn);
                    int isTimeOffIndex = reader.GetOrdinal(Constants.ColumnNames.IsTimeOff);
                    int PTODescriptionIndex = reader.GetOrdinal(Constants.ColumnNames.DescriptionColumn);
                    int timeOffHoursIndex = reader.GetOrdinal(Constants.ColumnNames.TimeOffHours);

                    while (reader.Read())
                    {
                        var personId = reader.GetInt32(personIdIndex);
                        var timeOffDate = reader.GetDateTime(timeOffDateIndex);
                        var offType = reader.GetInt32(isTimeOffIndex);
                        var timeOffHours = reader.GetDouble(timeOffHoursIndex);

                        string PTODescription = reader.IsDBNull(PTODescriptionIndex)
                                        ? string.Empty
                                        : reader.GetString(PTODescriptionIndex);
                        if (result.Any(p => p.Person.Id == personId))
                        {
                            var record = result.First(p => p.Person.Id == personId);
                            if (record.PTOOffDates == null)
                            {
                                record.PTOOffDates = new SortedList<DateTime, double>();
                            }
                            if (record.CompanyHolidayDates == null)
                            {
                                record.CompanyHolidayDates = new Dictionary<DateTime, string>();
                            }
                            if (record.LeaveOfAbsence == null)
                            {
                                record.LeaveOfAbsence = new Dictionary<DateTime, double>();
                            }

                            if (offType == 1)
                            {
                                record.PTOOffDates.Add(timeOffDate, timeOffHours);
                            }
                            else if (offType == 2)
                            {
                                record.LeaveOfAbsence.Add(timeOffDate, timeOffHours);
                            }
                            else
                            {
                                record.CompanyHolidayDates.Add(timeOffDate, PTODescription);
                            }
                        }
                    }
                }
            }
        }

    }
}


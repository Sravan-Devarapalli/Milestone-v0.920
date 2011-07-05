using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml;
using DataAccess.Other;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
using System.Security.Cryptography;
using System.Text;


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
        private const string PTODaysPerAnnumParam = "@PTODaysPerAnnum";
        private const string HireDateParam = "@HireDate";
        private const string TerminationDateParam = "@TerminationDate";
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
        private const string MilestoneIdParam = "@MilestoneId";
        private const string PersonStatusIdParam = "@PersonStatusId";
        private const string RoleNameParam = "@RoleName";
        private const string PersonStatusIdsListParam = "@PersonStatusIdsList";
        private const string ProjectIdParam = "@ProjectId";
        private const string LookedParam = "@Looked";
        private const string EmployeeNumberParam = "@EmployeeNumber";
        private const string UserIdParam = "@UserId";
        private const string PracticeManagerIdParam = "@PracticeManagerId";
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
        private const string TerminatedParam = "@Terminated";
        private const string InactiveParam = "@Inactive";
        private const string AlphabetParam = "@Alphabet";

        #endregion

        #region Stored Procedures

        private const string ConsultantUtilizationReportProcedure = "dbo.ConsultantUtilizationReport";
        private const string ConsultantUtilizationWeeklyProcedure = "dbo.ConsultantUtilizationWeekly";
        private const string ConsultantUtilizationDailyByPersonProcedure = "dbo.ConsultantUtilizationDailyByPerson";
        private const string PersonInsertProcedure = "dbo.PersonInsert";
        private const string PersonUpdateProcedure = "dbo.PersonUpdate";
        private const string PersonListAllSeniorityFilterProcedure = "dbo.PersonListAllSeniorityFilter";
        private const string PersonListAllShortProcedure = "dbo.PersonListAllShort";
        private const string PersonsGetBySeniorityAndStatusProcedure = "dbo.PersonsGetBySeniorityAndStatus";
        private const string PersonListShortByRoleAndStatusProcedure = "dbo.PersonListShortByRoleAndStatus";
        private const string PersonListByStatusListProcedure = "dbo.PersonListAllByStatusList";
        private const string GetPersonListByPersonIdListProcedure = "dbo.GetPersonListByPersonIds";
        private const string PersonListAllForMilestoneProcedure = "dbo.PersonListAllForMilestone";
        private const string PersonListRecruiterProcedure = "dbo.PersonListRecruiter";
        private const string PersonGetByIdProcedure = "dbo.PersonGetById";
        private const string PersonOverheadByPersonProcedure = "dbo.PersonOverheadByPerson";
        private const string PersonOverheadByTimescaleProcedure = "dbo.PersonOverheadByTimescale";
        private const string PersonGetCountProcedure = "dbo.PersonGetCount";
        private const string PersonGetCountByCommaSeparatedIdsListProcedure = "dbo.PersonGetCountByCommaSeparatedIdsList";
        private const string PersonListBenchExpenseProcedure = "dbo.PersonListBenchExpense";
        private const string UpdateLastPasswordChangedDateForPersonProcedure = "dbo.UpdateLastPasswordChangedDateForPerson";


        private const string MilestonePersonListOverheadByPersonProcedure =
            "dbo.MilestonePersonListOverheadByPerson";

        private const string PersonSetStatusProcedure = "dbo.PersonSetStatus";
        private const string PersonListSalespersonProcedure = "dbo.PersonListSalesperson";
        private const string PersonListPracticeManagerProcedure = "dbo.PersonListPracticeManager";
        private const string PersonListProjectOwnerProcedure = "dbo.PersonListProjectOwner";
        private const string PersonWorkDaysNumberProcedure = "dbo.PersonWorkDaysNumber";
        private const string PersonGetCountActiveProcedure = "dbo.PersonGetCountActive";
        private const string PersonGetByAliasProcedure = "dbo.PersonGetByAlias";
        private const string MembershipDeleteProcedure = "dbo.MembershipDelete";
        private const string aspnetUsersDeleteUserProcedure = "dbo.aspnet_Users_DeleteUser";
        private const string aspnetMembershipCreateUserProcedure = "dbo.aspnet_Membership_CreateUser";
        private const string MembershipAliasUpdateProcedure = "dbo.MembershipAliasUpdate";
        private const string PersonListSubordinatesProcedure = "dbo.PersonListSubordinates";
        private const string PersonOneOffListProcedure = "dbo.PersonOneOffList";
        private const string PersonEnsureIntegrityProcedure = "dbo.PersonEnsureIntegrity";
        private const string PersonGetExcelSetProcedure = "dbo.PersonExcelSet";
        private const string PermissionsGetAllowedClientsProcedure = "dbo.PermissionsGetAllowedClients";
        private const string PermissionsGetAllowedGroupsProcedure = "dbo.PermissionsGetAllowedGroups";
        private const string UserTemporaryCredentialsInsertProcedure = "dbo.UserTemporaryCredentialsInsert";
        private const string GetTemporaryCredentialsByUserNameProcedure = "dbo.GetTemporaryCredentialsByUserName";
        private const string SetNewPasswordForUserProcedure = "dbo.aspnet_Membership_SetPassword";
        private const string DeleteTemporaryCredentialsByUserNameProcedure = "dbo.DeleteTemporaryCredentialsByUserName";

        private const string PermissionsGetAllowedPracticeManagersProcedure =
            "dbo.PermissionsGetAllowedPracticeManagers";

        private const string PermissionsGetAllowedPracticesProcedure = "dbo.PermissionsGetAllowedPractices";
        private const string PermissionsGetAllowedSalespersonsProcedure = "dbo.PermissionsGetAllowedSalespersons";
        private const string PermissionsGetAllProcedure = "dbo.PermissionsGetAll";
        private const string PermissionsSetAllProcedure = "dbo.PermissionsSetAll";
        private const string PersonMilestoneWithFinancials = "dbo.PersonMilestoneWithFinancials";

        private const string PersonListAllSeniorityFilterWithPayProcedure = "dbo.PersonListAllSeniorityFilterWithCurrentPay";
        private const string PersonListAllSeniorityFilterWithPayByCommaSeparatedIdsListProcedure = "dbo.PersonListAllSeniorityFilterWithCurrentPayByCommaSeparatedIdsList";

        #endregion

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
        private const string EmployeesNumberColumn = "EmployeesNumber";
        private const string ConsultantsNumberColumn = "ConsultantsNumber";
        private const string AliasColumn = "Alias";
        private const string SeniorityIdColumn = "SeniorityId";
        private const string SeniorityNameColumn = "SeniorityName";
        private const string PTODaysPerAnnumColumn = "PTODaysPerAnnum";
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
        private const string TimesPaidPerMonthColumn = "TimesPaidPerMonth";
        private const string TermsColumn = "Terms";
        private const string VacationDaysColumn = "VacationDays";
        private const string BonusAmountColumn = "BonusAmount";
        private const string BonusHoursToCollectColumn = "BonusHoursToCollect";
        private const string IsYearBonusColumn = "IsYearBonus";
        private const string DefaultHoursPerDayColumn = "DefaultHoursPerDay";
        private const string PayPersonIdColumn = "PayPersonId";
        private const string PracticeNameColumn = "PracticeName";
        private const string IsMinimumLoadFactorColumn = "IsMinimumLoadFactor";
        private const string TimeScaleChangeStatusColumn = "TimeScaleChangeStatus";

        #endregion

        #region Functions

        private const string CompensationCoversMilestoneFunction = "dbo.CompensationCoversMilestone";
        private const string GetCurrentPayTypeFunction = "dbo.GetCurrentPayType";

        #endregion

        #region Queries

        private const string OneArgUdfFunctionQuery = "SELECT {0}({1})";

        private const string CompensationCoversMilestoneQuery = "SELECT {0}({1}, DEFAULT, DEFAULT)";
        private const string CompensationCoversExtendedMilestoneQuery = "SELECT {0}({1}, {2}, {3})";

        #endregion

        #endregion

        public static void SetAsDefaultManager(Person person)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.SetDefaultManager, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.DefaultManagerId, person.Id.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Checks if the person is a manager to somebody
        /// </summary>
        public static bool IsSomeonesManager(Person person)
        {
            string query = string.Format(
                Constants.Queries.SingleParameter,
                Constants.FunctionNames.IsSomeonesManager,
                Constants.ParameterNames.PersonId);

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(query, connection))
            {
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(
                    Constants.ParameterNames.PersonId,
                    person.Id.Value);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result == null)
                    return false;

                return (bool)result;
            }
        }

        /// <summary>
        /// Checks if the person is a manager to somebody
        /// </summary>
        public static Person[] ListManagersSubordinates(Person person)
        {
            var result = new List<Person>();

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(
                                        Constants.ProcedureNames.Person.ListManagersSubordinates,
                                        connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam, person.Id.Value);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    ReadPersonsShort(reader, result);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Set new manager
        /// </summary>
        public static void SetNewManager(Person oldManager, Person newManager)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.SetNewManager, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.OldManagerId, oldManager.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.NewManagerId, newManager.Id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Retrives consultans report
        /// </summary>
        /// <returns>An <see cref="Opportunity"/> object if found and null otherwise.</returns>
        public static DataSet GetConsultantUtilizationReport(ConsultantTableReportContext context)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(ConsultantUtilizationReportProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ActivePersons, context.ActivePersons);
                command.Parameters.AddWithValue(Constants.ParameterNames.ActiveProjects, context.ActiveProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedPersons, context.ProjectedPersons);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedProjects, context.ProjectedProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.InternalProjects, context.InternalProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExperimentalProjects, context.ExperimentalProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.Start, context.Start);
                command.Parameters.AddWithValue(Constants.ParameterNames.End, context.End);

                connection.Open();

                var adapter = new SqlDataAdapter(command);
                var dataset = new DataSet();
                adapter.Fill(dataset, "consultantsReport");

                return dataset;
            }
        }

        /// <summary>
        /// Retrives consultans report
        /// </summary>
        /// <returns>An <see cref="Opportunity"/> object if found and null otherwise.</returns>
        public static DataSet GetPersonMilestoneWithFinancials(int personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonMilestoneWithFinancials, connection))
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
        /// Retrives consultans report
        /// </summary>
        /// <returns>An <see cref="Opportunity"/> object if found and null otherwise.</returns>
        public static List<Quadruple<Person, int[], int, int>> GetConsultantUtilizationWeekly(ConsultantTimelineReportContext context)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(ConsultantUtilizationWeeklyProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ActivePersons, context.ActivePersons);
                command.Parameters.AddWithValue(Constants.ParameterNames.ActiveProjects, context.ActiveProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedPersons, context.ProjectedPersons);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedProjects, context.ProjectedProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExperimentalProjects, context.ExperimentalProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.InternalProjects, context.InternalProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.Start, context.Start);
                command.Parameters.AddWithValue(Constants.ParameterNames.Granularity, context.Granularity);
                command.Parameters.AddWithValue(Constants.ParameterNames.Period, context.Period);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, context.PracticeIdList);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimescaleIds, context.TimescaleIdList);
                command.Parameters.AddWithValue(Constants.ParameterNames.SortId, context.SortId);
                command.Parameters.AddWithValue(Constants.ParameterNames.SortDirection, context.SortDirection);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExcludeInternalPractices, context.ExcludeInternalPractices);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsSampleReport, context.IsSampleReport);

                connection.Open();

                return GetPersonLoadWithVactionDays(command);
            }
        }

        public static List<Triple<Person, int[], int>> ConsultantUtilizationDailyByPerson(int personId, ConsultantTimelineReportContext context)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(ConsultantUtilizationDailyByPersonProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ActiveProjects, context.ActiveProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedProjects, context.ProjectedProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.InternalProjects, context.InternalProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExperimentalProjects, context.ExperimentalProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.DaysForward, context.Period);
                command.Parameters.AddWithValue(Constants.ParameterNames.Start, context.Start);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);

                connection.Open();
                return GetPersonLoad(command);
            }
        }

        /// <summary>
        /// reads a dataset of persons into a collection
        /// </summary>
        private static List<Triple<Person, int[], int>> GetPersonLoad(SqlCommand command)
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    int personStatusNameIndex = reader.GetOrdinal(NameColumn);
                    int firstNameIndex = reader.GetOrdinal(FirstNameColumn);
                    int lastNameIndex = reader.GetOrdinal(LastNameColumn);
                    int employeeNumberIndex = reader.GetOrdinal(EmployeeNumberColumn);
                    int weeklyLoadIndex = reader.GetOrdinal(WeeklyUtilColumn);
                    int timescaleNameIndex = reader.GetOrdinal(TimescaleColumn);
                    int timescaleIdIndex = reader.GetOrdinal(TimescaleIdColumn);
                    int hireDateIndex = reader.GetOrdinal(HireDateColumn);
                    int avgUtilIndex = reader.GetOrdinal(AvgUtilColumn);

                    var res = new List<Triple<Person, int[], int>>();
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
                                    Id = (int)Enum.Parse(
                                        typeof(PersonStatusType),
                                        (string)reader[personStatusNameIndex]),
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
                        res.Add(new Triple<Person, int[], int>(person, load, avgUtil));
                    }

                    return res;
                }
            }

            return null;
        }


        /// <summary>
        /// reads a dataset of persons into a collection
        /// </summary>
        private static List<Quadruple<Person, int[], int, int>> GetPersonLoadWithVactionDays(SqlCommand command)
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    int personStatusNameIndex = reader.GetOrdinal(NameColumn);
                    int firstNameIndex = reader.GetOrdinal(FirstNameColumn);
                    int lastNameIndex = reader.GetOrdinal(LastNameColumn);
                    int employeeNumberIndex = reader.GetOrdinal(EmployeeNumberColumn);
                    int weeklyLoadIndex = reader.GetOrdinal(WeeklyUtilColumn);
                    int timescaleNameIndex = reader.GetOrdinal(TimescaleColumn);
                    int timescaleIdIndex = reader.GetOrdinal(TimescaleIdColumn);
                    int hireDateIndex = reader.GetOrdinal(HireDateColumn);
                    int avgUtilIndex = reader.GetOrdinal(AvgUtilColumn);
                    int personVacationDaysIndex = reader.GetOrdinal(PersonVactionDaysColumn);
                    int seniorityIdColumnIndex = reader.GetOrdinal(SeniorityIdColumn);
                    int SeniorityNameColumnIndex = reader.GetOrdinal(SeniorityNameColumn);

                    var res = new List<Quadruple<Person, int[], int, int>>();
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
                                    Id = (int)Enum.Parse(
                                        typeof(PersonStatusType),
                                        (string)reader[personStatusNameIndex]),
                                    Name = (string)reader[personStatusNameIndex]
                                },
                                CurrentPay = new Pay
                                {
                                    TimescaleName = reader.GetString(timescaleNameIndex),
                                    Timescale = (TimescaleType)reader.GetInt32(timescaleIdIndex)
                                },
                                HireDate = (DateTime)reader[hireDateIndex],
                                Seniority = new Seniority
                                {
                                     Id = reader.GetInt32(seniorityIdColumnIndex),
                                     Name = reader.GetString(SeniorityNameColumnIndex)
                                }
                            };
                        int[] load = Utils.StringToIntArray((string)reader[weeklyLoadIndex]);

                        int avgUtil = reader.GetInt32(avgUtilIndex);

                        int PersonVactionDays = reader.GetInt32(personVacationDaysIndex);


                        res.Add(new Quadruple<Person, int[], int, int>(person, load, avgUtil, PersonVactionDays));
                    }

                    return res;
                }
            }

            return null;
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
        /// Gets permissions for single target type
        /// </summary>
        /// <param name="person">Person to get permissions for</param>
        /// <param name="target">Target permissions type</param>
        /// <returns></returns>
        public static IEnumerable<int> GetPermissions(Person person, PermissionTarget target)
        {
            string procedure;

            //  Choose right procedure to execute
            switch (target)
            {
                case PermissionTarget.Client:
                    procedure = PermissionsGetAllowedClientsProcedure;
                    break;

                case PermissionTarget.Group:
                    procedure = PermissionsGetAllowedGroupsProcedure;
                    break;

                case PermissionTarget.Practice:
                    procedure = PermissionsGetAllowedPracticesProcedure;
                    break;

                case PermissionTarget.PracticeManager:
                    procedure = PermissionsGetAllowedPracticeManagersProcedure;
                    break;

                case PermissionTarget.Salesperson:
                    procedure = PermissionsGetAllowedSalespersonsProcedure;
                    break;

                default:
                    throw new Exception("PersonDAL.GetPermissions: no such permission target.");
            }

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(procedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(PersonIdParam,
                                                    person.Id.HasValue ? (object)person.Id.Value : DBNull.Value);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            var permission = new PersonPermission();
                            while (reader.Read())
                            {
                                permission.AddToList(target, (int)reader[PersonIdColumn]);
                            }

                            return permission.GetPermissions(target);
                        }
                    }
                }
            }

            return null;
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
                using (var command = new SqlCommand(PermissionsGetAllProcedure, connection))
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

            using (var command = new SqlCommand(PersonInsertProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(FirstNameParam,
                                                !string.IsNullOrEmpty(person.FirstName)
                                                    ? (object)person.FirstName
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(LastNameParam,
                                                !string.IsNullOrEmpty(person.LastName)
                                                    ? (object)person.LastName
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(PTODaysPerAnnumParam, person.PtoDays);
                command.Parameters.AddWithValue(HireDateParam, person.HireDate);
                command.Parameters.AddWithValue(AliasParam,
                                                !string.IsNullOrEmpty(person.Alias)
                                                    ? (object)person.Alias
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(DefaultPracticeParam,
                                                person.DefaultPractice != null
                                                    ? (object)person.DefaultPractice.Id
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(PersonStatusIdParam,
                                                person.Status != null ? (object)person.Status.Id : DBNull.Value);
                command.Parameters.AddWithValue(TerminationDateParam,
                                                person.TerminationDate.HasValue
                                                    ? (object)person.TerminationDate.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(TeleponeNumberParam,
                                                !string.IsNullOrEmpty(person.TelephoneNumber)
                                                    ? (object)person.TelephoneNumber
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(SeniorityIdParam,
                                                person.Seniority != null
                                                    ? (object)person.Seniority.Id
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(UserLoginParam,
                                                !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);

                if (person.Manager != null)
                    command.Parameters.AddWithValue(
                        Constants.ParameterNames.ManagerId, person.Manager.Id.Value);

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

        /// <summary>
        /// Retrives <see cref="Opportunity"/> data to be exported to excel.
        /// </summary>
        /// <returns>An <see cref="Opportunity"/> object if found and null otherwise.</returns>
        public static DataSet PersonGetExcelSet()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonGetExcelSetProcedure, connection))
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
            using (var command = new SqlCommand(PersonUpdateProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam,
                                                person.Id.HasValue ? (object)person.Id.Value : DBNull.Value);
                command.Parameters.AddWithValue(FirstNameParam,
                                                !string.IsNullOrEmpty(person.FirstName)
                                                    ? (object)person.FirstName
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(LastNameParam,
                                                !string.IsNullOrEmpty(person.LastName)
                                                    ? (object)person.LastName
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(PTODaysPerAnnumParam, person.PtoDays);
                command.Parameters.AddWithValue(HireDateParam, person.HireDate);
                command.Parameters.AddWithValue(TerminationDateParam,
                                                person.TerminationDate.HasValue
                                                    ? (object)person.TerminationDate.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(AliasParam,
                                                !string.IsNullOrEmpty(person.Alias)
                                                    ? (object)person.Alias
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(DefaultPracticeParam,
                                                person.DefaultPractice != null
                                                    ? (object)person.DefaultPractice.Id
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(TeleponeNumberParam,
                                                !string.IsNullOrEmpty(person.TelephoneNumber)
                                                    ? (object)person.TelephoneNumber
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(PersonStatusIdParam,
                                                person.Status != null ? (object)person.Status.Id : DBNull.Value);
                command.Parameters.AddWithValue(SeniorityIdParam,
                                                person.Seniority != null
                                                    ? (object)person.Seniority.Id
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(UserLoginParam,
                                                !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);

                command.Parameters.AddWithValue(EmployeeNumberParam,
                                                !string.IsNullOrEmpty(person.EmployeeNumber)
                                                    ? (object)person.EmployeeNumber
                                                    : DBNull.Value);

                if (person.Manager != null)
                    command.Parameters.AddWithValue(
                        Constants.ParameterNames.ManagerId, person.Manager.Id.Value);

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
        /// List all active <see cref="Person"/>s in the system
        /// </summary>
        /// <returns><see cref="List{T}"/> of all active persons in the system</returns>
        public static List<Person> PersonListAll()
        {
            return PersonListFiltered(null, false, 0, 0, null, null);
        }

        /// <summary>
        /// Lists all active persons who receive some recruiter commissions.
        /// </summary>
        /// <returns>The list of <see cref="Person"/> objects.</returns>
        public static List<Person> PersonListRecruiter(int? personId, DateTime? hireDate)
        {
            var result = new List<Person>();

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonListRecruiterProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam,
                                                personId.HasValue ? (object)personId.Value : DBNull.Value);
                command.Parameters.AddWithValue(HireDateParam,
                                                hireDate.HasValue ? (object)hireDate.Value : DBNull.Value);

                connection.Open();

                ReadPersons(command, result);
            }

            return result;
        }

        /// <summary>
        /// List the persons who recieve the sales commissions
        /// </summary>
        /// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
        /// <returns>The list of the <see cref="Person"/> objects.</returns>
        public static List<Person> PersonListSalesperson(bool includeInactive)
        {
            var result = new List<Person>();

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonListSalespersonProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(IncludeInactiveParam, includeInactive);

                connection.Open();
                ReadPersons(command, result);

                return result;
            }
        }

        /// <summary>
        /// List the persons who recieve the sales commissions
        /// </summary>
        /// <param name="person">Person to restrict permissions to</param>
        /// <param name="inactives">Determines whether inactive persons will are included into the results.</param>
        /// <returns>The list of the <see cref="Person"/> objects.</returns>
        public static List<Person> PersonListSalesperson(Person person, bool inactives)
        {
            var result = new List<Person>();

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonListSalespersonProcedure, connection))
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
        /// List the persons who recieve the Practice Management commissions
        /// </summary>
        /// <param name="projectId">An ID of the project to the Practice Manager be selected for.</param>
        /// <param name="endDate">An end date of the project the Practice Manager be selected for.</param>
        /// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
        /// <returns>
        /// The list of <see cref="Person"/> objects applicable to be a practice manager for the project.
        /// </returns>
        public static List<Person> PersonListPracticeManager(int? projectId, DateTime? endDate, bool includeInactive)
        {
            var result = new List<Person>();

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonListPracticeManagerProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ProjectIdParam,
                                                projectId.HasValue ? (object)projectId.Value : DBNull.Value);
                command.Parameters.AddWithValue(EndDateParam,
                                                endDate.HasValue ? (object)endDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(IncludeInactiveParam, includeInactive);

                connection.Open();
                ReadPersons(command, result);

                return result;
            }
        }

        /// <summary>
        /// List the persons who recieve the Practice Management commissions
        /// </summary>
        /// <param name="projectId">An ID of the project to the Practice Manager be selected for.</param>
        /// <param name="endDate">An end date of the project the Practice Manager be selected for.</param>
        /// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
        /// <param name="person">Person to apply restrictions to</param>
        /// <returns>
        /// The list of <see cref="Person"/> objects applicable to be a practice manager for the project.
        /// </returns>
        public static List<Person> PersonListPracticeManager(Person person, int? projectId, DateTime? endDate,
                                                             bool includeInactive)
        {
            var result = new List<Person>();

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonListPracticeManagerProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ProjectIdParam,
                                                projectId.HasValue ? (object)projectId.Value : DBNull.Value);
                command.Parameters.AddWithValue(EndDateParam,
                                                endDate.HasValue ? (object)endDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(IncludeInactiveParam, includeInactive);

                if (person != null)
                    command.Parameters.AddWithValue(PersonIdParam, person.Id);

                connection.Open();
                ReadPersons(command, result);

                return result;
            }
        }

        /// <summary>
        /// List the persons who recieve the Practice Management commissions
        /// </summary>
        /// <param name="endDate">An end date of the project the Practice Manager be selected for.</param>
        /// <param name="includeInactive">Determines whether inactive persons will are included into the results.</param>
        /// <param name="person"></param>
        /// <returns>
        /// The list of <see cref="Person"/> objects applicable to be a practice manager for the project.
        /// </returns>
        public static List<Person> PersonListProjectOwner(DateTime? endDate, bool includeInactive, Person person)
        {
            var result = new List<Person>();

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonListProjectOwnerProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(EndDateParam,
                    endDate.HasValue ? (object)endDate.Value : DBNull.Value);
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
                using (var command = new SqlCommand(PersonGetByIdProcedure, connection))
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
                using (var command = new SqlCommand(PersonListAllSeniorityFilterProcedure, connection))
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

        public static List<Person> PersonListFilteredWithCurrentPay(
            int? practice,
            bool showAll,
            int pageSize,
            int pageNo,
            string looked,
            DateTime startDate,
            DateTime endDate,
            int? recruiterId,
            int? maxSeniorityLevel,
            string sortBy,
            int? timeScaleId,
            bool projected,
            bool terminated,
            bool inactive,
            char? alphabet)
        {
            var personList = new List<Person>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(PersonListAllSeniorityFilterWithPayProcedure, connection))
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
                    if (!string.IsNullOrEmpty(sortBy))
                    {
                        command.Parameters.AddWithValue(SortByParam, sortBy);
                    }
                    command.Parameters.AddWithValue(TimescaleIdParam,
                                                    timeScaleId.HasValue ? (object)timeScaleId.Value : DBNull.Value);
                    command.Parameters.AddWithValue(ProjectedParam, projected);
                    command.Parameters.AddWithValue(TerminatedParam, terminated);
                    command.Parameters.AddWithValue(InactiveParam, inactive);
                    command.Parameters.AddWithValue(AlphabetParam, alphabet.HasValue ? (object)alphabet.Value : DBNull.Value);

                    connection.Open();
                    ReadPersonsWithCurrentPay(command, personList);
                }
            }
            return personList;
        }

        public static List<Person> PersonListFilteredWithCurrentPayByCommaSeparatedIdsList(
           string practiceIdsSelected,
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
           bool inactive,
           char? alphabet)
        {
            var personList = new List<Person>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(PersonListAllSeniorityFilterWithPayByCommaSeparatedIdsListProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(ShowAllParam, showAll);
                    command.Parameters.AddWithValue(PracticeIdsListParam,
                                                    practiceIdsSelected != null ? (object)practiceIdsSelected : DBNull.Value);
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
                    command.Parameters.AddWithValue(RecruiterIdsListParam,
                                                    recruiterIdsSelected != null ? (object)recruiterIdsSelected : DBNull.Value);
                    command.Parameters.AddWithValue(MaxSeniorityLevelParam,
                                                    maxSeniorityLevel.HasValue
                                                        ? (object)maxSeniorityLevel.Value
                                                        : DBNull.Value);
                    if (!string.IsNullOrEmpty(sortBy))
                    {
                        command.Parameters.AddWithValue(SortByParam, sortBy);
                    }
                    command.Parameters.AddWithValue(TimescaleIdsListParam,
                                                    timeScaleIdsSelected != null ? (object)timeScaleIdsSelected : DBNull.Value);
                    command.Parameters.AddWithValue(ProjectedParam, projected);
                    command.Parameters.AddWithValue(TerminatedParam, terminated);
                    command.Parameters.AddWithValue(InactiveParam, inactive);
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
        /// <param name="statusId">Person status id</param>
        /// <param name="startDate">Determines a start date when persons in the list must are available.</param>
        /// <param name="endDate">Determines an end date when persons in the list must are available.</param>
        /// <returns>A list of the <see cref="Person"/> objects.</returns>
        public static List<Person> PersonListAllShort(int? practice, int? statusId, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonListAllShortProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PracticeIdParam,
                                                practice.HasValue ? (object)practice.Value : DBNull.Value);
                command.Parameters.AddWithValue(StartDateParam,
                                                startDate > DateTime.MinValue ? (object)startDate : DBNull.Value);
                command.Parameters.AddWithValue(EndDateParam,
                                                endDate > DateTime.MinValue ? (object)endDate : DBNull.Value);
                command.Parameters.AddWithValue(PersonStatusIdParam,
                                                statusId.HasValue ? (object)statusId.Value : DBNull.Value);

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
        /// <param name="seniorityId"></param>
        /// <param name="statusId"></param>
        /// <returns></returns>
        public static List<Person> PersonsGetBySeniorityAndStatus(int seniorityId, int? statusId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonsGetBySeniorityAndStatusProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(SeniorityIdParam, seniorityId);
                command.Parameters.AddWithValue(PersonStatusIdParam,
                                                statusId.HasValue ? (object)statusId.Value : DBNull.Value);

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
        /// <returns></returns>
        public static List<Person> GetPersonListByStatusList(string statusList, int? personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonListByStatusListProcedure, connection))
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
            using (var command = new SqlCommand(PersonListAllForMilestoneProcedure, connection))
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
            using (var command = new SqlCommand(PersonListBenchExpenseProcedure, connection))
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
        /// Retrieves all subordinated persons for a specified practice manager.
        /// </summary>
        /// <param name="practiceManagerId">An ID of the practice manager to the data be retrieved for.</param>
        /// <returns>The list of the <see cref="Person"/> objects.</returns>
        public static List<Person> PersonListSubordinates(int practiceManagerId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonListSubordinatesProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PracticeManagerIdParam, practiceManagerId);

                connection.Open();

                var result = new List<Person>();
                ReadPersons(command, result);
                return result;
            }
        }

        /// <summary>
        /// Retrieves person one-off list.
        /// </summary>
        /// <returns>The list of the <see cref="Person"/> objects.</returns>
        public static List<Person> PersonOneOffList(DateTime today, int? maxSeniorityLevel)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonOneOffListProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(DateTodayParam, today);
                command.Parameters.AddWithValue(MaxSeniorityLevelParam,
                                                maxSeniorityLevel.HasValue
                                                    ? (object)maxSeniorityLevel.Value
                                                    : DBNull.Value);

                connection.Open();

                var result = new List<Person>();
                ReadPersons(command, result);
                return result;
            }
        }

        /// <summary>
        /// Retrieves a number of the work days for the <see cref="Person"/> and the period specified.
        /// </summary>
        /// <param name="personId">An ID of the person to retrieve the data for.</param>
        /// <param name="startDate">A start date of the period.</param>
        /// <param name="endDate">An end date of the period.</param>
        /// <returns>A number of work days for the specified person withing the specified period.</returns>
        public static int PersonWorkDaysNumber(int personId, DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonWorkDaysNumberProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam, personId);
                command.Parameters.AddWithValue(StartDateParam, startDate);
                command.Parameters.AddWithValue(EndDateParam, endDate);

                connection.Open();

                return (int)command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Retrives a number of persons who were active in the specified period
        /// </summary>
        /// <param name="startDate">The period start date.</param>
        /// <param name="endDate">The period end date.</param>
        /// <param name="employeesCount">The total number of prosons who were active.</param>
        /// <param name="consultantsCount">
        /// The number of persons who were active and who not in the Admin practice.
        /// </param>
        public static void PersonGetCountActive(DateTime startDate,
                                                DateTime endDate,
                                                out int employeesCount,
                                                out int consultantsCount)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonGetCountActiveProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(StartDateParam, startDate);
                command.Parameters.AddWithValue(EndDateParam, endDate);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (reader.Read())
                    {
                        employeesCount = Convert.ToInt32(reader[EmployeesNumberColumn]);
                        consultantsCount = Convert.ToInt32(reader[ConsultantsNumberColumn]);
                    }
                    else
                    {
                        employeesCount = 0;
                        consultantsCount = 0;
                    }
                }
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
            using (var command = new SqlCommand(PersonGetByAliasProcedure, connection))
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
            if (reader.HasRows)
            {
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
        }

        /// <summary>
        /// Calculates a number of <see cref="Person"/>s match with the specified conditions.
        /// </summary>
        /// <param name="practice">The <see cref="Person"/>'s default practice.</param>
        /// <param name="showAll">List all <see cref="Person"/>s if true and the only active otherwise.</param>
        /// <param name="looked">List all <see cref="Person"/>s by search string that matches for first name or last name  .</param>
        /// <param name="recruiterId">Determines an ID of the recruiter to retrieve the recruits for.</param>
        /// <returns>The number of the persons those match with the specified conditions.</returns>
        public static int PersonGetCount(int? practice, bool showAll, string looked, int? recruiterId, int? timeScaleId, bool projected, bool terminated, bool inactive, char? alphabet)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonGetCountProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ShowAllParam, showAll);
                if (practice.HasValue)
                {
                    command.Parameters.AddWithValue(PracticeIdParam, practice.Value);
                }
                command.Parameters.AddWithValue(LookedParam,
                                                !string.IsNullOrEmpty(looked) ? (object)looked : DBNull.Value);
                command.Parameters.AddWithValue(RecruiterIdParam,
                                                recruiterId.HasValue ? (object)recruiterId.Value : DBNull.Value);
                command.Parameters.AddWithValue(TimescaleIdParam,
                                                timeScaleId.HasValue ? (object)timeScaleId.Value : DBNull.Value);
                command.Parameters.AddWithValue(ProjectedParam, projected);
                command.Parameters.AddWithValue(TerminatedParam, terminated);
                command.Parameters.AddWithValue(InactiveParam, inactive);
                command.Parameters.AddWithValue(AlphabetParam, alphabet.HasValue ? (object)alphabet.Value : DBNull.Value);

                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Inactivate (logically delete) a person
        /// </summary>
        /// <param name="person"><see cref="Person"/> to delete</param>
        public static void PersonInactivate(Person person)
        {
            var status = new PersonStatus { Id = (int)PersonStatusType.Terminated };
            person.Status = status;
            PersonSetStatus(person);
        }

        /// <summary>
        /// Make a <see cref="Person"/> active
        /// </summary>
        /// <param name="person">person to activate</param>
        public static void PersonReactivate(Person person)
        {
            var status = new PersonStatus { Id = (int)PersonStatusType.Active };
            person.Status = status;
            PersonSetStatus(person);
        }

        /// <summary>
        /// Set status of person
        /// </summary>
        /// <param name="person"><see cref="Person"/> to delete</param>
        public static void PersonSetStatus(Person person)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(PersonSetStatusProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(PersonIdParam, person.Id);
                    command.Parameters.AddWithValue(PersonStatusIdParam, person.Status.Id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Retrives a list of overheads for the specified person.
        /// </summary>
        /// <param name="personId">An ID of the person to retrieve the data for.</param>
        /// <returns>The list of the <see cref="PersonOverhead"/> objects.</returns>
        public static List<PersonOverhead> PersonOverheadListByPerson(int personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonOverheadByPersonProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam, personId);

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
        public static List<PersonOverhead> PersonOverheadListByTimescale(TimescaleType timescale)
        {
            // Because % of Revenue type has the same list of overheads,
            //  exctract them as if it was 1099
            if (timescale == TimescaleType.PercRevenue)
                timescale = TimescaleType._1099Ctc;

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonOverheadByTimescaleProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(TimescaleIdParam, timescale);

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
        /// Retrieves a list of an overhead for the person on the milstone grouped by days.
        /// </summary>
        /// <param name="personId">An ID of the <see cref="Person"/> to retrive the data for.</param>
        /// <param name="milestoneId">An ID of the <see cref="Milestone"/> to retrieve the data for.</param>
        /// <returns>A collection of computed overheads by days.</returns>
        public static List<PersonOverhead> MilestonePersonListOverheadByPerson(int personId,
                                                                               int milestoneId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(MilestonePersonListOverheadByPersonProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam, personId);
                command.Parameters.AddWithValue(MilestoneIdParam, milestoneId);

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
        /// <param name="userId">An ID of the user to the membership info be deleted for.</param>
        public static void DeleteUser(string userName, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {

            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }
            using (var command = new SqlCommand(aspnetUsersDeleteUserProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(AppicationNameParam, "PracticeManagement");
                command.Parameters.AddWithValue(UserNameParam, userName);
                command.Parameters.AddWithValue(TablesToDeleteFromParam, 15);
                var NumTablesDeletedFromParm = new SqlParameter(PersonIdParam, SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(NumTablesDeletedFromParm);

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
            using (var command = new SqlCommand(aspnetMembershipCreateUserProcedure, connection))
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
            using (var command = new SqlCommand(MembershipAliasUpdateProcedure, connection))
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
            using (var command = new SqlCommand(MembershipDeleteProcedure, connection))
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
        /// Verifies the person's data for an integrity.
        /// </summary>
        /// <param name="personId">An ID of the person to be verified.</param>
        /// <exception cref="DataAccessException">When the person's data are incorrect.</exception>
        public static void PersonEnsureIntegrity(int personId, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

            using (var command = new SqlCommand(PersonEnsureIntegrityProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam, personId);

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
                    throw new DataAccessException(ex.Message, ex);
                }
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
            if (person.Id != null)
            {
                using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
                using (var command = new SqlCommand(PermissionsSetAllProcedure, connection))
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
        }

        private static void ReadPersonOverheads(DbDataReader reader, List<PersonOverhead> result)
        {
            if (reader.HasRows)
            {
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
                if (reader.HasRows)
                {
                    int aliasIndex = reader.GetOrdinal(AliasColumn);
                    int personStatusIdIndex = reader.GetOrdinal(PersonStatusIdColumn);
                    int personStatusNameIndex = reader.GetOrdinal(PersonStatusNameColumn);
                    int seniorityIdIndex = reader.GetOrdinal(SeniorityIdColumn);
                    int seniorityNameIndex = reader.GetOrdinal(SeniorityNameColumn);
                    int ptoDaysPerAnnumIndex = reader.GetOrdinal(PTODaysPerAnnumColumn);
                    int managerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ManagerId);
                    int managerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.ManagerFirstName);
                    int managerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.ManagerLastName);
                    int telephoneNumberIndex = reader.GetOrdinal(Constants.ColumnNames.TelephoneNumber);
                    int isDefManagerIndex;
                    int IsWelcomeEmailSentIndex;
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
                            PtoDays = reader.GetInt32(ptoDaysPerAnnumIndex),
                            HireDate = (DateTime)reader[HireDateColumn],
                            TelephoneNumber = !reader.IsDBNull(telephoneNumberIndex) ? reader.GetString(telephoneNumberIndex) : string.Empty
                        };

                        if (IsWelcomeEmailSentIndex > -1)
                        {
                            person.IsWelcomeEmailSent = reader.GetBoolean(IsWelcomeEmailSentIndex);
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

                        if (practicesOwnedIndex >= 0 && !reader.IsDBNull(practicesOwnedIndex))
                        {
                            person.PracticesOwned = new List<Practice>();

                            var xdoc = new XmlDocument();
                            var xmlPractices = reader.GetString(practicesOwnedIndex);
                            xdoc.LoadXml(xmlPractices);

                            foreach (XmlNode xpractice in xdoc.FirstChild.ChildNodes)
                            {
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
                                // Alias = reader.GetString(managerAliasIndex)
                            };
                        }

                        personList.Add(person);
                    }
                }
            }
        }


        private static void ReadPersonsWithCurrentPay(SqlCommand command, ICollection<Person> personList)
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    int aliasIndex = reader.GetOrdinal(AliasColumn);
                    int personStatusIdIndex = reader.GetOrdinal(PersonStatusIdColumn);
                    int personStatusNameIndex = reader.GetOrdinal(PersonStatusNameColumn);
                    int seniorityIdIndex = reader.GetOrdinal(SeniorityIdColumn);
                    int seniorityNameIndex = reader.GetOrdinal(SeniorityNameColumn);
                    int ptoDaysPerAnnumIndex = reader.GetOrdinal(PTODaysPerAnnumColumn);
                    int managerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ManagerId);
                    int managerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.ManagerFirstName);
                    int managerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.ManagerLastName);
                    int telephoneNumberIndex = reader.GetOrdinal(Constants.ColumnNames.TelephoneNumber);
                    int terminationDateIndex = reader.GetOrdinal(TerminationDateColumn);
                    int lastLoginDateIndex = reader.GetOrdinal(LastLoginDateColumn);
                    //Pay Related columns
                    int personIdIndex = reader.GetOrdinal(PersonIdColumn);
                    int startDateIndex = reader.GetOrdinal(StartDateColumn);
                    int endDateIndex = reader.GetOrdinal(EndDateColumn);
                    int amountIndex = reader.GetOrdinal(AmountColumn);
                    int timescaleIndex = reader.GetOrdinal(TimescaleColumn);
                    int timescaleNameIndex = reader.GetOrdinal(TimescaleNameColumn);
                    int amountHourlyIndex = reader.GetOrdinal(AmountHourlyColumn);
                    int timesPaidPerMonthIndex = reader.GetOrdinal(TimesPaidPerMonthColumn);
                    int termsIndex = reader.GetOrdinal(TermsColumn);
                    int vacationDaysIndex = reader.GetOrdinal(VacationDaysColumn);
                    int bonusAmountIndex = reader.GetOrdinal(BonusAmountColumn);
                    int bonusHoursToCollectIndex = reader.GetOrdinal(BonusHoursToCollectColumn);
                    int isYearBonusIndex = reader.GetOrdinal(IsYearBonusColumn);
                    int defaultHoursPerDayIndex = reader.GetOrdinal(DefaultHoursPerDayColumn);
                    int payPersonIdIndex = reader.GetOrdinal(PayPersonIdColumn);

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
                            PtoDays = reader.GetInt32(ptoDaysPerAnnumIndex),
                            HireDate = (DateTime)reader[HireDateColumn],
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
                            Seniority = !reader.IsDBNull(seniorityIdIndex) ? new Seniority
                            {
                                Id = reader.GetInt32(seniorityIdIndex),
                                Name = reader.GetString(seniorityNameIndex)
                            } : null,
                            Manager = !Convert.IsDBNull(reader[managerIdIndex]) ? new Person
                            {
                                Id = reader.GetInt32(managerIdIndex),
                                FirstName = reader.GetString(managerFirstNameIndex),
                                LastName = reader.GetString(managerLastNameIndex)
                                // Alias = reader.GetString(managerAliasIndex)
                            } : null
                        };

                        if (practicesOwnedIndex >= 0 && !reader.IsDBNull(practicesOwnedIndex))
                        {
                            person.PracticesOwned = new List<Practice>();

                            var xdoc = new XmlDocument();
                            var xmlPractices = reader.GetString(practicesOwnedIndex);
                            xdoc.LoadXml(xmlPractices);

                            foreach (XmlNode xpractice in xdoc.FirstChild.ChildNodes)
                            {
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
                                TimesPaidPerMonth =
                                    !reader.IsDBNull(timesPaidPerMonthIndex) ?
                                    (int?)reader.GetInt32(timesPaidPerMonthIndex) : null,
                                Terms = !reader.IsDBNull(termsIndex) ? (int?)reader.GetInt32(termsIndex) : null,
                                VacationDays =
                                    !reader.IsDBNull(vacationDaysIndex) ? (int?)reader.GetInt32(vacationDaysIndex) : null,
                                BonusAmount = reader.GetDecimal(bonusAmountIndex),
                                BonusHoursToCollect =
                                    !reader.IsDBNull(bonusHoursToCollectIndex) ?
                                    (int?)reader.GetInt32(bonusHoursToCollectIndex) : null,
                                IsYearBonus = reader.GetBoolean(isYearBonusIndex),
                                DefaultHoursPerDay = reader.GetDecimal(defaultHoursPerDayIndex)

                            };
                            person.CurrentPay = pay;
                        }

                        personList.Add(person);
                    }
                }
            }
        }

        private static void ReadPersonsShort(SqlDataReader reader, List<Person> result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(PersonIdColumn);
                int firstNameIndex = reader.GetOrdinal(FirstNameColumn);
                int lastNameIndex = reader.GetOrdinal(LastNameColumn);
                int isDefManagerIndex = reader.GetOrdinal(Constants.ColumnNames.IsDefaultManager);
                int hireDateIndex;
                try
                {
                    hireDateIndex = reader.GetOrdinal(HireDateColumn);
                }
                catch
                {
                    hireDateIndex = -1;
                }

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
                    var isDefaultManager = reader.GetBoolean(isDefManagerIndex);
                    if (isDefaultManager)
                        person.Manager = new Person { Id = personId };

                    result.Add(person);
                }
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
            using (var command = new SqlCommand(GetPersonListByPersonIdListProcedure, connection))
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

        /// <summary>
        /// Retrieves a short info on persons.
        /// </summary>
        /// <param name="statusId">Person status id</param>
        /// <param name="rolename">Person role</param>
        /// <returns>A list of the <see cref="Person"/> objects</returns>
        public static List<Person> PersonListShortByRoleAndStatus(int? statusId, string rolename)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonListShortByRoleAndStatusProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(PersonStatusIdParam,
                                                statusId.HasValue ? (object)statusId.Value : DBNull.Value);
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
            throw new NotImplementedException();
        }

        public static bool UserTemporaryCredentialsInsert(string userName, string password, int passwordFormat, string passwordSalt, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            int rowsEffeced = 0;

            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }
            using (var command = new SqlCommand(UserTemporaryCredentialsInsertProcedure, connection))
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
            using (SqlCommand command = new SqlCommand(GetTemporaryCredentialsByUserNameProcedure, connection))
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
                    if (users.Count > 0)
                        return users[0];
                    else
                        return null;
                }
            }
        }

        public static void DeleteTemporaryCredentialsByUserName(string userName)
        {
            using (SqlConnection connection = new SqlConnection(DataAccess.Other.DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(DeleteTemporaryCredentialsByUserNameProcedure, connection))
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
            if (reader.HasRows)
            {
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
        }

        public static void SetNewPasswordForUser(string userName,
            string newPassword, string passwordSalt, int passwordFormat, DateTime currentTimeUtc, string applicationName = "PracticeManagement")
        {

            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(SetNewPasswordForUserProcedure, connection))
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

        public static int PersonGetCount(string practiceIds, bool showAll, string looked, string recruiterIds, string timeScaleIds, bool projected, bool terminated, bool inactive, char? alphabet)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(PersonGetCountByCommaSeparatedIdsListProcedure, connection))
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
                command.Parameters.AddWithValue(InactiveParam, inactive);
                command.Parameters.AddWithValue(AlphabetParam, alphabet.HasValue ? (object)alphabet.Value : DBNull.Value);

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
            using (var command = new SqlCommand(UpdateLastPasswordChangedDateForPersonProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(EmailParam, email);

                connection.Open();
                command.ExecuteScalar();
            }
        }
        
    }
}


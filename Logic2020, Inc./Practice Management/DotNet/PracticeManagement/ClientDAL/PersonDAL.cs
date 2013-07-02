﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;
using DataAccess.Other;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
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
        private const string TitleNameParam = "@TitleName";
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
        public static List<Quadruple<Person, int[], int, int>> GetConsultantUtilizationWeekly(ConsultantTimelineReportContext context)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                List<Quadruple<Person, int[], int, int>> result = null;

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
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        result = GetPersonLoadWithVactionDays(reader);
                        reader.NextResult();
                        //ReadPersonsWeeklyUtlization(reader, result);
                        //reader.NextResult();
                        ReadPersonsEmploymentHistory(reader, result);
                    }
                }
                Dictionary<int, List<int>> PersonWeeklyUtlization = GetWeeklyUtilizationForConsultant(context);

                if (result != null && PersonWeeklyUtlization != null)
                {
                    foreach (var record in result)
                    {
                        if (record.First.Id.HasValue && PersonWeeklyUtlization.Any(p => p.Key == record.First.Id.Value))
                        {
                            record.Second = PersonWeeklyUtlization[record.First.Id.Value].ToArray();
                        }
                    }
                }

                return result;
            }
        }

        public static Dictionary<int, List<int>> GetWeeklyUtilizationForConsultant(ConsultantTimelineReportContext context)
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
                command.Parameters.AddWithValue(Constants.ParameterNames.InternalProjects, context.InternalProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.Start, context.Start);
                command.Parameters.AddWithValue(Constants.ParameterNames.Granularity, context.Granularity);
                command.Parameters.AddWithValue(Constants.ParameterNames.Period, context.Period);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, context.PracticeIdList);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimescaleIds, context.TimescaleIdList);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExcludeInternalPractices, context.ExcludeInternalPractices);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsSampleReport, context.IsSampleReport);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return ReadPersonsWeeklyUtlization(reader);
                }
            }
        }

        public static List<Triple<Person, int[], int>> ConsultantUtilizationDailyByPerson(int personId, ConsultantTimelineReportContext context)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.ConsultantUtilizationDailyByPersonProcedure, connection))
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
        private static List<Quadruple<Person, int[], int, int>> GetPersonLoadWithVactionDays(SqlDataReader reader)
        {
            if (reader.HasRows)
            {
                int personStatusIdIndex = reader.GetOrdinal(PersonStatusId);
                int personStatusNameIndex = reader.GetOrdinal(NameColumn);
                int firstNameIndex = reader.GetOrdinal(FirstNameColumn);
                int lastNameIndex = reader.GetOrdinal(LastNameColumn);
                int employeeNumberIndex = reader.GetOrdinal(EmployeeNumberColumn);
                //int weeklyLoadIndex = reader.GetOrdinal(WeeklyUtilColumn);
                int timescaleNameIndex = reader.GetOrdinal(TimescaleColumn);
                int timescaleIdIndex = reader.GetOrdinal(TimescaleIdColumn);
                int hireDateIndex = reader.GetOrdinal(HireDateColumn);
                int avgUtilIndex = reader.GetOrdinal(AvgUtilColumn);
                int personVacationDaysIndex = reader.GetOrdinal(PersonVactionDaysColumn);
                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);

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
                            }
                        };

                    if (Convert.IsDBNull(reader[TerminationDateColumn]))
                    {
                        person.TerminationDate = null;
                    }
                    else
                    {
                        person.TerminationDate = (DateTime)reader[TerminationDateColumn];
                    }

                    int[] load = { 0 };//Utils.StringToIntArray((string)reader[weeklyLoadIndex]);

                    int avgUtil = reader.GetInt32(avgUtilIndex);

                    int PersonVactionDays = reader.GetInt32(personVacationDaysIndex);

                    res.Add(new Quadruple<Person, int[], int, int>(person, load, avgUtil, PersonVactionDays));
                }

                return res;
            }
            return null;
        }

        private static Dictionary<int, List<int>> ReadPersonsWeeklyUtlization(SqlDataReader reader)
        {
            Dictionary<int, List<int>> PersonWeeklyUtlization = new Dictionary<int, List<int>>();
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int weeklyUtlizationIndex = reader.GetOrdinal(Constants.ColumnNames.WeeklyUtlization);
                while (reader.Read())
                {
                    int personid = reader.GetInt32(personIdIndex);
                    List<int> weeklyUtlization = null;
                    if (PersonWeeklyUtlization.Any(p => p.Key == personid))
                    {
                        weeklyUtlization = PersonWeeklyUtlization.First(p => p.Key == personid).Value;
                    }
                    else
                    {
                        weeklyUtlization = new List<int>();
                        PersonWeeklyUtlization.Add(personid, weeklyUtlization);
                    }
                    weeklyUtlization.Add(reader.GetInt32(weeklyUtlizationIndex));
                }
            }
            return PersonWeeklyUtlization;
        }

        private static void ReadPersonsEmploymentHistory(SqlDataReader reader, List<Quadruple<Person, int[], int, int>> result)
        {
            if (result == null) return;
            if (!reader.HasRows) return;
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
                if (result.Any(p => p.First.Id == employment.PersonId))
                {
                    person = result.First(p => p.First.Id == employment.PersonId).First;
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

        public static List<Person> PersonListShortByTitleAndStatus(string statusIds, string titleName)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Person.PersonListShortByTitleAndStatusProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(PersonStatusIdsListParam,
                                                !string.IsNullOrEmpty(statusIds) ? (object)statusIds : DBNull.Value);
                command.Parameters.AddWithValue(TitleNameParam,
                                                !string.IsNullOrEmpty(titleName) ? (object)titleName : DBNull.Value);

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

        public static int PersonGetCount(string practiceIds, bool showAll, string looked, string recruiterIds, string timeScaleIds, bool projected, bool terminated, bool terminatedPending, char? alphabet)
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
    }
}

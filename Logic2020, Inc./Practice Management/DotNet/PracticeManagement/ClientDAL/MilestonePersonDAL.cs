using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;

namespace DataAccess
{
    public static class MilestonePersonDAL
    {
        #region Constants

        #region Parameters

        private const string ProjectIdParam = "@ProjectId";
        private const string MilestoneIdParam = "@MilestoneId";
        private const string PersonIdParam = "@PersonId";
        private const string StartDateParam = "@StartDate";
        private const string EndDateParam = "@EndDate";
        private const string CheckStartDateEqualityParam = "@CheckStartDateEquality";
        private const string CheckEndDateEqualityParam = "@CheckEndDateEquality";
        private const string HoursPerDayParam = "@HoursPerDay";
        private const string PersonRoleIdParam = "@PersonRoleId";
        private const string AmountParam = "@Amount";
        private const string MilestonePersonIdParam = "@MilestonePersonId";
        private const string LocationParam = "@Location";
        private const string UserLoginParam = "@UserLogin";

        #endregion

        #region Columns

        private const string MilestonePersonIdColumn = "MilestonePersonId";
        private const string MilestoneIdColumn = "MilestoneId";
        private const string PersonIdColumn = "PersonId";
        private const string StartDateColumn = "StartDate";
        private const string EndDateColumn = "EndDate";
        private const string FirstNameColumn = "FirstName";
        private const string LastNameColumn = "LastName";
        private const string ProjectIdColumn = "ProjectId";
        private const string ProjectStatusIdColumn = "ProjectStatusId";
        private const string ProjectNumberColumn = "ProjectNumber";
        private const string ProjectNameColumn = "ProjectName";
        private const string ProjectStartDateColumn = "ProjectStartDate";
        private const string ProjectEndDateColumn = "ProjectEndDate";
        private const string ClientIdColumn = "ClientId";
        private const string ClientNameColumn = "ClientName";
        private const string MilestoneNameColumn = "MilestoneName";
        private const string MilestoneStartDateColumn = "MilestoneStartDate";
        private const string MilestoneProjectedDeliveryDateColumn = "MilestoneProjectedDeliveryDate";
        private const string HoursPerDayColumn = "HoursPerDay";
        private const string ExpectedHoursColumn = "ExpectedHours";
        private const string SalesCommissionColumn = "SalesCommission";
        private const string PersonRoleIdColumn = "PersonRoleId";
        private const string PersonRoleNameColumn = "RoleName";
        private const string AmountColumn = "Amount";
        private const string IsHourlyAmountColumn = "IsHourlyAmount";
        private const string DiscountColumn = "Discount";
        private const string MilestoneExpectedHoursColumn = "MilestoneExpectedHours";
        private const string MilestoneActualDeliveryDateColumn = "MilestoneActualDeliveryDate";
        private const string MilestoneHourlyRevenueColumn = "MilestoneHourlyRevenue";
        private const string PersonVacationsOnMilestoneColumn = "VacationDays";
        private const string PersonSeniorityIdColumn = "SeniorityId";
        private const string LocationColumn = "Location";

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// 	Gets milestones info for given person
        /// </summary>
        public static List<MilestonePersonEntry> GetConsultantMilestones(ConsultantMilestonesContext context)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                var command = new SqlCommand(Constants.ProcedureNames.MilestonePerson.ConsultantMilestones, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, context.PersonId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, context.StartDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, context.EndDate);

                command.Parameters.AddWithValue(Constants.ParameterNames.IncludeActive, context.IncludeActiveProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.IncludeProjected, context.IncludeProjectedProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.IncludeInactive, context.IncludeInactiveProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.IncludeCompleted, context.IncludeCompletedProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.IncludeInternal, context.IncludeInternalProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.IncludeExperimental, context.IncludeExperimentalProjects);
                command.Parameters.AddWithValue(Constants.ParameterNames.IncludeDefaultMileStone, context.IncludeDefaultMileStone);

                connection.Open();
                using (var reader = command.ExecuteReader())
                    return ReadDetailedMilestonePersonEntries(reader);
            }
        }

        /// <summary>
        /// 	Retrives the list of the <see cref = "Person" />s for the specified <see cref = "Project" />.
        /// </summary>
        /// <param name = "projects">Projects list</param>
        /// <returns>The list of the <see cref = "MilestonePerson" /> objects.</returns>
        public static void LoadMilestonePersonListForProject(List<Project> projects)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(
                Constants.ProcedureNames.MilestonePerson.MilestonePersonListByProjectShort, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ProjectIdParam,
                                                DataTransferObjects.Utils.Generic.IdsListToString(projects));

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    projects.ForEach(delegate(Project project) { project.ProjectPersons = new List<MilestonePerson>(); });
                    ReadMilestonePersonsShort(reader, projects);
                }
            }
        }

        /// <summary>
        /// 	Retrives the list of the <see cref = "Person" />s for the specified <see cref = "Project" />.
        /// </summary>
        /// <param name = "projectId">An ID of the project to the data be retrieved for.</param>
        /// <returns>The list of the <see cref = "MilestonePerson" /> objects.</returns>
        public static List<MilestonePerson> MilestonePersonListByProject(int projectId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                var command = new SqlCommand(Constants.ProcedureNames.MilestonePerson.MilestonePersonListByProject,
                                             connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ProjectIdParam, projectId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<MilestonePerson>();

                    ReadMilestonePersons(reader, result);

                    return result;
                }
            }
        }

        /// <summary>
        /// 	Retrives the list of the <see cref = "MilestonePerson" /> objects representing the participation
        /// 	of the specific <see cref = "Person" />.
        /// </summary>
        /// <param name = "projectId">An ID of the <see cref = "Project" /> to the data be retrived for.</param>
        /// <param name = "personId">An ID of the <see cref = "Person" /> to teh data be retrieved for.</param>
        /// <returns>The list of the <see cref = "MilestonePerson" /> objects.</returns>
        public static List<MilestonePerson> MilestonePersonListByProjectPerson(
            int projectId,
            int personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                var command = new SqlCommand(
                    Constants.ProcedureNames.MilestonePerson.MilestonePersonListByProjectPerson, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ProjectIdParam, projectId);
                command.Parameters.AddWithValue(PersonIdParam, personId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<MilestonePerson>();

                    ReadMilestonePersons(reader, result);

                    return result;
                }
            }
        }

        /// <summary>
        /// 	Retrives the list of the <see cref = "Person" />s for the specified <see cref = "Milestone" />.
        /// </summary>
        /// <param name = "milestoneId">An ID of the milestone to the data be retrieved for.</param>
        /// <returns>The list of the <see cref = "MilestonePerson" /> objects.</returns>
        public static List<MilestonePerson> MilestonePersonListByMilestone(int milestoneId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                var command = new SqlCommand(Constants.ProcedureNames.MilestonePerson.MilestonePersonListByMilestone,
                                             connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(MilestoneIdParam, milestoneId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<MilestonePerson>();

                    ReadMilestonePersons(reader, result);

                    return result;
                }
            }
        }

        public static List<MilestonePerson> MilestonePersonsByMilestoneForTEByProject(int milestoneId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                var command = new SqlCommand(Constants.ProcedureNames.MilestonePerson.MilestonePersonsByMilestoneForTEByProject,
                                             connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(MilestoneIdParam, milestoneId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<MilestonePerson>();

                    ReadMilestonePersonsForTEbyProjectReport(reader, result);

                    return result;
                }
            }
        }

        /// <summary>
        /// 	Retrives the list of the <see cref = "Milestone" />s for the specified <see cref = "Person" />.
        /// </summary>
        /// <param name = "personId">An ID of the person the the data be retrieved for.</param>
        /// <returns>The list of the <see cref = "MilestonePerson" /> objects.</returns>
        public static List<MilestonePerson> MilestonePersonListByPerson(int personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                var command = new SqlCommand(Constants.ProcedureNames.MilestonePerson.MilestonePersonListByPerson,
                                             connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam, personId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<MilestonePerson>();

                    ReadMilestonePersons(reader, result);

                    return result;
                }
            }
        }

        /// <summary>
        /// 	Retrives persons-milestones list with their details for the specified project laying out by months.
        /// </summary>
        /// <param name = "projectId">An ID of teh project to the data be retrieved for.</param>
        /// <param name = "startDate">A start of the interesting period.</param>
        /// <param name = "endDate">An end of the interesting period.</param>
        /// <returns>The list of the <see cref = "MilestonePerson" /> objects.</returns>
        public static List<MilestonePerson> MilestonePersonListByProjectMonthlyLayout(
            int projectId,
            DateTime startDate,
            DateTime endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                var command = new SqlCommand(
                    Constants.ProcedureNames.MilestonePerson.MilestonePersonListByProjectMonth, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ProjectIdParam, projectId);
                command.Parameters.AddWithValue(StartDateParam, startDate);
                command.Parameters.AddWithValue(EndDateParam, endDate);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<MilestonePerson>();

                    ReadMilestonePersons(reader, result);

                    return result;
                }
            }
        }

        /// <summary>
        /// 	Retrives the milestone-person link details for the specified <see cref = "Milestone" /> and
        /// 	<see cref = "Person" />
        /// </summary>
        /// <param name = "milestoneId">An ID of the <see cref = "Milestone" /> to the data be retrieved for.</param>
        /// <param name = "personId">An ID of the <see cref = "Person" /> to the data be retrieved for.</param>
        /// <returns>The <see cref = "MilestonePerson" /> object if found and null otherwise.</returns>
        public static MilestonePerson GetByMilestonePerson(int milestoneId, int personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                var command =
                    new SqlCommand(Constants.ProcedureNames.MilestonePerson.MilestonePersonGetByMilestonePerson,
                                   connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(MilestoneIdParam, milestoneId);
                command.Parameters.AddWithValue(PersonIdParam, personId);

                connection.Open();
                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var result = new List<MilestonePerson>();

                    ReadMilestonePersons(reader, result);

                    return result.Count > 0 ? result[0] : null;
                }
            }
        }

        /// <summary>
        /// 	Retrives the milestone-person link details.
        /// </summary>
        /// <param name = "milestonePersonId">An ID of the milestone-person association.</param>
        /// <returns>The <see cref = "MilestonePerson" /> object if found and null otherwise.</returns>
        public static MilestonePerson MilestonePersonGetByMilestonePersonId(int milestonePersonId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                var command =
                    new SqlCommand(Constants.ProcedureNames.MilestonePerson.MilestonePersonGetByMilestonePersonId,
                                   connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(MilestonePersonIdParam, milestonePersonId);

                connection.Open();
                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var result = new List<MilestonePerson>(1);

                    ReadMilestonePersonAssociations(reader, result);

                    return result.Count > 0 ? result[0] : null;
                }
            }
        }

        /// <summary>
        /// 	Retrieves the milestone-person entries for the specified milestone-person association.
        /// </summary>
        /// <param name = "milestonePersonId">An ID of the milestone-person association.</param>
        /// <returns>A list of the <see cref = "MilestonePersonEntry" /> objects.</returns>
        public static List<MilestonePersonEntry> MilestonePersonEntryListByMilestonePersonId(int milestonePersonId
                                                                                               , SqlConnection connection = null
                                                                                               , SqlTransaction activeTransaction = null
                                                                                             )
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

            using (var command =
                new SqlCommand(Constants.ProcedureNames.MilestonePerson.MilestonePersonEntryListByMilestonePersonId,
                               connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(MilestonePersonIdParam, milestonePersonId);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                if (activeTransaction != null)
                {
                    command.Transaction = activeTransaction;
                }

                using (var reader = command.ExecuteReader())
                {
                    var result = new List<MilestonePersonEntry>();

                    ReadMilestonePersonEntries(reader, result);

                    return result;
                }
            }
        }

        public static bool CheckTimeEntriesForMilestonePerson(int milestonePersonId, DateTime? startDate, DateTime? endDate
                , bool checkStartDateEquality, bool checkEndDateEquality)
        {
            bool result;
            try
            {
                using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
                using (var command = new SqlCommand(Constants.ProcedureNames.MilestonePerson.CheckTimeEntriesForMilestonePerson
                                                    , connection
                                                   )
                      )
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(MilestonePersonIdParam, milestonePersonId);

                    if (startDate.HasValue)
                    {
                        command.Parameters.AddWithValue(StartDateParam, startDate.Value);
                        command.Parameters.AddWithValue(CheckStartDateEqualityParam, checkStartDateEquality);
                    }
                    if (endDate.HasValue)
                    {
                        command.Parameters.AddWithValue(EndDateParam, endDate.Value);
                        command.Parameters.AddWithValue(CheckEndDateEqualityParam, checkEndDateEquality);
                    }

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

        /// <summary>
        /// 	Inserts the specified <see cref = "Milestone" />-<see cref = "Person" /> link to the database.
        /// </summary>
        /// <param name = "milestonePerson">The data to be saved to.</param>
        public static void MilestonePersonInsert(MilestonePerson milestonePerson, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

            using (var command = new SqlCommand(Constants.ProcedureNames.MilestonePerson.MilestonePersonInsert, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(MilestoneIdParam,
                                                milestonePerson.Milestone != null &&
                                                milestonePerson.Milestone.Id.HasValue
                                                    ? (object)milestonePerson.Milestone.Id.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(PersonIdParam,
                                                milestonePerson.Person != null && milestonePerson.Person.Id.HasValue
                                                    ? (object)milestonePerson.Person.Id.Value
                                                    : DBNull.Value);

                var milestonePersonId = new SqlParameter(MilestonePersonIdParam, SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(milestonePersonId);

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
                    milestonePerson.Id = (int)milestonePersonId.Value;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 	Inserts person-milestone details for the specified milestone and person.
        /// </summary>
        /// <param name = "entry">The data to be inserted.</param>
        /// <param name = "userName">A current user.</param>
        public static void MilestonePersonEntryInsert(MilestonePersonEntry entry, string userName, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

            using (var command = new SqlCommand(Constants.ProcedureNames.MilestonePerson.MilestonePersonEntryInsert, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(PersonIdParam,
                                                entry.ThisPerson.Id.HasValue
                                                    ? (object)entry.ThisPerson.Id.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(MilestonePersonIdParam, entry.MilestonePersonId);
                command.Parameters.AddWithValue(LocationParam, entry.Location);
                command.Parameters.AddWithValue(StartDateParam, entry.StartDate);
                command.Parameters.AddWithValue(EndDateParam,
                                                entry.EndDate.HasValue ? (object)entry.EndDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(HoursPerDayParam, entry.HoursPerDay);
                command.Parameters.AddWithValue(PersonRoleIdParam,
                                                entry.Role != null ? (object)entry.Role.Id : DBNull.Value);
                command.Parameters.AddWithValue(AmountParam,
                                                entry.HourlyAmount.HasValue
                                                    ? (object)entry.HourlyAmount.Value.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(UserLoginParam,
                                                !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);

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
        /// 	Removes persons-milestones details for the specified milestone and person.
        /// </summary>
        /// <param name = "milestonePersonId">An ID of the milestone-person association.</param>
        public static void MilestonePersonDeleteEntries(int milestonePersonId, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

            using (var command = new SqlCommand(Constants.ProcedureNames.MilestonePerson.MilestonePersonDeleteEntries, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(MilestonePersonIdParam, milestonePersonId);

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
        /// 	Deletes the specified <see cref = "Milestone" />-<see cref = "Person" /> link from the database.
        /// </summary>
        /// <param name = "milestonePerson">The data to be deleted from.</param>
        public static void MilestonePersonDelete(MilestonePerson milestonePerson)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (
                var command = new SqlCommand(Constants.ProcedureNames.MilestonePerson.MilestonePersonDelete, connection)
                )
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(MilestonePersonIdParam,
                                                milestonePerson.Id.HasValue
                                                    ? (object)milestonePerson.Id.Value
                                                    : DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void SaveMilestonePersonWrapper(MilestonePerson milestonePerson, string userName)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                connection.Open();
                var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                SaveMilestonePerson(milestonePerson, userName, connection, transaction);

                transaction.Commit();
            }
        }

        /// <summary>
        /// 	Saves the specified <see cref = "Milestone" />-<see cref = "Person" /> link to the database.
        /// </summary>
        /// <param name = "milestonePerson">The data to be saved to.</param>
        /// <param name = "userName">A current user.</param>
        /// <remarks>
        /// 	Must be run within a transaction.
        /// </remarks>
        public static void SaveMilestonePerson(MilestonePerson milestonePerson, string userName, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (!milestonePerson.Id.HasValue)
            {
                MilestonePersonInsert(milestonePerson, connection, activeTransaction);
            }

            MilestonePersonDeleteEntries(milestonePerson.Id.Value, connection, activeTransaction);

            foreach (var entry in milestonePerson.Entries)
            {
                entry.ThisPerson = milestonePerson.Person;
                entry.MilestonePersonId = milestonePerson.Id.Value;
                MilestonePersonEntryInsert(entry, userName, connection, activeTransaction);
            }
        }

        private static void ReadMilestonePersons(DbDataReader reader, List<MilestonePerson> result)
        {
            if (reader.HasRows)
            {
                var milestonePersonIdIndex = reader.GetOrdinal(MilestonePersonIdColumn);
                var milestoneIdIndex = reader.GetOrdinal(MilestoneIdColumn);
                var personIdIndex = reader.GetOrdinal(PersonIdColumn);
                var startDateIndex = reader.GetOrdinal(StartDateColumn);
                var endDateIndex = reader.GetOrdinal(EndDateColumn);
                var firstNameIndex = reader.GetOrdinal(FirstNameColumn);
                var lastNameIndex = reader.GetOrdinal(LastNameColumn);
                var projectIdIndex = reader.GetOrdinal(ProjectIdColumn);
                var projectNameIndex = reader.GetOrdinal(ProjectNameColumn);
                var projectStartDateIndex = reader.GetOrdinal(ProjectStartDateColumn);
                var projectEndDateIndex = reader.GetOrdinal(ProjectEndDateColumn);
                var clientIdIndex = reader.GetOrdinal(ClientIdColumn);
                var clientNameIndex = reader.GetOrdinal(ClientNameColumn);
                var milestoneNameIndex = reader.GetOrdinal(MilestoneNameColumn);
                var milestoneStartDateIndex = reader.GetOrdinal(MilestoneStartDateColumn);
                var milestoneProjectedDeliveryDateIndex = reader.GetOrdinal(MilestoneProjectedDeliveryDateColumn);
                var hoursPerDayIndex = reader.GetOrdinal(HoursPerDayColumn);
                var expectedHoursIndex = reader.GetOrdinal(ExpectedHoursColumn);
                var salesCommissionIndex = reader.GetOrdinal(SalesCommissionColumn);
                var personRoleIdIndex = reader.GetOrdinal(PersonRoleIdColumn);
                var personRoleNameIndex = reader.GetOrdinal(PersonRoleNameColumn);
                var amountIndex = reader.GetOrdinal(AmountColumn);
                var isHourlyAmountIndex = reader.GetOrdinal(IsHourlyAmountColumn);
                var discountIndex = reader.GetOrdinal(DiscountColumn);
                var milestoneExpectedHoursIndex = reader.GetOrdinal(MilestoneExpectedHoursColumn);
                var milestoneActualDeliveryDateIndex = reader.GetOrdinal(MilestoneActualDeliveryDateColumn);
                var milestoneHourlyRevenueIndex = reader.GetOrdinal(MilestoneHourlyRevenueColumn);
                var personSeniorityIdIndex = reader.GetOrdinal(PersonSeniorityIdColumn);

                int projectStatusIdIndex;
                try
                {
                    projectStatusIdIndex = reader.GetOrdinal(ProjectStatusIdColumn);
                }
                catch
                {
                    projectStatusIdIndex = -1;
                }

                while (reader.Read())
                {
                    var milestonePerson = new MilestonePerson { Id = reader.GetInt32(milestonePersonIdIndex) };

                    // Person on milestone
                    var entry = new MilestonePersonEntry
                                    {
                                        StartDate = reader.GetDateTime(startDateIndex),
                                        EndDate =
                                            !reader.IsDBNull(endDateIndex)
                                                ? (DateTime?)reader.GetDateTime(endDateIndex)
                                                : null,
                                        HoursPerDay = reader.GetDecimal(hoursPerDayIndex),
                                        ProjectedWorkload = reader.GetDecimal(expectedHoursIndex),
                                        HourlyAmount = !reader.IsDBNull(milestoneHourlyRevenueIndex)
                                                           ? reader.GetDecimal(milestoneHourlyRevenueIndex)
                                                           : 0
                                    };

                    if (!reader.IsDBNull(personRoleIdIndex))
                    {
                        entry.Role = new PersonRole
                                         {
                                             Id = reader.GetInt32(personRoleIdIndex),
                                             Name = reader.GetString(personRoleNameIndex)
                                         };
                        if (!reader.IsDBNull(startDateIndex))
                        {
                            entry.StartDate = reader.GetDateTime(startDateIndex);
                        }
                        else
                        {
                            entry.StartDate = DateTime.MinValue;
                        }
                        if (!reader.IsDBNull(endDateIndex))
                        {
                            entry.EndDate = reader.GetDateTime(endDateIndex);
                        }
                        else
                        {
                            entry.EndDate = DateTime.MaxValue;
                        }
                    }

                    milestonePerson.Entries = new List<MilestonePersonEntry>(1) { entry };

                    // Person details
                    milestonePerson.Person = new Person
                                                 {
                                                     Id = reader.GetInt32(personIdIndex),
                                                     FirstName = reader.GetString(firstNameIndex),
                                                     LastName = reader.GetString(lastNameIndex)
                                                 };

                    // Seniority
                    if (!reader.IsDBNull(personSeniorityIdIndex))
                    {
                        milestonePerson.Person.Seniority = new Seniority { Id = reader.GetInt32(personSeniorityIdIndex) };
                    }

                    // Milestone details
                    var project = new Project
                                      {
                                          Id = reader.GetInt32(projectIdIndex),
                                          Name = reader.GetString(projectNameIndex),
                                          StartDate = reader.GetDateTime(projectStartDateIndex),
                                          EndDate = reader.GetDateTime(projectEndDateIndex),
                                          Discount = reader.GetDecimal(discountIndex)
                                      };
                    milestonePerson.Milestone = new Milestone
                                                    {
                                                        Id = reader.GetInt32(milestoneIdIndex),
                                                        Description = reader.GetString(milestoneNameIndex),
                                                        Amount =
                                                            !reader.IsDBNull(amountIndex)
                                                                ? (decimal?)reader.GetDecimal(amountIndex)
                                                                : null,
                                                        StartDate = reader.GetDateTime(milestoneStartDateIndex),
                                                        ProjectedDeliveryDate =
                                                            reader.GetDateTime(milestoneProjectedDeliveryDateIndex),
                                                        IsHourlyAmount = reader.GetBoolean(isHourlyAmountIndex),
                                                        ExpectedHours = reader.GetDecimal(milestoneExpectedHoursIndex),
                                                        ActualDeliveryDate =
                                                            !reader.IsDBNull(milestoneActualDeliveryDateIndex)
                                                                ? (DateTime?)
                                                                  reader.GetDateTime(milestoneActualDeliveryDateIndex)
                                                                : null,
                                                        Project = project
                                                    };

                    // Project details

                    if (projectStatusIdIndex >= 0)
                        milestonePerson.Milestone.Project.Status = new ProjectStatus { Id = reader.GetInt32(projectStatusIdIndex) };

                    // Client details
                    milestonePerson.Milestone.Project.Client = new Client
                                                                   {
                                                                       Id = reader.GetInt32(clientIdIndex),
                                                                       Name = reader.GetString(clientNameIndex)
                                                                   };

                    // Sales Commission
                    milestonePerson.Milestone.Project.SalesCommission =
                        new List<Commission>
                            {
                                new Commission {FractionOfMargin = reader.GetDecimal(salesCommissionIndex)}
                            };

                    result.Add(milestonePerson);
                }
            }
        }

        private static void ReadMilestonePersonsForTEbyProjectReport(DbDataReader reader, List<MilestonePerson> result)
        {
            if (reader.HasRows)
            {
                var milestonePersonIdIndex = reader.GetOrdinal(MilestonePersonIdColumn);
                var personIdIndex = reader.GetOrdinal(PersonIdColumn);
                var firstNameIndex = reader.GetOrdinal(FirstNameColumn);
                var lastNameIndex = reader.GetOrdinal(LastNameColumn);

                while (reader.Read())
                {
                    var milestonePerson = new MilestonePerson { Id = reader.GetInt32(milestonePersonIdIndex) };

                     

                    // Person details
                    milestonePerson.Person = new Person
                    {
                        Id = reader.GetInt32(personIdIndex),
                        FirstName = reader.GetString(firstNameIndex),
                        LastName = reader.GetString(lastNameIndex)
                    };

                    result.Add(milestonePerson);
                }
            }
        }

        private static void ReadMilestonePersonsShort(DbDataReader reader, List<Project> projects)
        {
            if (reader.HasRows)
            {
                var milestonePersonIdIndex = reader.GetOrdinal(MilestonePersonIdColumn);
                var personIdIndex = reader.GetOrdinal(PersonIdColumn);
                var firstNameIndex = reader.GetOrdinal(FirstNameColumn);
                var lastNameIndex = reader.GetOrdinal(LastNameColumn);
                var projectIdIndex = reader.GetOrdinal(ProjectIdColumn);
                var seniorityIdIndex = reader.GetOrdinal(PersonSeniorityIdColumn);

                while (reader.Read())
                {
                    var project = new Project { Id = reader.GetInt32(projectIdIndex) };

                    var milestonePerson =
                        new MilestonePerson
                            {
                                Id = reader.GetInt32(milestonePersonIdIndex),
                                Person = new Person
                                             {
                                                 Id = reader.GetInt32(personIdIndex),
                                                 FirstName = reader.GetString(firstNameIndex),
                                                 LastName = reader.GetString(lastNameIndex),
                                                 Seniority = new Seniority
                                                 {
                                                     Id = reader.GetInt32(seniorityIdIndex)
                                                 }
                                             }
                            };

                    var i = projects.IndexOf(project);
                    projects[i].ProjectPersons.Add(milestonePerson);
                }
            }
        }

        private static List<MilestonePersonEntry> ReadDetailedMilestonePersonEntries(DbDataReader reader)
        {
            var result = new List<MilestonePersonEntry>();

            if (reader.HasRows)
            {
                var hoursPerDayIndex = reader.GetOrdinal(HoursPerDayColumn);
                var milestoneIdIndex = reader.GetOrdinal(MilestoneIdColumn);
                var milestoneNameIndex = reader.GetOrdinal(MilestoneNameColumn);
                var milestonePersonIdIndex = reader.GetOrdinal(MilestonePersonIdColumn);
                var endDateIndex = reader.GetOrdinal(EndDateColumn);
                var startDateIndex = reader.GetOrdinal(StartDateColumn);
                var projectIdIndex = reader.GetOrdinal(ProjectIdColumn);
                var projectNameIndex = reader.GetOrdinal(ProjectNameColumn);
                var clientIdIndex = reader.GetOrdinal(ClientIdColumn);
                var clientNameIndex = reader.GetOrdinal(ClientNameColumn);
                var projectStatusIdIndex = reader.GetOrdinal(ProjectStatusIdColumn);
                var projectNumberIndex = reader.GetOrdinal(ProjectNumberColumn);
                var projectManagerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerId);
                var projectManagerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerFirstName);
                var projectManagerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerLastName);

                while (reader.Read())
                {
                    // Client details
                    var client = new Client
                                     {
                                         Id = reader.GetInt32(clientIdIndex),
                                         Name = reader.GetString(clientNameIndex)
                                     };

                    // Project details
                    var project = new Project
                                      {
                                          Id = reader.GetInt32(projectIdIndex),
                                          Name = reader.GetString(projectNameIndex),
                                          ProjectNumber = reader.GetString(projectNumberIndex),
                                          Status = new ProjectStatus
                                                       {
                                                           Id = reader.GetInt32(projectStatusIdIndex)
                                                       },
                                          Client = client,
                                          ProjectManager = new Person
                                          {
                                              Id = reader.GetInt32(projectManagerIdIndex),
                                              LastName = reader.GetString(projectManagerLastNameIndex),
                                              FirstName = reader.GetString(projectManagerFirstNameIndex)
                                          }
                                      };

                    // Milestone details
                    var milestone = new Milestone
                                        {
                                            Id = reader.GetInt32(milestoneIdIndex),
                                            Description = reader.GetString(milestoneNameIndex),
                                            Project = project
                                        };

                    // Person on milestone
                    var entry = new MilestonePersonEntry
                                    {
                                        MilestonePersonId = reader.GetInt32(milestonePersonIdIndex),
                                        StartDate = reader.GetDateTime(startDateIndex),
                                        EndDate = !reader.IsDBNull(endDateIndex)
                                                      ? (DateTime?)reader.GetDateTime(endDateIndex)
                                                      : null,
                                        HoursPerDay = reader.GetDecimal(hoursPerDayIndex),
                                        ParentMilestone = milestone
                                    };

                    result.Add(entry);
                }
            }

            return result;
        }

        private static void ReadMilestonePersonAssociations(DbDataReader reader, List<MilestonePerson> result)
        {
            if (reader.HasRows)
            {
                var milestonePersonIdIndex = reader.GetOrdinal(MilestonePersonIdColumn);
                var milestoneIdIndex = reader.GetOrdinal(MilestoneIdColumn);
                var personIdIndex = reader.GetOrdinal(PersonIdColumn);
                var isHourlyAmountIndex = reader.GetOrdinal(IsHourlyAmountColumn);
                var milestoneNameIndex = reader.GetOrdinal(MilestoneNameColumn);
                var milestoneStartDateIndex = reader.GetOrdinal(MilestoneStartDateColumn);
                var milestoneProjectedDeliveryDateIndex = reader.GetOrdinal(MilestoneProjectedDeliveryDateColumn);
                var projectIdIndex = reader.GetOrdinal(ProjectIdColumn);
                var projectNameIndex = reader.GetOrdinal(ProjectNameColumn);
                var projectStartDateIndex = reader.GetOrdinal(ProjectStartDateColumn);
                var projectEndDateIndex = reader.GetOrdinal(ProjectEndDateColumn);
                var clientIdIndex = reader.GetOrdinal(ClientIdColumn);
                var clientNameIndex = reader.GetOrdinal(ClientNameColumn);
                var personSeniorityIdIndex = reader.GetOrdinal(PersonSeniorityIdColumn);

                while (reader.Read())
                {
                    var project = new Project
                                      {
                                          Id = reader.GetInt32(projectIdIndex),
                                          Name = reader.GetString(projectNameIndex),
                                          Client = new Client
                                                       {
                                                           Id = reader.GetInt32(clientIdIndex),
                                                           Name = reader.GetString(clientNameIndex)
                                                       },
                                          StartDate = !reader.IsDBNull(projectStartDateIndex)
                                                          ? (DateTime?)
                                                            reader.GetDateTime(projectStartDateIndex)
                                                          : null,
                                          EndDate = !reader.IsDBNull(projectEndDateIndex)
                                                        ? (DateTime?)
                                                          reader.GetDateTime(projectEndDateIndex)
                                                        : null
                                      };

                    var milestone = new Milestone
                                        {
                                            Id = reader.GetInt32(milestoneIdIndex),
                                            IsHourlyAmount = reader.GetBoolean(isHourlyAmountIndex),
                                            Description = reader.GetString(milestoneNameIndex),
                                            StartDate = reader.GetDateTime(milestoneStartDateIndex),
                                            ProjectedDeliveryDate =
                                                reader.GetDateTime(milestoneProjectedDeliveryDateIndex),
                                            Project = project
                                        };

                    var association = new MilestonePerson
                                          {
                                              Id = reader.GetInt32(milestonePersonIdIndex),
                                              Milestone = milestone,
                                              Person = new Person { Id = reader.GetInt32(personIdIndex) }
                                          };

                    if (!reader.IsDBNull(personSeniorityIdIndex))
                    {
                        association.Person.Seniority = new Seniority { Id = reader.GetInt32(personSeniorityIdIndex) };
                    }

                    result.Add(association);
                }
            }
        }

        private static void ReadMilestonePersonEntries(SqlDataReader reader, List<MilestonePersonEntry> result)
        {
            if (reader.HasRows)
            {
                var milestonePersonIdIndex = reader.GetOrdinal(MilestonePersonIdColumn);
                var startDateIndex = reader.GetOrdinal(StartDateColumn);
                var endDateIndex = reader.GetOrdinal(EndDateColumn);
                var personRoleIdIndex = reader.GetOrdinal(PersonRoleIdColumn);
                var personRoleNameIndex = reader.GetOrdinal(PersonRoleNameColumn);
                var amountIndex = reader.GetOrdinal(AmountColumn);
                var hoursPerDayIndex = reader.GetOrdinal(HoursPerDayColumn);
                var personVacationsOnMilestoneIndex = reader.GetOrdinal(PersonVacationsOnMilestoneColumn);
                var expectedHoursIndex = reader.GetOrdinal(ExpectedHoursColumn);
                var personSeniorityIdIndex = reader.GetOrdinal(PersonSeniorityIdColumn);
                var personIdIndex = reader.GetOrdinal(PersonIdColumn);
                var locationIndex = reader.GetOrdinal(LocationColumn);

                while (reader.Read())
                {
                    var entry =
                        new MilestonePersonEntry
                            {
                                MilestonePersonId = reader.GetInt32(milestonePersonIdIndex),
                                StartDate = reader.GetDateTime(startDateIndex),
                                EndDate =
                                    !reader.IsDBNull(endDateIndex)
                                        ? (DateTime?)reader.GetDateTime(endDateIndex)
                                        : null,
                                HourlyAmount =
                                    !reader.IsDBNull(amountIndex)
                                        ? (decimal?)reader.GetDecimal(amountIndex)
                                        : null,
                                HoursPerDay = reader.GetDecimal(hoursPerDayIndex),
                                VacationDays = reader.GetInt32(personVacationsOnMilestoneIndex),
                                ProjectedWorkload = reader.GetDecimal(expectedHoursIndex),
                                ThisPerson = new Person
                                                 {
                                                     Id = reader.GetInt32(personIdIndex),
                                                     Seniority = new Seniority
                                                                     {
                                                                         Id = reader.GetInt32(personSeniorityIdIndex)
                                                                     }
                                                 },
                                Location = !reader.IsDBNull(locationIndex)
                                        ? reader.GetString(locationIndex)
                                        : null
                            };

                    if (!reader.IsDBNull(personRoleIdIndex))
                        entry.Role =
                            new PersonRole
                                {
                                    Id = reader.GetInt32(personRoleIdIndex),
                                    Name = reader.GetString(personRoleNameIndex)
                                };

                    result.Add(entry);
                }
            }
        }

        #endregion
    }
}


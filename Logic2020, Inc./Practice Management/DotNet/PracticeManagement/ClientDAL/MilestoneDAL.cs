using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
    /// <summary>
    /// Access milestone data in database
    /// </summary>
    public static class MilestoneDAL
    {
        #region Constants

        #region Parameters

        private const string ProjectIdParam = "@ProjectId";
        private const string MilestoneIdParam = "@MilestoneId";
        private const string MilestonePersonIdParam = "@MilestonePersonId";
        private const string DescriptionParam = "@Description";
        private const string AmountParam = "@Amount";
        private const string StartDateParam = "@StartDate";
        private const string ProjectedDeliveryDateParam = "@ProjectedDeliveryDate";
        private const string ActualDeliveryDateParam = "@ActualDeliveryDate";
        private const string IsHourlyAmountParam = "@IsHourlyAmount";
        private const string ShiftDaysParam = "@ShiftDays";
        private const string PersonIdParam = "@PersonId";
        private const string MoveFutureMilestonesParam = "@MoveFutureMilestones";
        private const string CloneDurationParam = "@CloneDuration";
        private const string MilestoneCloneIdParam = "@MilestoneCloneId";
        private const string UserLoginParam = "@UserLogin";
        private const string ClientIdParam = "@ClientId";
        private const string LowerBoundParam = "@LowerBound";
        private const string UpperBoundParam = "@UpperBound";


        #endregion

        #region Stored Procedures

        private const string MilestoneListByProjectProcedure = "dbo.MilestoneListByProject";
        private const string MilestoneGetByIdProcedure = "dbo.MilestoneGetById";
        private const string MilestoneInsertProcedure = "dbo.MilestoneInsert";
        private const string MilestoneUpdateProcedure = "dbo.MilestoneUpdate";
        private const string MilestoneDeleteProcedure = "dbo.MilestoneDelete";
        private const string MilestoneMoveProcedure = "dbo.MilestoneMove";
        private const string MilestoneMoveEndProcedure = "dbo.MilestoneMoveEnd";
        private const string MilestoneCloneProcedure = "dbo.MilestoneClone";
        private const string DefaultMileStoneInsertProcedure = "dbo.DefaultMilestoneSettingInsert";
        private const string DefaultMileStoneGetProcedure = "dbo.GetDefaultMilestoneSetting";
        public const string GetPersonMilestonesAfterTerminationDateProcedure = "dbo.GetPersonMilestonesAfterTerminationDate";

        #endregion

        #region Columns

        private const string ProjectStartDateColumn = "ProjectStartDate";
        private const string ProjectEndDateColumn = "ProjectEndDate";
        private const string IsHourlyAmountColumn = "IsHourlyAmount";
        private const string ExpectedHoursColumn = "ExpectedHours";
        private const string DiscountColumn = "Discount";
        private const string SalesCommissionColumn = "SalesCommission";
        private const string PersonCountColumn = "PersonCount";
        private const string ProjectedDurationColumn = "ProjectedDuration";
        private const string ClientIdColumn = "ClientId";
        private const string ProjectIdColumn = "ProjectId";
        private const string MilestoneIdColumn = "MilestoneId";
        private const string ModifiedDateColumn = "ModifiedDate";
        private const string LowerBoundColumn = "LowerBound";
        private const string UpperBoundColumn = "UpperBound";

        #endregion

        #endregion

        /// <summary>
        /// Saves Default Project-milestone details into DB. Persons not assigned to any Project-Milestone 
        /// can enter time entery for this default Project Milestone.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="ProjectId"></param>
        /// <param name="MileStoneId"></param>
        public static void SaveDefaultMilestone(int? clientId, int? ProjectId, int? MilestoneId, int? lowerBound, int? upperBound)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(DefaultMileStoneInsertProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                if (clientId.HasValue)
                    command.Parameters.AddWithValue(ClientIdParam, clientId.Value);
                if (ProjectId.HasValue)
                    command.Parameters.AddWithValue(ProjectIdParam, ProjectId.Value);
                if (MilestoneId.HasValue)
                    command.Parameters.AddWithValue(MilestoneIdParam, MilestoneId.Value);
                if (lowerBound.HasValue)
                    command.Parameters.AddWithValue(LowerBoundParam, lowerBound.Value);
                if (lowerBound.HasValue)
                    command.Parameters.AddWithValue(UpperBoundParam, upperBound.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static DefaultMilestone GetDefaultMilestone()
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(DefaultMileStoneGetProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = ReadDefaultMilestone(reader);

                    return result;
                }
            }
        }


        /// <summary>
        /// Lists <see cref="Milestones"/> by the specified Project.
        /// </summary>
        /// <param name="projectId">An ID of the project to the data be retrived for.</param>
        /// <returns>The list of the <see cref="Milestone"/> objects.</returns>
        public static List<Milestone> MilestoneListByProject(int projectId)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(MilestoneListByProjectProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ProjectIdParam, projectId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<Milestone> result = new List<Milestone>();

                    ReadMilestones(reader, result);

                    return result;
                }
            }
        }

        public static List<Milestone> GetPersonMilestonesAfterTerminationDate(int personId, DateTime terminationDate)
        {

            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(GetPersonMilestonesAfterTerminationDateProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.TerminationDate, terminationDate);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    List<Milestone> result = new List<Milestone>(1);

                    ReadMilestonesForPersonTermination(reader, result);

                    return result;
                }
            }
        }

        private static void ReadMilestonesForPersonTermination(SqlDataReader reader, List<Milestone> result)
        {
            if (reader.HasRows)
            {
                int projectIdIndex = reader.GetOrdinal("ProjectId");
                int milestoneIdIndex = reader.GetOrdinal("MilestoneId");
                int descriptionIndex = reader.GetOrdinal("MilestoneName");
                int projectNameIndex = reader.GetOrdinal("ProjectName");

                while (reader.Read())
                {
                    Milestone milestone = new Milestone();

                    milestone.Id = reader.GetInt32(milestoneIdIndex);
                    milestone.Description = reader.GetString(descriptionIndex);

                    milestone.Project = new Project();
                    milestone.Project.Id = reader.GetInt32(projectIdIndex);
                    milestone.Project.Name = reader.GetString(projectNameIndex);
                    result.Add(milestone);
                }
            }
        }

        /// <summary>
        /// Retrieves a <see cref="Milestone"/> by the specified ID.
        /// </summary>
        /// <param name="milestoneId">An ID of the <see cref="Milestone"/> to be retrieved.</param>
        /// <returns>The <see cref="Milestone"/> object when found and null otherwise.</returns>
        public static Milestone GetById(int milestoneId)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(MilestoneGetByIdProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(MilestoneIdParam, milestoneId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    List<Milestone> result = new List<Milestone>(1);

                    ReadMilestones(reader, result);

                    return result.Count > 0 ? result[0] : null;
                }
            }
        }

        /// <summary>
        /// Inserts a <see cref="Milestone"/> into the database.
        /// </summary>
        /// <param name="milestone">The <see cref="Milestine"/> to be inserted to.</param>
        /// <param name="userName">A current user.</param>
        public static void MilestoneInsert(Milestone milestone, string userName)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(MilestoneInsertProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ProjectIdParam, (milestone.Project != null && milestone.Project.Id.HasValue) ?
                                                                        (object)milestone.Project.Id.Value : DBNull.Value
                                               );
                command.Parameters.AddWithValue(DescriptionParam, !string.IsNullOrEmpty(milestone.Description) ? (object)milestone.Description : DBNull.Value);
                command.Parameters.AddWithValue(AmountParam, milestone.Amount.HasValue ? (object)milestone.Amount.Value.Value : DBNull.Value);
                command.Parameters.AddWithValue(StartDateParam, milestone.StartDate);
                command.Parameters.AddWithValue(ProjectedDeliveryDateParam, milestone.ProjectedDeliveryDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsChargeable, milestone.IsChargeable);
                command.Parameters.AddWithValue(Constants.ParameterNames.ConsultantsCanAdjust, milestone.ConsultantsCanAdjust);
                command.Parameters.AddWithValue(ActualDeliveryDateParam, milestone.ActualDeliveryDate.HasValue ? (object)milestone.ActualDeliveryDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(IsHourlyAmountParam, milestone.IsHourlyAmount);
                command.Parameters.AddWithValue(UserLoginParam, !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);

                SqlParameter milestoneIdParam = new SqlParameter(MilestoneIdParam, SqlDbType.Int);
                milestoneIdParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(milestoneIdParam);

                connection.Open();

                SqlTransaction trn = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                command.Transaction = trn;

                command.ExecuteNonQuery();

                milestone.Id = (int)milestoneIdParam.Value;

                trn.Commit();
            }
        }

        /// <summary>
        /// Update a <see cref="Milestone"/> in the database.
        /// </summary>
        /// <param name="milestone">The <see cref="Milestine"/> to be updated.</param>
        /// <param name="userName">A current user.</param>
        public static void MilestoneUpdate(Milestone milestone, string userName)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(MilestoneUpdateProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(MilestoneIdParam, milestone.Id.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ConsultantsCanAdjust, milestone.ConsultantsCanAdjust);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsChargeable, milestone.IsChargeable);
                command.Parameters.AddWithValue(ProjectIdParam,
                    milestone.Project != null && milestone.Project.Id.HasValue ?
                    (object)milestone.Project.Id.Value : DBNull.Value);
                command.Parameters.AddWithValue(DescriptionParam,
                    !string.IsNullOrEmpty(milestone.Description) ?
                    (object)milestone.Description : DBNull.Value);
                command.Parameters.AddWithValue(AmountParam,
                    milestone.Amount.HasValue ? (object)milestone.Amount.Value.Value : DBNull.Value);
                command.Parameters.AddWithValue(StartDateParam, milestone.StartDate);
                command.Parameters.AddWithValue(ProjectedDeliveryDateParam, milestone.ProjectedDeliveryDate);
                command.Parameters.AddWithValue(ActualDeliveryDateParam,
                    milestone.ActualDeliveryDate.HasValue ?
                    (object)milestone.ActualDeliveryDate.Value : DBNull.Value);
                command.Parameters.AddWithValue(IsHourlyAmountParam, milestone.IsHourlyAmount);
                command.Parameters.AddWithValue(UserLoginParam,
                    !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);

                connection.Open();

                SqlTransaction trn = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                command.Transaction = trn;

                command.ExecuteNonQuery();

                trn.Commit();
            }
        }

        /// <summary>
        /// Deletes a <see cref="Milestone"/> from the database.
        /// </summary>
        /// <param name="milestone">The <see cref="Milestine"/> to be deleted from.</param>
        /// <param name="userName">A current user.</param>
        public static void MilestoneDelete(Milestone milestone, string userName)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(MilestoneDeleteProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(MilestoneIdParam, milestone.Id.Value);
                command.Parameters.AddWithValue(UserLoginParam,
                    !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Moves the specified milestone and optionally future milestones forward and backward.
        /// </summary>
        /// <param name="milestoneId">An ID of the <see cref="Milestone"/> to be moved.</param>
        /// <param name="shiftDays">A number of days to move.</param>
        /// <param name="moveFutureMilestones">Determines whether future milestones must be moved too.</param>
        public static void MilestoneMove(int milestoneId, int shiftDays, bool moveFutureMilestones)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(MilestoneMoveProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(MilestoneIdParam, milestoneId);
                command.Parameters.AddWithValue(ShiftDaysParam, shiftDays);
                command.Parameters.AddWithValue(MoveFutureMilestonesParam, moveFutureMilestones);

                connection.Open();

                SqlTransaction trnScope = connection.BeginTransaction();
                command.Transaction = trnScope;

                command.ExecuteNonQuery();

                trnScope.Commit();
            }
        }

        /// <summary>
        /// Moves the specified milestone end date forward and backward.
        /// </summary>
        /// <param name="milestoneId">An ID of the <see cref="Milestone"/> to be moved.</param>
        /// <param name="shiftDays">A number of days to move.</param>
        public static void MilestoneMoveEnd(int milestoneId, int milestonePersonId, int shiftDays)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(MilestoneMoveEndProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(MilestoneIdParam, milestoneId);
                command.Parameters.AddWithValue(ShiftDaysParam, shiftDays);
                command.Parameters.AddWithValue(MilestonePersonIdParam, milestonePersonId);

                connection.Open();

                SqlTransaction trnScope = connection.BeginTransaction();
                command.Transaction = trnScope;

                command.ExecuteNonQuery();

                trnScope.Commit();
            }
        }

        /// <summary>
        /// Clones a specified milestones and set a specified duration to a new one.
        /// </summary>
        /// <param name="milestoneId">An ID of the milestone to be cloned.</param>
        /// <param name="cloneDuration">A clone's duration.</param>
        /// <returns>An ID of a new milestone.</returns>
        public static int MilestoneClone(int milestoneId, int cloneDuration)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(MilestoneCloneProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(MilestoneIdParam, milestoneId);
                command.Parameters.AddWithValue(CloneDurationParam, cloneDuration);

                SqlParameter cloneIdParam =
                    new SqlParameter(MilestoneCloneIdParam, SqlDbType.Int);
                cloneIdParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(cloneIdParam);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();

                    return (int)cloneIdParam.Value;
                }
                catch (SqlException ex)
                {
                    throw new DataAccessException(ex);
                }
            }
        }

        /// <summary>
        /// Reades the records from the DbDataReader
        /// </summary>
        /// <param name="reader">The reader the data be read from.</param>
        /// <param name="result">The list of <see cref="Milestone"/> objects.</param>
        private static void ReadMilestones(DbDataReader reader, List<Milestone> result)
        {
            if (reader.HasRows)
            {
                int clientIdIndex = reader.GetOrdinal("ClientId");
                int projectIdIndex = reader.GetOrdinal("ProjectId");
                int milestoneIdIndex = reader.GetOrdinal("MilestoneId");
                int descriptionIndex = reader.GetOrdinal("Description");
                int amountIndex = reader.GetOrdinal("Amount");
                int startDateIndex = reader.GetOrdinal("StartDate");
                int projectedDeliveryDateIndex = reader.GetOrdinal("ProjectedDeliveryDate");
                int actualDeliveryDateIndex = reader.GetOrdinal("ActualDeliveryDate");
                int clientNameIndex = reader.GetOrdinal("ClientName");
                int projectNameIndex = reader.GetOrdinal("ProjectName");
                int projectStartDateIndex = reader.GetOrdinal(ProjectStartDateColumn);
                int projectEndDateIndex = reader.GetOrdinal(ProjectEndDateColumn);
                int isHourlyAmountIndex = reader.GetOrdinal(IsHourlyAmountColumn);
                int expectedHoursIndex = reader.GetOrdinal(ExpectedHoursColumn);
                int discountIndex = reader.GetOrdinal(DiscountColumn);
                int salesCommissionIndex = reader.GetOrdinal(SalesCommissionColumn);
                int personCountIndex = reader.GetOrdinal(PersonCountColumn);
                int projectedDurationIndex = reader.GetOrdinal(ProjectedDurationColumn);
                int milestoneIsChargeableIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneIsChargeable);
                int consultantsCanAdjustIndex = reader.GetOrdinal(Constants.ColumnNames.ConsultantsCanAdjust);
                int isMarginColorInfoEnabledIndex = -1;
                try
                {
                    isMarginColorInfoEnabledIndex = reader.GetOrdinal(Constants.ColumnNames.IsMarginColorInfoEnabledColumn);
                }
                catch
                {
                    isMarginColorInfoEnabledIndex = -1;
                }

                while (reader.Read())
                {
                    Milestone milestone = new Milestone();

                    milestone.Id = reader.GetInt32(milestoneIdIndex);
                    milestone.Description = reader.GetString(descriptionIndex);
                    milestone.Amount =
                        !reader.IsDBNull(amountIndex) ? (decimal?)reader.GetDecimal(amountIndex) : null;
                    milestone.StartDate = reader.GetDateTime(startDateIndex);
                    milestone.ProjectedDeliveryDate = reader.GetDateTime(projectedDeliveryDateIndex);
                    milestone.ActualDeliveryDate =
                        !reader.IsDBNull(actualDeliveryDateIndex) ?
                        (DateTime?)reader.GetDateTime(actualDeliveryDateIndex) : null;
                    milestone.IsHourlyAmount = reader.GetBoolean(isHourlyAmountIndex);
                    milestone.ExpectedHours = reader.GetDecimal(expectedHoursIndex);
                    milestone.PersonCount = reader.GetInt32(personCountIndex);
                    milestone.ProjectedDuration = reader.GetInt32(projectedDurationIndex);
                    milestone.IsChargeable = reader.GetBoolean(milestoneIsChargeableIndex);
                    milestone.ConsultantsCanAdjust = reader.GetBoolean(consultantsCanAdjustIndex);

                    milestone.Project = new Project();
                    milestone.Project.Id = reader.GetInt32(projectIdIndex);
                    milestone.Project.Name = reader.GetString(projectNameIndex);
                    milestone.Project.Discount = reader.GetDecimal(discountIndex);
                    milestone.Project.StartDate = reader.GetDateTime(projectStartDateIndex);
                    milestone.Project.EndDate = reader.GetDateTime(projectEndDateIndex);

                    milestone.Project.Client = new Client();
                    milestone.Project.Client.Id = reader.GetInt32(clientIdIndex);
                    milestone.Project.Client.Name = reader.GetString(clientNameIndex);

                    if (isMarginColorInfoEnabledIndex >= 0)
                    {
                        try
                        {
                            milestone.Project.Client.IsMarginColorInfoEnabled = reader.GetBoolean(isMarginColorInfoEnabledIndex);
                        }
                        catch
                        {

                        }
                    }

                    milestone.Project.SalesCommission =
                        new List<Commission>()
						{
							new Commission() {FractionOfMargin = reader.GetDecimal(salesCommissionIndex)}
						};

                    result.Add(milestone);
                }
            }
        }

        private static DefaultMilestone ReadDefaultMilestone(SqlDataReader reader)
        {
            DefaultMilestone defaultMilestone = null;
            if (reader.HasRows)
            {
                defaultMilestone = new DefaultMilestone();
                int ClientIdIndex = reader.GetOrdinal(ClientIdColumn);
                int ProjectIdIndex = reader.GetOrdinal(ProjectIdColumn);
                int MilestoneIdIndex = reader.GetOrdinal(MilestoneIdColumn);
                int ModifiedDateIndex = reader.GetOrdinal(ModifiedDateColumn);
                int LowerBoundIndex = reader.GetOrdinal(LowerBoundColumn);
                int UpperBoundIndex = reader.GetOrdinal(UpperBoundColumn);

                reader.Read();

                if (!reader.IsDBNull(ClientIdIndex))
                    defaultMilestone.ClientId = reader.GetInt32(ClientIdIndex);
                if (!reader.IsDBNull(ProjectIdIndex))
                    defaultMilestone.ProjectId = reader.GetInt32(ProjectIdIndex);
                if (!reader.IsDBNull(MilestoneIdIndex))
                    defaultMilestone.MilestoneId = reader.GetInt32(MilestoneIdIndex);
                if (!reader.IsDBNull(ModifiedDateIndex))
                    defaultMilestone.ModifiedDate = reader.GetDateTime(ModifiedDateIndex);
                if (!reader.IsDBNull(LowerBoundIndex))
                    defaultMilestone.LowerBound = reader.GetInt32(LowerBoundIndex);
                if (!reader.IsDBNull(UpperBoundIndex))
                    defaultMilestone.UpperBound = reader.GetInt32(UpperBoundIndex);
            }
            return defaultMilestone;
        }
    }
}


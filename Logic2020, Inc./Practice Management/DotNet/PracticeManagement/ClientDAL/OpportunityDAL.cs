using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;
using System.Data.SqlTypes;
using DataTransferObjects.ContextObjects;

namespace DataAccess
{
    /// <summary>
    /// 	Provides an access to the Opportunity database table.
    /// </summary>
    public static class OpportunityDAL
    {
        #region Methods

        /// <summary>
        /// 	Retrieves a list of the opportunities by the specified conditions.
        /// </summary>
        /// <param name = "activeOnly">Determines whether only active opportunities must are retrieved.</param>
        /// <param name = "looked">Determines a text to be searched within the opportunity name.</param>
        /// <param name = "clientId">Determines a client to retrieve the opportunities for.</param>
        /// <param name = "salespersonId">Determines a salesperson to retrieve the opportunities for.</param>
        /// <param name = "currentId"></param>
        /// <returns>A list of the <see cref = "Opportunity" /> objects.</returns>
        public static List<Opportunity> OpportunityListAll(OpportunityListContext context)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Opportunitites.OpportunityListAll, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ActiveOnlyParam, context.ActiveClientsOnly);
                command.Parameters.AddWithValue(Constants.ParameterNames.LookedParam,
                                                !string.IsNullOrEmpty(context.SearchText) ? (object)context.SearchText : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdParam,
                                                context.ClientId.HasValue ? (object)context.ClientId.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.SalespersonIdParam,
                                                context.SalespersonId.HasValue ? (object)context.SalespersonId.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.CurrentId,
                                                context.CurrentId.HasValue ? (object)context.CurrentId.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.TargetPerson,
                                                (object)context.TargetPersonId ?? DBNull.Value);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<Opportunity>();
                    ReadOpportunities(reader, result);
                    return result;
                }
            }
        }


        public static List<OpportunityPriority> GetOpportunityPrioritiesListAll()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Opportunitites.OpportunityPrioritiesListAll, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<OpportunityPriority>();
                    ReadOpportunityPriorityListAll(reader, result);
                    return result;
                }
            }
            
        }

        public static List<Opportunity> OpportunityListAllShort(OpportunityListContext context)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Opportunitites.OpportunityListAllShort, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ActiveOnlyParam, context.ActiveClientsOnly);


                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<Opportunity>();
                    ReadOpportunityListAllShort(reader, result);
                    return result;
                }
            }
        }

        private static void ReadOpportunityPriorityListAll(DbDataReader reader, List<OpportunityPriority> result)
        {
            if (reader.HasRows)
            {
                var priorityIndex = reader.GetOrdinal(Constants.ColumnNames.PriorityColumn);
                var descriptionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DescriptionColumn);

                while (reader.Read())
                {
                    // Reading the item
                    var opportunityPriority =
                        new OpportunityPriority
                        {
                           
                            Priority = reader.GetString(priorityIndex),
                            Description = reader.GetString(descriptionIdIndex)
                           
                        };

                    result.Add(opportunityPriority);

                }
            }
        }

        private static void ReadOpportunityListAllShort(DbDataReader reader, List<Opportunity> result)
        {
            if (reader.HasRows)
            {
                var opportunityIdIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityIdColumn);
                var nameIndex = reader.GetOrdinal(Constants.ColumnNames.NameColumn);
                var priorityIndex = reader.GetOrdinal(Constants.ColumnNames.PriorityColumn);
                var clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
                var clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
                var opportunityIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityIndexColumn);
                var groupIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn);
                var groupNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn);
                var salespersonIdIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonIdColumn);
                var salespersonFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonFirstNameColumn);
                var salespersonLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonLastNameColumn);
                var ownerIdIndex = reader.GetOrdinal(Constants.ColumnNames.OwnerIdColumn);
                var ownerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.OwnerFirstNameColumn);
                var ownerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.OwnerLastNameColumn);
                var EstimatedRevenueiIndex = reader.GetOrdinal(Constants.ColumnNames.EstimatedRevenueColumn);

                while (reader.Read())
                {
                    // Reading the item
                    var opportunity =
                        new Opportunity
                        {
                            Id = reader.GetInt32(opportunityIdIndex),
                            Name = reader.GetString(nameIndex),
                            Priority = reader.GetString(priorityIndex)[0],
                            Client = new Client
                            {
                                Id = reader.GetInt32(clientIdIndex),
                                Name = reader.GetString(clientNameIndex)
                            },

                            OpportunityIndex =
                                !reader.IsDBNull(opportunityIndex) ? (int?)reader.GetInt32(opportunityIndex) : null,
                            Salesperson =
                                !reader.IsDBNull(salespersonIdIndex)
                                    ? new Person
                                    {
                                        Id = reader.GetInt32(salespersonIdIndex),
                                        FirstName = reader.GetString(salespersonFirstNameIndex),
                                        LastName = reader.GetString(salespersonLastNameIndex),
                                    }
                                    : null,
                            Owner =
                                    !reader.IsDBNull(ownerIdIndex)
                                        ? new Person
                                        {
                                            Id = reader.GetInt32(ownerIdIndex),
                                            LastName = !reader.IsDBNull(ownerLastNameIndex) ? reader.GetString(ownerLastNameIndex) : null,
                                            FirstName = !reader.IsDBNull(ownerFirstNameIndex) ? reader.GetString(ownerFirstNameIndex) : null,
                                        }
                                        : null,
                            Group = !reader.IsDBNull(groupIdIndex)
                                        ? new ProjectGroup
                                        {
                                            Id = reader.GetInt32(groupIdIndex),
                                            Name = reader.GetString(groupNameIndex)
                                        }
                                        : null
                        };

                    if (!reader.IsDBNull(EstimatedRevenueiIndex))
                    {
                        opportunity.EstimatedRevenue = reader.GetDecimal(EstimatedRevenueiIndex);
                    }

                    result.Add(opportunity);

                }
            }
        }

        /// <summary>
        /// 	Retruves an <see cref = "Opportunity" /> by a specified ID.
        /// </summary>
        /// <param name = "opportunityId">An ID of the record to be retrieved.</param>
        /// <returns>An <see cref = "Opportunity" /> object if found and null otherwise.</returns>
        public static Opportunity OpportunityGetById(int opportunityId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Opportunitites.OpportunityGetById, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityIdParam, opportunityId);

                connection.Open();
                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var result = new List<Opportunity>(1);
                    ReadOpportunities(reader, result);
                    return result.Count > 0 ? result[0] : null;
                }
            }
        }

        /// <summary>
        /// 	Retrives <see cref = "Opportunity" /> data to be exported to excel.
        /// </summary>
        /// <returns>An <see cref = "Opportunity" /> object if found and null otherwise.</returns>
        public static DataSet OpportunityGetExcelSet()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Opportunitites.OpportunityGetExcelSet, connection))
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
        /// 	Inserts a new <see cref = "Opportunity" /> into the database.
        /// </summary>
        /// <param name = "userName">The name of the current user.</param>
        /// <param name = "opportunity">The data to be inserted.</param>
        public static void OpportunityInsert(Opportunity opportunity, string userName)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Opportunitites.OpportunityInsert, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.NameParam,
                                                !string.IsNullOrEmpty(opportunity.Name)
                                                    ? (object)opportunity.Name
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdParam,
                                                opportunity.Client != null && opportunity.Client.Id.HasValue
                                                    ? (object)opportunity.Client.Id.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.SalespersonIdParam,
                                                opportunity.Salesperson != null && opportunity.Salesperson.Id.HasValue
                                                    ? (object)opportunity.Salesperson.Id.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityStatusIdParam,
                                                opportunity.Status != null
                                                    ? (object)opportunity.Status.Id
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PriorityParam, opportunity.Priority);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedStartDateParam,
                                                opportunity.ProjectedStartDate.HasValue
                                                    ? (object)opportunity.ProjectedStartDate
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedEndDateParam,
                                                opportunity.ProjectedEndDate.HasValue
                                                    ? (object)opportunity.ProjectedEndDate.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.DescriptionParam,
                                                !string.IsNullOrEmpty(opportunity.Description)
                                                    ? (object)opportunity.Description
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdParam,
                                                opportunity.Practice != null
                                                    ? (object)opportunity.Practice.Id
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.BuyerNameParam,
                                                !string.IsNullOrEmpty(opportunity.BuyerName)
                                                    ? (object)opportunity.BuyerName
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PipelineParam,
                                                !string.IsNullOrEmpty(opportunity.Pipeline)
                                                    ? (object)opportunity.Pipeline
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProposedParam,
                                                !string.IsNullOrEmpty(opportunity.Proposed)
                                                    ? (object)opportunity.Proposed
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.SendOutParam,
                                                !string.IsNullOrEmpty(opportunity.SendOut)
                                                    ? (object)opportunity.SendOut
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam,
                                                !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam,
                                                opportunity.ProjectId.HasValue
                                                    ? (object)opportunity.ProjectId.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityIndexParam,
                                                opportunity.OpportunityIndex.HasValue
                                                    ? (object)opportunity.OpportunityIndex.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.EstimatedRevenueParam,
                                                opportunity.EstimatedRevenue != null && opportunity.EstimatedRevenue.HasValue
                                                    ? (object)opportunity.EstimatedRevenue
                                                    : DBNull.Value);


                command.Parameters.AddWithValue(
                    Constants.ParameterNames.OwnerId,
                    opportunity.Owner == null ? (object)DBNull.Value : opportunity.Owner.Id);

                command.Parameters.AddWithValue(
                    Constants.ParameterNames.GroupIdParam,
                    opportunity.Group == null ? (object)DBNull.Value : opportunity.Group.Id);


                var idParam = new SqlParameter(Constants.ParameterNames.OpportunityIdParam, SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(idParam);

                try
                {
                    connection.Open();

                    SqlTransaction trn = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    command.Transaction = trn;

                    command.ExecuteNonQuery();

                    trn.Commit();

                    opportunity.Id = (int)idParam.Value;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 	Updates a new <see cref = "Opportunity" /> in the database.
        /// </summary>
        /// <param name = "userName">The name of the current user.</param>
        /// <param name = "opportunity">The data to be updated.</param>
        public static void OpportunityUpdate(Opportunity opportunity, string userName)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Opportunitites.OpportunityUpdate, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.NameParam,
                                                !string.IsNullOrEmpty(opportunity.Name)
                                                    ? (object)opportunity.Name
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdParam,
                                                opportunity.Client != null && opportunity.Client.Id.HasValue
                                                    ? (object)opportunity.Client.Id.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.SalespersonIdParam,
                                                opportunity.Salesperson != null && opportunity.Salesperson.Id.HasValue
                                                    ? (object)opportunity.Salesperson.Id.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityStatusIdParam,
                                                opportunity.Status != null
                                                    ? (object)opportunity.Status.Id
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PriorityParam, opportunity.Priority);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedStartDateParam,
                                                opportunity.ProjectedStartDate.HasValue
                                                    ? (object)opportunity.ProjectedStartDate
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectedEndDateParam,
                                                opportunity.ProjectedEndDate.HasValue
                                                    ? (object)opportunity.ProjectedEndDate.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.DescriptionParam,
                                                !string.IsNullOrEmpty(opportunity.Description)
                                                    ? (object)opportunity.Description
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdParam,
                                                opportunity.Practice != null
                                                    ? (object)opportunity.Practice.Id
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.BuyerNameParam,
                                                !string.IsNullOrEmpty(opportunity.BuyerName)
                                                    ? (object)opportunity.BuyerName
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PipelineParam,
                                                !string.IsNullOrEmpty(opportunity.Pipeline)
                                                    ? (object)opportunity.Pipeline
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProposedParam,
                                                !string.IsNullOrEmpty(opportunity.Proposed)
                                                    ? (object)opportunity.Proposed
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.SendOutParam,
                                                !string.IsNullOrEmpty(opportunity.SendOut)
                                                    ? (object)opportunity.SendOut
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityIdParam,
                                                opportunity.Id.HasValue ? (object)opportunity.Id.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam,
                                                !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam,
                                                opportunity.ProjectId.HasValue
                                                    ? (object)opportunity.ProjectId.Value
                                                    : DBNull.Value);

                command.Parameters.AddWithValue(Constants.ParameterNames.EstimatedRevenueParam,
                                                opportunity.EstimatedRevenue != null && opportunity.EstimatedRevenue.HasValue
                                                    ? (object)opportunity.EstimatedRevenue
                                                    : DBNull.Value);

                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityIndexParam,
                                                opportunity.OpportunityIndex.HasValue
                                                    ? (object)opportunity.OpportunityIndex.Value
                                                    : DBNull.Value);

                command.Parameters.AddWithValue(
                    Constants.ParameterNames.OwnerId,
                    opportunity.Owner == null ? (object)DBNull.Value : opportunity.Owner.Id);

                command.Parameters.AddWithValue(
                    Constants.ParameterNames.GroupIdParam,
                    opportunity.Group == null ? (object)DBNull.Value : opportunity.Group.Id);

                command.Parameters.AddWithValue(
                    Constants.ParameterNames.PersonIdListParam,
                    opportunity.ProposedPersonIdList == null ? (object)DBNull.Value : opportunity.ProposedPersonIdList);

                try
                {
                    connection.Open();

                    SqlTransaction trn = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    command.Transaction = trn;

                    command.ExecuteNonQuery();

                    trn.Commit();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 	Creates a new <see cref = "Project" /> from the specified <see cref = "Opportunity" />.
        /// </summary>
        /// <param name = "opportunityId">An ID of the opportunity to create a project from.</param>
        /// <param name = "userName">A current user.</param>
        public static int OpportunityConvertToProject(int opportunityId, string userName)
        {
            int res;
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Opportunitites.OpportunityConvertToProject, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityIdParam, opportunityId);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam,
                                                !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);

                var idParam = new SqlParameter(Constants.ParameterNames.ProjectId, SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(idParam);

                try
                {

                    connection.Open();

                    SqlTransaction trn = connection.BeginTransaction();
                    command.Transaction = trn;

                    command.ExecuteNonQuery();

                    trn.Commit();

                    res = (int)idParam.Value;
                }
                catch (SqlException ex)
                {
                    throw new DataAccessException(ex);
                }
            }

            return res;
        }

        private static void ReadOpportunities(DbDataReader reader, List<Opportunity> result)
        {
            if (reader.HasRows)
            {
                var opportunityIdIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityIdColumn);
                var nameIndex = reader.GetOrdinal(Constants.ColumnNames.NameColumn);
                var clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
                var salespersonIdIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonIdColumn);
                var opportunityStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityStatusIdColumn);
                var priorityIndex = reader.GetOrdinal(Constants.ColumnNames.PriorityColumn);
                var projectedStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectedStartDateColumn);
                var projectedEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectedEndDateColumn);
                var opportunityNumberIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityNumberColumn);
                var descriptionIndex = reader.GetOrdinal(Constants.ColumnNames.DescriptionColumn);
                var clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
                var salespersonFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonFirstNameColumn);
                var salespersonLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonLastNameColumn);
                var salespersonStatusIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonStatusColumn);
                var opportunityStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityStatusNameColumn);
                var practiceIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                var practiceNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeNameColumn);
                var buyerNameIndex = reader.GetOrdinal(Constants.ColumnNames.BuyerNameColumn);
                var createDateIndex = reader.GetOrdinal(Constants.ColumnNames.CreateDateColumn);
                var pipelineIndex = reader.GetOrdinal(Constants.ColumnNames.PipelineColumn);
                var proposedIndex = reader.GetOrdinal(Constants.ColumnNames.ProposedColumn);
                var sendOutIndex = reader.GetOrdinal(Constants.ColumnNames.SendOutColumn);
                var projectId = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                var opportunityIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityIndexColumn);
                var revenueTypeIndex = reader.GetOrdinal(Constants.ColumnNames.RevenueTypeColumn);
                var ownerIdIndex = reader.GetOrdinal(Constants.ColumnNames.OwnerIdColumn);
                var ownerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.OwnerFirstNameColumn);
                var ownerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.OwnerLastNameColumn);
                var ownerStatusIndex = reader.GetOrdinal(Constants.ColumnNames.OwnerStatusColumn);
                var groupIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn);
                var lastUpdateIndex = reader.GetOrdinal(Constants.ColumnNames.LastUpdateColumn);
                var groupNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn);
                var practManagerIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeManagerIdColumn);
                int EstimatedRevenueiIndex = -1;
                try
                {
                    EstimatedRevenueiIndex = reader.GetOrdinal(Constants.ColumnNames.EstimatedRevenueColumn);
                }
                catch
                {
                    EstimatedRevenueiIndex = -1;
                }

                while (reader.Read())
                {
                    // Reading the item
                    var opportunity =
                        new Opportunity
                            {
                                Id = reader.GetInt32(opportunityIdIndex),
                                Name = reader.GetString(nameIndex),
                                Description =
                                    !reader.IsDBNull(descriptionIndex) ? reader.GetString(descriptionIndex) : null,
                                OpportunityNumber = reader.GetString(opportunityNumberIndex),
                                ProjectedStartDate =
                                    !reader.IsDBNull(projectedStartDateIndex)
                                        ? (DateTime?)reader.GetDateTime(projectedStartDateIndex)
                                        : null,
                                ProjectedEndDate =
                                    !reader.IsDBNull(projectedEndDateIndex)
                                        ? (DateTime?)reader.GetDateTime(projectedEndDateIndex)
                                        : null,
                                BuyerName =
                                    !reader.IsDBNull(buyerNameIndex) ? reader.GetString(buyerNameIndex) : null,
                                CreateDate = reader.GetDateTime(createDateIndex),
                                Priority = reader.GetString(priorityIndex)[0],
                                Pipeline =
                                    !reader.IsDBNull(pipelineIndex) ? reader.GetString(pipelineIndex) : null,
                                Proposed =
                                    !reader.IsDBNull(proposedIndex) ? reader.GetString(proposedIndex) : null,
                                SendOut =
                                    !reader.IsDBNull(sendOutIndex) ? reader.GetString(sendOutIndex) : null,
                                Client = new Client
                                             {
                                                 Id = reader.GetInt32(clientIdIndex),
                                                 Name = reader.GetString(clientNameIndex)
                                             },
                                Status = new OpportunityStatus
                                             {
                                                 Id = reader.GetInt32(opportunityStatusIdIndex),
                                                 Name = reader.GetString(opportunityStatusNameIndex)
                                             },
                                Salesperson =
                                    !reader.IsDBNull(salespersonIdIndex)
                                        ? new Person
                                              {
                                                  Id = reader.GetInt32(salespersonIdIndex),
                                                  FirstName = reader.GetString(salespersonFirstNameIndex),
                                                  LastName = reader.GetString(salespersonLastNameIndex),
                                                  Status = !reader.IsDBNull(salespersonStatusIndex) ? new PersonStatus { Name = reader.GetString(salespersonStatusIndex) } : null
                                              }
                                        : null,
                                Practice =
                                    new Practice
                                        {
                                            Id = reader.GetInt32(practiceIdIndex),
                                            Name = reader.GetString(practiceNameIndex),
                                            PracticeOwner = new Person
                                                                {
                                                                    Id = reader.GetInt32(practManagerIdIndex)
                                                                }
                                        },
                                LastUpdate = reader.GetDateTime(lastUpdateIndex),
                                ProjectId =
                                    !reader.IsDBNull(projectId) ? (int?)reader.GetInt32(projectId) : null,
                                OpportunityIndex =
                                    !reader.IsDBNull(opportunityIndex) ? (int?)reader.GetInt32(opportunityIndex) : null,
                                OpportunityRevenueType = (RevenueType)reader.GetInt32(revenueTypeIndex),
                                Owner =
                                    !reader.IsDBNull(ownerIdIndex)
                                        ? new Person
                                        {
                                            Id = reader.GetInt32(ownerIdIndex),
                                            LastName = !reader.IsDBNull(ownerLastNameIndex) ? reader.GetString(ownerLastNameIndex) : null,
                                            FirstName = !reader.IsDBNull(ownerFirstNameIndex) ? reader.GetString(ownerFirstNameIndex) : null,
                                            Status = !reader.IsDBNull(ownerStatusIndex) ? new PersonStatus() { Name = reader.GetString(ownerStatusIndex) } : null
                                        }
                                        : null,
                                Group = !reader.IsDBNull(groupIdIndex)
                                            ? new ProjectGroup
                                                  {
                                                      Id = reader.GetInt32(groupIdIndex),
                                                      Name = reader.GetString(groupNameIndex)
                                                  }
                                            : null
                            };

                    if (EstimatedRevenueiIndex > -1)
                    {
                        if (!reader.IsDBNull(EstimatedRevenueiIndex))
                        {
                            opportunity.EstimatedRevenue = reader.GetDecimal(EstimatedRevenueiIndex);
                        }
                    }

                    result.Add(opportunity);

                }
            }
        }

        #endregion

        public static int? GetOpportunityId(string opportunityNumber)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Opportunitites.OpportunityGetByNumber, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ColumnNames.OpportunityNumberColumn, opportunityNumber);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            SqlInt32 opportunityId = reader.GetSqlInt32(0);
                            if (!opportunityId.IsNull)
                            {
                                return opportunityId.Value;
                            }
                        }
                    }

                    return null;
                }
            }
        }

        public static List<Person> GetOpportunityPersons(int opportunityId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Opportunitites.GetOpportunityPersons, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ColumnNames.OpportunityIdColumn, opportunityId);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        var result = new List<Person>();
                        ReadPersons(reader, result);
                        return result;
                    }
                }
            }
        }

        private static void ReadPersons(DbDataReader reader, List<Person> result)
        {
            if (reader.HasRows)
            {
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);

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
        }

        public static int ConvertOpportunityToProject(int opportunityId, string userName, bool hasPersons)
        {
            int res;
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Opportunitites.ConvertOpportunityToProject, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityIdParam, opportunityId);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam,
                                                !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.HasPersons, hasPersons);

                var idParam = new SqlParameter(Constants.ParameterNames.ProjectId, SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(idParam);

                try
                {

                    connection.Open();

                    SqlTransaction trn = connection.BeginTransaction();
                    command.Transaction = trn;

                    command.ExecuteNonQuery();

                    trn.Commit();

                    res = (int)idParam.Value;
                }
                catch (SqlException ex)
                {
                    throw new DataAccessException(ex);
                }
            }

            return res;
        }

        public static void OpportunityPersonInsert(int opportunityId, string personIdList)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Opportunitites.OpportunityPersonInsert, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityIdParam, opportunityId);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdListParam, personIdList);

                try
                {
                    connection.Open();

                    SqlTransaction trn = connection.BeginTransaction();
                    command.Transaction = trn;

                    command.ExecuteNonQuery();

                    trn.Commit();
                }
                catch (SqlException ex)
                {
                    throw new DataAccessException(ex);
                }
            }
        }
        public static void OpportunityPersonDelete(int opportunityId, string personIdList)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Opportunitites.OpportunityPersonDelete, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityIdParam, opportunityId);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonIdListParam, personIdList);

                try
                {
                    connection.Open();

                    SqlTransaction trn = connection.BeginTransaction();
                    command.Transaction = trn;

                    command.ExecuteNonQuery();

                    trn.Commit();
                }
                catch (SqlException ex)
                {
                    throw new DataAccessException(ex);
                }
            }
        }

    }
}


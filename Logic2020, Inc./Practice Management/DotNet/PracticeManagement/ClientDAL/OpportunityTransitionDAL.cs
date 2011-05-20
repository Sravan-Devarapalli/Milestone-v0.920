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
    /// 	Provides an access to the OpportunityTransition database table.
    /// </summary>
    public static class OpportunityTransitionDAL
    {
        #region Methods

        /// <summary>
        /// 	Retrieves transittions for the specified opportunity.
        /// </summary>
        /// <param name = "opportunityId">An ID of the opportunity to retrieve the transitions for.</param>
        /// <returns>A list of the <see cref = "OpportunityTransition" /> objects.</returns>
        public static List<OpportunityTransition> OpportunityTransitionGetByOpportunity(
            int opportunityId)
        {
            return OpportunityTransitionGetByOpportunity(opportunityId, null);
        }

        /// <summary>
        /// 	Retrieves transittions for the specified opportunity.
        /// </summary>
        /// <returns>A list of the <see cref = "OpportunityTransition" /> objects.</returns>
        public static void OpportunityTransitionDelete(int opportunityTransitionId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command =
                new SqlCommand(Constants.ProcedureNames.Opportunitites.OpportunityTransitionDelete, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityTransitionId, opportunityTransitionId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 	Retrieves transittions for the specified opportunity.
        /// </summary>
        /// <param name = "opportunityId">An ID of the opportunity to retrieve the transitions for.</param>
        /// <param name = "statusType"></param>
        /// <returns>A list of the <see cref = "OpportunityTransition" /> objects.</returns>
        public static List<OpportunityTransition> OpportunityTransitionGetByOpportunity(
            int opportunityId, OpportunityTransitionStatusType? statusType)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command =
                new SqlCommand(Constants.ProcedureNames.Opportunitites.OpportunityTransitionGetByOpportunity, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityIdParam, opportunityId);
                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityTransitionStatusId, 
                    statusType.HasValue ? statusType : null);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<OpportunityTransition>();
                    ReadOpportunityTransitions(reader, result);
                    return result;
                }
            }
        }

        /// <summary>
        /// 	Retrieves transitions for the specified opportunity.
        /// </summary>
        /// <returns>A list of the <see cref = "OpportunityTransition" /> objects.</returns>
        public static List<OpportunityTransition> OpportunityTransitionGetByPerson(int personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command =
                new SqlCommand(Constants.ProcedureNames.Opportunitites.OpportunityTransitionGetByPerson, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                    return ReadOpportunityTransitionsByPerson(reader);
            }
        }

        /// <summary>
        /// 	Inserts a new opportunity transition notes.
        /// </summary>
        /// <param name = "transition">The data to be inserted.</param>
        public static int OpportunityTransitionInsert(OpportunityTransition transition)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Opportunitites.OpportunityTransitionInsert, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityIdParam, transition.Opportunity.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityTransitionStatusId,
                                                transition.OpportunityTransitionStatus != null
                                                    ? (object) transition.OpportunityTransitionStatus.Id
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId,
                                                transition.Person != null && transition.Person.Id.HasValue
                                                    ? (object) transition.Person.Id.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.NoteText,
                                                !string.IsNullOrEmpty(transition.NoteText)
                                                    ? (object) transition.NoteText
                                                    : DBNull.Value);

                var oppTransitionIdPrm = new SqlParameter(Constants.ParameterNames.OpportunityTransitionId, SqlDbType.Int) 
                    { Direction = ParameterDirection.Output };
                command.Parameters.Add(oppTransitionIdPrm);

                if (transition.TargetPerson != null)
                    command.Parameters.AddWithValue(
                        Constants.ParameterNames.TargetPerson, 
                        transition.TargetPerson.Id);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new DataAccessException(ex);
                }

                return (int) oppTransitionIdPrm.Value;
            }
        }

        private static List<OpportunityTransition> ReadOpportunityTransitionsByPerson(DbDataReader reader)
        {
            var result = new List<OpportunityTransition>();

            if (reader.HasRows)
            {
                var opportunityTransitionIdIndex =
                    reader.GetOrdinal(Constants.ColumnNames.OpportunityTransitionId);
                var opportunityIdIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityIdColumn);
                var opportunityNameIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityName);
                var opportunityTransitionStatusIdIndex =
                    reader.GetOrdinal(Constants.ColumnNames.OpportunityTransitionStatusId);
                var opportunityTransitionStatusNameIndex =
                    reader.GetOrdinal(Constants.ColumnNames.OpportunityTransitionStatusName);
                var clientNameIndex =
                    reader.GetOrdinal(Constants.ColumnNames.ClientName);
                var opportunityPriorityIndex =
                    reader.GetOrdinal(Constants.ColumnNames.PriorityColumn);
                var opportunityNumberIndex =
                    reader.GetOrdinal(Constants.ColumnNames.OpportunityNumberColumn);
                var opportunityLastUpdatedIndex =
                    reader.GetOrdinal(Constants.ColumnNames.LastUpdateColumn);

                while (reader.Read())
                {
                    var transition = new OpportunityTransition
                                         {
                                             Id = reader.GetInt32(opportunityTransitionIdIndex),
                                             Opportunity = new Opportunity
                                                               {
                                                                   Id = reader.GetInt32(opportunityIdIndex),
                                                                   Priority = new OpportunityPriority{ Priority = reader.GetString(opportunityPriorityIndex)},
                                                                   Client = new Client { Name = reader.GetString(clientNameIndex) },
                                                                   OpportunityNumber = reader.GetString(opportunityNumberIndex),
                                                                   LastUpdate = reader.GetDateTime(opportunityLastUpdatedIndex),
                                                                   Name = reader.GetString(opportunityNameIndex)
                                                               },
                                             OpportunityTransitionStatus = new OpportunityTransitionStatus
                                                                               {
                                                                                   Id =
                                                                                       reader.GetInt32(
                                                                                           opportunityTransitionStatusIdIndex),
                                                                                   Name =
                                                                                       reader.GetString(
                                                                                           opportunityTransitionStatusNameIndex)
                                                                               },
                                         };

                    result.Add(transition);
                }
            }

            return result;
        }

        private static void ReadOpportunityTransitions(DbDataReader reader, List<OpportunityTransition> result)
        {
            if (reader.HasRows)
            {
                var opportunityTransitionIdIndex =
                    reader.GetOrdinal(Constants.ColumnNames.OpportunityTransitionId);
                var opportunityIdIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityIdColumn);
                var opportunityTransitionStatusIdIndex =
                    reader.GetOrdinal(Constants.ColumnNames.OpportunityTransitionStatusId);
                var transitionDateIndex = reader.GetOrdinal(Constants.ColumnNames.TransitionDate);
                var personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                var noteTextIndex = reader.GetOrdinal(Constants.ColumnNames.NoteText);
                var opportunityTransitionStatusNameIndex =
                    reader.GetOrdinal(Constants.ColumnNames.OpportunityTransitionStatusName);
                var firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                var lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);

                var targetId = reader.GetOrdinal(Constants.ColumnNames.TargetPersonId);
                var targetFirstName = reader.GetOrdinal(Constants.ColumnNames.TargetFirstName);
                var targetLastName = reader.GetOrdinal(Constants.ColumnNames.TargetLastName);

                while (reader.Read())
                {
                    var transition = new OpportunityTransition
                    {
                        Id = reader.GetInt32(opportunityTransitionIdIndex),
                        Opportunity = new Opportunity { Id = reader.GetInt32(opportunityIdIndex) },
                        TransitionDate = reader.GetDateTime(transitionDateIndex),
                        NoteText =
                            !reader.IsDBNull(noteTextIndex)
                                ? reader.GetString(noteTextIndex)
                                : null,
                        OpportunityTransitionStatus = new OpportunityTransitionStatus
                        {
                            Id =
                                reader.GetInt32(
                                    opportunityTransitionStatusIdIndex),
                            Name =
                                reader.GetString(
                                    opportunityTransitionStatusNameIndex)
                        },
                        Person = new Person
                        {
                            Id = reader.GetInt32(personIdIndex),
                            FirstName = reader.GetString(firstNameIndex),
                            LastName = reader.GetString(lastNameIndex)
                        }
                    };

                    if (!reader.IsDBNull(targetId))
                    {
                        var target = new Person
                        {
                            Id = reader.GetInt32(targetId),
                            FirstName = reader.GetString(targetFirstName),
                            LastName = reader.GetString(targetLastName)
                        };

                        transition.TargetPerson = target;
                    }

                    result.Add(transition);
                }
            }
        }

        #endregion
    }
}

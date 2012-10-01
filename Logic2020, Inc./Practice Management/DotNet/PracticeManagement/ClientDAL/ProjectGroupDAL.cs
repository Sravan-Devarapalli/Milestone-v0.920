using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;
using System.Linq;

namespace DataAccess
{
    /// <summary>
    /// Encapsulates stored procedures in the data store
    /// </summary>
    public static class ProjectGroupDAL
    {
        #region Constants

        #region Parameters

        private const string GroupIdParam = "@GroupId";
        private const string ClientIdParam = "@ClientId";
        private const string CommaSeperatedClientListParam = "@CommaSeperatedClientList";
        private const string ProjectIdParam = "@ProjectId";
        private const string NameParam = "@Name";
        private const string isActiveParam = "@IsActive";
        private const string OldGroupNameParam = "@OldGroupName";
        private const string GroupNameParam = "@GroupName";

        #endregion

        #region Columns

        private const string GroupIdColumn = "GroupId";
        private const string IsActiveColumn = "Active";
        private const string NameColumn = "Name";
        private const string InUseColumn = "InUse";
        private const string CodeColumn = "Code";

        #endregion

        #endregion

        /// <summary>
        /// List all active clients in the system
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of active <see cref="Client"/>s in the system</returns>
        /// <remarks>
        /// The list of clients will probably not be large.  Locality of Reference suggests
        /// that if we want one client we are going to want another.  Might as well get them
        /// all at once.  A client can find a particular client by scanning the list.
        /// 
        /// If it should prove the case that the client list is large then a <see cref="List{T}"/>
        /// is not the best structure to support finding a specific <see cref="Client"/>.
        /// </remarks>
        public static List<ProjectGroup> GroupListAll(int? clientId, int? projectId)
        {
            return GroupListAll(clientId, projectId, null);
        }

        /// <summary>
        /// List all active clients in the system
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of active <see cref="Client"/>s in the system</returns>
        /// <remarks>
        /// The list of clients will probably not be large.  Locality of Reference suggests
        /// that if we want one client we are going to want another.  Might as well get them
        /// all at once.  A client can find a particular client by scanning the list.
        /// 
        /// If it should prove the case that the client list is large then a <see cref="List{T}"/>
        /// is not the best structure to support finding a specific <see cref="Client"/>.
        /// </remarks>
        public static List<ProjectGroup> GroupListAll(int? clientId, int? projectId, int? personId)
        {
            List<ProjectGroup> groupList = new List<ProjectGroup>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.ProjectGroup.ProjectGroupListAll, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(ClientIdParam,
                        clientId.HasValue ? (object)clientId.Value : DBNull.Value);
                    command.Parameters.AddWithValue(ProjectIdParam,
                        projectId.HasValue ? (object)projectId.Value : DBNull.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId,
                        personId.HasValue ? (object)personId.Value : DBNull.Value);
                    connection.Open();
                    ReadGroups(command, groupList);
                }
            }
            return groupList;
        }

        public static List<ProjectGroup> ListGroupByClientAndPersonInPeriod(int clientId, int personId, DateTime startDate, DateTime endDate)
        {
            List<ProjectGroup> groupList = new List<ProjectGroup>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.ProjectGroup.ListGroupByClientAndPersonInPeriod, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(ClientIdParam, clientId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            int groupIdIndex = reader.GetOrdinal(GroupIdColumn);
                            int nameIndex = reader.GetOrdinal(NameColumn);

                            while (reader.Read())
                            {
                                ProjectGroup projectGroup = new ProjectGroup
                                 {
                                     Id = (int)reader.GetInt32(groupIdIndex),
                                     Name = (string)reader.GetString(nameIndex)
                                 };
                                groupList.Add(projectGroup);
                            }
                        }
                    }
                }
            }
            return groupList;
        }


        public static Dictionary<int, List<ProjectGroup>> ClientGroupListAll(string commaSeperatedClientList, int? projectId, int? personId)
        {
            Dictionary<int, List<ProjectGroup>> clientGroups = new Dictionary<int, List<ProjectGroup>>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.ProjectGroup.GetClientsGroups, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(CommaSeperatedClientListParam, commaSeperatedClientList);
                    command.Parameters.AddWithValue(ProjectIdParam,
                        projectId.HasValue ? (object)projectId.Value : DBNull.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId,
                        personId.HasValue ? (object)personId.Value : DBNull.Value);
                    connection.Open();
                    ReadGroupWithClientId(command, clientGroups);
                }
            }
            return clientGroups;
        }

        private static void ReadGroupWithClientId(SqlCommand command, Dictionary<int, List<ProjectGroup>> clientGroups)
        {
            using (var reader = command.ExecuteReader())
            {
                if (reader != null)
                    while (reader.Read())
                    {
                        var clientId = (int)reader[Constants.ColumnNames.ClientIdColumn];
                        var pg = new ProjectGroup
                         {
                             Id = (int)reader[GroupIdColumn],
                             Name = (string)reader[NameColumn],
                             IsActive = (bool)reader[IsActiveColumn],
                             InUse = (int)reader[InUseColumn] == 1
                         };

                        if (clientGroups.Keys.Count > 0 && clientGroups.Keys.Any(c => clientId == c))
                        {
                            clientGroups[clientId].Add(pg);
                        }
                        else
                        {
                            clientGroups.Add(clientId, new List<ProjectGroup>() { pg });
                        }

                    }
            }

        }



        public static bool UpDateProductGroup(int clientId, int groupId, string groupName, bool isActive)
        {
            bool result = false;
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.ProjectGroup.ProjectGroupRenameByClient, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(ClientIdParam, clientId);
                    command.Parameters.AddWithValue(GroupIdParam, groupId);
                    command.Parameters.AddWithValue(GroupNameParam, groupName);
                    command.Parameters.AddWithValue(isActiveParam, isActive);
                    connection.Open();
                    result = command.ExecuteScalar().ToString() == "0";
                }
            }
            return result;
        }

        public static int ProjectGroupInsert(int clientId, string groupName, bool isActive)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.ProjectGroup.ProjectGroupInsert, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(GroupIdParam, DbType.Int32).Direction = ParameterDirection.Output;
                    command.Parameters.AddWithValue(ClientIdParam, clientId);
                    command.Parameters.AddWithValue(NameParam, groupName);
                    command.Parameters.AddWithValue(isActiveParam, isActive);
                    connection.Open();
                    command.ExecuteNonQuery();
                    return (int)command.Parameters[GroupIdParam].Value;
                }
            }
        }

        public static bool ProjectGroupDelete(int groupId)
        {
            bool result = false;
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.ProjectGroup.ProjectGroupDelete, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(GroupIdParam, groupId);
                    connection.Open();
                    result = command.ExecuteScalar().ToString() == "0";
                }
            }
            return result;
        }

        private static void ReadGroups(SqlCommand command, ICollection<ProjectGroup> groupList)
        {
            using (var reader = command.ExecuteReader())
            {
                if (reader != null)
                    while (reader.Read())
                    {
                        groupList.Add(ReadGroup(reader));
                    }
            }
        }

        private static ProjectGroup ReadGroup(SqlDataReader reader)
        {
            return ReadGroup(reader, NameColumn);
        }

        public static ProjectGroup ReadGroup(SqlDataReader reader, string groupNameColumn)
        {
            return new ProjectGroup
                       {
                           Id = (int)reader[GroupIdColumn],
                           Name = (string)reader[groupNameColumn],
                           IsActive = (bool)reader[IsActiveColumn],
                           InUse = (int)reader[InUseColumn] == 1,
                           Code = (string)reader[CodeColumn]
                       };
        }

        public static List<ProjectGroup> GetInternalBusinessUnits()
        {
            List<ProjectGroup> groupList = new List<ProjectGroup>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.ProjectGroup.GetInternalBusinessUnits, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                 
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            int groupIdIndex = reader.GetOrdinal(GroupIdColumn);
                            int nameIndex = reader.GetOrdinal(NameColumn);
                            int codeIndex = reader.GetOrdinal(CodeColumn);

                            while (reader.Read())
                            {
                                ProjectGroup projectGroup = new ProjectGroup
                                 {
                                     Id = (int)reader.GetInt32(groupIdIndex),
                                     Name = (string)reader.GetString(nameIndex),
                                     Code = (string)reader.GetString(codeIndex)
                                 };
                                groupList.Add(projectGroup);
                            }
                        }
                    }
                }
            }
            return groupList;
        
        }
    }
}


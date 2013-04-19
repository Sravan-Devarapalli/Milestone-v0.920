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

        #region BusinessUnit Methods

        public static List<ProjectGroup> GroupListAll(int? clientId, int? projectId)
        {
            return GroupListAll(clientId, projectId, null);
        }

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

        public static bool ProjectGroupUpdate(ProjectGroup projectGroup, string userLogin)
        {
            bool result = false;
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.ProjectGroup.ProjectGroupUpdate, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(ClientIdParam, projectGroup.ClientId);
                    command.Parameters.AddWithValue(GroupIdParam, projectGroup.Id);
                    command.Parameters.AddWithValue(GroupNameParam, projectGroup.Name);
                    command.Parameters.AddWithValue(isActiveParam, projectGroup.IsActive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                    command.Parameters.AddWithValue(Constants.ParameterNames.BusinessGroupIdParam, projectGroup.BusinessGroupId);
                    connection.Open();
                    result = command.ExecuteScalar().ToString() == "0";
                }
            }
            return result;
        }

        public static int ProjectGroupInsert(ProjectGroup projectGroup, string userLogin)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.ProjectGroup.ProjectGroupInsert, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(GroupIdParam, DbType.Int32).Direction = ParameterDirection.Output;
                    command.Parameters.AddWithValue(ClientIdParam, projectGroup.ClientId);
                    command.Parameters.AddWithValue(NameParam, projectGroup.Name);
                    command.Parameters.AddWithValue(isActiveParam, projectGroup.IsActive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                    command.Parameters.AddWithValue(Constants.ParameterNames.BusinessGroupIdParam, projectGroup.BusinessGroupId);
                    connection.Open();
                    command.ExecuteNonQuery();
                    return (int)command.Parameters[GroupIdParam].Value;
                }
            }
        }

        public static bool ProjectGroupDelete(int groupId, string userLogin)
        {
            bool result = false;
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.ProjectGroup.ProjectGroupDelete, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(GroupIdParam, groupId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
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
                           Code = (string)reader[CodeColumn],
                           BusinessGroupId = (int)reader[Constants.ColumnNames.BusinessGroupIdColumn]
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

        #endregion

        #region Business Group Methods

        public static void BusinessGroupUpdate(BusinessGroup businessGroup,string userLogin)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.ProjectGroup.BusinessGroupUpdate, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.BusinessGroupIdParam, businessGroup.Id);
                    command.Parameters.AddWithValue(Constants.ParameterNames.Name, businessGroup.Name);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsActive, businessGroup.IsActive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static int BusinessGroupInsert(BusinessGroup businessGroup, string userLogin)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.ProjectGroup.BusinessGroupInsert, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.BusinessGroupIdParam, DbType.Int32).Direction = ParameterDirection.Output;
                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdParam, businessGroup.ClientId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.Name, businessGroup.Name);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsActive, businessGroup.IsActive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                    connection.Open();
                    command.ExecuteNonQuery();
                    return (int)command.Parameters[Constants.ParameterNames.BusinessGroupIdParam].Value;
                }
            }
        }

        public static void BusinessGroupDelete(int businessGroupId, string userLogin)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.ProjectGroup.BusinessGroupDelete, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.BusinessGroupIdParam, businessGroupId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<BusinessGroup> GetBusinessGroupList(int? clientId, int? businessUnitId)
        {
            List<BusinessGroup> BusinessGroupList = new List<BusinessGroup>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.ProjectGroup.GetBusinessGroupList, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdParam,
                        clientId.HasValue ? (object)clientId.Value : DBNull.Value);

                    command.Parameters.AddWithValue(Constants.ParameterNames.BusinessUnitIdParam,
                        businessUnitId.HasValue ? (object)businessUnitId.Value : DBNull.Value);
                    
                    connection.Open();
                    ReadBusinessGroups(command, BusinessGroupList);
                }
            }
            return BusinessGroupList;
        }

        private static void ReadBusinessGroups(SqlCommand command, ICollection<BusinessGroup> businessGroupList)
        {
            using (var reader = command.ExecuteReader())
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        businessGroupList.Add(ReadBusinessGroup(reader));
                    }
                }
            }
        }

        private static BusinessGroup ReadBusinessGroup(SqlDataReader reader)
        {
            return new BusinessGroup
            {
                Id = (int)reader[Constants.ColumnNames.BusinessGroupIdColumn],
                Name = (string)reader[Constants.ColumnNames.Name],
                IsActive = (bool)reader[Constants.ColumnNames.Active],
                InUse = (bool)reader[Constants.ColumnNames.InUse],
                Code = (string)reader[Constants.ColumnNames.CodeColumn],
                ClientId = (int)reader[Constants.ColumnNames.ClientId]
            };
        }

        #endregion
    }
}


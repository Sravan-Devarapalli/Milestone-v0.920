﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
    /// <summary>
    /// 	Encapsulates stored procedures in the data store
    /// </summary>
    public static class ClientDAL
    {
        #region Constants

        #region Stored Procedures

        private const string ClientInsertProcedure = "dbo.ClientInsert";
        private const string ClientUpdateProcedure = "dbo.ClientUpdate";
        private const string ClientListAllProcedure = "dbo.ClientListAll";
        private const string ClientGetByIdProcedure = "dbo.ClientGetById";
        private const string ClientListAllForProjectProcedure = "dbo.ClientListAllForProject";
        private const string UpdateIsChargableForClientProcedure = "dbo.UpdateIsChargableForClient";
        private const string ColorsListAllProcedure = "dbo.ColorsListAll";
        private const string GetClientMarginColorInfoProcedure = "dbo.GetClientMarginColorInfo";
        private const string ClientMarginColorInfoInsertProcedure = "dbo.ClientMarginColorInfoInsert";
        private const string ClientListAllWithoutPermissionsProcedure = "dbo.ClientListAllWithoutPermissions";
        private const string GetInternalAccountProcedure = "dbo.GetInternalAccount";

        #endregion

        #region Parameters

        private const string ClientIdParam = "@ClientId";
        private const string NameParam = "@Name";
        private const string DefaultTermsParam = "@DefaultTerms";
        private const string DefaultSalespersonIdParam = "@DefaultSalespersonId";
        private const string DefaultDirectorIdParam = "@DefaultDirectorId";
        private const string DefaultDiscountParam = "@DefaultDiscount";
        private const string InactiveParam = "@Inactive";
        private const string ShowAllParam = "@ShowAll";
        private const string ProjectIdParam = "@ProjectId";
        private const string PersonIdParam = "@PersonId";
        private const string IsChargeableParam = "@IsChargeable";
        private const string ColorIdParam = "@ColorId";
        private const string StartRangeParam = "@StartRange";
        private const string EndRangeParam = "@EndRange";
        private const string isDeletePreviousMarginInfoParam = "@isDeletePreviousMarginInfo";
        private const string ApplyNewRuleParam = "@ApplyNewRule";
        private const string IsInternalParam = "@IsInternal";

        #endregion

        #region Columns

        private const string ClientIdColumn = "ClientId";
        private const string NameColumn = "Name";
        private const string DefaultDiscountColumn = "DefaultDiscount";
        private const string DefaultTermsColumn = "DefaultTerms";
        private const string DefaultSalespersonIdColumn = "DefaultSalespersonId";
        private const string DefaultDirectorIdColumn = "DefaultDirectorId";
        private const string InactiveColumn = "Inactive";
        private const string IsMarginColorInfoEnabledColumn = "IsMarginColorInfoEnabled";

        #endregion

        #endregion

        /// <summary>
        /// 	Insert client information to the system.
        /// </summary>
        /// <param name = "client">Client with information to add to the system</param>
        /// <remarks>
        /// 	At exit the
        /// 	<paramref name = "client" />
        /// 	ClientId will contain the systems generated ID
        /// </remarks>
        public static void ClientInsert(Client client)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(ClientInsertProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(NameParam, client.Name);
                    command.Parameters.AddWithValue(DefaultTermsParam, client.DefaultTerms);
                    command.Parameters.AddWithValue(DefaultDiscountParam, client.DefaultDiscount);
                    command.Parameters.AddWithValue(InactiveParam, client.Inactive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsChargeable, client.IsChargeable);
                    command.Parameters.AddWithValue(DefaultSalespersonIdParam, client.DefaultSalespersonId);
                    command.Parameters.AddWithValue(DefaultDirectorIdParam,
                        client.DefaultDirectorId.HasValue ? (object)client.DefaultDirectorId.Value : DBNull.Value);

                    var clientIdParameter = new SqlParameter(ClientIdParam, SqlDbType.Int) { Direction = ParameterDirection.Output };
                    command.Parameters.Add(clientIdParameter);

                    command.Parameters.AddWithValue(Constants.ParameterNames.IsMarginColorInfoEnabled, client.IsMarginColorInfoEnabled);
                    command.Parameters.AddWithValue(IsInternalParam, client.IsInternal);


                    try
                    {
                        connection.Open();

                        SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                        command.Transaction = transaction;

                        command.ExecuteNonQuery();
                        client.Id = (int)clientIdParameter.Value;

                        if (client.IsMarginColorInfoEnabled.HasValue && client.IsMarginColorInfoEnabled.Value && client.ClientMarginInfo != null)
                        {
                            for (int i = 0; i < client.ClientMarginInfo.Count; i++)
                            {
                                bool isDeletePreviousMarginInfo = false;
                                if (i == 0)
                                {
                                    isDeletePreviousMarginInfo = true;
                                }
                                else
                                {
                                    isDeletePreviousMarginInfo = false;
                                }
                                ClientMarginColorInfoInsert(client.Id, client.ClientMarginInfo[i], isDeletePreviousMarginInfo, connection, transaction);
                            }  
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        private static void ClientMarginColorInfoInsert(int? clientId, ClientMarginColorInfo marginColorInfo, bool isDeletePreviousMarginInfo, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

            using (var command = new SqlCommand(ClientMarginColorInfoInsertProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ClientIdParam, clientId.Value);
                command.Parameters.AddWithValue(ColorIdParam, marginColorInfo.ColorInfo.ColorId);
                command.Parameters.AddWithValue(StartRangeParam, marginColorInfo.StartRange);
                command.Parameters.AddWithValue(EndRangeParam, marginColorInfo.EndRange);
                command.Parameters.AddWithValue(isDeletePreviousMarginInfoParam, isDeletePreviousMarginInfo);

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
        /// 	Updates client information to the system.
        /// </summary>
        /// <param name = "client">Client with information to update to the system</param>
        /// <remarks>
        /// 	At exit the
        /// 	<paramref name = "client" />
        /// 	ClientId will contain the systems generated ID
        /// </remarks>
        public static void ClientUpdate(Client client)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(ClientUpdateProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(ClientIdParam, client.Id);
                    command.Parameters.AddWithValue(NameParam, client.Name);
                    command.Parameters.AddWithValue(DefaultDiscountParam, client.DefaultDiscount);
                    command.Parameters.AddWithValue(DefaultTermsParam, client.DefaultTerms);
                    command.Parameters.AddWithValue(DefaultSalespersonIdParam, client.DefaultSalespersonId);
                    command.Parameters.AddWithValue(DefaultDirectorIdParam,
                        client.DefaultDirectorId.HasValue ? (object)client.DefaultDirectorId.Value : DBNull.Value);
                    command.Parameters.AddWithValue(InactiveParam, client.Inactive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsChargeable, client.IsChargeable);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsMarginColorInfoEnabled, client.IsMarginColorInfoEnabled);
                    command.Parameters.AddWithValue(IsInternalParam, client.IsInternal);
                    try
                    {
                        connection.Open();

                        SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                        command.Transaction = transaction;

                        command.ExecuteNonQuery();

                        if (client.IsMarginColorInfoEnabled.HasValue && client.IsMarginColorInfoEnabled.Value && client.ClientMarginInfo != null)
                        {
                            for (int i=0;i<client.ClientMarginInfo.Count;i++)
                            {
                                bool isDeletePreviousMarginInfo = false;
                                if (i == 0)
                                {
                                    isDeletePreviousMarginInfo = true;
                                }
                                else
                                {
                                    isDeletePreviousMarginInfo = false;
                                }
                                ClientMarginColorInfoInsert(client.Id, client.ClientMarginInfo[i], isDeletePreviousMarginInfo, connection, transaction);
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// 	List all active clients in the system
        /// </summary>
        /// <returns>A
        /// 	<see cref = "List{T}" />
        /// 	of active
        /// 	<see cref = "Client" />
        /// 	s in the system</returns>
        /// <remarks>
        /// 	The list of clients will probably not be large.  Locality of Reference suggests
        /// 	that if we want one client we are going to want another.  Might as well get them
        /// 	all at once.  A client can find a particular client by scanning the list.
        /// 
        /// 	If it should prove the case that the client list is large then a
        /// 	<see cref = "List{T}" />
        /// 	is not the best structure to support finding a specific
        /// 	<see cref = "Client" />
        /// 	.
        /// </remarks>
        public static List<Client> ClientListAll()
        {
            var clientList = new List<Client>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(ClientListAllProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    connection.Open();
                    ReadClients(command, clientList, false);
                }
            }
            return clientList;
        }

        /// <summary>
        /// 	Retrieves a person record from the database.
        /// </summary>
        /// <param name = "clientId"></param>
        /// <param name="viewerUsername"></param>
        /// <returns></returns>
        public static Client GetById(int clientId, string viewerUsername)
        {
            var clientList = new List<Client>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(ClientGetByIdProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientId, clientId);

                    connection.Open();

                    ReadClients(command, clientList, true);
                }
            }

            return clientList.Count > 0 ? clientList[0] : null;
        }

        /// <summary>
        /// 	List all active and inactive clients in the system
        /// </summary>
        /// <returns>A
        /// 	<see cref = "List{T}" />
        /// 	of
        /// 	<see cref = "Client" />
        /// 	s in the system</returns>
        /// <remarks>
        /// 	This is to help reactivate a client, or to know the total activity in the client
        /// 	entity
        /// </remarks>
        public static List<Client> ClientListAllWithInactive()
        {
            var clientList = new List<Client>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(ClientListAllProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(ShowAllParam, 1);

                    connection.Open();
                    ReadClients(command, clientList, false);
                }
            }
            return clientList;
        }

        /// <summary>
        /// 	List all active and inactive clients in the system
        /// </summary>
        /// <param name = "person">Person to restrict results to</param>
        /// <param name = "inactives">Include inactive items</param>
        /// <returns>A
        /// 	<see cref = "List{T}" />
        /// 	of
        /// 	<see cref = "Client" />
        /// 	s in the system</returns>
        public static List<Client> ClientListAllSecure(Person person, bool inactives, bool applyNewRule = false)
        {
            var clientList = new List<Client>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(ClientListAllProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    if (inactives)
                        command.Parameters.AddWithValue(ShowAllParam, 1);

                    if (person != null)
                        command.Parameters.AddWithValue(PersonIdParam, person.Id);
                    else
                        command.Parameters.AddWithValue(PersonIdParam, DBNull.Value);
                    
                    if (applyNewRule == true)
                        command.Parameters.AddWithValue(ApplyNewRuleParam, 1);

                    connection.Open();
                    ReadClients(command, clientList, true, person);
                }
            }
            return clientList;
        }

        /// <summary>
        /// 	Retrives the list clients available for the specific project.
        /// </summary>
        /// <param name = "projectId">An ID of the project to retrive the data for.</param>
        /// <returns>The list of the
        /// 	<see cref = "Client" />
        /// 	objects.</returns>
        public static List<Client> ClientListAllForProject(int? projectId, int? loggedInPersonId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(ClientListAllForProjectProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(ProjectIdParam,
                                                projectId.HasValue ? (object)projectId.Value : DBNull.Value);
                command.Parameters.AddWithValue(PersonIdParam, loggedInPersonId);

                connection.Open();
                var result = new List<Client>();
                ReadClients(command, result, false);

                return result;
            }
        }

        private static void ReadClients(SqlCommand command, List<Client> result)
        {
            ReadClients(command, result, true);
        }

        public static void ClientInactivate(Client client)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand("ClientInactivate", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(ClientIdParam, client.Id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void ClientReactivate(Client client)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand("ClientReactivate", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(ClientIdParam, client.Id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void ReadClients(
            SqlCommand command,
            ICollection<Client> clientList,
            bool loadGroups)
        {
            ReadClients(command, clientList, loadGroups, null);
        }

        private static void ReadClients(
            SqlCommand command,
            ICollection<Client> clientList,
            bool loadGroups,
            Person person)
        {
            using (var reader = command.ExecuteReader())
            {
                int isMarginColorInfoEnabledIndex = -1;
                try
                {
                    isMarginColorInfoEnabledIndex = reader.GetOrdinal(Constants.ColumnNames.IsMarginColorInfoEnabledColumn);
                }
                catch
                {
                    isMarginColorInfoEnabledIndex = -1;
                }

                int isInternalIndex = -1;
                try
                {
                    isInternalIndex = reader.GetOrdinal(Constants.ColumnNames.IsInternalColumn);
                }
                catch
                {
                    isInternalIndex = -1;
                }

                while (reader.Read())
                {
                    var client = ReadClientBasic(reader);
                    client.DefaultDiscount = (decimal)reader[DefaultDiscountColumn];
                    client.DefaultTerms = (int)reader[DefaultTermsColumn];
                    client.DefaultSalespersonId = (int)reader[DefaultSalespersonIdColumn];
                    if (reader[DefaultDirectorIdColumn] != System.DBNull.Value)
                        client.DefaultDirectorId = (int)reader[DefaultDirectorIdColumn];
                    client.Inactive = (bool)reader[InactiveColumn];
                    client.IsChargeable = (bool)reader[Constants.ColumnNames.IsChargeable];
                    if (isInternalIndex > -1)
                    {
                        client.IsInternal = reader.GetBoolean(isInternalIndex);
                    }
                    if (isMarginColorInfoEnabledIndex >= 0)
                    {
                        try
                        {
                            if (!reader.IsDBNull(isMarginColorInfoEnabledIndex))
                            {
                                client.IsMarginColorInfoEnabled = reader.GetBoolean(isMarginColorInfoEnabledIndex);
                            }
                            else
                            {
                                client.IsMarginColorInfoEnabled = null;
                            }
                        }
                        catch
                        {
 
                        }
                    }

                    if (loadGroups)
                        client.Groups = ProjectGroupDAL.GroupListAll(client.Id, null, person == null ? null : person.Id);

                    clientList.Add(client);
                }
            }
        }

        private static Client ReadClientBasic(SqlDataReader reader)
        {
            return ReadClientBasic(reader, NameColumn);
        }

        private static Client ReadClientBasic(SqlDataReader reader, string clientNameColumn)
        {
            return new Client { Id = (int)reader[ClientIdColumn], Name = (string)reader[clientNameColumn] };
        }

        public static void UpdateIsChargableForClient(int? clientId, bool isChargable)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(UpdateIsChargableForClientProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(ClientIdParam, clientId.Value);
                    command.Parameters.AddWithValue(IsChargeableParam, isChargable);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<ColorInformation> GetAllColorsForMargin()
        {
            var colors = new List<ColorInformation>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(ColorsListAllProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        ReadColor(reader, colors);
                        return colors;
                    }
                }
            }
        }

        private static void ReadColor(SqlDataReader reader, List<ColorInformation> result)
        {
            if (reader.HasRows)
            {
                int colorIdIndex = reader.GetOrdinal(Constants.ColumnNames.Id);
                int colorValueIndex = reader.GetOrdinal(Constants.ColumnNames.ValueColumn);
                int colorDescriptionIndex = reader.GetOrdinal(Constants.ColumnNames.DescriptionColumn);

                while (reader.Read())
                {
                    result.Add(
                        new ColorInformation()
                        {
                            ColorId = reader.GetInt32(colorIdIndex),
                            ColorValue = reader.GetString(colorValueIndex),
                            ColorDescription = reader.GetString(colorDescriptionIndex)
                        }

                                                );
                }
            }
        }

        public static List<ClientMarginColorInfo> GetClientMarginColorInfo(int clientId)
        {
            var clientMarginColorInfo = new List<ClientMarginColorInfo>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(GetClientMarginColorInfoProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientId, clientId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        ReadClientMarginColorInfo(reader, clientMarginColorInfo);
                    }
                }
            }
            if (clientMarginColorInfo.Count > 0)
                return clientMarginColorInfo;
            else
                return null;
        }

        private static void ReadClientMarginColorInfo(SqlDataReader reader, List<ClientMarginColorInfo> clientMarginColorInfo)
        {
            if (reader.HasRows)
            {
                int colorIdIndex = reader.GetOrdinal(Constants.ColumnNames.ColorIdColumn);
                int colorValueIndex = reader.GetOrdinal(Constants.ColumnNames.ValueColumn);
                int startRangeIndex = reader.GetOrdinal(Constants.ColumnNames.StartRangeColumn);
                int endRangeIndex = reader.GetOrdinal(Constants.ColumnNames.EndRangeColumn);
                int colorDescriptionIndex = reader.GetOrdinal(Constants.ColumnNames.DescriptionColumn);

                while (reader.Read())
                {
                    clientMarginColorInfo.Add(
                                                new ClientMarginColorInfo()
                                                {
                                                    ColorInfo = new ColorInformation()
                                                    {
                                                        ColorId=reader.GetInt32(colorIdIndex),
                                                        ColorValue = reader.GetString(colorValueIndex),
                                                        ColorDescription = reader.GetString(colorDescriptionIndex)
                                                    },
                                                    StartRange = reader.GetInt32(startRangeIndex),
                                                    EndRange = reader.GetInt32(endRangeIndex),
                                                }
                        );
                }
            }
        }

        public static List<Client> ClientListAllWithoutPermissions()
        {
            var clientList = new List<Client>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(ClientListAllWithoutPermissionsProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                   
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var client = ReadClientBasic(reader);
                            client.Inactive = (bool)reader[InactiveColumn];
                            clientList.Add(client);
                        }
                    }
                }
            }
            return clientList;
        }

        public static Client GetInternalAccount()
        {
            Client client = new Client();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(GetInternalAccountProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           client = ReadClientBasic(reader);
                        }
                    }
                }
            }
            return client;
        }


    }
}


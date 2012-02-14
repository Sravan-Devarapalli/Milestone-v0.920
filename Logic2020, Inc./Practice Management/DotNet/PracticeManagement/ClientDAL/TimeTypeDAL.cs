using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using DataAccess.Other;
using DataTransferObjects;
using DataTransferObjects.CompositeObjects;
using DataTransferObjects.ContextObjects;
using DataTransferObjects.TimeEntry;

namespace DataAccess
{
    public static class TimeTypeDAL
    {

        /// <summary>
        /// Retrieves all existing time types
        /// </summary>
        /// <returns>Collection of new time types</returns>
        public static IEnumerable<TimeTypeRecord> GetAllTimeTypes()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeType.GetAll, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return new List<TimeTypeRecord>(ReadTimeTypes(reader));
                }
            }
        }

        public static List<TimeTypeRecord> GetAllAdministrativeTimeTypes()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeType.GetAllAdministrativeTimeTypes, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<TimeTypeRecord>();
                    ReadTimeTypesShort(reader, result);
                    return result;
                }
            }
        }


        public static Triple<int, int, int> GetAdministrativeChargeCodeValues(int timeTypeId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeType.GetAdministrativeChargeCodeValues, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeId, timeTypeId);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    int clientId = -1, projectId = -1, businessUnitId = -1;
                   
                    if (reader.HasRows)
                    {
                        int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
                        int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                        int businessUnitIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn);

                        while (reader.Read())
                        {
                            clientId = reader.GetInt32(clientIdIndex);
                            projectId = reader.GetInt32(projectIdIndex);
                            businessUnitId = reader.GetInt32(businessUnitIdIndex);
                        }

                    }

                    return new Triple<int, int, int>(clientId, projectId, businessUnitId);
                }
            }
        }


        /// <summary>
        /// Updates given time type
        /// </summary>
        /// <param name="timeType">Time type to update</param>
        public static void UpdateTimeType(TimeTypeRecord timeType)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeType.Update, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeId, timeType.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.Name, timeType.Name);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsDefault, timeType.IsDefault);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsInternalParam, timeType.IsInternal);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsActive, timeType.IsActive);
                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Adds new time type
        /// </summary>
        /// <param name="timeType">Time type to add</param>
        /// <returns>Id of added time type</returns>
        public static int AddTimeType(TimeTypeRecord timeType)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeType.Insert, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.Name, timeType.Name);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsDefault, timeType.IsDefault);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsInternalParam, timeType.IsInternal);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsActive, timeType.IsActive);

                SqlParameter timeTypeIdParam = new SqlParameter(Constants.ParameterNames.TimeTypeId, SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(timeTypeIdParam);

                connection.Open();

                command.ExecuteNonQuery();
                timeType.Id = (int)timeTypeIdParam.Value;
                return timeType.Id;
            }
        }

        /// <summary>
        /// Removes given time type
        /// </summary>
        /// <param name="timeType">Time type to remove</param>
        public static void RemoveTimeType(TimeTypeRecord timeType)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeType.Delete, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeId, timeType.Id);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        private static IEnumerable<TimeTypeRecord> ReadTimeTypes(DbDataReader reader)
        {
            if (reader.HasRows)
            {
                int timeTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeId);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
                int inUseIndex = reader.GetOrdinal(Constants.ColumnNames.InUse);
                int inFutureUseIndex = reader.GetOrdinal(Constants.ColumnNames.InFutureUse);
                int isDefaultIndex = reader.GetOrdinal(Constants.ColumnNames.IsDefault);
                int isAllowedToEditColumnIndex = reader.GetOrdinal(Constants.ColumnNames.IsAllowedToEditColumn);
                int isActiveColumnIndex = reader.GetOrdinal(Constants.ColumnNames.IsActive);
                int isInternalColumnIndex = reader.GetOrdinal(Constants.ColumnNames.IsInternalColumn);

                while (reader.Read())
                {
                    var tt = new TimeTypeRecord
                    {
                        Id = reader.GetInt32(timeTypeIdIndex),
                        Name = reader.GetString(nameIndex),
                        IsDefault = reader.GetBoolean(isDefaultIndex),
                        IsAllowedToEdit = reader.GetBoolean(isAllowedToEditColumnIndex),
                        IsActive = reader.GetBoolean(isActiveColumnIndex),
                        IsInternal = reader.GetBoolean(isInternalColumnIndex),
                        InFutureUse = Convert.ToBoolean(reader.GetInt32(inFutureUseIndex)),
                        InUse = bool.Parse(reader.GetString(inUseIndex))
                    };
                    yield return tt;

                }
            }
        }


        private static void ReadTimeTypesShort(DbDataReader reader, List<TimeTypeRecord> result)
        {
            if (reader.HasRows)
            {
                int timeTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeId);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);

                while (reader.Read())
                {
                    var tt = new TimeTypeRecord
                    {
                        Id = reader.GetInt32(timeTypeIdIndex),
                        Name = reader.GetString(nameIndex),

                    };
                    result.Add(tt);

                }
            }

        }

        public static string GetWorkTypeNameById(int worktypeId)
        {
            string name = "";
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.TimeEntry.GetWorkTypeNameByIdProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeId, worktypeId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            name = reader.GetString(reader.GetOrdinal(Constants.ParameterNames.Name));
                        }
                    }

                }
            }

            return name;

        }


    }
}


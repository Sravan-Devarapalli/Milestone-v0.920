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
    /// <summary>
    /// 	Provides an access to the UserActivityLog database table.
    /// </summary>
    public static class ActivityLogDAL
    {
        #region Methods

        /// <summary>
        /// 	Retrives the list of the activity log items for the specified period.
        /// </summary>
        /// <param name = "context"></param>
        /// <param name = "pageSize">A page size.</param>
        /// <param name = "pageNo">A number of the page to be retrived.</param>
        /// <returns>The list of the <see cref = "ActivityLogItem" /> objects.</returns>
        public static List<ActivityLogItem> ActivityLogListByPeriod(ActivityLogSelectContext context, int pageSize, int pageNo)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.ActivityLog.ActivityLogListByPeriodProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, context.StartDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, context.EndDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId,
                                                context.PersonId.HasValue
                                                    ? (object) context.PersonId.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId,
                                                context.ProjectId.HasValue
                                                    ? (object) context.ProjectId.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PageSize, pageSize);
                command.Parameters.AddWithValue(Constants.ParameterNames.PageNo, pageNo);
                command.Parameters.AddWithValue(Constants.ParameterNames.EventSource, context.Source.ToString());
                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityIdParam,
                                                context.OpportunityId.HasValue ? (object)context.OpportunityId.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneIdParam,
                                                context.MilestoneId.HasValue ? (object)context.MilestoneId.Value : DBNull.Value);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<ActivityLogItem>();
                    ReadActivityLog(reader, result);
                    return result;
                }
            }
        }

        /// <summary>
        /// 	Retrives a number of the activity log items for the specified period.
        /// </summary>
        /// <param name = "context"></param>
        /// <returns>The number of user's activities.</returns>
        public static int ActivityLogGetCount(ActivityLogSelectContext context)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.ActivityLog.ActivityLogGetCountProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, context.StartDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, context.EndDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId,
                                                context.PersonId.HasValue ? (object)context.PersonId.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId,
                                                context.ProjectId.HasValue ? (object)context.ProjectId.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.EventSource, context.Source.ToString());
                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityIdParam,
                                                context.OpportunityId.HasValue ? (object)context.OpportunityId.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneIdParam,
                                                context.MilestoneId.HasValue ? (object)context.MilestoneId.Value : DBNull.Value);

                connection.Open();
                var result = (int) command.ExecuteScalar();

                return result;
            }
        }

        private static void ReadActivityLog(DbDataReader reader, ICollection<ActivityLogItem> result)
        {
            if (reader.HasRows)
            {
                var activityIdIndex = reader.GetOrdinal(Constants.ColumnNames.ActivityId);
                var activityTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.ActivityTypeId);
                var sessionIdIndex = reader.GetOrdinal(Constants.ColumnNames.SessionId);
                var logDateIndex = reader.GetOrdinal(Constants.ColumnNames.LogDate);
                var systemUserIndex = reader.GetOrdinal(Constants.ColumnNames.SystemUser);
                var workstationIndex = reader.GetOrdinal(Constants.ColumnNames.Workstation);
                var applicationNameIndex = reader.GetOrdinal(Constants.ColumnNames.ApplicationName);
                var userLoginIndex = reader.GetOrdinal(Constants.ColumnNames.UserLogin);
                var personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                var lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                var firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                var logDataIndex = reader.GetOrdinal(Constants.ColumnNames.LogData);
                var activityNameIndex = reader.GetOrdinal(Constants.ColumnNames.ActivityName);

                while (reader.Read())
                {
                    var item =
                        new ActivityLogItem
                            {
                                // Log data
                                Id = reader.GetInt32(activityIdIndex),
                                ActivityTypeId = reader.GetInt32(activityTypeIdIndex),
                                ActivityName = reader.GetString(activityNameIndex),
                                SessionId = reader.GetInt32(sessionIdIndex),
                                LogDate = reader.GetDateTime(logDateIndex),
                                SystemUser = reader.GetString(systemUserIndex),
                                Workstation =
                                    !reader.IsDBNull(workstationIndex) ? reader.GetString(workstationIndex) : null,
                                ApplicationName =
                                    !reader.IsDBNull(applicationNameIndex)
                                        ? reader.GetString(applicationNameIndex)
                                        : null,
                                LogData = !reader.IsDBNull(logDataIndex) ? reader.GetString(logDataIndex) : null,
                                // User's data
                                Person =
                                    !reader.IsDBNull(userLoginIndex)
                                        ? new Person
                                              {
                                                  Id =
                                                      !reader.IsDBNull(personIdIndex)
                                                          ? (int?) reader.GetInt32(personIdIndex)
                                                          : null,
                                                  Alias = reader.GetString(userLoginIndex),
                                                  FirstName =
                                                      !reader.IsDBNull(firstNameIndex)
                                                          ? reader.GetString(firstNameIndex)
                                                          : null,
                                                  LastName =
                                                      !reader.IsDBNull(lastNameIndex)
                                                          ? reader.GetString(lastNameIndex)
                                                          : null
                                              }
                                        : null
                            };

                    result.Add(item);
                }
            }
        }

        public static void ActivityLogInsert(int activityTypeId, string logData)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.ActivityLog.UserActivityLogInsertProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ActivityTypeId, activityTypeId);
                command.Parameters.AddWithValue(Constants.ParameterNames.LogData, logData);

                connection.Open();
                command.ExecuteScalar();
            }
        }


        /// <summary>
        /// 	Returns database version
        /// </summary>
        /// <returns>Returns database version</returns>
        public static string GetDatabaseVersion()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.ActivityLog.GetDatabaseVersionFunction, connection))
            {
                command.CommandTimeout = connection.ConnectionTimeout;

                connection.Open();
                return (string) command.ExecuteScalar();
            }
        }

        #endregion
    }
}

using System;
using System.Data;
using System.Data.SqlClient;
using DataAccess.Other;

namespace DataAccess
{
    /// <summary>
    /// Access person data in database
    /// </summary>
    public static class AspMembershipDAL
    {
       
        /// <summary>
        /// Set User to Locked-Out
        /// </summary>        
        /// <param name="username"></param>
        /// <param name="applicationName"></param>
        public static void UserSetLockedOut(string username, string applicationName, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.AspMembership.UserSetLockedOutProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.UserName, username);
                command.Parameters.AddWithValue(Constants.ParameterNames.ApplicationName, applicationName);
                command.Parameters.AddWithValue(Constants.ParameterNames.LastLockoutDate, DateTime.Now);

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

        public static void UserUnLockOut(string username, string applicationName, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.AspMembership.UserUnLockedOutProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.UserName, username);
                command.Parameters.AddWithValue(Constants.ParameterNames.ApplicationName, applicationName);

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
    }

}

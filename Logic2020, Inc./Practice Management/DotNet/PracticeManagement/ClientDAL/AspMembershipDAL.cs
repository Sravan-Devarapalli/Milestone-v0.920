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
        #region Constants

        #region Parameters

        private const string ApplicationName = "@ApplicationName";
        private const string UserName = "@UserName";
        private const string LastLockoutDate = "@LastLockoutDate";

        #endregion

        #region Stored Procedures

        private const string PersonInsertProcedure = "dbo.PersonInsert";
        private const string UserSetLockedOutProcedure = "dbo.aspnet_Membership_LockUser";
        private const string UserUnLockedOutProcedure = "dbo.aspnet_Membership_UnlockUser ";

        #endregion

        #region Columns

        private const string DescriptionColumn = "Description";
        private const string RateColumn = "Rate";
        private const string HoursToCollectColumn = "HoursToCollect";
        private const string StartDateColumn = "StartDate";
        private const string EndDateColumn = "EndDate";
        private const string NotesColumn = "Notes";
        private const string PersonIdColumn = "PersonId";
        private const string FirstNameColumn = "FirstName";
        private const string LastNameColumn = "LastName";
        private const string MonthColumn = "Month";
        private const string RevenueColumn = "Revenue";
        private const string CogsColumn = "Cogs";
        private const string MarginColumn = "Margin";
        private const string HireDateColumn = "HireDate";
        private const string TerminationDateColumn = "TerminationDate";
        private const string IsPercentageColumn = "IsPercentage";
        private const string PersonStatusIdColumn = "PersonStatusId";
        private const string PersonStatusNameColumn = "PersonStatusName";
        private const string OverheadRateTypeIdColumn = "OverheadRateTypeId";
        private const string OverheadRateTypeNameColumn = "OverheadRateTypeName";
        private const string EmployeeNumberColumn = "EmployeeNumber";
        private const string BillRateMultiplierColumn = "BillRateMultiplier";
        private const string EmployeesNumberColumn = "EmployeesNumber";
        private const string ConsultantsNumberColumn = "ConsultantsNumber";
        private const string BenchStartDateColumn = "BenchStartDate";
        private const string AliasColumn = "Alias";
        private const string SeniorityIdColumn = "SeniorityId";
        private const string SeniorityNameColumn = "SeniorityName";

        #endregion

        #endregion

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
            using (SqlCommand command = new SqlCommand(UserSetLockedOutProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(UserName, username);
                command.Parameters.AddWithValue(ApplicationName, applicationName);
                command.Parameters.AddWithValue(LastLockoutDate, DateTime.Now);

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
            using (SqlCommand command = new SqlCommand(UserUnLockedOutProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(UserName, username);
                command.Parameters.AddWithValue(ApplicationName, applicationName);

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

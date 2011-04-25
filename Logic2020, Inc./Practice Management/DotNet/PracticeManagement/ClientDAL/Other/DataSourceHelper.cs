using System.Configuration;

namespace DataAccess.Other
{
    /// <summary>
    /// Utilities that connect the DAL to a data store
    /// </summary>
    public static class DataSourceHelper
    {
		#region Constants

		private const string ConnectionStringNotFound = "The connection string \"{0}\" not found.";
		private const string ConnectionStringConnection = "connection";
		private const string ConenctionStringAspNetMembership = "AspNetMembership";

		#endregion

        /// <summary>
        /// The SQL Server connection string for the data store is in
		/// config file for the applciation hosting this service
        /// </summary>
        public static string DataConnection
        {
            get
            {
				return GetConnectionString(ConnectionStringConnection);
            }
        }

		/// <summary>
		/// The SQL Server connection string for the membership store is in
		/// config file for the applciation hosting this service
		/// </summary>
		public static string MembershipConnection
		{
			get
			{
				return GetConnectionString(ConenctionStringAspNetMembership);
			}
		}

		private static string GetConnectionString(string connectionStringName)
		{
			ConnectionStringSettings settings =
				ConfigurationManager.ConnectionStrings[connectionStringName];
			string connectionString;
			if (settings == null)
			{
				throw new ConfigurationErrorsException(
					string.Format(ConnectionStringNotFound, connectionStringName));
			}
			else
			{
				connectionString = settings.ConnectionString;
			}
			return connectionString;
		}
	}
}

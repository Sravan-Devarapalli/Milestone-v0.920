using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace PracticeManagementDatabaseUI
{
    public class ConnectionProperties
    {
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }

        public string ConnectionString 
        { 
            get 
            {
                var builder = new SqlConnectionStringBuilder();
                builder["Server"] = ServerName;
                builder["Connect Timeout"] = 5;
                builder["Trusted_Connection"] = true;

                return builder.ConnectionString;
            } 
        }
    }
}


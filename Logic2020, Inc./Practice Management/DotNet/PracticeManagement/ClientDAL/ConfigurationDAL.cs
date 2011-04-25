using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataTransferObjects;
using System.Data.SqlClient;
using System.Data;
using DataAccess.Other;
using DataTransferObjects.ContextObjects;

namespace DataAccess
{
    public static class ConfigurationDAL
    {
        #region Constants 
         
        #region Parameters

        private const string TitleParam = "@Title";
        private const string ImageNameParam = "@FileName";
        private const string ImagePathParam = "@FilePath";
        private const string DataParam = "@Data";

        #endregion Parameters

        #region Columns

        private const string TitleColumn = "Title";
        private const string FileNameColumn = "FileName";
        private const string FilePathColumn = "FilePath";
        private const string DataColumn = "Data";

        #endregion Columns

        #endregion Constants


        public static void SaveCompanyLogoData(string title, string imagename, string imagePath, Byte[] data)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Configuration.CompanyLogoDataSaveProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(TitleParam, title);
                    command.Parameters.AddWithValue(ImageNameParam, !string.IsNullOrEmpty(imagename) ? (object)imagename : DBNull.Value);
                    command.Parameters.AddWithValue(ImagePathParam, !string.IsNullOrEmpty(imagePath) ? (object)imagePath : DBNull.Value);
                    command.Parameters.Add(DataParam, SqlDbType.VarBinary, -1);
                    command.Parameters[DataParam].Value = data != null ? (object)data : DBNull.Value;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static CompanyLogo GetCompanyLogoData()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Configuration.GetCompanyLogoDataProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        CompanyLogo companylogo = new CompanyLogo();
                        if (reader.HasRows)
                        {
                            int titleIndex = reader.GetOrdinal(TitleColumn);
                            int FileNameIndex = reader.GetOrdinal(FileNameColumn);
                            int FilePathIndex = reader.GetOrdinal(FilePathColumn);
                            int DataColumnIndex = reader.GetOrdinal(DataColumn);


                            while (reader.Read())
                            {
                                companylogo.Title = reader.GetString(titleIndex);
                                companylogo.FileName = !reader.IsDBNull(FileNameIndex) ? reader.GetString(FileNameIndex) : string.Empty;
                                companylogo.FilePath = !reader.IsDBNull(FilePathIndex) ? reader.GetString(FilePathIndex) : string.Empty;
                                companylogo.Data = (byte[])reader[DataColumnIndex];
                            }
                        }
                        return companylogo;
                    }
                }
            }
        }

        public static void SaveResourceKeyValuePairs(SettingsType settingType, Dictionary<string, string> dictionary)
        {
           if(dictionary != null && dictionary.Keys.Count > 0)
           {
           
            foreach (var item in dictionary)
	        {
                using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
                {
                    using (var command = new SqlCommand(Constants.ProcedureNames.Configuration.SaveSettingsKeyValuePairsProcedure, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = connection.ConnectionTimeout;

                        command.Parameters.AddWithValue(Constants.ParameterNames.SettingsTypeParam, (int)settingType);
                        command.Parameters.AddWithValue(Constants.ParameterNames.SettingsKeyParam, item.Key);
                        command.Parameters.AddWithValue(Constants.ParameterNames.ValueParam, item.Value);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
             }
           }
        }


        public static bool SaveResourceKeyValuePairItem(SettingsType settingType, string key, string value)
        {
            int rowsAffected = 0;
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Configuration.SaveSettingsKeyValuePairsProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.SettingsTypeParam, (int)settingType);
                    command.Parameters.AddWithValue(Constants.ParameterNames.SettingsKeyParam, key);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ValueParam, value);

                    connection.Open();
                    rowsAffected = command.ExecuteNonQuery();
                }
            }
            return rowsAffected > 0;
        }


        public static Dictionary<string, string> GetResourceKeyValuePairs(SettingsType settingType)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Configuration.GetSettingsKeyValuePairsProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue(Constants.ParameterNames.SettingsTypeParam, (int)settingType);
                    command.CommandTimeout = connection.ConnectionTimeout;
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Dictionary<string, string> dictionary = new Dictionary<string, string>();

                        if (reader.HasRows)
                        {
                            int ResourcesKeyIndex = reader.GetOrdinal(Constants.ColumnNames.SettingsKeyColumn);
                            int ValueIndex = reader.GetOrdinal(Constants.ColumnNames.ValueColumn);

                            while (reader.Read())
                            {
                               dictionary.Add(reader.GetString(ResourcesKeyIndex),reader.GetString(ValueIndex));
                            }
                        }

                        return dictionary;
                    }
                }
            }
        }
    }
}


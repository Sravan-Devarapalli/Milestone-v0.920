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
        private const string GoalTypeIdParam = "@GoalTypeId";
        private const string ColorIdParam = "@ColorId";
        private const string StartRangeParam = "@StartRange";
        private const string EndRangeParam = "@EndRange";
        private const string isDeletePreviousMarginInfoParam = "@isDeletePreviousMarginInfo";

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
            if (dictionary != null && dictionary.Keys.Count > 0)
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


        public static bool SaveResourceKeyValuePairItem(SettingsType settingType, string key, string value, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            int rowsAffected = 0;

            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

            using (var command = new SqlCommand(Constants.ProcedureNames.Configuration.SaveSettingsKeyValuePairsProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.SettingsTypeParam, (int)settingType);
                command.Parameters.AddWithValue(Constants.ParameterNames.SettingsKeyParam, key);
                command.Parameters.AddWithValue(Constants.ParameterNames.ValueParam, value);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                if (activeTransaction != null)
                {
                    command.Transaction = activeTransaction;
                }

                rowsAffected = command.ExecuteNonQuery();
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
                                dictionary.Add(reader.GetString(ResourcesKeyIndex), reader.GetString(ValueIndex));
                            }
                        }

                        return dictionary;
                    }
                }
            }
        }

        public static void SaveMarginInfoDetail(List<Triple<DefaultGoalType, Triple<SettingsType, string, string>, List<ClientMarginColorInfo>>> marginInfoList)
        {
            if (marginInfoList != null && marginInfoList.Count > 0)
            {
                using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
                {
                    connection.Open();

                    SqlTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

                    foreach (var triple in marginInfoList)
                    {
                        SaveResourceKeyValuePairItem((SettingsType)triple.Second.First, triple.Second.Second.ToString(), triple.Second.Third.ToString());

                        if (Convert.ToBoolean(triple.Second.Third) && triple.Third != null)
                        {

                            for (int i = 0; i < triple.Third.Count; i++)
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
                                DefaultMarginColorInfoInsert((int)triple.First, triple.Third[i], isDeletePreviousMarginInfo, connection, transaction);
                            }

                        }
                    }

                    transaction.Commit();
                }
            }
        }

        private static void DefaultMarginColorInfoInsert(int goalTypeId, ClientMarginColorInfo marginColorInfo, bool isDeletePreviousMarginInfo, SqlConnection connection = null, SqlTransaction activeTransaction = null)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

            using (var command = new SqlCommand(Constants.ProcedureNames.Configuration.SaveMarginInfoDefaultsProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(GoalTypeIdParam, goalTypeId);
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



        public static List<ClientMarginColorInfo> GetMarginColorInfoDefaults(DefaultGoalType goalType)
        {
            var clientMarginColorInfo = new List<ClientMarginColorInfo>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Configuration.GetMarginColorInfoDefaultsProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(GoalTypeIdParam, (int)goalType);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        ReadMarginColorInfoDefaults(reader, clientMarginColorInfo);
                    }
                }
            }
            if (clientMarginColorInfo.Count > 0)
                return clientMarginColorInfo;
            else
                return null;
        }

        private static void ReadMarginColorInfoDefaults(SqlDataReader reader, List<ClientMarginColorInfo> clientMarginColorInfo)
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
                                                        ColorId = reader.GetInt32(colorIdIndex),
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

    }
}


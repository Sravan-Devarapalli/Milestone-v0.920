using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
    public static class SeniorityDAL
    {

        #region Methods

        /// <summary>
        /// Selects a list of the seniorities.
        /// </summary>
        /// <returns>A list of the <see cref="Seniority"/> objects.</returns>
        public static List<Seniority> ListAll()
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Seniority.SeniorityListAllProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<Seniority> result = new List<Seniority>();
                    ReadSeniorities(reader, result);
                    return result;
                }
            }
        }

        private static void ReadSeniorities(DbDataReader reader, List<Seniority> result)
        {
            if (reader.HasRows)
            {
                int seniorityIdIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorityIdColumn);
                int seniorityIndex = reader.GetOrdinal(Constants.ColumnNames.Seniority);
                int seniorityCategoryIdIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorityCategoryId);
                int seniorityCategoryIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorityCategory);
                int seniorityValueIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorityValue);

                while (reader.Read())
                {
                    result.Add(
                        new Seniority()
                        {
                            Id = reader.GetInt32(seniorityIdIndex),
                            Name = reader.GetString(seniorityIndex),
                            SeniorityValue = reader.GetInt32(seniorityValueIndex),
                            SeniorityCategory = new SeniorityCategory
                            {
                                Id = reader.GetInt32(seniorityCategoryIdIndex),
                                Name = reader.GetString(seniorityCategoryIndex)
                            }
                        });
                }
            }
        }

        private static void ReadSeniorityCategories(DbDataReader reader, List<SeniorityCategory> result)
        {
            if (reader.HasRows)
            {
                int seniorityCategoryIdIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorityCategoryId);
                int seniorityCategoryIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorityCategory);

                while (reader.Read())
                {
                    result.Add(
                        new SeniorityCategory()
                        {
                            Id = reader.GetInt32(seniorityCategoryIdIndex),
                            Name = reader.GetString(seniorityCategoryIndex)
                        });
                }
            }
        }

        public static List<SeniorityCategory> ListAllSeniorityCategories()
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Seniority.ListAllSeniorityCategories, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<SeniorityCategory> result = new List<SeniorityCategory>();
                    ReadSeniorityCategories(reader, result);
                    return result;
                }
            }
        }

        #endregion
    }
}


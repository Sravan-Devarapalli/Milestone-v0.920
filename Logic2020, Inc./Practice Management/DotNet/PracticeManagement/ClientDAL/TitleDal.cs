using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataTransferObjects;
using DataAccess.Other;
using System.Data.SqlClient;
using System.Data;

namespace DataAccess
{
    public static class TitleDal
    {
        public static List<Title> GetAllTitles()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Title.GetAllTitles, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<Title>();
                    ReadTitles(reader, result);
                    return result;
                }
            }
        }

        private static void ReadTitles(SqlDataReader reader, List<Title> result)
        {
            if (reader.HasRows)
            {

                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                int titleTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleTypeId);
                int titleTypeIndex = reader.GetOrdinal(Constants.ColumnNames.TitleType);
                int sortOrderIndex = reader.GetOrdinal(Constants.ColumnNames.SortOrder);
                int pTOAccrualIndex = reader.GetOrdinal(Constants.ColumnNames.PTOAccrual);
                int minimumSalaryIndex = reader.GetOrdinal(Constants.ColumnNames.MinimumSalary);
                int maximumSalaryIndex = reader.GetOrdinal(Constants.ColumnNames.MaximumSalary);


                while (reader.Read())
                {
                    result.Add(
                        new Title()
                        {
                            TitleId = reader.GetInt32(titleIdIndex),
                            TitleName = reader.GetString(titleIndex),
                            TitleType = new TitleType()
                            {
                                TitleTypeId = reader.GetInt32(titleTypeIdIndex),
                                TitleTypeName = reader.GetString(titleTypeIndex)
                            },
                            SortOrder = reader.GetInt32(sortOrderIndex),
                            PTOAccrual = reader.GetInt32(pTOAccrualIndex),
                            MinimumSalary = !reader.IsDBNull(minimumSalaryIndex) ? reader.GetInt32(minimumSalaryIndex) : (int?)null,
                            MaximumSalary = !reader.IsDBNull(maximumSalaryIndex) ? reader.GetInt32(maximumSalaryIndex) : (int?)null
                        });
                }
            }
        }

        public static Title GetTitleById(int titleId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Title.GetTitleById, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.TitleId, titleId);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return ReadTitle(reader);
                }
            }
        }

        private static Title ReadTitle(SqlDataReader reader)
        {
            if (reader.HasRows)
            {

                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                int titleTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleTypeId);
                int titleTypeIndex = reader.GetOrdinal(Constants.ColumnNames.TitleType);
                int sortOrderIndex = reader.GetOrdinal(Constants.ColumnNames.SortOrder);
                int pTOAccrualIndex = reader.GetOrdinal(Constants.ColumnNames.PTOAccrual);
                int minimumSalaryIndex = reader.GetOrdinal(Constants.ColumnNames.MinimumSalary);
                int maximumSalaryIndex = reader.GetOrdinal(Constants.ColumnNames.MaximumSalary);


                while (reader.Read())
                {
                    return new Title()
                        {
                            TitleId = reader.GetInt32(titleIdIndex),
                            TitleName = reader.GetString(titleIndex),
                            TitleType = new TitleType()
                            {
                                TitleTypeId = reader.GetInt32(titleTypeIdIndex),
                                TitleTypeName = reader.GetString(titleTypeIndex)
                            },
                            SortOrder = reader.GetInt32(sortOrderIndex),
                            PTOAccrual = reader.GetInt32(pTOAccrualIndex),
                            MinimumSalary = !reader.IsDBNull(minimumSalaryIndex) ? reader.GetInt32(minimumSalaryIndex) : (int?)null,
                            MaximumSalary = !reader.IsDBNull(maximumSalaryIndex) ? reader.GetInt32(maximumSalaryIndex) : (int?)null
                        };
                }
            }
            return null;
        }

        public static void TitleInset(string title, int titleTypeId, int sortOrder, int pTOAccural, int? minimumSalary, int? maximumSalary, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Title.TitleInset, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.Title, title);
                command.Parameters.AddWithValue(Constants.ParameterNames.TitleTypeId, titleTypeId);
                command.Parameters.AddWithValue(Constants.ParameterNames.SortOrder, sortOrder);
                command.Parameters.AddWithValue(Constants.ParameterNames.PTOAccrual, pTOAccural);
                command.Parameters.AddWithValue(Constants.ParameterNames.MinimumSalary, minimumSalary.HasValue ? minimumSalary.Value : (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.MaximumSalary, maximumSalary.HasValue ? maximumSalary.Value : (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void TitleUpdate(int titleId,string title, int titleTypeId, int sortOrder, int pTOAccural, int? minimumSalary, int? maximumSalary, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Title.TitleUpdate, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                 command.Parameters.AddWithValue(Constants.ParameterNames.TitleId, titleId);
                command.Parameters.AddWithValue(Constants.ParameterNames.Title, title);
                command.Parameters.AddWithValue(Constants.ParameterNames.TitleTypeId, titleTypeId);
                command.Parameters.AddWithValue(Constants.ParameterNames.SortOrder, sortOrder);
                command.Parameters.AddWithValue(Constants.ParameterNames.PTOAccrual, pTOAccural);
                command.Parameters.AddWithValue(Constants.ParameterNames.MinimumSalary, minimumSalary.HasValue ? minimumSalary.Value : (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.MaximumSalary, maximumSalary.HasValue ? maximumSalary.Value : (Object)DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void TitleDelete(int titleId, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Title.TitleDelete, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(Constants.ParameterNames.TitleId, titleId);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
    public static class OverHeadHistoryDAL
    {
        private static void ReadOverheadHistory(DbDataReader reader, List<OverHeadHistory> result)
        {
            if (!reader.HasRows) return;
            int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDateColumn);
            int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDateColumn);
            int w2Salary_RateIndex = reader.GetOrdinal(Constants.ColumnNames.W2Salary_RateColumn);
            int w2Hourly_RateIndex = reader.GetOrdinal(Constants.ColumnNames.W2Hourly_RateColumn);
            int _1099_Hourly_RateIndex = reader.GetOrdinal(Constants.ColumnNames._1099_Hourly_RateColumn);

            while (reader.Read())
            {
                OverHeadHistory mlfHistory = new OverHeadHistory
                    {
                        StartDate = reader.GetDateTime(startDateIndex),
                        EndDate = !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null,
                        W2Salary_Rate = reader.GetDecimal(w2Salary_RateIndex),
                        W2Hourly_Rate = reader.GetDecimal(w2Hourly_RateIndex),
                        _1099_Hourly_Rate = reader.GetDecimal(_1099_Hourly_RateIndex)
                    };

                result.Add(mlfHistory);
            }
        }

        public static List<OverHeadHistory> GetOverheadHistory()
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.OverHeads.GetMLFHistory, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<OverHeadHistory> result = new List<OverHeadHistory>();

                    ReadOverheadHistory(reader, result);
                    return result;
                }
            }
        }
    }
}

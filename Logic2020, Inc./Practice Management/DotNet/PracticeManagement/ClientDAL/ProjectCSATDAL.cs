using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataTransferObjects;
using System.Data.SqlClient;
using DataAccess.Other;
using System.Data;

namespace DataAccess
{
    public static class ProjectCSATDAL
    {
        public static int CSATInsert(ProjectCSAT projectCSAT, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.CSATInsert, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, projectCSAT.ProjectId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ReviewStartDate, projectCSAT.ReviewStartDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ReviewEndDate, projectCSAT.ReviewEndDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.CompletionDate, projectCSAT.CompletionDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ReviewerId, projectCSAT.ReviewerId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ReferralScore, projectCSAT.ReferralScore);
                    command.Parameters.AddWithValue(Constants.ParameterNames.Comments, projectCSAT.Comments);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);

                    SqlParameter ProjectCSATIdParam = new SqlParameter(Constants.ParameterNames.ProjectCSATId, SqlDbType.Int);
                    ProjectCSATIdParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(ProjectCSATIdParam);

                    connection.Open();
                    command.ExecuteNonQuery();
                    return (int)ProjectCSATIdParam.Value;
                }
            }
        }

        public static void CSATDelete(int projectCSATId, string userLogin)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.CSATDelete, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectCSATId, projectCSATId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void CSATUpdate(ProjectCSAT projectCSAT, string userLogin)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.CSATUpdate, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectCSATId, projectCSAT.Id);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ReviewStartDate, projectCSAT.ReviewStartDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ReviewEndDate, projectCSAT.ReviewEndDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.CompletionDate, projectCSAT.CompletionDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ReviewerId, projectCSAT.ReviewerId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ReferralScore, projectCSAT.ReferralScore);
                    command.Parameters.AddWithValue(Constants.ParameterNames.Comments, projectCSAT.Comments);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<ProjectCSAT> CSATList(int? projectId)
        {
            List<ProjectCSAT> result = new List<ProjectCSAT>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.CSATList, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    if(projectId.HasValue)
                        command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, projectId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        ReadProjectCSAT(reader, result);
                    }
                }
            }
            return result;
        }

        public static void ReadProjectCSAT(SqlDataReader reader, List<ProjectCSAT> result)
        {
            if (reader.HasRows)
            {

                int projectCSATIdIndex = reader.GetOrdinal(Constants.ColumnNames.CSATId);
                int reviewStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.ReviewStartDate);
                int reviewEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.ReviewEndDate);
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                int completionDateIndex = reader.GetOrdinal(Constants.ColumnNames.CompletionDate);
                int reviewerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ReviewerId);
                int referralScoreIndex = reader.GetOrdinal(Constants.ColumnNames.ReferralScore);
                int commentsIndex = reader.GetOrdinal(Constants.ColumnNames.Comments);
                int reviewerNameIndex = reader.GetOrdinal(Constants.ColumnNames.ReviewerName);

                while (reader.Read())
                {
                    ProjectCSAT ProjectCSAT = new ProjectCSAT();
                    ProjectCSAT.Id = reader.GetInt32(projectCSATIdIndex);
                    ProjectCSAT.ReviewStartDate = reader.GetDateTime(reviewStartDateIndex);
                    ProjectCSAT.ReviewEndDate = reader.GetDateTime(reviewEndDateIndex);
                    ProjectCSAT.CompletionDate = reader.GetDateTime(completionDateIndex);
                    ProjectCSAT.Comments = reader.GetString(commentsIndex);
                    ProjectCSAT.ProjectId = reader.GetInt32(projectIdIndex);
                    ProjectCSAT.ReferralScore = reader.GetInt32(referralScoreIndex);
                    ProjectCSAT.ReviewerId = reader.GetInt32(reviewerIdIndex);
                    ProjectCSAT.ReviewerName = reader.GetString(reviewerNameIndex);
                    result.Add(ProjectCSAT);
                }
            }
        }
    }
}


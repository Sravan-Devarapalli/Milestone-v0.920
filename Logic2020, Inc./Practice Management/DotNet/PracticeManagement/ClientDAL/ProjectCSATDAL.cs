using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DataAccess.Other;
using DataTransferObjects;

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

                    SqlParameter ProjectCSATIdParam = new SqlParameter(Constants.ParameterNames.ProjectCSATId, SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
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
                    if (projectId.HasValue)
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
            if (!reader.HasRows) return;
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
                ProjectCSAT ProjectCSAT = new ProjectCSAT
                    {
                        Id = reader.GetInt32(projectCSATIdIndex),
                        ReviewStartDate = reader.GetDateTime(reviewStartDateIndex),
                        ReviewEndDate = reader.GetDateTime(reviewEndDateIndex),
                        CompletionDate = reader.GetDateTime(completionDateIndex),
                        Comments = reader.GetString(commentsIndex),
                        ProjectId = reader.GetInt32(projectIdIndex),
                        ReferralScore = reader.GetInt32(referralScoreIndex),
                        ReviewerId = reader.GetInt32(reviewerIdIndex),
                        ReviewerName = reader.GetString(reviewerNameIndex)
                    };
                result.Add(ProjectCSAT);
            }
        }

        public static List<Project> CSATSummaryReport(DateTime startDate, DateTime endDate, string practiceIds, string accountIds, bool isExport = false)
        {
            List<Project> result = new List<Project>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Reports.CSATSummaryReport, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIds);
                    command.Parameters.AddWithValue(Constants.ParameterNames.AccountIdsParam, accountIds);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsExport, isExport);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        ReadReportSummary(reader, result, isExport);
                    }
                }
            }
            return result;
        }

        public static void ReadReportSummary(SqlDataReader reader, List<Project> result, bool isExport)
        {
            if (!reader.HasRows) return;
            int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
            int accountIndex = reader.GetOrdinal(Constants.ColumnNames.Account);
            int businessGroupNameIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessGroupName);
            int businessUnitNameIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessUnitName);
            int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
            int buyerNameIndex = reader.GetOrdinal(Constants.ColumnNames.BuyerNameColumn);
            int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
            int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
            int practiceAreaNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeAreaName);
            int sowBudgetIndex = reader.GetOrdinal(Constants.ColumnNames.SowBudgetColumn);
            int referralScoreIndex = reader.GetOrdinal(Constants.ColumnNames.ReferralScore);
            int cSATIdIndex = reader.GetOrdinal(Constants.ColumnNames.CSATId);
            int HasMultipleCSATsIndex = reader.GetOrdinal(Constants.ColumnNames.HasMultipleCSATs);

            int projectOwnerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectOwnerFirstName);
            int ProjectOwnerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectOwnerLastName);
            int cSATEligibleIndex = reader.GetOrdinal(Constants.ColumnNames.CSATEligible);
            int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
            int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDate);
            int completedStatusDateIndex = reader.GetOrdinal(Constants.ColumnNames.CompletedStatusDate);
            int salesPersonIndex = reader.GetOrdinal(Constants.ColumnNames.SalesPerson);
            int directorFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorFirstNameColumn);
            int directorLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorLastNameColumn);
            int projectManagersIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagers);
            int cSATOwnerNameIndex = reader.GetOrdinal(Constants.ColumnNames.CSATOwnerName);
            int completionDateIndex = reader.GetOrdinal(Constants.ColumnNames.CompletionDate);
            int cSATReviewerIndex = reader.GetOrdinal(Constants.ColumnNames.CSATReviewer);
            int commentsIndex = reader.GetOrdinal(Constants.ColumnNames.Comments);
            int reviewStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.ReviewStartDate);
            int reviewEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.ReviewEndDate);
            while (reader.Read())
            {
                Project project;
                string projectNumber = !reader.IsDBNull(projectNumberIndex) ? reader.GetString(projectNumberIndex) : null;
                if (result.All(p => p.ProjectNumber != projectNumber))
                {
                    project = new Project
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            BuyerName =
                                !reader.IsDBNull(buyerNameIndex) ? reader.GetString(buyerNameIndex) : string.Empty,
                            ProjectNumber = projectNumber,
                            HasMultipleCSATs = reader.GetInt32(HasMultipleCSATsIndex) == 1,
                            Name = !reader.IsDBNull(projectNameIndex) ? reader.GetString(projectNameIndex) : null,
                            Client = new Client()
                                {
                                    Name = !reader.IsDBNull(accountIndex) ? reader.GetString(accountIndex) : null
                                },
                            BusinessGroup = new BusinessGroup()
                                {
                                    Name =
                                        !reader.IsDBNull(businessGroupNameIndex)
                                            ? reader.GetString(businessGroupNameIndex)
                                            : null
                                },
                            Group = new ProjectGroup()
                                {
                                    Name =
                                        !reader.IsDBNull(businessUnitNameIndex)
                                            ? reader.GetString(businessUnitNameIndex)
                                            : null
                                },
                            Status = new ProjectStatus()
                                {
                                    Name =
                                        !reader.IsDBNull(projectStatusNameIndex)
                                            ? reader.GetString(projectStatusNameIndex)
                                            : null
                                },
                            Practice = new Practice()
                                {
                                    Name =
                                        !reader.IsDBNull(practiceAreaNameIndex)
                                            ? reader.GetString(practiceAreaNameIndex)
                                            : null
                                },
                            SowBudget = !reader.IsDBNull(sowBudgetIndex) ? reader.GetDecimal(sowBudgetIndex) : 0,
                            CSATList = new List<ProjectCSAT>()
                        };

                    ProjectCSAT cSATItem = new ProjectCSAT()
                        {
                            ReferralScore = !reader.IsDBNull(referralScoreIndex) ? reader.GetInt32(referralScoreIndex) : -2,
                            Id = !reader.IsDBNull(cSATIdIndex) ? reader.GetInt32(cSATIdIndex) : -1,
                            CompletionDate = !reader.IsDBNull(completionDateIndex) ? reader.GetDateTime(completionDateIndex) : DateTime.MinValue,
                            ReviewStartDate = !reader.IsDBNull(reviewStartDateIndex) ? reader.GetDateTime(reviewStartDateIndex) : DateTime.MinValue,
                            ReviewEndDate = !reader.IsDBNull(reviewEndDateIndex) ? reader.GetDateTime(reviewEndDateIndex) : DateTime.MinValue,
                            ReviewerName = !reader.IsDBNull(cSATReviewerIndex) ? reader.GetString(cSATReviewerIndex) : null,
                            Comments = !reader.IsDBNull(commentsIndex) ? reader.GetString(commentsIndex) : null
                        };

                    if (isExport)
                    {
                        project.IsCSATEligible = reader.GetInt32(cSATEligibleIndex) == 1;
                        project.StartDate = !reader.IsDBNull(startDateIndex) ? reader.GetDateTime(startDateIndex) : DateTime.MinValue;
                        project.EndDate = !reader.IsDBNull(endDateIndex) ? reader.GetDateTime(endDateIndex) : DateTime.MinValue;
                        project.RecentCompletedStatusDate = !reader.IsDBNull(completedStatusDateIndex) ? reader.GetDateTime(completedStatusDateIndex) : DateTime.MinValue;
                        project.SalesPersonName = !reader.IsDBNull(salesPersonIndex) ? reader.GetString(salesPersonIndex) : null;
                        project.ProjectOwner = new Person()
                            {
                                FirstName = !reader.IsDBNull(projectOwnerFirstNameIndex) ? reader.GetString(projectOwnerFirstNameIndex) : null,
                                LastName = !reader.IsDBNull(ProjectOwnerLastNameIndex) ? reader.GetString(ProjectOwnerLastNameIndex) : null
                            };
                        project.ProjectManagerNames = !reader.IsDBNull(projectManagersIndex) ? reader.GetString(projectManagersIndex) : null;
                        project.CSATOwnerName = !reader.IsDBNull(cSATOwnerNameIndex) ? reader.GetString(cSATOwnerNameIndex) : null;
                        project.Director = new Person()
                            {
                                FirstName = !reader.IsDBNull(directorFirstNameIndex) ? reader.GetString(directorFirstNameIndex) : null,
                                LastName = !reader.IsDBNull(directorLastNameIndex) ? reader.GetString(directorLastNameIndex) : null
                            };
                    }
                    project.CSATList.Add(cSATItem);
                    result.Add(project);
                }
                else
                {
                    project = result.Find(p => p.ProjectNumber == projectNumber);
                    ProjectCSAT cSATItem = new ProjectCSAT()
                        {
                            ReferralScore = !reader.IsDBNull(referralScoreIndex) ? reader.GetInt32(referralScoreIndex) : -1,
                            Id = !reader.IsDBNull(cSATIdIndex) ? reader.GetInt32(cSATIdIndex) : -1,
                            CompletionDate = !reader.IsDBNull(completionDateIndex) ? reader.GetDateTime(completionDateIndex) : DateTime.MinValue,
                            ReviewStartDate = !reader.IsDBNull(reviewStartDateIndex) ? reader.GetDateTime(reviewStartDateIndex) : DateTime.MinValue,
                            ReviewEndDate = !reader.IsDBNull(reviewEndDateIndex) ? reader.GetDateTime(reviewEndDateIndex) : DateTime.MinValue,
                            ReviewerName = !reader.IsDBNull(cSATReviewerIndex) ? reader.GetString(cSATReviewerIndex) : null,
                            Comments = !reader.IsDBNull(commentsIndex) ? reader.GetString(commentsIndex) : null
                        };
                    project.CSATList.Add(cSATItem);
                }
            }
        }

        public static List<int> CSATReportHeader(DateTime startDate, DateTime endDate, string practiceIds, string accountIds)
        {
            List<int> result = new List<int>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Reports.CSATReportHeader, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIds);
                    command.Parameters.AddWithValue(Constants.ParameterNames.AccountIdsParam, accountIds);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        ReadReportHeader(reader, result);
                    }
                }
            }
            return result;
        }

        public static void ReadReportHeader(SqlDataReader reader, List<int> result)
        {
            if (!reader.HasRows) return;
            int promotersWithoutFilterIndex = reader.GetOrdinal(Constants.ColumnNames.PromotersWithoutFilter);
            int passivesWithoutFilterIndex = reader.GetOrdinal(Constants.ColumnNames.PassivesWithoutFilter);
            int detractorsWithoutFilterIndex = reader.GetOrdinal(Constants.ColumnNames.DetractorsWithoutFilter);
            int promotersWithFilterIndex = reader.GetOrdinal(Constants.ColumnNames.PromotersWithFilter);
            int passivesWithFilterIndex = reader.GetOrdinal(Constants.ColumnNames.PassivesWithFilter);
            int detractorsWithFilterIndex = reader.GetOrdinal(Constants.ColumnNames.DetractorsWithFilter);
            while (reader.Read())
            {
                result.Add(!reader.IsDBNull(promotersWithoutFilterIndex) ? reader.GetInt32(promotersWithoutFilterIndex) : 0);
                result.Add(!reader.IsDBNull(passivesWithoutFilterIndex) ? reader.GetInt32(passivesWithoutFilterIndex) : 0);
                result.Add(!reader.IsDBNull(detractorsWithoutFilterIndex) ? reader.GetInt32(detractorsWithoutFilterIndex) : 0);
                result.Add(!reader.IsDBNull(promotersWithFilterIndex) ? reader.GetInt32(promotersWithFilterIndex) : 0);
                result.Add(!reader.IsDBNull(passivesWithFilterIndex) ? reader.GetInt32(passivesWithFilterIndex) : 0);
                result.Add(!reader.IsDBNull(detractorsWithFilterIndex) ? reader.GetInt32(detractorsWithFilterIndex) : 0);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using DataAccess.Other;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;

namespace DataAccess
{
    using System.Data.SqlTypes;

    /// <summary>
    /// Access project data in database
    /// </summary>
    public class ProjectDAL
    {
        /// <summary>
        /// Retrieves the list of projects which intended for the specific client.
        /// </summary>
        /// <param name="clientId">An ID of the client.</param>
        /// <returns>The list of the <see cref="Project"/> objects.</returns>
        public static int ProjectCountByClient(int clientId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.ProjectsCountByClient, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdParam, clientId);

                    connection.Open();

                    return (int)command.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Retrieves the list of projects which intended for the specific client.
        /// </summary>
        /// <param name="clientId">An ID of the client.</param>
        /// <param name="viewerUsername"></param>
        /// <returns>The list of the <see cref="Project"/> objects.</returns>
        public static List<Project> ProjectListByClient(int clientId, string viewerUsername)
        {
            var projectList = new List<Project>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.ProjectsListByClient, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdParam, clientId);
                    command.Parameters.AddWithValue(
                        Constants.ParameterNames.Alias, viewerUsername);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    ReadProjects(reader, projectList, false); //If you alter readGroups Parameter to true then you need to add the GroupName parameter,InUse Parameter  in "dbo.ProjectsListByClient"
                }
            }

            return projectList;
        }


        public static List<Project> ListProjectsByClientShort(int clientId, bool IsOnlyActiveAndProjective)
        {
            var projectList = new List<Project>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.ListProjectsByClientShort, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdParam, clientId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsOnlyActiveAndProjective, IsOnlyActiveAndProjective);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    ReadProjectsShort(reader, projectList, false); //If you alter readGroups Parameter to true then you need to add the GroupName parameter,InUse Parameter  in "dbo.ProjectsListByClient"
                }
            }

            return projectList;
        }

        private static void ReadProjectsShort(SqlDataReader reader, List<Project> resultList, bool readGroups)
        {
            try
            {
                if (reader.HasRows)
                {
                    int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                    int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
                    int nameIndex = reader.GetOrdinal(Constants.ColumnNames.NameColumn);
                    int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
                    int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
                    int buyerNameIndex = reader.GetOrdinal(Constants.ColumnNames.BuyerNameColumn);

                    while (reader.Read())
                    {
                        var project = new Project
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            Name = reader.GetString(nameIndex),
                            ProjectNumber = reader.GetString(projectNumberIndex),
                            BuyerName = !reader.IsDBNull(buyerNameIndex) ? reader.GetString(buyerNameIndex) : null
                        };

                        project.Client = new Client
                        {
                            Id = reader.GetInt32(clientIdIndex),
                            Name = reader.GetString(clientNameIndex)
                        };
                        resultList.Add(project);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Retrieves the list of projects which intended for the specific client.
        /// </summary>
        /// <param name="clientId">An ID of the client.</param>
        /// <param name="viewerUsername"></param>
        /// <returns>The list of the <see cref="Project"/> objects.</returns>
        public static List<Project> ProjectListByClientWithSorting(int clientId, string viewerUsername, string sortBy)
        {
            var projectList = new List<Project>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.ProjectsListByClientWithSort, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdParam, clientId);
                    command.Parameters.AddWithValue(
                        Constants.ParameterNames.Alias, viewerUsername);

                    if (!string.IsNullOrEmpty(sortBy))
                    {
                        command.Parameters.AddWithValue(Constants.ParameterNames.SortByParam, sortBy);
                    }

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    ReadProjects(reader, projectList, true);
                }
            }

            return projectList;
        }
        /// <summary>
        /// Enlists the requested projects.
        /// </summary>
        /// <param name="clientId">An ID of the client the projects belong to.</param>
        /// <param name="showActive">If true - enlist the active project only.</param>
        /// <param name="showProjected">If true - the projected projects will are included in the results.</param>
        /// <param name="showCompleted">If true - the completed projects will are included in the results.</param>
        /// <param name="showExperimental">If true - the experimantal projects will are included in the results.</param>
        /// <param name="periodStart">The start of the period to enlist the projects within.</param>
        /// <param name="periodEnd">The end of the period to enlist the projects within.</param>
        /// <param name="salespersonId">Determines an ID of the salesperson to filter the list for.</param>
        /// <param name="practiceManagerId">Determines an ID of the practice manager to filter the list for.</param>
        /// <param name="practiceId"></param>
        /// <param name="projectGroupId"></param>
        /// <returns>The list of the projects are match with the specified conditions.</returns>
        public static List<Project> ProjectListAll(int? clientId,
                                                   bool showProjected,
                                                   bool showCompleted,
                                                   bool showActive,
                                                   bool showExperimental,
                                                   DateTime periodStart,
                                                   DateTime periodEnd,
                                                   int? salespersonId,
                                                   int? practiceManagerId,
                                                   int? practiceId,
                                                   int? projectGroupId)
        {
            return ProjectListAll(clientId, showProjected, showCompleted, showActive, showExperimental, periodStart, periodEnd, salespersonId, practiceManagerId, practiceId, projectGroupId, true);
        }

        /// <summary>
        /// Enlists the requested projects.
        /// </summary>
        /// <param name="clientId">An ID of the client the projects belong to.</param>
        /// <param name="showActive">If true - enlist the active project only.</param>
        /// <param name="showProjected">If true - the projected projects will are included in the results.</param>
        /// <param name="showCompleted">If true - the completed projects will are included in the results.</param>
        /// <param name="showExperimental">If true - the experimantal projects will are included in the results.</param>
        /// <param name="periodStart">The start of the period to enlist the projects within.</param>
        /// <param name="periodEnd">The end of the period to enlist the projects within.</param>
        /// <param name="salespersonId">Determines an ID of the salesperson to filter the list for.</param>
        /// <param name="practiceManagerId">Determines an ID of the practice manager to filter the list for.</param>
        /// <param name="practiceId"></param>
        /// <param name="projectGroupId"></param>
        /// <param name="readProjectGroups"></param>
        /// <returns>The list of the projects are match with the specified conditions.</returns>
        public static List<Project> ProjectListAll(int? clientId,
            bool showProjected,
            bool showCompleted,
            bool showActive,
            bool showExperimental,
            DateTime periodStart,
            DateTime periodEnd,
            int? salespersonId,
            int? practiceManagerId,
            int? practiceId,
            int? projectGroupId,
            bool readProjectGroups)
        {
            var projectList = new List<Project>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.ProjectListAll, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdParam,
                        clientId.HasValue ? (object)clientId.Value : null);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowProjectedParam, showProjected);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowCompletedParam, showCompleted);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowActiveParam, showActive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowExperimentalParam, showExperimental);
                    command.Parameters.AddWithValue(Constants.ParameterNames.SalespersonIdParam,
                        salespersonId.HasValue ? (object)salespersonId.Value : DBNull.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PracticeManagerIdParam,
                        practiceManagerId.HasValue ? (object)practiceManagerId.Value : DBNull.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdParam,
                        practiceId.HasValue ? (object)practiceId.Value : DBNull.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectGroupIdParam,
                        projectGroupId.HasValue ? (object)projectGroupId.Value : DBNull.Value);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    ReadProjects(reader, projectList, readProjectGroups);
                }
            }

            return projectList;
        }

        /// <summary>
        /// Enlists the requested projects.
        /// </summary>
        /// <param name="clientIdsList">Comma separated list of client ids. Null value means all clients.</param>
        /// <param name="showActive">If true - enlist the active project only.</param>
        /// <param name="showProjected">If true - the projected projects will are included in the results.</param>
        /// <param name="showCompleted">If true - the completed projects will are included in the results.</param>
        /// <param name="showExperimental">If true - the experimantal projects will are included in the results.</param>
        /// <param name="periodStart">The start of the period to enlist the projects within.</param>
        /// <param name="periodEnd">The end of the period to enlist the projects within.</param>
        /// <param name="salespersonIdsList"></param>
        /// <param name="practiceManagerIdsList"></param>
        /// <param name="practiceIdsList"></param>
        /// <param name="projectGroupIdsList"></param>
        /// <returns>The list of the projects are match with the specified conditions.</returns>
        public static List<Project> ProjectListAllMultiParameters(
            string clientIdsList,
            bool showProjected,
            bool showCompleted,
            bool showActive,
            bool showInternal,
            bool showExperimental,
            bool showInactive,
            DateTime periodStart,
            DateTime periodEnd,
            string salespersonIdsList,
            string practiceManagerIdsList,
            string practiceIdsList,
            string projectGroupIdsList,
            bool excludeInternalPractices)
        {
            var projectList = new List<Project>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.ProjectListAllMultiParameters, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdsParam, clientIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowProjectedParam, showProjected);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowCompletedParam, showCompleted);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowActiveParam, showActive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowInternalParam, showInternal);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowExperimentalParam, showExperimental);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowInactiveParam, showInactive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.SalespersonIdsParam, salespersonIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectOwnerIdsParam, practiceManagerIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectGroupIdsParam, projectGroupIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, periodStart);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, periodEnd);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ExcludeInternalPractices, excludeInternalPractices);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    ReadProjects(reader, projectList, false);//If readGroups parameter value set to true then you need to add InUse with value 1 as Parameter in stored procedure.
                }
            }

            return projectList;
        }

        public static List<Project> GetProjectListWithFinancials(
            string clientIdsList,
            bool showProjected,
            bool showCompleted,
            bool showActive,
            bool showInternal,
            bool showExperimental,
            bool showInactive,
            DateTime periodStart,
            DateTime periodEnd,
            string salespersonIdsList,
            string practiceManagerIdsList,
            string practiceIdsList,
            string projectGroupIdsList,
            bool excludeInternalPractices)
        {
            var projectList = new List<Project>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.GetProjectListWithFinancials, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdsParam, clientIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowProjectedParam, showProjected);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowCompletedParam, showCompleted);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowActiveParam, showActive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowInternalParam, showInternal);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowExperimentalParam, showExperimental);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowInactiveParam, showInactive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.SalespersonIdsParam, salespersonIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectOwnerIdsParam, practiceManagerIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectGroupIdsParam, projectGroupIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, periodStart);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, periodEnd);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ExcludeInternalPractices, excludeInternalPractices);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    ReadProjectsGroupByPerson(reader, projectList);
                }
            }

            return projectList;
        }

        public static List<MilestonePerson> GetProjectListGroupByPracticeManagers(
            string clientIdsList,
            bool showProjected,
            bool showCompleted,
            bool showActive,
            bool showInternal,
            bool showExperimental,
            bool showInactive,
            DateTime periodStart,
            DateTime periodEnd,
            string salespersonIdsList,
            string practiceManagerIdsList,
            string practiceIdsList,
            string projectGroupIdsList,
            bool excludeInternalPractices)
        {
            var MilestonePersons = new List<MilestonePerson>();
            var projectList = new List<Project>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.GetProjectListForGroupingPracticeManagers, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdsParam, clientIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowProjectedParam, showProjected);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowCompletedParam, showCompleted);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowActiveParam, showActive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowInternalParam, showInternal);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowExperimentalParam, showExperimental);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowInactiveParam, showInactive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.SalespersonIdsParam, salespersonIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectOwnerIdsParam, practiceManagerIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectGroupIdsParam, projectGroupIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, periodStart);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, periodEnd);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ExcludeInternalPractices, excludeInternalPractices);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    MilestonePersons = ReadProjectsGroupByPracticeManager(reader, projectList);
                }
            }
            return MilestonePersons;
        }

        private static List<MilestonePerson> ReadProjectsGroupByPracticeManager(SqlDataReader reader, List<Project> projectList)
        {
            var milestonePersons = new List<MilestonePerson>();
            ReadPracjectBasicInfoForGroupingPerson(reader, projectList);
            reader.NextResult();
            var practiceManagers = new List<PracticeManagerHistory>();
            if (reader.HasRows)
            {
                int practiceIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                int PracticeManagerIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeManagerIdColumn);
                int PracticeManagerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeManagerLastNameColumn);
                int PracticeManagerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeManagerFirstNameColumn);
                while (reader.Read())
                {
                    var practiceManagerHistory = new PracticeManagerHistory
                    {
                        PracticeId = reader.GetInt32(practiceIdIndex),
                        PracticeManagerId = reader.GetInt32(PracticeManagerIdIndex),
                        PracticeManagerLastName = reader.GetString(PracticeManagerLastNameIndex),
                        PracticeManagerFirstName = reader.GetString(PracticeManagerFirstNameIndex)
                    };
                    practiceManagers.Add(practiceManagerHistory);
                }
            }
            reader.NextResult();
            if (reader.HasRows) // Read Projects
            {
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                int PersonIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int MilestonePersonIdIndex = reader.GetOrdinal(Constants.ColumnNames.MilestonePersonId);
                int practiceIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                int practiceNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeNameColumn);
                int PracticeManagerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeManagerLastNameColumn);
                int PracticeManagerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeManagerFirstNameColumn);
                int PracticeManagerIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeManagerIdColumn);
                int MilstoneIdIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneIdColumn);
                int MilstoneNameIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneName);
                int MilstonePersonFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.MilestonePersonFirstName);
                int MilstonePersonLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.MilestonePersonLastName);
                int MonthStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.MonthStartDate);
                int MonthEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.MonthEndDate);
                int revenueIndex = reader.GetOrdinal(Constants.ColumnNames.RevenueColumn);
                int grossMarginIndex = reader.GetOrdinal(Constants.ColumnNames.GrossMarginColumn);
                while (reader.Read())
                {
                    var milestonepersonId = reader.GetInt32(MilestonePersonIdIndex);
                    if (reader.IsDBNull(practiceIdIndex))
                        continue;
                    var practiceId = reader.GetInt32(practiceIdIndex);
                    if (milestonepersonId == 4385 && practiceId == 22)
                    {
                    }
                    MilestonePerson milestonePerson = milestonePersons.Find((mp => mp.Id.HasValue && mp.Id.Value == milestonepersonId));

                    if (milestonePerson == null)
                    {
                        milestonePerson = new MilestonePerson
                        {
                            Id = milestonepersonId,
                            Milestone = new Milestone
                            {
                                Id = reader.GetInt32(MilstoneIdIndex),
                                Description = reader.GetString(MilstoneNameIndex),
                                Project = projectList.Find(p => p.Id != null && p.Id.Value == reader.GetInt32(projectIdIndex))
                            },
                            Person = new Person
                            {
                                Id = reader.GetInt32(PersonIdIndex),
                                FirstName = reader.GetString(MilstonePersonFirstNameIndex),
                                LastName = reader.GetString(MilstonePersonLastNameIndex)
                            },
                            PracticeList = new List<Practice>()
                        };
                        milestonePersons.Add(milestonePerson);
                    }

                    Practice practice = milestonePerson.PracticeList.Find(p => p.Id == practiceId);
                    if (practice == null)
                    {
                        practice = new Practice
                        {
                            Id = reader.GetInt32(practiceIdIndex),
                            Name = reader.GetString(practiceNameIndex),
                            PracticeOwner = new Person
                            {
                                Id = reader.GetInt32(PracticeManagerIdIndex),
                                FirstName = reader.GetString(PracticeManagerFirstNameIndex),
                                LastName = reader.GetString(PracticeManagerLastNameIndex)
                            },
                            ProjectedFinancialsByMonth = new Dictionary<DateTime, ComputedFinancials>(),
                            PracticeManagers = practiceManagers.FindAll(pms => pms.PracticeId == reader.GetInt32(practiceIdIndex)
                                                                        && pms.PracticeManagerId != reader.GetInt32(PracticeManagerIdIndex))
                        };

                        milestonePerson.PracticeList.Add(practice);
                    }

                    var financials = new ComputedFinancials
                    {
                        FinancialDate = reader.GetDateTime(MonthStartDateIndex),
                        Revenue = reader.GetDecimal(revenueIndex),
                        GrossMargin = reader.GetDecimal(grossMarginIndex),
                        SalesCommission = 0M,
                        PracticeManagementCommission = 0M,
                        Expenses = 0M,
                        ReimbursedExpenses = 0M
                    };
                    practice.ProjectedFinancialsByMonth.Add(financials.FinancialDate.Value, financials);
                }
            }



            //foreach (var practiceId in practiceManagers.Select(p => p.PracticeId).Distinct())
            //{
            //    foreach
            //}
            return milestonePersons;
        }

        private static void ReadPracjectBasicInfoForGroupingPerson(SqlDataReader reader, List<Project> projectList)
        {
            if (reader.HasRows) // Read Projects
            {
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.NameColumn);
                int practiceIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                int practiceNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeNameColumn);
                int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDateColumn);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDateColumn);
                int projectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusIdColumn);
                int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);

                while (reader.Read())
                {
                    var project = new Project
                    {
                        Id = reader.GetInt32(projectIdIndex),
                        Name = reader.GetString(nameIndex),
                        StartDate =
                            !reader.IsDBNull(startDateIndex) ? (DateTime?)reader.GetDateTime(startDateIndex) : null,
                        EndDate =
                            !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null,
                        ProjectNumber = reader.GetString(projectNumberIndex),
                        Practice = new Practice
                        {
                            Id = reader.GetInt32(practiceIdIndex),
                            Name = reader.GetString(practiceNameIndex)
                        }
                    };

                    project.Client = new Client
                    {
                        Id = reader.GetInt32(clientIdIndex),
                        Name = reader.GetString(clientNameIndex)
                    };

                    project.Status = new ProjectStatus
                    {
                        Id = reader.GetInt32(projectStatusIdIndex),
                        Name = reader.GetString(projectStatusNameIndex)
                    };

                    int directorIdIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorIdColumn),
                     directorLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorLastNameColumn),
                     directorFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorFirstNameColumn);
                    if (!reader.IsDBNull(directorIdIndex))
                    {
                        project.Director = new Person()
                        {
                            Id = (int?)reader.GetInt32(directorIdIndex),
                            FirstName = reader.GetString(directorFirstNameIndex),
                            LastName = reader.GetString(directorLastNameIndex)
                        };
                    }
                    project.Practice.PracticeOwner = new Person
                    {

                        Id = reader.GetInt32(reader.GetOrdinal(Constants.ColumnNames.PracticeManagerIdColumn)),
                        FirstName = reader.GetString(reader.GetOrdinal(Constants.ColumnNames.PracticeManagerFirstNameColumn)),
                        LastName = reader.GetString(reader.GetOrdinal(Constants.ColumnNames.PracticeManagerLastNameColumn))
                    };

                    project.Group = new ProjectGroup
                    {
                        Id = reader.GetInt32(reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn)),
                        Name = reader.GetString(reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn))
                    };
                    try
                    {
                        int SalespersonIdColumnIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonIdColumn);
                        int SalespersonFirstNameColumnIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonFirstNameColumn);
                        int SalespersonLastNameColumnIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonLastNameColumn);
                        int CommissionTypeColumnIndex = reader.GetOrdinal(Constants.ColumnNames.CommissionTypeColumn);
                        var commission = new Commission
                        {
                            TypeOfCommission = (CommissionType)reader.GetInt32(CommissionTypeColumnIndex),
                            PersonId = reader.GetInt32(SalespersonIdColumnIndex),
                            PersonFirstName = reader.GetString(SalespersonFirstNameColumnIndex),
                            PersonLastName = reader.GetString(SalespersonLastNameColumnIndex)
                        };
                        project.SalesCommission = new List<Commission>();
                        project.SalesCommission.Add(commission);
                    }
                    catch
                    {
                    }
                    projectList.Add(project);
                }
            }
        }

        private static void ReadProjectsGroupByPerson(SqlDataReader reader, List<Project> projectList)
        {
            ReadPracjectBasicInfoForGroupingPerson(reader, projectList);
            reader.NextResult();
            // Read Monthly financials

            projectList.ForEach(delegate(Project project)
            {
                if (project.ProjectedFinancialsByMonth == null)
                    project.ProjectedFinancialsByMonth =
                        new Dictionary<DateTime, ComputedFinancials>();
            });
            if (reader.HasRows)
            {
                int financialDateIndex = reader.GetOrdinal(Constants.ColumnNames.FinancialDateColumn);
                int revenueIndex = reader.GetOrdinal(Constants.ColumnNames.RevenueColumn);
                int grossMarginIndex = reader.GetOrdinal(Constants.ColumnNames.GrossMarginColumn);
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                while (reader.Read())
                {
                    var project = new Project { Id = reader.GetInt32(projectIdIndex) };
                    var financials =

                        new ComputedFinancials
                   {
                       FinancialDate = reader.GetDateTime(financialDateIndex),
                       Revenue = reader.GetDecimal(revenueIndex),
                       GrossMargin = reader.GetDecimal(grossMarginIndex),
                       SalesCommission = 0M,
                       PracticeManagementCommission = 0M,
                       Expenses = 0M,
                       ReimbursedExpenses = 0M
                   };
                    var i = projectList.IndexOf(project);
                    projectList[i].ProjectedFinancialsByMonth.Add(financials.FinancialDate.Value, financials);
                }
            }
        }

        /// <summary>
        /// Retrives a list of the projects by the specified conditions.
        /// </summary>
        /// <param name="looked">A text to be looked for.</param>
        /// <param name="personId"></param>
        /// <returns>A list of the <see cref="Project"/> objects.</returns>
        public static List<Project> ProjectSearchText(string looked, int personId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Project.ProjectSearchText, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.LookedParam,
                    !string.IsNullOrEmpty(looked) ? (object)looked : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<Project>();
                    ReadProjectSearchList(reader, result);
                    return result;
                }
            }
        }

        /// <summary>
        /// Reatrives a project with a specified ID.
        /// </summary>
        /// <param name="projectId">The ID of the requested project.</param>
        /// <param name="salespersonId">Determines an ID of the salesperson to filter the list for.</param>
        /// <param name="practiceManagerId">Determines an ID of the practice manager to filter the list for.</param>
        /// <returns>The <see cref="Project"/> record if found and null otherwise.</returns>
        public static Project GetById(int projectId, int? salespersonId, int? practiceManagerId)
        {
            List<Project> projectList = new List<Project>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.ProjectGetById, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.SalespersonIdParam,
                        salespersonId.HasValue ? (object)salespersonId.Value : DBNull.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PracticeManagerIdParam,
                        practiceManagerId.HasValue ? (object)practiceManagerId.Value : DBNull.Value);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    ReadProjects(reader, projectList);
                }
            }

            return projectList.Count > 0 ? projectList[0] : null;
        }

        /// <summary>
        /// Inserts the <see cref="Project"/> record into the3 database.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> to be inserted.</param>
        /// <param name="userName">A current user.</param>
        public static void InsertProject(Project project, string userName, SqlConnection connection, SqlTransaction currentTransaction)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.ProjectInsert, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdParam,
                    project.Client != null && project.Client.Id.HasValue ?
                    (object)project.Client.Id.Value : null);
                command.Parameters.AddWithValue(Constants.ParameterNames.GroupIdParam,
                    project.Group != null && project.Group.Id.HasValue ?
                    (object)project.Group.Id.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.DiscountParam, project.Discount);
                command.Parameters.AddWithValue(Constants.ParameterNames.TermsParam, project.Terms);
                command.Parameters.AddWithValue(Constants.ParameterNames.NameParam, project.Name);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsChargeable, project.IsChargeable);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectManagerId, project.ProjectManager.Id.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdParam,
                    project.Practice != null ? (object)project.Practice.Id : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectStatusIdParam,
                    project.Status != null ? (object)project.Status.Id : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.BuyerNameParam,
                    !string.IsNullOrEmpty(project.BuyerName) ? (object)project.BuyerName : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam,
                    !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);

                command.Parameters.AddWithValue(Constants.ParameterNames.DirecterIdParam,
                    project.Director != null && project.Director.Id.HasValue ? (object)project.Director.Id.Value : DBNull.Value);

                SqlParameter projectIdParam = new SqlParameter(Constants.ParameterNames.ProjectIdParam, SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(projectIdParam);

                if (currentTransaction != null)
                {
                    command.Transaction = currentTransaction;
                }

                command.ExecuteNonQuery();

                project.Id = (int)projectIdParam.Value;
            }
        }

        /// <summary>
        /// Updates the <see cref="Project"/> record in the database.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> to be updated.</param>
        /// <param name="userName">A current user.</param>
        public static void UpdateProject(Project project, string userName, SqlConnection connection, SqlTransaction currentTransaction)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }

            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.ProjectUpdate, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdParam,
                    project.Client != null && project.Client.Id.HasValue ?
                    (object)project.Client.Id.Value : null);
                command.Parameters.AddWithValue(Constants.ParameterNames.GroupIdParam,
                    project.Group != null && project.Group.Id.HasValue ?
                    (object)project.Group.Id.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.DiscountParam, project.Discount);
                command.Parameters.AddWithValue(Constants.ParameterNames.TermsParam, project.Terms);
                command.Parameters.AddWithValue(Constants.ParameterNames.NameParam, project.Name);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectManagerId, project.ProjectManager.Id.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdParam,
                    project.Practice != null ? (object)project.Practice.Id : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, project.Id.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectStatusIdParam,
                    project.Status != null ? (object)project.Status.Id : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.BuyerNameParam,
                    !string.IsNullOrEmpty(project.BuyerName) ? (object)project.BuyerName : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam,
                    !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsChargeable, project.IsChargeable);

                command.Parameters.AddWithValue(Constants.ParameterNames.DirecterIdParam,
                   project.Director != null && project.Director.Id.HasValue ? (object)project.Director.Id.Value : DBNull.Value);

                if (currentTransaction != null)
                {
                    command.Transaction = currentTransaction;
                }

                command.ExecuteNonQuery();

            }
        }

        /// <summary>
        /// Marks a <see cref="Project"/> inactive.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> to be inactivated.</param>
        public static void ProjectInactivate(Project project)
        {
            ProjectStatus status = new ProjectStatus { Id = (int)ProjectStatusType.Inactive };
            project.Status = status;
            ProjectSetStatus(project);
        }

        /// <summary>
        /// Marks a <see cref="Project"/> active.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> to be activated.</param>
        public static void ProjectReactivate(Project project)
        {
            var status = new ProjectStatus { Id = (int)ProjectStatusType.Active };
            project.Status = status;
            ProjectSetStatus(project);
        }

        /// <summary>
        /// Set status of Project
        /// </summary>
        /// <param name="project"><see cref="Project"/> to delete</param>
        public static void ProjectSetStatus(Project project)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.ProjectSetStatus, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, project.Id.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectStatusIdParam, project.Status.Id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Reads the records from the SqlDataReader object and places the data into the specified collection.
        /// </summary>
        /// <param name="reader">The SqlDataReader object to the data be read from.</param>
        /// <param name="resultList">The collection to the data be placed to.</param>
        private static void ReadProjects(SqlDataReader reader, List<Project> resultList)
        {
            ReadProjects(reader, resultList, true);
        }

        /// <summary>
        /// Reads the records from the SqlDataReader object and places the data into the specified collection.
        /// </summary>
        /// <param name="reader">The SqlDataReader object to the data be read from.</param>
        /// <param name="resultList">The collection to the data be placed to.</param>
        /// <param name="readGroups">Whether to read info about project group</param>
        private static void ReadProjects(SqlDataReader reader, List<Project> resultList, bool readGroups)
        {
            try
            {
                if (reader.HasRows)
                {
                    int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                    int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
                    int discountIndex = reader.GetOrdinal(Constants.ColumnNames.DiscountColumn);
                    int termsIndex = reader.GetOrdinal(Constants.ColumnNames.TermsColumn);
                    int nameIndex = reader.GetOrdinal(Constants.ColumnNames.NameColumn);
                    int practiceIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                    int practiceNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeNameColumn);
                    int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
                    int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDateColumn);
                    int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDateColumn);
                    int projectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusIdColumn);
                    int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
                    int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
                    int buyerNameIndex = reader.GetOrdinal(Constants.ColumnNames.BuyerNameColumn);
                    int opportunityId = reader.GetOrdinal(Constants.ColumnNames.OpportunityIdColumn);
                    int projectIsChargeableIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIsChargeable);
                    int clientIsChargeableIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIsChargeable);
                    int pmIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerId);
                    int pmFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerFirstName);
                    int pmLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagerLastName);
                    int salesPersonNameIndex = -1;
                    int practiceOwnerNameIndex = -1;
                    int projectGroupIdIndex = -1;
                    int projectGroupNameIndex = -1;
                    int groupInUseIndex = -1;
                    int attachmentFileNameIndex = -1;
                    try
                    {
                        projectGroupIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn);
                        projectGroupNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn);
                        groupInUseIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupInUseColumn);

                    }
                    catch
                    {
                        projectGroupIdIndex = -1;
                        projectGroupNameIndex = -1;
                        groupInUseIndex = -1;
                    }
                    try
                    {
                        practiceOwnerNameIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeOwnerName);
                    }
                    catch
                    {
                        practiceOwnerNameIndex = -1;
                    }
                    try
                    {
                        salesPersonNameIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonFullNameColumn);
                    }
                    catch
                    {
                        salesPersonNameIndex = -1;
                    }

                    try
                    {
                        attachmentFileNameIndex = reader.GetOrdinal(Constants.ColumnNames.FileName);
                    }
                    catch
                    {
                        attachmentFileNameIndex = -1;
                    }

                    while (reader.Read())
                    {
                        var project = new Project
                                          {
                                              Id = reader.GetInt32(projectIdIndex),
                                              Discount = reader.GetDecimal(discountIndex),
                                              Terms = reader.GetInt32(termsIndex),
                                              Name = reader.GetString(nameIndex),
                                              StartDate =
                                                  !reader.IsDBNull(startDateIndex) ? (DateTime?)reader.GetDateTime(startDateIndex) : null,
                                              EndDate =
                                                  !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null,
                                              ProjectNumber = reader.GetString(projectNumberIndex),
                                              BuyerName = !reader.IsDBNull(buyerNameIndex) ? reader.GetString(buyerNameIndex) : null,
                                              IsChargeable = reader.GetBoolean(projectIsChargeableIndex),
                                              Practice = new Practice
                                                             {
                                                                 Id = reader.GetInt32(practiceIdIndex),
                                                                 Name = reader.GetString(practiceNameIndex)
                                                             },
                                              ProjectManager = new Person
                                                                   {
                                                                       Id = reader.GetInt32(pmIdIndex),
                                                                       FirstName = reader.GetString(pmFirstNameIndex),
                                                                       LastName = reader.GetString(pmLastNameIndex)
                                                                   }
                                          };

                        if (practiceOwnerNameIndex >= 0)
                        {
                            try
                            {
                                project.Practice.PracticeOwnerName = reader.GetString(practiceOwnerNameIndex);
                            }
                            catch
                            {
                                project.Practice.PracticeOwnerName = string.Empty;
                            }
                        }

                        if (salesPersonNameIndex >= 0)
                        {
                            try
                            {
                                project.SalesPersonName = reader.GetString(salesPersonNameIndex);
                            }
                            catch
                            {
                                project.SalesPersonName = string.Empty;
                            }
                        }
                        if (attachmentFileNameIndex >= 0)
                        {
                            try
                            {
                                project.Attachment = new ProjectAttachment { AttachmentFileName = reader.GetString(attachmentFileNameIndex) };
                            }
                            catch
                            {
                            }
                        }

                        project.Client = new Client
                                             {
                                                 Id = reader.GetInt32(clientIdIndex),
                                                 Name = reader.GetString(clientNameIndex),
                                                 IsChargeable = reader.GetBoolean(clientIsChargeableIndex)
                                             };

                        project.Status = new ProjectStatus
                                             {
                                                 Id = reader.GetInt32(projectStatusIdIndex),
                                                 Name = reader.GetString(projectStatusNameIndex)
                                             };

                        project.OpportunityId =
                        !reader.IsDBNull(opportunityId) ? (int?)reader.GetInt32(opportunityId) : null;

                        if (readGroups)
                        {
                            if (projectGroupIdIndex >= 0)
                            {
                                try
                                {
                                    var group = new ProjectGroup
                                       {
                                           Id = (int)reader[projectGroupIdIndex],
                                           Name = (string)reader[projectGroupNameIndex],
                                           InUse = (int)reader[groupInUseIndex] == 1
                                       };

                                    project.Group = group;
                                }
                                catch
                                {
                                }
                            }
                        }
                        try
                        {
                            int directorIdIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorIdColumn),
                             directorLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorLastNameColumn),
                             directorFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.DirectorFirstNameColumn);
                            if (!reader.IsDBNull(directorIdIndex))
                            {
                                project.Director = new Person()
                                                        {
                                                            Id = (int?)reader.GetInt32(directorIdIndex),
                                                            FirstName = reader.GetString(directorFirstNameIndex),
                                                            LastName = reader.GetString(directorLastNameIndex)
                                                        };
                            }
                        }
                        catch
                        {
                        }
                        resultList.Add(project);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void ReadProjectSearchList(DbDataReader reader, List<Project> result)
        {
            if (reader.HasRows)
            {
                int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                int milestoneIdIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneIdColumn);
                int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNameColumn);
                int descriptionIndex = reader.GetOrdinal(Constants.ColumnNames.DescriptionColumn);
                int projectStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStartDateColumn);
                int projectEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectEndDateColumn);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
                int projectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusIdColumn);
                int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
                int projectGroupIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn);

                while (reader.Read())
                {
                    Project project = new Project();

                    project.Id = reader.GetInt32(projectIdIndex);
                    project.Name = reader.GetString(projectNameIndex);
                    project.StartDate =
                        !reader.IsDBNull(projectStartDateIndex) ?
                        (DateTime?)reader.GetDateTime(projectStartDateIndex) : null;
                    project.EndDate =
                        !reader.IsDBNull(projectEndDateIndex) ?
                        (DateTime?)reader.GetDateTime(projectEndDateIndex) : null;
                    project.ProjectNumber = reader.GetString(projectNumberIndex);

                    project.Client =
                        new Client
                            {
                                Id = reader.GetInt32(clientIdIndex),
                                Name = reader.GetString(clientNameIndex)
                            };

                    project.Status =
                        new ProjectStatus
                            {
                                Id = reader.GetInt32(projectStatusIdIndex),
                                Name = reader.GetString(projectStatusNameIndex)
                            };

                    if (!reader.IsDBNull(projectGroupIdIndex))
                    {
                        var groups = ProjectGroupDAL.GroupListAll(null, project.Id);
                        if (groups.Count() > 0)
                            project.Group = groups[0];
                    }

                    if (!reader.IsDBNull(milestoneIdIndex))
                    {
                        project.Milestones = new List<Milestone>();

                        Milestone milestone = new Milestone();

                        milestone.Id = reader.GetInt32(milestoneIdIndex);
                        milestone.Description = reader.GetString(descriptionIndex);

                        project.Milestones.Add(milestone);
                    }

                    result.Add(project);
                }
            }
        }

        public static int CloneProject(ProjectCloningContext context)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.CloneProject, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, context.Project.Id.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectStatusId, context.ProjectStatus.Id);
                    command.Parameters.AddWithValue(Constants.ParameterNames.CloneMilestones, context.CloneMilestones);
                    command.Parameters.AddWithValue(Constants.ParameterNames.CloneBillingInfo, context.CloneBillingInfo);
                    command.Parameters.AddWithValue(Constants.ParameterNames.CloneNotes, context.CloneNotes);
                    command.Parameters.AddWithValue(Constants.ParameterNames.CloneCommissions, context.CloneCommissions);

                    var clonedProjectId = new SqlParameter(Constants.ParameterNames.ClonedProjectId, SqlDbType.Int) { Direction = ParameterDirection.Output };
                    command.Parameters.Add(clonedProjectId);

                    connection.Open();
                    command.ExecuteNonQuery();

                    return (int)clonedProjectId.Value;
                }
            }
        }

        public static int? GetProjectId(string projectNumber)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.ProjectGetByNumber, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ColumnNames.ProjectNumberColumn, projectNumber);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader != null && reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            SqlInt32 projectId = reader.GetSqlInt32(0);
                            if (!projectId.IsNull)
                            {
                                return projectId.Value;
                            }
                        }
                    }

                    return null;
                }
            }
        }

        public static DataSet GetProjectMilestonesFinancials(int projectId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Project.ProjectMilestonesFinancials, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, projectId);
                connection.Open();

                var adapter = new SqlDataAdapter(command);
                var dataset = new DataSet();
                adapter.Fill(dataset, "ProjectMilestonesFinancials");

                return dataset;
            }
        }

        public static void CategoryItemBudgetSave(int itemId, BudgetCategoryType categoryType, DateTime monthStartDate, PracticeManagementCurrency amount)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(
                Constants.ProcedureNames.Project.CategoryItemBudgetSave, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ItemIdParam, itemId);
                command.Parameters.AddWithValue(Constants.ParameterNames.CategoryTypeIdParam, categoryType);
                command.Parameters.AddWithValue(Constants.ParameterNames.MonthStartDateParam, monthStartDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.AmountParam, amount.Value);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static List<ProjectsGroupedByPerson> PersonBudgetListByYear(int year, BudgetCategoryType categoryType)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(
                Constants.ProcedureNames.Project.CategoryItemListByCategoryType, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.CategoryTypeIdParam, categoryType);
                command.Parameters.AddWithValue(Constants.ParameterNames.YearParam, year);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<ProjectsGroupedByPerson>();
                    ReadPersonBudget(reader, result);
                    return result;
                }
            }
        }

        public static List<ProjectsGroupedByPerson> CalculateBudgetForPersons
            (DateTime startDate,
             DateTime endDate,
            bool showProjected,
            bool showCompleted,
            bool showActive,
            bool showInternal,
            bool showExperimental,
            bool showInactive,
            string practiceIdsList,
            bool excludeInternalPractices,
            string personIds,
            BudgetCategoryType categoryType)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(
                Constants.ProcedureNames.Project.CalculateBudgetForCategoryItems, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowProjectedParam, showProjected);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowCompletedParam, showCompleted);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowActiveParam, showActive);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowInternalParam, showInternal);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowExperimentalParam, showExperimental);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowInactiveParam, showInactive);

                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIdsList);

                command.Parameters.AddWithValue(Constants.ParameterNames.ItemIdsParam, personIds);

                command.Parameters.AddWithValue(Constants.ParameterNames.ExcludeInternalPractices, excludeInternalPractices);
                command.Parameters.AddWithValue(Constants.ParameterNames.CategoryTypeIdParam, categoryType);


                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<ProjectsGroupedByPerson>();
                    ReadCalculatedPersonBudget(reader, result);
                    return result;
                }
            }
        }

        public static List<ProjectsGroupedByPractice> CalculateBudgetForPractices
            (DateTime startDate,
             DateTime endDate,
            bool showProjected,
            bool showCompleted,
            bool showActive,
            bool showInternal,
            bool showExperimental,
            bool showInactive,
            string practiceIdsList,
            bool excludeInternalPractices)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(
                Constants.ProcedureNames.Project.CalculateBudgetForCategoryItems, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowProjectedParam, showProjected);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowCompletedParam, showCompleted);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowActiveParam, showActive);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowInternalParam, showInternal);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowExperimentalParam, showExperimental);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowInactiveParam, showInactive);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIdsList);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExcludeInternalPractices, excludeInternalPractices);
                command.Parameters.AddWithValue(Constants.ParameterNames.CategoryTypeIdParam, BudgetCategoryType.PracticeArea);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<ProjectsGroupedByPractice>();
                    ReadCalculatedPracticeBudget(reader, result);
                    return result;
                }
            }
        }

        private static void ReadCalculatedPersonBudget(SqlDataReader reader, List<ProjectsGroupedByPerson> result)
        {
            if (reader.HasRows)
            {
                int PersonIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int LastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int FirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int MonthStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.MonthStartDate);
                int AmountIndex = reader.GetOrdinal(Constants.ColumnNames.Amount);
                int RevenueIndex = reader.GetOrdinal(Constants.ColumnNames.RevenueColumn);
                while (reader.Read())
                {
                    ProjectsGroupedByPerson person;
                    var personId = reader.GetInt32(PersonIdIndex);
                    if (!result.Any(p => p.PersonId == personId))
                    {
                        person = new ProjectsGroupedByPerson()
                        {
                            PersonId = personId,
                            LastName = reader.GetString(LastNameIndex),
                            FirstName = reader.GetString(FirstNameIndex),
                            ProjectedFinancialsByMonth = new Dictionary<DateTime, ComputedFinancials>()
                        };

                        result.Add(person);
                    }
                    else
                    {
                        person = result.Find(p => p.PersonId == personId);
                    }
                    if (!reader.IsDBNull(MonthStartDateIndex))
                    {
                        var financials = new ComputedFinancials();
                        if (!reader.IsDBNull(AmountIndex))
                        {
                            financials.Revenue = reader.GetDecimal(AmountIndex);
                            //This is the Revenue entered through the for budget page.
                        }
                        if (!reader.IsDBNull(RevenueIndex))
                        {
                            financials.RevenueNet = reader.GetDecimal(RevenueIndex);
                            //Actually it is not Revenue Net. It is the Revenue calculated for the person.
                        }
                        person.ProjectedFinancialsByMonth.Add(reader.GetDateTime(MonthStartDateIndex), financials);
                    }
                }
            }
        }

        private static void ReadCalculatedPracticeBudget(SqlDataReader reader, List<ProjectsGroupedByPractice> result)
        {

            if (reader.HasRows)
            {
                int PracticeIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                int NameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
                int MonthStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.MonthStartDate);
                int AmountIndex = reader.GetOrdinal(Constants.ColumnNames.Amount);
                int RevenueIndex = reader.GetOrdinal(Constants.ColumnNames.RevenueColumn);
                while (reader.Read())
                {
                    ProjectsGroupedByPractice practice;
                    var practiceId = reader.GetInt32(PracticeIdIndex);
                    if (!result.Any(p => p.PracticeId == practiceId))
                    {
                        practice = new ProjectsGroupedByPractice()
                        {
                            PracticeId = practiceId,
                            Name = reader.GetString(NameIndex),
                            ProjectedFinancialsByMonth = new Dictionary<DateTime, ComputedFinancials>()
                        };
                        result.Add(practice);
                    }
                    else
                    {
                        practice = result.Find(p => p.PracticeId == practiceId);
                    }
                    if (!reader.IsDBNull(MonthStartDateIndex))
                    {
                        var financials = new ComputedFinancials();
                        if (!reader.IsDBNull(AmountIndex))
                        {
                            financials.Revenue = reader.GetDecimal(AmountIndex);
                            //This is the Revenue entered through the for budget page.
                        }
                        if (!reader.IsDBNull(RevenueIndex))
                        {
                            financials.RevenueNet = reader.GetDecimal(RevenueIndex);
                            //Actually it is not Revenue Net. It is the Revenue calculated for the person.
                        }
                        practice.ProjectedFinancialsByMonth.Add(reader.GetDateTime(MonthStartDateIndex), financials);
                    }
                }
            }
        }

        public static List<ProjectsGroupedByPractice> PracticeBudgetListByYear(int year)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(
                Constants.ProcedureNames.Project.CategoryItemListByCategoryType, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.CategoryTypeIdParam, BudgetCategoryType.PracticeArea);
                command.Parameters.AddWithValue(Constants.ParameterNames.YearParam, year);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var result = new List<ProjectsGroupedByPractice>();
                    ReadPracticeBudget(reader, result);
                    return result;
                }
            }
        }

        private static void ReadPracticeBudget(SqlDataReader reader, List<ProjectsGroupedByPractice> result)
        {
            if (reader.HasRows)
            {
                int PracticeIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
                int NameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
                int MonthStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.MonthStartDate);
                int AmountIndex = reader.GetOrdinal(Constants.ColumnNames.Amount);
                while (reader.Read())
                {
                    ProjectsGroupedByPractice practice;
                    var practiceId = reader.GetInt32(PracticeIdIndex);
                    if (!result.Any(p => p.PracticeId == practiceId))
                    {
                        practice = new ProjectsGroupedByPractice()
                        {
                            PracticeId = practiceId,
                            Name = reader.GetString(NameIndex),
                            ProjectedFinancialsByMonth = new Dictionary<DateTime, ComputedFinancials>()
                        };

                        result.Add(practice);
                    }
                    else
                    {
                        practice = result.Find(p => p.PracticeId == practiceId);
                    }
                    if (!reader.IsDBNull(MonthStartDateIndex))
                        practice.ProjectedFinancialsByMonth.Add(reader.GetDateTime(MonthStartDateIndex),
                             new ComputedFinancials()
                             {
                                 Revenue = reader.IsDBNull(AmountIndex) ? 0M : reader.GetDecimal(AmountIndex)
                             });
                }
            }
        }

        private static void ReadPersonBudget(SqlDataReader reader, List<ProjectsGroupedByPerson> result)
        {
            if (reader.HasRows)
            {
                int PersonIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int LastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int FirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int MonthStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.MonthStartDate);
                int AmountIndex = reader.GetOrdinal(Constants.ColumnNames.Amount);
                while (reader.Read())
                {
                    ProjectsGroupedByPerson person;
                    var personId = reader.GetInt32(PersonIdIndex);
                    if (!result.Any(p => p.PersonId == personId))
                    {
                        person = new ProjectsGroupedByPerson()
                        {
                            PersonId = personId,
                            LastName = reader.GetString(LastNameIndex),
                            FirstName = reader.GetString(FirstNameIndex),

                            //TerminationDate = reader.IsDBNull(TerminationDateIndex) ?
                            //                                     null : (DateTime?)reader.GetDateTime(TerminationDateIndex),
                            ProjectedFinancialsByMonth = new Dictionary<DateTime, ComputedFinancials>()
                        };

                        result.Add(person);
                    }
                    else
                    {
                        person = result.Find(p => p.PersonId == personId);
                    }
                    if (!reader.IsDBNull(MonthStartDateIndex))
                        person.ProjectedFinancialsByMonth.Add(reader.GetDateTime(MonthStartDateIndex),
                             new ComputedFinancials()
                             {
                                 Revenue = reader.IsDBNull(AmountIndex) ? 0M : reader.GetDecimal(AmountIndex)
                             });
                }
            }
        }

        public static void CategoryItemsSaveFromXML(List<CategoryItemBudget> categoryItems, int year)
        {
            GetCategoryItemNodes(categoryItems, year);
        }

        private static void CategoryItemsExecution(string xml, int year)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Project.CategoryItemsSaveFromXML, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.CategoryItemsXMLParam, xml);
                command.Parameters.AddWithValue(Constants.ParameterNames.YearParam, year);
                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        private static void GetCategoryItemNodes(List<CategoryItemBudget> categoryItems, int year)
        {
            var categoryIdList = categoryItems.Select(i => i.CategoryTypeId).Distinct();
            if (categoryIdList.ToList().Count > 0)
            {
                foreach (var categoryId in categoryIdList)
                {
                    string rootXml = "<Root>";

                    List<CategoryItemBudget> categoryList = categoryItems.FindAll(i => i.CategoryTypeId == categoryId);

                    string categoryNode = "<Category Id='" + ((int)categoryId) + "'>";

                    categoryNode = CreateItemNodes(categoryList, categoryNode) + "</Category>";

                    rootXml = rootXml + categoryNode + "</Root>";

                    CategoryItemsExecution(rootXml, year);
                }
            }
        }

        private static string CreateItemNodes(List<CategoryItemBudget> categoryList, string categoryNode)
        {
            var itemIdList = categoryList.Select(i => i.ItemId).Distinct();

            foreach (var itemId in itemIdList)
            {
                List<CategoryItemBudget> itemList = categoryList.FindAll(i => i.ItemId == itemId);

                string itemNode = "<Item Id='" + itemId + "'>";

                itemNode = CreateMonthNodes(itemList, itemNode) + "</Item>";

                categoryNode = categoryNode + itemNode;
            }
            return categoryNode;
        }

        private static string CreateMonthNodes(List<CategoryItemBudget> itemList, string itemNode)
        {
            var monthIdList = itemList.Select(i => i.Month).Distinct();

            foreach (var monthId in monthIdList)
            {
                List<CategoryItemBudget> monthList = itemList.FindAll(i => i.Month == monthId);

                string monthNode = "<Month Id='" + monthId + "'>";

                monthNode = CreateAmountNodes(monthList, monthNode) + "</Month>";

                itemNode = itemNode + monthNode;
            }
            return itemNode;
        }

        private static string CreateAmountNodes(List<CategoryItemBudget> monthList, string monthNode)
        {
            foreach (var month in monthList)
            {
                string amountNode = "<Amount>" + month.Amount + "</Amount>";

                monthNode = monthNode + amountNode;
            }
            return monthNode;
        }

        public static void SaveProjectAttachmentData(ProjectAttachment attachment, int projectId)
        {
            
             var connection = new SqlConnection(DataSourceHelper.DataConnection);
           

            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.SaveProjectAttachment, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId);
                command.Parameters.AddWithValue(Constants.ParameterNames.AttachmentFileName, !string.IsNullOrEmpty(attachment.AttachmentFileName) ? (object)attachment.AttachmentFileName : DBNull.Value);
                command.Parameters.Add(Constants.ParameterNames.AttachmentData, SqlDbType.VarBinary, -1);
                command.Parameters[Constants.ParameterNames.AttachmentData].Value = attachment.AttachmentData != null ? (object)attachment.AttachmentData : DBNull.Value;

                connection.Open();
                command.ExecuteNonQuery();

            }
        }

        public static byte[] GetProjectAttachmentData(int projectId)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.GetProjectAttachmentData, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    try
                    {
                        Byte[] AttachmentData = null;

                        if (reader.HasRows)
                        {
                            int AttachmentDataIndex = reader.GetOrdinal(Constants.ColumnNames.AttachmentDataColumn);


                            while (reader.Read())
                            {
                                AttachmentData = (byte[])reader[AttachmentDataIndex];
                            }
                        }

                        return AttachmentData;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }


        }
       
    }
}


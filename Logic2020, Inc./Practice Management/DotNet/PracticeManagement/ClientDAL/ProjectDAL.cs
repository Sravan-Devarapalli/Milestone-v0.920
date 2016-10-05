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
    using DataTransferObjects.TimeEntry;

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
        ///

        public static DateTime GetProjectLastChangeDateFortheGivenStatus(int projectId, int projectStatusId)
        {
            DateTime dateTime = DateTime.MinValue;
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.GetProjectLastChangeDateFortheGivenStatus, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, projectId);
                    command.Parameters.AddWithValue(
                        Constants.ParameterNames.ProjectStatusId, projectStatusId);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        if (reader.HasRows)
                        {
                            int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDate);
                            while (reader.Read())
                            {
                                dateTime = reader.GetDateTime(startDateIndex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            return dateTime;
        }

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

        public static List<Project> ListProjectsByClientShort(int clientId, bool IsOnlyActiveAndProjective, bool IsOnlyActiveAndInternal, bool IsOnlyEnternalProjects)
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
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsOnlyActiveAndInternal, IsOnlyActiveAndInternal);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsOnlyEnternalProjectsParam, IsOnlyEnternalProjects);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    ReadProjectsShort(reader, projectList, false); //If you alter readGroups Parameter to true then you need to add the GroupName parameter,InUse Parameter  in "dbo.ProjectsListByClient"
                }
            }

            return projectList;
        }

        public static List<Project> ListProjectsByClientAndPersonInPeriod(int clientId, bool isOnlyActiveAndInternal, bool isOnlyEnternalProjects, int personId, DateTime startDate, DateTime endDate)
        {
            var projectList = new List<Project>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.ListProjectsByClientAndPersonInPeriod, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdParam, clientId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsOnlyActiveAndInternal, isOnlyActiveAndInternal);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsOnlyEnternalProjectsParam, isOnlyEnternalProjects);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        if (reader.HasRows)
                        {
                            int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                            int nameIndex = reader.GetOrdinal(Constants.ColumnNames.NameColumn);
                            int assignedProjectIndex = reader.GetOrdinal(Constants.ColumnNames.AssignedProject);

                            while (reader.Read())
                            {
                                var project = new Project
                                {
                                    Id = reader.GetInt32(projectIdIndex),
                                    Name = reader.GetString(nameIndex),
                                    IsAssignedProject = reader.GetInt32(assignedProjectIndex) == 1
                                };
                                projectList.Add(project);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return projectList;
        }

        private static void ReadProjectsShort(SqlDataReader reader, List<Project> resultList, bool readGroups)
        {
            try
            {
                if (!reader.HasRows) return;
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.NameColumn);
                int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
                int buyerNameIndex = reader.GetOrdinal(Constants.ColumnNames.BuyerNameColumn);
                int descriptionIndex = -1;

                try
                {
                    descriptionIndex = reader.GetOrdinal(Constants.ColumnNames.DescriptionColumn);
                }
                catch
                {
                    descriptionIndex = -1;
                }

                while (reader.Read())
                {
                    var project = new Project
                    {
                        Id = reader.GetInt32(projectIdIndex),
                        Name = reader.GetString(nameIndex),
                        ProjectNumber = reader.GetString(projectNumberIndex),
                        BuyerName = !reader.IsDBNull(buyerNameIndex) ? reader.GetString(buyerNameIndex) : null
                    };

                    if (descriptionIndex > -1)
                    {
                        project.Description = !reader.IsDBNull(descriptionIndex) ? reader.GetString(descriptionIndex) : string.Empty;
                    }

                    project.Client = new Client
                    {
                        Id = reader.GetInt32(clientIdIndex),
                        Name = reader.GetString(clientNameIndex)
                    };
                    resultList.Add(project);
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
        /// <param name="showActive">If true - enlist the active project only.</param>
        /// <param name="showProjected">If true - the projected projects will are included in the results.</param>
        /// <param name="showCompleted">If true - the completed projects will are included in the results.</param>
        /// <param name="showExperimental">If true - the experimantal projects will are included in the results.</param>
        /// <param name="readProjectGroups"></param>
        /// <returns>The list of the projects are match with the specified conditions.</returns>
        public static List<Project> ProjectListAll(
            bool showProjected,
            bool showCompleted,
            bool showActive,
            bool showExperimental,
            bool showProposed,
            bool readProjectGroups)
        {
            var projectList = new List<Project>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.ProjectListAll, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowProjectedParam, showProjected);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowCompletedParam, showCompleted);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowActiveParam, showActive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowExperimentalParam, showExperimental);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowProposedParam, showProposed);

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
            bool showProposed,
            bool showInactive,
            DateTime periodStart,
            DateTime periodEnd,
            string salespersonIdsList,
            string practiceManagerIdsList,
            string practiceIdsList,
            string divisionIdsList,
            string channelIdsList,
            string revenueTypeIdsList,
            string offeringIdsList,
            string projectGroupIdsList,
            bool excludeInternalPractices,
            string userLogin)
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
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowProposedParam, showProposed);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowInactiveParam, showInactive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.SalespersonIdsParam, salespersonIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectOwnerIdsParam, practiceManagerIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.DivisionIdsParam, divisionIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ChannelIdsParam, channelIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.RevenueTypeIdsParam, revenueTypeIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.OfferingIdsParam, offeringIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectGroupIdsParam, projectGroupIdsList);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, periodStart);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, periodEnd);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ExcludeInternalPractices, excludeInternalPractices);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    ReadProjectsWithMilestones(reader, projectList);
                }
            }

            MilestonePersonDAL.LoadMilestonePersonListForProject(projectList);

            return projectList;
        }

        public static bool IsProjectSummaryCachedToday()
        {
            bool result = false;
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.IsProjectSummaryCachedToday, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    connection.Open();

                    result = (bool)command.ExecuteScalar();
                }
            }
            return result;
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

            MilestonePersonDAL.LoadMilestonePersonListForProject(projectList);

            return projectList;
        }

        public static List<MilestonePerson> GetProjectListGroupByPracticeManagers(
            string clientIdsList,
            bool showProjected,
            bool showCompleted,
            bool showActive,
            bool showInternal,
            bool showExperimental,
            bool showProposed,
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
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowProposedParam, showProposed);
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
            if (!reader.HasRows) return;
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
                    },
                    Client = new Client
                    {
                        Id = reader.GetInt32(clientIdIndex),
                        Name = reader.GetString(clientNameIndex)
                    },
                    Status = new ProjectStatus
                    {
                        Id = reader.GetInt32(projectStatusIdIndex),
                        Name = reader.GetString(projectStatusNameIndex)
                    }
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
                if (!reader.IsDBNull(reader.GetOrdinal(Constants.ColumnNames.PracticeManagerIdColumn)))
                {
                    project.Practice.PracticeOwner = new Person
                    {
                        Id = reader.GetInt32(reader.GetOrdinal(Constants.ColumnNames.PracticeManagerIdColumn)),
                        FirstName = reader.GetString(reader.GetOrdinal(Constants.ColumnNames.PracticeManagerFirstNameColumn)),
                        LastName = reader.GetString(reader.GetOrdinal(Constants.ColumnNames.PracticeManagerLastNameColumn))
                    };
                }
                project.Group = new ProjectGroup
                {
                    Id = reader.GetInt32(reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn)),
                    Name = reader.GetString(reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn))
                };
                try
                {
                    int salespersonIdColumnIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonIdColumn);
                    int salespersonFirstNameColumnIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonFirstNameColumn);
                    int salespersonLastNameColumnIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonLastNameColumn);

                    project.SalesPersonName = reader.GetString(salespersonLastNameColumnIndex) + ", " +
                                              reader.GetString(salespersonFirstNameColumnIndex);
                    project.SalesPersonId = reader.GetInt32(salespersonIdColumnIndex);
                }
                catch
                {
                }
                projectList.Add(project);
            }
            MilestonePersonDAL.LoadMilestonePersonListForProject(projectList);
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
            if (!reader.HasRows) return;
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
                        FinancialDate = reader.IsDBNull(financialDateIndex) ? (DateTime?)null : reader.GetDateTime(financialDateIndex),
                        Revenue = reader.GetDecimal(revenueIndex),
                        GrossMargin = reader.GetDecimal(grossMarginIndex),
                        Expenses = 0M,
                        ReimbursedExpenses = 0M
                    };
                var i = projectList.IndexOf(project);
                projectList[i].ProjectedFinancialsByMonth.Add(financials.FinancialDate.Value, financials);
            }
        }

        /// <summary>
        /// Retrives a list of the projects by the specified conditions.
        /// </summary>
        /// <param name="looked">A text to be looked for.</param>
        /// <param name="personId"></param>
        /// <returns>A list of the <see cref="Project"/> objects.</returns>
        public static List<Project> ProjectSearchText(
            string looked,
            int personId,
            string clientIdsList,
            bool showProjected,
            bool showCompleted,
            bool showActive,
            bool showInternal,
            bool showExperimental,
            bool showProposed,
            bool showInactive,
            string salespersonIdsList,
            string projectManagerIdsList,
            string practiceIdsList,
            string divisionIdsList,
            string channelIdsList,
            string revenueTypeIdsList,
            string offeringIdsList,
            string projectGroupIdsList)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Project.ProjectSearchText, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.LookedParam,
                    !string.IsNullOrEmpty(looked) ? (object)looked : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdsParam, clientIdsList);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowProjectedParam, showProjected);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowCompletedParam, showCompleted);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowActiveParam, showActive);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowInternalParam, showInternal);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowExperimentalParam, showExperimental);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowProposedParam, showProposed);
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowInactiveParam, showInactive);
                command.Parameters.AddWithValue(Constants.ParameterNames.SalespersonIdsParam, salespersonIdsList);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectOwnerIdsParam, projectManagerIdsList);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdsParam, practiceIdsList);
                command.Parameters.AddWithValue(Constants.ParameterNames.DivisionIdsParam, divisionIdsList);
                command.Parameters.AddWithValue(Constants.ParameterNames.ChannelIdsParam, channelIdsList);
                command.Parameters.AddWithValue(Constants.ParameterNames.RevenueTypeIdsParam, revenueTypeIdsList);
                command.Parameters.AddWithValue(Constants.ParameterNames.OfferingIdsParam, offeringIdsList);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectGroupIdsParam, projectGroupIdsList);

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
        /// <returns>The <see cref="Project"/> record if found and null otherwise.</returns>
        public static Project GetById(int projectId)
        {
            List<Project> projectList = new List<Project>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.ProjectGetById, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    ReadProjects(reader, projectList);
                }
            }

            return projectList.Count > 0 ? projectList[0] : null;
        }

        public static void SaveInternalProject(string projectName, string projectNumberSeries, int divisionId, int practiceId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Project.InsertInternalProject, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.NameParam, projectName);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectNumberSeries, projectNumberSeries);
                command.Parameters.AddWithValue(Constants.ParameterNames.DivisionId, divisionId);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdParam, practiceId);

                connection.Open();
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);

                }
            }
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
                command.Parameters.AddWithValue(Constants.ParameterNames.TermsParam, project.Terms);
                command.Parameters.AddWithValue(Constants.ParameterNames.NameParam, project.Name);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsNoteRequiredParam, project.IsNoteRequired);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsClientTimeEntryRequired, project.IsClientTimeEntryRequired);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectManagerIdsList, project.ProjectManagerIdsList);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdParam,
                    project.Practice != null ? (object)project.Practice.Id : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectStatusIdParam,
                    project.Status != null ? (object)project.Status.Id : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.BuyerNameParam,
                    !string.IsNullOrEmpty(project.BuyerName) ? (object)project.BuyerName : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam,
                    !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.DescriptionParam, !string.IsNullOrEmpty(project.Description) ? (object)project.Description : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.DirecterIdParam,
                    project.Director != null && project.Director.Id.HasValue ? (object)project.Director.Id.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.CanCreateCustomWorkTypesParam, project.CanCreateCustomWorkTypes);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsInternalParam, project.IsInternal);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectOwnerIdParam, project.ProjectOwner.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.SowBudgetParam, project.SowBudget.HasValue ? (object)project.SowBudget.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.POAmountParam, project.POAmount.HasValue ? (object)project.POAmount.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectCapabilityIds, !string.IsNullOrEmpty(project.ProjectCapabilityIds) ? project.ProjectCapabilityIds : string.Empty);
                command.Parameters.AddWithValue(Constants.ParameterNames.PONumber, !string.IsNullOrEmpty(project.PONumber) ? (Object)project.PONumber : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.SalespersonIdParam, project.SalesPersonId > 0 ? (object)project.SalesPersonId : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectNumber, !string.IsNullOrEmpty(project.ProjectNumber) ? (object)project.ProjectNumber : DBNull.Value);
                if (project.SeniorManagerId > 0)
                    command.Parameters.AddWithValue(Constants.ParameterNames.SeniorManagerId, project.SeniorManagerId);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsSeniorManagerUnassigned, project.IsSeniorManagerUnassigned);
                if (project.CSATOwnerId > 0)
                    command.Parameters.AddWithValue(Constants.ParameterNames.CSATOwnerId, project.CSATOwnerId);
                command.Parameters.AddWithValue(Constants.ParameterNames.PricingListId, project.PricingList != null && project.PricingList.PricingListId.HasValue ? (object)project.PricingList.PricingListId.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.BusinessTypeId, ((int)project.BusinessType) != 0 ? (object)((int)project.BusinessType) : DBNull.Value);
                SqlParameter projectIdParam = new SqlParameter(Constants.ParameterNames.ProjectIdParam, SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(projectIdParam);
                command.Parameters.AddWithValue(Constants.ParameterNames.DivisionId, project.Division.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.ChannelId, project.Channel.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.RevenueTypeParam, project.RevenueType.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.SubChannel, project.SubChannel);
                command.Parameters.AddWithValue(Constants.ParameterNames.OfferingId, project.Offering.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.PreviousProjectNumber, project.PreviousProject != null ? (object)project.PreviousProject.ProjectNumber : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.OutsourceId, project.OutsourceId);

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

                command.Parameters.AddWithValue(Constants.ParameterNames.PricingListId,
                   project.PricingList != null && project.PricingList.PricingListId.HasValue ?
                   (object)project.PricingList.PricingListId.Value : DBNull.Value);

                command.Parameters.AddWithValue(Constants.ParameterNames.BusinessTypeId,
                ((int)project.BusinessType) != 0 ? (object)((int)project.BusinessType) : DBNull.Value);

                command.Parameters.AddWithValue(Constants.ParameterNames.TermsParam, project.Terms);
                command.Parameters.AddWithValue(Constants.ParameterNames.NameParam, project.Name);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectManagerIdsList, project.ProjectManagerIdsList);
                command.Parameters.AddWithValue(Constants.ParameterNames.PracticeIdParam,
                    project.Practice != null ? (object)project.Practice.Id : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, project.Id.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectStatusIdParam,
                    project.Status != null ? (object)project.Status.Id : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.BuyerNameParam,
                    !string.IsNullOrEmpty(project.BuyerName) ? (object)project.BuyerName : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam,
                    !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.DescriptionParam, !string.IsNullOrEmpty(project.Description) ? (object)project.Description : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsNoteRequiredParam, project.IsNoteRequired);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsClientTimeEntryRequired, project.IsClientTimeEntryRequired);
                command.Parameters.AddWithValue(Constants.ParameterNames.CanCreateCustomWorkTypesParam, project.CanCreateCustomWorkTypes);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsInternalParam, project.IsInternal);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectOwnerIdParam, project.ProjectOwner.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.SowBudgetParam, project.SowBudget.HasValue ? (object)project.SowBudget.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.POAmountParam, project.POAmount.HasValue ? (object)project.POAmount.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectCapabilityIds, !string.IsNullOrEmpty(project.ProjectCapabilityIds) ? project.ProjectCapabilityIds : string.Empty);
                command.Parameters.AddWithValue(Constants.ParameterNames.PONumber, !string.IsNullOrEmpty(project.PONumber) ? (Object)project.PONumber : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.SalespersonIdParam, project.SalesPersonId > 0 ? (object)project.SalesPersonId : DBNull.Value);
                if (project.SeniorManagerId > 0)
                    command.Parameters.AddWithValue(Constants.ParameterNames.SeniorManagerId, project.SeniorManagerId);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsSeniorManagerUnassigned, project.IsSeniorManagerUnassigned);
                if (project.CSATOwnerId > 0)
                    command.Parameters.AddWithValue(Constants.ParameterNames.CSATOwnerId, project.CSATOwnerId);
                command.Parameters.AddWithValue(Constants.ParameterNames.DirecterIdParam,
                   project.Director != null && project.Director.Id.HasValue ? (object)project.Director.Id.Value : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.DivisionId, project.Division.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.ChannelId, project.Channel.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.RevenueTypeParam, project.RevenueType.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.SubChannel, project.SubChannel);
                command.Parameters.AddWithValue(Constants.ParameterNames.OfferingId, project.Offering.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.PreviousProjectNumber, project.PreviousProject != null ? (object)project.PreviousProject.ProjectNumber : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.OutsourceId, project.OutsourceId);

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
        /// <param name="readGroups">Whether to read info about project group</param>
        private static void ReadProjects(SqlDataReader reader, List<Project> resultList, bool readGroups = true)
        {
            try
            {
                if (!reader.HasRows) return;
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
                int isClientTimeEntryRequiredIndex = -1;
                int previousProjectNumberIndex = -1;
                int previousProjectIdIndex = -1;
                int outsourceIdIndex = -1;
                try
                {
                    outsourceIdIndex = reader.GetOrdinal(Constants.ColumnNames.OutsourceId);
                }
                catch { }

                try
                {
                    previousProjectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.PreviousProjectNumber);
                    previousProjectIdIndex = reader.GetOrdinal(Constants.ColumnNames.PreviousProjectId);
                }
                catch { }
                try
                {
                    isClientTimeEntryRequiredIndex = reader.GetOrdinal(Constants.ColumnNames.IsClientTimeEntryRequired);
                }
                catch { }
                int pmIndex = -1;
                try
                {
                    pmIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagersIdFirstNameLastName);
                }
                catch { }

                int ToAddressListIndex = -1;
                int ProjectOwnerAlias = -1;
                int descriptionIndex = -1;
                int salesPersonIdIndex = -1;
                int salesPersonNameIndex = -1;
                int practiceOwnerNameIndex = -1;
                int projectGroupIdIndex = -1;
                int projectGroupNameIndex = -1;
                int groupInUseIndex = -1;
                int isMarginColorInfoEnabledIndex = -1;
                int hasAttachmentsIndex = -1;
                int canCreateCustomWorkTypesIndex = -1;
                int IsInternalIndex = -1;
                int ClientIsInternalIndex = -1;
                int hasTimeEntriesIndex = -1;
                int isNoteRequiredIndex = -1;
                int projectBusinessTypesIndex = -1;
                int projectOwnerIdIndex = -1;
                int projectOwnerFirstNameIndex = -1;
                int projectOwnerLastNameIndex = -1;
                int pricingListIdIndex = -1;
                int pricingListNameIndex = -1;
                int opportunityNumberIndex = -1;
                int sowBudgetIndex = -1;
                int clientIsNoteRequiredIndex = -1;
                int poNumberIndex = -1;
                int isHouseAccountIndex = -1;
                int poAmountIndex = -1;
                int projectCapabilitiesIndex = -1;
                int divisionIdIndex = -1;
                int divisionNameIndex = -1;
                int channelIdIndex = -1;
                int ChannelNameIndex = -1;
                int subchannelIndex = -1;
                int revenueTypeIdIndex = -1;
                int revenueNameIndex = -1;
                int offeringIdIndex = -1;
                int offeringNameIndex = -1;

                try
                {
                    offeringNameIndex = reader.GetOrdinal(Constants.ColumnNames.OfferName);
                }
                catch { }
                try
                {
                    revenueNameIndex = reader.GetOrdinal(Constants.ColumnNames.RevenueName);
                }
                catch { }
                try
                {
                    ChannelNameIndex = reader.GetOrdinal(Constants.ColumnNames.ChannelName);
                }
                catch { }
                try
                {
                    divisionNameIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionName);
                }
                catch
                { }

                try
                {
                    offeringIdIndex = reader.GetOrdinal(Constants.ColumnNames.OfferingId);
                }
                catch
                { }
                try
                {
                    revenueTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.RevenueTypeId);
                }
                catch
                { }
                try
                {
                    subchannelIndex = reader.GetOrdinal(Constants.ColumnNames.SubChannel);
                }
                catch
                { }

                try
                {
                    channelIdIndex = reader.GetOrdinal(Constants.ColumnNames.ChannelId);
                }
                catch
                { }
                try
                {
                    divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);
                }
                catch
                { }
                try
                {
                    projectCapabilitiesIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectCapabilities);
                }
                catch
                { }

                try
                {
                    ProjectOwnerAlias = reader.GetOrdinal(Constants.ColumnNames.ProjectOwnerAlias);
                }
                catch
                { }
                try
                {
                    ToAddressListIndex = reader.GetOrdinal(Constants.ColumnNames.ToAddressList);
                }
                catch
                { }
                try
                {
                    poAmountIndex = reader.GetOrdinal(Constants.ColumnNames.POAmount);
                }
                catch
                { }
                try
                {
                    poNumberIndex = reader.GetOrdinal(Constants.ColumnNames.PONumber);
                }
                catch
                { }

                try
                {
                    isHouseAccountIndex = reader.GetOrdinal(Constants.ColumnNames.IsHouseAccount);
                }
                catch
                { }

                int seniorManagerIdIndex = -1;
                try
                {
                    seniorManagerIdIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorManagerId);
                }
                catch
                { }

                int isSeniorManagerUnassignedIndex = -1;
                try
                {
                    isSeniorManagerUnassignedIndex = reader.GetOrdinal(Constants.ColumnNames.IsSeniorManagerUnassigned);
                }
                catch
                { }
                int seniorManagerNameIndex = -1;
                try
                {
                    seniorManagerNameIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorManagerName);
                }
                catch
                { }
                int reviewerIdIndex = -1;
                try
                {
                    reviewerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ReviewerId);
                }
                catch
                { }
                int reviewerNameIndex = -1;
                try
                {
                    reviewerNameIndex = reader.GetOrdinal(Constants.ColumnNames.ReviewerName);
                }
                catch
                { }

                int projectCapabilityIdsIndex = -1;
                try
                {
                    projectCapabilityIdsIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectCapabilityIds);
                }
                catch
                { }

                try
                {
                    projectBusinessTypesIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessTypeId);
                }
                catch
                { }

                try
                {
                    clientIsNoteRequiredIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIsNoteRequired);
                }
                catch
                { }

                try
                {
                    opportunityNumberIndex = reader.GetOrdinal(Constants.ColumnNames.OpportunityNumberColumn);
                }
                catch
                { }

                try
                {
                    sowBudgetIndex = reader.GetOrdinal(Constants.ColumnNames.SowBudgetColumn);
                }
                catch
                { }

                try
                {
                    projectOwnerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectOwnerId);
                }
                catch
                {
                    projectOwnerIdIndex = -1;
                }

                try
                {
                    projectOwnerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectOwnerFirstName);
                    projectOwnerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectOwnerLastName);
                }
                catch
                {
                    projectOwnerFirstNameIndex = -1;
                }

                try
                {
                    isNoteRequiredIndex = reader.GetOrdinal(Constants.ColumnNames.IsNoteRequired);
                }
                catch
                {
                    isNoteRequiredIndex = -1;
                }

                try
                {
                    descriptionIndex = reader.GetOrdinal(Constants.ColumnNames.DescriptionColumn);
                }
                catch
                {
                    descriptionIndex = -1;
                }

                try
                {
                    hasAttachmentsIndex = reader.GetOrdinal(Constants.ColumnNames.HasAttachmentsColumn);
                }
                catch
                {
                    hasAttachmentsIndex = -1;
                }
                try
                {
                    pricingListIdIndex = reader.GetOrdinal(Constants.ColumnNames.PricingListId);
                }
                catch
                {
                    pricingListIdIndex = -1;
                }

                try
                {
                    pricingListNameIndex = reader.GetOrdinal(Constants.ColumnNames.PricingListNameColumn);
                }
                catch
                {
                    pricingListNameIndex = -1;
                }

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
                    salesPersonIdIndex = reader.GetOrdinal(Constants.ColumnNames.SalespersonIdColumn);
                }
                catch
                {
                    salesPersonIdIndex = -1;
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
                    isMarginColorInfoEnabledIndex = reader.GetOrdinal(Constants.ColumnNames.IsMarginColorInfoEnabledColumn);
                }
                catch
                {
                    isMarginColorInfoEnabledIndex = -1;
                }
                try
                {
                    canCreateCustomWorkTypesIndex = reader.GetOrdinal(Constants.ColumnNames.CanCreateCustomWorkTypesColumn);
                }
                catch
                {
                    canCreateCustomWorkTypesIndex = -1;
                }
                try
                {
                    IsInternalIndex = reader.GetOrdinal(Constants.ColumnNames.IsInternalColumn);
                }
                catch
                {
                    IsInternalIndex = -1;
                }
                try
                {
                    ClientIsInternalIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIsInternal);
                }
                catch
                {
                    ClientIsInternalIndex = -1;
                }

                try
                {
                    hasTimeEntriesIndex = reader.GetOrdinal(Constants.ParameterNames.HasTimeEntries);
                }
                catch
                {
                    hasTimeEntriesIndex = -1;
                }
                int businessGroupIdIndex = -1;
                int businessGroupNameIndex = -1;
                try
                {
                    businessGroupIdIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessGroupIdColumn);
                    businessGroupNameIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessGroupName);
                }
                catch
                {
                    businessGroupIdIndex = -1;
                    businessGroupNameIndex = -1;
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

                    };
                    if (pmIndex >= 0)
                    {
                        project.ProjectManagers = Utils.stringToProjectManagersList(reader.GetString(pmIndex));
                    }
                    if (projectCapabilitiesIndex >= 0)
                    {
                        project.Capabilities = !reader.IsDBNull(projectCapabilitiesIndex) ? reader.GetString(projectCapabilitiesIndex).Replace(";", ", ") : string.Empty;
                    }
                    if (ToAddressListIndex > -1)
                    {
                        project.ToAddressList = !reader.IsDBNull(ToAddressListIndex) ? reader.GetString(ToAddressListIndex) : string.Empty;
                    }
                    if (ProjectOwnerAlias > -1)
                    {
                        project.OwnerAlias = !reader.IsDBNull(ProjectOwnerAlias) ? reader.GetString(ProjectOwnerAlias) : string.Empty;
                    }
                    if (poNumberIndex > -1)
                    {
                        project.PONumber = !reader.IsDBNull(poNumberIndex) ? reader.GetString(poNumberIndex) : string.Empty;
                    }

                    if (projectCapabilityIdsIndex > -1)
                    {
                        project.ProjectCapabilityIds = reader.GetString(projectCapabilityIdsIndex);
                    }

                    if (sowBudgetIndex != -1)
                    {
                        project.SowBudget = reader.IsDBNull(sowBudgetIndex) ? null : (decimal?)reader.GetDecimal(sowBudgetIndex);
                    }

                    if (projectOwnerIdIndex > -1 && !reader.IsDBNull(projectOwnerIdIndex))
                    {
                        project.ProjectOwner = new Person()
                        {
                            Id = reader.GetInt32(projectOwnerIdIndex)
                        };
                    }

                    if (projectOwnerFirstNameIndex > -1 && project.ProjectOwner != null)
                    {
                        project.ProjectOwner.FirstName = reader.GetString(projectOwnerFirstNameIndex);
                        project.ProjectOwner.LastName = reader.GetString(projectOwnerLastNameIndex);
                    }

                    if (isNoteRequiredIndex > -1)
                    {
                        project.IsNoteRequired = reader.GetBoolean(isNoteRequiredIndex);
                    }

                    if (descriptionIndex > -1)
                    {
                        project.Description = !reader.IsDBNull(descriptionIndex) ? reader.GetString(descriptionIndex) : string.Empty;
                    }
                    if (poAmountIndex > -1)
                    {
                        project.POAmount = !reader.IsDBNull(poAmountIndex) ? (Decimal?)reader.GetDecimal(poAmountIndex) : null;
                    }

                    if (hasAttachmentsIndex >= 0)
                    {
                        project.HasAttachments = (int)reader[hasAttachmentsIndex] == 1;
                    }

                    if (hasTimeEntriesIndex >= 0)
                    {
                        project.HasTimeEntries = reader.GetBoolean(hasTimeEntriesIndex);
                    }

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

                    if (businessGroupIdIndex >= 0)
                    {
                        try
                        {
                            var businessGroup = new BusinessGroup
                            {
                                Id = (int)reader[businessGroupIdIndex],
                                Name = (string)reader[businessGroupNameIndex],
                            };

                            project.BusinessGroup = businessGroup;
                        }
                        catch
                        {
                        }
                    }
                    if (salesPersonIdIndex >= 0)
                    {
                        try
                        {
                            project.SalesPersonId = reader.GetInt32(salesPersonIdIndex);
                        }
                        catch
                        {
                            project.SalesPersonId = 0;
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
                    if (pricingListIdIndex > -1)
                    {
                        project.PricingList = !reader.IsDBNull(pricingListIdIndex) ?
                                                  new PricingList
                                                  {
                                                      PricingListId = reader.GetInt32(pricingListIdIndex),
                                                      Name = pricingListNameIndex > -1 ? reader.GetString(pricingListNameIndex) : string.Empty
                                                  }
                                                  : null;
                    }
                    project.Client = new Client
                    {
                        Id = reader.GetInt32(clientIdIndex),
                        Name = reader.GetString(clientNameIndex),
                        IsChargeable = reader.GetBoolean(clientIsChargeableIndex)
                    };

                    if (isHouseAccountIndex > -1)
                    {
                        project.Client.IsHouseAccount = reader.GetBoolean(isHouseAccountIndex);
                    }

                    if (clientIsNoteRequiredIndex != -1)
                    {
                        project.Client.IsNoteRequired = reader.GetBoolean(clientIsNoteRequiredIndex);
                    }

                    if (isMarginColorInfoEnabledIndex >= 0)
                    {
                        try
                        {
                            project.Client.IsMarginColorInfoEnabled = reader.GetBoolean(isMarginColorInfoEnabledIndex);
                        }
                        catch
                        {
                        }
                    }
                    if (canCreateCustomWorkTypesIndex > -1)
                    {
                        project.CanCreateCustomWorkTypes = reader.GetBoolean(canCreateCustomWorkTypesIndex);
                    }
                    if (IsInternalIndex > -1)
                    {
                        project.IsInternal = reader.GetBoolean(IsInternalIndex);
                    }
                    if (ClientIsInternalIndex > -1)
                    {
                        project.Client.IsInternal = reader.GetBoolean(ClientIsInternalIndex);
                    }
                    project.Status = new ProjectStatus
                    {
                        Id = reader.GetInt32(projectStatusIdIndex),
                        Name = reader.GetString(projectStatusNameIndex)
                    };

                    if (projectBusinessTypesIndex > -1)
                    {
                        project.BusinessType = reader.IsDBNull(projectBusinessTypesIndex) ? (BusinessType)Enum.Parse(typeof(BusinessType), "0") : (BusinessType)Enum.Parse(typeof(BusinessType), reader.GetInt32(projectBusinessTypesIndex).ToString());
                    }

                    project.OpportunityId =
                        !reader.IsDBNull(opportunityId) ? (int?)reader.GetInt32(opportunityId) : null;

                    if (opportunityNumberIndex >= 0)
                        project.OpportunityNumber = reader.IsDBNull(opportunityNumberIndex) ? null : reader.GetString(opportunityNumberIndex);

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

                                project.Group = @group;
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

                    if (seniorManagerIdIndex >= 0)
                    {
                        try
                        {
                            project.SeniorManagerId = reader.GetInt32(seniorManagerIdIndex);
                        }
                        catch
                        {
                        }
                    }
                    if (seniorManagerNameIndex >= 0)
                    {
                        try
                        {
                            project.SeniorManagerName = reader.GetString(seniorManagerNameIndex);
                        }
                        catch
                        {
                        }
                    }
                    if (isSeniorManagerUnassignedIndex >= 0)
                    {
                        try
                        {
                            project.IsSeniorManagerUnassigned = reader.GetBoolean(isSeniorManagerUnassignedIndex);
                        }
                        catch
                        {
                        }
                    }

                    if (reviewerIdIndex >= 0)
                    {
                        try
                        {
                            project.CSATOwnerId = reader.GetInt32(reviewerIdIndex);
                        }
                        catch
                        {
                        }
                    }
                    if (reviewerNameIndex >= 0)
                    {
                        try
                        {
                            project.CSATOwnerName = reader.GetString(reviewerNameIndex);
                        }
                        catch
                        {
                        }
                    }
                    if (divisionIdIndex >= 0)
                    {
                        try
                        {
                            project.Division = new ProjectDivision { Id = reader.GetInt32(divisionIdIndex) };
                        }
                        catch
                        { }
                    }
                    if (divisionNameIndex >= 0)
                    {
                        try
                        {
                            project.Division.Name = reader.GetString(divisionNameIndex);
                        }
                        catch { }
                    }
                    if (channelIdIndex >= 0)
                    {
                        try
                        {
                            project.Channel = new Channel { Id = reader.GetInt32(channelIdIndex) };
                        }
                        catch
                        { }
                    }
                    if (channelIdIndex >= 0)
                    {
                        try
                        {
                            project.Channel.Name = reader.GetString(ChannelNameIndex);
                        }
                        catch { }
                    }
                    if (subchannelIndex >= 0)
                    {
                        try
                        {
                            project.SubChannel = reader.GetString(subchannelIndex);
                        }
                        catch
                        { }
                    }
                    if (revenueTypeIdIndex >= 0)
                    {
                        try
                        {
                            project.RevenueType = new Revenue { Id = reader.GetInt32(revenueTypeIdIndex) };
                        }
                        catch
                        { }
                    }
                    if (revenueNameIndex >= 0)
                    {
                        try
                        {
                            project.RevenueType.Name = reader.GetString(revenueNameIndex);
                        }
                        catch { }
                    }
                    if (offeringIdIndex >= 0)
                    {
                        try
                        {
                            project.Offering = new Offering { Id = reader.GetInt32(offeringIdIndex) };
                        }
                        catch
                        { }
                    }
                    if (offeringNameIndex >= 0)
                    {
                        try
                        {
                            project.Offering.Name = reader.GetString(offeringNameIndex);
                        }
                        catch { }
                    }
                    if (isClientTimeEntryRequiredIndex >= 0)
                    {
                        project.IsClientTimeEntryRequired = reader.GetBoolean(isClientTimeEntryRequiredIndex);
                    }
                    if (previousProjectNumberIndex >= 0 && previousProjectIdIndex >= 0)
                    {
                        project.PreviousProject = reader.IsDBNull(previousProjectNumberIndex) ? null : new Project { Id = reader.GetInt32(previousProjectIdIndex), ProjectNumber = reader.GetString(previousProjectNumberIndex) };
                    }
                    if (outsourceIdIndex >= 0)
                    {
                        project.OutsourceId = reader.GetInt32(outsourceIdIndex);
                    }

                    resultList.Add(project);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void ReadProjectsWithMilestones(SqlDataReader reader, List<Project> resultList)
        {
            try
            {
                if (!reader.HasRows) return;
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
                int pmIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagersIdFirstNameLastName);

                int projectOwnerIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectOwnerId);
                int projectOwnerLastNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectOwnerLastName);
                int projectOwnerFirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectOwnerFirstName);
                int pONumberIndex = -1;

                int mileStoneIdIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneId);
                int mileStoneNameIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneName);

                int sowBudgetIndex = reader.GetOrdinal(Constants.ColumnNames.SowBudgetColumn);
                int salesPersonNameIndex = -1;
                int practiceOwnerNameIndex = -1;
                int projectGroupIdIndex = -1;
                int projectGroupNameIndex = -1;

                int businessTypeIdIndex = -1;
                int pricingListIdIndex = -1;
                int pricingListNameIndex = -1;
                int businessGroupIdIndex = -1;
                int businessGroupNameIndex = -1;
                int hasAttachments = -1;
                int seniorManagerNameIndex = -1;
                int seniorManagerIdIndex = -1;
                int isHouseAccountIndex = -1;

                int projectCapabilitiesIndex = -1;
                int divisionIdIndex = -1;
                int divisionNameIndex = -1;
                int ChannelIdIndex = -1;
                int ChannelNameIndex = -1;
                int subchannelIndex = -1;
                int revenueTypeIdIndex = -1;
                int revenueTypeNameIndex = -1;
                int offeringIdIndex = -1;
                int OfferingNameIndex = -1;
                int isClientTimeEntryRequiredIndex = -1;
                int previousProjectNumberIndex = -1;
                int previousProjectIdIndex = -1;
                int outsourceIdIndex = -1;
                try
                {
                    outsourceIdIndex = reader.GetOrdinal(Constants.ColumnNames.OutsourceId);
                }
                catch { }
                try
                {
                    previousProjectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.PreviousProjectNumber);
                    previousProjectIdIndex = reader.GetOrdinal(Constants.ColumnNames.PreviousProjectId);
                }
                catch { }
                try
                {
                    isClientTimeEntryRequiredIndex = reader.GetOrdinal(Constants.ColumnNames.IsClientTimeEntryRequired);
                }
                catch { }

                try
                {
                    divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);
                }
                catch { }

                try
                {
                    divisionNameIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionName);
                }
                catch { }

                try
                {
                    ChannelIdIndex = reader.GetOrdinal(Constants.ColumnNames.ChannelId);
                }
                catch { }

                try
                {
                    ChannelNameIndex = reader.GetOrdinal(Constants.ColumnNames.ChannelName);
                }
                catch { }

                try
                {
                    subchannelIndex = reader.GetOrdinal(Constants.ColumnNames.SubChannel);
                }
                catch { }

                try
                {
                    revenueTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.RevenueTypeId);
                }
                catch { }

                try
                {
                    revenueTypeNameIndex = reader.GetOrdinal(Constants.ColumnNames.RevenueName);
                }
                catch { }

                try
                {
                    offeringIdIndex = reader.GetOrdinal(Constants.ColumnNames.OfferingId);
                }
                catch { }

                try
                {
                    OfferingNameIndex = reader.GetOrdinal(Constants.ColumnNames.OfferName);
                }
                catch { }

                try
                {
                    projectCapabilitiesIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectCapabilities);
                }
                catch
                { }

                try
                {
                    isHouseAccountIndex = reader.GetOrdinal(Constants.ColumnNames.IsHouseAccount);
                }
                catch
                { }
                try
                {
                    pONumberIndex = reader.GetOrdinal(Constants.ColumnNames.PONumber);
                }
                catch
                { }

                try
                {
                    seniorManagerIdIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorManagerId);
                }
                catch
                { }

                try
                {
                    seniorManagerNameIndex = reader.GetOrdinal(Constants.ColumnNames.SeniorManagerName);
                }
                catch
                { }
                try
                {
                    businessTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessTypeId);
                }
                catch
                {
                    businessTypeIdIndex = -1;
                }
                try
                {
                    pricingListIdIndex = reader.GetOrdinal(Constants.ColumnNames.PricingListId);
                    pricingListNameIndex = reader.GetOrdinal(Constants.ColumnNames.PricingListNameColumn);
                }
                catch
                {
                    pricingListIdIndex = -1;
                    pricingListNameIndex = -1;
                }
                try
                {
                    businessGroupIdIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessGroupIdColumn);
                    businessGroupNameIndex = reader.GetOrdinal(Constants.ColumnNames.BusinessGroupName);
                }
                catch
                {
                    businessGroupIdIndex = -1;
                    businessGroupNameIndex = -1;
                }

                try
                {
                    projectGroupIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupIdColumn);
                    projectGroupNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn);
                }
                catch
                {
                    projectGroupIdIndex = -1;
                    projectGroupNameIndex = -1;
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
                    hasAttachments = reader.GetOrdinal(Constants.ColumnNames.HasAttachmentsColumn);
                }
                catch
                {
                    hasAttachments = -1;
                }

                while (reader.Read())
                {
                    var projectId = reader.GetInt32(projectIdIndex);
                    Project project;

                    if (resultList.Any(p => p.Id.Value == projectId))
                    {
                        project = resultList.First(p => p.Id.Value == projectId);
                    }
                    else
                    {
                        project = new Project
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

                            ProjectManagers = Utils.stringToProjectManagersList(reader.GetString(pmIndex)),
                            SowBudget = !reader.IsDBNull(sowBudgetIndex) ? (Decimal?)reader.GetDecimal(sowBudgetIndex) : null
                        };

                        if (previousProjectNumberIndex >= 0 && previousProjectIdIndex >= 0)
                        {
                            project.PreviousProject = reader.IsDBNull(previousProjectNumberIndex) ? null : new Project { Id = reader.GetInt32(previousProjectIdIndex), ProjectNumber = reader.GetString(previousProjectNumberIndex) };
                        }
                        if (outsourceIdIndex >= 0)
                        {
                            project.OutsourceId = reader.GetInt32(outsourceIdIndex);

                        }

                        if (divisionIdIndex >= 0)
                        {
                            try
                            {
                                project.Division = new ProjectDivision { Id = reader.GetInt32(divisionIdIndex), Name = reader.GetString(divisionNameIndex) };
                            }
                            catch { }
                        }

                        if (ChannelIdIndex >= 0)
                        {
                            try
                            {
                                project.Channel = new Channel { Id = reader.GetInt32(ChannelIdIndex), Name = reader.GetString(ChannelNameIndex) };
                            }
                            catch { }
                        }

                        if (subchannelIndex >= 0)
                        {
                            try
                            {
                                project.SubChannel = reader.GetString(subchannelIndex);
                            }
                            catch { }
                        }
                        if (offeringIdIndex >= 0)
                        {
                            try
                            {
                                project.Offering = new Offering { Id = reader.GetInt32(offeringIdIndex), Name = reader.GetString(OfferingNameIndex) };
                            }
                            catch { }
                        }
                        if (revenueTypeIdIndex >= 0)
                        {
                            try
                            {
                                project.RevenueType = new Revenue { Id = reader.GetInt32(revenueTypeIdIndex), Name = reader.GetString(revenueTypeNameIndex) };
                            }
                            catch { }
                        }

                        if (projectCapabilitiesIndex >= 0)
                        {

                            project.Capabilities = !reader.IsDBNull(projectCapabilitiesIndex) ? reader.GetString(projectCapabilitiesIndex).Replace(";", ", ") : string.Empty;
                        }

                        if (projectGroupIdIndex >= 0)
                        {
                            try
                            {
                                var group = new ProjectGroup
                                {
                                    Id = (int)reader[projectGroupIdIndex],
                                    Name = (string)reader[projectGroupNameIndex],
                                };

                                project.Group = @group;
                            }
                            catch
                            {
                            }
                        }

                        if (pricingListIdIndex >= 0)
                        {
                            try
                            {
                                var pricingList = new PricingList
                                {
                                    PricingListId = (int)reader[pricingListIdIndex],
                                    Name = (string)reader[pricingListNameIndex],
                                };

                                project.PricingList = pricingList;
                            }
                            catch
                            {
                            }
                        }

                        if (businessGroupIdIndex >= 0)
                        {
                            try
                            {
                                var businessGroup = new BusinessGroup
                                {
                                    Id = (int)reader[businessGroupIdIndex],
                                    Name = (string)reader[businessGroupNameIndex],
                                };

                                project.BusinessGroup = businessGroup;
                            }
                            catch
                            {
                            }
                        }
                        if (isClientTimeEntryRequiredIndex >= 0)
                        {
                            project.IsClientTimeEntryRequired = reader.GetBoolean(isClientTimeEntryRequiredIndex);
                        }

                        if (businessTypeIdIndex >= 0)
                        {
                            try
                            {
                                BusinessType businessType = (BusinessType)(int)reader[businessTypeIdIndex];
                                project.BusinessType = businessType;
                            }
                            catch
                            {
                            }
                        }
                        if (seniorManagerIdIndex >= 0)
                        {
                            try
                            {
                                project.SeniorManagerId = reader.GetInt32(seniorManagerIdIndex);
                            }
                            catch
                            {
                            }
                        }
                        if (seniorManagerNameIndex >= 0)
                        {
                            try
                            {
                                project.SeniorManagerName = reader.GetString(seniorManagerNameIndex);
                            }
                            catch
                            {
                            }
                        }
                        if (pONumberIndex > -1)
                        {
                            try
                            {
                                project.PONumber = !reader.IsDBNull(pONumberIndex) ? reader.GetString(pONumberIndex) : string.Empty;
                            }
                            catch
                            {
                            }
                        }

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

                        if (hasAttachments >= 0)
                        {
                            project.HasAttachments = (int)reader[hasAttachments] == 1;
                        }

                        project.Client = new Client
                        {
                            Id = reader.GetInt32(clientIdIndex),
                            Name = reader.GetString(clientNameIndex),
                            IsChargeable = reader.GetBoolean(clientIsChargeableIndex)
                        };
                        if (isHouseAccountIndex > -1)
                        {
                            try
                            {
                                project.Client.IsHouseAccount = !reader.IsDBNull(isHouseAccountIndex) && reader.GetBoolean(isHouseAccountIndex);
                            }
                            catch
                            {
                            }
                        }

                        project.Status = new ProjectStatus
                        {
                            Id = reader.GetInt32(projectStatusIdIndex),
                            Name = reader.GetString(projectStatusNameIndex)
                        };

                        project.OpportunityId =
                            !reader.IsDBNull(opportunityId) ? (int?)reader.GetInt32(opportunityId) : null;

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

                    if (!reader.IsDBNull(projectOwnerIdIndex))
                    {
                        project.ProjectOwner = new Person()
                        {
                            Id = reader.GetInt32(projectOwnerIdIndex),
                            FirstName = reader.GetString(projectOwnerFirstNameIndex),
                            LastName = reader.GetString(projectOwnerLastNameIndex)
                        };
                    }

                    if (reader.IsDBNull(mileStoneIdIndex)) continue;
                    var milestone = new Milestone
                    {
                        Description = reader.GetString(mileStoneNameIndex),
                        Id = reader.GetInt32(mileStoneIdIndex)
                    };
                    if (project.Milestones == null)
                    {
                        project.Milestones = new List<Milestone>();
                    }


                    project.Milestones.Add(milestone);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void ReadProjectSearchList(DbDataReader reader, List<Project> result)
        {
            if (!reader.HasRows) return;
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
            int hasAttachmentIndex = -1;
            int PONumberIndex = -1;
            try
            {
                PONumberIndex = reader.GetOrdinal(Constants.ColumnNames.PONumber);
            }
            catch
            {
                PONumberIndex = -1;
            }
            try
            {
                hasAttachmentIndex = reader.GetOrdinal(Constants.ColumnNames.HasAttachmentsColumn);
            }
            catch
            {
                hasAttachmentIndex = -1;
            }

            while (reader.Read())
            {
                Project project = new Project
                {
                    Id = reader.GetInt32(projectIdIndex),
                    Name = reader.GetString(projectNameIndex),
                    StartDate = !reader.IsDBNull(projectStartDateIndex)
                                    ? (DateTime?)reader.GetDateTime(projectStartDateIndex)
                                    : null,
                    EndDate = !reader.IsDBNull(projectEndDateIndex)
                                  ? (DateTime?)reader.GetDateTime(projectEndDateIndex)
                                  : null,
                    ProjectNumber = reader.GetString(projectNumberIndex),
                    Client = new Client
                    {
                        Id = reader.GetInt32(clientIdIndex),
                        Name = reader.GetString(clientNameIndex)
                    },
                    Status = new ProjectStatus
                    {
                        Id = reader.GetInt32(projectStatusIdIndex),
                        Name = reader.GetString(projectStatusNameIndex)
                    }
                };

                if (!reader.IsDBNull(projectGroupIdIndex))
                {
                    var groups = ProjectGroupDAL.GroupListAll(null, project.Id);
                    if (groups.Any())
                        project.Group = groups[0];
                }

                if (!reader.IsDBNull(milestoneIdIndex))
                {
                    project.Milestones = new List<Milestone>();

                    Milestone milestone = new Milestone
                    {
                        Id = reader.GetInt32(milestoneIdIndex),
                        Description = reader.GetString(descriptionIndex)
                    };

                    project.Milestones.Add(milestone);
                }

                if (hasAttachmentIndex >= 0)
                {
                    project.HasAttachments = (int)reader[hasAttachmentIndex] == 1;
                }
                if (PONumberIndex > -1)
                {
                    project.PONumber = reader.IsDBNull(PONumberIndex) ? string.Empty : reader.GetString(PONumberIndex);
                }

                result.Add(project);
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
                    if (reader.HasRows)
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
            bool showProposed,
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
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowProposedParam, showProposed);
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
            bool showProposed,
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
                command.Parameters.AddWithValue(Constants.ParameterNames.ShowProposedParam, showProposed);
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
            if (!reader.HasRows) return;
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
                if (result.All(p => p.PersonId != personId))
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
                if (reader.IsDBNull(MonthStartDateIndex)) continue;
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

        private static void ReadCalculatedPracticeBudget(SqlDataReader reader, List<ProjectsGroupedByPractice> result)
        {
            if (!reader.HasRows) return;
            int PracticeIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
            int NameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
            int MonthStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.MonthStartDate);
            int AmountIndex = reader.GetOrdinal(Constants.ColumnNames.Amount);
            int RevenueIndex = reader.GetOrdinal(Constants.ColumnNames.RevenueColumn);
            while (reader.Read())
            {
                ProjectsGroupedByPractice practice;
                var practiceId = reader.GetInt32(PracticeIdIndex);
                if (result.All(p => p.PracticeId != practiceId))
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
                if (reader.IsDBNull(MonthStartDateIndex)) continue;
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
            if (!reader.HasRows) return;
            int PracticeIdIndex = reader.GetOrdinal(Constants.ColumnNames.PracticeIdColumn);
            int NameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
            int MonthStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.MonthStartDate);
            int AmountIndex = reader.GetOrdinal(Constants.ColumnNames.Amount);
            while (reader.Read())
            {
                ProjectsGroupedByPractice practice;
                var practiceId = reader.GetInt32(PracticeIdIndex);
                if (result.All(p => p.PracticeId != practiceId))
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

        private static void ReadPersonBudget(SqlDataReader reader, List<ProjectsGroupedByPerson> result)
        {
            if (!reader.HasRows) return;
            int PersonIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            int LastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int FirstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int MonthStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.MonthStartDate);
            int AmountIndex = reader.GetOrdinal(Constants.ColumnNames.Amount);
            while (reader.Read())
            {
                ProjectsGroupedByPerson person;
                var personId = reader.GetInt32(PersonIdIndex);
                if (result.All(p => p.PersonId != personId))
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
            if (!categoryIdList.Any()) return;
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

        public static void SaveProjectAttachmentData(ProjectAttachment attachment, int projectId, string userName)
        {
            var connection = new SqlConnection(DataSourceHelper.DataConnection);

            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.SaveProjectAttachment, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId);
                command.Parameters.AddWithValue(Constants.ParameterNames.UploadedDateParam, DateTime.Now);
                command.Parameters.AddWithValue(Constants.ParameterNames.AttachmentFileName, !string.IsNullOrEmpty(attachment.AttachmentFileName) ? (object)attachment.AttachmentFileName : DBNull.Value);
                command.Parameters.Add(Constants.ParameterNames.AttachmentData, SqlDbType.VarBinary, -1);
                command.Parameters.AddWithValue(Constants.ParameterNames.CategoryIdParam, (int)attachment.Category);
                command.Parameters[Constants.ParameterNames.AttachmentData].Value = attachment.AttachmentData != null ? (object)attachment.AttachmentData : DBNull.Value;
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam,
                    !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static byte[] GetProjectAttachmentData(int projectId, int attachmentId)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.GetProjectAttachmentData, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, projectId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.AttachmentIdParam, attachmentId);

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

        public static List<ProjectAttachment> GetProjectAttachments(int projectId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.GetProjectAttachments, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var result = new List<ProjectAttachment>();
                        ReadProjectAttachments(reader, result);
                        return result;
                    }
                }
            }
        }

        public static void ReadProjectAttachments(SqlDataReader reader, List<ProjectAttachment> attachments)
        {
            if (!reader.HasRows) return;
            int idIndex = reader.GetOrdinal(Constants.ColumnNames.Id);
            int attachmentFileNameIndex = reader.GetOrdinal(Constants.ColumnNames.FileName);
            int uploadedDateIndex = reader.GetOrdinal(Constants.ColumnNames.UploadedDate);
            int attachmentSizeIndex = reader.GetOrdinal(Constants.ColumnNames.AttachmentSize);
            int categoryIdIndex = reader.GetOrdinal(Constants.ColumnNames.CategoryId);
            int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
            int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);

            while (reader.Read())
            {
                ProjectAttachment attachment = new ProjectAttachment
                {
                    AttachmentId = reader.GetInt32(idIndex),
                    AttachmentFileName = reader.GetString(attachmentFileNameIndex),
                    AttachmentSize = (int)reader.GetInt64(attachmentSizeIndex),
                    UploadedDate =
                        reader.IsDBNull(uploadedDateIndex)
                            ? null
                            : (DateTime?)reader.GetDateTime(uploadedDateIndex),
                    Category = (ProjectAttachmentCategory)reader.GetInt32(categoryIdIndex)
                };
                if (!reader.IsDBNull(personIdIndex))
                {
                    attachment.Uploader = reader.GetString(firstNameIndex) + " " + reader.GetString(lastNameIndex);
                }
                attachments.Add(attachment);
            }
        }

        public static void DeleteProjectAttachmentByProjectId(int? attachmentId, int projectId, string userName)
        {
            var connection = new SqlConnection(DataSourceHelper.DataConnection);

            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.DeleteProjectAttachmentByProjectId, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.AttachmentIdParam, attachmentId.HasValue ? (object)attachmentId : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam,
                    !string.IsNullOrEmpty(userName) ? (object)userName : DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void ProjectDelete(int projectId, string userName)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.ProjectDelete, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userName);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<Project> ProjectsAll()
        {
            var projectList = new List<Project>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.ProjectListAllWithoutFiltering, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    connection.Open();

                    var reader = command.ExecuteReader();

                    ReadProjects(reader, projectList);
                }
            }
            return projectList;
        }

        public static List<Project> GetProjectListByDateRange(bool showProjected, bool showCompleted, bool showActive, bool showInternal, bool showExperimental, bool showInactive, DateTime periodStart, DateTime periodEnd)
        {
            var projectList = new List<Project>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.GetProjectListByDateRange, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowProjectedParam, showProjected);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowCompletedParam, showCompleted);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowActiveParam, showActive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowInternalParam, showInternal);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowExperimentalParam, showExperimental);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ShowInactiveParam, showInactive);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, periodStart);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, periodEnd);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    ReadProjectsShort(reader, projectList);
                }
            }

            MilestonePersonDAL.LoadMilestonePersonListForProject(projectList);

            return projectList;
        }

        private static void ReadProjectsShort(SqlDataReader reader, List<Project> resultList)
        {
            try
            {
                if (!reader.HasRows) return;
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                int clientIdIndex = reader.GetOrdinal(Constants.ColumnNames.ClientIdColumn);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.NameColumn);
                int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDateColumn);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDateColumn);
                int projectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusIdColumn);
                int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
                int pmIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectManagersIdFirstNameLastName);

                int hasAttachmentIndex = -1;

                try
                {
                    hasAttachmentIndex = reader.GetOrdinal(Constants.ColumnNames.HasAttachmentsColumn);
                }
                catch
                {
                    hasAttachmentIndex = -1;
                }

                while (reader.Read())
                {
                    var projectId = reader.GetInt32(projectIdIndex);
                    Project project;

                    if (resultList.Any(p => p.Id.Value == projectId))
                    {
                        project = resultList.First(p => p.Id.Value == projectId);
                    }
                    else
                    {
                        project = new Project
                        {
                            Id = reader.GetInt32(projectIdIndex),
                            Name = reader.GetString(nameIndex),
                            StartDate =
                                !reader.IsDBNull(startDateIndex)
                                    ? (DateTime?)reader.GetDateTime(startDateIndex)
                                    : null,
                            EndDate =
                                !reader.IsDBNull(endDateIndex)
                                    ? (DateTime?)reader.GetDateTime(endDateIndex)
                                    : null,
                            ProjectNumber = reader.GetString(projectNumberIndex),
                            ProjectManagers = Utils.stringToProjectManagersList(reader.GetString(pmIndex)),
                            Client = new Client
                            {
                                Id = reader.GetInt32(clientIdIndex),
                                Name = reader.GetString(clientNameIndex)
                            },
                            Status = new ProjectStatus
                            {
                                Id = reader.GetInt32(projectStatusIdIndex),
                                Name = reader.GetString(projectStatusNameIndex)
                            }
                        };

                        if (hasAttachmentIndex >= 0)
                        {
                            project.HasAttachments = (int)reader[hasAttachmentIndex] == 1;
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

        public static bool IsUserHasPermissionOnProject(string user, int id, bool isProjectId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.IsUserHasPermissionOnProject, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, user);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IdParam, id);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsProjectIdParam, isProjectId);

                    connection.Open();

                    var result = command.ExecuteScalar();

                    return Convert.ToBoolean(result);
                }
            }
        }

        public static bool IsUserIsOwnerOfProject(string user, int id, bool isProjectId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.IsUserIsOwnerOfProject, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, user);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IdParam, id);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsProjectIdParam, isProjectId);

                    connection.Open();

                    var result = command.ExecuteScalar();

                    return Convert.ToBoolean(result);
                }
            }
        }

        public static bool IsUserIsProjectOwner(string user, int id)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.IsUserIsProjectOwner, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, user);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IdParam, id);

                    connection.Open();

                    var result = command.ExecuteScalar();

                    return Convert.ToBoolean(result);
                }
            }
        }

        public static List<Project> GetOwnerProjectsAfterTerminationDate(int personId, DateTime terminationDate)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.GetOwnerProjectsAfterTerminationDateProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.TerminationDate, terminationDate);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    List<Project> result = new List<Project>();

                    ReadProjectsAfterTermination(reader, result);

                    return result;
                }
            }
        }

        private static void ReadProjectsAfterTermination(SqlDataReader reader, List<Project> result)
        {
            if (!reader.HasRows) return;
            int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
            int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
            int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
            while (reader.Read())
            {
                var Project = new Project
                {
                    Id = reader.GetInt32(projectIdIndex),
                    Name = reader.GetString(projectNameIndex),
                    ProjectNumber = reader.GetString(projectNumberIndex)
                };
                result.Add(Project);
            }
        }

        public static List<Project> GetProjectsListByProjectGroupId(int projectGroupId, bool isInternal, int personId, DateTime startDate, DateTime endDate)
        {
            var projectList = new List<Project>();
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.ProjectsListByProjectGroupId, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectGroupIdParam, projectGroupId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IsInternalParam, isInternal);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var project = new Project
                            {
                                Id = (int)reader[Constants.ColumnNames.ProjectIdColumn],
                                Name = (string)reader[Constants.ColumnNames.NameColumn]
                            };
                            projectList.Add(project);
                        }
                    }
                }
            }
            return projectList;
        }

        public static List<Project> GetBusinessDevelopmentProject()
        {
            List<Project> project = null;
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.GetBusinessDevelopmentProject, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    project = new List<Project>();
                    while (reader.Read())
                    {
                        project.Add(ReadProjectShort(reader));
                    }
                }
            }
            return project;
        }

        public static Project GetProjectByIdShort(int projectId)
        {
            Project project = null;
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.GetProjectByIdShort, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int isNoteRequiredIndex = -1;

                        project = ReadProjectShort(reader);

                        try
                        {
                            isNoteRequiredIndex = reader.GetOrdinal(Constants.ColumnNames.IsNoteRequired);
                        }
                        catch
                        {
                            isNoteRequiredIndex = -1;
                        }

                        if (isNoteRequiredIndex > -1)
                        {
                            project.IsNoteRequired = reader.GetBoolean(isNoteRequiredIndex);
                        }
                    }
                }
            }
            return project;
        }

        private static Project ReadProjectShort(SqlDataReader reader)
        {
            Project project = new Project { Id = (int)reader[Constants.ColumnNames.ProjectIdColumn], Name = (string)reader[Constants.ColumnNames.NameColumn], ProjectNumber = (string)reader[Constants.ColumnNames.ProjectNumberColumn] };
            int endDateIndex = -1;
            try
            {
                endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDateColumn);
            }
            catch
            {
                endDateIndex = -1;
            }
            if (endDateIndex > -1)
            {
                project.EndDate = !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null;
            }
            return project;
        }

        public static List<TimeTypeRecord> GetTimeTypesByProjectId(int projectId, bool IsOnlyActive, DateTime? startDate, DateTime? endDate)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.GetTimeTypesByProjectIdProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsOnlyActiveParam, IsOnlyActive);
                if (startDate.HasValue && endDate.HasValue)
                {
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                }

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    List<TimeTypeRecord> result = new List<TimeTypeRecord>();

                    ReadTimeTypes(reader, result);

                    return result;
                }
            }
        }

        private static void ReadTimeTypes(SqlDataReader reader, List<TimeTypeRecord> result)
        {
            if (!reader.HasRows) return;
            int timeTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.TimeTypeId);
            int inUseIndex = reader.GetOrdinal(Constants.ColumnNames.InUse);
            int nameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
            int isDefaultIndex;
            try
            {
                isDefaultIndex = reader.GetOrdinal(Constants.ColumnNames.IsDefault);
            }
            catch (Exception ex)
            {
                isDefaultIndex = -1;
            }
            while (reader.Read())
            {
                var tt = new TimeTypeRecord
                {
                    Id = reader.GetInt32(timeTypeIdIndex),
                    Name = reader.GetString(nameIndex),
                    IsDefault = isDefaultIndex > -1 && reader.GetBoolean(isDefaultIndex),
                    InUse = Convert.ToBoolean(reader.GetInt32(inUseIndex))
                };

                result.Add(tt);
            }
        }

        public static void SetProjectTimeTypes(int projectId, string projectTimeTypesList, SqlConnection connection, SqlTransaction currentTransaction)
        {
            if (connection == null)
            {
                connection = new SqlConnection(DataSourceHelper.DataConnection);
            }
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.SetProjectTimeTypesProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectTimeTypesParam, projectTimeTypesList);
                try
                {
                    if (currentTransaction != null)
                    {
                        command.Transaction = currentTransaction;
                    }

                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new DataAccessException(ex);
                }
            }
        }

        public static Dictionary<DateTime, bool> GetIsHourlyRevenueByPeriod(int projectId, int personId, DateTime startDate, DateTime endDate)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.GetIsHourlyRevenueByPeriod, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        Dictionary<DateTime, bool> result = new Dictionary<DateTime, bool>();
                        if (reader.HasRows)
                        {
                            var chargeCodeDateIndex = reader.GetOrdinal(Constants.ColumnNames.ChargeCodeDate);
                            var isHourlyRevenueIndex = reader.GetOrdinal(Constants.ColumnNames.IsHourlyRevenueColumn);

                            while (reader.Read())
                            {
                                DateTime key = reader.GetDateTime(chargeCodeDateIndex);
                                bool value = reader.GetBoolean(isHourlyRevenueIndex);
                                result.Add(key, value);
                            }
                        }
                        return result;
                    }
                }
            }
        }

        public static Project GetProjectShortByProjectNumber(string projectNumber, int? milestoneId, DateTime? startDate, DateTime? endDate)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.ProjectShortGetByNumber, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ColumnNames.ProjectNumberColumn, projectNumber);
                    command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneId, milestoneId.HasValue ? (object)milestoneId : DBNull.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDateParam, startDate.HasValue ? (object)startDate : DBNull.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDateParam, endDate.HasValue ? (object)endDate : DBNull.Value);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                            int nameIndex = reader.GetOrdinal(Constants.ColumnNames.NameColumn);
                            int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDateColumn);
                            int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDateColumn);
                            int clientNameIndex = reader.GetOrdinal(Constants.ColumnNames.ClientNameColumn);
                            int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
                            int billingTypeIndex = reader.GetOrdinal(Constants.ColumnNames.BillingType);
                            int isInternalIndex = -1;
                            try
                            {
                                isInternalIndex = reader.GetOrdinal(Constants.ColumnNames.IsInternalColumn);
                            }
                            catch { }
                            var project = new Project
                            {
                                Id = reader.GetInt32(projectIdIndex),

                                Name = reader.GetString(nameIndex),
                                StartDate =
                                    !reader.IsDBNull(startDateIndex)
                                        ? (DateTime?)reader.GetDateTime(startDateIndex)
                                        : null,
                                EndDate =
                                    !reader.IsDBNull(endDateIndex)
                                        ? (DateTime?)reader.GetDateTime(endDateIndex)
                                        : null,
                                ProjectNumber = projectNumber,
                                BillableType =
                                    !reader.IsDBNull(billingTypeIndex)
                                        ? reader.GetString(billingTypeIndex)
                                        : string.Empty,
                                Status = new ProjectStatus
                                {
                                    Name = reader.GetString(projectStatusNameIndex)
                                },
                                Client = new Client
                                {
                                    Name = reader.GetString(clientNameIndex)
                                },
                                Group = new ProjectGroup
                                {
                                    Name =
                                        reader.GetString(
                                            reader.GetOrdinal(Constants.ColumnNames.ProjectGroupNameColumn))
                                },

                                IsInternal = reader.GetBoolean(isInternalIndex)

                            };

                            return project;
                        }
                    }

                    return null;
                }
            }
        }

        public static List<TimeTypeRecord> GetTimeTypesInUseDetailsByProject(int projectId, string timeTypeIds)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.GetTimeTypesInUseDetailsByProjectProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId);
                command.Parameters.AddWithValue(Constants.ParameterNames.TimeTypeIdsParam, timeTypeIds);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    List<TimeTypeRecord> result = new List<TimeTypeRecord>();
                    ReadTimeTypes(reader, result);
                    return result;
                }
            }
        }

        public static string AttachOpportunityToProject(int projectId, int opportunityId, string userLogin, int? pricingListId, bool link)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.AttachOpportunityToProject, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId);
                command.Parameters.AddWithValue(Constants.ParameterNames.OpportunityIdParam, opportunityId);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                command.Parameters.AddWithValue(Constants.ParameterNames.PricingListId, pricingListId.HasValue ? (object)pricingListId : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.LinkParam, link);
                connection.Open();

                var result = command.ExecuteScalar();
                return link ? result.ToString() : null;
            }
        }

        public static List<Attribution> GetProjectAttributionValues(int projectId)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.GetProjectAttributionValues, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    List<Attribution> result = new List<Attribution>();
                    ReadProjectAttributionValues(reader, result);
                    return result;
                }
            }
        }

        public static void ReadProjectAttributionValues(SqlDataReader reader, List<Attribution> result)
        {
            try
            {
                if (!reader.HasRows) return;
                int attributionIdIndex = reader.GetOrdinal(Constants.ColumnNames.AttributionId);
                int attributionTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.AttributionTypeId);
                int attributionRecordIdIndex = reader.GetOrdinal(Constants.ColumnNames.AttributionRecordId);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDateColumn);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDateColumn);
                int targetIdIndex = reader.GetOrdinal(Constants.ColumnNames.TargetId);
                int targetNameIndex = reader.GetOrdinal(Constants.ColumnNames.TargetName);
                int percentageIndex = reader.GetOrdinal(Constants.ColumnNames.Percentage);
                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);

                while (reader.Read())
                {
                    int targetId = reader.GetInt32(targetIdIndex);
                    string targetName = reader.GetString(targetNameIndex);
                    AttributionTypes attributionType = (AttributionTypes)reader.GetInt32(attributionTypeIdIndex);
                    AttributionRecordTypes attributionRecordType = (AttributionRecordTypes)reader.GetInt32(attributionRecordIdIndex);
                    int titleId = !reader.IsDBNull(titleIdIndex) ? (int)reader.GetInt32(titleIdIndex) : -1;
                    Attribution attribution = new Attribution()
                    {
                        Id = reader.GetInt32(attributionIdIndex),
                        AttributionType = attributionType,
                        AttributionRecordType = attributionRecordType,
                        CommissionPercentage = reader.GetDecimal(percentageIndex),
                        EndDate = !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null,
                        StartDate = !reader.IsDBNull(startDateIndex) ? (DateTime?)reader.GetDateTime(startDateIndex) : null,
                        TargetId = targetId,
                        TargetName = targetName
                    };
                    if (titleId != -1)
                    {
                        attribution.Title = new Title()
                        {
                            TitleId = titleId,
                            TitleName = reader.GetString(titleIndex)
                        };
                    }
                    else
                    {
                        attribution.Title = null;
                    }
                    result.Add(attribution);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SetProjectAttributionValues(int projectId, string attributionXML, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Project.SetProjectAttributionValues, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, projectId);
                command.Parameters.AddWithValue(Constants.ParameterNames.AttributionXML, attributionXML);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static List<Project> GetAttributionForGivenIds(string attributionIds)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Project.GetAttributionForGivenIds, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.AttributionIds, attributionIds);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    List<Project> result = new List<Project>();
                    ReadAttributionForGivenIds(reader, result);
                    return result;
                }
            }
        }

        public static void ReadAttributionForGivenIds(SqlDataReader reader, List<Project> result)
        {
            try
            {
                if (!reader.HasRows) return;
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                int attributionIdIndex = reader.GetOrdinal(Constants.ColumnNames.AttributionId);
                int attributionTypeIdIndex = reader.GetOrdinal(Constants.ColumnNames.AttributionTypeId);
                int attributionRecordIdIndex = reader.GetOrdinal(Constants.ColumnNames.AttributionRecordTypeId);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDateColumn);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDateColumn);
                int targetIdIndex = reader.GetOrdinal(Constants.ColumnNames.TargetId);
                while (reader.Read())
                {
                    Project project;
                    string projectNumber = reader.GetString(projectNumberIndex);
                    if (result.Any(p => p.ProjectNumber == projectNumber))
                    {
                        project = result.First(p => p.ProjectNumber == projectNumber);
                    }
                    else
                    {
                        project = new Project()
                        {
                            ProjectNumber = projectNumber,
                            Name = reader.GetString(projectNameIndex)
                        };
                        project.AttributionList = new List<Attribution>();
                        result.Add(project);
                    }
                    int targetId = reader.GetInt32(targetIdIndex);
                    AttributionTypes attributionType = (AttributionTypes)reader.GetInt32(attributionTypeIdIndex);
                    AttributionRecordTypes attributionRecordType = (AttributionRecordTypes)reader.GetInt32(attributionRecordIdIndex);
                    Attribution attribution = new Attribution()
                    {
                        Id = reader.GetInt32(attributionIdIndex),
                        AttributionType = attributionType,
                        AttributionRecordType = attributionRecordType,
                        EndDate = !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null,
                        StartDate = !reader.IsDBNull(startDateIndex) ? (DateTime?)reader.GetDateTime(startDateIndex) : null,
                        TargetId = targetId
                    };
                    project.AttributionList.Add(attribution);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Project Feedback

        public static List<ProjectFeedback> GetProjectFeedbackByProjectId(int projectId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Project.GetProjectFeedbackByProjectId, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectId, projectId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    List<ProjectFeedback> result = new List<ProjectFeedback>();
                    ReadProjectFeedback(reader, result);
                    return result;
                }
            }
        }

        public static void ReadProjectFeedback(SqlDataReader reader, List<ProjectFeedback> result)
        {
            try
            {
                if (!reader.HasRows) return;
                int feedbackIdIndex = reader.GetOrdinal(Constants.ColumnNames.FeedbackId);
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                int reviewStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.ReviewStartDate);
                int reviewEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.ReviewEndDate);
                int dueDateIndex = reader.GetOrdinal(Constants.ColumnNames.DueDate);
                int feedbackStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.FeedbackStatusId);
                int feedbackStatusIndex = reader.GetOrdinal(Constants.ColumnNames.FeedbackStatus);
                int completionCertificateByIndex = reader.GetOrdinal(Constants.ColumnNames.CompletionCertificateBy);
                int completionCertificateDateIndex = reader.GetOrdinal(Constants.ColumnNames.CompletionCertificateDate);
                int isCanceledIndex = reader.GetOrdinal(Constants.ColumnNames.IsCanceled);
                int cancelationReasonIndex = reader.GetOrdinal(Constants.ColumnNames.CancelationReason);
                int isGapIndex = reader.GetOrdinal(Constants.ColumnNames.IsGap);

                while (reader.Read())
                {
                    ProjectFeedback feedback = new ProjectFeedback()
                    {
                        Id = reader.GetInt32(feedbackIdIndex),
                        Person = new Person()
                        {
                            Id = reader.GetInt32(personIdIndex),
                            FirstName = reader.GetString(firstNameIndex),
                            LastName = reader.GetString(lastNameIndex),
                            Title = new Title()
                            {
                                TitleId = !reader.IsDBNull(titleIdIndex) ? reader.GetInt32(titleIdIndex) : -1,
                                TitleName = !reader.IsDBNull(titleIndex) ? reader.GetString(titleIndex) : string.Empty
                            }
                        },
                        ReviewStartDate = reader.GetDateTime(reviewStartDateIndex),
                        ReviewEndDate = reader.GetDateTime(reviewEndDateIndex),
                        DueDate = reader.GetDateTime(dueDateIndex),
                        Status = new ProjectFeedbackStatus()
                        {
                            Id = reader.GetInt32(feedbackStatusIdIndex),
                            Name = reader.GetString(feedbackStatusIndex)
                        },
                        CompletionCertificateBy = !reader.IsDBNull(completionCertificateByIndex) ? reader.GetString(completionCertificateByIndex) : string.Empty,
                        CompletionCertificateDate = !reader.IsDBNull(completionCertificateDateIndex) ? reader.GetDateTime(completionCertificateDateIndex) : DateTime.MinValue,
                        IsCanceled = reader.GetBoolean(isCanceledIndex),
                        CancelationReason = !reader.IsDBNull(cancelationReasonIndex) ? reader.GetString(cancelationReasonIndex) : string.Empty,
                        IsGap = reader.GetBoolean(isGapIndex)
                    };
                    result.Add(feedback);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ProjectFeedbackStatus> GetAllFeedbackStatuses()
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Project.GetAllFeedbackStatuses, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    List<ProjectFeedbackStatus> result = new List<ProjectFeedbackStatus>();
                    ReadProjectFeedbackStatus(reader, result);
                    return result;
                }
            }
        }

        public static void ReadProjectFeedbackStatus(SqlDataReader reader, List<ProjectFeedbackStatus> result)
        {
            try
            {
                if (!reader.HasRows) return;
                int feedbackStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.FeedbackStatusId);
                int feedbackStatusIndex = reader.GetOrdinal(Constants.ColumnNames.Name);

                while (reader.Read())
                {
                    ProjectFeedbackStatus feedback = new ProjectFeedbackStatus()
                    {
                        Id = reader.GetInt32(feedbackStatusIdIndex),
                        Name = reader.GetString(feedbackStatusIndex)
                    };
                    result.Add(feedback);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SaveFeedbackCancelationDetails(int feedbackId, int? statusId, bool isCanceled, string cancelationReason, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Project.SaveFeedbackCancelationDetails, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.FeedbackId, feedbackId);
                command.Parameters.AddWithValue(Constants.ParameterNames.StatusId, statusId.HasValue ? (object)statusId : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.IsCanceled, isCanceled);
                command.Parameters.AddWithValue(Constants.ParameterNames.CancelationReason, cancelationReason);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static bool CheckIfFeedbackExists(int? milestonePersonId, int? milestoneId, DateTime? startDate, DateTime? endDate)
        {
            bool result;
            try
            {
                using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.CheckIfFeedbackExists
                                                    , connection
                                                   )
                      )
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.MilestonePersonId, milestonePersonId.HasValue ? (object)milestonePersonId : DBNull.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.MilestoneId, milestoneId.HasValue ? (object)milestoneId : DBNull.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.StartDate, startDate.HasValue ? (object)startDate : DBNull.Value);
                    command.Parameters.AddWithValue(Constants.ParameterNames.EndDate, endDate.HasValue ? (object)endDate : DBNull.Value);

                    connection.Open();
                    result = Convert.ToBoolean(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                result = true;
            }

            return result;
        }

        public static bool CheckIfProjectNumberExists(string projectNumber)
        {
            bool result;
            try
            {
                using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
                using (var command = new SqlCommand(Constants.ProcedureNames.Project.CheckIfProjectNumberExists
                                                    , connection
                                                   )
                      )
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectNumber, !string.IsNullOrEmpty(projectNumber) ? (object)projectNumber : DBNull.Value);

                    connection.Open();
                    result = Convert.ToBoolean(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                result = true;
            }

            return result;
        }

        public static List<ProjectFeedbackMail> GetPersonsForIntialMailForProjectFeedback(int? feedbackId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Project.GetPersonsForIntialMailForProjectFeedback, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.FeedbackId, feedbackId.HasValue ? (object)feedbackId : DBNull.Value);
                command.CommandTimeout = connection.ConnectionTimeout;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    List<ProjectFeedbackMail> result = new List<ProjectFeedbackMail>();
                    ReadProjectFeedback(reader, result);
                    return result;
                }
            }
        }

        public static List<ProjectFeedbackMail> GetPersonsForProjectReviewCanceled(int personId, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Project.GetPersonsForProjectReviewCanceled, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                command.Parameters.AddWithValue(Constants.ParameterNames.UserLoginParam, userLogin);
                command.CommandTimeout = connection.ConnectionTimeout;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    List<ProjectFeedbackMail> result = new List<ProjectFeedbackMail>();
                    ReadProjectFeedback(reader, result);
                    return result;
                }
            }
        }

        private static void ReadProjectFeedback(SqlDataReader reader, List<ProjectFeedbackMail> feedbacks)
        {
            if (reader.HasRows)
            {
                int firstNameIndex = reader.GetOrdinal("FirstName");
                int lastNameIndex = reader.GetOrdinal("LastName");
                int personIdIndex = reader.GetOrdinal("PersonId");
                int titleIdIndex = reader.GetOrdinal("TitleId");
                int titleIndex = reader.GetOrdinal("Title");
                int reviewStartDateIndex = reader.GetOrdinal("ReviewPeriodStartDate");
                int reviewEndDateIndex = reader.GetOrdinal("ReviewPeriodEndDate");
                int dueDateIndex = reader.GetOrdinal("DueDate");
                int projectIdIndex = reader.GetOrdinal("ProjectId");
                int projectNameIndex = reader.GetOrdinal("ProjectName");
                int projectNumberIndex = reader.GetOrdinal("ProjectNumber");
                int projectManagersAliasesIndex = reader.GetOrdinal("ToAddressList");
                int directorAliasIndex = reader.GetOrdinal("DirectorAlias");
                int projectOwnerAliasIndex = reader.GetOrdinal("ProjectOwnerAlias");
                int seniorManagerAliasIndex = reader.GetOrdinal("SeniorManagerAlias");

                while (reader.Read())
                {
                    var projectId = reader.GetInt32(projectIdIndex);
                    var feedback = new ProjectFeedback
                    {
                        Person = new Person()
                        {
                            Id = reader.GetInt32(personIdIndex),
                            FirstName = reader.GetString(firstNameIndex),
                            LastName = reader.GetString(lastNameIndex),
                            Title = new Title()
                            {
                                TitleId = !reader.IsDBNull(titleIdIndex) ? reader.GetInt32(titleIdIndex) : -1,
                                TitleName = !reader.IsDBNull(titleIndex) ? reader.GetString(titleIndex) : string.Empty
                            }
                        },
                        ReviewStartDate = reader.GetDateTime(reviewStartDateIndex),
                        ReviewEndDate = reader.GetDateTime(reviewEndDateIndex),
                        DueDate = reader.GetDateTime(dueDateIndex)
                    };
                    if (feedbacks.Any(p => p.Project.Id.Value == projectId))
                    {
                        var mail = feedbacks.FirstOrDefault(p => p.Project.Id.Value == projectId);
                        mail.Resources.Add(feedback);
                    }
                    else
                    {
                        var feedbackMail = new ProjectFeedbackMail()
                        {
                            Project = new Project()
                            {
                                Id = reader.GetInt32(projectIdIndex),
                                Name = reader.GetString(projectNameIndex),
                                ProjectNumber = reader.GetString(projectNumberIndex)
                            },
                            ProjectManagersAliasList = !reader.IsDBNull(projectManagersAliasesIndex) ? reader.GetString(projectManagersAliasesIndex) : string.Empty,
                            ClientDirectorAlias = !reader.IsDBNull(directorAliasIndex) ? reader.GetString(directorAliasIndex) : string.Empty,
                            ProjectOwnerAlias = !reader.IsDBNull(projectOwnerAliasIndex) ? reader.GetString(projectOwnerAliasIndex) : string.Empty,
                            SeniorManagerAlias = !reader.IsDBNull(seniorManagerAliasIndex) ? reader.GetString(seniorManagerAliasIndex) : string.Empty,
                            Resources = new List<ProjectFeedback>()
                            {
                                feedback
                            }
                        };
                        feedbacks.Add(feedbackMail);
                    }
                }
            }
        }

        #endregion

        public static Project ProjectGetShortById(int projectId)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.ProjectGetShortById, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.ProjectIdParam, projectId);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    return ProjectDAL.ReadProjectShortById(reader);
                }
            }
        }

        private static Project ReadProjectShortById(SqlDataReader reader)
        {
            var project = new Project();
            if (reader.HasRows)
            {
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.NameColumn);
                int startDateIndex = reader.GetOrdinal(Constants.ColumnNames.StartDateColumn);
                int endDateIndex = reader.GetOrdinal(Constants.ColumnNames.EndDateColumn);
                int projectStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusIdColumn);
                int projectStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStatusNameColumn);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumberColumn);
                int ToAddressListIndex = -1;
                try
                {
                    ToAddressListIndex = reader.GetOrdinal(Constants.ColumnNames.ToAddressList);
                }
                catch
                { }
                while (reader.Read())
                {
                    var project1 = new Project
                    {
                        Id = reader.GetInt32(projectIdIndex),
                        Name = reader.GetString(nameIndex),
                        StartDate =
                            !reader.IsDBNull(startDateIndex) ? (DateTime?)reader.GetDateTime(startDateIndex) : null,
                        EndDate =
                            !reader.IsDBNull(endDateIndex) ? (DateTime?)reader.GetDateTime(endDateIndex) : null,
                        ProjectNumber = reader.GetString(projectNumberIndex),
                        Status = new ProjectStatus
                        {
                            Id = reader.GetInt32(projectStatusIdIndex),
                            Name = reader.GetString(projectStatusNameIndex)
                        }
                    };
                    if (ToAddressListIndex > -1)
                    {
                        project1.ToAddressList = !reader.IsDBNull(ToAddressListIndex) ? reader.GetString(ToAddressListIndex) : string.Empty;
                    }
                    project = project1;
                }
            }
            return project;
        }

        public static List<Project> PersonsByProjectReport(string accountIds, string payTypeIds, string personStatusIds, string practices, string projectStatusIds, bool excludeInternal)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Project.PersonsByProjectReport, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                command.Parameters.AddWithValue(Constants.ParameterNames.AccountIdsParam, accountIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.PayTypeIds, payTypeIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonStatusIds, personStatusIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.Practices, practices);
                command.Parameters.AddWithValue(Constants.ParameterNames.ProjectStatusIdsParam, projectStatusIds);
                command.Parameters.AddWithValue(Constants.ParameterNames.ExcludeInternalPractices, excludeInternal);
                command.CommandTimeout = connection.ConnectionTimeout;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<Project>();
                    ReadPersonsByProjectReport(reader, result);
                    return result;
                }
            }
        }

        private static void ReadPersonsByProjectReport(SqlDataReader reader, List<Project> projects)
        {
            if (reader.HasRows)
            {
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectIdColumn);
                int projectNumberIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectNumber);
                int projectNameIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectName);
                int projectStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectStartDate);
                int ProjectEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectEndDate);
                int milestoneIdIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneId);
                int milestoneStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneStartDate);
                int milestoneEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneEndDate);
                int descriptionColumnIndex = reader.GetOrdinal(Constants.ColumnNames.DescriptionColumn);
                int milestoneResourceStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneResourceStartDate);
                int milestoneResourceEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.MilestoneResourceEndDate);
                int personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                int firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                int lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                int titleIdIndex = reader.GetOrdinal(Constants.ColumnNames.TitleId);
                int titleIndex = reader.GetOrdinal(Constants.ColumnNames.Title);
                int personStatusIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusId);
                int personStatusNameIndex = reader.GetOrdinal(Constants.ColumnNames.PersonStatusName);
                int badgeStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeStartDate);
                int badgeEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BadgeEndDate);
                int organicBreakStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.OrganicBreakStartDate);
                int organicBreakEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.OrganicBreakEndDate);
                int blockStartDateIndex = reader.GetOrdinal(Constants.ColumnNames.BlockStartDate);
                int blockEndDateIndex = reader.GetOrdinal(Constants.ColumnNames.BlockEndDate);

                while (reader.Read())
                {
                    var projectId = reader.GetInt32(projectIdIndex);
                    var personId = reader.GetInt32(personIdIndex);
                    var milestoneId = reader.GetInt32(milestoneIdIndex);
                    var person = new Person()
                    {
                        Id = personId,
                        FirstName = reader.GetString(firstNameIndex),
                        LastName = reader.GetString(lastNameIndex),
                        Title = new Title()
                        {
                            TitleId = reader.GetInt32(titleIdIndex),
                            TitleName = reader.GetString(titleIndex)
                        },
                        Badge = new MSBadge()
                        {
                            BadgeStartDate = !reader.IsDBNull(badgeStartDateIndex) ? (DateTime?)reader.GetDateTime(badgeStartDateIndex) : null,
                            BadgeEndDate = !reader.IsDBNull(badgeEndDateIndex) ? (DateTime?)reader.GetDateTime(badgeEndDateIndex) : null,
                            OrganicBreakStartDate = !reader.IsDBNull(organicBreakStartDateIndex) ? (DateTime?)reader.GetDateTime(organicBreakStartDateIndex) : null,
                            OrganicBreakEndDate = !reader.IsDBNull(organicBreakEndDateIndex) ? (DateTime?)reader.GetDateTime(organicBreakEndDateIndex) : null,
                            BlockStartDate = !reader.IsDBNull(blockStartDateIndex) ? (DateTime?)reader.GetDateTime(blockStartDateIndex) : null,
                            BlockEndDate = !reader.IsDBNull(blockEndDateIndex) ? (DateTime?)reader.GetDateTime(blockEndDateIndex) : null
                        },
                        Status = new PersonStatus()
                        {
                            Id = reader.GetInt32(personStatusIdIndex),
                            Name = reader.GetString(personStatusNameIndex)
                        },
                        ResourceStartDate = reader.GetDateTime(milestoneResourceStartDateIndex),
                        ResourceEndDate = reader.GetDateTime(milestoneResourceEndDateIndex)
                    };
                    var milestone = new Milestone()
                    {
                        Id = milestoneId,
                        StartDate = reader.GetDateTime(milestoneStartDateIndex),
                        ProjectedDeliveryDate = reader.GetDateTime(milestoneEndDateIndex),
                        Description = reader.GetString(descriptionColumnIndex),
                        People = new List<Person>() { person }
                    };
                    if (projects.Any(p => p.Id == projectId))
                    {
                        var project = projects.FirstOrDefault(p => p.Id == projectId);
                        if (project.Milestones.Any(m => m.Id == milestoneId))
                        {
                            var projectMilestone = project.Milestones.FirstOrDefault(m => m.Id == milestoneId);
                            projectMilestone.People.Add(person);
                        }
                        else
                        {
                            project.Milestones.Add(milestone);
                        }
                    }
                    else
                    {
                        var project = new Project()
                        {
                            Id = projectId,
                            ProjectNumber = reader.GetString(projectNumberIndex),
                            Name = reader.GetString(projectNameIndex),
                            StartDate = reader.GetDateTime(projectStartDateIndex),
                            EndDate = reader.GetDateTime(ProjectEndDateIndex),
                            Milestones = new List<Milestone>() { milestone }
                        };
                        projects.Add(project);
                    }
                }
            }
        }

        public static void InsertTodayProjectsIntoCache()
        {
            var connection = new SqlConnection(DataSourceHelper.DataConnection);

            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.InsertTodayProjectsIntoCache, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static List<Offering> GetProjectOfferingList()
        {
            List<Offering> result = null;
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.getOfferingsList, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    result = new List<Offering>();
                    ReadOfferingList(reader, result);
                }
            }
            return result;
        }

        private static void ReadOfferingList(SqlDataReader reader, List<Offering> result)
        {
            if (reader.HasRows)
            {
                int idIndex = reader.GetOrdinal(Constants.ColumnNames.OfferingId);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.OfferingName);
                while (reader.Read())
                {
                    Offering offering = new Offering();
                    offering.Id = reader.GetInt32(idIndex);
                    offering.Name = reader.GetString(nameIndex);
                    result.Add(offering);
                }
            }
        }

        public static List<Revenue> GetProjectRevenueTypeList()
        {
            List<Revenue> result = null;
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.GetRevenueTypesList, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    result = new List<Revenue>();
                    ReadRevenueTypeList(reader, result);
                }
            }
            return result;
        }

        private static void ReadRevenueTypeList(SqlDataReader reader, List<Revenue> result)
        {
            if (reader.HasRows)
            {
                int idIndex = reader.GetOrdinal(Constants.ColumnNames.RevenueTypeId);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.RevenueName);
                int isDefaultIndex = reader.GetOrdinal(Constants.ColumnNames.IsDefault);
                while (reader.Read())
                {
                    Revenue revenue = new Revenue();
                    revenue.Id = reader.GetInt32(idIndex);
                    revenue.Name = reader.GetString(nameIndex);
                    revenue.IsDefault = reader.GetBoolean(isDefaultIndex);
                    result.Add(revenue);
                }
            }
        }

        public static List<Channel> GetChannelList()
        {
            List<Channel> result = null;
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.GetChannelsList, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    result = new List<Channel>();
                    ReadChannelList(reader, result);
                }
            }
            return result;
        }

        private static void ReadChannelList(SqlDataReader reader, List<Channel> result)
        {
            if (reader.HasRows)
            {
                int idIndex = reader.GetOrdinal(Constants.ColumnNames.ChannelId);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.ChannelName);
                int isDefaultIndex = reader.GetOrdinal(Constants.ColumnNames.IsDefault);
                while (reader.Read())
                {
                    Channel channel = new Channel();
                    channel.Id = reader.GetInt32(idIndex);
                    channel.Name = reader.GetString(nameIndex);
                    channel.IsDefault = reader.GetBoolean(isDefaultIndex);
                    result.Add(channel);
                }
            }
        }

        public static Channel GetChannelById(int channelId)
        {
            Channel result = null;
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.GetChannelById, connection))
            {
                command.Parameters.AddWithValue(Constants.ParameterNames.ChannelId, channelId);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    result = new Channel();
                    ReadChannel(reader, result);
                }
            }
            return result;
        }

        private static void ReadChannel(SqlDataReader reader, Channel result)
        {
            if (reader.HasRows)
            {
                int idIndex = reader.GetOrdinal(Constants.ColumnNames.ChannelId);
                int nameIndex = reader.GetOrdinal(Constants.ColumnNames.ChannelName);
                int IsSubChannelNamePickerIndex = reader.GetOrdinal(Constants.ColumnNames.IsSubChannelNamePicker);
                while (reader.Read())
                {
                    result.Id = reader.GetInt32(idIndex);
                    result.Name = reader.GetString(nameIndex);
                    result.IsSubChannelNamePicker = reader.GetBoolean(IsSubChannelNamePickerIndex);
                }
            }
        }

        public static List<ProjectDivision> GetProjectDivisions()
        {
            List<ProjectDivision> result = null;
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.GetProjectDivisions, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    result = new List<ProjectDivision>();
                    ReadProjectDivisions(reader, result);
                }
            }
            return result;
        }

        private static void ReadProjectDivisions(SqlDataReader reader, List<ProjectDivision> result)
        {
            if (reader.HasRows)
            {
                int divisionIdIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionId);
                int divisionNameIndex = reader.GetOrdinal(Constants.ColumnNames.DivisionName);
                int isExternalIndex = reader.GetOrdinal(Constants.ColumnNames.IsExternal);
                while (reader.Read())
                {
                    ProjectDivision division = new ProjectDivision();
                    division.Id = reader.GetInt32(divisionIdIndex);
                    division.Name = reader.GetString(divisionNameIndex);
                    division.IsExternal = reader.GetBoolean(isExternalIndex);
                    result.Add(division);
                }
            }
        }

        public static List<Project> GetProjectsForClients(string clientIds)
        {
            List<Project> result = null;
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.Project.GetProjectsForClients, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.ClientIdsParam, clientIds ?? (Object)DBNull.Value);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    result = new List<Project>();
                    ReadProjectsForClient(reader, result);
                }
            }
            return result;

        }

        private static void ReadProjectsForClient(SqlDataReader reader, List<Project> result)
        {
            if (reader.HasRows)
            {
                int projectIdIndex = reader.GetOrdinal(Constants.ColumnNames.ProjectId);
                int ProjectNameIndex = reader.GetOrdinal(Constants.ColumnNames.Name);
                while (reader.Read())
                {
                    Project project = new Project();
                    project.Id = reader.GetInt32(projectIdIndex);
                    project.Name = reader.GetString(ProjectNameIndex);

                    result.Add(project);
                }
            }
        }
    }
}


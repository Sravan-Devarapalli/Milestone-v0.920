using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel.Activation;
using System.Text;
using System.Xml;
using DataAccess;
using DataAccess.Other;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;

namespace PracticeManagementService
{
    // NOTE: If you change the class name "ProjectService" here, you must also update the reference to "ProjectService" in Web.config.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ProjectService : IProjectService
    {
        #region IProjectService Members

        public Project ProjectGetById(int projectId)
        {
            try
            {
                var project = ProjectDAL.GetById(projectId, null, null);
                return project;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "ProjectGetById", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }
        public ComputedFinancials GetProjectsComputedFinancials(int projectId)
        {
            try
            {
                return ComputedFinancialsDAL.FinancialsGetByProject(projectId);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetProjectsComputedFinancials", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        public DataSet GetProjectMilestonesFinancials(int projectId)
        {
            try
            {
                return ProjectDAL.GetProjectMilestonesFinancials(projectId);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetProjectMilestonesFinancials", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        /// <summary>
        /// Enlists number of requested projects by client.
        /// </summary>
        public int ProjectCountByClient(int clientId)
        {
            try
            {
                return ProjectDAL.ProjectCountByClient(clientId);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "ProjectCountByClient", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        /// <summary>
        /// Enlists the requested projects.
        /// </summary>
        /// <returns>The list of the projects.</returns>
        public List<Project> GetProjectListAll()
        {
            try
            {
                int? salespersonId = null;
                int? practiceManagerId = null;

                return ProjectDAL.ProjectListAll(null,
                    true,
                    false,
                    false,
                    true,
                    DateTime.MinValue,
                    DateTime.MaxValue,
                    salespersonId,
                    practiceManagerId,
                    null,
                    null);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetProjectListAll", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        public List<Project> ListProjectsByClient(int? clientId, string viewerUsername)
        {
            try
            {
                return clientId != null ? ProjectDAL.ProjectListByClient(clientId.Value, viewerUsername) : null;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "ListProjectsByClient", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        public List<Project> ListProjectsByClientShort(int? clientId, bool IsOnlyActiveAndProjective)
        {
            try
            {
                return clientId != null ? ProjectDAL.ListProjectsByClientShort(clientId.Value, IsOnlyActiveAndProjective) : null;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "ListProjectsByClientShort", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        public List<Project> ListProjectsByClientWithSort(int? clientId, string viewerUsername, string sortBy)
        {
            try
            {
                return clientId != null ? ProjectDAL.ProjectListByClientWithSorting(clientId.Value, viewerUsername, sortBy) : null;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "ListProjectsByClientWithSort", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        public int CloneProject(ProjectCloningContext context)
        {
            try
            {
                return ProjectDAL.CloneProject(context);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "CloneProject", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        /// <summary>
        /// Enlists the requested projects.
        /// </summary>
        public List<Project> GetProjectListCustom(bool projected, bool completed, bool active, bool experimantal)
        {

            try
            {
                return ProjectDAL.ProjectListAll(
                null,
                projected,
                completed,
                active,
                experimantal,
                DateTime.MinValue,
                DateTime.MaxValue,
                null,
                null,
                null,
                null,
                false);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetProjectListCustom", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }


        }

        /// <summary>
        /// Enlists the requested projects.
        /// </summary>
        /// <param name="clientId">An ID of the client the projects belong to.</param>
        /// <param name="showProjected">If true - the projected projects will be included in the results.</param>
        /// <param name="showCompleted">If true - the completed projects will be included in the results.</param>
        /// <param name="showActive">If true - the active (statusName=Active) projects will be included in the results.</param>
        /// <param name="showExperimental">If true - the experimantal projects will are included in the results.</param>
        /// <param name="periodStart">The start of the period to enlist the projects within.</param>
        /// <param name="periodEnd">The end of the period to enlist the projects within.</param>
        /// <param name="userName">The user (by email) to retrive the result for.</param>
        /// <param name="salespersonId">Determines an ID of the salesperson to filter the list for.</param>
        /// <param name="practiceManagerId">Determines an ID of the practice manager to filter the list for.</param>
        /// <param name="projectGroupId"></param>
        /// <param name="includeCurentYearFinancials">
        /// Determines the financial indexes for the current year need to be included into the result.
        /// </param>
        /// <param name="practiceId"></param>
        /// <returns>The list of the projects are match with the specified conditions.</returns>
        public List<Project> GetProjectList(int? clientId,
            bool showProjected,
            bool showCompleted,
            bool showActive,
            bool showExperimental,
            DateTime periodStart,
            DateTime periodEnd,
            string userName,
            int? salespersonId,
            int? practiceManagerId,
            int? practiceId,
            int? projectGroupId,
            ProjectCalculateRangeType includeCurentYearFinancials)
        {
            try
            {
                List<Project> result =
                ProjectRateCalculator.GetProjectList(
                    clientId,
                    showProjected,
                    showCompleted,
                    showActive,
                    showExperimental,
                    periodStart,
                    periodEnd,
                    salespersonId,
                    practiceManagerId,
                    practiceId,
                    projectGroupId,
                    includeCurentYearFinancials);

                return result;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetProjectList", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }


        }

        /// <summary>
        /// Enlists the requested projects.
        /// </summary>
        /// <param name="clientIds">Comma separated list of client ids. Null value means all clients.</param>
        /// <param name="showProjected">If true - the projected projects will be included in the results.</param>
        /// <param name="showCompleted">If true - the completed projects will be included in the results.</param>
        /// <param name="showActive">If true - the active (statusName=Active) projects will be included in the results.</param>
        /// <param name="showExperimental">If true - the experimantal projects will are included in the results.</param>
        /// <param name="periodStart">The start of the period to enlist the projects within.</param>
        /// <param name="periodEnd">The end of the period to enlist the projects within.</param>
        /// <param name="userName">The user (by email) to retrive the result for.</param>
        /// <param name="projectGroupIdsList"></param>
        /// <param name="includeCurentYearFinancials">
        /// Determines the financial indexes for the current year need to be included into the result.
        /// </param>
        /// <param name="salespersonIdsList"></param>
        /// <param name="projectOwnerIdsList"></param>
        /// <param name="practiceIdsList"></param>
        /// <returns>The list of the projects are match with the specified conditions.</returns>
        public List<Project> ProjectListAllMultiParameters(
            string clientIds,
            bool showProjected,
            bool showCompleted,
            bool showActive,
            bool showInternal,
            bool showExperimental,
            bool showInactive,
            DateTime periodStart,
            DateTime periodEnd,
            string userName,
            string salespersonIdsList,
            string projectOwnerIdsList,
            string practiceIdsList,
            string projectGroupIdsList,
            ProjectCalculateRangeType includeCurentYearFinancials,
            bool excludeInternalPractices)
        {

            try
            {
                List<Project> result =
               ProjectRateCalculator.GetProjectListMultiParameters(
                   clientIds,
                   showProjected,
                   showCompleted,
                   showActive,
                   showInternal,
                   showExperimental,
                   showInactive,
                   periodStart,
                   periodEnd,
                   salespersonIdsList,
                   projectOwnerIdsList,
                   practiceIdsList,
                   projectGroupIdsList,
                   includeCurentYearFinancials,
                   excludeInternalPractices);

                return result;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "ProjectListAllMultiParameters", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }


        public List<Project> GetProjectListByDateRange(
                                                    bool showProjected,
                                                    bool showCompleted,
                                                    bool showActive,
                                                    bool showInternal,
                                                    bool showExperimental,
                                                    bool showInactive,
                                                    DateTime periodStart,
                                                    DateTime periodEnd
                                                    )
        {

            try
            {
                List<Project> result =
               ProjectDAL.GetProjectListByDateRange(
                                                   showProjected,
                                                   showCompleted,
                                                   showActive,
                                                   showInternal,
                                                   showExperimental,
                                                   showInactive,
                                                   periodStart,
                                                   periodEnd
                                                   );

                return result;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetProjectListByDateRange", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }


        }

        public List<Project> GetProjectListWithFinancials(
            string clientIds,
            bool showProjected,
            bool showCompleted,
            bool showActive,
            bool showInternal,
            bool showExperimental,
            bool showInactive,
            DateTime periodStart,
            DateTime periodEnd,
            string salespersonIdsList,
            string projectOwnerIdsList,
            string practiceIdsList,
            string projectGroupIdsList,
            bool excludeInternalPractices
            )
        {

            try
            {
                List<Project> result =
                 ProjectDAL.GetProjectListWithFinancials(
                  clientIds,
              showProjected,
              showCompleted,
              showActive,
              showInternal,
              showExperimental,
              showInactive,
              periodStart,
              periodEnd,
              salespersonIdsList,
              projectOwnerIdsList,
              practiceIdsList,
              projectGroupIdsList,
              excludeInternalPractices);
                return result;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetProjectListWithFinancials", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }


        }

        public List<MilestonePerson> GetProjectListGroupByPracticeManagers(
            string clientIds,
            bool showProjected,
            bool showCompleted,
            bool showActive,
            bool showInternal,
            bool showExperimental,
            bool showInactive,
            DateTime periodStart,
            DateTime periodEnd,
            string salespersonIdsList,
            string projectOwnerIdsList,
            string practiceIdsList,
            string projectGroupIdsList,
            bool excludeInternalPractices
            )
        {

            try
            {
                return ProjectDAL.GetProjectListGroupByPracticeManagers(clientIds,
                                                                    showProjected,
                                                                    showCompleted,
                                                                    showActive,
                                                                    showInternal,
                                                                    showExperimental,
                                                                    showInactive,
                                                                    periodStart,
                                                                    periodEnd,
                                                                    salespersonIdsList,
                                                                    projectOwnerIdsList,
                                                                    practiceIdsList,
                                                                    projectGroupIdsList,
                                                                    excludeInternalPractices
                                                                    );
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetProjectListGroupByPracticeManagers", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }


        }

        public List<Project> GetBenchList(BenchReportContext context)
        {
            try
            {
                return ProjectRateCalculator.GetBenchAndAdmin(context);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetBenchList", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        public List<Project> GetBenchListWithoutBenchTotalAndAdminCosts(BenchReportContext context)
        {
            try
            {
                return ProjectRateCalculator.GetBenchListWithoutBenchTotalAndAdminCosts(context);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetBenchListWithoutBenchTotalAndAdminCosts", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        /// <summary>
        /// Enlists the requested projects.
        /// </summary>
        /// <param name="clientId">An ID of the client the projects belong to.</param>
        /// <param name="userName">The user (by email) to retrive the result for.</param>
        /// <returns>The list of the projects are belong to the specified client.</returns>
        public List<Project> GetProjectListByClient(int clientId, string userName)
        {

            try
            {
                int? salespersonId = null;
                int? practiceManagerId = null;

                // ProjectRateCalculator.VerifyPrivileges(userName, ref salespersonId, ref practiceManagerId);
                return ProjectDAL.ProjectListAll(clientId,
                    true,
                    true,
                    true,
                    true,
                    DateTime.MinValue,
                    DateTime.MaxValue,
                    salespersonId,
                    practiceManagerId,
                    null,
                    null,
                    false);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetProjectListByClient", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }


        }

        /// <summary>
        /// Retrives a list of the projects by the specified conditions.
        /// </summary>
        /// <param name="looked">A text to be looked for.</param>
        /// <param name="personId"></param>
        /// <returns>A list of the <see cref="Project"/> objects.</returns>
        public List<Project> ProjectSearchText(string looked, int personId)
        {
            try
            {
                return ProjectDAL.ProjectSearchText(looked, personId);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "ProjectSearchText", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        /// <summary>
        /// Reatrives a project with a specified ID.
        /// </summary>
        /// <param name="projectId">The ID of the requested project.</param>
        /// <param name="userName">The user (by email) to retrive the result for.</param>
        /// <returns>The <see cref="Project"/> record if found and null otherwise.</returns>
        public Project GetProjectDetail(int projectId, string userName)
        {
            try
            {
                Project result = new ProjectRateCalculator(projectId, userName).Project;

                if (result != null)
                {
                    if (result.Milestones != null)
                    {
                        foreach (Milestone milestone in result.Milestones)
                        {
                            milestone.ComputedFinancials =
                                ComputedFinancialsDAL.FinancialsGetByMilestone(milestone.Id.Value);
                        }
                    }

                    result.BillingInfo = GetProjectBillingInfo(projectId);
                }

                return result;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetProjectDetail", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public BillingInfo GetProjectBillingInfo(int projectId)
        {
            try
            {
                var billingInfo = ProjectBillingInfoDAL.ProjectBillingInfoGetById(projectId);

                return billingInfo;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetProjectBillingInfo", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        public Project GetProjectDetailWithoutMilestones(int projectId, string userName)
        {
            try
            {
                return ProjectRateCalculator.GetProjectDetail(projectId, userName);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetProjectDetailWithoutMilestones", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        /// <summary>
        /// Saves the <see cref="Project"/> data into the database.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> to be saved.</param>
        /// <param name="userName">A current user.</param>
        /// <returns>An ID of the saved project.</returns>
        public int SaveProjectDetail(Project project, string userName)
        {
            try
            {
                SqlTransaction currentTransaction = null;
                using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
                {
                    connection.Open();
                    currentTransaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                    if (!project.Id.HasValue)
                    {
                        ProjectDAL.InsertProject(project, userName, connection, currentTransaction);
                    }
                    else
                    {
                        ProjectDAL.UpdateProject(project, userName, connection, currentTransaction);
                    }

                    // Save project's commissions
                    if (project.SalesCommission != null)
                    {
                        project.SalesCommission.ForEach(delegate(Commission salesCommission)
                        {
                            salesCommission.ProjectWithMargin = project;
                            CommissionDAL.CommissionSet(salesCommission, connection, currentTransaction);
                        });
                    }
                    if (project.ManagementCommission != null)
                    {
                        project.ManagementCommission.ProjectWithMargin = project;
                        CommissionDAL.CommissionSet(project.ManagementCommission, connection, currentTransaction);
                    }
                    if (project.BillingInfo != null)
                    {
                        project.BillingInfo.Id = project.Id;
                        ProjectBillingInfoDAL.ProjectBillingInfoSave(project.BillingInfo, connection, currentTransaction);
                    }
                    else
                    {
                        ProjectBillingInfoDAL.ProjectBillingInfoDelete(project.Id.Value, connection, currentTransaction);
                    }

                    currentTransaction.Commit();
                }

                return project.Id.Value;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "SaveProjectDetail", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }


        }

        /// <summary>
        /// Provides an info for the month mini report.
        /// </summary>
        /// <param name="month">The month to the data be provided for.</param>
        /// <param name="userName">The user (by email) to retrive the result for.</param>
        /// <returns>A well-formed XML document with the report data.</returns>
        public string MonthMiniReport(
            DateTime month,
            string userName,
            bool showProjected,
            bool showCompleted,
            bool showActive,
            bool showExperimental,
            bool showInternal,
            bool showInactive)
        {
            try
            {
                var monthStart = new DateTime(month.Year, month.Month, 1);
                var monthEnd = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));

                var res = PersonStartsReport(monthStart, monthEnd, userName, null, null, showProjected, showCompleted, showActive, showExperimental, showInternal, showInactive);

                if (res.Count != 1)
                {
                    throw new Exception("'PersonStartsReport' method must return just one 'PesonStat' element for the month mini report");
                }

                var result = new StringBuilder();
                using (var writer = XmlWriter.Create(result))
                {
                    writer.WriteStartElement("Report");
                    writer.WriteAttributeString("Date", month.ToString("yyyy-MM-ddTHH:mm:ss"));
                    writer.WriteAttributeString("Revenue", res[0].Revenue.Value.ToString());
                    writer.WriteAttributeString("EmployeesCount", res[0].EmployeesCount.ToString());
                    writer.WriteAttributeString("ConsultantsCount", res[0].ConsultantsCount.ToString());
                }

                return result.ToString();
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "MonthMiniReport", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }


        }

        /// <summary>
        /// Retrives the data for the person stats report.
        /// </summary>
        /// <param name="startDate">The period start.</param>
        /// <param name="endDate">The period end.</param>
        /// <param name="userName">The user (by email) to retrive the result for.</param>
        /// <param name="salespersonId">Determines an ID of the salesperson to filter the list for.</param>
        /// <param name="practiceManagerId">Determines an ID of the practice manager to filter the list for.</param>
        /// <returns>The list of the <see cref="PersonStats"/> objects.</returns>
        public List<PersonStats> PersonStartsReport(DateTime startDate,
            DateTime endDate,
            string userName,
            int? salespersonId,
            int? practiceManagerId,
            bool showProjected,
            bool showCompleted,
            bool showActive,
            bool showExperimental,
            bool showInternal,
            bool showInactive
            )
        {

            try
            {
                // ProjectRateCalculator.VerifyPrivileges(userName, ref salespersonId, ref practiceManagerId);
                var result
                    = ComputedFinancialsDAL.PersonStatsByDateRange(
                        startDate, //startDate.AddMonths(-1), 
                        endDate,
                        salespersonId,
                        practiceManagerId,
                        showProjected,
                        showCompleted,
                        showActive,
                        showExperimental,
                        showInternal,
                        showInactive);

                for (var i = 1; i < result.Count; i++)
                {
                    var prevVirtualConsultants = result[i - 1].VirtualConsultants;
                    result[i].VirtualConsultantsChange = result[i].VirtualConsultants - prevVirtualConsultants;
                }

                // The data for a previous month was retrived to calculate the Virtual Consultants Change
                if (result.Count > 0 && result[0].Date < startDate)
                {
                    result.RemoveAt(0);
                }

                // Admin costs
                var allAdmin = ProjectRateCalculator.GetAdminCosts(startDate, endDate, userName);
                if (allAdmin != null)
                    foreach (var stats in result)
                        foreach (var financials in allAdmin.ProjectedFinancialsByMonth)
                            if (stats.Date.Month == financials.Key.Month && stats.Date.Year == financials.Key.Year)
                                stats.AdminCosts = financials.Value.Cogs;

                return result;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "PersonStartsReport", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public int? GetProjectId(string projectNumber)
        {
            try
            {
                return ProjectDAL.GetProjectId(projectNumber);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetProjectId", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }


        public List<ProjectsGroupedByPerson> PersonBudgetListByYear(int year, BudgetCategoryType categoryType)
        {
            try
            {
                return ProjectDAL.PersonBudgetListByYear(year, categoryType);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "PersonBudgetListByYear", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        public List<ProjectsGroupedByPractice> PracticeBudgetListByYear(int year)
        {
            try
            {
                return ProjectDAL.PracticeBudgetListByYear(year);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "PracticeBudgetListByYear", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        public void CategoryItemBudgetSave(int itemId, BudgetCategoryType categoryType, DateTime monthStartDate, PracticeManagementCurrency amount)
        {
            try
            {
                ProjectDAL.CategoryItemBudgetSave(itemId, categoryType, monthStartDate, amount);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "CategoryItemBudgetSave", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public List<ProjectsGroupedByPerson> CalculateBudgetForPersons(
            DateTime startDate,
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
            try
            {

                return ProjectDAL.CalculateBudgetForPersons(
                                                             startDate,
                                                             endDate,
                                                             showProjected,
                                                             showCompleted,
                                                             showActive,
                                                             showInternal,
                                                             showExperimental,
                                                             showInactive,
                                                             practiceIdsList,
                                                             excludeInternalPractices,
                                                             personIds,
                                                             categoryType);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "CalculateBudgetForPersons", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        public List<ProjectsGroupedByPractice> CalculateBudgetForPractices
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

            try
            {

                return ProjectDAL.CalculateBudgetForPractices(
                                                          startDate,
                                                          endDate,
                                                          showProjected,
                                                          showCompleted,
                                                          showActive,
                                                          showInternal,
                                                          showExperimental,
                                                          showInactive,
                                                          practiceIdsList,
                                                          excludeInternalPractices);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "CalculateBudgetForPractices", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        public void CategoryItemsSaveFromXML(List<CategoryItemBudget> categoryItems, int year)
        {
            try
            {

                ProjectDAL.CategoryItemsSaveFromXML(categoryItems, year);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "CategoryItemsSaveFromXML", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public void ProjectDelete(int projectId, string userName)
        {
            try
            {
                ProjectDAL.ProjectDelete(projectId, userName);//It will delete only Inactive and Experimental Projects as per #2702.
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "ProjectDelete", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        public ProjectExpense[] GetProjectExpensesForProject(ProjectExpense entity)
        {
            try
            {
                return (new ProjectExpenseDal()).GetForProject(entity);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "GetProjectExpensesForProject", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        public List<Project> AllProjectsWithFinancialTotalsAndPersons()
        {
            try
            {
                var projectsList = ProjectDAL.ProjectsAll();

                ComputedFinancialsDAL.LoadTotalFinancialsPeriodForProjects(projectsList, null, null);

                MilestonePersonDAL.LoadMilestonePersonListForProject(projectsList);

                return projectsList;
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "AllProjectsWithFinancialTotalsAndPersons", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        public bool IsUserHasPermissionOnProject(string user, int id, bool isProjectId)
        {
            try
            {
                return ProjectDAL.IsUserHasPermissionOnProject(user, id, isProjectId);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "IsUserHasPermissionOnProject", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }
        }

        public bool IsUserIsOwnerOfProject(string user, int id, bool isProjectId)
        {
            try
            {
                return ProjectDAL.IsUserIsOwnerOfProject(user, id, isProjectId);
            }
            catch (Exception e)
            {
                string logData = string.Format(Constants.Formatting.ErrorLogMessage, "IsUserHasPermissionOnProject", "ProjectService.svc", string.Empty,
                    e.Message, e.Source, e.InnerException == null ? string.Empty : e.InnerException.Message, e.InnerException == null ? string.Empty : e.InnerException.Source);
                ActivityLogDAL.ActivityLogInsert(20, logData);
                throw e;
            }

        }

        #endregion
    }
}



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
            return ProjectDAL.GetById(projectId, null, null);
        }
        public ComputedFinancials GetProjectsComputedFinancials(int projectId)
        {
            return ComputedFinancialsDAL.FinancialsGetByProject(projectId);
        }

        public DataSet GetProjectMilestonesFinancials(int projectId)
        {
            return ProjectDAL.GetProjectMilestonesFinancials(projectId);
        }

        /// <summary>
        /// Enlists number of requested projects by client.
        /// </summary>
        public int ProjectCountByClient(int clientId)
        {
            return ProjectDAL.ProjectCountByClient(clientId);
        }

        /// <summary>
        /// Enlists the requested projects.
        /// </summary>
        /// <returns>The list of the projects.</returns>
        public List<Project> GetProjectListAll()
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

        public List<Project> ListProjectsByClient(int? clientId, string viewerUsername)
        {
            return clientId != null ? ProjectDAL.ProjectListByClient(clientId.Value, viewerUsername) : null;
        }

        public List<Project> ListProjectsByClientShort(int? clientId, bool IsOnlyActiveAndProjective)
        {
            return clientId != null ? ProjectDAL.ListProjectsByClientShort(clientId.Value, IsOnlyActiveAndProjective) : null;
        }

        public List<Project> ListProjectsByClientWithSort(int? clientId, string viewerUsername, string sortBy)
        {
            return clientId != null ? ProjectDAL.ProjectListByClientWithSorting(clientId.Value, viewerUsername, sortBy) : null;
        }

        public int CloneProject(ProjectCloningContext context)
        {
            return ProjectDAL.CloneProject(context);
        }

        /// <summary>
        /// Enlists the requested projects.
        /// </summary>
        public List<Project> GetProjectListCustom(bool projected, bool completed, bool active, bool experimantal)
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
            bool includeCurentYearFinancials)
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
            bool includeCurentYearFinancials,
            bool excludeInternalPractices)
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

        public List<Project> GetBenchList(BenchReportContext context)
        {
            return ProjectRateCalculator.GetBenchAndAdmin(context);
        }

        public List<Project> GetBenchListWithoutBenchTotalAndAdminCosts(BenchReportContext context)
        {
            return ProjectRateCalculator.GetBenchListWithoutBenchTotalAndAdminCosts(context);
        }

        /// <summary>
        /// Enlists the requested projects.
        /// </summary>
        /// <param name="clientId">An ID of the client the projects belong to.</param>
        /// <param name="userName">The user (by email) to retrive the result for.</param>
        /// <returns>The list of the projects are belong to the specified client.</returns>
        public List<Project> GetProjectListByClient(int clientId, string userName)
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

        /// <summary>
        /// Retrives a list of the projects by the specified conditions.
        /// </summary>
        /// <param name="looked">A text to be looked for.</param>
        /// <param name="personId"></param>
        /// <returns>A list of the <see cref="Project"/> objects.</returns>
        public List<Project> ProjectSearchText(string looked, int personId)
        {
            return ProjectDAL.ProjectSearchText(looked, personId);
        }

        /// <summary>
        /// Reatrives a project with a specified ID.
        /// </summary>
        /// <param name="projectId">The ID of the requested project.</param>
        /// <param name="userName">The user (by email) to retrive the result for.</param>
        /// <returns>The <see cref="Project"/> record if found and null otherwise.</returns>
        public Project GetProjectDetail(int projectId, string userName)
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

        public BillingInfo GetProjectBillingInfo(int projectId)
        {
            var billingInfo = ProjectBillingInfoDAL.ProjectBillingInfoGetById(projectId);

            return billingInfo;
        }

        public Project GetProjectDetailWithoutMilestones(int projectId, string userName)
        {
            return ProjectRateCalculator.GetProjectDetail(projectId, userName);
        }

        /// <summary>
        /// Saves the <see cref="Project"/> data into the database.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> to be saved.</param>
        /// <param name="userName">A current user.</param>
        /// <returns>An ID of the saved project.</returns>
        public int SaveProjectDetail(Project project, string userName)
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

        public int? GetProjectId(string projectNumber)
        {
            return ProjectDAL.GetProjectId(projectNumber);
        }


        public List<ProjectsGroupedByPerson> PersonBudgetListByYear(int year, BudgetCategoryType categoryType)
        {
            return ProjectDAL.PersonBudgetListByYear(year, categoryType);
        }

        public List<ProjectsGroupedByPractice> PracticeBudgetListByYear(int year)
        {
            return ProjectDAL.PracticeBudgetListByYear(year);
        }

        public void CategoryItemBudgetSave(int itemId, BudgetCategoryType categoryType, DateTime monthStartDate, PracticeManagementCurrency amount)
        {
            ProjectDAL.CategoryItemBudgetSave(itemId, categoryType, monthStartDate, amount);
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

        public void CategoryItemsSaveFromXML(List<CategoryItemBudget> categoryItems, int year)
        {
            ProjectDAL.CategoryItemsSaveFromXML(categoryItems, year);
        }

        public void ProjectDelete(int projectId, string userName)
        {
            ProjectDAL.ProjectDelete(projectId, userName);//It will delete only Inactive and Experimental Projects as per #2702.
        }

        #endregion
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using DataAccess;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;

namespace PracticeManagementService
{
    /// <summary>
    /// Provides the logic for the calculation of the projects's rate.
    /// </summary>
    public class ProjectRateCalculator
    {
        #region Properties

        /// <summary>
        /// Gets or internally sets the project the rate be calculated for.
        /// </summary>
        public Project Project
        {
            get;
            private set;
        }

        #endregion

        #region Construction

        /// <summary>
        /// Creates a new instance of the <see cref="ProjectRateCalculator"/> class.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> the rates be calculated for.</param>
        public ProjectRateCalculator(Project project)
        {
            if (project != null)
            {
                this.Project = project;
                this.Project.ComputedFinancials = ComputedFinancialsDAL.FinancialsGetByProject(project.Id.Value);
            }
            //CalculateRate();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ProjectRateCalculator"/> class.
        /// </summary>
        /// <param name="projectId">An ID of the <see cref="Project"/> the rates be calculated for.</param>
        /// <param name="userName">The user (by email) to retrive the result for.</param>
        public ProjectRateCalculator(int projectId, string userName)
            : this(GetProjectDetail(projectId, userName))
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Enlists the requested projects.
        /// </summary>
        /// <param name="clientId">An ID of the client the projects belong to.</param>
        /// <param name="showActive">If true - the active (statusName=Active) projects will be included in the results.</param>
        /// <param name="showProjected">If true - the projected projects will be included in the results.</param>
        /// <param name="showCompleted">If true - the completed projects will are included in the results.</param>
        /// <param name="showExperimental">If true - the experimantal projects will are included in the results.</param>
        /// <param name="periodStart">The start of the period to enlist the projects within.</param>
        /// <param name="periodEnd">The end of the period to enlist the projects within.</param>
        /// <param name="salespersonId">Determines an ID of the salesperson to filter the list for.</param>
        /// <param name="practiceManagerId">Determines an ID of the practice manager to filter the list for.</param>
        /// <param name="includeCurentYearFinancials">
        /// If true - the current year financials will are included into the result
        /// otherwise the financials will are calculated for the specified period.
        /// </param>
        /// <returns>The list of the projects are match with the specified conditions.</returns>
        public static List<Project> GetProjectList(
            int? clientId,
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
            ProjectCalculateRangeType includeCurentYearFinancials)
        {
            List<Project> result =
                ProjectDAL.ProjectListAll(clientId,
                showProjected,
                showCompleted,
                showActive,
                showExperimental,
                periodStart,
                periodEnd,
                salespersonId,
                practiceManagerId,
                practiceId,
                projectGroupId);

            return CalculateRates(result, periodStart, periodEnd, includeCurentYearFinancials);
        }

        /// <summary>
        /// Enlists the requested projects.
        /// </summary>
        /// <param name="clientIds">Comma separated list of client ids. Null value means all clients.</param>
        /// <param name="showActive">If true - the active (statusName=Active) projects will be included in the results.</param>
        /// <param name="showProjected">If true - the projected projects will be included in the results.</param>
        /// <param name="showCompleted">If true - the completed projects will are included in the results.</param>
        /// <param name="showExperimental">If true - the experimantal projects will are included in the results.</param>
        /// <param name="periodStart">The start of the period to enlist the projects within.</param>
        /// <param name="periodEnd">The end of the period to enlist the projects within.</param>
        /// <param name="salespersonId">Determines an ID of the salesperson to filter the list for.</param>
        /// <param name="practiceManagerId">Determines an ID of the practice manager to filter the list for.</param>
        /// <param name="includeCurentYearFinancials">
        /// If true - the current year financials will are included into the result
        /// otherwise the financials will are calculated for the specified period.
        /// </param>
        /// <returns>The list of the projects are match with the specified conditions.</returns>
        public static List<Project> GetProjectListMultiParameters(
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
            string practiceManagerIdsList,
            string practiceIdsList,
            string projectGroupIdsList,
            ProjectCalculateRangeType includeCurentYearFinancials,
            bool excludeInternalPractices)
        {
            List<Project> result =
                ProjectDAL.ProjectListAllMultiParameters(
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
                practiceManagerIdsList,
                practiceIdsList,
                projectGroupIdsList,
                excludeInternalPractices);

            return CalculateRates(result, periodStart, periodEnd, includeCurentYearFinancials);
        }

        private static List<Project> CalculateRates(
            List<Project> result,
            DateTime periodStart,
            DateTime periodEnd,
            ProjectCalculateRangeType includeCurentYearFinancials)
        {
            DateTime currentYearStart = new DateTime(DateTime.Today.Year, 1, 1);
            DateTime currentYearEnd = new DateTime(currentYearStart.Year, 12, 31);

            // Recalculating the interest values
            return LoadFinancialsAndMilestonePersonInfo(
                currentYearStart,
                result,
                periodStart,
                periodEnd,
                includeCurentYearFinancials,
                currentYearEnd);
        }

        private static List<Project>
            LoadFinancialsAndMilestonePersonInfo(
                DateTime currentYearStart,
                List<Project> result,
                DateTime periodStart,
                DateTime periodEnd,
                ProjectCalculateRangeType calculatePeriodType,
                DateTime currentYearEnd)
        {
            ComputedFinancialsDAL.LoadFinancialsPeriodForProjects(
                result, periodStart, periodEnd);

            switch (calculatePeriodType)
            {
                case ProjectCalculateRangeType.ProjectValueInRange:
                    ComputedFinancialsDAL.LoadTotalFinancialsPeriodForProjects(
                        result,
                        periodStart,
                        periodEnd);
                    break;
                case ProjectCalculateRangeType.TotalProjectValue:
                    ComputedFinancialsDAL.LoadTotalFinancialsPeriodForProjects(
                        result,
                        null,
                        null);
                    break;
                case ProjectCalculateRangeType.CurrentFiscalYear:
                    Dictionary<string, DateTime> fyCalendar = GetFiscalYearPeriod(DateTime.Now);
                    ComputedFinancialsDAL.LoadTotalFinancialsPeriodForProjects(
                        result,
                        fyCalendar["StartMonth"],
                        fyCalendar["EndMonth"]);
                    break;

            } //For getting GrandTotal OR selected period Total or current Fiscal year total.

            MilestonePersonDAL.LoadMilestonePersonListForProject(result);

            return result;
        }


        public static List<Project> GetBenchListWithoutBenchTotalAndAdminCosts(BenchReportContext context)
        {
            var startDate = context.Start;
            var endDate = context.End;
            string userName = context.UserName;

            List<Project> result = new List<Project>();

            // Retrieve the list of persons who have some bench time
            var bench = PersonDAL.PersonListBenchExpense(context);

            // Calculate the bench expense
            foreach (Person person in bench)
            {
                Project benchProject = new Project();
                benchProject.Client = new Client();
                benchProject.Client.Name = Resources.Messages.BenchProjectClientName;
                benchProject.Client.Id = person.Id;

                benchProject.Name = person.LastName + ", " + person.FirstName;
                benchProject.StartDate = person.HireDate;
                benchProject.EndDate =
                    person.TerminationDate.HasValue ? person.TerminationDate.Value : endDate;

                benchProject.ProjectedFinancialsByMonth = person.ProjectedFinancialsByMonth;

                benchProject.Status = new ProjectStatus();
                if (person.Status != null && person.Status.Id == (int)PersonStatusType.Projected)
                {
                    benchProject.Status.Id = (int)ProjectStatusType.Projected;
                }

                benchProject.ProjectNumber = person.Status.Name;
                benchProject.AccessLevel = person.Seniority;
                benchProject.Practice = person.DefaultPractice;

                result.Add(benchProject);
            }

            return result;
        }

        /// <summary>
        /// Retrieves an info on the persons who have some bench time.
        /// Also retrieves an aggregated admin expense.
        /// </summary>
        /// <returns>The list of the <see cref="Person"/> objects.</returns>
        public static List<Project> GetBenchAndAdmin(BenchReportContext context)
        {
            var startDate = context.Start;
            var endDate = context.End;
            string userName = context.UserName;

            List<Project> result = new List<Project>();

            // Retrieve the list of persons who have some bench time
            var bench = PersonDAL.PersonListBenchExpense(context);

            // Calculate the bench expense
            foreach (Person person in bench)
            {
                Project benchProject = new Project();
                benchProject.Client = new Client();
                benchProject.Client.Name = Resources.Messages.BenchProjectClientName;
                benchProject.Client.Id = person.Id;

                benchProject.Name = person.LastName + ", " + person.FirstName;
                benchProject.StartDate = person.HireDate;
                benchProject.EndDate =
                    person.TerminationDate.HasValue ? person.TerminationDate.Value : endDate;

                benchProject.ProjectedFinancialsByMonth = person.ProjectedFinancialsByMonth;

                benchProject.Status = new ProjectStatus();
                if (person.Status != null && person.Status.Id == (int)PersonStatusType.Projected)
                {
                    benchProject.Status.Id = (int)ProjectStatusType.Projected;
                }

                benchProject.ProjectNumber = person.Status.Name;
                benchProject.AccessLevel = person.Seniority;
                benchProject.Practice = person.DefaultPractice;

                result.Add(benchProject);
            }

            // Bench total
            Project benchTotal = new Project();
            benchTotal.Name = Resources.Messages.BenchTotalProjectName;

            benchTotal.ProjectedFinancialsByMonth = new Dictionary<DateTime, ComputedFinancials>();
            for (DateTime dtTemp = startDate; dtTemp <= endDate; dtTemp = dtTemp.AddMonths(1))
            {
                ComputedFinancials financials = new ComputedFinancials();

                // Looking through the bench persons
                foreach (Person person in bench)
                {
                    foreach (KeyValuePair<DateTime, ComputedFinancials> personFinancials in person.ProjectedFinancialsByMonth)
                    {
                        if (personFinancials.Key.Year == dtTemp.Year && personFinancials.Key.Month == dtTemp.Month)
                        {
                            financials.Cogs += personFinancials.Value.Cogs;
                            financials.Revenue += personFinancials.Value.Revenue;
                            financials.GrossMargin +=
                                personFinancials.Value.Timescale == TimescaleType.Salary
                                ? personFinancials.Value.GrossMargin : 0;
                        }
                    }
                }

                benchTotal.ProjectedFinancialsByMonth.Add(dtTemp, financials);
            }

            result.Add(benchTotal);

            Project allAdmin = GetAdminCosts(startDate, endDate, userName);
            if (allAdmin != null)
            {
                result.Add(allAdmin);
            }

            return result;
        }

        /// <summary>
        /// Retrives the admin costs for the period.
        /// </summary>
        /// <param name="startDate">The period start.</param>
        /// <param name="endDate">The period end.</param>
        /// <param name="userName">The name of the current user.</param>
        /// <returns>The admid costs if available and null otherwise.</returns>
        public static Project GetAdminCosts(DateTime startDate, DateTime endDate, string userName)
        {
            Project allAdmin = null;
            // Only for the admin staffs
            if (Roles.IsUserInRole(userName, Constants.RoleNames.AdministratorRoleName))
            {
                // Retrive the list of admins (active only)
                var admins = PersonDAL.PersonListFiltered(Practice.AdminPractice, false, 0, 0, null, null);
                var calcs = admins.Select(
                    admin => new PersonRateCalculator(admin, false)).ToList();

                // Calculate an admin expense
                allAdmin = new Project
                {
                    Name = Resources.Messages.AdminProjectName,
                    StartDate = startDate,
                    EndDate = endDate,
                    Practice = new Practice { Id = Practice.AdminPractice },
                    ProjectedFinancialsByMonth = new Dictionary<DateTime, ComputedFinancials>()
                };

                for (var currentStart = startDate; currentStart <= endDate; currentStart = currentStart.AddMonths(1))
                {
                    var financials = new ComputedFinancials();

                    var currentEnd = currentStart.AddMonths(1).AddDays(-1);
                    var companyWorkDays = PersonRateCalculator.CompanyWorkDaysNumber(currentStart, currentEnd);
                    foreach (var calc in calcs)
                    {
                        var personHours = calc.GetPersonWorkDays(currentStart, currentEnd);
                        var companyHours = companyWorkDays * calc.DefaultHours;
                        var cogs = calc.CalculateCogsForHours(personHours, companyHours, 0M);
                        financials.Cogs += cogs;
                    }

                    financials.GrossMargin = -financials.Cogs;
                    allAdmin.ProjectedFinancialsByMonth.Add(currentStart, financials);
                }
            }

            return allAdmin;
        }

        /// <summary>
        /// Reatrives a project with a specified ID.
        /// </summary>
        /// <param name="projectId">The ID of the requested project.</param>
        /// <param name="userName">The user (by email) to retrive the result for.</param>
        /// <returns>The <see cref="Project"/> record if found and null otherwise.</returns>
        public static Project GetProjectDetail(int projectId, string userName)
        {
            int? salespersonId = null;
            int? practiceManagerId = null;

            // VerifyPrivileges(userName, ref salespersonId, ref practiceManagerId);
            Project result = ProjectDAL.GetById(projectId, salespersonId, practiceManagerId);
            if (result != null)
            {
                result.Milestones = MilestoneDAL.MilestoneListByProject(projectId);

                // Project's commissions
                result.SalesCommission = CommissionDAL.CommissionGetByProjectType(projectId, CommissionType.Sales);
                List<Commission> managementCommission =
                    CommissionDAL.CommissionGetByProjectType(projectId, CommissionType.PracticeManagement);
                result.ManagementCommission = managementCommission.Count > 0 ? managementCommission[0] : null;
            }

            return result;
        }

        public static Dictionary<string, DateTime> GetFiscalYearPeriod(DateTime currentMonth)
        {
            Dictionary<string, DateTime> fPeriod = new Dictionary<string, DateTime>();


            DateTime startMonth = new DateTime(currentMonth.Year, 1, 1);
            DateTime endMonth = new DateTime(currentMonth.Year, 12, 31);

            fPeriod.Add("StartMonth", startMonth);
            fPeriod.Add("EndMonth", endMonth);

            return fPeriod;
        }

        #endregion
    }
}


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
    public static class ProjectRateCalculator
    {
        #region Methods

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
            bool showProposed,
            bool showInactive,
            bool showAtRisk,
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
            ProjectCalculateRangeType includeCurentYearFinancials,
            bool excludeInternalPractices,
            string userLogin,
            bool useActuals,
            bool getFinancialsFromCache)
        {
            List<Project> result =
                ProjectDAL.ProjectListAllMultiParameters(
                clientIds,
                showProjected,
                showCompleted,
                showActive,
                showInternal,
                showExperimental,
                showProposed,
                showInactive,
                showAtRisk,
                periodStart,
                periodEnd,
                salespersonIdsList,
                practiceManagerIdsList,
                practiceIdsList,
                divisionIdsList,
                channelIdsList,
                revenueTypeIdsList,
                offeringIdsList,
                projectGroupIdsList,
                excludeInternalPractices,
                userLogin);

            if (!getFinancialsFromCache)
            {
                return CalculateRates(result, periodStart, periodEnd, includeCurentYearFinancials, useActuals);
            }
            return LoadFinancialsAndMilestonePersonInfoFromCache(
                result,
                periodStart,
                periodEnd,
                includeCurentYearFinancials);
        }

        private static List<Project> CalculateRates(
            List<Project> result,
            DateTime periodStart,
            DateTime periodEnd,
            ProjectCalculateRangeType includeCurentYearFinancials,
            bool useActuals)
        {
            // Recalculating the interest values
            return LoadFinancialsAndMilestonePersonInfo(
                result,
                periodStart,
                periodEnd,
                includeCurentYearFinancials,
                useActuals);
        }

        private static List<Project>
            LoadFinancialsAndMilestonePersonInfo(
                List<Project> result,
                DateTime periodStart,
                DateTime periodEnd,
                ProjectCalculateRangeType calculatePeriodType,
                bool useActuals
            )
        {
            ComputedFinancialsDAL.LoadFinancialsPeriodForProjects(result, periodStart, periodEnd, useActuals);

            switch (calculatePeriodType)
            {
                case ProjectCalculateRangeType.ProjectValueInRange:
                    CalculateTotalFinancials(result); //Reducing DB call(Alternative:- Calculating total value by Summing the result values only).
                    //ComputedFinancialsDAL.LoadTotalFinancialsPeriodForProjects(
                    //    result,
                    //    periodStart,
                    //    periodEnd);
                    break;

                case ProjectCalculateRangeType.TotalProjectValue:
                    ProjectDAL.InsertTodayProjectsIntoCache();
                    ComputedFinancialsDAL.LoadTotalFinancialsPeriodForProjectsFromCache(
                        result,
                        null,
                        null);
                    break;

                case ProjectCalculateRangeType.CurrentFiscalYear:
                    Dictionary<string, DateTime> fyCalendar = GetFiscalYearPeriod(SettingsHelper.GetCurrentPMTime().Date);
                    ComputedFinancialsDAL.LoadTotalFinancialsPeriodForProjects(
                        result,
                        fyCalendar["StartMonth"],
                        fyCalendar["EndMonth"],
                        useActuals);
                    break;
            } //For getting GrandTotal OR selected period Total or current Fiscal year total.

            MilestonePersonDAL.LoadMilestonePersonListForProject(result);

            return result;
        }

        private static List<Project>
           LoadFinancialsAndMilestonePersonInfoFromCache(
               List<Project> result,
               DateTime periodStart,
               DateTime periodEnd,
               ProjectCalculateRangeType calculatePeriodType
            // bool useActuals :From cache we renders data always useActuals as true
           )
        {
            ComputedFinancialsDAL.LoadFinancialsPeriodForProjectsFromCache(result, periodStart, periodEnd, false);

            switch (calculatePeriodType)
            {
                case ProjectCalculateRangeType.ProjectValueInRange:
                    CalculateTotalFinancials(result); //Reducing DB call(Alternative:- Calculating total value by Summing the result values only).
                    break;

                case ProjectCalculateRangeType.TotalProjectValue:
                    ComputedFinancialsDAL.LoadTotalFinancialsPeriodForProjectsFromCache(
                        result,
                        null,
                        null);
                    break;

                case ProjectCalculateRangeType.CurrentFiscalYear:
                    Dictionary<string, DateTime> fyCalendar = GetFiscalYearPeriod(SettingsHelper.GetCurrentPMTime().Date);
                    List<Project> currentYearProjectsList = result.Select(project => new Project { Id = project.Id }).ToList();
                    ComputedFinancialsDAL.LoadFinancialsPeriodForProjectsFromCache(currentYearProjectsList, fyCalendar["StartMonth"], fyCalendar["EndMonth"], false);
                    CalculateCurrentFiscalYearTotalFinancials(result, currentYearProjectsList);
                    break;
            } //For getting GrandTotal OR selected period Total or current Fiscal year total.

            MilestonePersonDAL.LoadMilestonePersonListForProject(result);

            return result;
        }

        private static void CalculateCurrentFiscalYearTotalFinancials(List<Project> projects, List<Project> currentYearProjects)
        {
            foreach (Project project in projects)
            {
                if (currentYearProjects.Any(p => p.Id == project.Id))
                {
                    Project currentYearProject = currentYearProjects.First(p => p.Id == project.Id);

                    var financials = new ComputedFinancials
                        {
                            FinancialDate = currentYearProject.StartDate,
                            Revenue = currentYearProject.ProjectedFinancialsByMonth.Values.Sum(mf => mf.Revenue),
                            RevenueNet = currentYearProject.ProjectedFinancialsByMonth.Values.Sum(mf => mf.RevenueNet),
                            Cogs = currentYearProject.ProjectedFinancialsByMonth.Values.Sum(mf => mf.Cogs),
                            GrossMargin = currentYearProject.ProjectedFinancialsByMonth.Values.Sum(mf => mf.GrossMargin),
                            HoursBilled = currentYearProject.ProjectedFinancialsByMonth.Values.Sum(mf => mf.HoursBilled),
                            Expenses = currentYearProject.ProjectedFinancialsByMonth.Values.Sum(mf => mf.Expenses),
                            ReimbursedExpenses =
                                currentYearProject.ProjectedFinancialsByMonth.Values.Sum(mf => mf.ReimbursedExpenses),
                            ActualRevenue =
                                currentYearProject.ProjectedFinancialsByMonth.Values.Sum(mf => mf.ActualRevenue),
                            ActualGrossMargin =
                                currentYearProject.ProjectedFinancialsByMonth.Values.Sum(mf => mf.ActualGrossMargin),
                            PreviousMonthsActualMarginValue = currentYearProject.ProjectedFinancialsByMonth.Values.Sum(mf => mf.PreviousMonthsActualMarginValue),
                            PreviousMonthsActualRevenueValue = currentYearProject.ProjectedFinancialsByMonth.Values.Sum(mf => mf.PreviousMonthsActualRevenueValue)
                        };

                    project.ComputedFinancials = financials;
                }
                else
                {
                    var financials = new ComputedFinancials
                    {
                        FinancialDate = project.StartDate,
                    };
                    project.ComputedFinancials = financials;
                }
            }
        }

        private static void CalculateTotalFinancials(List<Project> result)
        {
            foreach (var project in result)
            {
                var financials = new ComputedFinancials
                    {
                        FinancialDate = project.StartDate,
                        Revenue = project.ProjectedFinancialsByMonth.Values.Sum(mf => mf.Revenue),
                        RevenueNet = project.ProjectedFinancialsByMonth.Values.Sum(mf => mf.RevenueNet),
                        Cogs = project.ProjectedFinancialsByMonth.Values.Sum(mf => mf.Cogs),
                        GrossMargin = project.ProjectedFinancialsByMonth.Values.Sum(mf => mf.GrossMargin),
                        HoursBilled = project.ProjectedFinancialsByMonth.Values.Sum(mf => mf.HoursBilled),
                        Expenses = project.ProjectedFinancialsByMonth.Values.Sum(mf => mf.Expenses),
                        ReimbursedExpenses = project.ProjectedFinancialsByMonth.Values.Sum(mf => mf.ReimbursedExpenses),
                        ActualRevenue = project.ProjectedFinancialsByMonth.Values.Sum(mf => mf.ActualRevenue),
                        ActualGrossMargin = project.ProjectedFinancialsByMonth.Values.Sum(mf => mf.ActualGrossMargin),
                        PreviousMonthsActualRevenueValue = project.ProjectedFinancialsByMonth.Values.Sum(mf => mf.PreviousMonthsActualRevenueValue),
                        PreviousMonthsActualMarginValue = project.ProjectedFinancialsByMonth.Values.Sum(mf => mf.PreviousMonthsActualMarginValue)
                    };

                project.ComputedFinancials = financials;
            }
        }

        public static List<Project> GetBenchListWithoutBenchTotalAndAdminCosts(BenchReportContext context)
        {
            var endDate = context.End;
            List<Project> result = new List<Project>();

            // Retrieve the list of persons who have some bench time
            var bench = PersonDAL.PersonListBenchExpense(context);

            // Calculate the bench expense
            foreach (Person person in bench)
            {
                Project benchProject = new Project
                    {
                        Client = new Client { Name = Resources.Messages.BenchProjectClientName, Id = person.Id },
                        Name = person.LastName + ", " + person.FirstName,
                        StartDate = person.HireDate,
                        EndDate = person.TerminationDate.HasValue ? person.TerminationDate.Value : endDate,
                        ProjectedFinancialsByMonth = person.ProjectedFinancialsByMonth,
                        Status = new ProjectStatus()
                    };

                if (person.Status != null && person.Status.Id == (int)PersonStatusType.Contingent)
                {
                    benchProject.Status.Id = (int)ProjectStatusType.Projected;
                }

                if (person.Status != null) benchProject.ProjectNumber = person.Status.Name;
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
                Project benchProject = new Project
                    {
                        Client = new Client { Name = Resources.Messages.BenchProjectClientName, Id = person.Id },
                        Name = person.LastName + ", " + person.FirstName,
                        StartDate = person.HireDate,
                        EndDate = person.TerminationDate.HasValue ? person.TerminationDate.Value : endDate,
                        ProjectedFinancialsByMonth = person.ProjectedFinancialsByMonth,
                        Status = new ProjectStatus()
                    };

                if (person.Status != null && person.Status.Id == (int)PersonStatusType.Contingent)
                {
                    benchProject.Status.Id = (int)ProjectStatusType.Projected;
                }

                if (person.Status != null) benchProject.ProjectNumber = person.Status.Name;
                benchProject.AccessLevel = person.Seniority;
                benchProject.Practice = person.DefaultPractice;

                result.Add(benchProject);
            }

            // Bench total
            Project benchTotal = new Project
                {
                    Name = Resources.Messages.BenchTotalProjectName,
                    ProjectedFinancialsByMonth = new Dictionary<DateTime, ComputedFinancials>()
                };

            for (DateTime dtTemp = startDate; dtTemp <= endDate; dtTemp = dtTemp.AddMonths(1))
            {
                ComputedFinancials financials = new ComputedFinancials();

                // Looking through the bench persons
                DateTime temp = dtTemp;
                foreach (KeyValuePair<DateTime, ComputedFinancials> personFinancials in
                    bench.SelectMany(person => person.ProjectedFinancialsByMonth,
                                     (person, personFinancials) => new { person, personFinancials })
                         .Where(
                             @t =>
                             @t.personFinancials.Key.Year == temp.Year && @t.personFinancials.Key.Month == temp.Month)
                         .Select(@t => @t.personFinancials))
                {
                    financials.Cogs += personFinancials.Value.Cogs;
                    financials.Revenue += personFinancials.Value.Revenue;
                    financials.GrossMargin +=
                        personFinancials.Value.Timescale == TimescaleType.Salary
                            ? personFinancials.Value.GrossMargin : 0;
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
                var calcs = admins.Select(admin => new PersonRateCalculator(admin)).ToList();

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
                    var companyHours = PersonRateCalculator.CompanyWorkHoursNumber(currentStart, currentEnd);
                    DateTime start = currentStart;
                    foreach (var cogs in
                        calcs.Select(calc => new { calc, personHours = calc.GetPersonWorkHours(start, currentEnd) })
                             .Select(@t => @t.calc.CalculateCogsForHours(@t.personHours, companyHours)))
                    {
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
            Project result = ProjectDAL.GetById(projectId);
            if (result != null)
            {
                result.Milestones = MilestoneDAL.MilestoneListByProject(projectId);

                if (result.HasAttachments)
                {
                    result.Attachments = ProjectDAL.GetProjectAttachments(projectId);
                }
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

        #endregion Methods
    }
}


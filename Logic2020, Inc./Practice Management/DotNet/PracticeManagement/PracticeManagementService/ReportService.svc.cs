using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using DataAccess;
using DataTransferObjects;
using DataTransferObjects.Reports;
using DataTransferObjects.Reports.ByAccount;
using DataTransferObjects.Reports.ConsultingDemand;
using DataTransferObjects.Reports.HumanCapital;

namespace PracticeManagementService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ReportService : IReportService
    {
        public List<TimeEntriesGroupByClientAndProject> PersonTimeEntriesDetails(int personId, DateTime startDate, DateTime endDate)
        {
            return ReportDAL.PersonTimeEntriesDetails(personId, startDate, endDate);
        }

        public List<TimeEntriesGroupByClientAndProject> PersonTimeEntriesSummary(int personId, DateTime startDate, DateTime endDate)
        {
            return ReportDAL.PersonTimeEntriesSummary(personId, startDate, endDate);
        }

        public PersonTimeEntriesTotals GetPersonTimeEntriesTotalsByPeriod(int personId, DateTime startDate, DateTime endDate)
        {
            return ReportDAL.GetPersonTimeEntriesTotalsByPeriod(personId, startDate, endDate);
        }

        public List<PersonLevelGroupedHours> TimePeriodSummaryReportByResource(DateTime startDate, DateTime endDate, bool includePersonsWithNoTimeEntries, string personIds, string titleIds, string timescaleNames, string personStatusIds, string personDivisionIds)
        {
            return ReportDAL.TimePeriodSummaryReportByResource(startDate, endDate, includePersonsWithNoTimeEntries, personIds, titleIds, timescaleNames, personStatusIds, personDivisionIds);
        }

        public List<ProjectLevelGroupedHours> TimePeriodSummaryReportByProject(DateTime startDate, DateTime endDate, string clientIds, string personStatusIds)
        {
            return ReportDAL.TimePeriodSummaryReportByProject(startDate, endDate, clientIds, personStatusIds);
        }

        public List<WorkTypeLevelGroupedHours> TimePeriodSummaryReportByWorkType(DateTime startDate, DateTime endDate)
        {
            return ReportDAL.TimePeriodSummaryReportByWorkType(startDate, endDate);
        }

        public List<PersonLevelGroupedHours> ProjectSummaryReportByResource(string projectNumber, int? mileStoneId, DateTime? startDate, DateTime? endDate, string personRoleNames)
        {
            return ReportDAL.ProjectSummaryReportByResource(projectNumber, mileStoneId, startDate, endDate, personRoleNames);
        }

        public List<PersonLevelGroupedHours> ProjectDetailReportByResource(string projectNumber, int? mileStoneId, DateTime? startDate, DateTime? endDate, string personRoleNames, bool isExport = false)
        {
            return ReportDAL.ProjectDetailReportByResource(projectNumber, mileStoneId, startDate, endDate, personRoleNames, isExport);
        }

        public List<WorkTypeLevelGroupedHours> ProjectSummaryReportByWorkType(string projectNumber, int? mileStoneId, DateTime? startDate, DateTime? endDate, string categoryNames)
        {
            return ReportDAL.ProjectSummaryReportByWorkType(projectNumber, mileStoneId, startDate, endDate, categoryNames);
        }

        public List<Project> GetProjectsByClientId(int clientId)
        {
            return ReportDAL.GetProjectsByClientId(clientId);
        }

        public List<Project> ProjectSearchByName(string name)
        {
            try
            {
                return ReportDAL.ProjectSearchByName(name);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Milestone> GetMilestonesForProject(string projectNumber)
        {
            try
            {
                return ReportDAL.GetMilestonesForProject(projectNumber);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<PersonLevelPayCheck> TimePeriodSummaryByResourcePayCheck(DateTime startDate, DateTime endDate, bool includePersonsWithNoTimeEntries, string personIds, string titleIds, string timescaleNames, string personStatusIds, string personDivisionIds)
        {
            return ReportDAL.TimePeriodSummaryByResourcePayCheck(startDate, endDate, includePersonsWithNoTimeEntries, personIds, titleIds, timescaleNames, personStatusIds, personDivisionIds);
        }

        public List<PersonLevelTimeEntriesHistory> TimeEntryAuditReportByPerson(DateTime startDate, DateTime endDate)
        {
            return ReportDAL.TimeEntryAuditReportByPerson(startDate, endDate);
        }

        public List<ProjectLevelTimeEntriesHistory> TimeEntryAuditReportByProject(DateTime startDate, DateTime endDate)
        {
            return ReportDAL.TimeEntryAuditReportByProject(startDate, endDate);
        }

        public GroupByAccount AccountSummaryReportByBusinessUnit(int accountId, string businessUnitIds, string projectStatusIds, DateTime startDate, DateTime endDate)
        {
            return ReportDAL.AccountSummaryReportByBusinessUnit(accountId, businessUnitIds, projectStatusIds, startDate, endDate);
        }

        public GroupByAccount AccountSummaryReportByProject(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate, string projectStatusIds, string projectBillingTypes)
        {
            return ReportDAL.AccountSummaryReportByProject(accountId, businessUnitIds, startDate, endDate, projectStatusIds, projectBillingTypes);
        }

        public List<BusinessUnitLevelGroupedHours> AccountReportGroupByBusinessUnit(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate)
        {
            return ReportDAL.AccountReportGroupByBusinessUnit(accountId, businessUnitIds, startDate, endDate);
        }

        public List<GroupByPerson> AccountReportGroupByPerson(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate)
        {
            return ReportDAL.AccountReportGroupByPerson(accountId, businessUnitIds, startDate, endDate);
        }

        public List<Person> NewHireReport(DateTime startDate, DateTime endDate, string personStatusIds, string payTypeIds, string practiceIds, bool excludeInternalPractices, string personDivisionIds, string titleIds, string hireDates, string recruiterIds)
        {
            return ReportDAL.NewHireReport(startDate, endDate, personStatusIds, payTypeIds, practiceIds, excludeInternalPractices, personDivisionIds, titleIds, hireDates, recruiterIds);
        }

        public TerminationPersonsInRange TerminationReport(DateTime startDate, DateTime endDate, string payTypeIds, string personStatusIds, string titleIds, string terminationReasonIds, string practiceIds, bool excludeInternalPractices, string personDivisionIds, string recruiterIds, string hireDates, string terminationDates)
        {
            return ReportDAL.TerminationReport(startDate, endDate, payTypeIds, personStatusIds, titleIds, terminationReasonIds, practiceIds, excludeInternalPractices, personDivisionIds, recruiterIds, hireDates, terminationDates);
        }

        public List<TerminationPersonsInRange> TerminationReportGraph(DateTime startDate, DateTime endDate)
        {
            return ReportDAL.TerminationReportGraph(startDate, endDate);
        }

        #region ConsultingDemand

        public List<ConsultantGroupbyTitleSkill> ConsultingDemandSummary(DateTime startDate, DateTime endDate, string titles, string skills)
        {
            return ReportDAL.ConsultingDemandSummary(startDate, endDate, titles, skills);
        }

        public List<ConsultantGroupbyTitleSkill> ConsultingDemandDetailsByTitleSkill(DateTime startDate, DateTime endDate, string titles, string skills, string sortColumns)
        {
            return ReportDAL.ConsultingDemandDetailsByTitleSkill(startDate, endDate, titles, skills, sortColumns);
        }

        public List<ConsultantGroupByMonth> ConsultingDemandDetailsByMonth(DateTime startDate, DateTime endDate, string titles, string skills, string salesStages, string sortColumns, bool isFromPipeLinePopUp)
        {
            return ReportDAL.ConsultingDemandDetailsByMonth(startDate, endDate, titles, skills, salesStages, sortColumns, isFromPipeLinePopUp);
        }

        public Dictionary<string, int> ConsultingDemandGraphsByTitle(DateTime startDate, DateTime endDate, string Title, string salesStages)
        {
            return ReportDAL.ConsultingDemandGraphsByTitle(startDate, endDate, Title, salesStages);
        }

        public Dictionary<string, int> ConsultingDemandGraphsBySkills(DateTime startDate, DateTime endDate, string Skill, string salesStages)
        {
            return ReportDAL.ConsultingDemandGraphsBySkills(startDate, endDate, Skill, salesStages);
        }

        public List<ConsultantGroupbyTitle> ConsultingDemandTransactionReportByTitle(DateTime startDate, DateTime endDate, string Title, string sortColumns, string salesStages)
        {
            return ReportDAL.ConsultingDemandTransactionReportByTitle(startDate, endDate, Title, sortColumns, salesStages);
        }

        public List<ConsultantGroupbySkill> ConsultingDemandTransactionReportBySkill(DateTime startDate, DateTime endDate, string Skill, string sortColumns, string salesStages)
        {
            return ReportDAL.ConsultingDemandTransactionReportBySkill(startDate, endDate, Skill, sortColumns, salesStages);
        }

        public Dictionary<string, int> ConsultingDemandGrphsGroupsByTitle(DateTime startDate, DateTime endDate, string salesStages)
        {
            return ReportDAL.ConsultingDemandGrphsGroupsByTitle(startDate, endDate, salesStages);
        }

        public Dictionary<string, int> ConsultingDemandGrphsGroupsBySkill(DateTime startDate, DateTime endDate, string salesStages)
        {
            return ReportDAL.ConsultingDemandGrphsGroupsBySkill(startDate, endDate, salesStages);
        }

        public List<ConsultantGroupBySalesStage> ConsultingDemandDetailsBySalesStage(DateTime startDate, DateTime endDate, string titles, string skills, string sortColumns)
        {
            return ReportDAL.ConsultingDemandDetailsBySalesStage(startDate, endDate, titles, skills, sortColumns);
        }

        public List<AttainmentBillableutlizationReport> AttainmentBillableutlizationReport(DateTime startDate, DateTime endDate)
        {
            return ReportDAL.AttainmentBillableutlizationReport(startDate, endDate);
        }

        #endregion ConsultingDemand

        public List<Project> GetAttainmentProjectListMultiParameters(
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
         string projectGroupIdsList,
         ProjectCalculateRangeType includeCurentYearFinancials,
         bool excludeInternalPractices,
         string userLogin,
         bool IsMonthsColoumnsShown,
         bool IsQuarterColoumnsShown,
         bool IsYearToDateColoumnsShown,
         bool getFinancialsFromCache)
        {
            return ReportDAL.GetAttainmentProjectListMultiParameters(clientIds, showProjected, showCompleted, showActive, showInternal, showExperimental, showProposed, showInactive, showAtRisk, periodStart, periodEnd, salespersonIdsList, practiceManagerIdsList, practiceIdsList, projectGroupIdsList, includeCurentYearFinancials, excludeInternalPractices, userLogin, IsMonthsColoumnsShown, IsQuarterColoumnsShown, IsYearToDateColoumnsShown, getFinancialsFromCache);
        }

        public List<Project> ProjectAttributionReport(DateTime startDate, DateTime endDate)
        {
            return ReportDAL.ProjectAttributionReport(startDate, endDate);
        }

        public List<ResourceExceptionReport> ZeroHourlyRateExceptionReport(DateTime startDate, DateTime endDate)
        {
            return ReportDAL.ZeroHourlyRateExceptionReport(startDate, endDate);
        }

        public List<ResourceExceptionReport> ResourceAssignedOrUnassignedChargingExceptionReport(DateTime startDate, DateTime endDate, bool isUnassignedReport)
        {
            return ReportDAL.ResourceAssignedOrUnassignedChargingExceptionReport(startDate, endDate, isUnassignedReport);
        }

        public List<Person> RecruitingMetricsReport(DateTime startDate, DateTime endDate)
        {
            return ReportDAL.RecruitingMetricsReport(startDate, endDate);
        }

        public List<ProjectFeedback> ProjectFeedbackReport(string accountIds, string businessGroupIds, DateTime startDate, DateTime endDate, string projectStatus, string projectIds, string directorIds, string practiceIds, bool excludeInternalPractices, string personIds, string titleIds, string reviewStartdateMonths, string reviewEnddateMonths, string projectmanagerIds, string statusIds, bool isExport, string payTypeIds)
        {
            return ReportDAL.ProjectFeedbackReport(accountIds, businessGroupIds, startDate, endDate, projectStatus, projectIds, directorIds, practiceIds, excludeInternalPractices, personIds, titleIds, reviewStartdateMonths, reviewEnddateMonths, projectmanagerIds, statusIds, isExport, payTypeIds);
        }

        public List<BillingReport> BillingReportByCurrency(DateTime startDate, DateTime endDate, string practiceIds, string accountIds, string businessUnitIds, string directorIds, string salesPersonIds, string projectManagerIds, string seniorManagerIds)
        {
            return ReportDAL.BillingReportByCurrency(startDate, endDate, practiceIds, accountIds, businessUnitIds, directorIds, salesPersonIds, projectManagerIds, seniorManagerIds);
        }

        public List<BillingReport> BillingReportByHours(DateTime startDate, DateTime endDate, string practiceIds, string accountIds, string businessUnitIds, string directorIds, string salesPersonIds, string projectManagerIds, string seniorManagerIds)
        {
            return ReportDAL.BillingReportByHours(startDate, endDate, practiceIds, accountIds, businessUnitIds, directorIds, salesPersonIds, projectManagerIds, seniorManagerIds);
        }

        public List<ProjectLevelGroupedHours> NonBillableReport(DateTime startDate, DateTime endDate, string projectNumber, string directorIds, string businessUnitIds, string practiceIds)
        {
            return ReportDAL.NonBillableReport(startDate, endDate, projectNumber, directorIds, businessUnitIds, practiceIds);
        }

        public List<BadgedResourcesByTime> BadgedResourcesByTimeReport(string payTypes, string personStatusIds, DateTime startDate, DateTime endDate, int step)
        {
            return ReportDAL.BadgedResourcesByTimeReport(payTypes, personStatusIds, startDate, endDate, step);
        }

        public List<MSBadge> ListBadgeResourcesByType(string paytypes, string personStatuses, DateTime startDate, DateTime endDate, bool isNotBadged, bool isClockNotStart, bool isBlocked, bool isBreak, bool badgedOnProject, bool isBadgedException, bool isNotBadgedException)
        {
            return ReportDAL.ListBadgeResourcesByType(paytypes, personStatuses, startDate, endDate, isNotBadged, isClockNotStart, isBlocked, isBreak, badgedOnProject, isBadgedException, isNotBadgedException);
        }

        public List<GroupByPractice> ResourcesByPracticeReport(string paytypes, string PersonStatuses, string practices, DateTime startDate, DateTime endDate, int step)
        {
            return ReportDAL.ResourcesByPracticeReport(paytypes, PersonStatuses, practices, startDate, endDate, step);
        }

        public List<GroupbyTitle> ResourcesByTitleReport(string paytypes, string personStatuses, string titles, DateTime startDate, DateTime endDate, int step)
        {
            return ReportDAL.ResourcesByTitleReport(paytypes, personStatuses, titles, startDate, endDate, step);
        }

        public List<MSBadge> GetBadgeRequestNotApprovedList()
        {
            return ReportDAL.GetBadgeRequestNotApprovedList();
        }

        public List<MSBadge> GetAllBadgeDetails(string payTypes, string personStatuses)
        {
            return ReportDAL.GetAllBadgeDetails(payTypes, personStatuses);
        }

        public PersonTimeEntriesTotals UtilizationReport(int personId, DateTime startDate, DateTime endDate)
        {
            return ReportDAL.UtilizationReport(personId, startDate, endDate);
        }

        public List<ManagementMeetingReport> ManagedServiceReportByPerson(string paytypes, string personStatuses, DateTime startDate, DateTime endDate)
        {
            return ReportDAL.ManagedServiceReportByPerson(paytypes, personStatuses, startDate, endDate);
        }

        public void SaveManagedParametersByPerson(string userLogin, decimal actualRevenuePerHour, decimal targetRevenuePerHour, decimal hoursUtilization, decimal targetRevenuePerAnnum)
        {
            ReportDAL.SaveManagedParametersByPerson(userLogin, actualRevenuePerHour, targetRevenuePerHour, hoursUtilization, targetRevenuePerAnnum);
        }

        public RevenueReport GetManagedParametersByPerson(string userLogin)
        {
            return ReportDAL.GetManagedParametersByPerson(userLogin);
        }

        public List<GroupbyTitle> GetAveragePercentagesByTitles(string paytypes, string personStatuses, string titles, DateTime startDate, DateTime endDate)
        {
            return ReportDAL.GetAveragePercentagesByTitles(paytypes, personStatuses, titles, startDate, endDate);
        }

        public List<Project> ProjectsListWithFilters(
            string clientIdsList,
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
            string ProjectOwnerIdsList,
            string practiceIdsList,
            string projectGroupIdsList,
            string divisionIdsList,
            string channelIdsList,
            string revenueTypeIdsList,
            string offeringIdsList,
            string userLogin)
        {
            return ReportDAL.ProjectsListWithFilters(
                 clientIdsList,
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
                 ProjectOwnerIdsList,
                 practiceIdsList,
                 projectGroupIdsList,
                 divisionIdsList,
                 channelIdsList,
            revenueTypeIdsList,
            offeringIdsList,
                 userLogin
                 );
        }


        public List<ExpenseSummary> GetExpenseSummaryGroupedByProject(DateTime startDate, DateTime endDate, string clientIds, string divisionIds, string practiceIds, string projectIds, bool active, bool projected, bool completed, bool proposed, bool inActive, bool experimental, bool atRisk)
        {
            return ReportDAL.GetExpenseSummaryGroupedByProject(startDate, endDate, clientIds, divisionIds, practiceIds, projectIds, active, projected, completed, proposed, inActive, experimental, atRisk);
        }

        public List<ExpenseSummary> GetExpenseSummaryGroupedBytype(DateTime startDate, DateTime endDate, string expenseTypeIds)
        {
            return ReportDAL.GetExpenseSummaryGroupedBytype(startDate, endDate, expenseTypeIds);
        }

        public List<ExpenseSummary> ExpenseDetailReport(DateTime startDate, DateTime endDate, int? projectId, int? expenseTypeId)
        {
            return ReportDAL.ExpenseDetailReport(startDate, endDate, projectId, expenseTypeId);
        }

        public List<ExpenseSummary> DetailedExpenseSummary(DateTime startDate, DateTime endDate, string clientIds, string divisionIds, string practiceIds, string projectIds, bool active, bool projected, bool completed, bool proposed, bool inActive, bool experimental, bool atRisk, string expenseTypeIds)
        {
            return ReportDAL.DetailedExpenseSummary(startDate, endDate, clientIds, divisionIds, practiceIds, projectIds, active, projected, completed, proposed, inActive, experimental, atRisk, expenseTypeIds);
        }
    }
}


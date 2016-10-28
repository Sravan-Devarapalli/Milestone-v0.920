using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataTransferObjects;
using DataTransferObjects.Reports;
using DataTransferObjects.Reports.ByAccount;
using DataTransferObjects.Reports.ConsultingDemand;
using DataTransferObjects.Reports.HumanCapital;

namespace PracticeManagementService
{
    // NOTE: You can use the "Rename" command on the "Refractor" menu to change the interface name "IReportService" in both code and config file together.
    [ServiceContract]
    public interface IReportService
    {
        [OperationContract]
        List<TimeEntriesGroupByClientAndProject> PersonTimeEntriesDetails(int personId, DateTime startDate, DateTime endDate);

        [OperationContract]
        List<TimeEntriesGroupByClientAndProject> PersonTimeEntriesSummary(int personId, DateTime startDate, DateTime endDate);

        [OperationContract]
        PersonTimeEntriesTotals GetPersonTimeEntriesTotalsByPeriod(int personId, DateTime startDate, DateTime endDate);

        [OperationContract]
        List<PersonLevelGroupedHours> TimePeriodSummaryReportByResource(DateTime startDate, DateTime endDate, bool includePersonsWithNoTimeEntries, string personIds, string titleIds, string timescaleNames, string personStatusIds, string personDivisionIds);

        [OperationContract]
        List<ProjectLevelGroupedHours> TimePeriodSummaryReportByProject(DateTime startDate, DateTime endDate, string clientIds, string personStatusIds);

        [OperationContract]
        List<WorkTypeLevelGroupedHours> TimePeriodSummaryReportByWorkType(DateTime startDate, DateTime endDate);

        [OperationContract]
        List<PersonLevelGroupedHours> ProjectSummaryReportByResource(string projectNumber, int? mileStoneId, DateTime? startDate, DateTime? endDate, string personRoleNames);

        [OperationContract]
        List<PersonLevelGroupedHours> ProjectDetailReportByResource(string projectNumber, int? mileStoneId, DateTime? startDate, DateTime? endDate, string personRoleNames, bool isExport = false);

        [OperationContract]
        List<WorkTypeLevelGroupedHours> ProjectSummaryReportByWorkType(string projectNumber, int? mileStoneId, DateTime? startDate, DateTime? endDate, string categoryNames);

        [OperationContract]
        List<Project> GetProjectsByClientId(int clientId);

        [OperationContract]
        List<Project> ProjectSearchByName(string name);

        [OperationContract]
        List<Milestone> GetMilestonesForProject(string projectNumber);

        [OperationContract]
        List<PersonLevelPayCheck> TimePeriodSummaryByResourcePayCheck(DateTime startDate, DateTime endDate, bool includePersonsWithNoTimeEntries, string personIds, string titleIds, string timescaleNames, string personStatusIds, string personDivisionIds);

        [OperationContract]
        List<PersonLevelTimeEntriesHistory> TimeEntryAuditReportByPerson(DateTime startDate, DateTime endDate);

        [OperationContract]
        List<ProjectLevelTimeEntriesHistory> TimeEntryAuditReportByProject(DateTime startDate, DateTime endDate);

        [OperationContract]
        GroupByAccount AccountSummaryReportByBusinessUnit(int accountId, string businessUnitIds, string projectStatusIds, DateTime startDate, DateTime endDate);

        [OperationContract]
        GroupByAccount AccountSummaryReportByProject(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate, string projectStatusIds, string projectBillingTypes);

        [OperationContract]
        List<BusinessUnitLevelGroupedHours> AccountReportGroupByBusinessUnit(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate);

        [OperationContract]
        List<GroupByPerson> AccountReportGroupByPerson(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate);

        [OperationContract]
        List<Person> NewHireReport(DateTime startDate, DateTime endDate, string personStatusIds, string payTypeIds, string practiceIds, bool excludeInternalPractices, string personDivisionIds, string titleIds, string hireDates, string recruiterIds);

        [OperationContract]
        TerminationPersonsInRange TerminationReport(DateTime startDate, DateTime endDate, string payTypeIds, string personStatusIds, string titleIds, string terminationReasonIds, string practiceIds, bool excludeInternalPractices, string personDivisionIds, string recruiterIds, string hireDates, string terminationDates);

        [OperationContract]
        List<TerminationPersonsInRange> TerminationReportGraph(DateTime startDate, DateTime endDate);

        [OperationContract]
        List<ConsultantGroupbyTitleSkill> ConsultingDemandSummary(DateTime startDate, DateTime endDate, string titles, string skills);

        [OperationContract]
        List<ConsultantGroupbyTitleSkill> ConsultingDemandDetailsByTitleSkill(DateTime startDate, DateTime endDate, string titles, string skills, string sortColumns);

        [OperationContract]
        List<ConsultantGroupBySalesStage> ConsultingDemandDetailsBySalesStage(DateTime startDate, DateTime endDate, string titles, string skills, string sortColumns);

        [OperationContract]
        List<ConsultantGroupByMonth> ConsultingDemandDetailsByMonth(DateTime startDate, DateTime endDate, string titles, string skills, string salesStages, string sortColumns, bool isFromPipeLinePopUp);

        [OperationContract]
        Dictionary<string, int> ConsultingDemandGraphsByTitle(DateTime startDate, DateTime endDate, string Title, string salesStages);

        [OperationContract]
        Dictionary<string, int> ConsultingDemandGraphsBySkills(DateTime startDate, DateTime endDate, string Skill, string salesStages);

        [OperationContract]
        List<ConsultantGroupbyTitle> ConsultingDemandTransactionReportByTitle(DateTime startDate, DateTime endDate, string Title, string sortColumns, string salesStages);

        [OperationContract]
        List<ConsultantGroupbySkill> ConsultingDemandTransactionReportBySkill(DateTime startDate, DateTime endDate, string Skill, string sortColumns, string salesStages);

        [OperationContract]
        Dictionary<string, int> ConsultingDemandGrphsGroupsByTitle(DateTime startDate, DateTime endDate, string salesStages);

        [OperationContract]
        Dictionary<string, int> ConsultingDemandGrphsGroupsBySkill(DateTime startDate, DateTime endDate, string salesStages);

        [OperationContract]
        List<AttainmentBillableutlizationReport> AttainmentBillableutlizationReport(DateTime startDate, DateTime endDate);

        [OperationContract]
        List<Project> GetAttainmentProjectListMultiParameters(
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
        bool getFinancialsFromCache);

        [OperationContract]
        List<Project> ProjectAttributionReport(DateTime startDate, DateTime endDate);

        [OperationContract]
        List<ResourceExceptionReport> ZeroHourlyRateExceptionReport(DateTime startDate, DateTime endDate);

        [OperationContract]
        List<ResourceExceptionReport> ResourceAssignedOrUnassignedChargingExceptionReport(DateTime startDate, DateTime endDate, bool isUnassignedReport);

        [OperationContract]
        List<Person> RecruitingMetricsReport(DateTime startDate, DateTime endDate);

        [OperationContract]
        List<ProjectFeedback> ProjectFeedbackReport(string accountIds, string businessGroupIds, DateTime startDate, DateTime endDate, string projectStatus, string projectIds, string directorIds, string practiceIds, bool excludeInternalPractices, string personIds, string titleIds, string reviewStartdateMonths, string reviewEnddateMonths, string projectmanagerIds, string statusIds, bool isExport, string payTypeIds);

        [OperationContract]
        List<BillingReport> BillingReportByCurrency(DateTime startDate, DateTime endDate, string practiceIds, string accountIds, string businessUnitIds, string directorIds, string salesPersonIds, string projectManagerIds, string seniorManagerIds);

        [OperationContract]
        List<BillingReport> BillingReportByHours(DateTime startDate, DateTime endDate, string practiceIds, string accountIds, string businessUnitIds, string directorIds, string salesPersonIds, string projectManagerIds, string seniorManagerIds);

        [OperationContract]
        List<ProjectLevelGroupedHours> NonBillableReport(DateTime startDate, DateTime endDate, string projectNumber, string directorIds, string businessUnitIds, string practiceIds);

        [OperationContract]
        List<BadgedResourcesByTime> BadgedResourcesByTimeReport(string payTypes, string personStatusIds, DateTime startDate, DateTime endDate, int step);

        [OperationContract]
        List<MSBadge> ListBadgeResourcesByType(string paytypes, string personStatuses, DateTime startDate, DateTime endDate, bool isNotBadged, bool isClockNotStart, bool isBlocked, bool isBreak, bool badgedOnProject, bool isBadgedException, bool isNotBadgedException);

        [OperationContract]
        List<GroupByPractice> ResourcesByPracticeReport(string paytypes, string PersonStatuses, string practices, DateTime startDate, DateTime endDate, int step);

        [OperationContract]
        List<GroupbyTitle> ResourcesByTitleReport(string paytypes, string personStatuses, string titles, DateTime startDate, DateTime endDate, int step);

        [OperationContract]
        List<MSBadge> GetBadgeRequestNotApprovedList();

        [OperationContract]
        List<MSBadge> GetAllBadgeDetails(string payTypes, string personStatuses);

        [OperationContract]
        PersonTimeEntriesTotals UtilizationReport(int personId, DateTime startDate, DateTime endDate);

        [OperationContract]
        List<ManagementMeetingReport> ManagedServiceReportByPerson(string paytypes, string personStatuses, DateTime startDate, DateTime endDate);

        [OperationContract]
        void SaveManagedParametersByPerson(string userLogin, decimal actualRevenuePerHour, decimal targetRevenuePerHour, decimal hoursUtilization, decimal targetRevenuePerAnnum);

        [OperationContract]
        RevenueReport GetManagedParametersByPerson(string userLogin);

        [OperationContract]
        List<GroupbyTitle> GetAveragePercentagesByTitles(string paytypes, string personStatuses, string titles, DateTime startDate, DateTime endDate);

        [OperationContract]
        List<Project> ProjectsListWithFilters(
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
           string userLogin
           );

        [OperationContract]
        List<ExpenseSummary> GetExpenseSummaryGroupedByProject(DateTime startDate, DateTime endDate, string clientIds, string divisionIds, string practiceIds, string projectIds, bool active, bool projected, bool completed, bool proposed, bool inActive, bool experimental, bool atRisk);

        [OperationContract]
        List<ExpenseSummary> GetExpenseSummaryGroupedBytype(DateTime startDate, DateTime endDate, string expenseTypeIds);

        [OperationContract]
        List<ExpenseSummary> ExpenseDetailReport(DateTime startDate, DateTime endDate, int? projectId, int? expenseTypeId);

        [OperationContract]
        List<ExpenseSummary> DetailedExpenseSummary(DateTime startDate, DateTime endDate, string clientIds, string divisionIds, string practiceIds, string projectIds, bool active, bool projected, bool completed, bool proposed, bool inActive, bool experimental,bool atRisk, string expenseTypeIds);
    }
}


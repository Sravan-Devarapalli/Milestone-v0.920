﻿using System;
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
        List<PersonLevelGroupedHours> ProjectDetailReportByResource(string projectNumber, int? mileStoneId, DateTime? startDate, DateTime? endDate, string personRoleNames);

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
        GroupByAccount AccountSummaryReportByBusinessUnit(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate);

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
        bool showInactive,
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
    }
}


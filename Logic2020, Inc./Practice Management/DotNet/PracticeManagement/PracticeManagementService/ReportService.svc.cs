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

        public List<PersonLevelGroupedHours> ProjectDetailReportByResource(string projectNumber, int? mileStoneId, DateTime? startDate, DateTime? endDate, string personRoleNames)
        {
            return ReportDAL.ProjectDetailReportByResource(projectNumber, mileStoneId, startDate, endDate, personRoleNames);
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

        public List<PersonLevelPayCheck> TimePeriodSummaryByResourcePayCheck(DateTime startDate, DateTime endDate, bool includePersonsWithNoTimeEntries, string personIds, string seniorityIds, string timescaleNames, string personStatusIds, string personDivisionIds)
        {
            return ReportDAL.TimePeriodSummaryByResourcePayCheck(startDate, endDate, includePersonsWithNoTimeEntries, personIds, seniorityIds, timescaleNames, personStatusIds, personDivisionIds);
        }

        public List<PersonLevelTimeEntriesHistory> TimeEntryAuditReportByPerson(DateTime startDate, DateTime endDate)
        {
            return ReportDAL.TimeEntryAuditReportByPerson(startDate, endDate);
        }

        public List<ProjectLevelTimeEntriesHistory> TimeEntryAuditReportByProject(DateTime startDate, DateTime endDate)
        {
            return ReportDAL.TimeEntryAuditReportByProject(startDate, endDate);
        }

        public GroupByAccount AccountSummaryReportByBusinessUnit(int accountId, string businessUnitIds, DateTime startDate, DateTime endDate)
        {
            return ReportDAL.AccountSummaryReportByBusinessUnit(accountId, businessUnitIds, startDate, endDate);
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

        public List<ConsultantGroupbyTitleSkill> ConsultingDemandDetailsByTitleSkill(DateTime startDate, DateTime endDate, string titles, string skills,string sortColumns)
        {
            return ReportDAL.ConsultingDemandDetailsByTitleSkill(startDate, endDate, titles, skills, sortColumns);
        }

        public List<ConsultantGroupByMonth> ConsultingDemandDetailsByMonth(DateTime startDate, DateTime endDate, string titles, string skills, string sortColumns,bool isFromPipeLinePopUp)
        {
            return ReportDAL.ConsultingDemandDetailsByMonth(startDate, endDate, titles, skills, sortColumns, isFromPipeLinePopUp);
        }

        public  Dictionary<string, int> ConsultingDemandGraphsByTitle(DateTime startDate, DateTime endDate, string Title)
        {
            return ReportDAL.ConsultingDemandGraphsByTitle(startDate, endDate,Title);
        }

        public Dictionary<string, int> ConsultingDemandGraphsBySkills(DateTime startDate, DateTime endDate, string Skill)
        {
            return ReportDAL.ConsultingDemandGraphsBySkills(startDate, endDate, Skill);
        }

        public List<ConsultantGroupbyTitle> ConsultingDemandTransactionReportByTitle(DateTime startDate, DateTime endDate, string Title,string sortColumns)
        {
            return ReportDAL.ConsultingDemandTransactionReportByTitle(startDate, endDate, Title, sortColumns);
        }

        public List<ConsultantGroupbySkill> ConsultingDemandTransactionReportBySkill(DateTime startDate, DateTime endDate, string Skill,string sortColumns)
        {
            return ReportDAL.ConsultingDemandTransactionReportBySkill(startDate, endDate, Skill,sortColumns);
        }

        public Dictionary<string,int> ConsultingDemandGrphsGroupsByTitle(DateTime startDate, DateTime endDate)
        {
            return ReportDAL.ConsultingDemandGrphsGroupsByTitle(startDate, endDate);
        }

        public  Dictionary<string, int> ConsultingDemandGrphsGroupsBySkill(DateTime startDate, DateTime endDate)
        {
            return ReportDAL.ConsultingDemandGrphsGroupsBySkill(startDate, endDate);
        }

        #endregion

    }
}


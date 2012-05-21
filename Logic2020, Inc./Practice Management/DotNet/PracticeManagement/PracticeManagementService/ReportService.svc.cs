using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using DataTransferObjects.Reports;
using DataAccess;
using DataTransferObjects;

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

        public Triple<double, double, double> GetPersonTimeEntriesTotalsByPeriod(int personId, DateTime startDate, DateTime endDate)
        {
            return ReportDAL.GetPersonTimeEntriesTotalsByPeriod(personId, startDate, endDate);
        }

        public List<PersonLevelGroupedHours> TimePeriodSummaryReportByResource(DateTime startDate, DateTime endDate,bool includePersonsWithNoTimeEntries, string personIds, string seniorityIds, string timescaleNames,string personStatusIds, string personDivisionIds)
        {
            return ReportDAL.TimePeriodSummaryReportByResource(startDate, endDate, includePersonsWithNoTimeEntries, personIds, seniorityIds, timescaleNames, personStatusIds, personDivisionIds);
        }

        public List<ProjectLevelGroupedHours> TimePeriodSummaryReportByProject(DateTime startDate, DateTime endDate, string clientIds, string personStatusIds)
        {
            return ReportDAL.TimePeriodSummaryReportByProject(startDate, endDate, clientIds,personStatusIds);
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

    }
}


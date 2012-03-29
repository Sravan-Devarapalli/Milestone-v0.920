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

        public List<PersonLevelGroupedHours> TimePeriodSummaryReportByResource(DateTime startDate, DateTime endDate)
        {
            return ReportDAL.TimePeriodSummaryReportByResource(startDate, endDate);
        }

        public List<ProjectLevelGroupedHours> TimePeriodSummaryReportByProject(DateTime startDate, DateTime endDate)
        {
            return ReportDAL.TimePeriodSummaryReportByProject(startDate, endDate);
        }

        public List<WorkTypeLevelGroupedHours> TimePeriodSummaryReportByWorkType(DateTime startDate, DateTime endDate, string timeTypeCategoryIds, string orderByCerteria)
        {
            return ReportDAL.TimePeriodSummaryReportByWorkType(startDate, endDate, timeTypeCategoryIds, orderByCerteria);
        }


        public List<PersonLevelGroupedHours> ProjectSummaryReportByResource(string projectNumber,int? mileStoneId)
        {
            return ReportDAL.ProjectSummaryReportByResource(projectNumber, mileStoneId);
        }

        public List<WorkTypeLevelGroupedHours> ProjectSummaryReportByWorkType(string projectNumber, string timeTypeCategoryIds, string orderByCerteria)
        {
            return ReportDAL.ProjectSummaryReportByWorkType(projectNumber, timeTypeCategoryIds, orderByCerteria);
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

    }
}


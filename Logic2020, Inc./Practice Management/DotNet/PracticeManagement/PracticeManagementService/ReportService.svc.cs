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
            return ReportDAL.PersonTimeEntriesDetails(personId,  startDate, endDate);
        }

        public List<TimeEntriesGroupByClientAndProject> PersonTimeEntriesSummary(int personId, DateTime startDate, DateTime endDate)
        {
            return ReportDAL.PersonTimeEntriesSummary(personId, startDate, endDate);
        }

        public Triple<double, double, double> GetPersonTimeEntriesTotalsByPeriod(int personId, DateTime startDate, DateTime endDate)
        {
            return ReportDAL.GetPersonTimeEntriesTotalsByPeriod(personId, startDate, endDate);
        }

        public List<PersonLevelGroupedHours> TimePeriodSummaryReportByResource(DateTime startDate, DateTime endDate, string seniorityIds, string orderByCerteria)
        {
            return ReportDAL.TimePeriodSummaryReportByResource( startDate,  endDate,  seniorityIds,  orderByCerteria);
        }

        public List<ProjectLevelGroupedHours> TimePeriodSummaryReportByProject(DateTime startDate, DateTime endDate, string clientIds, string personStatusIds, string orderByCerteria)
        {
            return ReportDAL.TimePeriodSummaryReportByProject(startDate, endDate, clientIds, personStatusIds, orderByCerteria);
        }

        public List<WorkTypeLevelGroupedHours> TimePeriodSummaryReportByWorkType(DateTime startDate, DateTime endDate, string timeTypeCategoryIds, string orderByCerteria)
        {
            return TimePeriodSummaryReportByWorkType( startDate,  endDate,  timeTypeCategoryIds,  orderByCerteria);
        }

   
        public List<PersonLevelGroupedHours> ProjectSummaryReportByResource(string projectNumber, string personRoleIds, string orderByCerteria)
        {
            return ReportDAL.ProjectSummaryReportByResource(projectNumber, personRoleIds, orderByCerteria);
        }

        public List<WorkTypeLevelGroupedHours> ProjectSummaryReportByWorkType(int projectId, string timeTypeCategoryIds, string orderByCerteria)
        {
            return ReportDAL.ProjectSummaryReportByWorkType( projectId, timeTypeCategoryIds, orderByCerteria);
        }

    }
}


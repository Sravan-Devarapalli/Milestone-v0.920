using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataTransferObjects.Reports;
using DataTransferObjects;

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
        Triple<double, double, double> GetPersonTimeEntriesTotalsByPeriod(int personId, DateTime startDate, DateTime endDate);
        
        [OperationContract]
        List<PersonLevelGroupedHours> TimePeriodSummaryReportByResource(DateTime startDate, DateTime endDate, string seniorityIds, string orderByCerteria);

        [OperationContract]
        List<ProjectLevelGroupedHours> TimePeriodSummaryReportByProject(DateTime startDate, DateTime endDate, string clientIds, string personStatusIds, string orderByCerteria);

        [OperationContract]
        List<WorkTypeLevelGroupedHours> TimePeriodSummaryReportByWorkType(DateTime startDate, DateTime endDate, string timeTypeCategoryIds, string orderByCerteria);

        [OperationContract]
        List<PersonLevelGroupedHours> ProjectSummaryReportByResource(string projectNumber, string personRoleIds, string orderByCerteria);

        [OperationContract]
        List<WorkTypeLevelGroupedHours> ProjectSummaryReportByWorkType(string projectNumber, string timeTypeCategoryIds, string orderByCerteria);

        [OperationContract]
        List<PersonLevelGroupedHours> ProjectSummaryReportByResourceAndWorkType(string projectNumber, string personRoleIds, string orderByCerteria);
        
    }
}


﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PraticeManagement.ReportService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ReportService.IReportService")]
    public interface IReportService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/PersonTimeEntriesDetails", ReplyAction="http://tempuri.org/IReportService/PersonTimeEntriesDetailsResponse")]
        DataTransferObjects.Reports.TimeEntriesGroupByClientAndProject[] PersonTimeEntriesDetails(int personId, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/PersonTimeEntriesSummary", ReplyAction="http://tempuri.org/IReportService/PersonTimeEntriesSummaryResponse")]
        DataTransferObjects.Reports.TimeEntriesGroupByClientAndProject[] PersonTimeEntriesSummary(int personId, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/GetPersonTimeEntriesTotalsByPeriod", ReplyAction="http://tempuri.org/IReportService/GetPersonTimeEntriesTotalsByPeriodResponse")]
        DataTransferObjects.Triple<double, double, double> GetPersonTimeEntriesTotalsByPeriod(int personId, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/TimePeriodSummaryReportByResource", ReplyAction="http://tempuri.org/IReportService/TimePeriodSummaryReportByResourceResponse")]
        DataTransferObjects.Reports.PersonLevelGroupedHours[] TimePeriodSummaryReportByResource(System.DateTime startDate, System.DateTime endDate, bool includePersonsWithNoTimeEntries, string personIds, string seniorityIds, string timescaleNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/TimePeriodSummaryReportByProject", ReplyAction="http://tempuri.org/IReportService/TimePeriodSummaryReportByProjectResponse")]
        DataTransferObjects.Reports.ProjectLevelGroupedHours[] TimePeriodSummaryReportByProject(System.DateTime startDate, System.DateTime endDate, string clientIds, string personStatusIds);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/TimePeriodSummaryReportByWorkType", ReplyAction="http://tempuri.org/IReportService/TimePeriodSummaryReportByWorkTypeResponse")]
        DataTransferObjects.Reports.WorkTypeLevelGroupedHours[] TimePeriodSummaryReportByWorkType(System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/ProjectSummaryReportByResource", ReplyAction="http://tempuri.org/IReportService/ProjectSummaryReportByResourceResponse")]
        DataTransferObjects.Reports.PersonLevelGroupedHours[] ProjectSummaryReportByResource(string projectNumber, System.Nullable<int> mileStoneId, System.Nullable<System.DateTime> startDate, System.Nullable<System.DateTime> endDate, string personRoleNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/ProjectDetailReportByResource", ReplyAction="http://tempuri.org/IReportService/ProjectDetailReportByResourceResponse")]
        DataTransferObjects.Reports.PersonLevelGroupedHours[] ProjectDetailReportByResource(string projectNumber, System.Nullable<int> mileStoneId, System.Nullable<System.DateTime> startDate, System.Nullable<System.DateTime> endDate, string personRoleNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/ProjectSummaryReportByWorkType", ReplyAction="http://tempuri.org/IReportService/ProjectSummaryReportByWorkTypeResponse")]
        DataTransferObjects.Reports.WorkTypeLevelGroupedHours[] ProjectSummaryReportByWorkType(string projectNumber, System.Nullable<int> mileStoneId, System.Nullable<System.DateTime> startDate, System.Nullable<System.DateTime> endDate, string categoryNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/GetProjectsByClientId", ReplyAction="http://tempuri.org/IReportService/GetProjectsByClientIdResponse")]
        DataTransferObjects.Project[] GetProjectsByClientId(int clientId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/ProjectSearchByName", ReplyAction="http://tempuri.org/IReportService/ProjectSearchByNameResponse")]
        DataTransferObjects.Project[] ProjectSearchByName(string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/GetMilestonesForProject", ReplyAction="http://tempuri.org/IReportService/GetMilestonesForProjectResponse")]
        DataTransferObjects.Milestone[] GetMilestonesForProject(string projectNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/TimePeriodSummaryByResourcePayCheck", ReplyAction="http://tempuri.org/IReportService/TimePeriodSummaryByResourcePayCheckResponse")]
        DataTransferObjects.Reports.PersonLevelPayCheck[] TimePeriodSummaryByResourcePayCheck(System.DateTime startDate, System.DateTime endDate);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IReportServiceChannel : PraticeManagement.ReportService.IReportService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ReportServiceClient : System.ServiceModel.ClientBase<PraticeManagement.ReportService.IReportService>, PraticeManagement.ReportService.IReportService {
        
      
        
        public ReportServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ReportServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ReportServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ReportServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public DataTransferObjects.Reports.TimeEntriesGroupByClientAndProject[] PersonTimeEntriesDetails(int personId, System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.PersonTimeEntriesDetails(personId, startDate, endDate);
        }
        
        public DataTransferObjects.Reports.TimeEntriesGroupByClientAndProject[] PersonTimeEntriesSummary(int personId, System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.PersonTimeEntriesSummary(personId, startDate, endDate);
        }
        
        public DataTransferObjects.Triple<double, double, double> GetPersonTimeEntriesTotalsByPeriod(int personId, System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.GetPersonTimeEntriesTotalsByPeriod(personId, startDate, endDate);
        }
        
        public DataTransferObjects.Reports.PersonLevelGroupedHours[] TimePeriodSummaryReportByResource(System.DateTime startDate, System.DateTime endDate, bool includePersonsWithNoTimeEntries, string personIds, string seniorityIds, string timescaleNames) {
            return base.Channel.TimePeriodSummaryReportByResource(startDate, endDate, includePersonsWithNoTimeEntries, personIds, seniorityIds, timescaleNames);
        }
        
        public DataTransferObjects.Reports.ProjectLevelGroupedHours[] TimePeriodSummaryReportByProject(System.DateTime startDate, System.DateTime endDate, string clientIds, string personStatusIds) {
            return base.Channel.TimePeriodSummaryReportByProject(startDate, endDate, clientIds, personStatusIds);
        }
        
        public DataTransferObjects.Reports.WorkTypeLevelGroupedHours[] TimePeriodSummaryReportByWorkType(System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.TimePeriodSummaryReportByWorkType(startDate, endDate);
        }
        
        public DataTransferObjects.Reports.PersonLevelGroupedHours[] ProjectSummaryReportByResource(string projectNumber, System.Nullable<int> mileStoneId, System.Nullable<System.DateTime> startDate, System.Nullable<System.DateTime> endDate, string personRoleNames) {
            return base.Channel.ProjectSummaryReportByResource(projectNumber, mileStoneId, startDate, endDate, personRoleNames);
        }
        
        public DataTransferObjects.Reports.PersonLevelGroupedHours[] ProjectDetailReportByResource(string projectNumber, System.Nullable<int> mileStoneId, System.Nullable<System.DateTime> startDate, System.Nullable<System.DateTime> endDate, string personRoleNames) {
            return base.Channel.ProjectDetailReportByResource(projectNumber, mileStoneId, startDate, endDate, personRoleNames);
        }
        
        public DataTransferObjects.Reports.WorkTypeLevelGroupedHours[] ProjectSummaryReportByWorkType(string projectNumber, System.Nullable<int> mileStoneId, System.Nullable<System.DateTime> startDate, System.Nullable<System.DateTime> endDate, string categoryNames) {
            return base.Channel.ProjectSummaryReportByWorkType(projectNumber, mileStoneId, startDate, endDate, categoryNames);
        }
        
        public DataTransferObjects.Project[] GetProjectsByClientId(int clientId) {
            return base.Channel.GetProjectsByClientId(clientId);
        }
        
        public DataTransferObjects.Project[] ProjectSearchByName(string name) {
            return base.Channel.ProjectSearchByName(name);
        }
        
        public DataTransferObjects.Milestone[] GetMilestonesForProject(string projectNumber) {
            return base.Channel.GetMilestonesForProject(projectNumber);
        }
        
        public DataTransferObjects.Reports.PersonLevelPayCheck[] TimePeriodSummaryByResourcePayCheck(System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.TimePeriodSummaryByResourcePayCheck(startDate, endDate);
        }
    }
}


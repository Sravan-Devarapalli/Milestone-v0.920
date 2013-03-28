﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.296
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
        DataTransferObjects.Reports.PersonTimeEntriesTotals GetPersonTimeEntriesTotalsByPeriod(int personId, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/TimePeriodSummaryReportByResource", ReplyAction="http://tempuri.org/IReportService/TimePeriodSummaryReportByResourceResponse")]
        DataTransferObjects.Reports.PersonLevelGroupedHours[] TimePeriodSummaryReportByResource(System.DateTime startDate, System.DateTime endDate, bool includePersonsWithNoTimeEntries, string personIds, string titleIds, string timescaleNames, string personStatusIds, string personDivisionIds);
        
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
        DataTransferObjects.Reports.PersonLevelPayCheck[] TimePeriodSummaryByResourcePayCheck(System.DateTime startDate, System.DateTime endDate, bool includePersonsWithNoTimeEntries, string personIds, string seniorityIds, string timescaleNames, string personStatusIds, string personDivisionIds);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/TimeEntryAuditReportByPerson", ReplyAction="http://tempuri.org/IReportService/TimeEntryAuditReportByPersonResponse")]
        DataTransferObjects.Reports.PersonLevelTimeEntriesHistory[] TimeEntryAuditReportByPerson(System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/TimeEntryAuditReportByProject", ReplyAction="http://tempuri.org/IReportService/TimeEntryAuditReportByProjectResponse")]
        DataTransferObjects.Reports.ProjectLevelTimeEntriesHistory[] TimeEntryAuditReportByProject(System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/AccountSummaryReportByBusinessUnit", ReplyAction="http://tempuri.org/IReportService/AccountSummaryReportByBusinessUnitResponse")]
        DataTransferObjects.Reports.ByAccount.GroupByAccount AccountSummaryReportByBusinessUnit(int accountId, string businessUnitIds, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/AccountSummaryReportByProject", ReplyAction="http://tempuri.org/IReportService/AccountSummaryReportByProjectResponse")]
        DataTransferObjects.Reports.ByAccount.GroupByAccount AccountSummaryReportByProject(int accountId, string businessUnitIds, System.DateTime startDate, System.DateTime endDate, string projectStatusIds, string projectBillingTypes);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/AccountReportGroupByBusinessUnit", ReplyAction="http://tempuri.org/IReportService/AccountReportGroupByBusinessUnitResponse")]
        DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours[] AccountReportGroupByBusinessUnit(int accountId, string businessUnitIds, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/AccountReportGroupByPerson", ReplyAction="http://tempuri.org/IReportService/AccountReportGroupByPersonResponse")]
        DataTransferObjects.Reports.ByAccount.GroupByPerson[] AccountReportGroupByPerson(int accountId, string businessUnitIds, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/NewHireReport", ReplyAction="http://tempuri.org/IReportService/NewHireReportResponse")]
        DataTransferObjects.Person[] NewHireReport(System.DateTime startDate, System.DateTime endDate, string personStatusIds, string payTypeIds, string practiceIds, bool excludeInternalPractices, string personDivisionIds, string titleIds, string hireDates, string recruiterIds);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/TerminationReport", ReplyAction="http://tempuri.org/IReportService/TerminationReportResponse")]
        DataTransferObjects.Reports.HumanCapital.TerminationPersonsInRange TerminationReport(System.DateTime startDate, System.DateTime endDate, string payTypeIds, string personStatusIds, string titleIds, string terminationReasonIds, string practiceIds, bool excludeInternalPractices, string personDivisionIds, string recruiterIds, string hireDates, string terminationDates);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/TerminationReportGraph", ReplyAction="http://tempuri.org/IReportService/TerminationReportGraphResponse")]
        DataTransferObjects.Reports.HumanCapital.TerminationPersonsInRange[] TerminationReportGraph(System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/ConsultingDemandSummary", ReplyAction="http://tempuri.org/IReportService/ConsultingDemandSummaryResponse")]
        DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill[] ConsultingDemandSummary(System.DateTime startDate, System.DateTime endDate, string titles, string skills);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/ConsultingDemandDetailsByTitleSkill", ReplyAction="http://tempuri.org/IReportService/ConsultingDemandDetailsByTitleSkillResponse")]
        DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill[] ConsultingDemandDetailsByTitleSkill(System.DateTime startDate, System.DateTime endDate, string titles, string skills);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/ConsultingDemandDetailsByMonth", ReplyAction="http://tempuri.org/IReportService/ConsultingDemandDetailsByMonthResponse")]
        DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupByMonth[] ConsultingDemandDetailsByMonth(System.DateTime startDate, System.DateTime endDate, string titles, string skills);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/ConsultingDemandGraphsByTitle", ReplyAction="http://tempuri.org/IReportService/ConsultingDemandGraphsByTitleResponse")]
        System.Collections.Generic.Dictionary<string, int> ConsultingDemandGraphsByTitle(System.DateTime startDate, System.DateTime endDate, string Title);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/ConsultingDemandGraphsBySkills", ReplyAction="http://tempuri.org/IReportService/ConsultingDemandGraphsBySkillsResponse")]
        System.Collections.Generic.Dictionary<string, int> ConsultingDemandGraphsBySkills(System.DateTime startDate, System.DateTime endDate, string Skill);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/ConsultingDemandTransactionReportByTitle", ReplyAction="http://tempuri.org/IReportService/ConsultingDemandTransactionReportByTitleRespons" +
            "e")]
        DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitle[] ConsultingDemandTransactionReportByTitle(System.DateTime startDate, System.DateTime endDate, string Title);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/ConsultingDemandTransactionReportBySkill", ReplyAction="http://tempuri.org/IReportService/ConsultingDemandTransactionReportBySkillRespons" +
            "e")]
        DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbySkill[] ConsultingDemandTransactionReportBySkill(System.DateTime startDate, System.DateTime endDate, string Skill);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/ConsultingDemandGrphsGroupsByTitle", ReplyAction="http://tempuri.org/IReportService/ConsultingDemandGrphsGroupsByTitleResponse")]
        System.Collections.Generic.Dictionary<string, int> ConsultingDemandGrphsGroupsByTitle(System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportService/ConsultingDemandGrphsGroupsBySkill", ReplyAction="http://tempuri.org/IReportService/ConsultingDemandGrphsGroupsBySkillResponse")]
        System.Collections.Generic.Dictionary<string, int> ConsultingDemandGrphsGroupsBySkill(System.DateTime startDate, System.DateTime endDate);
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
        
        public DataTransferObjects.Reports.PersonTimeEntriesTotals GetPersonTimeEntriesTotalsByPeriod(int personId, System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.GetPersonTimeEntriesTotalsByPeriod(personId, startDate, endDate);
        }
        
        public DataTransferObjects.Reports.PersonLevelGroupedHours[] TimePeriodSummaryReportByResource(System.DateTime startDate, System.DateTime endDate, bool includePersonsWithNoTimeEntries, string personIds, string titleIds, string timescaleNames, string personStatusIds, string personDivisionIds) {
            return base.Channel.TimePeriodSummaryReportByResource(startDate, endDate, includePersonsWithNoTimeEntries, personIds, titleIds, timescaleNames, personStatusIds, personDivisionIds);
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
        
        public DataTransferObjects.Reports.PersonLevelPayCheck[] TimePeriodSummaryByResourcePayCheck(System.DateTime startDate, System.DateTime endDate, bool includePersonsWithNoTimeEntries, string personIds, string seniorityIds, string timescaleNames, string personStatusIds, string personDivisionIds) {
            return base.Channel.TimePeriodSummaryByResourcePayCheck(startDate, endDate, includePersonsWithNoTimeEntries, personIds, seniorityIds, timescaleNames, personStatusIds, personDivisionIds);
        }
        
        public DataTransferObjects.Reports.PersonLevelTimeEntriesHistory[] TimeEntryAuditReportByPerson(System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.TimeEntryAuditReportByPerson(startDate, endDate);
        }
        
        public DataTransferObjects.Reports.ProjectLevelTimeEntriesHistory[] TimeEntryAuditReportByProject(System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.TimeEntryAuditReportByProject(startDate, endDate);
        }
        
        public DataTransferObjects.Reports.ByAccount.GroupByAccount AccountSummaryReportByBusinessUnit(int accountId, string businessUnitIds, System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.AccountSummaryReportByBusinessUnit(accountId, businessUnitIds, startDate, endDate);
        }
        
        public DataTransferObjects.Reports.ByAccount.GroupByAccount AccountSummaryReportByProject(int accountId, string businessUnitIds, System.DateTime startDate, System.DateTime endDate, string projectStatusIds, string projectBillingTypes) {
            return base.Channel.AccountSummaryReportByProject(accountId, businessUnitIds, startDate, endDate, projectStatusIds, projectBillingTypes);
        }
        
        public DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours[] AccountReportGroupByBusinessUnit(int accountId, string businessUnitIds, System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.AccountReportGroupByBusinessUnit(accountId, businessUnitIds, startDate, endDate);
        }
        
        public DataTransferObjects.Reports.ByAccount.GroupByPerson[] AccountReportGroupByPerson(int accountId, string businessUnitIds, System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.AccountReportGroupByPerson(accountId, businessUnitIds, startDate, endDate);
        }
        
        public DataTransferObjects.Person[] NewHireReport(System.DateTime startDate, System.DateTime endDate, string personStatusIds, string payTypeIds, string practiceIds, bool excludeInternalPractices, string personDivisionIds, string titleIds, string hireDates, string recruiterIds) {
            return base.Channel.NewHireReport(startDate, endDate, personStatusIds, payTypeIds, practiceIds, excludeInternalPractices, personDivisionIds, titleIds, hireDates, recruiterIds);
        }
        
        public DataTransferObjects.Reports.HumanCapital.TerminationPersonsInRange TerminationReport(System.DateTime startDate, System.DateTime endDate, string payTypeIds, string personStatusIds, string titleIds, string terminationReasonIds, string practiceIds, bool excludeInternalPractices, string personDivisionIds, string recruiterIds, string hireDates, string terminationDates) {
            return base.Channel.TerminationReport(startDate, endDate, payTypeIds, personStatusIds, titleIds, terminationReasonIds, practiceIds, excludeInternalPractices, personDivisionIds, recruiterIds, hireDates, terminationDates);
        }
        
        public DataTransferObjects.Reports.HumanCapital.TerminationPersonsInRange[] TerminationReportGraph(System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.TerminationReportGraph(startDate, endDate);
        }
        
        public DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill[] ConsultingDemandSummary(System.DateTime startDate, System.DateTime endDate, string titles, string skills) {
            return base.Channel.ConsultingDemandSummary(startDate, endDate, titles, skills);
        }
        
        public DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitleSkill[] ConsultingDemandDetailsByTitleSkill(System.DateTime startDate, System.DateTime endDate, string titles, string skills) {
            return base.Channel.ConsultingDemandDetailsByTitleSkill(startDate, endDate, titles, skills);
        }
        
        public DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupByMonth[] ConsultingDemandDetailsByMonth(System.DateTime startDate, System.DateTime endDate, string titles, string skills) {
            return base.Channel.ConsultingDemandDetailsByMonth(startDate, endDate, titles, skills);
        }
        
        public System.Collections.Generic.Dictionary<string, int> ConsultingDemandGraphsByTitle(System.DateTime startDate, System.DateTime endDate, string Title) {
            return base.Channel.ConsultingDemandGraphsByTitle(startDate, endDate, Title);
        }
        
        public System.Collections.Generic.Dictionary<string, int> ConsultingDemandGraphsBySkills(System.DateTime startDate, System.DateTime endDate, string Skill) {
            return base.Channel.ConsultingDemandGraphsBySkills(startDate, endDate, Skill);
        }
        
        public DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbyTitle[] ConsultingDemandTransactionReportByTitle(System.DateTime startDate, System.DateTime endDate, string Title) {
            return base.Channel.ConsultingDemandTransactionReportByTitle(startDate, endDate, Title);
        }
        
        public DataTransferObjects.Reports.ConsultingDemand.ConsultantGroupbySkill[] ConsultingDemandTransactionReportBySkill(System.DateTime startDate, System.DateTime endDate, string Skill) {
            return base.Channel.ConsultingDemandTransactionReportBySkill(startDate, endDate, Skill);
        }
        
        public System.Collections.Generic.Dictionary<string, int> ConsultingDemandGrphsGroupsByTitle(System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.ConsultingDemandGrphsGroupsByTitle(startDate, endDate);
        }
        
        public System.Collections.Generic.Dictionary<string, int> ConsultingDemandGrphsGroupsBySkill(System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.ConsultingDemandGrphsGroupsBySkill(startDate, endDate);
        }
    }
}


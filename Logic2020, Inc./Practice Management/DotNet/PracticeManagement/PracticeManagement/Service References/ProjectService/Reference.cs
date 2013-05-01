﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.296
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PraticeManagement.ProjectService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ProjectService.IProjectService")]
    public interface IProjectService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetProjectsListByProjectGroupId", ReplyAction="http://tempuri.org/IProjectService/GetProjectsListByProjectGroupIdResponse")]
        DataTransferObjects.Project[] GetProjectsListByProjectGroupId(int projectGroupId, bool isInternal, int personId, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetBusinessDevelopmentProject", ReplyAction="http://tempuri.org/IProjectService/GetBusinessDevelopmentProjectResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataTransferObjects.ProjectsGroupedByClient))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataTransferObjects.ProjectsGroupedByClientGroup))]
        DataTransferObjects.Project GetBusinessDevelopmentProject();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetProjectByIdShort", ReplyAction="http://tempuri.org/IProjectService/GetProjectByIdShortResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataTransferObjects.ProjectsGroupedByClient))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataTransferObjects.ProjectsGroupedByClientGroup))]
        DataTransferObjects.Project GetProjectByIdShort(int projectId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetTimeTypesByProjectId", ReplyAction="http://tempuri.org/IProjectService/GetTimeTypesByProjectIdResponse")]
        DataTransferObjects.TimeEntry.TimeTypeRecord[] GetTimeTypesByProjectId(int projectId, bool IsOnlyActive, System.Nullable<System.DateTime> startDate, System.Nullable<System.DateTime> endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/SetProjectTimeTypes", ReplyAction="http://tempuri.org/IProjectService/SetProjectTimeTypesResponse")]
        void SetProjectTimeTypes(int projectId, string projectTimeTypesList);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetIsHourlyRevenueByPeriod", ReplyAction="http://tempuri.org/IProjectService/GetIsHourlyRevenueByPeriodResponse")]
        System.Collections.Generic.Dictionary<System.DateTime, bool> GetIsHourlyRevenueByPeriod(int projectId, int personId, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetProjectShortByProjectNumber", ReplyAction="http://tempuri.org/IProjectService/GetProjectShortByProjectNumberResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataTransferObjects.ProjectsGroupedByClient))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataTransferObjects.ProjectsGroupedByClientGroup))]
        DataTransferObjects.Project GetProjectShortByProjectNumber(string projectNumber, System.Nullable<int> milestoneId, System.Nullable<System.DateTime> startDate, System.Nullable<System.DateTime> endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetTimeTypesInUseDetailsByProject", ReplyAction="http://tempuri.org/IProjectService/GetTimeTypesInUseDetailsByProjectResponse")]
        DataTransferObjects.TimeEntry.TimeTypeRecord[] GetTimeTypesInUseDetailsByProject(int projectId, string timeTypeIds);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/AttachOpportunityToProject", ReplyAction="http://tempuri.org/IProjectService/AttachOpportunityToProjectResponse")]
        string AttachOpportunityToProject(int projectId, int opportunityId, string userLogin, System.Nullable<int> pricingListId, bool link);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/CSATInsert", ReplyAction="http://tempuri.org/IProjectService/CSATInsertResponse")]
        void CSATInsert(DataTransferObjects.ProjectCSAT projectCSAT, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/CSATDelete", ReplyAction="http://tempuri.org/IProjectService/CSATDeleteResponse")]
        void CSATDelete(int projectCSATId, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/CSATUpdate", ReplyAction="http://tempuri.org/IProjectService/CSATUpdateResponse")]
        void CSATUpdate(DataTransferObjects.ProjectCSAT projectCSAT, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/CSATCopyFromExistingCSAT", ReplyAction="http://tempuri.org/IProjectService/CSATCopyFromExistingCSATResponse")]
        void CSATCopyFromExistingCSAT(DataTransferObjects.ProjectCSAT projectCSAT, int copyProjectCSATId, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/CSATList", ReplyAction="http://tempuri.org/IProjectService/CSATListResponse")]
        DataTransferObjects.ProjectCSAT[] CSATList(System.Nullable<int> projectId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/ProjectGetById", ReplyAction="http://tempuri.org/IProjectService/ProjectGetByIdResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataTransferObjects.ProjectsGroupedByClient))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataTransferObjects.ProjectsGroupedByClientGroup))]
        DataTransferObjects.Project ProjectGetById(int projectId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetProjectLastChangeDateFortheGivenStatus", ReplyAction="http://tempuri.org/IProjectService/GetProjectLastChangeDateFortheGivenStatusRespo" +
            "nse")]
        System.DateTime GetProjectLastChangeDateFortheGivenStatus(int projectId, int projectStatusId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetProjectsComputedFinancials", ReplyAction="http://tempuri.org/IProjectService/GetProjectsComputedFinancialsResponse")]
        DataTransferObjects.ComputedFinancials GetProjectsComputedFinancials(int projectId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetProjectMilestonesFinancials", ReplyAction="http://tempuri.org/IProjectService/GetProjectMilestonesFinancialsResponse")]
        System.Data.DataSet GetProjectMilestonesFinancials(int projectId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/ProjectCountByClient", ReplyAction="http://tempuri.org/IProjectService/ProjectCountByClientResponse")]
        int ProjectCountByClient(int clientId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetProjectListByDateRange", ReplyAction="http://tempuri.org/IProjectService/GetProjectListByDateRangeResponse")]
        DataTransferObjects.Project[] GetProjectListByDateRange(bool showProjected, bool showCompleted, bool showActive, bool showInternal, bool showExperimental, bool showInactive, System.DateTime periodStart, System.DateTime periodEnd);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/ListProjectsByClient", ReplyAction="http://tempuri.org/IProjectService/ListProjectsByClientResponse")]
        DataTransferObjects.Project[] ListProjectsByClient(System.Nullable<int> clientId, string viewerUsername);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/ListProjectsByClientShort", ReplyAction="http://tempuri.org/IProjectService/ListProjectsByClientShortResponse")]
        DataTransferObjects.Project[] ListProjectsByClientShort(System.Nullable<int> clientId, bool IsOnlyActiveAndProjective, bool IsOnlyActiveAndInternal, bool IsOnlyEnternalProjects);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/ListProjectsByClientAndPersonInPeriod", ReplyAction="http://tempuri.org/IProjectService/ListProjectsByClientAndPersonInPeriodResponse")]
        DataTransferObjects.Project[] ListProjectsByClientAndPersonInPeriod(int clientId, bool isOnlyActiveAndInternal, bool isOnlyEnternalProjects, int personId, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/ListProjectsByClientWithSort", ReplyAction="http://tempuri.org/IProjectService/ListProjectsByClientWithSortResponse")]
        DataTransferObjects.Project[] ListProjectsByClientWithSort(System.Nullable<int> clientId, string viewerUsername, string sortBy);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/CloneProject", ReplyAction="http://tempuri.org/IProjectService/CloneProjectResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataTransferObjects.ProjectsGroupedByClient))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataTransferObjects.ProjectsGroupedByClientGroup))]
        int CloneProject(DataTransferObjects.ContextObjects.ProjectCloningContext context);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetProjectListCustom", ReplyAction="http://tempuri.org/IProjectService/GetProjectListCustomResponse")]
        DataTransferObjects.Project[] GetProjectListCustom(bool projected, bool completed, bool active, bool experimantal);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/ProjectListAllMultiParameters", ReplyAction="http://tempuri.org/IProjectService/ProjectListAllMultiParametersResponse")]
        DataTransferObjects.Project[] ProjectListAllMultiParameters(
                    string clientIds, 
                    bool showProjected, 
                    bool showCompleted, 
                    bool showActive, 
                    bool showInternal, 
                    bool showExperimental, 
                    bool showInactive, 
                    System.DateTime periodStart, 
                    System.DateTime periodEnd, 
                    string salespersonIdsList, 
                    string projectOwnerIdsList, 
                    string practiceIdsList, 
                    string projectGroupIdsList, 
                    DataTransferObjects.ProjectCalculateRangeType includeCurentYearFinancials, 
                    bool excludeInternalPractices, 
                    string userLogin, 
                    bool useActuals, 
                    bool getFinancialsFromCache);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/IsProjectSummaryCachedToday", ReplyAction="http://tempuri.org/IProjectService/IsProjectSummaryCachedTodayResponse")]
        bool IsProjectSummaryCachedToday();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetProjectListWithFinancials", ReplyAction="http://tempuri.org/IProjectService/GetProjectListWithFinancialsResponse")]
        DataTransferObjects.Project[] GetProjectListWithFinancials(string clientIds, bool showProjected, bool showCompleted, bool showActive, bool showInternal, bool showExperimental, bool showInactive, System.DateTime periodStart, System.DateTime periodEnd, string salespersonIdsList, string projectOwnerIdsList, string practiceIdsList, string projectGroupIdsList, bool excludeInternalPractices);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetProjectListGroupByPracticeManagers", ReplyAction="http://tempuri.org/IProjectService/GetProjectListGroupByPracticeManagersResponse")]
        DataTransferObjects.MilestonePerson[] GetProjectListGroupByPracticeManagers(string clientIds, bool showProjected, bool showCompleted, bool showActive, bool showInternal, bool showExperimental, bool showInactive, System.DateTime periodStart, System.DateTime periodEnd, string salespersonIdsList, string projectOwnerIdsList, string practiceIdsList, string projectGroupIdsList, bool excludeInternalPractices);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetBenchList", ReplyAction="http://tempuri.org/IProjectService/GetBenchListResponse")]
        DataTransferObjects.Project[] GetBenchList(DataTransferObjects.ContextObjects.BenchReportContext context);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetBenchListWithoutBenchTotalAndAdminCosts", ReplyAction="http://tempuri.org/IProjectService/GetBenchListWithoutBenchTotalAndAdminCostsResp" +
            "onse")]
        DataTransferObjects.Project[] GetBenchListWithoutBenchTotalAndAdminCosts(DataTransferObjects.ContextObjects.BenchReportContext context);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/ProjectSearchText", ReplyAction="http://tempuri.org/IProjectService/ProjectSearchTextResponse")]
        DataTransferObjects.Project[] ProjectSearchText(string looked, int personId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetProjectDetailWithoutMilestones", ReplyAction="http://tempuri.org/IProjectService/GetProjectDetailWithoutMilestonesResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataTransferObjects.ProjectsGroupedByClient))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataTransferObjects.ProjectsGroupedByClientGroup))]
        DataTransferObjects.Project GetProjectDetailWithoutMilestones(int projectId, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/SaveProjectDetail", ReplyAction="http://tempuri.org/IProjectService/SaveProjectDetailResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataTransferObjects.ProjectsGroupedByClient))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataTransferObjects.ProjectsGroupedByClientGroup))]
        int SaveProjectDetail(DataTransferObjects.Project project, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/MonthMiniReport", ReplyAction="http://tempuri.org/IProjectService/MonthMiniReportResponse")]
        string MonthMiniReport(System.DateTime month, string userName, bool showProjected, bool showCompleted, bool showActive, bool showExperimental, bool showInternal, bool showInactive, bool useActuals);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/PersonStartsReport", ReplyAction="http://tempuri.org/IProjectService/PersonStartsReportResponse")]
        DataTransferObjects.PersonStats[] PersonStartsReport(System.DateTime startDate, System.DateTime endDate, string userName, System.Nullable<int> salespersonId, System.Nullable<int> practiceManagerId, bool showProjected, bool showCompleted, bool showActive, bool showExperimental, bool showInternal, bool showInactive, bool useActuals);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetProjectId", ReplyAction="http://tempuri.org/IProjectService/GetProjectIdResponse")]
        System.Nullable<int> GetProjectId(string projectNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/PersonBudgetListByYear", ReplyAction="http://tempuri.org/IProjectService/PersonBudgetListByYearResponse")]
        DataTransferObjects.ProjectsGroupedByPerson[] PersonBudgetListByYear(int year, DataTransferObjects.BudgetCategoryType categoryType);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/PracticeBudgetListByYear", ReplyAction="http://tempuri.org/IProjectService/PracticeBudgetListByYearResponse")]
        DataTransferObjects.ProjectsGroupedByPractice[] PracticeBudgetListByYear(int year);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/CategoryItemBudgetSave", ReplyAction="http://tempuri.org/IProjectService/CategoryItemBudgetSaveResponse")]
        void CategoryItemBudgetSave(int itemId, DataTransferObjects.BudgetCategoryType categoryType, System.DateTime monthStartDate, DataTransferObjects.PracticeManagementCurrency amount);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/CalculateBudgetForPersons", ReplyAction="http://tempuri.org/IProjectService/CalculateBudgetForPersonsResponse")]
        DataTransferObjects.ProjectsGroupedByPerson[] CalculateBudgetForPersons(System.DateTime startDate, System.DateTime endDate, bool showProjected, bool showCompleted, bool showActive, bool showInternal, bool showExperimental, bool showInactive, string practiceIdsList, bool excludeInternalPractices, string personIds, DataTransferObjects.BudgetCategoryType categoryType);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/CalculateBudgetForPractices", ReplyAction="http://tempuri.org/IProjectService/CalculateBudgetForPracticesResponse")]
        DataTransferObjects.ProjectsGroupedByPractice[] CalculateBudgetForPractices(System.DateTime startDate, System.DateTime endDate, bool showProjected, bool showCompleted, bool showActive, bool showInternal, bool showExperimental, bool showInactive, string practiceIdsList, bool excludeInternalPractices);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/CategoryItemsSaveFromXML", ReplyAction="http://tempuri.org/IProjectService/CategoryItemsSaveFromXMLResponse")]
        void CategoryItemsSaveFromXML(DataTransferObjects.CategoryItemBudget[] categoryItems, int year);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/ProjectDelete", ReplyAction="http://tempuri.org/IProjectService/ProjectDeleteResponse")]
        void ProjectDelete(int projectId, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/GetProjectExpensesForProject", ReplyAction="http://tempuri.org/IProjectService/GetProjectExpensesForProjectResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataTransferObjects.ProjectsGroupedByClient))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DataTransferObjects.ProjectsGroupedByClientGroup))]
        DataTransferObjects.ProjectExpense[] GetProjectExpensesForProject(DataTransferObjects.ProjectExpense entity);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/AllProjectsWithFinancialTotalsAndPersons", ReplyAction="http://tempuri.org/IProjectService/AllProjectsWithFinancialTotalsAndPersonsRespon" +
            "se")]
        DataTransferObjects.Project[] AllProjectsWithFinancialTotalsAndPersons();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/IsUserHasPermissionOnProject", ReplyAction="http://tempuri.org/IProjectService/IsUserHasPermissionOnProjectResponse")]
        bool IsUserHasPermissionOnProject(string user, int id, bool isProjectId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/IsUserIsOwnerOfProject", ReplyAction="http://tempuri.org/IProjectService/IsUserIsOwnerOfProjectResponse")]
        bool IsUserIsOwnerOfProject(string user, int id, bool isProjectId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProjectService/IsUserIsProjectOwner", ReplyAction="http://tempuri.org/IProjectService/IsUserIsProjectOwnerResponse")]
        bool IsUserIsProjectOwner(string user, int id);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IProjectServiceChannel : PraticeManagement.ProjectService.IProjectService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ProjectServiceClient : System.ServiceModel.ClientBase<PraticeManagement.ProjectService.IProjectService>, PraticeManagement.ProjectService.IProjectService {
        
      
        
        public ProjectServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ProjectServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProjectServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProjectServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public DataTransferObjects.Project[] GetProjectsListByProjectGroupId(int projectGroupId, bool isInternal, int personId, System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.GetProjectsListByProjectGroupId(projectGroupId, isInternal, personId, startDate, endDate);
        }
        
        public DataTransferObjects.Project GetBusinessDevelopmentProject() {
            return base.Channel.GetBusinessDevelopmentProject();
        }
        
        public DataTransferObjects.Project GetProjectByIdShort(int projectId) {
            return base.Channel.GetProjectByIdShort(projectId);
        }
        
        public DataTransferObjects.TimeEntry.TimeTypeRecord[] GetTimeTypesByProjectId(int projectId, bool IsOnlyActive, System.Nullable<System.DateTime> startDate, System.Nullable<System.DateTime> endDate) {
            return base.Channel.GetTimeTypesByProjectId(projectId, IsOnlyActive, startDate, endDate);
        }
        
        public void SetProjectTimeTypes(int projectId, string projectTimeTypesList) {
            base.Channel.SetProjectTimeTypes(projectId, projectTimeTypesList);
        }
        
        public System.Collections.Generic.Dictionary<System.DateTime, bool> GetIsHourlyRevenueByPeriod(int projectId, int personId, System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.GetIsHourlyRevenueByPeriod(projectId, personId, startDate, endDate);
        }
        
        public DataTransferObjects.Project GetProjectShortByProjectNumber(string projectNumber, System.Nullable<int> milestoneId, System.Nullable<System.DateTime> startDate, System.Nullable<System.DateTime> endDate) {
            return base.Channel.GetProjectShortByProjectNumber(projectNumber, milestoneId, startDate, endDate);
        }
        
        public DataTransferObjects.TimeEntry.TimeTypeRecord[] GetTimeTypesInUseDetailsByProject(int projectId, string timeTypeIds) {
            return base.Channel.GetTimeTypesInUseDetailsByProject(projectId, timeTypeIds);
        }
        
        public string AttachOpportunityToProject(int projectId, int opportunityId, string userLogin, System.Nullable<int> pricingListId, bool link) {
            return base.Channel.AttachOpportunityToProject(projectId, opportunityId, userLogin, pricingListId, link);
        }
        
        public void CSATInsert(DataTransferObjects.ProjectCSAT projectCSAT, string userLogin) {
            base.Channel.CSATInsert(projectCSAT, userLogin);
        }
        
        public void CSATDelete(int projectCSATId, string userLogin) {
            base.Channel.CSATDelete(projectCSATId, userLogin);
        }
        
        public void CSATUpdate(DataTransferObjects.ProjectCSAT projectCSAT, string userLogin) {
            base.Channel.CSATUpdate(projectCSAT, userLogin);
        }
        
        public void CSATCopyFromExistingCSAT(DataTransferObjects.ProjectCSAT projectCSAT, int copyProjectCSATId, string userLogin) {
            base.Channel.CSATCopyFromExistingCSAT(projectCSAT, copyProjectCSATId, userLogin);
        }
        
        public DataTransferObjects.ProjectCSAT[] CSATList(System.Nullable<int> projectId) {
            return base.Channel.CSATList(projectId);
        }
        
        public DataTransferObjects.Project ProjectGetById(int projectId) {
            return base.Channel.ProjectGetById(projectId);
        }
        
        public System.DateTime GetProjectLastChangeDateFortheGivenStatus(int projectId, int projectStatusId) {
            return base.Channel.GetProjectLastChangeDateFortheGivenStatus(projectId, projectStatusId);
        }
        
        public DataTransferObjects.ComputedFinancials GetProjectsComputedFinancials(int projectId) {
            return base.Channel.GetProjectsComputedFinancials(projectId);
        }
        
        public System.Data.DataSet GetProjectMilestonesFinancials(int projectId) {
            return base.Channel.GetProjectMilestonesFinancials(projectId);
        }
        
        public int ProjectCountByClient(int clientId) {
            return base.Channel.ProjectCountByClient(clientId);
        }
        
        public DataTransferObjects.Project[] GetProjectListByDateRange(bool showProjected, bool showCompleted, bool showActive, bool showInternal, bool showExperimental, bool showInactive, System.DateTime periodStart, System.DateTime periodEnd) {
            return base.Channel.GetProjectListByDateRange(showProjected, showCompleted, showActive, showInternal, showExperimental, showInactive, periodStart, periodEnd);
        }
        
        public DataTransferObjects.Project[] ListProjectsByClient(System.Nullable<int> clientId, string viewerUsername) {
            return base.Channel.ListProjectsByClient(clientId, viewerUsername);
        }
        
        public DataTransferObjects.Project[] ListProjectsByClientShort(System.Nullable<int> clientId, bool IsOnlyActiveAndProjective, bool IsOnlyActiveAndInternal, bool IsOnlyEnternalProjects) {
            return base.Channel.ListProjectsByClientShort(clientId, IsOnlyActiveAndProjective, IsOnlyActiveAndInternal, IsOnlyEnternalProjects);
        }
        
        public DataTransferObjects.Project[] ListProjectsByClientAndPersonInPeriod(int clientId, bool isOnlyActiveAndInternal, bool isOnlyEnternalProjects, int personId, System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.ListProjectsByClientAndPersonInPeriod(clientId, isOnlyActiveAndInternal, isOnlyEnternalProjects, personId, startDate, endDate);
        }
        
        public DataTransferObjects.Project[] ListProjectsByClientWithSort(System.Nullable<int> clientId, string viewerUsername, string sortBy) {
            return base.Channel.ListProjectsByClientWithSort(clientId, viewerUsername, sortBy);
        }
        
        public int CloneProject(DataTransferObjects.ContextObjects.ProjectCloningContext context) {
            return base.Channel.CloneProject(context);
        }
        
        public DataTransferObjects.Project[] GetProjectListCustom(bool projected, bool completed, bool active, bool experimantal) {
            return base.Channel.GetProjectListCustom(projected, completed, active, experimantal);
        }
        
        public DataTransferObjects.Project[] ProjectListAllMultiParameters(
                    string clientIds, 
                    bool showProjected, 
                    bool showCompleted, 
                    bool showActive, 
                    bool showInternal, 
                    bool showExperimental, 
                    bool showInactive, 
                    System.DateTime periodStart, 
                    System.DateTime periodEnd, 
                    string salespersonIdsList, 
                    string projectOwnerIdsList, 
                    string practiceIdsList, 
                    string projectGroupIdsList, 
                    DataTransferObjects.ProjectCalculateRangeType includeCurentYearFinancials, 
                    bool excludeInternalPractices, 
                    string userLogin, 
                    bool useActuals, 
                    bool getFinancialsFromCache) {
            return base.Channel.ProjectListAllMultiParameters(clientIds, showProjected, showCompleted, showActive, showInternal, showExperimental, showInactive, periodStart, periodEnd, salespersonIdsList, projectOwnerIdsList, practiceIdsList, projectGroupIdsList, includeCurentYearFinancials, excludeInternalPractices, userLogin, useActuals, getFinancialsFromCache);
        }
        
        public bool IsProjectSummaryCachedToday() {
            return base.Channel.IsProjectSummaryCachedToday();
        }
        
        public DataTransferObjects.Project[] GetProjectListWithFinancials(string clientIds, bool showProjected, bool showCompleted, bool showActive, bool showInternal, bool showExperimental, bool showInactive, System.DateTime periodStart, System.DateTime periodEnd, string salespersonIdsList, string projectOwnerIdsList, string practiceIdsList, string projectGroupIdsList, bool excludeInternalPractices) {
            return base.Channel.GetProjectListWithFinancials(clientIds, showProjected, showCompleted, showActive, showInternal, showExperimental, showInactive, periodStart, periodEnd, salespersonIdsList, projectOwnerIdsList, practiceIdsList, projectGroupIdsList, excludeInternalPractices);
        }
        
        public DataTransferObjects.MilestonePerson[] GetProjectListGroupByPracticeManagers(string clientIds, bool showProjected, bool showCompleted, bool showActive, bool showInternal, bool showExperimental, bool showInactive, System.DateTime periodStart, System.DateTime periodEnd, string salespersonIdsList, string projectOwnerIdsList, string practiceIdsList, string projectGroupIdsList, bool excludeInternalPractices) {
            return base.Channel.GetProjectListGroupByPracticeManagers(clientIds, showProjected, showCompleted, showActive, showInternal, showExperimental, showInactive, periodStart, periodEnd, salespersonIdsList, projectOwnerIdsList, practiceIdsList, projectGroupIdsList, excludeInternalPractices);
        }
        
        public DataTransferObjects.Project[] GetBenchList(DataTransferObjects.ContextObjects.BenchReportContext context) {
            return base.Channel.GetBenchList(context);
        }
        
        public DataTransferObjects.Project[] GetBenchListWithoutBenchTotalAndAdminCosts(DataTransferObjects.ContextObjects.BenchReportContext context) {
            return base.Channel.GetBenchListWithoutBenchTotalAndAdminCosts(context);
        }
        
        public DataTransferObjects.Project[] ProjectSearchText(string looked, int personId) {
            return base.Channel.ProjectSearchText(looked, personId);
        }
        
        public DataTransferObjects.Project GetProjectDetailWithoutMilestones(int projectId, string userName) {
            return base.Channel.GetProjectDetailWithoutMilestones(projectId, userName);
        }
        
        public int SaveProjectDetail(DataTransferObjects.Project project, string userName) {
            return base.Channel.SaveProjectDetail(project, userName);
        }
        
        public string MonthMiniReport(System.DateTime month, string userName, bool showProjected, bool showCompleted, bool showActive, bool showExperimental, bool showInternal, bool showInactive, bool useActuals) {
            return base.Channel.MonthMiniReport(month, userName, showProjected, showCompleted, showActive, showExperimental, showInternal, showInactive, useActuals);
        }
        
        public DataTransferObjects.PersonStats[] PersonStartsReport(System.DateTime startDate, System.DateTime endDate, string userName, System.Nullable<int> salespersonId, System.Nullable<int> practiceManagerId, bool showProjected, bool showCompleted, bool showActive, bool showExperimental, bool showInternal, bool showInactive, bool useActuals) {
            return base.Channel.PersonStartsReport(startDate, endDate, userName, salespersonId, practiceManagerId, showProjected, showCompleted, showActive, showExperimental, showInternal, showInactive, useActuals);
        }
        
        public System.Nullable<int> GetProjectId(string projectNumber) {
            return base.Channel.GetProjectId(projectNumber);
        }
        
        public DataTransferObjects.ProjectsGroupedByPerson[] PersonBudgetListByYear(int year, DataTransferObjects.BudgetCategoryType categoryType) {
            return base.Channel.PersonBudgetListByYear(year, categoryType);
        }
        
        public DataTransferObjects.ProjectsGroupedByPractice[] PracticeBudgetListByYear(int year) {
            return base.Channel.PracticeBudgetListByYear(year);
        }
        
        public void CategoryItemBudgetSave(int itemId, DataTransferObjects.BudgetCategoryType categoryType, System.DateTime monthStartDate, DataTransferObjects.PracticeManagementCurrency amount) {
            base.Channel.CategoryItemBudgetSave(itemId, categoryType, monthStartDate, amount);
        }
        
        public DataTransferObjects.ProjectsGroupedByPerson[] CalculateBudgetForPersons(System.DateTime startDate, System.DateTime endDate, bool showProjected, bool showCompleted, bool showActive, bool showInternal, bool showExperimental, bool showInactive, string practiceIdsList, bool excludeInternalPractices, string personIds, DataTransferObjects.BudgetCategoryType categoryType) {
            return base.Channel.CalculateBudgetForPersons(startDate, endDate, showProjected, showCompleted, showActive, showInternal, showExperimental, showInactive, practiceIdsList, excludeInternalPractices, personIds, categoryType);
        }
        
        public DataTransferObjects.ProjectsGroupedByPractice[] CalculateBudgetForPractices(System.DateTime startDate, System.DateTime endDate, bool showProjected, bool showCompleted, bool showActive, bool showInternal, bool showExperimental, bool showInactive, string practiceIdsList, bool excludeInternalPractices) {
            return base.Channel.CalculateBudgetForPractices(startDate, endDate, showProjected, showCompleted, showActive, showInternal, showExperimental, showInactive, practiceIdsList, excludeInternalPractices);
        }
        
        public void CategoryItemsSaveFromXML(DataTransferObjects.CategoryItemBudget[] categoryItems, int year) {
            base.Channel.CategoryItemsSaveFromXML(categoryItems, year);
        }
        
        public void ProjectDelete(int projectId, string userName) {
            base.Channel.ProjectDelete(projectId, userName);
        }
        
        public DataTransferObjects.ProjectExpense[] GetProjectExpensesForProject(DataTransferObjects.ProjectExpense entity) {
            return base.Channel.GetProjectExpensesForProject(entity);
        }
        
        public DataTransferObjects.Project[] AllProjectsWithFinancialTotalsAndPersons() {
            return base.Channel.AllProjectsWithFinancialTotalsAndPersons();
        }
        
        public bool IsUserHasPermissionOnProject(string user, int id, bool isProjectId) {
            return base.Channel.IsUserHasPermissionOnProject(user, id, isProjectId);
        }
        
        public bool IsUserIsOwnerOfProject(string user, int id, bool isProjectId) {
            return base.Channel.IsUserIsOwnerOfProject(user, id, isProjectId);
        }
        
        public bool IsUserIsProjectOwner(string user, int id) {
            return base.Channel.IsUserIsProjectOwner(user, id);
        }
    }
}


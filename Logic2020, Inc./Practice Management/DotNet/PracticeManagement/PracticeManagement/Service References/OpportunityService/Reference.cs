﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PraticeManagement.OpportunityService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="OpportunityService.IOpportunityService")]
    public interface IOpportunityService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/UpdatePriorityIdForOpportunity", ReplyAction="http://tempuri.org/IOpportunityService/UpdatePriorityIdForOpportunityResponse")]
        void UpdatePriorityIdForOpportunity(int opportunityId, int priorityId, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/OpportunityGetExcelSet", ReplyAction="http://tempuri.org/IOpportunityService/OpportunityGetExcelSetResponse")]
        System.Data.DataSet OpportunityGetExcelSet();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/IsOpportunityPriorityInUse", ReplyAction="http://tempuri.org/IOpportunityService/IsOpportunityPriorityInUseResponse")]
        bool IsOpportunityPriorityInUse(int priorityId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/IsOpportunityHaveTeamStructure", ReplyAction="http://tempuri.org/IOpportunityService/IsOpportunityHaveTeamStructureResponse")]
        bool IsOpportunityHaveTeamStructure(int opportunityId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/GetOpportunityTransitions", ReplyAction="http://tempuri.org/IOpportunityService/GetOpportunityTransitionsResponse")]
        DataTransferObjects.OpportunityTransition[] GetOpportunityTransitions(int opportunityId, DataTransferObjects.OpportunityTransitionStatusType statusType);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/GetOpportunityTransitionsByPerson", ReplyAction="http://tempuri.org/IOpportunityService/GetOpportunityTransitionsByPersonResponse")]
        DataTransferObjects.OpportunityTransition[] GetOpportunityTransitionsByPerson(int personId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/OpportunityTransitionInsert", ReplyAction="http://tempuri.org/IOpportunityService/OpportunityTransitionInsertResponse")]
        int OpportunityTransitionInsert(DataTransferObjects.OpportunityTransition transition);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/OpportunityTransitionDelete", ReplyAction="http://tempuri.org/IOpportunityService/OpportunityTransitionDeleteResponse")]
        void OpportunityTransitionDelete(int transitionId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/OpportunityListAll", ReplyAction="http://tempuri.org/IOpportunityService/OpportunityListAllResponse")]
        DataTransferObjects.Opportunity[] OpportunityListAll(DataTransferObjects.ContextObjects.OpportunityListContext context);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/OpportunityListAllShort", ReplyAction="http://tempuri.org/IOpportunityService/OpportunityListAllShortResponse")]
        DataTransferObjects.Opportunity[] OpportunityListAllShort(DataTransferObjects.ContextObjects.OpportunityListContext context);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/GetOpportunityPrioritiesListAll", ReplyAction="http://tempuri.org/IOpportunityService/GetOpportunityPrioritiesListAllResponse")]
        DataTransferObjects.OpportunityPriority[] GetOpportunityPrioritiesListAll();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/GetOpportunityPriorities", ReplyAction="http://tempuri.org/IOpportunityService/GetOpportunityPrioritiesResponse")]
        DataTransferObjects.OpportunityPriority[] GetOpportunityPriorities(bool isinserted);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/InsertOpportunityPriority", ReplyAction="http://tempuri.org/IOpportunityService/InsertOpportunityPriorityResponse")]
        void InsertOpportunityPriority(DataTransferObjects.OpportunityPriority opportunityPriority);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/UpdateOpportunityPriority", ReplyAction="http://tempuri.org/IOpportunityService/UpdateOpportunityPriorityResponse")]
        void UpdateOpportunityPriority(int oldPriorityId, DataTransferObjects.OpportunityPriority opportunityPriority, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/DeleteOpportunityPriority", ReplyAction="http://tempuri.org/IOpportunityService/DeleteOpportunityPriorityResponse")]
        void DeleteOpportunityPriority(System.Nullable<int> updatedPriorityId, int deletedPriorityId, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/GetById", ReplyAction="http://tempuri.org/IOpportunityService/GetByIdResponse")]
        DataTransferObjects.Opportunity GetById(int opportunityId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/OpportunitySave", ReplyAction="http://tempuri.org/IOpportunityService/OpportunitySaveResponse")]
        System.Nullable<int> OpportunitySave(DataTransferObjects.Opportunity opportunity, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/OpportunityStatusListAll", ReplyAction="http://tempuri.org/IOpportunityService/OpportunityStatusListAllResponse")]
        DataTransferObjects.OpportunityStatus[] OpportunityStatusListAll();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/OpportunityTransitionStatusListAll", ReplyAction="http://tempuri.org/IOpportunityService/OpportunityTransitionStatusListAllResponse" +
            "")]
        DataTransferObjects.OpportunityTransitionStatus[] OpportunityTransitionStatusListAll();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/OpportunityConvertToProject", ReplyAction="http://tempuri.org/IOpportunityService/OpportunityConvertToProjectResponse")]
        int OpportunityConvertToProject(int opportunityId, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/GetOpportunityId", ReplyAction="http://tempuri.org/IOpportunityService/GetOpportunityIdResponse")]
        System.Nullable<int> GetOpportunityId(string opportunityNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/GetOpportunityPersons", ReplyAction="http://tempuri.org/IOpportunityService/GetOpportunityPersonsResponse")]
        DataTransferObjects.OpportunityPerson[] GetOpportunityPersons(int opportunityId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/ConvertOpportunityToProject", ReplyAction="http://tempuri.org/IOpportunityService/ConvertOpportunityToProjectResponse")]
        int ConvertOpportunityToProject(int opportunityId, string userName, bool hasPersons);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/OpportunityPersonInsert", ReplyAction="http://tempuri.org/IOpportunityService/OpportunityPersonInsertResponse")]
        void OpportunityPersonInsert(int opportunityId, string personIdList, int relationTypeId, string outSideResources);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/OpportunityPersonDelete", ReplyAction="http://tempuri.org/IOpportunityService/OpportunityPersonDeleteResponse")]
        void OpportunityPersonDelete(int opportunityId, string personIdList);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/OpportunityDelete", ReplyAction="http://tempuri.org/IOpportunityService/OpportunityDeleteResponse")]
        void OpportunityDelete(int opportunityId, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/GetOpportunityPriorityTransitionCount", ReplyAction="http://tempuri.org/IOpportunityService/GetOpportunityPriorityTransitionCountRespo" +
            "nse")]
        System.Collections.Generic.Dictionary<string, int> GetOpportunityPriorityTransitionCount(int daysPrevious);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/GetOpportunityStatusChangeCount", ReplyAction="http://tempuri.org/IOpportunityService/GetOpportunityStatusChangeCountResponse")]
        System.Collections.Generic.Dictionary<string, int> GetOpportunityStatusChangeCount(int daysPrevious);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/FilteredOpportunityListAll", ReplyAction="http://tempuri.org/IOpportunityService/FilteredOpportunityListAllResponse")]
        DataTransferObjects.Opportunity[] FilteredOpportunityListAll(bool showActive, bool showExperimental, bool showInactive, bool showLost, bool showWon, string clientIdsList, string opportunityGroupIdsList, string opportunityOwnerIdsList, string salespersonIdsList);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpportunityService/OpportunitySearchText", ReplyAction="http://tempuri.org/IOpportunityService/OpportunitySearchTextResponse")]
        DataTransferObjects.Opportunity[] OpportunitySearchText(string looked, int personId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IOpportunityServiceChannel : PraticeManagement.OpportunityService.IOpportunityService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class OpportunityServiceClient : System.ServiceModel.ClientBase<PraticeManagement.OpportunityService.IOpportunityService>, PraticeManagement.OpportunityService.IOpportunityService {
       
        public OpportunityServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public OpportunityServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OpportunityServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OpportunityServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void UpdatePriorityIdForOpportunity(int opportunityId, int priorityId, string userName) {
            base.Channel.UpdatePriorityIdForOpportunity(opportunityId, priorityId, userName);
        }
        
        public System.Data.DataSet OpportunityGetExcelSet() {
            return base.Channel.OpportunityGetExcelSet();
        }
        
        public bool IsOpportunityPriorityInUse(int priorityId) {
            return base.Channel.IsOpportunityPriorityInUse(priorityId);
        }
        
        public bool IsOpportunityHaveTeamStructure(int opportunityId) {
            return base.Channel.IsOpportunityHaveTeamStructure(opportunityId);
        }
        
        public DataTransferObjects.OpportunityTransition[] GetOpportunityTransitions(int opportunityId, DataTransferObjects.OpportunityTransitionStatusType statusType) {
            return base.Channel.GetOpportunityTransitions(opportunityId, statusType);
        }
        
        public DataTransferObjects.OpportunityTransition[] GetOpportunityTransitionsByPerson(int personId) {
            return base.Channel.GetOpportunityTransitionsByPerson(personId);
        }
        
        public int OpportunityTransitionInsert(DataTransferObjects.OpportunityTransition transition) {
            return base.Channel.OpportunityTransitionInsert(transition);
        }
        
        public void OpportunityTransitionDelete(int transitionId) {
            base.Channel.OpportunityTransitionDelete(transitionId);
        }
        
        public DataTransferObjects.Opportunity[] OpportunityListAll(DataTransferObjects.ContextObjects.OpportunityListContext context) {
            return base.Channel.OpportunityListAll(context);
        }
        
        public DataTransferObjects.Opportunity[] OpportunityListAllShort(DataTransferObjects.ContextObjects.OpportunityListContext context) {
            return base.Channel.OpportunityListAllShort(context);
        }
        
        public DataTransferObjects.OpportunityPriority[] GetOpportunityPrioritiesListAll() {
            return base.Channel.GetOpportunityPrioritiesListAll();
        }
        
        public DataTransferObjects.OpportunityPriority[] GetOpportunityPriorities(bool isinserted) {
            return base.Channel.GetOpportunityPriorities(isinserted);
        }
        
        public void InsertOpportunityPriority(DataTransferObjects.OpportunityPriority opportunityPriority) {
            base.Channel.InsertOpportunityPriority(opportunityPriority);
        }
        
        public void UpdateOpportunityPriority(int oldPriorityId, DataTransferObjects.OpportunityPriority opportunityPriority, string userName) {
            base.Channel.UpdateOpportunityPriority(oldPriorityId, opportunityPriority, userName);
        }
        
        public void DeleteOpportunityPriority(System.Nullable<int> updatedPriorityId, int deletedPriorityId, string userName) {
            base.Channel.DeleteOpportunityPriority(updatedPriorityId, deletedPriorityId, userName);
        }
        
        public DataTransferObjects.Opportunity GetById(int opportunityId) {
            return base.Channel.GetById(opportunityId);
        }
        
        public System.Nullable<int> OpportunitySave(DataTransferObjects.Opportunity opportunity, string userName) {
            return base.Channel.OpportunitySave(opportunity, userName);
        }
        
        public DataTransferObjects.OpportunityStatus[] OpportunityStatusListAll() {
            return base.Channel.OpportunityStatusListAll();
        }
        
        public DataTransferObjects.OpportunityTransitionStatus[] OpportunityTransitionStatusListAll() {
            return base.Channel.OpportunityTransitionStatusListAll();
        }
        
        public int OpportunityConvertToProject(int opportunityId, string userName) {
            return base.Channel.OpportunityConvertToProject(opportunityId, userName);
        }
        
        public System.Nullable<int> GetOpportunityId(string opportunityNumber) {
            return base.Channel.GetOpportunityId(opportunityNumber);
        }
        
        public DataTransferObjects.OpportunityPerson[] GetOpportunityPersons(int opportunityId) {
            return base.Channel.GetOpportunityPersons(opportunityId);
        }
        
        public int ConvertOpportunityToProject(int opportunityId, string userName, bool hasPersons) {
            return base.Channel.ConvertOpportunityToProject(opportunityId, userName, hasPersons);
        }
        
        public void OpportunityPersonInsert(int opportunityId, string personIdList, int relationTypeId, string outSideResources) {
            base.Channel.OpportunityPersonInsert(opportunityId, personIdList, relationTypeId, outSideResources);
        }
        
        public void OpportunityPersonDelete(int opportunityId, string personIdList) {
            base.Channel.OpportunityPersonDelete(opportunityId, personIdList);
        }
        
        public void OpportunityDelete(int opportunityId, string userName) {
            base.Channel.OpportunityDelete(opportunityId, userName);
        }
        
        public System.Collections.Generic.Dictionary<string, int> GetOpportunityPriorityTransitionCount(int daysPrevious) {
            return base.Channel.GetOpportunityPriorityTransitionCount(daysPrevious);
        }
        
        public System.Collections.Generic.Dictionary<string, int> GetOpportunityStatusChangeCount(int daysPrevious) {
            return base.Channel.GetOpportunityStatusChangeCount(daysPrevious);
        }
        
        public DataTransferObjects.Opportunity[] FilteredOpportunityListAll(bool showActive, bool showExperimental, bool showInactive, bool showLost, bool showWon, string clientIdsList, string opportunityGroupIdsList, string opportunityOwnerIdsList, string salespersonIdsList) {
            return base.Channel.FilteredOpportunityListAll(showActive, showExperimental, showInactive, showLost, showWon, clientIdsList, opportunityGroupIdsList, opportunityOwnerIdsList, salespersonIdsList);
        }
        
        public DataTransferObjects.Opportunity[] OpportunitySearchText(string looked, int personId) {
            return base.Channel.OpportunitySearchText(looked, personId);
        }
    }
}


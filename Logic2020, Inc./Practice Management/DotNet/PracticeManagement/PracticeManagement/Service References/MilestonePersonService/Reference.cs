﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PraticeManagement.MilestonePersonService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="MilestonePersonService.IMilestonePersonService")]
    public interface IMilestonePersonService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/GetMilestonePersonListByProject", ReplyAction="http://tempuri.org/IMilestonePersonService/GetMilestonePersonListByProjectRespons" +
            "e")]
        DataTransferObjects.MilestonePerson[] GetMilestonePersonListByProject(int projectId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/GetMilestonePersonListByProjectWithout" +
            "Pay", ReplyAction="http://tempuri.org/IMilestonePersonService/GetMilestonePersonListByProjectWithout" +
            "PayResponse")]
        DataTransferObjects.MilestonePerson[] GetMilestonePersonListByProjectWithoutPay(int projectId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/GetConsultantMilestones", ReplyAction="http://tempuri.org/IMilestonePersonService/GetConsultantMilestonesResponse")]
        DataTransferObjects.MilestonePersonEntry[] GetConsultantMilestones(DataTransferObjects.ContextObjects.ConsultantMilestonesContext context);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/GetMilestonePersonListByMilestone", ReplyAction="http://tempuri.org/IMilestonePersonService/GetMilestonePersonListByMilestoneRespo" +
            "nse")]
        DataTransferObjects.MilestonePerson[] GetMilestonePersonListByMilestone(int milestoneId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/GetMilestonePersonListByPerson", ReplyAction="http://tempuri.org/IMilestonePersonService/GetMilestonePersonListByPersonResponse" +
            "")]
        DataTransferObjects.MilestonePerson[] GetMilestonePersonListByPerson(int personId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/GetMilestonePersonDetail", ReplyAction="http://tempuri.org/IMilestonePersonService/GetMilestonePersonDetailResponse")]
        DataTransferObjects.MilestonePerson GetMilestonePersonDetail(int milestonePersonId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/CheckTimeEntriesForMilestonePerson", ReplyAction="http://tempuri.org/IMilestonePersonService/CheckTimeEntriesForMilestonePersonResp" +
            "onse")]
        bool CheckTimeEntriesForMilestonePerson(int milestonePersonId, System.Nullable<System.DateTime> startDate, System.Nullable<System.DateTime> endDate, bool checkStartDateEquality, bool checkEndDateEquality);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/CheckTimeEntriesForMilestonePersonWith" +
            "GivenRoleId", ReplyAction="http://tempuri.org/IMilestonePersonService/CheckTimeEntriesForMilestonePersonWith" +
            "GivenRoleIdResponse")]
        bool CheckTimeEntriesForMilestonePersonWithGivenRoleId(int milestonePersonId, System.Nullable<int> milestonePersonRoleId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/SaveMilestonePerson", ReplyAction="http://tempuri.org/IMilestonePersonService/SaveMilestonePersonResponse")]
        void SaveMilestonePerson(ref DataTransferObjects.MilestonePerson milestonePerson, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/SaveMilestonePersons", ReplyAction="http://tempuri.org/IMilestonePersonService/SaveMilestonePersonsResponse")]
        void SaveMilestonePersons(DataTransferObjects.MilestonePerson[] milestonePersons, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/DeleteMilestonePerson", ReplyAction="http://tempuri.org/IMilestonePersonService/DeleteMilestonePersonResponse")]
        void DeleteMilestonePerson(DataTransferObjects.MilestonePerson milestonePerson);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/GetMilestonePersonListByMilestoneNoFin" +
            "ancials", ReplyAction="http://tempuri.org/IMilestonePersonService/GetMilestonePersonListByMilestoneNoFin" +
            "ancialsResponse")]
        DataTransferObjects.MilestonePerson[] GetMilestonePersonListByMilestoneNoFinancials(int milestoneId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/MilestonePersonsByMilestoneForTEByProj" +
            "ect", ReplyAction="http://tempuri.org/IMilestonePersonService/MilestonePersonsByMilestoneForTEByProj" +
            "ectResponse")]
        DataTransferObjects.MilestonePerson[] MilestonePersonsByMilestoneForTEByProject(int milestoneId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/CalculateMilestonePersonFinancials", ReplyAction="http://tempuri.org/IMilestonePersonService/CalculateMilestonePersonFinancialsResp" +
            "onse")]
        DataTransferObjects.Financials.MilestonePersonComputedFinancials CalculateMilestonePersonFinancials(int milestonePersonId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/GetMilestonePersonsDetailsByMileStoneI" +
            "d", ReplyAction="http://tempuri.org/IMilestonePersonService/GetMilestonePersonsDetailsByMileStoneI" +
            "dResponse")]
        DataTransferObjects.MilestonePerson[] GetMilestonePersonsDetailsByMileStoneId(int milestoneId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/DeleteMilestonePersonEntry", ReplyAction="http://tempuri.org/IMilestonePersonService/DeleteMilestonePersonEntryResponse")]
        void DeleteMilestonePersonEntry(int milestonePersonEntryId, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/UpdateMilestonePersonEntry", ReplyAction="http://tempuri.org/IMilestonePersonService/UpdateMilestonePersonEntryResponse")]
        int UpdateMilestonePersonEntry(DataTransferObjects.MilestonePersonEntry entry, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/MilestonePersonEntryInsert", ReplyAction="http://tempuri.org/IMilestonePersonService/MilestonePersonEntryInsertResponse")]
        int MilestonePersonEntryInsert(DataTransferObjects.MilestonePersonEntry entry, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/MilestonePersonAndEntryInsert", ReplyAction="http://tempuri.org/IMilestonePersonService/MilestonePersonAndEntryInsertResponse")]
        int MilestonePersonAndEntryInsert(DataTransferObjects.MilestonePerson milestonePerson, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/GetMilestonePersonEntry", ReplyAction="http://tempuri.org/IMilestonePersonService/GetMilestonePersonEntryResponse")]
        DataTransferObjects.MilestonePersonEntry GetMilestonePersonEntry(int mpeid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/IsPersonAlreadyAddedtoMilestone", ReplyAction="http://tempuri.org/IMilestonePersonService/IsPersonAlreadyAddedtoMilestoneRespons" +
            "e")]
        bool IsPersonAlreadyAddedtoMilestone(int mileStoneId, int personId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMilestonePersonService/MilestoneResourceUpdate", ReplyAction="http://tempuri.org/IMilestonePersonService/MilestoneResourceUpdateResponse")]
        void MilestoneResourceUpdate(DataTransferObjects.Milestone milestone, DataTransferObjects.MilestoneUpdateObject milestoneUpdateObj, string userName);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IMilestonePersonServiceChannel : PraticeManagement.MilestonePersonService.IMilestonePersonService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MilestonePersonServiceClient : System.ServiceModel.ClientBase<PraticeManagement.MilestonePersonService.IMilestonePersonService>, PraticeManagement.MilestonePersonService.IMilestonePersonService {
        
       
        
        public MilestonePersonServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public MilestonePersonServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MilestonePersonServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MilestonePersonServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public DataTransferObjects.MilestonePerson[] GetMilestonePersonListByProject(int projectId) {
            return base.Channel.GetMilestonePersonListByProject(projectId);
        }
        
        public DataTransferObjects.MilestonePerson[] GetMilestonePersonListByProjectWithoutPay(int projectId) {
            return base.Channel.GetMilestonePersonListByProjectWithoutPay(projectId);
        }
        
        public DataTransferObjects.MilestonePersonEntry[] GetConsultantMilestones(DataTransferObjects.ContextObjects.ConsultantMilestonesContext context) {
            return base.Channel.GetConsultantMilestones(context);
        }
        
        public DataTransferObjects.MilestonePerson[] GetMilestonePersonListByMilestone(int milestoneId) {
            return base.Channel.GetMilestonePersonListByMilestone(milestoneId);
        }
        
        public DataTransferObjects.MilestonePerson[] GetMilestonePersonListByPerson(int personId) {
            return base.Channel.GetMilestonePersonListByPerson(personId);
        }
        
        public DataTransferObjects.MilestonePerson GetMilestonePersonDetail(int milestonePersonId) {
            return base.Channel.GetMilestonePersonDetail(milestonePersonId);
        }
        
        public bool CheckTimeEntriesForMilestonePerson(int milestonePersonId, System.Nullable<System.DateTime> startDate, System.Nullable<System.DateTime> endDate, bool checkStartDateEquality, bool checkEndDateEquality) {
            return base.Channel.CheckTimeEntriesForMilestonePerson(milestonePersonId, startDate, endDate, checkStartDateEquality, checkEndDateEquality);
        }
        
        public bool CheckTimeEntriesForMilestonePersonWithGivenRoleId(int milestonePersonId, System.Nullable<int> milestonePersonRoleId) {
            return base.Channel.CheckTimeEntriesForMilestonePersonWithGivenRoleId(milestonePersonId, milestonePersonRoleId);
        }
        
        public void SaveMilestonePerson(ref DataTransferObjects.MilestonePerson milestonePerson, string userName) {
            base.Channel.SaveMilestonePerson(ref milestonePerson, userName);
        }
        
        public void SaveMilestonePersons(DataTransferObjects.MilestonePerson[] milestonePersons, string userName) {
            base.Channel.SaveMilestonePersons(milestonePersons, userName);
        }
        
        public void DeleteMilestonePerson(DataTransferObjects.MilestonePerson milestonePerson) {
            base.Channel.DeleteMilestonePerson(milestonePerson);
        }
        
        public DataTransferObjects.MilestonePerson[] GetMilestonePersonListByMilestoneNoFinancials(int milestoneId) {
            return base.Channel.GetMilestonePersonListByMilestoneNoFinancials(milestoneId);
        }
        
        public DataTransferObjects.MilestonePerson[] MilestonePersonsByMilestoneForTEByProject(int milestoneId) {
            return base.Channel.MilestonePersonsByMilestoneForTEByProject(milestoneId);
        }
        
        public DataTransferObjects.Financials.MilestonePersonComputedFinancials CalculateMilestonePersonFinancials(int milestonePersonId) {
            return base.Channel.CalculateMilestonePersonFinancials(milestonePersonId);
        }
        
        public DataTransferObjects.MilestonePerson[] GetMilestonePersonsDetailsByMileStoneId(int milestoneId) {
            return base.Channel.GetMilestonePersonsDetailsByMileStoneId(milestoneId);
        }
        
        public void DeleteMilestonePersonEntry(int milestonePersonEntryId, string userName) {
            base.Channel.DeleteMilestonePersonEntry(milestonePersonEntryId, userName);
        }
        
        public int UpdateMilestonePersonEntry(DataTransferObjects.MilestonePersonEntry entry, string userName) {
            return base.Channel.UpdateMilestonePersonEntry(entry, userName);
        }
        
        public int MilestonePersonEntryInsert(DataTransferObjects.MilestonePersonEntry entry, string userName) {
            return base.Channel.MilestonePersonEntryInsert(entry, userName);
        }
        
        public int MilestonePersonAndEntryInsert(DataTransferObjects.MilestonePerson milestonePerson, string userName) {
            return base.Channel.MilestonePersonAndEntryInsert(milestonePerson, userName);
        }
        
        public DataTransferObjects.MilestonePersonEntry GetMilestonePersonEntry(int mpeid) {
            return base.Channel.GetMilestonePersonEntry(mpeid);
        }
        
        public bool IsPersonAlreadyAddedtoMilestone(int mileStoneId, int personId) {
            return base.Channel.IsPersonAlreadyAddedtoMilestone(mileStoneId, personId);
        }
        
        public void MilestoneResourceUpdate(DataTransferObjects.Milestone milestone, DataTransferObjects.MilestoneUpdateObject milestoneUpdateObj, string userName) {
            base.Channel.MilestoneResourceUpdate(milestone, milestoneUpdateObj, userName);
        }
    }
}


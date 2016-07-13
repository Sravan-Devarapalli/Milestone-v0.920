﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PraticeManagement.PracticeService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PracticeService.IPracticeService")]
    public interface IPracticeService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/GetPracticeList", ReplyAction="http://tempuri.org/IPracticeService/GetPracticeListResponse")]
        DataTransferObjects.Practice[] GetPracticeList();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/GetPracticeListForDivision", ReplyAction="http://tempuri.org/IPracticeService/GetPracticeListForDivisionResponse")]
        DataTransferObjects.Practice[] GetPracticeListForDivision(int divisionId, bool isProject);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/PracticeListAll", ReplyAction="http://tempuri.org/IPracticeService/PracticeListAllResponse")]
        DataTransferObjects.Practice[] PracticeListAll(DataTransferObjects.Person person);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/PracticeListAllWithCapabilities", ReplyAction="http://tempuri.org/IPracticeService/PracticeListAllWithCapabilitiesResponse")]
        DataTransferObjects.Practice[] PracticeListAllWithCapabilities();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/PracticeGetById", ReplyAction="http://tempuri.org/IPracticeService/PracticeGetByIdResponse")]
        DataTransferObjects.Practice[] PracticeGetById(System.Nullable<int> id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/UpdatePractice", ReplyAction="http://tempuri.org/IPracticeService/UpdatePracticeResponse")]
        void UpdatePractice(DataTransferObjects.Practice practice, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/InsertPractice", ReplyAction="http://tempuri.org/IPracticeService/InsertPracticeResponse")]
        int InsertPractice(DataTransferObjects.Practice practice, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/RemovePractice", ReplyAction="http://tempuri.org/IPracticeService/RemovePracticeResponse")]
        void RemovePractice(DataTransferObjects.Practice practice, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/GetPracticeCapabilities", ReplyAction="http://tempuri.org/IPracticeService/GetPracticeCapabilitiesResponse")]
        DataTransferObjects.PracticeCapability[] GetPracticeCapabilities(System.Nullable<int> practiceId, System.Nullable<int> capabilityId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/CapabilityDelete", ReplyAction="http://tempuri.org/IPracticeService/CapabilityDeleteResponse")]
        void CapabilityDelete(int capabilityId, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/CapabilityUpdate", ReplyAction="http://tempuri.org/IPracticeService/CapabilityUpdateResponse")]
        void CapabilityUpdate(DataTransferObjects.PracticeCapability capability, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/CapabilityInsert", ReplyAction="http://tempuri.org/IPracticeService/CapabilityInsertResponse")]
        void CapabilityInsert(DataTransferObjects.PracticeCapability capability, string userLogin);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPracticeServiceChannel : PraticeManagement.PracticeService.IPracticeService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PracticeServiceClient : System.ServiceModel.ClientBase<PraticeManagement.PracticeService.IPracticeService>, PraticeManagement.PracticeService.IPracticeService {
        
        public PracticeServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PracticeServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PracticeServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PracticeServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public DataTransferObjects.Practice[] GetPracticeList() {
            return base.Channel.GetPracticeList();
        }
        
        public DataTransferObjects.Practice[] GetPracticeListForDivision(int divisionId, bool isProject) {
            return base.Channel.GetPracticeListForDivision(divisionId, isProject);
        }
        
        public DataTransferObjects.Practice[] PracticeListAll(DataTransferObjects.Person person) {
            return base.Channel.PracticeListAll(person);
        }
        
        public DataTransferObjects.Practice[] PracticeListAllWithCapabilities() {
            return base.Channel.PracticeListAllWithCapabilities();
        }
        
        public DataTransferObjects.Practice[] PracticeGetById(System.Nullable<int> id) {
            return base.Channel.PracticeGetById(id);
        }
        
        public void UpdatePractice(DataTransferObjects.Practice practice, string userLogin) {
            base.Channel.UpdatePractice(practice, userLogin);
        }
        
        public int InsertPractice(DataTransferObjects.Practice practice, string userLogin) {
            return base.Channel.InsertPractice(practice, userLogin);
        }
        
        public void RemovePractice(DataTransferObjects.Practice practice, string userLogin) {
            base.Channel.RemovePractice(practice, userLogin);
        }
        
        public DataTransferObjects.PracticeCapability[] GetPracticeCapabilities(System.Nullable<int> practiceId, System.Nullable<int> capabilityId) {
            return base.Channel.GetPracticeCapabilities(practiceId, capabilityId);
        }
        
        public void CapabilityDelete(int capabilityId, string userLogin) {
            base.Channel.CapabilityDelete(capabilityId, userLogin);
        }
        
        public void CapabilityUpdate(DataTransferObjects.PracticeCapability capability, string userLogin) {
            base.Channel.CapabilityUpdate(capability, userLogin);
        }
        
        public void CapabilityInsert(DataTransferObjects.PracticeCapability capability, string userLogin) {
            base.Channel.CapabilityInsert(capability, userLogin);
        }
    }
}


﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
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
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/PracticeListAll", ReplyAction="http://tempuri.org/IPracticeService/PracticeListAllResponse")]
        DataTransferObjects.Practice[] PracticeListAll(DataTransferObjects.Person person);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/PracticeGetById", ReplyAction="http://tempuri.org/IPracticeService/PracticeGetByIdResponse")]
        DataTransferObjects.Practice[] PracticeGetById(System.Nullable<int> id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/UpdatePractice", ReplyAction="http://tempuri.org/IPracticeService/UpdatePracticeResponse")]
        void UpdatePractice(DataTransferObjects.Practice practice);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/InsertPractice", ReplyAction="http://tempuri.org/IPracticeService/InsertPracticeResponse")]
        int InsertPractice(DataTransferObjects.Practice practice);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPracticeService/RemovePractice", ReplyAction="http://tempuri.org/IPracticeService/RemovePracticeResponse")]
        void RemovePractice(DataTransferObjects.Practice practice);
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
        
        public DataTransferObjects.Practice[] PracticeListAll(DataTransferObjects.Person person) {
            return base.Channel.PracticeListAll(person);
        }
        
        public DataTransferObjects.Practice[] PracticeGetById(System.Nullable<int> id) {
            return base.Channel.PracticeGetById(id);
        }
        
        public void UpdatePractice(DataTransferObjects.Practice practice) {
            base.Channel.UpdatePractice(practice);
        }
        
        public int InsertPractice(DataTransferObjects.Practice practice) {
            return base.Channel.InsertPractice(practice);
        }
        
        public void RemovePractice(DataTransferObjects.Practice practice) {
            base.Channel.RemovePractice(practice);
        }
    }
}


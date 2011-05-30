﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PraticeManagement.OverheadService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="OverheadService.IOverheadService")]
    public interface IOverheadService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOverheadService/GetRateTypes", ReplyAction="http://tempuri.org/IOverheadService/GetRateTypesResponse")]
        DataTransferObjects.OverheadRateType[] GetRateTypes();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOverheadService/GetRateTypeDetail", ReplyAction="http://tempuri.org/IOverheadService/GetRateTypeDetailResponse")]
        DataTransferObjects.OverheadRateType GetRateTypeDetail(int overheadRateTypeId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOverheadService/GetOverheadFixedRates", ReplyAction="http://tempuri.org/IOverheadService/GetOverheadFixedRatesResponse")]
        DataTransferObjects.OverheadFixedRate[] GetOverheadFixedRates(bool activeOnly);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOverheadService/GetOverheadFixedRateDetail", ReplyAction="http://tempuri.org/IOverheadService/GetOverheadFixedRateDetailResponse")]
        DataTransferObjects.OverheadFixedRate GetOverheadFixedRateDetail(int overheadId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOverheadService/SaveOverheadFixedRateDetail", ReplyAction="http://tempuri.org/IOverheadService/SaveOverheadFixedRateDetailResponse")]
        System.Nullable<int> SaveOverheadFixedRateDetail(DataTransferObjects.OverheadFixedRate overhead);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOverheadService/OverheadFixedRateInactivate", ReplyAction="http://tempuri.org/IOverheadService/OverheadFixedRateInactivateResponse")]
        void OverheadFixedRateInactivate(int overheadId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOverheadService/OverheadFixedRateReactivate", ReplyAction="http://tempuri.org/IOverheadService/OverheadFixedRateReactivateResponse")]
        void OverheadFixedRateReactivate(int overheadId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOverheadService/GetMinimumLoadFactorOverheadMultipliers", ReplyAction="http://tempuri.org/IOverheadService/GetMinimumLoadFactorOverheadMultipliersRespon" +
            "se")]
        System.Collections.Generic.Dictionary<int, decimal> GetMinimumLoadFactorOverheadMultipliers(string OverHeadName, ref bool isInactive);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOverheadService/UpdateMinimumLoadFactorHistory", ReplyAction="http://tempuri.org/IOverheadService/UpdateMinimumLoadFactorHistoryResponse")]
        void UpdateMinimumLoadFactorHistory(int timeScaleId, decimal rate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOverheadService/UpdateMinimumLoadFactorStatus", ReplyAction="http://tempuri.org/IOverheadService/UpdateMinimumLoadFactorStatusResponse")]
        void UpdateMinimumLoadFactorStatus(bool inActive);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOverheadService/GetOverheadHistory", ReplyAction="http://tempuri.org/IOverheadService/GetOverheadHistoryResponse")]
        DataTransferObjects.OverHeadHistory[] GetOverheadHistory();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IOverheadServiceChannel : PraticeManagement.OverheadService.IOverheadService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class OverheadServiceClient : System.ServiceModel.ClientBase<PraticeManagement.OverheadService.IOverheadService>, PraticeManagement.OverheadService.IOverheadService {
        
        public OverheadServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public OverheadServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OverheadServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OverheadServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public DataTransferObjects.OverheadRateType[] GetRateTypes() {
            return base.Channel.GetRateTypes();
        }
        
        public DataTransferObjects.OverheadRateType GetRateTypeDetail(int overheadRateTypeId) {
            return base.Channel.GetRateTypeDetail(overheadRateTypeId);
        }
        
        public DataTransferObjects.OverheadFixedRate[] GetOverheadFixedRates(bool activeOnly) {
            return base.Channel.GetOverheadFixedRates(activeOnly);
        }
        
        public DataTransferObjects.OverheadFixedRate GetOverheadFixedRateDetail(int overheadId) {
            return base.Channel.GetOverheadFixedRateDetail(overheadId);
        }
        
        public System.Nullable<int> SaveOverheadFixedRateDetail(DataTransferObjects.OverheadFixedRate overhead) {
            return base.Channel.SaveOverheadFixedRateDetail(overhead);
        }
        
        public void OverheadFixedRateInactivate(int overheadId) {
            base.Channel.OverheadFixedRateInactivate(overheadId);
        }
        
        public void OverheadFixedRateReactivate(int overheadId) {
            base.Channel.OverheadFixedRateReactivate(overheadId);
        }
        
        public System.Collections.Generic.Dictionary<int, decimal> GetMinimumLoadFactorOverheadMultipliers(string OverHeadName, ref bool isInactive) {
            return base.Channel.GetMinimumLoadFactorOverheadMultipliers(OverHeadName, ref isInactive);
        }
        
        public void UpdateMinimumLoadFactorHistory(int timeScaleId, decimal rate) {
            base.Channel.UpdateMinimumLoadFactorHistory(timeScaleId, rate);
        }
        
        public void UpdateMinimumLoadFactorStatus(bool inActive) {
            base.Channel.UpdateMinimumLoadFactorStatus(inActive);
        }
        
        public DataTransferObjects.OverHeadHistory[] GetOverheadHistory() {
            return base.Channel.GetOverheadHistory();
        }
    }
}


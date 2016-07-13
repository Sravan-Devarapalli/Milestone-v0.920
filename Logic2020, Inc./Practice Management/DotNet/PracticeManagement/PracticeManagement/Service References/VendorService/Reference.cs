﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PraticeManagement.VendorService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="VendorService.IVendorService")]
    public interface IVendorService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IVendorService/GetListOfVendorTypes", ReplyAction="http://tempuri.org/IVendorService/GetListOfVendorTypesResponse")]
        DataTransferObjects.VendorType[] GetListOfVendorTypes();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IVendorService/GetAllActiveVendors", ReplyAction="http://tempuri.org/IVendorService/GetAllActiveVendorsResponse")]
        DataTransferObjects.Vendor[] GetAllActiveVendors();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IVendorService/GetListOfVendorsWithFilters", ReplyAction="http://tempuri.org/IVendorService/GetListOfVendorsWithFiltersResponse")]
        DataTransferObjects.Vendor[] GetListOfVendorsWithFilters(bool active, bool inactive, string vendorTypes, string looked);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IVendorService/GetVendorById", ReplyAction="http://tempuri.org/IVendorService/GetVendorByIdResponse")]
        DataTransferObjects.Vendor GetVendorById(int vendorId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IVendorService/SaveVendorDetail", ReplyAction="http://tempuri.org/IVendorService/SaveVendorDetailResponse")]
        int SaveVendorDetail(DataTransferObjects.Vendor vendor, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IVendorService/VendorValidations", ReplyAction="http://tempuri.org/IVendorService/VendorValidationsResponse")]
        void VendorValidations(DataTransferObjects.Vendor vendor);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IVendorService/GetVendorAttachments", ReplyAction="http://tempuri.org/IVendorService/GetVendorAttachmentsResponse")]
        DataTransferObjects.VendorAttachment[] GetVendorAttachments(int vendorId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IVendorService/SaveVendorAttachmentData", ReplyAction="http://tempuri.org/IVendorService/SaveVendorAttachmentDataResponse")]
        void SaveVendorAttachmentData(DataTransferObjects.VendorAttachment attachment, int vendorId, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IVendorService/DeleteVendorAttachmentById", ReplyAction="http://tempuri.org/IVendorService/DeleteVendorAttachmentByIdResponse")]
        void DeleteVendorAttachmentById(System.Nullable<int> attachmentId, int vendorId, string userName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IVendorService/GetVendorAttachmentData", ReplyAction="http://tempuri.org/IVendorService/GetVendorAttachmentDataResponse")]
        byte[] GetVendorAttachmentData(int vendorId, int attachmentId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IVendorService/ProjectListByVendor", ReplyAction="http://tempuri.org/IVendorService/ProjectListByVendorResponse")]
        DataTransferObjects.Project[] ProjectListByVendor(int vendorId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IVendorService/PersonListByVendor", ReplyAction="http://tempuri.org/IVendorService/PersonListByVendorResponse")]
        DataTransferObjects.Person[] PersonListByVendor(int vendorId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IVendorServiceChannel : PraticeManagement.VendorService.IVendorService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class VendorServiceClient : System.ServiceModel.ClientBase<PraticeManagement.VendorService.IVendorService>, PraticeManagement.VendorService.IVendorService {
        
        public VendorServiceClient() {
        }
        
        public VendorServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public VendorServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public VendorServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public VendorServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public DataTransferObjects.VendorType[] GetListOfVendorTypes() {
            return base.Channel.GetListOfVendorTypes();
        }
        
        public DataTransferObjects.Vendor[] GetAllActiveVendors() {
            return base.Channel.GetAllActiveVendors();
        }
        
        public DataTransferObjects.Vendor[] GetListOfVendorsWithFilters(bool active, bool inactive, string vendorTypes, string looked) {
            return base.Channel.GetListOfVendorsWithFilters(active, inactive, vendorTypes, looked);
        }
        
        public DataTransferObjects.Vendor GetVendorById(int vendorId) {
            return base.Channel.GetVendorById(vendorId);
        }
        
        public int SaveVendorDetail(DataTransferObjects.Vendor vendor, string userName) {
            return base.Channel.SaveVendorDetail(vendor, userName);
        }
        
        public void VendorValidations(DataTransferObjects.Vendor vendor) {
            base.Channel.VendorValidations(vendor);
        }
        
        public DataTransferObjects.VendorAttachment[] GetVendorAttachments(int vendorId) {
            return base.Channel.GetVendorAttachments(vendorId);
        }
        
        public void SaveVendorAttachmentData(DataTransferObjects.VendorAttachment attachment, int vendorId, string userName) {
            base.Channel.SaveVendorAttachmentData(attachment, vendorId, userName);
        }
        
        public void DeleteVendorAttachmentById(System.Nullable<int> attachmentId, int vendorId, string userName) {
            base.Channel.DeleteVendorAttachmentById(attachmentId, vendorId, userName);
        }
        
        public byte[] GetVendorAttachmentData(int vendorId, int attachmentId) {
            return base.Channel.GetVendorAttachmentData(vendorId, attachmentId);
        }
        
        public DataTransferObjects.Project[] ProjectListByVendor(int vendorId) {
            return base.Channel.ProjectListByVendor(vendorId);
        }
        
        public DataTransferObjects.Person[] PersonListByVendor(int vendorId) {
            return base.Channel.PersonListByVendor(vendorId);
        }
    }
}


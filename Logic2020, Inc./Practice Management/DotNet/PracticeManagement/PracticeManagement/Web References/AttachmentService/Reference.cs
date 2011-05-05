﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.1.
// 
#pragma warning disable 1591

namespace PraticeManagement.AttachmentService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="AttachmentServiceSoap", Namespace="http://tempuri.org/")]
    public partial class AttachmentService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback SaveProjectAttachmentOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetProjectAttachmentDataOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public AttachmentService() {
            this.Url = global::PraticeManagement.Properties.Settings.Default.PraticeManagement_AttachmentService_AttachmentService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event SaveProjectAttachmentCompletedEventHandler SaveProjectAttachmentCompleted;
        
        /// <remarks/>
        public event GetProjectAttachmentDataCompletedEventHandler GetProjectAttachmentDataCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SaveProjectAttachment", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SaveProjectAttachment(ProjectAttachment sow, int projectId) {
            this.Invoke("SaveProjectAttachment", new object[] {
                        sow,
                        projectId});
        }
        
        /// <remarks/>
        public void SaveProjectAttachmentAsync(ProjectAttachment sow, int projectId) {
            this.SaveProjectAttachmentAsync(sow, projectId, null);
        }
        
        /// <remarks/>
        public void SaveProjectAttachmentAsync(ProjectAttachment sow, int projectId, object userState) {
            if ((this.SaveProjectAttachmentOperationCompleted == null)) {
                this.SaveProjectAttachmentOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSaveProjectAttachmentOperationCompleted);
            }
            this.InvokeAsync("SaveProjectAttachment", new object[] {
                        sow,
                        projectId}, this.SaveProjectAttachmentOperationCompleted, userState);
        }
        
        private void OnSaveProjectAttachmentOperationCompleted(object arg) {
            if ((this.SaveProjectAttachmentCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SaveProjectAttachmentCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetProjectAttachmentData", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] GetProjectAttachmentData(int projectId) {
            object[] results = this.Invoke("GetProjectAttachmentData", new object[] {
                        projectId});
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public void GetProjectAttachmentDataAsync(int projectId) {
            this.GetProjectAttachmentDataAsync(projectId, null);
        }
        
        /// <remarks/>
        public void GetProjectAttachmentDataAsync(int projectId, object userState) {
            if ((this.GetProjectAttachmentDataOperationCompleted == null)) {
                this.GetProjectAttachmentDataOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetProjectAttachmentDataOperationCompleted);
            }
            this.InvokeAsync("GetProjectAttachmentData", new object[] {
                        projectId}, this.GetProjectAttachmentDataOperationCompleted, userState);
        }
        
        private void OnGetProjectAttachmentDataOperationCompleted(object arg) {
            if ((this.GetProjectAttachmentDataCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetProjectAttachmentDataCompleted(this, new GetProjectAttachmentDataCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ProjectAttachment {
        
        private string attachmentFileNameField;
        
        private byte[] attachmentDataField;
        
        /// <remarks/>
        public string AttachmentFileName {
            get {
                return this.attachmentFileNameField;
            }
            set {
                this.attachmentFileNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] AttachmentData {
            get {
                return this.attachmentDataField;
            }
            set {
                this.attachmentDataField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void SaveProjectAttachmentCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetProjectAttachmentDataCompletedEventHandler(object sender, GetProjectAttachmentDataCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetProjectAttachmentDataCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetProjectAttachmentDataCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591

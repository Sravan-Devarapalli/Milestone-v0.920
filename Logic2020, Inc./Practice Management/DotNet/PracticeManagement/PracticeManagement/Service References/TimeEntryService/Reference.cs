﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PraticeManagement.TimeEntryService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TimeEntriesGroupedByPerson", Namespace="http://schemas.datacontract.org/2004/07/DataTransferObjects.CompositeObjects")]
    [System.SerializableAttribute()]
    public partial class TimeEntriesGroupedByPerson : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.Dictionary<DataTransferObjects.Person, DataTransferObjects.TimeEntry.TimeEntryRecord[]> _groupedTimeEtnriesField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.Dictionary<DataTransferObjects.Person, DataTransferObjects.TimeEntry.TimeEntryRecord[]> _groupedTimeEtnries {
            get {
                return this._groupedTimeEtnriesField;
            }
            set {
                if ((object.ReferenceEquals(this._groupedTimeEtnriesField, value) != true)) {
                    this._groupedTimeEtnriesField = value;
                    this.RaisePropertyChanged("_groupedTimeEtnries");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TimeEntriesGroupedByTimeEntryHours", Namespace="http://schemas.datacontract.org/2004/07/DataTransferObjects.CompositeObjects")]
    [System.SerializableAttribute()]
    public partial class TimeEntriesGroupedByTimeEntryHours : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.Dictionary<DataTransferObjects.TimeEntry.TimeEntryHours, DataTransferObjects.TimeEntry.TimeEntryRecord[]> _groupedTimeEtnriesField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.Dictionary<DataTransferObjects.TimeEntry.TimeEntryHours, DataTransferObjects.TimeEntry.TimeEntryRecord[]> _groupedTimeEtnries {
            get {
                return this._groupedTimeEtnriesField;
            }
            set {
                if ((object.ReferenceEquals(this._groupedTimeEtnriesField, value) != true)) {
                    this._groupedTimeEtnriesField = value;
                    this.RaisePropertyChanged("_groupedTimeEtnries");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TimeEntriesGroupedByProject", Namespace="http://schemas.datacontract.org/2004/07/DataTransferObjects.CompositeObjects")]
    [System.SerializableAttribute()]
    public partial class TimeEntriesGroupedByProject : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.Dictionary<DataTransferObjects.Project, DataTransferObjects.TimeEntry.TimeEntryRecord[]> _groupedTimeEtnriesField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.Dictionary<DataTransferObjects.Project, DataTransferObjects.TimeEntry.TimeEntryRecord[]> _groupedTimeEtnries {
            get {
                return this._groupedTimeEtnriesField;
            }
            set {
                if ((object.ReferenceEquals(this._groupedTimeEtnriesField, value) != true)) {
                    this._groupedTimeEtnriesField = value;
                    this.RaisePropertyChanged("_groupedTimeEtnries");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="TimeEntryService.ITimeEntryService")]
    public interface ITimeEntryService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/GetAllTimeTypes", ReplyAction="http://tempuri.org/ITimeEntryService/GetAllTimeTypesResponse")]
        DataTransferObjects.TimeEntry.TimeTypeRecord[] GetAllTimeTypes();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/RemoveTimeType", ReplyAction="http://tempuri.org/ITimeEntryService/RemoveTimeTypeResponse")]
        void RemoveTimeType(DataTransferObjects.TimeEntry.TimeTypeRecord timeType);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/UpdateTimeType", ReplyAction="http://tempuri.org/ITimeEntryService/UpdateTimeTypeResponse")]
        void UpdateTimeType(DataTransferObjects.TimeEntry.TimeTypeRecord timeType);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/AddTimeType", ReplyAction="http://tempuri.org/ITimeEntryService/AddTimeTypeResponse")]
        int AddTimeType(DataTransferObjects.TimeEntry.TimeTypeRecord timeType);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/TimeZonesAll", ReplyAction="http://tempuri.org/ITimeEntryService/TimeZonesAllResponse")]
        DataTransferObjects.Timezone[] TimeZonesAll();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/SetTimeZone", ReplyAction="http://tempuri.org/ITimeEntryService/SetTimeZoneResponse")]
        void SetTimeZone(DataTransferObjects.Timezone timezone);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/RemoveTimeEntries", ReplyAction="http://tempuri.org/ITimeEntryService/RemoveTimeEntriesResponse")]
        void RemoveTimeEntries(int milestonePersonId, int timeTypeId, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/RemoveTimeEntryById", ReplyAction="http://tempuri.org/ITimeEntryService/RemoveTimeEntryByIdResponse")]
        void RemoveTimeEntryById(int id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/RemoveTimeEntry", ReplyAction="http://tempuri.org/ITimeEntryService/RemoveTimeEntryResponse")]
        void RemoveTimeEntry(DataTransferObjects.TimeEntry.TimeEntryRecord timeEntry);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/AddTimeEntry", ReplyAction="http://tempuri.org/ITimeEntryService/AddTimeEntryResponse")]
        int AddTimeEntry(DataTransferObjects.TimeEntry.TimeEntryRecord timeEntry, int defaultMpId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/HasTimeEntriesForMilestone", ReplyAction="http://tempuri.org/ITimeEntryService/HasTimeEntriesForMilestoneResponse")]
        bool HasTimeEntriesForMilestone(int milestoneId, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/UpdateTimeEntry", ReplyAction="http://tempuri.org/ITimeEntryService/UpdateTimeEntryResponse")]
        void UpdateTimeEntry(DataTransferObjects.TimeEntry.TimeEntryRecord timeEntry, int defaultMilestoneId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/ConstructAndUpdateTimeEntry", ReplyAction="http://tempuri.org/ITimeEntryService/ConstructAndUpdateTimeEntryResponse")]
        void ConstructAndUpdateTimeEntry(int id, string milestoneDate, string entryDate, int milestonePersonId, double actualHours, double forecastedHours, int timeTypeId, string note, string isReviewed, bool isChargeable, bool isCorrect, int modifiedById);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/GetTimeEntriesForPerson", ReplyAction="http://tempuri.org/ITimeEntryService/GetTimeEntriesForPersonResponse")]
        DataTransferObjects.TimeEntry.TimeEntryRecord[] GetTimeEntriesForPerson(DataTransferObjects.Person person, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/GetTimeEntriesProject", ReplyAction="http://tempuri.org/ITimeEntryService/GetTimeEntriesProjectResponse")]
        PraticeManagement.TimeEntryService.TimeEntriesGroupedByPerson GetTimeEntriesProject(DataTransferObjects.ContextObjects.TimeEntryProjectReportContext reportContext);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/GetTimeEntriesProjectCumulative", ReplyAction="http://tempuri.org/ITimeEntryService/GetTimeEntriesProjectCumulativeResponse")]
        PraticeManagement.TimeEntryService.TimeEntriesGroupedByTimeEntryHours GetTimeEntriesProjectCumulative(DataTransferObjects.ContextObjects.TimeEntryPersonReportContext reportContext);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/GetTimeEntriesPerson", ReplyAction="http://tempuri.org/ITimeEntryService/GetTimeEntriesPersonResponse")]
        PraticeManagement.TimeEntryService.TimeEntriesGroupedByProject GetTimeEntriesPerson(DataTransferObjects.ContextObjects.TimeEntryPersonReportContext reportContext);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/GetCurrentMilestones", ReplyAction="http://tempuri.org/ITimeEntryService/GetCurrentMilestonesResponse")]
        DataTransferObjects.MilestonePersonEntry[] GetCurrentMilestones(DataTransferObjects.Person person, System.DateTime startDate, System.DateTime endDate, int defaultMilestoneId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/GetTimeEntryMilestones", ReplyAction="http://tempuri.org/ITimeEntryService/GetTimeEntryMilestonesResponse")]
        DataTransferObjects.MilestonePersonEntry[] GetTimeEntryMilestones(DataTransferObjects.Person person, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/GetAllTimeEntries", ReplyAction="http://tempuri.org/ITimeEntryService/GetAllTimeEntriesResponse")]
        DataTransferObjects.TimeEntry.TimeEntryRecord[] GetAllTimeEntries(DataTransferObjects.ContextObjects.TimeEntrySelectContext selectContext, int startRow, int maxRows);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/GetTimeEntriesCount", ReplyAction="http://tempuri.org/ITimeEntryService/GetTimeEntriesCountResponse")]
        int GetTimeEntriesCount(DataTransferObjects.ContextObjects.TimeEntrySelectContext selectContext);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/GetTimeEntrySums", ReplyAction="http://tempuri.org/ITimeEntryService/GetTimeEntrySumsResponse")]
        DataTransferObjects.TimeEntry.TimeEntrySums GetTimeEntrySums(DataTransferObjects.ContextObjects.TimeEntrySelectContext selectContext);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/ToggleIsCorrect", ReplyAction="http://tempuri.org/ITimeEntryService/ToggleIsCorrectResponse")]
        void ToggleIsCorrect(DataTransferObjects.TimeEntry.TimeEntryRecord timeEntry);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/ToggleIsReviewed", ReplyAction="http://tempuri.org/ITimeEntryService/ToggleIsReviewedResponse")]
        void ToggleIsReviewed(DataTransferObjects.TimeEntry.TimeEntryRecord timeEntry);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/ToggleIsChargeable", ReplyAction="http://tempuri.org/ITimeEntryService/ToggleIsChargeableResponse")]
        void ToggleIsChargeable(DataTransferObjects.TimeEntry.TimeEntryRecord timeEntry);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/GetAllTimeEntryProjects", ReplyAction="http://tempuri.org/ITimeEntryService/GetAllTimeEntryProjectsResponse")]
        DataTransferObjects.Project[] GetAllTimeEntryProjects();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/GetTimeEntryProjectsByClientId", ReplyAction="http://tempuri.org/ITimeEntryService/GetTimeEntryProjectsByClientIdResponse")]
        DataTransferObjects.Project[] GetTimeEntryProjectsByClientId(System.Nullable<int> clientId, bool showActiveAndInternalProjectsOnly);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/GetAllTimeEntryMilestones", ReplyAction="http://tempuri.org/ITimeEntryService/GetAllTimeEntryMilestonesResponse")]
        DataTransferObjects.Milestone[] GetAllTimeEntryMilestones();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITimeEntryService/GetAllTimeEntryPersons", ReplyAction="http://tempuri.org/ITimeEntryService/GetAllTimeEntryPersonsResponse")]
        DataTransferObjects.Person[] GetAllTimeEntryPersons(System.DateTime entryDateFrom, System.DateTime entryDateTo);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ITimeEntryServiceChannel : PraticeManagement.TimeEntryService.ITimeEntryService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TimeEntryServiceClient : System.ServiceModel.ClientBase<PraticeManagement.TimeEntryService.ITimeEntryService>, PraticeManagement.TimeEntryService.ITimeEntryService {
        
        public TimeEntryServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public TimeEntryServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TimeEntryServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TimeEntryServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public DataTransferObjects.TimeEntry.TimeTypeRecord[] GetAllTimeTypes() {
            return base.Channel.GetAllTimeTypes();
        }
        
        public void RemoveTimeType(DataTransferObjects.TimeEntry.TimeTypeRecord timeType) {
            base.Channel.RemoveTimeType(timeType);
        }
        
        public void UpdateTimeType(DataTransferObjects.TimeEntry.TimeTypeRecord timeType) {
            base.Channel.UpdateTimeType(timeType);
        }
        
        public int AddTimeType(DataTransferObjects.TimeEntry.TimeTypeRecord timeType) {
            return base.Channel.AddTimeType(timeType);
        }
        
        public DataTransferObjects.Timezone[] TimeZonesAll() {
            return base.Channel.TimeZonesAll();
        }
        
        public void SetTimeZone(DataTransferObjects.Timezone timezone) {
            base.Channel.SetTimeZone(timezone);
        }
        
        public void RemoveTimeEntries(int milestonePersonId, int timeTypeId, System.DateTime startDate, System.DateTime endDate) {
            base.Channel.RemoveTimeEntries(milestonePersonId, timeTypeId, startDate, endDate);
        }
        
        public void RemoveTimeEntryById(int id) {
            base.Channel.RemoveTimeEntryById(id);
        }
        
        public void RemoveTimeEntry(DataTransferObjects.TimeEntry.TimeEntryRecord timeEntry) {
            base.Channel.RemoveTimeEntry(timeEntry);
        }
        
        public int AddTimeEntry(DataTransferObjects.TimeEntry.TimeEntryRecord timeEntry, int defaultMpId) {
            return base.Channel.AddTimeEntry(timeEntry, defaultMpId);
        }
        
        public bool HasTimeEntriesForMilestone(int milestoneId, System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.HasTimeEntriesForMilestone(milestoneId, startDate, endDate);
        }
        
        public void UpdateTimeEntry(DataTransferObjects.TimeEntry.TimeEntryRecord timeEntry, int defaultMilestoneId) {
            base.Channel.UpdateTimeEntry(timeEntry, defaultMilestoneId);
        }
        
        public void ConstructAndUpdateTimeEntry(int id, string milestoneDate, string entryDate, int milestonePersonId, double actualHours, double forecastedHours, int timeTypeId, string note, string isReviewed, bool isChargeable, bool isCorrect, int modifiedById) {
            base.Channel.ConstructAndUpdateTimeEntry(id, milestoneDate, entryDate, milestonePersonId, actualHours, forecastedHours, timeTypeId, note, isReviewed, isChargeable, isCorrect, modifiedById);
        }
        
        public DataTransferObjects.TimeEntry.TimeEntryRecord[] GetTimeEntriesForPerson(DataTransferObjects.Person person, System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.GetTimeEntriesForPerson(person, startDate, endDate);
        }
        
        public PraticeManagement.TimeEntryService.TimeEntriesGroupedByPerson GetTimeEntriesProject(DataTransferObjects.ContextObjects.TimeEntryProjectReportContext reportContext) {
            return base.Channel.GetTimeEntriesProject(reportContext);
        }
        
        public PraticeManagement.TimeEntryService.TimeEntriesGroupedByTimeEntryHours GetTimeEntriesProjectCumulative(DataTransferObjects.ContextObjects.TimeEntryPersonReportContext reportContext) {
            return base.Channel.GetTimeEntriesProjectCumulative(reportContext);
        }
        
        public PraticeManagement.TimeEntryService.TimeEntriesGroupedByProject GetTimeEntriesPerson(DataTransferObjects.ContextObjects.TimeEntryPersonReportContext reportContext) {
            return base.Channel.GetTimeEntriesPerson(reportContext);
        }
        
        public DataTransferObjects.MilestonePersonEntry[] GetCurrentMilestones(DataTransferObjects.Person person, System.DateTime startDate, System.DateTime endDate, int defaultMilestoneId) {
            return base.Channel.GetCurrentMilestones(person, startDate, endDate, defaultMilestoneId);
        }
        
        public DataTransferObjects.MilestonePersonEntry[] GetTimeEntryMilestones(DataTransferObjects.Person person, System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.GetTimeEntryMilestones(person, startDate, endDate);
        }
        
        public DataTransferObjects.TimeEntry.TimeEntryRecord[] GetAllTimeEntries(DataTransferObjects.ContextObjects.TimeEntrySelectContext selectContext, int startRow, int maxRows) {
            return base.Channel.GetAllTimeEntries(selectContext, startRow, maxRows);
        }
        
        public int GetTimeEntriesCount(DataTransferObjects.ContextObjects.TimeEntrySelectContext selectContext) {
            return base.Channel.GetTimeEntriesCount(selectContext);
        }
        
        public DataTransferObjects.TimeEntry.TimeEntrySums GetTimeEntrySums(DataTransferObjects.ContextObjects.TimeEntrySelectContext selectContext) {
            return base.Channel.GetTimeEntrySums(selectContext);
        }
        
        public void ToggleIsCorrect(DataTransferObjects.TimeEntry.TimeEntryRecord timeEntry) {
            base.Channel.ToggleIsCorrect(timeEntry);
        }
        
        public void ToggleIsReviewed(DataTransferObjects.TimeEntry.TimeEntryRecord timeEntry) {
            base.Channel.ToggleIsReviewed(timeEntry);
        }
        
        public void ToggleIsChargeable(DataTransferObjects.TimeEntry.TimeEntryRecord timeEntry) {
            base.Channel.ToggleIsChargeable(timeEntry);
        }
        
        public DataTransferObjects.Project[] GetAllTimeEntryProjects() {
            return base.Channel.GetAllTimeEntryProjects();
        }
        
        public DataTransferObjects.Project[] GetTimeEntryProjectsByClientId(System.Nullable<int> clientId, bool showActiveAndInternalProjectsOnly) {
            return base.Channel.GetTimeEntryProjectsByClientId(clientId, showActiveAndInternalProjectsOnly);
        }
        
        public DataTransferObjects.Milestone[] GetAllTimeEntryMilestones() {
            return base.Channel.GetAllTimeEntryMilestones();
        }
        
        public DataTransferObjects.Person[] GetAllTimeEntryPersons(System.DateTime entryDateFrom, System.DateTime entryDateTo) {
            return base.Channel.GetAllTimeEntryPersons(entryDateFrom, entryDateTo);
        }
    }
}


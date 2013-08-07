﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1008
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PraticeManagement.CalendarService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="CalendarService.ICalendarService")]
    public interface ICalendarService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICalendarService/GetCalendar", ReplyAction="http://tempuri.org/ICalendarService/GetCalendarResponse")]
        DataTransferObjects.CalendarItem[] GetCalendar(System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICalendarService/GetPersonCalendar", ReplyAction="http://tempuri.org/ICalendarService/GetPersonCalendarResponse")]
        DataTransferObjects.CalendarItem[] GetPersonCalendar(System.DateTime startDate, System.DateTime endDate, System.Nullable<int> personId, System.Nullable<int> practiceManagerId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICalendarService/SaveCalendar", ReplyAction="http://tempuri.org/ICalendarService/SaveCalendarResponse")]
        void SaveCalendar(DataTransferObjects.CalendarItem item, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICalendarService/GetCompanyHolidays", ReplyAction="http://tempuri.org/ICalendarService/GetCompanyHolidaysResponse")]
        int GetCompanyHolidays(int year);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICalendarService/GetRecurringHolidaysList", ReplyAction="http://tempuri.org/ICalendarService/GetRecurringHolidaysListResponse")]
        DataTransferObjects.Triple<int, string, bool>[] GetRecurringHolidaysList();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICalendarService/SetRecurringHoliday", ReplyAction="http://tempuri.org/ICalendarService/SetRecurringHolidayResponse")]
        void SetRecurringHoliday(System.Nullable<int> recurringHolidayId, bool isSet, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICalendarService/GetRecurringHolidaysInWeek", ReplyAction="http://tempuri.org/ICalendarService/GetRecurringHolidaysInWeekResponse")]
        System.Collections.Generic.Dictionary<System.DateTime, string> GetRecurringHolidaysInWeek(System.DateTime date, int personId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICalendarService/SaveSubstituteDay", ReplyAction="http://tempuri.org/ICalendarService/SaveSubstituteDayResponse")]
        void SaveSubstituteDay(DataTransferObjects.CalendarItem item, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICalendarService/DeleteSubstituteDay", ReplyAction="http://tempuri.org/ICalendarService/DeleteSubstituteDayResponse")]
        void DeleteSubstituteDay(int personId, System.DateTime substituteDayDate, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICalendarService/SaveTimeOff", ReplyAction="http://tempuri.org/ICalendarService/SaveTimeOffResponse")]
        void SaveTimeOff(System.DateTime startDate, System.DateTime endDate, bool dayOff, int personId, System.Nullable<double> actualHours, int timeTypeId, string userLogin, System.Nullable<int> approvedBy, System.Nullable<System.DateTime> OldSeriesStartDate, bool isFromAddTimeOffBtn);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICalendarService/GetTimeOffSeriesPeriod", ReplyAction="http://tempuri.org/ICalendarService/GetTimeOffSeriesPeriodResponse")]
        DataTransferObjects.Quadruple<System.DateTime, System.DateTime, System.Nullable<int>, string> GetTimeOffSeriesPeriod(int personId, System.DateTime date);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICalendarService/GetSubstituteDate", ReplyAction="http://tempuri.org/ICalendarService/GetSubstituteDateResponse")]
        System.DateTime GetSubstituteDate(int personId, System.DateTime holidayDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICalendarService/GetSubstituteDayDetails", ReplyAction="http://tempuri.org/ICalendarService/GetSubstituteDayDetailsResponse")]
        System.Collections.Generic.KeyValuePair<System.DateTime, string> GetSubstituteDayDetails(int personId, System.DateTime substituteDate);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICalendarServiceChannel : PraticeManagement.CalendarService.ICalendarService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CalendarServiceClient : System.ServiceModel.ClientBase<PraticeManagement.CalendarService.ICalendarService>, PraticeManagement.CalendarService.ICalendarService {
        
   
        
        public CalendarServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CalendarServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CalendarServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CalendarServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public DataTransferObjects.CalendarItem[] GetCalendar(System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.GetCalendar(startDate, endDate);
        }
        
        public DataTransferObjects.CalendarItem[] GetPersonCalendar(System.DateTime startDate, System.DateTime endDate, System.Nullable<int> personId, System.Nullable<int> practiceManagerId) {
            return base.Channel.GetPersonCalendar(startDate, endDate, personId, practiceManagerId);
        }
        
        public void SaveCalendar(DataTransferObjects.CalendarItem item, string userLogin) {
            base.Channel.SaveCalendar(item, userLogin);
        }
        
        public int GetCompanyHolidays(int year) {
            return base.Channel.GetCompanyHolidays(year);
        }
        
        public DataTransferObjects.Triple<int, string, bool>[] GetRecurringHolidaysList() {
            return base.Channel.GetRecurringHolidaysList();
        }
        
        public void SetRecurringHoliday(System.Nullable<int> recurringHolidayId, bool isSet, string userLogin) {
            base.Channel.SetRecurringHoliday(recurringHolidayId, isSet, userLogin);
        }
        
        public System.Collections.Generic.Dictionary<System.DateTime, string> GetRecurringHolidaysInWeek(System.DateTime date, int personId) {
            return base.Channel.GetRecurringHolidaysInWeek(date, personId);
        }
        
        public void SaveSubstituteDay(DataTransferObjects.CalendarItem item, string userLogin) {
            base.Channel.SaveSubstituteDay(item, userLogin);
        }
        
        public void DeleteSubstituteDay(int personId, System.DateTime substituteDayDate, string userLogin) {
            base.Channel.DeleteSubstituteDay(personId, substituteDayDate, userLogin);
        }
        
        public void SaveTimeOff(System.DateTime startDate, System.DateTime endDate, bool dayOff, int personId, System.Nullable<double> actualHours, int timeTypeId, string userLogin, System.Nullable<int> approvedBy, System.Nullable<System.DateTime> OldSeriesStartDate, bool isFromAddTimeOffBtn) {
            base.Channel.SaveTimeOff(startDate, endDate, dayOff, personId, actualHours, timeTypeId, userLogin, approvedBy, OldSeriesStartDate, isFromAddTimeOffBtn);
        }
        
        public DataTransferObjects.Quadruple<System.DateTime, System.DateTime, System.Nullable<int>, string> GetTimeOffSeriesPeriod(int personId, System.DateTime date) {
            return base.Channel.GetTimeOffSeriesPeriod(personId, date);
        }
        
        public System.DateTime GetSubstituteDate(int personId, System.DateTime holidayDate) {
            return base.Channel.GetSubstituteDate(personId, holidayDate);
        }
        
        public System.Collections.Generic.KeyValuePair<System.DateTime, string> GetSubstituteDayDetails(int personId, System.DateTime substituteDate) {
            return base.Channel.GetSubstituteDayDetails(personId, substituteDate);
        }
    }
}


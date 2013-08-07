using System;
using System.Collections.Generic;
using System.ServiceModel;

using DataTransferObjects;

namespace PracticeManagementService
{
    [ServiceContract]
    public interface ICalendarService
    {
        /// <summary>
        /// Retrieves a list of the calendar items from the database.
        /// </summary>
        /// <param name="startDate">The start of the period.</param>
        /// <param name="endDate">The end of the period.</param>
        /// An ID of the person to the calendar be retrieved for.
        /// If null the company calendar will be returned.
        /// An ID of the practice manager to retrieve the data for his subordinate
        /// <returns>The list of the <see cref="CalendarItem"/> objects.</returns>
        [OperationContract]
        List<CalendarItem> GetCalendar(DateTime startDate, DateTime endDate);

        [OperationContract]
        List<CalendarItem> GetPersonCalendar(DateTime startDate, DateTime endDate, int? personId, int? practiceManagerId);

        /// <summary>
        /// Saves a <see cref="CalendarItem"/> object to the database.
        /// </summary>
        /// <param name="item">The data to be saved to.</param>
        [OperationContract]
        void SaveCalendar(CalendarItem item, string userLogin);

        /// <summary>
        /// Returns No. of Company holidays in a given year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        [OperationContract]
        int GetCompanyHolidays(int year);

        [OperationContract]
        List<Triple<int, string, bool>> GetRecurringHolidaysList();

        [OperationContract]
        void SetRecurringHoliday(int? recurringHolidayId, bool isSet, string userLogin);

        [OperationContract]
        Dictionary<DateTime, string> GetRecurringHolidaysInWeek(DateTime date, int personId);

        [OperationContract]
        void SaveSubstituteDay(CalendarItem item, string userLogin);

        [OperationContract]
        void DeleteSubstituteDay(int personId, DateTime substituteDayDate, string userLogin);

        [OperationContract]
        void SaveTimeOff(DateTime startDate, DateTime endDate, bool dayOff, int personId, double? actualHours, int timeTypeId, string userLogin, int? approvedBy, DateTime? OldSeriesStartDate, bool isFromAddTimeOffBtn);

        [OperationContract]
        Quadruple<DateTime, DateTime, int?, string> GetTimeOffSeriesPeriod(int personId, DateTime date);

        [OperationContract]
        DateTime GetSubstituteDate(int personId, DateTime holidayDate);

        [OperationContract]
        KeyValuePair<DateTime, string> GetSubstituteDayDetails(int personId, DateTime substituteDate);
    }
}

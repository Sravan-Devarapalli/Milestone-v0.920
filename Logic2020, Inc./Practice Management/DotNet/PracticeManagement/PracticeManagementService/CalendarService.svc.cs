using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using DataAccess;
using DataTransferObjects;

namespace PracticeManagementService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CalendarService : ICalendarService
    {
        #region ICalendarService Members

        /// <summary>
        /// Retrieves a list of the calendar items from the database.
        /// </summary>
        /// <param name="startDate">The start of the period.</param>
        /// <param name="endDate">The end of the period.</param>
        /// <param name="personId">
        /// An ID of the person to the calendar be retrieved for.
        /// If null the company calendar will be returned.
        /// </param>
        /// <param name="practiceManagerId">
        /// An ID of the practice manager to retrieve the data for his subordinate
        /// </param>
        /// <returns>The list of the <see cref="CalendarItem"/> objects.</returns>
        public List<CalendarItem> GetCalendar(DateTime startDate, DateTime endDate)
        {
            return CalendarDAL.CalendarList(startDate, endDate);
        }

        public List<CalendarItem> GetPersonCalendar(DateTime startDate, DateTime endDate, int? personId, int? practiceManagerId)
        {
            return CalendarDAL.PersonCalendarList(startDate, endDate, personId, practiceManagerId);
        }

        /// <summary>
        /// Saves a <see cref="CalendarItem"/> object to the database.
        /// </summary>
        /// <param name="item">The data to be saved to.</param>
        public void SaveCalendar(CalendarItem item, string userLogin)
        {
            CalendarDAL.CalendarUpdate(item, userLogin);
        }

        public void SaveSubstituteDay(CalendarItem item, string userLogin)
        {
            CalendarDAL.SaveSubstituteDay(item, userLogin);
        }

        public void DeleteSubstituteDay(int personId, DateTime substituteDayDate, string userLogin)
        {
            CalendarDAL.DeleteSubstituteDay(personId, substituteDayDate, userLogin);
        }

        /// <summary>
        /// Returns No. of Company holidays in a given year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public int GetCompanyHolidays(int year)
        {
            return CalendarDAL.GetCompanyHolidays(year);
        }

        public List<Triple<int, string, bool>> GetRecurringHolidaysList()
        {
            return CalendarDAL.GetRecurringHolidaysList();
        }

        public void SetRecurringHoliday(int? recurringHolidayId, bool isSet, string userLogin)
        {
            CalendarDAL.SetRecurringHoliday(recurringHolidayId, isSet, userLogin);
        }

        public Dictionary<DateTime, string> GetRecurringHolidaysInWeek(DateTime date, int personId)
        {
            return CalendarDAL.GetRecurringHolidaysInWeek(date, personId);
        }

        public void SaveTimeOff(DateTime startDate, DateTime endDate, bool dayOff, int personId, double? actualHours, int timeTypeId, string userLogin, int? approvedBy)
        {
            CalendarDAL.SaveTimeOff(startDate, endDate, dayOff, personId, actualHours, timeTypeId, userLogin, approvedBy);
        }

        public Quadruple<DateTime, DateTime, int?, string> GetTimeOffSeriesPeriod(int personId, DateTime date)
        {
            return CalendarDAL.GetTimeOffSeriesPeriod(personId, date);
        }

        public DateTime GetSubstituteDate(int personId, DateTime holidayDate)
        {
            return CalendarDAL.GetSubstituteDate(personId, holidayDate);
        }

        public KeyValuePair<DateTime, string> GetSubstituteDayDetails(int personId, DateTime substituteDate)
        {
            return CalendarDAL.GetSubstituteDayDetails(personId, substituteDate);
        }


        #endregion
    }
}


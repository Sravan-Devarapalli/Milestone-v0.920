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
		/// An ID of the person to the cxalendar be retrived for.
		/// If null the company calendar will be returned.
		/// </param>
		/// <param name="practiceManagerId">
		/// An ID of the practice manager to retrieve the data for his subordinate
		/// </param>
		/// <returns>The list of the <see cref="CalendarItem"/> objects.</returns>
		public List<CalendarItem> GetCalendar(DateTime startDate, DateTime endDate, int? personId, int? practiceManagerId)
		{
			return CalendarDAL.CalendarList(startDate, endDate, personId, practiceManagerId);
		}

		/// <summary>
		/// Saves a <see cref="CalendarItem"/> object to the database.
		/// </summary>
		/// <param name="item">The data to be saved to.</param>
		public void SaveCalendar(CalendarItem item)
		{
			CalendarDAL.CalendarUpdate(item);
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

		#endregion
	}
}


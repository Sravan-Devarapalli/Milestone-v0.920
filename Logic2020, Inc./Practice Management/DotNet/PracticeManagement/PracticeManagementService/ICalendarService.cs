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
		/// <param name="personId">
		/// An ID of the person to the cxalendar be retrived for.
		/// If null the company calendar will be returned.
		/// </param>
		/// <param name="practiceManagerId">
		/// An ID of the practice manager to retrieve the data for his subordinate
		/// </param>
		/// <returns>The list of the <see cref="CalendarItem"/> objects.</returns>
		[OperationContract]
		List<CalendarItem> GetCalendar(DateTime startDate, DateTime endDate, int? personId, int? practiceManagerId);

		/// <summary>
		/// Saves a <see cref="CalendarItem"/> object to the database.
		/// </summary>
		/// <param name="item">The data to be saved to.</param>
		[OperationContract]
		void SaveCalendar(CalendarItem item);

        /// <summary>
        /// Returns No. of Company holidays in a given year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        [OperationContract]
        int GetCompanyHolidays(int year);
	}
}


using DataTransferObjects;
using PraticeManagement.CalendarService;
using System;

namespace PraticeManagement.Utils
{
    public class Calendar
    {
        public const int DefaultHoursPerWeek = 40;

        public static string GetCssClassByCalendarItem(CalendarItem calendarItem)
        {
            if (calendarItem == null)
                return string.Empty;

            return calendarItem.DayOff
                        ? (calendarItem.CompanyDayOff ? Resources.Controls.CssDayOff : Resources.Controls.CssCompanyDayOn)
                        : (calendarItem.CompanyDayOff ? Resources.Controls.CssCompanyDayOff : Resources.Controls.CssDayOn);
        }

        public static decimal GetWorkingHoursInCurrentYear(decimal hoursPerWeek)
        {
            //int companyHolidays=0;
            //using (var service = new CalendarServiceClient())
            //{
            //    companyHolidays = service.GetCompanyHolidays(DateTime.Now.Year);
            //}
            //return (52 * hoursPerWeek) - (companyHolidays * hoursPerWeek / 5);
            return (52 * hoursPerWeek);
        }
    }
}

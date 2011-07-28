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
                        ? (calendarItem.CompanyDayOff 
                            ? (calendarItem.Date.DayOfWeek == DayOfWeek.Sunday || calendarItem.Date.DayOfWeek == DayOfWeek.Saturday ? Resources.Controls.CssWeekEndDayOff : Resources.Controls.CssDayOff) 
                            : (calendarItem.Date.DayOfWeek == DayOfWeek.Sunday || calendarItem.Date.DayOfWeek == DayOfWeek.Saturday ? Resources.Controls.CssWeekEndDayOff : Resources.Controls.CssCompanyDayOn)
                          )
                        : (calendarItem.CompanyDayOff 
                            ? (calendarItem.Date.DayOfWeek == DayOfWeek.Sunday || calendarItem.Date.DayOfWeek == DayOfWeek.Saturday ? Resources.Controls.CssWeekEndDayOn : Resources.Controls.CssCompanyDayOff )
                            : (calendarItem.Date.DayOfWeek == DayOfWeek.Sunday || calendarItem.Date.DayOfWeek == DayOfWeek.Saturday ? Resources.Controls.CssWeekEndDayOn : Resources.Controls.CssDayOn)
                          );
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

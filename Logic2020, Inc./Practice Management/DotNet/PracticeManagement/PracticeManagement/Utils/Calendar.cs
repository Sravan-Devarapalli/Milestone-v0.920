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

        public static DateTime WeekStartDate(DateTime now)
        {
            return now.AddDays(-(int)now.DayOfWeek);
        }

        public static DateTime WeekEndDate(DateTime now)
        {
            return now.AddDays(6 - (int)now.DayOfWeek);
        }

        public static DateTime MonthStartDate(DateTime now)
        {
            return now.AddDays(1 - now.Day);
        }

        public static DateTime MonthEndDate(DateTime now)
        {
            return now.AddMonths(1).AddDays(-now.AddMonths(1).Day);
        }

        public static DateTime YearStartDate(DateTime now)
        {
            return now.AddDays(1 - now.DayOfYear);
        }

        public static DateTime YearEndDate(DateTime now)
        {
            return now.AddYears(1).AddDays(-now.AddYears(1).DayOfYear);
        }

        public static DateTime LastWeekStartDate(DateTime now)
        {
            return now.AddDays(-(int)now.DayOfWeek - 7);
        }

        public static DateTime LastWeekEndDate(DateTime now)
        {
            return now.AddDays(-(int)now.DayOfWeek - 1);
        }

        public static DateTime LastMonthStartDate(DateTime now)
        {
            return now.AddMonths(-1).AddDays(1 - now.AddMonths(-1).Day);
        }

        public static DateTime LastMonthEndDate(DateTime now)
        {
            return now.AddDays(-now.Day);
        }

        public static DateTime LastYearStartDate(DateTime now)
        {
            return now.AddYears(-1).AddDays(1 - now.AddYears(-1).DayOfYear);
        }

        public static DateTime LastYearEndDate(DateTime now)
        {
            return now.AddDays(-now.DayOfYear);
        }
    }
}

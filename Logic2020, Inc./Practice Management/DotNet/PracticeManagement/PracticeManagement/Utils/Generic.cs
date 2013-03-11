using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web;
using PraticeManagement.Controls;
using PraticeManagement.Controls.Generic.Filtering;
using ErrorEventArgs = PraticeManagement.Events.ErrorEventArgs;
using DataTransferObjects;
using System.Web.Security;
using System.Web.UI;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace PraticeManagement.Utils
{
    public class Generic
    {
        /// <summary>
        /// Calculates average value of int array, 
        /// not taking into account negative numbers
        /// </summary>
        /// <param name="arr">Int array</param>
        /// <returns>Average vaule</returns>
        public static int AvgLoad(int[] arr, int granularity)
        {
            int avg = 0,
                zeros = 0;
            foreach (int a in arr)
                if ((granularity == 1 && a <= 0) || (granularity > 1 && a < 0)) // if granularity =1 then do not factor the holidays or vacations into utilization %
                    zeros++;
                else
                    avg += a;
            if (arr.Length == zeros)
                return 0;
            else
                return avg / (arr.Length - zeros);
        }

        /// <summary>
        /// Parses date in common format
        /// </summary>
        /// <param name="text">String to parse</param>
        /// <returns>Parsed date, current time otherwise</returns>
        public static DateTime ParseDate(string text)
        {
            try
            {
                return DateTime.ParseExact(
                                        text,
                                        Constants.Formatting.EntryDateFormat,
                                        CultureInfo.CurrentCulture);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public static string SystemVersion
        {
            get
            {
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(currentAssembly.Location);
                return String.Format(Constants.Formatting.CurrentVersionFormat,
                                     assemblyName.Version.Major,
                                     assemblyName.Version.Minor,
                                     assemblyName.Version.Build,
                                     assemblyName.Version.Revision,
                                     new FileInfo(currentAssembly.Location).CreationTime,
                                     DataHelper.GetDatabaseVersion());
            }
        }

        public static string BinariesCreatedTime
        {
            get
            {
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(currentAssembly.Location);
                var fileInfo = new FileInfo(currentAssembly.Location);
                return HttpUtility.HtmlEncode(fileInfo.CreationTime.ToString("Mdyyyyhhmmss"));
            }
        }

        public static string GetClientUrl(string url, Control page)
        {
            return page.ResolveClientUrl(url) + "?time=" + BinariesCreatedTime;
        }

        public static void RedirectWithReturnTo(string targetUrl, string currentUrl, HttpResponse httpResponse)
        {
            httpResponse.Redirect(GetTargetUrlWithReturn(targetUrl, currentUrl));
        }

        public static void RedirectWithoutReturnTo(string targetUrl, HttpResponse httpResponse)
        {
            httpResponse.Redirect(Urls.GetUrlWithoutReturnTo(targetUrl));
        }

        public static string GetTargetUrlWithReturn(string targetUrl, string currentUrl)
        {
            if (String.IsNullOrEmpty(targetUrl))
                throw new ArgumentNullException(
                    Constants.QueryStringParameterNames.RedirectUrlArgument);

            var url = String.Format(
                targetUrl.IndexOf(
                    Constants.QueryStringParameterNames.QueryStringSeparator) < 0 ?
                    Constants.QueryStringParameterNames.RedirectFormat :
                    Constants.QueryStringParameterNames.RedirectWithQueryStringFormat,
                targetUrl,
                HttpUtility.UrlEncode(currentUrl));

            return Urls.GetUrlWithoutReturnTo(url);
        }

        public static DateTime GetWeekStartDate(DateTime now)
        {
            var endDate = GetNextWeeksFirstDay(now);
            var sevenDays = new TimeSpan(7, 0, 0, 0);
            return endDate.Subtract(sevenDays);
        }

        /// <summary>
        /// Returns the date of the next week's first day for a given <see cref="DateTime"/>.
        /// </summary>
        public static DateTime GetNextWeeksFirstDay(DateTime date)
        {
            int daysUntilNextWeeksFirstDay =
                Convert.ToInt32(CultureInfo.CurrentUICulture.DateTimeFormat.FirstDayOfWeek) -
                Convert.ToInt32(date.DayOfWeek) + 7;

            if (daysUntilNextWeeksFirstDay == 8)
            {
                daysUntilNextWeeksFirstDay = 1;
            }

            return date.AddDays(daysUntilNextWeeksFirstDay);
        }

        public static void RedirectIfNullEntity(object entity, HttpResponse response)
        {
            if (entity == null)
                response.Redirect(Constants.ApplicationPages.PageHasBeenRemoved);
        }

        public static int? ParseNullableInt(string personId)
        {
            return !String.IsNullOrEmpty(personId) ? (int?)Int32.Parse(personId) : null;
        }

        public static string AddEllipsis(string input, int maxLen, string ellipsisText)
        {
            if (input.Length > maxLen)
            {
                return input.Substring(0, maxLen) + ellipsisText;
            }
            return input;
        }

        public static void InvokeEventHandler(EventHandler handler, object sender)
        {
            InvokeEventHandler(handler, sender, EventArgs.Empty);
        }

        public static void InvokeEventHandler(EventHandler handler, object sender, EventArgs e)
        {
            if (handler != null) handler(sender, e);
        }

        public static void InvokeEventHandler<T>(EventHandler<T> handler, object sender, T e) where T : EventArgs
        {
            if (handler != null) handler(sender, e);
        }

        public static void InvokeErrorEvent(EventHandler<ErrorEventArgs> handler, object sender, ErrorEventArgs e)
        {
            InvokeEventHandler(handler, sender, e);
        }

        public static void InitStartEndDate(DateInterval1 dateIntervalControl)
        {
            var weekStartDate = GetWeekStartDate(DateTime.Now);
            dateIntervalControl.FromDate = weekStartDate;
            dateIntervalControl.ToDate = weekStartDate.AddDays(7.0);
        }

        public static DateTime GetNowWithTimeZone()
        {
            string timezone = string.Empty;
            string isDayLightSavingsTimeEffect = string.Empty;
            if (HttpContext.Current == null)
            {
                var keyValues = SettingsHelper.GetResourceKeyValuePairs(SettingsType.Application);
                timezone = keyValues[Constants.ResourceKeys.TimeZoneKey];
                isDayLightSavingsTimeEffect = keyValues[Constants.ResourceKeys.IsDayLightSavingsTimeEffectKey];
            }
            else
            {
                timezone = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.TimeZoneKey);
                isDayLightSavingsTimeEffect = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.IsDayLightSavingsTimeEffectKey);
            }

            if (timezone == "-08:00" && isDayLightSavingsTimeEffect.ToLower() == "true")
            {
                return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
            }
            else
            {
                var timezoneWithoutSign = timezone.Replace("+", string.Empty);
                TimeZoneInfo ctz = TimeZoneInfo.CreateCustomTimeZone("cid", TimeSpan.Parse(timezoneWithoutSign), "customzone", "customzone");
                return TimeZoneInfo.ConvertTime(DateTime.UtcNow, ctz);
            }
        }

        public static FormsAuthenticationTicket SetCustomFormsAuthenticationTicket(string userName, bool createPersistentCookie, Page page, string userDate = "",string userData ="")
        {
            string cookiestr;
            HttpCookie ck;
            FormsAuthenticationTicket tkt;
            var formsAuthenticationTimeOutStr = SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Application, Constants.ResourceKeys.FormsAuthenticationTimeOutKey);
            int formsAuthenticationTimeOut;
            if (!string.IsNullOrEmpty(formsAuthenticationTimeOutStr))
            {
                formsAuthenticationTimeOut = int.Parse(formsAuthenticationTimeOutStr);
            }
            else
            {
                formsAuthenticationTimeOut = 60;
            }
            tkt = new FormsAuthenticationTicket(2, userName, DateTime.Now, (DateTime.Now.AddSeconds(3)).AddMinutes(formsAuthenticationTimeOut), createPersistentCookie, userData);
            cookiestr = FormsAuthentication.Encrypt(tkt);
            ck = new HttpCookie(FormsAuthentication.FormsCookieName, cookiestr);
            if (createPersistentCookie)
                ck.Expires = tkt.Expiration;
            ck.Path = FormsAuthentication.FormsCookiePath;
            if ((page.Response.Cookies != null) && (page.Response.Cookies[FormsAuthentication.FormsCookieName] != null))
            {
                page.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
            }
            page.Response.Cookies.Add(ck);
            return tkt;
        }

        public static int[] StringToIntArray(string csv)
        {
            csv = csv.TrimEnd(',');
            var values = csv.Split(',');

            var res = new int[values.Length];

            for (var i = 0; i < values.Length; i++)
            {
                if(!string.IsNullOrEmpty(values[i]))
                    res[i] = Convert.ToInt32(values[i]);
            }

            return res;
        }

        public static string EncodedFileName(string fileName)
        {
            List<char> invalidChars = Path.GetInvalidFileNameChars().ToList();

            invalidChars.Add(';');

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < fileName.Length; i++)
            {
                if (!invalidChars.Any(c => c == fileName[i]))
                {
                    sb.Append(fileName[i]);
                }
            }

            return sb.ToString();
        }
    }
}


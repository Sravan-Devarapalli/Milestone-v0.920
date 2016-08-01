using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataTransferObjects;
using PraticeManagement.Controls;
using System.Web.UI;

namespace PraticeManagement.Utils
{
    public class ReportsFilterHelper
    {

        private static string SessionId
        {
            get
            {
                var session = HttpContext.Current.Request.Cookies["ASP.NET_SessionId"];
                if (session != null)
                {
                    return session.Value;
                }
                return string.Empty;
            }
        }

        public static void SaveFilterValues(ReportName report, object filter)
        {
            if (!string.IsNullOrEmpty(SessionId))
            {
                string filterData = SerializationHelper.SerializeBase64(filter);
                ServiceCallers.Custom.Person(p => p.SaveReportFilterValues(PracticeManagementMain.CurrentUserID, (int)report, filterData, PracticeManagementMain.PreviousUserId, SessionId));
            }
        }

        public static object GetFilterValues(ReportName report)
        {
            if (!string.IsNullOrEmpty(SessionId))
            {
                string filterData = ServiceCallers.Custom.Person(p => p.GetReportFilterValues(PracticeManagementMain.CurrentUserID, (int)report, PracticeManagementMain.PreviousUserId, SessionId));
                if (!string.IsNullOrEmpty(filterData))
                {
                    return SerializationHelper.DeserializeBase64(filterData);
                }
            }
            return null;
        }
    }
}


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
       
        public static void SaveFilterValues(ReportName report, object filter)
        {
           
            string filterData = SerializationHelper.SerializeBase64(filter);
            ServiceCallers.Custom.Person(p => p.SaveReportFilterValues(PracticeManagementMain.CurrentUserID, (int)report, filterData, PracticeManagementMain.PreviousUserId));
        }

        public static object GetFilterValues(ReportName report)
        {
            string filterData = ServiceCallers.Custom.Person(p => p.GetReportFilterValues(PracticeManagementMain.CurrentUserID, (int)report, PracticeManagementMain.PreviousUserId));
            if (!string.IsNullOrEmpty(filterData))
            {
                return SerializationHelper.DeserializeBase64(filterData);
            }
            return null;
        }
    }
}


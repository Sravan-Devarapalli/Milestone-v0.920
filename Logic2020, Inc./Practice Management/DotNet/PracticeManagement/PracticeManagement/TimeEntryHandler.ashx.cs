using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace PraticeManagement
{
    /// <summary>
    /// Summary description for TimeEntryHandler
    /// </summary>
    public class TimeEntryHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            
            string personId = context.Request.QueryString["personId"];
            string accountId = context.Request.QueryString["accountId"];
            string projectId = context.Request.QueryString["projectId"];
            string businessUnitId = context.Request.QueryString["businessUnitId"];
            string timeEntryId = context.Request.QueryString["timeEntryId"];
            string weekStartDate = context.Request.QueryString["weekStartDate"];


            if (
                !String.IsNullOrEmpty(personId) && !String.IsNullOrEmpty(accountId) && !String.IsNullOrEmpty(projectId) && !String.IsNullOrEmpty(businessUnitId) && !String.IsNullOrEmpty(timeEntryId) && !String.IsNullOrEmpty(weekStartDate) &&
                personId != "undefined" && accountId != "undefined" && projectId != "undefined" && businessUnitId != "undefined" && timeEntryId!= "undefined" && weekStartDate != "undefined" 
                )
            {
                    DateTime startDate = Convert.ToDateTime(weekStartDate);
                    DateTime endDate = Convert.ToDateTime(weekStartDate).AddDays(6d);
                    Dictionary<DateTime,bool> isChargeCodeTurnOffList = ServiceCallers.Custom.TimeEntry(p => p.GetIsChargeCodeTurnOffByPeriod(Convert.ToInt32(personId),Convert.ToInt32( accountId), Convert.ToInt32(businessUnitId), Convert.ToInt32(projectId), Convert.ToInt32(timeEntryId), startDate, endDate));
                    var list = isChargeCodeTurnOffList.Select(p => new { Date = p.Key.ToString() , IschargeCodeTurnOff = p.Value.ToString().ToLowerInvariant()}).ToList();
                    string isChargeCodeTurnOffJson = TimeEntryHandler.ToJSON(list);
                    context.Response.Write(isChargeCodeTurnOffJson);
            }
        }

        private static string ToJSON(Object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}

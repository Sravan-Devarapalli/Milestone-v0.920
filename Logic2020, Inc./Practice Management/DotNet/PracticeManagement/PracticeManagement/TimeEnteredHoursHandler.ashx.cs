using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PraticeManagement
{
    /// <summary>
    /// Summary description for TimeEnteredHoursHandler
    /// </summary>
    public class TimeEnteredHoursHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int personId = Convert.ToInt32(context.Request.QueryString["PersonId"]);
            DateTime date = Convert.ToDateTime(context.Request.QueryString["Date"]);
            double? totalHours;
            using (var serviceClient = new TimeEntryService.TimeEntryServiceClient())
            {
                totalHours = serviceClient.GetPersonTimeEnteredHoursByDay(personId, date, false);
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write(totalHours.HasValue ? totalHours.Value : 0);
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

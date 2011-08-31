using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PraticeManagement
{
    /// <summary>
    /// Summary description for OpportunityPriorityHandler
    /// </summary>
    public class OpportunityPriorityHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var opportunityId = Convert.ToInt32(context.Request.QueryString["OpportunityID"]);
            var priorityId = Convert.ToInt32(context.Request.QueryString["PriorityID"]);
            ServiceCallers.Custom.Opportunity(os => os.UpdatePriorityIdForOpportunity(opportunityId, priorityId,context.User.Identity.Name));
            context.Response.Write("Save Completed.");
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

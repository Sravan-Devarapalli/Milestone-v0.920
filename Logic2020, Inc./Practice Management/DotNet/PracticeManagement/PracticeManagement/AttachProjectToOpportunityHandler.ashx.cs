using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataTransferObjects;
using System.Web.Script.Serialization;

namespace PraticeManagement
{
    /// <summary>
    /// Summary description for AttachProjectToOpportunityHandler
    /// </summary>
    public class AttachProjectToOpportunityHandler : IHttpHandler
    {
        //private class clientProject
        //{ 
        //    string 
        //}
        public void ProcessRequest(HttpContext context)
        {
            if (!String.IsNullOrEmpty(context.Request.QueryString["getClientProjects"]))
            {
                var getClientProjects = Convert.ToBoolean(context.Request.QueryString["getClientProjects"]);
                if (getClientProjects)
                {
                    var clientId = Convert.ToInt32(context.Request.QueryString["clientId"]);
                    var projects = ServiceCallers.Custom.Project(client => client.ListProjectsByClientShort(clientId, true, false,false));

                    var SelectedList = projects.Select(p => new { p.Id.Value, p.DetailedProjectTitle, p.Description });

                    string ClientProjects = AttachProjectToOpportunityHandler.ToJSON(SelectedList);
                    context.Response.Write(ClientProjects);
                }
            }
            else
            {
                if (!(String.IsNullOrEmpty(context.Request.QueryString["opportunityID"]) && String.IsNullOrEmpty(context.Request.QueryString["projectId"])))
                {
                    var opportunityId = Convert.ToInt32(context.Request.QueryString["opportunityID"]);
                    var projectId = Convert.ToInt32(context.Request.QueryString["projectId"]);
                    var priorityId = Convert.ToInt32(context.Request.QueryString["priorityId"]);
                    var isOpportunityDescriptionSelected = Convert.ToBoolean(context.Request.QueryString["isOpportunityDescriptionSelected"]);

                    ServiceCallers.Custom.Opportunity(os => os.AttachProjectToOpportunity(opportunityId, projectId, priorityId, context.User.Identity.Name, isOpportunityDescriptionSelected));
                    context.Response.Write("Save Completed.");
                }

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


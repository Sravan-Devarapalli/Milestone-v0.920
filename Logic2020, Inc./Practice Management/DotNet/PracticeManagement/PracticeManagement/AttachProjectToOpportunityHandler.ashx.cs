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
                    var projects = ServiceCallers.Custom.Project(client => client.ListProjectsByClientShort(clientId, true));

                    Dictionary<string, string> values = new Dictionary<string, string>();
                    values.Add("-1", "Select Project ...");
                    foreach (Project project in projects)
                    {
                        values.Add(project.Id.ToString(), project.DetailedProjectTitle);
                    }
                    string ClientProjects =AttachProjectToOpportunityHandler.ToJSON(values);
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
                    ServiceCallers.Custom.Opportunity(os => os.AttachProjectToOpportunity(opportunityId, projectId, priorityId, context.User.Identity.Name));
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

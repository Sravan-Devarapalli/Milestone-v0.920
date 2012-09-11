using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Web.Script.Services;
using System.Web.Services;
using AjaxControlToolkit;
using PraticeManagement.ClientService;
using PraticeManagement.Controls;
using PraticeManagement.MilestoneService;
using PraticeManagement.Configuration;
using System;

namespace PraticeManagement
{
    /// <summary>
    /// 	Summary description for CompanyPerfomanceServ
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [ScriptService]
    [ScriptService]
    public class CompanyPerfomanceServ : WebService
    {
        [WebMethod]
        [ScriptMethod]
        public CascadingDropDownNameValue[] GetDdlProjectGroupContents(string knownCategoryValues, string category, string contextKey)
        {
            int clientId = int.Parse(knownCategoryValues.Split(':')[1].Split(';')[0]);
            var contextKeyArray = contextKey.Split(';');
            int personId = int.Parse(contextKeyArray[0]);
            DateTime startDate = Convert.ToDateTime(contextKeyArray[1]);
            DateTime endDate = Convert.ToDateTime(contextKeyArray[2]);
            var groups = ServiceCallers.Custom.Group(client => client.ListGroupByClientAndPersonInPeriod(clientId, personId, startDate, endDate));
            return groups.Select(group =>
                new CascadingDropDownNameValue(
                            group.Name,
                            group.Id.ToString())).ToArray();
        }
        
        [WebMethod]
        [ScriptMethod]
        public CascadingDropDownNameValue[] GetClients(string knownCategoryValues, string category)
        {
            var res = new List<CascadingDropDownNameValue>();
            using (var serviceClient = new ClientServiceClient())
            {
                try
                {
                    var clients = serviceClient.ClientListAll();
                    foreach (var client in clients)
                        res.Add(new CascadingDropDownNameValue(client.Name, client.Id.ToString()));
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
            return res.ToArray();
        }

        [WebMethod]
        [ScriptMethod]
        public CascadingDropDownNameValue[] GetProjects(string knownCategoryValues, string category, string contextKey)
        {
            var clientId = int.Parse(knownCategoryValues.Split(':')[1].Split(';')[0]);
            var selectedProjectId = contextKey == null ? -1 : int.Parse(contextKey);
            var projects = ServiceCallers.Custom.Project(client => client.ListProjectsByClientShort(clientId, false, false, false));

            return projects.Select(
                project => new CascadingDropDownNameValue(
                                    DataHelper.FormatDetailedProjectName(project),
                                    project.Id.ToString(),
                                    project.Id.Value == selectedProjectId)).ToArray();
        }


        [WebMethod]
        [ScriptMethod]
        public CascadingDropDownNameValue[] GetProjectsListByProjectGroupId(string knownCategoryValues, string category, string contextKey)
        {
            var groupId = int.Parse(knownCategoryValues.Split(':')[1].Split(';')[0]);
            var contextKeyArray = contextKey.Split(';');
            int personId = int.Parse(contextKeyArray[0]);
            DateTime startDate = Convert.ToDateTime(contextKeyArray[1]);
            DateTime endDate = Convert.ToDateTime(contextKeyArray[2]);
            var projects = ServiceCallers.Custom.Project(group => group.GetProjectsListByProjectGroupId(groupId, true, personId, startDate, endDate));

            return projects.Select(
                project => new CascadingDropDownNameValue(project.Name,project.Id.ToString())).ToArray();
        }


        [WebMethod]
        [ScriptMethod]
        public CascadingDropDownNameValue[] GetProjectsList(string knownCategoryValues, string category, string contextKey)
        {
            var clientId = int.Parse(knownCategoryValues.Split(':')[1].Split(';')[0]);
            var contextKeyArray = contextKey.Split(';');
            int personId = int.Parse(contextKeyArray[0]);
            DateTime startDate = Convert.ToDateTime(contextKeyArray[1]);
            DateTime endDate = Convert.ToDateTime(contextKeyArray[2]);
            var projects = ServiceCallers.Custom.Project(client => client.ListProjectsByClientAndPersonInPeriod(clientId, true, true, personId, startDate, endDate));

            var cddlist = projects.Select(
                project => new CascadingDropDownNameValue(project.Name,project.Id.ToString())).ToArray();
            return cddlist;
        }


        [WebMethod]
        [ScriptMethod]
        public CascadingDropDownNameValue[] GetMilestones(string knownCategoryValues, string category)
        {
            var res = new List<CascadingDropDownNameValue>();
            var projectId = int.Parse(knownCategoryValues.Split(';')[1].Split(':')[1]);
            using (var serviceClient = new MilestoneServiceClient())
            {
                try
                {
                    var lowerBound = MileStoneConfigurationManager.GetLowerBound();
                    var upperBound = MileStoneConfigurationManager.GetUpperBound();

                    if (!lowerBound.HasValue)
                    {
                        lowerBound = 180;
                    }
                    if (!upperBound.HasValue)
                    {
                        upperBound = 180;
                    }
                    var milestones = serviceClient.MilestoneListByProject(projectId);
                    var result = milestones.Where(item => item.StartDate <= DateTime.Today.AddDays(-1 * lowerBound.Value) && item.EndDate >= DateTime.Today.AddDays(upperBound.Value));
                    foreach (var milestone in result)
                        res.Add(new CascadingDropDownNameValue(milestone.Description, milestone.Id.ToString()));
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
            return res.ToArray();
        }
    }
}


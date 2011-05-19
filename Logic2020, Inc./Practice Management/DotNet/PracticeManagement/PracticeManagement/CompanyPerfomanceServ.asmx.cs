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
            var clientId = int.Parse(knownCategoryValues.Split(':')[1].Split(';')[0]);
            var selectedGroupId = contextKey == null ? -1 : int.Parse(contextKey);
            var groups = ServiceCallers.Custom.Group(client => client.GroupListAll(clientId, null));
            groups = groups.AsQueryable().Where(g =>( g.IsActive == true || g.Id.ToString() == contextKey)).ToArray();
            return groups.Select(group =>
                new CascadingDropDownNameValue(
                            group.Name,
                            group.Id.ToString(),
                            group.Id.Value == selectedGroupId)).ToArray();
        }

        [WebMethod]
        [ScriptMethod]
        public CascadingDropDownNameValue[] GetPersons(string prefixText, int count)
        {
            var persons =
                ServiceCallers.Custom.Person(
                    client => client.GetPersonListWithCurrentPay(
                            null, true, int.MaxValue, 0, prefixText, null, DataHelper.CurrentPerson.Alias, null, null, false, false, false, null));

            return persons.Select(person =>
                new CascadingDropDownNameValue(
                            person.PersonLastFirstName,
                            person.Id.ToString())).ToArray();
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
            var projects = ServiceCallers.Custom.Project(client => client.ListProjectsByClientShort(clientId, false));

            return projects.Select(
                project => new CascadingDropDownNameValue(
                                    DataHelper.FormatDetailedProjectName(project),
                                    project.Id.ToString(),
                                    project.Id.Value == selectedProjectId)).ToArray();
        }

        [WebMethod]
        [ScriptMethod]
        public CascadingDropDownNameValue[] GetActiveAndProjectedProjects(string knownCategoryValues, string category, string contextKey)
        {
            var clientId = int.Parse(knownCategoryValues.Split(':')[1].Split(';')[0]);
            var selectedProjectId = contextKey == null ? -1 : int.Parse(contextKey);
            var projects = ServiceCallers.Custom.Project(client => client.ListProjectsByClientShort(clientId, true));

            return projects.Select(
                project => new CascadingDropDownNameValue(
                                    DataHelper.FormatDetailedProjectName(project),
                                    project.Id.ToString(),
                                    project.Id.Value == selectedProjectId)).ToArray();
        }

        [WebMethod]
        [ScriptMethod]
        public CascadingDropDownNameValue[] GetAllTimeEntryProjectsByClientId(string knownCategoryValues, string category, string contextKey)
        {
            var clientId = int.Parse(knownCategoryValues.Split(':')[1].Split(';')[0]);
            var selectedProjectId = contextKey == null ? -1 : int.Parse(contextKey);
            var projects = ServiceCallers.Custom.TimeEntry(client => client.GetAllTimeEntryProjects());
            projects = projects.ToList().FindAll(p => p.Client != null && p.Client.Id.HasValue && p.Client.Id.Value == clientId).ToArray();

            return projects.Select(
                project => new CascadingDropDownNameValue(
                                    project.Name + " - " + project.ProjectNumber,
                                    project.Id.ToString(),
                                    project.Id.Value == selectedProjectId)).ToArray();
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


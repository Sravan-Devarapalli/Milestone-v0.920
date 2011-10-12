using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataTransferObjects;
using System.Text;

namespace PraticeManagement.Objects
{
    public class DetailedProjectReportItem
    {
        #region Constants

        private const string NotEndDate = "No End Date Specified.";
        private const string DetailsLabelFormat = "{0} - {1}";
        private const string AppendPersonFormat = "{0} {2}, {1}";
        private const string ToolTipView = " {1}{0}{2}{0}<b>Start:</b>{3:d}{0}<b>End:</b>{4:d}{0}<b>Project Manager(s):</b>{5}{0}<b>Resources:</b>{6}";

        #endregion

        #region ItemType enum

        public enum ItemType
        {
            ActiveProject,
            ProjectedProject
        }

        #endregion

        #region Constructors

        public DetailedProjectReportItem
            (DateTime reportStartDate, DateTime reportEndDate, Project project)
        {
            Project = project;
            ReportStartDate = reportStartDate;
            ReportEndDate = reportEndDate;
        }

        #endregion



        #region Properties

        public Project Project { get; set; }

        public DateTime ReportStartDate { get; set; }

        public DateTime ReportEndDate { get; set; }

        public DateTime StartDate
        {
            get { return Project.StartDate.Value < ReportStartDate ? ReportStartDate : Project.StartDate.Value; }
        }

        public DateTime EndDate
        {
            get
            {
                var endDate = Project.EndDate;

                if (endDate.HasValue)
                    return endDate.Value > ReportEndDate
                               ? ReportEndDate : endDate.Value;

                return ReportEndDate;
            }
        }

        public ItemType BarType
        {
            get
            {
                if (Project != null && Project.Status != null)
                    return Project.Status.StatusType == ProjectStatusType.Projected ? ItemType.ProjectedProject : ItemType.ActiveProject;

                return ItemType.ActiveProject;
            }
        }

        public string Label
        {
            get
            {
                return string.Format(
                    DetailsLabelFormat,
                    Project.Client.Name,
                    Project.Name);
            }
        }

        public string smallLabel
        {
            get
            {
                return string.Format(Project.ProjectNumber);
            }
        }

        public string Tooltip
        {
            get
            {
                return PrepareToolTipView(Project).Replace("'", "&#39;");
            }
        }

        public string NavigateUrl
        {
            get
            {
                return GetRedirectUrl(Project.Id.Value, Constants.ApplicationPages.ProjectDetail); ;
            }
        }


        private static string GetRedirectUrl(int argProjectId, string targetUrl)
        {
            return string.Format(Constants.ApplicationPages.DetailRedirectWithReturnFormat,
                                 targetUrl,
                                 argProjectId,
                                 Constants.ApplicationPages.Projects);
        }

        private static string PrepareToolTipView(Project project)
        {
            var persons = new StringBuilder();
            var personList = new List<MilestonePerson>();
            foreach (var projectPerson in project.ProjectPersons)
            {
                var personExist = false;
                if (personList != null)
                {
                    foreach (var mp in personList)
                        if (mp.Person.Id == projectPerson.Person.Id)
                        {
                            personExist = true;
                            break;
                        }
                }
                if (!personExist)
                {
                    personList.Add(projectPerson);
                }
            }
            for (int i = 0; i < personList.Count; i++)
            {

                persons.AppendFormat(AppendPersonFormat,
                                     "<br />",
                                     HttpUtility.HtmlEncode(personList[i].Person.FirstName),
                                     HttpUtility.HtmlEncode(personList[i].Person.LastName));
            }

            return string.Format(ToolTipView,
                "<br />",
                HttpUtility.HtmlEncode(project.Client.Name),
                HttpUtility.HtmlEncode(project.Name),
                project.StartDate.HasValue ? project.StartDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                project.EndDate.HasValue ? project.EndDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                GetProjectManagers(project.ProjectManagers),
                persons
                );
        }


        private static string GetProjectManagers(List<Person> list)
        {
            string names = string.Empty;
            foreach (var person in list)
            {
                names +=  "<br />" + person.Name ;
            }

            return names;
        }

        #endregion


    }
}


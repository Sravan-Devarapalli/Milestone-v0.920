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

        private const string ProjectOwnerFormat = "{0} {1}";
        private const string NotEndDate = "No End Date Specified.";
        private const string DetailsLabelFormat = "{0} - {1}";
        private const string AppendPersonFormat = "{0} {2} {1},";
        private const string AppendLastPersonFormat = "{0} {2} {1}";
        private const string ToolTipView = " {1}, {0} {2}, {0} Start: {3:d}, {0} End: {4:d}, {0} Owner: {5},{0} Resources: {6}";

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
                return PrepareToolTipView(Project);
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
                if (i == personList.Count - 1)
                {
                    persons.AppendFormat(AppendLastPersonFormat,
                                         Environment.NewLine,
                                         HttpUtility.HtmlEncode(personList[i].Person.FirstName),
                                         HttpUtility.HtmlEncode(personList[i].Person.LastName));
                }
                else
                {
                    persons.AppendFormat(AppendPersonFormat,
                                        Environment.NewLine,
                                        HttpUtility.HtmlEncode(personList[i].Person.FirstName),
                                        HttpUtility.HtmlEncode(personList[i].Person.LastName));
                }
            }

            return string.Format(ToolTipView,
                Environment.NewLine,
                HttpUtility.HtmlEncode(project.Client.Name),
                HttpUtility.HtmlEncode(project.Name),
                project.StartDate,
                project.EndDate,
                HttpUtility.HtmlEncode(string.Format(ProjectOwnerFormat, project.ProjectManager.LastName, project.ProjectManager.FirstName)),
                persons
                );
        }

        #endregion


    }
}

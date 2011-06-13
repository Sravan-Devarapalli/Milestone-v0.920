using System;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using PraticeManagement.Controls;
using PraticeManagement.ProjectService;
using DataTransferObjects;
using System.Linq;
using System.Collections.Generic;
using PraticeManagement.Utils;

namespace PraticeManagement
{
    public partial class ProjectSearch : PracticeManagementPageBase
    {
        private const string RegexSpecialChars = ".[]()\\-*%+|$?^";

        private string RegexReplace
        {
            get;
            set;
        }

        protected void Project_Command(object sender, CommandEventArgs e)
        {
            RedirectWithBack(
                string.Format(
                Constants.ApplicationPages.DetailRedirectFormat,
                Constants.ApplicationPages.ProjectDetail,
                e.CommandArgument),
                Constants.ApplicationPages.Projects);
        }

        protected void btnClientName_Command(object sender, CommandEventArgs e)
        {
            RedirectWithBack(
                string.Format(
                    Constants.ApplicationPages.DetailRedirectFormat,
                    Constants.ApplicationPages.ClientDetails,
                    e.CommandArgument),
                Constants.ApplicationPages.Projects);
        }

        protected void btnMilestoneName_Command(object sender, CommandEventArgs e)
        {
            string[] parts = e.CommandArgument.ToString().Split('_');

            if (parts.Length >= 2)
            {
                RedirectWithBack(string.Concat(
                    string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                        Constants.ApplicationPages.MilestoneDetail,
                        parts[0]), "&projectId=", parts[1]),
                     Constants.ApplicationPages.Projects);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DisplaySearch();
        }

        /// <summary>
        /// Hightlight the text searched.
        /// </summary>
        /// <param name="result">The found value.</param>
        /// <returns>The text with searched substring highlighted.</returns>
        protected string HighlightFound(object result)
        {
            if (string.IsNullOrEmpty(RegexReplace))
            {
                RegexReplace = txtSearchText.Text;

                foreach (char specialChar in RegexSpecialChars)
                {
                    RegexReplace =
                        RegexReplace.Replace(specialChar.ToString(), string.Format("\\{0}", specialChar));
                }

                RegexReplace = RegexReplace.Replace(" ", "\\s");
            }

            string strResult;

            if (result != null)
            {
                strResult =
                    Regex.Replace(
                    result.ToString(),
                    RegexReplace,
                    Constants.Formatting.SearchResultFormat,
                    RegexOptions.IgnoreCase);
            }
            else
            {
                strResult = string.Empty;
            }

            return strResult;
        }

        protected override void Display()
        {
            if (PreviousPage != null)
            {
                txtSearchText.Text = PreviousPage.SearchText;
                DisplaySearch();
            }
        }

        private void DisplaySearch()
        {
            Page.Validate();
            if (Page.IsValid)
            {
                using (var serviceClient = new ProjectServiceClient())
                {
                    try
                    {
                        var projects =
                            serviceClient.ProjectSearchText(
                                txtSearchText.Text,
                                DataHelper.CurrentPerson.Id.Value);

                        IEnumerable<int> ProjectIds = projects.ToList().FindAll(p => p.Id.HasValue).Select(q => q.Id.Value).Distinct();
                        var groupedProjects = new List<Project>();
                        foreach (var ProjectId in ProjectIds)
                        {
                            var project = projects.First(p => p.Id.HasValue && p.Id.Value == ProjectId);
                            var milestones = new List<Milestone>();
                            foreach (var tempProject in projects.ToList().FindAll(p => p.Id.HasValue && p.Id.Value == ProjectId))
                            {
                                if (tempProject.Milestones != null && tempProject.Milestones.Any())
                                {
                                    milestones.AddRange(tempProject.Milestones);
                                }
                            }
                            project.Milestones = milestones;
                            groupedProjects.Add(project);
                        }

                        lvProjects.DataSource = groupedProjects;
                        lvProjects.DataBind();
                    }
                    catch (CommunicationException)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
            }
        }

        protected void lvProjects_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var panel = e.Item.FindControl("pnlMilestones");
                var project = (e.Item as ListViewDataItem).DataItem as Project;
                var collapsiblepanelextender = e.Item.FindControl("cpe") as AjaxControlToolkit.CollapsiblePanelExtender;

                if (project.Milestones != null && project.Milestones.Any())
                {
                    if (panel != null)
                    {
                        var datalist = panel.FindControl("dtlProposedPersons") as DataList;
                        if (datalist != null)
                        {
                            foreach (var milestone in project.Milestones)
                            {
                                milestone.Project = project;
                            }
                            datalist.DataSource = project.Milestones;
                            datalist.DataBind();
                            hdnExpandCollapseExtendersIds.Value += "," + collapsiblepanelextender.ClientID;
                        }
                    }
                }
                else
                {
                    var btnExpandCollapseMilestones = e.Item.FindControl("btnExpandCollapseMilestones") as Image;
                    collapsiblepanelextender.Enabled = btnExpandCollapseMilestones.Visible = false;
                    var btnProjectName = e.Item.FindControl("btnProjectName") as LinkButton;
                    btnProjectName.Style.Add("padding-left", "15px");
                }
            }
        }

        protected string GetProjectNameCellCssClass(Project project)
        {
            string cssClass = ProjectHelper.GetIndicatorClassByStatusId(project.Status.Id);
            if (project.Status.Id == 3 && project.Attachment == null)
            {
                cssClass = "ActiveProjectWithoutSOW";
            }

            return cssClass;
        }
    }
}


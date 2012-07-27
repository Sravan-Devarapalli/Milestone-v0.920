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
using System.Web.UI.HtmlControls;

namespace PraticeManagement
{
    public partial class ProjectSearch : PracticeManagementPageBase
    {
        private const int ProjectStateColumnIndex = 0;
        private const string RegexSpecialChars = ".[]()\\-*%+|$?^";
        private const string PaddingClassForProjectName = "padLeft15Imp";
        private const string CursorPointerClass = " CursorPointer";

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
                        string searchText = txtSearchText.Text;
                        var projects =
                            serviceClient.ProjectSearchText(
                                searchText,
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

                        if (IsTextProjectNumberFormat(searchText) && groupedProjects.Count == 1)
                        {
                            Project project = groupedProjects.First();
                            RedirectWithBack(string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                                            Constants.ApplicationPages.ProjectDetail,
                                                            project.Id),
                                            Constants.ApplicationPages.Projects);
                        }
                        else
                        {
                            lvProjects.DataSource = groupedProjects;
                            lvProjects.DataBind();
                        }
                    }
                    catch (CommunicationException)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
            }
        }

        private bool IsTextProjectNumberFormat(string searchText)
        {
            searchText = searchText.ToUpper().Trim();
            int projectNumber = 0;
            bool isprojectFormat = false;
            if (searchText.StartsWith("P") && int.TryParse(searchText.Substring(1), out projectNumber))
            {
                isprojectFormat = true;
            }

            return isprojectFormat;
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
                    btnProjectName.Attributes["class"] = PaddingClassForProjectName;                   
                }
                string cssClass = ProjectHelper.GetIndicatorClassByStatusId(project.Status.Id);
                if (project.Status.Id == 3 && !project.HasAttachments)
                {
                    cssClass = "ActiveProjectWithoutSOW";
                }
                var htmlRow = e.Item.FindControl("boundingRow") as HtmlTableRow;
                FillProjectStateCell(htmlRow, cssClass, project.Status);
            }
        }

        private void FillProjectStateCell(HtmlTableRow row, string cssClass, ProjectStatus status)
        {
            var toolTip = status.Name;

            if (status.Id == (int)ProjectStatusType.Active)
            {
                if (cssClass == "ActiveProjectWithoutSOW")
                {
                    toolTip = "Active without Attachment";
                }
                else
                {
                    toolTip = "Active with Attachment";
                }
            }

            HtmlAnchor anchor = new HtmlAnchor();
            anchor.Attributes["class"] = cssClass + CursorPointerClass;
            anchor.Attributes["Description"] = toolTip;
            anchor.Attributes["onmouseout"] = "HidePanel();";
            anchor.Attributes["onmouseover"] = "SetTooltipText(this.attributes['Description'].value,this);";
            row.Cells[ProjectStateColumnIndex].Controls.Add(anchor);
        }

        protected string GetProjectNameCellToolTip(Project project)
        {
            string cssClass = GetProjectNameCellCssClass(project);

            var statusName = project.Status.Name;

            if (project.Status.Id == (int)ProjectStatusType.Active)
            {
                if (cssClass == "ActiveProjectWithoutSOW")
                {
                    statusName = "Active without Attachment.";
                }
                else
                {
                    statusName = "Active with Attachment.";
                }
            }


            return statusName;
        }

        protected string GetProjectNameCellCssClass(Project project)
        {
            string cssClass = ProjectHelper.GetIndicatorClassByStatusId(project.Status.Id);
            if (project.Status.Id == 3 && !project.HasAttachments)
            {
                cssClass = "ActiveProjectWithoutSOW";
            }

            return cssClass;
        }
    }
}


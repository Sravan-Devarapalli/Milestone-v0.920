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
using System.Web.Security;

namespace PraticeManagement
{
    public partial class ProjectSearch : PracticeManagementPageBase
    {
        private const int ProjectStateColumnIndex = 0;
        private const string RegexSpecialChars = ".[]()\\-*%+|$?^";
        private const string PaddingClassForProjectName = "padLeft15Imp";
        private const string CursorPointerClass = " CursorPointer";

        #region Fields

        private bool userIsAdministrator;

        #endregion Fields

        #region Properties

        private string SelectedClientIds
        {
            get
            {
                return cblClient.SelectedItems;
            }
            set
            {
                cblClient.SelectedItems = value;
            }
        }

        private string SelectedSalespersonIds
        {
            get
            {
                return cblSalesperson.SelectedItems;
            }
            set
            {
                cblSalesperson.SelectedItems = value;
            }
        }

        private string SelectedPracticeIds
        {
            get
            {
                return cblPractice.SelectedItems;
            }
            set
            {
                cblPractice.SelectedItems = value;
            }
        }

        private string SelectedDivisionIds
        {
            get
            {
                return cblDivision.SelectedItems;
            }
            set
            {
                cblDivision.SelectedItems = value;
            }
        }

        private string SelectedChannelIds
        {
            get
            {
                return cblChannel.SelectedItems;
            }
            set
            {
                cblChannel.SelectedItems = value;
            }
        }

        private string SelectedRevenueTypeIds
        {
            get
            {
                return cblRevenueType.SelectedItems;
            }
            set
            {
                cblRevenueType.SelectedItems = value;
            }
        }

        private string SelectedOfferingIds
        {
            get
            {
                return cblOffering.SelectedItems;
            }
            set
            {
                cblOffering.SelectedItems = value;
            }
        }

        private string SelectedGroupIds
        {
            get
            {
                return cblProjectGroup.SelectedItems;
            }
            set
            {
                cblProjectGroup.SelectedItems = value;
            }
        }

        private string SelectedProjectOwnerIds
        {
            get
            {
                return cblProjectOwner.SelectedItems;
            }
            set
            {
                cblProjectOwner.SelectedItems = value;
            }
        }

        private bool ShowActive
        {
            get
            {
                return chbActive.Checked;
            }
            set
            {
                chbActive.Checked = value;
            }
        }

        private bool ShowAtRisk
        {
            get
            {
                return chbAtRisk.Checked;
            }
            set
            {
                chbAtRisk.Checked = value;
            }
        }

        private bool ShowInternal
        {
            get
            {
                return chbInternal.Checked;
            }
            set
            {
                chbInternal.Checked = value;
            }
        }

        private bool ShowProposed
        {
            get
            {
                return chbProposed.Checked;
            }
            set
            {
                chbProposed.Checked = value;
            }
        }

        private bool ShowProjected
        {
            get
            {
                return chbProjected.Checked;
            }
            set
            {
                chbProjected.Checked = value;
            }
        }

        private bool ShowInactive
        {
            get
            {
                return chbInactive.Checked;
            }
            set
            {
                chbInactive.Checked = value;
            }
        }

        private bool ShowCompleted
        {
            get
            {
                return chbCompleted.Checked;
            }
            set
            {
                chbCompleted.Checked = value;
            }
        }

        private bool ShowExperimental
        {
            get
            {
                return chbExperimental.Checked;
            }
            set
            {
                chbExperimental.Checked = value;
            }
        }

        #endregion Properties

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
                CompanyPerformanceFilterSettings filters = Session["Filters"] as CompanyPerformanceFilterSettings;
                if (filters == null)
                {
                    filters = new CompanyPerformanceFilterSettings();
                    filters.ClientIdsList = filters.ProjectGroupIdsList = filters.SalespersonIdsList = filters.PracticeIdsList =
                        filters.DivisionIdsList = filters.ProjectOwnerIdsList = filters.OfferingIdsList = filters.RevenueTypeIdsList =
                        filters.ChannelIdsList = null;
                    filters.ShowActive = filters.ShowCompleted = filters.ShowProjected = filters.ShowInternal = filters.ShowProposed = true;
                    filters.ShowExperimental = filters.ShowInactive = false;
                }
                SelectedClientIds = filters.ClientIdsList;
                SelectedGroupIds = filters.ProjectGroupIdsList;
                SelectedSalespersonIds = filters.SalespersonIdsList;
                SelectedPracticeIds = filters.PracticeIdsList;
                SelectedDivisionIds = filters.DivisionIdsList;
                SelectedProjectOwnerIds = filters.ProjectOwnerIdsList;
                SelectedOfferingIds = filters.OfferingIdsList;
                SelectedRevenueTypeIds = filters.RevenueTypeIdsList;
                SelectedChannelIds = filters.ChannelIdsList;
                ShowActive = filters.ShowActive;
                ShowCompleted = filters.ShowCompleted;
                ShowAtRisk = filters.ShowAtRisk;
                ShowExperimental = filters.ShowExperimental;
                ShowInactive = filters.ShowInactive;
                ShowInternal = filters.ShowInternal;
                ShowProjected = filters.ShowProjected;
                ShowProposed = filters.ShowProposed;
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
                        string searchText = string.IsNullOrEmpty(txtSearchText.Text) ? null : txtSearchText.Text;
                        var projects =
                            serviceClient.ProjectSearchText(
                                searchText,
                                DataHelper.CurrentPerson.Id.Value,
                                SelectedClientIds,
                                ShowProjected,
                                ShowCompleted,
                                ShowActive,
                                ShowInternal,
                                ShowExperimental,
                                ShowProposed,
                                ShowInactive,
                                ShowAtRisk,
                                SelectedSalespersonIds,
                                SelectedProjectOwnerIds,
                                SelectedPracticeIds,
                                SelectedDivisionIds,
                                SelectedChannelIds,
                                SelectedRevenueTypeIds,
                                SelectedOfferingIds,
                                SelectedGroupIds
                                );

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
            bool isprojectFormat = false;
            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.ToUpper().Trim();
                int projectNumber = 0;

                if (searchText.StartsWith("P") && int.TryParse(searchText.Substring(1), out projectNumber))
                {
                    isprojectFormat = true;
                }
            }

            return isprojectFormat;
        }

        protected void lvProjects_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var panel = e.Item.FindControl("pnlMilestones");
                var project = (e.Item as ListViewDataItem).DataItem as Project;


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

                        }
                    }
                }
                else
                {
                    var btnExpandCollapseMilestones = e.Item.FindControl("btnExpandCollapseMilestones") as Image;

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

        protected void Page_Load(object sender, EventArgs e)
        {
            userIsAdministrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);

            if (!IsPostBack)
            {
                PreparePeriodView();
            }
        }

        private void PreparePeriodView()
        {

            //  If current user is administrator, don't apply restrictions
            var person =
                (Roles.IsUserInRole(
                    DataHelper.CurrentPerson.Alias,
                    DataTransferObjects.Constants.RoleNames.AdministratorRoleName)
                    || Roles.IsUserInRole(
                    DataHelper.CurrentPerson.Alias,
                    DataTransferObjects.Constants.RoleNames.OperationsRoleName))
                ? null : DataHelper.CurrentPerson;

            //If person is not administrator, return list of values when [All] is selected
            //     this is needed because we apply restrictions and don't want
            //     NULL to be returned, because that would mean all and restrictions
            //     are not going to be applied
            if (person != null)
            {
                cblSalesperson.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                cblProjectOwner.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                cblPractice.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                cblClient.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
                cblProjectGroup.AllSelectedReturnType = ScrollingDropDown.AllSelectedType.AllItems;
            }

            PraticeManagement.Controls.DataHelper.FillSalespersonList(
                person, cblSalesperson,
                Resources.Controls.AllSalespersonsText,
                true);
            SelectAllItems(cblSalesperson);

            PraticeManagement.Controls.DataHelper.FillProjectOwnerList(cblProjectOwner,
                "All People with Project Access",
                true,
                person, true);
            SelectAllItems(cblProjectOwner);
            PraticeManagement.Controls.DataHelper.FillPracticeList(
                person,
                cblPractice,
                Resources.Controls.AllPracticesText);
            SelectAllItems(cblPractice);

            PraticeManagement.Controls.DataHelper.FillClientsAndGroups(
                cblClient, cblProjectGroup);
            SelectAllItems(cblClient);
            SelectAllItems(cblProjectGroup);

            DataHelper.FillProjectDivisionList(cblDivision, false, true);
            SelectAllItems(cblDivision);

            DataHelper.FillChannelList(cblChannel, "All Channels");
            SelectAllItems(cblChannel);

            DataHelper.FillOfferingsList(cblOffering, "All Offerings");
            SelectAllItems(cblOffering);

            DataHelper.FillRevenueTypeList(cblRevenueType, "All Revenue Types");
            SelectAllItems(cblRevenueType);




        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Security
            if (!userIsAdministrator)
            {
                Person current = DataHelper.CurrentPerson;
                int? personId = current != null ? current.Id : null;

                bool userIsSalesperson =
                    Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName);
                bool userIsPracticeManager =
                    Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName);
                bool userIsBusinessUnitManager =
                    Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.BusinessUnitManagerRoleName);

                bool userIsDirector =
                    Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName);
                bool userIsSeniorLeadership =
               Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SeniorLeadershipRoleName);// #2913: userIsSeniorLeadership is added as per the requirement.

                SelectItemInControl(userIsSalesperson, cblSalesperson, personId);
                SelectItemInControl((userIsPracticeManager || userIsBusinessUnitManager || userIsDirector || userIsSeniorLeadership), cblProjectOwner, personId);// #2817: userIsDirector is added as per the requirement.

                bool userIsProjectLead =
                    Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.ProjectLead);
            }

        }

        /// <summary>
        /// Selects item in list control according to some condition
        /// </summary>
        /// <param name="condition">If true, the item will be selected</param>
        /// <param name="targetControl">Control to select item in</param>
        /// <param name="lookFor">Value to look for in the control</param>
        private static void SelectItemInControl(bool condition, ListControl targetControl, int? lookFor)
        {
            if (condition)
            {
                ListItem targetItem = targetControl.Items.FindByValue(lookFor == null ? null : lookFor.ToString());
                if (targetItem != null)
                    targetItem.Selected = true;
            }
        }

        private void SelectAllItems(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Selected = true;
            }
        }

    }
}


﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
using PraticeManagement.ClientService;
using PraticeManagement.Controls;
using PraticeManagement.Controls.Projects;
using PraticeManagement.ProjectGroupService;
using PraticeManagement.ProjectService;
using PraticeManagement.Utils;
using Resources;
using ProjectAttachment = PraticeManagement.AttachmentService.ProjectAttachment;
using ProjectCSAT = PraticeManagement.Controls.Projects.ProjectCSAT;

namespace PraticeManagement
{
    public partial class ProjectDetail : PracticeManagementPageBase, IPostBackEventHandler
    {
        #region Constants

        private const string MailToSubjectFormat = "mailto:{0}?subject={1}";
        private const string ProjectIdFormat = "projectId={0}";
        private const string ProjectKey = "Project";
        private const string ProjectAttachmentHandlerUrl = "~/Controls/Projects/ProjectAttachmentHandler.ashx?ProjectId={0}&FileName={1}&AttachmentId={2}";
        private const string AttachSOWMessage = "File should be in PDF, Word format, Excel, PowerPoint, MS Project, Visio, Exchange, OneNote, ZIP or RAR and should be no larger than {0} KB.";
        private const string ProjectAttachmentsKey = "ProjectAttachmentsKey";
        public const string AllTimeTypesKey = "AllTimeTypesKey";
        public const string ProjectTimeTypesKey = "ProjectTimeTypesKey";
        public const string IsInternalChangeErrorMessage = "Can not change project status as some work types are already in use.";
        public const string OpportunityLinkedTextFormat = "This project is linked to Opportunity {0}.";
        public const string ViewStateLoggedInPerson = "ViewStateLoggedInPerson";

        #endregion Constants

        #region Properties

        /// <summary>
        /// Gets or sets a project is currently dislplayed
        /// </summary>
        public Project Project
        {
            get
            {
                return ViewState[ProjectKey] as Project;
            }
            set
            {
                ViewState[ProjectKey] = value;
            }
        }

        public string LoggedInPersonFirstLastName
        {
            get
            {
                if (ViewState[ViewStateLoggedInPerson] == null)
                {
                    ViewState[ViewStateLoggedInPerson] = ServiceCallers.Custom.Person(p => p.GetPersonByAlias(User.Identity.Name)).PersonFirstLastName;
                }
                return (string)ViewState[ViewStateLoggedInPerson];
            }
        }

        public Client InternalClient
        {
            get
            {
                if (ViewState["InternalClient"] != null)
                    return ViewState["InternalClient"] as Client;
                var client = ServiceCallers.Custom.Client(c => c.GetInternalAccount());
                ViewState["InternalClient"] = client;
                return client;
            }
        }

        public int? ProjectId
        {
            get
            {
                if (SelectedId.HasValue)
                {
                    hdnProjectId.Value = SelectedId.Value.ToString();
                    return SelectedId;
                }
                if (!string.IsNullOrEmpty(hdnProjectId.Value))
                {
                    int projectid;
                    if (Int32.TryParse(hdnProjectId.Value, out projectid))
                    {
                        return projectid;
                    }
                }
                return null;
            }
            set
            {
                hdnProjectId.Value = value.ToString();
            }
        }

        public string CSAT
        {
            get
            {
                return Request.QueryString["CSAT"];
            }
        }

        public string Feedback
        {
            get
            {
                return Request.QueryString["Feedback"];
            }
        }

        private List<ProjectAttachment> AttachmentsForNewProject
        {
            get
            {
                return ViewState[ProjectAttachmentsKey] as List<ProjectAttachment>;
            }
            set
            {
                ViewState[ProjectAttachmentsKey] = value;
            }
        }

        private bool? IsUserHasPermissionOnProject
        {
            get
            {
                if (ProjectId.HasValue)
                {
                    if (ViewState["HasPermission"] == null)
                    {
                        ViewState["HasPermission"] = DataHelper.IsUserHasPermissionOnProject(User.Identity.Name, ProjectId.Value);
                    }
                    return (bool)ViewState["HasPermission"];
                }

                return null;
            }
        }

        public int SelectedDirector
        {
            get
            {
                if (ddlDirector.SelectedValue == "")
                {
                    return -1;
                }
                return Convert.ToInt32(ddlDirector.SelectedValue);
            }
        }

        public int SelectedStatus
        {
            get
            {
                if (ddlProjectStatus.SelectedValue == "")
                {
                    return -1;
                }
                return Convert.ToInt32(ddlProjectStatus.SelectedValue);
            }
        }

        //return true if the user is "project manager" or "project owner" of the project
        private bool? IsUserisOwnerOfProject
        {
            get
            {
                if (ProjectId.HasValue)
                {
                    if (ViewState["IsOwnerOfProject"] == null)
                    {
                        ViewState["IsOwnerOfProject"] = DataHelper.IsUserIsOwnerOfProject(User.Identity.Name, ProjectId.Value, true);
                    }
                    return (bool)ViewState["IsOwnerOfProject"];
                }

                return null;
            }
        }

        //return true if the user is "project owner" of the project
        private bool? IsUserIsProjectOwner
        {
            get
            {
                if (ProjectId.HasValue)
                {
                    if (ViewState["IsProjectOwner"] == null)
                    {
                        ViewState["IsProjectOwner"] = DataHelper.IsUserIsProjectOwner(User.Identity.Name, ProjectId.Value);
                    }
                    return (bool)ViewState["IsProjectOwner"];
                }

                return null;
            }
        }

        private int tblProjectDetailTabViewSwitch_ActiveViewIndex
        {
            get
            {
                if (ViewState["tblProjectDetailTabViewSwitch_ActiveViewIndex"] == null)
                {
                    return 0;
                }
                return (int)ViewState["tblProjectDetailTabViewSwitch_ActiveViewIndex"];
            }
            set
            {
                ViewState["tblProjectDetailTabViewSwitch_ActiveViewIndex"] = value;
            }
        }

        private bool HasAttachments
        {
            get
            {
                if (Project != null)
                {
                    if (Project.Attachments != null && Project.Attachments.Count > 0)
                    {
                        return true;
                    }
                }
                else
                {
                    if (AttachmentsForNewProject != null && AttachmentsForNewProject.Count > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public MessageLabel mlConfirmationControl { get { return mlConfirmation; } }

        public bool CSATTabEditPermission
        {
            get
            {
                return Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) || Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SeniorLeadershipRoleName);
            }
        }

        public bool noMileStones
        {
            get
            {
                if (ViewState["noMileStones_Key"] == null)
                {
                    ViewState["noMileStones_Key"] = false;
                }
                return (bool)ViewState["noMileStones_Key"];
            }
            set
            {
                ViewState["noMileStones_Key"] = value;
            }
        }

        public bool CompletedStatus
        {
            get
            {
                if (ViewState["CompletedStatus_Key"] == null)
                {
                    ViewState["CompletedStatus_Key"] = false;
                }
                return (bool)ViewState["CompletedStatus_Key"];
            }
            set
            {
                ViewState["CompletedStatus_Key"] = value;
            }
        }

        #endregion Properties

        public bool IsErrorPanelDisplay;
        public bool IsOtherPanelDisplay;
        private bool FromSaveButtonClick;

        #region Methods

        protected void custSowBudget_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = true;
            if (!string.IsNullOrEmpty(txtSowBudget.Text))
            {
                Decimal result;
                if (!Decimal.TryParse(txtSowBudget.Text, out result))
                {
                    e.IsValid = false;
                }
            }
        }

        public void ClearDirtyForChildControls()
        {
            ClearDirty();
        }

        protected void cvOpportunityRequired_Validate(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = ddlOpportunities.SelectedIndex != 0;
        }

        protected void cvClientOpportunityLinked_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (Project != null && Project.Client.Id.HasValue && Project.OpportunityId.HasValue && Project.Client.Id.Value.ToString() != ddlClientName.SelectedValue)
            {
                args.IsValid = false;
            }
        }

        protected void cvProjectManager_OnServerValidate(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = cblProjectManagers.SelectedValues != null && cblProjectManagers.SelectedValues.Count > 0;
        }

        protected void cvCapabilities_OnServerValidate(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = cblPracticeCapabilities.SelectedValues != null && cblPracticeCapabilities.SelectedValues.Count > 0;
        }

        protected void cvProjectOwner_OnServerValidate(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            int ownerId;
            if (int.TryParse(ddlProjectOwner.SelectedValue, out ownerId))
            {
                Person owner = ServiceCallers.Custom.Person(p => p.GetPersonDetailsShort(ownerId));
                PersonStatusType status = PersonStatus.ToStatusType(owner.Status.Id);
                if (status == PersonStatusType.Terminated || status == PersonStatusType.Inactive)
                {
                    args.IsValid = false;
                }
            }
        }

        protected void cvProjectManagerStatus_OnServerValidate(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = true;

            if (cblProjectManagers.SelectedItems != null)
            {
                string projectManagerIds = cblProjectManagers.SelectedItems;
                List<Person> projectManagers = ServiceCallers.Custom.Person(p => p.GetPersonListByPersonIdList(projectManagerIds)).ToList();
                foreach (Person manger in projectManagers)
                {
                    PersonStatusType status = PersonStatus.ToStatusType(manger.Status.Id);
                    if (status == PersonStatusType.Terminated || status == PersonStatusType.Inactive)
                    {
                        args.IsValid = false;
                    }
                }
            }
        }

        protected void cvProjectName_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (ProjectId.HasValue)
            {
                if (string.IsNullOrEmpty(lblProjectName.Text.Trim()))
                {
                    args.IsValid = false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(txtProjectNameFirstTime.Text.Trim()))
                {
                    args.IsValid = false;
                }
            }
        }

        protected void cvCloneStatus_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (((Project.Milestones == null || Project.Milestones.Count == 0) && ddlCloneProjectStatus.SelectedValue == "3") || (chbCloneMilestones.Checked == false && ddlCloneProjectStatus.SelectedValue == "3"))
            {
                args.IsValid = false;
                IsOtherPanelDisplay = true;
            }
        }

        protected void cvBNAllowSpace_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var isValid = valregBuyerName.IsValid;
            if (isValid)
            {
                var inputString = txtBuyerName.Text;
                var spacesRemovedInputString = inputString.Replace(" ", "");
                args.IsValid = ((inputString.Length - spacesRemovedInputString.Length) < 2);
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            bool userIsAdministrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            if (!IsPostBack)
            {
                bool userIsSalesPerson = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName);
                bool userIsPracticeManager = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName);
                bool userIsBusinessUnitManager = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.BusinessUnitManagerRoleName);
                bool userIsDirector = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName);// #2817: DirectorRoleName is added as per the requirement.
                bool userIsSeniorLeadership = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SeniorLeadershipRoleName);// #2913: userIsSeniorLeadership is added as per the requirement.
                bool userIsProjectLead = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.ProjectLead);

                if (ProjectId.HasValue && !userIsAdministrator)
                {
                    if (IsUserHasPermissionOnProject.HasValue && !IsUserHasPermissionOnProject.Value
                        && IsUserisOwnerOfProject.HasValue && !IsUserisOwnerOfProject.Value)
                    {
                        Response.Redirect(Constants.ApplicationPages.AccessDeniedPage);
                    }
                }

                if (!ProjectId.HasValue &&
                    userIsProjectLead && !(userIsAdministrator || userIsSalesPerson || userIsPracticeManager || userIsBusinessUnitManager || userIsDirector || userIsSeniorLeadership)
                    )
                {
                    Response.Redirect(Constants.ApplicationPages.AccessDeniedPage);
                }

                ddlNotes.Enabled = (userIsAdministrator || userIsDirector || (IsUserIsProjectOwner.HasValue && IsUserIsProjectOwner.Value));

                txtProjectName.Focus();

                ShowTabs();

                int size = Convert.ToInt32(SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Project, Constants.ResourceKeys.AttachmentFileSize));

                lblAttachmentMessage.Text = string.Format(AttachSOWMessage, size / 1000);
                DataHelper.FillAttachemntCategoryList(ddlAttachmentCategory);
                ddlCloneProjectStatus.SelectedValue = ((int)ProjectStatusType.Experimental).ToString();
            }
            mlConfirmation.ClearMessage();

            btnUpload.Attributes["onclick"] = "startUpload(); return false;";

            ddlCSATOwner.Enabled = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);

            if (!userIsAdministrator)
            {
                lblOppLinking.Style["display"] = "none";
                imgLink.Style["display"] = "none";
                lbOpportunity.Style["display"] = "none";
                imgUnlink.Style["display"] = "none";
                imgNavigateToOpp.Style["display"] = "none";
            }
        }

        public void ShowTabs()
        {
            if (ProjectId.HasValue)
            {
                TableCellHistoryg.Visible = true;
                cellProjectTools.Visible = true;
                cellProjectCSAT.Visible = true;
                cellCommissions.Visible = true;
                cellFeedback.Visible = true;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (ProjectId.HasValue && Project != null && Project.Milestones != null && Project.Milestones.Count > 0)
            {
                cellExpenses.Visible = true;
                cellCommissions.Visible = DataHelper.CurrentPerson.Title.TitleName == "Senior Manager" || Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) || Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SeniorLeadershipRoleName);
            }
            else
            {
                cellExpenses.Visible = false;
                cellCommissions.Visible = false;
            }
            if (ddlProjectGroup.Items.Count > 0)
                ddlProjectGroup.SortByText();
            AddStatusIcon();
            PopulateOpportunityLink();
            txtProjectNameFirstTime.Visible = !(ProjectId.HasValue && Project != null);
            imgEditProjectName.Visible =
            imgMailToCSATOwner.Visible =
            imgMailToSeniorManager.Visible =
            imgMailToProjectOwner.Visible =
            imgMailToClientDirector.Visible =
            imgMailToSalesperson.Visible = !txtProjectNameFirstTime.Visible;

            if (!IsPostBack && !string.IsNullOrEmpty(CSAT) && CSAT == "true" && cellProjectCSAT.Visible)
            {
                btnView_Command(btnProjectCSAT, new CommandEventArgs("", "10"));
            }
            if (!IsPostBack && !string.IsNullOrEmpty(Feedback) && Feedback == "true" && cellFeedback.Visible)
            {
                btnView_Command(btnFeedback, new CommandEventArgs("", "1"));
            }
        }

        /// <summary>
        /// Emits a JavaScript which prevent the loss of non-saved data.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            #region Security

            if (string.IsNullOrEmpty(ddlClientName.SelectedValue))
            {
                SetDefaultsToClientDependancyControls();
            }

            bool userIsAdministrator =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            bool userIsSalesPerson =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName);
            bool userIsPracticeManager =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName);
            bool userIsBusinessUnitManager = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.BusinessUnitManagerRoleName);
            bool userIsDirector =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName); // #2817: userIsDirector is added as per the requirement.
            bool userIsSeniorLeadership =
               Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SeniorLeadershipRoleName);// #2913: userIsSeniorLeadership is added as per the requirement.
            bool userIsProjectLead =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.ProjectLead);//added Project Lead as per #2941.

            //isReadOnly  variable can be removed as only this roles can access the page.
            bool isReadOnly = !userIsAdministrator && !userIsSalesPerson && !userIsPracticeManager && !userIsBusinessUnitManager && !userIsDirector && !userIsSeniorLeadership && !userIsProjectLead;// #2817: userIsDirector is added as per the requirement.
          
            txtProjectName.ReadOnly = isReadOnly;
            ddlClientName.Enabled = ddlPractice.Enabled = !isReadOnly;
            ddlSalesperson.Enabled = ddlProjectGroup.Enabled = !string.IsNullOrEmpty(ddlClientName.SelectedValue) && !isReadOnly;
            ddlPricingList.Enabled = !string.IsNullOrEmpty(ddlClientName.SelectedValue);
            if ((userIsPracticeManager || userIsBusinessUnitManager || userIsProjectLead) && !cblProjectManagers.Enabled && !ProjectId.HasValue)
            {
                try
                {
                    cblProjectManagers.SelectedValue = DataHelper.CurrentPerson.Id.ToString();
                }
                catch
                {
                    cblProjectManagers.SelectedValue = string.Empty;
                }
            }
            ddlProjectStatus.Enabled =
                // add new project mode
                !ProjectId.HasValue ||
                // Administrators always can change the project status
                userIsAdministrator ||
                // Practice Managers and Sales persons can change the status from Experimental, Inactive or Projected
                IsStatusValidForNonadmin(Project);

            btnAddMilistone.Visible = btnSave.Visible = !isReadOnly;

            AllowContinueWithoutSave = ProjectId.HasValue;

            if (IsPostBack && IsValidated)
            {
                Page.Validate(vsumProject.ValidationGroup);
                IsErrorPanelDisplay = !Page.IsValid;
            }

            NeedToShowDeleteButton();

            bool userIsHR = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.HRRoleName);
            bool userIsRecruiter = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.RecruiterRoleName);

            if (userIsProjectLead && !(userIsAdministrator || userIsSalesPerson || userIsPracticeManager || userIsBusinessUnitManager || userIsDirector || userIsSeniorLeadership || userIsHR || userIsRecruiter))
            {
                imgLink.Enabled = imgNavigateToOpp.Enabled = imgUnlink.Enabled = false;//as per #2941.
                cellProjectTools.Visible = false;
                ddlSalesperson.Enabled = txtSowBudget.Enabled = ddlDirector.Enabled = false;
            }

            #endregion Security

            try
            {
                if (!Page.IsValid)
                    IsErrorPanelDisplay = true;
            }
            catch
            {
            }
            if (IsErrorPanelDisplay && !IsOtherPanelDisplay)
            {
                PopulateErrorPanel();
            }
        }

        private void NeedToShowDeleteButton()
        {
            if (!ProjectId.HasValue)
                return;
            btnDelete.Visible = true;
            if (Project.Status.Id == 5)
            {
                btnDelete.Enabled = true;
            }
            else if (Project.Status.Id == 1 && (Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) || Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SeniorLeadershipRoleName)))
            {
                btnDelete.Enabled = true;
            }
            else
            {
                btnDelete.Enabled = false;
            }
        }

        protected void custProjectDesciption_ServerValidation(object source, ServerValidateEventArgs args)
        {
            args.IsValid = txtDescription.Text.Length <= 2000;
        }

        protected void btnMilestoneName_Command(object sender, CommandEventArgs e)
        {
            SaveAndRedirectToMilestone(e.CommandArgument);
        }

        private void SaveAndRedirectToMilestone(object commandArgument)
        {
            if (!SaveDirty || ValidateAndSave())
            {
                Redirect(string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                       Constants.ApplicationPages.MilestoneDetail,
                                       commandArgument) + GetProjectIdArgument(true, ProjectId.Value));
            }
        }

        protected void imgbtnDeleteAttachment_Click(object sender, EventArgs e)
        {
            var deleteBtn = (ImageButton)sender;
            var id = Convert.ToInt32(deleteBtn.Attributes["AttachmentId"]);

            if (ProjectId.HasValue)
            {
                AttachmentService.AttachmentService svc = WCFClientUtility.GetAttachmentService();

                svc.DeleteProjectAttachmentByProjectId(id, ProjectId.Value, User.Identity.Name);
                Project.Attachments.Remove(Project.Attachments.Find(pa => pa.AttachmentId == id));

                PopulateAttachmentControl(Project);
            }
            else
            {
                var list = AttachmentsForNewProject;
                list.Remove(list.Find(pa => pa.AttachmentId == id));

                AttachmentsForNewProject = list;
                BindProjectAttachments(AttachmentsForNewProject);
            }
        }

        private void BindProjectAttachments(List<ProjectAttachment> list)
        {
            if (list != null && list.Count > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repProjectAttachments.Visible = true;
                repProjectAttachments.DataSource = list;
                repProjectAttachments.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repProjectAttachments.Visible = false;
            }
        }

        protected void btnUpdateProjectName_OnClick(object sender, EventArgs e)
        {
            Page.Validate("ProjectName");

            if (Page.IsValid)
            {
                lblProjectNameLinkPopUp.Text = lblProjectName.Text = ProjectId.HasValue ? HttpUtility.HtmlEncode(txtProjectName.Text) : HttpUtility.HtmlEncode(txtProjectNameFirstTime.Text);
            }
            else
            {
                IsOtherPanelDisplay = true;
                mpeEditProjectName.Show();
            }
        }

        protected void btnAddMilistone_Click(object sender, EventArgs e)
        {
            if (!ProjectId.HasValue)
            {
                Page.Validate(vsumProject.ValidationGroup);
                if (Page.IsValid)
                {
                    int projectId = SaveData();

                    Redirect(Constants.ApplicationPages.MilestoneDetail + GetProjectIdArgument(false, projectId),
                        projectId.ToString());
                }
                else
                {
                    IsErrorPanelDisplay = true;
                }
            }
            else if (!SaveDirty || ValidateAndSave())
            {
                Redirect(Constants.ApplicationPages.MilestoneDetail + GetProjectIdArgument(false, ProjectId.Value),
                    ProjectId.Value.ToString());
            }
        }

        protected void stbAttachSOW_Click(object sender, EventArgs e)
        {
            if (IsDirty || !ProjectId.HasValue)
            {
                FromSaveButtonClick = false;
                ValidateSaveAndPopulate(false);
            }
            if (!IsDirty && ProjectId.HasValue)
            {
                mpeAttachSOW.Show();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            FromSaveButtonClick = true;
            ValidateSaveAndPopulate(true);
        }

        private void ValidateSaveAndPopulate(bool showSuccessPopup)
        {
            if (ValidateAndSave())
            {
                ClearDirty();
                if (showSuccessPopup)
                {
                    mlConfirmation.ShowInfoMessage(string.Format(Messages.SavedDetailsConfirmation, "Project"));
                    IsErrorPanelDisplay = true;
                }
            }

            if (Page.IsValid && ProjectId.HasValue)
            {
                ucProjectTimeTypes.AllTimeTypes = null;
                ucProjectTimeTypes.ProjectTimetypes = null;

                Project = GetCurrentProject(ProjectId);
                PopulateControls(Project);
                if (!SelectedId.HasValue)
                {
                    FillUnlinkedOpportunityList(Project.Client.Id);
                }
            }
            else
            {
                if (!cvIsInternal.IsValid)
                {
                    mlConfirmation.ShowErrorMessage(IsInternalChangeErrorMessage);
                    IsErrorPanelDisplay = true;
                }
                if (!cvWorkTypesAssigned.IsValid)
                {
                    mlConfirmation.ClearMessage();
                }
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (hdnProjectDelete.Value == "1")
            {
                using (var serviceClient = new ProjectServiceClient())
                {
                    try
                    {
                        serviceClient.ProjectDelete(ProjectId.Value, User.Identity.Name);

                        Redirect("Projects.aspx");
                    }
                    catch (Exception ex)
                    {
                        serviceClient.Abort();
                        mlConfirmation.ShowErrorMessage("{0}", ex.Message);
                        IsErrorPanelDisplay = true;
                    }
                }
            }
        }

        protected void btnDeleteWorkType_OnClick(object sender, EventArgs e)
        {
            var workTypeId = Convert.ToInt32(hdnWorkTypeId.Value);

            try
            {
                ServiceCallers.Custom.TimeType(tt => tt.RemoveTimeType(workTypeId));

                ucProjectTimeTypes.AllTimeTypes = ucProjectTimeTypes.AllTimeTypes.Where(tt => tt.Id != workTypeId).ToArray();
                ucProjectTimeTypes.ProjectTimetypes = ucProjectTimeTypes.ProjectTimetypes.Where(tt => tt.Id != workTypeId).ToArray();
                ucProjectTimeTypes.DataBindAllRepeaters();
            }
            catch (Exception ex)
            {
                if (ex.Message == "You cannot delete this Work type.Because, there are some time entries related to it.")
                {
                    mpeTimeEntriesRelatedToitPopup.Show();
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Sets the client discount
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlClientName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlClientName.SelectedValue))
            {
                int clientId = int.Parse(ddlClientName.SelectedValue);
                SetClientDefaultValues(clientId);

                FillUnlinkedOpportunityList(clientId);
            }
        }

        protected void ddlDirector_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateCSATOwnerList();
        }

        private void SetDefaultsToClientDependancyControls()
        {
            ddlDirector.SelectedIndex = 0;
            if (ddlProjectGroup.Items.Count > 0)
                ddlProjectGroup.SelectedIndex = 0;
        }

        private void AddListItemIfNotExists(DropDownList ddl, string value, Person person)
        {
            ListItem selectedItem = ddl.Items.FindByValue(value);
            if (selectedItem == null)
            {
                selectedItem = new ListItem();
                if (person == null)
                {
                    person = ServiceCallers.Custom.Person(p => p.GetPersonDetailsShort(int.Parse(value)));
                }
                selectedItem.Value = value;
                selectedItem.Text = person.PersonLastFirstName;
                ddl.Items.Add(selectedItem);
                ddl.SortByText();
            }
        }

        private void SetClientDefaultValues(int clientId)
        {
            Client client = GetClientByClientId(clientId);

            if (client != null)
            {
                if (ddlSalesperson.SelectedIndex == 0 && Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName)
                    && !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName)
                    && !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName)
                    && !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SeniorLeadershipRoleName))
                {
                    AddListItemIfNotExists(ddlSalesperson, DataHelper.CurrentPerson.Id.Value.ToString(), DataHelper.CurrentPerson);
                    ddlSalesperson.SelectedValue = DataHelper.CurrentPerson.Id.Value.ToString();
                }
                else
                {
                    if (ddlSalesperson.SelectedValue != DataHelper.CurrentPerson.Id.Value.ToString()
                        && Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName)
                        && !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName)
                        && !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName)
                        && !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SeniorLeadershipRoleName))
                    {
                        AddListItemIfNotExists(ddlSalesperson, client.DefaultSalespersonId.ToString(), null);
                        ddlSalesperson.SelectedIndex =
                            ddlSalesperson.Items.IndexOf(ddlSalesperson.Items.FindByValue(client.DefaultSalespersonId.ToString()));
                    }
                }

                if (client.DefaultDirectorId.HasValue)
                {
                    ListItem selectedDefaultDirector = ddlDirector.Items.FindByValue(client.DefaultDirectorId.Value.ToString());
                    if (selectedDefaultDirector != null)
                    {
                        ddlDirector.SelectedValue = selectedDefaultDirector.Value;
                    }
                    else
                    {
                        ddlDirector.SelectedIndex = 0;
                    }
                }
                else
                {
                    ddlDirector.SelectedIndex = 0;
                }

                if (client.IsNoteRequired)
                {
                    ddlNotes.SelectedValue = "1";
                    ddlNotes.Enabled = false;
                }
                else
                {
                    if (Project != null)
                    {
                        ddlNotes.SelectedValue = Project.IsNoteRequired ? "1" : "0";
                    }

                    bool userIsAdministrator = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                    bool userIsDirector = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName);// #2817: DirectorRoleName is added as per the requirement.
                    ddlNotes.Enabled = (userIsAdministrator || userIsDirector || (IsUserIsProjectOwner.HasValue && IsUserIsProjectOwner.Value));
                }

                var pricingLists = ServiceCallers.Custom.Client(clients => clients.GetPricingLists(client.Id));
                DataHelper.FillPricingLists(ddlPricingList, pricingLists.OrderBy(p => p.Name).ToArray());
            }

            DataHelper.FillProjectGroupList(ddlProjectGroup, clientId, null);
            if (ProjectId.HasValue)
            {
                var listItemValue = Project != null && Project.Group != null && Project.Group.Id.HasValue ?
                    ProjectId.Value.ToString() : "";
                var listItem = ddlProjectGroup.Items.FindByValue(listItemValue);
                if (listItem != null)
                    ddlProjectGroup.SelectedIndex = ddlProjectGroup.Items.IndexOf(listItem);
            }
            lblBusinessGroup.Text = string.Empty;
            SetBusinessGroupLabel();

            ddlClientName.Focus();
        }

        private static Client GetClientByClientId(int clientId)
        {
            using (var serviceClient = new ClientServiceClient())
            {
                try
                {
                    return serviceClient.GetClientDetail(clientId, DataHelper.CurrentPerson.Alias);
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        public void SetBusinessGroupLabel()
        {
            using (var serviceClient = new ProjectGroupServiceClient())
            {
                try
                {
                    if (ddlProjectGroup.SelectedValue != "")
                    {
                        BusinessGroup[] businessGroupList = serviceClient.GetBusinessGroupList(null, Convert.ToInt32(ddlProjectGroup.SelectedValue));

                        lblBusinessGroup.Text = businessGroupList.Any() ? businessGroupList.First().HtmlEncodedName : string.Empty;
                        lblBusinessGroup.ToolTip = businessGroupList.Any() ? businessGroupList.First().Name : string.Empty;
                    }
                    else
                    {
                        lblBusinessGroup.Text = string.Empty;
                    }
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void ddlProjectGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetBusinessGroupLabel();
        }

        protected void ddlCSATOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateDirectorsList();
        }

        public void PopulateCSATOwnerList()
        {
            var excludingPersons = new List<int>();
            if (ddlDirector.SelectedValue != "")
            {
                excludingPersons.Add(Convert.ToInt32(ddlDirector.SelectedValue));
            }
            int selectedValue = -1;
            string selectedText = "";
            if (ddlCSATOwner.SelectedValue != "")
            {
                if (!excludingPersons.Any(id => id == Convert.ToInt32(ddlCSATOwner.SelectedValue)))
                {
                    selectedValue = Convert.ToInt32(ddlCSATOwner.SelectedValue);
                    selectedText = ddlCSATOwner.SelectedItem.Text;
                }
            }
            DataHelper.FillCSATReviewerList(ddlCSATOwner, "-- Select CSAT Owner --", excludingPersons);
            if (selectedValue != -1)
            {
                ListItem selectedOwner = ddlCSATOwner.Items.FindByValue(selectedValue.ToString());
                if (selectedOwner == null)
                {
                    selectedOwner = new ListItem(selectedText, selectedValue.ToString());
                    ddlCSATOwner.Items.Add(selectedOwner);
                    ddlCSATOwner.SortByText();
                }
                ddlCSATOwner.SelectedValue = selectedValue.ToString();
            }
        }

        public void PopulateDirectorsList()
        {
            List<int> excludingPersons = new List<int>();
            if (ddlCSATOwner.SelectedValue != "")
            {
                excludingPersons.Add(Convert.ToInt32(ddlCSATOwner.SelectedValue));
            }
            int selectedValue = -1;
            string selectedText = "";
            if (ddlDirector.SelectedValue != "")
            {
                if (!excludingPersons.Any(id => id == Convert.ToInt32(ddlDirector.SelectedValue)))
                {
                    selectedValue = Convert.ToInt32(ddlDirector.SelectedValue);
                    selectedText = ddlDirector.SelectedItem.Text;
                }
            }
            DataHelper.FillDirectorsList(ddlDirector, "-- Select Client Director --", excludingPersons);
            if (selectedValue != -1)
            {
                ListItem selectedDirector = ddlDirector.Items.FindByValue(selectedValue.ToString());
                if (selectedDirector == null)
                {
                    selectedDirector = new ListItem(selectedText, selectedValue.ToString());
                    ddlDirector.Items.Add(selectedDirector);
                    ddlDirector.SortByText();
                }
                ddlDirector.SelectedValue = selectedValue.ToString();
            }
        }

        protected void DropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Only set an input focus
            ((Control)sender).Focus();
            if (ProjectId.HasValue)
            {
                ucCSAT.PopulateData(null);
            }
        }

        private void AddStatusIcon()
        {
            int statusId;
            string statusCssClass = "DisplayNone";
            string iconTooltip = ddlProjectStatus.SelectedItem.Text;
            if (int.TryParse(ddlProjectStatus.SelectedValue, out statusId))
            {
                if (statusId == 3 && !HasAttachments)
                {
                    statusCssClass = "ActiveProjectWithoutSOW";
                    iconTooltip = "Active without Attachment";
                }
                else if (statusId == 3 && HasAttachments)
                {
                    iconTooltip = "Active with Attachment";
                    statusCssClass = ProjectHelper.GetIndicatorClassByStatusId(statusId);
                }
                else
                {
                    statusCssClass = ProjectHelper.GetIndicatorClassByStatusId(statusId);
                }
            }
            divStatus.Attributes.Add("class", statusCssClass);
            divStatus.Attributes.Add("title", iconTooltip);
        }

        private void PopulateOpportunityLink()
        {
            if (Project != null && Project.Id.HasValue)
            {
                imgLink.Visible = true;
                imgNavigateToOpp.Visible = imgUnlink.Visible = Project.OpportunityId.HasValue;

                if (Project.OpportunityId.HasValue)
                {
                    lbOpportunity.Text = string.Format(OpportunityLinkedTextFormat, Project.OpportunityNumber);
                    imgNavigateToOpp.Attributes.Add("NavigateUrl", String.Format(Constants.ApplicationPages.DetailRedirectFormat, "OpportunityDetail.aspx", Project.OpportunityId.Value));//PraticeManagement.Utils.Urls.OpportunityDetailsLink(Project.OpportunityId.Value));
                }
                else
                {
                    lbOpportunity.Text = "There is no linked opportunity.";
                }
            }
        }

        protected void imgLink_Click(object sender, ImageClickEventArgs e)
        {
            if (IsDirty || !ProjectId.HasValue)
            {
                ValidateSaveAndPopulate(false);
                if (Page.IsValid)
                {
                    mpeLinkOpportunityPopup.Show();
                }
            }
            else
            {
                mpeLinkOpportunityPopup.Show();
            }
        }

        protected void imgUnlink_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                int? pricingListId = null;
                if (Project.PricingList != null)
                {
                    pricingListId = Project.PricingList.PricingListId.Value;
                }
                ServiceCallers.Custom.Project(p => p.AttachOpportunityToProject(Project.Id.Value, Project.OpportunityId.Value, User.Identity.Name, pricingListId, false));

                Project.OpportunityId = null;
                Project.OpportunityNumber = null;
                ddlOpportunities.SelectedIndex = -1;
            }
            catch
            { }
        }

        protected void btnLinkOpportunity_Click(object sender, EventArgs e)
        {
            Page.Validate(valSumLinkOpportunity.ValidationGroup);
            if (Page.IsValid)
            {
                try
                {
                    int opportunityId = Convert.ToInt32(ddlOpportunities.SelectedValue);
                    int? pricingListId = null;
                    if (Project.PricingList != null)
                    {
                        pricingListId = Project.PricingList.PricingListId.Value;
                    }
                    var opportunityNumber = ServiceCallers.Custom.Project(p => p.AttachOpportunityToProject(Project.Id.Value, opportunityId, User.Identity.Name, pricingListId, true));

                    Project.OpportunityId = opportunityId;
                    Project.OpportunityNumber = opportunityNumber;
                    mpeLinkOpportunityPopup.Hide();
                }
                catch
                { }
            }
            else
            {
                mpeLinkOpportunityPopup.Show();
            }
        }

        protected void custProjectStatus_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            // Verify for the selected status can be set by the user
            int currentStatus = 0;
            if (!string.IsNullOrEmpty(ddlProjectStatus.SelectedValue))
            {
                currentStatus = int.Parse(ddlProjectStatus.SelectedValue);
            }

            e.IsValid =
                // Administrators can set any status
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) ||
                // Practice Managers and Salespersons can set only status Experimental, Inactive or Projected
                (IsStatusValidForNonadmin(currentStatus) && IsStatusValidForNonadmin(Project)) ||
                // Status was not changed
                currentStatus == (Project != null && Project.Status != null ? Project.Status.Id : 0);
        }

        protected void custActiveStatus_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = true;
            if (!string.IsNullOrEmpty(ddlProjectStatus.SelectedValue) && (int)ProjectStatusType.Active == Convert.ToInt32(ddlProjectStatus.SelectedValue))
            {
                e.IsValid = !noMileStones;
            }
        }

        protected void cvAttachmentCategory_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (ddlAttachmentCategory.SelectedValue == "0")
            {
                args.IsValid = false;
            }
        }

        protected void cvClient_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (Project != null && Project.Id.HasValue && Project.Client.Id.HasValue && (ddlClientName.SelectedIndex == 0 || Project.Client.Id.Value != Convert.ToInt32(ddlClientName.SelectedValue)) && Project.HasTimeEntries)
            {
                args.IsValid = false;
            }
        }

        protected void cvGroup_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (Project != null && Project.Id.HasValue && Project.Group.Id.HasValue && Project.Group.Id.Value != Convert.ToInt32(ddlProjectGroup.SelectedValue == "" ? "0" : ddlProjectGroup.SelectedValue) && Project.HasTimeEntries)
            {
                args.IsValid = false;
            }
        }

        /// <summary>
        /// Determines whether the project status can be changed to or from the specified value by a non-administrator.
        /// </summary>
        /// <param name="statusId">An ID of the status to be verified.</param>
        /// <returns>true if the status is allowed and false otherwise.</returns>
        private static bool IsStatusValidForNonadmin(int statusId)
        {
            return
                statusId == (int)ProjectStatusType.Experimental ||
                statusId == (int)ProjectStatusType.Inactive ||
                statusId == (int)ProjectStatusType.Projected;
        }

        /// <summary>
        /// Determines whether the project status can be changed to or from the specified value by a non-administrator.
        /// </summary>
        /// <param name="project">The project which status need to be checked.</param>
        /// <returns>true if the status is allowed and false otherwise.</returns>
        private static bool IsStatusValidForNonadmin(Project project)
        {
            return
                // Project does not exist!
                project == null || project.Status == null ||
                // Verify the project status
                IsStatusValidForNonadmin(project.Status.Id);
        }

        private bool ValidateProjectTimeTypesTab()
        {
            bool isValid = true;
            if (ucProjectTimeTypes.ProjectTimetypes == null)
            {
                ucProjectTimeTypes.PopulateControls();
            }
            int viewIndex = mvProjectDetailTab.ActiveViewIndex;
            btnView_Command(btnProjectTimeTypes, new CommandEventArgs("", "2"));
            Page.Validate(vsumProject.ValidationGroup);
            if (!Page.IsValid)
            {
                isValid = false;
            }
            btnView_Command(rowSwitcher.Cells[viewIndex].Controls[0], new CommandEventArgs("", viewIndex.ToString()));
            return isValid;
        }

        private bool ValidateCommissionsTab()
        {
            bool isValid = true;
            if (!ProjectId.HasValue || Project == null || Project.Milestones == null || Project.Milestones.Count <= 0)
                return true;
            int viewIndex = mvProjectDetailTab.ActiveViewIndex;
            btnView_Command(btnCommissions, new CommandEventArgs("", "5"));
            projectAttribution.ValidateCommissionsTab();
            if (!Page.IsValid)
            {
                isValid = false;
            }
            btnView_Command(rowSwitcher.Cells[viewIndex].Controls[0], new CommandEventArgs("", viewIndex.ToString()));

            return isValid;
        }

        protected override bool ValidateAndSave()
        {
            bool result = false;
            Page.Validate(vsumProject.ValidationGroup);
            if (Page.IsValid && ValidateProjectTimeTypesTab() && (CompletedStatus || ValidateCompleStatusPopup()) && ValidateCommissionsTab())
            {
                cvCompletedStatus.IsValid = true;
                int? id = SaveData();
                CompletedStatus = false;
                ProjectId = id;
                ClearDirty();
                projectExpenses.BindExpenses();
                result = true;
                if (FromSaveButtonClick)
                {
                    IsErrorPanelDisplay = true;
                }
                int viewIndex = mvProjectDetailTab.ActiveViewIndex;
                if (viewIndex == 10)
                {
                    ucCSAT.PopulateData(null);
                }
            }
            else
            {
                IsErrorPanelDisplay = true;
            }
            return result;
        }

        public bool ValidateCompleStatusPopup()
        {
            int currentStatus = int.Parse(ddlProjectStatus.SelectedValue);
            if (ProjectId.HasValue && currentStatus != Project.Status.Id && currentStatus == (int)ProjectStatusType.Completed)
            {
                var projectCSATList = ucCSAT.ProjectCSATList;
                if (projectCSATList.Any(p => p.ReviewEndDate > DateTime.Now))
                {
                    IsOtherPanelDisplay = true;
                    cvCompletedStatus.IsValid = false;
                    lblCompletedStatus.Text = "Project status cannot be set to 'Completed' with CSAT Review dates beyond the current date. Click 'OK' to set the Review dates to current date or click 'Cancel' to make no changes.";
                    mpeCompletedStatusPopUp.Show();
                    return false;
                }
            }
            return true;
        }

        public bool ValidateAndSaveFromOtherChildControls()
        {
            return (!IsDirty || ValidateAndSave());
        }

        protected void btnPersonName_Command(object sender, CommandEventArgs e)
        {
            SaveAndRedirectToPerson(e.CommandArgument);
        }

        private void SaveAndRedirectToPerson(object commandArgument)
        {
            if (!SaveDirty || ValidateAndSave())
            {
                string[] argumentParts = commandArgument.ToString().Split(":".ToCharArray());
                Redirect(string.Format(Constants.ApplicationPages.RedirectMilestonePersonIdFormat,
                                       Constants.ApplicationPages.MilestonePersonDetail,
                                       argumentParts[0],
                                       argumentParts[1]));
            }
        }

        private int SaveData()
        {
            var project = new Project();
            PopulateData(project);
            int result = -1;
            using (var serviceClient = new ProjectServiceClient())
            {
                try
                {
                    result = serviceClient.SaveProjectDetail(project, User.Identity.Name);
                    if (ProjectId.HasValue)
                    {
                        Project = GetCurrentProject(ProjectId.Value);
                    }
                    hdnProjectId.Value = result.ToString();
                    DataHelper.FillProjectStatusList(ddlProjectStatus, string.Empty, new List<int>());
                    ddlProjectStatus.SelectedValue = project.Status != null ? project.Status.Id.ToString() : string.Empty;
                    if (Project != null && Project.Milestones != null && Project.Milestones.Count > 0)
                        projectAttribution.FinalSave();
                    ShowTabs();
                }
                catch (CommunicationException ex)
                {
                    serviceClient.Abort();
                    if (ex.Message == IsInternalChangeErrorMessage)
                    {
                        cvIsInternal.IsValid = false;
                    }
                    else if (ex.Message.Contains("Time has already been entered for the following Work Type(s). The Work Type(s) cannot be unassigned from this project."))
                    {
                        var message = ex.Message;
                        message = message.Replace("Time has already been entered for the following Work Type(s). The Work Type(s) cannot be unassigned from this project.", "");
                        cvWorkTypesAssigned.IsValid = false;
                        ucProjectTimeTypes.ShowAlert(message);
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            if (AttachmentsForNewProject != null && result != -1)
            {
                AttachmentService.AttachmentService svc = WCFClientUtility.GetAttachmentService();
                foreach (var attachment in AttachmentsForNewProject)
                {
                    svc.SaveProjectAttachment(attachment, result, User.Identity.Name);
                }
                ViewState.Remove(ProjectAttachmentsKey);
            }
            return result;
        }

        protected override void Display()
        {
            DataHelper.FillPracticeListOnlyActive(ddlPractice, "-- Select Practice Area --");
            DataHelper.FillClientListForProject(ddlClientName, "-- Select Account --", ProjectId);
            Person person = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) || Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName) ? null : DataHelper.CurrentPerson;
            DataHelper.FillSalespersonListOnlyActiveForLoginPerson(ddlSalesperson, person, "-- Select Salesperson --");
            DataHelper.FillProjectStatusList(ddlProjectStatus, string.Empty);
            DataHelper.FillProjectStatusList(ddlCloneProjectStatus, string.Empty, null, true);
            DataHelper.FillBusinessTypes(ddlBusinessOptions);
            PopulateDirectorsList();
            PopulateCSATOwnerList();
            DataHelper.FillSeniorManagerList(ddlSeniorManager, "-- Select Senior Manager --");
            string statusids = (int)PersonStatusType.Active + ", " + (int)PersonStatusType.TerminationPending;
            Person[] persons = ServiceCallers.Custom.Person(p => p.OwnerListAllShort(statusids));
            DataHelper.FillListDefault(cblProjectManagers, "All Project Managers", persons, false, "Id", "PersonLastFirstName");
            DataHelper.FillListDefault(ddlProjectOwner, "-- Select Project Owner --", persons, false, "Id", "PersonLastFirstName");

            if (ProjectId.HasValue)
            {
                Project = GetCurrentProject(ProjectId.Value);
                PopulateControls(Project);
                DataHelper.FillProjectStatusList(ddlProjectStatus, string.Empty, new List<int>());
                FillUnlinkedOpportunityList(Project.Client.Id);
            }
            else
            {
                int clientId;
                var excludeStatuses = new List<int>
                {
                    (int) (ProjectStatusType.Active),
                    (int) (ProjectStatusType.Completed)
                };
                DataHelper.FillProjectStatusList(ddlProjectStatus, string.Empty, excludeStatuses);
                ddlProjectStatus.SelectedIndex =
                    ddlProjectStatus.Items.IndexOf(
                    ddlProjectStatus.Items.FindByValue(((int)ProjectStatusType.Projected).ToString()));

                if (int.TryParse(Page.Request.QueryString["clientId"], out clientId))
                {
                    ddlClientName.SelectedValue = clientId.ToString();
                    SetClientDefaultValues(clientId);
                }
                // Default values for new projects.
                bool userIsAdministrator =
                    Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                if (!userIsAdministrator)
                {
                    ddlProjectStatus.Enabled = false;
                }
                PopulateAttachmentControl(Project);
                ucProjectTimeTypes.ResetSearchTextFilters();
                ucProjectTimeTypes.PopulateControls();

                var capabilities = ServiceCallers.Custom.Practice(p => p.GetPracticeCapabilities(null, null)).Where(pc => pc.IsActive);
                DataHelper.FillListDefault(cblPracticeCapabilities, "All Capabilities", capabilities.ToArray(), false, "CapabilityId", "MergedName");
            }
        }

        private Project GetCurrentProject(int? id)
        {
            Project project;
            using (var serviceClient = new ProjectServiceClient())
            {
                try
                {
                    project = serviceClient.GetProjectDetailWithoutMilestones(id.Value, User.Identity.Name);

                    Generic.RedirectIfNullEntity(project, Response);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
            return project;
        }

        protected void dtpProjectEnd_SelectionChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        private void PopulateControls(Project project)
        {
            if (project != null)
            {
                lblProjectNumber.Text = !string.IsNullOrEmpty(project.ProjectNumber) ? project.ProjectNumber + " - " : string.Empty;
                lblProjectNameLinkPopUp.Text = lblProjectName.Text = project.HtmlEncodedName;
                txtProjectNameFirstTime.Text = txtProjectName.Text = project.Name;
                lblProjectRange.Text = string.IsNullOrEmpty(project.ProjectRange) ? string.Empty : string.Format("({0})", project.ProjectRange);
                txtDescription.Text = project.Description;
                txtPONumber.Text = project.PONumber;
                ddlNotes.SelectedValue = project.IsNoteRequired ? "1" : "0";
                ddlNotes.Enabled = !project.Client.IsNoteRequired;
                txtSowBudget.Text = project.SowBudget != null ? project.SowBudget.Value.ToString("###,###,###,###,##0") : string.Empty;
                PopulateClientDropDown(project);
                FillAndSelectProjectGroupList(project);
                PopulatePracticeDropDown(project);
                ddlProjectStatus.SelectedValue = project.Status != null ? project.Status.Id.ToString() : string.Empty;
                PopulateProjectManagerDropDown(project);
                PopulateSalesPersonDropDown(project);
                financials.Project = project;
                txtBuyerName.Text = project.BuyerName;
                if (project.Director != null && project.Director.Id.HasValue)
                {
                    ListItem selectedDirector = ddlDirector.Items.FindByValue(project.Director.Id.Value.ToString());
                    if (selectedDirector == null)
                    {
                        selectedDirector = new ListItem(project.Director.PersonLastFirstName, project.Director.Id.Value.ToString());
                        ddlDirector.Items.Add(selectedDirector);
                        ddlDirector.SortByText();
                    }
                    ddlDirector.SelectedValue = selectedDirector.Value;
                }

                if (project.CSATOwnerId != 0)
                {
                    ListItem selectedOwner = ddlCSATOwner.Items.FindByValue(project.CSATOwnerId.ToString());
                    if (selectedOwner == null)
                    {
                        selectedOwner = new ListItem(project.CSATOwnerName, project.CSATOwnerId.ToString());
                        ddlCSATOwner.Items.Add(selectedOwner);
                        ddlCSATOwner.SortByText();
                    }
                    ddlCSATOwner.SelectedValue = project.CSATOwnerId.ToString();
                }
                PopulateDirectorsList();
                PopulateCSATOwnerList();

                if (project.SeniorManagerId != 0)
                {
                    ListItem selectedManager = ddlSeniorManager.Items.FindByValue(project.SeniorManagerId.ToString());
                    if (selectedManager == null)
                    {
                        selectedManager = new ListItem(project.SeniorManagerName, project.SeniorManagerId.ToString());
                        ddlSeniorManager.Items.Add(selectedManager);
                        //  ddlSeniorManager.SortByText();
                    }
                    ddlSeniorManager.SelectedValue = selectedManager.Value;
                }
                if (project.ProjectOwner != null)
                {
                    ListItem selectedProjectOwner = ddlProjectOwner.Items.FindByValue(project.ProjectOwner.Id.Value.ToString());
                    if (selectedProjectOwner == null)
                    {
                        Person selectedPerson = ServiceCallers.Custom.Person(p => p.GetPersonDetailsShort(project.ProjectOwner.Id.Value));
                        selectedProjectOwner = new ListItem(selectedPerson.PersonLastFirstName, project.ProjectOwner.Id.Value.ToString());
                        ddlProjectOwner.Items.Add(selectedProjectOwner);
                        ddlProjectOwner.SortByText();
                    }
                    ddlProjectOwner.SelectedValue = project.ProjectOwner.Id.ToString();
                }
                int viewIndex = mvProjectDetailTab.ActiveViewIndex;
                if (viewIndex == 8) //History
                {
                    activityLog.Update();
                }
            }
            PopulateAttachmentControl(project);
            ucProjectTimeTypes.ResetSearchTextFilters();
            ucProjectTimeTypes.PopulateControls();
        }

        private void PopulateAttachmentControl(Project project)
        {
            if (project != null && project.Attachments != null && project.Attachments.Count > 0)
            {
                divEmptyMessage.Style["display"] = "none";
                repProjectAttachments.Visible = true;
                repProjectAttachments.DataSource = project.Attachments;
                repProjectAttachments.DataBind();
            }
            else
            {
                divEmptyMessage.Style["display"] = "";
                repProjectAttachments.Visible = false;
            }
        }

        public string GetNavigateUrl(string attachmentFileName, int attachmentId)
        {
            if (Project != null && Project.Id.HasValue)
            {
                return string.Format(ProjectAttachmentHandlerUrl, Project.Id.ToString(), HttpUtility.UrlEncode(attachmentFileName), attachmentId);
            }
            return string.Empty;
        }

        public bool IsProjectCreated()
        {
            return (Project == null || !Project.Id.HasValue);
        }

        private void PopulateSalesPersonDropDown(Project project)
        {
            if (project != null && project.SalesPersonId > 0)
            {
                ListItem selectedSalesPerson = ddlSalesperson.Items.FindByValue(project.SalesPersonId.ToString());
                if (selectedSalesPerson == null)
                {
                    Person selectedPerson = DataHelper.GetPersonWithoutFinancials(project.SalesPersonId);

                    selectedSalesPerson = new ListItem(selectedPerson.PersonLastFirstName, selectedPerson.Id.Value.ToString());
                    ddlSalesperson.Items.Add(selectedSalesPerson);
                    ddlSalesperson.SortByText();
                }
                ddlSalesperson.SelectedValue = selectedSalesPerson.Value;
            }
        }

        private void PopulateProjectManagerDropDown(Project project)
        {
            if (project.ProjectManagers != null && project.ProjectManagers.Count > 0)
            {
                var selectedStr = new StringBuilder();
                foreach (var projManager in project.ProjectManagers)
                {
                    var managerId = projManager.Id.Value.ToString();
                    ListItem selectedManager = cblProjectManagers.Items.FindByValue(managerId);
                    if (selectedManager == null)
                    {
                        selectedManager = new ListItem(projManager.PersonLastFirstName, managerId);
                        cblProjectManagers.Items.Add(selectedManager);
                    }
                    selectedStr.Append(selectedManager.Value + ",");
                }
                cblProjectManagers.SelectedItems = selectedStr.ToString();
            }
            else
            {
                cblProjectManagers.SelectedItems = string.Empty;
            }
        }

        private void PopulatePracticeDropDown(Project project)
        {
            ListItem selectedPractice = null;
            if (project != null && project.Practice != null)
            {
                selectedPractice = ddlPractice.Items.FindByValue(project.Practice.Id.ToString());
            }
            var projectCapabilityIdList = project.ProjectCapabilityIds.Split(',');
            var capabilities = ServiceCallers.Custom.Practice(p => p.GetPracticeCapabilities(null, null));
            DataHelper.FillListDefault(cblPracticeCapabilities, "All Capabilities", capabilities.Where(pc => pc.IsActive || projectCapabilityIdList.Any(p => p == pc.CapabilityId.ToString())).ToArray(), false, "CapabilityId", "MergedName");
            // For situation, when disabled practice is assigned to project.
            if (selectedPractice == null)
            {
                selectedPractice = new ListItem(project.Practice.Name, project.Practice.Id.ToString());
                ddlPractice.Items.Add(selectedPractice);
                ddlPractice.SortByText();
            }
            ddlPractice.SelectedValue = selectedPractice.Value;
            cblPracticeCapabilities.SelectedItems = project.ProjectCapabilityIds;
        }

        private void PopulateClientDropDown(Project project)
        {
            ListItem selectedClient = null;
            if (project != null && project.Client != null)
            {
                selectedClient = ddlClientName.Items.FindByValue(project.Client.Id.ToString());
            }
            if (selectedClient == null)
            {
                selectedClient = new ListItem(project.Client.Name, project.Client.Id.ToString());
                ddlClientName.Items.Add(selectedClient);
                ddlClientName.SortByText();
            }
            ddlClientName.SelectedValue = selectedClient.Value;
        }

        private void FillAndSelectProjectGroupList(Project project)
        {
            if (project.Client != null && project.Client.Id.HasValue)
            {
                DataHelper.FillProjectGroupList(ddlProjectGroup, project.Client.Id.Value, null);
                var pricingLists = ServiceCallers.Custom.Client(client => client.GetPricingLists(project.Client.Id.Value));
                DataHelper.FillPricingLists(ddlPricingList, pricingLists.OrderBy(p => p.Name).ToArray());
                PopulateProjectGroupDropDown(project);
            }
        }

        private void FillUnlinkedOpportunityList(int? clientId)
        {
            if (clientId.HasValue)
            {
                DataHelper.FillUnlinkedOpportunityList(clientId, ddlOpportunities, "-- Select Opportunity --");
            }
            else
            {
                ddlOpportunities.Items.Clear();
                ddlOpportunities.Items.Add(new ListItem { Text = "-- Select Opportunity --", Value = "" });
                ddlOpportunities.DataBind();
            }
        }

        private void PopulateProjectGroupDropDown(Project project)
        {
            if (project != null && project.Group != null)
            {
                ListItem selectedProjectGroup = ddlProjectGroup.Items.FindByValue(project.Group.Id.ToString());
                ListItem selectedBusinessTypes = ddlBusinessOptions.Items.FindByValue(((int)project.BusinessType).ToString() == "0" ? "" : ((int)project.BusinessType).ToString());
                if (selectedProjectGroup == null)
                {
                    selectedProjectGroup = new ListItem(project.Group.Name, project.Group.Id.ToString());
                    ddlProjectGroup.Items.Add(selectedProjectGroup);
                }
                if (project.PricingList != null)
                {
                    ListItem selectedPricingList = ddlPricingList.Items.FindByValue(project.PricingList.PricingListId.ToString());
                    if (selectedPricingList == null)
                    {
                        selectedPricingList = new ListItem(project.PricingList.Name, project.PricingList.PricingListId.ToString());
                        ddlPricingList.Items.Add(selectedPricingList);
                    }
                    ddlPricingList.SelectedValue = selectedPricingList.Value;
                }
                else
                {
                    ddlPricingList.SelectedIndex = 0;
                }
                ddlBusinessOptions.SelectedValue = selectedBusinessTypes.Value;
                ddlProjectGroup.SelectedValue = selectedProjectGroup.Value;
                SetBusinessGroupLabel();
            }
        }

        private void PopulateData(Project project)
        {
            project.Id = ProjectId;
            project.Name = ProjectId.HasValue ? HttpUtility.HtmlDecode(lblProjectName.Text) : txtProjectNameFirstTime.Text;
            project.BuyerName = txtBuyerName.Text;
            project.PONumber = txtPONumber.Text;
            project.Client = new Client { Id = int.Parse(ddlClientName.SelectedValue) };
            project.Practice = new Practice { Id = int.Parse(ddlPractice.SelectedValue) };
            project.ProjectCapabilityIds = cblPracticeCapabilities.SelectedItems;
            project.Status = new ProjectStatus { Id = int.Parse(ddlProjectStatus.SelectedValue) };
            project.ProjectManagerIdsList = cblProjectManagers.SelectedItems;
            project.IsInternal = false; //AS per Matt Reilly MattR@logic2020.com
            //date: Sat, Mar 17, 2012 at 1:53 AM
            //subject: RE: Time Entry conversion - deployment step
            project.BusinessType = (BusinessType)Enum.Parse(typeof(BusinessType), ddlBusinessOptions.SelectedValue == string.Empty ? "0" : ddlBusinessOptions.SelectedValue);
            project.CanCreateCustomWorkTypes = true;
            PopulateProjectGroup(project);
            project.SalesPersonId = ddlSalesperson.SelectedIndex > 0 ? int.Parse(ddlSalesperson.SelectedValue) : 0;
            project.Description = txtDescription.Text;
            project.SowBudget = string.IsNullOrEmpty(txtSowBudget.Text) ? null : (decimal?)Convert.ToDecimal(txtSowBudget.Text);
            project.ProjectOwner = new Person { Id = Convert.ToInt32(ddlProjectOwner.SelectedValue) };
            PopulatePricingList(project);
            project.IsNoteRequired = ddlNotes.SelectedValue == "1";
            if (ddlDirector.SelectedIndex > 0)
                project.Director = new Person { Id = int.Parse(ddlDirector.SelectedValue) };
            int selectedSeniorManagerId = int.Parse(ddlSeniorManager.SelectedValue);
            project.SeniorManagerId = selectedSeniorManagerId;
            project.IsSeniorManagerUnassigned = selectedSeniorManagerId == -1;
            project.ProjectWorkTypesList = ucProjectTimeTypes.HdnTimeTypesAssignedToProjectValue;
            if (ddlCSATOwner.SelectedValue != string.Empty)
            {
                project.CSATOwnerId = int.Parse(ddlCSATOwner.SelectedValue);
            }
        }

        private void PopulatePricingList(Project project)
        {
            project.PricingList = ddlPricingList.SelectedValue == string.Empty ? null : new PricingList { PricingListId = int.Parse(ddlPricingList.SelectedValue) };
        }

        private void PopulateProjectGroup(Project project)
        {
            project.Group = new ProjectGroup { Id = int.Parse(ddlProjectGroup.SelectedValue) };
        }

        public static string GetProjectIdArgument(bool useAmpersand, int projectId)
        {
            return (useAmpersand ? "&" : "?") + string.Format(ProjectIdFormat, projectId);
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SelectView((Control)sender, viewIndex);
            tblProjectDetailTabViewSwitch_ActiveViewIndex = viewIndex;
            if (viewIndex == 8) //History
            {
                activityLog.Update();
            }
            else if (viewIndex == 4 && ProjectId.HasValue)
            {
                financials.Project = GetCurrentProject(ProjectId.Value);
            }
            else if (viewIndex == 10 && ProjectId.HasValue)
            {
                ucCSAT.PopulateData(null);
            }
        }

        private void SelectView(Control sender, int viewIndex)
        {
            mvProjectDetailTab.ActiveViewIndex = viewIndex;

            foreach (TableCell cell in tblProjectDetailTabViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";
        }

        protected void lnkProjectAttachment_OnClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdnDownloadAttachmentId.Value))
            {
                var id = Convert.ToInt32(hdnDownloadAttachmentId.Value);
                var list = AttachmentsForNewProject;
                if (list.Any(pa => pa.AttachmentId == id))
                {
                    var attachment = list[list.FindIndex(pa => pa.AttachmentId == id)];

                    string FileName = attachment.AttachmentFileName;
                    byte[] attachmentData = attachment.AttachmentData;

                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ContentType = "application/octet-stream";
                    HttpContext.Current.Response.AddHeader(
                        "content-disposition", string.Format("attachment; filename='{0}'", HttpUtility.HtmlEncode(FileName)));

                    int len = attachmentData.Length;
                    var buffer = new byte[1024];

                    Stream outStream = HttpContext.Current.Response.OutputStream;
                    using (var stream = new MemoryStream(attachmentData))
                    {
                        int bytes;
                        while (len > 0 && (bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outStream.Write(buffer, 0, bytes);
                            HttpContext.Current.Response.Flush();
                            len -= bytes;
                        }
                    }
                    HttpContext.Current.Response.End();
                }
            }
        }

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            if (ProjectId.HasValue)
            {
                Project = GetCurrentProject(ProjectId);
                PopulateAttachmentControl(Project);
            }
            ddlAttachmentCategory.SelectedValue = "0";
        }

        public static byte ObjToByte(object o)
        {
            return Convert.ToByte(o);
        }

        protected void lnkClone_OnClick(object sender, EventArgs e)
        {
            Page.Validate("CloneStatus");
            if (!Page.IsValid)
                return;
            var clonedId =
                DataHelper.CloneProject(
                    new ProjectCloningContext
                    {
                        Project = new Project { Id = ProjectId },
                        CloneMilestones = chbCloneMilestones.Checked,
                        CloneCommissions = chbCloneCommissions.Checked,
                        ProjectStatus = new ProjectStatus
                        {
                            Id = Convert.ToInt32(ddlCloneProjectStatus.SelectedValue)
                        }
                    });

            Generic.RedirectWithReturnTo(
                string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                Constants.ApplicationPages.ProjectDetail,
                clonedId),
                Request.Url.AbsoluteUri,
                Response
                );
        }

        protected void chbCloneMilestones_CheckedChanged(object sender, EventArgs e)
        {
            chbCloneCommissions.Checked = chbCloneCommissions.Enabled = chbCloneMilestones.Checked;
        }

        public string GetWrappedText(string name)
        {
            if (name.Length > 30)
            {
                for (int i = 30; i < name.Length; i = i + 30)
                {
                    name = name.Insert(i, "<wbr />");
                }
            }
            return name;
        }

        private void PopulateErrorPanel()
        {
            mpeErrorPanel.Show();
        }

        protected void imgMailToProjectOwner_OnClick(object sender, EventArgs e)
        {
            int ownerId;
            int.TryParse(ddlProjectOwner.SelectedValue, out ownerId);
            MailTo(ownerId);
        }

        protected void imgMailToSeniorManager_OnClick(object sender, EventArgs e)
        {
            int managerId;
            int.TryParse(ddlSeniorManager.SelectedValue, out managerId);
            MailTo(managerId);
        }

        protected void imgMailToCSATOwner_OnClick(object sender, EventArgs e)
        {
            int ownerId;
            int.TryParse(ddlCSATOwner.SelectedValue, out ownerId);
            MailTo(ownerId);
        }

        protected void imgMailToClientDirector_OnClick(object sender, EventArgs e)
        {
            int directorId;
            int.TryParse(ddlDirector.SelectedValue, out directorId);
            MailTo(directorId);
        }

        protected void imgMailToSalesperson_OnClick(object sender, EventArgs e)
        {
            int salesPersonId;
            int.TryParse(ddlSalesperson.SelectedValue, out salesPersonId);
            MailTo(salesPersonId);
        }

        private void MailTo(int personId)
        {
            string subject = Project.ProjectNumber + " - " + Project.Name;
            Person person = ServiceCallers.Custom.Person(p => p.GetPersonDetailsShort(personId));
            string peronEmailId = person.Alias;
            string function = string.Format("mailTo('{0}');", string.Format(MailToSubjectFormat, peronEmailId, subject));
            ScriptManager.RegisterStartupScript(this, GetType(), "Mailto", function, true);
        }

        protected void btnOkCompletedStatusPopup_Click(object sender, EventArgs e)
        {
            CompletedStatus = true;
            btnSave_Click(btnSave, new EventArgs());
        }

        protected void btnCancelCompletedStatusPopup_Click(object sender, EventArgs e)
        {
            ResetToPreviousData();
        }

        private void ResetToPreviousData()
        {
            Project = GetCurrentProject(ProjectId);
            PopulateControls(Project);
        }

        #endregion Methods

        #region Implementation of IPostBackEventHandler

        /// <summary>
        /// When implemented by a class, enables a server control to process an event raised when a form is posted to the server.
        /// </summary>
        /// <param name="eventArgument">A <see cref="T:System.String"/> that represents an optional
        /// event argument to be passed to the event handler. </param>
        public void RaisePostBackEvent(string eventArgument)
        {
            var args = eventArgument.Split(':');
            var target = args[0];

            switch (target)
            {
                case ProjectMilestonesFinancials.MILESTONE_TARGET:
                    SaveAndRedirectToMilestone(args[1]);
                    break;

                case ProjectPersons.PERSON_TARGET:
                    SaveAndRedirectToMilestone(args[1]);
                    break;

                case ProjectCSAT.ProjectCSAT_TARGET:
                    if (!SaveDirty || ValidateAndSave())
                        Redirect(eventArgument);
                    break;

                default:
                    if (!SaveDirty || ValidateAndSave())
                        Redirect(eventArgument);
                    break;
            }
        }

        #endregion Implementation of IPostBackEventHandler
    }
}


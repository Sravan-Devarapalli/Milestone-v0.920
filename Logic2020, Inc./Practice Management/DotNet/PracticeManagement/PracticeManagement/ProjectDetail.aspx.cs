using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
using PraticeManagement.AttachmentService;
using PraticeManagement.ClientService;
using PraticeManagement.Controls;
using PraticeManagement.DefaultCommissionService;
using PraticeManagement.ProjectService;
using PraticeManagement.Utils;
using BillingInfo = DataTransferObjects.BillingInfo;
using ProjectAttachment = DataTransferObjects.ProjectAttachment;

namespace PraticeManagement
{
    public partial class ProjectDetail : PracticeManagementPageBase, IPostBackEventHandler
    {
        #region Constants

        private const string ProjectIdFormat = "projectId={0}";
        private const string ProjectKey = "Project";
        private const string ProjectAttachmentHandlerUrl = "~/Controls/Projects/ProjectAttachmentHandler.ashx?ProjectId={0}&FileName={1}&AttachmentId={2}";
        private const string AttachSOWMessage = "File must be in PDF format and no larger than {0}kb";
        private const string AttachmentFileSize = "AttachmentFileSize";
        private const string ProjectAttachmentsKey = "ProjectAttachmentsKey";
        #endregion

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

        public int? ProjectId
        {
            get
            {
                if (SelectedId.HasValue)
                {
                    return SelectedId;
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.hdnProjectId.Value))
                    {
                        int projectid;
                        if (Int32.TryParse(this.hdnProjectId.Value, out projectid))
                        {
                            return projectid;
                        }
                    }
                    return null;
                }
            }
            set
            {
                this.hdnProjectId.Value = value.ToString();
            }
        }

        private List<AttachmentService.ProjectAttachment> AttachmentsForNewProject
        {
            get
            {
                return ViewState[ProjectAttachmentsKey] as List<AttachmentService.ProjectAttachment>;
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

        #endregion

        #region Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (ProjectId.HasValue && !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName))
                {
                    if (IsUserHasPermissionOnProject.HasValue && !IsUserHasPermissionOnProject.Value
                        && IsUserisOwnerOfProject.HasValue && !IsUserisOwnerOfProject.Value)
                    {
                        Response.Redirect(@"~\GuestPages\AccessDenied.aspx");
                    }
                }

                txtProjectName.Focus();

                int clientId = -1;
                if (int.TryParse(Page.Request.QueryString["clientId"], out clientId))
                {
                    ddlClientName.SelectedValue = clientId.ToString();
                    SetClientDefaultValues(clientId);
                    DataHelper.FillProjectGroupList(ddlProjectGroup, clientId, null);
                }

                if (!string.IsNullOrEmpty(Page.Request.QueryString["Id"]))
                {
                    TableCellHistoryg.Visible = true;
                    cellProjectTools.Visible = true;
                }

                int size = Convert.ToInt32(SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Project, AttachmentFileSize));

                lblAttachmentMessage.Text = string.Format(AttachSOWMessage, size / 1000);
            }
            mlConfirmation.ClearMessage();

            btnUpload.Attributes["onclick"] = "return CanShowPrompt();";



        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (ProjectId.HasValue && Project != null && Project.Milestones != null && Project.Milestones.Count > 0)
            {
                cellExpenses.Visible = true;
            }
            else
            {
                cellExpenses.Visible = false;
            }
            if (ddlProjectGroup.Items.Count > 0)
                ddlProjectGroup.SortByText();
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
            bool userIsDirector =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName); // #2817: userIsDirector is added as per the requirement.
            bool userIsSeniorLeadership =
               Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SeniorLeadershipRoleName);// #2913: userIsSeniorLeadership is added as per the requirement.

            bool isReadOnly = !userIsAdministrator && !userIsSalesPerson && !userIsPracticeManager && !userIsDirector && !userIsSeniorLeadership;// #2817: userIsDirector is added as per the requirement.

            txtProjectName.ReadOnly = txtClientDiscount.ReadOnly = isReadOnly;
            txtSalesCommission.ReadOnly = !userIsAdministrator;

            ddlClientName.Enabled = ddlPractice.Enabled = !isReadOnly;

            chbReceiveManagementCommission.Enabled = rlstManagementCommission.Enabled =
                userIsSalesPerson || userIsAdministrator;
            txtManagementCommission.ReadOnly = !userIsSalesPerson && !userIsAdministrator;

            chbReceivesSalesCommission.Enabled = userIsAdministrator;
            ddlSalesperson.Enabled = (userIsAdministrator || userIsDirector) && !string.IsNullOrEmpty(ddlClientName.SelectedValue);
            ddlProjectManager.Enabled = ddlDirector.Enabled = (userIsAdministrator || userIsDirector || userIsSeniorLeadership || userIsSalesPerson) && !string.IsNullOrEmpty(ddlClientName.SelectedValue);
            ddlProjectManager.Enabled = (userIsAdministrator || userIsDirector || userIsSeniorLeadership || userIsSalesPerson);

            if (userIsPracticeManager && !ddlProjectManager.Enabled && !ProjectId.HasValue)
            {
                try
                {
                    ddlProjectManager.SelectedValue = DataHelper.CurrentPerson.Id.ToString();
                }
                catch
                {
                    ddlProjectManager.SelectedValue = string.Empty;
                }
            }

            ddlProjectGroup.Enabled = !string.IsNullOrEmpty(ddlClientName.SelectedValue) && !isReadOnly;

            ddlProjectStatus.Enabled =
                // add new project mode
                !ProjectId.HasValue ||
                // Administrators always can change the project status
                userIsAdministrator ||
                // Practice Managers and Sales persons can change the status from Experimental, Inactive or Projected
                IsStatusValidForNonadmin(Project);

            foreach (ListItem item in rlstManagementCommission.Items)
            {
                item.Enabled = !isReadOnly;
            }
            btnAddMilistone.Visible = btnSave.Visible = !isReadOnly;

            #endregion

            AllowContinueWithoutSave = ProjectId.HasValue;

            if (IsPostBack && IsValidated)
            {
                Page.Validate();
            }

            txtSalesCommission.Enabled = !string.IsNullOrEmpty(ddlSalesperson.SelectedValue);
            chbReceivesSalesCommission.Enabled =
                chbReceivesSalesCommission.Enabled && !string.IsNullOrEmpty(ddlSalesperson.SelectedValue);

            NeedToShowDeleteButton();
        }

        private void NeedToShowDeleteButton()
        {
            if (ProjectId.HasValue && Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName))
            {
                btnDelete.Visible = true;

                if (Project.Status.Id == 1 || Project.Status.Id == 5)//Status Ids 1 :-Inactive and 5:- Experimental.
                {
                    btnDelete.Enabled = true;
                }
                else
                {
                    btnDelete.Enabled = false;
                }
            }
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
                AttachmentService.AttachmentService svc = Utils.WCFClientUtility.GetAttachmentService();

                svc.DeleteProjectAttachmentByProjectId(id, ProjectId.Value, User.Identity.Name);
                Project.Attachments.Remove(Project.Attachments.Find(pa => pa.AttachmentId == id));

                PopulateAttachmentControl(Project);
            }
            else
            {
                var list = AttachmentsForNewProject;
                list.Remove(list.Find(pa => pa.AttachmentId == id));

                AttachmentsForNewProject = list;
                gvProjectAttachments.DataSource = list;
            }

            gvProjectAttachments.DataBind();

        }

        protected void btnAddMilistone_Click(object sender, EventArgs e)
        {
            if (!ProjectId.HasValue)
            {
                Page.Validate();
                if (Page.IsValid)
                {
                    int projectId = SaveData();

                    Redirect(Constants.ApplicationPages.MilestoneDetail + GetProjectIdArgument(false, projectId),
                        projectId.ToString());
                }
            }
            else if (!SaveDirty || ValidateAndSave())
            {
                Redirect(Constants.ApplicationPages.MilestoneDetail + GetProjectIdArgument(false, ProjectId.Value),
                    ProjectId.Value.ToString());
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateAndSave())
            {
                ClearDirty();
                mlConfirmation.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Project"));
            }

            if (Page.IsValid && ProjectId.HasValue)
            {
                Project = GetCurrentProject(ProjectId);
                if (Project != null)
                    PopulateControls(Project);
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            Page.Validate(vsumProjectAttachment.ValidationGroup);
            if (Page.IsValid)
            {
                AttachmentService.ProjectAttachment attachment = PopulateProjectAttachment();

                if (ProjectId.HasValue)
                {
                    AttachmentService.AttachmentService svc = Utils.WCFClientUtility.GetAttachmentService();
                    svc.SaveProjectAttachment(attachment, ProjectId.Value, User.Identity.Name);
                }
                else
                {
                    attachment.UploadedDate = DateTime.Now;

                    if (AttachmentsForNewProject == null)
                    {
                        AttachmentsForNewProject = new List<AttachmentService.ProjectAttachment>();
                    }

                    attachment.AttachmentId = AttachmentsForNewProject.Count + 1;
                    AttachmentsForNewProject.Add(attachment);

                    gvProjectAttachments.DataSource = AttachmentsForNewProject;
                    gvProjectAttachments.DataBind();
                }
            }

            if (Page.IsValid && ProjectId.HasValue)
            {
                Project = GetCurrentProject(ProjectId);
                if (Project != null)
                    PopulateAttachmentControl(Project);
            }
            btnUpload.Enabled = false;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (hdnProjectDelete.Value == "1")
            {
                using (var serviceClient = new ProjectService.ProjectServiceClient())
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
                    }
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
            }

        }

        private void SetDefaultsToClientDependancyControls()
        {
            chbIsChargeable.Checked = false;
            txtClientDiscount.Text = string.Empty;
            ddlSalesperson.SelectedIndex = 0;
            ddlDirector.SelectedIndex = 0;

            if (Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName)
                && !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName)
                && !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName)
                  && !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName)
                && !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SeniorLeadershipRoleName))
            {
                ddlSalesperson.SelectedValue = DataHelper.CurrentPerson.Id.ToString();
            }

            if (ddlProjectGroup.Items.Count > 0)
                ddlProjectGroup.SelectedIndex = 0;

            ddlProjectGroup.Enabled = ddlDirector.Enabled = ddlSalesperson.Enabled = false;
        }

        private void SetClientDefaultValues(int clientId)
        {
            try
            {
                Client client = GetClientByClientId(clientId);

                if (client != null)
                {
                    txtClientDiscount.Text =
                        client.DefaultDiscount != 0 ? client.DefaultDiscount.ToString() : string.Empty;

                    chbIsChargeable.Checked = client.IsChargeable;


                    if (Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName)
                        && !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName)
                        && !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName)
                         && !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SeniorLeadershipRoleName))
                    {
                        ddlSalesperson.SelectedValue = DataHelper.CurrentPerson.Id.ToString();
                    }
                    else
                    {
                        ddlSalesperson.SelectedIndex =
                            ddlSalesperson.Items.IndexOf(ddlSalesperson.Items.FindByValue(client.DefaultSalespersonId.ToString()));
                        ddlSalesperson_SelectedIndexChanged(ddlSalesperson, EventArgs.Empty);
                    }

                    //  If it's a new project, change chargeability when client is changed
                    if (!ProjectId.HasValue)
                    {
                        chbIsChargeable.Checked = client.IsChargeable;
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
                }

                DataHelper.FillProjectGroupList(ddlProjectGroup, clientId, null);
                if (ProjectId.HasValue)
                {
                    var listItemValue = this.Project != null && this.Project.Group != null && this.Project.Group.Id.HasValue ?
                            ProjectId.Value.ToString() : "";
                    var listItem = ddlProjectGroup.Items.FindByValue(listItemValue);
                    if (listItem != null)
                        ddlProjectGroup.SelectedIndex = ddlProjectGroup.Items.IndexOf(listItem);
                }
            }
            catch (FaultException<ExceptionDetail>)
            {
                throw;
            }

            ddlClientName.Focus();
        }

        private static Client GetClientByClientId(int clientId)
        {
            using (ClientServiceClient serviceClient = new ClientServiceClient())
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

        protected void ddlProjectGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void DropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Only set an input focus
            ((Control)sender).Focus();
        }


        /// <summary>
        /// Sets the salesperson commission
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlSalesperson_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlSalesperson.SelectedValue))
            {
                int personId = int.Parse(ddlSalesperson.SelectedValue);
                using (DefaultCommissionServiceClient serviceClient = new DefaultCommissionServiceClient())
                {
                    try
                    {
                        DefaultCommission commission = serviceClient.DefaultSalesCommissionByPerson(personId);
                        if (commission != null)
                        {
                            txtSalesCommission.Text = commission.FractionOfMargin.ToString();
                            chbReceivesSalesCommission.Checked = true;
                            txtSalesCommission.Enabled = true;
                        }
                    }
                    catch (FaultException<ExceptionDetail>)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
            }

            ddlSalesperson.Focus();
        }

        protected void chbReceivesSalesCommission_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSalesCommissionState();

            IsDirty = true;
            chbReceivesSalesCommission.Focus();
        }

        protected void cstSalesCommission_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string regex = @"[1-9]?\d(\.\d{1,2})?";
            RegexStringValidator rsv = new RegexStringValidator(regex);
            try
            {
                rsv.Validate(args.Value);
            }
            catch
            {
                args.IsValid = false;
                cstSalesCommission.ErrorMessage = cstSalesCommission.ToolTip = "A number with 2 decimal digits is allowed for the Sales Commission.";
            }
            if (args.IsValid && Convert.ToDecimal(args.Value) <= 0)
            {
                args.IsValid = false;
                cstSalesCommission.ErrorMessage = cstSalesCommission.ToolTip = "The Sales Commission must be greater than zero.";
            }
        }

        protected void cstManagementCommission_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string regex = @"[1-9]?\d(\.\d{1,2})?";
            RegexStringValidator rsv = new RegexStringValidator(regex);
            try
            {
                rsv.Validate(args.Value);
            }
            catch
            {
                args.IsValid = false;
                cstManagementCommission.ErrorMessage = cstManagementCommission.ToolTip = "A number with 2 decimal digits is allowed for the Management Commission.";
            }
            if (args.IsValid && Convert.ToDecimal(args.Value) <= 0)
            {
                args.IsValid = false;
                cstManagementCommission.ErrorMessage = cstManagementCommission.ToolTip = "The Management Commission must be greater than zero.";
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

        protected void cvProjectAttachment_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (fuProjectAttachment.HasFile)
            {
                string extension = System.IO.Path.GetExtension(fuProjectAttachment.FileName);
                if (!(extension.ToLowerInvariant() == ".pdf"))
                {
                    args.IsValid = false;
                }
            }
        }

        protected void cvalidatorProjectAttachment_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (fuProjectAttachment.HasFile)
            {
                string extension = System.IO.Path.GetExtension(fuProjectAttachment.FileName);
                if (extension.ToLowerInvariant() == ".pdf")
                {
                    int size = Convert.ToInt32(SettingsHelper.GetResourceValueByTypeAndKey(SettingsType.Project, AttachmentFileSize));

                    if (fuProjectAttachment.PostedFile.ContentLength > size)
                    {
                        int kbSize = (int)size / 1024;
                        cvalidatorProjectAttachment.ErrorMessage = "File Must be less than " + kbSize + " Kb.";
                        cvalidatorProjectAttachment.ToolTip = "File Must be less than " + kbSize + " Kb.";
                        args.IsValid = false;
                    }
                }
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

        protected override bool ValidateAndSave()
        {
            bool result = false;
            Page.Validate(vsumProject.ValidationGroup);
            if (Page.IsValid)
            {
                int? id = SaveData();
                if (id.HasValue)
                {
                    this.ProjectId = id;
                    projectExpenses.BindExpenses();
                }
                result = true;
            }

            return result;
        }

        private void UpdateSalesCommissionState()
        {
            txtSalesCommission.Enabled = reqSalesCommission.Enabled =
                cstSalesCommission.Enabled =
                chbReceivesSalesCommission.Checked;
        }

        protected void chbReceiveManagementCommission_CheckedChanged(object sender, EventArgs e)
        {
            UpdateManagementCommissionState();

            IsDirty = true;
            chbReceiveManagementCommission.Focus();
        }

        private void UpdateManagementCommissionState()
        {
            txtManagementCommission.Enabled = reqManagementCommission.Enabled =
                cstManagementCommission.Enabled =
                rlstManagementCommission.Enabled =
                chbReceiveManagementCommission.Checked && chbReceiveManagementCommission.Enabled;
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
            Project project = new Project();
            PopulateData(project);
            int result = -1;
            using (ProjectServiceClient serviceClient = new ProjectServiceClient())
            {
                try
                {
                    result = serviceClient.SaveProjectDetail(project, User.Identity.Name);

                }
                catch (CommunicationException ex)
                {
                    serviceClient.Abort();
                    throw;
                }
            }

            if (AttachmentsForNewProject != null && result != -1)
            {
                AttachmentService.AttachmentService svc = Utils.WCFClientUtility.GetAttachmentService();
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
            DataHelper.FillPracticeWithOwnerListOnlyActive(ddlPractice, string.Empty);
            DataHelper.FillClientListForProject(ddlClientName, "-- Select Client --", ProjectId);
            DataHelper.FillSalespersonListOnlyActive(ddlSalesperson, "-- Select Sales person --");
            DataHelper.FillProjectStatusList(ddlProjectStatus, string.Empty);
            DataHelper.FillDirectorsList(ddlDirector, "-- Select Client Director --");
            DataHelper.FillOwnersList(ddlProjectManager, "-- Select Owner --");

            int? id = ProjectId;
            if (id.HasValue)
            {
                Project = GetCurrentProject(id);

                if (Project != null)
                    PopulateControls(Project);
            }
            else
            {

                // Default values for new projects.
                bool userIsAdministrator =
                    Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);

                ddlProjectStatus.SelectedIndex =
                       ddlProjectStatus.Items.IndexOf(
                       ddlProjectStatus.Items.FindByValue(((int)ProjectStatusType.Projected).ToString()));

                if (!userIsAdministrator)
                {
                    ddlProjectStatus.Enabled = false;
                }
            }

            UpdateSalesCommissionState();
            UpdateManagementCommissionState();
        }

        private Project GetCurrentProject(int? id)
        {
            Project project;
            using (ProjectServiceClient serviceClient = new ProjectServiceClient())
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
            txtProjectName.Text = project.Name;
            lblProjectNumber.Text = project.ProjectNumber;
            chbIsChargeable.Checked = project.IsChargeable;

            PopulateClientDropDown(project);
            FillAndSelectProjectGroupList(project);
            PopulatePracticeDropDown(project);
            SelectProjectStatus(project);
            ShowStartAndEndDate(project);
            PopulateProjectManagerDropDown(project);

            financials.Project = project;

            txtClientDiscount.Text =
                project.Discount != 0 ? project.Discount.ToString() : string.Empty;
            txtBuyerName.Text = project.BuyerName;

            DisplaySalesCommissions(project);

            DisplayPracticeManagementCommissions(project);

            // Billing info
            billingInfo.Info =
                ServiceCallers.Invoke<ProjectServiceClient, BillingInfo>
                    (client => client.GetProjectBillingInfo(project.Id.Value));

            if (project.OpportunityId.HasValue)
            {
                lbOpportunity.Visible = true;
                lbOpportunity.NavigateUrl =
                    Generic.GetTargetUrlWithReturn(
                        string.Format(
                            Constants.ApplicationPages.DetailRedirectFormat,
                            Constants.ApplicationPages.OpportunityDetail,
                            project.OpportunityId.Value),
                        Request.Url.AbsoluteUri);
            }

            if (project.Director != null && project.Director.Id.HasValue)
            {
                ListItem selectedDirector = ddlDirector.Items.FindByValue(project.Director.Id.Value.ToString());
                if (selectedDirector == null)
                {
                    Person selectedPerson = DataHelper.GetPerson(project.Director.Id.Value);
                    selectedDirector = new ListItem(selectedPerson.PersonLastFirstName, selectedPerson.Id.Value.ToString());
                    ddlDirector.Items.Add(selectedDirector);
                    ddlDirector.SortByText();
                }

                ddlDirector.SelectedValue = selectedDirector.Value;
            }

            PopulateAttachmentControl(project);
        }

        private void PopulateAttachmentControl(Project project)
        {
            if (project.Attachments != null && project.Attachments.Count > 0)
            {
                gvProjectAttachments.DataSource = project.Attachments;
            }
            gvProjectAttachments.DataBind();
        }

        public string GetNavigateUrl(string attachmentFileName, int attachmentId)
        {
            if (Project != null && Project.Id.HasValue)
            {
                return string.Format(ProjectAttachmentHandlerUrl, Project.Id.ToString(), attachmentFileName, attachmentId);
            }
            return string.Empty;
        }

        public bool IsProjectCreated()
        {
            return (Project == null || !Project.Id.HasValue);
        }

        private void PopulateSalesPersonDropDown()
        {
            Project = GetCurrentProject(ProjectId);

            if (Project != null && Project.SalesCommission != null)
            {
                ListItem selectedSalesPerson = ddlSalesperson.Items.FindByValue(Project.SalesCommission[0].PersonId.Value.ToString());
                if (selectedSalesPerson == null)
                {
                    Person selectedPerson = DataHelper.GetPerson(Project.SalesCommission[0].PersonId.Value);

                    selectedSalesPerson = new ListItem(selectedPerson.PersonLastFirstName, selectedPerson.Id.Value.ToString());
                    ddlSalesperson.Items.Add(selectedSalesPerson);
                    ddlSalesperson.SortByText();
                }

                ddlSalesperson.SelectedValue = selectedSalesPerson.Value;

            }
        }

        private void PopulateProjectManagerDropDown(Project project)
        {
            if (project.ProjectManager.Id != null)
            {
                var managerId = project.ProjectManager.Id.Value.ToString();
                ListItem selectedManager = ddlProjectManager.Items.FindByValue(managerId);
                if (selectedManager == null)
                {
                    selectedManager = new ListItem(project.ProjectManager.PersonLastFirstName, managerId);
                    ddlProjectManager.Items.Add(selectedManager);
                    ddlProjectManager.SortByText();
                }

                ddlProjectManager.SelectedValue = selectedManager.Value;
            }
            else
            {
                ddlProjectManager.SelectedValue = string.Empty;
            }
        }

        private void DisplayPracticeManagementCommissions(Project project)
        {
            // Practice Management Commissions
            if (project.ManagementCommission != null)
            {
                chbReceiveManagementCommission.Checked = project.ManagementCommission.FractionOfMargin != 0;
                txtManagementCommission.Text = project.ManagementCommission.FractionOfMargin.ToString();
                rlstManagementCommission.SelectedIndex =
                    rlstManagementCommission.Items.IndexOf(rlstManagementCommission.Items.FindByValue(
                    project.ManagementCommission.MarginTypeId.HasValue ?
                    project.ManagementCommission.MarginTypeId.Value.ToString() : string.Empty));
                hidPracticeManagementCommissionId.Value = Convert.ToString(project.ManagementCommission.Id);
            }
        }

        private void DisplaySalesCommissions(Project project)
        {
            // Sales Commissions
            if (project.SalesCommission != null && project.SalesCommission.Count > 0)
            {
                PopulateSalesPersonDropDown();

                chbReceivesSalesCommission.Checked = project.SalesCommission[0].FractionOfMargin != 0;
                txtSalesCommission.Text = project.SalesCommission[0].FractionOfMargin.ToString();
                hidSalesCommissionId.Value = Convert.ToString(project.SalesCommission[0].Id);
            }
        }

        private void ShowStartAndEndDate(Project project)
        {
            if (project.StartDate.HasValue)
            {
                lblProjectStart.Text = project.StartDate.Value.ToString("MM/dd/yyyy");
            }
            if (project.EndDate.HasValue)
            {
                lblProjectEnd.Text = project.EndDate.Value.ToString("MM/dd/yyyy");
            }
        }

        private void SelectProjectStatus(Project project)
        {
            ddlProjectStatus.SelectedIndex =
                ddlProjectStatus.Items.IndexOf(
                ddlProjectStatus.Items.FindByValue(
                project.Status != null ? project.Status.Id.ToString() : string.Empty));
        }

        private void PopulatePracticeDropDown(Project project)
        {
            ListItem selectedPractice = null;
            if (project != null && project.Practice != null)
            {
                selectedPractice = ddlPractice.Items.FindByValue(project.Practice.Id.ToString());
            }

            // For situation, when disabled practice is assigned to project.
            if (selectedPractice == null)
            {
                selectedPractice = new ListItem(string.Format("{0} ({1})", project.Practice.Name, project.Practice.PracticeOwnerName), project.Practice.Id.ToString());
                ddlPractice.Items.Add(selectedPractice);
                ddlPractice.SortByText();
            }

            ddlPractice.SelectedValue = selectedPractice.Value;
        }

        private void PopulateClientDropDown(Project project)
        {
            ListItem selectedClient = null;
            if (project != null && project.Client != null)
            {
                selectedClient = ddlClientName.Items.FindByValue(project.Client.Id.ToString());
            }

            // For situation, when disabled practice is assigned to project.
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
                PopulateProjectGroupDropDown(project);
            }
        }

        private void PopulateProjectGroupDropDown(Project project)
        {
            if (project != null && project.Group != null)
            {
                ListItem selectedProjectGroup = ddlProjectGroup.Items.FindByValue(project.Group.Id.ToString());
                if (selectedProjectGroup == null)
                {
                    selectedProjectGroup = new ListItem(project.Group.Name, project.Group.Id.ToString());
                    ddlProjectGroup.Items.Add(selectedProjectGroup);
                }

                ddlProjectGroup.SelectedValue = selectedProjectGroup.Value;
            }
        }

        private void PopulateData(Project project)
        {
            project.Id = ProjectId;
            project.Name = txtProjectName.Text;
            project.Discount =
                !string.IsNullOrEmpty(txtClientDiscount.Text.Trim()) ? decimal.Parse(txtClientDiscount.Text) : 0;
            project.BuyerName = txtBuyerName.Text;
            project.Client = new Client { Id = int.Parse(ddlClientName.SelectedValue) };
            project.Practice = new Practice { Id = int.Parse(ddlPractice.SelectedValue) };
            project.Status = new ProjectStatus { Id = int.Parse(ddlProjectStatus.SelectedValue) };
            project.ProjectManager = new Person { Id = int.Parse(ddlProjectManager.SelectedValue) };
            project.IsChargeable = chbIsChargeable.Checked;
            PopulateProjectGroup(project);
            PopulateSalesCommission(project);
            PopulatePracticeManagementCommission(project);
            project.BillingInfo = billingInfo.Info;
            if (ddlDirector.SelectedIndex > 0)
                project.Director = new Person { Id = int.Parse(ddlDirector.SelectedValue) };

        }

        private AttachmentService.ProjectAttachment PopulateProjectAttachment()
        {
            if (fuProjectAttachment.HasFile)
            {
                AttachmentService.ProjectAttachment attachment = new AttachmentService.ProjectAttachment();
                attachment.AttachmentFileName = fuProjectAttachment.FileName;
                attachment.AttachmentData = fuProjectAttachment.FileBytes;
                attachment.AttachmentSize = fuProjectAttachment.FileBytes.Length;
                return attachment;
            }
            return null;
        }

        private void PopulatePracticeManagementCommission(Project project)
        {
            // Practice Management Commissions
            project.ManagementCommission = new Commission();
            project.ManagementCommission.TypeOfCommission = CommissionType.PracticeManagement;
            if (!string.IsNullOrEmpty(txtManagementCommission.Text) && chbReceiveManagementCommission.Checked)
            {
                project.ManagementCommission.FractionOfMargin = decimal.Parse(txtManagementCommission.Text);
            }
            project.ManagementCommission.MarginTypeId = int.Parse(rlstManagementCommission.SelectedValue);
            project.ManagementCommission.Id =
                !string.IsNullOrEmpty(hidPracticeManagementCommissionId.Value) ?
                (int?)int.Parse(hidPracticeManagementCommissionId.Value) : null;

            var practiceList = DataHelper.GetPracticeById(project.Practice.Id);

            if (practiceList.Length > 0)
            {
                project.ManagementCommission.PersonId = practiceList[0].PracticeOwner.Id;
            }
        }

        private void PopulateSalesCommission(Project project)
        {
            // Sales Commissions
            Commission salesCommission = new Commission();
            salesCommission.TypeOfCommission = CommissionType.Sales;
            if (!string.IsNullOrEmpty(ddlSalesperson.SelectedValue))
            {
                salesCommission.PersonId = int.Parse(ddlSalesperson.SelectedValue);
            }
            if (chbReceivesSalesCommission.Checked && !string.IsNullOrEmpty(txtSalesCommission.Text))
            {
                salesCommission.FractionOfMargin = decimal.Parse(txtSalesCommission.Text);
            }

            salesCommission.Id =
                !string.IsNullOrEmpty(hidSalesCommissionId.Value) ? (int?)int.Parse(hidSalesCommissionId.Value) : null;
            project.SalesCommission = new List<Commission>() { salesCommission };
        }

        private void PopulateProjectGroup(Project project)
        {
            project.Group = new ProjectGroup { Id = int.Parse(ddlProjectGroup.SelectedValue) };
        }

        private static string GetProjectIdArgument(bool useAmpersand, int projectId)
        {
            return (useAmpersand ? "&" : "?") + string.Format(ProjectIdFormat, projectId);
        }

        protected void rlstManagementCommission_SelectedIndexChanged(object sender, EventArgs e)
        {
            IsDirty = true;
            rlstManagementCommission.Focus();
        }

        protected void nOpportunity_OnNoteAdded(object source, EventArgs args)
        {
            activityLog.Update();
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);
            SelectView((Control)sender, viewIndex, false);

            if (viewIndex == 6) //History
            {
                activityLog.Update();
            }
            else if (viewIndex == 0 && ProjectId.HasValue)
            {
                financials.Project = GetCurrentProject(ProjectId.Value);
            }

        }

        private void SelectView(Control sender, int viewIndex, bool selectOnly)
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
            var id = Convert.ToInt32(((LinkButton)sender).CommandName);
            var list = AttachmentsForNewProject;
            var attachment = list[list.FindIndex(pa => pa.AttachmentId == id)];

            string FileName = attachment.AttachmentFileName;
            byte[] attachmentData = attachment.AttachmentData;

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "Application/pdf";
            HttpContext.Current.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}", FileName));

            int len = attachmentData.Length;
            int bytes;
            byte[] buffer = new byte[1024];

            Stream outStream = HttpContext.Current.Response.OutputStream;
            using (MemoryStream stream = new MemoryStream(attachmentData))
            {
                while (len > 0 && (bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outStream.Write(buffer, 0, bytes);
                    HttpContext.Current.Response.Flush();
                    len -= bytes;
                }
            }
            HttpContext.Current.Response.End();
        }

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
        }

        protected void lnkClone_OnClick(object sender, EventArgs e)
        {
            var clonedId =
                DataHelper.CloneProject(
                    new ProjectCloningContext
                    {
                        Project = new Project { Id = ProjectId },
                        CloneMilestones = chbCloneMilestones.Checked,
                        CloneCommissions = chbCloneCommissions.Checked,
                        CloneBillingInfo = chbCloneBillingInfo.Checked,
                        CloneNotes = chbCloneNotes.Checked,
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

        public string GetWrappedText(string name)
        {
            if (name.Length > 50)
            {
                for (int i = 50; i < name.Length; i = i + 50)
                {
                    name = name.Insert(i, "<wbr />");
                }
            }
            return name;
        }

        #endregion

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
                case PraticeManagement.Controls.Projects.ProjectMilestonesFinancials.MILESTONE_TARGET:
                    SaveAndRedirectToMilestone(args[1]);
                    break;

                case PraticeManagement.Controls.Projects.ProjectPersons.PERSON_TARGET:
                    SaveAndRedirectToMilestone(args[1]);
                    break;

                default:
                    if (!SaveDirty || ValidateAndSave())
                        Redirect(eventArgument);
                    break;
            }
        }

        #endregion
    }
}


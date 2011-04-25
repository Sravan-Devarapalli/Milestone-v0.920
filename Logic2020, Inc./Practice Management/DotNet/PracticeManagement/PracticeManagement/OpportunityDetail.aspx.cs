using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Web.Security;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.Controls;
using PraticeManagement.OpportunityService;
using PraticeManagement.ProjectService;
using PraticeManagement.Utils;
using System.Web.UI;
using System.Linq;
using System.Collections;

namespace PraticeManagement
{
    public partial class OpportunityDetail : PracticeManagementPageBase, IPostBackEventHandler
    {
        #region Fields

        private bool _userIsAdministrator;
        private bool _userIsRecruiter;
        private bool _userIsHR;
        private bool _userIsSalesperson;
        // private const string DuplicateOpportunityName = "There is another opportunity with same name.";

        #endregion

        #region Properties

        private const int WonProjectId = 4;

        //private String ExMessage { get; set; }

        /// <summary>
        /// 	Gets a selected opportunity
        /// </summary>
        private Opportunity Opportunity
        {
            get
            {
                if (OpportunityId.HasValue)
                {
                    using (var serviceClient = new OpportunityServiceClient())
                    {
                        try
                        {
                            return serviceClient.GetById(OpportunityId.Value);
                        }
                        catch (CommunicationException)
                        {
                            serviceClient.Abort();
                            throw;
                        }
                    }
                }
                return null;
            }
        }

        private int? OpportunityId
        {
            get
            {
                if (SelectedId.HasValue)
                {
                    return SelectedId;
                }
                else
                {
                    int id;
                    if (Int32.TryParse(hdnOpportunityId.Value, out id))
                    {
                        return id;
                    }

                    return null;
                }
            }
            set
            {
                hdnOpportunityId.Value = value.ToString();
            }
        }

        #endregion

        #region Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.FillClientList(ddlClient, string.Empty);
                DataHelper.FillSalespersonListOnlyActive(ddlSalesperson, string.Empty);
                DataHelper.FillOpportunityStatusList(ddlStatus, string.Empty);

                DataHelper.FillPracticeListOnlyActive(ddlPractice, string.Empty);

                activityLog.OpportunityId = OpportunityId;

                otePipeline.Visible =
                    oteProposed.Visible =
                    oteSendOut.Visible =
                        nOpportunity.Visible =
                            OpportunityId.HasValue;
                if (OpportunityId.HasValue)
                {
                    otePipeline.OpportunityId = OpportunityId.Value;
                    oteProposed.OpportunityId = OpportunityId.Value;
                    oteSendOut.OpportunityId = OpportunityId.Value;

                    tpHistory.Visible = true;
                    tpTools.Visible = true;
                }
            }

            mlConfirmation.ClearMessage();
            // Security
            InitSecurity();
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (IsPostBack && OpportunityId.HasValue)
            {
                activityLog.Update();
            }
        }

        private void InitSecurity()
        {
            var roles = new List<string>(Roles.GetRolesForUser());

            _userIsAdministrator =
                roles.Contains(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            roles.Contains(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName);
            _userIsSalesperson =
                roles.Contains(DataTransferObjects.Constants.RoleNames.SalespersonRoleName);
            _userIsRecruiter =
                roles.Contains(DataTransferObjects.Constants.RoleNames.RecruiterRoleName);
            _userIsHR =
                roles.Contains(DataTransferObjects.Constants.RoleNames.HRRoleName);
        }

        protected void custTransitionStatus_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            int statusId;
            e.IsValid =
                !int.TryParse(e.Value, out statusId) || statusId != (int)OpportunityTransitionStatusType.Lost ||
                // Only Administratos, Salesperson or Practice Manager can set the status to Lost.
                _userIsAdministrator || _userIsSalesperson;
        }

        protected void custEstRevenue_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = true;
            Decimal result;
            bool isDecimal = Decimal.TryParse(txtEstRevenue.Text, out result);

            if (isDecimal)
            {
                if (result < 1000)
                {
                    e.IsValid = false;
                }
            }
        }


        protected void custEstimatedRevenue_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = true;
            Decimal result;
           string revenueText = txtEstRevenue.Text;

           bool isDecimal = Decimal.TryParse(revenueText, out result);

            if (!isDecimal)
            {
                e.IsValid = false;
            }
        }

        /// <summary>
        /// 	Creates a project fron the opportunity.
        /// </summary>
        protected void btnConvertToProject_Click(object sender, EventArgs e)
        {
            Page.Validate(vsumWonConvert.ValidationGroup);
            if (!Page.IsValid)
            {
                return;
            }

            var opportunity = Opportunity;

            if (opportunity == null)
            {
                custOpportunityNotSaved.IsValid = false;
            }
            else if (CanUserEditOpportunity(opportunity))
            {
                if (IsDirty)
                {
                    if (!SaveDirty)
                    {
                        Display();
                    }
                    else if (!ValidateAndSave())
                    {
                        return;
                    }
                }

                Page.Validate(vsumWonConvert.ValidationGroup);
                if (Page.IsValid)
                {
                    using (var serviceClient = new OpportunityServiceClient())
                    {
                        try
                        {
                            var projectId = serviceClient.OpportunityConvertToProject(OpportunityId.Value,
                                                                                      User.Identity.Name);

                            Response.Redirect(
                                Urls.GetProjectDetailsUrl(projectId, Request.Url.AbsoluteUri));
                        }
                        catch (CommunicationException)
                        {
                            serviceClient.Abort();
                            throw;
                        }
                    }
                }
                else
                {
                    // Display a validation error dialog
                    var sb = new StringBuilder();
                    if (!reqBuyerName.IsValid)
                    {
                        sb.AppendFormat("{0}\\n", reqBuyerName.ErrorMessage);
                    }
                    if (!reqProjectedStartDate.IsValid)
                    {
                        sb.AppendFormat("{0}\\n", reqProjectedStartDate.ErrorMessage);
                    }
                    if (!reqProjectedEndDate.IsValid)
                    {
                        sb.AppendFormat("{0}\\n", reqProjectedEndDate.ErrorMessage);
                    }
                    if (!custWonConvert.IsValid)
                    {
                        sb.AppendFormat("{0}\\n", custWonConvert.ErrorMessage);
                    }

                    ltrWonConvertInvalid.Text = string.Format(ltrWonConvertInvalid.Text, sb);
                    ltrWonConvertInvalid.Visible = true;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool IsSavedWithoutErrors = ValidateAndSave();
            activityLog.Update();
            if (IsSavedWithoutErrors)
            {
                mlConfirmation.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Opportunity"));
                ClearDirty();
            }
            if (Page.IsValid && OpportunityId.HasValue)
            {
                FillControls();
            }
        }

        protected override bool ValidateAndSave()
        {
            var retValue = false;

            Page.Validate(vsumOpportunity.ValidationGroup);
            if (Page.IsValid)
            {
                var opportunity = new Opportunity();
                PopulateData(opportunity);

                using (var serviceClient = new OpportunityServiceClient())
                {
                    try
                    {
                        int? id = serviceClient.OpportunitySave(opportunity, User.Identity.Name);

                        //return true;

                        if( string.IsNullOrEmpty(ddlProjects.SelectedValue) 
                            ||   ddlProjects.SelectedValue == "-1"
                          )
                        {
                            IsDirty = false;
                        }

                        if (id.HasValue)
                        {
                            OpportunityId = id;
                        }

                        retValue = true;
                    }
                    catch (CommunicationException ex)
                    {
                        serviceClient.Abort();
                        // ExMessage = ex.Message;
                        throw;
                        //Page.Validate(vsumOpportunity.ValidationGroup);
                    }
                }
            }

            return retValue;
        }

        protected override void Display()
        {
            FillControls();
        }

        private void FillControls()
        {
            if (OpportunityId.HasValue)
            {
                var opportunity = Opportunity;

                if (opportunity != null)
                {

                    PopulateControls(opportunity);

                    UpdateState(opportunity);
                }
            }
        }

        /// <summary>
        /// 	Updates control state for the security reasons.
        /// </summary>
        /// <param name = "opportunity">An opportunity is being displayed.</param>
        private void UpdateState(Opportunity opportunity)
        {
            // Security
            var canEdit = CanUserEditOpportunity(opportunity);

            var isStatusReadOnly =
                !canEdit ||
                (!_userIsAdministrator && !_userIsSalesperson && _userIsRecruiter) ||
                (!_userIsAdministrator && !_userIsSalesperson && _userIsHR); //#2817: this line is added asper requirement.

            txtOpportunityName.ReadOnly =
                  txtBuyerName.ReadOnly =
                  txtDescription.ReadOnly =
                  dpProjectedStartDate.ReadOnly =
                  dpProjectedEndDate.ReadOnly =
                  txtPipeline.ReadOnly =
                  txtProposed.ReadOnly =
                  txtSendOut.ReadOnly =
                  txtEstRevenue.ReadOnly =
                /*txtReviewOrder.ReadOnly =*/
                                         !canEdit;

            ddlClient.Enabled =
                ddlPriority.Enabled =
                ddlSalesperson.Enabled =
                ddlPractice.Enabled =
                btnSave.Enabled =
                dfOwner.Enabled =
                btnConvertToProject.Enabled =
                otePipeline.Enabled =
                oteProposed.Enabled =
                oteSendOut.Enabled =
                /*cddClientGroups.Enabled =
                cddClientProjects.Enabled =*/ canEdit;

            ddlClientGroup.Visible =
                ddlProjects.Visible = canEdit;

            ddlStatus.Enabled = !isStatusReadOnly;

            if (!canEdit)
                btnConvertToProject.OnClientClick = Resources.Controls.OpportunityInappropriatePersonMessage;

            lblReadOnlyWarning.Visible = !canEdit;
        }

        /// <summary>
        /// 	Determine whether the current user can convert the specified opportunity
        /// </summary>
        /// <param name = "opportunity">The <see cref = "Opportunity" /> to be checked for.</param>
        /// <returns>true if the user can convert the <see cref = "Opportunity" /> and false otherwise.</returns>
        private bool CanUserEditOpportunity(Opportunity opportunity)
        {
            var current = DataHelper.CurrentPerson;

            var isOwnerOrPracticeManagerOrTheSamePractice =
                current != null && current.Id.HasValue && // Current is not null
                // Current is Opportunity Owner
                (opportunity.Owner != null && current.Id == opportunity.Owner.Id
                    ||
                // or current is practice manager
                opportunity.Practice.PracticeOwner != null && opportunity.Practice.PracticeOwner.Id == current.Id
                    ||
                // or current is of the same practice
                current.DefaultPractice != null && opportunity.Practice != null && current.DefaultPractice == opportunity.Practice);

            return _userIsAdministrator || isOwnerOrPracticeManagerOrTheSamePractice || _userIsRecruiter || _userIsSalesperson || _userIsHR;//#2817: _userIsHR is added as per requirement.
        }

        private void PopulateControls(Opportunity opportunity)
        {
            txtEstRevenue.Text = opportunity.EstimatedRevenue != null ? opportunity.EstimatedRevenue.Value.ToString("###,###,###,###,##0") : string.Empty;
            txtOpportunityName.Text = opportunity.Name;
            lblOpportunityNumber.Text = opportunity.OpportunityNumber;
            dpProjectedStartDate.DateValue =
                opportunity.ProjectedStartDate.HasValue
                    ? opportunity.ProjectedStartDate.Value
                    : DateTime.MinValue;
            dpProjectedEndDate.DateValue =
                opportunity.ProjectedEndDate.HasValue
                    ? opportunity.ProjectedEndDate.Value
                    : DateTime.MinValue;
            txtDescription.Text = opportunity.Description;
            txtBuyerName.Text = opportunity.BuyerName;
            txtPipeline.Text = opportunity.Pipeline;
            txtProposed.Text = opportunity.Proposed;
            txtSendOut.Text = opportunity.SendOut;
            /*txtReviewOrder.Text = opportunity.OpportunityIndex.HasValue
                                      ? opportunity.OpportunityIndex.Value.ToString()
                                      : string.Empty;*/
            lblLastUpdate.Text = opportunity.LastUpdate.ToShortDateString();

            ddlStatus.SelectedIndex =
                ddlStatus.Items.IndexOf(
                    ddlStatus.Items.FindByValue(
                        opportunity.Status != null ? opportunity.Status.Id.ToString() : string.Empty));
            ddlClient.SelectedIndex =
                ddlClient.Items.IndexOf(
                    ddlClient.Items.FindByValue(
                        opportunity.Client != null && opportunity.Client.Id.HasValue
                            ? opportunity.Client.Id.Value.ToString()
                            : string.Empty));
            ddlPriority.SelectedIndex =
                ddlPriority.Items.IndexOf(
                    ddlPriority.Items.FindByValue(opportunity.Priority.ToString()));

            PopulateSalesPersonDropDown();

            PopulatePracticeDropDown();


            cddClientGroups.ContextKey = opportunity.Group != null && opportunity.Group.Id.HasValue
                                             ? opportunity.Group.Id.Value.ToString()
                                             : "-1";

            if (opportunity.ProjectId.HasValue)
            {
                var projectId = opportunity.ProjectId.Value.ToString();

                cddClientProjects.ContextKey = projectId;
                UpdateProjectLink(projectId);
            }
            else
            {
                cddClientProjects.ContextKey = "-1";
                if (OpportunityId.HasValue)
                {
                    UpdateProjectLink(string.Empty);
                }
            }

            btnConvertToProject.Enabled = opportunity.Status.Id != (int)OpportunityStatusType.Won;

            if (opportunity.Owner != null)
                dfOwner.SelectedManager = opportunity.Owner;
        }

        private void PopulateSalesPersonDropDown()
        {
            if (Opportunity != null && Opportunity.Salesperson != null)
            {
                ListItem selectedSalesPerson = ddlSalesperson.Items.FindByValue(Opportunity.Salesperson.Id.ToString());
                if (selectedSalesPerson == null)
                {
                    selectedSalesPerson = new ListItem(Opportunity.Salesperson.Name, Opportunity.Salesperson.Id.ToString());
                    ddlSalesperson.Items.Add(selectedSalesPerson);
                    ddlSalesperson.SortByText();
                }

                ddlSalesperson.SelectedValue = selectedSalesPerson.Value;

            }
        }

        private void PopulatePracticeDropDown()
        {

            if (Opportunity != null && Opportunity.Practice != null)
            {
                ListItem selectedPractice = ddlPractice.Items.FindByValue(Opportunity.Practice.Id.ToString());
                // For situation, when disabled practice is assigned to Opportunity.
                if (selectedPractice == null)
                {
                    selectedPractice = new ListItem(Opportunity.Practice.Name, Opportunity.Practice.Id.ToString());
                    ddlPractice.Items.Add(selectedPractice);
                    ddlPractice.SortByText();
                }

                ddlPractice.SelectedValue = selectedPractice.Value;
            }
        }

        private void PopulateData(Opportunity opportunity)
        {
            opportunity.Id = OpportunityId;
            opportunity.Name = txtOpportunityName.Text;
            opportunity.ProjectedStartDate =
                dpProjectedStartDate.DateValue != DateTime.MinValue
                    ? (DateTime?)dpProjectedStartDate.DateValue
                    : null;
            opportunity.ProjectedEndDate =
                dpProjectedEndDate.DateValue != DateTime.MinValue
                    ? (DateTime?)dpProjectedEndDate.DateValue
                    : null;
            opportunity.Priority = ddlPriority.SelectedValue[0];
            opportunity.Description = txtDescription.Text;
            opportunity.BuyerName = txtBuyerName.Text;
            opportunity.EstimatedRevenue = Convert.ToDecimal(txtEstRevenue.Text);
            opportunity.Pipeline = txtPipeline.Text;
            opportunity.Proposed = txtProposed.Text;
            opportunity.SendOut = txtSendOut.Text;
            /*opportunity.OpportunityIndex =
                !string.IsNullOrEmpty(txtReviewOrder.Text)
                    ? (int?) int.Parse(txtReviewOrder.Text)
                    : null;*/


            opportunity.Status =
                new OpportunityStatus { Id = int.Parse(ddlStatus.SelectedValue) };
            opportunity.Client =
                new Client { Id = int.Parse(ddlClient.SelectedValue) };

            if (ddlClientGroup.Items.Count > 0 && ddlClientGroup.SelectedValue != string.Empty)
                opportunity.Group = new ProjectGroup { Id = Convert.ToInt32(ddlClientGroup.SelectedValue) };
            else
                opportunity.Group = null;

            opportunity.Salesperson =
                !string.IsNullOrEmpty(ddlSalesperson.SelectedValue)
                    ? new Person { Id = int.Parse(ddlSalesperson.SelectedValue) }
                    : null;
            opportunity.Practice =
                new Practice { Id = int.Parse(ddlPractice.SelectedValue) };

            if (!(string.IsNullOrEmpty(ddlProjects.SelectedValue)
                || ddlProjects.SelectedValue == "-1")
               )
            {
                opportunity.ProjectId = int.Parse(ddlProjects.SelectedValue);
            }

            if (dfOwner.SelectedManager != null)
                opportunity.Owner = dfOwner.SelectedManager;
        }

        protected void ddlProjects_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var projectId = ddlProjects.SelectedValue;

            if (projectId == "-1")
                hlProject.Visible = false;
            else
                UpdateProjectLink(projectId);
        }

        protected void custOppDescription_ServerValidation(object source, ServerValidateEventArgs args)
        {
            args.IsValid = txtDescription.Text.Length <= 2000;
        }

        private void UpdateProjectLink(string projectId)
        {
            hlProject.CssClass = string.IsNullOrEmpty(projectId) ? "tab-invisible" : "tab-visible";
            string url = GetCurrentUrl();
            string returl = hlProject.NavigateUrl =
                Urls.GetProjectDetailsUrl(projectId, url);
            hlProject.Text = Resources.Controls.OpportunityDetail_Navigate_to_project;

            if (!SelectedId.HasValue && OpportunityId.HasValue)
            {
                string[] splitArray = { "returnTo=" };
                extProjectView.ReturnToUrl = returl.Split(splitArray, StringSplitOptions.None)[1];
            }
        }

        private string GetCurrentUrl()
        {
            string currentUrl = Request.Url.AbsoluteUri;
            if (!SelectedId.HasValue && OpportunityId.HasValue)
            {

                return currentUrl.Substring(0, currentUrl.IndexOf('?')) + "?id=" + OpportunityId.Value + "&returnTo=OpportunityList.aspx";
            }
            else
                return currentUrl;
        }

        protected void custWonConvert_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid =
                ddlStatus.SelectedValue != WonProjectId.ToString();
        }

        #endregion

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            if (ValidateAndSave())
            {
                Redirect(eventArgument == string.Empty ? Constants.ApplicationPages.OpportunityList : eventArgument);
            }
        }

        #endregion
    }
}


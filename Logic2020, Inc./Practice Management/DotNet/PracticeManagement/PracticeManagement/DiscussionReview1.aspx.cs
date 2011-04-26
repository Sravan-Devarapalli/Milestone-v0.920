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
using System.Data;
using DataTransferObjects.ContextObjects;
using System.Drawing;
using System.Web.UI.HtmlControls;

namespace PraticeManagement
{
    public partial class DiscussionReview1 : PracticeManagementPageBase, IPostBackEventHandler
    {
        #region Events

        public event EventHandler NoteAdded;

        #endregion

        #region Fields

        private bool _userIsAdministrator;
        private bool _userIsRecruiter;
        private bool _userIsSalesperson;

        #endregion

        #region Properties

        ////private Project selectedProjectValue;
        private const int WonProjectId = 4;
        private const string OPPORTUNITY_KEY = "OPPORTUNITY_KEY";
        private const string OPPORTUNITIES_LIST_KEY = "OPPORTUNITIES_LIST_KEY";
        private const string PreviousReportContext_Key = "PREVIOUSREPORTCONTEXT_KEY";
        private const string DistinctPotentialBoldPersons_Key = "DISTINCTPOTENTIALBOLDPERSONS_KEY";
        private const string EstRevenueFormat = "Est. Revenue - {0}";
        private const string WordBreak = "<wbr />";
        /// <summary>
        /// 	Gets a selected opportunity
        /// </summary>
        private Opportunity Opportunity
        {
            get
            {
                if (Cache[OPPORTUNITY_KEY] != null && OpportunityId.HasValue)
                {
                    if ((Cache[OPPORTUNITY_KEY] as Opportunity).Id == OpportunityId)
                    {
                        return Cache[OPPORTUNITY_KEY] as Opportunity;
                    }
                }

                if (OpportunityId.HasValue)
                {
                    using (var serviceClient = new OpportunityServiceClient())
                    {
                        try
                        {
                            Cache[OPPORTUNITY_KEY] = serviceClient.GetById(OpportunityId.Value);
                            return Cache[OPPORTUNITY_KEY] as Opportunity;
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

        private Opportunity[] OpportunitiesList
        {
            get
            {
                if (Cache[OPPORTUNITIES_LIST_KEY] != null)
                {
                    return Cache[OPPORTUNITIES_LIST_KEY] as Opportunity[];
                }

                else
                {
                    var result = ServiceCallers.Custom.Opportunity(c => c.OpportunityListAllShort(new OpportunityListContext { ActiveClientsOnly = true }));
                    Cache[OPPORTUNITIES_LIST_KEY] = result;
                    return result;
                }

            }
        }

        private int? OpportunityId
        {
            get
            {
                int id;
                if (Int32.TryParse(hdnOpportunityId.Value, out id))
                {
                    return id;
                }

                return null;
            }
            set
            {
                hdnOpportunityId.Value = value.ToString();
            }
        }

        private int? SelectedIndex
        {
            get
            {
                string index = Request.QueryString["index"];
                int result;
                if (!int.TryParse(index, out result))
                    return null;
                return result;
            }
        }

        private bool HasProposedPersons
        {
            get
            {
                return ucProposedResources.HasProposedPersons;
            }
        }

        #endregion

        #region Methods

        #region Page Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Cache.Remove(OPPORTUNITY_KEY);
                Cache.Remove(OPPORTUNITIES_LIST_KEY);
            }

            BindOpportunitiesData();

            if (!IsPostBack)
            {
                DataHelper.FillClientList(ddlClient, string.Empty);
                DataHelper.FillSalespersonListOnlyActive(ddlSalesperson, string.Empty);
                DataHelper.FillOpportunityStatusList(ddlStatus, string.Empty);

                DataHelper.FillPracticeListOnlyActive(ddlPractice, string.Empty);

                if (SelectedIndex.HasValue)
                {
                    lvOpportunities.SelectedIndex = SelectedIndex.Value;
                }

                LoadOpportunityDetails();

                activityLog.OpportunityId = OpportunityId;
            }

            mlConfirmation.ClearMessage();

            // Security
            InitSecurity();

            if (hdnValueChanged.Value == "false")
            {
                btnSave.Attributes.Add("disabled", "true");
            }
            else
            {
                btnSave.Attributes.Remove("disabled");
            }

            lblSaved.Text = string.Empty;
            lblError.Text = string.Empty;
            divResultDescription.Style["display"] = "none";
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {

            if (lvOpportunities.SelectedIndex <= 0)
            {
                imgBtnFirst.Visible = false;
                imgBtnPrevious.Visible = false;
                imgFirst.Visible = true;
                imgPrevious.Visible = true;

                imgBtnNext.Visible = true;
                imgBtnLast.Visible = true;
                imgNext.Visible = false;
                imgLast.Visible = false;
            }
            else if (lvOpportunities.SelectedIndex >= lvOpportunities.Items.Count - 1)
            {
                imgBtnFirst.Visible = true;
                imgBtnPrevious.Visible = true;
                imgFirst.Visible = false;
                imgPrevious.Visible = false;

                imgBtnNext.Visible = false;
                imgBtnLast.Visible = false;
                imgNext.Visible = true;
                imgLast.Visible = true;
            }
            else
            {
                imgBtnFirst.Visible = true;
                imgBtnPrevious.Visible = true;
                imgFirst.Visible = false;
                imgPrevious.Visible = false;

                imgBtnNext.Visible = true;
                imgBtnLast.Visible = true;
                imgNext.Visible = false;
                imgLast.Visible = false;
            }
            lblTotalOpportunities.Text = lvOpportunities.Items.Count.ToString();
            lblCurrentOpportunity.Text = Convert.ToString(lvOpportunities.SelectedIndex + 1);
            lvOpportunities.DataBind();
            activityLog.OpportunityId = OpportunityId;
            activityLog.Update();

        }

        #endregion

        #region topandleft

        public void imgBtnPrevious_OnClick(object sender, EventArgs e)
        {
            if (!CheckForDirtyBehaviour())
            {
                return;
            }
            lvOpportunities.SelectedIndex = lvOpportunities.SelectedIndex - 1;

            if (IsPostBack && Page.IsValid)
                LoadOpportunityDetails();

        }

        public void imgBtnNext_OnClick(object sender, EventArgs e)
        {
            if (!CheckForDirtyBehaviour())
            {
                return;
            }
            lvOpportunities.SelectedIndex = lvOpportunities.SelectedIndex + 1;

            if (IsPostBack && Page.IsValid)
                LoadOpportunityDetails();

        }

        public void lvOpportunities_SelectedIndexChanging(object sender, ListViewSelectEventArgs e)
        {
            if (!CheckForDirtyBehaviour())
            {
                return;
            }
            lvOpportunities.SelectedIndex = e.NewSelectedIndex;


            if (IsPostBack && Page.IsValid)
                LoadOpportunityDetails();

        }

        public void imgBtnFirst_OnClick(object sender, EventArgs e)
        {
            if (!CheckForDirtyBehaviour())
            {
                return;
            }

            if (lvOpportunities.Items.Count > 0)
                lvOpportunities.SelectedIndex = 0;

            if (IsPostBack && Page.IsValid)
                LoadOpportunityDetails();
        }

        public void imgBtnLast_OnClick(object sender, EventArgs e)
        {
            if (!CheckForDirtyBehaviour())
            {
                return;
            }

            if (lvOpportunities.Items.Count > 0)
                lvOpportunities.SelectedIndex = lvOpportunities.Items.Count - 1;

            if (IsPostBack && Page.IsValid)
                LoadOpportunityDetails();

        }

        public void LoadOpportunityDetails()
        {
            lvOpportunities.DataBind();
            hdnOpportunityId.Value = lvOpportunities.SelectedValue.ToString();
            ucProposedResources.OpportunityId = OpportunityId;

            if (IsPostBack)
                FillControls();

            BindNotesData();

            ucProposedResources.FillProposedResources();
            ucProposedResources.FillPotentialResources();
        }

        #endregion

        private void BindOpportunitiesData()
        {
            lvOpportunities.DataSource = OpportunitiesList;
        }

        private void BindNotesData()
        {
            var notes = ServiceCallers.Custom.Milestone(c => c.NoteListByTargetId(Convert.ToInt32(OpportunityId), 4));
            lvNotes.DataSource = notes;
            lvNotes.DataBind();
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
        }

        protected void btnAddNote_Click(object sender, EventArgs e)
        {
            Page.Validate(tbNote.ValidationGroup);

            if (Page.IsValid)
            {
                var note = new Note
                {
                    Author = new Person
                    {
                        Id = DataHelper.CurrentPerson.Id
                    },
                    CreateDate = DateTime.Now,
                    NoteText = tbNote.Text,
                    Target = (NoteTarget)4,
                    TargetId = (int)OpportunityId
                };

                ServiceCallers.Custom.Milestone(client => client.NoteInsert(note));

                lvNotes.DataSource = ServiceCallers.Custom.Milestone(c => c.NoteListByTargetId(Convert.ToInt32(OpportunityId), 4));
                lvNotes.DataBind();

                tbNote.Text = string.Empty;

                Utils.Generic.InvokeEventHandler(NoteAdded, this, e);
            }
        }

        /// <summary>
        /// 	Creates a project from the opportunity.
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
                if (!CheckForDirtyBehaviour())
                {
                    return;
                }

                Page.Validate(vsumWonConvert.ValidationGroup);
                if (Page.IsValid)
                {
                    if (HasProposedPersons)
                    {
                        Page.Validate(vsumHasPersons.ValidationGroup);
                        if (Page.IsValid)
                        {
                            ConvertToProject();
                        }
                    }
                    else
                    {
                        ConvertToProject();
                    }
                }
            }
        }

        public bool CheckForDirtyBehaviour()
        {
            bool result = true;
            if (CanUserEditOpportunity(Opportunity))
            {
                if (IsDirty)
                {
                    if (!SaveDirty)
                    {
                        ClearDirty();
                        Display();
                    }
                    else if (!ValidateAndSave())
                    {
                        return false;
                    }
                }
            }
            return result;
        }

        private void ConvertToProject()
        {
            using (var serviceClient = new OpportunityServiceClient())
            {
                try
                {
                    var projectId = serviceClient.ConvertOpportunityToProject(OpportunityId.Value,
                                                                              User.Identity.Name, HasProposedPersons);

                    Response.Redirect(
                            Urls.GetProjectDetailsUrl(projectId, Request.Url.AbsoluteUri + "?index=" + lvOpportunities.SelectedIndex));
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            divResultDescription.Style["display"] = "inline";

            bool IsSavedWithoutErrors = ValidateAndSave();
            activityLog.Update();
            if (IsSavedWithoutErrors)
            {
                mlConfirmation.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Opportunity"));
                ClearDirty();
                if (IsPostBack && Page.IsValid)
                {
                    LoadOpportunityDetails();
                }

                ScriptManager.RegisterClientScriptBlock(upOpportunityDetail, upOpportunityDetail.GetType(), "", "FadeOutLabel()", true);
            }

            if (Page.IsValid)
            {
                BindOpportunitiesData();
                lvOpportunities.DataBind();
                lvOpportunities.SelectedIndex = GetSelectedIndex(OpportunityId.Value);
            }
        }

        private int GetSelectedIndex(int OpportunityId)
        {
            foreach (ListViewDataItem item in lvOpportunities.Items)
            {
                Label lblOpportunityName = item.FindControl("lblOpportunityName") as Label;

                if (lblOpportunityName.Attributes["OpportunityID"] == OpportunityId.ToString())
                {
                    return item.DisplayIndex;
                }
            }

            return 0;
        }

        protected void btnCancelChanges_Click(object sender, EventArgs e)
        {
            if (IsDirty)
            {
                ClearDirty();
                LoadOpportunityDetails();
                tbNote.Text = "";
            }
        }

        protected void btnAttachToProject_Click(object sender, EventArgs e)
        {
            int clientId;

            if (int.TryParse(ddlClient.SelectedValue, out clientId))
            {
                var projects = ServiceCallers.Custom.Project(client => client.ListProjectsByClientShort(clientId, true));
                DataHelper.FillListDefault(ddlProjects, "Select Project ...", projects, false, "Id", "DetailedProjectTitle");

                if (ddlProjects.Items.Count == 1)
                {
                    ddlProjects.Items[0].Enabled = true;
                }

                ddlProjects.SelectedValue = (Opportunity.ProjectId.HasValue) ? Opportunity.ProjectId.Value.ToString() : string.Empty;

            }
            else if (ddlProjects.Items != null && ddlProjects.Items.Count == 0)
            {
                ddlProjects.Items.Add(new ListItem() { Text = "Select Project ...", Value = "" });
            }

            mpeAttachToProject.Show();
        }

        public void lvOpportunities_OnDataBound(object sender, EventArgs e)
        {
            FocusToSelectedItem();
        }

        private void FocusToSelectedItem()
        {
            if (lvOpportunities.Items[lvOpportunities.SelectedIndex].FindControl("imgTransparent") != null)
            {
                lvOpportunities.Items[lvOpportunities.SelectedIndex].FindControl("imgTransparent").Focus();
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

                        if (id.HasValue)
                        {
                            OpportunityId = id;
                        }

                        retValue = true;
                        ClearDirty();
                        lblSaved.Text = "Saved";

                        Cache.Remove(OPPORTUNITY_KEY);
                        Cache.Remove(OPPORTUNITIES_LIST_KEY);
                        Cache.Remove(PreviousReportContext_Key);
                        Cache.Remove(DistinctPotentialBoldPersons_Key);
                    }
                    catch (CommunicationException ex)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
            }
            else
            {
                dpEndDate.ErrorMessage = string.Empty;
                dpStartDate.ErrorMessage = string.Empty;
                lblError.Text = "Error";
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
                (!_userIsAdministrator && !_userIsSalesperson && _userIsRecruiter);

            txtOpportunityName.ReadOnly =
                  txtBuyerName.ReadOnly =
                  txtDescription.ReadOnly =
                  txtEstRevenue.ReadOnly =
                                         !canEdit;

            ddlClient.Enabled =
                ddlPriority.Enabled =
                ddlSalesperson.Enabled =
                ddlPractice.Enabled =
                btnSave.Enabled =
                dfOwner.Enabled = canEdit;

            btnConvertToProject.Enabled =
                btnAttachToProject.Enabled = canEdit && !opportunity.ProjectId.HasValue;

            ddlClientGroup.Visible = canEdit;

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

            return _userIsAdministrator || isOwnerOrPracticeManagerOrTheSamePractice || _userIsRecruiter || _userIsSalesperson;
        }

        private void PopulateControls(Opportunity opportunity)
        {
            txtEstRevenue.Text = opportunity.EstimatedRevenue != null ? opportunity.EstimatedRevenue.Value.ToString("###,###,###,###,##0") : string.Empty;
            txtOpportunityName.Text = opportunity.Name;
            lblOpportunityNumber.Text = opportunity.OpportunityNumber;

            dpStartDate.DateValue = opportunity.ProjectedStartDate.HasValue ? opportunity.ProjectedStartDate.Value : DateTime.MinValue;
            dpEndDate.DateValue = opportunity.ProjectedEndDate.HasValue ? opportunity.ProjectedEndDate.Value : DateTime.MinValue;

            txtDescription.Text = opportunity.Description;
            txtBuyerName.Text = opportunity.BuyerName;
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

            PopulateOwnerDropDown();

            PopulateClientGroupDropDown();

            PopulatePriorityHint();

            hdnValueChanged.Value = "false";
            btnSave.Attributes.Add("disabled", "true");
        }

        private void PopulatePriorityHint()
        {
            var opportunityPriorities = DataHelper.GetOpportunityPrioritiesListAll();
            lvOpportunityPriorities.DataSource = opportunityPriorities;
            lvOpportunityPriorities.DataBind();
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
            else
            {
                ddlPractice.SelectedIndex = 0;
            }
        }

        private void PopulateOwnerDropDown()
        {
            if (Opportunity.Owner != null)
                dfOwner.SelectedManager = Opportunity.Owner;
            else
                dfOwner.SetEmptyItem();
        }

        private void PopulateClientGroupDropDown()
        {
            cddClientGroups.ContextKey = Opportunity.Group != null && Opportunity.Group.Id.HasValue
                                             ? Opportunity.Group.Id.Value.ToString()
                                             : "-1";
            if (cddClientGroups.ContextKey == "-1")
                cddClientGroups.SelectedValue = string.Empty;
        }

        private void PopulateData(Opportunity opportunity)
        {
            opportunity.Id = OpportunityId;
            opportunity.Name = txtOpportunityName.Text;
            opportunity.ProjectedStartDate = dpStartDate.DateValue != DateTime.MinValue
                        ? (DateTime?)dpStartDate.DateValue
                        : null;
            opportunity.ProjectedEndDate =
                dpEndDate.DateValue != DateTime.MinValue
                        ? (DateTime?)dpEndDate.DateValue
                        : null;
            opportunity.Priority = ddlPriority.SelectedValue[0];
            opportunity.Description = txtDescription.Text;
            opportunity.BuyerName = txtBuyerName.Text;

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

            opportunity.EstimatedRevenue = Convert.ToDecimal(txtEstRevenue.Text);

            if (!(string.IsNullOrEmpty(ddlProjects.SelectedValue)
                 || ddlProjects.SelectedValue == "-1")
               )
            {
                opportunity.ProjectId = int.Parse(ddlProjects.SelectedValue);
            }

            if (dfOwner.SelectedManager != null)
                opportunity.Owner = dfOwner.SelectedManager;
        }

        protected static string GetFormattedEstimatedRevenue(Decimal? estimatedRevenue)
        {
            try
            {
                int result = (int)estimatedRevenue / 1000;
                return result.ToString(DataTransferObjects.PracticeManagementCurrency.CurrencyLargeDisplayFormat) + "k";
            }
            catch
            {
                return string.Empty;
            }
        }

        protected static string GetTruncatedOpportunityName(String Name)
        {
            if (Name.Length > 27)
            {
                Name = Name.Substring(0, 25) + "..";
            }

            return Name;
        }

        protected static string GetWrappedText(String NoteText)
        {
            if (NoteText.Length > 70)
            {
                for (int i = 10; i < NoteText.Length; i = i + 10)
                {
                    NoteText = NoteText.Insert(i, WordBreak);
                }
            }

            return NoteText;
        }

        #region Validations

        protected void cvDfOwnerRequired_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = (dfOwner.SelectedManager != null);
        }

        protected void custTransitionStatus_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            int statusId;
            e.IsValid =
                !int.TryParse(e.Value, out statusId) || statusId != (int)OpportunityTransitionStatusType.Lost ||
                // Only Administratos, Salesperson or Practice Manager can set the status to Lost.
                _userIsAdministrator || _userIsSalesperson;
        }

        protected void custOppDescription_ServerValidation(object source, ServerValidateEventArgs args)
        {
            args.IsValid = txtDescription.Text.Length <= 2000;
        }

        protected void custWonConvert_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid =
                ddlStatus.SelectedValue != WonProjectId.ToString();
        }

        protected void cvLen_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            var length = tbNote.Text.Length;
            args.IsValid = length > 0 && length <= 2000;
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

        #endregion

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


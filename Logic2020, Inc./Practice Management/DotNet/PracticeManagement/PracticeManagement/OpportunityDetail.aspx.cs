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
using PraticeManagement.Controls.Opportunities;
using AjaxControlToolkit;
using System.Web.UI.HtmlControls;

namespace PraticeManagement
{
    public partial class OpportunityDetail : PracticeManagementPageBase, IPostBackEventHandler
    {
        #region Events

        public event EventHandler NoteAdded;

        #endregion

        #region Fields

        private bool _userIsAdministrator;
        private bool _userIsRecruiter;
        private bool _userIsSalesperson;
        private bool _userIsHR;

        #endregion

        #region Properties

        private const int WonProjectId = 4;
        private const string OPPORTUNITY_KEY = "OPPORTUNITY_KEY";
        private const string NOTE_LIST_KEY = "NOTE_LIST_KEY";
        private const string PreviousReportContext_Key = "PREVIOUSREPORTCONTEXT_KEY";
        private const string DistinctPotentialBoldPersons_Key = "DISTINCTPOTENTIALBOLDPERSONS_KEY";
        private const string EstRevenueFormat = "Est. Revenue - {0}";
        private const string WordBreak = "<wbr />";
        private const string NEWLY_ADDED_NOTES_LIST = "NEWLY_ADDED_NOTES_LIST";
        private const string OpportunityPersons_Key = "OpportunityPersons_Key_1";
        private const string StrawMan_Key = "STRAWMAN_KEY_1";
        private List<NameValuePair> quantities;

        private const string ANIMATION_SHOW_SCRIPT =
                        @"<OnClick>
                        	<Sequence>
                        		<Parallel Duration=""0"" AnimationTarget=""{0}"">
                        			<Discrete Property=""style"" propertyKey=""border"" ValuesScript=""['thin solid navy']""/>
                        		</Parallel>
                        		<Parallel Duration="".4"" Fps=""20"" AnimationTarget=""{0}"">
                        			<Resize  Width=""260"" Height=""{1}"" Unit=""px"" />
                        		</Parallel>
                        	</Sequence>
                        </OnClick>";

        private const string ANIMATION_HIDE_SCRIPT =
                        @"<OnClick>
                        	<Sequence>
                        		<Parallel Duration="".4"" Fps=""20"" AnimationTarget=""{0}"">
                        			<Resize Width=""0"" Height=""0"" Unit=""px"" />
                        		</Parallel>
                        		<Parallel Duration=""0"" AnimationTarget=""{0}"">
                        			<Discrete Property=""style"" propertyKey=""border"" ValuesScript=""['none']""/>
                        		</Parallel>
                        	</Sequence>
                        </OnClick>";

        /// <summary>
        /// 	Gets a selected opportunity
        /// </summary>
        private Opportunity Opportunity
        {
            get
            {
                if (ViewState[OPPORTUNITY_KEY] != null && OpportunityId.HasValue)
                {
                    return ViewState[OPPORTUNITY_KEY] as Opportunity;
                }

                if (OpportunityId.HasValue)
                {
                    using (var serviceClient = new OpportunityServiceClient())
                    {
                        try
                        {

                            var result = serviceClient.GetById(OpportunityId.Value);
                            Generic.RedirectIfNullEntity(result, Response);
                            ViewState[OPPORTUNITY_KEY] = result;
                            return result;
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
                }
                return null;
            }
            set
            {
                hdnOpportunityId.Value = value.ToString();
            }
        }

        private OpportunityPerson[] ProposedPersons
        {
            get
            {
                if (ViewState[OpportunityPersons_Key] != null)
                {
                    return ViewState[OpportunityPersons_Key] as OpportunityPerson[];
                }

                return (new List<OpportunityPerson>()).AsQueryable().ToArray();
            }

            set
            {
                ViewState[OpportunityPersons_Key] = value;
            }
        }

        private OpportunityPerson[] StrawMans
        {
            get
            {
                if (ViewState[StrawMan_Key] != null)
                {
                    return ViewState[StrawMan_Key] as OpportunityPerson[];
                }

                return (new List<OpportunityPerson>()).AsQueryable().ToArray();
            }

            set
            {
                ViewState[StrawMan_Key] = value;
            }
        }

        protected List<NameValuePair> Quantities
        {
            get
            {
                if (quantities == null)
                {
                    quantities = new List<NameValuePair>();

                    for (var index = 0; index <= 10; index++)
                    {
                        var item = new NameValuePair();
                        item.Id = index;
                        item.Name = index.ToString();
                        quantities.Add(item);
                    }
                }
                return quantities;
            }

        }

        //private bool HasProposedPersonsOfTypeNormal
        //{
        //    get
        //    {
        //        return ucProposedResources.HasProposedPersonsOfTypeNormal;
        //    }
        //}

        #endregion

        #region Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState.Remove(OPPORTUNITY_KEY);
                ViewState.Remove(NOTE_LIST_KEY);
                ViewState.Remove(NEWLY_ADDED_NOTES_LIST);
            }


            if (!IsPostBack)
            {
                DataHelper.FillClientList(ddlClient, string.Empty);
                DataHelper.FillSalespersonListOnlyActive(ddlSalesperson, string.Empty);
                DataHelper.FillOpportunityStatusList(ddlStatus, string.Empty);
                DataHelper.FillPracticeListOnlyActive(ddlPractice, string.Empty);
                DataHelper.FillOpportunityPrioritiesList(ddlPriority, string.Empty);
                DataHelper.FillOwnersList(ddlOpportunityOwner, "-- Select Owner --");

                FillPotentialResources();

                var Strawmen = ServiceCallers.Custom.Person(c => c.GetStrawManListAll());
                rpTeamStructure.DataSource = Strawmen;
                rpTeamStructure.DataBind();

                FillProposedResourcesAndStrawMans();


                PopulatePriorityHint();

                if (OpportunityId.HasValue)
                {
                    FillGroupAndProjectDropDown();

                    LoadOpportunityDetails();
                    activityLog.OpportunityId = OpportunityId;
                }
                else
                {
                    //ucProposedResources.FillPotentialResources();
                    //upProposedResources.Update();
                }

                tpHistory.Visible = OpportunityId.HasValue;
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

            animHide.Animations = GetAnimationsScriptForAnimHide();
            animShow.Animations = GetAnimationsScriptForAnimShow();
        }

        public void FillPotentialResources()
        {
            var potentialPersons = ServiceCallers.Custom.Person(c => c.GetPersonListByStatusList("1,3", null));
            cblPotentialResources.DataSource = potentialPersons.OrderBy(c => c.LastName);
            cblPotentialResources.DataBind();
        }

        public void FillProposedResourcesAndStrawMans()
        {
            if (OpportunityId.HasValue)
            {

                using (var serviceClient = new OpportunityServiceClient())
                {
                    OpportunityPerson[] opPersons = serviceClient.GetOpportunityPersons(OpportunityId.Value);
                    ProposedPersons = opPersons.Where(op => op.RelationType == 1).AsQueryable().ToArray();
                    StrawMans = opPersons.Where(op => op.RelationType == 2).AsQueryable().ToArray();
                }

                dtlProposedPersons.DataSource = ProposedPersons.Select(p => new { Name = p.Person.Name, id = p.Person.Id, PersonType = p.PersonType });
                dtlProposedPersons.DataBind();

                dtlTeamStructure.DataSource = StrawMans.Select(p => new { Name = p.Person.Name, id = p.Person.Id, PersonType = p.PersonType, Quantity = p.Quantity });
                dtlTeamStructure.DataBind();

            }
        }


        protected static string GetFormattedPersonName(string personLastFirstName, int opportunityPersonTypeId)
        {
            if (opportunityPersonTypeId == (int)OpportunityPersonType.NormalPerson)
            {
                return personLastFirstName;
            }
            else
            {
                return "<strike>" + personLastFirstName + "</strike>";
            }

        }


        public string GetAnimationsScriptForAnimShow()
        {
            int lvCount = lvOpportunityPriorities.Items.Count;

            int height = ((lvCount + 1) * (35)) - 10;

            if (height > 150)
            {
                height = 180;
            }

            return string.Format(ANIMATION_SHOW_SCRIPT, pnlPriority.ID, 180);
        }

        public string GetAnimationsScriptForAnimHide()
        {
            return string.Format(ANIMATION_HIDE_SCRIPT, pnlPriority.ID);
        }

        private void FillGroupAndProjectDropDown()
        {
            if (OpportunityId.HasValue)
            {
                var groups = ServiceCallers.Custom.Group(client => client.GroupListAll(Opportunity.Client.Id, null));
                groups = groups.AsQueryable().Where(g => (g.IsActive == true)).ToArray();
                DataHelper.FillListDefault(ddlClientGroup, string.Empty, groups, false);

                var projects = ServiceCallers.Custom.Project(client => client.ListProjectsByClientShort(Opportunity.Client.Id, true));
                DataHelper.FillListDefault(ddlProjects, "Select Project ...", projects, false, "Id", "DetailedProjectTitle");
                if (ddlProjects.Items.Count == 1)
                {
                    ddlProjects.Items[0].Enabled = true;
                }
                upAttachToProject.Update();
                upOpportunityDetail.Update();
            }
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (ddlClientGroup.Items.Count > 0)
                ddlClientGroup.SortByText();

            activityLog.OpportunityId = OpportunityId;
            activityLog.Update();
            upActivityLog.Update();
            UpdatePanel1.Update();

            NeedToShowDeleteButton();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "MultipleSelectionCheckBoxes_OnClickKeyName", string.Format("MultipleSelectionCheckBoxes_OnClick('{0}');", cblPotentialResources.ClientID), true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableOrDisableConvertOrAttachToProj", "EnableOrDisableConvertOrAttachToProj();", true);
        }

        protected void cblPotentialResources_OnDataBound(object senser, EventArgs e)
        {
            //foreach (ListItem item in cblPotentialResources.Items)
            //{
            //    item.Selected = false;

            //    if (OpportunityId.HasValue)
            //    {
            //        foreach (var opPerson in OpportunityPersons)
            //        {
            //            if (opPerson.Person.Id.Value.ToString() == item.Value)
            //            {
            //                item.Attributes["selectedchecktype"] = opPerson.PersonType.ToString();
            //                break;
            //            }
            //        }
            //    }
            //}
        }

        private string GetPersonsIndexesWithPersonTypeString(List<OpportunityPerson> persons)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var person in persons)
            {

                if (person.Person.Id.HasValue)
                {
                    var item = cblPotentialResources.Items.FindByValue(person.Person.Id.Value.ToString());
                    if (item != null)
                    {
                        sb.Append(cblPotentialResources.Items.IndexOf(
                                         cblPotentialResources.Items.FindByValue(person.Person.Id.Value.ToString())
                                                                     ).ToString()
                                   );
                        sb.Append(':');
                        sb.Append(person.PersonType.ToString());
                        sb.Append(',');
                    }
                }
            }
            return sb.ToString();
        }

        private string GetTeamStructure(List<OpportunityPerson> optypersons)
        {
            var sb = new StringBuilder();

            foreach (var optyperson in optypersons.FindAll(op => op.RelationType == (int)OpportunityPersonRelationType.TeamStructure))
            {
                if (optyperson.Person != null && optyperson.Person.Id.HasValue)
                {
                    sb.Append(
                        string.Format("{0}:{1}|{2},",
                        optyperson.Person.Id.Value.ToString(),
                        optyperson.PersonType.ToString(),
                        optyperson.Quantity));
                }
            }
            return sb.ToString();
        }


        private void NeedToShowDeleteButton()
        {
            if (OpportunityId.HasValue && _userIsAdministrator)
            {
                btnDelete.Visible = true;

                if (Opportunity.Status.Id == 3 || Opportunity.Status.Id == 5)//Status Ids 3 :-Inactive and 5:- Experimental.
                {
                    btnDelete.Enabled = true;
                }
                else
                {
                    btnDelete.Enabled = false;
                }
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

        public void LoadOpportunityDetails()
        {
            //ucProposedResources.OpportunityId = OpportunityId;

            if (IsPostBack)
                FillControls();

            // PopulateProposedResources();
        }

        //private void PopulateProposedResources()
        //{
        //    ucProposedResources.FillProposedResources();
        //    ucProposedResources.FillPotentialResources();
        //    upProposedResources.Update();
        //}


        /// <summary>
        /// 	Creates a project from the opportunity.
        /// </summary>
        protected void btnConvertToProject_Click(object sender, EventArgs e)
        {
            Page.Validate(vsumOpportunity.ValidationGroup);
            if (Page.IsValid)
            {
                Page.Validate(vsumWonConvert.ValidationGroup);
                if (!Page.IsValid)
                {
                    return;
                }

                var opportunity = Opportunity;

                if (opportunity == null && !OpportunityId.HasValue)
                {
                    if (IsDirty)
                    {
                        if (!SaveDirty)
                        {
                            return;
                        }
                    }

                    if (!ValidateAndSave())
                    {
                        return;
                    }


                    opportunity = Opportunity;
                    //ucProposedResources.OpportunityId = Opportunity.Id;
                }

                if (CanUserEditOpportunity(opportunity))
                {
                    if (!CheckForDirtyBehaviour())
                    {
                        return;
                    }

                    Page.Validate(vsumWonConvert.ValidationGroup);
                    if (Page.IsValid)
                    {
                        //ucProposedResources.FillProposedResources();
                        //upProposedResources.Update();
                        //if (HasProposedPersonsOfTypeNormal)
                        //{
                        //    Page.Validate(vsumHasPersons.ValidationGroup);
                        //    if (Page.IsValid)
                        //    {
                        //      ConvertToProject();
                        //    }
                        //}
                        //else
                        //{
                        //    ConvertToProject();
                        //}
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

        //private void ConvertToProject()
        //{
        //    using (var serviceClient = new OpportunityServiceClient())
        //    {
        //        try
        //        {
        //            var projectId = serviceClient.ConvertOpportunityToProject(OpportunityId.Value,
        //                                                                      User.Identity.Name, HasProposedPersonsOfTypeNormal);
        //            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
        //            {
        //                Response.Redirect(
        //                        Urls.GetProjectDetailsUrl(projectId, Request.Url.AbsoluteUri));
        //            }
        //            else
        //            {
        //                if (Request.Url.AbsoluteUri.Contains('?'))
        //                {
        //                    Response.Redirect(
        //                        Urls.GetProjectDetailsUrl(projectId, Request.Url.AbsoluteUri + "&id=" + OpportunityId.Value));
        //                }
        //                else
        //                {
        //                    Response.Redirect(
        //                       Urls.GetProjectDetailsUrl(projectId, Request.Url.AbsoluteUri + "?id=" + OpportunityId.Value));
        //                }
        //            }
        //        }
        //        catch (CommunicationException)
        //        {
        //            serviceClient.Abort();
        //            throw;
        //        }
        //    }
        //}

        protected void ddlClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlClientGroup.Items.Clear();
            ddlProjects.Items.Clear();

            if (!(ddlClient.SelectedIndex == 0))
            {
                int clientId;

                if (int.TryParse(ddlClient.SelectedValue, out clientId))
                {
                    var groups = ServiceCallers.Custom.Group(client => client.GroupListAll(clientId, null));
                    groups = groups.AsQueryable().Where(g => (g.IsActive == true)).ToArray();
                    DataHelper.FillListDefault(ddlClientGroup, string.Empty, groups, false);

                    var projects = ServiceCallers.Custom.Project(client => client.ListProjectsByClientShort(clientId, true));
                    DataHelper.FillListDefault(ddlProjects, "Select Project ...", projects, false, "Id", "DetailedProjectTitle");

                    if (ddlProjects.Items.Count == 1)
                    {
                        ddlProjects.Items[0].Enabled = true;
                    }
                }
                else if (ddlProjects.Items != null && ddlProjects.Items.Count == 0)
                {
                    ddlProjects.Items.Add(new ListItem() { Text = "Select Project ...", Value = "" });
                }
            }
            upOpportunityDetail.Update();
            upAttachToProject.Update();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (hdnOpportunityDelete.Value == "1")
            {
                using (var serviceClient = new OpportunityService.OpportunityServiceClient())
                {
                    try
                    {
                        serviceClient.OpportunityDelete(OpportunityId.Value, User.Identity.Name);

                        Redirect(Constants.ApplicationPages.OpportunitySummary);
                    }
                    catch (Exception ex)
                    {
                        serviceClient.Abort();
                        mlConfirmation.ShowErrorMessage("{0}", ex.Message);
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            upAttachToProject.Update();
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
            }
        }

        protected void btnCancelChanges_Click(object sender, EventArgs e)
        {
            if (IsDirty)
            {
                ViewState.Remove(NOTE_LIST_KEY);
                ViewState.Remove(NEWLY_ADDED_NOTES_LIST);

                if (OpportunityId.HasValue)
                {
                    LoadOpportunityDetails();
                }
                else
                {
                    ResetControls();
                }
                ClearDirty();
            }
            btnSave.Enabled = false;
        }

        private void ResetControls()
        {
            txtBuyerName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtEstRevenue.Text = string.Empty;
            txtOpportunityName.Text = string.Empty;
            ddlClient.SelectedIndex = 0;
            ddlClientGroup.SelectedIndex = 0;
            ddlPractice.SelectedIndex = 0;
            ddlPriority.SelectedIndex = 0;
            ddlSalesperson.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;
            dpStartDate.TextValue = string.Empty;
            dpEndDate.TextValue = string.Empty;
            //ucProposedResources.ResetProposedResources();
            //ucProposedResources.FillPotentialResources();
            //upProposedResources.Update();
        }

        protected void btnAttachToProject_Click(object sender, EventArgs e)
        {
            Page.Validate(vsumOpportunity.ValidationGroup);
            if (Page.IsValid)
            {
                mpeAttachToProject.Show();
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

                        ViewState.Remove(OPPORTUNITY_KEY);
                        ViewState.Remove(PreviousReportContext_Key);
                        ViewState.Remove(DistinctPotentialBoldPersons_Key);
                        ViewState.Remove(NEWLY_ADDED_NOTES_LIST);
                        btnSave.Enabled = false;
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

                    upOpportunityDetail.Update();
                    upDescription.Update();
                    upAttachToProject.Update();
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
                  txtEstRevenue.ReadOnly =
                                         !canEdit;

            ddlClient.Enabled =
                ddlPriority.Enabled =
                ddlSalesperson.Enabled =
                ddlPractice.Enabled =
                btnSave.Enabled =
                ddlOpportunityOwner.Enabled =
                 canEdit;//ucProposedResources.Enabled =

            btnConvertToProject.Enabled =
               btnAttachToProject.Enabled = canEdit && (opportunity.Project == null);
            hdnHasProjectIdOrPermission.Value = canEdit && (opportunity.Project == null) ? "false" : "true";

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

            return _userIsAdministrator || isOwnerOrPracticeManagerOrTheSamePractice || _userIsRecruiter || _userIsSalesperson || _userIsHR;//#2817: _userIsHR is added as per requirement.
        }

        private void PopulateControls(Opportunity opportunity)
        {
            txtEstRevenue.Text = opportunity.EstimatedRevenue != null ? opportunity.EstimatedRevenue.Value.ToString("###,###,###,###,##0") : string.Empty;
            txtOpportunityName.Text = opportunity.Name;
            lblOpportunityNumber.Text = opportunity.OpportunityNumber;
            if (opportunity.Project != null)
            {
                hpProject.Text = string.Format("Linked to Project {0}", opportunity.Project.ProjectNumber);
                hpProject.NavigateUrl =
                      Utils.Generic.GetTargetUrlWithReturn(string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                 Constants.ApplicationPages.ProjectDetail,
                                 opportunity.Project.Id.ToString())
                                 , Request.Url.AbsoluteUri
                                 );
            }
            else
            {
                hpProject.Visible = false;
            }

            dpStartDate.DateValue = opportunity.ProjectedStartDate.HasValue ? opportunity.ProjectedStartDate.Value : DateTime.MinValue;
            dpEndDate.DateValue = opportunity.ProjectedEndDate.HasValue ? opportunity.ProjectedEndDate.Value : DateTime.MinValue;

            txtDescription.Text = opportunity.Description;
            txtBuyerName.Text = opportunity.BuyerName;
            lblLastUpdate.Text = opportunity.LastUpdate.ToString("MM/dd/yyyy");

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
                ddlPriority.Items.FindByValue(opportunity.Priority == null ? "0" : opportunity.Priority.Id.ToString()));

            //if (!string.IsNullOrEmpty(opportunity.OutSideResources))
            //{
            //    if (opportunity.OutSideResources[opportunity.OutSideResources.Length - 1] == ';')
            //    {
            //        opportunity.OutSideResources = opportunity.OutSideResources.Substring(0, opportunity.OutSideResources.Length - 1);
            //    }
            //    ltrlOutSideResources.Text = opportunity.OutSideResources.Replace(";", "<br/>");
            //}


            PopulateSalesPersonDropDown();

            PopulatePracticeDropDown();

            PopulateOwnerDropDown();

            PopulateClientGroupDropDown();
            PopulateProjectsDropDown();
            hdnValueChanged.Value = "false";
            btnSave.Attributes.Add("disabled", "true");

            hdnProposedPersonsIndexes.Value = GetPersonsIndexesWithPersonTypeString(ProposedPersons.AsQueryable().ToList());
            //hdnProposedOutSideResources.Value = opportunity.OutSideResources;
            hdnTeamStructure.Value = GetTeamStructure(StrawMans.AsQueryable().ToList());
        }

        private void PopulateProjectsDropDown()
        {
            ListItem selectedProject = null;
            if (Opportunity.Project != null && Opportunity.Project.Id.HasValue)
            {
                selectedProject = ddlProjects.Items.FindByValue(Opportunity.Project.Id.ToString());
            }
            if (selectedProject != null)
            {
                ddlProjects.SelectedValue = selectedProject.Value;
            }
        }

        private void PopulatePriorityHint()
        {
            var opportunityPriorities = OpportunityPriorityHelper.GetOpportunityPriorities(true);

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
            {
                var ownerId = Opportunity.Owner.Id.Value.ToString();
                ListItem selectedOwner = ddlOpportunityOwner.Items.FindByValue(ownerId);
                if (selectedOwner == null)
                {
                    selectedOwner = new ListItem(Opportunity.Owner.PersonLastFirstName, ownerId);
                    ddlOpportunityOwner.Items.Add(selectedOwner);
                    ddlOpportunityOwner.SortByText();
                }

                ddlOpportunityOwner.SelectedValue = selectedOwner.Value;
            }
            else
            {
                ddlOpportunityOwner.SelectedValue = string.Empty;
            }
        }

        private void PopulateClientGroupDropDown()
        {
            if (Opportunity.Group != null && Opportunity.Group.Id.HasValue)
            {
                ListItem selectedGroup = ddlClientGroup.Items.FindByValue(Opportunity.Group.Id.ToString());
                if (selectedGroup == null)
                {
                    selectedGroup = new ListItem(Opportunity.Group.Name, Opportunity.Group.Id.ToString());
                    ddlClientGroup.Items.Add(selectedGroup);
                    ddlClientGroup.SortByText();
                }

                ddlClientGroup.SelectedValue = selectedGroup.Value;
            }
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

            int priorityId;
            if (int.TryParse(ddlPriority.SelectedValue, out priorityId))
            {
                opportunity.Priority = new OpportunityPriority { Id = priorityId };
            }

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
                opportunity.Project = new Project { Id = int.Parse(ddlProjects.SelectedValue) };
            }

            var selectedValue = ddlOpportunityOwner.SelectedValue;
            opportunity.Owner = string.IsNullOrEmpty(selectedValue) ?
                null :
                new Person(Convert.ToInt32(selectedValue));

            opportunity.ProposedPersonIdList = hdnProposedResourceIdsWithTypes.Value;
            opportunity.StrawManList = hdnTeamStructure.Value;
            //opportunity.OutSideResources = txtOutSideResources.Text;
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

        protected void rpTeamStructure_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var hdnIndex = e.Item.FindControl("hdnIndex") as HiddenField;
                hdnIndex.Value = e.Item.ItemIndex.ToString();

                var ddlQty = e.Item.FindControl("ddlQuantity") as DropDownList;
                ddlQty.DataSource = Quantities;
                ddlQty.DataBind();
            }
        }

        protected void btnSaveTeamStructure_OnClick(object sender, EventArgs e)
        {

            string[] strawMansSelectedWithIndexes = hdnTeamStructureWithIndexes.Value.Split(',');
            string[] strawMansSelectedWithIds = hdnTeamStructureWithIndexes.Value.Split(',');

            List<OpportunityPerson> opportunityPersons = new List<OpportunityPerson>();

            for (int i = 0; i < strawMansSelectedWithIndexes.Length; i++)
            {
                string[] splitArray = { ":", "|" };
                string[] list = strawMansSelectedWithIndexes[i].Split(splitArray, StringSplitOptions.None);
                var idsList = strawMansSelectedWithIds[i].Split(splitArray, StringSplitOptions.None);
                if (list.Length == 3)
                {
                    var repItem = rpTeamStructure.Items[Convert.ToInt32(list[0])] as RepeaterItem;

                    var hdnPersonId = repItem.FindControl("hdnPersonId") as HiddenField;

                    var lblStrawman = repItem.FindControl("lblStrawman") as Label;

                    var firstname = lblStrawman.Attributes["FirstName"];
                    var lastName = lblStrawman.Attributes["LastName"];

                    var id = Convert.ToInt32(hdnPersonId.Value);

                    var operson = new OpportunityPerson()
                    {
                        Person = new Person() { Id = id, FirstName = firstname, LastName = lastName },
                        PersonType = Convert.ToInt32(list[1]),
                        Quantity = Convert.ToInt32(list[2]),
                        RelationType = 2
                    };
                    opportunityPersons.Add(operson);
                }
            }

            StrawMans = opportunityPersons.AsQueryable().ToArray();

            dtlTeamStructure.DataSource = StrawMans.Select(p => new { Name = p.Person.Name, id = p.Person.Id, PersonType = p.PersonType, Quantity = p.Quantity });
            dtlTeamStructure.DataBind();
        }

        protected void btnAddProposedResources_Click(object sender, EventArgs e)
        {
            string[] ProposedPersonsSelected = hdnProposedPersonsIndexes.Value.Split(',');

            List<OpportunityPerson> opportunityPersons = new List<OpportunityPerson>();

            for (int i = 0; i < ProposedPersonsSelected.Length; i++)
            {
                string[] list = ProposedPersonsSelected[i].Split(':');
                if (list.Length == 2)
                {
                    var name = cblPotentialResources.Items[Convert.ToInt32(list[0])].Text;
                    var firstname = name.Split(',')[1].Trim();
                    var lastName = name.Split(',')[0].Trim();
                    var id = Convert.ToInt32(cblPotentialResources.Items[Convert.ToInt32(list[0])].Value);

                    var operson = new OpportunityPerson()
                    {
                        Person = new Person() { Id = id, FirstName = firstname, LastName = lastName },
                        PersonType = Convert.ToInt32(list[1])
                    };
                    opportunityPersons.Add(operson);
                }
            }

            ProposedPersons = opportunityPersons.AsQueryable().ToArray();

            dtlProposedPersons.DataSource = opportunityPersons.Select(p => new { Name = p.Person.Name, id = p.Person.Id, PersonType = p.PersonType });
            dtlProposedPersons.DataBind();

            //ltrlOutSideResources.Text = txtOutSideResources.Text.Replace(";", "<br/>");

        }

        #region Validations

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


        protected void cvPriority_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = true;
            var selectedText = ddlPriority.SelectedItem.Text.ToUpperInvariant();
            if (selectedText == "PO" || selectedText == "A" || selectedText == "B")
            {
                if (ProposedPersons.Count() < 1 && StrawMans.Count() < 1)
                {
                    e.IsValid = false;
                }
            }
        }


        #endregion

        #endregion

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            if (IsDirty)
            {
                if (ValidateAndSave())
                {
                    Redirect(eventArgument == string.Empty ? Constants.ApplicationPages.OpportunityList : eventArgument);
                }
            }
            else
            {
                if (OpportunityId.HasValue)
                {
                    Page.Validate(vsumOpportunity.ValidationGroup);
                    if (Page.IsValid)
                    {
                        Redirect(eventArgument == string.Empty ? Constants.ApplicationPages.OpportunityList : eventArgument);
                    }
                }
                else
                {
                    Redirect(eventArgument == string.Empty ? Constants.ApplicationPages.OpportunityList : eventArgument);
                }
            }
        }
        #endregion

    }
}


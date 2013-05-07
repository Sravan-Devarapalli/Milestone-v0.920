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
using System.Web;
using DataTransferObjects;
using PraticeManagement.ProjectGroupService;

namespace PraticeManagement
{
    public partial class OpportunityDetail : PracticeManagementPageBase, IPostBackEventHandler
    {

        #region Fields

        private bool _userIsAdministrator;
        private bool _userIsRecruiter;
        private bool _userIsSalesperson;
        private bool _userIsHR;
        private bool IsErrorPanelDisplay;
        private List<Person> _inactiveStrawmanList;

        #endregion

        #region Constants

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
        private const string StrawMansListEncodeFormat = "{0}:{1}|{2}?{3},";
        private const string StrawMansImpactOkSaveButtonId = "btnStrawmansImpactOkSave";
        private const string SaveButtonId = "btnSave";
        private const string StrawMansDateEncodeFormat = "MM/dd/yyyy";
        private List<NameValuePair> quantities;

        private const string ValidationTextForPriority = "You must add a Team Make-Up to this opportunity before it can be saved with a {0} Sales Stage.";

        private const string ANIMATION_SHOW_SCRIPT =
                        @"<OnClick>
                        	<Sequence>
                        		<Parallel Duration=""0"" AnimationTarget=""{0}"">
                        			<Discrete Property=""style"" propertyKey=""border"" ValuesScript=""['thin solid navy']""/>
                        		</Parallel>
                        		<Parallel Duration="".4"" Fps=""20"" AnimationTarget=""{0}"">
                        			<Resize  Width=""350"" Height=""{1}"" Unit=""px"" />
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

        #endregion

        #region Properties

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

        private List<Person> InactiveStrawmanList
        {
            get
            {
                if (_inactiveStrawmanList == null)
                {
                    string coloumSpliter = hdnColoumSpliter.Value;
                    string rowSpliter = hdnRowSpliter.Value;
                    string[] inactiveStrawmans = hdnUsedInactiveStrawmanList.Value.Split(new string[] { rowSpliter }, StringSplitOptions.None);
                    _inactiveStrawmanList = new List<Person>();

                    for (int i = 0; i < inactiveStrawmans.Length; i++)
                    {
                        string[] strawmanDetails = inactiveStrawmans[i].Split(new string[] { coloumSpliter }, StringSplitOptions.None); ;
                        Person strawman = new Person
                        {
                            FirstName = strawmanDetails[0],
                            Id = int.Parse(strawmanDetails[1])
                        };
                        _inactiveStrawmanList.Add(strawman);
                    }
                }
                return _inactiveStrawmanList;
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
                DataHelper.FillClientList(ddlClient, "-- Select Account --");
                DataHelper.FillSalespersonListOnlyActive(ddlSalesperson, "-- Select Salesperson --");
                DataHelper.FillOpportunityStatusList(ddlStatus, "-- Select Status --");
                DataHelper.FillPracticeListOnlyActive(ddlPractice, "-- Select Practice --");
                DataHelper.FillOpportunityPrioritiesList(ddlPriority, "-- Select Sales Stage --");
                DataHelper.FillOwnersList(ddlOpportunityOwner, "-- Select Owner --");

                FillPotentialResources();

                hdnRowSpliter.Value = Guid.NewGuid().ToString();
                hdnColoumSpliter.Value = Guid.NewGuid().ToString();
                var Strawmen = ServiceCallers.Custom.Person(c => c.GetStrawmenListAllShort(false));
                hdnStrawmanListInDropdown.Value = GetStrawmanListInStringFormat(Strawmen);

                ddlStrawmen.DataSource = Strawmen;
                ddlStrawmen.DataBind();
                ddlStrawmen.Items.Insert(0, new ListItem { Text = "-- Select Strawman --", Value = "0" });
                ddlQuantity.DataSource = Quantities;
                ddlQuantity.DataBind();

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
            var potentialPersons = ServiceCallers.Custom.Person(c => c.GetPersonListByStatusList("1,3,5", null));
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

                dtlProposedPersons.DataSource = ProposedPersons.Select(p => new { Name = p.Person.Name, id = p.Person.Id, PersonType = p.PersonType }).OrderBy(p => p.Name);
                dtlProposedPersons.DataBind();

                FillStrawMans(StrawMans, true);
            }
        }

        public void FillStrawMans(OpportunityPerson[] strawMans, bool fillInactiveStrawman = false)
        {
            dtlTeamStructure.DataSource = GetTeamStructureDataSource(StrawMans);
            dtlTeamStructure.DataBind();
            if (fillInactiveStrawman)
            {
                var inactiveStrawmen = strawMans.Where(p => p.Person.Status.Id != (int)PersonStatusType.Active);
                List<Person> persons = new List<Person>();
                foreach (OpportunityPerson op in inactiveStrawmen)
                {
                    persons.Add(op.Person);
                }
                hdnUsedInactiveStrawmanList.Value = GetStrawmanListInStringFormat(persons.ToArray());
            }
        }

        private string GetStrawmanListInStringFormat(Person[] strawMans)
        {
            StringBuilder strawmanListInDropdown = new StringBuilder();
            string rowSpliter = hdnRowSpliter.Value;
            string coloumSpliter = hdnColoumSpliter.Value;
            strawmanListInDropdown.Append("-- Select Strawman --" + coloumSpliter + "0" + rowSpliter);
            for (int i = 0; i < strawMans.Length; i++)
            {
                Person strawMan = strawMans[i];
                if (i != strawMans.Length - 1)
                {
                    strawmanListInDropdown.Append(strawMan.PersonLastFirstName + coloumSpliter + strawMan.Id + rowSpliter);
                }
                else
                {
                    strawmanListInDropdown.Append(strawMan.PersonLastFirstName + coloumSpliter + strawMan.Id);
                }
            }
            return strawmanListInDropdown.ToString();
        }

        private object GetTeamStructureDataSource(OpportunityPerson[] strawMans)
        {
            return StrawMans.Select(p => new { Name = p.Person.Name, id = p.Person.Id, PersonType = p.PersonType, Quantity = p.Quantity, NeedBy = p.NeedBy }).OrderBy(p => p.NeedBy).ThenBy(p => p.Name);
        }

        protected static string GetFormattedPersonName(string personLastFirstName, int opportunityPersonTypeId)
        {
            if (opportunityPersonTypeId == (int)OpportunityPersonType.NormalPerson)
            {
                return HttpUtility.HtmlEncode(personLastFirstName);
            }
            else
            {
                return "<strike>" + HttpUtility.HtmlEncode(personLastFirstName) + "</strike>";
            }

        }

        public string GetAnimationsScriptForAnimShow()
        {
            return string.Format(ANIMATION_SHOW_SCRIPT, pnlPriority.ID, 205);
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

                var projects = ServiceCallers.Custom.Project(client => client.ListProjectsByClientShort(Opportunity.Client.Id, true, false, false));
                DataHelper.FillListDefault(ddlProjects, "-- Select Project --", projects, false, "Id", "DetailedProjectTitle");

                AddAttrbuteToddlProjects(projects);

                var pricingLists = ServiceCallers.Custom.Client(client => client.GetPricingLists(Opportunity.Client.Id));
                DataHelper.FillPricingLists(ddlPricingList, pricingLists.OrderBy(p => p.Name).ToArray());

                if (ddlProjects.Items.Count == 1)
                {
                    ddlProjects.Items[0].Enabled = true;
                }
                upAttachToProject.Update();
                upOpportunityDetail.Update();
            }
        }

        private void AddAttrbuteToddlProjects(Project[] projects)
        {
            foreach (ListItem item in ddlProjects.Items)
            {
                if (projects.Any(p => p.Id.Value.ToString() == item.Value.ToString()))
                {
                    item.Attributes.Add("Description", projects.First(p => p.Id.Value.ToString() == item.Value.ToString()).Description);
                }
                else
                {
                    item.Attributes.Add("Description", string.Empty);
                }
            }
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (ddlClientGroup.Items.Count > 0)
                ddlClientGroup.SortByText();

            if (hdnValueChanged.Value == "false")
            {
                btnSave.Attributes.Add("disabled", "true");
            }
            else
            {
                btnSave.Attributes.Remove("disabled");
            }

            activityLog.OpportunityId = OpportunityId;
            activityLog.Update();
            upActivityLog.Update();
            UpdatePanel1.Update();

            NeedToShowDeleteButton();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "MultipleSelectionCheckBoxes_OnClickKeyName", string.Format("MultipleSelectionCheckBoxes_OnClick('{0}');", cblPotentialResources.ClientID), true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableOrDisableConvertOrAttachToProj", "EnableOrDisableConvertOrAttachToProj();", true);

            if (!IsPostBack && (SelectedId.HasValue || !string.IsNullOrEmpty(ReturnUrl)))
            {
                btnCancelChanges.OnClientClick = string.Empty;
            }

            if (Opportunity != null && Opportunity.ProjectedStartDate.HasValue)
            {
                hdnOpportunityProjectedStartDate.Value = Opportunity.ProjectedStartDate.Value.Date.ToShortDateString();
            }
            else
            {
                hdnOpportunityProjectedStartDate.Value = string.Empty;
            }

        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (IsErrorPanelDisplay)
            {
                PopulateErrorPanel();
            }

        }

        protected void cblPotentialResources_OnDataBound(object senser, EventArgs e)
        {
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

        private string GetPersonsIdsWithPersonTypeString(List<OpportunityPerson> persons)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var operson in persons)
            {

                if (operson.Person.Id.HasValue)
                {

                    sb.Append(operson.Person.Id.Value).ToString();
                    sb.Append(':');
                    sb.Append(operson.PersonType.ToString());
                    sb.Append(',');

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
                        string.Format(StrawMansListEncodeFormat,
                        optyperson.Person.Id.Value.ToString(),
                        optyperson.PersonType.ToString(),
                        optyperson.Quantity,
                        optyperson.NeedBy.Value.ToString(StrawMansDateEncodeFormat)));
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
                    IsErrorPanelDisplay = true;
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

                        if ((StrawMans.Any(s => s.PersonType == 1)) || ProposedPersons.Any(p => p.PersonType == 1))
                        {
                            Page.Validate(vsumHasPersons.ValidationGroup);
                            if (Page.IsValid)
                            {
                                ConvertToProject();
                            }
                            else
                            {
                                lbEndDate.Font.Bold = true;
                                IsErrorPanelDisplay = true;
                            }
                        }
                        else
                        {
                            ConvertToProject();
                        }
                    }
                    else
                    {
                        IsErrorPanelDisplay = true;
                    }
                }
            }
            else
            {
                IsErrorPanelDisplay = true;
            }
        }

        private void PopulateErrorPanel()
        {
            mpeErrorPanel.Show();
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
                                                                              User.Identity.Name, (StrawMans.Any(s => s.PersonType == 1) || ProposedPersons.Any(p => p.PersonType == 1)));
                    if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                    {
                        Response.Redirect(
                                Urls.GetProjectDetailsUrl(projectId, Request.Url.AbsoluteUri));
                    }
                    else
                    {
                        if (Request.Url.AbsoluteUri.Contains('?'))
                        {
                            Response.Redirect(
                                Urls.GetProjectDetailsUrl(projectId, Request.Url.AbsoluteUri + "&id=" + OpportunityId.Value));
                        }
                        else
                        {
                            Response.Redirect(
                               Urls.GetProjectDetailsUrl(projectId, Request.Url.AbsoluteUri + "?id=" + OpportunityId.Value));
                        }
                    }
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void ddlClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlClientGroup.Items.Clear();
            ddlProjects.Items.Clear();
            lblBusinessGroup.Text = "";
            if (!(ddlClient.SelectedIndex == 0))
            {
                int clientId;

                if (int.TryParse(ddlClient.SelectedValue, out clientId))
                {
                    var groups = ServiceCallers.Custom.Group(client => client.GroupListAll(clientId, null));
                    groups = groups.AsQueryable().Where(g => (g.IsActive == true)).ToArray();
                    DataHelper.FillListDefault(ddlClientGroup, string.Empty, groups, false);

                    var projects = ServiceCallers.Custom.Project(client => client.ListProjectsByClientShort(clientId, true, false, false));
                    DataHelper.FillListDefault(ddlProjects, "-- Select Project --", projects, false, "Id", "DetailedProjectTitle");
                    AddAttrbuteToddlProjects(projects);
                    if (ddlProjects.Items.Count == 1)
                    {
                        ddlProjects.Items[0].Enabled = true;
                    }
                    var pricingLists = ServiceCallers.Custom.Client(client => client.GetPricingLists(clientId));
                    DataHelper.FillPricingLists(ddlPricingList, pricingLists.OrderBy(p => p.Name).ToArray());
                }
                else if (ddlProjects.Items != null && ddlProjects.Items.Count == 0)
                {
                    ddlProjects.Items.Add(new ListItem() { Text = "-- Select Project --", Value = "" });
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
                        IsErrorPanelDisplay = true;
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            upAttachToProject.Update();
            bool IsSavedWithoutErrors = ValidateAndSave();

            ProcessAfterSave(IsSavedWithoutErrors);
        }

        private void ProcessAfterSave(bool isSavedWithoutErrors)
        {
            activityLog.Update();
            if (isSavedWithoutErrors)
            {
                mlConfirmation.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Opportunity"));
                ClearDirty();
                if (IsPostBack && Page.IsValid)
                {
                    LoadOpportunityDetails();
                }
                IsErrorPanelDisplay = true;
            }
        }

        protected void btnStrawmansImpactOkSave_Click(object sender, EventArgs e)
        {
            hdnTeamStructure.Value = hdnNewStrawmansList.Value;
            StrawMans = strawMansListDecode(hdnNewStrawmansList.Value);
            FillStrawMans(StrawMans);
            bool IsSavedWithoutErrors = SaveData(false);

            ProcessAfterSave(IsSavedWithoutErrors);
        }

        protected void btnCancelChanges_Click(object sender, EventArgs e)
        {
            if (SelectedId.HasValue || !string.IsNullOrEmpty(ReturnUrl))
            {
                ReturnToPreviousPage();
            }
            else
            {
                ClearDirty();
                Response.Redirect("~/DiscussionReview2.aspx");
            }
        }

        private void ResetControls()
        {
            txtBuyerName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtEstRevenue.Text = string.Empty;
            txtOpportunityName.Text = string.Empty;
            lblOpportunityName.Text = string.Empty;
            ddlClient.SelectedIndex = 0;
            ddlClientGroup.Items.Clear();
            lblBusinessGroup.Text = "";
            ddlPractice.SelectedIndex = 0;
            ddlBusinessOptions.SelectedIndex = 0;
            ddlPriority.SelectedIndex = 0;
            ddlSalesperson.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;
            dpStartDate.TextValue = string.Empty;
            dpEndDate.TextValue = string.Empty;
            dpCloseDate.TextValue = string.Empty;
            ddlPriority.Attributes["selectedPriorityText"] = ddlPriority.SelectedItem.Text;
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
            else
            {
                IsErrorPanelDisplay = true;
            }
        }

        protected void cvOpportunityStrawmanEndDateCheck_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = true;
            compEndDate.Validate();
            if (compEndDate.IsValid)
            {
                bool check = true;
                DateTime? ProjectedEndDate = dpEndDate.DateValue != DateTime.MinValue ? (DateTime?)dpEndDate.DateValue : null;
                String StrawManList = hdnTeamStructure.Value;
                string[] strawMansSelectedWithIds = hdnTeamStructure.Value.Split(',');
                for (int i = 0; i < strawMansSelectedWithIds.Length; i++)
                {
                    string[] splitArray = { "?" }; // personId:personType|Quantity?NeedBy
                    string[] list = strawMansSelectedWithIds[i].Split(splitArray, StringSplitOptions.None);
                    if (list.Length == 2)
                    {
                        var NeedBy = DateTime.Parse(list[1]);
                        check = (ProjectedEndDate != null ? NeedBy <= ProjectedEndDate : true);
                        if (!check)
                            break;
                    }
                }
                e.IsValid = check;
            }
        }

        protected override bool ValidateAndSave()
        {
            var retValue = false;

            Page.Validate(vsumOpportunity.ValidationGroup);
            if (Page.IsValid)
            {
                var strawMansNeedbyDatesValid = ValidateStrawMansNeedByDates();

                if (strawMansNeedbyDatesValid)
                {
                    retValue = SaveData(retValue);
                }
            }
            else
            {
                dpEndDate.ErrorMessage = string.Empty;
                dpStartDate.ErrorMessage = string.Empty;
                dpCloseDate.ErrorMessage = string.Empty;
                IsErrorPanelDisplay = true;
            }

            return retValue;
        }

        private bool SaveData(bool result)
        {
            var opportunity = new Opportunity();
            PopulateData(opportunity);
            if (opportunity.Priority.Id == Constants.OpportunityPriorityIds.PriorityIdOfPO && opportunity.Project == null)
            {
                mpeAttachToProject.Show();
                return false;
            }

            using (var serviceClient = new OpportunityServiceClient())
            {
                try
                {
                    int? id = serviceClient.OpportunitySave(opportunity, User.Identity.Name);

                    if (id.HasValue)
                    {
                        OpportunityId = id;
                    }

                    result = true;
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
            return result;
        }

        public void SetBusinessGroupLabel()
        {
            using (var serviceClient = new ProjectGroupServiceClient())
            {
                try
                {
                    if (ddlClientGroup.SelectedValue != "")
                    {
                        BusinessGroup[] businessGroupList = serviceClient.GetBusinessGroupList(null, Convert.ToInt32(ddlClientGroup.SelectedValue));
                        BusinessGroup bg = businessGroupList.Any() ? businessGroupList.First() : null;
                        lblBusinessGroup.Text = HttpUtility.HtmlEncode(bg.Name.Length > 32 ? bg.Name.Substring(0, 35) + "...." : bg.Name);
                        lblBusinessGroup.ToolTip = bg != null ? bg .Name : string.Empty;
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

        protected void ddlClientGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetBusinessGroupLabel();
        }

        private bool ValidateStrawMansNeedByDates()
        {
            //If Startdate modifies then Check Strawmans needby date impact with StartDate.
            bool impacted = false;
            DateTime? StartDate = dpStartDate.DateValue;
            DateTime opportunityProjectedStartDate;
            bool startDateMovedBackward = false;

            if (Opportunity != null && Opportunity.ProjectedStartDate.HasValue)
            {
                opportunityProjectedStartDate = Opportunity.ProjectedStartDate.Value.Date;
                startDateMovedBackward = (Opportunity.ProjectedStartDate.Value.Date > dpStartDate.DateValue.Date);
            }
            else
            {
                opportunityProjectedStartDate = StartDate.Value.Date;
            }
            lblStrawmansImpacted.Text = string.Empty;
            lblNewOpportunityStartDate.Text = StartDate.Value.ToShortDateString();


            string[] strawMansSelectedWithIds = hdnTeamStructure.Value.Split(',');
            var newStrawMansList = new StringBuilder();
            for (int i = 0; i < strawMansSelectedWithIds.Length; i++)
            {
                string[] splitArray = { ":", "|", "?" }; // personId:personType|Quantity?NeedBy
                string[] list = strawMansSelectedWithIds[i].Split(splitArray, StringSplitOptions.None);
                if (list.Length == 4)
                {
                    var id = Convert.ToInt32(list[0]);
                    var personType = Convert.ToInt32(list[1]);
                    var quantity = Convert.ToInt32(list[2]);
                    var needBy = DateTime.Parse(list[3]);

                    if (((Opportunity == null || !startDateMovedBackward) && needBy.Date < StartDate.Value.Date)
                        || (Opportunity != null && startDateMovedBackward && needBy.Date == opportunityProjectedStartDate)
                        )
                    {
                        string strawmanName = string.Empty;
                        if (ddlStrawmen.Items.FindByValue(list[0]) != null)
                        {
                            strawmanName = ddlStrawmen.Items.FindByValue(list[0]).Text;
                        }
                        else
                        {
                            if (InactiveStrawmanList.Any(p => p.Id == int.Parse(list[0])))
                            {
                                strawmanName = InactiveStrawmanList.First(p => p.Id == int.Parse(list[0])).FirstName;
                            }
                        }

                        lblStrawmansImpacted.Text = lblStrawmansImpacted.Text + "<br/> - " + strawmanName + "(" + quantity + ")";
                        impacted = true;
                        newStrawMansList.Append(string.Format(StrawMansListEncodeFormat,
                                                                id,
                                                                personType,
                                                                quantity,
                                                                StartDate.Value.ToString(StrawMansDateEncodeFormat)));
                    }
                    else
                    {
                        newStrawMansList.Append(string.Format(StrawMansListEncodeFormat,
                                                                id,
                                                                personType,
                                                                quantity,
                                                                needBy.ToString(StrawMansDateEncodeFormat)));
                    }
                }
            }
            hdnNewStrawmansList.Value = newStrawMansList.ToString();

            if (impacted)
            {
                mpeStrawmansImpactedWithOpportunityStartDate.Show();
            }

            return !impacted;
        }

        protected override void Display()
        {
            DataHelper.FillBusinessTypes(ddlBusinessOptions);
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
                    upTeamMakeUp.Update();
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
                ddlBusinessOptions.Enabled =
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
            lblOpportunityName.Text = opportunity.HtmlEncodedName;
            lblOpportunityNumber.Text = opportunity.OpportunityNumber;

            if (opportunity.Project != null)
            {
                hpProject.Visible = true;
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
            dpCloseDate.DateValue = opportunity.CloseDate.HasValue ? opportunity.CloseDate.Value : DateTime.MinValue;

            txtDescription.Text = opportunity.Description;
            txtBuyerName.Text = opportunity.BuyerName;
            lblLastUpdate.Text = opportunity.LastUpdate.ToString("MM/dd/yyyy");

            ddlStatus.SelectedIndex =
                ddlStatus.Items.IndexOf(
                    ddlStatus.Items.FindByValue(
                        opportunity.Status != null ? opportunity.Status.Id.ToString() : string.Empty));

            string selectClientId = opportunity.Client != null && opportunity.Client.Id.HasValue ? opportunity.Client.Id.Value.ToString() : string.Empty;
            ListItem item = ddlClient.Items.FindByValue(selectClientId);
            int clientid = 0;
            if(item == null && int.TryParse(selectClientId,out clientid))
            {
                var selectClient = ServiceCallers.Custom.Client(c=>c.GetClientDetailsShort(clientid));
                ddlClient.Items.Add(new ListItem(selectClient.HtmlEncodedName,selectClientId));
            }
            ddlClient.SelectedValue = selectClientId;

            ddlPriority.SelectedIndex =
                ddlPriority.Items.IndexOf(
                ddlPriority.Items.FindByValue(opportunity.Priority == null ? "0" : opportunity.Priority.Id.ToString()));

            ddlPriority.Attributes["selectedPriorityText"] = ddlPriority.SelectedItem.Text;

            PopulateSalesPersonDropDown();

            PopulatePracticeDropDown();

            PopulateOwnerDropDown();

            PopulateClientGroupDropDown();
            PopulateProjectsDropDown();
            hdnValueChanged.Value = "false";
            btnSave.Attributes.Add("disabled", "true");

            hdnProposedResourceIdsWithTypes.Value = GetPersonsIdsWithPersonTypeString(ProposedPersons.AsQueryable().ToList());
            hdnProposedPersonsIndexes.Value = GetPersonsIndexesWithPersonTypeString(ProposedPersons.AsQueryable().ToList());
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


            //cvPriority validation

            List<OpportunityPriority> priorityList = opportunityPriorities.Where(p => p.Id == Constants.OpportunityPriorityIds.PriorityIdOfPO || p.Id == Constants.OpportunityPriorityIds.PriorityIdOfA || p.Id == Constants.OpportunityPriorityIds.PriorityIdOfB).ToList();


            if (priorityList != null && priorityList.Count > 0)
            {
                string displayNames = priorityList.First().DisplayName;
                if (priorityList.Count > 1)
                {
                    for (int i = 1; i < priorityList.Count - 1; i++)
                    {
                        displayNames += " , " + priorityList[i].DisplayName;
                    }
                    displayNames = displayNames + " or " + priorityList[priorityList.Count - 1].DisplayName;
                }

                cvPriority.ErrorMessage = cvPriority.ToolTip =
            string.Format(ValidationTextForPriority, displayNames);
            }

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
                SetBusinessGroupLabel();
            }
            if (Opportunity.PricingList != null && Opportunity.PricingList.PricingListId.HasValue)
            {
                ListItem selectedGroup = ddlPricingList.Items.FindByValue(Opportunity.PricingList.PricingListId.ToString());
                if (selectedGroup == null)
                {
                    selectedGroup = new ListItem(Opportunity.PricingList.Name, Opportunity.PricingList.PricingListId.ToString());
                    ddlPricingList.Items.Add(selectedGroup);
                    ddlPricingList.SortByText();
                }

                ddlPricingList.SelectedValue = selectedGroup.Value;
            }
            else
            {
                ddlPricingList.SelectedIndex = 0;
            }
            ListItem selectedBusinessTypes = ddlBusinessOptions.Items.FindByValue(((int)Opportunity.BusinessType).ToString() == "0" ? "" : ((int)Opportunity.BusinessType).ToString());
            ddlBusinessOptions.SelectedValue = selectedBusinessTypes.Value;
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

            opportunity.CloseDate = dpCloseDate.DateValue != DateTime.MinValue
                        ? (DateTime?)dpCloseDate.DateValue
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

            if (ddlPricingList.Items.Count > 0 && ddlPricingList.SelectedValue != string.Empty)
                opportunity.PricingList = new PricingList { PricingListId = Convert.ToInt32(ddlPricingList.SelectedValue) };
            else
                opportunity.PricingList = null;

            opportunity.Salesperson =
                !string.IsNullOrEmpty(ddlSalesperson.SelectedValue)
                    ? new Person { Id = int.Parse(ddlSalesperson.SelectedValue) }
                    : null;
            opportunity.Practice =
                new Practice { Id = int.Parse(ddlPractice.SelectedValue) };

            opportunity.EstimatedRevenue = Convert.ToDecimal(txtEstRevenue.Text);


            opportunity.Description = txtDescription.Text;

            if (!(string.IsNullOrEmpty(ddlProjects.SelectedValue)
                 || ddlProjects.SelectedValue == "-1")
               )
            {
                opportunity.Project = new Project { Id = int.Parse(ddlProjects.SelectedValue) };
                if (rbtnProjectDescription.Checked)
                {
                    opportunity.Description = ddlProjects.SelectedItem.Attributes["Description"];
                }
            }

            var selectedValue = ddlOpportunityOwner.SelectedValue;
            opportunity.Owner = string.IsNullOrEmpty(selectedValue) ?
                null :
                new Person(Convert.ToInt32(selectedValue));

            opportunity.BusinessType = (BusinessType)Enum.Parse(typeof(BusinessType), ddlBusinessOptions.SelectedValue == "" ? "0" : ddlBusinessOptions.SelectedValue);

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
            StrawMans = strawMansListDecode(hdnTeamStructure.Value);

            dtlTeamStructure.DataSource = GetTeamStructureDataSource(StrawMans);
            dtlTeamStructure.DataBind();
        }

        private OpportunityPerson[] strawMansListDecode(string strawMansList)
        {
            string[] strawMansSelectedWithIds = strawMansList.Split(',');

            List<OpportunityPerson> opportunityPersons = new List<OpportunityPerson>();

            for (int i = 0; i < strawMansSelectedWithIds.Length; i++)
            {
                string[] splitArray = { ":", "|", "?" }; // personId:personType|Quantity?NeedBy
                string[] list = strawMansSelectedWithIds[i].Split(splitArray, StringSplitOptions.None);
                if (list.Length == 4)
                {

                    var personName = string.Empty;
                    if (ddlStrawmen.Items.FindByValue(list[0]) != null)
                    {
                        personName = ddlStrawmen.Items.FindByValue(list[0]).Text;
                    }
                    else
                    {
                        if (InactiveStrawmanList.Any(p => p.Id == int.Parse(list[0])))
                        {
                            personName = InactiveStrawmanList.First(p => p.Id == int.Parse(list[0])).FirstName;
                        }
                    }
                    var firstname = personName.Split(',')[1];
                    var lastName = personName.Split(',')[0];

                    var id = Convert.ToInt32(list[0]);

                    var operson = new OpportunityPerson()
                    {
                        Person = new Person() { Id = id, FirstName = firstname, LastName = lastName },
                        PersonType = Convert.ToInt32(list[1]),
                        Quantity = Convert.ToInt32(list[2]),
                        RelationType = 2,
                        NeedBy = DateTime.Parse(list[3])
                    };
                    opportunityPersons.Add(operson);
                }
            }
            return opportunityPersons.AsQueryable().ToArray();
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

            var selectedValue = ddlPriority.SelectedValue;
            if (selectedValue == Constants.OpportunityPriorityIds.PriorityIdOfPO.ToString() || selectedValue == Constants.OpportunityPriorityIds.PriorityIdOfA.ToString() || selectedValue == Constants.OpportunityPriorityIds.PriorityIdOfB.ToString())
            {
                if (ProposedPersons.Count() < 1 && StrawMans.Count() < 1)
                {
                    e.IsValid = false;
                }
            }
        }

        protected void cvOwner_OnServerValidate(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            int ownerId;
            if (int.TryParse(ddlOpportunityOwner.SelectedValue, out ownerId))
            {
                Person owner = ServiceCallers.Custom.Person(p => p.GetPersonDetailsShort(ownerId));
                PersonStatusType status = PersonStatus.ToStatusType(owner.Status.Id);
                if (status == PersonStatusType.Terminated || status == PersonStatusType.Inactive)
                {
                    args.IsValid = false;
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
                    RedirectWithOutReturnTo(eventArgument == string.Empty ? Constants.ApplicationPages.OpportunityList : eventArgument);
                }
            }
            else
            {
                RedirectWithOutReturnTo(eventArgument == string.Empty ? Constants.ApplicationPages.OpportunityList : eventArgument);
            }
        }
        #endregion

    }
}


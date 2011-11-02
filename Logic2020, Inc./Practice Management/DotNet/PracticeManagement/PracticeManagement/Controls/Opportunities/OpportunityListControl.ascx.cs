using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.ContextObjects;
using DataTransferObjects;
using PraticeManagement.Utils;
using System.Web.UI.HtmlControls;
using AjaxControlToolkit;
using System.Text;
using PraticeManagement.OpportunityService;
using PraticeManagement.Controls.Generic.Filtering;

namespace PraticeManagement.Controls.Opportunities
{
    public enum OpportunityListFilterMode
    {
        GenericFilter,
        ByTargetPerson
    }
    public partial class OpportunityListControl : PracticeManagementFilterControl<OpportunitySortingContext>
    {
        private const string ViewStateSortOrder = "SortOrder";
        private const string ViewStateSortDirection = "SortDirection";
        private const string CssArrowClass = "arrow";
        private const string EDITED_OPPORTUNITY_NOTE_LIST_KEY = "EditedOpportunityNoteList";
        private const string EDITED_OPPORTUNITY_NOTEID_LIST_KEY = "EditedOpportunityNoteIdList";
        private const string NoteTextBoxID = "txtNote";
        private const string NoteId = "NoteId";
        private const string OpportunityIdValue = "OpportunityId";
        private const string Watermarker = "watermarker";
        private const string WordBreak = "<wbr />";
        private const string Description = "<b>Description : </b>{0}";

        private const string ANIMATION_SHOW_SCRIPT =
                     @"<OnClick>
                        	<Sequence>
                        		<Parallel Duration=""0"" AnimationTarget=""{0}"">
                        			<Discrete Property=""style"" propertyKey=""border"" ValuesScript=""['thin solid navy']""/>
                        		</Parallel>
                        		<Parallel Duration="".4"" Fps=""20"" AnimationTarget=""{0}"">
                        			<Resize  Width=""257"" Height=""{1}"" Unit=""px"" />
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
        private List<NameValuePair> quantities;

        #region Properties

        public bool AllowAutoRedirectToDetails { get; set; }

        public int? TargetPersonId { get; set; }

        private Dictionary<int, String> EditedOpportunityList
        {
            get
            {
                if (ViewState[EDITED_OPPORTUNITY_NOTE_LIST_KEY] == null)
                {
                    ViewState[EDITED_OPPORTUNITY_NOTE_LIST_KEY] = new Dictionary<int, String>();
                }

                return (Dictionary<int, String>)ViewState[EDITED_OPPORTUNITY_NOTE_LIST_KEY];
            }
            set { ViewState[EDITED_OPPORTUNITY_NOTE_LIST_KEY] = value; }
        }

        private Dictionary<int, int> EditedOpportunityNoteIdList
        {
            get
            {
                if (ViewState[EDITED_OPPORTUNITY_NOTEID_LIST_KEY] == null)
                {
                    ViewState[EDITED_OPPORTUNITY_NOTEID_LIST_KEY] = new Dictionary<int, int>();
                }

                return (Dictionary<int, int>)ViewState[EDITED_OPPORTUNITY_NOTEID_LIST_KEY];
            }
            set { ViewState[EDITED_OPPORTUNITY_NOTEID_LIST_KEY] = value; }
        }

        protected string OrderBy
        {
            get
            {
                return GetViewStateValue<string>(ViewStateSortOrder, null);
            }
            set
            {
                SetViewStateValue(ViewStateSortOrder, value);
            }
        }

        protected SortDirection SortDirection
        {
            get
            {
                return GetViewStateValue(ViewStateSortDirection, SortDirection.Ascending);
            }
            set
            {
                SetViewStateValue(ViewStateSortDirection, value);
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
                        if (index > 0)
                            item.Name = index.ToString();
                        quantities.Add(item);
                    }
                }
                return quantities;
            }

        }

        private Opportunity[] OpportunitiesList
        {
            get
            {
                return DataHelper.GetFilteredOpportunitiesForDiscussionReview2();
            }
        }

        private Dictionary<string, int> PriorityTrendList
        {
            get
            {
                using (var serviceClient = new OpportunityService.OpportunityServiceClient())
                {
                    return serviceClient.GetOpportunityPriorityTransitionCount(Constants.Dates.HistoryDays);
                }
            }
        }

        private Dictionary<string, int> StatusChangesList
        {
            get
            {
                using (var serviceClient = new OpportunityService.OpportunityServiceClient())
                {
                    return serviceClient.GetOpportunityStatusChangeCount(Constants.Dates.HistoryDays);
                }
            }
        }

        #endregion

        public Opportunity[] GetOpportunities()
        {

            return DataHelper.GetFilteredOpportunitiesForDiscussionReview2();

        }

        protected void lvOpportunities_Sorting(object sender, ListViewSortEventArgs e)
        {
            var newOrder = e.SortExpression;

            if (newOrder == OrderBy)
            {
                SortDirection =
                    SortDirection == SortDirection.Ascending ?
                        SortDirection.Descending : SortDirection.Ascending;
            }
            else
            {
                OrderBy = newOrder;
                SortDirection = SortDirection.Ascending;
            }

            SetHeaderIconsAccordingToSordOrder();
            FireFilterOptionsChanged();

            var x = EditedOpportunityList;
        }

        protected override void ResetControls()
        {
            OrderBy = null;
            SortDirection = SortDirection.Ascending;

            SetHeaderIconsAccordingToSordOrder();
        }

        protected override void InitControls()
        {
            OrderBy = Filter.OrderBy;
            SortDirection = DataTransferObjects.Utils.Generic.ToEnum<SortDirection>(Filter.SortDirection, SortDirection.Ascending);

            SetHeaderIconsAccordingToSordOrder();
        }

        protected override OpportunitySortingContext InitFilter()
        {
            return new OpportunitySortingContext
            {
                SortDirection = SortDirection.ToString(),
                OrderBy = OrderBy
            };
        }

        private void SetHeaderIconsAccordingToSordOrder()
        {
            var row = lvOpportunities.FindControl("lvHeader") as System.Web.UI.HtmlControls.HtmlTableRow;

            if (row != null)
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    var cell = row.Cells[i];

                    if (cell.HasControls())
                    {
                        foreach (var ctrl in cell.Controls)
                        {
                            if (ctrl is LinkButton)
                            {
                                var lb = ctrl as LinkButton;
                                lb.CssClass = CssArrowClass;
                                if (lb.CommandArgument == OrderBy)
                                    lb.CssClass = GetCssClass();
                            }
                        }
                    }
                }
        }

        protected string GetCssClass()
        {
            return string.Format("{0} sort-{1}",
                                CssArrowClass,
                                SortDirection.ToString() == SortDirection.Ascending.ToString() ? "up" : "down");
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ResetControls();
                InitFilter();
                FireFilterOptionsChanged();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            pnlSummary.Controls.Add(GetSummaryDetails());
            PopulatePriorityHint();
        }

        private void PopulatePriorityHint()
        {
            var opportunityPriorities = OpportunityPriorityHelper.GetOpportunityPriorities(true);
            var row = lvOpportunities.FindControl("lvHeader") as System.Web.UI.HtmlControls.HtmlTableRow;
            var lvOp = row.FindControl("lvOpportunityPriorities") as ListView;
            lvOp.DataSource = opportunityPriorities;
            lvOp.DataBind();
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            PreparePrioritiesWithAnimations();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "MultipleSelectionCheckBoxes_OnClickKeyName", string.Format("MultipleSelectionCheckBoxes_OnClick('{0}');", cblPotentialResources.ClientID), true);
        }

        private void PreparePrioritiesWithAnimations()
        {
            var row = lvOpportunities.FindControl("lvHeader") as System.Web.UI.HtmlControls.HtmlTableRow;

            var img = row.FindControl("imgPriorityHint") as Image;
            var lvOp = row.FindControl("lvOpportunityPriorities") as ListView;
            var pnlPriority = row.FindControl("pnlPriority") as Panel;
            var btnClosePriority = row.FindControl("btnClosePriority") as Button;

            var animHide = row.FindControl("animHide") as AnimationExtender;
            var animShow = row.FindControl("animShow") as AnimationExtender;

            int lvCount = lvOp.Items.Count;

            int height = ((lvCount + 1) * (35)) - 10;

            if (height > 150)
            {
                height = 177;
            }

            animShow.Animations = string.Format(ANIMATION_SHOW_SCRIPT, pnlPriority.ID, height);
            animHide.Animations = string.Format(ANIMATION_HIDE_SCRIPT, pnlPriority.ID);

            img.Attributes["onclick"]
               = string.Format("setHintPosition('{0}', '{1}');", img.ClientID, pnlPriority.ClientID);
        }

        public void DatabindOpportunities()
        {
            if (!IsPostBack)
            {
                var potentialPersons = ServiceCallers.Custom.Person(c => c.GetPersonListByStatusList("1,3", null));
                cblPotentialResources.DataSource = potentialPersons.OrderBy(c => c.LastName);
                cblPotentialResources.DataBind();
                var Strawmen = ServiceCallers.Custom.Person(c => c.GetStrawManListAll());
                rpTeamStructure.DataSource = Strawmen;
                rpTeamStructure.DataBind();
            }
            var opportunities = GetOpportunities();
            lvOpportunities.DataSource = opportunities;
            lvOpportunities.DataBind();
            lblOpportunitiesCount.Text = string.Format(lblOpportunitiesCount.Text, opportunities.Length);

            //  IsPostBack here means that method is called on postback
            //      so it means that it's coming from search and we should redirect if there's the only result
            if (IsPostBack && lvOpportunities.Items.Count == 1 && AllowAutoRedirectToDetails)
            {
                var detailsLink =
                    Urls.OpportunityDetailsLink(opportunities[0].Id.Value);

                PraticeManagement.Utils.Generic.RedirectWithReturnTo(detailsLink, Request.Url.AbsoluteUri, Response);
            }
        }


        protected string GetOpportunityDetailsLink(int opportunityId, int index)
        {
            return Utils.Generic.GetTargetUrlWithReturn(Urls.OpportunityDetailsLink(opportunityId), Request.Url.AbsoluteUri);
        }

        public string GetProjectDetailUrl(Opportunity opty)
        {
            if (opty.Project != null)
            {
                return Utils.Generic.GetTargetUrlWithReturn(string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                 Constants.ApplicationPages.ProjectDetail,
                                 opty.Project.Id.ToString())
                                 , Request.Url.AbsoluteUri
                                 );
            }
            return string.Empty;
        }

        protected string GetPersonDetailsLink(int personId, int index)
        {
            return Urls.GetPersonDetailsUrl(
                     new Person(personId),
                     Request.Url.AbsoluteUri);
        }

        protected static string GetDaysOld(DateTime date, bool IsCreateDate)
        {
            var span = DateTime.Now.Subtract(date);

            var days = span.Days;

            if (IsCreateDate)
            {
                return days > 0 ? string.Format("{0}", days) : "Current";
            }
            else
            {
                return days > 0 ? string.Format("{0} day{1}", days, days == 1 ? string.Empty : "s") : "Current";
            }
        }

        protected static string GetFormattedEstimatedRevenue(Decimal? estimatedRevenue)
        {
            return estimatedRevenue.GetFormattedEstimatedRevenue();
        }

        protected static string GetRevenueTypeCaption(RevenueType type)
        {
            if (type == RevenueType.Undefined || type == RevenueType.Unknown)
            {
                return Constants.Formatting.UnknownValue;
            }
            return type.ToString();
        }

        protected static bool IsNeedToShowPerson(Person person)
        {
            if (person == null)
            {
                return false;
            }

            if (person.Status.Name == PersonStatusType.Inactive.ToString()
                || person.Status.Name == PersonStatusType.Terminated.ToString())
            {
                return false;
            }

            return true;
        }

        protected static string GetSalesTeam(Person SalesPerson, Person Owner)
        {
            string salesTeam = (IsNeedToShowPerson(SalesPerson) ? SalesPerson.LastName : string.Empty)
                               + "<br/>"
                               + (IsNeedToShowPerson(Owner) ? Owner.LastName : string.Empty);

            return salesTeam;
        }

        protected string GetNoteText(int OpportunityId)
        {
            if (EditedOpportunityList != null && EditedOpportunityList.Keys.Count > 0)
            {
                try
                {
                    return EditedOpportunityList[OpportunityId].ToString();
                }
                catch (KeyNotFoundException ex)
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        protected string GetNoteId(int OpportunityId)
        {
            if (EditedOpportunityNoteIdList != null && EditedOpportunityNoteIdList.Keys.Count > 0)
            {
                try
                {
                    return EditedOpportunityNoteIdList[OpportunityId].ToString();
                }
                catch (KeyNotFoundException ex)
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                ImageButton btnDelete = sender as ImageButton;

                ListViewItem row = btnDelete.NamingContainer as ListViewItem;

                TextBox txtNote = row.FindControl(NoteTextBoxID) as TextBox;
                int opportunityId = int.Parse(txtNote.Attributes[OpportunityIdValue]);

                if (!string.IsNullOrEmpty(txtNote.Attributes[NoteId]))
                {
                    int noteId = int.Parse(txtNote.Attributes[NoteId]);

                    ServiceCallers.Custom.Milestone(client => client.NoteDelete(noteId));

                    EditedOpportunityList.Remove(opportunityId);
                    EditedOpportunityNoteIdList.Remove(opportunityId);
                    txtNote.Attributes.Remove(NoteId);
                }

                txtNote.Text = string.Empty;
                txtNote.Attributes["MyDirty"] = "false";
            }
        }

        protected void btnSave_OnClick(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                ImageButton btnSave = sender as ImageButton;

                ListViewItem row = btnSave.NamingContainer as ListViewItem;

                ImageButton btnDelete = row.FindControl("imgbtnDelete") as ImageButton;

                TextBoxWatermarkExtender extender1 = ((Control)row).FindControl(Watermarker) as TextBoxWatermarkExtender;
                extender1.BehaviorID = "";

                TextBox txtNote = row.FindControl(NoteTextBoxID) as TextBox;
                int opportunityId = int.Parse(txtNote.Attributes[OpportunityIdValue]);

                if (!string.IsNullOrEmpty(txtNote.Attributes[NoteId]))
                {
                    int noteId = int.Parse(txtNote.Attributes[NoteId]);

                    var note = new Note
                    {
                        Author = new Person
                        {
                            Id = DataHelper.CurrentPerson.Id
                        },
                        CreateDate = DateTime.Now,
                        NoteText = txtNote.Text,
                        Target = NoteTarget.Opportunity,
                        TargetId = opportunityId,
                        Id = noteId
                    };

                    ServiceCallers.Custom.Milestone(client => client.NoteUpdate(note));

                    EditedOpportunityList[opportunityId] = txtNote.Text;

                }
                else
                {
                    if (!string.IsNullOrEmpty(txtNote.Text))
                    {
                        var note = new Note
                        {
                            Author = new Person
                            {
                                Id = DataHelper.CurrentPerson.Id
                            },
                            CreateDate = DateTime.Now,
                            NoteText = txtNote.Text,
                            Target = NoteTarget.Opportunity,
                            TargetId = opportunityId
                        };

                        int noteId = ServiceCallers.Custom.Milestone(client => client.NoteInsert(note));

                        txtNote.Attributes[NoteId] = noteId.ToString();

                        EditedOpportunityList.Add(opportunityId, txtNote.Text);

                        EditedOpportunityNoteIdList.Add(opportunityId, noteId);
                    }
                }

                txtNote.Attributes["MyDirty"] = "false";
            }
        }

        protected void cvLen_OnServerValidate(object source, ServerValidateEventArgs args)
        {

            CustomValidator val = source as CustomValidator;

            ListViewItem row = val.NamingContainer as ListViewItem;

            TextBox txtNote = row.FindControl(NoteTextBoxID) as TextBox;

            var length = txtNote.Text.Length;
            args.IsValid = length > 0 && length <= 2000;
        }

        protected void lvOpportunities_OnItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var dtlProposedPersons = e.Item.FindControl("dtlProposedPersons") as DataList;
                var dtlTeamStructure = e.Item.FindControl("dtlTeamStructure") as DataList;
                var hdnProposedPersonsIndexes = e.Item.FindControl("hdnProposedPersonsIndexes") as HiddenField;
                var hdnTeamStructure = e.Item.FindControl("hdnTeamStructure") as HiddenField;

                var oppty = (e.Item as ListViewDataItem).DataItem as Opportunity;

                var ddlPriority = e.Item.FindControl("ddlPriorityList") as DropDownList;

                if (ddlPriority != null)
                {
                    OpportunityPriority[] priorities = GetOpportunityPriorities();
                    DataHelper.FillListDefault(ddlPriority, string.Empty, priorities, true, "Id", "Priority");
                    ddlPriority.SelectedValue = oppty.Priority.Id.ToString();
                    ddlPriority.Attributes["OpportunityID"] = oppty.Id.Value.ToString();
                }

                if (oppty != null && oppty.ProposedPersons != null)
                {
                    dtlProposedPersons.DataSource = oppty.ProposedPersons.FindAll(op=>op.RelationType==(int) OpportunityPersonRelationType.ProposedResource).OrderBy(op => op.Person.LastName + op.Person.FirstName);
                    dtlTeamStructure.DataSource = oppty.ProposedPersons.FindAll(op => op.RelationType == (int)OpportunityPersonRelationType.TeamStructure).OrderBy(op => op.Person.LastName + op.Person.FirstName);
                    dtlProposedPersons.DataBind();
                }
                if (oppty.ProposedPersons != null)
                {
                    hdnProposedPersonsIndexes.Value = GetPersonsIndexesWithPersonTypeString(oppty.ProposedPersons, cblPotentialResources);
                    hdnTeamStructure.Value = GetTeamStructure(oppty.ProposedPersons);
                }
                if (!string.IsNullOrEmpty(oppty.OutSideResources))
                {
                    var hdnOutSideResources = e.Item.FindControl("hdnOutSideResources") as HiddenField;
                    var ltrlOutSideResources = e.Item.FindControl("ltrlOutSideResources") as Literal;
                    hdnOutSideResources.Value = oppty.OutSideResources;
                    if (!string.IsNullOrEmpty(oppty.OutSideResources) && oppty.OutSideResources[oppty.OutSideResources.Length - 1] == ';')
                    {
                        oppty.OutSideResources = oppty.OutSideResources.Substring(0, oppty.OutSideResources.Length - 1);
                    }
                    ltrlOutSideResources.Text = oppty.OutSideResources.Replace(";", "<br/>");
                }
            }
        }

        private OpportunityPriority[] GetOpportunityPriorities()
        {
            if (ViewState["OpportunityPrioritiesList"] == null)
            {
                var priorityList = OpportunityPriorityHelper.GetOpportunityPriorities(true);
                ViewState["OpportunityPrioritiesList"] = priorityList;
                return priorityList;
            }

            return ViewState["OpportunityPrioritiesList"] as OpportunityPriority[];
        }

        private string GetPersonsIndexesWithPersonTypeString(List<OpportunityPerson> optypersons, CheckBoxList cblPotentialResources)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var optyperson in optypersons.FindAll(op => op.RelationType == (int)OpportunityPersonRelationType.ProposedResource))
            {

                if (optyperson.Person != null && optyperson.Person.Id.HasValue)
                {
                    var item = cblPotentialResources.Items.FindByValue(optyperson.Person.Id.Value.ToString());
                    if (item != null)
                    {
                        sb.Append(cblPotentialResources.Items.IndexOf(
                                         cblPotentialResources.Items.FindByValue(optyperson.Person.Id.Value.ToString())
                                                                     ).ToString()
                                   );
                        sb.Append(':');
                        sb.Append(((int)optyperson.PersonType).ToString());
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

        protected void btnSaveProposedResources_OnClick(object sender, EventArgs e)
        {
            int opportunityId;
            if (Int32.TryParse(hdnCurrentOpportunityId.Value, out opportunityId))
            {
                var selectedList = hdnProposedResourceIdsWithTypes.Value;

                using (var serviceClient = new OpportunityServiceClient())
                {
                    serviceClient.OpportunityPersonInsert(opportunityId, selectedList, (int)OpportunityPersonRelationType.ProposedResource, hdnProposedOutSideResources.Value);
                }
            }
            hdnCurrentOpportunityId.Value = string.Empty;
        }

        protected void btnSaveTeamStructureHidden_OnClick(object sender, EventArgs e)
        {
            int opportunityId;
            if (Int32.TryParse(hdnCurrentOpportunityId.Value, out opportunityId))
            {
                var selectedList = hdnTeamStructure.Value;

                using (var serviceClient = new OpportunityServiceClient())
                {
                    serviceClient.OpportunityPersonInsert(opportunityId, selectedList, (int)OpportunityPersonRelationType.TeamStructure, string.Empty);
                }
            }
            hdnCurrentOpportunityId.Value = string.Empty;
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

        protected static string GetWrappedText(String descriptionText)
        {
            if (descriptionText == null)
            {
                return string.Format(Description, string.Empty);
            }

            descriptionText = descriptionText.Trim();


            if (descriptionText.Length > 500)
            {
                descriptionText = descriptionText.Substring(0, 500) + ".....";
            }

            for (int i = 0; i < descriptionText.Length; i = i + 15)
            {
                descriptionText = descriptionText.Insert(i, WordBreak);
            }


            return string.Format(Description, descriptionText); ;
        }

        private Table GetSummaryDetails()
        {
            var opportunities = DataHelper.GetFilteredOpportunitiesForDiscussionReview2(false);
            return OpportunitiesHelper.GetFormatedSummaryDetails(opportunities, PriorityTrendList, StatusChangesList);
        }

        protected void rpTeamStructure_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var ddlQty = e.Item.FindControl("ddlQuantity") as DropDownList;
            ddlQty.DataSource = Quantities;
            ddlQty.DataBind();

        }


    }
}


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

        public bool AllowAutoRedirectToDetails { get; set; }

        public OpportunityListFilterMode FilterMode { get; set; }

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

        public Opportunity[] GetOpportunities()
        {
            return
                FilterMode == OpportunityListFilterMode.GenericFilter ?
                DataHelper.GetFilteredOpportunities()
                :
                DataHelper.GetOpportunitiesForTargetPerson(TargetPersonId);
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

        public void DatabindOpportunities()
        {
            var opportunities = GetOpportunities();
            lvOpportunities.DataSource = opportunities;
            lvOpportunities.DataBind();

            updProposedResources.Visible = false;
            //  IsPostBack here means that method is called on postback
            //      so it means that it's coming from search and we should redirect if there's the only result
            if (IsPostBack && lvOpportunities.Items.Count == 1 && AllowAutoRedirectToDetails)
            {
                var detailsLink =
                    Urls.OpportunityDetailsLink(opportunities[0].Id.Value);

                PraticeManagement.Utils.Generic.RedirectWithReturnTo(detailsLink, Request.Url.AbsoluteUri, Response);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                updProposedResources.Visible = false;
            }
        }

        protected string GetOpportunityDetailsLink(int opportunityId, int index)
        {
            return Utils.Generic.GetTargetUrlWithReturn(Urls.OpportunityDetailsLink(opportunityId), Request.Url.AbsoluteUri);
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

        protected void btnOppotunity_Click(object sender, EventArgs e)
        {
            LinkButton btnOpportunityNumber = sender as LinkButton;
            ListViewItem row = btnOpportunityNumber.NamingContainer as ListViewItem;

            Button btn = row.FindControl("btnOpportunity")as Button; ;
            ucProposedResources.OpportunityId = int.Parse(btn.Text);
            updProposedResources.Update();
            updProposedResources.Visible = true;

            
            int RowIndex = lvOpportunities.Items.IndexOf(row as ListViewDataItem);

            HtmlTableRow SelectedRow = row.FindControl("trOpportunity") as HtmlTableRow;
            SelectedRow.Style.Add("background-color", "#e2ebff");

            if (!string.IsNullOrEmpty(hdnPreviouslyClickedRowIndex.Value))
            {
                int previousRowIndex = Convert.ToInt32(hdnPreviouslyClickedRowIndex.Value);
                if (previousRowIndex != RowIndex)
                {
                    TextBox txtNote = lvOpportunities.Items[previousRowIndex].FindControl(NoteTextBoxID) as TextBox;
                    TextBoxWatermarkExtender extender = lvOpportunities.Items[previousRowIndex].FindControl(Watermarker) as TextBoxWatermarkExtender;
                    extender.BehaviorID = "behavior";
                    HtmlTableRow previouslySelectedRow = lvOpportunities.Items[previousRowIndex].FindControl("trOpportunity") as HtmlTableRow;
                    previouslySelectedRow.Style.Add("background-color", "");
                }
            }

            hdnPreviouslyClickedRowIndex.Value = RowIndex.ToString();
            TextBox txtNoteBox = ((Control)row).FindControl(NoteTextBoxID) as TextBox;
            TextBoxWatermarkExtender extender1 = ((Control)row).FindControl(Watermarker) as TextBoxWatermarkExtender;
            extender1.BehaviorID = string.Empty;
            ucProposedResources.FillProposedResources();
            ucProposedResources.FillPotentialResources();
        }

        public bool SaveAllNotes()
        {
            Page.Validate(valsum.ValidationGroup);
            if (Page.IsValid)
            {
                foreach (ListViewItem item in lvOpportunities.Items)
                {
                    TextBoxWatermarkExtender extender1 = item.FindControl(Watermarker) as TextBoxWatermarkExtender;
                    extender1.BehaviorID = "";
                    TextBox txtNote = item.FindControl(NoteTextBoxID) as TextBox;

                    if (!string.IsNullOrEmpty(txtNote.Text) &&
                        txtNote.Text != extender1.WatermarkText)
                    {
                        int opportunityId = int.Parse(txtNote.Attributes[OpportunityIdValue]);

                        bool isSaved = EditedOpportunityList.Keys.Any(k => k == opportunityId);

                        if (!isSaved)
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

                }

                return true;
            }
            return false;
        }
        
    }
}

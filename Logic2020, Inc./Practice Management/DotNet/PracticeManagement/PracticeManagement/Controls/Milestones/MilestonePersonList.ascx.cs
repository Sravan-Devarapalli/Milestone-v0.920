using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.TimeEntry;
using PraticeManagement.Controls;
using PraticeManagement.MilestonePersonService;
using PraticeManagement.MilestoneService;
using PraticeManagement.PersonService;
using PraticeManagement.Security;
using PraticeManagement.Utils;
using Resources;
using System.Web.UI.HtmlControls;
using System.Linq;
using PraticeManagement.PersonRoleService;
using PraticeManagement.Controls.Milestones;

namespace PraticeManagement.Controls.Milestones
{
    public partial class MilestonePersonList : UserControl
    {
        #region Constants

        private const string MILESTONE_PERSON_ID_ARGUMENT = "milestonePersonId";
        private const int AMOUNT_COLUMN_INDEX = 7;
        private const string MILESTONE_PERSONS_KEY = "MilestonePersons";
        private const string ADD_MILESTONE_PERSON_ENTRIES_KEY = "ADD_MILESTONE_PERSON_ENTRIES_KEY";
        private const string PERSONSLISTFORMILESTONE_KEY = "PERSONSLISTFORMILESTONE_KEY";
        private const string ROLELISTFORPERSONS_KEY = "ROLELISTFORPERSONS_KEY";
        private const string DDLPERSON_KEY = "ddlPerson";
        private const string DDLROLE_KEY = "ddlRole";
        private const string DEFAULT_NUMBER_HOURS_PER_DAY = "8";
        private const string PERSON_ID_KEY = "PERSON_ID_KEY";
        private const string mpbar = "mpbar";
        private const string DuplPersonName = "The specified person is already assigned on this milestone.";
        private const string lblTargetMargin = "lblTargetMargin";
        private const string dpPersonStartInsert = "dpPersonStartInsert";
        private const string dpPersonEndInsert = "dpPersonEndInsert";
        private const string txtAmountInsert = "txtAmountInsert";
        private const string txtHoursPerDayInsert = "txtHoursPerDayInsert";
        private const string txtHoursInPeriodInsert = "txtHoursInPeriodInsert";
        private const string milestoneHasTimeEntries = "Cannot delete milesone person because this person has already entered time for this milestone.";

        #endregion

        #region Fields

        private ExceptionDetail _internalException;
        private Milestone _milestoneValue;
        private bool errorOccured = false;

        private SeniorityAnalyzer _seniorityAnalyzer;

        #endregion

        #region Properties

        private PraticeManagement.MilestoneDetail HostingPage
        {
            get { return ((PraticeManagement.MilestoneDetail)Page); }
        }

        public String ExMessage { get; set; }

        public Milestone Milestone
        {
            get
            {
                return HostingPage.Milestone;
            }
        }

        public GridView gvMilestonePersonEntriesObject
        {
            get { return gvMilestonePersonEntries; }
        }

        public MessageLabel lblResultMessageObject
        {
            get
            {
                return lblResultMessage;
            }
        }

        public ValidationSummary vsumMileStonePersonsObject
        {
            get
            {
                return vsumMileStonePersons;
            }
        }

        public List<MilestonePerson> MilestonePersons
        {
            get
            {
                return ViewState[MILESTONE_PERSONS_KEY] as List<MilestonePerson>;
            }
            set
            {
                ViewState[MILESTONE_PERSONS_KEY] = value;
            }
        }

        public List<MilestonePersonEntry> AddMilestonePersonEntries
        {
            get
            {
                return ViewState[ADD_MILESTONE_PERSON_ENTRIES_KEY] as List<MilestonePersonEntry>;
            }
            set
            {
                ViewState[ADD_MILESTONE_PERSON_ENTRIES_KEY] = value;
            }
        }


        public List<Dictionary<string, string>> repeaterOldValues
        {
            get
            {
                return ViewState["REPEATEROLDVALUES_KEY"] as List<Dictionary<string, string>>;
            }
            set
            {
                ViewState["REPEATEROLDVALUES_KEY"] = value;
            }
        }


        public Person[] PersonsListForMilestone
        {
            get
            {
                if (ViewState[PERSONSLISTFORMILESTONE_KEY] != null)
                {
                    return ViewState[PERSONSLISTFORMILESTONE_KEY] as Person[];
                }

                using (var serviceClient = new PersonServiceClient())
                {
                    try
                    {
                        Person[] persons = serviceClient.PersonListAllForMilestone(null, Milestone.StartDate, Milestone.EndDate);
                        ViewState[PERSONSLISTFORMILESTONE_KEY] = persons;
                        Array.Sort(persons);
                        return persons;
                    }
                    catch (CommunicationException)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }

            }
            set
            {
                ViewState[PERSONSLISTFORMILESTONE_KEY] = value;
            }
        }


        public PersonRole[] RoleListForPersons
        {
            get
            {
                if (ViewState[ROLELISTFORPERSONS_KEY] != null)
                {
                    return ViewState[ROLELISTFORPERSONS_KEY] as PersonRole[];
                }

                using (var serviceClient = new PersonRoleServiceClient())
                {
                    try
                    {
                        PersonRole[] roles = serviceClient.GetPersonRoles();

                        ViewState[ROLELISTFORPERSONS_KEY] = roles;
                        return roles;
                    }
                    catch (CommunicationException)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }

            }
            set
            {
                ViewState[ROLELISTFORPERSONS_KEY] = value;
            }

        }


        private bool? IsUserHasPermissionOnProject
        {
            get
            {
                int? milestoneId = HostingPage.MilestoneId;

                if (milestoneId.HasValue)
                {
                    if (ViewState["HasPermission"] == null)
                    {
                        ViewState["HasPermission"] = DataHelper.IsUserHasPermissionOnProject(HostingPage.User.Identity.Name, milestoneId.Value, false);
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
                int? id = HostingPage.MilestoneId;
                if (id.HasValue)
                {
                    if (ViewState["IsOwnerOfProject"] == null)
                    {
                        ViewState["IsOwnerOfProject"] = DataHelper.IsUserIsOwnerOfProject(HostingPage.User.Identity.Name, id.Value, false);
                    }
                    return (bool)ViewState["IsOwnerOfProject"];
                }

                return null;
            }
        }

        public bool isInsertedRowsAreNotsaved
        {
            get
            {
                return (gvMilestonePersonEntries.EditIndex > -1 || repPerson.Items.Count > 0) ? true : false;
            }
        }

        private bool IsSaveCommit
        {
            get;
            set;
        }

        public bool ISDatBindRows { get; set; }

        #endregion

        #region Methods

        #region Validation

        protected void custPersonStart_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var dpPersonStart = ((Control)source).Parent.FindControl("dpPersonStart") as DatePicker;
            args.IsValid = dpPersonStart.DateValue.Date >= Milestone.StartDate.Date;
        }

        protected void custPersonEnd_ServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator custPerson = source as CustomValidator;
            GridViewRow gvRow = custPerson.NamingContainer as GridViewRow;
            var ddl = gvRow.FindControl("ddlPersonName") as DropDownList;
            Person person = GetPersonBySelectedValue(ddl.SelectedValue);

            var dpPersonEnd = ((Control)source).Parent.FindControl("dpPersonEnd") as DatePicker;
            bool isGreaterThanMilestone = dpPersonEnd.DateValue <= Milestone.ProjectedDeliveryDate;

            if (!isGreaterThanMilestone)
            {

                bool terminationAndCompensation =
                    ChechTerminationAndCompensation(dpPersonEnd.DateValue, person);


            }

            var entries = new List<MilestonePersonEntry>();

            foreach (var milestonePerson in MilestonePersons)
            {
                entries.AddRange(milestonePerson.Entries);
            }

            entries = entries.OrderBy(entry => entry.ThisPerson.LastName).ThenBy(ent => ent.StartDate).AsQueryable().ToList();
            args.IsValid = isGreaterThanMilestone;
        }

        protected void custPerson_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            CustomValidator custPerson = sender as CustomValidator;
            GridViewRow gvRow = custPerson.NamingContainer as GridViewRow;
            var ddl = gvRow.FindControl("ddlPersonName") as DropDownList;
            var dpPersonStart = ((Control)sender).Parent.FindControl("dpPersonStart") as DatePicker;
            var dpPersonEnd = ((Control)sender).Parent.FindControl("dpPersonEnd") as DatePicker;
            Person person = GetPersonBySelectedValue(ddl.SelectedValue);

            List<MilestonePerson> MilestonePersonList = MilestonePersons.Where(mp => mp.Person.Id.Value == person.Id.Value).AsQueryable().ToList();

            List<MilestonePersonEntry> entries = new List<MilestonePersonEntry>();

            foreach (var item in MilestonePersonList)
            {
                entries.AddRange(item.Entries);
            }

            foreach (MilestonePersonEntry entry in entries)
            {
                if (person == null ||
                    person.HireDate > entry.StartDate ||
                    (person.TerminationDate.HasValue && entry.EndDate.HasValue &&
                     person.TerminationDate.Value < entry.EndDate))
                {
                    args.IsValid = false;
                    break;
                }
            }

            if (person == null ||
                               person.HireDate > dpPersonStart.DateValue.Date ||
                               (person.TerminationDate.HasValue &&
                                person.TerminationDate.Value < dpPersonEnd.DateValue.Date))
            {
                args.IsValid = false;
            }
        }

        protected void custDuplicatedPerson_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!String.IsNullOrEmpty(ExMessage))
            {
                args.IsValid = !(ExMessage == DuplPersonName);
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void reqHourlyRevenue_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = !Milestone.IsHourlyAmount || !string.IsNullOrEmpty(e.Value);
        }

        protected void custPeriodOvberlapping_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            var dpPersonStart = ((Control)sender).Parent.FindControl("dpPersonStart") as DatePicker;
            var dpPersonEnd = ((Control)sender).Parent.FindControl("dpPersonEnd") as DatePicker;

            DateTime startDate = dpPersonStart.DateValue;
            DateTime endDate =
                dpPersonEnd.DateValue != DateTime.MinValue ? dpPersonEnd.DateValue : Milestone.ProjectedDeliveryDate;

            CustomValidator custPerson = sender as CustomValidator;
            GridViewRow gvRow = custPerson.NamingContainer as GridViewRow;
            var ddl = gvRow.FindControl("ddlPersonName") as DropDownList;
            Person person = GetPersonBySelectedValue(ddl.SelectedValue);

            List<MilestonePersonEntry> entries = new List<MilestonePersonEntry>();

            foreach (var item in MilestonePersons)
            {
                entries.AddRange(item.Entries);
            }
            entries = entries.OrderBy(entry => entry.ThisPerson.LastName).ThenBy(ent => ent.StartDate).AsQueryable().ToList();
            // Validate overlapping with other entries.
            for (int i = 0; i < entries.Count; i++)
            {
                if (i != gvMilestonePersonEntries.EditIndex && entries[i].ThisPerson.Id == person.Id.Value)
                {
                    DateTime entryStartDate = entries[i].StartDate;
                    DateTime entryEndDate =
                        entries[i].EndDate.HasValue
                            ?
                                entries[i].EndDate.Value
                            : Milestone.ProjectedDeliveryDate;

                    if ((startDate >= entryStartDate && startDate <= entryEndDate) ||
                        (endDate >= entryStartDate && endDate <= entryEndDate))
                    {
                        e.IsValid = false;
                        break;
                    }
                }
            }
        }

        protected void custPeriodVacationOverlapping_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            var custPersonStartDate = ((Control)sender).Parent.Parent.FindControl("custPersonStart") as CustomValidator;
            var compPersonEnd = ((Control)sender).Parent.Parent.FindControl("compPersonEnd") as CompareValidator;

            CustomValidator custPerson = sender as CustomValidator;
            GridViewRow gvRow = custPerson.NamingContainer as GridViewRow;
            var ddlPersonName = gvRow.FindControl("ddlPersonName") as DropDownList;

            var isStartDateValid = ((System.Web.UI.WebControls.BaseValidator)(custPersonStartDate)).IsValid;
            var isEndDateValid = compPersonEnd.IsValid;

            if (isStartDateValid && isEndDateValid)
            {
                var dpPersonStart = ((Control)sender).Parent.FindControl("dpPersonStart") as DatePicker;
                var dpPersonEnd = ((Control)sender).Parent.FindControl("dpPersonEnd") as DatePicker;

                DateTime startDate = dpPersonStart.DateValue;
                DateTime endDate =
                    dpPersonEnd.DateValue != DateTime.MinValue ? dpPersonEnd.DateValue : Milestone.ProjectedDeliveryDate;

                // Validate overlapping with other entries.
                int days = GetPersonWorkDaysNumber(startDate, endDate, ddlPersonName);
                if (days == 0)
                {
                    e.IsValid = false;
                }
            }
        }

        #endregion

        private bool ChechTerminationAndCompensation(DateTime endDate, Person person)
        {

            DateTime termDate =
                person.TerminationDate.HasValue
                    ?
                        person.TerminationDate.Value
                    :
                        DateTime.MaxValue;

            return
                termDate >= endDate &&
                DataHelper.IsCompensationCoversMilestone(person, person.HireDate, endDate);
        }

        public Person GetPersonBySelectedValue(String id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (ViewState[PERSON_ID_KEY] != null && (ViewState[PERSON_ID_KEY] as Person) != null && (ViewState[PERSON_ID_KEY] as Person).Id.ToString() == id)
                {
                    return (ViewState[PERSON_ID_KEY] as Person);
                }

                using (var serviceCLient = new PersonServiceClient())
                {
                    try
                    {
                        var person = serviceCLient.GetPersonById(int.Parse(id));
                        ViewState[PERSON_ID_KEY] = person;
                        return person;
                    }
                    catch (CommunicationException)
                    {
                        serviceCLient.Abort();
                        throw;
                    }
                }
            }
            return null;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            _seniorityAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);

            base.OnInit(e);
        }

        private void PopulateData(MilestonePerson milestonePerson)
        {
            milestonePerson.Milestone = new Milestone();
            milestonePerson.Milestone.Id = HostingPage.SelectedId.Value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            IsSaveCommit = true;
            ISDatBindRows = true;
            if (!IsPostBack)
            {

                GetLatestData();
            }
            StoreRepeterEntriiesInViewState();
        }

        public void GetLatestData()
        {
            int? id = HostingPage.MilestoneId;
            if (id.HasValue)
            {
                if (repeaterOldValues != null)
                {
                    repeaterOldValues.Clear();
                    repeaterOldValues = repeaterOldValues;

                }

                if (AddMilestonePersonEntries != null)
                {
                    AddMilestonePersonEntries.Clear();
                    AddMilestonePersonEntries = AddMilestonePersonEntries;
                    repPerson.DataSource = AddMilestonePersonEntries;
                    repPerson.DataBind();
                }

                gvMilestonePersonEntries.EditIndex = -1;
                lblResultMessage.ClearMessage();
                List<MilestonePerson> milestonePersons = GetMilestonePersons();
                MilestonePersons = milestonePersons;
                FillMilestonePersonEntries(milestonePersons);
            }
        }

        private void FillMilestonePersonEntries(List<MilestonePerson> milestonePersons)
        {
            var entries = new List<MilestonePersonEntry>();

            foreach (var milestonePerson in milestonePersons)
            {
                if (milestonePerson != null)
                {
                    if (!Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName))
                    {
                        if (IsUserHasPermissionOnProject.HasValue && !IsUserHasPermissionOnProject.Value
                            && IsUserisOwnerOfProject.HasValue && !IsUserisOwnerOfProject.Value)
                            Response.Redirect(@"~\GuestPages\AccessDenied.aspx");
                    }

                    _seniorityAnalyzer.IsOtherGreater(milestonePerson.Person);

                    entries.AddRange(milestonePerson.Entries);
                }
            }

            PopulateControls(Milestone);

            entries = entries.OrderBy(entry => entry.ThisPerson.LastName).ThenBy(ent => ent.StartDate).AsQueryable().ToList();

            BindEntriesGrid(entries);
        }


        private void BindEntriesGrid(List<MilestonePersonEntry> entries)
        {
            var tmp = new List<MilestonePersonEntry>(entries);

            if (tmp.Count == 0)
            {
                thInsertMilestonePerson.Visible = true;
                thHourlyRate.Visible = Milestone.IsHourlyAmount;
            }
            else
            {
                thInsertMilestonePerson.Visible = false;
            }

            gvMilestonePersonEntries.DataSource = tmp;

            gvMilestonePersonEntries.Columns[AMOUNT_COLUMN_INDEX].Visible = Milestone.IsHourlyAmount;
            gvMilestonePersonEntries.DataBind();
        }

        protected void gvMilestonePersonEntries_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var entry = (MilestonePersonEntry)e.Row.DataItem;

                var ddlPersonName = e.Row.FindControl("ddlPersonName") as DropDownList;
                var ddlRole = e.Row.FindControl("ddlRole") as DropDownList;
                var lableTargetMargin = e.Row.FindControl(lblTargetMargin) as Label;
                var lnkPersonName = e.Row.FindControl("lnkPersonName") as HyperLink;
                var lblHoursPerDay = e.Row.FindControl("lblHoursPerDay") as Label;
                var lblHoursInPeriodDay = e.Row.FindControl("lblHoursInPeriodDay") as Label;

                if (lblHoursPerDay != null)
                {
                    lblHoursPerDay.Text = entry.HoursPerDay.ToString("0.00");
                }

                if (lblHoursInPeriodDay != null)
                {
                    lblHoursInPeriodDay.Text = entry.ProjectedWorkload.ToString("0.00");
                }

                if (lnkPersonName != null)
                {
                    lnkPersonName.NavigateUrl = GetMpeRedirectUrl(entry.MilestonePersonId);
                    lnkPersonName.Attributes["PersonId"] = entry.ThisPerson.Id.ToString();

                }

                DateTime startDate = Milestone.StartDate;
                DateTime endDate = Milestone.ProjectedDeliveryDate;

                if (ddlPersonName != null)
                {
                    DataHelper.FillPersonList(ddlPersonName, string.Empty, PersonsListForMilestone, string.Empty);

                    List<MilestonePerson> MilestonePersonList = MilestonePersons.Where(mp => mp.Id == entry.MilestonePersonId).AsQueryable().ToList();


                    bool result = false;

                    foreach (var mp in MilestonePersonList)
                    {
                        if (mp.Entries.Any(en => en.HasTimeEntries == true))
                        {
                            result = true;
                            break;
                        }
                    }

                    ddlPersonName.SelectedValue = entry.ThisPerson.Id.Value.ToString();
                    ddlPersonName.Enabled = !result;
                }

                if (ddlRole != null)
                {
                    DataHelper.FillListDefault(ddlRole, string.Empty, RoleListForPersons, false);

                    ddlRole.SelectedIndex =
                        ddlRole.Items.IndexOf(
                                                 ddlRole.Items.FindByValue(entry.Role != null
                                                                               ? entry.Role.Id.ToString()
                                                                               : string.Empty));

                }


                var rowSa = new SeniorityAnalyzer(DataHelper.CurrentPerson);
                if (rowSa.IsOtherGreater(entry.ThisPerson))
                {
                    if (lableTargetMargin != null)
                    {
                        lableTargetMargin.Text = Resources.Controls.HiddenCellText;
                    }

                    if (!(IsUserisOwnerOfProject.HasValue && IsUserisOwnerOfProject.Value))
                    {
                        if (!Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName)
                            || !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName))// #2817: DirectorRoleName is added as per the requirement.
                        {

                            var imgMilestonePersonEntryEdit = e.Row.FindControl("imgMilestonePersonEntryEdit") as ImageButton;

                            if (imgMilestonePersonEntryEdit != null)
                            {
                                imgMilestonePersonEntryEdit.Enabled = false;
                            }
                            else
                            {
                                e.Row.Enabled = false;
                            }

                        }
                    }
                }
                else if (lableTargetMargin != null)
                {
                    lableTargetMargin.Text = (entry.ComputedFinancials != null) ? string.Format(Constants.Formatting.PercentageFormat, entry.ComputedFinancials.TargetMargin) : string.Format(Constants.Formatting.PercentageFormat, 0);
                }

            }
        }

        protected void repPerson_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var repItem = e.Item;
            var bar = repItem.FindControl(mpbar) as MilestonePersonBar;

            if (gvMilestonePersonEntries.Rows.Count > 0)
            {
                if (!(gvMilestonePersonEntries.Rows.Count % 2 == 0))
                {
                    if (e.Item.ItemType == ListItemType.Item)
                    {
                        bar.BarColor = "#F9FAFF";
                    }
                    else if (e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        bar.BarColor = "White";
                    }
                }
            }

            DateTime startDate = Milestone.StartDate;
            DateTime endDate = Milestone.ProjectedDeliveryDate;

            var tdAmountInsert = bar.FindControl("tdAmountInsert") as HtmlTableCell;
            var ddlPerson = bar.FindControl(DDLPERSON_KEY) as DropDownList;
            var ddlRole = bar.FindControl(DDLROLE_KEY) as DropDownList;
            var dpPersonStart = bar.FindControl(dpPersonStartInsert) as DatePicker;
            var dpPersonEnd = bar.FindControl(dpPersonEndInsert) as DatePicker;
            var txtAmount = bar.FindControl(txtAmountInsert) as TextBox;
            var txtHoursPerDay = bar.FindControl(txtHoursPerDayInsert) as TextBox;

            if (dpPersonStart != null)
                dpPersonStart.DateValue = DateTime.Parse(repeaterOldValues[e.Item.ItemIndex][dpPersonStartInsert]);

            if (dpPersonEnd != null)
                dpPersonEnd.DateValue = DateTime.Parse(repeaterOldValues[e.Item.ItemIndex][dpPersonEndInsert]);

            if (ddlPerson != null)
            {
                DataHelper.FillPersonList(ddlPerson, string.Empty, PersonsListForMilestone, string.Empty);

                if (!string.IsNullOrEmpty(repeaterOldValues[e.Item.ItemIndex][DDLPERSON_KEY]))
                    ddlPerson.SelectedValue = repeaterOldValues[e.Item.ItemIndex][DDLPERSON_KEY];
            }

            if (ddlRole != null)
            {
                DataHelper.FillListDefault(ddlRole, string.Empty, RoleListForPersons, false);

                if (!string.IsNullOrEmpty(repeaterOldValues[e.Item.ItemIndex][DDLROLE_KEY]))
                    ddlRole.SelectedValue = repeaterOldValues[e.Item.ItemIndex][DDLROLE_KEY];
            }


            if (tdAmountInsert != null)
                tdAmountInsert.Visible = Milestone.IsHourlyAmount;

            if (tdAmountInsert.Visible && txtAmount != null)
                txtAmount.Text = repeaterOldValues[e.Item.ItemIndex][txtAmountInsert];

            if (txtHoursPerDay != null)
                txtHoursPerDay.Text = repeaterOldValues[e.Item.ItemIndex][txtHoursPerDayInsert];

        }


        public List<MilestonePerson> GetMilestonePersons()
        {
            using (var serviceClient = new MilestonePersonServiceClient())
            {
                try
                {
                    return serviceClient.GetMilestonePersonsDetailsByMileStoneId(HostingPage.MilestoneId.Value).AsQueryable().ToList();
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        private void PopulateControls(Milestone milestone)
        {
            // Security
            bool isReadOnly =
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) &&
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName) &&
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName) &&
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName);// #2817: DirectorRoleName is added as per the requirement.

            btnAddPerson.Visible = gvMilestonePersonEntries.Columns[0].Visible = !isReadOnly;
        }

        protected string GetMpeRedirectUrl(object milestonePersonId)
        {
            var mpePageUrl =
               string.Format(
                      Constants.ApplicationPages.RedirectMilestonePersonIdFormat,
                      Constants.ApplicationPages.MilestonePersonDetail,
                      Milestone.Id.Value,
                      milestonePersonId);

            var url = Request.Url.AbsoluteUri;

            if (!HostingPage.SelectedId.HasValue)
            {

                url = url.Replace("?", "?id=" + HostingPage.MilestoneId.Value.ToString() + "&");
            }

            return PraticeManagement.Utils.Generic.GetTargetUrlWithReturn(mpePageUrl, url);
        }

        protected void btnAddPerson_Click(object sender, EventArgs e)
        {
            lblResultMessage.ClearMessage();
            AddRowAndBindRepeater(null);
        }

        private void StoreRepeterEntriiesInViewState()
        {
            var repoldValues = new List<Dictionary<string, string>>();

            foreach (RepeaterItem repItem in repPerson.Items)
            {
                var bar = repItem.FindControl(mpbar) as MilestonePersonBar;

                var ddlPerson = bar.FindControl(DDLPERSON_KEY) as DropDownList;
                var ddlRole = bar.FindControl(DDLROLE_KEY) as DropDownList;
                var dpPersonStart = bar.FindControl(dpPersonStartInsert) as DatePicker;
                var dpPersonEnd = bar.FindControl(dpPersonEndInsert) as DatePicker;
                var txtAmount = bar.FindControl(txtAmountInsert) as TextBox;
                var txtHoursPerDay = bar.FindControl(txtHoursPerDayInsert) as TextBox;
                var txtHoursInPeriod = bar.FindControl(txtHoursInPeriodInsert) as TextBox;

                var dic = new Dictionary<string, string>();
                dic.Add(DDLPERSON_KEY, ddlPerson.SelectedValue);
                dic.Add(DDLROLE_KEY, ddlRole.SelectedValue);
                dic.Add(dpPersonStartInsert, dpPersonStart.DateValue.ToString());
                dic.Add(dpPersonEndInsert, dpPersonEnd.DateValue.ToString());
                dic.Add(txtAmountInsert, txtAmount.Text);
                dic.Add(txtHoursPerDayInsert, txtHoursPerDay.Text);
                dic.Add(txtHoursInPeriodInsert, txtHoursInPeriod.Text);
                repoldValues.Add(dic);
            }

            repeaterOldValues = repoldValues;
        }

        internal void AddRowAndBindRepeater(Dictionary<string, string> dic)
        {
            if (AddMilestonePersonEntries == null)
            {
                AddMilestonePersonEntries = new List<MilestonePersonEntry>();
            }

            if (repeaterOldValues == null)
            {
                repeaterOldValues = new List<Dictionary<string, string>>();
            }

            var entry = new MilestonePersonEntry() { };

            entry.StartDate = Milestone.StartDate;
            entry.EndDate = Milestone.ProjectedDeliveryDate;

            if (dic == null)
            {
                dic = new Dictionary<string, string>();
                dic.Add(DDLPERSON_KEY, string.Empty);
                dic.Add(DDLROLE_KEY, string.Empty);
                dic.Add(dpPersonStartInsert, Milestone.StartDate.ToString());
                dic.Add(dpPersonEndInsert, Milestone.ProjectedDeliveryDate.ToString());
                dic.Add(txtAmountInsert, string.Empty);
                dic.Add(txtHoursPerDayInsert, string.Empty);
                dic.Add(txtHoursInPeriodInsert, string.Empty);
            }

            repeaterOldValues.Add(dic);
            repeaterOldValues = repeaterOldValues;

            AddMilestonePersonEntries.Add(entry);

            AddMilestonePersonEntries = AddMilestonePersonEntries;
            repPerson.DataSource = AddMilestonePersonEntries;
            repPerson.DataBind();
        }

        public void RemoveItemAndDaabindRepeater(int barIndex)
        {
            AddMilestonePersonEntries.RemoveAt(barIndex);
            repeaterOldValues.RemoveAt(barIndex);
            AddMilestonePersonEntries = AddMilestonePersonEntries;
            repPerson.DataSource = AddMilestonePersonEntries;
            repPerson.DataBind();
        }

        public void AddAndBindRow(RepeaterItem repItem, bool isSaveCommit, bool iSDatBindRows = true)
        {
            var bar = repItem.FindControl(mpbar) as MilestonePersonBar;
            var ddlPerson = bar.FindControl(DDLPERSON_KEY) as DropDownList;
            var ddlRole = bar.FindControl(DDLROLE_KEY) as DropDownList;
            var dpPersonStart = bar.FindControl(dpPersonStartInsert) as DatePicker;
            var dpPersonEnd = bar.FindControl(dpPersonEndInsert) as DatePicker;
            var txtAmount = bar.FindControl(txtAmountInsert) as TextBox;
            var txtHoursPerDay = bar.FindControl(txtHoursPerDayInsert) as TextBox;
            var txtHoursInPeriod = bar.FindControl(txtHoursInPeriodInsert) as TextBox;

            MilestonePerson milestonePerson = new MilestonePerson();
            milestonePerson.Milestone = Milestone;
            milestonePerson.Entries = new List<MilestonePersonEntry>();
            var entry = new MilestonePersonEntry();
            entry.StartDate = dpPersonStart.DateValue;
            entry.EndDate = dpPersonEnd.DateValue != DateTime.MinValue ? (DateTime?)dpPersonEnd.DateValue : null;
            Person person = null;
            if (!string.IsNullOrEmpty(ddlPerson.SelectedValue))
            {
                 person = GetPersonBySelectedValue(ddlPerson.SelectedValue);

                entry.ThisPerson = person;
                milestonePerson.Person = person;

            }

            // Role
            if (!string.IsNullOrEmpty(ddlRole.SelectedValue))
            {
                entry.Role =
                    new PersonRole
                    {
                        Id = int.Parse(ddlRole.SelectedValue),
                        Name = ddlRole.SelectedItem.Text
                    };
            }
            else
                entry.Role = null;

            // Amount
            if (!string.IsNullOrEmpty(txtAmount.Text))
            {
                entry.HourlyAmount = decimal.Parse(txtAmount.Text);
            }

            if (String.IsNullOrEmpty(txtHoursPerDay.Text) && String.IsNullOrEmpty(txtHoursInPeriod.Text))
            {
                txtHoursPerDay.Text = DEFAULT_NUMBER_HOURS_PER_DAY;
            }


            // Flags

            bool isHoursInPeriodChanged = !String.IsNullOrEmpty(txtHoursInPeriod.Text) &&
                                          (entry.ProjectedWorkloadWithVacation != decimal.Parse(txtHoursInPeriod.Text));

            // Check if need to recalculate Hours per day value
            // Get working days person on Milestone for current person
            DateTime endDate = entry.EndDate.HasValue ? entry.EndDate.Value : Milestone.ProjectedDeliveryDate;
            int days = GetPersonWorkDaysNumber(entry.StartDate, endDate, ddlPerson);
            decimal hoursPerDay;

            if (isHoursInPeriodChanged)
            {
                // Recalculate hours per day according to HoursInPerod 
                hoursPerDay = (days != 0) ? decimal.Round(decimal.Parse(txtHoursInPeriod.Text) / days, 2) : 0;
                // If calculated value more then 24 hours set 24 hours as maximum value for working day
                entry.HoursPerDay = (hoursPerDay > 24M) ? 24M : hoursPerDay;
                // Recalculate Hours In Period
                entry.ProjectedWorkload = entry.HoursPerDay * days;
            }
            else
            {
                // If Hours Per Day is ommited set 8 hours as default value for working day
                entry.HoursPerDay = !String.IsNullOrEmpty(txtHoursPerDay.Text)
                                        ? decimal.Parse(txtHoursPerDay.Text)
                                        : 8M;
                // Recalculate Hours In Period
                entry.ProjectedWorkload = entry.HoursPerDay * days;
            }



            if (MilestonePersons.Any(mp => mp.Person.Id == entry.ThisPerson.Id))
            {
                var index = MilestonePersons.FindIndex(mp => mp.Person.Id == entry.ThisPerson.Id);
                var mperson = MilestonePersons[index];

                //mperson.Person = person;
                if (mperson.Id.HasValue)
                {
                    entry.MilestonePersonId = mperson.Id.Value;
                }

                mperson.Entries.Add(entry);

                if (isSaveCommit)
                    SaveData(ref mperson, iSDatBindRows);

                MilestonePersons[index] = mperson;

            }
            else
            {
                milestonePerson.Entries.Add(entry);
                if (isSaveCommit)
                {
                    SaveData(ref milestonePerson, iSDatBindRows);
                }

                MilestonePersons.Add(milestonePerson);
            }

            MilestonePersons = MilestonePersons;

            if (isSaveCommit && iSDatBindRows)
            {
                FillMilestonePersonEntries(MilestonePersons);
            }

        }

        protected void imgMilestonePersonEntryEdit_OnClick(object sender, EventArgs e)
        {
            ImageButton imgEdit = sender as ImageButton;
            GridViewRow row = imgEdit.NamingContainer as GridViewRow;
            OnEditClick(row.DataItemIndex);
        }

        protected void imgMilestonePersonDelete_OnClick(object sender, EventArgs e)
        {
            ImageButton imgDelete = sender as ImageButton;
            GridViewRow row = imgDelete.NamingContainer as GridViewRow;

            var entries = new List<MilestonePersonEntry>();

            foreach (var mP in MilestonePersons)
            {
                entries.AddRange(mP.Entries);
            }
            entries = entries.OrderBy(entry => entry.ThisPerson.LastName).ThenBy(ent => ent.StartDate).AsQueryable().ToList();

            MilestonePersonEntry mpentry = entries[row.DataItemIndex];
            var index = MilestonePersons.FindIndex(mp => mp.Id == mpentry.MilestonePersonId);
            var mperson = MilestonePersons[index];

            mperson.Person = GetPersonBySelectedValue(mperson.Person.Id.Value.ToString());

            lblResultMessage.ClearMessage();
            if (CheckTimeEntriesExist(mpentry.MilestonePersonId, mpentry.StartDate, mpentry.EndDate, true, true))
            {
                lblResultMessage.ShowErrorMessage(milestoneHasTimeEntries);
                return;
            }

            if (mperson.Entries.Count > 1)
            {
                mperson.Entries.RemoveAll(mpe => mpe.MilestonePersonId == mpentry.MilestonePersonId && mpe.StartDate == mpentry.StartDate);
                entries.RemoveAt(row.DataItemIndex);
                SaveData(ref mperson);
                MilestonePersons[index] = mperson;

            }
            else
            {
                try
                {
                    var milestonePerson = new MilestonePerson { Id = mpentry.MilestonePersonId };
                    ServiceCallers.Custom.MilestonePerson(serviceClient => serviceClient.DeleteMilestonePerson(milestonePerson));
                    MilestonePersons.RemoveAt(index);
                    HostingPage.Milestone = HostingPage.GetMilestoneById(HostingPage.MilestoneId);
                    HostingPage.Project = HostingPage.Milestone.Project;
                    HostingPage.FillComputedFinancials(HostingPage.Milestone);

                }
                catch (Exception exc)
                {
                    lblVacationIncludedText.Text = exc.Message;
                    lblVacationIncludedText.Visible = true;
                }
            }


            MilestonePersons = MilestonePersons;
            var entrieslist = new List<MilestonePersonEntry>();

            foreach (var mP in MilestonePersons)
            {
                entrieslist.AddRange(mP.Entries);
            }

            entrieslist = entrieslist.OrderBy(en => en.ThisPerson.LastName).ThenBy(en => en.StartDate).AsQueryable().ToList();
            BindEntriesGrid(entrieslist);
        }


        public void OnEditClick(int editRowIndex)
        {
            gvMilestonePersonEntries.EditIndex = editRowIndex;

            var entries = new List<MilestonePersonEntry>();

            foreach (var milestonePerson in MilestonePersons)
            {
                entries.AddRange(milestonePerson.Entries);
            }
            entries = entries.OrderBy(entry => entry.ThisPerson.LastName).ThenBy(ent => ent.StartDate).AsQueryable().ToList();

            BindEntriesGrid(entries);
        }

        protected void imgMilestonePersonEntryCancel_OnClick(object sender, EventArgs e)
        {
            gvMilestonePersonEntries.EditIndex = -1;

            var entries = new List<MilestonePersonEntry>();

            foreach (var milestonePerson in MilestonePersons)
            {
                entries.AddRange(milestonePerson.Entries);
            }
            entries = entries.OrderBy(entry => entry.ThisPerson.LastName).ThenBy(ent => ent.StartDate).AsQueryable().ToList();

            BindEntriesGrid(entries);

            lblResultMessage.ClearMessage();
        }


        protected void imgCopy_OnClick(object sender, EventArgs e)
        {
            ImageButton imgCopy = sender as ImageButton;
            GridViewRow row = imgCopy.NamingContainer as GridViewRow;
            var dic = new Dictionary<string, string>();

            if (gvMilestonePersonEntries.EditIndex == row.DataItemIndex)
            {
                var ddlPersonName = row.FindControl("ddlPersonName") as DropDownList;
                var ddlRole = row.FindControl("ddlRole") as DropDownList;
                var dpPersonStart = row.FindControl("dpPersonStart") as DatePicker;
                var dpPersonEnd = row.FindControl("dpPersonEnd") as DatePicker;
                var txtHoursPerDay = row.FindControl("txtHoursPerDay") as TextBox;
                var txtAmount = row.FindControl("txtAmount") as TextBox;
                var txtHoursInPeriod = row.FindControl("txtHoursInPeriod") as TextBox;


                dic.Add(DDLPERSON_KEY, ddlPersonName.SelectedValue);
                dic.Add(DDLROLE_KEY, ddlRole.SelectedValue);
                dic.Add(dpPersonStartInsert, dpPersonStart.DateValue.ToString());
                dic.Add(dpPersonEndInsert, dpPersonEnd.DateValue.ToString());
                dic.Add(txtAmountInsert, txtAmount.Text);
                dic.Add(txtHoursPerDayInsert, txtHoursPerDay.Text);
                dic.Add(txtHoursInPeriodInsert, txtHoursInPeriod.Text);


            }
            else
            {
                var lnkPersonName = row.FindControl("lnkPersonName") as HyperLink;
                var lblRole = row.FindControl("lblRole") as Label;
                var lblStartDate = row.FindControl("lblStartDate") as Label;
                var lblEndDate = row.FindControl("lblEndDate") as Label;
                var lblHoursPerDay = row.FindControl("lblHoursPerDay") as Label;
                var lblAmount = row.FindControl("lblAmount") as Label;
                var lblHoursInPeriodDay = row.FindControl("lblHoursInPeriodDay") as Label;

                var hourlyrate = lblAmount.Text.Replace("$", "");

                dic.Add(DDLPERSON_KEY, lnkPersonName.Attributes["PersonId"]);
                dic.Add(DDLROLE_KEY, lblRole.Attributes["RoleId"]);
                dic.Add(dpPersonStartInsert, lblStartDate.Text);
                dic.Add(dpPersonEndInsert, lblEndDate.Text);
                dic.Add(txtAmountInsert, hourlyrate);
                dic.Add(txtHoursPerDayInsert, lblHoursPerDay.Text);
                dic.Add(txtHoursInPeriodInsert, lblHoursInPeriodDay.Text);
            }

            AddRowAndBindRepeater(dic);

        }

        protected void gvMilestonePersonEntries_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow gvRow = gvMilestonePersonEntries.Rows[e.RowIndex];
            HiddenField hdnId = gvRow.FindControl("hdnPersonId") as HiddenField;
            var ddl = gvRow.FindControl("ddlPersonName") as DropDownList;
            Person person = GetPersonBySelectedValue(hdnId.Value);
            var milestonePerson = MilestonePersons.First(mp => mp.Person.Id.Value == person.Id.Value);

            var index = MilestonePersons.FindIndex(mp => mp.Person.Id.Value == person.Id.Value);

            var entries = new List<MilestonePersonEntry>();

            foreach (var mP in MilestonePersons)
            {
                entries.AddRange(mP.Entries);
            }
            entries = entries.OrderBy(entry => entry.ThisPerson.LastName).ThenBy(ent => ent.StartDate).AsQueryable().ToList();


            Page.Validate(vsumMilestonePersonEntry.ValidationGroup);
            if (Page.IsValid)
            {
                MilestonePersonEntry entry = entries[e.RowIndex];

                if (!UpdateMilestonePersonEntry(entry, gvMilestonePersonEntries.Rows[e.RowIndex], true))
                {
                    IsErrorOccuredWhileUpdatingRow = true;
                    HostingPage.lblResultObject.ShowErrorMessage("Error occured while saving resources.");
                    return;
                }

                IsErrorOccuredWhileUpdatingRow = false;

                hdnId.Value = ddl.SelectedValue;
                milestonePerson.Person = GetPersonBySelectedValue(ddl.SelectedValue);


                if (IsSaveCommit)
                {
                    gvMilestonePersonEntries.EditIndex = -1;
                    SaveData(ref milestonePerson, ISDatBindRows);
                }

                MilestonePersons[index] = milestonePerson;
                MilestonePersons = MilestonePersons;
                if (ISDatBindRows)
                {
                    var entrieslist = new List<MilestonePersonEntry>();

                    foreach (var mP in MilestonePersons)
                    {
                        entrieslist.AddRange(mP.Entries);
                    }

                    entrieslist = entrieslist.OrderBy(en => en.ThisPerson.LastName).ThenBy(en => en.StartDate).AsQueryable().ToList();
                    BindEntriesGrid(entrieslist);
                }

                e.Cancel = true;
            }
            else
            {
                HostingPage.lblResultObject.ShowErrorMessage("Error occured while saving resources.");
            }
        }

        private bool UpdateMilestonePersonEntry(MilestonePersonEntry entry, GridViewRow gridViewRow, bool isRowUpdating)
        {
            lblResultMessage.ClearMessage();
            var dpStartDate = gridViewRow.FindControl("dpPersonStart") as DatePicker;
            var dpEndDate = gridViewRow.FindControl("dpPersonEnd") as DatePicker;
            var ddlRole = gridViewRow.FindControl("ddlRole") as DropDownList;
            var txtHoursPerDay = gridViewRow.FindControl("txtHoursPerDay") as TextBox;
            var txtAmount = gridViewRow.FindControl("txtAmount") as TextBox;
            var txtHoursInPeriod = gridViewRow.FindControl("txtHoursInPeriod") as TextBox;
            var ddlPersonName = gridViewRow.FindControl("ddlPersonName") as DropDownList;

            if (isRowUpdating)
            {
                if (entry.StartDate < dpStartDate.DateValue
                    && CheckTimeEntriesExist(entry.MilestonePersonId, entry.StartDate, dpStartDate.DateValue, true, false)
                   )
                {
                    lblResultMessage.ShowErrorMessage("Cannot update milestone person details because there (s)he has reported hours between existing start date and currently selected start date.");
                    errorOccured = true;
                    return false;
                }
                if (entry.EndDate > dpEndDate.DateValue
                    && CheckTimeEntriesExist(entry.MilestonePersonId, dpEndDate.DateValue, entry.EndDate, false, true)
                   )
                {
                    lblResultMessage.ShowErrorMessage("Cannot update milestone person details because there (s)he has reported hours between currently selected end date and existing end date.");
                    errorOccured = true;
                    return false;
                }
            }

            entry.StartDate = dpStartDate.DateValue;
            entry.EndDate = dpEndDate.DateValue != DateTime.MinValue ? (DateTime?)dpEndDate.DateValue : null;

            if (!string.IsNullOrEmpty(ddlPersonName.SelectedValue))
            {
                var person = GetPersonBySelectedValue(ddlPersonName.SelectedValue);

                entry.ThisPerson = person;
            }

            // Role
            if (!string.IsNullOrEmpty(ddlRole.SelectedValue))
            {
                entry.Role =
                    new PersonRole
                    {
                        Id = int.Parse(ddlRole.SelectedValue),
                        Name = ddlRole.SelectedItem.Text
                    };
            }
            else
                entry.Role = null;

            // Amount
            if (!string.IsNullOrEmpty(txtAmount.Text))
            {
                entry.HourlyAmount = decimal.Parse(txtAmount.Text);
            }

            if (String.IsNullOrEmpty(txtHoursPerDay.Text) && String.IsNullOrEmpty(txtHoursInPeriod.Text))
            {
                txtHoursPerDay.Text = DEFAULT_NUMBER_HOURS_PER_DAY;
            }


            // Flags
            bool isUpdate = (entry.MilestonePersonId != 0);
            bool isHoursInPeriodChanged = !String.IsNullOrEmpty(txtHoursInPeriod.Text) &&
                                          (entry.ProjectedWorkloadWithVacation != decimal.Parse(txtHoursInPeriod.Text));

            // Check if need to recalculate Hours per day value
            // Get working days person on Milestone for current person
            DateTime endDate = entry.EndDate.HasValue ? entry.EndDate.Value : Milestone.ProjectedDeliveryDate;
            int days = GetPersonWorkDaysNumber(entry.StartDate, endDate, ddlPersonName);
            decimal hoursPerDay;

            // Update
            if (isUpdate)
            {
                if (isHoursInPeriodChanged)
                {
                    var newTotalDays = decimal.Parse(txtHoursInPeriod.Text);
                    // Recalculate hours per day according to HoursInPerod 
                    hoursPerDay = (days != 0) ? decimal.Round(newTotalDays / (days + entry.VacationDays), 2) : 0;

                    // If calculated value more then 24 hours set 24 hours as maximum value for working day
                    entry.HoursPerDay = (hoursPerDay > 24M) ? 24M : hoursPerDay;
                    // Recalculate Hours In Period
                    entry.ProjectedWorkload = entry.HoursPerDay * days;
                }
                else
                {
                    // If Hours Per Day is ommited set 8 hours as default value for working day
                    entry.HoursPerDay = !String.IsNullOrEmpty(txtHoursPerDay.Text)
                                            ? decimal.Parse(txtHoursPerDay.Text)
                                            : 8M;
                    // Recalculate Hours In Period
                    entry.ProjectedWorkload = entry.HoursPerDay * days;
                }
            }
            // Insert
            else
            {
                if (isHoursInPeriodChanged)
                {
                    // Recalculate hours per day according to HoursInPerod 
                    hoursPerDay = (days != 0) ? decimal.Round(decimal.Parse(txtHoursInPeriod.Text) / days, 2) : 0;
                    // If calculated value more then 24 hours set 24 hours as maximum value for working day
                    entry.HoursPerDay = (hoursPerDay > 24M) ? 24M : hoursPerDay;
                    // Recalculate Hours In Period
                    entry.ProjectedWorkload = entry.HoursPerDay * days;
                }
                else
                {
                    // If Hours Per Day is ommited set 8 hours as default value for working day
                    entry.HoursPerDay = !String.IsNullOrEmpty(txtHoursPerDay.Text)
                                            ? decimal.Parse(txtHoursPerDay.Text)
                                            : 8M;
                    // Recalculate Hours In Period
                    entry.ProjectedWorkload = entry.HoursPerDay * days;
                }
            }

            return true;
        }

        private bool CheckTimeEntriesExist(int MilestonePersonId, DateTime? startDate, DateTime? endDate, bool checkStartDateEquality, bool checkEndDateEquality)
        {
            using (var serviceClient = new MilestonePersonServiceClient())
            {
                try
                {
                    return serviceClient.CheckTimeEntriesForMilestonePerson(MilestonePersonId, startDate, endDate, checkStartDateEquality, checkEndDateEquality);

                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        private void SaveData(ref MilestonePerson milestonePerson, bool iSDatBindRows = true)
        {
            var person = milestonePerson.Person;
            var milestone = milestonePerson.Milestone;
            var tempPerson = new Person
            {
                Id = person.Id
            };
            var tempMilestone = new Milestone
            {
                Id = milestone.Id
            };
            milestonePerson.Milestone = tempMilestone;
            milestonePerson.Person = tempPerson;
            foreach (var entry in milestonePerson.Entries)
            {
                entry.ThisPerson = tempPerson;
            }
            using (var serviceClient = new MilestonePersonServiceClient())
            {
                try
                {
                    serviceClient.SaveMilestonePerson(ref milestonePerson, HostingPage.User.Identity.Name);
                    milestonePerson.Person = person;
                    milestonePerson.Milestone = milestone;
                    if (iSDatBindRows)
                    {
                        foreach (var entry in milestonePerson.Entries)
                        {
                            entry.ComputedFinancials = serviceClient.FinancialsGetByMilestonePersonEntry(milestonePerson.Milestone.Id.Value, milestonePerson.Person.Id.Value, entry.StartDate);
                            entry.ThisPerson = milestonePerson.Person;
                        }
                    }

                }
                catch (Exception ex)
                {
                    serviceClient.Abort();
                    ExMessage = ex.Message;
                    Page.Validate();
                }

            }

            if (iSDatBindRows)
            {
                HostingPage.Milestone = HostingPage.GetMilestoneById(HostingPage.MilestoneId);
                HostingPage.Project = HostingPage.Milestone.Project;
                HostingPage.FillComputedFinancials(HostingPage.Milestone);
            }
        }

        protected bool GetIsHourlyRate()
        {
            return Milestone.IsHourlyAmount;
        }

        private int GetPersonWorkDaysNumber(DateTime startDate, DateTime endDate, DropDownList ddlPersonName)
        {
            int days = -1;
            if (!string.IsNullOrEmpty(ddlPersonName.SelectedValue))
            {
                using (var serviceClient = new PersonServiceClient())
                {
                    try
                    {
                        days = serviceClient.GetPersonWorkDaysNumber(int.Parse(ddlPersonName.SelectedValue), startDate,
                                                                     endDate);
                    }
                    catch (FaultException<ExceptionDetail> ex)
                    {
                        _internalException = ex.Detail;
                        serviceClient.Abort();
                        Page.Validate();
                    }
                    catch (CommunicationException ex)
                    {
                        _internalException = new ExceptionDetail(ex);
                        serviceClient.Abort();
                        Page.Validate();
                    }
                }
            }
            return days;
        }

        #endregion



        internal void CopyItemAndDaabindRepeater(int barIndex)
        {
            MilestonePersonEntry mpentry = AddMilestonePersonEntries[barIndex];

            AddMilestonePersonEntries.Add(mpentry);
            Dictionary<string, string> dic = repeaterOldValues[barIndex];
            repeaterOldValues.Add(dic);
            AddMilestonePersonEntries = AddMilestonePersonEntries;
            repeaterOldValues = repeaterOldValues;
            repPerson.DataSource = AddMilestonePersonEntries;
            repPerson.DataBind();
        }

        internal bool ValidateAll()
        {
            var result = true;
            var mpersons = new List<MilestonePerson>();

            if (gvMilestonePersonEntries.EditIndex > -1)
            {
                Page.Validate(vsumMilestonePersonEntry.ValidationGroup);
                if (Page.IsValid)
                {
                    IsSaveCommit = false;
                    ISDatBindRows = false;

                    gvMilestonePersonEntries_RowUpdating(this,
                                                    new GridViewUpdateEventArgs(gvMilestonePersonEntries.EditIndex));
                    IsSaveCommit = true;
                    ISDatBindRows = true;
                    if (IsErrorOccuredWhileUpdatingRow)
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }

            if (repPerson.Items.Count > 0)
            {

                bool output = true;
                foreach (RepeaterItem mpBar in repPerson.Items)
                {
                    var bar = mpBar.FindControl(mpbar) as MilestonePersonBar;
                    output = bar.ValidateAll(mpBar, false);
                    if (!output)
                    {
                        result = false;
                    }
                }
            }
            MilestonePersons = GetMilestonePersons();
            return result;
        }

        public bool IsErrorOccuredWhileUpdatingRow { get; set; }

        internal bool SaveAll()
        {
            var result = true;
            var mpersons = new List<MilestonePerson>();

            mpersons.AddRange(MilestonePersons);

            if (gvMilestonePersonEntries.EditIndex > -1)
            {
                Page.Validate(vsumMilestonePersonEntry.ValidationGroup);
                if (Page.IsValid)
                {
                    IsSaveCommit = true;

                    ISDatBindRows = false;

                    gvMilestonePersonEntries_RowUpdating(this,
                                                    new GridViewUpdateEventArgs(gvMilestonePersonEntries.EditIndex));
                    if (IsErrorOccuredWhileUpdatingRow)
                    {
                        result = false;
                    }

                    ISDatBindRows = false;
                }
                else
                {
                    result = false;
                }
            }

            if (result)
            {
                if (repPerson.Items.Count > 0)
                {

                    bool output = true;
                    foreach (RepeaterItem mpBar in repPerson.Items)
                    {
                        var bar = mpBar.FindControl(mpbar) as MilestonePersonBar;
                        output = bar.SaveAll(mpBar, true, false);
                        if (!output)
                        {
                            result = false;
                        }
                    }
                }
            }

            if (!result)
            {
                MilestonePersons = mpersons;
            }


            return result;
        }



        internal void AddEmptyRow()
        {
            if (gvMilestonePersonEntries.Rows.Count == 0 && repPerson.Items.Count == 0)
            {
                AddRowAndBindRepeater(null);
            }
        }
    }
}

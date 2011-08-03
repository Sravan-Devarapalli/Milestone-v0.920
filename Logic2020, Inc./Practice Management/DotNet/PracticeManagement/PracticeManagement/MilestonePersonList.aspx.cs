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

namespace PraticeManagement
{
    public partial class MilestonePersonList : PracticeManagementPageBase, IPostBackEventHandler
    {
        #region Constants

        private const string MILESTONE_PERSON_ID_ARGUMENT = "milestonePersonId";
        private const int AMOUNT_COLUMN_INDEX = 4;
        private const string MILESTONE_PERSONS_KEY = "MilestonePersons";
        private const string ADD_MILESTONE_PERSON_ENTRIES_KEY = "ADD_MILESTONE_PERSON_ENTRIES_KEY";
        private const string PERSONSLISTFORMILESTONE_KEY = "PERSONSLISTFORMILESTONE_KEY";
        private const string ROLELISTFORPERSONS_KEY = "ROLELISTFORPERSONS_KEY";
        private const string DDLPERSON_KEY = "ddlPerson";
        private const string DDLROLE_KEY = "ddlRole";
        private const string DEFAULT_NUMBER_HOURS_PER_DAY = "8";
        private const string BUTTON_INSERT_ID = "btnInsert";
        private const string DuplPersonName = "The specified person is already assigned on this milestone.";
        private const string lblTargetMargin = "lblTargetMargin";

        #endregion

        #region Fields

        private ExceptionDetail _internalException;
        private Milestone _milestoneValue;
        private bool errorOccured = false;

        private SeniorityAnalyzer _seniorityAnalyzer;

        #endregion

        #region Properties

        private String ExMessage { get; set; }

        protected Milestone Milestone
        {
            get
            {
                if (_milestoneValue == null)
                {
                    using (var serviceClient = new MilestoneServiceClient())
                    {
                        try
                        {
                            _milestoneValue = serviceClient.GetMilestoneById(SelectedId.Value);
                        }
                        catch (CommunicationException)
                        {
                            serviceClient.Abort();
                            throw;
                        }
                    }
                }

                return _milestoneValue;
            }
            set { _milestoneValue = value; }
        }


        private List<MilestonePerson> MilestonePersons
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

        private List<MilestonePersonEntry> AddMilestonePersonEntries
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


        private Person[] PersonsListForMilestone
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


        private PersonRole[] RoleListForPersons
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
                int? milestoneId = SelectedId;

                if (milestoneId.HasValue)
                {
                    if (ViewState["HasPermission"] == null)
                    {
                        ViewState["HasPermission"] = DataHelper.IsUserHasPermissionOnProject(User.Identity.Name, milestoneId.Value, false);
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
                int? id = SelectedId;
                if (id.HasValue)
                {
                    if (ViewState["IsOwnerOfProject"] == null)
                    {
                        ViewState["IsOwnerOfProject"] = DataHelper.IsUserIsOwnerOfProject(User.Identity.Name, id.Value, false);
                    }
                    return (bool)ViewState["IsOwnerOfProject"];
                }

                return null;
            }
        }

        #endregion

        #region Methods

        #region Validation



        protected void custPersonStartInsert_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            var custPerson = sender as CustomValidator;
            var bar = custPerson.NamingContainer as RepeaterItem;
            var dpPersonStartInsert = bar.FindControl("dpPersonStartInsert") as DatePicker;

            args.IsValid = dpPersonStartInsert.DateValue.Date >= Milestone.StartDate.Date;
        }

        protected void custPersonEndInsert_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            var custPerson = sender as CustomValidator;

            var bar = custPerson.NamingContainer as RepeaterItem;

            var dpPersonEndInsert = bar.FindControl("dpPersonEndInsert") as DatePicker;
            var ddlPerson = bar.FindControl(DDLPERSON_KEY) as DropDownList;

            var dpPersonEnd = dpPersonEndInsert;

            Person person = GetPersonBySelectedValue(ddlPerson.SelectedValue);

            bool isGreaterThanMilestone = dpPersonEnd.DateValue <= Milestone.ProjectedDeliveryDate;

            if (!isGreaterThanMilestone)
            {
                lblMoveMilestoneDate.Text = dpPersonEnd.DateValue.ToShortDateString();

                bool terminationAndCompensation =
                    ChechTerminationAndCompensation(dpPersonEnd.DateValue, person);

                cellMoveMilestone.Visible = terminationAndCompensation;
                cellTerminationOrCompensation.Visible =
                    !terminationAndCompensation;
            }

            //pnlChangeMilestone.Visible = !isGreaterThanMilestone;
            //btnMoveMilestone.Enabled = SelectedMilestonePersonId.HasValue;
            args.IsValid = isGreaterThanMilestone;
        }

        protected void custPeriodOvberlappingInsert_ServerValidate(object sender, ServerValidateEventArgs e)
        {

            var custPerson = sender as CustomValidator;

            var bar = custPerson.NamingContainer as RepeaterItem;

            var dpPersonStartInsert = bar.FindControl("dpPersonStartInsert") as DatePicker;
            var dpPersonEndInsert = bar.FindControl("dpPersonEndInsert") as DatePicker;
            var ddlPerson = bar.FindControl(DDLPERSON_KEY) as DropDownList;

            var dpPersonStart = dpPersonStartInsert;
            var dpPersonEnd = dpPersonEndInsert;

            DateTime startDate = dpPersonStart.DateValue;
            DateTime endDate =
                dpPersonEnd.DateValue != DateTime.MinValue ? dpPersonEnd.DateValue : Milestone.ProjectedDeliveryDate;



            Person person = GetPersonBySelectedValue(ddlPerson.SelectedValue);

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


        protected void custPeriodVacationOverlappingInsert_ServerValidate(object sender, ServerValidateEventArgs e)
        {

            var custPerson = sender as CustomValidator;

            var bar = custPerson.NamingContainer as RepeaterItem;

            var custPersonStartInsert = bar.FindControl("custPersonStartInsert") as CustomValidator;
            var compPersonEndInsert = bar.FindControl("compPersonEndInsert") as CompareValidator;
            var dpPersonStartInsert = bar.FindControl("dpPersonStartInsert") as DatePicker;
            var dpPersonEndInsert = bar.FindControl("dpPersonEndInsert") as DatePicker;
            var ddlPerson = bar.FindControl(DDLPERSON_KEY) as DropDownList;

            var custPersonStartDate = custPersonStartInsert;
            var compPersonEnd = compPersonEndInsert;

            var ddlPersonName = ddlPerson;

            var isStartDateValid = ((System.Web.UI.WebControls.BaseValidator)(custPersonStartDate)).IsValid;
            var isEndDateValid = compPersonEnd.IsValid;

            if (isStartDateValid && isEndDateValid)
            {
                var dpPersonStart = dpPersonStartInsert;
                var dpPersonEnd = dpPersonEndInsert;

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


        protected void custPersonInsert_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            CustomValidator custPerson = sender as CustomValidator;

            var bar = custPerson.NamingContainer as RepeaterItem;

            var ddlPerson = bar.FindControl(DDLPERSON_KEY) as DropDownList;

            Person person = GetPersonBySelectedValue(ddlPerson.SelectedValue);

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
        }


        protected void custEntriesInsert_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            CustomValidator custPerson = sender as CustomValidator;

            var bar = custPerson.NamingContainer as RepeaterItem;

            var ddlPerson = bar.FindControl(DDLPERSON_KEY) as DropDownList;

            Person person = GetPersonBySelectedValue(ddlPerson.SelectedValue);

            List<MilestonePerson> MilestonePersonList = MilestonePersons.Where(mp => mp.Person.Id.Value == person.Id.Value).AsQueryable().ToList();

            List<MilestonePersonEntry> entries = new List<MilestonePersonEntry>();

            foreach (var item in MilestonePersonList)
            {
                entries.AddRange(item.Entries);
            }

            args.IsValid = entries.Count > 0;
        }

        protected void custDuplicatedPersonInsert_ServerValidate(object source, ServerValidateEventArgs args)
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
                lblMoveMilestoneDate.Text = dpPersonEnd.DateValue.ToShortDateString();

                bool terminationAndCompensation =
                    ChechTerminationAndCompensation(dpPersonEnd.DateValue, person);

                cellMoveMilestone.Visible = terminationAndCompensation;
                cellTerminationOrCompensation.Visible =
                    !terminationAndCompensation;
            }

            pnlChangeMilestone.Visible = !isGreaterThanMilestone;
            ////  btnMoveMilestone.Enabled = SelectedMilestonePersonId.HasValue;
            args.IsValid = isGreaterThanMilestone;
        }

        protected void custPerson_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            CustomValidator custPerson = sender as CustomValidator;
            GridViewRow gvRow = custPerson.NamingContainer as GridViewRow;
            var ddl = gvRow.FindControl("ddlPersonName") as DropDownList;
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
        }

        protected void custEntries_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            CustomValidator custPerson = sender as CustomValidator;
            GridViewRow gvRow = custPerson.NamingContainer as GridViewRow;
            var ddl = gvRow.FindControl("ddlPersonName") as DropDownList;
            Person person = GetPersonBySelectedValue(ddl.SelectedValue);

            List<MilestonePerson> MilestonePersonList = MilestonePersons.Where(mp => mp.Person.Id.Value == person.Id.Value).AsQueryable().ToList();

            List<MilestonePersonEntry> entries = new List<MilestonePersonEntry>();

            foreach (var item in MilestonePersonList)
            {
                entries.AddRange(item.Entries);
            }

            args.IsValid = entries.Count > 0;
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

        private Person GetPersonBySelectedValue(String id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                using (var serviceCLient = new PersonServiceClient())
                {
                    try
                    {
                        return serviceCLient.GetPersonById(int.Parse(id));
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

        protected void btnMoveMilestone_Click(object sender, EventArgs e)
        {
            var editIndex = gvMilestonePersonEntries.EditIndex;

            //  We're using footer row when entering the page, so it's not
            //  get counted as editable row, so it's index is Count-1
            GridViewRow gridViewRow =
                editIndex < 0 ? gvMilestonePersonEntries.FooterRow : gvMilestonePersonEntries.Rows[editIndex];
            var dpPersonEnd = gridViewRow.FindControl("dpPersonEnd") as DatePicker;

            TimeSpan shift = dpPersonEnd.DateValue.Subtract(Milestone.ProjectedDeliveryDate);

            // DataHelper.ShiftMilestoneEnd(shift.Days, SelectedMilestonePersonId.Value, _milestoneValue.Id.Value);

            ReturnToPreviousPage();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (gvMilestonePersonEntries.Rows.Count == 0 && repPerson.Items.Count == 0)
            {
                AddRowAndBindRepeater();
            }

            btnSave.Visible =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) ||
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName) ||
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName) ||
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName);// #2817: DirectorRoleName is added as per the requirement.
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateAndSave())
            {
                ClearDirty();
                lblResultMessage.ShowInfoMessage(Messages.MilestonePersonSaved);
                List<MilestonePerson> milestonePersons = GetMilestonePersons();
                MilestonePersons = milestonePersons;
                FillMilestonePersonEntries(milestonePersons);
            }
        }

        protected void dpPersonStart_SelectionChanged(object sender, EventArgs e)
        {
            IsDirty = true;
            ((Control)sender).Focus();
        }

        protected void dpPersonEnd_SelectionChanged(object sender, EventArgs e)
        {
            IsDirty = true;
            ((Control)sender).Focus();
        }



        protected void btnCancelAndReturn_OnClick(object sender, EventArgs e)
        {
            ReturnToPreviousPage();
        }

        protected override void OnInit(EventArgs e)
        {
            _seniorityAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);

            base.OnInit(e);
        }


        protected override bool ValidateAndSave()
        {
            bool result = false;

            //  If used asked to save dirty page and there's a row being edited...
            if (SaveDirty && gvMilestonePersonEntries.EditIndex >= 0)
            {
                // ...fire an event to update that row.

                gvMilestonePersonEntries_RowUpdating(this,
                                                     new GridViewUpdateEventArgs(gvMilestonePersonEntries.EditIndex));
            }

            Page.Validate(vsumMilestonePerson.ValidationGroup);
            if (Page.IsValid)
            {
                SaveData();
                result = Page.IsValid;
                if (result)
                {
                    ClearDirty();
                }
            }

            return result;

        }

        private void SaveData()
        {
            using (var serviceClient = new MilestonePersonServiceClient())
            {
                try
                {
                    serviceClient.SaveMilestonePersons(MilestonePersons.AsQueryable().ToArray(), User.Identity.Name);

                }
                catch (Exception ex)
                {

                    serviceClient.Abort();
                    ExMessage = ex.Message;
                    Page.Validate(vsumMilestonePerson.ValidationGroup);
                }

            }
        }

        private void PopulateData(MilestonePerson milestonePerson)
        {
            milestonePerson.Milestone = new Milestone();
            milestonePerson.Milestone.Id = SelectedId.Value;
        }

        protected override void Display()
        {
            lblResultMessage.ClearMessage();
            int? id = SelectedId;
            if (id.HasValue)
            {
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

                    if (milestonePerson.Id.HasValue)
                    {
                        milestonePerson.ActualActivity =
                            new List<TimeEntryRecord>(TimeEntryHelper.GetTimeEntriesMilestonePerson(milestonePerson));
                        milestonePerson.EmptyPeriodPoints =
                            DataHelper.GetDatePointsForPerson(
                                                                 milestonePerson.StartDate,
                                                                 milestonePerson.EndDate,
                                                                 milestonePerson.Person
                                );
                    }


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
                var label = e.Row.FindControl(lblTargetMargin) as Label;

                DateTime startDate = Milestone.StartDate;
                DateTime endDate = Milestone.ProjectedDeliveryDate;

                if (entry.MilestonePersonId < 0)
                {
                    if (ddlPersonName != null)
                        DataHelper.FillPersonListForMilestone(ddlPersonName, string.Empty, null, startDate,
                                                              endDate);
                    label.Text = string.Empty;
                }
                else
                {
                    if (ddlPersonName != null)
                    {
                        DataHelper.FillPersonListForMilestone(ddlPersonName, string.Empty, entry.MilestonePersonId, startDate,
                                                            endDate);

                        List<MilestonePerson> MilestonePersonList = MilestonePersons.Where(mp => mp.Id == entry.MilestonePersonId).AsQueryable().ToList();

                        bool result = false;

                        foreach (var item in MilestonePersonList)
                        {
                            if (item.ActualActivity.Count > 0)
                            {
                                result = true;
                                break;
                            }
                        }

                        ddlPersonName.Enabled = result || MilestonePersons.Where(mp => mp.Id == entry.MilestonePersonId).Count() > 1 ? false : true;
                        ddlPersonName.SelectedValue = MilestonePersonList[0].Person.Id.Value.ToString();

                    }

                    if (ddlRole != null)
                    {
                        DataHelper.FillPersonRoleList(ddlRole, string.Empty);

                        ddlRole.SelectedIndex =
                            ddlRole.Items.IndexOf(
                                                     ddlRole.Items.FindByValue(entry.Role != null
                                                                                   ? entry.Role.Id.ToString()
                                                                                   : string.Empty));

                    }

                    var rowSa = new SeniorityAnalyzer(DataHelper.CurrentPerson);
                    if (rowSa.IsOtherGreater(entry.ThisPerson))
                    {
                        if (label != null)
                            label.Text = Resources.Controls.HiddenCellText;

                        if (!(IsUserisOwnerOfProject.HasValue && IsUserisOwnerOfProject.Value))
                        {
                            if (!Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName)
                                || !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName))// #2817: DirectorRoleName is added as per the requirement.
                            {
                                e.Row.Enabled = false;
                            }
                        }
                    }
                }

            }
        }

        protected void repPerson_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            DateTime startDate = Milestone.StartDate;
            DateTime endDate = Milestone.ProjectedDeliveryDate;

            var ddlPerson = e.Item.FindControl(DDLPERSON_KEY) as DropDownList;
            var ddlRole = e.Item.FindControl(DDLROLE_KEY) as DropDownList;
            var dpPersonStartInsert = e.Item.FindControl("dpPersonStartInsert") as DatePicker;
            var dpPersonEndInsert = e.Item.FindControl("dpPersonEndInsert") as DatePicker;


            if (dpPersonStartInsert != null)
                dpPersonStartInsert.DateValue = Milestone.StartDate;

            if (dpPersonEndInsert != null)
                dpPersonEndInsert.DateValue = Milestone.ProjectedDeliveryDate;

            if (ddlPerson != null)
                DataHelper.FillPersonList(ddlPerson, string.Empty, PersonsListForMilestone, string.Empty);

            if (ddlRole != null)
                DataHelper.FillListDefault(ddlRole, string.Empty, RoleListForPersons, false);

        }

        private void FillPersonListForMilestone(ListControl control, string firstItemText, int? milestonePersonId, DateTime startDate, DateTime endDate)
        {

        }



        private List<MilestonePerson> GetMilestonePersons()
        {
            using (var serviceClient = new MilestonePersonServiceClient())
            {
                try
                {
                    return serviceClient.GetMilestonePersonsDetailsByMileStoneId(SelectedId.Value).AsQueryable().ToList();
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
            Milestone = milestone;

            if (milestone.Project != null)
            {
                pdProjectInfo.Populate(milestone.Project);
            }

            lblMilestoneName.Text = milestone.Description;
            lblMilestoneStartDate.Text = milestone.StartDate.ToShortDateString();
            lblMilestoneEndDate.Text = milestone.ProjectedDeliveryDate.ToShortDateString();

            // Security
            bool isReadOnly =
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) &&
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName) &&
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName) &&
                !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName);// #2817: DirectorRoleName is added as per the requirement.

            btnAddPerson.Visible = gvMilestonePersonEntries.Columns[0].Visible = !isReadOnly;
        }

        protected void lnkPersonName_OnClick(object sender, EventArgs e)
        {
            LinkButton lnkPersonName = sender as LinkButton;
            var milestonePersonId = lnkPersonName.Attributes["MilestonePersonId"];
            GridViewRow gvRow = lnkPersonName.NamingContainer as GridViewRow;
            int index = gvRow.DataItemIndex;


            if (IsDirty)
            {
                if (ValidateAndSave())
                {
                    if (!string.IsNullOrEmpty(milestonePersonId) && milestonePersonId != "0")
                    {
                        Redirect(GetMpeRedirectUrl(milestonePersonId));
                    }
                    else
                    {
                        List<MilestonePerson> milestonePersons = GetMilestonePersons();

                        var entries = new List<MilestonePersonEntry>();

                        foreach (var milestonePerson in milestonePersons)
                        {
                            entries.AddRange(milestonePerson.Entries);
                        }

                        entries = entries.OrderBy(entry => entry.ThisPerson.LastName).ThenBy(ent => ent.StartDate).AsQueryable().ToList();

                        if (entries.Count > index)
                        {
                            Redirect(GetMpeRedirectUrl(entries[index].MilestonePersonId));
                        }

                    }

                }
            }
            else
            {
                Redirect(GetMpeRedirectUrl(milestonePersonId));
            }



        }

        protected string GetMpeRedirectUrl(object milestonePersonId)
        {
            var mpePageUrl =
                string.Format(
                       Constants.ApplicationPages.RedirectMilestonePersonIdFormat,
                       Constants.ApplicationPages.MilestonePersonDetail,
                       SelectedId.Value,
                       milestonePersonId);

            return Generic.GetTargetUrlWithReturn(mpePageUrl, Request.Url.AbsoluteUri);
        }

        protected void btnAddPerson_Click(object sender, EventArgs e)
        {
            AddRowAndBindRepeater();
        }

        private void AddRowAndBindRepeater()
        {
            if (AddMilestonePersonEntries == null)
            {
                AddMilestonePersonEntries = new List<MilestonePersonEntry>();
            }

            AddMilestonePersonEntries.Add(new MilestonePersonEntry());

            AddMilestonePersonEntries = AddMilestonePersonEntries;
            repPerson.DataSource = AddMilestonePersonEntries;
            repPerson.DataBind();
        }

        //private void plusMakeVisible(bool isplusVisible)
        //{
        //    //if (isplusVisible)
        //    //{
        //    //    btnPlus.Visible = true;
        //    //    btnInsert.Visible = false;
        //    //    btnCancel.Visible = false;
        //    //    ddlPerson.Visible = ddlRole.Visible = dpPersonStartInsert.Visible = dpPersonEndInsert.Visible = lblAmountInsert.Visible =
        //    //    txtAmountInsert.Visible = txtHoursInPeriodInsert.Visible = txtHoursPerDayInsert.Visible = false;
        //    //}
        //    //else
        //    //{
        //    //    btnPlus.Visible = false;
        //    //    btnInsert.Visible = true;
        //    //    btnCancel.Visible = true;
        //    //    ddlPerson.Visible = ddlRole.Visible = dpPersonStartInsert.Visible = dpPersonEndInsert.Visible = lblAmountInsert.Visible =
        //    //    txtAmountInsert.Visible = txtHoursInPeriodInsert.Visible = txtHoursPerDayInsert.Visible = true;
        //    //}

        //}

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            ImageButton btnCancel = sender as ImageButton;
            var bar = btnCancel.NamingContainer as RepeaterItem;
            RemoveItemAndDaabindRepeater(bar.ItemIndex);
        }

        private void RemoveItemAndDaabindRepeater(int barIndex)
        {
            AddMilestonePersonEntries.RemoveAt(barIndex);
            AddMilestonePersonEntries = AddMilestonePersonEntries;
            repPerson.DataSource = AddMilestonePersonEntries;
            repPerson.DataBind();
        }

        private void AddAndBindRow(RepeaterItem bar)
        {
            var ddlPerson = bar.FindControl(DDLPERSON_KEY) as DropDownList;
            var ddlRole = bar.FindControl(DDLROLE_KEY) as DropDownList;
            var dpPersonStartInsert = bar.FindControl("dpPersonStartInsert") as DatePicker;
            var dpPersonEndInsert = bar.FindControl("dpPersonEndInsert") as DatePicker;
            var txtAmountInsert = bar.FindControl("txtAmountInsert") as TextBox;
            var txtHoursPerDayInsert = bar.FindControl("txtHoursPerDayInsert") as TextBox;
            var txtHoursInPeriodInsert = bar.FindControl("txtHoursInPeriodInsert") as TextBox;

            MilestonePerson milestonePerson = new MilestonePerson();
            milestonePerson.Milestone = Milestone;
            milestonePerson.Entries = new List<MilestonePersonEntry>();
            var entry = new MilestonePersonEntry();
            entry.StartDate = dpPersonStartInsert.DateValue;
            entry.EndDate = dpPersonEndInsert.DateValue != DateTime.MinValue ? (DateTime?)dpPersonEndInsert.DateValue : null;
            if (!string.IsNullOrEmpty(ddlPerson.SelectedValue))
            {
                var person = GetPersonBySelectedValue(ddlPerson.SelectedValue);

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
            if (!string.IsNullOrEmpty(txtAmountInsert.Text))
            {
                entry.HourlyAmount = decimal.Parse(txtAmountInsert.Text);
            }

            if (String.IsNullOrEmpty(txtHoursPerDayInsert.Text) && String.IsNullOrEmpty(txtHoursInPeriodInsert.Text))
            {
                txtHoursPerDayInsert.Text = DEFAULT_NUMBER_HOURS_PER_DAY;
            }


            // Flags

            bool isHoursInPeriodChanged = !String.IsNullOrEmpty(txtHoursInPeriodInsert.Text) &&
                                          (entry.ProjectedWorkloadWithVacation != decimal.Parse(txtHoursInPeriodInsert.Text));

            // Check if need to recalculate Hours per day value
            // Get working days person on Milestone for current person
            DateTime endDate = entry.EndDate.HasValue ? entry.EndDate.Value : Milestone.ProjectedDeliveryDate;
            int days = GetPersonWorkDaysNumber(entry.StartDate, endDate, ddlPerson);
            decimal hoursPerDay;

            if (isHoursInPeriodChanged)
            {
                // Recalculate hours per day according to HoursInPerod 
                hoursPerDay = (days != 0) ? decimal.Round(decimal.Parse(txtHoursInPeriodInsert.Text) / days, 2) : 0;
                // If calculated value more then 24 hours set 24 hours as maximum value for working day
                entry.HoursPerDay = (hoursPerDay > 24M) ? 24M : hoursPerDay;
                // Recalculate Hours In Period
                entry.ProjectedWorkload = entry.HoursPerDay * days;
            }
            else
            {
                // If Hours Per Day is ommited set 8 hours as default value for working day
                entry.HoursPerDay = !String.IsNullOrEmpty(txtHoursPerDayInsert.Text)
                                        ? decimal.Parse(txtHoursPerDayInsert.Text)
                                        : 8M;
                // Recalculate Hours In Period
                entry.ProjectedWorkload = entry.HoursPerDay * days;
            }

            milestonePerson.Entries.Add(entry);

            MilestonePersons.Add(milestonePerson);

            MilestonePersons = MilestonePersons;

            FillMilestonePersonEntries(MilestonePersons);

            IsDirty = true;

        }

        protected void btnInsertPerson_Click(object sender, EventArgs e)
        {
            ImageButton btnCancel = sender as ImageButton;
            var bar = btnCancel.NamingContainer as RepeaterItem;

            Page.Validate(vsumMilestonePersonEntry.ValidationGroup);
            if (Page.IsValid)
            {
                lblResultMessage.ClearMessage();
                AddAndBindRow(bar);

                pnlChangeMilestone.Visible = false;
                RemoveItemAndDaabindRepeater(bar.ItemIndex);
            }


        }


        protected void imgMilestonePersonEntryEdit_OnClick(object sender, EventArgs e)
        {
            ImageButton imgEdit = sender as ImageButton;
            GridViewRow row = imgEdit.NamingContainer as GridViewRow;
            gvMilestonePersonEntries.EditIndex = row.DataItemIndex;

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
            pnlChangeMilestone.Visible = false;
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
                    return;
                }

                hdnId.Value = ddl.SelectedValue;
                milestonePerson.Person = GetPersonBySelectedValue(ddl.SelectedValue);
                gvMilestonePersonEntries.EditIndex = -1;

                MilestonePersons[index] = milestonePerson;
                entries = entries.OrderBy(en => en.ThisPerson.LastName).ThenBy(en => en.StartDate).AsQueryable().ToList();
                BindEntriesGrid(entries);
                e.Cancel = true;

                IsDirty = true;

                pnlChangeMilestone.Visible = false;
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

        public void RaisePostBackEvent(string eventArgument)
        {
            if (ValidateAndSave())
            {
                Response.Redirect(
                    Generic.GetTargetUrlWithReturn(
                        eventArgument, Request.Url.AbsoluteUri));
            }
        }
    }
}

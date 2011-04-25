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



namespace PraticeManagement
{
    public partial class MilestonePersonDetail : PracticeManagementPageBase, IPostBackEventHandler
    {
        #region Constants

        private const string MILESTONE_PERSON_ID_ARGUMENT = "milestonePersonId";
        private const int AMOUNT_COLUMN_INDEX = 4;
        private const string MILESTONE_PERSON_KEY = "MilestonePerson";
        private const string DEFAULT_NUMBER_HOURS_PER_DAY = "8";
        private const string BUTTON_INSERT_ID = "btnInsert";
        private const string DuplPersonName = "The specified person is already assigned on this milestone.";

        #endregion

        #region Fields

        private ExceptionDetail _internalException;
        private Milestone _milestoneValue;
        private Person _selectedPersonValue;

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

        protected int? SelectedMilestonePersonId
        {
            get { return GetArgumentInt32(MILESTONE_PERSON_ID_ARGUMENT); }
        }

        private Person SelectedPerson
        {
            get
            {
                if (_selectedPersonValue == null && !string.IsNullOrEmpty(ddlPersonName.SelectedValue))
                {
                    using (var serviceCLient = new PersonServiceClient())
                    {
                        try
                        {
                            _selectedPersonValue = serviceCLient.GetPersonById(int.Parse(ddlPersonName.SelectedValue));
                        }
                        catch (CommunicationException)
                        {
                            serviceCLient.Abort();
                            throw;
                        }
                    }
                }

                return _selectedPersonValue;
            }
        }

        private MilestonePerson MilestonePerson
        {
            get
            {
                if (ViewState[MILESTONE_PERSON_KEY] == null)
                {
                    ViewState[MILESTONE_PERSON_KEY] =
                        new MilestonePerson
                            {
                                Entries = new List<MilestonePersonEntry>()
                            };
                }

                return ViewState[MILESTONE_PERSON_KEY] as MilestonePerson;
            }
            set
            {
                value.Entries.Sort();
                ViewState[MILESTONE_PERSON_KEY] = value;
            }
        }

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
            var dpPersonEnd = ((Control)source).Parent.FindControl("dpPersonEnd") as DatePicker;
            bool isGreaterThanMilestone = dpPersonEnd.DateValue <= Milestone.ProjectedDeliveryDate;

            if (!isGreaterThanMilestone)
            {
                lblMoveMilestoneDate.Text = dpPersonEnd.DateValue.ToShortDateString();

                bool terminationAndCompensation =
                    ChechTerminationAndCompensation(dpPersonEnd.DateValue);

                cellMoveMilestone.Visible = terminationAndCompensation;
                cellTerminationOrCompensation.Visible =
                    !terminationAndCompensation;
            }

            pnlChangeMilestone.Visible = !isGreaterThanMilestone;
            btnMoveMilestone.Enabled = SelectedMilestonePersonId.HasValue;
            args.IsValid = isGreaterThanMilestone;
        }

        private bool ChechTerminationAndCompensation(DateTime endDate)
        {
            // NOTE: See updates to #1333 for details on this.
            Person person = SelectedPerson;
            DateTime termDate =
                person.TerminationDate.HasValue
                    ?
                        SelectedPerson.TerminationDate.Value
                    :
                        DateTime.MaxValue;

            return
                termDate >= endDate &&
                DataHelper.IsCompensationCoversMilestone(
                                                            person, person.HireDate, endDate);
        }

        protected void custPerson_ServerValidate(object source, ServerValidateEventArgs args)
        {
            Person person = SelectedPerson;

            foreach (MilestonePersonEntry entry in MilestonePerson.Entries)
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

        protected void custEntries_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = MilestonePerson.Entries.Count > 0;
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

            // Validate overlapping with other entries.
            for (int i = 0; i < MilestonePerson.Entries.Count; i++)
            {
                if (i != gvMilestonePersonEntries.EditIndex)
                {
                    DateTime entryStartDate = MilestonePerson.Entries[i].StartDate;
                    DateTime entryEndDate =
                        MilestonePerson.Entries[i].EndDate.HasValue
                            ?
                                MilestonePerson.Entries[i].EndDate.Value
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
                int days = GetPersonWorkDaysNumber(startDate, endDate);
                if (days == 0)
                {
                    e.IsValid = false;
                }
            }
        }

        #endregion

        protected void Page_PreRender(object sender, EventArgs e)
        {
            btnSave.Visible =
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) ||
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName) ||
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName) ||
                Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName);// #2817: DirectorRoleName is added as per the requirement.
            btnDelete.Visible = btnSave.Visible && SelectedMilestonePersonId.HasValue;

            if (gvMilestonePersonEntries.FooterRow != null && gvMilestonePersonEntries.FooterRow.Cells[0].Visible)
            {
                var btnInsert =
                    gvMilestonePersonEntries.FooterRow.FindControl(BUTTON_INSERT_ID) as IButtonControl;
                btnInsert.Text = Resources.Controls.SaveLabel;
            }

            if (IsPostBack && MilestonePerson != null && MilestonePerson.Entries.Count > 0 && MilestonePerson.Person != null)
            {
                MilestonePerson.EmptyPeriodPoints =
                            DataHelper.GetDatePointsForPerson(
                                                                 MilestonePerson.StartDate,
                                                                 MilestonePerson.EndDate,
                                                                 MilestonePerson.Person
                                );
                if (MilestonePerson.Id != null)
                {
                    MilestonePerson.ActualActivity =
                                new List<TimeEntryRecord>(TimeEntryHelper.GetTimeEntriesMilestonePerson(MilestonePerson));
                }
                else
                {
                    MilestonePerson.ActualActivity = new List<TimeEntryRecord>();
                }
                mpActivity.ActivityPeriod = MilestonePerson;
                mpCumulative.ActivityPeriod = MilestonePerson;
                mpCumulativeDaily.ActivityPeriod = MilestonePerson;
                DisplayPersonRate(MilestonePerson.ComputedFinancials);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateAndSave())
            {
                BindEntriesGrid(MilestonePerson.Entries);
                ClearDirty();
                lblResultMessage.ShowInfoMessage(Messages.MilestonePersonSaved);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var milestonePerson = new MilestonePerson { Id = SelectedMilestonePersonId };

                ServiceCallers.Custom.MilestonePerson(serviceClient => serviceClient.DeleteMilestonePerson(milestonePerson));

                ReturnToPreviousPage();
            }
            catch (Exception exc)
            {
                lblVacationIncludedText.Text = exc.Message;
                lblVacationIncludedText.Visible = true;
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

        protected void gvMilestonePersonEntries_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Footer)
            {
                var entry = (MilestonePersonEntry)e.Row.DataItem;
                if (e.Row.RowType == DataControlRowType.DataRow && entry.MilestonePersonId < 0)
                {
                    e.Row.Visible = false;
                }
                else
                {
                    var ddlRole = e.Row.FindControl("ddlRole") as DropDownList;
                    if (ddlRole != null)
                    {
                        DataHelper.FillPersonRoleList(ddlRole, string.Empty);
                        if (e.Row.RowType == DataControlRowType.DataRow)
                        {
                            ddlRole.SelectedIndex =
                                ddlRole.Items.IndexOf(
                                                         ddlRole.Items.FindByValue(entry.Role != null
                                                                                       ? entry.Role.Id.ToString()
                                                                                       : string.Empty));
                        }
                    }

                    if (entry != null)
                    {
                        var rowSa = new SeniorityAnalyzer(DataHelper.CurrentPerson);
                        if (rowSa.IsOtherGreater(entry.ThisPerson))
                        {
                            //We have to hide Cells[6] which is margin based on the seniority levels 
                            // of Milestone Person and logged in user.
                            var label = e.Row.Cells[6].Controls[1] as System.Web.UI.WebControls.Label;
                            if (label != null)
                                label.Text = Resources.Controls.HiddenCellText;

                            if (!Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.PracticeManagerRoleName)
                                || !Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.DirectorRoleName))// #2817: DirectorRoleName is added as per the requirement.
                            {
                                e.Row.Enabled = false;
                            }
                        }

                    }
                }
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                {
                    e.Row.Cells[i].Visible = false;
                }
                e.Row.Cells[e.Row.Cells.Count - 1].ColumnSpan = e.Row.Cells.Count;
                e.Row.Cells[e.Row.Cells.Count - 1].HorizontalAlign = HorizontalAlign.Right;
            }
        }

        protected void gvMilestonePersonEntries_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvMilestonePersonEntries.EditIndex = -1;
            BindEntriesGrid(MilestonePerson.Entries);
            e.Cancel = true;
            lblResultMessage.ClearMessage();
            pnlChangeMilestone.Visible = false;
        }

        protected void gvMilestonePersonEntries_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvMilestonePersonEntries.EditIndex = e.NewEditIndex;
            BindEntriesGrid(MilestonePerson.Entries);
            e.Cancel = true;
            lblResultMessage.ClearMessage();
        }

        protected void gvMilestonePersonEntries_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            MilestonePerson milestonePerson = MilestonePerson;
            MilestonePersonEntry entry = milestonePerson.Entries[e.RowIndex];
            lblResultMessage.ClearMessage();
            if (CheckTimeEntriesExist(entry.MilestonePersonId, entry.StartDate, entry.EndDate, true, true))
            {
                lblResultMessage.ShowErrorMessage(string.Format(Messages.CannotModifyMilestonePerson, "delete"));
                return;
            }
            milestonePerson.Entries.RemoveAt(e.RowIndex);

            BindEntriesGrid(MilestonePerson.Entries);
            e.Cancel = true;

            MilestonePerson = milestonePerson;
            IsDirty = true;

            RefreshPersonRate();
        }

        protected void gvMilestonePersonEntries_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            Page.Validate(vsumMilestonePersonEntry.ValidationGroup);
            if (Page.IsValid)
            {
                MilestonePerson milestonePerson = MilestonePerson;

                MilestonePersonEntry entry = milestonePerson.Entries[e.RowIndex];

                if (!UpdateMilestonePersonEntry(entry, gvMilestonePersonEntries.Rows[e.RowIndex], true))
                {
                    return;
                }

                gvMilestonePersonEntries.EditIndex = -1;
                BindEntriesGrid(MilestonePerson.Entries);
                e.Cancel = true;

                MilestonePerson = milestonePerson;
                IsDirty = true;

                RefreshPersonRate();

                pnlChangeMilestone.Visible = false;
            }
        }

        protected bool AddAndBindRow()
        {
            Page.Validate(vsumMilestonePersonEntry.ValidationGroup);
            bool valid = Page.IsValid;
            if (valid)
            {
                MilestonePerson milestonePerson = MilestonePerson;
                var entry = new MilestonePersonEntry();
                UpdateMilestonePersonEntry(entry, gvMilestonePersonEntries.FooterRow, false);
                milestonePerson.Entries.Add(entry);

                BindEntriesGrid(MilestonePerson.Entries);

                MilestonePerson = milestonePerson;
                IsDirty = true;

                _seniorityAnalyzer.IsOtherGreater(SelectedPerson);

                RefreshPersonRate();
            }

            return valid;
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            if (gvMilestonePersonEntries.FooterRow.Cells[0].Visible)
            {
                AddAndBindRow();
            }
            else
            {
                for (int i = 0; i < gvMilestonePersonEntries.FooterRow.Cells.Count - 1; i++)
                {
                    gvMilestonePersonEntries.FooterRow.Cells[i].Visible = true;
                }
            }
        }

        protected void btnCancelAndReturn_OnClick(object sender, EventArgs e)
        {
            ReturnToPreviousPage();
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

        private void RefreshPersonRate()
        {
            Page.Validate(vsumMilestonePerson.ValidationGroup);
            if (Page.IsValid)
            {
                MilestonePerson rate = GetPersonRate();
                if (rate != null)
                {
                    DisplayPersonRate(rate.ComputedFinancials);
                    BindEntriesGrid(rate.Entries);
                }
                else
                {
                    DisplayPersonRate(null);
                }
            }
            else
            {
                DisplayPersonRate(null);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            _seniorityAnalyzer = new SeniorityAnalyzer(DataHelper.CurrentPerson);

            base.OnInit(e);
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
                entry.ThisPerson = new Person
                                       {
                                           Id = int.Parse(ddlPersonName.SelectedValue)
                                       };

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
            int days = GetPersonWorkDaysNumber(entry.StartDate, endDate);
            decimal hoursPerDay;

            // Update
            if (isUpdate)
            {
                if (isHoursInPeriodChanged)
                {
                    var newTotalDays = decimal.Parse(txtHoursInPeriod.Text);
                    // Recalculate hours per day according to HoursInPerod 
                    hoursPerDay = (days != 0) ? decimal.Round(newTotalDays / (days+entry.VacationDays), 2) : 0;

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

        protected override bool ValidateAndSave()
        {
            bool result = false;
            bool newEntryValid = true;
            errorOccured = false;

            //  If used asked to save dirty page and there's a row being edited...
            if (SaveDirty && gvMilestonePersonEntries.EditIndex >= 0)
            {
                // ...fire an event to update that row.

                gvMilestonePersonEntries_RowUpdating(this,
                                                     new GridViewUpdateEventArgs(gvMilestonePersonEntries.EditIndex));
            }

            Page.Validate(vsumMilestonePerson.ValidationGroup);

            if (Page.IsValid && !errorOccured)
            {
                //  If used asked to save dirty page and there's newly added row...
                if (SaveDirty && gvMilestonePersonEntries.FooterRow.Cells[0].Visible)
                {
                    // ...add it to the grid.
                    newEntryValid = AddAndBindRow();
                }

                // If new row was added successfully or there was no row, save the grid.
                if (newEntryValid)
                {
                    SaveDate();
                    result = Page.IsValid;
                }
            }

            return result && newEntryValid;
        }

        protected override void Display()
        {
            // Added filter for ovelaps MileStone time and Person active time
            DateTime startDate = Milestone.StartDate;
            DateTime endDate = Milestone.ProjectedDeliveryDate;

            DataHelper.FillPersonListForMilestone(ddlPersonName, string.Empty, SelectedMilestonePersonId, startDate,
                                                  endDate);
            lblResultMessage.ClearMessage();
            int? id = SelectedId;
            if (id.HasValue)
            {
                if (SelectedMilestonePersonId.HasValue) // Edit Milestone-Person details
                {
                    MilestonePerson milestonePerson = GetMilestonePerson();

                    if (milestonePerson != null)
                    {
                        MilestonePerson = milestonePerson;
                        ShowPreviousAndNext();
                        _seniorityAnalyzer.IsOtherGreater(milestonePerson.Person);
                        PopulateControls(milestonePerson);
                        DisplayPersonRate(milestonePerson.ComputedFinancials);

                        milestonePerson.ActualActivity =
                            new List<TimeEntryRecord>(TimeEntryHelper.GetTimeEntriesMilestonePerson(MilestonePerson));
                        milestonePerson.EmptyPeriodPoints =
                            DataHelper.GetDatePointsForPerson(
                                                                 milestonePerson.StartDate,
                                                                 milestonePerson.EndDate,
                                                                 milestonePerson.Person
                                );

                        mpActivity.ActivityPeriod = milestonePerson;
                        mpCumulativeDaily.ActivityPeriod = milestonePerson;
                        mpCumulative.ActivityPeriod = milestonePerson;

                        if (milestonePerson.ActualActivity.Count > 0)
                        {
                            ddlPersonName.Enabled = false;
                            lblTimeEntry.Visible = true;
                            lblDeleteActive.Visible = true;
                            btnDelete.Enabled = false;
                        }
                    }
                }
                else // Add new Milestone-Person details
                {
                    btnDelete.Visible = false;
                    PopulateControls(Milestone);
                    BindEntriesGrid(MilestonePerson.Entries);
                }
            }
            else
            {
                throw new InvalidOperationException(Messages.InvalidMilestoneId);
            }
        }

        private void ShowPreviousAndNext()
        {
            var mps = Milestone.MilestonePersons.Length;
            if ((mps > 1 && SelectedMilestonePersonId.HasValue) && mps != 0)
            {
                var currentLeft =
                    Array.FindIndex(
                        Milestone.MilestonePersons,
                        mp => mp.Id.Value == SelectedMilestonePersonId.Value);
                var currentRight =
                    Array.FindLastIndex(
                        Milestone.MilestonePersons,
                        mp => mp.Id.Value == SelectedMilestonePersonId.Value);

                var prev = (mps + currentLeft - 1) % mps;
                var next = (currentRight + 1) % mps;

                if (mps > 2)
                {
                    divLeft.Visible = true;
                    InitLinkButton(lnkPrev, divLeft, captionLeft, lblLeft, Milestone.MilestonePersons[prev]);
                }
                else
                {
                    divLeft.Visible = false;
                }

                InitLinkButton(lnkNext, divRight, captionRight, lblRight, Milestone.MilestonePersons[next]);
                divPrevNextMainContent.Visible = true;
            }
            else
            {
                divPrevNextMainContent.Visible = false;
            }
        }

        private void InitLinkButton(
            HyperLink hlink,
            HtmlGenericControl div,
            HtmlGenericControl span,
            HtmlGenericControl label,
            MilestonePerson milestonePerson)
        {
            span.InnerText
                = string.Format(
                    "{0}, {1}",
                    milestonePerson.Person.LastName,
                    milestonePerson.Person.FirstName);

            label.InnerText
                = string.Format(
                    "({0} - {1})",
                    milestonePerson.StartDate.ToString(Constants.Formatting.EntryDateFormat),
                    milestonePerson.EndDate.ToString(Constants.Formatting.EntryDateFormat));

            var mpId = milestonePerson.Id.Value;
            hlink.NavigateUrl = GetMilestonePersonUrllWithReturn(mpId);
            hlink.Attributes.Add("onclick", "javascript:checkDirty(\"" + mpId + "\")");
        }

        private string GetMilestonePersonUrllWithReturn(int mpId)
        {
            return Generic.GetTargetUrlWithReturn(
                string.Format(
                    Constants.ApplicationPages.RedirectMilestonePersonIdFormat,
                    Constants.ApplicationPages.MilestonePersonDetail,
                    Milestone.Id.Value,
                    mpId
                    ), Request.Url.AbsoluteUri);
        }

        private MilestonePerson GetMilestonePerson()
        {
            using (var serviceClient = new MilestonePersonServiceClient())
            {
                try
                {
                    return serviceClient.GetMilestonePersonDetail(SelectedMilestonePersonId.Value);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected string GetVacationTooltip(int val)
        {
            var tooltip = string.Format(Messages.VacationIncluded, val);

            lblVacationIncludedText.Visible = val > 0.0M;
            lblVacationIncludedText.Text = tooltip;

            return tooltip;
        }

        private void DisplayPersonRate(ComputedFinancials rate)
        {
            if (MilestonePerson.Id.HasValue)
            {
                var milestonePersonId = MilestonePerson.Id.Value;
                if (Page.IsPostBack && MilestonePerson != null && MilestonePerson.Person != null)
                {
                    // As the property _seniorityAnalyzer is initialized every post back in OnInit method, 
                    // but comparing the seniority of logged in user and milestone person and seeting value for GreaterSeniorityExists
                    // is done only in Display method. But in post back Display() method is not called and GreaterSeniorityExists is not set.

                    _seniorityAnalyzer.IsOtherGreater(MilestonePerson.Person);
                }
                financials.SeniorityAnalyzer = _seniorityAnalyzer;
                financials.Financials = ServiceCallers.Custom.MilestonePerson(c => c.CalculateMilestonePersonFinancials(milestonePersonId));
            }
        }

        private MilestonePerson GetPersonRate()
        {
            MilestonePerson rate = null;
            if (!string.IsNullOrEmpty(ddlPersonName.SelectedValue))
            {
                using (var serviceClient = new PersonServiceClient())
                {
                    try
                    {
                        MilestonePerson milestonePerson = MilestonePerson;
                        PopulateData(milestonePerson);
                        rate = serviceClient.GetPersonRate(milestonePerson);

                        foreach (var item in milestonePerson.Entries)
                        {
                            var uItem = rate.Entries.Find(mpe => (mpe.StartDate == item.StartDate && mpe.EndDate == item.EndDate));
                            if (uItem != null)
                            {
                                item.VacationDays = uItem.VacationDays;
                                item.ComputedFinancials = uItem.ComputedFinancials;
                            }
                        }

                    }
                    catch (FaultException<ExceptionDetail> ex)
                    {
                        _internalException = ex.Detail;
                        serviceClient.Abort();
                        ExMessage = ex.Message;
                        Page.Validate();
                        rate = null;
                    }
                    catch (CommunicationException)
                    {
                        serviceClient.Abort();
                        throw;
                    }
                }
            }
            return rate;
        }

        private void PopulateControls(MilestonePerson milestonePerson)
        {
            PopulateControls(milestonePerson.Milestone);

            ddlPersonName.SelectedIndex =
                ddlPersonName.Items.IndexOf(ddlPersonName.Items.FindByValue(
                                                                               milestonePerson.Person != null &&
                                                                               milestonePerson.Person.Id.HasValue
                                                                                   ?
                                                                                       milestonePerson.Person.Id.Value.
                                                                                           ToString()
                                                                                   : string.Empty));

            BindEntriesGrid(milestonePerson.Entries);
        }

        private void BindEntriesGrid(List<MilestonePersonEntry> entries)
        {
            var tmp = new List<MilestonePersonEntry>(entries);

            if (tmp.Count == 0)
            {
                tmp.Add(new MilestonePersonEntry() { MilestonePersonId = -1 });
            }
            else
            {
                tmp.Sort();
            }
            gvMilestonePersonEntries.DataSource = tmp;

            gvMilestonePersonEntries.Columns[AMOUNT_COLUMN_INDEX].Visible = Milestone.IsHourlyAmount;
            gvMilestonePersonEntries.ShowFooter = gvMilestonePersonEntries.EditIndex < 0;
            gvMilestonePersonEntries.DataBind();

            if (entries.Count == 0)
            {
                var btnNewRow = (LinkButton)gvMilestonePersonEntries.Rows[0].FindControl("btnInsert");
                btnInsert_Click(null, null);
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

            gvMilestonePersonEntries.Columns[gvMilestonePersonEntries.Columns.Count - 1].Visible = !isReadOnly;
            btnDelete.Visible = !isReadOnly;
        }

        private void SaveDate()
        {
            MilestonePerson milestonePerson = MilestonePerson;
            PopulateData(milestonePerson);
            

            using (var serviceClient = new MilestonePersonServiceClient())
            {
                try
                {
                    serviceClient.SaveMilestonePerson(ref milestonePerson, User.Identity.Name);
                    MilestonePerson = milestonePerson;
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
            var personId = int.Parse(ddlPersonName.SelectedValue);
            if (MilestonePerson.Person != null && 
                MilestonePerson.Person.Id.HasValue && 
                personId == MilestonePerson.Person.Id.Value)
            {
                milestonePerson.Person = MilestonePerson.Person;
                return;
            }
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    milestonePerson.Person = serviceClient.GetPersonDetail(personId);
                }
                catch(Exception ex)
                {
                    _internalException = new ExceptionDetail(ex);
                    serviceClient.Abort();
                    Page.Validate();
                }
            }
        }

        private int GetPersonWorkDaysNumber(DateTime startDate, DateTime endDate)
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

        protected void btnMoveMilestone_Click(object sender, EventArgs e)
        {
            var editIndex = gvMilestonePersonEntries.EditIndex;

            //  We're using footer row when entering the page, so it's not
            //  get counted as editable row, so it's index is Count-1
            GridViewRow gridViewRow =
                editIndex < 0 ? gvMilestonePersonEntries.FooterRow : gvMilestonePersonEntries.Rows[editIndex];
            var dpPersonEnd = gridViewRow.FindControl("dpPersonEnd") as DatePicker;

            TimeSpan shift = dpPersonEnd.DateValue.Subtract(Milestone.ProjectedDeliveryDate);

            DataHelper.ShiftMilestoneEnd(shift.Days, SelectedMilestonePersonId.Value, _milestoneValue.Id.Value);

            ReturnToPreviousPage();
        }

        #endregion

        public void RaisePostBackEvent(string eventArgument)
        {
            if (ValidateAndSave())
            {
                // Response.Redirect(GetMilestonePersonUrllWithReturn(Convert.ToInt32(eventArgument)));
                Response.Redirect(
                    Generic.GetTargetUrlWithReturn(
                        eventArgument, Request.Url.AbsoluteUri));
            }
        }
    }
}


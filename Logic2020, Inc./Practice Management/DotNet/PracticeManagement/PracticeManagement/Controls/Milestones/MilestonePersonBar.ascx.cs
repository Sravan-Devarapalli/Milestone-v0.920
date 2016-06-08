using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.PersonService;
using System.ServiceModel;
using PraticeManagement.MilestoneService;
using System.Web.Security;

namespace PraticeManagement.Controls.Milestones
{
    public partial class MilestonePersonBar : System.Web.UI.UserControl
    {
        private const string DuplPersonName = "The specified person is already assigned on this milestone.";
        private const string milestonePersonEntryInsert = "MilestonePersonEntryInsert";
        public const string BothDatesEmployementError = "{0} cannot be assigned to the project due to his/her hire date-{1} and termination date-{2} from the company. Please update his/her start and end dates to align with that date.";
        public const string TotalOutOfEmploymentError = "The person you are trying to add is not set as being active during the entire length of their participation in the milestone.  Please adjust the person's hire and compensation records, or change the dates that they are attached to this milestone.";
        public const string TerminationDateEmployementError = "{0} cannot be assigned to the project past {1} due to his/her termination from the company. Please update his/her start and end dates to align with that date.";
        public const string HireDateEmployementError = "{0} cannot be assigned to the project before {1} as he/she is not hired into the company. Please update his/her start and end dates to align with that date.";
        private const string BadgeInBreakPeriodError = "This resource assignment conflicts with the person’s 6mos break period of {0}-{1}.";
        private ExceptionDetail _internalException;

        private PraticeManagement.MilestoneDetail HostingPage
        {
            get { return ((PraticeManagement.MilestoneDetail)Page); }
        }

        private MilestonePersonList HostingControl
        {
            get { return HostingPage.MilestonePersonEntryListControlObject; }
        }

        public string BarColor
        {
            set
            {
                trBar.Style["background-color"] = value;
            }
        }

        public MSBadge BadgeDetails
        {
            get;
            set;
        }

        protected bool IsAdmin
        {
            get
            {
                return Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            }
        }

        protected bool IsOperations
        {
            get
            {
                return Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.OperationsRoleName);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
          custExceptionNotMoreThan18moEndDate.Enabled =  custBadgeAfterJuly.Enabled = custBadgeNotInEmpHistory.Enabled = custBadgeHasMoredays.Enabled = compBadgeEndWithPersonEnd.Enabled = compBadgeStartWithPersonStart.Enabled = reqBadgeStart.Enabled = compBadgeStartType.Enabled = custBadgeStart.Enabled = reqBadgeEnd.Enabled = compBadgeEndType.Enabled = compBadgeEnd.Enabled = custBadgeEnd.Enabled = custBlocked.Enabled = custBadgeInBreakPeriod.Enabled = chbBadgeRequiredInsert.Checked;
        }

        #region Validation

        protected void reqHourlyRevenue_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = !HostingPage.Milestone.IsHourlyAmount || !string.IsNullOrEmpty(e.Value);
        }

        protected void custPersonStartInsert_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            var result = dpPersonStartInsert.DateValue.Date >= (HostingPage.IsSaveAllClicked ? HostingPage.dtpPeriodFromObject.DateValue : HostingPage.Milestone.StartDate.Date);

            if (HostingPage.IsSaveAllClicked && HostingPage.dtpPeriodFromObject.DateValue.Date > HostingPage.Milestone.StartDate.Date)
            {
                result = true;
            }

            args.IsValid = result;
        }

        protected void custPersonEndInsert_ServerValidate(object sender, ServerValidateEventArgs args)
        {

            var dpPersonEnd = dpPersonEndInsert;

            var bar = btnInsert.NamingContainer.NamingContainer as RepeaterItem;

            Person person = HostingControl.GetPersonBySelectedValue(ddlPerson.SelectedValue);

            bool isGreaterThanMilestone = dpPersonEnd.DateValue <= (HostingPage.IsSaveAllClicked ? HostingPage.dtpPeriodToObject.DateValue : HostingPage.Milestone.ProjectedDeliveryDate);

            if (HostingPage.IsSaveAllClicked && HostingPage.dtpPeriodToObject.DateValue.Date < HostingPage.Milestone.EndDate.Date)
            {
                isGreaterThanMilestone = true;
            }

            args.IsValid = isGreaterThanMilestone;
        }

        protected void cvMaxRows_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (ddlPerson.SelectedItem.Attributes[Constants.Variables.IsStrawMan].ToLowerInvariant() == "true")
            {
                e.IsValid = true;
            }
            else
            {

                if (HostingPage.IsSaveAllClicked)
                {
                    string ddlPersonSelectedValue = ddlPerson.SelectedValue, ddlRoleSelectedValue = ddlRole.SelectedValue;
                    var count = 0;

                    for (int i = 0; i < HostingControl.MilestonePersonsEntries.Count(); i++)
                    {
                        var entry = HostingControl.MilestonePersonsEntries[i];
                        if (entry.IsShowPlusButton)
                        {
                            string personId = "", roleId = "";
                            if (entry.IsEditMode)
                            {
                                personId = (HostingControl.gvMilestonePersonEntriesObject.Rows[i].FindControl("ddlPersonName") as DropDownList).SelectedValue;
                                roleId = (HostingControl.gvMilestonePersonEntriesObject.Rows[i].FindControl("ddlRole") as DropDownList).SelectedValue;

                            }
                            else
                            {
                                personId = entry.ThisPerson.Id.ToString();
                                roleId = entry.Role != null ? entry.Role.Id.ToString() : string.Empty;
                            }

                            if (personId == ddlPersonSelectedValue && roleId == ddlRoleSelectedValue)
                            {
                                count = count + entry.ExtendedResourcesRowCount;
                            }

                        }
                    }

                    var newEntriesCount = HostingControl.repeaterOldValues.Where(entry => entry["ddlPerson"].ToLowerInvariant() == ddlPersonSelectedValue.ToLowerInvariant() && entry["ddlRole"].ToLowerInvariant() == ddlRoleSelectedValue.ToLowerInvariant()).Count();

                    count = count + newEntriesCount;

                    if (count > 5)
                    {
                        e.IsValid = false;
                    }

                }
                else
                {
                    var personId = ddlPerson.SelectedValue;
                    var roleId = ddlRole.SelectedValue;
                    List<MilestonePersonEntry> entries = HostingControl.MilestonePersonsEntries;
                    var rowsCount = entries.Where(mpe => mpe.IsNewEntry == false && mpe.ThisPerson.Id.Value.ToString() == personId && (mpe.Role != null ? mpe.Role.Id.ToString() : string.Empty) == roleId).Count();
                    if (rowsCount > 4)
                    {
                        e.IsValid = false;
                    }
                }
            }
        }

        protected void custPeriodOvberlappingInsert_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (ddlPerson.SelectedItem.Attributes[Constants.Variables.IsStrawMan].ToLowerInvariant() == "true")
            {
                e.IsValid = true;
            }
            else
            {
                if (HostingPage.IsSaveAllClicked)
                {
                    ValidateOvelappingWhenSaveAllClicked(sender, e);
                }
                else
                {
                    ValidateOvelappingWhenSaveClicked(sender, e);
                }
            }
        }

        private void ValidateOvelappingWhenSaveClicked(object sender, ServerValidateEventArgs e)
        {
            DateTime startDate = dpPersonStartInsert.DateValue;
            DateTime endDate =
                dpPersonEndInsert.DateValue != DateTime.MinValue ? dpPersonEndInsert.DateValue : HostingPage.Milestone.ProjectedDeliveryDate;

            List<MilestonePersonEntry> entries = HostingControl.MilestonePersonsEntries;
            // Validate overlapping with other entries.
            for (int i = 0; i < entries.Count; i++)
            {
                var personId = entries[i].ThisPerson.Id.ToString();
                if (personId == ddlPerson.SelectedValue && entries[i].IsNewEntry == false)
                {

                    try
                    {
                        DateTime entryStartDate = entries[i].StartDate;

                        DateTime entryEndDate = entries[i].EndDate.HasValue ? entries[i].EndDate.Value : HostingPage.Milestone.ProjectedDeliveryDate;


                        if ((startDate >= entryStartDate && startDate <= entryEndDate) ||
                               (endDate >= entryStartDate && endDate <= entryEndDate) ||
                               (endDate >= entryEndDate && startDate <= entryEndDate)
                           )
                        {
                            e.IsValid = false;
                            break;

                        }
                    }
                    catch
                    {

                    }
                }
            }
            if (e.IsValid)
            {
                int personId;
                int.TryParse(ddlPerson.SelectedValue, out personId);
                e.IsValid = !(ServiceCallers.Custom.Person(p => p.CheckIfPersonEntriesOverlapps(HostingControl.Milestone.Id.Value, personId, dpPersonStartInsert.DateValue, dpPersonEndInsert.DateValue)));
            }
        }

        private void ValidateOvelappingWhenSaveAllClicked(object sender, ServerValidateEventArgs e)
        {
            DateTime startDate = dpPersonStartInsert.DateValue;
            DateTime endDate =
                dpPersonEndInsert.DateValue != DateTime.MinValue ? dpPersonEndInsert.DateValue : HostingPage.Milestone.ProjectedDeliveryDate;



            List<MilestonePersonEntry> entries = HostingControl.MilestonePersonsEntries;
            // Validate overlapping with other entries.
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                string personId = "";

                if (entry.IsRepeaterEntry)
                {
                    personId = entries[i].ThisPerson.Id.ToString();
                }
                else if (entry.IsShowPlusButton)
                {
                    personId = entries[i].IsEditMode ? entries[i].EditedEntryValues["ddlPersonName"] : entries[i].ThisPerson.Id.ToString();
                }
                else
                {
                    var index = entries.FindIndex(mpe => mpe.Id == entries[i].ShowingPlusButtonEntryId);

                    personId = entries[index].IsEditMode ? entries[index].EditedEntryValues["ddlPersonName"] : entries[index].ThisPerson.Id.ToString();
                }


                if (personId == ddlPerson.SelectedValue && ((entries[i].IsNewEntry == false) || HostingPage.ValidateNewEntry))
                {

                    try
                    {
                        DateTime entryStartDate =
                                            entries[i].IsEditMode
                                            ? Convert.ToDateTime(entries[i].EditedEntryValues["dpPersonStart"])
                                            : entries[i].StartDate;

                        DateTime entryEndDate =
                                            entries[i].IsEditMode ?
                                            Convert.ToDateTime(entries[i].EditedEntryValues["dpPersonEnd"])
                                            : entries[i].EndDate.HasValue ? entries[i].EndDate.Value : HostingPage.Milestone.ProjectedDeliveryDate;


                        if ((startDate >= entryStartDate && startDate <= entryEndDate) ||
                               (endDate >= entryStartDate && endDate <= entryEndDate) ||
                               (endDate >= entryEndDate && startDate <= entryEndDate)
                           )
                        {
                            e.IsValid = false;
                            break;

                        }
                    }
                    catch
                    {

                    }
                }
            }
            if (e.IsValid)
            {
                int personId;
                int.TryParse(ddlPerson.SelectedValue, out personId);
                e.IsValid = !(ServiceCallers.Custom.Person(p => p.CheckIfPersonEntriesOverlapps(HostingControl.Milestone.Id.Value, personId, dpPersonStartInsert.DateValue, dpPersonEndInsert.DateValue)));
            }
        }

        protected void custPeriodVacationOverlappingInsert_ServerValidate(object sender, ServerValidateEventArgs e)
        {
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
                    dpPersonEnd.DateValue != DateTime.MinValue ? dpPersonEnd.DateValue : HostingPage.Milestone.ProjectedDeliveryDate;

                // Validate overlapping with other entries.
                PersonWorkingHoursDetailsWithinThePeriod personWorkingHoursDetailsWithinThePeriod = GetPersonWorkingHoursDetailsWithinThePeriod(startDate, endDate, ddlPersonName);
                if (personWorkingHoursDetailsWithinThePeriod.TotalWorkHoursExcludingVacationHours == 0)
                {
                    e.IsValid = false;
                }
            }
        }

        protected void custPersonInsert_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            CustomValidator custPerson = sender as CustomValidator;
            Person person = HostingControl.GetPersonBySelectedValue(ddlPerson.SelectedValue);
            Dictionary<string, List<DateTime>> PersonDatesViolations = HostingControl.IsRangeInThePersonEmpHistory(person, dpPersonStartInsert.DateValue.Date, dpPersonEndInsert.DateValue.Date);
            args.IsValid = !(PersonDatesViolations.Count > 0);
            if (PersonDatesViolations != null && PersonDatesViolations.Keys.Any())
            {
                string firstKey = PersonDatesViolations.Keys.First();
                switch (firstKey)
                {
                    case "HireDate": custPerson.ToolTip = string.Format(HireDateEmployementError, person.LastName + ", " + person.FirstName, PersonDatesViolations[firstKey][0].ToString(Constants.Formatting.EntryDateFormat)); break;
                    case "TerminationDate": custPerson.ToolTip = string.Format(TerminationDateEmployementError, person.LastName + ", " + person.FirstName, PersonDatesViolations[firstKey][0].ToString(Constants.Formatting.EntryDateFormat)); break;
                    case "Both": custPerson.ToolTip = string.Format(BothDatesEmployementError, person.LastName + ", " + person.FirstName, PersonDatesViolations[firstKey][0].ToString(Constants.Formatting.EntryDateFormat), PersonDatesViolations[firstKey][1].ToString(Constants.Formatting.EntryDateFormat)); break;
                    case "TotalOut": custPerson.ToolTip = string.Format(TotalOutOfEmploymentError); break;
                }
            }
            custPerson.ErrorMessage = custPerson.ToolTip;
        }

        protected void cvHoursInPeriod_ServerValidate(object source, ServerValidateEventArgs e)
        {
            if (reqPersonStart.IsValid && reqPersonEnd.IsValid)
            {
                var txtHoursInPeriod = txtHoursInPeriodInsert;
                var value = txtHoursInPeriod.Text.Trim();

                if (!string.IsNullOrEmpty(value))
                {
                    decimal Totalhours;
                    if (decimal.TryParse(value, out Totalhours) && Totalhours > 0M)
                    {
                        var dpPersonStart = ((Control)source).Parent.FindControl("dpPersonStart") as DatePicker;
                        var dpPersonEnd = ((Control)source).Parent.FindControl("dpPersonEnd") as DatePicker;
                        PersonWorkingHoursDetailsWithinThePeriod personWorkingHoursDetailsWithinThePeriod = GetPersonWorkingHoursDetailsWithinThePeriod(dpPersonStartInsert.DateValue, dpPersonEndInsert.DateValue, ddlPerson);

                        // calculate hours per day according to HoursInPerod 
                        var hoursPerDay = (personWorkingHoursDetailsWithinThePeriod.TotalWorkDaysIncludingVacationDays != 0) ? decimal.Round(Totalhours / (personWorkingHoursDetailsWithinThePeriod.TotalWorkDaysIncludingVacationDays), 2) : 0;

                        e.IsValid = hoursPerDay > 0M;
                    }
                }
            }
        }

        protected void custBlocked_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            var ddlPersonName = ((Control)source).Parent.FindControl("ddlPerson") as CustomDropDown;
            var dpBadgeStart = ((Control)source).Parent.FindControl("dpBadgeStartInsert") as DatePicker;
            var dpBadgeEnd = ((Control)source).Parent.FindControl("dpBadgeEndInsert") as DatePicker;
            var personId = Convert.ToInt32(ddlPersonName.SelectedValue);
            var details = ServiceCallers.Custom.Person(p => p.GetBadgeDetailsByPersonId(personId)).ToList();
            BadgeDetails = details.Count > 0 ? details[0] : null;
            if (BadgeDetails != null)
                args.IsValid = !(BadgeDetails.IsBlocked && dpBadgeStart.DateValue <= BadgeDetails.BlockEndDate && BadgeDetails.BlockStartDate <= dpBadgeEnd.DateValue);
        }

        protected void custBadgeStart_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var dpBadgeStart = ((Control)source).Parent.FindControl("dpBadgeStartInsert") as DatePicker;

            var result = dpBadgeStart.DateValue.Date >= (HostingPage.IsSaveAllClicked ? HostingPage.dtpPeriodFromObject.DateValue : HostingPage.Milestone.StartDate.Date);

            if (HostingPage.IsSaveAllClicked && HostingPage.dtpPeriodFromObject.DateValue.Date > HostingPage.Milestone.StartDate.Date)
            {
                result = true;
            }

            args.IsValid = result;
        }

        protected void custAftertheBreak_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            var dpBadgeStart = ((Control)sender).Parent.FindControl("dpBadgeStartInsert") as DatePicker;
            var personId = Convert.ToInt32(ddlPerson.SelectedValue);
            if (!reqBadgeStart.IsValid || !compBadgeStartType.IsValid || !custBadgeStart.IsValid || !compBadgeStartWithPersonStart.IsValid || !reqBadgeEnd.IsValid)
                return;
            var isInBreakPereiod = ServiceCallers.Custom.Person(p => p.CheckIfDatesInDeactivationHistory(personId, dpBadgeStartInsert.DateValue, dpBadgeEndInsert.DateValue));
            args.IsValid = !(isInBreakPereiod.Any(i => i.Equals(true)));
        }

        protected void custBadgeEnd_ServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator custPerson = source as CustomValidator;
            GridViewRow gvRow = custPerson.NamingContainer as GridViewRow;
            var dpBadgeEnd = ((Control)source).Parent.FindControl("dpBadgeEndInsert") as DatePicker;
            bool isGreaterThanMilestone = dpBadgeEnd.DateValue <= (HostingPage.IsSaveAllClicked ? HostingPage.dtpPeriodToObject.DateValue : HostingPage.Milestone.ProjectedDeliveryDate);

            if (HostingPage.IsSaveAllClicked && HostingPage.dtpPeriodToObject.DateValue.Date < HostingPage.Milestone.EndDate.Date)
            {
                isGreaterThanMilestone = true;
            }

            args.IsValid = isGreaterThanMilestone;
        }

        //protected void dpPersonStart_SelectionChanged(object sender, EventArgs e)
        //{
        //    if (!chbBadgeRequiredInsert.Checked)
        //        return;
        //    dpBadgeStartInsert.TextValue = dpPersonStartInsert.TextValue;
        //    dpBadgeEndInsert.TextValue = dpPersonEndInsert.TextValue;
        //}

        protected void chbBadgeRequired_CheckedChanged(object sender, EventArgs e)
        {
            if (chbBadgeRequiredInsert.Checked)
            {
                dpBadgeEndInsert.EnabledTextBox = dpBadgeStartInsert.EnabledTextBox = chbBadgeExceptionInsert.Enabled =
                custExceptionNotMoreThan18moEndDate.Enabled = custBadgeAfterJuly.Enabled = custBadgeNotInEmpHistory.Enabled = custBadgeHasMoredays.Enabled = compBadgeEndWithPersonEnd.Enabled = compBadgeStartWithPersonStart.Enabled = reqBadgeStart.Enabled = compBadgeStartType.Enabled = custBadgeStart.Enabled = reqBadgeEnd.Enabled = compBadgeEndType.Enabled = compBadgeEnd.Enabled = custBadgeEnd.Enabled = custBlocked.Enabled = custBadgeInBreakPeriod.Enabled = true;
                dpBadgeStartInsert.ReadOnly = dpBadgeEndInsert.ReadOnly = false;
                chbOpsApprovedInsert.Enabled = IsAdmin || IsOperations;
                Person person = HostingControl.GetPersonBySelectedValue(ddlPerson.SelectedValue);
                if (person != null && !person.IsStrawMan && person.EmploymentHistory != null)
                {
                    //if (person.EmploymentHistory.Any(p => p.HireDate <= HostingControl.Milestone.ProjectedDeliveryDate && (!p.TerminationDate.HasValue || (p.TerminationDate.HasValue && HostingControl.Milestone.StartDate <= p.TerminationDate))))
                    //{
                    //    Employment employment = person.EmploymentHistory.First(p => p.HireDate <= HostingControl.Milestone.ProjectedDeliveryDate && (!p.TerminationDate.HasValue || (p.TerminationDate.HasValue && HostingControl.Milestone.StartDate <= p.TerminationDate)));
                    //    dpBadgeStartInsert.TextValue = HostingControl.Milestone.StartDate < employment.HireDate.Date ? employment.HireDate.Date.ToString(Constants.Formatting.EntryDateFormat) : HostingControl.Milestone.StartDate.ToString(Constants.Formatting.EntryDateFormat);
                    //    dpBadgeEndInsert.TextValue = employment.TerminationDate.HasValue ? HostingControl.Milestone.ProjectedDeliveryDate < employment.TerminationDate.Value ? HostingControl.Milestone.ProjectedDeliveryDate.ToString(Constants.Formatting.EntryDateFormat) : employment.TerminationDate.Value.ToString(Constants.Formatting.EntryDateFormat) : HostingControl.Milestone.ProjectedDeliveryDate.ToString(Constants.Formatting.EntryDateFormat);
                    //}
                    dpBadgeStartInsert.TextValue = dpPersonStartInsert.TextValue;
                    dpBadgeEndInsert.TextValue = dpPersonEndInsert.TextValue;
                }
                if (person != null && person.IsStrawMan)
                {
                    dpBadgeStartInsert.TextValue = HostingControl.Milestone.StartDate.ToString(Constants.Formatting.EntryDateFormat);
                    dpBadgeEndInsert.TextValue = HostingControl.Milestone.EndDate.ToString(Constants.Formatting.EntryDateFormat);
                }
            }
            else
            {
                dpBadgeEndInsert.EnabledTextBox = dpBadgeStartInsert.EnabledTextBox = chbBadgeExceptionInsert.Enabled = chbOpsApprovedInsert.Enabled =
                custExceptionNotMoreThan18moEndDate.Enabled = custBadgeAfterJuly.Enabled = custBadgeNotInEmpHistory.Enabled = custBadgeHasMoredays.Enabled = compBadgeEndWithPersonEnd.Enabled = compBadgeStartWithPersonStart.Enabled = reqBadgeStart.Enabled = compBadgeStartType.Enabled = custBadgeStart.Enabled = reqBadgeEnd.Enabled = compBadgeEndType.Enabled = compBadgeEnd.Enabled = custBadgeEnd.Enabled = custBlocked.Enabled = custBadgeInBreakPeriod.Enabled = false;
                dpBadgeStartInsert.ReadOnly = dpBadgeEndInsert.ReadOnly = true;
                dpBadgeStartInsert.TextValue = dpBadgeEndInsert.TextValue = string.Empty;
                chbOpsApprovedInsert.Checked = chbBadgeExceptionInsert.Checked = false;
            }
            chbOpsApprovedInsert.Enabled = false;
        }

        protected void custBadgeInBreakPeriod_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var validator = source as CustomValidator;
            args.IsValid = true;
            if (BadgeDetails != null && BadgeDetails.BreakStartDate.HasValue && !chbBadgeExceptionInsert.Checked)
            {
                args.IsValid = !(dpBadgeStartInsert.DateValue <= BadgeDetails.BreakEndDate && BadgeDetails.BreakStartDate <= dpBadgeEndInsert.DateValue);
                validator.ErrorMessage = validator.ToolTip = string.Format(BadgeInBreakPeriodError, BadgeDetails.BreakStartDate.Value.ToShortDateString(), BadgeDetails.BreakEndDate.Value.ToShortDateString());
            }
        }

        protected void custBadgeNotInEmpHistory_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            var custPerson = sender as CustomValidator;
            Person person = HostingControl.GetPersonBySelectedValue(ddlPerson.SelectedValue);
            Dictionary<string, List<DateTime>> PersonDatesViolations = HostingControl.IsRangeInThePersonEmpHistory(person, dpBadgeStartInsert.DateValue.Date, dpBadgeEndInsert.DateValue.Date);
            args.IsValid = !(PersonDatesViolations.Count > 0);
        }

        protected void custBadgeAfterJuly_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = (dpBadgeStartInsert.DateValue >= new DateTime(2014, 7, 1) && dpBadgeEndInsert.DateValue >= new DateTime(2014, 7, 1));
        }

        protected void custBadgeHasMoredays_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!chbBadgeRequiredInsert.Checked || !reqBadgeStart.IsValid || !reqBadgeEnd.IsValid || !compBadgeStartType.IsValid || !compBadgeEndType.IsValid)
                return;
            var date1 = dpBadgeStartInsert.DateValue;
            var date2 = dpBadgeEndInsert.DateValue;
            args.IsValid = !((((date2.Year - date1.Year) * 12) + date2.Month - date1.Month) >= 18 && !chbBadgeExceptionInsert.Checked);
        }

        protected void custExceptionNotMoreThan18moEndDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!chbBadgeExceptionInsert.Checked)
                return;
            //var chbBadgeRequired = ((Control)source).Parent.FindControl("chbBadgeRequired") as CheckBox;
            //var reqBadgeStart = ((Control)source).Parent.FindControl("reqBadgeStart") as RequiredFieldValidator;
            //var compBadgeStartType = ((Control)source).Parent.FindControl("compBadgeStartType") as CompareValidator;
            //var reqBadgeEnd = ((Control)source).Parent.FindControl("reqBadgeEnd") as RequiredFieldValidator;
            //var compBadgeEndType = ((Control)source).Parent.FindControl("compBadgeEndType") as CompareValidator;
            if (!chbBadgeRequiredInsert.Checked || !reqBadgeStart.IsValid || !reqBadgeEnd.IsValid || !compBadgeStartType.IsValid || !compBadgeEndType.IsValid)
                return;
            var lblConsultantsEnd = ((Control)source).Parent.FindControl("lblConsultantsEnd") as Label;
            var endDatevalue = lblConsultantsEnd.Attributes["DateValue"];
           // var dpBadgeEnd = ((Control)source).Parent.FindControl("dpBadgeEnd") as DatePicker;
            var badgeEndDate = Convert.ToDateTime(endDatevalue);
            args.IsValid = !(badgeEndDate.Date >= dpBadgeEndInsert.DateValue.Date);
            if (!args.IsValid)
                HostingControl.ShowExceptionPopup = true;
        }

        #endregion

        private PersonWorkingHoursDetailsWithinThePeriod GetPersonWorkingHoursDetailsWithinThePeriod(DateTime startDate, DateTime endDate, DropDownList ddlPersonName)
        {

            if (!string.IsNullOrEmpty(ddlPersonName.SelectedValue))
            {
                using (var serviceClient = new PersonServiceClient())
                {
                    try
                    {
                        return serviceClient.GetPersonWorkingHoursDetailsWithinThePeriod(int.Parse(ddlPersonName.SelectedValue), startDate,
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
            return null;
        }

        protected void btnInsertPerson_Click(object sender, EventArgs e)
        {
            var bar = btnInsert.NamingContainer.NamingContainer as RepeaterItem;
            if (Page.IsValid)
            {
                var result = InsertPerson(bar);
            }
            //if (!result)
            //{
            //    HostingPage.lblResultObject.ShowErrorMessage("Error occured while saving resources.");
            //}
        }

        private bool InsertPerson(RepeaterItem bar, bool isSaveCommit = true, bool iSDatBindRows = true, bool isValidating = true)
        {
            HostingControl.vsumMileStonePersonsObject.ValidationGroup = milestonePersonEntryInsert + bar.ItemIndex.ToString();

            bool result=false;
            if (isValidating && ValidateAndSave())
            {
                Page.Validate(HostingControl.vsumMileStonePersonsObject.ValidationGroup);
                result = Page.IsValid;
            }

            if (result)
            {
                HostingControl.lblResultMessageObject.ClearMessage();
                HostingControl.AddAndBindRow(bar, isSaveCommit, iSDatBindRows);

                if (isSaveCommit && iSDatBindRows)
                    HostingControl.RemoveItemAndDaabindRepeater(bar.ItemIndex);
                HostingControl.tblValidationObject.Visible = false;
            }
            else
            {
                HostingControl.tblValidationObject.Visible = true;
                if (HostingControl.ShowExceptionPopup)
                    HostingControl.mpeException.Show();
            }

            return result;

        }

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            var bar = btnCancel.NamingContainer.NamingContainer as RepeaterItem;
            HostingControl.tblValidationObject.Visible = false;
            HostingControl.RemoveItemAndDaabindRepeater(bar.ItemIndex);
        }

        protected void imgCopy_OnClick(object sender, EventArgs e)
        {
            var bar = imgCopy.NamingContainer.NamingContainer as RepeaterItem;
            HostingControl.CopyItemAndDaabindRepeater(bar.ItemIndex);
        }

        public void ddlPersonName_Changed(object sender, EventArgs e)
        {
            DropDownList ddlPerson = sender as DropDownList;
            GridViewRow gvRow = ddlPerson.NamingContainer as GridViewRow;
            var dpPersonStart = ((Control)sender).Parent.FindControl("dpPersonStartInsert") as DatePicker;
            var dpPersonEnd = ((Control)sender).Parent.FindControl("dpPersonEndInsert") as DatePicker;
            Person person = HostingControl.GetPersonBySelectedValue(ddlPerson.SelectedValue);
            if (person != null && !person.IsStrawMan && person.EmploymentHistory != null)
            {
                if (person.EmploymentHistory.Any(p => p.HireDate <= HostingControl.Milestone.ProjectedDeliveryDate && (!p.TerminationDate.HasValue || (p.TerminationDate.HasValue && HostingControl.Milestone.StartDate <= p.TerminationDate))))
                {
                    Employment employment = person.EmploymentHistory.First(p => p.HireDate <= HostingControl.Milestone.ProjectedDeliveryDate && (!p.TerminationDate.HasValue || (p.TerminationDate.HasValue && HostingControl.Milestone.StartDate <= p.TerminationDate)));
                    if (chbBadgeRequiredInsert.Checked)
                    {
                        dpBadgeStartInsert.TextValue = dpPersonStart.TextValue = HostingControl.Milestone.StartDate < employment.HireDate.Date ? employment.HireDate.Date.ToString(Constants.Formatting.EntryDateFormat) : HostingControl.Milestone.StartDate.ToString(Constants.Formatting.EntryDateFormat);
                        dpBadgeEndInsert.TextValue = dpPersonEnd.TextValue = employment.TerminationDate.HasValue ? HostingControl.Milestone.ProjectedDeliveryDate < employment.TerminationDate.Value ? HostingControl.Milestone.ProjectedDeliveryDate.ToString(Constants.Formatting.EntryDateFormat) : employment.TerminationDate.Value.ToString(Constants.Formatting.EntryDateFormat) : HostingControl.Milestone.ProjectedDeliveryDate.ToString(Constants.Formatting.EntryDateFormat);
                    }
                    else
                    {
                        dpPersonStart.TextValue = HostingControl.Milestone.StartDate < employment.HireDate.Date ? employment.HireDate.Date.ToString(Constants.Formatting.EntryDateFormat) : HostingControl.Milestone.StartDate.ToString(Constants.Formatting.EntryDateFormat);
                        dpPersonEnd.TextValue = employment.TerminationDate.HasValue ? HostingControl.Milestone.ProjectedDeliveryDate < employment.TerminationDate.Value ? HostingControl.Milestone.ProjectedDeliveryDate.ToString(Constants.Formatting.EntryDateFormat) : employment.TerminationDate.Value.ToString(Constants.Formatting.EntryDateFormat) : HostingControl.Milestone.ProjectedDeliveryDate.ToString(Constants.Formatting.EntryDateFormat);
                    }
                }
                var badgeDetails = ServiceCallers.Custom.Person(p => p.GetBadgeDetailsByPersonId(Convert.ToInt32(ddlPerson.SelectedValue))).ToList();
                if (badgeDetails != null && badgeDetails.Count > 0 && badgeDetails[0].BadgeEndDate.HasValue)
                {
                    lblConsultantsEnd.Text = badgeDetails[0].BadgeEndDate.Value.ToString("MM/dd/yyyy");
                    lblConsultantsEnd.Attributes["DateValue"] = lblConsultantsEnd.Text;
                }
                else
                {
                    lblConsultantsEnd.Text = string.Empty;
                    lblConsultantsEnd.Attributes["DateValue"] = DateTime.MinValue.ToShortDateString();
                }
            }
            if (person != null && person.IsStrawMan)
            {
               dpPersonStart.TextValue = HostingControl.Milestone.StartDate.ToString(Constants.Formatting.EntryDateFormat);
               dpPersonEnd.TextValue = HostingControl.Milestone.EndDate.ToString(Constants.Formatting.EntryDateFormat);
               dpBadgeEndInsert.EnabledTextBox = dpBadgeStartInsert.EnabledTextBox = chbBadgeExceptionInsert.Enabled = chbOpsApprovedInsert.Enabled = chbBadgeRequiredInsert.Enabled = false;
               dpBadgeStartInsert.ReadOnly = dpBadgeEndInsert.ReadOnly = true;
               dpBadgeStartInsert.TextValue = dpBadgeEndInsert.TextValue = string.Empty;
            }
        }

        protected string GetValidationGroup()
        {
            var bar = btnInsert.NamingContainer.NamingContainer as RepeaterItem;
            return milestonePersonEntryInsert + bar.ItemIndex.ToString();
        }

        internal bool ValidateAll(RepeaterItem mpBar, bool isSaveCommit)
        {
            return InsertPerson(mpBar, isSaveCommit, false);
        }

        internal bool SaveAll(RepeaterItem mpBar, bool isSaveCommit, bool iSDatBindRows)
        {
            return InsertPerson(mpBar, isSaveCommit, iSDatBindRows, false);
        }

        private bool ValidateAndSave()
        {
            reqPersonStart.Validate();
            reqPersonEnd.Validate();
            compPersonStartType.Validate();
            compPersonEndType.Validate();
            reqHourlyRevenue.Validate();
            if (reqPersonStart.IsValid && reqPersonEnd.IsValid)
            {
                compPersonEndInsert.Validate();
            }
            return (reqPersonStart.IsValid && reqPersonEnd.IsValid && compPersonStartType.IsValid && compPersonEndType.IsValid && reqHourlyRevenue.IsValid && compPersonEndInsert.IsValid);
        
        }

    }
}


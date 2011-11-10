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

namespace PraticeManagement.Controls.Milestones
{
    public partial class MilestonePersonBar : System.Web.UI.UserControl
    {
        private const string DuplPersonName = "The specified person is already assigned on this milestone.";
        private const string milestonePersonEntryInsert = "MilestonePersonEntryInsert";
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

        protected void Page_Load(object sender, EventArgs e)
        {

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

        protected void custPeriodOvberlappingInsert_ServerValidate(object sender, ServerValidateEventArgs e)
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


        private void ValidateOvelappingWhenSaveClicked(object sender, ServerValidateEventArgs e)
        {
            DateTime startDate = dpPersonStartInsert.DateValue;
            DateTime endDate =
                dpPersonEndInsert.DateValue != DateTime.MinValue ? dpPersonEndInsert.DateValue : HostingPage.Milestone.ProjectedDeliveryDate;



            List<MilestonePersonEntry> entries = HostingControl.MilestonePersonsEntries;
            // Validate overlapping with other entries.
            for (int i = 0; i < entries.Count; i++)
            {
                var roleId =  entries[i].Role != null ? entries[i].Role.Id.ToString() : string.Empty;
                var personId = entries[i].ThisPerson.Id.ToString();
                if (personId == ddlPerson.SelectedValue && roleId == ddlRole.SelectedValue && entries[i].IsNewEntry == false)
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
                string roleId = "", personId = "";

                if (entry.IsShowPlusButton)
                {
                    roleId = entries[i].IsEditMode ? entries[i].EditedEntryValues["ddlRole"] : entries[i].Role != null ? entries[i].Role.Id.ToString() : string.Empty;
                    personId = entries[i].IsEditMode ? entries[i].EditedEntryValues["ddlPersonName"] : entries[i].ThisPerson.Id.ToString();
                }
                else
                {
                    var index = entries.FindIndex(mpe => mpe.Id == entries[i].ShowingPlusButtonEntryId);

                    personId = entries[index].IsEditMode ? entries[index].EditedEntryValues["ddlPersonName"] : entries[index].ThisPerson.Id.ToString();
                    roleId = entries[index].IsEditMode ? entries[index].EditedEntryValues["ddlRole"] : (entries[index].Role != null ? entries[index].Role.Id.ToString() : string.Empty);
                }

                 
                if (personId == ddlPerson.SelectedValue && roleId == ddlRole.SelectedValue && ((entries[i].IsNewEntry == false) || HostingPage.ValidateNewEntry))
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
                int days = GetPersonWorkDaysNumber(startDate, endDate, ddlPersonName);
                if (days == 0)
                {
                    e.IsValid = false;
                }
            }
        }

        protected void custPersonInsert_ServerValidate(object sender, ServerValidateEventArgs args)
        {

            Person person = HostingControl.GetPersonBySelectedValue(ddlPerson.SelectedValue);

            List<MilestonePersonEntry> entries = HostingControl.MilestonePersonsEntries.Where(entr => entr.ThisPerson.Id.Value == person.Id.Value).AsQueryable().ToList();

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

        protected void cvHoursInPeriod_ServerValidate(object source, ServerValidateEventArgs e)
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
                    int days = GetPersonWorkDaysNumber(dpPersonStartInsert.DateValue, dpPersonEndInsert.DateValue, ddlPerson);

                    // calculate hours per day according to HoursInPerod 
                    var hoursPerDay = (days != 0) ? decimal.Round(Totalhours / (days), 2) : 0;

                    e.IsValid = hoursPerDay > 0M;
                }
            }
        }

        #endregion

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

        protected void btnInsertPerson_Click(object sender, EventArgs e)
        {
            var bar = btnInsert.NamingContainer.NamingContainer as RepeaterItem;
            var result = InsertPerson(bar);
            if (!result)
            {
                HostingPage.lblResultObject.ShowErrorMessage("Error occured while saving resources.");
            }
        }

        private bool InsertPerson(RepeaterItem bar, bool isSaveCommit = true, bool iSDatBindRows = true, bool isValidating = true)
        {
            HostingControl.vsumMileStonePersonsObject.ValidationGroup = milestonePersonEntryInsert + bar.ItemIndex.ToString();

            bool result = true;
            if (isValidating)
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
            }

            return result;

        }

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            var bar = btnCancel.NamingContainer.NamingContainer as RepeaterItem;

            HostingControl.RemoveItemAndDaabindRepeater(bar.ItemIndex);
        }

        protected void imgCopy_OnClick(object sender, EventArgs e)
        {
            var bar = imgCopy.NamingContainer.NamingContainer as RepeaterItem;
            HostingControl.CopyItemAndDaabindRepeater(bar.ItemIndex);
        }


        protected string GetValidationGroup()
        {
            var bar = btnInsert.NamingContainer.NamingContainer as RepeaterItem;
            return milestonePersonEntryInsert + bar.ItemIndex.ToString();
        }

        internal bool ValidateAll(RepeaterItem mpBar, bool isSaveCommit)
        {
            return InsertPerson(mpBar, isSaveCommit,false);
        }

        internal bool SaveAll(RepeaterItem mpBar, bool isSaveCommit, bool iSDatBindRows)
        {
            return InsertPerson(mpBar, isSaveCommit, iSDatBindRows,false);
        }

    }
}


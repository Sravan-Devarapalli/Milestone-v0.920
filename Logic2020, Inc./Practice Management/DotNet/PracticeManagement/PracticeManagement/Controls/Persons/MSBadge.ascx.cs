using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;

namespace PraticeManagement.Controls.Persons
{
    public partial class MSBadge : System.Web.UI.UserControl
    {
        public DataTransferObjects.MSBadge BadgeDetails
        {
            get
            {
                return ViewState["MSBadge_Key"] as DataTransferObjects.MSBadge;
            }
            set
            {
                ViewState["MSBadge_Key"] = value;
            }
        }

        public List<Employment> EmpHistory
        {
            get;
            set;
        }

        private PraticeManagement.PersonDetail HostingPage
        {
            get { return ((PraticeManagement.PersonDetail)Page); }
        }

        public CheckBox BlockCheckBox
        {
            get
            {
                return chbBlockFromMS;
            }
        }

        public CheckBox ExceptionCheckBox
        {
            get
            {
                return chbException;
            }
        }

        public DropDownList PreviousBadgeDdl
        {
            get
            {
                return ddlPreviousAtMS;
            }
        }

        public List<Employment> GetFresherEmpHistory()
        {
            return new List<Employment>() { new Employment() { HireDate = HostingPage.CurrentHireDate, TerminationDate = null } };
        }

        protected void custExceptionNotMoreThan18moEndDate_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (!reqExceptionEnd.IsValid || !reqExceptionStart.IsValid || !cvExceptionStart.IsValid || !cvExceptionEnd.IsValid || !cvExceptionDateRanges.IsValid)
                return;
            if (BadgeDetails.BadgeStartDate.HasValue)
            {
                args.IsValid = BadgeDetails.BadgeEndDate.Value.Date <= dtpExceptionEnd.DateValue.Date; 
            }
        }

        protected void custExceptionMorethan18_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (!reqExceptionEnd.IsValid || !reqExceptionStart.IsValid || !cvExceptionStart.IsValid || !cvExceptionEnd.IsValid || !cvExceptionDateRanges.IsValid)
                return;
            if ((BadgeDetails.BadgeStartDate.HasValue && BadgeDetails.BadgeStartDateSource == "MS Exception" && BadgeDetails.BadgeEndDateSource == "MS Exception" && BadgeDetails.PlannedEndDateSource == "MS Exception") || !BadgeDetails.BadgeStartDate.HasValue)
            {
                var date1 = dtpExceptionStart.DateValue;
                var date2 = dtpExceptionEnd.DateValue.AddDays(1);
                args.IsValid = ((((date2.Year - date1.Year) * 12) + date2.Month - date1.Month) >= 18);
            }
        }

        protected void custBlockDatesInEmpHistory_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            var isValid = false;
            if (!reqBlockStart.IsValid || !reqBlockEnd.IsValid || !cvBlockEnd.IsValid || !cvBlockDateRange.IsValid || !cvBlockStart.IsValid)
                return;
            if (EmpHistory == null)
            {
                if (HostingPage.PersonId.HasValue)
                    EmpHistory = ServiceCallers.Custom.Person(p => p.GetPersonEmploymentHistoryById(HostingPage.PersonId.Value).ToList());
                else
                    EmpHistory = GetFresherEmpHistory();
            }
            foreach (var employment in EmpHistory)
            {
                if (dtpBlockStart.DateValue >= employment.HireDate && (!employment.TerminationDate.HasValue || (dtpBlockEnd.DateValue <= employment.TerminationDate.Value)))
                    isValid = true;
            }
            args.IsValid = isValid;
        }

        protected void custExceptionInEmpHistory_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            var isValid = false;
            if (!reqExceptionEnd.IsValid || !reqExceptionStart.IsValid || !cvExceptionStart.IsValid || !cvExceptionEnd.IsValid || !cvExceptionDateRanges.IsValid)
                return;
            if (EmpHistory == null)
            {
                if (HostingPage.PersonId.HasValue)
                    EmpHistory = ServiceCallers.Custom.Person(p => p.GetPersonEmploymentHistoryById(HostingPage.PersonId.Value).ToList());
                else
                    EmpHistory = GetFresherEmpHistory();
            }
            foreach (var employment in EmpHistory)
            {
                if (dtpExceptionStart.DateValue >= employment.HireDate && (!employment.TerminationDate.HasValue || (dtpExceptionEnd.DateValue <= employment.TerminationDate.Value)))
                    isValid = true;
            }
            args.IsValid = isValid;
        }

        protected void custBlockDatesOverlappedException_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (!reqExceptionEnd.IsValid || !reqExceptionStart.IsValid || !cvExceptionStart.IsValid || !cvExceptionEnd.IsValid || !cvExceptionDateRanges.IsValid || !reqBlockStart.IsValid || !reqBlockEnd.IsValid || !cvBlockEnd.IsValid || !cvBlockDateRange.IsValid || !cvBlockStart.IsValid || !HostingPage.PersonId.HasValue)
                return;
            if (BadgeDetails.IsBlocked)
            {
                args.IsValid = !(dtpBlockStart.DateValue <= dtpExceptionEnd.DateValue && dtpExceptionStart.DateValue <= dtpBlockEnd.DateValue);
            }
        }

        protected void custPersonInProject_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (!chbBlockFromMS.Checked || !reqBlockStart.IsValid || !reqBlockEnd.IsValid || !cvBlockEnd.IsValid || !cvBlockDateRange.IsValid || !cvBlockStart.IsValid || !HostingPage.PersonId.HasValue)
                return;
            args.IsValid = !ServiceCallers.Custom.Person(p => p.CheckIfPersonInProjectForDates(HostingPage.PersonId.Value, dtpBlockStart.DateValue, dtpBlockEnd.DateValue));
        }

        protected void custExceptionDatesOverlappsBlock_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (!reqExceptionEnd.IsValid || !reqExceptionStart.IsValid || !cvExceptionStart.IsValid || !cvExceptionEnd.IsValid || !cvExceptionDateRanges.IsValid || !reqBlockStart.IsValid || !reqBlockEnd.IsValid || !cvBlockEnd.IsValid || !cvBlockDateRange.IsValid || !cvBlockStart.IsValid || !HostingPage.PersonId.HasValue)
                return;
            if (BadgeDetails.IsException)
            {
                args.IsValid = !(dtpBlockStart.DateValue <= dtpExceptionEnd.DateValue && dtpExceptionStart.DateValue <= dtpBlockEnd.DateValue);
            }
        }

        protected void cvExceptionStartAfterJuly_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (!reqExceptionEnd.IsValid || !reqExceptionStart.IsValid || !cvExceptionStart.IsValid || !cvExceptionEnd.IsValid || !cvExceptionDateRanges.IsValid)
                return;
            args.IsValid = dtpExceptionStart.DateValue >= new DateTime(2014, 7, 1);
        }

        public string ValidationGroup
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (HostingPage.PersonId.HasValue)
                {
                    PopulateData();
                    AssignValidationGroup();
                    lnkHistory.Visible = true;
                }
                else
                {
                    lnkHistory.Visible = false;
                    AssignValidationGroup();
                }
            }
        }

        protected void custNotFuture_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = !(dtpLastBadgeStart.DateValue.Date > DateTime.Today.Date);
        }

        protected void repMSBadge_DataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (DataTransferObjects.MSBadge)e.Item.DataItem;
                var hlProjectNumber = e.Item.FindControl("hlProjectNumber") as HyperLink;
                var lblIsApproved = e.Item.FindControl("lblIsApproved") as Label;
                hlProjectNumber.Visible = dataItem.Project.Id.Value != -1;
                lblIsApproved.Text = dataItem.Project.Id == -1 ? "-----" : dataItem.IsApproved ? "Yes" : "No";
            }
        }

        private void BindBadgeHistory()
        {
            var details = ServiceCallers.Custom.Person(p => p.GetLogic2020BadgeHistory(HostingPage.PersonId.Value)).ToList();
            if (details.Count > 0)
            {
                repMSBadge.Visible = true;
                repMSBadge.DataSource = details;
                repMSBadge.DataBind();
                divEmptyMessage.Style["display"] = "none";
            }
            else
            {
                repMSBadge.Visible = false;
                divEmptyMessage.Style["display"] = "";
            }
        }

        protected string GetProjectDetailsLink(int? projectId)
        {

            return Utils.Generic.GetTargetUrlWithReturn(String.Format(Constants.ApplicationPages.DetailRedirectFormat, Constants.ApplicationPages.ProjectDetail, projectId.Value),
                                                        Constants.ApplicationPages.PersonsPage);
        }

        public void PopulateData()
        {
            var details = ServiceCallers.Custom.Person(p => p.GetBadgeDetailsByPersonId(HostingPage.PersonId.Value)).ToList();
            var badge = details.Count == 0 ? new DataTransferObjects.MSBadge() : details[0];
            BadgeDetails = badge;
            txtBadgeStart.Text = badge.BadgeStartDate.HasValue ? badge.BadgeStartDate.Value.ToShortDateString() : string.Empty;
            txtPlannedEnd.Text = badge.PlannedEndDate.HasValue ? badge.PlannedEndDate.Value.ToShortDateString() : string.Empty;
            txtPreviousBadgeAlias.Text = badge.PreviousBadgeAlias;
            txtBadgeEnd.Text = badge.BadgeEndDate.HasValue ? badge.BadgeEndDate.Value.ToShortDateString() : string.Empty;
            dtpLastBadgeStart.TextValue = badge.LastBadgeStartDate.HasValue ? badge.LastBadgeStartDate.Value.ToShortDateString() : string.Empty;
            txtBreakStart.Text = badge.BreakStartDate.HasValue ? badge.BreakStartDate.Value.ToShortDateString() : string.Empty;
            txtBreakEnd.Text = badge.BreakEndDate.HasValue ? badge.BreakEndDate.Value.ToShortDateString() : string.Empty;
            dtpLastBadgeEnd.TextValue = badge.LastBadgeEndDate.HasValue ? badge.LastBadgeEndDate.Value.ToShortDateString() : string.Empty;
            dtpBlockStart.TextValue = badge.BlockStartDate.HasValue ? badge.BlockStartDate.Value.ToShortDateString() : string.Empty;
            dtpBlockEnd.TextValue = badge.BlockEndDate.HasValue ? badge.BlockEndDate.Value.ToShortDateString() : string.Empty;
            dtpExceptionStart.TextValue = badge.ExceptionStartDate.HasValue ? badge.ExceptionStartDate.Value.ToShortDateString() : string.Empty;
            dtpExceptionEnd.TextValue = badge.ExceptionEndDate.HasValue ? badge.ExceptionEndDate.Value.ToShortDateString() : string.Empty;
            ddlPreviousAtMS.SelectedValue = badge.IsPreviousBadge ? "1" : "0";
            chbException.Checked = badge.IsException;
            chbBlockFromMS.Checked = badge.IsBlocked;
            chbBlockFromMS_CheckedChanged(chbBlockFromMS, new EventArgs());
            chbException_CheckedChanged(chbException, new EventArgs());
            ddlPreviousAtMS_OnIndexChanged(ddlPreviousAtMS, new EventArgs());
            lblPlannedDateSource.Text = string.IsNullOrEmpty(badge.PlannedEndDateSource) ? "Available Now" : badge.PlannedEndDateSource;
            lblBadgeStartDateSource.Text = string.IsNullOrEmpty(badge.BadgeStartDateSource) ? "Available Now" : badge.BadgeStartDateSource;
            lblBadgeEndDateSource.Text = string.IsNullOrEmpty(badge.BadgeEndDateSource) ? "Available Now" : badge.BadgeEndDateSource;
            reqBlockStart.Enabled = reqBlockEnd.Enabled = cvBlockStart.Enabled = cvBlockEnd.Enabled = cvBlockDateRange.Enabled = custBlockDatesInEmpHistory.Enabled = chbBlockFromMS.Checked;
            custExceptionDatesOverlappsBlock.Enabled = custExceptionMorethan18.Enabled = custExceptionNotMoreThan18moEndDate.Enabled = 
            custPersonInProject.Enabled = custBlockDatesOverlappedException.Enabled = reqExceptionEnd.Enabled = reqExceptionStart.Enabled = cvExceptionStartAfterJuly.Enabled = cvExceptionStart.Enabled = cvExceptionDateRanges.Enabled = cvExceptionEnd.Enabled = custExceptionInEmpHistory.Enabled = chbException.Checked;
            reqPreviousAlias.Enabled = reqLastBadgeStart.Enabled = reqLastbadgeEnd.Enabled = cvLastbadgeEnd.Enabled = cvLastBadgeRange.Enabled = custNotFuture.Enabled = cvLastBadgeStart.Enabled = ddlPreviousAtMS.SelectedValue == "1";
            BindBadgeHistory();
            badgeHistory.PopulateData();
            pnlHistoryPanel.Attributes["class"] = badgeHistory.Width;
        }

        public void chbBlockFromMS_CheckedChanged(object sender, EventArgs e)
        {
            dtpBlockStart.ReadOnly =
            dtpBlockEnd.ReadOnly = !chbBlockFromMS.Checked;
            dtpBlockStart.EnabledTextBox = dtpBlockEnd.EnabledTextBox = chbBlockFromMS.Checked;

            if (chbBlockFromMS.Checked)
            {
                if (BadgeDetails == null)
                {
                    dtpBlockStart.TextValue = dtpBlockEnd.TextValue = string.Empty;
                }
                else
                {
                    dtpBlockStart.TextValue = BadgeDetails.BlockStartDate.HasValue ? BadgeDetails.BlockStartDate.Value.ToShortDateString() : string.Empty;
                    dtpBlockEnd.TextValue = BadgeDetails.BlockEndDate.HasValue ? BadgeDetails.BlockEndDate.Value.ToShortDateString() : string.Empty;
                }
            }
            else
            {
                dtpBlockStart.TextValue = string.Empty;
                dtpBlockEnd.TextValue = string.Empty;
            }
            custExceptionDatesOverlappsBlock.Enabled = 
              custPersonInProject.Enabled = custBlockDatesOverlappedException.Enabled = reqBlockStart.Enabled = reqBlockEnd.Enabled = cvBlockStart.Enabled = cvBlockEnd.Enabled = cvBlockDateRange.Enabled = custBlockDatesInEmpHistory.Enabled = chbBlockFromMS.Checked;
        }

        public void chbException_CheckedChanged(object sender, EventArgs e)
        {
            dtpExceptionStart.ReadOnly =
             dtpExceptionEnd.ReadOnly = !chbException.Checked;
            dtpExceptionStart.EnabledTextBox = dtpExceptionEnd.EnabledTextBox = chbException.Checked;
            if (chbException.Checked)
            {
                if (BadgeDetails == null)
                {
                    dtpExceptionStart.TextValue = dtpExceptionEnd.TextValue = string.Empty;
                }
                else
                {
                    dtpExceptionStart.TextValue = BadgeDetails.ExceptionStartDate.HasValue ? BadgeDetails.ExceptionStartDate.Value.ToShortDateString() : string.Empty;
                    dtpExceptionEnd.TextValue = BadgeDetails.ExceptionEndDate.HasValue ? BadgeDetails.ExceptionEndDate.Value.ToShortDateString() : string.Empty;
                }
            }
            else
            {
                dtpExceptionStart.TextValue = string.Empty;
                dtpExceptionEnd.TextValue = string.Empty;
            }
            custExceptionDatesOverlappsBlock.Enabled = custExceptionMorethan18.Enabled = custExceptionNotMoreThan18moEndDate.Enabled =
                custBlockDatesOverlappedException.Enabled = reqExceptionEnd.Enabled = reqExceptionStart.Enabled = cvExceptionStartAfterJuly.Enabled = cvExceptionStart.Enabled = cvExceptionDateRanges.Enabled = cvExceptionEnd.Enabled = custExceptionInEmpHistory.Enabled = chbException.Checked;
        }

        public void ddlPreviousAtMS_OnIndexChanged(object sender, EventArgs e)
        {
            dtpLastBadgeStart.ReadOnly =
             dtpLastBadgeEnd.ReadOnly = ddlPreviousAtMS.SelectedValue != "1";
            txtPreviousBadgeAlias.Enabled =
             dtpLastBadgeStart.EnabledTextBox = dtpLastBadgeEnd.EnabledTextBox = ddlPreviousAtMS.SelectedValue == "1";
            if (ddlPreviousAtMS.SelectedValue == "0")
            {
                txtPreviousBadgeAlias.Text = string.Empty;
                dtpLastBadgeEnd.TextValue = dtpLastBadgeStart.TextValue = string.Empty;
            }
            else
            {
                if (BadgeDetails == null)
                    txtPreviousBadgeAlias.Text = dtpLastBadgeEnd.TextValue = dtpLastBadgeStart.TextValue = string.Empty;
                else
                {
                    txtPreviousBadgeAlias.Text = BadgeDetails.PreviousBadgeAlias;
                    dtpLastBadgeEnd.TextValue = BadgeDetails.LastBadgeEndDate.HasValue ? BadgeDetails.LastBadgeEndDate.Value.ToShortDateString() : string.Empty;
                    dtpLastBadgeStart.TextValue = BadgeDetails.LastBadgeStartDate.HasValue ? BadgeDetails.LastBadgeStartDate.Value.ToShortDateString() : string.Empty;
                }
            }
            reqPreviousAlias.Enabled = reqLastBadgeStart.Enabled = reqLastbadgeEnd.Enabled = cvLastbadgeEnd.Enabled = cvLastBadgeRange.Enabled = custNotFuture.Enabled = cvLastBadgeStart.Enabled = ddlPreviousAtMS.SelectedValue == "1";
        }

        protected string GetDateFormat(DateTime date)
        {
            return date.ToString(Constants.Formatting.EntryDateFormat);
        }

        public DataTransferObjects.MSBadge PrepareBadgeDetails()
        {
            var loggedInPersonId = DataHelper.CurrentPerson.Id.Value;
            return new DataTransferObjects.MSBadge()
            {

                Person = new DataTransferObjects.Person()
                {
                    Id = HostingPage.PersonId
                },
                IsBlocked = chbBlockFromMS.Checked,
                BlockStartDate = chbBlockFromMS.Checked ? (DateTime?)dtpBlockStart.DateValue : null,
                BlockEndDate = chbBlockFromMS.Checked ? (DateTime?)dtpBlockEnd.DateValue : null,
                IsPreviousBadge = ddlPreviousAtMS.SelectedValue == "1",
                PreviousBadgeAlias = ddlPreviousAtMS.SelectedValue == "1" ? txtPreviousBadgeAlias.Text : null,
                LastBadgeStartDate = ddlPreviousAtMS.SelectedValue == "1" ? (DateTime?)dtpLastBadgeStart.DateValue : null,
                LastBadgeEndDate = ddlPreviousAtMS.SelectedValue == "1" ? (DateTime?)dtpLastBadgeEnd.DateValue : null,
                IsException = chbException.Checked,
                ExceptionStartDate = chbException.Checked ? (DateTime?)dtpExceptionStart.DateValue : null,
                ExceptionEndDate = chbException.Checked ? (DateTime?)dtpExceptionEnd.DateValue : null,
                ModifiedById = loggedInPersonId
            };
        }

        public void SaveData()
        {
            ServiceCallers.Custom.Person(p => p.SaveBadgeDetailsByPersonId(PrepareBadgeDetails()));
        }

        public void AssignValidationGroup()
        {
            reqPreviousAlias.ValidationGroup = reqLastBadgeStart.ValidationGroup = cvLastBadgeStart.ValidationGroup = cvLastbadgeEnd.ValidationGroup = custNotFuture.ValidationGroup = cvLastBadgeRange.ValidationGroup = reqLastbadgeEnd.ValidationGroup = cvBlockStart.ValidationGroup = reqBlockStart.ValidationGroup = dtpLastBadgeStart.ValidationGroup = dtpLastBadgeEnd.ValidationGroup = dtpBlockStart.ValidationGroup =
            custPersonInProject.ValidationGroup = custBlockDatesOverlappedException.ValidationGroup =
            custExceptionDatesOverlappsBlock.ValidationGroup = custExceptionMorethan18.ValidationGroup = custExceptionNotMoreThan18moEndDate.ValidationGroup =
            dtpExceptionStart.ValidationGroup = custExceptionInEmpHistory.ValidationGroup = custBlockDatesInEmpHistory.ValidationGroup = cvExceptionStartAfterJuly.ValidationGroup = cvExceptionStart.ValidationGroup = reqExceptionStart.ValidationGroup = dtpBlockEnd.ValidationGroup = cvBlockEnd.ValidationGroup = cvBlockDateRange.ValidationGroup = reqBlockEnd.ValidationGroup = dtpExceptionEnd.ValidationGroup = cvExceptionDateRanges.ValidationGroup = cvExceptionEnd.ValidationGroup = reqExceptionEnd.ValidationGroup = ValidationGroup;
        }

        public void ValidateMSBadgeDetails()
        {
            if (Page.IsValid)
            {
                reqPreviousAlias.Validate();
                reqLastBadgeStart.Validate();
                cvLastBadgeStart.Validate();
                reqLastbadgeEnd.Validate();
                cvLastBadgeRange.Validate();
                cvLastbadgeEnd.Validate();
                reqBlockStart.Validate();
                cvBlockEnd.Validate();
                reqBlockEnd.Validate();
                cvBlockDateRange.Validate();
                cvBlockStart.Validate();
                reqExceptionStart.Validate();
                reqExceptionEnd.Validate();
                cvExceptionDateRanges.Validate();
                cvExceptionStartAfterJuly.Validate();
                cvExceptionStart.Validate();
                cvExceptionEnd.Validate();
                custNotFuture.Validate();
                custBlockDatesInEmpHistory.Validate();
                custExceptionInEmpHistory.Validate();
                custBlockDatesOverlappedException.Validate();
                custPersonInProject.Validate();
                custExceptionDatesOverlappsBlock.Validate();
                custExceptionMorethan18.Validate();
                custExceptionNotMoreThan18moEndDate.Validate();
            }
        }

        protected void lnkHistory_Click(object sender, EventArgs e)
        {
            mpeBadgeHistoryPanel.Show();
        }
    }
}

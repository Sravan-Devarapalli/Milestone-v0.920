using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using System.Web.Security;

namespace PraticeManagement.Controls.Persons
{
    public partial class MSBadge : System.Web.UI.UserControl
    {
        //public const string assignedInProjectMsg = "You will not be able to adjust the 18 months dates as the person is assigned to projects from {0}-{1}.";

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

        public List<DataTransferObjects.MSBadge> BadgeDates
        {
            get;
            set;
        }

        public bool IsFirstValidation
        {
            get;
            set;
        }

        public bool IsDeactivatePriorValidated
        {
            get;
            set;
        }

        public List<Employment> GetFresherEmpHistory()
        {
            return new List<Employment>() { new Employment() { HireDate = HostingPage.CurrentHireDate, TerminationDate = null } };
        }

        protected void custExceptionNotMoreThan18moEndDate_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (!reqExceptionEnd.IsValid || !reqExceptionStart.IsValid || !cvExceptionStart.IsValid || !cvExceptionEnd.IsValid || !cvExceptionDateRanges.IsValid)
                return;
            if (BadgeDetails != null && BadgeDetails.BadgeStartDate.HasValue)
            {
                args.IsValid = BadgeDetails.BadgeEndDate.Value.Date <= dtpExceptionEnd.DateValue.Date; 
            }
        }

        protected void custExceptionMorethan18_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (!reqExceptionEnd.IsValid || !reqExceptionStart.IsValid || !cvExceptionStart.IsValid || !cvExceptionEnd.IsValid || !cvExceptionDateRanges.IsValid)
                return;
            if (BadgeDetails == null || (BadgeDetails.BadgeStartDate.HasValue && BadgeDetails.BadgeStartDateSource == "MS Exception" && BadgeDetails.BadgeEndDateSource == "MS Exception" && BadgeDetails.PlannedEndDateSource == "MS Exception") || !BadgeDetails.BadgeStartDate.HasValue)
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
            if (BadgeDetails != null && BadgeDetails.IsBlocked)
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
            if (BadgeDetails != null && BadgeDetails.IsException)
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

        protected void custBlockStartAfterJuly_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (!reqBlockStart.IsValid || !reqBlockEnd.IsValid || !cvBlockStart.IsValid || !cvBlockEnd.IsValid || !cvBlockDateRange.IsValid)
                return;
            args.IsValid = dtpBlockStart.DateValue >= new DateTime(2014, 7, 1);
        }

        protected void custDeactivateNeed18moDates_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if(BadgeDetails == null)
                args.IsValid = false;
            else
                args.IsValid = BadgeDetails.BadgeStartDate.HasValue;
            if (!args.IsValid)
            {
                dpDeactivatedDate.TextValue = txtOrganicEnd.Text = txtOrganicStart.Text = string.Empty;
            }
        }

        protected void custDeactivateDateIn18moDates_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (!custDeactivateNeed18moDates.IsValid || !compDeactivatedDate.IsValid || BadgeDetails == null || BadgeDetails.BadgeStartDateSource == "Badge Deactivation Date")
                return;
            var deactivateDate  = dpDeactivatedDate.DateValue;
            args.IsValid = (deactivateDate.Date <= dpBadgeEnd.DateValue.Date && deactivateDate.Date >= dpBadgeStart.DateValue.Date);
        }

        protected void custDeactivateDatePriorProjectDates_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (!custDeactivateNeed18moDates.IsValid || !compDeactivatedDate.IsValid || !custDeactivateDateIn18moDates.IsValid || BadgeDetails == null) 
                return;
            IsDeactivatePriorValidated = true;
            BadgeDates = ServiceCallers.Custom.Person(p=>p.GetBadgeRecordsAfterDeactivatedDate(HostingPage.PersonId.Value,dpDeactivatedDate.DateValue).ToList());
            args.IsValid = !(BadgeDates.Any(b => dpDeactivatedDate.DateValue < b.BadgeStartDate.Value));
        }
        protected void custAftertheBreak_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (!reqBadgeStart.IsValid || !compBadgeStart.IsValid || !custBeforeJuly.IsValid)
                return;
            var isInBreakPereiod = ServiceCallers.Custom.Person(p => p.CheckIfDatesInDeactivationHistory(HostingPage.PersonId.Value, dpBadgeStart.DateValue, dpBadgeEnd.DateValue));
            args.IsValid = !(isInBreakPereiod.Any(i => i.Equals(true)));
        }
        protected void custDeactivateWithinProject_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (!IsFirstValidation || BadgeDetails == null) 
                return;
            args.IsValid = !(BadgeDates.Any(b => (dpDeactivatedDate.DateValue >= b.BadgeStartDate.Value) && (dpDeactivatedDate.DateValue < b.BadgeEndDate.Value)));
            if (!args.IsValid)
            {
                HostingPage.IsOtherPanelDisplay = true;
                mpeDeactivateWithinProject.Show();
            }
        }

        protected void btnOk_Click(object sender, EventArgs args)
        {
            IsFirstValidation = false;
            HostingPage.btnSave_Click(HostingPage.SaveButton,new EventArgs());
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
                //reqBadgeStart.Enabled = reqBadgeEnd.Enabled = false;
                tdExcludeInReports.Visible = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
            }
            IsFirstValidation = true;
            IsDeactivatePriorValidated = false;
            if (string.IsNullOrEmpty(dpBadgeStart.TextValue) && string.IsNullOrEmpty(dpBadgeEnd.TextValue))
            {
                reqBadgeStart.Enabled = reqBadgeEnd.Enabled = false;
            }
        }

        protected void custNotFuture_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = !(dtpLastBadgeStart.DateValue.Date > DateTime.Today.Date);
        }

        protected void custLessThan18Mo_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (lblBadgeStartDateSource.Text != "Manual Entry")
                return;
            var endDate = dpBadgeEnd.DateValue.AddDays(1);
            var months = ((endDate.Year - dpBadgeStart.DateValue.Year) * 12) + endDate.Month - dpBadgeStart.DateValue.Month;
            args.IsValid = !(months < 18);
        }

        protected void custMoreThan18Mo_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (lblBadgeStartDateSource.Text != "Manual Entry")
                return;
            if (reqBadgeStart.IsValid && reqBadgeEnd.IsValid && compBadgeEnd.IsValid && compBadgeStart.IsValid && compBadgeEndLess.IsValid)
            {
                var startdate = dpBadgeStart.DateValue;
                var realEnddate = startdate.AddMonths(18).AddDays(-1);
                var presentEndDate = dpBadgeEnd.DateValue;
                var months = ((presentEndDate.Year - startdate.Year) * 12) + presentEndDate.Month - startdate.Month;
                var isValid = true;
                if (months >= 18 && realEnddate.Date != presentEndDate.Date)
                {
                    if (!chbException.Checked)
                        isValid = false;
                    else
                    {
                        if (reqExceptionStart.IsValid && reqExceptionEnd.IsValid && cvExceptionStart.IsValid && cvExceptionEnd.IsValid)
                        {
                            var expectedStartDate = presentEndDate.AddMonths(-18).AddDays(1);
                            isValid = realEnddate.AddDays(1).Date >= dtpExceptionStart.DateValue.Date && presentEndDate.Date <= dtpExceptionEnd.DateValue.Date;
                            isValid = isValid || (startdate.Date >= dtpExceptionStart.DateValue.Date && expectedStartDate.Date <= dtpExceptionEnd.DateValue.Date);
                        }
                    }
                }
                args.IsValid = isValid;
            }
        }

        protected void custProjectsAssigned_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (lblBadgeStartDateSource.Text != "Manual Entry" || dpBadgeStart.DateValue == DateTime.MinValue)
                return;
            if (BadgeDetails == null || !BadgeDetails.BadgeStartDate.HasValue)
                return;
            var validator = sender as CustomValidator;
            var isStartDateShrinked = BadgeDetails.BadgeStartDate.HasValue && (BadgeDetails.BadgeStartDate.Value.Date < dpBadgeStart.DateValue.Date);
            var isEndDateShrinked = BadgeDetails.BadgeEndDate.HasValue && (BadgeDetails.BadgeEndDate.Value.Date > dpBadgeEnd.DateValue.Date);
            var currentEndDate = dpBadgeEnd.DateValue.AddDays(1);
            var currentStartDate = dpBadgeStart.DateValue.AddDays(-1);
            args.IsValid = !(ServiceCallers.Custom.Person(p => p.CheckIfPersonInProjectsForThisPeriod(isEndDateShrinked ? (DateTime?)currentEndDate : null, isEndDateShrinked ? BadgeDetails.BadgeEndDate : null, isStartDateShrinked ? (DateTime?)currentStartDate : null, isStartDateShrinked ? BadgeDetails.BadgeStartDate : null, HostingPage.PersonId.Value)));
            //validator.ErrorMessage = validator.ToolTip = string.Format(assignedInProjectMsg, BadgeDetails.BadgeStartDate.Value.Date.ToShortDateString(), BadgeDetails.BadgeEndDate.Value.Date.ToShortDateString());
        }

        protected void cust18moNotInEmployment_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (lblBadgeStartDateSource.Text != "Manual Entry")
                return;
            var startDate = new DateTime();
            var endDate = new DateTime();
            var isValid = false;
            if (DateTime.TryParse(dpBadgeStart.TextValue, out startDate) && DateTime.TryParse(dpBadgeEnd.TextValue, out endDate))
            {
                if (EmpHistory == null)
                {
                    if (HostingPage.PersonId.HasValue)
                        EmpHistory = ServiceCallers.Custom.Person(p => p.GetPersonEmploymentHistoryById(HostingPage.PersonId.Value).ToList());
                    else
                        EmpHistory = GetFresherEmpHistory();
                }
                foreach (var employment in EmpHistory)
                {
                    if (startDate >= employment.HireDate && (!employment.TerminationDate.HasValue || (endDate <= employment.TerminationDate.Value)))
                        isValid = true;
                }
                args.IsValid = isValid;
            }
        }

        protected void custBeforeJuly_ServerValidate(object sender, ServerValidateEventArgs args)
        {
            if (lblBadgeStartDateSource.Text != "Manual Entry")
                return;
            var july2014 = new DateTime(2014, 7, 1);
            args.IsValid = dpBadgeStart.DateValue.Date >= july2014;
        }

        protected void repMSBadge_DataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (DataTransferObjects.MSBadge)e.Item.DataItem;
                var hlProjectNumber = e.Item.FindControl("hlProjectNumber") as HyperLink;
                var lblIsApproved = e.Item.FindControl("lblIsApproved") as Label;
                var lblProjectStatus = e.Item.FindControl("lblProjectStatus") as Label;
                hlProjectNumber.Visible = dataItem.Project.Id.Value != -1;
                lblIsApproved.Text = dataItem.Project.Id == -1 ? "-----" : dataItem.IsApproved ? "Yes" : "No";
                lblProjectStatus.Text = dataItem.Project.Id == -1 ? string.Empty : dataItem.Project.Status.StatusType.ToString();
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
            dpBadgeStart.TextValue = badge.BadgeStartDate.HasValue ? badge.BadgeStartDate.Value.ToShortDateString() : string.Empty;
            txtPlannedEnd.Text = badge.PlannedEndDate.HasValue ? badge.PlannedEndDate.Value.ToShortDateString() : string.Empty;
            txtPreviousBadgeAlias.Text = badge.PreviousBadgeAlias;
            dpBadgeEnd.TextValue = badge.BadgeEndDate.HasValue ? badge.BadgeEndDate.Value.ToShortDateString() : string.Empty;
            dtpLastBadgeStart.TextValue = badge.LastBadgeStartDate.HasValue ? badge.LastBadgeStartDate.Value.ToShortDateString() : string.Empty;
            txtBreakStart.Text = badge.BreakStartDate.HasValue ? badge.BreakStartDate.Value.ToShortDateString() : string.Empty;
            txtBreakEnd.Text = badge.BreakEndDate.HasValue ? badge.BreakEndDate.Value.ToShortDateString() : string.Empty;
            dtpLastBadgeEnd.TextValue = badge.LastBadgeEndDate.HasValue ? badge.LastBadgeEndDate.Value.ToShortDateString() : string.Empty;
            dtpBlockStart.TextValue = badge.BlockStartDate.HasValue ? badge.BlockStartDate.Value.ToShortDateString() : string.Empty;
            dtpBlockEnd.TextValue = badge.BlockEndDate.HasValue ? badge.BlockEndDate.Value.ToShortDateString() : string.Empty;
            dtpExceptionStart.TextValue = badge.ExceptionStartDate.HasValue ? badge.ExceptionStartDate.Value.ToShortDateString() : string.Empty;
            dtpExceptionEnd.TextValue = badge.ExceptionEndDate.HasValue ? badge.ExceptionEndDate.Value.ToShortDateString() : string.Empty;

            dpDeactivatedDate.TextValue = badge.DeactivatedDate.HasValue ? badge.DeactivatedDate.Value.ToShortDateString() : string.Empty;
            txtOrganicStart.Text = badge.OrganicBreakStartDate.HasValue ? badge.OrganicBreakStartDate.Value.ToShortDateString() : string.Empty;
            txtOrganicEnd.Text = badge.OrganicBreakEndDate.HasValue ? badge.OrganicBreakEndDate.Value.ToShortDateString() : string.Empty;

            ddlPreviousAtMS.SelectedValue = badge.IsPreviousBadge ? "1" : "0";
            chbExcludeInReports.Checked = badge.ExcludeInReports;
            chbManageServiceContract.Checked = badge.IsMSManagedService;
            chbException.Checked = badge.IsException;
            chbBlockFromMS.Checked = badge.IsBlocked;
            chbBlockFromMS_CheckedChanged(chbBlockFromMS, new EventArgs());
            chbException_CheckedChanged(chbException, new EventArgs());
            ddlPreviousAtMS_OnIndexChanged(ddlPreviousAtMS, new EventArgs());
            lblPlannedDateSource.Text = string.IsNullOrEmpty(badge.PlannedEndDateSource) ? "Available Now" : badge.PlannedEndDateSource;
            lblBadgeStartDateSource.Text = string.IsNullOrEmpty(badge.BadgeStartDateSource) ? "Available Now" : badge.BadgeStartDateSource;
            lblBadgeEndDateSource.Text = string.IsNullOrEmpty(badge.BadgeEndDateSource) ? "Available Now" : badge.BadgeEndDateSource;
            reqBlockStart.Enabled = reqBlockEnd.Enabled = cvBlockStart.Enabled = custBlockStartAfterJuly.Enabled = cvBlockEnd.Enabled = cvBlockDateRange.Enabled = custBlockDatesInEmpHistory.Enabled = custPersonInProject.Enabled = chbBlockFromMS.Checked;
            compDeactivatedDate.Enabled = custDeactivateNeed18moDates.Enabled = custDeactivateDateIn18moDates.Enabled = custDeactivateDatePriorProjectDates.Enabled = custDeactivateWithinProject.Enabled = cust18moNotInEmployment.Enabled = true;
            reqBadgeStart.Enabled =  reqBadgeEnd.Enabled =  custLessThan18Mo.Enabled = custMoreThan18Mo.Enabled = custBeforeJuly.Enabled = custProjectsAssigned.Enabled = BadgeDetails.BadgeStartDate.HasValue;
            custExceptionDatesOverlappsBlock.Enabled = custExceptionMorethan18.Enabled = custExceptionNotMoreThan18moEndDate.Enabled =
            custBlockDatesOverlappedException.Enabled = reqExceptionEnd.Enabled = reqExceptionStart.Enabled = cvExceptionStartAfterJuly.Enabled = cvExceptionStart.Enabled = cvExceptionDateRanges.Enabled = cvExceptionEnd.Enabled = custExceptionInEmpHistory.Enabled = chbException.Checked;
            reqPreviousAlias.Enabled = reqLastBadgeStart.Enabled = reqLastbadgeEnd.Enabled = cvLastbadgeEnd.Enabled = cvLastBadgeRange.Enabled = custNotFuture.Enabled = cvLastBadgeStart.Enabled = ddlPreviousAtMS.SelectedValue == "1";
            compBadgeStart.Enabled = compBadgeEnd.Enabled = compBadgeEndLess.Enabled = custAftertheBreak.Enabled = true;
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
              custPersonInProject.Enabled = custBlockDatesOverlappedException.Enabled = custBlockStartAfterJuly.Enabled = reqBlockStart.Enabled = reqBlockEnd.Enabled = cvBlockStart.Enabled = cvBlockEnd.Enabled = cvBlockDateRange.Enabled = custBlockDatesInEmpHistory.Enabled = chbBlockFromMS.Checked;
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
                ModifiedById = loggedInPersonId,
                BadgeStartDate = dpBadgeStart.TextValue == string.Empty ? null : (DateTime?)dpBadgeStart.DateValue,
                BadgeEndDate = dpBadgeEnd.TextValue == string.Empty ? null : (DateTime?)dpBadgeEnd.DateValue,
                BadgeStartDateSource = lblBadgeStartDateSource.Text,
                BadgeEndDateSource = lblBadgeEndDateSource.Text,
                BreakStartDate = txtBreakStart.Text == string.Empty? null:(DateTime?)Convert.ToDateTime(txtBreakStart.Text),
                BreakEndDate = txtBreakEnd.Text == string.Empty ? null : (DateTime?)Convert.ToDateTime(txtBreakEnd.Text),
                DeactivatedDate = dpDeactivatedDate.TextValue == string.Empty ? null : (DateTime?)dpDeactivatedDate.DateValue,
                OrganicBreakStartDate = dpDeactivatedDate.TextValue == string.Empty ? null : (DateTime?)Convert.ToDateTime(txtOrganicStart.Text),
                OrganicBreakEndDate = dpDeactivatedDate.TextValue == string.Empty ? null : (DateTime?)Convert.ToDateTime(txtOrganicEnd.Text),
                ExcludeInReports = chbExcludeInReports.Checked,
                IsMSManagedService = chbManageServiceContract.Checked
            };
        }

        public void SaveData()
        {
            ServiceCallers.Custom.Person(p => p.SaveBadgeDetailsByPersonId(PrepareBadgeDetails()));
        }

        public void AssignValidationGroup()
        {
            cust18moNotInEmployment.ValidationGroup = custProjectsAssigned.ValidationGroup = custBeforeJuly.ValidationGroup = custMoreThan18Mo.ValidationGroup = custLessThan18Mo.ValidationGroup = compBadgeEndLess.ValidationGroup = compBadgeEnd.ValidationGroup = reqBadgeEnd.ValidationGroup = reqBadgeStart.ValidationGroup = compBadgeStart.ValidationGroup = reqPreviousAlias.ValidationGroup = reqLastBadgeStart.ValidationGroup = cvLastBadgeStart.ValidationGroup = cvLastbadgeEnd.ValidationGroup = custNotFuture.ValidationGroup = cvLastBadgeRange.ValidationGroup = reqLastbadgeEnd.ValidationGroup = cvBlockStart.ValidationGroup = reqBlockStart.ValidationGroup = dtpLastBadgeStart.ValidationGroup = dtpLastBadgeEnd.ValidationGroup = dtpBlockStart.ValidationGroup =
            custPersonInProject.ValidationGroup = custBlockDatesOverlappedException.ValidationGroup = custDeactivateDateIn18moDates.ValidationGroup = custDeactivateNeed18moDates.ValidationGroup = compDeactivatedDate.ValidationGroup = custDeactivateDatePriorProjectDates.ValidationGroup = custDeactivateWithinProject.ValidationGroup =
            custExceptionDatesOverlappsBlock.ValidationGroup = custExceptionMorethan18.ValidationGroup = custExceptionNotMoreThan18moEndDate.ValidationGroup = dtpExceptionStart.ValidationGroup = custExceptionInEmpHistory.ValidationGroup =custAftertheBreak.ValidationGroup=
            custBlockDatesInEmpHistory.ValidationGroup = cvExceptionStartAfterJuly.ValidationGroup = cvExceptionStart.ValidationGroup = reqExceptionStart.ValidationGroup = dtpBlockEnd.ValidationGroup = custBlockStartAfterJuly.ValidationGroup = cvBlockEnd.ValidationGroup = cvBlockDateRange.ValidationGroup = reqBlockEnd.ValidationGroup = dtpExceptionEnd.ValidationGroup = cvExceptionDateRanges.ValidationGroup = cvExceptionEnd.ValidationGroup = reqExceptionEnd.ValidationGroup = ValidationGroup;
        }

        public void ValidateMSBadgeDetails()
        {
            if (Page.IsValid)
            {
                compBadgeStart.Validate();
                compBadgeEnd.Validate();
                reqBadgeStart.Validate();
                reqBadgeEnd.Validate();
                custAftertheBreak.Validate();
                cust18moNotInEmployment.Validate();
                custProjectsAssigned.Validate();
                custBeforeJuly.Validate();
                custMoreThan18Mo.Validate();
                custLessThan18Mo.Validate();
                compBadgeEndLess.Validate();
                compDeactivatedDate.Validate();
                custDeactivateNeed18moDates.Validate();
                custDeactivateDateIn18moDates.Validate();
                custDeactivateDatePriorProjectDates.Validate();
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
                custBlockStartAfterJuly.Validate();
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
                if (Page.IsValid)
                {
                    custDeactivateWithinProject.Validate();
                    HostingPage.IsErrorPanelDisplay = false;
                }
            }
        }

        protected void lnkHistory_Click(object sender, EventArgs e)
        {
            mpeBadgeHistoryPanel.Show();
        }

        protected void dpDeactivatedDate_Change(object sender, EventArgs e)
        {
            var deactivateDate = new DateTime();
            if (DateTime.TryParse(dpDeactivatedDate.TextValue, out deactivateDate))
            {
                var organicStart = deactivateDate.AddDays(1);
                txtOrganicStart.Text = organicStart.ToString("MM/dd/yyyy");
                txtOrganicEnd.Text = organicStart.AddMonths(6).AddDays(-1).ToString("MM/dd/yyyy");
            }
            else
            {
                txtOrganicEnd.Text = txtOrganicStart.Text = string.Empty;
            }
        }

        protected void dpBadgeStart_Changed(object sender, EventArgs e)
        {
            var startdate = new DateTime();
            compBadgeStart.Validate();
            compBadgeEnd.Validate();
            if (compBadgeStart.IsValid && compBadgeEnd.IsValid)
            {
                custBeforeJuly.Validate();
                if (custBeforeJuly.IsValid)
                {
                    if (DateTime.TryParse(dpBadgeStart.TextValue, out startdate))
                    {
                        if (BadgeDetails != null && BadgeDetails.BadgeStartDate.HasValue && BadgeDetails.BadgeStartDate.Value.Date == startdate.Date)
                        {
                            dpBadgeEnd.TextValue = BadgeDetails.BadgeEndDate.HasValue ? BadgeDetails.BadgeEndDate.Value.ToShortDateString() : string.Empty;
                            txtBreakStart.Text = BadgeDetails.BreakStartDate.HasValue ? BadgeDetails.BreakStartDate.Value.ToShortDateString() : string.Empty;
                            txtBreakEnd.Text = BadgeDetails.BreakEndDate.HasValue ? BadgeDetails.BreakEndDate.Value.ToShortDateString() : string.Empty;
                            lblBadgeStartDateSource.Text = string.IsNullOrEmpty(BadgeDetails.BadgeStartDateSource) ? "Available Now" : BadgeDetails.BadgeStartDateSource;
                            lblBadgeEndDateSource.Text = string.IsNullOrEmpty(BadgeDetails.BadgeEndDateSource) ? "Available Now" : BadgeDetails.BadgeEndDateSource;
                            lblPlannedDateSource.Text = string.IsNullOrEmpty(BadgeDetails.PlannedEndDateSource) ? "Available Now" : BadgeDetails.PlannedEndDateSource;
                        }
                        else
                        {
                            dpBadgeEnd.TextValue = startdate.AddMonths(18).AddDays(-1).ToString("MM/dd/yyyy");
                            var breakstart = startdate.AddMonths(18);
                            txtBreakStart.Text = breakstart.ToString("MM/dd/yyyy");
                            txtBreakEnd.Text = breakstart.AddMonths(6).AddDays(-1).ToString("MM/dd/yyyy");
                            lblBadgeStartDateSource.Text = "Manual Entry";
                            lblBadgeEndDateSource.Text = "Manual Entry";
                        }
                    }
                    else
                    {
                        dpBadgeEnd.TextValue = txtBreakStart.Text = txtBreakEnd.Text = txtPlannedEnd.Text = string.Empty;
                        lblBadgeStartDateSource.Text = lblBadgeEndDateSource.Text = lblPlannedDateSource.Text = "Available Now";
                    }
                }
                else
                {
                    HostingPage.IsErrorPanelDisplay = true;
                }
            }
        }

        protected void dpBadgeEnd_Changed(object sender, EventArgs e)
        {
            var enddate = new DateTime();
            custBeforeJuly.Validate();
            if (custBeforeJuly.IsValid)
            {
                if (DateTime.TryParse(dpBadgeEnd.TextValue, out enddate))
                {
                    if (BadgeDetails != null && BadgeDetails.BadgeEndDate.HasValue && BadgeDetails.BadgeEndDate.Value.Date == enddate.Date)
                    {
                        txtBreakStart.Text = BadgeDetails.BreakStartDate.HasValue ? BadgeDetails.BreakStartDate.Value.ToShortDateString() : string.Empty;
                        txtBreakEnd.Text = BadgeDetails.BreakEndDate.HasValue ? BadgeDetails.BreakEndDate.Value.ToShortDateString() : string.Empty;
                        lblBadgeEndDateSource.Text = string.IsNullOrEmpty(BadgeDetails.BadgeEndDateSource) ? "Available Now" : BadgeDetails.BadgeEndDateSource;
                    }
                    else
                    {
                        var breakStart = enddate.AddDays(1);
                        txtBreakStart.Text = breakStart.ToString("MM/dd/yyyy");
                        txtBreakEnd.Text = breakStart.AddMonths(6).AddDays(-1).ToString("MM/dd/yyyy");
                        lblBadgeEndDateSource.Text = "Manual Entry";
                        lblBadgeStartDateSource.Text = "Manual Entry";
                    }
                    reqBadgeStart.Enabled = true;
                }
                else
                {
                    dpBadgeStart.TextValue = txtBreakStart.Text = txtBreakEnd.Text = string.Empty;
                    lblBadgeStartDateSource.Text = lblBadgeEndDateSource.Text = "Available Now";
                    reqBadgeStart.Enabled = false;
                }
            }
            else
            {
                HostingPage.IsErrorPanelDisplay = true;
            }
        }
    }
}


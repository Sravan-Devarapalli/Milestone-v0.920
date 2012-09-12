using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using DataTransferObjects.Utils;
using PraticeManagement.Configuration;
using PraticeManagement.Controls;
using PraticeManagement.Events;
using PraticeManagement.PersonService;
using Resources;
using PraticeManagement.Security;
using PraticeManagement.OpportunityService;
using DataTransferObjects.ContextObjects;
using PraticeManagement.Utils;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.IO;
using System.Threading;

namespace PraticeManagement
{
    public partial class PersonDetail : PracticeManagementPersonDetailPageBase, IPostBackEventHandler
    {

        #region Constants

        private const string PersonStatusKey = "PersonStatus";
        private const string UserNameParameterName = "userName";
        private const string DuplicatePersonName = "There is another Person with the same First Name and Last Name.";
        private const string DuplicateEmail = "There is another Person with the same Email.";
        private const string lblTimeEntriesExistFormat = "There are time entries submitted by person after {0}.";
        private const string lblProjectMilestomesExistFormat = "{0} is assigned to below Project - Milesone(s) after {1}:";
        private const string lblTerminationDateErrorFormat = "Unable to set Termination Date for {0} due to the following:";
        private const string lblOwnerProjectsExistFormat = "{0} is designated as the Owner for the following project(s):";
        private const string lblOwnerOpportunitiesFormat = "{0} is designated as the Owner for the following Opportunities:";
        private const string StartDateIncorrect = "The Start Date is incorrect. There are several other compensation records for the specified period. Please edit them first.";
        private const string EndDateIncorrect = "The End Date is incorrect. There are several other compensation records for the specified period. Please edit them first.";
        private const string PeriodIncorrect = "The period is incorrect. There records falls into the period specified in an existing record.";
        private const string HireDateInCorrect = "Person cannot have the compensation for the days before his hire date.";
        private const string TerminationReasonFirstItem = "- - Select Termination Reason - -";
        private const string CloseAnActiveCompensation = "This person still has an active compensation record. Click OK to close their compensation record as of their termination date, or click Cancel to exit without saving changes.";
        private const string CloseAnOpenEndedCompensation = "This person still has an open compensation record. Click OK to close their compensation record as of their termination date, or click Cancel to exit without saving changes.";
        private const string HireDateChangeMessage = "This person has compensation record(s) before/after the new hire date. Click OK to adjust the compensation record to reflect the new hire date, or click Cancel to exit without saving changes.";
        private const string CancelTerminationMessage = "Following are the list of projects in which {0} resource's end date(s)  were set to his/her previous termination date automatically. Please reset the end dates for {0} in the below listed 'Projects-Milestones' if applicable.";
        private const string displayNone = "displayNone";
        private const string SalaryToContractException = "Salary Type to Contract Type Violation";

        #endregion

        #region Fields

        private ExceptionDetail _internalException;
        private int _saveCode;
        private bool? _userIsAdministratorValue;
        private bool? _userIsHRValue;

        #endregion Fields

        #region Properties

        private String ExMessage { get; set; }

        public PersonStatusType? PersonStatusId
        {
            get
            {
                PersonStatusType? result;
                if (IsStatusChangeClicked && PopupStatus.HasValue)
                {
                    result = PopupStatus;
                }
                else
                {
                    result = !string.IsNullOrEmpty(ddlPersonStatus.SelectedValue)
                           ?
                               (PersonStatusType?)int.Parse(ddlPersonStatus.SelectedValue)
                           : null;
                }

                return result;
            }
            set
            {
                ddlPersonStatus.SelectedIndex =
                    ddlPersonStatus.Items.IndexOf(
                        ddlPersonStatus.Items.FindByValue(value.HasValue ? ((int)value.Value).ToString() : string.Empty));
            }
        }

        private PersonStatusType? PopupStatus
        {
            get
            {
                if ((PrevPersonStatusId == (int)PersonStatusType.Active || PrevPersonStatusId == (int)PersonStatusType.Contingent) && rbnTerminate.Checked)
                {
                    //Employee with terminated / termination Pending.
                    return dtpPopUpTerminateDate.DateValue.Date >= DateTime.Now.Date ? (PrevPersonStatusId == (int)PersonStatusType.Active ? PersonStatusType.TerminationPending : PersonStatusType.Contingent) : PersonStatusType.Terminated;
                }
                else if ((PrevPersonStatusId == (int)PersonStatusType.TerminationPending && rbnCancleTermination.Checked) || ((PrevPersonStatusId == (int)PersonStatusType.Contingent || PrevPersonStatusId == (int)PersonStatusType.Terminated) && rbnActive.Checked))
                {
                    //Cancel Termination.
                    //Active employee with the selected hire date.
                    //Active employee with new Hire Date. with new compensation record.
                    return PersonStatusType.Active;
                }
                else if (PrevPersonStatusId == (int)PersonStatusType.Terminated && rbnContingent.Checked)
                {
                    //system automatically opens new compensation record.  
                    return PersonStatusType.Contingent;
                }
                else
                {
                    return null;
                }
            }
        }

        private int? PersonId
        {
            get
            {
                if (SelectedId.HasValue)
                {
                    return SelectedId;
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.hdnPersonId.Value))
                    {
                        int projectid;
                        if (Int32.TryParse(this.hdnPersonId.Value, out projectid))
                        {
                            return projectid;
                        }
                    }
                    return null;
                }
            }
            set
            {
                this.hdnPersonId.Value = value.ToString();
            }
        }

        /// <summary>
        /// Gets whether the current user is in the Administrator role.
        /// </summary>
        protected bool UserIsAdministrator
        {
            get
            {
                if (!_userIsAdministratorValue.HasValue)
                {
                    _userIsAdministratorValue =
                        Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName);
                }

                return _userIsAdministratorValue.Value;
            }
        }

        protected bool UserIsHR
        {
            get
            {
                if (!_userIsHRValue.HasValue)
                {
                    _userIsHRValue =
                        Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.HRRoleName);
                }

                return _userIsHRValue.Value;
            }
        }

        public bool UserIsRecruiter
        {
            get { return (bool)ViewState["UserIsRecruiter"]; }
            set { ViewState["UserIsRecruiter"] = value; }
        }

        private Pay PayFooter
        {
            get
            {
                return ViewState["PayFooter"] as Pay;
            }
            set
            {
                ViewState["PayFooter"] = value;
            }
        }

        private List<Pay> PayHistory
        {
            get
            {
                return ViewState["PAY_HISTORY"] as List<Pay>;
            }
            set
            {
                ViewState["PAY_HISTORY"] = value;
            }
        }

        /// <summary>
        /// Gets or sets original person Status ID, need to store it for comparing with new one
        /// </summary>
        public int PrevPersonStatusId
        {
            get
            {
                if (ViewState[PersonStatusKey] == null)
                {
                    ViewState[PersonStatusKey] = -1;
                }
                return (int)ViewState[PersonStatusKey];
            }
            set { ViewState[PersonStatusKey] = value; }
        }

        private DateTime? PreviousHireDate
        {
            get
            {
                return (DateTime?)ViewState["ViewState_PreviousHireDate"];
            }
            set
            {
                ViewState["ViewState_PreviousHireDate"] = value;
            }
        }

        private DateTime? PreviousTerminationDate
        {
            get
            {
                return (DateTime?)ViewState["ViewState_PrevTerminationDate"];
            }
            set
            {
                ViewState["ViewState_PrevTerminationDate"] = value;
            }
        }

        private string PreviousTerminationReasonId
        {
            get
            {
                return (string)ViewState["ViewState_PreviousTerminationReason"];
            }
            set
            {
                ViewState["ViewState_PreviousTerminationReason"] = value;
            }
        }

        private DateTime? TerminationDateBeforeCurrentHireDate
        {
            get
            {
                return (DateTime?)ViewState["ViewState_TerminationDateBeforeCurrentHireDate"];
            }
            set
            {
                ViewState["ViewState_TerminationDateBeforeCurrentHireDate"] = value;
            }
        }

        public override Person PersonUnsavedData { get; set; }

        public override string LoginPageUrl { get; set; }

        public override PersonPermission Permissions { get; set; }

        public bool IsStatusChangeClicked
        {
            get
            {
                return (bool)ViewState["ViewState_IsStatusChangeClicked"];
            }
            set
            {
                ViewState["ViewState_IsStatusChangeClicked"] = value;
            }
        }

        public DateTime? HireDate
        {
            get
            {
                if (IsStatusChangeClicked && PersonStatusId == PersonStatusType.Active && PrevPersonStatusId != (int)PersonStatusType.TerminationPending)
                {
                    return GetDate(dtpActiveHireDate.DateValue);
                }
                else if (IsStatusChangeClicked && PersonStatusId == PersonStatusType.Contingent && PrevPersonStatusId != (int)PersonStatusType.Contingent)
                {
                    return GetDate(dtpContingentHireDate.DateValue);
                }
                else
                {
                    return GetDate(dtpHireDate.DateValue);
                }
            }
        }

        public DateTime? TerminationDate
        {
            get
            {
                return IsStatusChangeClicked ? ((PopupStatus.Value == PersonStatusType.Terminated || PopupStatus.Value == PersonStatusType.TerminationPending || (PrevPersonStatusId == (int)PersonStatusType.Contingent && PopupStatus.Value == PersonStatusType.Contingent)) ? GetDate(dtpPopUpTerminateDate.DateValue) : null) : GetDate(dtpTerminationDate.DateValue);
            }
        }

        private DateTime? GetDate(DateTime dateTime)
        {
            return dateTime != DateTime.MinValue ? (DateTime?)dateTime : null;
        }

        public string TerminationReasonId
        {
            get
            {
                return IsStatusChangeClicked ? ((PopupStatus.Value == PersonStatusType.Terminated || PopupStatus.Value == PersonStatusType.TerminationPending || (PrevPersonStatusId == (int)PersonStatusType.Contingent && PopupStatus.Value == PersonStatusType.Contingent)) ? ddlPopUpTerminationReason.SelectedValue : string.Empty) : ddlTerminationReason.SelectedValue;
            }
        }

        private bool IsLockOut
        {
            get
            {
                return IsStatusChangeClicked && (PopupStatus == PersonStatusType.Active || PopupStatus == PersonStatusType.Contingent) && PrevPersonStatusId == (int)PersonStatusType.Terminated ? false : chbLockedOut.Checked;
            }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            mlConfirmation.ClearMessage();
            if (!IsPostBack)
            {
                DataHelper.FillPracticeListOnlyActive(ddlDefaultPractice, string.Empty);
                DataHelper.FillPersonDivisionList(ddlDivision);
                txtFirstName.Focus();
                UserIsRecruiter = Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.RecruiterRoleName);
                AddTriggersToUpdatePanel(false);
            }

            AllowContinueWithoutSave = cellActivityLog.Visible = PersonId.HasValue;
            personOpportunities.TargetPersonId = PersonId;
            mlError.ClearMessage();
            this.dvTerminationDateErrors.Visible = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Security
            chblRoles.Visible =
                btnResetPassword.Visible = locRolesLabel.Visible = chbLockedOut.Visible = UserIsAdministrator || UserIsHR;//#2817 UserisHR is added as per requirement.
            txtEmployeeNumber.ReadOnly = !UserIsAdministrator && !UserIsHR;//#2817 UserisHR is added as per requirement.

            if (!UserIsAdministrator && !UserIsHR && !PersonId.HasValue)//#2817 UserisHR is added as per requirement.
            {
                // Recruiter should not be able to set the person active.
                PersonStatusId = PersonStatusType.Contingent;
            }
            recruiterInfo.ReadOnly = !UserIsAdministrator && !UserIsHR;//#2817 UserisHR is added as per requirement.
            lbPayChexID.Visible = txtPayCheckId.Visible = UserIsAdministrator;
            btnAddDefaultRecruitingCommission.Enabled = UserIsAdministrator || UserIsRecruiter || UserIsHR;//#2817 UserisHR is added as per requirement.
            cellPermissions.Visible = UserIsAdministrator || UserIsHR;//#2817 UserisHR is added as per requirement.

            if (PrevPersonStatusId == (int)PersonStatusType.Active || PrevPersonStatusId == (int)PersonStatusType.Terminated || (PrevPersonStatusId == (int)PersonStatusType.Contingent && dtpTerminationDate.DateValue == DateTime.MinValue))
            {
                if (PrevPersonStatusId != (int)PersonStatusType.Terminated)
                {
                    //FillTerminationReasonsByTerminationDate(null, ddlTerminationReason);
                    ddlTerminationReason.Visible = false;
                    txtTerminationReason.Visible = true;
                }
                else
                {
                    ddlTerminationReason.Enabled = false;
                    ddlTerminationReason.Visible = true;
                    txtTerminationReason.Visible = false;
                }
                dtpTerminationDate.EnabledTextBox = false;
                dtpTerminationDate.ReadOnly = true;
            }
            else if (PrevPersonStatusId == (int)PersonStatusType.TerminationPending || (PrevPersonStatusId == (int)PersonStatusType.Contingent && dtpTerminationDate.DateValue != DateTime.MinValue))
            {
                dtpTerminationDate.EnabledTextBox = true;
                dtpTerminationDate.ReadOnly = false;

                ddlTerminationReason.Visible = true;
                txtTerminationReason.Visible = false;
            }

            if (PrevPersonStatusId == (int)PersonStatusType.Terminated)
            {
                dtpHireDate.ReadOnly = true;
                dtpHireDate.EnabledTextBox = false;
            }
            ddlPersonStatus.Visible = !(lblPersonStatus.Visible = btnChangeEmployeeStatus.Visible = PersonId.HasValue);

        }

        private void LoadChangeEmployeeStatusPopUpData()
        {
            if (PrevPersonStatusId == (int)PersonStatusType.Active)
            {
                rbnCancleTermination.CssClass =
                rbnActive.CssClass =
                divActive.Attributes["class"] =
                rbnContingent.CssClass =
                divContingent.Attributes["class"] = displayNone;

                dtpPopUpTerminateDate.DateValue = DateTime.Now.Date;
                rbnTerminate.CssClass = "";
                rbnActive.Checked = rbnCancleTermination.Checked = rbnContingent.Checked = !(rbnTerminate.Checked = true);
                divTerminate.Attributes["class"] = "padLeft25 PaddingTop6";
            }
            else if (PrevPersonStatusId == (int)PersonStatusType.TerminationPending)
            {
                rbnCancleTermination.CssClass = "";
                rbnActive.Checked = rbnTerminate.Checked = rbnContingent.Checked = rbnCancleTermination.Checked = false;

                rbnActive.CssClass =
                rbnTerminate.CssClass =
                rbnContingent.CssClass =
                divActive.Attributes["class"] =
                divTerminate.Attributes["class"] =
                divContingent.Attributes["class"] = displayNone;
            }
            else if (PrevPersonStatusId == (int)PersonStatusType.Contingent)
            {
                dtpActiveHireDate.DateValue = dtpHireDate.DateValue;
                dtpPopUpTerminateDate.DateValue = DateTime.Now.Date;

                rbnActive.CssClass = rbnTerminate.CssClass = "";
                rbnCancleTermination.CssClass =
                rbnContingent.CssClass =
                divActive.Attributes["class"] =
                divContingent.Attributes["class"] =
                divTerminate.Attributes["class"] = displayNone;
                rbnActive.Checked = rbnCancleTermination.Checked = rbnContingent.Checked = rbnTerminate.Checked = false;
            }
            else if (PrevPersonStatusId == (int)PersonStatusType.Terminated)
            {
                dtpActiveHireDate.DateValue = dtpContingentHireDate.DateValue = PreviousTerminationDate.Value.AddDays(1);

                rbnActive.CssClass = rbnContingent.CssClass = "";
                rbnCancleTermination.CssClass =
                divActive.Attributes["class"] =
                rbnTerminate.CssClass =
                divTerminate.Attributes["class"] =
                divContingent.Attributes["class"] = displayNone;
                rbnActive.Checked = rbnCancleTermination.Checked = rbnContingent.Checked = rbnTerminate.Checked = false;
            }

            FillTerminationReasonsByTerminationDate(dtpPopUpTerminateDate, ddlPopUpTerminationReason);
        }

        #endregion

        protected void btnEndCompensationOk_Click(object sender, EventArgs e)
        {
            cvEndCompensation.Enabled = false;
            mpeChangeStatusEndCompensation.Hide();
            DisableValidatecustTerminateDateTE = false;
            Save_Click(sender, e);
        }

        protected void btnEndCompensationCancel_Click(object source, EventArgs args)
        {
            ResetToPreviousData();
            mpeChangeStatusEndCompensation.Hide();
        }

        protected void btnHireDateChangeOk_Click(object sender, EventArgs e)
        {
            cvHireDateChange.Enabled = false;
            mpeHireDateChange.Hide();
            Save_Click(sender, e);
        }

        protected void btnHireDateChangeCancel_Click(object source, EventArgs args)
        {
            ResetToPreviousData();
            mpeHireDateChange.Hide();
        }

        protected void dtpTerminationDate_OnSelectionChanged(object sender, EventArgs e)
        {
            if (dtpTerminationDate.DateValue != DateTime.MinValue && PreviousTerminationDate.HasValue && PreviousTerminationDate.Value != GetDate(dtpTerminationDate.DateValue).Value)
            {
                custCancelTermination.Enabled = PreviousTerminationDate.Value < GetDate(dtpTerminationDate.DateValue).Value;
            }

            FillTerminationReasonsByTerminationDate((DatePicker)sender, ddlTerminationReason);
        }

        private void FillTerminationReasonsByTerminationDate(DatePicker terminationDate, ListControl ddlTerminationReasons)
        {
            var reasons = new List<TerminationReason>();
            if (terminationDate != null)
            {
                ddlTerminationReasons.SelectedIndex = 0;
                if (PrevPersonStatusId == (int)PersonStatusType.Contingent)
                {
                    reasons = SettingsHelper.GetTerminationReasonsList().Where(tr => tr.IsContigent == true).ToList();
                }
                else if (GetDate(terminationDate.DateValue).HasValue && PayHistory.Any(p => p.StartDate.Date <= terminationDate.DateValue.Date && (!p.EndDate.HasValue || p.EndDate.Value > terminationDate.DateValue.Date)))
                {
                    var pay = PayHistory.First(p => p.StartDate.Date <= terminationDate.DateValue.Date && (!p.EndDate.HasValue || p.EndDate.Value > terminationDate.DateValue.Date));
                    switch (pay.Timescale)
                    {
                        case TimescaleType.Hourly:
                            reasons = SettingsHelper.GetTerminationReasonsList().Where(tr => tr.IsW2HourlyRule == true).ToList();
                            break;
                        case TimescaleType.Salary:
                            reasons = SettingsHelper.GetTerminationReasonsList().Where(tr => tr.IsW2SalaryRule == true).ToList();
                            break;
                        case TimescaleType._1099Ctc:
                        case TimescaleType.PercRevenue:
                            reasons = SettingsHelper.GetTerminationReasonsList().Where(tr => tr.Is1099Rule == true).ToList();
                            break;
                        default:
                            break;
                    }
                }
            }

            DataHelper.FillTerminationReasonsList(ddlTerminationReasons, TerminationReasonFirstItem, reasons.ToArray());
        }

        protected void dtpPopUpTerminationDate_OnSelectionChanged(object sender, EventArgs e)
        {
            FillTerminationReasonsByTerminationDate((DatePicker)sender, ddlPopUpTerminationReason);
            divTerminate.Attributes["class"] = "padLeft25 PaddingTop6";
            mpeViewPersonChangeStatus.Show();
        }

        protected void btnChangeEmployeeStatus_Click(object sender, EventArgs e)
        {
            LoadChangeEmployeeStatusPopUpData();
            mpeViewPersonChangeStatus.Show();
        }

        protected void btnCancelChangePersonStatus_Click(object source, EventArgs args)
        {
            IsStatusChangeClicked = false;
            ResetToPreviousData();
            mpeViewPersonChangeStatus.Hide();
        }

        private void ResetToPreviousData()
        {
            var person = GetPerson(PersonId);
            PopulateControls(person);
        }

        protected void btnOkChangePersonStatus_Click(object source, EventArgs args)
        {
            if (PopupStatus.HasValue)
            {
                IsStatusChangeClicked = true;
                DisableValidatecustTerminateDateTE = false;
                custCompensationCoversMilestone.Enabled = false;
                cvEndCompensation.Enabled = true;
                cvHireDateChange.Enabled = true;
                custCancelTermination.Enabled = false;

                var popupStatus = PopupStatus.Value == PersonStatusType.Contingent && PrevPersonStatusId == (int)PersonStatusType.Contingent ? PersonStatusType.TerminationPending : PopupStatus.Value;

                switch (popupStatus)
                {
                    case PersonStatusType.TerminationPending:
                    case PersonStatusType.Terminated:
                        Page.Validate(valSummaryChangePersonStatusToTerminate.ValidationGroup);
                        if (!Page.IsValid)
                        {
                            divTerminate.Attributes["class"] = "padLeft25 PaddingTop6";
                            divActive.Attributes["class"] = "displayNone";
                            divContingent.Attributes["class"] = "displayNone";
                            mpeViewPersonChangeStatus.Show();
                        }
                        else
                        {
                            FillTerminationReasonsByTerminationDate(dtpPopUpTerminateDate, ddlTerminationReason);
                        }
                        break;
                    case PersonStatusType.Contingent:
                        //change employee status to contingent.
                        Page.Validate(valSummaryChangePersonStatusToContingent.ValidationGroup);
                        if (!Page.IsValid)
                        {
                            divContingent.Attributes["class"] = "padLeft25 PaddingTop6";
                            divActive.Attributes["class"] = "displayNone";
                            divTerminate.Attributes["class"] = "displayNone";
                            mpeViewPersonChangeStatus.Show();
                        }
                        break;
                    case PersonStatusType.Active:
                        if (rbnCancleTermination.Checked)
                        {
                            custCancelTermination.Enabled = true;
                            dtpPopUpTerminateDate.TextValue = string.Empty;
                            ddlPopUpTerminationReason.SelectedIndex = 0;
                        }
                        else
                        {
                            //change employee status to active.
                            Page.Validate(valSummaryChangePersonStatusToActive.ValidationGroup);
                            if (!Page.IsValid)
                            {
                                divActive.Attributes["class"] = "padLeft25 PaddingTop6";
                                divTerminate.Attributes["class"] = "displayNone";
                                divContingent.Attributes["class"] = "displayNone";

                                mpeViewPersonChangeStatus.Show();
                            }
                        }
                        break;
                    default:
                        IsStatusChangeClicked = false;
                        break;

                }
                if (Page.IsValid && IsStatusChangeClicked)
                {
                    lblPersonStatus.Text = PopupStatus.Value.ToString();
                    dtpHireDate.DateValue = HireDate.Value;
                    dtpTerminationDate.TextValue = TerminationDate.HasValue? TerminationDate.Value.ToShortDateString() : string.Empty;
                    ddlTerminationReason.SelectedValue = TerminationReasonId;
                    AddTriggersToUpdatePanel(IsStatusChangeClicked);
                    Save_Click(source, args);
                }
            }
            else
            {
                mpeViewPersonChangeStatus.Show();
            }
        }

        protected void recruiterInfo_InfoChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        protected void lnkSaveReport_OnClick(object sender, EventArgs e)
        {
            string html = hdnSaveReportText.Value;
            HTMLToPdf(html, "Error");
        }

        protected void lnkSaveReportCancelTermination_OnClick(object sender, EventArgs e)
        {
            string html = hdnSaveReportTextCancelTermination.Value;
            HTMLToPdf(html, "Information");
        }

        public void HTMLToPdf(String HTML, string filename)
        {
            var document = new iTextSharp.text.Document();
            iTextSharp.text.pdf.PdfWriter.GetInstance(document, new FileStream(Request.PhysicalApplicationPath + @"\" + filename + ".pdf", FileMode.Create));

            document.Open();
            var styles = new iTextSharp.text.html.simpleparser.StyleSheet();
            var hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);
            hw.Parse(new StringReader(HTML));
            document.Close();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "Application/pdf";
            HttpContext.Current.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}", filename + ".pdf"));
            HttpContext.Current.Response.WriteFile(Request.PhysicalApplicationPath + @"\" + filename + ".pdf");
        }

        private void AddTriggersToUpdatePanel(bool addPostBackTrigger)
        {
            //if (upnlBody.Triggers.Count > 3)
            //{
            //    upnlBody.Triggers.RemoveAt(3);
            //    upnlBody.Triggers.RemoveAt(3);
            //    upnlBody.Triggers.RemoveAt(3);
            //}
            //if (addPostBackTrigger)
            //{
            //    var trBtnsave = new PostBackTrigger { ControlID = "btnSave" };
            //    var trBntEndCompensationOk = new PostBackTrigger { ControlID = "bntEndCompensationOk" };
            //    var trBtnTerminationProcessOk = new PostBackTrigger { ControlID = "btnTerminationProcessOK" };
            //    upnlBody.Triggers.Add(trBtnsave);
            //    upnlBody.Triggers.Add(trBntEndCompensationOk);
            //    upnlBody.Triggers.Add(trBtnTerminationProcessOk);
            //}
            //else
            //{
            //    var trBtnsave = new AsyncPostBackTrigger { ControlID = "btnSave", EventName = "click" };
            //    var trBntEndCompensationOk = new AsyncPostBackTrigger { ControlID = "bntEndCompensationOk", EventName = "click" };
            //    var trBtnTerminationProcessOk = new AsyncPostBackTrigger { ControlID = "btnTerminationProcessOK", EventName = "click" };
            //    upnlBody.Triggers.Add(trBtnsave);
            //    upnlBody.Triggers.Add(trBntEndCompensationOk);
            //    upnlBody.Triggers.Add(trBtnTerminationProcessOk);
            //}
        }

        /// <summary>
        /// Saves the user's input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            var updatePersonStatusDropdown = true;
            custCompensationCoversMilestone.Enabled = cvEndCompensation.Enabled = cvHireDateChange.Enabled = !IsStatusChangeClicked;
            custCancelTermination.Enabled = false;
            if (PreviousTerminationDate.HasValue && dtpTerminationDate.DateValue != DateTime.MinValue && PreviousTerminationDate.Value != GetDate(dtpTerminationDate.DateValue).Value)
            {
                custCancelTermination.Enabled = true;
            }
            if (PersonId.HasValue)
            {
                updatePersonStatusDropdown = false;
                PersonStatusId = (PersonStatusType)PrevPersonStatusId;
            }
            Save_Click(sender, e);
            if (PersonId.HasValue && updatePersonStatusDropdown)
            {
                DataHelper.FillPersonStatusList(ddlPersonStatus);
            }
        }

        protected void btnCancleTerminationOKButton_OnClick(object source, EventArgs args)
        {
            custCancelTermination.Enabled = false;
            dtpPopUpTerminateDate.TextValue = string.Empty;
            ddlPopUpTerminationReason.SelectedIndex = 0;
            Save_Click(source, args);
            mpeCancelTermination.Hide();
        }

        protected void btnCancelTerminationCancelButton_OnClick(object sender, EventArgs e)
        {
            ResetToPreviousData();
            mpeCancelTermination.Hide();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            int viewindex = mvPerson.ActiveViewIndex;
            TableCell CssSelectCell = null;
            foreach (TableCell item in tblPersonViewSwitch.Rows[0].Cells)
            {
                if (!string.IsNullOrEmpty(item.CssClass))
                {
                    CssSelectCell = item;
                }
            }

            if (ValidateAndSave() && Page.IsValid)
            {
                ClearDirty();
                mlError.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Person"));
            }
            if (!string.IsNullOrEmpty(ExMessage) || Page.IsValid)
            {
                mvPerson.ActiveViewIndex = viewindex;
                SetCssClassEmpty();
                CssSelectCell.CssClass = "SelectedSwitch";
            }
            if (Page.IsValid && PersonId.HasValue)
            {
                var person = GetPerson(PersonId);
                if (person != null)
                {
                    gvCompensationHistory.EditIndex = -1;
                    lblEmployeeNumber.Visible = true;
                    txtEmployeeNumber.Visible = true;
                    PayHistory = person.PaymentHistory;
                    PopulateControls(person);
                }
            }
        }

        protected override bool ValidateAndSave()
        {
            return ValidateAndSavePersonDetails();
        }

        private void ValidatePage()
        {
            custTerminateDateTE.Enabled = false;
            int activeindex = mvPerson.ActiveViewIndex;
            for (int i = 0, j = mvPerson.ActiveViewIndex; i < mvPerson.Views.Count; i++, j++)
            {
                if (j == mvPerson.Views.Count)
                {
                    j = 0;
                }
                SelectView(rowSwitcher.Cells[j].Controls[0], j, true);
                Page.Validate(valsPerson.ValidationGroup);
                if (!Page.IsValid)
                {
                    break;
                }
            }

            if (cvEndCompensation.Enabled && Page.IsValid)
            {
                //Page.Validate("EndCompensation");
                cvEndCompensation.Validate();
                SelectView(rowSwitcher.Cells[activeindex].Controls[0], activeindex, true);
            }

            if (cvHireDateChange.Enabled && Page.IsValid)
            {
                cvHireDateChange.Validate();
                SelectView(rowSwitcher.Cells[activeindex].Controls[0], activeindex, true);
            }

            if (!DisableValidatecustTerminateDateTE && Page.IsValid)
            {
                custTerminateDateTE.Enabled = true;
                Page.Validate(valsPerson.ValidationGroup);
                SelectView(rowSwitcher.Cells[activeindex].Controls[0], activeindex, true);
            }
            if (custCancelTermination.Enabled && Page.IsValid)
            {
                //Page.Validate("EndCompensation");
                custCancelTermination.Validate();
                SelectView(rowSwitcher.Cells[activeindex].Controls[0], activeindex, true);
            }
        }

        public bool ValidateAndSavePersonDetails()
        {
            ValidatePage();
            if (Page.IsValid)
            {
                int? personId = SaveData();
                if (personId.HasValue)
                {
                    PersonId = personId;
                    ClearDirty();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Switches the MultiView with the <see cref="Person"/> details.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);

            if (SaveDirty && (mvPerson.Views[viewIndex] == vwRates || mvPerson.Views[viewIndex] == vwWhatIf) &&
                !ValidateAndSave())
            {
                return;
            }

            SelectView((Control)sender, viewIndex, false);

            if (viewIndex == 9) //History
            {
                activityLog.Update();
            }

            if (viewIndex == 6) //Opportunities
            {
                personOpportunities.DatabindOpportunities();
            }
        }

        /// <summary>
        /// Redirects to Compensation Details page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnStartDate_Command(object sender, CommandEventArgs e)
        {
            if (!SaveDirty || (ValidateAndSave() && Page.IsValid))
            {
                Redirect(
                    string.Format(Constants.ApplicationPages.RedirectStartDateFormat,
                                  Constants.ApplicationPages.CompensationDetail,
                                  PersonId,
                                  HttpUtility.UrlEncode((string)e.CommandArgument)));
            }
        }

        protected void btnAddCompensation_Click(object sender, EventArgs e)
        {
            if (!PersonId.HasValue)
            {
                // Save a New Record
                ValidatePage();
                if (Page.IsValid)
                {
                    int? personId = SaveData();
                    if (Page.IsValid)
                    {
                        Redirect(string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                               Constants.ApplicationPages.CompensationDetail,
                                               personId),
                                 personId.ToString());
                    }
                }
            }
            else if (!SaveDirty || (ValidateAndSave() && Page.IsValid))
            {
                Redirect(
                    string.Format(Constants.ApplicationPages.DetailRedirectFormat,
                                  Constants.ApplicationPages.CompensationDetail,
                                  PersonId), PersonId.Value.ToString());
            }
        }

        protected void btnRecruitingCommissionStartDate_Command(object sender, CommandEventArgs e)
        {
            if (!SaveDirty || (ValidateAndSave() && Page.IsValid))
            {
                Redirect(
                    string.Format(Constants.ApplicationPages.RedirectPersonIdFormat,
                                  Constants.ApplicationPages.DefaultRecruitingCommissionDetail,
                                  e.CommandArgument,
                                  PersonId), PersonId.Value.ToString());
            }
        }

        protected void btnAddDefaultRecruitingCommission_Click(object sender, EventArgs e)
        {
            if (!PersonId.HasValue)
            {
                // Save a New Record
                ValidatePage();
                if (Page.IsValid)
                {
                    int? personId = SaveData();
                    if (Page.IsValid)
                    {
                        Redirect(string.Format(Constants.ApplicationPages.RedirectPersonIdFormat,
                                               Constants.ApplicationPages.DefaultRecruitingCommissionDetail,
                                               string.Empty,
                                               personId),
                                 personId.ToString());
                    }
                }
            }
            else if (!SaveDirty || (ValidateAndSave() && Page.IsValid))
            {
                Redirect(
                    string.Format(Constants.ApplicationPages.RedirectPersonIdFormat,
                                  Constants.ApplicationPages.DefaultRecruitingCommissionDetail,
                                  string.Empty,
                                  PersonId), PersonId.Value.ToString());
            }
        }

        protected void btnUnlock_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmailAddress.Text) && !IsDirty)
            {
                MembershipUser user = Membership.GetUser(txtEmailAddress.Text);

                if (user != null)
                {
                    user.UnlockUser();
                }
            }
        }

        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmailAddress.Text) && !IsDirty)
            {
                MembershipUser user = Membership.GetUser(txtEmailAddress.Text);

                if (user != null)
                {
                    user.ResetPassword();
                    btnResetPassword.Visible = false;
                    lblPaswordResetted.Visible = true;
                    chbLockedOut.Checked = user.IsLockedOut;
                }
            }
        }

        private void SetCssClassEmpty()
        {
            foreach (TableCell cell in tblPersonViewSwitch.Rows[0].Cells)
            {
                cell.CssClass = string.Empty;
            }
        }

        private void SelectView(Control sender, int viewIndex, bool selectOnly)
        {
            mvPerson.ActiveViewIndex = viewIndex;

            SetCssClassEmpty();

            ((WebControl)sender.Parent).CssClass = "SelectedSwitch";

            if (mvPerson.GetActiveView() == vwWhatIf && !selectOnly)
            {
                // Recalculation the rates
                DisplayCalculatedRate();
            }
        }

        private void DisplayCalculatedRate()
        {
            if (!SaveDirty || (ValidateAndSave() && Page.IsValid))
            {
                /*
                 *  There's no need to call GetPerson, because it
                 *      calls PersonRateCalculator constructor,
                 *      which will later be called in WhatIf.DisplayCalculatorRate
                 *      
                 *      Also, WhatIf doesn't use any Person properties
                 *      except of PersonId
                 */
                //Person person = GetPerson(SelectedId);

                var fakePerson = new Person { Id = PersonId };
                whatIf.Person = fakePerson; // person;
            }
        }

        private void DisplayPersonPermissions()
        {
            ShowPermissionsPerPage();
            ShowPermissionsPerEntities();
        }

        private void ShowPermissionsPerEntities()
        {
            //  If we're editing existing user and having administrators rights
            if (PersonId.HasValue && (UserIsAdministrator || UserIsHR))//#2817 UserisHR is added as per requirement.
            {
                rpPermissions.Visible = true;
                var permissions = DataHelper.GetPermissions(new Person { Id = PersonId.Value });
                rpPermissions.ApplyPermissions(permissions);
            }
        }

        private void ShowPermissionsPerPage()
        {
            var permDiff = new List<PermissionDiffeneceItem>();

            IPrincipal userNew =
                new GenericPrincipal(new GenericIdentity(txtEmailAddress.Text), GetSelectedRoles().ToArray());
            IPrincipal userCurrent =
                new GenericPrincipal(new GenericIdentity(txtEmailAddress.Text),
                                     Roles.GetRolesForUser(txtEmailAddress.Text));

            // Retrieve and sort locations
            System.Configuration.Configuration config =
                WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
            var locationsSorted = new List<ConfigurationLocation>(config.Locations.Count);
            locationsSorted.AddRange(config.Locations.Cast<ConfigurationLocation>());

            // Evaluate and display permissions for secure pages
            foreach (var location in locationsSorted.OrderBy(location => location.Path))
            {
                var description =
                    location.OpenConfiguration().GetSection("locationDescription") as
                    LocationDescriptionConfigurationSection;
                if (description != null && !string.IsNullOrEmpty(description.Title))
                {
                    permDiff.Add(new PermissionDiffeneceItem
                    {
                        Title = description.Title,
                        Old = UrlAuthorizationModule.CheckUrlAccessForPrincipal("~/" + location.Path, userNew, "GET"),
                        New = UrlAuthorizationModule.CheckUrlAccessForPrincipal("~/" + location.Path, userCurrent, "GET")
                    });
                }
            }

            gvPermissionDiffenrece.DataSource = permDiff;
            gvPermissionDiffenrece.DataBind();
        }

        /// <summary>
        /// Retrives the data and display them.
        /// </summary>
        protected override void Display()
        {
            rpPermissions.Visible = IsStatusChangeClicked = false;
            if (PreviousPage != null && PreviousPage is CompensationDetail)
            {
                mlConfirmation.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Person"));
            }

            if (!PersonId.HasValue)
            {
                var status = new List<PersonStatus>();
                status.Add(new PersonStatus { Id = (int)PersonStatusType.Active, Name = PersonStatusType.Active.ToString() });
                status.Add(new PersonStatus { Id = (int)PersonStatusType.Contingent, Name = PersonStatusType.Contingent.ToString() });
                DataHelper.FillListDefault(ddlPersonStatus, string.Empty, status.ToArray(), true);
                PersonStatusId = PersonStatusType.Active;
                FillTerminationReasonsByTerminationDate(dtpTerminationDate, ddlTerminationReason);

                ddlPersonStatus.Visible = true;
                lblPersonStatus.Visible = false;
                btnChangeEmployeeStatus.Visible = false;
            }
            else
            {
                DataHelper.FillPersonStatusList(ddlPersonStatus);
            }
            DataHelper.FillSenioritiesList(ddlSeniority);

            chblRoles.DataSource = Roles.GetAllRoles();
            chblRoles.DataBind();

            int? id = PersonId;

            personProjects.PersonId = id;
            personProjects.UserIsAdministrator = UserIsAdministrator || UserIsHR; //#2817 UserIsHR is added as per requirement.

            Person person = null;

            if (id.HasValue) // Edit existing person mode
            {
                person = GetPerson(id);

                if (person != null)
                {
                    PayHistory = person.PaymentHistory;
                    PopulateControls(person);
                }
                else
                {
                    UpdateRecruiterList();
                }
            }
            else // Add new person mode
            {
                // Hide Employee Number related controls
                lblEmployeeNumber.Visible = false;
                txtEmployeeNumber.Visible = false;
                reqEmployeeNumber.Enabled = false;

                cellRates.Visible = cellWhatIf.Visible = false;
                btnResetPassword.Visible = false;

                UpdateRecruiterList();
            }

            UpdateSalesCommissionState();
            UpdateManagementCommissionState();

            personOpportunities.DataBind();
        }

        private static Person GetPerson(int? id)
        {
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    var person = serviceClient.GetPersonDetail(id.Value);
                    person.EmploymentHistory = serviceClient.GetPersonEmploymentHistoryById(person.Id.Value).ToList();
                    return person;
                }
                catch (FaultException<ExceptionDetail>)
                {
                    serviceClient.Abort();
                    throw;
                }
            }
        }

        protected void dtpHireDate_SelectionChanged(object sender, EventArgs e)
        {
            UpdateRecruiterList();
            IsDirty = true;
            dtpHireDate.Focus();
        }

        protected void chblRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            IsDirty = true;
            chblRoles.Focus();
        }

        #region Populate controls

        private void PopulateControls(Person person)
        {
            //  Names, email, dates etc.
            PopulateBasicData(person);

            // Default recruiter commissions
            PopulateRecruiterCommissions(person);

            // Payment history
            PopulatePayment(person);

            // Default commissions info
            PopulatePersonCommissions(person);

            // Role/Seniority
            PopulateRolesAndSeniority(person);

            // EmploymentHistory
            PopulateEmploymentHistory(person);
        }

        /// <summary>
        /// Updates a recruiter list
        /// </summary>
        private void UpdateRecruiterList()
        {
            if (!PersonId.HasValue)
            {
                var person = new Person { HireDate = dtpHireDate.DateValue };

                recruiterInfo.Person = person;

                if (!UserIsAdministrator && !UserIsHR && UserIsRecruiter)//#2817 UserisHR is added as per requirement.
                {
                    // Recruiter cannot set another resruiter for the person
                    var current = DataHelper.CurrentPerson;
                    if (current != null && current.Id.HasValue)
                    {
                        try
                        {
                            recruiterInfo.SetRecruiter(current.Id.Value);
                            btnSave.Enabled = true;
                        }
                        catch (Exception e)
                        {
                            btnSave.Enabled = false;
                            mlError.ShowErrorMessage(e.Message);
                        }
                    }
                }
            }
        }

        private void PopulateRolesAndSeniority(Person person)
        {
            if (person.Seniority != null)
            {
                ddlSeniority.SelectedIndex =
                    ddlSeniority.Items.IndexOf(ddlSeniority.Items.FindByValue(person.Seniority.Id.ToString()));
            }

            // Roles
            foreach (ListItem item in chblRoles.Items)
            {
                item.Selected = Array.IndexOf(person.RoleNames, item.Text) >= 0;
            }
        }

        private void PopulatePersonCommissions(Person person)
        {
            if (person.DefaultPersonCommissions != null)
            {
                // Sales commission
                foreach (DefaultCommission commission in person.DefaultPersonCommissions)
                {
                    if (commission.TypeOfCommission == CommissionType.Sales)
                    {
                        chbSalesCommissions.Checked = true;
                        txtSalesCommissionsGross.Text = commission.FractionOfMargin.ToString();

                        break;
                    }
                }

                // Practice management commission
                foreach (DefaultCommission commission in person.DefaultPersonCommissions)
                {
                    if (commission.TypeOfCommission == CommissionType.PracticeManagement)
                    {
                        chbManagementCommissions.Checked = true;
                        txtManagementCommission.Text = commission.FractionOfMargin.ToString();
                        rlstManagementCommission.SelectedIndex =
                            rlstManagementCommission.Items.IndexOf(rlstManagementCommission.Items.FindByValue(
                                commission.MarginTypeId.HasValue
                                    ?
                                        commission.MarginTypeId.Value.ToString()
                                    : string.Empty));

                        break;
                    }
                }
            }
        }

        private void PopulatePayment(Person person)
        {
            PopulatePayment(person.PaymentHistory);
        }

        private void PopulatePayment(List<Pay> paymentHistory)
        {
            gvCompensationHistory.DataSource = paymentHistory;
            gvCompensationHistory.DataBind();
        }

        private void PopulateEmploymentHistory(Person person)
        {
            gvEmploymentHistory.DataSource = person.EmploymentHistory;
            gvEmploymentHistory.DataBind();
        }

        private void PopulateRecruiterCommissions(Person person)
        {
            if (person.DefaultPersonRecruiterCommission != null)
            {
                gvRecruitingCommissions.DataSource = person.DefaultPersonRecruiterCommission;
                gvRecruitingCommissions.DataBind();
            }

            // Recruiter commissions for the given person
            recruiterInfo.Person = person;
        }

        private void PopulateBasicData(Person person)
        {
            txtFirstName.Text = person.FirstName;
            txtLastName.Text = person.LastName;
            dtpHireDate.DateValue = person.HireDate;
            PreviousHireDate = person.HireDate;
            PreviousTerminationDate = (person.EmploymentHistory != null && person.EmploymentHistory.Count > 0) ? person.EmploymentHistory.Last().TerminationDate : null;

            //Last but one Termination date for Hire Date validation.
            TerminationDateBeforeCurrentHireDate = person.EmploymentHistory.Any(p => p.HireDate.Date < person.HireDate.Date) ? person.EmploymentHistory.Last(p => p.HireDate.Date < person.HireDate.Date).TerminationDate : null;

            PopulateTerminationDate(person.TerminationDate);
            FillTerminationReasonsByTerminationDate(dtpTerminationDate, ddlTerminationReason);
            PopulateTerminationReason(person.TerminationReasonid);
            txtEmailAddress.Text = person.Alias;
            txtTelephoneNumber.Text = person.TelephoneNumber.Trim();
            ddlPersonType.SelectedValue = person.IsOffshore ? "1" : "0";
            txtPayCheckId.Text = string.IsNullOrEmpty(person.PaychexID) ? "" : person.PaychexID;
            if ((int)person.DivisionType != 0)
            {
                ddlDivision.SelectedValue = ((int)person.DivisionType).ToString();
            }

            PopulatePracticeDropDown(person);

            PersonStatusId = person.Status != null ? (PersonStatusType?)person.Status.Id : null;
            PrevPersonStatusId = (person.Status != null) ? person.Status.Id : -1;
            lblPersonStatus.Text = PersonStatusId.HasValue ? DataHelper.GetDescription((PersonStatusType)PrevPersonStatusId) : string.Empty;

            txtEmployeeNumber.Text = person.EmployeeNumber;

            //Set Locked-Out CheckBox value
            chbLockedOut.Checked = person.LockedOut;
            defaultManager.EnsureDatabound();
            // Select manager and exclude self from the list
            defaultManager.SelectedManager = person.Manager;
            //defaultManager.ExcludePerson(new Person { Id = SelectedId });
            //newManager.ExcludePerson(new Person { Id = SelectedId });

            repPracticesOwned.DataSource = person.PracticesOwned;
            repPracticesOwned.DataBind();
            if (person.IsDefaultManager)
            {
                this.hdnIsDefaultManager.Value = person.IsDefaultManager.ToString();
            }
        }

        private void PopulatePracticeDropDown(Person person)
        {
            if (person != null && person.DefaultPractice != null)
            {
                ListItem selectedPractice = ddlDefaultPractice.Items.FindByValue(person.DefaultPractice.Id.ToString());

                if (selectedPractice == null)
                {
                    selectedPractice = new ListItem(person.DefaultPractice.Name, person.DefaultPractice.Id.ToString());
                    ddlDefaultPractice.Items.Add(selectedPractice);
                    ddlDefaultPractice.SortByText();
                }

                ddlDefaultPractice.SelectedValue = selectedPractice.Value;
            }
        }

        private void PopulateTerminationReason(int? terminationReasonId)
        {
            string selectedValue = string.Empty;

            if (terminationReasonId.HasValue)
            {
                selectedValue = terminationReasonId.Value.ToString();
                var item = ddlTerminationReason.Items.FindByValue(terminationReasonId.Value.ToString());
                if (item == null)
                {
                    var newItem = SettingsHelper.GetTerminationReasonsList().First(tr => tr.Id == terminationReasonId.Value);
                    ddlTerminationReason.Items.Add(new ListItem { Value = newItem.Id.ToString(), Text = newItem.Name });
                }
            }

            ddlTerminationReason.SelectedValue = selectedValue;
            PreviousTerminationReasonId = terminationReasonId.HasValue ? terminationReasonId.Value.ToString() : null;
        }

        private void PopulateTerminationDate(DateTime? terminationDate)
        {
            dtpTerminationDate.DateValue =
                   terminationDate.HasValue ? terminationDate.Value : DateTime.MinValue;
            PreviousTerminationDate = terminationDate;
        }

        #endregion

        /// <summary>
        /// Collects the user input to the DTO.
        /// </summary>
        /// <param name="person">The DTO to the data be collected to.</param>
        private void PopulateData(Person person)
        {
            person.Id = PersonId;
            person.FirstName = txtFirstName.Text;
            person.LastName = txtLastName.Text;
            person.HireDate = HireDate.Value;

            person.IsOffshore = ddlPersonType.SelectedValue == "1";
            person.PaychexID = txtPayCheckId.Text;
            person.TerminationDate = TerminationDate;// dtpTerminationDate.DateValue != DateTime.MinValue ? (DateTime?)dtpTerminationDate.DateValue : null;
            person.TerminationReasonid = string.IsNullOrEmpty(TerminationReasonId) ? null : (int?)Convert.ToInt32(TerminationReasonId);// ddlTerminationReason.SelectedValue != string.Empty ? (int?)Convert.ToInt32(ddlTerminationReason.SelectedValue) : null;

            person.Alias = txtEmailAddress.Text;
            person.TelephoneNumber = txtTelephoneNumber.Text;

            person.EmployeeNumber = txtEmployeeNumber.Text;

            person.Status = new PersonStatus { Id = (int)PersonStatusId };

            //Set Locked-Out value
            person.LockedOut = IsLockOut;

            person.DefaultPractice =
                !string.IsNullOrEmpty(ddlDefaultPractice.SelectedValue) ?
                new Practice { Id = int.Parse(ddlDefaultPractice.SelectedValue) } : null;

            // Filling the recruiter commissions for the given person
            person.RecruiterCommission = recruiterInfo.RecruiterCommission;

            // Filling the default commissions info
            var salesCommission = new DefaultCommission
            {
                TypeOfCommission = CommissionType.Sales,
                FractionOfMargin =
                    chbSalesCommissions.Checked
                        ? decimal.Parse(txtSalesCommissionsGross.Text)
                        : -1
            };

            var managementCommission = new DefaultCommission
            {
                TypeOfCommission = CommissionType.PracticeManagement,
                FractionOfMargin =
                    chbManagementCommissions.Checked
                        ? decimal.Parse(txtManagementCommission.Text)
                        : 0,
                MarginTypeId = int.Parse(rlstManagementCommission.SelectedValue)
            };

            person.DefaultPersonCommissions = new List<DefaultCommission> { salesCommission, managementCommission };

            // Role/Seniority
            if (!string.IsNullOrEmpty(ddlSeniority.SelectedValue))
            {
                person.Seniority = new Seniority { Id = int.Parse(ddlSeniority.SelectedValue) };
            }

            // Roles
            var roleNames = GetSelectedRoles();
            person.RoleNames = roleNames.ToArray();

            person.Manager = defaultManager.SelectedManager;

            if (ddlDivision.SelectedIndex != 0)
            {
                person.DivisionType = (PersonDivisionType)Enum.Parse(typeof(PersonDivisionType), ddlDivision.SelectedValue);
            }
        }

        private List<string> GetSelectedRoles()
        {
            return (from ListItem item in chblRoles.Items where item.Selected select item.Text).ToList();
        }

        /// <summary>
        /// Saves the user's input.
        /// </summary>
        private int? SaveData()
        {
            string loginPageUrl = base.Request.Url.Scheme + "://" + base.Request.Url.Host + (IsAzureWebRole() ? string.Empty : (":" + base.Request.Url.Port.ToString())) + base.Request.ApplicationPath + "/Login.aspx";

            var person = new Person();
            PopulateData(person);

            if (PersonId.HasValue && PrevPersonStatusId != -1 && PrevPersonStatusId == (int)PersonStatusType.Terminated && (PersonStatusId.Value == PersonStatusType.Active || PersonStatusId.Value == PersonStatusType.Contingent))
            {
                TransferToCompesationDetailPage(person, loginPageUrl);
            }
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    int? personId = serviceClient.SavePersonDetail(person, User.Identity.Name, loginPageUrl);
                    SaveRoles(person);

                    if (personId.Value < 0)
                    {
                        // Creating User error
                        _saveCode = personId.Value;
                        Page.Validate();
                        return null;
                    }

                    SavePersonsPermissions(person, serviceClient);

                    IsDirty = IsStatusChangeClicked = false;
                    AddTriggersToUpdatePanel(IsStatusChangeClicked);

                    return personId;
                }
                catch (Exception ex)
                {
                    serviceClient.Abort();
                    ExMessage = ex.Message;
                    Page.Validate();
                }
            }

            return null;
        }

        private void TransferToCompesationDetailPage(Person person, string loginPageUrl)
        {
            //To transfer the unsave person data to Compensation detail page.
            person.PaymentHistory = PayHistory;
            PersonUnsavedData = person;
            LoginPageUrl = loginPageUrl;

            if (bool.Parse(hfReloadPerms.Value))
            {
                Permissions = rpPermissions.GetPermissions();
            }
            Server.Transfer("~/CompensationDetail.aspx?Id=" + PersonUnsavedData.Id + "&returnTo=" + Request.Url, true);
        }

        private static bool IsAzureWebRole()
        {
            try
            {
                return RoleEnvironment.IsAvailable;
            }
            catch
            {
                return false;
            }
        }

        private void SavePersonsPermissions(Person person, PersonServiceClient serviceClient)
        {
            if (bool.Parse(hfReloadPerms.Value))
            {
                PersonPermission permissions = rpPermissions.GetPermissions();

                serviceClient.SetPermissionsForPerson(person, permissions);
            }
        }

        private static void SaveRoles(Person person)
        {
            if (string.IsNullOrEmpty(person.Alias)) return;

            // Saving roles
            string[] currentRoles = Roles.GetRolesForUser(person.Alias);

            if (person.RoleNames.Length > 0)
            {
                // New roles
                string[] newRoles =
                    Array.FindAll(person.RoleNames, value => Array.IndexOf(currentRoles, value) < 0);

                if (newRoles.Length > 0)
                    Roles.AddUserToRoles(person.Alias, newRoles);
            }

            if (currentRoles.Length > 0)
            {
                // Redundant roles
                string[] redundantRoles =
                    Array.FindAll(currentRoles, value => Array.IndexOf(person.RoleNames, value) < 0);

                if (redundantRoles.Length > 0)
                    Roles.RemoveUserFromRoles(person.Alias, redundantRoles);
            }
        }

        protected void vwPermissions_PreRender(object sender, EventArgs e)
        {
            if (UserIsAdministrator || UserIsHR)//#2817 UserisHR is added as per requirement.
            {
                DisplayPersonPermissions();
                hfReloadPerms.Value = bool.TrueString;
            }
        }

        protected void cvPracticeArea_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (PersonStatusId.HasValue && PersonStatusId == PersonStatusType.Active)
            {
                if (string.IsNullOrEmpty(ddlDefaultPractice.SelectedValue))
                {
                    args.IsValid = false;
                    return;
                }
            }
            args.IsValid = true;
        }

        protected void custCompensationCoversMilestone_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // Checks if the person is active
            if (PersonStatusId.Value == PersonStatusType.Active || PersonStatusId == PersonStatusType.TerminationPending)
            {
                args.IsValid = !PersonId.HasValue || (PersonId.HasValue && DataHelper.CurrentPayExists(PersonId.Value));
            }
        }

        protected void lbSetPracticeOwner_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlDefaultPractice.SelectedValue))
            {
                var practiceList = DataHelper.GetPracticeById(int.Parse(ddlDefaultPractice.SelectedValue));
                defaultManager.SelectedManager = practiceList[0].PracticeOwner;
            }
        }

        #region Validation

        private ExceptionDetail internalException;
        private bool DisableValidatecustTerminateDateTE;

        protected void custValPractice_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            var custValPractice = sender as CustomValidator;

            var gvRow = custValPractice.NamingContainer as GridViewRow;

            var ddlPractice = gvRow.FindControl("ddlPractice") as DropDownList;

            e.IsValid = ddlPractice.SelectedIndex > 0;
        }

        protected void custCancelTermination_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (dtpTerminationDate.DateValue != DateTime.MinValue)
            {
                List<Milestone> milestonesAfterTerminationDate = new List<Milestone>();
                DateTime terminationDate;
                if (PreviousTerminationDate.HasValue && PreviousTerminationDate.Value != GetDate(dtpTerminationDate.DateValue).Value)
                {
                    terminationDate = PreviousTerminationDate.Value < GetDate(dtpTerminationDate.DateValue).Value ? PreviousTerminationDate.Value : GetDate(dtpTerminationDate.DateValue).Value;
                }
                else
                {
                    terminationDate = GetDate(dtpTerminationDate.DateValue).Value;
                }

                milestonesAfterTerminationDate.AddRange(ServiceCallers.Custom.Person(p => p.GetPersonMilestonesAfterTerminationDate(this.PersonId.Value, terminationDate)));
                if (milestonesAfterTerminationDate.Any<Milestone>())
                {
                    //this.lblProjectMilestomesExist.Text = string.Format(lblProjectMilestomesExistFormat, person.Name, terminationDate.Value.ToString("MM/dd/yyy"));
                    this.dtlCancelProjectMilestones.DataSource = milestonesAfterTerminationDate;
                    this.dtlCancelProjectMilestones.DataBind();
                    custCancelTermination.Text = string.Format(CancelTerminationMessage, txtLastName.Text + ", " + txtFirstName.Text);
                    args.IsValid = false;
                    mpeCancelTermination.Show();
                }
            }

        }

        protected void custValSeniority_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            var custValPractice = sender as CustomValidator;

            var gvRow = custValPractice.NamingContainer as GridViewRow;

            var ddlSeniorityName = gvRow.FindControl("ddlSeniorityName") as DropDownList;

            e.IsValid = ddlSeniorityName.SelectedIndex > 0;
        }

        protected void custValSalesCommission_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            var custValSalesCommission = sender as CustomValidator;

            var gvRow = custValSalesCommission.NamingContainer as GridViewRow;

            var txtSalesCommission = gvRow.FindControl("txtSalesCommission") as TextBox;

            var salesComm = txtSalesCommission.Text;
            decimal salecCommValue;
            if (!string.IsNullOrEmpty(salesComm))
            {
                if (!decimal.TryParse(salesComm, out salecCommValue))
                {
                    e.IsValid = false;
                    custValSalesCommission.ErrorMessage = custValSalesCommission.ToolTip =
                        "A number with 2 decimal digits is allowed for the sales commission %.";
                    return;
                }
                else if (salecCommValue < 0.00M)
                {
                    e.IsValid = false;
                    custValSalesCommission.ErrorMessage = custValSalesCommission.ToolTip =
                        "Sales Commission % must be greater than or equal 0.";
                    return;
                }
            }
            e.IsValid = true;
        }

        protected void custPersonName_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!String.IsNullOrEmpty(ExMessage))
            {
                args.IsValid = !(ExMessage == DuplicatePersonName);
            }
        }

        protected void custEmailAddress_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!String.IsNullOrEmpty(ExMessage))
            {
                args.IsValid = !(ExMessage == DuplicateEmail);
            }
        }

        protected void custEmployeeNumber_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid =
                _internalException == null ||
                _internalException.Message != ErrorCode.PersonEmployeeNumberUniquenesViolation.ToString();
        }

        protected void custPersonData_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid =
                _internalException == null ||
                _internalException.Message == ErrorCode.PersonNameUniquenesViolation.ToString() ||
                _internalException.Message == ErrorCode.PersonEmailUniquenesViolation.ToString() ||
                _internalException.Message == ErrorCode.PersonEmployeeNumberUniquenesViolation.ToString();

            if (_internalException != null)
            {
                ((CustomValidator)source).Text = _internalException.Message;
            }
        }

        // Any person who is projected should not have any roles checked.  
        // User should  uncheck their roles to save the record.
        protected void custRoles_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (PersonStatusId.HasValue && PersonStatusId.Value == PersonStatusType.Contingent)
            {
                // Roles
                args.IsValid = !chblRoles.Items.Cast<ListItem>().Any(item => item.Selected);
            }
        }

        protected void cvRolesActiveStatus_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (PersonStatusId.HasValue && PersonStatusId == PersonStatusType.Active)
            {
                // Roles
                args.IsValid = chblRoles.Items.Cast<ListItem>().Any(item => item.Selected);
            }
        }

        protected void custSeniority_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            // Active persons must have the Seniority set.
            e.IsValid =
                PersonStatusId.Value != PersonStatusType.Active ||
                !string.IsNullOrEmpty(ddlSeniority.SelectedValue);
        }

        protected void custTerminationDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (PersonStatusId.HasValue && (PersonStatusId.Value == PersonStatusType.Terminated || PersonStatusId.Value == PersonStatusType.TerminationPending))
            {
                args.IsValid = TerminationDate.HasValue;
            }
        }

        protected void custTerminationReason_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (PersonStatusId.HasValue && (PersonStatusId.Value == PersonStatusType.Terminated || PersonStatusId.Value == PersonStatusType.TerminationPending))
            {
                args.IsValid = !string.IsNullOrEmpty(TerminationReasonId);//(ddlTerminationReason.SelectedIndex != 0);
            }
        }

        ///<summary>
        ///validates change employee pop up controls.
        ///</summary>
        protected void cvTerminationReason_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = (ddlPopUpTerminationReason.SelectedIndex != 0);
        }

        protected void cvWithTerminationDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = (PreviousTerminationDate.HasValue) ? HireDate > PreviousTerminationDate : ((TerminationDateBeforeCurrentHireDate.HasValue) ? HireDate > TerminationDateBeforeCurrentHireDate : true);
        }

        protected void custWithPreviousTermDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = (TerminationDateBeforeCurrentHireDate.HasValue) ? HireDate > TerminationDateBeforeCurrentHireDate : true;
        }

        /// <summary>
        /// Validates a user ctreate status code.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void custUserName_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = _saveCode == default(int);

            string message;
            switch (-_saveCode)
            {
                case (int)MembershipCreateStatus.DuplicateEmail:
                    message = Messages.DuplicateEmail;
                    break;
                case (int)MembershipCreateStatus.DuplicateUserName:
                    //  Because we're using email as username in the system,
                    //      DuplicateUserName is equal to our PersonEmailUniquenesViolation
                    message = Messages.DuplicateEmail;
                    break;
                case (int)MembershipCreateStatus.InvalidAnswer:
                    message = Messages.InvalidAnswer;
                    break;
                case (int)MembershipCreateStatus.InvalidEmail:
                    message = Messages.InvalidEmail;
                    break;
                case (int)MembershipCreateStatus.InvalidPassword:
                    message = Messages.InvalidPassword;
                    break;
                case (int)MembershipCreateStatus.InvalidQuestion:
                    message = Messages.InvalidQuestion;
                    break;
                case (int)MembershipCreateStatus.InvalidUserName:
                    message = Messages.InvalidUserName;
                    break;
                case (int)MembershipCreateStatus.ProviderError:
                    message = Messages.ProviderError;
                    break;
                case (int)MembershipCreateStatus.UserRejected:
                    message = Messages.UserRejected;
                    break;
                default:
                    message = custUserName.ErrorMessage;
                    return;
            }
            custUserName.ErrorMessage = custUserName.ToolTip = message;
        }

        protected void custReqEmailAddress_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (PersonStatusId.HasValue && PersonStatusId.Value == PersonStatusType.Active)
            {
                args.IsValid = !string.IsNullOrEmpty(txtEmailAddress.Text);
            }
        }

        protected void custPersonStatus_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid =
                // Only administrators can set a status to Active or Terminated
                UserIsAdministrator || UserIsHR || !PersonStatusId.HasValue ||
                PersonStatusId.Value == PersonStatusType.Contingent || PersonStatusId.Value == PersonStatusType.Inactive;//#2817 UserisHR is added as per requirement.
        }

        protected void custHireDate_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            List<RecruiterCommission> commissions = recruiterInfo.RecruiterCommission;
            Person current = DataHelper.CurrentPerson;
            e.IsValid =
                UserIsAdministrator || UserIsHR ||
                (current != null && current.Id.HasValue && commissions != null &&
                 commissions.Count > 0 &&
                 commissions.Count(commission => commission.RecruiterId != current.Id.Value) == 0);//#2817 UserisHR This person still has an open compensation record. Click OK to close their compensation record as of their termination date, or click Cancel to exit without saving changes.is added as per requirement.
        }

        protected void cvEndCompensation_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            var validator = ((CustomValidator)sender);
            if (TerminationDate.HasValue && (PersonStatusId == PersonStatusType.Terminated || PersonStatusId == PersonStatusType.TerminationPending || PersonStatusId == PersonStatusType.Contingent))
            {
                if (PayHistory.Any(p => p.EndDate.HasValue && p.EndDate.Value.AddDays(-1).Date > TerminationDate.Value.Date))
                {
                    e.IsValid = false;
                    validator.ErrorMessage = CloseAnActiveCompensation;
                }
                else if (PayHistory.Any(p => !p.EndDate.HasValue))
                {
                    e.IsValid = false;
                    validator.ErrorMessage = CloseAnOpenEndedCompensation;
                }
                else
                {
                    e.IsValid = true;
                }
                if (!e.IsValid)
                {
                    mpeChangeStatusEndCompensation.Show();
                }

                validator.Text = validator.ToolTip = validator.ErrorMessage;

                DisableValidatecustTerminateDateTE = !e.IsValid;
            }
        }

        protected void cvHireDateChange_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            DateTime? terminationDate = IsStatusChangeClicked ? PreviousTerminationDate : TerminationDateBeforeCurrentHireDate;

            var validator = ((CustomValidator)sender);
            e.IsValid = true;
            if (PreviousHireDate.HasValue && HireDate != PreviousHireDate && PayHistory != null)
            {
                if (PayHistory.Any(p => p.EndDate.HasValue && p.EndDate.Value.AddDays(-1).Date < HireDate.Value.Date
                                        &&
                                        (
                                            (terminationDate.HasValue && p.StartDate > terminationDate.Value.Date)
                                            || !terminationDate.HasValue
                                        )
                                )
                    )
                {
                    e.IsValid = false;
                }
                Pay firstPay = PayHistory.OrderBy(p => p.StartDate).Where(p =>
                                        (terminationDate.HasValue && p.StartDate > terminationDate.Value.Date)
                                        || !terminationDate.HasValue).FirstOrDefault();
                if (firstPay != null && firstPay.StartDate != HireDate)
                {
                    e.IsValid = false;
                }
                if (!e.IsValid)
                {
                    validator.ErrorMessage = HireDateChangeMessage;
                    mpeHireDateChange.Show();
                }

                validator.Text = validator.ToolTip = validator.ErrorMessage;
            }
        }

        #endregion

        #region Commissions

        protected void chbSalesCommissions_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSalesCommissionState();
            IsDirty = true;
        }

        protected void chbManagementCommissions_CheckedChanged(object sender, EventArgs e)
        {
            UpdateManagementCommissionState();
            IsDirty = true;
        }

        private void UpdateSalesCommissionState()
        {
            txtSalesCommissionsGross.Enabled =
                reqSalesCommissionsGross.Enabled = compSalesCommissionsGross.Enabled =
                                                   chbSalesCommissions.Checked;
        }

        private void UpdateManagementCommissionState()
        {
            txtManagementCommission.Enabled =
                reqManagementCommission.Enabled = compManagementCommission.Enabled =
                                                  rlstManagementCommission.Enabled =
                                                  chbManagementCommissions.Checked;
        }

        #endregion

        protected void valRecruterRole_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            // This is a stub for the issue #1858
            args.IsValid = true;
        }

        protected void nPerson_OnNoteAdded(object source, EventArgs args)
        {
            activityLog.Update();
        }

        protected void custTerminationDateTE_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime? terminationDate = TerminationDate;// (this.dtpTerminationDate.DateValue != DateTime.MinValue) ? new DateTime?(this.dtpTerminationDate.DateValue) : null;
            bool TEsExistsAfterTerminationDate = false;
            List<Milestone> milestonesAfterTerminationDate = new List<Milestone>();
            List<Project> ownerProjects = new List<Project>();
            List<Opportunity> ownerOpportunities = new List<Opportunity>();

            using (PersonServiceClient serviceClient = new PersonServiceClient())
            {
                if (this.PersonId.HasValue && terminationDate.HasValue)
                {
                    TEsExistsAfterTerminationDate = serviceClient.CheckPersonTimeEntriesAfterTerminationDate(this.PersonId.Value, terminationDate.Value);
                    milestonesAfterTerminationDate.AddRange(serviceClient.GetPersonMilestonesAfterTerminationDate(this.PersonId.Value, terminationDate.Value.AddDays(1)));
                    ownerProjects.AddRange(serviceClient.GetOwnerProjectsAfterTerminationDate(this.PersonId.Value, terminationDate.Value.AddDays(1)));
                    ownerOpportunities.AddRange(serviceClient.GetActiveOpportunitiesByOwnerId(this.PersonId.Value));
                }
            }
            if (TEsExistsAfterTerminationDate || milestonesAfterTerminationDate.Any<Milestone>() || ownerProjects.Any<Project>() || ownerOpportunities.Any<Opportunity>())
            {
                this.dvTerminationDateErrors.Visible = true;

                var person = DataHelper.GetPerson(this.PersonId.Value);
                lblTerminationDateError.Text = string.Format(lblTerminationDateErrorFormat, person.Name);

                mpeViewTerminationDateErrors.Show();

                if (TEsExistsAfterTerminationDate)
                {
                    this.lblTimeEntriesExist.Visible = true;
                    this.lblTimeEntriesExist.Text = string.Format(lblTimeEntriesExistFormat, terminationDate.Value.ToString("MM/dd/yyy"));
                }
                else
                {
                    this.lblTimeEntriesExist.Visible = false;
                }
                if (milestonesAfterTerminationDate.Any<Milestone>())
                {
                    this.dvProjectMilestomesExist.Visible = true;
                    this.lblProjectMilestomesExist.Text = string.Format(lblProjectMilestomesExistFormat, person.Name, terminationDate.Value.ToString("MM/dd/yyy"));
                    this.dtlProjectMilestones.DataSource = milestonesAfterTerminationDate;
                    this.dtlProjectMilestones.DataBind();
                }
                else
                {
                    this.dvProjectMilestomesExist.Visible = false;
                }

                if (ownerProjects.Any<Project>())
                {
                    this.divOwnerProjectsExist.Visible = true;
                    this.lblOwnerProjectsExist.Text = string.Format(lblOwnerProjectsExistFormat, person.Name);
                    this.dtlOwnerProjects.DataSource = ownerProjects;
                    this.dtlOwnerProjects.DataBind();
                }
                else
                {
                    this.divOwnerProjectsExist.Visible = false;
                }

                if (ownerOpportunities.Any<Opportunity>())
                {
                    this.divOwnerOpportunitiesExist.Visible = true;
                    this.lblOwnerOpportunities.Text = string.Format(lblOwnerOpportunitiesFormat, person.Name);
                    this.dtlOwnerOpportunities.DataSource = ownerOpportunities;
                    this.dtlOwnerOpportunities.DataBind();
                }
                else
                {
                    this.divOwnerOpportunitiesExist.Visible = false;
                }

                //this.dtpTerminationDate.DateValue = terminationDate.Value;
                args.IsValid = false;
            }
        }

        protected void custIsDefautManager_ServerValidate(object source, ServerValidateEventArgs args)
        {
            bool isDefaultManager;
            DateTime? terminationDate = TerminationDate;// (this.dtpTerminationDate.DateValue != DateTime.MinValue) ? new DateTime?(this.dtpTerminationDate.DateValue) : null;
            if ((terminationDate.HasValue && bool.TryParse(this.hdnIsDefaultManager.Value, out isDefaultManager)) && isDefaultManager)
            {
                args.IsValid = false;
                custIsDefautManager.ToolTip = custIsDefautManager.ErrorMessage;
            }
        }

        protected void btnTerminationProcessCancel_OnClick(object source, EventArgs args)
        {
            ResetToPreviousData();
            mpeViewTerminationDateErrors.Hide();
        }

        protected void btnTerminationProcessOK_OnClick(object source, EventArgs args)
        {
            DisableValidatecustTerminateDateTE = true;
            cvEndCompensation.Enabled = false;
            Save_Click(source, args);
            mpeViewTerminationDateErrors.Hide();
        }

        #region gvCompensationHistory Events

        private void _gvCompensationHistory_OnRowDataBound(GridViewRow gvRow, Pay pay)
        {

            var dpStartDate = gvRow.FindControl("dpStartDate") as DatePicker;
            var dpEndDate = gvRow.FindControl("dpEndDate") as DatePicker;
            var ddlBasis = gvRow.FindControl("ddlBasis") as DropDownList;
            var ddlPractice = gvRow.FindControl("ddlPractice") as DropDownList;
            var ddlSeniorityName = gvRow.FindControl("ddlSeniorityName") as DropDownList;
            var txtAmount = gvRow.FindControl("txtAmount") as TextBox;
            var txtVacationDays = gvRow.FindControl("txtVacationDays") as TextBox;
            var txtSalesCommission = gvRow.FindControl("txtSalesCommission") as TextBox;

            DataHelper.FillSenioritiesList(ddlSeniorityName, "-- Select Seniority --");
            DataHelper.FillPracticeListOnlyActive(ddlPractice, "-- Select Practice Area --");

            dpStartDate.DateValue = pay.StartDate;
            dpEndDate.DateValue = pay.EndDate.HasValue ? pay.EndDate.Value.AddDays(-1) : DateTime.MinValue;
            ddlBasis.Attributes["vacationdaysId"] = txtVacationDays.ClientID;

            if (pay.Timescale == TimescaleType.Salary)
            {
                ddlBasis.SelectedIndex = 0;
                txtVacationDays.Enabled = true;
            }
            else if (pay.Timescale == TimescaleType.Hourly)
            {
                ddlBasis.SelectedIndex = 1;
                txtVacationDays.Enabled = true;
            }
            else if (pay.Timescale == TimescaleType._1099Ctc)
            {
                ddlBasis.SelectedIndex = 2;
                txtVacationDays.Enabled = false;
            }
            else
            {
                ddlBasis.SelectedIndex = 3;
                txtVacationDays.Enabled = false;
            }

            txtAmount.Text = pay.Amount.Value.ToString();

            if (pay.PracticeId.HasValue)
            {
                ListItem selectedPractice = ddlPractice.Items.FindByValue(pay.PracticeId.Value.ToString());
                if (selectedPractice == null)
                {
                    var practices = DataHelper.GetPracticeById(pay.PracticeId);
                    if (practices != null && practices.Length > 0)
                    {
                        selectedPractice = new ListItem(practices[0].Name, practices[0].Id.ToString());
                        ddlPractice.Items.Add(selectedPractice);
                        ddlPractice.SortByText();
                        ddlPractice.SelectedValue = selectedPractice.Value;
                    }
                }
                else
                {
                    ddlPractice.SelectedValue = selectedPractice.Value;
                }
            }


            if (pay.SeniorityId.HasValue)
            {
                ListItem selectedSeniority = ddlSeniorityName.Items.FindByValue(pay.SeniorityId.Value.ToString());
                if (selectedSeniority != null)
                {
                    ddlSeniorityName.SelectedValue = selectedSeniority.Value;
                }
            }

            txtVacationDays.Text = pay.VacationDays.HasValue ? pay.VacationDays.Value.ToString() : "0";
            txtSalesCommission.Text = pay.SalesCommissionFractionOfMargin.HasValue ? pay.SalesCommissionFractionOfMargin.Value.ToString() : "0.00";
        }

        protected void gvCompensationHistory_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var gvRow = e.Row;
                var pay = gvRow.DataItem as Pay;

                var imgCopy = e.Row.FindControl("imgCopy") as Image;
                var imgEditCompensation = e.Row.FindControl("imgEditCompensation") as Image;
                var imgCompensationDelete = e.Row.FindControl("imgCompensationDelete") as Image;
                var isVisible = (pay.EndDate.HasValue) ? !((pay.EndDate.Value.AddDays(-1) < dtpHireDate.DateValue) || (PersonStatusId.HasValue && PersonStatusId.Value == PersonStatusType.Terminated)) : true;

                imgCopy.Visible = isVisible;

                if (gvCompensationHistory.EditIndex == e.Row.DataItemIndex)
                {
                    _gvCompensationHistory_OnRowDataBound(gvRow, pay);
                }
                else
                {
                    imgCompensationDelete.Visible =
                    imgEditCompensation.Visible = isVisible;
                }
            }

            if (e.Row.RowType == DataControlRowType.Footer && e.Row.Visible && PayFooter != null)
            {
                var gvRow = e.Row;
                _gvCompensationHistory_OnRowDataBound(gvRow, PayFooter);
            }

        }

        protected void imgCompensationDelete_OnClick(object sender, EventArgs args)
        {
            var imgCompensationDelete = sender as ImageButton;

            var row = imgCompensationDelete.NamingContainer as GridViewRow;
            var cvDeleteCompensation = row.FindControl("cvDeleteCompensation") as CustomValidator;
            cvDeleteCompensation.IsValid = true;

            var startDate = Convert.ToDateTime(imgCompensationDelete.Attributes["StartDate"]);
            var endDateText = imgCompensationDelete.Attributes["EndDate"];
            DateTime? endDate = null;

            if (endDateText != string.Empty)
            {
                endDate = Convert.ToDateTime(endDateText);
            }

            bool result = true;

            if (PersonId.HasValue)
            {

                if (DateTime.Today >= startDate)
                {
                    using (var serviceClient = new PersonServiceClient())
                    {
                        result = serviceClient.IsPersonHaveActiveStatusDuringThisPeriod(PersonId.Value, startDate, endDate);
                    }
                }
                else
                {
                    result = false;
                }

                if (result)
                {
                    cvDeleteCompensation.IsValid = false;
                }
                else
                {
                    using (var serviceClient = new PersonServiceClient())
                    {
                        serviceClient.DeletePay(PersonId.Value, startDate);

                        PayHistory.Remove(PayHistory.First(p => p.StartDate.Date == startDate));
                        PayHistory = PayHistory;
                        PopulatePayment(PayHistory);
                    }
                }
            }

        }

        protected void imgCancel_OnClick(object sender, EventArgs e)
        {
            mlConfirmation.ClearMessage();
            gvCompensationHistory.EditIndex = -1;
            PopulatePayment(PayHistory);
        }

        protected void imgEditCompensation_OnClick(object sender, EventArgs e)
        {
            ImageButton imgEdit = sender as ImageButton;
            GridViewRow row = imgEdit.NamingContainer as GridViewRow;
            gvCompensationHistory.EditIndex = row.DataItemIndex;
            gvCompensationHistory.ShowFooter = false;
            PopulatePayment(PayHistory);
        }

        private bool validateAndSave(object sender, EventArgs e)
        {
            bool resultreturn = true;
            ImageButton imgUpdate = sender as ImageButton;
            GridViewRow gvRow = imgUpdate.NamingContainer as GridViewRow;
            var _gvCompensationHistory = gvRow.NamingContainer as GridView;
            var ddlBasis = gvRow.FindControl("ddlBasis") as DropDownList;
            var compVacationDays = gvRow.FindControl("compVacationDays") as CompareValidator;
            var rfvVacationDays = gvRow.FindControl("rfvVacationDays") as RequiredFieldValidator;
            var txtVacationDays = gvRow.FindControl("txtVacationDays") as TextBox;
            if (ddlBasis.SelectedIndex == 2 || ddlBasis.SelectedIndex == 3)
            {
                compVacationDays.Enabled = false;
                rfvVacationDays.Enabled = false;
                txtVacationDays.Enabled = false;

            }
            else
            {
                compVacationDays.Enabled = true;
                rfvVacationDays.Enabled = true;
                txtVacationDays.Enabled = true;
            }
            Page.Validate(valSumCompensation.ValidationGroup);
            if (Page.IsValid)
            {
                var operation = Convert.ToString(imgUpdate.Attributes["operation"]);
                DateTime startDate;
                var dpStartDate = gvRow.FindControl("dpStartDate") as DatePicker;
                var dpEndDate = gvRow.FindControl("dpEndDate") as DatePicker;
                var ddlPractice = gvRow.FindControl("ddlPractice") as DropDownList;
                var ddlSeniorityName = gvRow.FindControl("ddlSeniorityName") as DropDownList;
                var txtAmount = gvRow.FindControl("txtAmount") as TextBox;
                var txtSalesCommission = gvRow.FindControl("txtSalesCommission") as TextBox;

                Pay pay = new Pay();
                var index = 0;
                Pay oldPay;
                if (operation == "Update")
                {
                    startDate = Convert.ToDateTime(imgUpdate.Attributes["StartDate"]);
                    index = PayHistory.FindIndex(p => p.StartDate.Date == startDate);
                    oldPay = PayHistory[index];
                    pay.OldStartDate = oldPay.StartDate;
                    pay.OldEndDate = oldPay.EndDate;
                }
                else
                {
                    oldPay = PayFooter;
                }
                pay.TimesPaidPerMonth = oldPay.TimesPaidPerMonth;
                pay.DefaultHoursPerDay = oldPay.DefaultHoursPerDay;
                pay.Terms = !(ddlBasis.SelectedIndex == 2 || ddlBasis.SelectedIndex == 3) ? 5 : 14;
                pay.IsYearBonus = !(ddlBasis.SelectedIndex == 2 || ddlBasis.SelectedIndex == 3) ? oldPay.IsYearBonus : false;
                pay.BonusAmount = !(ddlBasis.SelectedIndex == 2 || ddlBasis.SelectedIndex == 3) ? oldPay.BonusAmount : 0;
                pay.BonusHoursToCollect = !(ddlBasis.SelectedIndex == 2 || ddlBasis.SelectedIndex == 3) ? oldPay.BonusHoursToCollect : null;

                pay.StartDate = dpStartDate.DateValue;
                pay.EndDate = dpEndDate.DateValue != DateTime.MinValue ? (DateTime?)dpEndDate.DateValue.AddDays(1) : null;

                if (ddlBasis.SelectedIndex == 0)
                {
                    pay.Timescale = TimescaleType.Salary;
                }
                else if (ddlBasis.SelectedIndex == 1)
                {
                    pay.Timescale = TimescaleType.Hourly;
                }
                else if (ddlBasis.SelectedIndex == 2)
                {
                    pay.Timescale = TimescaleType._1099Ctc;
                }
                else
                {
                    pay.Timescale = TimescaleType.PercRevenue;
                }

                decimal result;
                if (decimal.TryParse(txtAmount.Text, out result))
                {
                    pay.Amount = result;
                }

                pay.VacationDays = !string.IsNullOrEmpty(txtVacationDays.Text) && !(ddlBasis.SelectedIndex == 2 || ddlBasis.SelectedIndex == 3) ?
                   (int?)int.Parse(txtVacationDays.Text) : null;

                pay.SeniorityId = int.Parse(ddlSeniorityName.SelectedValue);
                pay.PracticeId = int.Parse(ddlPractice.SelectedValue);
                pay.SalesCommissionFractionOfMargin = Convert.ToDecimal(String.IsNullOrEmpty(txtSalesCommission.Text) ? "0.00" : txtSalesCommission.Text);

                pay.PersonId = PersonId.Value;

                using (PersonServiceClient serviceClient = new PersonServiceClient())
                {
                    try
                    {
                        serviceClient.SavePay(pay, HttpContext.Current.User.Identity.Name);
                    }
                    catch (FaultException<ExceptionDetail> ex)
                    {
                        internalException = ex.Detail;
                        serviceClient.Abort();
                        string exceptionMessage = internalException.InnerException != null ? internalException.InnerException.Message : string.Empty;
                        if (exceptionMessage == SalaryToContractException)
                        {
                            var row = ((ImageButton)sender).NamingContainer;
                            CustomValidator cVSalaryToContractVoilation = row.FindControl("cvSalaryToContractVoilation") as CustomValidator;
                            if (cVSalaryToContractVoilation != null)
                                cVSalaryToContractVoilation.IsValid = false;
                        }
                        else
                        {
                            Logging.LogErrorMessage(
                                ex.Message,
                                ex.Source,
                                internalException.InnerException != null ? internalException.InnerException.Message : string.Empty,
                                string.Empty,
                                HttpContext.Current.Request.Url.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped),
                                string.Empty,
                                Thread.CurrentPrincipal.Identity.Name);
                        }
                        resultreturn = false;
                    }
                }

            }
            else
            {
                resultreturn = false;
            }


            return resultreturn;
        }

        protected void imgUpdateCompensation_OnClick(object sender, EventArgs e)
        {
            mlConfirmation.ClearMessage();
            if (validateAndSave(sender, e))
            {
                ImageButton imgUpdate = sender as ImageButton;
                GridViewRow gvRow = imgUpdate.NamingContainer as GridViewRow;
                var _gvCompensationHistory = gvRow.NamingContainer as GridView;
                var operation = Convert.ToString(imgUpdate.Attributes["operation"]);
                if (operation != "Update")
                {
                    _gvCompensationHistory.ShowFooter = false;
                    PayFooter = null;
                }
                gvCompensationHistory.EditIndex = -1;
                ClearDirty();
                var person = GetPerson(PersonId);
                if (person != null)
                {
                    PayHistory = person.PaymentHistory;
                    PopulateControls(person);
                }
                mlConfirmation.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Compensation"));
            }
            else
            {
                Page.Validate(valSumCompensation.ValidationGroup);
                if (Page.IsValid)
                {
                    if (internalException != null)
                    {
                        string data = internalException.ToString();
                        string innerexceptionMessage = internalException.InnerException.Message;
                        if (data.Contains("CK_Pay_DateRange"))
                        {
                            mlConfirmation.ShowErrorMessage("Compensation for the same period already exists.");
                        }
                        else if (innerexceptionMessage == StartDateIncorrect || innerexceptionMessage == EndDateIncorrect || innerexceptionMessage == PeriodIncorrect || innerexceptionMessage == HireDateInCorrect)
                        {
                            mlConfirmation.ShowErrorMessage(innerexceptionMessage);
                        }
                    }
                }
            }
        }

        protected void imgCancelFooter_OnClick(object sender, EventArgs e)
        {
            ImageButton imgCancle = sender as ImageButton;
            GridViewRow row = imgCancle.NamingContainer as GridViewRow;
            var _gvCompensationHistory = row.NamingContainer as GridView;
            _gvCompensationHistory.ShowFooter = false;
            PopulatePayment(PayHistory);
            mlConfirmation.ClearMessage();

        }

        protected void imgCopy_OnClick(object sender, EventArgs e)
        {
            ImageButton imgCopy = sender as ImageButton;
            GridViewRow row = imgCopy.NamingContainer as GridViewRow;
            var _gvCompensationHistory = row.NamingContainer as GridView;
            gvCompensationHistory.EditIndex = -1;
            var pay = (Pay)PayHistory[row.DataItemIndex];
            var gvRow = _gvCompensationHistory.FooterRow;
            _gvCompensationHistory.ShowFooter = true;
            PayFooter = pay;
            _gvCompensationHistory.DataSource = PayHistory;
            _gvCompensationHistory.DataBind();
        }

        protected string GetTerminationReasonById(int? terminationReasonId)
        {
            if (terminationReasonId.HasValue && Utils.SettingsHelper.GetTerminationReasonsList().Any(t => t.Id == terminationReasonId))
            {
                return Utils.SettingsHelper.GetTerminationReasonsList().First(t => t.Id == terminationReasonId).Name;
            }
            return string.Empty;
        }

        #endregion

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            custCompensationCoversMilestone.Enabled = cvEndCompensation.Enabled = cvHireDateChange.Enabled = true;
            custCancelTermination.Enabled = false;
            if (PreviousTerminationDate.HasValue && dtpTerminationDate.DateValue != DateTime.MinValue && PreviousTerminationDate.Value != GetDate(dtpTerminationDate.DateValue).Value)
            {
                custCancelTermination.Enabled = true;
            }
            bool result = ValidateAndSavePersonDetails();
            if (result)
            {

                var query = Request.QueryString.ToString();
                var backUrl = string.Format(
                        Constants.ApplicationPages.DetailRedirectFormat,
                        Constants.ApplicationPages.PersonDetail,
                        this.PersonId.Value);
                RedirectWithBack(eventArgument, backUrl);
            }

        }

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects;
using Microsoft.WindowsAzure.ServiceRuntime;
using PraticeManagement.Configuration;
using PraticeManagement.Controls;
using PraticeManagement.PersonService;
using PraticeManagement.Security;
using PraticeManagement.Utils;
using Resources;

namespace PraticeManagement
{
    public partial class PersonDetail : PracticeManagementPersonDetailPageBase, IPostBackEventHandler
    {

        #region Constants

        private const string PersonStatusKey = "PersonStatus";
        private const string UserNameParameterName = "userName";
        private const string DuplicatePersonName = "There is another Person with the same First Name and Last Name.";
        private const string DuplicateEmail = "There is another Person with the same Email.";
        private const string DuplicatePersonId = "There is another Person with the same Employee Number.";
        private const string lblTimeEntriesExistFormat = "There are time entries submitted by person after {0}.";
        private const string lblProjectMilestomesExistFormat = "{0} is assigned to below Project - Milesone(s) after {1}:";
        private const string lblTerminationDateErrorFormat = "Unable to set Termination Date for {0} due to the following:";
        private const string lblOwnerProjectsExistFormat = "{0} is designated as the Owner for the following project(s):";
        private const string lblOwnerOpportunitiesFormat = "{0} is designated as the Owner for the following Opportunities:";
        private const string TerminationReasonFirstItem = "- - Select Termination Reason - -";
        private const string CloseAnActiveCompensation = "This person still has an active compensation record. Click OK to close their compensation record as of their termination date, or click Cancel to exit without saving changes.";
        private const string CloseAnOpenEndedCompensation = "This person still has an open compensation record. Click OK to close their compensation record as of their termination date, or click Cancel to exit without saving changes.";
        private const string HireDateChangeMessage = "This person has compensation record(s) before/after the new hire date. Click OK to adjust the compensation record to reflect the new hire date, or click Cancel to exit without saving changes.";
        public const string ReHireMessage = "On switching contractor status from 1099 Hourly or 1099 POR to W2-Hourly or W2-Salary he/she will be terminated on the end date of the latest compensation record and will be considered as Re-Hired from the start date of new compensation record. Click ok to continue or click cancel to exit without saving changes.";
        private const string CancelTerminationMessage = "Following are the list of projects in which {0} resource's end date(s)  were set to his/her previous termination date automatically. Please reset the end dates for {0} in the below listed 'Projects-Milestones' if applicable.";
        private const string displayNone = "displayNone";
        public const string SalaryToContractException = "Salary Type to Contract Type Violation";
        public const string SalaryToContractMessage = "To switch employee status from W2-Hourly or W2-Salary to a status of 1099 Hourly or 1099 POR, the user will have to terminate their employment using the \"Change Employee Status\" workflow, select a termination reason, and then re-activate the person's status via the \"Change Employee Status\" workflow, changing their pay type to \"1099 Hourly\" or \"1099 POR\".";
        public const string EmployeePayTypeChangeVoilationMessage = "On changing employee pay type, Time-Off(s) related to previous pay type existing in the new pay type date range will be deleted. Click ok to continue or click cancel to exit without saving the changes.";
        public const string StartDateIncorrect = "The Start Date is incorrect. There are several other compensation records for the specified period. Please edit them first.";
        public const string EndDateIncorrect = "The End Date is incorrect. There are several other compensation records for the specified period. Please edit them first.";
        public const string PeriodIncorrect = "The period is incorrect. There records falls into the period specified in an existing record.";
        public const string HireDateInCorrect = "Person cannot have the compensation for the days before his hire date.";
        public const string SLTApprovalPopUpMessage = "The inputted value is outside of the approved salary band for this level. A salary figure outside of the band requires approval from a member of the Senior Leadership Team. Please ensure that you have received that approval before continuing.";
        public const string SLTPTOApprovalPopUpMessage = "Any change to a person's allotted PTO accrual requires approval from a member of the Senior Leadership Team. Please ensure that you have received that approval before continuing.";

        #endregion

        #region Fields
        
        private int _saveCode;
        private bool? _userIsAdministratorValue;
        private bool? _userIsHRValue;
        private bool IsErrorPanelDisplay;
        private bool IsOtherPanelDisplay;
        private Pay payForcvEmployeePayTypeChangeViolation;
        private ExceptionDetail internalException;
        private bool _disableValidatecustTerminateDateTE;
        private int _finalWizardView = 2;
        private int _startingWizardView = 0;
        private DateTime? _editablePayStartDate;

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
                else if ((PrevPersonStatusId == (int)PersonStatusType.Terminated && rbnContingent.Checked) || (PrevPersonStatusId == (int)PersonStatusType.Contingent && rbnCancleTermination.Checked))
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
                        int _personid;
                        if (Int32.TryParse(this.hdnPersonId.Value, out _personid))
                        {
                            return _personid;
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

        public DateTime? PersonHireDate
        {
            get
            {
                return (DateTime?)ViewState["PersonHireDate_Key"];
            }
            set
            {
                ViewState["PersonHireDate_Key"] = value;
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

        private bool IsRehire
        {
            get
            {
                if (ViewState["View_IsRehire"] == null)
                {
                    ViewState["View_IsRehire"] = false;
                }
                return (bool)ViewState["View_IsRehire"];
            }
            set
            {
                ViewState["View_IsRehire"] = value;
            }
        }

        private bool IsWizards
        {
            get
            {
                return !PersonId.HasValue;
            }
        }

        private int ActiveWizard
        {
            get
            {
                if (ViewState["ActiveWizard"] == null)
                {
                    ViewState["ActiveWizard"] = _startingWizardView;
                }
                return (int)ViewState["ActiveWizard"];
            }
            set
            {
                ViewState["ActiveWizard"] = value;
            }
        }

        private Dictionary<int, int[]> ActiveWizardsArray
        {
            get
            {
                Dictionary<int, int[]> _ActiveWizardsArray = new Dictionary<int, int[]>();
                _ActiveWizardsArray.Add(0, new int[] { 0 });
                _ActiveWizardsArray.Add(1, new int[] { 0, 1 });
                _ActiveWizardsArray.Add(2, new int[] { 0, 1, 2 });
                return _ActiveWizardsArray;
            }
        }

        private string Email
        {
            get
            {
                string email = string.Empty;
                email = !string.IsNullOrEmpty(txtEmailAddress.Text) ? txtEmailAddress.Text + '@' + ddlDomain.SelectedValue : string.Empty;
                return email;
            }
        }

        #endregion

        #region Events

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            mlConfirmation.ClearMessage();
            AllowContinueWithoutSave = cellActivityLog.Visible = PersonId.HasValue;
            personOpportunities.TargetPersonId = PersonId;
            mlError.ClearMessage();
            this.dvTerminationDateErrors.Visible = false;

            if (mvPerson.Views[mvPerson.ActiveViewIndex] == vwActivityLog)
            {
                activityLog.Update();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Security
            chblRoles.Visible =
            btnResetPassword.Visible =
            locRolesLabel.Visible =
            ddlRecruiter.Enabled =
            cellPermissions.Visible =
            chbLockedOut.Visible = UserIsAdministrator || UserIsHR;//#2817 UserisHR is added as per requirement.
            txtEmployeeNumber.ReadOnly = !UserIsAdministrator && !UserIsHR;//#2817 UserisHR is added as per requirement.
            lbPayChexID.Visible = txtPayCheckId.Visible = UserIsAdministrator;

            if (!UserIsAdministrator && !UserIsHR && !PersonId.HasValue)//#2817 UserisHR is added as per requirement.
            {
                // Recruiter should not be able to set the person active.
                PersonStatusId = PersonStatusType.Contingent;
            }

            //Disable TerminationDate, TerminationReason
            DisableTerminationDateAndReason();

            ddlPersonStatus.Visible = !(lblPersonStatus.Visible = btnChangeEmployeeStatus.Visible = PersonId.HasValue);
            btnAddCompensation.Enabled = !(PersonStatusId == PersonStatusType.Terminated);

            DisableInactiveViews();
            DisplayWizardButtons();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (IsErrorPanelDisplay && !IsOtherPanelDisplay)
            {
                PopulateErrorPanel();
            }
        }

        #endregion

        #region Control Events

        #region mpeChangeStatusEndCompensation Events

        protected void btnEndCompensationOk_Click(object sender, EventArgs e)
        {
            cvEndCompensation.Enabled = false;
            mpeChangeStatusEndCompensation.Hide();
            _disableValidatecustTerminateDateTE = false;
            Save_Click(sender, e);
        }

        protected void btnEndCompensationCancel_Click(object source, EventArgs args)
        {
            ResetToPreviousData();
            mpeChangeStatusEndCompensation.Hide();
        }

        #endregion

        #region mpeEmployeePayTypeChange Events

        protected void btnEmployeePayTypeChangeViolationOk_Click(object sender, EventArgs e)
        {
            cvEmployeePayTypeChangeViolation.Enabled = false;
            GridViewRow gvRow = null;
            if (gvCompensationHistory.EditIndex != -1)
            {
                gvRow = gvCompensationHistory.Rows[gvCompensationHistory.EditIndex];
            }
            else
            {
                if (gvCompensationHistory.ShowFooter)
                {
                    gvRow = gvCompensationHistory.FooterRow;
                }
            }
            if (gvRow != null)
            {
                var imgUpdate = gvRow.FindControl("imgUpdateCompensation") as ImageButton;
                imgUpdateCompensation_OnClick((Object)imgUpdate, new EventArgs());
            }
            mpeEmployeePayTypeChange.Hide();
            cvEmployeePayTypeChangeViolation.Enabled = true;
        }

        protected void btnEmployeePayTypeChangeViolationCancel_Click(object source, EventArgs args)
        {
            ResetToPreviousData();
            cvEmployeePayTypeChangeViolation.Enabled = true;
            gvCompensationHistory.EditIndex = -1;
            gvCompensationHistory.ShowFooter = false;
            PopulatePayment(PayHistory);
            mpeEmployeePayTypeChange.Hide();
        }

        #endregion

        #region mpeHireDateChange Events

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

        #endregion

        #region mpeRehireConfirmation Events

        protected void btnRehireConfirmationOk_Click(object sender, EventArgs e)
        {
            cvRehireConfirmation.Enabled = false;
            mpeRehireConfirmation.Hide();
            IsRehire = true;
            _disableValidatecustTerminateDateTE = false;
            GridViewRow gvRow = null;
            if (gvCompensationHistory.EditIndex != -1)
            {
                gvRow = gvCompensationHistory.Rows[gvCompensationHistory.EditIndex];
            }
            else
            {
                if (gvCompensationHistory.ShowFooter)
                {
                    gvRow = gvCompensationHistory.FooterRow;
                }
            }
            if (gvRow != null)
            {
                var imgUpdate = gvRow.FindControl("imgUpdateCompensation") as ImageButton;
                imgUpdateCompensation_OnClick((Object)imgUpdate, new EventArgs());
            }
        }

        protected void btnRehireConfirmationCancel_Click(object source, EventArgs args)
        {
            ResetToPreviousData();
            cvRehireConfirmation.Enabled = true;
            gvCompensationHistory.EditIndex = -1;
            gvCompensationHistory.ShowFooter = false;
            PopulatePayment(PayHistory);
            mpeRehireConfirmation.Hide();
        }

        #endregion

        #region mpeViewPersonChangeStatus Events

        protected void dtpPopUpTerminationDate_OnSelectionChanged(object sender, EventArgs e)
        {
            FillTerminationReasonsByTerminationDate((DatePicker)sender, ddlPopUpTerminationReason);
            divTerminate.Attributes["class"] = "padLeft25 PaddingTop6";
            mpeViewPersonChangeStatus.Show();
        }

        protected void btnCancelChangePersonStatus_Click(object source, EventArgs args)
        {
            IsStatusChangeClicked = false;
            ResetToPreviousData();
            mpeViewPersonChangeStatus.Hide();
        }

        protected void btnOkChangePersonStatus_Click(object source, EventArgs args)
        {
            if (PopupStatus.HasValue)
            {
                IsStatusChangeClicked = true;
                _disableValidatecustTerminateDateTE = false;
                custCompensationCoversMilestone.Enabled = false;
                cvEndCompensation.Enabled = cvHireDateChange.Enabled = custCancelTermination.Enabled = true;

                var popupStatus = PopupStatus.Value == PersonStatusType.Contingent && PrevPersonStatusId == (int)PersonStatusType.Contingent && rbnTerminate.Checked ? PersonStatusType.TerminationPending : PopupStatus.Value;

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
                        if (rbnCancleTermination.Checked)
                        {
                            dtpPopUpTerminateDate.TextValue = ddlPopUpTerminationReason.SelectedValue = string.Empty;
                        }
                        else
                        {
                            Page.Validate(valSummaryChangePersonStatusToContingent.ValidationGroup);
                            if (!Page.IsValid)
                            {
                                divContingent.Attributes["class"] = "padLeft25 PaddingTop6";
                                divActive.Attributes["class"] = "displayNone";
                                divTerminate.Attributes["class"] = "displayNone";
                                mpeViewPersonChangeStatus.Show();
                            }
                        }
                        break;
                    case PersonStatusType.Active:
                        if (rbnCancleTermination.Checked)
                        {
                            dtpPopUpTerminateDate.TextValue = ddlPopUpTerminationReason.SelectedValue = string.Empty;
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
                    IsRehire = PrevPersonStatusId == (int)PersonStatusType.Terminated;
                    PersonStatusId = PopupStatus.Value;
                    lblPersonStatus.Text = DataHelper.GetDescription(PopupStatus.Value);
                    dtpHireDate.DateValue = HireDate.Value;
                    dtpTerminationDate.TextValue = TerminationDate.HasValue ? TerminationDate.Value.ToShortDateString() : string.Empty;
                    if (TerminationDate.HasValue)
                    {
                        ddlTerminationReason.Visible = true;
                        txtTerminationReason.Visible = false;
                        ddlTerminationReason.SelectedValue = TerminationReasonId;
                    }
                    Save_Click(source, args);
                }
            }
            else
            {
                mpeViewPersonChangeStatus.Show();
            }
        }

        #endregion

        #region mpeCancelTermination Events

        protected void btnCancleTerminationOKButton_OnClick(object source, EventArgs args)
        {
            custCancelTermination.Enabled = false;
            Save_Click(source, args);
            mpeCancelTermination.Hide();
        }

        protected void btnCancelTerminationCancelButton_OnClick(object sender, EventArgs e)
        {
            ResetToPreviousData();
            mpeCancelTermination.Hide();
        }

        #endregion

        #region mpeViewTerminationDateErrors Events

        protected void btnTerminationProcessCancel_OnClick(object source, EventArgs args)
        {
            ResetToPreviousData();
            cvRehireConfirmation.Enabled = true;
            mpeViewTerminationDateErrors.Hide();
        }

        protected void btnTerminationProcessOK_OnClick(object source, EventArgs args)
        {
            _disableValidatecustTerminateDateTE = true;
            cvEndCompensation.Enabled = false;
            if (!IsRehire)
            {
                Save_Click(source, args);
            }
            else
            {
                GridViewRow gvRow = null;
                if (gvCompensationHistory.EditIndex != -1)
                {
                    gvRow = gvCompensationHistory.Rows[gvCompensationHistory.EditIndex];
                }
                else
                {
                    if (gvCompensationHistory.ShowFooter)
                    {
                        gvRow = gvCompensationHistory.FooterRow;
                    }
                }
                if (gvRow != null)
                {
                    var imgUpdate = gvRow.FindControl("imgUpdateCompensation") as ImageButton;
                    imgUpdateCompensation_OnClick((Object)imgUpdate, new EventArgs());
                }
            }
            mpeViewTerminationDateErrors.Hide();
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

        #endregion

        protected void dtpTerminationDate_OnSelectionChanged(object sender, EventArgs e)
        {
            FillTerminationReasonsByTerminationDate((DatePicker)sender, ddlTerminationReason);
        }

        protected void btnChangeEmployeeStatus_Click(object sender, EventArgs e)
        {
            LoadChangeEmployeeStatusPopUpData();
            mpeViewPersonChangeStatus.Show();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var updatePersonStatusDropdown = true;
            custCompensationCoversMilestone.Enabled = cvEndCompensation.Enabled = cvHireDateChange.Enabled = !IsStatusChangeClicked;
            custCancelTermination.Enabled = true;

            if (PersonId.HasValue)
            {
                updatePersonStatusDropdown = false;
                PersonStatusId = (PersonStatusType)PrevPersonStatusId;
            }
            Save_Click(sender, e);
            if (PersonId.HasValue && updatePersonStatusDropdown)
            {
                FillAllPersonStatuses(PersonStatusId.Value);
            }
        }

        protected void btnView_Command(object sender, CommandEventArgs e)
        {
            int viewIndex = int.Parse((string)e.CommandArgument);

            if (SaveDirty && !ValidateAndSave())
            {
                return;
            }

            SelectView((Control)sender, viewIndex, false);

            if (mvPerson.Views[viewIndex] == vwActivityLog) //History
            {
                activityLog.Update();
            }

            if (mvPerson.Views[viewIndex] == vwOpportunities) //Opportunities
            {
                personOpportunities.DatabindOpportunities();
            }
        }

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

        protected void btnUnlock_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Email) && !IsDirty)
            {
                MembershipUser user = Membership.GetUser(Email);

                if (user != null)
                {
                    user.UnlockUser();
                }
            }
        }

        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Email) && !IsDirty)
            {
                MembershipUser user = Membership.GetUser(Email);

                if (user != null)
                {
                    user.ResetPassword();
                    btnResetPassword.Visible = false;
                    lblPaswordResetted.Visible = true;
                    chbLockedOut.Checked = user.IsLockedOut;

                }
            }
        }

        protected void dtpHireDate_SelectionChanged(object sender, EventArgs e)
        {
            IsDirty = true;
            dtpHireDate.Focus();
            if (IsWizards)
            {
                personnelCompensation.StartDate = dtpHireDate.DateValue;
            }
        }

        protected void ddlPersonTitle_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsWizards)
            {
                int titleId = 0;
                int.TryParse(ddlPersonTitle.SelectedValue, out titleId);
                personnelCompensation.TitleId = titleId == 0 ? (int?)null : titleId;
                personnelCompensation.UpdatePTOHours();
            }
            else
            {
                hdTitleChanged.Value = 1.ToString();
            }
        }

        protected void personnelCompensation_OnTitleChanged(object sender, EventArgs e)
        {
            if (IsWizards)
            {
                if (personnelCompensation.TitleId.HasValue)
                {
                    ddlPersonTitle.SelectedValue = personnelCompensation.TitleId.Value.ToString();
                }
                else
                {
                    if (ddlPersonTitle.Items.FindByValue("") != null)
                        ddlPersonTitle.SelectedValue = ddlPersonTitle.Items.FindByValue("").Value;
                }
            }
        }

        protected void personnelCompensation_SaveDetails(object sender, EventArgs e)
        {
            btnNext_Click(btnNext, null);
        }

        protected void personnelCompensation_OnPracticeChanged(object sender, EventArgs e)
        {
            if (IsWizards)
            {
                if (personnelCompensation.PracticeId.HasValue)
                    ddlDefaultPractice.SelectedValue = personnelCompensation.PracticeId.Value.ToString();
                else
                    ddlDefaultPractice.SelectedIndex = 0;
            }
        }

        protected void ddlDefaultPractice_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsWizards)
            {
                int practiceId = 0;
                int.TryParse(ddlDefaultPractice.SelectedValue, out practiceId);
                personnelCompensation.PracticeId = practiceId == 0 ? (int?)null : practiceId;
            }
        }

        protected void chblRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            IsDirty = true;
            chblRoles.Focus();
        }

        protected void vwPermissions_PreRender(object sender, EventArgs e)
        {
            if (UserIsAdministrator || UserIsHR)//#2817 UserisHR is added as per requirement.
            {
                DisplayPersonPermissions();
                hfReloadPerms.Value = bool.TrueString;
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

        protected void nPerson_OnNoteAdded(object source, EventArgs args)
        {
            activityLog.Update();
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

        #region Validation

        protected void custCompensationCoversMilestone_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // Checks if the person is active
            if (PersonStatusId.Value == PersonStatusType.Active || PersonStatusId == PersonStatusType.TerminationPending)
            {
                args.IsValid = !PersonId.HasValue || (PersonId.HasValue && DataHelper.CurrentPayExists(PersonId.Value));
            }
        }

        protected void custActiveCompensation_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // Checks if the person is active
            if (PersonStatusId.Value == PersonStatusType.Active)
            {
                args.IsValid = !personnelCompensation.EndDate.HasValue || (personnelCompensation.EndDate.HasValue && personnelCompensation.EndDate.Value.AddDays(-1) >= Utils.Generic.GetNowWithTimeZone().Date);
            }
        }

        protected void custCancelTermination_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;

            if (PreviousTerminationDate.HasValue &&
                    (
                        (PrevPersonStatusId == (int)PersonStatusType.Contingent && PersonStatusId == PersonStatusType.Contingent && !TerminationDate.HasValue)
                        || (TerminationDate.HasValue && PreviousTerminationDate.Value < TerminationDate.Value)
                        || (PrevPersonStatusId == (int)PersonStatusType.TerminationPending && (PersonStatusId == PersonStatusType.Active || PersonStatusId == PersonStatusType.Contingent) && !TerminationDate.HasValue)
                        )
                )
            {
                List<Milestone> milestonesAfterTerminationDate = new List<Milestone>();

                milestonesAfterTerminationDate.AddRange(ServiceCallers.Custom.Person(p => p.GetPersonMilestonesAfterTerminationDate(this.PersonId.Value, PreviousTerminationDate.Value)));
                if (milestonesAfterTerminationDate.Any<Milestone>())
                {
                    this.dtlCancelProjectMilestones.DataSource = milestonesAfterTerminationDate;
                    this.dtlCancelProjectMilestones.DataBind();
                    custCancelTermination.Text = string.Format(CancelTerminationMessage, txtLastName.Text + ", " + txtFirstName.Text);
                    args.IsValid = false;
                    mpeCancelTermination.Show();
                    IsOtherPanelDisplay = true;
                }
            }
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
                ExMessage == null ||
                ExMessage != DuplicatePersonId;
        }

        protected void custPersonData_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid =
                ExMessage == null ||
                ExMessage == DuplicatePersonName ||
                ExMessage == DuplicateEmail ||
                ExMessage == DuplicatePersonId;

            if (ExMessage != null)
            {
                ((CustomValidator)source).Text = ExMessage;
            }
        }

        protected void cvRoles_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = chblRoles.Items.Cast<ListItem>().Any(item => item.Selected);
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
            if (TerminationDate.HasValue)
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

        protected void custPersonStatus_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid =
                // Only administrators can set a status to Active or Terminated
                UserIsAdministrator || UserIsHR || !PersonStatusId.HasValue ||
                PersonStatusId.Value == PersonStatusType.Contingent || PersonStatusId.Value == PersonStatusType.Inactive;//#2817 UserisHR is added as per requirement.
        }

        protected void custHireDate_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            //need to refill the ddlRecruiter as per the new hire date
            Person current = DataHelper.CurrentPerson;
            e.IsValid =
                UserIsAdministrator || UserIsHR ||
                (current != null && current.Id.HasValue && int.Parse(ddlRecruiter.SelectedValue) != current.Id.Value);//#2817 UserisHR This person still has an open compensation record. Click OK to close their compensation record as of their termination date, or click Cancel to exit without saving changes.is added as per requirement.
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
                    IsOtherPanelDisplay = true;
                }

                validator.Text = validator.ToolTip = validator.ErrorMessage;

                _disableValidatecustTerminateDateTE = !e.IsValid;
            }
        }

        protected void cvEmployeePayTypeChangeViolation_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = true;
            if (payForcvEmployeePayTypeChangeViolation != null)
            {
                payForcvEmployeePayTypeChangeViolation.EndDate = payForcvEmployeePayTypeChangeViolation.EndDate.HasValue ? payForcvEmployeePayTypeChangeViolation.EndDate.Value : new DateTime(2029, 12, 31);
                bool isTimeOffExists = ServiceCallers.Custom.Person(p => p.IsPersonTimeOffExistsInSelectedRangeForOtherthanGivenTimescale(payForcvEmployeePayTypeChangeViolation.PersonId, payForcvEmployeePayTypeChangeViolation.StartDate, payForcvEmployeePayTypeChangeViolation.EndDate.Value, (int)payForcvEmployeePayTypeChangeViolation.Timescale));
                if (isTimeOffExists)
                {
                    var validator = ((CustomValidator)sender);
                    e.IsValid = false;
                    validator.Text = validator.ToolTip = validator.ErrorMessage = EmployeePayTypeChangeVoilationMessage;
                    mpeEmployeePayTypeChange.Show();
                    IsOtherPanelDisplay = true;
                }
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
                    IsOtherPanelDisplay = true;
                }

                validator.Text = validator.ToolTip = validator.ErrorMessage;
            }
        }

        protected void cvRehireConfirmation_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = true;
            var validator = ((CustomValidator)sender);
            var payHistory = PayHistory;
            if (payHistory != null && payHistory.Any(p => p.StartDate >= HireDate))
            {
                payHistory = payHistory.OrderBy(p => p.StartDate).ToList();
                PopulateUnCommitedPay(payHistory);
                Pay firstPay = payHistory.Where(p => p.StartDate >= HireDate).FirstOrDefault();
                if (firstPay != null && (firstPay.Timescale == TimescaleType.PercRevenue || firstPay.Timescale == TimescaleType._1099Ctc))
                {
                    if (payHistory.Any(p => p.StartDate > firstPay.StartDate && p.StartDate <= DateTime.Now.Date && (p.Timescale == TimescaleType.Salary || p.Timescale == TimescaleType.Hourly)))
                    {
                        e.IsValid = false;
                    }
                }
            }

            if (!e.IsValid)
            {
                validator.ErrorMessage = ReHireMessage;
                mpeRehireConfirmation.Show();
                IsOtherPanelDisplay = true;
            }

            validator.Text = validator.ToolTip = validator.ErrorMessage;
        }

        protected void custTerminationDateTE_ServerValidate(object source, ServerValidateEventArgs args)
        {
            Pay latestPay = null;
            var payHistory = PayHistory;
            if (IsRehire && payHistory != null && payHistory.Any(p => p.StartDate >= HireDate))
            {
                payHistory = payHistory.OrderBy(p => p.StartDate).ToList();
                PopulateUnCommitedPay(payHistory);
                Pay firstPay = payHistory.Where(p => p.StartDate >= HireDate).FirstOrDefault();
                if (firstPay != null && (firstPay.Timescale == TimescaleType.PercRevenue || firstPay.Timescale == TimescaleType._1099Ctc))
                {
                    if (payHistory.Any(p => p.StartDate > firstPay.StartDate && p.StartDate <= DateTime.Now.Date && (p.Timescale == TimescaleType.Salary || p.Timescale == TimescaleType.Hourly)))
                    {
                        latestPay = payHistory.Where(p => p.StartDate >= firstPay.StartDate && p.StartDate < DateTime.Today && (p.Timescale == TimescaleType.PercRevenue || p.Timescale == TimescaleType._1099Ctc)).LastOrDefault();
                    }
                }
            }
            DateTime? terminationDate = !IsRehire ? TerminationDate : (latestPay != null && latestPay.EndDate.HasValue ? (DateTime?)latestPay.EndDate.Value.AddDays(-1) : null);

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
                IsOtherPanelDisplay = true;

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

        protected void cvSLTApproval_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (!IsWizards && hdTitleChanged.Value == 1.ToString() && PersonId.HasValue)
            {
                Pay pay = ServiceCallers.Custom.Person(p => p.GetCurrentByPerson(PersonId.Value));
                int titleId = int.Parse(ddlPersonTitle.SelectedValue);
                Title title = ServiceCallers.Custom.Title(t => t.GetTitleById(titleId));

                if (pay != null && pay.Timescale == TimescaleType.Salary)
                {
                    if ((title.MinimumSalary.HasValue && title.MinimumSalary.Value > pay.Amount) || (title.MaximumSalary.HasValue && pay.Amount > title.MaximumSalary.Value))
                    {
                        args.IsValid = false;
                        hdcvSLTApproval.Value = true.ToString();
                        ltrlSLTApprovalPopUp.Text = SLTApprovalPopUpMessage;
                        mpeSLTApprovalPopUp.Show();
                        IsOtherPanelDisplay = true;
                    }
                    if (title.MinimumSalary.HasValue && title.MinimumSalary.Value <= pay.Amount && title.MaximumSalary.HasValue && pay.Amount <= title.MaximumSalary.Value)
                    {
                        hdcvSLTApproval.Value = false.ToString();
                    }
                }
            }
        }

        protected void cvSLTPTOApproval_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            cvSLTApproval.Validate();
            args.IsValid = true;
            if (!IsWizards && hdTitleChanged.Value == 1.ToString() && PersonId.HasValue && cvSLTApproval.IsValid)
            {
                Pay pay = ServiceCallers.Custom.Person(p => p.GetCurrentByPerson(PersonId.Value));
                int titleId = int.Parse(ddlPersonTitle.SelectedValue);
                Title title = ServiceCallers.Custom.Title(t => t.GetTitleById(titleId));

                if (pay != null && pay.Timescale == TimescaleType.Salary)
                {
                    if (pay.VacationDays.HasValue && title.PTOAccrual != pay.VacationDays.Value)
                    {
                        args.IsValid = false;
                        hdcvSLTPTOApproval.Value = true.ToString();
                        ltrlSLTPTOApprovalPopUp.Text = SLTPTOApprovalPopUpMessage;
                        mpeSLTPTOApprovalPopUp.Show();
                        IsOtherPanelDisplay = true;
                    }
                    else
                    {
                        hdcvSLTPTOApproval.Value = false.ToString();
                    }
                }
                hdTitleChanged.Value = 0.ToString();
            }
        }

        #endregion

        #region gvCompensationHistory Events

        #region mpeSLTApprovalPopUp Events

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            mpeSLTApprovalPopUp.Hide();
            if (!IsWizards)
            {
                if (hdcvSLTApproval.Value == true.ToString())
                {
                    ResetToPreviousData();
                }
                else
                {
                    GridViewRow gvRow = null;
                    if (gvCompensationHistory.EditIndex != -1)
                    {
                        gvRow = gvCompensationHistory.Rows[gvCompensationHistory.EditIndex];
                    }
                    else
                    {
                        if (gvCompensationHistory.ShowFooter)
                        {
                            gvRow = gvCompensationHistory.FooterRow;
                        }
                    }
                    if (gvRow != null)
                    {
                        var txtAmount = gvRow.FindControl("txtAmount") as TextBox;
                        txtAmount.Text = "";
                        txtAmount.Focus();
                    }
                }
            }
        }

        protected void btnSLTApproval_OnClick(object sender, EventArgs e)
        {
            mpeSLTApprovalPopUp.Hide();
            if (!IsWizards)
            {
                if (hdcvSLTApproval.Value == true.ToString())
                {
                    cvSLTApproval.Enabled = false;
                    Save_Click(sender, e);
                    cvSLTApproval.Enabled = true;
                }
                else
                {
                    GridViewRow gvRow = null;
                    if (gvCompensationHistory.EditIndex != -1)
                    {
                        gvRow = gvCompensationHistory.Rows[gvCompensationHistory.EditIndex];
                    }
                    else
                    {
                        if (gvCompensationHistory.ShowFooter)
                        {
                            gvRow = gvCompensationHistory.FooterRow;
                        }
                    }
                    if (gvRow != null)
                    {
                        var hdSLTApproval = gvRow.FindControl("hdSLTApproval") as HiddenField;
                        var txtAmount = gvRow.FindControl("txtAmount") as TextBox;
                        var hdAmount = gvRow.FindControl("hdAmount") as HiddenField;
                        var cvSLTPTOApprovalValidation = gvRow.FindControl("cvSLTPTOApprovalValidation") as CustomValidator;
                        var imgUpdateCompensation = gvRow.FindControl("imgUpdateCompensation") as ImageButton;
                        hdAmount.Value = txtAmount.Text;
                        hdSLTApproval.Value = true.ToString();
                        cvSLTPTOApprovalValidation.Validate();
                        if (cvSLTPTOApprovalValidation.IsValid)
                        {
                            imgUpdateCompensation_OnClick(imgUpdateCompensation, null);
                        }
                    }
                }
            }
        }

        protected void cvSLTApprovalValidation_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            var cvSLTApprovalValidation = source as CustomValidator;
            var gvRow = cvSLTApprovalValidation.NamingContainer as GridViewRow;
            var ddlBasis = gvRow.FindControl("ddlBasis") as DropDownList;
            var ddlTitle = gvRow.FindControl("ddlTitle") as DropDownList;
            var txtAmount = gvRow.FindControl("txtAmount") as TextBox;
            var hdAmount = gvRow.FindControl("hdAmount") as HiddenField;
            var custValTitle = gvRow.FindControl("custValTitle") as CustomValidator;
            var custValPractice = gvRow.FindControl("custValPractice") as CustomValidator;
            var reqAmount = gvRow.FindControl("reqAmount") as RequiredFieldValidator;
            var rfvVacationDays = gvRow.FindControl("rfvVacationDays") as RequiredFieldValidator;
            var hdSLTApproval = gvRow.FindControl("hdSLTApproval") as HiddenField;
            var cmpAmount = gvRow.FindControl("cmpAmount") as CompareValidator;
            var txtVacationDays = gvRow.FindControl("txtVacationDays") as TextBox;
            var hdVacationDay = gvRow.FindControl("hdVacationDay") as HiddenField;


            bool IsSLTApproval = !(hdSLTApproval.Value == false.ToString() || hdAmount.Value != txtAmount.Text);

            custValTitle.Validate();
            custValPractice.Validate();
            reqAmount.Validate();
            rfvVacationDays.Validate();
            cmpAmount.Validate();

            decimal salary;
            if (decimal.TryParse(txtAmount.Text, out salary) && custValTitle.IsValid)
            {
                int titleId = int.Parse(ddlTitle.SelectedValue);
                Title title = ServiceCallers.Custom.Title(t => t.GetTitleById(titleId));

                if (!IsSLTApproval && ddlBasis.SelectedIndex == 0 && custValPractice.IsValid && reqAmount.IsValid && rfvVacationDays.IsValid && cmpAmount.IsValid)
                {
                    if ((title.MinimumSalary.HasValue && title.MinimumSalary.Value > salary) || (title.MaximumSalary.HasValue && salary > title.MaximumSalary.Value))
                    {
                        args.IsValid = false;
                        ltrlSLTApprovalPopUp.Text = SLTApprovalPopUpMessage;
                        mpeSLTApprovalPopUp.Show();
                        IsOtherPanelDisplay = true;
                    }
                }
                if (title.MinimumSalary.HasValue && title.MinimumSalary.Value <= salary && title.MaximumSalary.HasValue && salary <= title.MaximumSalary.Value)
                {
                    hdSLTApproval.Value = false.ToString();
                }
            }
        }

        #endregion

        #region mpeSLTPTOApprovalPopUp Events

        protected void btnCancelSLTPTOApproval_OnClick(object sender, EventArgs e)
        {
            mpeSLTPTOApprovalPopUp.Hide();
            if (!IsWizards)
            {
                if (hdcvSLTPTOApproval.Value == true.ToString())
                {
                    ResetToPreviousData();
                }
                else
                {
                    GridViewRow gvRow = null;
                    if (gvCompensationHistory.EditIndex != -1)
                    {
                        gvRow = gvCompensationHistory.Rows[gvCompensationHistory.EditIndex];
                    }
                    else
                    {
                        if (gvCompensationHistory.ShowFooter)
                        {
                            gvRow = gvCompensationHistory.FooterRow;
                        }
                    }
                    if (gvRow != null)
                    {
                        var txtVacationDays = gvRow.FindControl("txtVacationDays") as TextBox;
                        txtVacationDays.Text = "";
                        txtVacationDays.Focus();
                    }
                }
            }
        }

        protected void btnSLTPTOApproval_OnClick(object sender, EventArgs e)
        {
            mpeSLTPTOApprovalPopUp.Hide();
            if (!IsWizards)
            {
                if (hdcvSLTPTOApproval.Value == true.ToString())
                {
                    cvSLTPTOApproval.Enabled = false;
                    Save_Click(sender, e);
                    cvSLTPTOApproval.Enabled = true;
                }
                else
                {
                    GridViewRow gvRow = null;
                    if (gvCompensationHistory.EditIndex != -1)
                    {
                        gvRow = gvCompensationHistory.Rows[gvCompensationHistory.EditIndex];
                    }
                    else
                    {
                        if (gvCompensationHistory.ShowFooter)
                        {
                            gvRow = gvCompensationHistory.FooterRow;
                        }
                    }
                    if (gvRow != null)
                    {
                        var hdSLTPTOApproval = gvRow.FindControl("hdSLTPTOApproval") as HiddenField;
                        var txtVacationDays = gvRow.FindControl("txtVacationDays") as TextBox;
                        var hdVacationDay = gvRow.FindControl("hdVacationDay") as HiddenField;
                        var imgUpdateCompensation = gvRow.FindControl("imgUpdateCompensation") as ImageButton;
                        hdVacationDay.Value = txtVacationDays.Text;
                        hdSLTPTOApproval.Value = true.ToString();
                        imgUpdateCompensation_OnClick(imgUpdateCompensation, null);
                    }
                }
            }
        }

        protected void cvSLTPTOApprovalValidation_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            var cvSLTPTOApprovalValidation = source as CustomValidator;
            var gvRow = cvSLTPTOApprovalValidation.NamingContainer as GridViewRow;
            var ddlBasis = gvRow.FindControl("ddlBasis") as DropDownList;
            var ddlTitle = gvRow.FindControl("ddlTitle") as DropDownList;
            var custValTitle = gvRow.FindControl("custValTitle") as CustomValidator;
            var custValPractice = gvRow.FindControl("custValPractice") as CustomValidator;
            var reqAmount = gvRow.FindControl("reqAmount") as RequiredFieldValidator;
            var rfvVacationDays = gvRow.FindControl("rfvVacationDays") as RequiredFieldValidator;
            var cmpAmount = gvRow.FindControl("cmpAmount") as CompareValidator;
            var txtVacationDays = gvRow.FindControl("txtVacationDays") as TextBox;
            var hdVacationDay = gvRow.FindControl("hdVacationDay") as HiddenField;
            var hdSLTPTOApproval = gvRow.FindControl("hdSLTPTOApproval") as HiddenField;
            var cvSLTApprovalValidation = gvRow.FindControl("cvSLTApprovalValidation") as CustomValidator;

            bool IsSLTPTOApproval = !(hdSLTPTOApproval.Value == false.ToString() || hdVacationDay.Value != txtVacationDays.Text);

            custValTitle.Validate();
            custValPractice.Validate();
            reqAmount.Validate();
            rfvVacationDays.Validate();
            cmpAmount.Validate();
            cvSLTApprovalValidation.Validate();
            int ptoAccrual = 0;
            if (int.TryParse(txtVacationDays.Text, out ptoAccrual) && custValTitle.IsValid && cvSLTApprovalValidation.IsValid)
            {
                int titleId = int.Parse(ddlTitle.SelectedValue);
                Title title = ServiceCallers.Custom.Title(t => t.GetTitleById(titleId));

                if (!IsSLTPTOApproval && ddlBasis.SelectedIndex == 0 && custValPractice.IsValid && reqAmount.IsValid && rfvVacationDays.IsValid && cmpAmount.IsValid)
                {
                    if (title.PTOAccrual != ptoAccrual)
                    {
                        args.IsValid = false;
                        ltrlSLTPTOApprovalPopUp.Text = SLTPTOApprovalPopUpMessage;
                        mpeSLTPTOApprovalPopUp.Show();
                        IsOtherPanelDisplay = true;
                    }
                }
                if (title.PTOAccrual == ptoAccrual)
                {
                    hdSLTPTOApproval.Value = false.ToString();
                }
            }
        }

        #endregion

        private void _gvCompensationHistory_OnRowDataBound(GridViewRow gvRow, Pay pay)
        {

            var dpStartDate = gvRow.FindControl("dpStartDate") as DatePicker;
            var dpEndDate = gvRow.FindControl("dpEndDate") as DatePicker;
            var ddlBasis = gvRow.FindControl("ddlBasis") as DropDownList;
            var ddlPractice = gvRow.FindControl("ddlPractice") as DropDownList;
            var ddlTitle = gvRow.FindControl("ddlTitle") as DropDownList;
            var txtAmount = gvRow.FindControl("txtAmount") as TextBox;
            var hdAmount = gvRow.FindControl("hdAmount") as HiddenField;
            var hdSLTApproval = gvRow.FindControl("hdSLTApproval") as HiddenField;
            var txtVacationDays = gvRow.FindControl("txtVacationDays") as TextBox;
            var hdVacationDay = gvRow.FindControl("hdVacationDay") as HiddenField;
            var hdSLTPTOApproval = gvRow.FindControl("hdSLTPTOApproval") as HiddenField;



            DataHelper.FillTitleList(ddlTitle, "-- Select Title --");
            DataHelper.FillPracticeListOnlyActive(ddlPractice, "-- Select Practice Area --");

            dpStartDate.DateValue = pay.StartDate;
            dpEndDate.DateValue = pay.EndDate.HasValue ? pay.EndDate.Value.AddDays(-1) : DateTime.MinValue;

            if (pay.Timescale == TimescaleType.Salary)
            {
                ddlBasis.SelectedIndex = 0;
                txtVacationDays.Enabled = true;
            }
            else if (pay.Timescale == TimescaleType.Hourly)
            {
                ddlBasis.SelectedIndex = 1;
                txtVacationDays.Enabled = false;
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

            hdAmount.Value = txtAmount.Text = pay.Amount.Value.ToString();
            hdSLTApproval.Value = pay.SLTApproval.ToString();
            hdSLTPTOApproval.Value = pay.SLTPTOApproval.ToString();
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


            if (pay.TitleId.HasValue)
            {
                ListItem selectedTitle = ddlTitle.Items.FindByValue(pay.TitleId.Value.ToString());
                if (selectedTitle != null)
                {
                    ddlTitle.SelectedValue = selectedTitle.Value;
                }
            }

            hdVacationDay.Value = txtVacationDays.Text = pay.VacationDays.HasValue ? pay.VacationDays.Value.ToString() : "0";
        }

        protected void gvCompensationHistory_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var gvRow = e.Row;
                var pay = gvRow.DataItem as Pay;
                var now = Utils.Generic.GetNowWithTimeZone();
                var imgCopy = e.Row.FindControl("imgCopy") as Image;
                var imgEditCompensation = e.Row.FindControl("imgEditCompensation") as Image;
                var imgCompensationDelete = e.Row.FindControl("imgCompensationDelete") as Image;
                var isVisible = (pay.EndDate.HasValue) ? !((pay.EndDate.Value.AddDays(-1) < now.Date) || (PersonStatusId.HasValue && PersonStatusId.Value == PersonStatusType.Terminated)) || (_editablePayStartDate.HasValue && _editablePayStartDate.Value == pay.StartDate) : true;

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
                PayFooter.SLTApproval = false;
                PayFooter.SLTPTOApproval = false;
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
                    IsErrorPanelDisplay = true;
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
            if (ddlBasis.SelectedIndex != 0)
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

            if (Page.IsValid && cvRehireConfirmation.Enabled)
            {
                cvRehireConfirmation.Validate();
                _disableValidatecustTerminateDateTE = true;
            }
            if (Page.IsValid && !_disableValidatecustTerminateDateTE)
            {
                custTerminateDateTE.Enabled = true;
                custTerminateDateTE.Validate();
            }
            Pay pay = new Pay();
            if (Page.IsValid)
            {
                var operation = Convert.ToString(imgUpdate.Attributes["operation"]);
                DateTime startDate;
                var dpStartDate = gvRow.FindControl("dpStartDate") as DatePicker;
                var dpEndDate = gvRow.FindControl("dpEndDate") as DatePicker;
                var ddlPractice = gvRow.FindControl("ddlPractice") as DropDownList;
                var ddlTitle = gvRow.FindControl("ddlTitle") as DropDownList;
                var txtAmount = gvRow.FindControl("txtAmount") as TextBox;
                var hdSLTApproval = gvRow.FindControl("hdSLTApproval") as HiddenField;
                var hdSLTPTOApproval = gvRow.FindControl("hdSLTPTOApproval") as HiddenField;


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

                pay.VacationDays = !string.IsNullOrEmpty(txtVacationDays.Text) && ddlBasis.SelectedIndex == 0 ?
                   (int?)int.Parse(txtVacationDays.Text) : null;
                pay.TitleId = int.Parse(ddlTitle.SelectedValue);
                pay.SLTApproval = ddlBasis.SelectedIndex == 0 && bool.Parse(hdSLTApproval.Value);
                pay.SLTPTOApproval = ddlBasis.SelectedIndex == 0 && bool.Parse(hdSLTPTOApproval.Value);
                pay.PracticeId = int.Parse(ddlPractice.SelectedValue);
                pay.PersonId = PersonId.Value;

                if (cvEmployeePayTypeChangeViolation.Enabled)
                {
                    payForcvEmployeePayTypeChangeViolation = pay;
                    cvEmployeePayTypeChangeViolation.Validate();
                }
            }

            if (Page.IsValid)
            {
                using (PersonServiceClient serviceClient = new PersonServiceClient())
                {
                    try
                    {
                        serviceClient.SavePay(pay, LoginPageUrl, HttpContext.Current.User.Identity.Name);
                    }
                    catch (FaultException<ExceptionDetail> ex)
                    {
                        internalException = ex.Detail;
                        string data = internalException.ToString();
                        serviceClient.Abort();
                        string exceptionMessage = internalException.InnerException != null ? internalException.InnerException.Message : string.Empty;
                        if (exceptionMessage == SalaryToContractException)
                        {
                            var row = ((ImageButton)sender).NamingContainer;
                            CustomValidator cVSalaryToContractVoilation = row.FindControl("cvSalaryToContractVoilation") as CustomValidator;
                            if (cVSalaryToContractVoilation != null)
                                cVSalaryToContractVoilation.IsValid = false;
                        }
                        else if (!(data.Contains("CK_Pay_DateRange") || exceptionMessage == StartDateIncorrect || exceptionMessage == EndDateIncorrect || exceptionMessage == PeriodIncorrect || exceptionMessage == HireDateInCorrect))
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
                cvEmployeePayTypeChangeViolation.Enabled = true;
            }
            else
            {
                resultreturn = false;
            }
            if (!Page.IsValid)
            {
                IsErrorPanelDisplay = true;
            }
            return resultreturn;
        }

        protected void imgUpdateCompensation_OnClick(object sender, EventArgs e)
        {
            mlConfirmation.ClearMessage();
            if (validateAndSave(sender, e))
            {
                IsRehire = false;
                cvRehireConfirmation.Enabled = true;
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
                IsErrorPanelDisplay = true;
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
                            IsErrorPanelDisplay = true;
                        }
                        else if (innerexceptionMessage == StartDateIncorrect || innerexceptionMessage == EndDateIncorrect || innerexceptionMessage == PeriodIncorrect || innerexceptionMessage == HireDateInCorrect)
                        {
                            mlConfirmation.ShowErrorMessage(innerexceptionMessage);
                            IsErrorPanelDisplay = true;
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
            PayFooter = (Pay)pay.Clone();
            pay.SLTApproval = false;
            pay.SLTPTOApproval = false;
            PopulatePayment(PayHistory);
            //_gvCompensationHistory.DataSource = PayHistory;
            //_gvCompensationHistory.DataBind();
        }

        protected void ddlBasis_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            EnableDisableVacationDays();
            UpdatePTOHours();
        }

        protected void ddlTitle_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var ddlTitle = sender as DropDownList;
            var gvRow = ddlTitle.NamingContainer as GridViewRow;
            var hdSLTApproval = gvRow.FindControl("hdSLTApproval") as HiddenField;
            var hdSLTPTOApproval = gvRow.FindControl("hdSLTPTOApproval") as HiddenField;
            hdSLTPTOApproval.Value = hdSLTApproval.Value = false.ToString();
            UpdatePTOHours();
        }

        private void UpdatePTOHours()
        {
            GridViewRow gvRow = null;
            if (gvCompensationHistory.EditIndex != -1)
            {
                gvRow = gvCompensationHistory.Rows[gvCompensationHistory.EditIndex];
            }
            else
            {
                if (gvCompensationHistory.ShowFooter)
                {
                    gvRow = gvCompensationHistory.FooterRow;
                }
            }
            if (gvRow != null)
            {
                var ddlBasis = gvRow.FindControl("ddlBasis") as DropDownList;
                var ddlTitle = gvRow.FindControl("ddlTitle") as DropDownList;
                var txtVacationDays = gvRow.FindControl("txtVacationDays") as TextBox;
                int titleId = 0;
                if (ddlBasis.SelectedIndex == 0 && ddlTitle.SelectedIndex > 0 && int.TryParse(ddlTitle.SelectedValue, out titleId))
                {
                    Title title = ServiceCallers.Custom.Title(t => t.GetTitleById(titleId));
                    txtVacationDays.Text = title.PTOAccrual.ToString();
                }
            }
        }

        private void EnableDisableVacationDays()
        {
            GridViewRow gvRow = null;
            if (gvCompensationHistory.EditIndex != -1)
            {
                gvRow = gvCompensationHistory.Rows[gvCompensationHistory.EditIndex];
            }
            else
            {
                if (gvCompensationHistory.ShowFooter)
                {
                    gvRow = gvCompensationHistory.FooterRow;
                }
            }
            if (gvRow != null)
            {
                var ddlBasis = gvRow.FindControl("ddlBasis") as DropDownList;
                var txtVacationDays = gvRow.FindControl("txtVacationDays") as TextBox;
                txtVacationDays.Enabled = ddlBasis.SelectedIndex == 0;//0 : W2-salary
                txtVacationDays.Text = txtVacationDays.Enabled ? txtVacationDays.Text : 0.ToString();
            }
        }

        protected void custValTitle_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            var custValTitle = sender as CustomValidator;

            var gvRow = custValTitle.NamingContainer as GridViewRow;

            var ddlTitle = gvRow.FindControl("ddlTitle") as DropDownList;

            e.IsValid = ddlTitle.SelectedIndex > 0;
        }

        protected void custValPractice_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            var custValPractice = sender as CustomValidator;

            var gvRow = custValPractice.NamingContainer as GridViewRow;

            var ddlPractice = gvRow.FindControl("ddlPractice") as DropDownList;

            e.IsValid = ddlPractice.SelectedIndex > 0;
        }

        #endregion

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            custCompensationCoversMilestone.Enabled = cvEndCompensation.Enabled = cvHireDateChange.Enabled = custCancelTermination.Enabled = true;
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

        #region Wizards Events

        protected void btnWizardsCancel_Click(object sender, EventArgs e)
        {
            mpeWizardsCancelPopup.Show();
        }

        protected void btnWizardsCancelPopupOKButton_OnClick(object sender, EventArgs e)
        {
            mpeWizardsCancelPopup.Hide();
            Response.Redirect("~/Config/Persons.aspx?ApplyFilterFromCookie=true");
        }

        protected void btnWizardsCancelPopupCancelButton_OnClick(object sender, EventArgs e)
        {
            mpeWizardsCancelPopup.Hide();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (ValidateActiveWizards())
            {
                if (ActiveWizard == _finalWizardView)
                {

                    Save_Click(null, new EventArgs());
                    if (Page.IsValid)
                    {
                        Response.Redirect(Constants.ApplicationPages.PersonsPage);
                    }

                }
                if (ActiveWizard != _finalWizardView)
                {
                    ActiveWizard++;
                    SelectView(rowSwitcher.Cells[ActiveWizard].Controls[0], ActiveWizard, true);
                }
            }
            IsOtherPanelDisplay = personnelCompensation.SLTApprovalPopupDisplayed ? true : IsOtherPanelDisplay;
        }

        #endregion

        #endregion

        #region Methods

        public static void SaveRoles(Person person, string[] currentRoles)
        {
            if (string.IsNullOrEmpty(person.Alias)) return;

            // Saving roles


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

        private bool ValidateActiveWizards()
        {
            int activeindex = mvPerson.ActiveViewIndex;
            int[] activeWizardsArray = ActiveWizardsArray[ActiveWizard];
            Page.Validate(valsPerson.ValidationGroup);
            if (Page.IsValid)
            {
                foreach (int i in activeWizardsArray)
                {
                    SelectView(rowSwitcher.Cells[i].Controls[0], i, true);
                    Page.Validate(valsPerson.ValidationGroup);
                    if (!Page.IsValid)
                    {
                        break;
                    }
                }
            }

            if (!Page.IsValid)
            {
                IsErrorPanelDisplay = true;
            }
            else
            {
                PersonValidation();
                if (Page.IsValid)
                {
                    SelectView(rowSwitcher.Cells[activeindex].Controls[0], activeindex, true);
                }
            }
            return Page.IsValid;
        }

        private void DisableInactiveViews()
        {
            if (IsWizards)
            {
                int[] activeWizardsArray = ActiveWizardsArray[ActiveWizard];
                for (int i = 0; i < tblPersonViewSwitch.Rows[0].Cells.Count; i++)
                {
                    TableCell item = tblPersonViewSwitch.Rows[0].Cells[i];
                    item.Enabled = activeWizardsArray.Any(a => a == i);
                }
            }
            else
            {
                for (int i = 0; i < tblPersonViewSwitch.Rows[0].Cells.Count; i++)
                {
                    TableCell item = tblPersonViewSwitch.Rows[0].Cells[i];
                    item.Enabled = true;
                }
            }

            divCompensationHistory.Visible = !IsWizards;
            divCompensation.Visible = IsWizards;
        }

        private void DisplayWizardButtons()
        {
            btnCancelAndReturn.Visible = btnSave.Visible = !IsWizards;
            btnNext.Visible = btnWizardsCancel.Visible = IsWizards;
            btnNext.Text = ActiveWizard == _finalWizardView ? "Finish" : "Next";
        }

        private void DisableTerminationDateAndReason()
        {
            if (PrevPersonStatusId == (int)PersonStatusType.Active || PrevPersonStatusId == (int)PersonStatusType.Terminated || (PrevPersonStatusId == (int)PersonStatusType.Contingent && dtpTerminationDate.DateValue == DateTime.MinValue))
            {
                if (PrevPersonStatusId != (int)PersonStatusType.Terminated && IsStatusChangeClicked && PersonStatusId == PersonStatusType.Terminated)
                {
                    ddlTerminationReason.Visible = true;
                    txtTerminationReason.Visible = false;
                    ddlTerminationReason.Enabled = false;
                }
                else if (PrevPersonStatusId != (int)PersonStatusType.Terminated)
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
            else if (PrevPersonStatusId == (int)PersonStatusType.TerminationPending || (PrevPersonStatusId == (int)PersonStatusType.Contingent && PreviousTerminationDate.HasValue))
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

        private void HTMLToPdf(String HTML, string filename)
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

        private void ResetToPreviousData()
        {
            var person = GetPerson(PersonId);
            PopulateControls(person);
        }

        private void FillTerminationReasonsByTerminationDate(DatePicker terminationDate, ListControl ddlTerminationReasons)
        {
            var reasons = new List<TerminationReason>();
            if (terminationDate != null)
            {
                ddlTerminationReasons.SelectedValue = string.Empty;
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

        private void FillAllPersonStatuses(PersonStatusType PersonStatusId)
        {
            DataHelper.FillPersonStatusList(ddlPersonStatus);
            ddlPersonStatus.SelectedValue = ((int)PersonStatusId).ToString();
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
                IsErrorPanelDisplay = true;

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

            if (custCancelTermination.Enabled && Page.IsValid)
            {
                custCancelTermination.Validate();
                SelectView(rowSwitcher.Cells[activeindex].Controls[0], activeindex, true);
            }

            if (!_disableValidatecustTerminateDateTE && Page.IsValid)
            {
                custTerminateDateTE.Enabled = true;
                custTerminateDateTE.Validate();
                //Page.Validate(valsPerson.ValidationGroup);
                SelectView(rowSwitcher.Cells[activeindex].Controls[0], activeindex, true);
            }
            if (cvSLTApproval.Enabled && Page.IsValid)
            {
                cvSLTApproval.Validate();
            }
            if (cvSLTPTOApproval.Enabled && Page.IsValid)
            {
                cvSLTPTOApproval.Validate();
            }
            if (!Page.IsValid)
            {
                IsErrorPanelDisplay = true;
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
                    hdTitleChanged.Value = 0.ToString();
                    return true;
                }
            }
            return false;
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
                var permissions = DataHelper.GetPermissions(new Person { Id = PersonId.Value });
                rpPermissions.ApplyPermissions(permissions);
            }
        }

        private void ShowPermissionsPerPage()
        {
            var permDiff = new List<PermissionDiffeneceItem>();

            IPrincipal userNew =
                new GenericPrincipal(new GenericIdentity(Email), GetSelectedRoles().ToArray());
            IPrincipal userCurrent =
                new GenericPrincipal(new GenericIdentity(Email),
                                     Roles.GetRolesForUser(Email));

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

        protected override void Display()
        {
            IsStatusChangeClicked = false;
            if (Request.QueryString["ShowConfirmMessage"] != null && Request.QueryString["ShowConfirmMessage"] == "1")
            {
                mlConfirmation.ShowInfoMessage(string.Format(Resources.Messages.SavedDetailsConfirmation, "Person"));
                IsErrorPanelDisplay = true;
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
            DataHelper.FillSenioritiesList(ddlSeniority, "-- Select Seniority --");
            DataHelper.FillPracticeListOnlyActive(ddlDefaultPractice, "-- Select Practice Area --");
            DataHelper.FillRecruiterList(ddlRecruiter, "--Select Recruiter--");
            DataHelper.FillPersonDivisionList(ddlDivision);
            ddlDivision.SelectedValue = "2";//Division default value is 'Consulting'
            DataHelper.FillTitleList(ddlPersonTitle, "-- Select Title --");
            DataHelper.FillDomainsList(ddlDomain);
            txtFirstName.Focus();

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
            }
            else // Add new person mode
            {
                // Hide Employee Number related controls
                lblEmployeeNumber.Visible =
                txtEmployeeNumber.Visible =
                reqEmployeeNumber.Enabled =
                btnResetPassword.Visible = false;
            }
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

        #region Populate controls

        private void PopulateControls(Person person)
        {
            //  Names, email, dates etc.
            PopulateBasicData(person);

            // Payment history
            PopulatePayment(person);

            // Role/Seniority
            PopulateRolesAndSeniority(person);

            // EmploymentHistory
            PopulateEmploymentHistory(person);
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

        private void PopulatePayment(Person person)
        {
            PopulatePayment(person.PaymentHistory);
        }

        private void PopulatePayment(List<Pay> paymentHistory)
        {
            var now = Utils.Generic.GetNowWithTimeZone();
            //if person status is active or terminated pending or contigent and does not have current pay then we need to show last pay as editable.
            if (PersonStatusId.HasValue && (PersonStatusId.Value != PersonStatusType.Terminated)
                && !paymentHistory.Any(p => p.StartDate <= now.Date && (!p.EndDate.HasValue || (p.EndDate.HasValue && now.Date <= p.EndDate.Value.AddDays(-1)))))
            {
                Pay pay = paymentHistory.OrderByDescending(p => p.StartDate).FirstOrDefault(p => p.StartDate < now.Date);
                _editablePayStartDate = pay != null && (!PersonHireDate.HasValue || (PersonHireDate.HasValue && pay.StartDate >= PersonHireDate.Value.Date)) ? pay.StartDate : (DateTime?)null;
            }
            gvCompensationHistory.DataSource = paymentHistory;
            gvCompensationHistory.DataBind();
        }

        private void PopulateEmploymentHistory(Person person)
        {
            gvEmploymentHistory.DataSource = person.EmploymentHistory;
            gvEmploymentHistory.DataBind();
        }

        private void PopulateRecruiterDropDown(Person person)
        {
            if (person.RecruiterId.HasValue)
            {
                ListItem selectedRecruiter = ddlRecruiter.Items.FindByValue(person.RecruiterId.Value.ToString());
                if (selectedRecruiter == null)
                {
                    var recruiter = ServiceCallers.Custom.Person(p => p.GetPersonDetailsShort(person.RecruiterId.Value));
                    selectedRecruiter = new ListItem(recruiter.PersonLastFirstName, person.RecruiterId.Value.ToString());
                    ddlRecruiter.Items.Add(selectedRecruiter);
                    ddlRecruiter.SortByText();
                }
                ddlRecruiter.SelectedValue = selectedRecruiter.Value;
            }
        }

        private void PopulateBasicData(Person person)
        {
            txtFirstName.Text = person.FirstName;
            txtLastName.Text = person.LastName;
            PersonHireDate = dtpHireDate.DateValue = person.HireDate;
            PreviousHireDate = person.HireDate;
            PreviousTerminationDate = (person.EmploymentHistory != null && person.EmploymentHistory.Count > 0) ? person.EmploymentHistory.Last().TerminationDate : null;

            //Last but one Termination date for Hire Date validation.
            TerminationDateBeforeCurrentHireDate = person.EmploymentHistory.Any(p => p.HireDate.Date < person.HireDate.Date) ? person.EmploymentHistory.Last(p => p.HireDate.Date < person.HireDate.Date).TerminationDate : null;

            //Populate PersonStatus.
            PersonStatusId = person.Status != null ? (PersonStatusType?)person.Status.Id : null;
            PrevPersonStatusId = (person.Status != null) ? person.Status.Id : -1;
            lblPersonStatus.Text = PersonStatusId.HasValue ? DataHelper.GetDescription((PersonStatusType)PrevPersonStatusId) : string.Empty;

            //Populate Termination date and termination reason.
            PopulateTerminationDate(person.TerminationDate);
            FillTerminationReasonsByTerminationDate(dtpTerminationDate, ddlTerminationReason);
            PopulateTerminationReason(person.TerminationReasonid);

            txtEmailAddress.Text = person.AliasWithoutDomain;
            if (ddlDomain.Items.FindByValue(person.Domain) != null)
                ddlDomain.SelectedValue = person.Domain;
            else if (!string.IsNullOrEmpty(person.Domain))
            {
                ddlDomain.Items.Add(new ListItem() { Text = person.Domain, Value = person.Domain });
                ddlDomain.SelectedValue = person.Domain;
            }
            txtTelephoneNumber.Text = person.TelephoneNumber.Trim();
            ddlPersonType.SelectedValue = person.IsOffshore ? "1" : "0";
            txtPayCheckId.Text = string.IsNullOrEmpty(person.PaychexID) ? "" : person.PaychexID;
            if ((int)person.DivisionType != 0)
            {
                ddlDivision.SelectedValue = ((int)person.DivisionType).ToString();
            }

            PopulatePracticeDropDown(person);
            PopulateRecruiterDropDown(person);

            txtEmployeeNumber.Text = person.EmployeeNumber;

            //Set Locked-Out CheckBox value
            chbLockedOut.Checked = person.LockedOut;
            defaultManager.EnsureDatabound();
            // Select manager and exclude self from the list
            defaultManager.SelectedManager = person.Manager;

            if (person.IsDefaultManager)
            {
                this.hdnIsDefaultManager.Value = person.IsDefaultManager.ToString();
            }

            if (person.Title != null)
            {
                ddlPersonTitle.SelectedValue = person.Title.TitleId.ToString();
            }
            hdTitleChanged.Value = 0.ToString();
            hdcvSLTApproval.Value = person.SLTApproval.ToString();
            hdcvSLTPTOApproval.Value = person.SLTPTOApproval.ToString();
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

            person.Alias = Email;
            person.TelephoneNumber = txtTelephoneNumber.Text;

            person.EmployeeNumber = txtEmployeeNumber.Text;

            person.Status = new PersonStatus { Id = (int)PersonStatusId };

            //Set Locked-Out value
            person.LockedOut = IsLockOut;

            person.DefaultPractice =
                !string.IsNullOrEmpty(ddlDefaultPractice.SelectedValue) ?
                new Practice { Id = int.Parse(ddlDefaultPractice.SelectedValue) } : null;

            int recruiterID;
            person.RecruiterId = int.TryParse(ddlRecruiter.SelectedValue, out recruiterID) ? (int?)recruiterID : null;
            if (ddlPersonTitle.SelectedIndex != 0)
            {
                person.Title = new Title() { TitleId = int.Parse(ddlPersonTitle.SelectedValue), TitleName = ddlPersonTitle.SelectedItem.Text };
            }
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

            if (IsWizards)
            {
                int[] activeWizardsArray = ActiveWizardsArray[ActiveWizard];
                if (activeWizardsArray.Any(a => mvPerson.Views[a] == vwCompensation))
                {
                    person.CurrentPay = personnelCompensation.Pay;
                }
            }
            else
            {
                var today = (SettingsHelper.GetCurrentPMTime()).Date;
                person.CurrentPay = PayHistory.FirstOrDefault(c => today >= c.StartDate && (!c.EndDate.HasValue || today < c.EndDate)) ?? PayHistory.FirstOrDefault(c => today < c.StartDate);
                person.SLTApproval = person.CurrentPay != null && person.CurrentPay.Timescale == TimescaleType.Salary && bool.Parse(hdcvSLTApproval.Value);
                person.SLTPTOApproval = person.CurrentPay != null && person.CurrentPay.Timescale == TimescaleType.Salary && bool.Parse(hdcvSLTPTOApproval.Value);
            }
        }

        private List<string> GetSelectedRoles()
        {
            return (from ListItem item in chblRoles.Items where item.Selected select item.Text).ToList();
        }

        private int? SaveData()
        {
            var person = new Person();
            PopulateData(person);

            if (PersonId.HasValue && PrevPersonStatusId == (int)PersonStatusType.Terminated && (PersonStatusId.Value == PersonStatusType.Active || PersonStatusId.Value == PersonStatusType.Contingent))
            {
                TransferToCompesationDetailPage(person);
            }
            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    Person oldPerson = null;
                    if (person.Id.HasValue)
                    {
                        oldPerson = serviceClient.GetPersonDetailsShort(person.Id.Value);
                    }
                    string[] currentRoles = Roles.GetRolesForUser(person.Alias);
                    if (oldPerson != null)
                    {
                        if (currentRoles.Length == 0)
                            currentRoles = Roles.GetRolesForUser(oldPerson.Alias);
                        oldPerson.RoleNames = currentRoles;
                    }
                    int? personId = serviceClient.SavePersonDetail(person, User.Identity.Name, LoginPageUrl, IsWizards);
                    SaveRoles(person, currentRoles);

                    serviceClient.SendAdministratorAddedEmail(person, oldPerson);
                    if (personId.Value < 0)
                    {
                        // Creating User error
                        _saveCode = personId.Value;
                        Page.Validate();
                        if (!Page.IsValid)
                        {
                            IsErrorPanelDisplay = true;
                        }
                        return null;
                    }

                    SavePersonsPermissions(person, serviceClient);

                    IsDirty = IsStatusChangeClicked = false;
                    return personId;
                }
                catch (Exception ex)
                {
                    serviceClient.Abort();
                    ExMessage = ex.Message;
                    Page.Validate();

                    if (!Page.IsValid)
                    {
                        IsErrorPanelDisplay = true;
                    }
                    else
                    {
                        Logging.LogErrorMessage(
                            ex.Message,
                            ex.Source,
                            ex.InnerException != null ? ex.InnerException.Message : string.Empty,
                            string.Empty,
                            HttpContext.Current.Request.Url.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped),
                            string.Empty,
                            Thread.CurrentPrincipal.Identity.Name);
                    }
                }
            }
            return null;
        }

        private void PersonValidation()
        {
            var person = new Person();
            PopulateData(person);

            using (var serviceClient = new PersonServiceClient())
            {
                try
                {
                    serviceClient.PersonValidations(person);
                }
                catch (Exception ex)
                {
                    serviceClient.Abort();
                    ExMessage = ex.Message;
                    Page.Validate();

                    if (!Page.IsValid)
                    {
                        IsErrorPanelDisplay = true;
                    }
                }
            }
        }

        private void TransferToCompesationDetailPage(Person person)
        {
            //To transfer the unsave person data to Compensation detail page.
            person.PaymentHistory = PayHistory;
            PersonUnsavedData = person;
            if (bool.Parse(hfReloadPerms.Value))
            {
                Permissions = rpPermissions.GetPermissions();
            }
            Server.Transfer("~/CompensationDetail.aspx?Id=" + PersonUnsavedData.Id + "&returnTo=" + Request.Url, true);
        }

        private void SavePersonsPermissions(Person person, PersonServiceClient serviceClient)
        {
            if (bool.Parse(hfReloadPerms.Value))
            {
                PersonPermission permissions = rpPermissions.GetPermissions();

                serviceClient.SetPermissionsForPerson(person, permissions);
            }
        }

        private void PopulateUnCommitedPay(List<Pay> payHistory)
        {
            Pay pay = null;
            GridViewRow gvRow = null;
            if (gvCompensationHistory.EditIndex != -1)
            {
                gvRow = gvCompensationHistory.Rows[gvCompensationHistory.EditIndex];
            }
            else
            {
                if (gvCompensationHistory.ShowFooter)
                {
                    gvRow = gvCompensationHistory.FooterRow;
                }
            }
            if (gvRow != null)
            {
                pay = new Pay();

                DateTime startDate;
                var dpStartDate = gvRow.FindControl("dpStartDate") as DatePicker;
                var dpEndDate = gvRow.FindControl("dpEndDate") as DatePicker;
                var imgUpdate = gvRow.FindControl("imgUpdateCompensation") as ImageButton;
                var ddlBasis = gvRow.FindControl("ddlBasis") as DropDownList;
                var operation = Convert.ToString(imgUpdate.Attributes["operation"]);

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
                var index = 0;
                if (operation == "Update")
                {
                    startDate = Convert.ToDateTime(imgUpdate.Attributes["StartDate"]);
                    index = PayHistory.FindIndex(p => p.StartDate.Date == startDate);
                    payHistory[index] = pay;
                }
                else
                {
                    payHistory.Add(pay);
                }
            }
        }

        private void PopulateErrorPanel()
        {
            mpeErrorPanel.Show();
        }

        #endregion

    }
}


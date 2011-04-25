using System;
using System.Drawing;
using System.Web.Security;
using System.Web.UI.WebControls;
using DataTransferObjects.TimeEntry;
using PraticeManagement.Utils;

namespace PraticeManagement.Controls.TimeEntry
{
    public partial class SingleTimeEntry : System.Web.UI.UserControl
    {
        #region Constants

        private const string DateBehindViewstate = "7555B3A7-8713-490F-8D5B-368A02E6A205";
        private const string TimeentryBehindViewstate = "6BD8832C-A37A-497F-82A9-58A40C759499";
        private const string ScriptKey = "6BD8832C-A37A-497F-82A9-58A40C759499";
        private const string Note_ErrorMessageKey = "Note_ErrorMessage";
        private const string Hours_ErrorMessageKey = "Hours_ErrorMessage";

        private const string StringLengthValidationScript = @"
               function ValidateStringLength(source, arguments)
               {
                    var slen = arguments.Value.length;
                    // alert(arguments.Value + '\n' + slen);
                    if (slen >= 3 && slen <= 1000){
                        arguments.IsValid = true;
                    } else {
                        arguments.IsValid = false;
                    }
               }
        ";

        #endregion

        #region Properties

        protected string[] UserRoles { get { return Roles.GetRolesForUser(); } }

        protected bool UserIsAdmin
        {
            get
            {
                return Array.FindIndex(
                    UserRoles, 
                    r => r == DataTransferObjects.Constants.RoleNames.AdministratorRoleName) >= 0;
            }
        }

        protected bool UserIsConsultant
        {
            get
            {
                return Array.FindIndex(
                    UserRoles,
                    r => r == DataTransferObjects.Constants.RoleNames.ConsultantRoleName) >= 0;
            }
        }

        public TimeEntryRecord TimeEntryBehind
        {
            get
            {
                return ViewState[TimeentryBehindViewstate] as TimeEntryRecord;
            }
            set
            {
                ViewState[TimeentryBehindViewstate] = value;
            }
        }

        public DateTime DateBehind
        {
            get
            {
                return (DateTime) ViewState[DateBehindViewstate];
            }
            set
            {
                ViewState[DateBehindViewstate] = value;
            }
        }

        public bool IsChargeable
        {
            set
            {
                chbIsChargeable.Checked = value;
                hdnDefaultIsChargeable.Value = hdnIsChargeable.Value = value.ToString().ToLower();
            }
        }

        public string VerticalTotalCalculatorExtenderId
        {
            get
            {
                return hfVerticalTotalCalculatorExtender.Value;
            }
            set
            {
                hfVerticalTotalCalculatorExtender.Value = value;
            }
        }

        public string ProjectMilestoneDropdownClientId
        {
            set
            {
                this.deActualHours.ProjectMilestoneDropdownIdValue = value;
            }
        }

        public string TimeTypeDropdownClientId
        {
            set
            {
                this.deActualHours.TimeTypeDropdownId = value;
            }
        }

        public string HorizontalTotalCalculatorExtenderId
        {
            get
            {
                return hfHorizontalTotalCalculatorExtender.Value;
            }
            set
            {
                hfHorizontalTotalCalculatorExtender.Value = value;
            }
        }

        public string SpreadSheetTotalCalculatorExtenderId
        {
            get
            {
                return hfSpreadSheetTotalCalculatorExtender.Value;
            }
            set
            {
                hfSpreadSheetTotalCalculatorExtender.Value = value;
            }
        }

        #endregion

        #region Custom Events

        public delegate bool ReadyToUpdateTeHandler(object sender, ReadyToUpdateTeArguments args);
        public event ReadyToUpdateTeHandler ReadyToUpdateTE;

        protected virtual bool OnReadyToUpdateTE()
        {
            try
            {
                if (ReadyToUpdateTE(
                       this,
                       new ReadyToUpdateTeArguments(TimeEntryBehind)))
                {
                    mlMessage.ShowInfoMessage(Resources.Messages.TimeEntryUpdatedSuccessfully);
                    BecomeGreen();
                    hfDirtyHours.Value = string.Empty;
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception exc)
            {
                BecomeRed(exc.Message);
                return false;
            }
        }

        private void BecomeGreen()
        {
            tbActualHours.BackColor = Color.LightGreen;
        }

        private void BecomeRed(string message)
        {
            tbActualHours.BackColor = Color.Red;
            mlMessage.ShowErrorMessage(message);
        }

        #endregion

        #region Control events


        protected void Page_Load(object sender, EventArgs e)
        {
            //  Postback check is not needed here
            //  because we're using partial update and need
            //  to register scripts on postback also.
            AddSctipts();            

            tbActualHours.TabIndex = TimeEntries.TabIndex;
            CanelControlStyle();

            ApplyControlStyle();
        }

        public void CanelControlStyle()
        {
            tbActualHours.BackColor = Color.White;
        }

        protected void btnSaveTE_Click(object sender, EventArgs e)
        {
            SaveTimeEntry();
        }

        public bool SaveTimeEntry()
        {

            if (!Modified) return true;

            if (string.IsNullOrEmpty(this.tbNotes.Text) 
                && string.IsNullOrEmpty(this.tbActualHours.Text) 
                && this.TimeEntryBehind != null
               )
            {
                    TimeEntryHelper.RemoveTimeEntry(this.TimeEntryBehind);
                    return true;
            }

            bool isValidNote = IsValidNote();
            bool isValidHours = IsValidHours();

            if (isValidNote && isValidHours)
            {
                InitTimeEntryBehind();  //  Fill object with values from controls
                return OnReadyToUpdateTE();    //  Fire event to the bar
            }
            else
            {
                tbActualHours.BackColor = Color.Red;
                return false;
            }
        }


        private bool IsValidNote()
        {
            imgNote.ImageUrl = string.IsNullOrEmpty(tbNotes.Text) ?
                        PraticeManagement.Constants.ApplicationResources.AddCommentIcon :
                        PraticeManagement.Constants.ApplicationResources.RecentCommentIcon;

            var note = tbNotes.Text;
            if (string.IsNullOrEmpty(note) || note.Length < 3 || note.Length > 1000)
            {
                Session[Note_ErrorMessageKey] = "Note";
                return false;
            }
            return true;
        }
        private bool IsValidHours()
        {
            double hours;
            if (string.IsNullOrEmpty(tbActualHours.Text))
            {
                Session[Hours_ErrorMessageKey] = "Hours";
                return false;
            }
            //  Check that hours is double between 0.0 and 24.0
            if (double.TryParse(tbActualHours.Text, out hours))
            {
                if (hours >= 0.0 && hours <= 24.0)
                {
                    return true;
                }
            }
            Session[Hours_ErrorMessageKey] = "Hours";
            return false;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var existingTimeEntry = TimeEntryBehind != null;
            if (existingTimeEntry && !Modified)
                FillControls();
        }

        protected void cvNotes_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            var note = args.Value;
            if (note.Length < 3 || note.Length > 1000)
                args.IsValid = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Include string length validation script if it's not already included
        /// </summary>
        private void AddSctipts()
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered(ScriptKey))
            {
                Page.ClientScript.RegisterClientScriptBlock(
                    GetType(), ScriptKey, StringLengthValidationScript, true);
            }
        }

        public void ShowWarningMessage(string message)
        {
            mlMessage.ShowWarningMessage(message);
        }

        public bool ChargeabilityEnabled
        {
            set
            {
                //  Shows/hides chargeability if the user is not admin
                //  and corresponding value is provided
                chbIsChargeable.Enabled = UserIsAdmin || value;
            }
        }

        private void FillControls()
        {
            EnsureChildControls();
            tbNotes.Text = TimeEntryBehind.Note;
            hdnNotes.Value = TimeEntryBehind.Note;
            imgNote.ImageUrl
                = string.IsNullOrEmpty(tbNotes.Text)
                      ? Constants.ApplicationResources.AddCommentIcon
                      : Constants.ApplicationResources.RecentCommentIcon;

            lblEntryDate.Text =
                TimeEntryBehind.EntryDate.ToString(
                    Constants.Formatting.EntryDateFormat);
            tbActualHours.Text = TimeEntryBehind.ActualHours.ToString(Constants.Formatting.DoubleFormat);
            hdnActualHours.Value = tbActualHours.Text;
            chbIsChargeable.Checked = TimeEntryBehind.IsChargeable;
            hdnIsChargeable.Value = chbIsChargeable.Checked.ToString().ToLower();
            chbForDiffProject.Checked = !TimeEntryBehind.IsCorrect;
            hdnForDiffProject.Value = chbForDiffProject.Checked.ToString().ToLower();
            ReviewStatus reviewStatus = TimeEntryBehind.IsReviewed;
            lblReview.Text = reviewStatus.ToString();

            imgNote.ToolTip = TimeEntryBehind.Note;

            if (TimeEntryBehind.IsReviewed == ReviewStatus.Approved)
                Disabled = true;
        }

        private void InitTimeEntryBehind()
        {
            if (TimeEntryBehind == null)
                TimeEntryBehind = new TimeEntryRecord { MilestoneDate = DateBehind };

            TimeEntryBehind.Note = tbNotes.Text;
            TimeEntryBehind.EntryDate =
                Utils.Generic.ParseDate(lblEntryDate.Text);
            TimeEntryBehind.ActualHours =
                Convert.ToDouble(tbActualHours.Text);
            TimeEntryBehind.IsChargeable = chbIsChargeable.Checked;
            TimeEntryBehind.IsCorrect = !chbForDiffProject.Checked;
        }

        protected string GetNowDate()
        {
            return DateTime.Now.ToString(Constants.Formatting.EntryDateFormat);
        }

        public void UpdatehiddenHours()
        {
            this.hfDirtyHours.Value = this.tbActualHours.Text;
        }

        public bool ForceIncorrect
        {
            set
            {
                chbForDiffProject.Checked = true;
                chbForDiffProject.Enabled = !value;
            }
        }

        private void ApplyControlStyle()
        {
            if (tbActualHours.ReadOnly)
                tbActualHours.BackColor = Color.Gray;
        }

        public bool Disabled
        {
            set
            {
                var enabled = !value;

                tbNotes.Enabled =
                chbIsChargeable.Enabled =
                chbForDiffProject.Enabled =
                tbActualHours.Enabled = enabled;

                tbActualHours.ReadOnly = value;

                ApplyControlStyle();
            }
        }

        /// <summary>
        /// Whether user had entered any text into the control
        /// </summary>
        public bool Modified
        {
            get
            {
                return !string.IsNullOrEmpty(hfDirtyHours.Value);
            }
        }

        #endregion
    }
}


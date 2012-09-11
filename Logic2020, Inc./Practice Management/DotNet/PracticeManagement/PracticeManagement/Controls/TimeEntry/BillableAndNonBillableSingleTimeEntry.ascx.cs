using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Drawing;
using DataTransferObjects.TimeEntry;
using PraticeManagement.Utils;

namespace PraticeManagement.Controls.TimeEntry
{
    public partial class BillableAndNonBillableSingleTimeEntry : System.Web.UI.UserControl
    {
        #region Constants

        private const string DateBehindViewstate = "7555B3A7-8713-490F-8D5B-368A02E6A205";
        private const string imgNoteClientIdAttribute = "imgNoteClientId";
        private const string IsPTOAttribute = "IsPTO";
        private const string txtboxNoteClienIdAttribute = "txtboxNoteClienId";
        private const string isChargeCodeTurnOffDisableAttribute = "isChargeCodeTurnOffDisable";
        private const string isHourlyRevenueDisableAttribute = "isHourlyRevenueDisable";
        private const string IsEmpDisableAttribute = "IsEmpDisable";
         
        #endregion

        #region Properties


        public bool IsPTO { get; set; }

        public XElement TimeEntryRecordBillableElement
        {
            get;
            set;
        }

        public XElement TimeEntryRecordNonBillableElement
        {
            get;
            set;
        }

        public bool IsThereAtleastOneTimeEntryrecord
        {
            get
            {
                return ((!string.IsNullOrEmpty(tbNotes.Text)) || (!string.IsNullOrEmpty(tbBillableHours.Text)) || (!string.IsNullOrEmpty(tbNonBillableHours.Text)));
            }
        }

        public DateTime DateBehind
        {
            get
            {
                return (DateTime)ViewState[DateBehindViewstate];
            }
            set
            {
                ViewState[DateBehindViewstate] = value;
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

        public TimeEntry_New HostingPage
        {
            get
            {
                return ((TimeEntry_New)Page);
            }
        }


        public string IsNoteRequired
        {
            set
            {
                hdnIsNoteRequired.Value = value;
            }
            get
            {
                return hdnIsNoteRequired.Value;
            }
        }

        public string IsHourlyRevenue
        {
            set
            {
                hdnIsHourlyRevenue.Value = value;
            }
            get
            {
                return hdnIsHourlyRevenue.Value;
            }
        }

        public string IsChargeCodeTurnOff
        {
            get
            {
                return hdnIsChargeCodeTurnOff.Value;
            }
            set
            {
                hdnIsChargeCodeTurnOff.Value = value;
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

        public string BillableTextBoxClientId
        {
            get
            {
                return tbBillableHours.ClientID;
            }
        }

        public string NonBillableTextBoxClientId
        {
            get
            {
                return tbNonBillableHours.ClientID;
            }
        }

        #endregion

        #region Control events

        protected void Page_Load(object sender, EventArgs e)
        {
            tbNotes.Attributes[imgNoteClientIdAttribute] = imgNote.ClientID; 
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            SpreadSheetTotalCalculatorExtenderId = HostingPage.SpreadSheetTotalCalculatorExtenderId;
        }

        public void CanelControlStyle()
        {
            tbBillableHours.BackColor = Color.White;
            tbNonBillableHours.BackColor = Color.White;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (TimeEntryRecordBillableElement != null)
            {
                FillBillableControls();
            }

            if (TimeEntryRecordNonBillableElement != null)
            {
                FillNonBillableControls();
            }

            CanelControlStyle();
            ApplyControlStyle();

            tbBillableHours.Attributes[IsPTOAttribute] = IsPTO.ToString();
            tbBillableHours.Attributes[txtboxNoteClienIdAttribute] = tbNotes.ClientID;
            tbNonBillableHours.Attributes[IsPTOAttribute] = IsPTO.ToString();
            tbNonBillableHours.Attributes[txtboxNoteClienIdAttribute] = tbNotes.ClientID;

            MaintainEditedtbHoursStyle();

            tbNonBillableHours.Attributes[isHourlyRevenueDisableAttribute] = Convert.ToBoolean(IsHourlyRevenue) ? "0" : "1";
            tbNonBillableHours.Attributes[isChargeCodeTurnOffDisableAttribute] = Convert.ToBoolean(IsChargeCodeTurnOff) ? "1" : "0";
            tbBillableHours.Attributes[isChargeCodeTurnOffDisableAttribute] = Convert.ToBoolean(IsChargeCodeTurnOff) ? "1" : "0";

            tbNonBillableHours.Attributes[IsEmpDisableAttribute] =
            tbBillableHours.Attributes[IsEmpDisableAttribute] = HostingPage.IsDateInPersonEmployeeHistoryList[DateBehind.Date]  ? "0" : "1";
        }

        private void MaintainEditedtbHoursStyle()
        {

            if (tbBillableHours.Style["background-color"] != "red")
            {
                tbBillableHours.Style["background-color"] = (hfDirtyBillableHours.Value == "dirty") ? "gold" : "none";
            }

            if (tbNonBillableHours.Style["background-color"] != "red")
            {
                tbNonBillableHours.Style["background-color"] = (hfDirtyNonBillableHours.Value == "dirty") ? "gold" : "none";
            }
        }

        protected void cvNotes_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            var note = args.Value;
            if (note.Length < 3 || note.Length > 1000)
                args.IsValid = false;
        }

        #endregion

        #region Methods

        public void ShowWarningMessage(string message)
        {
            mlMessage.ShowWarningMessage(message);
        }

        private void FillBillableControls()
        {
            EnsureChildControls();
            tbNotes.Text = TimeEntryRecordBillableElement.Attribute(XName.Get(TimeEntry_New.NoteXname)).Value;
            hdnNotes.Value = TimeEntryRecordBillableElement.Attribute(XName.Get(TimeEntry_New.NoteXname)).Value;
            imgNote.ImageUrl
                = string.IsNullOrEmpty(tbNotes.Text)
                      ? Constants.ApplicationResources.AddCommentIcon
                      : Constants.ApplicationResources.RecentCommentIcon;


            tbBillableHours.Text = TimeEntryRecordBillableElement.Attribute(XName.Get(TimeEntry_New.ActualHoursXname)).Value;

            hdnBillableHours.Value = tbBillableHours.Text;

            var isReviewd = TimeEntryRecordBillableElement.Attribute(XName.Get(TimeEntry_New.IsReviewedXname)).Value;
            lblReview.Text = isReviewd;
            if (isReviewd == ReviewStatus.Approved.ToString())
                Disabled = true;

            hfDirtyBillableHours.Value = TimeEntryRecordBillableElement.Attribute(XName.Get(TimeEntry_New.IsDirtyXname)).Value;

            imgNote.ToolTip = tbNotes.Text;
        }

        private void FillNonBillableControls()
        {
            EnsureChildControls();
            tbNotes.Text = TimeEntryRecordNonBillableElement.Attribute(XName.Get(TimeEntry_New.NoteXname)).Value.Trim();
            hdnNotes.Value = TimeEntryRecordNonBillableElement.Attribute(XName.Get(TimeEntry_New.NoteXname)).Value.Trim();
            imgNote.ImageUrl
                = string.IsNullOrEmpty(tbNotes.Text)
                      ? Constants.ApplicationResources.AddCommentIcon
                      : Constants.ApplicationResources.RecentCommentIcon;

            tbNonBillableHours.Text = TimeEntryRecordNonBillableElement.Attribute(XName.Get(TimeEntry_New.ActualHoursXname)).Value;


            hdnNonBillableHours.Value = tbNonBillableHours.Text;

            var isReviewd = TimeEntryRecordNonBillableElement.Attribute(XName.Get(TimeEntry_New.IsReviewedXname)).Value;
            lblReview.Text = isReviewd;
            if (isReviewd == ReviewStatus.Approved.ToString())
                Disabled = true;

            hfDirtyNonBillableHours.Value = TimeEntryRecordNonBillableElement.Attribute(XName.Get(TimeEntry_New.IsDirtyXname)).Value;

            imgNote.ToolTip = tbNotes.Text;

            MaintainEditedtbHoursStyle();
        }

        protected string GetNowDate()
        {
            return DateTime.Now.ToString(Constants.Formatting.EntryDateFormat);
        }

        private void ApplyControlStyle()
        {
            if (!tbBillableHours.Enabled && string.IsNullOrEmpty(tbBillableHours.Text))
                tbBillableHours.BackColor = Color.Gray;

            if (!tbNonBillableHours.Enabled && string.IsNullOrEmpty(tbNonBillableHours.Text))
                tbNonBillableHours.BackColor = Color.Gray;
        }

        public bool Disabled
        {
            set
            {
                var enabled = !value;

                btnSaveNotes.Enabled =
                tbNotes.Enabled =
                tbBillableHours.Enabled = tbNonBillableHours.Enabled = enabled;
            }
        }

        internal void UpdateBillableElementEditedValues(XElement element)
        {
            if (element.HasAttributes && element.Attribute(XName.Get(TimeEntry_New.ActualHoursXname)) != null)
            {
                element.Attribute(XName.Get(TimeEntry_New.ActualHoursXname)).Value = tbBillableHours.Text;
                element.Attribute(XName.Get(TimeEntry_New.NoteXname)).Value = tbNotes.Text.Trim();
                element.Attribute(XName.Get(TimeEntry_New.IsDirtyXname)).Value = hfDirtyBillableHours.Value;
            }
            else
            {
                var time = SettingsHelper.GetCurrentPMTime();
                element.SetAttributeValue(XName.Get(TimeEntry_New.ActualHoursXname), tbBillableHours.Text);
                element.SetAttributeValue(XName.Get(TimeEntry_New.NoteXname), tbNotes.Text.Trim());
                element.SetAttributeValue(XName.Get(TimeEntry_New.EntryDateXname), time.ToString(Constants.Formatting.EntryDateFormat));
                element.SetAttributeValue(XName.Get(TimeEntry_New.IsDirtyXname), hfDirtyBillableHours.Value);
            }
        }

        internal void UpdateNonBillableElementEditedValues(XElement element)
        {
            if (element.HasAttributes && element.Attribute(XName.Get(TimeEntry_New.ActualHoursXname)) != null)
            {
                element.Attribute(XName.Get(TimeEntry_New.ActualHoursXname)).Value = tbNonBillableHours.Text;
                element.Attribute(XName.Get(TimeEntry_New.NoteXname)).Value = tbNotes.Text.Trim();
                element.Attribute(XName.Get(TimeEntry_New.IsDirtyXname)).Value = hfDirtyNonBillableHours.Value;
            }
            else
            {
                var time = SettingsHelper.GetCurrentPMTime();
                element.SetAttributeValue(XName.Get(TimeEntry_New.ActualHoursXname), tbNonBillableHours.Text);
                element.SetAttributeValue(XName.Get(TimeEntry_New.NoteXname), tbNotes.Text.Trim());
                element.SetAttributeValue(XName.Get(TimeEntry_New.EntryDateXname), time.ToString(Constants.Formatting.EntryDateFormat));
                element.SetAttributeValue(XName.Get(TimeEntry_New.IsDirtyXname), hfDirtyNonBillableHours.Value);
            }
        }

        internal void UpdateVerticalTotalCalculatorExtenderId(string clientId)
        {
            VerticalTotalCalculatorExtenderId = clientId;
        }

        internal void ValidateNoteAndHours()
        {
            var isValidNote = IsValidNote();
            var isValidBillableHours = IsValidBillableHours();
            var isValidNonBillableHours = IsValidNonBillableHours();

            if (!isValidNote)
            {
                HostingPage.IsValidNote = isValidNote;

                if (IsNoteRequired.ToLower() == "false")
                {
                    HostingPage.IsNoteRequiredBecauseOfNonBillable = true;
                }
              
            }

            if (!(isValidBillableHours && isValidNonBillableHours))
                HostingPage.IsValidHours = isValidBillableHours && isValidNonBillableHours;

            if (isValidNote && isValidBillableHours && isValidNonBillableHours)
            {
                tbBillableHours.Style["background-color"] = "none";

            }
            else
            {
                tbBillableHours.Style["background-color"] = "red";
            }


            if (isValidNote && isValidBillableHours && isValidNonBillableHours)
            {
                tbNonBillableHours.Style["background-color"] = "none";
            }
            else
            {
                tbNonBillableHours.Style["background-color"] = "red";
            }

        }

        private bool IsValidNote()
        {
            imgNote.ImageUrl = string.IsNullOrEmpty(tbNotes.Text.Trim()) ?
                        PraticeManagement.Constants.ApplicationResources.AddCommentIcon :
                        PraticeManagement.Constants.ApplicationResources.RecentCommentIcon;

            var note = tbNotes.Text.Trim();

           
            if (!string.IsNullOrEmpty(tbNonBillableHours.Text) //For Non-Billable Note is Mandatory
                || (hdnIsNoteRequired.Value.ToLowerInvariant() == "true" && (!string.IsNullOrEmpty(tbBillableHours.Text)))
                
                )
            {
                if (string.IsNullOrEmpty(note) || note.Length < 3 || note.Length > 1000)
                {
                    return false;
                }
            }
            else if (!string.IsNullOrEmpty(note))
            {
                if (note.Length < 3 || note.Length > 1000)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidNonBillableHours()
        {
            double hours;
            if (string.IsNullOrEmpty(tbNonBillableHours.Text))
            {
                if (string.IsNullOrEmpty(tbNotes.Text) || !string.IsNullOrEmpty(tbBillableHours.Text))
                {
                    return true;
                }

                return false;
            }
            //  Check that hours is double between 0.0 and 24.0
            if (double.TryParse(tbNonBillableHours.Text, out hours))
            {
                if (hours > 0.0 && hours <= 24.0)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsValidBillableHours()
        {
            double hours;
            if (string.IsNullOrEmpty(tbBillableHours.Text))
            {
                if (string.IsNullOrEmpty(tbNotes.Text) || !string.IsNullOrEmpty(tbNonBillableHours.Text))
                {
                    return true;
                }

                return false;
            }
            //  Check that hours is double between 0.0 and 24.0
            if (double.TryParse(tbBillableHours.Text, out hours))
            {
                if (hours > 0.0 && hours <= 24.0)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion


    }
}


using System;
using System.Drawing;
using System.Web.Security;
using System.Web.UI.WebControls;
using DataTransferObjects.TimeEntry;
using PraticeManagement.Utils;
using System.Collections.Generic;
using System.Xml.Linq;

namespace PraticeManagement.Controls.TimeEntry
{
    public partial class SingleTimeEntry_New : System.Web.UI.UserControl
    {
        #region Constants

        private const string DateBehindViewstate = "7555B3A7-8713-490F-8D5B-368A02E6A205";

        #endregion

        #region Properties

        public bool IsThereAtleastOneTimeEntryrecord
        {
            get
            {
                return ((!string.IsNullOrEmpty(tbNotes.Text)) || (!string.IsNullOrEmpty(tbActualHours.Text)));
            }
        }

        public bool IsPTO
        {
            get
            {
                return ViewState["isPTOWOrktype"] != null ? (bool)ViewState["isPTOWOrktype"] : false;
            }
            set
            {
                ViewState["isPTOWOrktype"] = value;
            }
        }

        public bool IsHoliday
        {
            get
            {
                return ViewState["IsHoliday"] != null ? (bool)ViewState["IsHoliday"] : false;
            }
            set
            {
                ViewState["IsHoliday"] = value;
            }
        }

        public XElement TimeEntryRecordElement
        {
            get;
            set;
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

        public string IsChargeCodeTurnOff
        {
            set
            {
                hdnIsChargeCodeTurnOff.Value = value;
            }
            get
            {
                return hdnIsChargeCodeTurnOff.Value;
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

        #region Control events

        protected void Page_Load(object sender, EventArgs e)
        {

            tbNotes.Attributes["imgNoteClientId"] = imgNote.ClientID;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            SpreadSheetTotalCalculatorExtenderId = HostingPage.SpreadSheetTotalCalculatorExtenderId;

            bool isChargeCodeTurnOff = false;
            Boolean.TryParse(IsChargeCodeTurnOff, out isChargeCodeTurnOff);
            if (isChargeCodeTurnOff)
            {
                if (!IsPostBack)
                {
                    tbActualHours.Attributes["isChargeCodeTurnOffDisable"] = "1";
                }
                tbActualHours.Enabled = false;
                tbActualHours.BackColor = Color.Gray;
            }
            tbActualHours.Attributes["IsHireDateDisable"] = HostingPage.SelectedPerson.HireDate > DateBehind ? "1" : "0";
            tbActualHours.Attributes["IsTerminationDateDisable"] = !HostingPage.SelectedPerson.TerminationDate.HasValue ||
                        (HostingPage.SelectedPerson.TerminationDate.HasValue &&
                                    HostingPage.SelectedPerson.TerminationDate.Value >= DateBehind
                         ) ? "0" : "1";
        }

        public void CanelControlStyle()
        {
            tbActualHours.BackColor = Color.White;
        }


        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (TimeEntryRecordElement != null)
            {
                FillControls();
            }

            if (IsPTO)
            {
                tbNotes.Enabled = false;
                btnSaveNotes.Enabled = false;
            }

            if (IsHoliday)
            {
                imgClear.Style["display"] = "none";
            }

            CanelControlStyle();
            ApplyControlStyle();

            tbActualHours.Attributes["IsPTO"] = IsPTO.ToString();
            tbActualHours.Attributes["txtboxNoteClienId"] = tbNotes.ClientID;

            MaintainEditedtbActualHoursStyle();
        }

        private void MaintainEditedtbActualHoursStyle()
        {
            if (tbActualHours.Style["background-color"] != "red")
            {
                tbActualHours.Style["background-color"] = (hfDirtyHours.Value == "dirty") ? "gold" : "none";
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

        private void FillControls()
        {
            EnsureChildControls();
            tbNotes.Text = TimeEntryRecordElement.Attribute(XName.Get("Note")).Value;
            hdnNotes.Value = TimeEntryRecordElement.Attribute(XName.Get("Note")).Value;
            imgNote.ImageUrl
                = string.IsNullOrEmpty(tbNotes.Text)
                      ? Constants.ApplicationResources.AddCommentIcon
                      : Constants.ApplicationResources.RecentCommentIcon;


            tbActualHours.Text = TimeEntryRecordElement.Attribute(XName.Get("ActualHours")).Value;
            hdnActualHours.Value = tbActualHours.Text;

            var isReviewd = TimeEntryRecordElement.Attribute(XName.Get("IsReviewed")).Value;
            lblReview.Text = isReviewd;
            if (isReviewd == ReviewStatus.Approved.ToString())
                Disabled = true;

            hfDirtyHours.Value = TimeEntryRecordElement.Attribute(XName.Get("IsDirty")).Value;

            imgNote.ToolTip = tbNotes.Text;


        }

        protected string GetNowDate()
        {
            return DateTime.Now.ToString(Constants.Formatting.EntryDateFormat);
        }

        private void ApplyControlStyle()
        {
            if (!tbActualHours.Enabled && string.IsNullOrEmpty(tbActualHours.Text))
                tbActualHours.BackColor = Color.Gray;
        }

        public bool Disabled
        {
            set
            {
                var enabled = !value;

                btnSaveNotes.Enabled =
                tbNotes.Enabled =
                tbActualHours.Enabled = enabled;
            }
        }

        public bool InActiveNotes
        {
            set
            {
                imgNote.Enabled = !value;
            }
        }

        internal void UpdateEditedValues(XElement element)
        {
            if (element.HasAttributes && element.Attribute(XName.Get("ActualHours")) != null)
            {
                element.Attribute(XName.Get("ActualHours")).Value = tbActualHours.Text;
                element.Attribute(XName.Get("Note")).Value = tbNotes.Text;
                element.Attribute(XName.Get("IsDirty")).Value = hfDirtyHours.Value;
            }
            else
            {
                var time = SettingsHelper.GetCurrentPMTime();
                element.SetAttributeValue(XName.Get("ActualHours"), tbActualHours.Text);
                element.SetAttributeValue(XName.Get("Note"), tbNotes.Text);
                element.SetAttributeValue(XName.Get("EntryDate"), time.ToString(Constants.Formatting.EntryDateFormat));
                element.SetAttributeValue(XName.Get("IsDirty"), hfDirtyHours.Value);
            }
        }

        internal void UpdateVerticalTotalCalculatorExtenderId(string clientId)
        {
            VerticalTotalCalculatorExtenderId = clientId;
        }

        internal void ValidateNoteAndHours()
        {
            var isValidNote = IsValidNote();
            var isValidHours = IsValidHours();
            var isValidPTOHours = true;

            if (IsPTO)
            {
                isValidPTOHours = IsValidPTOHours();
                if (!isValidPTOHours)
                    HostingPage.IsValidPTOHours = isValidPTOHours;
            }

            if (!isValidNote)
                HostingPage.IsValidNote = isValidNote;

            if (!isValidHours)
                HostingPage.IsValidHours = isValidHours;

            if (isValidNote && isValidHours && isValidPTOHours)
            {
                tbActualHours.Style["background-color"] = "none";
            }
            else
            {
                tbActualHours.Style["background-color"] = "red";
            }
        }

        private bool IsValidPTOHours()
        {
            double hours;
            if (string.IsNullOrEmpty(tbActualHours.Text))
            {
                if (string.IsNullOrEmpty(tbNotes.Text))
                {
                    return true;
                }

                return false;
            }
            //  Check that hours is double between 0.0 and 24.0
            if (double.TryParse(tbActualHours.Text, out hours))
            {
                if (hours > 0.0 && hours <= 8)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsValidNote()
        {
            imgNote.ImageUrl = string.IsNullOrEmpty(tbNotes.Text) ?
                        PraticeManagement.Constants.ApplicationResources.AddCommentIcon :
                        PraticeManagement.Constants.ApplicationResources.RecentCommentIcon;

            if (IsPTO)
                return true;

            var note = tbNotes.Text;

            if (hdnIsNoteRequired.Value.ToLowerInvariant() == "true" && !string.IsNullOrEmpty(tbActualHours.Text))
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

        private bool IsValidHours()
        {
            double hours;
            if (string.IsNullOrEmpty(tbActualHours.Text))
            {
                if (string.IsNullOrEmpty(tbNotes.Text))
                {
                    return true;
                }

                return false;
            }
            //  Check that hours is double between 0.0 and 24.0
            if (double.TryParse(tbActualHours.Text, out hours))
            {
                if (hours > 0.0 && hours <= 24.0)
                {
                    return true;
                }
            }
            return false;
        }


        internal void AddAttributeToPTOTextBox(string clientId)
        {
            tbActualHours.Attributes["HorizontalTotalCalculator"] = clientId;
        }

        #endregion


    }
}


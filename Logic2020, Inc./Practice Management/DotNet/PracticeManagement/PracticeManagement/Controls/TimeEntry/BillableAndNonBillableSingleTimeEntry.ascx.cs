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
            tbNotes.Attributes["imgNoteClientId"] = imgNote.ClientID;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            SpreadSheetTotalCalculatorExtenderId = HostingPage.SpreadSheetTotalCalculatorExtenderId;

            var IsChargeCodeOff = TimeEntryRecordNonBillableElement != null ? TimeEntryRecordNonBillableElement.Attribute(XName.Get("IsChargeCodeOff")) : null;
            if ((IsChargeCodeOff != null && IsChargeCodeOff.Value == 1.ToString()) || !Convert.ToBoolean(IsHourlyRevenue))
            {
                tbNonBillableHours.Attributes["disable"] = "1";
            }


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

            tbBillableHours.Attributes["IsPTO"] = IsPTO.ToString();
            tbBillableHours.Attributes["txtboxNoteClienId"] = tbNotes.ClientID;
            tbNonBillableHours.Attributes["IsPTO"] = IsPTO.ToString();
            tbNonBillableHours.Attributes["txtboxNoteClienId"] = tbNotes.ClientID;

            MaintainEditedtbHoursStyle();
        }

        private void MaintainEditedtbHoursStyle()
        {
            if (tbBillableHours.BackColor != Color.Red)
            {
                tbBillableHours.BackColor = (hfDirtyBillableHours.Value == "dirty") ? Color.Gold : Color.White;
            }

            if (tbNonBillableHours.BackColor != Color.Red)
            {
                tbNonBillableHours.BackColor = (hfDirtyNonBillableHours.Value == "dirty") ? Color.Gold : Color.White;
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
            tbNotes.Text = TimeEntryRecordBillableElement.Attribute(XName.Get("Note")).Value;
            hdnNotes.Value = TimeEntryRecordBillableElement.Attribute(XName.Get("Note")).Value;
            imgNote.ImageUrl
                = string.IsNullOrEmpty(tbNotes.Text)
                      ? Constants.ApplicationResources.AddCommentIcon
                      : Constants.ApplicationResources.RecentCommentIcon;


            tbBillableHours.Text = TimeEntryRecordBillableElement.Attribute(XName.Get("ActualHours")).Value;

            var IsChargeCodeOff = TimeEntryRecordBillableElement.Attribute(XName.Get("IsChargeCodeOff"));
            if (IsChargeCodeOff != null && IsChargeCodeOff.Value == 1.ToString())
            {
                tbBillableHours.Attributes["disable"] = "1";
            }
            hdnBillableHours.Value = tbBillableHours.Text;

            var isReviewd = TimeEntryRecordBillableElement.Attribute(XName.Get("IsReviewed")).Value;
            lblReview.Text = isReviewd;
            if (isReviewd == ReviewStatus.Approved.ToString())
                Disabled = true;

            hfDirtyBillableHours.Value = TimeEntryRecordBillableElement.Attribute(XName.Get("IsDirty")).Value;

            imgNote.ToolTip = tbNotes.Text;
        }

        private void FillNonBillableControls()
        {
            EnsureChildControls();
            tbNotes.Text = TimeEntryRecordNonBillableElement.Attribute(XName.Get("Note")).Value;
            hdnNotes.Value = TimeEntryRecordNonBillableElement.Attribute(XName.Get("Note")).Value;
            imgNote.ImageUrl
                = string.IsNullOrEmpty(tbNotes.Text)
                      ? Constants.ApplicationResources.AddCommentIcon
                      : Constants.ApplicationResources.RecentCommentIcon;

            tbNonBillableHours.Text = TimeEntryRecordNonBillableElement.Attribute(XName.Get("ActualHours")).Value;


            hdnNonBillableHours.Value = tbNonBillableHours.Text;

            var isReviewd = TimeEntryRecordNonBillableElement.Attribute(XName.Get("IsReviewed")).Value;
            lblReview.Text = isReviewd;
            if (isReviewd == ReviewStatus.Approved.ToString())
                Disabled = true;

            hfDirtyNonBillableHours.Value = TimeEntryRecordNonBillableElement.Attribute(XName.Get("IsDirty")).Value;

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
            if (element.HasAttributes && element.Attribute(XName.Get("ActualHours")) != null)
            {
                element.Attribute(XName.Get("ActualHours")).Value = tbBillableHours.Text;
                element.Attribute(XName.Get("Note")).Value = tbNotes.Text;
                element.Attribute(XName.Get("IsDirty")).Value = hfDirtyBillableHours.Value;
            }
            else
            {
                var time = SettingsHelper.GetCurrentPMTime();
                element.SetAttributeValue(XName.Get("ActualHours"), tbBillableHours.Text);
                element.SetAttributeValue(XName.Get("Note"), tbNotes.Text);
                element.SetAttributeValue(XName.Get("EntryDate"), time.ToString(Constants.Formatting.EntryDateFormat));
                element.SetAttributeValue(XName.Get("IsDirty"), hfDirtyBillableHours.Value);
            }
        }

        internal void UpdateNonBillableElementEditedValues(XElement element)
        {
            if (element.HasAttributes && element.Attribute(XName.Get("ActualHours")) != null)
            {
                element.Attribute(XName.Get("ActualHours")).Value = tbNonBillableHours.Text;
                element.Attribute(XName.Get("Note")).Value = tbNotes.Text;
                element.Attribute(XName.Get("IsDirty")).Value = hfDirtyNonBillableHours.Value;
            }
            else
            {
                var time = SettingsHelper.GetCurrentPMTime();
                element.SetAttributeValue(XName.Get("ActualHours"), tbNonBillableHours.Text);
                element.SetAttributeValue(XName.Get("Note"), tbNotes.Text);
                element.SetAttributeValue(XName.Get("EntryDate"), time.ToString(Constants.Formatting.EntryDateFormat));
                element.SetAttributeValue(XName.Get("IsDirty"), hfDirtyNonBillableHours.Value);
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
                HostingPage.IsValidNote = isValidNote;

            if (!(isValidBillableHours && isValidNonBillableHours))
                HostingPage.IsValidHours = isValidBillableHours && isValidNonBillableHours;

            if (isValidNote && isValidBillableHours && isValidNonBillableHours)
            {
                tbBillableHours.BackColor = tbBillableHours.BackColor != Color.Red ? tbBillableHours.BackColor : Color.White;
            }
            else
            {
                tbBillableHours.BackColor = Color.Red;
            }


            if (isValidNote && isValidBillableHours && isValidNonBillableHours)
            {
                tbNonBillableHours.BackColor = tbNonBillableHours.BackColor != Color.Red ? tbNonBillableHours.BackColor : Color.White;
            }
            else
            {
                tbNonBillableHours.BackColor = Color.Red;
            }

        }

        private bool IsValidNote()
        {
            imgNote.ImageUrl = string.IsNullOrEmpty(tbNotes.Text) ?
                        PraticeManagement.Constants.ApplicationResources.AddCommentIcon :
                        PraticeManagement.Constants.ApplicationResources.RecentCommentIcon;

            var note = tbNotes.Text;

            if (hdnIsNoteRequired.Value.ToLowerInvariant() == "true" && (!string.IsNullOrEmpty(tbBillableHours.Text) ||!string.IsNullOrEmpty(tbNonBillableHours.Text)) )
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


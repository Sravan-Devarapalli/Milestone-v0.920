using System;
using System.Drawing;
using System.Web.Security;
using System.Web.UI.WebControls;
using DataTransferObjects.TimeEntry;
using PraticeManagement.Utils;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
namespace PraticeManagement.Controls.TimeEntry
{
    public partial class AdministrativeSingleTimeEntry : System.Web.UI.UserControl
    {
        #region Constants

        private const string DateBehindViewstate = "7555B3A7-8713-490F-8D5B-368A02E6A205";
        private const string DefaultIdFieldName = "Id";
        private const string DefaultNameFieldName = "Name";
        private const string IsEmpDisableAttribute = "IsEmpDisable";
        private const string IsPersonSalaryTypeDisableAttribute = "IsPersonSalaryTypeDisable";
        private const string IsPersonHourlyTypeDisableAttribute = "IsPersonHourlyTypeDisable";
        private const string IsChargeCodeTurnOffDisableAttribute = "isChargeCodeTurnOffDisable";
        private const string ImgNoteClientIdAttribute = "imgNoteClientId";

        #endregion

        #region Properties

        public TimeTypeRecord TimeTypeRecord
        {
            get;
            set;
        }

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

        //IsHoliday gives information that is this a holiday project or not
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

        //IsHolidayDate gives information that is the selected day is company holiday or not.
        public bool IsHolidayDate
        {
            get
            {
                return ViewState["IsHolidayDate"] != null ? (bool)ViewState["IsHolidayDate"] : false;
            }
            set
            {
                ViewState["IsHolidayDate"] = value;
            }
        }


        public bool IsORT
        {
            get
            {
                return ViewState["IsORTWOrktype"] != null ? (bool)ViewState["IsORTWOrktype"] : false;
            }
            set
            {
                ViewState["IsORTWOrktype"] = value;
            }
        }

        public bool IsUnpaid
        {
            get
            {
                return ViewState["IsUnpaidWorktype"] != null ? (bool)ViewState["IsUnpaidWorktype"] : false;
            }
            set
            {
                ViewState["IsUnpaidWorktype"] = value;
            }
        }

        public bool IsSickLeave
        {
            get
            {
                return ViewState["IsSickLeaveWorktype"] != null ? (bool)ViewState["IsSickLeaveWorktype"] : false;
            }
            set
            {
                ViewState["IsSickLeaveWorktype"] = value;
            }
        }

        public XElement TimeEntryRecordElement
        {
            get;
            set;
        }

        public XElement ParentCalendarItem
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

        public string ApprovedByClientId
        {
            get
            {
                return tblApprovedby.ClientID;
            }
        }


        #endregion

        #region Control events

        protected void Page_Load(object sender, EventArgs e)
        {
            tbNotes.Attributes[ImgNoteClientIdAttribute] = imgNote.ClientID;
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
                    tbActualHours.Attributes[IsChargeCodeTurnOffDisableAttribute] = "1";
                }
                tbActualHours.Enabled = false;
                tbActualHours.BackColor = Color.Gray;
            }

            tbActualHours.Attributes[IsEmpDisableAttribute] = HostingPage.IsDateInPersonEmployeeHistoryList[DateBehind.Date] ? "0" : "1";
            tbActualHours.Attributes[IsPersonSalaryTypeDisableAttribute] = HostingPage.IsPersonSalaryHourlyTypeList.Where(t => t.First == DateBehind).First().Second ? "0" : "1";
            tbActualHours.Attributes[IsPersonHourlyTypeDisableAttribute] = HostingPage.IsPersonSalaryHourlyTypeList.Where(t => t.First == DateBehind).First().Third ? "0" : "1";
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

                HostingPage.AdminstratorSectionTargetHours[DateBehind].Value = tbActualHours.ClientID;
                HostingPage.AdminstratorSectionTargetNotes[DateBehind].Value = tbNotes.ClientID;

            }
            if (IsHoliday || IsUnpaid || IsHolidayDate)
            {
                imgClear.Style["display"] = "none";
            }
            else if (TimeTypeRecord != null)
            {
                if (TimeTypeRecord.IsW2SalaryAllowed && TimeTypeRecord.IsW2HourlyAllowed)
                {
                    tbActualHours.Enabled = true;
                }
                else if (TimeTypeRecord.IsW2SalaryAllowed)
                {
                    tbActualHours.Enabled = HostingPage.IsPersonSalaryHourlyTypeList.Where(t => t.First == DateBehind).First().Second
                                        && HostingPage.IsDateInPersonEmployeeHistoryList[DateBehind.Date];
                }
                else if (TimeTypeRecord.IsW2HourlyAllowed)
                {
                    tbActualHours.Enabled = HostingPage.IsPersonSalaryHourlyTypeList.Where(t => t.First == DateBehind).First().Third
                                        && HostingPage.IsDateInPersonEmployeeHistoryList[DateBehind.Date];
                }
            }

            tblApprovedby.Style["display"] = IsORT ? "" : "none";

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

            var selectedVal = TimeEntryRecordElement.Attribute(XName.Get("ApprovedById")).Value.ToLower();
            hdnApprovedManagerId.Value = TimeEntryRecordElement.Attribute(XName.Get("ApprovedById")).Value.ToLower();

            if (selectedVal != string.Empty)
            {
                ddlApprovedManagers.SelectedIndex = 0;
            }

            var selectedItem = ddlApprovedManagers.Items.FindByValue(selectedVal);

            if (selectedItem != null)
            {
                ddlApprovedManagers.SelectedValue = selectedVal;
            }
            else
            {
                var approvedByName = TimeEntryRecordElement.Attribute(XName.Get("ApprovedByName")).Value;
                selectedItem = new ListItem(approvedByName, selectedVal);
                ddlApprovedManagers.Items.Add(selectedItem);
                ddlApprovedManagers.SelectedValue = selectedVal;
            }

            if (selectedVal != string.Empty)
            {
                imgNote.ToolTip = tbNotes.Text + " Approved By " + ddlApprovedManagers.SelectedItem.Text + ".";
            }
            else
            {
                imgNote.ToolTip = tbNotes.Text;
            }


            var isReviewd = TimeEntryRecordElement.Attribute(XName.Get("IsReviewed")).Value;
            lblReview.Text = isReviewd;


            hfDirtyHours.Value = TimeEntryRecordElement.Attribute(XName.Get("IsDirty")).Value;

        }

        internal void DataBindApprovedManagers()
        {
            var managers = HostingPage.ApprovedManagers;
            DataHelper.FillApprovedManagersList(ddlApprovedManagers, "- - Select a Manager - -", managers, false);
            ddlApprovedManagers.AppendDataBoundItems = false;
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
                ddlApprovedManagers.Enabled =
                tbActualHours.Enabled = enabled;
            }
        }

        public bool DisabledReadonly
        {
            set
            {
                var enabled = !value;

                btnSaveNotes.Enabled =
                tbNotes.Enabled =
                ddlApprovedManagers.Enabled =
                tbActualHours.Enabled = enabled;

                if (!enabled)
                {
                    tbActualHours.Attributes["readonly"] = "readonly";
                    tbActualHours.Attributes["class"] = "bgColorWhiteImp";
                    
                }
                else
                {
                    tbActualHours.Attributes.Remove("class");
                    tbActualHours.Attributes.Remove("readonly");
                }
            }
        }

        public bool InActiveNotes
        {
            set
            {
                imgNote.Enabled = !value;
            }
        }

        internal void RoundActualHours(XElement element)
        {
            string hours = tbActualHours.Text;
            if (!string.IsNullOrEmpty(hours))
            {
                double _hours = Convert.ToDouble(hours);
                if (_hours % 0.25 < 0.125)
                {
                    _hours = _hours - _hours % 0.25;
                }
                else
                {
                    _hours = _hours + (0.25 - _hours % 0.25);
                }
                hours = _hours.ToString();
                element.Attribute(XName.Get("ActualHours")).Value = hours;
            }
        }

        internal void UpdateEditedValues(XElement element, bool isORT)
        {
            if (element.HasAttributes && element.Attribute(XName.Get("ActualHours")) != null)
            {
                element.Attribute(XName.Get("ActualHours")).Value = tbActualHours.Text;
                element.Attribute(XName.Get("Note")).Value = tbNotes.Text;
                element.Attribute(XName.Get("IsDirty")).Value = hfDirtyHours.Value;
                element.Attribute(XName.Get("ApprovedById")).Value = isORT ? ddlApprovedManagers.SelectedValue : string.Empty;
                element.Attribute(XName.Get("ApprovedByName")).Value = isORT ? ddlApprovedManagers.SelectedItem.Text : string.Empty;
            }
            else
            {
                var time = SettingsHelper.GetCurrentPMTime();
                element.SetAttributeValue(XName.Get("ActualHours"), tbActualHours.Text);
                element.SetAttributeValue(XName.Get("Note"), tbNotes.Text);
                element.SetAttributeValue(XName.Get("EntryDate"), time.ToString(Constants.Formatting.EntryDateFormat));
                element.SetAttributeValue(XName.Get("IsDirty"), hfDirtyHours.Value);
                element.SetAttributeValue(XName.Get("ApprovedById"), isORT ? ddlApprovedManagers.SelectedValue : string.Empty);
                element.SetAttributeValue(XName.Get("ApprovedByName"), isORT ? ddlApprovedManagers.SelectedItem.Text : string.Empty);
            }
        }

        internal void UpdateVerticalTotalCalculatorExtenderId(string clientId)
        {
            VerticalTotalCalculatorExtenderId = clientId;
        }

        internal void ValidateNoteAndHours(bool isORT)
        {
            var isValidNote = IsValidNote();
            var isValidApprovedManager = isORT ? IsValidApprovedManager() : true;
            var isValidAdminstrativeHours = true;
            var isValidHours = true;

            isValidAdminstrativeHours = IsValidAdminstrativeHours(isORT);
            if (!isValidAdminstrativeHours)
                HostingPage.IsValidAdminstrativeHours = isValidAdminstrativeHours;

            if (!isValidApprovedManager)
                HostingPage.IsValidApprovedManager = isValidApprovedManager;

            if (!isValidHours)
                HostingPage.IsValidHours = isValidHours;

            if (!isValidNote)
                HostingPage.IsValidNote = isValidNote;

            if (isValidNote && isValidHours && isValidAdminstrativeHours && isValidApprovedManager)
            {
                tbActualHours.Style["background-color"] = "none";
            }
            else
            {
                tbActualHours.Style["background-color"] = "red";
            }
        }

        private bool IsValidApprovedManager()
        {
            if (string.IsNullOrEmpty(tbActualHours.Text) && string.IsNullOrEmpty(tbNotes.Text))
            {
                return true;
            }
            if (ddlApprovedManagers.SelectedIndex == 0)
            {
                return false;
            }
            return true;
        }

        private bool IsValidAdminstrativeHours(bool isORT)
        {
            double hours;
            if (string.IsNullOrEmpty(tbActualHours.Text))
            {
                if (string.IsNullOrEmpty(tbNotes.Text))
                {
                    if (!isORT || (isORT && ddlApprovedManagers.SelectedIndex == 0))
                        return true;
                }

                return false;
            }
            //  Check that hours is double between 0.25 and 8.0
            if (double.TryParse(tbActualHours.Text, out hours))
            {
                if (hours > 0.25 && hours <= 8)
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

            if (!string.IsNullOrEmpty(tbNotes.Text) && (tbNotes.Text.Length < 3 || tbNotes.Text.Length > 1000))
            {
                return false;
            }

            return true;
        }

        internal void AddAttributeToPTOTextBox(string clientId)
        {
            tbActualHours.Attributes["HorizontalTotalCalculator"] = clientId;
        }

        #endregion
    }
}


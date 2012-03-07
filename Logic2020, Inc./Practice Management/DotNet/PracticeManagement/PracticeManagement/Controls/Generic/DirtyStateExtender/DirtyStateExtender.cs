using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;


[assembly: WebResource("PraticeManagement.Controls.Generic.DirtyStateExtender.DirtyStateExtender.js", "text/javascript")]

namespace PraticeManagement.Controls.Generic.DirtyStateExtender
{
    [ClientScriptResource(
        "PraticeManagement.Controls.Generic.DirtyStateExtender.DirtyStateBehavior",
        "PraticeManagement.Controls.Generic.DirtyStateExtender.DirtyStateExtender.js")]
    [TargetControlType(typeof(HiddenField))]
    public class DirtyStateExtender : ExtenderControlBase
    {
        #region Constants

        private const string NOTE_ID = "NoteIdValue";
        private const string HIDDEN_NOTE_ID = "HiddenNoteIdValue";
        private const string ACTUAL_HOURS_ID = "ActualHoursIdValue";
        private const string HIDDEN_ACTUAL_HOURS_ID = "HiddenActualHoursIdValue";
        private const string HORIZONTAL_TCEXTENDER_ID = "HorizontalTotalCalculatorExtenderIdValue";
        private const string VERTICAL_TCEXTENDER_ID = "VerticalTotalCalculatorExtenderIdValue";
        private const string SPREADSHEET_TCEXTENDER_ID = "SpreadSheetExtenderIdValue";
        private const string APPROVEDMANAGERSID_ID = "ApprovedManagersIdValue";
        private const string HIDDENAPPROVEDMANAGERSID_ID = "HiddenApprovedManagersIdValue";

        #endregion

        #region Properties

        [ExtenderControlProperty]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(TextBox))]
        [ClientPropertyName("NoteIdValue")]
        public string NoteId
        {
            get
            {
                return GetPropertyValue(NOTE_ID, string.Empty);
            }
            set
            {
                SetPropertyValue(NOTE_ID, value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(HiddenField))]
        [ClientPropertyName("HiddenNoteIdValue")]
        public string HiddenNoteId
        {
            get
            {
                return GetPropertyValue(HIDDEN_NOTE_ID, string.Empty);
            }
            set
            {
                SetPropertyValue(HIDDEN_NOTE_ID, value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(TextBox))]
        [ClientPropertyName("ActualHoursIdValue")]
        public string ActualHoursId
        {
            get
            {
                return GetPropertyValue(ACTUAL_HOURS_ID, string.Empty);
            }
            set
            {
                SetPropertyValue(ACTUAL_HOURS_ID, value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(HiddenField))]
        [ClientPropertyName("HiddenActualHoursIdValue")]
        public string HiddenActualHoursId
        {
            get
            {
                return GetPropertyValue(HIDDEN_ACTUAL_HOURS_ID, string.Empty);
            }
            set
            {
                SetPropertyValue(HIDDEN_ACTUAL_HOURS_ID, value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(HiddenField))]
        [ClientPropertyName("HorizontalTotalCalculatorExtenderIdValue")]
        public string HorizontalTotalCalculatorExtenderId
        {
            get
            {
                return GetPropertyValue(HORIZONTAL_TCEXTENDER_ID, string.Empty);
            }
            set
            {
                SetPropertyValue(HORIZONTAL_TCEXTENDER_ID, value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(HiddenField))]
        [ClientPropertyName("VerticalTotalCalculatorExtenderIdValue")]
        public string VerticalTotalCalculatorExtenderId
        {
            get
            {
                return GetPropertyValue(VERTICAL_TCEXTENDER_ID, string.Empty);
            }
            set
            {
                SetPropertyValue(VERTICAL_TCEXTENDER_ID, value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(HiddenField))]
        [ClientPropertyName("SpreadSheetExtenderIdValue")]
        public string SpreadSheetExtenderId
        {
            get
            {
                return GetPropertyValue(SPREADSHEET_TCEXTENDER_ID, string.Empty);
            }
            set
            {
                SetPropertyValue(SPREADSHEET_TCEXTENDER_ID, value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(HiddenField))]
        [ClientPropertyName("ApprovedManagersIdValue")]
        public string ApprovedManagersIdValue
        {
            get
            {
                return GetPropertyValue(APPROVEDMANAGERSID_ID, string.Empty);
            }
            set
            {
                SetPropertyValue(APPROVEDMANAGERSID_ID, value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(HiddenField))]
        [ClientPropertyName("HiddenApprovedManagersIdValue")]
        public string HiddenApprovedManagersIdValue
        {
            get
            {
                return GetPropertyValue(HIDDENAPPROVEDMANAGERSID_ID, string.Empty);
            }
            set
            {
                SetPropertyValue(HIDDENAPPROVEDMANAGERSID_ID, value);
            }
        }

        #endregion
    }
}

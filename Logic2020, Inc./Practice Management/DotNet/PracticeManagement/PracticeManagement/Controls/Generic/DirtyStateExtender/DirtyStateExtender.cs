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
        private const string TIMETYPE_DROPDOWN_ID = "TimeTypeDropdownIdValue";
        private const string PROJECT_MILESTONE_DROPDOWN_ID = "ProjectMilestoneDropdownIdValue";

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
        [ClientPropertyName("TimeTypeDropdownIdValue")]
        public string TimeTypeDropdownId
        {
            get
            {
                return GetPropertyValue(TIMETYPE_DROPDOWN_ID, string.Empty);
            }
            set
            {
                SetPropertyValue(TIMETYPE_DROPDOWN_ID, value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("ProjectMilestoneDropdownIdValue")]
        public string ProjectMilestoneDropdownIdValue
        {
            get
            {
                return GetPropertyValue(PROJECT_MILESTONE_DROPDOWN_ID, string.Empty);
            }
            set
            {
                SetPropertyValue(PROJECT_MILESTONE_DROPDOWN_ID, value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("true")]
        [IDReferenceProperty(typeof(HiddenField))]
        [ClientPropertyName("IsNoteRequired")]
        public string IsNoteRequired
        {
            get
            {
                return GetPropertyValue("IsNoteRequired", string.Empty);
            }
            set
            {
                SetPropertyValue("IsNoteRequired", value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("false")]
        [IDReferenceProperty(typeof(HiddenField))]
        [ClientPropertyName("IsPTOTimeType")]
        public string IsPTOTimeType
        {
            get
            {
                return GetPropertyValue("IsPTOTimeType", string.Empty);
            }
            set
            {
                SetPropertyValue("IsPTOTimeType", value);
            }
        }

        #endregion
    }
}

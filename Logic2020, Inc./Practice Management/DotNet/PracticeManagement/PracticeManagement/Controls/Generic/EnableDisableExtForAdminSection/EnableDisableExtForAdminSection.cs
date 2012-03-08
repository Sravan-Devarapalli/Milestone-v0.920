using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

[assembly: WebResource("PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSection.js", "text/javascript")]

namespace PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection
{
    [ClientScriptResource("PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSectionBehavior", "PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSection.js")]
    [TargetControlType(typeof(Control))]    
    public class EnableDisableExtForAdminSection : ExtenderControlBase
    {
        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("targetAcutalHoursHiddenFieldId")]
        public string TargetAcutalHoursHiddenFieldId
        {
            get
            {
                return GetPropertyValue("targetAcutalHoursHiddenFieldId", string.Empty);
            }
            set
            {
                SetPropertyValue("targetAcutalHoursHiddenFieldId", value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("targetNotesHiddenFieldId")]
        public string TargetNotesHiddenFieldId
        {
            get
            {
                return GetPropertyValue("targetNotesHiddenFieldId", string.Empty);
            }
            set
            {
                SetPropertyValue("targetNotesHiddenFieldId", value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("targetManagersHiddenFieldId")]
        public string TargetManagersHiddenFieldId
        {
            get
            {
                return GetPropertyValue("targetManagersHiddenFieldId", string.Empty);
            }
            set
            {
                SetPropertyValue("targetManagersHiddenFieldId", value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("hoursControlsToCheck")]
        public string HoursControlsToCheck
        {
            get
            {
                return GetPropertyValue("hoursControlsToCheck", string.Empty);
            }
            set
            {
                SetPropertyValue("hoursControlsToCheck", value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("notesControlsToCheck")]
        public string NotesControlsToCheck
        {
            get
            {
                return GetPropertyValue("notesControlsToCheck", string.Empty);
            }
            set
            {
                SetPropertyValue("notesControlsToCheck", value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("managersControlsToCheck")]
        public string ManagersControlsToCheck
        {
            get
            {
                return GetPropertyValue("managersControlsToCheck", string.Empty);
            }
            set
            {
                SetPropertyValue("managersControlsToCheck", value);
            }
        }


        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("hiddenNotesControlsToCheck")]
        public string HiddenNotesControlsToCheck
        {
            get
            {
                return GetPropertyValue("hiddenNotesControlsToCheck", string.Empty);
            }
            set
            {
                SetPropertyValue("hiddenNotesControlsToCheck", value);
            }
        }


        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("hiddenManagersControlsToCheck")]
        public string HiddenManagersControlsToCheck
        {
            get
            {
                return GetPropertyValue("hiddenManagersControlsToCheck", string.Empty);
            }
            set
            {
                SetPropertyValue("hiddenManagersControlsToCheck", value);
            }
        }

        
        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("deleteControlsToCheck")]
        public string DeleteControlsToCheck
        {
            get
            {
                return GetPropertyValue("deleteControlsToCheck", string.Empty);
            }
            set
            {
                SetPropertyValue("deleteControlsToCheck", value);
            }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("closeControlsToCheck")]
        public string CloseControlsToCheck
        {
            get
            {
                return GetPropertyValue("closeControlsToCheck", string.Empty);
            }
            set
            {
                SetPropertyValue("closeControlsToCheck", value);
            }
        }
        
    }
}

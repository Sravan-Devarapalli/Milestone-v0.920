using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

[assembly: WebResource("PraticeManagement.Controls.Generic.ElementDisabler.ElementDisablerExtender.js", "text/javascript")]

namespace PraticeManagement.Controls.Generic.ElementDisabler
{
    /// <summary>
    /// Extender that disables given ControlToDisable control after postback if
    /// postback was caused by the control which this extender extends.    
    /// </summary>
    [ClientScriptResource("PraticeManagement.Controls.Generic.ElementDisabler.ElementDisablerBehavior", "PraticeManagement.Controls.Generic.ElementDisabler.ElementDisablerExtender.js")]
    [TargetControlType(typeof(Button))]    
    public class ElementDisablerExtender : ExtenderControlBase
    {
        /// <summary>
        /// Name of the property in client JavaScript class.
        /// </summary>
        private const string ControlToDisableIdPropertyName = "controlToDisableId";

        /// <summary>
        /// Id of the control that must be disabled after postback.
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(Control))]
        [ClientPropertyName("controlToDisableId")]
        public string ControlToDisableID
        {
            get { return GetPropertyValue(ControlToDisableIdPropertyName, string.Empty); }
            set { SetPropertyValue(ControlToDisableIdPropertyName, value); }
        }
    }
}

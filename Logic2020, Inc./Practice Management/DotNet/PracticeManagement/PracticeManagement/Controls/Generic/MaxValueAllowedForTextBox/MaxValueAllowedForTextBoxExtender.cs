using System.Web.UI;
using AjaxControlToolkit;
[assembly: WebResource("PraticeManagement.Controls.Generic.MaxValueAllowedForTextBox.MaxValueAllowedForTextBoxExtender.js", "text/javascript")]
namespace PraticeManagement.Controls.Generic.MaxValueAllowedForTextBox
{
    using System.ComponentModel;
    using System;
    [ClientScriptResource("PraticeManagement.Controls.Generic.MaxValueAllowedForTextBox.MaxValueAllowedForTextBoxExtender", "PraticeManagement.Controls.Generic.MaxValueAllowedForTextBox.MaxValueAllowedForTextBoxExtender.js")]
    [TargetControlType(typeof(Control))]
    public class MaxValueAllowedForTextBoxExtender : ExtenderControlBase
    {
        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("controlsToCheck")]
        public string ControlsToCheck
        {
            get { return GetPropertyValue("controlsToCheck", string.Empty); }
            set { SetPropertyValue("controlsToCheck", value); }
        }

        [ExtenderControlProperty]
        [DefaultValue(Int32.MaxValue)]
        [ClientPropertyName("maximumValue")]
        public int MaximumValue
        {
            get { return GetPropertyValue("maximumValue", Int32.MaxValue); }
            set { SetPropertyValue("maximumValue", value); }
        }
    }
}


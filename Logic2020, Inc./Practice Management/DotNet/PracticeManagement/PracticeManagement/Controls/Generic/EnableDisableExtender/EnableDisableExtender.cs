using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("PraticeManagement.Controls.Generic.EnableDisableExtender.EnableDisableExtender.js", "text/javascript")]

namespace PraticeManagement.Controls.Generic.EnableDisableExtender
{
    using System.ComponentModel;

    [ClientScriptResource("PraticeManagement.Controls.Generic.EnableDisableExtender.EnableDisableExtenderBehavior", "PraticeManagement.Controls.Generic.EnableDisableExtender.EnableDisableExtender.js")]
    [TargetControlType(typeof(Control))]
    public class EnableDisableExtender : ExtenderControlBase
    {
        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("controlsToCheck")]
        public string ControlsToCheck
        {
            get { return GetPropertyValue("controlsToCheck", string.Empty); }
            set { SetPropertyValue("controlsToCheck", value); }
        }
    }
}


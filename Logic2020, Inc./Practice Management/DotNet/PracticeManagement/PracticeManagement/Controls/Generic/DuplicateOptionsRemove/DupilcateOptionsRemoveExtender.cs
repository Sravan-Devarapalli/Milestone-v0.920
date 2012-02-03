using System.Web.UI;
using AjaxControlToolkit;
[assembly: WebResource("PraticeManagement.Controls.Generic.DuplicateOptionsRemove.DupilcateOptionsRemoveExtender.js", "text/javascript")]
namespace PraticeManagement.Controls.Generic.DuplicateOptionsRemove
{
    using System.ComponentModel;
    using System;
    [ClientScriptResource("PraticeManagement.Controls.Generic.DuplicateOptionsRemove.DuplicateOptionsRemoveBehavior", "PraticeManagement.Controls.Generic.DuplicateOptionsRemove.DupilcateOptionsRemoveExtender.js")]
    [TargetControlType(typeof(Control))]
    public class DupilcateOptionsRemoveExtender : ExtenderControlBase
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
        [DefaultValue("")]
        [ClientPropertyName("plusButtonClientID")]
        public string PlusButtonClientID
        {
            get { return GetPropertyValue("plusButtonClientID", string.Empty); }
            set { SetPropertyValue("plusButtonClientID", value); }
        }
      
    }
}



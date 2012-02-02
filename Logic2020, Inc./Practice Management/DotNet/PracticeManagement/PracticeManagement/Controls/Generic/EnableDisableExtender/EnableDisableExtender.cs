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

        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("weekStartDate")]
        public string WeekStartDate
        {
            get { return GetPropertyValue("weekStartDate", string.Empty); }
            set { SetPropertyValue("weekStartDate", value); }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("accountId")]
        public string AccountId
        {
            get { return GetPropertyValue("accountId", string.Empty); }
            set { SetPropertyValue("accountId", value); }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("projectId")]
        public string ProjectId
        {
            get { return GetPropertyValue("projectId", string.Empty); }
            set { SetPropertyValue("projectId", value); }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("businessUnitId")]
        public string BusinessUnitId
        {
            get { return GetPropertyValue("businessUnitId", string.Empty); }
            set { SetPropertyValue("businessUnitId", value); }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("personId")]
        public string PersonId
        {
            get { return GetPropertyValue("personId", string.Empty); }
            set { SetPropertyValue("personId", value); }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("popUpBehaviourId")]
        public string PopUpBehaviourId
        {
            get { return GetPropertyValue("popUpBehaviourId", string.Empty); }
            set { SetPropertyValue("popUpBehaviourId", value); }
        }
        

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using AjaxControlToolkit;
using System.Web.UI.WebControls;
using System.ComponentModel;
using PraticeManagement.Utils;
[assembly: WebResource("PraticeManagement.Controls.Opportunities.ViewProjectExtender.ViewProjectExtender.js", "text/javascript")]

namespace PraticeManagement.Controls.Opportunities.ViewProjectExtender
{
    /// <summary>
    /// Extender that shows the link for selected project in the DropDownList
    /// </summary>
    [ClientScriptResource("PraticeManagement.Controls.Opportunities.ViewProjectExtender.ViewProjectBehavior", "PraticeManagement.Controls.Opportunities.ViewProjectExtender.ViewProjectExtender.js")]
    [TargetControlType(typeof(DropDownList))]
    public class ViewProjectExtender : ExtenderControlBase
    {
        private const string ControlToShowProjectLinkIDPropertyName = "controlToShowProjectLinkId";
        private const string EmptyValuePropertyName = "emptyValue";
        private const string ReturnToUrlPropertyName = "returnToUrl";

        /// <summary>
        /// Id of the control for the project navigation
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        [IDReferenceProperty(typeof(HyperLink))]
        [ClientPropertyName("controlToShowProjectLinkId")]
        public string ControlToShowProjectLinkID
        {
            get { return GetPropertyValue(ControlToShowProjectLinkIDPropertyName, string.Empty); }
            set { SetPropertyValue(ControlToShowProjectLinkIDPropertyName, value); }
        }

        /// <summary>
        /// Id of the control for the project navigation
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("-1")]
        [ClientPropertyName("emptyValue")]
        public string EmptyValue
        {
            get { return GetPropertyValue(EmptyValuePropertyName, string.Empty); }
            set { SetPropertyValue(EmptyValuePropertyName, value); }
        }

        /// <summary>
        /// url of the current Opportunity when we navigate to any project through "Navigate to Project"
        /// link in Opportunity Detail page.
        /// </summary>
        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("returnToUrl")]
        public string ReturnToUrl
        {
            get { return GetPropertyValue(ReturnToUrlPropertyName, string.Empty); }
            set { SetPropertyValue(ReturnToUrlPropertyName, value); }
        }
    }
}

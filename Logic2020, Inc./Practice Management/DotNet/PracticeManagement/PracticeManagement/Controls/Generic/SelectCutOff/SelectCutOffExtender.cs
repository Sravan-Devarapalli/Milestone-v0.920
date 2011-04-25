using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("PraticeManagement.Controls.Generic.SelectCutOff.SelectCutOffExtender.js", "text/javascript")]

namespace PraticeManagement.Controls.Generic.SelectCutOff
{
    using System.ComponentModel;

    /// <summary>
    /// Extender that fixes width of select element in IE.
    /// </summary>
    [ClientScriptResource("PraticeManagement.Controls.Generic.SelectCutOff.SelectCutOffBehavior", "PraticeManagement.Controls.Generic.SelectCutOff.SelectCutOffExtender.js")]
    [TargetControlType(typeof(Control))]
    public class SelectCutOffExtender : ExtenderControlBase
    {
        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("normalCssClass")]
        public string NormalCssClass
        {
            get { return GetPropertyValue("normalCssClass", string.Empty); }
            set { SetPropertyValue("normalCssClass", value); }
        }

        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("extendedCssClass")]
        public string ExtendedCssClass
        {
            get { return GetPropertyValue("extendedCssClass", string.Empty); }
            set { SetPropertyValue("extendedCssClass", value); }
        }
    }
}

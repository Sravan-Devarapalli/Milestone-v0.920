using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("PraticeManagement.Controls.Generic.SearchHighlighting.SearchHighlightingExtender.js", "text/javascript")]

namespace PraticeManagement.Controls.Generic.SearchHighlighting
{
    using System.ComponentModel;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Extender that fixes width of select element in IE.
    /// </summary>
    [ClientScriptResource("PraticeManagement.Controls.Generic.SearchHighlighting.SearchHighlightingBehavior", "PraticeManagement.Controls.Generic.SearchHighlighting.SearchHighlightingExtender.js")]
    [TargetControlType(typeof(TextBox))]
    public class SearchHighlightingExtender : ExtenderControlBase
    {
        [ExtenderControlProperty]
        [DefaultValue("")]
        [ClientPropertyName("searchInsideBlockId")]
        public string SearchInsideBlockId
        {
            get { return GetPropertyValue("SearchInsideBlockId", string.Empty); }
            set { SetPropertyValue("SearchInsideBlockId", value); }
        }
    }
}

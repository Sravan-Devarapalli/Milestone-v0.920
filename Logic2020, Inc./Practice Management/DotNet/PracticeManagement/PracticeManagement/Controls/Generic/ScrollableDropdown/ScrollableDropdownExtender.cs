using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("PraticeManagement.Controls.Generic.ScrollableDropdown.ScrollableDropdownExtender.js", "text/javascript")]

namespace PraticeManagement.Controls.Generic.ScrollableDropdown
{
    using System.ComponentModel;
    using System.Web.UI.WebControls;
    using System;
    using System.Web.UI.HtmlControls;
    using System.Drawing;

    /// <summary>
    /// Extender that fixes width of select element in IE.
    /// </summary>
    [ClientScriptResource("PraticeManagement.Controls.Generic.ScrollableDropdown.ScrollableDropdownBehavior", "PraticeManagement.Controls.Generic.ScrollableDropdown.ScrollableDropdownExtender.js")]
    [TargetControlType(typeof(ScrollingDropDown))]
    public class ScrollableDropdownExtender : ExtenderControlBase
    {
        #region Constants

        private const string Label_ID = "LabelIdValue";
        private const string DisplayText_Value = "DisplayTextValue";
        private const string EditImageUrl_Value = "EditImageUrlValue";
        private const string ExtenderWidth_Value = "ExtenderWidthValue";

        private Color _extenderBorderColor;

        #endregion
        
        [ExtenderControlProperty]
        [ClientPropertyName("labelValue")]
        [Browsable(false)]
        public string LabelId
        {
            get
            {
                return ID + "Label";
            }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("editImageUrl")]
        [DefaultValue("~/Images/Dropdown_Arrow.png")]
        public string EditImageUrl
        {
            get { return GetPropertyValue(EditImageUrl_Value, string.Empty); }
            set { SetPropertyValue(EditImageUrl_Value, value); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("displayTextValue")]
        public string DisplayText
        {
            get
            {
                if (UseAdvanceFeature)
                {
                    string selectedString = ((ScrollingDropDown)this.TargetControl).SelectedString;
                    return selectedString.Length > 32 ? selectedString.Substring(0, 30)+".." : selectedString;
                }
                else
                {
                    return GetPropertyValue(DisplayText_Value, string.Empty);
                }
            }
            set { SetPropertyValue(DisplayText_Value, value); }
        }

        public bool UseAdvanceFeature
        {
            get;
            set;
        }

        [ExtenderControlProperty]
        [ClientPropertyName("Width")]
        public string Width
        {
            get { return GetPropertyValue(ExtenderWidth_Value, string.Empty); }
            set { SetPropertyValue(ExtenderWidth_Value, value); }
        }

        protected override void OnInit(EventArgs e)
        {
            Load += Page_Load;
            base.OnInit(e);
        }

        protected virtual void Page_Load(object sender, EventArgs e)
        {
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            writer.WriteLine(GetHTMLToRender());
            base.RenderControl(writer);
        }

        private string GetHTMLToRender()
        {
            string htmlWithoutImageFormat = "<div ID={0} class=\"scrollextLabel\"{2}><label style=\"vertical-align:top;\">{1}</label></div>";
            string htmlWithImageFormat = "<div ID={0} class=\"scrollextLabel\"{3}><label style=\"vertical-align:top;\">{1}</label><span style=\"padding-left:20px;float:right;\"><image src={2} /></span></div>";

            if (!string.IsNullOrEmpty(EditImageUrl))
            {
                return string.Format(htmlWithImageFormat, LabelId, DisplayText, ResolveUrl(EditImageUrl), !string.IsNullOrEmpty(Width) ? "style=\"width:" + Width + "\"" : string.Empty);
            }
            else
            {
                return string.Format(htmlWithoutImageFormat, LabelId, DisplayText, !string.IsNullOrEmpty(Width) ? "style=\"width:" + Width + "\"" : string.Empty);
            }
        }
    }
}

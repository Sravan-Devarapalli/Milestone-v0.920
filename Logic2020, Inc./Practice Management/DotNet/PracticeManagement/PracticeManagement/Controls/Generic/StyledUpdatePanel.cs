using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web;
using System.Web.UI;

namespace PraticeManagement.Controls.Generic
{

    public class StyledUpdatePanel : UpdatePanel {

        [
        Category("Appearance"),
        Description("The CSS class applied to the UpdatePanel rendering")
        ]
        public string CssClass {
            get {
                string s = (string)ViewState["CssClass"];
                return (s != null) ? s : String.Empty;
            }
            set {
                ViewState["CssClass"] = value;
            }
        }

        protected override void RenderChildren(HtmlTextWriter writer) {
            if (IsInPartialRendering == false) {
                string cssClass = CssClass;
                if (cssClass.Length > 0) {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
                }
            }
            base.RenderChildren(writer);
        }
    }
}


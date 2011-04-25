using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web;
using System.Web.UI;

namespace PraticeManagement.Controls.Generic
{

    public class StyledUpdateProgress : UpdateProgress {

        [
        Category("Appearance"),
        Description("The CSS class applied to the UpdateProgress rendering")
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
        [
        Category("Appearance"),
        Description("The CSS style applied to the UpdateProgress rendering")
        ]
        public string Style
        {
            get
            {
                string s = (string)ViewState["Style"];
                return (s != null) ? s : String.Empty;
            }
            set
            {
                ViewState["Style"] = value;
            }
        }

        protected override void RenderChildren(HtmlTextWriter writer) 
        {
            if (!string.IsNullOrEmpty(CssClass)) 
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            if (!string.IsNullOrEmpty(Style))
                writer.AddAttribute(HtmlTextWriterAttribute.Style, Style);
            
            base.RenderChildren(writer);
        }
    }
}


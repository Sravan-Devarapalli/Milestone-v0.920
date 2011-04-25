using System.ComponentModel;
using System.Web.UI;
namespace PraticeManagement.Controls.Menu
{
    public class CssFriendlyMenu : System.Web.UI.WebControls.Menu
    {
        [
         Category("Appearance"),
         Description("The CSS class applied to the UpdatePanel rendering")
         ]
        public string CssClassPrefix
        {
            get
            {
                string s = (string)ViewState["CssClassPrefix"];
                return (s != null) ? s : string.Empty;
            }
            set
            {
                ViewState["CssClassPrefix"] = value;
            }
        }
    }
}

